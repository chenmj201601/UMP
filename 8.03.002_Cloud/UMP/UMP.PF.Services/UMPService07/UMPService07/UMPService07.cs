using Oracle.DataAccess.Client;
using PFShareClasses01;
using PFShareClassesS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace UMPService07
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant, InstanceContextMode = InstanceContextMode.Single)]
    public partial class UMPService07 : ServiceBase
    {
        public static string IStrEventLogSource = "Service 07";
        public static string IStrApplicationName = "UMP";
        public static string IStrSpliterChar = string.Empty;

        private int IIntThisServicePort = 0;
        private string IStrSiteBaseDirectory = string.Empty;
        private int IIntSiteHttpBindingPort = 0;

        private static EventLog IEventLog = null;

        private int IIntDatabaseType = 0;
        private string IStrDatabaseProfile = string.Empty;

        private string IStrVerificationCode102 = string.Empty;
        private string IStrVerificationCode002 = string.Empty;

        private bool IBoolCanContinue = true;

        private Thread IThreadReadDBProfile;

        private Thread IThreadSynchronousExtensionData;
        private bool IBooInSynchronous = false;
        private bool IBoolCanAbortSynchronous = false;

        private Thread IThreadDeleteEncryptionData;
        private bool IBoolInDeleteEncryption = false;
        private bool IBoolCanAbortDeleteEncryption = false;

        public UMPService07()
        {
            InitializeComponent();
        }

        #region 服务的启动与停止
        protected override void OnStart(string[] args)
        {
            Thread LThreadStartService = new Thread(new ThreadStart(StartService07));
            LThreadStartService.Start();
        }

        protected override void OnStop()
        {
            try
            {
                WriteEntryLog("Stop Service 07", EventLogEntryType.Information);

                IBoolCanContinue = false;

                System.Threading.Thread.Sleep(200);

                while (IBooInSynchronous ||!IBoolCanAbortSynchronous) { System.Threading.Thread.Sleep(100); }

                while (IBoolInDeleteEncryption || !IBoolCanAbortDeleteEncryption) { System.Threading.Thread.Sleep(100); }

                try { IThreadReadDBProfile.Abort(); }
                catch { }

                try { IThreadSynchronousExtensionData.Abort(); }
                catch { }

                try { IThreadDeleteEncryptionData.Abort(); }
                catch { }

                Thread.Sleep(500);
                WriteEntryLog("Service 07 Stopped", EventLogEntryType.Information);
            }
            catch (Exception ex)
            {
                WriteEntryLog("Stop Service 07 Exception:\n" + ex.ToString(), EventLogEntryType.Error);
            }
        }
        #endregion

        #region 启动 Service 07
        private void StartService07()
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
                IIntThisServicePort = IIntSiteHttpBindingPort - 7;

                IThreadReadDBProfile = new Thread(new ThreadStart(ReadDatabaseConnectionProfile));
                IThreadReadDBProfile.Start();

                IThreadSynchronousExtensionData = new Thread(new ThreadStart(SynchronousExtensionData));
                IThreadSynchronousExtensionData.Start();

                IThreadDeleteEncryptionData = new Thread(new ThreadStart(DeleteCompleteEncryptionData));
                IThreadDeleteEncryptionData.Start();

            }
            catch(Exception ex)
            {
                WriteEntryLog("StartService07()\n" + ex.ToString(), EventLogEntryType.Error);
            }
        }
        #endregion

        #region 读取数据库连接信息
        private void ReadDatabaseConnectionProfile()
        {
            while (IIntDatabaseType == 0 || string.IsNullOrEmpty(IStrDatabaseProfile))
            {
                //WriteEntryLog("ReadDatabaseConnectionProfile() \nIIntDatabaseType = " + IIntDatabaseType.ToString() + "\nIStrDatabaseProfile = " + IStrDatabaseProfile + "\nIBoolCanContinue = " + IBoolCanContinue.ToString(), EventLogEntryType.Information);
                if (!IBoolCanContinue) { break; }
                GetDatabaseConnectionProfile();
                Thread.Sleep(1000);
            }
        }
        #endregion

        #region 同步分机信息 至 T_11_101 表
        private void SynchronousExtensionData()
        {
            try
            {
                while (IBoolCanContinue)
                {
                    if (IIntDatabaseType == 0 || string.IsNullOrEmpty(IStrDatabaseProfile)) { Thread.Sleep(2000);  IBooInSynchronous = false; continue; }

                    IBooInSynchronous = true;

                    ActionSynchronousExtensionData();
                    System.Threading.Thread.Sleep(1000);
                    IBooInSynchronous = false;
                    IBoolCanAbortSynchronous = true;
                }
                IBooInSynchronous = false;
                IBoolCanAbortSynchronous = true;
            }
            catch (Exception ex)
            {
                IBooInSynchronous = false;
                IBoolCanAbortSynchronous = true;
                WriteEntryLog("SynchronousExtensionData()\n" + ex.ToString(), EventLogEntryType.Error);
            }
            
        }

        private void ActionSynchronousExtensionData()
        {
            string LStrDynamicSQL = string.Empty;
            string LStrTemp = string.Empty;
            string LStrUserDefaultPwd = string.Empty;
            string LStrVerificationCode002 = string.Empty;
            string LStrVerificationCode102 = string.Empty;
            string LStrDataTimeNow = string.Empty;
            string LStrDataID = string.Empty;
            SqlConnection LSqlConnection = null;
            OracleConnection LOracleConnection = null;

            string LStr11101001 = string.Empty;
            string LStr11101002 = string.Empty;
            string LStr11101012 = string.Empty;
            string LStr11101013 = string.Empty;
            string LStr11101014 = string.Empty;
            string LStr11101015 = string.Empty;
            string LStr11101017 = string.Empty;

            string LStrStep = string.Empty;

            try
            {
                DatabaseOperation01Return LDatabaseOperation01Return = new DatabaseOperation01Return();
                DataOperations01 LDataOperations01 = new DataOperations01();

                LStrVerificationCode002 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M002);
                LStrVerificationCode102 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M102);

                #region 获取租户列表
                LStrDynamicSQL = "SELECT * FROM T_00_121 ORDER BY C001";
                LDatabaseOperation01Return = LDataOperations01.SelectDataByDynamicSQL(IIntDatabaseType, IStrDatabaseProfile, LStrDynamicSQL);

                //租户列表
                DataSet LDataSet00121 = LDatabaseOperation01Return.DataSetReturn;
                #endregion

                #region 获取所有录音服务器和录音、录屏通道
                List<string> LListStrServerRentInfo = new List<string>();

                LStrDynamicSQL = "SELECT * FROM T_11_101_00000 WHERE (C001 > 2210000000000000000 AND C001 < 2220000000000000000 AND C002 = 1) OR (C001 > 2310000000000000000 AND C001 < 2320000000000000000 AND C002 = 1)";
                LDatabaseOperation01Return = LDataOperations01.SelectDataByDynamicSQL(IIntDatabaseType, IStrDatabaseProfile, LStrDynamicSQL);
                //所有录音录屏服务器
                DataSet LDataSet11101AllLGServer = LDatabaseOperation01Return.DataSetReturn;
                foreach (DataRow LDataRowSingleServer in LDataSet11101AllLGServer.Tables[0].Rows)
                {
                    LStr11101001 = LDataRowSingleServer["C001"].ToString();
                    LStr11101017 = LDataRowSingleServer["C017"].ToString();
                    LStr11101017 = LStr11101017.Substring(9);
                    LStr11101017 = EncryptionAndDecryption.EncryptDecryptString(LStr11101017, LStrVerificationCode102, EncryptionAndDecryption.UMPKeyAndIVType.M102);
                    LDataRowSingleServer["C017"] = LStr11101017;
                    //服务器19位编码 + char(27) + 服务器IP + char(27) + "未被分配"
                    LListStrServerRentInfo.Add(LStr11101001 + IStrSpliterChar + LStr11101017 + IStrSpliterChar + "0");
                }

                LStrDynamicSQL = "SELECT * FROM T_11_101_00000 WHERE (C001 > 2250000000000000000 AND C001 < 2260000000000000000 AND (C002 = 1 OR C002 = 2)) OR (C001 > 2320000000000000000 AND C001 < 2330000000000000000 AND (C002 = 1 OR C002 = 2)) ORDER BY C001, C002";
                LDatabaseOperation01Return = LDataOperations01.SelectDataByDynamicSQL(IIntDatabaseType, IStrDatabaseProfile, LStrDynamicSQL);
                //所有录音录屏通道
                DataSet LDataSet11101AllLGChannel = LDatabaseOperation01Return.DataSetReturn;
                for (int LIntLoopChannel = 0; LIntLoopChannel < LDataSet11101AllLGChannel.Tables[0].Rows.Count; LIntLoopChannel++)
                {
                    //C001:通道编码 19 位
                    //C012:通道ID 0～2000
                    //C013:录音服务器IP
                    //C014:分机号
                    //C015:通道名
                    LStr11101002 = LDataSet11101AllLGChannel.Tables[0].Rows[LIntLoopChannel]["C002"].ToString();
                    if (LStr11101002 == "1")
                    {
                        LStr11101013 = LDataSet11101AllLGChannel.Tables[0].Rows[LIntLoopChannel]["C013"].ToString();
                        DataRow[] LDataRowArray = LDataSet11101AllLGServer.Tables[0].Select("C001 = " + LStr11101013);
                        LStr11101017 = LDataRowArray[0]["C017"].ToString();
                        LDataSet11101AllLGChannel.Tables[0].Rows[LIntLoopChannel]["C017"] = LStr11101017;
                    }
                    if (LStr11101002 == "2")
                    {
                        LStr11101001 = LDataSet11101AllLGChannel.Tables[0].Rows[LIntLoopChannel]["C001"].ToString();
                        LDataSet11101AllLGChannel.Tables[0].Rows[LIntLoopChannel - 1]["C014"] = LDataSet11101AllLGChannel.Tables[0].Rows[LIntLoopChannel]["C012"].ToString();
                        if (LStr11101001.Substring(0, 3) == "225")
                        {
                            LDataSet11101AllLGChannel.Tables[0].Rows[LIntLoopChannel - 1]["C015"] = LDataSet11101AllLGChannel.Tables[0].Rows[LIntLoopChannel]["C011"].ToString();
                        }
                    }
                }

                #endregion

                #region 获取租户租用的服务器
                string LStrRentBegin = string.Empty, LStrRentEnd = string.Empty;
                string LStr11201004 = string.Empty;

                List<string> LListStrRentToken = new List<string>();
                List<List<string>> LListListRentServer = new List<List<string>>();

                foreach (DataRow LDataRowSingleRent in LDataSet00121.Tables[0].Rows)
                {
                    LStrRentBegin = LDataRowSingleRent["C011"].ToString();
                    LStrRentEnd = LDataRowSingleRent["C012"].ToString();
                    LStrRentBegin = EncryptionAndDecryption.EncryptDecryptString(LStrRentBegin, LStrVerificationCode102, EncryptionAndDecryption.UMPKeyAndIVType.M102);
                    LStrRentEnd = EncryptionAndDecryption.EncryptDecryptString(LStrRentEnd, LStrVerificationCode102, EncryptionAndDecryption.UMPKeyAndIVType.M102);

                    LStrTemp = LDataRowSingleRent["C021"].ToString();
                    LStrTemp = EncryptionAndDecryption.EncryptDecryptString(LStrTemp, LStrVerificationCode102, EncryptionAndDecryption.UMPKeyAndIVType.M102);
                    LStrDataID = LDataRowSingleRent["C001"].ToString();

                    //租户Token + char(27) + 租户19位编码
                    LListStrRentToken.Add(LStrTemp + IStrSpliterChar + LStrDataID);

                    List<string> LListStrRentServerInfo = new List<string>();

                    if (DateTime.Parse(LStrRentBegin) <= DateTime.UtcNow && DateTime.Parse(LStrRentEnd) >= DateTime.UtcNow)
                    {
                        LStrDynamicSQL = "SELECT * FROM T_11_201_" + LStrTemp + " WHERE C003 = " + LStrDataID + " AND ((C004 > 2210000000000000000 AND C004 < 2220000000000000000) OR (C004 > 2310000000000000000 AND C004 < 2320000000000000000))";
                        LDatabaseOperation01Return = LDataOperations01.SelectDataByDynamicSQL(IIntDatabaseType, IStrDatabaseProfile, LStrDynamicSQL);

                        foreach (DataRow LDataRowRentSingleServer in LDatabaseOperation01Return.DataSetReturn.Tables[0].Rows)
                        {
                            LStr11201004 = LDataRowRentSingleServer["C004"].ToString();
                            DataRow[] LDataRowArray = LDataSet11101AllLGServer.Tables[0].Select("C001 = " + LStr11201004);
                            if (LDataRowArray.Length > 0)
                            {
                                LStr11101017 = LDataRowArray[0]["C017"].ToString();
                                //服务器19位编码 + char(27) + 服务器IP
                                LListStrRentServerInfo.Add(LStr11201004 + IStrSpliterChar + LStr11101017);
                            }
                        }
                    }
                    LListListRentServer.Add(LListStrRentServerInfo);
                }
                #endregion

                #region 将未分配给租户的服务器全部分配给顶级租户

                for (int LIntLoopServer = 0; LIntLoopServer < LListStrServerRentInfo.Count; LIntLoopServer++)
                {
                    foreach (List<string> LListStrRentServerInfo in LListListRentServer)
                    {
                        foreach (string LStrsingleServer in LListStrRentServerInfo)
                        {
                            if ((LStrsingleServer + IStrSpliterChar + "0") == LListStrServerRentInfo[LIntLoopServer])
                            {
                                string[] LStrArrayTemp = LStrsingleServer.Split(IStrSpliterChar.ToCharArray());
                                LListStrServerRentInfo[LIntLoopServer] = LStrArrayTemp[0] + IStrSpliterChar + LStrArrayTemp[1] + IStrSpliterChar + "1";
                            }
                        }
                    }
                }

                foreach (string LStrUnRentServer in LListStrServerRentInfo)
                {
                    string[] LStrArrayTemp = LStrUnRentServer.Split(IStrSpliterChar.ToCharArray());
                    if (LStrArrayTemp[2] == "0")
                    {
                        //List<string> LListTemp = new List<string>();
                        //LListTemp.Add(LStrArrayTemp[0] + IStrSpliterChar + LStrArrayTemp[1]);
                        LListListRentServer[0].Add(LStrArrayTemp[0] + IStrSpliterChar + LStrArrayTemp[1]);
                    }
                }
                #endregion

                #region 检查租户分机资源是否在原始配置资源中，如果不在，则从表中删除
                bool LBoolRented = false;
                List<string> LListStrRentServers = new List<string>();
                for (int LIntLoopRent = 0; LIntLoopRent < LListStrRentToken.Count; LIntLoopRent++)
                {
                    LListStrRentServers.Clear();
                    string[] LStrRentBasicArray = LListStrRentToken[LIntLoopRent].Split(IStrSpliterChar.ToCharArray());
                    foreach (string LStrRentServices in LListListRentServer[LIntLoopRent])
                    {
                        string[] LStrServerArray = LStrRentServices.Split(IStrSpliterChar.ToCharArray());
                        LListStrRentServers.Add(LStrServerArray[1]);
                    }
                    LStrDynamicSQL = "SELECT * FROM T_11_101_" + LStrRentBasicArray[0] + " WHERE C001 > 1040000000000000000 AND C001 < 1050000000000000000 AND C002 = 1";
                    LDatabaseOperation01Return = LDataOperations01.SelectDataByDynamicSQL(IIntDatabaseType, IStrDatabaseProfile, LStrDynamicSQL);
                    if (!LDatabaseOperation01Return.BoolReturn) { continue; }
                    foreach (DataRow LDataRowSingleChannel in LDatabaseOperation01Return.DataSetReturn.Tables[0].Rows)
                    {
                        LStr11101001 = LDataRowSingleChannel["C001"].ToString();
                        LStr11101017 = LDataRowSingleChannel["C017"].ToString();
                        LStrStep = LStr11101017;
                        LStr11101017 = EncryptionAndDecryption.EncryptDecryptString(LStr11101017, LStrVerificationCode102, EncryptionAndDecryption.UMPKeyAndIVType.M102);
                        string[] LStrExtensionServerIP = LStr11101017.Split(IStrSpliterChar.ToCharArray());
                        if (LStrExtensionServerIP.Length < 2)
                        {
                            //WriteEntryLog("ActionSynchronousExtensionData()\nError Extension\n{" + LStrStep + "}", EventLogEntryType.Warning);
                            continue;
                        }
                        
                        LBoolRented = false;
                        foreach (string LStrSingleServer in LListStrRentServers)
                        {
                            if (LStrSingleServer == LStrExtensionServerIP[1]) { LBoolRented = true; break; }
                        }
                        if (!LBoolRented)
                        {
                            LStrDynamicSQL = "UPDATE T_11_101_" + LStrRentBasicArray[0] + " SET C012 = '0' WHERE C001 = " + LStr11101001;
                            LDataOperations01.ExecuteDynamicSQL(IIntDatabaseType, IStrDatabaseProfile, LStrDynamicSQL);
                        }
                        else
                        {
                            DataRow[] LDataRowFindChannel = LDataSet11101AllLGChannel.Tables[0].Select("C002 = 1 AND C013 = '" + LStrExtensionServerIP[1] + "' AND C014 = '" + LStrExtensionServerIP[0] + "'");
                            if (LDataRowFindChannel == null || LDataRowFindChannel.Length <= 0)
                            {
                                LStrDynamicSQL = "UPDATE T_11_101_" + LStrRentBasicArray[0] + " SET C012 = '0' WHERE C001 = " + LStr11101001;
                                LDataOperations01.ExecuteDynamicSQL(IIntDatabaseType, IStrDatabaseProfile, LStrDynamicSQL);
                            }
                        }
                    }
                }
                #endregion

                #region 遍历所有Channel，添加到对应的租户中
                //C001:通道编码 19 位
                //C012:通道ID 0～2000
                //C017:录音服务器IP
                //C014:分机号
                //C015:通道名
                for (int LIntLoopRent = 0; LIntLoopRent < LListStrRentToken.Count; LIntLoopRent++)
                {
                    string[] LStrRentBasicArray = LListStrRentToken[LIntLoopRent].Split(IStrSpliterChar.ToCharArray());

                    #region 获取新用户默认密码
                    LStrDynamicSQL = "SELECT C006 FROM T_11_001_" + LStrRentBasicArray[0] + " WHERE C003 = 11010501";
                    LDatabaseOperation01Return = LDataOperations01.SelectDataByDynamicSQL(IIntDatabaseType, IStrDatabaseProfile, LStrDynamicSQL);
                    DataSet LDataSet11010501 = LDatabaseOperation01Return.DataSetReturn;
                    LStrUserDefaultPwd = LDataSet11010501.Tables[0].Rows[0][0].ToString();
                    LStrUserDefaultPwd = EncryptionAndDecryption.EncryptDecryptString(LStrUserDefaultPwd, LStrVerificationCode102, EncryptionAndDecryption.UMPKeyAndIVType.M102);
                    LStrUserDefaultPwd = LStrUserDefaultPwd.Substring(8);
                    #endregion

                    foreach (string LStrRentServices in LListListRentServer[LIntLoopRent])
                    {
                        string[] LStrServerArray = LStrRentServices.Split(IStrSpliterChar.ToCharArray());

                        DataRow[] LDataRowChannelBasic = LDataSet11101AllLGChannel.Tables[0].Select("C002 = 1 AND C017 = '" + LStrServerArray[1] + "'");
                        foreach (DataRow LDataRowSingleChannel in LDataRowChannelBasic)
                        {
                            LStr11101014 = LDataRowSingleChannel["C014"].ToString();
                            LStr11101013 = LStrServerArray[1];
                            LStr11101001 = LDataRowSingleChannel["C001"].ToString();
                            LStr11101012 = LDataRowSingleChannel["C012"].ToString();
                            LStr11101015 = LDataRowSingleChannel["C015"].ToString();
                            LStr11101017 = LDataRowSingleChannel["C017"].ToString();

                            LStrTemp = EncryptionAndDecryption.EncryptDecryptString(LStr11101014 + IStrSpliterChar + LStr11101013, LStrVerificationCode002, EncryptionAndDecryption.UMPKeyAndIVType.M002);
                            LStrDynamicSQL = "SELECT * FROM T_11_101_" + LStrRentBasicArray[0] + " WHERE C001 IN (SELECT C001 FROM T_11_101_" + LStrRentBasicArray[0] + " WHERE C001 > 1040000000000000000 AND C001 < 1050000000000000000 AND C017 = '" + LStrTemp + "') ORDER BY C002";

                            LDatabaseOperation01Return = LDataOperations01.SelectDataByDynamicSQL(IIntDatabaseType, IStrDatabaseProfile, LStrDynamicSQL);
                            DataSet LDataSetSave2DBBI = LDatabaseOperation01Return.DataSetReturn;
                            LDataSetSave2DBBI.Tables[0].TableName = "T_11_101_" + LStrRentBasicArray[0];
                            if (LDataSetSave2DBBI.Tables[0].Rows.Count <= 0)
                            {
                                LStrDataID = GetSerialIDByType(IIntDatabaseType, IStrDatabaseProfile, LStrRentBasicArray[0], 11, 104).ToString();
                                //LStrUserDefaultPwd = EncryptionAndDecryption.EncryptDecryptString(LStrDataID + LStrUserDefaultPwd, LStrVerificationCode002, EncryptionAndDecryption.UMPKeyAndIVType.M002);
                                #region 如果是新增分机，初始化2行数据
                                for (int LIntAddRow = 1; LIntAddRow <= 2; LIntAddRow++)
                                {
                                    DataRow LDataRowNewData = LDataSetSave2DBBI.Tables[0].NewRow();
                                    LDataRowNewData.BeginEdit();
                                    LDataRowNewData["C001"] = long.Parse(LStrDataID);
                                    LDataRowNewData["C002"] = LIntAddRow;
                                    if (LIntAddRow == 1)
                                    {
                                        LDataRowNewData["C011"] = "101" + LStrRentBasicArray[0] + "00000000001";
                                        LDataRowNewData["C012"] = "1";
                                        LDataRowNewData["C013"] = "1";
                                        LDataRowNewData["C014"] = "0";
                                        LDataRowNewData["C015"] = "N";
                                        LDataRowNewData["C016"] = "09";
                                        LDataRowNewData["C017"] = LStrTemp;
                                        LDataRowNewData["C020"] = EncryptionAndDecryption.EncryptStringSHA512(LStrDataID + LStrUserDefaultPwd, LStrVerificationCode002, EncryptionAndDecryption.UMPKeyAndIVType.M002); ;
                                    }
                                    if (LIntAddRow == 2)
                                    {
                                        LDataRowNewData["C011"] = "2014/01/01 00:00:00";
                                        LDataRowNewData["C012"] = "0";
                                        LDataRowNewData["C013"] = "0";
                                    }
                                    LDataRowNewData.EndEdit();
                                    LDataSetSave2DBBI.Tables[0].Rows.Add(LDataRowNewData);

                                    if (IIntDatabaseType == 2)
                                    {
                                        LStrDynamicSQL = "INSERT INTO T_11_201_" + LStrRentBasicArray[0] + " VALUES(0, 0, 102" + LStrRentBasicArray[0] + "00000000001, " + LStrDataID + ", '2014-01-01 00:00:00', '2199-12-31 23:59:59')";
                                    }
                                    else
                                    {
                                        LStrDynamicSQL = "INSERT INTO T_11_201_" + LStrRentBasicArray[0] + " VALUES(0, 0, 102" + LStrRentBasicArray[0] + "00000000001, " + LStrDataID + ", TO_DATE('2014-01-01 00:00:00','yyyy-MM-dd HH24:mi:ss'), TO_DATE('2199-12-31 23:59:59','yyyy-MM-dd HH24:mi:ss'))";
                                    }
                                    LDataOperations01.ExecuteDynamicSQL(IIntDatabaseType, IStrDatabaseProfile, LStrDynamicSQL);
                                }
                                #endregion

                            }
                            LStrDataID = LDataSetSave2DBBI.Tables[0].Rows[0]["C001"].ToString();

                            #region 更新分机信息
                            LDataSetSave2DBBI.Tables[0].Rows[0]["C012"] = "1";
                            LDataSetSave2DBBI.Tables[0].Rows[0]["C018"] = LStr11101015;
                            LStr11101013 = LDataRowSingleChannel["C013"].ToString();
                            //WriteEntryLog("LStr11101013 = " + LStr11101013, EventLogEntryType.Warning);
                            if (LStr11101013.Substring(0, 3) == "221")
                            {
                                //LDataSetSave2DBBI.Tables[0].Rows[1]["C015"] = LStr11101017;
                                LDataSetSave2DBBI.Tables[0].Rows[1]["C015"] = LStr11101013;
                                LDataSetSave2DBBI.Tables[0].Rows[1]["C016"] = LStr11101012;
                                LDataSetSave2DBBI.Tables[0].Rows[1]["C017"] = LStr11101001;
                            }
                            else
                            {
                                //LDataSetSave2DBBI.Tables[0].Rows[1]["C018"] = LStr11101017;
                                LDataSetSave2DBBI.Tables[0].Rows[1]["C018"] = LStr11101013;
                                LDataSetSave2DBBI.Tables[0].Rows[1]["C019"] = LStr11101012;
                                LDataSetSave2DBBI.Tables[0].Rows[1]["C020"] = LStr11101001;
                            }
                            #endregion

                            #region 将数据保存到MSSQL数据库
                            if (IIntDatabaseType == 2)
                            {
                                LSqlConnection = new SqlConnection(IStrDatabaseProfile);

                                LStrDynamicSQL = "SELECT * FROM T_11_101_" + LStrRentBasicArray[0] + " WHERE C001 = " + LStrDataID + " ORDER BY C002 ASC";
                                SqlDataAdapter LSqlDataAdapter1 = new SqlDataAdapter(LStrDynamicSQL, LSqlConnection);
                                SqlCommandBuilder LSqlCommandBuilder1 = new SqlCommandBuilder();

                                LSqlCommandBuilder1.ConflictOption = ConflictOption.OverwriteChanges;
                                LSqlCommandBuilder1.SetAllValues = false;
                                LSqlCommandBuilder1.DataAdapter = LSqlDataAdapter1;
                                LSqlDataAdapter1.Update(LDataSetSave2DBBI, "T_11_101_" + LStrRentBasicArray[0]);
                                LDataSetSave2DBBI.AcceptChanges();
                                LSqlCommandBuilder1.Dispose();
                                LSqlDataAdapter1.Dispose();
                            }
                            #endregion

                            #region 将数据保存到Oracle数据库
                            if (IIntDatabaseType == 3)
                            {
                                LOracleConnection = new OracleConnection(IStrDatabaseProfile);

                                LStrDynamicSQL = "SELECT * FROM T_11_101_" + LStrRentBasicArray[0] + " WHERE C001 = " + LStrDataID + " ORDER BY C002 ASC";
                                OracleDataAdapter LOracleDataAdapter1 = new OracleDataAdapter(LStrDynamicSQL, LOracleConnection);
                                OracleCommandBuilder LOracleCommandBuilder1 = new OracleCommandBuilder();

                                LOracleCommandBuilder1.ConflictOption = ConflictOption.OverwriteChanges;
                                LOracleCommandBuilder1.SetAllValues = false;
                                LOracleCommandBuilder1.DataAdapter = LOracleDataAdapter1;
                                LOracleDataAdapter1.Update(LDataSetSave2DBBI, "T_11_101_" + LStrRentBasicArray[0]);
                                LDataSetSave2DBBI.AcceptChanges();
                                LOracleCommandBuilder1.Dispose();
                                LOracleDataAdapter1.Dispose();
                            }
                            #endregion
                        }
                    }
                }
                #endregion

            }
            catch (Exception ex)
            {
                IBooInSynchronous = false;
                IBoolCanAbortSynchronous = true;
                WriteEntryLog("ActionSynchronousExtensionData()\n" + LStrStep + "\n" + ex.ToString(), EventLogEntryType.Error);
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

        #region 删除已经加密成功的录音记录
        private void DeleteCompleteEncryptionData()
        {
            string LStrDynamicSQL = string.Empty;
            string LStr21998001 = string.Empty;

            try
            {
                DatabaseOperation01Return LDatabaseOperation01Return = new DatabaseOperation01Return();
                DatabaseOperation01Return LDatabaseOperation02Return = new DatabaseOperation01Return();
                DataOperations01 LDataOperations01 = new DataOperations01();

                while (IBoolCanContinue)
                {
                    if (IIntDatabaseType == 0 || string.IsNullOrEmpty(IStrDatabaseProfile)) { IBoolInDeleteEncryption = false; Thread.Sleep(2000); continue; }

                    IBoolInDeleteEncryption = true;
                    if (IIntDatabaseType == 2)
                    {
                        LStrDynamicSQL = "SELECT TOP 500 C001 FROM T_21_998 WHERE C019 <> 'E'";
                    }
                    else
                    {
                        LStrDynamicSQL = "SELECT * FROM T_21_998 WHERE C019 <> 'E' AND ROWNUM <= 500";
                    }

                    LDatabaseOperation02Return = LDataOperations01.SelectDataByDynamicSQL(IIntDatabaseType, IStrDatabaseProfile, LStrDynamicSQL);
                    if (LDatabaseOperation02Return.BoolReturn)
                    {
                        foreach (DataRow LDataRowSingleRow in LDatabaseOperation02Return.DataSetReturn.Tables[0].Rows)
                        {
                            LStr21998001 = LDataRowSingleRow["C001"].ToString();
                            LStrDynamicSQL = "DELETE FROM T_21_998 WHERE C001 = '" + LStr21998001 + "'";
                            LDatabaseOperation01Return = LDataOperations01.ExecuteDynamicSQL(IIntDatabaseType, IStrDatabaseProfile, LStrDynamicSQL);
                            if (!LDatabaseOperation01Return.BoolReturn)
                            {
                                WriteEntryLog("DeleteCompleteEncryptionData()\n" + LDatabaseOperation01Return.StrReturn, EventLogEntryType.Error);
                            }
                        }
                    }
                    System.Threading.Thread.Sleep(1500);
                    IBoolInDeleteEncryption = false;
                    IBoolCanAbortDeleteEncryption = true;
                }
                IBoolInDeleteEncryption = false;
                IBoolCanAbortDeleteEncryption = true;
            }
            catch (Exception ex)
            {
                IBoolInDeleteEncryption = false;
                IBoolCanAbortDeleteEncryption = true;
                WriteEntryLog("DeleteCompleteEncryptionData()\n" + ex.ToString(), EventLogEntryType.Error);
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
                    if (LStrDirectorySingle == "WinServices") { break; }
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

                WriteEntryLog("Readed UMP.PF Binding Port : " + LIntReturn.ToString() + ", This Service Use " + (LIntReturn - 7).ToString(), EventLogEntryType.Information);
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
    }
}
