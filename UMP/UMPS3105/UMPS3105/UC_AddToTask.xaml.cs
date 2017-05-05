using System;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
using System.ComponentModel;
using System.Collections.Generic;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;
using System.Collections.ObjectModel;
using VoiceCyber.UMP.Common;
using VoiceCyber.NAudio.Controls;
using Common3105;
using UMPS3105.Models;
using VoiceCyber.Common;
using UMPS3105.Wcf31031;
using ColAlias = VoiceCyber.UMP.Common31031;
using CtrolAgent = Common3105.CtrolAgent;
namespace UMPS3105
{
    /// <summary>
    /// UC_AddToTask.xaml 的交互逻辑
    /// </summary>
    public partial class UC_AddToTask
    {
        public CCheckData SelCheckData;
        public AppealApprovalMainView ParentPage;
        public ProcessReCheck PrePage;



        public string AppealFlowItemID = string.Empty;
        private RecordInfoItem mListCurrentRecord;
        private List<CtrolQA> mListCtrolQA;

        //用户复检任务
        public static ObservableCollection<UserTasksInfoShow> ListCurrentUserTasks = new ObservableCollection<UserTasksInfoShow>();

        public UC_AddToTask()
        {
            InitializeComponent();
            SelCheckData = new CCheckData();
            mListCtrolQA = new List<CtrolQA>();
            mListCurrentRecord = new RecordInfoItem();
            AppealFlowItemID = string.Empty;

            BtnConfirm.Click += BtnConfirm_Click;
            BtnClose.Click += BtnClose_Click;
            BtnPre.Click += BtnPre_Click;
            this.Loaded+=UC_AddToTask_Loaded;

        }

        void UC_AddToTask_Loaded(object sender, RoutedEventArgs e) 
        {
            InitTaskDetailColumns();
            InitTasks();
            ChangeLanguage();
        }

        ///复检任务信息
        ///
        private void InitTaskDetailColumns() 
        {
            string[] lans = "3105T000127,3105T000128,3105T000129,3105T000130,3105T000131,3105T000132,3105T000133,3105T000134,3105T000135,3105T000136,3105T000137,3105T000138,3105T000139,3105T000140".Split(',');
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


        /// <summary>
        /// 加载当前用户分配的任务信息
        /// </summary>
        private void InitTasks()
        {
            try
            {
                ListCurrentUserTasks.Clear();
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)ColAlias.S3103Codes.GetCurrentUserTasks;
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
                            taskInfo.IsFinish = taskInfo.IsFinish == "Y" ? CurrentApp.GetLanguageInfo("3105T000141", "Finished") : CurrentApp.GetLanguageInfo("3105T000142", "Unfinished");
                            taskInfo.IsShare = taskInfo.IsShare == "Y" ? CurrentApp.GetLanguageInfo("3105T000143", "Yes") : CurrentApp.GetLanguageInfo("3105T000145", "No");
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


       //前一步
        void BtnPre_Click(object sender, RoutedEventArgs e) 
        {
            if (S3105App.AppealProcess == "N")
            {
                ParentPage.PopupPanel.Title = CurrentApp.GetLanguageInfo("3105T00011", "Re-assessment");
                ParentPage.PopupPanel.Content = PrePage;
            }
            else
            {
                ParentPage.PopupPanel.Title = CurrentApp.GetLanguageInfo("3105T00011", "Re-assessment");
                ParentPage.PopupPanel.Content = PrePage;
            }
        }

        void BtnClose_Click(object sender,RoutedEventArgs e)
        {
            var parent = ParentPage.PopupPanel as PopupPanel;
            if (parent != null)
            {
                parent.IsOpen = false;
            }
        }

        void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {

            ColAlias.UserTasksInfoShow currentTask = (ColAlias.UserTasksInfoShow)LvDoubleTaskData.SelectedItem;
            if(currentTask ==null)
            {
                ShowInformation(CurrentApp.GetLanguageInfo("3105T000145", "No Task Is Select"));
                return;
            }

            //查询这条录音的录音信息
            if (!GetRecordInfoByRef(SelCheckData.RecoredReference))
            {
                return;
            }
            List<RecordInfoItem> lstRecordInfoItem = new List<RecordInfoItem>();
            lstRecordInfoItem.Add(mListCurrentRecord);

            currentTask.TaskAllRecordLength = currentTask.TaskAllRecordLength + mListCurrentRecord.Duration;
            currentTask.AssignNum = currentTask.AssignNum + 1;
            try
            {
                
                //先更新表T_31_020_TaskList
                //提交任务
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)ColAlias.S3103Codes.UpdateTask;
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
                    webRequest.Code = (int)ColAlias.S3103Codes.SaveTaskRecord;
                     ColAlias::TaskInfoDetail taskinfodetail;
                    foreach (RecordInfoItem record in lstRecordInfoItem)
                    {
                        strrecords += record.SerialID + ",";
                        taskinfodetail = new ColAlias:: TaskInfoDetail();
                        taskinfodetail.TaskID = currentTask.TaskID;
                        taskinfodetail.RecoredReference = record.SerialID;
                        taskinfodetail.IsLock = "N";
                        taskinfodetail.AllotType = 4;  //对应T_31_022表的C006 4表示录音是从审批通过分配到任务中的录音
                        taskinfodetail.FromTaskID = -1;
                        taskinfodetail.TaskType = "3"; //对应T-31_022表的C010
                        taskinfodetail.StartRecordTime = Convert.ToDateTime(record.StartRecordTime).ToUniversalTime();
                        taskinfodetail.Duration = record.Duration.ToString();
                        taskinfodetail.Extension = record.Extension;
                        taskinfodetail.CalledID = record.CalledID;
                        taskinfodetail.CallerID = record.CallerID;
                        taskinfodetail.Direction =int.Parse(record.Direction);
                        List<CtrolAgent> templst = new List<CtrolAgent>();
                        //if (S3103App.GroupingWay.Contains("A"))  //以后这个工程加了分组方式后再启用 
                        //{
                        //    templst = S3103App.ListCtrolAgentInfos.Where(p => p.AgentName == record.ReAgent).ToList();
                        //}
                        //else if (S3103App.GroupingWay.Contains("R"))
                        //{
                        //    templst = S3103App.ListCtrolRealityExtension.Where(p => p.AgentName == record.ReAgent).ToList();
                        //}
                        //else if (S3103App.GroupingWay.Contains("E"))
                        //{
                        //    templst = S3103App.ListCtrolExtension.Where(p => p.AgentName == record.ReAgent).ToList();
                        //}
                        //if (templst.Count() > 0)
                        //{
                        //    taskinfodetail.AgtOrExtID = templst[0].AgentID;
                        //    taskinfodetail.AgtOrExtName = templst[0].AgentName;
                        //}
                        //else
                        //{
                        //    taskinfodetail.AgtOrExtID = "";
                        //    taskinfodetail.AgtOrExtName = "";
                        //}
                        templst = S3105App.ListCtrolAgentInfos.Where(p => p.AgentName == record.Agent).ToList();
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
                        foreach (RecordInfoItem r in lstRecordInfoItem)
                        {
                            webReturn = new WebReturn();
                            webRequest = new WebRequest();
                            webRequest.Session = CurrentApp.Session;
                            webRequest.Code = (int)ColAlias::S3103Codes.UpdateTaskID2Record;
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
                        foreach (RecordInfoItem r in lstRecordInfoItem)
                        {
                            webReturn = new WebReturn();
                            webRequest = new WebRequest();
                            webRequest.Session = CurrentApp.Session;
                            webRequest.Code = (int)ColAlias::S3103Codes.UpdateTaskID2Record;
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
                }
            }
            catch (Exception ex)
            {
                
                CurrentApp.WriteOperationLog(ColAlias.S3103Consts.OPT_DoubleTaskAssign.ToString(), ConstValue.OPT_RESULT_FAIL, "");
                ShowException(ex.Message);
            }


            //更改申诉表信息
            string error = PrePage.UpdateReCheckData("8", AppealFlowItemID, SelCheckData);

            if (!string.IsNullOrEmpty(error))
            {
                ShowException(error);
            }
            else
            {
                ShowInformation(CurrentApp.GetLanguageInfo("3105T00022", "Operation Succeed."));
                PrePage.UpdateTable19("8");
                ParentPage.ApprovalHistory();
                ParentPage.BindData();
                ParentPage.ShowProcessReCheck(false);
                #region 写操作日志
                string strLog = string.Format("{0} {1}", Utils.FormatOptLogString("3105T00001"), SelCheckData.RecoredReference);
                CurrentApp.WriteOperationLog(S3105Consts.OPT_Approval.ToString(), ConstValue.OPT_RESULT_SUCCESS, strLog);
                #endregion
            }
        }



        private bool GetRecordInfoByRef(string recordreference)//20140212... 2014年2月12号
        {
            bool ret = false;
            try
            {
                string tablename = ConstValue.TABLE_NAME_RECORD + "_" + CurrentApp.Session.RentInfo.Token;
                var tableInfo =
                        CurrentApp.Session.ListPartitionTables.FirstOrDefault(
                            t => t.TableName == ConstValue.TABLE_NAME_RECORD && t.PartType == TablePartType.DatetimeRange);
                if (tableInfo != null)//有分表 当前仅按年月分表 ex：T_21_001_00000_1405
                {
                    tablename += "_" + recordreference.Substring(0, 4);
                }
                string strSql = string.Format("SELECT * FROM {0} WHERE C002={1}", tablename, recordreference);
                WebRequest webRequest = new WebRequest();
                webRequest.Session = CurrentApp.Session;
                webRequest.Code = (int)ColAlias.S3103Codes.GetRecordData;
                webRequest.ListData.Add(strSql);
                webRequest.ListData.Add(tablename);
                webRequest.ListData.Add("mark");
                //Service31031Client client = new Service31031Client();
                Service31031Client client = new Service31031Client(WebHelper.CreateBasicHttpBinding(CurrentApp.Session), WebHelper.CreateEndpointAddress(CurrentApp.Session.AppServerInfo, "Service31031"));
                //WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.UMPTaskOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    if (webReturn.Message != ColAlias.S3103Consts.Err_TableNotExit)
                        ShowException(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return false;
                }
                if (webReturn.ListData == null)
                {
                    ShowException(string.Format("Fail. ListData is null"));
                    return false;
                }
                if (webReturn.ListData.Count <= 0) { return false; }

                OperationReturn optReturn = XMLHelper.DeserializeObject<Common3105.RecordInfo>(webReturn.ListData[0]);
                if (!optReturn.Result)
                {
                    ShowException(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return false;
                }
                Common3105.RecordInfo recordInfo = optReturn.Data as Common3105.RecordInfo;
                if (recordInfo == null)
                {
                    ShowException(string.Format("Fail. RecordInfo is null"));
                    return false;
                }
                mListCurrentRecord = new RecordInfoItem(recordInfo);
                ret = true;
            }
            catch
            {
                ret = false;
            }
            return ret;
        }



        private string ReturnTableName(DateTime stopTime)
        {
            string strTableName = string.Empty;
            strTableName = string.Format("{0}_{1}_{2}", ConstValue.TABLE_NAME_RECORD,
                            CurrentApp.Session.RentInfo.Token, stopTime.ToString("yyMM"));
            return strTableName;
        }

        public override void ChangeLanguage()
        {
            try
            {
                base.ChangeLanguage();
                InitTaskDetailColumns();
                InitTasks();
            }
            catch (Exception e)
            {

            }
        }
    }
}
