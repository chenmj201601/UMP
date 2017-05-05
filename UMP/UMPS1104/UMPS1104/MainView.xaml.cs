using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;
using VoiceCyber.UMP.Common;
using System.Collections.ObjectModel;
using UMPS1104.Models;
using System.Data;
using UMPS1104.WCFService00000;
using System.ServiceModel;
using VoiceCyber.UMP.Controls.Wcf11012;
using VoiceCyber.Common;

namespace UMPS1104
{
    /// <summary>
    /// Page1.xaml 的交互逻辑
    /// </summary>
    public partial class MainView
    {
        public BackgroundWorker mWorker;

        public string ModelCode = string.Empty;
        private ObservableCollection<OperationInfo> ListOpt;
        private ObservableCollection<ObjectItem> ListObjItems;
        private ObservableCollection<ViewColumnInfo> mListGridTreeColumns;
        private ObjectItem mRoot;
        private ObjectItem TreeVItem;
        private UCSkillGroup UCSG;
        private UCOrgType UCOT;
        private string OPT_Code;
        private S1104App S1104app;

        #region 保存数据至数据库
        public static BackgroundWorker IBWSaveData = null;
        public static bool IBoolCallReturn = true;
        public static string IStrCallReturn = string.Empty;
        public static List<string> IListStrAfterSave = new List<string>();
        public static bool IBoolIsBusy = false;
        #endregion

        public MainView()
        {
            InitializeComponent();

            ModelCode = S1104App.CurrentLoadingModule;
            ListOpt = new ObservableCollection<OperationInfo>();
            ListObjItems = new ObservableCollection<ObjectItem>();
            mRoot = new ObjectItem();
            mListGridTreeColumns = new ObservableCollection<ViewColumnInfo>();

            this.TreeViewOrgAgent.ItemsSource = mRoot.Children;
            this.TreeViewOrgAgent.SelectedItemChanged += TreeViewOrgAgent_SelectedItemChanged;
            this.TvObjects.ItemsSource = mRoot.Children;
            this.TvObjects.SelectedItemChanged += TvObjects_SelectedItemChanged;
        }

        void TvObjects_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                TreeVItem = TvObjects.SelectedItem as ObjectItem;
                if (TreeVItem == null)
                {
                    return;
                }
                //ShowObjectDetail();
            }
            catch { }
        }

        void TreeViewOrgAgent_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                TreeVItem = TreeViewOrgAgent.SelectedItem as ObjectItem;
                if (TreeVItem == null)
                {
                    return;
                }
                //ShowObjectDetail();
            }
            catch { }
        }

        #region Init and Load
        protected override void Init()
        {
            try
            {
                PageName = "MainView";
                StylePath = "UMPS1104/MainPage.xaml";
                //ModelCode = CurrentApp.LoadingMessageReturn.Data;

                base.Init();

                ChangeTheme();
                InitColumnData();
                ChangeLanguage();
                //GetShowInformation();
                //SetBusy(true);
                mRoot.Children.Clear();
                GetShowInformation();
                mWorker = new BackgroundWorker();
                mWorker.DoWork += (s, de) =>
                {
                    //触发Loaded消息
                    CurrentApp.SendLoadedMessage();
                };
                mWorker.RunWorkerCompleted += (s, re) =>
                {
                    mWorker.Dispose();
                    SetBusy(false, string.Empty);
                    //InitOperation();
                    InitColumns();
                    //ShowObjectDetail();
                };
                mWorker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }
        #endregion

        #region  EventHandlers

        //protected override void PageHead_PageHeadEvent(object sender, PageHeadEventArgs e)
        //{
        //    base.PageHead_PageHeadEvent(sender, e);
        //    switch (e.Code)
        //    {
        //        //切换主题
        //        case 100:
        //            ThemeInfo themeInfo = e.Data as ThemeInfo;
        //            if (themeInfo != null)
        //            {
        //                ThemeInfo = themeInfo;
        //                CurrentApp.Session.ThemeInfo = themeInfo;
        //                CurrentApp.Session.ThemeName = themeInfo.Name;
        //                ChangeTheme();
        //                SendThemeChangeMessage();
        //            }
        //            break;
        //        //切换语言
        //        case 110:
        //            LangTypeInfo langType = e.Data as LangTypeInfo;
        //            if (langType != null)
        //            {
        //                LangTypeInfo = langType;
        //                CurrentApp.Session.LangTypeInfo = langType;
        //                CurrentApp.Session.LangTypeID = langType.LangID;
        //                SetBusy(true);
        //                mWorker = new BackgroundWorker();
        //                mWorker.DoWork += (s, de) => CurrentApp.InitAllLanguageInfos();
        //                mWorker.RunWorkerCompleted += (s, re) =>
        //                {
        //                    mWorker.Dispose();
        //                    SetBusy(false);
        //                    ChangeLanguage();

        //                    PopupPanel.ChangeLanguage();

        //                    SendLanguageChangeMessage();
        //                };
        //                mWorker.RunWorkerAsync();
        //            }
        //            break;
        //        //展开或关闭侧边栏
        //        case 121:
        //            //OpenCloseLeftPanel();
        //            break;
        //        case 202:
        //            SendNavigateHomeMessage();
        //            break;
        //    }
        //}

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
                    //("1" + ex.Message);
                }
            }
            if (!bPage)
            {
                //如果通过Url没有加载成功，就从已经编译到程序集的默认资源加载
                try
                {
                    string uri = string.Format("/UMPS1104;component/Themes/{0}/{1}",
                        "Default"
                        , StylePath);
                    ResourceDictionary resource = new ResourceDictionary();
                    resource.Source = new Uri(uri, UriKind.RelativeOrAbsolute);
                    Resources.MergedDictionaries.Add(resource);
                }
                catch (Exception ex)
                {
                    //("2" + ex.Message);
                }
            }

            //固定资源(有些资源包含转换器，命令等自定义类型，
            //这些资源不能通过url来动态加载，他只能固定的编译到程序集中
            try
            {
                string uri = string.Format("/UMPS1104;component/Themes/Default/UMPS1104/MainPageStatic.xaml");
                ResourceDictionary resource = new ResourceDictionary();
                resource.Source = new Uri(uri, UriKind.RelativeOrAbsolute);
                Resources.MergedDictionaries.Add(resource);
            }
            catch (Exception ex)
            {
                //("3" + ex.Message);
            }
        }

        #endregion

        #region ChangLanguage
        public override void ChangeLanguage()
        {
            base.ChangeLanguage();

            if (this.PopupPanel.IsOpen)
            {
                this.PopupPanel.Title = CurrentApp.GetLanguageInfo("1104005", "");
                this.PopupPanel.ChangeLanguage();
            }
            if (CurrentApp.StartArgs == "1105")
            {
                this.LabTreeHeard.Content = CurrentApp.GetLanguageInfo("1104001", "");
            }
            else
            {
                this.LabTreeHeard.Content = CurrentApp.GetLanguageInfo("1104002", "");
            }
            this.TabSampleView.Header = CurrentApp.GetLanguageInfo("1104016", "");
            this.TabGridView.Header = CurrentApp.GetLanguageInfo("1104017", "");
            InitOperations();
            this.ExpBasicOpt.Header = CurrentApp.GetLanguageInfo("1104006", "jichucaozuo");
            InitColumns();
        }
        #endregion
        private void InitOperations()
        {
            ListOpt.Clear();
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)RequestCode.WSGetUserOptList;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                webRequest.ListData.Add("11");
                webRequest.ListData.Add(ModelCode);
                Service11012Client client = new Service11012Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11012"));
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                webReturn.ListData.Sort();
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<OperationInfo>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    OperationInfo roleInfo = optReturn.Data as OperationInfo;
                    if (roleInfo != null)
                    {
                        roleInfo.Display = CurrentApp.GetLanguageInfo("FO" + roleInfo.ID, roleInfo.Display);
                        ListOpt.Add(roleInfo);
                    }
                }
                CurrentApp.WriteLog("PageInit", string.Format("InitOperations"));
                CreatButton();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }
        private void InitOperation()
        {
            ListOpt.Clear();
            OperationInfo item;
            //添加
            item = new OperationInfo();
            item.ID = 1104001;
            //item.ID = 1105001;
            item.Icon = "Images/add.png";
            item.Display = CurrentApp.GetLanguageInfo("1104003", "add");
            ListOpt.Add(item);

            item = new OperationInfo();
            item.ID = 1104002;
            //item.ID = 1105002;
            item.Icon = "Images/remove.png";
            item.Display = CurrentApp.GetLanguageInfo("1104004", "remove");
            ListOpt.Add(item);

            item = new OperationInfo();
            item.ID = 1104003;
            //item.ID = 1105003;
            item.Icon = "Images/setting.png";
            item.Display = CurrentApp.GetLanguageInfo("1104005", "set");
            ListOpt.Add(item);

            CreatButton();
        }

        private void CreatButton()
        {
            this.PanelBasicOpts.Children.Clear();
            for (int i = 0; i < ListOpt.Count; i++)
            {
                OperationInfo item = ListOpt[i];
                //基本操作按钮
                Button btn = new Button();
                btn.Click += ButtonRight_Click;
                btn.DataContext = item;
                btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                PanelBasicOpts.Children.Add(btn);
            }
        }

        private void ButtonRight_Click(object sender, RoutedEventArgs e)
        {
            //三个按钮的触发后内容：add、delete、modify
            var btn = e.Source as Button;
            if (btn != null)
            {
                var optItem = btn.DataContext as OperationInfo;
                if (optItem != null)
                {
                    switch (optItem.ID)
                    {
                        case 1104001:
                            //添加
                            OPT_Code = "A";
                            UCSG = new UCSkillGroup();
                            UCSG.SGTreeVItem = TreeVItem;
                            UCSG.IStrCurrentMethod = OPT_Code;
                            UCSG.IParentPage = this;
                            UCSG.CurrentApp = CurrentApp;
                            this.PopupPanel.Title = CurrentApp.GetLanguageInfo("1104005", "SkillGroup");
                            this.PopupPanel.Content = UCSG;
                            this.PopupPanel.IsOpen = true;
                            break;
                        case 1105001:
                            OPT_Code = "A";
                            UCOT = new UCOrgType();
                            UCOT.OTTreeVItem = TreeVItem;
                            UCOT.IStrCurrentMethod = OPT_Code;
                            UCOT.IParentPage = this;
                            UCOT.CurrentApp = CurrentApp;
                            this.PopupPanel.Title = CurrentApp.GetLanguageInfo("1104005", "OrgType");
                            this.PopupPanel.Content = UCOT;
                            this.PopupPanel.IsOpen = true;
                            break;
                        case 1104003:
                            //编辑信息
                            if (TreeVItem == null) { return; }
                            OPT_Code = "E";

                            UCSG = new UCSkillGroup();
                            UCSG.SGTreeVItem = TreeVItem;
                            UCSG.IStrCurrentMethod = OPT_Code;
                            UCSG.IParentPage = this;
                            UCSG.CurrentApp = CurrentApp;
                            this.PopupPanel.Title = CurrentApp.GetLanguageInfo("1104005", "SkillGroup");
                            this.PopupPanel.Content = UCSG;
                            this.PopupPanel.IsOpen = true;
                            break;
                        case 1105003:
                            if (TreeVItem == null) { return; }
                            OPT_Code = "E";
                            if (TreeVItem.ObjID.ToString() == "905" + CurrentApp.Session.RentInfo.Token + "00000000000")
                            {
                                ShowException(CurrentApp.GetLanguageInfo("1104T010", ""));
                                return;
                            }
                            UCOT = new UCOrgType();
                            UCOT.OTTreeVItem = TreeVItem;
                            UCOT.IStrCurrentMethod = OPT_Code;
                            UCOT.IParentPage = this;
                            UCOT.CurrentApp = CurrentApp;
                            this.PopupPanel.Title = CurrentApp.GetLanguageInfo("1104005", "OrgType");
                            this.PopupPanel.Content = UCOT;
                            this.PopupPanel.IsOpen = true;
                            break;
                        case 1104002:
                            //删除
                            if (TreeVItem == null) { return; }
                            RemoveSkillGroup();
                            break;
                        case 1105002:
                            if (TreeVItem == null) { return; }
                            { RemoveOrgType(); }
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        public void GetShowInformation()
        {
            try
            {
                //Dispatcher.Invoke(new Action(() => mRoot.Children.Clear())); 
                //mRoot.Children.Clear();
                if (S1104App.IDataTable11009 == null || S1104App.IDataTable11009.Rows == null || S1104App.IDataTable11009.Rows.Count == 0) { return; }
                if (ModelCode == "1105")
                {
                    DataRow[] DT = S1104App.IDataTable11009.Select(string.Format("C000=1"), "C002");//org

                    foreach (DataRow drOrg in DT)
                    {
                        ObjectItem item = new ObjectItem();
                        item.ObjType = ConstValue.RESOURCE_ORGTYPE;
                        item.ObjID = Convert.ToInt64(drOrg["C001"].ToString());
                        item.Description = drOrg["C009"].ToString();
                        item.State = Convert.ToInt32(drOrg["C004"].ToString());
                        if (item.ObjID.ToString() == "905" + CurrentApp.Session.RentInfo.Token + "00000000000")
                        {
                            item.Name = CurrentApp.GetLanguageInfo("1104000", "顶级机构");
                        }
                        else
                        {
                            item.Name = drOrg["C006"].ToString();
                        }
                        item.FullName = item.Name;
                        item.IsExpanded = true;
                        item.LockMethod = drOrg["C002"].ToString();
                        if (item.State == 1)
                        {
                            item.Icon = "Images/orgtype.png";
                        }
                        else
                        {
                            item.Icon = "Images/orgtypelock.png";
                        }
                        //Dispatcher.Invoke(new Action(() => mRoot.AddChild(item)));
                        mRoot.AddChild(item);
                    }
                }
                else
                {
                    DataRow[] DT = S1104App.IDataTable11009.Select(string.Format("C000=2"), "C002");//skillgroup

                    foreach (DataRow drOrg in DT)
                    {
                        ObjectItem item = new ObjectItem();
                        item.ObjType = ConstValue.RESOURCE_TECHGROUP;
                        item.ObjID = Convert.ToInt64(drOrg["C001"].ToString());
                        item.FullName = (drOrg["C006"].ToString());//code
                        item.Name = drOrg["C008"].ToString();
                        item.Description = drOrg["C009"].ToString();
                        //item.State = Convert.ToInt32(drOrg["C002"].ToString());
                        item.IsExpanded = true;
                        item.LockMethod = drOrg["C002"].ToString();
                        item.State = Convert.ToInt32(drOrg["C004"].ToString());
                        item.Description = drOrg["C009"].ToString();
                        if (item.State == 1)
                        {
                            item.Icon = "Images/skillgroup.png";
                        }
                        else
                        {
                            item.Icon = "Images/skillgrouplock.png";
                        }
                        //Dispatcher.Invoke(new Action(() => mRoot.AddChild(item)));
                        mRoot.AddChild(item);
                    }
                }
            }
            catch (Exception ex) { ShowException(ex.Message); }
        }

        #region NetPipeMessage
        //protected override void App_NetPipeEvent(WebRequest webRequest)
        //{
        //    base.App_NetPipeEvent(webRequest);

        //    Dispatcher.Invoke(new Action(() =>
        //    {
        //        try
        //        {
        //            var code = webRequest.Code;
        //            var session = webRequest.Session;
        //            var strData = webRequest.Data;
        //            switch (code)
        //            {
        //                case (int)RequestCode.SCLanguageChange:
        //                    LangTypeInfo langTypeInfo =
        //                       CurrentApp.Session.SupportLangTypes.FirstOrDefault(l => l.LangID.ToString() == strData);
        //                    if (langTypeInfo != null)
        //                    {
        //                        LangTypeInfo = langTypeInfo;
        //                        CurrentApp.Session.LangTypeInfo = langTypeInfo;
        //                        CurrentApp.Session.LangTypeID = langTypeInfo.LangID;
        //                        //if (MyWaiter != null)
        //                        //{
        //                        //    MyWaiter.Visibility = Visibility.Visible;
        //                        //}
        //                        mWorker = new BackgroundWorker();
        //                        mWorker.DoWork += (s, de) => CurrentApp.InitAllLanguageInfos();
        //                        mWorker.RunWorkerCompleted += (s, re) =>
        //                        {
        //                            mWorker.Dispose();
        //                            SetBusy(false);
        //                            //if (PopupPanel != null)
        //                            //{
        //                            //    PopupPanel.ChangeLanguage();
        //                            //}
        //                            if (PageHead != null)
        //                            {
        //                                PageHead.SessionInfo = CurrentApp.Session;
        //                                PageHead.InitInfo();
        //                            }

        //                        };
        //                        mWorker.RunWorkerAsync();
        //                    }
        //                    break;
        //                case (int)RequestCode.SCThemeChange:
        //                    ThemeInfo themeInfo = CurrentApp.Session.SupportThemes.FirstOrDefault(t => t.Name == strData);
        //                    if (themeInfo != null)
        //                    {
        //                        ThemeInfo = themeInfo;
        //                        CurrentApp.Session.ThemeInfo = themeInfo;
        //                        CurrentApp.Session.ThemeName = themeInfo.Name;
        //                        ChangeTheme();
        //                        PageHead.SessionInfo = CurrentApp.Session;
        //                        PageHead.InitInfo();
        //                    }
        //                    break;
        //                case (int)RequestCode.SCOperation:
        //                    ModelCode = strData;
        //                    Init();
        //                    TreeVItem = null;
        //                    ObjectDetail.ItemsSource = null;
        //                    break;
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            ShowException(ex.Message);
        //        }
        //    }));
        //}
        #endregion

        public void SaveSkillGroupEdited()
        {
            List<string> LListStrWcfArgs = new List<string>();
            string LStrSkillCode = string.Empty, LStrSkillName = string.Empty;
            string LStrTypeDescriber = string.Empty;
            string TreeItemName = string.Empty;
            try
            {
                //调用子窗口获取list
                LListStrWcfArgs = UCSG.GetDateSkillGroup();
                if (LListStrWcfArgs.Count == 0) { return; }
                TreeItemName = LListStrWcfArgs[9];
                //WebRequest LWebRequestClientLoading = new WebRequest();
                //LWebRequestClientLoading.Code = 12111;
                //WebReturn LWebReturn = CurrentApp.SendNetPipeMessage(LWebRequestClientLoading);
                //IBoolIsBusy = true;

                Service00000Client client = new Service00000Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session)
                    , WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service00000"));
                OperationDataArgs LWCFOperationReturn = new OperationDataArgs();
                LWCFOperationReturn = client.OperationMethodA(22, LListStrWcfArgs);
                IStrCallReturn = LWCFOperationReturn.StringReturn;
                IBoolCallReturn = LWCFOperationReturn.BoolReturn;
                IListStrAfterSave.Clear();
                string Code = string.Empty;
                if (OPT_Code == "A")
                {
                    Code = "FO1104001";
                }
                else
                {
                    Code = "FO1104003";
                }
                if (IBoolCallReturn)
                {
                    PopupPanel.IsOpen = false;
                    ResetTreeList();

                    #region 写操作日志
                    string msg_success = string.Format("{0}:{1}", Utils.FormatOptLogString(Code), TreeItemName);
                    CurrentApp.WriteOperationLog(Code.Substring(2), ConstValue.OPT_RESULT_SUCCESS, msg_success);
                    #endregion
                }
                else
                {
                    if (IStrCallReturn == "S906E01")
                    {
                        ShowException(CurrentApp.GetLanguageInfo("1104T002", ""));
                    }
                    else
                    {
                        ShowException(CurrentApp.GetLanguageInfo("1104T009", "") + "\n" + IStrCallReturn);
                    }
                    #region 写操作日志
                    string msg_success = string.Format("{0}:{1}", Utils.FormatOptLogString(Code), TreeItemName);
                    CurrentApp.WriteOperationLog(Code.Substring(2), ConstValue.OPT_RESULT_FAIL, msg_success);
                    #endregion
                    return;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.ToString());
            }
        }
        public void SaveOrgTypeEdited()
        {
            List<string> LListStrWcfArgs = new List<string>();
            string LStrSkillCode = string.Empty, LStrSkillName = string.Empty;
            string LStrTypeDescriber = string.Empty;
            string TreeItemName = string.Empty;
            try
            {
                //调用子窗口获取list
                LListStrWcfArgs = UCOT.GetDateOrgType();
                if (LListStrWcfArgs.Count == 0) { return; }
                TreeItemName = LListStrWcfArgs[6];

                //WebRequest LWebRequestClientLoading = new WebRequest();
                //LWebRequestClientLoading.Code = 12111;
                //WebReturn LWebReturn = CurrentApp.SendNetPipeMessage(LWebRequestClientLoading);
                //IBoolIsBusy = true;

                Service00000Client client = new Service00000Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session)
                    , WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service00000"));
                OperationDataArgs LWCFOperationReturn = new OperationDataArgs();
                LWCFOperationReturn = client.OperationMethodA(22, LListStrWcfArgs);
                IStrCallReturn = LWCFOperationReturn.StringReturn;
                IBoolCallReturn = LWCFOperationReturn.BoolReturn;
                IListStrAfterSave.Clear();
                string Code = string.Empty;
                if (OPT_Code == "A")
                {
                    Code = "FO1105001";
                }
                else
                {
                    Code = "FO1105003";
                }
                if (IBoolCallReturn)
                {
                    PopupPanel.IsOpen = false;
                    ResetTreeList();
                    #region 写操作日志
                    string msg_success = string.Format("{0}:{1}", Utils.FormatOptLogString(Code), TreeItemName);
                    CurrentApp.WriteOperationLog(Code.Substring(2), ConstValue.OPT_RESULT_SUCCESS, msg_success);
                    #endregion
                }
                else
                {
                    if (IStrCallReturn == "S905E01")
                    {
                        ShowException(CurrentApp.GetLanguageInfo("1104T001", ""));
                    }
                    else
                    {
                        ShowException(CurrentApp.GetLanguageInfo("1104T009", "") + "\n" + IStrCallReturn);
                    }
                    #region 写操作日志
                    string msg_success = string.Format("{0}:{1}", Utils.FormatOptLogString(Code), TreeItemName);
                    CurrentApp.WriteOperationLog(Code.Substring(2), ConstValue.OPT_RESULT_FAIL, msg_success);
                    #endregion
                    return;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.ToString());
            }
        }
        private void RemoveSkillGroup()
        {
            List<string> LListStrWcfArgs = new List<string>();
            string LStrVerificationCode104 = string.Empty;
            string TreeItemName = TreeVItem.Name;
            try
            {
                if (TreeVItem == null) { return; }

                string LStrSkillCode = TreeVItem.FullName;
                if (MessageBox.Show(string.Format(CurrentApp.GetLanguageInfo("1104T012", ""), LStrSkillCode), CurrentApp.Session.AppName, MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) != MessageBoxResult.Yes) { return; }


                LListStrWcfArgs.Add(CurrentApp.Session.DatabaseInfo.TypeID.ToString());                                  //0
                LListStrWcfArgs.Add(CurrentApp.Session.DatabaseInfo.GetConnectionString());                              //1
                LListStrWcfArgs.Add(CurrentApp.Session.RentInfo.Token);                                                  //2
                LListStrWcfArgs.Add("2");                                                                                   //3
                LListStrWcfArgs.Add(TreeVItem.ObjID.ToString());                                             //4
                LListStrWcfArgs.Add("");                                                                                    //5
                LListStrWcfArgs.Add(LStrSkillCode);                                                                       //6
                LListStrWcfArgs.Add(CurrentApp.Session.UserID.ToString());                                               //7
                LListStrWcfArgs.Add("");                                                                                    //8
                LListStrWcfArgs.Add("");                                                                                    //9
                LListStrWcfArgs.Add(TreeVItem.State.ToString());                                             //10
                LListStrWcfArgs.Add("");                                                                                    //11
                LListStrWcfArgs.Add("D");                                                                     //12

                WebRequest LWebRequestClientLoading = new WebRequest();
                LWebRequestClientLoading.Code = 12111;
                WebReturn LWebReturn = CurrentApp.SendNetPipeMessage(LWebRequestClientLoading);
                IBoolIsBusy = true;
                Service00000Client client = new Service00000Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session)
                    , WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service00000"));
                OperationDataArgs LWCFOperationReturn = new OperationDataArgs();
                LWCFOperationReturn = client.OperationMethodA(22, LListStrWcfArgs);
                IStrCallReturn = LWCFOperationReturn.StringReturn;
                IBoolCallReturn = LWCFOperationReturn.BoolReturn;
                IListStrAfterSave.Clear();
                if (IBoolCallReturn)
                {
                    ResetTreeList();
                    #region 写操作日志
                    string msg_success = string.Format("{0}:{1}", Utils.FormatOptLogString("FO1104002"), TreeItemName);
                    CurrentApp.WriteOperationLog("1104002", ConstValue.OPT_RESULT_SUCCESS, msg_success);
                    #endregion
                }
                else
                {
                    //删除失败
                    #region 写操作日志
                    string msg_success = string.Format("{0}:{1}", Utils.FormatOptLogString("FO1104002"), TreeItemName);
                    CurrentApp.WriteOperationLog("1104002", ConstValue.OPT_RESULT_FAIL, msg_success);
                    #endregion
                    return;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.ToString());
            }
        }
        private void RemoveOrgType()
        {
            List<string> LListStrWcfArgs = new List<string>();
            string LStrVerificationCode104 = string.Empty;
            string TreeItemName = TreeVItem.Name;
            try
            {
                if (TreeVItem == null) { return; }

                //LStrVerificationCode104 = CurrentApp.CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104);
                string LStrOrgTypeID = TreeVItem.ObjID.ToString();
                if (LStrOrgTypeID == "905" + CurrentApp.Session.RentInfo.Token + "00000000000")
                {
                    ShowException(CurrentApp.GetLanguageInfo("1104T004", ""));
                    return;
                }
                string LStrOrgTypeName = TreeVItem.Name;
                //LStrOrgTypeName = EncryptionAndDecryption.EncryptDecryptString(LStrOrgTypeName, LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                if (MessageBox.Show(string.Format(CurrentApp.GetLanguageInfo("1104T011", ""), LStrOrgTypeName), CurrentApp.Session.AppName, MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) != MessageBoxResult.Yes) { return; }

                LListStrWcfArgs.Add(CurrentApp.Session.DatabaseInfo.TypeID.ToString());                                  //0
                LListStrWcfArgs.Add(CurrentApp.Session.DatabaseInfo.GetConnectionString());                              //1
                LListStrWcfArgs.Add(CurrentApp.Session.RentInfo.Token);                                                  //2
                LListStrWcfArgs.Add("1");                                                                                   //3
                LListStrWcfArgs.Add(TreeVItem.ObjID.ToString());                                             //4
                LListStrWcfArgs.Add("");                                                                                    //5
                LListStrWcfArgs.Add(LStrOrgTypeName);                                                                       //6
                LListStrWcfArgs.Add(CurrentApp.Session.UserID.ToString());                                               //7
                LListStrWcfArgs.Add("");                                                                                    //8
                LListStrWcfArgs.Add("");                                                                                    //9
                LListStrWcfArgs.Add(TreeVItem.State.ToString());                                             //10
                LListStrWcfArgs.Add("");                                                                                    //11
                LListStrWcfArgs.Add("D");                                                                     //12

                WebRequest LWebRequestClientLoading = new WebRequest();
                LWebRequestClientLoading.Code = 12111;
                WebReturn LWebReturn = CurrentApp.SendNetPipeMessage(LWebRequestClientLoading);
                IBoolIsBusy = true;
                Service00000Client client = new Service00000Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session)
                    , WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service00000"));
                OperationDataArgs LWCFOperationReturn = new OperationDataArgs();
                LWCFOperationReturn = client.OperationMethodA(22, LListStrWcfArgs);
                IStrCallReturn = LWCFOperationReturn.StringReturn;
                IBoolCallReturn = LWCFOperationReturn.BoolReturn;
                IListStrAfterSave.Clear();
                if (IBoolCallReturn)
                {
                    ResetTreeList();
                    #region 写操作日志
                    string msg_success = string.Format("{0}:{1}", Utils.FormatOptLogString("FO1105002"), TreeItemName);
                    CurrentApp.WriteOperationLog("1105002", ConstValue.OPT_RESULT_SUCCESS, msg_success);
                    #endregion
                }
                else
                {
                    //删除失败
                    #region 写操作日志
                    string msg_success = string.Format("{0}:{1}", Utils.FormatOptLogString("FO1105002"), TreeItemName);
                    CurrentApp.WriteOperationLog("1105002", ConstValue.OPT_RESULT_FAIL, msg_success);
                    #endregion
                    return;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.ToString());
            }
        }

        public void ResetTreeList()//重新加载
        {
            ((S1104App)CurrentApp).Load11009Data();
            mRoot.Children.Clear();
            GetShowInformation();
        }

        private void ShowObjectDetail()
        {
            try
            {
                if (TreeVItem == null || TreeVItem.Name == string.Empty || TreeVItem.ObjID == null)
                {
                    //ObjectDetail = new ObjectDetailViewer();
                    return;
                }
                ObjectDetail.Title = TreeVItem.Name;
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.UriSource = new Uri(string.Format("/Themes/Default/UMPS1104/{0}", TreeVItem.Icon), UriKind.Relative);
                image.EndInit();
                ObjectDetail.Icon = image;
                List<PropertyItem> listProperties = new List<PropertyItem>();
                switch (TreeVItem.ObjType)
                {
                    case 905://org
                        //DataRow[] OrgRow = CurrentApp.IDataTable11009.Select(string.Format("C001={0}", TreeVItem.ObjID));
                        //if (OrgRow != null && OrgRow.Count() != 0)
                        {
                            PropertyItem property = new PropertyItem();
                            property.Name = CurrentApp.GetLanguageInfo("1104007", "name");
                            property.ToolTip = property.Name;
                            property.Value = TreeVItem.Name;
                            listProperties.Add(property);
                            property = new PropertyItem();
                            property.Name = CurrentApp.GetLanguageInfo("1104008", "bianhao");
                            property.ToolTip = property.Name;
                            property.Value = TreeVItem.ObjID.ToString();
                            listProperties.Add(property);
                            property = new PropertyItem();
                            property.Name = CurrentApp.GetLanguageInfo("1104009", "状态");
                            property.ToolTip = property.Name;
                            property.Value = TreeVItem.LockMethod;
                            if (property.Value == "1")
                            {
                                property.Value = CurrentApp.GetLanguageInfo("1104014", "zheengcha");
                            }
                            else
                            {
                                property.Value = CurrentApp.GetLanguageInfo("1104015", "jinyong");
                            }
                            listProperties.Add(property);
                            property = new PropertyItem();
                            property.Name = CurrentApp.GetLanguageInfo("1104010", "miaoshu");
                            property.ToolTip = property.Name;
                            property.Value = TreeVItem.Description;
                            listProperties.Add(property);
                        }
                        break;
                    case 906://skillgroup
                        //DataRow[] AgentRow = CurrentApp.IDataTable11009.Select(string.Format("C001={0}", TreeVItem.ObjID));
                        //if (AgentRow != null && AgentRow.Count() != 0)
                        {
                            PropertyItem property = new PropertyItem();
                            property.Name = CurrentApp.GetLanguageInfo("1104007", "name");
                            property.ToolTip = property.Name;
                            property.Value = TreeVItem.Name;
                            listProperties.Add(property);
                            property = new PropertyItem();
                            property.Name = CurrentApp.GetLanguageInfo("1104008", "bianma");
                            property.ToolTip = property.Name;
                            property.Value = TreeVItem.FullName;
                            listProperties.Add(property);
                            property = new PropertyItem();
                            property.Name = CurrentApp.GetLanguageInfo("1104009", "状态");
                            property.ToolTip = property.Name;
                            property.Value = TreeVItem.LockMethod;
                            if (property.Value == "1")
                            {
                                property.Value = CurrentApp.GetLanguageInfo("1104014", "zheengcha");
                            }
                            else
                            {
                                property.Value = CurrentApp.GetLanguageInfo("1104015", "jinyong");
                            }
                            listProperties.Add(property);
                            property = new PropertyItem();
                            property.Name = CurrentApp.GetLanguageInfo("1104010", "miaoshu");
                            property.ToolTip = property.Name;
                            property.Value = TreeVItem.Description;
                            listProperties.Add(property);
                        }
                        break;
                }
                ObjectDetail.ItemsSource = listProperties;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitColumnData()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)RequestCode.WSGetUserViewColumnList;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(CurrentApp.Session.UserInfo.UserID.ToString());
                webRequest.ListData.Add("1101001");
                Service11012Client client = new Service11012Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11012"));
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("InitColumnData Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    ShowException(string.Format("No columns"));
                    return;
                }
                mListGridTreeColumns.Clear();
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<ViewColumnInfo>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("InitColumnData Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    ViewColumnInfo column = optReturn.Data as ViewColumnInfo;
                    if (column != null) { mListGridTreeColumns.Add(column); }
                }

                CurrentApp.WriteLog("PageInit", string.Format("Init ViewColumn"));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitColumns()
        {
            try
            {
                ViewColumnInfo column;
                int nameColumnWidth;
                GridViewColumn gvc;
                GridViewColumnHeader gvch;
                List<GridViewColumn> listColumns = new List<GridViewColumn>();
                column = mListGridTreeColumns.FirstOrDefault(c => c.ColumnName == "Name");
                gvch = new GridViewColumnHeader();
                gvch.Content = string.Empty;
                if (column != null)
                {
                    nameColumnWidth = column.Width;
                }
                else
                {
                    nameColumnWidth = 250;
                }

                //gvc = new GridViewColumn();
                //gvc.Header = string.Empty;
                //gvc.Width = 25;
                //DataTemplate checkTemplate = (DataTemplate)Resources["CheckCellTemplate"];
                //if (checkTemplate != null)
                //{
                //    gvc.CellTemplate = checkTemplate;
                //}
                //else
                //{
                //    gvc.DisplayMemberBinding = new Binding("IsChecked");
                //}
                //listColumns.Add(gvc);

                gvc = new GridViewColumn();
                gvc.Header = CurrentApp.GetLanguageInfo("COL1104001", "State");
                gvc.Width = 80;
                DataTemplate objectStateTemplate = (DataTemplate)Resources["ObjectStateCellTemplate"];
                if (objectStateTemplate != null)
                {
                    gvc.CellTemplate = objectStateTemplate;
                }
                else
                {
                    gvc.DisplayMemberBinding = new Binding("State");
                }
                listColumns.Add(gvc);

                if (CurrentApp.StartArgs == "1104")
                {
                    //gvc = new GridViewColumn();
                //    gvc.Header = CurrentApp.GetLanguageInfo("COL1104002", "Full Name");
                //    gvc.Width = 250;
                //    DataTemplate fullNameTemplate = (DataTemplate)Resources["FullNameCellTemplate"];
                //    if (fullNameTemplate != null)
                //    {
                //        gvc.CellTemplate = fullNameTemplate;
                //    }
                //    else
                //    {
                //        gvc.DisplayMemberBinding = new Binding("FullName");
                //    }
                //    listColumns.Add(gvc);
                //}
                //else
                //{
                    gvc = new GridViewColumn();
                    gvc.Header = CurrentApp.GetLanguageInfo("COL1104002", "code");
                    gvc.Width = 100;
                    DataTemplate fullNameTemplate = (DataTemplate)Resources["FullNameCellTemplate"];
                    if (fullNameTemplate != null)
                    {
                        gvc.CellTemplate = fullNameTemplate;
                    }
                    else
                    {
                        gvc.DisplayMemberBinding = new Binding("FullName");
                    }
                    listColumns.Add(gvc);
                }

                gvc = new GridViewColumn();
                gvc.Header = CurrentApp.GetLanguageInfo("COL1104003", "Description");
                gvc.Width = 300;
                DataTemplate descriptionTemplate = (DataTemplate)Resources["DescriptionCellTemplate"];
                if (descriptionTemplate != null)
                {
                    gvc.CellTemplate = descriptionTemplate;
                }
                else
                {
                    gvc.DisplayMemberBinding = new Binding("Description");
                }
                listColumns.Add(gvc);

                //gvc = new GridViewColumn();
                //gvc.Header = string.Empty;
                //gvc.Width = 150;
                //DataTemplate operationTemplate = (DataTemplate)Resources["OperationCellTemplate"];
                //if (operationTemplate != null)
                //{
                //    gvc.CellTemplate = operationTemplate;
                //}
                //else
                //{
                //    gvc.DisplayMemberBinding = new Binding();
                //}
                //listColumns.Add(gvc);

                DataTemplate nameColumnTemplate = (DataTemplate)Resources["NameColumnTemplate"];
                if (nameColumnTemplate != null)
                {
                    TvObjects.SetColumns(nameColumnTemplate, gvch, nameColumnWidth, listColumns);
                }
                else
                {
                    TvObjects.SetColumns(gvch, nameColumnWidth, listColumns);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }
    }
}
