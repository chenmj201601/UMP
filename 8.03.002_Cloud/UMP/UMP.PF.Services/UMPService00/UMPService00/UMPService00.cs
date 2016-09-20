using PFShareClassesS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.IO;
using System.Security.Authentication;
using VoiceCyber.UMP.Common;
using VoiceCyber.Common;
using System.Reflection;
using VoiceCyber.UMP.Encryptions;

namespace UMPService00
{
    public partial class UMPService00 : ServiceBase
    {
        public static string IStrEventLogSource = "Service 00";
        public static string IStrApplicationName = "UMP";
        private int IIntThisServicePort = 0;
        private string IStrBaseDirectory = string.Empty;
        private static LogOperator mLogOperator;

        public static EventLog IEventLog = null;
        private static readonly object ILockObject = new object();

        /// <summary>
        /// 服务是否可以继续工作
        /// </summary>
        public bool IBoolCanContinue = true;
        /// <summary>
        /// 服务是否正在处理过程中
        /// </summary>
        private bool IBoolIsBusing = false;

        public static List<ClientOperations> GlobalListConnectedClient = new List<ClientOperations>();
        public static List<Thread> GlobalListClientThread = new List<Thread>();

        private Thread ThreadCheckThreadIsAlive = null;

        Thread LThreadLogOperator;
        Thread LThreadGenerateXml;

        public static X509Certificate2 IX509CertificateServer = null;
        static TcpListener ITcpListener = null;

        private LogOperation mLogOperation;

        public UMPService00()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {

            CreateFileLog();
            WriteLog(LogMode.Info, string.Format("Service starting..."));
            
            
            Thread LThreadStart = new Thread(new ThreadStart(StartService00));
            LThreadStart.Start();
            CreatelLocalMachineIni();
            LThreadGenerateXml = new Thread(new ThreadStart(GenerateXmlOnStart));
            LThreadGenerateXml.Start();

            mLogOperation = new LogOperation();
            LThreadLogOperator = new Thread(new ThreadStart(mLogOperation.LogCompressionAndDelete));
            LThreadLogOperator.Start();
        }

        /// <summary>
        /// 在服务启动时 生成参数xml
        /// </summary>
        private void GenerateXmlOnStart()
        {
            try
            {
                bool bIsUMPServer = false;
                //判断是不是UMP服务器
                bIsUMPServer = GloableParamOperation.CheckIsUMPServer();
                string strCurrentPath = Common.GetCurrentBaseDirectory().Trim('\\');
                GloableParamOperation gbParam =null;
                int iWebPort = 0;
                string strWebHost = string.Empty;
                bool bResult = false;
                string strAuthenServerHost = string.Empty;
                string strAuthenServerPort = string.Empty;

                //如果获取端口出错 就假设是Voice服务器  从ProgramData\VoiceServer下获取IIS绑定信息
                if (!bIsUMPServer)
                {
         
                    UMPService00.WriteLog("voice server ");

                    gbParam = new GloableParamOperation(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\VoiceServer");
                    bResult = gbParam.GetWebSitePort(ref iWebPort, ref strWebHost);
                    strAuthenServerHost = strWebHost;
                    strAuthenServerPort = iWebPort.ToString();
                }
                else
                {
                    //如果能获取到IIS绑定信息 则Host=127.0.0.1 并获取认证服务器信息
                    //UMPService00.IEventLog.WriteEntry("UMP server");
                    UMPService00.WriteLog("UMP server ");
                    gbParam = new GloableParamOperation(strCurrentPath.Trim('\\'));
                    bResult = gbParam.GetWebSitePort(ref iWebPort);
                    bIsUMPServer = true;
                    strWebHost = "127.0.0.1";
                    List<string> lstAuthenServerInfo = new List<string>();
                    bool IsGetAuthenServer = gbParam.GetAuthenticateServerInfo(ref lstAuthenServerInfo);
                    if (!IsGetAuthenServer)
                    {
                        //UMPService00.IEventLog.WriteEntry("Get Authenticate Server Info error", EventLogEntryType.Error);
                        UMPService00.WriteLog(LogMode.Error, "Get Authenticate Server Info error");
                    }
                    else
                    {
                        strAuthenServerHost = lstAuthenServerInfo[0];
                        strAuthenServerPort = lstAuthenServerInfo[1];
                    }
                }
                if (!bResult)
                {
                   // UMPService00.IEventLog.WriteEntry("GenerateXmlOnStart() error : Get website info error", EventLogEntryType.Warning);
                    UMPService00.WriteLog(LogMode.Warn, "GenerateXmlOnStart() error : Get website info error");

                    return;
                }

                //先假设本机为UMP服务器 从ProgramData\UMP.Server下获得数据库信息
                string strDBFilePath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                DatabaseInfo dbInfo = null;
                bResult = DataBaseXmlOperator.GetDBInfo(strDBFilePath, ref dbInfo);

                if (!bResult)
                {
                    strDBFilePath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\VoiceServer";
                    bResult = DataBaseXmlOperator.GetDBInfo(strDBFilePath, ref dbInfo);
                }
                //如果没有获取到数据库信息 那假设本机为Voice服务器 从C:\ProgramData\VoiceServer\UMP.Server下获取数据库信息
                if (!bResult)
                {
                    //UMPService00.IEventLog.WriteEntry("GenerateXmlOnStart() error : Get database info error", EventLogEntryType.Warning);
                    UMPService00.WriteLog(LogMode.Warn, "GenerateXmlOnStart() error : Get database info error");
                    return;
                }
                //UMPService00.IEventLog.WriteEntry("GenerateXmlOnStart dbinfo = "+dbInfo.TypeName + "," + dbInfo.Host + ":" + dbInfo.Port + "," + dbInfo.LoginName + "," + dbInfo.Password + "," + dbInfo.DBName, EventLogEntryType.Warning);
                
                AppServerInfo serverInfo = new AppServerInfo();
                serverInfo.Protocol = "http";
                serverInfo.Address = strWebHost;
                serverInfo.Port = iWebPort;
                serverInfo.SupportHttps = false;

                int iResult = 0;
                try
                {
                    string strPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\Temp";
                    GenerateXml generate = new GenerateXml(strPath, dbInfo, serverInfo, strAuthenServerHost, strAuthenServerPort);
                    bool bIsParamsExists = false;
                    iResult = generate.Generate(bIsUMPServer, ref bIsParamsExists);
                    generate.ClenData();
                }
                catch (Exception ex)
                {
                    //UMPService00.IEventLog.WriteEntry("GenerateXmlOnStart() Geneerate xml failed ,ErrorInfo : " + ex.Message, EventLogEntryType.Error);
                    UMPService00.WriteLog(LogMode.Error, "GenerateXmlOnStart() Geneerate xml failed ,ErrorInfo : " + ex.Message);
                }

                if (iResult == 0)
                {
                   // UMPService00.IEventLog.WriteEntry("Replace param xml success", EventLogEntryType.Information);
                    UMPService00.WriteLog(LogMode.Info, "Replace param xml success");
                    //创建socket 开始发送广播
                    string strSimpFilePath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\VoiceCyber\\UMP\\config" + "\\umpparam_simp.xml";
                    SendBroadcastData.WriteLogPath(strSimpFilePath);
                    SendBroadcastData.SendBroadcastMessage(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\VoiceCyber\\UMP\\config");
                }
            }
            catch (Exception ex)
            {
               // UMPService00.IEventLog.WriteEntry("GenerateXmlOnStart() Error : " + ex.Message, EventLogEntryType.Error);

                UMPService00.WriteLog(LogMode.Error, "GenerateXmlOnStart() Error : " + ex.Message);

            }
        }

        protected override void OnStop()
        {
            try
            {
                IBoolCanContinue = false;
                while (IBoolIsBusing) { Thread.Sleep(100); }
                if (ITcpListener != null) { ITcpListener.Stop(); ITcpListener = null; }
                CloseAllClientThread();
                if (LThreadLogOperator != null && LThreadLogOperator.ThreadState == System.Threading.ThreadState.Running)
                {
                    LThreadLogOperator.Abort();
                }
                if (LThreadGenerateXml != null && LThreadGenerateXml.ThreadState == System.Threading.ThreadState.Running)
                {
                    LThreadGenerateXml.Abort();
                }
                WriteEntryLog("Service Stopped At : " + DateTime.Now.ToString("G"), EventLogEntryType.Information);

                
            }
            catch (Exception ex)
            {
                WriteEntryLog("Stop Service Failed\n" + ex.ToString(), EventLogEntryType.Error);
            }

            if (mLogOperator != null)
            {
                mLogOperator.Stop();
            }
        }

        /// <summary>
        ///  修改或创建LocalMachine.ini 获得本机的IP协议  写入多播的IP和端口、MachineID
        /// </summary>
        private void CreatelLocalMachineIni()
        {
            string strProgramDataPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\VoiceCyber\\UMP\\config";
            DirectoryInfo dir = new DirectoryInfo(strProgramDataPath);
            if (!dir.Exists)
            {
                dir.Create();
            }
            string strIniPath = strProgramDataPath + "\\localmachine.ini";
            FileInfo fi = new FileInfo(strIniPath);
            if (!fi.Exists)
            {
                FileStream fs = fi.Create();
                fs.Close();
            }
            IniOperation ini = new IniOperation(strIniPath);

            string strHostName = Dns.GetHostName(); //得到本机的主机名
            IPHostEntry ipEntry = Dns.GetHostEntry(strHostName);
            if (ipEntry.AddressList.Count() > 0)
            {
                foreach (IPAddress adress in ipEntry.AddressList)
                {
                    if (adress.AddressFamily == AddressFamily.InterNetwork)
                    {
                        ini.IniWriteValue("LocalMachine", "SubscribeAddress", "224.0.2.26,3789");
                        break;
                    }
                    //else if (adress.AddressFamily == AddressFamily.InterNetworkV6)
                    //{
                    //    ini.IniWriteValue("LocalMachine", "SubscribeAddress", "ff01::0226,3789");
                    //    break;
                    //}
                    continue;
                }
            }
            string strMachineID = ini.IniReadValue("LocalMachine", "MachineID");
            if (string.IsNullOrEmpty(strMachineID))
            {
                ini.IniWriteValue("LocalMachine", "MachineID", Guid.NewGuid().ToString());
            }
        }

        private void StartService00()
        {
            if (!EventLog.SourceExists(IStrEventLogSource)) { EventLog.CreateEventSource(IStrEventLogSource, IStrApplicationName); }
            UMPServiceLog.Source = IStrEventLogSource;
            UMPServiceLog.Log = IStrApplicationName;
            UMPServiceLog.ModifyOverflowPolicy(OverflowAction.OverwriteOlder, 3);
            IEventLog = UMPServiceLog;
            WriteEntryLog("Service Started At : " + DateTime.Now.ToString("G"), EventLogEntryType.Information);

            IStrBaseDirectory = Common.GetCurrentBaseDirectory();
            if (string.IsNullOrEmpty(IStrBaseDirectory)) { return; }

            IIntThisServicePort = 8009;

            if (!CreateCheckThreadIsAlive()) { return; }
            StartTcpSSLServer(IIntThisServicePort);
            WaitForSocketClientConnect();

           
        }

        /// <summary>
        /// 将日志写入 Windows 事情
        /// </summary>
        /// <param name="AStrWriteBody">日志内容</param>
        /// <param name="AEntryType">日志类型</param>
        private void WriteEntryLog(string AStrWriteBody, EventLogEntryType AEntryType)
        {
            try
            {
               // UMPServiceLog.WriteEntry(AStrWriteBody, AEntryType);

                WriteLog(AEntryType == EventLogEntryType.Error ? LogMode.Error : LogMode.Info, AStrWriteBody);
            }
            catch { }
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
                WriteEntryLog("CreateCheckThreadIsAlive()  Begin", EventLogEntryType.Information);
                ThreadCheckThreadIsAlive = new System.Threading.Thread(new System.Threading.ThreadStart(CheckThreadIsAliveAction));
                ThreadCheckThreadIsAlive.Start();
                WriteEntryLog("CreateCheckThreadIsAlive()  Finished", EventLogEntryType.Information);
            }
            catch (Exception ex)
            {
                WriteEntryLog("CreateCheckThreadIsAlive()  Failed\n" + ex.ToString(), EventLogEntryType.Error);
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
                    if (!LBoolReturn) { WriteEntryLog(LStrRetrun, EventLogEntryType.Error); }

                    Thread.Sleep(5000);
                }
            }
            catch (Exception ex)
            {
                WriteEntryLog("CheckThreadIsAliveAction()  Failed\n" + ex.ToString(), EventLogEntryType.Error);
            }
        }

        private void AddConnectedClientAndThread(ClientOperations AClientOperation, Thread AClientThread, ref bool ABoolReturn, ref string AStrReturn)
        {
            try
            {
                ABoolReturn = true;
                AStrReturn = string.Empty;

                lock (ILockObject)
                {
                    GlobalListConnectedClient.Add(AClientOperation);
                    GlobalListClientThread.Add(AClientThread);
                    WriteEntryLog("AddConnectedClientAndThread() GlobalListClientThread.Count = " + GlobalListClientThread.Count.ToString() + "    GlobalListConnectedClient.Count = " + GlobalListConnectedClient.Count.ToString(), EventLogEntryType.Information);
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

                lock (ILockObject)
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
                        WriteEntryLog("RemoveIsNotAliveThread() GlobalListClientThread.Count = " + AIntThreadCount.ToString() + "    GlobalListConnectedClient.Count = " + AIntConnectedCount.ToString(), EventLogEntryType.Information);
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

        private void StartTcpSSLServer(int AIntPort)
        {
            string LStrInstallReturn = string.Empty;

            try
            {
                WriteEntryLog("StartTcpSSLServer() Begin", EventLogEntryType.Information);

                X509Store LX509Store = new X509Store(StoreName.My, StoreLocation.LocalMachine);

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
                //    WriteEntryLog("StartTcpSSLServer() Install CertificateFile (UMP.PF.Certificate.pfx) Begin", EventLogEntryType.Information);
                //    LStrInstallReturn = InstallCertificateFile();
                //    if (!string.IsNullOrEmpty(LStrInstallReturn))
                //    {
                //        WriteEntryLog("StartTcpSSLServer() Install CertificateFile (UMP.PF.Certificate.pfx) Failed\n" + LStrInstallReturn, EventLogEntryType.Error);
                //        return;
                //    }
                //    else
                //    {
                //        WriteEntryLog("StartTcpSSLServer() Install CertificateFile (UMP.PF.Certificate.pfx) Finished", EventLogEntryType.Information);
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
                //        WriteEntryLog("StartTcpSSLServer() Certificates Find Failed (2D508A175B6836ADB6E220BA63E00F2F0881E75F)", EventLogEntryType.Error);
                //    }
                //}

                //WriteEntryLog("IX509CertificateServer.GetCertHashString() " + IX509CertificateServer.GetCertHashString(), EventLogEntryType.Information);

                #region 使用安全证书方式 2015-07-06 修改
                //List<string> lstDirs = Directory.GetDirectories(IStrBaseDirectory.Trim('\\')).ToList();
                //WriteEntryLog("lstDirs.count = " + lstDirs.Count, EventLogEntryType.Warning);
                string strDir = string.Empty;
                //if (lstDirs.Contains(System.IO.Path.Combine(IStrBaseDirectory,@"WinServices")))
                //{
                //    strDir = System.IO.Path.Combine(IStrBaseDirectory, @"WinServices\UMP.SSL.Certificate.pfx");
                //}
                //else
                //{
                //    strDir = System.IO.Path.Combine(IStrBaseDirectory, @"UMP.SSL.Certificate.pfx");
                //}

                strDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UMP.SSL.Certificate.pfx");

                WriteEntryLog("IX509CertificateServerPath = " + strDir, EventLogEntryType.Warning);
                IX509CertificateServer = new X509Certificate2(strDir, "VoiceCyber,123");

                #endregion

                ITcpListener = new TcpListener(IPAddress.Any, AIntPort);       
                ITcpListener.Start();
                WriteEntryLog("StartTcpSSLServer() Finished, SSL Tcp Listener Point : " + AIntPort.ToString() + "\nWaiting For A Socket Link From Client", EventLogEntryType.Information);
            }
            catch (Exception ex)
            {
                WriteEntryLog("StartTcpSSLServer() Failed\n" + ex.ToString(), EventLogEntryType.Error);
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
                   // BIsServiceStart = true;
                    //UMPService00.IEventLog.WriteEntry("BIsServiceStart = " + BIsServiceStart, EventLogEntryType.Warning);
                    AddConnectedClientAndThread(LClientOperations, LClientThread, ref LBoolReturn, ref LStrReturn);
                    if (LBoolReturn) { LClientThread.Start(); } else { WriteEntryLog(LStrReturn, EventLogEntryType.Error); }
                    Thread.Sleep(100);
                    IBoolIsBusing = false;
                }
            }
            catch (Exception ex)
            {
                IBoolCanContinue = false;
                WriteEntryLog("WaitForSocketClientConnect() Failed\n" + ex.ToString(), EventLogEntryType.Error);
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
                WriteEntryLog("CloseAllClientThread() Failed\n" + ex.ToString(), EventLogEntryType.Error);
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
                LStrCertificateFileFullName = System.IO.Path.Combine(IStrBaseDirectory, @"Components\Certificates\UMP.PF.Certificate.pfx");
                byte[] LByteReadedCertificate = System.IO.File.ReadAllBytes(LStrCertificateFileFullName);
                LStrInstallArea = "StoreName = My";
                LStrReturn = InstallCertificateToStore(StoreName.My, LByteReadedCertificate, "VoiceCyber,123");
                if (!string.IsNullOrEmpty(LStrReturn)) { return LStrReturn; }
            }
            catch (Exception ex)
            {
                LStrReturn = "InstallCertificateFile() " + LStrInstallArea + "\n" + ex.ToString();
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
                LStrReturn = "InstallCertificateToStore() " + AStoreName.ToString() + "\n" + ex.ToString();
            }

            return LStrReturn;
        }


        //写成文本日志
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
            WriteLog(mode, "UMPService00", msg);
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
                        string.Format("UMP\\{0}\\Logs", "UMPService00"));
                }
                else
                {
                    strReturn = Path.Combine(strReturn, "UMPService00");
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


        internal static void WriteLog(EventLogEntryType eventLogEntryType, string LStrEntryBody)
        {
            throw new NotImplementedException();
        }

        internal static void WriteLog(string p, EventLogEntryType eventLogEntryType)
        {
            throw new NotImplementedException();
        }
      
    }


}
