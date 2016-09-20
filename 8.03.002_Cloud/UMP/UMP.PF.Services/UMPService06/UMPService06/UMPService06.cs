using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using PFShareClassesS;
using System.IO;
using System.Xml;
using VoiceCyber.Common;

namespace UMPService06
{
    public partial class UMPService06 : ServiceBase
    {
        public static string IStrEventLogSource = "Service 06";
        public static string IStrApplicationName = "UMP";

        public static int IIntThisServicePort = 0;
        public static string IStrBaseDirectory = string.Empty;
        public static string IStrSiteBindingIPAddress = string.Empty;

      //  public static EventLog IEventLog = null;
        public static X509Certificate IX509CertificateServer = null;

        /// <summary>
        /// 服务是否可以继续工作
        /// </summary>
        public bool IBoolCanContinue = true;
        /// <summary>
        /// 服务是否正在处理过程中
        /// </summary>
        private bool IBoolIsBusing = false;

        public static TcpListener ITcpListener = null;

        private Thread ThreadCheckThreadIsAlive = null;
        private static readonly object ILockObjectCheckThread = new object();
        private static List<ClientOperations> GlobalListConnectedClient = new List<ClientOperations>();
        private static List<Thread> GlobalListClientThread = new List<Thread>();
        private int ICheckSessionInterval = 30;  //检查客户端间隔，单位s，检查过程中发现会话已经超时，则关闭连接
        private int ISessionTimeout = 60; //Session超时时间，单位s，超过此时间没有发送或接收消息，则关闭本次会话

        #region 数据库类型及数据库连接参数
        public static int IIntDBType = 0;
        public static string IStrDBConnectProfile = string.Empty;
        #endregion


        #region  日志操作线程
        private Thread IThreadLogOperation;
        #endregion

        public UMPService06(string []args)
        {
            InitializeComponent();
            if(args.Length>0)
            {
                Console.WriteLine(args[0]);
               if(args[0].Trim().ToLower()=="-start")
               {
                   OnStart(args);
               }
            }
        }
        #region
        protected override void OnStart(string[] args)
        {
            Thread ThreadStart = new Thread(StartService06);
            ThreadStart.Start();

            LogOperation logOperation = new LogOperation();
            if (IThreadLogOperation != null)
            {
                try
                {
                    IThreadLogOperation.Abort();
                }
                catch (System.Exception e)
                {
                }
            }
            Thread LThreadLogOperator = new Thread(new ThreadStart(logOperation.LogCompressionAndDelete));
            LThreadLogOperator.Start();
        }

        protected override void OnStop()
        {
            try
            {
                IBoolCanContinue = false;
                while (IBoolIsBusing) { Thread.Sleep(100); }
                if (ITcpListener != null) { ITcpListener.Stop(); ITcpListener = null; }
                CloseAllClientThread();

                FileLog.WriteInfo("Service Stopped At :", DateTime.Now.ToString("G"));
                if (ThreadCheckThreadIsAlive !=null)
                {
                    ThreadCheckThreadIsAlive.Abort();
                    ThreadCheckThreadIsAlive = null;
                }

                if (IThreadLogOperation != null)
                {
                    try
                    {
                        IThreadLogOperation.Abort();
                        IThreadLogOperation = null;
                    }
                    catch (System.Exception e)
                    {
                    }
                }
            }
            catch (Exception ex)
            {
                FileLog.WriteError("Service Stopped   Error At :", ex.ToString());
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
                FileLog.WriteError("CloseAllClientThread()", ex.ToString());
            }
        }

        ///// <summary>
        ///// 将日志写入 Windows 事情
        ///// </summary>
        ///// <param name="AStrWriteBody">日志内容</param>
        ///// <param name="AEntryType">日志类型</param>
        //private void WriteEntryLog(string AStrWriteBody, EventLogEntryType AEntryType)
        //{
        //    try
        //    {
        //        UMPServiceLog.WriteEntry(AStrWriteBody, AEntryType);
        //    }
        //    catch { }
        //}

        #endregion


        #region MyRegion
        private void StartService06()
        {
            FileLog.WriteInfo("StartService06()", "Started");
            IStrBaseDirectory = GetCurrentBaseDirectory();
            FileLog.WriteInfo("StartService06()", "IStrBaseDirectory=" + IStrBaseDirectory);

            if (string.IsNullOrEmpty(IStrBaseDirectory)) { return; }

            IIntThisServicePort = GetUMPPFBasicHttpPort();
            while (IIntThisServicePort == 0)
            {
                Thread.Sleep(5000);
                IIntThisServicePort = GetUMPPFBasicHttpPort();
            }

            //Service06端口号是UMP端口-6
            IIntThisServicePort -= 6;
            FileLog.WriteInfo("StartService06()", "IIntThisServicePort:" + IIntThisServicePort);

            while (IIntDBType == 0 || string.IsNullOrEmpty(IStrDBConnectProfile)) 
            {
                GetDatabaseConnectionProfile();
                System.Threading.Thread.Sleep(5000);
            }

            if (!CreateCheckThreadIsAlive()) { return; }

            StartTcpSSLServer(IIntThisServicePort);
            FileLog.WriteInfo("StartService06()", "StartTcpSSLServer");

            WaitForSocketClientConnect();
        }

        private void StartTcpSSLServer(int AIntPort)
        {
            string LStrInstallReturn = string.Empty;
            try
            {
                FileLog.WriteInfo("StartTcpSSLServer() Begion", "");

                X509Store LX509Store = new X509Store(StoreName.My, StoreLocation.LocalMachine);

                #region 使用安全证书方式 2015-07-06 修改
                List<string> lstDirs = Directory.GetDirectories(IStrBaseDirectory.Trim('\\')).ToList();
                FileLog.WriteInfo("StartTcpSSLServer()", "lstDirs.count = " + lstDirs.Count);
                string strDir = string.Empty;
                if (lstDirs.Contains(System.IO.Path.Combine(IStrBaseDirectory,@"WinServices")))
                {
                    //UMP.PF.Certificate.pfx //UMP.SSL.Certificate.pfx
                    strDir = System.IO.Path.Combine(IStrBaseDirectory, @"WinServices\UMP.SSL.Certificate.pfx");
                }
                else
                {
                    strDir = System.IO.Path.Combine(IStrBaseDirectory, @"UMP.SSL.Certificate.pfx");
                }
                
                FileLog.WriteInfo("StartTcpSSLServer()", "IX509CertificateServerPath = " + strDir);
                IX509CertificateServer = new X509Certificate2(strDir, "VoiceCyber,123");
                
                #endregion

                FileLog.WriteInfo("StartTcpSSLServer() ", " ITcpListener.Start();");
                ITcpListener = new TcpListener(IPAddress.Any, AIntPort);       
                ITcpListener.Start();
                FileLog.WriteInfo("StartTcpSSLServer() ", "Finished, SSL Tcp Listener Point : " + AIntPort.ToString() + " Waiting For A Socket Link From Client");

            }
            catch (Exception ex)
            {
                FileLog.WriteInfo("StartTcpSSLServer()Error", ex.ToString());
            }
        }

        private void WaitForSocketClientConnect()
        {
            bool LBoolReturn = true;
            string LStrReturn = string.Empty;
            FileLog.WriteInfo("WaitForSocketClientConnect()", "");
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
                    FileLog.WriteInfo("WaitForSocketClientConnect()", "1");
                    AddConnectedClientAndThread(LClientOperations, LClientThread, ref LBoolReturn, ref LStrReturn);
                    if (LBoolReturn) 
                    {
                        LClientThread.Start(); 
                    } 
                    else 
                    {
                        FileLog.WriteError("WaitForSocketClientConnect()", LStrReturn);
                    }
                    Thread.Sleep(100);

                    IBoolIsBusing = false;
                }
            }
            catch (Exception ex)
            {
                IBoolCanContinue = false;
                FileLog.WriteInfo("WaitForSocketClientConnect()", "Failed\n" + ex.ToString());
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
                    FileLog.WriteInfo("AddConnectedClientAndThread()", "AddConnectedClientAndThread() GlobalListClientThread.Count = " + GlobalListClientThread.Count.ToString() + "    GlobalListConnectedClient.Count = " + GlobalListConnectedClient.Count.ToString());
                }
            }
            catch (Exception ex)
            {
                ABoolReturn = false;
                AStrReturn = "AddConnectedClientAndThread()\n" + ex.Message;
                FileLog.WriteError("AddConnectedClientAndThread()", ex.Message.ToString());
            }
        }


        /// <summary>
        /// 创建检测 Thread 是否活着的进程
        /// </summary>
        /// <returns></returns>
        private bool CreateCheckThreadIsAlive()
        {
            bool LBoolReturn = true;

            try
            {
                FileLog.WriteInfo("CreateCheckThreadIsAlive()", "Begin");
                ThreadCheckThreadIsAlive = new System.Threading.Thread(new System.Threading.ThreadStart(CheckThreadIsAliveAction));
                ThreadCheckThreadIsAlive.Start();
                FileLog.WriteInfo("CreateCheckThreadIsAlive() "," Finished");
            }
            catch (Exception ex)
            {
                FileLog.WriteInfo("CreateCheckThreadIsAlive()","  Failed\n" + ex.ToString());
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
                    if (!LBoolReturn) 
                    {                      
                        FileLog.WriteInfo("CheckThreadIsAliveAction()","Failed\n RemoveIsNotAliveThread()" );
                    }

                    Thread.Sleep(3000);
                }
            }
            catch (Exception ex)
            {
                FileLog.WriteInfo("CheckThreadIsAliveAction()", "Failed\n" + ex.ToString());
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
                        FileLog.WriteInfo("RemoveIsNotAliveThread()", "RemoveIsNotAliveThread() GlobalListClientThread.Count = " + AIntThreadCount.ToString() + "  GlobalListConnectedClient.Count = " + AIntConnectedCount.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                ABoolReturn = false;
                AStrRetrun = "RemoveIsNotAliveThread()   Failed\n" + ex.Message;
            }
        }
        /// <summary>
        /// 获取UMP.PF站点梆定的http端口
        /// </summary>
        /// <returns></returns>
        private int GetUMPPFBasicHttpPort() 
        {
            int LIntReturn = 0;
            string LStrXmlFileName = string.Empty;

            try
            {
               // FileLog.WriteInfo("GetUMPPFBasicHttpPort() ", "Read UMP.PF Binding Port Information ...");

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
               // FileLog.WriteInfo("GetUMPPFBasicHttpPort() ", "Readed UMP.PF Binding Port : " + LIntReturn.ToString()  )    ;
            }
            catch (Exception ex)
            {
                LIntReturn = 0;
                FileLog.WriteInfo("GetUMPPFBasicHttpPort() ", "Read UMP.PF Binding Port Information Failed\n" + ex.ToString());
            }

            return LIntReturn;
        }

          /// <summary>
        /// 获取当前服务安装的路径
        /// </summary>
        /// <returns></returns>
        public string GetCurrentBaseDirectory()
        {
            string LStrReturn = string.Empty;
            try
            {
                LStrReturn = AppDomain.CurrentDomain.BaseDirectory;
                string[] LStrDerectoryArray = LStrReturn.Split('\\');
                LStrReturn = string.Empty;
                foreach (var LStrDirectorySingle in LStrDerectoryArray)
                {
                    if (LStrDirectorySingle.ToLower() =="winservices")
                    {
                        break;
                    }
                    LStrReturn += LStrDirectorySingle + "\\";
                    //FileLog.WriteInfo("GetCurrentBaseDirectory()", "Read AppDomain.CurrentDomain.BaseDirectory : " + LStrReturn);
                }
            }
            catch (Exception ex)
            {
                LStrReturn = string.Empty;
                FileLog.WriteError("GetCurrentBaseDirectory()", "Read AppDomain.CurrentDomain.BaseDirectory Failed \n" + ex.ToString());
            }
            return LStrReturn;
        }


        /// <summary>
        /// 获取数据路类型及数据库连接串
        /// </summary>
        /// <returns>string.Empty成功，Error01未进行数据库配置，否则失败</returns>
        /// 
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
                IIntDBType = 0;
                IStrDBConnectProfile = string.Empty;
                LStrVerificationCode = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104);

                LStrXmlFileName = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                //LStrXmlFileName =System.AppDomain.CurrentDomain.BaseDirectory;//测试用
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

                FileLog.WriteInfo("GetDatabaseConnectionProfile() " ,"LStrDBConnectProfile=" +LStrDBConnectProfile);
            }
            catch (Exception ex)
            {
                IIntDBType = 0;
                IStrDBConnectProfile = string.Empty;
                FileLog.WriteError("GetDatabaseConnectionProfile()" ,"Error :"+ ex.ToString());
            }
        }
        #endregion


        /// <summary>
        /// 创建加密Code
        /// </summary>
        /// <param name="AKeyIVID"></param>
        /// <returns></returns>
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

    }

}
