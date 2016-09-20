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
using VoiceCyber.UMP.Controls;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Common3601;
using System.Windows.Controls.Primitives;
using VoiceCyber.Wpf.AvalonDock.Layout;
using System.Windows.Forms;
using UMPS3601.Wcf36011;
using Binding = System.Windows.Data.Binding;

namespace UMPS3601
{
    /// <summary>
    /// CreateQuestionsPage.xaml 的交互逻辑
    /// </summary>
    public partial class CreateQuestionsPage
    {
        private struct UpLoadFileInfo
        {
            public string StrAccessoryName;
            public string StrAccessoryPath;
            public string StrAccessoryType;
        }

        #region Members
        public ExamProduction ParentPage;
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
        private ObservableCollection<CQuestionsParam> _mLstNewExamQuestions;
        private ObservableCollection<CQuestionsParam> _mLstAllExamQuestions;
        private List<CQuestionsParam> _mListQuestionInfos;
        private CQuestionsParam _mCExamQuestionsTemp;
        private bool _mbEnableBrowse = false;
        private string _mUploadFilePath;
        #endregion

        public CreateQuestionsPage()
        {
            InitializeComponent();
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
            _mQuestionInfo = App.GQuestionInfo;
            _mLstNewExamQuestions = new ObservableCollection<CQuestionsParam>();
            _mLstAllExamQuestions = new ObservableCollection<CQuestionsParam>();
            _mListQuestionInfos = new List<CQuestionsParam>();
            _mCExamQuestionsTemp = new CQuestionsParam();
            CategoryTree.SelectedItemChanged += OrgCategoryTree_SelectedItemChanged;
        }

        #region 初始化 & 全局消息
        protected override void Init()
        {
            try
            {
                PageHead.AppName = UMPApp.GetLanguageInfo("3601T00002", "Create Question");
                StylePath = "UMPS3601/MainPageStyle.xmal";
                base.Init();

                InitBasicSetingBut();
                InitPanels();
                InitQuestionButs();
                GetUmpSetupPath();
                GetUpLoadFilePath();
                CreateToolBarButtons();
                SendLoadedMessage();
                ChangeTheme();
                ChangeLanguage();

                NewLqDocument.ItemsSource = _mLstNewExamQuestions;
                AllLqDocument.ItemsSource = _mLstAllExamQuestions;

                switch (_mQuestionInfo.OptType)
                {
                    case S3601Codes.OperationUpdateQuestion:
                        _mUpLoadFileInfo.StrAccessoryName = _mQuestionInfo.QuestionsParam.StrAccessoryName;
                        _mUpLoadFileInfo.StrAccessoryPath = _mQuestionInfo.QuestionsParam.StrAccessoryPath;
                        _mUpLoadFileInfo.StrAccessoryType = _mQuestionInfo.QuestionsParam.StrAccessoryType;
                        InitCQTButton((QuestionType)_mQuestionInfo.QuestionsParam.IntType);
                        InitQuestionButs(_mQuestionInfo.QuestionsParam.IntType);
                        InitCategoryTreeInfo();
                        SetInterface(_mQuestionInfo.QuestionsParam.IntType);
                        break;
                    case S3601Codes.OperationCreateQuestion:
                        InitCategoryTreeInfo();
                        break;
                }
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
                            PopupPanel.ChangeLanguage();
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
                                    if (PopupPanel != null)
                                    {
                                        PopupPanel.ChangeLanguage();
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

        private void InitBasicSetingBut()
        {
            BasicSetBut.Children.Clear();
            System.Windows.Controls.Button btn;
            OperationInfo opt;
            btn = new System.Windows.Controls.Button();
            btn.Click += GoToExamProductionPage_Click;
            opt = new OperationInfo();
            opt.Display = UMPApp.GetLanguageInfo("3601T00088", "Go Back");
            opt.Icon = "Images/back.png";
            btn.DataContext = opt;
            btn.SetResourceReference(StyleProperty, "OptButtonStyle");
            BasicSetBut.Children.Add(btn);
        }

        private void InitCQTButton()
        {
            CQTBut.Children.Clear();
            System.Windows.Controls.Button btn;
            OperationInfo opt;
            btn = new System.Windows.Controls.Button();
            btn.Click += CreateTrueOrFalse_Click;
            opt = new OperationInfo();
            opt.Display = UMPApp.GetLanguageInfo("3601T00098", "True Or False");
            opt.Icon = "Images/add.png";
            btn.DataContext = opt;
            btn.SetResourceReference(StyleProperty, "OptButtonStyle");
            CQTBut.Children.Add(btn);

            btn = new System.Windows.Controls.Button();
            btn.Click += CreateSingleChoice_Click;
            opt = new OperationInfo();
            opt.Display = UMPApp.GetLanguageInfo("3601T00097", "Single Choice");
            opt.Icon = "Images/add.png";
            btn.DataContext = opt;
            btn.SetResourceReference(StyleProperty, "OptButtonStyle");
            CQTBut.Children.Add(btn);

            btn = new System.Windows.Controls.Button();
            btn.Click += CreateMultipleChoice_Click;
            opt = new OperationInfo();
            opt.Display = UMPApp.GetLanguageInfo("3601T00099", "Multiple Choice");
            opt.Icon = "Images/add.png";
            btn.DataContext = opt;
            btn.SetResourceReference(StyleProperty, "OptButtonStyle");
            CQTBut.Children.Add(btn);
        }

        private void InitCQTButton(QuestionType s3601Codes)
        {
            CQTBut.Children.Clear();
            System.Windows.Controls.Button btn;
            OperationInfo opt;
            switch (s3601Codes)
            {
                case QuestionType.Trueorfalse:
                    btn = new System.Windows.Controls.Button();
                    btn.Click += CreateTrueOrFalse_Click;
                    opt = new OperationInfo();
                    opt.Display = UMPApp.GetLanguageInfo("3601T00098", "True Or False");
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
                    opt.Display = UMPApp.GetLanguageInfo("3601T00097", "Single Choice");
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
                    opt.Display = UMPApp.GetLanguageInfo("3601T00099", "Multiple Choice");
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
            opt.Display = UMPApp.GetLanguageInfo("3601T00004", "Change Question");
            opt.Icon = "Images/add.png";
            btn.DataContext = opt;
            btn.SetResourceReference(StyleProperty, "OptButtonStyle");
            QSBut.Children.Add(btn);

            btn = new System.Windows.Controls.Button();
            btn.Click += DeleteQuestion_Click;
            opt = new OperationInfo();
            opt.Display = UMPApp.GetLanguageInfo("3601T00005", "Delete Question");
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
            opt.Display = UMPApp.GetLanguageInfo("3601T00007", "Browse");
            opt.ID = 31070021;
            opt.Icon = "Images/add.png";
            btn.DataContext = opt;
            btn.SetResourceReference(StyleProperty, "OptButtonStyle");
            QSBut.Children.Add(btn);

            btn = new System.Windows.Controls.Button();
            btn.Click += DeleteQuestion_Click;
            opt = new OperationInfo();
            opt.Display = UMPApp.GetLanguageInfo("3601T00005", "Delete");
            opt.ID = 31070021;
            opt.Icon = "Images/add.png";
            btn.DataContext = opt;
            btn.SetResourceReference(StyleProperty, "OptButtonStyle");
            QSBut.Children.Add(btn);
        }

        void InitNewQuestionInformationList()
        {
            try
            {
                string[] lans = "3601T00016,3601T00017,3601T00018,3601T00019,3601T00020,3601T00021,3601T00022,3601T00023,3601T00024,3601T00025,3601T00026,3601T00026,3601T00026,3601T00026,3601T00026,3601T00026,3601T00027,3601T00028,3601T00029,3601T00030,3601T00031,3601T00032".Split(',');
                string[] cols = "LongNum,StrCategoryName,IntType,StrQuestionsContect,StrAnswerOne,StrAnswerTwo,StrAnswerThree,StrAnswerFour,StrAnswerFive,StrAnswerSix,CorrectAnswerOne,CorrectAnswerTwo,  CorrectAnswerThree,CorrectAnswerFour,CorrectAnswerFive, CorrectAnswerSix, IntUseNumber, StrAccessoryType, StrAccessoryName, StrAccessoryPath, StrFounderName,StrDateTime".Split(',');
                int[] colwidths = { 100, 100, 60, 180, 80, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100 };
                GridView columnGridView = new GridView();
                GridViewColumn gvc;

                for (int i = 0; i < 22; i++)
                {
                    gvc = new GridViewColumn();
                    gvc.Header = UMPApp.GetLanguageInfo(lans[i], cols[i]);
                    gvc.Width = colwidths[i];
                    gvc.DisplayMemberBinding = new Binding(cols[i]);
                    columnGridView.Columns.Add(gvc);
                }
                NewLqDocument.View = columnGridView;
            }
            catch (Exception ex)
            {
                UMPApp.ShowExceptionMessage(ex.Message);
            }
        }

        void InitAllQuestionInformationList()
        {
            try
            {
                string[] lans = "3601T00016,3601T00017,3601T00018,3601T00019,3601T00020,3601T00021,3601T00022,3601T00023,3601T00024,3601T00025,3601T00026,3601T00026,3601T00026,3601T00026,3601T00026,3601T00026,3601T00027,3601T00028,3601T00029,3601T00030,3601T00031,3601T00032".Split(',');
                string[] cols = "LongNum,StrCategoryName,IntType,StrQuestionsContect,StrAnswerOne,StrAnswerTwo,StrAnswerThree,StrAnswerFour,StrAnswerFive,StrAnswerSix,CorrectAnswerOne,CorrectAnswerTwo,  CorrectAnswerThree,CorrectAnswerFour,CorrectAnswerFive, CorrectAnswerSix, IntUseNumber, StrAccessoryType, StrAccessoryName, StrAccessoryPath, StrFounderName,StrDateTime".Split(',');
                int[] colwidths = { 100, 100, 60, 180, 80, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100 };
                GridView columnGridView = new GridView();
                GridViewColumn gvc;

                for (int i = 0; i < 22; i++)
                {
                    gvc = new GridViewColumn();
                    gvc.Header = UMPApp.GetLanguageInfo(lans[i], cols[i]);
                    gvc.Width = colwidths[i];
                    gvc.DisplayMemberBinding = new Binding(cols[i]);
                    columnGridView.Columns.Add(gvc);
                }
                AllLqDocument.View = columnGridView;
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
                PanelItem panelItem;
                panelItem = new PanelItem();
                panelItem.PanelId = S3601Consts.PANEL_ID_TREE;
                panelItem.Name = S3601Consts.PANEL_NAME_TREE;
                panelItem.ContentId = S3601Consts.PANEL_CONTENTID_TREE;
                panelItem.Title = "Question Category";
                panelItem.Icon = "Images/00005.png";
                panelItem.IsVisible = true;
                panelItem.CanClose = true;
                _mListPanels.Add(panelItem);

                panelItem = new PanelItem();
                panelItem.PanelId = S3601Consts.PANEL_ID_NEWQUESTION;
                panelItem.Name = S3601Consts.PANEL_NAME_NEWQUESTION;
                panelItem.ContentId = S3601Consts.PANEL_CONTENTID_NEWQUESTION;
                panelItem.Title = "Questions";
                panelItem.Icon = "Images/00004.png";
                panelItem.IsVisible = true;
                panelItem.CanClose = true;
                _mListPanels.Add(panelItem);

                panelItem = new PanelItem();
                panelItem.PanelId = S3601Consts.PANEL_ID_ALLQUESTION;
                panelItem.Name = S3601Consts.PANEL_NAME_ALLQUESTION;
                panelItem.ContentId = S3601Consts.PANEL_CONTENTID_ALLQUESTION;
                panelItem.Title = "Questions";
                panelItem.Icon = "Images/00006.png";
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
                case (int)QuestionType.Trueorfalse:
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
                string uri = string.Format("/Themes/Default/UMPS3601/MainPageStyle.xaml");
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
            InitNewQuestionInformationList();
            InitAllQuestionInformationList();

            if (PageHead != null)
            {
                PageHead.AppName = UMPApp.GetLanguageInfo("3601T00002", "Create Question");
            }
            CQTExpandStyleOpt.Header = UMPApp.GetLanguageInfo("3601T00109", "Choice Question Type");
            QSExpandStyleOpt.Header = UMPApp.GetLanguageInfo("3601T00010", "Change Question");
            BSExpandStyleOpt.Header = UMPApp.GetLanguageInfo("3601T00111", "Basic Setting");
            PanelObjectTreeBox.Title = UMPApp.GetLanguageInfo("3601T00034", "Category");
            PanelObjectNewListQuestion.Title = UMPApp.GetLanguageInfo("3601T00140", "New Question");
            PanelObjectAllListQuestion.Title = UMPApp.GetLanguageInfo("3601T00141", "All Question");
            SCPanelLearnDocument.Title = UMPApp.GetLanguageInfo("3601T00097", "Single Choice");
            MCPanelLearnDocument.Title = UMPApp.GetLanguageInfo("3601T00099", "Multiple Choice");
            TOFPanelLearnDocument.Title = UMPApp.GetLanguageInfo("3601T00098", "True Or False");
            lbSCAttachments.Content = UMPApp.GetLanguageInfo("3601T00030", "Attachments path");
            lbMCAttachments.Content = UMPApp.GetLanguageInfo("3601T00030", "Attachments path");
            lbTOFAttachments.Content = UMPApp.GetLanguageInfo("3601T00030", "Attachments path");
            butSCBrowse.Content = UMPApp.GetLanguageInfo("3601T00007", "Browse");
            butMCBrowse.Content = UMPApp.GetLanguageInfo("3601T00007", "Browse");
            butTOFBrowse.Content = UMPApp.GetLanguageInfo("3601T00007", "Browse");
            butSCClear.Content = UMPApp.GetLanguageInfo("3601T00112", "Clear");
            butMCClear.Content = UMPApp.GetLanguageInfo("3601T00112", "Clear");
            butTOFClear.Content = UMPApp.GetLanguageInfo("3601T00112", "Clear");
            butSCSave.Content = UMPApp.GetLanguageInfo("3601T00113", "Save");
            butMCSave.Content = UMPApp.GetLanguageInfo("3601T00113", "Save");
            butTOFSave.Content = UMPApp.GetLanguageInfo("3601T00113", "Save");
            lbSCQusetionConten.Content = UMPApp.GetLanguageInfo("3601T00035", "QusetionConten:");
            lbMCQusetionConten.Content = UMPApp.GetLanguageInfo("3601T00035", "QusetionConten:");
            lbTOFQusetionConten.Content = UMPApp.GetLanguageInfo("3601T00035", "QusetionConten:");
            lbSCOptionAnswer.Content = UMPApp.GetLanguageInfo("3601T00026", "Answer");
            lbMCOptionAnswer.Content = UMPApp.GetLanguageInfo("3601T00026", "Answer");
            lbTOFOptionAnswer.Content = UMPApp.GetLanguageInfo("3601T00026", "Answer");
            BrowseQuestionDocument.Title = UMPApp.GetLanguageInfo("3601T00003", "Browse Question");
            rbutTOFTrue.Content = UMPApp.GetLanguageInfo("3601T00064", "Yes");
            rbutTOFFalse.Content = UMPApp.GetLanguageInfo("3601T00065", "No");
        }
        #endregion

        #region Add Test Node Tree
        public void InitCategoryTreeInfo()
        {
            try
            {
                _mListCategoryTreeParam.Clear();
                WebRequest webRequest = new WebRequest();
                webRequest.Session = App.Session;
                webRequest.Code = (int)S3601Codes.OperationGetQuestionCategory;
                Service36011Client client = new Service36011Client(WebHelper.CreateBasicHttpBinding(App.Session), WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service36011"));
                //var client = new Service36011Client();
                WebReturn webReturn = client.UmpTaskOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                    return;

                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<PapersCategoryParam>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        UMPApp.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        continue;
                    }
                    PapersCategoryParam param = optReturn.Data as PapersCategoryParam;
                    if (param == null)
                    {
                        UMPApp.ShowExceptionMessage(string.Format("Fail. queryItem is null"));
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
                UMPApp.ShowExceptionMessage(ex.Message);
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
                temp.Icon = "/Themes/Default/UMPS3601/Images/document.ico";
                temp.LongNum = param.LongNum;
                temp.StrName = param.StrName;
                temp.LongParentNodeId = param.LongParentNodeId;
                temp.StrParentNodeName = param.StrParentNodeName;
                temp.LongFounderId = param.LongFounderId;
                temp.StrFounderName = param.StrFounderName;
                temp.StrDateTime = param.StrDateTime;
//                 if (temp.StrName == App.g_CategoryTree.StrParentNodeName)
//                 {
//                     temp.IsChecked = true;
//                     temp.IsExpanded = true;
//                 }
                AddChildNode(parentInfo, temp);
            }
            catch (Exception ex)
            {
                UMPApp.ShowExceptionMessage(ex.Message);
            }
            return temp;
        }

        private void AddChildNode(CCategoryTree parentItem, CCategoryTree item)
        {
            Dispatcher.Invoke(new Action(() => parentItem.AddChild(item)));
        }

        #endregion

        #region Click

        void NewLqDocument_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                BrowseQuestionDocument.Show();
                App.GListQuestionInfos.Clear();
                if (Keyboard.IsKeyDown(Key.RightCtrl) ||
                    Keyboard.IsKeyDown(Key.RightShift) ||
                    Keyboard.IsKeyDown(Key.LeftCtrl) ||
                    Keyboard.IsKeyDown(Key.LeftShift))
                {
                    return;
                }
                InitQSButton();
                _mCExamQuestionsTemp = new CQuestionsParam();
                App.GQuestionInfo = new QuestionInfo();
                App.GListQuestionInfos.Clear();
                var item = NewLqDocument.SelectedItem as CQuestionsParam;
                _mCExamQuestionsTemp = item;
                _mListQuestionInfos.Add(_mCExamQuestionsTemp);
                App.GListQuestionInfos = _mListQuestionInfos;
                if (_mCExamQuestionsTemp != null)
                {
                    switch (_mCExamQuestionsTemp.IntType)
                    {
                        case (int)QuestionType.Trueorfalse:
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
                UMPApp.ShowExceptionMessage(ex.Message);
            }
        }

        void AllLqDocument_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                BrowseQuestionDocument.Show();
                App.GListQuestionInfos.Clear();
                if (Keyboard.IsKeyDown(Key.RightCtrl) ||
                    Keyboard.IsKeyDown(Key.RightShift) ||
                    Keyboard.IsKeyDown(Key.LeftCtrl) ||
                    Keyboard.IsKeyDown(Key.LeftShift))
                {
                    return;
                }
                InitQSButton();
                _mCExamQuestionsTemp = new CQuestionsParam();
                App.GQuestionInfo = new QuestionInfo();
                App.GListQuestionInfos.Clear();
                var item = AllLqDocument.SelectedItem as CQuestionsParam;
                _mCExamQuestionsTemp = item;
                _mListQuestionInfos.Add(_mCExamQuestionsTemp);
                App.GListQuestionInfos = _mListQuestionInfos;
                if (_mCExamQuestionsTemp != null)
                {
                    switch (Convert.ToInt16(_mCExamQuestionsTemp.IntType))
                    {
                        case (int)QuestionType.Trueorfalse:
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
                UMPApp.ShowExceptionMessage(ex.Message);
            }
        }

        void LqDocument_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            try
            {
                _mListQuestionInfos.Clear();
                App.GListQuestionInfos.Clear();
                int iCount = NewLqDocument.SelectedItems.Count;
                for (int i = 0; i < iCount; i++)
                {
                    var itame = NewLqDocument.SelectedItems[i] as CQuestionsParam;
                    _mListQuestionInfos.Add(itame);
                }
                App.GListQuestionInfos = _mListQuestionInfos;
                if (_mListQuestionInfos.Count > 1)
                {
                    InitQSButton2();
                }   
            }
            catch (Exception ex)
            {
                UMPApp.ShowExceptionMessage(ex.Message);
            }
        }

        private void GoToExamProductionPage_Click( object sender, RoutedEventArgs e )
        {
            var btn = e.Source as System.Windows.Controls.Button;
            if (btn != null)
            {
                NavigateEpPage();
            }
        }

        private void CreateTrueOrFalse_Click(object sender, RoutedEventArgs e)
        {
            TrueOrFalseShow();
        }

        private void CreateSingleChoice_Click(object sender, RoutedEventArgs e)
        {
            SingleChoiceShow();
        }

        private void CreateMultipleChoice_Click(object sender, RoutedEventArgs e)
        {
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
                        case (int)QuestionType.Trueorfalse:
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
                UMPApp.ShowExceptionMessage(ex.Message);
            }
            
        }

        private void butTOFClear_Click(object sender, RoutedEventArgs e)
        {
            TxtTOFQusetionConten.Clear();
            TxtTOFAttachments.Clear();
            rbutTOFTrue.IsChecked = false;
            rbutTOFFalse.IsChecked = false;
            _mbEnableBrowse = false; 
        }

        private void butTOFSave_Click(object sender, RoutedEventArgs e)
        {
            CQuestionsParam cExamQuestion = new CQuestionsParam();
            try
            {
                cExamQuestion.StrQuestionsContect = GetEditBoxContents_1024(TxtTOFQusetionConten.Text);
                if (cExamQuestion.StrQuestionsContect == null) return;

                if (rbutTOFTrue.IsChecked == true || rbutTOFFalse.IsChecked == true)
                {
                    if (rbutTOFTrue.IsChecked == true)
                        cExamQuestion.CorrectAnswerOne = "T";
                    else
                        cExamQuestion.CorrectAnswerOne = "F";
                }
                else
                {
                    UMPApp.ShowInfoMessage(UMPApp.GetLanguageInfo("3601T00143", "Please set up a correct answer"));
                    return;
                }
                if (_mbEnableBrowse)
                {
                    if (!string.IsNullOrWhiteSpace(TxtTOFAttachments.Text))
                    {
                        if (!UpLoadFiles(TxtTOFAttachments.Text))
                            return;

                        if (!string.IsNullOrWhiteSpace(_mUpLoadFileInfo.StrAccessoryPath))
                        {
                            cExamQuestion.StrAccessoryPath = _mUpLoadFileInfo.StrAccessoryPath;
                            cExamQuestion.StrAccessoryName = _mUpLoadFileInfo.StrAccessoryName;
                            cExamQuestion.StrAccessoryType = _mUpLoadFileInfo.StrAccessoryType;
                        }
                        else
                        {
                            UMPApp.ShowInfoMessage(UMPApp.GetLanguageInfo("3601T00132", "UMP Path is null !"));
                            return;
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
                cExamQuestion.IntType = (int)QuestionType.Trueorfalse;

                switch (_mQuestionInfo.OptType)
                {
                    case S3601Codes.OperationUpdateQuestion:
                        UpDateDbOpt(cExamQuestion);
                        _mQuestionInfo.OptType = S3601Codes.OperationCreateQuestion;
                        break;
                    case S3601Codes.OperationCreateQuestion:
                        WriteDbOpt(cExamQuestion);
                        break;
                }
            }
            catch (Exception ex)
            {
                UMPApp.ShowExceptionMessage(ex.Message);
            }
        }

        private void butSCClear_Click(object sender, RoutedEventArgs e)
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

        private void butSCSave_Click(object sender, RoutedEventArgs e)
        {
            CQuestionsParam cExamQuestion = new CQuestionsParam();
            try
            {
                if (_mCategoryNodeTemp.LongNum == 0 && _mQuestionInfo.OptType == S3601Codes.OperationCreateQuestion)
                {
                    UMPApp.ShowExceptionMessage(UMPApp.GetLanguageInfo("3601T00120", "Not for the category, unable to save the Question!"));
                    return;
                }

                cExamQuestion.StrQuestionsContect = GetEditBoxContents_1024(TxtSCQusetionConten.Text);
                if (cExamQuestion.StrQuestionsContect == null) return;

                cExamQuestion.StrAnswerOne = GetEditBoxContents_200(TxtSCOptionA.Text);
                if (cExamQuestion.StrAnswerOne == null) return;

                cExamQuestion.StrAnswerTwo = GetEditBoxContents_200(TxtSCOptionB.Text);
                if (cExamQuestion.StrAnswerTwo == null) return;

                cExamQuestion.StrAnswerThree = GetEditBoxContents_200(TxtSCOptionC.Text);
                if (cExamQuestion.StrAnswerThree == null) return;

                cExamQuestion.StrAnswerFour = GetEditBoxContents_200(TxtSCOptionD.Text);
                if (cExamQuestion.StrAnswerFour == null) return;

                if (rbutA.IsChecked == true ||
                    rbutB.IsChecked == true ||
                    rbutC.IsChecked == true ||
                    rbutD.IsChecked == true)
                {
                    if (rbutA.IsChecked == true)
                        cExamQuestion.CorrectAnswerOne = "A";
                    else if(rbutB.IsChecked == true)
                        cExamQuestion.CorrectAnswerOne = "B";
                    else if (rbutC.IsChecked == true)
                        cExamQuestion.CorrectAnswerOne = "C";
                    else if (rbutD.IsChecked == true)
                        cExamQuestion.CorrectAnswerOne = "D";
                }
                else
                {
                    UMPApp.ShowInfoMessage(UMPApp.GetLanguageInfo("3601T00143", "Please set up a correct answer"));
                    return;
                }

                cExamQuestion.LongCategoryNum = _mCategoryNodeTemp.LongNum;
                cExamQuestion.StrCategoryName = _mCategoryNodeTemp.StrName;
                cExamQuestion.IntType = (int)QuestionType.SingleChoice;
                
                if (_mbEnableBrowse)
                {
                    if (!string.IsNullOrWhiteSpace(TxtSCAttachments.Text))
                    {
                        if (!UpLoadFiles(TxtSCAttachments.Text))
                            return;

                        if (!string.IsNullOrWhiteSpace(_mUpLoadFileInfo.StrAccessoryPath))
                        {
                            cExamQuestion.StrAccessoryPath = _mUpLoadFileInfo.StrAccessoryPath;
                            cExamQuestion.StrAccessoryName = _mUpLoadFileInfo.StrAccessoryName;
                            cExamQuestion.StrAccessoryType = _mUpLoadFileInfo.StrAccessoryType;
                        }
                        else
                        {
                            UMPApp.ShowInfoMessage(UMPApp.GetLanguageInfo("3601T00132", "UMP Path is null !"));
                            return;
                        }
                    }
                }
                else
                {
                    cExamQuestion.StrAccessoryPath = _mUpLoadFileInfo.StrAccessoryPath;
                    cExamQuestion.StrAccessoryName = _mUpLoadFileInfo.StrAccessoryName;
                    cExamQuestion.StrAccessoryType = _mUpLoadFileInfo.StrAccessoryType;
                }
                
                switch (_mQuestionInfo.OptType)
                {
                    case S3601Codes.OperationUpdateQuestion:
                        UpDateDbOpt(cExamQuestion);
                        _mQuestionInfo.OptType = S3601Codes.OperationCreateQuestion;
                        break;
                    case S3601Codes.OperationCreateQuestion:
                        WriteDbOpt(cExamQuestion);
                        break;
                }
            }
            catch (Exception ex)
            {
                UMPApp.ShowExceptionMessage(ex.Message);
            }
        }

        private void butMCClear_Click(object sender, RoutedEventArgs e)
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

        private void butMCSave_Click(object sender, RoutedEventArgs e)
        {
            CQuestionsParam cExamQuestion = new CQuestionsParam();
            try
            {
                cExamQuestion.StrQuestionsContect = GetEditBoxContents_1024(TxtMCQusetionConten.Text);
                if (cExamQuestion.StrQuestionsContect == null) return;

                cExamQuestion.StrAnswerOne = GetEditBoxContents_200(TxtMCOptionA.Text);
                if (cExamQuestion.StrAnswerOne == null) return;

                cExamQuestion.StrAnswerTwo = GetEditBoxContents_200(TxtMCOptionB.Text);
                if (cExamQuestion.StrAnswerTwo == null) return;

                cExamQuestion.StrAnswerThree = GetEditBoxContents_200(TxtMCOptionC.Text);
                if (cExamQuestion.StrAnswerThree == null) return;

                cExamQuestion.StrAnswerFour = GetEditBoxContents_200(TxtMCOptionD.Text);
                if (cExamQuestion.StrAnswerFour == null) return;
                cExamQuestion.StrAnswerFive = GetEditBoxContents_200(TxtMCOptionE.Text);
                if (cExamQuestion.StrAnswerFive == null) return;
                cExamQuestion.StrAnswerSix = GetEditBoxContents_200(TxtMCOptionF.Text);
                if (cExamQuestion.StrAnswerSix == null) return;

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
                    UMPApp.ShowInfoMessage(UMPApp.GetLanguageInfo("3601T00143", "Please set up a correct answer"));
                    return;
                }

                if (_mbEnableBrowse)
                {
                    if (!string.IsNullOrWhiteSpace(TxtMCAttachments.Text))
                    {
                        if (!UpLoadFiles(TxtMCAttachments.Text))
                            return;

                        if (!string.IsNullOrWhiteSpace(_mUpLoadFileInfo.StrAccessoryPath))
                        {
                            cExamQuestion.StrAccessoryPath = _mUpLoadFileInfo.StrAccessoryPath;
                            cExamQuestion.StrAccessoryName = _mUpLoadFileInfo.StrAccessoryName;
                            cExamQuestion.StrAccessoryType = _mUpLoadFileInfo.StrAccessoryType;
                        }
                        else
                        {
                            UMPApp.ShowInfoMessage(UMPApp.GetLanguageInfo("3601T00132", "UMP Path is null !"));
                            return;
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

                switch (_mQuestionInfo.OptType)
                {
                    case S3601Codes.OperationUpdateQuestion:
                        UpDateDbOpt(cExamQuestion);
                        _mQuestionInfo.OptType = S3601Codes.OperationCreateQuestion;
                        break;
                    case S3601Codes.OperationCreateQuestion:
                        WriteDbOpt(cExamQuestion);
                        break;
                }
            }
            catch (Exception ex)
            {
                UMPApp.ShowExceptionMessage(ex.Message);
            }
        }

        private void UpDateQuestion_Click(object sender, RoutedEventArgs e)
        {
            var btn = e.Source as System.Windows.Controls.Button;
            if (btn != null)
            {
                _mbEnableBrowse = false;
                App.GMessageSource = new S3601Codes();
                App.GMessageSource = S3601Codes.MessageCreatequestionspage;
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
                App.GQueryModify = false;
                BrowseQustions browseQustions = new BrowseQustions();
                //newMainCategory.CategoryTree = m_CategoryNodeTemp;
                browseQustions.CqParentPage = this;
                PopupPanel.Content = browseQustions;
                App.GMessageSource = new S3601Codes();
                App.GMessageSource = S3601Codes.MessageCreatequestionspage;
                PopupPanel.IsOpen = true;
            }
        }

        private void DeleteQuestion_Click(object sender, RoutedEventArgs e)
        {
            if (_mListQuestionInfos.Count == 0)
                return;

            if (DeleteQuestionInfos())
            {
                foreach( CQuestionsParam cExamQuestion in _mListQuestionInfos )
                {
                    _mLstNewExamQuestions.Remove(cExamQuestion);
                    _mLstAllExamQuestions.Remove(cExamQuestion);
                    _mCExamQuestionsTemp = new CQuestionsParam();
                }
                _mListQuestionInfos.Clear();
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
                InitCQTButton();
                _mCategoryNodeTemp = nodeInfo;
                GetQuestionInfos();
            }
            catch (Exception ex)
            {
                UMPApp.ShowExceptionMessage(ex.Message);
            }
        }
        #endregion

        #region Operations
        private void NavigateEpPage()
        {
            try
            {
                if (NavigationService != null)
                    NavigationService.Navigate(new Uri("ExamProduction.xaml", UriKind.Relative));
            }
            catch (Exception ex)
            {
                UMPApp.ShowExceptionMessage(ex.Message);
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
                        switch (item.PanelId)
                        {
                            case 1:
                                panel.Title = UMPApp.GetLanguageInfo("3601T00034", item.Name);
                                break;
                            case 2:
                                panel.Title = UMPApp.GetLanguageInfo("3601T00140", item.Name);
                                break;
                            case 3:
                                panel.Title = UMPApp.GetLanguageInfo("3601T00141", item.Name);
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

        private void UpDateDbOpt(CQuestionsParam cExamQuestion)
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
                webRequest.Session = UMPApp.Session;
                webRequest.Code = (int)S3601Codes.OperationUpdateQuestion;
                OperationReturn optReturn = XMLHelper.SeriallizeObject(cExamQuestion);
                if (!optReturn.Result)
                {
                    UMPApp.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                webRequest.ListData.Add(optReturn.Data.ToString());
                client = new Service36011Client(WebHelper.CreateBasicHttpBinding(App.Session), WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service36011"));
                //client = new Service36011Client();
                webReturn = client.UmpTaskOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    UMPApp.ShowExceptionMessage(string.Format("{0}{1} ", UMPApp.GetLanguageInfo("3601T00015", "Insert data failed"), webReturn.Message));
                    return;
                }
                if (App.GMessageSource == S3601Codes.MessageCreatequestionspage)
                {
                    _mLstNewExamQuestions.Remove(
                        _mLstNewExamQuestions.Where(p => p.LongNum == cExamQuestion.LongNum).FirstOrDefault());
                    _mLstAllExamQuestions.Remove(
                        _mLstAllExamQuestions.Where(p => p.LongNum == cExamQuestion.LongNum).FirstOrDefault());
                }
                _mLstNewExamQuestions.Add(cExamQuestion);
                _mLstAllExamQuestions.Add(cExamQuestion);
                _mCExamQuestionsTemp = cExamQuestion;
            }
            catch (Exception ex)
            {
                UMPApp.ShowExceptionMessage(ex.Message);
            }
        }


        private void WriteDbOpt( CQuestionsParam cExamQuestion)
        {
            WebRequest webRequest;
            Service36011Client client;
            WebReturn webReturn;
            try
            {
                if (!App.GQueryModify)
                {
                    //生成新的查询配置表主键
                    webRequest = new WebRequest();
                    webRequest.Session = UMPApp.Session;
                    webRequest.Code = (int)RequestCode.WSGetSerialID;
                    webRequest.ListData.Add("36");
                    webRequest.ListData.Add("3601");
                    webRequest.ListData.Add(DateTime.Now.ToString("yyyyMMddHHmmss"));
                    cExamQuestion.StrDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    client = new Service36011Client(WebHelper.CreateBasicHttpBinding(App.Session), WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service36011"));
                    //client = new Service36011Client();
                    webReturn = client.UmpTaskOperation(webRequest);
                    client.Close();
                    if (!webReturn.Result)
                        return;
                    string strNewResultID = webReturn.Data;
                    if (string.IsNullOrEmpty(strNewResultID))
                        return;

                    cExamQuestion.LongNum = Convert.ToInt64(strNewResultID);
                }

                webRequest = new WebRequest();
                webRequest.Session = UMPApp.Session;
                webRequest.Code = (int)S3601Codes.OperationAddQuestion;
                OperationReturn optReturn = XMLHelper.SeriallizeObject(cExamQuestion);
                if (!optReturn.Result)
                {
                    App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                webRequest.ListData.Add(optReturn.Data.ToString());
                client = new Service36011Client(WebHelper.CreateBasicHttpBinding(App.Session), WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service36011"));
                //client = new Service36011Client();
                webReturn = client.UmpTaskOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    UMPApp.ShowExceptionMessage(webReturn.Message);
                    return;
                }
                else
                {
                    ///App.ShowExceptionMessage(App.GetLanguageInfo("3601T00001", "Save data Success"));
                    _mLstNewExamQuestions.Add(cExamQuestion);
                    _mLstAllExamQuestions.Add(cExamQuestion);
                }
            }
            catch (Exception ex)
            {
                UMPApp.ShowExceptionMessage(ex.Message);
            }
        }

        //获取UMP安装目录
        private void GetUmpSetupPath()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = UMPApp.Session;
                webRequest.Code = (int)S3601Codes.OperationGetUmpsetuppath;
                Service36011Client client = new Service36011Client(WebHelper.CreateBasicHttpBinding(App.Session), WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service36011"));
                //var client = new Service36011Client();
                WebReturn webReturn = client.UmpTaskOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    UMPApp.ShowExceptionMessage(webReturn.Message);
                    return;
                }
                if (string.IsNullOrWhiteSpace(webReturn.Data)) { return; }
                _mUmpFileRootPath = webReturn.Data;
                _mUmpFileRootPathOld = _mUmpFileRootPath;
                UMPApp.WriteLog("UMPFolderPath", _mUmpFileRootPath);
            }
            catch (Exception ex)
            {
                UMPApp.ShowExceptionMessage(ex.Message);
            }
        }

        private void GetUpLoadFilePath()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = UMPApp.Session;
                webRequest.Code = (int)S3601Codes.OperationGetUploadFilePath;
                Service36011Client client = new Service36011Client(WebHelper.CreateBasicHttpBinding(App.Session), WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service36011"));
                //var client = new Service36011Client();
                WebReturn webReturn = client.UmpTaskOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    UMPApp.ShowExceptionMessage(webReturn.Message);
                    return;
                }
                if (string.IsNullOrWhiteSpace(webReturn.Data)) { return; }
                _mUploadFilePath = webReturn.Data;
                UMPApp.WriteLog("UMPUploadFolderPath", _mUploadFilePath);
            }
            catch (Exception ex)
            {
                UMPApp.ShowExceptionMessage(ex.Message);
            }
        }

        //上传文件
        private bool UpLoadFiles(string localPath)
        {
            WebReturn webReturn = new WebReturn();
            UpRequest upRquest = new UpRequest();
            try
            {
                string serverPath = System.IO.Path.Combine( localPath, _mUmpFileRootPath);
                byte[] btFileArrayRead = System.IO.File.ReadAllBytes(localPath);
                List<byte> listFileByte = new List<byte>();
                Service36011Client client = new Service36011Client(WebHelper.CreateBasicHttpBinding(App.Session), WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service36011"));
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
                            UMPApp.ShowInfoMessage(string.Format("{0}: {1}", webReturn.Message, serverPath));
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
                        UMPApp.ShowInfoMessage(string.Format("{0}: {1}", webReturn.Message, serverPath));
                        return false;
                    }
                }
                UMPApp.WriteLog("UpPath", serverPath);
            }
            catch (Exception ex)
            {
                #region 写操作日志
                string strLog = string.Format("{0} {1}{2}", UMPApp.Session.UserID, Utils.FormatOptLogString("FO3106005"), _mBrowsePath);
                UMPApp.WriteOperationLog(S3601Consts.OPT_UpLoad.ToString(), ConstValue.OPT_RESULT_FAIL, strLog);
                #endregion
                UMPApp.ShowExceptionMessage(ex.Message);
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
                    UMPApp.ShowInfoMessage(UMPApp.GetLanguageInfo("3601T00133", "Content beyond 1024"));
                    return null;
                }
            }
            else
            {
                UMPApp.ShowInfoMessage(UMPApp.GetLanguageInfo("3601T00131", "Problems can not be empty"));
                return null;
            }
            strTemp = strStr;
            return strTemp;
        }

        private string GetEditBoxContents_200(string strStr)
        {
            string strTemp;
            if (!string.IsNullOrWhiteSpace(strStr))
            {
                if (strStr.Length > 200)
                {
                    UMPApp.ShowInfoMessage(UMPApp.GetLanguageInfo("3601T00134", "Content beyond 200"));
                    return null;
                }
            }
            else
            {
                UMPApp.ShowInfoMessage(UMPApp.GetLanguageInfo("3601T00142", "The answer options cannot be empty"));
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
            _mIntQuestionType = (int)QuestionType.Trueorfalse;
        }

        private void SingleChoiceShow()
        {
            SCPanelLearnDocument.Show();
            MCPanelLearnDocument.Hide();
            TOFPanelLearnDocument.Hide();
            _mIntQuestionType = (int)QuestionType.SingleChoice;
        }

        private void MultipleChoiceShow()
        {
            SCPanelLearnDocument.Hide();
            MCPanelLearnDocument.Show();
            TOFPanelLearnDocument.Hide();
            _mIntQuestionType = (int)QuestionType.MultipleChioce;
        }

        private void SetInterface(int interficeType)
        {
            switch (interficeType)
            {
                case (int)QuestionType.Trueorfalse:
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
            _mQuestionInfo = App.GQuestionInfo;
            InitQuestionButs(_mQuestionInfo.QuestionsParam.IntType);
            SetInterface(_mQuestionInfo.QuestionsParam.IntType);
            App.GQuestionInfo = new QuestionInfo();
        }

        private bool DeleteQuestionInfos()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = UMPApp.Session;
                webRequest.Code = (int)S3601Codes.OperationDeleteQuestion;
                
                OperationReturn optReturn = XMLHelper.SeriallizeObject(_mListQuestionInfos);
                if (!optReturn.Result)
                {
                    UMPApp.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return false;
                }
                webRequest.ListData.Add(optReturn.Data.ToString());
                Service36011Client client = new Service36011Client(WebHelper.CreateBasicHttpBinding(App.Session), WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service36011"));
                //var client = new Service36011Client();
                WebReturn webReturn = client.UmpTaskOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    UMPApp.ShowExceptionMessage(string.Format("{0}{1} ", UMPApp.GetLanguageInfo("3107T00092", "Delete Failed"), webReturn.Message));
                    #region 写操作日志
                    //strLog = string.Format("{0} {1}{2}", Utils.FormatOptLogString("3107T00005"), Utils.FormatOptLogString("3107T00028"), papersCategoryParam.StrName);
                    UMPApp.WriteOperationLog(S3601Consts.OPT_AutoTask.ToString(), ConstValue.OPT_RESULT_FAIL, null);
                    #endregion
                    return false;
                }
                if (webReturn.Message == S3601Consts.HadUse)// 该查询条件被使用无法删除
                {
                    UMPApp.ShowInfoMessage(UMPApp.GetLanguageInfo("3601T00011", "Can't Delete"));
                    return false;
                }
                UMPApp.ShowInfoMessage(UMPApp.GetLanguageInfo("3601T00012", "Delete Sucessed"));
                InitCategoryTreeInfo();
            }
            catch (Exception ex)
            {
                UMPApp.ShowExceptionMessage(ex.Message);
                return false;
            }
            return true;
        }

        private void InitRadioToF(CQuestionsParam cExamQuestion)
        {
            QuestionInfoGrid.Children.Clear();
            Grid grid = new Grid();
            TrueOrFlasePage trueOrFlasePage = new TrueOrFlasePage();
            ToFQuestionInfo tempParam = new ToFQuestionInfo();
            tempParam.IntNum = 0;
            tempParam.questionParam = cExamQuestion;
            trueOrFlasePage.SetQuestionInfo = tempParam;
            trueOrFlasePage.SetInformation();
            grid.Children.Add(trueOrFlasePage);
            Grid.SetRow(trueOrFlasePage, 0);
            QuestionInfoGrid.Children.Add(grid);
        }

        private void InitRadioSingle(CQuestionsParam cExamQuestion)
        {
            QuestionInfoGrid.Children.Clear();
            Grid grid = new Grid();
            SingleChoicePage singleChoicePage = new SingleChoicePage();
            SCQuestionInfo tempParam = new SCQuestionInfo();
            tempParam.IntNum = 0;
            tempParam.questionParam = cExamQuestion;
            singleChoicePage.SetQuestionInfo = tempParam;
            singleChoicePage.SetInformation();
            grid.Children.Add(singleChoicePage);
            Grid.SetRow(singleChoicePage, 0);
            QuestionInfoGrid.Children.Add(grid);
        }

        private void InitRadioMultiple(CQuestionsParam cExamQuestion)
        {
            QuestionInfoGrid.Children.Clear();
            Grid grid = new Grid();
            MultipleChoicePage multipleChoicePage = new MultipleChoicePage();
            MCQuestionInfo tempParam = new MCQuestionInfo();
            tempParam.IntNum = 0;
            tempParam.questionParam = cExamQuestion;
            multipleChoicePage.SetQuestionInfo = tempParam;
            multipleChoicePage.SetInformation();
            grid.Children.Add(multipleChoicePage);
            Grid.SetRow(multipleChoicePage, 0);
            QuestionInfoGrid.Children.Add(grid);
        }

        private void GetQuestionInfos()
        {
            try
            {
                _mLstAllExamQuestions.Clear();
                var param = new PapersCategoryParam();
                var webRequest = new WebRequest();
                var client = new Service36011Client(WebHelper.CreateBasicHttpBinding(App.Session),
                    WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service36011"));
                //var client = new Service36011Client();
                webRequest.Session = UMPApp.Session;
                webRequest.Code = (int)S3601Codes.OperationGetQuestions;
                param.LongNum = _mCategoryNodeTemp.LongNum;
                param.StrName = _mCategoryNodeTemp.StrName;
                OperationReturn optReturn = XMLHelper.SeriallizeObject(param);
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
                    UMPApp.ShowExceptionMessage(string.Format("{0}{1} ", UMPApp.GetLanguageInfo("3601T00076", "Insert data failed"), webReturn.Message));
                    return;
                }

                if (webReturn.ListData.Count <= 0) { return; }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    optReturn = new OperationReturn();
                    optReturn = XMLHelper.DeserializeObject<CQuestionsParam>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        UMPApp.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        continue;
                    }
                    var cExamQuestions = optReturn.Data as CQuestionsParam;
                    if (cExamQuestions == null)
                    {
                        UMPApp.ShowExceptionMessage(string.Format("Fail. filesItem is null"));
                        return;
                    }

                    _mLstAllExamQuestions.Add(cExamQuestions);
                }
            }
            catch (Exception ex)
            {
                UMPApp.ShowExceptionMessage(ex.Message);
            }
        }

        #endregion
    }
}
