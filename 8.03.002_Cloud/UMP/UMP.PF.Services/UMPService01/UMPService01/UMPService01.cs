using CircleQueueClass;
using PFShareClasses01;
using PFShareClassesS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Xml;
using VoiceCyber.Common;
using VoiceCyber.SDKs.Licenses;
using VoiceCyber.SharpZips.Zip;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Encryptions;
using License = VoiceCyber.SDKs.Licenses.License;

namespace UMPService01
{
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant, InstanceContextMode = InstanceContextMode.Single)]
    public partial class UMPService01 : ServiceBase, IEncryptable
    {
        public static string IStrEventLogSource = "Service 01";
        public static string IStrApplicationName = "UMP";
        public static int IIntThisServicePort = 0;
        public static string IStrBaseDirectory = string.Empty;
        public static string IStrSiteBindingIPAddress = string.Empty;

        public static List<DataTable> IDataTable11002 = new List<DataTable>();

        public static EventLog IEventLog = null;

        #region 标志回删录音记录
        public static readonly object ILockObjectUpdate21001033 = new object();
        public static readonly object ILockObjectUpdate2100123X = new object();
        public static YoungCircleQueue<List<string>> IYoungCircleQueueUpdate21001033 = new YoungCircleQueue<List<string>>(1);
        public static YoungCircleQueue<List<string>> IYoungCircleQueueUpdate2100123X = new YoungCircleQueue<List<string>>(1);
        #endregion

        #region 读取License
        private LicenseHelper mLicHelper = new LicenseHelper();
        public static readonly object ILockObjectReadLicense = new object();
        public static YoungCircleQueue<string> IYoungCircleQueueReadLicense = new YoungCircleQueue<string>(1);
        #endregion

        #region 将客户端消息写操作日志
        public static readonly object ILock11901 = new object();
        public static YoungCircleQueue<ClientOpArgsAndReturn> IYoungCircleQueueWriteOperation = new YoungCircleQueue<ClientOpArgsAndReturn>(1);
        #endregion

        private Thread IThreadReadDBProfile;
        private Thread IThreadUpdate21001033;
        private Thread IThreadUpdate2100123X;
        private Thread IThreadInsert11901;
        private Thread IThreadReadLicense;

        public static readonly object ILockObjectClient = new object();
        public static List<string> IListStrLoginClient = new List<string>();
        public static List<string> IListStrLoginGuid = new List<string>();
        public static List<DateTime> IListDateTimeClient = new List<DateTime>();

        #region 与获取许可有关的变量
        /// <summary>
        /// 当前登录连接的序号
        /// </summary>
        public static ulong IULongConnectedSerialID = 0;

        public static string IStrLicenseInfor = string.Empty;
        public static bool IBoolGetLicense = false;
        public static string IStrLicenseNum = string.Empty;
        public static string IStrIsReadedLicense = "0";
        public static DateTime IDateTimeLastReadedLicense = DateTime.UtcNow;
        public static int IIntP01 = 1;
        public static int IIntP02 = 1;
        public static string IStrP03 = DateTime.UtcNow.ToString("G");
        #endregion

        #region 数据库类型及数据库连接参数
        public static int IIntDBType = 0;
        public static string IStrDBConnectProfile = string.Empty;
        #endregion

        /// <summary>
        /// 服务是否可以继续工作
        /// </summary>
        public bool IBoolCanContinue = true;

        #region 服务是否正在处理过程中的Bool变量
        private bool IBoolIsBusing = false;
        private bool IBoolInUpdate21001033 = false;
        private bool IBoolInUpdate2100123X = false;
        private bool IBoolInInsert11901 = false;
        private bool IBoolInReadLicense = false;
        #endregion

        private static readonly object ILockObjectCheckThread = new object();
        private static List<ClientOperations> GlobalListConnectedClient = new List<ClientOperations>();
        private static List<Thread> GlobalListClientThread = new List<Thread>();

        private Thread ThreadCheckThreadIsAlive = null;

        public static X509Certificate2 IX509CertificateServer = null;
        static TcpListener ITcpListener = null;
        private long _mLicID;
        private LicConnector _mLicChecker;
        #region Log
        private string mAppName = "UMPService01";
        private ConfigInfo mConfigInfo;
        #endregion

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
                        string.Format("UMP\\{0}\\Logs", mAppName));
                }
                else
                {
                    strReturn = Path.Combine(strReturn, mAppName);
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

        private void CreateFileLog()
        {
            try
            {
                string path = GetLogPath();
                gLogOperator = new LogOperator();
                gLogOperator.LogPath = path;
                gLogOperator.Start();
                string strInfo = string.Empty;
                strInfo += string.Format("LogPath:{0}\r\n", path);
                strInfo += string.Format("\tExePath:{0}\r\n", AppDomain.CurrentDomain.BaseDirectory);
                strInfo += string.Format("\tName:{0}\r\n", AppDomain.CurrentDomain.FriendlyName);
                strInfo += string.Format("\tVersion:{0}\r\n",
                    System.Reflection.Assembly.GetExecutingAssembly().GetName().Version);
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
            if (gLogOperator != null)
            {
                gLogOperator.WriteLog(mode, category, msg);
            }
        }

        public void WriteLog(string category, string msg)
        {
            WriteLog(LogMode.Info, category, msg);
        }

        public static void WriteLog(LogMode mode, string msg)
        {
            WriteLog(mode, "UMPService01", msg);
        }

        public void WriteLog(string msg)
        {
            WriteLog(LogMode.Info, msg);
        }

        private void SetLogMode()
        {
            try
            {
                if (mConfigInfo == null) { return; }
                var setting = mConfigInfo.ListSettings.FirstOrDefault(s => s.Key == ConstValue.GS_KEY_LOG_MODE);
                if (setting == null) { return; }
                string strValue = setting.Value;
                int intValue;
                if (int.TryParse(strValue, out intValue)
                    && intValue > 0)
                {
                    if (gLogOperator != null)
                    {
                        gLogOperator.LogMode = (LogMode)intValue;
                        WriteLog(LogMode.Info, string.Format("LogMode changed.\t{0}", (LogMode)intValue));
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLog(LogMode.Error, string.Format("SetLogMode fail.\t{0}", ex.Message));
            }
        }

        #region ConfigInfo

        private void LoadConfigInfo()
        {
            try
            {
                string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                    ConstValue.TEMP_DIR_UMP);
                path = Path.Combine(path, mAppName);
                path = Path.Combine(path, ConstValue.TEMP_FILE_CONFIGINFO);
                if (!File.Exists(path))
                {
                    WriteLog(LogMode.Error, string.Format("ConfigInfo file not exist.\t{0}", path));
                    return;
                }
                OperationReturn optReturn = XMLHelper.DeserializeFile<ConfigInfo>(path);
                if (!optReturn.Result)
                {
                    WriteLog(LogMode.Error,
                        string.Format("LoadConfigInfo fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                ConfigInfo configInfo = optReturn.Data as ConfigInfo;
                if (configInfo == null)
                {
                    WriteLog(LogMode.Error, string.Format("LoadConfigInfo fail.\tConfigInfo is null"));
                    return;
                }
                mConfigInfo = configInfo;
                WriteLog(LogMode.Info, string.Format("LoadConfigInfo end.\t{0}", path));
            }
            catch (Exception ex)
            {
                WriteLog(LogMode.Error, string.Format("LoadConfigInfo fail.\t{0}", ex.Message));
            }
        }

        #endregion

        public UMPService01()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            CreateFileLog();
            WriteLog(LogMode.Info, string.Format("Service starting..."));
            LoadConfigInfo();
            SetLogMode();
            Thread LThreadStart = new Thread(new ThreadStart(StartService01));
            LThreadStart.Start();
        }

        protected override void OnStop()
        {
            try
            {
                IBoolCanContinue = false;
                while (IBoolIsBusing) { Thread.Sleep(100); }
                if (ITcpListener != null) { ITcpListener.Stop(); ITcpListener = null; }
                CloseAllClientThread();

                try
                {
                    IThreadReadDBProfile.Abort();
                }
                catch { }

                try
                {
                    while (IBoolInUpdate21001033) { Thread.Sleep(100); }
                    IThreadUpdate21001033.Abort();
                }
                catch { }

                try
                {
                    while (IBoolInUpdate2100123X) { Thread.Sleep(100); }
                    IThreadUpdate2100123X.Abort();
                }
                catch { }

                try
                {
                    while (IBoolInInsert11901) { Thread.Sleep(100); }
                    IThreadInsert11901.Abort();
                }
                catch { }

                try
                {
                    while (IBoolInReadLicense) { Thread.Sleep(100); }
                    IThreadReadLicense.Abort();
                }
                catch { }

                WriteLog(LogMode.Info, string.Format("Service stopped"));
            }
            catch (Exception ex)
            {
                WriteLog(LogMode.Error, "Stop Service Failed\n" + ex.Message );
            }
        }

        private void StartService01()
        {
            if (!EventLog.SourceExists(IStrEventLogSource)) { EventLog.CreateEventSource(IStrEventLogSource, IStrApplicationName); }
            UMPServiceLog.Source = IStrEventLogSource;
            UMPServiceLog.Log = IStrApplicationName;
            UMPServiceLog.ModifyOverflowPolicy(OverflowAction.OverwriteOlder, 3);
            IEventLog = UMPServiceLog;
            WriteLog(LogMode.Info, "Service Started At : " + DateTime.Now.ToString("G"));

            //CreateNetPipeServer();
            IStrBaseDirectory = GetCurrentBaseDirectory();
            if (string.IsNullOrEmpty(IStrBaseDirectory)) { return; }

            IIntThisServicePort = GetUMPPFBasicHttpPort();
            while (IIntThisServicePort == 0)
            {
                Thread.Sleep(1000);
                IIntThisServicePort = GetUMPPFBasicHttpPort();
            }
            IIntThisServicePort -= 1;

            //GetDatabaseConnectionProfile();

            //while (IIntDBType == 0 || string.IsNullOrEmpty(IStrDBConnectProfile))
            //{
            //    if (!IBoolCanContinue) { break; }
            //    GetDatabaseConnectionProfile();
            //    Thread.Sleep(1000);
            //}

            IThreadReadDBProfile = new Thread(new ThreadStart(ReadDatabaseConnectionProfile));
            IThreadReadDBProfile.Start();

            if (!CreateCheckThreadIsAlive()) { return; }

            IThreadUpdate21001033 = new Thread(new ThreadStart(ActiveUpdate21001033));
            IThreadUpdate21001033.Start();

            IThreadUpdate2100123X = new Thread(new ThreadStart(ActiveUpdate2100123X));
            IThreadUpdate2100123X.Start();

            IThreadInsert11901 = new Thread(new ThreadStart(ActiveInsert11901));
            IThreadInsert11901.Start();

            IThreadReadLicense = new Thread(new ThreadStart(ReadLicenseInformation));
            IThreadReadLicense.Start();

            StartTcpSSLServer(IIntThisServicePort);

            GetSetted0000FromServer03Xml();
            WaitForSocketClientConnect();
            
        }

        #region 读取数据库连接信息
        private void ReadDatabaseConnectionProfile()
        {
            while (IIntDBType == 0 || string.IsNullOrEmpty(IStrDBConnectProfile))
            {
                if (!IBoolCanContinue) { break; }
                GetDatabaseConnectionProfile();
                Thread.Sleep(1000);
            }
        }
        #endregion

        #region 读取License
        private bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        private bool ReadMessageFromServer(SslStream ASslStream, ref string AStrReadedMessage)
        {
            bool LBoolReturn = true;

            try
            {
                AStrReadedMessage = string.Empty;

                StringBuilder LStringBuilderData = new StringBuilder();
                int LIntReadedBytes = -1, LIntEndKeyPosition;
                byte[] LByteReadeBuffer = new byte[1024];

                do
                {
                    LIntReadedBytes = ASslStream.Read(LByteReadeBuffer, 0, LByteReadeBuffer.Length);
                    Decoder LDecoder = Encoding.UTF8.GetDecoder();
                    char[] LChars = new char[LDecoder.GetCharCount(LByteReadeBuffer, 0, LIntReadedBytes)];
                    LDecoder.GetChars(LByteReadeBuffer, 0, LIntReadedBytes, LChars, 0);
                    LStringBuilderData.Append(LChars);
                    if (LStringBuilderData.ToString().IndexOf(AscCodeToChr(27) + "End" + AscCodeToChr(27)) > 0) { break; }
                }
                while (LIntReadedBytes != 0);
                AStrReadedMessage = LStringBuilderData.ToString();
                LIntEndKeyPosition = AStrReadedMessage.IndexOf(AscCodeToChr(27) + "End" + AscCodeToChr(27));
                if (LIntEndKeyPosition > 0)
                {
                    AStrReadedMessage = AStrReadedMessage.Substring(0, LIntEndKeyPosition);
                }
            }
            catch (Exception ex)
            {
                LBoolReturn = false;
                AStrReadedMessage = ex.Message;
            }

            return LBoolReturn;
        }

        #region 生成 MD5 Hash 字符串
        /// <summary>
        /// 生成 MD5 Hash 字符串
        /// </summary>
        /// <param name="AStrSource"></param>
        /// <returns></returns>
        private string CreateMD5HashString(string AStrSource)
        {
            string LStrHashPassword = string.Empty;
            try
            {
                MD5CryptoServiceProvider LMD5Crypto = new MD5CryptoServiceProvider();
                byte[] LByteArray = Encoding.Unicode.GetBytes(AStrSource);
                LByteArray = LMD5Crypto.ComputeHash(LByteArray);
                StringBuilder LStrBuilder = new StringBuilder();
                foreach (byte LByte in LByteArray) { LStrBuilder.Append(LByte.ToString("X2").ToUpper()); }
                LStrHashPassword = LStrBuilder.ToString();
            }
            catch { LStrHashPassword = "YoungPassword"; }
            return LStrHashPassword;
        }
        #endregion


        private void GetSetted0000FromLicenseServer()
        {
            TcpClient LTcpClient = null;
            SslStream LSslStream = null;
            string LStrVerificationCode004 = string.Empty;
            string LStrSendMessage = string.Empty;
            string LStrReadMessage = string.Empty;

            try
            {
                lock (ILockObjectClient)
                {
                    if (IULongConnectedSerialID == ulong.MaxValue) { IULongConnectedSerialID = 0; }
                    IULongConnectedSerialID += 1;
                    WriteLog(LogMode.Info, "Try To Log In To The UMP System, The Serial Number: " + IULongConnectedSerialID.ToString());
                    if ( IStrIsReadedLicense == "0" || IStrIsReadedLicense == "2")
                    {
                        LStrVerificationCode004 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004);
                        LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString("G008", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);

                        try
                        {
                            //LTcpClient = new TcpClient("127.0.0.1", 8009);

                            LTcpClient = new TcpClient();
                            LTcpClient.SendTimeout = 10000;
                            LTcpClient.Connect("127.0.0.1", 8009);
                            LSslStream = new SslStream(LTcpClient.GetStream(), false, new RemoteCertificateValidationCallback(ValidateServerCertificate), null);
                            LSslStream.AuthenticateAsClient("VoiceCyber.PF", null, SslProtocols.Default, false);
                            byte[] LByteMesssage = Encoding.UTF8.GetBytes(LStrSendMessage + "\r\n");
                            LSslStream.Write(LByteMesssage); LSslStream.Flush();
                            if (ReadMessageFromServer(LSslStream, ref LStrReadMessage))
                            {
                                if (LStrReadMessage.Contains("1100001") && LStrReadMessage.Contains("1100002"))
                                {
                                    LStrReadMessage = LStrReadMessage.Replace("\r\n", "");
                                    string[] LStrReadedLicense = LStrReadMessage.Split(AscCodeToChr(27).ToArray(), StringSplitOptions.RemoveEmptyEntries);
                                    foreach (string LStrSingleLicense in LStrReadedLicense)
                                    {
                                        if (LStrSingleLicense.Substring(0, 7) == "1100000") { IStrP03 = LStrSingleLicense.Substring(8); }
                                        if (LStrSingleLicense.Substring(0, 7) == "1100001") { IIntP01 = int.Parse(LStrSingleLicense.Substring(8)); }
                                        if (LStrSingleLicense.Substring(0, 7) == "1100002") { IIntP02 = int.Parse(LStrSingleLicense.Substring(8)); }
                                    }
                                    IStrIsReadedLicense = "1";
                                }
                                else
                                {
                                    if (IStrIsReadedLicense == "0")
                                    {
                                        //WriteLog(LogMode.Error, "1-2-------------IStrP03 = [{0}]\n" + IStrP03);
                                        DateTime dtP03 = Convert.ToDateTime(IStrP03);
                                        DateTime dtNow = DateTime.UtcNow;
                                        TimeSpan span = dtP03.Subtract(dtNow);
                                        if ((int)span.TotalHours > 72 || (int)span.TotalHours == 0)
                                        {
                                            IStrP03 = DateTime.UtcNow.AddHours(72).ToString("G");
                                        }
                                    }
                                    IStrIsReadedLicense = "2";
                                    WriteLog(LogMode.Error, "GetUMPSetted0000()1.ReadMessageFromServer()\n" + LStrReadMessage + "\n" );
                                }
                            }
                            else
                            {
                                if (IStrIsReadedLicense == "0")
                                {
                                    DateTime dtP03 = Convert.ToDateTime(IStrP03);
                                    DateTime dtNow = DateTime.UtcNow;
                                    TimeSpan span = dtP03.Subtract(dtNow);
                                    if ((int)span.TotalHours > 72 || (int)span.TotalHours == 0)
                                    {
                                        IStrP03 = DateTime.UtcNow.AddHours(72).ToString("G");
                                    }
                                }
                                IStrIsReadedLicense = "2";
                                WriteLog(LogMode.Error, "GetUMPSetted0000()2.ReadMessageFromServer()\n" + LStrReadMessage + "\n" );
                            }
                        }
                        catch (Exception ex)
                        {
                            if (IStrIsReadedLicense == "0" || IStrIsReadedLicense == "1")
                            {
                                DateTime dtP03 = Convert.ToDateTime(IStrP03);
                                DateTime dtNow = DateTime.UtcNow;
                                TimeSpan span = dtP03.Subtract(dtNow);
                                if ((int)span.TotalHours > 72 || (int)span.TotalHours == 0)
                                {
                                    IStrP03 = DateTime.UtcNow.AddHours(72).ToString("G");
                                }
                            }
                            IStrIsReadedLicense = "2";
                            WriteLog(LogMode.Error, "GetUMPSetted0000()3\n" + ex.Message );
                        }
                        Write2Server03Xml();
                        
                    }
                    WriteLog(LogMode.Info, "GetUMPSetted0000()4.ReadMessageFromServer()\n1100000 = " + IStrP03 + "  1100001 = " + IIntP01.ToString() + "   1100002 = " + IIntP02.ToString());
                }
            }
            catch (Exception ex)
            {
                WriteLog(LogMode.Error, "GetUMPSetted0000()\n" + ex.Message + "\n" );
            }
            finally
            {
                if (LSslStream != null) { LSslStream.Close(); LSslStream = null; }
                if (LTcpClient != null) { LTcpClient.Close(); LTcpClient = null; }
            }
        }

        /// <summary>
        /// 获取license
        /// </summary>
        private bool GetLicenseMessageInfo(string strAddress, string strPort)
        {
            try
            {
                List<License> listLicenses = new List<License>();
                License lic = new License();
                lic.Name = "UserNumber";
                lic.SerialNo = 1100001;
                lic.Expiration = LicDefines.KEYWORD_LICENSE_EXPIRATION_UNLIMITED;
                lic.Type = LicOwnerType.Mono;
                lic.DataType = LicDataType.Number;
                lic.MajorID = 1121;
                lic.MinorID = 2;
                lic.RequestValue = "100";
                lic.Value = "0";
                listLicenses.Add(lic);
                lic = new License();
                lic.Name = "OnlineUserNumber";
                lic.SerialNo = 1100002;
                lic.Expiration = LicDefines.KEYWORD_LICENSE_EXPIRATION_UNLIMITED;
                lic.Type = LicOwnerType.Mono;
                lic.DataType = LicDataType.Number;
                lic.MajorID = 1121;
                lic.MinorID = 2;
                lic.RequestValue = "1000";
                lic.Value = "0";
                listLicenses.Add(lic);
                lic = new License();
                lic.Name = "Lic";
                lic.SerialNo = 1100101;
                lic.MajorID = 1121;
                lic.MinorID = 2;
                listLicenses.Add(lic);

                if (mLicHelper != null)
                {
                    mLicHelper.Stop();
                    mLicHelper = null;
                }
                mLicHelper = new LicenseHelper();
                mLicHelper.Debug += (mode, cat, msg) => WriteLog(string.Format("{0}\t{1}", cat, msg));
                mLicHelper.LicInfoChanged += mLicHelper_LicInfoChanged;
                mLicHelper.Host = strAddress;
                mLicHelper.Port = Convert.ToInt32(strPort);
                mLicHelper.ClearLicense();
                for (int i = 0; i < listLicenses.Count; i++)
                {
                    mLicHelper.ListLicenses.Add(listLicenses[i]);
                }

                mLicHelper.Start();
                mLicHelper.Multicast.Reset();
                mLicHelper.Multicast.WaitOne(10 * 1000);

                switch (mLicHelper.LicMgeNum)
                {
                    case 1:
                        WriteLog( LogMode.Error, string.Format("License conn't connect !"));
                        return false;
                    case 2:
                        IStrLicenseNum = mLicHelper.LicNum;

                        foreach (var param in mLicHelper.LstLicensesInfos)
                        {
                            switch (param.SerialNo)
                            {
                                case 1100001:
                                    IIntP01 = int.Parse(param.Value);
                                    break;
                                case 1100002:
                                    IIntP02 = int.Parse(param.Value);
                                    break;
                                case 1100101:
                                    IStrP03 = param.Expiration;
                                    IStrLicenseInfor = param.Value;
                                    break;
                            }
                        }
                        mLicHelper.Stop();
                        WriteLog(LogMode.Info,
                            string.Format(
                                "GetLicenseMessageInfo() 1100001 = {0}\n1100002 = {1} \n 1100000 = {2}\n1100101 = {3}",
                                IIntP01, IIntP02,
                                IStrP03, IStrLicenseInfor));
                        break;
                }
            }
            catch (Exception ex)
            {
                WriteLog(LogMode.Info, string.Format("Fail.\t{0}", ex.Message));
                return false;
            }
            return true;
        }

        private void GetSettedLicenseServer()
        {
            try
            {
                lock (ILockObjectClient)
                {
                    if (IULongConnectedSerialID == ulong.MaxValue) { IULongConnectedSerialID = 0; }
                    IULongConnectedSerialID += 1;
                    WriteLog(LogMode.Info, "Try To Log In To The UMP System, The Serial Number: " + IULongConnectedSerialID.ToString());
                    if (IStrIsReadedLicense == "0" || IStrIsReadedLicense == "2")
                    {
                        try
                        {
                            if (IBoolGetLicense)
                            {
                                IStrIsReadedLicense = "1";
                            }
                            else
                            {
                                if (IStrIsReadedLicense == "0")
                                {
                                    DateTime dtP03 = Convert.ToDateTime(IStrP03);
                                    DateTime dtNow = DateTime.UtcNow;
                                    TimeSpan span = dtP03.Subtract(dtNow);
                                    if ((int)span.TotalHours > 72 || (int)span.TotalHours == 0)
                                    {
                                        IStrP03 = DateTime.UtcNow.AddHours(72).ToString("G");
                                    }
                                }
                                IStrIsReadedLicense = "2";
                                WriteLog(LogMode.Info, "GetSettedLicenseServer()2.ReadMessageFromServer()\n1100000 = " + IStrP03 + "  1100001 = " + IIntP01.ToString() + "   1100002 = " + IIntP02.ToString());
                            }
                        }
                        catch (Exception ex)
                        {
                            if (IStrIsReadedLicense == "0" || IStrIsReadedLicense == "1")
                            {
                                DateTime dtP03 = Convert.ToDateTime(IStrP03);
                                DateTime dtNow = DateTime.UtcNow;
                                TimeSpan span = dtP03.Subtract(dtNow);
                                if ((int)span.TotalHours > 72 || (int)span.TotalHours == 0)
                                {
                                    IStrP03 = DateTime.UtcNow.AddHours(72).ToString("G");
                                }
                            }
                            IStrIsReadedLicense = "2";
                            WriteLog(LogMode.Error, "GetSettedLicenseServer()3\n" + ex.Message);
                        }
                        Write2Server03Xml();
                    }
                    WriteLog(LogMode.Info, "GetSettedLicenseServer()4.ReadMessageFromServer()\n1100000 = " + IStrP03 + "  1100001 = " + IIntP01.ToString() + "   1100002 = " + IIntP02.ToString());
                }
            }
            catch (Exception ex)
            {
                WriteLog(LogMode.Error, "GetSettedLicenseServer()\n" + ex.Message + "\n");
            }
        }

        /// <summary>
        /// 从Args02.UMP.xml中获取licensed地址和端口
        /// </summary>
        private bool GetLicenseInfo()
        {
            string LStrXmlFileName = string.Empty;
            string LStrVerificationCode101 = string.Empty;
            string LStrP01 = string.Empty, LStrP02 = string.Empty, LStrP03 = string.Empty, LStrP04 = string.Empty, LStrP05 = string.Empty;

            try
            {
                LStrVerificationCode101 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M101);
                LStrXmlFileName = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                LStrXmlFileName = Path.Combine(LStrXmlFileName, @"UMP.Server\Args02.UMP.xml");
                XmlDocument LXmlDocArgs02 = new XmlDocument();
                LXmlDocArgs02.Load(LStrXmlFileName);
                XmlNode LXmlNodeLicenseServer = LXmlDocArgs02.SelectSingleNode("Parameters02").SelectSingleNode("LicenseServer");
                XmlNode LXmlNodeLicServer = LXmlNodeLicenseServer.SelectSingleNode("LicServer");
                while (LXmlNodeLicServer != null)
                {
                    LStrP01 = LXmlNodeLicServer.Attributes["P01"].Value;
                    LStrP02 = LXmlNodeLicServer.Attributes["P02"].Value;
                    if (LStrP02 == "1" && LStrP01 == "1")
                    {
                        LStrP03 = LXmlNodeLicServer.Attributes["P03"].Value;
                        LStrP03 = EncryptionAndDecryption.EncryptDecryptString(LStrP03, LStrVerificationCode101, EncryptionAndDecryption.UMPKeyAndIVType.M101);
                        LStrP04 = LXmlNodeLicServer.Attributes["P04"].Value;
                        LStrP04 = EncryptionAndDecryption.EncryptDecryptString(LStrP04, LStrVerificationCode101, EncryptionAndDecryption.UMPKeyAndIVType.M101);
                        LStrP05 = LXmlNodeLicServer.Attributes["P05"].Value;
                        LStrP05 = EncryptionAndDecryption.EncryptDecryptString(LStrP05, LStrVerificationCode101, EncryptionAndDecryption.UMPKeyAndIVType.M101);
                        break;
                    }
                    if (LStrP02 == "1" && LStrP01 == "0")
                    {
                        LStrP03 = LXmlNodeLicServer.Attributes["P03"].Value;
                        LStrP03 = EncryptionAndDecryption.EncryptDecryptString(LStrP03, LStrVerificationCode101, EncryptionAndDecryption.UMPKeyAndIVType.M101);
                        LStrP04 = LXmlNodeLicServer.Attributes["P04"].Value;
                        LStrP04 = EncryptionAndDecryption.EncryptDecryptString(LStrP04, LStrVerificationCode101, EncryptionAndDecryption.UMPKeyAndIVType.M101);
                        LStrP05 = LXmlNodeLicServer.Attributes["P05"].Value;
                        LStrP05 = EncryptionAndDecryption.EncryptDecryptString(LStrP05, LStrVerificationCode101, EncryptionAndDecryption.UMPKeyAndIVType.M101);
                    }

                    LXmlNodeLicServer = LXmlNodeLicServer.NextSibling;
                }

              return  GetLicenseMessageInfo(LStrP03, LStrP04);
            }
            catch (Exception ex)
            {
                WriteLog(LogMode.Error, "GetLicenseInfo()\n" + ex.Message + "\n");
                return false;
            }
        }

        private void mLicHelper_LicInfoChanged(List<License> listLics)
        {
            try
            {
                WriteLog(LogMode.Info, string.Format("LicInfo changed"));
                if (listLics != null)
                {
                    for (int i = 0; i < listLics.Count; i++)
                    {
                        WriteLog(LogMode.Info, string.Format("LicInfo:\t{0}", listLics[i]));
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLog(LogMode.Error, string.Format("mLicHelper_LicInfoChanged() {0}", ex.Message));
            }
        }

        private void Write2Server03Xml()
        {
            string LStrXmlFileName = string.Empty;
            string LStrP00 = string.Empty, LStrP01 = string.Empty, LStrP02 = string.Empty, LStrP03 = string.Empty, LStrP04 = string.Empty, LStrP05 = string.Empty;
            string LStrVerificationCode001 = string.Empty;

            try
            {
                LStrVerificationCode001 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M001);
                LStrXmlFileName = System.IO.Path.Combine(IStrBaseDirectory, @"GlobalSettings\UMP.Server.03.xml");
                XmlDocument LXmlDocServer03 = new XmlDocument();
                LXmlDocServer03.Load(LStrXmlFileName);

                XmlNodeList LXmlNodeListUMPSetted = LXmlDocServer03.SelectSingleNode("UMPSetted").ChildNodes;
                foreach (XmlNode LXmlNodeSingleSetted in LXmlNodeListUMPSetted)
                {
                    LStrP00 = LXmlNodeSingleSetted.Attributes["P00"].Value;
                    if (LStrP00 != "0000") { continue; }
                    LXmlNodeSingleSetted.Attributes["P01"].Value = EncryptionAndDecryption.EncryptDecryptString("P01" + IIntP01.ToString(), LStrVerificationCode001, EncryptionAndDecryption.UMPKeyAndIVType.M001);
                    LXmlNodeSingleSetted.Attributes["P02"].Value = EncryptionAndDecryption.EncryptDecryptString("P02" + IIntP02.ToString(), LStrVerificationCode001, EncryptionAndDecryption.UMPKeyAndIVType.M001);
                    LXmlNodeSingleSetted.Attributes["P03"].Value = EncryptionAndDecryption.EncryptDecryptString("P03" + IStrP03, LStrVerificationCode001, EncryptionAndDecryption.UMPKeyAndIVType.M001);
                    LXmlNodeSingleSetted.Attributes["P04"].Value = EncryptionAndDecryption.EncryptDecryptString("P04" + IStrIsReadedLicense, LStrVerificationCode001, EncryptionAndDecryption.UMPKeyAndIVType.M001);
                    if (IStrIsReadedLicense == "1") { LXmlNodeSingleSetted.Attributes["P05"].Value = EncryptionAndDecryption.EncryptDecryptString("P05" + DateTime.UtcNow.ToString("G"), LStrVerificationCode001, EncryptionAndDecryption.UMPKeyAndIVType.M001); }
                    break;
                }
                LXmlDocServer03.Save(LStrXmlFileName);
            }
            catch (Exception ex)
            {
                WriteLog(LogMode.Error, "Write2Server03Xml()\n" + ex.Message + "\n");
            }
        }

        private void ReadLicenseInformation()
        {
            string LStrCurrentMode = string.Empty;
            string LStrDogNumber = string.Empty;
            bool bEnable = true;

            try
            {
                int iStartTime = System.Environment.TickCount;
                int iDifference = 0;
                
                while (IBoolCanContinue)
                {
                    IBoolInReadLicense = true;
                    int iEndTime = System.Environment.TickCount;
                    if ((iEndTime - iStartTime) > iDifference)
                    {
                        iStartTime = System.Environment.TickCount;
                        bEnable = true;
                        IStrIsReadedLicense = "0";
                    }

                    if (bEnable)
                    {
                        #region 判断是否数据库是否已经配置

                        if (IIntDBType == 0 || string.IsNullOrEmpty(IStrDBConnectProfile))
                        {
                            Thread.Sleep(100);
                            IBoolInReadLicense = false;
                            continue;
                        }

                        #endregion

                        IBoolGetLicense = GetLicenseInfo();
                        GetSettedLicenseServer();
                        if (ReadLicenseDogNumber())
                        {
                            ReadLicenseDetail();
                        }

                        if (IBoolGetLicense)
                        {
                            iDifference = 8*60*60*1000;
                            WriteLog(LogMode.Info,
                                "ReadLicenseInformation() Read license after 8 hours to get success : " + iDifference);
                        }
                        else
                        {
                            iDifference = 5*60*1000;
                            WriteLog(LogMode.Error,
                                "ReadLicenseInformation() After 5 minutes to get license to read failure : " +
                                iDifference);
                        }
                        bEnable = false;
                    }

                    IBoolInReadLicense = false;
                    Thread.Sleep(100);  
                }
            }
            catch (Exception ex)
            {
                WriteLog(LogMode.Error, "ReadLicenseInformation()\n" + ex.Message);
            }
        }

        private bool ReadLicenseDogNumber()
        {
            string LStrVerificationCode001 = string.Empty;

            try
            {
                LStrVerificationCode001 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M001);
                if (string.IsNullOrEmpty(IStrLicenseNum))
                {
                    IStrLicenseNum = "YoungLic00";
                    return false;
                }

                WriteLog(LogMode.Info, "Readed License Dog Number : " + IStrLicenseNum);

                IStrLicenseNum = EncryptionAndDecryption.EncryptDecryptString(IStrLicenseNum, LStrVerificationCode001, EncryptionAndDecryption.UMPKeyAndIVType.M001);
                string[] LStrArrayDogNumber = new string[1];
                LStrArrayDogNumber[0] = IStrLicenseNum;
                File.WriteAllLines(System.IO.Path.Combine(IStrBaseDirectory, "GlobalSettings", "UMP.Young.01"), LStrArrayDogNumber);
                Thread.Sleep(100);
            }
            catch (Exception ex)
            {
                WriteLog(LogMode.Error, "ReadLicenseDogNumber()\n" + ex.Message);
                return false;
            }
            return true;
        }

        private bool ReadLicenseDetail( )
        {
            string LStrVerificationCode101 = string.Empty;
            string LStrVerificationCode002 = string.Empty;
            string LStrVerificationCode102 = string.Empty;
            string LStrReadMessage = string.Empty;

            string LStrDynamicSQL = string.Empty;
            string LStrSelectSQL = string.Empty;

            string LStr11003C002 = string.Empty;
            string LStr11003C002Add = string.Empty;
            string LStr11003C008 = string.Empty;
            string LStr11003C008Hash8 = string.Empty;
            string LStr11003C007 = string.Empty;
            string LStr11003C006 = string.Empty;

            Int64 LInt64LicenseID = 0;
            string LStrLicenseValue = string.Empty;

            DatabaseOperation01Return LDBOperationReturn;
            try
            {
                DataOperations01 LDataOperation = new DataOperations01();
                LStrVerificationCode101 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M101);
                LStrVerificationCode002 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M002);
                LStrVerificationCode102 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M102);

                IStrLicenseNum = EncryptionAndDecryption.EncryptDecryptString(IStrLicenseNum, LStrVerificationCode101, EncryptionAndDecryption.UMPKeyAndIVType.M101);

                #region 读取系统T_11_003信息
                if (IIntDBType == 2)
                {
                    LStrDynamicSQL = "WITH CTE AS\n";
                    LStrDynamicSQL += "(\n";
                    LStrDynamicSQL += "SELECT *,1 AS LEVEL FROM T_11_003_00000  WHERE C002 IN (SELECT C002 FROM T_11_003_00000 WHERE C003='0')\n";
                    LStrDynamicSQL += "UNION ALL\n";
                    LStrDynamicSQL += "SELECT T.*,CTE.LEVEL+1 FROM T_11_003_00000 T,CTE WHERE CTE.C002=T.C003\n";
                    LStrDynamicSQL += ")\n";
                    LStrDynamicSQL += "SELECT C002 + 1000000000 C002, C003\n";
                    LStrDynamicSQL += "FROM CTE\n";
                    LStrDynamicSQL += "ORDER BY CONVERT(VARCHAR(20),CTE.C002)";
                }
                if (IIntDBType == 3)
                {
                    LStrDynamicSQL = "SELECT C002 + 1000000000 C002, C003 FROM T_11_003_00000 START WITH C003 = 0 CONNECT BY PRIOR C002 = C003";
                }
                LDBOperationReturn = LDataOperation.SelectDataByDynamicSQL(IIntDBType, IStrDBConnectProfile, LStrDynamicSQL);
                if (IStrIsReadedLicense == "1")
                {
                    foreach (DataRow LDataRowSingleFO in LDBOperationReturn.DataSetReturn.Tables[0].Rows)
                    {
                        LStr11003C002Add = LDataRowSingleFO[0].ToString();
                        LStr11003C002 = (Int64.Parse(LStr11003C002Add) - 1000000000).ToString();
                        LStr11003C008 = EncryptionAndDecryption.EncryptDecryptString(LStr11003C002Add + IStrLicenseNum, LStrVerificationCode002, EncryptionAndDecryption.UMPKeyAndIVType.M002);
                        LStrDynamicSQL = "UPDATE T_11_003_00000 SET C008 = '" + LStr11003C008 + "' WHERE C002 = " + LStr11003C002;
                        LDataOperation.ExecuteDynamicSQL(IIntDBType, IStrDBConnectProfile, LStrDynamicSQL);

                        //LStr11003C008 = EncryptionAndDecryption.EncryptDecryptString(LStr11003C002Add + AStrDogNumber, LStrVerificationCode002, EncryptionAndDecryption.UMPKeyAndIVType.M002);
                        LStr11003C008Hash8 = CreateMD5HashString(LStr11003C008).Substring(0, 8);
                        LStr11003C007 = LStr11003C002Add + AscCodeToChr(27) + "N";
                        LStr11003C007 = EncryptionAndDecryption.EncryptStringYKeyIV(LStr11003C007, LStr11003C008Hash8, LStr11003C008Hash8);

                        LStrDynamicSQL = "UPDATE T_11_003_00000 SET C007 = '" + LStr11003C007 + "' WHERE C002 = " + LStr11003C002;
                        LDataOperation.ExecuteDynamicSQL(IIntDBType, IStrDBConnectProfile, LStrDynamicSQL);
                    }
                }
                #endregion

                #region 正常连接到License Server，读取具体信息
                if (IStrIsReadedLicense == "1")
                {
                    WriteLog(LogMode.Warn, "ReadLicenseDetail()\n" + IStrLicenseNum + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                    WriteLog(LogMode.Warn, "LicenseInfo\n" + IStrLicenseInfor + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                    char[] chOptType = IStrLicenseInfor.ToCharArray();

                    if (IIntDBType == 2)
                    {
                        LStrSelectSQL = "SELECT * FROM T_11_003_00000";
                    }
                    if (IIntDBType == 3)
                    {
                        LStrSelectSQL = "SELECT * FROM T_11_003_00000";
                    }

                    LDBOperationReturn = LDataOperation.SelectDataByDynamicSQL(IIntDBType, IStrDBConnectProfile, LStrSelectSQL);

                    if (!LDBOperationReturn.BoolReturn)
                    {
                        WriteLog(LogMode.Error, "UpdataTableData21001C033()\n" + LDBOperationReturn.StrReturn);
                        return false;
                    }

                    for (int i = 0; i < LDBOperationReturn.DataSetReturn.Tables[0].Rows.Count; i++)
                    {
                        DataRow LDataRowSingleFO = LDBOperationReturn.DataSetReturn.Tables[0].Rows[i];
                        int intOptNum = Convert.ToInt32(LDataRowSingleFO["C016"]);
                        LStr11003C002 = LDataRowSingleFO["C002"].ToString();
                        if (intOptNum > 0)
                        {
                            LStr11003C006 = EncryptionAndDecryption.EncryptDecryptString(LDataRowSingleFO["C006"].ToString(), LStrVerificationCode102, EncryptionAndDecryption.UMPKeyAndIVType.M102);
                            string[] strTemp = LStr11003C006.Split(AscCodeToChr(27).ToArray());
                            if (strTemp[2] == "Y")
                            {
                                long longValue;
                                if (long.TryParse(LStr11003C002, out longValue))
                                {
                                    longValue = longValue + 1000000000;
                                    if (longValue <= 1000000000) { continue; }
                                    LStrLicenseValue = chOptType[intOptNum].ToString();
                                    LStr11003C008 = EncryptionAndDecryption.EncryptDecryptString(longValue + IStrLicenseNum, LStrVerificationCode002, EncryptionAndDecryption.UMPKeyAndIVType.M002);
                                    LStr11003C008Hash8 = CreateMD5HashString(LStr11003C008).Substring(0, 8);
                                    LStr11003C007 = longValue + AscCodeToChr(27) + LStrLicenseValue;
                                    LStr11003C007 = EncryptionAndDecryption.EncryptStringYKeyIV(LStr11003C007, LStr11003C008Hash8, LStr11003C008Hash8);
                                    LStrDynamicSQL = "UPDATE T_11_003_00000 SET C007 = '" + LStr11003C007 + "' WHERE C002 = " + LStr11003C002;
                                    LDataOperation.ExecuteDynamicSQL(IIntDBType, IStrDBConnectProfile, LStrDynamicSQL);
                                    if (LStrLicenseValue == "Y") { UpdateParentFO(longValue.ToString(), IStrLicenseNum, LDBOperationReturn.DataSetReturn.Tables[0]); }
                                }
                            }
                        }
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                WriteLog(LogMode.Error, "ReadLicenseDetail()\n" + ex.Message + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                return false;
            }
            return true;
        }

//         private bool ReadLicenseDogNumber(ref string AStrDogNumber)
//         {
//             TcpClient LTcpClient = null;
//             SslStream LSslStream = null;
//             string LStrVerificationCode004 = string.Empty;
//             string LStrVerificationCode001 = string.Empty;
//             string LStrSendMessage = string.Empty;
//             string LStrReadMessage = string.Empty;
// 
//             string LStrDogNumber = string.Empty;
// 
//             try
//             {
//                 if (IStrIsReadedLicense == "1")
//                 {
//                     LStrVerificationCode001 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M001);
//                     LStrVerificationCode004 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004);
//                     LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString("G010", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
// 
//                     LTcpClient = new TcpClient();
//                     LTcpClient.SendTimeout = 10000;
//                     LTcpClient.Connect("127.0.0.1", 8009);
//                     LSslStream = new SslStream(LTcpClient.GetStream(), false, new RemoteCertificateValidationCallback(ValidateServerCertificate), null);
//                     LSslStream.AuthenticateAsClient("VoiceCyber.PF", null, SslProtocols.Default, false);
//                     byte[] LByteMesssage = Encoding.UTF8.GetBytes(LStrSendMessage + "\r\n");
//                     LSslStream.Write(LByteMesssage); LSslStream.Flush();
//                     if (ReadMessageFromServer(LSslStream, ref LStrReadMessage))
//                     {
//                         if (LStrReadMessage.Contains("1200000"))
//                         {
//                             LStrReadMessage = LStrReadMessage.Replace("\r\n", "");
//                             string[] LStrReadedLicense = LStrReadMessage.Split(AscCodeToChr(27).ToArray(), StringSplitOptions.RemoveEmptyEntries);
//                             foreach (string LStrSingleLicense in LStrReadedLicense)
//                             {
//                                 if (LStrSingleLicense.Substring(0, 7) == "1200000") { 
//                                     LStrDogNumber = LStrSingleLicense.Substring(8);
//                                 }
//                             }
//                         }
//                     }
//                 }
// 
//                 if (string.IsNullOrEmpty(LStrDogNumber)) { 
//                     LStrDogNumber = "YoungLic00";
//                     return false;
//                 }
//                 AStrDogNumber = LStrDogNumber;
// 
//                 WriteLog(LogMode.Info, "Readed License Dog Number : " + LStrDogNumber );
// 
//                 LStrDogNumber = EncryptionAndDecryption.EncryptDecryptString(LStrDogNumber, LStrVerificationCode001, EncryptionAndDecryption.UMPKeyAndIVType.M001);
//                 string[] LStrArrayDogNumber = new string[1];
//                 LStrArrayDogNumber[0] = LStrDogNumber;
//                 File.WriteAllLines(System.IO.Path.Combine(IStrBaseDirectory, "GlobalSettings", "UMP.Young.01"), LStrArrayDogNumber);
//                 Thread.Sleep(100);
//             }
//             catch (Exception ex)
//             {
//                 WriteLog(LogMode.Error, "ReadLicenseDogNumber()\n" + ex.Message);
//                 return false;
//             }
//             finally
//             {
//                 if (LSslStream != null) { LSslStream.Close(); LSslStream = null; }
//                 if (LTcpClient != null) { LTcpClient.Close(); LTcpClient = null; }
//             }
//             return true;
//         }

        private bool ReadLicenseDetail(string AStrDogNumber)
        {
            TcpClient LTcpClient = null;
            SslStream LSslStream = null;

            string LStrVerificationCode002 = string.Empty;
            string LStrVerificationCode004 = string.Empty;
            string LStrVerificationCode102 = string.Empty;
            string LStrSendMessage = string.Empty;
            string LStrReadMessage = string.Empty;

            string LStrDynamicSQL = string.Empty;
            string LStrSelectSQL = string.Empty;

            string LStr11003C002 = string.Empty;
            string LStr11003C002Add = string.Empty;
            string LStr11003C008 = string.Empty;
            string LStr11003C008Hash8 = string.Empty;
            string LStr11003C007 = string.Empty;
            string LStr11003C006 = string.Empty;

            string LStrLicenseID = string.Empty;
            Int64 LInt64LicenseID = 0;
            string LStrLicenseValue = string.Empty;

            DatabaseOperation01Return LDBOperationReturn;
            try
            {
                DataOperations01 LDataOperation = new DataOperations01();
                LStrVerificationCode002 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M002);
                LStrVerificationCode102 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M102);

                #region 读取系统T_11_003信息
                if (IIntDBType == 2)
                {
                    LStrDynamicSQL = "WITH CTE AS\n";
                    LStrDynamicSQL += "(\n";
                    LStrDynamicSQL += "SELECT *,1 AS LEVEL FROM T_11_003_00000  WHERE C002 IN (SELECT C002 FROM T_11_003_00000 WHERE C003='0')\n";
                    LStrDynamicSQL += "UNION ALL\n";
                    LStrDynamicSQL += "SELECT T.*,CTE.LEVEL+1 FROM T_11_003_00000 T,CTE WHERE CTE.C002=T.C003\n";
                    LStrDynamicSQL += ")\n";
                    LStrDynamicSQL += "SELECT C002 + 1000000000 C002, C003\n";
                    LStrDynamicSQL += "FROM CTE\n";
                    LStrDynamicSQL += "ORDER BY CONVERT(VARCHAR(20),CTE.C002)";
                }
                if (IIntDBType == 3)
                {
                    LStrDynamicSQL = "SELECT C002 + 1000000000 C002, C003 FROM T_11_003_00000 START WITH C003 = 0 CONNECT BY PRIOR C002 = C003";
                }
                LDBOperationReturn = LDataOperation.SelectDataByDynamicSQL(IIntDBType, IStrDBConnectProfile, LStrDynamicSQL);
                if (IStrIsReadedLicense == "1")
                {
                    foreach (DataRow LDataRowSingleFO in LDBOperationReturn.DataSetReturn.Tables[0].Rows)
                    {
                        LStr11003C002Add = LDataRowSingleFO[0].ToString();
                        LStr11003C002 = (Int64.Parse(LStr11003C002Add) - 1000000000).ToString();
                        LStr11003C008 = EncryptionAndDecryption.EncryptDecryptString(LStr11003C002Add + AStrDogNumber, LStrVerificationCode002, EncryptionAndDecryption.UMPKeyAndIVType.M002);
                        LStrDynamicSQL = "UPDATE T_11_003_00000 SET C008 = '" + LStr11003C008 + "' WHERE C002 = " + LStr11003C002;
                        LDataOperation.ExecuteDynamicSQL(IIntDBType, IStrDBConnectProfile, LStrDynamicSQL);

                        //LStr11003C008 = EncryptionAndDecryption.EncryptDecryptString(LStr11003C002Add + AStrDogNumber, LStrVerificationCode002, EncryptionAndDecryption.UMPKeyAndIVType.M002);
                        LStr11003C008Hash8 = CreateMD5HashString(LStr11003C008).Substring(0, 8);
                        LStr11003C007 = LStr11003C002Add + AscCodeToChr(27) + "N";
                        LStr11003C007 = EncryptionAndDecryption.EncryptStringYKeyIV(LStr11003C007, LStr11003C008Hash8, LStr11003C008Hash8);

                        LStrDynamicSQL = "UPDATE T_11_003_00000 SET C007 = '" + LStr11003C007 + "' WHERE C002 = " + LStr11003C002;
                        LDataOperation.ExecuteDynamicSQL(IIntDBType, IStrDBConnectProfile, LStrDynamicSQL);
                    }
                }
                #endregion

                #region 正常连接到License Server，读取具体信息
                if (IStrIsReadedLicense == "1")
                {
                    LStrVerificationCode004 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString("G009", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);

                    LTcpClient = new TcpClient();
                    LTcpClient.SendTimeout = 10000;
                    LTcpClient.Connect("127.0.0.1", 8009);
                    LSslStream = new SslStream(LTcpClient.GetStream(), false, new RemoteCertificateValidationCallback(ValidateServerCertificate), null);
                    LSslStream.AuthenticateAsClient("VoiceCyber.PF", null, SslProtocols.Default, false);
                    byte[] LByteMesssage = Encoding.UTF8.GetBytes(LStrSendMessage + "\r\n");
                    LSslStream.Write(LByteMesssage); LSslStream.Flush();
                    if (ReadMessageFromServer(LSslStream, ref LStrReadMessage))
                    {
                        WriteLog(LogMode.Warn, "ReadLicenseDetail()\n" + AStrDogNumber+DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                        WriteLog(LogMode.Warn, "ReadMessageFromServer()\n" + LStrReadMessage + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));

                        string[] LStrReadedLicense = LStrReadMessage.Split(AscCodeToChr(27).ToArray(), StringSplitOptions.RemoveEmptyEntries);
                        string[] LStrArrayLicenseInfo = LStrReadedLicense[0].ToString().Split(AscCodeToChr(30).ToArray());
                        LStrLicenseID = LStrArrayLicenseInfo[0];
                        char[] chOptType = LStrArrayLicenseInfo[1].ToCharArray();
                        
                        if (IIntDBType == 2)
                        {
                            LStrSelectSQL = "SELECT * FROM T_11_003_00000";
                        }
                        if (IIntDBType == 3)
                        {
                            LStrSelectSQL = "SELECT * FROM T_11_003_00000";
                        }

                        LDBOperationReturn = LDataOperation.SelectDataByDynamicSQL(IIntDBType, IStrDBConnectProfile, LStrSelectSQL);

                        if (!LDBOperationReturn.BoolReturn)
                        {
                            WriteLog(LogMode.Error, "UpdataTableData21001C033()\n" + LDBOperationReturn.StrReturn);
                            return false;
                        }

                        for (int i = 0; i < LDBOperationReturn.DataSetReturn.Tables[0].Rows.Count; i++)
                        {
                            DataRow LDataRowSingleFO = LDBOperationReturn.DataSetReturn.Tables[0].Rows[i];
                            int intOptNum = Convert.ToInt32(LDataRowSingleFO["C016"]);
                            LStr11003C002 = LDataRowSingleFO["C002"].ToString();
                            if (intOptNum > 0)
                            {
                                LStr11003C006 = EncryptionAndDecryption.EncryptDecryptString(LDataRowSingleFO["C006"].ToString(), LStrVerificationCode102, EncryptionAndDecryption.UMPKeyAndIVType.M102);
                                string[] strTemp = LStr11003C006.Split(AscCodeToChr(27).ToArray());
                                if (strTemp[2] == "Y")
                                {
                                    if (string.IsNullOrEmpty(LStrLicenseID)) { continue; }
                                    long longValue;
                                    if (long.TryParse(LStr11003C002, out longValue))
                                    {
                                        longValue = longValue + 1000000000;
                                        if (longValue <= 1000000000) { continue; }
                                        LStrLicenseValue = chOptType[intOptNum].ToString();
                                        LStr11003C008 = EncryptionAndDecryption.EncryptDecryptString(longValue + AStrDogNumber, LStrVerificationCode002, EncryptionAndDecryption.UMPKeyAndIVType.M002);
                                        LStr11003C008Hash8 = CreateMD5HashString(LStr11003C008).Substring(0, 8);
                                        LStr11003C007 = longValue + AscCodeToChr(27) + LStrLicenseValue;
                                        LStr11003C007 = EncryptionAndDecryption.EncryptStringYKeyIV(LStr11003C007, LStr11003C008Hash8, LStr11003C008Hash8);
                                        LStrDynamicSQL = "UPDATE T_11_003_00000 SET C007 = '" + LStr11003C007 + "' WHERE C002 = " + LStr11003C002;
                                        LDataOperation.ExecuteDynamicSQL(IIntDBType, IStrDBConnectProfile, LStrDynamicSQL);
                                        if (LStrLicenseValue == "Y") { UpdateParentFO(longValue.ToString(), AStrDogNumber, LDBOperationReturn.DataSetReturn.Tables[0]); }
                                    }
                                }
                            }  
                        }
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                WriteLog(LogMode.Error, "ReadLicenseDetail()\n" + ex.Message + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") );
                return false;
            }
            finally
            {
                if (LSslStream != null) { LSslStream.Close(); LSslStream = null; }
                if (LTcpClient != null) { LTcpClient.Close(); LTcpClient = null; }
            }
            return true;
        }

        private void UpdateParentFO(string AStrCurrentLicenseID, string AStrDogNumber, DataTable ADataTalbe11003)
        {
            string LStr11003C003 = string.Empty;
            Int64 LInt11003C003 = 0;
            string LStr11003C008 = string.Empty;
            string LStr11003C008Hash8 = string.Empty;
            string LStr11003C007 = string.Empty;

            string LStrDynamicSQL = string.Empty;
            string LStrVerificationCode002 = string.Empty;

            try
            {
                DataOperations01 LDataOperation = new DataOperations01();
                LStrVerificationCode002 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M002);

                DataRow[] LDataRowSelected = ADataTalbe11003.Select("C002 = " + AStrCurrentLicenseID);
                if (LDataRowSelected.Length > 0)
                {
                    LStr11003C003 = LDataRowSelected[0]["C003"].ToString();
                    LInt11003C003 = Int64.Parse(LStr11003C003) + 1000000000;

                    LStr11003C008 = EncryptionAndDecryption.EncryptDecryptString(LInt11003C003.ToString() + AStrDogNumber, LStrVerificationCode002, EncryptionAndDecryption.UMPKeyAndIVType.M002);
                    LStr11003C008Hash8 = CreateMD5HashString(LStr11003C008).Substring(0, 8);
                    LStr11003C007 = LInt11003C003.ToString() + AscCodeToChr(27) + "Y";
                    LStr11003C007 = EncryptionAndDecryption.EncryptStringYKeyIV(LStr11003C007, LStr11003C008Hash8, LStr11003C008Hash8);
                    LStrDynamicSQL = "UPDATE T_11_003_00000 SET C007 = '" + LStr11003C007 + "' WHERE C002 = " + LStr11003C003;
                    LDataOperation.ExecuteDynamicSQL(IIntDBType, IStrDBConnectProfile, LStrDynamicSQL);
                    UpdateParentFO(LInt11003C003.ToString(), AStrDogNumber, ADataTalbe11003);
                }
            }
            catch (Exception ex)
            {
                WriteLog(LogMode.Error, "UpdateParentFO()\n CurrentLicenseID = " + AStrCurrentLicenseID + "\n" + ex.Message);
            }
        }
        #endregion

        #region 处理本地回删录音的信息
        private void ActiveUpdate21001033()
        {
            // 0-操作码
            // 1-媒体类型 1，2
            // 2-月份 YYYYMM
            // 3-机器名
            // 4-IP地址列表  IP+ CHAR(30) + IP .....
            List<string> LListStrCondition = new List<string>();

            try
            {
                while (IBoolCanContinue)
                {
                    IBoolInUpdate21001033 = true;

                    #region 判断是否数据库是否已经配置
                    if (IIntDBType == 0 || string.IsNullOrEmpty(IStrDBConnectProfile))
                    {
                        IBoolInUpdate21001033 = false; Thread.Sleep(2000); continue;
                    }
                    #endregion

                    #region 循环从队列中获取一个对象
                    lock (ILockObjectUpdate21001033)
                    {
                        if (!IYoungCircleQueueUpdate21001033.CircleQueueIsEmpty())
                        {
                            LListStrCondition = IYoungCircleQueueUpdate21001033.PopElement();
                        }
                        else
                        {
                            LListStrCondition.Clear();
                        }
                    }
                    if (LListStrCondition.Count <= 0) { Thread.Sleep(500); IBoolInUpdate21001033 = false; continue; }
                    #endregion
                    
                    string[] LStrArrayConditons = LListStrCondition.ToArray();

                    for (int LIntLoop = 1; LIntLoop < LStrArrayConditons.Length; LIntLoop++)
                    {
                        WriteLog(LogMode.Info, "ActiveUpdate21001033() " + LIntLoop.ToString() + " = " + LStrArrayConditons[LIntLoop] );
                    }
                    UpdataTableData21001C033(LStrArrayConditons);

                    Thread.Sleep(500);
                    IBoolInUpdate21001033 = false;
                }
            }
            catch (Exception ex)
            {
                WriteLog(LogMode.Error, "ActiveUpdate21001033()\n" + ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="AStrArrayConditons">
        /// 0-操作码
        /// 1-媒体类型 1，2
        /// 2-月份 YYYYMM
        /// 3-机器名
        /// 4-IP地址列表  IP+ CHAR(30) + IP .....
        /// </param>
        private void UpdataTableData21001C033(string[] AStrArrayConditons)
        {
            string LStrSelectSQL = string.Empty;
            string LStrUpdateSQL = string.Empty;
            //List<string> LListStrUpdateSQL = new List<string>();
            string LStrTableName = string.Empty;
            string LStrBeginC006 = string.Empty;
            string LStrEndC006 = string.Empty;

            try
            {
                DataOperations01 LDataOperation = new DataOperations01();
                DatabaseOperation01Return LDBOperationReturn = new DatabaseOperation01Return();

                if (IIntDBType == 2)
                {
                    LStrSelectSQL = "SELECT NAME FROM SYSOBJECTS WHERE NAME LIKE 'T_21_001%'";
                }
                if (IIntDBType == 3)
                {
                    LStrSelectSQL = "SELECT TABLE_NAME FROM USER_TABLES WHERE TABLE_NAME LIKE 'T_21_001%'";
                }
                LDBOperationReturn = LDataOperation.SelectDataByDynamicSQL(IIntDBType, IStrDBConnectProfile, LStrSelectSQL);
                if (!LDBOperationReturn.BoolReturn)
                {
                    WriteLog(LogMode.Error, "UpdataTableData21001C033()\n" + LDBOperationReturn.StrReturn );
                    return;
                }
                LStrBeginC006 = AStrArrayConditons[2] + "00000000";
                LStrEndC006 = AStrArrayConditons[2] + "99999999";
                string[] LStrArrayIPAddress = AStrArrayConditons[4].Split(AscCodeToChr(30).ToArray(), StringSplitOptions.RemoveEmptyEntries);
                List<string> LListUpdateTarget = LStrArrayIPAddress.ToList();
                LListUpdateTarget.Add(AStrArrayConditons[3]);
                foreach (DataRow LDataRowSingleTable in LDBOperationReturn.DataSetReturn.Tables[0].Rows)
                {
                    LStrTableName = LDataRowSingleTable[0].ToString().Trim();
                    
                    LStrUpdateSQL = "UPDATE " + LStrTableName + " SET C033 = 'Y' WHERE C006 > " + LStrBeginC006 + " AND C006 < " + LStrEndC006;
                    if (LStrTableName.Length > 14)
                    {
                        if (LStrTableName.Substring(15, 4) != AStrArrayConditons[2].Substring(2)) { continue; }
                    }
                    foreach (string LStrSingleTarget in LListUpdateTarget)
                    {
                        LDBOperationReturn = LDataOperation.ExecuteDynamicSQL(IIntDBType, IStrDBConnectProfile, LStrUpdateSQL + " AND C020 = '" + LStrSingleTarget + "' AND C014 = " + AStrArrayConditons[1]);
                        if (!LDBOperationReturn.BoolReturn)
                        {
                            WriteLog(LogMode.Error, "UpdataTableData21001C033()\n" + LDBOperationReturn.StrReturn + "\n" + LStrUpdateSQL + " AND C020 = '" + LStrSingleTarget + "' AND C014 = " + AStrArrayConditons[1] );
                        }
                        else
                        {
                            WriteLog(LogMode.Info, "UpdataTableData21001C033()\nEffect Row(s):" + LDBOperationReturn.StrReturn + "\n" + LStrUpdateSQL + " AND C020 = '" + LStrSingleTarget + "' AND C014 = " + AStrArrayConditons[1]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLog(LogMode.Error, "UpdataTableData21001C033()\n" + ex.Message);
            }
        }
        #endregion

        #region 处理归档回删信息
        private void ActiveUpdate2100123X()
        {
            List<string> LListStrCondition = new List<string>();

            try
            {
                while (IBoolCanContinue)
                {
                    IBoolInUpdate2100123X = true;

                    #region 判断是否数据库是否已经配置
                    if (IIntDBType == 0 || string.IsNullOrEmpty(IStrDBConnectProfile))
                    {
                        IBoolInUpdate2100123X = false; Thread.Sleep(2000); continue;
                    }
                    #endregion

                    #region 循环从队列中获取一个对象
                    lock (ILockObjectUpdate2100123X)
                    {
                        if (!IYoungCircleQueueUpdate2100123X.CircleQueueIsEmpty())
                        {
                            LListStrCondition = IYoungCircleQueueUpdate2100123X.PopElement();
                        }
                        else
                        {
                            LListStrCondition.Clear();
                        }
                    }
                    if (LListStrCondition.Count <= 0) { Thread.Sleep(500); IBoolInUpdate2100123X = false; continue; }
                    #endregion

                    string[] LStrArrayConditons = LListStrCondition.ToArray();

                    for (int LIntLoop = 1; LIntLoop < LStrArrayConditons.Length; LIntLoop++)
                    {
                       WriteLog(LogMode.Info, "ActiveUpdate2100123X() " + LIntLoop.ToString() + " = " + LStrArrayConditons[LIntLoop] );
                    }
                    UpdataTableData21001C23X(LStrArrayConditons);

                    Thread.Sleep(500);
                    IBoolInUpdate2100123X = false;
                }
            }
            catch (Exception ex)
            {
                WriteLog(LogMode.Error, "ActiveUpdate2100123X()\n" + ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="AStrArrayConditons">
        /// 0-操作码
        /// 1-媒体类型 1，2
        /// 2-月份 YYYYMM
        /// 3-空值
        /// 4-存储设备ID（10位） .....
        /// </param>
        private void UpdataTableData21001C23X(string[] AStrArrayConditons)
        {
            string LStrSelectSQL = string.Empty;
            string LStrUpdateSQL = string.Empty;
            string LStrTableName = string.Empty;
            string LStrColumnName = string.Empty;

            string LStrBeginC006 = string.Empty;
            string LStrEndC006 = string.Empty;

            try
            {
                DataOperations01 LDataOperation = new DataOperations01();
                DatabaseOperation01Return LDBOperationReturn = new DatabaseOperation01Return();

                if (IIntDBType == 2)
                {
                    LStrSelectSQL = "SELECT NAME FROM SYSOBJECTS WHERE NAME LIKE 'T_21_001%'";
                }
                if (IIntDBType == 3)
                {
                    LStrSelectSQL = "SELECT TABLE_NAME FROM USER_TABLES WHERE TABLE_NAME LIKE 'T_21_001%'";
                }
                LDBOperationReturn = LDataOperation.SelectDataByDynamicSQL(IIntDBType, IStrDBConnectProfile, LStrSelectSQL);
                if (!LDBOperationReturn.BoolReturn)
                {
                    WriteLog(LogMode.Error, "UpdataTableData21001C23X()\n" + LDBOperationReturn.StrReturn );
                    return;
                }

                LStrBeginC006 = AStrArrayConditons[2] + "00000000";
                LStrEndC006 = AStrArrayConditons[2] + "99999999";

                foreach (DataRow LDataRowSingleTable in LDBOperationReturn.DataSetReturn.Tables[0].Rows)
                {
                    LStrTableName = LDataRowSingleTable[0].ToString().Trim();

                    if (LStrTableName.Length > 14)
                    {
                        if (LStrTableName.Substring(15, 4) != AStrArrayConditons[2].Substring(2)) { continue; }
                    }

                    for (int LIntLoopColumn = 231; LIntLoopColumn <= 240; LIntLoopColumn++)
                    {
                        LStrColumnName = "C" + LIntLoopColumn.ToString();
                        LStrUpdateSQL = "UPDATE " + LStrTableName + "SET " + LStrColumnName + " = " + LStrColumnName + " * (-1) WHERE " + LStrColumnName + " = " + AStrArrayConditons[4] + " AND C006 > " + LStrBeginC006 + " AND C006 < " + LStrEndC006;
                        LDBOperationReturn = LDataOperation.ExecuteDynamicSQL(IIntDBType, IStrDBConnectProfile, LStrUpdateSQL);
                        if (!LDBOperationReturn.BoolReturn)
                        {
                            WriteLog(LogMode.Error, "UpdataTableData21001C23X()\n" + LDBOperationReturn.StrReturn + "\n" + LStrUpdateSQL );
                        }
                        else
                        {
                            WriteLog(LogMode.Info, "UpdataTableData21001C23X()\nEffect Row(s):" + LDBOperationReturn.StrReturn + "\n" + LStrUpdateSQL );
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLog(LogMode.Error, "UpdataTableData21001C23X()\n" + ex.Message );
            }
        }
        #endregion

        #region 向数据库中写入操作日志
        private void ActiveInsert11901()
        {
            ClientOpArgsAndReturn LClientOpArgsAndReturn = new ClientOpArgsAndReturn();
            List<string> LListStrOperationArgs = new List<string>();
            string LStrSaveReturnMessage = string.Empty;
            string LStrVerificationCode004 = string.Empty;
            string LStrVerificationCode104 = string.Empty;

            try
            {
                while (IBoolCanContinue)
                {
                    #region 判断是否数据库是否已经配置
                    if (IIntDBType == 0 || string.IsNullOrEmpty(IStrDBConnectProfile))
                    {
                        IBoolInInsert11901 = false; Thread.Sleep(2000); continue;
                    }
                    #endregion
                    
                    IBoolInInsert11901 = true;

                    #region 循环从队列中获取一个对象
                    lock (ILock11901)
                    {
                        if (!IYoungCircleQueueWriteOperation.CircleQueueIsEmpty())
                        {
                            LClientOpArgsAndReturn = IYoungCircleQueueWriteOperation.PopElement();
                        }
                        else
                        {
                            LClientOpArgsAndReturn.LListArguments.Clear();
                        }
                    }
                    if (LClientOpArgsAndReturn.LListArguments.Count <= 0) { Thread.Sleep(500); IBoolInInsert11901 = false; continue; }
                    #endregion

                    /// 0：客户端SessionID
                    /// 1：模块ID                      2：功能操作编号                3：租户Token（5位）          4：操作用户ID 
                    /// 5：当前操作角色                6：机器名                      7：机器IP                    8：操作时间 UTC
                    /// 9：操作结果                    10：操作内容对应的语言包ID     11：替换参数                 12：异常错误

                    LListStrOperationArgs.Clear();

                    string[] LStrReturnCode = LClientOpArgsAndReturn.LStrReturnCode.Split(AscCodeToChr(27).ToArray(), StringSplitOptions.RemoveEmptyEntries);
                    LStrVerificationCode004 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    LStrVerificationCode104 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104);
                    for (int LIntLoop = 0; LIntLoop < LStrReturnCode.Length; LIntLoop++)
                    {
                        LStrReturnCode[LIntLoop] = EncryptionAndDecryption.EncryptDecryptString(LStrReturnCode[LIntLoop], LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                    }
                    string LStrOperationCode = LClientOpArgsAndReturn.LListArguments[0];
                    if (LStrOperationCode == "M01A01" && LStrReturnCode[0] == "S01A00") { LClientOpArgsAndReturn.LLongClientSessionID = long.Parse(LStrReturnCode[3]); }
                    //0
                    LListStrOperationArgs.Add(LClientOpArgsAndReturn.LLongClientSessionID.ToString());
                    //1
                    LListStrOperationArgs.Add("11");
                    //2
                    LListStrOperationArgs.Add(SocketOperartionTo11003004(LClientOpArgsAndReturn.LListArguments[0]));
                    //3
                    if (LStrOperationCode == "M01A01")
                    {
                        if (LStrReturnCode[0] == "S01A00") { LListStrOperationArgs.Add(LStrReturnCode[1]); } else { LListStrOperationArgs.Add("00000"); }
                    }
                    else
                    {
                        LListStrOperationArgs.Add(LClientOpArgsAndReturn.LListArguments[1]);
                    }
                    //4
                    if (LStrOperationCode == "M01A01")
                    {
                        if (LStrReturnCode[0] == "S01A00") { LListStrOperationArgs.Add(LStrReturnCode[2]); } else { LListStrOperationArgs.Add("0"); }
                    }
                    else
                    {
                        LListStrOperationArgs.Add(LClientOpArgsAndReturn.LListArguments[2]);
                    }
                    //5
                    LListStrOperationArgs.Add("0");
                    //6,7
                    if (LStrOperationCode == "M01A01")
                    {
                        LListStrOperationArgs.Add(LClientOpArgsAndReturn.LListArguments[5]);
                        LListStrOperationArgs.Add(LClientOpArgsAndReturn.LListArguments[6]);
                    }
                    else
                    {
                        string LStrHostName = string.Empty, LStrHostAddress = string.Empty;
                        GetHostNameAddressByLoginSessionID(LListStrOperationArgs[0], LListStrOperationArgs[3], ref LStrHostName, ref LStrHostAddress);
                        LListStrOperationArgs.Add(LStrHostName);
                        LListStrOperationArgs.Add(LStrHostAddress);
                    }
                    //8
                    LListStrOperationArgs.Add(LClientOpArgsAndReturn.LDateTimeOperation.ToString("G"));
                    //9
                    LListStrOperationArgs.Add(SocketReturnCodeTo11003009(LStrOperationCode, LStrReturnCode[0]));
                    //10
                    LListStrOperationArgs.Add("YA0000" + LStrReturnCode[0]);
                    //11
                    string LStrReplaceArguments = string.Empty;
                    if (LStrOperationCode == "M01A01")
                    {
                        LStrReplaceArguments = LClientOpArgsAndReturn.LListArguments[1] + AscCodeToChr(30) + AscCodeToChr(30) + AscCodeToChr(30) + EncryptionAndDecryption.EncryptDecryptString(LClientOpArgsAndReturn.LListArguments[2], LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    }
                    else if (LStrOperationCode == "M01A04")
                    {
                        string LStrOldPassword = string.Empty;
                        string LStrNewPassword = string.Empty;
                        LStrOldPassword = EncryptionAndDecryption.EncryptDecryptString(LClientOpArgsAndReturn.LListArguments[3], LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                        LStrNewPassword = EncryptionAndDecryption.EncryptDecryptString(LClientOpArgsAndReturn.LListArguments[4], LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                        LStrReplaceArguments = GetUserAccountByUserID(LClientOpArgsAndReturn.LListArguments[2], LClientOpArgsAndReturn.LListArguments[1]) + AscCodeToChr(30) + AscCodeToChr(30) + AscCodeToChr(30) + LStrOldPassword + AscCodeToChr(30) + AscCodeToChr(30) + AscCodeToChr(30) + LStrNewPassword;
                    }
                    else
                    {
                        LStrReplaceArguments = "";
                    }
                    LListStrOperationArgs.Add(LStrReplaceArguments);
                    //12
                    LListStrOperationArgs.Add(LClientOpArgsAndReturn.LStrReturnMessage);

                    S01BOperations LS01BOperations = new S01BOperations(IIntDBType, IStrDBConnectProfile);
                    if (!LS01BOperations.S01BOperation01(LListStrOperationArgs, ref LStrSaveReturnMessage)) { WriteLog(LogMode.Error, LStrSaveReturnMessage ); }

                    Thread.Sleep(500);
                    IBoolInInsert11901 = false;
                }
            }
            catch (Exception ex)
            {
                WriteLog(LogMode.Error, "ActiveInsert11901()\n" + ex.Message );
            }
        }

        private string SocketOperartionTo11003004(string AStrOperationID)
        {
            string LStrReturn = string.Empty;

            try
            {
                switch (AStrOperationID)
                {
                    case "M01A01":
                        LStrReturn = "110001";
                        break;
                    case "M01A02":
                        LStrReturn = "110002";
                        break;
                    case "M01A04":
                        LStrReturn = "110003";
                        break;
                    default:
                        LStrReturn = "110000";
                        break;
                }
            }
            catch { LStrReturn = "110000"; }

            return LStrReturn;
        }

        private string SocketReturnCodeTo11003009(string AStrOperationID, string AStrReturnCode)
        {
            string LStrReturn = string.Empty;

            try
            {
                switch(AStrOperationID)
                {
                    case "M01A01":
                        if (AStrReturnCode == "S01A00") { LStrReturn = "R1"; } else { LStrReturn = "R0"; }
                        break;
                    case "M01A02":
                        if (AStrReturnCode == "S01AA1") { LStrReturn = "R1"; } else { LStrReturn = "R0"; }
                        break;
                    case "M01A04":
                        if (AStrReturnCode == "S01A01") { LStrReturn = "R1"; } else { LStrReturn = "R0"; }
                        break;
                    default:
                        LStrReturn = "R3";
                        break;
                }
                
            }
            catch { LStrReturn = "R3"; }

            return LStrReturn;
        }

        private void GetHostNameAddressByLoginSessionID(string AStrSessionID, string AStrRentToken, ref string AStrHostName, ref string AStrHostAddress)
        {
            string LStrVerificationCode102 = string.Empty;
            string LStrSelectSQL = string.Empty;

            try
            {
                AStrHostName = "Host Name"; AStrHostAddress = "0.0.0.0";
                DataOperations01 LDataOperation = new DataOperations01();
                DatabaseOperation01Return LDBOperationReturn = new DatabaseOperation01Return();

                LStrVerificationCode102 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M102);
                LStrSelectSQL = "SELECT C004, C005 FROM T_11_002_" + AStrRentToken + " WHERE C006 = " + AStrSessionID;
                LDBOperationReturn = LDataOperation.SelectDataByDynamicSQL(IIntDBType, IStrDBConnectProfile, LStrSelectSQL);
                if (LDBOperationReturn.StrReturn == "1")
                {
                    AStrHostName = LDBOperationReturn.DataSetReturn.Tables[0].Rows[0][0].ToString();
                    AStrHostAddress = LDBOperationReturn.DataSetReturn.Tables[0].Rows[0][1].ToString();
                    AStrHostName = EncryptionAndDecryption.EncryptDecryptString(AStrHostName, LStrVerificationCode102, EncryptionAndDecryption.UMPKeyAndIVType.M102);
                    AStrHostAddress = EncryptionAndDecryption.EncryptDecryptString(AStrHostAddress, LStrVerificationCode102, EncryptionAndDecryption.UMPKeyAndIVType.M102);
                }
            }
            catch { AStrHostName = "Host Name"; AStrHostAddress = "0.0.0.0"; }

            return;
        }

        private string GetUserAccountByUserID(string AStrUserID, string AStrRentToken)
        {
            string LStrReturn = string.Empty;
            string LStrVerificationCode102 = string.Empty;
            string LStrSelectSQL = string.Empty;

            try
            {
                LStrReturn = AStrUserID;

                DataOperations01 LDataOperation = new DataOperations01();
                DatabaseOperation01Return LDBOperationReturn = new DatabaseOperation01Return();
                LStrVerificationCode102 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M102);

                LStrSelectSQL = "SELECT C002 FROM T_11_005_" + AStrRentToken + " WHERE C001 = " + AStrUserID;
                LDBOperationReturn = LDataOperation.SelectDataByDynamicSQL(IIntDBType, IStrDBConnectProfile, LStrSelectSQL);
                if (LDBOperationReturn.StrReturn == "1")
                {
                    LStrReturn = LDBOperationReturn.DataSetReturn.Tables[0].Rows[0][0].ToString();
                    LStrReturn = EncryptionAndDecryption.EncryptDecryptString(LStrReturn, LStrVerificationCode102, EncryptionAndDecryption.UMPKeyAndIVType.M102);
                }
            }
            catch { LStrReturn = AStrUserID; }

            return LStrReturn;
        }
        #endregion

        /// <summary>
        /// 将日志写入 Windows 事情
        /// </summary>
        /// <param name="AStrWriteBody">日志内容</param>
        /// <param name="AEntryType">日志类型</param>
        //private void WriteEntryLog(string AStrWriteBody, EventLogEntryType AEntryType)
        //{
        //    try
        //    {
        //        UMPServiceLog.WriteEntry(AStrWriteBody, AEntryType);
        //    }
        //    catch { }
        //}

        /// <summary>
        /// 获取当前服务安装的路径
        /// </summary>
        /// <returns></returns>
        public string GetCurrentBaseDirectory()
        {
            string LStrReturn = string.Empty;
            string LStrVerificationCode001 = string.Empty;

            try
            {
                //System.Reflection.Assembly.GetEntryAssembly().Location
                LStrVerificationCode001 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M001);

                //LStrReturn = AppDomain.CurrentDomain.BaseDirectory;
                //WriteEntryLog("Read AppDomain.CurrentDomain.BaseDirectory (1) : " + LStrReturn, EventLogEntryType.Information);
                //string[] LStrDirectoryArray = LStrReturn.Split('\\');
                //LStrReturn = string.Empty;
                //foreach (string LStrDirectorySingle in LStrDirectoryArray)
                //{
                //    if (LStrDirectorySingle.ToUpper() == "WinServices".ToUpper()) { break; }
                //    LStrReturn += LStrDirectorySingle + "\\";
                //}
                string path = AppDomain.CurrentDomain.BaseDirectory;
                path = path.Substring(0, path.LastIndexOf("\\"));
                path = path.Substring(0, path.LastIndexOf("\\"));
                LStrReturn = path;
                WriteLog(LogMode.Info, "Read AppDomain.CurrentDomain.BaseDirectory (2) : " + LStrReturn );

                if (!System.IO.File.Exists(System.IO.Path.Combine(LStrReturn, "GlobalSettings", "UMP.Young.01")))
                {
                    System.IO.FileStream LFileStream = new System.IO.FileStream(System.IO.Path.Combine(LStrReturn, "GlobalSettings", "UMP.Young.01"), System.IO.FileMode.Create, System.IO.FileAccess.ReadWrite);
                    StreamWriter LStreamWriter = new StreamWriter(LFileStream);
                    LStreamWriter.WriteLine(EncryptionAndDecryption.EncryptDecryptString("YoungLic00", LStrVerificationCode001, EncryptionAndDecryption.UMPKeyAndIVType.M001));
                    LStreamWriter.Flush();
                    LStreamWriter.Close();
                    LFileStream.Close();
                    System.Threading.Thread.Sleep(200);
                }
            }
            catch (Exception ex)
            {
                LStrReturn = string.Empty;
                WriteLog(LogMode.Error, "Read AppDomain.CurrentDomain.BaseDirectory Failed\n" + ex.Message );
            }

            return LStrReturn;
        }

        /// <summary>
        /// 获取UMP.PF站点绑定的http端口
        /// </summary>
        /// <returns></returns>
        private int GetUMPPFBasicHttpPort()
        {
            int LIntReturn = 0;
            string LStrXmlFileName = string.Empty;

            try
            {
                WriteLog(LogMode.Info, "Read UMP.PF Binding Port Information ...");

                #region 以前获取的方式
                //ServerManager LServerManager = new ServerManager();
                //foreach (Site LSiteSingle in LServerManager.Sites)
                //{
                //    if (LSiteSingle.Name.Equals("UMP.PF"))
                //    {
                //        if (LSiteSingle.State == ObjectState.Started)
                //        {
                //            foreach (Binding LBindingSingle in LSiteSingle.Bindings)
                //            {
                //                if (LBindingSingle.Protocol.ToLower() == "http")
                //                {
                //                    IPEndPoint LIPEndPoint = LBindingSingle.EndPoint;
                //                    LIntReturn = LIPEndPoint.Port;
                //                }
                //            }
                //        }
                //        break;
                //    }
                //}
                #endregion

                LStrXmlFileName = System.IO.Path.Combine(IStrBaseDirectory, @"GlobalSettings\UMP.Server.01.xml");
                XmlDocument LXmlDocServer01 = new XmlDocument();
                LXmlDocServer01.Load(LStrXmlFileName);
                XmlNode LXMLNodeSection = LXmlDocServer01.SelectSingleNode("UMPSetted").SelectSingleNode("IISBindingProtocol");
                XmlNodeList LXmlNodeBindingProtocol = LXMLNodeSection.ChildNodes;
                foreach (XmlNode LXmlNodeSingleBinding in LXmlNodeBindingProtocol)
                {
                    if (LXmlNodeSingleBinding.Attributes["Protocol"].Value == "http")
                    {
                        IStrSiteBindingIPAddress = LXmlNodeSingleBinding.Attributes["IPAddress"].Value;
                        LIntReturn = int.Parse(LXmlNodeSingleBinding.Attributes["BindInfo"].Value);
                        break;
                    }
                }

                WriteLog(LogMode.Info, "Readed UMP.PF Binding Port : " + LIntReturn.ToString() + ", This Service Use " + (LIntReturn - 1).ToString());
            }
            catch(Exception ex)
            {
                LIntReturn = 0;
                WriteLog(LogMode.Error, "Read UMP.PF Binding Port Information Failed\n" + ex.Message );
            }

            return LIntReturn;
        }

        #region 检测 Thread 是否活着 的所有代码
        /// <summary>
        /// 创建检测 Thread 是否活着的进程
        /// </summary>
        /// <returns></returns>
        private bool CreateCheckThreadIsAlive()
        {
            bool LBoolReturn = true;

            try
            {
                WriteLog(LogMode.Info, "CreateCheckThreadIsAlive()  Begin" );
                ThreadCheckThreadIsAlive = new System.Threading.Thread(new System.Threading.ThreadStart(CheckThreadIsAliveAction));
                ThreadCheckThreadIsAlive.Start();
                WriteLog(LogMode.Info, "CreateCheckThreadIsAlive()  Finished" );
            }
            catch (Exception ex)
            {
                WriteLog(LogMode.Error, "CreateCheckThreadIsAlive()  Failed\n" + ex.Message );
            }

            return LBoolReturn;
        }

        /// <summary>
        /// 检测进程是否真正活着的主体程序
        /// </summary>
        private void CheckThreadIsAliveAction()
        {
            bool LBoolReturn = true, LBoolRemoved = false;
            int LIntThreadCount = 0, LIntConnectedCount = 0;
            string LStrRetrun = string.Empty;

            try
            {
                while (IBoolCanContinue)
                {
                    RemoveIsNotAliveThread(ref LBoolReturn, ref LBoolRemoved, ref LIntThreadCount, ref LIntConnectedCount, ref LStrRetrun);
                    if (!LBoolReturn) { WriteLog(LogMode.Error, LStrRetrun ); }

                    Thread.Sleep(5000);
                }
            }
            catch (Exception ex)
            {
                WriteLog(LogMode.Error, "CheckThreadIsAliveAction()  Failed\n" + ex.Message );
            }
        }

        private void AddConnectedClientAndThread(ClientOperations AClientOperation, Thread AClientThread, ref bool ABoolReturn, ref string AStrReturn)
        {
            try
            {
                ABoolReturn = true;
                AStrReturn = string.Empty;

                lock (ILockObjectCheckThread)
                {
                    GlobalListConnectedClient.Add(AClientOperation);
                    GlobalListClientThread.Add(AClientThread);
                    WriteLog(LogMode.Info, "AddConnectedClientAndThread() GlobalListClientThread.Count = " + GlobalListClientThread.Count.ToString() + "    GlobalListConnectedClient.Count = " + GlobalListConnectedClient.Count.ToString() );
                }
            }
            catch (Exception ex)
            {
                ABoolReturn = false;
                AStrReturn = "AddConnectedClientAndThread()\n" + ex.Message;
            }
        }

        private void RemoveIsNotAliveThread(ref bool ABoolReturn, ref bool ABoolRemoved, ref int AIntThreadCount, ref int AIntConnectedCount, ref string AStrRetrun)
        {
            int LIntLoopClientThread, LIntAllClientThread;
            bool LBoolMoved = false;

            try
            {
                AStrRetrun = string.Empty;
                ABoolReturn = true;
                ABoolRemoved = false;

                lock (ILockObjectCheckThread)
                {
                    LBoolMoved = false;
                    LIntAllClientThread = GlobalListClientThread.Count - 1;

                    AIntThreadCount = GlobalListClientThread.Count;
                    AIntConnectedCount = GlobalListConnectedClient.Count;

                    for (LIntLoopClientThread = LIntAllClientThread; LIntLoopClientThread >= 0; LIntLoopClientThread--)
                    {
                        if (GlobalListClientThread[LIntLoopClientThread].IsAlive == false)
                        {
                            GlobalListClientThread.RemoveAt(LIntLoopClientThread);
                            GlobalListConnectedClient.RemoveAt(LIntLoopClientThread);
                            LBoolMoved = true;
                        }
                    }
                    if (LBoolMoved)
                    {
                        ABoolRemoved = true;
                        AIntThreadCount = GlobalListClientThread.Count;
                        AIntConnectedCount = GlobalListConnectedClient.Count;
                        WriteLog(LogMode.Info, "RemoveIsNotAliveThread() GlobalListClientThread.Count = " + AIntThreadCount.ToString() + "    GlobalListConnectedClient.Count = " + AIntConnectedCount.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                ABoolReturn = false;
                AStrRetrun = "RemoveIsNotAliveThread()   Failed\n" + ex.Message;
            }
        }
        #endregion

        ///// <summary>
        ///// 处理其他程序通过net.pipe传递过来的消息
        ///// </summary>
        ///// <param name="AInterfaceArgs"></param>
        //public void ProcessingClientMessage(PFShareClassesC.ShareClassForInterface AInterfaceArgs)
        //{
        //    //throw new NotImplementedException();
        //}

        private void StartTcpSSLServer(int AIntPort)
        {
            string LStrInstallReturn = string.Empty;

            try
            {
                WriteLog(LogMode.Info, "StartTcpSSLServer() Begin");

                #region 原使用安全证书方式 2015-07-06 修改

                //X509Store LX509Store = new X509Store(StoreName.My, StoreLocation.LocalMachine);

                //LX509Store.Open(OpenFlags.ReadOnly);
                //foreach (X509Certificate LX509CertificateSingle in LX509Store.Certificates)
                //{
                //    if (LX509CertificateSingle.GetCertHashString().Trim() == "2D508A175B6836ADB6E220BA63E00F2F0881E75F")
                //    {
                //        IX509CertificateServer = LX509CertificateSingle;
                //        break;
                //    }
                //}
                //LX509Store.Close(); LX509Store = null;

                //if (IX509CertificateServer == null)
                //{
                //    WriteLog(LogMode"StartTcpSSLServer() Install CertificateFile (UMP.PF.Certificate.pfx) Begin", EventLogEntryType.Information);
                //    LStrInstallReturn = InstallCertificateFile();
                //    if (!string.IsNullOrEmpty(LStrInstallReturn))
                //    {
                //        WriteLog(LogMode"StartTcpSSLServer() Install CertificateFile (UMP.PF.Certificate.pfx) Failed\n" + LStrInstallReturn, EventLogEntryType.Error);
                //        return;
                //    }
                //    else
                //    {
                //        WriteLog(LogMode"StartTcpSSLServer() Install CertificateFile (UMP.PF.Certificate.pfx) Finished", EventLogEntryType.Information);
                //    }

                //    LX509Store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
                //    LX509Store.Open(OpenFlags.ReadOnly);
                //    foreach (X509Certificate LX509CertificateSingle in LX509Store.Certificates)
                //    {
                //        if (LX509CertificateSingle.GetCertHashString().Trim() == "2D508A175B6836ADB6E220BA63E00F2F0881E75F")
                //        {
                //            IX509CertificateServer = LX509CertificateSingle;
                //            break;
                //        }
                //    }
                //    LX509Store.Close(); LX509Store = null;
                //    if (IX509CertificateServer == null)
                //    {
                //        WriteLog(LogMode"StartTcpSSLServer() Certificates Find Failed (2D508A175B6836ADB6E220BA63E00F2F0881E75F)", EventLogEntryType.Error);
                //    }
                //}

                //WriteLog(LogMode"IX509CertificateServer.GetCertHashString() " + IX509CertificateServer.GetCertHashString(), EventLogEntryType.Information);
                #endregion

                #region 使用安全证书方式 2015-07-06 修改
                IX509CertificateServer = new X509Certificate2(System.IO.Path.Combine(IStrBaseDirectory, "Components", "Certificates", "UMP.SSL.Certificate.pfx"), "VoiceCyber,123");
                #endregion

                ITcpListener = new TcpListener(IPAddress.Any, AIntPort);
                ITcpListener.Start();
                WriteLog(LogMode.Info, "StartTcpSSLServer() Finished, SSL Tcp Listener Point : " + AIntPort.ToString() + "\nWaiting For A Socket Link From Client" );
            }
            catch (Exception ex)
            {
                WriteLog(LogMode.Error, "StartTcpSSLServer() Failed\n" + ex.Message );
            }
        }

        private void WaitForSocketClientConnect()
        {
            bool LBoolReturn = true;
            string LStrReturn = string.Empty;

            try
            {
                while (IBoolCanContinue)
                {
                    if (!ITcpListener.Pending()) { Thread.Sleep(200); continue; }

                    TcpClient LTcpClient = ITcpListener.AcceptTcpClient();

                    if (!IBoolCanContinue) { return; }
                    
                    IBoolIsBusing = true;
                    ClientOperations LClientOperations = new ClientOperations(LTcpClient);
                    Thread LClientThread = new Thread(new ThreadStart(LClientOperations.ClientMessageOperation));
                    AddConnectedClientAndThread(LClientOperations, LClientThread, ref LBoolReturn, ref LStrReturn);
                    if (LBoolReturn) { LClientThread.Start(); } else { WriteLog(LogMode.Error, LStrReturn ); }
                    Thread.Sleep(100);

                    IBoolIsBusing = false;
                }
            }
            catch (Exception ex)
            {
                IBoolCanContinue = false;
                WriteLog(LogMode.Error, "WaitForSocketClientConnect() Failed\n" + ex.Message );
            }
        }

        private void CloseAllClientThread()
        {
            int LIntAllConnected = 0;

            try
            {
                LIntAllConnected = GlobalListConnectedClient.Count - 1;
                while (LIntAllConnected >= 0)
                {
                    GlobalListConnectedClient[LIntAllConnected].StopThisClientThread();
                    LIntAllConnected -= 1;
                }
            }
            catch (Exception ex)
            {
                WriteLog(LogMode.Error, "CloseAllClientThread() Failed\n" + ex.Message );
            }
        }

        /// <summary>
        /// 安装安全证书，区域为：受信任的根证书发布机构、受信任的发布者
        /// </summary>
        /// <param name="AStrCertificateFileName">证书名</param>
        /// <param name="AStrPassword">证书安装密码</param>
        /// <returns>如果为空，表示安装成功，否则为错误信息</returns>
        private string InstallCertificateFile()
        {
            string LStrReturn = string.Empty;
            string LStrCertificateFileFullName = string.Empty;
            string LStrInstallArea = string.Empty;

            try
            {
                LStrCertificateFileFullName = System.IO.Path.Combine(IStrBaseDirectory, @"Components\Certificates\UMP.SSL.Certificate.pfx");
                byte[] LByteReadedCertificate = System.IO.File.ReadAllBytes(LStrCertificateFileFullName);
                //LStrInstallArea = "StoreName = TrustedPublisher";
                //LStrReturn = InstallCertificateToStore(StoreName.TrustedPublisher, LByteReadedCertificate, "VoiceCyber,123");
                //if (!string.IsNullOrEmpty(LStrReturn)) { return LStrReturn; }
                //LStrInstallArea = "StoreName = AuthRoot";
                //LStrReturn = InstallCertificateToStore(StoreName.AuthRoot, LByteReadedCertificate, "VoiceCyber,123");
                //if (!string.IsNullOrEmpty(LStrReturn)) { return LStrReturn; }
                LStrInstallArea = "StoreName = My";
                LStrReturn = InstallCertificateToStore(StoreName.My, LByteReadedCertificate, "VoiceCyber,123");
                if (!string.IsNullOrEmpty(LStrReturn)) { return LStrReturn; }
            }
            catch (Exception ex)
            {
                LStrReturn = "InstallCertificateFile() " + LStrInstallArea + "\n" + ex.Message;
            }

            return LStrReturn;
        }

        /// <summary>
        /// 将证书安装到制定存储区域
        /// </summary>
        /// <param name="AStoreName">证书存储区的名称</param>
        /// <param name="AByteCertificate">证书文件内容</param>
        /// <param name="AStrPassword">证书密码</param>
        /// <returns>如果为空，表示安装成功，否则为错误信息</returns>
        private string InstallCertificateToStore(StoreName AStoreName, byte[] AByteCertificate, string AStrPassword)
        {
            string LStrReturn = string.Empty;

            try
            {
                X509Certificate2 LX509Certificate = new X509Certificate2(AByteCertificate, AStrPassword);
                X509Store LX509Store = new X509Store(AStoreName, StoreLocation.LocalMachine);
                
                LX509Store.Open(OpenFlags.ReadWrite);
                LX509Store.Remove(LX509Certificate);
                LX509Store.Add(LX509Certificate);
                LX509Store.Close();
            }
            catch (Exception ex)
            {
                LStrReturn = "InstallCertificateToStore() " + AStoreName.ToString() + "\n" + ex.Message;
            }

            return LStrReturn;
        }

        /// <summary>
        /// 获取数据路类型及数据库连接串
        /// </summary>
        /// <returns>string.Empty成功，Error01未进行数据库配置，否则失败</returns>
        private void GetDatabaseConnectionProfile()
        {
            string LStrXmlFileName = string.Empty;
            string LStrVerificationCode = string.Empty;

            string LStrAttributesData = string.Empty;
            //0:数据库服务器；1：端口；2：数据库名或服务名；3：登录用户；4：登录密码；5：其他参数
            List<string> LListStrDBProfile = new List<string>();

            try
            {
                IIntDBType = 0;
                IStrDBConnectProfile = string.Empty;

                LStrXmlFileName = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                LStrXmlFileName = Path.Combine(LStrXmlFileName, @"UMP.Server\Args01.UMP.xml");
                if (!File.Exists(LStrXmlFileName))
                {
                    WriteLog(LogMode.Warn, "GetDatabaseConnectionProfile() \nThe Database Connection Parameters Is Not Configured");
                    return;
                }
                XmlDocument LXmlDocArgs01 = new XmlDocument();
                LXmlDocArgs01.Load(LStrXmlFileName);
                XmlNodeList LXmlNodeListDatabase = LXmlDocArgs01.SelectSingleNode("DatabaseParameters").ChildNodes;
                if (LXmlNodeListDatabase.Count <= 0)
                {
                    WriteLog(LogMode.Warn, "GetDatabaseConnectionProfile() \nThe Database Connection Parameters Is Not Configured");
                    return;
                }

                LStrVerificationCode = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104);

                #region 数据库连接参数
                foreach (XmlNode LXmlNodeSingleDatabase in LXmlNodeListDatabase)
                {
                    LStrAttributesData = LXmlNodeSingleDatabase.Attributes["P03"].Value;
                    LStrAttributesData = EncryptionAndDecryption.EncryptDecryptString(LStrAttributesData, LStrVerificationCode, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                    if (LStrAttributesData != "1") { continue; }

                    //数据库类型
                    LStrAttributesData = LXmlNodeSingleDatabase.Attributes["P02"].Value;
                    LStrAttributesData = EncryptionAndDecryption.EncryptDecryptString(LStrAttributesData, LStrVerificationCode, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                    IIntDBType = int.Parse(LStrAttributesData);

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

                if (IIntDBType == 2)
                {
                    IStrDBConnectProfile = string.Format("Data Source={0},{1};Initial Catalog={2};User Id={3};Password={4}", LListStrDBProfile[0], LListStrDBProfile[1], LListStrDBProfile[2], LListStrDBProfile[3], LListStrDBProfile[4]);
                    LStrDBConnectProfile = string.Format("Data Source={0},{1};Initial Catalog={2};User Id={3};Password={4}", LListStrDBProfile[0], LListStrDBProfile[1], LListStrDBProfile[2], LListStrDBProfile[3], "******");
                    LStrDBConnectProfile = "DataBase Type : MS SQL Server\n" + LStrDBConnectProfile;
                }
                if (IIntDBType == 3)
                {
                    IStrDBConnectProfile = string.Format("Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST={0}) (PORT={1})))(CONNECT_DATA=(SERVICE_NAME= {2})));User Id={3}; Password={4}", LListStrDBProfile[0], LListStrDBProfile[1], LListStrDBProfile[2], LListStrDBProfile[3], LListStrDBProfile[4]);
                    LStrDBConnectProfile = string.Format("Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST={0}) (PORT={1})))(CONNECT_DATA=(SERVICE_NAME= {2})));User Id={3}; Password={4}", LListStrDBProfile[0], LListStrDBProfile[1], LListStrDBProfile[2], LListStrDBProfile[3], "******");
                    LStrDBConnectProfile = "DataBase Type : Oracle\n" + LStrDBConnectProfile;
                }
                #endregion

                WriteLog(LogMode.Info, "GetDatabaseConnectionProfile() \n" + LStrDBConnectProfile );
            }
            catch (Exception ex)
            {
                IIntDBType = 0;
                IStrDBConnectProfile = string.Empty;
                WriteLog(LogMode.Error, "GetDatabaseConnectionProfile() Failed\n" + ex.Message );
            }
        }

        /// <summary>
        /// 从GlobalSettings\UMP.Server.03.xml中读取License信息
        /// </summary>
        private void GetSetted0000FromServer03Xml()
        {
            string LStrXmlFileName = string.Empty;
            string LStrP00 = string.Empty, LStrP01 = string.Empty, LStrP02 = string.Empty, LStrP03 = string.Empty, LStrP04 = string.Empty, LStrP05 = string.Empty;
            string LStrVerificationCode101 = string.Empty;

            try
            {
                LStrVerificationCode101 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M101);

                LStrXmlFileName = System.IO.Path.Combine(IStrBaseDirectory, @"GlobalSettings\UMP.Server.03.xml");
                XmlDocument LXmlDocServer03 = new XmlDocument();
                LXmlDocServer03.Load(LStrXmlFileName);
                XmlNodeList LXmlNodeListUMPSetted = LXmlDocServer03.SelectSingleNode("UMPSetted").ChildNodes;
                foreach (XmlNode LXmlNodeSingleSetted in LXmlNodeListUMPSetted)
                {
                    if (LXmlNodeSingleSetted.NodeType == XmlNodeType.Comment) { continue; }
                    LStrP00 = LXmlNodeSingleSetted.Attributes["P00"].Value;
                    if (LStrP00 != "0000") { continue; }
                    LStrP01 = LXmlNodeSingleSetted.Attributes["P01"].Value;
                    LStrP02 = LXmlNodeSingleSetted.Attributes["P02"].Value;
                    LStrP03 = LXmlNodeSingleSetted.Attributes["P03"].Value;
                    LStrP04 = LXmlNodeSingleSetted.Attributes["P04"].Value;
                    LStrP05 = LXmlNodeSingleSetted.Attributes["P05"].Value;

                    if (string.IsNullOrEmpty(LStrP04)) { IStrIsReadedLicense = "0"; return; }
                    LStrP01 = EncryptionAndDecryption.EncryptDecryptString(LStrP01, LStrVerificationCode101, EncryptionAndDecryption.UMPKeyAndIVType.M101).Substring(3);
                    LStrP02 = EncryptionAndDecryption.EncryptDecryptString(LStrP02, LStrVerificationCode101, EncryptionAndDecryption.UMPKeyAndIVType.M101).Substring(3);
                    LStrP03 = EncryptionAndDecryption.EncryptDecryptString(LStrP03, LStrVerificationCode101, EncryptionAndDecryption.UMPKeyAndIVType.M101).Substring(3);
                    LStrP04 = EncryptionAndDecryption.EncryptDecryptString(LStrP04, LStrVerificationCode101, EncryptionAndDecryption.UMPKeyAndIVType.M101).Substring(3);
                    if (LStrP04 == "1") { LStrP05 = EncryptionAndDecryption.EncryptDecryptString(LStrP05, LStrVerificationCode101, EncryptionAndDecryption.UMPKeyAndIVType.M101).Substring(3); }
                    IIntP01 = int.Parse(LStrP01); IIntP02 = int.Parse(LStrP02); IStrP03 = LStrP03; IStrIsReadedLicense = LStrP04;
                    break;
                }
                
            }
            catch (Exception ex)
            {
                WriteLog(LogMode.Error, "GetSetted0000FromServer03Xml() Failed\n" + ex.Message );
            }
            finally
            {
                WriteLog(LogMode.Info, "GetSetted0000FromServer03Xml\n1100000 = " + IStrP03 + "  1100001 = " + IIntP01.ToString() + "   1100002 = " + IIntP02.ToString());
            }
        }

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

        private string AscCodeToChr(int AsciiCode)
        {
            System.Text.ASCIIEncoding asciiEncoding = new System.Text.ASCIIEncoding();
            byte[] byteArray = new byte[] { (byte)AsciiCode };
            string strCharacter = asciiEncoding.GetString(byteArray);
            return (strCharacter);
        }

        #region Encrypt and Decrypt

        public string DecryptString(string source, int mode)
        {
            //return source;
            return ServerAESEncryption.DecryptString(source, (EncryptionMode)mode, Encoding.ASCII);
        }

        public string DecryptString(string source)
        {
            return source;
        }

        public string EncryptString(string source, int mode)
        {
            //return source;
            return ServerAESEncryption.EncryptString(source, (EncryptionMode)mode, Encoding.ASCII);
        }

        public string EncryptString(string source)
        {
            return source;
        }

        public string DecryptString(string source, int mode, Encoding encoding)
        {
            return ServerAESEncryption.DecryptString(source, (EncryptionMode)mode, encoding);
        }

        public string EncryptString(string source, int mode, Encoding encoding)
        {
            return ServerAESEncryption.EncryptString(source, (EncryptionMode)mode, encoding);
        }

        public byte[] DecryptBytes(byte[] source, int mode)
        {
            return ServerAESEncryption.DecryptBytes(source, (EncryptionMode)mode);
        }

        public byte[] DecryptBytes(byte[] source)
        {
            return source;
        }

        public byte[] EncryptBytes(byte[] source, int mode)
        {
            return ServerAESEncryption.EncryptBytes(source, (EncryptionMode)mode);
        }

        public byte[] EncryptBytes(byte[] source)
        {
            return source;
        }

        #endregion
    }

    public class ClientOperations
    {
        private TcpClient ITcpClient = null;

        private bool IBoolCanSendMessage = true;
        private bool IBoolInSendingMessage = false;
        private bool IBoolCanRecive = true;
        private string IStrSpliterChar = string.Empty;
        public SslStream ISslStream = null;

        public ClientOperations(TcpClient ATcpClient)
        {
            ITcpClient = ATcpClient;
            IStrSpliterChar = AscCodeToChr(27);
        }

        public void ClientMessageOperation()
        {
            string LStrReadedData = string.Empty;
            string LStrEntryBody = string.Empty;

            try
            {
                LStrEntryBody = "A Client Connected\n";
                LStrEntryBody += "IPAddress : " + ((IPEndPoint)ITcpClient.Client.RemoteEndPoint).Address.ToString();
                LStrEntryBody += " Port : " + ((IPEndPoint)ITcpClient.Client.RemoteEndPoint).Port.ToString() + "\n";
                UMPService01.WriteLog(LogMode.Info, LStrEntryBody);
                //ISslStream = new SslStream(ITcpClient.GetStream(), false, ValidateServerCertificate, null);
                //ISslStream.AuthenticateAsServer(UMPService01.IX509CertificateServer, false, System.Security.Authentication.SslProtocols.Default, false);
                //ISslStream.ReadTimeout = 10000;
                //ISslStream.WriteTimeout = 10000;
                ISslStream = new SslStream(ITcpClient.GetStream(), false, (s, cert, chain, err) => true);
                ISslStream.AuthenticateAsServer(UMPService01.IX509CertificateServer);
                while (IBoolCanRecive)
                {
                    try
                    {
                        LStrReadedData = ReadMessageFromClient();
                        if (string.IsNullOrEmpty(LStrReadedData)) { continue; }
                        DealClientMessage(LStrReadedData);
                    }
                    catch (Exception ex)
                    {
                        IBoolCanRecive = false;
                        UMPService01.WriteLog(LogMode.Warn, "ISslStream.Read()\n" + ex.Message);
                    }
                }
                while (IBoolInSendingMessage) { Thread.Sleep(50); }
                IBoolCanSendMessage = false;
            }
            catch (AuthenticationException ex)
            {
                IBoolCanSendMessage = false;
                IBoolCanRecive = false;
                UMPService01.WriteLog(LogMode.Error, "ClientMessageOperation()\nAuthenticationException:" + ex.Message );
                if (ex.InnerException != null)
                {
                    UMPService01.WriteLog(LogMode.Error, "ClientMessageOperation()\nInnerException:" + ex.InnerException.ToString() );
                }
                StopThisClientThread();
            }
            catch (Exception ex)
            {
                IBoolCanSendMessage = false;
                IBoolCanRecive = false;
                UMPService01.WriteLog(LogMode.Error, "ClientMessageOperation()\n" + ex.Message );
                StopThisClientThread();
            }
        }

        public void StopThisClientThread()
        {
            try
            {
                UMPService01.WriteLog(LogMode.Info, "StopThisClientThread() Client : " + ITcpClient.Client.RemoteEndPoint.ToString() );
                SendMessageToClient("Result=StopService");
                IBoolCanSendMessage = false;
                IBoolCanRecive = false;
                while (IBoolCanSendMessage) { Thread.Sleep(50); }
                if (ISslStream != null) { ISslStream.Close(); }
                if (ITcpClient != null) { ITcpClient.Close(); }
            }
            catch (Exception ex)
            {
                UMPService01.WriteLog(LogMode.Error, "StopThisClientThread()\n" + ex.Message );
            }
        }

        private void SendMessageToClient(string AStrMessage)
        {
            string LStrSendMessage = string.Empty;

            try
            {
                if (!IBoolCanSendMessage) { return; }
                IBoolInSendingMessage = true;
                LStrSendMessage = AStrMessage;
                if (string.IsNullOrEmpty(LStrSendMessage)) { LStrSendMessage = " "; }
                LStrSendMessage += "\r\n";
                byte[] LByteMessage = Encoding.UTF8.GetBytes(LStrSendMessage);
                ISslStream.Write(LByteMessage);
                ISslStream.Flush();
            }
            catch (Exception ex)
            {
                UMPService01.WriteLog(LogMode.Error, "SendMessageToClient()\n" + ex.Message );
            }
            finally
            {
                IBoolInSendingMessage = false;
            }
        }

        private string ReadMessageFromClient()
        {
            string LStrReadedData = string.Empty;

            try
            {
                StringBuilder LStringBuilderData = new StringBuilder();
                int LIntReadedBytes = -1, LIntEndKeyPosition;
                byte[] LByteReadeBuffer = new byte[1024];

                do
                {
                    LIntReadedBytes = ISslStream.Read(LByteReadeBuffer, 0, LByteReadeBuffer.Length);
                    Decoder LDecoder = Encoding.UTF8.GetDecoder();
                    char[] LChars = new char[LDecoder.GetCharCount(LByteReadeBuffer, 0, LIntReadedBytes)];
                    LDecoder.GetChars(LByteReadeBuffer, 0, LIntReadedBytes, LChars, 0);
                    LStringBuilderData.Append(LChars);
                    if (LStringBuilderData.ToString().IndexOf("\r\n") > 0) { break; }
                }
                while (LIntReadedBytes != 0);
                LStrReadedData = LStringBuilderData.ToString();
                LIntEndKeyPosition = LStrReadedData.IndexOf("\r\n");
                if (LIntEndKeyPosition > 0)
                {
                    LStrReadedData = LStrReadedData.Substring(0, LIntEndKeyPosition);
                    UMPService01.WriteLog(LogMode.Info, "ReadMessageFromClient() " + LStrReadedData + " (" + ITcpClient.Client.RemoteEndPoint.ToString() + ")" );
                }
                IBoolCanRecive = false;
            }
            catch (Exception ex)
            {
                IBoolCanRecive = false;
                LStrReadedData = string.Empty;
                UMPService01.WriteLog(LogMode.Error, "ReadMessageFromClient()\n" + ex.Message + "\n" + ITcpClient.Client.RemoteEndPoint.ToString() );
            }

            return LStrReadedData;
        }

        private bool ReadMessageFromServer(SslStream ASslStream, ref string AStrReadedMessage)
        {
            bool LBoolReturn = true;

            try
            {
                AStrReadedMessage = string.Empty;

                StringBuilder LStringBuilderData = new StringBuilder();
                int LIntReadedBytes = -1, LIntEndKeyPosition;
                byte[] LByteReadeBuffer = new byte[1024];

                do
                {
                    LIntReadedBytes = ASslStream.Read(LByteReadeBuffer, 0, LByteReadeBuffer.Length);
                    Decoder LDecoder = Encoding.UTF8.GetDecoder();
                    char[] LChars = new char[LDecoder.GetCharCount(LByteReadeBuffer, 0, LIntReadedBytes)];
                    LDecoder.GetChars(LByteReadeBuffer, 0, LIntReadedBytes, LChars, 0);
                    LStringBuilderData.Append(LChars);
                    if (LStringBuilderData.ToString().IndexOf(AscCodeToChr(27) + "End" + AscCodeToChr(27)) > 0) { break; }
                }
                while (LIntReadedBytes != 0);
                AStrReadedMessage = LStringBuilderData.ToString();
                LIntEndKeyPosition = AStrReadedMessage.IndexOf(AscCodeToChr(27) + "End" + AscCodeToChr(27));
                if (LIntEndKeyPosition > 0)
                {
                    AStrReadedMessage = AStrReadedMessage.Substring(0, LIntEndKeyPosition);
                }
            }
            catch (Exception ex)
            {
                LBoolReturn = false;
                AStrReadedMessage = ex.Message;
            }

            return LBoolReturn;
        }

        private void DealClientMessage(string AStrMessage)
        {
            string LStrVerificationCode104 = string.Empty;
            string LStrVerificationCode004 = string.Empty;
            string LStrClientMethod = string.Empty;
            string LStrSendMessage = string.Empty;

            try
            {
                #region 判断是否数据库是否已经配置
                if (UMPService01.IIntDBType == 0 || string.IsNullOrEmpty(UMPService01.IStrDBConnectProfile))
                {
                    UMPService01.WriteLog(LogMode.Warn, "DealClientMessage() Database Parameters Are Not Configured : " + LStrClientMethod + "\n" + ITcpClient.Client.RemoteEndPoint.ToString() );
                    LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString("E01A14", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    SendMessageToClient(LStrSendMessage);             //数据库参数未配置
                    return;
                }
                #endregion

                string[] LStrUserSendData = AStrMessage.Split(IStrSpliterChar.ToArray());
                if (LStrUserSendData.Length <= 0) { return; }
                LStrVerificationCode004 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004);
                LStrVerificationCode104 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104);
                LStrClientMethod = EncryptionAndDecryption.EncryptDecryptString(LStrUserSendData[0], LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                if (LStrClientMethod.Length != 6)
                {
                    UMPService01.WriteLog(LogMode.Warn, "DealClientMessage() Error Command : " + LStrClientMethod + "\n" + ITcpClient.Client.RemoteEndPoint.ToString());
                    LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString("E01001", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    SendMessageToClient(LStrSendMessage);             //错误的指令
                    return;
                }
                UMPService01.WriteLog(LogMode.Info, string.Format("DealClientMessage() Receive Command : {0}" + "\n" + ITcpClient.Client.RemoteEndPoint.ToString(), LStrClientMethod) );
                int LIntDataLength = LStrUserSendData.Length;
                if (LStrClientMethod != "M01D01" && LStrClientMethod != "M01E01")
                {
                    for (int LIntDataLoop = 0; LIntDataLoop < LIntDataLength; LIntDataLoop++)
                    {
                        string strTemp = EncryptionAndDecryption.EncryptDecryptString(LStrUserSendData[LIntDataLoop], LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                        LStrUserSendData[LIntDataLoop] = strTemp.TrimEnd('\0');

                    }
                }
                switch (LStrClientMethod)
                {
                    case "M01A01":           //用户登录
                        UserLoginUMPSystem(LStrUserSendData);
                        break;
                    case "M01A02":           //用户退出
                        UserLogoutUMPSystem(LStrUserSendData);
                        break;
                    case "M01A03":           //用户在线
                        UserIsOnlineUMPSystem(LStrUserSendData);
                        break;
                    case "M01A04":          //用户修改密码
                        UserChangeLoginPassword(LStrUserSendData);
                        break;
                    case "M01A11":          //座席登录
                        AgentLoginUMPSystem(LStrUserSendData);
                        break;
                    case "M01A12":          //座席退出
                        AgentLogoutUMPSystem(LStrUserSendData);
                        break;
                    case "M01A13":          //座席在线
                        AgentIsOnlineUMPSystem(LStrUserSendData);
                        break;
                    case "M01A14":          //Agentex座席登录
                        AgentexLoginUMPSystem(LStrUserSendData);
                        break;
                    case "M01A15":          //Agentex座席退出
                        AgentexLogoutUMPSystem(LStrUserSendData);
                        break;
                    case "M01A16":          //Agentex座席在线
                        AgentexIsOnlineUMPSystem(LStrUserSendData);
                        break;
                    case "M01A21":          //SFTP服务身份验证
                        SftpUserAuthentication(LStrUserSendData);
                        break;
                    case "M01B01":           //写操作日志
                        WriteOperationsLog(LStrUserSendData);
                        break;
                    case "M01C01":            //重新获取数据库连接参数
                        GetDatabaseConnectionProfile(LStrUserSendData);
                        break;
                    case "M01D01":           //加密或解密数据
                        EncryptionAndDecryptionString(LStrUserSendData);
                        break;
                    case "M01E01":          //录音文件本地回删后，回写数据库删除标志位
                        Update21001C033(LStrUserSendData);
                        break;
                    case "M01E02":          //获取参数配置的XML文件
                        ReturnCompressedFiles(LStrUserSendData);
                        break;
                    case "M01E03":          //归档设备上的文件回删后，回写数据库标志
                        Update21001C23X(LStrUserSendData);
                        break;
                    case "M01E04":          //获取
                        ReturnSoftDogSerialNum(LStrUserSendData);
                        break;
                    default:
                        LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString("E01002", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                        SendMessageToClient(LStrSendMessage);             //系统不能识别的指令
                        IBoolCanRecive = false;
                        break;
                }
            }
            catch(Exception ex)
            {
                LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString("E01999", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                SendMessageToClient(LStrSendMessage);
                UMPService01.WriteLog(LogMode.Error, "DealClientMessage()\n" + ex.Message + "\n" + ITcpClient.Client.RemoteEndPoint.ToString() );
            }
        }

        private void UserLoginUMPSystem(string[] AStrArrayLoginInfo)
        {
            bool LBoolCallReturnValue = true;
            string LStrCallReturnCode = string.Empty;
            string LStrCallReturnMessage = string.Empty;
            List<string> LListStrOtherInfo = new List<string>();
            string LStrSendMessage = string.Empty;
            string LStrVerificationCode004 = string.Empty;
            string LStrLoginAsSaRole = string.Empty;

            try
            {
                LStrVerificationCode004 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004);
                if (AStrArrayLoginInfo.Length < 7)
                {
                    LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString("E01A00", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    SendMessageToClient(LStrSendMessage);          //参数错误
                    UMPService01.WriteLog(LogMode.Error, "1.UserLoginUMPSystem()\nReturnCode：E01A00" );
                    return;
                }
                S01AOperations LS01AOperations = new S01AOperations(UMPService01.IIntDBType, UMPService01.IStrDBConnectProfile, ITcpClient);
                LS01AOperations.S01ASetLicenseInfo(UMPService01.IIntP01, UMPService01.IIntP02, UMPService01.IStrP03, UMPService01.IStrIsReadedLicense);
                LListStrOtherInfo.Add(AStrArrayLoginInfo[4]);       //0登录应用程序模块ID
                LListStrOtherInfo.Add(AStrArrayLoginInfo[5]);       //1客户端机器名
                if (AStrArrayLoginInfo[4] == "11000")
                {
                    LListStrOtherInfo.Add(AStrArrayLoginInfo[6]);       //2客户端机器IP
                }
                else
                {
                    LListStrOtherInfo.Add(((IPEndPoint)ITcpClient.Client.RemoteEndPoint).Address.ToString());
                }
                LStrLoginAsSaRole = "0";
                if (AStrArrayLoginInfo.Length >= 8) { LStrLoginAsSaRole = AStrArrayLoginInfo[7]; }
                LBoolCallReturnValue = LS01AOperations.S01AOperation01(AStrArrayLoginInfo[1], AStrArrayLoginInfo[2], AStrArrayLoginInfo[3], LStrLoginAsSaRole, LListStrOtherInfo, ref LStrCallReturnCode, ref LStrCallReturnMessage);
                SendMessageToClient(LStrCallReturnCode);
                if (!LBoolCallReturnValue)
                {
                    UMPService01.WriteLog(LogMode.Error, string.Format("2.UserLoginUMPSystem() Return Code :\n{0} Return Message :\n {1}\n {2}",
                        LStrCallReturnCode, LStrCallReturnMessage, ITcpClient.Client.RemoteEndPoint.ToString()));
                }
                else
                {
                    UMPService01.WriteLog(LogMode.Info, "3.UserLoginUMPSystem() Return Code :\n" + LStrCallReturnCode + "\n" + ITcpClient.Client.RemoteEndPoint.ToString() );
                }
            }
            catch (Exception ex)
            {
                UMPService01.WriteLog(LogMode.Error, "4.UserLoginUMPSystem()\n" + ex.Message + "\n" + ITcpClient.Client.RemoteEndPoint.ToString() );
            }
            finally
            {
                try
                {
                    ClientOpArgsAndReturn LClientOperationReturn = new ClientOpArgsAndReturn();
                    LClientOperationReturn.LBoolReturn = LBoolCallReturnValue;
                    LClientOperationReturn.LLongClientSessionID = 0;
                    LClientOperationReturn.LDateTimeOperation = DateTime.UtcNow;
                    LClientOperationReturn.LStrReturnCode = LStrCallReturnCode;
                    LClientOperationReturn.LStrReturnMessage = LStrCallReturnMessage;
                    LClientOperationReturn.LListArguments.Add(AStrArrayLoginInfo[0]);   //0
                    LClientOperationReturn.LListArguments.Add(AStrArrayLoginInfo[1]);   //1
                    LClientOperationReturn.LListArguments.Add(AStrArrayLoginInfo[2]);   //2
                    LClientOperationReturn.LListArguments.Add(AStrArrayLoginInfo[3]);   //3
                    LClientOperationReturn.LListArguments.Add(string.Empty);            //4
                    LClientOperationReturn.LListArguments.Add(AStrArrayLoginInfo[5]);   //5
                    LClientOperationReturn.LListArguments.Add(LListStrOtherInfo[2]);    //6
                    LClientOperationReturn.LListArguments.Add(LStrLoginAsSaRole);       //7
                    lock (UMPService01.ILock11901) { UMPService01.IYoungCircleQueueWriteOperation.PushElement(LClientOperationReturn); }
                }
                catch { }
            }
        }

        private void UserLogoutUMPSystem(string[] AStrArrayLogoutInfo)
        {
            bool LBoolCallReturnValue = true;
            string LStrCallReturnCode = string.Empty;
            string LStrCallReturnMessage = string.Empty;
            string LStrSendMessage = string.Empty;
            string LStrVerificationCode004 = string.Empty;

            try
            {
                LStrVerificationCode004 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004);
                if (AStrArrayLogoutInfo.Length < 4)
                {
                    LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString("E01A00", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    SendMessageToClient(LStrSendMessage);          //参数错误
                    UMPService01.WriteLog(LogMode.Error, "UserLogoutUMPSystem()\nReturnCode：E01A00" );
                    return;
                }
                S01AOperations LS01AOperations = new S01AOperations(UMPService01.IIntDBType, UMPService01.IStrDBConnectProfile, ITcpClient);
                LBoolCallReturnValue = LS01AOperations.S01AOperation02(AStrArrayLogoutInfo[1], AStrArrayLogoutInfo[2], AStrArrayLogoutInfo[3], ref LStrCallReturnCode, ref LStrCallReturnMessage);
                SendMessageToClient(LStrCallReturnCode);
                if (!LBoolCallReturnValue)
                {
                    UMPService01.WriteLog(LogMode.Error, "UserLogoutUMPSystem() Return Code :\n" + LStrCallReturnCode + "Return Message :\n" + LStrCallReturnMessage + "\n" + ITcpClient.Client.RemoteEndPoint.ToString() );
                }
                else
                {
                    UMPService01.WriteLog(LogMode.Info, "UserLogoutUMPSystem() Return Code :\n" + LStrCallReturnCode + "\n" + ITcpClient.Client.RemoteEndPoint.ToString() );
                }
            }
            catch (Exception ex)
            {
                UMPService01.WriteLog(LogMode.Error, "UserLogoutUMPSystem()\n" + ex.Message + "\n" + ITcpClient.Client.RemoteEndPoint.ToString());
            }
            finally
            {
                try
                {
                    ClientOpArgsAndReturn LClientOperationReturn = new ClientOpArgsAndReturn();

                    LClientOperationReturn.LBoolReturn = LBoolCallReturnValue;
                    LClientOperationReturn.LLongClientSessionID = long.Parse(AStrArrayLogoutInfo[3]);
                    LClientOperationReturn.LDateTimeOperation = DateTime.UtcNow;
                    LClientOperationReturn.LStrReturnCode = LStrCallReturnCode;
                    LClientOperationReturn.LStrReturnMessage = LStrCallReturnMessage;
                    LClientOperationReturn.LListArguments = AStrArrayLogoutInfo.ToList<string>();

                    lock (UMPService01.ILock11901) { UMPService01.IYoungCircleQueueWriteOperation.PushElement(LClientOperationReturn); }
                }
                catch { }
            }
        }

        private void UserIsOnlineUMPSystem(string[] AStrArrayLogoutInfo)
        {
            bool LBoolCallReturnValue = true;
            string LStrCallReturnCode = string.Empty;
            string LStrCallReturnMessage = string.Empty;
            string LStrSendMessage = string.Empty;
            string LStrVerificationCode004 = string.Empty;

            try
            {
                LStrVerificationCode004 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004);
                if (AStrArrayLogoutInfo.Length < 4)
                {
                    LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString("E01A00", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    SendMessageToClient(LStrSendMessage);          //参数错误
                    UMPService01.WriteLog(LogMode.Error, "UserIsOnlineUMPSystem()\nReturnCode：E01A00" );
                    return;
                }
                S01AOperations LS01AOperations = new S01AOperations(UMPService01.IIntDBType, UMPService01.IStrDBConnectProfile, ITcpClient);
                LBoolCallReturnValue = LS01AOperations.S01AOperation03(AStrArrayLogoutInfo[1], AStrArrayLogoutInfo[2], AStrArrayLogoutInfo[3], ref LStrCallReturnCode, ref LStrCallReturnMessage);
                SendMessageToClient(LStrCallReturnCode);
                if (!LBoolCallReturnValue)
                {
                    UMPService01.WriteLog(LogMode.Error, "UserIsOnlineUMPSystem() Return Code :\n" + LStrCallReturnCode + "Return Message :\n" + LStrCallReturnMessage + "\n" + ITcpClient.Client.RemoteEndPoint.ToString() );
                }
                else
                {
                    UMPService01.WriteLog(LogMode.Info, "UserIsOnlineUMPSystem() Return Code :\n" + LStrCallReturnCode + "\n" + ITcpClient.Client.RemoteEndPoint.ToString() );
                }
            }
            catch (Exception ex)
            {
                UMPService01.WriteLog(LogMode.Error, "UserIsOnlineUMPSystem()\n" + ex.Message + "\n" + ITcpClient.Client.RemoteEndPoint.ToString());
            }
        }

        private void UserChangeLoginPassword(string[] AStrArrayChangeInfo)
        {
            bool LBoolCallReturnValue = true;
            string LStrCallReturnCode = string.Empty;
            string LStrCallReturnMessage = string.Empty;
            string LStrSendMessage = string.Empty;
            string LStrVerificationCode004 = string.Empty;

            try
            {
                LStrVerificationCode004 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004);
                if (AStrArrayChangeInfo.Length < 6)
                {
                    LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString("E01A00", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    SendMessageToClient(LStrSendMessage);          //参数错误
                    UMPService01.WriteLog(LogMode.Error, "UserChangeLoginPassword()\nReturnCode：E01A00" );
                    return;
                }

                S01AOperations LS01AOperations = new S01AOperations(UMPService01.IIntDBType, UMPService01.IStrDBConnectProfile, ITcpClient);
                LBoolCallReturnValue = LS01AOperations.S01AOperation04(AStrArrayChangeInfo[1], AStrArrayChangeInfo[2], AStrArrayChangeInfo[3], AStrArrayChangeInfo[4], AStrArrayChangeInfo[5], ref LStrCallReturnCode, ref LStrCallReturnMessage);
                SendMessageToClient(LStrCallReturnCode);
                if (!LBoolCallReturnValue)
                {
                    UMPService01.WriteLog(LogMode.Error, "UserChangeLoginPassword() Return Code :\n" + LStrCallReturnCode + "Return Message :\n" + LStrCallReturnMessage + "\n" + ITcpClient.Client.RemoteEndPoint.ToString() );
                }
                else
                {
                    UMPService01.WriteLog(LogMode.Info, "UserChangeLoginPassword() Return Code :\n" + LStrCallReturnCode + "\n" + ITcpClient.Client.RemoteEndPoint.ToString() );
                }
            }
            catch (Exception ex)
            {
                UMPService01.WriteLog(LogMode.Error, "UserChangeLoginPassword()\n" + ex.Message + "\n" + ITcpClient.Client.RemoteEndPoint.ToString());
            }
            finally
            {
                try
                {
                    ClientOpArgsAndReturn LClientOperationReturn = new ClientOpArgsAndReturn();

                    LClientOperationReturn.LBoolReturn = LBoolCallReturnValue;
                    LClientOperationReturn.LLongClientSessionID = long.Parse(AStrArrayChangeInfo[5]);
                    LClientOperationReturn.LDateTimeOperation = DateTime.UtcNow;
                    LClientOperationReturn.LStrReturnCode = LStrCallReturnCode;
                    LClientOperationReturn.LStrReturnMessage = LStrCallReturnMessage;
                    LClientOperationReturn.LListArguments = AStrArrayChangeInfo.ToList<string>();

                    lock (UMPService01.ILock11901) { UMPService01.IYoungCircleQueueWriteOperation.PushElement(LClientOperationReturn); }
                }
                catch { }
            }
        }

        private void AgentLoginUMPSystem(string[] AStrArrayLoginInfo)
        {
            bool LBoolCallReturnValue = true;
            string LStrCallReturnCode = string.Empty;
            string LStrCallReturnMessage = string.Empty;
            List<string> LListStrOtherInfo = new List<string>();
            string LStrSendMessage = string.Empty;
            string LStrVerificationCode004 = string.Empty;

            try
            {
                LStrVerificationCode004 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004);
                if (AStrArrayLoginInfo.Length < 7)
                {
                    LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString("E01A00", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    SendMessageToClient(LStrSendMessage);          //参数错误
                    UMPService01.WriteLog(LogMode.Error, "AgentLoginUMPSystem()\nReturnCode：E01A00" );
                    return;
                }
                S01AOperations LS01AOperations = new S01AOperations(UMPService01.IIntDBType, UMPService01.IStrDBConnectProfile, ITcpClient);
                LListStrOtherInfo.Add(AStrArrayLoginInfo[4]);       //0登录应用程序模块ID
                LListStrOtherInfo.Add(AStrArrayLoginInfo[5]);       //1客户端机器名
                if (AStrArrayLoginInfo[4] == "11000")
                {
                    LListStrOtherInfo.Add(AStrArrayLoginInfo[6]);       //2客户端机器IP
                }
                else
                {
                    LListStrOtherInfo.Add(((IPEndPoint)ITcpClient.Client.RemoteEndPoint).Address.ToString());
                }
                LBoolCallReturnValue = LS01AOperations.S01AOperation11(AStrArrayLoginInfo[1], AStrArrayLoginInfo[2], AStrArrayLoginInfo[3], LListStrOtherInfo, ref LStrCallReturnCode, ref LStrCallReturnMessage);
                SendMessageToClient(LStrCallReturnCode);
                if (!LBoolCallReturnValue)
                {
                    UMPService01.WriteLog(LogMode.Error, "AgentLoginUMPSystem() Return Code :\n" + LStrCallReturnCode + "Return Message :\n" + LStrCallReturnMessage + "\n" + ITcpClient.Client.RemoteEndPoint.ToString() );
                }
                else
                {
                    UMPService01.WriteLog(LogMode.Info, "AgentLoginUMPSystem() Return Code :\n" + LStrCallReturnCode + "\n" + ITcpClient.Client.RemoteEndPoint.ToString() );
                }
            }
            catch (Exception ex)
            {
                UMPService01.WriteLog(LogMode.Error, "AgentLoginUMPSystem()\n" + ex.Message + "\n" + ITcpClient.Client.RemoteEndPoint.ToString());
            }
        }

        private void AgentLogoutUMPSystem(string[] AStrArrayLogoutInfo)
        {
            bool LBoolCallReturnValue = true;
            string LStrCallReturnCode = string.Empty;
            string LStrCallReturnMessage = string.Empty;
            string LStrSendMessage = string.Empty;
            string LStrVerificationCode004 = string.Empty;

            try
            {
                LStrVerificationCode004 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004);
                if (AStrArrayLogoutInfo.Length < 4)
                {
                    LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString("E01A00", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    SendMessageToClient(LStrSendMessage);          //参数错误
                    UMPService01.WriteLog(LogMode.Error, "AgentLogoutUMPSystem()\nReturnCode：E01A00" );
                    return;
                }
                S01AOperations LS01AOperations = new S01AOperations(UMPService01.IIntDBType, UMPService01.IStrDBConnectProfile, ITcpClient);
                LBoolCallReturnValue = LS01AOperations.S01AOperation12(AStrArrayLogoutInfo[1], AStrArrayLogoutInfo[2], AStrArrayLogoutInfo[3], ref LStrCallReturnCode, ref LStrCallReturnMessage);
                SendMessageToClient(LStrCallReturnCode);
                if (!LBoolCallReturnValue)
                {
                    UMPService01.WriteLog(LogMode.Error, "AgentLogoutUMPSystem() Return Code :\n" + LStrCallReturnCode + "Return Message :\n" + LStrCallReturnMessage + "\n" + ITcpClient.Client.RemoteEndPoint.ToString() );
                }
                else
                {
                    UMPService01.WriteLog(LogMode.Info, "AgentLogoutUMPSystem() Return Code :\n" + LStrCallReturnCode + "\n" + ITcpClient.Client.RemoteEndPoint.ToString() );
                }
            }
            catch (Exception ex)
            {
                UMPService01.WriteLog(LogMode.Error, "AgentLogoutUMPSystem()\n" + ex.Message + "\n" + ITcpClient.Client.RemoteEndPoint.ToString());
            }
        }

        private void AgentIsOnlineUMPSystem(string[] AStrArrayLogoutInfo)
        {
            bool LBoolCallReturnValue = true;
            string LStrCallReturnCode = string.Empty;
            string LStrCallReturnMessage = string.Empty;
            string LStrSendMessage = string.Empty;
            string LStrVerificationCode004 = string.Empty;

            try
            {
                LStrVerificationCode004 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004);
                if (AStrArrayLogoutInfo.Length < 4)
                {
                    LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString("E01A00", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    SendMessageToClient(LStrSendMessage);          //参数错误
                    UMPService01.WriteLog(LogMode.Error, "AgentIsOnlineUMPSystem()\nReturnCode：E01A00" );
                    return;
                }
                S01AOperations LS01AOperations = new S01AOperations(UMPService01.IIntDBType, UMPService01.IStrDBConnectProfile, ITcpClient);
                LBoolCallReturnValue = LS01AOperations.S01AOperation13(AStrArrayLogoutInfo[1], AStrArrayLogoutInfo[2], AStrArrayLogoutInfo[3], ref LStrCallReturnCode, ref LStrCallReturnMessage);
                SendMessageToClient(LStrCallReturnCode);
                if (!LBoolCallReturnValue)
                {
                    UMPService01.WriteLog(LogMode.Error, "AgentIsOnlineUMPSystem() Return Code :\n" + LStrCallReturnCode + "Return Message :\n" + LStrCallReturnMessage + "\n" + ITcpClient.Client.RemoteEndPoint.ToString() );
                }
                else
                {
                    UMPService01.WriteLog(LogMode.Info, "AgentIsOnlineUMPSystem() Return Code :\n" + LStrCallReturnCode + "\n" + ITcpClient.Client.RemoteEndPoint.ToString() );
                }
            }
            catch (Exception ex)
            {
                UMPService01.WriteLog(LogMode.Error, "UserIsOnlineUMPSystem()\n" + ex.Message + "\n" + ITcpClient.Client.RemoteEndPoint.ToString());
            }
        }

        private void AgentexLoginUMPSystem(string[] AStrArrayLoginInfo)
        {
            bool LBoolCallReturnValue = true;
            string LStrCallReturnCode = string.Empty;
            string LStrCallReturnMessage = string.Empty;
            List<string> LListStrOtherInfo = new List<string>();
            string LStrSendMessage = string.Empty;
            string LStrVerificationCode004 = string.Empty;
            string LStrLoginAsSaRole = string.Empty;

            try
            {
                LStrVerificationCode004 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004);
                if (AStrArrayLoginInfo.Length < 7)
                {
                    LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString("E01A00", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    SendMessageToClient(LStrSendMessage);          //参数错误
                    UMPService01.WriteLog(LogMode.Error, "AgentexLoginUMPSystem()\nReturnCode：E01A00");
                    return;
                }
                S01AOperations LS01AOperations = new S01AOperations(UMPService01.IIntDBType, UMPService01.IStrDBConnectProfile, ITcpClient);
                LListStrOtherInfo.Add(AStrArrayLoginInfo[4]);       //0登录应用程序模块ID
                LListStrOtherInfo.Add(AStrArrayLoginInfo[5]);       //1客户端机器名
                if (AStrArrayLoginInfo[4] == "11000")
                {
                    LListStrOtherInfo.Add(AStrArrayLoginInfo[6]);       //2客户端机器IP
                }
                else
                {
                    LListStrOtherInfo.Add(((IPEndPoint)ITcpClient.Client.RemoteEndPoint).Address.ToString());
                }
                LBoolCallReturnValue = LS01AOperations.S01AOperation14(AStrArrayLoginInfo[1], AStrArrayLoginInfo[2],
                    AStrArrayLoginInfo[3], LListStrOtherInfo, ref LStrCallReturnCode,
                    ref LStrCallReturnMessage);
                SendMessageToClient(LStrCallReturnCode);
                if (!LBoolCallReturnValue)
                {
                    UMPService01.WriteLog(LogMode.Error,
                        "AgentexLoginUMPSystem() Return Code :\n" + LStrCallReturnCode + "Return Message :\n" +
                        LStrCallReturnMessage + "\n" + ITcpClient.Client.RemoteEndPoint.ToString());
                }
                else
                {
                    UMPService01.WriteLog(LogMode.Info,
                        "AgentexLoginUMPSystem() Return Code :\n" + LStrCallReturnCode + "\n" +
                        ITcpClient.Client.RemoteEndPoint.ToString());
                }
            }
            catch (Exception ex)
            {
                UMPService01.WriteLog(LogMode.Error, "AgentexLoginUMPSystem()\n" + ex.Message + "\n" + ITcpClient.Client.RemoteEndPoint.ToString());
            }
        }

        private void AgentexLogoutUMPSystem(string[] AStrArrayLogoutInfo)
        {
            bool LBoolCallReturnValue = true;
            string LStrCallReturnCode = string.Empty;
            string LStrCallReturnMessage = string.Empty;
            string LStrSendMessage = string.Empty;
            string LStrVerificationCode004 = string.Empty;

            try
            {
                LStrVerificationCode004 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004);
                if (AStrArrayLogoutInfo.Length < 4)
                {
                    LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString("E01A00", LStrVerificationCode004,
                        EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    SendMessageToClient(LStrSendMessage);          //参数错误
                    UMPService01.WriteLog(LogMode.Error, "AgentexLogoutUMPSystem()\nReturnCode：E01A00");
                    return;
                }
                S01AOperations LS01AOperations = new S01AOperations(UMPService01.IIntDBType, UMPService01.IStrDBConnectProfile, ITcpClient);
                LBoolCallReturnValue = LS01AOperations.S01AOperation15(AStrArrayLogoutInfo[1], AStrArrayLogoutInfo[2],
                    AStrArrayLogoutInfo[3], ref LStrCallReturnCode, ref LStrCallReturnMessage);
                SendMessageToClient(LStrCallReturnCode);
                if (!LBoolCallReturnValue)
                {
                    UMPService01.WriteLog(LogMode.Error,
                        "AgentexLogoutUMPSystem() Return Code :\n" + LStrCallReturnCode + "Return Message :\n" +
                        LStrCallReturnMessage + "\n" + ITcpClient.Client.RemoteEndPoint.ToString());
                }
                else
                {
                    UMPService01.WriteLog(LogMode.Info,
                        "AgentexLogoutUMPSystem() Return Code :\n" + LStrCallReturnCode + "\n" +
                        ITcpClient.Client.RemoteEndPoint.ToString());
                }
            }
            catch (Exception ex)
            {
                UMPService01.WriteLog(LogMode.Error,
                    "AgentexLogoutUMPSystem()\n" + ex.Message + "\n" + ITcpClient.Client.RemoteEndPoint.ToString());
            }
        }

        private void AgentexIsOnlineUMPSystem(string[] AStrArrayLogoutInfo)
        {
            bool LBoolCallReturnValue = true;
            string LStrCallReturnCode = string.Empty;
            string LStrCallReturnMessage = string.Empty;
            string LStrSendMessage = string.Empty;
            string LStrVerificationCode004 = string.Empty;

            try
            {
                LStrVerificationCode004 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004);
                if (AStrArrayLogoutInfo.Length < 4)
                {
                    LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString("E01A00", LStrVerificationCode004,
                        EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    SendMessageToClient(LStrSendMessage);          //参数错误
                    UMPService01.WriteLog(LogMode.Error, "AgentexIsOnlineUMPSystem()\nReturnCode：E01A00");
                    return;
                }
                S01AOperations LS01AOperations = new S01AOperations(UMPService01.IIntDBType, UMPService01.IStrDBConnectProfile, ITcpClient);
                LBoolCallReturnValue = LS01AOperations.S01AOperation03(AStrArrayLogoutInfo[1], AStrArrayLogoutInfo[2],
                    AStrArrayLogoutInfo[3], ref LStrCallReturnCode, ref LStrCallReturnMessage);
                SendMessageToClient(LStrCallReturnCode);
                if (!LBoolCallReturnValue)
                {
                    UMPService01.WriteLog(LogMode.Error,
                        "AgentexIsOnlineUMPSystem() Return Code :\n" + LStrCallReturnCode + "Return Message :\n" +
                        LStrCallReturnMessage + "\n" + ITcpClient.Client.RemoteEndPoint.ToString());
                }
                else
                {
                    UMPService01.WriteLog(LogMode.Info,
                        "AgentexIsOnlineUMPSystem() Return Code :\n" + LStrCallReturnCode + "\n" +
                        ITcpClient.Client.RemoteEndPoint.ToString());
                }
            }
            catch (Exception ex)
            {
                UMPService01.WriteLog(LogMode.Error,
                    "AgentexIsOnlineUMPSystem()\n" + ex.Message + "\n" + ITcpClient.Client.RemoteEndPoint.ToString());
            }
        }

        private void SftpUserAuthentication(string[] AStrArrayAuthenticationInfo)
        {
            bool LBoolCallReturnValue = true;
            string LStrCallReturnCode = string.Empty;
            string LStrCallReturnMessage = string.Empty;
            string LStrSendMessage = string.Empty;
            string LStrVerificationCode004 = string.Empty;

            try
            {
                LStrVerificationCode004 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004);
                if (AStrArrayAuthenticationInfo.Length < 7)
                {
                    LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString("E01A00", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    SendMessageToClient(LStrSendMessage);          //参数错误
                    UMPService01.WriteLog(LogMode.Error, "SftpUserAuthentication()\nReturnCode：E01A00" );
                    return;
                }
                //foreach (string LStrSingleArgs in AStrArrayAuthenticationInfo)
                //{
                //    UMPService01.WriteLog(LogMode.Warn, "SftpUserAuthentication() " + LStrSingleArgs);
                //}
                S01AOperations LS01AOperations = new S01AOperations(UMPService01.IIntDBType, UMPService01.IStrDBConnectProfile, ITcpClient);
                LBoolCallReturnValue = LS01AOperations.S01AOperation21(AStrArrayAuthenticationInfo[1], AStrArrayAuthenticationInfo[2], AStrArrayAuthenticationInfo[3], AStrArrayAuthenticationInfo[4], AStrArrayAuthenticationInfo[5], AStrArrayAuthenticationInfo[6], ref LStrCallReturnCode, ref LStrCallReturnMessage);
                SendMessageToClient(LStrCallReturnCode);
                if (!LBoolCallReturnValue)
                {
                    UMPService01.WriteLog(LogMode.Error, "SftpUserAuthentication() Return Code :\n" + LStrCallReturnCode + "Return Message :\n" + LStrCallReturnMessage + "\n" + ITcpClient.Client.RemoteEndPoint.ToString() );
                }
                else
                {
                    UMPService01.WriteLog(LogMode.Info, "SftpUserAuthentication() Return Code :\n" + LStrCallReturnCode + "\n" + ITcpClient.Client.RemoteEndPoint.ToString() );
                }
            }
            catch (Exception ex)
            {
                UMPService01.WriteLog(LogMode.Error, "SftpUserAuthentication()\n" + ex.Message + "\n" + ITcpClient.Client.RemoteEndPoint.ToString());
            }
        }

        private void WriteOperationsLog(string[] AStrArrayOperationInfo)
        {
            bool LBoolCallReturnValue = true;
            string LStrCallReturnCode = string.Empty;
            string LStrCallReturnMessage = string.Empty;
            List<string> LListStrOperationInfo = new List<string>();
            string LStrSendMessage = string.Empty;
            string LStrVerificationCode004 = string.Empty;
            try
            {
                LStrVerificationCode004 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004);

                #region 参数数量错误，返回
                if (AStrArrayOperationInfo.Length < 14)
                {
                    LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString("E01B00", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    SendMessageToClient(LStrSendMessage);          //参数错误
                    UMPService01.WriteLog(LogMode.Error, "5.UserLoginUMPSystem()\nReturnCode：E01B00" );
                    return;
                }
                #endregion

                #region 初始化需要的数据List<string>
                LListStrOperationInfo.Add(AStrArrayOperationInfo[1]);           //0：客户端SessionID
                LListStrOperationInfo.Add(AStrArrayOperationInfo[2]);           //1：模块ID
                LListStrOperationInfo.Add(AStrArrayOperationInfo[3]);           //2：功能操作编号
                LListStrOperationInfo.Add(AStrArrayOperationInfo[4]);           //3：租户Token（5位）
                LListStrOperationInfo.Add(AStrArrayOperationInfo[5]);           //4：操作用户ID
                LListStrOperationInfo.Add(AStrArrayOperationInfo[6]);           //5：当前操作角色
                LListStrOperationInfo.Add(AStrArrayOperationInfo[7]);           //6：机器名
                if (AStrArrayOperationInfo[2] == "11000")                       //7：机器IP
                { LListStrOperationInfo.Add(AStrArrayOperationInfo[8]); }
                else { LListStrOperationInfo.Add(((IPEndPoint)ITcpClient.Client.RemoteEndPoint).Address.ToString()); }
                LListStrOperationInfo.Add(DateTime.UtcNow.ToString());          //8：操作时间 UTC
                LListStrOperationInfo.Add(AStrArrayOperationInfo[10]);           //9：操作结果
                LListStrOperationInfo.Add(AStrArrayOperationInfo[11]);          //10：操作内容对应的语言包ID
                LListStrOperationInfo.Add(AStrArrayOperationInfo[12]);          //11：替换参数
                LListStrOperationInfo.Add(AStrArrayOperationInfo[13]);          //12：异常错误
                #endregion

                #region 将操作日志写入数据库
                S01BOperations LS01BOperations = new S01BOperations(UMPService01.IIntDBType, UMPService01.IStrDBConnectProfile, ITcpClient);
                LBoolCallReturnValue = LS01BOperations.S01BOperation01(LListStrOperationInfo, ref LStrCallReturnCode, ref LStrCallReturnMessage);
                SendMessageToClient(LStrCallReturnCode);
                #endregion

                if (!LBoolCallReturnValue)
                {
                    UMPService01.WriteLog(LogMode.Error, "WriteOperationsLog() Return Code :\n" + LStrCallReturnCode + "Return Message :\n" + LStrCallReturnMessage + "\n" + ITcpClient.Client.RemoteEndPoint.ToString() );
                }
            }
            catch (Exception ex)
            {
                UMPService01.WriteLog(LogMode.Error, "WriteOperationsLog()\n" + ex.Message + "\n" + ITcpClient.Client.RemoteEndPoint.ToString());
            }
        }

        private void GetDatabaseConnectionProfile(string[] AStrArrayOperationInfo)
        {
            string LStrXmlFileName = string.Empty;
            string LStrVerificationCode = string.Empty;

            string LStrAttributesData = string.Empty;
            //0:数据库服务器；1：端口；2：数据库名或服务名；3：登录用户；4：登录密码；5：其他参数
            List<string> LListStrDBProfile = new List<string>();

            try
            {

                LStrXmlFileName = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

                #region 重新启动Service 07
                //string[] LStrArrayWriteLine = new string[2];
                //Stream LStreamBatch = File.Create(LStrXmlFileName + @"\RestarService.bat");
                //LStreamBatch.Close();
                //LStrArrayWriteLine[0] = "net stop \"UMP Service 07\"";
                //LStrArrayWriteLine[1] = "net start \"UMP Service 07\"";
                //File.WriteAllLines(LStrXmlFileName + @"\RestarService.bat", LStrArrayWriteLine);
                //ExecuteBatchCommand(LStrXmlFileName + @"\RestarService.bat");
                //File.Delete(LStrXmlFileName + @"\RestarService.bat");
                #endregion
                

                LStrXmlFileName = Path.Combine(LStrXmlFileName, @"UMP.Server\Args01.UMP.xml");
                if (!File.Exists(LStrXmlFileName))
                {
                    UMPService01.WriteLog(LogMode.Warn, "GetDatabaseConnectionProfile() \nThe Database Connection Parameters Is Not Configured");
                    return;
                }
                XmlDocument LXmlDocArgs01 = new XmlDocument();
                LXmlDocArgs01.Load(LStrXmlFileName);
                XmlNodeList LXmlNodeListDatabase = LXmlDocArgs01.SelectSingleNode("DatabaseParameters").ChildNodes;

                LStrVerificationCode = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104);

                #region 数据库连接参数
                foreach (XmlNode LXmlNodeSingleDatabase in LXmlNodeListDatabase)
                {
                    LStrAttributesData = LXmlNodeSingleDatabase.Attributes["P03"].Value;
                    LStrAttributesData = EncryptionAndDecryption.EncryptDecryptString(LStrAttributesData, LStrVerificationCode, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                    if (LStrAttributesData != "1") { continue; }

                    //数据库类型
                    LStrAttributesData = LXmlNodeSingleDatabase.Attributes["P02"].Value;
                    LStrAttributesData = EncryptionAndDecryption.EncryptDecryptString(LStrAttributesData, LStrVerificationCode, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                    UMPService01.IIntDBType = int.Parse(LStrAttributesData);

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

                if (UMPService01.IIntDBType == 2)
                {
                    UMPService01.IStrDBConnectProfile = string.Format("Data Source={0},{1};Initial Catalog={2};User Id={3};Password={4}", LListStrDBProfile[0], LListStrDBProfile[1], LListStrDBProfile[2], LListStrDBProfile[3], LListStrDBProfile[4]);
                    LStrDBConnectProfile = string.Format("Data Source={0},{1};Initial Catalog={2};User Id={3};Password={4}", LListStrDBProfile[0], LListStrDBProfile[1], LListStrDBProfile[2], LListStrDBProfile[3], "******");
                    LStrDBConnectProfile = "DataBase Type : MS SQL Server\n" + LStrDBConnectProfile;
                }
                if (UMPService01.IIntDBType == 3)
                {
                    UMPService01.IStrDBConnectProfile = string.Format("Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST={0}) (PORT={1})))(CONNECT_DATA=(SERVICE_NAME= {2})));User Id={3}; Password={4}", LListStrDBProfile[0], LListStrDBProfile[1], LListStrDBProfile[2], LListStrDBProfile[3], LListStrDBProfile[4]);
                    LStrDBConnectProfile = string.Format("Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST={0}) (PORT={1})))(CONNECT_DATA=(SERVICE_NAME= {2})));User Id={3}; Password={4}", LListStrDBProfile[0], LListStrDBProfile[1], LListStrDBProfile[2], LListStrDBProfile[3], "******");
                    LStrDBConnectProfile = "DataBase Type : Oracle\n" + LStrDBConnectProfile;
                }
                #endregion

                UMPService01.WriteLog(LogMode.Info, "GetDatabaseConnectionProfile() \n" + LStrDBConnectProfile );
            }
            catch (Exception ex)
            {
                UMPService01.WriteLog(LogMode.Error, "GetDatabaseConnectionProfile() Failed\n" + ex.Message );
            }

        }

        //执行批处理文件
        private void ExecuteBatchCommand(string AStrBatchFileName)
        {
            try
            {
                Process LocalProExec = new Process();

                LocalProExec.StartInfo.FileName = AStrBatchFileName;
                LocalProExec.StartInfo.UseShellExecute = false;
                LocalProExec.StartInfo.RedirectStandardInput = true;
                LocalProExec.StartInfo.RedirectStandardOutput = true;
                LocalProExec.StartInfo.RedirectStandardError = true;
                LocalProExec.StartInfo.CreateNoWindow = true;

                LocalProExec.Start();
                LocalProExec.WaitForExit();
                if (LocalProExec.HasExited == false)
                {
                    LocalProExec.Kill();
                }
                LocalProExec.Dispose();
            }
            catch (Exception ex)
            {
                UMPService01.WriteLog(LogMode.Error, "ExecuteBatchCommand() Failed\n" + ex.Message);
            }
        }

        /// <summary>
        /// 加密或解密数据
        /// </summary>
        /// <param name="AStrEncDecInformation">
        /// 1：加密或解密方法
        /// 2：待加密或解密串
        /// </param>
        private void EncryptionAndDecryptionString(string[] AStrEncDecInformation)
        {
            string LStrSendMessage = string.Empty;
            string LStrVerificationCode = string.Empty;

            try
            {
                UMPService01.WriteLog(LogMode.Info, "AStrEncDecInformation[0] = " + AStrEncDecInformation[0]);
                UMPService01.WriteLog(LogMode.Info,"AStrEncDecInformation[1] = " + AStrEncDecInformation[1]);
                UMPService01.WriteLog(LogMode.Info,"AStrEncDecInformation[2] = " + AStrEncDecInformation[2]);
                switch (AStrEncDecInformation[1])
                {
                    case "M102":
                        LStrVerificationCode = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M102);
                        LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString(AStrEncDecInformation[2], LStrVerificationCode, EncryptionAndDecryption.UMPKeyAndIVType.M102);
                        break;
                    case "SM001":
                        LStrVerificationCode = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M001);
                        LStrSendMessage = EncryptionAndDecryption.EncryptStringSHA512(AStrEncDecInformation[2], LStrVerificationCode, EncryptionAndDecryption.UMPKeyAndIVType.M001);
                        break;
                    case "SM002":
                        LStrVerificationCode = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M002);
                        LStrSendMessage = EncryptionAndDecryption.EncryptStringSHA512(AStrEncDecInformation[2], LStrVerificationCode, EncryptionAndDecryption.UMPKeyAndIVType.M002);
                        break;
                    default:
                        break;
                }
                if (string.IsNullOrEmpty(LStrSendMessage)) { LStrSendMessage = AscCodeToChr(27) + AscCodeToChr(27) + AscCodeToChr(27) + AscCodeToChr(27) + AscCodeToChr(27); }
                UMPService01.WriteLog(LogMode.Info, "Return Message = " + LStrSendMessage);
                SendMessageToClient(LStrSendMessage);
            }
            catch (Exception ex)
            {
                UMPService01.WriteLog(LogMode.Error, "EncryptionAndDecryptionString()\n" + ex.Message + "\n" + ITcpClient.Client.RemoteEndPoint.ToString() );
            }
        }

        /// <summary>
        /// 录音文件从本地回删后，回写数据库标志位
        /// </summary>
        /// <param name="AStrUpdateConditions">
        /// 0-操作码
        /// 1-媒体类型 1，2
        /// 2-月份 YYYYMM
        /// 3-机器名
        /// 4-IP地址列表  IP+ CHAR(30) + IP .....
        /// </param>
        private void Update21001C033(string[] AStrUpdateConditions)
        {
            List<string> LListStrUpdConditions = new List<string>();
            string LStrSendMessage = string.Empty;
            string LStrVerificationCode004 = string.Empty;

            try
            {
                LStrVerificationCode004 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004);
                foreach (string LStrSingleCondition in AStrUpdateConditions) { LListStrUpdConditions.Add(LStrSingleCondition); }
                lock (UMPService01.ILockObjectUpdate21001033)
                {
                    UMPService01.IYoungCircleQueueUpdate21001033.PushElement(LListStrUpdConditions);
                }
                LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString("RM01E01", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                SendMessageToClient(LStrSendMessage);
            }
            catch (Exception ex)
            {
                LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString("EM01E01", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                SendMessageToClient(LStrSendMessage);
                UMPService01.WriteLog(LogMode.Error, "Update21001C033()\n" + ex.Message + "\n" + ITcpClient.Client.RemoteEndPoint.ToString());
            }
        }

        /// <summary>
        /// 录音文件从归档设备删除后，回写数据库标志
        /// </summary>
        /// <param name="AStrUpdateConditions">
        /// 0-操作码
        /// 1-媒体类型 1，2
        /// 2-月份 YYYYMM
        /// 3-机器名
        /// 4-存储设备ID（19位）
        /// </param>
        private void Update21001C23X(string[] AStrUpdateConditions)
        {
            List<string> LListStrUpdConditions = new List<string>();
            string LStrSendMessage = string.Empty;
            string LStrVerificationCode004 = string.Empty;

            try
            {
                LStrVerificationCode004 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004);
                foreach (string LStrSingleCondition in AStrUpdateConditions) { LListStrUpdConditions.Add(LStrSingleCondition); }
                lock (UMPService01.ILockObjectUpdate2100123X)
                {
                    UMPService01.IYoungCircleQueueUpdate2100123X.PushElement(LListStrUpdConditions);
                }
                LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString("RM01E03", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                SendMessageToClient(LStrSendMessage);
            }
            catch (Exception ex)
            {
                LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString("EM01E03", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                SendMessageToClient(LStrSendMessage);
                UMPService01.WriteLog(LogMode.Error, "Update21001C23X()\n" + ex.Message + "\n" + ITcpClient.Client.RemoteEndPoint.ToString());
            }
        }

        /// <summary>
        /// 从应用服务器上获取压缩文件，返回URL和解压密码
        /// </summary>
        /// <param name="AStrRequestArguments">
        /// 0-操作码 M01E02
        /// 1-用户登录后系统分配的流水号
        /// 2-文件类型 0:full，VX：全部voice，V*：指定voiceid，P：PBXDevice，S：Simple
        /// 3-是否加密
        /// 4-是否开启64位加密方法
        /// </param>
        private void ReturnCompressedFiles(string[] AStrRequestArguments)
        {
            string LStrZipPassword = string.Empty;
            string LStrXmlFileName = string.Empty;
            string LStrSendMessage = string.Empty;
            string LStrVerificationCode004 = string.Empty;
            string LStrP01 = string.Empty, LStrP02 = string.Empty;
            string LStrSourceFolder = string.Empty;
            List<string> LListStrSourceFile = new List<string>();
            string LStrTargetFolder = string.Empty;
            string LStrTargetFileName = string.Empty;
            

            try
            {
                LStrVerificationCode004 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004);
                LStrXmlFileName = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                LStrXmlFileName = Path.Combine(LStrXmlFileName, @"UMP.Server\Args02.UMP.xml");
                XmlDocument LXmlDocArgs02 = new XmlDocument();
                LXmlDocArgs02.Load(LStrXmlFileName);
                XmlNode LXmlNodeListConfigrations = LXmlDocArgs02.SelectSingleNode("Parameters02").SelectSingleNode("LGConfig");
                LStrP01 = LXmlNodeListConfigrations.Attributes["P01"].Value;
                LStrP02 = LXmlNodeListConfigrations.Attributes["P02"].Value;
                if (LStrP01 == "CAD") { LStrSourceFolder = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), LStrP02); }

                if (AStrRequestArguments[2] == "0")
                {
                    LListStrSourceFile.Add(System.IO.Path.Combine(LStrSourceFolder, "umpparam_full.xml"));
                }

                if (AStrRequestArguments[3] == "1")
                {
                    LStrZipPassword = PFShareClassesS.PasswordVerifyOptions.GeneratePassword(32, 6);
                }
                LStrTargetFolder = System.IO.Path.Combine(UMPService01.IStrBaseDirectory, "MediaData");
                LStrTargetFileName = AStrRequestArguments[1] + "." + DateTime.UtcNow.ToString("yyyyMMddHHmmss") + ".zip";

                using (ZipOutputStream LZipOutStream = new ZipOutputStream(File.Create(System.IO.Path.Combine(LStrTargetFolder, LStrTargetFileName))))
                {
                    LZipOutStream.SetLevel(9);
                    if (AStrRequestArguments[4] == "0") { LZipOutStream.UseZip64 = UseZip64.Off; }
                    byte[] LByteBuffer = new byte[1024];
                    if (!string.IsNullOrEmpty(LStrZipPassword)) { LZipOutStream.Password = LStrZipPassword; }
                    foreach (string LStrSingleFile in LListStrSourceFile)
                    {
                        ZipEntry LZipEntry = new ZipEntry(Path.GetFileName(LStrSingleFile));
                        LZipEntry.DateTime = DateTime.UtcNow;
                        LZipOutStream.PutNextEntry(LZipEntry);
                        using (FileStream LFileStream = File.OpenRead(LStrSingleFile))
                        {
                            int LIntSourceBytes;
                            do
                            {
                                LIntSourceBytes = LFileStream.Read(LByteBuffer, 0, LByteBuffer.Length);
                                LZipOutStream.Write(LByteBuffer, 0, LIntSourceBytes);
                            } while (LIntSourceBytes > 0);

                        }
                    }
                    LZipOutStream.Finish(); LZipOutStream.Close();
                }

                LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString("RM01E02", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                LStrSendMessage += AscCodeToChr(27) + EncryptionAndDecryption.EncryptDecryptString("MediaData/" + LStrTargetFileName, LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                LStrSendMessage += AscCodeToChr(27) + EncryptionAndDecryption.EncryptDecryptString(LStrZipPassword, LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                UMPService01.WriteLog(LogMode.Info, "ReturnCompressedFiles()\n" + LStrSendMessage + "\n" + ITcpClient.Client.RemoteEndPoint.ToString() );
                SendMessageToClient(LStrSendMessage);
            }
            catch (Exception ex)
            {
                LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString("EM01E02", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                SendMessageToClient(LStrSendMessage);
                UMPService01.WriteLog(LogMode.Error, "ReturnCompressedFiles()\n" + ex.Message + "\n" + ITcpClient.Client.RemoteEndPoint.ToString());
            }
        }

        /// <summary>
        /// 从应用服务器上获取压缩文件，返回URL和解压密码
        /// </summary>
        /// <param name="AStrRequestArguments">
        /// 0-操作码 M01E04
        /// 1-用户登录后系统分配的流水号
        /// 2-文件类型 0:full，VX：全部voice，V*：指定voiceid，P：PBXDevice，S：Simple
        /// 3-是否加密
        /// 4-是否开启64位加密方法
        /// </param>
        private void ReturnSoftDogSerialNum(string[] AStrRequestArguments)
        {
            string LStrZipPassword = string.Empty;
            string LStrSendMessage = string.Empty;
            string LStrVerificationCode004 = string.Empty;
            string LStrVerificationCode101 = string.Empty;
            string LStrTargetFolder = string.Empty;
            string LStrTargetFileName = string.Empty;
            string LStrDogNumber = string.Empty;

            try
            {
                LStrVerificationCode004 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004);
                LStrVerificationCode101 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M101);

                LStrTargetFolder = System.IO.Path.Combine(UMPService01.IStrBaseDirectory, "GlobalSettings");
                LStrTargetFileName = LStrTargetFolder+"\\UMP.Young.01";
                FileStream fs = new FileStream(LStrTargetFileName, FileMode.Open, FileAccess.Read);//读取文件设定
                StreamReader m_streamReader = new StreamReader(fs);//设定读写的编码
                //使用StreamReader类来读取文件  
                m_streamReader.BaseStream.Seek(0, SeekOrigin.Begin);
                string strLine = m_streamReader.ReadLine();
                LStrDogNumber = EncryptionAndDecryption.EncryptDecryptString(strLine, LStrVerificationCode101, EncryptionAndDecryption.UMPKeyAndIVType.M101);
                
                LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString("RM01E04", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                LStrSendMessage += AscCodeToChr(27) + EncryptionAndDecryption.EncryptDecryptString(LStrDogNumber, LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                UMPService01.WriteLog(LogMode.Info, "ReturnCompressedFiles()\n" + LStrSendMessage + "\n" + ITcpClient.Client.RemoteEndPoint.ToString() );
                SendMessageToClient(LStrSendMessage);
            }
            catch (Exception ex)
            {
                LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString("EM01E04", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                SendMessageToClient(LStrSendMessage);
                UMPService01.WriteLog(LogMode.Error, "ReturnCompressedFiles()\n" + ex.Message + "\n" + ITcpClient.Client.RemoteEndPoint.ToString());
            }
        }

        private string AscCodeToChr(int AsciiCode)
        {
            System.Text.ASCIIEncoding asciiEncoding = new System.Text.ASCIIEncoding();
            byte[] byteArray = new byte[] { (byte)AsciiCode };
            string strCharacter = asciiEncoding.GetString(byteArray);
            return (strCharacter);
        }

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

        private bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            //if (sslPolicyErrors == SslPolicyErrors.RemoteCertificateChainErrors || sslPolicyErrors == SslPolicyErrors.None) { return true; }
            //return false;
            return true;
        }
    }

    public class ClientOpArgsAndReturn
    {
        public List<string> LListArguments = new List<string>();
        public bool LBoolReturn = true;
        public long LLongClientSessionID = 0;
        public DateTime LDateTimeOperation = DateTime.UtcNow;
        public string LStrReturnCode = string.Empty;
        public string LStrReturnMessage = string.Empty;
    }
}