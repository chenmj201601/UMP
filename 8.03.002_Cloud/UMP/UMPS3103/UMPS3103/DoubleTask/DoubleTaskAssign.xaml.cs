using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common31031;
using VoiceCyber.UMP.Communications;
using UMPS3103.Wcf31031;
using UMPS3103.Models;
using VoiceCyber.NAudio.Controls;
using UMPS3103.Codes;
using VoiceCyber.UMP.CommonService03;
using System.IO;
using VoiceCyber.Wpf.AvalonDock.Layout;
using VoiceCyber.Wpf.AvalonDock.Layout.Serialization;
using System.Windows.Controls.Primitives;
using UMPS3103.Wcf11012;
using System.Timers;
using VoiceCyber.UMP.Controls;
using UMPS3103.Wcf31021;

namespace UMPS3103.DoubleTask
{
    /// <summary>
    /// DoubleTaskAssign.xaml 的交互逻辑
    /// </summary>
    public partial class DoubleTaskAssign
    {
        private ObservableCollection<RecordInfoItem> mListCurrentRecordInfoItems;
        private List<RecordInfoItem> mListAllRecordInfoItems;
        private RecordInfoItem mCurrentRecordInfoItem;
        private RecordPlayItem mCurrentRecordPlayItem;
        private ObservableCollection<CallInfoPropertyItem> mListCallInfoPropertyItems;
        public static ObservableCollection<UserTasksInfoShow> ListCurrentUserTasks = new ObservableCollection<UserTasksInfoShow>();
        private ObservableCollection<RecordPlayItem> mListRecordPlayItems;
        private List<PanelItem> mListPanels;

        private Service03Helper mService03Helper;
        private List<SettingInfo> mListSettingInfos;
        private List<SftpServerInfo> mListSftpServers;
        private List<DownloadParamInfo> mListDownloadParams;
        private List<RecordEncryptInfo> mListRecordEncryptInfos;
        private BackgroundWorker mWorker;
        private UCPlayBox mUCPlayBox;
        public event RoutedPropertyChangedEventHandler<PlayerEventArgs> PlayerEvent;

        private int mCircleMode;
        private int mPageIndex;
        private int mPageCount;
        private int mPageSize;
        private int mRecordTotal;
        private int mMaxRecords;
        private bool mIsQueryContinue;

        /// <summary>
        /// 查询评分不允许被查出的处理sql条件
        /// </summary>
        string NoQueryScoreStr = string.Empty;
        /// <summary>
        /// 查詢評分不允許被查出 false 不允許
        /// </summary>
        bool ShowQuery = false;

        private List<GridViewOrder> GviewList;
        private GridViewColumn GView;
        private GridViewOrder GVitem;


        public DoubleTaskAssign()
        {
            InitializeComponent();
            mListCurrentRecordInfoItems=new ObservableCollection<RecordInfoItem>();
            mListAllRecordInfoItems=new List<RecordInfoItem>();
            mCurrentRecordInfoItem = new RecordInfoItem();
            mListCallInfoPropertyItems = new ObservableCollection<CallInfoPropertyItem>();
            mListPanels = new List<PanelItem>();
            mListSftpServers = S3103App.mListSftpServers;
            mService03Helper = S3103App.mService03Helper;
            mListRecordEncryptInfos = S3103App.mListRecordEncryptInfos;
            mListDownloadParams = S3103App.mListDownloadParams;
            mListSettingInfos = new List<SettingInfo>();
            mListRecordPlayItems = new ObservableCollection<RecordPlayItem>();
            GviewList = new List<GridViewOrder>();

            mCircleMode = 0;
            mPageIndex = 0;
            mPageCount = 0;
            mPageSize = 100;
            mRecordTotal = 0;
            mMaxRecords = 100000;
            mIsQueryContinue = false;

            TxtPage.KeyUp += TxtPage_KeyUp;
            LvRecordData.ItemsSource = mListCurrentRecordInfoItems;
            //当前选中创建操作按钮 更新录音信息
            LvRecordData.SelectionChanged += LvRecordData_SelectionChanged;
            //播放录音
            //LvRecordData.MouseDoubleClick += LvRecordData_MouseDoubleClick;
            //VoicePlayer.PlayerEvent += VoicePlayer_PlayerEvent; 
            ListBoxCallInfo.ItemsSource = mListCallInfoPropertyItems;
            LvDoubleTaskData.ItemsSource = ListCurrentUserTasks;
        }

        protected override void Init() //用了init()方法之后，不能在用this.load的方法了
        {
            try
            {
                PageName = "UMP ReCheckTask";
                StylePath = "UMPS3103/MainPageStyle.xaml";
                base.Init();
                if (CurrentApp != null)
                {
                    CurrentApp.SendLoadedMessage();
                }

                SetBusy(true, CurrentApp.GetMessageLanguageInfo("001", string.Format("Loading...")));
                mWorker = new BackgroundWorker();
                mWorker.DoWork += (s, de) =>
                {
                };
                mWorker.RunWorkerCompleted += (s, re) =>
                {
                    mWorker.Dispose();
                    SetBusy(false, "");

                    InitOperationButton();
                    InitReCheckTaskColumns();
                    InitTaskDetailColumns();
                    InitPanels();
                    CreateToolBarButtons();
                    CreateOptButtons();
                    //创建查询记录页面
                    CreatePageButtons();
                    CreateCallInfoItems();
                    //LoadLayout();
                    InitTasks();
                    ChangeLanguage();
                    ChangeTheme();
                };
                mWorker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
               ShowException(ex.Message);
            }
        }
        

        #region 按鈕&頁面跳轉

        /// <summary>
        /// 根据权限生成操作按钮
        /// </summary>
        private void CreateOptButtons()
        {
            PanelBasicOpts.Children.Clear();
            Button btn;
            OperationInfo opt;
            if (S3103App.ListOperationInfos.Where(p => p.ID == S3103Consts.OPT_DoubleTask).ToList().Count > 0)
            {
                btn = new Button();
                opt = new OperationInfo();
                opt.Display = CurrentApp.GetLanguageInfo("FO3103019", "Query");
                opt.ID = S3103Consts.OPT_DoubleTask;
                opt.Icon = "Images/search.png";
                btn.Click += BasicOpt_Click;
                btn.DataContext = opt;
                btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                PanelBasicOpts.Children.Add(btn);
            }


            if (S3103App.ListOperationInfos.Where(p => p.ID == S3103Consts.OPT_DoubleTask).ToList().Count > 0)
            {
                btn = new Button();
                opt = new OperationInfo();
                opt.Display = CurrentApp.GetLanguageInfo("3103T00149", "Save To Recheck Task");
                opt.ID = 1;
                opt.Icon = "Images/save.png";
                btn.Click += BasicOpt_Click;
                btn.DataContext = opt;
                btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                PanelBasicOpts.Children.Add(btn);
            }

            if (S3103App.ListOperationInfos.Where(p => p.ID == S3103Consts.OPT_DoubleTask).ToList().Count > 0)
            {
                btn = new Button();
                opt = new OperationInfo();
                opt.Display = CurrentApp.GetLanguageInfo("3103T00150", "Add To Recheck Task");
                opt.ID = 2;
                opt.Icon = "Images/add.png";
                btn.Click += BasicOpt_Click;
                btn.DataContext = opt;
                btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                PanelBasicOpts.Children.Add(btn);
            }
        }

        void BasicOpt_Click(object sender, RoutedEventArgs e)
        {
            var btn = e.Source as Button;
            if (btn != null)
            {
                var optItem = btn.DataContext as OperationInfo;
                if (optItem != null)
                {
                    switch (optItem.ID)
                    {
                        case S3103Consts.OPT_DoubleTask:
                            PopupPanel.Title = CurrentApp.GetLanguageInfo("FO3103019", "Query Info");
                            QueryReCheck queryRecordInTask = new QueryReCheck();
                            queryRecordInTask.CurrentApp = CurrentApp;
                            queryRecordInTask.ParentPage = this;
                            PopupPanel.Content = queryRecordInTask;
                            PopupPanel.IsOpen = true;
                            break;
                        case 1:
                            SaveToTask();
                            break;
                        case 2:
                            {
                                SetBusy(true,"");
                                AddToReCheckTask();
                                SetBusy(false,"");
                            }
                            break;
                    }
                }
            }
        }


        /// <summary>
        /// 获取跳轉任務跟蹤、初检权限
        /// </summary>
        private void InitOperationButton()
        {
            PanelOherOpts.Children.Clear();
            Button btn;
            if (S3103App.ListOperationInfos.Where(p => p.ID == S3103Consts.OPT_TASKTRACK).Count() > 0) //任務跟蹤權限
            {
                btn = new Button();
                btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                var opt = new OperationInfo();
                opt.Display = CurrentApp.GetLanguageInfo("3103T00103", "Go To Task Track"); 
                opt.Icon = "Images/controltarget.png";
                btn.DataContext = opt;
                btn.Click += btn2TaskTrack_Click;
                btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                PanelOherOpts.Children.Add(btn);
            }
            if (S3103App.ListOperationInfos.Where(p => p.ID == S3103Consts.OPT_TASKASSIGN).Count() > 0) //初檢權限
            {
                btn = new Button();
                btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                var opt = new OperationInfo();
                opt.Display = CurrentApp.GetLanguageInfo("3103T00107", "Move To Task Assign");
                opt.Icon = "Images/controltarget.png";
                btn.DataContext = opt;
                btn.Click += btn2TaskAssign_Click;
                btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                PanelOherOpts.Children.Add(btn);
            }
        }

        private void btn2TaskTrack_Click(object sender, RoutedEventArgs e)
        {
            S3103App.NativePageFlag = false;
            var temp = CurrentApp as S3103App;
            TaskTrack taskTrackView = new TaskTrack();
            CurrentApp.CurrentView = taskTrackView;
            taskTrackView.PageName = "TaskTrack";
            if (temp != null)
            {
                temp.InitMainView(taskTrackView);
                //temp.InitMainView(0);
            }
        }
        private void btn2TaskAssign_Click(object sender, RoutedEventArgs e)
        {
            S3103App.NativePageFlag = false;
            var temp = CurrentApp as S3103App;
            TaskAssign taskAssignView = new TaskAssign();
            CurrentApp.CurrentView = taskAssignView;
            taskAssignView.PageName = "TaskAssign";
            if (temp != null)
            {
                temp.InitMainView(taskAssignView);
                //temp.InitMainView(0);
            }
        }

        #endregion

        private void SaveToTask()
        {
            if (mListCurrentRecordInfoItems.Where(p => p.IsCheck == true).Count() == 0)
            {
                MessageBox.Show(CurrentApp.GetLanguageInfo("3103T00121", "Please select one record at least."), CurrentApp.AppName);
                return;
            }

            PopupPanel.Title = CurrentApp.GetLanguageInfo("3103T00117", "New Task");
            ReCheckToQA Task2QA = new ReCheckToQA();
            Task2QA.CurrentApp = CurrentApp;
            Task2QA.PageParent = this;
            PopupPanel.Content = Task2QA;
            List<RecordInfoItem> Item=new List<RecordInfoItem>();
            RecordInfoItem tempItem;
            bool IsAdd = true;
            for (int i = 0; i < mListCurrentRecordInfoItems.Count; i++)
            {
                if (mListCurrentRecordInfoItems[i].IsCheck == true)
                {
                    tempItem = new RecordInfoItem();
                    tempItem.TaskUserID = mListCurrentRecordInfoItems[i].TaskUserID;
                    tempItem.TaskUserName = mListCurrentRecordInfoItems[i].TaskUserName;
                    foreach (RecordInfoItem recordInfoItem in Item)
                    {
                        if (tempItem.TaskUserID == recordInfoItem.TaskUserID) IsAdd = false;
                    }
                    if (IsAdd)
                    {
                        Item.Add(tempItem);
                    }
                }
            }
            Task2QA.ReCheckRecordInfoItem = Item;
            PopupPanel.IsOpen = true;
        }

        #region 複檢查詢

        void InitReCheckTaskColumns()
        {
            bool hadColumnsSetting = false;//该用户是否保存有列信息

            string[] lans = "3103T00058,3103T00025,3103T00152,3103T00059,3103T00060,3103T00061,3103T00062,3103T00063,3103T00064,3103T00065,3103T00066,3103T00067,3103T00068,3103T00069".Split(',');
            string[] cols = "RowID,SerialID,TaskUserName,StartRecordTime,StopRecordTime,VoiceID,ChannelID,VoiceIP,Extension,Agent,StrDuration,DirShow,DecCallerID,DecCalledID".Split(',');
            int[] colwidths = { 50, 140, 80,100, 140, 100, 150, 100, 100, 150, 100, 150, 120, 100 };
            string[] widths = { "" };
            List<string> columnsList = WriteLog.ReadColumnsXml(string.Format("{0}ss", CurrentApp.Session.UserID.ToString()));
            if (columnsList != null)
            {
                hadColumnsSetting = true;
                lans = columnsList[0].Split(',');
                cols = columnsList[1].Split(',');
                widths = columnsList[2].Split(',');
            }
            GviewList.Clear();

            GridView ColumnGridView = new GridView();
            GridViewColumn gvc;

            #region//全选以及播放
            gvc = new GridViewColumn();
            CheckBox chkAllCheck = new CheckBox();
            chkAllCheck.Content = CurrentApp.GetLanguageInfo("3103T00085", "All Select");
            chkAllCheck.Click += chkAllCheck_Click;
            gvc.Header = (object)chkAllCheck;
            gvc.Width = 60;
            DataTemplate CellTemplate = (DataTemplate)Resources["CheckRecord"];
            if (CellTemplate != null)
            {
                gvc.CellTemplate = CellTemplate;
            }
            else
            {
                gvc.DisplayMemberBinding = new Binding("CheckRecord");
            }
            ColumnGridView.Columns.Add(gvc);

            gvc = new GridViewColumn();
            gvc.Header = "播放";

            #endregion

            for (int i = 0; i < 14; i++)
            {
                GVitem = new GridViewOrder();
                gvc = new GridViewColumn();
                gvc.Header = CurrentApp.GetLanguageInfo(lans[i], cols[i]);
                gvc.Width = colwidths[i];
                GVitem.Header = CurrentApp.GetLanguageInfo(lans[i], cols[i]);
                GVitem.cols = cols[i];
                GVitem.Lans = lans[i];
                if (!hadColumnsSetting)
                {
                    GVitem.widths = gvc.Width = colwidths[i];
                }
                else
                {
                    GVitem.widths = gvc.Width = Convert.ToDouble(widths[i]);
                }
                gvc.DisplayMemberBinding = new Binding(cols[i]);

                GviewList.Add(GVitem);
                ColumnGridView.Columns.Add(gvc);
            }
            LvRecordData.View = ColumnGridView;
        }
        void chkAllCheck_Click(object sender, RoutedEventArgs e)
        {
            CheckBox chkAllCheck = sender as CheckBox;
            if (chkAllCheck.IsChecked == true)
            {
                foreach (RecordInfoItem r in mListCurrentRecordInfoItems)
                {
                    r.IsCheck = true;
                }
            }
            else
            {
                foreach (RecordInfoItem r in mListCurrentRecordInfoItems)
                {
                    r.IsCheck = false;
                }
            }
        }

        private string ConditionStr(string strConditionString, long currentRowID, string tableName)
        {
            string strSql = string.Empty;
            try
            {
                //申訴過程中、申訴后不被允許查出
                bool ShowAppeal = false;
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)RequestCode.WSGetGlobalParamList;
                webRequest.ListData.Add("31");
                webRequest.ListData.Add("0");
                webRequest.ListData.Add("310103");
                Service11012Client client = new Service11012Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service11012"));
                //WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                   ShowException(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return string.Empty;
                }
                
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<GlobalParamInfo>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                       ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return string.Empty;
                    }
                    GlobalParamInfo GlobalParamInfo = optReturn.Data as GlobalParamInfo;
                    if (GlobalParamInfo.ParamID == 31010301)
                    {
                        ShowQuery = GlobalParamInfo.ParamValue == "310103010" ? true : false;//false 不允許
                        //ShowQuery = true;
                    }
                    if (GlobalParamInfo.ParamID == 31010302)
                    {
                        ShowAppeal = GlobalParamInfo.ParamValue == "310103020" ? true : false;//false 不允許
                    }
                }
                if (!ShowQuery)
                {
                    strConditionString += string.Format(" AND T308.C010 IN (3, 4,7) AND T308.C009 = 'Y'");
                }
                if (ShowQuery)
                {
                    strConditionString += string.Format(" AND T308.C010 IN (1, 2,3, 4,7) AND T308.C009 = 'Y'");
                }
                if (!ShowAppeal)
                {
                    strConditionString += string.Format(" AND T308.C014 = 0");
                }
                switch (CurrentApp.Session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT * FROM(SELECT TOP {2} T308.c005 TaskUserID,T11.C003 UserAccount,T308.C003 TemplateID,T308.C004 ISSCORE,T21.*" +
                                               ",CASE WHEN EXISTS(SELECT T322.* FROM T_31_022_{0} T322 WHERE T322.c002=T308.C002 AND T322.C010 in (3,4))THEN '1' ELSE '0' END TaskType1" +
                                               ",CASE	WHEN EXISTS (SELECT T322.* FROM T_31_022_{0} T322 WHERE T322.c002 = T308.C002 AND T322.C010 IN (1,2) ) THEN '1' ELSE '0' END TaskType2" +
                                               "  FROM T_31_008_{0} T308 RIGHT JOIN {1} T21 ON T21.C002 = T308.C002 LEFT JOIN T_11_005_{0} T11 ON T11.C001 = T308.C005 "+
                                                "{3} AND T21.C001 > {4} AND T21.C014<2 ORDER BY T21.C001,T21.C005,T21.C039)T WHERE T.TaskType1='0' AND T.TaskType2 = '1'",
                                               CurrentApp.Session.RentInfo.Token, tableName, mPageSize, strConditionString, currentRowID);
                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM(SELECT T308.c005 TaskUserID,T11.C003 UserAccount,T308.C003 TemplateID,T308.C004 ISSCORE,T21.*" +
                                               ",CASE WHEN EXISTS(SELECT T322.* FROM T_31_022_{0} T322 WHERE T322.c002=T308.C002 AND T322.C010 in (3,4))THEN '1' ELSE '0' END TaskType1" +
                                               ",CASE	WHEN EXISTS (SELECT T322.* FROM T_31_022_{0} T322 WHERE T322.c002 = T308.C002 AND T322.C010 IN (1,2) ) THEN '1' ELSE '0' END TaskType2" +
                                               " FROM T_31_008_{0} T308 RIGHT JOIN {1} T21 ON T21.C002 = T308.C002 LEFT JOIN T_11_005_{0} T11 ON T11.C001 = T308.C005 " +
                                               "{3} AND T21.C001 > {4} AND T21.C014<2 AND ROWNUM<={2} ORDER BY T21.C001,T21.C005,T21.C039)T WHERE T.TaskType1='0' AND T.TaskType2 = '1'",
                                               CurrentApp.Session.RentInfo.Token, tableName, mPageSize, strConditionString, currentRowID);
                        break;
                    default:
                       ShowException(string.Format("DBType invalid"));
                        return string.Empty;
                }



            }
            catch (Exception ex)
            {
               ShowException(ex.Message);
                return string.Empty;
            }
            return strSql;
        }

        public void QueryDoubleTaskRecord(string strCondition, List<DateTimeSpliteAsDay> lstDateTimeSplitAsDay)
        {
            mRecordTotal = 0;
            mPageIndex = 0;
            mPageCount = 0;
            mPageSize = S3103Consts.USER_PARAM_PAGESIZE;
            mMaxRecords = S3103Consts.USER_PARAM_MAXRECORDS;
            mIsQueryContinue = true;
            //CreateOptButtons();
            mListAllRecordInfoItems.Clear();
            mListCurrentRecordInfoItems.Clear();

            #region 不允许查出查询评分的SQL条件
            DateTimeSpliteAsDay datetime = lstDateTimeSplitAsDay.FirstOrDefault();
            if (datetime == null)
            {
               ShowException(string.Format("DateTimeSpliteAsDay  is null"));
                return;
            }
            switch (CurrentApp.Session.DBType)
            {
                case 2:
                    NoQueryScoreStr = string.Format("T21.C005 >= '{0}' AND T21.C005 <= '{1}'",
                        datetime.StartDayTime.ToString("yyyy/MM/dd HH:mm:ss"), datetime.StopDayTime.ToString("yyyy/MM/dd HH:mm:ss"));
                    break;
                case 3:
                    NoQueryScoreStr = string.Format("T21.C005 >=TO_DATE ('{0}','YYYY-MM-DD HH24:MI:SS') AND T21.C005 <=TO_DATE( '{1}','YYYY-MM-DD HH24:MI:SS')",
                        datetime.StartDayTime.ToString("yyyy/MM/dd HH:mm:ss"), datetime.StopDayTime.ToString("yyyy/MM/dd HH:mm:ss"));
                    break;
                default:
                   ShowException(string.Format("DBType invalid"));
                    return ;
            }

            #endregion

            try
            {
                SetBusy(true,"");
                mWorker = new BackgroundWorker();
                mWorker.DoWork += (s, de) =>
                {
                    var tableInfo =
                    CurrentApp.Session.ListPartitionTables.FirstOrDefault(
                    t => t.TableName == ConstValue.TABLE_NAME_RECORD && t.PartType == TablePartType.DatetimeRange);
                    if (tableInfo == null)
                    {
                        tableInfo = CurrentApp.Session.ListPartitionTables.FirstOrDefault(
                            t => t.TableName == ConstValue.TABLE_NAME_RECORD && t.PartType == VoiceCyber.UMP.Common.TablePartType.DatetimeRange);
                        if (tableInfo == null)
                        {
                            QueryRecord(strCondition, 0, string.Format("{0}_{1}", ConstValue.TABLE_NAME_RECORD, CurrentApp.Session.RentInfo.Token));
                        }
                        else
                        {
                            ////按录音服务器查询
                            QueryRecord(strCondition, 0, string.Format("{0}_{1}", ConstValue.TABLE_NAME_RECORD, CurrentApp.Session.RentInfo.Token));
                        }
                    }
                    else
                    {
                        DateTimeSpliteAsDay datetimespliteasday = lstDateTimeSplitAsDay.FirstOrDefault();
                        if (datetimespliteasday == null)
                        {
                           ShowException(string.Format("DateTimeSpliteAsDay  is null"));
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
                            string strTableName = string.Format("{0}_{1}_{2}", ConstValue.TABLE_NAME_RECORD,
                                CurrentApp.Session.RentInfo.Token, partTable);
                            QueryRecord(strCondition, 0, strTableName);
                        }
                    }
                };
                mWorker.RunWorkerCompleted += (s, re) =>
                {
                    mWorker.Dispose();
                    SetBusy(false,"");
                    mIsQueryContinue = false;
                    //CreateOptButtons();
                    //InitBasicOperations();
                    SetPageState();
                };
                mWorker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
               ShowException(ex.Message);
            }

            #region 写操作日志
            CurrentApp.WriteOperationLog(S3103Consts.OPT_TASKQUERY.ToString(), ConstValue.OPT_RESULT_SUCCESS, "");
            #endregion
        }

        private void QueryRecord(string strConditionString, long currentRowID,string tableName)
        {
            try
            {
                if (!mIsQueryContinue) { return; }
                string strSql = string.Empty;
                strSql=ConditionStr(strConditionString, currentRowID, tableName);
                if (string.IsNullOrWhiteSpace(strSql))
                {
                    return;
                }

                CurrentApp.WriteLog("DoubleTaskQueryRecord", string.Format("{0}", strSql));
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3103Codes.GetRecheckRecordData;
                webRequest.ListData.Add(strSql);
                webRequest.ListData.Add(tableName);
                if(!ShowQuery)
                {
                    webRequest.ListData.Add(NoQueryScoreStr);
                }
                else
                {
                    webRequest.ListData.Add(string.Empty);
                }
                //Service31031Client client = new Service31031Client();
                Service31031Client client = new Service31031Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31031"));
                //WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.UMPTaskOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    if (webReturn.Message != S3103Consts.Err_TableNotExit)
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
                    RecordInfoItem item = new RecordInfoItem(recordInfo);
                    item.DirShow = recordInfo.Direction == 0 ? CurrentApp.GetLanguageInfo("3103T00090", "Dial Out") : CurrentApp.GetLanguageInfo("3103T00089", "Dial In");
                    int total = mRecordTotal + 1;
                    if (total > mMaxRecords)
                    {
                       ShowException(
                          string.Format("Larger than allowed max records, some record can't be displayed"));
                        return;
                    }
                    mRecordTotal = total;
                    item.RowNumber = mRecordTotal;
                    item.RowID = mRecordTotal;
                    item.StrDirection = CurrentApp.GetLanguageInfo(string.Format("3102TIP001{0}", item.Direction),
                        item.Direction.ToString());
                    item.Background = GetRecordBackground(item);
                    mListAllRecordInfoItems.Add(item);
                    if (mRecordTotal < mPageSize+1)
                    {
                        AddNewRecord(item);
                    }
                    currentRowID = recordInfo.RowID;
                    SetPageState();
                }
                QueryRecord( strConditionString, currentRowID, tableName);
            }
            catch (Exception ex)
            {
               ShowException(ex.Message);
                #region 写操作日志
                CurrentApp.WriteOperationLog(S3103Consts.OPT_TASKASSIGNQUERY.ToString(), ConstValue.OPT_RESULT_FAIL, "");
                #endregion
            }
        }

        private Brush GetRecordBackground(RecordInfoItem recordInfoItem)
        {
            int rowNumber = recordInfoItem.RowNumber;
            if (rowNumber % 2 == 0)
            {
                return Brushes.LightGray;
            }
            return Brushes.Transparent;
        }
        
        private void AddNewRecord(RecordInfoItem recordInfoItem)
        {
            Dispatcher.Invoke(new Action(() => mListCurrentRecordInfoItems.Add(recordInfoItem)));
        }
        #endregion

        #region 翻頁
        
        private void CreatePageButtons()
        {
            try
            {
                List<ToolButtonItem> listBtns = new List<ToolButtonItem>();
                ToolButtonItem item = new ToolButtonItem();
                item.Name = "TB" + "StopQueryMark";
                item.Display = CurrentApp.GetLanguageInfo("3103T00080", "Stop Query");
                item.Tip = CurrentApp.GetLanguageInfo("3103T00080", "Stop Query");
                item.Icon = "Images/stop.png";
                listBtns.Add(item);

                item = new ToolButtonItem();
                item.Name = "TB" + "FirstPage";
                item.Display = CurrentApp.GetLanguageInfo("3103T00081", "First Page");
                item.Tip = CurrentApp.GetLanguageInfo("3103T00081", "First Page");
                item.Icon = "Images/first.ico";
                listBtns.Add(item);
                item = new ToolButtonItem();
                item.Name = "TB" + "PrePage";
                item.Display = CurrentApp.GetLanguageInfo("3103T00082", "Pre Page");
                item.Tip = CurrentApp.GetLanguageInfo("3103T00082", "Pre Page");
                item.Icon = "Images/pre.ico";
                listBtns.Add(item);
                item = new ToolButtonItem();
                item.Name = "TB" + "NextPage";
                item.Display = CurrentApp.GetLanguageInfo("3103T00083", "Next Page");
                item.Tip = CurrentApp.GetLanguageInfo("3103T00083", "Next Page");
                item.Icon = "Images/next.ico";
                listBtns.Add(item);
                item = new ToolButtonItem();
                item.Name = "TB" + "LastPage";
                item.Display = CurrentApp.GetLanguageInfo("3103T00084", "Last Page");
                item.Tip = CurrentApp.GetLanguageInfo("3103T00084", "Last Page");
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

        void PageButton_Click(object sender, RoutedEventArgs e)
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
               ShowException(ex.Message);
            }
        }
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
                string temp = CurrentApp.GetLanguageInfo("3103T00097", "Records");
                string strPageInfo = string.Format("{0}/{1} {2} {3}", mPageIndex + 1, mPageCount, mRecordTotal, temp);
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

        void TxtPage_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter && mListCurrentRecordInfoItems.Count != 0)
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
               ShowException(ex.Message);
            }
        }
        #endregion

        #region 右側複檢任務欄
        /// Author           : Luoyihua
        ///  Created          : 2014-11-06 16:46:00
        /// <summary>
        /// 初始化任务列
        /// </summary>
        private void InitTaskDetailColumns()
        {
            string[] lans = "3103T00008,3103T00001,3103T00017,3103T00002,3103T00006,3103T00007,3103T00009,3103T00010,3103T00011,3103T00014,3103T00012,3103T00015,3103T00016,3103T00021".Split(',');
            string[] cols = "AssignUserFName,TaskName,IsFinish,TaskDesc,IsShare,AssignTime,AssignNum,DealLine,AlreadyScoreNum,ModifyTime,ModifyUserFName,BelongYear,BelongMonth,FinishTime".Split(',');
            int[] colwidths = { 100, 120, 75, 120, 75, 150, 75, 150, 100, 150, 120, 50, 50, 150 };
            GridView columngv = new GridView();
            for (int i = 0; i < 14; i++)
            {
                DataTemplate CellTemplate = (DataTemplate)Resources[cols[i]];
                SetColumnGridView(cols[i], ref columngv, lans[i], cols[i], CellTemplate, colwidths[i]);
            }
            LvDoubleTaskData.View = columngv;
        }

        /// Author           : Luoyihua
        ///  Created          : 2014-11-06 16:46:00
        /// <summary>
        /// 初始化GridView
        /// </summary>
        /// <param name="columnname">数据列名</param>
        /// <param name="ColumnGridView">GridView</param>
        /// <param name="langid">列名对应的语言包ID</param>
        /// <param name="diaplay">默认显示</param>
        /// <param name="datatemplate">列样式信息</param>
        /// <param name="width">列宽度</param>
        private void SetColumnGridView(string columnname, ref GridView ColumnGridView, string langid, string diaplay, DataTemplate datatemplate, int width)
        {
            GridViewColumn gvc = new GridViewColumn();
            gvc.Header = CurrentApp.GetLanguageInfo(langid, diaplay);
            gvc.Width = width;
            gvc.HeaderStringFormat = columnname;
            if (datatemplate != null)
            {
                gvc.CellTemplate = datatemplate;
            }
            else
                gvc.DisplayMemberBinding = new Binding(columnname);
            ColumnGridView.Columns.Add(gvc);
        }
        /// <summary>
        /// 加载当前用户分配的任务信息
        /// </summary>
        private void InitTasks()
        {
            try
            {
                //string dtstart = DateTime.Now.AddMonths(-1).ToUniversalTime().ToString();
                //string dtend = DateTime.Now.AddMonths(1).ToUniversalTime().ToString();
                ListCurrentUserTasks.Clear();
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S3103Codes.GetCurrentUserTasks;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                webRequest.ListData.Add("0");//0:我分配的 1:分配给我的 2:全部
                webRequest.ListData.Add("0");//0:未完成 1:已完成 2全部
                //webRequest.ListData.Add(dtstart); //开始时间
                //webRequest.ListData.Add(dtend); //截止时间
                webRequest.ListData.Add("3"); //初檢 1；複檢 3

                //Service31031Client client = new Service31031Client();
                Service31031Client client = new Service31031Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31031"));
                //WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.UMPTaskOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                   ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData.Count > 0)
                {
                    for (int i = 0; i < webReturn.ListData.Count; i++)
                    {
                        OperationReturn optReturn = XMLHelper.DeserializeObject<UserTasksInfoShow>(webReturn.ListData[i]);
                        if (!optReturn.Result)
                        {
                           ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                            return;
                        }
                        UserTasksInfoShow taskInfo = optReturn.Data as UserTasksInfoShow;
                        if (taskInfo != null && ListCurrentUserTasks.Where(p => p.TaskID == taskInfo.TaskID).Count() == 0)
                        {
                            taskInfo.IsFinish = taskInfo.IsFinish == "Y" ? CurrentApp.GetLanguageInfo("3103T00019", "Finished") : CurrentApp.GetLanguageInfo("3103T00020", "Unfinished");
                            taskInfo.IsShare = taskInfo.IsShare == "Y" ? CurrentApp.GetLanguageInfo("3103T00022", "Yes") : CurrentApp.GetLanguageInfo("3103T00023", "No");
                            ListCurrentUserTasks.Add(taskInfo);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
               ShowException(ex.Message);
            }
        }

        #endregion

        #region 複檢任務分配

        public void SaveRecheckTask2QA(UserTasksInfoShow userTaskInfoShow, List<CtrolQA> listctrolqa)
        {
            try
            {
                userTaskInfoShow.AlreadyScoreNum = 0;

                
                List<RecordInfoItem> lstData = new List<RecordInfoItem>();
                if (mListAllRecordInfoItems.Count == 0)
                {
                    ShowInformation(CurrentApp.GetLanguageInfo("3103T00127", "No  Record."));
                    return;
                }

                SetBusy(true,"");
                if (mListCurrentRecordInfoItems.Where(p => p.IsCheck == true).Count() > 0)
                {
                    long recordlength = 0;
                    Random random = new Random();
                    for (int i = 0; i < mListCurrentRecordInfoItems.Count; i++)
                    {
                        if (mListCurrentRecordInfoItems[i].IsCheck == true)
                        {
                            lstData.Add(mListCurrentRecordInfoItems[i]);
                            recordlength += mListCurrentRecordInfoItems[i].Duration;
                        }
                    }

                    if (listctrolqa.Count() > lstData.Count())
                    {
                        ShowInformation(CurrentApp.GetLanguageInfo("3103T00155", "The record is less than the number of quality inspector."));
                        SetBusy(false, "");
                        return;
                    }
                    //打乱顺序
                    List<RecordInfoItem> lstRecordTemp = new List<RecordInfoItem>();
                    foreach (RecordInfoItem item in lstData)
                    {
                        lstRecordTemp.Insert(random.Next(lstRecordTemp.Count), item);
                    }
                    lstData = lstRecordTemp;

                    //每个qa多少条
                    int avgNum = lstData.Count / listctrolqa.Count();

                    #region 如果分不完，提示
                    int leave = lstData.Count % listctrolqa.Count();
                    if (leave > 0)
                    {
                        string messageBoxText = CurrentApp.GetLanguageInfo("3103T00182", "The selected record distribution cannot be fully assigned.");

                        MessageBoxButton button = MessageBoxButton.OKCancel;
                        MessageBoxImage icon = MessageBoxImage.Warning;
                        //显示消息框
                        MessageBoxResult result = MessageBox.Show(messageBoxText, "3103", button, icon);
                        //处理消息框信息
                        switch (result)
                        {
                            case MessageBoxResult.Cancel:
                                SetBusy(false, "");
                                return;
                        }
                    }
                    #endregion

                    if (avgNum == 0)
                    {
                        SetBusy(false, "");
                        return;
                    }
                    string tempname = userTaskInfoShow.TaskName;
                    for (int i = 0; i < listctrolqa.Count(); i++)
                    {
                        bool isSelfDef = tempname.ToLower().Contains("qa" + DateTime.Now.ToString("yyyyMM")) ? false : true;//自定义任务名称
                        if (tempname.Length != 14) { isSelfDef = true; }
                        if (isSelfDef)//多个QA+自定义 
                        {
                            userTaskInfoShow.TaskName = "(" + listctrolqa[i].UserName + ")" + tempname;
                        }
                        if (!isSelfDef)//多个QA+默认
                        {
                            userTaskInfoShow.TaskName = "(" + listctrolqa[i].UserName + ")" + DateTime.Now.ToString("yyyyMMddHHmm");
                        }
                        lstRecordTemp = new List<RecordInfoItem>();
                        List<CtrolQA> lstCtrolQaTemp = new List<CtrolQA>();
                        lstCtrolQaTemp.Add(listctrolqa[i]);
                        for (int k = 0; k < avgNum; k++)
                        {
                            RecordInfoItem r = new RecordInfoItem();
                            r = lstData[i * avgNum + k];
                            recordlength += r.Duration;
                            lstRecordTemp.Add(r);
                        }

                        userTaskInfoShow.TaskAllRecordLength = recordlength;
                        userTaskInfoShow.AssignNum = lstRecordTemp.Count;
                        SaveTask(userTaskInfoShow, lstCtrolQaTemp, lstRecordTemp);
                    }
                }
                else
                {
                    MessageBox.Show(CurrentApp.GetLanguageInfo("3103T00115", "Please select one record at least."), CurrentApp.AppName);
                    return;
                }
                SetBusy(false,"");
            }
            catch (Exception ex)
            {
               ShowException(ex.Message);
            }
        }

        void SaveTask(UserTasksInfoShow userTaskInfoShow, List<CtrolQA> listctrolqa, List<RecordInfoItem> lstRecordInfoItem)
        {
            long taskID = 0;
            try
            {
                //提交任务
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3103Codes.SaveTask;
                OperationReturn optReturn = XMLHelper.SeriallizeObject(userTaskInfoShow);
                if (!optReturn.Result)
                {
                   ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                webRequest.Data = optReturn.Data.ToString();

                //Service31031Client client = new Service31031Client();
                Service31031Client client = new Service31031Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31031"));
                //WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.UMPTaskOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                   ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                }
                else
                {
                    taskID = Convert.ToInt64(webReturn.Data);
                    userTaskInfoShow.TaskID = taskID;


                    //提交任务QA T_31_021_00000
                    webReturn = new WebReturn();
                    webRequest = new WebRequest();
                    webRequest.Session = CurrentApp.Session;
                    webRequest.Code = (int)S3103Codes.SaveTaskQA;
                    webRequest.Data = taskID.ToString();
                    foreach (CtrolQA qa in listctrolqa)
                    {
                        optReturn = XMLHelper.SeriallizeObject(qa);
                        if (!optReturn.Result)
                        {
                           ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                            return;
                        }
                        webRequest.ListData.Add(optReturn.Data.ToString());
                    }

                    //client = new Service31031Client();
                    client = new Service31031Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31031"));
                    //WebHelper.SetServiceClient(client);
                    webReturn = client.UMPTaskOperation(webRequest);
                    client.Close();
                    if (!webReturn.Result)
                    {
                       ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                        return;
                    }

                    //提交任务录音T_31_022_00000
                    webReturn = new WebReturn();
                    webRequest = new WebRequest();
                    webRequest.Session = CurrentApp.Session;
                    webRequest.Code = (int)S3103Codes.SaveTaskRecord;
                    TaskInfoDetail taskinfodetail;
                    foreach (RecordInfoItem record in lstRecordInfoItem)
                    {
                        taskinfodetail = new TaskInfoDetail();
                        taskinfodetail.TaskID = taskID;
                        taskinfodetail.RecoredReference = record.SerialID;
                        taskinfodetail.IsLock = "N";
                        taskinfodetail.AllotType = 1;
                        taskinfodetail.FromTaskID = -1;
                        taskinfodetail.TaskType = "3";
                        taskinfodetail.StartRecordTime = Convert.ToDateTime(record.StartRecordTime).ToUniversalTime();//
                        taskinfodetail.Duration = record.Duration.ToString();
                        taskinfodetail.Extension = record.Extension;
                        taskinfodetail.CalledID = record.CalledID;
                        taskinfodetail.CallerID = record.CallerID;
                        taskinfodetail.Direction = record.Direction;

                        List<CtrolAgent> templst = new List<CtrolAgent>();
                        if (S3103App.GroupingWay.Contains("A"))
                        {
                            templst = S3103App.ListCtrolAgentInfos.Where(p => p.AgentName == record.ReAgent).ToList();
                        }
                        else if (S3103App.GroupingWay.Contains("R"))
                        {
                            templst = S3103App.ListCtrolRealityExtension.Where(p => p.AgentName == record.ReAgent).ToList();
                        }
                        else if (S3103App.GroupingWay.Contains("E"))
                        {
                            templst = S3103App.ListCtrolExtension.Where(p => p.AgentName == record.ReAgent).ToList();
                        }
                        var temp = S3103App.mListAuInfoItems.FirstOrDefault(m => m.Name == record.ReAgent);
                        if (templst.Count() > 0)
                        {
                            taskinfodetail.AgtOrExtID = templst[0].AgentID;
                            taskinfodetail.AgtOrExtName = templst[0].AgentName;
                        }
                        else
                        {
                            taskinfodetail.AgtOrExtID = temp.ID.ToString();
                            taskinfodetail.AgtOrExtName = record.Agent;
                        }

                        optReturn = XMLHelper.SeriallizeObject(taskinfodetail);
                        if (!optReturn.Result)
                        {
                           ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                            return;
                        }
                        webRequest.ListData.Add(optReturn.Data.ToString());
                    }

                    webRequest.Data = taskID.ToString();
                    //client = new Service31031Client();
                    client = new Service31031Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31031"));
                    webReturn = client.UMPTaskOperation(webRequest);
                    client.Close();
                    if (!webReturn.Result)
                    {
                       ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                        return;
                    }

                    string strlogrecords = "";
                    //将任务号更新到录音表
                    var tableInfo = CurrentApp.Session.ListPartitionTables.FirstOrDefault(
                       t => t.TableName == ConstValue.TABLE_NAME_RECORD && t.PartType == VoiceCyber.UMP.Common.TablePartType.DatetimeRange);
                    if (tableInfo == null)
                    {
                        string tableName = ConstValue.TABLE_NAME_RECORD;
                        foreach (RecordInfoItem r in lstRecordInfoItem)
                        {
                            strlogrecords += r.SerialID + ",";
                            webReturn = new WebReturn();
                            webRequest = new WebRequest();
                            webRequest.Session = CurrentApp.Session;
                            webRequest.Code = (int)S3103Codes.UpdateTaskID2Record;
                            webRequest.ListData.Add(taskID.ToString());
                            webRequest.ListData.Add(tableName);
                            webRequest.ListData.Add("2"); //1 初检任务 2复检 
                            webRequest.ListData.Add(r.SerialID.ToString());

                            //client = new Service31031Client();
                            client = new Service31031Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31031"));
                            //WebHelper.SetServiceClient(client);
                            webReturn = client.UMPTaskOperation(webRequest);
                            client.Close();
                            if (!webReturn.Result)
                            {
                               ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                                return;
                            }
                        }
                    }
                    else
                    {
                        string tableName = string.Empty;
                        foreach (RecordInfoItem r in lstRecordInfoItem)
                        {
                            strlogrecords += r.SerialID + ",";
                            webReturn = new WebReturn();
                            webRequest = new WebRequest();
                            webRequest.Session = CurrentApp.Session;
                            webRequest.Code = (int)S3103Codes.UpdateTaskID2Record;
                            webRequest.ListData.Add(taskID.ToString());
                            tableName = ReturnTableName(Convert.ToDateTime(r.StartRecordTime));
                            webRequest.ListData.Add(tableName);
                            webRequest.ListData.Add("2");//1 初检任务 2复检 
                            webRequest.ListData.Add(r.SerialID.ToString());


                            //client = new Service31031Client();
                            client = new Service31031Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31031"));
                            //WebHelper.SetServiceClient(client);
                            webReturn = client.UMPTaskOperation(webRequest);
                            client.Close();
                            if (!webReturn.Result)
                            {
                               ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                                return;
                            }
                        }
                    }

                    #region 写操作日志
                    string strLog = string.Format("{0} {1}", Utils.FormatOptLogString("3103T00001"), userTaskInfoShow.TaskName);
                    strLog += string.Format("{0} {1}", Utils.FormatOptLogString("3103T00025"), strlogrecords.TrimEnd(','));
                    CurrentApp.WriteOperationLog(S3103Consts.OPT_DoubleTaskAssign.ToString(), ConstValue.OPT_RESULT_SUCCESS, strLog);
                    #endregion

                    List<RecordInfoItem> ListRecordTemp = mListAllRecordInfoItems.ToList();
                    //清空mListCurrentRecordInfoItems;
                    foreach (RecordInfoItem r in lstRecordInfoItem)
                    {
                        ListRecordTemp.Remove(r);
                    }
                    mListCurrentRecordInfoItems.Clear();
                    mListAllRecordInfoItems = ListRecordTemp;

                    mRecordTotal = mListAllRecordInfoItems.Count();
                    FillListView();
                    SetPageState();

                    //添加该任务到任务列表
                    userTaskInfoShow.IsFinish = "N";
                    ListCurrentUserTasks.Add(userTaskInfoShow);
                }
                InitTasks();
            }
            catch (Exception ex)
            {
                CurrentApp.WriteOperationLog(S3103Consts.OPT_DoubleTaskAssign.ToString(), ConstValue.OPT_RESULT_FAIL, "");
                MessageBox.Show(ex.Message, CurrentApp.AppName);
            }

            return;
        }

        private string ReturnTableName(DateTime stopTime)
        {
            string strTableName = string.Empty;
            strTableName = string.Format("{0}_{1}_{2}", ConstValue.TABLE_NAME_RECORD,
                            CurrentApp.Session.RentInfo.Token, stopTime.ToString("yyMM"));
            return strTableName;
        }

        #endregion

        #region 複檢任務調整

        private void AddToReCheckTask()
        {
            string messageName = CurrentApp.GetLanguageInfo("FO3103009", "Add To Task");
            if (mListAllRecordInfoItems.Count == 0)
            {
                string temp = CurrentApp.GetLanguageInfo("3103T00108", "No Record");
                MessageBox.Show(temp, messageName);
                return;
            }
            UserTasksInfoShow currentTask = (UserTasksInfoShow)LvDoubleTaskData.SelectedItem;
            if (currentTask == null)
            {
                string temp = CurrentApp.GetLanguageInfo("3103T00109", "No Task Is Select");
                MessageBox.Show(temp, messageName);
                return;
            }
            if (Convert.ToDateTime(currentTask.DealLine) < DateTime.Now)
            {
                string temp = CurrentApp.GetLanguageInfo("3103T00110", "Task Is Past Due");
                MessageBox.Show(temp, messageName);
                return;
            }

            List<RecordInfoItem> lstData = new List<RecordInfoItem>();
            long recordlength = 0;
            int recordcount = 0;
            //如果当前list一个未选中，则是全部附加到任务中
            if (mListCurrentRecordInfoItems.Where(p => p.IsCheck == true).Count() > 0)
            {
                for (int i = 0; i < mListCurrentRecordInfoItems.Count; i++)
                {
                    if (mListCurrentRecordInfoItems[i].IsCheck == true)
                    {
                        if (mListCurrentRecordInfoItems[i].TaskUserID == currentTask.FinishUserID.ToString())
                        {
                           ShowException(CurrentApp.GetLanguageInfo("", "不能将复检任务分配给与初检质检员相同的人"));
                            return;
                        }
                        lstData.Add(mListCurrentRecordInfoItems[i]);
                        recordlength += mListCurrentRecordInfoItems[i].Duration;
                        recordcount++;
                    }
                }
            }
            else
            {
                string temp = CurrentApp.GetLanguageInfo("3103T00108", "No Record");
                MessageBox.Show(temp, messageName);
                return;
            }

            currentTask.TaskAllRecordLength = currentTask.TaskAllRecordLength + recordlength;
            currentTask.AssignNum = currentTask.AssignNum + recordcount;
            try
            {

                //先更新表T_31_020_TaskList
                //提交任务
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3103Codes.UpdateTask;
                OperationReturn optReturn = XMLHelper.SeriallizeObject(currentTask);
                if (!optReturn.Result)
                {
                   ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                webRequest.Data = optReturn.Data.ToString();
                string strrecords = "";
                //Service31031Client client = new Service31031Client();
                Service31031Client client = new Service31031Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31031"));
                //WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.UMPTaskOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                   ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                else
                {
                    //再更新表T_31_022_TaskListMappingRecord
                    webReturn = new WebReturn();
                    webRequest = new WebRequest();
                    webRequest.Session = CurrentApp.Session;
                    webRequest.Code = (int)S3103Codes.SaveTaskRecord;
                    TaskInfoDetail taskinfodetail;
                    foreach (RecordInfoItem record in lstData)
                    {
                        strrecords += record.SerialID + ",";
                        taskinfodetail = new TaskInfoDetail();
                        taskinfodetail.TaskID = currentTask.TaskID;
                        taskinfodetail.RecoredReference = record.SerialID;
                        taskinfodetail.IsLock = "N";
                        taskinfodetail.AllotType = 1;
                        taskinfodetail.FromTaskID = -1;
                        taskinfodetail.TaskType = "3";
                        taskinfodetail.StartRecordTime = Convert.ToDateTime(record.StartRecordTime).ToUniversalTime();
                        taskinfodetail.Duration = record.Duration.ToString();
                        taskinfodetail.Extension = record.Extension;
                        taskinfodetail.CalledID = record.CalledID;
                        taskinfodetail.CallerID = record.CallerID;
                        taskinfodetail.Direction = record.Direction;
                        List<CtrolAgent> templst = new List<CtrolAgent>();
                        if (S3103App.GroupingWay.Contains("A"))
                        {
                            templst = S3103App.ListCtrolAgentInfos.Where(p => p.AgentName == record.ReAgent).ToList();
                        }
                        else if (S3103App.GroupingWay.Contains("R"))
                        {
                            templst = S3103App.ListCtrolRealityExtension.Where(p => p.AgentName == record.ReAgent).ToList();
                        }
                        else if (S3103App.GroupingWay.Contains("E"))
                        {
                            templst = S3103App.ListCtrolExtension.Where(p => p.AgentName == record.ReAgent).ToList();
                        }
                        if (templst.Count() > 0)
                        {
                            taskinfodetail.AgtOrExtID = templst[0].AgentID;
                            taskinfodetail.AgtOrExtName = templst[0].AgentName;
                        }
                        else
                        {
                            taskinfodetail.AgtOrExtID = "";
                            taskinfodetail.AgtOrExtName = "";
                        }
                        optReturn = XMLHelper.SeriallizeObject(taskinfodetail);
                        if (!optReturn.Result)
                        {
                           ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                            return;
                        }
                        webRequest.ListData.Add(optReturn.Data.ToString());
                    }

                    webRequest.Data = currentTask.TaskID.ToString();
                    //client = new Service31031Client();
                    client = new Service31031Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31031"));
                    //WebHelper.SetServiceClient(client); 
                    webReturn = client.UMPTaskOperation(webRequest);
                    client.Close();
                    if (!webReturn.Result)
                    {
                       ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                        return;
                    }


                    //将任务号更新到录音表
                    var tableInfo = CurrentApp.Session.ListPartitionTables.FirstOrDefault(
                       t => t.TableName == ConstValue.TABLE_NAME_RECORD && t.PartType == VoiceCyber.UMP.Common.TablePartType.DatetimeRange);
                    if (tableInfo == null)
                    {
                        string tableName = ConstValue.TABLE_NAME_RECORD;
                        foreach (RecordInfoItem r in lstData)
                        {
                            webReturn = new WebReturn();
                            webRequest = new WebRequest();
                            webRequest.Session = CurrentApp.Session;
                            webRequest.Code = (int)S3103Codes.UpdateTaskID2Record;
                            webRequest.ListData.Add(currentTask.TaskID.ToString());
                            webRequest.ListData.Add(tableName);
                            webRequest.ListData.Add("2");//1 初检任务 2复检 
                            webRequest.ListData.Add(r.SerialID.ToString());

                            //client = new Service31031Client();
                            client = new Service31031Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31031"));
                            //WebHelper.SetServiceClient(client);
                            webReturn = client.UMPTaskOperation(webRequest);
                            client.Close();
                            if (!webReturn.Result)
                            {
                               ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                                return;
                            }
                        }
                    }
                    else
                    {
                        string tableName = string.Empty;
                        foreach (RecordInfoItem r in lstData)
                        {
                            webReturn = new WebReturn();
                            webRequest = new WebRequest();
                            webRequest.Session = CurrentApp.Session;
                            webRequest.Code = (int)S3103Codes.UpdateTaskID2Record;
                            webRequest.ListData.Add(currentTask.TaskID.ToString());
                            tableName = ReturnTableName(Convert.ToDateTime(r.StartRecordTime));
                            webRequest.ListData.Add(tableName);
                            webRequest.ListData.Add("2");//1 初检任务 2复检 
                            webRequest.ListData.Add(r.SerialID.ToString());


                            //client = new Service31031Client();
                            client = new Service31031Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31031"));
                            //WebHelper.SetServiceClient(client);
                            webReturn = client.UMPTaskOperation(webRequest);
                            client.Close();
                            if (!webReturn.Result)
                            {
                               ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                                return;
                            }
                        }
                    }
                    #region 写操作日志
                    string strLog = string.Format("{0} {1}", Utils.FormatOptLogString("3103T00025"), strrecords.TrimEnd(','));
                    strLog += string.Format("{0} {1}", Utils.FormatOptLogString("3103T00150"), currentTask.TaskName);
                    CurrentApp.WriteOperationLog(S3103Consts.OPT_DoubleTaskAssign.ToString(), ConstValue.OPT_RESULT_SUCCESS, strLog);
                    #endregion
                    List<RecordInfoItem> ListRecordTemp = mListAllRecordInfoItems.ToList();
                    //清空mListCurrentRecordInfoItems;
                    foreach (RecordInfoItem r in lstData)
                    {
                        ListRecordTemp.Remove(r);
                    }
                    mListCurrentRecordInfoItems.Clear();
                    mListAllRecordInfoItems = ListRecordTemp;

                    mRecordTotal = mListAllRecordInfoItems.Count();
                    FillListView();
                    SetPageState();

                    //更新该任务到任务列表   
                    ListCurrentUserTasks.Remove(ListCurrentUserTasks.FirstOrDefault(p => p.TaskID == currentTask.TaskID));
                    ListCurrentUserTasks.Add(currentTask);
                }
            }
            catch (Exception ex)
            {
                CurrentApp.WriteOperationLog(S3103Consts.OPT_DoubleTaskAssign.ToString(), ConstValue.OPT_RESULT_FAIL, "");
                MessageBox.Show(ex.Message, CurrentApp.AppName);

            }

            return;

        }


        #endregion

        #region 播放錄音&錄音簡情

        void LvRecordData_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var item = LvRecordData.SelectedItem as RecordInfoItem;
            if (item != null)
            {
                mCurrentRecordInfoItem = item;
                PlayRecord();
            }
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
                    info.Player = CurrentApp.Session.UserID;
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
                    mUCPlayBox.CurrentApp = CurrentApp;
                    mUCPlayBox.PlayStopped += mUCPlayBox_PlayStopped;
                    mUCPlayBox.ParentPage2 = this;
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

                }
            }
            /*
             try
            {
                if (mCurrentRecordInfoItem != null)
                {
                    if (ListSftpServers == null || ListSftpServers.Count <= 0)
                    {
                        App.ShowExceptionMessage(string.Format("{0}",
                            App.GetLanguageInfo("3103T00122", "SftpServer not exist")));
                        return;
                    }
                    var sftpServer = ListSftpServers.FirstOrDefault(s => s.HostAddress == mCurrentRecordInfoItem.VoiceIP);
                    if (sftpServer == null)
                    {
                        App.ShowExceptionMessage(string.Format("{0}\r\n\r\n{1}",
                            App.GetLanguageInfo("3103T00122", "SftpServer not exist"),
                            mCurrentRecordInfoItem.VoiceIP));
                        return;
                    }
                    string strPartInfo = string.Empty;
                    PartitionTableInfo partInfo =
                        App.Session.ListPartitionTables.FirstOrDefault(
                            p =>
                                p.TableName == ConstValue.TABLE_NAME_RECORD && p.PartType == TablePartType.DatetimeRange);
                    if (partInfo != null)
                    {
                        DateTime startTime = Convert.ToDateTime(mCurrentRecordInfoItem.StartRecordTime);
                        strPartInfo = string.Format("{0}{1}", startTime.ToString("yy"), startTime.ToString("MM"));
                    }
                    if (mService03Helper == null)
                    {
                        App.ShowExceptionMessage(string.Format("Fail.\tService03Helper is null"));
                        return;
                    }
                    RequestMessage request = new RequestMessage();
                    request.Command = (int)Service03Command.DownloadRecordFile;
                    request.ListData.Add(sftpServer.HostAddress);
                    request.ListData.Add(sftpServer.HostPort.ToString());
                    request.ListData.Add(string.Format("{0}|{1}", App.Session.UserID, App.Session.RentInfo.Token));
                    request.ListData.Add(App.Session.UserInfo.Password);
                    request.ListData.Add(mCurrentRecordInfoItem.RowID.ToString());
                    request.ListData.Add(mCurrentRecordInfoItem.SerialID.ToString());
                    request.ListData.Add(strPartInfo);

                    OperationReturn optReturn = null;
                    VoicePlayer.IsEnabled = false;
                    mWorker = new BackgroundWorker();
                    mWorker.DoWork += (s, de) =>
                    {
                        optReturn = mService03Helper.DoRequest(request);
                    };
                    mWorker.RunWorkerCompleted += (s, re) =>
                    {
                        mWorker.Dispose();
                        VoicePlayer.IsEnabled = true;
                        if (!optReturn.Result)
                        {
                            App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                            return;
                        }
                        ReturnMessage retMessage = optReturn.Data as ReturnMessage;
                        if (retMessage == null)
                        {
                            App.ShowExceptionMessage(string.Format("ReturnMessage is null"));
                            return;
                        }
                        if (!retMessage.Result)
                        {
                            App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", retMessage.Code, retMessage.Message));
                            return;
                        }
                        try
                        {
                            string path = retMessage.Data;
                            path = string.Format("{0}://{1}:{2}/MediaData/{3}",
                                App.Session.AppServerInfo.Protocol,
                                App.Session.AppServerInfo.Address,
                                App.Session.AppServerInfo.Port,
                                path);
                            App.WriteLog(string.Format("Url:{0}", path));
                            VoicePlayer.Url = path;
                            VoicePlayer.Title = mCurrentRecordInfoItem.RecordReference.ToString();
                            VoicePlayer.Play();
                        }
                        catch (Exception ex)
                        {
                            App.ShowExceptionMessage(ex.Message);
                        }
                    };
                    mWorker.RunWorkerAsync();
                }
            }
             * */
            catch (Exception ex)
            {
               ShowException(ex.Message);
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
                        LvRecordData.SelectedItem = playItem;
                        mUCPlayBox = new UCPlayBox();
                        mUCPlayBox.CurrentApp = CurrentApp;
                        mUCPlayBox.PlayStopped += mUCPlayBox_PlayStopped;
                        mUCPlayBox.ParentPage2 = this;
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
               ShowException(ex.Message);
            }
        }


        private void OnPlayerEvent(object sender, RoutedPropertyChangedEventArgs<PlayerEventArgs> e)
        {
            if (PlayerEvent != null)
            {
                PlayerEvent(sender, e);
            }
        }

        void VoicePlayer_PlayerEvent(object sender, RoutedPropertyChangedEventArgs<PlayerEventArgs> e)
        {
            try
            {
                //For Test
                //App.WriteLog("Player", string.Format("{0}\t{1}", code, param));
                OnPlayerEvent(sender, e);
                if (e.NewValue == null) { return; }
                int code = e.NewValue.Code;
                var param = e.NewValue.Data;
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
               ShowException(ex.Message);
            }
        }


        void LvRecordData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RecordInfoItem item = LvRecordData.SelectedItem as RecordInfoItem;
            if (item != null)
            {
                mCurrentRecordInfoItem = item;

                //InitBasicOperations();
                //CreateOptButtons();
                CreateCallInfoItems();
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
                    item.Name = CurrentApp.GetLanguageInfo("3103T00025", "Record Reference");
                    item.Value = mCurrentRecordInfoItem.SerialID.ToString();
                    mListCallInfoPropertyItems.Add(item);
                    item = new CallInfoPropertyItem();
                    item.Name = CurrentApp.GetLanguageInfo("3103T00059", "StartRecord Time");
                    item.Value = mCurrentRecordInfoItem.StartRecordTime;
                    mListCallInfoPropertyItems.Add(item);
                    item = new CallInfoPropertyItem();
                    item.Name = CurrentApp.GetLanguageInfo("3103T00060", "StopRecord Time");
                    item.Value = mCurrentRecordInfoItem.StopRecordTime;
                    mListCallInfoPropertyItems.Add(item);
                    item = new CallInfoPropertyItem();
                    item.Name = CurrentApp.GetLanguageInfo("3103T00066", "Duration");
                    item.Value = mCurrentRecordInfoItem.StrDuration;
                    mListCallInfoPropertyItems.Add(item);
                    item = new CallInfoPropertyItem();
                    item.Name = CurrentApp.GetLanguageInfo("3103T00067", "Direction");
                    item.Value = mCurrentRecordInfoItem.DirShow;
                    mListCallInfoPropertyItems.Add(item);
                    item = new CallInfoPropertyItem();
                    item.Name = CurrentApp.GetLanguageInfo("3103T00064", "Extension");
                    item.Value = mCurrentRecordInfoItem.Extension;
                    mListCallInfoPropertyItems.Add(item);
                    item = new CallInfoPropertyItem();
                    item.Name = CurrentApp.GetLanguageInfo("3103T00065", "Agent ID");
                    item.Value = mCurrentRecordInfoItem.Agent;
                    mListCallInfoPropertyItems.Add(item);
                    item = new CallInfoPropertyItem();
                    item.Name = CurrentApp.GetLanguageInfo("3103T00068", "Caller ID");
                    item.Value =S3103App.DecryptString(mCurrentRecordInfoItem.CallerID);
                    mListCallInfoPropertyItems.Add(item);
                    item = new CallInfoPropertyItem();
                    item.Name = CurrentApp.GetLanguageInfo("3103T00069", "Called ID");
                    item.Value = S3103App.DecryptString(mCurrentRecordInfoItem.CalledID);
                    mListCallInfoPropertyItems.Add(item);
                }
            }
            catch (Exception ex)
            {
               ShowException(ex.Message);
            }
        }
        #endregion

        #region 右上角功能按鈕

        private void InitPanels()
        {
            try
            {
                mListPanels.Clear();
                OperationInfo optInfo;

                PanelItem panelItem = new PanelItem();
                panelItem.Name = "RecordList";
                panelItem.ContentID = "PanelRecordList";
                panelItem.Title = CurrentApp.GetLanguageInfo("3103T00070", "Record List");
                panelItem.Icon = "Images/recordlist.png";
                panelItem.IsVisible = true;
                panelItem.CanClose = false;
                mListPanels.Add(panelItem);

                panelItem = new PanelItem();
                panelItem.Name = "ObjectList";
                panelItem.ContentID = "PanelObjectBox";
                panelItem.Title = CurrentApp.GetLanguageInfo("3103T00057", "Allot Task List");
                panelItem.Icon = "Images/objectbox.png";
                panelItem.CanClose = true;
                panelItem.IsVisible = true;
                mListPanels.Add(panelItem);

                panelItem = new PanelItem();
                panelItem.Name = "PlayBox";
                panelItem.ContentID = "PanelPlayBox";
                panelItem.Title = CurrentApp.GetLanguageInfo("3103T00047", "Play Box");
                panelItem.Icon = "Images/playbox.png";
                panelItem.CanClose = true;
                optInfo = S3103App.ListOperationInfos.Where(o => o.ID == S3103Consts.OPT_PLAYRECORD).First();
                if (optInfo != null)
                {
                    panelItem.IsVisible = true;
                    mListPanels.Add(panelItem);
                }

                panelItem = new PanelItem();
                panelItem.Name = "CallInfo";
                panelItem.ContentID = "PanelCallInfo";
                panelItem.Title = CurrentApp.GetLanguageInfo("3103T00071", "Call Information");
                panelItem.Icon = "Images/callinfo.png";
                panelItem.CanClose = true;
                panelItem.IsVisible = true;
                mListPanels.Add(panelItem);

                //panelItem = new PanelItem();
                //panelItem.Name = "Memo";
                //panelItem.ContentID = "PanelMemo";
                //panelItem.Title = App.GetLanguageInfo("3103T00072", "Memo");
                //panelItem.Icon = "Images/memobox.png";
                //panelItem.CanClose = true;
                //optInfo = ListOperations.FirstOrDefault(o => o.ID == S3103Consts.OPT_MEMORECORD);
                //if (optInfo != null)
                //{
                //    panelItem.IsVisible = true;
                //    mListPanels.Add(panelItem);
                //}
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

                //toolItem = new ToolButtonItem();
                //toolItem.Name = "BT" + "SaveLayout";
                //toolItem.Display = App.GetLanguageInfo("FO3103013", "Save Layout");
                //toolItem.Tip = App.GetLanguageInfo("FO3103013", "Save Layout");
                //toolItem.Icon = "Images/savelayout.png";
                //btn = new Button();
                //btn.Click += ToolButton_Click;
                //btn.DataContext = toolItem;
                //btn.SetResourceReference(StyleProperty, "ToolBarButtonStyle");
                //optInfo = App.ListOperationInfos.Where(p => p.ID == S3103Consts.OPT_SAVELAYOUT).First();
                //if (optInfo != null)
                //{
                //    PanelToolButton.Children.Add(btn);
                //}

                //toolItem = new ToolButtonItem();
                //toolItem.Name = "BT" + "ResetLayout";
                //toolItem.Display = App.GetLanguageInfo("FO3103012", "Reset Layout");
                //toolItem.Tip = App.GetLanguageInfo("FO3103012", "Reset Layout");
                //toolItem.Icon = "Images/resetlayout.png";
                //btn = new Button();
                //btn.Click += ToolButton_Click;
                //btn.DataContext = toolItem;
                //btn.SetResourceReference(StyleProperty, "ToolBarButtonStyle");
                //optInfo = App.ListOperationInfos.Where(p => p.ID == S3103Consts.OPT_RESETLGYOUT).First();
                //if (optInfo != null)
                //{
                //    PanelToolButton.Children.Add(btn);
                //}

                //保存列的顺序
                toolItem = new ToolButtonItem();
                toolItem.Name = "BT" + "SaveColumns";
                toolItem.Display = CurrentApp.GetLanguageInfo("3103T00168", "Save Columns");
                toolItem.Tip = CurrentApp.GetLanguageInfo("3103T00168", "Save Columns");
                toolItem.Icon = "Images/savelayout.png";
                btn = new Button();
                btn.Click += ToolButton_Click;
                btn.DataContext = toolItem;
                btn.SetResourceReference(StyleProperty, "ToolBarButtonStyle");
                PanelToolButton.Children.Add(btn);

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
                        case "BTResetLayout":
                            LoadLayout();
                            SetViewStatus();
                            break;
                        case "BTSaveColumns":
                            SaveColumns();
                            break;
                    }
                }
            }
        }


        private void SetPanelVisible()
        {
            try
            {
                var panel =
                      PanelManager.Layout.Descendents().OfType<LayoutAnchorable>().FirstOrDefault(p => p.ContentId == "PanelRecordList");
                if (panel != null)
                {
                    panel.Title = CurrentApp.GetLanguageInfo("3103T00070", "Record List");
                }

                panel =
                      PanelManager.Layout.Descendents().OfType<LayoutAnchorable>().FirstOrDefault(p => p.ContentId == "PanelObjectBox");
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
                    panel.Title = CurrentApp.GetLanguageInfo("3103T00057", "Allot Task List");
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
                    panel.Title = CurrentApp.GetLanguageInfo("3103T00047", "Play Box");
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
                    panel.Title = CurrentApp.GetLanguageInfo("3103T00071", "Call Information");
                }

                //panel =
                //     PanelManager.Layout.Descendents().OfType<LayoutAnchorable>().FirstOrDefault(p => p.ContentId == "PanelMemo");
                //if (panel != null)
                //{
                //    var panelItem = mListPanels.FirstOrDefault(p => p.ContentID == panel.ContentId);
                //    if (panelItem != null)
                //    {
                //        if (panelItem.IsVisible)
                //        {
                //            panel.Show();
                //        }
                //        else
                //        {
                //            panel.Hide();
                //        }
                //        LayoutAnchorable panel1 = panel;
                //        panel.IsVisibleChanged += (s, e) =>
                //        {
                //            panelItem.IsVisible = panel1.IsVisible;
                //            SetViewStatus();
                //        };
                //    }
                //    panel.Title = App.GetLanguageInfo("3103T00072", "Memo");
                //}
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

        private void SaveLayout()
        {
            try
            {
                var serializer = new XmlLayoutSerializer(PanelManager);
                var dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                    string.Format("UMP/{0}", CurrentApp.AppName));
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                string path = Path.Combine(dir, "layout.xml");
                using (var stream = new StreamWriter(path))
                {
                    serializer.Serialize(stream);
                }
                ShowInformation(CurrentApp.GetLanguageInfo("3103T00128", "Save layout end."));
            }
            catch (Exception ex)
            {
               ShowException(ex.Message);
            }
        }
        private void LoadLayout()
        {
            try
            {
                string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                    string.Format("UMP/{0}/layout.xml", CurrentApp.AppName));
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
                ChangeLanguage();
            }
            catch (Exception ex)
            {
               ShowException(ex.Message);
            }
        }

        //保存自定义列
        private void SaveColumns()
        {
            try
            {
                GVitem = new GridViewOrder();
                string cols = string.Empty;
                string lans = string.Empty;
                string width = string.Empty;
                var columns = LvRecordData.View as GridView;
                if (columns == null) { return; }
                int i = 0;
                foreach (GridViewColumn item in columns.Columns)
                {
                    GVitem = GviewList.Where(g => g.Header == item.Header.ToString()).FirstOrDefault();
                    if (GVitem != null)
                    {
                        if (i == 0)//排除第一个,
                        {
                            cols = GVitem.cols;
                            lans = GVitem.Lans;
                            width = item.Width.ToString();
                        }
                        else
                        {
                            cols = cols + "," + GVitem.cols;
                            lans = lans + "," + GVitem.Lans;
                            width = width + "," + item.Width.ToString();
                        }
                        i++;
                    }
                }
                if (string.IsNullOrWhiteSpace(cols))
                {
                    return;
                }
                WriteLog.CreatColumnsInfoXml(string.Format("{0}ss", CurrentApp.Session.UserID.ToString()), lans, cols, width);
                ShowInformation(CurrentApp.GetLanguageInfo("3103T00128", "Save layout end."));
            }
            catch (Exception ex)
            {
               ShowException(ex.Message);
            }
        }
        #endregion


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
                    // ShowException("1" + ex.Message);
                }
            }
            if (!bPage)
            {
                //如果通过Url没有加载成功，就从已经编译到程序集的默认资源加载
                try
                {
                    string uri = string.Format("/UMPS3103;component/Themes/{0}/{1}", "Default", StylePath);
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
                string uri = string.Format("/UMPS3103;component/Themes/Default/UMPS3103/MainPageStatic.xaml");
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
                string uri = string.Format("/UMPS3103;component/Themes/Default/UMPS3103/QMAvalonDock.xaml");
                ResourceDictionary resource = new ResourceDictionary();
                resource.Source = new Uri(uri, UriKind.RelativeOrAbsolute);
                Resources.MergedDictionaries.Add(resource);
            }
            catch (Exception ex)
            {
                //ShowException("3" + ex.Message);
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

        public void OpenPasswordPanel(UMPUserControl content)
        {
            try
            {
                PopupPanel.Content = content;
                PopupPanel.IsOpen = true;
            }
            catch (Exception ex)
            {
               ShowException(ex.Message);
            }
        }

        #region 语言
        public override void ChangeLanguage()
        {
            try
            {
                base.ChangeLanguage();
                #region panel
                var panel = GetPanleByContentID("PanelObjectBox");
                if (panel != null)
                {
                    panel.Title = CurrentApp.GetLanguageInfo("3103T00151", "My Calibration Task");
                }
                panel = GetPanleByContentID("PanelRecordList");
                if (panel != null)
                {
                    panel.Title = CurrentApp.GetLanguageInfo("3103T00070", "Record List");
                }
                panel = GetPanleByContentID("PanelPlayBox");
                if (panel != null)
                {
                    panel.Title = CurrentApp.GetLanguageInfo("3103T00047", "Play Box");
                }
                panel = GetPanleByContentID("PanelCallInfo");
                if (panel != null)
                {
                    panel.Title = CurrentApp.GetLanguageInfo("3103T00071", "Call Information");
                }
                #endregion

                PopupPanel.ChangeLanguage();
                //给换语言包
                ExpanderBasic.Header = CurrentApp.GetLanguageInfo("3103T00052", "Basic Operations");
                ExpanderOther.Header = CurrentApp.GetLanguageInfo("3103T00053", "Other Position");
                //列名
            }
            catch { }
        }
        #endregion

        #region ohter
        private LayoutAnchorable GetPanleByContentID(string contentID)
        {
            var panel =
                PanelManager.Layout.Descendents()
                    .OfType<LayoutAnchorable>()
                    .FirstOrDefault(p => p.ContentId == contentID);
            return panel;
        }

        public void SetBusy(bool flag)
        {
            if (flag)
            {
                SetBusy(true, "");
            }
            else
            {
                SetBusy(false, "");
            }
        }

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
        #endregion
    }
}
