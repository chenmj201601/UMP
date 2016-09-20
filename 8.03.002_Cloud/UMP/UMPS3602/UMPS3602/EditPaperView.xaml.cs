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
using System.Windows.Media;
using Common3602;
using UMPS3602.Models;
using UMPS3602.Wcf36021;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;
using VoiceCyber.Wpf.AvalonDock.Layout;

namespace UMPS3602
{
    /// <summary>
    /// EditPaper.xaml 的交互逻辑
    /// </summary>
    public partial class EditPaperView
    {
        #region Members
        public ExamProductionView ParentPage;
        private readonly List<CCategoryTree> _mListCategoryTreeParam;
        private CCategoryTree _mCategoryNodeTemp;
        private readonly ObservableCollection<CQuestionsParam> _mObjQuestionInfo;
        private readonly List<CQuestionsParam> _mLstQuestionInfos;
        private readonly List<CQuestionsParam> _mLstQuestionsTemp;
        private readonly ObservableCollection<CPaperQuestionParam> _mObjPaperQuestionInfos;
        private readonly List<CPaperQuestionParam> _mLstPaperQuestions;
        private readonly List<CPaperQuestionParam> _mLstPaperQuestionsTemp;
        public readonly List<CPaperQuestionParam> MLstPaperQuestionsOld;
        public List<CPaperQuestionParam> MLstChangePaperQuestion;
        private CSearchQuestionParam _mSearchQuestionParam;
        private List<CCategoryTree> _mlstSearchCategoryNode;

        private readonly CCategoryTree _mCategoryNode;
        private PaperInfo _mPaperInfo;
        private static bool _mEnableBack = true;
        private static bool _mbPaperContentTable;
        private static bool _mbPaperTable;
        private List<PanelItem> _mListPanels;
        private List<long> _mlstCategoryNum;
        private static bool _mbChangePaperProperties; 

        private int _mPageIndex;//页的索引,这个是从0开始算的
        private int _mPageCount;
        private int _mPageSize;
        private int _mQuestionNum;
        private int _mMaxInfos;
        private string _mPageNum;
        private bool _mbListViewEnable;
        #endregion

        public EditPaperView()
        {
            _mbChangePaperProperties = false;
            _mbPaperContentTable = false;
            _mbPaperTable = false;
            _mCategoryNodeTemp = new CCategoryTree();
            _mListCategoryTreeParam = new List<CCategoryTree>();
            _mCategoryNode = new CCategoryTree();
            _mObjQuestionInfo = new ObservableCollection<CQuestionsParam>();
            _mLstQuestionInfos = new List<CQuestionsParam>();
            _mLstQuestionsTemp = new List<CQuestionsParam>();
            _mPaperInfo = S3602App.GPaperInfo;
            _mObjPaperQuestionInfos = new ObservableCollection<CPaperQuestionParam>();
            _mLstPaperQuestions = new List<CPaperQuestionParam>();
            _mLstPaperQuestionsTemp = new List<CPaperQuestionParam>();
            MLstPaperQuestionsOld = new List<CPaperQuestionParam>();
            MLstChangePaperQuestion = new List<CPaperQuestionParam>();
            _mlstSearchCategoryNode = new List<CCategoryTree>();

            _mPageIndex = 0;
            _mPageCount = 0;
            _mPageSize = 200;
            _mQuestionNum = 0;
            _mMaxInfos = 100000;
            InitializeComponent();
            _mSearchQuestionParam = new CSearchQuestionParam();
            _mlstCategoryNum = new List<long>();
            CategoryTree.SelectedItemChanged += OrgCategoryTree_SelectedItemChanged;
            _mListPanels = new List<PanelItem>();
        }

        #region 初始化 & 全局消息
        protected override void Init()
        {
            try
            {
                PageName = "EditPaper";
                StylePath = "UMPS3602/MainPageStyle.xaml";
                base.Init();
                QIDocument.ItemsSource = _mObjQuestionInfo;
                PCDocument.ItemsSource = _mObjPaperQuestionInfos;
                ChbTofQuestion.IsChecked = true;
                ChbSingleQuestion.IsChecked = true;
                ChbMultipleQuestion.IsChecked = true;
                TbQuestionName.IsReadOnly = true;
                TbQuestionName.Background = Brushes.LightGray;
                InitPanels();
                CreateToolBarButtons();
                CurrentApp.SendLoadedMessage();
                TxtPage.KeyUp += TxtPage_KeyUp;
                CreatePageButtons();
                ChangeLanguage();
                GetPaperQustions();
            }
            catch (Exception ex)
            {
               ShowException(ex.Message);
            }
        }

        private void InitText()
        {
            PaperName.Text = string.Format("{0}: {1}", CurrentApp.GetLanguageInfo("3602T00038", "Paper Name:"),
                _mPaperInfo.PaperParam.StrName);
            ScoreName.Text = string.Format("{0}: {1}", CurrentApp.GetLanguageInfo("3602T00043", "Score:"),
                _mPaperInfo.PaperParam.IntScores);
            TestTimeName.Text = string.Format("{0}: {1}", CurrentApp.GetLanguageInfo("3602T00045", "Test Time:"),
                _mPaperInfo.PaperParam.IntTestTime);
            PassMarkName.Text = string.Format("{0}: {1}", CurrentApp.GetLanguageInfo("3602T00044", "Pass Mark:"),
                _mPaperInfo.PaperParam.IntPassMark);
            EditorName.Text = string.Format("{0}: {1}", CurrentApp.GetLanguageInfo("3602T00046", "Editor:"),
                _mPaperInfo.PaperParam.StrEditor);
            DateTimeName.Text = string.Format("{0}: {1}", CurrentApp.GetLanguageInfo("3602T00047", "DateTime:"),
                _mPaperInfo.PaperParam.StrDateTime);
            NoPassName.Text = string.Format("{0}: {1}", CurrentApp.GetLanguageInfo("3602T00049", "Audit:"),
                _mPaperInfo.PaperParam.IntAudit);
            UseName.Text = string.Format("{0}: {1}", CurrentApp.GetLanguageInfo("3602T00048", "Used:"),
                _mPaperInfo.PaperParam.IntUsed);
        }

        private void InitButton()
        {
            SavePaperBut.Children.Clear();
            Button btn;
            OperationInfo opt;
            btn = new Button();
            btn.Click += SavePaper_Click;
            opt = new OperationInfo();
            opt.Display = CurrentApp.GetLanguageInfo("3602T00084", "Save Paper");
            opt.Icon = "Images/add.png";
            btn.DataContext = opt;
            btn.SetResourceReference(StyleProperty, "OptButtonStyle");
            SavePaperBut.Children.Add(btn);

            BrowsePaperBut.Children.Clear();
            btn = new Button();
            btn.Click += BrowsePaper_Click;
            opt = new OperationInfo();
            opt.Display = CurrentApp.GetLanguageInfo("3602T00010", "Browse");
            opt.Icon = "Images/add.png";
            btn.DataContext = opt;
            btn.SetResourceReference(StyleProperty, "OptButtonStyle");
            BrowsePaperBut.Children.Add(btn);

            SearchQuestionBut.Children.Clear();
            btn = new Button();
            btn.Click += SearchPaper_Click;
            opt = new OperationInfo();
            opt.Display = CurrentApp.GetLanguageInfo("3602T00121", "Search Question");
            opt.Icon = "Images/add.png";
            btn.DataContext = opt;
            btn.SetResourceReference(StyleProperty, "OptButtonStyle");
            SearchQuestionBut.Children.Add(btn);
        }

        private void InitBasicSetingBut()
        {
            BasicSetBut.Children.Clear();
            ChangePropertis.Children.Clear();
            ButSearchCategroy.Children.Clear();
            OperationInfo opt;
            Button btn = new Button();
            btn.Click += GoToExamProductionPage_Click;
            opt = new OperationInfo();
            opt.Display = CurrentApp.GetLanguageInfo("3602T00088", "Go Back");
            opt.Icon = "Images/back.png";
            btn.DataContext = opt;
            btn.SetResourceReference(StyleProperty, "OptButtonStyle");
            BasicSetBut.Children.Add(btn);

            btn = new Button();
            btn.Click += SearchCategory_Click;
            opt = new OperationInfo();
            opt.Icon = "Images/search.png";
            TbSearchCategroy.Text = CurrentApp.GetLanguageInfo("3602T00153", "Search Category");
            btn.DataContext = opt;
            btn.SetResourceReference(StyleProperty, "OptButtonStyle");
            ButSearchCategroy.Children.Add(btn);

            btn = new Button();
            btn.Click += UpDatePaper_Click;
            opt = new OperationInfo();
            opt.Display = CurrentApp.GetLanguageInfo("3602T00008", "Change Paper Information");
            opt.Icon = "Images/add.png";
            btn.DataContext = opt;
            btn.SetResourceReference(StyleProperty, "OptButtonStyle");
            ChangePropertis.Children.Add(btn);
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

        private void InitQuestionInformationList()
        {
            try
            {
                string[] lans =
                    "3602T00016,3602T00017,3602T00018,3602T00019,3602T00020,3602T00021,3602T00022,3602T00023,3602T00024,3602T00025,3602T00026,3602T00026,3602T00026,3602T00026,3602T00026,3602T00026,3602T00027,3602T00028,3602T00029,3602T00030,3602T00031,3602T00032"
                        .Split(',');
                string[] cols =
                    "LongNum,StrCategoryName,StrType,StrQuestionsContect,StrAnswerOne,StrAnswerTwo,StrAnswerThree,StrAnswerFour,StrAnswerFive,StrAnswerSix,StrConOne,CorrectAnswerTwo,CorrectAnswerThree,CorrectAnswerFour,CorrectAnswerFive,CorrectAnswerSix, IntUseNumber, StrAccessoryType, StrAccessoryName, StrAccessoryPath, StrFounderName,StrDateTime"
                        .Split(',');
                int[] colwidths =
                {
                    150, 150, 60, 180, 80, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100,
                    100, 100, 100, 100, 100
                };
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
                QIDocument.View = columnGridView;
            }
            catch (Exception ex)
            {
               ShowException(ex.Message);
            }
        }

        private void InitPaperContentList()
        {
            try
            {
                string[] lans =
                    "3602T00051,3602T00052,3602T00053,3602T00035,3602T00054,3602T00020,3602T00021,3602T00022,3602T00023,3602T00024,3602T00025,3602T00026,3602T00026,3602T00026,3602T00026,3602T00026,3602T00026,3602T00055,3602T00028,3602T00029,3602T00030,3602T00017"
                        .Split(',');
                string[] cols =
                    "LongPaperNum,LongQuestionNum,StrQuestionType,StrQuestionsContect,StrEnableChange,StrAnswerOne,StrAnswerTwo,StrAnswerThree,StrAnswerFour,StrAnswerFive,StrAnswerSix,StrConOne,CorrectAnswerTwo,  CorrectAnswerThree,CorrectAnswerFour,CorrectAnswerFive,CorrectAnswerSix, IntScore, StrAccessoryType, StrAccessoryName, StrAccessoryPath, StrQuestionCategory"
                        .Split(',');
                int[] colwidths =
                {
                    150, 150, 60, 180, 80, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100,
                    100, 100, 100, 100, 100
                };
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
                PCDocument.View = columnGridView;
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
                panelItem.PanelId = S3602Consts.PANEL_ID_TREE;
                panelItem.Name = S3602Consts.PANEL_NAME_TREE;
                panelItem.ContentId = S3602Consts.PANEL_CONTENTID_TREE;
                panelItem.Title = CurrentApp.GetLanguageInfo("3602T00124", "Question Category");
                panelItem.Icon = "Images/00005.png";
                panelItem.IsVisible = true;
                panelItem.CanClose = true;
                _mListPanels.Add(panelItem);

                panelItem = new PanelItem();
                panelItem.PanelId = S3602Consts.PANEL_ID_SEARCHINFO;
                panelItem.Name = S3602Consts.PANEL_NAME_SEARCHINFO;
                panelItem.ContentId = S3602Consts.PANEL_CONTENTID_SEARCHINFO;
                panelItem.Title = CurrentApp.GetLanguageInfo("3602T00122", "Question Category");
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
            try
            {
                string uri = "/UMPS3602;component/Themes/Default/UMPS3602/FormStyle.xaml";
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
            InitBasicSetingBut();
            string strSql = string.Format("SELECT * FROM T_36_021_{0}", CurrentApp.Session.RentInfo.Token);
            InitCategoryTreeInfo(strSql);
            InitButton();
            InitQuestionInformationList();
            InitPaperContentList();
            InitText();
            PageName = "EditPaper";
            CurrentApp.AppTitle = CurrentApp.GetLanguageInfo("3602T00007", "Edit Paper");
            EditPaperStyleOpt.Header = CurrentApp.GetLanguageInfo("3602T00007", "Edit Paper");
            PaperPropertisStyleOpt.Header = CurrentApp.GetLanguageInfo("3602T00188", "Paper Propertis");
            BSExpandStyleOpt.Header = CurrentApp.GetLanguageInfo("3602T00002", "Basic Setting");
            PanelObjectTreeBox.Title = CurrentApp.GetLanguageInfo("3602T00017", "Category");
            QuestionInfoDocument.Title = CurrentApp.GetLanguageInfo("3602T00004", "Questions");
            PanelObjectPaperContent.Title = CurrentApp.GetLanguageInfo("3602T00005", "Questions");
            PanelSearchInfo.Title = CurrentApp.GetLanguageInfo("3602T00122", "Search Info");
            ChbQuestionName.Content = CurrentApp.GetLanguageInfo("3602T00123", "Questinon Content");
            ChbTofQuestion.Content = CurrentApp.GetLanguageInfo("3602T00098", "Ture Or False");
            ChbSingleQuestion.Content = CurrentApp.GetLanguageInfo("3602T00097", "Single");
            ChbMultipleQuestion.Content = CurrentApp.GetLanguageInfo("3602T00099", "Multiple");
            PanelObjectPaper.Title = CurrentApp.GetLanguageInfo("3602T00003", "Paper");
        }
        #endregion

        #region Add Test Node Tree
        public void InitCategoryTreeInfo(string strSql )
        {
            try
            {
                _mListCategoryTreeParam.Clear();
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3602Codes.OptGetQuestionCategory;
                Service36021Client client = new Service36021Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service36021"));
                //Service36021Client client = new Service36021Client();
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
                   ShowException(string.Format("{0}", webReturn.Message));
                    return;
                }

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
                       ShowException("Fail. queryItem is null");
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

        public void InitCategoryTree(List<CCategoryTree> listPapersCategoryParam, long longParentNodeId, CCategoryTree categoryNodes)
        {
            foreach (CCategoryTree param in listPapersCategoryParam)
            {
                if (param.LongParentNodeId == longParentNodeId)
                {
                    var nodeTemp = GetCategoryNodeInfo(categoryNodes, param);
                    InitCategoryTree(listPapersCategoryParam, param.LongNum, nodeTemp);
                }
            }
        }

        public CCategoryTree GetCategoryNodeInfo(CCategoryTree parentInfo, CCategoryTree param)
        {
            CCategoryTree temp = new CCategoryTree();
            try
            {
                temp.Icon = "/UMPS3602;component/Themes/Default/UMPS3602/Images/document.ico";
                temp.LongNum = param.LongNum;
                temp.StrName = param.LongParentNodeId == 0 ? CurrentApp.GetLanguageInfo("3602T00017", "Category") : param.StrName;
                temp.LongParentNodeId = param.LongParentNodeId;
                if (_mlstCategoryNum.Count <= 0)
                {
                    if (param.LongParentNodeId == 0) temp.IsExpanded = true;
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

        private void AddChildNode(CCategoryTree parentItem, CCategoryTree item)
        {
            Dispatcher.Invoke(new Action(() => parentItem.AddChild(item)));
        }

        #endregion

        #region Click
        private void SearchCategory_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(TxtSearchCategroy.Text)) return;
                string strSql = string.Format("SELECT * FROM T_36_021_{0} WHERE C002 like '%{1}%'", CurrentApp.Session.RentInfo.Token, TxtSearchCategroy.Text);
                SearchCategoryTreeInfo(strSql);
            }
            catch (Exception ex)
            {
               ShowException(ex.Message);
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

        void PcDocument_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            CPaperQuestionParam itame = null;
            try
            {
                QIDocument.Background = Brushes.LightGray;
                CategoryTree.Background = Brushes.LightGray;
                PCDocument.Background = Brushes.White;
                _mLstPaperQuestionsTemp.Clear();
                if (Keyboard.IsKeyDown(Key.RightCtrl) ||
                    Keyboard.IsKeyDown(Key.RightShift) ||
                    Keyboard.IsKeyDown(Key.LeftCtrl) ||
                    Keyboard.IsKeyDown(Key.LeftShift))
                {
                    int iCount = PCDocument.SelectedItems.Count;
                    for (int i = 0; i < iCount; i++)
                    {
                        itame = PCDocument.SelectedItems[i] as CPaperQuestionParam;
                        _mLstPaperQuestionsTemp.Add(itame);
                    }
                }
                else
                {
                    itame = PCDocument.SelectedItem as CPaperQuestionParam;
                    CPaperQuestionParam editPaperQuestion = itame;
                    _mLstPaperQuestionsTemp.Add(editPaperQuestion);
                }

                if (itame != null)
                {
                    AddQuestionBut.Children.Clear();
                    Button btn;
                    OperationInfo opt;
                    btn = new Button();
                    btn.Click += DeleteQuestions_Click;
                    opt = new OperationInfo();
                    opt.Display = CurrentApp.GetLanguageInfo("3602T00137", "Delete");
                    opt.Icon = "Images/Delete.png";
                    btn.DataContext = opt;
                    btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                    AddQuestionBut.Children.Add(btn);

                    UsableScore();
                }
            }
            catch (Exception ex)
            {
               ShowException(ex.Message);
            }
        }

        void PcDocument_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DeleteQuestion();
        }

        void QiDocument_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                QIDocument.Background = Brushes.White;
                CQuestionsParam itame = null; 
                CategoryTree.Background = Brushes.LightGray;
                PCDocument.Background = Brushes.LightGray;
                _mLstQuestionsTemp.Clear();
                S3602App.GLstQuestionInfos.Clear();
                if (Keyboard.IsKeyDown(Key.RightCtrl) ||
                    Keyboard.IsKeyDown(Key.RightShift) ||
                    Keyboard.IsKeyDown(Key.LeftCtrl) ||
                    Keyboard.IsKeyDown(Key.LeftShift))
                {
                    int iCount = QIDocument.SelectedItems.Count;
                    for (int i = 0; i < iCount; i++)
                    {
                        itame = QIDocument.SelectedItems[i] as CQuestionsParam;
                        _mLstQuestionsTemp.Add(itame);
                    }
                }
                else
                {
                    itame = QIDocument.SelectedItem as CQuestionsParam;
                    CQuestionsParam questionInfo = itame;
                    _mLstQuestionsTemp.Add(questionInfo);
                }
                
                S3602App.GLstQuestionInfos = _mLstQuestionsTemp;
                if (itame != null)
                {
                    AddQuestionBut.Children.Clear();
                    Button btn;
                    OperationInfo opt;
                    btn = new Button();
                    btn.Click += AddQuestions_Click;
                    opt = new OperationInfo();
                    opt.Display = CurrentApp.GetLanguageInfo("3602T00060", "Add TO Paper");
                    opt.Icon = "Images/add.png";
                    btn.DataContext = opt;
                    btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                    AddQuestionBut.Children.Add(btn);

                    UsableScore();
                }
            }
            catch (Exception ex)
            {
               ShowException(ex.Message);
            }
        }

        void QiDocument_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            AddQuestion();
        }

        private void GoToExamProductionPage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var btn = e.Source as Button;
                if (btn != null)
                {
                    if (_mbChangePaperProperties)
                    {
                        MessageBox.Show(string.Format("{0}",
                                CurrentApp.GetLanguageInfo("3602T00189", "Paper properties have been modified, please save the data again.")),
                                CurrentApp.GetLanguageInfo("3602T00090", "Warning"),
                                MessageBoxButton.OK);
                        return;
                    }

                    int iSroce = 0;
                    foreach (var paperQuestionParam in _mLstPaperQuestions)
                    {
                        iSroce += paperQuestionParam.IntScore;
                    }
                    if (iSroce < _mPaperInfo.PaperParam.IntScores)
                    {
                        MessageBoxResult result =
                            MessageBox.Show(string.Format("{0}\t\n ({3}: {4} > {1}: {2} )",
                                CurrentApp.GetLanguageInfo("3602T00091", "Integrity test whether to save enough"),
                                CurrentApp.GetLanguageInfo("3602T00144", "Question Score"), iSroce,
                                CurrentApp.GetLanguageInfo("3602T00145", "Paper Score"),
                                _mPaperInfo.PaperParam.IntScores),
                                CurrentApp.GetLanguageInfo("3602T00090", "Warning"),
                                MessageBoxButton.OKCancel);
                        if (result != MessageBoxResult.OK)
                            return;
                    }
                    if (iSroce > _mPaperInfo.PaperParam.IntScores)
                    {
                        MessageBoxResult result =
                            MessageBox.Show(string.Format("{0}\t\n ({3}: {4} < {1}: {2} )",
                                CurrentApp.GetLanguageInfo("3602T00091", "Integrity test whether to save enough"),
                                CurrentApp.GetLanguageInfo("3602T00144", "Question Score"), iSroce,
                                CurrentApp.GetLanguageInfo("3602T00145", "Paper Score"),
                                _mPaperInfo.PaperParam.IntScores),
                                CurrentApp.GetLanguageInfo("3602T00090", "Warning"),
                                MessageBoxButton.OKCancel);
                        if (result != MessageBoxResult.OK)
                            return;
                    }
                    if (!_mEnableBack)
                    {
                        MessageBoxResult result =
                            MessageBox.Show(CurrentApp.GetLanguageInfo("3602T00089", "Not save paper whether to quit!"),
                                CurrentApp.GetLanguageInfo("3602T00090", "Warning"),
                                MessageBoxButton.OKCancel);
                        if (result != MessageBoxResult.OK)
                            return;
                    }

                    CreateExamProductionView();
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void UpDatePaper_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                S3602App.GQueryModify = false;
                S3602App.GPaperInfo = new PaperInfo();
                S3602App.GPaperInfo = _mPaperInfo;
                S3602App.GPaperInfo.OptType = S3602Codes.OptUpdatePaper;
                AddPaperPage newPage = new AddPaperPage();
                newPage.ParentEditPage = this;
                newPage.CurrentApp = CurrentApp;
                PopupChangePaper.Content = newPage;
                PopupChangePaper.Title = CurrentApp.GetLanguageInfo("3602T00008", "Update Paper");
                PopupChangePaper.IsOpen = true;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }

        }


        private void AddQuestions_Click(object sender, RoutedEventArgs e)
        {
            AddQuestion();
        }

        private void DeleteQuestions_Click(object sender, RoutedEventArgs e)
        {
            DeleteQuestion();
        }

        private void SavePaper_Click(object sender, RoutedEventArgs e)
        {
            if (_mLstPaperQuestions.Count == 0)
            {
               ShowException(CurrentApp.GetLanguageInfo("3602T00085", "The paper did not add the title, unable to save"));
                return;
            }
            int iSroce = 0;
            foreach (var paperQuestionParam in _mLstPaperQuestions)
            {
                iSroce += paperQuestionParam.IntScore;
            }
            if (iSroce != _mPaperInfo.PaperParam.IntScores)
            {
                MessageBoxResult result =
                    MessageBox.Show(
                        string.Format("{0}\t\n ({3}: {4} || {1}: {2} )",
                            CurrentApp.GetLanguageInfo("3602T00086", "Integrity test whether to save enough"),
                            CurrentApp.GetLanguageInfo("3602T00144", "Question Score"), iSroce,
                            CurrentApp.GetLanguageInfo("3602T00145", "Paper Score"), _mPaperInfo.PaperParam.IntScores),
                        CurrentApp.GetLanguageInfo("3602T00090", "Warning"), MessageBoxButton.OKCancel);
                if (result != MessageBoxResult.OK)
                    return;             
            }
            iSroce = _mPaperInfo.PaperParam.IntScores - iSroce;

            List<CPaperQuestionParam> lstDelPaperQuestion = new List<CPaperQuestionParam>();
            List<CPaperQuestionParam> lstAddPaperQuestion = new List<CPaperQuestionParam>(); 
            foreach (var param in _mLstPaperQuestions)
            {
                CPaperQuestionParam temParam = MLstPaperQuestionsOld.FirstOrDefault(p => p.LongQuestionNum == param.LongQuestionNum);
                if (temParam == null)
                {
                    lstAddPaperQuestion.Add(param);
                }
            }

            foreach (var param in MLstPaperQuestionsOld)
            {
                CPaperQuestionParam temParam = _mLstPaperQuestions.FirstOrDefault(p => p.LongQuestionNum == param.LongQuestionNum);
                if (temParam == null)
                {
                    lstDelPaperQuestion.Add(param);
                }
            }
            bool bAdd = true;
            bool bDelete = true;
            if (lstAddPaperQuestion.Count > 0)
            {
                bAdd = WritePaperQuestion(lstAddPaperQuestion);
                if ( bAdd )
                {
                    foreach (var paperQuestion in lstAddPaperQuestion)
                    {
                        MLstPaperQuestionsOld.Add(paperQuestion);
                    }
                }
            }

            if (lstDelPaperQuestion.Count > 0)
            {
                bDelete = DeletePaperQuestion(lstDelPaperQuestion);
                if (bDelete)
                {
                    foreach (var paperQuestion in lstDelPaperQuestion)
                    {
                        MLstPaperQuestionsOld.Remove(paperQuestion);
                    }
                }
            }

            if (MLstChangePaperQuestion.Count > 0 && MLstChangePaperQuestion != null)
            {
                bAdd = ChangePaperQuestion(MLstChangePaperQuestion);
                if (bAdd)
                {
                    foreach (var paperQuestion in MLstChangePaperQuestion)
                    {
                        MLstPaperQuestionsOld.Add(paperQuestion);
                    }
                }
                MLstChangePaperQuestion = new List<CPaperQuestionParam>();
            }

            if (bAdd && bDelete)
            {
                ShowInformation(CurrentApp.GetLanguageInfo("3602T00087", "Success!"));
                _mEnableBack = true;
            }
            SetQuestionNum(iSroce);
            _mbChangePaperProperties = false;
        }

        private void BrowsePaper_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                PanelObjectPaper.IsSelected = true;
            }
            catch (Exception ex)
            {
               ShowException( ex.Message);
            }
        }

        private void SearchPaper_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                S3602App.GQueryModify = false;
                S3602App.GLstCCategoryTrees = _mListCategoryTreeParam;
                SearchQuestionPage newPage = new SearchQuestionPage();
                newPage.CategoryTree.ItemsSource = _mCategoryNode.Children;
                newPage.ParentPage = this;
                newPage.CurrentApp = CurrentApp;
                PopupPanelSearch.Content = newPage;
                PopupPanelSearch.Title = CurrentApp.GetLanguageInfo("3602T00121", "Search Question");
                PopupPanelSearch.IsOpen = true;
            }
            catch (Exception ex)
            {
               ShowException(ex.Message);
            }
        }

        private void PanelObjectPaper_IsSelectedChanged(object sender, EventArgs e)
        {
            try
            {
                if (!_mbPaperTable)
                {
                    S3602App.GPaperInfo.OptType = S3602Codes.MsgEditPaperPage;
                    S3602App.GlistPaperQuestionParam = new List<CPaperQuestionParam>();
                    List<CPaperQuestionParam> temp = new List<CPaperQuestionParam>();
                    foreach (var param in _mLstPaperQuestions)
                    {
                        temp.Add(param);
                    }
                    S3602App.GlistPaperQuestionParam = temp;

                    S3602App.GQueryModify = false;
                    TestPaperPage newPage = new TestPaperPage();
                    TestPaperPageName.Children.Add(newPage);
                    newPage.CurrentApp = CurrentApp;
                    _mbPaperTable = true;
                    _mbPaperContentTable = false;
                }
            }
            catch (Exception ex)
            {
               ShowException(ex.Message);
            }
        }

        private void PanelObjectPaperContent_IsSelectedChanged(object sender, EventArgs e)
        {
            if ( ! _mbPaperContentTable)
            {
                _mbPaperContentTable = true;
                _mbPaperTable = false;
            }
        }

        private void TbQuestionName_TextChanged(object sender, TextChangedEventArgs e)
        {
            _mLstQuestionsTemp.Clear();
            _mObjQuestionInfo.Clear();
            string strTemp = TbQuestionName.Text;
            if (string.IsNullOrEmpty(strTemp))
            {
                SetQuestionObservableCollection();
                return;
            }
            int index = 0;
            foreach (var param in _mLstQuestionInfos)
            {
                string strContect = param.StrQuestionsContect;
                index = strContect.IndexOf(strTemp);
                if (index != -1)
                {
                    _mObjQuestionInfo.Add(param);
                }
            }
        }

        private void CheckQuestion_Click(object sender, RoutedEventArgs e)
        {
            ChangeBoxCLick();
        }

        /// <summary>
        /// 单击树事件
        /// </summary>
        private void OrgCategoryTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                PCDocument.Background = Brushes.LightGray;
                QIDocument.Background = Brushes.LightGray;
                CategoryTree.Background = Brushes.White;

                CCategoryTree nodeInfo = CategoryTree.SelectedItem as CCategoryTree;
                if (nodeInfo == null) { return; }
                _mCategoryNodeTemp = nodeInfo;
                _mPageNum = null;
                _mLstQuestionInfos.Clear();
                _mQuestionNum = 0;
                _mPageIndex = 0;
                _mPageCount = 0;
                _mbListViewEnable = true;
                GetQuestionInfos();
                //ChangeListTableInfo();
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
            catch
            {
                // ignored
            }
        }

        #endregion

        #region Operations
        private string SetSql(string stNum)
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
                        strSql = string.Format("select TOP {0} X.* from T_36_022_{1} X where ", _mPageSize, CurrentApp.Session.RentInfo.Token);
                    }
                    else
                    {
                        strSql = string.Format("select TOP {0} X.* from T_36_022_{1} X where X.c001>'{2}' and ", _mPageSize, CurrentApp.Session.RentInfo.Token, _mPageNum);
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

            if (_mSearchQuestionParam.StrAccessoryType != null)
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
                                _mSearchQuestionParam.StrStartTime, _mSearchQuestionParam.StrEndTime,_mPageSize);
                    else
                        strTemp =
                            string.Format(
                                " and X.c025 between to_date('{0}','yyyy-mm-dd hh24:mi:ss')  and to_date('{1}','yyyy-mm-dd hh24:mi:ss') ORDER BY X.C001) WHERE ROWNUM <= {2}",
                                _mSearchQuestionParam.StrStartTime, _mSearchQuestionParam.StrEndTime, _mPageSize);
                    break;
            }
            return strSql += strTemp;
        }

        private void GetQuestionInfos()
        {
            try
            {
                string strSql = SetSql();
                var webRequest = new WebRequest();
                Service36021Client client = new Service36021Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service36021"));
                //var client = new Service36021Client();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3602Codes.OptGetQuestions;
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
                   ShowException(string.Format("{0}{1} ", CurrentApp.GetLanguageInfo("3602T00076", "Query the database to fail!"), webReturn.Message));
                    return;
                }

                if (webReturn.ListData.Count <= 0)
                {
                    if (_mbListViewEnable)
                    {
                        SetPageState();
                        SetQuestionObservableCollection();
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
                    bool bEnable = true;
                    foreach (var editPaper in _mLstPaperQuestions)
                    {
                        if (cExamQuestions.LongNum == editPaper.LongQuestionNum)
                        {
                            bEnable = false;
                        }
                    }

                    _mPageNum = cExamQuestions.LongNum.ToString();
                    if (bEnable)
                    {
                        int total = _mQuestionNum + 1;
                        if (total > _mMaxInfos)
                        {
                            ShowInformation(CurrentApp.GetLanguageInfo("3602T00147", string.Format("Larger than allowed max Questions, some Questions can't be displayed")));
                            return;
                        }
                        _mQuestionNum = total;
                        _mLstQuestionInfos.Add(cExamQuestions);
                        SetPageState();
                    }
                }
                //ChangeListTableInfo();
                
                if (_mbListViewEnable)
                {
                    if (_mLstQuestionInfos.Count >= _mPageSize)
                    {
                        SetQuestionObservableCollection();
                        _mbListViewEnable = false;
                    }    
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
                Service36021Client client = new Service36021Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service36021"));
                //var client = new Service36021Client();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3602Codes.OptGetQuestions;
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
                   ShowException(string.Format("{0}{1} ", CurrentApp.GetLanguageInfo("3602T00076", "Query the database to fail!"), webReturn.Message));
                    return;
                }

                if (webReturn.ListData.Count <= 0)
                {
                    if (_mbListViewEnable)
                    {
                        SetPageState();
                        SetQuestionObservableCollection();
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
                    bool bEnable = true;
                    foreach (var editPaper in _mLstPaperQuestions)
                    {
                        if (cExamQuestions.LongNum == editPaper.LongQuestionNum)
                        {
                            bEnable = false;
                        }
                    }

                    _mPageNum = cExamQuestions.LongNum.ToString();
                    if (bEnable)
                    {
                        int total = _mQuestionNum + 1;
                        if (total > _mMaxInfos)
                        {
                            ShowInformation(CurrentApp.GetLanguageInfo("3602T00147", string.Format("Larger than allowed max Questions, some Questions can't be displayed")));
                            return;
                        }
                        _mQuestionNum = total;
                        _mLstQuestionInfos.Add(cExamQuestions);
                        SetPageState();
                    }
                }
                //ChangeListTableInfo();

                if (_mbListViewEnable)
                {
                    if (_mLstQuestionInfos.Count >= _mPageSize)
                    {
                        SetQuestionObservableCollection();
                        _mbListViewEnable = false;
                    }
                }

                SearchQuestionInfos();
            }
            catch (Exception ex)
            {
               ShowException(ex.Message);
            }
        }

        private void GetPaperQustions()
        {
            try
            {
                _mLstPaperQuestions.Clear();
                MLstPaperQuestionsOld.Clear();
                var webRequest = new WebRequest();
                Service36021Client client = new Service36021Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service36021"));
                //var client = new Service36021Client();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3602Codes.OptGetPaperQuestions;
                OperationReturn optReturn = XMLHelper.SeriallizeObject(_mPaperInfo.PaperParam);
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
                   ShowException(string.Format("{0}{1} ", CurrentApp.GetLanguageInfo("3602T00076", "Query the database to fail!"), webReturn.Message));
                    return;
                }

                if (webReturn.ListData.Count <= 0) { return; }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    optReturn = new OperationReturn();
                    optReturn = XMLHelper.DeserializeObject<CPaperQuestionParam>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                       ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        continue;
                    }
                    var cEditPaper = optReturn.Data as CPaperQuestionParam;
                    if (cEditPaper == null)
                    {
                       ShowException(string.Format("Fail. filesItem is null"));
                        return;
                    }

                    _mLstPaperQuestions.Add(cEditPaper);
                    MLstPaperQuestionsOld.Add(cEditPaper);
                }
                SetPaperQuestionObservableCollection();
            }
            catch (Exception ex)
            {
               ShowException(ex.Message);
            }
        }

        private void UsableScore()
        {
            S3602App.GIntUsableScore = 0;
            int allScore = _mPaperInfo.PaperParam.IntScores;
            int intTemp = 0;
            if (_mObjPaperQuestionInfos.Count > 0)
                foreach (var cEditPaper in _mObjPaperQuestionInfos)
                {
                    intTemp += cEditPaper.IntScore;
                }
            S3602App.GIntUsableScore = allScore - intTemp;
        }

        public void UpdatePaperContentList()
        {
            foreach (var editPaper in S3602App.GlistPaperQuestionParam)
            {
                _mLstPaperQuestions.Add(editPaper);
            }
        }

        private void QuestionName_Click(object sender, RoutedEventArgs e)
        {
            if (ChbQuestionName.IsChecked == true)
            {
                TbQuestionName.IsReadOnly = false;
                ChbTofQuestion.IsChecked = true;
                ChbSingleQuestion.IsChecked = true;
                ChbMultipleQuestion.IsChecked = true;
                TbQuestionName.Background = Brushes.White;
            }
            else
            {
                TbQuestionName.IsReadOnly = true;
                TbQuestionName.Background = Brushes.LightGray;
            }
        }

        public void ChangeListTableInfo()
        {
            foreach (var editPaper in _mLstPaperQuestions)
            {
                _mLstQuestionInfos.Remove(_mLstQuestionInfos.FirstOrDefault(p => p.LongNum == editPaper.LongQuestionNum));
            }
            SetQuestionObservableCollection();
            SetPaperQuestionObservableCollection();
        }

        public void ChangeBoxCLick()
        {
            TbQuestionName.IsReadOnly = true;
            ChbQuestionName.IsChecked = false;
            TbQuestionName.Text = string.Empty;
            _mObjQuestionInfo.Clear();
            foreach (var param in _mLstQuestionInfos)
            {
                switch (param.IntType)
                {
                    case (int)QuestionType.TrueOrFalse:
                        if (ChbTofQuestion.IsChecked == true)
                            _mObjQuestionInfo.Add(param);
                        break;
                    case (int)QuestionType.SingleChoice:
                        if (ChbSingleQuestion.IsChecked == true)
                            _mObjQuestionInfo.Add(param);
                        break;
                    case (int)QuestionType.MultipleChioce:
                        if (ChbMultipleQuestion.IsChecked == true)
                            _mObjQuestionInfo.Add(param);
                        break;
                }
            }
        }

        private void QuestionListTable()
        {
            _mLstQuestionsTemp.Clear();
            foreach (var editPaperQuestion in _mLstPaperQuestionsTemp)
            {
                var questionsParam = new CQuestionsParam();
                questionsParam.LongNum = editPaperQuestion.LongQuestionNum;
                CQuestionsParam param = GetQuestionInfo(questionsParam);
                if (param != null)
                {
                    if (System.String.CompareOrdinal(_mCategoryNodeTemp.StrName, param.StrCategoryName) == 0)
                        _mLstQuestionsTemp.Add(param);
                }
            }
        }

        private bool DeletePaperQuestion(List<CPaperQuestionParam> lstDelPaperQuestion)
        {
            string strLog = string.Empty;
            var webRequest = new WebRequest();
            try
            {
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int) S3602Codes.OptDeletePaperQuestions;
                OperationReturn optReturn = XMLHelper.SeriallizeObject(lstDelPaperQuestion);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                }
                webRequest.ListData.Add(optReturn.Data.ToString());
                var client = new Service36021Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service36021"));
                //var client = new Service36021Client();
                WebReturn webReturn = client.UmpTaskOperation(webRequest);
                client.Close();
                CurrentApp.WriteLog(CurrentApp.GetLanguageInfo("3602T00137", "Delete"));
                if (!webReturn.Result)
                {
                    #region 写操作日志

                    strLog = string.Format("{0} {1} : {2}", Utils.FormatOptLogString("3602T00137"),
                        Utils.FormatOptLogString("3602T00166"), webReturn.Message);
                    CurrentApp.WriteOperationLog(S3602Consts.OPT_Delete.ToString(), ConstValue.OPT_RESULT_FAIL, strLog);

                    #endregion

                    CurrentApp.WriteLog(webReturn.Message);
                    ShowException(string.Format("{0} ", webReturn.Message));
                    return false;
                }

                if (webReturn.Message == S3602Consts.HadUse)
                {
                    #region 写操作日志

                    strLog = string.Format("{0} {1} : {2}", Utils.FormatOptLogString("3602T00137"),
                        Utils.FormatOptLogString("3602T00166"), CurrentApp.GetLanguageInfo("3602T00011", "Can't Delete"));
                    CurrentApp.WriteOperationLog(S3602Consts.OPT_Delete.ToString(), ConstValue.OPT_RESULT_FAIL, strLog);
                    CurrentApp.WriteLog(webReturn.Message);
                    #endregion
 
                    ShowInformation(CurrentApp.GetLanguageInfo("3602T00011", "Can't Delete"));
                    return false;
                }
                #region 写操作日志

                strLog = string.Format("{0} {1}", Utils.FormatOptLogString("3602T00137"),
                    Utils.FormatOptLogString("3602T00165"));
                CurrentApp.WriteOperationLog(S3602Consts.OPT_Delete.ToString(), ConstValue.OPT_RESULT_SUCCESS, strLog);
                CurrentApp.WriteLog(CurrentApp.GetLanguageInfo("3602T00165", "Delete Success"));
                #endregion
            }
            catch (Exception ex)
            {
                #region 写操作日志

                strLog = string.Format("{0} {1} : {2}", Utils.FormatOptLogString("3602T00137"),
                    Utils.FormatOptLogString("3602T00166"), ex.Message);
                CurrentApp.WriteOperationLog(S3602Consts.OPT_Delete.ToString(), ConstValue.OPT_RESULT_FAIL, strLog);

                #endregion

                CurrentApp.WriteLog(ex.Message);
                ShowInformation(ex.Message);
            }
            return true;
        }

        private bool WritePaperQuestion(List<CPaperQuestionParam> lstAddPaperQuestion)
        {
            string strLog = string.Empty;
            WebRequest webRequest = new WebRequest();
            WebReturn webReturn = new WebReturn();
            try
            {
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int) S3602Codes.OptAddPaperQuestions;
                OperationReturn optReturn = XMLHelper.SeriallizeObject(lstAddPaperQuestion);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return false;
                }
                webRequest.ListData.Add(optReturn.Data.ToString());
                var client = new Service36021Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service36021"));
                //var client = new Service36021Client();
                webReturn = client.UmpTaskOperation(webRequest);
                client.Close();
                CurrentApp.WriteLog(CurrentApp.GetLanguageInfo("3602T00084", "Save"));
                if (!webReturn.Result)
                {
                    #region 写操作日志

                    strLog = string.Format("{0} {1} : {2}", Utils.FormatOptLogString("3602T00084"),
                        Utils.FormatOptLogString("3602T00169"), webReturn.Message);
                    CurrentApp.WriteOperationLog(S3602Consts.OPT_Add.ToString(), ConstValue.OPT_RESULT_FAIL, strLog);

                    #endregion

                    CurrentApp.WriteLog(webReturn.Message);

                    ShowException(string.Format("{0} {1} ",
                        CurrentApp.GetLanguageInfo("3602T00066", "Insert data failed!"), webReturn.Message));
                    return false;
                }
                #region 写操作日志

                strLog = string.Format("{0} {1}", Utils.FormatOptLogString("3602T00084"),
                    Utils.FormatOptLogString("3602T00170"));
                CurrentApp.WriteOperationLog(S3602Consts.OPT_Add.ToString(), ConstValue.OPT_RESULT_SUCCESS, strLog);

                #endregion
                CurrentApp.WriteLog(CurrentApp.GetLanguageInfo("3602T00170", "Save Success"));
                return true;
            }
            catch (Exception ex)
            {
                #region 写操作日志

                strLog = string.Format("{0} {1} : {2}", Utils.FormatOptLogString("3602T00084"),
                    Utils.FormatOptLogString("3602T00169"), ex.Message);
                CurrentApp.WriteOperationLog(S3602Consts.OPT_Add.ToString(), ConstValue.OPT_RESULT_FAIL, strLog);

                #endregion

                CurrentApp.WriteLog(ex.Message);
                ShowInformation(ex.Message);
            }
            return false;
        }

        private bool ChangePaperQuestion(List<CPaperQuestionParam> lstChangePaperQuestion)
        {
            WebRequest webRequest = new WebRequest();
            WebReturn webReturn = new WebReturn();
            string strLog = string.Empty;
            try
            {
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int) S3602Codes.OptChangePaperQuestions;
                OperationReturn optReturn = XMLHelper.SeriallizeObject(lstChangePaperQuestion);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return false;
                }
                webRequest.ListData.Add(optReturn.Data.ToString());
                var client = new Service36021Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service36021"));
                //var client = new Service36021Client();
                webReturn = client.UmpTaskOperation(webRequest);
                client.Close();
                CurrentApp.WriteLog(CurrentApp.GetLanguageInfo("3602T00171", "Reset Question Value"));
                if (!webReturn.Result)
                {
                    #region 写操作日志

                    strLog = string.Format("{0} {1} : {2}", Utils.FormatOptLogString("3602T00171"),
                        Utils.FormatOptLogString("3602T00172"), webReturn.Message);
                    CurrentApp.WriteOperationLog(S3602Consts.OPT_Add.ToString(), ConstValue.OPT_RESULT_FAIL, strLog);

                    #endregion

                    CurrentApp.WriteLog(webReturn.Message);

                    ShowException(string.Format("{0}{1} ",
                        CurrentApp.GetLanguageInfo("3602T00066", "Insert data failed :"), webReturn.Message));
                    return false;
                }
                #region 写操作日志
                strLog = string.Format("{0} {1}", Utils.FormatOptLogString("3602T00171"),
                    Utils.FormatOptLogString("3602T00173"));
                CurrentApp.WriteOperationLog(S3602Consts.OPT_Change.ToString(), ConstValue.OPT_RESULT_SUCCESS, strLog);

                #endregion

                CurrentApp.WriteLog(CurrentApp.GetLanguageInfo("3602T00173", "Reset Success"));
                return true;
            }
            catch (Exception ex)
            {
                #region 写操作日志

                strLog = string.Format("{0} {1} : {2}", Utils.FormatOptLogString("3602T00171"),
                    Utils.FormatOptLogString("3602T00172"), ex.Message);
                CurrentApp.WriteOperationLog(S3602Consts.OPT_Add.ToString(), ConstValue.OPT_RESULT_FAIL, strLog);

                #endregion

                CurrentApp.WriteLog(ex.Message);
                ShowInformation(ex.Message);
            }
            return false;
        }

        private CQuestionsParam GetQuestionInfo(CQuestionsParam questionParam)
        {
            var webRequest = new WebRequest();
            try
            {
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3602Codes.OptQueryQuestions;
                OperationReturn optReturn = XMLHelper.SeriallizeObject(questionParam);
                if (!optReturn.Result)
                {
                   ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                }
                webRequest.ListData.Add(optReturn.Data.ToString());
                var client = new Service36021Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service36021"));
                //var client = new Service36021Client();
                WebReturn webReturn = client.UmpTaskOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    //CurrentApp.ShowExceptionMessage(string.Format("{0}", webReturn.Message));
                    return null;
                }
                var param = optReturn.Data as CQuestionsParam;
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    optReturn = new OperationReturn();
                    optReturn = XMLHelper.DeserializeObject<CQuestionsParam>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                       ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        continue;
                    }
                    param = optReturn.Data as CQuestionsParam;
                    if (param == null) return null;
                }
                return param;
            }
            catch (Exception ex)
            {
                ShowInformation(ex.Message);
            }
            return null;
        }

        public void SetBackValue()
        {
            _mEnableBack = false;
        }

        private void DeleteQuestion()
        {
            try
            {
                MessageBoxResult result =
                       MessageBox.Show(string.Format("{0}", CurrentApp.GetLanguageInfo("3602T00160", "Confirm delete Question?")),
                       CurrentApp.GetLanguageInfo("3602T00090", "Warning"),
                       MessageBoxButton.OKCancel);
                if (result != MessageBoxResult.OK)
                    return;

                QuestionListTable();
                foreach (var param in _mLstQuestionsTemp)
                {
                    _mLstQuestionInfos.Add(param);
                }
                foreach (var param in _mLstPaperQuestionsTemp)
                {
                    _mLstPaperQuestions.Remove(
                        _mLstPaperQuestions.FirstOrDefault(p => p.LongQuestionNum == param.LongQuestionNum));
                }
                _mPageNum = null;
                _mQuestionNum = 0;
                _mPageIndex = 0;
                _mPageCount = 0;
                for (int i = 0; i < _mLstQuestionInfos.Count; i++)
                {
                    int total = _mQuestionNum + 1;
                    if (total > _mMaxInfos)
                    {
                        ShowInformation(CurrentApp.GetLanguageInfo("3602T00147", string.Format("Larger than allowed max Questions, some Questions can't be displayed")));
                        return;
                    }
                    _mQuestionNum = total;
                    SetPageState();
                }
                SetQuestionObservableCollection();
                SetPaperQuestionObservableCollection();
            }
            catch (Exception ex)
            {
               ShowException(ex.Message);
            }
        }

        private void AddQuestion()
        {
            try
            {
                var item = QIDocument.SelectedItem as CQuestionsParam;
                if (item != null)
                {
                    S3602App.GQueryModify = false;
                    PaperAddQuestionPage newaPage = new PaperAddQuestionPage();
                    newaPage.ParentPage = this;
                    newaPage.CurrentApp = CurrentApp;
                    PopupPanelQuestionProperty.Content = newaPage;
                    PopupPanelQuestionProperty.Title = CurrentApp.GetLanguageInfo("3602T00139",
                        "Setting Question Property");
                    PopupPanelQuestionProperty.IsOpen = true;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        public void SearchQuestions( CSearchQuestionParam searchQuestionParam )
        {
            _mSearchQuestionParam = new CSearchQuestionParam();
            _mSearchQuestionParam = searchQuestionParam;
            _mPageNum = null;
            _mLstQuestionInfos.Clear();
            _mQuestionNum = 0;
            _mPageIndex = 0;
            _mPageCount = 0;
            _mbListViewEnable = true;

            SearchQuestionInfos();
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
                            case 0:
                                panel.Title = CurrentApp.GetLanguageInfo("3602T00122", item.Name);
                                break;
                            case 1:
                                panel.Title = CurrentApp.GetLanguageInfo("3602T00017", item.Name);
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

        private bool SetQuestionNum(int iScores)
        {
            string strLog = string.Empty;
            try
            {
                WebRequest webRequest = new WebRequest();
                //var client = new Service36021Client();
                Service36021Client client = new Service36021Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service36021"));
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int) S3602Codes.OptSetQuestionNum;
                _mPaperInfo.PaperParam.IntQuestionNum = _mObjPaperQuestionInfos.Count;
                _mPaperInfo.PaperParam.IntIntegrity = iScores == 0 ? 1 : 0;

                OperationReturn optReturn = XMLHelper.SeriallizeObject(_mPaperInfo.PaperParam);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return false;
                }
                webRequest.ListData.Add(optReturn.Data.ToString());
                WebReturn webReturn = client.UmpTaskOperation(webRequest);
                client.Close();
                CurrentApp.WriteLog(CurrentApp.GetLanguageInfo("3602T00174", "Set Question Num"));
                if (!webReturn.Result)
                {
                    #region 写操作日志

                    strLog = string.Format("{0} {1} : {2}", Utils.FormatOptLogString("3602T00174"),
                        Utils.FormatOptLogString("3602T00172"), webReturn.Message);
                    CurrentApp.WriteOperationLog(S3602Consts.OPT_Change.ToString(), ConstValue.OPT_RESULT_FAIL, strLog);

                    #endregion

                    CurrentApp.WriteLog(webReturn.Message);

                    ShowException(string.Format("{0}{1} ",
                        CurrentApp.GetLanguageInfo("3602T00083", "Failed to set datafor:"), webReturn.Message));
                    return false;
                }

                #region 写操作日志

                strLog = string.Format("{0} {1}", Utils.FormatOptLogString("3602T00174"),
                    Utils.FormatOptLogString("3602T00173"));
                CurrentApp.WriteOperationLog(S3602Consts.OPT_Change.ToString(), ConstValue.OPT_RESULT_SUCCESS, strLog);

                #endregion

                 CurrentApp.WriteLog(CurrentApp.GetLanguageInfo("3602T00173", "Set Success"));
            }
            catch (Exception ex)
            {
                #region 写操作日志

                strLog = string.Format("{0} {1} : {2}", Utils.FormatOptLogString("3602T00174"),
                    Utils.FormatOptLogString("3602T00172"), ex.Message);
                CurrentApp.WriteOperationLog(S3602Consts.OPT_Change.ToString(), ConstValue.OPT_RESULT_FAIL, strLog);

                #endregion

                CurrentApp.WriteLog(ex.Message);
                ShowException(ex.Message);
                return false;
            }
            return true;
        }

        private void SetQuestionObservableCollection()
        {
            _mObjQuestionInfo.Clear();
            int iCount = 0;
            foreach (var questionInfos in _mLstQuestionInfos)
            {
                if (iCount == _mPageSize)
                {
                    return;
                }
                string strTemp = String.Empty;
                switch ((QuestionType)questionInfos.IntType)
                {
                    case  QuestionType.TrueOrFalse:
                        strTemp = CurrentApp.GetLanguageInfo("3602T00098", "True Or False");
                        break;
                    case QuestionType.SingleChoice:
                        strTemp = CurrentApp.GetLanguageInfo("3602T00097", "Single Choice");
                        break;
                    case QuestionType.MultipleChioce:
                        strTemp = CurrentApp.GetLanguageInfo("3602T00099", "Multiple Chioce");
                        break;
                }
                questionInfos.StrType = strTemp;
                questionInfos.StrConOne = questionInfos.CorrectAnswerOne;
                if (questionInfos.CorrectAnswerOne == "F")
                {
                    questionInfos.StrConOne = CurrentApp.GetLanguageInfo("3602T00119", "F");
                }
                if (questionInfos.CorrectAnswerOne == "T")
                {
                    questionInfos.StrConOne = CurrentApp.GetLanguageInfo("3602T00118", "T");
                }
                _mObjQuestionInfo.Add(questionInfos);
                iCount++;
            }            
        }

        private void CreateExamProductionView()
        {
            try
            {
                S3602App.IsModifyScoreSheet = false;
                ExamProductionView newView = new ExamProductionView();
                newView.PageName = "ExamProduction";
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

        private void SetPaperQuestionObservableCollection()
        {
            _mObjPaperQuestionInfos.Clear();
            foreach (var paperQuestionInfo in _mLstPaperQuestions)
            {
                string strTemp = String.Empty;
                switch ((QuestionType)paperQuestionInfo.IntQuestionType)
                {
                    case QuestionType.TrueOrFalse:
                        strTemp = CurrentApp.GetLanguageInfo("3602T00098", "True Or False");
                        break;
                    case QuestionType.SingleChoice:
                        strTemp = CurrentApp.GetLanguageInfo("3602T00097", "Single Choice");
                        break;
                    case QuestionType.MultipleChioce:
                        strTemp = CurrentApp.GetLanguageInfo("3602T00099", "Multiple Chioce");
                        break;
                }
                paperQuestionInfo.StrEnableChange = paperQuestionInfo.EnableChange == 0
                    ? CurrentApp.GetLanguageInfo("3602T00119", "N")
                    : CurrentApp.GetLanguageInfo("3602T00118", "Y");
                paperQuestionInfo.StrConOne = paperQuestionInfo.CorrectAnswerOne;
                if (paperQuestionInfo.CorrectAnswerOne == "F")
                {
                    paperQuestionInfo.StrConOne = CurrentApp.GetLanguageInfo("3602T00119", "F");
                }
                if (paperQuestionInfo.CorrectAnswerOne == "T")
                {
                    paperQuestionInfo.StrConOne = CurrentApp.GetLanguageInfo("3602T00118", "T");
                }
                paperQuestionInfo.StrQuestionType = strTemp;
                _mObjPaperQuestionInfos.Add(paperQuestionInfo);
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
                _mObjQuestionInfo.Clear();
                int intStart = _mPageIndex * _mPageSize;
                int intEnd = (_mPageIndex + 1) * _mPageSize;
                for (int i = intStart; i < intEnd && i < _mQuestionNum; i++)
                {
                    CQuestionsParam questionsParam = _mLstQuestionInfos[i];

                    switch ((QuestionType)questionsParam.IntType)
                    {
                        case QuestionType.TrueOrFalse:
                            questionsParam.StrType = CurrentApp.GetLanguageInfo("3602T00098", "True Or False");
                            break;
                        case QuestionType.SingleChoice:
                            questionsParam.StrType = CurrentApp.GetLanguageInfo("3602T00097", "Single Choice");
                            break;
                        case QuestionType.MultipleChioce:
                            questionsParam.StrType = CurrentApp.GetLanguageInfo("3602T00099", "Multiple Chioce");
                            break;
                    }

                    _mObjQuestionInfo.Add(questionsParam);
                }
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
                webRequest.Code = (int)S3602Codes.OptGetQuestionCategory;
                Service36021Client client = new Service36021Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service36021"));
                //var client = new Service36021Client();
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

        public void ChangePaperInfo(PaperInfo paperInfo)
        {
            _mbChangePaperProperties = true;
            _mPaperInfo = paperInfo;
            PaperName.Text = string.Format("{0}: {1}", CurrentApp.GetLanguageInfo("3602T00038", "Paper Name:"),
                paperInfo.PaperParam.StrName);
            ScoreName.Text = string.Format("{0}: {1}", CurrentApp.GetLanguageInfo("3602T00043", "Score:"),
                paperInfo.PaperParam.IntScores);
            TestTimeName.Text = string.Format("{0}: {1}", CurrentApp.GetLanguageInfo("3602T00045", "Test Time:"),
                paperInfo.PaperParam.IntTestTime);
            PassMarkName.Text = string.Format("{0}: {1}", CurrentApp.GetLanguageInfo("3602T00044", "Pass Mark:"),
                paperInfo.PaperParam.IntPassMark);
            EditorName.Text = string.Format("{0}: {1}", CurrentApp.GetLanguageInfo("3602T00046", "Editor:"),
                paperInfo.PaperParam.StrEditor);
            DateTimeName.Text = string.Format("{0}: {1}", CurrentApp.GetLanguageInfo("3602T00047", "DateTime:"),
                paperInfo.PaperParam.StrDateTime);
            NoPassName.Text = string.Format("{0}: {1}", CurrentApp.GetLanguageInfo("3602T00049", "Audit:"),
                paperInfo.PaperParam.IntAudit);
            UseName.Text = string.Format("{0}: {1}", CurrentApp.GetLanguageInfo("3602T00048", "Used:"),
                paperInfo.PaperParam.IntUsed);
        }

        #endregion
    }
}
