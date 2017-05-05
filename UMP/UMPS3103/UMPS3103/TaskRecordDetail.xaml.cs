// ***********************************************************************
// Assembly         : UMPS3103
// Author           : Luoyihua
// Created          : 11-06-2014
//
// Last Modified By : Luoyihua
// Last Modified On : 11-10-2014
// ***********************************************************************
// <copyright file="TaskRecordDetail.xaml.cs" company="VoiceCodes">
//     Copyright (c) VoiceCodes. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UMPS3103.Models;
using UMPS3103.Wcf31031;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common31031;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;
using VoiceCyber.Wpf.AvalonDock.Layout;

namespace UMPS3103
{
    /// <summary>
    /// TaskRecordDetail.xaml 的交互逻辑
    /// </summary>
    public partial class TaskRecordDetail 
    {
        /// <summary>
        /// 父页面
        /// </summary>
        public TaskTrack PageParent;
        /// <summary>
        /// 修改任务true  评分false
        /// </summary>
        public static bool IsModify = false;
        /// <summary>
        /// 选中的源任务
        /// </summary>
        public static UserTasksInfoShow SelectTask;
        public BasicScoreSheetItem selScoreSheet;
        public static ObservableCollection<TaskInfoDetail> ListCurrentTaskDetail = new ObservableCollection<TaskInfoDetail>();
        //创建按钮
        private ObservableCollection<OperationInfo> mListBasicOperations;
        //异步线程
        private BackgroundWorker mWorker;
        /// <summary>
        /// 选中的需要移动|删除的任务录音
        /// </summary>
        public TaskInfoDetail currentSelRecord;

        private bool mShowViewScore;
        /// <summary>
        /// true 为查看任务评分的成绩、false 为没有在任务中评分
        /// </summary>
        public bool mViewScore;
        private bool mScore;

        /// <summary>
        /// 录音信息
        /// </summary>
        public RecordInfoItem recorditem ;

        /// <summary>
        /// 获取该录音所做的历史记录
        /// </summary>
        public string recordOptHistory = string.Empty;
        public TaskRecordDetail()
        {
            InitializeComponent();

            mShowViewScore = false;
            mViewScore = false;
            mScore = true;
            mListBasicOperations = new ObservableCollection<OperationInfo>();

            LVTaskDetail.ItemsSource = ListCurrentTaskDetail;
            LVTaskDetail.SelectionChanged += LVTaskDetail_SelectedItemChanged;
            LVTaskDetail.MouseDoubleClick += LVTaskDetail_MouseDoubleClick;
        }

        protected override void Init() //用了init()方法之后，不能在用this.load的方法了
        {
            try
            {
                PageName = "UMP TaskRecordDetail";
                StylePath = "UMPS3103/MainPageStyle.xaml";
                base.Init();
                if (CurrentApp != null)
                {
                    CurrentApp.SendLoadedMessage();
                }

                InitTaskDetailColumns();
                InitTaskDetail();
                CreateOperationButton();
                SetBusy(true, CurrentApp.GetMessageLanguageInfo("001", string.Format("Loading...")));

                mWorker = new BackgroundWorker();
                mWorker.DoWork += (s, de) =>
                {
                };
                mWorker.RunWorkerCompleted += (s, re) =>
                {
                    mWorker.Dispose();
                    SetBusy(false, "");
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


        /// <summary>
        /// 加载当前任务的任务详情
        /// </summary>
        public void InitTaskDetail()
        {
            try
            {
                try
                {
                    ListCurrentTaskDetail.Clear();
                }
                catch { }
                string isSeptable = "0";//0：不分表  1：分表
                var tableInfo =
                    CurrentApp.Session.ListPartitionTables.FirstOrDefault(
                        t => t.TableName == ConstValue.TABLE_NAME_RECORD && t.PartType == TablePartType.DatetimeRange);
                if (tableInfo != null)
                {
                    isSeptable = "1";
                }
                string tempStr = string.Empty;
                //判斷當前任務類型
                if (SelectTask.TaskType == 1 || SelectTask.TaskType == 2)
                {
                    tempStr = string.Format("T08.C010='3' OR T08.C010='4'");
                }
                if (SelectTask.TaskType == 3 || SelectTask.TaskType == 4) 
                {
                    tempStr = string.Format("T08.C010='5' OR T08.C010='6'");
                }

                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S3103Codes.GetTaskRecordByTaskID;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(SelectTask.TaskID.ToString());
                webRequest.ListData.Add(SelectTask.TaskName);
                webRequest.ListData.Add(isSeptable);
                webRequest.ListData.Add(tempStr);//任務類型
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
                        OperationReturn optReturn = XMLHelper.DeserializeObject<TaskInfoDetail>(webReturn.ListData[i]);
                        if (!optReturn.Result)
                        {
                           ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                            return;
                        }
                        TaskInfoDetail taskdetail = optReturn.Data as TaskInfoDetail;
                        if (taskdetail != null && !ListCurrentTaskDetail.Contains(taskdetail))
                        {
                            //AllotTypeName:1任务分配过来，2从其它任务移动过来的 3推荐录音 4申诉审批到复检
                            taskdetail.AllotTypeName = GetAllotTypeName(taskdetail.AllotType);
                            taskdetail.IsLock = taskdetail.IsLock == "Y" ? CurrentApp.GetLanguageInfo("3103T00022", "Yes") : CurrentApp.GetLanguageInfo("3103T00023", "No");
                            taskdetail.strDirection = taskdetail.Direction == 1 ? CurrentApp.GetLanguageInfo("3103T00089", "Call In") : CurrentApp.GetLanguageInfo("3103T00090", "Call Out");
                            taskdetail.CalledID = S3103App.DecryptString(taskdetail.CalledID);
                            taskdetail.CallerID = S3103App.DecryptString(taskdetail.CallerID);
                            if (string.IsNullOrWhiteSpace(taskdetail.AgtOrExtID))
                            {
                                taskdetail.AgtOrExtID = AgentAndUserFullName(taskdetail.AgtOrExtName, 1);
                            }
                            taskdetail.AgentFullName = AgentAndUserFullName(taskdetail.AgtOrExtID,0);
                            ListCurrentTaskDetail.Add(taskdetail);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
               ShowException(ex.Message);
            }
        }
              
        /// <summary>
        /// 0，匹配全名；1，匹配ID
        /// </summary>
        private string AgentAndUserFullName(string AuID,int type)
        { 
            try
            {
                switch (type)
                {
                    case 0:
                        long tempID = Convert.ToInt64(AuID);
                        string AuFullName = string.Empty;
                        var temp = S3103App.mListAuInfoItems.FirstOrDefault(m => m.ID == tempID);
                        if (temp != null)
                        {
                            AuFullName = temp.FullName;
                        }
                        if (string.IsNullOrWhiteSpace(AuFullName))
                        {
                            AuFullName = AuID;
                        }
                        return AuFullName;
                    case 1:
                        string AuId = string.Empty;
                        var temp1 = S3103App.mListAuInfoItems.FirstOrDefault(m => m.Name == AuID);
                        if (temp1 != null)
                        {
                            AuId = temp1.ID.ToString();
                        }
                        if (string.IsNullOrWhiteSpace(AuId))
                        {
                            AuId= AuID;
                        }
                        return AuId;
                    default:
                        return AuID;
                }
            }
            catch (Exception ex)
            {
                return AuID;
            }
        }

        public string GetAllotTypeName(int? type)
        {
            string typename = "";
            if (type != null)
            { 
                switch (type)
                {
                    case 1:
                        typename = CurrentApp.GetLanguageInfo("3103T00134", "Assign Task");
                        break;
                    case 2:
                        typename = CurrentApp.GetLanguageInfo("3103T00135", "Edit Task");
                        break;
                    case 3:
                        typename = CurrentApp.GetLanguageInfo("3103T00136", "Recommended ");
                        break;
                    case 4:
                        typename = CurrentApp.GetLanguageInfo("3103T00190", "Appeal To ReCheck ");
                        break;
                    default:
                        break;
                }
            }
            return typename;
        }

        public void LVTaskDetail_SelectedItemChanged(object sender, SelectionChangedEventArgs e)
        {
            TaskInfoDetail focuseitem = LVTaskDetail.SelectedItem as TaskInfoDetail;

            recorditem = new RecordInfoItem();
            recorditem.VoiceIP = focuseitem.VoiceIP;
            recorditem.RecordReference = focuseitem.RecoredReference.ToString();
            recorditem.EncryptFlag = focuseitem.EncryptFlag;
            recorditem.MediaType = focuseitem.MediaType;
            recorditem.RecordInfo = new RecordInfo();
            recorditem.RecordInfo.EncryptFlag = focuseitem.EncryptFlag;
            recorditem.RecordInfo.MediaType = focuseitem.MediaType;
            recorditem.RecordInfo.RecordReference = focuseitem.RecoredReference.ToString();
            recorditem.RecordInfo.VoiceIP = focuseitem.VoiceIP;
            recorditem.RecordInfo.ChannelID = focuseitem.ChannelID;
            recorditem.RecordInfo.WaveFormat = focuseitem.WaveFormat;
            recorditem.RecordInfo.StartRecordTime = focuseitem.StartRecordTime;
            recorditem.RecordInfo.SerialID = focuseitem.RecoredReference;
            recorditem.RecordInfo.RowID = focuseitem.RowID;

            WebRequest webRequest = new WebRequest();
            webRequest.Code = (int)S3103Codes.GetRecordHistoryOpt;
            webRequest.Session = CurrentApp.Session;
            webRequest.ListData.Add(recorditem.RecordReference);
            webRequest.ListData.Add(SelectTask.TaskType.ToString());
            //Service31031Client client = new Service31031Client();
            Service31031Client client = new Service31031Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31031"));
            //WebHelper.SetServiceClient(client);
            WebReturn webReturn = client.UMPTaskOperation(webRequest);
            client.Close();
            recordOptHistory = webReturn.Data;
            //有查看分数权限 && 记录有分数
            if (focuseitem.TaskScore != null && S3103App.ListOperationInfos.Where(p => p.ID == S3103Consts.OPT_VIEWSCORE).ToList().Count > 0)
            {
                mScore = false;
                //没有查看按钮
                if (mListBasicOperations.Where(p => p.ID == 3103018).ToList().Count == 0)
                {
                    mShowViewScore = true;
                    CreateOperationButton();
                }
            }
            else
            {
                mScore = true;
                mShowViewScore = false;
                CreateOperationButton();
            }
        }
        private void LVTaskDetail_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            mViewScore = mScore == false ? true : false;
            if (mListBasicOperations.Where(p => p.ID == S3103Consts.OPT_TASKRECORDSCORE).Count() > 0 || mListBasicOperations.Where(p => p.ID == S3103Consts.OPT_VIEWSCORE).Count() > 0) { TaskDetailOptScore(); }            

            //TaskInfoDetail item = LVTaskDetail.SelectedItem as TaskInfoDetail;
            //if (item == null)
            //    return;
            //try
            //{

            //    mUCPlayBox = new UCPlayBox();
            //    mUCPlayBox.TaskPage1 = this;
            //    mUCPlayBox.ListEncryptInfo = App.mListRecordEncryptInfos;
            //    //mUCPlayBox.ListUserSettingInfos = mListSettingInfos;
            //    mUCPlayBox.ListSftpServers = App.mListSftpServers;
            //    mUCPlayBox.ListDownloadParams = App.mListDownloadParams;
            //    mUCPlayBox.Service03Helper = App.mService03Helper;
            //    mUCPlayBox.RecordInfoItem = recorditem;
            //    mUCPlayBox.IsAutoPlay = true;
            //    mUCPlayBox.Play(true);
            //    BorderPlayer.Child = mUCPlayBox;

            //    mUCRecordMemo = new UCRecordMemo();
            //    mUCRecordMemo.RecordInfoItem = item;
            //    BorderMemo.Child = mUCRecordMemo;

            //    #region 写操作日志
            //    string strLog = string.Format("{0} {1}", Utils.FormatOptLogString("3103T00025"), item.RecoredReference);
            //    App.WriteOperationLog(S3103Consts.OPT_PLAYRECORD.ToString(), ConstValue.OPT_RESULT_SUCCESS, strLog);
            //    #endregion
            //}
            //catch (Exception ex)
            //{
            //    App.ShowExceptionMessage(ex.Message);
            //}
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

        /// <summary>
        ///初始化任务列
        /// </summary>
        private void InitTaskDetailColumns()
        {
            //string[] lens = "3103T00025,3103T00001,3103T00028,3103T00029,3103T00030,3103T00055,".Split(',');
            string[] lens = "3103T00025,3103T00001,3103T00028,3103T00029,3103T00030,3103T00055,3103T00059,3103T00066,3103T00178,3103T00179,3103T00068,3103T00069,3103T00067".Split(',');
            //string[] cols = "RecoredReference,TaskName,LockTime,AllotTypeName,FromTaskName,TaskScore".Split(',');
            string[] cols = "RecoredReference,TaskName,LockTime,AllotTypeName,FromTaskName,TaskScore,StartRecordTime,Duration,AgtOrExtName,AgentFullName,CallerID,CalledID,strDirection".Split(',');
            int[] colwidths = { 140, 120, 150, 100, 120,80,150,100,80,80,100,100,80 };
            //GridView columngv = new GridView();
            GridView ColumnGridView = new GridView();
            GridViewColumn gvc;
            for (int i = 0; i < cols.Length; i++)
            {
                //DataTemplate CellTemplate = (DataTemplate)Resources[cols[i]];
                //SetColumnGridView(cols[i], ref columngv, lens[i], cols[i], CellTemplate, colwidths[i]);
                gvc = new GridViewColumn();
                gvc.Header = CurrentApp.GetLanguageInfo(lens[i], cols[i]);
                gvc.Width = colwidths[i];
                if (cols[i] == "StartRecordTime")
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
            LVTaskDetail.View = ColumnGridView;
        }
        
        /// <summary>
        /// 生成菜单
        /// </summary>
        private void CreateOperationButton()
        {
            mListBasicOperations.Clear();
            OperationInfo item1 = new OperationInfo();
            item1.ID = 31030;
            item1.ParentID = 1102;
            item1.SortID = 0;
            item1.Icon = "Images/back.png";
            item1.Display = CurrentApp.GetLanguageInfo("3103T00024", "Return");
            item1.Description = null;
            mListBasicOperations.Add(item1);

            if (mShowViewScore)
            {
                OperationInfo item2 = new OperationInfo();
                item2.ID = 3103018;
                item2.ParentID = 1102;
                item2.SortID = 0;
                item2.Icon = "Images/objectbox.png";
                item2.Display = CurrentApp.GetLanguageInfo("FO3103018", "View Score");
                item2.Description = null;
                mListBasicOperations.Add(item2);
            }
          
            if (IsModify)//调整任务页面
            {
                List<OperationInfo> lstModify = S3103App.ListOperationInfos.Where(p => p.ID == S3103Consts.OPT_MODIFYTASKFINISHTIME).ToList();
                if (lstModify.Count > 0)//有修改任务时间权
                {
                    OperationInfo item = new OperationInfo();
                    item.ID = S3103Consts.OPT_MODIFYTASKFINISHTIME;
                    item.ParentID = 1102;
                    item.SortID = 0;
                    item.Icon = "Images/modify.png";
                    item.Display = CurrentApp.GetLanguageInfo("3103T00034", "Change Expiration Date");
                    item.Description = null;
                    mListBasicOperations.Add(item);
                }
                List<OperationInfo> lstReMove = S3103App.ListOperationInfos.Where(p => p.ID == S3103Consts.OPT_REMOVETASKRECORD).ToList();
                if (lstReMove.Count > 0)//移除任务记录
                {
                    OperationInfo item = new OperationInfo();
                    item.ID = S3103Consts.OPT_REMOVETASKRECORD;
                    item.ParentID = 1102;
                    item.SortID = 0;
                    item.Icon = "Images/reset.png";
                    item.Display = CurrentApp.GetLanguageInfo("3103T00074", "Remove");
                    item.Description = null;
                    mListBasicOperations.Add(item);
                }
                List<OperationInfo> lstmove2other = S3103App.ListOperationInfos.Where(p => p.ID == S3103Consts.OPT_MOVETOOTHERTASK).ToList();
                if (lstmove2other.Count > 0)//移动到其他任务
                {
                    OperationInfo item = new OperationInfo();
                    item.ID = S3103Consts.OPT_MOVETOOTHERTASK;
                    item.ParentID = 1102;
                    item.SortID = 0;
                    item.Icon = "Images/right.png";
                    item.Display = CurrentApp.GetLanguageInfo("3103T00075", "Move To Other Task");
                    item.Description = null;
                    mListBasicOperations.Add(item);
                }
            }
            else//查看任务
            {
                List<OperationInfo> lstScore = S3103App.ListOperationInfos.Where(p => p.ID == S3103Consts.OPT_TASKRECORDSCORE).ToList();
                if (lstScore.Count > 0 && mScore==true)//有评分权限
                {
                    OperationInfo item = new OperationInfo();
                    item.ID = S3103Consts.OPT_TASKRECORDSCORE;
                    item.ParentID = 1102;
                    item.SortID = 0;
                    item.Icon = "Images/search.png";
                    item.Display = CurrentApp.GetLanguageInfo("3103T00040", "Score Task");
                    item.Description = null;
                    mListBasicOperations.Add(item);
                }
            }
            CreateOptButtons();
        }

        private void CreateOptButtons()
        {
            PanelBasicOpts.Children.Clear();
            Button btn;
            OperationInfo item;
            for (int i = 0; i < mListBasicOperations.Count; i++)
            {
                item = mListBasicOperations[i];
                item.Display = mListBasicOperations[i].Display;
                btn = new Button();
                btn.Click += BasicOpt_Click;
                btn.DataContext = item;
                btn.SetResourceReference(StyleProperty, "OptButtonStyle");
                PanelBasicOpts.Children.Add(btn);
            }
        }

        /// <summary>
        /// 判断点击按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void BasicOpt_Click(object sender, RoutedEventArgs e)
        {
            var btn = e.Source as Button;
            if (btn != null)
            {
                //各操作按纽触发按钮方法 
                var optItem = btn.DataContext as OperationInfo;
                switch (optItem.ID)
                {
                    case 31030:
                        var temp = CurrentApp as S3103App;
                        TaskTrack taskTrackView = new TaskTrack();
                        CurrentApp.CurrentView = taskTrackView;
                        taskTrackView.PageName = "TaskTrack";
                        if (temp != null)
                        {
                            temp.InitMainView(taskTrackView);
                            //temp.InitMainView(0);
                        }
                        break;
                    case S3103Consts.OPT_MODIFYTASKFINISHTIME:
                        TaskDetailOptModifyDealTime();
                        break;
                    case S3103Consts.OPT_REMOVETASKRECORD:
                        ConfirmRemoveTaskRecord();
                        break;
                    case S3103Consts.OPT_MOVETOOTHERTASK:
                        ConfirmMove2OtherTask();
                        break;
                    case S3103Consts.OPT_TASKRECORDSCORE:
                        mViewScore = false;
                        TaskDetailOptScore();
                        break;
                    case S3103Consts.OPT_VIEWSCORE:
                        mViewScore = true;
                        TaskDetailOptScore();
                        break;
                    default:
                        break;
                }
            }
        }

        /// Author           : Luoyihua
        ///  Created          : 2014-11-06 16:46:00
        /// <summary>
        /// 点击修改任务完成时间按钮
        /// </summary>
        private void TaskDetailOptModifyDealTime()
        {
            PopupPanel.Title = CurrentApp.GetLanguageInfo("3103T00034", "Change Expiration Date");
            TaskFinishTimeChange popChangeTime = new TaskFinishTimeChange();
            popChangeTime.CurrentApp = CurrentApp;
            popChangeTime.SelectTask = SelectTask;
            popChangeTime.ParentPage = this;
            PopupPanel.Content = popChangeTime;
            PopupPanel.IsOpen = true;
        }

        /// Author           : Luoyihua
        ///  Created          : 2015-01-13 09:34:43
        /// <summary>
        /// Confirms the remove task record.
        /// </summary>
        private void ConfirmRemoveTaskRecord()
        {
            //currentSelRecord = (TaskInfoDetail)LVTaskDetail.SelectedItem;
            //if (currentSelRecord != null)
            bool IsContinue = true;
            string RecordList = string.Empty;
            int RecordLength=0;
            var listDRecord = LVTaskDetail.SelectedItems;
            if (listDRecord.Count>0)
            {
                for (int i = 0; i < listDRecord.Count;i++ )
                {
                    var item = listDRecord[i] as TaskInfoDetail;
                    if (item == null)
                    {
                        IsContinue = false;
                    }
                    if (item != null && item.TaskScore != null)
                    {
                        ShowInformation(CurrentApp.GetLanguageInfo("3103T00077", "No Operation for Scored Task."));
                        return;
                    }
                    RecordList += item.RecoredReference+",";
                    RecordLength+=Convert.ToInt32(Converter.Time2Second(item.Duration));
                }
                if (IsContinue)
                {
                    string messageBoxText = CurrentApp.GetLanguageInfo("3103T00076", "Confirm Remove?");
                    MessageBoxButton button = MessageBoxButton.OKCancel;
                    MessageBoxImage icon = MessageBoxImage.Warning;
                    MessageBoxResult result = MessageBox.Show(messageBoxText, CurrentApp.AppName, button, icon);
                    switch (result)
                    {
                        case MessageBoxResult.OK:
                            RemoveTaskRecord(RecordList.Substring(0, RecordList.Length - 1), listDRecord.Count,RecordLength);
                            break;
                        case MessageBoxResult.Cancel:
                            break;
                    }
                }
            }
        }

        /// Author           : Luoyihua
        ///  Created          : 2015-01-13 09:34:43
        /// <summary>
        /// Removes the task record.
        /// </summary>
        /// <param name="selrecord">The selrecord.</param>
        private void RemoveTaskRecord(string RecordList , int Dnum,int recordLength)
        {
            TaskInfoDetail item = LVTaskDetail.SelectedItem as TaskInfoDetail;
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S3103Codes.DeleteRecordFromTask;
                webRequest.Session = CurrentApp.Session;
                webRequest.ListData.Add(RecordList);
                webRequest.ListData.Add((ListCurrentTaskDetail.Count - Dnum).ToString());//当前任务中录音数量
                webRequest.ListData.Add(item.TaskID.ToString());
                webRequest.ListData.Add(SelectTask.AlreadyScoreNum.ToString());//当前任务中已评分数量
                webRequest.ListData.Add((SelectTask.TaskAllRecordLength - recordLength).ToString());//当前任务中录音总时长
                //Service31031Client client = new Service31031Client();
                Service31031Client client = new Service31031Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31031"));
                //WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.UMPTaskOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    CurrentApp.WriteOperationLog(S3103Consts.OPT_REMOVETASKRECORD.ToString(), ConstValue.OPT_RESULT_FAIL, "");
                   ShowException(string.Format("{0}.\t{1}\t{2}", CurrentApp.GetLanguageInfo("3103T00043", "Operation Field."), webReturn.Code, webReturn.Message));
                    return;
                }
                else
                {
                    #region 写操作日志
                    string strLog = string.Format("{0} {1}", Utils.FormatOptLogString("3103T00001"), SelectTask.TaskName);
                    strLog += string.Format("{0} {1}", Utils.FormatOptLogString("3103T00025"), RecordList);
                    CurrentApp.WriteOperationLog(S3103Consts.OPT_REMOVETASKRECORD.ToString(), ConstValue.OPT_RESULT_SUCCESS, strLog);
                    CurrentApp.WriteLog("DeleteTaskRecord", string.Format("{0} \t OldTaskCount {1} \t DeleteCount {2} \t NowTaskCount {3}", strLog, ListCurrentTaskDetail.Count, Dnum, ListCurrentTaskDetail.Count - Dnum));
                    #endregion
                    ShowInformation(CurrentApp.GetLanguageInfo("3103T00042", "Operation Succeed."));
                    SelectTask.AssignNum = ListCurrentTaskDetail.Count - Dnum;
                    InitTaskDetail();
                }
            }
            catch (Exception ex)
            {
               ShowException(string.Format("{0}.\t{0}", CurrentApp.GetLanguageInfo("3103T00043", "Operation Field."), ex.Message));
            }
        }

        /// Author           : Luoyihua
        ///  Created          : 2015-01-13 09:34:43
        /// <summary>
        /// Confirms the re move2 other task.
        /// </summary>
        private void ConfirmMove2OtherTask()
        {
            //currentSelRecord = (TaskInfoDetail)LVTaskDetail.SelectedItem;
            //if (currentSelRecord != null)
            bool IsContinue = true;
            string RecordList = string.Empty;
            double moveTime = 0;
            var listMRecord = LVTaskDetail.SelectedItems;
            if (listMRecord.Count > 0)
            {
                for (int i = 0; i < listMRecord.Count; i++)
                {
                    var item = listMRecord[i] as TaskInfoDetail;
                    if (item == null)
                    {
                        IsContinue = false;
                    }
                    if (item != null && item.TaskScore != null)
                    {
                        ShowInformation(CurrentApp.GetLanguageInfo("3103T00077", "No Operation for Scored Task."));
                        return;
                    }
                    RecordList += item.RecoredReference + ",";
                    moveTime += Converter.Time2Second(item.Duration);
                }
                if (S3103App.ListCtrolQAInfos.Count > 0 && IsContinue == true)
                {
                    PopupPanel.Title = CurrentApp.GetLanguageInfo("3103T00075", "Move to Other Task");
                    UCCanOperationTask ucopt = new UCCanOperationTask();
                    ucopt.CurrentApp = CurrentApp;
                    ucopt.ParentPage = this;
                    ucopt.selRemoveRecord = RecordList.Substring(0, RecordList.Length - 1);
                    ucopt.MoveNum = listMRecord.Count;
                    ucopt.MoveTime = moveTime;
                    ucopt.selfromtask = SelectTask;
                    PopupPanel.Content = ucopt;
                    PopupPanel.IsOpen = true;
                }
            }
        }

        /// Author           : Luoyihua
        ///  Created          : 2014-11-06 16:46:002                          
        /// <summary>
        /// 选择评分表
        /// </summary>
        private void TaskDetailOptScore()
        {
            currentSelRecord = (TaskInfoDetail)LVTaskDetail.SelectedItem;
            if (currentSelRecord != null)
            {
                PopupPanel.Title = CurrentApp.GetLanguageInfo("3103T00044", "Choose Scoring Template");
                TemplateSelect popSelTemplate = new TemplateSelect();
                popSelTemplate.CurrentApp = CurrentApp;
                popSelTemplate.ParentPage = this;
                popSelTemplate.selTaskRecord = currentSelRecord;
                popSelTemplate.LoadUserScoreSheetList();
                if (popSelTemplate.mListScoreSheetItems.Count() == 1)
                {
                    popSelTemplate.LoadUserScoreResultList();
                    ScoreTaskRecord(popSelTemplate.mListScoreSheetItems[0]);
                }
                else
                {
                    PopupPanel.Content = popSelTemplate;
                    PopupPanel.IsOpen = true;
                }
            }
        }

        /// Author           : Luoyihua
        ///  Created          : 2014-11-06 16:46:00
        /// <summary>
        /// 评分
        /// </summary>
        /// <param name="seltemplate">The seltemplate.</param>
        public void ScoreTaskRecord(BasicScoreSheetItem item)
        {
            selScoreSheet = item;
            if (selScoreSheet != null && currentSelRecord!=null)
            {
                ShowPopupPanel(false);
                TaskScoreForm mScoreForm = new TaskScoreForm();
                mScoreForm.PageParent = this;
                mScoreForm.selScoreSheet = selScoreSheet;
                mScoreForm.SelTaskRecord = currentSelRecord;
                CurrentApp.CurrentView = mScoreForm;
                mScoreForm.PageName = "TaskScoreForm";
                if (mViewScore.Equals(true))//查看成绩
                {
                    if (currentSelRecord.ScoreUserID != CurrentApp.Session.UserID)//非打分人员
                    {
                        mScoreForm.isEnableScore = false;
                    }
                    if (S3103App.ListAppealTaskParam.Where(p => p.GroupID == 310102).Count() >= 4)
                    {
                        if (SelectTask.TaskType == 1 || SelectTask.TaskType == 2)//初检
                        {
                            if (S3103App.ListAppealTaskParam.Where(p => p.ParamID == 31010203).First().ParamValue.Substring(8) == "1" && recordOptHistory.Contains("A"))//申诉过的录音
                            {
                                mScoreForm.isEnableScore = false;
                            }
                            if (S3103App.ListAppealTaskParam.Where(p => p.ParamID == 31010204).First().ParamValue.Substring(8) == "1" && recordOptHistory.Contains("T"))//分配复检的录音
                            {
                                mScoreForm.isEnableScore = false;
                            }
                        }
                        else if (SelectTask.TaskType == 3 || SelectTask.TaskType == 4)//复检
                        {
                            if (S3103App.ListAppealTaskParam.Where(p => p.ParamID == 31010203).First().ParamValue.Substring(8) == "1" && recordOptHistory.Contains("A"))//申诉过的录音
                            {
                                mScoreForm.isEnableScore = false;
                            }
                        }
                    }
                }
                var temp = CurrentApp as S3103App;
                if (temp != null)
                {
                    temp.InitMainView(mScoreForm);
                    //temp.InitMainView(4);
                }
            }
        }

        public void ShowPopupPanel(bool isshow)
        {
            PopupPanel.IsOpen = isshow;
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

        #region  

        public override void ChangeLanguage()
        {
            try
            {
                base.ChangeLanguage();

                CreateOperationButton();
                InitTaskDetailColumns();

                SetPageContentLanguage();

                //给换语言包
                ExpanderBasic.Header = CurrentApp.GetLanguageInfo("3103T00052", "Basic Operations");
                ExpanderOther.Header = CurrentApp.GetLanguageInfo("3103T00053", "Other Position");
                
            }
            catch { }            
        }
                
        private LayoutAnchorable GetPanleByContentID(string contentID)
        {
            var panel =
                PanelManager.Layout.Descendents()
                    .OfType<LayoutAnchorable>()
                    .FirstOrDefault(p => p.ContentId == contentID);
            return panel;
        }
        private void SetPageContentLanguage()
        {
            var panel = GetPanleByContentID("PanelPlayBox");
            if (panel != null)
            {
                panel.Title = CurrentApp.GetLanguageInfo("3103T00047", "Player");
            }
            panel = GetPanleByContentID("PanelTaskDtail");
            if (panel != null)
            {
                panel.Title = SelectTask.TaskName;
            }
            panel = GetPanleByContentID("PanelMemo");
            if (panel != null)
            {
                panel.Title = CurrentApp.GetLanguageInfo("3103T00051", "Remark");
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
