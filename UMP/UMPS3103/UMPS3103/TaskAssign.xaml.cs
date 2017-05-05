using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common31031;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;
using System.Windows.Controls.Primitives;
using UMPS3103.Wcf31031;
using UMPS3103.Wcf11012;
using UMPS3103.Commands;
using UMPS3103.Models;
using VoiceCyber.Wpf.AvalonDock.Layout;
using VoiceCyber.Wpf.AvalonDock.Layout.Serialization;
using System.Text;
using VoiceCyber.NAudio.Controls;
using UMPS3103.Codes;
using VoiceCyber.UMP.CommonService03;
using System.Timers;
using UMPS3103.Wcf31021;
using UMPS3103.DoubleTask;

namespace UMPS3103
{
    /// <summary>
    /// TaskAssign.xaml 的交互逻辑
    /// </summary>
    public partial class TaskAssign
    {
        #region StaticMembers

        //所有权限
        public static ObservableCollection<UserTasksInfoShow> ListCurrentUserTasks = new ObservableCollection<UserTasksInfoShow>();
        private ObservableCollection<OperationInfo> mListBasicOperations;
        public ObservableCollection<RecordInfoItem> mListCurrentRecordInfoItems;
        private ObservableCollection<CallInfoPropertyItem> mListCallInfoPropertyItems;
        private List<RecordInfoItem> mListAllRecordInfoItems;
        private ObservableCollection<RecordPlayItem> mListRecordPlayItems;
        private RecordInfoItem mCurrentRecordInfoItem;
        private RecordPlayItem mCurrentRecordPlayItem;
        private BackgroundWorker mWorker;

        private List<PanelItem> mListPanels;    
        private UCKeyWord mUCPlayBox;

        private List<SftpServerInfo> mListSftpServers;
        private Service03Helper mService03Helper;
        private List<SettingInfo> mListSettingInfos;
        //private List<SftpServerInfo> mListSftpServers;
        private List<DownloadParamInfo> mListDownloadParams;
        private List<RecordEncryptInfo> mListRecordEncryptInfos;


        public ObjectItemTask mRootItem;

        private int mCircleMode;
        private int mPageIndex;
        private int mPageCount;
        private int mPageSize;
        private int mRecordTotal;
        private int mMaxRecords;
        private bool mIsQueryContinue;
        public event RoutedPropertyChangedEventHandler<PlayerEventArgs> PlayerEvent;

        //ABCD拼接SQL查询串的临时存储
        public string ABCDTempStr = string.Empty;

        //保存自定义列
        private List<GridViewOrder> GviewList;
        private GridViewOrder GVitem;
        #endregion

        public TaskAssign()
        {
            InitializeComponent();
            mListBasicOperations = new ObservableCollection<OperationInfo>();
            mListPanels = new List<PanelItem>();
            mListCurrentRecordInfoItems = new ObservableCollection<RecordInfoItem>();
            mListAllRecordInfoItems = new List<RecordInfoItem>();
            mCurrentRecordInfoItem = new RecordInfoItem();
            mListRecordPlayItems = new ObservableCollection<RecordPlayItem>();
            mListCallInfoPropertyItems = new ObservableCollection<CallInfoPropertyItem>();
            mListRecordEncryptInfos = S3103App.mListRecordEncryptInfos;
            mListDownloadParams = S3103App.mListDownloadParams;
            mListSettingInfos = new List<SettingInfo>();
            mListSftpServers = S3103App.mListSftpServers;
            mService03Helper = S3103App.mService03Helper;
            GviewList = new List<GridViewOrder>();

            mCircleMode = 0;
            mPageIndex = 0;
            mPageCount = 0;
            mPageSize = 100;
            mRecordTotal = 0;
            mMaxRecords = 100000;
            mIsQueryContinue = false;
            //页面跳到第几页
            TxtPage.KeyUp += TxtPage_KeyUp;

            //当前选中创建操作按钮 更新录音信息
            LvRecordData.SelectionChanged += LvRecordData_SelectionChanged;
            LvRecordData.ItemsSource = mListCurrentRecordInfoItems;
            
            LvTaskData.ItemsSource = ListCurrentUserTasks;
            LvTaskData.SelectionChanged += LvTaskData_SelectedItemChanged;
            ListBoxCallInfo.ItemsSource = mListCallInfoPropertyItems;
        }

        /// <summary>
        /// 初始化用户信息以及样式
        /// </summary>
        protected override void Init() //用了init()方法之后，不能在用this.load的方法了
        {
            try
            {
                PageName = "TaskAssign";
                StylePath = "UMPS3103/MainPageStyle.xaml";
                base.Init();
                if (CurrentApp != null)
                {
                    CurrentApp.SendLoadedMessage();
                }

                InitOperationButton();

                BindCommands();
                //加载当前用户分配的任务
                InitTaskDetailColumns();
                InitTasks();
                InitTaskRecordColumns();
                //LoadLayout();
                SetBusy(true, CurrentApp.GetMessageLanguageInfo("001", string.Format("Loading...")));
                mWorker = new BackgroundWorker();
                mWorker.DoWork += (s, de) =>
                {
                };
                mWorker.RunWorkerCompleted += (s, re) =>
                {
                    mWorker.Dispose();
                    SetBusy(false, "");

                    QueryRecordInTask querypage = new QueryRecordInTask();
                    querypage.PageParent = this;
                    querypage.CurrentApp = CurrentApp;
                    mRootItem = new ObjectItemTask();
                    querypage.InitOrgAndAgentAndExtension(mRootItem, S3103App.CurrentOrg);
                    
                    InitBasicOperations();
                    CreateOptButtons();
                    InitPanels();
                    CreateToolBarButtons();
                    SetPanelVisible();

                    //创建查询记录页面
                    CreatePageButtons();
                    CreateCallInfoItems();
                    ChangeTheme();
                    ChangeLanguage();
                };
                mWorker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
               ShowException(ex.Message);
            }
        }


        /// <summary>
        /// 获取任務跟蹤、复检权限
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
                btn.IsEnabled = !mIsQueryContinue;
                PanelOherOpts.Children.Add(btn);
            }
            if (S3103App.ListOperationInfos.Where(p => p.ID == S3103Consts.OPT_DoubleTask).Count() > 0) //複檢權限
            {
                btn = new Button();
                btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                 var opt = new OperationInfo();
                 opt.Display = CurrentApp.GetLanguageInfo("3103T00148", "Go To Task ReCheck");
                opt.Icon = "Images/controltarget.png";
                btn.DataContext = opt;
                btn.Click += btn2TaskReCheck_Click;
                btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                btn.IsEnabled = !mIsQueryContinue;
                PanelOherOpts.Children.Add(btn);
            }
        }


        /// <summary>
        /// 加载当前用户分配的任务信息
        /// </summary>
        private void InitTasks()
        {
            try
            {
                string dtstart = DateTime.Now.AddMonths(-1).ToUniversalTime().ToString();
                string dtend = DateTime.Now.AddMonths(1).ToUniversalTime().ToString();
                ListCurrentUserTasks.Clear();
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S3103Codes.GetCurrentUserTasks;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(CurrentApp.Session.UserID.ToString());
                webRequest.ListData.Add("0");//0:我分配的 1:分配给我的 2:全部
                webRequest.ListData.Add("0");//0:未完成 1:已完成 2全部
                //webRequest.ListData.Add(dtstart); //开始时间
                //webRequest.ListData.Add(dtend); //截止时间
                webRequest.ListData.Add("1"); //初檢 1；複檢 3

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

        private void  InitTaskRecordColumns()
        {
            bool hadColumnsSetting=false;//该用户是否保存有列信息

            string[] lans = "3103T00058,3103T00162,3103T00025,3103T00059,3103T00060,3103T00064,3103T00177,3103T00065,3103T00066,3103T00067,3103T00068,3103T00069,3103T00174,3103T00175,3103T00176,3103T00180,3103T00061,3103T00062,3103T00063,3103T00185,3103T00186,3103T00187".Split(',');
            string[] cols = "RowID,MediaType,SerialID,StartRecordTime,StopRecordTime,Extension,ReaExtension,Agent,StrDuration,DirShow,DecCallerID,DecCalledID,StrServiceAttitude,StrProfessionalLevel,StrRecordDurationError,StrRepeatCallIn,VoiceID,ChannelID,VoiceIP,AbnormalScores,AfterEventProcessing,SeatAgentSpeechAnomaly".Split(',');
            int[] colwidths = { 50, 40, 140, 140, 140, 80,80, 80, 100, 80, 100, 100,100,100,100, 100,80, 80, 100,80,120,160 };
            string[] widths ={""};

            List<string> columnsList = WriteLog.ReadColumnsXml(string.Format("{0}T", CurrentApp.Session.UserID.ToString()));
            if(columnsList!=null)
            {
                hadColumnsSetting=true;
                lans = columnsList[0].Split(',');
                cols = columnsList[1].Split(',');
                widths = columnsList[2].Split(',');
            }
            GridView ColumnGridView = new GridView();
            GridViewColumn gvc;
            GviewList.Clear();

            #region//全选以及播放
            gvc = new GridViewColumn();
            CheckBox chkAllCheck= new CheckBox();
            chkAllCheck.Content = CurrentApp.GetLanguageInfo("3103T00085", "All Select");
            chkAllCheck.Click+=chkAllCheck_Click;
            gvc.Header =(object)chkAllCheck;
            gvc.Width=60;
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

            for (int i = 0; i < cols.Count(); i++)
            {
                GVitem = new GridViewOrder();
                gvc = new GridViewColumn();
                gvc.Header = CurrentApp.GetLanguageInfo(lans[i], cols[i]);
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
                if (cols[i] == "RowID")
                {
                    DataTemplate dt = Resources["CellRowNumberTemplate"] as DataTemplate;
                    gvc.CellTemplate = dt;
                }
                else if (cols[i] == "MediaType")
                {
                    DataTemplate dt = Resources["CellMediaTypeTemplate"] as DataTemplate;
                    gvc.CellTemplate = dt;
                }
                else
                {
                    gvc.DisplayMemberBinding = new Binding(cols[i]);
                }
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
                foreach(RecordInfoItem r in  mListCurrentRecordInfoItems)
                {
                    r.IsCheck = true;
                }
            }
            else 
            {
                foreach(RecordInfoItem r in mListCurrentRecordInfoItems)
                {
                    r.IsCheck = false;
                }
            }
        }


        /// Author           : Luoyihua
        ///  Created          : 2014-11-06 16:46:00
        /// <summary>
        /// 初始化任务列
        /// </summary>
        private void InitTaskDetailColumns()
        {
            string[] lans = "3103T00001,3103T00017,3103T00002,3103T00006,3103T00007,3103T00008,3103T00009,3103T00010,3103T00011,3103T00014,3103T00012,3103T00015,3103T00016,3103T00021".Split(',');
            string[] cols = "TaskName,IsFinish,TaskDesc,IsShare,AssignTime,AssignUserFName,AssignNum,DealLine,AlreadyScoreNum,ModifyTime,ModifyUserFName,BelongYear,BelongMonth,FinishTime".Split(',');
            int[] colwidths = { 120, 75, 120, 75, 150, 100, 75, 150, 100, 150, 120, 50, 50, 150 };
            GridView columngv = new GridView();
            for (int i = 0; i < 14; i++)
            {
                DataTemplate CellTemplate = (DataTemplate)Resources[cols[i]];
                SetColumnGridView(cols[i], ref columngv, lans[i], cols[i], CellTemplate, colwidths[i]);
            }
            LvTaskData.View = columngv;
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
        /// 点击任务记录 生成对应按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void LvTaskData_SelectedItemChanged(object sender, SelectionChangedEventArgs e)
        { 
        
        }


        public void SaveTask2QA(UserTasksInfoShow userTaskInfoShow,List<CtrolQA> listctrolqa,int TypeOfAVG) 
        {
            userTaskInfoShow.AlreadyScoreNum = 0;

            SetBusy(true, "");
            List<RecordInfoItem> lstData = new List<RecordInfoItem>();
            if(mListAllRecordInfoItems.Count ==0)
            {
                ShowInformation(CurrentApp.GetLanguageInfo("3103T00127", "No  Record."));
                return;
            }

            //列表选中，则分选中的录音，列表未选中，则全部分配
            if (mListCurrentRecordInfoItems.Where(p => p.IsCheck == true).Count() > 0)
            {
                if (TypeOfAVG == 1)
                {
                    long recordlength = 0;
                    for (int i = 0; i < mListCurrentRecordInfoItems.Count; i++)
                    {
                        if (mListCurrentRecordInfoItems[i].IsCheck == true)
                        {
                            lstData.Add(mListCurrentRecordInfoItems[i]);
                            recordlength += mListCurrentRecordInfoItems[i].Duration;
                        }
                    }
                    userTaskInfoShow.AssignNum = lstData.Count;
                    userTaskInfoShow.TaskAllRecordLength = recordlength;
                    SaveTask(userTaskInfoShow, listctrolqa, lstData);
                }
                else if (TypeOfAVG == 2)  //要平均分配选中的qa
                {
                    #region TypeOfAVG==2
                    if (mListCurrentRecordInfoItems.Where(p => p.IsCheck == true).Count() > 0)
                    {
                        int qaNum = listctrolqa.Count();
                        Random random = new Random();
                        for (int i = 0; i < mListCurrentRecordInfoItems.Count; i++)
                        {
                            if (mListCurrentRecordInfoItems[i].IsCheck == true)
                            {
                                lstData.Add(mListCurrentRecordInfoItems[i]);
                            }
                        }

                        if (qaNum > lstData.Count)
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
                        int avgNum = lstData.Count / qaNum;

                        #region 如果分不完，提示
                        int leave = lstData.Count % qaNum;
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
                        for (int i = 0; i < qaNum; i++)
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
                            long recordlength = 0;
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
                    else  //录音列表没有选中录音
                    {
                        int qaNum = listctrolqa.Count();
                        Random random = new Random();

                        //打乱顺序
                        List<RecordInfoItem> lstRecordTemp = new List<RecordInfoItem>();
                        foreach (RecordInfoItem item in mListAllRecordInfoItems)
                        {
                            lstRecordTemp.Insert(random.Next(lstRecordTemp.Count), item);
                        }
                        lstData = lstRecordTemp;

                        //每个qa多少条
                        int avgNum = lstData.Count / qaNum;
                        if (avgNum == 0)
                        {
                            return;
                        }
                        for (int i = 0; i < qaNum; i++)
                        {
                            lstRecordTemp = new List<RecordInfoItem>();
                            List<CtrolQA> lstCtrolQaTemp = new List<CtrolQA>();
                            lstCtrolQaTemp.Add(listctrolqa[i]);
                            for (int k = 0; k < avgNum; k++)
                            {
                                RecordInfoItem r = new RecordInfoItem();
                                r = lstData[i * avgNum + k];
                                lstRecordTemp.Add(r);
                            }

                            long recordlength = 0;
                            foreach (RecordInfoItem record in lstRecordTemp)
                            {
                                recordlength += record.Duration;
                            }
                            userTaskInfoShow.TaskAllRecordLength = recordlength;
                            userTaskInfoShow.AssignNum = lstRecordTemp.Count;

                            userTaskInfoShow.TaskName = listctrolqa[i].UserName + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            SaveTask(userTaskInfoShow, lstCtrolQaTemp, lstRecordTemp);
                        }
                    }
                    #endregion
                }
            }
            else  //没有选中的录音
            {
                ShowInformation(CurrentApp.GetLanguageInfo("3103T00115", "Please select one record at least."));
                return;
            }
            SetBusy(false, "");
        }

        void SaveTask(UserTasksInfoShow userTaskInfoShow, List<CtrolQA> listctrolqa,List<RecordInfoItem> lstRecordInfoItem) 
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

                Service31031Client client = new Service31031Client();
              //  Service31031Client client = new Service31031Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31031"));
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
                    webRequest.Data=taskID.ToString();                    
                    foreach(CtrolQA qa in listctrolqa)
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
                    TaskInfoDetail taskinfodetail ;
                    foreach (RecordInfoItem record in lstRecordInfoItem)
                    {
                        taskinfodetail = new TaskInfoDetail();
                        taskinfodetail.TaskID = taskID;
                        taskinfodetail.RecoredReference =record.SerialID;
                        taskinfodetail.IsLock = "N";
                        taskinfodetail.AllotType = 1;
                        taskinfodetail.FromTaskID = -1;
                        taskinfodetail.TaskType = "1";
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
                              webRequest.ListData.Add("1");
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
                         foreach(RecordInfoItem r in  lstRecordInfoItem)
                         {
                             strlogrecords += r.SerialID + ",";
                             webReturn = new WebReturn();
                             webRequest = new WebRequest();
                             webRequest.Session = CurrentApp.Session;
                             webRequest.Code = (int)S3103Codes.UpdateTaskID2Record;
                             webRequest.ListData.Add(taskID.ToString());
                             tableName = ReturnTableName(Convert.ToDateTime(r.StartRecordTime));
                             webRequest.ListData.Add(tableName);
                             webRequest.ListData.Add("1");
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
                     CurrentApp.WriteOperationLog(S3103Consts.OPT_TASKASSIGN.ToString(), ConstValue.OPT_RESULT_SUCCESS, strLog);
                     #endregion

                     List<RecordInfoItem> ListRecordTemp = mListAllRecordInfoItems.ToList();
                    //清空mListCurrentRecordInfoItems;
                    foreach(RecordInfoItem r in lstRecordInfoItem)
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
            catch (Exception  ex)
            {
                CurrentApp.WriteOperationLog(S3103Consts.OPT_TASKASSIGN.ToString(), ConstValue.OPT_RESULT_FAIL, "");
               ShowException(ex.Message);
            }

            return ;
        }
    
        public void QueryRecord(bool isAgentday,List<QueryConditionDetail> lstQueryCondition, List<CtrolAgent> lstCtrolAgent, List<DateTimeSpliteAsDay> lstDateTimeSplitAsDay)
        {
            string strConditionString = string.Empty;
            mRecordTotal = 0;
            mPageIndex = 0;
            mPageCount = 0;
            mPageSize = S3103Consts.USER_PARAM_PAGESIZE;
            mMaxRecords = S3103Consts.USER_PARAM_MAXRECORDS;
            mIsQueryContinue = true;
            CreateOptButtons();
            mListAllRecordInfoItems.Clear();
            mListCurrentRecordInfoItems.Clear();
            bool scoreCon = false;
            string scoreVal = "0";
            scoreCon = lstQueryCondition.Where(p => p.ConditionItemID == S3103Consts.CON_SCORE).ToList().Count() > 0 ? true : false;
            if (scoreCon)
            {
                scoreVal = lstQueryCondition.Where(p => p.ConditionItemID == S3103Consts.CON_SCORE).ToList().First().Value01;
            }
            ABCDTempStr = string.Empty;
            if (!isAgentday) //为没有勾选座席每天
            {
                strConditionString = MakeConditionString(lstQueryCondition, lstCtrolAgent);
                try
                {
                    SetBusy(true, "");
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
                                QueryRecord(scoreCon, scoreVal, strConditionString, 0, string.Format("{0}_{1}", ConstValue.TABLE_NAME_RECORD, CurrentApp.Session.RentInfo.Token));
                            }
                            else
                            {
                                ////按录音服务器查询
                                QueryRecord(scoreCon, scoreVal, strConditionString, 0, string.Format("{0}_{1}", ConstValue.TABLE_NAME_RECORD, CurrentApp.Session.RentInfo.Token));
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
                                string  strTableName = string.Format("{0}_{1}_{2}", ConstValue.TABLE_NAME_RECORD,
                                    CurrentApp.Session.RentInfo.Token, partTable);
                                QueryRecord(scoreCon,scoreVal, strConditionString, 0, strTableName);
                            }                            
                        }
                    };
                    mWorker.RunWorkerCompleted += (s, re) =>
                    {
                        mWorker.Dispose();
                        SetBusy(false, "");
                        mIsQueryContinue = false;
                        CreateOptButtons();
                        InitBasicOperations();
                        CreateOptButtons();
                        SetPageState();
                    };
                    mWorker.RunWorkerAsync();
                }
                catch (Exception ex)
                {
                   ShowException(ex.Message);
                }
            }
            else //勾选座席每天
            {
                try
                {
                    SetBusy(true, "");
                    mWorker = new BackgroundWorker();
                    strConditionString = MakeConditionString(lstQueryCondition, lstCtrolAgent);
                    mWorker.DoWork += (s, de) =>
                    {
                        var tableInfo = CurrentApp.Session.ListPartitionTables.FirstOrDefault(
                        t => t.TableName == ConstValue.TABLE_NAME_RECORD && t.PartType == VoiceCyber.UMP.Common.TablePartType.DatetimeRange);
                        if (tableInfo == null)
                        {
                            foreach (CtrolAgent ctrolagent in lstCtrolAgent)
                            {
                                foreach (DateTimeSpliteAsDay datesplite in lstDateTimeSplitAsDay)
                                {
                                    string tableName = string.Format("{0}_{1}", ConstValue.TABLE_NAME_RECORD, CurrentApp.Session.RentInfo.Token);
                                        //ConstValue.TABLE_NAME_RECORD;
                                    QueryRecord2(scoreCon,scoreVal, strConditionString, ctrolagent, datesplite, tableName);
                                }
                            }
                        }
                        else 
                        {
                            foreach (CtrolAgent ctrolagent in lstCtrolAgent)
                            {
                                foreach (DateTimeSpliteAsDay datesplite in lstDateTimeSplitAsDay)
                                {
                                    string tableName = ReturnTableName(datesplite.StopDayTime);
                                    QueryRecord2(scoreCon,scoreVal, strConditionString, ctrolagent, datesplite, tableName);
                                }
                            }
                        }
                    };
                    mWorker.RunWorkerCompleted += (s, re) =>
                    {
                        mWorker.Dispose();
                        SetBusy(false, "");
                        mIsQueryContinue = false;
                        InitBasicOperations();
                        CreateOptButtons();
                        SetPageState();
                    };
                    mWorker.RunWorkerAsync();
                }
                catch (Exception ex)
                {
                   ShowException(ex.Message);
                }
            }

            #region 写操作日志
            CurrentApp.WriteOperationLog(S3103Consts.OPT_TASKASSIGNQUERY.ToString(), ConstValue.OPT_RESULT_SUCCESS, "");
            #endregion
        }

        private string ReturnTableName(DateTime stopTime)
        {
            string strTableName = string.Empty;
            strTableName = string.Format("{0}_{1}_{2}", ConstValue.TABLE_NAME_RECORD,
                            CurrentApp.Session.RentInfo.Token, stopTime.ToString("yyMM"));
            return strTableName;
        }

        /// <summary>
        /// 座席每天
        /// </summary>
        /// <param name="strConditionString"></param>
        /// <param name="ctrolagent"></param>
        /// <param name="datesplite"></param>
        /// <param name="tableName"></param>
        private void QueryRecord2(bool scoreCon,string scoreVal, string strConditionString, CtrolAgent ctrolagent, DateTimeSpliteAsDay datesplite, string tableName) 
        {
            try
            {
                if (!mIsQueryContinue) { return; }
                string strSql=string.Empty;
                strSql = ConditionStr2(strConditionString, ctrolagent, datesplite, tableName);
                CurrentApp.WriteLog("QueryRecord", string.Format("{0}", strSql));

                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3103Codes.GetRecordData;
                webRequest.ListData.Add(strSql);
                webRequest.ListData.Add(tableName);
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
                    if (item.ServiceAttitude != 0)
                    {
                        item.StrServiceAttitude = recordInfo.ServiceAttitude == 1 ? CurrentApp.GetLanguageInfo("3103T00169", "Good") : CurrentApp.GetLanguageInfo("3103T00170", "Bad");
                    }
                    if (item.ProfessionalLevel != 0)
                    {
                        item.StrProfessionalLevel = recordInfo.ProfessionalLevel == 1 ? CurrentApp.GetLanguageInfo("3103T00169", "Good") : CurrentApp.GetLanguageInfo("3103T00170", "Bad");
                    }
                    if (item.RecordDurationError != 0)
                    {
                        item.StrRecordDurationError = recordInfo.RecordDurationError == 2 ? CurrentApp.GetLanguageInfo("3103T00172", "Normal") : CurrentApp.GetLanguageInfo("3103T00173", "Abnormal");
                    }
                    if (item.RepeatCallIn != 0)
                    {
                        item.StrRepeatCallIn = recordInfo.RepeatCallIn == 2 ? CurrentApp.GetLanguageInfo("3103T00172", "Normal") : CurrentApp.GetLanguageInfo("3103T00181", "Repetition");
                    }
                    if (item.AbnormalScores != 0) 
                    {
                        item.StrAbnormalScores = recordInfo.AbnormalScores == 2 ? CurrentApp.GetLanguageInfo("3103T00172", "Normal") : CurrentApp.GetLanguageInfo("3103T00173", "Abnormal");
                    }
                    if (item.AfterEventProcessing != 0)
                    {
                        item.StrAfterEventProcessing = recordInfo.AfterEventProcessing == 2 ? CurrentApp.GetLanguageInfo("3103T00172", "Normal") : CurrentApp.GetLanguageInfo("3103T00173", "Abnormal");
                    }
                    if (item.SeatAgentSpeechAnomaly != 0)
                    {
                        item.StrSeatAgentSpeechAnomaly = recordInfo.SeatAgentSpeechAnomaly == 2 ? CurrentApp.GetLanguageInfo("3103T00172", "Normal") : CurrentApp.GetLanguageInfo("3103T00173", "Abnormal");
                    }
                    item.Background = GetRecordBackground(item);
                    mListAllRecordInfoItems.Add(item);
                    if (mRecordTotal < mPageSize)
                    {
                        AddNewRecord(item);
                    }
                    //currentRowID = item.RowID;
                    SetPageState();
                }
                //QueryRecord(strConditionString, currentRowID);
            }
            catch (Exception ex)
            {
               ShowException(ex.Message);
                #region 写操作日志
                CurrentApp.WriteOperationLog(S3103Consts.OPT_TASKASSIGNQUERY.ToString(), ConstValue.OPT_RESULT_FAIL, "");
                #endregion
            }
        }

        /// <summary>
        ///  座席每天
        /// </summary>
        /// <returns></returns>
        private string ConditionStr2(string strConditionString, CtrolAgent ctrolagent, DateTimeSpliteAsDay datesplite, string tableName)
        {
            string strSql = string.Empty;
            try
            {
                string tempStr = string.Empty;
                //查詢評分不允許被查出
                bool ShowQuery = false;
                //申訴過程中、申訴后不被允許查出
                bool ShowAppeal = false;
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)RequestCode.WSGetGlobalParamList;
                webRequest.ListData.Add("31");
                webRequest.ListData.Add("0");
                webRequest.ListData.Add("310102");
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
                    if (GlobalParamInfo.ParamID == 31010201)
                    {
                        ShowQuery = GlobalParamInfo.ParamValue == "310102010" ? true : false;//false 不允許
                    }
                    if (GlobalParamInfo.ParamID == 31010202)
                    {
                        ShowAppeal = GlobalParamInfo.ParamValue == "310102020" ? true : false;//false 不允許
                    }
                }

                if (!ShowAppeal)
                {
                    tempStr += string.Format("OR T8.C014 > 0");
                }
                if (!ShowQuery)
                {
                    tempStr = string.Format("AND (T8.C010 IN ( 1, 2, 3, 4, 5, 6) {0})", tempStr);
                }
                if (ShowQuery)
                {
                    tempStr = string.Format("AND (T8.C010 IN ( 3, 4, 5, 6) {0})", tempStr);
                }
                string tempTableSuffix = CurrentApp.Session.RentInfo.Token;
                if (tableName.Length > 14)
                {
                    tempTableSuffix = tableName.Substring(tableName.Length - 10, 10);
                }
                //ABCD拼接查询串
                string abcdTable = string.IsNullOrWhiteSpace(ABCDTempStr) ? string.Empty : string.Format(",T_31_054_{0} T354", tempTableSuffix);

                string agenOrex = string.Empty;
                if (S3103App.GroupingWay.Contains("A"))
                {
                    agenOrex = string.Format("T21.C039");
                }
                else if (S3103App.GroupingWay.Contains("R"))
                {
                    agenOrex = string.Format("T21.C058");
                }
                else if (S3103App.GroupingWay.Contains("E"))
                {
                    agenOrex = string.Format("T21.C042");
                }

                switch (CurrentApp.Session.DBType)//WHERE {0}  AND后面的条件是点了座席全部后的限制条件，不存在重复问题--备注
                { 
                    case 2:
                        strSql = string.Format("SELECT  TOP {1} * FROM (SELECT T21.*,{8} CASE WHEN EXISTS (SELECT * FROM T_31_022_{6} T22 WHERE T21.C002 = T22.C002) THEN '1' ELSE '0' END IsTask" +
                                               ",CASE WHEN EXISTS (SELECT * FROM T_31_008_{6} T8 WHERE T21.C002=T8.C002 {7}) THEN '1' ELSE '0' END ISSCORE" +
                                               " FROM {5} T21{9} WHERE {0}  AND T21.C005 >= '{2}' AND T21.C005 <= '{3}' AND {10} ='{4}' AND T21.C014<2 AND ISNULL(C104, -1)<=0) T WHERE T.IsTask = '0' AND T.ISSCORE='0' ORDER BY NEWID(),C001,C005,C039 "
                                , strConditionString, ctrolagent.EveryAgentNum, datesplite.StartDayTime.ToString("yyyy/MM/dd HH:mm:ss"), datesplite.StopDayTime.ToString("yyyy/MM/dd HH:mm:ss"), ctrolagent.AgentName, tableName
                                , CurrentApp.Session.RentInfo.Token, tempStr, ABCDTempStr, abcdTable, agenOrex);
                        break;
                    case 3://使用sys_guid() 有时会获取到相同的记录,即和前一次查询的结果集是一样的.有些说是和操作系统有关,在windows平台下正常,而在linux等平台下始终是相同不变的数据集;有些说是因为sys_guid()函数本身的问题,即sys_guid()会在查询上生成一个16字节的全局唯一标识符,这个标识符在绝大部分平台上由一个宿主标识符和进程或进程的线程标识符组成,这就是说,它很可能是随机的,但是并不表示一定是百分之百的这样.所以用dbms_random.value
                        strSql = string.Format("SELECT * FROM (SELECT T21.*,{8} CASE WHEN EXISTS (SELECT * FROM T_31_022_{6} T22 WHERE T21.C002 = T22.C002) THEN '1' ELSE '0' END IsTask" +
                                               ",CASE WHEN EXISTS (SELECT * FROM T_31_008_{6} T8 WHERE T21.C002=T8.C002 {7}) THEN '1' ELSE '0' END ISSCORE " +
                                               "FROM {5} T21{9} WHERE {0}  AND T21.C005 >= TO_DATE('{2}', 'YYYY-MM-DD HH24:MI:SS') AND " +
                                               "T21.C005 <= TO_DATE('{3}','YYYY-MM-DD HH24:MI:SS') AND {10} ='{4}' AND T21.C014<2 AND NVL(C104,-1)<=0  ORDER BY dbms_random.value,C001,C005,C039) T WHERE T.IsTask = '0' AND T.ISSCORE='0' AND ROWNUM <= {1}"
                            , strConditionString, ctrolagent.EveryAgentNum, datesplite.StartDayTime.ToString("yyyy/MM/dd HH:mm:ss"), datesplite.StopDayTime.ToString("yyyy/MM/dd HH:mm:ss"), ctrolagent.AgentName, tableName
                            , CurrentApp.Session.RentInfo.Token, tempStr, ABCDTempStr, abcdTable, agenOrex);
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


        /// <summary>
        /// 座席全部
        /// </summary>
        /// <param name="strConditionString"></param>
        /// <param name="currentRowID"></param>
        /// <param name="tableName"></param>
        private void QueryRecord(bool scoreCon,string scoreVal,string strConditionString, long currentRowID,string tableName)
        {
            try
            {
                if (!mIsQueryContinue) { return; }
                string strSql = string.Empty;
                strSql=ConditionStr(strConditionString, currentRowID, tableName);

                CurrentApp.WriteLog("QueryRecord", string.Format("{0}", strSql));

                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)S3103Codes.GetRecordData;
                webRequest.ListData.Add(strSql);
                webRequest.ListData.Add(tableName);
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
                    if (item.ServiceAttitude != 0)
                    {
                        item.StrServiceAttitude = recordInfo.ServiceAttitude == 1 ? CurrentApp.GetLanguageInfo("3103T00169", "Good") : CurrentApp.GetLanguageInfo("3103T00170", "Bad");
                    }
                    if (item.ProfessionalLevel != 0)
                    {
                        item.StrProfessionalLevel = recordInfo.ProfessionalLevel == 1 ? CurrentApp.GetLanguageInfo("3103T00169", "Good") : CurrentApp.GetLanguageInfo("3103T00170", "Bad");
                    }
                    if (item.RecordDurationError != 0)
                    {
                        item.StrRecordDurationError = recordInfo.RecordDurationError == 2 ? CurrentApp.GetLanguageInfo("3103T00172", "Normal") : CurrentApp.GetLanguageInfo("3103T00173", "Abnormal");
                    }
                    if (item.RepeatCallIn != 0)
                    {
                        item.StrRepeatCallIn = recordInfo.RepeatCallIn == 2 ? CurrentApp.GetLanguageInfo("3103T00172", "Normal") : CurrentApp.GetLanguageInfo("3103T00181", "Repetition");
                    }
                    if (item.AbnormalScores != 0) 
                    {
                        item.StrAbnormalScores = recordInfo.AbnormalScores == 2 ? CurrentApp.GetLanguageInfo("3103T00172", "Normal") : CurrentApp.GetLanguageInfo("3103T00173", "Abnormal");
                    }
                    if (item.AfterEventProcessing != 0) 
                    {
                        item.StrAfterEventProcessing = recordInfo.AfterEventProcessing == 2 ? CurrentApp.GetLanguageInfo("3103T00172", "Normal") : CurrentApp.GetLanguageInfo("3103T00173", "Abnormal");
                    }
                    if (item.SeatAgentSpeechAnomaly != 0) 
                    {
                        item.StrSeatAgentSpeechAnomaly = recordInfo.SeatAgentSpeechAnomaly == 2 ? CurrentApp.GetLanguageInfo("3103T00172", "Normal") : CurrentApp.GetLanguageInfo("3103T00173", "Abnormal");
                    }
                    item.Background = GetRecordBackground(item);
                    mListAllRecordInfoItems.Add(item);
                    if (mRecordTotal < mPageSize+1)
                    {
                        AddNewRecord(item);
                    }
                    currentRowID = recordInfo.RowID;
                    SetPageState();
                }
                QueryRecord(scoreCon, scoreVal,strConditionString, currentRowID, tableName);
            }
            catch (Exception ex)
            {
               ShowException(ex.Message);
                #region 写操作日志
                CurrentApp.WriteOperationLog(S3103Consts.OPT_TASKASSIGNQUERY.ToString(), ConstValue.OPT_RESULT_FAIL, "");
                #endregion
            }
        }

        /// <summary>
        ///  座席全部
        /// </summary>
        /// <returns></returns>
        private string ConditionStr(string strConditionString, long currentRowID, string tableName)
        {
            string strSql = string.Empty;
            try
            {
                string tempStr = string.Empty;
                //查詢評分不允許被查出
                bool ShowQuery = false;
                //申訴過程中、申訴后不被允許查出
                bool ShowAppeal = false;
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)RequestCode.WSGetGlobalParamList;
                webRequest.ListData.Add("31");
                webRequest.ListData.Add("0");
                webRequest.ListData.Add("310102");
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
                    if (GlobalParamInfo.ParamID == 31010201)
                    {
                        ShowQuery = GlobalParamInfo.ParamValue == "310102010" ? true : false;//false 不允許
                    }
                    if (GlobalParamInfo.ParamID == 31010202)
                    {
                        ShowAppeal = GlobalParamInfo.ParamValue == "310102020" ? true : false;//false 不允許
                    }
                }
                if (!ShowAppeal)
                {
                    tempStr += string.Format("OR T8.C014 > 0");
                }
                if (!ShowQuery)
                {
                    tempStr = string.Format("AND (T8.C010 IN ( 1, 2, 3, 4, 5, 6) {0})",tempStr);
                }
                if (ShowQuery)
                {
                    tempStr = string.Format("AND (T8.C010 IN ( 3, 4, 5, 6) {0})", tempStr);
                }
                string tempTableSuffix = CurrentApp.Session.RentInfo.Token;
                if (tableName.Length >14)
                {
                    tempTableSuffix = tableName.Substring(tableName.Length - 10, 10);
                }
                //ABCD拼接查询串
                string abcdTable = string.IsNullOrWhiteSpace(ABCDTempStr) ? string.Empty : string.Format(",T_31_054_{0} T354", tempTableSuffix);

                switch (CurrentApp.Session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT TOP {2} * FROM (SELECT T21.*,{6} CASE WHEN EXISTS (SELECT * FROM T_31_022_{4} T22 WHERE T21.C002 = T22.C002) THEN '1' ELSE '0' END IsTask," +
                                               "CASE WHEN EXISTS (SELECT * FROM T_31_008_{4} T8 WHERE T21.C002=T8.C002 {5}) THEN '1' ELSE '0' END ISSCORE " +
                                               "FROM {3}  T21{7} WHERE T21.C001 > {1} AND {0} AND T21.C014<2 ) T WHERE T.IsTask = '0' AND T.ISSCORE='0' ORDER BY C001,C005,C039 "
                            , strConditionString
                            , currentRowID
                            , mPageSize, tableName, CurrentApp.Session.RentInfo.Token, tempStr, ABCDTempStr, abcdTable);
                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM (SELECT T21.*,{6} CASE WHEN EXISTS (SELECT * FROM T_31_022_{4} T22 WHERE T21.C002 = T22.C002) THEN '1' ELSE '0' END IsTask," +
                                               "CASE WHEN EXISTS (SELECT * FROM T_31_008_{4} T8 WHERE T21.C002=T8.C002 {5}) THEN '1' ELSE '0' END ISSCORE " +
                                               "FROM {3} T21{7} WHERE {0} AND T21.C001 > {1} AND T21.C014<2 ORDER BY C001,C005,C039) T WHERE T.IsTask = '0' AND T.ISSCORE=0 AND ROWNUM <= {2}"
                        , strConditionString
                        , currentRowID
                        , mPageSize, tableName, CurrentApp.Session.RentInfo.Token, tempStr, ABCDTempStr, abcdTable);
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

        private void AddNewRecord(RecordInfoItem recordInfoItem)
        {
            Dispatcher.Invoke(new Action(() => mListCurrentRecordInfoItems.Add(recordInfoItem)));
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

        private string MakeConditionString(List<QueryConditionDetail> lstdetai, List<CtrolAgent> lstCtrolAgent) 
        {
            string StrSelect = string.Empty;
           //是否储存座席每天
            // 查询条件：C005 C039在前
            foreach(QueryConditionDetail querydetail in lstdetai)
            {
                if(querydetail.IsEnable == true)
                {
                    switch (querydetail.ConditionItemID)
                    {
                        case S3103Consts.CON_TIMEFROMTO:
                            {
                                if (CurrentApp.Session.DBType == 2)
                                {
                                    StrSelect += string.Format("T21.C005 >= '{0}' AND T21.C005 <= '{1}' AND ", querydetail.Value01, querydetail.Value02);
                                }
                                else if (CurrentApp.Session.DBType == 3)
                                {

                                    StrSelect += string.Format("T21.C005 >=TO_DATE ('{0}','YYYY-MM-DD HH24:MI:SS') AND T21.C005 <=TO_DATE( '{1}','YYYY-MM-DD HH24:MI:SS') AND ", querydetail.Value01, querydetail.Value02);
                                }
                            }
                            break;
                    }
                }
            } 
            //if (lstdetai.Count > 0 && lstCtrolAgent.Count() > 0)
            //{
            //    if (S3103App.GroupingWay.Contains("A"))
            //    {
            //        string strag = "T21.C039 IN (";
            //        foreach (CtrolAgent cag in lstCtrolAgent)
            //        {
            //            if (cag.AgentName != "N/A")
            //                strag += "'" + cag.AgentName + "',";
            //        }
            //        strag = strag.TrimEnd(',');
            //        strag += ") AND ";
            //        StrSelect += strag;
            //    }
            //    else if (S3103App.GroupingWay.Contains("R"))
            //    {
            //        string strag = "T21.C058 IN (";
            //        foreach (CtrolAgent Rex in lstCtrolAgent)
            //        {
            //            if (!string.IsNullOrWhiteSpace(Rex.AgentName))
            //                strag += "'" + Rex.AgentName + "',";
            //        }
            //        strag = strag.TrimEnd(',');
            //        strag += ") AND ";
            //        StrSelect += strag;
            //    }
            //    else if (S3103App.GroupingWay.Contains("E"))
            //    {
            //        string strag = "T21.C042 IN (";
            //        foreach (CtrolAgent Cex in lstCtrolAgent)
            //        {
            //            if (!string.IsNullOrWhiteSpace(Cex.AgentName))
            //                strag += "'" + Cex.AgentName + "',";
            //        }
            //        strag = strag.TrimEnd(',');
            //        strag += ") AND ";
            //        StrSelect += strag;
            //    }
            //}
            string abcdAgent = string.Empty;//储存abcd项选择的机构下的座席
            foreach(QueryConditionDetail querydetail in lstdetai)
            {
                if(querydetail.IsEnable == true)
                {
                    switch(querydetail.ConditionItemID)
                    {
                        //case S3103Consts.CON_TIMEFROMTO:
                        //    {
                        //        if(S3103App.Session.DBType ==2)
                        //        {

                        //            StrSelect += string.Format("C005 >= '{0}' AND C005 <= '{1}' AND ", querydetail.Value01, querydetail.Value02);
                        //        }
                        //        else if (S3103App.Session.DBType==3) 
                        //        {

                        //            StrSelect += string.Format("C005 >=TO_DATE ('{0}','YYYY-MM-DD HH24:MI:SS') AND C005 <=TO_DATE( '{1}','YYYY-MM-DD HH24:MI:SS') AND ", querydetail.Value01, querydetail.Value02);
                        //        }
                        //    }
                        //    break;
                        case S3103Consts.CON_DURATIONFROMTO: 
                            {
                                //暂时没有
                                StrSelect += string.Format("T21.C012 >= {0} AND T21.C012 <= {1} AND ", querydetail.Value01,
                                            querydetail.Value02);
                            }
                            break;
                        case S3103Consts.CON_EXTENSION_MULTITEXT :
                            {
                                if (querydetail.IsLike == false)
                                {
                                    StrSelect +=
                                      string.Format("T21.C042 IN (SELECT C011 FROM T_00_901 WHERE C001 = {0}) AND "
                                          , querydetail.Value01);
                                }
                                else 
                                {
                                    if (querydetail.Value01.Length > 0)
                                    {
                                        StrSelect += string.Format("T21.C042 LIKE '%{0}%'  AND ", querydetail.Value01);
                                    }
                                }
                              
                            }
                            break;
                        case S3103Consts.CON_CALLERID_LIKETEXT:
                            {
                                if (querydetail.IsLike == false)
                                {
                                    StrSelect += string.Format("T21.C040 IN (SELECT C011  FROM T_00_901 WHERE C001={0}) AND ", querydetail.Value01);
                                }
                                else 
                                {
                                    if(querydetail.Value01.Length>0)
                                    {
                                        StrSelect += string.Format("T21.C040 LIKE '%{0}%'  AND ", querydetail.Value01);
                                    }
                                }
                            }
                            break;
                        case S3103Consts.CON_CALLEDID_LIKETEXT:
                            {
                                if (querydetail.IsLike == false)
                                {
                                    StrSelect += string.Format("T21.C041 IN (SELECT C011  FROM T_00_901 WHERE C001={0}) AND ", querydetail.Value01);
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
                        case S3103Consts.CON_RECORDREFERENCE_MULTITEXT:
                            {
                                if (querydetail.IsLike == false)
                                {
                                    StrSelect += string.Format("T21.C002 IN (SELECT C011  FROM T_00_901 WHERE C001={0} ) AND ", querydetail.Value01);
                                }
                                else
                                {
                                    StrSelect += string.Format("T21.C002 LIKE '%{0}%'  AND ", querydetail.Value01);
                                }
                           
                            }
                            break;
                        case S3103Consts.CON_AGENT_MULTITEXT://座席、分机不能模糊查询
                            {
                                string tempColumns = string.Empty;
                                if (S3103App.GroupingWay.Contains("A"))
                                {
                                    tempColumns = string.Format("T21.C039 ");
                                }
                                else if (S3103App.GroupingWay.Contains("R"))
                                {
                                    tempColumns = string.Format("T21.C058 ");
                                }
                                else if (S3103App.GroupingWay.Contains("E"))
                                {
                                    tempColumns = string.Format("T21.C042 ");
                                }
                                StrSelect += string.Format(" {0} IN  (SELECT C011  FROM T_00_901 WHERE C001={1}) AND ",tempColumns, querydetail.Value01);
                            }
                            break;
                        case S3103Consts.CON_CTIREFERENCE_MULTITEXT: 
                            {
                                if (querydetail.IsLike == false)
                                {
                                    StrSelect += string.Format("T21.C047 IN  (SELECT C011  FROM T_00_901 WHERE C001={0}) AND ", querydetail.Value01);
                                }
                                else
                                {
                                    StrSelect += string.Format("T21.C047 LIKE '%{0}%' AND ", querydetail.Value01);
                                }   
                            }
                            break;
                        case S3103Consts.CON_CHANNELID_MULTITEXT: 
                            {
                                if (querydetail.IsLike == false)
                                {
                                    StrSelect += string.Format("T21.C038 IN  (SELECT C011  FROM T_00_901 WHERE C001={0}) AND ", querydetail.Value01);
                                }
                                else
                                {
                                    StrSelect += string.Format("T21.C038 LIKE '%{0}%' AND ", querydetail.Value01);
                                }   
                            }
                            break;
                        case S3103Consts.CON_DIRECTION: 
                            {
                                StrSelect += string.Format("T21.C045 ={0} AND ", querydetail.Value01);
                            }
                            break;
                        case S3103Consts.CON_KEYWORD:
                            {
                                StrSelect += string.Format("T21.C002 IN (SELECT DISTINCT C002 FROM T_51_009_00000 WHERE C008 IN ({0})) AND", querydetail.Value02);
                                //if (querydetail.IsEnable == false)
                                //{
                                //    StrSelect += string.Format("T21.C002 IN (SELECT DISTINCT C002 FROM T_51_009_00000 WHERE C008 IN ({0})) AND", querydetail.Value02);
                                //}
                                //else 
                                //{
                                //    StrSelect += string.Format("T21.C002 LIKE '% ({0}) %' AND ", querydetail.Value02);
                                //}
                            }
                            break;

                            //ABCD
                        case S3103Consts.CON_ServiceAttitude:
                        case S3103Consts.CON_ProfessionalLevel:
                        case S3103Consts.CON_RecordDurationError:
                        case S3103Consts.CON_RepeatCallIn:
                        case S3103Consts.CON_ACSpeExceptProportion:
                        case S3103Consts.CON_AfterDealDurationExcept:
                        case S3103Consts.CON_ExceptionScore:
                            string column=string.Empty;
                            string sqltemp=string.Empty;
                            switch (querydetail.Value02.Length)//判断T_31_054的列值
                            {
                                case 1:
                                    column=string.Format("C00{0}",querydetail.Value02);
                                    break;
                                case 2:
                                    column=string.Format("C0{0}",querydetail.Value02);
                                    break;
                                case 3:
                                    column=string.Format("C{0}",querydetail.Value02);
                                    break;
                                default:
                                    continue;//跳出当前循环
                            }
                            switch (querydetail.Value01)//组合条件
                            {
                                case "0":
                                    sqltemp=string.Format("{0} IN('1','2')",column);
                                    break;
                                case "1":
                                case "2":
                                     sqltemp=string.Format("{0}='{1}'",column,querydetail.Value01);
                                    break;
                            }
                            ABCDTempStr = string.Format("T354.{0} AS {1},", column, querydetail.Value04);
                            StrSelect += string.Format(" T354.{0} AND", sqltemp);
                            abcdAgent += string.Format("{0},", querydetail.Value03);
                            break;
                    }//swith.end
                }//if.end
            }//foreach.end
            string columTemp = string.Empty;//判断列名
            if (S3103App.GroupingWay.Contains("A"))
            {
                columTemp = string.Format("C039");
            }
            else if (S3103App.GroupingWay.Contains("R"))
            {
                columTemp = string.Format("C058");
            }
            else if (S3103App.GroupingWay.Contains("E"))
            {
                columTemp = string.Format("C042");
            }
            if (!string.IsNullOrWhiteSpace(abcdAgent))
            {
                abcdAgent = DropReptitionStr(abcdAgent);
                StrSelect += string.Format(" T21.{0} IN({1}) AND T21.C002 = T354.C201", columTemp, abcdAgent);
            }
            if (StrSelect.Length > 4 && StrSelect.Substring(StrSelect.Length - 4) == "AND ")
                StrSelect = StrSelect.Substring(0, StrSelect.Length - 4);
            return StrSelect;
        }

        /// <summary>
        /// 去掉重复的座席
        /// </summary>
        private string DropReptitionStr(string oldStr)
        {
            string newStr = string.Empty;
            if (string.IsNullOrWhiteSpace(oldStr)) { return string.Empty; }
            try
            {
                string[] arrInfo = oldStr.Split(',');
                foreach (string tempAgent in arrInfo)
                {
                    if (!newStr.Contains(tempAgent))
                    {
                        newStr += string.Format("{0},",tempAgent);
                    }
                }
                newStr = newStr.Trim(',');
            }
            catch (Exception)
            {
                
            }
            return newStr;
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
                //    panel.Title = S3103App.GetLanguageInfo("3103T00072", "Memo");
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
                //toolItem.Display = S3103App.GetLanguageInfo("FO3103013", "Save Layout");
                //toolItem.Tip = S3103App.GetLanguageInfo("FO3103013", "Save Layout");
                //toolItem.Icon = "Images/savelayout.png";
                //btn = new Button();
                //btn.Click += ToolButton_Click;
                //btn.DataContext = toolItem;
                //btn.SetResourceReference(StyleProperty, "ToolBarButtonStyle");
                //optInfo = S3103App.ListOperationInfos.FirstOrDefault(o => o.ID == S3103Consts.OPT_SAVELAYOUT);
                //if (optInfo != null)
                //{
                //    PanelToolButton.Children.Add(btn);
                //}

                //toolItem = new ToolButtonItem();
                //toolItem.Name = "BT" + "ResetLayout";
                //toolItem.Display = S3103App.GetLanguageInfo("FO3103012", "Reset Layout");
                //toolItem.Tip = S3103App.GetLanguageInfo("FO3103012", "Reset Layout");
                //toolItem.Icon = "Images/resetlayout.png";
                //btn = new Button();
                //btn.Click += ToolButton_Click;
                //btn.DataContext = toolItem;
                //btn.SetResourceReference(StyleProperty, "ToolBarButtonStyle");
                //optInfo = S3103App.ListOperationInfos.FirstOrDefault(o => o.ID == S3103Consts.OPT_RESETLGYOUT);
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
                if (string.IsNullOrWhiteSpace(cols) ) 
                {
                    return;
                }
                WriteLog.CreatColumnsInfoXml(string.Format("{0}T", CurrentApp.Session.UserID.ToString()), lans, cols, width);
                ShowInformation(CurrentApp.GetLanguageInfo("3103T00128", "Save layout end."));
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
                optInfo = S3103App.ListOperationInfos.FirstOrDefault(o => o.ID == S3103Consts.OPT_PLAYRECORD);
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
                //panelItem.Title = S3103App.GetLanguageInfo("3103T00072", "Memo");
                //panelItem.Icon = "Images/memobox.png";
                //panelItem.CanClose = true;
                //optInfo = S3103App.ListOperationInfos.FirstOrDefault(o => o.ID == S3103Consts.OPT_MEMORECORD);
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

        void TxtPage_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.Enter)
                {
                    if (mPageCount > 0)
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
                    else
                        TxtPage.Text = "0";
                }
            }
            catch (Exception ex)
            {
               ShowException(ex.Message);
            }
        }

        private void BindCommands()
        {
            CommandBindings.Add(
                new CommandBinding(URMainPageCommands.TaskAssignQueryCommand,
                    TaskAssignQuery_Executed,
                    (s, e) => e.CanExecute = true));
            CommandBindings.Add(
             new CommandBinding(URMainPageCommands.SaveToTaskCommand,
                    SaveToTask_Executed,
                    (s, e) => e.CanExecute = true));
            CommandBindings.Add(
             new CommandBinding(URMainPageCommands.AddToTaskCommand,
                    AddToTask_Executed,
                    (s, e) => e.CanExecute = true));

        }

        private void CreateOptButtons()
        {
            PanelBasicOpts.Children.Clear();
            Button btn;
            foreach (OperationInfo opt in S3103App.ListOperationInfos)
            {
                if (opt.ID == S3103Consts.OPT_TASKASSIGNQUERY)
                {
                    btn = new Button();
                    btn.Click += BasicOpt_Click;
                    btn.DataContext = opt;
                    btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                    btn.IsEnabled = !mIsQueryContinue;
                    PanelBasicOpts.Children.Add(btn);
                }
                if (opt.ID == S3103Consts.OPT_ADDTOTASK)
                {
                    btn = new Button();
                    btn.Click += BasicOpt_Click;
                    btn.DataContext = opt;
                    btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                    PanelBasicOpts.Children.Add(btn);
                }
                if (opt.ID == S3103Consts.OPT_SAVETOTASK)
                {
                    btn = new Button();
                    btn.Click += BasicOpt_Click;
                    btn.DataContext = opt;
                    btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                    PanelBasicOpts.Children.Add(btn);
                }
            }
        }

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
                        case S3103Consts.OPT_TASKASSIGNQUERY:
                            QueryRecordInTaskAssign();
                            break;
                        case S3103Consts.OPT_SAVETOTASK:
                            SaveToTask();
                            break;
                        case S3103Consts.OPT_ADDTOTASK:
                            {
                                SetBusy(true,string.Empty);
                                AddToTask();
                                SetBusy(false, string.Empty);
                            }
                            break;
                    }
                }
            }
        }

        private void SaveToTask_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            SaveToTask();
        }

        private void AddToTask_Executed(object sender, ExecutedRoutedEventArgs e)
        {
        }
        private void TaskAssignQuery_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            QueryRecordInTaskAssign();
        }

        private void SaveToTask()
        {
            if (mListCurrentRecordInfoItems.Where(p => p.IsCheck == true).Count() == 0)
            {
                ShowInformation(CurrentApp.GetLanguageInfo("3103T00121", "Please select one record at least."));
                return;
            }

            PopupPanel.Title = CurrentApp.GetLanguageInfo("3103T00117", "New Task");
            AssignToQA Task2QA = new AssignToQA();
            Task2QA.CurrentApp = CurrentApp;
            Task2QA.PageParent = this;
            PopupPanel.Content = Task2QA;
            PopupPanel.IsOpen = true;
        }

        private void AddToTask()
        {
            string messageName = CurrentApp.GetLanguageInfo("FO3103009", "Add To Task");
            if (mListAllRecordInfoItems.Count == 0)
            {
                string temp = CurrentApp.GetLanguageInfo("3103T00108", "No Record");
                MessageBox.Show(temp, messageName);
                return;
            }
            UserTasksInfoShow currentTask = (UserTasksInfoShow)LvTaskData.SelectedItem;
            if(currentTask ==null)
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
                        taskinfodetail.TaskType = "1";
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

                    webRequest.Data =  currentTask.TaskID.ToString();
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
                            webRequest.ListData.Add("1");
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
                            webRequest.ListData.Add("1");
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
                    strLog += string.Format("{0} {1}", Utils.FormatOptLogString("FO3103009"), currentTask.TaskName);
                    CurrentApp.WriteOperationLog(S3103Consts.OPT_TASKASSIGN.ToString(), ConstValue.OPT_RESULT_SUCCESS, strLog);
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
                    ListCurrentUserTasks.Remove(ListCurrentUserTasks.FirstOrDefault(p=>p.TaskID == currentTask.TaskID));
                    ListCurrentUserTasks.Add(currentTask);
                }
            }
            catch (Exception ex)
            {
                CurrentApp.WriteOperationLog(S3103Consts.OPT_ADDTOTASK.ToString(), ConstValue.OPT_RESULT_FAIL, "");
                MessageBox.Show(ex.Message, CurrentApp.AppName);

            }

            return;

        }

        //具体的查询方法
        private void QueryRecordInTaskAssign()
        {
            PopupPanel.Title = CurrentApp.GetLanguageInfo("FO3103007", "Query Info");
            QueryRecordInTask queryRecordInTask = new QueryRecordInTask();
            queryRecordInTask.CurrentApp = CurrentApp;
            queryRecordInTask.PageParent = this;
            queryRecordInTask.mRootItem = mRootItem;
            PopupPanel.Content = queryRecordInTask;
            PopupPanel.IsOpen = true;
        }

        private void btn2TaskTrack_Click(object sender, RoutedEventArgs e)
        {
            S3103App.NativePageFlag = false;
            TaskTrack taskTrackView=new TaskTrack();
            CurrentApp.CurrentView = taskTrackView;
            taskTrackView.PageName = "TaskTrack";
            var temp = CurrentApp as S3103App;
            if (temp != null)
            {
                temp.InitMainView(taskTrackView);
                //temp.InitMainView(1);
            }
        }
        private void btn2TaskReCheck_Click(object sender, RoutedEventArgs e)
        {
            S3103App.NativePageFlag = false;
            DoubleTaskAssign doubleTaskView = new DoubleTaskAssign();
            doubleTaskView.PageName = "DoubleTaskAssign";
            var temp = CurrentApp as S3103App;
            if (temp != null)
            {
                temp.InitMainView(doubleTaskView);
                //temp.InitMainView(2);
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
                    item.Value = S3103App.DecryptString(mCurrentRecordInfoItem.CallerID);
                    mListCallInfoPropertyItems.Add(item);
                    item = new CallInfoPropertyItem();
                    item.Name = CurrentApp.GetLanguageInfo("3103T00069", "Called ID");
                    item.Value =S3103App.DecryptString( mCurrentRecordInfoItem.CalledID);
                    mListCallInfoPropertyItems.Add(item);
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
            var optInfo = S3103App.ListOperationInfos.FirstOrDefault(o => o.ID == S3103Consts.OPT_TASKASSIGN);
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
                 BorderPlayBox.Focus();
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

                     //mUCPlayBox = new UCPlayBox();
                     mUCPlayBox = new UCKeyWord();
                     mUCPlayBox.CurrentApp = CurrentApp;
                     mUCPlayBox.PlayStopped += mUCPlayBox_PlayStopped;
                     mUCPlayBox.ParentPage1 = this;
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
                        //mUCPlayBox = new UCPlayBox();
                        mUCPlayBox = new UCKeyWord();
                        mUCPlayBox.CurrentApp = CurrentApp;
                        mUCPlayBox.PlayStopped += mUCPlayBox_PlayStopped;
                        mUCPlayBox.ParentPage1 = this;
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
        
        #region 页头命令

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

        public override void ChangeLanguage()
        {
            base.ChangeLanguage();
            try
            {
                CurrentApp.AppTitle = CurrentApp.GetLanguageInfo(string.Format("FO{0}", CurrentApp.ModuleID),
                   "Task Management");
                //Operation
                for (int i = 0; i < S3103App.ListOperationInfos.Count; i++)
                {
                    S3103App.ListOperationInfos[i].Display = CurrentApp.GetLanguageInfo(string.Format("FO{0}", S3103App.ListOperationInfos[i].ID),
                        S3103App.ListOperationInfos[i].ID.ToString());
                }
                CreateOptButtons();
                InitTaskRecordColumns();
                InitTaskDetailColumns();
                CreateCallInfoItems();
                InitOperationButton();
                

                #region panel
                var panel = GetPanleByContentID("PanelObjectBox");
                if (panel != null)
                {
                    panel.Title = CurrentApp.GetLanguageInfo("3103T00057", "Allot Task List");
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
            catch (Exception ex)
            {
            }
        }

        #region ohter
        private LayoutAnchorable GetPanleByContentID(string contentID)
        {
            var panel =
                PanelManager.Layout.Descendents()
                    .OfType<LayoutAnchorable>()
                    .FirstOrDefault(p => p.ContentId == contentID);
            return panel;
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
                    string uri = string.Format("/UMPS3103;component/Themes/{0}/{1}",
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
