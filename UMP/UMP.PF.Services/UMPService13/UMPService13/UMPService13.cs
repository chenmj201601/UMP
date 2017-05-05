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

namespace UMPService13
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant, InstanceContextMode = InstanceContextMode.Single)]
    public partial class UMPService13 : ServiceBase
    {
        public static string IStrEventLogSource = "Service 13";
        public static string IStrApplicationName = "UMP";
        public static string IStrSpliterChar = string.Empty;

        private static LogOperator mLogOperator;

        //private int IIntThisServicePort = 0;
        //private int IIntSiteHttpBindingPort = 0;
        private string IStrSiteBaseDirectory = string.Empty;

        private static EventLog IEventLog = null;

        private bool IBoolCanContinue = true;


        private Thread IThreadDelDataMain;
        private Thread IThreadReadDBProfile;

        private int IIntDatabaseType = 0;
        private string IStrDatabaseProfile = string.Empty;

        private string IStrVerificationCode102 = string.Empty;
        private string IStrVerificationCode002 = string.Empty;
        private List<CCustomField> ILstCustomFieldConfig = new List<CCustomField>();
        
        #region 初始化
        public UMPService13()
        {
            InitializeComponent();
        }
        #endregion

        #region DEBUG 服务的启动与停止

        public void Start()
        {
            try
            {
                if (Program.IsConsole)
                {
                    CreateFileLog();
                }
                UMPService13 Serverce13 = new UMPService13();
                OnDebug(LogMode.Info, string.Format("Server13 starting..."));
                Serverce13.ReadDatabaseConnectionProfile();
                Serverce13.DelDataMain();

            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("Server13 start fail.\t{0}", ex.Message));
            }
        }

        #endregion

        #region 服务的启动与停止

        protected override void OnStart(string[] args)
        {
            CreateFileLog();
            WriteLog(LogMode.Info, string.Format("Service13 starting..."));
            StartService13();

        }

        protected override void OnStop()
        {
            try
            {

                Thread.Sleep(500);
                StopReadDBProfileThread();
                StopDelDataMainThread();

                WriteLog(LogMode.Info, string.Format("Service13 stopped"));
                if (mLogOperator != null)
                {
                    mLogOperator.Stop();
                    mLogOperator = null;
                }
            }
            catch (Exception ex)
            {
                WriteEntryLog("Stop Service13 Exception:\n" + ex.ToString(), EventLogEntryType.Error);
            }
        }
        #endregion

        #region 停止读数据库文件线程
        private void StopReadDBProfileThread()
        {
            try
            {
                if (IThreadReadDBProfile != null)
                {
                    try
                    {
                        IThreadReadDBProfile.Abort();
                    }
                    catch { }
                    IThreadReadDBProfile = null;
                    OnDebug(LogMode.Info, string.Format("ReadDBProfileThread stopped"));
                }
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("StopReadDBProfileThread fail.\t{0}", ex.Message));
            }
        }
        #endregion

        #region 停止数据回删线程
        private void StopDelDataMainThread()
        {
            try
            {
                if (IThreadDelDataMain != null)
                {
                    try
                    {
                        IThreadDelDataMain.Abort();
                    }
                    catch { }
                    IThreadDelDataMain = null;
                    OnDebug(LogMode.Info, string.Format("DelDataMainThread stopped"));
                }
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("StopDelDataMainThread fail.\t{0}", ex.Message));
            }
        }
        #endregion       

        #region 启动 Service 13
        public void StartService13()
        {
            try
            {
                if (!EventLog.SourceExists(IStrEventLogSource)) { EventLog.CreateEventSource(IStrEventLogSource, IStrApplicationName); }
                UMPServiceLog.Source = IStrEventLogSource;
                UMPServiceLog.Log = IStrApplicationName;
                UMPServiceLog.ModifyOverflowPolicy(OverflowAction.OverwriteOlder, 3);
                IEventLog = UMPServiceLog;

                WriteEntryLog("Service13 Started At : " + DateTime.Now.ToString("G"), EventLogEntryType.Information);
                
                //启动线程
                
                //IStrSiteBaseDirectory = GetSiteBaseDirectory();
                //IIntSiteHttpBindingPort = GetUMPPFBasicHttpPort();
                //IIntThisServicePort = IIntSiteHttpBindingPort - 2;

                //IThreadReadDBProfile = new Thread(new ThreadStart(ReadDatabaseConnectionProfile));
                //IThreadReadDBProfile.Start();

                IThreadDelDataMain = new Thread(new ThreadStart(DelDataMain));
                IThreadDelDataMain.Start();

            }
            catch (Exception ex)
            {
                WriteEntryLog("StartService13()\n" + ex.ToString(), EventLogEntryType.Error);
            }
        }
        #endregion

        #region 回删主程序
        private void DelDataMain()
        { 
          try
          {
                    ReadDatabaseConnectionProfile();//读数据库文件信息

                    WriteEntryLog("Service13 Started Exec DelDataMain At : " + DateTime.Now.ToString("G"), EventLogEntryType.Information);
                    DelDataManage("00000"); //只考虑00000的情况
                    Thread.Sleep(5000);


            }
          catch (Exception ex)
          {
              WriteEntryLog("DelDataMain()\n" + ex.ToString(), EventLogEntryType.Error);
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
                        WriteEntryLog("ExecuteDynamicSQL()\n" + AListStrSQLString[LIntLoopDynamic] + "\n" + ex.ToString(), EventLogEntryType.Error);
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
                WriteEntryLog("ExecuteDynamicSQL() \n" + ex.ToString(), EventLogEntryType.Error);
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

        #region 处理回删数据  参数租户代码

        private void DelDataManage(string ARentToken)
        {
            while (true) //线程循环开始

            {WriteEntryLog("Service13 Started Go DelDataMain At : " + DateTime.Now.ToString("G"), EventLogEntryType.Information);
            string LStrDynamicSQL = string.Empty;

            //T_00_202
            string LStr202C003 = string.Empty;
            string LStr202C009 = string.Empty;

            try
            {
                string LStrTableName = "T_21_001";
                
                string structnow = DateTime.UtcNow.ToString("yyyyMMddHHmmss");

                DatabaseOperation01Return LDatabase202Return = new DatabaseOperation01Return();
                DataOperations01 LData202Operations = new DataOperations01();
                DatabaseOperation01Return LDatabase204Return = new DatabaseOperation01Return();
                DataOperations01 LData204Operations = new DataOperations01();

                #region //查询T_00_202中的有效策略 启用&未删除&有效时间&租户对应
                LStrDynamicSQL = string.Empty;
                if (IIntDatabaseType == 2)
                {
                    LStrDynamicSQL = string.Format("SELECT * FROM T_00_202 WHERE C005='1' AND C003 LIKE '259%' AND C006='0' AND C010<{0} AND C011>{0} AND C000='{1}'", structnow, ARentToken);
                }
                if (IIntDatabaseType == 3)
                {
                    LStrDynamicSQL = string.Format("SELECT * FROM T_00_202 WHERE C005='1' AND C003 LIKE '259%' AND C006='0' AND C010<{0} AND C011>{0} AND C000='{1}'", structnow, ARentToken);
                }
                if (string.IsNullOrEmpty(LStrDynamicSQL)) { return; }
                LDatabase202Return = LData202Operations.SelectDataByDynamicSQL(IIntDatabaseType, IStrDatabaseProfile, LStrDynamicSQL);
                if (!LDatabase202Return.BoolReturn)
                {
                    WriteEntryLog("DelDataManage()\n" + LStrDynamicSQL + "\n" + LDatabase202Return.StrReturn, EventLogEntryType.Error);
                    return;
                }
                #endregion


                #region //根据配置策略确定其数据筛选条件
                int DelRows,LoopTimes,LPRecordRows,FeachRows;
                DataRow[] FoundRows;


                foreach (DataRow LDataRowSingle202 in LDatabase202Return.DataSetReturn.Tables[0].Rows)//已经查出的数据
                {
      
                                LStr202C003 = LDataRowSingle202["C003"].ToString();
                                LStr202C009 = LDataRowSingle202["C009"].ToString();

                             
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
                                        WriteEntryLog("DelDataManage()\n" + LStrDynamicSQL + "\n" + LDatabase204Return.StrReturn, EventLogEntryType.Error);
                                        continue;
                                    }
                                    if (LDatabase204Return.DataSetReturn.Tables[0].Rows.Count == 0)
                                    {
                                        WriteEntryLog("DelDataManage()\n" + LStrDynamicSQL + "\n" + "LDatabase204ReturnCount=0", EventLogEntryType.Warning);
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

                                    #region 从回删表中取得数据写入到T_00_205

                                    //确定是否分表
                                    DatabaseOperation01Return LDatabaseReturn = new DatabaseOperation01Return();
                                    DataOperations01 LDataOperations = new DataOperations01();

                                    LStrDynamicSQL = "SELECT * FROM T_00_000 WHERE C000 = '00000' AND C003 = 'LP' AND C004='1'";
                                    if (string.IsNullOrEmpty(LStrDynamicSQL)) { continue; }
                                    LDatabaseReturn = LDataOperations.SelectDataByDynamicSQL(IIntDatabaseType, IStrDatabaseProfile, LStrDynamicSQL);                
                                    LPRecordRows = LDatabaseReturn.DataSetReturn.Tables[0].Rows.Count;                                   
                                    
                                    //不分表
                                    if (LPRecordRows == 0)
                                    {
                                        string LStrSelectRows = "1000";
                                        LStrTableName = "T_21_001_00000";
                                        string LStrSerialColumn = "C001";

                                        //先确定回删记录总量
                                        LStrDynamicSQL = "SELECT *  FROM " + LStrTableName + " WHERE  C084=0  AND C091>=8";
                                        if (string.IsNullOrEmpty(LStrDynamicSQL)) { continue; }
                                        LDatabaseReturn = LDataOperations.SelectDataByDynamicSQL(IIntDatabaseType, IStrDatabaseProfile, LStrDynamicSQL);
                                        DelRows = LDatabaseReturn.DataSetReturn.Tables[0].Rows.Count;
                                        //确定回删循环次数
                                        double d = DelRows / 1000;
                                        LoopTimes = (int)d + 1;
                                        //循环提取1000条数据
                                        LStrSelectRows = "1000";
                                        string LC001Value ="0";
                                        for (int j = 0; j < LoopTimes; j++)
                                        {
                                            if (IIntDatabaseType == 2)
                                            {
                                                LStrDynamicSQL = "SELECT TOP " + LStrSelectRows + " * FROM " + LStrTableName + " WHERE " + LStrSerialColumn + " > "+LC001Value+" AND C084=0  AND C091>=8 ORDER BY " + LStrSerialColumn + " ASC";
                                            }
                                            if (IIntDatabaseType == 3)
                                            {
                                                LStrDynamicSQL = "SELECT  * FROM " + LStrTableName + " WHERE " + LStrSerialColumn + " > " + LC001Value + " AND C084=0  AND C091>=8 AND ROWNUM<="+LStrSelectRows+" ORDER BY " + LStrSerialColumn + " ASC";
                                            }

                                            DataTable DtSuccessData = FeachDelData(LStrDynamicSQL);
                                            FeachRows = DtSuccessData.Rows.Count;//取得记录数

                                            if (FeachRows > 0)
                                            { FoundRows = DtSuccessData.Select(rowfilter);
                                              if (FoundRows.Count() > 0)
                                              {
                                                //满足筛选条件的记录大于0 写入T_00_205 同时更新T_00_202的筛选数量
                                                WriteEntryLog("Service13 Started SaveResultTo205And001 At : " + DateTime.Now.ToString("G"), EventLogEntryType.Information);
                                                SaveResultTo205And001(LStr202C003, ARentToken, FoundRows, filtertarget, isPCMCondation, LStrTableName);
                                              }

                                              LC001Value = DtSuccessData.Rows[FeachRows - 1]["C001"].ToString();
                                            
                                            } 

                                            

                                        }
                                    }
                                    
                                    else   //月份表
                                    {   //分表清单
                                        
                                        DataTable LDataTableExistLogicPartitionTables = ObtainRentExistLogicPartitionTables(ARentToken, LStrTableName);
                                        if (LDataTableExistLogicPartitionTables == null) { continue; }
                                        
                                        foreach (DataRow LSingleDataTable in LDataTableExistLogicPartitionTables.Rows)
                                        {
                                            LStrTableName = LSingleDataTable["TABLE_NAME"].ToString();
                                            //先确定回删记录总量
                                            LStrDynamicSQL = "SELECT * FROM " + LStrTableName + " WHERE  C084=0  AND C091>=8";
                                            if (string.IsNullOrEmpty(LStrDynamicSQL)) { continue; }
                                            LDatabaseReturn = LDataOperations.SelectDataByDynamicSQL(IIntDatabaseType, IStrDatabaseProfile, LStrDynamicSQL);
                                            DelRows = LDatabaseReturn.DataSetReturn.Tables[0].Rows.Count;
                                            //确定回删循环次数
                                            double d = DelRows / 1000;
                                            LoopTimes = (int)d + 1;
                                            //循环提取1000条数据
                                            string LStrSelectRows = "1000";
                                            string LC001Value = "0";
                                            string LStrSerialColumn = "C001";
                                            for (int j = 0; j < LoopTimes; j++)
                                            {
                                                if (IIntDatabaseType == 2)
                                                {
                                                    LStrDynamicSQL = "SELECT TOP " + LStrSelectRows + " * FROM " + LStrTableName + " WHERE " + LStrSerialColumn + " > " + LC001Value + " AND C084=0  AND C091>=8 ORDER BY " + LStrSerialColumn + " ASC";
                                                }
                                                if (IIntDatabaseType == 3)
                                                {
                                                    LStrDynamicSQL = "SELECT  * FROM " + LStrTableName + " WHERE " + LStrSerialColumn + " > " + LC001Value + " AND C084=0  AND C091>=8  AND ROWNUM<="+LStrSelectRows+" ORDER BY " + LStrSerialColumn + " ASC";
                                                }
                                                
                                                DataTable DtSuccessData = FeachDelData(LStrDynamicSQL);
                                                FeachRows = DtSuccessData.Rows.Count;//取得记录数

                                                FoundRows = DtSuccessData.Select(rowfilter);
                                                if (FoundRows.Count() > 0)
                                                {
                                                    //满足筛选条件的记录大于0 写入T_00_205 同时更新T_00_202的筛选数量
                                                    WriteEntryLog("Service13 Started SaveResultTo205And001 At : " + DateTime.Now.ToString("G"), EventLogEntryType.Information);
                                                    SaveResultTo205And001(LStr202C003, ARentToken, FoundRows, filtertarget, isPCMCondation, LStrTableName);
                                                    WriteEntryLog("Service13 End SaveResultTo205And001 At : " + DateTime.Now.ToString("G"), EventLogEntryType.Information);
                                                }
                                                if (FeachRows - 1<0) { continue; }
                                                LC001Value = DtSuccessData.Rows[FeachRows - 1]["C001"].ToString();

                                            }

                                        }

                                    }
                                    #endregion



                }

                #endregion



            }
            catch (Exception ex)
            {
                WriteEntryLog("DelDataManage()\n" + ex.ToString(), EventLogEntryType.Error);
            }
         Thread.Sleep(1000);
         }//线程循环
        }
        #endregion

        #region 从回删表T_21_001_00000或T_21_001_00000_1601中取出1000条数据进行处理
        private DataTable FeachDelData(string LStrDynamicSQL)
        {
            DataTable LDataTableReturn = new DataTable();
            try
            {
                DatabaseOperation01Return LDatabaseOperationReturn = new DatabaseOperation01Return();
                DataOperations01 LDataOperations = new DataOperations01();
                LDatabaseOperationReturn = LDataOperations.SelectDataByDynamicSQL(IIntDatabaseType, IStrDatabaseProfile, LStrDynamicSQL);
                LDataTableReturn = LDatabaseOperationReturn.DataSetReturn.Tables[0];

                if (!LDatabaseOperationReturn.BoolReturn)
                {
                    LDataTableReturn = null;
                    WriteEntryLog("ObtainRentList()\n" + LDatabaseOperationReturn.StrReturn, EventLogEntryType.Error);
                }
                else
                {
                    LDataTableReturn = LDatabaseOperationReturn.DataSetReturn.Tables[0];
                }
                
            }
            catch (Exception ex)
            {
                WriteEntryLog("FeachDelData()\n" + ex.ToString(), EventLogEntryType.Error);
            }
            return LDataTableReturn;
        }

        #endregion

        #region 更新筛选结果到T_00_205，回写T_21_001
        /// <summary>
        /// 更新筛选结果到T_00_202，数据写入T_00_205，回写T_21_001的C084
        /// <param name="LStr202C003">筛选策略编码</param>
        /// <param name="ARentToken">租户</param>
        /// <param name="FoundRows">符合筛选回删的记录</param>
        /// <param name="filtertarget">筛选条件</param>
        /// <param name="DataFromTableName">筛选记录所在的表</param>
        private void SaveResultTo205And001(string LStr202C003, string ARentToken, DataRow[] FoundRows, string filtertarget, bool isPCMCondation, string DataFromTableName)
        {
            try
            {

                #region insert2205 update21001
     
                string strAllRecord = "";
                foreach (DataRow dr in FoundRows)
                {
                    List<string> LStrInsertSQL = new List<string>();
                    string temp005 = "";
                    string temp006 = "";
                    string temp007 = "";
                    if (isPCMCondation)//PCM条件
                    {
                        temp005 = ConvertString(dr["C078"], "");
                        temp006 = ConvertString(dr["C079"], "");
                        temp007 = ConvertString(dr["C080"], "");
                        continue;//如果是PCM就不写入了
                    }
                    else
                    {
                        temp005 = ConvertString(dr["C035"], "");
                        temp006 = ConvertString(dr["C036"], "");
                    }
                    //INSERT INTO T_00_205(C001, C002, C003, C004, C011, C012,C008) VALUES(1,2,3,4,5,'00000','192.168.4.1')

                    if (IIntDatabaseType == 2)
                    {
                        string strutc = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
                        string sql = string.Format("INSERT INTO T_00_205(C001,C002,C003,C004,C005,C006,C007,C011,C012,C008,C013) VALUES({0},{1},{2},{3},'{4}','{5}','{6}',{7},'{8}','{9}','{10}')",
                            LStr202C003,
                            '3',
                            ConvertString(dr["C001"], "-1"),
                            ConvertString(dr["C002"], "-1"),
                            temp005,
                            temp006,
                            temp007,
                            strutc,
                            ARentToken,
                            ConvertString(dr["C020"], "127.0.0.1"), DataFromTableName);
                        strAllRecord += ConvertString(dr["C002"], "-1") + ",";
                        sql = sql + ";";
                        LStrInsertSQL.Add(sql);
                        //ExecuteDynamicSQL(LStrInsertSQL);

                        //update t_21_001_00000 c084
                        sql = string.Format("UPDATE  " + DataFromTableName + " SET C084={0} WHERE C002={1}",
                            LStr202C003,
                            ConvertString(dr["C002"], "-1"));
                        LStrInsertSQL.Add(sql);
                        ExecuteDynamicSQL(LStrInsertSQL);
                    }
                    if (IIntDatabaseType == 3)
                    {

                        string strutc = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
                        string sql = string.Format("INSERT INTO T_00_205(C001,C002,C003,C004,C005,C006,C007,C011,C012,C008,C013) VALUES({0},{1},{2},{3},'{4}','{5}','{6}',{7},'{8}','{9}','{10}')",
                            LStr202C003,
                            '3',
                            ConvertString(dr["C001"], "-1"),
                            ConvertString(dr["C002"], "-1"),
                            temp005,
                            temp006,
                            temp007,
                            strutc,
                            ARentToken,
                            ConvertString(dr["C020"], "127.0.0.1"), DataFromTableName);
                        strAllRecord += ConvertString(dr["C002"], "-1") + ",";
                        LStrInsertSQL.Add(sql);
                      //  ExecuteDynamicSQL(LStrInsertSQL);

                        //update t_21_001_00000 c084
                      //  LStrInsertSQL = new List<string>();
                        sql = string.Format("UPDATE  " + DataFromTableName + " SET C084={0} WHERE C002={1}",
                            LStr202C003,
                            ConvertString(dr["C002"], "-1"));
                        LStrInsertSQL.Add(sql);
                        ExecuteDynamicSQL(LStrInsertSQL);

                    }

                }

                WriteEntryLog("DelSuccessData:\n" + strAllRecord, EventLogEntryType.Information);
                #endregion

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
                    WriteEntryLog("Upd202Fail:\n", EventLogEntryType.Information);
                    return;
                }
                WriteEntryLog("Upd202Success:\n", EventLogEntryType.Information);
                #endregion
            }
            catch (Exception ex)
            {
                WriteEntryLog("SaveResultTo205And001()\n" + ex.ToString(), EventLogEntryType.Error);
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
        
        #region LogOperator 写成文本日志  20160812

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
            WriteLog(mode, "UMPService13", msg);
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
                        string.Format("UMP\\{0}\\Logs", "UMPService13"));
                }
                else
                {
                    strReturn = Path.Combine(strReturn, "UMPService13");
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

        #region Debug

        public event Action<LogMode, string, string> Debug;

        private void OnDebug(LogMode mode, string category, string msg)
        {
            if (Debug != null)
            {
                Debug(mode, category, msg);

                if (Program.IsConsole)
                {
                    WriteLog(mode, category, msg);
                }
            }
        }

        private void OnDebug(LogMode mode, string msg)
        {
            OnDebug(mode, "Service13", msg);
        }

        void UmpService13_Debug(LogMode mode, string category, string msg)
        {
            WriteLog(mode, category, msg);
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