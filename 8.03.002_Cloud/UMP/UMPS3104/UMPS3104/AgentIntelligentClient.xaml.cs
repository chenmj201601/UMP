using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using PFShareClassesC;
using UMPS3104.Commands;
using UMPS3104.Models;
using UMPS3104.S3102Codes;
using UMPS3104.Wcf00000;
using UMPS3104.Wcf11012;
using UMPS3104.Wcf31041;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common31041;
using VoiceCyber.UMP.Common31041.Common3102;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;
using VoiceCyber.Wpf.AvalonDock.Layout;
using VoiceCyber.Wpf.AvalonDock.Layout.Serialization;
using System.Xml;
using UMPS1600;

namespace UMPS3104
{
    /// <summary>
    /// AgentIntelligentClient.xaml 的交互逻辑
    /// </summary>
    public partial class AgentIntelligentClient
    {

        [DllImport("kernel32.dll")]
        public static extern int OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);
        #region StaticMembers
        //所有权限
        public static ObservableCollection<OperationInfo> ListOperations = new ObservableCollection<OperationInfo>();
        private ObservableCollection<OperationInfo> mListBasicOperations;
        private ObservableCollection<RecordInfoItem> mListCurrentRecordInfoItems;
        private ObservableCollection<AgentAndUserInfoItems> mListAuInfoItems;
        private ObservableCollection<CallInfoPropertyItem> mListCallInfoPropertyItems;
        private ObservableCollection<RecordPlayHistoryItem> mListRecordPlayHistoryItems;
        private ObservableCollection<RecordScoreInfoItem> mListRecordScoreInfoItems;
        private List<RecordInfoItem> mListAllRecordInfoItems;
        private List<RecordPlayHistoryItem> mListAllHistoryItems;
        private List<RecordScoreInfoItem> mListAllScoreItems;
        public List<SftpServerInfo> mListSftpServers;
        private List<SettingInfo> mListSettingInfos;
        private List<DownloadParamInfo> mListDownloadParams;
        private List<RecordEncryptInfo> mListRecordEncryptInfos;
        private RecordInfoItem mCurrentRecordInfoItem;
        private RecordScoreInfoItem mCurrentRecordScoreInfoItem;
        private RecordPlayItem mCurrentRecordPlayItem;
        private BackgroundWorker mWorker;
        private ObjectItemClient mRootItem;
        private List<ObjectItemClient> mListSelectedObjects;
        private List<ObjectItemClient> mListAllObjects;
        private PlayRecordTime playTime;


        private UCPlayBox mUCPlayBox;
        private Service03Helper mService03Helper;
        private int mCircleMode;
        

        private List<PanelItem> mListPanels;

        private int mPageIndex;
        private int mPageCount;
        private int mPageSize;
        private int mRecordTotal;
        private int mMaxRecords;
        /// <summary>
        /// false停止查詢
        /// </summary>
        private bool mIsQueryContinue;
        /// <summary>
        /// 判斷是直接關閉程序，還是回到登錄界面
        /// </summary>
        private bool IsClose = true;


        public List<FolderTree> ListFolderInfo;
        public FolderTree mCurentFolder;
        public ObservableCollection<FilesItemInfo> mListFilesInfo;

        /// <summary>
        /// ABCD拼接SQL查询串的临时存储
        /// </summary>
        public string ABCDTempStr = string.Empty;

        /// <summary>
        /// 老挝专用，保存Url
        /// </summary>
        private string mLaosUrl = string.Empty;


        /// <summary>
        /// 获取评分相关的参数配置
        /// </summary>
        public List<GlobalParamInfo> ListScoreParam;

        private IMMainPage mIMMainPage;
        private int mMsgCount;
        #endregion



        public AgentIntelligentClient()
        {
            InitializeComponent();
            mListPanels = new List<PanelItem>();
            mListAllRecordInfoItems = new List<RecordInfoItem>();
            mListAllHistoryItems = new List<RecordPlayHistoryItem>();
            mListAllScoreItems = new List<RecordScoreInfoItem>();
            mCurrentRecordInfoItem = new RecordInfoItem();
            mCurrentRecordScoreInfoItem = new RecordScoreInfoItem();
            mListBasicOperations = new ObservableCollection<OperationInfo>();
            mListCurrentRecordInfoItems = new ObservableCollection<RecordInfoItem>();
            mListRecordPlayHistoryItems = new ObservableCollection<RecordPlayHistoryItem>();
            mListRecordScoreInfoItems = new ObservableCollection<RecordScoreInfoItem>();
            mListCallInfoPropertyItems = new ObservableCollection<CallInfoPropertyItem>();
            mListSelectedObjects = new List<ObjectItemClient>();
            mListAllObjects = new List<ObjectItemClient>();
            mListSftpServers = new List<SftpServerInfo>();
            mListSettingInfos = new List<SettingInfo>();
            mListDownloadParams = new List<DownloadParamInfo>();
            mListRecordEncryptInfos = new List<RecordEncryptInfo>();
            mRootItem = new ObjectItemClient();
            playTime = new PlayRecordTime();
            mService03Helper = new Service03Helper();
            mCircleMode = 0;
            mPageIndex = 0;
            mPageCount = 0;
            mPageSize = 100;
            mRecordTotal = 0;
            mMaxRecords = 100000;
            mIsQueryContinue = false;

            ListFolderInfo = new List<FolderTree>();
            mCurentFolder = new FolderTree();
            mListFilesInfo = new ObservableCollection<FilesItemInfo>();
            ListScoreParam = new List<GlobalParamInfo>();

            mMsgCount = 0;

            this.Loaded += AgentIntelligentClient_Loaded;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (!IsClose)
            {
                return;
            }
            if (!App.defaultHttp)
            {
                if (App.Session.AppServerInfo.Protocol.ToUpper() == "HTTP")
                {
                    App.Session.AppServerInfo.Protocol = "https";
                    App.Session.AppServerInfo.Port += 1;
                }
            }
            if (mIMMainPage != null)
            {
                try
                {
                    mIMMainPage.LogOff();
                }
                catch { }
                mIMMainPage = null;
            }
            int pid = 0;
            try
            {
                ListOperations.Clear();
                List<string> clientListString = new List<string>();
                clientListString.Add(App.renterID); //租户编码
                clientListString.Add(App.Session.UserInfo.UserID.ToString()); //用户编码
                clientListString.Add(App.Session.SessionID); //登录分配的SessionID
                Service00000Client loginClient = new Service00000Client(WebHelper.CreateBasicHttpBinding(App.Session),
                    WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service00000"));
                OperationDataArgs args = loginClient.OperationMethodA(42, clientListString); //43登录、42退出
                string temp = App.DecryptString(args.StringReturn);

                Process[] ps = Process.GetProcesses();
                foreach (Process p in ps)
                {
                    if (p.ProcessName == "UMPS3104")
                    {
                        pid = p.Id;
                    }
                }
                base.OnClosed(e);
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
            finally
            {
                if (pid > 0)
                {
                    int ExcelProcess;
                    ExcelProcess = OpenProcess(0x0010 | 0x0020, false, pid);
                    //判断进程是否仍然存在
                    if (ExcelProcess > 0)
                    {
                        try
                        {
                            //通过进程ID,找到进程
                            System.Diagnostics.Process process = System.Diagnostics.Process.GetProcessById(pid);
                            //Kill 进程
                            process.Kill();
                        }
                        catch (Exception)
                        {
                            //强制结束Excel进程失败,可以记录一下日志.
                        }
                    }
                }
            }
        }

        void AgentIntelligentClient_Loaded(object sender, RoutedEventArgs e)
        {
            PageHead.PageHeadEvent += PageHead_PageHeadEvent;
            TxtPage.KeyUp += TxtPage_KeyUp;

            //当前选中创建操作按钮 更新录音信息
            LvRecordData.SelectionChanged += LvRecordData_SelectionChanged;
            LvRecordScoreResult.SelectionChanged += LvRecordScoreResult_SelectionChanged;
            LvRecordData.ItemsSource = mListCurrentRecordInfoItems;

            //录音播放历史
            LvRecordPlayHistory.ItemsSource = mListRecordPlayHistoryItems;
            //评分详情清單
            LvRecordScoreResult.ItemsSource = mListRecordScoreInfoItems;
            //播放录音
            //LvRecordData.MouseDoubleClick += LvRecordData_MouseDoubleClick;
            ListBoxCallInfo.ItemsSource = mListCallInfoPropertyItems;
            VoicePlayer.PlayerEvent += VoicePlayer_PlayerEvent;

            //教材库
            bFolderTree.ItemsSource = mCurentFolder.Children;
            bFolderTree.SelectedItemChanged+=bFolderTree_SelectedItemChanged;
            LvBooks.ItemsSource = mListFilesInfo;
            LvBooks.MouseDoubleClick += LvBooks_MouseDoubleClick;


            Init();
            TableInfo();
            ChangeLanguage();
            ChangeTheme();

            BindCommands();
            InitTaskRecordColumns();
            InitRecordScoreResultColumns();
            InitPlayHistoryColumns();
            LaosLinkSetting();
            InitBookDocumentColumns();
            LoadLayout();
            try
            {
                if (App.PwdState < 1.0)
                {
                    PopupPanel.Title = App.GetLanguageInfo("3104T00096", "Change PassWord ");
                    ChangePassWord changePassWord = new ChangePassWord();
                    changePassWord.PageParent = this;
                    PopupPanel.Content = changePassWord;
                    PopupPanel.IsOpen = true;
                }

                //MyWaiter 控件  底部的进度条
                MyWaiter.Visibility = Visibility.Visible;
                mWorker = new BackgroundWorker();
                //注册线程主体方法
                mWorker.DoWork += (s, de) =>
                {
                    InitOperation();
                    LoadSftpServerList();
                    GetFolders(mCurentFolder, -1);
                    GetScoreParams();
                };
                //当后台操作已完成、被取消或引发异常时发生
                mWorker.RunWorkerCompleted += (s, re) =>
                {
                    mWorker.Dispose();
                    MyWaiter.Visibility = Visibility.Collapsed;

                    InitPanels();
                    ShowPanel(0);
                    ShowPanel("PanelScoreList", false);
                    InitBasicOperations();
                    CreateOptButtons();
                    CreateToolBarButtons();
                    SetPanelVisible();
                    GetAuInfoLists();
                    LoadDownloadParamList();
                    LoadRecordEncryptInfos();

                    //创建查询记录页面
                    CreatePageButtons();
                    CreateCallInfoItems();
                    if (mCurrentRecordInfoItem.sAppealMark == "1" || mCurrentRecordInfoItem.sAppealMark == "2")
                    {
                        CreatAppealInfoDetail();
                    }
                    else
                    {
                        stkAppealInfo.Children.Clear();
                    }

                    //播放相關
                    SetService03Helper();


                };
                mWorker.RunWorkerAsync();//触发DoWork事件

            }
            catch (Exception ex)
            {
                WriteLog.WriteLogToFile("(ಥ_ಥ)\t  AgentIntelligentClient_Loaded", ex.Message + "\t" + ex.StackTrace);
                App.ShowExceptionMessage(ex.Message);
            }
        }

        
        private void BindCommands()//基本操作按钮Binding
        {
            try
            {
                CommandBindings.Add(
                new CommandBinding(URMainPageCommands.QueryRecordCommand,
                    QueryRecord_Executed,
                    (s, e) => e.CanExecute = true));
                CommandBindings.Add(
                 new CommandBinding(URMainPageCommands.AgentAppealCommand,
                        AgentAppeal_Executed,
                        (s, e) => e.CanExecute = true));
                CommandBindings.Add(
                 new CommandBinding(URMainPageCommands.ViewScoreResultCommand,
                        ViewScoreResult_Executed,
                        (s, e) => e.CanExecute = true));
                CommandBindings.Add(
                    new CommandBinding(URMainPageCommands.RecordPlayHistoryCommand,
                    RecordPlayHistoryView_Executed,
                    (s, e) => e.CanExecute = true));
                CommandBindings.Add(new CommandBinding(URMainPageCommands.LaosLinkCommand,
                    LaosLinkCommand_Click,
                    (s, e) => e.CanExecute = true));
            }
            catch (Exception ex)
            {
            }
        }

        private void CreatePageButtons()
        {
            try
            {
                List<ToolButtonItem> listBtns = new List<ToolButtonItem>();
                ToolButtonItem item = new ToolButtonItem();
                item.Name = "TB" + "StopQueryMark";
                item.Display = App.GetLanguageInfo("3104T00049", "Stop Query");
                item.Tip = App.GetLanguageInfo("3104T00049", "Stop Query");
                item.Icon = "Images/stop.png";
                listBtns.Add(item);

                item = new ToolButtonItem();
                item.Name = "TB" + "FirstPage";
                item.Display = App.GetLanguageInfo("3104T00050", "First Page");
                item.Tip = App.GetLanguageInfo("3104T00050", "First Page");
                item.Icon = "Images/first.ico";
                listBtns.Add(item);

                item = new ToolButtonItem();
                item.Name = "TB" + "PrePage";
                item.Display = App.GetLanguageInfo("3104T00051", "Pre Page");
                item.Tip = App.GetLanguageInfo("3104T00051", "Pre Page");
                item.Icon = "Images/pre.ico";
                listBtns.Add(item);

                item = new ToolButtonItem();
                item.Name = "TB" + "NextPage";
                item.Display = App.GetLanguageInfo("3104T00052", "Next Page");
                item.Tip = App.GetLanguageInfo("3104T00052", "Next Page");
                item.Icon = "Images/next.ico";
                listBtns.Add(item);

                item = new ToolButtonItem();
                item.Name = "TB" + "LastPage";
                item.Display = App.GetLanguageInfo("3104T00053", "Last Page");
                item.Tip = App.GetLanguageInfo("3104T00053", "Last Page");
                item.Icon = "Images/last.ico";
                listBtns.Add(item);

                PanelPageButtons.Children.Clear();
                for (int i = 0; i < listBtns.Count; i++)
                {
                    ToolButtonItem toolBtn = listBtns[i];
                    Button btn = new Button();
                    btn.DataContext = toolBtn;
                    btn.SetResourceReference(StyleProperty, "ToolBarButtonStyle");
                    btn.Click += PageButton_Click;
                    PanelPageButtons.Children.Add(btn);
                }
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        void PageButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btn = e.Source as Button;
                if (btn != null)
                {
                    var item = btn.DataContext as ToolButtonItem;
                    if (item == null) { return; }
                    switch (item.Name)
                    {
                        case "TB" + "FirstPage":
                            if (mPageIndex > 0)
                            {
                                mPageIndex = 0;
                                FillListView();
                                SetPageState();
                            }
                            break;
                        case "TB" + "PrePage":
                            if (mPageIndex > 0)
                            {
                                mPageIndex--;
                                FillListView();
                                SetPageState();
                            }
                            break;
                        case "TB" + "NextPage":
                            if (mPageIndex < mPageCount - 1)
                            {
                                mPageIndex++;
                                FillListView();
                                SetPageState();
                            }
                            break;
                        case "TB" + "LastPage":
                            if (mPageIndex < mPageCount - 1)
                            {
                                mPageIndex = mPageCount - 1;
                                FillListView();
                                SetPageState();
                            }
                            break;
                        case "TB" + "StopQueryMark":
                            {
                                mIsQueryContinue = false;
                            }
                            break;
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        private void CreateToolBarButtons()
        {
            try
            {
                PanelToolButton.Children.Clear();
                ToolButtonItem toolItem;
                ToggleButton toggleBtn;
                Button btn;
                OperationInfo optInfo;
                for (int i = 0; i < mListPanels.Count; i++)
                {
                    PanelItem item = mListPanels[i];
                    if (!item.CanClose) { continue; }
                    toolItem = new ToolButtonItem();
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
                toolItem = new ToolButtonItem();
                toolItem.Name = "BT" + "ModifyPassWord";
                toolItem.Display= App.GetLanguageInfo("3104T00096", "Change PassWord");
                toolItem.Tip = App.GetLanguageInfo("3104T00096", "Change PassWord");
                toolItem.Icon = "Images/lock.png";
                btn = new Button();
                btn.Click += ToolButton_Click;
                btn.DataContext = toolItem;
                btn.SetResourceReference(StyleProperty, "ToolBarButtonStyle");
                PanelToolButton.Children.Add(btn);

                /*
                toolItem = new ToolButtonItem();
                toolItem.Name = "BT" + "SaveLayout";
                toolItem.Display = App.GetLanguageInfo("3104T00058", "Save Layout");
                toolItem.Tip = App.GetLanguageInfo("3104T00058", "Save Layout");
                toolItem.Icon = "Images/savelayout.png";
                btn = new Button();
                btn.Click += ToolButton_Click;
                btn.DataContext = toolItem;
                btn.SetResourceReference(StyleProperty, "ToolBarButtonStyle");
                optInfo = ListOperations.FirstOrDefault(o => o.ID == S3104Consts.OPT_SaveLayout);
                if (optInfo != null)
                {
                    PanelToolButton.Children.Add(btn);
                }

                toolItem = new ToolButtonItem();
                toolItem.Name = "BT" + "ResetLayout";
                toolItem.Display = App.GetLanguageInfo("3104T00057", "Reset Layout");
                toolItem.Tip = App.GetLanguageInfo("3104T00057", "Reset Layout");
                toolItem.Icon = "Images/resetlayout.png";
                btn = new Button();
                btn.Click += ToolButton_Click;
                btn.DataContext = toolItem;
                btn.SetResourceReference(StyleProperty, "ToolBarButtonStyle");
                optInfo = ListOperations.FirstOrDefault(o => o.ID == S3104Consts.OPT_ResetLayout);
                if (optInfo != null)
                {
                    PanelToolButton.Children.Add(btn);
                }
                */
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }


        void PanelToggleButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var toggleBtn = e.Source as ToggleButton;
                if (toggleBtn != null)
                {
                    ToolButtonItem item = toggleBtn.DataContext as ToolButtonItem;
                    if (item != null)
                    {
                        PanelItem panelItem = mListPanels.FirstOrDefault(p => string.Format("TB{0}", p.Name) == item.Name);
                        if (panelItem != null)
                        {
                            panelItem.IsVisible = toggleBtn.IsChecked == true;
                        }
                    }
                    SetPanelVisible();
                }
            }
            catch (Exception) { }
        }

        private void SetPanelVisible()
        {
            try
            {
                var panel =
                      PanelManager.Layout.Descendents().OfType<LayoutAnchorable>().FirstOrDefault(p => p.ContentId == "PanelRecordList");
                if (panel != null)
                {
                    panel.Title = App.GetLanguageInfo("3104T00036", "Record List");
                }

                panel =
                     PanelManager.Layout.Descendents().OfType<LayoutAnchorable>().FirstOrDefault(p => p.ContentId == "PanelScoreList");
                if (panel != null)
                {
                    var panelItem = mListPanels.FirstOrDefault(p => p.ContentID == panel.ContentId);
                    if (panelItem != null)
                    {
                        if (panelItem.IsVisible)
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
                            panelItem.IsVisible = panel1.IsVisible;
                            SetViewStatus();
                        };
                    }
                    panel.Title = App.GetLanguageInfo("3104T00134", "RecordScore List");
                }

                panel =
                     PanelManager.Layout.Descendents().OfType<LayoutAnchorable>().FirstOrDefault(p => p.ContentId == "PanelPlayHistoryList");
                if (panel != null)
                {
                    var panelItem = mListPanels.FirstOrDefault(p => p.ContentID == panel.ContentId);
                    if (panelItem != null)
                    {
                        if (panelItem.IsVisible)
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
                            panelItem.IsVisible = panel1.IsVisible;
                            SetViewStatus();
                        };
                    }
                    panel.Title = App.GetLanguageInfo("3104T00076", "PlayHistory List");
                }

                panel =
                     PanelManager.Layout.Descendents().OfType<LayoutAnchorable>().FirstOrDefault(p => p.ContentId == "PanelPlayBox");
                if (panel != null)
                {
                    var panelItem = mListPanels.FirstOrDefault(p => p.ContentID == panel.ContentId);
                    if (panelItem != null)
                    {
                        if (panelItem.IsVisible)
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
                            panelItem.IsVisible = panel1.IsVisible;
                            SetViewStatus();
                        };
                    }
                    panel.Title = App.GetLanguageInfo("3104T00037", "Play Box");
                }

                panel =
                     PanelManager.Layout.Descendents().OfType<LayoutAnchorable>().FirstOrDefault(p => p.ContentId == "PanelCallInfo");
                if (panel != null)
                {
                    var panelItem = mListPanels.FirstOrDefault(p => p.ContentID == panel.ContentId);
                    if (panelItem != null)
                    {
                        if (panelItem.IsVisible)
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
                            panelItem.IsVisible = panel1.IsVisible;
                            SetViewStatus();
                        };
                    }
                    panel.Title = App.GetLanguageInfo("3104T00039", "Call Information");
                }

                panel =
                     PanelManager.Layout.Descendents().OfType<LayoutAnchorable>().FirstOrDefault(p => p.ContentId == "PanelAppealDetail");
                if (panel != null)
                {
                    var panelItem = mListPanels.FirstOrDefault(p => p.ContentID == panel.ContentId);
                    if (panelItem != null)
                    {
                        if (panelItem.IsVisible)
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
                            panelItem.IsVisible = panel1.IsVisible;
                            SetViewStatus();
                        };
                    }
                    panel.Title = App.GetLanguageInfo("3104T00038", "Appeal Detail");
                }

                panel =
                    PanelManager.Layout.Descendents().OfType<LayoutAnchorable>().FirstOrDefault(p => p.ContentId == "PanelScoreDetail");
                if (panel != null)
                {
                    var panelItem = mListPanels.FirstOrDefault(p => p.ContentID == panel.ContentId);
                    if (panelItem != null)
                    {
                        if (panelItem.IsVisible)
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
                            panelItem.IsVisible = panel1.IsVisible;
                            SetViewStatus();
                        };
                    }
                    panel.Title = App.GetLanguageInfo("3104T00075", "Score Detail");
                }
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }
        void ToolButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var toolBtn = e.Source as Button;
                if (toolBtn != null)
                {
                    ToolButtonItem item = toolBtn.DataContext as ToolButtonItem;
                    if (item != null)
                    {
                        switch (item.Name)
                        {
                            case "BTSaveLayout":
                                SaveLayout();
                                break;
                            case "BTResetLayout":
                                ResetLayout();
                                SetViewStatus();
                                break;
                            case "BTModifyPassWord":
                                PopupPanel.Title = App.GetLanguageInfo("3104T00096", "Change PassWord ");
                                ChangePassWord changePassWord = new ChangePassWord();
                                changePassWord.PageParent = this;
                                PopupPanel.Content = changePassWord;
                                PopupPanel.IsOpen = true;
                                break;
                        }
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        private void ResetLayout()
        {
            try
            {
                string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                    string.Format("UMP/{0}/layout.xml", App.AppName));
                if (File.Exists(path)) { File.Delete(path); }
                ShowPanel(2);
                App.ShowInfoMessage(App.GetLanguageInfo("FO3104005", "Resetlayout") + App.GetLanguageInfo("3104T00093", "Successed"));
            }
            catch (Exception ex) { }
            return;
        }
        private void SetViewStatus()
        {
            try
            {
                for (int i = 0; i < PanelToolButton.Children.Count; i++)
                {
                    var toggleBtn = PanelToolButton.Children[i] as ToggleButton;
                    if (toggleBtn != null)
                    {
                        ToolButtonItem item = toggleBtn.DataContext as ToolButtonItem;
                        if (item == null) { continue; }
                        PanelItem panelItem = mListPanels.FirstOrDefault(p => string.Format("TB{0}", p.Name) == item.Name);
                        if (panelItem != null)
                        {
                            toggleBtn.IsChecked = panelItem.IsVisible;
                        }
                    }
                }
            }
            catch (Exception ex) { }
        }

        private void InitPanels()
        {
            try
            {
                mListPanels.Clear();
                OperationInfo optInfo;

                PanelItem panelItem = new PanelItem();
                panelItem.Name = "RecordList";
                panelItem.ContentID = "PanelRecordList";
                panelItem.Title = App.GetLanguageInfo("3104T00036", "Record List");
                panelItem.Icon = "Images/recordlist.png";
                panelItem.IsVisible = true;
                panelItem.CanClose = false;
                mListPanels.Add(panelItem);

                panelItem = new PanelItem();
                panelItem.Name = "PanelScoreList";
                panelItem.ContentID = "PanelScoreList";
                panelItem.Title = App.GetLanguageInfo("3104T00134", "RecordScore List");
                panelItem.Icon = "Images/recordlist.png";
                panelItem.IsVisible = true;
                panelItem.CanClose = false;
                mListPanels.Add(panelItem);

                panelItem = new PanelItem();
                panelItem.Name = "PlayBox";
                panelItem.ContentID = "PanelPlayBox";
                panelItem.Title = App.GetLanguageInfo("3104T00037", "Play Box");
                panelItem.Icon = "Images/playbox.png";
                panelItem.CanClose = true;
                optInfo = ListOperations.FirstOrDefault(o => o.ID == S3104Consts.OPT_PlayRecord);
                if (optInfo != null)
                {
                    panelItem.IsVisible = true;
                    mListPanels.Add(panelItem);
                }

                panelItem = new PanelItem();
                panelItem.Name = "PlayHistoryList";
                panelItem.ContentID = "PanelPlayHistoryList";
                panelItem.Title = App.GetLanguageInfo("3104T00076", "PlayHistory List");
                panelItem.Icon = "Images/playlist.png";
                panelItem.CanClose = true;
                optInfo = ListOperations.FirstOrDefault(o => o.ID == S3104Consts.OPT_RecordPlayHistory);
                if (optInfo != null)
                {
                    panelItem.IsVisible = true;
                    mListPanels.Add(panelItem);
                }

                panelItem = new PanelItem();
                panelItem.Name = "CallInfo";
                panelItem.ContentID = "PanelCallInfo";
                panelItem.Title = App.GetLanguageInfo("3104T00039", "Call Information");
                panelItem.Icon = "Images/callinfo.png";
                panelItem.CanClose = true;
                panelItem.IsVisible = true;
                mListPanels.Add(panelItem);


                panelItem = new PanelItem();
                panelItem.Name = "AppealDetail";
                panelItem.ContentID = "PanelAppealDetail";
                panelItem.Title = App.GetLanguageInfo("3104T00038", "Appeal Detail");
                panelItem.Icon = "Images/appealdetail.png";
                panelItem.CanClose = true;
                panelItem.IsVisible = true;
                mListPanels.Add(panelItem);

                panelItem = new PanelItem();
                panelItem.Name = "ScoreDetail";
                panelItem.ContentID = "PanelScoreDetail";
                panelItem.Title = App.GetLanguageInfo("3104T00075", "Score Detail");
                panelItem.Icon = "Images/scoredetail.png";
                panelItem.CanClose = true;
                panelItem.IsVisible = true;
                mListPanels.Add(panelItem);
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        //从App里取到所有权限
        void InitOperation()
        {
            ListOperations = new ObservableCollection<OperationInfo>(App.ListOperationInfos);
        }

        private void SaveLayout()
        {
            try
            {
                var serializer = new XmlLayoutSerializer(PanelManager);
                var dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                    string.Format("UMP/{0}", App.AppName));
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                string path = Path.Combine(dir, "layout.xml");
                using (var stream = new StreamWriter(path))
                {
                    serializer.Serialize(stream);
                }

                App.ShowInfoMessage(App.GetLanguageInfo("3104T00110", "Save layout end."));
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }
        /// <summary>
        /// 读取LayoutAnchorable控件中信息
        /// </summary>
        private void LoadLayout()
        {
            try
            {
                string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                    string.Format("UMP/{0}/layout.xml", App.AppName));
                if (!File.Exists(path)) { return; }
                var serializer = new XmlLayoutSerializer(PanelManager);
                using (var stream = new StreamReader(path))
                {
                    serializer.Deserialize(stream);
                }

                for (int i = 0; i < mListPanels.Count; i++)
                {
                    PanelItem item = mListPanels[i];
                    var panel =
                      PanelManager.Layout.Descendents().OfType<LayoutAnchorable>().FirstOrDefault(p => p.ContentId == item.ContentID);
                    if (panel != null)
                    {
                        item.IsVisible = panel.IsVisible;
                    }
                }
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void InitTaskRecordColumns()
        {
            try
            {
                string[] lans = "3104T00045,3104T00150,3104T00001,3104T00003,3104T00140,3104T00091,3104T00023,3104T00046,3104T00048,3104T00030,3104T00029,3104T00026,3104T00027,3104T00028,3104T00174,3104T00175,3104T00176,3104T00180,3104T00059,3104T00047,3104T00060".Split(',');
                string[] cols = "RowNumber,MediaType,SerialID,AppealMark,IsScored,BookMark,StartRecordTime,StopRecordTime,Extension,Agent,DurationStr,CallerID,CalledID,Direction,StrServiceAttitude,StrProfessionalLevel,StrRecordDurationError,StrRepeatCallIn,VoiceID,ChannelID,VoiceIP".Split(',');
                int[] colwidths = { 50, 40, 150, 80, 80, 80, 140, 140, 80, 80, 100, 100, 150, 100, 100, 100, 100, 100, 80, 80, 100 };
                GridView ColumnGridView = new GridView();
                GridViewColumn gvc;

                for (int i = 0; i < cols.Count(); i++)
                {
                    gvc = new GridViewColumn();
                    gvc.Header = App.GetLanguageInfo(lans[i], cols[i]);
                    gvc.Width = colwidths[i];
                    if (cols[i] == "RowNumber")
                    {
                        DataTemplate dt = Resources["CellRowNumberTemplate"] as DataTemplate;
                        gvc.CellTemplate = dt;
                    }
                    else if (cols[i] == "MediaType")
                    {
                        DataTemplate dt = Resources["CellMediaTypeTemplate"] as DataTemplate;
                        gvc.CellTemplate = dt;
                    }
                    else if (cols[i].Contains("RecordTime").Equals(true))
                    {
                        var binding = new Binding(cols[i]);
                        binding.StringFormat = "yyyy-MM-dd HH:mm:ss";
                        gvc.DisplayMemberBinding = binding;
                    }
                    else
                    {
                        gvc.DisplayMemberBinding = new Binding(cols[i]);
                    }
                    ColumnGridView.Columns.Add(gvc);
                }
                LvRecordData.View = ColumnGridView;
            }
            catch (Exception ex)
            {

            }
        }

        private void InitRecordScoreResultColumns()
        {
            try
            {
                string[] lans = "3104T00032,3104T00025,3104T00136,3104T00135,3104T00086,3104T00003".Split(',');
                string[] cols = "ScoreTime,Score,IsFinal,ScorePerson,TemplateName,AppealMark".Split(',');
                int[] colwidths = { 120, 80, 80, 120, 120, 80 };
                GridView ColumnGridView = new GridView();
                GridViewColumn gvc;

                for (int i = 0; i < 6; i++)
                {
                    gvc = new GridViewColumn();
                    gvc.Header = App.GetLanguageInfo(lans[i], cols[i]);
                    gvc.Width = colwidths[i];
                    if (i == 0)
                    {
                        var binding = new Binding(cols[i]);
                        binding.StringFormat = "yyyy-MM-dd HH:mm:ss";
                        gvc.DisplayMemberBinding = binding;
                    }
                    else
                    {
                        gvc.DisplayMemberBinding = new Binding(cols[i]);
                    }
                    ColumnGridView.Columns.Add(gvc);
                }
                LvRecordScoreResult.View = ColumnGridView;
            }
            catch (Exception)
            {

            }
        }

        private void InitPlayHistoryColumns()
        {
            try
            {
                string[] lans = "3104T00045,3104T00001,3104T00077,3104T00078,3104T00079,3104T00042,3104T00080,3104T00081,3104T00082".Split(',');
                string[] cols = "RowNumber,RecordReference,UserID,PlayDate,PlayDurationStr,Type,PlayTimes,StartPositionStr,StopPositionStr".Split(',');
                int[] colwidths = { 50, 150, 150, 130, 100, 50, 60, 100, 100 };
                GridView ColumnGridView = new GridView();
                GridViewColumn gvc;

                for (int i = 0; i < 9; i++)
                {
                    gvc = new GridViewColumn();
                    gvc.Header = App.GetLanguageInfo(lans[i], cols[i]);
                    gvc.Width = colwidths[i];
                    if (i == 3)
                    {
                        var binding = new Binding(cols[i]);
                        binding.StringFormat = "yyyy-MM-dd HH:mm:ss";
                        gvc.DisplayMemberBinding = binding;
                    }
                    else
                    {
                        gvc.DisplayMemberBinding = new Binding(cols[i]);
                    }
                    ColumnGridView.Columns.Add(gvc);
                }
                LvRecordPlayHistory.View = ColumnGridView;
            }
            catch (Exception)
            {

            }
        }

        public void TableInfo()
        {
            try
            {
                List<string> TableNameString = new List<string>();
                TableNameString.Add(App.Session.DBType.ToString());//数据库类型
                TableNameString.Add(App.Session.DBConnectionString);//数据库连接串
                TableNameString.Add(App.Session.RentInfo.Token);//租户编码
                Service00000Client loginClient = new Service00000Client(WebHelper.CreateBasicHttpBinding(App.Session),
                            WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service00000"));
                OperationDataArgs args = loginClient.OperationMethodA(09, TableNameString);//获取分表信息
                if (!args.BoolReturn)
                {
                    App.ShowExceptionMessage(App.GetLanguageInfo("3104T00111", "No Table-T_21_*"));
                    return;
                }
                if (args.ListStringReturn.Count == 0)
                {
                    App.isCutMonth = false;
                }
                List<PartitionTableInfo> listPartTables = new List<PartitionTableInfo>();
                for (int i = 0; i < args.ListStringReturn.Count; i++)
                {
                    string[] values = args.ListStringReturn[i].Split(new[] { ConstValue.SPLITER_CHAR }, StringSplitOptions.RemoveEmptyEntries);
                    if (values.Length == 3)
                    {
                        App.isCutMonth = true;

                        PartitionTableInfo info = new PartitionTableInfo();
                        info.TableName = values[0];
                        info.PartType = TablePartType.DatetimeRange;
                        info.Other1 = values[1];
                        listPartTables.Add(info);
                    }
                }
                App.Session.ListPartitionTables = listPartTables;
                loginClient.Close();
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }


        private void InitBasicOperations()
        {
            try
            {
                List<OperationInfo> listOptInfos = new List<OperationInfo>();
                var optInfo = ListOperations.FirstOrDefault(o => o.ID == S3104Consts.OPT_QueryRecord);
                if (optInfo != null)
                {
                    listOptInfos.Add(optInfo);
                }
                listOptInfos = listOptInfos.OrderBy(o => o.SortID).ToList();
                mListBasicOperations.Clear();
                for (int i = 0; i < listOptInfos.Count; i++)
                {
                    mListBasicOperations.Add(listOptInfos[i]);
                }
            }
            catch (Exception) { }
        }

        /// <summary>
        /// 創建左側功能按鈕
        /// </summary>
        private void CreateOptButtons()
        {
            try
            {
                //BasicOpts
                PanelBasicOpts.Children.Clear();
                Button btn;
                foreach (OperationInfo opt in ListOperations)
                {
                    if (opt.ID == S3104Consts.OPT_QueryRecord)
                    {
                        btn = new Button();
                        btn.Click += BasicOpt_Click;
                        btn.DataContext = opt;
                        btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                        PanelBasicOpts.Children.Add(btn);
                    }
                    if (opt.ID == S3104Consts.OPT_RecordPlayHistory)
                    {
                        btn = new Button();
                        btn.Click += BasicOpt_Click;
                        btn.DataContext = opt;
                        btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                        PanelBasicOpts.Children.Add(btn);
                    }

                    if (LvRecordScoreResult.Items.Count != 0 && mCurrentRecordScoreInfoItem != null && mCurrentRecordScoreInfoItem.ScoreID > 0)
                    {
                        if (opt.ID == S3104Consts.OPT_ViewScoreResult)
                        {
                            btn = new Button();
                            btn.Click += BasicOpt_Click;
                            btn.DataContext = opt;
                            btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                            PanelBasicOpts.Children.Add(btn);
                        }
                        if (ListScoreParam.Where(p => p.GroupID == 310104).Count() >= 4)
                        {
                            DateTime ScoreTime = mCurrentRecordScoreInfoItem.ScoreTime.AddHours(Convert.ToInt32((ListScoreParam.Where(p => p.ParamID == 31010401).First().ParamValue.Substring(9, 4))));
                            DateTime appealTime = ScoreTime.AddHours(Convert.ToInt32((ListScoreParam.Where(p => p.ParamID == 31010402).First().ParamValue.Substring(9, 4))));
                            DateTime appealDeadLine = ScoreTime.AddHours(Convert.ToInt32((ListScoreParam.Where(p => p.ParamID == 31010403).First().ParamValue.Substring(9, 4))));//申诉过期时间
                            if (ListScoreParam.Where(p => p.ParamID == 31010402).First().ParamValue.Substring(8, 1) == "1" && DateTime.Compare(appealTime, DateTime.Now) > 0) { continue; }//分数可见后多久可申诉
                            if (ListScoreParam.Where(p => p.ParamID == 31010403).First().ParamValue.Substring(8, 1) == "1" && DateTime.Compare(appealDeadLine, DateTime.Now) <= 0) { continue; }//申诉过期时间
                        }
                        if (opt.ID == S3104Consts.OPT_AgentAppeal)
                        {
                            btn = new Button();
                            btn.Click += BasicOpt_Click;
                            btn.DataContext = opt;
                            btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                            PanelBasicOpts.Children.Add(btn);
                        }
                    }
                }
                //OtherOpts
                PanelOtherOpts.Children.Clear();
                btn = new Button();
                btn.Click += BasicOpt_Click;
                OperationInfo opt1 = new OperationInfo();
                opt1.ID = S3104Consts.OPT_Goto;
                opt1.Display = App.GetLanguageInfo("3104T00144", "Go To");
                opt1.Description = App.GetLanguageInfo("3104T00144", "Go To");
                btn.DataContext = opt1;
                btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                PanelOtherOpts.Children.Add(btn);
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        #region  其它页面跳转过来的方法
        public void QueryRecord(List<QueryConditionDetail> lstQueryCondition, List<CtrolAgent> lstCtrolAgent, List<DateTimeSpliteAsDay> lstDateTimeSplitAsDay)
        {
            PanelRecordList.IsSelected = true;
            string strConditionString = string.Empty;
            mRecordTotal = 0;
            mPageIndex = 0;
            mPageCount = 0;
            mPageSize = S3104Consts.USER_PARAM_PAGESIZE;
            mMaxRecords = S3104Consts.USER_PARAM_MAXRECORDS;
            mIsQueryContinue = true;
            mListAllRecordInfoItems.Clear();
            mListCurrentRecordInfoItems.Clear();

            try
            {
                PanelBasicOpts.IsEnabled = false;
                
                MyWaiter.Visibility = Visibility.Visible;
                mWorker = new BackgroundWorker();
                mWorker.DoWork += (s, de) =>
                {
                    var tableInfo = App.Session.ListPartitionTables.FirstOrDefault(
                       t => t.TableName == ConstValue.TABLE_NAME_RECORD && t.PartType == TablePartType.DatetimeRange);

                    if (!App.isCutMonth)
                    {
                        tableInfo = App.Session.ListPartitionTables.FirstOrDefault(
                            t => t.TableName == ConstValue.TABLE_NAME_RECORD && t.PartType == TablePartType.VoiceID);
                        string tableName = string.Format("{0}_{1}", ConstValue.TABLE_NAME_RECORD, App.Session.RentInfo.Token);
                        if (tableInfo == null)
                        {
                            strConditionString = MakeConditionString(lstQueryCondition, tableName);
                            QueryRecord(strConditionString, 0, tableName);
                        }
                        else
                        {
                            ////按录音服务器查询
                            strConditionString = MakeConditionString(lstQueryCondition, tableName);
                            QueryRecord(strConditionString, 0, tableName);
                        }
                    }
                    else
                    {
                        //按月
                        DateTimeSpliteAsDay datetimespliteasday = lstDateTimeSplitAsDay.FirstOrDefault();
                        if (datetimespliteasday == null)
                        {
                            App.ShowExceptionMessage(string.Format("DateTimeSpliteAsDay  is null"));
                            return;
                        }
                        DateTime beginTime = Convert.ToDateTime(datetimespliteasday.StartDayTime);
                        DateTime endTime = Convert.ToDateTime(datetimespliteasday.StopDayTime);
                        DateTime baseTime = beginTime;
                        string partTable;
                        int monthCount = Utils.GetTimeMonthCount(beginTime, endTime);
                        for (int i = 0; i <= monthCount; i++)
                        {
                            partTable = baseTime.AddMonths(i).ToString("yyMM");
                            string tableName = string.Format("{0}_{1}_{2}", ConstValue.TABLE_NAME_RECORD,
                                App.Session.RentInfo.Token, partTable);
                            strConditionString = MakeConditionString(lstQueryCondition, tableName);
                            QueryRecord(strConditionString, 0, tableName);
                        }
                    }
                };
                mWorker.RunWorkerCompleted += (s, re) =>
                {
                    mWorker.Dispose();
                    MyWaiter.Visibility = Visibility.Collapsed;
                    PanelBasicOpts.IsEnabled = true;
                    mIsQueryContinue = false;
                    InitBasicOperations();
                    CreateOptButtons();
                    SetPageState();
                };
                mWorker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
                #region 写操作日志
                App.WriteOperationLog(S3104Consts.OPT_QueryRecord.ToString(), ConstValue.OPT_RESULT_FAIL, "");
                #endregion
            }
            #region 写操作日志
            App.WriteOperationLog(S3104Consts.OPT_QueryRecord.ToString(), ConstValue.OPT_RESULT_SUCCESS, "");
            #endregion
        }

        /// <summary>
        /// 座席全部
        /// </summary>
        /// <param name="strConditionString"></param>
        /// <param name="currentRowID"></param>
        /// <param name="tableName"></param>
        private void QueryRecord(string strConditionString, long currentRowID, string tableName)
        {
            try
            {
                if (!mIsQueryContinue) { return; }
                string strSql;
                string tempTableSuffix = App.Session.RentInfo.Token;
                if (tableName.Length > 14)
                {
                    tempTableSuffix = tableName.Substring(tableName.Length - 10, 10);
                }
                //ABCD拼接查询串
                string abcdTable = string.IsNullOrWhiteSpace(ABCDTempStr) ? string.Empty : string.Format("T_31_054_{0} T354,", tempTableSuffix);
                switch (App.Session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT TOP {2} T21.*,{5}T308.C001 AS SCOREID,T308.C004 AS SCORE,T308.C003 AS  TEMPLATEID,T308.C014 AS APPEALMARK,T319.C009 AS AppealState," +
                                               "T342.C009 AS BOOKMARK FROM {6} {3} T21 LEFT JOIN T_31_019_{4} T319 ON T319.C003 = T21.C002 AND T319.C001 IN " +
                                               "(SELECT MIN (C001)	FROM T_31_019_{4} T319 WHERE  T319.C003 = T21.C002) LEFT JOIN T_31_008_{4} T308 ON  T21.C002=T308.C002 AND T308.C001 IN" +
                                               " (SELECT MIN(C001)FROM T_31_008_{4} T308 WHERE T308.C002=T21.C002) LEFT JOIN T_31_042_{4} T342 ON T21.C002=T342.C003 " +
                                               "AND T342.C011 IN(SELECT MIN(C011)FROM T_31_042_{4} T342 WHERE T342.C003=T21.C002)" +
                                               "WHERE  {0} AND T21.C001 > {1} ORDER BY T21.C001,T21.C005"
                           , strConditionString, currentRowID, mPageSize, tableName, App.Session.RentInfo.Token, ABCDTempStr, abcdTable);
                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM (SELECT T21.*,{5}T308.C001 AS SCOREID,T308.C004 AS SCORE,T308.C003 AS  TEMPLATEID,T308.C014 AS APPEALMARK,T319.C009 AS AppealState," +
                                               "case when exists(select C011 from T_31_042_{4} T342 where T342.C003=T21.C002)then '1' else '0' end BOOKMARK " +
                                               "FROM {6} {3} T21 left join t_31_019_{4} t319 on t319.c003 = t21.c002 " +
                                               "left join T_31_008_{4} T308 on   T308.C002 = T21.C002  AND T308.C001 IN(select min(C001) from T_31_008_{4} group by C002) " +
                                               "LEFT JOIN T_31_042_{4} T342 ON T21.C002=T342.C003 and T342.C011 in(select min(C011)from T_31_042_{4} group by C003)" +
                                               "WHERE {0} AND T21.C001 > {1} ORDER BY T21.C001) WHERE ROWNUM <= {2}"
                            , strConditionString, currentRowID, mPageSize, tableName, App.Session.RentInfo.Token, ABCDTempStr, abcdTable);
                        break;
                    default:
                        App.ShowExceptionMessage(App.GetLanguageInfo("3104T00112", string.Format("DBType invalid")));
                        return;
                }
                App.WriteLog("QueryRecord", string.Format("{0}", strSql));
                WriteLog.WriteLogToFile("QueryString", strSql);

                WebRequest webRequest = new WebRequest();
                webRequest.Session = App.Session;
                webRequest.Code = (int)S3104Codes.GetRecordData;
                webRequest.ListData.Add(strSql);
                webRequest.ListData.Add(tableName);
                //Service31041Client client = new Service31041Client();
                Service31041Client client = new Service31041Client(WebHelper.CreateBasicHttpBinding(App.Session), WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service31041"));
                WebReturn webReturn = client.UMPClientOperation(webRequest);
                client.Close();
                //App.ShowInfoMessage(webReturn.Message);
                if (!webReturn.Result)
                {
                    WriteLog.WriteLogToFile("(ಥ_ಥ)", webReturn.Message);
                    App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.Message == S3104Consts.Err_TableNotExit) { return; }
                if (webReturn.ListData == null)
                {
                    App.ShowExceptionMessage(App.GetLanguageInfo("3104T00113", string.Format("Fail. ListData is null")));
                    return;
                }
                if (webReturn.ListData.Count <= 0) { return; }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<RecordInfo>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        continue;
                    }
                    RecordInfo recordInfo = optReturn.Data as RecordInfo;
                    if (recordInfo == null)
                    {
                        App.ShowExceptionMessage(App.GetLanguageInfo("3104T00113", string.Format("Fail. RecordInfo is null")));
                        return;
                    }
                    RecordInfoItem item = new RecordInfoItem(recordInfo);
                    int total = mRecordTotal + 1;
                    if (total > mMaxRecords)
                    {
                        App.ShowInfoMessage(App.GetLanguageInfo("3104T00114",
                          string.Format("Larger than allowed max records, some record can't be displayed")));
                        return;
                    }
                    mRecordTotal = total;
                    item.RowNumber = mRecordTotal;
                    item.Direction = App.GetLanguageInfo(string.Format("3104T0003{0}", 4 - Convert.ToInt32(item.Direction)),
                        item.Direction);
                    item.sAppealMark = item.AppealMark;
                    switch (item.AppealMark)
                    {
                        case "N":
                        case "0":
                            item.AppealMark = App.GetLanguageInfo("3104T00128", "No Appeal");
                            break;
                        case "1":
                            item.AppealMark = App.GetLanguageInfo("3104T00129", "Appealing");
                            break;
                        case "2":
                            item.AppealMark = App.GetLanguageInfo("3104T00069", "Appealed");
                            break;
                        default:
                            break;
                    }
                    item.BookMark = App.GetLanguageInfo(item.BookMark, item.BookMark);
                    item.IsScored = item.IsScored == "0" ? App.GetLanguageInfo("3104T00141", item.IsScored) : App.GetLanguageInfo("3104T00035", item.IsScored);
                    item.DurationStr = Converter.Second2Time(item.Duration);
                    item.Background = GetRecordBackground(item);
                    mListAllRecordInfoItems.Add(item);
                    if (mRecordTotal < mPageSize + 1)
                    {
                        AddNewRecord(item);
                    }
                    currentRowID = item.RowID;//實際翻頁用
                    //currentRowID = item.RowNumber;//數據不夠翻頁測試，重複列出數據，手動設置。
                    SetPageState();
                }
                QueryRecord(strConditionString, currentRowID, tableName);
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }
        //查詢條件
        private string MakeConditionString(List<QueryConditionDetail> lstdetai, string tableName)
        {
            string StrSelect = string.Empty;
            try
            {
                //是否储存座席每天
                foreach (QueryConditionDetail querydetail in lstdetai)
                {
                    if (querydetail.IsEnable == true)
                    {
                        string Temp901 = string.Format("C011");
                        if (App.Session.DBType == 2) Temp901 = string.Format("CONVERT (NVARCHAR(128), C011)");

                        switch (querydetail.ConditionItemID)
                        {
                            case S3104Consts.CON_TIMEFROMTO:
                                {
                                    if (App.Session.DBType == 2)
                                    {

                                        StrSelect += string.Format("T21.C005 >= '{0}' AND T21.C005 <= '{1}' AND ", querydetail.Value01, querydetail.Value02);
                                    }
                                    else if (App.Session.DBType == 3)
                                    {

                                        StrSelect += string.Format("T21.C005 >=TO_DATE ('{0}','YYYY-MM-DD HH24:MI:SS') AND T21.C005 <=TO_DATE( '{1}','YYYY-MM-DD HH24:MI:SS') AND ", querydetail.Value01, querydetail.Value02);
                                    }
                                }
                                break;
                            case S3104Consts.CON_DURATIONFROMTO:
                                {
                                    //暂时没有
                                    StrSelect += string.Format("T21.C012 >= {0} AND T21.C012 <= {1} AND ", querydetail.Value01,
                                                querydetail.Value02);
                                }
                                break;
                            case S3104Consts.CON_EXTENSION_MULTITEXT:
                                {
                                    if (querydetail.IsLike == false)
                                    {
                                        StrSelect +=
                                          string.Format("T21.C042 IN (SELECT {1} FROM T_00_901 WHERE T_00_901.C001 = {0}) AND "
                                              , querydetail.Value01, Temp901);
                                    }
                                    else
                                    {
                                        if (querydetail.Value01.Length > 0)
                                        {
                                            StrSelect += string.Format("T21.C042 LIKE '%{0}%'  AND  ", querydetail.Value01);
                                        }
                                    }

                                }
                                break;
                            case S3104Consts.CON_CALLERID_LIKETEXT:
                                {
                                    if (querydetail.IsLike == false)
                                    {
                                        StrSelect += string.Format("T21.C040 IN (SELECT {1}   FROM T_00_901 WHERE T_00_901.C001={0}) AND  ", querydetail.Value01, Temp901);
                                    }
                                    else
                                    {
                                        if (querydetail.Value01.Length > 0)
                                        {
                                            StrSelect += string.Format("T21.C040 LIKE '%{0}%'  AND  ", querydetail.Value01);
                                        }
                                    }
                                }
                                break;
                            case S3104Consts.CON_CALLEDID_LIKETEXT:
                                {
                                    if (querydetail.IsLike == false)
                                    {
                                        StrSelect += string.Format("T21.C041 IN (SELECT {1}   FROM T_00_901 WHERE T_00_901.C001={0}) AND ", querydetail.Value01, Temp901);
                                    }
                                    else
                                    {
                                        if (querydetail.Value01.Length > 0)
                                        {
                                            StrSelect += string.Format("T21.C041 LIKE '%{0}%' AND ", querydetail.Value01);
                                        }
                                    }

                                }
                                break;
                            case S3104Consts.CON_RECORDREFERENCE_MULTITEXT:
                                {
                                    if (querydetail.IsLike == false)
                                    {
                                        StrSelect += string.Format("T21.C002 IN (SELECT {1}   FROM T_00_901 WHERE T_00_901.C001={0} ) AND ", querydetail.Value01, Temp901);
                                    }
                                    else
                                    {
                                        StrSelect += string.Format("T21.C002 LIKE '%{0}%'  AND  ", querydetail.Value01);
                                    }

                                }
                                break;
                            case S3104Consts.CON_AGENT_MULTITEXT:
                                {
                                    if (querydetail.IsLike == false)
                                    {
                                        //StrSelect += string.Format("{1}.C039 IN  (SELECT {2}   FROM T_00_901 WHERE T_00_901.C001={0}) AND  ", querydetail.Value01, tableName, Temp901);
                                        StrSelect += string.Format("T21.C039 IN  ('{0}') AND  ", App.Session.UserInfo.Account);
                                    }
                                    else
                                    {
                                        StrSelect += string.Format("T21.C039 LIKE '%{0}%' AND ", querydetail.Value01);
                                    }
                                }
                                break;
                            case S3104Consts.CON_CTIREFERENCE_MULTITEXT:
                                {
                                    if (querydetail.IsLike == false)
                                    {
                                        StrSelect += string.Format("T21.C047 IN  (SELECT {1}   FROM T_00_901 WHERE T_00_901.C001={0}) AND ", querydetail.Value01, Temp901);
                                    }
                                    else
                                    {
                                        StrSelect += string.Format("T21.C047 LIKE '%{0}%' AND ", querydetail.Value01);
                                    }
                                }
                                break;
                            case S3104Consts.CON_CHANNELID_MULTITEXT:
                                {
                                    if (querydetail.IsLike == false)
                                    {
                                        StrSelect += string.Format("T21.C038 IN  (SELECT {1}   FROM T_00_901 WHERE T_00_901.C001={0}) AND ", querydetail.Value01, Temp901);
                                    }
                                    else
                                    {
                                        StrSelect += string.Format("T21.C038 LIKE '%{0}%' AND  ", querydetail.Value01);
                                    }
                                }
                                break;
                            case S3104Consts.CON_DIRECTION:
                                {
                                    StrSelect += string.Format("T21.C045 ={0} AND  ", querydetail.Value01);
                                }
                                break;
                            case S3104Consts.CON_HasAppeal:
                                {
                                    StrSelect += string.Format("T319.C009 >='{0}' AND  ", querydetail.Value01);
                                }
                                break;
                            case S3104Consts.CON_BookMark:
                                {
                                    StrSelect += string.Format("T342.c009 IS NOT NULL AND ", querydetail.Value01);
                                }
                                break;
                            case S3104Consts.CON_HasScore:
                                {
                                    StrSelect += string.Format("T308.C001 IS NOT NULL AND  ", querydetail.Value01);
                                }
                                break;
                                
                            //ABCD
                            case S3104Consts.WDE_ServiceAttitude:
                            case S3104Consts.WDE_ProfessionalLevel:
                            case S3104Consts.WDE_RecordDurationError:
                            case S3104Consts.WDE_RepeatCallIn:
                                string column = string.Empty;
                                string sqltemp = string.Empty;
                                switch (querydetail.Value02.Length)//判断T_31_054的列值
                                {
                                    case 1:
                                        column = string.Format("C00{0}", querydetail.Value02);
                                        break;
                                    case 2:
                                        column = string.Format("C0{0}", querydetail.Value02);
                                        break;
                                    case 3:
                                        column = string.Format("C{0}", querydetail.Value02);
                                        break;
                                    default:
                                        continue;//跳出当前循环
                                }
                                switch (querydetail.Value01)//组合条件
                                {
                                    case "0":
                                        sqltemp = string.Format("{0} IN('1','2')", column);
                                        break;
                                    case "1":
                                    case "2":
                                        sqltemp = string.Format("{0}='{1}'", column, querydetail.Value01);
                                        break;
                                }
                                ABCDTempStr = string.Format("T354.{0} AS {1},", column, querydetail.Value03);
                                StrSelect += string.Format(" T354.{0} AND ", sqltemp);
                                break;
                        }//switch.end
                    }//if.end
                }//foreach.end
                if (!string.IsNullOrWhiteSpace(ABCDTempStr))
                {
                    StrSelect += string.Format(" T21.C002 = T354.C201 AND");
                }
            }
            catch (Exception) { }
            return StrSelect + " 1=1 ";
        }

        private void AddNewRecord(RecordInfoItem recordInfoItem)
        {
            Dispatcher.Invoke(new Action(() => mListCurrentRecordInfoItems.Add(recordInfoItem)));
        }

        private void AddNewScore(RecordScoreInfoItem scoreInfoItem)
        {
            Dispatcher.Invoke(new Action(() => mListRecordScoreInfoItems.Add(scoreInfoItem)));
        }

        private void AddNewHistory(RecordPlayHistoryItem recordPlayHistoryItem)
        {
            Dispatcher.Invoke(new Action(() => mListRecordPlayHistoryItems.Add(recordPlayHistoryItem)));
        }

        private Brush GetRecordBackground(RecordInfoItem recordInfoItem)
        {
            try
            {
                int rowNumber = recordInfoItem.RowNumber;
                if (rowNumber % 2 == 0)
                {
                    return Brushes.LightGray;
                }
            }
            catch (Exception) { }
            return Brushes.Transparent;
        }

        private Brush GetRecordBackground(RecordScoreInfoItem scoreInfoItem)
        {
            try
            {
                int rowNumber = scoreInfoItem.RowNumber;
                if (rowNumber % 2 == 0)
                {
                    return Brushes.LightGray;
                }
            }
            catch (Exception) { }
            return Brushes.Transparent;
        }

        private Brush GetPlayHistoryBackground(RecordPlayHistoryItem recordPlayHistory)
        {
            try
            {
                int rowNumber = recordPlayHistory.RowNumber;
                if (rowNumber % 2 == 0)
                {
                    return Brushes.LightGray;
                }
            }
            catch (Exception) { }
            return Brushes.Transparent;
        }


        private LayoutAnchorable GetPanleByContentID(string contentID)
        {
            var panel =
                PanelManager.Layout.Descendents()
                    .OfType<LayoutAnchorable>()
                    .FirstOrDefault(p => p.ContentId == contentID);
            return panel;
        }

        #endregion


        #region  功能操作按纽的操作
        private void QueryRecord_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            QueryRecordInClient();
        }
        public void QueryRecordInClient()
        {
            try
            {
                PopupPanel.Title = App.GetLanguageInfo("3104T00083", "Query Info");
                ShowPanel("PanelScoreList", false);
                mCurrentRecordScoreInfoItem = null;
                LvRecordScoreResult.SelectedItem = null;
                CreateOptButtons();
                ClientQuery queryRecordInTask = new ClientQuery();
                queryRecordInTask.PageParent = this;
                PopupPanel.Content = queryRecordInTask;
                PopupPanel.IsOpen = true;
            }
            catch (Exception) { }
        }


        private void AgentAppeal_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            AgentAppeal();
        }


        public void AgentAppeal()
        {
            try
            {
                var items = LvRecordData.SelectedItems;
                var scoreItem = LvRecordScoreResult.SelectedItems;
                if (scoreItem != null && scoreItem.Count > 0 && items != null && items.Count > 0)
                {
                    RecordInfoItem recordtemp = mCurrentRecordInfoItem;
                    RecordScoreInfoItem scoreInfoItem = mCurrentRecordScoreInfoItem;

                    if (!(scoreInfoItem.Score >= 0.0))
                    {
                        App.ShowInfoMessage(App.GetLanguageInfo("3104T00115", "No Score ,Can't Appeal"));
                        return;
                    }
                    WriteLog.WriteLogToFile("Appeal", recordtemp.SerialID + "\t" + recordtemp.RowID + "\t" + recordtemp.sAppealMark);
                    if (scoreInfoItem.Final.Equals("0"))
                    {
                        App.ShowInfoMessage(App.GetLanguageInfo("3104T00137", "Isn't Final Score ,Can't  Appeal"));
                        return;
                    }
                    if (scoreInfoItem.sAppealMark.Equals("1") || scoreInfoItem.sAppealMark.Equals("2"))
                    {
                        App.ShowInfoMessage(App.GetLanguageInfo("3104T00116", "Already Appeal ,Can't Repeat Appeal"));
                        return;
                    }

                    if (scoreInfoItem.Score >= 0.0)
                    {
                        //此处申诉框 
                        PopupPanel.Title = App.GetLanguageInfo("3104T00002", "Appeal ");
                        AppealPage appealpage = new AppealPage();
                        appealpage.PageParent = this;
                        appealpage.recordinfoitem = recordtemp;
                        appealpage.aScoreInfoItem = scoreInfoItem;
                        PopupPanel.Content = appealpage;
                        PopupPanel.IsOpen = true;
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        public void RecordScoreListView()
        {
            try
            {
                mListRecordScoreInfoItems.Clear();
                int mScoreTotal = 0;
                WebRequest webRequest = new WebRequest();
                webRequest.Session = App.Session;
                webRequest.Code = (int)S3104Codes.GetScoreDate;
                webRequest.ListData.Add(mCurrentRecordInfoItem.SerialID.ToString());
                Service31041Client client = new Service31041Client(WebHelper.CreateBasicHttpBinding(App.Session), WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service31041"));
                //Service31041Client client = new Service31041Client();
                WebReturn webReturn = client.UMPClientOperation(webRequest);
                WriteLog.WriteLogToFile("ScoreListQuery", webReturn.Message);
                client.Close();
                if (!webReturn.Result)
                {
                    App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    App.ShowExceptionMessage(App.GetLanguageInfo("3104T00113", string.Format("Fail. ListData is null")));
                    return;
                }
                if (webReturn.ListData.Count <= 0) { return; }
                DateTime scoreShowTime = new DateTime();
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<RecordScoreInfo>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        continue;
                    }
                    RecordScoreInfo scoreInfo = optReturn.Data as RecordScoreInfo;
                    if (scoreInfo == null)
                    {
                        App.ShowExceptionMessage(App.GetLanguageInfo("3104T00113", string.Format("Fail. RecordInfo is null")));
                        return;
                    }
                    RecordScoreInfoItem item = new RecordScoreInfoItem(scoreInfo);
                    mScoreTotal += 1;
                    item.RowNumber = mScoreTotal;
                    item.sAppealMark = item.AppealMark;
                    item.ScorePerson = AgentAndUserName(item.strScorePerson);
                    item.ScoreTime = item.ScoreTime.ToLocalTime();
                    switch (item.AppealMark)
                    {
                        case "N":
                        case "0":
                            item.AppealMark = App.GetLanguageInfo("3104T00128", "No Appeal");
                            break;
                        case "1":
                            item.AppealMark = App.GetLanguageInfo("3104T00129", "Appealing");
                            break;
                        case "2":
                            item.AppealMark = App.GetLanguageInfo("3104T00069", "Appealed");
                            break;
                        default:
                            break;
                    }
                    //item.IsFinal = App.GetLanguageInfo(string.Format("3104T00{0}", 137 - Convert.ToInt32(item.IsFinal)), item.Final);
                    item.IsFinal = item.Final == "1" ? App.GetLanguageInfo("3104T00142", item.Final) : App.GetLanguageInfo("3104T00143", item.Final);
                    item.Background = GetRecordBackground(item);
                    mListAllScoreItems.Add(item);
                    if (ListScoreParam.Where(p => p.ParamID == 31010401).Count() != 0)
                    {
                        scoreShowTime = item.ScoreTime.AddHours(Convert.ToInt32((ListScoreParam.Where(p => p.ParamID == 31010401).First().ParamValue.Substring(9, 4))));
                        if (ListScoreParam.Where(p => p.ParamID == 31010401).First().ParamValue.Substring(8, 1) == "1" && DateTime.Compare(scoreShowTime, DateTime.Now) > 0) { continue; }//分数可见时间
                    }
                    if (mScoreTotal < mPageSize + 1)
                    {
                        AddNewScore(item);
                    }
                    //翻頁方法不需要寫
                }
                if (!LvRecordScoreResult.Focus())
                {
                    mCurrentRecordScoreInfoItem = null;
                    LvRecordScoreResult.SelectedItem = null;
                    CreateOptButtons();
                }
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }


        private void RecordPlayHistoryView_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            RecordPlayHistoryView();
        }

        public void RecordPlayHistoryView()
        {
            try
            {
                mListRecordPlayHistoryItems.Clear();
                int mHistoryTotal = 0;
                WebRequest webRequest = new WebRequest();
                webRequest.Session = App.Session;
                webRequest.Code = (int)S3104Codes.GetPlayHistory;
                webRequest.ListData.Add(App.ListCtrolAgentInfos[0].AgentID);
                //Service31041Client client = new Service31041Client();
                Service31041Client client = new Service31041Client(WebHelper.CreateBasicHttpBinding(App.Session), WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service31041"));
                WebReturn webReturn = client.UMPClientOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    App.ShowExceptionMessage(App.GetLanguageInfo("3104T00113", string.Format("Fail. ListData is null")));
                    return;
                }
                if (webReturn.ListData.Count <= 0) { return; }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<RecordPlayHistoryInfo>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        continue;
                    }
                    RecordPlayHistoryInfo recordPlayHistoryInfo = optReturn.Data as RecordPlayHistoryInfo;
                    if (recordPlayHistoryInfo == null)
                    {
                        App.ShowExceptionMessage(App.GetLanguageInfo("3104T00117", string.Format("Fail. PlayHistoryInfo is null")));
                        return;
                    }
                    RecordPlayHistoryItem item = new RecordPlayHistoryItem(recordPlayHistoryInfo);
                    mHistoryTotal += 1;
                    item.RowNumber = mHistoryTotal;
                    item.Background = GetPlayHistoryBackground(item);
                    item.PlayDurationStr = Converter.Second2Time(item.PlayDuration);
                    item.StartPositionStr = Converter.Second2Time(item.StartPosition);
                    item.StopPositionStr = Converter.Second2Time(item.StopPosition);
                    item.UserID = AgentAndUserName(item.longUserID);
                    item.Type = App.GetLanguageInfo(item.Type, item.intType.ToString());
                    mListAllHistoryItems.Add(item);
                    if (mHistoryTotal < mPageSize + 1)
                    {
                        AddNewHistory(item);
                    }
                }
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        /// <summary>
        /// 写入播放录音历史
        /// </summary>
        private void WritePlayHistory()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S3104Codes.WritePlayHistory;
                webRequest.Session = App.Session;
                webRequest.ListData.Add("305");//C001 播放历史ID  此数据未使用
                webRequest.ListData.Add(mCurrentRecordInfoItem.SerialID.ToString());//Coo2录音流水号
                webRequest.ListData.Add(App.Session.UserInfo.UserID.ToString());//Coo3用户ID 
                webRequest.ListData.Add(System.DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));//Coo4播放日期
                webRequest.ListData.Add(Convert.ToString(mCurrentRecordInfoItem.Duration));//Coo5播放时长
                webRequest.ListData.Add("1");//Coo6 1为查询播放;2 CQC播放 3智能客户端播放
                webRequest.ListData.Add("1");//Coo7 播放次数
                webRequest.ListData.Add("0");//Coo8   开始位置
                webRequest.ListData.Add(Convert.ToString(mCurrentRecordInfoItem.Duration));//Coo9    结束位置
                //Service31041Client client = new Service31041Client();
                Service31041Client client = new Service31041Client(WebHelper.CreateBasicHttpBinding(App.Session), WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service31041"));
                WebReturn webReturn = client.UMPClientOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    App.ShowExceptionMessage(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }


        void BasicOpt_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var btn = e.Source as Button;
                if (btn != null)
                {
                    var optItem = btn.DataContext as OperationInfo;
                    if (optItem != null)
                    {
                        switch (optItem.ID)
                        {
                            case S3104Consts.OPT_QueryRecord:
                                QueryRecordInClient();
                                break;
                            case S3104Consts.OPT_AgentAppeal:
                                AgentAppeal();
                                break;
                            case S3104Consts.OPT_ViewScoreResult:
                                ViewScoreResult();
                                break;
                            case S3104Consts.OPT_RecordPlayHistory:
                                ShowPanel("PlayHistoryList", true);
                                RecordPlayHistoryView();
                                break;
                            case S3104Consts.OPT_Goto:
                                WebBrowser web = new WebBrowser();
                                web.parentPage = this;
                                PopupPanel.Title = string.Format("WFM");
                                PopupPanel.Content = web;
                                PopupPanel.IsOpen = true;
                                break;
                        }
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        private void ViewScoreResult_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            ViewScoreResult();
        }

        public void ViewScoreResult()
        {
            try
            {
                VoicePlayer.Close();
                var items = LvRecordScoreResult.SelectedItems;
                if (items != null && items.Count > 0 && mCurrentRecordScoreInfoItem != null)
                {
                    UCScoreDetail ucScoreDetail = new UCScoreDetail();
                    ucScoreDetail.PageParent = this;
                    ucScoreDetail.RecordInfoItem = mCurrentRecordInfoItem;
                    ucScoreDetail.aScoreInfoItem = mCurrentRecordScoreInfoItem;
                    ucScoreDetail.ListSftpServers = mListSftpServers;
                    ucScoreDetail.ListDownloadParams = mListDownloadParams;
                    ucScoreDetail.ListEncryptInfo = mListRecordEncryptInfos;
                    ucScoreDetail.Service03Helper = mService03Helper;
                    GetAllObjects();
                    ucScoreDetail.ListAllObjects = mListAllObjects;
                    BorderScoreDetail.Child = ucScoreDetail;
                }
                else
                {
                    return;
                }
                ShowPanel(3);
            }
            catch (Exception ex)
            {

            }
        }

        void VoicePlayer_PlayerEvent(int code, object param)
        {
            try
            {
                //For Test
                //App.WriteLog("Player", string.Format("{0}\t{1}", code, param));

                switch (code)
                {
                    case VoiceCyber.NAudio.Defines.EVT_PLAYING:
                        if (mCurrentRecordPlayItem != null)
                        {
                            TimeSpan ts = TimeSpan.Parse(param.ToString());
                            mCurrentRecordPlayItem.StopPosition = ts.TotalSeconds;
                            mCurrentRecordPlayItem.Duration = mCurrentRecordPlayItem.StopPosition -
                                                              mCurrentRecordPlayItem.StartPosition;
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        #endregion

        #region 教材库


        void GetFoldersFromDB()
        {
            try
            {
                ListFolderInfo.Clear();
                WebRequest webRequest = new WebRequest();
                webRequest.Session = App.Session;
                webRequest.Code = (int)S3104Codes.GetFolder;
                Service31041Client client = new Service31041Client(WebHelper.CreateBasicHttpBinding(App.Session), WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service31041"));
                //Service31041Client client = new Service31041Client();
                WebReturn webReturn = client.UMPClientOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    App.ShowExceptionMessage(webReturn.Message);
                    return;
                }
                if (webReturn.ListData.Count <= 0) { return; }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<FolderInfo>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        continue;
                    }
                    FolderInfo folderInfo = optReturn.Data as FolderInfo;
                    if (folderInfo == null)
                    {
                        App.ShowExceptionMessage(string.Format("Fail. folderInfo is null"));
                        return;
                    }
                    FolderTree folderItem = new FolderTree();
                    folderItem.FolderID = folderInfo.FolderID;
                    folderItem.FolderName = folderInfo.FolderName;
                    folderItem.TreeParentID = folderInfo.TreeParentID;
                    folderItem.TreeParentName = folderInfo.TreeParentName;
                    folderItem.CreatorId = folderInfo.CreatorId;
                    folderItem.CreatorName = folderInfo.CreatorName;
                    folderItem.CreatorTime = folderInfo.CreatorTime;
                    folderItem.UserID1 = folderInfo.UserID1;
                    folderItem.UserID2 = folderInfo.UserID2;
                    folderItem.UserID3 = folderInfo.UserID3;
                    ListFolderInfo.Add(folderItem);
                }
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        public void GetFolders(FolderTree parentInfo, long PId)
        {
            try
            {
                if (PId == -1)
                {
                    GetFoldersFromDB();
                }
                int i = ListFolderInfo.Where(p => p.TreeParentID == PId).Count();
                FolderTree Item = new FolderTree();
                for (int s = 0; s < i; s++)
                {
                    Item = ListFolderInfo.Where(p => p.TreeParentID == PId).ElementAt(s);
                    if (Item == null)
                    {
                        return;
                    }
                    Item.Icon = "/Themes/Default/UMPS3104/Images/document.ico";
                    
                    GetFolders(Item, Item.FolderID);
                    AddChildFolder(parentInfo, Item);
                }

            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void AddChildFolder(FolderTree parentItem, FolderTree item)
        {
            Dispatcher.Invoke(new Action(() => parentItem.AddChild(item)));
        }
        
        private void bFolderTree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                FolderTree Item = bFolderTree.SelectedItem as FolderTree;
                if (Item == null) { return; }
                string UserList = Item.UserID1 + Item.UserID2 + Item.UserID3;
                if (!UserList.Contains(App.Session.UserID.ToString()))
                {
                    App.ShowInfoMessage(App.GetLanguageInfo("3104T00156", "No Permission."));
                    return;
                }
                PanelBookList.IsSelected = true;
                InitBookList(Item.FolderID.ToString());
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }


        void InitBookDocumentColumns()
        {
            try
            {
                string[] lans = "3104T00045,3104T00157,3104T00158,3104T00159,3104T00160,3104T00161,3104T00167".Split(',');
                string[] cols = "RowNumber,FileName,FilePath,FileDescription,mFromType,mFileType,FileType".Split(',');
                int[] colwidths = { 60, 120, 160, 200, 85, 85,50,40 };
                GridView ColumnGridView = new GridView();
                GridViewColumn gvc;
                if (!string.IsNullOrWhiteSpace(mLaosUrl))
                {
                    lans = "3104T00045,3104T00157,3104T00158,3104T00159,3104T00160,3104T00161,3104T00167,".Split(',');
                    cols = "RowNumber,FileName,FilePath,FileDescription,mFromType,mFileType,FileType,Link".Split(',');
                }


                for (int i = 0; i < cols.Count(); i++)
                {
                    gvc = new GridViewColumn();
                    gvc.Header = App.GetLanguageInfo(lans[i], cols[i]);
                    gvc.Width = colwidths[i];
                    if (cols[i] == "FileType")
                    {
                        DataTemplate dt = Resources["CellBookTypeTemplate"] as DataTemplate;
                        gvc.CellTemplate = dt;
                    }
                        else if (cols[i] == "Link")
                    {
                        DataTemplate dt = Resources["CellLaosLinkTemplate"] as DataTemplate;
                        gvc.CellTemplate = dt;
                    }
                    else
                    {
                        gvc.DisplayMemberBinding = new Binding(cols[i]);
                    }
                    ColumnGridView.Columns.Add(gvc);
                }
                LvBooks.View = ColumnGridView;
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void InitBookList(string FolderID)
        {
            mListFilesInfo.Clear();
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = App.Session;
                webRequest.Code = (int)S3104Codes.GetFiles;
                webRequest.ListData.Add(FolderID);
                Service31041Client client = new Service31041Client(WebHelper.CreateBasicHttpBinding(App.Session), WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service31041"));
                //Service31041Client client = new Service31041Client();
                WebReturn webReturn = client.UMPClientOperation(webRequest);
                client.Close();
                if (!webReturn.Result) { return; }
                if (webReturn.ListData.Count <= 0) { return; }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<FilesItemInfo>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        continue;
                    }
                    FilesItemInfo filesItem = optReturn.Data as FilesItemInfo;
                    if (filesItem == null)
                    {
                        App.ShowExceptionMessage(string.Format("Fail. filesItem is null"));
                        return;
                    }
                    filesItem.RowNumber = i + 1;
                    filesItem.mFileType = filesItem.FileType == "0" ? App.GetLanguageInfo("3104T00163", "Is't Wav Document") : App.GetLanguageInfo("3104T00162", "Is Wav Document");
                    filesItem.mFromType = filesItem.FromType == "0" ? App.GetLanguageInfo("3104T00164", "Manual Uploaded") : App.GetLanguageInfo("3104T00165", "Query Upload");
                    
                    mListFilesInfo.Add(filesItem);
                }

            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        void LvBooks_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var item = LvBooks.SelectedItem as FilesItemInfo;
                if (item != null)
                {
                    BrowseHistory(item);
                    if (item.FileType == "0")
                    {
                        bool flag = DownloadFiles(item.FilePath, item.FileName);
                        if (!flag)
                        {
                            #region 写操作日志
                            string strLog = string.Format("{0} {1}{2}", App.Session.UserID.ToString(), Utils.FormatOptLogString("3104T00168"), item.FileName);
                            App.WriteOperationLog(S3104Consts.Opt_Book.ToString(), ConstValue.OPT_RESULT_FAIL, strLog);
                            #endregion
                            App.ShowExceptionMessage(App.GetLanguageInfo("3104T00154", "Operation Falied."));
                            return;
                        }
                        else
                        {
                            #region 写操作日志
                            string strLog = string.Format("{0} {1}{2}", App.Session.UserID.ToString(), Utils.FormatOptLogString("3104T00168"), item.FileName);
                            App.WriteOperationLog(S3104Consts.Opt_Book.ToString(), ConstValue.OPT_RESULT_SUCCESS, strLog);
                            #endregion
                            App.ShowInfoMessage(App.GetLanguageInfo("3104T00169", "DownLoad To DeskTop"));
                            return;
                        }
                    }
                    else 
                    { 
                        if(item.FromType=="0")
                        {
                            PlayFiles(item.FilePath, item.FileName);
                        }
                        else if (item.FromType == "1")
                        {
                            PlayFiles(string.Empty, item.FileName);
                            PlayRecord(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                App.ShowInfoMessage(ex.Message);
            }
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        bool DownloadFiles(string downloadPath, string Fname)
        {
            System.Net.HttpWebRequest LHttpWebRequest = null;
            Stream LStreamResponse = null;
            System.IO.FileStream LStreamCertificateFile = null;
            Fname = downloadPath.Substring(downloadPath.LastIndexOf(Fname));
            string LStrFileFullName = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), Fname);
            string ServerPath = System.IO.Path.Combine(string.Format("http://{0}:{1}", App.Session.AppServerInfo.Address, App.Session.AppServerInfo.Port - 1), downloadPath);
            //string path = System.IO.Path.Combine(string.Format("http://192.168.4.166:8081"), downloadPath);
            ServerPath = ServerPath.Replace("\\", "/");
            try
            {
                LStreamCertificateFile = new FileStream(LStrFileFullName, FileMode.Create);
                LHttpWebRequest = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(ServerPath);
                long LContenctLength = LHttpWebRequest.GetResponse().ContentLength;
                LHttpWebRequest.AddRange(0);
                LStreamResponse = LHttpWebRequest.GetResponse().GetResponseStream();

                byte[] LbyteRead = new byte[1024];
                int LIntReadedSize = LStreamResponse.Read(LbyteRead, 0, 1024);
                while (LIntReadedSize > 0)
                {
                    LStreamCertificateFile.Write(LbyteRead, 0, LIntReadedSize);
                    LIntReadedSize = LStreamResponse.Read(LbyteRead, 0, 1024);
                }
                LStreamCertificateFile.Close(); LStreamCertificateFile.Dispose();
            }
            catch (Exception ex)
            {
                App.WriteLog("DownLoad File To Disk  Failed.", string.Format("FileName : {0} \t  SavePath : {1} \t   ServerPath :{2} \t  Message :{3}", Fname, LStrFileFullName, ServerPath, ex.Message));
                App.ShowExceptionMessage(ex.Message);
                return false;
            }
            finally
            {
                if (LHttpWebRequest != null) { LHttpWebRequest.Abort(); }
                if (LStreamResponse != null) { LStreamResponse.Close(); LStreamResponse.Dispose(); }
                if (LStreamCertificateFile != null) { LStreamCertificateFile.Close(); LStreamCertificateFile.Dispose(); }
            }
            App.WriteLog("DownLoad File To Disk  Sucessed!", string.Format("SavePath : {0} \t   ServerPath :{1} \t  ", LStrFileFullName, ServerPath));
            return true;
        }

        /// <summary>
        /// 播放音频
        /// </summary>
        void PlayFiles(string downloadPath, string Fname)
        {
            string ServerPath = System.IO.Path.Combine(string.Format("http://{0}:{1}", App.Session.AppServerInfo.Address, App.Session.AppServerInfo.Port - 1), downloadPath);
            //string ServerPath = System.IO.Path.Combine(string.Format("http://192.168.4.166:8081"), downloadPath);
            ServerPath = ServerPath.Replace("\\", "/");
            try
            {
                UCPlayBox mUCPlayBox = new UCPlayBox();
                mUCPlayBox.MainPage = this;
                BorderPlayBox.Child = mUCPlayBox;
                mUCPlayBox.PlayUrl = ServerPath;
                PanelPlayBox.IsVisible = true;
            }
            catch (Exception ex)
            {
                #region 写操作日志
                string strLog = string.Format("{0} {1}{2}", App.Session.UserID.ToString(), Utils.FormatOptLogString("FO3104004"), Fname);
                App.WriteOperationLog(S3104Consts.OPT_PlayRecord.ToString(), ConstValue.OPT_RESULT_FAIL, strLog);
                #endregion
                App.WriteLog("Play File Failed.", string.Format("ServerPath : {0} \t   Message :{1}", ServerPath, ex.Message));
                App.ShowExceptionMessage(ex.Message);
            }
            #region 写操作日志
            string strLog1 = string.Format("{0} {1}{2}", App.Session.UserID.ToString(), Utils.FormatOptLogString("FO3104004"), Fname);
            App.WriteOperationLog(S3104Consts.OPT_PlayRecord.ToString(), ConstValue.OPT_RESULT_SUCCESS, strLog1);
            #endregion
            App.WriteLog("Play File Sucessed!", string.Format("ServerPath : {0} ", ServerPath));
        }

        private void PlayRecord(FilesItemInfo fileInfo)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(fileInfo.FileName)) { return; }

                #region 判断是否分表
                string tablename = ConstValue.TABLE_NAME_RECORD + "_" + App.Session.RentInfo.Token;
                if (App.isCutMonth)//有分表 当前仅按年月分表 ex：T_21_001_00000_1405
                {
                    tablename += "_" + fileInfo.FileName.Substring(0, 4);
                }
                #endregion
                string strSql = string.Format("SELECT * FROM {0} WHERE C002={1}", tablename, fileInfo.FileName);

                WebRequest webRequest = new WebRequest();
                webRequest.Session = App.Session;
                webRequest.Code = (int)S3104Codes.GetRecordData;
                webRequest.ListData.Add(strSql);
                webRequest.ListData.Add(tablename);
                //Service31041Client client = new Service31041Client();
                Service31041Client client = new Service31041Client(WebHelper.CreateBasicHttpBinding(App.Session), WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service31041"));
                WebReturn webReturn = client.UMPClientOperation(webRequest);
                client.Close();
                //App.ShowInfoMessage(webReturn.Message);
                if (!webReturn.Result)
                {
                    App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.Message == S3104Consts.Err_TableNotExit) { return; }
                if (webReturn.ListData == null)
                {
                    App.ShowExceptionMessage(App.GetLanguageInfo("3104T00113", string.Format("Fail. ListData is null")));
                    return;
                }
                if (webReturn.ListData.Count <= 0) { return; }
                OperationReturn optReturn = XMLHelper.DeserializeObject<RecordInfo>(webReturn.ListData[0]);
                if (!optReturn.Result)
                {
                    App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                RecordInfo recordInfo = optReturn.Data as RecordInfo;
                if (recordInfo == null)
                {
                    App.ShowExceptionMessage(App.GetLanguageInfo("3104T00113", string.Format("Fail. RecordInfo is null")));
                    return;
                }
                RecordPlayItem first = null;
                RecordInfoItem item = new RecordInfoItem(recordInfo);
                if (item != null)
                {
                    RecordPlayInfo info = new RecordPlayInfo();              //播放历史信息
                    info.RecordReference = item.SerialID.ToString();
                    info.StartPosition = 0;
                    info.StopPosition = item.Duration * 1000;
                    info.Duration = info.StopPosition - info.StartPosition;
                    info.Player = App.Session.UserID;
                    info.PlayTime = DateTime.Now;
                    info.PlayTimes = 0;
                    info.PlayTerminal = 1;
                    RecordPlayItem Playitem = new RecordPlayItem(info);
                    Playitem.RecordInfoItem = item;

                    first = Playitem;
                }
                if (first != null)
                {
                    mCurrentRecordPlayItem = first;
                    LvRecordData.SelectedItem = mCurrentRecordPlayItem;

                    mUCPlayBox = new UCPlayBox();
                    mUCPlayBox.PlayStopped += mUCPlayBox_PlayStopped;
                    mUCPlayBox.MainPage = this;
                    mUCPlayBox.ListEncryptInfo = mListRecordEncryptInfos;
                    //mUCPlayBox.ListUserSettingInfos = mListSettingInfos;
                    mUCPlayBox.ListSftpServers = mListSftpServers;
                    mUCPlayBox.ListDownloadParams = mListDownloadParams;
                    mUCPlayBox.Service03Helper = mService03Helper;
                    mUCPlayBox.RecordPlayItem = first;
                    mUCPlayBox.IsAutoPlay = true;
                    mUCPlayBox.CircleMode = mCircleMode;
                    mUCPlayBox.StartPosition = first.StartPosition;
                    mUCPlayBox.StopPostion = first.StopPosition;
                    BorderPlayBox.Child = mUCPlayBox;

                    //播放历史写入数据库
                    WritePlayHistory();

                    ShowPanel(1);
                    RecordPlayHistoryView();
                }
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        #region  LaosLink

        private void LaosLinkSetting()
        {
            try
            {
                string xmlFileName = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), string.Format("UMP\\UMPS3106"));
                if (!Directory.Exists(xmlFileName)) { Directory.CreateDirectory(xmlFileName); }
                xmlFileName = string.Format("{0}\\LaosLinkSetting.xml", xmlFileName);
                if (!File.Exists(xmlFileName))
                {
                    if (DownLaosXml().Equals(false))
                    {
                        if (File.Exists(xmlFileName)) { File.Delete(xmlFileName); }
                        mLaosUrl = string.Empty; 
                        return; 
                    }
                }
                XmlDocument laosXml = new XmlDocument();
                laosXml.Load(xmlFileName);
                if (laosXml.SelectSingleNode(string.Format("root/Setting/IsLinkToUrl")).InnerText == "1")
                {
                    mLaosUrl = string.Format("{0}/{1}", laosXml.SelectSingleNode(string.Format("root/Setting/LaosURL")).InnerText, laosXml.SelectSingleNode(string.Format("root/Setting/Agentsuffix")).InnerText);
                }
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private bool DownLaosXml()
        {
            System.Net.HttpWebRequest LHttpWebRequest = null;
            Stream LStreamResponse = null;
            System.IO.FileStream LStreamDownXml = null;
            string localPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), string.Format("UMP\\UMPS3106\\LaosLinkSetting.xml"));
            string ServerPath = string.Format("http://{0}:{1}/Components/LinkToLaosURL/LaosLinkSetting.xml", App.Session.AppServerInfo.Address, App.Session.AppServerInfo.Port - 1);
            //string path = string.Format("http://192.168.4.166:8081/Components/LinkToLaosURL/LaosLinkSetting.xml");
            try
            {
                LStreamDownXml = new FileStream(localPath, FileMode.Create);
                LHttpWebRequest = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(ServerPath);
                long LContenctLength = LHttpWebRequest.GetResponse().ContentLength;
                LHttpWebRequest.AddRange(0);
                LStreamResponse = LHttpWebRequest.GetResponse().GetResponseStream();

                byte[] LbyteRead = new byte[1024];
                int LIntReadedSize = LStreamResponse.Read(LbyteRead, 0, 1024);
                while (LIntReadedSize > 0)
                {
                    LStreamDownXml.Write(LbyteRead, 0, LIntReadedSize);
                    LIntReadedSize = LStreamResponse.Read(LbyteRead, 0, 1024);
                }
                LStreamDownXml.Close(); LStreamDownXml.Dispose();
            }
            catch (Exception ex)
            {
                App.WriteLog("DownLoad Xml Failed.", string.Format("LocalPath : {0} \t   ServerPath :{1} \t  Message :{2}", localPath, ServerPath, ex.Message));
                return false;
            }
            finally
            {
                if (LHttpWebRequest != null) { LHttpWebRequest.Abort(); }
                if (LStreamResponse != null) { LStreamResponse.Close(); LStreamResponse.Dispose(); }
                if (LStreamDownXml != null) { LStreamDownXml.Close(); LStreamDownXml.Dispose(); }
            }
            return true;
        } 


        private void LaosLinkCommand_Click(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                var temp = e.Parameter as FilesItemInfo;

                System.Diagnostics.Process ie = new System.Diagnostics.Process();
                ie.StartInfo.FileName = "IEXPLORE.EXE";
                ie.StartInfo.Arguments = string.Format(@"{0}BID={1}&UID={2}", mLaosUrl, temp.FileName, App.Session.UserInfo.Account);
                ie.Start();
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        #endregion

        private void BrowseHistory(FilesItemInfo bookInfo)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = App.Session;
                webRequest.Code = (int)S3104Codes.WriteBrowseHistory;
                OperationReturn optReturn = XMLHelper.SeriallizeObject(bookInfo);
                if (!optReturn.Result)
                {
                    App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                webRequest.ListData.Add(optReturn.Data.ToString());
                webRequest.ListData.Add(App.Session.UserID.ToString());
                webRequest.ListData.Add(App.Session.UserInfo.UserName);
                //Service31041Client client = new Service31041Client();
                Service31041Client client = new Service31041Client(WebHelper.CreateBasicHttpBinding(App.Session), WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service31041"));
                WebReturn webReturn = client.UMPClientOperation(webRequest);
                client.Close();
                //App.ShowInfoMessage(webReturn.Message);
                if (!webReturn.Result)
                {
                    App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }
        #endregion

        #region Record列表操作

        void LvRecordData_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var item = LvRecordData.SelectedItem as RecordInfoItem;
                if (item != null )
                {
                    mCurrentRecordInfoItem = item;
                    PlayRecord();
                    LvRecordData.SelectedItem = null;
                }
            }
            catch (Exception) { }
        }

        private void PlayRecord()
        {
            if (mCurrentRecordInfoItem == null)
            {
                return;
            }
            try
            {
                RecordPlayItem first = null;
                var recordItem = LvRecordData.SelectedItem as RecordInfoItem;
                if (recordItem != null)
                {
                    RecordPlayInfo info = new RecordPlayInfo();              //播放历史信息
                    info.RecordReference = recordItem.SerialID.ToString();
                    info.StartPosition = 0;
                    info.StopPosition = recordItem.Duration * 1000;
                    info.Duration = info.StopPosition - info.StartPosition;
                    info.Player = App.Session.UserID;
                    info.PlayTime = DateTime.Now;
                    info.PlayTimes = 0;
                    info.PlayTerminal = 1;
                    RecordPlayItem item = new RecordPlayItem(info);
                    item.RecordInfoItem = recordItem;

                    first = item;
                }
                if (first != null)
                {
                    mCurrentRecordPlayItem = first;
                    LvRecordData.SelectedItem = mCurrentRecordPlayItem;
                    
                    mUCPlayBox = new UCPlayBox();
                    mUCPlayBox.PlayStopped += mUCPlayBox_PlayStopped;
                    mUCPlayBox.MainPage = this;
                    mUCPlayBox.ListEncryptInfo = mListRecordEncryptInfos;
                    //mUCPlayBox.ListUserSettingInfos = mListSettingInfos;
                    mUCPlayBox.ListSftpServers = mListSftpServers;
                    mUCPlayBox.ListDownloadParams = mListDownloadParams;
                    mUCPlayBox.Service03Helper = mService03Helper;
                    mUCPlayBox.RecordPlayItem = first;
                    mUCPlayBox.RecordInfoItem = recordItem;
                    mUCPlayBox.IsAutoPlay = true;
                    //mUCPlayBox.Play(true);
                    mUCPlayBox.CircleMode = mCircleMode;
                    mUCPlayBox.StartPosition = first.StartPosition;
                    mUCPlayBox.StopPostion = first.StopPosition;
                    BorderPlayBox.Child = mUCPlayBox;
                    //播放历史写入数据库
                    WritePlayHistory();

                    RecordPlayHistoryView();
                    ShowPanel(1);
                }




                //if (mListSftpServers == null || mListSftpServers.Count <= 0)
                //{
                //    App.ShowExceptionMessage(string.Format("SftpServer not exist"));
                //    return;
                //}
                //var sftpServer = mListSftpServers.FirstOrDefault(s => s.HostAddress == mCurrentRecordInfoItem.VoiceIP);
                //if (sftpServer == null)
                //{
                //    App.ShowExceptionMessage(string.Format("{0}\r\n\r\n{1}", "SftpServer not exist", mCurrentRecordInfoItem.VoiceIP));
                //    return;
                //}
                //string strPartInfo = string.Empty;
                ////PartitionTableInfo partInfo =
                ////    App.Session.ListPartitionTables.FirstOrDefault(
                ////        p =>
                ////            p.TableName == ConstValue.TABLE_NAME_RECORD && p.PartType == TablePartType.DatetimeRange);
                //if (App.isCutMonth)
                //{
                //    DateTime startTime = mCurrentRecordInfoItem.StartRecordTime;
                //    strPartInfo = string.Format("{0}{1}", startTime.ToString("yy"), startTime.ToString("MM"));
                //}
                //WebRequest webRequest = new WebRequest();
                //webRequest.Session = App.Session;
                //webRequest.Code = (int)S3104Codes.GetRecordFile;
                //webRequest.ListData.Add(sftpServer.HostAddress);
                //webRequest.ListData.Add(sftpServer.HostPort.ToString());
                //webRequest.ListData.Add(mCurrentRecordInfoItem.SerialID.ToString());
                //webRequest.ListData.Add(mCurrentRecordInfoItem.RowID.ToString());
                //webRequest.ListData.Add(strPartInfo);
                //Service31041Client client = new Service31041Client(WebHelper.CreateBasicHttpBinding(App.Session), WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service31041"));
                ////Service31041Client client = new Service31041Client();
                //WebReturn webReturn = null;
                //SetBusy(true, string.Format("Downloading record file..."));
                //VoicePlayer.IsEnabled = false;
                //mWorker = new BackgroundWorker();
                //mWorker.DoWork += (s, de) =>
                //{
                //    webReturn = client.UMPClientOperation(webRequest);
                //    client.Close();
                //    //播放历史写入数据库
                //    WritePlayHistory();
                //};
                //mWorker.RunWorkerCompleted += (s, re) =>
                //{
                //    mWorker.Dispose();
                //    VoicePlayer.IsEnabled = true;
                //    SetBusy(false, "");
                //    if (webReturn == null)
                //    {
                //        App.ShowInfoMessage(string.Format("WebReturn is null"));
                //        return;
                //    }
                //    if (!webReturn.Result)
                //    {
                //        App.ShowInfoMessage(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                //        return;
                //    }
                //    string path = webReturn.Data;
                //    path = string.Format("{0}://{1}:{2}/MediaData/{3}",
                //        App.Session.AppServerInfo.Protocol,
                //        App.Session.AppServerInfo.Address,
                //        App.Session.AppServerInfo.Port,
                //        path);
                //    App.WriteLog(string.Format("Url:{0}", path));
                //    VoicePlayer.Url = path;
                //    VoicePlayer.Play();
                //    ShowPanel(1);
                //    RecordPlayHistoryView();
                //};
                //mWorker.RunWorkerAsync();
                string strLog = string.Format("{0}{1}", Utils.FormatOptLogString("3104T00001"), mCurrentRecordInfoItem.SerialID);//播放录音是：
                App.WriteOperationLog(S3104Consts.OPT_PlayRecord.ToString(), ConstValue.OPT_RESULT_SUCCESS, strLog);
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }



        void LvRecordData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                ShowPanel("AppealDetail", false);
                mCurrentRecordScoreInfoItem = null;
                LvRecordScoreResult.SelectedItem = null;
                RecordInfoItem item = LvRecordData.SelectedItem as RecordInfoItem;
                if (item != null)
                {
                    mCurrentRecordInfoItem = item;

                    if (!string.IsNullOrWhiteSpace(mCurrentRecordInfoItem.Score))
                    {
                        RecordScoreListView();
                        ShowPanel("PanelScoreList", true);
                        LvRecordScoreResult.Focus();
                    }
                    else
                    {
                        ShowPanel("PanelScoreList", false);
                    }
                    CreateOptButtons();
                    InitBasicOperations();
                    CreateCallInfoItems();
                }
            }
            catch (Exception) { }
        }

        void LvRecordScoreResult_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                RecordScoreInfoItem scoreInfoItem = LvRecordScoreResult.SelectedItem as RecordScoreInfoItem;
                mCurrentRecordScoreInfoItem = scoreInfoItem;
                if (scoreInfoItem != null)
                {
                    CreateOptButtons();
                    if (mCurrentRecordScoreInfoItem.sAppealMark == "1" || mCurrentRecordScoreInfoItem.sAppealMark == "2")
                    {
                        CreatAppealInfoDetail();
                    }
                    else
                    {
                        stkAppealInfo.Children.Clear();
                        ShowPanel("AppealDetail", false);
                    }

                }
            }
            catch (Exception) { }
        }

        public void SetBusy(bool isBusy, string msg)
        {
            try
            {
                if (isBusy)
                {
                    MyWaiter.Visibility = Visibility.Visible;
                    SetStatuMessage(msg);
                }
                else
                {
                    MyWaiter.Visibility = Visibility.Collapsed;
                    SetStatuMessage(string.Empty);
                }
            }
            catch (Exception)
            {

            }
        }
        private void SetStatuMessage(string msg)
        {
            try
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    if (string.IsNullOrEmpty(msg))
                    {
                        TxtStatus.Text = string.Empty;
                    }
                    else
                    {
                        TxtStatus.Text = msg;
                    }
                }));
            }
            catch (Exception)
            {

            }
        }


        private void GetAllObjects()
        {
            try
            {
                mListAllObjects.Clear();
                GetAllObjects(mRootItem);
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void GetAllObjects(ObjectItemClient parentItem)
        {
            try
            {
                for (int i = 0; i < parentItem.Children.Count; i++)
                {
                    ObjectItemClient item = parentItem.Children[i] as ObjectItemClient;
                    if (item != null)
                    {
                        mListAllObjects.Add(item);
                        GetAllObjects(item);
                    }
                }
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void GetAuInfoLists()
        {
            try
            {
                mListAuInfoItems = new ObservableCollection<AgentAndUserInfoItems>();
                WebRequest webRequest = new WebRequest();
                webRequest.Session = App.Session;
                webRequest.Code = (int)S3104Codes.GetAuInfoList;
                webRequest.ListData.Add("11");
                //Service31041Client client = new Service31041Client();
                Service31041Client client = new Service31041Client(WebHelper.CreateBasicHttpBinding(App.Session), WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service31041"));
                WebReturn webReturn = client.UMPClientOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    WriteLog.WriteLogToFile("(ಥ_ಥ)", webReturn.Message);
                    App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    App.ShowExceptionMessage(App.GetLanguageInfo("3104T00113", string.Format("Fail. ListData is null")));
                    return;
                }
                if (webReturn.ListData.Count <= 0) { return; }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<AgentAndUserInfoList>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        continue;
                    }
                    AgentAndUserInfoList auInfoList = optReturn.Data as AgentAndUserInfoList;
                    if (auInfoList == null)
                    {
                        App.ShowExceptionMessage(App.GetLanguageInfo("3104T00113", string.Format("Fail. AgentAndUserInfoList is null")));
                        return;
                    }
                    AgentAndUserInfoItems item = new AgentAndUserInfoItems(auInfoList);
                    mListAuInfoItems.Add(item);
                }
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private string AgentAndUserName(long AuID)
        {
            string AuFullName = string.Empty;
            try
            {
                var temp = mListAuInfoItems.FirstOrDefault(m => m.ID == AuID);
                if (temp != null)
                {
                    AuFullName = DecryptString(temp.FullName);
                }
                if (string.IsNullOrWhiteSpace(AuFullName))
                {
                    AuFullName = AuID.ToString();
                }
            }
            catch (Exception ex)
            {
                AuFullName = AuID.ToString();
                App.ShowExceptionMessage(ex.Message);
            }
            return AuFullName;
        }

        public void ShowPanel(string panelName, bool isShow)
        {
            var panel = mListPanels.FirstOrDefault(p => p.Name == panelName);
            if (panel != null)
            {
                panel.IsVisible = isShow;
                SetPanelVisible();
            }
        }

        public void ShowPanel(int mode)
        {
            try
            {
                switch (mode)
                {
                    //初始模式
                    case 0:
                        ShowPanel("PlayHistoryList", false);
                        ShowPanel("PlayBox", false);
                        ShowPanel("CallInfo", false);
                        ShowPanel("AppealDetail", false);
                        ShowPanel("ScoreDetail", false);
                        break;
                    //查询播放模式
                    case 1:
                        ShowPanel("PlayHistoryList", false);
                        ShowPanel("PlayBox", true);
                        ShowPanel("CallInfo", true);
                        ShowPanel("AppealDetail", false);
                        ShowPanel("ScoreDetail", false);
                        break;
                    //备注模式
                    case 2:
                        ShowPanel("PlayHistoryList", false);
                        ShowPanel("PlayBox", false);
                        ShowPanel("CallInfo", true);
                        if (mCurrentRecordScoreInfoItem != null)
                        {
                            if (mCurrentRecordScoreInfoItem.sAppealMark == "1" || mCurrentRecordScoreInfoItem.sAppealMark == "2")
                            {
                                ShowPanel("AppealDetail", true);
                            }
                            else
                            {
                                ShowPanel("AppealDetail", false);
                            }
                        }
                        ShowPanel("ScoreDetail", false);
                        break;
                    //评分模式
                    case 3:
                        ShowPanel("PlayHistoryList", false);
                        ShowPanel("PlayBox", false);
                        ShowPanel("CallInfo", false);
                        ShowPanel("AppealDetail", false);
                        ShowPanel("ScoreDetail", true);
                        break;
                }
            }
            catch (Exception)
            {

            }
        }


        private void CreateCallInfoItems()
        {
            try
            {
                mListCallInfoPropertyItems.Clear();
                if (mCurrentRecordInfoItem != null)
                {
                    CallInfoPropertyItem item = new CallInfoPropertyItem();
                    DateTime tempDateTime = new DateTime();
                    item.Name = App.GetLanguageInfo("3104T00001", "Record Reference");
                    item.Value = mCurrentRecordInfoItem.SerialID.ToString();
                    mListCallInfoPropertyItems.Add(item);
                    item = new CallInfoPropertyItem();
                    item.Name = App.GetLanguageInfo("3104T00023", "StartRecord Time");
                    if (mCurrentRecordInfoItem.StartRecordTime != tempDateTime)
                    {
                        item.Value = mCurrentRecordInfoItem.StartRecordTime.ToString("yyyy-MM-dd HH:mm:ss");
                    }
                    mListCallInfoPropertyItems.Add(item);
                    item = new CallInfoPropertyItem();
                    item.Name = App.GetLanguageInfo("3104T00046", "StopRecord Time");
                    if (mCurrentRecordInfoItem.StopRecordTime != tempDateTime)
                    {
                        item.Value = mCurrentRecordInfoItem.StopRecordTime.ToString("yyyy-MM-dd HH:mm:ss");
                    }
                    mListCallInfoPropertyItems.Add(item);
                    item = new CallInfoPropertyItem();
                    item.Name = App.GetLanguageInfo("3104T00029", "Duration");
                    if (mCurrentRecordInfoItem.Duration != 0)
                    {
                        item.Value = Converter.Second2Time(mCurrentRecordInfoItem.Duration);
                    }
                    mListCallInfoPropertyItems.Add(item);
                    item = new CallInfoPropertyItem();
                    item.Name = App.GetLanguageInfo("3104T00028", "Direction");
                    item.Value = mCurrentRecordInfoItem.Direction;
                    mListCallInfoPropertyItems.Add(item);
                    item = new CallInfoPropertyItem();
                    item.Name = App.GetLanguageInfo("3104T00048", "Extension");
                    item.Value = mCurrentRecordInfoItem.Extension;
                    mListCallInfoPropertyItems.Add(item);
                    item = new CallInfoPropertyItem();
                    item.Name = App.GetLanguageInfo("3104T00030", "Agent ID");
                    item.Value = mCurrentRecordInfoItem.Agent;
                    mListCallInfoPropertyItems.Add(item);
                    item = new CallInfoPropertyItem();
                    item.Name = App.GetLanguageInfo("3104T00026", "Caller ID");
                    item.Value = mCurrentRecordInfoItem.CallerID;
                    mListCallInfoPropertyItems.Add(item);
                    item = new CallInfoPropertyItem();
                    item.Name = App.GetLanguageInfo("3104T00027", "Called ID");
                    item.Value = mCurrentRecordInfoItem.CalledID;
                    mListCallInfoPropertyItems.Add(item);
                    //ShowPanel(2);
                    ShowPanel("CallInfo", true);
                }
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }
        /// <summary>
        /// 申诉详情--右側信息欄
        /// </summary>
        private void CreatAppealInfoDetail()
        {
            try
            {
                stkAppealInfo.Children.Clear();
                if (mCurrentRecordScoreInfoItem != null && mCurrentRecordScoreInfoItem.ScoreID > 0 && mCurrentRecordScoreInfoItem.sAppealMark != "0" && mCurrentRecordScoreInfoItem.sAppealMark != "N")
                {
                    WebRequest webRequest = new WebRequest();
                    webRequest.Session = App.Session;
                    webRequest.Code = (int)S3104Codes.GetOwerAppeal;
                    webRequest.ListData.Add(mCurrentRecordInfoItem.SerialID.ToString());//录音记录ID C003
                    //webRequest.ListData.Add(mCurrentRecordScoreInfoItem.ScoreID.ToString());//成绩ID C002
                    webRequest.ListData.Add(mCurrentRecordScoreInfoItem.TemplateID.ToString());//评分表ID C003
                    //Service31041Client client = new Service31041Client();
                    Service31041Client client = new Service31041Client(WebHelper.CreateBasicHttpBinding(App.Session), WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service31041"));
                    WebReturn webReturn = client.UMPClientOperation(webRequest);
                    client.Close();
                    if (!webReturn.Result)
                    {
                        App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                        return;
                    }
                    if (webReturn.Message == S3104Consts.AppealOvered)
                    {
                        return;
                    }
                    if (webReturn.ListData == null)
                    {
                        App.ShowExceptionMessage(App.GetLanguageInfo("3104T00113", string.Format("Fail. ListData is null")));
                        return;
                    }
                    if (webReturn.ListData.Count <= 0) { return; }
                    for (int i = 0; i < webReturn.ListData.Count; i++)
                    {
                        OperationReturn optReturn = XMLHelper.DeserializeObject<AppealInfoDetailItem>(webReturn.ListData[i]);
                        if (!optReturn.Result)
                        {
                            App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                            continue;
                        }
                        AppealInfoDetailItem appealInfo = optReturn.Data as AppealInfoDetailItem;
                        if (appealInfo == null)
                        {
                            App.ShowExceptionMessage(App.GetLanguageInfo("3104T00113", string.Format("Fail. AppealInfoDetailItem is null")));
                            return;
                        }
                        if (appealInfo.ID == 1 || appealInfo.ID == 2)
                        {
                            GroupBox gpBox = new GroupBox();
                            gpBox.Header = App.GetLanguageInfo("3104T00002", "Appeal");
                            StackPanel stkPanel = new StackPanel();
                            TextBlock txb = new TextBlock();
                            txb.Text = App.GetLanguageInfo("3104T00042", "Type") + "：" + gpBox.Header;
                            stkPanel.Children.Add(txb);
                            txb = new TextBlock();
                            txb.Text = App.GetLanguageInfo("3104T00041", "Appeal Person") + ":" + AgentAndUserName(appealInfo.PersonID);
                            stkPanel.Children.Add(txb);
                            txb = new TextBlock();
                            txb.Text = App.GetLanguageInfo("3104T00004", "Appeal Time") + "：" + appealInfo.Time.ToString("yyyy/MM/dd HH:mm:ss");
                            stkPanel.Children.Add(txb);
                            txb = new TextBlock();
                            txb.TextWrapping = TextWrapping.Wrap;
                            txb.Text = App.GetLanguageInfo("3104T00008", "Appeal Reason") + "：" + appealInfo.Demo;
                            stkPanel.Children.Add(txb);
                            gpBox.Content = stkPanel;
                            stkAppealInfo.Children.Add(gpBox);
                        }
                        if (appealInfo.ID == 3 || appealInfo.ID == 4)
                        {
                            GroupBox gpBox = new GroupBox();
                            gpBox.Header = App.GetLanguageInfo("3104T00012", "ReCheck");
                            StackPanel stkPanel = new StackPanel();
                            TextBlock txb = new TextBlock();
                            txb.Text = App.GetLanguageInfo("3104T00042", "Type") + "：" + gpBox.Header;
                            stkPanel.Children.Add(txb);
                            txb = new TextBlock();
                            txb.Text = App.GetLanguageInfo("3104T00013", "Check Person") + "：" + AgentAndUserName(appealInfo.PersonID);
                            stkPanel.Children.Add(txb);
                            txb = new TextBlock();
                            txb.Text = App.GetLanguageInfo("3104T00016", "Check Time") + "：" + appealInfo.Time.ToString("yyyy/MM/dd HH:mm:ss");
                            stkPanel.Children.Add(txb);
                            txb = new TextBlock();
                            if (appealInfo.ID == 3)//复核驳回
                            {
                                txb.Text = App.GetLanguageInfo("3104T00044", "ReCheck Result") + "：" + App.GetLanguageInfo("3104T00022", "ReCheck-reject");
                            }
                            else//复核通过
                            {
                                txb.Text = App.GetLanguageInfo("3104T00044", "ReCheck Result") + "：" + App.GetLanguageInfo("3104T00131", "accept");
                            }
                            stkPanel.Children.Add(txb);
                            txb = new TextBlock();
                            txb.TextWrapping = TextWrapping.Wrap;
                            txb.Text = App.GetLanguageInfo("3104T00014", "ReCheck Remarks") + "：" + appealInfo.Demo;
                            stkPanel.Children.Add(txb);
                            gpBox.Content = stkPanel;
                            stkAppealInfo.Children.Add(gpBox);
                        }
                        if (appealInfo.ID == 5 || appealInfo.ID == 6 || appealInfo.ID == 7)
                        {
                            GroupBox gpBox = new GroupBox();
                            gpBox.Header = App.GetLanguageInfo("3104T00009", "Check");
                            StackPanel stkPanel = new StackPanel();
                            TextBlock txb = new TextBlock();
                            txb.Text = App.GetLanguageInfo("3104T00042", "Type") + "：" + gpBox.Header;
                            stkPanel.Children.Add(txb);
                            txb = new TextBlock();
                            txb.Text = App.GetLanguageInfo("3104T00010", "Check Person") + "：" + AgentAndUserName(appealInfo.PersonID);
                            stkPanel.Children.Add(txb);
                            txb = new TextBlock();
                            txb.Text = App.GetLanguageInfo("3104T00015", "Check Time") + "：" + appealInfo.Time.ToString("yyyy/MM/dd HH:mm:ss");
                            stkPanel.Children.Add(txb);
                            txb = new TextBlock();
                            if (appealInfo.ID == 5)//审批通过不修改分数
                            {
                                txb.Text = App.GetLanguageInfo("3104T00043", "Check Result") + "：" + App.GetLanguageInfo("3104T00132", "Review by not modify the score");
                            }
                            if (appealInfo.ID == 6)//审批通过重新评分
                            {
                                txb.Text = App.GetLanguageInfo("3104T00043", "Check Result") + "：" + App.GetLanguageInfo("3104T00133", "Check through to score");
                            }
                            if (appealInfo.ID == 7)//审批驳回
                            {
                                txb.Text = App.GetLanguageInfo("3104T00043", "Check Result") + "：" + App.GetLanguageInfo("3104T00018", "ReCheck-reject");
                            }
                            stkPanel.Children.Add(txb);
                            txb = new TextBlock();
                            txb.TextWrapping = TextWrapping.Wrap;
                            txb.Text = App.GetLanguageInfo("3104T00011", "Check Remarks") + "：" + appealInfo.Demo;
                            stkPanel.Children.Add(txb);
                            gpBox.Content = stkPanel;
                            stkAppealInfo.Children.Add(gpBox);
                        }
                    }
                }
                ShowPanel(2);
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        void TxtPage_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter && mListAllRecordInfoItems.Count != 0)
                {
                    int pageIndex;
                    if (!int.TryParse(TxtPage.Text, out pageIndex))
                    {
                        TxtPage.Text = (mPageIndex + 1).ToString();
                        return;
                    }
                    pageIndex--;
                    if (pageIndex < 0)
                    {
                        pageIndex = 0;
                    }
                    if (pageIndex > mPageCount - 1)
                    {
                        pageIndex = mPageCount - 1;
                    }
                    mPageIndex = pageIndex;
                    FillListView();
                    SetPageState();
                }
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void FillListView()
        {
            try
            {
                mListCurrentRecordInfoItems.Clear();
                int intStart = mPageIndex * mPageSize;
                int intEnd = (mPageIndex + 1) * mPageSize;
                for (int i = intStart; i < intEnd && i < mRecordTotal; i++)
                {
                    mListCurrentRecordInfoItems.Add(mListAllRecordInfoItems[i]);
                }
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }
        /// <summary>
        /// 錄音詳情翻頁
        /// </summary>
        private void SetPageState()
        {
            try
            {
                int pageCount = mRecordTotal / mPageSize;
                int mod = mRecordTotal % mPageSize;
                if (mod > 0)
                {
                    pageCount++;
                }
                mPageCount = pageCount;
                string temp = App.GetLanguageInfo("3104T00056", "Records");
                string strPageInfo = string.Format("{0}/{1} {2} {3}", mPageIndex + 1, mPageCount, mRecordTotal, temp);
                Dispatcher.Invoke(new Action(() =>
                {
                    TxtPageInfo.Text = strPageInfo;
                    TxtPage.Text = (mPageIndex + 1).ToString();
                }));
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }
        #endregion

        #region 播放

        private void LoadRecordEncryptInfos()
        {
            try
            {
                mListRecordEncryptInfos.Clear();
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)RequestCode.WSGetUserParamList;
                webRequest.Session = App.Session;
                webRequest.ListData.Add(App.Session.UserInfo.UserID.ToString());
                webRequest.ListData.Add("1");
                webRequest.ListData.Add(S3104Consts.USER_PARAM_GROUP_ENCRYPTINFO.ToString());
                //App.MonitorHelper.AddWebRequest(webRequest);
                Service11012Client client = new Service11012Client(WebHelper.CreateBasicHttpBinding(App.Session),
                    WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service11012"));
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.DoOperation(webRequest);
                //App.MonitorHelper.AddWebReturn(webReturn);
                client.Close();
                if (!webReturn.Result)
                {
                    App.ShowExceptionMessage(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    App.ShowExceptionMessage(string.Format("WSFail.\tListData is null"));
                    return;
                }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<UserParamInfo>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    UserParamInfo info = optReturn.Data as UserParamInfo;
                    if (info != null)
                    {
                        int paramID = info.ParamID;
                        string strValue = info.ParamValue;
                        string[] arrValue = strValue.Split(new[] { ConstValue.SPLITER_CHAR_3 }, StringSplitOptions.None);
                        string strAddress = string.Empty;
                        string strPassword = string.Empty;
                        string strExpireTime = string.Empty;
                        if (arrValue.Length > 0)
                        {
                            strAddress = arrValue[0];
                        }
                        if (arrValue.Length > 1)
                        {
                            strPassword = arrValue[1];
                        }
                        if (arrValue.Length > 2)
                        {
                            strExpireTime = arrValue[2];
                        }
                        DateTime dtExpireTime = Converter.NumberToDatetime(strExpireTime);
                        if (string.IsNullOrEmpty(strAddress)
                            || string.IsNullOrEmpty(strPassword))
                        {
                            App.WriteLog("LoadEncryptInfo", string.Format("Fail.\tEncryptInfo invalid."));
                            continue;
                        }
                        if (paramID > S3104Consts.USER_PARAM_GROUP_ENCRYPTINFO * 1000
                            && paramID < (S3104Consts.USER_PARAM_GROUP_ENCRYPTINFO + 1) * 1000
                            && dtExpireTime > DateTime.Now.ToUniversalTime())
                        {
                            RecordEncryptInfo encryptInfo = new RecordEncryptInfo();
                            encryptInfo.UserID = App.Session.UserID;
                            encryptInfo.ServerAddress = strAddress;
                            encryptInfo.Password = strPassword;
                            encryptInfo.EndTime = dtExpireTime;
                            mListRecordEncryptInfos.Add(encryptInfo);
                        }
                    }
                }

                App.WriteLog("PageLoad", string.Format("Init RecordEncryptInfo."));
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void LoadSftpServerList()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = App.Session;
                webRequest.Code = (int)S3104Codes.GetSftpServerList;
                webRequest.ListData.Add(App.Session.UserID.ToString());
                Service31041Client client = new Service31041Client(WebHelper.CreateBasicHttpBinding(App.Session), WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service31041"));
                //Service31041Client client=new Service31041Client();
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.UMPClientOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    App.ShowExceptionMessage(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    App.ShowExceptionMessage(string.Format("Fail.\tListData is null"));
                    return;
                }
                mListSftpServers.Clear();
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[i];
                    OperationReturn optReturn = XMLHelper.DeserializeObject<SftpServerInfo>(strInfo);
                    if (!optReturn.Result)
                    {
                        App.WriteLog("LoadSftp", string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        continue;
                    }
                    SftpServerInfo sftpInfo = optReturn.Data as SftpServerInfo;
                    if (sftpInfo == null)
                    {
                        App.WriteLog("LoadSftp", string.Format("Fail.\tSftpServerInfo is null"));
                        continue;
                    }
                    mListSftpServers.Add(sftpInfo);
                }

                App.WriteLog("PageLoad", string.Format("Load SftpServerInfo"));
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void LoadDownloadParamList()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = App.Session;
                webRequest.Code = (int)S3104Codes.GetDownloadParamList;
                webRequest.ListData.Add(App.Session.UserID.ToString());
                Service31041Client client = new Service31041Client(WebHelper.CreateBasicHttpBinding(App.Session), WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service31041"));
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.UMPClientOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    App.ShowExceptionMessage(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    App.ShowExceptionMessage(string.Format("Fail.\tListData is null"));
                    return;
                }
                mListDownloadParams.Clear();
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[i];
                    OperationReturn optReturn = XMLHelper.DeserializeObject<DownloadParamInfo>(strInfo);
                    if (!optReturn.Result)
                    {
                        App.WriteLog("LoadSftp", string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        continue;
                    }
                    DownloadParamInfo info = optReturn.Data as DownloadParamInfo;
                    if (info == null)
                    {
                        App.WriteLog("LoadSftp", string.Format("Fail.\tSftpServerInfo is null"));
                        continue;
                    }
                    mListDownloadParams.Add(info);
                }

                App.WriteLog("PageLoad", string.Format("Load DownloadParams"));
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }


        private void SetService03Helper()
        {
            try
            {
                mService03Helper.HostAddress = App.Session.AppServerInfo.Address;
                if (App.Session.AppServerInfo.SupportHttps)
                {
                    mService03Helper.HostPort = App.Session.AppServerInfo.Port - 4;
                }
                else
                {
                    mService03Helper.HostPort = App.Session.AppServerInfo.Port - 3;
                }
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        void mUCPlayBox_PlayStopped()//播放停止的按钮
        {
            try
            {
                if (mCircleMode == 2)//当mCircleMode为列表循环的时候
                {
                    var playItem = mUCPlayBox.RecordPlayItem;
                    if (playItem != null)
                    {
                        //int index = mListRecordPlayItems.IndexOf(playItem);
                        //if (index < 0) { return; }
                        //if (index == mListRecordPlayItems.Count - 1)
                        //{
                        //    playItem = mListRecordPlayItems[0];
                        //}
                        //else
                        //{
                        //    playItem = mListRecordPlayItems[index + 1];
                        //}
                        LvRecordData.SelectedItem = playItem;
                        mUCPlayBox = new UCPlayBox();
                        mUCPlayBox.PlayStopped += mUCPlayBox_PlayStopped;
                        mUCPlayBox.MainPage = this;
                        mUCPlayBox.ListEncryptInfo = mListRecordEncryptInfos;
                        //mUCPlayBox.ListUserSettingInfos = mListSettingInfos;
                        mUCPlayBox.ListSftpServers = mListSftpServers;
                        mUCPlayBox.ListDownloadParams = mListDownloadParams;
                        mUCPlayBox.Service03Helper = mService03Helper;
                        mUCPlayBox.RecordPlayItem = playItem;
                        mUCPlayBox.IsAutoPlay = true;
                        mUCPlayBox.CircleMode = mCircleMode;
                        mUCPlayBox.StartPosition = playItem.StartPosition;
                        mUCPlayBox.StopPostion = playItem.StopPosition;
                        BorderPlayBox.Child = mUCPlayBox;
                    }
                }
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        #endregion

        #region 页头命令
        private void Init()
        {
            try
            {
                PageName = "UMPTaskAssignPage";
                StylePath = "UMPS3104/MainPageStyle.xaml";
                ThemeInfo = App.Session.ThemeInfo;
                LangTypeInfo = App.Session.LangTypeInfo;
                AppServerInfo = App.Session.AppServerInfo;

                PageHead.SessionInfo = App.Session;
                PageHead.InitInfo();
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        void PageHead_PageHeadEvent(object sender, PageHeadEventArgs e)
        {
            try
            {
                switch (e.Code)
                {
                    case 100:
                        ThemeInfo themeInfo = e.Data as ThemeInfo;
                        if (themeInfo != null)
                        {
                            ThemeInfo = themeInfo;
                            App.Session.ThemeInfo = themeInfo;
                            ChangeTheme();
                            WriteXmlTheme(themeInfo);
                            //SendThemeChangeMessage();
                        }
                        break;
                    case 110:
                        LangTypeInfo langType = e.Data as LangTypeInfo;
                        if (langType != null)
                        {
                            LangTypeInfo = langType;
                            App.Session.LangTypeInfo = langType;
                            App.InitLanguageInfos();
                            ChangeLanguage();
                            WriteXmlLanguage(langType);
                            PopupPanel.ChangeLanguage();
                            //SendLanguageChangeMessage();
                        }
                        break;
                    case 121:
                        OpenCloseLeftPanel();
                        break;
                    case 120:
                        //更改密码
                        PopupPanel.Title = App.GetLanguageInfo("3104T00096", "Change PassWord ");
                        ChangePassWord changePassWord = new ChangePassWord();
                        changePassWord.PageParent = this;
                        PopupPanel.Content = changePassWord;
                        PopupPanel.IsOpen = true;
                        break;
                    case 201:
                        //注销
                        ReturnLogin();
                        break;
                    case 202:
                        //返回登录
                        ReturnLogin();
                        break;
                    case PageHeadEventArgs.EVT_OPENIM:
                        OpenIMPanel();
                        break;
                }
            }
            catch (Exception)
            {

            }
        }

        //返回登录界面
        private void ReturnLogin()
        {
            //关闭IM面板
            if (mIMMainPage != null)
            {
                try
                {
                    mIMMainPage.LogOff();
                }
                catch { }
                mIMMainPage = null;
            }
            try
            {
                mIsQueryContinue = false;
                ListOperations.Clear();
                List<string> clientListString = new List<string>();
                clientListString.Add(App.renterID);//租户编码
                clientListString.Add(App.Session.UserInfo.UserID.ToString());//用户编码
                clientListString.Add(App.Session.SessionID);//登录分配的SessionID
                Service00000Client loginClient = new Service00000Client(WebHelper.CreateBasicHttpBinding(App.Session),
                                    WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service00000"));
                OperationDataArgs args = loginClient.OperationMethodA(42, clientListString);//43登录、42退出
                string temp = App.DecryptString(args.StringReturn);
                VoicePlayer.Close();
                loginClient.Close();
                IsClose = false;
                Login lg = new Login();
                lg.Show();
                this.Close();
            }
            catch (Exception) { }
        }

        /// <summary>
        /// 更新C:\Users\Administrator\AppData\Local\UMP.Client\UMPClientInit.xml文件 默认样式
        /// </summary>
        /// <param name="themeInfo"></param>
        private void WriteXmlTheme(ThemeInfo themeInfo)
        {
            try
            {
                string xmlFileName = string.Empty;
                //xmlFileName = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"UMP.Client\UMPClientInit.xml");
                xmlFileName = Environment.CurrentDirectory + string.Format("\\UMPClientInit.xml");
                App.XmlPathString = xmlFileName;
                if (!App.IsExsitXml())
                {
                    App.ShowExceptionMessage(App.GetLanguageInfo("3104T00118", string.Format("UMPClientInit.xml  Is Not Exist")));
                    return;
                }
                DataSet Xmlds = new DataSet();
                Xmlds.ReadXml(xmlFileName);
                Xmlds.Tables["SupportStyle"].Rows[0]["Name"] = themeInfo.Name;
                Xmlds.Tables["SupportStyle"].Rows[0]["Color"] = themeInfo.Color;
                Xmlds.WriteXml(xmlFileName);
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        /// <summary>
        /// 更新C:\Users\Administrator\AppData\Local\UMP.Client\UMPClientInit.xml文件 默认语言
        /// </summary>
        /// <param name="langType"></param>
        private void WriteXmlLanguage(LangTypeInfo langType)
        {
            try
            {
                string xmlFileName = string.Empty;
                //xmlFileName = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"UMP.Client\UMPClientInit.xml");
                xmlFileName = Environment.CurrentDirectory + string.Format("\\UMPClientInit.xml");
                App.XmlPathString = xmlFileName;
                if (!App.IsExsitXml())
                {
                    App.ShowExceptionMessage(App.GetLanguageInfo("3104T00118", string.Format("UMPClientInit.xml  Is Not Exist")));
                    return;
                }
                DataSet Xmlds = new DataSet();
                Xmlds.ReadXml(xmlFileName);
                Xmlds.Tables["LanguagesType"].Rows[0]["LocalLanguage"] = langType.LangID;
                Xmlds.WriteXml(xmlFileName);
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void OpenCloseLeftPanel()
        {
            try
            {
                if (GridLeft.Width.Value > 0)
                {
                    GridLeft.Width = new GridLength(0);
                }
                else
                {
                    GridLeft.Width = new GridLength(200);
                }
            }
            catch (Exception)
            {

            }
        }

        public void OpenPasswordPanel(UMPUserControl content)
        {
            try
            {
                PopupPanelPassword.Content = content;
                PopupPanelPassword.IsOpen = true;
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        public override void ChangeLanguage()
        {
            try
            {
                base.ChangeLanguage();

                //Operation
                for (int i = 0; i < ListOperations.Count; i++)
                {
                    ListOperations[i].Display = App.GetLanguageInfo(string.Format("FO{0}", ListOperations[i].ID),
                        ListOperations[i].ID.ToString());
                }
                CreateOptButtons();
                InitTaskRecordColumns();
                InitRecordScoreResultColumns();
                InitPlayHistoryColumns();
                InitBookDocumentColumns();
                CreateCallInfoItems();
                CreatAppealInfoDetail(); 

                ExpanderBasic.Header = App.GetLanguageInfo("3104T00061", "Basic Operations");
                ExpanderOther.Header = App.GetLanguageInfo("3104T00062", "Other Position");
                #region panel
                var panelItem = mListPanels.FirstOrDefault(p => p.Name == "RecordList");
                if (panelItem != null)
                {
                    panelItem.Title = App.GetLanguageInfo("3104T00036", "Record List");
                }
                panelItem = mListPanels.FirstOrDefault(p => p.Name == "PlayHistoryList");
                if (panelItem != null)
                {
                    panelItem.Title = App.GetLanguageInfo("3104T00076", "PlayHistory List");
                }
                panelItem = mListPanels.FirstOrDefault(p => p.Name == "PanelScoreList");
                if (panelItem != null)
                {
                    panelItem.Title = App.GetLanguageInfo("3104T00134", "RecordScore List");
                }
                panelItem = mListPanels.FirstOrDefault(p => p.Name == "PlayBox");
                if (panelItem != null)
                {
                    panelItem.Title = App.GetLanguageInfo("3104T00037", "Play Box");
                }
                panelItem = mListPanels.FirstOrDefault(p => p.Name == "CallInfo");
                if (panelItem != null)
                {
                    panelItem.Title = App.GetLanguageInfo("3104T00039", "Call Infomation");
                }
                panelItem = mListPanels.FirstOrDefault(p => p.Name == "AppealDetail");
                if (panelItem != null)
                {
                    panelItem.Title = App.GetLanguageInfo("3104T00038", "Appeal Detail");
                }
                panelItem = mListPanels.FirstOrDefault(p => p.Name == "PanelScore");
                if (panelItem != null)
                {
                    panelItem.Title = App.GetLanguageInfo("3104T00086", "Score Table");
                }
                panelItem = mListPanels.FirstOrDefault(p => p.Name == "ScoreDetail");
                if (panelItem != null)
                {
                    panelItem.Title = App.GetLanguageInfo("3104T00075", "Score Detail");
                }

                var panel = GetPanleByContentID("PanelRecordList");
                if (panel != null)
                {
                    panel.Title = App.GetLanguageInfo("3104T00036", "Record List");
                }
                panel = GetPanleByContentID("PanelPlayHistoryList");
                if (panel != null)
                {
                    panel.Title = App.GetLanguageInfo("3104T00076", "PlayHistory List");
                }
                panel = GetPanleByContentID("PanelScoreList");
                if (panel != null)
                {
                    panel.Title = App.GetLanguageInfo("3104T00134", "RecordScore List");
                }
                panel = GetPanleByContentID("PanelPlayBox");
                if (panel != null)
                {
                    panel.Title = App.GetLanguageInfo("3104T00037", "Play Box");
                }
                panel = GetPanleByContentID("PanelCallInfo");
                if (panel != null)
                {
                    panel.Title = App.GetLanguageInfo("3104T00039", "Call Infomation");
                }
                panel = GetPanleByContentID("PanelAppealDetail");
                if (panel != null)
                {
                    panel.Title = App.GetLanguageInfo("3104T00038", "Appeal Detail");
                }
                panel = GetPanleByContentID("PanelScore");
                if (panel != null)
                {
                    panel.Title = App.GetLanguageInfo("3104T00086", "Score Table");
                }
                panel = GetPanleByContentID("PanelScoreDetail");
                if (panel != null)
                {
                    panel.Title = App.GetLanguageInfo("3104T00075", "Score Detail");
                }
                panel = GetPanleByContentID("PanelBookList");
                if (panel != null)
                {
                    panel.Title = App.GetLanguageInfo("3104T00166", "Book List");
                }
                #endregion

                CreateToolBarButtons();
            }
            catch (Exception ex)
            {

            }
        }
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
                    // App.ShowExceptionMessage("1" + ex.Message);
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
                string uri = string.Format("/Themes/Default/UMPS3104/MainPageStatic.xaml");
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
        #endregion

        #region IM
        private void CreateIMPanel()
        {
            try
            {
                var session = App.Session;
                if (session == null) { return; }

                List<string> listMsgs = new List<string>();
                mIMMainPage = new IMMainPage(session);
                mIMMainPage.StatusChangeEvent += IMMainPage_StatusChangeEvent;
                mIMMainPage.Width = 780;
                mIMMainPage.Height = 500;
                mIMMainPage.Login();
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        private void OpenIMPanel()
        {
            try
            {
                var session = App.Session;
                if (session == null) { return; }
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S3104Codes.GetIMRole;
                webRequest.Session = App.Session;
                webRequest.ListData.Add("1601");
                //Service31041Client client = new Service31041Client();
                Service31041Client client = new Service31041Client(WebHelper.CreateBasicHttpBinding(App.Session), WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service31041"));
                WebReturn webReturn = new WebReturn();
                webReturn = client.UMPClientOperation(webRequest);
                client.Close();
                if (webReturn.Result.Equals(false)) { return; }
                if (mIMMainPage == null)
                {
                    CreateIMPanel();
                }

                PopupPanelIM.Title = string.Format("{0}", session.UserInfo.Account);
                PopupPanelIM.Width = 800;
                PopupPanelIM.Height = 550;
                PopupPanelIM.IsModal = false;
                PopupPanelIM.Content = mIMMainPage;
                PopupPanelIM.IsOpen = true;
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        //IM Chart 事件代码
        private const int IM_STATUS_LOGIN = 1;
        private const int IM_STATUS_LOGOFF = 2;
        private const int IM_STATUS_NEWMSG = 3;

        protected virtual void IMMainPage_StatusChangeEvent(int msgType, string strMsg)
        {
            App.WriteLog("IMMessage", string.Format("{0}\t{1}", msgType, strMsg));
            switch (msgType)
            {
                case IM_STATUS_LOGIN:

                    break;
                case IM_STATUS_LOGOFF:

                    break;
                case IM_STATUS_NEWMSG:
                    mMsgCount++;
                    SetIMMsgState();
                    break;
            }
        }

        private void SetIMMsgState()
        {
            try
            {
                var pageHead = PageHead;
                if (pageHead == null) { return; }

                if (mMsgCount > 0)
                {
                    pageHead.IsMsgCountVisible = true;
                    if (mMsgCount > 9)
                    {
                        pageHead.MsgCount = "9+";
                    }
                    else
                    {
                        pageHead.MsgCount = mMsgCount.ToString();
                    }
                }
                else
                {
                    pageHead.IsMsgCountVisible = false;
                    pageHead.MsgCount = mMsgCount.ToString();
                }
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        #endregion

        #region E-Learning
        private void CreateExamBtn()
        {
            try
            {

                WebRequest webRequest = new WebRequest();
                webRequest.Session = App.Session;
                webRequest.Code = (int)S3104Codes.GetExamInfo;
                Service31041Client client = new Service31041Client(WebHelper.CreateBasicHttpBinding(App.Session), WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service31041"));
                //Service31041Client client = new Service31041Client();
                WebReturn webReturn = client.UMPClientOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    App.ShowExceptionMessage(webReturn.Message);
                    return;
                }
                if (webReturn.ListData.Count <= 0) { return; }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<FolderInfo>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        continue;
                    }
                    FolderInfo folderInfo = optReturn.Data as FolderInfo;
                    if (folderInfo == null)
                    {
                        App.ShowExceptionMessage(string.Format("Fail. folderInfo is null"));
                        return;
                    }
                    FolderTree folderItem = new FolderTree();
                    folderItem.FolderID = folderInfo.FolderID;
                    folderItem.FolderName = folderInfo.FolderName;
                    folderItem.TreeParentID = folderInfo.TreeParentID;
                    folderItem.TreeParentName = folderInfo.TreeParentName;
                    folderItem.CreatorId = folderInfo.CreatorId;
                    folderItem.CreatorName = folderInfo.CreatorName;
                    folderItem.CreatorTime = folderInfo.CreatorTime;
                    folderItem.UserID1 = folderInfo.UserID1;
                    folderItem.UserID2 = folderInfo.UserID2;
                    folderItem.UserID3 = folderInfo.UserID3;
                    ListFolderInfo.Add(folderItem);
                }
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }




        #endregion

        #region 获取全局参数配置
        private void GetScoreParams()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = App.Session;
                webRequest.Code = (int)RequestCode.WSGetGlobalParamList;
                webRequest.ListData.Add("31");
                webRequest.ListData.Add("0");
                webRequest.ListData.Add("310104");
                Service11012Client client = new Service11012Client(WebHelper.CreateBasicHttpBinding(App.Session), WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service11012"));
                //WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    App.ShowExceptionMessage(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                }

                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<GlobalParamInfo>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    }
                    GlobalParamInfo GlobalParamInfo = optReturn.Data as GlobalParamInfo;
                    ListScoreParam.Add(GlobalParamInfo);
                }
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        #endregion

        #region Encryption and Decryption

        public static string EncryptString(string strSource)//加密
        {
            string strTemp = EncryptionAndDecryption.EncryptDecryptString(strSource,
             CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004),
             EncryptionAndDecryption.UMPKeyAndIVType.M004);
            return strTemp;
        }

        public static string DecryptString(string strSource)//解密
        {
            string strTemp = EncryptionAndDecryption.EncryptDecryptString(strSource,
              CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104),
              EncryptionAndDecryption.UMPKeyAndIVType.M104);
            return strTemp;
        }

        public static string CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType aKeyIVID)
        {
            string lStrReturn;
            int LIntRand;
            Random lRandom = new Random();
            string LStrTemp;

            try
            {
                lStrReturn = DateTime.Now.ToString("yyyyMMddHHmmss");
                LIntRand = lRandom.Next(0, 14);
                LStrTemp = LIntRand.ToString("00");
                lStrReturn = lStrReturn.Insert(LIntRand, "VCT");
                LIntRand = lRandom.Next(0, 17);
                LStrTemp += LIntRand.ToString("00");
                lStrReturn = lStrReturn.Insert(LIntRand, "UMP");
                LIntRand = lRandom.Next(0, 20);
                LStrTemp += LIntRand.ToString("00");
                lStrReturn = lStrReturn.Insert(LIntRand, ((int)aKeyIVID).ToString("000"));

                lStrReturn = EncryptionAndDecryption.EncryptStringY(LStrTemp + lStrReturn);
            }
            catch { lStrReturn = string.Empty; }

            return lStrReturn;
        }

        #endregion
    }
}
