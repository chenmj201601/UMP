using CircleQueueClass;
using Oracle.DataAccess.Client;
using PFShareClasses01;
using PFShareClassesS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceProcess;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;
using VoiceCyber.Common;
using System.Reflection;

namespace UMPService02
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant, InstanceContextMode = InstanceContextMode.Single)]
    public partial class UMPService02 : ServiceBase
    {
        public static string IStrEventLogSource = "Service 02";
        public static string IStrApplicationName = "UMP";
        public static string IStrSpliterChar = string.Empty;

        private static LogOperator mLogOperator;


        private int IIntThisServicePort = 0;
        private string IStrSiteBaseDirectory = string.Empty;
        private int IIntSiteHttpBindingPort = 0;

        private static EventLog IEventLog = null;

        private bool IBoolCanContinue = true;

        private Thread IThreadReadDBProfile;

        private Thread IThreadLogicalPartition;
        private bool IBoolInDoing = false;
        private bool IBoolCanAbort = false;

        private Thread IThreadAnasisMediaData;
        private bool IBooInAnasis = false;
        private bool IBoolCanAbortAnasis = false;

        private Thread IThreadClearData;
        private bool IBooInClear = false;
        private bool IBoolCanAbortClear = false;

        private Thread IThreadMove21Data;
        private bool IBooInMove21 = false;
        private bool IBoolCanAbortMove21 = false;

        private int IIntDatabaseType = 0;
        private string IStrDatabaseProfile = string.Empty;

        private string IStrVerificationCode102 = string.Empty;
        private string IStrVerificationCode002 = string.Empty;
        private List<CCustomField> ILstCustomFieldConfig = new List<CCustomField>();

        private YoungCircleQueue<string> IYoungCircleQueue = new YoungCircleQueue<string>(512);

        private static readonly object ILockObject = new object();

        private Thread IThreadDoFieldEncrypt;
        private Queue<string> IQueueField998Data = new Queue<string>();
        private static readonly object ILockObject998 = new object();

        public UMPService02()
        {
            InitializeComponent();
            //OnStart(null);
        }

        #region 服务的启动与停止
        protected override void OnStart(string[] args)
        {
            CreateFileLog();
            WriteLog(LogMode.Info, string.Format("Service starting..."));

            Thread LThreadStartService = new Thread(new ThreadStart(StartService02));
            LThreadStartService.Start();
        }

        /*private void CreateFileLog()
        {
            throw new NotImplementedException();
        }

        private void WriteLog(LogMode logMode, string p)
        {
            throw new NotImplementedException();
        }*/

        protected override void OnStop()
        {
            try
            {
                WriteEntryLog("Stop Service 02", EventLogEntryType.Information);
                IBoolCanContinue = false;

                while (IBoolInDoing) { System.Threading.Thread.Sleep(100); }
                while (!IBoolCanAbort) { System.Threading.Thread.Sleep(100); }

                while (IBooInAnasis) { System.Threading.Thread.Sleep(100); }
                while (!IBoolCanAbortAnasis) { System.Threading.Thread.Sleep(100); }

                while (IBooInClear) { System.Threading.Thread.Sleep(100); }
                while (!IBoolCanAbortClear) { System.Threading.Thread.Sleep(100); }

                while (IBooInMove21) { System.Threading.Thread.Sleep(100); }
                while (!IBoolCanAbortMove21) { System.Threading.Thread.Sleep(100); }

                Thread.Sleep(500);
                IThreadLogicalPartition.Abort();
                IThreadAnasisMediaData.Abort();
                IThreadClearData.Abort();
                IThreadMove21Data.Abort();
                IThreadReadDBProfile.Abort();
                IThreadDoFieldEncrypt.Abort();

                WriteEntryLog("Service 02 Stopped", EventLogEntryType.Information);

                if (mLogOperator != null)
                {
                    mLogOperator.Stop();
                }
            }
            catch (Exception ex)
            {
                WriteEntryLog("Stop Service 02 Exception:\n" + ex.ToString(), EventLogEntryType.Error);
            }
        }
        #endregion

        #region 启动 Service 02
        private void StartService02()
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

                WriteEntryLog("Service Started At : " + DateTime.Now.ToString("G"), EventLogEntryType.Information);

                IStrSiteBaseDirectory = GetSiteBaseDirectory();
                IIntSiteHttpBindingPort = GetUMPPFBasicHttpPort();
                IIntThisServicePort = IIntSiteHttpBindingPort - 2;

                IThreadReadDBProfile = new Thread(new ThreadStart(ReadDatabaseConnectionProfile));
                IThreadReadDBProfile.Start();

                IThreadMove21Data = new Thread(new ThreadStart(Move21000221001));
                IThreadMove21Data.Start();

                IThreadLogicalPartition = new Thread(new ThreadStart(LogicPartitionDataMain));
                IThreadLogicalPartition.Start();

                IThreadAnasisMediaData = new Thread(new ThreadStart(ObjectInfoInMediaData));
                IThreadAnasisMediaData.Start();

                IThreadClearData = new Thread(new ThreadStart(ClearTemporaryData));
                IThreadClearData.Start();

                IThreadDoFieldEncrypt = new Thread(new ThreadStart(OperationEncryptData));
                IThreadDoFieldEncrypt.Start();
            }
            catch (Exception ex)
            {
                WriteEntryLog("StartService02()\n" + ex.ToString(), EventLogEntryType.Error);
            }
        }
        #endregion

        #region 读取数据库连接信息
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

        #region 将录音数据从T_21_000转移到T_21_001
        private void Move21000221001()
        {
            string LStrDynamicSQL = string.Empty;

            string LStrC000 = string.Empty;
            string LStrC001 = string.Empty, LStrC002 = string.Empty, LStrC003 = string.Empty, LStrC004 = string.Empty, LStrC005 = string.Empty;
            string LStrC006 = string.Empty, LStrC007 = string.Empty, LStrC008 = string.Empty, LStrC009 = string.Empty, LStrC010 = string.Empty;
            string LStrC103 = string.Empty;
            string LStrAllColumnData = string.Empty;
            string LStrColumnName = string.Empty, LStrColumnData = string.Empty;

            string LStrTable21001Xml = string.Empty;
            string LStrDataType = string.Empty;

            long LLong21001C002 = 0;

            try
            {
                DatabaseOperation01Return LDatabaseReturn = new DatabaseOperation01Return();
                DatabaseOperation01Return LDatabaseReturn2 = new DatabaseOperation01Return();
                DataOperations01 LDataOperations = new DataOperations01();
                SqlConnection LSqlConnection = null;
                OracleConnection LOracleConnection = null;

                while (IBoolCanContinue)
                {
                    if (IIntDatabaseType == 0 || string.IsNullOrEmpty(IStrDatabaseProfile)) { IBooInMove21 = false; Thread.Sleep(2000); continue; }
                    //IStrDatabaseProfile = "Data Source=192.168.4.182,1433;Initial Catalog=UMPDataDB0419;User Id=PFDEV;Password=PF,123";
                    IBooInMove21 = true;

                    GetCustomFieldConfig();

                    #region 加载T_21_001配置文件
                    LStrTable21001Xml = System.IO.Path.Combine(IStrSiteBaseDirectory, @"MAMT\DBObjects\8.03.002\00-T\T_21_001.XML");
                    XmlDocument LXmlDocumentTable = new XmlDocument();
                    LXmlDocumentTable.Load(LStrTable21001Xml);
                    XmlNode LXMLNodeTableDefine = LXmlDocumentTable.SelectSingleNode("TableDefine");
                    XmlNodeList LXmlNodeListTableColumns = LXMLNodeTableDefine.SelectSingleNode("ColumnDefine").ChildNodes;
                    #endregion

                    #region 转移错误数据
                    try
                    {
                        LStrDynamicSQL = "INSERT INTO T_21_999 SELECT * FROM T_21_000 WHERE C000 < -1";
                        LDatabaseReturn = LDataOperations.ExecuteDynamicSQL(IIntDatabaseType, IStrDatabaseProfile, LStrDynamicSQL);
                        if (!LDatabaseReturn.BoolReturn)
                        {
                            WriteEntryLog("Move21000221001()\n" + LStrDynamicSQL + "\n" + LDatabaseReturn.StrReturn, EventLogEntryType.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        WriteEntryLog("Move21000221001()\n" + LStrDynamicSQL + "\n" + ex.ToString(), EventLogEntryType.Error);
                    }
                    #endregion

                    #region 查询出 50 条录音数据
                    LStrDynamicSQL = string.Empty;
                    if (IIntDatabaseType == 2)
                    {
                        LStrDynamicSQL = "SELECT TOP 50 * FROM T_21_000 WHERE C000 > 0";
                    }
                    if (IIntDatabaseType == 3)
                    {
                        LStrDynamicSQL = "SELECT * FROM T_21_000 WHERE C000 > 0 AND ROWNUM <= 50";
                    }
                    if (string.IsNullOrEmpty(LStrDynamicSQL)) { continue; }
                    LDatabaseReturn = LDataOperations.SelectDataByDynamicSQL(IIntDatabaseType, IStrDatabaseProfile, LStrDynamicSQL);
                    if (!LDatabaseReturn.BoolReturn)
                    {
                        WriteEntryLog("Move21000221001()\n" + LStrDynamicSQL + "\n" + LDatabaseReturn.StrReturn, EventLogEntryType.Error);
                        IBooInMove21 = false; Thread.Sleep(100); continue;
                    }
                    #endregion

                    #region 逐条分析，写入T_21_001
                    foreach (DataRow LDataRowSingle in LDatabaseReturn.DataSetReturn.Tables[0].Rows)
                    {
                        LStrC000 = LDataRowSingle["C000"].ToString();
                        LStrC001 = LDataRowSingle["C001"].ToString(); LStrC002 = LDataRowSingle["C002"].ToString();
                        LStrC003 = LDataRowSingle["C003"].ToString(); LStrC004 = LDataRowSingle["C004"].ToString();
                        LStrC005 = LDataRowSingle["C005"].ToString(); LStrC006 = LDataRowSingle["C006"].ToString();
                        LStrC007 = LDataRowSingle["C007"].ToString(); LStrC008 = LDataRowSingle["C008"].ToString();
                        LStrC009 = LDataRowSingle["C009"].ToString(); LStrC010 = LDataRowSingle["C010"].ToString();
                        LStrC103 = LDataRowSingle["C103"].ToString();
                        LStrAllColumnData = LStrC001 + LStrC002 + LStrC003 + LStrC004 + LStrC005 + LStrC006 + LStrC007 + LStrC008 + LStrC009 + LStrC010;
                        LStrAllColumnData = LStrAllColumnData.Replace(IStrSpliterChar + IStrSpliterChar, AscCodeToChr(30));
                        string[] LStrArrayColumnAndData = LStrAllColumnData.Split(AscCodeToChr(30).ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                        try
                        {
                            LStrDynamicSQL = "SELECT * FROM T_21_001 WHERE 1 = 2";
                            LDatabaseReturn2 = LDataOperations.SelectDataByDynamicSQL(IIntDatabaseType, IStrDatabaseProfile, LStrDynamicSQL);
                            DataSet LDataSetSave2DB = LDatabaseReturn2.DataSetReturn;
                            if (LDataSetSave2DB.Tables.Count <= 0) { continue; }
                            LDataSetSave2DB.Tables[0].TableName = "T_21_001";
                            DataRow LDataRowNew21001 = LDataSetSave2DB.Tables[0].NewRow();
                            LDataRowNew21001.BeginEdit();

                            #region 传入的内容

                            foreach (string LStrSingleColumnAndData in LStrArrayColumnAndData)
                            {
                                //WriteEntryLog("Move21000221001()\nWrite Data To T_21_001\n" + LStrSingleColumnAndData, EventLogEntryType.Warning);
                                string[] LStrSingleObject = LStrSingleColumnAndData.Split(IStrSpliterChar.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                                if (LStrSingleObject.Length != 2) { continue; }

                                //匹配Args04.UMP.xml里面的客户化字段 替换为相应数据库字段
                                CCustomField cur = ILstCustomFieldConfig.Where(p => p.CustomName == LStrSingleObject[0]).FirstOrDefault();
                                if (cur != null)
                                    LStrColumnName = cur.DBColumn.ToUpper();
                                else
                                    LStrColumnName = LStrSingleObject[0].ToUpper();
                                LStrColumnData = LStrSingleObject[1];
                                LStrDataType = GetColumnDataType(LStrColumnName, LXmlNodeListTableColumns);
                                if (string.IsNullOrEmpty(LStrDataType)) { continue; }
                                switch (LStrDataType)
                                {
                                    case "01":
                                        LDataRowNew21001[LStrColumnName] = Int16.Parse(LStrColumnData);
                                        break;
                                    case "02":
                                        LDataRowNew21001[LStrColumnName] = int.Parse(LStrColumnData);
                                        break;
                                    case "03":
                                        LDataRowNew21001[LStrColumnName] = long.Parse(LStrColumnData);
                                        break;
                                    case "04":
                                        LDataRowNew21001[LStrColumnName] = decimal.Parse(LStrColumnData);
                                        break;
                                    case "11":
                                        LDataRowNew21001[LStrColumnName] = LStrColumnData;
                                        break;
                                    case "12":
                                        LDataRowNew21001[LStrColumnName] = LStrColumnData;
                                        break;
                                    case "13":
                                        LDataRowNew21001[LStrColumnName] = LStrColumnData;
                                        break;
                                    case "14":
                                        LDataRowNew21001[LStrColumnName] = LStrColumnData;
                                        break;
                                    case "21":
                                        LDataRowNew21001[LStrColumnName] = DateTime.Parse(LStrColumnData);
                                        break;
                                    default:
                                        LDataRowNew21001[LStrColumnName] = LStrColumnData;
                                        break;
                                }
                            }
                            #endregion

                            #region 附加内容
                            LLong21001C002 = 0;
                            LDataRowNew21001["C001"] = long.Parse(LStrC000);
                            while (LLong21001C002 == 0)
                            {
                                LLong21001C002 = GetSerialIDByType(LStrC000, IIntDatabaseType, IStrDatabaseProfile, "00000", 21, 201, (DateTime.Parse(LDataRowNew21001["C005"].ToString())).ToString("yyyyMMddHHmmss"));
                            }
                            LDataRowNew21001["C002"] = LLong21001C002;
                            TimeSpan LTimeSpan = new TimeSpan(0, 0, int.Parse(LDataRowNew21001["C012"].ToString()));
                            LDataRowNew21001["C013"] = LTimeSpan.Hours.ToString("00") + ":" + LTimeSpan.Minutes.ToString("00") + ":" + LTimeSpan.Seconds.ToString("00");
                            LDataRowNew21001["C016"] = long.Parse(DateTime.Parse(LStrC103).ToString("yyyyMMddHHmmssfff"));
                            LDataRowNew21001["C017"] = long.Parse(DateTime.UtcNow.ToString("yyyyMMddHHmmssfff"));
                            DateTime LDataTimeLocal = DateTime.Parse(LDataRowNew21001["C004"].ToString());
                            DateTime LDataTimeUtc = DateTime.Parse(LDataRowNew21001["C005"].ToString());
                            TimeSpan LTimeSpanDiff = LDataTimeUtc - LDataTimeLocal;
                            LDataRowNew21001["C018"] = (Int16)LTimeSpanDiff.TotalMinutes;
                            #endregion

                            #region 默认值设定   
                            if (LDataRowNew21001["C110"] == null || LDataRowNew21001["C110"].ToString() == "")
                                LDataRowNew21001["C110"] = "127.0.0.1";
                            if (LDataRowNew21001["C039"] == null || LDataRowNew21001["C039"].ToString() == "" || LDataRowNew21001["C039"].ToString() == " ")//如果坐席为空，使用分机号替换
                                LDataRowNew21001["C039"] = LDataRowNew21001["C042"];
                            if (LDataRowNew21001["C058"] == null || LDataRowNew21001["C058"].ToString() == "")//如果真实分机为空，使用分机号替换
                                LDataRowNew21001["C058"] = LDataRowNew21001["C042"];
                            for (int i = 200; i <= 240; i++)
                            {
                                if (LDataRowNew21001["C" + i.ToString()] == null || LDataRowNew21001["C" + i.ToString()].ToString() == "")
                                    LDataRowNew21001["C" + i.ToString()] = "0";
                            }
                            #endregion

                            LDataRowNew21001.EndEdit();
                            LDataSetSave2DB.Tables[0].Rows.Add(LDataRowNew21001);

                            #region 将记录写入MSSQL数据库
                            if (IIntDatabaseType == 2)
                            {
                                LSqlConnection = new SqlConnection(IStrDatabaseProfile);
                                SqlDataAdapter LSqlDataAdapter = new SqlDataAdapter(LStrDynamicSQL, LSqlConnection);
                                SqlCommandBuilder LSqlCommandBuilder = new SqlCommandBuilder();

                                LSqlCommandBuilder.ConflictOption = ConflictOption.OverwriteChanges;
                                LSqlCommandBuilder.SetAllValues = false;
                                LSqlCommandBuilder.DataAdapter = LSqlDataAdapter;
                                LSqlDataAdapter.Update(LDataSetSave2DB, "T_21_001");
                                LDataSetSave2DB.AcceptChanges();
                                LSqlCommandBuilder.Dispose();
                                LSqlDataAdapter.Dispose();
                            }
                            #endregion

                            #region 将记录志写入Oracle数据库
                            if (IIntDatabaseType == 3)
                            {
                                LOracleConnection = new OracleConnection(IStrDatabaseProfile);
                                OracleDataAdapter LOracleDataAdapter = new OracleDataAdapter(LStrDynamicSQL, LOracleConnection);
                                OracleCommandBuilder LOracleCommandBuilder = new OracleCommandBuilder();

                                LOracleCommandBuilder.ConflictOption = ConflictOption.OverwriteChanges;
                                LOracleCommandBuilder.SetAllValues = false;
                                LOracleCommandBuilder.DataAdapter = LOracleDataAdapter;
                                LOracleDataAdapter.Update(LDataSetSave2DB, "T_21_001");
                                LDataSetSave2DB.AcceptChanges();
                                LOracleCommandBuilder.Dispose();
                                LOracleDataAdapter.Dispose();
                            }
                            #endregion

                            #region C014=1 & C025=E数据写入998
                            try
                            {
                                if (LDataRowNew21001["C014"].ToString() == "1" && LDataRowNew21001["C025"].ToString().ToUpper() == "E")
                                {
                                    string strsql = string.Format("INSERT INTO T_21_998(C001,C002,C003,C004,C005,C006,C007,C008,C009,C010,C011,C012,C013,C014,C015,C016,C019,C020) SELECT C001,C002,C005,C006,C022,C020,C037,C038,C035,C036,C077,C042,C058,C039,C040,C041,C025,C110 FROM T_21_001 WHERE C001={0}"
                                        , LDataRowNew21001["C001"]);
                                    LDataOperations.ExecuteDynamicSQL(IIntDatabaseType, IStrDatabaseProfile, strsql);
                                }
                            }
                            catch {
                                lock (ILockObject998)
                                {
                                    IQueueField998Data.Enqueue(LDataRowNew21001["C001"].ToString());
                                }
                            }
                            #endregion

                            LStrDynamicSQL = "DELETE FROM T_21_000 WHERE C000 = " + LStrC000;
                            LDataOperations.ExecuteDynamicSQL(IIntDatabaseType, IStrDatabaseProfile, LStrDynamicSQL);
                        }
                        catch (Exception ex)
                        {
                            LStrDynamicSQL = "INSERT INTO T_21_999 SELECT * FROM T_21_000 WHERE C000 = " + LStrC000;
                            LDataOperations.ExecuteDynamicSQL(IIntDatabaseType, IStrDatabaseProfile, LStrDynamicSQL);

                            LStrDynamicSQL = "DELETE FROM T_21_000 WHERE C000 = " + LStrC000;
                            LDataOperations.ExecuteDynamicSQL(IIntDatabaseType, IStrDatabaseProfile, LStrDynamicSQL);

                            LDataOperations.ExecuteDynamicSQL(IIntDatabaseType, IStrDatabaseProfile, LStrDynamicSQL);
                            WriteEntryLog("Move21000221001()\nWrite Data To T_21_001\nC000 = " + LStrC000 + "\n" + ex.ToString(), EventLogEntryType.Error);
                        }
                        finally
                        {
                            if (LSqlConnection != null)
                            {
                                if (LSqlConnection.State == System.Data.ConnectionState.Open) { LSqlConnection.Close(); }
                                LSqlConnection.Dispose();
                            }
                            if (LOracleConnection != null)
                            {
                                if (LOracleConnection.State == ConnectionState.Open) { LOracleConnection.Close(); }
                                LOracleConnection.Dispose(); LOracleConnection = null;
                            }
                        }
                    }
                    #endregion

                    IBooInMove21 = false;
                    IBoolCanAbortMove21 = true;
                    System.Threading.Thread.Sleep(100);
                }
                IBoolCanAbortMove21 = true;
            }
            catch (Exception ex)
            {
                IBooInMove21 = false;
                IBoolCanAbortMove21 = true;
                WriteEntryLog("Move21000221001()\nC000 = " + LStrC000 + "\n" + LStrAllColumnData  + "\n" + ex.ToString(), EventLogEntryType.Error);
            }
        }

        private string GetColumnDataType(string AStrColumnName, XmlNodeList AXmlNodeListColumn)
        {
            string LStrDataType = string.Empty;
            string LStrColumnName = string.Empty;

            try
            {
                foreach (XmlNode LXmlNodeSingleColumn in AXmlNodeListColumn)
                {
                    LStrColumnName = LXmlNodeSingleColumn.Attributes["P01"].Value;
                    if (LStrColumnName != AStrColumnName) { continue; }
                    LStrDataType = LXmlNodeSingleColumn.Attributes["P02"].Value;
                    break;
                }
            }
            catch (Exception ex)
            {
                LStrDataType = string.Empty;
                WriteEntryLog("GetColumnDataType()\nColumn Name = " + LStrColumnName + "\n" + ex.ToString(), EventLogEntryType.Error);
            }
            return LStrDataType;
        }
        #endregion

        #region 处理座席、操作日志的机器名、IP
        private void ObjectInfoInMediaData()
        {
            DateTime LDateTimeLastReadArgs = DateTime.Now.AddMinutes(-6);
            string LStrPopObject = string.Empty;
            List<string> LListStrArguments = new List<string>();
            string LStrDynamicSQL = string.Empty;

            try
            {
                while (IBoolCanContinue)
                {
                    IBooInAnasis = true;

                    #region 每 5 分钟重新读取数据库连接参数
                    if (IIntDatabaseType == 0 || string.IsNullOrEmpty(IStrDatabaseProfile)) { IBooInAnasis = false; Thread.Sleep(2000); continue; }
                    //if (LDateTimeLastReadArgs.AddMinutes(5) <= DateTime.Now)
                    //{
                    //    GetDatabaseConnectionProfile();
                    //    if (IIntDatabaseType == 0 || string.IsNullOrEmpty(IStrDatabaseProfile))
                    //    {
                    //        IBooInAnasis = false; Thread.Sleep(6000); continue;
                    //    }

                    //    LDateTimeLastReadArgs = DateTime.Now;
                    //}
                    #endregion

                    #region 从循环队列中获取一个对象
                    lock (ILockObject)
                    {
                        if (!IYoungCircleQueue.CircleQueueIsEmpty())
                        {
                            LStrPopObject = IYoungCircleQueue.PopElement();
                        }
                        else
                        {
                            LStrPopObject = string.Empty;
                        }
                    }
                    if (string.IsNullOrEmpty(LStrPopObject)) { IBooInAnasis = false; System.Threading.Thread.Sleep(500); continue; }
                    #endregion

                    #region 将座席信息写入到数据库
                    LListStrArguments.Clear();
                    if (LStrPopObject.Contains(IStrSpliterChar + "103" + IStrSpliterChar))
                    {
                        //DatabaseOperation01Return LDatabaseOperation01Return = new DatabaseOperation01Return();
                        //DataOperations01 LDataOperations01 = new DataOperations01();
                        ////string LStrDataID = string.Empty;

                        try
                        {
                            string[] LStrAgentInfo = LStrPopObject.Split(IStrSpliterChar.ToArray());

                            #region 初始化参数
                            LListStrArguments.Add(IIntDatabaseType.ToString());                                           //0-数据库类型
                            LListStrArguments.Add(IStrDatabaseProfile);                                                   //1-数据库连接串
                            LListStrArguments.Add(LStrAgentInfo[0]);                                                      //2-租户TOKEN
                            LListStrArguments.Add("102" + LStrAgentInfo[0] + "00000000001");                              //3-该租户系统管理员
                            LListStrArguments.Add("A");                                                                   //4-表示增加座席
                            LListStrArguments.Add("0");                                                                   //5-
                            LListStrArguments.Add(LStrAgentInfo[2]);                                                      //6-座席ID
                            LListStrArguments.Add("B01" + IStrSpliterChar + "101" + LStrAgentInfo[0] + "00000000001");    //7-所属机构
                            LListStrArguments.Add("B07" + IStrSpliterChar + LStrAgentInfo[2]);                            //8-座席ID
                            LListStrArguments.Add("B08" + IStrSpliterChar + LStrAgentInfo[2]);                            //9-座席全名
                            LListStrArguments.Add("B02" + IStrSpliterChar + "1");                                         //10-
                            LListStrArguments.Add("U00" + IStrSpliterChar + "102" + LStrAgentInfo[0] + "00000000001");    //11-该租户系统管理员
                            #endregion

                            OperationDataArgs LOperationDataArgs = OperationA27(LListStrArguments);
                            if (!LOperationDataArgs.BoolReturn && LOperationDataArgs.StringReturn != "W000E03")
                            {
                                WriteEntryLog("Save Agent Information To Database Failed()\nOperationA27()\n" + LStrPopObject + "\n" + LOperationDataArgs.StringReturn, EventLogEntryType.Error);
                            }
                        }
                        catch (Exception ex)
                        {
                            WriteEntryLog("Save Agent Information To Database Failed()\nException\n" + ex.ToString(), EventLogEntryType.Error);
                        }
                    }
                    #endregion

                    #region 将操作日志中的机器名、IP写入到数据库
                    if (LStrPopObject.Contains(IStrSpliterChar + "107" + IStrSpliterChar) || LStrPopObject.Contains(IStrSpliterChar + "108" + IStrSpliterChar))
                    {
                        string LStrDataID = string.Empty;

                        try
                        {
                            string[] LStrObjectInfo = LStrPopObject.Split(IStrSpliterChar.ToArray());
                            LStrDataID = GetSerialIDByType(IIntDatabaseType, IStrDatabaseProfile, LStrObjectInfo[0], 11, int.Parse(LStrObjectInfo[1])).ToString();
                            LStrDynamicSQL = "INSERT INTO T_11_101_" + LStrObjectInfo[0] + "(C001, C002, C017) VALUES('" + LStrDataID + "', 1, '" + EncryptionAndDecryption.EncryptDecryptString(LStrObjectInfo[2], IStrVerificationCode002, EncryptionAndDecryption.UMPKeyAndIVType.M002) + "')";
                            DatabaseOperation01Return LDatabaseOperationReturn = new DatabaseOperation01Return();
                            DataOperations01 LDataOperations = new DataOperations01();
                            LDatabaseOperationReturn = LDataOperations.ExecuteDynamicSQL(IIntDatabaseType, IStrDatabaseProfile, LStrDynamicSQL);
                            if (!LDatabaseOperationReturn.BoolReturn)
                            {
                                WriteEntryLog("Save Computer Information To Database Failed()\n" + LStrPopObject + "\n" + LDatabaseOperationReturn.StrReturn, EventLogEntryType.Error);
                            }
                        }
                        catch (Exception ex)
                        {
                            WriteEntryLog("Save Computer Information To Database Failed()\nException\n" + ex.ToString(), EventLogEntryType.Error);
                        }
                    }
                    #endregion

                    IBooInAnasis = false;
                    IBoolCanAbortAnasis = true;
                    System.Threading.Thread.Sleep(500);
                }
                IBoolCanAbortAnasis = true;
            }
            catch (Exception ex)
            {
                IBooInAnasis = false;
                IBoolCanAbortAnasis = true;
                WriteEntryLog("ObjectInfoInMediaData()\n" + ex.ToString(), EventLogEntryType.Error);
            }
        }

        //保存座席信息
        private OperationDataArgs OperationA27(List<string> AListStringArgs)
        {
            OperationDataArgs LOperationReturn = new OperationDataArgs();
            string LStrDynamicSQL = string.Empty;
            string LStrTemp = string.Empty;
            string LStrUserDefaultPwd = string.Empty;
            string LStrVerificationCode002 = string.Empty;
            string LStrVerificationCode102 = string.Empty;
            string LStrDataTimeNow = string.Empty;
            string LStrDataID = string.Empty;
            SqlConnection LSqlConnection = null;
            OracleConnection LOracleConnection = null;

            try
            {
                DatabaseOperation01Return LDatabaseOperation01Return = new DatabaseOperation01Return();
                DataOperations01 LDataOperations01 = new DataOperations01();

                LStrVerificationCode002 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M002);
                LStrVerificationCode102 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M102);

                if (AListStringArgs[4] == "D")
                {
                    #region 删除座席信息
                    LStrDynamicSQL = "SELECT * FROM T_11_101_" + AListStringArgs[2] + " WHERE C001 = " + AListStringArgs[5] + " AND C002 = 2 AND C013 <> '0'";
                    LDatabaseOperation01Return = LDataOperations01.SelectDataByDynamicSQL(int.Parse(AListStringArgs[0]), AListStringArgs[1], LStrDynamicSQL);
                    DataSet LDataSetExistUsed = LDatabaseOperation01Return.DataSetReturn;
                    if (LDataSetExistUsed.Tables[0].Rows.Count > 0)
                    {
                        LOperationReturn.BoolReturn = false;
                        LOperationReturn.StringReturn = "W000E02";          //已经存在录音记录，不能被删除
                        return LOperationReturn;
                    }
                    LStrDynamicSQL = "DELETE FROM T_11_101_" + AListStringArgs[2] + " WHERE C001 = " + AListStringArgs[5];
                    LDatabaseOperation01Return = LDataOperations01.ExecuteDynamicSQL(int.Parse(AListStringArgs[0]), AListStringArgs[1], LStrDynamicSQL);
                    LOperationReturn.StringReturn = LDatabaseOperation01Return.StrReturn;
                    if (!LDatabaseOperation01Return.BoolReturn) { LOperationReturn.BoolReturn = false; }
                    LStrDynamicSQL = "DELETE FROM T_11_201_" + AListStringArgs[2] + " WHERE C003 = " + AListStringArgs[5] + " OR C004 = " + AListStringArgs[5];
                    LDataOperations01.ExecuteDynamicSQL(int.Parse(AListStringArgs[0]), AListStringArgs[1], LStrDynamicSQL);
                    LOperationReturn.StringReturn = LStrDataID;
                    #endregion
                }
                else
                {
                    LStrDataID = AListStringArgs[5];

                    #region 如果是增加座席，判断座席是否已经存在
                    if (AListStringArgs[4] == "A")
                    {
                        LStrTemp = EncryptionAndDecryption.EncryptDecryptString(AListStringArgs[6], LStrVerificationCode002, EncryptionAndDecryption.UMPKeyAndIVType.M002);
                        LStrDynamicSQL = "SELECT * FROM T_11_101_" + AListStringArgs[2] + " WHERE C001 >= 1030000000000000001 AND C001 < 1040000000000000000 AND C002 = 1 AND C017 = '" + LStrTemp + "'";
                        LDatabaseOperation01Return = LDataOperations01.SelectDataByDynamicSQL(int.Parse(AListStringArgs[0]), AListStringArgs[1], LStrDynamicSQL);
                        DataSet LDataSetExistData = LDatabaseOperation01Return.DataSetReturn;
                        if (LDataSetExistData.Tables[0].Rows.Count > 0)
                        {
                            LOperationReturn.BoolReturn = false;
                            LOperationReturn.StringReturn = "W000E03";          //座席已经存在，不能增加
                            LOperationReturn.ListStringReturn.Add(LDataSetExistData.Tables[0].Rows[0]["C001"].ToString());
                            return LOperationReturn;
                        }
                    }
                    #endregion

                    //新增座席从系统中获取新的流水号
                    if (AListStringArgs[4] == "A") { LStrDataID = GetSerialIDByType(AListStringArgs, 11, 103).ToString(); }

                    #region 读取座席基本信息
                    LStrDynamicSQL = "SELECT * FROM T_11_101_" + AListStringArgs[2] + " WHERE C001 = " + LStrDataID + " ORDER BY C002 ASC";
                    LDatabaseOperation01Return = LDataOperations01.SelectDataByDynamicSQL(int.Parse(AListStringArgs[0]), AListStringArgs[1], LStrDynamicSQL);
                    DataSet LDataSetSave2DBBI = LDatabaseOperation01Return.DataSetReturn;
                    LDataSetSave2DBBI.Tables[0].TableName = "T_11_101_" + AListStringArgs[2];
                    #endregion

                    #region 获取用户管理的用户
                    LStrDynamicSQL = "SELECT * FROM T_11_201_" + AListStringArgs[2] + " WHERE C003 = " + AListStringArgs[3] + " AND C004 > 1020000000000000000 AND C004 < 1030000000000000000";
                    LDatabaseOperation01Return = LDataOperations01.SelectDataByDynamicSQL(int.Parse(AListStringArgs[0]), AListStringArgs[1], LStrDynamicSQL);
                    DataSet LDataSetMyControlUser = LDatabaseOperation01Return.DataSetReturn;
                    #endregion

                    #region 获取管理我的用户
                    LStrDynamicSQL = "SELECT * FROM T_11_201_" + AListStringArgs[2] + " WHERE C003 > 1020000000000000000 AND C003 < 1030000000000000000 AND C004 = " + AListStringArgs[3];
                    LDatabaseOperation01Return = LDataOperations01.SelectDataByDynamicSQL(int.Parse(AListStringArgs[0]), AListStringArgs[1], LStrDynamicSQL);
                    DataSet LDataSetControlMyUser = LDatabaseOperation01Return.DataSetReturn;
                    #endregion

                    #region 获取用户管理的座席
                    LStrDynamicSQL = "SELECT * FROM T_11_201_" + AListStringArgs[2] + " WHERE C003 > 1020000000000000000 AND C003 < 1030000000000000000 AND C004 = " + LStrDataID;
                    LDatabaseOperation01Return = LDataOperations01.SelectDataByDynamicSQL(int.Parse(AListStringArgs[0]), AListStringArgs[1], LStrDynamicSQL);
                    DataSet LDataSetSave2DBUA = LDatabaseOperation01Return.DataSetReturn;
                    LDataSetSave2DBUA.Tables[0].TableName = "T_11_201_" + AListStringArgs[2];
                    #endregion

                    #region 获取技能组包含的座席
                    LStrDynamicSQL = "SELECT * FROM T_11_201_" + AListStringArgs[2] + " WHERE C003 > 9060000000000000000 AND C003 < 9070000000000000000 AND C004 = " + LStrDataID;
                    LDatabaseOperation01Return = LDataOperations01.SelectDataByDynamicSQL(int.Parse(AListStringArgs[0]), AListStringArgs[1], LStrDynamicSQL);
                    DataSet LDataSetSave2DBSA = LDatabaseOperation01Return.DataSetReturn;
                    LDataSetSave2DBSA.Tables[0].TableName = "T_11_201_" + AListStringArgs[2];
                    #endregion

                    #region 如果是新增座席，初始化2行数据
                    if (AListStringArgs[4] == "A")
                    {
                        #region 获取新用户默认密码
                        LStrDynamicSQL = "SELECT C006 FROM T_11_001_" + AListStringArgs[2] + " WHERE C003 = 11010501";
                        LDatabaseOperation01Return = LDataOperations01.SelectDataByDynamicSQL(int.Parse(AListStringArgs[0]), AListStringArgs[1], LStrDynamicSQL);
                        DataSet LDataSet11010501 = LDatabaseOperation01Return.DataSetReturn;
                        LStrUserDefaultPwd = LDataSet11010501.Tables[0].Rows[0][0].ToString();
                        LStrUserDefaultPwd = EncryptionAndDecryption.EncryptDecryptString(LStrUserDefaultPwd, LStrVerificationCode102, EncryptionAndDecryption.UMPKeyAndIVType.M102);
                        LStrUserDefaultPwd = LStrUserDefaultPwd.Substring(8);
                        LStrUserDefaultPwd = EncryptionAndDecryption.EncryptStringSHA512(LStrDataID + LStrUserDefaultPwd, LStrVerificationCode002, EncryptionAndDecryption.UMPKeyAndIVType.M002);
                        #endregion

                        for (int LIntAddRow = 1; LIntAddRow <= 2; LIntAddRow++)
                        {
                            DataRow LDataRowNewData = LDataSetSave2DBBI.Tables[0].NewRow();
                            LDataRowNewData.BeginEdit();
                            LDataRowNewData["C001"] = long.Parse(LStrDataID);
                            LDataRowNewData["C002"] = LIntAddRow;
                            if (LIntAddRow == 1)
                            {
                                LDataRowNewData["C013"] = "1";
                                LDataRowNewData["C014"] = "0";
                                LDataRowNewData["C016"] = "21";
                                LDataRowNewData["C020"] = LStrUserDefaultPwd;
                            }
                            if (LIntAddRow == 2)
                            {
                                LDataRowNewData["C011"] = "2014/01/01 00:00:00";
                                LDataRowNewData["C012"] = "0";
                                LDataRowNewData["C013"] = "0";
                            }
                            LDataRowNewData.EndEdit();
                            LDataSetSave2DBBI.Tables[0].Rows.Add(LDataRowNewData);
                        }
                    }
                    #endregion

                    int LIntAllSaveItem = AListStringArgs.Count;

                    #region 更新座席基本信息
                    int LIntPropertyID = 0, LIntPropertyRow = 0;
                    string LStrColumnName = string.Empty;
                    for (int LIntLoopSaveItem = 7; LIntLoopSaveItem < LIntAllSaveItem; LIntLoopSaveItem++)
                    {
                        string[] LStrDataSpliter = AListStringArgs[LIntLoopSaveItem].Split(AscCodeToChr(27).ToCharArray());
                        if (LStrDataSpliter[0].Substring(0, 1) != "B") { continue; }
                        LIntPropertyID = int.Parse(LStrDataSpliter[0].Substring(1));
                        if (LIntPropertyID > 0 && LIntPropertyID <= 10) { LIntPropertyRow = 0; LStrColumnName = "C" + (LIntPropertyID + 10).ToString("000"); }
                        if (LIntPropertyID > 10 && LIntPropertyID <= 20) { LIntPropertyRow = 1; LStrColumnName = "C" + LIntPropertyID.ToString("000"); }
                        if (LIntPropertyID == 7 || LIntPropertyID == 8)
                        {
                            LStrTemp = EncryptionAndDecryption.EncryptDecryptString(LStrDataSpliter[1], LStrVerificationCode002, EncryptionAndDecryption.UMPKeyAndIVType.M002);
                            LDataSetSave2DBBI.Tables[0].Rows[LIntPropertyRow][LStrColumnName] = LStrTemp;
                        }
                        else
                        {
                            LDataSetSave2DBBI.Tables[0].Rows[LIntPropertyRow][LStrColumnName] = LStrDataSpliter[1];
                        }
                    }
                    #endregion

                    #region 更新管理人员信息
                    int LIntAllOldManager = 0;
                    string LStrManager = string.Empty;
                    LIntAllOldManager = LDataSetSave2DBUA.Tables[0].Rows.Count;
                    for (int LIntLoopOldManager = LIntAllOldManager - 1; LIntLoopOldManager >= 0; LIntLoopOldManager--)
                    {
                        LStrManager = LDataSetSave2DBUA.Tables[0].Rows[LIntLoopOldManager]["C003"].ToString();
                        DataRow[] LDataRowIsMyCtrl = LDataSetMyControlUser.Tables[0].Select("C004 = " + LStrManager);
                        if (LDataRowIsMyCtrl.Length > 0) { LDataSetSave2DBUA.Tables[0].Rows[LIntLoopOldManager].Delete(); }
                    }
                    for (int LIntLoopSaveItem = 7; LIntLoopSaveItem < LIntAllSaveItem; LIntLoopSaveItem++)
                    {
                        string[] LStrDataSpliter = AListStringArgs[LIntLoopSaveItem].Split(AscCodeToChr(27).ToCharArray());
                        if (LStrDataSpliter[0].Substring(0, 1) != "U") { continue; }
                        DataRow LDataRowNew = LDataSetSave2DBUA.Tables[0].NewRow();
                        LDataRowNew.BeginEdit();
                        LDataRowNew["C001"] = 0;
                        LDataRowNew["C002"] = 0;
                        LDataRowNew["C003"] = long.Parse(LStrDataSpliter[1]);
                        LDataRowNew["C004"] = long.Parse(LStrDataID);
                        LDataRowNew["C005"] = DateTime.Parse("2014/01/01 00:00:00");
                        LDataRowNew["C006"] = DateTime.Parse("2199/12/31 23:59:59");
                        LDataRowNew.EndEdit();
                        LDataSetSave2DBUA.Tables[0].Rows.Add(LDataRowNew);
                    }
                    foreach (DataRow LDataRowSingleControlMy in LDataSetControlMyUser.Tables[0].Rows)
                    {
                        LStrManager = LDataRowSingleControlMy["C003"].ToString();
                        DataRow[] LDataRowExistCtrlAgent = LDataSetSave2DBUA.Tables[0].Select("C003 = " + LStrManager);
                        if (LDataRowExistCtrlAgent.Length > 0) { continue; }
                        DataRow LDataRowNew = LDataSetSave2DBUA.Tables[0].NewRow();
                        LDataRowNew.BeginEdit();
                        LDataRowNew["C001"] = 0;
                        LDataRowNew["C002"] = 0;
                        LDataRowNew["C003"] = LStrManager;
                        LDataRowNew["C004"] = long.Parse(LStrDataID);
                        LDataRowNew["C005"] = DateTime.Parse("2014/01/01 00:00:00");
                        LDataRowNew["C006"] = DateTime.Parse("2199/12/31 23:59:59");
                        LDataRowNew.EndEdit();
                        LDataSetSave2DBUA.Tables[0].Rows.Add(LDataRowNew);
                    }
                    #endregion

                    #region 更新座席所属技能组信息
                    int LIntAllOldSkillGroup = 0;
                    LIntAllOldSkillGroup = LDataSetSave2DBSA.Tables[0].Rows.Count;
                    for (int LIntLoopSkillGroup = LIntAllOldSkillGroup - 1; LIntLoopSkillGroup >= 0; LIntLoopSkillGroup--)
                    {
                        LDataSetSave2DBSA.Tables[0].Rows[LIntLoopSkillGroup].Delete();
                    }
                    for (int LIntLoopSaveItem = 7; LIntLoopSaveItem < LIntAllSaveItem; LIntLoopSaveItem++)
                    {
                        string[] LStrDataSpliter = AListStringArgs[LIntLoopSaveItem].Split(AscCodeToChr(27).ToCharArray());
                        if (LStrDataSpliter[0].Substring(0, 1) != "S") { continue; }
                        DataRow LDataRowNew = LDataSetSave2DBSA.Tables[0].NewRow();
                        LDataRowNew.BeginEdit();
                        LDataRowNew["C001"] = 0;
                        LDataRowNew["C002"] = 0;
                        LDataRowNew["C003"] = long.Parse(LStrDataSpliter[1]);
                        LDataRowNew["C004"] = long.Parse(LStrDataID);
                        LDataRowNew["C005"] = DateTime.Parse("2014/01/01 00:00:00");
                        LDataRowNew["C006"] = DateTime.Parse("2199/12/31 23:59:59");
                        LDataRowNew.EndEdit();
                        LDataSetSave2DBSA.Tables[0].Rows.Add(LDataRowNew);
                    }
                    #endregion

                    #region 将数据保存到MSSQL数据库
                    if (AListStringArgs[0] == "2")
                    {
                        LSqlConnection = new SqlConnection(AListStringArgs[1]);

                        LStrDynamicSQL = "SELECT * FROM T_11_101_" + AListStringArgs[2] + " WHERE C001 = " + LStrDataID + " ORDER BY C002 ASC";
                        SqlDataAdapter LSqlDataAdapter1 = new SqlDataAdapter(LStrDynamicSQL, LSqlConnection);
                        SqlCommandBuilder LSqlCommandBuilder1 = new SqlCommandBuilder();

                        LSqlCommandBuilder1.ConflictOption = ConflictOption.OverwriteChanges;
                        LSqlCommandBuilder1.SetAllValues = false;
                        LSqlCommandBuilder1.DataAdapter = LSqlDataAdapter1;
                        LSqlDataAdapter1.Update(LDataSetSave2DBBI, "T_11_101_" + AListStringArgs[2]);
                        LDataSetSave2DBBI.AcceptChanges();
                        LSqlCommandBuilder1.Dispose();
                        LSqlDataAdapter1.Dispose();

                        LStrDynamicSQL = "SELECT * FROM T_11_201_" + AListStringArgs[2] + " WHERE C003 > 1020000000000000000 AND C003 < 1030000000000000000 AND C004 = " + LStrDataID;
                        SqlDataAdapter LSqlDataAdapter2 = new SqlDataAdapter(LStrDynamicSQL, LSqlConnection);
                        SqlCommandBuilder LSqlCommandBuilder2 = new SqlCommandBuilder();

                        LSqlCommandBuilder2.ConflictOption = ConflictOption.OverwriteChanges;
                        LSqlCommandBuilder2.SetAllValues = false;
                        LSqlCommandBuilder2.DataAdapter = LSqlDataAdapter2;
                        LSqlDataAdapter2.Update(LDataSetSave2DBUA, "T_11_201_" + AListStringArgs[2]);
                        LDataSetSave2DBUA.AcceptChanges();
                        LSqlCommandBuilder2.Dispose();
                        LSqlDataAdapter2.Dispose();

                        LStrDynamicSQL = "SELECT * FROM T_11_201_" + AListStringArgs[2] + " WHERE C003 > 9060000000000000000 AND C003 < 9070000000000000000 AND C004 = " + LStrDataID;
                        SqlDataAdapter LSqlDataAdapter3 = new SqlDataAdapter(LStrDynamicSQL, LSqlConnection);
                        SqlCommandBuilder LSqlCommandBuilder3 = new SqlCommandBuilder();

                        LSqlCommandBuilder3.ConflictOption = ConflictOption.OverwriteChanges;
                        LSqlCommandBuilder3.SetAllValues = false;
                        LSqlCommandBuilder3.DataAdapter = LSqlDataAdapter3;
                        LSqlDataAdapter3.Update(LDataSetSave2DBSA, "T_11_201_" + AListStringArgs[2]);
                        LDataSetSave2DBSA.AcceptChanges();
                        LSqlCommandBuilder3.Dispose();
                        LSqlDataAdapter3.Dispose();
                    }
                    #endregion

                    #region 将数据保存到Oracle数据库
                    if (AListStringArgs[0] == "3")
                    {
                        LOracleConnection = new OracleConnection(AListStringArgs[1]);

                        LStrDynamicSQL = "SELECT * FROM T_11_101_" + AListStringArgs[2] + " WHERE C001 = " + LStrDataID + " ORDER BY C002 ASC";
                        OracleDataAdapter LOracleDataAdapter1 = new OracleDataAdapter(LStrDynamicSQL, LOracleConnection);
                        OracleCommandBuilder LOracleCommandBuilder1 = new OracleCommandBuilder();

                        LOracleCommandBuilder1.ConflictOption = ConflictOption.OverwriteChanges;
                        LOracleCommandBuilder1.SetAllValues = false;
                        LOracleCommandBuilder1.DataAdapter = LOracleDataAdapter1;
                        LOracleDataAdapter1.Update(LDataSetSave2DBBI, "T_11_101_" + AListStringArgs[2]);
                        LDataSetSave2DBBI.AcceptChanges();
                        LOracleCommandBuilder1.Dispose();
                        LOracleDataAdapter1.Dispose();

                        LStrDynamicSQL = "SELECT * FROM T_11_201_" + AListStringArgs[2] + " WHERE C003 > 1020000000000000000 AND C003 < 1030000000000000000 AND C004 = " + LStrDataID;
                        OracleDataAdapter LOracleDataAdapter2 = new OracleDataAdapter(LStrDynamicSQL, LOracleConnection);
                        OracleCommandBuilder LOracleCommandBuilder2 = new OracleCommandBuilder();

                        LOracleCommandBuilder2.ConflictOption = ConflictOption.OverwriteChanges;
                        LOracleCommandBuilder2.SetAllValues = false;
                        LOracleCommandBuilder2.DataAdapter = LOracleDataAdapter2;
                        LOracleDataAdapter2.Update(LDataSetSave2DBUA, "T_11_201_" + AListStringArgs[2]);
                        LDataSetSave2DBUA.AcceptChanges();
                        LOracleCommandBuilder2.Dispose();
                        LOracleDataAdapter2.Dispose();

                        LStrDynamicSQL = "SELECT * FROM T_11_201_" + AListStringArgs[2] + " WHERE C003 > 9060000000000000000 AND C003 < 9070000000000000000 AND C004 = " + LStrDataID;
                        OracleDataAdapter LOracleDataAdapter3 = new OracleDataAdapter(LStrDynamicSQL, LOracleConnection);
                        OracleCommandBuilder LOracleCommandBuilder3 = new OracleCommandBuilder();

                        LOracleCommandBuilder3.ConflictOption = ConflictOption.OverwriteChanges;
                        LOracleCommandBuilder3.SetAllValues = false;
                        LOracleCommandBuilder3.DataAdapter = LOracleDataAdapter3;
                        LOracleDataAdapter3.Update(LDataSetSave2DBSA, "T_11_201_" + AListStringArgs[2]);
                        LDataSetSave2DBSA.AcceptChanges();
                        LOracleCommandBuilder3.Dispose();
                        LOracleDataAdapter3.Dispose();
                    }
                    #endregion

                    LOperationReturn.StringReturn = LStrDataID;
                }
            }
            catch (Exception ex)
            {
                LOperationReturn.BoolReturn = false;
                LOperationReturn.StringReturn = ex.ToString();
            }
            finally
            {
                if (LSqlConnection != null)
                {
                    if (LSqlConnection.State == System.Data.ConnectionState.Open) { LSqlConnection.Close(); }
                    LSqlConnection.Dispose();
                }
                if (LOracleConnection != null)
                {
                    if (LOracleConnection.State == ConnectionState.Open) { LOracleConnection.Close(); }
                    LOracleConnection.Dispose(); LOracleConnection = null;
                }
            }

            return LOperationReturn;
        }
        #endregion

        #region 处理IQueueField998Data队列中的数据
        private void OperationEncryptData()
        {
            while (IBoolCanContinue)
            {
                if (IQueueField998Data.Count > 0)
                {
                    lock (ILockObject998)
                    {
                        #region
                        try
                        {
                            string totalC001 = "";
                            int totalcount = 0;
                            if (IQueueField998Data.Count > 50)//大于50条，每次处理50条到998表
                            {
                                totalcount = 50;
                                for (int i = 0; i < 50; i++)
                                {
                                    totalC001 += IQueueField998Data.Peek() + ",";
                                    IQueueField998Data.Dequeue();
                                }
                                totalC001.TrimEnd(',');
                            }
                            else//否则全部
                            {
                                totalcount = IQueueField998Data.Count;
                                foreach (string item in IQueueField998Data)
                                {
                                    totalC001 += item + ",";
                                }
                                IQueueField998Data.Clear();
                                totalC001.TrimEnd(',');
                            }
                            if (totalC001 != "")
                            {
                                string strsql = string.Format("INSERT INTO T_21_998(C001,C002,C003,C004,C005,C006,C007,C008,C009,C010,C011,C012,C013,C014,C015,C016,C019,C020) SELECT C001,C002,C005,C006,C022,C020,C037,C038,C035,C036,C077,C042,C058,C039,C040,C041,C025,C110 FROM T_21_001 WHERE C001 IN ({0})"
                                            , totalC001);
                                DataOperations01 LDataOperations = new DataOperations01();
                                LDataOperations.ExecuteDynamicSQL(IIntDatabaseType, IStrDatabaseProfile, strsql);
                            }
                        }
                        catch (Exception ex)
                        {
                            WriteEntryLog("OperationEncryptData()\n" + ex.ToString(), EventLogEntryType.Error);
                        }
                        #endregion                        
                    }
                }
                Thread.Sleep(2000);
            }
        }
        #endregion

        #region 清除UMP临时数据(登录历史记录、查询用临时数据、MediaData)
        private void ClearTemporaryData()
        {
            DateTime LDateTimeLastReadArgs = DateTime.Now.AddMinutes(-6);
            DatabaseOperation01Return LDatabaseReturn01 = new DatabaseOperation01Return();
            DatabaseOperation01Return LDatabaseReturn02 = new DatabaseOperation01Return();
            DatabaseOperation01Return LDatabaseReturn03 = new DatabaseOperation01Return();
            DataOperations01 LDataOperations = new DataOperations01();
            string LStrSelectSQL = string.Empty;
            string LStrDeleteSQL = string.Empty;
            string LStrVerificationCode102 = string.Empty;
            string LStrTableName = string.Empty;
            string LStrDateTimeDB = string.Empty;
            string LStrDateTime = string.Empty;
            string LStrUserID = string.Empty;
            string LStrStep = string.Empty;
            DateTime LDateTimeTemp;

            try
            {
                LStrVerificationCode102 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M102);

                while (IBoolCanContinue)
                {
                    if (IIntDatabaseType == 0 || string.IsNullOrEmpty(IStrDatabaseProfile)) { IBooInClear = false; Thread.Sleep(2000); continue; }

                    IBooInClear = true;

                    #region 清理超时登录流水记录
                    LStrStep = "T_11_002";
                    try
                    {
                        LStrSelectSQL = "SELECT C021 FROM T_00_121";
                        LDatabaseReturn01 = LDataOperations.SelectDataByDynamicSQL(IIntDatabaseType, IStrDatabaseProfile, LStrSelectSQL);
                        foreach (DataRow LDataRowSingleRent in LDatabaseReturn01.DataSetReturn.Tables[0].Rows)
                        {
                            LStrTableName = LDataRowSingleRent[0].ToString();
                            LStrTableName = EncryptionAndDecryption.EncryptDecryptString(LStrTableName, LStrVerificationCode102, EncryptionAndDecryption.UMPKeyAndIVType.M102);
                            LStrTableName = "T_11_002_" + LStrTableName;
                            LStrSelectSQL = "SELECT C001, C002 FROM " + LStrTableName;
                            LDatabaseReturn02 = LDataOperations.SelectDataByDynamicSQL(IIntDatabaseType, IStrDatabaseProfile, LStrSelectSQL);
                            if (!LDatabaseReturn02.BoolReturn) { continue; }
                            if (LDatabaseReturn02.StrReturn == "0" || LDatabaseReturn02.DataSetReturn.Tables.Count == 0) { continue; }
                            foreach (DataRow LDataRowSingleLoginInfo in LDatabaseReturn02.DataSetReturn.Tables[0].Rows)
                            {
                                LStrUserID = LDataRowSingleLoginInfo[0].ToString();
                                LStrDateTimeDB = LDataRowSingleLoginInfo[1].ToString();
                                LStrDateTime = EncryptionAndDecryption.EncryptDecryptString(LStrDateTimeDB, LStrVerificationCode102, EncryptionAndDecryption.UMPKeyAndIVType.M102);
                                if (!DateTime.TryParse(LStrDateTime, out LDateTimeTemp)) { continue; }
                                if (DateTime.Parse(LStrDateTime).AddHours(16) > DateTime.UtcNow) { continue; }
                                LStrDeleteSQL = "DELETE FROM " + LStrTableName + " WHERE C001 = " + LStrUserID + " AND C002 = '" + LStrDateTimeDB + "'";
                                LDatabaseReturn03 = LDataOperations.ExecuteDynamicSQL(IIntDatabaseType, IStrDatabaseProfile, LStrDeleteSQL);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        WriteEntryLog("ClearTemporaryData().T_11_002.Data\nTable Name = " + LStrTableName + "\n" + ex.ToString(), EventLogEntryType.Error);
                    }
                    #endregion

                    #region 清理 T_00_901
                    LStrStep = "T_00_901";
                    try
                    {
                        LStrDateTime = "911" + DateTime.UtcNow.AddDays(-1).ToString("yyMMddHH") + "99999999";
                        LStrDeleteSQL = "DELETE FROM T_00_901 WHERE C001 < " + LStrDateTime;
                        LDatabaseReturn03 = LDataOperations.ExecuteDynamicSQL(IIntDatabaseType, IStrDatabaseProfile, LStrDeleteSQL);
                    }
                    catch (Exception ex)
                    {
                        WriteEntryLog("ClearTemporaryData().T_00_901.Data\n" + ex.ToString(), EventLogEntryType.Error);
                    }
                    #endregion

                    #region 清理MediaData目录下的文件
                    LStrStep = "MediaData";
                    try
                    {
                        string LStrMediaDataFolder = System.IO.Path.Combine(IStrSiteBaseDirectory, "MediaData");
                        DateTime LDateTimeBefore2Days = DateTime.Now.AddDays(-1);
                        DirectoryInfo LDirectoryInfoMediaData = new DirectoryInfo(LStrMediaDataFolder);
                        FileInfo[] LFileInfoTemporary = LDirectoryInfoMediaData.GetFiles();
                        foreach (FileInfo LFileInfoSingle in LFileInfoTemporary)
                        {
                            if (LFileInfoSingle.CreationTime < LDateTimeBefore2Days)
                            {
                                File.Delete(LFileInfoSingle.FullName);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        WriteEntryLog("ClearTemporaryData().MediaData.Files\n" + ex.ToString(), EventLogEntryType.Error);
                    }
                    #endregion

                    IBooInClear = false;
                    IBoolCanAbortClear = true;
                    System.Threading.Thread.Sleep(500);
                }
                IBoolCanAbortClear = true;
            }
            catch (Exception ex)
            {
                IBooInClear = false;
                IBoolCanAbortClear = true;
                WriteEntryLog("ClearTemporaryData()\n" + ex.ToString(), EventLogEntryType.Error);
            }
        }
        #endregion

        private void LogicPartitionDataMain()
        {
            DateTime LDateTimeLastReadArgs = DateTime.Now.AddMinutes(-6);
            int LIntDelaySeconds = 0;
            List<string> LListStrRentExistObjects = new List<string>();

            try
            {
                while (IBoolCanContinue)
                {
                    if (IIntDatabaseType == 0 || string.IsNullOrEmpty(IStrDatabaseProfile)) { IBooInMove21 = false; Thread.Sleep(2000); continue; }

                    IBoolInDoing = true;

                    //获取所有需要转移的数据表
                    DataTable LDataTableMovedTables = ObtainMoveDataTables();
                    if (LDataTableMovedTables == null) { IBoolInDoing = false; Thread.Sleep(5000); continue; }

                    //设置延时
                    LIntDelaySeconds = int.Parse(LDataTableMovedTables.Rows[0]["P03"].ToString());

                    //获取所有租户列表
                    LListStrRentExistObjects.Clear();
                    DataTable LDataTableRentList = ObtainRentList(ref LListStrRentExistObjects);
                    if (LDataTableRentList == null) { IBoolInDoing = false; Thread.Sleep(5000); continue; }

                    //将主表数据转移到租户表中
                    MoveDataFromMain2Rent(LDataTableMovedTables, LDataTableRentList, ref LListStrRentExistObjects);

                    //将每个租户的数据转移到逻辑分区表中
                    MoveRentData2LogicPartitionTable(LDataTableRentList, LDataTableMovedTables);
                    //WriteEntryLog("Copy the data end, waiting for the next.", EventLogEntryType.Information);
                    Thread.Sleep(LIntDelaySeconds * 1000);
                    IBoolInDoing = false;
                    IBoolCanAbort = true;
                }
                IBoolCanAbort = true;
            }
            catch (Exception ex)
            {
                WriteEntryLog("LogicPartitionDataMain()\n"+ ex.ToString(), EventLogEntryType.Error);
            }
        }

        #region 将主表数据转移到租户表中
        private void MoveDataFromMain2Rent(DataTable ADataTableLogicTalbes, DataTable ADataTableRentList, ref List<string> AListStrExistObjects)
        {
            string LStrTableName = string.Empty;
            string LStrSerialColumn = string.Empty;
            string LStrTimeColumn = string.Empty;
            string LStrSelectRows = string.Empty;
            string LStrBeforeSeconds = string.Empty;
            int LIntBeforeSeconds = 0;
            string LStrDynamicSQL = string.Empty;
            string LStrRentToken = string.Empty;
            List<string> LListStrDynamicSQL = new List<string>();
            List<string> LListStrSQLMethod = new List<string>();
            List<string> LListStrSQLKey = new List<string>();
            List<string> LListStrSourceTable = new List<string>();
            DataTable LDtARData = new DataTable();

            string LStrSingleObject = string.Empty;

            string LStrAgentID = string.Empty;
            string LStr11901006 = string.Empty, LStr11901007 = string.Empty;
            string LStrTimeData = string.Empty;

            try
            {
                //WriteEntryLog("MoveDataFromMain2Rent() Begin", EventLogEntryType.Information);

                DatabaseOperation01Return LDatabaseOperationReturn = new DatabaseOperation01Return();
                DataOperations01 LDataOperations = new DataOperations01();

                foreach (DataRow LDataRowSingleTable in ADataTableLogicTalbes.Rows)
                {
                    DateTime LDateTimeUTCNow = DateTime.UtcNow;

                    LStrTableName = LDataRowSingleTable["TableName"].ToString();
                    if (!LStrTableName.Contains("T_")) { continue; }
                    LStrSerialColumn = LDataRowSingleTable["P05"].ToString();
                    LStrTimeColumn = LDataRowSingleTable["P04"].ToString();
                    LStrSelectRows = LDataRowSingleTable["P01"].ToString();
                    LStrBeforeSeconds = LDataRowSingleTable["P02"].ToString();
                    LIntBeforeSeconds = int.Parse(LStrBeforeSeconds) * (-1);
                    LDateTimeUTCNow = LDateTimeUTCNow.AddSeconds(LIntBeforeSeconds);
                    LStrDynamicSQL = string.Empty;

                    List<string> AListRecordID = new List<string>();
                    #region 按 ASC 的方式处理 设置的数据
                    LStrDynamicSQL = string.Empty;
                    if (IIntDatabaseType == 2)
                    {
                        if (LStrTableName == "T_21_001")
                            LStrDynamicSQL = "SELECT TOP " + LStrSelectRows + " * FROM " + LStrTableName + " WHERE " + LStrSerialColumn + " > 0 AND C025 NOT IN('E','F') ORDER BY " + LStrSerialColumn + " ASC";
                        else
                            LStrDynamicSQL = "SELECT TOP " + LStrSelectRows + " * FROM " + LStrTableName + " WHERE " + LStrSerialColumn + " > 0 ORDER BY " + LStrSerialColumn + " ASC";
                    }
                    if (IIntDatabaseType == 3)
                    {
                        if (LStrTableName == "T_21_001")
                            LStrDynamicSQL = "SELECT * FROM " + LStrTableName + " WHERE " + LStrSerialColumn + " > 0 AND ROWNUM <= " + LStrSelectRows + " AND C025 NOT IN('E','F') ORDER BY " + LStrSerialColumn + " ASC";
                        else
                            LStrDynamicSQL = "SELECT * FROM " + LStrTableName + " WHERE " + LStrSerialColumn + " > 0 AND ROWNUM <= " + LStrSelectRows + " ORDER BY " + LStrSerialColumn + " ASC";
                    }

                    if (string.IsNullOrEmpty(LStrDynamicSQL)) { continue; }

                    LDatabaseOperationReturn = LDataOperations.SelectDataByDynamicSQL(IIntDatabaseType, IStrDatabaseProfile, LStrDynamicSQL);
                    if (!LDatabaseOperationReturn.BoolReturn)
                    {
                        WriteEntryLog("MoveDataFromMain2Rent()\n" + LDatabaseOperationReturn.StrReturn + "\n" + LStrDynamicSQL, EventLogEntryType.Error);
                        continue;
                    }

                    LListStrDynamicSQL.Clear(); LListStrSQLMethod.Clear(); LListStrSQLKey.Clear(); LListStrSourceTable.Clear();
                    foreach (DataRow LDataRowSingleData in LDatabaseOperationReturn.DataSetReturn.Tables[0].Rows)
                    {
                        DateTime LDateTimeInData = DateTime.UtcNow;

                        LStrTimeData = LDataRowSingleData[LStrTimeColumn].ToString();
                        if (Regex.IsMatch(LStrTimeData, @"^\d{14}$"))
                        {
                            LDateTimeInData = DateTime.Parse(LStrTimeData.Substring(0, 4) + "/" + LStrTimeData.Substring(4, 2) + "/" + LStrTimeData.Substring(6, 2) + " " + LStrTimeData.Substring(8, 2) + ":" + LStrTimeData.Substring(10, 2) + ":" + LStrTimeData.Substring(12, 2));
                        }
                        else
                        {
                            LDateTimeInData = DateTime.Parse(LStrTimeData);
                        }
                        if (LDateTimeInData > LDateTimeUTCNow) { continue; }
                        LStrRentToken = GetRentTokenByDataRow(LDataRowSingleData, LStrTableName, ADataTableRentList);
                        if (string.IsNullOrEmpty(LStrRentToken))
                        {
                            LStrDynamicSQL = "UPDATE " + LStrTableName + "SET " + LStrSerialColumn + " = " + LStrSerialColumn + " * (-1) WHERE " + LStrSerialColumn + " = " + LDataRowSingleData[LStrSerialColumn].ToString();
                            LListStrDynamicSQL.Add(LStrDynamicSQL);
                            LListStrSQLMethod.Add("U");
                            LListStrSQLKey.Add(LDataRowSingleData[LStrSerialColumn].ToString());
                            LListStrSourceTable.Add(LStrTableName);
                            continue;
                        }
                        else
                        {
                            LStrDynamicSQL = "INSERT INTO " + LStrTableName + "_" + LStrRentToken + " SELECT * FROM " + LStrTableName + " WHERE " + LStrSerialColumn + " = " + LDataRowSingleData[LStrSerialColumn].ToString();
                            LListStrDynamicSQL.Add(LStrDynamicSQL);
                            LListStrSQLMethod.Add("I");
                            LListStrSQLKey.Add(LDataRowSingleData[LStrSerialColumn].ToString());
                            LListStrSourceTable.Add(LStrTableName);

                            LStrDynamicSQL = "DELETE FROM " + LStrTableName + " WHERE " + LStrSerialColumn + " = " + LDataRowSingleData[LStrSerialColumn].ToString();
                            LListStrDynamicSQL.Add(LStrDynamicSQL);
                            LListStrSQLMethod.Add("D");
                            LListStrSQLKey.Add(LDataRowSingleData[LStrSerialColumn].ToString());
                            LListStrSourceTable.Add(LStrTableName);
                        }

                        #region 处理座席信息
                        if (LStrTableName == "T_21_001")
                        {
                            LStrAgentID = LDataRowSingleData["C039"].ToString().Trim();
                            if (string.IsNullOrEmpty(LStrAgentID)) { continue; }
                            if (LStrAgentID == "N/A") { continue; }
                            LStrSingleObject = LStrRentToken + IStrSpliterChar + "103" + IStrSpliterChar + LStrAgentID;
                            if (AListStrExistObjects.Contains(LStrSingleObject)) { continue; }
                            AListStrExistObjects.Add(LStrSingleObject);
                            lock (ILockObject) { IYoungCircleQueue.PushElement(LStrSingleObject); }
                        }
                        #endregion

                        #region 操作日志的机器名、IP
                        if (LStrTableName == "T_11_901")
                        {
                            LStr11901006 = LDataRowSingleData["C006"].ToString().Trim();
                            LStr11901007 = LDataRowSingleData["C007"].ToString().Trim();
                            if (!string.IsNullOrEmpty(LStr11901006))
                            {
                                LStrSingleObject = LStrRentToken + IStrSpliterChar + "107" + IStrSpliterChar + LStr11901006;
                                if (!AListStrExistObjects.Contains(LStrSingleObject))
                                {
                                    AListStrExistObjects.Add(LStrSingleObject);
                                    lock (ILockObject) { IYoungCircleQueue.PushElement(LStrSingleObject); }
                                }
                            }
                            if (!string.IsNullOrEmpty(LStr11901007))
                            {
                                LStrSingleObject = LStrRentToken + IStrSpliterChar + "108" + IStrSpliterChar + LStr11901007;
                                if (!AListStrExistObjects.Contains(LStrSingleObject))
                                {
                                    AListStrExistObjects.Add(LStrSingleObject);
                                    lock (ILockObject) { IYoungCircleQueue.PushElement(LStrSingleObject); }
                                }
                            }
                        }
                        #endregion
                    }
                    ExecuteDynamicSQL(LListStrDynamicSQL, LListStrSQLMethod, LListStrSQLKey, LListStrSourceTable, ref AListRecordID);
                    #endregion
                    ARDataManage(LStrRentToken, AListRecordID, LDatabaseOperationReturn.DataSetReturn.Tables[0]);

                    StaticDataManage(LStrRentToken, AListRecordID, LDatabaseOperationReturn.DataSetReturn.Tables[0]);

                    #region 按 DESC 的方式处理 设置的数据
                    LStrDynamicSQL = string.Empty; AListRecordID.Clear();
                    if (IIntDatabaseType == 2)
                    {
                        if (LStrTableName == "T_21_001")
                            LStrDynamicSQL = "SELECT TOP " + LStrSelectRows + " * FROM " + LStrTableName + " WHERE " + LStrSerialColumn + " > 0 AND C025 NOT IN('E','F') ORDER BY " + LStrSerialColumn + " DESC";
                        else
                            LStrDynamicSQL = "SELECT TOP " + LStrSelectRows + " * FROM " + LStrTableName + " WHERE " + LStrSerialColumn + " > 0 ORDER BY " + LStrSerialColumn + " DESC";
                    }
                    if (IIntDatabaseType == 3)
                    {
                        if (LStrTableName == "T_21_001")
                            LStrDynamicSQL = "SELECT * FROM " + LStrTableName + " WHERE " + LStrSerialColumn + " > 0 AND ROWNUM <= " + LStrSelectRows + " AND C025 NOT IN('E','F') ORDER BY " + LStrSerialColumn + " DESC";
                        else
                            LStrDynamicSQL = "SELECT * FROM " + LStrTableName + " WHERE " + LStrSerialColumn + " > 0 AND ROWNUM <= " + LStrSelectRows + " ORDER BY " + LStrSerialColumn + " DESC";
                    }

                    if (string.IsNullOrEmpty(LStrDynamicSQL)) { continue; }

                    LDatabaseOperationReturn = LDataOperations.SelectDataByDynamicSQL(IIntDatabaseType, IStrDatabaseProfile, LStrDynamicSQL);
                    if (!LDatabaseOperationReturn.BoolReturn)
                    {
                        WriteEntryLog("MoveDataFromMain2Rent()\n" + LDatabaseOperationReturn.StrReturn, EventLogEntryType.Error);
                        continue;
                    }

                    LListStrDynamicSQL.Clear(); LListStrSQLMethod.Clear(); LListStrSQLKey.Clear(); LListStrSourceTable.Clear();
                    foreach (DataRow LDataRowSingleData in LDatabaseOperationReturn.DataSetReturn.Tables[0].Rows)
                    {
                        DateTime LDateTimeInData = DateTime.UtcNow;

                        LStrTimeData = LDataRowSingleData[LStrTimeColumn].ToString();
                        if (Regex.IsMatch(LStrTimeData, @"^\d{14}$"))
                        {
                            LDateTimeInData = DateTime.Parse(LStrTimeData.Substring(0, 4) + "/" + LStrTimeData.Substring(4, 2) + "/" + LStrTimeData.Substring(6, 2) + " " + LStrTimeData.Substring(8, 2) + ":" + LStrTimeData.Substring(10, 2) + ":" + LStrTimeData.Substring(12, 2));
                        }
                        else
                        {
                            LDateTimeInData = DateTime.Parse(LStrTimeData);
                        }
                        if (LDateTimeInData > LDateTimeUTCNow) { continue; }
                        LStrRentToken = GetRentTokenByDataRow(LDataRowSingleData, LStrTableName, ADataTableRentList);
                        if (string.IsNullOrEmpty(LStrRentToken))
                        {
                            LStrDynamicSQL = "UPDATE " + LStrTableName + "SET " + LStrSerialColumn + " = " + LStrSerialColumn + " * (-1) WHERE " + LStrSerialColumn + " = " + LDataRowSingleData[LStrSerialColumn].ToString();
                            LListStrDynamicSQL.Add(LStrDynamicSQL);
                            LListStrSQLMethod.Add("U");
                            LListStrSQLKey.Add(LDataRowSingleData[LStrSerialColumn].ToString());
                            LListStrSourceTable.Add(LStrTableName);
                            continue;
                        }
                        else
                        {
                            LStrDynamicSQL = "INSERT INTO " + LStrTableName + "_" + LStrRentToken + " SELECT * FROM " + LStrTableName + " WHERE " + LStrSerialColumn + " = " + LDataRowSingleData[LStrSerialColumn].ToString();
                            LListStrDynamicSQL.Add(LStrDynamicSQL);
                            LListStrSQLMethod.Add("I");
                            LListStrSQLKey.Add(LDataRowSingleData[LStrSerialColumn].ToString());
                            LListStrSourceTable.Add(LStrTableName);

                            LStrDynamicSQL = "DELETE FROM " + LStrTableName + " WHERE " + LStrSerialColumn + " = " + LDataRowSingleData[LStrSerialColumn].ToString();
                            LListStrDynamicSQL.Add(LStrDynamicSQL);
                            LListStrSQLMethod.Add("D");
                            LListStrSQLKey.Add(LDataRowSingleData[LStrSerialColumn].ToString());
                            LListStrSourceTable.Add(LStrTableName);
                        }

                        #region 处理座席信息
                        if (LStrTableName == "T_21_001")
                        {
                            LStrAgentID = LDataRowSingleData["C039"].ToString().Trim();
                            if (string.IsNullOrEmpty(LStrAgentID)) { continue; }
                            LStrSingleObject = LStrRentToken + IStrSpliterChar + "103" + IStrSpliterChar + LStrAgentID;
                            if (AListStrExistObjects.Contains(LStrSingleObject)) { continue; }
                            AListStrExistObjects.Add(LStrSingleObject);
                            lock (ILockObject) { IYoungCircleQueue.PushElement(LStrSingleObject); }
                        }
                        #endregion

                        #region 操作日志的机器名、IP
                        if (LStrTableName == "T_11_901")
                        {
                            LStr11901006 = LDataRowSingleData["C006"].ToString().Trim();
                            LStr11901007 = LDataRowSingleData["C007"].ToString().Trim();
                            if (!string.IsNullOrEmpty(LStr11901006))
                            {
                                LStrSingleObject = LStrRentToken + IStrSpliterChar + "107" + IStrSpliterChar + LStr11901006;
                                if (!AListStrExistObjects.Contains(LStrSingleObject))
                                {
                                    AListStrExistObjects.Add(LStrSingleObject);
                                    lock (ILockObject) { IYoungCircleQueue.PushElement(LStrSingleObject); }
                                }
                            }
                            if (!string.IsNullOrEmpty(LStr11901007))
                            {
                                LStrSingleObject = LStrRentToken + IStrSpliterChar + "108" + IStrSpliterChar + LStr11901007;
                                if (!AListStrExistObjects.Contains(LStrSingleObject))
                                {
                                    AListStrExistObjects.Add(LStrSingleObject);
                                    lock (ILockObject) { IYoungCircleQueue.PushElement(LStrSingleObject); }
                                }
                            }
                        }
                        #endregion
                    }
                    ExecuteDynamicSQL(LListStrDynamicSQL, LListStrSQLMethod, LListStrSQLKey, LListStrSourceTable, ref AListRecordID);
                    #endregion
                    ARDataManage(LStrRentToken, AListRecordID, LDatabaseOperationReturn.DataSetReturn.Tables[0]);

                    StaticDataManage(LStrRentToken, AListRecordID, LDatabaseOperationReturn.DataSetReturn.Tables[0]);
                }

                //WriteEntryLog("MoveDataFromMain2Rent() End", EventLogEntryType.Information);
            }
            catch (Exception ex)
            {
                WriteEntryLog("MoveDataFromMain2Rent()\n" + ex.ToString(), EventLogEntryType.Error);
            }
        }
        #endregion

        #region 将所有租户表中的数据库转移到逻辑分区表中
        private void MoveRentData2LogicPartitionTable(DataTable ADataTableRentList, DataTable ADataTableMovedTables)
        {
            string LStrRentToken = string.Empty;
            string LStrTableName = string.Empty;
            string LStrDepentColumn = string.Empty;
            string LStr00000001 = string.Empty;

            string LStrDynamicSQL = string.Empty;

            try
            {
                DatabaseOperation01Return LDatabaseOperationReturn = new DatabaseOperation01Return();
                DataOperations01 LDataOperations = new DataOperations01();

                foreach (DataRow LDataRowSingleRent in ADataTableRentList.Rows)
                {
                    LStrRentToken = LDataRowSingleRent["C021"].ToString();
                    foreach (DataRow LDataRowSingleTable in ADataTableMovedTables.Rows)
                    {
                        LStrTableName = LDataRowSingleTable["TableName"].ToString();
                        LStrDepentColumn = LDataRowSingleTable["P06"].ToString();
                        LStr00000001 = "LP_" + LStrTableName.Substring(2) + "." + LStrDepentColumn;
                        LStrDynamicSQL = "SELECT * FROM T_00_000 WHERE C000 = '" + LStrRentToken + "' AND C001 = '" + LStr00000001 + "' AND C004 = '1'";
                        LDatabaseOperationReturn = LDataOperations.SelectDataByDynamicSQL(IIntDatabaseType, IStrDatabaseProfile, LStrDynamicSQL);
                        if (!LDatabaseOperationReturn.BoolReturn)
                        {
                            WriteEntryLog("MoveRentData2LogicPartitionTable()\n" + LStrDynamicSQL + "\n" + LDatabaseOperationReturn.StrReturn, EventLogEntryType.Error);
                            continue;
                        }
                        if (LDatabaseOperationReturn.StrReturn != "1") { continue; }
                        DataTable LDataTableExistLogicPartitionTables = ObtainRentExistLogicPartitionTables(LStrRentToken, LStrTableName);
                        if (LDataTableExistLogicPartitionTables == null) { continue; }

                        MoverEveryRentData2LogicPartitionTable(LStrRentToken, LDataRowSingleTable, LDataTableExistLogicPartitionTables);
                    }
                }
            }
            catch (Exception ex)
            {
                WriteEntryLog("MoveRentData2LogicPartitionTable()\n" + ex.ToString(), EventLogEntryType.Error);
            }
        }
        #endregion

        #region 转移每个租户表中的数据，根据 租户 Token、表名
        private void MoverEveryRentData2LogicPartitionTable(string AStrRentToken, DataRow ADataRowSourceTable, DataTable AExistLogicPartitionTables)
        {
            List<string> LListStrExistTables = new List<string>();

            string LStrTableName = string.Empty;
            string LStrSerialColumn = string.Empty;
            string LStrDepentColumn = string.Empty;
            string LStrSourceTableName = string.Empty;
            string LStrTargetTableName = string.Empty;
            string LStrP07 = string.Empty, LStrP08 = string.Empty, LStrP09 = string.Empty;
            int LIntP08 = 0, LIntP09 = 0;

            string LStrDynamicSQL = string.Empty;
            List<string> LListStrDynamicSQL = new List<string>();

            string LStrDepentValue = string.Empty;
            string LStrPartitionSuffix = string.Empty;

            try
            {
                DatabaseOperation01Return LDatabaseOperationReturn = new DatabaseOperation01Return();
                DataOperations01 LDataOperations = new DataOperations01();

                foreach (DataRow LDataRowSingleTableName in AExistLogicPartitionTables.Rows) { LListStrExistTables.Add(LDataRowSingleTableName[0].ToString()); }

                LStrTableName = ADataRowSourceTable["TableName"].ToString();
                LStrSerialColumn = ADataRowSourceTable["P05"].ToString();
                LStrDepentColumn = ADataRowSourceTable["P06"].ToString();
                LStrP07 = ADataRowSourceTable["P07"].ToString();
                LStrP08 = ADataRowSourceTable["P08"].ToString();
                LStrP09 = ADataRowSourceTable["P09"].ToString();
                LIntP08 = int.Parse(LStrP08); LIntP09 = int.Parse(LStrP09);
                LStrSourceTableName = LStrTableName + "_" + AStrRentToken;

                LStrDynamicSQL = "SELECT * FROM " + LStrSourceTableName;
                LDatabaseOperationReturn = LDataOperations.SelectDataByDynamicSQL(IIntDatabaseType, IStrDatabaseProfile, LStrDynamicSQL);
                if (!LDatabaseOperationReturn.BoolReturn)
                {
                    WriteEntryLog("MoverEveryRentData2LogicPartitionTable()\n" + LDatabaseOperationReturn.StrReturn, EventLogEntryType.Error); return;
                }
                //WriteEntryLog("MoverEveryRentData2LogicPartitionTable()\n" + LStrDynamicSQL + "\nSelect Return Rows: " + LDatabaseOperationReturn.StrReturn, EventLogEntryType.Information);

                LListStrDynamicSQL.Clear();
                foreach (DataRow LDataRowSingleData in LDatabaseOperationReturn.DataSetReturn.Tables[0].Rows)
                {
                    LStrDepentValue = LDataRowSingleData[LStrDepentColumn].ToString();
                    LStrPartitionSuffix = LStrDepentValue.Substring(LIntP08, LIntP09);
                    LStrTargetTableName = LStrSourceTableName + "_" + LStrPartitionSuffix;
                    if (!LListStrExistTables.Contains(LStrTargetTableName))
                    {
                        if (!CreateLogicPartitionTableDBType2(LStrTableName, AStrRentToken, LStrPartitionSuffix)) { continue; }
                        LListStrExistTables.Add(LStrTargetTableName);
                        Create7MonthTable(ref LListStrExistTables, LStrPartitionSuffix, LStrTableName, AStrRentToken);

                        ///++++++++++++++++++++++++++++++++++++++++如果创建T_21_001_RentToken_YYmm也创建T_31_054_RentToken_YYmm+++++++++++++++++++++
                        if (LStrTableName.Equals("T_21_001"))
                        {
                            //1、查看有没有T_31_054_RentToken_YYmm表
                            String LStrTempT_31_054 = "T_31_054";
                            DataTable LDataTableTempExistLogicPartitionTables = ObtainRentExistLogicPartitionTables(AStrRentToken, LStrTempT_31_054);
                            if (LDataTableTempExistLogicPartitionTables == null) { continue; }

                            string LStrTempTargetTableName = LStrTempT_31_054 + "_" + AStrRentToken + "_" + LStrPartitionSuffix;
                            bool LBoolTempTargetHaveCreate= true;
                            foreach (DataRow LDataRowTempSingleTableName in LDataTableTempExistLogicPartitionTables.Rows)
                            {
                                if (LDataRowTempSingleTableName[0].ToString().Equals(LStrTempTargetTableName)) 
                                {
                                    LBoolTempTargetHaveCreate = false;
                                    break;
                                }
                            }
                            if (LBoolTempTargetHaveCreate) 
                            {
                                //2、创建T_31_054_RentToken_YYmm表
                                if (!CreateLogicPartitionTableDBType2(LStrTempT_31_054, AStrRentToken, LStrPartitionSuffix)) { continue; }
                            }                            
                        }
                        ///++++++++++++++++++++++++++++++++++++++++如果创建T_21_001_RentToken_YYmm也创建T_31_054_RentToken_YYmm+++++++++++++++++++++

                        LListStrExistTables.Add(LStrTargetTableName);
                    }
                    LStrDynamicSQL = "INSERT INTO " + LStrTargetTableName + " SELECT * FROM " + LStrSourceTableName + " WHERE " + LStrSerialColumn + " = " + LDataRowSingleData[LStrSerialColumn].ToString();
                    LListStrDynamicSQL.Add(LStrDynamicSQL);
                    LStrDynamicSQL = "DELETE FROM " + LStrSourceTableName + " WHERE " + LStrSerialColumn + " = " + LDataRowSingleData[LStrSerialColumn].ToString();
                    LListStrDynamicSQL.Add(LStrDynamicSQL);
                }
                ExecuteDynamicSQL(LListStrDynamicSQL);
            }
            catch (Exception ex)
            {
                WriteEntryLog("MoverEveryRentData2LogicPartitionTable()\n" + ex.ToString(), EventLogEntryType.Error);
            }
        }

        /// <summary>
        /// 创建前后3个月的分表
        /// </summary>
        /// <param name="LListStrExistTables"></param>
        /// <param name="strYearMonth"></param>
        /// <param name="LStrSourceTableName">T_21_001_00000</param>
        private void Create7MonthTable(ref List<string> LListStrExistTables, string strYearMonth, string LStrTableName, string AStrRentToken)
        {
            DateTime dt = Convert.ToDateTime("20" + strYearMonth.Insert(2, "-") + "-01 00:00:00");
            for (int i = -3; i < 4; i++)
            {
                if (i != 0)
                {
                    string tempYM = dt.AddMonths(i).ToString("yyMM");
                    string tempTBName = LStrTableName + "_" + AStrRentToken + "_" + tempYM;
                    if (!LListStrExistTables.Contains(tempTBName))
                    {
                        if(CreateLogicPartitionTableDBType2(LStrTableName, AStrRentToken, tempYM))
                            LListStrExistTables.Add(tempTBName);

                    }
                }
            }
        }
        #endregion

        #region 获取所有需要转移的数据表
        private DataTable ObtainMoveDataTables()
        {
            DataTable LDataTableReturn = new DataTable();
            string LStrXmlFileName = string.Empty;
            string LStrVerificationCode101 = string.Empty;
            string LStrMoveTables = string.Empty;

            try
            {
                #region 加载逻辑分区基本配置信息XML
                LStrXmlFileName = System.IO.Path.Combine(IStrSiteBaseDirectory, @"GlobalSettings\UMP.Server.02.xml");
                LStrVerificationCode101 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M101);

                XmlDocument LXmlDocServer02 = new XmlDocument();
                LXmlDocServer02.Load(LStrXmlFileName);
                XmlNode LXMLNodeLPTableList = LXmlDocServer02.SelectSingleNode("LPTableList");
                XmlNodeList LXmlNodeListPartitionTable = LXMLNodeLPTableList.ChildNodes;
                #endregion

                #region 初始化表
                LDataTableReturn.Columns.Add("TableName", typeof(string));      //表名
                LDataTableReturn.Columns.Add("P01", typeof(string));            //从基础表向租户表每次导入的数据条数
                LDataTableReturn.Columns.Add("P02", typeof(string));            //只导入这个参数值（单位：秒）前的数据
                LDataTableReturn.Columns.Add("P03", typeof(string));            //在下次导入前延时多少秒
                LDataTableReturn.Columns.Add("P04", typeof(string));            //参照的时间字段
                LDataTableReturn.Columns.Add("P05", typeof(string));            //表对应的主键 - 流水号
                LDataTableReturn.Columns.Add("P06", typeof(string));            //生成逻辑分区表名的依赖字段
                LDataTableReturn.Columns.Add("P07", typeof(string));            //获取方法
                LDataTableReturn.Columns.Add("P08", typeof(string));            //起始位置
                LDataTableReturn.Columns.Add("P09", typeof(string));            //字符数量
                #endregion

                #region 将分区表信息写入到返回的表中
                string LStrTableName = string.Empty;
                foreach (XmlNode LXmlNodeSingleLPTable in LXmlNodeListPartitionTable)
                {
                    if (LXmlNodeSingleLPTable.NodeType == XmlNodeType.Comment) { continue; }
                    DataRow LDataRow = LDataTableReturn.NewRow();
                    LDataRow.BeginEdit();
                    LStrTableName = EncryptionAndDecryption.EncryptDecryptString(LXmlNodeSingleLPTable.Attributes["Name"].Value, LStrVerificationCode101, EncryptionAndDecryption.UMPKeyAndIVType.M101);
                    LDataRow["TableName"] = LStrTableName;
                    LDataRow["P01"] = LXMLNodeLPTableList.Attributes["P01"].Value;
                    LDataRow["P02"] = LXMLNodeLPTableList.Attributes["P02"].Value;
                    LDataRow["P03"] = LXMLNodeLPTableList.Attributes["P03"].Value;
                    XmlNode LXmlNodePartitionColumn = LXmlNodeSingleLPTable.SelectSingleNode("PartitionColumn");
                    LDataRow["P04"] = EncryptionAndDecryption.EncryptDecryptString(LXmlNodePartitionColumn.Attributes["P03"].Value, LStrVerificationCode101, EncryptionAndDecryption.UMPKeyAndIVType.M101);
                    LDataRow["P05"] = EncryptionAndDecryption.EncryptDecryptString(LXmlNodePartitionColumn.Attributes["P04"].Value, LStrVerificationCode101, EncryptionAndDecryption.UMPKeyAndIVType.M101);
                    LDataRow["P06"] = EncryptionAndDecryption.EncryptDecryptString(LXmlNodePartitionColumn.Attributes["Name"].Value, LStrVerificationCode101, EncryptionAndDecryption.UMPKeyAndIVType.M101); ;
                    LDataRow["P07"] = EncryptionAndDecryption.EncryptDecryptString(LXmlNodePartitionColumn.Attributes["P00"].Value, LStrVerificationCode101, EncryptionAndDecryption.UMPKeyAndIVType.M101); ;
                    LDataRow["P08"] = LXmlNodePartitionColumn.Attributes["P01"].Value;
                    LDataRow["P09"] = LXmlNodePartitionColumn.Attributes["P02"].Value;
                    LDataRow.EndEdit();
                    LDataTableReturn.Rows.Add(LDataRow);
                    LStrMoveTables += LStrTableName;
                    if (LXmlNodeSingleLPTable.NextSibling != null) { LStrMoveTables += " /"; }
                }
                #endregion

                //if (!string.IsNullOrEmpty(LStrMoveTables))
                //{
                //    WriteEntryLog("ObtainMoveDataTables() " + LStrMoveTables, EventLogEntryType.Information);
                //}
            }
            catch (Exception ex)
            {
                LDataTableReturn = null;
                WriteEntryLog("ObtainMoveDataTables()\n" + ex.ToString(), EventLogEntryType.Error);
            }

            return LDataTableReturn;
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
                    WriteEntryLog("ObtainRentList()\n" + LDatabaseOperationReturn.StrReturn, EventLogEntryType.Error);
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
                WriteEntryLog("ObtainRentList()\n" + ex.ToString(), EventLogEntryType.Error);
            }

            return LDataTableReturn;
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
                    WriteEntryLog("ObtainRentExistLogicPartitionTables()\n" + LStrDynamicSQL + "\n" + LDatabaseOperationReturn.StrReturn, EventLogEntryType.Error);
                }
                else
                {
                    LDataTableReturn = LDatabaseOperationReturn.DataSetReturn.Tables[0];
                }
            }
            catch (Exception ex)
            {
                LDataTableReturn = null;
                WriteEntryLog("ObtainRentExistLogicPartitionTables()\n" + ex.ToString(), EventLogEntryType.Error);
            }
            return LDataTableReturn;
        }
        #endregion

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
                        WriteEntryLog("ExecuteDynamicSQL()\n" + LStrSingleDynamicSQL + "\n" + ex.ToString(), EventLogEntryType.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                LBoolReturn = false;
                WriteEntryLog("ExecuteDynamicSQL()\n" + ex.ToString(), EventLogEntryType.Error);
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

        private bool ExecuteDynamicSQL(List<string> AListStrSQLString, List<string> AListStrSQLMethod, List<string> AListStrSQLKey, List<string> AListStrSoureTable, ref List<string> AListRecordID)
        {
            bool LBoolReturn = true;

            SqlConnection LSqlConnection = null;
            SqlCommand LSqlCommand = null;
            OracleConnection LOracleConnection = null;
            OracleCommand LOracleCommand = null;
            int LIntExecuteReturn = 0;
            List<string> LListStrUpdateSQL = new List<string>();

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

                for (int LIntLoopDynamic = 0; LIntLoopDynamic < AListStrSQLString.Count; LIntLoopDynamic++)
                {
                    try
                    {
                        if (IIntDatabaseType == 2)
                        {
                            LSqlCommand = new SqlCommand(AListStrSQLString[LIntLoopDynamic], LSqlConnection);
                            LIntExecuteReturn = LSqlCommand.ExecuteNonQuery();
                        }
                        if (IIntDatabaseType == 3)
                        {
                            LOracleCommand = new OracleCommand(AListStrSQLString[LIntLoopDynamic], LOracleConnection);
                            LIntExecuteReturn = LOracleCommand.ExecuteNonQuery();
                        }
                        if (IIntDatabaseType > 0 &&
                            AListStrSQLMethod[LIntLoopDynamic] == "I" &&
                            AListStrSoureTable[LIntLoopDynamic] == "T_21_001")
                        {
                            //数据从T_21_001写入T_21_001_00000成功，记录 T_21_001.C001
                            AListRecordID.Add(AListStrSQLKey[LIntLoopDynamic]);
                        }
                    }
                    catch (Exception ex)
                    {
                        LBoolReturn = false;
                        WriteEntryLog("ExecuteDynamicSQL - 3()\n" + AListStrSQLString[LIntLoopDynamic] + "\n" + ex.ToString(), EventLogEntryType.Error);
                        if (AListStrSQLMethod[LIntLoopDynamic] == "I" && AListStrSoureTable[LIntLoopDynamic] == "T_21_001")
                        {
                            LListStrUpdateSQL.Clear();
                            LListStrUpdateSQL.Add("UPDATE T_21_001 SET C001 = " + AListStrSQLKey[LIntLoopDynamic] + " * (-1) WHERE C001 = " + AListStrSQLKey[LIntLoopDynamic]);
                            ExecuteDynamicSQL(LListStrUpdateSQL);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LBoolReturn = false;
                WriteEntryLog("ExecuteDynamicSQL() - 3\n" + ex.ToString(), EventLogEntryType.Error);
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
                WriteEntryLog("GetRentTokenByDataRow()\n" + ex.ToString(), EventLogEntryType.Error);
            }

            return LStrReturn;
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

                WriteEntryLog("GetDatabaseConnectionProfile() \n" + LStrDBConnectProfile, EventLogEntryType.Information);
            }
            catch (Exception ex)
            {
                IIntDatabaseType = 0;
                IStrDatabaseProfile = string.Empty;
                WriteEntryLog("GetDatabaseConnectionProfile()\n" + ex.ToString(), EventLogEntryType.Error);
            }
        }
        #endregion

        #region 获取客户化字段的配置信息
        /// <summary>
        /// 获取客户化字段的配置信息
        /// </summary>
        private void GetCustomFieldConfig()
        {
            string LStrXmlFileName = string.Empty;
            ILstCustomFieldConfig = new List<CCustomField>();

            try
            {
                ILstCustomFieldConfig.Clear();

                LStrXmlFileName = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                LStrXmlFileName = System.IO.Path.Combine(LStrXmlFileName, @"UMP.Server\Args04.UMP.xml");

                XmlDocument LXmlDocArgs04 = new XmlDocument();
                LXmlDocArgs04.Load(LStrXmlFileName);
                XmlNodeList LXmlNodeListDatabase = LXmlDocArgs04.SelectSingleNode("CustomFieldConfigs").ChildNodes;

                foreach (XmlNode LXmlNodeSingleDatabase in LXmlNodeListDatabase)
                {
                    CCustomField tempCF = new CCustomField();
                    tempCF.CustomName = LXmlNodeSingleDatabase.Attributes["P01"].Value;
                    tempCF.DBColumn = LXmlNodeSingleDatabase.Attributes["P02"].Value;
                    if (string.IsNullOrEmpty(tempCF.CustomName) || string.IsNullOrEmpty(tempCF.DBColumn))
                        continue;
                    ILstCustomFieldConfig.Add(tempCF);
                }
            }
            catch (Exception ex)
            {
                WriteEntryLog("GetCustomFieldConfig()\n" + ex.ToString(), EventLogEntryType.Error);
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
                WriteEntryLog("UMP.PF Site Directory : " + LStrReturn, EventLogEntryType.Information);
            }
            catch (Exception ex)
            {
                LStrReturn = string.Empty;
                WriteEntryLog("GetSiteBaseDirectory()\n" + ex.ToString(), EventLogEntryType.Error);
            }

            return LStrReturn;
        }
        #endregion

        #region 在 windows 的事件中写入日志
        private void WriteEntryLog(string AStrWriteBody, EventLogEntryType AEntryType)
        {
            try
            {
                //UMPServiceLog.WriteEntry(AStrWriteBody, AEntryType);
                WriteLog(AEntryType == EventLogEntryType.Error ? LogMode.Error : LogMode.Info, AStrWriteBody);
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

        #region 根据租户TOKEN和YYMM创建表
        private bool CreateLogicPartitionTableDBType2(string AStrTableName, string AStrRentToken, string AStrYearMonth)
        {
            bool LBoolReturn = true;

            #region 局部变量定义
            string LStrDataTypeConvertFile = string.Empty;
            string LStrTableObjectFolder = string.Empty;
            string LStrTableObjectFile = string.Empty;

            string LStrTableName = string.Empty;
            string LStrTabelVersion = string.Empty;
            string LStrCreateTableName = string.Empty;

            string LStrDDLSql = string.Empty;
            List<string> LStrCreateOrAlterSQL = new List<string>();

            string LStrColumnName = string.Empty;
            string LStrDataType = string.Empty;
            string LStrDataWidth = string.Empty;
            string LStrDataScale = string.Empty;
            string LStrCanNull = string.Empty;
            string LStrDataConverted = string.Empty;

            string LStrDefaultValue = string.Empty;
            string LStrDefValueName = string.Empty;

            string LStrIndexName = string.Empty;
            string LStrCreateIndexName = string.Empty;
            string LStrIsPrimaryKey = string.Empty;
            string LStrIsUnique = string.Empty;
            string LStrIndexType = string.Empty;
            string LStrIndexColumn = string.Empty;
            string LStrOrderType = string.Empty;
            #endregion

            try
            {
                #region 加载数据转换 和 表定义文件
                LStrDataTypeConvertFile = System.IO.Path.Combine(IStrSiteBaseDirectory, @"MAMT\Configrations\DBDataTypeConvert.XML");
                LStrTableObjectFolder = System.IO.Path.Combine(IStrSiteBaseDirectory, @"MAMT\ForLogicPartition");

                DataSet LDataSetDataTypeDefine = new DataSet();
                StreamReader LStreamReader = new StreamReader(LStrDataTypeConvertFile, Encoding.UTF8);
                LDataSetDataTypeDefine.ReadXml(LStreamReader);
                LStreamReader.Close();

                LStrTableObjectFile = System.IO.Path.Combine(LStrTableObjectFolder, AStrTableName + ".XML");
                XmlDocument LXmlDocumentTable = new XmlDocument();
                LXmlDocumentTable.Load(LStrTableObjectFile);
                #endregion

                #region 获取表名，并校验表名是否正确
                XmlNode LXMLNodeTableDefine = LXmlDocumentTable.SelectSingleNode("TableDefine");
                LStrTableName = LXMLNodeTableDefine.Attributes["P01"].Value;
                LStrTabelVersion = LXMLNodeTableDefine.Attributes["P02"].Value;

                if (LStrTableName != AStrTableName)
                {
                    WriteEntryLog("CreateLogicPartitionTable()\n" + LStrTableObjectFile + "!= " + LStrTableName, EventLogEntryType.Error);
                    return false;
                }
                #endregion

                LStrCreateTableName = LStrTableName + "_" + AStrRentToken + "_" + AStrYearMonth;

                if (IIntDatabaseType == 2)
                {
                    #region 生成创建表的语法
                    LStrDDLSql = "CREATE TABLE " + LStrCreateTableName + "(\n";
                    XmlNodeList LXmlNodeListTableColumns = LXMLNodeTableDefine.SelectSingleNode("ColumnDefine").ChildNodes;
                    foreach (XmlNode LXmlNodeSingleColumn in LXmlNodeListTableColumns)
                    {
                        LStrColumnName = LXmlNodeSingleColumn.Attributes["P01"].Value;
                        LStrDataType = LXmlNodeSingleColumn.Attributes["P02"].Value;
                        LStrDataWidth = LXmlNodeSingleColumn.Attributes["P03"].Value;
                        LStrDataScale = LXmlNodeSingleColumn.Attributes["P04"].Value;
                        LStrCanNull = LXmlNodeSingleColumn.Attributes["P05"].Value;

                        LStrDDLSql += " " + LStrColumnName;
                        DataRow[] LDataRowDataConvert = LDataSetDataTypeDefine.Tables[0].Select("Type0 = '" + LStrDataType + "'");
                        LStrDataConverted = LDataRowDataConvert[0]["Type2"].ToString();
                        if (LStrDataType == "01" || LStrDataType == "02" || LStrDataType == "03" || LStrDataType == "21")
                        {
                            LStrDDLSql += "     " + LStrDataConverted;
                        }
                        else if (LStrDataType == "04")
                        {
                            LStrDDLSql += "     " + LStrDataConverted + "(" + LStrDataWidth + "," + LStrDataScale + ")";
                        }
                        else if (LStrDataType == "11" || LStrDataType == "12" || LStrDataType == "13" || LStrDataType == "14")
                        {
                            LStrDDLSql += "     " + LStrDataConverted + "(" + LStrDataWidth + ")";
                        }
                        if (LStrCanNull == "0") { LStrDDLSql += "       NOT NULL"; } else { LStrDDLSql += "       NULL"; }
                        if (LXmlNodeSingleColumn.NextSibling != null) { LStrDDLSql += ",\n"; }
                    }
                    LStrDDLSql += "\n)";
                    LStrCreateOrAlterSQL.Add(LStrDDLSql);

                    LStrDDLSql = "INSERT INTO T_00_000(C000, C001, C002, C003, C004, C005, C007, C010) VALUES('" + AStrRentToken + "', '" + LStrCreateTableName + "', '" + LStrTabelVersion + "', 'TB', '1', GETDATE(), '0', 'NULL')";
                    LStrCreateOrAlterSQL.Add(LStrDDLSql);
                    #endregion

                    #region 生成字段的 Default Value 语法
                    foreach (XmlNode LXmlNodeSingleColumn in LXmlNodeListTableColumns)
                    {
                        LStrDefaultValue = LXmlNodeSingleColumn.Attributes["P06"].Value.Trim();
                        if (string.IsNullOrEmpty(LStrDefaultValue)) { continue; }
                        LStrColumnName = LXmlNodeSingleColumn.Attributes["P01"].Value;
                        LStrDefValueName = "DF_" + LStrCreateTableName.Substring(2) + "_" + LStrColumnName;
                        LStrDDLSql = "ALTER TABLE " + LStrCreateTableName + " ADD CONSTRAINT " + LStrDefValueName + " DEFAULT (" + LStrDefaultValue + ") FOR " + LStrColumnName;
                        LStrCreateOrAlterSQL.Add(LStrDDLSql);
                        LStrDDLSql = "INSERT INTO T_00_000(C000, C001, C002, C003, C004, C005, C007, C010) VALUES('" + AStrRentToken + "', '" + LStrDefValueName + "', '" + LStrTabelVersion + "', 'DF', '1', GETDATE(), '0', '" + LStrCreateTableName + "')";
                        LStrCreateOrAlterSQL.Add(LStrDDLSql);
                    }
                    #endregion

                    #region 创建主键、索引
                    XmlNodeList LXMLNodeIndexDefine = LXMLNodeTableDefine.SelectSingleNode("IndexDefine").ChildNodes;
                    foreach (XmlNode LXmlNodeSingleIndex in LXMLNodeIndexDefine)
                    {
                        LStrIndexName = LXmlNodeSingleIndex.Attributes["P01"].Value;
                        LStrCreateIndexName = LStrIndexName + "_" + AStrRentToken + "_" + AStrYearMonth;

                        LStrIsPrimaryKey = LXmlNodeSingleIndex.Attributes["P02"].Value;
                        LStrIsUnique = LXmlNodeSingleIndex.Attributes["P03"].Value;
                        if (LStrIsPrimaryKey == "1") { LStrDDLSql = "ALTER TABLE " + LStrCreateTableName + " ADD  CONSTRAINT " + LStrCreateIndexName + " PRIMARY KEY CLUSTERED("; LStrIndexType = "PK"; }
                        else
                        {
                            if (LStrIsUnique == "1") { LStrDDLSql = "CREATE UNIQUE NONCLUSTERED INDEX " + LStrCreateIndexName + " ON " + LStrCreateTableName + "("; }
                            else { LStrDDLSql = "CREATE NONCLUSTERED INDEX " + LStrCreateIndexName + " ON " + LStrCreateTableName + "("; }
                            LStrIndexType = "IX";
                        }
                        foreach (XmlNode LXmlNodeSingleIndexColumn in LXmlNodeSingleIndex.ChildNodes)
                        {
                            LStrIndexColumn = LXmlNodeSingleIndexColumn.Attributes["P01"].Value;
                            LStrOrderType = LXmlNodeSingleIndexColumn.Attributes["P02"].Value;
                            if (LStrOrderType == "A") { LStrDDLSql += LStrIndexColumn + " ASC"; } else { LStrDDLSql += LStrIndexColumn + " DESC"; }
                            if (LXmlNodeSingleIndexColumn.NextSibling != null) { LStrDDLSql += ","; }
                        }
                        LStrDDLSql += ")";
                        LStrCreateOrAlterSQL.Add(LStrDDLSql);

                        LStrDDLSql = "INSERT INTO T_00_000(C000, C001, C002, C003, C004, C005, C007, C010) VALUES('" + AStrRentToken + "', '" + LStrCreateIndexName + "', '" + LStrTabelVersion + "', '" + LStrIndexType + "', '1', GETDATE(), '0', '" + LStrCreateTableName + "')";
                        LStrCreateOrAlterSQL.Add(LStrDDLSql);
                    }
                    #endregion
                }
                if (IIntDatabaseType == 3)
                {
                    #region 生成创建表的语法
                    LStrDDLSql = "CREATE TABLE " + LStrCreateTableName + "(\n";
                    XmlNodeList LXmlNodeListTableColumns = LXMLNodeTableDefine.SelectSingleNode("ColumnDefine").ChildNodes;
                    foreach (XmlNode LXmlNodeSingleColumn in LXmlNodeListTableColumns)
                    {
                        LStrColumnName = LXmlNodeSingleColumn.Attributes["P01"].Value;
                        LStrDataType = LXmlNodeSingleColumn.Attributes["P02"].Value;
                        LStrDataWidth = LXmlNodeSingleColumn.Attributes["P03"].Value;
                        LStrDataScale = LXmlNodeSingleColumn.Attributes["P04"].Value;
                        LStrCanNull = LXmlNodeSingleColumn.Attributes["P05"].Value;

                        LStrDDLSql += " " + LStrColumnName;
                        DataRow[] LDataRowDataConvert = LDataSetDataTypeDefine.Tables[0].Select("Type0 = '" + LStrDataType + "'");
                        LStrDataConverted = LDataRowDataConvert[0]["Type3"].ToString();
                        if (LStrDataType == "01" || LStrDataType == "02" || LStrDataType == "03" || LStrDataType == "21")
                        {
                            LStrDDLSql += "     " + LStrDataConverted;
                        }
                        else if (LStrDataType == "04")
                        {
                            LStrDDLSql += "     " + LStrDataConverted + "(" + LStrDataWidth + "," + LStrDataScale + ")";
                        }
                        else if (LStrDataType == "11" || LStrDataType == "12" || LStrDataType == "13" || LStrDataType == "14")
                        {
                            LStrDDLSql += "     " + LStrDataConverted + "(" + LStrDataWidth + ")";
                        }
                        if (LStrCanNull == "0") { LStrDDLSql += "       NOT NULL"; } else { LStrDDLSql += "       NULL"; }
                        if (LXmlNodeSingleColumn.NextSibling != null) { LStrDDLSql += ",\n"; }
                    }
                    LStrDDLSql += "\n)";
                    LStrCreateOrAlterSQL.Add(LStrDDLSql);

                    LStrDDLSql = "INSERT INTO T_00_000(C000, C001, C002, C003, C004, C005, C007, C010) VALUES('" + AStrRentToken + "', '" + LStrCreateTableName + "', '" + LStrTabelVersion + "', 'TB', '1', SYSDATE, '0', 'NULL')";
                    LStrCreateOrAlterSQL.Add(LStrDDLSql);
                    #endregion

                    #region 生成字段的 Default Value 语法
                    foreach (XmlNode LXmlNodeSingleColumn in LXmlNodeListTableColumns)
                    {
                        LStrDefaultValue = LXmlNodeSingleColumn.Attributes["P06"].Value.Trim();
                        if (string.IsNullOrEmpty(LStrDefaultValue)) { continue; }
                        LStrColumnName = LXmlNodeSingleColumn.Attributes["P01"].Value;

                        LStrDefValueName = "DF_" + LStrCreateTableName.Substring(2) + "_" + LStrColumnName;
                        LStrDDLSql = "ALTER TABLE " + LStrCreateTableName + " MODIFY (" + LStrColumnName + " DEFAULT " + LStrDefaultValue + ")";
                        LStrCreateOrAlterSQL.Add(LStrDDLSql);
                        LStrDDLSql = "INSERT INTO T_00_000(C000, C001, C002, C003, C004, C005, C007, C010) VALUES('" + AStrRentToken + "', '" + LStrDefValueName + "', '" + LStrTabelVersion + "', 'DF', '1', SYSDATE, '0', '" + LStrCreateTableName + "')";
                        LStrCreateOrAlterSQL.Add(LStrDDLSql);
                    }
                    #endregion

                    #region 创建主键、索引
                    XmlNodeList LXMLNodeIndexDefine = LXMLNodeTableDefine.SelectSingleNode("IndexDefine").ChildNodes;
                    foreach (XmlNode LXmlNodeSingleIndex in LXMLNodeIndexDefine)
                    {
                        LStrIndexName = LXmlNodeSingleIndex.Attributes["P01"].Value;
                        LStrCreateIndexName = LStrIndexName + "_" + AStrRentToken + "_" + AStrYearMonth;

                        LStrIsPrimaryKey = LXmlNodeSingleIndex.Attributes["P02"].Value;
                        LStrIsUnique = LXmlNodeSingleIndex.Attributes["P03"].Value;
                        if (LStrIsPrimaryKey == "1")
                        {
                            LStrDDLSql = "ALTER TABLE " + LStrCreateTableName + " ADD ( CONSTRAINT " + LStrCreateIndexName + " PRIMARY KEY ("; LStrIndexType = "PK";
                            foreach (XmlNode LXmlNodeSingleIndexColumn in LXmlNodeSingleIndex.ChildNodes)
                            {
                                LStrIndexColumn = LXmlNodeSingleIndexColumn.Attributes["P01"].Value;
                                LStrDDLSql += LStrIndexColumn;
                                if (LXmlNodeSingleIndexColumn.NextSibling != null) { LStrDDLSql += ","; }
                            }
                            LStrDDLSql += "))";
                        }
                        else
                        {
                            if (LStrIsUnique == "1") { LStrDDLSql = "CREATE UNIQUE INDEX " + LStrCreateIndexName + " ON " + LStrCreateTableName + " ("; }
                            else { LStrDDLSql = "CREATE INDEX " + LStrCreateIndexName + " ON " + LStrCreateTableName + " ("; }
                            LStrIndexType = "IX";
                            foreach (XmlNode LXmlNodeSingleIndexColumn in LXmlNodeSingleIndex.ChildNodes)
                            {
                                LStrIndexColumn = LXmlNodeSingleIndexColumn.Attributes["P01"].Value;
                                LStrOrderType = LXmlNodeSingleIndexColumn.Attributes["P02"].Value;
                                if (LStrOrderType == "A") { LStrDDLSql += LStrIndexColumn + " ASC"; } else { LStrDDLSql += LStrIndexColumn + " DESC"; }
                                if (LXmlNodeSingleIndexColumn.NextSibling != null) { LStrDDLSql += ","; }
                            }
                            LStrDDLSql += ")";
                        }

                        LStrCreateOrAlterSQL.Add(LStrDDLSql);

                        LStrDDLSql = "INSERT INTO T_00_000(C000, C001, C002, C003, C004, C005, C007, C010) VALUES('" + AStrRentToken + "', '" + LStrCreateIndexName + "', '" + LStrTabelVersion + "', '" + LStrIndexType + "', '1', SYSDATE, '0', '" + LStrCreateTableName + "')";
                        LStrCreateOrAlterSQL.Add(LStrDDLSql);
                    }
                    #endregion
                }
                LBoolReturn = ExecuteDynamicSQL(LStrCreateOrAlterSQL);
            }
            catch (Exception ex)
            {
                LBoolReturn = false;
                WriteEntryLog("CreateLogicPartitionTable()\n" + ex.ToString(), EventLogEntryType.Error);
            }

            return LBoolReturn;
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
                WriteEntryLog("Read UMP.PF Binding Port Information ...", EventLogEntryType.Information);

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

                WriteEntryLog("Readed UMP.PF Binding Port : " + LIntReturn.ToString() + ", This Service Use " + (LIntReturn - 2).ToString(), EventLogEntryType.Information);
            }
            catch (Exception ex)
            {
                LIntReturn = 0;
                WriteEntryLog("Read UMP.PF Binding Port Information Failed\n" + ex.ToString(), EventLogEntryType.Error);
            }

            return LIntReturn;
        }
        #endregion

        #region 获取系统19位流水号
        private long GetSerialIDByType(List<string> AListStringArgs, int AIntModule, int AIntType)
        {
            long LLongSerialID = 0;

            try
            {
                DatabaseOperation01Return LDatabaseOperation01Return = new DatabaseOperation01Return();
                DataOperations01 LDataOperations01 = new DataOperations01();
                LDatabaseOperation01Return = LDataOperations01.GetSerialNumberByProcedure(int.Parse(AListStringArgs[0]), AListStringArgs[1], AIntModule, AIntType, AListStringArgs[2], "20140101000000");
                if (!LDatabaseOperation01Return.BoolReturn) { LLongSerialID = 0; }
                else { LLongSerialID = long.Parse(LDatabaseOperation01Return.StrReturn); }
            }
            catch { LLongSerialID = 0; }

            return LLongSerialID;
        }

        private long GetSerialIDByType(int AIntDBType, string AStrDBProfile, string AStrRentToken, int AIntModule, int AIntType)
        {
            long LLongSerialID = 0;

            try
            {
                DatabaseOperation01Return LDatabaseOperation01Return = new DatabaseOperation01Return();
                DataOperations01 LDataOperations01 = new DataOperations01();
                LDatabaseOperation01Return = LDataOperations01.GetSerialNumberByProcedure(AIntDBType, AStrDBProfile, AIntModule, AIntType, AStrRentToken, "20140101000000");
                if (!LDatabaseOperation01Return.BoolReturn) { LLongSerialID = 0; }
                else { LLongSerialID = long.Parse(LDatabaseOperation01Return.StrReturn); }
            }
            catch { LLongSerialID = 0; }

            return LLongSerialID;
        }

        private long GetSerialIDByType(string AStrC000, int AIntDBType, string AStrDBProfile, string AStrRentToken, int AIntModule, int AIntType, string AStrTime)
        {
            long LLongSerialID = 0;

            try
            {
                DatabaseOperation01Return LDatabaseOperation01Return = new DatabaseOperation01Return();
                DataOperations01 LDataOperations01 = new DataOperations01();
                LDatabaseOperation01Return = LDataOperations01.GetSerialNumberByProcedure(AIntDBType, AStrDBProfile, AIntModule, AIntType, AStrRentToken, AStrTime);
                if (!LDatabaseOperation01Return.BoolReturn) { LLongSerialID = 0; }
                else { LLongSerialID = long.Parse(LDatabaseOperation01Return.StrReturn); }
                //WriteEntryLog("GetSerialIDByType()\n" + LDatabaseOperation01Return.StrReturn, EventLogEntryType.Warning);
            }
            catch (Exception ex)
            {
                LLongSerialID = 0;
                WriteEntryLog("GetSerialIDByType()\nC000 = " + AStrC000 + "\nTime = " + AStrTime + "\n" + ex.ToString(), EventLogEntryType.Error);
            }

            return LLongSerialID;
        }
        #endregion

        #region 处理A&R数据

        /// <summary>
        /// 处理A&R数据
        /// </summary>
        /// <param name="ARentToken">记录所属租户</param>
        /// <param name="AListRecordID">转换成功的记录</param>
        /// <param name="LDtARData">选择的待转换记录</param>
        private void ARDataManage(string ARentToken, List<string> AListRecordID, DataTable LDtARData)
        {
            if (AListRecordID.Count == 0) { return; }
            DataTable DtSuccessData = LDtARData.Clone();
            foreach (DataRow dr in LDtARData.Rows)
            {
                if (AListRecordID.Contains(dr["C001"].ToString()))
                    DtSuccessData.ImportRow(dr);
            }
            string LStrDynamicSQL = string.Empty;
            int fcount = DtSuccessData.Rows.Count;

            //T_00_202
            string LStr202C003 = string.Empty;
            string LStr202C009 = string.Empty;

            //T_11_101_00000
            string LStr101C001 = "0";
            string LStr101C011 = string.Empty;
            try
            {
                string LStrTableName = "T_21_001";
                DataTable LDataTableExistLogicPartitionTables = ObtainRentExistLogicPartitionTables(ARentToken, LStrTableName);
                string structnow = DateTime.UtcNow.ToString("yyyyMMddHHmmss");

                DatabaseOperation01Return LDatabase202Return = new DatabaseOperation01Return();
                DataOperations01 LData202Operations = new DataOperations01();
                DatabaseOperation01Return LDatabase204Return = new DatabaseOperation01Return();
                DataOperations01 LData204Operations = new DataOperations01();
                DatabaseOperation01Return LDatabase101Return = new DatabaseOperation01Return();
                DataOperations01 LData101Operations = new DataOperations01();

                #region 查询T_00_202中的有效策略 启用&未删除&有效时间&租户对应
                LStrDynamicSQL = string.Empty;
                if (IIntDatabaseType == 2)
                {
                    LStrDynamicSQL = string.Format("SELECT * FROM T_00_202 WHERE C005='1' AND C006='0' AND C010<{0} AND C011>{0} AND C000='{1}'", structnow, ARentToken);
                }
                if (IIntDatabaseType == 3)
                {
                    LStrDynamicSQL = string.Format("SELECT * FROM T_00_202 WHERE C005='1' AND C006='0' AND C010<{0} AND C011>{0} AND C000='{1}'", structnow, ARentToken);
                }
                if (string.IsNullOrEmpty(LStrDynamicSQL)) { return; }
                LDatabase202Return = LData202Operations.SelectDataByDynamicSQL(IIntDatabaseType, IStrDatabaseProfile, LStrDynamicSQL);
                if (!LDatabase202Return.BoolReturn)
                {
                    WriteEntryLog("ARDataManage()\n" + LStrDynamicSQL + "\n" + LDatabase202Return.StrReturn, EventLogEntryType.Error);
                    return;
                }
                #endregion

                #region //根据配置的A&R策略
                LStrDynamicSQL = string.Empty;
                if (IIntDatabaseType == 2)
                {
                    LStrDynamicSQL = string.Format("SELECT C001,C011 FROM T_11_101_{0} WHERE C001 LIKE '256%' AND C002=3", ARentToken);
                }
                if (IIntDatabaseType == 3)
                {
                    LStrDynamicSQL = string.Format("SELECT C001,C011 FROM T_11_101_{0} WHERE C001 LIKE '256%' AND C002=3", ARentToken);
                }
                if (string.IsNullOrEmpty(LStrDynamicSQL)) { return; }
                LDatabase101Return = LData101Operations.SelectDataByDynamicSQL(IIntDatabaseType, IStrDatabaseProfile, LStrDynamicSQL);
                if (!LDatabase101Return.BoolReturn)
                {
                    WriteEntryLog("ARDataManage()\n" + LStrDynamicSQL + "\n" + LDatabase101Return.StrReturn, EventLogEntryType.Error);
                    return;
                }
                if (LDatabase101Return.DataSetReturn.Tables[0] != null && LDatabase101Return.DataSetReturn.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow LDataRowSingle in LDatabase101Return.DataSetReturn.Tables[0].Rows)//已经查出的数据
                    {
                        LStr101C001 = LDataRowSingle["C001"].ToString();//2560000000000000001
                        LStr101C011 = LDataRowSingle["C011"].ToString();//2571507100900000001;2571507101100000001
                        DataRow[] FoundRows;//符合筛选条件的记录
                        string[] arrFilter = LStr101C011.Split(';');
                        foreach (string strFilter in arrFilter)
                        {
                            foreach (DataRow LDataRowSingle202 in LDatabase202Return.DataSetReturn.Tables[0].Rows)//已经查出的数据
                            {
                                LStr202C003 = LDataRowSingle202["C003"].ToString();
                                LStr202C009 = LDataRowSingle202["C009"].ToString();
                                if (strFilter == LStr202C003)//对于匹配的筛选策略，再查询204表进行筛选数据
                                {
                                    LStrDynamicSQL = string.Empty;
                                    if (IIntDatabaseType == 2)
                                    {
                                        LStrDynamicSQL = string.Format("SELECT * FROM T_00_204 WHERE C001={0}", LStr202C003);
                                    }
                                    if (IIntDatabaseType == 3)
                                    {
                                        LStrDynamicSQL = string.Format("SELECT * FROM T_00_204 WHERE C001={0}", LStr202C003);
                                    }
                                    if (string.IsNullOrEmpty(LStrDynamicSQL)) { return; }
                                    LDatabase204Return = LData204Operations.SelectDataByDynamicSQL(IIntDatabaseType, IStrDatabaseProfile, LStrDynamicSQL);
                                    if (!LDatabase204Return.BoolReturn)
                                    {
                                        WriteEntryLog("ARDataManage()\n" + LStrDynamicSQL + "\n" + LDatabase204Return.StrReturn, EventLogEntryType.Error);
                                        continue;
                                    }
                                    if (LDatabase204Return.DataSetReturn.Tables[0].Rows.Count == 0)
                                    {
                                        WriteEntryLog("ARDataManage()\n" + LStrDynamicSQL + "\n" + "LDatabase204ReturnCount=0", EventLogEntryType.Warning);
                                        continue;
                                    }
                                    string rowfilter = string.Empty;
                                    string filtertarget = "1";
                                    bool isPCMCondation = false;
                                    foreach (DataRow LDataRow in LDatabase204Return.DataSetReturn.Tables[0].Rows)
                                    {
                                        string dtinput = LDataRow["C008"].ToString();
                                        if (IsDateTime(dtinput))//假如是时间格式的值
                                            dtinput = Convert.ToDateTime(dtinput).ToString("yyyy-MM-dd HH:mm:ss");
                                        filtertarget = LDataRow["C002"].ToString();
                                        string strC005 = LDataRow["C005"].ToString();
                                        string strC006 = LDataRow["C006"].ToString();
                                        if (strC005.ToUpper() == "C014" && dtinput == "3")//假如筛选条件是C014媒体类型 并且值是3（PCM）
                                        {
                                            isPCMCondation = true;
                                            rowfilter += " (C014='1') AND (C078<>'' OR C079<>'' OR C080<>'')";
                                            if (LDataRow["C009"] != null && LDataRow["C009"].ToString() != "")
                                                rowfilter += " " + LDataRow["C009"].ToString();
                                            else
                                                rowfilter += " " + "AND";
                                            continue;
                                        }
                                        if (strC006.ToUpper() != "IN")
                                        {
                                            if (strC006 == "<>")
                                            {
                                                rowfilter += " ("
                                                    + strC005
                                                    + " "
                                                    + strC006
                                                    + "'"
                                                    + dtinput
                                                    + "' OR "
                                                    + strC005
                                                    
                                                    + " IS NULL)";
                                            }
                                            else
                                            {
                                                rowfilter += " ("
                                                    + strC005
                                                    + " "
                                                    + strC006
                                                    + "'"
                                                    + dtinput
                                                    + "')";
                                            }
                                        }
                                        else
                                        {
                                            rowfilter += " ("
                                                + strC005
                                                + " "
                                                + strC006
                                                + "("
                                                + dtinput
                                                + "))";
                                        }
                                        if (LDataRow["C009"] != null && LDataRow["C009"].ToString() != "")
                                            rowfilter += " " + LDataRow["C009"].ToString();
                                        else
                                            rowfilter += " " + "AND";
                                    }
                                    if (rowfilter.Substring(rowfilter.Length - 2).ToUpper() == "OR")//逻辑串末尾是 OR
                                        rowfilter = rowfilter.Substring(0, rowfilter.Length - 2);
                                    else if (rowfilter.Substring(rowfilter.Length - 3).ToUpper() == "AND")//逻辑串末尾是 AND
                                        rowfilter = rowfilter.Substring(0, rowfilter.Length - 3);

                                    FoundRows = DtSuccessData.Select(rowfilter);
                                    if (FoundRows.Count() > 0)
                                    {
                                        //满足筛选条件的记录大于0 写入T_00_205 同时更新T_00_202的筛选数量
                                        SaveResultTo202And205(LStr202C003, ARentToken, FoundRows, filtertarget, LStr101C001, isPCMCondation);
                                    }
                                }
                            }
                        }
                    }
                }
                else//没有配置AR策略
                { return; }
                #endregion

                #region //逐条处理筛选策略
                //foreach (DataRow LDataRowSingle in LDatabase202Return.DataSetReturn.Tables[0].Rows)
                //{
                //    LStr202C003 = LDataRowSingle["C003"].ToString();
                //    LStr202C009 = LDataRowSingle["C009"].ToString();
                //    DataRow[] FoundRows;//符合筛选条件的记录

                //    LStrDynamicSQL = string.Empty;
                //    if (IIntDatabaseType == 2)
                //    {
                //        LStrDynamicSQL = string.Format("SELECT * FROM T_00_204 WHERE C001={0}", LStr202C003);
                //    }
                //    if (IIntDatabaseType == 3)
                //    {
                //        LStrDynamicSQL = string.Format("SELECT * FROM T_00_204 WHERE C001={0}", LStr202C003);
                //    }
                //    if (string.IsNullOrEmpty(LStrDynamicSQL)) { return; }
                //    LDatabase204Return = LData204Operations.SelectDataByDynamicSQL(IIntDatabaseType, IStrDatabaseProfile, LStrDynamicSQL);
                //    if (!LDatabase204Return.BoolReturn)
                //    {
                //        WriteEntryLog("ARDataManage()\n" + LStrDynamicSQL + "\n" + LDatabase204Return.StrReturn, EventLogEntryType.Error);
                //        continue;
                //    }
                //    if (LDatabase204Return.DataSetReturn.Tables[0].Rows.Count == 0)
                //    {
                //        WriteEntryLog("ARDataManage()\n" + LStrDynamicSQL + "\n" + "LDatabase204ReturnCount=0", EventLogEntryType.Warning);
                //        continue;
                //    }
                //    string rowfilter = string.Empty;
                //    string filtertarget = "1";
                //    foreach (DataRow LDataRow in LDatabase204Return.DataSetReturn.Tables[0].Rows)
                //    {
                //        string dtinput = LDataRow["C008"].ToString();
                //        if (IsDateTime(dtinput))//假如是时间格式的值 则转换为string的UTC
                //            dtinput = Convert.ToDateTime(dtinput).ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss");
                //        filtertarget = LDataRow["C002"].ToString();
                //        if (LDataRow["C006"].ToString().ToLower() != "in")
                //        {
                //            rowfilter += " ("
                //                + LDataRow["C005"].ToString()
                //                + " "
                //                + LDataRow["C006"].ToString()
                //                + "'"
                //                + dtinput
                //                + "')";
                //        }
                //        else
                //        {
                //            rowfilter += " ("
                //                + LDataRow["C005"].ToString()
                //                + " "
                //                + LDataRow["C006"].ToString()
                //                + "("
                //                + dtinput
                //                + "))";
                //        }
                //        if (LDataRow["C009"] != null && LDataRow["C009"].ToString() != "")
                //            rowfilter += " " + LDataRow["C009"].ToString();
                //        else
                //            rowfilter += " " + "AND";
                //    }
                //    if (rowfilter.Substring(rowfilter.Length - 2) == "OR")//逻辑串末尾是 OR
                //        rowfilter = rowfilter.Substring(0, rowfilter.Length - 2);
                //    else//逻辑串末尾是 AND
                //        rowfilter = rowfilter.Substring(0, rowfilter.Length - 3);

                //    FoundRows = DtSuccessData.Select(rowfilter);
                //    if (FoundRows.Count() > 0)
                //    {
                //        //满足筛选条件的记录大于0 写入T_00_205 同时更新T_00_202的筛选数量
                //        int tempcount = Convert.ToInt32(LDataRowSingle["C009"].ToString()) + FoundRows.Count();
                //        SaveResultTo202And205(tempcount, LStr202C003, ARentToken, FoundRows, filtertarget, LStr101C001);
                //    }
                //}
                #endregion
            }
            catch (Exception ex)
            {
                WriteEntryLog("ARDataManage()\n" + ex.ToString(), EventLogEntryType.Error);
            }
        }

        #region 更新筛选结果到T_00_202，T_00_205
        /// <summary>
        /// 更新筛选结果到T_00_202，T_00_205
        /// </summary>
        /// <param name="tempcount">满足条件的筛选结果</param>
        /// <param name="LStr202C003">筛选策略编码</param>
        /// <param name="ARentToken">租户</param>
        /// <param name="LStr101C001">筛选策略编码</param>
        private void SaveResultTo202And205(string LStr202C003, string ARentToken, DataRow[] FoundRows, string filtertarget, string LStr101C001, bool isPCMCondation)
        {
            try
            {
                #region update202
                DatabaseOperation01Return LDatabaseReturn = new DatabaseOperation01Return();
                DataOperations01 LDataOperations = new DataOperations01();
                string LStrDynamicSQL = string.Empty;
                if (IIntDatabaseType == 2)
                {
                    LStrDynamicSQL = string.Format("UPDATE T_00_202 SET C009=C009+{0} WHERE C003={1} AND C006='0' AND C000='{2}'", FoundRows.Count(), LStr202C003, ARentToken);
                }
                if (IIntDatabaseType == 3)
                {
                    LStrDynamicSQL = string.Format("UPDATE T_00_202 SET C009=C009+{0} WHERE C003={1} AND C006='0' AND C000='{2}'", FoundRows.Count(), LStr202C003, ARentToken);
                }
                if (string.IsNullOrEmpty(LStrDynamicSQL)) { return; }
                LDatabaseReturn = LDataOperations.SelectDataByDynamicSQL(IIntDatabaseType, IStrDatabaseProfile, LStrDynamicSQL);
                if (!LDatabaseReturn.BoolReturn)
                {
                    WriteEntryLog("SaveResultTo202And205()\n" + "UPDATE202" + "\n" + LDatabaseReturn.StrReturn, EventLogEntryType.Warning);
                    return;
                }
                #endregion
                #region insert2205
                List<string> LStrInsertSQL = new List<string>();
                string strAllRecord = "";
                foreach (DataRow dr in FoundRows)
                {
                    string temp005 = "";
                    string temp006 = "";
                    string temp007 = "";
                    if (isPCMCondation)//PCM条件
                    {
                        temp005 = ConvertString(dr["C078"], "");
                        temp006 = ConvertString(dr["C079"], "");
                        temp007 = ConvertString(dr["C080"], "");
                    }
                    else
                    {
                        temp005 = ConvertString(dr["C035"], "");
                        temp006 = ConvertString(dr["C036"], "");
                    }
                    //INSERT INTO T_00_205(C001, C002, C003, C004, C011, C012,C008) VALUES(1,2,3,4,5,'00000','192.168.4.1')
                    string strutc = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
                    string sql = string.Format("INSERT INTO T_00_205(C001,C002,C003,C004,C005,C006,C007,C011,C012,C008) VALUES({0},{1},{2},{3},'{4}','{5}','{6}',{7},'{8}','{9}')",
                        LStr202C003,
                        LStr101C001,
                        ConvertString(dr["C001"], "-1"),
                        ConvertString(dr["C002"], "-1"),
                        temp005,
                        temp006,
                        temp007,
                        strutc,
                        ARentToken,
                        ConvertString(dr["C020"], "127.0.0.1"));
                    LStrInsertSQL.Add(sql);
                    strAllRecord += ConvertString(dr["C002"], "-1") + ",";
                }
                ExecuteDynamicSQL(LStrInsertSQL);
                WriteEntryLog("ARSuccessData:\n" + strAllRecord, EventLogEntryType.Information);
                #endregion
            }
            catch (Exception ex)
            {
                WriteEntryLog("SaveResultTo202And205()\n" + ex.ToString(), EventLogEntryType.Error);
            }
        }
        private string ConvertString(object obj, string dfval)
        {
            if (obj != null)
                dfval = obj.ToString();
            return dfval;
        }
        public bool IsDateTime(string source)
        {
            return Regex.IsMatch(source, @"^(?ni:(?=\d)((?'year'((1[6-9])|([2-9]\d))\d\d)(?'sep'[/.-])(?'month'0?[1-9]|1[012])\2(?'day'((?<!(\2((0?[2469])|11)\2))31)|(?<!\2(0?2)\2)(29|30)|((?<=((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|(16|[2468][048]|[3579][26])00)\2\3\2)29)|((0?[1-9])|(1\d)|(2[0-8])))(?:(?=\x20\d)\x20|$))?((?<time>((0?[1-9]|1[012])(:[0-5]\d){0,2}(\x20[AP]M))|([01]\d|2[0-3])(:[0-5]\d){1,2}))?)$");
        }
        #endregion
        #endregion

        #region 统计数据处理 T_21_101 目前按照:通道,分机,坐席 处理，统计分UTC以及Local

        /// <summary>
        /// 处理AR数据
        /// </summary>
        /// <param name="ARentToken">记录所属租户</param>
        /// <param name="AListRecordID">转换成功的记录</param>
        /// <param name="LDtARData">选择的待转换记录</param>
        private void StaticDataManage(string ARentToken, List<string> AListRecordID, DataTable LDtARData)
        {
            if (AListRecordID.Count == 0) { return; }
            DataTable DtSuccessData = LDtARData.Clone();//处理成功的录音记录
            foreach (DataRow dr in LDtARData.Rows)
            {
                if (AListRecordID.Contains(dr["C001"].ToString()))
                    DtSuccessData.ImportRow(dr);
            }
            string LStrDynamicSQL = string.Empty;
            int fcount = DtSuccessData.Rows.Count;

            try
            {
                //查询T_21_101表的记录并保存下来
                DatabaseOperation01Return LDatabase101Return = new DatabaseOperation01Return();
                DataOperations01 LData101Operations = new DataOperations01();
                LStrDynamicSQL = string.Empty;
                if (IIntDatabaseType == 2)
                {
                    LStrDynamicSQL = string.Format("SELECT * FROM T_21_101 WHERE C006='{0}'", ARentToken);
                }
                if (IIntDatabaseType == 3)
                {
                    LStrDynamicSQL = string.Format("SELECT * FROM T_21_101 WHERE C006='{0}'", ARentToken);
                }
                if (string.IsNullOrEmpty(LStrDynamicSQL)) { return; }
                LDatabase101Return = LData101Operations.SelectDataByDynamicSQL(IIntDatabaseType, IStrDatabaseProfile, LStrDynamicSQL);
                if (!LDatabase101Return.BoolReturn)
                {
                    WriteEntryLog("StaticDataManage()\n" + LStrDynamicSQL + "\n" + LDatabase101Return.StrReturn, EventLogEntryType.Error);
                    return;
                }
                DataTable DT101 = LDatabase101Return.DataSetReturn.Tables[0];
                List<CStaticDataClass> LstCDBExit = new List<CStaticDataClass>();
                List<CStaticDataClass> LstDataForDB = new List<CStaticDataClass>();
                #region 处理21_101数据到StaticDataClass
                foreach (DataRow d in DT101.Rows)
                {
                    CStaticDataClass s = new CStaticDataClass();
                    s.C001 = Convert.ToInt32(d["C001"].ToString());
                    s.C002 = Convert.ToInt64(d["C002"].ToString());
                    s.C003 = d["C003"].ToString();
                    s.C004 = d["C004"].ToString();
                    s.C005 = Convert.ToInt32(d["C005"].ToString());
                    s.C006 = d["C006"].ToString();
                    s.C007 = d["C007"].ToString();
                    LstCDBExit.Add(s);
                }
                #endregion
                foreach (DataRow dr in DtSuccessData.Rows)
                {
                    DateTime dtLocal = Convert.ToDateTime(dr["C004"].ToString());
                    DateTime dtUTC = Convert.ToDateTime(dr["C005"].ToString());
                    string strScreen = dr["C014"].ToString()=="1" ? "1" : "2";//1:录音，2：录屏，3：isa录屏 对应21_101的C007
                    string strColumnName = "";
                    #region 通道/本地时间
                    CStaticDataClass tmpLChanel = new CStaticDataClass();
                    tmpLChanel.C001 = 1;//通道
                    tmpLChanel.C002 = Convert.ToInt64(dtLocal.ToString("yyyyMMdd"));
                    tmpLChanel.C003 = "L";//本地时间
                    tmpLChanel.C004 = dr["C020"].ToString() + (char)27 + dr["C038"].ToString();
                    tmpLChanel.C005 = 1;
                    tmpLChanel.C006 = ARentToken;
                    tmpLChanel.C007 = strScreen;
                    strColumnName = GetRecordTimeSpan(dtLocal.ToString("HHmmss"));
                    if (IsExitThisStatic(LstCDBExit, tmpLChanel))//数据库已经存在 该通道，当天已经统计过，存在记录 修改数值+1
                    {
                        CStaticDataClass exitStatic = LstCDBExit.Where(p => p.C001 == tmpLChanel.C001
                            && p.C002 == tmpLChanel.C002 && p.C003 == tmpLChanel.C003 && p.C004 == tmpLChanel.C004
                            && p.C007 == tmpLChanel.C007).FirstOrDefault();
                        exitStatic.ColumnValue += 1;//+1
                        exitStatic.ColumnName = strColumnName;
                        exitStatic.ISADD = "U";
                    }
                    else//数据库不存在 肯定是插入
                    {
                        if (!IsExitThisStatic(LstDataForDB, tmpLChanel))//数据库不存在并且 本次DtSuccessData也未添加过
                        {
                            tmpLChanel.ISADD = "I";//添加插入标识
                            tmpLChanel.ColumnValue = 1;
                            tmpLChanel.ColumnName = strColumnName;
                            LstDataForDB.Add(tmpLChanel);
                        }
                        else//本次DtSuccessData添加过  则更新
                        {
                            tmpLChanel.ISADD = "U";
                            tmpLChanel.ColumnValue += 1;
                            tmpLChanel.ColumnName = strColumnName;
                            LstDataForDB.Add(tmpLChanel);
                        }
                    }
                    #endregion
                    #region 通道/UTC时间
                    CStaticDataClass tmpUChanel = new CStaticDataClass();
                    tmpUChanel.C001 = 1;//通道
                    tmpUChanel.C002 = Convert.ToInt64(dtUTC.ToString("yyyyMMdd"));
                    tmpUChanel.C003 = "U";//UTC时间
                    tmpUChanel.C004 = dr["C020"].ToString() + (char)27 + dr["C038"].ToString();
                    tmpUChanel.C005 = 1;
                    tmpUChanel.C006 = ARentToken;
                    tmpUChanel.C007 = strScreen;
                    strColumnName = GetRecordTimeSpan(dtUTC.ToString("HHmmss"));
                    if (IsExitThisStatic(LstCDBExit, tmpUChanel))//已经统计过一次 该通道，当天已经统计过，存在记录 修改数值+1
                    {
                        CStaticDataClass exitStatic = LstCDBExit.Where(p => p.C001 == tmpUChanel.C001
                            && p.C002 == tmpUChanel.C002 && p.C003 == tmpUChanel.C003 && p.C004 == tmpUChanel.C004
                            && p.C007 == tmpUChanel.C007).FirstOrDefault();
                        exitStatic.ColumnValue += 1;//+1 多次存在的基础上添加
                        exitStatic.ColumnName = strColumnName;
                        exitStatic.ISADD = "U";
                    }
                    else//数据库不存在 肯定是插入
                    {
                        if (!IsExitThisStatic(LstDataForDB, tmpUChanel))//数据库不存在并且 本次DtSuccessData也未添加过
                        {
                            tmpUChanel.ISADD = "I";//添加插入标识
                            tmpUChanel.ColumnValue = 1;
                            tmpUChanel.ColumnName = strColumnName;
                            LstDataForDB.Add(tmpUChanel);
                        }
                        else//本次DtSuccessData添加过  则更新
                        {
                            tmpUChanel.ISADD = "U";
                            tmpUChanel.ColumnValue += 1;
                            tmpUChanel.ColumnName = strColumnName;
                            LstDataForDB.Add(tmpUChanel);
                        }
                    }
                    #endregion

                    #region 分机/本地时间
                    CStaticDataClass tmpLExt = new CStaticDataClass();
                    tmpLExt.C001 = 2;//分机
                    tmpLExt.C002 = Convert.ToInt64(dtLocal.ToString("yyyyMMdd"));
                    tmpLExt.C003 = "L";//UTC时间
                    tmpLExt.C004 = dr["C020"].ToString() + (char)27 + dr["C042"].ToString();
                    tmpLExt.C005 = 1;
                    tmpLExt.C006 = ARentToken;
                    tmpLExt.C007 = strScreen;
                    strColumnName = GetRecordTimeSpan(dtLocal.ToString("HHmmss"));
                    if (IsExitThisStatic(LstCDBExit, tmpLExt))//已经统计过一次 该通道，当天已经统计过，存在记录 修改数值+1
                    {
                        CStaticDataClass exitStatic = LstCDBExit.Where(p => p.C001 == tmpLExt.C001
                            && p.C002 == tmpLExt.C002 && p.C003 == tmpLExt.C003 && p.C004 == tmpLExt.C004
                            && p.C007 == tmpLExt.C007).FirstOrDefault();
                        exitStatic.ColumnValue += 1;//+1
                        exitStatic.ColumnName = strColumnName;
                        exitStatic.ISADD = "U";
                    }
                    else//数据库不存在 肯定是插入
                    {
                        if (!IsExitThisStatic(LstDataForDB, tmpLExt))//数据库不存在并且 本次DtSuccessData也未添加过
                        {
                            tmpLExt.ISADD = "I";//添加插入标识
                            tmpLExt.ColumnValue = 1;
                            tmpLExt.ColumnName = strColumnName;
                            LstDataForDB.Add(tmpLExt);
                        }
                        else//本次DtSuccessData添加过  则更新
                        {
                            tmpLExt.ISADD = "U";
                            tmpLExt.ColumnValue += 1;
                            tmpLExt.ColumnName = strColumnName;
                            LstDataForDB.Add(tmpLExt);
                        }
                    }
                    #endregion
                    #region 分机/UTC时间
                    CStaticDataClass tmpUExt = new CStaticDataClass();
                    tmpUExt.C001 = 2;//分机
                    tmpUExt.C002 = Convert.ToInt64(dtUTC.ToString("yyyyMMdd"));
                    tmpUExt.C003 = "U";//UTC时间
                    tmpUExt.C004 = dr["C020"].ToString() + (char)27 + dr["C042"].ToString();
                    tmpUExt.C005 = 1;
                    tmpUExt.C006 = ARentToken;
                    tmpUExt.C007 = strScreen;
                    strColumnName = GetRecordTimeSpan(dtUTC.ToString("HHmmss"));
                    if (IsExitThisStatic(LstCDBExit, tmpUExt))//已经统计过一次 该通道，当天已经统计过，存在记录 修改数值+1
                    {
                        CStaticDataClass exitStatic = LstCDBExit.Where(p => p.C001 == tmpUExt.C001
                            && p.C002 == tmpUExt.C002 && p.C003 == tmpUExt.C003 && p.C004 == tmpUExt.C004
                            && p.C007 == tmpUExt.C007).FirstOrDefault();
                        exitStatic.ColumnValue += 1;//+1
                        exitStatic.ColumnName = strColumnName;
                        exitStatic.ISADD = "U";
                    }
                    else//数据库不存在 肯定是插入
                    {
                        if (!IsExitThisStatic(LstDataForDB, tmpUExt))//数据库不存在并且 本次DtSuccessData也未添加过
                        {
                            tmpUExt.ISADD = "I";//添加插入标识
                            tmpUExt.ColumnValue = 1;
                            tmpUExt.ColumnName = strColumnName;
                            LstDataForDB.Add(tmpUExt);
                        }
                        else//本次DtSuccessData添加过  则更新
                        {
                            tmpUExt.ISADD = "U";
                            tmpUExt.ColumnValue += 1;
                            tmpUExt.ColumnName = strColumnName;
                            LstDataForDB.Add(tmpUExt);
                        }
                    }
                    #endregion

                    #region 座席/本地时间
                    CStaticDataClass tmpLAgt = new CStaticDataClass();
                    tmpLAgt.C001 = 3;//坐席
                    tmpLAgt.C002 = Convert.ToInt64(dtLocal.ToString("yyyyMMdd"));
                    tmpLAgt.C003 = "L";//UTC时间
                    tmpLAgt.C004 = dr["C039"].ToString();
                    tmpLAgt.C005 = 1;
                    tmpLAgt.C006 = ARentToken;
                    tmpLAgt.C007 = strScreen;
                    strColumnName = GetRecordTimeSpan(dtLocal.ToString("HHmmss"));
                    if (IsExitThisStatic(LstCDBExit, tmpLAgt))//已经统计过一次 该通道，当天已经统计过，存在记录 修改数值+1
                    {
                        CStaticDataClass exitStatic = LstCDBExit.Where(p => p.C001 == tmpLAgt.C001
                            && p.C002 == tmpLAgt.C002 && p.C003 == tmpLAgt.C003 && p.C004 == tmpLAgt.C004
                            && p.C007 == tmpLAgt.C007).FirstOrDefault();
                        exitStatic.ColumnValue += 1;//+1
                        exitStatic.ColumnName = strColumnName;
                        exitStatic.ISADD = "U";
                    }
                    else//当天的第一次统计
                    {
                        if (!IsExitThisStatic(LstDataForDB, tmpLAgt))//数据库不存在并且 本次DtSuccessData也未添加过
                        {
                            tmpLAgt.ISADD = "I";//添加插入标识
                            tmpLAgt.ColumnValue = 1;
                            tmpLAgt.ColumnName = strColumnName;
                            LstDataForDB.Add(tmpLAgt);
                        }
                        else//本次DtSuccessData添加过  则更新
                        {
                            tmpLAgt.ISADD = "U";
                            tmpLAgt.ColumnValue += 1;
                            tmpLAgt.ColumnName = strColumnName;
                            LstDataForDB.Add(tmpLAgt);
                        }
                    }
                    #endregion
                    #region 座席/UTC时间
                    CStaticDataClass tmpUAgt = new CStaticDataClass();
                    tmpUAgt.C001 = 3;//座席
                    tmpUAgt.C002 = Convert.ToInt64(dtUTC.ToString("yyyyMMdd"));
                    tmpUAgt.C003 = "U";//UTC时间
                    tmpUAgt.C004 = dr["C039"].ToString();
                    tmpUAgt.C005 = 1;
                    tmpUAgt.C006 = ARentToken;
                    tmpUAgt.C007 = strScreen;
                    strColumnName = GetRecordTimeSpan(dtUTC.ToString("HHmmss"));
                    if (IsExitThisStatic(LstCDBExit, tmpUAgt))//已经统计过一次 该通道，当天已经统计过，存在记录 修改数值+1
                    {
                        CStaticDataClass exitStatic = LstCDBExit.Where(p => p.C001 == tmpUAgt.C001
                            && p.C002 == tmpUAgt.C002 && p.C003 == tmpUAgt.C003 && p.C004 == tmpUAgt.C004
                            && p.C007 == tmpUAgt.C007).FirstOrDefault();
                        exitStatic.ColumnValue += 1;//+1
                        exitStatic.ColumnName = strColumnName;
                        exitStatic.ISADD = "U";
                    }
                    else//当天的第一次统计
                    {
                        if (!IsExitThisStatic(LstDataForDB, tmpUAgt))//数据库不存在并且 本次DtSuccessData也未添加过
                        {
                            tmpUAgt.ISADD = "I";//添加插入标识
                            tmpUAgt.ColumnValue = 1;
                            tmpUAgt.ColumnName = strColumnName;
                            LstDataForDB.Add(tmpUAgt);
                        }
                        else//本次DtSuccessData添加过  则更新
                        {
                            tmpUAgt.ISADD = "U";
                            tmpUAgt.ColumnValue += 1;
                            tmpUAgt.ColumnName = strColumnName;
                            LstDataForDB.Add(tmpUAgt);
                        }
                    }
                    #endregion
                }
                #region 处理统计记录写入数据库
                List<string> LStrInsertSQL = new List<string>();
                List<string> LStrUpdateSQL = new List<string>();
                if (LstDataForDB.Count > 0)//
                {
                    foreach (CStaticDataClass sdc in LstDataForDB)
                    {
                        string strSQL = string.Empty;
                        if (sdc.ISADD == "I")//插入
                        {
                            strSQL = string.Format("INSERT INTO T_21_101(C001,C002,C003,C004,C005,C006,{6},C007) VALUES({0},{1},'{2}','{3}',{4},'{5}',{7},'{8}')",
                                sdc.C001, sdc.C002, sdc.C003, sdc.C004, sdc.C005, sdc.C006, sdc.ColumnName, sdc.ColumnValue, sdc.C007);
                            LStrInsertSQL.Add(strSQL);
                        }
                        else if (sdc.ISADD == "U")//更新  没有值的不处理
                        {
                            strSQL = string.Format("UPDATE T_21_101 SET {0}={0}+{1} WHERE C001={2} AND C002={3} AND C003='{4}' AND C004='{5}' AND C006='{6}' AND C007='{7}'",
                                sdc.ColumnName, sdc.ColumnValue, sdc.C001, sdc.C002, sdc.C003, sdc.C004, sdc.C006, sdc.C007);
                            LStrUpdateSQL.Add(strSQL);
                        }
                    }
                }
                if (LstCDBExit.Count > 0)//
                {
                    foreach (CStaticDataClass sdc in LstCDBExit)
                    {
                        string strSQL = string.Empty;
                        if (sdc.ISADD == "U")//更新  没有值的不处理
                        {
                            strSQL = string.Format("UPDATE T_21_101 SET {0}={0}+{1} WHERE C001={2} AND C002={3} AND C003='{4}' AND C004='{5}' AND C006='{6}' AND C007='{7}'",
                                sdc.ColumnName, sdc.ColumnValue, sdc.C001, sdc.C002, sdc.C003, sdc.C004, sdc.C006,sdc.C007);
                            LStrUpdateSQL.Add(strSQL);
                        }
                    }
                }
                if (LStrInsertSQL.Count > 0)
                    ExecuteDynamicSQL(LStrInsertSQL);
                if (LStrUpdateSQL.Count > 0)
                    ExecuteDynamicSQL(LStrUpdateSQL);
	            #endregion
            }
            catch (Exception ex)
            {
                WriteEntryLog("StaticDataManage()\n" + ex.ToString(), EventLogEntryType.Error);
            }
        }

        /// <summary>
        /// 判断是否统计过
        /// </summary>
        /// <param name="LstC101Data"></param>
        /// <param name="tempData"></param>
        /// <returns></returns>
        private bool IsExitThisStatic(List<CStaticDataClass> LstC101Data, CStaticDataClass tempData)
        { 
            bool ret = false;
            if (LstC101Data.Where(p => p.C001 == tempData.C001 && p.C002 == tempData.C002 && p.C003 == tempData.C003
                && p.C004 == tempData.C004 && p.C006 == tempData.C006 && p.C007 == tempData.C007).ToList().Count > 0)
            {
                ret = true;
            }
            return ret;
        }

        /// <summary>
        /// 根据时间确定列名 08:17:23 - 081723 - C0815
        /// </summary>
        /// <param name="time"></param>
        private string GetRecordTimeSpan(string strTime)
        {
            string strResult = "C";
            string strH = strTime.Substring(0, 2);//小时 08
            string strM = strTime.Substring(2, 2);//分钟 17
            int IMinute = Convert.ToInt32(strM);

            strResult += strH;// C08
            if (IMinute <= 4)
                strResult += "00";
            else if (IMinute <= 9)
                strResult += "05";
            else if (IMinute <= 14)
                strResult += "10";
            else if (IMinute <= 19)
                strResult += "15";
            else if (IMinute <= 24)
                strResult += "20";
            else if (IMinute <= 29)
                strResult += "25";
            else if (IMinute <= 34)
                strResult += "30";
            else if (IMinute <= 39)
                strResult += "35";
            else if (IMinute <= 44)
                strResult += "40";
            else if (IMinute <= 49)
                strResult += "45";
            else if (IMinute <= 54)
                strResult += "50";
            else if (IMinute <= 59)
                strResult += "55";
            return strResult;
        }
        #endregion

        //写成文本日志  20160812
        #region LogOperator

        private void CreateFileLog()
        {
            try
            {
                string path = GetLogPath();
                mLogOperator = new LogOperator();
                mLogOperator.LogPath = path;

                mLogOperator.Start();
                string strInfo = string.Empty;
                strInfo += string.Format("LogPath:{0}\r\n", path);
                strInfo += string.Format("\tExePath:{0}\r\n", AppDomain.CurrentDomain.BaseDirectory);
                strInfo += string.Format("\tName:{0}\r\n", AppDomain.CurrentDomain.FriendlyName);
                strInfo += string.Format("\tVersion:{0}\r\n", Assembly.GetExecutingAssembly().GetName().Version);
                strInfo += string.Format("\tHost:{0}\r\n", Environment.MachineName);
                strInfo += string.Format("\tAccount:{0}", Environment.UserName);
                WriteLog(LogMode.Info, string.Format("{0}", strInfo));
            }
            catch (Exception ex)
            {
                WriteLog(LogMode.Error, string.Format("CreateFileLog fail.\t{0}", ex.Message));
            }
        }

        public static void WriteLog(LogMode mode, string category, string msg)
        {
            if (mLogOperator != null)
            {
                mLogOperator.WriteLog(mode, category, msg);
            }
        }

        public static void WriteLog(string category, string msg)
        {
            WriteLog(LogMode.Info, category, msg);
        }

        public static void WriteLog(LogMode mode, string msg)
        {
            WriteLog(mode, "UMPService02", msg);
        }

        public static void WriteLog(string msg)
        {
            WriteLog(LogMode.Info, msg);
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
                        string.Format("UMP\\{0}\\Logs", "UMPService02"));
                }
                else
                {
                    strReturn = Path.Combine(strReturn, "UMPService02");
                }
                //创建日志文件夹
                if (!Directory.Exists(strReturn))
                {
                    Directory.CreateDirectory(strReturn);
                }
            }
            catch { }
            return strReturn;
        }

        private void SetLogMode()
        {
            try
            {
                //if (mConfigInfo == null) { return; }
                //var setting = mConfigInfo.ListSettings.FirstOrDefault(s => s.Key == ConstValue.GS_KEY_LOG_MODE);
                //if (setting == null) { return; }
                //string strValue = setting.Value;
                //int intValue;
                //if (int.TryParse(strValue, out intValue)
                //    && intValue > 0)
                //{
                //    if (mLogOperator != null)
                //    {
                //        mLogOperator.LogMode = (LogMode)intValue;
                //        OnDebug(LogMode.Info, string.Format("LogMode changed.\t{0}", (LogMode)intValue));
                //    }
                //}
            }
            catch (Exception ex)
            {
                WriteLog(LogMode.Error, string.Format("SetLogMode fail.\t{0}", ex.Message));
            }
        }

        #endregion

    }

    public class OperationDataArgs
    {
        bool LBoolValue = true;
        string LStrValue = string.Empty;
        DataSet LDataSetValue = new DataSet();
        List<string> LListStrValue = new List<string>();
        List<DataSet> LListDataSetValue = new List<DataSet>();


        
        public bool BoolReturn
        {
            get { return LBoolValue; }
            set { LBoolValue = value; }
        }

        public string StringReturn
        {
            get { return LStrValue; }
            set { LStrValue = value; }
        }

        public DataSet DataSetReturn
        {
            get { return LDataSetValue; }
            set { LDataSetValue = value; }
        }

        public List<string> ListStringReturn
        {
            get { return LListStrValue; }
            set { LListStrValue = value; }
        }

        public List<DataSet> ListDataSetReturn
        {
            get { return LListDataSetValue; }
            set { LListDataSetValue = value; }
        }


    }
}
