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
using VoiceCyber.Wpf.AvalonDock.Layout;

namespace UMPS3603
{
    /// <summary>
    /// Page1.xaml 的交互逻辑
    /// </summary>
    public partial class ExamProductionView
    {
        #region Members
        private BackgroundWorker _mWorker;
        private readonly ObservableCollection<TestInfoParam> _mObservableCollectionTestInfo;
        private readonly ObservableCollection<TestUserParam> _mObservableCollectionTestUser;
        private readonly List<TestInfoParam> _mlstTestInformations;
        private TestInfoParam _mTestInfoTemp;
        private readonly List<TestUserParam> _mlstTestUserParam;
        private static bool _mbPaperTable = false;
        private static bool _mbExamineeInfoTable = false;
        private List<PanelItem> _mListPanels;

        private int _mPageIndex;//页的索引,这个是从0开始算的
        private int _mPageCount;
        private int _mPageSize;
        private int _mQuestionNum;
        private int _mMaxInfos;
        private string _mPageNum;
        private bool _mbListViewEnable;
        private TestInfoParamEx _mTestInfoParamEx;
        #endregion

        public ExamProductionView()
        {
            InitializeComponent();
            _mObservableCollectionTestInfo = new ObservableCollection<TestInfoParam>();
            _mObservableCollectionTestUser = new ObservableCollection<TestUserParam>();
            _mlstTestInformations = new List<TestInfoParam>();
            _mlstTestUserParam = new List<TestUserParam>();
            _mListPanels = new List<PanelItem>();
            _mPageIndex = 0;
            _mPageCount = 0;
            _mPageSize = 200;
            _mQuestionNum = 0;
            _mMaxInfos = 100000;
            //Loaded += PapersListInfo_Loaded;
        }

        void PapersListInfoLoaded()
        {
            _mObservableCollectionTestInfo.Clear();
            try
            {
                SetBusy(true, CurrentApp.GetMessageLanguageInfo("001", "Loading data, please wait..."));
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
                    TestInfoListTable.ItemsSource = _mObservableCollectionTestInfo;
                    TestUserListTable.ItemsSource = _mObservableCollectionTestUser;
                    _mPageNum = null;
                    _mlstTestInformations.Clear();
                    _mQuestionNum = 0;
                    _mPageIndex = 0;
                    _mPageCount = 0;
                    _mbListViewEnable = true;
                    GetAllTestInfos();
                    _mWorker.Dispose();
                    SetBusy(false, CurrentApp.GetMessageLanguageInfo("001", "Ready"));
                };
                _mWorker.RunWorkerAsync(); //触发DoWork事件
            }
            catch (Exception)
            {
                SetBusy(false, CurrentApp.GetMessageLanguageInfo("001", "Ready"));
            }
        }

        #region 初始化 & 全局消息
        protected override void Init()
        {
            try
            {
                PageName = "ExaminationManagement"; 
                StylePath = "UMPS3603/MainPageStyle.xaml";
                base.Init();

                InitPanels();
                CreateToolBarButtons();
                CurrentApp.SendLoadedMessage();
                TxtPage.KeyUp += TxtPage_KeyUp;
                CreatePageButtons();
                ChangeLanguage();
                PapersListInfoLoaded();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitButton()
        {
            BasicSetBut.Children.Clear();
            Button btn;
            OperationInfo opt;
            btn = new Button();
            btn.Click += AddTestInfo_Click;
            opt = new OperationInfo();
            opt.Display = CurrentApp.GetLanguageInfo("3603T00006", "Add Test Information");
            opt.Icon = "Images/add.png";
            btn.DataContext = opt;
            btn.SetResourceReference(StyleProperty, "OptButtonStyle");
            BasicSetBut.Children.Add(btn);

            btn = new Button();
            btn.Click += SearchTestInfo_Click;
            opt = new OperationInfo();
            opt.Display = CurrentApp.GetLanguageInfo("3603T00078", "Search Test Information");
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
            opt.Display = CurrentApp.GetLanguageInfo("3603T00007", "Set Test User");
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
            opt.Display = CurrentApp.GetLanguageInfo("3603T00008", "Delete Test");
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
                int[] colwidths = { 150, 150, 150, 100, 150, 180, 80, 100, 100, 180, 100 };
                var columnGridView = new GridView();

                for (int i = 0; i < cols.Length; i++)
                {
                    var gvc = new GridViewColumn
                    {
                        Header = CurrentApp.GetLanguageInfo(lans[i], cols[i]),
                        Width = colwidths[i],
                        DisplayMemberBinding = new Binding(cols[i])
                    };
                    columnGridView.Columns.Add(gvc);
                }
                TestInfoListTable.View = columnGridView;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        void InitUserListTable()
        {
            try
            {
                string[] lans = "3603T00018,3603T00019,3603T00020,3603T00021,3603T00022,3603T00023,3603T00024,3603T00025,3603T00026".Split(',');
                string[] cols = "LongTestNum,LongTestUserNum,StrTestUserName,LongPaperNum,StrPaperName,IntScore,StrStartTime,StrEndTime,StrTestStatue".Split(',');
                int[] colwidths = { 150, 150, 150, 100, 60, 180, 80, 100, 100, 150, 100 };
                var columnGridView = new GridView();

                for (int i = 0; i < cols.Length; i++)
                {
                    var gvc = new GridViewColumn
                    {
                        Header = CurrentApp.GetLanguageInfo(lans[i], cols[i]),
                        Width = colwidths[i],
                        DisplayMemberBinding = new Binding(cols[i])
                    };
                    columnGridView.Columns.Add(gvc);
                }
                TestUserListTable.View = columnGridView;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
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
                panelItem.Title = CurrentApp.GetLanguageInfo("3603T00005", "Examinee");
                panelItem.Icon = "Images/00005.png";
                panelItem.IsVisible = true;
                panelItem.CanClose = true;
                _mListPanels.Add(panelItem);

                panelItem = new PanelItem();
                panelItem.PanelId = S3603Consts.PANEL_ID_PAPER;
                panelItem.Name = S3603Consts.PANEL_NAME_PAPER;
                panelItem.ContentId = S3603Consts.PANEL_CONTENTID_PAPER;
                panelItem.Title = CurrentApp.GetLanguageInfo("3603T00036", "Paper");
                panelItem.Icon = "Images/00006.png";
                panelItem.IsVisible = true;
                panelItem.CanClose = true;
                _mListPanels.Add(panelItem);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
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
                ShowException(ex.Message);
            }
        }

        private void CreatePageButtons()
        {
            try
            {
                List<CToolButtonItem> listBtns = new List<CToolButtonItem>();
                CToolButtonItem item = new CToolButtonItem();
                item.Name = "TB" + "FirstPage";
                item.Display = CurrentApp.GetLanguageInfo("3603T00081", "First Page");
                item.Tip = CurrentApp.GetLanguageInfo("3603T00081", "First Page");
                item.Icon = "Images/first.ico";
                listBtns.Add(item);
                item = new CToolButtonItem();
                item.Name = "TB" + "PrePage";
                item.Display = CurrentApp.GetLanguageInfo("3603T00082", "Pre Page");
                item.Tip = CurrentApp.GetLanguageInfo("3603T00082", "Pre Page");
                item.Icon = "Images/pre.ico";
                listBtns.Add(item);
                item = new CToolButtonItem();
                item.Name = "TB" + "NextPage";
                item.Display = CurrentApp.GetLanguageInfo("3603T00083", "Next Page");
                item.Tip = CurrentApp.GetLanguageInfo("3603T00083", "Next Page");
                item.Icon = "Images/next.ico";
                listBtns.Add(item);
                item = new CToolButtonItem();
                item.Name = "TB" + "LastPage";
                item.Display = CurrentApp.GetLanguageInfo("3603T00084", "Last Page");
                item.Tip = CurrentApp.GetLanguageInfo("3603T00084", "Last Page");
                item.Icon = "Images/last.ico";
                listBtns.Add(item);

                PanelPageButtons.Children.Clear();
                for (int i = 0; i < listBtns.Count; i++)
                {
                    CToolButtonItem toolBtn = listBtns[i];
                    Button btn = new Button();
                    btn.DataContext = toolBtn;
                    btn.SetResourceReference(StyleProperty, "ToolBarButtonStyle");
                    btn.Click += PageButton_Click;
                    PanelPageButtons.Children.Add(btn);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion

        #region 样式&语言
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
                    //S3603App.ShowExceptionMessage("1" + ex.Message);
                }
            }
            if (!bPage)
            {
                //如果通过Url没有加载成功，就从已经编译到程序集的默认资源加载
                try
                {
                    string uri = string.Format("/UMPS3603;component/Themes/{0}/{1}",
                        "Default"
                        , StylePath);
                    ResourceDictionary resource = new ResourceDictionary();
                    resource.Source = new Uri(uri, UriKind.RelativeOrAbsolute);
                    Resources.MergedDictionaries.Add(resource);
                }
                catch (Exception)
                {
                    //S3603App.ShowExceptionMessage("2" + ex.Message);
                }
            }

            try
            {
                string uri = "/UMPS3603;component/Themes/Default/UMPS3603/AvalonDock.xaml";
                ResourceDictionary resource = new ResourceDictionary();
                resource.Source = new Uri(uri, UriKind.RelativeOrAbsolute);
                Resources.MergedDictionaries.Add(resource);
            }
            catch (Exception)
            {
                //S3602App.ShowExceptionMessage("2" + ex.Message);
            }
        }

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();

            InitButton();
            InitTestListTable();
            InitUserListTable();
            InitLanguage();
            PageName = "ExaminationManagement";
            CurrentApp.AppTitle = CurrentApp.GetLanguageInfo("3603T00003", "Examination Management");
            if (PopupCreateTest != null)
            { PopupCreateTest.ChangeLanguage(); }
            if (PopupPaperInfo != null)
            { PopupPaperInfo.ChangeLanguage(); }
            if (PopupTestUser != null)
            { PopupTestUser.ChangeLanguage(); }
        }

        private void InitLanguage()
        {
            BsExpandStyleOpt.Header = CurrentApp.GetLanguageInfo("3603T00001", "Basic Setting");
            DeleteExpandStyleOpt.Header = CurrentApp.GetLanguageInfo("3603T00002", "Delete Setting");
            TestInformationTable.Title = CurrentApp.GetLanguageInfo("3603T00004", "Test Information");
            ExamineeInformationTable.Title = CurrentApp.GetLanguageInfo("3603T00005", "Examinee Information");
            PanelObjectPaper.Title = CurrentApp.GetLanguageInfo("3603T00036", "Paper");
        }

        #endregion

        #region Click
        void TxtPage_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)//如果按了下enter按钮,那么就执行下面的代码
                {
                    int pageIndex;
                    if (!int.TryParse(TxtPage.Text, out pageIndex) || _mPageCount == 0)
                    {
                        TxtPage.Text = (_mPageIndex + 1).ToString();
                        return;
                    }
                    pageIndex--;
                    if (pageIndex < 0)
                    {
                        pageIndex = 0;
                    }
                    if (pageIndex > _mPageCount - 1)
                    {
                        pageIndex = _mPageCount - 1;
                    }
                    _mPageIndex = pageIndex;
                    FillListView();
                    SetPageState();
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        void PageButton_Click(object sender, RoutedEventArgs e)//选择看第几页的按钮
        {
            Button btn = e.Source as Button;
            if (btn != null)
            {
                var item = btn.DataContext as CToolButtonItem;
                if (item == null) { return; }
                switch (item.Name)
                {
                    case "TB" + "FirstPage":
                        if (_mPageIndex > 0)
                        {
                            _mPageIndex = 0;
                            FillListView();
                            SetPageState();
                        }
                        break;
                    case "TB" + "PrePage":
                        if (_mPageIndex > 0)
                        {
                            _mPageIndex--;
                            FillListView();
                            SetPageState();
                        }
                        break;
                    case "TB" + "NextPage":
                        if (_mPageIndex < _mPageCount - 1)
                        {
                            _mPageIndex++;
                            FillListView();
                            SetPageState();
                        }
                        break;
                    case "TB" + "LastPage":
                        if (_mPageIndex < _mPageCount - 1)
                        {
                            _mPageIndex = _mPageCount - 1;
                            FillListView();
                            SetPageState();
                        }
                        break;
                }
            }
        }

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
            catch
            {
                // ignored
            }
        }

        private void AddTestInfo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                S3603App.GQueryModify = false;
                PaperInfo newPage = new PaperInfo();
                newPage.ParentPage = this;
                newPage.CurrentApp = CurrentApp;
                PopupCreateTest.Content = newPage;
                PopupCreateTest.Title = CurrentApp.GetLanguageInfo("3603T00055", "Paper Information");
                PopupCreateTest.IsOpen = true;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void SearchTestInfo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                S3603App.GQueryModify = false;
                SearchTestInfoPage newPage = new SearchTestInfoPage();
                newPage.ParentPage = this;
                newPage.CurrentApp = CurrentApp;
                PopupCreateTest.Content = newPage;
                PopupCreateTest.Title = CurrentApp.GetLanguageInfo("3603T00078", "Search Test Info");
                PopupCreateTest.IsOpen = true;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void AddTestUser_Click(object sender, RoutedEventArgs e)
        {
            TestInfoParam item = _mTestInfoTemp;
            if (item != null)
            {
                try
                {
                    UserManagement newpage = new UserManagement();
                    newpage.PageParent = this;
                    newpage.MTestInfoParam = item;
                    newpage.CurrentApp = CurrentApp;
                    PopupTestUser.Content = newpage;
                    PopupTestUser.Title = CurrentApp.GetLanguageInfo("3603T00007", "ScoreSheet User Management");
                    PopupTestUser.IsOpen = true;
                }
                catch (Exception ex)
                {
                    ShowException(ex.Message);
                }
            }
        }

        private void DeleteTestInfo_Click(object sender, RoutedEventArgs e)
        {
            int iCount = TestInfoListTable.SelectedItems.Count;
            if (iCount == 0)
            {
                return;
            }

            MessageBoxResult result1 =
                        MessageBox.Show(
                            CurrentApp.GetLanguageInfo("3603T00066",
                                "Sure you want to delete?"),
                            CurrentApp.GetLanguageInfo("3603T00090", "Warning"),
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
                    MessageBox.Show(
                        CurrentApp.GetLanguageInfo("3603T00094",
                            "The examination paper is already in use cannot be deleted!"),
                        CurrentApp.GetLanguageInfo("3603T00090", "Warning"),
                        MessageBoxButton.OKCancel);
                    return;
                }

                if (DeleteTest(item))
                {
                    ShowInformation(CurrentApp.GetLanguageInfo("3603T00054", "Success!"));
                    _mlstTestInformations.Remove(_mlstTestInformations.FirstOrDefault(p => p.LongTestNum == item.LongTestNum));
                    SetObservableCollectionTestInfo();
                    int total = _mQuestionNum - 1;
                    if (total > _mMaxInfos)
                    {
                        ShowInformation(CurrentApp.GetLanguageInfo("3601T00096", "Larger than allowed max Questions, some Questions can\'t be displayed"));
                        return;
                    }
                    _mQuestionNum = total;
                    SetPageState();
                    _mObservableCollectionTestUser.Clear();
                }
            }
        }

        private void TestInformationTable_IsSelectedChanged(object sender, EventArgs e)
        {

        }

        private void PanelObjectPaper_IsSelectedChanged(object sender, EventArgs e)
        {
            try
            {
                if (!_mbPaperTable)
                {
                    var item = TestInfoListTable.SelectedItem as TestInfoParam;
                    if (item != null)
                    {
                        S3603App.GQueryModify = false;
                        S3603App.GTestInformation = new TestInfoParam();
                        S3603App.GTestInformation = item;
                        S3603App.GPaperParam = new CPaperParam();
                        CPaperParam paperParam;
                        if (GetPaperInfo(SqlSearchPaper(item), out paperParam))
                        {
                            S3603App.GPaperParam = paperParam;
                            TestPaperPage newPage = new TestPaperPage();
                            TestPaperPageName.Children.Clear();//把之前的页面清空防止重复加载
                            TestPaperPageName.Children.Add(newPage);
                            newPage.CurrentApp = CurrentApp;
                        }

                    }
                    _mbPaperTable = true;
                    _mbExamineeInfoTable = false;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
            
        }

        void TestInformationTable_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                ExamineeInformationTable.IsSelected = true;
                _mTestInfoTemp = null;
                var item = TestInfoListTable.SelectedItem as TestInfoParam;
                _mTestInfoTemp = item;
                if (item != null)
                {
                    RefreshTestUserInfo();
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
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
            try
            {
                S3603App.GQueryModify = false;
                AddTestInfo newPage = new AddTestInfo();
                newPage.ParentPage = this;
                newPage.CurrentApp = CurrentApp;
                PopupCreateTest.Content = newPage;
                PopupCreateTest.Title = CurrentApp.GetLanguageInfo("3603T00055", "Paper Information");
                PopupCreateTest.IsOpen = true;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void GetTestUserInfo(TestInfoParam testInfoTemp)
        {
            _mlstTestUserParam.Clear();
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3603Codes.OptGetTestUserInfo;
                Service36031Client client = new Service36031Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                                          WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service36031"));
                //var client = new Service36031Client();
                OperationReturn optReturn = XMLHelper.SeriallizeObject(testInfoTemp);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                webRequest.ListData.Add(optReturn.Data.ToString());
                WebReturn webReturn = client.UmpTaskOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                CurrentApp.WriteLog("GetTestUserInfo", webReturn.Data);

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
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        continue;
                    }
                    var testInfo = optReturn.Data as TestUserParam;
                    if (testInfo == null)
                    {
                        ShowException("Fail. filesItem is null");
                        return;
                    }

                    _mlstTestUserParam.Add(testInfo);
                }
                SetObservableCollectionTestUser();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private string SetSql()
        {
            string strSql = string.Empty;
            switch (CurrentApp.Session.DBType)
            {
                case 2:
                    strSql =
                        string.Format(
                            "SELECT TOP 200 X.* FROM T_36_025_{0} X ORDER BY X.C001 DESC", CurrentApp.Session.RentInfo.Token);
                    break;
                case 3:
                    strSql =
                       string.Format(
                           "SELECT * FROM (SELECT X.*  FROM T_36_025_{0} X  ORDER BY X.C001 DESC) WHERE ROWNUM <= 200 ", CurrentApp.Session.RentInfo.Token);
                    break;
            }
            return strSql;
        }

        private void GetAllTestInfos()
        {
            try
            {
                string strSql = SetSql();
                var webRequest = new WebRequest();
                Service36031Client client = new Service36031Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service36031"));
                //var client = new Service36031Client();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3603Codes.OptGetTestInfo;
                OperationReturn optReturn = XMLHelper.SeriallizeObject(strSql);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                webRequest.ListData.Add(optReturn.Data.ToString());
                WebReturn webReturn = client.UmpTaskOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("{0}: ,{1}",
                        CurrentApp.GetLanguageInfo("3603T00065", "Search Failed"), webReturn.Message));
                    return;
                }

                if (webReturn.ListData.Count <= 0)
                {
                    if (_mbListViewEnable)
                    {
                        SetPageState();
                        SetObservableCollectionTestInfo();
                        _mbListViewEnable = false;
                    }
                    return;
                }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    optReturn = XMLHelper.DeserializeObject<TestInfoParam>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        continue;
                    }
                    var testInfo = optReturn.Data as TestInfoParam;
                    if (testInfo == null)
                    {
                        ShowException("Fail. filesItem is null");
                        return;
                    }

                    _mPageNum = testInfo.LongTestNum.ToString();
                    int total = _mQuestionNum + 1;
                    if (total > _mMaxInfos)
                    {
                        ShowInformation(CurrentApp.GetLanguageInfo("3601T00096", "Larger than allowed max Questions, some Questions can\'t be displayed"));
                        return;
                    }
                    _mQuestionNum = total;
                    _mlstTestInformations.Add(testInfo);
                    SetPageState();  
                }
                if (_mbListViewEnable)
                {
                    SetObservableCollectionTestInfo();
                    _mbListViewEnable = false;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private string SetSearchSql()
        {
            string strTemp = null;
            string strSql = string.Empty;
            switch (CurrentApp.Session.DBType)
            {
                case 2:
                    if (string.IsNullOrEmpty(_mPageNum))
                    {
                        strSql = string.Format("SELECT TOP {1} X.* FROM T_36_025_{0} X where ",
                            CurrentApp.Session.RentInfo.Token, _mPageSize);
                    }
                    else
                    {
                        strSql = string.Format("SELECT TOP {1} X.* FROM T_36_025_{0} X where X.c001>'{2}' and ",
                            CurrentApp.Session.RentInfo.Token, _mPageSize, _mPageNum);
                    }
                    if (!string.IsNullOrWhiteSpace(_mTestInfoParamEx.StrPaperName))
                    {
                        strTemp = string.Format(" X.c003 like '%{0}%' ", _mTestInfoParamEx.StrPaperName);
                        strSql += strTemp;
                    }
                    break;
                case 3:
                    if (string.IsNullOrEmpty(_mPageNum))
                    {
                        strSql =
                            string.Format(
                                "SELECT * FROM (SELECT X.*  FROM T_36_025_{0} X  where ",
                                CurrentApp.Session.RentInfo.Token);
                    }
                    else
                    {
                        strSql =
                            string.Format(
                                "SELECT * FROM (SELECT X.*  FROM T_36_025_{0} X  where  X.c001>'{1}' and ",
                                CurrentApp.Session.RentInfo.Token, _mPageNum);
                    }

                    if (!string.IsNullOrWhiteSpace(_mTestInfoParamEx.StrPaperName))
                    {
                        strTemp = string.Format(" X.c003 like '%{0}%' ", _mTestInfoParamEx.StrPaperName);
                        strSql += strTemp;
                    }
                    break;
            }

            if (_mTestInfoParamEx.LongPaperNum > 0)
            {
                if (strTemp == null)
                {
                    strTemp = string.Format(" X.c002 = '{0}' ", _mTestInfoParamEx.LongPaperNum);
                }
                else
                {
                    strTemp = string.Format(" and X.c002 = '{0}' ", _mTestInfoParamEx.LongPaperNum);
                }
                strSql += strTemp;
            }

            if (_mTestInfoParamEx.LongEditorId > 0 )
            {
                if (strTemp == null)
                {
                    strTemp = string.Format(" X.c006 = '{0}' ", _mTestInfoParamEx.LongEditorId);
                }
                else
                {
                    strTemp = string.Format(" and X.c006 = '{0}' ", _mTestInfoParamEx.LongEditorId);
                }
                strSql += strTemp;
            }

            if (!string.IsNullOrEmpty(_mTestInfoParamEx.StrEditor))
            {
                if (strTemp == null)
                {
                    strTemp = string.Format(" X.c007 = '{0}' ", _mTestInfoParamEx.StrEditor);
                }
                else
                {
                    strTemp = string.Format(" and X.c007 = '{0}' ", _mTestInfoParamEx.StrEditor);
                }
                strSql += strTemp;
            }

            if (!string.IsNullOrEmpty(_mTestInfoParamEx.StrTestStatue))
            {
                if (strTemp == null)
                    strTemp = string.Format(" X.c009 = '{0}' ", _mTestInfoParamEx.StrTestStatue);
                else
                    strTemp = string.Format(" and X.c009 = '{0}' ", _mTestInfoParamEx.StrTestStatue);
                strSql += strTemp;
            }

            if (!string.IsNullOrEmpty(_mTestInfoParamEx.StrTestTime))
            {
                switch (CurrentApp.Session.DBType)
                {
                    case 2:
                        if (strTemp == null)
                            strTemp = string.Format(" X.c005 = '{0}' ", _mTestInfoParamEx.StrTestTime);
                        else
                            strTemp = string.Format(" and X.c005 = '{0}' ", _mTestInfoParamEx.StrTestTime);
                        break;
                    case 3:
                        if (strTemp == null)
                            strTemp = string.Format(" X.c005 = to_date('{0}','yyyy-mm-dd hh24:mi:ss') ", _mTestInfoParamEx.StrTestTime);
                        else
                            strTemp = string.Format(" and X.c005 = to_date('{0}','yyyy-mm-dd hh24:mi:ss') ", _mTestInfoParamEx.StrTestTime);
                        break;
                }

                
                strSql += strTemp;
            }

            switch (CurrentApp.Session.DBType)
            {
                case 2:
                    if (strTemp == null)
                        strTemp = string.Format(" X.c008 >= '{0}' and X.c008 <= '{1}' ORDER BY X.C001", _mTestInfoParamEx.StrStartTime, _mTestInfoParamEx.StrEndTime);
                    else
                        strTemp = string.Format(" and X.c008 >= '{0}' and X.c008 <= '{1}' ORDER BY X.C001", _mTestInfoParamEx.StrStartTime, _mTestInfoParamEx.StrEndTime);
                    break;
                case 3:
                    if (strTemp == null)
                        strTemp = string.Format(" X.c008 between to_date('{0}','yyyy-mm-dd hh24:mi:ss')  and to_date('{1}','yyyy-mm-dd hh24:mi:ss') ORDER BY X.C001 ) WHERE ROWNUM <= {2}", _mTestInfoParamEx.StrStartTime, _mTestInfoParamEx.StrEndTime, _mPageSize);
                    else
                        strTemp = string.Format(" and X.c008 between to_date('{0}','yyyy-mm-dd hh24:mi:ss')  and to_date('{1}','yyyy-mm-dd hh24:mi:ss') ORDER BY X.C001 ) WHERE ROWNUM <= {2}", _mTestInfoParamEx.StrStartTime, _mTestInfoParamEx.StrEndTime, _mPageSize);
                    break;
            }
            return strSql + strTemp;
        }

        private void SearchTestInfo(string strSql)
        {
            string strLog;
            try
            {
                var webRequest = new WebRequest();
                Service36031Client client = new Service36031Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service36031"));
                //var client = new Service36031Client();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3603Codes.OptGetTestInfo;
                OperationReturn optReturn = XMLHelper.SeriallizeObject(strSql);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                webRequest.ListData.Add(optReturn.Data.ToString());
                WebReturn webReturn = client.UmpTaskOperation(webRequest);
                client.Close();
                CurrentApp.WriteLog(CurrentApp.GetLanguageInfo("3603T00078", "Search"));
                if (!webReturn.Result)
                {
                    #region 写操作日志
                    strLog = string.Format("{0} {1} : {2}", Utils.FormatOptLogString("3603T00078"),
                        Utils.FormatOptLogString("3603T00089"), webReturn.Message);
                    CurrentApp.WriteOperationLog(S3603Consts.OPT_Search.ToString(), ConstValue.OPT_RESULT_FAIL, strLog);

                    #endregion

                    CurrentApp.WriteLog(webReturn.Message);

                    ShowException(string.Format("{0}: ,{1}",
                        CurrentApp.GetLanguageInfo("3603T00065", "Search Failed"), webReturn.Message));
                    return;
                }

                #region 写操作日志

                strLog = string.Format("{0} {1}", Utils.FormatOptLogString("3603T00078"),
                    Utils.FormatOptLogString("3603T00090"));
                CurrentApp.WriteOperationLog(S3603Consts.OPT_Search.ToString(), ConstValue.OPT_RESULT_SUCCESS, strLog);

                #endregion

                CurrentApp.WriteLog(CurrentApp.GetLanguageInfo("3603T00090", "Search Success"));

                if (webReturn.ListData.Count <= 0)
                {
                    if (_mbListViewEnable)
                    {
                        SetPageState();
                        SetObservableCollectionTestInfo();
                        _mbListViewEnable = false;
                    }
                    return;
                }
                foreach (string strData in webReturn.ListData)
                {
                    optReturn = XMLHelper.DeserializeObject<TestInfoParam>(strData);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        continue;
                    }
                    var testInfo = optReturn.Data as TestInfoParam;
                    if (testInfo == null)
                    {
                        ShowException("Fail. filesItem is null");
                        return;
                    }

                    _mPageNum = testInfo.LongTestNum.ToString();
                    int total = _mQuestionNum + 1;
                    if (total > _mMaxInfos)
                    {
                        ShowInformation(CurrentApp.GetLanguageInfo("3601T00145", "Larger than allowed max Questions, some Questions can\'t be displayed"));
                        return;
                    }
                    _mQuestionNum = total;
                    _mlstTestInformations.Add(testInfo);
                    SetPageState();
                }
                if (_mbListViewEnable)
                {
                    SetObservableCollectionTestInfo();
                    _mbListViewEnable = false;
                }
            }
            catch (Exception ex)
            {
                #region 写操作日志

                strLog = string.Format("{0} {1} : {2}", Utils.FormatOptLogString("3603T00078"),
                    Utils.FormatOptLogString("3603T00089"), ex.Message);
                CurrentApp.WriteOperationLog(S3603Consts.OPT_Search.ToString(), ConstValue.OPT_RESULT_FAIL, strLog);

                #endregion

                CurrentApp.WriteLog(ex.Message);
                ShowException(ex.Message);
            }
        }

        private void SearchTestInfos()
        {
            string strLog;
            try
            {
                string strSql = SetSearchSql();
                var webRequest = new WebRequest();
                Service36031Client client = new Service36031Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service36031"));
                //var client = new Service36031Client();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3603Codes.OptGetTestInfo;
                OperationReturn optReturn = XMLHelper.SeriallizeObject(strSql);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                webRequest.ListData.Add(optReturn.Data.ToString());
                WebReturn webReturn = client.UmpTaskOperation(webRequest);
                client.Close();
                CurrentApp.WriteLog(CurrentApp.GetLanguageInfo("3603T00078", "Search"));
                if (!webReturn.Result)
                {
                    #region 写操作日志

                    strLog = string.Format("{0} {1} : {2}", Utils.FormatOptLogString("3603T00078"),
                        Utils.FormatOptLogString("3603T00089"), webReturn.Message);
                    CurrentApp.WriteOperationLog(S3603Consts.OPT_Search.ToString(), ConstValue.OPT_RESULT_FAIL, strLog);

                    #endregion

                    CurrentApp.WriteLog(webReturn.Message);

                    ShowException(string.Format("{0}: ,{1}",
                        CurrentApp.GetLanguageInfo("3603T00065", "Search Failed"), webReturn.Message));
                    return;
                }

                #region 写操作日志

                strLog = string.Format("{0} {1}", Utils.FormatOptLogString("3603T00078"),
                    Utils.FormatOptLogString("3603T00090"));
                CurrentApp.WriteOperationLog(S3603Consts.OPT_Search.ToString(), ConstValue.OPT_RESULT_SUCCESS, strLog);

                #endregion

                CurrentApp.WriteLog(CurrentApp.GetLanguageInfo("3603T00090", "Search Success"));

                if (webReturn.ListData.Count <= 0)
                {
                    if (_mbListViewEnable)
                    {
                        SetPageState();
                        SetObservableCollectionTestInfo();
                        _mbListViewEnable = false;
                    }
                    return;
                }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    optReturn = XMLHelper.DeserializeObject<TestInfoParam>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        continue;
                    }
                    var testInfo = optReturn.Data as TestInfoParam;
                    if (testInfo == null)
                    {
                        ShowException("Fail. filesItem is null");
                        return;
                    }

                    _mPageNum = testInfo.LongTestNum.ToString();
                    int total = _mQuestionNum + 1;
                    if (total > _mMaxInfos)
                    {
                        ShowInformation(CurrentApp.GetLanguageInfo("3601T00145", "Larger than allowed max Questions, some Questions can\'t be displayed"));
                        return;
                    }
                    _mQuestionNum = total;
                    _mlstTestInformations.Add(testInfo);
                    SetPageState();
                }
                if (_mbListViewEnable)
                {
                    SetObservableCollectionTestInfo();
                    _mbListViewEnable = false;
                }
                SearchTestInfos();
            }
            catch (Exception ex)
            {
                #region 写操作日志

                strLog = string.Format("{0} {1} : {2}", Utils.FormatOptLogString("3603T00078"),
                    Utils.FormatOptLogString("3603T00089"), ex.Message);
                CurrentApp.WriteOperationLog(S3603Consts.OPT_Search.ToString(), ConstValue.OPT_RESULT_FAIL, strLog);

                #endregion

                CurrentApp.WriteLog(ex.Message);
                ShowException(ex.Message);
            }
        }

        private void SetObservableCollectionTestInfo()
        {
            _mObservableCollectionTestInfo.Clear();
            foreach (var param in _mlstTestInformations)
            {
                param.StrTestStatue = param.StrTestStatue == "Y"
                    ? CurrentApp.GetLanguageInfo("3603T00086", "Y")
                    : CurrentApp.GetLanguageInfo("3603T00087", "N");

                _mObservableCollectionTestInfo.Add(param);
            }
        }

        private void SetObservableCollectionTestUser()
        {
            _mObservableCollectionTestUser.Clear();
            foreach (var param in _mlstTestUserParam)
            {
                param.StrTestStatue = param.StrTestStatue == "Y"
                    ? CurrentApp.GetLanguageInfo("3603T00086", "Y")
                    : CurrentApp.GetLanguageInfo("3603T00087", "N");
                _mObservableCollectionTestUser.Add(param);
            }
        }

        public void UpdateTestInfoTable()
        {
            if (S3603App.GTestInformation != null)
            {
                _mlstTestInformations.Insert(0, S3603App.GTestInformation);
                SetObservableCollectionTestInfo();
                int total = _mQuestionNum + 1;
                if (total > _mMaxInfos)
                {
                    ShowInformation(CurrentApp.GetLanguageInfo("3601T00096", "Larger than allowed max Questions, some Questions can\'t be displayed"));
                    return;
                }
                _mQuestionNum = total;
                SetPageState();
            }     
        }

        public void SearchTestInfo(TestInfoParamEx testInfoParamEx)
        {
            _mTestInfoParamEx = new TestInfoParamEx();
            _mTestInfoParamEx = testInfoParamEx;
            _mPageNum = null;
            _mlstTestInformations.Clear();
            _mQuestionNum = 0;
            _mPageIndex = 0;
            _mPageCount = 0;
            _mbListViewEnable = true;
            if (_mTestInfoParamEx.LongTestNum != 0)
            {
                string strSql = string.Format("SELECT * FROM T_36_025_{0} where c001='{1}'",
                    CurrentApp.Session.RentInfo.Token, _mTestInfoParamEx.LongTestNum);
                SearchTestInfo(strSql);
            }
            else
            {
                SearchTestInfos();
            }
        }

        private bool DeleteTest(TestInfoParam testInfoParam)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3603Codes.OptDeleteTestInfo;
                Service36031Client client = new Service36031Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                     WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service36031"));
                //var client = new Service36031Client();
                OperationReturn optReturn = XMLHelper.SeriallizeObject(testInfoParam);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return false;
                }
                webRequest.ListData.Add(optReturn.Data.ToString());
                //Service36031Client client = new Service36031Client();
                WebReturn webReturn = client.UmpTaskOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(CurrentApp.GetLanguageInfo("3603T00083", "Delete Failed"));
                    return false;
                }
                if (webReturn.Message == S3603Consts.HadUse)// 该查询条件被使用无法删除
                {
                    ShowInformation(CurrentApp.GetLanguageInfo("3603T00011", "Can't Delete"));
                    return false;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
                return false;
            }
            return true;
        }

        private string SqlSearchPaper( TestInfoParam testInfoParam)
        {
            var sql = string.Format("SELECT * FROM T_36_023_{0} WHERE c001='{1}'", CurrentApp.Session.RentInfo.Token,
                testInfoParam.LongPaperNum);
            return sql;
        }

        private bool GetPaperInfo(string strSql,out CPaperParam paperParam)
        {
            paperParam = new CPaperParam();
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3603Codes.OptSearchPapers;
                OperationReturn optReturn = XMLHelper.SeriallizeObject(strSql);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return false;
                }
                webRequest.ListData.Add(optReturn.Data.ToString());
                Service36031Client client = new Service36031Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service36031"));
                //var client = new Service36031Client();
                WebReturn webReturn = client.UmpTaskOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("{0}: {1}",
                        CurrentApp.GetLanguageInfo("3603T00065", "Search Failed"), webReturn.Message));
                    return false;
                }

                if (webReturn.ListData.Count <= 0) { return true; }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    optReturn = XMLHelper.DeserializeObject<CPaperParam>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        continue;
                    }
                    var paperInfo = optReturn.Data as CPaperParam;
                    if (paperInfo == null)
                    {
                        ShowException("Fail. filesItem is null");
                        return false;
                    }

                    paperParam = paperInfo;
                }
                
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
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
                                panel.Title = CurrentApp.GetLanguageInfo("3603T00005", item.Name);
                                break;
                            case 2:
                                panel.Title = CurrentApp.GetLanguageInfo("3603T00036", item.Name);
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
                ShowException(ex.Message);
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

        private void SetPageState()//设置的每页的状态
        {
            try
            {
                int pageCount = _mQuestionNum / _mPageSize;
                int mod = _mQuestionNum % _mPageSize;
                if (mod > 0)
                {
                    pageCount++;
                }
                _mPageCount = pageCount;
                string strPageInfo = string.Format("{0}/{1} {2} {3}", _mPageIndex + 1, _mPageCount, _mQuestionNum,
                    CurrentApp.GetLanguageInfo("3603T00080", "Sum"));
                Dispatcher.Invoke(new Action(() =>
                {
                    TxtPageInfo.Text = strPageInfo;
                    TxtPage.Text = (_mPageIndex + 1).ToString();
                }));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void FillListView()
        {
            try
            {
                _mObservableCollectionTestInfo.Clear();
                int intStart = _mPageIndex * _mPageSize;
                int intEnd = (_mPageIndex + 1) * _mPageSize;
                for (int i = intStart; i < intEnd && i < _mQuestionNum; i++)
                {
                    TestInfoParam testInfoParam = _mlstTestInformations[i];

                    

                    _mObservableCollectionTestInfo.Add(testInfoParam);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        public void RefreshTestUserInfo()
        {
            InitTestInfoBut();
            GetTestUserInfo(_mTestInfoTemp);
            InitDeleteBut(); 
        }

        #endregion
    }
}
