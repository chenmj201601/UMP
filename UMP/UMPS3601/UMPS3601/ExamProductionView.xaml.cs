using Common3601;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using UMPS3601.Models;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;
using System.Collections.ObjectModel;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using UMPS3601.Wcf36011;
using VoiceCyber.Wpf.AvalonDock.Layout;


namespace UMPS3601
{
    /// <summary>
    /// ExamProduction.xaml 的交互逻辑
    /// </summary>
    public partial class ExamProductionView
    {
        #region Members
        private BackgroundWorker _mWorker;
        private List<CCategoryTree> _mListCategoryTreeParam;
        private CCategoryTree _mCategoryNode;
        private CCategoryTree _mCategoryNodeTemp;
        private List<PanelItem> _mListPanels;
        private ObservableCollection<CQuestionsParam> _mListExamQuestions;
        private ObservableCollection<CPaperParam> _mListAddPapers;
        private List<CQuestionsParam> _mlistQuestionsParams;
        private CQuestionsParam _mCExamQuestionsTemp;
        private PaperInfo _mAddPaperInfo;
        private string _mUmpFileRootPath;
        private bool _mBTableAddQuestions = true;
        private bool _mBTableBrowsePapers = false;
        private List<CQuestionsParam> _mListQuestionInfosTemp;
        private List<long> _mlstCategoryNum = new List<long>();
        private CSearchQuestionParam _mSearchQuestionParam;
        private List<CCategoryTree> _mlstSearchCategoryNode;

        private int _mPageIndex;//页的索引,这个是从0开始算的
        private int _mPageCount;
        private int _mPageSize;
        private int _mQuestionNum;
        private int _mMaxInfos;
        private string _mPageNum;
        private bool _mbListViewEnable;
        #endregion

        public ExamProductionView()
        {
            InitializeComponent();
            _mPageIndex = 0;
            _mPageCount = 0;
            _mPageSize = 200;
            _mQuestionNum = 0;
            _mMaxInfos = 100000;
            _mCategoryNode = new CCategoryTree();
            _mListCategoryTreeParam = new List<CCategoryTree>();
            _mListPanels = new List<PanelItem>();
            _mListExamQuestions = new ObservableCollection<CQuestionsParam>();
            _mUmpFileRootPath = null;
            _mCExamQuestionsTemp = new CQuestionsParam();
            _mListQuestionInfosTemp = new List<CQuestionsParam>();
            _mAddPaperInfo = new PaperInfo();
            _mListAddPapers = new ObservableCollection<CPaperParam>();
            _mlistQuestionsParams = new List<CQuestionsParam>();
            _mCategoryNodeTemp = new CCategoryTree();
            _mPageNum = null;
            _mbListViewEnable = true;
            _mSearchQuestionParam = new CSearchQuestionParam();
            _mlstSearchCategoryNode = new List<CCategoryTree>();
            //Loaded += QuestionsListInfo_Loaded;
            CategoryTree.SelectedItemChanged += OrgCategoryTree_SelectedItemChanged;
        }

        void QuestionsListInfoLoaded()
        {
            _mListExamQuestions.Clear();
            try
            {
                SetBusy(true, CurrentApp.GetMessageLanguageInfo("001", string.Format("Loading data, please wait...")));
                _mWorker = new BackgroundWorker();
                _mWorker.WorkerReportsProgress = true;
                _mWorker.WorkerSupportsCancellation = true;
                //注册线程主体方法
                _mWorker.DoWork += (s, de) =>
                {
                    GetUmpSetupPath();
                };
                _mWorker.RunWorkerCompleted += (s, re) =>
                {
                    ChangeLanguage();
                    ATDocument.ItemsSource = _mListExamQuestions;
                    _mWorker.Dispose();
                    SetBusy(false, "...");
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
                PageName = "ExamProduction";
                StylePath = "UMPS3601/MainPageStyle.xaml";
                base.Init();
                string strSql = string.Format("SELECT * FROM T_36_021_{0}", CurrentApp.Session.RentInfo.Token);
                InitCategoryTreeInfo(strSql);
                InitBut();
                InitPanels();
                CreateToolBarButtons();
                SetPanelVisible();
                SetViewStatus();
                CurrentApp.SendLoadedMessage();
                TxtPage.KeyUp += TxtPage_KeyUp;
                CreatePageButtons();
                ChangeLanguage();
                QuestionsListInfoLoaded();
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
                panelItem.PanelId = S3601Consts.PANEL_ID_TREE;
                panelItem.Name = S3601Consts.PANEL_NAME_TREE;
                panelItem.ContentId = S3601Consts.PANEL_CONTENTID_TREE;
                panelItem.Title = CurrentApp.GetLanguageInfo("3601T00034", "Category");
                panelItem.Icon = "Images/00005.png";
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
                item.Display = CurrentApp.GetLanguageInfo("3601T00148", "First Page");
                item.Tip = CurrentApp.GetLanguageInfo("3601T00148", "First Page");
                item.Icon = "Images/first.ico";
                listBtns.Add(item);
                item = new CToolButtonItem();
                item.Name = "TB" + "PrePage";
                item.Display = CurrentApp.GetLanguageInfo("3601T00149", "Pre Page");
                item.Tip = CurrentApp.GetLanguageInfo("3601T00149", "Pre Page");
                item.Icon = "Images/pre.ico";
                listBtns.Add(item);
                item = new CToolButtonItem();
                item.Name = "TB" + "NextPage";
                item.Display = CurrentApp.GetLanguageInfo("3601T00150", "Next Page");
                item.Tip = CurrentApp.GetLanguageInfo("3601T00150", "Next Page");
                item.Icon = "Images/next.ico";
                listBtns.Add(item);
                item = new CToolButtonItem();
                item.Name = "TB" + "LastPage";
                item.Display = CurrentApp.GetLanguageInfo("3601T00151", "Last Page");
                item.Tip = CurrentApp.GetLanguageInfo("3601T00151", "Last Page");
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
                catch (Exception ex)
                {
                    //ShowException("1" + ex.Message);
                }
            }
            if (!bPage)
            {
                //如果通过Url没有加载成功，就从已经编译到程序集的默认资源加载
                try
                {
                    string uri = string.Format("/UMPS3601;component/Themes/{0}/{1}",
                        "Default"
                        , StylePath);
                    ResourceDictionary resource = new ResourceDictionary();
                    resource.Source = new Uri(uri, UriKind.RelativeOrAbsolute);
                    Resources.MergedDictionaries.Add(resource);
                }
                catch (Exception ex)
                {
                    //ShowException("2" + ex.Message);
                }
            }
            try
            {
                string uri = string.Format("/UMPS3601;component/Themes/Default/UMPS3601/AvalonDock.xaml");
                ResourceDictionary resource = new ResourceDictionary();
                resource.Source = new Uri(uri, UriKind.RelativeOrAbsolute);
                Resources.MergedDictionaries.Add(resource);
            }
            catch (Exception ex)
            {
                //ShowException("2" + ex.Message);
            }
        }

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();

            InitQuestionInformationList();
            CurrentApp.AppTitle = CurrentApp.GetLanguageInfo("3601T00135", "Question Bank");
            PageName = "QuestionBank";
            PanelObjectTreeBox.Title = CurrentApp.GetLanguageInfo("3601T00034", "Question Category");
            BSExpandStyleOpt.Header = CurrentApp.GetLanguageInfo("3601T00033", "Basic Settings");
            DeleteExpandStyleOpt.Header = CurrentApp.GetLanguageInfo("3601T00114", "Delete Setting");
            AddQuestionsDocument.Title = CurrentApp.GetLanguageInfo("3601T00035", "Questions");
            CSExpandStyleOpt.Header = CurrentApp.GetLanguageInfo("3601T00152", "Category Settings");

            SetPageState();
        }
        #endregion

        #region Add Test Node Tree

        public CCategoryTree GetCategoryNodeInfo(CCategoryTree parentInfo, CCategoryTree param)
        {
            CCategoryTree temp = new CCategoryTree();
            try
            {
                temp.Icon = "/UMPS3601;component/Themes/Default/UMPS3601/Images/document.ico";
                temp.LongNum = param.LongNum;
                temp.StrName = param.LongParentNodeId == 0 ? CurrentApp.GetLanguageInfo("3601T00017", "Category") : param.StrName;
                temp.LongParentNodeId = param.LongParentNodeId;
                if (_mlstCategoryNum.Count <= 0)
                {
                    if (param.LongParentNodeId == 0) 
                        temp.IsExpanded = true;
                }
                else
                {
                    int iCount = 0;
                    foreach (var num in _mlstCategoryNum)
                    {
                        iCount++;
                        if (param.LongNum == num)
                        {
                            temp.IsExpanded = true;
                            if (iCount == _mlstCategoryNum.Count)
                                temp.IsChecked = true;
                        }
                    }
                    foreach (var categoryTree in _mlstSearchCategoryNode)
                    {
                        if (param.LongNum == categoryTree.LongNum)
                        {
                            temp.ChangeBrush = Brushes.Gold;
                        }
                    }
                }
                temp.StrParentNodeName = param.StrParentNodeName;
                temp.LongFounderId = param.LongFounderId;
                temp.StrFounderName = param.StrFounderName;
                temp.StrDateTime = param.StrDateTime;
                AddChildNode(parentInfo, temp);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
            return temp;
        }

        public CCategoryTree GetAddPapersNodeInfo(CCategoryTree parentInfo, PapersCategoryParam param)
        {
            CCategoryTree temp = new CCategoryTree();
            try
            {
                temp.StrName = param.StrName;
                temp.Icon = "/UMPS3601;component/Themes/Default/UMPS3601/Images/document.ico";
                temp.LongNum = param.LongNum;
                temp.StrName = param.StrName;
                temp.LongParentNodeId = param.LongParentNodeId;
                temp.StrParentNodeName = param.StrParentNodeName;
                temp.LongFounderId = param.LongFounderId;
                temp.StrFounderName = param.StrFounderName;
                temp.StrDateTime = param.StrDateTime;
                AddChildNode(parentInfo, temp);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
            return temp;
        }

        public void InitCategoryTree(List<CCategoryTree> listPapersCategoryParam, long longParentNodeId, CCategoryTree categoryNodes)
        {
            CCategoryTree nodeTemp = new CCategoryTree();
            foreach (CCategoryTree param in listPapersCategoryParam)
            {
                if (param.LongParentNodeId == longParentNodeId)
                {
                    CCategoryTree tempNode = new CCategoryTree();
                    nodeTemp = GetCategoryNodeInfo(categoryNodes, param);
                    InitCategoryTree(listPapersCategoryParam, param.LongNum, nodeTemp);   
                }
            }
        }

        public void InitAddPapersTree(List<PapersCategoryParam> listPapersCategoryParam, long longParentNodeId, CCategoryTree addPaperNodes)
        {
            CCategoryTree nodeTemp = new CCategoryTree();
            foreach (PapersCategoryParam param in listPapersCategoryParam)
            {
                if (param.LongParentNodeId == longParentNodeId)
                {
                    nodeTemp = GetAddPapersNodeInfo(addPaperNodes, param);
                    InitAddPapersTree(listPapersCategoryParam, param.LongNum, nodeTemp);
                }
            }
        }

        private void AddChildNode(CCategoryTree parentItem, CCategoryTree item)
        {
            Dispatcher.Invoke(new Action(() => parentItem.AddChild(item)));
        }

        #endregion

        #region Init Information
        void InitBut()
        {
            BasicSetBut.Children.Clear();
            ButSearchCategroy.Children.Clear();
            Button btn;
            OperationInfo opt;
            btn = new Button();
            btn.Click += AddQuestions_Click;
            opt = new OperationInfo();
            opt.Display = CurrentApp.GetLanguageInfo("3601T00002", "Create Question");
            opt.Icon = "Images/add.png";
            btn.DataContext = opt;
            btn.SetResourceReference(StyleProperty, "OptButtonStyle");
            BasicSetBut.Children.Add(btn);

            btn = new Button();
            btn.Click += SearchQuestions_Click;
            opt = new OperationInfo();
            opt.Display = CurrentApp.GetLanguageInfo("3601T00115", "Search Questions");
            opt.Icon = "Images/add.png";
            btn.DataContext = opt;
            btn.SetResourceReference(StyleProperty, "OptButtonStyle");
            BasicSetBut.Children.Add(btn);

            btn = new Button();
            btn.Click += SearchCategory_Click;
            opt = new OperationInfo();
            opt.Icon = "Images/search.png";
            TbSearchCategroy.Text = CurrentApp.GetLanguageInfo("3601T00154", "Search Category");
            btn.DataContext = opt;
            btn.SetResourceReference(StyleProperty, "OptButtonStyle");
            ButSearchCategroy.Children.Add(btn);

            btn = new Button();
            btn.Click += CreateCategory_Click;
            opt = new OperationInfo();
            opt.Display = CurrentApp.GetLanguageInfo("3601T00013", "Add Create");
            opt.Icon = "Images/add.png";
            btn.DataContext = opt;
            btn.SetResourceReference(StyleProperty, "OptButtonStyle");
            CategorySetBut.Children.Add(btn);

            btn = new Button();
            btn.Click += ChangeCategory_Click;
            opt = new OperationInfo();
            opt.Display = CurrentApp.GetLanguageInfo("3601T00153", "Channge Create");
            opt.Icon = "Images/add.png";
            btn.DataContext = opt;
            btn.SetResourceReference(StyleProperty, "OptButtonStyle");
            CategorySetBut.Children.Add(btn);
            
        }

        void InitQuestionInformationList()
        {
            try
            {
                string[] lans = "3601T00016,3601T00017,3601T00018,3601T00019,3601T00020,3601T00021,3601T00022,3601T00023,3601T00024,3601T00025,3601T00026,3601T00026,3601T00026,3601T00026,3601T00026,3601T00026,3601T00027,3601T00028,3601T00029,3601T00030,3601T00031,3601T00032".Split(',');
                string[] cols = "LongNum,StrCategoryName,StrType,StrQuestionsContect,StrAnswerOne,StrAnswerTwo,StrAnswerThree,StrAnswerFour,StrAnswerFive,StrAnswerSix,StrConOne,CorrectAnswerTwo,  CorrectAnswerThree,CorrectAnswerFour,CorrectAnswerFive, CorrectAnswerSix, IntUseNumber, StrAccessoryType, StrAccessoryName, StrAccessoryPath, StrFounderName,StrDateTime".Split(',');
                int[] colwidths = { 150,150, 60, 180, 80, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100 };
                GridView columnGridView = new GridView();
                GridViewColumn gvc;

                for (int i = 0; i < 22; i++)
                {
                    gvc = new GridViewColumn();
                    gvc.Header = CurrentApp.GetLanguageInfo(lans[i], cols[i]);
                    gvc.Width = colwidths[i];
                    gvc.DisplayMemberBinding = new Binding(cols[i]);
                    columnGridView.Columns.Add(gvc);
                }
                ATDocument.View = columnGridView;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        public void InitCategoryTreeInfo(string strSql)
        {
            try
            {
                _mListCategoryTreeParam.Clear();
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3601Codes.OperationGetQuestionCategory;
                Service36011Client client = new Service36011Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service36011"));
                //var client = new Service36011Client();
                OperationReturn optReturn = XMLHelper.SeriallizeObject(strSql);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                webRequest.ListData.Add(optReturn.Data.ToString());
                WebReturn webReturn = client.UmpTaskOperation(webRequest);
                client.Close();
                if( !webReturn.Result )
                    return;

                for (int i = 0; i < webReturn.ListData.Count; i++ )
                {
                    optReturn = XMLHelper.DeserializeObject<PapersCategoryParam>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        continue;
                    }
                    PapersCategoryParam param = optReturn.Data as PapersCategoryParam;
                    if (param == null)
                    {
                        ShowException(string.Format("Fail. queryItem is null"));
                        return;
                    }

                    CCategoryTree tempTree = new CCategoryTree();
                    tempTree.LongNum = param.LongNum;
                    tempTree.StrName = param.StrName;
                    tempTree.LongParentNodeId = param.LongParentNodeId;
                    tempTree.StrParentNodeName = param.StrParentNodeName;
                    tempTree.LongFounderId = param.LongFounderId;
                    tempTree.StrFounderName = param.StrFounderName;
                    tempTree.StrDateTime = param.StrDateTime;
                    _mListCategoryTreeParam.Add(tempTree);
                }
                
                _mCategoryNode.Children.Clear();
                CategoryTree.ItemsSource = _mCategoryNode.Children;
                InitCategoryTree(_mListCategoryTreeParam, 0, _mCategoryNode);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        public void SearchCategoryTreeInfo(string strSql)
        {
            try
            {
                _mlstSearchCategoryNode = new List<CCategoryTree>();
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3601Codes.OperationGetQuestionCategory;
                Service36011Client client = new Service36011Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service36011"));
                //var client = new Service36011Client();
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
                    return;

                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    optReturn = XMLHelper.DeserializeObject<PapersCategoryParam>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        continue;
                    }
                    PapersCategoryParam param = optReturn.Data as PapersCategoryParam;
                    if (param == null)
                    {
                        ShowException(string.Format("Fail. queryItem is null"));
                        return;
                    }

                    CCategoryTree tempTree = new CCategoryTree();
                    tempTree.LongNum = param.LongNum;
                    tempTree.StrName = param.StrName;
                    tempTree.LongParentNodeId = param.LongParentNodeId;
                    tempTree.StrParentNodeName = param.StrParentNodeName;
                    tempTree.LongFounderId = param.LongFounderId;
                    tempTree.StrFounderName = param.StrFounderName;
                    tempTree.StrDateTime = param.StrDateTime;
                    _mlstSearchCategoryNode.Add(tempTree);
                }

                _mlstCategoryNum = new List<long>();
                foreach (var categoryTree in _mlstSearchCategoryNode)
                {
                    GetCategoryNum(categoryTree.LongNum);
                }

                var distinctNames = _mlstCategoryNum.Distinct();
                _mlstCategoryNum = new List<long>(distinctNames.ToList());
                    
                _mCategoryNode.Children.Clear();
                CategoryTree.ItemsSource = _mCategoryNode.Children;
                InitCategoryTree(_mListCategoryTreeParam, 0, _mCategoryNode);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
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

        void ATDocument_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                _mListQuestionInfosTemp.Clear();
                _mCExamQuestionsTemp = new CQuestionsParam();
                CQuestionsParam itame = null;
                S3601App.GListQuestionInfos.Clear();
                if (Keyboard.IsKeyDown(Key.RightCtrl) ||
                    Keyboard.IsKeyDown(Key.RightShift) ||
                    Keyboard.IsKeyDown(Key.LeftCtrl) ||
                    Keyboard.IsKeyDown(Key.LeftShift))
                {
                    int iCount = ATDocument.SelectedItems.Count;
                    for (int i = 0; i < iCount; i++)
                    {
                        itame = ATDocument.SelectedItems[i] as CQuestionsParam;
                        _mListQuestionInfosTemp.Add(itame);
                    }
                }
                else
                {
                    itame = ATDocument.SelectedItem as CQuestionsParam;
                    _mCExamQuestionsTemp = itame;
                    _mListQuestionInfosTemp.Add(itame);
                }
     
                S3601App.GListQuestionInfos = _mListQuestionInfosTemp;
                if (itame != null)
                {
                    BasicSetBut.Children.Clear();
                    DeleteSetBut.Children.Clear();
                    Button btn;
                    OperationInfo opt;
                    btn = new Button();
                    btn.Click += AddQuestions_Click;
                    opt = new OperationInfo();
                    opt.Display = CurrentApp.GetLanguageInfo("3601T00002", "Create Question");
                    opt.Icon = "Images/add.png";
                    btn.DataContext = opt;
                    btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                    BasicSetBut.Children.Add(btn);

                    btn = new Button();
                    btn.Click += SearchQuestions_Click;
                    opt = new OperationInfo();
                    opt.Display = CurrentApp.GetLanguageInfo("3601T00115", "Search Questions");
                    opt.Icon = "Images/add.png";
                    btn.DataContext = opt;
                    btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                    BasicSetBut.Children.Add(btn);

                    btn = new Button();
                    btn.Click += ExamineQuestion_Click;
                    opt = new OperationInfo();
                    opt.Display = CurrentApp.GetLanguageInfo("3601T00003", "Browse");
                    opt.Icon = "Images/add.png";
                    btn.DataContext = opt;
                    btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                    BasicSetBut.Children.Add(btn);

                    btn = new Button();
                    btn.Click += UpDateQuestion_Click;
                    opt = new OperationInfo();
                    opt.Display = CurrentApp.GetLanguageInfo("3601T00004", "Change Question");
                    opt.Icon = "Images/add.png";
                    btn.DataContext = opt;
                    btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                    BasicSetBut.Children.Add(btn);

                    btn = new Button();
                    btn.Click += DeleteQuestion_Click;
                    opt = new OperationInfo();
                    opt.Display = CurrentApp.GetLanguageInfo("3601T00005", "Delete Question");
                    opt.Icon = "Images/Delete.png";
                    btn.DataContext = opt;
                    btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                    DeleteSetBut.Children.Add(btn);

                    btn = new Button();
                    btn.Click += AddToPaper_Click;
                    opt = new OperationInfo();
                    opt.Display = CurrentApp.GetLanguageInfo("3601T00077", "Add To Paper");
                    opt.Icon = "Images/Delete.png";
                    btn.DataContext = opt;
                    btn.SetResourceReference(StyleProperty, "OptButtonStyle");

                    btn = new Button();
                    btn.Click += RemoveFromPaper_Click;
                    opt = new OperationInfo();
                    opt.Display = CurrentApp.GetLanguageInfo("3601T00078", "Remove from Paper");
                    opt.Icon = "Images/Delete.png";
                    btn.DataContext = opt;
                    btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void ATDocument_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ExamineQuestion();
        }

        private void ExamineQuestion_Click(object sender, RoutedEventArgs e)
        {
            var btn = e.Source as Button;
            if (btn != null)
            {
                ExamineQuestion();
            }
        }

        private void UpDateQuestion_Click(object sender, RoutedEventArgs e)
        {
            var btn = e.Source as Button;
            if (btn != null)
            {
                S3601App.GQuestionInfo.QuestionsParam = _mCExamQuestionsTemp;
                S3601App.GQuestionInfo.OptType = S3601Codes.OperationUpdateQuestion;
                var optItem = btn.DataContext as OperationInfo;
                GetCategoryTreeInfo(_mCExamQuestionsTemp.LongCategoryNum);
                CreateAddQuestionPage();
            }
        }

        private void DeleteQuestion_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(CurrentApp.GetLanguageInfo("3601T00106", "Confirm delete Question?"),
                        CurrentApp.GetLanguageInfo("3601T00090", "Warning"),
                        MessageBoxButton.OKCancel);
            if (result != MessageBoxResult.OK)
                return;

            foreach (CQuestionsParam param in _mListQuestionInfosTemp )
            {
                if ( param.IntUseNumber > 0)
                {
                    MessageBox.Show(CurrentApp.GetLanguageInfo("3601T00008", "No questions have been used to delete!"),
                        CurrentApp.GetLanguageInfo("3601T00090", "Warning"),
                        MessageBoxButton.OK);
                    return;
                }
            }

            if (DeleteQuestionInfos())
            {
                foreach( CQuestionsParam cExamQuestion in _mListQuestionInfosTemp )
                {
                    _mlistQuestionsParams.Remove(cExamQuestion);
                }     
            }
            SetObservableCollection();
        }

        private void AddToPaper_Click(object sender, RoutedEventArgs e)
        {
            S3601App.GQueryModify = false;
            OptPaperPage newPage = new OptPaperPage();
            newPage.ParentPage = this;
            PopupPanelQuestionInfo.Content = newPage;
            S3601App.GMessageSource = new S3601Codes();
            S3601App.GMessageSource = S3601Codes.MessageExamproduction;
            PopupPanelQuestionInfo.IsOpen = true;
        }

        private void RemoveFromPaper_Click(object sender, RoutedEventArgs e)
        {
            S3601App.GQueryModify = false;
            PaperRemovQuestionPage newPage = new PaperRemovQuestionPage();
            newPage.ParentPage = this;
            PopupPanelQuestionInfo.Content = newPage;
            S3601App.GMessageSource = new S3601Codes();
            S3601App.GMessageSource = S3601Codes.MessageExamproduction;
            PopupPanelQuestionInfo.IsOpen = true;
        }

        private void AddQuestions_Click(object sender, RoutedEventArgs e)
        {
            var btn = e.Source as Button;
            if (btn != null)
            {
                S3601App.GQuestionInfo.QuestionsParam = new CQuestionsParam();
                S3601App.GQuestionInfo.OptType = S3601Codes.OperationCreateQuestion; 
                var optItem = btn.DataContext as OperationInfo;
                CreateAddQuestionPage();
            }
        }

        private void CreateCategory_Click( object sender, RoutedEventArgs e )
        {
            try
            {
                if (string.IsNullOrEmpty(_mCategoryNodeTemp.StrName))
                {
                    return;
                }

                S3601App.GQueryModify = false;
                S3601App.GMessageSource = S3601Codes.MessageCreateCategory;
                List<CCategoryTree> lstCategoryTreeChild = GetAllChildInfo(_mCategoryNodeTemp);
                if (lstCategoryTreeChild == null)
                {
                    return;
                }
                _mCategoryNodeTemp.LstChildInfos = lstCategoryTreeChild;
                S3601App.GCategoryTreeNode = _mCategoryNodeTemp;
                NewMainCategory newPage = new NewMainCategory();
                newPage.ParentPage = this;
                newPage.CurrentApp = CurrentApp;
                PopupPanelCreateCategory.Content = newPage;
                PopupPanelCreateCategory.Title = CurrentApp.GetLanguageInfo("3601T00013", "Create Category");
                PopupPanelCreateCategory.IsOpen = true;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void SearchCategory_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty( TxtSearchCategroy.Text ))
                {
                   return;
                }

                string strSql = string.Format("SELECT * FROM T_36_021_{0} WHERE C002 like '%{1}%'", CurrentApp.Session.RentInfo.Token, TxtSearchCategroy.Text);
                SearchCategoryTreeInfo(strSql);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void ChangeCategory_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                S3601App.GMessageSource = S3601Codes.MessageUpdateCategory;
                if (_mCategoryNodeTemp.LongParentNodeId == 0)
                {
                    return;
                }
                List<CCategoryTree> lstCategoryTreeChild = GetAllChildInfo(_mCategoryNodeTemp);
                if (lstCategoryTreeChild == null)
                {
                    return;
                }
                _mCategoryNodeTemp.LstChildInfos = lstCategoryTreeChild;
                List<CCategoryTree> lstCategoryTreeNode = GetAllNodeInfo(_mCategoryNodeTemp);
                if (lstCategoryTreeNode == null)
                {
                    return;
                }
                _mCategoryNodeTemp.LstNodeInfos = lstCategoryTreeNode;
                S3601App.GCategoryTreeNode = _mCategoryNodeTemp;
                S3601App.GQueryModify = false;
                NewMainCategory newPage = new NewMainCategory();
                newPage.ParentPage = this;
                newPage.CurrentApp = CurrentApp;
                PopupPanelCreateCategory.Content = newPage;
                PopupPanelCreateCategory.Title = CurrentApp.GetLanguageInfo("3601T00153", "Change Category");
                PopupPanelCreateCategory.IsOpen = true;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void DeleteCategory_Click(object sender, RoutedEventArgs e)
        {
            string strChildNode = String.Empty;
            List<CCategoryTree> lstCategoryTreeChild = GetAllChildInfo(_mCategoryNodeTemp);
            if (lstCategoryTreeChild.Count > 0)
            {
                strChildNode = string.Format("{0} {1}", lstCategoryTreeChild.Count, CurrentApp.GetLanguageInfo("3601T00159", "Child Node Information"));
            }
            else
            {
                strChildNode = string.Format("{0}", CurrentApp.GetLanguageInfo("3601T00160", "Child Node Information"));
            }

            MessageBoxResult result =
                MessageBox.Show(
                    string.Format("{0}\n{1}",
                        CurrentApp.GetLanguageInfo("3601T00156",
                            "Confirm whether delete categories? Delete nodes will also delete all child nodes."),
                        strChildNode),
                    CurrentApp.GetLanguageInfo("3601T00090", "Warning"),
                    MessageBoxButton.OKCancel);
            if (result != MessageBoxResult.OK)
                return;

            string strLog;
            var btn = e.Source as Button;
            PapersCategoryParam papersCategoryParam = new PapersCategoryParam();
            papersCategoryParam.LongNum = _mCategoryNodeTemp.LongNum;
            papersCategoryParam.StrName = _mCategoryNodeTemp.StrName;
            papersCategoryParam.LongParentNodeId = _mCategoryNodeTemp.LongParentNodeId;
            papersCategoryParam.StrParentNodeName = _mCategoryNodeTemp.StrParentNodeName;
            papersCategoryParam.LongFounderId = _mCategoryNodeTemp.LongFounderId;
            papersCategoryParam.StrFounderName = _mCategoryNodeTemp.StrFounderName;
            papersCategoryParam.StrDateTime = _mCategoryNodeTemp.StrDateTime;

            try
            {
                if (btn != null)
                {
                    WebRequest webRequest = new WebRequest();
                    webRequest.Session = CurrentApp.Session;
                    webRequest.Code = (int)S3601Codes.OperationDeleteCategory;
                    Service36011Client client = new Service36011Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                        WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service36011"));
                    OperationReturn optReturn = XMLHelper.SeriallizeObject(papersCategoryParam);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    webRequest.ListData.Add(optReturn.Data.ToString());
                    //var client = new Service36011Client();
                    WebReturn webReturn = client.UmpTaskOperation(webRequest);
                    client.Close();
                    CurrentApp.WriteLog(CurrentApp.GetLanguageInfo("3601T00014", "Delete"));
                    if (!webReturn.Result)
                    {
                        ShowException(CurrentApp.GetLanguageInfo("3107T00092", "Delete Failed"));
                        #region 写操作日志
                        strLog = string.Format("{0} {1}{2}", Utils.FormatOptLogString("3601T00014"), Utils.FormatOptLogString("3601T00083"), papersCategoryParam.StrName);
                        CurrentApp.WriteOperationLog(S3601Consts.OPT_Delete.ToString(), ConstValue.OPT_RESULT_FAIL, strLog);
                        #endregion
                        CurrentApp.WriteLog(webReturn.Message);
                        return;
                    }
                    if (webReturn.Message == S3601Consts.HadUse)// 该查询条件被使用无法删除
                    {
                        #region 写操作日志
                        strLog = string.Format("{0} {1}{2}", Utils.FormatOptLogString("3601T00014"), Utils.FormatOptLogString("3601T00011"), papersCategoryParam.StrName);
                        CurrentApp.WriteOperationLog(S3601Consts.OPT_Delete.ToString(), ConstValue.OPT_RESULT_FAIL, strLog);
                        #endregion
                        CurrentApp.WriteLog(CurrentApp.GetLanguageInfo("3601T00011", "Can't Delete"));
                        ShowInformation(CurrentApp.GetLanguageInfo("3601T00011", "Can't Delete"));
                    }
                    else
                    {
                        CurrentApp.WriteLog(CurrentApp.GetLanguageInfo("3601T00012", "Delete Sucessed"));
                        ShowInformation(CurrentApp.GetLanguageInfo("3601T00012", "Delete Sucessed"));
                        string strSql = string.Format("SELECT * FROM T_36_021_{0}", CurrentApp.Session.RentInfo.Token);
                        InitCategoryTreeInfo(strSql);
                    }  
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void SearchQuestions_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                S3601App.GQueryModify = false;
                S3601App.GPageSize = _mPageSize;
                S3601App.GLstCCategoryTrees = _mListCategoryTreeParam;
                SearchQuestionPage newPage = new SearchQuestionPage();
                newPage.CategoryTree.ItemsSource = _mCategoryNode.Children;
                newPage.ParentPage = this;
                newPage.CurrentApp = this.CurrentApp;
                PopupPanelSearchQuestion.Content = newPage;
                PopupPanelSearchQuestion.Title = CurrentApp.GetLanguageInfo("3601T00115", "Search Question");
                PopupPanelSearchQuestion.IsOpen = true;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
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
            catch { }
        }

        /// <summary>
        /// 单击树事件
        /// </summary>
        private void OrgCategoryTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                CCategoryTree nodeInfo = CategoryTree.SelectedItem as CCategoryTree;
                if (nodeInfo == null) { return; }

                BasicSetBut.Children.Clear();
                DeleteSetBut.Children.Clear();
                CategorySetBut.Children.Clear();
                Button btn;
                OperationInfo opt;
                btn = new Button();
                btn.Click += AddQuestions_Click;
                opt = new OperationInfo();
                opt.Display = CurrentApp.GetLanguageInfo("3601T00002", "Create Question");
                opt.Icon = "Images/add.png";
                btn.DataContext = opt;
                btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                BasicSetBut.Children.Add(btn);

                btn = new Button();
                btn.Click += CreateCategory_Click;
                opt = new OperationInfo();
                opt.Display = CurrentApp.GetLanguageInfo("3601T00013", "Add Create");
                opt.Icon = "Images/add.png";
                btn.DataContext = opt;
                btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                CategorySetBut.Children.Add(btn);

                btn = new Button();
                btn.Click += ChangeCategory_Click;
                opt = new OperationInfo();
                opt.Display = CurrentApp.GetLanguageInfo("3601T00153", "Channge Create");
                opt.Icon = "Images/add.png";
                btn.DataContext = opt;
                btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                CategorySetBut.Children.Add(btn);

                btn = new Button();
                btn.Click += SearchQuestions_Click;
                opt = new OperationInfo();
                opt.Display = CurrentApp.GetLanguageInfo("3601T00115", "Search Questions");
                opt.Icon = "Images/add.png";
                btn.DataContext = opt;
                btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                BasicSetBut.Children.Add(btn);

                if (nodeInfo.LongParentNodeId != 0)
                {
                    btn = new Button();
                    btn.Click += DeleteCategory_Click;
                    opt = new OperationInfo();
                    opt.Display = CurrentApp.GetLanguageInfo("3601T00014", "Delete Create");
                    opt.ID = 31070021;
                    opt.Icon = "Images/Delete.png";
                    btn.DataContext = opt;
                    btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                    DeleteSetBut.Children.Add(btn);
                }
                
                _mCategoryNodeTemp = nodeInfo;
                S3601App.GCategoryTree = nodeInfo;
                _mPageNum = null;
                _mlistQuestionsParams.Clear();
                _mQuestionNum = 0;
                _mPageIndex = 0;
                _mPageCount = 0;
                _mbListViewEnable = true;
                GetQuestionInfos();

            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void AddQuestionsDocument_IsSelectedChanged(object sender, EventArgs e)
        {
            if( !_mBTableAddQuestions )
            {
                _mBTableAddQuestions = true;
                _mBTableBrowsePapers = false;
                DeleteSetBut.Children.Clear();

                try
                {
                    BasicSetBut.Children.Clear();
                    Button btn;
                    OperationInfo opt;
                    btn = new Button();
                    btn.Click += AddQuestions_Click;
                    opt = new OperationInfo();
                    opt.Display = CurrentApp.GetLanguageInfo("3601T00002", "Create Question");
                    opt.Icon = "Images/add.png";
                    btn.DataContext = opt;
                    btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                    BasicSetBut.Children.Add(btn);

                    btn = new Button();
                    btn.Click += SearchQuestions_Click;
                    opt = new OperationInfo();
                    opt.Display = CurrentApp.GetLanguageInfo("3601T00115", "Search Questions");
                    opt.Icon = "Images/add.png";
                    btn.DataContext = opt;
                    btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                    BasicSetBut.Children.Add(btn);

                    PanelObjectTreeBox.Show();
                    PanelToolButton.Visibility = Visibility.Visible;
                    //RefreshTree();
                }
                catch (Exception ex)
                {
                    ShowException(ex.Message);
                }
            }  
        }

        #endregion

        #region Operations
        public void RefreshTree(CCategoryTree cCategoryTree)
        {
            _mListCategoryTreeParam.Add(cCategoryTree);
            _mlstCategoryNum = new List<long>();
            GetCategoryNum(cCategoryTree.LongNum);
            string strSql = string.Format("SELECT * FROM T_36_021_{0}", CurrentApp.Session.RentInfo.Token);
            InitCategoryTreeInfo(strSql);
        }

//        public void UpdateCategory(CCategoryTree cCategoryTree)
//         {
//             foreach (var categoryTree in _mListCategoryTreeParam)
//             {
//                 if (categoryTree.LongNum == cCategoryTree.LongNum)
//                 {
//                     categoryTree.StrName = cCategoryTree.StrName;
//                 }
//             }
//         }

        public void UpdateQuestion()
        {
            GetCategoryTreeInfo(_mCExamQuestionsTemp.LongCategoryNum);
            CreateAddQuestionPage();
        }

        public void AddPaperList()
        {
            if ( S3601App.GPaperInfo.OptType == S3601Codes.OperationAddPaper )
            {
                _mAddPaperInfo = S3601App.GPaperInfo;
                _mListAddPapers.Add(_mAddPaperInfo.PaperParam);
            }
            else
            {
                _mListAddPapers.Remove(
                    _mListAddPapers.FirstOrDefault(p => p.LongNum == S3601App.GPaperInfo.PaperParam.LongNum));
                _mListAddPapers.Add(S3601App.GPaperInfo.PaperParam);
            }
        }

        private void CreateAddQuestionPage()
        {
            try
            {
                CreateQuestionsView  newView = new CreateQuestionsView();
                newView.PageName = "CreateQuestion";
                newView.CurrentApp = this.CurrentApp;
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
                        panel.Title = CurrentApp.GetLanguageInfo("3601T00034", "Question Category");

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

        private string SetSql(string stNum )
        {
            string strSql = string.Empty;
            switch (CurrentApp.Session.DBType)
            {
                case 2:
                    strSql =
                        string.Format(
                            "SELECT TOP {0} X.* FROM T_36_022_{1} X WHERE c002='{2}' AND X.C001 > '{3}' ORDER BY X.C001",
                            _mPageSize, CurrentApp.Session.RentInfo.Token, _mCategoryNodeTemp.LongNum, stNum);
                    
                    break;
                case 3:
                     strSql =
                        string.Format(
                            "SELECT * FROM (SELECT X.*  FROM T_36_022_{1} X  WHERE X.c002='{2}' And X.C001 > '{3}' ORDER BY X.C001) WHERE ROWNUM <= {0} ",
                            _mPageSize, CurrentApp.Session.RentInfo.Token, _mCategoryNodeTemp.LongNum, stNum);
                    break;
            }  
            return strSql;    
        }

        private string SetSql()
        {
            if (_mPageNum == null)
            {
                string strSql = string.Empty;
                switch (CurrentApp.Session.DBType)
                {
                    case 2:
                        strSql =
                            string.Format(
                                "SELECT TOP {0} X.* FROM T_36_022_{1} X WHERE X.c002='{2}' ORDER BY X.C001",
                                _mPageSize, CurrentApp.Session.RentInfo.Token, _mCategoryNodeTemp.LongNum);
                        break;
                    case 3:
                        strSql =
                           string.Format(
                               "SELECT * FROM (SELECT X.*  FROM T_36_022_{1} X  WHERE X.c002='{2}' ORDER BY X.C001) WHERE ROWNUM <= {0} ",
                               _mPageSize, CurrentApp.Session.RentInfo.Token, _mCategoryNodeTemp.LongNum);
                        break;
                }
                return strSql; 
            }
            else
            {
                return SetSql(_mPageNum);
            }
        }

        private void GetQuestionInfos()
        {
            try
            {
                string strSql = SetSql();
                var webRequest = new WebRequest();
                var client = new Service36011Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service36011"));
                //var client = new Service36011Client();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3601Codes.OperationGetQuestions;
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
                    ShowException(CurrentApp.GetLanguageInfo("3601T00076", "Insert data failed"));
                    return;
                }

                if (webReturn.ListData.Count <= 0)
                {
                    if (_mbListViewEnable)
                    {
                        SetPageState();
                        SetObservableCollection();
                        _mbListViewEnable = false;
                    }
                    return;
                }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    optReturn = new OperationReturn();
                    optReturn = XMLHelper.DeserializeObject<CQuestionsParam>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        continue;
                    }
                    var cExamQuestions = optReturn.Data as CQuestionsParam;
                    if (cExamQuestions == null)
                    {
                        ShowException(string.Format("Fail. filesItem is null"));
                        return;
                    }
                    int total = _mQuestionNum + 1;
                    if (total > _mMaxInfos)
                    {
                        ShowInformation(CurrentApp.GetLanguageInfo("3601T00145", string.Format("Larger than allowed max Questions, some Questions can't be displayed")));
                        return;
                    }
                    _mQuestionNum = total;
                    _mPageNum = cExamQuestions.LongNum.ToString();
                    _mlistQuestionsParams.Add(cExamQuestions);
                    SetPageState();
                }
                if (_mbListViewEnable)
                {
                    SetObservableCollection();
                    _mbListViewEnable = false;
                }
                GetQuestionInfos();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void SearchQuestionInfos()
        {
            try
            {
                string strSql = SetSearchSql();
                var webRequest = new WebRequest();
                var client = new Service36011Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service36011"));
                //var client = new Service36011Client();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3601Codes.OperationGetQuestions;
                OperationReturn optReturn = XMLHelper.SeriallizeObject(strSql);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                webRequest.ListData.Add(optReturn.Data.ToString());
                WebReturn webReturn = client.UmpTaskOperation(webRequest);
                client.Close();
                string strLog = string.Empty;
                CurrentApp.WriteLog(CurrentApp.GetLanguageInfo("3601T00115", "Search Questions"));
                if (!webReturn.Result)
                {
                    #region 写操作日志
                    strLog = string.Format("{0} {1} : {2}", Utils.FormatOptLogString("3601T00115"), Utils.FormatOptLogString("3601T00188"), webReturn.Message);
                    CurrentApp.WriteOperationLog(S3601Consts.OPT_Search.ToString(), ConstValue.OPT_RESULT_FAIL, strLog);
                    CurrentApp.WriteLog(webReturn.Message);
                    #endregion
                    ShowException(CurrentApp.GetLanguageInfo("3601T00076", "Insert data failed"));
                    return;
                }
                #region 写操作日志
                strLog = string.Format("{0} {1}", Utils.FormatOptLogString("3601T00115"), Utils.FormatOptLogString("3601T00189"));
                CurrentApp.WriteOperationLog(S3601Consts.OPT_Search.ToString(), ConstValue.OPT_RESULT_SUCCESS, strLog);
                #endregion
                CurrentApp.WriteLog(CurrentApp.GetLanguageInfo("3601T00189", "Search Success"));

                if (webReturn.ListData.Count <= 0)
                {
                    if (_mbListViewEnable)
                    {
                        SetPageState();
                        SetObservableCollection();
                        _mbListViewEnable = false;
                    }
                    return;
                }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    optReturn = new OperationReturn();
                    optReturn = XMLHelper.DeserializeObject<CQuestionsParam>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        continue;
                    }
                    var cExamQuestions = optReturn.Data as CQuestionsParam;
                    if (cExamQuestions == null)
                    {
                        ShowException(string.Format("Fail. filesItem is null"));
                        return;
                    }
                    int total = _mQuestionNum + 1;
                    if (total > _mMaxInfos)
                    {
                        ShowInformation(CurrentApp.GetLanguageInfo("3601T00145", string.Format("Larger than allowed max Questions, some Questions can't be displayed")));
                        return;
                    }
                    _mQuestionNum = total;
                    _mPageNum = cExamQuestions.LongNum.ToString();
                    _mlistQuestionsParams.Add(cExamQuestions);
                    SetPageState();
                }
                if (_mbListViewEnable)
                {
                    SetObservableCollection();
                    _mbListViewEnable = false;
                }
                SearchQuestionInfos();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private bool DeleteQuestionInfos()
        {
            string strLog = null;
            try
            {
                var webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3601Codes.OperationDeleteQuestion;
                Service36011Client client = new Service36011Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service36011"));
                OperationReturn optReturn = XMLHelper.SeriallizeObject(_mListQuestionInfosTemp);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return false ;
                }
                webRequest.ListData.Add(optReturn.Data.ToString());
                //var client = new Service36011Client();
                WebReturn webReturn = client.UmpTaskOperation(webRequest);
                client.Close();
                CurrentApp.WriteLog(CurrentApp.GetLanguageInfo("3601T00005", "Delete Questions"));
                if (!webReturn.Result)
                {
                    ShowException(CurrentApp.GetLanguageInfo("3107T00092", "Delete Failed"));
                    #region 写操作日志
                    strLog = string.Format("{0} {1} : {2}", Utils.FormatOptLogString("3107T00005"), Utils.FormatOptLogString("3601T00083"), webReturn.Message);
                    CurrentApp.WriteOperationLog(S3601Consts.OPT_Delete.ToString(), ConstValue.OPT_RESULT_FAIL, strLog);
                    #endregion
                    return false ;
                }
                if (webReturn.Message == S3601Consts.HadUse)// 该查询条件被使用无法删除
                {
                    ShowInformation(CurrentApp.GetLanguageInfo("3601T00011", "Can't Delete"));
                    #region 写操作日志
                    strLog = string.Format("{0} {1} : {2}", Utils.FormatOptLogString("3107T00005"), Utils.FormatOptLogString("3601T00083"), CurrentApp.GetLanguageInfo("3601T00011", "Can't Delete"));
                    CurrentApp.WriteOperationLog(S3601Consts.OPT_Delete.ToString(), ConstValue.OPT_RESULT_FAIL, strLog);
                    #endregion
                    CurrentApp.WriteLog(CurrentApp.GetLanguageInfo("3601T00011", "Can't Delete"));
                    return false ;
                }
                ShowInformation(CurrentApp.GetLanguageInfo("3601T00012", "Delete Sucessed"));
                CurrentApp.WriteLog(CurrentApp.GetLanguageInfo("3601T00012", "Delete Sucessed"));
                string strSql = string.Format("SELECT * FROM T_36_021_{0}", CurrentApp.Session.RentInfo.Token);
                InitCategoryTreeInfo(strSql);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
                return false;
            }
            return true;
        }

        public void OpenEditPaperPage()
        {
            //CreateEditPaperPage();
        }

        private string SetSearchSql()
        {
            string strTemp = null;
            string strSql = string.Empty;
            int iMax = 0;
            int iMin = 0;

            switch (CurrentApp.Session.DBType)
            {
                case 2:
                    if (string.IsNullOrEmpty(_mPageNum))
                    {
                        strSql = string.Format("select TOP {0} X.* from T_36_022_{1} X where ", S3601App.GPageSize, CurrentApp.Session.RentInfo.Token);
                    }
                    else
                    {
                        strSql = string.Format("select TOP {0} X.* from T_36_022_{1} X where X.c001>'{2}' and ", S3601App.GPageSize, CurrentApp.Session.RentInfo.Token, _mPageNum);
                    }
                    if (_mSearchQuestionParam.LongCategoryNum != 0)
                    {
                        strTemp = string.Format(" X.c002='{0}'", _mSearchQuestionParam.LongCategoryNum);
                        strSql += strTemp;
                    }
                    break;
                case 3:
                    if (string.IsNullOrEmpty(_mPageNum))
                    {
                        strSql = string.Format("SELECT * FROM ( select X.* from T_36_022_{0} X where ", CurrentApp.Session.RentInfo.Token);
                    }
                    else
                    {
                        strSql = string.Format("SELECT * FROM ( select X.* from T_36_022_{0} X where X.c001>'{1}' and ", CurrentApp.Session.RentInfo.Token, _mPageNum);
                    }

                    if (_mSearchQuestionParam.LongCategoryNum != 0)
                    {
                        strTemp = string.Format(" X.c002='{0}'", _mSearchQuestionParam.LongCategoryNum);
                        strSql += strTemp;
                    }
                    break;
            }

            if (!string.IsNullOrEmpty(_mSearchQuestionParam.StrQuestionsContect))
            {
                if (strTemp == null)
                    strTemp = string.Format(" X.c005 like '%{0}%'", _mSearchQuestionParam.StrQuestionsContect);
                else
                    strTemp = string.Format(" and X.c005 like '%{0}%'", _mSearchQuestionParam.StrQuestionsContect);
                strSql += strTemp;
            }
            string[] strIndex = _mSearchQuestionParam.StrQuestionType.Split((char)27);

            if (strIndex.Length > 1)
            {
                for (int i = 0; i < strIndex.Length; i++)
                {
                    if (!string.IsNullOrEmpty(strIndex[i]))
                    {
                        if (i == 0 && strTemp != null)
                        {
                            strTemp = string.Format(" and ( X.c004 = '{0}'", strIndex[i]);
                        }
                        if (i == 0 && strTemp == null)
                        {
                            strTemp = string.Format(" (X.c004 = '{0}'", strIndex[i]);
                        }
                        if (i != 0)
                        {
                            strTemp = string.Format(" or X.c004 = '{0}'", strIndex[i]);
                        }
                        strSql += strTemp;
                    }
                }
                strSql += string.Format(" )");
            }
            else
            {
                if (!string.IsNullOrEmpty(strIndex[0]))
                {
                    if (strTemp == null)
                        strTemp = string.Format(" X.c004 = '{0}'", strIndex[0]);
                    else
                        strTemp = string.Format(" and X.c004 = '{0}'", strIndex[0]);
                    strSql += strTemp;
                }
            }
            
            if (_mSearchQuestionParam.StrAccessoryType!= null)
            {
                if (strTemp == null)
                    strTemp = string.Format(" X.c020 = '{0}'", _mSearchQuestionParam.StrAccessoryType);
                else
                    strTemp = string.Format(" and X.c020 = '{0}'", _mSearchQuestionParam.StrAccessoryType);
                strSql += strTemp;
            }

            iMin = _mSearchQuestionParam.IntUseMin;
            iMax = _mSearchQuestionParam.IntUseMax;
            if (iMin != 0 || iMax != 0)
            {
                if (iMin > iMax)
                {
                    ShowException(string.Format("{0}",
                        CurrentApp.GetLanguageInfo("3602T00122", "UsedNum range is not correct!")));
                    return null;
                }
                else
                {
                    if (strTemp == null)
                    {
                        strTemp = string.Format(" X.c019 >= {0} AND X.c019 <= {1} ", iMin, iMax);
                    }
                    else
                    {
                        strTemp = string.Format(" and X.c019 >= {0} AND X.c019 <= {1} ", iMin, iMax);
                    }
                    strSql += strTemp;
                }
            }

            switch (CurrentApp.Session.DBType)
            {
                case 2:
                    if (strTemp == null)
                        strTemp = string.Format(" X.c025 >= '{0}' and X.c025 <= '{1}' ORDER BY X.C001", _mSearchQuestionParam.StrStartTime, _mSearchQuestionParam.StrEndTime);
                    else
                        strTemp = string.Format(" and X.c025 >= '{0}' and X.c025 <= '{1}' ORDER BY X.C001", _mSearchQuestionParam.StrStartTime, _mSearchQuestionParam.StrEndTime);
                    break;
                case 3:
                    if (strTemp == null)
                        strTemp =
                            string.Format(
                                " X.c025 between to_date('{0}','yyyy-mm-dd hh24:mi:ss')  and to_date('{1}','yyyy-mm-dd hh24:mi:ss') ORDER BY X.C001 )WHERE ROWNUM <= {2}",
                                _mSearchQuestionParam.StrStartTime, _mSearchQuestionParam.StrEndTime, S3601App.GPageSize);
                    else
                        strTemp =
                            string.Format(
                                " and X.c025 between to_date('{0}','yyyy-mm-dd hh24:mi:ss')  and to_date('{1}','yyyy-mm-dd hh24:mi:ss') ORDER BY X.C001) WHERE ROWNUM <= {2}",
                                _mSearchQuestionParam.StrStartTime, _mSearchQuestionParam.StrEndTime, S3601App.GPageSize);
                    break;
            }
            return strSql += strTemp;
        }

        public void SearchQuestions( CSearchQuestionParam searchQuestionParam )
        {
            _mSearchQuestionParam = new CSearchQuestionParam();
            _mSearchQuestionParam = searchQuestionParam;
            _mPageNum = null;
            _mlistQuestionsParams.Clear();
            _mQuestionNum = 0;
            _mPageIndex = 0;
            _mPageCount = 0;
            _mbListViewEnable = true;

            SearchQuestionInfos();
        }

        //获取UMP安装目录
        private void GetUmpSetupPath()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3601Codes.OperationGetUmpsetuppath;
                Service36011Client client = new Service36011Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service36011"));
                //var client = new Service36011Client();
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

        private void ExamineQuestion()
        {
            try
            {
                S3601App.GQueryModify = false;
                BrowseQustions newPage = new BrowseQustions();
                newPage.EpParentPage = this;
                newPage.CurrentApp = this.CurrentApp;
                PopupPanelQuestionInfo.Content = newPage;
                PopupPanelQuestionInfo.Title = CurrentApp.GetLanguageInfo("3601T00117", "Question Information");
                S3601App.GMessageSource = new S3601Codes();
                S3601App.GMessageSource = S3601Codes.MessageExamproduction;
                PopupPanelQuestionInfo.IsOpen = true;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void SetObservableCollection()
        {
            _mListExamQuestions.Clear();
            foreach (var questionsParam in _mlistQuestionsParams)
            {
                switch ((QuestionType)questionsParam.IntType)
                {
                    case QuestionType.TrueOrFalse:
                        questionsParam.StrType = CurrentApp.GetLanguageInfo("3601T00098", "True Or False");
                        break;
                    case QuestionType.SingleChoice:
                        questionsParam.StrType = CurrentApp.GetLanguageInfo("3601T00097", "Single Choice");
                        break;
                    case QuestionType.MultipleChioce:
                        questionsParam.StrType = CurrentApp.GetLanguageInfo("3601T00099", "Multiple Chioce");
                        break;
                }
                questionsParam.StrConOne = questionsParam.CorrectAnswerOne;
                if (questionsParam.CorrectAnswerOne == "F")
                {
                    questionsParam.StrConOne = CurrentApp.GetLanguageInfo("3601T00065", "F");
                }
                if (questionsParam.CorrectAnswerOne == "T")
                {
                    questionsParam.StrConOne = CurrentApp.GetLanguageInfo("3601T00064", "T");
                }
                _mListExamQuestions.Add(questionsParam);
            }
        }

        private void GetCategoryTreeInfo(long longCategoryNum)
        {
            _mlstCategoryNum = new List<long>();
            GetCategoryNum(longCategoryNum);
            S3601App.GLstCategoryNum = _mlstCategoryNum;
        }

        private void GetCategoryNum(long longCategoryNum)
        {
            if (longCategoryNum == 0)
            {
                return;
            }
            long lNum = new long();
            foreach (var categoryTree in _mListCategoryTreeParam)
            {
                if (categoryTree.LongNum == longCategoryNum)
                {
                    lNum = categoryTree.LongParentNodeId;
                    GetCategoryNum(lNum);
                    _mlstCategoryNum.Add(lNum);
                    return;
                }
            }
            return;
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
                    CurrentApp.GetLanguageInfo("3601T00147", "Sum"));
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
                _mListExamQuestions.Clear();
                int intStart = _mPageIndex * _mPageSize;
                int intEnd = (_mPageIndex + 1) * _mPageSize;
                for (int i = intStart; i < intEnd && i < _mQuestionNum; i++)
                {
                    CQuestionsParam questionsParam = _mlistQuestionsParams[i];

                    switch ((QuestionType)questionsParam.IntType)
                    {
                        case QuestionType.TrueOrFalse:
                            questionsParam.StrType = CurrentApp.GetLanguageInfo("3601T00098", "True Or False");
                            break;
                        case QuestionType.SingleChoice:
                            questionsParam.StrType = CurrentApp.GetLanguageInfo("3601T00097", "Single Choice");
                            break;
                        case QuestionType.MultipleChioce:
                            questionsParam.StrType = CurrentApp.GetLanguageInfo("3601T00099", "Multiple Chioce");
                            break;
                    }

                    _mListExamQuestions.Add(questionsParam);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private List<CCategoryTree> GetAllChildInfo(CCategoryTree categoryTreeNode)
        {
            List<CCategoryTree> lstCategoryTreeChild = new List<CCategoryTree>();
            if (_mListCategoryTreeParam.Count <= 0)
            {
                return null;
            }
            foreach (var categoryTree in _mListCategoryTreeParam)
            {
                if (categoryTreeNode.LongNum == categoryTree.LongParentNodeId)
                {
                    lstCategoryTreeChild.Add(categoryTree);
                }
            }
            return lstCategoryTreeChild;
        }

        private List<CCategoryTree> GetAllNodeInfo(CCategoryTree categoryTreeNode)
        {
            List<CCategoryTree> lstCategoryTreeNode = new List<CCategoryTree>();
            if (_mListCategoryTreeParam.Count <= 0)
            {
                return null;
            }
            foreach (var categoryTree in _mListCategoryTreeParam)
            {
                if (categoryTreeNode.LongParentNodeId == categoryTree.LongParentNodeId)
                {
                    lstCategoryTreeNode.Add(categoryTree);
                }
            }
            return lstCategoryTreeNode;
        }

        #endregion
    }
}
