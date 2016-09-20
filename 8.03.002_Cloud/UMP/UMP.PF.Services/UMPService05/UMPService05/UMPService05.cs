using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceProcess;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using CommonService05;
using Oracle.DataAccess.Client;
using PFShareClasses01;
using PFShareClassesS;
using VoiceCyber.Common;

namespace UMPService05
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant, InstanceContextMode = InstanceContextMode.Single)]
    public partial class UMPService05 : ServiceBase
    {
        #region Member
        public static string IStrEventLogSource = "Service 05";
        public static string IStrApplicationName = "UMP";
        public static string IStrSpliterChar = string.Empty;

        public const string FILE_NAME_LOCALMACHINE = "localmachine.ini";

        private int IIntThisServicePort = 0;
        private string IStrSiteBaseDirectory = string.Empty;
        private int IIntSiteHttpBindingPort = 0;

        private static EventLog IEventLog = null;

        private bool IBoolCanContinue = true;

        private Thread IThreadReadDBProfile;

        private Thread IThreadTaskPublish;
        private bool IBooInTaskPublish = false;
        private bool IBoolCanAbortTaskPublish = false;

        private int IIntDatabaseType = 0;
        private string IStrDatabaseProfile = string.Empty;

        private string IStrVerificationCode102 = string.Empty;
        private string IStrVerificationCode002 = string.Empty;

        private static readonly object ILockObject = new object();

        /// <summary>
        /// 每次服务运行读取的有效任务
        /// </summary>
        private List<T_AutoTaskSet> IlistTask;
        /// <summary>
        /// 最后一次任务运行时间
        /// </summary>
        private List<TaskAlloted> IlstAlloted;
        /// <summary>
        /// 所有的时长比例
        /// </summary>
        public List<T_AutoTaskRate> ILstAutoTaskRateAll;
        #endregion

        public UMPService05()
        {
            InitializeComponent();
            FileLog.FilePath = GetLogPath();
            //OnStart(null);
        }

        #region 服务起停
        protected override void OnStart(string[] args)
        {
            Thread LThreadStartService = new Thread(new ThreadStart(StartService05));
            LThreadStartService.Start();
        }

        protected override void OnStop()
        {
            try
            {
                FileLog.WriteInfo("OnStop", "Stop Service 05");
                IBoolCanContinue = false;

                while (IBooInTaskPublish) { System.Threading.Thread.Sleep(100); }
                while (!IBoolCanAbortTaskPublish) { System.Threading.Thread.Sleep(100); }
                Thread.Sleep(500);
                IThreadTaskPublish.Abort();
                IThreadReadDBProfile.Abort();

                FileLog.WriteInfo("OnStop", "Service 05 Stopped");
            }
            catch (Exception ex)
            {
                FileLog.WriteError("OnStop", "Stop Service 05 Exception:\n" + ex.ToString());
            }
        }    
        #endregion

        #region 启动服务
        private void StartService05()
        {
            try
            {
                if (!EventLog.SourceExists(IStrEventLogSource)) { EventLog.CreateEventSource(IStrEventLogSource, IStrApplicationName); }
                UMPServiceLog.Source = IStrEventLogSource;
                UMPServiceLog.Log = IStrApplicationName;
                UMPServiceLog.ModifyOverflowPolicy(OverflowAction.OverwriteOlder, 3);
                IEventLog = UMPServiceLog;

                IStrSpliterChar = AscCodeToChr(27);
                IStrVerificationCode002 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M002);
                IStrVerificationCode102 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M102);

                FileLog.WriteInfo("StartService05", "Service Started");

                IStrSiteBaseDirectory = GetSiteBaseDirectory();
                IIntSiteHttpBindingPort = GetUMPPFBasicHttpPort();
                IIntThisServicePort = IIntSiteHttpBindingPort - 5;

                IThreadReadDBProfile = new Thread(new ThreadStart(ReadDatabaseConnectionProfile));
                IThreadReadDBProfile.Start();

                IThreadTaskPublish = new Thread(new ThreadStart(TaskPublishThreadWorker));
                IThreadTaskPublish.Start();
            }
            catch (Exception ex)
            {
                FileLog.WriteError("StartService05", ex.ToString());
            }
        }
        #endregion

        private void TaskPublishThreadWorker()
        {
            Thread.Sleep(2000);//等待2秒 读取数据库配置信息
            List<string> LListStrRentExistObjects = new List<string>();
            DateTime now;
            DateTime lastRunTime;
            lastRunTime = new DateTime(2000, 01, 01);
            while (IBoolCanContinue)
            {
                now = DateTime.Now;
                TimeSpan ts = now - lastRunTime;
                if (ts.TotalMinutes >= 10)
                {
                    lastRunTime = now;
                    try
                    {
                        if (IIntDatabaseType == 0 || string.IsNullOrEmpty(IStrDatabaseProfile)) { IBooInTaskPublish = false; Thread.Sleep(2000); continue; }
                        //IStrDatabaseProfile = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=192.168.9.238) (PORT=1521)))(CONNECT_DATA=(SERVICE_NAME= ORCL)));User Id=VCLOG5; Password=VCLOG5";
                        IBooInTaskPublish = true;

                        //-----------------------------------------------------
                        FileLog.DeleteOldLog();
                        DataTable LDataTableRentList = ObtainRentList(ref LListStrRentExistObjects);
                        if (LDataTableRentList != null)
                            DoAutoTaskMain(LDataTableRentList);
                        else
                            FileLog.WriteInfo("TaskPublishThreadWorker", "RentList == 0");
                        //-----------------------------------------------------

                        IBooInTaskPublish = false;
                        IBoolCanAbortTaskPublish = true;
                    }
                    catch (Exception ex)
                    {
                        IBooInTaskPublish = false;
                        IBoolCanAbortTaskPublish = true;
                        FileLog.WriteError("TaskPublishThreadWorker()", ex.Message);
                    }
                }
                else
                {
                    Thread.Sleep(10000);
                }
            }
        }

        #region 任务相关
        private void DoAutoTaskMain(DataTable ADataTableRentList)
        {
            string LStrRentToken = string.Empty;
            try
            {
                DatabaseOperation01Return LDatabaseOperationReturn = new DatabaseOperation01Return();
                DataOperations01 LDataOperations = new DataOperations01();

                foreach (DataRow LDataRowSingleRent in ADataTableRentList.Rows)
                {
                    LStrRentToken = LDataRowSingleRent["C021"].ToString();
                    DataTable LDataTableExistLogicPartitionTables = ObtainRentExistLogicPartitionTables(LStrRentToken, "T_21_001");
                    GetTaskList(LStrRentToken);
                    StartAutoTaskAllot(LDataTableExistLogicPartitionTables, LStrRentToken);
                }
            }
            catch (Exception ex)
            {
                FileLog.WriteError("DoAutoTask()", ex.Message);
            }
        }

        /// <summary>
        /// 获取配置的自动任务
        /// </summary>
        private void GetTaskList(string LStrRentToken)
        {
            try
            {
                IlistTask = new List<T_AutoTaskSet>();
                IlstAlloted = new List<TaskAlloted>();
                ILstAutoTaskRateAll = new List<T_AutoTaskRate>();
                List<string> LListStrRentExistObjects = new List<string>(); 
                string LStrDynamicSQL = string.Empty;

                DatabaseOperation01Return LDatabaseOperationReturn = new DatabaseOperation01Return();
                DataOperations01 LDataOperations = new DataOperations01();

                string utcdtNow = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                #region 任务信息
                if (IIntDatabaseType == 2)
                {
                    LStrDynamicSQL = string.Format("SELECT T.*,F.C001 F1,F.C002 F2,F.C003 F3,F.C004 F4,F.C005 F5,F.C006 F6,F.C007 F7,F.C008 F8,F.C009 F9,F.C010 F10,F.C011 F11,F.C012 F12,F.C013 F13,F.C014 F14,F.C015 F15,F.C016 F16,F.C017 F17,F.C018 F18,F.C019 F19,F.C020 F20,F.C021 F21,F.C022 F22,F.C023 F23,F.C024 F24,F.C025 F25,S.C001 S1,S.C002 S2,S.C003 S3,S.C004 S4,S.C005 S5,S.C006 S6,S.C007 S7,S.C008 S8,S.C009 S9,S.C010 S10,S.C011 S11,S.C012 S12,S.C013 S13,S.C014 S14,S.C015 S15,S.C016 S16,S.C017 S17 FROM T_31_023_{0} T LEFT JOIN T_31_024_{0} F ON T.C013=F.C001 LEFT JOIN T_31_026_{0} S ON T.C014=S.C001",
                        LStrRentToken);
                    LStrDynamicSQL += string.Format(" WHERE T.C003='Y' AND F.C002='Y' AND F.C004<'{0}' AND F.C005>'{0}'", utcdtNow);
                }
                if (IIntDatabaseType == 3)
                {
                    LStrDynamicSQL = string.Format("SELECT T.*,F.C001 F1,F.C002 F2,F.C003 F3,F.C004 F4,F.C005 F5,F.C006 F6,F.C007 F7,F.C008 F8,F.C009 F9,F.C010 F10,F.C011 F11,F.C012 F12,F.C013 F13,F.C014 F14,F.C015 F15,F.C016 F16,F.C017 F17,F.C018 F18,F.C019 F19,F.C020 F20,F.C021 F21,F.C022 F22,F.C023 F23,F.C024 F24,F.C025 F25,S.C001 S1,S.C002 S2,S.C003 S3,S.C004 S4,S.C005 S5,S.C006 S6,S.C007 S7,S.C008 S8,S.C009 S9,S.C010 S10,S.C011 S11,S.C012 S12,S.C013 S13,S.C014 S14,S.C015 S15,S.C016 S16,S.C017 S17 FROM T_31_023_{0} T LEFT JOIN T_31_024_{0} F ON T.C013=F.C001 LEFT JOIN T_31_026_{0} S ON T.C014=S.C001",
                        LStrRentToken);
                    LStrDynamicSQL += string.Format(" WHERE T.C003='Y' AND F.C002='Y' AND F.C004<to_date('{0}', 'yyyy-mm-dd hh24:mi:ss') AND F.C005>to_date('{0}', 'yyyy-mm-dd hh24:mi:ss')", utcdtNow);
                } 
                if (string.IsNullOrEmpty(LStrDynamicSQL)) { return; }
                LDatabaseOperationReturn = LDataOperations.SelectDataByDynamicSQL(IIntDatabaseType, IStrDatabaseProfile, LStrDynamicSQL);
                if (!LDatabaseOperationReturn.BoolReturn)
                {
                    FileLog.WriteError("GetTaskList() 1", LDatabaseOperationReturn.StrReturn);
                    return;
                }
                foreach (DataRow LDataRow in LDatabaseOperationReturn.DataSetReturn.Tables[0].Rows)
                {
                    T_AutoTaskSet mat = new T_AutoTaskSet();
                    T_QueryParam mqp = new T_QueryParam();
                    T_RunFrequency mrf = new T_RunFrequency();

                    #region 处理信息到T_AutoTaskSet
                    mat.AutoTaskID = Convert2Long(LDataRow["C001"]);
                    mat.Type = Convert2Int(LDataRow["C002"]);
                    mat.Status = Convert2String(LDataRow["C003"]);
                    mat.Creator = Convert2String(LDataRow["C004"]);
                    mat.CreatorName = Convert2String(LDataRow["C005"]);
                    mat.IsShare = Convert2String(LDataRow["C006"]);
                    mat.IsAVGAssign = Convert2String(LDataRow["C007"]);
                    mat.IsDropExtendRecord = Convert2String(LDataRow["C008"]);
                    mat.QMIDsOne = Convert2String(LDataRow["C009"]);
                    mat.QMIDsTwo = Convert2String(LDataRow["C010"]);
                    mat.QMIDsThree = Convert2String(LDataRow["C011"]);
                    mat.OrgTenantID = Convert2String(LDataRow["C012"]);
                    mat.QueryID = Convert2String(LDataRow["C013"]);
                    mat.FrequencyID = Convert2String(LDataRow["C014"]);
                    mat.IsPeriod = Convert2String(LDataRow["C015"]);
                    mat.IsDownGet = Convert2String(LDataRow["C016"]);
                    mat.AutoTaskSetName = Convert2String(LDataRow["C017"]);
                    mat.AutoTaskSetDesc = Convert2String(LDataRow["C018"]);
                    mat.IsTaskPacketShare = Convert2String(LDataRow["C019"]);
                    mat.TaskDealLineDay = Convert2Int(LDataRow["C020"]);

                    mqp.QueryID = Convert2Long(LDataRow["F1"]);
                    mqp.IsValid = Convert2String(LDataRow["F2"]);
                    mqp.QueryName = Convert2String(LDataRow["F3"]);
                    mqp.ValitdTimeStart = Convert2String(LDataRow["F4"]);
                    mqp.ValitdTimeEnd = Convert2String(LDataRow["F5"]);
                    mqp.IsRecentTime = Convert2String(LDataRow["F6"]);
                    mqp.TimeType = Convert2String(LDataRow["F7"]);
                    mqp.TimeNum = Convert2Int(LDataRow["F8"]);
                    mqp.StartRecordTime = Convert2String(LDataRow["F9"]);
                    mqp.StopRecordTime = Convert2String(LDataRow["F10"]);
                    mqp.CallDirection = Convert2String(LDataRow["F11"]);
                    mqp.Screen = Convert2String(LDataRow["F12"]);
                    mqp.IsEmtion = Convert2String(LDataRow["F13"]);
                    mqp.IsKeyWord = Convert2String(LDataRow["F14"]);
                    mqp.IsBookMark = Convert2String(LDataRow["F15"]);
                    mqp.IsScore = Convert2String(LDataRow["F16"]);
                    mqp.AgentAssType = Convert2String(LDataRow["F17"]);
                    mqp.AgentAssNum = Convert2Int(LDataRow["F18"]);
                    mqp.AgentAssPer = Convert2Int(LDataRow["F19"]);
                    mqp.AgentsIDOne = Convert2String(LDataRow["F20"]);
                    mqp.AgentsIDTwo = Convert2String(LDataRow["F21"]);
                    mqp.AgentsIDThree = Convert2String(LDataRow["F22"]);
                    mqp.DurationMin = Convert2Int(LDataRow["F23"]);
                    mqp.DurationMax = Convert2Int(LDataRow["F24"]);
                    mqp.ABCDCondition = Convert2String(LDataRow["F25"]);

                    mrf.FrequencyID = Convert2Long(LDataRow["S1"]);
                    mrf.RunFreq = Convert2String(LDataRow["S2"]);
                    mrf.DayTime = Convert2String(LDataRow["S3"]);
                    mrf.DayOfWeek = Convert2Int(LDataRow["S4"]);
                    mrf.DayOfMonth = Convert2Int(LDataRow["S5"]);
                    mrf.IsUniteSetOfPeriod = Convert2String(LDataRow["S6"]);
                    mrf.UniteSetPeriod = Convert2Int(LDataRow["S7"]);
                    mrf.DayOfFirstPeriod = Convert2Int(LDataRow["S8"]);
                    mrf.DayOfSecondPeriod = Convert2Int(LDataRow["S9"]);
                    mrf.DayOfThirdPeriod = Convert2Int(LDataRow["S10"]);
                    mrf.IsUniteSetOfSeason = Convert2String(LDataRow["S11"]);
                    mrf.UniteSetSeason = Convert2Int(LDataRow["S12"]);
                    mrf.DayOfFirstSeaSon = Convert2Int(LDataRow["S13"]);
                    mrf.DayOfSecondSeason = Convert2Int(LDataRow["S14"]);
                    mrf.DayOfThirdSeason = Convert2Int(LDataRow["S15"]);
                    mrf.DayOfFourSeason = Convert2Int(LDataRow["S16"]);
                    mrf.DayOfYear = Convert2Int(LDataRow["S17"]);

                    mat.CQueryParam = mqp;
                    mat.CRunFrequency = mrf;
                    #endregion
                    IlistTask.Add(mat);
                }

                #endregion
                #region 最后一次分配时间查询
                if (IIntDatabaseType == 2)
                {
                    LStrDynamicSQL = string.Format("SELECT * FROM T_31_057_{0}",
                        LStrRentToken);
                }
                if (IIntDatabaseType == 3)
                {
                    LStrDynamicSQL = string.Format("SELECT * FROM T_31_057_{0}",
                        LStrRentToken);
                } 
                if (string.IsNullOrEmpty(LStrDynamicSQL)) { return; }
                LDatabaseOperationReturn = LDataOperations.SelectDataByDynamicSQL(IIntDatabaseType, IStrDatabaseProfile, LStrDynamicSQL);
                if (!LDatabaseOperationReturn.BoolReturn)
                {
                    FileLog.WriteError("GetTaskList() 2", LDatabaseOperationReturn.StrReturn);
                    return;
                }
                foreach (DataRow LDataRow in LDatabaseOperationReturn.DataSetReturn.Tables[0].Rows)
                {
                    try
                    {
                        TaskAlloted task = new TaskAlloted() { AutoTaskID = Convert.ToInt64(LDataRow["C001"]), AllotTime = LDataRow["C002"].ToString() };
                        IlstAlloted.Add(task);
                    }
                    catch { }
                }
                #endregion
                #region 任务分配时长比率
                if (IIntDatabaseType == 2)
                {
                    LStrDynamicSQL = string.Format("SELECT * FROM T_31_048_{0}",
                        LStrRentToken);
                }
                if (IIntDatabaseType == 3)
                {
                    LStrDynamicSQL = string.Format("SELECT * FROM T_31_048_{0}",
                        LStrRentToken);
                } 
                if (string.IsNullOrEmpty(LStrDynamicSQL)) { return; }
                LDatabaseOperationReturn = LDataOperations.SelectDataByDynamicSQL(IIntDatabaseType, IStrDatabaseProfile, LStrDynamicSQL);
                if (!LDatabaseOperationReturn.BoolReturn)
                {
                    FileLog.WriteError("GetTaskList() 3", LDatabaseOperationReturn.StrReturn);
                    return;
                }
                foreach (DataRow LDataRow in LDatabaseOperationReturn.DataSetReturn.Tables[0].Rows)
                {
                    try
                    {
                        T_AutoTaskRate atr = new T_AutoTaskRate();
                        atr.AutoTaskSetID = Convert.ToInt64(LDataRow["C001"].ToString());
                        atr.Min = Convert.ToInt32(LDataRow["C002"].ToString());
                        atr.Max = Convert.ToInt32(LDataRow["C003"].ToString());
                        atr.Rate = Convert.ToInt32(LDataRow["C004"].ToString());
                        atr.Num = 0;
                        ILstAutoTaskRateAll.Add(atr);
                    }
                    catch { }
                }
                #endregion

                foreach (T_AutoTaskSet ats in IlistTask)
                {
                    TaskAlloted cta = IlstAlloted.Where(p => p.AutoTaskID == ats.AutoTaskID).FirstOrDefault();
                    ats.CTaskAlloted = cta;

                    List<T_AutoTaskRate> clstatr = ILstAutoTaskRateAll.Where(p => p.AutoTaskSetID == ats.AutoTaskID).ToList();
                    ats.LstAutoTaskRate = clstatr;
                }
            }
            catch (Exception ex)
            {
                FileLog.WriteError("GetTaskList()", ex.Message);
            }
        }

        private void StartAutoTaskAllot(DataTable LDataTableExistLogicPartitionTables, string LStrRentToken)
        {
            DateTime dt = DateTime.Now;////--utc
            foreach (T_AutoTaskSet task in IlistTask)
            {
                if (task.CRunFrequency == null || task.CQueryParam == null //没有查询条件和运行频率
                    || task.Status.ToUpper() == "N" || task.CQueryParam.IsValid=="N") //查询条件或者任务禁用
                    return;
                switch (task.CRunFrequency.RunFreq.ToUpper())
                { 
                    case ConstDefine.DAU_TaskRunFreq_Day:
                        if (dt.TimeOfDay >= TimeSpan.Parse(task.CRunFrequency.DayTime) && !CheckIsDoed(task))//当前时间(13:22:00)在配置的时间(13:00:00)之后 并且 需要分配
                        {
                            FileLog.WriteInfo("StartAutoTaskAllot=>Day,AutoTaskID=", String.Format("{0},taskName={2}, Start Time:{1}", task.AutoTaskID, DateTime.Now.TimeOfDay, task.AutoTaskSetName));
                            DoTaskAllot(task, LDataTableExistLogicPartitionTables, LStrRentToken);
                            FileLog.WriteInfo("StartAutoTaskAllot=>Day,AutoTaskID=", task.AutoTaskID.ToString() + ", Finish.");
                        }
                        break;
                    case ConstDefine.DAU_TaskRunFreq_Week:
                        if (dt.TimeOfDay >= TimeSpan.Parse(task.CRunFrequency.DayTime) && !CheckIsDoed(task))
                            {
                                FileLog.WriteInfo("StartAutoTaskAllot=>Week,AutoTaskID=", String.Format("{0},taskName={2}, Start Time:{1}", task.AutoTaskID, DateTime.Now.TimeOfDay, task.AutoTaskSetName));
                                DoTaskAllot(task, LDataTableExistLogicPartitionTables, LStrRentToken);
                                FileLog.WriteInfo("StartAutoTaskAllot=>Week,AutoTaskID=", task.AutoTaskID.ToString() + ", Finish.");

                            }
                        break;
                    case ConstDefine.DAU_TaskRunFreq_Month:
                        if (dt.TimeOfDay >= TimeSpan.Parse(task.CRunFrequency.DayTime) && !CheckIsDoed(task))
                        {
                            FileLog.WriteInfo("StartAutoTaskAllot=>Month,AutoTaskID=", String.Format("{0},taskName={2}, Start Time:{1}", task.AutoTaskID, DateTime.Now.TimeOfDay, task.AutoTaskSetName));
                            DoTaskAllot(task, LDataTableExistLogicPartitionTables, LStrRentToken);
                            FileLog.WriteInfo("StartAutoTaskAllot=>Month,AutoTaskID=", task.AutoTaskID.ToString() + ", Finish.");
                        }
                        break;
                    case ConstDefine.DAU_TaskRunFreq_Quarter:
                        if (dt.TimeOfDay >= TimeSpan.Parse(task.CRunFrequency.DayTime) && !CheckIsDoed(task))
                        {
                            FileLog.WriteInfo("StartAutoTaskAllot=>Quarter,AutoTaskID=", String.Format("{0},taskName={2}, Start Time:{1}", task.AutoTaskID, DateTime.Now.TimeOfDay, task.AutoTaskSetName));
                            DoTaskAllot(task, LDataTableExistLogicPartitionTables, LStrRentToken);
                            FileLog.WriteInfo("StartAutoTaskAllot=>Quarter,AutoTaskID=", task.AutoTaskID.ToString() + ", Finish.");
                        }
                        break;
                    case ConstDefine.DAU_TaskRunFreq_Year:
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// 任务分配主方法
        /// </summary>
        /// <param name="task"></param>
        private void DoTaskAllot(T_AutoTaskSet task, DataTable LDataTableExistLogicPartitionTables,string LStrRentToken)
        {
            try
            {
                DatabaseOperation01Return LDatabaseOperationReturn = new DatabaseOperation01Return();
                DataOperations01 LDataOperations = new DataOperations01();
                T_QueryParam qp= task.CQueryParam;
                string strStartData = "";
                string strEndData = "";
                DateTime dtNow = DateTime.Now;
                if (qp.IsRecentTime.ToUpper() == "Y")//启用最近时间
                {
                    switch (qp.TimeType.ToUpper())
                    {
                        case "H":
                            strEndData = dtNow.ToString("yyyy-MM-dd HH:mm:ss");
                            strStartData = dtNow.AddHours(-qp.TimeNum).ToString("yyyy-MM-dd HH:mm:ss");
                            break;
                        case "D":
                            strEndData = dtNow.ToString("yyyy-MM-dd HH:mm:ss");
                            strStartData = dtNow.AddDays(-qp.TimeNum).ToString("yyyy-MM-dd HH:mm:ss");
                            break;
                        case "M":
                            strEndData = dtNow.ToString("yyyy-MM-dd HH:mm:ss");
                            strStartData = dtNow.AddMonths(-qp.TimeNum).ToString("yyyy-MM-dd HH:mm:ss");
                            break;
                        case "Y":
                            strEndData = dtNow.ToString("yyyy-MM-dd HH:mm:ss");
                            strStartData = dtNow.AddYears(-qp.TimeNum).ToString("yyyy-MM-dd HH:mm:ss");
                            break;
                    }
                }
                else
                {
                    strStartData = qp.StartRecordTime;
                    strEndData = qp.StopRecordTime;
                }

                DateTime dtStartData = Convert.ToDateTime(strStartData);// 2015-9-1 16:00:00
                DateTime dtEndData = Convert.ToDateTime(strEndData);    // 2015-9-8 16:00:00
                //int iStart = Convert.ToInt32(dtStartData.ToString("yyMMdd"));//150901
                //int iEnd = Convert.ToInt32(dtEndData.ToString("yyMMdd"));    //150908
                List<string> lstAgents = new List<string>();
                lstAgents = GetLstAgents(qp);
                bool IsLogicPartitionTables = false;
                bool hasABCD = qp.ABCDCondition != "";

                //DateTime dtnow = DateTime.Now; //当前时间
                //double dtsub = dtnow.Subtract(dtnow.ToUniversalTime()).TotalHours;//时间差分界线 -12~12
                //double tsub = 0;//16 点分界线 小时大于16跨天  小于16，当天
                //if (dtsub > 0)//东半球 +8小时
                //    tsub = 24 - dtsub;//北京 08：00：00 对应UTC 00：00：00
                //else
                //    tsub = dtsub;

                ///所有选中待分配
                List<RecordInfo> lstAgentSelect = new List<RecordInfo>();
                if (LDataTableExistLogicPartitionTables.Rows.Count == 0)//没有分表
                {
                    foreach (string agent in lstAgents)//每个坐席查一次
                    {
                        if (qp.AgentAssType == "0")//每天M条 时间按天切片/不用理会分配比率
                        {
                            #region 每天M条
                            DateTime tempCurdate = dtStartData;
                            while (tempCurdate.Date <= dtEndData)
                            {
                                //当前坐席选中待分配
                                List<RecordInfo> lstCurAgentSel = new List<RecordInfo>();
                                ///查询出的坐席录音
                                List<RecordInfo> lstQueryRecordAgentAll = new List<RecordInfo>();
                                ///满足时长的坐席录音
                                List<RecordInfo> lstDurAll = new List<RecordInfo>();
                                ///不满足时长的坐席录音
                                List<RecordInfo> lstUnDurAll = new List<RecordInfo>();

                                string timeStart = tempCurdate.ToString("yyyy-MM-dd") + " 00:00:00";
                                string timeEnd = tempCurdate.ToString("yyyy-MM-dd") + " 23:59:59";
                                if (Convert.ToDateTime(timeStart) < dtStartData)//如果开始时间比总范围的时间小 则开始时间为总范围的开始时间
                                    timeStart = dtStartData.ToString("yyyy-MM-dd HH:mm:ss");
                                if (Convert.ToDateTime(timeEnd) > dtEndData)//如果结束时间比总范围的时间大 则结束时间为总范围的结束时间
                                    timeEnd = dtEndData.ToString("yyyy-MM-dd HH:mm:ss");

                                string strSQL = GetConditionStrAll(task, "T_21_001_" + LStrRentToken, LStrRentToken, timeStart, timeEnd, agent, "T_31_054_" + LStrRentToken);
                                GetReordBySql(strSQL, ref lstQueryRecordAgentAll, agent, timeStart, timeEnd);
                                if (lstQueryRecordAgentAll.Count == 0)
                                {
                                    tempCurdate = tempCurdate.AddDays(1);
                                    continue;
                                }

                                //包含ABCD-按分机排序 / 否则打乱顺序
                                if (hasABCD)
                                    lstQueryRecordAgentAll = lstQueryRecordAgentAll.OrderBy(p => p.Extension).ToList();
                                else
                                    lstQueryRecordAgentAll = lstQueryRecordAgentAll.Select(a => new { a, newID = Guid.NewGuid() }).OrderBy(b => b.newID).Select(c => c.a).ToList();
                                //满足时长的记录
                                lstDurAll = lstQueryRecordAgentAll.Where(p => p.Duration >= qp.DurationMin && p.Duration <= qp.DurationMax).ToList();
                                //不满足时长的记录
                                lstUnDurAll = lstQueryRecordAgentAll.Where(p => p.Duration < qp.DurationMin || p.Duration > qp.DurationMax).ToList();

                                //当前任务配置的时长比率
                                List<T_AutoTaskRate> lstTaskRate = ILstAutoTaskRateAll.Where(p => p.AutoTaskSetID == task.AutoTaskID).ToList();
                                if (lstTaskRate.Count > 0)//配置有时长比率
                                {
                                    #region 配置有时长比率
                                    foreach (T_AutoTaskRate atr in lstTaskRate)
                                    {
                                        lstCurAgentSel.Clear();
                                        //比率atr需要分配的数量
                                        int assNum = System.Convert.ToInt32(atr.Rate / 100.0 * Convert.ToDouble(qp.AgentAssNum));//四舍五入取整
                                        //满足时长比率的录音
                                        List<RecordInfo> lstatr = lstDurAll.Where(p => p.Duration >= atr.Min && p.Duration <= atr.Max).ToList();
                                        if (lstatr.Count < assNum)//满足条件的录音少于需要分配的录音 
                                        {
                                            lstCurAgentSel.AddRange(lstatr);
                                            foreach (RecordInfo rif in lstUnDurAll)//不够的从不满足条件的记录里面补充，补充的记录还不够则忽略
                                            {
                                                if (lstCurAgentSel.Count < assNum && !lstCurAgentSel.Contains(rif))
                                                    lstCurAgentSel.Add(rif);
                                                else
                                                    break;
                                            }
                                        }
                                        else//满足比率的录音数量大于需要分配的数量
                                        {
                                            foreach (RecordInfo rif in lstatr)
                                            {
                                                if (lstCurAgentSel.Count < assNum && !lstCurAgentSel.Contains(rif))
                                                    lstCurAgentSel.Add(rif);
                                                else
                                                    break;
                                            }
                                        }
                                        lstAgentSelect.AddRange(lstCurAgentSel);
                                    }
                                    #endregion
                                }
                                else//没有配置时长比率 
                                {
                                    #region 没有配置时长比率
                                    if (lstDurAll.Count > qp.AgentAssNum)//满足条件的记录数大于需要分配的数量
                                    {
                                        foreach (RecordInfo rif in lstDurAll)
                                        {
                                            if (lstCurAgentSel.Count < Convert2Int(qp.AgentAssNum))
                                                lstCurAgentSel.Add(rif);
                                            else
                                                break;
                                        }
                                    }
                                    else
                                    {
                                        lstCurAgentSel.AddRange(lstDurAll);
                                        foreach (RecordInfo rif in lstUnDurAll)//不够的从不满足条件的记录里面补充，补充的记录还不够则忽略
                                        {
                                            if (lstCurAgentSel.Count < Convert2Int(qp.AgentAssNum))
                                                lstCurAgentSel.Add(rif);
                                            else
                                                break;
                                        }
                                    }
                                    lstAgentSelect.AddRange(lstCurAgentSel);
                                    #endregion
                                }
                                tempCurdate = tempCurdate.AddDays(1);
                            }
                            #endregion
                        }
                        else if (qp.AgentAssType == "2")//当前时间段agent一共M条；时长/数量，根据配置算出
                        {
                            #region 当前时间每个坐席一共M条 
                            //当前坐席选中待分配
                            List<RecordInfo> lstCurAgentSel = new List<RecordInfo>();
                            ///当前坐席该时间段内的所以录音
                            List<RecordInfo> lstQueryRecordAgentAll = new List<RecordInfo>();
                            ///满足时长的坐席录音
                            List<RecordInfo> lstDurAll = new List<RecordInfo>();
                            ///不满足时长的坐席录音
                            List<RecordInfo> lstUnDurAll = new List<RecordInfo>();

                            string strSQL = GetConditionStrAll(task, "T_21_001_" + LStrRentToken, LStrRentToken, strStartData, strEndData, agent, "T_31_054_" + LStrRentToken);
                            GetReordBySql(strSQL, ref lstQueryRecordAgentAll, agent, strStartData, strEndData);
                            //包含ABCD-按分机排序 / 否则打乱顺序
                            if (hasABCD)
                                lstQueryRecordAgentAll = lstQueryRecordAgentAll.OrderBy(p => p.Extension).ToList();
                            else
                                lstQueryRecordAgentAll = lstQueryRecordAgentAll.Select(a => new { a, newID = Guid.NewGuid() }).OrderBy(b => b.newID).Select(c => c.a).ToList();
                            //满足时长条件的记录
                            lstDurAll = lstQueryRecordAgentAll.Where(p => p.Duration >= qp.DurationMin && p.Duration <= qp.DurationMax).ToList();
                            //不满足时长的记录
                            lstUnDurAll = lstQueryRecordAgentAll.Where(p => p.Duration < qp.DurationMin || p.Duration > qp.DurationMax).ToList();
                            //当前任务配置的时长比率
                            List<T_AutoTaskRate> lstTaskRate = ILstAutoTaskRateAll.Where(p=>p.AutoTaskSetID == task.AutoTaskID).ToList();
                            if (lstTaskRate.Count > 0)//配置有时长比率
                            {
                                #region 配置有时长比率
                                foreach (T_AutoTaskRate atr in lstTaskRate)
                                {
                                    lstCurAgentSel.Clear();
                                    //比率atr需要分配的数量
                                    int assNum = System.Convert.ToInt32(atr.Rate / 100.0 * Convert.ToDouble(qp.AgentAssNum));//四舍五入取整
                                    //满足时长比率的录音
                                    List<RecordInfo> lstatr = lstDurAll.Where(p => p.Duration >= atr.Min && p.Duration <= atr.Max).ToList();
                                    if (lstatr.Count < assNum)//满足条件的录音少于需要分配的录音 
                                    {
                                        lstCurAgentSel.AddRange(lstatr);
                                        foreach (RecordInfo rif in lstUnDurAll)//不够的从不满足条件的记录里面补充，补充的记录还不够则忽略
                                        {
                                            if (lstCurAgentSel.Count < assNum && !lstCurAgentSel.Contains(rif))
                                                lstCurAgentSel.Add(rif);
                                            else
                                                break;
                                        }
                                    }
                                    else//满足比率的录音数量大于需要分配的数量
                                    {
                                        foreach (RecordInfo rif in lstatr)
                                        {
                                            if (lstCurAgentSel.Count < assNum && !lstCurAgentSel.Contains(rif))
                                                lstCurAgentSel.Add(rif);
                                            else
                                                break;
                                        }
                                    }
                                    lstAgentSelect.AddRange(lstCurAgentSel);
                                }
                                #endregion
                            }
                            else//没有配置时长比率 
                            {
                                #region 没有配置时长比率 
                                if (lstDurAll.Count > qp.AgentAssNum)//满足条件的记录数大于需要分配的数量
                                {
                                    foreach (RecordInfo rif in lstDurAll)
                                    {
                                        if (lstCurAgentSel.Count < Convert2Int(qp.AgentAssNum))
                                            lstCurAgentSel.Add(rif);
                                        else
                                            break;
                                    }
                                }
                                else
                                {
                                    lstCurAgentSel.AddRange(lstDurAll);
                                    foreach (RecordInfo rif in lstUnDurAll)//不够的从不满足条件的记录里面补充，补充的记录还不够则忽略
                                    {
                                        if (lstCurAgentSel.Count < Convert2Int(qp.AgentAssNum))
                                            lstCurAgentSel.Add(rif);
                                        else
                                            break;
                                    }
                                }
                                lstAgentSelect.AddRange(lstCurAgentSel);
                                #endregion
                            }
                            #endregion
                        }
                    }
                }
                else//分表 T_21_001_00000_1505
                {
                    IsLogicPartitionTables = true;
                    foreach (string agent in lstAgents)//每个坐席查一次
                    {
                        if (qp.AgentAssType == "0")//每天M条 时间按天切片/不用理会分配比率
                        {
                            DateTime tempCurdate = dtStartData;
                            #region 每天M条
                            while (tempCurdate.Date <= dtEndData)
                            {
                                //当前坐席选中待分配
                                List<RecordInfo> lstCurAgentSel = new List<RecordInfo>();
                                ///查询出的坐席录音
                                List<RecordInfo> lstQueryRecordAgentAll = new List<RecordInfo>();
                                ///满足时长的坐席录音
                                List<RecordInfo> lstDurAll = new List<RecordInfo>();
                                ///不满足时长的坐席录音
                                List<RecordInfo> lstUnDurAll = new List<RecordInfo>();
                                string tablename = "";
                                string table54 = "";

                                string timeStart = tempCurdate.ToString("yyyy-MM-dd") + " 00:00:00";
                                string timeEnd = tempCurdate.ToString("yyyy-MM-dd") + " 23:59:59";
                                if (Convert.ToDateTime(timeStart) < dtStartData)//如果开始时间比总范围的时间小 则开始时间为总范围的开始时间
                                    timeStart = dtStartData.ToString("yyyy-MM-dd HH:mm:ss");
                                if (Convert.ToDateTime(timeEnd) > dtEndData)//如果结束时间比总范围的时间大 则结束时间为总范围的结束时间
                                    timeEnd = dtEndData.ToString("yyyy-MM-dd HH:mm:ss");
                                int stmonth = DateTime.Parse(timeStart).ToUniversalTime().Month;//本地开始时间转UTC的月份
                                int edmonth = DateTime.Parse(timeEnd).ToUniversalTime().Month;//本地结束时间转UTC的月份

                                if (edmonth != stmonth)//UTC跨月份
                                {
                                    tablename = "T_21_001_" + LStrRentToken + "_" + DateTime.Parse(timeStart).ToUniversalTime().ToString("yyMM");
                                    table54 = "T_31_054_" + LStrRentToken + "_" + DateTime.Parse(timeStart).ToUniversalTime().ToString("yyMM");
                                    if (IsTableExit(tablename, LDataTableExistLogicPartitionTables))//如果时间段的表存在
                                    {
                                        string strSQL = GetConditionStrAll(task, tablename, LStrRentToken, timeStart, timeEnd, agent, table54);
                                        GetReordBySql(strSQL, ref lstQueryRecordAgentAll, agent, timeStart, timeEnd);
                                    }
                                    tablename = "T_21_001_" + LStrRentToken + "_" + DateTime.Parse(timeEnd).ToUniversalTime().ToString("yyMM");
                                    if (IsTableExit(tablename, LDataTableExistLogicPartitionTables))//如果时间段的表存在
                                    {
                                        string strSQL = GetConditionStrAll(task, tablename, LStrRentToken, timeStart, timeEnd, agent, table54);
                                        GetReordBySql(strSQL, ref lstQueryRecordAgentAll, agent, timeStart, timeEnd);
                                    }
                                }
                                else
                                {
                                    tablename = "T_21_001_" + LStrRentToken + "_" + DateTime.Parse(timeStart).ToUniversalTime().ToString("yyMM");
                                    table54 = "T_31_054_" + LStrRentToken + "_" + DateTime.Parse(timeStart).ToUniversalTime().ToString("yyMM");
                                    if (IsTableExit(tablename, LDataTableExistLogicPartitionTables))//如果时间段的表存在
                                    {
                                        string strSQL = GetConditionStrAll(task, tablename, LStrRentToken, timeStart, timeEnd, agent, table54);
                                        GetReordBySql(strSQL, ref lstQueryRecordAgentAll, agent, timeStart, timeEnd);
                                    }
                                }

                                if (lstQueryRecordAgentAll.Count > 0)//记录数大于0
                                {
                                    //包含ABCD-按分机排序 / 否则打乱顺序
                                    if (hasABCD)
                                        lstQueryRecordAgentAll = lstQueryRecordAgentAll.OrderBy(p => p.Extension).ToList();
                                    else
                                        lstQueryRecordAgentAll = lstQueryRecordAgentAll.Select(a => new { a, newID = Guid.NewGuid() }).OrderBy(b => b.newID).Select(c => c.a).ToList();
                                    //满足时长的记录
                                    lstDurAll = lstQueryRecordAgentAll.Where(p => p.Duration >= qp.DurationMin && p.Duration <= qp.DurationMax).ToList();
                                    //不满足时长的记录
                                    lstUnDurAll = lstQueryRecordAgentAll.Where(p => p.Duration < qp.DurationMin || p.Duration > qp.DurationMax).ToList();
                                    //当前任务配置的时长比率
                                    List<T_AutoTaskRate> lstTaskRate = ILstAutoTaskRateAll.Where(p => p.AutoTaskSetID == task.AutoTaskID).ToList();
                                    if (lstTaskRate.Count > 0)//配置有时长比率
                                    {
                                        #region 配置有时长比率
                                        foreach (T_AutoTaskRate atr in lstTaskRate)
                                        {
                                            lstCurAgentSel.Clear();
                                            //比率atr需要分配的数量
                                            int assNum = System.Convert.ToInt32(atr.Rate / 100.0 * Convert.ToDouble(qp.AgentAssNum));//四舍五入取整
                                            //满足时长比率的录音
                                            List<RecordInfo> lstatr = lstDurAll.Where(p => p.Duration >= atr.Min && p.Duration <= atr.Max).ToList();
                                            if (lstatr.Count < assNum)//满足条件的录音少于需要分配的录音 
                                            {
                                                lstCurAgentSel.AddRange(lstatr);
                                                foreach (RecordInfo rif in lstUnDurAll)//不够的从不满足条件的记录里面补充，补充的记录还不够则忽略
                                                {
                                                    if (lstCurAgentSel.Count < assNum && !lstCurAgentSel.Contains(rif))
                                                        lstCurAgentSel.Add(rif);
                                                    else
                                                        break;
                                                }
                                            }
                                            else//满足比率的录音数量大于需要分配的数量
                                            {
                                                foreach (RecordInfo rif in lstatr)
                                                {
                                                    if (lstCurAgentSel.Count < assNum && !lstCurAgentSel.Contains(rif))
                                                        lstCurAgentSel.Add(rif);
                                                    else
                                                        break;
                                                }
                                            }
                                            lstAgentSelect.AddRange(lstCurAgentSel);
                                        }
                                        #endregion
                                    }
                                    else//没有配置时长比率 
                                    {
                                        #region 没有配置时长比率
                                        if (lstDurAll.Count > qp.AgentAssNum)//满足条件的记录数大于需要分配的数量
                                        {
                                            foreach (RecordInfo rif in lstDurAll)
                                            {
                                                if (lstCurAgentSel.Count < Convert2Int(qp.AgentAssNum))
                                                    lstCurAgentSel.Add(rif);
                                                else
                                                    break;
                                            }
                                        }
                                        else
                                        {
                                            lstCurAgentSel.AddRange(lstDurAll);
                                            foreach (RecordInfo rif in lstUnDurAll)//不够的从不满足条件的记录里面补充，补充的记录还不够则忽略
                                            {
                                                if (lstCurAgentSel.Count < Convert2Int(qp.AgentAssNum))
                                                    lstCurAgentSel.Add(rif);
                                                else
                                                    break;
                                            }
                                        }
                                        lstAgentSelect.AddRange(lstCurAgentSel);
                                        #endregion
                                    }
                                }
                                tempCurdate = tempCurdate.AddDays(1);
                            }                   
                            #endregion
                        }
                        else if (qp.AgentAssType == "2")//当前时间段agent一共M条；时长/数量，根据配置算出
                        {
                            #region 当前时间每个坐席一共M条 
                            //当前坐席选中待分配
                            List<RecordInfo> lstCurAgentSel = new List<RecordInfo>();
                            ///当前坐席该时间段内的所以录音
                            List<RecordInfo> lstQueryRecordAgentAll = new List<RecordInfo>();
                            ///满足时长的坐席录音
                            List<RecordInfo> lstDurAll = new List<RecordInfo>();
                            ///不满足时长的坐席录音
                            List<RecordInfo> lstUnDurAll = new List<RecordInfo>();
                            List<string> lstTablename = new List<string>();
                            int itempStart = Convert.ToInt32(dtStartData.ToString("yyMM"));
                            int itempEnd = Convert.ToInt32(dtEndData.ToString("yyMM"));
                            for (int i = itempStart; i < itempEnd + 1; i++)
                            {
                                string tablename = "T_21_001_" + LStrRentToken + "_" + i.ToString();
                                string table54 = "T_31_054_" + LStrRentToken + "_" + i.ToString();
                                if (IsTableExit(tablename, LDataTableExistLogicPartitionTables))
                                {
                                    string strSQL = GetConditionStrAll(task, tablename, LStrRentToken, strStartData, strEndData, agent, table54);
                                    GetReordBySql(strSQL, ref lstQueryRecordAgentAll, agent, strStartData, strEndData);
                                }
                            }

                            //包含ABCD-按分机排序 / 否则打乱顺序
                            if (hasABCD)
                                lstQueryRecordAgentAll = lstQueryRecordAgentAll.OrderBy(p => p.Extension).ToList();
                            else
                                lstQueryRecordAgentAll = lstQueryRecordAgentAll.Select(a => new { a, newID = Guid.NewGuid() }).OrderBy(b => b.newID).Select(c => c.a).ToList();
                            //满足时长条件的记录
                            lstDurAll = lstQueryRecordAgentAll.Where(p => p.Duration >= qp.DurationMin && p.Duration <= qp.DurationMax).ToList();
                            //不满足时长的记录
                            lstUnDurAll = lstQueryRecordAgentAll.Where(p => p.Duration < qp.DurationMin || p.Duration > qp.DurationMax).ToList();
                            //当前任务配置的时长比率
                            List<T_AutoTaskRate> lstTaskRate = ILstAutoTaskRateAll.Where(p => p.AutoTaskSetID == task.AutoTaskID).ToList();
                            if (lstTaskRate.Count > 0)//配置有时长比率
                            {
                                #region 配置有时长比率
                                foreach (T_AutoTaskRate atr in lstTaskRate)
                                {
                                    lstCurAgentSel.Clear();//清空上一比率
                                    //比率atr需要分配的数量
                                    int assNum = System.Convert.ToInt32(atr.Rate / 100.0 * Convert.ToDouble(qp.AgentAssNum));//四舍五入取整
                                    //满足时长比率的录音
                                    List<RecordInfo> lstatr = lstDurAll.Where(p => p.Duration >= atr.Min && p.Duration <= atr.Max).ToList();
                                    if (lstatr.Count < assNum)//满足条件的录音少于需要分配的录音 
                                    {
                                        lstCurAgentSel.AddRange(lstatr);
                                        foreach (RecordInfo rif in lstUnDurAll)//不够的从不满足条件的记录里面补充，补充的记录还不够则忽略
                                        {
                                            if (lstCurAgentSel.Count < assNum && !lstCurAgentSel.Contains(rif))
                                                lstCurAgentSel.Add(rif);
                                            else
                                                break;
                                        }
                                    }
                                    else//满足比率的录音数量大于需要分配的数量
                                    {
                                        foreach (RecordInfo rif in lstatr)
                                        {
                                            if (lstCurAgentSel.Count < assNum && !lstCurAgentSel.Contains(rif))
                                                lstCurAgentSel.Add(rif);
                                            else
                                                break;
                                        }
                                    }
                                    lstAgentSelect.AddRange(lstCurAgentSel);
                                }
                                #endregion
                            }
                            else//没有配置时长比率 
                            {
                                #region 没有配置时长比率
                                if (lstDurAll.Count > qp.AgentAssNum)//满足条件的记录数大于需要分配的数量
                                {
                                    foreach (RecordInfo rif in lstDurAll)
                                    {
                                        if (lstCurAgentSel.Count < Convert2Int(qp.AgentAssNum))
                                            lstCurAgentSel.Add(rif);
                                        else
                                            break;
                                    }
                                }
                                else
                                {
                                    lstCurAgentSel.AddRange(lstDurAll);
                                    foreach (RecordInfo rif in lstUnDurAll)//不够的从不满足条件的记录里面补充，补充的记录还不够则忽略
                                    {
                                        if (lstCurAgentSel.Count < Convert2Int(qp.AgentAssNum))
                                            lstCurAgentSel.Add(rif);
                                        else
                                            break;
                                    }
                                }
                                lstAgentSelect.AddRange(lstCurAgentSel);
                                #endregion
                            }
                            #endregion
                        }
                    }
                }
                if (lstAgentSelect.Count > 0)
                {
                    if (hasABCD)
                        lstAgentSelect = lstAgentSelect.OrderBy(p => p.Extension).ToList();
                    else
                        lstAgentSelect = lstAgentSelect.Select(a => new { a, newID = Guid.NewGuid() }).OrderBy(b => b.newID).Select(c => c.a).ToList();
                    UserTasksInfoShow userTaskInfoShow = new UserTasksInfoShow();
                    userTaskInfoShow.TaskType = 2;
                    userTaskInfoShow.IsShare = "N";
                    userTaskInfoShow.AssignTime = DateTime.UtcNow.ToString("yyyy/MM/dd HH:mm:ss");
                    userTaskInfoShow.AssignUser = 0; //
                    userTaskInfoShow.AssignUserFName = "0";
                    userTaskInfoShow.BelongYear = DateTime.UtcNow.Year;
                    userTaskInfoShow.BelongYear = DateTime.UtcNow.Year;
                    userTaskInfoShow.BelongMonth = DateTime.UtcNow.Month;
                    userTaskInfoShow.DealLine = DateTime.UtcNow.AddDays(task.TaskDealLineDay).ToString("yyyy/MM/dd HH:mm:ss");
                    if (!string.IsNullOrWhiteSpace(task.QMIDsOne))//QA数大于0
                    {
                        List<string> mListCtrolQA = new List<string>();
                        foreach (string strqa in task.QMIDsOne.Split(','))
                        {
                            if (!mListCtrolQA.Contains(strqa))
                                mListCtrolQA.Add(strqa);
                        }

                        int qaNum = mListCtrolQA.Count();
                        int avgNum = lstAgentSelect.Count / qaNum;

                        if (avgNum == 0)
                        { return; }
                        for (int i = 0; i < qaNum; i++)
                        {
                            List<RecordInfo> lstRecordTemp = new List<RecordInfo>();
                            List<string> lstCtrolQaTemp = new List<string>();
                            lstCtrolQaTemp.Add(mListCtrolQA[i]);
                            long recordlength = 0;
                            for (int k = 0; k < avgNum; k++)
                            {
                                RecordInfo r = new RecordInfo();
                                r = lstAgentSelect[i * avgNum + k];
                                recordlength += r.Duration;
                                lstRecordTemp.Add(r);
                            }
                            userTaskInfoShow.TaskName = "A" + DateTime.Now.ToString("yyyyMMddHHmmss");
                            userTaskInfoShow.TaskAllRecordLength = recordlength;
                            userTaskInfoShow.AssignNum = lstRecordTemp.Count;
                            SaveTask(task, userTaskInfoShow, lstCtrolQaTemp, lstRecordTemp, LStrRentToken, IsLogicPartitionTables);
                            Thread.Sleep(1000);
                        }
                    }
                    else
                        FileLog.WriteInfo("DoTaskAllot()", "QA.Count=0");
                }
                else
                    FileLog.WriteInfo("DoTaskAllot()", "AgentSelect.Count=0");
            }
            catch (Exception ex)
            {
                FileLog.WriteError("DoTaskAllot()", ex.Message);
            }
        }

        private void SaveTask(T_AutoTaskSet task,UserTasksInfoShow newTask, List<string> listctrolqa, List<RecordInfo> lstRecordInfoItem, string LStrRentToken, bool IsLogicPartitionTables)
        {
            FileLog.WriteInfo("SaveTask()", string.Format("autotaskid={0},taskname={1},recordCount={2},qa={3}",
                task.AutoTaskID, newTask.TaskName, lstRecordInfoItem.Count, listctrolqa[0]));
            //IIntDatabaseType  IStrDatabaseProfile
            long taskID = 0;
            OperationReturn optReturn = new OperationReturn();
            int errNum = 0;
            long serialID = 0;
            string errMsg = string.Empty;
            #region 创建任务31_020 获得任务ID
            try
            {
                if (IIntDatabaseType == 2)
                {
                    #region MSSQL
                    DbParameter[] mssqlParameters =
                        {
                            MssqlOperation.GetDbParameter("@ainparam01",MssqlDataType.Varchar,5 ),
                            MssqlOperation.GetDbParameter("@ainparam02",MssqlDataType.NVarchar,64),
                            MssqlOperation.GetDbParameter("@ainparam03",MssqlDataType.NVarchar,1024),
                            MssqlOperation.GetDbParameter("@ainparam04",MssqlDataType.Varchar,5),
                            MssqlOperation.GetDbParameter("@ainparam05",MssqlDataType.Varchar,1),
                            MssqlOperation.GetDbParameter("@ainparam06",MssqlDataType.Varchar,20),
                            MssqlOperation.GetDbParameter("@ainparam07",MssqlDataType.Varchar,20),
                            MssqlOperation.GetDbParameter("@ainparam08",MssqlDataType.Varchar,10),
                            MssqlOperation.GetDbParameter("@ainparam09",MssqlDataType.Varchar,20),
                            MssqlOperation.GetDbParameter("@ainparam10",MssqlDataType.Varchar,10),
                            MssqlOperation.GetDbParameter("@ainparam11",MssqlDataType.Varchar,10),
                            //MssqlOperation.GetDbParameter("@ainparam12",MssqlDataType.NVarchar,1024),
                            MssqlOperation.GetDbParameter("@ainparam13",MssqlDataType.Varchar,10),
                            MssqlOperation.GetDbParameter("@ainparam14",MssqlDataType.Varchar,10),
                            MssqlOperation.GetDbParameter("@ainparam15",MssqlDataType.Varchar,20),
                            MssqlOperation.GetDbParameter("@ainparam16",MssqlDataType.NVarchar,1024),
                            MssqlOperation.GetDbParameter("@ainparam17",MssqlDataType.Varchar,20),
                            MssqlOperation.GetDbParameter("@ainparam18",MssqlDataType.Varchar,5),
                            MssqlOperation.GetDbParameter("@aoutparam01",MssqlDataType.Varchar,20),
                            MssqlOperation.GetDbParameter("@aouterrornumber",MssqlDataType.Bigint,0),
                            MssqlOperation.GetDbParameter("@aouterrorstring",MssqlDataType.NVarchar,4000)

                        };
                    mssqlParameters[0].Value = LStrRentToken;
                    mssqlParameters[1].Value = newTask.TaskName;//*
                    mssqlParameters[2].Value = "";
                    mssqlParameters[3].Value = newTask.TaskType;//*
                    mssqlParameters[4].Value = newTask.IsShare;//*
                    mssqlParameters[5].Value = newTask.AssignTime;//*
                    mssqlParameters[6].Value = string.Format("102{0}00000000001", LStrRentToken);

                    mssqlParameters[7].Value = newTask.AssignNum;//*
                    mssqlParameters[8].Value = newTask.DealLine;
                    mssqlParameters[9].Value = newTask.AlreadyScoreNum;//*
                    mssqlParameters[10].Value = newTask.RemindDayTime;
                    // mssqlParameters[11].Value = newTask.ReminderIDs;
                    mssqlParameters[11].Value = newTask.BelongYear;//*
                    mssqlParameters[12].Value = newTask.BelongMonth;//*
                    mssqlParameters[13].Value = newTask.FinishTime;
                    mssqlParameters[14].Value = string.Format("102{0}00000000001", LStrRentToken);
                    mssqlParameters[15].Value = newTask.TaskAllRecordLength;//*
                    mssqlParameters[16].Value = 308;

                    mssqlParameters[17].Value = serialID;
                    mssqlParameters[18].Value = errNum;
                    mssqlParameters[19].Value = errMsg;
                    mssqlParameters[17].Direction = ParameterDirection.Output;
                    mssqlParameters[18].Direction = ParameterDirection.Output;
                    mssqlParameters[19].Direction = ParameterDirection.Output;
                    optReturn = MssqlOperation.ExecuteStoredProcedure(IStrDatabaseProfile, "P_31_003",
                       mssqlParameters);
                    if (!optReturn.Result)
                    {
                        FileLog.WriteError("SaveTask()1", optReturn.Message);
                        return;
                    }
                    if (mssqlParameters[18].Value.ToString() != "0")
                    {
                        optReturn.Message = string.Format("{0}\t{1}", mssqlParameters[19].Value, mssqlParameters[14].Value);
                        FileLog.WriteError("SaveTask()2", string.Format("{0}\t{1}", mssqlParameters[19].Value, mssqlParameters[14].Value));
                        return;
                    }
                    else
                    {
                        taskID = Convert.ToInt64(mssqlParameters[17].Value.ToString());
                        FileLog.WriteInfo("SaveTask()3", "taskID=" + taskID.ToString());
                    }
                    #endregion
                }
                else if (IIntDatabaseType == 3)
                {
                    #region ORACLE
                    DbParameter[] dbParameters =
                        {
                            OracleOperation.GetDbParameter("ainparam01",OracleDataType.Varchar2,5 ),
                            OracleOperation.GetDbParameter("ainparam02",OracleDataType.Nvarchar2,64),
                            OracleOperation.GetDbParameter("ainparam03",OracleDataType.Nvarchar2,1024),
                            OracleOperation.GetDbParameter("ainparam04",OracleDataType.Varchar2,5),
                            OracleOperation.GetDbParameter("ainparam05",OracleDataType.Varchar2,1),
                            OracleOperation.GetDbParameter("ainparam06",OracleDataType.Varchar2,20),
                            OracleOperation.GetDbParameter("ainparam07",OracleDataType.Varchar2,20),
                            OracleOperation.GetDbParameter("ainparam08",OracleDataType.Varchar2,10),
                            OracleOperation.GetDbParameter("ainparam09",OracleDataType.Varchar2,20),
                            OracleOperation.GetDbParameter("ainparam10",OracleDataType.Varchar2,10),
                            OracleOperation.GetDbParameter("ainparam11",OracleDataType.Varchar2,10),
                            // OracleOperation.GetDbParameter("ainparam12",OracleDataType.Nvarchar2,1024),
                            OracleOperation.GetDbParameter("ainparam13",OracleDataType.Varchar2,10),
                            OracleOperation.GetDbParameter("ainparam14",OracleDataType.Varchar2,10),
                             OracleOperation.GetDbParameter("ainparam15",OracleDataType.Varchar2,20),
                            OracleOperation.GetDbParameter("ainparam16",OracleDataType.Nvarchar2,1024),
                            OracleOperation.GetDbParameter("ainparam17",OracleDataType.Varchar2,20),
                            OracleOperation.GetDbParameter("ainparam18",OracleDataType.Varchar2,5),
                            OracleOperation.GetDbParameter("aoutparam01",OracleDataType.Varchar2,19),
                            OracleOperation.GetDbParameter("errornumber",OracleDataType.Int32,0),
                            OracleOperation.GetDbParameter("errorstring",OracleDataType.Nvarchar2,200)
                        };
                    dbParameters[0].Value = LStrRentToken;
                    dbParameters[1].Value = newTask.TaskName;
                    dbParameters[2].Value = newTask.TaskDesc;
                    dbParameters[3].Value = newTask.TaskType;
                    dbParameters[4].Value = newTask.IsShare;
                    dbParameters[5].Value = newTask.AssignTime;
                    dbParameters[6].Value = string.Format("102{0}00000000001", LStrRentToken); //newTask.AssignUser;

                    dbParameters[7].Value = newTask.AssignNum;
                    dbParameters[8].Value = newTask.DealLine;
                    dbParameters[9].Value = newTask.AlreadyScoreNum;
                    dbParameters[10].Value = newTask.RemindDayTime;
                    //dbParameters[11].Value = newTask.ReminderIDs;
                    dbParameters[11].Value = newTask.BelongYear;
                    dbParameters[12].Value = newTask.BelongMonth;
                    dbParameters[13].Value = newTask.FinishTime;
                    dbParameters[14].Value = string.Format("102{0}00000000001", LStrRentToken);
                    dbParameters[15].Value = newTask.TaskAllRecordLength;
                    dbParameters[16].Value = 308;


                    dbParameters[17].Value = serialID;
                    dbParameters[18].Value = errNum;
                    dbParameters[19].Value = errMsg;
                    dbParameters[17].Direction = ParameterDirection.Output;
                    dbParameters[18].Direction = ParameterDirection.Output;
                    dbParameters[19].Direction = ParameterDirection.Output;
                    optReturn = OracleOperation.ExecuteStoredProcedure(IStrDatabaseProfile, " P_31_003",
                        dbParameters);

                    if (!optReturn.Result)
                    {
                        FileLog.WriteError("SaveTask() 1", optReturn.Message);
                        return;
                    }
                    if (dbParameters[18].Value.ToString() != "0")
                    {
                        optReturn.Message = string.Format("{0}\t{1}", dbParameters[19].Value, dbParameters[14].Value);
                        FileLog.WriteError("SaveTask() 2", string.Format("{0}\t{1}", dbParameters[19].Value, dbParameters[14].Value));
                        return;
                    }
                    else
                    {
                        taskID = Convert.ToInt64(dbParameters[17].Value.ToString());
                        FileLog.WriteInfo("SaveTask()", "taskID=" + taskID.ToString());
                    }
                    #endregion
                }
                newTask.TaskID = taskID;//创建任务 获得创建的任务ID
            }
            catch (Exception ex)
            {
                FileLog.WriteError("SaveTask e1", ex.Message);
                return;
            }
            #endregion
            #region 提交任务QA T_31_021_00000
            try
            {
                if (IIntDatabaseType == 2)
                {
                    using (SqlConnection connection = new SqlConnection(IStrDatabaseProfile))
                    {
                        DataSet dataSet = new DataSet();
                        connection.Open();
                        SqlDataAdapter sqlDA = new SqlDataAdapter(string.Format("SELECT  *  FROM  T_31_021_{0} WHERE  C001={1}", LStrRentToken, taskID), connection);
                        sqlDA.Fill(dataSet);
                        //设置主键
                        dataSet.Tables[0].PrimaryKey = new DataColumn[] { dataSet.Tables[0].Columns[0], dataSet.Tables[0].Columns[1] };
                        SqlCommandBuilder sqlCB = new SqlCommandBuilder(sqlDA);
                        sqlDA.InsertCommand = sqlCB.GetInsertCommand();
                        dataSet.Tables[0].Rows.Clear();
                        foreach (string strQA in listctrolqa)
                        {
                            DataRow drCurrent = dataSet.Tables[0].NewRow();
                            drCurrent["C001"] = taskID;
                            drCurrent["C002"] = strQA;
                            drCurrent["C003"] = strQA;
                            drCurrent["C004"] = "Q";
                            dataSet.Tables[0].Rows.Add(drCurrent);
                        }
                        sqlDA.Update(dataSet);
                        dataSet.AcceptChanges();
                        sqlDA.Dispose();
                        connection.Close();
                    }
                }
                else if (IIntDatabaseType == 3)
                {
                    using (OracleConnection connection = new OracleConnection(IStrDatabaseProfile))
                    {
                        DataSet dataSet = new DataSet();
                        connection.Open();
                        OracleDataAdapter oracleDA = new OracleDataAdapter(string.Format("SELECT  *  FROM  T_31_021_{0} WHERE  C001={1}", LStrRentToken, taskID), connection);
                        oracleDA.Fill(dataSet);
                        //设置主键
                        dataSet.Tables[0].PrimaryKey = new DataColumn[] { dataSet.Tables[0].Columns[0], dataSet.Tables[0].Columns[1] };
                        OracleCommandBuilder oracleCB = new OracleCommandBuilder(oracleDA);
                        oracleDA.InsertCommand = oracleCB.GetInsertCommand();
                        dataSet.Tables[0].Rows.Clear();
                        foreach (string strQA in listctrolqa)
                        {
                            DataRow drCurrent = dataSet.Tables[0].NewRow();
                            drCurrent["C001"] = taskID;
                            drCurrent["C002"] = strQA;
                            drCurrent["C003"] = strQA;
                            drCurrent["C004"] = "Q";
                            dataSet.Tables[0].Rows.Add(drCurrent);
                            oracleDA.Update(dataSet);
                            dataSet.AcceptChanges();
                            oracleDA.Dispose();
                            connection.Close();
                        }
                    }
                }
                FileLog.WriteInfo("SaveTask2QA", "Save task:" + taskID.ToString() + " to QA:" + listctrolqa[0]);
            }
            catch (Exception ex)
            {
                FileLog.WriteError("SaveTask e2", ex.Message);
                return;
            }
            #endregion
            #region 提交任务录音T_31_022_00000
            try
            {
                List<TaskInfoDetail> tempListTID = new List<TaskInfoDetail>();
                TaskInfoDetail taskinfo;
                string strRecord = "";
                foreach (RecordInfo record in lstRecordInfoItem)
                {
                    taskinfo = new TaskInfoDetail();
                    strRecord += record.SerialID.ToString() + ",";
                    taskinfo.TaskID = taskID;
                    taskinfo.RecoredReference = record.SerialID;
                    taskinfo.IsLock = "N";
                    taskinfo.AllotType = 1;
                    taskinfo.FromTaskID = -1;
                    taskinfo.TaskType = "2";
                    taskinfo.StartRecordTime = Convert.ToDateTime(record.StartRecordTime).ToUniversalTime();
                    taskinfo.Duration = record.Duration.ToString();
                    taskinfo.Extension = record.Extension;
                    taskinfo.CalledID = record.CalledID;
                    taskinfo.CallerID = record.CallerID;
                    taskinfo.Direction = record.Direction;
                    taskinfo.AgtOrExtID = record.Agent;
                    taskinfo.AgtOrExtName = record.Agent;
                    tempListTID.Add(taskinfo);
                }
                if (IIntDatabaseType == 2)
                {
                    using (SqlConnection connection = new SqlConnection(IStrDatabaseProfile))
                    {
                        DataSet dataSet = new DataSet();
                        connection.Open();
                        SqlDataAdapter sqlDA = new SqlDataAdapter(string.Format("SELECT  *  FROM  T_31_022_{0} WHERE  C001={1}", LStrRentToken, taskID), connection);
                        sqlDA.Fill(dataSet);
                        //设置主键
                        dataSet.Tables[0].PrimaryKey = new DataColumn[] { dataSet.Tables[0].Columns[0], dataSet.Tables[0].Columns[1] };
                        SqlCommandBuilder sqlCB = new SqlCommandBuilder(sqlDA);
                        sqlDA.InsertCommand = sqlCB.GetInsertCommand();
                        foreach (TaskInfoDetail taskinfodetail in tempListTID)
                        {
                            DataRow drNewRow = dataSet.Tables[0].NewRow();
                            drNewRow["C001"] = taskinfodetail.TaskID;
                            drNewRow["C002"] = taskinfodetail.RecoredReference;
                            drNewRow["C003"] = taskinfodetail.IsLock;
                            drNewRow["C006"] = taskinfodetail.AllotType;
                            //drNewRow["C008"] = taskinfodetail.AgtOrExtID;
                            drNewRow["C009"] = taskinfodetail.AgtOrExtName;
                            drNewRow["C010"] = taskinfodetail.TaskType;
                            drNewRow["C011"] = taskinfodetail.StartRecordTime;
                            drNewRow["C012"] = taskinfodetail.Duration;
                            drNewRow["C014"] = taskinfodetail.CallerID;
                            drNewRow["C015"] = taskinfodetail.CalledID;
                            drNewRow["C016"] = taskinfodetail.Direction;
                            dataSet.Tables[0].Rows.Add(drNewRow);
                        }
                        sqlDA.Update(dataSet);
                        dataSet.AcceptChanges();
                        sqlDA.Dispose();
                        connection.Close();
                    }
                }
                else if (IIntDatabaseType == 3)
                {
                    using (OracleConnection connection = new OracleConnection(IStrDatabaseProfile))
                    {
                        DataSet dataSet = new DataSet();
                        connection.Open();
                        OracleDataAdapter oracleDA = new OracleDataAdapter(string.Format("SELECT  *  FROM  T_31_022_{0} WHERE  C001={1} ", LStrRentToken, taskID), connection);
                        oracleDA.Fill(dataSet);
                        //设置主键
                        dataSet.Tables[0].PrimaryKey = new DataColumn[] { dataSet.Tables[0].Columns[0], dataSet.Tables[0].Columns[1] };
                        OracleCommandBuilder oracleCB = new OracleCommandBuilder(oracleDA);
                        oracleDA.InsertCommand = oracleCB.GetInsertCommand();
                        foreach (TaskInfoDetail taskinfodetail in tempListTID)
                        {
                            DataRow drNewRow = dataSet.Tables[0].NewRow();
                            drNewRow["C001"] = taskinfodetail.TaskID;
                            drNewRow["C002"] = taskinfodetail.RecoredReference;
                            drNewRow["C003"] = taskinfodetail.IsLock;
                            drNewRow["C006"] = taskinfodetail.AllotType;
                            //drNewRow["C008"] = taskinfodetail.AgtOrExtID;
                            drNewRow["C009"] = taskinfodetail.AgtOrExtName;
                            drNewRow["C010"] = taskinfodetail.TaskType;
                            drNewRow["C011"] = taskinfodetail.StartRecordTime;
                            drNewRow["C012"] = taskinfodetail.Duration;
                            drNewRow["C014"] = taskinfodetail.CallerID;
                            drNewRow["C015"] = taskinfodetail.CalledID;
                            drNewRow["C016"] = taskinfodetail.Direction;
                            dataSet.Tables[0].Rows.Add(drNewRow);
                        }
                        oracleDA.Update(dataSet);
                        dataSet.AcceptChanges();
                        oracleDA.Dispose();
                        connection.Close();
                    }
                }
                FileLog.WriteInfo("SaveTaskRecord", "Save taskrecord :" + strRecord + " to QA:" + listctrolqa[0]);
            }
            catch (Exception ex)
            {
                FileLog.WriteError("SaveTask 22", ex.Message);
                RemoveTask(LStrRentToken, taskID);
                return;
            }
            #endregion
            #region 将任务号更新到录音表
            try 
            {
                string tablename = "";
                string LStrDynamicSQL = string.Empty;
                List<string> LListStrDynamicSQL = new List<string>();
                if (!IsLogicPartitionTables)//分表
                {
                    tablename = "T_21_001_" + LStrRentToken;
                    foreach (RecordInfo record in lstRecordInfoItem)
                    {
                        LStrDynamicSQL = string.Format("UPDATE {0} SET C104={1} WHERE C002='{2}'", tablename, taskID, record.RecordReference);
                        LListStrDynamicSQL.Add(LStrDynamicSQL);
                    }
                }
                else
                {
                    foreach (RecordInfo record in lstRecordInfoItem)
                    {
                        tablename = "T_21_001_" + LStrRentToken + "_" + record.RecordReference.Substring(0, 4);
                        LStrDynamicSQL = string.Format("UPDATE {0} SET C104={1} WHERE C002='{2}'", tablename, taskID, record.RecordReference);
                        LListStrDynamicSQL.Add(LStrDynamicSQL);
                    }
                }
                ExecuteDynamicSQL(LListStrDynamicSQL);
                FileLog.WriteInfo("SaveTask", "Update taskid to RecordMainTable Success.");
            }
            catch (Exception ex)
            {
                FileLog.WriteInfo("SaveTask() 21", ex.Message);
            }
            
            #endregion
            #region 更新分配时间表 T_31_057_00000
            try
            {
                DatabaseOperation01Return LDatabaseReturn = new DatabaseOperation01Return();
                DataOperations01 LDataOperations = new DataOperations01();
                TaskAlloted curTA = IlstAlloted.Where(p => p.AutoTaskID == task.AutoTaskID).FirstOrDefault();
                string strSql = "";
                string tempstrDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                if (curTA != null)//存在分配时间 更新
                {
                    strSql = string.Format("UPDATE T_31_057_{0} SET C002='{1}' WHERE C001='{2}'", LStrRentToken, tempstrDate, task.AutoTaskID);
                }
                else//以前没有分配过 插入
                {
                    strSql = string.Format("INSERT INTO T_31_057_{0}(C001,C002) VALUES('{1}','{2}')", LStrRentToken, task.AutoTaskID, tempstrDate);
                }
                LDatabaseReturn = LDataOperations.ExecuteDynamicSQL(IIntDatabaseType, IStrDatabaseProfile, strSql);
                if (!LDatabaseReturn.BoolReturn)
                    FileLog.WriteError("SaveTask", strSql + ";" + LDatabaseReturn.StrReturn);
                else
                {
                    TaskAlloted tempadd = new TaskAlloted();
                    tempadd.AutoTaskID = task.AutoTaskID;
                    tempadd.AllotTime = newTask.AssignTime;
                    IlstAlloted.Add(tempadd);
                    FileLog.WriteInfo("SaveTask", "Update AssignTimeTable Success.");
                }
            }
            catch (Exception ex)
            {
                FileLog.WriteError("SaveTask() 57", ex.Message);
            }
            #endregion
        }

        /// <summary>
        /// 提交任务录音失败时，删除任务
        /// </summary>
        private void RemoveTask(string LStrRentToken, long taskid)
        {
            try
            {
                DatabaseOperation01Return LDatabaseOperationReturn = new DatabaseOperation01Return();
                DataOperations01 LDataOperations = new DataOperations01();

                string LStrDynamicSQL = string.Format("DELETE FROM T_31_020_{0} WHERE C001 = {1}", LStrRentToken, taskid);
                LDataOperations.ExecuteDynamicSQL(IIntDatabaseType, IStrDatabaseProfile, LStrDynamicSQL);

                FileLog.WriteInfo("RemoveTask:", taskid.ToString() + " Success.");
            }
            catch (Exception ex)
            {
                FileLog.WriteError("RemoveTask", ex.Message);
            }
        }

        private bool IsTableExit(string tablename, DataTable DataTableExist)
        {
            bool ret = false;
            foreach (DataRow dr in DataTableExist.Rows)
            {
                if (dr[0].ToString() == tablename)
                    return true;
            }
            return ret;
        }

        private void GetReordBySql(string strSelectSql,ref List<RecordInfo> lstQueryRecord ,string agent,string startdate,string enddate)
        {
            DatabaseOperation01Return LDatabaseOperationReturn = new DatabaseOperation01Return();
            DataOperations01 LDataOperations = new DataOperations01();

            if (string.IsNullOrEmpty(strSelectSql)) { return; }
            LDatabaseOperationReturn = LDataOperations.SelectDataByDynamicSQL(IIntDatabaseType, IStrDatabaseProfile, strSelectSql);
            if (!LDatabaseOperationReturn.BoolReturn)
            {
                FileLog.WriteError("DoTaskAllot() 1", LDatabaseOperationReturn.StrReturn + ";" + strSelectSql);
                return;
            }
            foreach (DataRow dr in LDatabaseOperationReturn.DataSetReturn.Tables[0].Rows)//查询出的满足条件的记录
            {
                RecordInfo item = new RecordInfo();
                item.RowID = Convert.ToInt64(dr["C001"]);
                item.SerialID = Convert.ToInt64(dr["C002"]);
                item.RecordReference = dr["C002"].ToString();
                item.StartRecordTime = Convert.ToDateTime(dr["C005"]).ToLocalTime();
                item.StopRecordTime = Convert.ToDateTime(dr["C009"]).ToLocalTime();
                item.VoiceIP = dr["C020"].ToString();
                item.VoiceID = Convert.ToInt32(dr["C037"]);
                item.ChannelID = Convert.ToInt32(dr["C038"]);
                item.Extension = dr["C042"].ToString();
                item.Agent = dr["C039"].ToString();
                item.Duration = Convert.ToInt32(dr["C012"]);
                item.Direction = dr["C045"].ToString() == "1" ? 1 : 0;
                item.CallerID = dr["C040"].ToString();
                item.CalledID = dr["C041"].ToString();
                item.IsScore = dr["ISSCORE"].ToString();
                lstQueryRecord.Add(item);
            }
            FileLog.WriteInfo("GetReordBySql() agent=" + agent, "GetCount = " + LDatabaseOperationReturn.DataSetReturn.Tables[0].Rows.Count.ToString() + " " + startdate +"-" + enddate);
        }

        /// <summary>
        /// 判断当前任务是否已经分配
        /// </summary>
        /// <param name="task"></param>
        /// <returns>true:不需要再分配   false：需要分配</returns>
        private bool CheckIsDoed(T_AutoTaskSet task)
        {
            bool btaskFlag = true;
            DateTime dtNow = DateTime.Now;
            DateTime dtTask = new DateTime();
            List<TaskAlloted> lsAlloted = IlstAlloted.Where(p => p.AutoTaskID == task.AutoTaskID).ToList();
            if (lsAlloted.Count == 0)//该任务没有分配过
            {
                btaskFlag = false;
            }
            else
                dtTask = DateTime.Parse(lsAlloted.First().AllotTime);//最后一次分配的时间

            switch (task.CRunFrequency.RunFreq.ToUpper())
            {
                case ConstDefine.DAU_TaskRunFreq_Day:
                    {
                        if (!btaskFlag)
                            return false;
                        if (dtNow.Date == dtTask.Date)//最后一次分配的时间是今天
                            return true;
                        else
                            return false;
                    }
                    break;
                case ConstDefine.DAU_TaskRunFreq_Week:
                    if (btaskFlag)//该任务分配过
                    {
                        if (GetDayOfWeek(dtNow) == (int)task.CRunFrequency.DayOfWeek)//并且当前时间是配置的星期数
                        {
                            if (dtNow.Date == dtTask.Date)//最后一次分配的时间是今天 
                                return true;
                            else
                                return false;
                        }
                        else
                            return true;
                    }
                    else//没有分配过
                    {
                        if ((GetDayOfWeek(dtNow) == (int)task.CRunFrequency.DayOfWeek))//当前时间是配置的星期数
                            return false;
                        else
                            return true;
                    }
                    break;
                case ConstDefine.DAU_TaskRunFreq_Month:
                    if (btaskFlag)
                    {
                        int maxday = GetMonthMaxDay();
                        int runDayOfMonth = (int)task.CRunFrequency.DayOfMonth;
                        if (runDayOfMonth > maxday && dtNow.Date.Day == maxday)
                        {
                            if (dtNow.Date == dtTask.Date)
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else if (dtNow.Date.Day == runDayOfMonth)
                        {
                            if (dtNow.Date == dtTask.Date)
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else
                        {
                            return true;
                        }
                    }
                    else 
                    {
                        int maxday = GetMonthMaxDay();
                        int runDayOfMonth = (int)task.CRunFrequency.DayOfMonth;
                        if (runDayOfMonth > maxday && dtNow.Date.Day == maxday)
                        {
                            return false;
                        }
                        else if (dtNow.Date.Day == runDayOfMonth)
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                    break;
                case ConstDefine.DAU_TaskRunFreq_Quarter:
                    if (btaskFlag)//任务分配过
                    {
                        int numAssQt = (int)Math.Ceiling(dtTask.Month / 3.0);//分配时的季度

                        int numNumQt = (int)Math.Ceiling(dtNow.Month / 3.0);//当前时间的季度
                        int numMonthOfQuater = dtNow.Month % 3;
                        int numQuater = (int)Math.Ceiling(dtNow.Month / 3.0);
                        if (numMonthOfQuater == 0) { numMonthOfQuater = 3; }
                        int month = (numQuater - 1) * 3 + 1;

                        DateTime tempStart = Convert.ToDateTime(dtNow.ToString("yyyy") + "-" + month.ToString() + "-1 00:00:00");
                        DateTime tempEnd = dtNow;
                        int tempDays = (int)(tempEnd.Subtract(tempStart).TotalDays) + 1;//当前时间是该季度的多少天 没有0天 1-1，4-1，7-1都算季度第一天

                        if (dtNow.Year >= dtTask.Year && numNumQt > numAssQt && tempDays == task.CRunFrequency.UniteSetSeason)//现在时间的年份大，季度大 并且天数与配置相同
                            return false;
                        else
                            return true;
                    }
                    else
                    { 
                        int numMonthOfQuater = dtNow.Month % 3;
                        int numQuater = (int)Math.Ceiling(dtNow.Month / 3.0);
                        if (numMonthOfQuater == 0) { numMonthOfQuater = 3; }

                        int month = (numQuater - 1) * 3 + 1;

                        DateTime tempStart = Convert.ToDateTime(dtNow.ToString("yyyy") + "-" + month.ToString() + "-1 00:00:00");
                        DateTime tempEnd = dtNow;
                        int tempDays = (int)(tempEnd.Subtract(tempStart).TotalDays) + 1;//当前时间是该季度的多少天 没有0天 1-1，4-1，7-1都算季度第一天
                        if (tempDays == task.CRunFrequency.UniteSetSeason)//与配置天数相同
                            return false;//可分配
                        else
                            return true;//不能分配
                    }
                    break;                    
                case ConstDefine.DAU_TaskRunFreq_Year:
                    return true;
                    break;
                default: 
                    return true;
                    break;
            }
        }

        /// <summary>
        /// 获取查询sql
        /// </summary>
        /// <param name="task">任务配置</param>
        /// <param name="tablename">主表名称</param>
        /// <param name="LStrRentToken">租户编号</param>
        /// <param name="strStartData">开始时间</param>
        /// <param name="strEndData">截止时间</param>
        /// <param name="agents">坐席</param>
        /// <param name="top">查询数量</param>
        /// <returns></returns>
        private string GetConditionStr(T_AutoTaskSet task, string tablename, string LStrRentToken, string strStartData, string strEndData, string agents,string top)
        {
            try
            {
                T_QueryParam qp = task.CQueryParam;
                StringBuilder strsb = new StringBuilder();

                if (IIntDatabaseType == 2)
                {
                    strsb.Append(string.Format("SELECT * FROM (SELECT TOP {0} T21.*,",top));
                    strsb.Append(string.Format("CASE WHEN EXISTS (SELECT * FROM T_31_022_{0} T22 WHERE T21.C002 = T22.C002) THEN '1' ELSE '0' END IsTask,"
                        , LStrRentToken));
                    strsb.Append("CASE WHEN EXISTS");
                    strsb.Append(string.Format("(SELECT * FROM T_31_008_{0} T8 WHERE T21.C002=T8.C002 AND (T8.C010 IN (1,2,3,4,5,6) OR T8.C014>0)) THEN '1' ELSE '0' END ISSCORE",
                        LStrRentToken));
                    strsb.Append(string.Format(" FROM {0} T21 WHERE C001 > 0", tablename));
                    //时间条件
                    strsb.Append(string.Format(" AND C005 >= '{0}' AND C005 <= '{1}'", strStartData, strEndData));

                    strsb.Append(string.Format(" AND C012 >= '{0}' AND C012 <= '{1}'", qp.DurationMin, qp.DurationMax));
                    //呼叫方向条件
                    if (qp.CallDirection != "2")
                        strsb.Append(string.Format(" AND C045 ='{0}'", qp.CallDirection));
                    //坐席条件
                    strsb.Append(string.Format(" AND C039='{0}' ORDER BY C001,C005,C039", agents));
                    strsb.Append(") T WHERE T.IsTask = '0' AND T.ISSCORE='0' ");
                }
                else if (IIntDatabaseType == 3)
                {
                    strsb.Append("SELECT * FROM (SELECT T21.*,");
                    strsb.Append(string.Format("CASE WHEN EXISTS (SELECT * FROM T_31_022_{0} T22 WHERE T21.C002 = T22.C002) THEN '1' ELSE '0' END IsTask,"
                        , LStrRentToken));
                    strsb.Append("CASE WHEN EXISTS");
                    strsb.Append(string.Format("(SELECT * FROM T_31_008_{0} T8 WHERE T21.C002=T8.C002 AND (T8.C010 IN (1,2,3,4,5,6) OR T8.C014>0)) THEN '1' ELSE '0' END ISSCORE",
                        LStrRentToken));
                    strsb.Append(string.Format(" FROM {0} T21 WHERE C001 > 0", tablename));
                    //时间条件
                    strsb.Append(string.Format(" AND C005>= TO_DATE('{0}','YYYY-MM-DD HH24:MI:SS') AND C005<=TO_DATE ('{1}','YYYY-MM-DD HH24:MI:SS')", strStartData, strEndData));
                    //呼叫方向条件
                    if (qp.CallDirection != "2")
                        strsb.Append(" AND C045 ={3} ");
                    //坐席条件
                    strsb.Append(string.Format(" AND C039='{0}' ORDER BY C001,C005,C039", agents));
                    strsb.Append(string.Format(") T WHERE T.IsTask = '0' AND T.ISSCORE='0' AND ROWNUM<={0}",top));
                }

                return strsb.ToString();
            }
            catch (Exception ex)
            {
                FileLog.WriteInfo("GetConditionStr", ex.Message);
                return "";
            }            
        }

        /// <summary>
        /// 获取查询sql 当前坐席 当前时间的全部录音
        /// </summary>
        /// <param name="task">任务配置</param>
        /// <param name="tablename">主表名称</param>
        /// <param name="LStrRentToken">租户编号</param>
        /// <param name="strStartData">开始时间</param>
        /// <param name="strEndData">截止时间</param>
        /// <param name="agents">坐席</param>
        /// <param name="top">查询数量</param>
        /// <returns></returns>
        private string GetConditionStrAll(T_AutoTaskSet task, string tablename, string LStrRentToken, string strStartData, string strEndData, string agents,string table54)
        {
            try
            {
                T_QueryParam qp = task.CQueryParam;
                StringBuilder strsb = new StringBuilder();
                bool hasABCD = qp.ABCDCondition != "";

                if (IIntDatabaseType == 2)
                {
                    strsb.Append("SELECT * FROM (SELECT T21.*,");
                    strsb.Append(string.Format("CASE WHEN EXISTS (SELECT * FROM T_31_022_{0} T22 WHERE T21.C002 = T22.C002) THEN '1' ELSE '0' END IsTask,"
                        , LStrRentToken));
                    strsb.Append("CASE WHEN EXISTS");
                    strsb.Append(string.Format("(SELECT * FROM T_31_008_{0} T8 WHERE T21.C002=T8.C002 AND (T8.C010 IN (1,2,3,4,5,6) OR T8.C014>0)) THEN '1' ELSE '0' END ISSCORE",
                        LStrRentToken));
                    strsb.Append(string.Format(" FROM {0} T21", tablename));
                    if (hasABCD)
                        strsb.Append(",T_31_054_00000 T354");
                    strsb.Append(" WHERE T21.C001>0 ");
                    //时间条件
                    strsb.Append(string.Format(" AND T21.C004>='{0}' AND T21.C004<='{1}'", strStartData, strEndData));
                    ////录音时长
                    //strsb.Append(string.Format(" AND C012>={0} AND C012<={1}", qp.DurationMin, qp.DurationMax));
                    //呼叫方向条件
                    if (qp.CallDirection != "2")
                        strsb.Append(string.Format(" AND T21.C045 ='{0}'", qp.CallDirection));
                    //坐席条件
                    strsb.Append(string.Format(" AND T21.C039='{0}'", agents));
                    //不查询录屏
                    strsb.Append(" AND T21.C014<>2");
                    if (hasABCD)
                        strsb.Append(" AND T21.C002=T354.C201 AND "+ qp.ABCDCondition);
                    strsb.Append(") T WHERE T.IsTask = '0' AND T.ISSCORE='0' ");
                }
                else if (IIntDatabaseType == 3)
                {
                    strsb.Append("SELECT * FROM (SELECT T21.*,");
                    strsb.Append(string.Format("CASE WHEN EXISTS (SELECT * FROM T_31_022_{0} T22 WHERE T21.C002 = T22.C002) THEN '1' ELSE '0' END IsTask,"
                        , LStrRentToken));
                    strsb.Append("CASE WHEN EXISTS");
                    strsb.Append(string.Format("(SELECT * FROM T_31_008_{0} T8 WHERE T21.C002=T8.C002 AND (T8.C010 IN (1,2,3,4,5,6) OR T8.C014>0)) THEN '1' ELSE '0' END ISSCORE",
                        LStrRentToken));
                    strsb.Append(string.Format(" FROM {0} T21", tablename));
                    if (hasABCD)
                        strsb.Append(",T_31_054_00000 T354");
                    strsb.Append(" WHERE T21.C001>0 ");
                    //时间条件
                    strsb.Append(string.Format(" AND T21.C004>=TO_DATE('{0}','YYYY-MM-DD HH24:MI:SS') AND T21.C004<=TO_DATE ('{1}','YYYY-MM-DD HH24:MI:SS')", strStartData, strEndData));
                    ////录音时长
                    //strsb.Append(string.Format(" AND C012>={0} AND C012<={1}", qp.DurationMin, qp.DurationMax));
                    //呼叫方向条件
                    if (qp.CallDirection != "2")
                        strsb.Append(" AND T21.C045 ={3} ");
                    //坐席条件
                    strsb.Append(string.Format(" AND T21.C039='{0}'", agents));
                    //不查询录屏
                    strsb.Append(" AND T21.C014<>2");
                    if (hasABCD)
                        strsb.Append(" AND T21.C002=T354.C201 AND " + qp.ABCDCondition);
                    strsb.Append(") T WHERE T.IsTask = '0' AND T.ISSCORE='0'");
                }

                return strsb.ToString();
            }
            catch (Exception ex)
            {
                FileLog.WriteInfo("GetConditionStr", ex.Message);
                return "";
            }
        }

        /// <summary>
        /// 获取分配的agent
        /// </summary>
        /// <param name="tqp"></param>
        /// <returns></returns>
        private List<string> GetLstAgents(T_QueryParam tqp)
        {
            List<string> lstAgent = new List<string>();
            foreach (string s in tqp.AgentsIDOne.TrimEnd(',').Split(','))
            {
                if (!string.IsNullOrWhiteSpace(s) && !lstAgent.Contains(s) && s.ToLower()!= "n/a")
                    lstAgent.Add(s);
            }
            foreach (string s in tqp.AgentsIDTwo.TrimEnd(',').Split(','))
            {
                if (!string.IsNullOrWhiteSpace(s) && !lstAgent.Contains(s) && s.ToLower() != "n/a")
                    lstAgent.Add(s);
            }
            foreach (string s in tqp.AgentsIDThree.TrimEnd(',').Split(','))
            {
                if (!string.IsNullOrWhiteSpace(s) && !lstAgent.Contains(s) && s.ToLower() != "n/a")
                    lstAgent.Add(s);
            }
            return lstAgent;
        }

        private string GetStrSQLAgents(T_QueryParam tqp)
        {
            string agents = "";
            foreach (string s in tqp.AgentsIDOne.TrimEnd(',').Split(','))
            {
                if (!string.IsNullOrWhiteSpace(s))
                    agents += "'" + s + "',";
            }
            foreach (string s in tqp.AgentsIDTwo.TrimEnd(',').Split(','))
            {
                if (!string.IsNullOrWhiteSpace(s))
                    agents += "'" + s + "',";
            }
            foreach (string s in tqp.AgentsIDThree.TrimEnd(',').Split(','))
            {
                if (!string.IsNullOrWhiteSpace(s))
                    agents += "'" + s + "',";
            }
            return agents.TrimEnd(',');
        }
        #endregion

        #region OTHER

        #region 批量执行 SQL 语句
        private bool ExecuteDynamicSQL(List<string> AListStrSQLString)
        {
            bool LBoolReturn = true;

            SqlConnection LSqlConnection = null;
            SqlCommand LSqlCommand = null;
            OracleConnection LOracleConnection = null;
            OracleCommand LOracleCommand = null;
            int LIntExecuteReturn = 0;

            try
            {
                if (IIntDatabaseType == 2)
                {
                    LSqlConnection = new SqlConnection(IStrDatabaseProfile);
                    LSqlConnection.Open();
                }
                if (IIntDatabaseType == 3)
                {
                    LOracleConnection = new OracleConnection(IStrDatabaseProfile);
                    LOracleConnection.Open();
                }

                foreach (string LStrSingleDynamicSQL in AListStrSQLString)
                {
                    try
                    {
                        if (IIntDatabaseType == 2)
                        {
                            LSqlCommand = new SqlCommand(LStrSingleDynamicSQL, LSqlConnection);
                            LIntExecuteReturn = LSqlCommand.ExecuteNonQuery();
                        }
                        if (IIntDatabaseType == 3)
                        {
                            LOracleCommand = new OracleCommand(LStrSingleDynamicSQL, LOracleConnection);
                            LIntExecuteReturn = LOracleCommand.ExecuteNonQuery();
                        }
                    }
                    catch (Exception ex)
                    {
                        LBoolReturn = false;
                        FileLog.WriteError("ExecuteDynamicSQL", LStrSingleDynamicSQL + ";" + ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                LBoolReturn = false;
                FileLog.WriteError("ExecuteDynamicSQL", ex.Message);
            }
            finally
            {
                if (LSqlCommand != null) { LSqlCommand.Dispose(); LSqlCommand = null; }
                if (LSqlConnection != null)
                {
                    if (LSqlConnection.State == System.Data.ConnectionState.Open) { LSqlConnection.Close(); }
                    LSqlConnection.Dispose();
                }
                if (LOracleCommand != null) { LOracleCommand.Dispose(); LOracleCommand = null; }
                if (LOracleConnection != null)
                {
                    if (LOracleConnection.State == ConnectionState.Open) { LOracleConnection.Close(); }
                    LOracleConnection.Dispose(); LOracleConnection = null;
                }
            }

            return LBoolReturn;
        }
        #endregion

        #region 获取所有租户列表
        private DataTable ObtainRentList(ref List<string> AListStrRentExistObjects)
        {
            DataTable LDataTableReturn = new DataTable();
            string LStrDynamicSQL = string.Empty;
            string LStrRentToken = string.Empty;
            string LStrSingleObject = string.Empty;

            try
            {
                DatabaseOperation01Return LDatabaseOperationReturn = new DatabaseOperation01Return();
                DataOperations01 LDataOperations = new DataOperations01();

                LStrDynamicSQL = "SELECT * FROM T_00_121 ORDER BY C001 ASC";
                LDatabaseOperationReturn = LDataOperations.SelectDataByDynamicSQL(IIntDatabaseType, IStrDatabaseProfile, LStrDynamicSQL);
                if (!LDatabaseOperationReturn.BoolReturn)
                {
                    LDataTableReturn = null;
                    FileLog.WriteError("ObtainRentList()", LDatabaseOperationReturn.StrReturn);
                }
                else
                {
                    LDataTableReturn = LDatabaseOperationReturn.DataSetReturn.Tables[0];
                    foreach (DataRow LDataRowSingleRent in LDataTableReturn.Rows)
                    {
                        LDataRowSingleRent["C002"] = EncryptionAndDecryption.EncryptDecryptString(LDataRowSingleRent["C002"].ToString(), IStrVerificationCode102, EncryptionAndDecryption.UMPKeyAndIVType.M102);
                        LDataRowSingleRent["C011"] = EncryptionAndDecryption.EncryptDecryptString(LDataRowSingleRent["C011"].ToString(), IStrVerificationCode102, EncryptionAndDecryption.UMPKeyAndIVType.M102);
                        LDataRowSingleRent["C012"] = EncryptionAndDecryption.EncryptDecryptString(LDataRowSingleRent["C012"].ToString(), IStrVerificationCode102, EncryptionAndDecryption.UMPKeyAndIVType.M102);
                        LStrRentToken = EncryptionAndDecryption.EncryptDecryptString(LDataRowSingleRent["C021"].ToString(), IStrVerificationCode102, EncryptionAndDecryption.UMPKeyAndIVType.M102);
                        while (!Regex.IsMatch(LStrRentToken, @"^\d{5}$"))
                        {
                            LStrRentToken = EncryptionAndDecryption.EncryptDecryptString(LDataRowSingleRent["C021"].ToString(), IStrVerificationCode102, EncryptionAndDecryption.UMPKeyAndIVType.M102);
                        }
                        LDataRowSingleRent["C021"] = LStrRentToken;
                        LDataRowSingleRent["C022"] = EncryptionAndDecryption.EncryptDecryptString(LDataRowSingleRent["C022"].ToString(), IStrVerificationCode102, EncryptionAndDecryption.UMPKeyAndIVType.M102);

                        //LStrRentToken = LDataRowSingleRent["C021"].ToString();

                        LStrDynamicSQL = "SELECT C017 FROM T_11_101_" + LStrRentToken + " WHERE C001 > 1030000000000000000 AND C001 < 1040000000000000000 AND C002 = 1 ORDER BY C001 ASC";
                        LDatabaseOperationReturn = LDataOperations.SelectDataByDynamicSQL(IIntDatabaseType, IStrDatabaseProfile, LStrDynamicSQL);
                        if (LDatabaseOperationReturn.BoolReturn)
                        {
                            foreach (DataRow LDataRowSingleAgent in LDatabaseOperationReturn.DataSetReturn.Tables[0].Rows)
                            {
                                LStrSingleObject = EncryptionAndDecryption.EncryptDecryptString(LDataRowSingleAgent["C017"].ToString(), IStrVerificationCode102, EncryptionAndDecryption.UMPKeyAndIVType.M102);
                                LStrSingleObject = LStrRentToken + IStrSpliterChar + "103" + IStrSpliterChar + LStrSingleObject;
                                AListStrRentExistObjects.Add(LStrSingleObject);
                            }
                        }

                        LStrDynamicSQL = "SELECT C017 FROM T_11_101_" + LStrRentToken + " WHERE C001 > 1070000000000000000 AND C001 < 1080000000000000000 AND C002 = 1 ORDER BY C001 ASC";
                        LDatabaseOperationReturn = LDataOperations.SelectDataByDynamicSQL(IIntDatabaseType, IStrDatabaseProfile, LStrDynamicSQL);
                        if (LDatabaseOperationReturn.BoolReturn)
                        {
                            foreach (DataRow LDataRowSingleAgent in LDatabaseOperationReturn.DataSetReturn.Tables[0].Rows)
                            {
                                LStrSingleObject = EncryptionAndDecryption.EncryptDecryptString(LDataRowSingleAgent["C017"].ToString(), IStrVerificationCode102, EncryptionAndDecryption.UMPKeyAndIVType.M102);
                                LStrSingleObject = LStrRentToken + IStrSpliterChar + "107" + IStrSpliterChar + LStrSingleObject;
                                AListStrRentExistObjects.Add(LStrSingleObject);
                            }
                        }

                        LStrDynamicSQL = "SELECT C017 FROM T_11_101_" + LStrRentToken + " WHERE C001 > 1080000000000000000 AND C001 < 1090000000000000000 AND C002 = 1 ORDER BY C001 ASC";
                        LDatabaseOperationReturn = LDataOperations.SelectDataByDynamicSQL(IIntDatabaseType, IStrDatabaseProfile, LStrDynamicSQL);
                        if (LDatabaseOperationReturn.BoolReturn)
                        {
                            foreach (DataRow LDataRowSingleAgent in LDatabaseOperationReturn.DataSetReturn.Tables[0].Rows)
                            {
                                LStrSingleObject = EncryptionAndDecryption.EncryptDecryptString(LDataRowSingleAgent["C017"].ToString(), IStrVerificationCode102, EncryptionAndDecryption.UMPKeyAndIVType.M102);
                                LStrSingleObject = LStrRentToken + IStrSpliterChar + "108" + IStrSpliterChar + LStrSingleObject;
                                AListStrRentExistObjects.Add(LStrSingleObject);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LDataTableReturn = null;
                FileLog.WriteError("ObtainRentList()1", ex.ToString());
            }

            return LDataTableReturn;
        }
        #endregion

        #region 根据数据信息获取租户的Token
        private string GetRentTokenByDataRow(DataTable ADataTableRentList)
        {
            string LStrReturn = string.Empty;

            try
            {
                LStrReturn = ADataTableRentList.Rows[0]["C021"].ToString();
            }
            catch (Exception ex)
            {
                LStrReturn = string.Empty;
                FileLog.WriteError("GetRentTokenByDataRow()", ex.Message);
            }

            return LStrReturn;
        }
        #endregion

        #region 根据数据信息获取租户的Token
        private string GetRentTokenByDataRow(DataRow ADataRowTableData, string AStrTableName, DataTable ADataTableRentList)
        {
            string LStrReturn = string.Empty;

            try
            {
                if (AStrTableName == "T_21_001")
                {
                    LStrReturn = ADataTableRentList.Rows[0]["C021"].ToString();
                }
                if (AStrTableName == "T_11_901")
                {
                    LStrReturn = ADataRowTableData["C022"].ToString();
                }
            }
            catch (Exception ex)
            {
                LStrReturn = string.Empty;
                FileLog.WriteError("GetRentTokenByDataRow()1", ex.Message);
            }

            return LStrReturn;
        }
        #endregion

        #region 根据租户Token、表名获取已经存在逻辑分区表
        private DataTable ObtainRentExistLogicPartitionTables(string AStrRentToken, string AStrTableName)
        {
            DataTable LDataTableReturn = new DataTable();
            string LStrDynamicSQL = string.Empty;

            try
            {
                DatabaseOperation01Return LDatabaseOperationReturn = new DatabaseOperation01Return();
                DataOperations01 LDataOperations = new DataOperations01();

                if (IIntDatabaseType == 2)
                {
                    LStrDynamicSQL = "SELECT NAME AS TABLE_NAME FROM SYSOBJECTS WHERE NAME LIKE '" + AStrTableName + "_" + AStrRentToken + "_%' ORDER BY NAME ASC";
                }
                if (IIntDatabaseType == 3)
                {
                    LStrDynamicSQL = "SELECT TABLE_NAME FROM USER_TABLES WHERE TABLE_NAME LIKE '" + AStrTableName + "_" + AStrRentToken + "_%' ORDER BY TABLE_NAME ASC";
                }
                LDatabaseOperationReturn = LDataOperations.SelectDataByDynamicSQL(IIntDatabaseType, IStrDatabaseProfile, LStrDynamicSQL);
                if (!LDatabaseOperationReturn.BoolReturn)
                {
                    LDataTableReturn = null;
                    FileLog.WriteError("ObtainRentExistLogicPartitionTables()", LStrDynamicSQL + "; " + LDatabaseOperationReturn.StrReturn);
                }
                else
                {
                    LDataTableReturn = LDatabaseOperationReturn.DataSetReturn.Tables[0];
                }
            }
            catch (Exception ex)
            {
                LDataTableReturn = null;
                FileLog.WriteError("ObtainRentExistLogicPartitionTables()", ex.Message);
            }
            return LDataTableReturn;
        }
        #endregion

        #region 读取数据库连接信息
        /// <summary>
        /// 读取数据库连接信息
        /// </summary>
        private void ReadDatabaseConnectionProfile()
        {
            while (IIntDatabaseType == 0 || string.IsNullOrEmpty(IStrDatabaseProfile))
            {
                if (!IBoolCanContinue) { break; }
                GetDatabaseConnectionProfile();
                Thread.Sleep(1000);
            }
        }
        #endregion

        #region 获取数据库连接信息
        private void GetDatabaseConnectionProfile()
        {
            string LStrXmlFileName = string.Empty;
            string LStrVerificationCode = string.Empty;

            string LStrAttributesData = string.Empty;
            //0:数据库服务器；1：端口；2：数据库名或服务名；3：登录用户；4：登录密码；5：其他参数
            List<string> LListStrDBProfile = new List<string>();

            try
            {
                IIntDatabaseType = 0;
                IStrDatabaseProfile = string.Empty;

                LStrVerificationCode = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104);

                LStrXmlFileName = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                LStrXmlFileName = System.IO.Path.Combine(LStrXmlFileName, @"UMP.Server\Args01.UMP.xml");

                XmlDocument LXmlDocArgs01 = new XmlDocument();
                LXmlDocArgs01.Load(LStrXmlFileName);
                XmlNodeList LXmlNodeListDatabase = LXmlDocArgs01.SelectSingleNode("DatabaseParameters").ChildNodes;

                #region 读取数据库连接参数
                foreach (XmlNode LXmlNodeSingleDatabase in LXmlNodeListDatabase)
                {
                    LStrAttributesData = LXmlNodeSingleDatabase.Attributes["P03"].Value;
                    LStrAttributesData = EncryptionAndDecryption.EncryptDecryptString(LStrAttributesData, LStrVerificationCode, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                    if (LStrAttributesData != "1") { continue; }

                    //数据库类型
                    LStrAttributesData = LXmlNodeSingleDatabase.Attributes["P02"].Value;
                    LStrAttributesData = EncryptionAndDecryption.EncryptDecryptString(LStrAttributesData, LStrVerificationCode, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                    IIntDatabaseType = int.Parse(LStrAttributesData);

                    //数据库服务器名或IP地址
                    LStrAttributesData = LXmlNodeSingleDatabase.Attributes["P04"].Value;
                    LStrAttributesData = EncryptionAndDecryption.EncryptDecryptString(LStrAttributesData, LStrVerificationCode, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                    LListStrDBProfile.Add(LStrAttributesData);

                    //数据库服务端口
                    LStrAttributesData = LXmlNodeSingleDatabase.Attributes["P05"].Value;
                    LStrAttributesData = EncryptionAndDecryption.EncryptDecryptString(LStrAttributesData, LStrVerificationCode, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                    LListStrDBProfile.Add(LStrAttributesData);

                    //数据库名或Service Name
                    LStrAttributesData = LXmlNodeSingleDatabase.Attributes["P06"].Value;
                    LStrAttributesData = EncryptionAndDecryption.EncryptDecryptString(LStrAttributesData, LStrVerificationCode, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                    LListStrDBProfile.Add(LStrAttributesData);

                    //登录用户
                    LStrAttributesData = LXmlNodeSingleDatabase.Attributes["P07"].Value;
                    LStrAttributesData = EncryptionAndDecryption.EncryptDecryptString(LStrAttributesData, LStrVerificationCode, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                    LListStrDBProfile.Add(LStrAttributesData);

                    //登录密码
                    LStrAttributesData = LXmlNodeSingleDatabase.Attributes["P08"].Value;
                    LStrAttributesData = EncryptionAndDecryption.EncryptDecryptString(LStrAttributesData, LStrVerificationCode, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                    LListStrDBProfile.Add(LStrAttributesData);

                    //其他参数
                    LStrAttributesData = LXmlNodeSingleDatabase.Attributes["P09"].Value;
                    LStrAttributesData = EncryptionAndDecryption.EncryptDecryptString(LStrAttributesData, LStrVerificationCode, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                    LListStrDBProfile.Add(LStrAttributesData);

                    break;
                }
                #endregion

                #region 创建数据库连接字符串
                string LStrDBConnectProfile = string.Empty;

                if (IIntDatabaseType == 2)
                {
                    IStrDatabaseProfile = string.Format("Data Source={0},{1};Initial Catalog={2};User Id={3};Password={4}", LListStrDBProfile[0], LListStrDBProfile[1], LListStrDBProfile[2], LListStrDBProfile[3], LListStrDBProfile[4]);
                    LStrDBConnectProfile = string.Format("Data Source={0},{1};Initial Catalog={2};User Id={3};Password={4}", LListStrDBProfile[0], LListStrDBProfile[1], LListStrDBProfile[2], LListStrDBProfile[3], "******");
                    LStrDBConnectProfile = "DataBase Type : MS SQL Server\n" + LStrDBConnectProfile;
                }
                if (IIntDatabaseType == 3)
                {
                    IStrDatabaseProfile = string.Format("Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST={0}) (PORT={1})))(CONNECT_DATA=(SERVICE_NAME= {2})));User Id={3}; Password={4}", LListStrDBProfile[0], LListStrDBProfile[1], LListStrDBProfile[2], LListStrDBProfile[3], LListStrDBProfile[4]);
                    LStrDBConnectProfile = string.Format("Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST={0}) (PORT={1})))(CONNECT_DATA=(SERVICE_NAME= {2})));User Id={3}; Password={4}", LListStrDBProfile[0], LListStrDBProfile[1], LListStrDBProfile[2], LListStrDBProfile[3], "******");
                    LStrDBConnectProfile = "DataBase Type : Oracle\n" + LStrDBConnectProfile;
                }
                #endregion

                FileLog.WriteInfo("GetDatabaseConnectionProfile()", LStrDBConnectProfile);
            }
            catch (Exception ex)
            {
                IIntDatabaseType = 0;
                IStrDatabaseProfile = string.Empty;
                FileLog.WriteError("GetDatabaseConnectionProfile()", ex.Message);
            }
        }
        #endregion

        #region 获取UMP.PF站点物理路径
        private string GetSiteBaseDirectory()
        {
            string LStrReturn = string.Empty;

            try
            {
                LStrReturn = AppDomain.CurrentDomain.BaseDirectory;
                string[] LStrDirectoryArray = LStrReturn.Split('\\');
                LStrReturn = string.Empty;
                foreach (string LStrDirectorySingle in LStrDirectoryArray)
                {
                    if (LStrDirectorySingle.ToLower() == "winservices") { break; }
                    LStrReturn += LStrDirectorySingle + "\\";
                }
                FileLog.WriteInfo("GetSiteBaseDirectory()", "UMP.PF Site Directory : " + LStrReturn);
            }
            catch (Exception ex)
            {
                LStrReturn = string.Empty;
                FileLog.WriteError("GetSiteBaseDirectory()", ex.Message);
            }

            return LStrReturn;
        }
        #endregion

        #region 在 windows 的事件中写入日志
        private void WriteEntryLog(string AStrWriteBody, EventLogEntryType AEntryType)
        {
            try
            {
                UMPServiceLog.WriteEntry(AStrWriteBody, AEntryType);
            }
            catch { }
        }
        #endregion

        #region 创建加密解密验证字符串
        private string CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType AKeyIVID)
        {
            string LStrReturn = string.Empty;
            int LIntRand = 0;
            string LStrTemp = string.Empty;

            try
            {
                Random LRandom = new Random();
                LStrReturn = DateTime.Now.ToString("yyyyMMddHHmmss");
                LIntRand = LRandom.Next(0, 14);
                LStrTemp = LIntRand.ToString("00");
                LStrReturn = LStrReturn.Insert(LIntRand, "VCT");
                LIntRand = LRandom.Next(0, 17);
                LStrTemp += LIntRand.ToString("00");
                LStrReturn = LStrReturn.Insert(LIntRand, "UMP");
                LIntRand = LRandom.Next(0, 20);
                LStrTemp += LIntRand.ToString("00");
                LStrReturn = LStrReturn.Insert(LIntRand, ((int)AKeyIVID).ToString("000"));

                LStrReturn = EncryptionAndDecryption.EncryptStringY(LStrTemp + LStrReturn);
            }
            catch { LStrReturn = string.Empty; }

            return LStrReturn;
        }
        #endregion

        #region 创建分割字符 char(27)
        private string AscCodeToChr(int AsciiCode)
        {
            System.Text.ASCIIEncoding asciiEncoding = new System.Text.ASCIIEncoding();
            byte[] byteArray = new byte[] { (byte)AsciiCode };
            string strCharacter = asciiEncoding.GetString(byteArray);
            return (strCharacter);
        }
        #endregion

        #region 获取UMP.PF站点绑定的http端口
        private int GetUMPPFBasicHttpPort()
        {
            int LIntReturn = 0;
            string LStrXmlFileName = string.Empty;

            try
            {
                FileLog.WriteInfo("GetRentTokenByDataRow()", "Read UMP.PF Binding Port Information ...");

                LStrXmlFileName = System.IO.Path.Combine(IStrSiteBaseDirectory, @"GlobalSettings\UMP.Server.01.xml");
                XmlDocument LXmlDocServer01 = new XmlDocument();
                LXmlDocServer01.Load(LStrXmlFileName);
                XmlNode LXMLNodeSection = LXmlDocServer01.SelectSingleNode("UMPSetted").SelectSingleNode("IISBindingProtocol");
                XmlNodeList LXmlNodeBindingProtocol = LXMLNodeSection.ChildNodes;
                foreach (XmlNode LXmlNodeSingleBinding in LXmlNodeBindingProtocol)
                {
                    if (LXmlNodeSingleBinding.Attributes["Protocol"].Value == "http")
                    {
                        LIntReturn = int.Parse(LXmlNodeSingleBinding.Attributes["BindInfo"].Value);
                        break;
                    }
                }

                FileLog.WriteInfo("GetRentTokenByDataRow()", "Readed UMP.PF Binding Port : " + LIntReturn.ToString() + ", This Service Use " + (LIntReturn - 2).ToString());
            }
            catch (Exception ex)
            {
                LIntReturn = 0;
                FileLog.WriteError("GetRentTokenByDataRow()", "Read UMP.PF Binding Port Information Failed." + ex.Message);
            }

            return LIntReturn;
        }
        #endregion

        #region OTHER
        private string Convert2String(object obj)
        {
            return obj == null ? "" : obj.ToString();
        }
        private long Convert2Long(object obj)
        {
            return obj == null ? -1 : Convert.ToInt64(obj.ToString());
        }
        private int Convert2Int(object obj)
        {
            if (obj == null || string.IsNullOrEmpty(obj.ToString()))
                return -1;
            else
                return Convert.ToInt32(obj.ToString());
        }
        private int GetDayOfWeek(DateTime dt)
        {
            if (dt.DayOfWeek == DayOfWeek.Monday)
            {
                return 1;
            }
            else if (dt.DayOfWeek == DayOfWeek.Tuesday)
            {
                return 2;
            }
            else if (dt.DayOfWeek == DayOfWeek.Wednesday)
            {
                return 3;
            }
            else if (dt.DayOfWeek == DayOfWeek.Thursday)
            {
                return 4;
            }
            else if (dt.DayOfWeek == DayOfWeek.Friday)
            {
                return 5;
            }
            else if (dt.DayOfWeek == DayOfWeek.Saturday)
            {
                return 6;
            }
            else if (dt.DayOfWeek == DayOfWeek.Sunday)
            {
                return 7;
            }
            return 0;
        }

        /// <summary>
        /// 得到本月最多天数
        /// </summary>
        /// <returns></returns>
        protected int GetMonthMaxDay()
        {
            int maxday = 0;
            int month = DateTime.Now.Month;
            int year = DateTime.Now.Year;
            switch (month)
            {
                case 2:
                    {
                        if ((year % 4 == 0 && year % 100 != 0) || (year % 400 == 0))
                        {
                            maxday = 29;
                        }
                        else
                            maxday = 28;
                    }
                    break;
                case 1:
                case 3:
                case 5:
                case 7:
                case 8:
                case 10:
                case 12:
                    maxday = 31;
                    break;
                case 4:
                case 6:
                case 9:
                case 11:
                    maxday = 30;
                    break;

            }
            return maxday;
        }

        private int IntParse(string str, int defaultValue)
        {
            int outRet = defaultValue;
            int.TryParse(str, out outRet);

            return outRet;
        }

        private string GetLogPath()
        {
            string strReturn = string.Empty;
            try
            {
                //从LocalMachine文件中读取日志路径
                string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                    @"VoiceCyber\UMP\config\localmachine.ini");
                if (File.Exists(path))
                {
                    string[] arrInfos = File.ReadAllLines(path, Encoding.Default);
                    for (int i = 0; i < arrInfos.Length; i++)
                    {
                        string strInfo = arrInfos[i];
                        if (strInfo.StartsWith("LogPath="))
                        {
                            string str = strInfo.Substring(8);
                            if (!string.IsNullOrEmpty(str))
                            {
                                strReturn = str;
                                break;
                            }
                        }
                    }
                }
                if (string.IsNullOrEmpty(strReturn)
                    || !Directory.Exists(strReturn))
                {
                    //如果读取失败，或者目录不存在，使用默认目录
                    strReturn = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                        "UMP\\UMPService05\\Logs");
                }
                else
                {
                    strReturn = Path.Combine(strReturn, "UMPService05");
                }
            }
            catch {
                strReturn = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                            "UMP\\UMPService05\\Logs");
            }

            //创建日志文件夹
            if (!Directory.Exists(strReturn))
            {
                Directory.CreateDirectory(strReturn);
            }
            return strReturn;
        }
        #endregion               
        #endregion
    }
}
