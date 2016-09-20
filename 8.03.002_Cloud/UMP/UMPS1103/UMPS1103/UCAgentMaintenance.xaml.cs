using PFShareClassesC;
using PFShareControls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.ServiceModel;
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
using UMPS1103.Models;
using UMPS1103.Wcf11011;
using UMPS1103.WCFService00000;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;
using VoiceCyber.UMP.Controls.Wcf11012;
using VoiceCyber.Wpf.CustomControls;

namespace UMPS1103
{
    /// <summary>
    /// Page1.xaml 的交互逻辑
    /// </summary>
    public partial class UCAgentMaintenance
    {
        public BackgroundWorker mWorker;
        private SelectedInfo mSelectInfo;

        private List<TreeViewItem> IListTVIAgent = new List<TreeViewItem>();
        private ObservableCollection<ViewColumnInfo> mListGridTreeColumns;
        ObjectItem mRoot;

        public string IStrCurrentMethod = string.Empty;
        public string LStrItemData = string.Empty;
        UCAgentBasicInformation UCAgentInfo = new UCAgentBasicInformation();
        private S1103App S1103App;

        #region 保存数据至数据库
        private BackgroundWorker IBWSaveData = null;
        private bool IBoolCallReturn = true;
        private string IStrCallReturn = string.Empty;
        private List<string> IListStrAfterSave = new List<string>();
        #endregion
        private bool IBoolIsBusy = false;

        private ObservableCollection<OperationInfo> ListOpt;
        public ObjectItem TreeVItem;

        public UCAgentMaintenance()
        {
            InitializeComponent();

            ListOpt = new ObservableCollection<OperationInfo>();
            mListGridTreeColumns = new ObservableCollection<ViewColumnInfo>();
            mRoot = new ObjectItem();
            this.TreeViewOrgAgent.SelectedItemChanged += TreeViewOrgAgent_SelectedItemChanged;
            this.TreeViewOrgAgent.ItemsSource = mRoot.Children;
            this.TvObjects.SelectedItemChanged += TvObjects_SelectedItemChanged;
            this.TvObjects.ItemsSource = mRoot.Children;
        }

        private void TvObjects_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                LStrItemData = string.Empty;
                TreeVItem = TvObjects.SelectedItem as ObjectItem;
                if (TreeVItem == null)
                {
                    return;
                }
                LStrItemData = TreeVItem.ObjID.ToString();
                //TreeVItem.IsSelected = true;
                ShowObjectDetail();
            }
            catch { }
        }

        void TreeViewOrgAgent_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                LStrItemData = string.Empty;
                TreeVItem = TreeViewOrgAgent.SelectedItem as ObjectItem;
                if (TreeVItem == null)
                {
                    return;
                }
                LStrItemData = TreeVItem.ObjID.ToString();
                ShowObjectDetail();
                //准备拖放的数据
                PrepareDragDropData(TreeVItem);
            }
            catch { }
        }
        #region Init and Load
        protected override void Init()
        {
            try
            {
                PageName = "UCAgentMaintenance";
                StylePath = "UMPS1103/UCAgentMaintenance.xaml";
                if (CurrentApp != null)
                {
                    S1103App = CurrentApp as S1103App;
                }
                else
                {
                    S1103App = new S1103App(false);
                }
                base.Init();
                ChangeTheme();
                InitColumnData();
                ChangeLanguage();
                ShowControlOrgAgent();

                SetBusy(true, string.Empty);
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
                };
                mWorker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
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
                    string uri = string.Format("/UMPS1103;component/Themes/{0}/{1}",
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

            //固定资源(有些资源包含转换器，命令等自定义类型，
            //这些资源不能通过url来动态加载，他只能固定的编译到程序集中
            try
            {
                string uri = string.Format("/UMPS1103;component/Themes/Default/UMPS1103/UCAgentMaintenanceStatic.xaml");
                ResourceDictionary resource = new ResourceDictionary();
                resource.Source = new Uri(uri, UriKind.RelativeOrAbsolute);
                Resources.MergedDictionaries.Add(resource);
            }
            catch (Exception ex)
            {
                //ShowException("3" + ex.Message);
            }

            try
            {
                string uri = string.Format("/UMPS1103;component/Themes/Default/UMPS1103/MoreInfo.xaml");
                ResourceDictionary resource = new ResourceDictionary();
                resource.Source = new Uri(uri, UriKind.RelativeOrAbsolute);
                Resources.MergedDictionaries.Add(resource);
            }
            catch (Exception ex)
            {
                //ShowException("3" + ex.Message);
            }
        }
        #endregion

        #region ChangLanguage
        public override void ChangeLanguage()
        {
            base.ChangeLanguage();
            CurrentApp.AppTitle = CurrentApp.GetLanguageInfo("FO1103", "UMP Agent Management");
            if (this.PopupPanel.IsOpen)
            {
                this.PopupPanel.Title = CurrentApp.GetLanguageInfo("S1103005", "");
                this.PopupPanel.ChangeLanguage();
            }
            this.TabSampleView.Header = CurrentApp.GetLanguageInfo("S1103101", "");
            this.TabGridView.Header = CurrentApp.GetLanguageInfo("S1103102", "");
            InitOperations();
            this.ExpBasicOpt.Header = CurrentApp.GetLanguageInfo("S1103027", "jichucaozuo");
            this.LabTreeHeard.Content = CurrentApp.GetLanguageInfo("S1103001", "zhubiaoti");

            InitColumns();
        }
        #endregion

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
        //                        SetBusy(true);
        //                        mWorker = new BackgroundWorker();
        //                        mWorker.DoWork += (s, de) => CurrentApp.InitAllLanguageInfos();
        //                        mWorker.RunWorkerCompleted += (s, re) =>
        //                        {
        //                            mWorker.Dispose();
        //                            SetBusy(false);
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
        //                    }
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

        public void ShowControlOrgAgent()
        {
            try
            {
                mRoot.Children.Clear();
                LoadOrg(mRoot, "0");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "3");
            }
        }

        private void LoadOrg(ObjectItem parentItem, string ParentID)
        {
            mRoot.Children.Clear();
            DataRow[] DT = S1103App.IDataTable11006.Select(string.Format("C004={0}", ParentID));

            foreach (DataRow drOrg in DT)
            {
                ObjectItem item = new ObjectItem();
                item.StartDragged += ObjectItem_StartDragged;
                item.DragOver += ObjectItem_DragOver;
                item.Dropped += ObjectItem_Dropped;
                item.ObjType = ConstValue.RESOURCE_ORG;
                long TempObjID;
                if (long.TryParse(drOrg["C001"].ToString(), out TempObjID))
                {
                    item.ObjID = TempObjID;
                }
                item.Name = S1103App.DecryptString(drOrg["C002"].ToString());
                item.FullName = item.Name;
                item.State = 1;
                //item.Description = drOrg["C004"].ToString();
                item.IsChecked = false;
                item.IsExpanded = true;
                if (item.ObjID.ToString() == ConstValue.ORG_ROOT.ToString())
                {
                    item.Icon = "Images/root.ico";
                }
                else
                {
                    item.Icon = "Images/org.ico";
                }
                //加载下面的机构和用户
                LoadOrg(item, item.ObjID.ToString());
                LoadAgent(item, item.ObjID.ToString());

                Dispatcher.Invoke(new Action(() => parentItem.AddChild(item)));
            }
        }

        private void LoadAgent(ObjectItem parentItem, string ParentID)
        {
            DataRow[] DT_Agent = S1103App.IDataTable11101.Select(string.Format("C011={0} AND C002=1", ParentID));

            foreach (DataRow drOrg in DT_Agent)
            {
                ObjectItem item = new ObjectItem();
                item.StartDragged += ObjectItem_StartDragged;
                item.DragOver += ObjectItem_DragOver;
                item.Dropped += ObjectItem_Dropped;
                item.ObjType = ConstValue.RESOURCE_AGENT;
                item.ObjID = Convert.ToInt64(drOrg["C001"].ToString());
                item.Name = string.Format("{0}({1})", S1103App.DecryptString(drOrg["C017"].ToString()), S1103App.DecryptString(drOrg["C018"].ToString()));
                int TempInt;
                if (int.TryParse(drOrg["C012"].ToString(), out TempInt))
                {
                    item.State = Convert.ToInt32(drOrg["C012"].ToString());
                }
                long TempObjID;
                if (long.TryParse(drOrg["C011"].ToString(), out TempObjID))
                {
                    item.ObjParentID = TempObjID;
                }
                if (item.State == 1)
                {
                    item.Icon = "Images/agent.ico";
                }
                else
                {
                    item.Icon = "Images/agentforbid.ico";
                }
                item.IsChecked = false;
                item.FullName = S1103App.DecryptString(drOrg["C017"].ToString());
                if (drOrg["C016"].ToString() == "00")
                    item.Description = CurrentApp.GetLanguageInfo("S1103032", "手工添加");
                else
                    item.Description = CurrentApp.GetLanguageInfo("S1103033", "录音表添加");
                item.LockMethod = drOrg["C015"].ToString();
                item.IsExpanded = true;
                //加载下面的机构和用户
                LoadOrg(item, item.ObjID.ToString());
                Dispatcher.Invoke(new Action(() => parentItem.AddChild(item)));
            }
        }

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
                webRequest.ListData.Add("1103");
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
            //ListOpt.Clear();
            OperationInfo item;
            //添加
            item = new OperationInfo();
            item.ID = 1103001;
            item.Icon = "Images/add.png";
            item.Display = CurrentApp.GetLanguageInfo("S1103023", "add");
            ListOpt.Add(item);

            item = new OperationInfo();
            item.ID = 1103002;
            item.Icon = "Images/remove.png";
            item.Display = CurrentApp.GetLanguageInfo("S1103024", "remove");
            ListOpt.Add(item);

            item = new OperationInfo();
            item.ID = 1103003;
            item.Icon = "Images/setting.png";
            item.Display = CurrentApp.GetLanguageInfo("S1103005", "set");
            ListOpt.Add(item);

            item = new OperationInfo();
            item.ID = 1103004;
            item.Icon = "Images/setting.png";
            item.Display = CurrentApp.GetLanguageInfo("S1103028", "Restore password");
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

        void ButtonRight_Click(object sender, RoutedEventArgs e)
        {
            var btn = e.Source as Button;
            if (btn != null)
            {
                var optItem = btn.DataContext as OperationInfo;
                if (optItem != null)
                {
                    switch (optItem.ID)
                    {
                        case 1103001:
                            //添加
                            IStrCurrentMethod = "A";
                            UCAgentInfo = new UCAgentBasicInformation();
                            UCAgentInfo.IPageParent = this;
                            UCAgentInfo.IStrOrgID = LStrItemData;
                            UCAgentInfo.ModelID = IStrCurrentMethod;
                            UCAgentInfo.CurrentApp = CurrentApp;
                            this.PopupPanel.Title = CurrentApp.GetLanguageInfo("S1103005", "");
                            this.PopupPanel.Content = UCAgentInfo;
                            this.PopupPanel.IsOpen = true;
                            break;
                        case 1103003:
                            //编辑信息
                            if (LStrItemData == string.Empty || LStrItemData.Length <= 3 || LStrItemData.Substring(0, 3) != "103") { return; }
                            IStrCurrentMethod = "E";
                            UCAgentInfo = new UCAgentBasicInformation();
                            UCAgentInfo.IPageParent = this;
                            UCAgentInfo.IStrAgentID = LStrItemData;
                            UCAgentInfo.ModelID = IStrCurrentMethod;
                            UCAgentInfo.CurrentApp = CurrentApp;
                            this.PopupPanel.Title = CurrentApp.GetLanguageInfo("S1103005", "");
                            this.PopupPanel.Content = UCAgentInfo;
                            this.PopupPanel.IsOpen = true;
                            break;
                        case 1103002:
                            //删除
                            if (LStrItemData == string.Empty || LStrItemData.Length <= 3 || LStrItemData.Substring(0, 3) != "103") { return; }
                            IStrCurrentMethod = "D";
                            RemoveSingleAgent();
                            //SaveAgentInformation();
                            break;
                        case 1103004:
                            //还原密码
                            if (LStrItemData == string.Empty || LStrItemData.Length <= 3 || LStrItemData.Substring(0, 3) != "103") { return; }
                            IStrCurrentMethod = "R";
                            System.Windows.Forms.DialogResult rr = System.Windows.Forms.MessageBox.Show(string.Format(CurrentApp.GetLanguageInfo("S1103029", "确定还原该坐席密码吗？"), TreeVItem.Name), CurrentApp.AppName, System.Windows.Forms.MessageBoxButtons.YesNo, System.Windows.Forms.MessageBoxIcon.Question);
                            if (rr == System.Windows.Forms.DialogResult.Yes)
                            {
                                RestoreAgentPassword();
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private void RestoreAgentPassword()
        {
            try
            {
                //先获取全局参数默认密码是什么
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)RequestCode.WSGetGlobalParamList;
                webRequest.ListData.Add("11");
                webRequest.ListData.Add("11010501");
                webRequest.ListData.Add(string.Empty);

                Service11012Client client = new Service11012Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(
                        CurrentApp.Session.AppServerInfo,
                        "Service11012"));
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                GlobalParamInfo GlobalParam = new GlobalParamInfo();
                OperationReturn optReturn = XMLHelper.DeserializeObject<GlobalParamInfo>(webReturn.ListData[0]);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("InitColumnData Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                GlobalParam = optReturn.Data as GlobalParamInfo;
                if (GlobalParam == null) { return; }

                //修改坐席密码，用update语句
                List<string> pwdString = new List<string>();
                pwdString.Add(CurrentApp.Session.DBType.ToString());//数据库类型
                pwdString.Add(CurrentApp.Session.DBConnectionString);//数据库连接串
                pwdString.Add(CurrentApp.Session.RentInfo.Token);//租户编码
                pwdString.Add(TreeVItem.ObjID.ToString());//用户编码
                pwdString.Add(GlobalParam.ParamValue.Substring(8));//新密码
                Service00000Client loginClient = new Service00000Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service00000"));
                OperationDataArgs args = loginClient.OperationMethodA(45, pwdString);
                if (args.BoolReturn)//成功
                {
                    CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("S1103030", "Modify Sucessed"));
                    #region 写操作日志
                    string msg_success = string.Format("{0} {1} :{2}", CurrentApp.Session.UserInfo.UserName, Utils.FormatOptLogString("FO1103004"), TreeVItem.Name);
                    CurrentApp.WriteOperationLog("1103004", ConstValue.OPT_RESULT_SUCCESS, msg_success);
                    #endregion
                }
                else//失败
                {
                    ShowException(string.Format("{0}:{1}", CurrentApp.GetLanguageInfo("S1103031", args.StringReturn), args.StringReturn));
                    #region 写操作日志
                    string msg_Fail = string.Format("{0} {1} :{2}", CurrentApp.Session.UserInfo.UserName, Utils.FormatOptLogString("FO1103004"), TreeVItem.Name);
                    CurrentApp.WriteOperationLog("1103004".ToString(), ConstValue.OPT_RESULT_FAIL, msg_Fail);
                    #endregion
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }

        }

        public void SaveAgentInformation()
        {
            /// 0：数据库类型
            /// 1：数据库连接串
            /// 2：租户编码（5位）
            /// 3：用户编码（19位）
            /// 4：方法（E：修改；D：删除；A：增加）
            /// 5：座席编码（19位。如果方法 = ‘A’，该值为 0或String.Empty）
            /// 6：座席号
            /// 7～N：BXX，属性ID("00"格式) + char(27) + 实际数据；S00 + char(27) + 技能组ID； U00 + char(27) + 用户ID
            /// 
            List<string> LListStrWcfArgs = new List<string>();
            string LStrItemDataTemp = string.Empty;
            string LStrSAUserID = string.Empty;

            try
            {
                LListStrWcfArgs.Add(CurrentApp.Session.DatabaseInfo.TypeID.ToString());                                  //0
                LListStrWcfArgs.Add(CurrentApp.Session.DatabaseInfo.GetConnectionString());                              //1
                LListStrWcfArgs.Add(CurrentApp.Session.RentInfo.Token);                                                  //2
                LListStrWcfArgs.Add(CurrentApp.Session.UserInfo.UserID.ToString());                                      //3
                LListStrWcfArgs.Add(IStrCurrentMethod);                                                           //4
                if (IStrCurrentMethod == "A") { LListStrWcfArgs.Add("0"); }                                       //5
                else
                {
                    if (TreeVItem == null) { return; }
                    LStrItemDataTemp = TreeVItem.ObjID.ToString();
                    if (LStrItemDataTemp.Substring(0, 3) != "103") { return; }
                    LListStrWcfArgs.Add(LStrItemDataTemp);
                }

                List<string> LListStrBasicInfo = UCAgentInfo.GetElementSettedData();
                if (LListStrBasicInfo.Count <= 0) { return; }
                LListStrWcfArgs.Add(LListStrBasicInfo[1].Substring(4));                                            //6

                foreach (string LStrSingleData in LListStrBasicInfo) { LListStrWcfArgs.Add(LStrSingleData); }
                if (IStrCurrentMethod == "E")
                {
                    List<string> LListStrSkillInfo = UCAgentInfo.GetSGData();
                    List<string> LListStrManagerInfo = UCAgentInfo.GetManagerData();

                    foreach (string LStrSingleData in LListStrSkillInfo) { LListStrWcfArgs.Add(LStrSingleData); }
                    foreach (string LStrSingleData in LListStrManagerInfo)
                    {
                        LListStrWcfArgs.Add(LStrSingleData);
                    }
                }
                else
                {
                    LStrSAUserID = "102" + CurrentApp.Session.RentInfo.Token + "00000000001";
                    LListStrWcfArgs.Add("U00" + ConstValue.SPLITER_CHAR + LStrSAUserID);
                    if (CurrentApp.Session.UserInfo.UserID.ToString() != LStrSAUserID)
                    {
                        LListStrWcfArgs.Add("U00" + ConstValue.SPLITER_CHAR + CurrentApp.Session.UserInfo.UserID.ToString());
                    }
                }
                WebRequest LWebRequestClientLoading = new WebRequest();
                LWebRequestClientLoading.Code = 12111;
                WebReturn LWebReturn = CurrentApp.SendNetPipeMessage(LWebRequestClientLoading);
                IBoolIsBusy = true;

                TreeViewOrgAgent.IsEnabled = false;
                IBWSaveData = new BackgroundWorker();
                IBWSaveData.RunWorkerCompleted += IBWSaveData_RunWorkerCompleted;
                IBWSaveData.DoWork += IBWSaveData_DoWork;
                IBWSaveData.RunWorkerAsync(LListStrWcfArgs);
                this.PopupPanel.IsOpen = false;
            }
            catch (Exception ex)
            {
                IBoolIsBusy = false;
                TreeViewOrgAgent.IsEnabled = true;
                WebRequest LWebRequestClientLoading = new WebRequest();
                LWebRequestClientLoading.Code = 12112;
                WebReturn LWebReturn = CurrentApp.SendNetPipeMessage(LWebRequestClientLoading);
                if (IBWSaveData != null)
                {
                    IBWSaveData.Dispose(); IBWSaveData = null;
                }
                MessageBox.Show(ex.ToString(), "1");
            }
        }

        private void RemoveSingleAgent()
        {
            List<string> LListStrWcfArgs = new List<string>();
            string LStrItemData = string.Empty;

            try
            {
                //TreeViewItem LTreeViewItemCurrent = TreeViewOrgAgent.SelectedItem as TreeViewItem;
                //if (LTreeViewItemCurrent == null) { return; }
                if (TreeVItem == null) { return; }
                //LStrItemData = LTreeViewItemCurrent.DataContext.ToString();
                LStrItemData = TreeVItem.ObjID.ToString();
                if (LStrItemData.Substring(0, 3) != "103") { return; }

                if (MessageBox.Show(string.Format(CurrentApp.GetLanguageInfo("S1103021", ""), TreeVItem.Name), CurrentApp.AppTitle, MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) != MessageBoxResult.Yes) { return; }

                LListStrWcfArgs.Add(CurrentApp.Session.DatabaseInfo.TypeID.ToString());                                  //0
                LListStrWcfArgs.Add(CurrentApp.Session.DatabaseInfo.GetConnectionString());                              //1
                LListStrWcfArgs.Add(CurrentApp.Session.RentInfo.Token);                                                  //2
                LListStrWcfArgs.Add(CurrentApp.Session.UserInfo.UserID.ToString());                                      //3
                LListStrWcfArgs.Add("D");                                                                                   //4
                LListStrWcfArgs.Add(LStrItemData);                                                                          //5

                WebRequest LWebRequestClientLoading = new WebRequest();
                LWebRequestClientLoading.Code = 12111;
                WebReturn LWebReturn = CurrentApp.SendNetPipeMessage(LWebRequestClientLoading);
                IBoolIsBusy = true;

                TreeViewOrgAgent.IsEnabled = false;
                IBWSaveData = new BackgroundWorker();
                IBWSaveData.RunWorkerCompleted += IBWSaveData_RunWorkerCompleted;
                IBWSaveData.DoWork += IBWSaveData_DoWork;
                IBWSaveData.RunWorkerAsync(LListStrWcfArgs);
            }
            catch (Exception ex)
            {
                IBoolIsBusy = false;
                TreeViewOrgAgent.IsEnabled = true;
                WebRequest LWebRequestClientLoading = new WebRequest();
                LWebRequestClientLoading.Code = 12112;
                WebReturn LWebReturn = CurrentApp.SendNetPipeMessage(LWebRequestClientLoading);
                if (IBWSaveData != null)
                {
                    IBWSaveData.Dispose(); IBWSaveData = null;
                }
                MessageBox.Show(ex.ToString());
            }

        }

        public List<string> GetAgentParentOrg(string AStrAgentID)
        {
            List<string> LListStrReturn = new List<string>();
            string LStrItemData = string.Empty;

            TreeViewItem LTreeViewItemParent = null;

            try
            {
                TreeViewItem LTreeViewItemCurrent = TreeViewOrgAgent.SelectedItem as TreeViewItem;
                if (LTreeViewItemCurrent == null) { return LListStrReturn; }
                LStrItemData = LTreeViewItemCurrent.DataContext.ToString();
                if (LStrItemData.Substring(0, 3) != "103") { return LListStrReturn; }
                LTreeViewItemParent = LTreeViewItemCurrent.Parent as TreeViewItem;
                while (LTreeViewItemParent != null)
                {
                    LListStrReturn.Add(LTreeViewItemParent.DataContext.ToString() + ConstValue.SPLITER_CHAR + LTreeViewItemParent.Header.ToString());
                    LTreeViewItemParent = LTreeViewItemParent.Parent as TreeViewItem;
                }
            }
            catch { LListStrReturn.Clear(); }

            return LListStrReturn;
        }

        private void IBWSaveData_DoWork(object sender, DoWorkEventArgs e)
        {
            Service00000Client LService00000Client = null;
            BasicHttpBinding LBasicHttpBinding = null;
            EndpointAddress LEndpointAddress = null;
            List<string> LListStrWcfArgs = new List<string>();

            try
            {
                LListStrWcfArgs = e.Argument as List<string>;

                LBasicHttpBinding = WebHelper.CreateBasicHttpBinding(CurrentApp.Session);
                LEndpointAddress = WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service00000");
                OperationDataArgs LWCFOperationReturn = new OperationDataArgs();
                LService00000Client = new Service00000Client(LBasicHttpBinding, LEndpointAddress);
                LWCFOperationReturn = LService00000Client.OperationMethodA(27, LListStrWcfArgs);
                IStrCallReturn = LWCFOperationReturn.StringReturn;
                IBoolCallReturn = LWCFOperationReturn.BoolReturn;
                IListStrAfterSave.Clear();
                if (IBoolCallReturn)
                {
                    foreach (string LStrSingleArgs in LListStrWcfArgs) { IListStrAfterSave.Add(LStrSingleArgs); }
                    IListStrAfterSave[5] = IStrCallReturn;
                    S1103App.LoadAboutAgentData();
                }
            }
            catch (Exception ex)
            {
                IBoolCallReturn = false;
                IStrCallReturn = ex.ToString();
            }
            finally
            {
                if (LService00000Client != null)
                {
                    if (LService00000Client.State == CommunicationState.Opened) { LService00000Client.Close(); LService00000Client = null; }
                }
            }
        }

        private void IBWSaveData_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            try
            {
                IBoolIsBusy = false;
                TreeViewOrgAgent.IsEnabled = true;
                WebRequest LWebRequestClientLoading = new WebRequest();
                LWebRequestClientLoading.Code = 12112;
                WebReturn LWebReturn = CurrentApp.SendNetPipeMessage(LWebRequestClientLoading);

                string OptID = string.Empty;
                string Tname = string.Empty;
                switch (IStrCurrentMethod)
                {
                    case "A":
                        OptID = "FO1103001";
                        Tname = LStrItemData;
                        break;
                    case "E":
                        OptID = "FO1103003";
                        Tname = TreeVItem.Name;
                        break;
                    case "D":
                        OptID = "FO1103002";
                        Tname = TreeVItem.Name;
                        break;
                    case "R":
                        OptID = "FO1103004";
                        Tname = TreeVItem.Name;
                        break;
                    default:
                        break;
                }
                LStrItemData = string.Empty;
                if (!IBoolCallReturn)
                {
                    if (IStrCallReturn.Contains("W000E"))
                    {
                        //MessageBox.Show(App.GetDisplayCharater("UCAgentMaintenance", IStrCallReturn), App.Session.AppName, MessageBoxButton.OK, MessageBoxImage.Warning);
                        if (IStrCallReturn == "W000E02")
                        {
                            ShowException(CurrentApp.GetLanguageInfo("S1103019", ""));
                        }
                        else
                        {
                            ShowException(CurrentApp.GetLanguageInfo("S1103020", ""));
                        }
                    }
                    else
                    {
                        MessageBox.Show(CurrentApp.GetLanguageInfo("S1103018", "") + "\n" + IStrCallReturn, CurrentApp.Session.AppName, MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    #region 写操作日志
                    string msg_Fail = string.Format("{0} {1} :{2}", CurrentApp.Session.UserInfo.UserName, Utils.FormatOptLogString(OptID), Tname);
                    CurrentApp.WriteOperationLog(OptID.Substring(2), ConstValue.OPT_RESULT_FAIL, msg_Fail);
                    #endregion
                    return;
                }
                ResetAgentTreeViewList();
                #region 写操作日志
                string msg_Success = string.Format("{0} {1} :{2}", CurrentApp.Session.UserInfo.UserName, Utils.FormatOptLogString(OptID), Tname);
                CurrentApp.WriteOperationLog(OptID.Substring(2), ConstValue.OPT_RESULT_SUCCESS, msg_Success);
                #endregion
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "2");
            }
            finally
            {
                if (IBWSaveData != null) { IBWSaveData.Dispose(); IBWSaveData = null; }
            }
        }

        private void ResetAgentTreeViewList()
        {
            try
            {
                ShowControlOrgAgent();
            }
            catch { }
        }

        private void ShowObjectDetail()
        {
            try
            {
                if (TreeVItem == null) { return; }
                ObjectDetail.Title = TreeVItem.Name;
                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.UriSource = new Uri(string.Format("/UMPS1103;component/Themes/Default/UMPS1103/{0}", TreeVItem.Icon), UriKind.Relative);
                image.EndInit();
                ObjectDetail.Icon = image;
                List<PropertyItem> listProperties = new List<PropertyItem>();
                switch (TreeVItem.ObjID.ToString().Substring(0, 3))
                {
                    case "101":
                        //DataRow[] OrgRow = App.IDataTable11006.Select(string.Format("C001={0}", LStrItemData));
                        //if (OrgRow != null && OrgRow.Count() != 0)
                        {
                            PropertyItem property = new PropertyItem();
                            property.Name = CurrentApp.GetLanguageInfo("S1103013", "name");
                            property.ToolTip = property.Name;
                            property.Value = TreeVItem.Name;
                            listProperties.Add(property);
                            property = new PropertyItem();
                            property.Name = CurrentApp.GetLanguageInfo("S1103012", "bianhao");
                            property.ToolTip = property.Name;
                            property.Value = LStrItemData;
                            listProperties.Add(property);
                        }
                        break;
                    case "103":
                        DataRow[] AgentRow = S1103App.IDataTable11101.Select(string.Format("C001={0}", LStrItemData));
                        if (AgentRow != null && AgentRow.Count() != 0)
                        {
                            PropertyItem property = new PropertyItem();
                            property.Name = CurrentApp.GetLanguageInfo("S1103008", "name");
                            property.ToolTip = property.Name;
                            property.Value = TreeVItem.Name;
                            listProperties.Add(property);
                            property = new PropertyItem();
                            property.Name = CurrentApp.GetLanguageInfo("S1103012", "bianma");
                            property.ToolTip = property.Name;
                            property.Value = LStrItemData;
                            listProperties.Add(property);
                            property = new PropertyItem();
                            property.Name = CurrentApp.GetLanguageInfo("S1103011", "状态");
                            property.ToolTip = property.Name;
                            property.Value = AgentRow[0]["C012"].ToString();
                            if (property.Value == "1")
                            {
                                property.Value = CurrentApp.GetLanguageInfo("S1103009", "zheengcha");
                            }
                            else
                            {
                                property.Value = CurrentApp.GetLanguageInfo("S1103010", "jinyong");
                            }
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
                gvc.Header = CurrentApp.GetLanguageInfo("COL1103001", "State");
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


                gvc = new GridViewColumn();
                gvc.Header = CurrentApp.GetLanguageInfo("COL1103002", "Full Name");
                gvc.Width = 250;
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

                gvc = new GridViewColumn();
                gvc.Header = CurrentApp.GetLanguageInfo("COL1103003", "Description");
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


                gvc = new GridViewColumn();
                gvc.Header = CurrentApp.GetLanguageInfo("COL1103004", "Lock");
                gvc.Width = 80;
                DataTemplate lockMethodTemplate = (DataTemplate)Resources["LockMethodCellTemplate"];
                if (lockMethodTemplate != null)
                {
                    gvc.CellTemplate = lockMethodTemplate;
                }
                else
                {
                    gvc.DisplayMemberBinding = new Binding("LockMethod");
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

        #region DragDrop

        private void PrepareDragDropData(ObjectItem item)
        {
            if (item != null)
            {
                #region 如果没有按下Shift和Ctl，直接重置SelectInfo，然后添加当前项

                if (!Keyboard.IsKeyDown(Key.LeftShift)
                    && !Keyboard.IsKeyDown(Key.RightShift)
                    && !Keyboard.IsKeyDown(Key.LeftCtrl)
                    && !Keyboard.IsKeyDown(Key.RightCtrl))
                {
                    //重置SelectInfo,将SelectInfo里的对象的IsSelected属性都设为false
                    //然后清除所有项目
                    if (mSelectInfo != null)
                    {
                        for (int i = 0; i < mSelectInfo.ListItems.Count; i++)
                        {
                            mSelectInfo.ListItems[i].IsSelected = false;
                        }
                        mSelectInfo.ListItems.Clear();
                        mSelectInfo.Parent = null;
                    }
                    ////插入当前ObjectItem，将IsSelected设为True
                    ////暂时只能是用户对象可以拖放
                    //if (item.ObjType == ConstValues.OBJTYPE_USER)
                    //{
                    //    item.IsSelected = true;
                    //    mSelectInfo = new SelectedInfo();
                    //    mSelectInfo.ObjType = item.ObjType;
                    //    mSelectInfo.Parent = item.Parent;
                    //    mSelectInfo.ListItems.Add(item);
                    //}
                    item.IsSelected = true;
                    mSelectInfo = new SelectedInfo();
                    mSelectInfo.ObjType = item.ObjType;
                    mSelectInfo.Parent = item.Parent;
                    mSelectInfo.ListItems.Add(item);
                }

                #endregion

                #region 如果按下了Shift或Ctl键，向SelectInfo里追加

                else
                {
                    //如果SelectInfo为空，创建并初始化
                    if (mSelectInfo == null)
                    {
                        mSelectInfo = new SelectedInfo();
                        mSelectInfo.ObjType = item.ObjType;
                        mSelectInfo.Parent = item.Parent;
                    }
                    //如果类型不同，重置SelectInfo
                    if (mSelectInfo.ObjType != item.ObjType)
                    {
                        for (int i = 0; i < mSelectInfo.ListItems.Count; i++)
                        {
                            mSelectInfo.ListItems[i].IsSelected = false;
                        }
                        mSelectInfo.ListItems.Clear();
                        mSelectInfo.ObjType = item.ObjType;
                        mSelectInfo.Parent = item.Parent;
                    }
                    var parent = mSelectInfo.Parent;
                    if (parent != null)
                    {
                        //如果当前父级与SelectInfo的父级不同，重置SelectInfo
                        if (parent != item.Parent)
                        {
                            for (int i = 0; i < mSelectInfo.ListItems.Count; i++)
                            {
                                mSelectInfo.ListItems[i].IsSelected = false;
                            }
                            mSelectInfo.ListItems.Clear();
                            mSelectInfo.Parent = item.Parent;
                        }
                        //如果按下Ctl项，追加当前项，否则追加一系列连续的项（按下了Shift键）
                        if (Keyboard.IsKeyDown(Key.LeftCtrl)
                            || Keyboard.IsKeyDown(Key.RightCtrl))
                        {
                            item.IsSelected = true;
                            if (!mSelectInfo.ListItems.Contains(item))
                            {
                                mSelectInfo.ListItems.Add(item);
                            }
                        }
                        else
                        {
                            var objParent = parent as ObjectItem;
                            if (objParent != null)
                            {
                                //取得最小和最大索引
                                int index, minIndex = int.MaxValue, maxIndex = int.MinValue, currentIndex;
                                for (int i = 0; i < mSelectInfo.ListItems.Count; i++)
                                {
                                    index = objParent.Children.IndexOf(mSelectInfo.ListItems[i]);
                                    if (index >= 0)
                                    {
                                        minIndex = Math.Min(index, minIndex);
                                        maxIndex = Math.Max(index, maxIndex);
                                    }
                                }
                                currentIndex = objParent.Children.IndexOf(item);
                                if (currentIndex >= 0)
                                {
                                    minIndex = Math.Min(minIndex, currentIndex);
                                    maxIndex = Math.Max(maxIndex, currentIndex);

                                    //追加需要追加的项
                                    List<ObjectItem> tempList = new List<ObjectItem>();
                                    ObjectItem tempItem;
                                    for (int i = minIndex; i <= maxIndex; i++)
                                    {
                                        tempItem = objParent.Children[i] as ObjectItem;
                                        if (tempItem != null && tempItem.ObjType == mSelectInfo.ObjType)
                                        {
                                            tempList.Add(tempItem);
                                            if (!mSelectInfo.ListItems.Contains(tempItem))
                                            {
                                                tempItem.IsSelected = true;
                                                mSelectInfo.ListItems.Add(tempItem);
                                            }
                                        }
                                    }
                                    //重置需要重置的项
                                    for (int i = 0; i < mSelectInfo.ListItems.Count; i++)
                                    {
                                        tempItem = mSelectInfo.ListItems[i];
                                        if (!tempList.Contains(tempItem))
                                        {
                                            tempItem.IsSelected = false;
                                            mSelectInfo.ListItems.Remove(tempItem);
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

        private void ObjectItem_StartDragged(object sender, DragDropEventArgs e)
        {
            var dragSource = e.DragSource;
            var dragData = mSelectInfo;
            if (dragSource != null && dragData != null)
            {
                DragDrop.DoDragDrop(dragSource, dragData, DragDropEffects.Move);
            }
        }

        private void ObjectItem_DragOver(object sender, DragDropEventArgs e)
        {

        }

        private void ObjectItem_Dropped(object sender, DragDropEventArgs e)
        {
            var currentObjItem = sender as ObjectItem;
            if (currentObjItem != null)
            {
                var dragData = e.DragData as IDataObject;
                if (dragData != null)
                {
                    var selectInfo = dragData.GetData(typeof(SelectedInfo)) as SelectedInfo;
                    if (selectInfo != null)
                    {
                        MoveObject(selectInfo, currentObjItem);
                    }
                }
            }
        }

        private void MoveObject(SelectedInfo selectInfo, ObjectItem targetItem)
        {
            if (targetItem == null || selectInfo == null || selectInfo.Parent == null) { return; }

            //如果按下了方向键，同一父亲下调换位置
            if (Keyboard.IsKeyDown(Key.U))
            {
                //目标项与源项目父亲要相同
                if (targetItem.Parent != selectInfo.Parent) { return; }
                //源项目不能包含有目标项
                if (selectInfo.ListItems.Contains(targetItem)) { return; }
                ChangeObjectIndex(selectInfo, targetItem.Parent as ObjectItem, targetItem, false);
            }
            else if (Keyboard.IsKeyDown(Key.D))
            {
                //目标项与源项目父亲要相同
                if (targetItem.Parent != selectInfo.Parent) { return; }
                //源项目不能包含有目标项
                if (selectInfo.ListItems.Contains(targetItem)) { return; }
                ChangeObjectIndex(selectInfo, targetItem.Parent as ObjectItem, targetItem, true);
            }
            else
            {
                //目标项不能是源项目的父亲
                if (targetItem == selectInfo.Parent) { return; }
                //目标项只能是机构类型
                //var targetOrg = targetItem.Data as BasicOrgInfo;
                if (targetItem == null) { return; }
                if (targetItem.ObjType != ConstValue.RESOURCE_ORG) { return; }
                if (selectInfo.ObjType == ConstValue.RESOURCE_AGENT)
                {
                    MoveAgent(selectInfo, targetItem);
                    S1103App.LoadAboutAgentData();
                    ResetAgentTreeViewList();
                }
            }
        }

        private void MoveAgent(SelectedInfo selectInfo, ObjectItem orgItem)
        {
            try
            {
                //var orgInfo = orgItem.Data as BasicOrgInfo;
                if (orgItem == null) { return; }
                string strUserID = string.Empty;
                string strInfoMsg = string.Empty;
                for (int i = 0; i < selectInfo.ListItems.Count; i++)
                {
                    //var userItem = selectInfo.ListItems[i].Data as BasicUserInfo;
                    var userItem = selectInfo.ListItems[i];
                    if (userItem != null)
                    {
                        //管理员不能移动
                        //if (userItem.UserID == S1101Consts.USER_ADMIN || userItem.UserID == App.Session.UserID) { continue; }
                        strUserID += string.Format("{0}{1}", userItem.ObjID, ConstValue.SPLITER_CHAR);
                        strInfoMsg += string.Format("{0} \t", userItem.FullName);
                    }
                }
                strUserID = strUserID.TrimEnd(new[] { ConstValue.SPLITER_CHAR });
                if (string.IsNullOrEmpty(strUserID)) { return; }

                strInfoMsg = string.Format("{0}\r\n\r\n{1} -> {2}",
                    CurrentApp.GetMessageLanguageInfo("004", "Confirm Move User ?"),
                    strInfoMsg,
                    orgItem.Name);
                var result = MessageBox.Show(strInfoMsg, CurrentApp.AppName, MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result != MessageBoxResult.Yes) { return; }
                SetBusy(true, string.Empty);
                mWorker = new BackgroundWorker();
                mWorker.DoWork += (s, de) =>
                {
                    try
                    {
                        WebRequest webRequest = new WebRequest();
                        webRequest.Session = CurrentApp.Session;
                        webRequest.Code = 30;
                        webRequest.ListData.Add(orgItem.ObjID.ToString());
                        webRequest.ListData.Add(strUserID);
                        Service11011Client client = new Service11011Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                            WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11011"));
                        WebHelper.SetServiceClient(client);
                        WebReturn webReturn = client.DoOperation(webRequest);
                        client.Close();
                        if (!webReturn.Result)
                        {
                            ShowException(string.Format("{0}\t{1}", webReturn.Code, webReturn.Message));
                        }
                    }
                    catch (Exception ex)
                    {
                        ShowException(ex.Message);
                    }
                };
                mWorker.RunWorkerCompleted += (s, re) =>
                {
                    mWorker.Dispose();
                    SetBusy(false, string.Empty);
                };
                mWorker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void ChangeObjectIndex(SelectedInfo selectInfo, ObjectItem parentItem, ObjectItem targetItem, bool isDown)
        {
            try
            {
                if (selectInfo == null || parentItem == null || targetItem == null) { return; }

                //用一个集合暂存待移动的项
                List<ObjectItem> listTemp = new List<ObjectItem>();
                for (int i = 0; i < selectInfo.ListItems.Count; i++)
                {
                    listTemp.Add(selectInfo.ListItems[i]);
                }
                for (int i = 0; i < listTemp.Count; i++)
                {
                    var tempItem = listTemp[i];
                    if (parentItem.Children.Contains(tempItem))
                    {
                        parentItem.Children.Remove(tempItem);
                    }
                }
                var index = parentItem.Children.IndexOf(targetItem);
                if (index >= 0)
                {
                    if (isDown)
                    {
                        for (int i = index + 1; i <= listTemp.Count + index; i++)
                        {
                            var tempItem = listTemp[i - index - 1];
                            if (i > parentItem.Children.Count) { parentItem.Children.Add(tempItem); }
                            else
                            {
                                parentItem.Children.Insert(i, tempItem);
                            }
                        }
                    }
                    else
                    {
                        for (int i = index; i < listTemp.Count + index; i++)
                        {
                            var tempItem = listTemp[i - index];
                            if (i > parentItem.Children.Count) { parentItem.Children.Add(tempItem); }
                            else
                            {
                                parentItem.Children.Insert(i, tempItem);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }
        #endregion
    }
}
