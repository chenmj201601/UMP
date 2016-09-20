//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    80184b1f-52c6-471e-bb3d-bac4f24a8079
//        CLR Version:              4.0.30319.18444
//        Name:                     QMMainView
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS3102
//        File Name:                QMMainView
//
//        created by Charley at 2014/11/2 14:54:06
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using UMPS3102.Codes;
using UMPS3102.Commands;
using UMPS3102.Converters;
using UMPS3102.Models;
using UMPS3102.Wcf11012;
using UMPS3102.Wcf31021;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common31021;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;
using VoiceCyber.UMP.Controls.Players;
using VoiceCyber.Wpf.AvalonDock.Layout;
using VoiceCyber.Wpf.AvalonDock.Layout.Serialization;
using VoiceCyber.Wpf.CustomControls;
using Defines = VoiceCyber.Common.Defines;

namespace UMPS3102
{
    /// <summary>
    /// QMMainView.xaml 的交互逻辑
    /// </summary>
    public partial class QMMainView
    {

        #region Members

        private ObservableCollection<OperationInfo> mListBasicOperations;
        private ObservableCollection<ViewColumnInfo> mListRecordDataColumns;
        private ObservableCollection<ViewColumnInfo> mListRecordPlayColumns;
        private ObservableCollection<ViewColumnInfo> mListScoreSheetColumns;
        private BackgroundWorker mWorker;
        private List<PanelItem> mListPanels;
        private ObservableCollection<QueryCondition> mListQueryConditions;
        private ObservableCollection<RecordInfoItem> mListAllRecordInfoItems;
        private ObservableCollection<RecordInfoItem> mListCurrentRecordInfoItems;//这个是在查询出来的结果记录框框里面的每一个记录
        private ObservableCollection<RecordPlayItem> mListRecordPlayItems;
        private ObservableCollection<BasicScoreSheetItem> mListScoreSheetItems;
        private ObservableCollection<CallInfoPropertyItem> mListCallInfoPropertyItems;
        private ObservableCollection<PlayDateTypeItem> mListPlayDateTypeItems;
        private RecordInfoItem mCurrentRecordInfoItem;
        private RecordPlayItem mCurrentRecordPlayItem;
        private ObjectItem mRootItem;
        private List<ObjectItem> mListSelectedObjects;
        private List<ObjectItem> mListAllObjects;
        private List<SettingInfo> mListSettingInfos;
        private List<SftpServerInfo> mListSftpServers;
        private List<DownloadParamInfo> mListDownloadParams;
        private List<RecordEncryptInfo> mListRecordEncryptInfos;
        private UCPlayBox mUCPlayBox;
        private UCRecordMemo mUCRecordMemo;
        private UCRecordBookmark mUCRecordBookmark;
        private ExportDataHelper mExportDataHelper;
        private Service03Helper mService03Helper;
        private UCScoreDetail ucScoreDetail;
        private UCConversationInfo ucConversationInfo;

        private int mCircleMode;
        private string mLayoutInfo;
        private int mPageIndex;//页的索引,这个是从0开始算的
        private int mPageCount;
        private int mPageSize;
        private int mRecordTotal;
        private int mMaxRecords;
        private bool mIsQueryContinue;
        private bool mIsSkipQuikQuery; //是否跳过快速查询   true是跳过 false 是不跳过
        private bool mIsSortAsending;

        //评分控制的相关的参数
        public static List<ScoreParamsInfo> mScoreParams;

        //分组方式的全局参数
        public string GroupingWay;

        //判断当前录音是否申诉过
        public static bool IsCompained_;

        //判断当前录音是否分配过
        public static bool IsTasked_;

        public bool IsBusy;

        //管理对象在页面打开的时候先存入临时表，然后创建一个临时表ID
        private string mManageObjectQueryID;
        private bool mIsSaveTempTable;

        //存能够管理的坐席和分机
        //private List<string> mListControlAgent;
        //private List<string> mListControlExtension;
        #endregion


        #region StaticMembers

        //这个是各种操作的列表  可以看OperationInfo这个类里面的,其中每一个都是代表的一种操作
        public static ObservableCollection<OperationInfo> ListOperations = new ObservableCollection<OperationInfo>();


        #endregion


        public QMMainView()
        {
            InitializeComponent();

            mListRecordDataColumns = new ObservableCollection<ViewColumnInfo>();
            mListRecordPlayColumns = new ObservableCollection<ViewColumnInfo>();
            mListBasicOperations = new ObservableCollection<OperationInfo>();
            mListScoreSheetColumns = new ObservableCollection<ViewColumnInfo>();
            mListPanels = new List<PanelItem>();
            mListSelectedObjects = new List<ObjectItem>();
            mListAllObjects = new List<ObjectItem>();
            mListSettingInfos = new List<SettingInfo>();
            mListSftpServers = new List<SftpServerInfo>();
            mListDownloadParams = new List<DownloadParamInfo>();
            mListRecordEncryptInfos = new List<RecordEncryptInfo>();
            mListQueryConditions = new ObservableCollection<QueryCondition>();
            mListAllRecordInfoItems = new ObservableCollection<RecordInfoItem>();
            mListCurrentRecordInfoItems = new ObservableCollection<RecordInfoItem>();
            mListRecordPlayItems = new ObservableCollection<RecordPlayItem>();
            mListScoreSheetItems = new ObservableCollection<BasicScoreSheetItem>();
            mListCallInfoPropertyItems = new ObservableCollection<CallInfoPropertyItem>();
            mListPlayDateTypeItems = new ObservableCollection<PlayDateTypeItem>();
            mManageObjectQueryID = string.Empty;
            mIsSaveTempTable = false;

            mScoreParams = new List<ScoreParamsInfo>();

            mRootItem = new ObjectItem();
            mService03Helper = new Service03Helper();
            mService03Helper.Debug += Service03Helper_Debug;
            mCircleMode = 0;
            mPageIndex = 0;
            mPageCount = 0;
            mPageSize = 200;
            mRecordTotal = 0;
            mMaxRecords = 100000;
            mIsQueryContinue = false;
            mIsSortAsending = true;


            ToolButtonItem aa = new ToolButtonItem();
            aa.Icon = "Images/StopRecord.png";
            QueryStop.DataContext = aa;
            QueryStop.SetResourceReference(StyleProperty, "ToolBarButtonStyle");
            QueryStop.Click += QueryStop_Click;
            //QueryStop.ToolTip = CurrentApp.GetLanguageInfo("3102TIP0128","Query  Stop");
            Unloaded += QMMainPage_Unloaded;
            ComboQueryConditions.SelectionChanged += ComboQueryConditions_SelectionChanged;
            TxtPage.KeyUp += TxtPage_KeyUp;
            LvRecordData.SelectionChanged += LvRecordData_SelectionChanged;
            LvScoreSheet.MouseDoubleClick += LvScoreSheet_MouseDoubleClick;
            ComboPlayList.SelectionChanged += ComboPlayList_SelectionChanged;
        }

        private void QueryStop_Click(object sender, RoutedEventArgs e)
        {
            mIsQueryContinue = false;
            //QueryStop.Visibility = Visibility.Collapsed;
        }

        void QMMainPage_Unloaded(object sender, RoutedEventArgs e)
        {
            if (mService03Helper != null)
            {
                mService03Helper.Close();
            }
        }

        #region Init and Load

        protected override void Init()
        {
            try
            {
                PageName = "QMMainView";
                StylePath = "UMPS3102/QMMainPage.xaml";

                base.Init();

                ComboQueryConditions.ItemsSource = mListQueryConditions;
                LvRecordData.ItemsSource = mListCurrentRecordInfoItems;
                LvRecordPlay.ItemsSource = mListRecordPlayItems;
                LvScoreSheet.ItemsSource = mListScoreSheetItems;

                TvObjects.ItemsSource = mRootItem.Children;
                ListBoxCallInfo.ItemsSource = mListCallInfoPropertyItems;
                ComboPlayList.ItemsSource = mListPlayDateTypeItems;

                BindCommands();
                //ChangeTheme();
                //ChangeLanguage();

                mRootItem.Children.Clear();
                SetBusy(true, CurrentApp.GetMessageLanguageInfo("001", string.Format("Loading data, please wait...")));
                mWorker = new BackgroundWorker();
                mWorker.DoWork += (s, de) =>
                {
                    //触发Loaded消息
                    CurrentApp.SendLoadedMessage();

                    LoadLayout();
                    InitOperations();
                    LoadUserSettingInfos();
                    InitRecordDataColumns();
                    InitRecordPlayColumns();
                    InitScoreSheetColumns();
                    InitQueryConditions();
                    LoadGroupingMethodParams();
                    InitControlObjects();
                    GetAllObjects();
                    mIsSaveTempTable = SaveManageObjectQueryInfos();
                    LoadSftpServerList();
                    LoadDownloadParamList();
                    LoadScoreSheetParams();
                    LoadRecordEncryptInfos();

                    CurrentApp.WriteLog("PageLoad", string.Format("All data loaded"));
                };
                mWorker.RunWorkerCompleted += (s, re) =>
                {
                    mWorker.Dispose();
                    SetBusy(false, string.Empty);

                    InitPanels();
                    InitLayout();
                    ShowPanel(0);

                    InitBasicOperations();
                    InitPlayDateTypeItems();
                    CreateOptButtons();
                    CreateToolBarButtons();
                    SetPanelVisible();
                    SetService03Helper();
                    CreateRecordDataColumns();
                    CreateRecordPlayColumns();
                    CreateScoreSheetColumns();
                    CreatePageButtons();
                    CreatePlayButtons();
                    CreateCallInfoItems();
                    InitUserSettings();
                    mRootItem.IsChecked = false;
                    if (mRootItem.Children.Count > 0)
                    {
                        mRootItem.Children[0].IsExpanded = true;
                    }

                    //ChangeTheme();
                    ChangeLanguage();

                    CurrentApp.WriteLog(string.Format("Load\t\tPage load end"));

                    ParseStartArgs();
                };
                mWorker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void BindCommands()
        {
            CommandBindings.Add(new CommandBinding(QMMainPageCommands.DeletePlayItemCommand,
                DeletePlayItemCommand_Executed,
                (s, e) => e.CanExecute = true));

            CommandBindings.Add(new CommandBinding(QMMainPageCommands.AddScoreCommand,
              AddScoreCommand_Executed,
              (s, e) => e.CanExecute = true));

            CommandBindings.Add(new CommandBinding(QMMainPageCommands.ModifyScoreCommand,
              ModifyScoreCommand_Executed,
              (s, e) => e.CanExecute = true));

            CommandBindings.Add(new CommandBinding(QMMainPageCommands.GridViewColumnHeaderCommand,
                GridViewColumnHeader_Executed,
                (s, e) => e.CanExecute = true));
        }

        private void InitOperations()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)RequestCode.WSGetUserOptList;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(CurrentApp.Session.UserInfo.UserID.ToString());
                webRequest.ListData.Add("31");
                webRequest.ListData.Add("3102");
                Service11012Client client = new Service11012Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11012"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                ListOperations.Clear();
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<OperationInfo>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    OperationInfo optInfo = optReturn.Data as OperationInfo;
                    if (optInfo != null)
                    {
                        optInfo.Display = CurrentApp.GetLanguageInfo(string.Format("FO{0}", optInfo.ID), optInfo.ID.ToString());
                        optInfo.Description = optInfo.Display;
                        ListOperations.Add(optInfo);
                    }
                }

                CurrentApp.WriteLog("PageLoad", string.Format("Load Operation"));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitRecordDataColumns()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)RequestCode.WSGetUserViewColumnList;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(CurrentApp.Session.UserInfo.UserID.ToString());
                webRequest.ListData.Add("3102001");
                Service11012Client client = new Service11012Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11012"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                List<ViewColumnInfo> listColumns = new List<ViewColumnInfo>();
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<ViewColumnInfo>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    ViewColumnInfo columnInfo = optReturn.Data as ViewColumnInfo;
                    if (columnInfo != null)
                    {
                        columnInfo.Display = columnInfo.ColumnName;
                        listColumns.Add(columnInfo);
                    }
                }
                listColumns = listColumns.OrderBy(c => c.SortID).ToList();
                mListRecordDataColumns.Clear();
                for (int i = 0; i < listColumns.Count; i++)
                {
                    mListRecordDataColumns.Add(listColumns[i]);
                }

                CurrentApp.WriteLog("PageLoad", string.Format("Load RecordColumn"));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitRecordPlayColumns()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)RequestCode.WSGetUserViewColumnList;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(CurrentApp.Session.UserInfo.UserID.ToString());
                webRequest.ListData.Add("3102002");
                Service11012Client client = new Service11012Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11012"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                List<ViewColumnInfo> listColumns = new List<ViewColumnInfo>();
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<ViewColumnInfo>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    ViewColumnInfo columnInfo = optReturn.Data as ViewColumnInfo;
                    if (columnInfo != null)
                    {
                        columnInfo.Display = columnInfo.ColumnName;
                        listColumns.Add(columnInfo);
                    }
                }
                listColumns = listColumns.OrderBy(c => c.SortID).ToList();
                mListRecordPlayColumns.Clear();
                for (int i = 0; i < listColumns.Count; i++)
                {
                    mListRecordPlayColumns.Add(listColumns[i]);
                }

                CurrentApp.WriteLog("PageLoad", string.Format("Load PlayColumn"));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitScoreSheetColumns()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)RequestCode.WSGetUserViewColumnList;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(CurrentApp.Session.UserInfo.UserID.ToString());
                webRequest.ListData.Add("3102003");
                Service11012Client client = new Service11012Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11012"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                List<ViewColumnInfo> listColumns = new List<ViewColumnInfo>();
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<ViewColumnInfo>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    ViewColumnInfo columnInfo = optReturn.Data as ViewColumnInfo;
                    if (columnInfo != null)
                    {
                        columnInfo.Display = columnInfo.ColumnName;
                        listColumns.Add(columnInfo);
                    }
                }
                listColumns = listColumns.OrderBy(c => c.SortID).ToList();
                mListScoreSheetColumns.Clear();
                for (int i = 0; i < listColumns.Count; i++)
                {
                    mListScoreSheetColumns.Add(listColumns[i]);
                }

                CurrentApp.WriteLog("PageLoad", string.Format("Load ScoreSheetColumn"));
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
                mListPanels.Clear();
                OperationInfo optInfo;

                PanelItem panelItem = new PanelItem();
                panelItem.Name = S3102Consts.PANEL_NAME_RECORDLIST;
                panelItem.ContentID = S3102Consts.PANEL_CONTENTID_RECORDLIST;
                panelItem.Title = CurrentApp.GetLanguageInfo("3102P001", "Record List");
                panelItem.Icon = "Images/recordlist.png";
                panelItem.IsVisible = true;
                panelItem.CanClose = false;
                mListPanels.Add(panelItem);

                panelItem = new PanelItem();
                panelItem.Name = S3102Consts.PANEL_NAME_PLAYLIST;
                panelItem.ContentID = S3102Consts.PANEL_CONTENTID_PLAYLIST;
                panelItem.Title = CurrentApp.GetLanguageInfo("3102P002", "Play List");
                panelItem.Icon = "Images/playlist.png";
                panelItem.CanClose = true;
                optInfo = ListOperations.FirstOrDefault(o => o.ID == S3102Consts.OPT_PLAYRECORD);
                if (optInfo != null)
                {
                    panelItem.IsVisible = true;
                    mListPanels.Add(panelItem);
                }

                panelItem = new PanelItem();
                panelItem.Name = S3102Consts.PANEL_NAME_OBJECTLIST;
                panelItem.ContentID = S3102Consts.PANEL_CONTENTID_OBJECTLIST;
                panelItem.Title = CurrentApp.GetLanguageInfo("3102P003", "Manage Object");
                panelItem.Icon = "Images/objectbox.png";
                panelItem.CanClose = true;
                panelItem.IsVisible = true;
                mListPanels.Add(panelItem);

                panelItem = new PanelItem();
                panelItem.Name = S3102Consts.PANEL_NAME_PLAYBOX;
                panelItem.ContentID = S3102Consts.PANEL_CONTENTID_PLAYBOX;
                panelItem.Title = CurrentApp.GetLanguageInfo("3102P004", "Play Box");
                panelItem.Icon = "Images/playbox.png";
                panelItem.CanClose = true;
                optInfo = ListOperations.FirstOrDefault(o => o.ID == S3102Consts.OPT_PLAYRECORD);
                if (optInfo != null)
                {
                    panelItem.IsVisible = true;
                    mListPanels.Add(panelItem);
                }

                panelItem = new PanelItem();
                panelItem.Name = S3102Consts.PANEL_NAME_CALLINO;
                panelItem.ContentID = S3102Consts.PANEL_CONTENTID_CALLINO;
                panelItem.Title = CurrentApp.GetLanguageInfo("3102P005", "Call Information");
                panelItem.Icon = "Images/callinfo.png";
                panelItem.CanClose = true;
                panelItem.IsVisible = true;
                mListPanels.Add(panelItem);

                panelItem = new PanelItem();
                panelItem.Name = S3102Consts.PANEL_NAME_MEMO;
                panelItem.ContentID = S3102Consts.PANEL_CONTENTID_MEMO;
                panelItem.Title = CurrentApp.GetLanguageInfo("3102P006", "Memo");
                panelItem.Icon = "Images/memobox.png";
                panelItem.CanClose = true;
                optInfo = ListOperations.FirstOrDefault(o => o.ID == S3102Consts.OPT_MEMORECORD);
                if (optInfo != null)
                {
                    panelItem.IsVisible = true;
                    mListPanels.Add(panelItem);
                }

                panelItem = new PanelItem();
                panelItem.Name = S3102Consts.PANEL_NAME_BOOKMARK;
                panelItem.ContentID = S3102Consts.PANEL_CONTENTID_BOOKMARK;
                panelItem.Title = CurrentApp.GetLanguageInfo("3102P007", "Bookmark");
                panelItem.Icon = "Images/bookmark.png";
                panelItem.CanClose = true;
                optInfo = ListOperations.FirstOrDefault(o => o.ID == S3102Consts.OPT_BOOKMARKRECORD);
                if (optInfo != null)
                {
                    panelItem.IsVisible = true;
                    mListPanels.Add(panelItem);
                }

                panelItem = new PanelItem();
                panelItem.Name = S3102Consts.PANEL_NAME_SCORE;
                panelItem.ContentID = S3102Consts.PANEL_CONTENTID_SCORE;
                panelItem.Title = CurrentApp.GetLanguageInfo("3102P008", "Score");
                panelItem.Icon = "Images/scorebox.png";
                panelItem.CanClose = true;
                optInfo = ListOperations.FirstOrDefault(o => o.ID == S3102Consts.OPT_SCORERECORD);
                if (optInfo != null)
                {
                    panelItem.IsVisible = true;
                    mListPanels.Add(panelItem);
                }

                panelItem = new PanelItem();
                panelItem.Name = S3102Consts.PANEL_NAME_SCOREDETAIL;
                panelItem.ContentID = S3102Consts.PANEL_CONTENTID_SCOREDETAIL;
                panelItem.Title = CurrentApp.GetLanguageInfo("3102P009", "Score Detail");
                panelItem.Icon = "Images/scoredetail.png";
                panelItem.CanClose = true;
                optInfo = ListOperations.FirstOrDefault(o => o.ID == S3102Consts.OPT_SCOREDETAIL);
                if (optInfo != null)
                {
                    panelItem.IsVisible = true;
                    mListPanels.Add(panelItem);
                }

                //会话信息
                panelItem = new PanelItem();
                panelItem.Name = S3102Consts.PANEL_NAME_CONVERSATIONINFO;
                panelItem.ContentID = S3102Consts.PANEL_CONTENTID_CONVERSATIONINFO;
                panelItem.Title = CurrentApp.GetLanguageInfo("3102P010", "会话信息");
                panelItem.Icon = "Images/speechinfo.png";
                panelItem.CanClose = true;
                optInfo = ListOperations.FirstOrDefault(o => o.ID == S3102Consts.OPT_PLAYRECORD);
                if (optInfo != null)
                {
                    panelItem.IsVisible = true;
                    mListPanels.Add(panelItem);
                }

            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitBasicOperations()
        {
            List<OperationInfo> listOptInfos = new List<OperationInfo>();
            var optInfo = ListOperations.FirstOrDefault(o => o.ID == S3102Consts.OPT_QUERYRECORD);
            if (optInfo != null)
            {
                listOptInfos.Add(optInfo);
            }
            if (mListCurrentRecordInfoItems.Count > 0)
            {
                optInfo = ListOperations.FirstOrDefault(o => o.ID == S3102Consts.OPT_EXPORTDATA);
                if (optInfo != null)
                {
                    listOptInfos.Add(optInfo);
                }
                optInfo = ListOperations.FirstOrDefault(o => o.ID == S3102Consts.OPT_EXPORTRECORD);
                if (optInfo != null)
                {
                    listOptInfos.Add(optInfo);
                }
            }
            if (mCurrentRecordInfoItem != null)
            {
                optInfo = ListOperations.FirstOrDefault(o => o.ID == S3102Consts.OPT_PLAYRECORD);
                if (optInfo != null)
                {
                    listOptInfos.Add(optInfo);
                }
                optInfo = ListOperations.FirstOrDefault(o => o.ID == S3102Consts.OPT_MEMORECORD);
                if (optInfo != null)
                {
                    listOptInfos.Add(optInfo);
                }
                optInfo = ListOperations.FirstOrDefault(o => o.ID == S3102Consts.OPT_BOOKMARKRECORD);
                if (optInfo != null)
                {
                    listOptInfos.Add(optInfo);
                }
                optInfo = ListOperations.FirstOrDefault(o => o.ID == S3102Consts.OPT_SCORERECORD);
                if (optInfo != null)
                {
                    listOptInfos.Add(optInfo);
                }
                optInfo = ListOperations.FirstOrDefault(o => o.ID == S3102Consts.OPT_RECORDSCOREDETAIL);
                if (optInfo != null)
                {
                    listOptInfos.Add(optInfo);
                }
                optInfo = ListOperations.FirstOrDefault(o => o.ID == S3102Consts.OPT_ADDLIBRARY);
                if (optInfo != null)
                {
                    listOptInfos.Add(optInfo);
                }
            }
            listOptInfos = listOptInfos.OrderBy(o => o.SortID).ToList();
            mListBasicOperations.Clear();
            for (int i = 0; i < listOptInfos.Count; i++)
            {
                mListBasicOperations.Add(listOptInfos[i]);
            }
        }

        private void LoadLayout()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)RequestCode.WSGetLayoutInfo;
                webRequest.ListData.Add("310201");
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                webRequest.ListData.Add("0");
                Service11012Client client = new Service11012Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(
                        CurrentApp.Session.AppServerInfo,
                        "Service11012"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    CurrentApp.WriteLog("LoadLayout", string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                string strLayoutInfo = webReturn.Data;
                if (string.IsNullOrEmpty(strLayoutInfo))
                {
                    CurrentApp.WriteLog("LoadLayout", string.Format("Fail.\tLayoutInfo is empty"));
                    return;
                }
                mLayoutInfo = strLayoutInfo;

                CurrentApp.WriteLog("PageLoad", string.Format("Load Layout"));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitLayout()
        {
            try
            {
                if (string.IsNullOrEmpty(mLayoutInfo)) { return; }
                byte[] byteLayoutInfo = Encoding.UTF8.GetBytes(mLayoutInfo);
                MemoryStream ms = new MemoryStream();
                ms.Write(byteLayoutInfo, 0, byteLayoutInfo.Length);
                ms.Position = 0;
                var serializer = new XmlLayoutSerializer(PanelManager);
                using (var stream = new StreamReader(ms))
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
                ShowException(ex.Message);
            }
        }

        private void InitQueryConditions()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S3102Codes.GetUserQueryCondition;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(CurrentApp.Session.UserInfo.UserID.ToString());
                Service31021Client client = new Service31021Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31021"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                Dispatcher.Invoke(new Action(() => mListQueryConditions.Clear()));
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<QueryCondition>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    QueryCondition queryCondition = optReturn.Data as QueryCondition;
                    if (queryCondition != null)
                    {
                        Dispatcher.Invoke(new Action(() => mListQueryConditions.Add(queryCondition)));
                    }
                }
                Dispatcher.Invoke(new Action(() =>
                {
                    if (mListQueryConditions.Count > 0)
                    {
                        ComboQueryConditions.Visibility = Visibility.Visible;
                    }
                }));

                CurrentApp.WriteLog("PageLoad", string.Format("Load QueryCondition"));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitControlObjects()
        {
            InitControlOrgs(mRootItem, -1);

            CurrentApp.WriteLog("PageLoad", string.Format("Load ControlObject"));
        }

        private void InitControlOrgs(ObjectItem parentItem, long parentID)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)RequestCode.WSGetUserCtlObjList;
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                webRequest.ListData.Add("0");
                webRequest.ListData.Add(ConstValue.RESOURCE_ORG.ToString());
                webRequest.ListData.Add(parentID.ToString());
                Service11012Client client = new Service11012Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(
                        CurrentApp.Session.AppServerInfo,
                        "Service11012"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    ShowException(string.Format("Fail.\tListData is null"));
                    return;
                }
                OperationReturn optReturn;
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[i];
                    optReturn = XMLHelper.DeserializeObject<ResourceObject>(strInfo);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    ResourceObject info = optReturn.Data as ResourceObject;
                    if (info == null)
                    {
                        ShowException(string.Format("ResourceObject is null"));
                        return;
                    }
                    long objID = info.ObjID;
                    string strName = info.Name;
                    string strDesc = info.FullName;
                    if (string.IsNullOrEmpty(strDesc))
                    {
                        strDesc = strName;
                    }
                    ObjectItem item = new ObjectItem();
                    item.ObjType = ConstValue.RESOURCE_ORG;
                    item.ObjID = objID;
                    item.Name = strName;
                    item.Description = strDesc;
                    item.Data = info;
                    if (objID == ConstValue.ORG_ROOT)
                    {
                        item.Icon = "Images/rootorg.ico";
                    }
                    else
                    {
                        item.Icon = "Images/org.ico";
                    }
                    InitControlOrgs(item, objID);
                    if (GroupingWay.IndexOf("R") >= 0)
                    {
                        InitControlRealExtensions(item, objID);
                    }
                    if (GroupingWay.IndexOf("E") >= 0)
                    {
                        InitControlExtensions(item, objID);
                    }
                    if (GroupingWay.IndexOf("A") >= 0)
                    {
                        InitControlAgents(item, objID);
                    }
                    AddChildObject(parentItem, item);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitControlAgents(ObjectItem parentItem, long parentID)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)RequestCode.WSGetUserCtlObjList;
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                webRequest.ListData.Add("0");
                webRequest.ListData.Add(ConstValue.RESOURCE_AGENT.ToString());
                webRequest.ListData.Add(parentID.ToString());
                webRequest.ListData.Add("1");
                Service11012Client client = new Service11012Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(
                        CurrentApp.Session.AppServerInfo,
                        "Service11012"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    ShowException(string.Format("Fail.\tListData is null"));
                    return;
                }
                OperationReturn optReturn;
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[i];
                    optReturn = XMLHelper.DeserializeObject<ResourceObject>(strInfo);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    ResourceObject info = optReturn.Data as ResourceObject;
                    if (info == null)
                    {
                        ShowException(string.Format("ResourceObject is null"));
                        return;
                    }
                    long objID = info.ObjID;
                    string strName = info.Name;
                    string strDesc = info.FullName;
                    if (string.IsNullOrEmpty(strDesc))
                    {
                        strDesc = strName;
                    }
                    ObjectItem item = new ObjectItem();
                    item.Parent = parentItem;
                    item.ObjType = ConstValue.RESOURCE_AGENT;
                    item.ObjID = objID;
                    item.Name = strName;
                    item.Description = strDesc;
                    item.Data = info;
                    item.Icon = "Images/agent.ico";
                    AddChildObject(parentItem, item);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitControlExtensions(ObjectItem parentItem, long parentID)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)RequestCode.WSGetUserCtlObjList;
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                webRequest.ListData.Add("0");
                webRequest.ListData.Add(ConstValue.RESOURCE_EXTENSION.ToString());
                webRequest.ListData.Add(parentID.ToString());
                webRequest.ListData.Add("1");
                Service11012Client client = new Service11012Client(
                     WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                     WebHelper.CreateEndpointAddress(
                         CurrentApp.Session.AppServerInfo,
                         "Service11012"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    ShowException(string.Format("Fail.\tListData is null"));
                    return;
                }
                OperationReturn optReturn;
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[i];
                    optReturn = XMLHelper.DeserializeObject<ResourceObject>(strInfo);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    ResourceObject info = optReturn.Data as ResourceObject;
                    if (info == null)
                    {
                        ShowException(string.Format("ResourceObject is null"));
                        return;
                    }
                    long objID = info.ObjID;
                    string strName = info.Name;
                    string strDesc = info.Other04;
                    ObjectItem item = new ObjectItem();
                    item.ObjType = ConstValue.RESOURCE_EXTENSION;
                    item.ObjID = objID;
                    item.Parent = parentItem;
                    item.Name = strName;
                    item.FullName = strDesc;
                    item.Description = strDesc;
                    item.Data = info;
                    item.Icon = "Images/extension.ico";
                    AddChildObject(parentItem, item);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitControlRealExtensions(ObjectItem parentItem, long parentID)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)RequestCode.WSGetUserCtlObjList;
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                webRequest.ListData.Add("0");
                webRequest.ListData.Add(ConstValue.RESOURCE_REALEXT.ToString());
                webRequest.ListData.Add(parentID.ToString());
                webRequest.ListData.Add("1");
                Service11012Client client = new Service11012Client(
                   WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                   WebHelper.CreateEndpointAddress(
                       CurrentApp.Session.AppServerInfo,
                       "Service11012"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    ShowException(string.Format("Fail.\tListData is null"));
                    return;
                }
                OperationReturn optReturn;
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[i];
                    optReturn = XMLHelper.DeserializeObject<ResourceObject>(strInfo);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    ResourceObject info = optReturn.Data as ResourceObject;
                    if (info == null)
                    {
                        ShowException(string.Format("ResourceObject is null"));
                        return;
                    }
                    long objID = info.ObjID;
                    string strName = info.Name;
                    string strDesc = info.FullName;
                    if (string.IsNullOrEmpty(strDesc))
                    {
                        strDesc = strName;
                    }
                    ObjectItem item = new ObjectItem();
                    item.ObjType = ConstValue.RESOURCE_REALEXT;
                    item.ObjID = objID;
                    item.Parent = parentItem;
                    item.Name = strName;
                    item.FullName = strDesc;
                    item.Description = strDesc;
                    item.Data = info;
                    item.Icon = "Images/RealExtension.ico";
                    AddChildObject(parentItem, item);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadUserSettingInfos()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3102Codes.GetUserSettingList;
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                string strGroupInfo = string.Format("310201{0}310202{0}310203{0}310204", ConstValue.SPLITER_CHAR);
                webRequest.ListData.Add(strGroupInfo);
                webRequest.ListData.Add(string.Empty);
                Service31021Client client = new Service31021Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(
                        CurrentApp.Session.AppServerInfo,
                        "Service31021"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    ShowException(string.Format("Fail.\tListData is null"));
                    return;
                }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[i];
                    OperationReturn optReturn = XMLHelper.DeserializeObject<SettingInfo>(strInfo);
                    if (!optReturn.Result)
                    {
                        CurrentApp.WriteLog("LoadSetting", string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        continue;
                    }
                    SettingInfo settingInfo = optReturn.Data as SettingInfo;
                    if (settingInfo == null)
                    {
                        CurrentApp.WriteLog("LoadSetting", string.Format("Fail.\tSettingInfo is null"));
                        continue;
                    }
                    var temp =
                        mListSettingInfos.FirstOrDefault(
                            s => s.ParamID == settingInfo.ParamID && s.UserID == settingInfo.UserID);
                    if (temp == null)
                    {
                        mListSettingInfos.Add(settingInfo);
                    }
                    else
                    {
                        mListSettingInfos.Remove(temp);
                        mListSettingInfos.Add(settingInfo);
                    }
                }

                CurrentApp.WriteLog("PageLoad", string.Format("Load UserSetting"));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void InitPlayDateTypeItems()
        {
            try
            {
                mListPlayDateTypeItems.Clear();
                PlayDateTypeItem item = new PlayDateTypeItem();
                item.Type = 0;
                item.Display = CurrentApp.GetLanguageInfo(string.Format("3102TIP006{0}", item.Type), item.Type.ToString());
                mListPlayDateTypeItems.Add(item);
                item = new PlayDateTypeItem();
                item.Type = 1;
                item.Display = CurrentApp.GetLanguageInfo(string.Format("3102TIP006{0}", item.Type), item.Type.ToString());
                mListPlayDateTypeItems.Add(item);
                item = new PlayDateTypeItem();
                item.Type = 2;
                item.Display = CurrentApp.GetLanguageInfo(string.Format("3102TIP006{0}", item.Type), item.Type.ToString());
                mListPlayDateTypeItems.Add(item);
                item = new PlayDateTypeItem();
                item.Type = 3;
                item.Display = CurrentApp.GetLanguageInfo(string.Format("3102TIP006{0}", item.Type), item.Type.ToString());
                mListPlayDateTypeItems.Add(item);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadSftpServerList()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3102Codes.GetSftpServerList;
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                Service31021Client client = new Service31021Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(
                        CurrentApp.Session.AppServerInfo,
                        "Service31021"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    ShowException(string.Format("Fail.\tListData is null"));
                    return;
                }
                mListSftpServers.Clear();
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[i];
                    OperationReturn optReturn = XMLHelper.DeserializeObject<SftpServerInfo>(strInfo);
                    if (!optReturn.Result)
                    {
                        CurrentApp.WriteLog("LoadSftp", string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        continue;
                    }
                    SftpServerInfo sftpInfo = optReturn.Data as SftpServerInfo;
                    if (sftpInfo == null)
                    {
                        CurrentApp.WriteLog("LoadSftp", string.Format("Fail.\tSftpServerInfo is null"));
                        continue;
                    }
                    mListSftpServers.Add(sftpInfo);
                }

                CurrentApp.WriteLog("PageLoad", string.Format("Load SftpServerInfo"));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadDownloadParamList()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3102Codes.GetDownloadParamList;
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                Service31021Client client = new Service31021Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(
                        CurrentApp.Session.AppServerInfo,
                        "Service31021"));
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    ShowException(string.Format("Fail.\tListData is null"));
                    return;
                }
                mListDownloadParams.Clear();
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[i];
                    OperationReturn optReturn = XMLHelper.DeserializeObject<DownloadParamInfo>(strInfo);
                    if (!optReturn.Result)
                    {
                        CurrentApp.WriteLog("LoadSftp", string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        continue;
                    }
                    DownloadParamInfo info = optReturn.Data as DownloadParamInfo;
                    if (info == null)
                    {
                        CurrentApp.WriteLog("LoadSftp", string.Format("Fail.\tSftpServerInfo is null"));
                        continue;
                    }
                    mListDownloadParams.Add(info);
                }

                CurrentApp.WriteLog("PageLoad", string.Format("Load DownloadParams"));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        //读取全局参数GlobalParamInfo 这个参数和查询评分有关的~  当获得的参数为
        private void LoadScoreSheetParams()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)RequestCode.WSGetGlobalParamList;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add("31");
                webRequest.ListData.Add("0");
                webRequest.ListData.Add("310101");
                Service11012Client client = new Service11012Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11012"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                List<GlobalParamInfo> listGlobalParam = new List<GlobalParamInfo>();
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<GlobalParamInfo>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    GlobalParamInfo GlobalParamInfo = optReturn.Data as GlobalParamInfo;
                    if (GlobalParamInfo == null) { return; }
                    string tempType = GlobalParamInfo.ParamValue.Substring(GlobalParamInfo.ParamValue.Length - 2, 1);
                    string tempIsScore = GlobalParamInfo.ParamValue.Substring(GlobalParamInfo.ParamValue.Length - 1, 1);
                    ScoreParamsInfo tempThisClass = new ScoreParamsInfo();
                    tempThisClass.Type = tempType;
                    tempThisClass.Value = tempIsScore;
                    mScoreParams.Add(tempThisClass);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadRecordEncryptInfos()
        {
            try
            {
                mListRecordEncryptInfos.Clear();
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)RequestCode.WSGetUserParamList;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(CurrentApp.Session.UserInfo.UserID.ToString());
                webRequest.ListData.Add("1");
                webRequest.ListData.Add(S3102Consts.USER_PARAM_GROUP_ENCRYPTINFO.ToString());
                CurrentApp.MonitorHelper.AddWebRequest(webRequest);
                Service11012Client client = new Service11012Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11012"));
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.DoOperation(webRequest);
                CurrentApp.MonitorHelper.AddWebReturn(webReturn);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    ShowException(string.Format("WSFail.\tListData is null"));
                    return;
                }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<UserParamInfo>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
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
                            CurrentApp.WriteLog("LoadEncryptInfo", string.Format("Fail.\tEncryptInfo invalid."));
                            continue;
                        }
                        if (paramID > S3102Consts.USER_PARAM_GROUP_ENCRYPTINFO * 1000
                            && paramID < (S3102Consts.USER_PARAM_GROUP_ENCRYPTINFO + 1) * 1000
                            && dtExpireTime > DateTime.Now.ToUniversalTime())
                        {
                            RecordEncryptInfo encryptInfo = new RecordEncryptInfo();
                            encryptInfo.UserID = CurrentApp.Session.UserID;
                            encryptInfo.ServerAddress = strAddress;
                            encryptInfo.Password = strPassword;
                            encryptInfo.EndTime = dtExpireTime;
                            mListRecordEncryptInfos.Add(encryptInfo);
                        }
                    }
                }

                CurrentApp.WriteLog("PageLoad", string.Format("Init RecordEncryptInfo."));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadGroupingMethodParams()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)RequestCode.WSGetGlobalParamList;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add("11");
                webRequest.ListData.Add("12010401");
                webRequest.ListData.Add("120104");
                Service11012Client client = new Service11012Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11012"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                //List<GlobalParamInfo> listGlobalParam = new List<GlobalParamInfo>();

                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string str = webReturn.ListData[i];
                    str = str.Replace("&#x1B;", "");
                    OperationReturn optReturn = XMLHelper.DeserializeObject<GlobalParamInfo>(str);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    GlobalParamInfo GlobalParamInfo = optReturn.Data as GlobalParamInfo;
                    if (GlobalParamInfo == null) { return; }
                    string tempGroupWay = GlobalParamInfo.ParamValue.Substring(8);
                    //string tempIsScore = GlobalParamInfo.ParamValue.Substring(GlobalParamInfo.ParamValue.Length - 1, 1);
                    //ScoreParamsInfo tempThisClass = new ScoreParamsInfo();
                    //tempThisClass.Value = tempGroupWay;
                    //tempThisClass.Value = tempIsScore;
                    //mScoreParams.Add(tempThisClass);
                    GroupingWay = tempGroupWay;
                }
                //MessageBox.Show(GroupingWay);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        //将管理对象放入临时表 生成临时表ID
        private bool SaveManageObjectQueryInfos()
        {
            //将当前用户管理的对象保存到临时表中
            try
            {
                mManageObjectQueryID = string.Empty;

                //系统管理员身份的用户无需考虑管理权限，他能管理所有分机、坐席
                if (CurrentApp.Session.RoleID == ConstValue.ROLE_SYSTEMADMIN) { return false; }

                if (mListAllObjects == null) { return true; }

                string strType, strID, strName, strInfo;
                List<string> listObjectInfos = new List<string>();
                for (int i = 0; i < mListAllObjects.Count; i++)
                {
                    ObjectItem item = mListAllObjects[i];
                    //分机和坐席都控制
                    if (item.ObjType == ConstValue.RESOURCE_AGENT
                        || item.ObjType == ConstValue.RESOURCE_EXTENSION
                        || item.ObjType == ConstValue.RESOURCE_REALEXT)
                    {
                        strType = item.ObjType.ToString();
                        strID = item.ObjID.ToString();
                        strName = item.Name;
                        strInfo = string.Format("{0}{1}{2}{1}{3}",
                            strType,
                            ConstValue.SPLITER_CHAR,
                            strID,
                            strName);
                        listObjectInfos.Add(strInfo);
                    }
                }
                string test = string.Empty;
                //分组方式为纯坐席的情况下普通用户不查N/A，只有超级管理员才能查N/A，所以这里不将其放入管理对象里面  小金说的
                if (GroupingWay == "A")
                {

                }
                else
                {
                    if (GroupingWay.IndexOf("A") >= 0)
                    {
                        test = string.Format("{0}{1}{2}{1}{3}",
                           ConstValue.RESOURCE_AGENT.ToString(),
                           ConstValue.SPLITER_CHAR,
                            0,
                           "N/A");
                        listObjectInfos.Add(test);
                    }
                    if (GroupingWay.IndexOf("E") >= 0)
                    {
                        test = string.Format("{0}{1}{2}{1}{3}",
                           ConstValue.RESOURCE_EXTENSION.ToString(),
                           ConstValue.SPLITER_CHAR,
                           0,
                           "N/A");
                        listObjectInfos.Add(test);
                    }
                    if (GroupingWay.IndexOf("R") >= 0)
                    {
                        test = string.Format("{0}{1}{2}{1}{3}",
                           ConstValue.RESOURCE_REALEXT.ToString(),
                           ConstValue.SPLITER_CHAR,
                            0,
                           "N/A");
                        listObjectInfos.Add(test);
                    }
                }

                //if (GroupingWay != "E")
                //{
                //    if (GroupingWay == "A" && CurrentApp.Session.RoleID == ConstValue.ROLE_SYSTEMADMIN)
                //    {
                //        test = string.Format("{0}{1}{2}{1}{3}",
                //            ConstValue.RESOURCE_AGENT.ToString(),
                //            ConstValue.SPLITER_CHAR,
                //            "NULL",
                //            "N/A");
                //    }
                //    if (GroupingWay == "EA")
                //    {
                //        test = string.Format("{0}{1}{2}{1}{3}",
                //                  ConstValue.RESOURCE_AGENT.ToString(),
                //                  ConstValue.SPLITER_CHAR,
                //                  "NULL",
                //                  "N/A");
                //        listObjectInfos.Add(test);
                //    }
                //}
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3102Codes.InsertManageObjectQueryInfos;
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                webRequest.ListData.Add(listObjectInfos.Count.ToString());
                for (int i = 0; i < listObjectInfos.Count; i++)
                {
                    webRequest.ListData.Add(listObjectInfos[i]);
                }
                Service31021Client client = new Service31021Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(
                        CurrentApp.Session.AppServerInfo,
                        "Service31021"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return false;
                }
                mManageObjectQueryID = webReturn.Data;
                return true;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
                return false;
            }
        }

        #endregion

        #region EventHandlers

        void BasicOpt_Click(object sender, RoutedEventArgs e)
        {
            var btn = e.Source as Button;
            if (btn != null)
            {
                var optItem = btn.DataContext as OperationInfo;
                if (optItem == null) { return; }
                switch (optItem.ID)
                {
                    case S3102Consts.OPT_QUERYRECORD:
                        OpenQueryCondition();
                        break;
                    case S3102Consts.OPT_PLAYRECORD:
                        PlayRecord();
                        break;
                    case S3102Consts.OPT_MEMORECORD:
                        MemoRecord();
                        break;
                    case S3102Consts.OPT_RECOMMANDRECORD:
                        //PlayRecord();
                        break;
                    case S3102Consts.OPT_SCORERECORD:
                        ScoreRecord();
                        break;
                    case S3102Consts.OPT_EXPORTDATA:
                        OpenExportDataOption();
                        break;
                    case S3102Consts.OPT_EXPORTRECORD:
                        OpenExportRecordOption();
                        break;
                    case S3102Consts.OPT_BOOKMARKRECORD:
                        BookmarkRecord();
                        break;
                    case S3102Consts.OPT_RECORDSCOREDETAIL:
                        OpenRecordScoreDetail();
                        break;
                    case S3102Consts.OPT_ADDLIBRARY:
                        OpenLibraryTree();
                        break;
                }
            }
        }

        void PanelToggleButton_Click(object sender, RoutedEventArgs e)//右上角的按钮事件
        {
            var toggleBtn = e.Source as ToggleButton;
            if (toggleBtn != null)
            {
                ToolButtonItem item = toggleBtn.DataContext as ToolButtonItem;
                if (item != null)
                {
                    PanelItem panelItem = mListPanels.FirstOrDefault(p => string.Format("TB{0}", p.Name) == item.Name);
                    if (panelItem == null) { return; }
                    //==================播放器，标签，评分还有备注这四个地方的录音不能有两两同时播放 注意  备注===========
                    if (panelItem.Name == S3102Consts.PANEL_NAME_PLAYBOX)
                    {
                        ShowPanel(1);
                    }
                    if (panelItem.Name == S3102Consts.PANEL_NAME_BOOKMARK)
                    {
                        ShowPanel(4);
                    }
                    if (panelItem.Name == S3102Consts.PANEL_CONTENTID_SCOREDETAIL)
                    {
                        ShowPanel(3);
                    }
                    //=========================================by 汤澈=====================================================
                    if (panelItem != null)
                    {
                        panelItem.IsVisible = toggleBtn.IsChecked == true;
                    }
                }
                SetPanelVisible();
            }

        }

        void ToolButton_Click(object sender, RoutedEventArgs e)
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
                        case "BTRestoreLayout":
                            RestoreLayout();
                            break;
                        case "BTResetLayout":
                            ResetLayout();
                            break;
                        case "BTSetting":
                            OpenSetting();
                            break;
                        case "BTSetStart":
                            SetPlayItemStartPosition();
                            break;
                        case "BTSetStop":
                            SetPlayItemStopPosition();
                            break;
                        case "BTCircleMode":
                            SetCircleMode();
                            break;
                        case "BTSavePlay":
                            SavePlayInfo();
                            break;
                        case "BTClear":
                            ClearPlayList();
                            break;
                    }
                }
            }
        }

        void ClearPlayList()
        {
            mListRecordPlayItems.Clear();
        }

        void PageButton_Click(object sender, RoutedEventArgs e)//选择看第几页的按钮
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
                    //case "TB"+"QueryStop":
                    //    mIsQueryContinue = false;
                    //    btn.Visibility = Visibility.Collapsed;
                    //    break;
                }
            }
        }

        void ComboQueryConditions_SelectionChanged(object sender, SelectionChangedEventArgs e)//这个是快速查询框的事件
        {
            if (mIsSkipQuikQuery) { return; }
            var queryCondition = ComboQueryConditions.SelectedItem as QueryCondition;
            if (queryCondition != null)
            {
                try
                {
                    PopupPanel.Title = string.Format("Query Condition");
                    UCQueryCondition ucQueryCondition = new UCQueryCondition();
                    ucQueryCondition.CurrentApp = CurrentApp;
                    ucQueryCondition.PageParent = this;
                    ucQueryCondition.IsAutoQuery = true;
                    bool isSkip = true;
                    var settingInfo =
                        mListSettingInfos.FirstOrDefault(s => s.ParamID == S3102Consts.USER_PARAM_SKIPCONDITIONPANEL);
                    if (settingInfo != null && settingInfo.StringValue == "0")
                    {
                        isSkip = false;
                    }
                    ucQueryCondition.IsSkip = isSkip;
                    ucQueryCondition.QueryCondition = queryCondition;
                    PopupPanel.Content = ucQueryCondition;
                    PopupPanel.IsOpen = true;
                }
                catch (Exception ex)
                {
                    ShowException(ex.Message);
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
                    if (!int.TryParse(TxtPage.Text, out pageIndex) || mPageCount == 0)
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
                ShowException(ex.Message);
            }
        }

        //当每选择中了一个记录的时候,那么就会出现以下操作
        //1.会在左边旁边那里多了许多按钮,比如,录音标记,录音评分,录音备注等
        //
        void LvRecordData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RecordInfoItem item = LvRecordData.SelectedItem as RecordInfoItem;
            if (item != null)
            {
                mCurrentRecordInfoItem = item;

                InitBasicOperations();
                CreateOptButtons();
                CreateCallInfoItems();

                Iscomplained();
                IsTasked();
            }
        }

        void LvRecordData_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var item = LvRecordData.SelectedItem as RecordInfoItem;
            if (item != null)
            {
                mCurrentRecordInfoItem = item;
                Iscomplained();
                IsTasked();
                PlayRecord();
            }
        }

        void LvRecordPlay_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var item = LvRecordPlay.SelectedItem as RecordPlayItem;
                if (item != null)
                {
                    mCurrentRecordPlayItem = item;
                    var recordInfoItem = item.RecordInfoItem;
                    if (recordInfoItem == null)
                    {
                        //如果还没有查询记录信息，先查询记录信息
                        QueryRecordFromPlayList(item);
                    }
                    if (recordInfoItem == null) { return; }
                    mCurrentRecordInfoItem = recordInfoItem;

                    ucConversationInfo = new UCConversationInfo();
                    ucConversationInfo.CurrentApp = CurrentApp;
                    ucConversationInfo.PageParent = this;
                    ucConversationInfo.PlayingRecord = mCurrentRecordInfoItem;
                    ConversationInfo.Child = ucConversationInfo;
                    //ucConversationInfo.aaa();

                    mUCPlayBox = new UCPlayBox();
                    mUCPlayBox.CurrentApp = CurrentApp;
                    mUCPlayBox.PlayStopped += mUCPlayBox_PlayStopped;
                    mUCPlayBox.MainPage = this;
                    mUCPlayBox.ListEncryptInfo = mListRecordEncryptInfos;
                    mUCPlayBox.ListUserSettingInfos = mListSettingInfos;
                    mUCPlayBox.ListSftpServers = mListSftpServers;
                    mUCPlayBox.ListDownloadParams = mListDownloadParams;
                    mUCPlayBox.Service03Helper = mService03Helper;
                    mUCPlayBox.RecordPlayItem = item;
                    mUCPlayBox.IsAutoPlay = true;
                    mUCPlayBox.CircleMode = mCircleMode;
                    mUCPlayBox.StartPosition = mCurrentRecordPlayItem.StartPosition;
                    mUCPlayBox.StopPostion = mCurrentRecordPlayItem.StopPosition;
                    mUCPlayBox.PlayerEvent += mUCPlayBox_PlayerEvent;
                    BorderPlayBox.Child = mUCPlayBox;
                    CreatePlayButtons();
                    ShowPanel(S3102Consts.PANEL_NAME_PLAYBOX, true);
                    ShowPanel(S3102Consts.PANEL_NAME_CONVERSATIONINFO, true);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        void mUCPlayBox_PlayerEvent(object sender, RoutedPropertyChangedEventArgs<UMPEventArgs> e)
        {
            try
            {
                if (e.NewValue == null) { return; }
                int code = e.NewValue.Code;
                var param = e.NewValue.Data;
                switch (code)
                {
                    case MediaPlayerEventCodes.PLAYING:
                        double pos = TimeSpan.Parse(param.ToString()).TotalMilliseconds;
                        //ListBox lb = new ListBox();
                        //关于这里先存这里  这个是保存的位置
                        if (ucConversationInfo == null)
                        {
                            return;
                        }
                        ucConversationInfo.SkipToCurrent(pos);
                        //lb.ScrollIntoView();
                        break;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }


        void LvScoreSheet_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var item = LvScoreSheet.SelectedItem as BasicScoreSheetItem;

            //如果当前IsVisible表示是否能够打开评分框  true为能打开  false 为不能打开
            if (item != null && item.IsVisible == false)
            {
                return;
            }
            if (item != null)
            {
                ucScoreDetail = new UCScoreDetail();
                ucScoreDetail.CurrentApp = CurrentApp;
                ucScoreDetail.PageParent = this;
                ucScoreDetail.ListSftpServers = mListSftpServers;
                ucScoreDetail.ListDownloadParams = mListDownloadParams;
                ucScoreDetail.Service03Helper = mService03Helper;
                ucScoreDetail.ScoreSheetItem = item;
                ucScoreDetail.IsAddScore = item.Flag == 0;
                ucScoreDetail.RecordInfoItem = mCurrentRecordInfoItem;
                GetAllObjects();
                ucScoreDetail.ListAllObjects = mListAllObjects;
                BorderScoreDetail.Child = ucScoreDetail;
                ShowPanel(S3102Consts.PANEL_NAME_SCOREDETAIL, true);
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
                        int index = mListRecordPlayItems.IndexOf(playItem);
                        if (index < 0) { return; }
                        if (index == mListRecordPlayItems.Count - 1)
                        {
                            playItem = mListRecordPlayItems[0];
                        }
                        else
                        {
                            playItem = mListRecordPlayItems[index + 1];
                        }
                        LvRecordPlay.SelectedItem = playItem;
                        mUCPlayBox = new UCPlayBox();
                        mUCPlayBox.CurrentApp = CurrentApp;
                        //订阅事件  这里在mUCPlayBox_PlayStopped方法里  就是这个事件的内容了
                        mUCPlayBox.PlayStopped += mUCPlayBox_PlayStopped;
                        mUCPlayBox.MainPage = this;
                        mUCPlayBox.ListEncryptInfo = mListRecordEncryptInfos;
                        mUCPlayBox.ListUserSettingInfos = mListSettingInfos;
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
                ShowException(ex.Message);
            }
        }

        void ComboPlayList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var playDateTypeItem = ComboPlayList.SelectedItem as PlayDateTypeItem;
            if (playDateTypeItem != null)
            {
                DateTime dtBegin, dtEnd;
                switch (playDateTypeItem.Type)
                {
                    case 0:
                        dtBegin = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd") + " 00:00:00");
                        dtEnd = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd") + " 23:59:59");
                        break;
                    case 1:
                        dtBegin = DateTime.Parse(DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd") + " 00:00:00");
                        dtEnd = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd") + " 23:59:59");
                        break;
                    //=====================================这是我加的最近一周和最近一月,还未获取时间参数只是粗略的算了下而已==
                    case 2:
                        dtBegin = DateTime.Parse(DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd") + " 00:00:00");
                        dtEnd = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd") + " 23:59:59");
                        break;
                    case 3:
                        dtBegin = DateTime.Parse(DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd") + " 00:00:00");
                        dtEnd = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd") + " 23:59:59");
                        break;
                    //=========================================================================================by tangche=====
                    default:
                        ShowException(string.Format("Fail.\tDateType invalid"));
                        return;
                }
                List<RecordPlayInfo> listPlayInfos = new List<RecordPlayInfo>();
                SetBusy(true, string.Empty);
                mWorker = new BackgroundWorker();
                mWorker.DoWork += (s, de) => LoadPlayInfos(dtBegin, dtEnd, ref listPlayInfos);
                mWorker.RunWorkerCompleted += (s, re) =>
                {
                    mWorker.Dispose();
                    SetBusy(false, string.Empty);

                    mListRecordPlayItems.Clear();
                    for (int i = 0; i < listPlayInfos.Count; i++)
                    {
                        RecordPlayInfo info = listPlayInfos[i];
                        RecordPlayItem item = new RecordPlayItem(info);
                        var recordItem = mListAllRecordInfoItems.FirstOrDefault(r => r.SerialID == info.RecordID);
                        if (recordItem != null)
                        {
                            item.RecordInfoItem = recordItem;
                        }
                        item.RowNumber = mListRecordPlayItems.Count + 1;
                        item.Background = GetPlayBackground(item);
                        mListRecordPlayItems.Add(item);
                    }
                };
                mWorker.RunWorkerAsync();
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

        void Service03Helper_Debug(LogMode mode, string category, string msg)
        {
            CurrentApp.WriteLog(category, msg);
        }

        #endregion

        #region CommandHandlers

        private void DeletePlayItemCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var playItem = e.Parameter as RecordPlayItem;
            if (playItem != null)
            {
                var item = mListRecordPlayItems.FirstOrDefault(i => i.SerialID == playItem.SerialID);
                if (item != null)
                {
                    mListRecordPlayItems.Remove(item);
                }
            }
        }

        private void AddScoreCommand_Executed(object sender, ExecutedRoutedEventArgs e)//给录音评分
        {
            var item = e.Parameter as BasicScoreSheetItem;
            if (item != null)
            {
                ucScoreDetail = new UCScoreDetail();
                ucScoreDetail.CurrentApp = CurrentApp;
                ucScoreDetail.PageParent = this;
                ucScoreDetail.ListUserSettingInfos = mListSettingInfos;
                ucScoreDetail.ListSftpServers = mListSftpServers;
                ucScoreDetail.ListDownloadParams = mListDownloadParams;
                ucScoreDetail.ListEncryptInfo = mListRecordEncryptInfos;
                ucScoreDetail.Service03Helper = mService03Helper;
                ucScoreDetail.ScoreSheetItem = item;
                ucScoreDetail.IsAddScore = true;
                ucScoreDetail.RecordInfoItem = mCurrentRecordInfoItem;
                GetAllObjects();
                ucScoreDetail.ListAllObjects = mListAllObjects;
                BorderScoreDetail.Child = ucScoreDetail;
                ShowPanel(S3102Consts.PANEL_NAME_SCOREDETAIL, true);
            }
        }

        private void ModifyScoreCommand_Executed(object sender, ExecutedRoutedEventArgs e)//修改录音评分
        {
            var item = e.Parameter as BasicScoreSheetItem;
            if (item != null)
            {
                ucScoreDetail = new UCScoreDetail();
                ucScoreDetail.CurrentApp = CurrentApp;
                ucScoreDetail.PageParent = this;
                //==================加了这段,解决了"不能修改评过分的录音记录的分数bug"===============
                ucScoreDetail.ListUserSettingInfos = mListSettingInfos;
                ucScoreDetail.ListSftpServers = mListSftpServers;
                ucScoreDetail.ListDownloadParams = mListDownloadParams;
                ucScoreDetail.ListEncryptInfo = mListRecordEncryptInfos;
                ucScoreDetail.Service03Helper = mService03Helper;
                //================================by 汤澈============================================
                ucScoreDetail.ScoreSheetItem = item;
                ucScoreDetail.IsAddScore = false;
                ucScoreDetail.RecordInfoItem = mCurrentRecordInfoItem;
                GetAllObjects();
                ucScoreDetail.ListAllObjects = mListAllObjects;
                BorderScoreDetail.Child = ucScoreDetail;
                ShowPanel(S3102Consts.PANEL_NAME_SCOREDETAIL, true);
            }
        }

        private void GridViewColumnHeader_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                var columnInfo = e.Parameter as ViewColumnInfo;
                if (columnInfo == null) { return; }
                string strColumn = columnInfo.ColumnName;
                if (strColumn == "RowNumber"
                    || strColumn == "SerialID"
                    || strColumn == "StartRecordTime"
                    || strColumn == "Agent"
                    || strColumn == "Extension"
                    || strColumn == "Duration"
                    || strColumn == "VoiceID"
                    || strColumn == "ChannelID"
                    || strColumn == "VoiceIP"
                    || strColumn == "Direction"
                    || strColumn == "CallerID"
                    || strColumn == "CalledID"
                    || strColumn == "MediaType")
                {
                    ListSorter.SortList<ObservableCollection<RecordInfoItem>, RecordInfoItem>(ref mListAllRecordInfoItems, strColumn,
                       mIsSortAsending ? SortDirection.Ascending : SortDirection.Descending);
                    mIsSortAsending = !mIsSortAsending;
                    FillListView();
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion

        #region Operations

        private void OpenSetting()
        {
            try
            {
                PopupPanel.Title = "Query Management Setting";
                UCCustomSetting ucCustomSetting = new UCCustomSetting();
                ucCustomSetting.PageParent = this;
                ucCustomSetting.CurrentApp = CurrentApp;
                PopupPanel.Content = ucCustomSetting;
                PopupPanel.IsOpen = true;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void OpenQueryCondition()
        {
            try
            {
                GetAllObjects();
                GetCheckedObjects();
                PopupPanel.Title = "Query Condition";
                UCQueryCondition ucQueryCondition = new UCQueryCondition();
                ucQueryCondition.CurrentApp = CurrentApp;
                ucQueryCondition.PageParent = this;
                ucQueryCondition.ListSelectedObjects = mListSelectedObjects;
                ucQueryCondition.ListAllObjects = mListAllObjects;
                ucQueryCondition.ManageObjectQueryID = mManageObjectQueryID;
                ucQueryCondition.IsSaveTempTable = mIsSaveTempTable;
                PopupPanel.Content = ucQueryCondition;
                PopupPanel.IsOpen = true;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadPlayInfos(DateTime beginTime, DateTime endTime, ref List<RecordPlayInfo> listPlayInfos)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3102Codes.GetPlayInfoList;
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                webRequest.ListData.Add(beginTime.ToString("yyyy-MM-dd HH:mm:ss"));
                webRequest.ListData.Add(endTime.ToString("yyyy-MM-dd HH:mm:ss"));
                webRequest.ListData.Add(string.Empty);
                Service31021Client client = new Service31021Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(
                        CurrentApp.Session.AppServerInfo,
                        "Service31021"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    ShowException(string.Format("ListData is null"));
                    return;
                }
                OperationReturn optReturn;
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    optReturn = XMLHelper.DeserializeObject<RecordPlayInfo>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    RecordPlayInfo info = optReturn.Data as RecordPlayInfo;
                    if (info == null)
                    {
                        ShowException(string.Format("Fail.\tRecordPlayInfo is null"));
                        return;
                    }
                    listPlayInfos.Add(info);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #region QueryRecord  查询操作

        private void SetQueryBtnEnable(bool isEnable)//查询的时候  对查询按钮的控制  当为false的时候  按钮就不能点了
        {
            try
            {
                BorderBasicOpt.IsEnabled = isEnable;
                //for (int i = 0; i <= PanelBasicOpts.Children.Count;i++ )
                //{

                //        Button btn = PanelBasicOpts.Children[i] as Button;
                //        if (isEnable)
                //            btn.SetResourceReference(StyleProperty, "OptButtonStyle");//启用样式
                //        else
                //            btn.SetResourceReference(StyleProperty, "OptUnEnableButtonStyle");//禁用样式  
                //        btn.IsEnabled = isEnable;

                //}
                //Button btn = PanelBasicOpts.Children[0] as Button; 
                //if (isEnable)
                //    btn.SetResourceReference(StyleProperty, "OptButtonStyle");//启用样式
                //else
                //    btn.SetResourceReference(StyleProperty, "OptUnEnableButtonStyle");//禁用样式  
                //btn.IsEnabled = isEnable;
            }
            catch { }
        }

        public void QueryRecord(string strConditionString, string strConditionLog, List<QueryConditionDetail> listDetails, QueryCondition queryCondition)//
        {
            try
            {
                //点击查询后恢复到查询布局  也就是关闭掉其他的窗口===============
                ShowPanel(0);
                //=====by 汤澈===================================================
                SetQueryBtnEnable(false);
                QueryStop.Visibility = Visibility.Visible;
                //QueryStop.IsEnabled = true;
                mIsSkipQuikQuery = true;//

                if (queryCondition == null)
                {
                    ComboQueryConditions.SelectedItem = null;//当设置了selectedItem的时候  会出发   combox的ComboPlayList_SelectionChanged事件，而那里面有一个  UCQueryCondition ucQueryCondition = new UCQueryCondition();  这样会重新  加载下查询窗口,然后重新做一次快速查询,这样就会出现两个弹框
                }
                else
                {
                    QueryCondition temp = mListQueryConditions.FirstOrDefault(q => q.ID == queryCondition.ID);
                    if (temp == null)
                    {
                        mListQueryConditions.Add(queryCondition);
                        ComboQueryConditions.SelectedItem = queryCondition; //这里也是同上面一大段注释是一样的.
                    }
                    else
                    {
                        ComboQueryConditions.SelectedItem = temp;
                    }
                }
                mIsSkipQuikQuery = false;

                mRecordTotal = 0;
                mPageIndex = 0;
                mPageCount = 0;
                var settingInfo = mListSettingInfos.FirstOrDefault(s => s.ParamID == S3102Consts.USER_PARAM_PAGESIZE);
                if (settingInfo != null)
                {
                    mPageSize = Convert.ToInt32(settingInfo.StringValue);
                }
                settingInfo = mListSettingInfos.FirstOrDefault(s => s.ParamID == S3102Consts.USER_PARAM_MAXRECORDS);
                if (settingInfo != null)
                {
                    mMaxRecords = Convert.ToInt32(settingInfo.StringValue);
                }
                mIsQueryContinue = true;
                mListAllRecordInfoItems.Clear();
                mListCurrentRecordInfoItems.Clear();
                //MyWaiter.Visibility = Visibility.Visible;
                //ButtonCancel.Visibility = Visibility.Visible;                
                //SetStatuMessage(CurrentApp.GetMessageLanguageInfo("002", "Reading record..."));
                SetBusy(true, CurrentApp.GetMessageLanguageInfo("002", "Reading record..."));
                mWorker = new BackgroundWorker();
                mWorker.DoWork += (s, de) =>
                {
                    QueryRecord(strConditionString, listDetails);

                    SaveQueryResult(queryCondition);//保存查询结果信息
                    //QueryStop.Visibility = Visibility.Visible;这里是对界面进行处理的 不能在其他线程里做 只能在主线程里做 所以放这里是错误的
                };
                mWorker.RunWorkerCompleted += (s, re) =>
                {
                    mWorker.Dispose();
                    //MyWaiter.Visibility = Visibility.Hidden;
                    //ButtonCancel.Visibility = Visibility.Hidden;
                    //SetStatuMessage(string.Empty);
                    SetBusy(false, string.Empty);
                    mIsQueryContinue = false;
                    mCurrentRecordInfoItem = null;
                    InitBasicOperations();
                    CreateOptButtons();
                    SetPageState();
                    SetQueryBtnEnable(true);
                    QueryStop.Visibility = Visibility.Collapsed;
                    #region 写操作日志

                    strConditionLog += string.Format("{0} {1} ", mRecordTotal, Utils.FormatOptLogString("31021002"));
                    CurrentApp.WriteOperationLog(S3102Consts.OPT_QUERYRECORD.ToString(), ConstValue.OPT_RESULT_SUCCESS,
                        strConditionLog);

                    #endregion
                };
                mWorker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void SaveQueryResult(QueryCondition queryCondition)
        {
            try
            {
                if (queryCondition == null) { return; }
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3102Codes.SaveQueryResult;
                webRequest.ListData.Add(queryCondition.ID.ToString());
                webRequest.ListData.Add(mRecordTotal.ToString());
                Service31021Client client = new Service31021Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31021"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    CurrentApp.WriteLog("SaveQueryCondition", string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                CurrentApp.WriteLog("SaveQueryCondition", string.Format("End.\t{0}", webReturn.Data));
            }
            catch (Exception ex)
            {
                CurrentApp.WriteLog("SaveQueryCondition", string.Format("Fail.\t{0}", ex.Message));
            }
        }

        private void QueryRecord(string strConditionString, List<QueryConditionDetail> listDetails)
        {
            try
            {
                QueryStateInfo queryStateInfo = new QueryStateInfo();
                queryStateInfo.ConditionDetail = listDetails;
                queryStateInfo.ConditionString = strConditionString;
                queryStateInfo.RowID = 0;
                queryStateInfo.StatisticalTableName = StatisticalPartTable(listDetails);
                var tableInfo =
                    CurrentApp.Session.ListPartitionTables.FirstOrDefault(
                        t => t.TableName == ConstValue.TABLE_NAME_RECORD && t.PartType == TablePartType.DatetimeRange);
                if (tableInfo == null)
                {
                    tableInfo =
                    CurrentApp.Session.ListPartitionTables.FirstOrDefault(
                        t => t.TableName == ConstValue.TABLE_NAME_RECORD && t.PartType == TablePartType.VoiceID);
                    if (tableInfo == null)
                    {
                        queryStateInfo.TableName = string.Format("{0}_{1}", ConstValue.TABLE_NAME_RECORD,
                            CurrentApp.Session.RentInfo.Token);
                        QueryRecord(queryStateInfo);
                    }
                    else
                    {
                        //按录音服务器查询,没有实现，暂时还是按普通方式来
                        queryStateInfo.TableName = string.Format("{0}_{1}", ConstValue.TABLE_NAME_RECORD,
                            CurrentApp.Session.RentInfo.Token);
                        QueryRecord(queryStateInfo);
                    }
                }
                else
                {
                    //按月查询
                    var timeDetail =
                        listDetails.FirstOrDefault(d => d.ConditionItemID == S3102Consts.CON_TIMETYPEFROMTO);
                    if (timeDetail == null)
                    {
                        ShowException(string.Format("TimeTypeFromToDetail is null"));
                        return;
                    }
                    DateTime beginTime = Convert.ToDateTime(timeDetail.Value01);
                    DateTime endTime = Convert.ToDateTime(timeDetail.Value02);
                    DateTime baseTime = beginTime;
                    string partTable;
                    int monthCount = Utils.GetTimeMonthCount(beginTime, endTime);
                    for (int i = 0; i <= monthCount; i++)
                    {
                        partTable = baseTime.AddMonths(i).ToString("yyMM");
                        queryStateInfo.TableName = string.Format("{0}_{1}_{2}", ConstValue.TABLE_NAME_RECORD,
                            CurrentApp.Session.RentInfo.Token, partTable);
                        queryStateInfo.RowID = 0;
                        QueryRecord(queryStateInfo);
                        //在没查询一个分表之后将其行号  也就是T_21_001的C001字段比较的值置为0 这样才能正常比较

                    }
                }
                //tc
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        //统计分析表[ABCD]的分表
        private string StatisticalPartTable(List<QueryConditionDetail> listDetails)
        {
            QueryStateInfo queryStateInfo = new QueryStateInfo();
            var tableInfo =
                CurrentApp.Session.ListPartitionTables.FirstOrDefault(
                    t => t.TableName == ConstValue.TABLE_NAME_STATISTICS && t.PartType == TablePartType.DatetimeRange);
            if (tableInfo == null)
            {
                tableInfo =
                CurrentApp.Session.ListPartitionTables.FirstOrDefault(
                    t => t.TableName == ConstValue.TABLE_NAME_STATISTICS && t.PartType == TablePartType.VoiceID);
                if (tableInfo == null)
                {
                    queryStateInfo.TableName = string.Format("{0}_{1}", ConstValue.TABLE_NAME_STATISTICS,
                        CurrentApp.Session.RentInfo.Token);
                }
                else
                {
                    //按录音服务器查询,没有实现，暂时还是按普通方式来
                    queryStateInfo.TableName = string.Format("{0}_{1}", ConstValue.TABLE_NAME_STATISTICS,
                        CurrentApp.Session.RentInfo.Token);
                }
            }
            else
            {
                //按月查询
                var timeDetail =
                    listDetails.FirstOrDefault(d => d.ConditionItemID == S3102Consts.CON_TIMETYPEFROMTO);
                if (timeDetail == null)
                {
                    ShowException(string.Format("TimeTypeFromToDetail is null"));
                    return string.Empty;
                }
                DateTime beginTime = Convert.ToDateTime(timeDetail.Value01);
                DateTime endTime = Convert.ToDateTime(timeDetail.Value02);
                DateTime baseTime = beginTime;
                string partTable;
                int monthCount = Utils.GetTimeMonthCount(beginTime, endTime);
                for (int i = 0; i <= monthCount; i++)
                {
                    partTable = baseTime.AddMonths(i).ToString("yyMM");
                    queryStateInfo.TableName = string.Format("{0}_{1}_{2}", ConstValue.TABLE_NAME_STATISTICS,
                        CurrentApp.Session.RentInfo.Token, partTable);
                    //在没查询一个分表之后将其行号  也就是T_21_001的C001字段比较的值置为0 这样才能正常比较
                }
            }
            return queryStateInfo.TableName;
        }

        private void QueryRecord(QueryStateInfo queryStateInfo)//这个就是查询完之后继续查询的
        {
            try
            {
                string tempStaticticTableAndColumn = string.Empty;
                string tempStatictictable = string.Empty;
                string temp7 = string.Empty;
                var a0 = queryStateInfo.ConditionDetail.FirstOrDefault(p => p.ConditionItemID == S3102Consts.CON_SERVICEATTITUDE && p.IsEnable == true);

                var a1 = queryStateInfo.ConditionDetail.FirstOrDefault(p => p.ConditionItemID == S3102Consts.CON_RECORDDURATIONEXCEPT && p.IsEnable == true);
                var a2 = queryStateInfo.ConditionDetail.FirstOrDefault(p => p.ConditionItemID == S3102Consts.CON_REPEATEDCALL && p.IsEnable == true);
                var a3 = queryStateInfo.ConditionDetail.FirstOrDefault(p => p.ConditionItemID == S3102Consts.CON_CALLPEAK && p.IsEnable == true);
                var a4 = queryStateInfo.ConditionDetail.FirstOrDefault(p => p.ConditionItemID == S3102Consts.CON_PROFESSIONAlLEVEL && p.IsEnable == true);
                if (a0 != null)
                {
                    temp7 = string.Format("AND X.C002={0}.C201", queryStateInfo.StatisticalTableName);
                    tempStatictictable = "," + queryStateInfo.StatisticalTableName;
                    tempStaticticTableAndColumn = string.Format(",{0}.{1} SERVICE ", queryStateInfo.StatisticalTableName, a0.Value03);
                }
                if (a1 != null)
                {
                    temp7 = string.Format("AND X.C002={0}.C201", queryStateInfo.StatisticalTableName);
                    tempStatictictable = "," + queryStateInfo.StatisticalTableName;
                    tempStaticticTableAndColumn += string.Format(",{0}.{1} RECORDDUR ", queryStateInfo.StatisticalTableName, a1.Value03);
                }
                if (a2 != null)
                {
                    temp7 = string.Format("AND X.C002={0}.C201", queryStateInfo.StatisticalTableName);
                    tempStatictictable = "," + queryStateInfo.StatisticalTableName;
                    tempStaticticTableAndColumn += string.Format(",{0}.{1} REPEATEDCALL ", queryStateInfo.StatisticalTableName, a2.Value03);
                }
                if (a3 != null)
                {
                    temp7 = string.Format("AND X.C002={0}.C201", queryStateInfo.StatisticalTableName);
                    tempStatictictable = "," + queryStateInfo.StatisticalTableName;
                    tempStaticticTableAndColumn += string.Format(",{0}.{1} CALLPEAK ", queryStateInfo.StatisticalTableName, a3.Value03);
                }
                if (a4 != null)
                {
                    temp7 = string.Format("AND X.C002={0}.C201", queryStateInfo.StatisticalTableName);
                    tempStatictictable = "," + queryStateInfo.StatisticalTableName;
                    tempStaticticTableAndColumn += string.Format(",{0}.{1} PROFESSIONAlLEVEL ", queryStateInfo.StatisticalTableName, a4.Value03);
                }

                //if (a0 != null)
                //{
                //    temp7 = "AND X.C002={6}.C201";
                //    tempStatictictable = "," + queryStateInfo.StatisticalTableName;
                //    var b = queryStateInfo.ConditionDetail.FirstOrDefault(p => p.ConditionItemID == S3102Consts.CON_RECORDDURATIONEXCEPT && p.IsEnable == true);
                //    if (b != null)
                //    {
                //        tempStaticticTableAndColumn = string.Format(",{0}.{1} SERVICE,{0}.{2} RECORDDUR", queryStateInfo.StatisticalTableName, a0.Value03, b.Value03);
                //    }
                //    else
                //    {
                //        tempStaticticTableAndColumn = string.Format(",{0}.{1} SERVICE ", queryStateInfo.StatisticalTableName, a0.Value03);
                //    }
                //}
                //else
                //{

                //    var b = queryStateInfo.ConditionDetail.FirstOrDefault(p => p.ConditionItemID == S3102Consts.CON_RECORDDURATIONEXCEPT && p.IsEnable == true);
                //    if (b != null)
                //    {
                //        temp7 = string.Format("AND X.C002={0}.C201", queryStateInfo.StatisticalTableName);
                //        tempStatictictable = "," + queryStateInfo.StatisticalTableName;
                //        tempStaticticTableAndColumn = string.Format(",{0}.{1} RECORDDUR", queryStateInfo.StatisticalTableName, b.Value03);
                //    }
                //}
                if (!mIsQueryContinue) { return; }
                //根据设置确定所查询的媒体类型
                //注：媒体类型（C014）：0：录音录屏混合记录（老版本VCLog，UMP中录音录屏是分开的）；1：录音记录；2：录屏记录；3：艺赛旗录屏
                string strConditionMediaType = string.Empty;
                if (mListSettingInfos != null)
                {
                    var info1 =
                        mListSettingInfos.FirstOrDefault(s => s.ParamID == S3102Consts.USER_PARAM_QUERYVOICERECORD);
                    if (info1 != null && info1.StringValue == "1")
                    {
                        strConditionMediaType += string.Format(" X.C014 = '1' OR");
                    }
                    var info2 =
                        mListSettingInfos.FirstOrDefault(s => s.ParamID == S3102Consts.USER_PARAM_QUERYSCREENRECORD);
                    if (info2 != null && info2.StringValue == "1")
                    {
                        strConditionMediaType += string.Format(" X.C014 = '2' OR X.C014 = '3' OR");
                    }
                }
                //默认仅查询录音记录
                if (string.IsNullOrEmpty(strConditionMediaType))
                {
                    strConditionMediaType += string.Format(" X.C014 = '1' OR");
                }
                //不管什么方式，录音带录屏的记录都要查询出
                strConditionMediaType += string.Format(" X.C014 = '0' OR");
                //把多余的 OR去掉
                strConditionMediaType = strConditionMediaType.Substring(0, strConditionMediaType.Length - 3);
                strConditionMediaType = string.Format("({0})", strConditionMediaType);
                string strSql;
                switch (CurrentApp.Session.DBType)
                {
                    case 2:
                        strSql =
                            string.Format(
                                "SELECT TOP {0} X.*  {5} FROM {1} X {6}  WHERE X.C001 > {3} AND {2} AND {4}  {7} ORDER BY X.C001, X.C005"
                                , mPageSize
                                , queryStateInfo.TableName
                                , queryStateInfo.ConditionString
                                , queryStateInfo.RowID
                                , strConditionMediaType
                                , tempStaticticTableAndColumn
                                , tempStatictictable
                                , temp7);
                        break;
                    case 3:
                        strSql =
                            string.Format(
                                "SELECT * FROM (SELECT X.* {5} FROM {0} X {6} WHERE X.C001 > {2} AND {1} AND {4}  {7} ORDER BY X.C001,X.C005) WHERE ROWNUM <= {3} "
                                , queryStateInfo.TableName
                                , queryStateInfo.ConditionString
                                , queryStateInfo.RowID
                                , mPageSize
                                , strConditionMediaType
                                , tempStaticticTableAndColumn
                                , tempStatictictable
                                , temp7);
                        break;
                    default:
                        ShowException(string.Format("DBType invalid"));
                        return;
                }
                CurrentApp.WriteLog("QueryRecord", string.Format("{0}", strSql));

                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3102Codes.GetRecordData;
                webRequest.ListData.Add(strSql);
                webRequest.ListData.Add(tempStatictictable);
                //Service31021Client client = new Service31021Client();
                Service31021Client client = new Service31021Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31021"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    if (webReturn.Code == Defines.RET_DBACCESS_TABLE_NOT_EXIST)
                    {
                        //如果是由于表不存在，忽略此错误，因为分表的情况下确实存在某些月份的表不存在
                        CurrentApp.WriteLog("QueryRecord", string.Format("Fail.\tTable not exist.\t{0}", webReturn.Message));
                        return;
                    }
                    ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    ShowException(string.Format("Fail. ListData is null"));
                    return;
                }
                if (webReturn.ListData.Count <= 0) { return; }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<RecordInfo>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        continue;
                    }
                    RecordInfo recordInfo = optReturn.Data as RecordInfo;
                    if (recordInfo == null)
                    {
                        ShowException(string.Format("Fail. RecordInfo is null"));
                        return;
                    }
                    recordInfo.ParticipantNum = S3102App.DecryptString(recordInfo.ParticipantNum);
                    recordInfo.CalledID = S3102App.DecryptString(recordInfo.CalledID);
                    recordInfo.CallerID = S3102App.DecryptString(recordInfo.CallerID);
                    RecordInfoItem item = new RecordInfoItem(recordInfo);
                    int total = mRecordTotal + 1;
                    if (total > mMaxRecords)
                    {
                        //======================================加语言包=========================================
                        CurrentApp.ShowInfoMessage(CurrentApp.GetLanguageInfo("3102N017", string.Format("Larger than allowed max records, some record can't be displayed")));
                        //======================================by 汤澈==========================================
                        return;
                    }
                    mRecordTotal = total;
                    item.RowNumber = mRecordTotal;
                    item.StrDirection = CurrentApp.GetLanguageInfo(string.Format("3102TIP001{0}", item.Direction),
                        item.Direction.ToString());
                    if (item.IsScored == 2)
                    {
                        double dValue;
                        if (double.TryParse(item.Score, out dValue))
                        {
                            item.StrIsScored = dValue.ToString("0.00");
                        }
                        else
                        {
                            item.StrIsScored = item.Score;
                        }
                    }
                    else
                    {
                        item.StrIsScored = CurrentApp.GetLanguageInfo(string.Format("3102TIP001IsScored{0}", item.IsScored),
                            item.IsScored.ToString());
                    }
                    //item.StrIsScored = CurrentApp.GetLanguageInfo(string.Format("3102TIP001IsScored{0}", item.IsScored),
                    //    item.IsScored.ToString());
                    item.StrIsHaveBookMark = CurrentApp.GetLanguageInfo(string.Format("3102TIP002{0}", item.IsHaveBookMark),
                        item.IsHaveBookMark.ToString());
                    item.StrIsHaveMemo = CurrentApp.GetLanguageInfo(string.Format("3102TIP002{0}", item.IsHaveMemo),
                        item.IsHaveMemo.ToString());
                    item.StrIsHaveVoiceBookMark = CurrentApp.GetLanguageInfo(string.Format("3102TIP002{0}", item.IsHaveVoiceBookMark),
                        item.IsHaveVoiceBookMark.ToString());
                    item.StrServiceAttitude = CurrentApp.GetLanguageInfo(string.Format("3102TIP009{0}", item.ServiceAttitude),
                        item.ServiceAttitude);
                    item.StrProfessionalLevel = CurrentApp.GetLanguageInfo(string.Format("3102TIP009{0}", item.ProfessionalLevel),
                        item.ProfessionalLevel);
                    item.StrRecordDurationExcept = CurrentApp.GetLanguageInfo(string.Format("3102TIP010{0}", item.RecordDurationExcept),
                        item.RecordDurationExcept);
                    item.StrRepeatedCall = CurrentApp.GetLanguageInfo(string.Format("3102TIP011{0}", item.RepeatedCall),
                        item.RepeatedCall);
                    item.StrCallPeak = CurrentApp.GetLanguageInfo(string.Format("3102TIP012{0}", item.CallPeak),
                        item.CallPeak);
                    //item.Background = GetRecordBackground(item);
                    mListAllRecordInfoItems.Add(item);
                    if (mRecordTotal <= mPageSize)
                    {
                        AddNewRecord(item);
                    }
                    queryStateInfo.RowID = item.RowID;
                    SetPageState();
                }
                QueryRecord(queryStateInfo);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void QueryRecordFromPlayList(RecordPlayItem playItem)
        {
            try
            {
                long recordID = playItem.RecordID;
                string strSql = string.Format("SELECT * FROM T_21_001_{0} WHERE C002 = {1}", CurrentApp.Session.RentInfo.Token, recordID);
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3102Codes.GetRecordData;
                webRequest.ListData.Add(strSql);
                webRequest.ListData.Add(string.Empty);
                Service31021Client client = new Service31021Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31021"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    ShowException(string.Format("Fail.\t ListData is null"));
                    return;
                }
                if (webReturn.ListData.Count <= 0)
                {
                    ShowException(string.Format("Fail.\t StrRecordInfo not exist."));
                    return;
                }
                string strInfo = webReturn.ListData[0];
                OperationReturn optReturn = XMLHelper.DeserializeObject<RecordInfo>(strInfo);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                RecordInfo recordInfo = optReturn.Data as RecordInfo;
                if (recordInfo == null)
                {
                    ShowException(string.Format("Fail.\t RecordInfo is null"));
                    return;
                }
                RecordInfoItem recordItem = new RecordInfoItem(recordInfo);
                playItem.RecordInfoItem = recordItem;

                CurrentApp.WriteLog("QueryRecord", string.Format("QueryRecordFromPlayList.\t{0}", recordInfo.SerialID));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        private void PlayRecord()//增加了批量播放的功能
        {
            try
            {
                if (IsBusy)
                {
                    CurrentApp.GetLanguageInfo("3102N054", "Loading,Please Wait");
                    return;
                }
                RecordPlayItem first = null;
                var items = LvRecordData.SelectedItems;   //listview控件 的 SelectedItems方法是获得当前选中的项（可以很多~~）
                for (int i = 0; i < items.Count; i++)
                {
                    OperationReturn optReturn = GetRecordPlayInfoID();
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    var recordItem = items[i] as RecordInfoItem;
                    if (recordItem != null)
                    {
                        RecordPlayInfo info = new RecordPlayInfo();              //播放历史信息
                        info.SerialID = Convert.ToInt64(optReturn.Data.ToString());
                        info.RecordID = recordItem.SerialID;
                        info.RecordReference = recordItem.RecordReference;
                        info.StartPosition = 0;
                        info.StopPosition = recordItem.Duration * 1000;
                        info.Duration = info.StopPosition - info.StartPosition;
                        info.Player = CurrentApp.Session.UserID;
                        info.PlayTime = DateTime.Now;
                        info.PlayTimes = 0;
                        info.PlayTerminal = 1;
                        RecordPlayItem item = new RecordPlayItem(info);
                        item.RecordInfoItem = recordItem;
                        item.RowNumber = mListRecordPlayItems.Count + 1;
                        item.Background = GetPlayBackground(item);
                        mListRecordPlayItems.Add(item);
                        if (i == 0)
                        {
                            first = item;
                        }
                    }
                }
                if (first != null)
                {
                    mCurrentRecordPlayItem = first;
                    LvRecordPlay.SelectedItem = mCurrentRecordPlayItem;

                    ucConversationInfo = new UCConversationInfo();
                    ucConversationInfo.CurrentApp = CurrentApp;
                    ucConversationInfo.PageParent = this;
                    ucConversationInfo.PlayingRecord = mCurrentRecordInfoItem;
                    ConversationInfo.Child = ucConversationInfo;
                    //ucConversationInfo.aaa();

                    if (mUCPlayBox != null)
                    {
                        mUCPlayBox.Close();
                    }
                    mUCPlayBox = new UCPlayBox();
                    mUCPlayBox.CurrentApp = CurrentApp;
                    mUCPlayBox.PlayStopped += mUCPlayBox_PlayStopped;
                    mUCPlayBox.MainPage = this;
                    mUCPlayBox.ListEncryptInfo = mListRecordEncryptInfos;
                    mUCPlayBox.ListEncryptInfo = mListRecordEncryptInfos;
                    mUCPlayBox.ListUserSettingInfos = mListSettingInfos;
                    mUCPlayBox.ListSftpServers = mListSftpServers;
                    mUCPlayBox.ListDownloadParams = mListDownloadParams;
                    mUCPlayBox.Service03Helper = mService03Helper;
                    mUCPlayBox.RecordPlayItem = first;
                    mUCPlayBox.IsAutoPlay = true;
                    mUCPlayBox.CircleMode = mCircleMode;
                    mUCPlayBox.StartPosition = first.StartPosition;
                    mUCPlayBox.StopPostion = first.StopPosition;
                    //这里先注释掉,先不要做语音文字同步
                    mUCPlayBox.PlayerEvent += mUCPlayBox_PlayerEvent;

                    BorderPlayBox.Child = mUCPlayBox;

                    #region 写操作日志    (由于增加一个批量播放的功能，先遗留)

                    //string strLog = string.Format("{0} {1}", Utils.FormatOptLogString("COL3102001RecordReference"), info.RecordReference);
                    //CurrentApp.WriteOperationLog(S3102Consts.OPT_PLAYRECORD.ToString(), ConstValue.OPT_RESULT_SUCCESS, strLog);

                    #endregion

                    ShowPanel(1);
                    //ShowPanel(S3102Consts.PANEL_NAME_CONVERSATIONINFO, true);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void SetPlayItemStartPosition()
        {
            try
            {
                var playItem = LvRecordPlay.SelectedItem as RecordPlayItem;
                if (playItem == null) { return; }
                if (mUCPlayBox == null || mUCPlayBox.RecordInfoItem == null) { return; }
                TimeSpan ts = mUCPlayBox.GetCurrentTime();
                playItem.StartPosition = ts.TotalMilliseconds;
                //最短时长1s
                if (playItem.StartPosition + 1000 > playItem.StopPosition)
                {
                    playItem.StopPosition = playItem.StartPosition + 1000;
                }
                playItem.Duration = playItem.StopPosition - playItem.StartPosition;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void SetPlayItemStopPosition()
        {
            try
            {
                var playItem = LvRecordPlay.SelectedItem as RecordPlayItem;
                if (playItem == null) { return; }
                if (mUCPlayBox == null || mUCPlayBox.RecordInfoItem == null) { return; }
                TimeSpan ts = mUCPlayBox.GetCurrentTime();
                playItem.StopPosition = ts.TotalMilliseconds;
                //最短时长1s
                if (playItem.StopPosition - 1000 < playItem.StartPosition)
                {
                    playItem.StopPosition = playItem.StartPosition + 1000;
                }
                playItem.Duration = playItem.StopPosition - playItem.StartPosition;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void SetCircleMode()
        {
            if (mCircleMode == 0)
            {
                mCircleMode = 1;
            }
            else if (mCircleMode == 1)
            {
                mCircleMode = 2;
            }
            else
            {
                mCircleMode = 0;
            }
            if (mUCPlayBox != null)
            {
                mUCPlayBox.CircleMode = mCircleMode;
            }
            CreatePlayButtons();
        }

        private void SavePlayInfo()
        {
            try
            {
                List<RecordPlayInfo> listInfos = new List<RecordPlayInfo>();
                for (int i = 0; i < mListRecordPlayItems.Count; i++)
                {
                    RecordPlayItem item = mListRecordPlayItems[i];
                    item.SetPlayInfo();
                    RecordPlayInfo info = item.RecordPlayInfo;
                    if (info != null)
                    {
                        listInfos.Add(info);
                    }
                }
                int count = listInfos.Count;
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3102Codes.SavePlayInfo;
                webRequest.ListData.Add(count.ToString());
                OperationReturn optReturn;
                for (int i = 0; i < count; i++)
                {
                    optReturn = XMLHelper.SeriallizeObject(listInfos[i]);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    webRequest.ListData.Add(optReturn.Data.ToString());
                }
                Service31021Client client = new Service31021Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(
                        CurrentApp.Session.AppServerInfo,
                        "Service31021"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                CurrentApp.WriteLog("SavePlayInfo",
                    string.Format("{0}", webReturn.ListData == null ? string.Empty : webReturn.ListData.Count.ToString()));
                ShowInformation(string.Format(CurrentApp.GetLanguageInfo("3102N024", "Save play info end")));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void MemoRecord()
        {
            if (mCurrentRecordInfoItem == null) { return; }
            try
            {

                //===========================================解决了备注不能播放录音的bug==================================================
                OperationReturn optReturn = GetRecordPlayInfoID();
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                RecordPlayInfo info = new RecordPlayInfo();

                info.SerialID = Convert.ToInt64(optReturn.Data.ToString());
                info.RecordID = mCurrentRecordInfoItem.SerialID;
                info.RecordReference = mCurrentRecordInfoItem.RecordReference;
                info.StartPosition = 0;
                info.StopPosition = mCurrentRecordInfoItem.Duration * 1000;
                info.Duration = info.StopPosition - info.StartPosition;
                info.Player = CurrentApp.Session.UserID;
                info.PlayTime = DateTime.Now;
                info.PlayTimes = 0;
                info.PlayTerminal = 1;
                RecordPlayItem item = new RecordPlayItem(info);
                item.RecordInfoItem = mCurrentRecordInfoItem;
                item.RowNumber = mListRecordPlayItems.Count + 1;
                item.Background = GetPlayBackground(item);
                mListRecordPlayItems.Add(item);
                mCurrentRecordPlayItem = item;
                LvRecordPlay.SelectedItem = mCurrentRecordPlayItem;
                //=====================================================by 汤澈=========================


                mUCPlayBox = new UCPlayBox();
                mUCPlayBox.CurrentApp = CurrentApp;
                mUCPlayBox.PlayStopped += mUCPlayBox_PlayStopped;
                mUCPlayBox.MainPage = this;
                mUCPlayBox.ListEncryptInfo = mListRecordEncryptInfos;
                mUCPlayBox.ListUserSettingInfos = mListSettingInfos;
                mUCPlayBox.ListSftpServers = mListSftpServers;
                mUCPlayBox.ListDownloadParams = mListDownloadParams;
                mUCPlayBox.Service03Helper = mService03Helper;
                mUCPlayBox.RecordInfoItem = mCurrentRecordInfoItem;
                mUCPlayBox.IsAutoPlay = true;
                mUCPlayBox.StartPosition = item.StartPosition;
                mUCPlayBox.StopPostion = item.StopPosition;
                mUCPlayBox.CircleMode = 0;
                BorderPlayBox.Child = mUCPlayBox;

                mUCRecordMemo = new UCRecordMemo();
                mUCRecordMemo.CurrentApp = CurrentApp;
                mUCRecordMemo.PageParent = this;
                mUCRecordMemo.RecordInfoItem = mCurrentRecordInfoItem;
                BorderMemo.Child = mUCRecordMemo;

                ShowPanel(2);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void BookmarkRecord()
        {
            if (mCurrentRecordInfoItem == null) { return; }
            try
            {
                mUCRecordBookmark = new UCRecordBookmark();
                mUCRecordBookmark.CurrentApp = CurrentApp;
                mUCRecordBookmark.PageParent = this;
                mUCRecordBookmark.ListUserSettingInfos = mListSettingInfos;
                mUCRecordBookmark.ListSftpServers = mListSftpServers;
                mUCRecordBookmark.ListDownloadParams = mListDownloadParams;
                mUCRecordBookmark.ListEncryptInfo = mListRecordEncryptInfos;
                mUCRecordBookmark.Service03Helper = mService03Helper;
                mUCRecordBookmark.RecordInfoItem = mCurrentRecordInfoItem;
                BorderBookmark.Child = mUCRecordBookmark;

                ShowPanel(4);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void ScoreRecord()
        {
            LoadUserScoreSheetList();
            LoadUserScoreResultList();
            ShowPanel(3);
        }

        private void LoadUserScoreSheetList()//获得未打分评分表，即可用的评分表
        {
            try
            {
                if (mCurrentRecordInfoItem == null) { return; }
                GetAllObjects();
                long agentID = 0;
                long orgID = 0;
                var agentInfo = mListAllObjects.FirstOrDefault(a => a.ObjType == ConstValue.RESOURCE_AGENT && a.Name == mCurrentRecordInfoItem.Agent);
                if (agentInfo != null)
                {
                    agentID = agentInfo.ObjID;
                    var orgInfo = agentInfo.Parent as ObjectItem;
                    if (orgInfo != null)
                    {
                        if (orgInfo.ObjType == ConstValue.RESOURCE_ORG)
                        {
                            orgID = orgInfo.ObjID;
                        }
                    }
                }
                TxtScoreSerialID.Text = mCurrentRecordInfoItem.SerialID.ToString();
                TxtScoreStartRecordTime.Text = mCurrentRecordInfoItem.LocalStartRecordTime.ToString("yyyy-MM-dd HH:mm:ss");
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S3102Codes.GetUserScoreSheetList;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(mCurrentRecordInfoItem.SerialID.ToString());
                webRequest.ListData.Add(agentID.ToString());//这句是找到每条录音记录的坐席
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());//因为用户和评分表没有关系,所以我屏蔽这句
                webRequest.ListData.Add("0");
                int aaa = webRequest.ListData.Count;
                Service31021Client client = new Service31021Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31021"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                //string strtemp = webReturn.ListData[0];
                if (!webReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    ShowException(string.Format("Fail.\tListData is null"));
                    return;
                }

                mListScoreSheetItems.Clear();
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[i];
                    OperationReturn optReturn = XMLHelper.DeserializeObject<BasicScoreSheetInfo>(strInfo);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    BasicScoreSheetInfo info = optReturn.Data as BasicScoreSheetInfo;
                    if (info == null)
                    {
                        ShowException(string.Format("Fail.\tBaiscScoreSheetInfo is null"));
                        return;
                    }
                    info.AgentID = agentID;
                    info.OrgID = orgID;
                    BasicScoreSheetItem item = new BasicScoreSheetItem(info);
                    if (item.Flag == 0)
                    {
                        item.StrScore = CurrentApp.GetLanguageInfo("3102TIP001IsScored0", "No Score");
                    }
                    else
                    {
                        item.StrScore = item.Score.ToString();
                    }
                    for (int j = 0; j < mScoreParams.Count; j++)
                    {
                        if (mScoreParams[j].Type == "1" && mScoreParams[j].Value == "1" && IsTasked_)
                        {
                            item.IsVisible = false;
                            break;
                        }
                        if (mScoreParams[j].Type == "2" && mScoreParams[j].Value == "1" && IsCompained_)
                        {
                            item.IsVisible = false;
                            break;
                        }
                        item.IsVisible = true;
                    }
                    item.RowNumber = i + 1;
                    item.Background = GetScoreSheetBackground(item);
                    mListScoreSheetItems.Add(item);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void LoadUserScoreResultList()//获得已打分的评分表
        {
            try
            {
                if (mCurrentRecordInfoItem == null) { return; }
                GetAllObjects();
                long agentID = 0;
                var agentInfo = mListAllObjects.FirstOrDefault(a => a.ObjType == ConstValue.RESOURCE_AGENT && a.Name == mCurrentRecordInfoItem.Agent);
                if (agentInfo != null)
                {
                    agentID = agentInfo.ObjID;
                    var orgInfo = agentInfo.Parent as ObjectItem;
                    if (orgInfo != null)
                    {
                        if (orgInfo.ObjType == ConstValue.RESOURCE_ORG)
                        {
                        }
                    }
                }
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S3102Codes.GetUserScoreSheetList;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(mCurrentRecordInfoItem.SerialID.ToString());
                webRequest.ListData.Add(agentID.ToString());//这句是找到每条录音记录的坐席
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());//因为用户和评分表没有关系,所以我屏蔽这句
                webRequest.ListData.Add("1");
                Service31021Client client = new Service31021Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31021"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    ShowException(string.Format("Fail.\tListData is null"));
                    return;
                }
                List<BasicScoreSheetInfo> listItems = new List<BasicScoreSheetInfo>();
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[i];
                    OperationReturn optReturn = XMLHelper.DeserializeObject<BasicScoreSheetInfo>(strInfo);
                    if (!optReturn.Result)
                    {
                        ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    BasicScoreSheetInfo info = optReturn.Data as BasicScoreSheetInfo;
                    if (info == null)
                    {
                        ShowException(string.Format("Fail.\tBaiscScoreSheetInfo is null"));
                        return;
                    }
                    listItems.Add(info);
                    //BasicScoreSheetItem item =
                    //    mListScoreSheetItems.FirstOrDefault(s => s.ScoreSheetID == info.ScoreSheetID);
                    //if (item != null)
                    //{
                    //    //可能有多次打分成绩，取最近的打分成绩
                    //    if (info.ScoreResultID > item.ScoreResultID)
                    //    {
                    //        item.ScoreSheetInfo = info;
                    //        item.ScoreResultID = info.ScoreResultID;
                    //        item.Score = info.Score;
                    //        item.Flag = 1;
                    //    }
                    //}
                }
                for (int i = 0; i < mListScoreSheetItems.Count; i++)
                {
                    BasicScoreSheetItem item = mListScoreSheetItems[i];
                    for (int j = 0; j < mScoreParams.Count; j++)
                    {
                        if (mScoreParams[j].Type == "1" && mScoreParams[j].Value == "1" && IsTasked_)
                        {
                            item.IsVisible = false;
                            break;
                        }
                        if (mScoreParams[j].Type == "2" && mScoreParams[j].Value == "1" && IsCompained_)
                        {
                            item.IsVisible = false;
                            break;
                        }
                        item.IsVisible = true;
                    }

                    var temp = listItems.FirstOrDefault(s => s.ScoreSheetID == item.ScoreSheetID && s.IsFinalScore);
                    if (temp == null)
                    {
                        item.IsFinalScore = true;
                        item.Flag = 0;
                        item.StrScore = CurrentApp.GetLanguageInfo("3102TIP001IsScored0", "No Score");
                    }
                    else
                    {
                        item.ScoreResultID = temp.ScoreResultID;
                        if (item.ScoreSheetInfo != null)
                        {
                            item.ScoreSheetInfo.ScoreResultID = item.ScoreResultID;
                        }
                        item.Score = temp.Score;
                        item.Flag = 1;
                        item.StrScore = item.Score.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void OpenExportDataOption()
        {
            var settingInfo =
                mListSettingInfos.FirstOrDefault(s => s.ParamID == S3102Consts.USER_PARAM_EXPORTDATA_NOTSHOW);
            if (settingInfo != null && settingInfo.StringValue == "1")
            {
                ExportData();
            }
            else
            {
                UCExportDataOption ucOption = new UCExportDataOption();
                ucOption.CurrentApp = CurrentApp;
                ucOption.PageParent = this;
                PopupPanel.Title = "ExportData Option";
                PopupPanel.Content = ucOption;
                PopupPanel.IsOpen = true;
            }
        }

        public void ExportData()
        {
            try
            {
                string strLog = string.Empty;
                var settingInfo =
                    mListSettingInfos.FirstOrDefault(s => s.ParamID == S3102Consts.USER_PARAM_EXPORTDATA_TYPE);
                List<RecordInfoItem> listData = new List<RecordInfoItem>();
                if (settingInfo != null && settingInfo.StringValue == "2")
                {
                    strLog += string.Format("{0}<{0}{1}{0}>{0} ", ConstValue.SPLITER_CHAR_2, "31021501");
                    for (int i = 0; i < mListCurrentRecordInfoItems.Count; i++)
                    {
                        listData.Add(mListCurrentRecordInfoItems[i]);
                    }
                }
                else if (settingInfo != null && settingInfo.StringValue == "3")
                {
                    strLog += string.Format("{0}<{0}{1}{0}>{0} ", ConstValue.SPLITER_CHAR_2, "31021502");
                    for (int i = 0; i < mListAllRecordInfoItems.Count; i++)
                    {
                        listData.Add(mListAllRecordInfoItems[i]);
                    }
                }
                else
                {
                    strLog += string.Format("{0}<{0}{1}{0}>{0} ", ConstValue.SPLITER_CHAR_2, "31021500");
                    var items = LvRecordData.SelectedItems;
                    if (items != null)
                    {
                        for (int i = 0; i < items.Count; i++)
                        {
                            var item = items[i] as RecordInfoItem;
                            if (item != null)
                            {
                                listData.Add(item);
                            }
                        }
                    }
                }
                if (mExportDataHelper == null)
                {
                    mExportDataHelper = new ExportDataHelper();
                }
                OperationReturn optReturn = mExportDataHelper.ExportDataToExecel(listData, mListRecordDataColumns.ToList(), CurrentApp);
                if (!optReturn.Result)
                {
                    if (optReturn.Code == Defines.RET_NOT_EXIST)
                    {
                        ShowException(string.Format(CurrentApp.GetLanguageInfo("3102N027", "Fail.\tNo Record Data")));
                        return;
                    }
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                if (optReturn.Code == Defines.RET_ALREADY_EXIST) { return; }

                #region 写操作日志

                strLog += string.Format("{0} {1}", listData.Count, Utils.FormatOptLogString("31021002"));
                CurrentApp.WriteOperationLog(S3102Consts.OPT_EXPORTDATA.ToString(), ConstValue.OPT_RESULT_SUCCESS, strLog);

                #endregion

                CurrentApp.ShowInfoMessage(string.Format("{0}\t{1}", CurrentApp.GetMessageLanguageInfo("006", "Export data end"),
                    optReturn.Data));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void OpenExportRecordOption()
        {
            UCExportRecordOption ucOption = new UCExportRecordOption();
            ucOption.CurrentApp = CurrentApp;
            ucOption.MainPage = this;
            PopupPanel.Title = "Export Record Option";
            List<RecordInfoItem> listItems = new List<RecordInfoItem>();
            var items = LvRecordData.SelectedItems;
            if (items != null)
            {
                for (int i = 0; i < items.Count; i++)
                {
                    RecordInfoItem item = items[i] as RecordInfoItem;
                    if (item != null)
                    {
                        listItems.Add(item);
                    }
                }
            }
            ucOption.ListRecordItems = listItems;
            ucOption.ListSftpServers = mListSftpServers;
            ucOption.ListDownloadParams = mListDownloadParams;
            ucOption.ListEncryptInfo = mListRecordEncryptInfos;
            ucOption.ListUserSettingInfos = mListSettingInfos;
            ucOption.Service03Helper = mService03Helper;
            PopupPanel.Content = ucOption;
            PopupPanel.IsOpen = true;
        }

        private void OpenRecordScoreDetail()
        {
            if (mCurrentRecordInfoItem == null) { return; }
            try
            {
                RecordScoreDetail recordScoreDetail = new RecordScoreDetail();
                recordScoreDetail.CurrentApp = CurrentApp;
                recordScoreDetail.MainPage = this;
                recordScoreDetail.SelectRecordInfoItem = mCurrentRecordInfoItem;
                PopupPanel.Title = "Record Score Detail";
                PopupPanel.Content = recordScoreDetail;
                PopupPanel.IsOpen = true;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }

        }

        //打开教材库的文件夹的树
        private void OpenLibraryTree()
        {
            if (mCurrentRecordInfoItem == null) { return; }
            try
            {
                UCLibraryTree ucLibraryTree = new UCLibraryTree();
                ucLibraryTree.MainPage = this;
                ucLibraryTree.CurrentApp = CurrentApp;
                ucLibraryTree.SelectRecordInfoItem = mCurrentRecordInfoItem;

                PopupPanel.Title = "Library Tree";
                PopupPanel.Content = ucLibraryTree;
                PopupPanel.IsOpen = true;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void SaveLayout()
        {
            try
            {
                MemoryStream ms = new MemoryStream();
                var serializer = new XmlLayoutSerializer(PanelManager);
                string strLayoutInfo;
                using (var stream = new StreamWriter(ms))
                {
                    serializer.Serialize(stream);
                    ms.Position = 0;
                    byte[] byteLayoutInfo = new byte[ms.Length];
                    ms.Read(byteLayoutInfo, 0, byteLayoutInfo.Length);
                    strLayoutInfo = Encoding.UTF8.GetString(byteLayoutInfo);
                }
                if (string.IsNullOrEmpty(strLayoutInfo))
                {
                    ShowException(string.Format("Fail.\tLayoutInfo is null"));
                    return;
                }
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)RequestCode.WSSaveLayoutInfo;
                webRequest.ListData.Add("310201");
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                webRequest.ListData.Add(strLayoutInfo);
                Service11012Client client = new Service11012Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(
                        CurrentApp.Session.AppServerInfo,
                        "Service11012"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }

                #region 写操作日志

                string strLog = string.Empty;
                CurrentApp.WriteOperationLog(S3102Consts.OPT_SAVELAYOUT.ToString(), ConstValue.OPT_RESULT_SUCCESS, strLog);

                #endregion

                CurrentApp.ShowInfoMessage(string.Format(CurrentApp.GetLanguageInfo("3102N025", "Save layout end.")));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void ResetLayout()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)RequestCode.WSGetLayoutInfo;
                webRequest.ListData.Add("310201");
                webRequest.ListData.Add("0");
                webRequest.ListData.Add("0");
                Service11012Client client = new Service11012Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(
                        CurrentApp.Session.AppServerInfo,
                        "Service11012"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    CurrentApp.WriteLog("ResetLayout", string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                string strLayoutInfo = webReturn.Data;
                if (string.IsNullOrEmpty(strLayoutInfo))
                {
                    CurrentApp.WriteLog("ResetLayout", string.Format("Fail.\tLayoutInfo is empty"));
                    return;
                }
                mLayoutInfo = strLayoutInfo;
                InitLayout();
                SetViewStatus();

                #region 写操作日志

                string strLog = string.Empty;
                CurrentApp.WriteOperationLog(S3102Consts.OPT_RESETLGYOUT.ToString(), ConstValue.OPT_RESULT_SUCCESS, strLog);

                #endregion
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void RestoreLayout()
        {
            try
            {
                LoadLayout();
                InitLayout();
                SetViewStatus();

                #region 写操作日志

                string strLog = string.Empty;
                CurrentApp.WriteOperationLog(S3102Consts.OPT_RESTORELAYOUT.ToString(), ConstValue.OPT_RESULT_SUCCESS, strLog);

                #endregion
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
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
                ShowException(ex.Message);
            }
        }

        #endregion

        #region Others

        private void ParseStartArgs()
        {
            try
            {
                if (CurrentApp == null) { return; }
                string strArgs = CurrentApp.StartArgs;
                if (string.IsNullOrEmpty(strArgs)) { return; }
                CurrentApp.WriteLog("ParseStartArgs", string.Format("StartArgs:{0}", strArgs));
                string[] arrArgs = strArgs.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < arrArgs.Length; i++)
                {
                    string strSingleArg = arrArgs[i];
                    int indexParam = strSingleArg.IndexOf(":");
                    if (indexParam < 0) { continue; }
                    string strParam = strSingleArg.Substring(0, indexParam);
                    string strValue = strSingleArg.Substring(indexParam + 1);
                    long longValue;
                    switch (strParam)
                    {
                        case "/FQ":
                            if (long.TryParse(strValue, out longValue))
                            {
                                CurrentApp.WriteLog("ParseStartArgs", string.Format("FQ;QueryID:{0}", longValue));
                                OnFastQuery(longValue);
                            }
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void OnFastQuery(long queryID)
        {
            try
            {
                for (int i = 0; i < ComboQueryConditions.Items.Count; i++)
                {
                    var item = ComboQueryConditions.Items[i] as QueryCondition;
                    if (item == null) { continue; }
                    if (item.ID == queryID)
                    {
                        ComboQueryConditions.SelectedItem = item;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void AddNewRecord(RecordInfoItem recordInfoItem)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                var count = mListCurrentRecordInfoItems.Count;
                Brush brush = Brushes.Transparent;
                if (count % 2 == 0)
                {
                    brush = Brushes.LightGray;
                }
                recordInfoItem.Background = brush;
                mListCurrentRecordInfoItems.Add(recordInfoItem);
            }));
        }

        private void AddChildObject(ObjectItem parentItem, ObjectItem item)
        {
            Dispatcher.Invoke(new Action(() => parentItem.AddChild(item)));
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
                    RecordInfoItem item = mListAllRecordInfoItems[i];
                    item.StrDirection = CurrentApp.GetLanguageInfo(string.Format("3102TIP001{0}", item.Direction),
                        item.Direction.ToString());
                    if (item.IsScored == 2)
                    {
                        item.StrIsScored = item.Score;
                    }
                    else
                    {
                        item.StrIsScored = CurrentApp.GetLanguageInfo(string.Format("3102TIP001IsScored{0}", item.IsScored),
                            item.IsScored.ToString());
                    }
                    //item.StrIsScored = CurrentApp.GetLanguageInfo(string.Format("3102TIP001IsScored{0}", item.IsScored),
                    //    item.IsScored.ToString());
                    item.StrIsHaveBookMark = CurrentApp.GetLanguageInfo(string.Format("3102TIP002{0}", item.IsHaveBookMark),
                        item.IsHaveBookMark.ToString());
                    item.StrIsHaveMemo = CurrentApp.GetLanguageInfo(string.Format("3102TIP002{0}", item.IsHaveMemo),
                        item.IsHaveMemo.ToString());
                    item.StrIsHaveVoiceBookMark = CurrentApp.GetLanguageInfo(string.Format("3102TIP002{0}", item.IsHaveVoiceBookMark),
                        item.IsHaveVoiceBookMark.ToString());
                    item.StrServiceAttitude = CurrentApp.GetLanguageInfo(string.Format("3102TIP009{0}", item.ServiceAttitude),
                        item.ServiceAttitude);
                    item.StrProfessionalLevel = CurrentApp.GetLanguageInfo(string.Format("3102TIP009{0}", item.ProfessionalLevel),
                        item.ProfessionalLevel);
                    item.StrRecordDurationExcept = CurrentApp.GetLanguageInfo(string.Format("3102TIP010{0}", item.RecordDurationExcept),
                        item.RecordDurationExcept);
                    item.StrRepeatedCall = CurrentApp.GetLanguageInfo(string.Format("3102TIP011{0}", item.RepeatedCall),
                        item.RepeatedCall);
                    item.StrCallPeak = CurrentApp.GetLanguageInfo(string.Format("3102TIP012{0}", item.CallPeak),
                        item.CallPeak);
                    if (i % 2 == 0)
                    {
                        item.Background = Brushes.LightGray;
                    }
                    else
                    {
                        item.Background = Brushes.Transparent;
                    }
                    mListCurrentRecordInfoItems.Add(item);
                }
                InitBasicOperations();
                CreateOptButtons();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void SetService03Helper()
        {
            try
            {
                mService03Helper.HostAddress = CurrentApp.Session.AppServerInfo.Address;
                if (CurrentApp.Session.AppServerInfo.SupportHttps)
                {
                    mService03Helper.HostPort = CurrentApp.Session.AppServerInfo.Port - 4;
                }
                else
                {
                    mService03Helper.HostPort = CurrentApp.Session.AppServerInfo.Port - 3;
                }

                ////For Test
                //mService03Helper.HostAddress = "192.168.5.31";
                //mService03Helper.HostPort = 8081 - 3;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void SetPageState()//设置的每页的状态
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
                string strPageInfo = string.Format("{0}/{1} {2} {3}", mPageIndex + 1, mPageCount, mRecordTotal,
                    CurrentApp.GetLanguageInfo("31021002", "Records"));
                Dispatcher.Invoke(new Action(() =>
                {
                    TxtPageInfo.Text = strPageInfo;
                    TxtPage.Text = (mPageIndex + 1).ToString();
                }));
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void CreateRecordDataColumns()
        {
            try
            {
                GridView gv = new GridView();
                GridViewColumn gvc;
                GridViewColumnHeader gvch;
                for (int i = 0; i < mListRecordDataColumns.Count; i++)
                {
                    ViewColumnInfo columnInfo = mListRecordDataColumns[i];
                    if (columnInfo.Visibility == "1")
                    {
                        gvc = new GridViewColumn();
                        gvch = new GridViewColumnHeader();
                        gvch.Content = columnInfo.Display;
                        gvch.Content = CurrentApp.GetLanguageInfo(string.Format("COL3102001{0}", columnInfo.ColumnName), columnInfo.Display);
                        gvch.ToolTip = CurrentApp.GetLanguageInfo(string.Format("COL3102001{0}", columnInfo.ColumnName), columnInfo.Display);
                        gvch.Command = QMMainPageCommands.GridViewColumnHeaderCommand;
                        gvch.CommandParameter = columnInfo;
                        gvc.Header = gvch;
                        gvc.Width = columnInfo.Width;
                        Binding binding = null;
                        DataTemplate dt = null;
                        string strName = columnInfo.ColumnName;
                        if (columnInfo.ColumnName == "MediaType")
                        {
                            dt = Resources["CellMediaTypeTemplate"] as DataTemplate;
                        }
                        if (columnInfo.ColumnName == "RowNumber")
                        {
                            dt = Resources["CellRowNumberTemplate"] as DataTemplate;
                        }
                        if (dt != null)
                        {
                            gvc.CellTemplate = dt;
                        }
                        else
                        {
                            if (columnInfo.ColumnName == "StartRecordTime")
                            {
                                strName = "LocalStartRecordTime";
                            }
                            if (columnInfo.ColumnName == "StopRecordTime")
                            {
                                strName = "LocalStopRecordTime";
                            }
                            if (columnInfo.ColumnName == "Direction")
                            {
                                //从数据库读出来的呼叫方向是用0或1代表的  这里是将其显示的内容绑定到这列中
                                strName = "StrDirection";
                            }
                            if (columnInfo.ColumnName == "IsScored")
                            {
                                strName = "StrIsScored";
                            }
                            if (columnInfo.ColumnName == "IsHaveBookMark")
                            {
                                strName = "StrIsHaveBookMark";
                            }
                            if (columnInfo.ColumnName == "IsHaveMemo")
                            {
                                strName = "StrIsHaveMemo";
                            }
                            if (columnInfo.ColumnName == "IsHaveVoiceBookMark")
                            {
                                strName = "StrIsHaveVoiceBookMark";
                            }
                            if (columnInfo.ColumnName == "ServiceAttitude")
                            {
                                strName = "StrServiceAttitude";
                            }
                            if (columnInfo.ColumnName == "RecordDurationExcept")
                            {
                                strName = "StrRecordDurationExcept";
                            }
                            if (columnInfo.ColumnName == "RepeatedCall")
                            {
                                strName = "StrRepeatedCall";
                            }
                            if (columnInfo.ColumnName == "CallPeak")
                            {
                                strName = "StrCallPeak";
                            }
                            if (columnInfo.ColumnName == "ProfessionalLevel")
                            {
                                strName = "StrProfessionalLevel";
                            }
                            binding = new Binding(strName);
                            if (columnInfo.ColumnName == "Duration")
                            {
                                binding.Converter = new SecondToTimeConverter();
                            }
                            if (columnInfo.ColumnName == "StartRecordTime"
                                || columnInfo.ColumnName == "StopRecordTime")
                            {
                                binding.Converter = new CellDatetimeConverter();
                            }
                            dt = new DataTemplate();
                            FrameworkElementFactory fef = new FrameworkElementFactory(typeof(EditableTextBlock));
                            fef.SetBinding(EditableTextBlock.TextProperty, binding);
                            fef.SetValue(EditableTextBlock.IsReadOnlyProperty, true);
                            dt.VisualTree = fef;
                            gvc.CellTemplate = dt;
                        }
                        gv.Columns.Add(gvc);
                    }
                }
                LvRecordData.View = gv;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void CreateRecordPlayColumns()
        {
            try
            {
                GridView gv = new GridView();
                GridViewColumn gvc;
                GridViewColumnHeader gvch;
                for (int i = 0; i < mListRecordPlayColumns.Count; i++)
                {
                    ViewColumnInfo columnInfo = mListRecordPlayColumns[i];
                    if (columnInfo.Visibility == "1")
                    {
                        gvc = new GridViewColumn();
                        gvch = new GridViewColumnHeader();
                        gvch.Content = CurrentApp.GetLanguageInfo(string.Format("COL3102002{0}", columnInfo.ColumnName), columnInfo.Display);
                        gvch.ToolTip = CurrentApp.GetLanguageInfo(string.Format("COL3102002{0}", columnInfo.ColumnName), columnInfo.Display);
                        gvc.Header = gvch;
                        gvc.Width = columnInfo.Width;
                        if (columnInfo.ColumnName == "Operations")
                        {
                            DataTemplate dt = Resources["CellPlayOperationTemplate"] as DataTemplate;
                            if (dt != null)
                            {
                                gvc.CellTemplate = dt;
                            }
                            else
                            {
                                gvc.DisplayMemberBinding = new Binding(columnInfo.ColumnName);
                            }
                        }
                        else
                        {
                            gvc.DisplayMemberBinding = new Binding(columnInfo.ColumnName);
                        }
                        Binding binding;
                        if (columnInfo.ColumnName == "StartPosition"
                            || columnInfo.ColumnName == "StopPosition"
                            || columnInfo.ColumnName == "Duration")
                        {
                            binding = new Binding(columnInfo.ColumnName);
                            binding.Converter = new MilliSecondToTimeConverter();
                            gvc.DisplayMemberBinding = binding;
                        }
                        if (columnInfo.ColumnName == "PlayTime")
                        {
                            binding = new Binding(columnInfo.ColumnName);
                            binding.Converter = new CellDatetimeConverter();
                            gvc.DisplayMemberBinding = binding;
                        }
                        gv.Columns.Add(gvc);
                    }
                }
                LvRecordPlay.View = gv;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void CreateScoreSheetColumns()
        {
            try
            {
                GridView gv = new GridView();
                GridViewColumn gvc;
                GridViewColumnHeader gvch;
                for (int i = 0; i < mListScoreSheetColumns.Count; i++)
                {
                    ViewColumnInfo columnInfo = mListScoreSheetColumns[i];
                    if (columnInfo.Visibility == "1")
                    {
                        gvc = new GridViewColumn();
                        gvch = new GridViewColumnHeader();
                        gvch.Content = CurrentApp.GetLanguageInfo(string.Format("COL3102003{0}", columnInfo.ColumnName), columnInfo.Display);
                        gvch.ToolTip = CurrentApp.GetLanguageInfo(string.Format("COL3102003{0}", columnInfo.ColumnName), columnInfo.Display);
                        gvc.Header = gvch;
                        gvc.Width = columnInfo.Width;

                        DataTemplate dt = null;
                        Binding binding;
                        string strColName = columnInfo.ColumnName;
                        if (strColName == "Operations")
                        {
                            dt = Resources["CellScoreSheetOperationTemplate"] as DataTemplate;//这个模板是在QMMainPageStatic.xaml里
                            if (dt != null)
                            {
                                gvc.CellTemplate = dt;
                            }
                        }
                        if (strColName == "Score")
                        {
                            strColName = "StrScore";
                        }
                        if (dt != null)
                        {
                            gvc.CellTemplate = dt;
                        }
                        else
                        {
                            binding = new Binding(strColName);
                            gvc.DisplayMemberBinding = binding;
                        }

                        //Binding binding;
                        //if (columnInfo.ColumnName == "StartPosition"
                        //    || columnInfo.ColumnName == "StopPosition"
                        //    || columnInfo.ColumnName == "Duration")
                        //{
                        //    binding = new Binding(columnInfo.ColumnName);
                        //    binding.Converter = new SecondToTimeSpanConverter();
                        //    gvc.DisplayMemberBinding = binding;
                        //}
                        //if (columnInfo.ColumnName == "PlayTime")
                        //{
                        //    binding = new Binding(columnInfo.ColumnName);
                        //    binding.Converter = new CellDatetimeConverter();
                        //    gvc.DisplayMemberBinding = binding;
                        //}
                        gv.Columns.Add(gvc);
                    }
                }
                LvScoreSheet.View = gv;
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
                List<ToolButtonItem> listBtns = new List<ToolButtonItem>();

                ToolButtonItem item = new ToolButtonItem();
                //item.Name = "TB" + "QueryStop";
                //item.Display = CurrentApp.GetLanguageInfo("3102B005s", "SSSSS");
                //item.Tip = CurrentApp.GetLanguageInfo("3102B005s", "SSSSS");
                //item.Icon = "Images/last.ico";
                //listBtns.Add(item);
                //item = new ToolButtonItem();
                item.Name = "TB" + "FirstPage";
                item.Display = CurrentApp.GetLanguageInfo("3102B001", "First Page");
                item.Tip = CurrentApp.GetLanguageInfo("3102B001", "First Page");
                item.Icon = "Images/first.ico";
                listBtns.Add(item);
                item = new ToolButtonItem();
                item.Name = "TB" + "PrePage";
                item.Display = CurrentApp.GetLanguageInfo("3102B002", "Pre Page");
                item.Tip = CurrentApp.GetLanguageInfo("3102B002", "Pre Page");
                item.Icon = "Images/pre.ico";
                listBtns.Add(item);
                item = new ToolButtonItem();
                item.Name = "TB" + "NextPage";
                item.Display = CurrentApp.GetLanguageInfo("3102B003", "Next Page");
                item.Tip = CurrentApp.GetLanguageInfo("3102B003", "Next Page");
                item.Icon = "Images/next.ico";
                listBtns.Add(item);
                item = new ToolButtonItem();
                item.Name = "TB" + "LastPage";
                item.Display = CurrentApp.GetLanguageInfo("3102B004", "Last Page");
                item.Tip = CurrentApp.GetLanguageInfo("3102B004", "Last Page");
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
                ShowException(ex.Message);
            }
        }

        private void CreatePlayButtons()
        {
            try
            {
                List<ToolButtonItem> listBtns = new List<ToolButtonItem>();
                ToolButtonItem item = new ToolButtonItem();
                item.Name = "BT" + "SetStart";
                item.Display = CurrentApp.GetLanguageInfo("3102B013", "SetStart");
                item.Tip = CurrentApp.GetLanguageInfo("3102B013", "Set start position");
                item.Icon = "Images/startposition.png";
                listBtns.Add(item);
                item = new ToolButtonItem();
                item.Name = "BT" + "SetStop";
                item.Display = CurrentApp.GetLanguageInfo("3102B014", "SetStop");
                item.Tip = CurrentApp.GetLanguageInfo("3102B014", "Set stop position");
                item.Icon = "Images/stopposition.png";
                listBtns.Add(item);
                item = new ToolButtonItem();
                item.Name = "BT" + "SavePlay";
                item.Display = CurrentApp.GetLanguageInfo("3102B012", "Save");
                item.Tip = CurrentApp.GetLanguageInfo("3102B012", "Save");
                item.Icon = "Images/save.png";
                listBtns.Add(item);
                item = new ToolButtonItem();
                item.Name = "BT" + "CircleMode";
                item.Display = CurrentApp.GetLanguageInfo(string.Format("3102TIP005{0}", mCircleMode), mCircleMode.ToString());
                item.Tip = CurrentApp.GetLanguageInfo(string.Format("3102TIP005{0}", mCircleMode), mCircleMode.ToString());
                item.Icon = mCircleMode == 1 ? "Images/singlecircle.png" : mCircleMode == 2 ? "Images/listcircle.png" : "Images/nocircle.png";
                listBtns.Add(item);

                item = new ToolButtonItem();
                item.Name = "BT" + "Clear";
                item.Display = CurrentApp.GetLanguageInfo("3102B016", "Clear");
                item.Tip = CurrentApp.GetLanguageInfo("3102B016", "Clear");
                item.Icon = "Images/clear.png";
                listBtns.Add(item);

                PanelPlayButtons.Children.Clear();
                for (int i = 0; i < listBtns.Count; i++)
                {
                    ToolButtonItem toolBtn = listBtns[i];
                    Button btn = new Button();
                    btn.DataContext = toolBtn;
                    btn.SetResourceReference(StyleProperty, "ToolBarButtonStyle");
                    btn.Click += ToolButton_Click;
                    PanelPlayButtons.Children.Add(btn);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void OpenCloseLeftPanel()
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

        private void CreateOptButtons()
        {
            PanelBasicOpts.Children.Clear();
            OperationInfo item;
            Button btn;
            for (int i = 0; i < mListBasicOperations.Count; i++)
            {
                item = mListBasicOperations[i];
                //基本操作按钮
                btn = new Button();
                btn.Click += BasicOpt_Click;
                btn.DataContext = item;
                btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                PanelBasicOpts.Children.Add(btn);
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
                toolItem.Name = "BT" + "SaveLayout";
                toolItem.Display = CurrentApp.GetLanguageInfo("FO3102009", "Save Layout");
                toolItem.Tip = CurrentApp.GetLanguageInfo("FO3102009", "Save Layout");
                toolItem.Icon = "Images/savelayout.png";
                btn = new Button();
                btn.Click += ToolButton_Click;
                btn.DataContext = toolItem;
                btn.SetResourceReference(StyleProperty, "ToolBarButtonStyle");
                optInfo = ListOperations.FirstOrDefault(o => o.ID == S3102Consts.OPT_SAVELAYOUT);
                if (optInfo != null)
                {
                    PanelToolButton.Children.Add(btn);
                }

                toolItem = new ToolButtonItem();
                toolItem.Name = "BT" + "RestoreLayout";
                toolItem.Display = CurrentApp.GetLanguageInfo("FO3102013", "Restore Layout");
                toolItem.Tip = CurrentApp.GetLanguageInfo("FO3102013", "Restore Layout");
                toolItem.Icon = "Images/restorelayout.png";
                btn = new Button();
                btn.Click += ToolButton_Click;
                btn.DataContext = toolItem;
                btn.SetResourceReference(StyleProperty, "ToolBarButtonStyle");
                optInfo = ListOperations.FirstOrDefault(o => o.ID == S3102Consts.OPT_RESTORELAYOUT);
                if (optInfo != null)
                {
                    PanelToolButton.Children.Add(btn);
                }

                toolItem = new ToolButtonItem();
                toolItem.Name = "BT" + "ResetLayout";
                toolItem.Display = CurrentApp.GetLanguageInfo("FO3102010", "Reset Layout");
                toolItem.Tip = CurrentApp.GetLanguageInfo("FO3102010", "Reset Layout");
                toolItem.Icon = "Images/resetlayout.png";
                btn = new Button();
                btn.Click += ToolButton_Click;
                btn.DataContext = toolItem;
                btn.SetResourceReference(StyleProperty, "ToolBarButtonStyle");
                optInfo = ListOperations.FirstOrDefault(o => o.ID == S3102Consts.OPT_RESETLGYOUT);
                if (optInfo != null)
                {
                    PanelToolButton.Children.Add(btn);
                }

                toolItem = new ToolButtonItem();
                toolItem.Name = "BT" + "Setting";
                toolItem.Display = CurrentApp.GetLanguageInfo("FO3102011", "Custom Setting");
                toolItem.Tip = CurrentApp.GetLanguageInfo("FO3102011", "Custom Setting");
                toolItem.Icon = "Images/setting.png";
                btn = new Button();
                btn.Click += ToolButton_Click;
                btn.DataContext = toolItem;
                btn.SetResourceReference(StyleProperty, "ToolBarButtonStyle");
                optInfo = ListOperations.FirstOrDefault(o => o.ID == S3102Consts.OPT_CUSTOMSETTING);
                if (optInfo != null)
                {
                    PanelToolButton.Children.Add(btn);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }


        public void SetBusy(bool isBusy)
        {
            SetBusy(false, string.Empty);
        }

        private void SetViewStatus()
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

        private void SetPanelVisible()
        {
            try
            {
                var panel =
                      PanelManager.Layout.Descendents().OfType<LayoutAnchorable>().FirstOrDefault(p => p.ContentId == S3102Consts.PANEL_CONTENTID_RECORDLIST);
                if (panel != null)
                {
                    panel.Title = CurrentApp.GetLanguageInfo("3102P001", "Record List");
                }

                panel =
                      PanelManager.Layout.Descendents().OfType<LayoutAnchorable>().FirstOrDefault(p => p.ContentId == S3102Consts.PANEL_CONTENTID_PLAYLIST);
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
                        panel.Closing += PanelDocument_Closing;
                    }
                    panel.Title = CurrentApp.GetLanguageInfo("3102P002", "Play List");
                }

                panel =
                      PanelManager.Layout.Descendents().OfType<LayoutAnchorable>().FirstOrDefault(p => p.ContentId == S3102Consts.PANEL_CONTENTID_OBJECTLIST);
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
                        panel.Closing += PanelDocument_Closing;
                    }
                    panel.Title = CurrentApp.GetLanguageInfo("3102P003", "Manage Object");
                }

                panel =
                     PanelManager.Layout.Descendents().OfType<LayoutAnchorable>().FirstOrDefault(p => p.ContentId == S3102Consts.PANEL_CONTENTID_PLAYBOX);
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
                        panel.Closing += PanelDocument_Closing;
                    }
                    panel.Title = CurrentApp.GetLanguageInfo("3102P004", "Play Box");
                }

                panel =
                     PanelManager.Layout.Descendents().OfType<LayoutAnchorable>().FirstOrDefault(p => p.ContentId == S3102Consts.PANEL_CONTENTID_CALLINO);
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
                        panel.Closing += PanelDocument_Closing;
                    }
                    panel.Title = CurrentApp.GetLanguageInfo("3102P005", "Call Information");
                }

                panel =
                     PanelManager.Layout.Descendents().OfType<LayoutAnchorable>().FirstOrDefault(p => p.ContentId == S3102Consts.PANEL_CONTENTID_MEMO);
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
                        panel.Closing += PanelDocument_Closing;
                    }
                    panel.Title = CurrentApp.GetLanguageInfo("3102P006", "Memo");
                }

                panel =
                     PanelManager.Layout.Descendents().OfType<LayoutAnchorable>().FirstOrDefault(p => p.ContentId == S3102Consts.PANEL_CONTENTID_BOOKMARK);
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
                        panel.Closing += PanelDocument_Closing;
                    }
                    panel.Title = CurrentApp.GetLanguageInfo("3102P007", "Bookmark");
                }

                panel =
                     PanelManager.Layout.Descendents().OfType<LayoutAnchorable>().FirstOrDefault(p => p.ContentId == S3102Consts.PANEL_CONTENTID_SCORE);
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
                        panel.Closing += PanelDocument_Closing;
                    }
                    panel.Title = CurrentApp.GetLanguageInfo("3102P008", "Score");
                }

                panel =
                    PanelManager.Layout.Descendents().OfType<LayoutAnchorable>().FirstOrDefault(p => p.ContentId == S3102Consts.PANEL_CONTENTID_SCOREDETAIL);
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
                        panel.Closing += PanelDocument_Closing;
                    }
                    panel.Title = CurrentApp.GetLanguageInfo("3102P009", "Score Detail");
                }

                panel =
                    PanelManager.Layout.Descendents().OfType<LayoutAnchorable>().FirstOrDefault(p => p.ContentId == S3102Consts.PANEL_CONTENTID_CONVERSATIONINFO);
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
                        panel.Closing += PanelDocument_Closing;
                    }
                    panel.Title = CurrentApp.GetLanguageInfo("3102P010", "会话信息");
                }

            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private Brush GetPlayBackground(RecordPlayItem recordPlayItem)//播放列表的背景
        {
            int rowNumber = recordPlayItem.RowNumber;
            if (rowNumber % 2 == 0)
            {
                return Brushes.LightGray;
            }
            return Brushes.Transparent;
        }

        private Brush GetScoreSheetBackground(BasicScoreSheetItem scoreSheetItem)
        {
            int rowNumber = scoreSheetItem.RowNumber;
            if (rowNumber % 2 == 0)
            {
                return Brushes.LightGray;
            }
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

        private void GetCheckedObjects()
        {
            try
            {
                mListSelectedObjects.Clear();
                GetCheckedObjects(mRootItem);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void GetCheckedObjects(ObjectItem parentItem)
        {
            try
            {
                for (int i = 0; i < parentItem.Children.Count; i++)
                {
                    ObjectItem item = parentItem.Children[i] as ObjectItem;
                    if (item != null)
                    {
                        if (item.ObjType == ConstValue.RESOURCE_ORG)
                        {
                            if (item.IsChecked == false) { continue; }
                            GetCheckedObjects(item);
                        }
                        else
                        {
                            if (item.IsChecked == true)
                            {
                                mListSelectedObjects.Add(item);
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

        private void GetAllObjects()
        {
            try
            {
                mListAllObjects.Clear();
                GetAllObjects(mRootItem);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void GetAllObjects(ObjectItem parentItem)
        {
            try
            {
                for (int i = 0; i < parentItem.Children.Count; i++)
                {
                    ObjectItem item = parentItem.Children[i] as ObjectItem;
                    if (item != null)
                    {
                        mListAllObjects.Add(item);
                        GetAllObjects(item);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
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
                    item.Name = CurrentApp.GetLanguageInfo("COL3102001RecordReference", "Record Reference");
                    item.Value = mCurrentRecordInfoItem.RecordReference;
                    mListCallInfoPropertyItems.Add(item);
                    item = new CallInfoPropertyItem();
                    item.Name = CurrentApp.GetLanguageInfo("COL3102001StartRecordTime", "StartRecord Time");
                    item.Value = mCurrentRecordInfoItem.LocalStartRecordTime.ToString("yyyy-MM-dd HH:mm:ss");
                    mListCallInfoPropertyItems.Add(item);
                    item = new CallInfoPropertyItem();
                    item.Name = CurrentApp.GetLanguageInfo("COL3102001StopRecordTime", "StopRecord Time");
                    item.Value = mCurrentRecordInfoItem.LocalStopRecordTime.ToString("yyyy-MM-dd HH:mm:ss");
                    mListCallInfoPropertyItems.Add(item);
                    item = new CallInfoPropertyItem();
                    item.Name = CurrentApp.GetLanguageInfo("COL3102001Duration", "Duration");
                    //item.Value = mCurrentRecordInfoItem.Duration.ToString("00:00:00");
                    item.Value = Converter.Second2Time(mCurrentRecordInfoItem.Duration);
                    mListCallInfoPropertyItems.Add(item);
                    item = new CallInfoPropertyItem();

                    item.Name = CurrentApp.GetLanguageInfo("COL3102001Direction", "Direction");
                    if (mCurrentRecordInfoItem.Direction.ToString() == "0")
                    {
                        item.Value = CurrentApp.GetLanguageInfo("3102TIP0010", "Call Out");
                    }
                    else
                    {
                        item.Value = CurrentApp.GetLanguageInfo("3102TIP0011", "Call In");
                    }
                    //item.Value = mCurrentRecordInfoItem.Direction.ToString();
                    mListCallInfoPropertyItems.Add(item);
                    item = new CallInfoPropertyItem();
                    item.Name = CurrentApp.GetLanguageInfo("COL3102001Extension", "Extension");
                    item.Value = mCurrentRecordInfoItem.Extension;
                    mListCallInfoPropertyItems.Add(item);
                    item = new CallInfoPropertyItem();
                    item.Name = CurrentApp.GetLanguageInfo("COL3102001Agent", "Agent ID");
                    item.Value = mCurrentRecordInfoItem.Agent;
                    mListCallInfoPropertyItems.Add(item);
                    item = new CallInfoPropertyItem();
                    item.Name = CurrentApp.GetLanguageInfo("COL3102001CallerID", "Caller ID");
                    item.Value = mCurrentRecordInfoItem.CallerID;
                    mListCallInfoPropertyItems.Add(item);
                    item = new CallInfoPropertyItem();
                    item.Name = CurrentApp.GetLanguageInfo("COL3102001CalledID", "Called ID");
                    item.Value = mCurrentRecordInfoItem.CalledID;
                    mListCallInfoPropertyItems.Add(item);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private OperationReturn GetRecordPlayInfoID()
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)RequestCode.WSGetSerialID;
                webRequest.ListData.Add("31");
                webRequest.ListData.Add("305");
                webRequest.ListData.Add(DateTime.Now.ToString("yyyyMMddHHmmss"));
                Service11012Client client = new Service11012Client(
                    WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(
                        CurrentApp.Session.AppServerInfo,
                        "Service11012"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    optReturn.Result = false;
                    optReturn.Code = webReturn.Code;
                    optReturn.Message = webReturn.Message;
                    return optReturn;
                }
                optReturn.Data = webReturn.Data;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
            }
            return optReturn;
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
            switch (mode)
            {
                //初始模式
                case 0:
                    ShowPanel(S3102Consts.PANEL_NAME_OBJECTLIST, true);
                    ShowPanel(S3102Consts.PANEL_NAME_CALLINO, false);
                    ShowPanel(S3102Consts.PANEL_NAME_PLAYLIST, false);
                    ShowPanel(S3102Consts.PANEL_NAME_PLAYBOX, false);
                    ShowPanel(S3102Consts.PANEL_NAME_MEMO, false);
                    ShowPanel(S3102Consts.PANEL_NAME_BOOKMARK, false);
                    ShowPanel(S3102Consts.PANEL_NAME_SCORE, false);
                    ShowPanel(S3102Consts.PANEL_NAME_SCOREDETAIL, false);
                    ShowPanel(S3102Consts.PANEL_NAME_CONVERSATIONINFO, false);
                    break;
                //查询播放模式
                case 1:
                    ShowPanel(S3102Consts.PANEL_NAME_OBJECTLIST, false);
                    ShowPanel(S3102Consts.PANEL_NAME_CALLINO, false);
                    ShowPanel(S3102Consts.PANEL_NAME_PLAYLIST, true);
                    ShowPanel(S3102Consts.PANEL_NAME_PLAYBOX, true);
                    ShowPanel(S3102Consts.PANEL_NAME_MEMO, false);
                    ShowPanel(S3102Consts.PANEL_NAME_BOOKMARK, false);
                    ShowPanel(S3102Consts.PANEL_NAME_SCORE, false);
                    ShowPanel(S3102Consts.PANEL_NAME_SCOREDETAIL, false);
                    ShowPanel(S3102Consts.PANEL_NAME_CONVERSATIONINFO, true);
                    break;
                //备注模式
                case 2:
                    //做标记  by 汤澈   备注这里还是需要修改的
                    ShowPanel(S3102Consts.PANEL_NAME_OBJECTLIST, false);
                    ShowPanel(S3102Consts.PANEL_NAME_CALLINO, false);
                    ShowPanel(S3102Consts.PANEL_NAME_PLAYLIST, false);
                    ShowPanel(S3102Consts.PANEL_NAME_PLAYBOX, true);
                    ShowPanel(S3102Consts.PANEL_NAME_MEMO, true);
                    ShowPanel(S3102Consts.PANEL_NAME_BOOKMARK, false);
                    ShowPanel(S3102Consts.PANEL_NAME_SCORE, false);
                    ShowPanel(S3102Consts.PANEL_NAME_SCOREDETAIL, false);
                    ShowPanel(S3102Consts.PANEL_NAME_CONVERSATIONINFO, false);
                    break;
                //评分模式
                case 3:
                    ShowPanel(S3102Consts.PANEL_NAME_OBJECTLIST, false);
                    ShowPanel(S3102Consts.PANEL_NAME_CALLINO, false);
                    ShowPanel(S3102Consts.PANEL_NAME_PLAYLIST, false);
                    ShowPanel(S3102Consts.PANEL_NAME_PLAYBOX, false);
                    ShowPanel(S3102Consts.PANEL_NAME_MEMO, false);
                    ShowPanel(S3102Consts.PANEL_NAME_BOOKMARK, false);
                    ShowPanel(S3102Consts.PANEL_NAME_SCORE, true);
                    ShowPanel(S3102Consts.PANEL_NAME_SCOREDETAIL, false);
                    ShowPanel(S3102Consts.PANEL_NAME_CONVERSATIONINFO, false);
                    break;
                //Bookmark
                case 4:
                    ShowPanel(S3102Consts.PANEL_NAME_OBJECTLIST, true);
                    ShowPanel(S3102Consts.PANEL_NAME_CALLINO, false);
                    ShowPanel(S3102Consts.PANEL_NAME_PLAYLIST, false);
                    ShowPanel(S3102Consts.PANEL_NAME_PLAYBOX, false);
                    ShowPanel(S3102Consts.PANEL_NAME_MEMO, false);
                    ShowPanel(S3102Consts.PANEL_NAME_BOOKMARK, true);
                    ShowPanel(S3102Consts.PANEL_NAME_SCORE, false);
                    ShowPanel(S3102Consts.PANEL_NAME_SCOREDETAIL, false);
                    ShowPanel(S3102Consts.PANEL_NAME_CONVERSATIONINFO, false);
                    break;
            }
        }

        private void InitUserSettings()
        {
            try
            {
                var settingInfo = mListSettingInfos.FirstOrDefault(s => s.ParamID == S3102Consts.USER_PARAM_PAGESIZE);
                if (settingInfo != null)
                {
                    mPageSize = Convert.ToInt32(settingInfo.StringValue);
                }
                settingInfo = mListSettingInfos.FirstOrDefault(s => s.ParamID == S3102Consts.USER_PARAM_MAXRECORDS);
                if (settingInfo != null)
                {
                    mMaxRecords = Convert.ToInt32(settingInfo.StringValue);
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        public void ReloadUserSettings()
        {
            LoadUserSettingInfos();
        }

        public void ReloadRecordDataColumns()
        {
            InitRecordDataColumns();
            CreateRecordDataColumns();
        }

        public void ReloadScoreSheetList()
        {
            LoadUserScoreSheetList();
            LoadUserScoreResultList();
        }

        //写一个方法 看下选中的录音是否被申诉过了  true 表示已申诉  false表示未申诉
        private void Iscomplained()
        {
            IsCompained_ = false;
            string temp = mCurrentRecordInfoItem.SerialID.ToString();
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S3102Codes.IsComplainedRecord;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(temp);
                Service31021Client client = new Service31021Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31021"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.Data == null)
                {
                    ShowException(string.Format("Fail.\tListData is null"));
                    return;
                }
                if (webReturn.Data == "1")
                {
                    //被申诉过
                    IsCompained_ = true;
                }
                else
                {
                    //未被申诉过
                    IsCompained_ = false;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        //是否被分配成了任务
        private void IsTasked()
        {
            IsTasked_ = false;
            string temp = mCurrentRecordInfoItem.SerialID.ToString();
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S3102Codes.IsTaskedRecord;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(temp);
                Service31021Client client = new Service31021Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session),
                    WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31021"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.Data == null)
                {
                    ShowException(string.Format("Fail.\tListData is null"));
                    return;
                }
                if (webReturn.Data == "1")
                {
                    //被分配到任务
                    IsTasked_ = true;
                }
                else
                {
                    //未被分配到任务
                    IsTasked_ = false;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion

        #region ChangTheme

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
                    //ShowException("1" + ex.Message);
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
                    //ShowException("2" + ex.Message);
                }
            }

            //固定资源(有些资源包含转换器，命令等自定义类型，
            //这些资源不能通过url来动态加载，他只能固定的编译到程序集中
            try
            {
                string uri = string.Format("/UMPS3102;component/Themes/Default/UMPS3102/QMAvalonDock.xaml");
                ResourceDictionary resource = new ResourceDictionary();
                resource.Source = new Uri(uri, UriKind.RelativeOrAbsolute);
                Resources.MergedDictionaries.Add(resource);
            }
            catch (Exception ex)
            {
                //ShowException("3" + ex.Message);
            }
            try
            {
                string uri = string.Format("/UMPS3102;component/Themes/Default/UMPS3102/QMMainPageStatic.xaml");
                ResourceDictionary resource = new ResourceDictionary();
                resource.Source = new Uri(uri, UriKind.RelativeOrAbsolute);
                Resources.MergedDictionaries.Add(resource);
            }
            catch (Exception ex)
            {
                //ShowException("3" + ex.Message);
            }
        }

        #endregion

        #region ChangLanguage

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();

            CurrentApp.AppTitle = CurrentApp.GetLanguageInfo(string.Format("FO{0}", CurrentApp.ModuleID), "Query Management");
            //Operation
            for (int i = 0; i < ListOperations.Count; i++)
            {
                ListOperations[i].Display = CurrentApp.GetLanguageInfo(string.Format("FO{0}", ListOperations[i].ID),
                    ListOperations[i].ID.ToString());
            }
            CreateOptButtons();

            //RecordData
            for (int i = 0; i < mListCurrentRecordInfoItems.Count; i++)
            {
                RecordInfoItem item = mListCurrentRecordInfoItems[i];
                item.StrDirection = CurrentApp.GetLanguageInfo(string.Format("3102TIP001{0}", item.Direction),
                    item.Direction.ToString());
                if (item.IsScored == 2)
                {
                    double dValue;
                    if (double.TryParse(item.Score, out dValue))
                    {
                        item.StrIsScored = dValue.ToString("0.00");
                    }
                    else
                    {
                        item.StrIsScored = item.Score;
                    }
                }
                else
                {
                    item.StrIsScored = CurrentApp.GetLanguageInfo(string.Format("3102TIP001IsScored{0}", item.IsScored),
                        item.IsScored.ToString());
                }
                item.StrIsHaveBookMark = CurrentApp.GetLanguageInfo(string.Format("3102TIP002{0}", item.IsHaveBookMark),
                    item.IsHaveBookMark.ToString());
                item.StrIsHaveMemo = CurrentApp.GetLanguageInfo(string.Format("3102TIP002{0}", item.IsHaveMemo),
                    item.IsHaveMemo.ToString());
                item.StrIsHaveVoiceBookMark = CurrentApp.GetLanguageInfo(string.Format("3102TIP002{0}", item.IsHaveVoiceBookMark),
                    item.IsHaveVoiceBookMark.ToString());
                item.StrServiceAttitude = CurrentApp.GetLanguageInfo(string.Format("3102TIP009{0}", item.ServiceAttitude),
                        item.ServiceAttitude);
                item.StrProfessionalLevel = CurrentApp.GetLanguageInfo(string.Format("3102TIP009{0}", item.ProfessionalLevel),
                    item.ProfessionalLevel);
                item.StrRecordDurationExcept = CurrentApp.GetLanguageInfo(string.Format("3102TIP010{0}", item.RecordDurationExcept),
                    item.RecordDurationExcept);
                item.StrRepeatedCall = CurrentApp.GetLanguageInfo(string.Format("3102TIP011{0}", item.RepeatedCall),
                    item.RepeatedCall);
                item.StrCallPeak = CurrentApp.GetLanguageInfo(string.Format("3102TIP012{0}", item.CallPeak),
                    item.CallPeak);
            }

            //ScoreSheetItem
            for (int i = 0; i < mListScoreSheetItems.Count; i++)
            {
                var item = mListScoreSheetItems[i];
                if (item.Flag == 0)
                {
                    item.StrScore = CurrentApp.GetLanguageInfo("3102TIP001IsScored0", "No Score");
                }
                else
                {
                    item.StrScore = item.Score.ToString();
                }
            }

            CreateRecordDataColumns();
            CreateRecordPlayColumns();

            //Other
            ExpBasicOpt.Header = CurrentApp.GetLanguageInfo("31021000", "Basic Operations");
            ExpOtherPos.Header = CurrentApp.GetLanguageInfo("31021001", "Other Position");
            SetPageState();

            //ToolButton
            CreatePageButtons();
            CreatePlayButtons();

            //PlayDateType
            for (int i = 0; i < mListPlayDateTypeItems.Count; i++)
            {
                PlayDateTypeItem item = mListPlayDateTypeItems[i];
                mListPlayDateTypeItems[i].Display =
                    CurrentApp.GetLanguageInfo(string.Format("3102TIP006{0}", item.Type), item.Type.ToString());
            }

            #region Panel

            var panelItem = mListPanels.FirstOrDefault(p => p.Name == "RecordList");
            if (panelItem != null)
            {
                panelItem.Title = CurrentApp.GetLanguageInfo("3102P001", "Record List");
            }
            panelItem = mListPanels.FirstOrDefault(p => p.Name == "PlayList");
            if (panelItem != null)
            {
                panelItem.Title = CurrentApp.GetLanguageInfo("3102P002", "Play List");
            }
            panelItem = mListPanels.FirstOrDefault(p => p.Name == S3102Consts.PANEL_NAME_OBJECTLIST);
            if (panelItem != null)
            {
                panelItem.Title = CurrentApp.GetLanguageInfo("3102P003", "Manage Object");
            }
            panelItem = mListPanels.FirstOrDefault(p => p.Name == "PlayBox");
            if (panelItem != null)
            {
                panelItem.Title = CurrentApp.GetLanguageInfo("3102P004", "Play Box");
            }
            panelItem = mListPanels.FirstOrDefault(p => p.Name == "CallInfo");
            if (panelItem != null)
            {
                panelItem.Title = CurrentApp.GetLanguageInfo("3102P005", "Call Information");
            }
            panelItem = mListPanels.FirstOrDefault(p => p.Name == "Memo");
            if (panelItem != null)
            {
                panelItem.Title = CurrentApp.GetLanguageInfo("3102P006", "Memo");
            }
            panelItem = mListPanels.FirstOrDefault(p => p.Name == "Bookmark");
            if (panelItem != null)
            {
                panelItem.Title = CurrentApp.GetLanguageInfo("3102P007", "Bookmark");
            }
            panelItem = mListPanels.FirstOrDefault(p => p.Name == S3102Consts.PANEL_NAME_SCORE);
            if (panelItem != null)
            {
                panelItem.Title = CurrentApp.GetLanguageInfo("3102P008", "Score");
            }
            panelItem = mListPanels.FirstOrDefault(p => p.Name == S3102Consts.PANEL_NAME_SCOREDETAIL);
            if (panelItem != null)
            {
                panelItem.Title = CurrentApp.GetLanguageInfo("3102P009", "ScoreDetail");
            }
            panelItem = mListPanels.FirstOrDefault(p => p.Name == "PanelConversationInfo");
            if (panelItem != null)
            {
                panelItem.Title = CurrentApp.GetLanguageInfo("3102P010", "ConversationInfo");
            }

            var panel = GetPanleByContentID("PanelRecordList");
            if (panel != null)
            {
                panel.Title = CurrentApp.GetLanguageInfo("3102P001", "Record List");
            }
            panel = GetPanleByContentID("PanelPlayList");
            if (panel != null)
            {
                panel.Title = CurrentApp.GetLanguageInfo("3102P002", "Record List");
            }
            panel = GetPanleByContentID("PanelObjectBox");
            if (panel != null)
            {
                panel.Title = CurrentApp.GetLanguageInfo("3102P003", "Manage Object");
            }
            panel = GetPanleByContentID("PanelPlayBox");
            if (panel != null)
            {
                panel.Title = CurrentApp.GetLanguageInfo("3102P004", "Plaly Box");
            }
            panel = GetPanleByContentID("PanelCallInfo");
            if (panel != null)
            {
                panel.Title = CurrentApp.GetLanguageInfo("3102P005", "Call Information");
            }
            panel = GetPanleByContentID("PanelMemo");
            if (panel != null)
            {
                panel.Title = CurrentApp.GetLanguageInfo("3102P006", "Memo");
            }
            panel = GetPanleByContentID("PanelBookmark");
            if (panel != null)
            {
                panel.Title = CurrentApp.GetLanguageInfo("3102P007", "Bookmark");
            }
            panel = GetPanleByContentID("PanelScore");
            if (panel != null)
            {
                panel.Title = CurrentApp.GetLanguageInfo("3102P008", "Score");
            }
            panel = GetPanleByContentID("PanelConversationInfo");
            if (panel != null)
            {
                panel.Title = CurrentApp.GetLanguageInfo("3102P010", "ConversationInfo");
            }

            #endregion

            //ToolbarButton
            CreateToolBarButtons();

            //Popup
            PopupPanel.ChangeLanguage();
            PopupPanelPassword.ChangeLanguage();

            //Sub UserControl
            if (mUCPlayBox != null)
            {
                mUCPlayBox.ChangeLanguage();
            }
            if (mUCRecordMemo != null)
            {
                mUCRecordMemo.ChangeLanguage();
            }
            if (mUCRecordBookmark != null)
            {
                mUCRecordBookmark.ChangeLanguage();
            }
            if (ucConversationInfo != null)
            {
                ucConversationInfo.ChangeLanguage();
            }
        }

        #endregion

        protected override void OnAppEvent(WebRequest webRequest)
        {
            base.OnAppEvent(webRequest);
            var code = webRequest.Code;
            switch (code)
            {
                case (int)RequestCode.ACPageHeadLeftPanel:
                    if (GridLeft.Width.Value == 0)
                    {
                        GridLeft.Width = new GridLength(200);
                    }
                    else
                    {
                        GridLeft.Width = new GridLength(0);
                    }
                    break;
            }
        }
    }
}
