using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using Common3603;
using UMPS3603.Models;
using UMPS3603.Wcf36031;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;
using VoiceCyber.Wpf.AvalonDock.Layout;

namespace UMPS3603
{
    /// <summary>
    /// Page1.xaml 的交互逻辑
    /// </summary>
    public partial class ExamProductionPage
    {
        #region Members
        private BackgroundWorker _mWorker;
        private readonly ObservableCollection<TestInfoParam> _mObservableCollectionTestInfo;
        private readonly ObservableCollection<TestUserParam> _mObservableCollectionTestUser;
        private readonly List<TestInfoParam> _mlstTestInformations;
        private TestInfoParam _mTestInfoTemp;
        private readonly List<TestUserParam> _mlstTestUserParam;
        private TestUserParam _mTestUserParamTemp;
        private static bool _mbPaperTable = false;
        private static bool _mbExamineeInfoTable = false;
        private List<PanelItem> _mListPanels;
        #endregion

        public ExamProductionPage()
        {
            InitializeComponent();
            _mObservableCollectionTestInfo = new ObservableCollection<TestInfoParam>();
            _mObservableCollectionTestUser = new ObservableCollection<TestUserParam>();
            _mlstTestInformations = new List<TestInfoParam>();
            _mlstTestUserParam = new List<TestUserParam>();
            _mTestUserParamTemp = new TestUserParam();
            _mListPanels = new List<PanelItem>();
            Loaded += PapersListInfo_Loaded;
        }

        void PapersListInfo_Loaded(object sender, RoutedEventArgs e)
        {
            _mObservableCollectionTestInfo.Clear();
            try
            {
                SetBusy(true);
                _mWorker = new BackgroundWorker();
                _mWorker.WorkerReportsProgress = true;
                _mWorker.WorkerSupportsCancellation = true;
                //注册线程主体方法
                _mWorker.DoWork += (s, de) =>
                {
                    /*GetUmpSetupPath();*/
                    //InitCategoryTreeInfo();
                };
                _mWorker.RunWorkerCompleted += (s, re) =>
                {
                    ChangeLanguage();
                    ChangeTheme();
                    TestInfoListTable.ItemsSource = _mObservableCollectionTestInfo;
                    TestUserListTable.ItemsSource = _mObservableCollectionTestUser;
                    GetAllTestInfos();
                    _mWorker.Dispose();
                    SetBusy(false);
                };
                _mWorker.RunWorkerAsync(); //触发DoWork事件
            }
            catch (Exception)
            {
                SetBusy(false);
            }
        }

        #region 初始化 & 全局消息
        protected override void Init()
        {
            try
            {
                PageHead.AppName = UMPApp.GetLanguageInfo("3603T00003", "Examination Management"); 
                StylePath = "UMPS3603/MainPageStyle.xmal";
                base.Init();

                InitPanels();
                CreateToolBarButtons();
                SendLoadedMessage();
                ChangeTheme();
                ChangeLanguage();

            }
            catch (Exception ex)
            {
                UMPApp.ShowExceptionMessage(ex.Message);
            }
        }

        protected override void PageHead_PageHeadEvent(object sender, PageHeadEventArgs e)
        {
            base.PageHead_PageHeadEvent(sender, e);

            switch (e.Code)
            {
                //切换主题
                case 100:
                    ThemeInfo themeInfo = e.Data as ThemeInfo;
                    if (themeInfo != null)
                    {
                        ThemeInfo = themeInfo;
                        UMPApp.Session.ThemeInfo = themeInfo;
                        UMPApp.Session.ThemeName = themeInfo.Name;
                        ChangeTheme();
                        SendThemeChangeMessage();
                    }
                    break;
                //切换语言
                case 110:
                    LangTypeInfo langType = e.Data as LangTypeInfo;
                    if (langType != null)
                    {
                        LangTypeInfo = langType;
                        UMPApp.Session.LangTypeInfo = langType;
                        UMPApp.Session.LangTypeID = langType.LangID;
                        MyWaiter.Visibility = Visibility.Visible;
                        _mWorker = new BackgroundWorker();
                        _mWorker.DoWork += (s, de) => UMPApp.InitAllLanguageInfos();
                        _mWorker.RunWorkerCompleted += (s, re) =>
                        {
                            _mWorker.Dispose();
                            MyWaiter.Visibility = Visibility.Hidden;
                            ChangeLanguage();
                            PopupCreateTest.ChangeLanguage();
                            SendLanguageChangeMessage();
                        };
                        _mWorker.RunWorkerAsync();
                    }
                    break;
            }
        }

        protected override void App_NetPipeEvent(WebRequest webRequest)
        {
            base.App_NetPipeEvent(webRequest);

            Dispatcher.Invoke(new Action(() =>
            {
                try
                {
                    var code = webRequest.Code;
                    var strData = webRequest.Data;
                    switch (code)
                    {
                        case (int)RequestCode.SCLanguageChange:
                            LangTypeInfo langTypeInfo = UMPApp.Session.SupportLangTypes.FirstOrDefault(l => l.LangID.ToString() == strData);
                            if (langTypeInfo != null)
                            {
                                LangTypeInfo = langTypeInfo;
                                UMPApp.Session.LangTypeInfo = langTypeInfo;
                                UMPApp.Session.LangTypeID = langTypeInfo.LangID;
                                if (MyWaiter != null)
                                {
                                    MyWaiter.Visibility = Visibility.Visible;
                                }
                                _mWorker = new BackgroundWorker();
                                _mWorker.DoWork += (s, de) => UMPApp.InitAllLanguageInfos();
                                _mWorker.RunWorkerCompleted += (s, re) =>
                                {
                                    _mWorker.Dispose();
                                    if (MyWaiter != null)
                                    {
                                        MyWaiter.Visibility = Visibility.Hidden;
                                    }
                                    if (PopupCreateTest != null)
                                    {
                                        PopupCreateTest.ChangeLanguage();
                                    }
                                    if (PageHead != null)
                                    {
                                        PageHead.SessionInfo = UMPApp.Session;
                                        PageHead.InitInfo();
                                    }
                                };
                                _mWorker.RunWorkerAsync();
                            }
                            break;
                        case (int)RequestCode.SCThemeChange:
                            ThemeInfo themeInfo = UMPApp.Session.SupportThemes.FirstOrDefault(t => t.Name == strData);
                            if (themeInfo != null)
                            {
                                ThemeInfo = themeInfo;
                                UMPApp.Session.ThemeInfo = themeInfo;
                                UMPApp.Session.ThemeName = themeInfo.Name;
                                ChangeTheme();
                                PageHead.SessionInfo = UMPApp.Session;
                                PageHead.InitInfo();
                            }
                            break;
                    }
                }
                catch (Exception ex)
                {
                    UMPApp.ShowExceptionMessage(ex.Message);
                }
            }));
        }

        private void InitButton()
        {
            BasicSetBut.Children.Clear();
            Button btn;
            OperationInfo opt;
            btn = new Button();
            btn.Click += AddTestInfo_Click;
            opt = new OperationInfo();
            opt.Display = UMPApp.GetLanguageInfo("3603T00006", "Add Test Information");
            opt.Icon = "Images/add.png";
            btn.DataContext = opt;
            btn.SetResourceReference(StyleProperty, "OptButtonStyle");
            BasicSetBut.Children.Add(btn);
        }

        private void InitTestInfoBut()
        {
            TestInfoBut.Children.Clear();
            Button btn;
            OperationInfo opt;
            btn = new Button();
            btn.Click += AddTestUser_Click;
            opt = new OperationInfo();
            opt.Display = UMPApp.GetLanguageInfo("3603T00007", "Set Test User");
            opt.Icon = "Images/add.png";
            btn.DataContext = opt;
            btn.SetResourceReference(StyleProperty, "OptButtonStyle");
            TestInfoBut.Children.Add(btn);
        }

        private void InitDeleteBut()
        {
            DeleteSetBut.Children.Clear();
            Button btn;
            OperationInfo opt;
            btn = new Button();
            btn.Click += DeleteTestInfo_Click;
            opt = new OperationInfo();
            opt.Display = UMPApp.GetLanguageInfo("3603T00008", "Delete Test");
            opt.Icon = "Images/Delete.png";
            btn.DataContext = opt;
            btn.SetResourceReference(StyleProperty, "OptButtonStyle");
            DeleteSetBut.Children.Add(btn);
        }

        void InitTestListTable()
        {
            try
            {
                string[] lans = "3603T00009,3603T00010,3603T00011,3603T00012,3603T00013,3603T00014,3603T00015,3603T00016,3603T00017".Split(',');
                string[] cols = "LongTestNum,LongPaperNum,StrPaperName,StrExplain,StrTestTime,LongEditorId,StrEditor,StrDateTime,StrTestStatue".Split(',');
                int[] colwidths = { 150, 150, 150, 100, 60, 180, 80, 100, 100, 100, 100 };
                var columnGridView = new GridView();

                for (int i = 0; i < cols.Length; i++)
                {
                    var gvc = new GridViewColumn
                    {
                        Header = UMPApp.GetLanguageInfo(lans[i], cols[i]),
                        Width = colwidths[i],
                        DisplayMemberBinding = new Binding(cols[i])
                    };
                    columnGridView.Columns.Add(gvc);
                }
                TestInfoListTable.View = columnGridView;
            }
            catch (Exception ex)
            {
                UMPApp.ShowExceptionMessage(ex.Message);
            }
        }

        void InitUserListTable()
        {
            try
            {
                string[] lans = "3603T00018,3603T00019,3603T00020,3603T00021,3603T00022,3603T00023,3603T00024,3603T00025,3603T00026".Split(',');
                string[] cols = "LongTestNum,LongTestUserNum,StrTestUserName,LongPaperNum,StrPaperName,IntScore,StrStartTime,StrEndTime,StrTestStatue".Split(',');
                int[] colwidths = { 150, 150, 150, 100, 60, 180, 80, 100, 100, 100, 100 };
                var columnGridView = new GridView();

                for (int i = 0; i < cols.Length; i++)
                {
                    var gvc = new GridViewColumn
                    {
                        Header = UMPApp.GetLanguageInfo(lans[i], cols[i]),
                        Width = colwidths[i],
                        DisplayMemberBinding = new Binding(cols[i])
                    };
                    columnGridView.Columns.Add(gvc);
                }
                TestUserListTable.View = columnGridView;
            }
            catch (Exception ex)
            {
                UMPApp.ShowExceptionMessage(ex.Message);
            }
        }

        private void InitPanels()
        {
            try
            {
                _mListPanels.Clear();
                PanelItem panelItem = new PanelItem();
                panelItem.PanelId = S3603Consts.PANEL_ID_EXAMINFO;
                panelItem.Name = S3603Consts.PANEL_NAME_EXAMINFO;
                panelItem.ContentId = S3603Consts.PANEL_CONTENTID_EXAMINFO;
                panelItem.Title = "Question Category";
                panelItem.Icon = "Images/00005.png";
                panelItem.IsVisible = true;
                panelItem.CanClose = true;
                _mListPanels.Add(panelItem);

                panelItem = new PanelItem();
                panelItem.PanelId = S3603Consts.PANEL_ID_PAPER;
                panelItem.Name = S3603Consts.PANEL_NAME_PAPER;
                panelItem.ContentId = S3603Consts.PANEL_CONTENTID_PAPER;
                panelItem.Title = "Question Category";
                panelItem.Icon = "Images/00005.png";
                panelItem.IsVisible = true;
                panelItem.CanClose = true;
                _mListPanels.Add(panelItem);
            }
            catch (Exception ex)
            {
                UMPApp.ShowExceptionMessage(ex.Message);
            }
        }

        private void CreateToolBarButtons()
        {
            try
            {
                PanelToolButton.Children.Clear();
                CToolButtonItem toolItem;
                ToggleButton toggleBtn;
                for (int i = 0; i < _mListPanels.Count; i++)
                {
                    PanelItem item = _mListPanels[i];
                    if (!item.CanClose) { continue; }
                    toolItem = new CToolButtonItem();
                    toolItem.Name = "TB" + item.Name;
                    toolItem.Display = item.Title;
                    toolItem.Tip = item.Title;
                    toolItem.Icon = item.Icon;
                    toggleBtn = new ToggleButton();
                    toggleBtn.Click += PanelToggleButton_Click;
                    toggleBtn.DataContext = toolItem;
                    toggleBtn.IsChecked = item.IsVisible;
                    toggleBtn.SetResourceReference(StyleProperty, "ToolBarToggleBtnStyle");
                    PanelToolButton.Children.Add(toggleBtn);
                }
            }
            catch (Exception ex)
            {
                UMPApp.ShowExceptionMessage(ex.Message);
            }
        }

        #endregion

        #region 样式&语言
        public override void ChangeTheme()
        {
            base.ChangeTheme();

            bool bPage = true;
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
                catch (Exception ex)
                {
                    //App.ShowExceptionMessage("1" + ex.Message);
                }
            }
            if (!bPage)
            {
                //如果通过Url没有加载成功，就从已经编译到程序集的默认资源加载
                try
                {
                    string uri = string.Format("/Themes/{0}/{1}",
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
                string uri = string.Format("/Themes/Default/UMPS3603/MainPageStyle.xaml");
                ResourceDictionary resource = new ResourceDictionary();
                resource.Source = new Uri(uri, UriKind.RelativeOrAbsolute);
                Resources.MergedDictionaries.Add(resource);
            }
            catch (Exception ex)
            {
                //App.ShowExceptionMessage("3" + ex.Message);
            }

            var pageHead = PageHead;
            if (pageHead != null)
            {
                pageHead.ChangeTheme();
                pageHead.InitInfo();
            }

        }

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();

            InitButton();
            InitTestListTable();
            InitUserListTable();
            InitLanguage();

            if (PageHead != null)
            {
                PageHead.AppName = UMPApp.GetLanguageInfo("3603T00003", "Examination Management"); 
            }
        }

        private void InitLanguage()
        {
            BsExpandStyleOpt.Header = UMPApp.GetLanguageInfo("3603T00001", "Basic Setting");
            DeleteExpandStyleOpt.Header = UMPApp.GetLanguageInfo("3603T00002", "Delete Setting");
            TestInformationTable.Title = UMPApp.GetLanguageInfo("3603T00004", "Test Information");
            ExamineeInformationTable.Title = UMPApp.GetLanguageInfo("3603T00005", "Examinee Information");
            PanelObjectPaper.Title = UMPApp.GetLanguageInfo("3603T00036", "Paper");
        }

        #endregion

        #region Click

        void PanelToggleButton_Click(object sender, RoutedEventArgs e)
        {
            var toggleBtn = e.Source as ToggleButton;
            if (toggleBtn != null)
            {
                CToolButtonItem item = toggleBtn.DataContext as CToolButtonItem;
                if (item != null)
                {
                    PanelItem panelItem = _mListPanels.FirstOrDefault(p => string.Format("TB{0}", p.Name) == item.Name);
                    if (panelItem == null) { return; }
                    panelItem.IsVisible = toggleBtn.IsChecked == true;
                }
                SetPanelVisible();
            }
        }

        private void PanelDocument_Closing(object sender, CancelEventArgs e)
        {
            try
            {
                e.Cancel = true;
                var panel = sender as LayoutAnchorable;
                if (panel != null)
                {
                    panel.Hide();
                }
            }
            catch { }
        }

        private void AddTestInfo_Click(object sender, RoutedEventArgs e)
        {
            App.GQueryModify = false;
            PaperInfo newPage = new PaperInfo();
            newPage.ParentPage = this;
            PopupCreateTest.Content = newPage;
            PopupCreateTest.Title = UMPApp.GetLanguageInfo("3603T00055", "Paper Information");
            PopupCreateTest.IsOpen = true;
        }

        private void AddTestUser_Click(object sender, RoutedEventArgs e)
        {
            TestInfoParam item = _mTestInfoTemp;
            if (item != null)
            {
                UserManagement newpage = new UserManagement();
                newpage.PageParent = this;
                newpage._mTestInfoParam = item;
                PopupTestUser.Content = newpage;
                PopupTestUser.Title = UMPApp.GetLanguageInfo("3603T00007", "ScoreSheet User Management");
                PopupTestUser.IsOpen = true;
            }
        }

        private void DeleteTestInfo_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result1 =
                        MessageBox.Show(
                            UMPApp.GetLanguageInfo("3603T00066",
                                "Sure you want to delete?"),
                            UMPApp.GetLanguageInfo("3603T00090", "Warning"),
                            MessageBoxButton.OKCancel);
            if (result1 == MessageBoxResult.Cancel)
            {
                return;
            }

            TestInfoParam item = _mTestInfoTemp;
            if (item != null)
            {
                if (item.StrTestStatue == TestStratue.USED)
                {
                    MessageBoxResult result =
                        MessageBox.Show(
                            UMPApp.GetLanguageInfo("3603T00094",
                                "The examination paper is already in use cannot be deleted!"),
                            UMPApp.GetLanguageInfo("3603T00090", "Warning"),
                            MessageBoxButton.OKCancel);
                    return;
                }

                if (DeleteTest(item))
                {
                    UMPApp.ShowInfoMessage(UMPApp.GetLanguageInfo("3603T00054", "Success!"));
                    _mlstTestInformations.Remove(_mlstTestInformations.FirstOrDefault(p => p.LongTestNum == item.LongTestNum));
                    SetObservableCollectionTestInfo();
                    _mObservableCollectionTestUser.Clear();
                }
            }
        }

        private void TestInformationTable_IsSelectedChanged(object sender, EventArgs e)
        {

        }

        private void PanelObjectPaper_IsSelectedChanged(object sender, EventArgs e)
        {
            if (!_mbPaperTable)
            {
                var item = TestInfoListTable.SelectedItem as TestInfoParam;
                if (item != null)
                {
                    App.GQueryModify = false;
                    App.GTestInformation = new TestInfoParam();
                    App.GTestInformation = item;
                    App.GPaperParam = new CPaperParam();
                    CPaperParam paperParam = new CPaperParam();
                    if (GetPaperInfo(SqlSearchPaper(item), out paperParam))
                    {
                        App.GPaperParam = paperParam;
                        TestPaperPage testPaperPage = new TestPaperPage();
                        TestPaperPageName.Children.Add(testPaperPage);
                    }
                    
                }
                _mbPaperTable = true;
                _mbExamineeInfoTable = false;
            }
        }


        void TestInformationTable_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                _mTestInfoTemp = null;
                var item = TestInfoListTable.SelectedItem as TestInfoParam;
                _mTestInfoTemp = item;
                if (item != null)
                {
                    InitTestInfoBut();
                    GetTestUserInfo(_mTestInfoTemp);
                    InitDeleteBut();
                }
            }
            catch (Exception ex)
            {
                UMPApp.ShowExceptionMessage(ex.Message);
            }

        }

        void TestInformationTable_KeyDown(object sender, KeyEventArgs e)
        {
           
        }

        private void ExamineeInformationTable_IsSelectedChanged(object sender, EventArgs e)
        {
            if (!_mbExamineeInfoTable)
            {
                _mbExamineeInfoTable = true;
                _mbPaperTable = false;
            }
        }
        #endregion

        #region Operations
        public void OpenTestInfo()
        {
            App.GQueryModify = false;
            AddTestInfo newPage = new AddTestInfo();
            newPage.ParentPage = this;
            PopupCreateTest.Content = newPage;
            PopupCreateTest.Title = UMPApp.GetLanguageInfo("3603T00055", "Paper Information");
            PopupCreateTest.IsOpen = true;
        }

        public void SetBusy(bool isBusying)
        {
            MyWaiter.Visibility = isBusying ? Visibility.Visible : Visibility.Collapsed;
        }

        private void GetTestUserInfo(TestInfoParam testInfoTemp)
        {
            _mlstTestUserParam.Clear();
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = App.Session;
                webRequest.Code = (int)S3603Codes.OptGetTestUserInfo;
                                         Service36031Client client = new Service36031Client(WebHelper.CreateBasicHttpBinding(App.Session),
                                          WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service36031"));
                //var client = new Service36031Client();
                OperationReturn optReturn = XMLHelper.SeriallizeObject(testInfoTemp);
                if (!optReturn.Result)
                {
                    UMPApp.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                webRequest.ListData.Add(optReturn.Data.ToString());
                WebReturn webReturn = client.UmpTaskOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                App.WriteLog("GetTestUserInfo", webReturn.Data);

                if (webReturn.ListData.Count <= 0)
                {
                    _mObservableCollectionTestUser.Clear();
                    return;
                }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    optReturn = XMLHelper.DeserializeObject<TestUserParam>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        UMPApp.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        continue;
                    }
                    var testInfo = optReturn.Data as TestUserParam;
                    if (testInfo == null)
                    {
                        UMPApp.ShowExceptionMessage("Fail. filesItem is null");
                        return;
                    }

                    _mlstTestUserParam.Add(testInfo);
                }
                SetObservableCollectionTestUser();
            }
            catch (Exception ex)
            {
                UMPApp.ShowExceptionMessage(ex.Message);
            }
        }

        private void GetAllTestInfos()
        {
            try
            {
                _mlstTestInformations.Clear();
                var webRequest = new WebRequest();
                Service36031Client client = new Service36031Client(WebHelper.CreateBasicHttpBinding(App.Session), WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service36031"));
                //var client = new Service36031Client();
                webRequest.Session = UMPApp.Session;
                webRequest.Code = (int)S3603Codes.OptGetTestInfo;

                WebReturn webReturn = client.UmpTaskOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    UMPApp.ShowExceptionMessage(UMPApp.GetLanguageInfo("3603T00015", "Insert data failed"));
                    return;
                }

                if (webReturn.ListData.Count <= 0) { return; }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<TestInfoParam>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        UMPApp.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        continue;
                    }
                    var testInfo = optReturn.Data as TestInfoParam;
                    if (testInfo == null)
                    {
                        UMPApp.ShowExceptionMessage("Fail. filesItem is null");
                        return;
                    }

                    _mlstTestInformations.Add(testInfo);
                }
                SetObservableCollectionTestInfo();
            }
            catch (Exception ex)
            {
                UMPApp.ShowExceptionMessage(ex.Message);
            }
        }

        private void SetObservableCollectionTestInfo()
        {
            _mObservableCollectionTestInfo.Clear();
            foreach (var param in _mlstTestInformations)
            {
                _mObservableCollectionTestInfo.Add(param);
            }
        }

        private void SetObservableCollectionTestUser()
        {
            _mObservableCollectionTestUser.Clear();
            foreach (var param in _mlstTestUserParam)
            {
                _mObservableCollectionTestUser.Add(param);
            }
        }

        public void UpdateTestInfoTable()
        {
            if( App.GTestInformation != null)
                _mObservableCollectionTestInfo.Add(App.GTestInformation);
        }

        private bool DeleteTest(TestInfoParam testInfoParam)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = UMPApp.Session;
                webRequest.Code = (int)S3603Codes.OptDeleteTestInfo;
                 Service36031Client client = new Service36031Client(WebHelper.CreateBasicHttpBinding(App.Session),
                     WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service36031"));
                //var client = new Service36031Client();
                OperationReturn optReturn = XMLHelper.SeriallizeObject(testInfoParam);
                if (!optReturn.Result)
                {
                    UMPApp.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return false;
                }
                webRequest.ListData.Add(optReturn.Data.ToString());
                //Service36031Client client = new Service36031Client();
                WebReturn webReturn = client.UmpTaskOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    UMPApp.ShowExceptionMessage(UMPApp.GetLanguageInfo("3603T00083", "Delete Failed"));
                    return false;
                }
                if (webReturn.Message == S3603Consts.HadUse)// 该查询条件被使用无法删除
                {
                    UMPApp.ShowInfoMessage(UMPApp.GetLanguageInfo("3603T00011", "Can't Delete"));
                    return false;
                }
            }
            catch (Exception ex)
            {
                UMPApp.ShowExceptionMessage(ex.Message);
                return false;
            }
            return true;
        }

        private string SqlSearchPaper( TestInfoParam testInfoParam)
        {
            string sql = string.Empty;
            sql = string.Format("SELECT * FROM T_36_023_{0} WHERE c001='{1}'", UMPApp.Session.RentInfo.Token,
                testInfoParam.LongPaperNum);
            return sql;
        }

        private bool GetPaperInfo(string strSql,out CPaperParam paperParam)
        {
            paperParam = new CPaperParam();
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = UMPApp.Session;
                webRequest.Code = (int)S3603Codes.OptSearchPapers;
                OperationReturn optReturn = XMLHelper.SeriallizeObject(strSql);
                if (!optReturn.Result)
                {
                    UMPApp.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return false;
                }
                webRequest.ListData.Add(optReturn.Data.ToString());
                Service36031Client client = new Service36031Client(WebHelper.CreateBasicHttpBinding(App.Session),
                    WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service36031"));
                //var client = new Service36031Client();
                WebReturn webReturn = client.UmpTaskOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    UMPApp.ShowExceptionMessage(string.Format("{0}: {1}",
                        UMPApp.GetLanguageInfo("3603T00065", "Search Failed"), webReturn.Message));
                    return false;
                }

                if (webReturn.ListData.Count <= 0) { return true; }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    optReturn = XMLHelper.DeserializeObject<CPaperParam>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        UMPApp.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        continue;
                    }
                    var paperInfo = optReturn.Data as CPaperParam;
                    if (paperInfo == null)
                    {
                        UMPApp.ShowExceptionMessage("Fail. filesItem is null");
                        return false;
                    }

                    paperParam = paperInfo;
                }
                
            }
            catch (Exception ex)
            {
                UMPApp.ShowExceptionMessage(ex.Message);
            }
            return true;
        }

        private void SetPanelVisible()
        {
            try
            {
                for (int i = 0; i < _mListPanels.Count; i++)
                {
                    var item = _mListPanels[i];
                    var panel = PanelManager.Layout.Descendents().OfType<LayoutAnchorable>().FirstOrDefault(p => p.ContentId == item.ContentId);
                    if (panel != null)
                    {
                        switch (item.PanelId)
                        {
                            case 1:
                                panel.Title = UMPApp.GetLanguageInfo("3603T00005", item.Name);
                                break;
                            case 2:
                                panel.Title = UMPApp.GetLanguageInfo("3603T00036", item.Name);
                                break;
                        }

                        if (item.IsVisible)
                        {
                            panel.Show();
                        }
                        else
                        {
                            panel.Hide();
                        }
                        LayoutAnchorable panel1 = panel;
                        panel.IsVisibleChanged += (s, e) =>
                        {
                            item.IsVisible = panel1.IsVisible;
                            SetViewStatus();
                        };
                        panel.Closing += PanelDocument_Closing;
                    }
                }
            }
            catch (Exception ex)
            {
                UMPApp.ShowExceptionMessage(ex.Message);
            }
        }

        private void SetViewStatus()
        {
            for (int i = 0; i < PanelToolButton.Children.Count; i++)
            {
                var toggleBtn = PanelToolButton.Children[i] as ToggleButton;
                if (toggleBtn != null)
                {
                    var item = toggleBtn.DataContext as CToolButtonItem;
                    if (item == null) { continue; }
                    PanelItem panelItem = _mListPanels.FirstOrDefault(p => string.Format("TB{0}", p.Name) == item.Name);
                    if (panelItem != null)
                    {
                        toggleBtn.IsChecked = panelItem.IsVisible;
                    }
                }
            }
        }

        #endregion
    }
}
