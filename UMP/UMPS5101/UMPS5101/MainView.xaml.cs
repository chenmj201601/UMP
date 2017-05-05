using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection.Emit;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Common5101;
using UMPS5101.Models;
using UMPS5101.Wcf51011;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;

namespace UMPS5101
{
    /// <summary>
    /// MainView.xaml 的交互逻辑
    /// </summary>
    public partial class MainView
    {
        private const int MEnable = 1;
        private const int MClose = 0;
        private const int MDelete = 3;

        #region Members

        private BackgroundWorker _mWorker;
        private readonly ObservableCollection<KeywordInfoParam> _mObservableKwInfo;
        private readonly ObservableCollection<KwContentInfoParam> _mObservableKwConnectInfo;
        private readonly ObservableCollection<KwContentInfoParam> _mObservableShowKwConInfo;
        private List<KwContentInfoParam> _mLstKwContents;
        private List<KeywordInfoParam> _mLstKeywords;
        private KwContentInfoParam _mKwContentTemp;
        private bool _mbKeywordTable = false;
        private bool _mbKwContentTable = true;
        private SelectKwContentParam _mSelectKwContent;
        private List<KwContentInfoParam> _mlstConnectInfo;

        private int _mKwConPageIndex;//页的索引,这个是从0开始算的
        private bool _mbListKwConViewEnable;
        private int _mKwConNum;
        private string _mKwConPageNum;
        private int _mMaxKwConInfos;
        private int _mKwConPageSize;
        private int _mKwConPageCount;

        private int _mKwPageIndex;//页的索引,这个是从0开始算的
        private bool _mbListKwViewEnable;
        private int _mKwNum;
        private string _mKwPageNum;
        private int _mMaxKwInfos;
        private int _mKwPageSize;
        private int _mKwPageCount;
        #endregion

        public MainView()
        {
            InitializeComponent();
            _mObservableKwInfo = new ObservableCollection<KeywordInfoParam>();
            _mObservableKwConnectInfo = new ObservableCollection<KwContentInfoParam>();
            _mLstKwContents = new List<KwContentInfoParam>();
            _mKwContentTemp = new KwContentInfoParam();
            _mSelectKwContent = new SelectKwContentParam();
            _mLstKeywords = new List<KeywordInfoParam>();
            _mlstConnectInfo = new List<KwContentInfoParam>();
            _mObservableShowKwConInfo = new ObservableCollection<KwContentInfoParam>();

            _mKwConPageIndex = 0;
            _mKwConNum = 0;
            _mMaxKwConInfos = 100000;
            _mKwConPageSize = 200;
            _mKwConPageCount = 0;

            _mKwPageIndex = 0;
            _mKwNum = 0;
            _mMaxKwInfos = 100000;
            _mKwPageSize = 200;
            _mKwPageCount = 0;
        }

        #region 初始化 & 全局消息

        protected override void Init()
        {
            try
            {
                PageName = "KeywordConfig";
                StylePath = "UMPS5101/MainPageStyle.xaml";
                base.Init();

                CurrentApp.SendLoadedMessage();
                ChangeLanguage();
                ListInfoLoaded();
                CreateKwConPageButtons();
                CreateKwPageButtons();
                InitKwListTable();
                InitKwContentListTable();
                InitShowKwConListTable();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        void ListInfoLoaded()
        {
            _mObservableKwInfo.Clear();
            _mObservableKwConnectInfo.Clear();
            try
            {
                SetBusy(true, CurrentApp.GetMessageLanguageInfo("001", "Loading data, please wait..."));
                _mWorker = new BackgroundWorker
                {
                    WorkerReportsProgress = true,
                    WorkerSupportsCancellation = true
                };
                //注册线程主体方法
                _mWorker.DoWork += (s, de) =>
                {

                };
                _mWorker.RunWorkerCompleted += (s, re) =>
                {
                    ChangeLanguage();
                    KwListView.ItemsSource = _mObservableKwInfo;
                    KwContentListView.ItemsSource = _mObservableKwConnectInfo;
                    ShowKwConListView.ItemsSource = _mObservableShowKwConInfo;
                    _mbListKwConViewEnable = true;
                    _mbListKwViewEnable = true;
                    GetKwContents(SetSql_5107());
                    GetKeywords(SetSql_5106());
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

        private void CreateKwConPageButtons()
        {
            try
            {
                List<CToolButtonItem> listBtns = new List<CToolButtonItem>();
                CToolButtonItem item = new CToolButtonItem();
                item.Name = "TB" + "FirstPage";
                item.Display = CurrentApp.GetLanguageInfo("5101T00041", "First Page");
                item.Tip = CurrentApp.GetLanguageInfo("5101T00041", "First Page");
                item.Icon = "Images/first.ico";
                listBtns.Add(item);
                item = new CToolButtonItem();
                item.Name = "TB" + "PrePage";
                item.Display = CurrentApp.GetLanguageInfo("5101T00042", "Pre Page");
                item.Tip = CurrentApp.GetLanguageInfo("5101T00042", "Pre Page");
                item.Icon = "Images/pre.ico";
                listBtns.Add(item);
                item = new CToolButtonItem();
                item.Name = "TB" + "NextPage";
                item.Display = CurrentApp.GetLanguageInfo("5101T00043", "Next Page");
                item.Tip = CurrentApp.GetLanguageInfo("5101T00043", "Next Page");
                item.Icon = "Images/next.ico";
                listBtns.Add(item);
                item = new CToolButtonItem();
                item.Name = "TB" + "LastPage";
                item.Display = CurrentApp.GetLanguageInfo("5101T00044", "Last Page");
                item.Tip = CurrentApp.GetLanguageInfo("5101T00044", "Last Page");
                item.Icon = "Images/last.ico";
                listBtns.Add(item);

                PanelKwConPageButtons.Children.Clear();
                for (int i = 0; i < listBtns.Count; i++)
                {
                    CToolButtonItem toolBtn = listBtns[i];
                    Button btn = new Button();
                    btn.DataContext = toolBtn;
                    btn.SetResourceReference(StyleProperty, "ToolBarButtonStyle");
                    btn.Click += KwConPageButton_Click;
                    PanelKwConPageButtons.Children.Add(btn);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void CreateKwPageButtons()
        {
            try
            {
                List<CToolButtonItem> listBtns = new List<CToolButtonItem>();
                CToolButtonItem item = new CToolButtonItem();
                item.Name = "TB" + "FirstPage";
                item.Display = CurrentApp.GetLanguageInfo("5101T00041", "First Page");
                item.Tip = CurrentApp.GetLanguageInfo("5101T00041", "First Page");
                item.Icon = "Images/first.ico";
                listBtns.Add(item);
                item = new CToolButtonItem();
                item.Name = "TB" + "PrePage";
                item.Display = CurrentApp.GetLanguageInfo("5101T00042", "Pre Page");
                item.Tip = CurrentApp.GetLanguageInfo("5101T00042", "Pre Page");
                item.Icon = "Images/pre.ico";
                listBtns.Add(item);
                item = new CToolButtonItem();
                item.Name = "TB" + "NextPage";
                item.Display = CurrentApp.GetLanguageInfo("5101T00043", "Next Page");
                item.Tip = CurrentApp.GetLanguageInfo("5101T00043", "Next Page");
                item.Icon = "Images/next.ico";
                listBtns.Add(item);
                item = new CToolButtonItem();
                item.Name = "TB" + "LastPage";
                item.Display = CurrentApp.GetLanguageInfo("5101T00044", "Last Page");
                item.Tip = CurrentApp.GetLanguageInfo("5101T00044", "Last Page");
                item.Icon = "Images/last.ico";
                listBtns.Add(item);

                PanelKwPageButtons.Children.Clear();
                for (int i = 0; i < listBtns.Count; i++)
                {
                    CToolButtonItem toolBtn = listBtns[i];
                    Button btn = new Button();
                    btn.DataContext = toolBtn;
                    btn.SetResourceReference(StyleProperty, "ToolBarButtonStyle");
                    btn.Click += KwPageButton_Click;
                    PanelKwPageButtons.Children.Add(btn);
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
                    ResourceDictionary resource = new ResourceDictionary
                    {
                        Source = new Uri(uri, UriKind.RelativeOrAbsolute)
                    };
                    Resources.MergedDictionaries.Add(resource);
                    bPage = true;
                }
                catch (Exception)
                {
                    //S5101App.ShowExceptionMessage("1" + ex.Message);
                }
            }
            if (!bPage)
            {
                //如果通过Url没有加载成功，就从已经编译到程序集的默认资源加载
                try
                {
                    string uri = string.Format("/UMPS5101;component/Themes/{0}/{1}",
                        "Default"
                        , StylePath);
                    ResourceDictionary resource = new ResourceDictionary
                    {
                        Source = new Uri(uri, UriKind.RelativeOrAbsolute)
                    };
                    Resources.MergedDictionaries.Add(resource);
                }
                catch (Exception)
                {
                    //S5101App.ShowExceptionMessage("2" + ex.Message);
                }
            }

            try
            {
                string uri = "/UMPS5101;component/Themes/Default/UMPS5101/AvalonDock.xaml";
                ResourceDictionary resource = new ResourceDictionary
                {
                    Source = new Uri(uri, UriKind.RelativeOrAbsolute)
                };
                Resources.MergedDictionaries.Add(resource);
            }
            catch (Exception)
            {
                //S5101App.ShowExceptionMessage("2" + ex.Message);
            }

            try
            {
                string uri = "/UMPS5101;component/Themes/Default/UMPS5101/MainStatic.xaml";
                ResourceDictionary resource = new ResourceDictionary
                {
                    Source = new Uri(uri, UriKind.RelativeOrAbsolute)
                };
                Resources.MergedDictionaries.Add(resource);
            }
            catch (Exception)
            {
                //S5101App.ShowExceptionMessage("2" + ex.Message);
            }
        }

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();
            PageName = "KeywordConfig";
            CurrentApp.AppTitle = CurrentApp.GetLanguageInfo("5101T00001", "Keyword Config");
            BasicExpandOpt.Header = CurrentApp.GetLanguageInfo("5101T00020", "Content Operation");
            KeywordTable.Title = CurrentApp.GetLanguageInfo("5101T00007", "Keyword Informatica");
            KeywordConnectTable.Title = CurrentApp.GetLanguageInfo("5101T00008", "Keyword Connect");
            InitKwContentButton();
        }

        void InitKwButton()
        {
            ButBasicOpt.Children.Clear();
            ButDeleteOpt.Children.Clear();
            ButEditOpt.Children.Clear();
            ButEnableOpt.Children.Clear();
            var btn = new Button();
            btn.Click += AddKeyword_Click;
            var opt = new OperationInfo
            {
                Display = CurrentApp.GetLanguageInfo("5101T00002", "Add"),
                Icon = "Images/add.png"
            };
            btn.DataContext = opt;
            btn.SetResourceReference(StyleProperty, "OptButtonStyle");
            ButBasicOpt.Children.Add(btn);

            btn = new Button();
            btn.Click += ChangeKeyword_Click;
            opt = new OperationInfo
            {
                Display = CurrentApp.GetLanguageInfo("5101T00004", "Change"),
                Icon = "Images/add.png"
            };
            btn.DataContext = opt;
            btn.SetResourceReference(StyleProperty, "OptButtonStyle");
            ButBasicOpt.Children.Add(btn);

            btn = new Button();
            btn.Click += SelectKw_Click;
            opt = new OperationInfo
            {
                Display = CurrentApp.GetLanguageInfo("5101T00033", "Select"),
                Icon = "Images/search.png"
            };
            btn.DataContext = opt;
            btn.SetResourceReference(StyleProperty, "OptButtonStyle");
            ButBasicOpt.Children.Add(btn);
        }

        void InitEditContentBut()
        {
            ButEditOpt.Children.Clear();
            var btn = new Button();
            btn.Click += EditKeyword_Click;
            var opt = new OperationInfo
            {
                Display = CurrentApp.GetLanguageInfo("5101T00006", "Edit Connect"),
                Icon = "Images/add.png"
            };
            btn.DataContext = opt;
            btn.SetResourceReference(StyleProperty, "OptButtonStyle");
            ButEditOpt.Children.Add(btn);
        }

        void InitKwContentEnableBut()
        {
            ButEnableOpt.Children.Clear();
            var btn = new Button();
            btn.Click += EnableKwContent_Click;
            var opt = new OperationInfo
            {
                Display = CurrentApp.GetLanguageInfo("5101T00005", "Enable"),
                Icon = "Images/add.png"
            };
            btn.DataContext = opt;
            btn.SetResourceReference(StyleProperty, "OptButtonStyle");
            ButEnableOpt.Children.Add(btn);
        }

        void InitKwEnableBut()
        {
            ButEnableOpt.Children.Clear();
            var btn = new Button();
            btn.Click += EnableKeyword_Click;
            var opt = new OperationInfo
            {
                Display = CurrentApp.GetLanguageInfo("5101T00005", "Enable"),
                Icon = "Images/add.png"
            };
            btn.DataContext = opt;
            btn.SetResourceReference(StyleProperty, "OptButtonStyle");
            ButEnableOpt.Children.Add(btn);
        }

        void InitKwContentCloseBut()
        {
            ButEnableOpt.Children.Clear();
            var btn = new Button();
            btn.Click += EnableKwContent_Click;
            var opt = new OperationInfo
            {
                Display = CurrentApp.GetLanguageInfo("5101T00048", "Close"),
                Icon = "Images/add.png"
            };
            btn.DataContext = opt;
            btn.SetResourceReference(StyleProperty, "OptButtonStyle");
            ButEnableOpt.Children.Add(btn);
        }

        void InitKwCloseBut()
        {
            ButEnableOpt.Children.Clear();
            var btn = new Button();
            btn.Click += EnableKeyword_Click;
            var opt = new OperationInfo
            {
                Display = CurrentApp.GetLanguageInfo("5101T00048", "Close"),
                Icon = "Images/add.png"
            };
            btn.DataContext = opt;
            btn.SetResourceReference(StyleProperty, "OptButtonStyle");
            ButEnableOpt.Children.Add(btn);
        }

        void InitKwContentDeleteBut()
        {
            ButDeleteOpt.Children.Clear();
            var btn = new Button();
            btn.Click += DeleteKwContent_Click;
            var opt = new OperationInfo
            {
                Display = CurrentApp.GetLanguageInfo("5101T00019", "Delete Connect"),
                Icon = "Images/Delete.png"
            };
            btn.DataContext = opt;
            btn.SetResourceReference(StyleProperty, "OptButtonStyle");
            ButDeleteOpt.Children.Add(btn);
        }

        void InitKwDeleteBut()
        {
            ButDeleteOpt.Children.Clear();
            var btn = new Button();
            btn.Click += DeleteKeyword_Click;
            var opt = new OperationInfo
            {
                Display = CurrentApp.GetLanguageInfo("5101T00019", "Delete keyword"),
                Icon = "Images/Delete.png"
            };
            btn.DataContext = opt;
            btn.SetResourceReference(StyleProperty, "OptButtonStyle");
            ButDeleteOpt.Children.Add(btn);
        }

        void InitKwContentRestoreBut()
        {
            ButEnableOpt.Children.Clear();
            ButDeleteOpt.Children.Clear();
            ButBasicOpt.Children.Clear();

            var btn = new Button();
            btn.Click += SelectKw_Click;
            var opt = new OperationInfo
            {
                Display = CurrentApp.GetLanguageInfo("5101T00033", "Select"),
                Icon = "Images/search.png"
            };
            btn.DataContext = opt;
            btn.SetResourceReference(StyleProperty, "OptButtonStyle");
            ButBasicOpt.Children.Add(btn);

            btn = new Button();
            btn.Click += DeleteKwContent_Click;
            opt = new OperationInfo
            {
                Display = CurrentApp.GetLanguageInfo("5101T00056", "Restore"),
                Icon = "Images/add.png"
            };
            btn.DataContext = opt;
            btn.SetResourceReference(StyleProperty, "OptButtonStyle");
            ButDeleteOpt.Children.Add(btn);
        }

        void InitKwRestoreBut()
        {
            ButBasicOpt.Children.Clear();
            ButEditOpt.Children.Clear();
            ButEnableOpt.Children.Clear();
            ButDeleteOpt.Children.Clear();

            var btn = new Button();
            btn.Click += SelectKw_Click;
            var opt = new OperationInfo
            {
                Display = CurrentApp.GetLanguageInfo("5101T00033", "Select"),
                Icon = "Images/search.png"
            };
            btn.DataContext = opt;
            btn.SetResourceReference(StyleProperty, "OptButtonStyle");
            ButDeleteOpt.Children.Add(btn);

            btn = new Button();
            btn.Click += DeleteKeyword_Click;
            opt = new OperationInfo
            {
                Display = CurrentApp.GetLanguageInfo("5101T00056", "Restore"),
                Icon = "Images/add.png"
            };
            btn.DataContext = opt;
            btn.SetResourceReference(StyleProperty, "OptButtonStyle");
            ButDeleteOpt.Children.Add(btn);
        }

        void InitKwContentButton()
        {
            ButDeleteOpt.Children.Clear();
            ButEditOpt.Children.Clear();
            ButEnableOpt.Children.Clear();
            ButBasicOpt.Children.Clear();
            var btn = new Button();
            btn.Click += AddKwContent_Click;
            var opt = new OperationInfo
            {
                Display = CurrentApp.GetLanguageInfo("5101T00002", "Add"),
                Icon = "Images/add.png"
            };
            btn.DataContext = opt;
            btn.SetResourceReference(StyleProperty, "OptButtonStyle");
            ButBasicOpt.Children.Add(btn);

            btn = new Button();
            btn.Click += ChangeKwContent_Click;
            opt = new OperationInfo
            {
                Display = CurrentApp.GetLanguageInfo("5101T00004", "Change"),
                Icon = "Images/add.png"
            };
            btn.DataContext = opt;
            btn.SetResourceReference(StyleProperty, "OptButtonStyle");
            ButBasicOpt.Children.Add(btn);

            btn = new Button();
            btn.Click += SelectKwContent_Click;
            opt = new OperationInfo
            {
                Display = CurrentApp.GetLanguageInfo("5101T00033", "Select"),
                Icon = "Images/search.png"
            };
            btn.DataContext = opt;
            btn.SetResourceReference(StyleProperty, "OptButtonStyle");
            ButBasicOpt.Children.Add(btn);
        }

        void InitKwListTable()
        {
            try
            {
                string[] lans =
                    "5101T00010,5101T00018,5101T00011,5101T00012,5101T00013,5101T00014,5101T00015".Split(',');
                string[] cols =
                    "State,StrImage,StrKw,StrAddLocalTime,StrAddPaperName,StrChangeLocalTime,StrChangePaperName"
                        .Split(',');
                int[] colwidths = {70, 70, 150, 150, 150, 150, 150};
                var columnGridView = new GridView();
                for (int i = 0; i < cols.Length; i++)
                {
                    DataTemplate dt;
                    var gvc = new GridViewColumn();
                    if (cols[i] == "State")
                    {
                        dt = Resources["CellStateTemplate"] as DataTemplate;
                        if (dt != null)
                        {
                            gvc.CellTemplate = dt;
                        }
                        else
                        {
                            gvc.DisplayMemberBinding = new Binding(cols[0]);
                        }
                    }
                    else if (cols[i] == "StrImage")
                    {
                        dt = Resources["CellStrImageTemplate"] as DataTemplate;
                        if (dt != null)
                        {
                            gvc.CellTemplate = dt;
                        }
                        else
                        {
                            gvc.DisplayMemberBinding = new Binding(cols[0]);
                        }
                    }
                    else
                    {
                        gvc.DisplayMemberBinding = new Binding(cols[i]);
                    }
                    gvc.Header = CurrentApp.GetLanguageInfo(lans[i], cols[i]);
                    gvc.Width = colwidths[i];
                    columnGridView.Columns.Add(gvc);
                }
                KwListView.View = columnGridView;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        void SetSelectKwListTable()
        {
            try
            {
                string[] lans =
                    "5101T00010,5101T00051,5101T00018,5101T00011,5101T00012,5101T00013,5101T00014,5101T00015,5101T00016,5101T00017,5101T00058,5101T00059".Split(',');
                string[] cols =
                    "State,StrDeleteState,StrImage,StrKw,StrAddLocalTime,StrAddPaperName,StrChangeLocalTime,StrChangePaperName,StrDeleteLocalTime,StrDeletePaperName,StrRestoreLocalTime,StrRestorePaperName"
                        .Split(',');
                int[] colwidths = { 70, 100, 70, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150 };
                var columnGridView = new GridView();
                for (int i = 0; i < cols.Length; i++)
                {
                    DataTemplate dt;
                    var gvc = new GridViewColumn();
                    if (cols[i] == "State")
                    {
                        dt = Resources["CellStateTemplate"] as DataTemplate;
                        if (dt != null)
                        {
                            gvc.CellTemplate = dt;
                        }
                        else
                        {
                            gvc.DisplayMemberBinding = new Binding(cols[0]);
                        }
                    }
                    else if (cols[i] == "StrImage")
                    {
                        dt = Resources["CellStrImageTemplate"] as DataTemplate;
                        if (dt != null)
                        {
                            gvc.CellTemplate = dt;
                        }
                        else
                        {
                            gvc.DisplayMemberBinding = new Binding(cols[0]);
                        }
                    }
                    else
                    {
                        gvc.DisplayMemberBinding = new Binding(cols[i]);
                    }
                    gvc.Header = CurrentApp.GetLanguageInfo(lans[i], cols[i]);
                    gvc.Width = colwidths[i];
                    columnGridView.Columns.Add(gvc);
                }
                KwListView.View = columnGridView;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        void InitShowKwConListTable()
        {
            try
            {
                string[] lans = "5101T00037".Split(',');
                string[] cols = "StrKwContent".Split(',');
                int[] colwidths = {300};
                var columnGridView = new GridView();
                for (int i = 0; i < cols.Length; i++)
                {
                    var gvc = new GridViewColumn();
                    gvc.DisplayMemberBinding = new Binding(cols[i]);
                    gvc.Header = CurrentApp.GetLanguageInfo(lans[i], cols[i]);
                    gvc.Width = colwidths[i];
                    columnGridView.Columns.Add(gvc);
                }
                ShowKwConListView.View = columnGridView;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        void SetSelectKwContentListTable()
        {
            try
            {
                string[] lans = "5101T00010,5101T00051,5101T00008,5101T00012,5101T00013,5101T00014,5101T00015,5101T00016,5101T00017,5101T00058,5101T00059".Split(',');
                string[] cols =
                    "State,StrDeleteState,StrKwContent,StrAddLocalTime,StrAddPaperName,StrChangeLocalTime,StrChangePaperName,StrDeleteLocalTime,StrDeletePaperName,StrRestoreLocalTime,StrRestorePaperName"
                        .Split(',');
                int[] colwidths = { 70, 100, 150, 150, 150, 150, 150, 150, 150, 150, 150, 150 };
                var columnGridView = new GridView();
                for (int i = 0; i < cols.Length; i++)
                {
                    DataTemplate dt;
                    var gvc = new GridViewColumn();
                    if (cols[i] == "State")
                    {
                        dt = Resources["CellStateTemplate"] as DataTemplate;
                        if (dt != null)
                        {
                            gvc.CellTemplate = dt;
                        }
                        else
                        {
                            gvc.DisplayMemberBinding = new Binding(cols[0]);
                        }
                    }
                    else
                    {
                        gvc.DisplayMemberBinding = new Binding(cols[i]);
                    }
                    gvc.Header = CurrentApp.GetLanguageInfo(lans[i], cols[i]);
                    gvc.Width = colwidths[i];
                    columnGridView.Columns.Add(gvc);
                }
                KwContentListView.View = columnGridView;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        void InitKwContentListTable()
        {
            try
            {
                string[] lans = "5101T00010,5101T00008,5101T00012,5101T00013,5101T00014,5101T00015".Split(',');
                string[] cols =
                    "State,StrKwContent,StrAddLocalTime,StrAddPaperName,StrChangeLocalTime,StrChangePaperName"
                        .Split(',');
                int[] colwidths = {70, 150, 150, 150, 150, 150};
                var columnGridView = new GridView();
                for (int i = 0; i < cols.Length; i++)
                {
                    DataTemplate dt;
                    var gvc = new GridViewColumn();
                    if (cols[i] == "State")
                    {
                        dt = Resources["CellStateTemplate"] as DataTemplate;
                        if (dt != null)
                        {
                            gvc.CellTemplate = dt;
                        }
                        else
                        {
                            gvc.DisplayMemberBinding = new Binding(cols[0]);
                        }
                    }
                    else
                    {
                        gvc.DisplayMemberBinding = new Binding(cols[i]);
                    }
                    gvc.Header = CurrentApp.GetLanguageInfo(lans[i], cols[i]);
                    gvc.Width = colwidths[i];
                    columnGridView.Columns.Add(gvc);
                }
                KwContentListView.View = columnGridView;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion

        #region click

        void AddKwContent_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                S5101App.GQueryModify = false;
                S5101App.GOptInfo = S5101Codes.OptAdd;
                AddKwContentPage newPage = new AddKwContentPage
                {
                    ParentPage = this,
                    CurrentApp = CurrentApp
                };
                PopupPanel.Content = newPage;
                PopupPanel.Title = CurrentApp.GetLanguageInfo("5101T00029", "Add Content");
                PopupPanel.IsOpen = true;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        void ChangeKwContent_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var item = KwContentListView.SelectedItem as KwContentInfoParam;
                if (item != null)
                {
                    S5101App.GKwConnectInfo = item;
                    S5101App.GQueryModify = false;
                    S5101App.GOptInfo = S5101Codes.OptChange;
                    AddKwContentPage newPage = new AddKwContentPage
                    {
                        ParentPage = this,
                        CurrentApp = CurrentApp
                    };
                    PopupPanel.Content = newPage;
                    PopupPanel.Title = CurrentApp.GetLanguageInfo("5101T00030", "Change Keyword Content");
                    PopupPanel.IsOpen = true;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        void SelectKwContent_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                S5101App.GQueryModify = false;
                S5101App.GOptInfo = S5101Codes.OptSelect;
                SelectKwContentPage newPage = new SelectKwContentPage()
                {
                    ParentPage = this,
                    CurrentApp = CurrentApp
                };
                PopupPanel.Content = newPage;
                PopupPanel.Title = CurrentApp.GetLanguageInfo("5101T00035", "Select Content");
                PopupPanel.IsOpen = true;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        void EnableKwContent_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var item = KwContentListView.SelectedItem as KwContentInfoParam;
                if (item != null)
                {
                    item.State = item.State == 0 ? 1 : 0;
                    var param = _mLstKwContents.Where(p => p.LongKwContentNum == item.LongKwContentNum).FirstOrDefault();
                    param = item;
                    SetKwContentTable();
                    if (item.State == 0)
                    {
                        InitKwContentEnableBut();
                    }
                    else
                    {
                        InitKwContentCloseBut();
                    }

                    UpdateKwContent(param);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        void DeleteKwContent_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var item = KwContentListView.SelectedItem as KwContentInfoParam;
                if (item != null)
                {
                    MessageBoxResult result;
                    if (item.State == MDelete)
                    {
                        result = MessageBox.Show(CurrentApp.GetLanguageInfo("5101T00057",
                        "Sure to restore to delete keywords content?"),
                        CurrentApp.GetLanguageInfo("5101T00050", "Warning"),
                        MessageBoxButton.OKCancel);
                    }
                    else
                    {
                        result = MessageBox.Show(CurrentApp.GetLanguageInfo("5101T00049",
                        "Sure you want to delete?"),
                        CurrentApp.GetLanguageInfo("5101T00050", "Warning"),
                        MessageBoxButton.OKCancel);
                    }
                    if (result == MessageBoxResult.Cancel)
                    {
                        return;
                    }

                    if (item.State == MDelete)
                    {
                        S5101App.GOptInfo = S5101Codes.OptRestore;
                        item.State = 0;
                        item.IntDelete = 0;
                        var localtime = DateTime.Now;
                        var utc = localtime.ToUniversalTime();
                        item.StrRestoreUtcTime = utc.ToString("yyyy-MM-dd HH:mm:ss");
                        item.StrRestoreLocalTime = localtime.ToString("yyyy-MM-dd HH:mm:ss");
                        item.StrRestorePaperName = CurrentApp.Session.UserInfo.UserName;
                        item.LongRestorePaperNum = CurrentApp.Session.UserInfo.UserID;
                    }
                    else
                    {
                        S5101App.GOptInfo = S5101Codes.OptDelete;
                        item.State = MDelete;
                        item.IntDelete = 1;
                        var localtime = DateTime.Now;
                        var utc = localtime.ToUniversalTime();
                        item.StrDeleteUtcTime = utc.ToString("yyyy-MM-dd HH:mm:ss");
                        item.StrDeleteLocalTime = localtime.ToString("yyyy-MM-dd HH:mm:ss");
                        item.StrDeletePaperName = CurrentApp.Session.UserInfo.UserName;
                        item.LongDeletePaperNum = CurrentApp.Session.UserInfo.UserID;
                    }
                    if (DeleteKwContent(item))
                    {
                        KwContentInfoParam param = new KwContentInfoParam();
                        param = _mLstKwContents.Where(p => p.LongKwContentNum == item.LongKwContentNum).FirstOrDefault();
                        if (S5101App.GOptInfo == S5101Codes.OptRestore)
                        {
                            param = item;
                        }
                        else
                        {

                            _mLstKwContents.Remove(param);
                        }

                        SetKwContentTable();
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        void AddKeyword_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                S5101App.GQueryModify = false;
                S5101App.GOptInfo = S5101Codes.OptAdd;
                AddKeywordPage newPage = new AddKeywordPage
                {
                    ParentPage = this,
                    CurrentApp = CurrentApp
                };
                PopupPanel.Content = newPage;
                PopupPanel.Title = CurrentApp.GetLanguageInfo("5101T00009", "Add Keyword");
                PopupPanel.IsOpen = true;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        void ChangeKeyword_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var item = KwListView.SelectedItem as KeywordInfoParam;
                if (item != null)
                {
                    S5101App.GKwInfo = item;
                    S5101App.GQueryModify = false;
                    S5101App.GOptInfo = S5101Codes.OptChange;
                    AddKeywordPage newPage = new AddKeywordPage
                    {
                        ParentPage = this,
                        CurrentApp = CurrentApp
                    };
                    PopupPanel.Content = newPage;
                    PopupPanel.Title = CurrentApp.GetLanguageInfo("5101T00022", "Change Keyword");
                    PopupPanel.IsOpen = true;
                }                
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        void SelectKw_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                S5101App.GQueryModify = false;
                S5101App.GOptInfo = S5101Codes.OptSelect;
                SelectKwPage newPage = new SelectKwPage()
                {
                    ParentPage = this,
                    CurrentApp = CurrentApp
                };
                PopupPanel.Content = newPage;
                PopupPanel.Title = CurrentApp.GetLanguageInfo("5101T00064", "Select Keyword");
                PopupPanel.IsOpen = true;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        void EnableKeyword_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var item = KwListView.SelectedItem as KeywordInfoParam;
                if (item != null)
                {
                    item.State = item.State == 0 ? 1 : 0;
                    var param = _mLstKeywords.Where(p => p.LongKwNum == item.LongKwNum).FirstOrDefault();
                    param = item;
                    SetKeywordTable();
                    if (item.State == 0)
                    {
                        InitKwEnableBut();
                    }
                    else
                    {
                        InitKwCloseBut();
                    }

                    UpdateKw(param);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        void EditKeyword_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var item = KwListView.SelectedItem as KeywordInfoParam;
                if (item != null)
                {
                    S5101App.GLongKeywordNum = item.LongKwNum;
                    S5101App.GQueryModify = false;
                    S5101App.GOptInfo = S5101Codes.OptEditKwContent;
                    EditKwInfoPage newPage = new EditKwInfoPage
                    {
                        ParentPage = this,
                        CurrentApp = CurrentApp
                    };
                    PopupPanel.Content = newPage;
                    PopupPanel.Title = CurrentApp.GetLanguageInfo("5101T00032", "Edit Content");
                    PopupPanel.IsOpen = true;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        void DeleteKeyword_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var item = KwListView.SelectedItem as KeywordInfoParam;
                if (item != null)
                {
                    MessageBoxResult result;
                    if (item.State == MDelete)
                    {
                        result = MessageBox.Show(CurrentApp.GetLanguageInfo("5101T00062",
                        "Sure to restore to delete keyword?"),
                        CurrentApp.GetLanguageInfo("5101T00050", "Warning"),
                        MessageBoxButton.OKCancel);
                    }
                    else
                    {
                        result = MessageBox.Show(CurrentApp.GetLanguageInfo("5101T00063",
                        "Sure you want to delete?"),
                        CurrentApp.GetLanguageInfo("5101T00050", "Warning"),
                        MessageBoxButton.OKCancel);
                    }
                    if (result == MessageBoxResult.Cancel)
                    {
                        return;
                    }

                    if (item.State == MDelete)
                    {
                        S5101App.GOptInfo = S5101Codes.OptRestore;
                        item.State = 0;
                        item.IntDelete = 0;
                        var localtime = DateTime.Now;
                        var utc = localtime.ToUniversalTime();
                        item.StrRestoreUtcTime = utc.ToString("yyyy-MM-dd HH:mm:ss");
                        item.StrRestoreLocalTime = localtime.ToString("yyyy-MM-dd HH:mm:ss");
                        item.StrRestorePaperName = CurrentApp.Session.UserInfo.UserName;
                        item.LongRestorePaperNum = CurrentApp.Session.UserInfo.UserID;
                    }
                    else
                    {
                        S5101App.GOptInfo = S5101Codes.OptDelete;
                        item.State = MDelete;
                        item.IntDelete = 1;
                        var localtime = DateTime.Now;
                        var utc = localtime.ToUniversalTime();
                        item.StrDeleteUtcTime = utc.ToString("yyyy-MM-dd HH:mm:ss");
                        item.StrDeleteLocalTime = localtime.ToString("yyyy-MM-dd HH:mm:ss");
                        item.StrDeletePaperName = CurrentApp.Session.UserInfo.UserName;
                        item.LongDeletePaperNum = CurrentApp.Session.UserInfo.UserID;
                    }
                    if (DeleteKw(item))
                    {
                        KeywordInfoParam param = new KeywordInfoParam();
                        param = _mLstKeywords.Where(p => p.LongKwNum == item.LongKwNum).FirstOrDefault();
                        if (S5101App.GOptInfo == S5101Codes.OptRestore)
                        {
                            param = item;
                        }
                        else
                        {

                            _mLstKeywords.Remove(param);
                        }

                        SetKeywordTable();

                        string sql = string.Format("Select * from T_51_008 where C002='{0}'", item.LongKwNum);
                        OptClearKwContent(sql);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void KeywordConnectTable_OnIsSelectedChanged(object sender, EventArgs e)
        {
            if (!_mbKwContentTable)
            {
                _mbKwContentTable = true;
                _mbKeywordTable = false;
                InitKwContentButton();
                BasicExpandOpt.Header = CurrentApp.GetLanguageInfo("5101T00020", "Content Operation");
            }
        }

        private void KeywordTable_OnIsSelectedChanged(object sender, EventArgs e)
        {
            if (!_mbKeywordTable)
            {
                _mbKwContentTable = false;
                _mbKeywordTable = true;
                BasicExpandOpt.Header = CurrentApp.GetLanguageInfo("5101T00021", "Content Operation");
                InitKwButton();
            }
        }

        void KwConPageButton_Click(object sender, RoutedEventArgs e)//选择看第几页的按钮
        {
            Button btn = e.Source as Button;
            if (btn != null)
            {
                var item = btn.DataContext as CToolButtonItem;
                if (item == null) { return; }
                switch (item.Name)
                {
                    case "TB" + "FirstPage":
                        if (_mKwConPageIndex > 0)
                        {
                            _mKwConPageIndex = 0;
                            KwConFillListView();
                            SetKwConPageState();
                        }
                        break;
                    case "TB" + "PrePage":
                        if (_mKwConPageIndex > 0)
                        {
                            _mKwConPageIndex--;
                            KwConFillListView();
                            SetKwConPageState();
                        }
                        break;
                    case "TB" + "NextPage":
                        if (_mKwConPageIndex < _mKwConPageCount - 1)
                        {
                            _mKwConPageIndex++;
                            KwConFillListView();
                            SetKwConPageState();
                        }
                        break;
                    case "TB" + "LastPage":
                        if (_mKwConPageIndex < _mKwConPageCount - 1)
                        {
                            _mKwConPageIndex = _mKwConPageCount - 1;
                            KwConFillListView();
                            SetKwConPageState();
                        }
                        break;
                }
            }
        }

        void KwPageButton_Click(object sender, RoutedEventArgs e)//选择看第几页的按钮
        {
            Button btn = e.Source as Button;
            if (btn != null)
            {
                var item = btn.DataContext as CToolButtonItem;
                if (item == null) { return; }
                switch (item.Name)
                {
                    case "TB" + "FirstPage":
                        if (_mKwPageIndex > 0)
                        {
                            _mKwPageIndex = 0;
                            KwFillListView();
                            SetKwPageState();
                        }
                        break;
                    case "TB" + "PrePage":
                        if (_mKwPageIndex > 0)
                        {
                            _mKwPageIndex--;
                            KwFillListView();
                            SetKwPageState();
                        }
                        break;
                    case "TB" + "NextPage":
                        if (_mKwPageIndex < _mKwPageCount - 1)
                        {
                            _mKwPageIndex++;
                            KwFillListView();
                            SetKwPageState();
                        }
                        break;
                    case "TB" + "LastPage":
                        if (_mKwPageIndex < _mKwPageCount - 1)
                        {
                            _mKwPageIndex = _mKwPageCount - 1;
                            KwFillListView();
                            SetKwPageState();
                        }
                        break;
                }
            }
        }

        private void KwConMouseLeftButtonUp_OnHandler(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var item = KwContentListView.SelectedItem as KwContentInfoParam;
                if (item != null)
                {
                    switch (item.State)
                    {
                        case MClose:
                            InitKwContentButton();
                            InitKwContentEnableBut();
                            InitKwContentDeleteBut();
                            ButEditOpt.Children.Clear();
                            break;
                        case MEnable:
                            InitKwContentButton();
                            InitKwContentCloseBut();
                            InitKwContentDeleteBut();
                            ButEditOpt.Children.Clear();
                            break;
                        case MDelete:
                            InitKwContentRestoreBut();
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void KwMouseLeftButtonUp_OnHandler(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var item = KwListView.SelectedItem as KeywordInfoParam;
                if (item != null)
                {
                    switch (item.State)
                    {
                        case MClose:
                            InitKwButton();
                            InitKwEnableBut();
                            InitKwDeleteBut();
                            InitEditContentBut();
                            break;
                        case MEnable:
                            InitKwButton();
                            InitKwCloseBut();
                            InitKwDeleteBut();
                            InitEditContentBut();
                            break;
                        case MDelete:
                            InitKwRestoreBut();
                            break;
                    }
                    BorderKwContent.Visibility = Visibility.Visible;
                    TxtKwName.Text = item.StrKw;
                    GetAllKwCon(SetSql_5108(item.LongKwNum.ToString()));
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion

        #region

        private void GetKwContents(string strSql)
        {
            try
            {
                var webRequest = new WebRequest();
                Service51011Client client = new Service51011Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service51011"));
                //var client = new Service51011Client();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S5101Codes.OptSelectKwContent;
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
                        CurrentApp.GetLanguageInfo("5101T00065", "Insert data failed"), webReturn.Message));
                    return;
                }

                if (webReturn.ListData.Count <= 0)
                {
                    if (_mbListKwConViewEnable)
                    {
                        SetKwConPageState();
                        SetKwContentTable();
                        _mbListKwConViewEnable = false;
                    }
                    return;
                }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    optReturn = XMLHelper.DeserializeObject<KwContentInfoParam>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        continue;
                    }
                    var param = optReturn.Data as KwContentInfoParam;
                    if (param == null)
                    {
                        ShowException("Fail. filesItem is null");
                        return;
                    }

                    _mKwConPageNum = param.LongKwContentNum.ToString();
                    int total = _mKwConNum + 1;
                    if (total > _mMaxKwConInfos)
                    {
                        ShowInformation(CurrentApp.GetLanguageInfo("5101T00054", "Larger than allowed max Questions, some Questions can\'t be displayed"));
                        return;
                    }
                    _mKwConNum = total;
                    _mLstKwContents.Add(param);
                    SetKwConPageState();
                }
                if (_mbListKwConViewEnable)
                {
                    SetKwContentTable();
                    _mbListKwConViewEnable = false;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void GetKwContents()
        {
            try
            {
                string strSql = SelectKwConSql();
                var webRequest = new WebRequest();
                Service51011Client client = new Service51011Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service51011"));
                //var client = new Service51011Client();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S5101Codes.OptSelectKwContent;
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
                        CurrentApp.GetLanguageInfo("5101T00065", "Insert data failed"), webReturn.Message));
                    return;
                }

                if (webReturn.ListData.Count <= 0)
                {
                    if (_mbListKwConViewEnable)
                    {
                        SetKwConPageState();
                        SetKwContentTable();
                        _mbListKwConViewEnable = false;
                    }
                    return;
                }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    optReturn = XMLHelper.DeserializeObject<KwContentInfoParam>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        continue;
                    }
                    var param = optReturn.Data as KwContentInfoParam;
                    if (param == null)
                    {
                        ShowException("Fail. filesItem is null");
                        return;
                    }

                    _mKwConPageNum = param.LongKwContentNum.ToString();
                    int total = _mKwConNum + 1;
                    if (total > _mMaxKwConInfos)
                    {
                        ShowInformation(CurrentApp.GetLanguageInfo("5101T00054", "Larger than allowed max Questions, some Questions can\'t be displayed"));
                        return;
                    }
                    _mKwConNum = total;
                    _mLstKwContents.Add(param);
                    SetKwConPageState();
                }
                if (_mbListKwConViewEnable)
                {
                    SetKwContentTable();
                    _mbListKwConViewEnable = false;
                }
                GetKwContents();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void GetKeywords( string strSql )
        {
            try
            {
                var webRequest = new WebRequest();
                Service51011Client client = new Service51011Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service51011"));
                //var client = new Service51011Client();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S5101Codes.OptSelectKeyword;
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
                        CurrentApp.GetLanguageInfo("5101T00065", "Insert data failed"), webReturn.Message));
                    return;
                }

                if (webReturn.ListData.Count <= 0)
                {
                    if (_mbListKwViewEnable)
                    {
                        SetKwPageState();
                        SetKeywordTable();
                        _mbListKwViewEnable = false;
                    }
                    return;
                }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    optReturn = XMLHelper.DeserializeObject<KeywordInfoParam>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        continue;
                    }
                    var param = optReturn.Data as KeywordInfoParam;
                    if (param == null)
                    {
                        ShowException("Fail. filesItem is null");
                        return;
                    }

                    _mKwPageNum = param.LongKwNum.ToString();
                    int total = _mKwNum + 1;
                    if (total > _mMaxKwInfos)
                    {
                        ShowInformation(CurrentApp.GetLanguageInfo("5101T00054", "Larger than allowed max Questions, some Questions can\'t be displayed"));
                        return;
                    }
                    _mKwNum = total;
                    _mLstKeywords.Add(param);
                    SetKwPageState();
                }
                if (_mbListKwViewEnable)
                {
                    SetKeywordTable();
                    _mbListKwViewEnable = false;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void GetKeywords()
        {
            try
            {
                string strSql = SelectKwSql();
                var webRequest = new WebRequest();
                Service51011Client client = new Service51011Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service51011"));
                //var client = new Service51011Client();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int) S5101Codes.OptSelectKeyword;
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
                        CurrentApp.GetLanguageInfo("5101T00065", "Insert data failed"), webReturn.Message));
                    return;
                }

                if (webReturn.ListData.Count <= 0)
                {
                    if (_mbListKwViewEnable)
                    {
                        SetKwPageState();
                        SetKeywordTable();
                        _mbListKwViewEnable = false;
                    }
                    return;
                }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    optReturn = XMLHelper.DeserializeObject<KeywordInfoParam>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        continue;
                    }
                    var param = optReturn.Data as KeywordInfoParam;
                    if (param == null)
                    {
                        ShowException("Fail. filesItem is null");
                        return;
                    }

                    _mKwPageNum = param.LongKwNum.ToString();
                    int total = _mKwNum + 1;
                    if (total > _mMaxKwInfos)
                    {
                        ShowInformation(CurrentApp.GetLanguageInfo("5101T00054",
                            "Larger than allowed max Questions, some Questions can\'t be displayed"));
                        return;
                    }
                    _mKwNum = total;
                    _mLstKeywords.Add(param);
                    SetKwPageState();
                }
                if (_mbListKwViewEnable)
                {
                    SetKeywordTable();
                    _mbListKwViewEnable = false;
                }
                GetKeywords();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void SetKwContentTable()
        {
            _mObservableKwConnectInfo.Clear();
            foreach (var param in _mLstKwContents)
            {
                if (S5101App.GOptInfo == S5101Codes.OptSelect)
                {
                    param.StrDeleteState = param.IntDelete == 0 ? CurrentApp.GetLanguageInfo("5101T00053", "No Delete") : CurrentApp.GetLanguageInfo("5101T00052", "Delete");
                }

                _mObservableKwConnectInfo.Add(param);
            }
        }

        private void SetKeywordTable()
        {
            _mObservableKwInfo.Clear();
            foreach (var param in _mLstKeywords)
            {
                if (S5101App.GOptInfo == S5101Codes.OptSelect)
                {
                    param.StrDeleteState = param.IntDelete == 0 ? CurrentApp.GetLanguageInfo("5101T00053", "No Delete") : CurrentApp.GetLanguageInfo("5101T00052", "Delete");
                }

                _mObservableKwInfo.Add(param);
            }
        }

        private void SetKwConPageState()//设置的每页的状态
        {
            try
            {
                int pageCount = _mKwConNum / _mKwConPageSize;
                int mod = _mKwConNum % _mKwConPageSize;
                if (mod > 0)
                {
                    pageCount++;
                }
                _mKwConPageCount = pageCount;
                string strPageInfo = string.Format("{0}/{1} {2} {3}", _mKwConPageIndex + 1, _mKwConPageCount, _mKwConNum,
                    CurrentApp.GetLanguageInfo("5101T00040", "Sum"));
                Dispatcher.Invoke(new Action(() =>
                {
                    TxtKwConPageInfo.Text = strPageInfo;
                    TxtKwConPage.Text = (_mKwConPageIndex + 1).ToString();
                }));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void SetKwPageState()//设置的每页的状态
        {
            try
            {
                int pageCount = _mKwNum / _mKwPageSize;
                int mod = _mKwNum % _mKwPageSize;
                if (mod > 0)
                {
                    pageCount++;
                }
                _mKwPageCount = pageCount;
                string strPageInfo = string.Format("{0}/{1} {2} {3}", _mKwPageIndex + 1, _mKwPageCount, _mKwNum,
                    CurrentApp.GetLanguageInfo("5101T00040", "Sum"));
                Dispatcher.Invoke(new Action(() =>
                {
                    TxtKwPageInfo.Text = strPageInfo;
                    TxtKwPage.Text = (_mKwPageIndex + 1).ToString();
                }));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void KwConFillListView()
        {
            try
            {
                _mObservableKwConnectInfo.Clear();
                int intStart = _mKwConPageIndex * _mKwConPageSize;
                int intEnd = (_mKwConPageIndex + 1) * _mKwConPageSize;
                for (int i = intStart; i < intEnd && i < _mKwConNum; i++)
                {
                    KwContentInfoParam kwConnectInfo = _mLstKwContents[i];
                    _mObservableKwConnectInfo.Add(kwConnectInfo);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void KwFillListView()
        {
            try
            {
                _mObservableKwInfo.Clear();
                int intStart = _mKwPageIndex * _mKwPageSize;
                int intEnd = (_mKwPageIndex + 1) * _mKwPageSize;
                for (int i = intStart; i < intEnd && i < _mKwNum; i++)
                {
                    KeywordInfoParam kwConnectInfo = _mLstKeywords[i];
                    _mObservableKwInfo.Add(kwConnectInfo);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private string SetSql_5107()
        {
            string sql = string.Empty;
            switch (CurrentApp.Session.DBType)
            {
                case S5101App.GMsSql:
                    sql = "select top 1000 X.* from T_51_007 X where C008 = 0 ORDER BY X.C001 DESC";
                    break;
                case S5101App.GOracle:
                    sql = "SELECT * FROM (SELECT X.*  FROM T_51_007 X where C008 = 0 ORDER BY X.C001 DESC) WHERE ROWNUM <= 1000 ";
                    break;
            }
            return sql;
        }

        private string SetSql_5106()
        {
            string sql = string.Empty;
            switch (CurrentApp.Session.DBType)
            {
                case S5101App.GMsSql:
                    sql = "select top 1000 X.* from T_51_006 X where C010 = 0 ORDER BY X.C001 DESC";
                    break;
                case S5101App.GOracle:
                    sql = "SELECT * FROM (SELECT X.*  FROM T_51_006 X where C010 = 0 ORDER BY X.C001 DESC) WHERE ROWNUM <= 1000 ";
                    break;
            }
            return sql;
        }

        private string SelectKwConSql()
        {
            string strTemp = null;
            string sql = string.Empty;
            switch (CurrentApp.Session.DBType)
            {
                case S5101App.GMsSql:
                    if (string.IsNullOrEmpty(_mKwConPageNum))
                    {
                        sql = string.Format("SELECT TOP {0} X.* FROM T_51_007 X where ", _mKwConPageSize);
                    }
                    else
                    {
                        sql = string.Format("SELECT TOP {0} X.* FROM T_51_007 X where X.c001>'{1}' and ", _mKwConPageSize, _mKwConPageNum);
                    }
                    if (!string.IsNullOrEmpty(_mSelectKwContent.StrContent))
                    {
                        strTemp = string.Format(" X.c002 like '%{0}%' ", _mSelectKwContent.StrContent);
                        sql += strTemp;
                    }
                    break;
                case S5101App.GOracle:
                    if (string.IsNullOrEmpty(_mKwConPageNum))
                    {
                        sql = "SELECT * FROM (SELECT X.*  FROM T_51_007 X  where ";
                    }
                    else
                    {
                        sql = string.Format(
                            "SELECT * FROM (SELECT X.*  FROM T_51_007 X  where  X.c001>'{0}' and ", _mKwConPageNum);
                    }

                    if (!string.IsNullOrEmpty(_mSelectKwContent.StrContent))
                    {
                        strTemp = string.Format(" X.c002 like '%{0}%' ", _mSelectKwContent.StrContent);
                        sql += strTemp;
                    }
                    break;
            }

            if (_mSelectKwContent.BEnable)
            {
                strTemp = string.Format(strTemp == null ? " X.c003 = '{0}' " : " and X.c003 = '{0}' ", _mSelectKwContent.IntEnable);
                sql += strTemp;
            }

            if (_mSelectKwContent.BDelete)
            {
                strTemp = string.Format(strTemp == null ? " X.c008 = '{0}' " : " and X.c008 = '{0}' ", _mSelectKwContent.IntDelete);
                sql += strTemp;
            }

            switch (CurrentApp.Session.DBType)
            {
                case 2:
                    strTemp = string.Format(strTemp == null ? " X.c005 >= '{0}' and X.c005 <= '{1}' ORDER BY X.C001" : " and X.c005 >= '{0}' and X.c005 <= '{1}' ORDER BY X.C001", _mSelectKwContent.StrStartTime, _mSelectKwContent.StrEndTime);
                    break;
                case 3:
                    strTemp = string.Format(strTemp == null ? " X.c005 between to_date('{0}','yyyy-mm-dd hh24:mi:ss')  and to_date('{1}','yyyy-mm-dd hh24:mi:ss') ORDER BY X.C001 ) WHERE ROWNUM <= {2}" : " and X.c005 between to_date('{0}','yyyy-mm-dd hh24:mi:ss')  and to_date('{1}','yyyy-mm-dd hh24:mi:ss') ORDER BY X.C001 ) WHERE ROWNUM <= {2}", _mSelectKwContent.StrStartTime, _mSelectKwContent.StrEndTime, _mKwConPageSize);
                    break;
            }

            return sql + strTemp;
        }

        private string SelectKwSql()
        {
            string strTemp = null;
            string sql = string.Empty;
            switch (CurrentApp.Session.DBType)
            {
                case S5101App.GMsSql:
                    if (string.IsNullOrEmpty(_mKwPageNum))
                    {
                        sql = string.Format("SELECT TOP {0} X.* FROM T_51_006 X where ", _mKwPageSize);
                    }
                    else
                    {
                        sql = string.Format("SELECT TOP {0} X.* FROM T_51_006 X where X.c001>'{1}' and ", _mKwPageSize, _mKwPageNum);
                    }
                    if (!string.IsNullOrEmpty(_mSelectKwContent.StrContent))
                    {
                        strTemp = string.Format(" X.c002 like '%{0}%' ", _mSelectKwContent.StrContent);
                        sql += strTemp;
                    }
                    break;
                case S5101App.GOracle:
                    if (string.IsNullOrEmpty(_mKwPageNum))
                    {
                        sql = "SELECT * FROM (SELECT X.*  FROM T_51_006 X  where ";
                    }
                    else
                    {
                        sql = string.Format(
                            "SELECT * FROM (SELECT X.*  FROM T_51_006 X  where  X.c001>'{0}' and ", _mKwPageNum);
                    }

                    if (!string.IsNullOrEmpty(_mSelectKwContent.StrContent))
                    {
                        strTemp = string.Format(" X.c002 like '%{0}%' ", _mSelectKwContent.StrContent);
                        sql += strTemp;
                    }
                    break;
            }

            if (_mSelectKwContent.BEnable)
            {
                strTemp = string.Format(strTemp == null ? " X.c005 = '{0}' " : " and X.c005 = '{0}' ", _mSelectKwContent.IntEnable);
                sql += strTemp;
            }

            if (_mSelectKwContent.BDelete)
            {
                strTemp = string.Format(strTemp == null ? " X.c010 = '{0}' " : " and X.c010 = '{0}' ", _mSelectKwContent.IntDelete);
                sql += strTemp;
            }

            switch (CurrentApp.Session.DBType)
            {
                case 2:
                    strTemp = string.Format(strTemp == null ? " X.c007 >= '{0}' and X.c007 <= '{1}' ORDER BY X.C001" : " and X.c007 >= '{0}' and X.c007 <= '{1}' ORDER BY X.C001", _mSelectKwContent.StrStartTime, _mSelectKwContent.StrEndTime);
                    break;
                case 3:
                    strTemp = string.Format(strTemp == null ? " X.c007 between to_date('{0}','yyyy-mm-dd hh24:mi:ss')  and to_date('{1}','yyyy-mm-dd hh24:mi:ss') ORDER BY X.C001 ) WHERE ROWNUM <= {2}" : " and X.c007 between to_date('{0}','yyyy-mm-dd hh24:mi:ss')  and to_date('{1}','yyyy-mm-dd hh24:mi:ss') ORDER BY X.C001 ) WHERE ROWNUM <= {2}", _mSelectKwContent.StrStartTime, _mSelectKwContent.StrEndTime, _mKwPageSize);
                    break;
            }

            return sql + strTemp;
        }

        private bool UpdateKwContent(KwContentInfoParam kwConnectInfo)
        {
            try
            {
                string strLog;
                var webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S5101Codes.OptUpdateKwContent;
                OperationReturn optReturn = XMLHelper.SeriallizeObject(kwConnectInfo);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return false;
                }
                webRequest.ListData.Add(optReturn.Data.ToString());
                var client = new Service51011Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service51011"));
                //var client = new Service51011Client();
                var webReturn = client.UmpTaskOperation(webRequest);
                client.Close();

                CurrentApp.WriteLog(CurrentApp.GetLanguageInfo("5101T00045", "Update"));
                if (!webReturn.Result)
                {
                    #region 写操作日志

                    strLog = string.Format("{0} {1} : {2}", Utils.FormatOptLogString("5101T00045"),
                        Utils.FormatOptLogString("5101T00046"), webReturn.Message);
                    CurrentApp.WriteOperationLog(S5101Consts.OPT_Update.ToString(), ConstValue.OPT_RESULT_FAIL, strLog);

                    #endregion

                    CurrentApp.WriteLog(webReturn.Message);
                    ShowException(string.Format("{0}: {1}",
                        CurrentApp.GetLanguageInfo("5101T00046", "Upate keyword Content Fail!"),
                        webReturn.Message));
                    return false;
                }
                #region 写操作日志

                strLog = string.Format("{0} {1}", Utils.FormatOptLogString("5101T00045"),
                    Utils.FormatOptLogString("5101T00047"));
                CurrentApp.WriteOperationLog(S5101Consts.OPT_Update.ToString(), ConstValue.OPT_RESULT_SUCCESS, strLog);
                CurrentApp.WriteLog(CurrentApp.GetLanguageInfo("5101T00047", "Update Success"));
                #endregion
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
                return false;
            }
            return true;
        }

        private bool UpdateKw(KeywordInfoParam kwInfo)
        {
            try
            {
                string strLog;
                var webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S5101Codes.OptUpdateKw;
                OperationReturn optReturn = XMLHelper.SeriallizeObject(kwInfo);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return false;
                }
                webRequest.ListData.Add(optReturn.Data.ToString());
                var client = new Service51011Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service51011"));
                //var client = new Service51011Client();
                var webReturn = client.UmpTaskOperation(webRequest);
                client.Close();

                CurrentApp.WriteLog(CurrentApp.GetLanguageInfo("5101T00045", "Update"));
                if (!webReturn.Result)
                {
                    #region 写操作日志

                    strLog = string.Format("{0} {1} : {2}", Utils.FormatOptLogString("5101T00045"),
                        Utils.FormatOptLogString("5101T00046"), webReturn.Message);
                    CurrentApp.WriteOperationLog(S5101Consts.OPT_Update.ToString(), ConstValue.OPT_RESULT_FAIL, strLog);

                    #endregion

                    CurrentApp.WriteLog(webReturn.Message);
                    ShowException(string.Format("{0}: {1}",
                        CurrentApp.GetLanguageInfo("5101T00046", "Upate keyword Content Fail!"),
                        webReturn.Message));
                    return false;
                }
                #region 写操作日志

                strLog = string.Format("{0} {1}", Utils.FormatOptLogString("5101T00045"),
                    Utils.FormatOptLogString("5101T00047"));
                CurrentApp.WriteOperationLog(S5101Consts.OPT_Update.ToString(), ConstValue.OPT_RESULT_SUCCESS, strLog);
                CurrentApp.WriteLog(CurrentApp.GetLanguageInfo("5101T00047", "Update Success"));
                #endregion
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
                return false;
            }
            return true;
        }

        private bool DeleteKwContent(KwContentInfoParam kwConnectInfo)
        {
            try
            {
                string strLog;
                var webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S5101Codes.OptDeleteKwContent;
                OperationReturn optReturn = XMLHelper.SeriallizeObject(kwConnectInfo);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return false;
                }
                webRequest.ListData.Add(optReturn.Data.ToString());
                var client = new Service51011Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service51011"));
                //var client = new Service51011Client();
                var webReturn = client.UmpTaskOperation(webRequest);
                client.Close();

                CurrentApp.WriteLog(CurrentApp.GetLanguageInfo("5101T00045", "Update"));
                if (!webReturn.Result)
                {
                    #region 写操作日志

                    strLog = string.Format("{0} {1} : {2}", Utils.FormatOptLogString("5101T00045"),
                        Utils.FormatOptLogString("5101T00046"), webReturn.Message);
                    CurrentApp.WriteOperationLog(S5101Consts.OPT_Update.ToString(), ConstValue.OPT_RESULT_FAIL, strLog);

                    #endregion

                    CurrentApp.WriteLog(webReturn.Message);
                    ShowException(string.Format("{0}: {1}",
                        CurrentApp.GetLanguageInfo("5101T00046", "Upate keyword Content Fail!"),
                        webReturn.Message));
                    return false;
                }
                #region 写操作日志

                strLog = string.Format("{0} {1}", Utils.FormatOptLogString("5101T00045"),
                    Utils.FormatOptLogString("5101T00047"));
                CurrentApp.WriteOperationLog(S5101Consts.OPT_Update.ToString(), ConstValue.OPT_RESULT_SUCCESS, strLog);
                CurrentApp.WriteLog(CurrentApp.GetLanguageInfo("5101T00047", "Update Success"));
                #endregion
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
                return false;
            }
            return true;
        }

        private bool DeleteKw(KeywordInfoParam kwInfo)
        {
            try
            {
                string strLog;
                var webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S5101Codes.OptDeleteKw;
                OperationReturn optReturn = XMLHelper.SeriallizeObject(kwInfo);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return false;
                }
                webRequest.ListData.Add(optReturn.Data.ToString());
                var client = new Service51011Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service51011"));
                //var client = new Service51011Client();
                var webReturn = client.UmpTaskOperation(webRequest);
                client.Close();

                CurrentApp.WriteLog(CurrentApp.GetLanguageInfo("5101T00045", "Update"));
                if (!webReturn.Result)
                {
                    #region 写操作日志

                    strLog = string.Format("{0} {1} : {2}", Utils.FormatOptLogString("5101T00045"),
                        Utils.FormatOptLogString("5101T00046"), webReturn.Message);
                    CurrentApp.WriteOperationLog(S5101Consts.OPT_Update.ToString(), ConstValue.OPT_RESULT_FAIL, strLog);

                    #endregion

                    CurrentApp.WriteLog(webReturn.Message);
                    ShowException(string.Format("{0}: {1}",
                        CurrentApp.GetLanguageInfo("5101T00046", "Upate keyword Content Fail!"),
                        webReturn.Message));
                    return false;
                }
                #region 写操作日志

                strLog = string.Format("{0} {1}", Utils.FormatOptLogString("5101T00045"),
                    Utils.FormatOptLogString("5101T00047"));
                CurrentApp.WriteOperationLog(S5101Consts.OPT_Update.ToString(), ConstValue.OPT_RESULT_SUCCESS, strLog);
                CurrentApp.WriteLog(CurrentApp.GetLanguageInfo("5101T00047", "Update Success"));
                #endregion
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
                return false;
            }
            return true;
        }

        public void SetKwContents(KwContentInfoParam kwConnectInfo)
        {
            if (S5101Codes.OptAdd == S5101App.GOptInfo)
            {
                _mLstKwContents.Insert(0, kwConnectInfo);
            }
            else
            {
                KwContentInfoParam param = new KwContentInfoParam();
                param =
                    _mLstKwContents.Where(p => p.LongKwContentNum == kwConnectInfo.LongKwContentNum).FirstOrDefault();
                param = kwConnectInfo;
            }
            SetKwContentTable();
        }

        public void SetKeywords(KeywordInfoParam keywordInfo)
        {
            if (S5101Codes.OptAdd == S5101App.GOptInfo)
            {
                _mLstKeywords.Insert(0, keywordInfo);
            }
            else
            {
                KeywordInfoParam param = new KeywordInfoParam();
                param =
                    _mLstKeywords.Where(p => p.LongKwNum == keywordInfo.LongKwNum).FirstOrDefault();
                param = keywordInfo;
            }
            SetKeywordTable();
        }

        public void SelectKwContent(SelectKwContentParam selectKwContent)
        {
            _mKwConPageNum = null;
            _mLstKwContents.Clear();
            _mKwConNum = 0;
            _mKwConPageIndex = 0;
            _mKwConPageCount = 0;
            _mbListKwConViewEnable = true;
            _mSelectKwContent = selectKwContent;
            SetSelectKwContentListTable();
            GetKwContents();
        }

        public void SelectKeyword(SelectKwContentParam selectKwContent)
        {
            _mKwPageNum = null;
            _mLstKeywords.Clear();
            _mKwNum = 0;
            _mKwPageIndex = 0;
            _mKwPageCount = 0;
            _mbListKwViewEnable = true;
            _mSelectKwContent = selectKwContent;
            SetSelectKwListTable();
            GetKeywords();
        }

        private string SetSql_5108(string num)
        {
            string sql = string.Empty;
            switch (CurrentApp.Session.DBType)
            {
                case S5101App.GMsSql:
                    sql = string.Format("select * from T_51_008 where C002 = '{0}'", num);
                    break;
                case S5101App.GOracle:
                    sql = string.Format("select * from T_51_008 where C002 = '{0}'", num);
                    break;
            }
            return sql;
        }

        private void GetAllKwCon(string strSql)
        {
            try
            {
                _mObservableShowKwConInfo.Clear();
                var webRequest = new WebRequest();
                Service51011Client client = new Service51011Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service51011"));
                //var client = new Service51011Client();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S5101Codes.OptSelectAssignKwContent;
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
                        CurrentApp.GetLanguageInfo("5101T00065", "Insert data failed"), webReturn.Message));
                    return;
                }

                if (webReturn.ListData.Count <= 0)
                {
                    return;
                }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    optReturn = XMLHelper.DeserializeObject<KwContentInfoParam>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        continue;
                    }
                    var param = optReturn.Data as KwContentInfoParam;
                    if (param == null)
                    {
                        ShowException("Fail. filesItem is null");
                        return;
                    }
                    _mObservableShowKwConInfo.Add(param);
                }
                
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private bool OptClearKwContent(string sql)
        {
            try
            {
                string strLog;
                var webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S5101Codes.OptClearKwContent;
                OperationReturn optReturn = XMLHelper.SeriallizeObject(sql);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return false;
                }
                webRequest.ListData.Add(optReturn.Data.ToString());
                var client = new Service51011Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service51011"));
                //var client = new Service51011Client();
                var webReturn = client.UmpTaskOperation(webRequest);
                client.Close();

                CurrentApp.WriteLog(CurrentApp.GetLanguageInfo("5101T00045", "Update"));
                if (!webReturn.Result)
                {
                    #region 写操作日志

                    strLog = string.Format("{0} {1} : {2}", Utils.FormatOptLogString("5101T00045"),
                        Utils.FormatOptLogString("5101T00046"), webReturn.Message);
                    CurrentApp.WriteOperationLog(S5101Consts.OPT_Update.ToString(), ConstValue.OPT_RESULT_FAIL, strLog);

                    #endregion

                    CurrentApp.WriteLog(webReturn.Message);
                    ShowException(string.Format("{0}: {1}",
                        CurrentApp.GetLanguageInfo("5101T00046", "Upate keyword Content Fail!"),
                        webReturn.Message));
                    return false;
                }
                #region 写操作日志

                strLog = string.Format("{0} {1}", Utils.FormatOptLogString("5101T00045"),
                    Utils.FormatOptLogString("5101T00047"));
                CurrentApp.WriteOperationLog(S5101Consts.OPT_Update.ToString(), ConstValue.OPT_RESULT_SUCCESS, strLog);
                CurrentApp.WriteLog(CurrentApp.GetLanguageInfo("5101T00047", "Update Success"));
                #endregion
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
                return false;
            }
            return true;
        }
       
        #endregion
    }

    
}
