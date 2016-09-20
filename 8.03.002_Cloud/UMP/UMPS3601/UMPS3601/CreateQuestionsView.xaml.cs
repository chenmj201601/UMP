using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UMPS3601.Models;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Common3601;
using System.Windows.Controls.Primitives;
using VoiceCyber.Wpf.AvalonDock.Layout;
using System.Windows.Forms;
using System.Windows.Media;
using UMPS3601.Wcf36011;

namespace UMPS3601
{
    /// <summary>
    /// CreateQuestionsPage.xaml 的交互逻辑
    /// </summary>
    public partial class CreateQuestionsView
    {
        private struct UpLoadFileInfo
        {
            public string StrAccessoryName;
            public string StrAccessoryPath;
            public string StrAccessoryType;
        }

        #region Members
        public ExamProductionView ParentPage;
        private BackgroundWorker _mWorker;
        private List<CCategoryTree> _mListCategoryTreeParam;
        private CCategoryTree _mCategoryNode;
        private CCategoryTree _mCategoryNodeTemp;
        private List<PanelItem> _mListPanels;
        private List<PanelItem> _mListQuestionBuns;
        private string _mBrowsePath = "c:\\";
        private string _mFilter = "All(*.*)|*.*|GIF image(*.gif)|*.gif|JPEG Image File(*.jpg)|*.jpg|JPEG Image File(*.jpeg)|*.jpeg|Portable Network Graphice(*.png)|*.png|Wav(*.wav)|*.wav|Pcm(*.pcm)|*.pcm";
        private string _mUmpFileRootPath;
        private static string _mUmpFileRootPathOld;
        private UpLoadFileInfo _mUpLoadFileInfo;
        private int _mIntQuestionType;
        private static QuestionInfo _mQuestionInfo;
        private ObservableCollection<CQuestionsParam> _mOcNewExamQuestions;
        private ObservableCollection<CQuestionsParam> _mOcAllExamQuestions;
        private List<CQuestionsParam> _mLstNewExamQuestions;
        private List<CQuestionsParam> _mLstAllExamQuestions;
        private List<CQuestionsParam> _mListQuestionInfosTemp;
        private CQuestionsParam _mCExamQuestionsTemp;
        private bool _mbEnableBrowse = false;
        private string _mUploadFilePath;
        private List<long> _mlstCategoryNum;
        private CQuestionsParam _mQuestionsSaveOld;
        private List<CCategoryTree> _mlstSearchCategoryNode;
        private CExcelHelper _mExcelHelper;
        List<CQuestionsParam> _mLstExcelInfo;
        List<CQuestionsParam> _mLstExcelInfoErr;

        private int _mPageIndex;//页的索引,这个是从0开始算的
        private int _mPageCount;
        private int _mPageSize;
        private int _mQuestionNum;
        private int _mMaxInfos;
        private string _mPageNum;
        private bool _mbListViewEnable;
        #endregion

        public CreateQuestionsView()
        {
            InitializeComponent();
            _mPageIndex = 0;
            _mPageCount = 0;
            _mPageSize = 200;
            _mQuestionNum = 0;
            _mMaxInfos = 100000;
            SCPanelLearnDocument.Hide();
            MCPanelLearnDocument.Hide();
            TOFPanelLearnDocument.Hide();
            BrowseQuestionDocument.Hide();
            _mListCategoryTreeParam = new List<CCategoryTree>();
            _mCategoryNode = new CCategoryTree();
            _mCategoryNodeTemp = new CCategoryTree();
            _mListPanels = new List<PanelItem>();
            _mListQuestionBuns = new List<PanelItem>();
            _mUmpFileRootPath = null;
            _mUmpFileRootPathOld = null;
            _mUploadFilePath = string.Empty;
            _mUpLoadFileInfo = new UpLoadFileInfo();
            _mIntQuestionType = 0;
            _mQuestionInfo = S3601App.GQuestionInfo;
            _mOcNewExamQuestions = new ObservableCollection<CQuestionsParam>();
            _mOcAllExamQuestions = new ObservableCollection<CQuestionsParam>();
            _mLstNewExamQuestions = new List<CQuestionsParam>();
            _mLstAllExamQuestions = new List<CQuestionsParam>();
            _mListQuestionInfosTemp = new List<CQuestionsParam>();
            _mCExamQuestionsTemp = new CQuestionsParam();
            _mlstCategoryNum = new List<long>();
            _mQuestionsSaveOld = new CQuestionsParam();
            _mlstSearchCategoryNode = new List<CCategoryTree>();
            _mExcelHelper = new CExcelHelper();
            _mLstExcelInfo = new List<CQuestionsParam>();
            _mLstExcelInfoErr = new List<CQuestionsParam>();
            CategoryTree.SelectedItemChanged += OrgCategoryTree_SelectedItemChanged;
        }

        #region 初始化 & 全局消息
        protected override void Init()
        {
            try
            {
                PageName = "CreateQuestion";
                StylePath = "UMPS3601/MainPageStyle.xaml";
                base.Init();

                InitBasicSetingBut();
                InitPanels();
                InitQuestionButs();
                GetUmpSetupPath();
                GetUpLoadFilePath();
                CreateToolBarButtons();
                CurrentApp.SendLoadedMessage();
                TxtPage.KeyUp += TxtPage_KeyUp;
                CreatePageButtons();
                ChangeLanguage();

                NewLqDocument.ItemsSource = _mOcNewExamQuestions;
                AllLqDocument.ItemsSource = _mOcAllExamQuestions;
                string strSql = string.Format("SELECT * FROM T_36_021_{0}", CurrentApp.Session.RentInfo.Token);
                switch (_mQuestionInfo.OptType)
                {
                    case S3601Codes.OperationUpdateQuestion:
                        _mUpLoadFileInfo.StrAccessoryName = _mQuestionInfo.QuestionsParam.StrAccessoryName;
                        _mUpLoadFileInfo.StrAccessoryPath = _mQuestionInfo.QuestionsParam.StrAccessoryPath;
                        _mUpLoadFileInfo.StrAccessoryType = _mQuestionInfo.QuestionsParam.StrAccessoryType;
                        //InitCQTButton((QuestionType)_mQuestionInfo.QuestionsParam.IntType);
                        InitQuestionButs(_mQuestionInfo.QuestionsParam.IntType);
                        _mlstCategoryNum = S3601App.GLstCategoryNum;
                        InitCategoryTreeInfo(strSql);
                        SetInterface(_mQuestionInfo.QuestionsParam.IntType);
                        break;
                    case S3601Codes.OperationCreateQuestion:
                        InitCategoryTreeInfo(strSql);
                        break;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitBasicSetingBut()
        {
            BasicSetBut.Children.Clear();
            ButSearchCategroy.Children.Clear();
            System.Windows.Controls.Button btn;
            OperationInfo opt;
            btn = new System.Windows.Controls.Button();
            btn.Click += GoToExamProductionPage_Click;
            opt = new OperationInfo();
            opt.Display = CurrentApp.GetLanguageInfo("3601T00088", "Go Back");
            opt.Icon = "Images/back.png";
            btn.DataContext = opt;
            btn.SetResourceReference(StyleProperty, "OptButtonStyle");
            BasicSetBut.Children.Add(btn);

            btn = new System.Windows.Controls.Button();
            btn.Click += SearchCategory_Click;
            opt = new OperationInfo();
            opt.Icon = "Images/search.png";
            TbSearchCategroy.Text = CurrentApp.GetLanguageInfo("3601T00154", "Search Category");
            btn.DataContext = opt;
            btn.SetResourceReference(StyleProperty, "OptButtonStyle");
            ButSearchCategroy.Children.Add(btn);
        }

        private void InitCQTButton()
        {
            CQTBut.Children.Clear();
            QIBut.Children.Clear();
            System.Windows.Controls.Button btn;
            OperationInfo opt;
            btn = new System.Windows.Controls.Button();
            btn.Click += CreateTrueOrFalse_Click;
            opt = new OperationInfo();
            opt.Display = CurrentApp.GetLanguageInfo("3601T00098", "True Or False");
            opt.Icon = "Images/add.png";
            btn.DataContext = opt;
            btn.SetResourceReference(StyleProperty, "OptButtonStyle");
            CQTBut.Children.Add(btn);

            btn = new System.Windows.Controls.Button();
            btn.Click += CreateSingleChoice_Click;
            opt = new OperationInfo();
            opt.Display = CurrentApp.GetLanguageInfo("3601T00097", "Single Choice");
            opt.Icon = "Images/add.png";
            btn.DataContext = opt;
            btn.SetResourceReference(StyleProperty, "OptButtonStyle");
            CQTBut.Children.Add(btn);

            btn = new System.Windows.Controls.Button();
            btn.Click += CreateMultipleChoice_Click;
            opt = new OperationInfo();
            opt.Display = CurrentApp.GetLanguageInfo("3601T00099", "Multiple Choice");
            opt.Icon = "Images/add.png";
            btn.DataContext = opt;
            btn.SetResourceReference(StyleProperty, "OptButtonStyle");
            CQTBut.Children.Add(btn);

            btn = new System.Windows.Controls.Button();
            btn.Click += CreateImportFile_Click;
            opt = new OperationInfo();
            opt.Display = CurrentApp.GetLanguageInfo("3601T00163", "Create Import File");
            opt.Icon = "Images/add.png";
            btn.DataContext = opt;
            btn.SetResourceReference(StyleProperty, "OptButtonStyle");
            QIBut.Children.Add(btn);

            btn = new System.Windows.Controls.Button();
            btn.Click += ImportFile_Click;
            opt = new OperationInfo();
            opt.Display = CurrentApp.GetLanguageInfo("3601T00164", "Import Question");
            opt.Icon = "Images/add.png";
            btn.DataContext = opt;
            btn.SetResourceReference(StyleProperty, "OptButtonStyle");
            QIBut.Children.Add(btn);
        }

        private void InitCQTButton(QuestionType s3601Codes)
        {
            CQTBut.Children.Clear();
            System.Windows.Controls.Button btn;
            OperationInfo opt;
            switch (s3601Codes)
            {
                case QuestionType.TrueOrFalse:
                    btn = new System.Windows.Controls.Button();
                    btn.Click += CreateTrueOrFalse_Click;
                    opt = new OperationInfo();
                    opt.Display = CurrentApp.GetLanguageInfo("3601T00098", "True Or False");
                    opt.ID = 31070021;
                    opt.Icon = "Images/add.png";
                    btn.DataContext = opt;
                    btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                    CQTBut.Children.Add(btn);
                    break;
                case QuestionType.SingleChoice:
                    btn = new System.Windows.Controls.Button();
                    btn.Click += CreateSingleChoice_Click;
                    opt = new OperationInfo();
                    opt.Display = CurrentApp.GetLanguageInfo("3601T00097", "Single Choice");
                    opt.ID = 31070021;
                    opt.Icon = "Images/add.png";
                    btn.DataContext = opt;
                    btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                    CQTBut.Children.Add(btn);
                    break;
                case QuestionType.MultipleChioce:
                    btn = new System.Windows.Controls.Button();
                    btn.Click += CreateMultipleChoice_Click;
                    opt = new OperationInfo();
                    opt.Display = CurrentApp.GetLanguageInfo("3601T00099", "Multiple Choice");
                    opt.ID = 31070021;
                    opt.Icon = "Images/add.png";
                    btn.DataContext = opt;
                    btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                    CQTBut.Children.Add(btn);
                    break;
                default:
                    InitCQTButton();
                    break;
            }  
        }

        private void InitQSButton()
        {
            QSBut.Children.Clear();
            System.Windows.Controls.Button btn;
            OperationInfo opt;

            btn = new System.Windows.Controls.Button();
            btn.Click += UpDateQuestion_Click;
            opt = new OperationInfo();
            opt.Display = CurrentApp.GetLanguageInfo("3601T00004", "Change Question");
            opt.Icon = "Images/add.png";
            btn.DataContext = opt;
            btn.SetResourceReference(StyleProperty, "OptButtonStyle");
            QSBut.Children.Add(btn);

            btn = new System.Windows.Controls.Button();
            btn.Click += DeleteQuestion_Click;
            opt = new OperationInfo();
            opt.Display = CurrentApp.GetLanguageInfo("3601T00005", "Delete Question");
            opt.Icon = "Images/add.png";
            btn.DataContext = opt;
            btn.SetResourceReference(StyleProperty, "OptButtonStyle");
            QSBut.Children.Add(btn);
        }

        private void InitQSButton2()
        {
            QSBut.Children.Clear();
            System.Windows.Controls.Button btn;
            OperationInfo opt;
            btn = new System.Windows.Controls.Button();
            btn.Click += ExamineQuestion_Click;
            opt = new OperationInfo();
            opt.Display = CurrentApp.GetLanguageInfo("3601T00007", "Browse");
            opt.ID = 31070021;
            opt.Icon = "Images/add.png";
            btn.DataContext = opt;
            btn.SetResourceReference(StyleProperty, "OptButtonStyle");
            QSBut.Children.Add(btn);

            btn = new System.Windows.Controls.Button();
            btn.Click += DeleteQuestion_Click;
            opt = new OperationInfo();
            opt.Display = CurrentApp.GetLanguageInfo("3601T00005", "Delete");
            opt.ID = 31070021;
            opt.Icon = "Images/add.png";
            btn.DataContext = opt;
            btn.SetResourceReference(StyleProperty, "OptButtonStyle");
            QSBut.Children.Add(btn);
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
                    var btn = new System.Windows.Controls.Button();
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

        void InitNewQuestionInformationList()
        {
            try
            {
                string[] lans = "3601T00016,3601T00017,3601T00018,3601T00019,3601T00020,3601T00021,3601T00022,3601T00023,3601T00024,3601T00025,3601T00026,3601T00026,3601T00026,3601T00026,3601T00026,3601T00026,3601T00027,3601T00028,3601T00029,3601T00030,3601T00031,3601T00032".Split(',');
                string[] cols = "LongNum,StrCategoryName,StrType,StrQuestionsContect,StrAnswerOne,StrAnswerTwo,StrAnswerThree,StrAnswerFour,StrAnswerFive,StrAnswerSix,StrConOne,CorrectAnswerTwo,  CorrectAnswerThree,CorrectAnswerFour,CorrectAnswerFive, CorrectAnswerSix, IntUseNumber, StrAccessoryType, StrAccessoryName, StrAccessoryPath, StrFounderName,StrDateTime".Split(',');
                int[] colwidths = { 100, 100, 60, 180, 80, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100 };
                GridView columnGridView = new GridView();
                GridViewColumn gvc;

                for (int i = 0; i < 22; i++)
                {
                    gvc = new GridViewColumn();
                    gvc.Header = CurrentApp.GetLanguageInfo(lans[i], cols[i]);
                    gvc.Width = colwidths[i];
                    gvc.DisplayMemberBinding = new System.Windows.Data.Binding(cols[i]);
                    columnGridView.Columns.Add(gvc);
                }
                NewLqDocument.View = columnGridView;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        void InitAllQuestionInformationList()
        {
            try
            {
                string[] lans = "3601T00016,3601T00017,3601T00018,3601T00019,3601T00020,3601T00021,3601T00022,3601T00023,3601T00024,3601T00025,3601T00026,3601T00026,3601T00026,3601T00026,3601T00026,3601T00026,3601T00027,3601T00028,3601T00029,3601T00030,3601T00031,3601T00032".Split(',');
                string[] cols = "LongNum,StrCategoryName,StrType,StrQuestionsContect,StrAnswerOne,StrAnswerTwo,StrAnswerThree,StrAnswerFour,StrAnswerFive,StrAnswerSix,StrConOne,CorrectAnswerTwo,  CorrectAnswerThree,CorrectAnswerFour,CorrectAnswerFive, CorrectAnswerSix, IntUseNumber, StrAccessoryType, StrAccessoryName, StrAccessoryPath, StrFounderName,StrDateTime".Split(',');
                int[] colwidths = { 100, 100, 60, 180, 80, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100 };
                GridView columnGridView = new GridView();
                GridViewColumn gvc;

                for (int i = 0; i < 22; i++)
                {
                    gvc = new GridViewColumn();
                    gvc.Header = CurrentApp.GetLanguageInfo(lans[i], cols[i]);
                    gvc.Width = colwidths[i];
                    gvc.DisplayMemberBinding = new System.Windows.Data.Binding(cols[i]);
                    columnGridView.Columns.Add(gvc);
                }
                AllLqDocument.View = columnGridView;
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
                PanelItem panelItem;
                panelItem = new PanelItem();
                panelItem.PanelId = S3601Consts.PANEL_ID_TREE;
                panelItem.Name = S3601Consts.PANEL_NAME_TREE;
                panelItem.ContentId = S3601Consts.PANEL_CONTENTID_TREE;
                panelItem.Title = CurrentApp.GetLanguageInfo("3601T00034", "Category");
                panelItem.Icon = "Images/00005.png";
                panelItem.IsVisible = true;
                panelItem.CanClose = true;
                _mListPanels.Add(panelItem);

                panelItem = new PanelItem();
                panelItem.PanelId = S3601Consts.PANEL_ID_NEWQUESTION;
                panelItem.Name = S3601Consts.PANEL_NAME_NEWQUESTION;
                panelItem.ContentId = S3601Consts.PANEL_CONTENTID_NEWQUESTION;
                panelItem.Title = CurrentApp.GetLanguageInfo("3601T00140", "Questions");
                panelItem.Icon = "Images/00004.png";
                panelItem.IsVisible = true;
                panelItem.CanClose = true;
                _mListPanels.Add(panelItem);

                panelItem = new PanelItem();
                panelItem.PanelId = S3601Consts.PANEL_ID_ALLQUESTION;
                panelItem.Name = S3601Consts.PANEL_NAME_ALLQUESTION;
                panelItem.ContentId = S3601Consts.PANEL_CONTENTID_ALLQUESTION;
                panelItem.Title = CurrentApp.GetLanguageInfo("3601T00141", "Questions");
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

        private void InitQuestionButs()
        {
            _mListQuestionBuns.Clear();
            PanelItem panelItem = new PanelItem();
            panelItem.PanelId = S3601Consts.BUT_ID_TRUNTOFALSE;
            panelItem.Name = S3601Consts.BUT_NAME_TRUNTOFALSE;
            panelItem.ContentId = S3601Consts.BUT_CONTENTID_TRUNTOFALSE;
            panelItem.Title = "TrueOrFalse";
            panelItem.Icon = "Images/00005.png";
            panelItem.IsVisible = true;
            panelItem.CanClose = true;
            _mListQuestionBuns.Add(panelItem);
        }

        private void InitQuestionButs( int iButType )
        {
            switch (iButType)
            {
                case (int)QuestionType.TrueOrFalse:
                    TrueOrFalseShow();
                    break;
                case (int)QuestionType.SingleChoice:
                    SingleChoiceShow();
                    break;
                case (int)QuestionType.MultipleChioce:
                    MultipleChoiceShow();
                    break;
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
                    //S3601App.ShowExceptionMessage("1" + ex.Message);
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
                    //S3601App.ShowExceptionMessage("2" + ex.Message);
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
            try
            {
                string uri = string.Format("/UMPS3601;component/Themes/Default/UMPS3601/FormStyle.xaml");
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
            InitNewQuestionInformationList();
            InitAllQuestionInformationList();

            PageName = "CreateQuestion";
            CurrentApp.AppTitle = CurrentApp.GetLanguageInfo("3601T00002", "Create Question");
            CQTExpandStyleOpt.Header = CurrentApp.GetLanguageInfo("3601T00109", "Choice Question Type");
            QSExpandStyleOpt.Header = CurrentApp.GetLanguageInfo("3601T00010", "Change Question");
            BSExpandStyleOpt.Header = CurrentApp.GetLanguageInfo("3601T00111", "Basic Setting");
            QIExpandStyleOpt.Header = CurrentApp.GetLanguageInfo("3601T00164", "Import Question");
            PanelObjectTreeBox.Title = CurrentApp.GetLanguageInfo("3601T00034", "Category");
            PanelObjectNewListQuestion.Title = CurrentApp.GetLanguageInfo("3601T00140", "New Question");
            PanelObjectAllListQuestion.Title = CurrentApp.GetLanguageInfo("3601T00141", "All Question");
            SCPanelLearnDocument.Title = CurrentApp.GetLanguageInfo("3601T00097", "Single Choice");
            MCPanelLearnDocument.Title = CurrentApp.GetLanguageInfo("3601T00099", "Multiple Choice");
            TOFPanelLearnDocument.Title = CurrentApp.GetLanguageInfo("3601T00098", "True Or False");
            lbSCAttachments.Content = CurrentApp.GetLanguageInfo("3601T00030", "Attachments path");
            lbMCAttachments.Content = CurrentApp.GetLanguageInfo("3601T00030", "Attachments path");
            lbTOFAttachments.Content = CurrentApp.GetLanguageInfo("3601T00030", "Attachments path");
            butSCBrowse.Content = CurrentApp.GetLanguageInfo("3601T00007", "Browse");
            butMCBrowse.Content = CurrentApp.GetLanguageInfo("3601T00007", "Browse");
            butTOFBrowse.Content = CurrentApp.GetLanguageInfo("3601T00007", "Browse");
            butSCSave.Content = CurrentApp.GetLanguageInfo("3601T00113", "Save");
            butMCSave.Content = CurrentApp.GetLanguageInfo("3601T00113", "Save");
            butTOFSave.Content = CurrentApp.GetLanguageInfo("3601T00113", "Save");
            lbSCQusetionConten.Content = CurrentApp.GetLanguageInfo("3601T00035", "QusetionConten:");
            lbMCQusetionConten.Content = CurrentApp.GetLanguageInfo("3601T00035", "QusetionConten:");
            lbTOFQusetionConten.Content = CurrentApp.GetLanguageInfo("3601T00035", "QusetionConten:");
            lbSCOptionAnswer.Content = CurrentApp.GetLanguageInfo("3601T00026", "Answer");
            lbMCOptionAnswer.Content = CurrentApp.GetLanguageInfo("3601T00026", "Answer");
            lbTOFOptionAnswer.Content = CurrentApp.GetLanguageInfo("3601T00026", "Answer");
            BrowseQuestionDocument.Title = CurrentApp.GetLanguageInfo("3601T00003", "Browse Question");
            rbutTOFTrue.Content = CurrentApp.GetLanguageInfo("3601T00064", "Yes");
            rbutTOFFalse.Content = CurrentApp.GetLanguageInfo("3601T00065", "No");

        }
        #endregion

        #region Add Test Node Tree
        public void InitCategoryTreeInfo( string strSql )
        {
            try
            {
                _mListCategoryTreeParam.Clear();
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3601Codes.OperationGetQuestionCategory;
                Service36011Client client = new Service36011Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service36011"));
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
            CCategoryTree nodeTemp = new CCategoryTree();
            foreach (CCategoryTree param in listPapersCategoryParam)
            {
                if (param.LongParentNodeId == longParentNodeId)
                {
                    nodeTemp = GetCategoryNodeInfo(categoryNodes, param);
                    InitCategoryTree(listPapersCategoryParam, param.LongNum, nodeTemp);
                }
            }
        }

        public CCategoryTree GetCategoryNodeInfo(CCategoryTree parentInfo, CCategoryTree param)
        {
            CCategoryTree temp = new CCategoryTree();
            try
            {
                temp.StrName = param.StrName;
                temp.Icon = "/UMPS3601;component/Themes/Default/UMPS3601/Images/document.ico";
                temp.LongNum = param.LongNum;
                temp.StrName = param.LongParentNodeId == 0 ? CurrentApp.GetLanguageInfo("3601T00017", "Category") : param.StrName;
                temp.LongParentNodeId = param.LongParentNodeId;
                if (_mlstCategoryNum.Count <=0)
                {
                    if (param.LongParentNodeId == 0) temp.IsExpanded = true;
                }
                else
                {
                    int iCount = 0;
                    foreach (var num in _mlstCategoryNum)
                    {
                        iCount ++;
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

        void CreateImportFile_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            try
            {
                saveFileDialog.FileName = string.Format("{0}.xls", DateTime.Now.ToString("yyyy-MM-dd"));
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filepath = saveFileDialog.FileName;
                    CreateExcelFile(filepath);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        void ImportFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            string strError = string.Empty;
            try
            {
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    SetBusy(true, CurrentApp.GetLanguageInfo("3601T00181", "Subject is being imported, please later..."));
                    this.IsEnabled = false;
                    string filepath = openFileDialog.FileName;
                    string[] strTemp = filepath.Split('.');
                    if (strTemp[1].CompareTo("xls") != 0)
                    {
                        MessageBoxResult result = System.Windows.MessageBox.Show(CurrentApp.GetLanguageInfo("3601T00196", "Import file error please select the file again."), CurrentApp.GetLanguageInfo("3601T00090", "Warning"), MessageBoxButton.OK);
                    }
                    else
                    {
                        if (ProcessImportFileInfo(filepath))
                        {
                            StartImportFile();
                            CExcelHelper excelHelper = new CExcelHelper();
                            if (excelHelper.SetExcelInfo(filepath, out strError, _mLstExcelInfoErr))
                            {
                                ShowInformation(string.Format("{0}\n{1} : {2}",
                                    CurrentApp.GetLanguageInfo("3601T00182", "Import is complete!"),
                                    CurrentApp.GetLanguageInfo("3601T00183", "Error NUm"), _mLstExcelInfoErr.Count));

                                #region 写操作日志
                                CurrentApp.WriteOperationLog(S3601Consts.OPT_Add.ToString(), ConstValue.OPT_RESULT_FAIL, string.Format("{0} {1} : {2}", Utils.FormatOptLogString("3601T00164"),
                                    Utils.FormatOptLogString("3601T00182"), string.Format("{0}\n{1} : {2}",
                                    CurrentApp.GetLanguageInfo("3601T00182", "Import is complete!"),
                                    CurrentApp.GetLanguageInfo("3601T00183", "Error NUm"), _mLstExcelInfoErr.Count)));
                                #endregion
                            }
                        }
                    }
                    
                    this.IsEnabled = true;
                    SetBusy(false, "...");
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
                this.IsEnabled = true;
                SetBusy(false, "...");
            }
        }

        void PageButton_Click(object sender, RoutedEventArgs e)//选择看第几页的按钮
        {
            var btn = e.Source as System.Windows.Controls.Button;
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

        void TxtPage_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
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

        void NewLqDocument_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                BrowseQuestionDocument.Show();
                _mListQuestionInfosTemp.Clear();
                _mCExamQuestionsTemp = new CQuestionsParam();
                CQuestionsParam itame = null;
                S3601App.GListQuestionInfos.Clear();
                if (Keyboard.IsKeyDown(Key.RightCtrl) ||
                    Keyboard.IsKeyDown(Key.RightShift) ||
                    Keyboard.IsKeyDown(Key.LeftCtrl) ||
                    Keyboard.IsKeyDown(Key.LeftShift))
                {
                    int iCount = NewLqDocument.SelectedItems.Count;
                    for (int i = 0; i < iCount; i++)
                    {
                        itame = NewLqDocument.SelectedItems[i] as CQuestionsParam;
                        _mListQuestionInfosTemp.Add(itame);
                    }
                }
                else
                {
                    itame = NewLqDocument.SelectedItem as CQuestionsParam;
                    _mCExamQuestionsTemp = itame;
                    _mListQuestionInfosTemp.Add(itame);
                }
                if (_mListQuestionInfosTemp.Count > 1)
                {
                    InitQSButton2();
                }
                else
                {
                    InitQSButton();
                }
                S3601App.GListQuestionInfos = _mListQuestionInfosTemp;
                if (_mCExamQuestionsTemp != null)
                {
                    switch (_mCExamQuestionsTemp.IntType)
                    {
                        case (int)QuestionType.TrueOrFalse:
                            InitRadioToF(_mCExamQuestionsTemp);
                            break;
                        case (int)QuestionType.SingleChoice:
                            InitRadioSingle(_mCExamQuestionsTemp);
                            break;
                        case (int)QuestionType.MultipleChioce:
                            InitRadioMultiple(_mCExamQuestionsTemp);
                            break;
                    }
                    BrowseQuestionDocument.IsSelected = true;
                }
                    
            }
            catch(Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        void AllLqDocument_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                BrowseQuestionDocument.Show();
                _mListQuestionInfosTemp.Clear();
                _mCExamQuestionsTemp = new CQuestionsParam();
                CQuestionsParam itame = null;
                S3601App.GListQuestionInfos.Clear();
                if (Keyboard.IsKeyDown(Key.RightCtrl) ||
                    Keyboard.IsKeyDown(Key.RightShift) ||
                    Keyboard.IsKeyDown(Key.LeftCtrl) ||
                    Keyboard.IsKeyDown(Key.LeftShift))
                {
                    int iCount = AllLqDocument.SelectedItems.Count;
                    for (int i = 0; i < iCount; i++)
                    {
                        itame = AllLqDocument.SelectedItems[i] as CQuestionsParam;
                        _mListQuestionInfosTemp.Add(itame);
                    }
                }
                else
                {
                    itame = AllLqDocument.SelectedItem as CQuestionsParam;
                    _mCExamQuestionsTemp = itame;
                    _mListQuestionInfosTemp.Add(itame);
                }
                
                if (_mListQuestionInfosTemp.Count > 1)
                {
                    InitQSButton2();
                }
                else
                {
                    InitQSButton();
                }
                S3601App.GListQuestionInfos = _mListQuestionInfosTemp;
                if (_mCExamQuestionsTemp != null)
                {
                    switch (Convert.ToInt16(_mCExamQuestionsTemp.IntType))
                    {
                        case (int)QuestionType.TrueOrFalse:
                            InitRadioToF(_mCExamQuestionsTemp);
                            break;
                        case (int)QuestionType.SingleChoice:
                            InitRadioSingle(_mCExamQuestionsTemp);
                            break;
                        case (int)QuestionType.MultipleChioce:
                            InitRadioMultiple(_mCExamQuestionsTemp);
                            break;
                    }
                    BrowseQuestionDocument.IsSelected = true;
                }

            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        void LqDocument_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            try
            {
                
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void GoToExamProductionPage_Click( object sender, RoutedEventArgs e )
        {
            var btn = e.Source as System.Windows.Controls.Button;
            if (btn != null)
            {
                ExamProductionView newView = new ExamProductionView();
                newView.PageName = "ExamProduction";
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
        }

        private void CreateTrueOrFalse_Click(object sender, RoutedEventArgs e)
        {
            if (!InspectSCQuestionPage())
            {
                MessageBoxResult result = System.Windows.MessageBox.Show(CurrentApp.GetLanguageInfo("3601T00162", "Question has not been saved save?"),
                    CurrentApp.GetLanguageInfo("3601T00090", "Warning"),
                    MessageBoxButton.OKCancel);
                if (result != MessageBoxResult.OK)
                    SCClear();
                else
                {
                    if (!SCSave())
                    {
                        return;
                    }
                }
            }

            if (!InspectMCQuestionPage())
            {
                MessageBoxResult result = System.Windows.MessageBox.Show(CurrentApp.GetLanguageInfo("3601T00162", "Question has not been saved save?"),
                    CurrentApp.GetLanguageInfo("3601T00090", "Warning"),
                    MessageBoxButton.OKCancel);
                if (result != MessageBoxResult.OK)
                    MCClear();
                else
                {
                    if (!MCSave())
                    {
                        return;
                    }
                }
            }
            
            TrueOrFalseShow();
        }

        private void CreateSingleChoice_Click(object sender, RoutedEventArgs e)
        {
            if (!InspectTOFQuestionPage())
            {
                MessageBoxResult result = System.Windows.MessageBox.Show(CurrentApp.GetLanguageInfo("3601T00162", "Question has not been saved save?"),
                    CurrentApp.GetLanguageInfo("3601T00090", "Warning"),
                    MessageBoxButton.OKCancel);
                if (result != MessageBoxResult.OK)
                    TOFClear();
                else
                {
                    if (!TOFSave())
                    {
                        return;
                    }
                }
            }    

            if (!InspectMCQuestionPage())
            {
                MessageBoxResult result = System.Windows.MessageBox.Show(CurrentApp.GetLanguageInfo("3601T00162", "Question has not been saved save?"),
                    CurrentApp.GetLanguageInfo("3601T00090", "Warning"),
                    MessageBoxButton.OKCancel);
                if (result != MessageBoxResult.OK)
                    MCClear();
                else
                {
                    if (!MCSave())
                    {
                        return;
                    }
                }
            }

            SingleChoiceShow();
        }

        private void CreateMultipleChoice_Click(object sender, RoutedEventArgs e)
        {
            if (!InspectSCQuestionPage())
            {
                MessageBoxResult result = System.Windows.MessageBox.Show(CurrentApp.GetLanguageInfo("3601T00162", "Question has not been saved save?"),
                    CurrentApp.GetLanguageInfo("3601T00090", "Warning"),
                    MessageBoxButton.OKCancel);
                if (result != MessageBoxResult.OK)
                    SCClear();
                else
                {
                    if (!SCSave())
                    {
                        return;
                    }
                }
            }

            if (!InspectTOFQuestionPage())
            {
                MessageBoxResult result = System.Windows.MessageBox.Show(CurrentApp.GetLanguageInfo("3601T00162", "Question has not been saved save?"),
                    CurrentApp.GetLanguageInfo("3601T00090", "Warning"),
                    MessageBoxButton.OKCancel);
                if (result != MessageBoxResult.OK)
                    TOFClear();
                else
                {
                    if (!TOFSave())
                    {
                        return;
                    }
                }
            }    

            MultipleChoiceShow();
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

        private void butBrowse_Click(object sender, EventArgs e)
        {
            _mUpLoadFileInfo = new UpLoadFileInfo();
            string strFileName = null;
            _mbEnableBrowse = false;
            string strTemp;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            try
            {
                openFileDialog.InitialDirectory = _mBrowsePath;
                openFileDialog.Filter = _mFilter;
                openFileDialog.RestoreDirectory = true;
                openFileDialog.FilterIndex = 1;
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    _mbEnableBrowse = true;
                    _mBrowsePath = null;
                    _mFilter = null;
                    strTemp = openFileDialog.FileName;
                    _mFilter = openFileDialog.Filter;
                    string[] strStr = strTemp.Split(new[] { '\\' });
                    int iSize = strStr.Length;
                    int iCount = 0;
                    foreach (string str in strStr)
                    {
                        _mBrowsePath += str + "\\";
                        iCount++;
                        if (iCount == iSize)
                        {
                            strFileName = str;
                        }
                    }
                    _mUmpFileRootPath = null;
                    _mUmpFileRootPath = _mUploadFilePath + "\\" + strFileName;
                    //_mUmpFileRootPath = _mUmpFileRootPathOld + "\\" + strFileName;
                    _mUpLoadFileInfo.StrAccessoryName = strFileName;
                    _mUpLoadFileInfo.StrAccessoryPath = _mUmpFileRootPath ;

                    if (strFileName != null) strStr = strFileName.Split(new[] { '.' });
                    iSize = strStr.Length;
                    iCount = 0;
                    foreach(string str in strStr )
                    {
                        iCount++;
                        if (iCount == iSize)
                        {
                            _mUpLoadFileInfo.StrAccessoryType = str;
                        }
                    }
                    switch (_mIntQuestionType)
                    {
                        case (int)QuestionType.TrueOrFalse:
                            TxtTOFAttachments.Text = strTemp;
                            break;
                        case (int)QuestionType.SingleChoice:
                            TxtSCAttachments.Text = strTemp;
                            break;
                        case (int)QuestionType.MultipleChioce:
                            TxtMCAttachments.Text = strTemp;
                            break;
                    }
                }
            }
            catch(Exception ex)
            {
                _mUpLoadFileInfo = new UpLoadFileInfo();
                _mUmpFileRootPath = null;
                ShowException(ex.Message);
            }
            
        }

        private void TOFClear()
        {
            TxtTOFQusetionConten.Clear();
            TxtTOFAttachments.Clear();
            rbutTOFTrue.IsChecked = false;
            rbutTOFFalse.IsChecked = false;
            _mbEnableBrowse = false; 
        }

        private bool TOFSave()
        {
            try
            {
                CQuestionsParam cExamQuestion = new CQuestionsParam();
                if (_mCategoryNodeTemp.LongNum == 0 && _mQuestionInfo.OptType == S3601Codes.OperationCreateQuestion)
                {
                    ShowException(CurrentApp.GetLanguageInfo("3601T00120", "Not for the category, unable to save the Question!"));
                    return false;
                }

                cExamQuestion.StrQuestionsContect = GetEditBoxContents_1024(TxtTOFQusetionConten.Text);
                if (cExamQuestion.StrQuestionsContect == null) return false;

                if (rbutTOFTrue.IsChecked == true || rbutTOFFalse.IsChecked == true)
                {
                    if (rbutTOFTrue.IsChecked == true)
                        cExamQuestion.CorrectAnswerOne = "T";
                    else
                        cExamQuestion.CorrectAnswerOne = "F";
                }
                else
                {
                    ShowInformation(CurrentApp.GetLanguageInfo("3601T00143", "Please set up a correct answer"));
                    return false;
                }
                if (_mbEnableBrowse)
                {
                    if (!string.IsNullOrWhiteSpace(TxtTOFAttachments.Text))
                    {
                        if (!UpLoadFiles(TxtTOFAttachments.Text))
                            return false;

                        if (!string.IsNullOrWhiteSpace(_mUpLoadFileInfo.StrAccessoryPath))
                        {
                            cExamQuestion.StrAccessoryPath = _mUpLoadFileInfo.StrAccessoryPath;
                            cExamQuestion.StrAccessoryName = _mUpLoadFileInfo.StrAccessoryName;
                            cExamQuestion.StrAccessoryType = _mUpLoadFileInfo.StrAccessoryType;
                        }
                        else
                        {
                            ShowInformation(CurrentApp.GetLanguageInfo("3601T00132", "UMP Path is null !"));
                            return false;
                        }
                    }
                }
                else
                {
                    cExamQuestion.StrAccessoryPath = _mUpLoadFileInfo.StrAccessoryPath;
                    cExamQuestion.StrAccessoryName = _mUpLoadFileInfo.StrAccessoryName;
                    cExamQuestion.StrAccessoryType = _mUpLoadFileInfo.StrAccessoryType;
                }

                cExamQuestion.LongCategoryNum = _mCategoryNodeTemp.LongNum;
                cExamQuestion.StrCategoryName = _mCategoryNodeTemp.StrName;
                cExamQuestion.IntType = (int)QuestionType.TrueOrFalse;
                if (!IsSameQuestion(cExamQuestion))
                {
                    ShowInformation(CurrentApp.GetLanguageInfo("3601T00155", "Questions have been saved!"));
                    return false;
                }
                switch (_mQuestionInfo.OptType)
                {
                    case S3601Codes.OperationUpdateQuestion:
                        if (UpDateDbOpt(cExamQuestion))
                        {
                            ShowInformation(CurrentApp.GetLanguageInfo("3601T00161", "Question change success!"));
                            TOFClear();
                            _mQuestionInfo.OptType = S3601Codes.OperationCreateQuestion;
                        }
                        break;
                    case S3601Codes.OperationCreateQuestion:
                        if (WriteDbOpt(cExamQuestion))
                        {
                            _mQuestionsSaveOld = new CQuestionsParam();
                            _mQuestionsSaveOld = cExamQuestion;
                            TOFClear();
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
                return false;
            }
            return true;
        }

        private void butTOFSave_Click(object sender, RoutedEventArgs e)
        {
            TOFSave();
        }

        private void SCClear()
        {
            TxtSCQusetionConten.Clear();
            TxtSCOptionA.Clear();
            TxtSCOptionB.Clear();
            TxtSCOptionC.Clear();
            TxtSCOptionD.Clear();
            TxtSCAttachments.Clear();
            rbutA.IsChecked = false;
            rbutB.IsChecked = false;
            rbutC.IsChecked = false;
            rbutD.IsChecked = false;
            _mbEnableBrowse = false;            
        }

        private bool SCSave()
        {
            try
            {
                CQuestionsParam cExamQuestion = new CQuestionsParam();
                if (_mCategoryNodeTemp.LongNum == 0 && _mQuestionInfo.OptType == S3601Codes.OperationCreateQuestion)
                {
                    ShowException(CurrentApp.GetLanguageInfo("3601T00120", "Not for the category, unable to save the Question!"));
                    return false;
                }

                cExamQuestion.StrQuestionsContect = GetEditBoxContents_1024(TxtSCQusetionConten.Text);
                if (cExamQuestion.StrQuestionsContect == null) return false;

                cExamQuestion.StrAnswerOne = GetEditBoxContents_200(TxtSCOptionA.Text, true);
                if (cExamQuestion.StrAnswerOne == null) return false;

                cExamQuestion.StrAnswerTwo = GetEditBoxContents_200(TxtSCOptionB.Text, true);
                if (cExamQuestion.StrAnswerTwo == null) return false;

                cExamQuestion.StrAnswerThree = GetEditBoxContents_200(TxtSCOptionC.Text, true);
                if (cExamQuestion.StrAnswerThree == null) return false;

                cExamQuestion.StrAnswerFour = GetEditBoxContents_200(TxtSCOptionD.Text, true);
                if (cExamQuestion.StrAnswerFour == null) return false;

                if (rbutA.IsChecked == true ||
                    rbutB.IsChecked == true ||
                    rbutC.IsChecked == true ||
                    rbutD.IsChecked == true)
                {
                    if (rbutA.IsChecked == true)
                        cExamQuestion.CorrectAnswerOne = "A";
                    else if (rbutB.IsChecked == true)
                        cExamQuestion.CorrectAnswerOne = "B";
                    else if (rbutC.IsChecked == true)
                        cExamQuestion.CorrectAnswerOne = "C";
                    else if (rbutD.IsChecked == true)
                        cExamQuestion.CorrectAnswerOne = "D";
                }
                else
                {
                    ShowInformation(CurrentApp.GetLanguageInfo("3601T00143", "Please set up a correct answer"));
                    return false;
                }

                cExamQuestion.LongCategoryNum = _mCategoryNodeTemp.LongNum;
                cExamQuestion.StrCategoryName = _mCategoryNodeTemp.StrName;
                cExamQuestion.IntType = (int)QuestionType.SingleChoice;

                if (_mbEnableBrowse)
                {
                    if (!string.IsNullOrWhiteSpace(TxtSCAttachments.Text))
                    {
                        if (!UpLoadFiles(TxtSCAttachments.Text))
                            return false;

                        if (!string.IsNullOrWhiteSpace(_mUpLoadFileInfo.StrAccessoryPath))
                        {
                            cExamQuestion.StrAccessoryPath = _mUpLoadFileInfo.StrAccessoryPath;
                            cExamQuestion.StrAccessoryName = _mUpLoadFileInfo.StrAccessoryName;
                            cExamQuestion.StrAccessoryType = _mUpLoadFileInfo.StrAccessoryType;
                        }
                        else
                        {
                            ShowInformation(CurrentApp.GetLanguageInfo("3601T00132", "UMP Path is null !"));
                            return false;
                        }
                    }
                }
                else
                {
                    cExamQuestion.StrAccessoryPath = _mUpLoadFileInfo.StrAccessoryPath;
                    cExamQuestion.StrAccessoryName = _mUpLoadFileInfo.StrAccessoryName;
                    cExamQuestion.StrAccessoryType = _mUpLoadFileInfo.StrAccessoryType;
                }

                if (!IsSameQuestion(cExamQuestion))
                {
                    ShowInformation(CurrentApp.GetLanguageInfo("3601T00155", "Questions have been saved!"));
                    return false;
                }
                switch (_mQuestionInfo.OptType)
                {
                    case S3601Codes.OperationUpdateQuestion:
                        if (UpDateDbOpt(cExamQuestion))
                        {
                            ShowInformation(CurrentApp.GetLanguageInfo("3601T00161", "Question change success!"));
                            SCClear();
                            _mQuestionInfo.OptType = S3601Codes.OperationCreateQuestion;
                        }
                        break;
                    case S3601Codes.OperationCreateQuestion:
                        if (WriteDbOpt(cExamQuestion))
                        {
                            _mQuestionsSaveOld = new CQuestionsParam();
                            _mQuestionsSaveOld = cExamQuestion;
                            SCClear();
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
                return false;
            }
            return true;
        }

        private void butSCSave_Click(object sender, RoutedEventArgs e)
        {
            SCSave();
        }

        private void MCClear()
        {
            TxtMCQusetionConten.Clear();
            TxtMCOptionA.Clear();
            TxtMCOptionB.Clear();
            TxtMCOptionC.Clear();
            TxtMCOptionD.Clear();
            TxtMCOptionE.Clear();
            TxtMCOptionF.Clear();
            TxtMCAttachments.Clear();
            checkBoxA.IsChecked = false;
            checkBoxB.IsChecked = false;
            checkBoxC.IsChecked = false;
            checkBoxD.IsChecked = false;
            checkBoxE.IsChecked = false;
            checkBoxF.IsChecked = false;
            _mbEnableBrowse = false;
        }

        private bool MCSave()
        {
            try
            {
                CQuestionsParam cExamQuestion = new CQuestionsParam();
                if (_mCategoryNodeTemp.LongNum == 0 && _mQuestionInfo.OptType == S3601Codes.OperationCreateQuestion)
                {
                    ShowException(CurrentApp.GetLanguageInfo("3601T00120", "Not for the category, unable to save the Question!"));
                    return false;
                }

                cExamQuestion.StrQuestionsContect = GetEditBoxContents_1024(TxtMCQusetionConten.Text);
                if (cExamQuestion.StrQuestionsContect == null) return false;

                cExamQuestion.StrAnswerOne = GetEditBoxContents_200(TxtMCOptionA.Text, true);
                if (cExamQuestion.StrAnswerOne == null) return false;

                cExamQuestion.StrAnswerTwo = GetEditBoxContents_200(TxtMCOptionB.Text, true);
                if (cExamQuestion.StrAnswerTwo == null) return false;

                cExamQuestion.StrAnswerThree = GetEditBoxContents_200(TxtMCOptionC.Text, true);
                if (cExamQuestion.StrAnswerThree == null) return false;

                cExamQuestion.StrAnswerFour = GetEditBoxContents_200(TxtMCOptionD.Text, true);
                if (cExamQuestion.StrAnswerFour == null) return false;

                cExamQuestion.StrAnswerFive = GetEditBoxContents_200(TxtMCOptionE.Text, false);
                cExamQuestion.StrAnswerSix = GetEditBoxContents_200(TxtMCOptionF.Text, false);

                if (checkBoxA.IsChecked == true ||
                    checkBoxB.IsChecked == true ||
                    checkBoxC.IsChecked == true ||
                    checkBoxD.IsChecked == true ||
                    checkBoxE.IsChecked == true ||
                    checkBoxF.IsChecked == true)
                {
                    if (checkBoxA.IsChecked == true)
                        cExamQuestion.CorrectAnswerOne = "A";
                    if (checkBoxB.IsChecked == true)
                        cExamQuestion.CorrectAnswerTwo = "B";
                    if (checkBoxC.IsChecked == true)
                        cExamQuestion.CorrectAnswerThree = "C";
                    if (checkBoxD.IsChecked == true)
                        cExamQuestion.CorrectAnswerFour = "D";
                    if (checkBoxE.IsChecked == true)
                        cExamQuestion.CorrectAnswerFive = "E";
                    if (checkBoxF.IsChecked == true)
                        cExamQuestion.CorrectAnswerSix = "F";
                }
                else
                {
                    ShowInformation(CurrentApp.GetLanguageInfo("3601T00143", "Please set up a correct answer"));
                    return false;
                }

                if (_mbEnableBrowse)
                {
                    if (!string.IsNullOrWhiteSpace(TxtMCAttachments.Text))
                    {
                        if (!UpLoadFiles(TxtMCAttachments.Text))
                            return false;

                        if (!string.IsNullOrWhiteSpace(_mUpLoadFileInfo.StrAccessoryPath))
                        {
                            cExamQuestion.StrAccessoryPath = _mUpLoadFileInfo.StrAccessoryPath;
                            cExamQuestion.StrAccessoryName = _mUpLoadFileInfo.StrAccessoryName;
                            cExamQuestion.StrAccessoryType = _mUpLoadFileInfo.StrAccessoryType;
                        }
                        else
                        {
                            ShowInformation(CurrentApp.GetLanguageInfo("3601T00132", "UMP Path is null !"));
                            return false;
                        }
                    }
                }
                else
                {
                    cExamQuestion.StrAccessoryPath = _mUpLoadFileInfo.StrAccessoryPath;
                    cExamQuestion.StrAccessoryName = _mUpLoadFileInfo.StrAccessoryName;
                    cExamQuestion.StrAccessoryType = _mUpLoadFileInfo.StrAccessoryType;
                }
                cExamQuestion.LongCategoryNum = _mCategoryNodeTemp.LongNum;
                cExamQuestion.StrCategoryName = _mCategoryNodeTemp.StrName;
                cExamQuestion.IntType = (int)QuestionType.MultipleChioce;

                if (!IsSameQuestion(cExamQuestion))
                {
                    ShowInformation(CurrentApp.GetLanguageInfo("3601T00155", "Questions have been saved!"));
                    return false;
                }

                switch (_mQuestionInfo.OptType)
                {
                    case S3601Codes.OperationUpdateQuestion:
                        if (UpDateDbOpt(cExamQuestion))
                        {
                            ShowInformation(CurrentApp.GetLanguageInfo("3601T00161", "Question change success!"));
                            MCClear();
                            _mQuestionInfo.OptType = S3601Codes.OperationCreateQuestion;
                        }
                        break;
                    case S3601Codes.OperationCreateQuestion:
                        if (WriteDbOpt(cExamQuestion))
                        {
                            _mQuestionsSaveOld = new CQuestionsParam();
                            _mQuestionsSaveOld = cExamQuestion;
                            MCClear();
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
                return false;
            }
            return true;
        }

        private void butMCSave_Click(object sender, RoutedEventArgs e)
        {
            MCSave();
        }

        private void UpDateQuestion_Click(object sender, RoutedEventArgs e)
        {
            var btn = e.Source as System.Windows.Controls.Button;
            if (btn != null)
            {
                _mbEnableBrowse = false;
                S3601App.GMessageSource = new S3601Codes();
                S3601App.GMessageSource = S3601Codes.MessageCreatequestionspage;
                _mQuestionInfo.QuestionsParam = _mCExamQuestionsTemp;
                _mQuestionInfo.OptType = S3601Codes.OperationUpdateQuestion;
                InitQuestionButs(_mQuestionInfo.QuestionsParam.IntType);
                SetInterface(_mQuestionInfo.QuestionsParam.IntType);        
            }
        }

        private void ExamineQuestion_Click(object sender, RoutedEventArgs e)
        {
            var btn = e.Source as System.Windows.Controls.Button;
            if (btn != null)
            {
                try
                {
                    S3601App.GQueryModify = false;
                    BrowseQustions newPage = new BrowseQustions();
                    newPage.CqParentPage = this;
                    PopupPanel.Content = newPage;
                    newPage.CurrentApp = CurrentApp;
                    S3601App.GMessageSource = new S3601Codes();
                    S3601App.GMessageSource = S3601Codes.MessageCreatequestionspage;
                    PopupPanel.IsOpen = true;
                }
                catch (Exception ex)
                {
                    ShowException(ex.Message);
                }
            }
        }

        private void DeleteQuestion_Click(object sender, RoutedEventArgs e)
        {
            int iNewCount = NewLqDocument.SelectedItems.Count;
            int iAllCount = AllLqDocument.SelectedItems.Count;
            if (iNewCount == 0 && iAllCount == 0)
                return;
            MessageBoxResult result = System.Windows.MessageBox.Show(CurrentApp.GetLanguageInfo("3601T00106", "Confirm delete Question?"),
                        CurrentApp.GetLanguageInfo("3601T00090", "Warning"),
                        MessageBoxButton.OKCancel);
            if (result != MessageBoxResult.OK)
                return;
            foreach (CQuestionsParam param in _mListQuestionInfosTemp)
            {
                if (param.IntUseNumber > 0)
                {
                    System.Windows.MessageBox.Show(CurrentApp.GetLanguageInfo("3601T00008", "No questions have been used to delete!"),
                        CurrentApp.GetLanguageInfo("3601T00090", "Warning"),
                        MessageBoxButton.OK);
                    return;
                }
            }

            if (DeleteQuestionInfos())
            {
                foreach( CQuestionsParam cExamQuestion in _mListQuestionInfosTemp )
                {
                    _mLstNewExamQuestions.Remove(cExamQuestion);
                    _mLstAllExamQuestions.Remove(cExamQuestion);
                    int total = _mQuestionNum - 1;
                    if (total > _mMaxInfos)
                    {
                        ShowInformation(CurrentApp.GetLanguageInfo("3601T00145", string.Format("Larger than allowed max Questions, some Questions can't be displayed")));
                    }
                    _mQuestionNum = total;
                    SetPageState();
                    _mCExamQuestionsTemp = new CQuestionsParam();
                }
                _mListQuestionInfosTemp.Clear();
            }
            SetAllObservableCollection();
            SetNewObservableCollection();
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
                InitCQTButton();
                _mCategoryNodeTemp = nodeInfo;
                _mPageNum = null;
                _mLstAllExamQuestions.Clear();
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
        #endregion

        #region Operations
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
                                panel.Title = CurrentApp.GetLanguageInfo("3601T00034", item.Name);
                                break;
                            case 2:
                                panel.Title = CurrentApp.GetLanguageInfo("3601T00140", item.Name);
                                break;
                            case 3:
                                panel.Title = CurrentApp.GetLanguageInfo("3601T00141", item.Name);
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
                    CToolButtonItem item = toggleBtn.DataContext as CToolButtonItem;
                    if (item == null) { continue; }
                    PanelItem panelItem = _mListPanels.FirstOrDefault(p => string.Format("TB{0}", p.Name) == item.Name);
                    if (panelItem != null)
                    {
                        toggleBtn.IsChecked = panelItem.IsVisible;
                    }
                }
            }
        }

        private bool UpDateDbOpt(CQuestionsParam cExamQuestion)
        {
            WebRequest webRequest;
            Service36011Client client;
            WebReturn webReturn;
            try
            {
                cExamQuestion.StrDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                cExamQuestion.LongNum = _mQuestionInfo.QuestionsParam.LongNum;
                cExamQuestion.LongCategoryNum = _mQuestionInfo.QuestionsParam.LongCategoryNum;
                cExamQuestion.StrCategoryName = _mQuestionInfo.QuestionsParam.StrCategoryName;
                webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3601Codes.OperationUpdateQuestion;
                OperationReturn optReturn = XMLHelper.SeriallizeObject(cExamQuestion);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return false;
                }
                webRequest.ListData.Add(optReturn.Data.ToString());
                client = new Service36011Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service36011"));
                //client = new Service36011Client();
                webReturn = client.UmpTaskOperation(webRequest);
                client.Close();
                CurrentApp.WriteLog(CurrentApp.GetLanguageInfo("3601T00004", "Update Question"));
                string strLog = string.Empty;
                if (!webReturn.Result)
                {
                    #region 写操作日志
                    strLog = string.Format("{0} {1} : {2}", Utils.FormatOptLogString("3601T00004"), Utils.FormatOptLogString("3601T00186"), webReturn.Message);
                    CurrentApp.WriteOperationLog(S3601Consts.OPT_Add.ToString(), ConstValue.OPT_RESULT_FAIL, strLog);
                    #endregion
                    CurrentApp.WriteLog(webReturn.Message);
                    ShowException(string.Format("{0}{1} ", CurrentApp.GetLanguageInfo("3601T00015", "Insert data failed"), webReturn.Message));
                    return false;
                }

                #region 写操作日志
                strLog = string.Format("{0} {1}", Utils.FormatOptLogString("3601T00004"), Utils.FormatOptLogString("3601T00187"));
                CurrentApp.WriteOperationLog(S3601Consts.OPT_Add.ToString(), ConstValue.OPT_RESULT_SUCCESS, strLog);
                #endregion
                CurrentApp.WriteLog(CurrentApp.GetLanguageInfo("3601T00187", "Update Success"));

                if (S3601App.GMessageSource == S3601Codes.MessageCreatequestionspage)
                {
                    _mLstNewExamQuestions.Remove(
                        _mLstNewExamQuestions.Where(p => p.LongNum == cExamQuestion.LongNum).FirstOrDefault());
                    _mLstAllExamQuestions.Remove(
                        _mLstAllExamQuestions.Where(p => p.LongNum == cExamQuestion.LongNum).FirstOrDefault());
                }
                _mLstNewExamQuestions.Insert(0,cExamQuestion);
                _mLstAllExamQuestions.Insert(0,cExamQuestion);
                _mCExamQuestionsTemp = cExamQuestion;
                SetNewObservableCollection();
                SetAllObservableCollection();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
                return false;
            }
            return true;
        }

        private bool WriteDbOpt( CQuestionsParam cExamQuestion)
        {
            WebRequest webRequest;
            Service36011Client client;
            WebReturn webReturn;
            try
            {
                if (!S3601App.GQueryModify)
                {
                    //生成新的查询配置表主键
                    webRequest = new WebRequest();
                    webRequest.Session = CurrentApp.Session;
                    webRequest.Code = (int)RequestCode.WSGetSerialID;
                    webRequest.ListData.Add("36");
                    webRequest.ListData.Add("3601");
                    webRequest.ListData.Add(DateTime.Now.ToString("yyyyMMddHHmmss"));
                    cExamQuestion.StrDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    client = new Service36011Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service36011"));
                    //client = new Service36011Client();
                    webReturn = client.UmpTaskOperation(webRequest);
                    client.Close();
                    if (!webReturn.Result)
                        return false;
                    string strNewResultID = webReturn.Data;
                    if (string.IsNullOrEmpty(strNewResultID))
                        return false;

                    cExamQuestion.LongNum = Convert.ToInt64(strNewResultID);
                }

                webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3601Codes.OperationAddQuestion;
                OperationReturn optReturn = XMLHelper.SeriallizeObject(cExamQuestion);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return false;
                }
                webRequest.ListData.Add(optReturn.Data.ToString());
                client = new Service36011Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service36011"));
                //client = new Service36011Client();
                webReturn = client.UmpTaskOperation(webRequest);
                client.Close();
                string strLog = string.Empty;
                CurrentApp.WriteLog(CurrentApp.GetLanguageInfo("3601T00002","Create Question"));
                if (!webReturn.Result)
                {
                    #region 写操作日志
                    strLog = string.Format("{0} {1} : {2}", Utils.FormatOptLogString("3601T00002"), Utils.FormatOptLogString("3601T00184"), webReturn.Message);
                    CurrentApp.WriteOperationLog(S3601Consts.OPT_Add.ToString(), ConstValue.OPT_RESULT_FAIL, strLog);
                    #endregion
                    CurrentApp.WriteLog(webReturn.Message);
                    ShowException(webReturn.Message);

                    return false;
                }
                else
                {
                    #region 写操作日志
                    strLog = string.Format("{0} {1} ", Utils.FormatOptLogString("3601T00002"), Utils.FormatOptLogString("3601T00185"));
                    CurrentApp.WriteOperationLog(S3601Consts.OPT_Add.ToString(), ConstValue.OPT_RESULT_SUCCESS, strLog);
                    #endregion
                    CurrentApp.WriteLog(CurrentApp.GetLanguageInfo("3601T00185", "Create Success"));
                    ///S3601App.ShowExceptionMessage(S3601App.GetLanguageInfo("3601T00001", "Save data Success"));
                    _mLstNewExamQuestions.Insert(0, cExamQuestion);
                    _mLstAllExamQuestions.Insert(0, cExamQuestion);
                    int total = _mQuestionNum + 1;
                    if (total > _mMaxInfos)
                    {
                        ShowInformation(CurrentApp.GetLanguageInfo("3601T00145", string.Format("Larger than allowed max Questions, some Questions can't be displayed")));
                    }
                    _mQuestionNum = total;
                    SetPageState();
                }
                SetNewObservableCollection();
                SetAllObservableCollection();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
                return false;
            }
            return true;
        }

        //获取UMP安装目录
        private void GetUmpSetupPath()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3601Codes.OperationGetUmpsetuppath;
                Service36011Client client = new Service36011Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service36011"));
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
                _mUmpFileRootPathOld = _mUmpFileRootPath;
                CurrentApp.WriteLog("UMPFolderPath", _mUmpFileRootPath);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void GetUpLoadFilePath()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3601Codes.OperationGetUploadFilePath;
                Service36011Client client = new Service36011Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service36011"));
                //var client = new Service36011Client();
                WebReturn webReturn = client.UmpTaskOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(webReturn.Message);
                    return;
                }
                if (string.IsNullOrWhiteSpace(webReturn.Data))
                {
                    ShowException(CurrentApp.GetLanguageInfo("3601T00144", "Path no configuration in the attachment, please configure accessory path!"));
                    return;
                }
                _mUploadFilePath = webReturn.Data;
                CurrentApp.WriteLog("UMPUploadFolderPath", _mUploadFilePath);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        //上传文件
        private bool UpLoadFiles(string localPath )
        {
            WebReturn webReturn = new WebReturn();
            UpRequest upRquest = new UpRequest();
            try
            {
                string serverPath = System.IO.Path.Combine(localPath, _mUmpFileRootPath);
                byte[] btFileArrayRead = System.IO.File.ReadAllBytes(localPath);
                List<byte> listFileByte = new List<byte>();
                Service36011Client client = new Service36011Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service36011"));
                //var client = new Service36011Client();
                int iCount = 0;
                foreach (byte tempByte in btFileArrayRead )
                {
                    listFileByte.Add(tempByte);
                    iCount += 1;
                    if (iCount == 1048576)
                    {
                        upRquest.SvPath = serverPath;
                        upRquest.ListByte = listFileByte.ToArray();
                        webReturn = client.UmpUpOperation(upRquest);
                        if (webReturn.Code == Defines.RET_FAIL)
                        {
                            ShowInformation(string.Format("{0}: {1}", webReturn.Message, serverPath));
                            return false;
                        }
                        listFileByte.Clear();
                        iCount = 0;
                    }
                }
                if (listFileByte.Count > 0)
                {
                    upRquest.SvPath = serverPath;
                    upRquest.ListByte = listFileByte.ToArray();
                    webReturn = client.UmpUpOperation(upRquest);
                    if (webReturn.Code == Defines.RET_FAIL)
                    {
                        ShowInformation(string.Format("{0}: {1}", webReturn.Message, serverPath));
                        return false;
                    }
                }
                CurrentApp.WriteLog("UpPath", serverPath);
            }
            catch (Exception ex)
            {
                //#region 写操作日志
                //string strLog = string.Format("{0} {1}{2}", CurrentApp.Session.UserID, Utils.FormatOptLogString("FO3106005"), _mBrowsePath);
                //CurrentApp.WriteOperationLog(S3601Consts.OPT_UpLoad.ToString(), ConstValue.OPT_RESULT_FAIL, strLog);
                //#endregion
                ShowException(ex.Message);
                return false;
            }
            return true;
        }

        private bool UpLoadFiles(string localPath, string umpFileRootPath)
        {
            WebReturn webReturn = new WebReturn();
            UpRequest upRquest = new UpRequest();
            try
            {
                string serverPath = System.IO.Path.Combine(localPath, umpFileRootPath);
                byte[] btFileArrayRead = System.IO.File.ReadAllBytes(localPath);
                List<byte> listFileByte = new List<byte>();
                Service36011Client client = new Service36011Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service36011"));
                //var client = new Service36011Client();
                int iCount = 0;
                foreach (byte tempByte in btFileArrayRead)
                {
                    listFileByte.Add(tempByte);
                    iCount += 1;
                    if (iCount == 1048576)
                    {
                        upRquest.SvPath = serverPath;
                        upRquest.ListByte = listFileByte.ToArray();
                        webReturn = client.UmpUpOperation(upRquest);
                        if (webReturn.Code == Defines.RET_FAIL)
                        {
                            //ShowInformation(string.Format("{0}: {1}", webReturn.Message, serverPath));
                            return false;
                        }
                        listFileByte.Clear();
                        iCount = 0;
                    }
                }
                if (listFileByte.Count > 0)
                {
                    upRquest.SvPath = serverPath;
                    upRquest.ListByte = listFileByte.ToArray();
                    webReturn = client.UmpUpOperation(upRquest);
                    if (webReturn.Code == Defines.RET_FAIL)
                    {
                        //ShowInformation(string.Format("{0}: {1}", webReturn.Message, serverPath));
                        return false;
                    }
                }
                CurrentApp.WriteLog("UpPath", serverPath);
            }
            catch (Exception ex)
            {
                //#region 写操作日志
                //string strLog = string.Format("{0} {1}{2}", CurrentApp.Session.UserID, Utils.FormatOptLogString("FO3106005"), _mBrowsePath);
                //CurrentApp.WriteOperationLog(S3601Consts.OPT_UpLoad.ToString(), ConstValue.OPT_RESULT_FAIL, strLog);
                //#endregion
                //ShowException(ex.Message);
                return false;
            }
            return true;
        }

        private string GetEditBoxContents_1024( string strStr)
        {
            string strTemp;
            if (!string.IsNullOrWhiteSpace(strStr))
            {
                if (strStr.Length > 1024)
                {
                    ShowInformation(CurrentApp.GetLanguageInfo("3601T00133", "Content beyond 1024"));
                    return null;
                }
            }
            else
            {
                ShowInformation(CurrentApp.GetLanguageInfo("3601T00131", "Problems can not be empty"));
                return null;
            }
            strTemp = strStr;
            return strTemp;
        }

        private string GetExcelContents_1024(string strStr)
        {
            string strTemp;
            if (!string.IsNullOrWhiteSpace(strStr))
            {
                if (strStr.Length > 1024)
                {
                    //ShowInformation(CurrentApp.GetLanguageInfo("3601T00133", "Content beyond 1024"));
                    return null;
                }
            }
            else
            {
               // ShowInformation(CurrentApp.GetLanguageInfo("3601T00131", "Problems can not be empty"));
                return null;
            }
            strTemp = strStr;
            return strTemp;
        }

        private string GetEditBoxContents_200(string strStr, bool bEnable)
        {
            string strTemp;
            if (!string.IsNullOrWhiteSpace(strStr))
            {
                if (strStr.Length > 200)
                {
                    ShowInformation(CurrentApp.GetLanguageInfo("3601T00134", "Content beyond 200"));
                    return null;
                }
            }
            else
            {
                if (bEnable)
                {
                    ShowInformation(CurrentApp.GetLanguageInfo("3601T00142", "A to D option can't be empty!"));
                }
                return null;
            }
            strTemp = strStr;
            return strTemp;
        }

        private string GetExcelContents_200(string strStr, bool bEnable)
        {
            string strTemp;
            if (!string.IsNullOrWhiteSpace(strStr))
            {
                if (strStr.Length > 200)
                {
                    //ShowInformation(CurrentApp.GetLanguageInfo("3601T00134", "Content beyond 200"));
                    return null;
                }
            }
            else
            {
                if (bEnable)
                {
                    //ShowInformation(CurrentApp.GetLanguageInfo("3601T00142", "A to D option can't be empty!"));
                }
                return null;
            }
            strTemp = strStr;
            return strTemp;
        }

        private void TrueOrFalseShow()
        {
            SCPanelLearnDocument.Hide();
            MCPanelLearnDocument.Hide();
            TOFPanelLearnDocument.Show();
            _mIntQuestionType = (int)QuestionType.TrueOrFalse;
            TOFPanelLearnDocument.IsSelected = true;
        }

        private void SingleChoiceShow()
        {
            SCPanelLearnDocument.Show();
            MCPanelLearnDocument.Hide();
            TOFPanelLearnDocument.Hide();
            _mIntQuestionType = (int)QuestionType.SingleChoice;
            SCPanelLearnDocument.IsSelected = true;
        }

        private void MultipleChoiceShow()
        {
            SCPanelLearnDocument.Hide();
            MCPanelLearnDocument.Show();
            TOFPanelLearnDocument.Hide();
            _mIntQuestionType = (int)QuestionType.MultipleChioce;
            MCPanelLearnDocument.IsSelected = true;
        }

        private void SetInterface(int interficeType)
        {
            switch (interficeType)
            {
                case (int)QuestionType.TrueOrFalse:
                    TxtTOFQusetionConten.Text = _mQuestionInfo.QuestionsParam.StrQuestionsContect;
                    TxtTOFAttachments.Text = _mQuestionInfo.QuestionsParam.StrAccessoryPath;
                    if (_mQuestionInfo.QuestionsParam.CorrectAnswerOne == "T")
                        rbutTOFTrue.IsChecked = true;
                    else
                        rbutTOFFalse.IsChecked = true;
                    TOFPanelLearnDocument.IsSelected = true;
                    break;
                case (int)QuestionType.SingleChoice:
                    TxtSCQusetionConten.Text = _mQuestionInfo.QuestionsParam.StrQuestionsContect;
                    TxtSCOptionA.Text = _mQuestionInfo.QuestionsParam.StrAnswerOne;
                    TxtSCOptionB.Text = _mQuestionInfo.QuestionsParam.StrAnswerTwo;
                    TxtSCOptionC.Text = _mQuestionInfo.QuestionsParam.StrAnswerThree;
                    TxtSCOptionD.Text = _mQuestionInfo.QuestionsParam.StrAnswerFour;
                    TxtSCAttachments.Text = _mQuestionInfo.QuestionsParam.StrAccessoryPath;
                    if (_mQuestionInfo.QuestionsParam.CorrectAnswerOne == "A")
                        rbutA .IsChecked = true;
                    else if (_mQuestionInfo.QuestionsParam.CorrectAnswerOne == "B")
                        rbutB.IsChecked = true;
                    else if (_mQuestionInfo.QuestionsParam.CorrectAnswerOne == "C")
                        rbutC.IsChecked = true;
                    else if (_mQuestionInfo.QuestionsParam.CorrectAnswerOne == "D")
                        rbutD.IsChecked = true;
                    SCPanelLearnDocument.IsSelected = true;
                    break;
                case (int)QuestionType.MultipleChioce:
                    TxtMCQusetionConten.Text = _mQuestionInfo.QuestionsParam.StrQuestionsContect;
                    TxtMCOptionA.Text = _mQuestionInfo.QuestionsParam.StrAnswerOne;
                    TxtMCOptionB.Text = _mQuestionInfo.QuestionsParam.StrAnswerTwo;
                    TxtMCOptionC.Text = _mQuestionInfo.QuestionsParam.StrAnswerThree;
                    TxtMCOptionD.Text = _mQuestionInfo.QuestionsParam.StrAnswerFour;
                    TxtMCOptionE.Text = _mQuestionInfo.QuestionsParam.StrAnswerFive;
                    TxtMCOptionF.Text = _mQuestionInfo.QuestionsParam.StrAnswerSix;
                    TxtMCAttachments.Text = _mQuestionInfo.QuestionsParam.StrAccessoryPath;
                    if (_mQuestionInfo.QuestionsParam.CorrectAnswerOne == "A")
                        checkBoxA.IsChecked = true;
                    if (_mQuestionInfo.QuestionsParam.CorrectAnswerTwo == "B")
                        checkBoxB.IsChecked = true;
                    if (_mQuestionInfo.QuestionsParam.CorrectAnswerThree == "C")
                        checkBoxC.IsChecked = true;
                    if (_mQuestionInfo.QuestionsParam.CorrectAnswerFour == "D")
                        checkBoxD.IsChecked = true;
                    if (_mQuestionInfo.QuestionsParam.CorrectAnswerFive == "E")
                        checkBoxE.IsChecked = true;
                    if (_mQuestionInfo.QuestionsParam.CorrectAnswerSix == "F")
                        checkBoxF.IsChecked = true;
                    MCPanelLearnDocument.IsSelected = true;
                    break;
            }
        }

        public void RefreshQuestion()
        {
            _mQuestionInfo = S3601App.GQuestionInfo;
            InitQuestionButs(_mQuestionInfo.QuestionsParam.IntType);
            SetInterface(_mQuestionInfo.QuestionsParam.IntType);
            S3601App.GQuestionInfo = new QuestionInfo();
        }

        private bool DeleteQuestionInfos()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3601Codes.OperationDeleteQuestion;
                
                OperationReturn optReturn = XMLHelper.SeriallizeObject(_mListQuestionInfosTemp);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return false;
                }
                webRequest.ListData.Add(optReturn.Data.ToString());
                Service36011Client client = new Service36011Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service36011"));
                //var client = new Service36011Client();
                WebReturn webReturn = client.UmpTaskOperation(webRequest);
                client.Close();
                string strLog = string.Empty;
                CurrentApp.WriteLog(CurrentApp.GetLanguageInfo("3601T00005", "Delete Question"));
                if (!webReturn.Result)
                {
                    #region 写操作日志
                    strLog = string.Format("{0} {1}{2}", Utils.FormatOptLogString("3601T00005"), Utils.FormatOptLogString("3601T00083"), webReturn.Message);
                    CurrentApp.WriteOperationLog(S3601Consts.OPT_Delete.ToString(), ConstValue.OPT_RESULT_FAIL, null);
                    CurrentApp.WriteLog(webReturn.Message);
                    #endregion
                    ShowException(string.Format("{0}{1} ", CurrentApp.GetLanguageInfo("3107T00092", "Delete Failed"), webReturn.Message));
                    
                    return false;
                }
                if (webReturn.Message == S3601Consts.HadUse)// 该查询条件被使用无法删除
                {
                    #region 写操作日志
                    strLog = string.Format("{0} {1}{2}", Utils.FormatOptLogString("3601T00005"), Utils.FormatOptLogString("3601T00083"), CurrentApp.GetLanguageInfo("3601T00011", "Can't Delete"));
                    CurrentApp.WriteOperationLog(S3601Consts.OPT_Delete.ToString(), ConstValue.OPT_RESULT_FAIL, null);
                    CurrentApp.WriteLog(CurrentApp.GetLanguageInfo("3601T00011", "Can't Delete"));
                    #endregion
                    ShowInformation(CurrentApp.GetLanguageInfo("3601T00011", "Can't Delete"));
                    return false;
                }
                CurrentApp.WriteLog(CurrentApp.GetLanguageInfo("3601T00012", "Delete Sucessed"));
                ShowInformation(CurrentApp.GetLanguageInfo("3601T00012", "Delete Sucessed"));
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

        private void InitRadioToF(CQuestionsParam cExamQuestion)
        {
            try
            {
                QuestionInfoGrid.Children.Clear();
                Grid grid = new Grid();
                TrueOrFlasePage newPage = new TrueOrFlasePage();
                ToFQuestionInfo tempParam = new ToFQuestionInfo();
                tempParam.IntNum = 0;
                tempParam.questionParam = cExamQuestion;
                newPage.SetQuestionInfo = tempParam;
                newPage.CurrentApp = this.CurrentApp;
                newPage.SetInformation();
                grid.Children.Add(newPage);
                Grid.SetRow(newPage, 0);
                QuestionInfoGrid.Children.Add(grid);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitRadioSingle(CQuestionsParam cExamQuestion)
        {
            try
            {
                QuestionInfoGrid.Children.Clear();
                Grid grid = new Grid();
                SingleChoicePage newPage = new SingleChoicePage();
                SCQuestionInfo tempParam = new SCQuestionInfo();
                tempParam.IntNum = 0;
                tempParam.questionParam = cExamQuestion;
                newPage.SetQuestionInfo = tempParam;
                newPage.CurrentApp = this.CurrentApp;
                newPage.SetInformation();
                grid.Children.Add(newPage);
                Grid.SetRow(newPage, 0);
                QuestionInfoGrid.Children.Add(grid);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            } 
        }

        private void InitRadioMultiple(CQuestionsParam cExamQuestion)
        {
            try
            {
                QuestionInfoGrid.Children.Clear();
                Grid grid = new Grid();
                MultipleChoicePage newPage = new MultipleChoicePage();
                MCQuestionInfo tempParam = new MCQuestionInfo();
                tempParam.IntNum = 0;
                tempParam.questionParam = cExamQuestion;
                newPage.SetQuestionInfo = tempParam;
                newPage.CurrentApp = this.CurrentApp;
                newPage.SetInformation();
                grid.Children.Add(newPage);
                Grid.SetRow(newPage, 0);
                QuestionInfoGrid.Children.Add(grid);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

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
                    ShowException(string.Format("{0}{1} ", CurrentApp.GetLanguageInfo("3601T00076", "Insert data failed"), webReturn.Message));
                    return;
                }

                if (webReturn.ListData.Count <= 0)
                {
                    if (_mbListViewEnable)
                    {
                        SetPageState();
                        SetAllObservableCollection();
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
                    _mLstAllExamQuestions.Add(cExamQuestions);
                    SetPageState();
                }
                if (_mbListViewEnable)
                {
                    SetAllObservableCollection();
                    _mbListViewEnable = false;
                }
                GetQuestionInfos();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void SetAllObservableCollection()
        {
            _mOcAllExamQuestions.Clear();
            foreach (var questionsParam in _mLstAllExamQuestions)
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
                _mOcAllExamQuestions.Add(questionsParam);
            }
        }

        private void SetNewObservableCollection()
        {
            _mOcNewExamQuestions.Clear();
            foreach (var questionsParam in _mLstNewExamQuestions)
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
                _mOcNewExamQuestions.Add(questionsParam);
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
                _mOcAllExamQuestions.Clear();
                int intStart = _mPageIndex * _mPageSize;
                int intEnd = (_mPageIndex + 1) * _mPageSize;
                for (int i = intStart; i < intEnd && i < _mQuestionNum; i++)
                {
                    CQuestionsParam questionsParam = _mLstAllExamQuestions[i];

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

                    _mOcAllExamQuestions.Add(questionsParam);
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

        private bool IsSameQuestion(CQuestionsParam cExamQuestion)
        {
            if (string.Equals(_mQuestionsSaveOld.StrQuestionsContect, cExamQuestion.StrQuestionsContect) &&
                string.Equals(_mQuestionsSaveOld.StrAnswerOne, cExamQuestion.StrAnswerOne) &&
                string.Equals(_mQuestionsSaveOld.StrAnswerTwo, cExamQuestion.StrAnswerTwo) &&
                string.Equals(_mQuestionsSaveOld.StrAnswerThree, cExamQuestion.StrAnswerThree) &&
                string.Equals(_mQuestionsSaveOld.StrAnswerFour, cExamQuestion.StrAnswerFour) &&
                string.Equals(_mQuestionsSaveOld.StrAnswerFive, cExamQuestion.StrAnswerFive) &&
                string.Equals(_mQuestionsSaveOld.StrAnswerSix, cExamQuestion.StrAnswerSix) &&
                string.Equals(_mQuestionsSaveOld.CorrectAnswerOne, cExamQuestion.CorrectAnswerOne) &&
                string.Equals(_mQuestionsSaveOld.CorrectAnswerTwo, cExamQuestion.CorrectAnswerTwo) &&
                string.Equals(_mQuestionsSaveOld.CorrectAnswerTwo, cExamQuestion.CorrectAnswerTwo) &&
                string.Equals(_mQuestionsSaveOld.CorrectAnswerThree, cExamQuestion.CorrectAnswerThree) &&
                string.Equals(_mQuestionsSaveOld.CorrectAnswerFour, cExamQuestion.CorrectAnswerFour) &&
                string.Equals(_mQuestionsSaveOld.CorrectAnswerFive, cExamQuestion.CorrectAnswerFive) &&
                string.Equals(_mQuestionsSaveOld.CorrectAnswerSix, cExamQuestion.CorrectAnswerSix) &&
                string.Equals(_mQuestionsSaveOld.StrAccessoryPath, cExamQuestion.StrAccessoryPath)
                )
            {
                return false;
            }
            return true;
        }

        private bool InspectTOFQuestionPage()
        {
            if (!string.IsNullOrEmpty(TxtTOFQusetionConten.Text) || !string.IsNullOrEmpty(TxtTOFAttachments.Text))
            {
                return false;
            }

            return true;
        }

        private bool InspectSCQuestionPage()
        {
            if (!string.IsNullOrEmpty(TxtSCQusetionConten.Text) || !string.IsNullOrEmpty(TxtSCAttachments.Text) ||
                !string.IsNullOrEmpty(TxtSCOptionA.Text) || !string.IsNullOrEmpty(TxtSCOptionB.Text) ||
                !string.IsNullOrEmpty(TxtSCOptionC.Text) || !string.IsNullOrEmpty(TxtSCOptionD.Text))
            {
                return false;
            }

            return true;
        }

        private bool InspectMCQuestionPage()
        {
            if (!string.IsNullOrEmpty(TxtMCQusetionConten.Text) || !string.IsNullOrEmpty(TxtMCAttachments.Text) ||
                !string.IsNullOrEmpty(TxtMCOptionA.Text) || !string.IsNullOrEmpty(TxtMCOptionB.Text) ||
                !string.IsNullOrEmpty(TxtMCOptionC.Text) || !string.IsNullOrEmpty(TxtMCOptionD.Text) ||
                !string.IsNullOrEmpty(TxtMCOptionE.Text) || !string.IsNullOrEmpty(TxtMCOptionF.Text))
            {
                return false;
            }

            return true;
        }

        private void CreateExcelFile(string strFilePath)
        {
            string strLog = string.Empty;
            try
            {
                CurrentApp.WriteLog(CurrentApp.GetLanguageInfo("3601T00163", "Create Excel"));

                CExcelHelper excelHelper = new CExcelHelper();
                object[,] data =
                {
                    {
                        string.Format("{0}\n{1}\n{2}\n{3}",
                            CurrentApp.GetLanguageInfo("3601T00165",
                                "Note: is not allowed to delete or modify the first and second row, third row start adding corresponding data!Category field can need not fill out!"),
                            CurrentApp.GetLanguageInfo("3601T00166",
                                "First, the types of questions please fill in the Numbers: 1. The true-false 2. Single topic selection 3. Multiple choice"),
                            CurrentApp.GetLanguageInfo("3601T00167",
                                "Second, the correct answer to fill in the format: 1. The true-false or F (T) 2. The single topic selection (A or B or C or D) 3. Multiple choice (A | B | | D | F C or B)"),
                                 CurrentApp.GetLanguageInfo("3601T00192",
                                "Third, , please fill out the import state N, when subject to import after successful import status automatically modified to Y. If the import status to fill in to Y questions will not be imported.")),
                        "", "", "", "", "", "", "", "", "", "", ""
                    },
                    {
                        string.Format("{0}", CurrentApp.GetLanguageInfo("3601T00179", "Import State")),
                        string.Format("{0}", CurrentApp.GetLanguageInfo("3601T00178", "Num")),
                        string.Format("{0}", CurrentApp.GetLanguageInfo("3601T00018", "Type")),
                        string.Format("{0}", CurrentApp.GetLanguageInfo("3601T00019", "Question")),
                        string.Format("{0}", CurrentApp.GetLanguageInfo("3601T00020", "AnswerOne")),
                        string.Format("{0}", CurrentApp.GetLanguageInfo("3601T00021", "AnswerTwo")),
                        string.Format("{0}", CurrentApp.GetLanguageInfo("3601T00022", "AnswerThree")),
                        string.Format("{0}", CurrentApp.GetLanguageInfo("3601T00023", "AnswerFour")),
                        string.Format("{0}", CurrentApp.GetLanguageInfo("3601T00024", "AnswerFive")),
                        string.Format("{0}", CurrentApp.GetLanguageInfo("3601T00025", "AnswerSix")),
                        string.Format("{0}", CurrentApp.GetLanguageInfo("3601T00026", "Answer")),
                        string.Format("{0}", CurrentApp.GetLanguageInfo("3601T00030", "AccessoryPath"))

                    },
                    {
                        "N", "1", "1|2|3",
                        string.Format("{0}",
                            CurrentApp.GetLanguageInfo("3601T00168", "Do not remove or modify this column!")),
                        "A", "B",
                        "C", "D", "E", "F", "A|B|C|D|E", ""
                    }
                };
                int[] columnWidth = {20, 20, 30, 30, 30, 30, 30, 30, 30, 30, 30, 30};
                string strError = string.Empty;
                excelHelper.CreateExcel(strFilePath, data, columnWidth, out strError);
                if (!string.IsNullOrEmpty(strError))
                {
                    ShowException(strError);
                }
                else
                {
                    ShowInformation(CurrentApp.GetLanguageInfo("3601T00169", "Create Success!"));
                }
            }
            catch (Exception ex)
            {
                #region 写操作日志
                strLog = string.Format("{0} {1} : {2}", Utils.FormatOptLogString("3601T00163"),
                    Utils.FormatOptLogString("3601T00190"), ex.Message);
                CurrentApp.WriteOperationLog(S3601Consts.OPT_Add.ToString(), ConstValue.OPT_RESULT_FAIL, strLog);
                #endregion
                CurrentApp.WriteLog(ex.Message);
                
                ShowException(ex.Message);
                return;
            }
            CurrentApp.WriteLog(CurrentApp.GetLanguageInfo("3601T00169", "Create Success"));
        }

        private bool ProcessImportFileInfo(string strFilePath)
        {
            string strError = string.Empty;
            CExcelHelper excelHelper = new CExcelHelper();
            excelHelper.ReadExcelFile(strFilePath, out strError);
            if (!string.IsNullOrEmpty(strError))
            {
                ShowException(strError);
                return false;
            }

            if (!excelHelper.CheckSerialNum())
            {
                ShowInformation(CurrentApp.GetLanguageInfo("3601T00180", "Import subject fixed repetition, please rearrange the serial number to do the import!"));
                return false;
            }

            List<string[]> lstImportInfo = excelHelper.GetImportInfo();
            if (lstImportInfo.Count <= 2)
            {
                ShowInformation(CurrentApp.GetLanguageInfo("3601T00171", "The document does not add the questions!"));
                return false;
            }

            int iCount = 0;
            _mLstExcelInfo = new List<CQuestionsParam>();
            _mLstExcelInfoErr = new List<CQuestionsParam>();
            
            foreach (var stringse in lstImportInfo)
            {
                bool bEnable = true;
                CQuestionsParam cExamQuestion = new CQuestionsParam();
                try
                {
                    if (iCount >= 2 && stringse[0] != "Y")
                    {
                        WebRequest webRequest;
                        Service36011Client client;
                        WebReturn webReturn;
                        if (!S3601App.GQueryModify)
                        {
                            //生成新的查询配置表主键
                            webRequest = new WebRequest();
                            webRequest.Session = CurrentApp.Session;
                            webRequest.Code = (int)RequestCode.WSGetSerialID;
                            webRequest.ListData.Add("36");
                            webRequest.ListData.Add("3601");
                            webRequest.ListData.Add(DateTime.Now.ToString("yyyyMMddHHmmss"));
                            cExamQuestion.StrDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            client = new Service36011Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service36011"));
                            //client = new Service36011Client();
                            webReturn = client.UmpTaskOperation(webRequest);
                            client.Close();
                            if (!webReturn.Result)
                                return false;
                            string strNewResultID = webReturn.Data;
                            if (string.IsNullOrEmpty(strNewResultID))
                                return false;
                            cExamQuestion.LongNum = Convert.ToInt64(strNewResultID);
                            cExamQuestion.IntExcelNum = Convert.ToInt32(stringse[1]);
                        }

                        cExamQuestion.StrQuestionsContect = GetExcelContents_1024(stringse[3]);
                        if (cExamQuestion.StrQuestionsContect == null) bEnable = false;
                        cExamQuestion.IntType = Convert.ToInt32(stringse[2].ToString());
                        switch (cExamQuestion.IntType)
                        {
                            case (int) QuestionType.SingleChoice:
                                cExamQuestion.StrAnswerOne = GetExcelContents_200(stringse[4], true);
                                if (cExamQuestion.StrAnswerOne == null) bEnable = false;
                                cExamQuestion.StrAnswerTwo = GetExcelContents_200(stringse[5], true);
                                if (cExamQuestion.StrAnswerTwo == null) bEnable = false;
                                cExamQuestion.StrAnswerThree = GetExcelContents_200(stringse[6], true);
                                if (cExamQuestion.StrAnswerThree == null) bEnable = false;
                                cExamQuestion.StrAnswerFour = GetExcelContents_200(stringse[7], true);
                                if (cExamQuestion.StrAnswerFour == null) bEnable = false;
                                if (stringse[10] == "A" || stringse[10] == "B" || stringse[10] == "C" || stringse[10] == "D")
                                {
                                    cExamQuestion.CorrectAnswerOne = stringse[10];
                                }
                                else
                                {
                                    bEnable = false;
                                }
                                break;
                            case (int) QuestionType.MultipleChioce:
                                cExamQuestion.StrAnswerOne = GetExcelContents_200(stringse[4], true);
                                if (cExamQuestion.StrAnswerOne == null) bEnable = false;
                                cExamQuestion.StrAnswerTwo = GetExcelContents_200(stringse[5], true);
                                if (cExamQuestion.StrAnswerTwo == null) bEnable = false;
                                cExamQuestion.StrAnswerThree = GetExcelContents_200(stringse[6], true);
                                if (cExamQuestion.StrAnswerThree == null) bEnable = false;
                                cExamQuestion.StrAnswerFour = GetExcelContents_200(stringse[7], true);
                                if (cExamQuestion.StrAnswerFour == null) bEnable = false;
                                cExamQuestion.StrAnswerFive = GetExcelContents_200(stringse[8], true);
                                if (cExamQuestion.StrAnswerFive == null) bEnable = false;
                                cExamQuestion.StrAnswerSix = GetExcelContents_200(stringse[9], true);
                                if (cExamQuestion.StrAnswerSix == null) bEnable = false;
                                string[] strAnswer = stringse[10].Split('|');
                                foreach (var s in strAnswer)
                                {
                                    switch (s)
                                    {
                                        case "A":
                                            cExamQuestion.CorrectAnswerOne = s;
                                            break;
                                        case "B":
                                            cExamQuestion.CorrectAnswerTwo = s;
                                            break;
                                        case "C":
                                            cExamQuestion.CorrectAnswerThree = s;
                                            break;
                                        case "D":
                                            cExamQuestion.CorrectAnswerFour = s;
                                            break;
                                        case "E":
                                            cExamQuestion.CorrectAnswerFive = s;
                                            break;
                                        case "F":
                                            cExamQuestion.CorrectAnswerSix = s;
                                            break;
                                        default:
                                            bEnable = false;
                                            break;
                                    }
                                }             
                                break;
                            case (int) QuestionType.TrueOrFalse:
                                if (stringse[10] == "T" || stringse[10] == "F")
                                {
                                    cExamQuestion.CorrectAnswerOne = stringse[10];
                                }
                                else
                                {
                                    bEnable = false;
                                }
                                break;
                            default:
                                bEnable = false;
                                break;
                        }
                        if (!string.IsNullOrEmpty(stringse[11]))
                        {
                            string[] strStr = stringse[11].Split(new[] { '\\' });
                            int iSize = strStr.Length;
                            if (iSize > 1)
                            {
                                string strFileName = strStr[iSize - 1];
                                if (strFileName != null)
                                {
                                    _mUmpFileRootPath = _mUploadFilePath + "\\" + strFileName;
                                    cExamQuestion.StrAccessoryName = strFileName;
                                    cExamQuestion.StrAccessoryPath = _mUmpFileRootPath;
                                    cExamQuestion.StrLoaclAccessoryPath = stringse[11];
                                    strStr = strFileName.Split(new[] { '.' });
                                    iSize = strStr.Length;
                                    if (iSize > 1)
                                    {
                                        cExamQuestion.StrAccessoryType = strStr[iSize - 1];
                                    }
                                    else
                                    {
                                        bEnable = false;
                                    }
                                }
                            }
                            else
                            {
                                bEnable = false;
                            }
                        }
                        cExamQuestion.LongCategoryNum = _mCategoryNodeTemp.LongNum;
                        cExamQuestion.StrCategoryName = _mCategoryNodeTemp.StrName;
                        
                        if (bEnable)
                        {
                            _mLstExcelInfo.Add(cExamQuestion);
                        }
                        else
                        {
                            _mLstExcelInfoErr.Add(cExamQuestion);
                        }  
                    }
                }
                catch (Exception ex)
                {
                    //ShowException( ex.Message);
                    _mLstExcelInfoErr.Add(cExamQuestion);
                   
                }
                iCount++;
                _mUmpFileRootPath = null;
            }
            return true;
        }

        private void StartImportFile()
        {
            if (_mLstExcelInfo.Count > 0)
            {
                int iCount = 0;
                List<CQuestionsParam> lstExcelInfo = new List<CQuestionsParam>();
                List<CQuestionsParam> lstExcelInfoErr = new List<CQuestionsParam>();
                foreach (var param in _mLstExcelInfo)
                {
                    if (!string.IsNullOrEmpty(param.StrLoaclAccessoryPath))
                    {
                        if (!UpLoadFiles(param.StrLoaclAccessoryPath, param.StrAccessoryPath))
                            lstExcelInfoErr.Add(param);
                        else
                        {
                            lstExcelInfo.Add(param);
                            iCount++;
                        }
                    }
                    else
                    {
                        lstExcelInfo.Add(param);
                        iCount++;
                    }
                    
                    if (iCount == 2)
                    {
                        if (ImportFile(lstExcelInfo))
                        {
                            lstExcelInfo = new List<CQuestionsParam>();
                            iCount = 0;
                        }
                        else
                        {
                            foreach (var info in lstExcelInfo)
                            {
                                lstExcelInfoErr.Add(info);
                            }
                        }
                    }
                }

                if (!ImportFile(lstExcelInfo))
                {
                    foreach (var info in lstExcelInfo)
                    {
                        lstExcelInfoErr.Add(info);
                    }
                }

                foreach (var param in lstExcelInfoErr)
                {
                    _mLstExcelInfoErr.Add(param);
                }
            }
        }

        private bool ImportFile(List<CQuestionsParam> lstExamQuestion)
        {
            string strLog = string.Empty;
            CurrentApp.WriteLog(CurrentApp.GetLanguageInfo("3601T00164", "Import Question"));
            WebRequest webRequest;
            Service36011Client client;
            WebReturn webReturn;
            try
            {
                webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int) S3601Codes.OperationImportExcelFile;
                OperationReturn optReturn = XMLHelper.SeriallizeObject(lstExamQuestion);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return false;
                }
                webRequest.ListData.Add(optReturn.Data.ToString());
                client = new Service36011Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service36011"));
                //client = new Service36011Client();
                webReturn = client.UmpTaskOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    #region 写操作日志

                    strLog = string.Format("{0} {1} : {2}", Utils.FormatOptLogString("3601T00164"),
                        Utils.FormatOptLogString("3601T00191"), webReturn.Message);
                    CurrentApp.WriteOperationLog(S3601Consts.OPT_Add.ToString(), ConstValue.OPT_RESULT_FAIL, strLog);

                    #endregion

                    CurrentApp.WriteLog(webReturn.Message);

                    ShowException(webReturn.Message);
                    return false;
                }
            }
            catch (Exception ex)
            {
                #region 写操作日志

                strLog = string.Format("{0} {1} : {2}", Utils.FormatOptLogString("3601T00164"),
                    Utils.FormatOptLogString("3601T00191"), ex.Message);
                CurrentApp.WriteOperationLog(S3601Consts.OPT_Add.ToString(), ConstValue.OPT_RESULT_FAIL, strLog);

                #endregion

                CurrentApp.WriteLog(ex.Message);

                ShowException(ex.Message);
                return false;
            }
            CurrentApp.WriteLog(CurrentApp.GetLanguageInfo("3601T00170", "Import Success"));
            return true;
        }

        #endregion
    }
}
