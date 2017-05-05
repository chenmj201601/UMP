using Common1111;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UMPS1111.Wcf11111;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;

namespace UMPS1111
{
    /// <summary>
    /// Page1.xaml 的交互逻辑
    /// </summary>
    public partial class MainView
    {
        #region Memebers

        private ObservableCollection<ResourceInfo> ListResources = new ObservableCollection<ResourceInfo>();
        private List<ResourceInfo> Listresources = new List<ResourceInfo>();
        private List<Relation> ListRelation = new List<Relation>();
        private List<TenantInfo> ListTenant = new List<TenantInfo>();
        private List<NodeData> mListRootItems = new List<NodeData>();//存放树的所有节点内容
        ResourceInfo resource = new ResourceInfo();
        ResourceInfo DeleteResource = new ResourceInfo();
        string StarTime = string.Empty;
        string EndTime = string.Empty;
        //存放资源名称：录音服务器
        private List<ResourceNameNum> ListresourceName = new List<ResourceNameNum>();

        private BackgroundWorker mWorker;
        private NodeData mRoot;

        #endregion
        public S1111App S1111App;
        public MainView()
        {
            InitializeComponent();

            this.ButtonAdd.Click += ButtonAdd_Click;
            this.ButtonDelete.Click += ButtonDelete_Click;
            this.ComboResource.SelectionChanged += ComboResource_SelectionChanged;
            mRoot = new NodeData();
        }

        #region EventHandler

        private void OpenCloseLeftPanel()
        {
            if (GridLeft.Width.Value > 0)
            {
                GridLeft.Width = new GridLength(0);
            }
            else
            {
                GridLeft.Width = new GridLength(200);
            }
        }
        #endregion

        void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            if (DeleteResource.IsUsed)
            {
                MessageBoxResult MBR = S1111App.ShowYNMessage(CurrentApp.GetLanguageInfo("1111N005", "确定要删除该条记录吗？"));
                if (MBR.Equals(MessageBoxResult.Yes))
                {
                    if (DeleteRelation(DeleteResource))
                    {
                        ShowInformation(CurrentApp.GetLanguageInfo("1111N003", "delete ok"));
                        #region 记录日志
                        string msg = string.Format("{0}{1}{2}", DeleteResource.TenantName, Utils.FormatOptLogString("FO1111002"), DeleteResource.ResourceName);
                        CurrentApp.WriteOperationLog(S1111Consts.OPT_Delete.ToString(), ConstValue.OPT_RESULT_SUCCESS, msg);
                        #endregion
                    }
                    else
                    {
                        ShowInformation(CurrentApp.GetLanguageInfo("1111N004", "delete fail"));
                        #region 记录日志
                        string msg = string.Format("{0}{1}{2}", DeleteResource.TenantName, Utils.FormatOptLogString("FO1111002"), DeleteResource.ResourceName);
                        CurrentApp.WriteOperationLog(S1111Consts.OPT_Delete.ToString(), ConstValue.OPT_RESULT_FAIL, msg);
                        #endregion
                    }
                    //DeleteResource = new ResourceInfo();
                }
            }
            else
            {
                ShowInformation(CurrentApp.GetLanguageInfo("1111N009", "请选择需要删除的对象"));
            }
        }

        void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            OpenPanel();
        }

        void ComboResource_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadListBox();
        }

        protected override void Init()
        {
            try
            {
                this.ListBoxReources.ItemsSource = ListResources;
                this.ComboResource.ItemsSource = ListresourceName;
                this.TreeViewObjects.ItemsSource = mRoot.Children;

                PageName = "MainView";
                StylePath = "UMPS1111/MainPage.xaml";

                base.Init();
                if (CurrentApp != null)
                {
                    S1111App = CurrentApp as S1111App;
                }
                else
                {
                    S1111App = new S1111App(false);
                }
                ChangeTheme();
                ChangeLanguage();

                SetBusy(true, string.Empty);
                mWorker = new BackgroundWorker();
                mWorker.DoWork += (s, de) =>
                {
                     //触发Loaded消息
                    CurrentApp.SendLoadedMessage();

                    InitListresourceName();
                    LoadRentInfo();
                    LoadRelationInfo();
                };
                mWorker.RunWorkerCompleted += (s, re) =>
                {
                    mWorker.Dispose();
                    SetBusy(false, string.Empty);

                    LoadVCLogServerInfo();
                    LoadListBox();
                    InitTree();
                };
                mWorker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                CurrentApp.ShowExceptionMessage(ex.Message);
            }
        }

        private void InitListresourceName()
        {
            ListresourceName.Clear();
            ResourceNameNum rnn = new ResourceNameNum();
            rnn.ResourceNum = S1111Consts.VCLogServer;
            rnn.ResourceName = CurrentApp.GetLanguageInfo("111100001", "录音服务器");
            rnn.ResourceCode = S1111Consts.ResourcesCode_VCLog;
            ListresourceName.Add(rnn);
        }
        //初始化ListTenant
        public void LoadRentInfo()
        {
            ListTenant.Clear();
            WebRequest webRequestR = new WebRequest();
            webRequestR.Session = CurrentApp.Session;
            webRequestR.Code = (int)WebCodes.GetRentInfo;
            Service11111Client clientR = new Service11111Client(
               WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
               WebHelper.CreateEndpointAddress(
                   CurrentApp.Session.AppServerInfo,
                   "Service11111"));
            //Service1111Client clientR = new Service1111Client();
            WebHelper.SetServiceClient(clientR);
            WebReturn webReturnR = clientR.DoOperation(webRequestR);
            clientR.Close();
            if (!webReturnR.Result)
            {
                CurrentApp.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", webReturnR.Code, webReturnR.Message));
                return;
            }
            if (webReturnR.ListData == null)
            {
                CurrentApp.ShowExceptionMessage(string.Format("WSFail.\t{0}\t{1}", webReturnR.Code, webReturnR.Message));
                return;
            }
            OperationReturn optReturnR;
            for (int i = 0; i < webReturnR.ListData.Count; i++)
            {
                optReturnR = XMLHelper.DeserializeObject<TenantInfo>(webReturnR.ListData[i]);
                if (!optReturnR.Result)
                {
                    CurrentApp.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", optReturnR.Code, optReturnR.Message));
                    return;
                }
                TenantInfo info = optReturnR.Data as TenantInfo;
                if (info == null)
                {
                    CurrentApp.ShowExceptionMessage(string.Format("ResourcePropertyInfo is null"));
                    return;
                }
                info.RentName = S1111App.DecryptString(info.RentName);
                ListTenant.Add(info);
            }
        }

        //初始化ListRelation
        public void LoadRelationInfo()
        {
            ListRelation.Clear();
            WebRequest webRequestR = new WebRequest();
            webRequestR.Session = CurrentApp.Session;
            webRequestR.Code = (int)WebCodes.GetRelation;
            Service11111Client client = new Service11111Client(
               WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
               WebHelper.CreateEndpointAddress(
                   CurrentApp.Session.AppServerInfo,
                   "Service11111"));
            //Service1111Client client = new Service1111Client();
            WebHelper.SetServiceClient(client);
            WebReturn webReturnR = client.DoOperation(webRequestR);
            client.Close();
            if (!webReturnR.Result)
            {
                CurrentApp.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", webReturnR.Code, webReturnR.Message));
                return;
            }
            if (webReturnR.ListData == null)
            {
                CurrentApp.ShowExceptionMessage(string.Format("WSFail.\t{0}\t{1}", webReturnR.Code, webReturnR.Message));
                return;
            }
            OperationReturn optReturnR;
            for (int i = 0; i < webReturnR.ListData.Count; i++)
            {
                optReturnR = XMLHelper.DeserializeObject<Relation>(webReturnR.ListData[i]);
                if (!optReturnR.Result)
                {
                    CurrentApp.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", optReturnR.Code, optReturnR.Message));
                    return;
                }
                Relation info = optReturnR.Data as Relation;
                if (info == null)
                {
                    CurrentApp.ShowExceptionMessage(string.Format("ResourcePropertyInfo is null"));
                    return;
                }
                ListRelation.Add(info);
            }
        }
        //初始化Listresources
        public void LoadVCLogServerInfo()
        {
            Listresources.Clear();
            WebRequest webRequest = new WebRequest();
            webRequest.Session = CurrentApp.Session;
            webRequest.Code = (int)WebCodes.GetVoiceIP_Name201;
            Service11111Client client = new Service11111Client(
               WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
               WebHelper.CreateEndpointAddress(
                   CurrentApp.Session.AppServerInfo,
                   "Service11111"));
            //Service1111Client client = new Service1111Client();
            WebHelper.SetServiceClient(client);
            WebReturn webReturn = client.DoOperation(webRequest);
            client.Close();
            if (!webReturn.Result)
            {
                CurrentApp.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                return;
            }
            if (webReturn.ListData == null)
            {
                CurrentApp.ShowExceptionMessage(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                return;
            }
            OperationReturn optReturn;
            for (int i = 0; i < webReturn.ListData.Count; i++)
            {
                optReturn = XMLHelper.DeserializeObject<ResourceInfo>(webReturn.ListData[i]);
                if (!optReturn.Result)
                {
                    CurrentApp.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                ResourceInfo info = optReturn.Data as ResourceInfo;
                if (info == null)
                {
                    CurrentApp.ShowExceptionMessage(string.Format("ResourcePropertyInfo is null"));
                    return;
                }

                long InfoID = info.ResourceID;
                for (int j = 0; j < ListRelation.Count; j++)
                {
                    if (ListRelation[j].ResourceID == InfoID)
                    {
                        info.IsUsed = true;
                        info.TenantID = ListRelation[j].UserID;
                        for (int k = 0; k < ListTenant.Count; k++)
                        {
                            if (ListTenant[k].RentID == ListRelation[j].UserID)
                            {
                                info.TenantName = ListTenant[k].RentName;
                            }
                        }
                    }
                }
                info.Description = info.ToString();
                Listresources.Add(info);
            }
            CurrentApp.WriteLog("PageLoad", string.Format("Init ResourceInfo"));
        }

        //初始化listbox
        public void LoadListBox()
        {
            ListResources.Clear();
            var ResourceTemp = this.ComboResource.SelectedItem;
            long ResourceCode = 0;
            if (ResourceTemp != null)
            {
                ResourceCode = (ResourceTemp as ResourceNameNum).ResourceNum;
            }
            List<ResourceInfo> ResourceTempList = Listresources.Where(s => s.ResourceCode == ResourceCode).ToList();
            foreach (ResourceInfo RI in ResourceTempList)
            {
                ListResources.Add(RI);
            }
        }

        #region tree
        public void InitTree()
        {
            mListRootItems.Clear();
            mRoot.Children.Clear();

            for (int i = 0; i < ListTenant.Count; i++)
            {
                TenantInfo TI = ListTenant[i];
                NodeData RootNode = new NodeData();
                RootNode.Parent = mRoot;
                RootNode.Data = TI;
                RootNode.Name = TI.RentName;
                RootNode.Description = TI.ToString();
                RootNode.ObjID = TI.RentID;
                RootNode.Type = S1111Consts.Rent;
                RootNode.Icon = "Images/root.ico";
                //mRoot.AddChild(RootNode);
                Dispatcher.Invoke(new Action(() => mRoot.AddChild(RootNode)));
                InitResourceName(RootNode, TI.RentID);
                mListRootItems.Add(RootNode);
            }
        }

        private void InitResourceName(NodeData RootNode, long RentID)
        {
            for (int i = 0; i < ListresourceName.Count; i++)
            {
                ResourceNameNum RN = ListresourceName[i];
                NodeData ResourceNameNode = new NodeData();
                ResourceNameNode.Parent = mRoot;
                ResourceNameNode.Data = RN;
                ResourceNameNode.Name = RN.ResourceName;
                ResourceNameNode.Description = RN.ToString();
                ResourceNameNode.ObjID = RN.ResourceNum;
                ResourceNameNode.Type = S1111Consts.Resource;
                ResourceNameNode.Icon = "Images/voiceserver.png";

                Dispatcher.Invoke(new Action(() => RootNode.AddChild(ResourceNameNode)));
                InitResource(ResourceNameNode, RentID);
                mListRootItems.Add(ResourceNameNode);
            }
        }

        private void InitResource(NodeData ResourceNameNode, long RentID)
        {
            //对resourceinfo类的list进行筛选查询，然后绑给child
            ResourceNameNode.Children.Clear();
            List<Relation> TempList = ListRelation.Where(s => s.UserID == RentID).ToList();

            for (int i = 0; i < TempList.Count; i++)
            {
                NodeData ValueNode = new NodeData();
                switch (ResourceNameNode.ObjID)
                {
                    case S1111Consts.VCLogServer:
                        ResourceInfo tempRI = Listresources.FirstOrDefault(p => p.ResourceID == TempList[i].ResourceID);
                        if (tempRI != null)
                        {
                            string Name = tempRI.Tostring();

                            ValueNode.Parent = ResourceNameNode;
                            ValueNode.Data = tempRI;
                            ValueNode.Name = tempRI.Tostring();
                            ValueNode.Description = string.Format(TempList[i].ToString(), CurrentApp.GetLanguageInfo("1111002", "有效时段"));
                            ValueNode.ObjID = tempRI.ResourceID;
                            ValueNode.Type = S1111Consts.ResourceObject;
                            ValueNode.Icon = "Images/voiceserver.png";
                        }
                        break;
                }
                if (ValueNode.Name != null)
                {
                    Dispatcher.Invoke(new Action(() => ResourceNameNode.AddChild(ValueNode)));
                    mListRootItems.Add(ValueNode);
                }
            }
        }
        #endregion

        #region ChangTheme

        public override void ChangeTheme()
        {
            base.ChangeTheme();

            bool bPage = false;
            if (AppServerInfo != null)
            {
                //优先从服务器上加载资源文件
                try
                {
                    string uri = string.Format("{0}://{1}:{2}/Themes/{3}/{4}",
                        AppServerInfo.Protocol,
                        AppServerInfo.Address,
                        AppServerInfo.Port,
                        ThemeInfo.Name
                        , StylePath);
                    ResourceDictionary resource = new ResourceDictionary();
                    resource.Source = new Uri(uri, UriKind.RelativeOrAbsolute);
                    Resources.MergedDictionaries.Add(resource);
                    bPage = true;
                }
                catch (Exception)
                {
                    //App.ShowExceptionMessage("1" + ex.Message);
                }
            }
            if (!bPage)
            {
                //如果通过Url没有加载成功，就从已经编译到程序集的默认资源加载
                try
                {
                    string uri = string.Format("/UMPS1111;component/Themes/{0}/{1}",
                        "Default"
                        , StylePath);
                    ResourceDictionary resource = new ResourceDictionary();
                    resource.Source = new Uri(uri, UriKind.RelativeOrAbsolute);
                    Resources.MergedDictionaries.Add(resource);
                }
                catch (Exception ex)
                {
                    //App.ShowExceptionMessage("2" + ex.Message);
                }
            }
        }

        #endregion

        //......ing
        #region ChangLanguage
        public override void ChangeLanguage()
        {
            try
            {
                base.ChangeLanguage();
                CurrentApp.AppTitle = CurrentApp.GetLanguageInfo("FO1111", "UMP Rent Management");
                LabRent.Content = CurrentApp.GetLanguageInfo("1111L001", "Rent");
                ButtonAdd.Content = CurrentApp.GetLanguageInfo("1111B001", "Add");
                ButtonDelete.Content = CurrentApp.GetLanguageInfo("1111B002", "Delete");
                if (this.PopupPanel.IsOpen)
                {
                    PopupPanel.ChangeLanguage();
                }
                InitListresourceName();
                InitTree();
            }
            catch (Exception ex)
            { }
        }

        #endregion

        #region Panel
        private void ListBoxItem_MouseDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                ResourceInfo RI = new ResourceInfo();
                RI = ListBoxReources.SelectedItem as ResourceInfo;
                if (RI == null) { return; }
                if (resource != null)
                {
                    if (resource.TenantName != null && !RI.IsUsed)
                    {
                        RI.TenantName = resource.TenantName;
                        RI.TenantID = resource.TenantID;
                    }
                }
                resource = RI;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void OpenPanel()
        {
            try
            {
                if (resource != null)
                {
                    if (resource.TenantName != null)
                    {
                        if (resource.ResourceName != null && resource.IsUsed == false)
                        {
                            PopupPanel.Title = CurrentApp.GetLanguageInfo("1111T001", "Add Resource");
                            SettingPage Settingpage = new SettingPage();
                            Settingpage.ParentPage = this;
                            Settingpage.CurrentApp = S1111App;
                            Settingpage.resource = resource;
                            PopupPanel.Content = Settingpage;
                            PopupPanel.IsOpen = true;
                        }
                        else
                        {
                            ShowInformation(CurrentApp.GetLanguageInfo("1111N008", "请选择需要添加的资源对象"));
                        }
                    }
                    else
                    {
                        ShowInformation(CurrentApp.GetLanguageInfo("1111N007", "请选择需要添加的租户对象"));
                    }
                }
            }catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                NodeData ND = new NodeData();
                ND = TreeViewObjects.SelectedItem as NodeData;
                if (ND == null) { return; }
                if (ND.Type == 1)
                {
                    if (resource == null)
                    {
                        resource = new ResourceInfo();
                    }
                    resource.TenantName = ND.Name;
                    resource.TenantID = ND.ObjID;
                }
                else if (ND.Type == 3)
                {
                    for (int i = 0; i < Listresources.Count; i++)
                    {
                        if (ND.ObjID == Listresources[i].ResourceID)
                        {
                            DeleteResource = Listresources[i];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region save & delete
        public bool SaveRelation(ResourceInfo RI, string ST, string ET)
        {
            bool IsSaved = true;
            //long rentID=ListTenant.FirstOrDefault(p=>p.RentName==RI.TenantName).RentID;
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)WebCodes.Relation;
                webRequest.ListData.Add(RI.TenantID.ToString());
                webRequest.ListData.Add(RI.ResourceID.ToString());
                webRequest.ListData.Add(ST);
                webRequest.ListData.Add(ET);
                Service11111Client client = new Service11111Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(
                        CurrentApp.Session.AppServerInfo,
                        "Service11111"));
                //Service1111Client client = new Service1111Client();
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    CurrentApp.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    IsSaved = false;
                }
                if (IsSaved)
                {
                    //更新listRelation
                    Relation relation = new Relation();
                    relation.ResourceID = RI.ResourceID;
                    relation.UserID = RI.TenantID;
                    relation.StartTime = Convert.ToDateTime(ST);
                    relation.EndTime = Convert.ToDateTime(ET);
                    ListRelation.Add(relation);
                    //更新listResources，更新界面显示
                    for (int i = 0; i < Listresources.Count; i++)
                    {
                        if (Listresources[i].ResourceID == RI.ResourceID && Listresources[i].TenantName != null && Listresources[i].IsUsed == false)
                        {
                            Listresources[i].IsUsed = true;
                            Listresources[i].Description = Listresources[i].ToString();
                        }
                    }
                    LoadListBox();
                    AddTreeNode(RI);
                }
                resource = new ResourceInfo();
            }
            catch (Exception ex)
            {
                CurrentApp.ShowExceptionMessage(ex.Message);
                IsSaved = false;
            }
            return IsSaved;
        }

        public bool DeleteRelation(ResourceInfo RI)
        {
            bool IsDelete = true;
            //long rentID=ListTenant.FirstOrDefault(p=>p.RentName==RI.TenantName).RentID;
            try
            {
                WebRequest webRequest = new VoiceCyber.UMP.Communications.WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)WebCodes.Relation;
                webRequest.ListData.Add(RI.TenantID.ToString());
                webRequest.ListData.Add(RI.ResourceID.ToString());
                Service11111Client client = new Service11111Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(
                        CurrentApp.Session.AppServerInfo,
                        "Service11111"));
                //Service1111Client client = new Service1111Client();
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    CurrentApp.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    IsDelete = false;
                }
                if (IsDelete)
                {
                    //更新listRelation
                    for (int i = 0; i < ListRelation.Count; i++)
                    {
                        if (ListRelation[i].ResourceID == RI.ResourceID && ListRelation[i].UserID == RI.TenantID)
                        {
                            //删掉该条记录
                            ListRelation.RemoveAt(i);
                        }
                    }

                    //更新listResources,更新界面显示
                    for (int i = 0; i < Listresources.Count; i++)
                    {
                        if (Listresources[i].ResourceID == RI.ResourceID && Listresources[i].TenantName != null && Listresources[i].IsUsed == true)
                        {
                            Listresources[i].IsUsed = false;
                            Listresources[i].Description = Listresources[i].ToString();
                        }
                    }
                    LoadListBox();
                    DeleteTreeNode(RI);
                }
                DeleteResource = new ResourceInfo();
            }
            catch (Exception ex)
            {
                CurrentApp.ShowExceptionMessage(ex.Message);
                IsDelete = false;
            }
            return IsDelete;
        }
        #endregion

        #region 修改树
        private void AddTreeNode(ResourceInfo RI)
        {
            foreach (NodeData ND in mRoot.Children)
            {
                if (ND.ObjID == RI.TenantID)
                {
                    foreach (NodeData nd in ND.Children)
                    {
                        if (nd.ObjID == RI.ResourceCode)
                        {
                            InitResource(nd, RI.TenantID);
                        }
                    }
                }
            }
        }

        private void DeleteTreeNode(ResourceInfo RI)
        {
            foreach (NodeData ND in mRoot.Children)
            {
                if (ND.ObjID == RI.TenantID)
                {
                    for (int i = 0; i < ND.Children.Count; i++)
                    {
                        NodeData nd = ND.Children[i] as NodeData;
                        if (nd.ObjID == RI.ResourceCode)
                        {
                            for (int j = 0; j < ND.Children[i].Children.Count; j++)
                            //foreach (NodeData node in nd.Children)
                            {
                                NodeData node = ND.Children[i].Children[j] as NodeData;
                                if (node.ObjID == RI.ResourceID)
                                {
                                    ND.Children[i].RemoveChild(node);
                                    mListRootItems.Remove(node);
                                }
                            }
                        }
                    }
                }
            }
        }
        #endregion
    }
}
