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
using VoiceCyber.UMP.Controls;
using System.Collections.ObjectModel;
using System.Windows.Controls.Primitives;
using UMPS3601.Wcf36011;
using VoiceCyber.Wpf.AvalonDock.Layout;


namespace UMPS3601
{
    /// <summary>
    /// ExamProduction.xaml 的交互逻辑
    /// </summary>
    public partial class ExamProduction
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

        #endregion

        public ExamProduction()
        {
            InitializeComponent();
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
            Loaded += QuestionsListInfo_Loaded;
            CategoryTree.SelectedItemChanged += OrgCategoryTree_SelectedItemChanged;
        }

        void QuestionsListInfo_Loaded(object sender, RoutedEventArgs e)
        {
            _mListExamQuestions.Clear();
            try
            {
                SetBusy(true);
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
                    ChangeTheme();
                    ATDocument.ItemsSource = _mListExamQuestions;
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
                            PopupPanelQuestionInfo.ChangeLanguage();
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
                            LangTypeInfo langTypeInfo =
                               UMPApp.Session.SupportLangTypes.FirstOrDefault(l => l.LangID.ToString() == strData);
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
                                    if (PopupPanelQuestionInfo != null)
                                    {
                                        PopupPanelQuestionInfo.ChangeLanguage();
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

        protected override void Init()
        {
            try
            {
                PageHead.AppName = "Exam Production";
                StylePath = "UMPS3601/MainPageStyle.xmal";
                base.Init();
                InitCategoryTreeInfo();
                InitAddQuestionsBut();
                InitPanels();
                CreateToolBarButtons();
                SetPanelVisible();
                SetViewStatus();
                SendLoadedMessage();
                ChangeTheme();
                ChangeLanguage();
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
                panelItem.PanelId = S3601Consts.PANEL_ID_TREE;
                panelItem.Name = S3601Consts.PANEL_NAME_TREE;
                panelItem.ContentId = S3601Consts.PANEL_CONTENTID_TREE;
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

            InitQuestionInformationList();

            if (PageHead != null)
            {
                PageHead.AppName = UMPApp.GetLanguageInfo("3601T00135", "Question Bank");
            }
            PanelObjectTreeBox.Title = UMPApp.GetLanguageInfo("3601T00034", "Question Category");
            BSExpandStyleOpt.Header = UMPApp.GetLanguageInfo("3601T00033", "Basic Settings");
            DeleteExpandStyleOpt.Header = UMPApp.GetLanguageInfo("3601T00114", "Delete Setting");
            AddQuestionsDocument.Title = UMPApp.GetLanguageInfo("3601T00035", "Questions");
        }
        #endregion

        #region Add Test Node Tree

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
                AddChildNode(parentInfo, temp);
            }
            catch (Exception ex)
            {
                UMPApp.ShowExceptionMessage(ex.Message);
            }
            return temp;
        }

        public CCategoryTree GetAddPapersNodeInfo(CCategoryTree parentInfo, PapersCategoryParam param)
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
                AddChildNode(parentInfo, temp);
            }
            catch (Exception ex)
            {
                UMPApp.ShowExceptionMessage(ex.Message);
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

        private void RemoveChildNode(CCategoryTree parentItem, CCategoryTree item)
        {
            Dispatcher.Invoke(new Action(() => parentItem.RemoveChild(item)));
        }

        #endregion

        #region Init Information
        void InitAddQuestionsBut()
        {
            BasicSetBut.Children.Clear();
            Button btn;
            OperationInfo opt;
            btn = new Button();
            btn.Click += AddQuestions_Click;
            opt = new OperationInfo();
            opt.Display = UMPApp.GetLanguageInfo("3601T00002", "Create Question");
            opt.Icon = "Images/add.png";
            btn.DataContext = opt;
            btn.SetResourceReference(StyleProperty, "OptButtonStyle");
            BasicSetBut.Children.Add(btn);

            btn = new Button();
            btn.Click += SearchQuestions_Click;
            opt = new OperationInfo();
            opt.Display = UMPApp.GetLanguageInfo("3601T00115", "Search Questions");
            opt.Icon = "Images/add.png";
            btn.DataContext = opt;
            btn.SetResourceReference(StyleProperty, "OptButtonStyle");
            BasicSetBut.Children.Add(btn);
        }

        void InitQuestionInformationList()
        {
            try
            {
                string[] lans = "3601T00016,3601T00017,3601T00018,3601T00019,3601T00020,3601T00021,3601T00022,3601T00023,3601T00024,3601T00025,3601T00026,3601T00026,3601T00026,3601T00026,3601T00026,3601T00026,3601T00027,3601T00028,3601T00029,3601T00030,3601T00031,3601T00032".Split(',');
                string[] cols = "LongNum,StrCategoryName,StrType,StrQuestionsContect,StrAnswerOne,StrAnswerTwo,StrAnswerThree,StrAnswerFour,StrAnswerFive,StrAnswerSix,CorrectAnswerOne,CorrectAnswerTwo,  CorrectAnswerThree,CorrectAnswerFour,CorrectAnswerFive, CorrectAnswerSix, IntUseNumber, StrAccessoryType, StrAccessoryName, StrAccessoryPath, StrFounderName,StrDateTime".Split(',');
                int[] colwidths = { 150,150, 60, 180, 80, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100 };
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
                ATDocument.View = columnGridView;
            }
            catch (Exception ex)
            {
                UMPApp.ShowExceptionMessage(ex.Message);
            }
        } 

        public void InitCategoryTreeInfo()
        {
            try
            {
                _mListCategoryTreeParam.Clear();
                WebRequest webRequest = new WebRequest();
                webRequest.Session = UMPApp.Session;
                webRequest.Code = (int)S3601Codes.OperationGetQuestionCategory;
                Service36011Client client = new Service36011Client(WebHelper.CreateBasicHttpBinding(App.Session),
                    WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service36011"));
                //var client = new Service36011Client();
                WebReturn webReturn = client.UmpTaskOperation(webRequest);
                client.Close();
                if( !webReturn.Result )
                    return;

                for (int i = 0; i < webReturn.ListData.Count; i++ )
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

        
        #endregion

        #region Click
        void ATDocument_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                _mListQuestionInfosTemp.Clear();
                _mCExamQuestionsTemp = new CQuestionsParam();
                CQuestionsParam itame = null;
                App.GListQuestionInfos.Clear();
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
     
                App.GListQuestionInfos = _mListQuestionInfosTemp;
                if (itame != null)
                {
                    BasicSetBut.Children.Clear();
                    DeleteSetBut.Children.Clear();
                    Button btn;
                    OperationInfo opt;
                    btn = new Button();
                    btn.Click += AddQuestions_Click;
                    opt = new OperationInfo();
                    opt.Display = UMPApp.GetLanguageInfo("3601T00002", "Create Question");
                    opt.Icon = "Images/add.png";
                    btn.DataContext = opt;
                    btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                    BasicSetBut.Children.Add(btn);

                    btn = new Button();
                    btn.Click += SearchQuestions_Click;
                    opt = new OperationInfo();
                    opt.Display = UMPApp.GetLanguageInfo("3601T00115", "Search Questions");
                    opt.Icon = "Images/add.png";
                    btn.DataContext = opt;
                    btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                    BasicSetBut.Children.Add(btn);

                    btn = new Button();
                    btn.Click += ExamineQuestion_Click;
                    opt = new OperationInfo();
                    opt.Display = UMPApp.GetLanguageInfo("3601T00003", "Browse");
                    opt.Icon = "Images/add.png";
                    btn.DataContext = opt;
                    btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                    BasicSetBut.Children.Add(btn);

                    btn = new Button();
                    btn.Click += UpDateQuestion_Click;
                    opt = new OperationInfo();
                    opt.Display = UMPApp.GetLanguageInfo("3601T00004", "Change Question");
                    opt.Icon = "Images/add.png";
                    btn.DataContext = opt;
                    btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                    BasicSetBut.Children.Add(btn);

                    btn = new Button();
                    btn.Click += DeleteQuestion_Click;
                    opt = new OperationInfo();
                    opt.Display = UMPApp.GetLanguageInfo("3601T00005", "Delete Question");
                    opt.Icon = "Images/Delete.png";
                    btn.DataContext = opt;
                    btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                    DeleteSetBut.Children.Add(btn);

                    btn = new Button();
                    btn.Click += AddToPaper_Click;
                    opt = new OperationInfo();
                    opt.Display = UMPApp.GetLanguageInfo("3601T00077", "Add To Paper");
                    opt.Icon = "Images/Delete.png";
                    btn.DataContext = opt;
                    btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                    //BasicSetBut.Children.Add(btn);

                    btn = new Button();
                    btn.Click += RemoveFromPaper_Click;
                    opt = new OperationInfo();
                    opt.Display = UMPApp.GetLanguageInfo("3601T00078", "Remove from Paper");
                    opt.Icon = "Images/Delete.png";
                    btn.DataContext = opt;
                    btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                    //BasicSetBut.Children.Add(btn);
                }
            }
            catch (Exception ex)
            {
                UMPApp.ShowExceptionMessage(ex.Message);
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
                App.GQuestionInfo.QuestionsParam = _mCExamQuestionsTemp;
                App.GQuestionInfo.OptType = S3601Codes.OperationUpdateQuestion;
                //var optItem = btn.DataContext as OperationInfo;
                CreateAddQuestionPage();
            }
        }

        private void DeleteQuestion_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(UMPApp.GetLanguageInfo("3601T00106", "Confirm delete Question?"),
                        UMPApp.GetLanguageInfo("3601T00090", "Warning"),
                        MessageBoxButton.OKCancel);
            if (result != MessageBoxResult.OK)
                return;

            foreach (CQuestionsParam param in _mListQuestionInfosTemp )
            {
                if ( param.IntUseNumber > 0)
                {
                    MessageBox.Show(UMPApp.GetLanguageInfo("3601T00008", "No questions have been used to delete!"),
                        UMPApp.GetLanguageInfo("3601T00090", "Warning"),
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
            App.GQueryModify = false;
            OptPaperPage newPage = new OptPaperPage();
            //newMainCategory.CategoryTree = m_CategoryNodeTemp;
            newPage.ParentPage = this;
            PopupPanelQuestionInfo.Content = newPage;
            App.GMessageSource = new S3601Codes();
            App.GMessageSource = S3601Codes.MessageExamproduction;
            PopupPanelQuestionInfo.IsOpen = true;
        }

        private void RemoveFromPaper_Click(object sender, RoutedEventArgs e)
        {
            App.GQueryModify = false;
            PaperRemovQuestionPage newPage = new PaperRemovQuestionPage();
            //newMainCategory.CategoryTree = m_CategoryNodeTemp;
            newPage.ParentPage = this;
            PopupPanelQuestionInfo.Content = newPage;
            App.GMessageSource = new S3601Codes();
            App.GMessageSource = S3601Codes.MessageExamproduction;
            PopupPanelQuestionInfo.IsOpen = true;
        }

        private void AddQuestions_Click(object sender, RoutedEventArgs e)
        {
            var btn = e.Source as Button;
            if (btn != null)
            {
                App.GQuestionInfo.QuestionsParam = new CQuestionsParam();
                App.GQuestionInfo.OptType = S3601Codes.OperationCreateQuestion; 
                //var optItem = btn.DataContext as OperationInfo;
                CreateAddQuestionPage();
            }
        }

        private void CreateCategory_Click( object sender, RoutedEventArgs e )
        {
            var btn = e.Source as Button;
            if (btn != null)
            {
                App.GQueryModify = false;
                NewMainCategory newMainCategory = new NewMainCategory();
                //newMainCategory.CategoryTree = m_CategoryNodeTemp;
                newMainCategory.ParentPage = this;
                PopupPanelCreateCategory.Content = newMainCategory;
                PopupPanelCreateCategory.Title = UMPApp.GetLanguageInfo("3601T00013", "Create Category");
                PopupPanelCreateCategory.IsOpen = true;
            }
        }

        private void DeleteCategory_Click(object sender, RoutedEventArgs e)
        {
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
                    webRequest.Session = UMPApp.Session;
                    webRequest.Code = (int)S3601Codes.OperationDeleteCategory;
                    Service36011Client client = new Service36011Client(WebHelper.CreateBasicHttpBinding(App.Session),
                        WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service36011"));
                    OperationReturn optReturn = XMLHelper.SeriallizeObject(papersCategoryParam);
                    if (!optReturn.Result)
                    {
                        UMPApp.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    webRequest.ListData.Add(optReturn.Data.ToString());
                    //var client = new Service36011Client();
                    WebReturn webReturn = client.UmpTaskOperation(webRequest);
                    client.Close();
                    if (!webReturn.Result)
                    {
                        UMPApp.ShowExceptionMessage(UMPApp.GetLanguageInfo("3107T00092", "Delete Failed"));
                        #region 写操作日志
                        strLog = string.Format("{0} {1}{2}", Utils.FormatOptLogString("3107T00005"), Utils.FormatOptLogString("3107T00028"), papersCategoryParam.StrName);
                        UMPApp.WriteOperationLog(S3601Consts.OPT_AutoTask.ToString(), ConstValue.OPT_RESULT_FAIL, strLog);
                        #endregion
                        return;
                    }
                    if (webReturn.Message == S3601Consts.HadUse)// 该查询条件被使用无法删除
                    {
                        UMPApp.ShowInfoMessage(UMPApp.GetLanguageInfo("3601T00011", "Can't Delete"));
                    }
                    else
                    {
                        UMPApp.ShowInfoMessage(UMPApp.GetLanguageInfo("3601T00012", "Delete Sucessed"));
                        InitCategoryTreeInfo();
                    }
                }
            }
            catch (Exception ex)
            {
                UMPApp.ShowExceptionMessage(ex.Message);
            }
        }

        private void SearchQuestions_Click(object sender, RoutedEventArgs e)
        {
            App.GQueryModify = false;
            SearchQuestionPage newPage = new SearchQuestionPage();
            newPage.CategoryTree.ItemsSource = _mCategoryNode.Children;
            newPage.ParentPage = this;
            PopupPanelSearchQuestion.Content = newPage;
            PopupPanelSearchQuestion.Title = UMPApp.GetLanguageInfo("3601T00115", "Search Question");
            PopupPanelSearchQuestion.IsOpen = true;
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
                Button btn;
                OperationInfo opt;
                btn = new Button();
                btn.Click += AddQuestions_Click;
                opt = new OperationInfo();
                opt.Display = UMPApp.GetLanguageInfo("3601T00002", "Create Question");
                opt.Icon = "Images/add.png";
                btn.DataContext = opt;
                btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                BasicSetBut.Children.Add(btn);

                btn = new Button();
                btn.Click += CreateCategory_Click;
                opt = new OperationInfo();
                opt.Display = UMPApp.GetLanguageInfo("3601T00013", "Add Create");
                opt.Icon = "Images/add.png";
                btn.DataContext = opt;
                btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                BasicSetBut.Children.Add(btn);

                btn = new Button();
                btn.Click += SearchQuestions_Click;
                opt = new OperationInfo();
                opt.Display = UMPApp.GetLanguageInfo("3601T00115", "Search Questions");
                opt.Icon = "Images/add.png";
                btn.DataContext = opt;
                btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                BasicSetBut.Children.Add(btn);

                if (nodeInfo.LongParentNodeId != 0)
                {
                    btn = new Button();
                    btn.Click += DeleteCategory_Click;
                    opt = new OperationInfo();
                    opt.Display = UMPApp.GetLanguageInfo("3601T00014", "Add Create");
                    opt.ID = 31070021;
                    opt.Icon = "Images/Delete.png";
                    btn.DataContext = opt;
                    btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                    DeleteSetBut.Children.Add(btn);
                }
                
                _mCategoryNodeTemp = nodeInfo;
                App.GCategoryTree = nodeInfo;

                GetQuestionInfos();
            }
            catch (Exception ex)
            {
                UMPApp.ShowExceptionMessage(ex.Message);
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
                    opt.Display = UMPApp.GetLanguageInfo("3601T00002", "Create Question");
                    opt.Icon = "Images/add.png";
                    btn.DataContext = opt;
                    btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                    BasicSetBut.Children.Add(btn);

                    btn = new Button();
                    btn.Click += SearchQuestions_Click;
                    opt = new OperationInfo();
                    opt.Display = UMPApp.GetLanguageInfo("3601T00115", "Search Questions");
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
                    UMPApp.ShowExceptionMessage(ex.Message);
                }
            }  
        }

        #endregion

        #region Operations
        public void RefreshTree()
        {
            InitCategoryTreeInfo();
        }

        public void UpdateQuestion()
        {
            CreateAddQuestionPage();
        }

        public void AddPaperList()
        {
            if ( App.GPaperInfo.OptType == S3601Codes.OperationAddPaper )
            {
                _mAddPaperInfo = App.GPaperInfo;
                _mListAddPapers.Add(_mAddPaperInfo.PaperParam);
            }
            else
            {
                _mListAddPapers.Remove(
                    _mListAddPapers.FirstOrDefault(p => p.LongNum == App.GPaperInfo.PaperParam.LongNum));
                _mListAddPapers.Add(App.GPaperInfo.PaperParam);
            }
        }

        private void CreateAddQuestionPage()
        {
            //App.ShowInfoMessage(string.Format("Create ScoreSheet"));
            try
            {
                App.IsModifyScoreSheet = false;
                if (NavigationService != null)
                    NavigationService.Navigate(new Uri("CreateQuestionsPage.xaml", UriKind.Relative));
            }
            catch (Exception ex)
            {
                UMPApp.ShowExceptionMessage(ex.Message);
            }
        }

        private void CreateEditPaperPage()
        {
            //App.ShowInfoMessage(string.Format("Create ScoreSheet"));
            try
            {
                App.IsModifyScoreSheet = false;
                if (NavigationService != null)
                    NavigationService.Navigate(new Uri("EditPaperPage.xaml", UriKind.Relative));
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
                        panel.Title = UMPApp.GetLanguageInfo("3601T00034", "Question Category");

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

        private void GetQuestionInfos()
        {
            try
            {
                _mlistQuestionsParams.Clear();
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
                    UMPApp.ShowExceptionMessage(UMPApp.GetLanguageInfo("3601T00076", "Insert data failed"));
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

                    _mlistQuestionsParams.Add(cExamQuestions);
                }
                SetObservableCollection();
            }
            catch (Exception ex)
            {
                UMPApp.ShowExceptionMessage(ex.Message);
            }
        }

        private bool DeleteQuestionInfos()
        {
            string strLog = null;
            try
            {
                var webRequest = new WebRequest();
                webRequest.Session = UMPApp.Session;
                webRequest.Code = (int)S3601Codes.OperationDeleteQuestion;
                //Service36011Client client = new Service36011Client(WebHelper.CreateBasicHttpBinding(App.Session),
                //    WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service36011"));
                OperationReturn optReturn = XMLHelper.SeriallizeObject(_mListQuestionInfosTemp);
                if (!optReturn.Result)
                {
                    UMPApp.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return false ;
                }
                webRequest.ListData.Add(optReturn.Data.ToString());
                var client = new Service36011Client();
                WebReturn webReturn = client.UmpTaskOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    UMPApp.ShowExceptionMessage(UMPApp.GetLanguageInfo("3107T00092", "Delete Failed"));
                    #region 写操作日志
                    //strLog = string.Format("{0} {1}{2}", Utils.FormatOptLogString("3107T00005"), Utils.FormatOptLogString("3107T00028"), papersCategoryParam.StrName);
                    UMPApp.WriteOperationLog(S3601Consts.OPT_AutoTask.ToString(), ConstValue.OPT_RESULT_FAIL, strLog);
                    #endregion
                    return false ;
                }
                if (webReturn.Message == S3601Consts.HadUse)// 该查询条件被使用无法删除
                {
                    UMPApp.ShowInfoMessage(UMPApp.GetLanguageInfo("3601T00011", "Can't Delete"));
                    return false ;
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

        public void SetBusy(bool isBusying)
        {
            MyWaiter.Visibility = isBusying ? Visibility.Visible : Visibility.Collapsed;
        }

        public void OpenEditPaperPage()
        {
            CreateEditPaperPage();
        }

        public void SearchQuestions()
        {
            _mlistQuestionsParams.Clear();
            foreach (var param in App.GListQuestionInfos)
            {
                _mlistQuestionsParams.Add(param);
            }
            SetObservableCollection();
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
                UMPApp.WriteLog("UMPFolderPath", _mUmpFileRootPath);
            }
            catch (Exception ex)
            {
                UMPApp.ShowExceptionMessage(ex.Message);
            }
        }

        private void ExamineQuestion()
        {
            App.GQueryModify = false;
            BrowseQustions browseQustions = new BrowseQustions();
            browseQustions.EpParentPage = this;
            PopupPanelQuestionInfo.Content = browseQustions;
            PopupPanelQuestionInfo.Title = UMPApp.GetLanguageInfo("3601T00117", "Question Information");
            App.GMessageSource = new S3601Codes();
            App.GMessageSource = S3601Codes.MessageExamproduction;
            PopupPanelQuestionInfo.IsOpen = true;
        }

        private void SetObservableCollection()
        {
            _mListExamQuestions.Clear();
            foreach (var questionsParam in _mlistQuestionsParams)
            {
                switch ((QuestionType)questionsParam.IntType)
                {
                    case QuestionType.Trueorfalse:
                        questionsParam.StrType = UMPApp.GetLanguageInfo("3601T00098", "True Or False");
                        break;
                    case QuestionType.SingleChoice:
                        questionsParam.StrType = UMPApp.GetLanguageInfo("3601T00097", "Single Choice");
                        break;
                    case QuestionType.MultipleChioce:
                        questionsParam.StrType = UMPApp.GetLanguageInfo("3601T00099", "Multiple Chioce");
                        break;
                }


                _mListExamQuestions.Add(questionsParam);
            }
        }

        #endregion

        
    }
}
