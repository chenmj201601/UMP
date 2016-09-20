using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Common3602;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;
using System.Collections.ObjectModel;
using UMPS3602.Models;
using UMPS3602.Wcf36021;

namespace UMPS3602
{
    /// <summary>
    /// ExamProductionPage.xaml 的交互逻辑
    /// </summary>
    public partial class ExamProductionView
    {
        #region Members
        private BackgroundWorker _mWorker;
        private readonly ObservableCollection<CPaperParam> _mObservableCollectionPaperParams;
        private readonly List<CPaperParam> _mListPaperInfos;
        private readonly List<CPaperParam> _mListPaperParamsOld;
        private string _mUmpFileRootPath;
        private PaperInfo _mAddPaperInfo;
        private readonly List<CPaperParam> _mListPaperParams;
        private CPaperParam _mPaperParamTemp;

        private int _mPageIndex;//页的索引,这个是从0开始算的
        private int _mPageCount;
        private int _mPageSize;
        private int _mQuestionNum;
        private int _mMaxInfos;
        private string _mPageNum;
        private bool _mbListViewEnable;
        private CSearchPaperParam _mSearchPaperParam;
        #endregion

        public ExamProductionView()
        {
            InitializeComponent();
            _mObservableCollectionPaperParams = new ObservableCollection<CPaperParam>();
            _mListPaperInfos = new List<CPaperParam>();
            _mUmpFileRootPath = null;
            _mAddPaperInfo = new PaperInfo();
            _mListPaperParams = new List<CPaperParam>();
            _mPaperParamTemp = new CPaperParam();
            _mListPaperParamsOld = new List<CPaperParam>();
            _mSearchPaperParam = new CSearchPaperParam();
            _mPageIndex = 0;
            _mPageCount = 0;
            _mPageSize = 200;
            _mQuestionNum = 0;
            _mMaxInfos = 100000;
            //Loaded += PapersListInfo_Loaded;
        }

        void PapersListInfeLoaded()
        {
            _mObservableCollectionPaperParams.Clear();
            try
            {
                SetBusy(true, CurrentApp.GetMessageLanguageInfo("001", "Loading data, please wait..."));
                _mWorker = new BackgroundWorker();
                _mWorker.WorkerReportsProgress = true;
                _mWorker.WorkerSupportsCancellation = true;
                //注册线程主体方法
                _mWorker.DoWork += (s, de) =>
                {
                    GetUmpSetupPath();
                    //InitCategoryTreeInfo();
                };
                _mWorker.RunWorkerCompleted += (s, re) =>
                {
                    ChangeLanguage();
                    PapersListTable.ItemsSource = _mObservableCollectionPaperParams;
                    _mPageNum = null;
                    _mListPaperInfos.Clear();
                    _mListPaperParamsOld.Clear();
                    _mQuestionNum = 0;
                    _mPageIndex = 0;
                    _mPageCount = 0;
                    _mbListViewEnable = true;
                    GetAllPaperInfos();
                    _mWorker.Dispose();
                    SetBusy(false,"...");
                };
                _mWorker.RunWorkerAsync(); //触发DoWork事件
            }
            catch (Exception)
            {
                SetBusy(false, "...");
            }
        }

        #region 初始化 & 全局消息
        protected override void Init()
        {
            try
            {
                PageName = "Paper";
                StylePath = "UMPS3602/MainPageStyle.xaml";
                base.Init();

                CurrentApp.SendLoadedMessage();
                TxtPage.KeyUp += TxtPage_KeyUp;
                CreatePageButtons();
                ChangeLanguage();
                PapersListInfeLoaded();
            }
            catch (Exception ex)
            {
               ShowException(ex.Message);
            }
        }

        private void InitBasicSetingBut()
        {
            BasicSetBut.Children.Clear();
            Button btn;
            OperationInfo opt;
            btn = new Button();
            btn.Click += AddPapers_Click;
            opt = new OperationInfo();
            opt.Display = CurrentApp.GetLanguageInfo("3602T00006", "Add Paper");
            opt.Icon = "Images/add.png";
            btn.DataContext = opt;
            btn.SetResourceReference(StyleProperty, "OptButtonStyle");
            BasicSetBut.Children.Add(btn);

            btn = new Button();
            btn.Click += SearchPapers_Click;
            opt = new OperationInfo();
            opt.Display = CurrentApp.GetLanguageInfo("3602T00001", "Search Paper");
            opt.Icon = "Images/add.png";
            btn.DataContext = opt;
            btn.SetResourceReference(StyleProperty, "OptButtonStyle");
            BasicSetBut.Children.Add(btn);
        }

        private void CancelSearchBut()
        {
            CancelSpeechBut.Children.Clear();
            Button btn;
            OperationInfo opt;
            btn = new Button();
            btn.Click += CancelSpeech_Click;
            opt = new OperationInfo();
            opt.Display = CurrentApp.GetLanguageInfo("3602T00129", "Cancel Speech");
            opt.Icon = "Images/add.png";
            btn.DataContext = opt;
            btn.SetResourceReference(StyleProperty, "OptButtonStyle");
            CancelSpeechBut.Children.Add(btn);
        }

        private void CreatePageButtons()
        {
            try
            {
                List<CToolButtonItem> listBtns = new List<CToolButtonItem>();
                CToolButtonItem item = new CToolButtonItem();
                item.Name = "TB" + "FirstPage";
                item.Display = CurrentApp.GetLanguageInfo("3602T00149", "First Page");
                item.Tip = CurrentApp.GetLanguageInfo("3602T00149", "First Page");
                item.Icon = "Images/first.ico";
                listBtns.Add(item);
                item = new CToolButtonItem();
                item.Name = "TB" + "PrePage";
                item.Display = CurrentApp.GetLanguageInfo("3602T00150", "Pre Page");
                item.Tip = CurrentApp.GetLanguageInfo("3602T00150", "Pre Page");
                item.Icon = "Images/pre.ico";
                listBtns.Add(item);
                item = new CToolButtonItem();
                item.Name = "TB" + "NextPage";
                item.Display = CurrentApp.GetLanguageInfo("3602T00151", "Next Page");
                item.Tip = CurrentApp.GetLanguageInfo("3602T00151", "Next Page");
                item.Icon = "Images/next.ico";
                listBtns.Add(item);
                item = new CToolButtonItem();
                item.Name = "TB" + "LastPage";
                item.Display = CurrentApp.GetLanguageInfo("3602T00152", "Last Page");
                item.Tip = CurrentApp.GetLanguageInfo("3602T00152", "Last Page");
                item.Icon = "Images/last.ico";
                listBtns.Add(item);

                PanelPageButtons.Children.Clear();
                for (int i = 0; i < listBtns.Count; i++)
                {
                    CToolButtonItem toolBtn = listBtns[i];
                    var btn = new Button {DataContext = toolBtn};
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
                    //S3602App.ShowExceptionMessage("1" + ex.Message);
                }
            }
            if (!bPage)
            {
                //如果通过Url没有加载成功，就从已经编译到程序集的默认资源加载
                try
                {
                    string uri = string.Format("/UMPS3602;component/Themes/{0}/{1}",
                        "Default"
                        , StylePath);
                    ResourceDictionary resource = new ResourceDictionary();
                    resource.Source = new Uri(uri, UriKind.RelativeOrAbsolute);
                    Resources.MergedDictionaries.Add(resource);
                }
                catch (Exception)
                {
                    //S3602App.ShowExceptionMessage("2" + ex.Message);
                }
            }
            try
            {
                string uri = "/UMPS3602;component/Themes/Default/UMPS3602/AvalonDock.xaml";
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
            InitPaperListTable();
            InitBasicSetingBut();  
            PageName = "Paper";
            CurrentApp.AppTitle = CurrentApp.GetLanguageInfo("3602T00135", "Paper");

            BSExpandStyleOpt.Header = CurrentApp.GetLanguageInfo("3602T00002", "Basic Setting");
            DeleteExpandStyleOpt.Header = CurrentApp.GetLanguageInfo("3602T00114", "Delete");
            PapersTable.Title = CurrentApp.GetLanguageInfo("3602T00003", "Papers");
        }
        #endregion

        #region Init Information
        void InitPaperListTable()
        {
            try
            {
                string[] lans = "3602T00037,3602T00038,3602T00039,3602T00040,3602T00136,3602T00042,3602T00043,3602T00044,3602T00045,3602T00046,3602T00047,3602T00048".Split(',');
                string[] cols = "LongNum, StrName, StrDescribe, StrType, StrIntegrity, IntQuestionNum, IntScores, IntPassMark, IntTestTime, StrEditor, StrDateTime, StrUsed".Split(',');
                int[] colwidths = { 150, 150, 150, 100, 50, 60, 80, 100, 100, 100, 150, 100 };
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
                PapersListTable.View = columnGridView;
            }
            catch (Exception ex)
            {
               ShowException(ex.Message);
            }
        }
        #endregion

        #region Click
        void PageButton_Click(object sender, RoutedEventArgs e)//选择看第几页的按钮
        {
            var btn = e.Source as Button;
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

        private void AddPapers_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                S3602App.GQueryModify = false;
                S3602App.GPaperInfo = new PaperInfo();
                S3602App.GPaperInfo.OptType = S3602Codes.OptAddPaper;
                AddPaperPage newPage = new AddPaperPage();
                newPage.ParentExamPage = this;
                newPage.CurrentApp = CurrentApp;
                PopupAddPaper.Content = newPage;
                PopupAddPaper.Title = CurrentApp.GetLanguageInfo("3602T00006", "Add Paper");
                PopupAddPaper.IsOpen = true;
            }
            catch (Exception ex)
            {
               ShowException(ex.Message);
            }
        }

        private void SearchPapers_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                S3602App.GQueryModify = false;
                SearchPaperPage newPage = new SearchPaperPage();
                newPage.ParentPage = this;
                newPage.CurrentApp = CurrentApp;
                PopupSearchPaper.Content = newPage;
                PopupSearchPaper.Title = CurrentApp.GetLanguageInfo("3602T00001", "Search Paper");
                PopupSearchPaper.IsOpen = true;
            }
            catch (Exception ex)
            {
               ShowException(ex.Message);
            }
        }

        private void CancelSpeech_Click(object sender, RoutedEventArgs e)
        {
            _mPageNum = null;
            _mListPaperInfos.Clear();
            _mQuestionNum = 0;
            _mPageIndex = 0;
            _mPageCount = 0;
            _mbListViewEnable = true;
            foreach (var param in _mListPaperParamsOld)
            {
                int total = _mQuestionNum + 1;
                if (total > _mMaxInfos)
                {
                    ShowInformation(CurrentApp.GetLanguageInfo("3602T00147", "Larger than allowed max Questions, some Questions can\'t be displayed"));
                    return;
                }
                _mQuestionNum = total;
                _mListPaperInfos.Add(param);
                SetPageState();
            }
            CancelSpeechBut.Children.Clear();
            SetObservableCollection();
        }

        private void PapersTable_IsSelectedChanged(object sender, EventArgs e)
        {

        }

        void PapersListTable_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (Keyboard.IsKeyDown(Key.RightCtrl) ||
                    Keyboard.IsKeyDown(Key.RightShift) ||
                    Keyboard.IsKeyDown(Key.LeftCtrl) ||
                    Keyboard.IsKeyDown(Key.LeftShift))
                {
                    return;
                }
                _mListPaperParams.Clear();

                _mPaperParamTemp = new CPaperParam();
                S3602App.GPaperInfo = new PaperInfo();
                var item = PapersListTable.SelectedItem as CPaperParam;
                _mPaperParamTemp = item;
                _mListPaperParams.Add(_mPaperParamTemp);
                S3602App.GPaperInfo.PaperParam = _mPaperParamTemp;
                if (item != null)
                {
                    BasicSetBut.Children.Clear();
                    DeleteSetBut.Children.Clear();
                    Button btn;
                    OperationInfo opt;
                    btn = new Button();
                    btn.Click += AddPapers_Click;
                    opt = new OperationInfo();
                    opt.Display = CurrentApp.GetLanguageInfo("3602T00006", "Add Paper");
                    opt.Icon = "Images/add.png";
                    btn.DataContext = opt;
                    btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                    BasicSetBut.Children.Add(btn);

                    btn = new Button();
                    btn.Click += SearchPapers_Click;
                    opt = new OperationInfo();
                    opt.Display = CurrentApp.GetLanguageInfo("3602T00001", "Search Paper");
                    opt.Icon = "Images/add.png";
                    btn.DataContext = opt;
                    btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                    BasicSetBut.Children.Add(btn);

                    btn = new Button();
                    btn.Click += EditPapers_Click;
                    opt = new OperationInfo();
                    opt.Display = CurrentApp.GetLanguageInfo("3602T00007", "Edit Paper");
                    opt.Icon = "Images/add.png";
                    btn.DataContext = opt;
                    btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                    BasicSetBut.Children.Add(btn);

                    

                    btn = new Button();
                    btn.Click += ExaminePaper_Click;
                    opt = new OperationInfo();
                    opt.Display = CurrentApp.GetLanguageInfo("3602T00010", "Check Paper");
                    opt.Icon = "Images/Delete.png";
                    btn.DataContext = opt;
                    btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                    BasicSetBut.Children.Add(btn);

                    btn = new Button();
                    btn.Click += DeletePaper_Click;
                    opt = new OperationInfo();
                    opt.Display = CurrentApp.GetLanguageInfo("3602T00009", "Delete Paper");
                    opt.Icon = "Images/Delete.png";
                    btn.DataContext = opt;
                    btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                    DeleteSetBut.Children.Add(btn);

                    
                }
            }
            catch (Exception ex)
            {
               ShowException(ex.Message);
            }
        
        }
        
        private void ExaminePaper_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                S3602App.GQueryModify = false;
                var item = PapersListTable.SelectedItem as CPaperParam;
                if (item != null)
                {
                    S3602App.GPaperInfo.PaperParam = item;
                    TestPaperPage newPage = new TestPaperPage();
                    newPage.ExamProductionParentPage = this;
                    newPage.CurrentApp = CurrentApp;
                    PopupPaperInfo.Content = newPage;
                    PopupPaperInfo.Title = CurrentApp.GetLanguageInfo("3602T00003", "Paper");
                    PopupPaperInfo.IsOpen = true;
                }
            }
            catch (Exception ex)
            {
               ShowException(ex.Message);
            }
        }

        private void EditPapers_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var item = PapersListTable.SelectedItem as CPaperParam;
                if (item != null)
                {
                    S3602App.GPaperInfo = new PaperInfo();
                    S3602App.GPaperInfo.PaperParam = _mPaperParamTemp;
                    S3602App.GPaperInfo.OptType = S3602Codes.OptEditPaper;
                    CreateEditPaperPage();
                }
            }
            catch (Exception ex)
            {
               ShowException( ex.Message);
            }

            
        }

       
        private void DeletePaper_Click(object sender, RoutedEventArgs e)
        {
            var item = PapersListTable.SelectedItem as CPaperParam;
            if (item == null)
                return;

            if (_mListPaperParams.Count <= 0)
                return;

            MessageBoxResult result = MessageBox.Show(CurrentApp.GetLanguageInfo("3602T00094", "Is sure to delete the examination paper, test paper deleted and delete item!"),
                        CurrentApp.GetLanguageInfo("3602T00090", "Warning"),
                        MessageBoxButton.OKCancel);
            if (result != MessageBoxResult.OK)
                return;

            foreach (var paperParam in _mListPaperParams)
            {
                if (paperParam.IntUsed == 1)
                {
                   ShowException(string.Format("{0} : {1}", CurrentApp.GetLanguageInfo("3602T00092", "The examination paper is already in use cannot be deleted!"), paperParam.StrName));
                    return;
                }
            }

            if (DeletePaper())
            {
                ShowInformation(CurrentApp.GetLanguageInfo("3602T00012", "Success!"));
                foreach (var paperParam in _mListPaperParams)
                {
                    _mListPaperInfos.Remove(_mListPaperInfos.FirstOrDefault(p => p.LongNum == paperParam.LongNum));
                    int total = _mQuestionNum - 1;
                    if (total > _mMaxInfos)
                    {
                        ShowInformation(CurrentApp.GetLanguageInfo("3602T00147", "Larger than allowed max Questions, some Questions can\'t be displayed"));
                    }
                    _mQuestionNum = total;
                    SetPageState();
                }
                DeletePaperAllQuestions();
                SetObservableCollection();
            }
            _mListPaperParams.Clear();
            _mPaperParamTemp = new CPaperParam();
            S3602App.GPaperInfo = new PaperInfo();
        }

       #endregion

        #region Operations
        private string SetSql()
        {
            string strSql = string.Empty;
            switch (CurrentApp.Session.DBType)
            {
                case 2:
                    strSql =
                        string.Format(
                            "SELECT TOP 200 X.* FROM T_36_023_{0} X  ORDER BY X.C001 DESC ",
                            CurrentApp.Session.RentInfo.Token);
                    break;
                case 3:
                    strSql =
                       string.Format(
                           "SELECT * FROM (SELECT X.*  FROM T_36_023_{0} X  ORDER BY X.C001 DESC) WHERE ROWNUM <= 200 ",
                           CurrentApp.Session.RentInfo.Token);
                    break;
            }
            return strSql;
        }

        private void GetAllPaperInfos()
        {
            try
            {
                string strSql = SetSql();
                var webRequest = new WebRequest();
                Service36021Client client = new Service36021Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service36021"));
                //var client = new Service36021Client();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3602Codes.OptGetPapers;
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
                   ShowException(string.Format("{0} {1} ", CurrentApp.GetLanguageInfo("3602T00076", "Query the database to fail!"), webReturn.Message));
                    return;
                }

                if (webReturn.ListData.Count <= 0) { return; }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    optReturn = XMLHelper.DeserializeObject<CPaperParam>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                       ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        continue;
                    }
                    var cExamPapers = optReturn.Data as CPaperParam;
                    if (cExamPapers == null)
                    {
                       ShowException("Fail. filesItem is null");
                        return;
                    }
                    int total = _mQuestionNum + 1;
                    if (total > _mMaxInfos)
                    {
                        ShowInformation(CurrentApp.GetLanguageInfo("3602T00147", "Larger than allowed max Questions, some Questions can\'t be displayed"));
                        return;
                    }
                    _mQuestionNum = total;
                    _mPageNum = cExamPapers.LongNum.ToString();
                    _mListPaperInfos.Add(cExamPapers);
                    _mListPaperParamsOld.Add(cExamPapers);
                    SetPageState();
                }
                SetObservableCollection();
                _mbListViewEnable = false;
            }
            catch (Exception ex)
            {
               ShowException(ex.Message);
            }
        }

        private string SetSearchSql()
        {
            string strTemp = null;
            int iMax = 0;
            int iMin = 0;
            string strSql = string.Empty;

            switch (CurrentApp.Session.DBType)
            {
                case 2:
                    if (string.IsNullOrEmpty(_mPageNum))
                    {
                        strSql = string.Format("SELECT TOP {1} X.* FROM T_36_023_{0} X where ",
                            CurrentApp.Session.RentInfo.Token, _mPageSize);
                    }
                    else
                    {
                        strSql = string.Format("SELECT TOP {1} X.* FROM T_36_023_{0} X where X.c001>'{2}' and ",
                            CurrentApp.Session.RentInfo.Token, _mPageSize, _mPageNum);
                    }
                    if (!string.IsNullOrWhiteSpace(_mSearchPaperParam.StrName))
                    {
                        strTemp = string.Format(" X.c002 like '%{0}%' ", _mSearchPaperParam.StrName);
                        strSql += strTemp;
                    }
                    break;
                case 3:
                    if (string.IsNullOrEmpty(_mPageNum))
                    {
                        strSql =
                            string.Format(
                                "SELECT * FROM (SELECT X.*  FROM T_36_023_{0} X  where ",
                                CurrentApp.Session.RentInfo.Token);
                    }
                    else
                    {
                        strSql =
                            string.Format(
                                "SELECT * FROM (SELECT X.*  FROM T_36_023_{0} X  where  X.c001>'{1}' and ",
                                CurrentApp.Session.RentInfo.Token, _mPageNum);
                    }
                    
                    if (!string.IsNullOrWhiteSpace(_mSearchPaperParam.StrName))
                    {
                        strTemp = string.Format(" X.c002 like '%{0}%' ", _mSearchPaperParam.StrName);
                        strSql += strTemp;
                    }
                    break;
            }

            iMin = _mSearchPaperParam.IntScoresMin;
            iMax = _mSearchPaperParam.IntScoresMax;
            if (iMin != 0 || iMax != 0)
            {
                if (iMin > iMax)
                {
                   ShowException(string.Format("{0}",
                        CurrentApp.GetLanguageInfo("3602T00131", "Score range is not correct!")));
                    return null;
                }
                else
                {
                    if (strTemp == null)
                    {
                        strTemp = string.Format(" X.c008 >= {0} AND X.c008 <= {1} ", iMin, iMax);
                    }
                    else
                    {
                        strTemp = string.Format(" and X.c008 >= {0} AND X.c008 <= {1} ", iMin, iMax);
                    }
                    strSql += strTemp;
                }
            }
            iMin = _mSearchPaperParam.IntPassMarkMin;
            iMax = _mSearchPaperParam.IntPassMarkMax;
            if (iMin != 0 || iMax != 0)
            {
                if (iMin > iMax)
                {
                   ShowException(string.Format("{0}",
                        CurrentApp.GetLanguageInfo("3602T00132", "PassMark range is not correct!")));
                    return null;
                }
                else
                {
                    if (strTemp == null)
                    {
                        strTemp = string.Format(" X.c009 >= {0} AND X.c009 <= {1} ", iMin, iMax);
                    }
                    else
                    {
                        strTemp = string.Format(" and X.c009 >= {0} AND X.c009 <= {1} ", iMin, iMax);
                    }
                    strSql += strTemp;
                }
            }

            iMin = _mSearchPaperParam.IntTimeMin;
            iMax = _mSearchPaperParam.IntTimeMax;
            if (iMin != 0 || iMax != 0)
            {
                if (iMin > iMax)
                {
                   ShowException(string.Format("{0}",
                        CurrentApp.GetLanguageInfo("3602T00133", "TimeMark range is not correct!")));
                    return null;
                }
                else
                {
                    if (strTemp == null)
                    {
                        strTemp = string.Format(" X.c010 >= {0} AND X.c010 <= {1} ", iMin, iMax);
                    }
                    else
                    {
                        strTemp = string.Format(" and X.c010 >= {0} AND X.c010 <= {1} ", iMin, iMax);
                    }
                    strSql += strTemp;
                }
            }

            if (_mSearchPaperParam.IntUsed != -1)
            {
                if (strTemp == null)
                    strTemp = string.Format(" X.c014 = '{0}' ", _mSearchPaperParam.IntUsed);
                else
                    strTemp = string.Format(" and X.c014 = '{0}' ", _mSearchPaperParam.IntUsed);
                strSql += strTemp;
            }
            
            switch (CurrentApp.Session.DBType)
            {
                case 2:
                    if (strTemp == null)
                        strTemp = string.Format(" X.c013 >= '{0}' and X.c013 <= '{1}' ORDER BY X.C001", _mSearchPaperParam.StrStartTime, _mSearchPaperParam.StrEndTime);
                    else
                        strTemp = string.Format(" and X.c013 >= '{0}' and X.c013 <= '{1}' ORDER BY X.C001", _mSearchPaperParam.StrStartTime, _mSearchPaperParam.StrEndTime);
                    break;
                case 3:
                    if (strTemp == null)
                        strTemp = string.Format(" X.c013 between to_date('{0}','yyyy-mm-dd hh24:mi:ss')  and to_date('{1}','yyyy-mm-dd hh24:mi:ss') ORDER BY X.C001 ) WHERE ROWNUM <= {2}", _mSearchPaperParam.StrStartTime, _mSearchPaperParam.StrEndTime, _mPageSize);
                    else
                        strTemp = string.Format(" and X.c013 between to_date('{0}','yyyy-mm-dd hh24:mi:ss')  and to_date('{1}','yyyy-mm-dd hh24:mi:ss') ORDER BY X.C001 ) WHERE ROWNUM <= {2}", _mSearchPaperParam.StrStartTime, _mSearchPaperParam.StrEndTime, _mPageSize);
                    break;
            }
           return strSql + strTemp;
        }

        private void SearchPaperInfos()
        {
            string strLog;
            try
            {
                string strSql = SetSearchSql();
                var webRequest = new WebRequest();
                Service36021Client client = new Service36021Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service36021"));
                //var client = new Service36021Client();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int) S3602Codes.OptGetPapers;
                OperationReturn optReturn = XMLHelper.SeriallizeObject(strSql);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                webRequest.ListData.Add(optReturn.Data.ToString());
                WebReturn webReturn = client.UmpTaskOperation(webRequest);
                client.Close();
                CurrentApp.WriteLog(CurrentApp.GetLanguageInfo("3602T00121", "Query"));
                if (!webReturn.Result)
                {
                    #region 写操作日志

                    strLog = string.Format("{0} {1} : {2}", Utils.FormatOptLogString("3602T00121"),
                        Utils.FormatOptLogString("3602T00168"), webReturn.Message);
                    CurrentApp.WriteOperationLog(S3602Consts.OPT_Add.ToString(), ConstValue.OPT_RESULT_FAIL, strLog);
                    CurrentApp.WriteLog(webReturn.Message);

                    #endregion


                    ShowException(string.Format("{0} {1} ",
                        CurrentApp.GetLanguageInfo("3602T00076", "Query the database to fail!"), webReturn.Message));
                    return;
                }

                #region 写操作日志

                strLog = string.Format("{0} {1}", Utils.FormatOptLogString("3602T00121"),
                    Utils.FormatOptLogString("3602T00167"));
                CurrentApp.WriteOperationLog(S3602Consts.OPT_Add.ToString(), ConstValue.OPT_RESULT_SUCCESS, strLog);
                CurrentApp.WriteLog(CurrentApp.GetLanguageInfo("3602T00167", "Query Success"));

                #endregion

                if (webReturn.ListData.Count <= 0)
                {
                    if (_mbListViewEnable)
                    {
                        SetObservableCollection();
                        _mbListViewEnable = false;
                    }
                    return;
                }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    optReturn = XMLHelper.DeserializeObject<CPaperParam>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        continue;
                    }
                    var cExamPapers = optReturn.Data as CPaperParam;
                    if (cExamPapers == null)
                    {
                        ShowException("Fail. filesItem is null");
                        return;
                    }
                    int total = _mQuestionNum + 1;
                    if (total > _mMaxInfos)
                    {
                        ShowInformation(CurrentApp.GetLanguageInfo("3602T00147",
                            "Larger than allowed max Questions, some Questions can\'t be displayed"));
                        return;
                    }
                    _mQuestionNum = total;
                    _mPageNum = cExamPapers.LongNum.ToString();
                    _mListPaperInfos.Add(cExamPapers);
                    SetPageState();
                }
                if (_mbListViewEnable)
                {
                    SetObservableCollection();
                    _mbListViewEnable = false;
                }
                SearchPaperInfos();
            }
            catch (Exception ex)
            {
                #region 写操作日志

                strLog = string.Format("{0} {1} : {2}", Utils.FormatOptLogString("3602T00121"),
                    Utils.FormatOptLogString("3602T00168"), ex.Message);
                CurrentApp.WriteOperationLog(S3602Consts.OPT_Add.ToString(), ConstValue.OPT_RESULT_FAIL, strLog);

                #endregion

                CurrentApp.WriteLog(ex.Message);
                ShowException(ex.Message);
            }
        }

        private void CreateEditPaperPage()
        {
            try
            {
                S3602App.IsModifyScoreSheet = false;
                EditPaperView newView = new EditPaperView();
                newView.PageName = "EditPaper";
                newView.CurrentApp = CurrentApp;
                if (CurrentApp.RunAsModule)
                {
                    CurrentApp.CurrentView = newView;
                    CurrentApp.InitCurrentView();
                }
                else
                {
                    var app = App.Current;
                    if (app != null)
                    {
                        var window = app.MainWindow;
                        if (window != null)
                        {
                            var shell = window.Content as Shell;
                            if (shell != null)
                            {
                                shell.SetView(newView);
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

        //获取UMP安装目录
        private void GetUmpSetupPath()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3602Codes.OptGetUmpsetuppath;
                Service36021Client client = new Service36021Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service36021"));
                //Service36021Client client = new Service36021Client();
                WebReturn webReturn = client.UmpTaskOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                   ShowException(webReturn.Message);
                    return;
                }
                if (string.IsNullOrWhiteSpace(webReturn.Data)) { return; }
                _mUmpFileRootPath = webReturn.Data;
                CurrentApp.WriteLog("UMPFolderPath", _mUmpFileRootPath);
            }
            catch (Exception ex)
            {
               ShowException(ex.Message);
            }
        }

        public void AddPaperList()
        {
            if (S3602App.GPaperInfo.OptType == S3602Codes.OptAddPaper)
            {
                _mAddPaperInfo = S3602App.GPaperInfo;
                _mListPaperInfos.Insert(0, _mAddPaperInfo.PaperParam);
            }
            else
            {
                _mListPaperInfos.Remove(
                    _mListPaperInfos.FirstOrDefault(p => p.LongNum == S3602App.GPaperInfo.PaperParam.LongNum));
                _mListPaperInfos.Insert(0, S3602App.GPaperInfo.PaperParam);
            }
            SetObservableCollection();
        }

        public void OpenEditPaperPage()
        {
            CreateEditPaperPage();
        }

        private bool DeletePaper()
        {
            string strLog;
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int) S3602Codes.OptDeletePaper;
                Service36021Client client = new Service36021Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service36021"));
                OperationReturn optReturn = XMLHelper.SeriallizeObject(_mListPaperParams);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return false;
                }
                webRequest.ListData.Add(optReturn.Data.ToString());
                //Service36021Client client = new Service36021Client();
                WebReturn webReturn = client.UmpTaskOperation(webRequest);
                client.Close();
                CurrentApp.WriteLog(CurrentApp.GetLanguageInfo("3602T00009", "Delete"));
                if (!webReturn.Result)
                {
                    #region 写操作日志

                    strLog = string.Format("{0} {1} : {2}", Utils.FormatOptLogString("3602T00009"),
                        Utils.FormatOptLogString("3602T00166"), webReturn.Message);
                    CurrentApp.WriteOperationLog(S3602Consts.OPT_Delete.ToString(), ConstValue.OPT_RESULT_FAIL, strLog);

                    #endregion

                    CurrentApp.WriteLog(webReturn.Message);

                    ShowException(string.Format("{0}{1}", CurrentApp.GetLanguageInfo("3602T00083", "Delete Failed"),
                        webReturn.Message));
                    return false;
                }
                if (webReturn.Message == S3602Consts.HadUse) // 该查询条件被使用无法删除
                {
                    #region 写操作日志

                    strLog = string.Format("{0} {1} : {2}", Utils.FormatOptLogString("3602T00009"),
                        Utils.FormatOptLogString("3602T00166"), CurrentApp.GetLanguageInfo("3602T00011", "Can't Delete"));
                    CurrentApp.WriteOperationLog(S3602Consts.OPT_Delete.ToString(), ConstValue.OPT_RESULT_FAIL, strLog);

                    #endregion

                    CurrentApp.WriteLog(webReturn.Message);
                    ShowInformation(CurrentApp.GetLanguageInfo("3602T00011", "Can't Delete"));
                    return false;
                }

                CurrentApp.WriteLog(CurrentApp.GetLanguageInfo("3602T00165", "Delete Success"));
            }
            catch (Exception ex)
            {
                #region 写操作日志

                strLog = string.Format("{0} {1} : {2}", Utils.FormatOptLogString("3602T00009"),
                    Utils.FormatOptLogString("3602T00166"), ex.Message);
                CurrentApp.WriteOperationLog(S3602Consts.OPT_Delete.ToString(), ConstValue.OPT_RESULT_FAIL, strLog);

                #endregion

                CurrentApp.WriteLog(ex.Message);
                ShowException(ex.Message);
                return false;
            }
            return true;
        }

        private bool DeletePaperAllQuestions()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3602Codes.OptDeletePaperAllQuestions;
                Service36021Client client = new Service36021Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service36021"));
                OperationReturn optReturn = XMLHelper.SeriallizeObject(_mListPaperParams);
                if (!optReturn.Result)
                {
                   ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return false;
                }
                webRequest.ListData.Add(optReturn.Data.ToString());
                //Service36021Client client = new Service36021Client();
                WebReturn webReturn = client.UmpTaskOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                   ShowException(string.Format("{0}", CurrentApp.GetLanguageInfo("3602T00093", "Try to delete failed in the examination paper!")));
                    return false;
                }
                if (webReturn.Message == S3602Consts.HadUse)// 该查询条件被使用无法删除
                {
                    ShowInformation(CurrentApp.GetLanguageInfo("3602T00011", "Can't Delete"));
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

        public void SearchPaper(CSearchPaperParam searchPaperParam)
        {
            _mSearchPaperParam = new CSearchPaperParam();
            _mSearchPaperParam = searchPaperParam;
            _mPageNum = null;
            _mListPaperInfos.Clear();
            _mQuestionNum = 0;
            _mPageIndex = 0;
            _mPageCount = 0;
            _mbListViewEnable = true;
            SearchPaperInfos();
            CancelSearchBut();
            
        }

        private void SetObservableCollection()
        {
            _mObservableCollectionPaperParams.Clear();
            foreach (var param in _mListPaperInfos)
            {
                param.StrIntegrity = param.IntIntegrity == 1 ? CurrentApp.GetLanguageInfo("3602T00140", "Y") : CurrentApp.GetLanguageInfo("3602T00141", "N");

                switch (param.CharType)
                {
                    case '1':
                        param.StrType = CurrentApp.GetLanguageInfo("3602T00070", "1");
                        break;
                    case '2':
                        param.StrType = CurrentApp.GetLanguageInfo("3602T00071", "2");
                        break;
                    case '3':
                        param.StrType = CurrentApp.GetLanguageInfo("3602T00072", "3");
                        break;
                    case '4':
                        param.StrType = CurrentApp.GetLanguageInfo("3602T00073", "4");
                        break;
                    case '5':
                        param.StrType = CurrentApp.GetLanguageInfo("3602T00074", "5");
                        break;
                }

                param.StrUsed = param.IntUsed == 1 ? CurrentApp.GetLanguageInfo("3602T00064", "Y") : CurrentApp.GetLanguageInfo("3602T00065", "N");

                _mObservableCollectionPaperParams.Add(param);
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
                    CurrentApp.GetLanguageInfo("3602T00148", "Sum"));
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
                _mObservableCollectionPaperParams.Clear();
                int intStart = _mPageIndex * _mPageSize;
                int intEnd = (_mPageIndex + 1) * _mPageSize;
                for (int i = intStart; i < intEnd && i < _mQuestionNum; i++)
                {
                    CPaperParam paperParam = _mListPaperInfos[i];


                    paperParam.StrIntegrity = paperParam.IntIntegrity == 1 ? CurrentApp.GetLanguageInfo("3602T00140", "Y") : CurrentApp.GetLanguageInfo("3602T00141", "N");

                    switch (paperParam.CharType)
                    {
                        case '1':
                            paperParam.StrType = CurrentApp.GetLanguageInfo("3602T00070", "1");
                            break;
                        case '2':
                            paperParam.StrType = CurrentApp.GetLanguageInfo("3602T00071", "2");
                            break;
                        case '3':
                            paperParam.StrType = CurrentApp.GetLanguageInfo("3602T00072", "3");
                            break;
                        case '4':
                            paperParam.StrType = CurrentApp.GetLanguageInfo("3602T00073", "4");
                            break;
                        case '5':
                            paperParam.StrType = CurrentApp.GetLanguageInfo("3602T00074", "5");
                            break;
                    }
                    paperParam.StrUsed = paperParam.IntUsed == 1
                        ? CurrentApp.GetLanguageInfo("3602T00064", "Y")
                        : CurrentApp.GetLanguageInfo("3602T00065", "N");

                    _mObservableCollectionPaperParams.Add(paperParam);
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
