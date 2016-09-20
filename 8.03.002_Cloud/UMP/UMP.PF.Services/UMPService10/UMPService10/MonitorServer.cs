//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    eb518a1f-4962-442e-a9f7-b491de2c1ffc
//        CLR Version:              4.0.30319.18408
//        Name:                     MonitorServer
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPService10
//        File Name:                MonitorServer
//
//        created by Charley at 2016/6/27 15:44:02
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Xml;
using VoiceCyber.Common;
using VoiceCyber.DBAccesses;
using VoiceCyber.SDKs.DEC;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.CommonService10;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Encryptions;
using WebRequest = VoiceCyber.UMP.Communications.WebRequest;


namespace UMPService10
{
    public partial class MonitorServer : IEncryptable, INetServer
    {

        #region Members

        private const string CERT_PASSWORD = "VoiceCyber,123";
        private string mAppName = "UMPService10";
        private LogOperator mLogOperator;
        private ConfigInfo mConfigInfo;
        private SessionInfo mSession;

        private string mRootDir;
        private int mPort;
        private bool mCanDBConnected;

        private TcpListener mTcpListener;
        private X509Certificate2 mCertificate;
        private ConfigChangeOperator mConfigChangeOperator;
        private DECMessageHelper mDECMessageHelper;

        private NetPipeHelper mNetPipeHelper;
        private LocalMonitorHelper mLocalMonHelper;
        private DecConnector mDecConnector;

        private List<INetSession> mListMonitorSessions;
        private List<ResourceConfigInfo> mListResourceInfos;
        private List<ExtensionInfo> mListExtensionInfos;

        private Thread mThreadCheckSession;
        private Thread mThreadReReadParam;
        private Thread mThreadUpdateExtState;

        private int mSessionTimeout = 600;                  //Session超时时间，单位s，超过此时间没有发送或接收消息，则关闭本次会话
        private int mCheckSessionInterval = 30;             //检查客户端间隔，单位s，检查过程中发现会话已经超时，则关闭连接
        private int mReReadParamInterval = 30;              //重读参数（数据库，AppServer，ConfigInfo等）时间间隔，单位m
        private int mReConnectDBInterval = 10;              //当数据库连接失败，重连数据库时间间隔，单位s
        private int mUpdateExtStateInterval = 30;           //定时检查分机状态的时间间隔，单位m
        private int mQueryExtStateWaitNum = 10;             //每查询一个分机状态要等待的时间，单位ms

        #endregion


        #region ListSessions

        public IList<INetSession> ListSessions
        {
            get { return mListMonitorSessions; }
        }

        #endregion


        public MonitorServer()
        {
            mListMonitorSessions = new List<INetSession>();
            mListResourceInfos = new List<ResourceConfigInfo>();
            mListExtensionInfos = new List<ExtensionInfo>();

            mPort = 8081 - 10;
            mCanDBConnected = false;
        }

        public void Start()
        {
            try
            {
                if (Program.IsConsole)
                {
                    CreateFileLog();
                }
                OnDebug(LogMode.Info, string.Format("MonitorServer starting..."));
                Init();
                LoadResourceInfos();
                InitExtStateInfos();
                LoadServerCertificate();
                StartListener();
                CreateDECMessageHelper();
                CreateDECConnector();
                CreateConfigChangeOperator();

                //启动线程
                CreateCheckSessionThread();
                CreateReReadParamThread();
                CreateUpdateExtStateThread();

                OnDebug(LogMode.Info, string.Format("MonitorServer started"));
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("MonitorServer start fail.\t{0}", ex.Message));
            }
        }

        public void Stop()
        {
            try
            {
                //停止线程
                StopUpdateExtStateThread();
                StopCheckSessionThread();
                StopReReadParamThread();

                if (mConfigChangeOperator != null)
                {
                    mConfigChangeOperator.Stop();
                    mConfigChangeOperator = null;
                }
                if (mDecConnector != null)
                {
                    mDecConnector.Close();
                    mDecConnector = null;
                }

                for (int i = 0; i < mListMonitorSessions.Count; i++)
                {
                    mListMonitorSessions[i].Stop();
                }
                StopListener();
                OnDebug(LogMode.Info, string.Format("MonitorServer stopped"));
                if (mLogOperator != null)
                {
                    mLogOperator.Stop();
                    mLogOperator = null;
                }
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("MonitorServer stop fail.\t{0}", ex.Message));
            }
        }


        #region Init

        private void Init()
        {
            try
            {
                //加载配置选项
                LoadConfigInfo();
                ApplyConfigInfo();

                //获取主目录
                string path = AppDomain.CurrentDomain.BaseDirectory;
                path = path.Substring(0, path.LastIndexOf("\\"));
                path = path.Substring(0, path.LastIndexOf("\\"));
                OnDebug(LogMode.Info, string.Format("Root directory:{0}", path));
                mRootDir = path;
                mLocalMonHelper = new LocalMonitorHelper();

                mSession = new SessionInfo();
                mSession.AppName = mAppName;
                mSession.SessionID = Guid.NewGuid().ToString();
                InitAppServerInfo();
                InitDatabaseInfo();
                InitRentInfo();

                GetAppServerInfo();
                GetDatabaseInfo();

                mLocalMonHelper.IsRememberObject = mSession.IsMonitor;
                OnDebug(LogMode.Info,
                    string.Format("SessionInfo:{0}", mSession));
                TestDBConnection();

                CreateNetPipeService();
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("MonitorServer init fail.\t{0}", ex.Message));
            }
        }

        private void InitAppServerInfo()
        {
            AppServerInfo appServerInfo;

            //appServerInfo = new AppServerInfo();
            //appServerInfo.Protocol = "http";
            //appServerInfo.Address = "192.168.6.55";
            //appServerInfo.Port = 8081;
            //appServerInfo.SupportHttps = false;
            //appServerInfo.SupportNetTcp = false;

            appServerInfo = new AppServerInfo();
            appServerInfo.Protocol = "http";
            appServerInfo.Address = "192.168.9.118";
            appServerInfo.Port = 8081;
            appServerInfo.SupportHttps = false;
            appServerInfo.SupportNetTcp = false;
            if (mSession != null)
            {
                mSession.AppServerInfo = appServerInfo;
            }
        }

        private void InitDatabaseInfo()
        {
            DatabaseInfo dbInfo;

            dbInfo = new DatabaseInfo();
            dbInfo.TypeID = 3;
            dbInfo.Host = "192.168.9.238";
            dbInfo.Port = 1521;
            dbInfo.DBName = "orcl";
            dbInfo.LoginName = "ump1";
            dbInfo.Password = "ump1";

            //dbInfo = new DatabaseInfo();
            //dbInfo.TypeID = 2;
            //dbInfo.Host = "192.168.4.182";
            //dbInfo.Port = 1433;
            //dbInfo.DBName = "UMPDataDB0818";
            //dbInfo.LoginName = "PFDEV";
            //dbInfo.Password = "PF,123";
            if (mSession != null)
            {
                mSession.DatabaseInfo = dbInfo;
                mSession.DBConnectionString = dbInfo.GetConnectionString();
            }
        }

        private void InitRentInfo()
        {
            RentInfo rentInfo = new RentInfo();
            rentInfo.ID = ConstValue.RENT_DEFAULT;
            rentInfo.Name = ConstValue.VCT_COMPANY_SHORTNAME;
            rentInfo.Domain = ConstValue.VCT_COMPANY_SHORTNAME;
            rentInfo.Token = ConstValue.RENT_DEFAULT_TOKEN;

            if (mSession != null)
            {
                mSession.RentInfo = rentInfo;
                mSession.RentID = rentInfo.ID;
            }
        }

        #endregion


        #region 侦听连接

        private void StartListener()
        {
            try
            {
                mTcpListener = new TcpListener(IPAddress.Any, mPort);
                mTcpListener.Start();
                OnDebug(LogMode.Info, string.Format("MonitorLisener started.\t{0}", mTcpListener.LocalEndpoint));
                mTcpListener.BeginAcceptTcpClient(AcceptTcpClient, mTcpListener);
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("StartListener fail.\t{0}", ex.Message));
            }
        }

        private void StopListener()
        {
            try
            {
                if (mTcpListener != null)
                {
                    mTcpListener.Stop();
                }
                mTcpListener = null;
                OnDebug(LogMode.Info, string.Format("MonitorLisener stopped."));
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("StopListener fail.\t{0}", ex.Message));
            }
        }

        private void AcceptTcpClient(IAsyncResult result)
        {
            TcpListener listener = result.AsyncState as TcpListener;
            if (listener == null)
            {
                OnDebug(LogMode.Error, string.Format("Listener is null"));
                return;
            }
            TcpClient client = null;
            try
            {
                client = listener.EndAcceptTcpClient(result);
            }
            catch (Exception ex)
            {
                var dispose = ex as ObjectDisposedException;
                if (dispose == null)
                {
                    OnDebug(LogMode.Error, string.Format("EndAcceptTcpClient fail.\t{0}", ex));
                }
            }
            if (client != null)
            {
                try
                {
                    OnDebug(LogMode.Info, string.Format("New client connected.\t{0}", client.Client.RemoteEndPoint));
                    MonitorOperations session = new MonitorOperations(client);
                    session.Debug += OnDebug;
                    session.Name = mAppName;
                    session.EncryptObject = this;
                    session.ConfigInfo = mConfigInfo;
                    session.Certificate = mCertificate;
                    session.ListExtensionInfos = mListExtensionInfos;
                    session.ListResourceInfos = mListResourceInfos;
                    session.RootDir = mRootDir;
                    session.IsSSL = true;
                    session.Start();
                    mListMonitorSessions.Add(session);
                }
                catch (Exception ex)
                {
                    OnDebug(LogMode.Error, string.Format("AcceptTcpClient fail.\t{0}", ex.Message));
                }
            }
            try
            {
                listener.BeginAcceptTcpClient(AcceptTcpClient, listener);
            }
            catch (Exception ex)
            {
                var dispose = ex as ObjectDisposedException;
                if (dispose == null)
                {
                    OnDebug(LogMode.Error, string.Format("BeginAcceptTcpClient fail.\t{0}", ex.Message));
                }
            }
        }

        #endregion


        #region AppServerInfo

        private void GetAppServerInfo()
        {
            try
            {
                string path = Path.Combine(mRootDir, "GlobalSettings\\UMP.Server.01.xml");
                if (!File.Exists(path))
                {
                    OnDebug(LogMode.Error, string.Format("GlobalSetting UMP.Server.01.xml file not exist.\t{0}", path));
                    return;
                }
                XmlDocument doc = new XmlDocument();
                doc.Load(path);
                XmlNode node = doc.SelectSingleNode("UMPSetted/IISBindingProtocol");
                if (node == null)
                {
                    OnDebug(LogMode.Error, string.Format("IISBindingProtocol node not exist"));
                    return;
                }
                if (mSession == null || mSession.AppServerInfo == null) { return; }
                AppServerInfo appServerInfo = mSession.AppServerInfo;
                int intPort;
                XmlNodeList listNodes = node.ChildNodes;
                for (int i = 0; i < listNodes.Count; i++)
                {
                    XmlNode temp = listNodes[i];
                    if (temp.Attributes != null)
                    {
                        var protocol = temp.Attributes["Protocol"];
                        if (protocol != null && protocol.Value == "http")
                        {
                            appServerInfo.Protocol = "http";
                            var strPort = temp.Attributes["BindInfo"].Value;
                            if (int.TryParse(strPort, out intPort) && intPort > 0)
                            {
                                appServerInfo.Port = intPort;
                                mPort = intPort - 10;
                                OnDebug(LogMode.Info, string.Format("ServerPort is {0}", mPort));
                            }
                            var strAddress = temp.Attributes["IPAddress"].Value;
                            if (!string.IsNullOrEmpty(strAddress))
                            {
                                appServerInfo.Address = strAddress;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("GetAppServerInfo fail.\t{0}", ex.Message));
            }
        }

        #endregion


        #region DatabaseInfo

        private void GetDatabaseInfo()
        {
            try
            {
                string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "UMP.Server\\Args01.UMP.xml");
                if (!File.Exists(path))
                {
                    OnDebug(LogMode.Error, string.Format("UMP.Server\\Args01.UMP.xml file not exist.\t{0}", path));
                    return;
                }
                XmlDocument doc = new XmlDocument();
                doc.Load(path);
                XmlNode node = doc.SelectSingleNode("DatabaseParameters");
                if (node == null)
                {
                    OnDebug(LogMode.Error, string.Format("DatabaseParameters node not exist"));
                    return;
                }
                if (mSession == null || mSession.DatabaseInfo == null) { return; }
                DatabaseInfo dbInfo = mSession.DatabaseInfo;
                string strValue;
                int intValue;
                XmlNodeList listNodes = node.ChildNodes;
                for (int i = 0; i < listNodes.Count; i++)
                {
                    XmlNode temp = listNodes[i];
                    if (temp.Attributes != null)
                    {
                        var isEnableAttr = temp.Attributes["P03"];
                        if (isEnableAttr != null)
                        {
                            strValue = isEnableAttr.Value;
                            strValue = DecryptString(strValue);
                            if (strValue != "1") { continue; }
                        }
                        var attr = temp.Attributes["P02"];
                        if (attr != null)
                        {
                            strValue = attr.Value;
                            strValue = DecryptString(strValue);
                            if (int.TryParse(strValue, out intValue))
                            {
                                dbInfo.TypeID = intValue;
                            }
                        }
                        attr = temp.Attributes["P04"];
                        if (attr != null)
                        {
                            strValue = attr.Value;
                            strValue = DecryptString(strValue);
                            dbInfo.Host = strValue;
                        }
                        attr = temp.Attributes["P05"];
                        if (attr != null)
                        {
                            strValue = attr.Value;
                            strValue = DecryptString(strValue);
                            if (int.TryParse(strValue, out intValue))
                            {
                                dbInfo.Port = intValue;
                            }
                        }
                        attr = temp.Attributes["P06"];
                        if (attr != null)
                        {
                            strValue = attr.Value;
                            strValue = DecryptString(strValue);
                            dbInfo.DBName = strValue;
                        }
                        attr = temp.Attributes["P07"];
                        if (attr != null)
                        {
                            strValue = attr.Value;
                            strValue = DecryptString(strValue);
                            dbInfo.LoginName = strValue;
                        }
                        attr = temp.Attributes["P08"];
                        if (attr != null)
                        {
                            strValue = attr.Value;
                            strValue = DecryptString(strValue);
                            dbInfo.Password = strValue;
                        }
                    }
                }
                mSession.DBConnectionString = dbInfo.GetConnectionString();
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("GetDatabaseInfo fail.\t{0}", ex.Message));
            }
        }

        #endregion


        #region 服务器证书

        private void LoadServerCertificate()
        {
            try
            {
                string path;
                //if (Program.IsDebug)
                //{
                //    path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UMP.SSL.Certificate.pfx");
                //}
                //else
                //{
                //    path = Path.Combine(mRootDir, "Components\\Certificates\\UMP.SSL.Certificate.pfx");

                //}
                path = Path.Combine(mRootDir, "Components\\Certificates\\UMP.SSL.Certificate.pfx");
                if (!File.Exists(path))
                {
                    OnDebug(LogMode.Error, string.Format("Server certificate file not exist.\t{0}", path));
                    return;
                }
                X509Certificate2 cert = new X509Certificate2(path, CERT_PASSWORD);
                mCertificate = cert;
                OnDebug(LogMode.Info, string.Format("Load ServerCertificate end.\t{0}", cert.Thumbprint));
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("LoadServerCertificate fail.\t{0}", ex.Message));
            }
        }

        #endregion


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
                    OnDebug(LogMode.Error, string.Format("ConfigInfo file not exist.\t{0}", path));
                    return;
                }
                OperationReturn optReturn = XMLHelper.DeserializeFile<ConfigInfo>(path);
                if (!optReturn.Result)
                {
                    OnDebug(LogMode.Error,
                        string.Format("LoadConfigInfo fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                ConfigInfo configInfo = optReturn.Data as ConfigInfo;
                if (configInfo == null)
                {
                    OnDebug(LogMode.Error, string.Format("LoadConfigInfo fail.\tConfigInfo is null"));
                    return;
                }
                mConfigInfo = configInfo;
                OnDebug(LogMode.Info, string.Format("LoadConfigInfo end.\t{0}", path));
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("LoadConfigInfo fail.\t{0}", ex.Message));
            }
        }

        private void ApplyConfigInfo()
        {
            try
            {
                if (mConfigInfo == null
                    || mConfigInfo.ListSettings == null) { return; }

                SetLogMode();

                string strValue;
                int intValue;
                string strLog = string.Empty;

                var setting =
                    mConfigInfo.ListSettings.FirstOrDefault(s => s.Key == ConstValue.GS_KEY_TIMEOUT_SESSION);
                if (setting != null)
                {
                    strValue = setting.Value;
                    if (int.TryParse(strValue, out intValue)
                        && intValue >= 5
                        && intValue <= 60 * 60 * 24)
                    {
                        mSessionTimeout = intValue;
                        strLog += string.Format("{0}:{1};", ConstValue.GS_KEY_TIMEOUT_SESSION, intValue);
                    }
                }
                setting =
                    mConfigInfo.ListSettings.FirstOrDefault(s => s.Key == ConstValue.GS_KEY_INTERVAL_REREADPARAM);
                if (setting != null)
                {
                    strValue = setting.Value;
                    if (int.TryParse(strValue, out intValue)
                        && intValue >= 1
                        && intValue <= 60 * 24)
                    {
                        mReReadParamInterval = intValue;
                        strLog += string.Format("{0}:{1};", ConstValue.GS_KEY_INTERVAL_REREADPARAM, intValue);
                    }
                }
                setting =
                    mConfigInfo.ListSettings.FirstOrDefault(s => s.Key == ConstValue.GS_KEY_INTERVAL_RECONNECTDB);
                if (setting != null)
                {
                    strValue = setting.Value;
                    if (int.TryParse(strValue, out intValue)
                        && intValue >= 1
                        && intValue <= 60 * 60)
                    {
                        mReConnectDBInterval = intValue;
                        strLog += string.Format("{0}:{1};", ConstValue.GS_KEY_INTERVAL_RECONNECTDB, intValue);
                    }
                }
                setting =
                    mConfigInfo.ListSettings.FirstOrDefault(s => s.Key == ConstValue.GS_KEY_INTERVAL_CHECKSESSION);
                if (setting != null)
                {
                    strValue = setting.Value;
                    if (int.TryParse(strValue, out intValue)
                        && intValue >= 5
                        && intValue <= 60 * 60)
                    {
                        mCheckSessionInterval = intValue;
                        strLog += string.Format("{0}:{1};", ConstValue.GS_KEY_INTERVAL_CHECKSESSION, intValue);
                    }
                }
                setting =
                  mConfigInfo.ListSettings.FirstOrDefault(s => s.Key == Service10Consts.GS_KEY_S10_INTERVAL_UPDATEEXTSTATE);
                if (setting != null)
                {
                    strValue = setting.Value;

                    if (int.TryParse(strValue, out intValue)
                        && intValue >= 1
                        && intValue <= 60 * 24)
                    {
                        mUpdateExtStateInterval = intValue;
                        strLog += string.Format("{0}:{1};", Service10Consts.GS_KEY_S10_INTERVAL_UPDATEEXTSTATE, intValue);
                    }
                }
                setting =
                    mConfigInfo.ListSettings.FirstOrDefault(
                        s => s.Key == Service10Consts.GS_KEY_S10_QUERYEXTSTATE_WAITNUM);
                if (setting != null)
                {
                    strValue = setting.Value;

                    if (int.TryParse(strValue, out intValue)
                        && intValue >= 1
                        && intValue <= 60 * 1000)
                    {
                        mQueryExtStateWaitNum = intValue;
                        strLog += string.Format("{0}:{1};", Service10Consts.GS_KEY_S10_QUERYEXTSTATE_WAITNUM, intValue);
                    }
                }
                if (!string.IsNullOrEmpty(strLog))
                {
                    OnDebug(LogMode.Info, string.Format("ApplyConfigInfo:\t{0}", strLog));
                }
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("ApplyConfigInfo fail.\t{0}", ex.Message));
            }
        }

        #endregion


        #region 重读参数

        private void CreateReReadParamThread()
        {
            try
            {
                if (mThreadReReadParam != null)
                {
                    try
                    {
                        mThreadReReadParam.Abort();
                    }
                    catch { }
                    mThreadReReadParam = null;
                }
                mThreadReReadParam = new Thread(ReReadParamWorker);
                mThreadReReadParam.Start();
                OnDebug(LogMode.Info,
                    string.Format("ReReadParamThread started.\t{0}", mThreadReReadParam.ManagedThreadId));
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("CreateReReadParamThread fail.\t{0}", ex.Message));
            }
        }

        private void StopReReadParamThread()
        {
            try
            {
                if (mThreadReReadParam != null)
                {
                    try
                    {
                        mThreadReReadParam.Abort();
                    }
                    catch { }
                    mThreadReReadParam = null;
                    OnDebug(LogMode.Info, string.Format("ReReadParamThread stopped"));
                }
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("StopReReadParamThread fail.\t{0}", ex.Message));
            }
        }

        private void ReReadParamWorker()
        {
            try
            {
                while (true)
                {
                    if (!mCanDBConnected)
                    {
                        Thread.Sleep(mReConnectDBInterval * 1000); //如果数据库没有连接成功，等待10s重连

                        GetDatabaseInfo();
                    }
                    else
                    {
                        Thread.Sleep(mReReadParamInterval * 1000 * 60); //等待30分钟后重读参数

                        //以下重读参数
                        OnDebug(LogMode.Info, string.Format("Begin reread param"));

                        //加载配置选项
                        LoadConfigInfo();
                        ApplyConfigInfo();

                        //重读AppServerInfo和DatabaseInfo
                        GetAppServerInfo();
                        GetDatabaseInfo();
                        OnDebug(LogMode.Info,
                            string.Format("SessionInfo:{0}", mSession));
                    }

                    TestDBConnection();
                    if (mCanDBConnected)
                    {
                        LoadResourceInfos();
                        InitExtStateInfos();
                    }
                }
            }
            catch (Exception ex)
            {
                var abort = ex as ThreadAbortException;
                if (abort == null)
                {
                    OnDebug(LogMode.Error, string.Format("ReReadParamWorker fail.\t{0}", ex));
                }
            }
        }

        private void TestDBConnection()
        {
            try
            {
                if (mSession == null
                 || mSession.DatabaseInfo == null)
                {
                    OnDebug(LogMode.Error, string.Format("SessionInfo or DatabaseInfo is null"));
                    return;
                }
                DatabaseInfo dbInfo = mSession.DatabaseInfo;
                string strConn = dbInfo.GetConnectionString();
                int dbType = dbInfo.TypeID;
                OperationReturn optReturn;
                switch (dbType)
                {
                    case 2:
                        optReturn = MssqlOperation.TestDBConnection(strConn);
                        break;
                    case 3:
                        optReturn = OracleOperation.TestDBConnection(strConn);
                        break;
                    default:
                        OnDebug(LogMode.Error, string.Format("Database type invalid.\t{0}", dbType));
                        return;
                }
                if (!optReturn.Result)
                {
                    mCanDBConnected = false;
                    OnDebug(LogMode.Error,
                        string.Format("TestDBConnection fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                mCanDBConnected = true;
                OnDebug(LogMode.Info,
                    string.Format("TestDBConnection Database:{0};ConnectionTimeout:{1}", optReturn.Message,
                        optReturn.Data));
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("CheckDBConnection fail.\t{0}", ex.Message));
            }
        }

        #endregion


        #region 检查会话

        private void CreateCheckSessionThread()
        {
            try
            {
                mThreadCheckSession = new Thread(CheckSessionWorker);
                mThreadCheckSession.Start();

                OnDebug(LogMode.Info,
                    string.Format("CheckSession thread started.\t{0}", mThreadCheckSession.ManagedThreadId));
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("CreateCheckSessionThread fail.\t{0}", ex.Message));
            }
        }

        private void StopCheckSessionThread()
        {
            try
            {
                if (mThreadCheckSession != null)
                {
                    mThreadCheckSession.Abort();
                    mThreadCheckSession = null;
                }

                OnDebug(LogMode.Info, string.Format("CheckSession thread stopped"));
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("StopCheckSessionThread fail.\t{0}", ex.Message));
            }
        }

        private void CheckSessionWorker()
        {
            try
            {
                while (true)
                {
                    string strActive = string.Empty;
                    string strNotActive = string.Empty;
                    for (int i = mListMonitorSessions.Count - 1; i >= 0; i--)
                    {
                        INetSession session = mListMonitorSessions[i];
                        DateTime lastTime = session.LastActiveTime;
                        DateTime now = DateTime.Now;
                        //Session超时
                        if ((now - lastTime).TotalSeconds > mSessionTimeout)
                        {
                            OnDebug(LogMode.Info, string.Format("Session timeout.\t{0}", session.RemoteEndpoint));
                            session.Stop();
                            mListMonitorSessions.Remove(session);
                        }
                        else
                        {
                            if (session.IsConnected)
                            {
                                strActive += string.Format("{0};", session.RemoteEndpoint);
                            }
                            else
                            {
                                strNotActive += string.Format("{0}", session.RemoteEndpoint);
                            }
                        }
                    }
                    OnDebug(LogMode.Info, string.Format("Check clients:{0}", mListMonitorSessions.Count));
                    if (!string.IsNullOrEmpty(strActive))
                    {
                        OnDebug(LogMode.Info, string.Format("Actives:{0}", strActive));
                    }
                    if (!string.IsNullOrEmpty(strNotActive))
                    {
                        OnDebug(LogMode.Info, string.Format("NotActive:{0}", strNotActive));
                    }
                    Thread.Sleep(mCheckSessionInterval * 1000);
                }
            }
            catch (Exception ex)
            {
                var abort = ex as ThreadAbortException;
                if (abort == null)
                {
                    OnDebug(LogMode.Error, string.Format("CheckSessionWorker fail.\t{0}", ex.Message));
                }
            }
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
            OnDebug(mode, "MonitorServer", msg);
        }

        #endregion


        #region 资源配置信息

        private void LoadResourceInfos()
        {
            try
            {
                if (mSession == null
                    || mSession.AppServerInfo == null
                    || mSession.DatabaseInfo == null)
                {
                    OnDebug(LogMode.Error, string.Format("AppServerInfo or DatabaseInfo is null"));
                    return;
                }
                DatabaseInfo dbInfo = mSession.DatabaseInfo;
                string strConn = mSession.DBConnectionString;

                OperationReturn optReturn;
                string rentToken = "00000";
                string strSql;
                DataSet objDataSet;

                List<ResourceConfigInfo> listConfigInfos = new List<ResourceConfigInfo>();
                List<ExtensionInfo> listExtensionInfos = new List<ExtensionInfo>();
                List<ExtensionInfo> listRealExtInfos = new List<ExtensionInfo>();
                int decCount = 0;
                int voiceCount = 0;
                int screenCount = 0;
                int voiceChanCount = 0;
                int screenChanCount = 0;


                #region 获取DECServer资源

                switch (dbInfo.TypeID)
                {
                    case 2:
                        strSql =
                            string.Format(
                                "SELECT * FROM T_11_101_{0} WHERE C001 > {1} AND C001 < {2} AND C002 = 1 ORDER BY C001, C002",
                                rentToken,
                                DECServerInfo.RESOURCE_DATAEXCHANGECENTER * 10000000000000000,
                                (DECServerInfo.RESOURCE_DATAEXCHANGECENTER + 1) * 10000000000000000);
                        optReturn = MssqlOperation.GetDataSet(strConn, strSql);
                        if (!optReturn.Result)
                        {
                            OnDebug(LogMode.Error,
                                string.Format("LoadConfigInfo fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                            return;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        strSql =
                             string.Format(
                                 "SELECT * FROM T_11_101_{0} WHERE C001 > {1} AND C001 < {2} AND C002 = 1 ORDER BY C001, C002",
                                 rentToken,
                                 DECServerInfo.RESOURCE_DATAEXCHANGECENTER * 10000000000000000,
                                 (DECServerInfo.RESOURCE_DATAEXCHANGECENTER + 1) * 10000000000000000);
                        optReturn = OracleOperation.GetDataSet(strConn, strSql);
                        if (!optReturn.Result)
                        {
                            OnDebug(LogMode.Error,
                                string.Format("LoadConfigInfo fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                            return;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    default:
                        OnDebug(LogMode.Error, string.Format("Database type not support.\t{0}", dbInfo.TypeID));
                        return;
                }
                if (objDataSet == null)
                {
                    OnDebug(LogMode.Error, string.Format("ObjDataSet is null"));
                    return;
                }
                for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = objDataSet.Tables[0].Rows[i];

                    DECServerInfo dec = new DECServerInfo();
                    dec.ObjType = DECServerInfo.RESOURCE_DATAEXCHANGECENTER;
                    long objID = Convert.ToInt64(dr["C001"]);
                    dec.ObjID = objID;

                    var temp = listConfigInfos.FirstOrDefault(r => r.ObjID == dec.ObjID);
                    if (temp != null)
                    {
                        listConfigInfos.Remove(temp);
                    }
                    decCount++;
                    listConfigInfos.Add(dec);
                }

                #endregion


                #region 获取DECServer 的配置信息

                for (int i = 0; i < listConfigInfos.Count; i++)
                {
                    var resource = listConfigInfos[i];
                    if (resource.ObjType != DECServerInfo.RESOURCE_DATAEXCHANGECENTER) { continue; }
                    var dec = resource as DECServerInfo;
                    if (dec == null) { continue; }
                    switch (dbInfo.TypeID)
                    {
                        case 2:
                            strSql =
                                string.Format(
                                    "SELECT * FROM T_11_101_{0} WHERE C001 ={1} and (C002 = 1 OR C002 = 2 OR C002 = 3 OR C002 = 91) ORDER BY C001, C002",
                                    rentToken,
                                    dec.ObjID);
                            optReturn = MssqlOperation.GetDataSet(strConn, strSql);
                            if (!optReturn.Result)
                            {
                                OnDebug(LogMode.Error,
                                    string.Format("LoadConfigInfo fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                                return;
                            }
                            objDataSet = optReturn.Data as DataSet;
                            break;
                        case 3:
                            strSql =
                                string.Format(
                                    "SELECT * FROM T_11_101_{0} WHERE C001 ={1} and (C002 = 1 OR C002 = 2 OR C002 = 3 OR C002 = 91) ORDER BY C001, C002",
                                    rentToken,
                                    dec.ObjID);
                            optReturn = OracleOperation.GetDataSet(strConn, strSql);
                            if (!optReturn.Result)
                            {
                                OnDebug(LogMode.Error,
                                    string.Format("LoadConfigInfo fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                                return;
                            }
                            objDataSet = optReturn.Data as DataSet;
                            break;
                        default:
                            OnDebug(LogMode.Error, string.Format("Database type not support.\t{0}", dbInfo.TypeID));
                            return;
                    }
                    if (objDataSet == null)
                    {
                        OnDebug(LogMode.Error, string.Format("ObjDataSet is null"));
                        return;
                    }
                    for (int j = 0; j < objDataSet.Tables[0].Rows.Count; j++)
                    {
                        DataRow dr = objDataSet.Tables[0].Rows[j];

                        int row = Convert.ToInt32(dr["C002"]);
                        if (row == 1)
                        {
                            int key = Convert.ToInt32(dr["C011"]);
                            dec.Key = key;
                            int id = Convert.ToInt32(dr["C012"]);
                            dec.ID = id;
                            string strHostAddress = dr["C017"].ToString();
                            strHostAddress = DecodeEncryptValue(strHostAddress);
                            dec.HostAddress = strHostAddress;
                            string strHostName = dr["C018"].ToString();
                            strHostName = DecodeEncryptValue(strHostName);
                            dec.HostName = strHostName;
                            string strPort = dr["C019"].ToString();
                            strPort = DecodeEncryptValue(strPort);
                            int intValue;
                            if (int.TryParse(strPort, out intValue))
                            {
                            }
                            dec.HostPort = intValue;
                        }
                        if (row == 91)
                        {
                            dec.Continent = dr["C011"].ToString();
                            dec.Country = dr["C012"].ToString();
                        }
                    }
                }

                #endregion


                #region 获取分机资源

                switch (dbInfo.TypeID)
                {
                    case 2:
                        strSql =
                            string.Format(
                                "SELECT * FROM T_11_101_{0} WHERE C001 > {1} AND C001 < {2} AND C002 = 1 ORDER BY C001, C002",
                                rentToken,
                                ExtensionInfo.RESOURCE_EXTENSION * 10000000000000000,
                                (ExtensionInfo.RESOURCE_EXTENSION + 1) * 10000000000000000);
                        optReturn = MssqlOperation.GetDataSet(strConn, strSql);
                        if (!optReturn.Result)
                        {
                            OnDebug(LogMode.Error,
                                string.Format("LoadConfigInfo fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                            return;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        strSql =
                             string.Format(
                                 "SELECT * FROM T_11_101_{0} WHERE C001 > {1} AND C001 < {2} AND C002 = 1 ORDER BY C001, C002",
                                 rentToken,
                                 ExtensionInfo.RESOURCE_EXTENSION * 10000000000000000,
                                 (ExtensionInfo.RESOURCE_EXTENSION + 1) * 10000000000000000);
                        optReturn = OracleOperation.GetDataSet(strConn, strSql);
                        if (!optReturn.Result)
                        {
                            OnDebug(LogMode.Error,
                                string.Format("LoadConfigInfo fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                            return;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    default:
                        OnDebug(LogMode.Error, string.Format("Database type not support.\t{0}", dbInfo.TypeID));
                        return;
                }
                if (objDataSet == null)
                {
                    OnDebug(LogMode.Error, string.Format("ObjDataSet is null"));
                    return;
                }
                for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = objDataSet.Tables[0].Rows[i];

                    ExtensionInfo dec = new ExtensionInfo();
                    dec.ObjType = ExtensionInfo.RESOURCE_EXTENSION;
                    long objID = Convert.ToInt64(dr["C001"]);
                    dec.ObjID = objID;

                    var temp = listExtensionInfos.FirstOrDefault(r => r.ObjID == dec.ObjID);
                    if (temp != null)
                    {
                        listExtensionInfos.Remove(temp);
                    }
                    listExtensionInfos.Add(dec);
                }

                #endregion


                #region 获取分机配置信息

                for (int i = 0; i < listExtensionInfos.Count; i++)
                {
                    var ext = listExtensionInfos[i];
                    if (ext.ObjType != ExtensionInfo.RESOURCE_EXTENSION) { continue; }
                    switch (dbInfo.TypeID)
                    {
                        case 2:
                            strSql =
                                string.Format(
                                    "SELECT * FROM T_11_101_{0} WHERE C001 ={1} and (C002 = 1 OR C002 = 2) ORDER BY C001, C002",
                                    rentToken,
                                    ext.ObjID);
                            optReturn = MssqlOperation.GetDataSet(strConn, strSql);
                            if (!optReturn.Result)
                            {
                                OnDebug(LogMode.Error,
                                    string.Format("LoadConfigInfo fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                                return;
                            }
                            objDataSet = optReturn.Data as DataSet;
                            break;
                        case 3:
                            strSql =
                                string.Format(
                                    "SELECT * FROM T_11_101_{0} WHERE C001 ={1} and (C002 = 1 OR C002 = 2) ORDER BY C001, C002",
                                    rentToken,
                                    ext.ObjID);
                            optReturn = OracleOperation.GetDataSet(strConn, strSql);
                            if (!optReturn.Result)
                            {
                                OnDebug(LogMode.Error,
                                    string.Format("LoadConfigInfo fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                                return;
                            }
                            objDataSet = optReturn.Data as DataSet;
                            break;
                        default:
                            OnDebug(LogMode.Error, string.Format("Database type not support.\t{0}", dbInfo.TypeID));
                            return;
                    }
                    if (objDataSet == null)
                    {
                        OnDebug(LogMode.Error, string.Format("ObjDataSet is null"));
                        return;
                    }
                    for (int j = 0; j < objDataSet.Tables[0].Rows.Count; j++)
                    {
                        DataRow dr = objDataSet.Tables[0].Rows[j];

                        int row = Convert.ToInt32(dr["C002"]);
                        if (row == 1)
                        {
                            ext.ParentObjID = Convert.ToInt64(dr["C011"]);
                            string strExtIP = DecryptFromDB(dr["C017"].ToString());
                            string[] arrExtIP = strExtIP.Split(new[] { ConstValue.SPLITER_CHAR }, StringSplitOptions.None);
                            string strExt = strExtIP;
                            if (arrExtIP.Length > 0)
                            {
                                strExt = arrExtIP[0];
                            }
                            ext.Extension = strExt;
                        }
                    }
                }

                #endregion


                #region 获取真实分机资源

                switch (dbInfo.TypeID)
                {
                    case 2:
                        strSql =
                            string.Format(
                                "SELECT * FROM T_11_101_{0} WHERE C001 > {1} AND C001 < {2} AND C002 = 1 ORDER BY C001, C002",
                                rentToken,
                                ExtensionInfo.RESOURCE_REALEXT * 10000000000000000,
                                (ExtensionInfo.RESOURCE_REALEXT + 1) * 10000000000000000);
                        optReturn = MssqlOperation.GetDataSet(strConn, strSql);
                        if (!optReturn.Result)
                        {
                            OnDebug(LogMode.Error,
                                string.Format("LoadConfigInfo fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                            return;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        strSql =
                             string.Format(
                                 "SELECT * FROM T_11_101_{0} WHERE C001 > {1} AND C001 < {2} AND C002 = 1 ORDER BY C001, C002",
                                 rentToken,
                                 ExtensionInfo.RESOURCE_REALEXT * 10000000000000000,
                                 (ExtensionInfo.RESOURCE_REALEXT + 1) * 10000000000000000);
                        optReturn = OracleOperation.GetDataSet(strConn, strSql);
                        if (!optReturn.Result)
                        {
                            OnDebug(LogMode.Error,
                                string.Format("LoadConfigInfo fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                            return;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    default:
                        OnDebug(LogMode.Error, string.Format("Database type not support.\t{0}", dbInfo.TypeID));
                        return;
                }
                if (objDataSet == null)
                {
                    OnDebug(LogMode.Error, string.Format("ObjDataSet is null"));
                    return;
                }
                for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = objDataSet.Tables[0].Rows[i];

                    ExtensionInfo dec = new ExtensionInfo();
                    dec.ObjType = ExtensionInfo.RESOURCE_REALEXT;
                    long objID = Convert.ToInt64(dr["C001"]);
                    dec.ObjID = objID;

                    var temp = listRealExtInfos.FirstOrDefault(r => r.ObjID == dec.ObjID);
                    if (temp != null)
                    {
                        listRealExtInfos.Remove(temp);
                    }
                    listRealExtInfos.Add(dec);
                }

                #endregion


                #region 获取真实分机配置信息

                for (int i = 0; i < listRealExtInfos.Count; i++)
                {
                    var ext = listRealExtInfos[i];
                    if (ext.ObjType != ExtensionInfo.RESOURCE_REALEXT) { continue; }
                    switch (dbInfo.TypeID)
                    {
                        case 2:
                            strSql =
                                string.Format(
                                    "SELECT * FROM T_11_101_{0} WHERE C001 ={1} and (C002 = 1 OR C002 = 2) ORDER BY C001, C002",
                                    rentToken,
                                    ext.ObjID);
                            optReturn = MssqlOperation.GetDataSet(strConn, strSql);
                            if (!optReturn.Result)
                            {
                                OnDebug(LogMode.Error,
                                    string.Format("LoadConfigInfo fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                                return;
                            }
                            objDataSet = optReturn.Data as DataSet;
                            break;
                        case 3:
                            strSql =
                                string.Format(
                                    "SELECT * FROM T_11_101_{0} WHERE C001 ={1} and (C002 = 1 OR C002 = 2) ORDER BY C001, C002",
                                    rentToken,
                                    ext.ObjID);
                            optReturn = OracleOperation.GetDataSet(strConn, strSql);
                            if (!optReturn.Result)
                            {
                                OnDebug(LogMode.Error,
                                    string.Format("LoadConfigInfo fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                                return;
                            }
                            objDataSet = optReturn.Data as DataSet;
                            break;
                        default:
                            OnDebug(LogMode.Error, string.Format("Database type not support.\t{0}", dbInfo.TypeID));
                            return;
                    }
                    if (objDataSet == null)
                    {
                        OnDebug(LogMode.Error, string.Format("ObjDataSet is null"));
                        return;
                    }
                    for (int j = 0; j < objDataSet.Tables[0].Rows.Count; j++)
                    {
                        DataRow dr = objDataSet.Tables[0].Rows[j];

                        int row = Convert.ToInt32(dr["C002"]);
                        if (row == 1)
                        {
                            ext.ParentObjID = Convert.ToInt64(dr["C011"]);
                            string strExtIP = DecryptFromDB(dr["C017"].ToString());
                            string[] arrExtIP = strExtIP.Split(new[] { ConstValue.SPLITER_CHAR }, StringSplitOptions.None);
                            string strExt = strExtIP;
                            if (arrExtIP.Length > 0)
                            {
                                strExt = arrExtIP[0];
                            }
                            ext.Extension = strExt;
                        }
                    }
                }

                #endregion


                #region 获取VoiceServer资源

                switch (dbInfo.TypeID)
                {
                    case 2:
                        strSql =
                            string.Format(
                                "SELECT * FROM T_11_101_{0} WHERE C001 > {1} AND C001 < {2} AND C002 = 1 ORDER BY C001, C002",
                                rentToken,
                                VoiceServerInfo.RESOURCE_VOICESERVER * 10000000000000000,
                                (VoiceServerInfo.RESOURCE_VOICESERVER + 1) * 10000000000000000);
                        optReturn = MssqlOperation.GetDataSet(strConn, strSql);
                        if (!optReturn.Result)
                        {
                            OnDebug(LogMode.Error,
                                string.Format("LoadConfigInfo fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                            return;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        strSql =
                             string.Format(
                                 "SELECT * FROM T_11_101_{0} WHERE C001 > {1} AND C001 < {2} AND C002 = 1 ORDER BY C001, C002",
                                 rentToken,
                                 VoiceServerInfo.RESOURCE_VOICESERVER * 10000000000000000,
                                 (VoiceServerInfo.RESOURCE_VOICESERVER + 1) * 10000000000000000);
                        optReturn = OracleOperation.GetDataSet(strConn, strSql);
                        if (!optReturn.Result)
                        {
                            OnDebug(LogMode.Error,
                                string.Format("LoadConfigInfo fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                            return;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    default:
                        OnDebug(LogMode.Error, string.Format("Database type not support.\t{0}", dbInfo.TypeID));
                        return;
                }
                if (objDataSet == null)
                {
                    OnDebug(LogMode.Error, string.Format("ObjDataSet is null"));
                    return;
                }
                for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = objDataSet.Tables[0].Rows[i];

                    VoiceServerInfo voice = new VoiceServerInfo();
                    voice.ObjType = VoiceServerInfo.RESOURCE_VOICESERVER;
                    long objID = Convert.ToInt64(dr["C001"]);
                    voice.ObjID = objID;

                    var temp = listConfigInfos.FirstOrDefault(r => r.ObjID == voice.ObjID);
                    if (temp != null)
                    {
                        listConfigInfos.Remove(temp);
                    }
                    voiceCount++;
                    listConfigInfos.Add(voice);
                }

                #endregion


                #region 获取VoiceServer配置信息

                for (int i = 0; i < listConfigInfos.Count; i++)
                {
                    var resource = listConfigInfos[i];
                    if (resource.ObjType != VoiceServerInfo.RESOURCE_VOICESERVER) { continue; }
                    var voice = resource as VoiceServerInfo;
                    if (voice == null) { continue; }
                    switch (dbInfo.TypeID)
                    {
                        case 2:
                            strSql =
                                string.Format(
                                    "SELECT * FROM T_11_101_{0} WHERE C001 ={1} and (C002 = 1 OR C002 = 2 OR C002 = 3 OR C002 = 91) ORDER BY C001, C002",
                                    rentToken,
                                    voice.ObjID);
                            optReturn = MssqlOperation.GetDataSet(strConn, strSql);
                            if (!optReturn.Result)
                            {
                                OnDebug(LogMode.Error,
                                    string.Format("LoadConfigInfo fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                                return;
                            }
                            objDataSet = optReturn.Data as DataSet;
                            break;
                        case 3:
                            strSql =
                                string.Format(
                                    "SELECT * FROM T_11_101_{0} WHERE C001 ={1} and (C002 = 1 OR C002 = 2 OR C002 = 3 OR C002 = 91) ORDER BY C001, C002",
                                    rentToken,
                                    voice.ObjID);
                            optReturn = OracleOperation.GetDataSet(strConn, strSql);
                            if (!optReturn.Result)
                            {
                                OnDebug(LogMode.Error,
                                    string.Format("LoadConfigInfo fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                                return;
                            }
                            objDataSet = optReturn.Data as DataSet;
                            break;
                        default:
                            OnDebug(LogMode.Error, string.Format("Database type not support.\t{0}", dbInfo.TypeID));
                            return;
                    }
                    if (objDataSet == null)
                    {
                        OnDebug(LogMode.Error, string.Format("ObjDataSet is null"));
                        return;
                    }
                    for (int j = 0; j < objDataSet.Tables[0].Rows.Count; j++)
                    {
                        DataRow dr = objDataSet.Tables[0].Rows[j];

                        int row = Convert.ToInt32(dr["C002"]);
                        if (row == 1)
                        {
                            int key = Convert.ToInt32(dr["C011"]);
                            voice.Key = key;
                            int id = Convert.ToInt32(dr["C012"]);
                            voice.ID = id;
                            string strHostAddress = dr["C017"].ToString();
                            strHostAddress = DecodeEncryptValue(strHostAddress);
                            voice.HostAddress = strHostAddress;
                            string strHostName = dr["C018"].ToString();
                            strHostName = DecodeEncryptValue(strHostName);
                            voice.HostName = strHostName;
                            string strPort = dr["C019"].ToString();
                            strPort = DecodeEncryptValue(strPort);
                            int intValue;
                            if (int.TryParse(strPort, out intValue))
                            {
                            }
                            voice.HostPort = intValue;
                        }
                        if (row == 3)
                        {
                            string strNMonPort = dr["C012"].ToString();
                            strNMonPort = DecodeEncryptValue(strNMonPort);
                            int intValue;
                            if (int.TryParse(strNMonPort, out intValue))
                            {
                            }
                            voice.NMonPort = intValue;
                        }
                        if (row == 91)
                        {
                            voice.Continent = dr["C011"].ToString();
                            voice.Country = dr["C012"].ToString();
                        }
                    }
                }

                #endregion


                #region 获取ScreenServer资源

                switch (dbInfo.TypeID)
                {
                    case 2:
                        strSql =
                            string.Format(
                                "SELECT * FROM T_11_101_{0} WHERE C001 > {1} AND C001 < {2} AND C002 = 1 ORDER BY C001, C002",
                                rentToken,
                                ScreenServerInfo.RESOURCE_SCREENSERVER * 10000000000000000,
                                (ScreenServerInfo.RESOURCE_SCREENSERVER + 1) * 10000000000000000);
                        optReturn = MssqlOperation.GetDataSet(strConn, strSql);
                        if (!optReturn.Result)
                        {
                            OnDebug(LogMode.Error,
                                string.Format("LoadConfigInfo fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                            return;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        strSql =
                             string.Format(
                                 "SELECT * FROM T_11_101_{0} WHERE C001 > {1} AND C001 < {2} AND C002 = 1 ORDER BY C001, C002",
                                 rentToken,
                                 ScreenServerInfo.RESOURCE_SCREENSERVER * 10000000000000000,
                                 (ScreenServerInfo.RESOURCE_SCREENSERVER + 1) * 10000000000000000);
                        optReturn = OracleOperation.GetDataSet(strConn, strSql);
                        if (!optReturn.Result)
                        {
                            OnDebug(LogMode.Error,
                                string.Format("LoadConfigInfo fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                            return;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    default:
                        OnDebug(LogMode.Error, string.Format("Database type not support.\t{0}", dbInfo.TypeID));
                        return;
                }
                if (objDataSet == null)
                {
                    OnDebug(LogMode.Error, string.Format("ObjDataSet is null"));
                    return;
                }
                for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = objDataSet.Tables[0].Rows[i];

                    ScreenServerInfo screen = new ScreenServerInfo();
                    screen.ObjType = ScreenServerInfo.RESOURCE_SCREENSERVER;
                    long objID = Convert.ToInt64(dr["C001"]);
                    screen.ObjID = objID;

                    var temp = listConfigInfos.FirstOrDefault(r => r.ObjID == screen.ObjID);
                    if (temp != null)
                    {
                        listConfigInfos.Remove(temp);
                    }
                    screenCount++;
                    listConfigInfos.Add(screen);
                }

                #endregion


                #region 获取ScreenServer配置信息

                for (int i = 0; i < listConfigInfos.Count; i++)
                {
                    var resource = listConfigInfos[i];
                    if (resource.ObjType != ScreenServerInfo.RESOURCE_SCREENSERVER) { continue; }
                    var screen = resource as ScreenServerInfo;
                    if (screen == null) { continue; }
                    switch (dbInfo.TypeID)
                    {
                        case 2:
                            strSql =
                                string.Format(
                                    "SELECT * FROM T_11_101_{0} WHERE C001 ={1} and (C002 = 1 OR C002 = 2 OR C002 = 3 OR C002 = 91) ORDER BY C001, C002",
                                    rentToken,
                                    screen.ObjID);
                            optReturn = MssqlOperation.GetDataSet(strConn, strSql);
                            if (!optReturn.Result)
                            {
                                OnDebug(LogMode.Error,
                                    string.Format("LoadConfigInfo fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                                return;
                            }
                            objDataSet = optReturn.Data as DataSet;
                            break;
                        case 3:
                            strSql =
                                string.Format(
                                    "SELECT * FROM T_11_101_{0} WHERE C001 ={1} and (C002 = 1 OR C002 = 2 OR C002 = 3 OR C002 = 91) ORDER BY C001, C002",
                                    rentToken,
                                    screen.ObjID);
                            optReturn = OracleOperation.GetDataSet(strConn, strSql);
                            if (!optReturn.Result)
                            {
                                OnDebug(LogMode.Error,
                                    string.Format("LoadConfigInfo fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                                return;
                            }
                            objDataSet = optReturn.Data as DataSet;
                            break;
                        default:
                            OnDebug(LogMode.Error, string.Format("Database type not support.\t{0}", dbInfo.TypeID));
                            return;
                    }
                    if (objDataSet == null)
                    {
                        OnDebug(LogMode.Error, string.Format("ObjDataSet is null"));
                        return;
                    }
                    for (int j = 0; j < objDataSet.Tables[0].Rows.Count; j++)
                    {
                        DataRow dr = objDataSet.Tables[0].Rows[j];

                        int row = Convert.ToInt32(dr["C002"]);
                        if (row == 1)
                        {
                            int key = Convert.ToInt32(dr["C011"]);
                            screen.Key = key;
                            int id = Convert.ToInt32(dr["C012"]);
                            screen.ID = id;
                            string strHostAddress = dr["C017"].ToString();
                            strHostAddress = DecodeEncryptValue(strHostAddress);
                            screen.HostAddress = strHostAddress;
                            string strHostName = dr["C018"].ToString();
                            strHostName = DecodeEncryptValue(strHostName);
                            screen.HostName = strHostName;
                            string strPort = dr["C019"].ToString();
                            strPort = DecodeEncryptValue(strPort);
                            int intValue;
                            if (int.TryParse(strPort, out intValue))
                            {
                            }
                            screen.HostPort = intValue;
                        }
                        if (row == 2)
                        {
                            string strMonPort = dr["C014"].ToString();
                            strMonPort = DecodeEncryptValue(strMonPort);
                            int intValue;
                            if (int.TryParse(strMonPort, out intValue))
                            {
                            }
                            screen.MonPort = intValue;
                        }
                        if (row == 91)
                        {
                            screen.Continent = dr["C011"].ToString();
                            screen.Country = dr["C012"].ToString();
                        }
                    }
                }

                #endregion


                #region 获取录音通道资源

                for (int i = 0; i < listConfigInfos.Count; i++)
                {
                    var resource = listConfigInfos[i];
                    if (resource.ObjType != VoiceServerInfo.RESOURCE_VOICESERVER) { continue; }
                    var voice = resource as VoiceServerInfo;
                    if (voice == null) { continue; }
                    switch (dbInfo.TypeID)
                    {
                        case 2:
                            strSql =
                                string.Format(
                                    "SELECT * FROM T_11_101_{0} WHERE C001 > {1} AND C001 < {2} AND C002 = 1 AND C013 = '{3}' ORDER BY C001, C002",
                                    rentToken,
                                    VoiceChanInfo.RESOURCE_VOICECHANNEL * 10000000000000000,
                                    (VoiceChanInfo.RESOURCE_VOICECHANNEL + 1) * 10000000000000000,
                                    voice.ObjID);
                            optReturn = MssqlOperation.GetDataSet(strConn, strSql);
                            if (!optReturn.Result)
                            {
                                OnDebug(LogMode.Error,
                                    string.Format("LoadConfigInfo fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                                return;
                            }
                            objDataSet = optReturn.Data as DataSet;
                            break;
                        case 3:
                            strSql =
                                string.Format(
                                    "SELECT * FROM T_11_101_{0} WHERE C001 > {1} AND C001 < {2} AND C002 = 1 AND C013 = '{3}' ORDER BY C001, C002",
                                    rentToken,
                                    VoiceChanInfo.RESOURCE_VOICECHANNEL * 10000000000000000,
                                    (VoiceChanInfo.RESOURCE_VOICECHANNEL + 1) * 10000000000000000,
                                    voice.ObjID);
                            optReturn = OracleOperation.GetDataSet(strConn, strSql);
                            if (!optReturn.Result)
                            {
                                OnDebug(LogMode.Error,
                                    string.Format("LoadConfigInfo fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                                return;
                            }
                            objDataSet = optReturn.Data as DataSet;
                            break;
                        default:
                            OnDebug(LogMode.Error, string.Format("Database type not support.\t{0}", dbInfo.TypeID));
                            return;
                    }
                    if (objDataSet == null)
                    {
                        OnDebug(LogMode.Error, string.Format("ObjDataSet is null"));
                        return;
                    }
                    for (int j = 0; j < objDataSet.Tables[0].Rows.Count; j++)
                    {
                        DataRow dr = objDataSet.Tables[0].Rows[j];

                        VoiceChanInfo channel = new VoiceChanInfo();
                        channel.ObjType = VoiceChanInfo.RESOURCE_VOICECHANNEL;
                        long objID = Convert.ToInt64(dr["C001"]);
                        channel.ObjID = objID;

                        var temp = listConfigInfos.FirstOrDefault(r => r.ObjID == channel.ObjID);
                        if (temp != null)
                        {
                            listConfigInfos.Remove(temp);
                        }
                        voiceChanCount++;
                        voice.ListChildObjects.Add(channel);
                        listConfigInfos.Add(channel);
                    }
                }

                #endregion


                #region 获取录音通道配置信息

                for (int i = 0; i < listConfigInfos.Count; i++)
                {
                    var resource = listConfigInfos[i];
                    if (resource.ObjType != VoiceChanInfo.RESOURCE_VOICECHANNEL) { continue; }
                    var channel = resource as VoiceChanInfo;
                    if (channel == null) { continue; }
                    switch (dbInfo.TypeID)
                    {
                        case 2:
                            strSql =
                                string.Format(
                                    "SELECT * FROM T_11_101_{0} WHERE C001 ={1} and (C002 = 1 or C002 = 2) ORDER BY C001, C002",
                                    rentToken,
                                    channel.ObjID);
                            optReturn = MssqlOperation.GetDataSet(strConn, strSql);
                            if (!optReturn.Result)
                            {
                                OnDebug(LogMode.Error,
                                    string.Format("LoadConfigInfo fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                                return;
                            }
                            objDataSet = optReturn.Data as DataSet;
                            break;
                        case 3:
                            strSql =
                                string.Format(
                                    "SELECT * FROM T_11_101_{0} WHERE C001 ={1} and (C002 = 1 or C002 = 2) ORDER BY C001, C002",
                                    rentToken,
                                    channel.ObjID);
                            optReturn = OracleOperation.GetDataSet(strConn, strSql);
                            if (!optReturn.Result)
                            {
                                OnDebug(LogMode.Error,
                                    string.Format("LoadConfigInfo fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                                return;
                            }
                            objDataSet = optReturn.Data as DataSet;
                            break;
                        default:
                            OnDebug(LogMode.Error, string.Format("Database type not support.\t{0}", dbInfo.TypeID));
                            return;
                    }
                    if (objDataSet == null)
                    {
                        OnDebug(LogMode.Error, string.Format("ObjDataSet is null"));
                        return;
                    }
                    for (int j = 0; j < objDataSet.Tables[0].Rows.Count; j++)
                    {
                        DataRow dr = objDataSet.Tables[0].Rows[j];

                        int row = Convert.ToInt32(dr["C002"]);
                        if (row == 1)
                        {
                            int key = Convert.ToInt32(dr["C011"]);
                            channel.Key = key;
                            int id = Convert.ToInt32(dr["C012"]);
                            channel.ID = id;
                            long parentObjID = Convert.ToInt64(dr["C013"]);
                            channel.ParentObjID = parentObjID;
                        }
                        if (row == 2)
                        {
                            string strChanName = dr["C011"].ToString();
                            channel.ChanName = strChanName;
                            string strExtension = dr["C012"].ToString();
                            channel.Extension = strExtension;
                        }
                    }
                }

                #endregion


                #region 获取录屏通道资源

                for (int i = 0; i < listConfigInfos.Count; i++)
                {
                    var resource = listConfigInfos[i];
                    if (resource.ObjType != ScreenServerInfo.RESOURCE_SCREENSERVER) { continue; }
                    var screen = resource as ScreenServerInfo;
                    if (screen == null) { continue; }
                    switch (dbInfo.TypeID)
                    {
                        case 2:
                            strSql =
                                string.Format(
                                    "SELECT * FROM T_11_101_{0} WHERE C001 > {1} AND C001 < {2} AND C002 = 1 AND C013 = '{3}' ORDER BY C001, C002",
                                    rentToken,
                                    ScreenChanInfo.RESOURCE_SCREENCHANNEL * 10000000000000000,
                                    (ScreenChanInfo.RESOURCE_SCREENCHANNEL + 1) * 10000000000000000,
                                    screen.ObjID);
                            optReturn = MssqlOperation.GetDataSet(strConn, strSql);
                            if (!optReturn.Result)
                            {
                                OnDebug(LogMode.Error,
                                    string.Format("LoadConfigInfo fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                                return;
                            }
                            objDataSet = optReturn.Data as DataSet;
                            break;
                        case 3:
                            strSql =
                                string.Format(
                                    "SELECT * FROM T_11_101_{0} WHERE C001 > {1} AND C001 < {2} AND C002 = 1 AND C013 = '{3}' ORDER BY C001, C002",
                                    rentToken,
                                    ScreenChanInfo.RESOURCE_SCREENCHANNEL * 10000000000000000,
                                    (ScreenChanInfo.RESOURCE_SCREENCHANNEL + 1) * 10000000000000000,
                                    screen.ObjID);
                            optReturn = OracleOperation.GetDataSet(strConn, strSql);
                            if (!optReturn.Result)
                            {
                                OnDebug(LogMode.Error,
                                    string.Format("LoadConfigInfo fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                                return;
                            }
                            objDataSet = optReturn.Data as DataSet;
                            break;
                        default:
                            OnDebug(LogMode.Error, string.Format("Database type not support.\t{0}", dbInfo.TypeID));
                            return;
                    }
                    if (objDataSet == null)
                    {
                        OnDebug(LogMode.Error, string.Format("ObjDataSet is null"));
                        return;
                    }
                    for (int j = 0; j < objDataSet.Tables[0].Rows.Count; j++)
                    {
                        DataRow dr = objDataSet.Tables[0].Rows[j];

                        ScreenChanInfo channel = new ScreenChanInfo();
                        channel.ObjType = ScreenChanInfo.RESOURCE_SCREENCHANNEL;
                        long objID = Convert.ToInt64(dr["C001"]);
                        channel.ObjID = objID;

                        var temp = listConfigInfos.FirstOrDefault(r => r.ObjID == channel.ObjID);
                        if (temp != null)
                        {
                            listConfigInfos.Remove(temp);
                        }
                        screenChanCount++;
                        screen.ListChildObjects.Add(channel);
                        listConfigInfos.Add(channel);
                    }
                }

                #endregion


                #region 获取录屏通道配置资源

                for (int i = 0; i < listConfigInfos.Count; i++)
                {
                    var resource = listConfigInfos[i];
                    if (resource.ObjType != ScreenChanInfo.RESOURCE_SCREENCHANNEL) { continue; }
                    var channel = resource as ScreenChanInfo;
                    if (channel == null) { continue; }
                    switch (dbInfo.TypeID)
                    {
                        case 2:
                            strSql =
                                string.Format(
                                    "SELECT * FROM T_11_101_{0} WHERE C001 ={1} and (C002 = 1 or C002 = 2) ORDER BY C001, C002",
                                    rentToken,
                                    channel.ObjID);
                            optReturn = MssqlOperation.GetDataSet(strConn, strSql);
                            if (!optReturn.Result)
                            {
                                OnDebug(LogMode.Error,
                                    string.Format("LoadConfigInfo fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                                return;
                            }
                            objDataSet = optReturn.Data as DataSet;
                            break;
                        case 3:
                            strSql =
                                string.Format(
                                    "SELECT * FROM T_11_101_{0} WHERE C001 ={1} and (C002 = 1 or C002 = 2) ORDER BY C001, C002",
                                    rentToken,
                                    channel.ObjID);
                            optReturn = OracleOperation.GetDataSet(strConn, strSql);
                            if (!optReturn.Result)
                            {
                                OnDebug(LogMode.Error,
                                    string.Format("LoadConfigInfo fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                                return;
                            }
                            objDataSet = optReturn.Data as DataSet;
                            break;
                        default:
                            OnDebug(LogMode.Error, string.Format("Database type not support.\t{0}", dbInfo.TypeID));
                            return;
                    }
                    if (objDataSet == null)
                    {
                        OnDebug(LogMode.Error, string.Format("ObjDataSet is null"));
                        return;
                    }
                    for (int j = 0; j < objDataSet.Tables[0].Rows.Count; j++)
                    {
                        DataRow dr = objDataSet.Tables[0].Rows[j];

                        int row = Convert.ToInt32(dr["C002"]);
                        if (row == 1)
                        {
                            int key = Convert.ToInt32(dr["C011"]);
                            channel.Key = key;
                            int id = Convert.ToInt32(dr["C012"]);
                            channel.ID = id;
                            long parentObjID = Convert.ToInt64(dr["C013"]);
                            channel.ParentObjID = parentObjID;
                        }
                        if (row == 2)
                        {
                            string strChanName = dr["C011"].ToString();
                            channel.ChanName = strChanName;
                            string strExtension = dr["C012"].ToString();
                            channel.Extension = strExtension;
                        }
                    }
                }

                #endregion


                mListResourceInfos.Clear();
                for (int i = 0; i < listConfigInfos.Count; i++)
                {
                    var resource = listConfigInfos[i];
                    mListResourceInfos.Add(resource);
                }

                //分机和真实分机列表合并
                mListExtensionInfos.Clear();
                for (int i = 0; i < listRealExtInfos.Count; i++)
                {
                    var ext = listRealExtInfos[i];
                    var temp = mListExtensionInfos.FirstOrDefault(e => e.Extension == ext.Extension);
                    if (temp == null)
                    {
                        mListExtensionInfos.Add(ext);
                    }
                }
                for (int i = 0; i < listExtensionInfos.Count; i++)
                {
                    var ext = listExtensionInfos[i];
                    var temp = mListExtensionInfos.FirstOrDefault(e => e.Extension == ext.Extension);
                    if (temp == null)
                    {
                        mListExtensionInfos.Add(ext);
                    }
                }
                int extCount = mListExtensionInfos.Count;

                OnDebug(LogMode.Info,
                    string.Format("LoadResourceInfos end.\tDECCount:{0};ExtCount:{1};VoiceCount:{2};ScreenCount:{3};VoiceChanCount:{4};ScreenChanCount:{5}",
                    decCount,
                    extCount,
                    voiceCount,
                    screenCount,
                    voiceChanCount,
                    screenChanCount));
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("LoadResourceInfo fail.\t{0}", ex.Message));
            }
        }

        #endregion


        #region 配置参数更新

        private void CreateConfigChangeOperator()
        {
            try
            {
                if (mConfigChangeOperator != null)
                {
                    mConfigChangeOperator.Stop();
                    mConfigChangeOperator = null;
                }
                mConfigChangeOperator = new ConfigChangeOperator();
                mConfigChangeOperator.Debug += OnDebug;
                mConfigChangeOperator.ConfigChanged += ConfigChangeOperator_ConfigChanged;
                mConfigChangeOperator.Start();

                OnDebug(LogMode.Info, string.Format("ConfigChangeOperator started."));
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("CreateConfigChangeOperator fail.\t{0}", ex.Message));
            }
        }

        void ConfigChangeOperator_ConfigChanged()
        {
            try
            {
                LoadResourceInfos();        //重新加载配置资源信息
                InitExtStateInfos();        //重新初始化分机状态
                CreateDECConnector();       //重新连接DEC，连接成功后会重新查询分机状态
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("DealConfigChanged fail.\t{0}", ex.Message));
            }
        }

        #endregion


        #region 分机状态初始化

        private void InitExtStateInfos()
        {
            try
            {
                for (int i = 0; i < mListExtensionInfos.Count; i++)
                {
                    var extInfo = mListExtensionInfos[i];

                    extInfo.ListStateInfos.Clear();
                    ExtStateInfo extState;

                    //LoginState
                    extState = new ExtStateInfo();
                    extState.ObjID = extInfo.ObjID;
                    extState.Name = extInfo.Extension;
                    extState.StateType = Service10Consts.STATE_TYPE_LOGIN;
                    extState.State = (int)LoginState.None;
                    extInfo.ListStateInfos.Add(extState);

                    //CallState
                    extState = new ExtStateInfo();
                    extState.ObjID = extInfo.ObjID;
                    extState.Name = extInfo.Extension;
                    extState.StateType = Service10Consts.STATE_TYPE_CALL;
                    extState.State = (int)CallState.Idle;
                    extInfo.ListStateInfos.Add(extState);

                    //RecordState
                    extState = new ExtStateInfo();
                    extState.ObjID = extInfo.ObjID;
                    extState.Name = extInfo.Extension;
                    extState.StateType = Service10Consts.STATE_TYPE_RECORD;
                    extState.State = (int)RecordState.None;
                    extInfo.ListStateInfos.Add(extState);

                    //DirectionState
                    extState = new ExtStateInfo();
                    extState.ObjID = extInfo.ObjID;
                    extState.Name = extInfo.Extension;
                    extState.StateType = Service10Consts.STATE_TYPE_DIRECTION;
                    extState.State = (int)DirectionState.None;
                    extInfo.ListStateInfos.Add(extState);

                    //AgentState
                    extState = new ExtStateInfo();
                    extState.ObjID = extInfo.ObjID;
                    extState.Name = extInfo.Extension;
                    extState.StateType = Service10Consts.STATE_TYPE_AGENT;
                    extState.State = (int)AgentState.Unkown;
                    extInfo.ListStateInfos.Add(extState);
                }
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("InitExtStateInfos fail.\t{0}", ex.Message));
            }
        }

        #endregion


        #region DECMessageHelper

        private void CreateDECMessageHelper()
        {
            try
            {
                mDECMessageHelper = new DECMessageHelper();
                mDECMessageHelper.Debug += OnDebug;

                OnDebug(LogMode.Info, string.Format("DECMessageHelper created"));
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("CreateDECMessageHelper fail.\t{0}", ex.Message));
            }
        }

        #endregion


        #region DEC 订阅消息

        private void CreateDECConnector()
        {
            try
            {
                var dec =
                    mListResourceInfos.FirstOrDefault(r => r.ObjType == DECServerInfo.RESOURCE_DATAEXCHANGECENTER) as
                        DECServerInfo;
                if (dec == null)
                {
                    OnDebug(LogMode.Error, string.Format("DECServer not exist."));
                    return;
                }
                if (mDecConnector != null)
                {
                    mDecConnector.Close();
                    mDecConnector = null;
                }
                mDecConnector = new DecConnector();
                mDecConnector.Debug += DecConnector_Debug;
                mDecConnector.MessageReceivedEvent += DecConnector_MessageReceivedEvent;
                mDecConnector.ServerConnectedEvent += DecConnector_ServerConnectedEvent;
                mDecConnector.AppName = string.Format("UMP Service 10");
                mDecConnector.AppVersion = GetAppVersion();
                mDecConnector.IsSubscribeAfterAuthed = false;

                mDecConnector.Host = dec.HostAddress;
                mDecConnector.Port = dec.HostPort;

                //mDecConnector.Host = "192.168.6.27";
                //mDecConnector.Port = 3072;

                mDecConnector.ModuleNumber = 0;
                mDecConnector.ModuleType = 4416;

                mDecConnector.BeginConnect();

                OnDebug(LogMode.Info,
                    string.Format("DECConnector started.\t{0}:{1}", mDecConnector.Host, mDecConnector.Port));
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("CreateDECConnector fail.\t{0}", ex.Message));
            }
        }

        void DecConnector_ServerConnectedEvent(bool isConnected, string msg)
        {
            try
            {
                if (isConnected)
                {
                    MessageString message;
                    MessageString mask;

                    //登录DEC成功后，订阅消息

                    #region 订阅CTIHubServer发布的所有消息

                    //订阅VoiceServer发布所有消息
                    mask = new MessageString();
                    mask.SourceModule = 0xffff;      //模块掩码
                    mask.SourceNumber = 0x0000;
                    mask.TargetModule = 0x0000;
                    mask.TargetNumber = 0x0000;
                    mask.LargeType = 0x0000;
                    mask.MiddleType = 0x0000;
                    mask.SmallType = 0x0000;
                    mask.Number = 0x0000;
                    message = new MessageString();
                    message.SourceModule = 0x210B;      //由CTIHubServer发布的所有消息
                    message.SourceNumber = 0x0000;
                    message.TargetModule = 0x0000;
                    message.TargetNumber = 0x0000;
                    message.LargeType = 0x0000;
                    message.MiddleType = 0x0000;
                    message.SmallType = 0x0000;
                    message.Number = 0x0000;
                    mDecConnector.AddSubscribe(mask, message);

                    #endregion


                    #region 订阅VoiceServer发布的所有消息

                    //订阅VoiceServer发布所有消息
                    mask = new MessageString();
                    mask.SourceModule = 0xffff;      //模块掩码
                    mask.SourceNumber = 0x0000;
                    mask.TargetModule = 0x0000;
                    mask.TargetNumber = 0x0000;
                    mask.LargeType = 0x0000;
                    mask.MiddleType = 0x0000;
                    mask.SmallType = 0x0000;
                    mask.Number = 0x0000;
                    message = new MessageString();
                    message.SourceModule = 0x1514;      //由VoiceServer发布的所有消息
                    message.SourceNumber = 0x0000;
                    message.TargetModule = 0x0000;
                    message.TargetNumber = 0x0000;
                    message.LargeType = 0x0000;
                    message.MiddleType = 0x0000;
                    message.SmallType = 0x0000;
                    message.Number = 0x0000;
                    mDecConnector.AddSubscribe(mask, message);

                    #endregion


                    #region 订阅ScreenServer发布的所有消息

                    //订阅ScreenServer发布的所有消息
                    mask = new MessageString();
                    mask.SourceModule = 0xffff;      //模块掩码
                    mask.SourceNumber = 0x0000;
                    mask.TargetModule = 0x0000;
                    mask.TargetNumber = 0x0000;
                    mask.LargeType = 0x0000;
                    mask.MiddleType = 0x0000;
                    mask.SmallType = 0x0000;
                    mask.Number = 0x0000;
                    message = new MessageString();
                    message.SourceModule = 0x1516;      //由ScreenServer发布的所有消息
                    message.SourceNumber = 0x0000;
                    message.TargetModule = 0x0000;
                    message.TargetNumber = 0x0000;
                    message.LargeType = 0x0000;
                    message.MiddleType = 0x0000;
                    message.SmallType = 0x0000;
                    message.Number = 0x0000;
                    mDecConnector.AddSubscribe(mask, message);

                    #endregion


                    //查询分机状态
                    UpdateExtState(mQueryExtStateWaitNum);

                }
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("DealServerConnectedEvent fail.\t{0}", ex.Message));
            }
        }

        void DecConnector_MessageReceivedEvent(object sender, VoiceCyber.SDKs.DEC.MessageReceivedEventArgs e)
        {
            try
            {
                var distHead = e.DistHead;
                var sourceID = distHead.SourceID;
                //OnDebug(LogMode.Info, string.Format("Time:{0}\tSerial:0x{1:X16}\tModuleID:0x{2:X4}",
                //    distHead.Time.ToString("yyyy-MM-dd HH:mm:ss"),
                //    sourceID,
                //    distHead.ModuleID));
                var message = distHead.Message;
                //OnDebug(LogMode.Info,
                //    string.Format("Message:{0:X4}:{1:X4}:{2:X4}:{3:X4}-{4:X4}:{5:X4}:{6:X4}:{7:X4}",
                //        message.SourceModule,
                //        message.SourceNumber,
                //        message.TargetModule,
                //        message.TargetNumber,
                //        message.Number,
                //        message.SmallType,
                //        message.MiddleType,
                //        message.LargeType));
                if (distHead.BaseType == DecDefines.NETPACK_BASETYPE_APPLICATION_VER1)
                {
                    var appHead = e.AppHead;
                    if (appHead != null)
                    {
                        //OnDebug(LogMode.Info,
                        //    string.Format(
                        //        "Channel:{0}\tEncrypt:{1}\tCompress:{2}\tFormat:{3}\tDataSize:{4}\tValidSize:{5}",
                        //        appHead.Channel,
                        //        appHead.Encrypt,
                        //        appHead.Compress,
                        //        appHead.Format,
                        //        appHead.DataSize,
                        //        appHead.ValidSize));

                        OnDebug(LogMode.Debug, "DECConnector", string.Format("Message:{0}", message));

                        string strData = Helpers.DecryptString(appHead.Encrypt, sourceID, e.Data);

                        //OnDebug(LogMode.Info, string.Format("Data:{0}", strData));
                        DealDECMessage(message, strData);
                    }
                }
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("DealVoiceServerMessage fail.\t{0}", ex.Message));
            }
        }

        void DecConnector_Debug(string msg)
        {
            OnDebug(LogMode.Debug, "DECConnector", msg);
        }

        #endregion


        #region 分机状态定时更新

        private void CreateUpdateExtStateThread()
        {
            try
            {
                mThreadUpdateExtState = new Thread(UpdateExtStateWorker);
                mThreadUpdateExtState.Start();

                OnDebug(LogMode.Info,
                    string.Format("UpdateExtState thread started.\t{0}", mThreadUpdateExtState.ManagedThreadId));
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("CreateUpdateExtStateThread fail.\t{0}", ex.Message));
            }
        }

        private void StopUpdateExtStateThread()
        {
            try
            {
                if (mThreadUpdateExtState != null)
                {
                    mThreadUpdateExtState.Abort();
                }
                mThreadUpdateExtState = null;

                OnDebug(LogMode.Info, string.Format("UpdateExtState thread stopped"));
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("StopUpdateExtStateThread fail.\t{0}", ex.Message));
            }
        }

        private void UpdateExtStateWorker()
        {
            try
            {
                while (true)
                {
                    Thread.Sleep(mUpdateExtStateInterval * 1000 * 60);

                    try
                    {
                        OnDebug(LogMode.Info, string.Format("Begin UpdateExtState."));

                        //CreateDECConnector();       //重新连接DEC并查询通道状态信息

                        UpdateExtState(mQueryExtStateWaitNum * 10);
                    }
                    catch (Exception ex)
                    {
                        OnDebug(LogMode.Error, string.Format("UpdateExtStateWorker fail.\t{0}", ex.Message));
                    }
                }
            }
            catch (Exception ex)
            {
                var abort = ex as ThreadAbortException;
                if (abort == null)
                {
                    OnDebug(LogMode.Error, string.Format("UpdateExtStateWorker fail.\t{0}", ex.Message));
                }
            }
        }

        private void UpdateExtState(int waitNum)
        {
            try
            {
                MessageString message;
                XmlDocument doc;
                DateTime dt;
                string strMessageID;
                XmlElement nodeMessage;
                XmlElement nodeDeviceInfo;
                string strMessage;

                if (mDecConnector == null) { return; }


                //向CTIHub服务器查询分机状态
                for (int i = 0; i < mListExtensionInfos.Count; i++)
                {
                    var ext = mListExtensionInfos[i];

                    message = new MessageString();
                    message.SourceModule = 0x0000;
                    message.SourceNumber = 0x0000;
                    message.TargetModule = 0x210B;
                    message.TargetNumber = 0x0000;
                    message.Number = 0x0002;
                    message.SmallType = 0x0003;
                    message.MiddleType = 0x0001;
                    message.LargeType = 0x0005;

                    //内容
                    mDecConnector.DataEncrypt = 0;
                    mDecConnector.DataFormat = DecDefines.NETPACK_BASEHEAD_VER1_FORMAT_XML;
                    doc = new XmlDocument();
                    doc.CreateXmlDeclaration("1.0", "UTF-8", "yes");

                    dt = DateTime.Now;
                    dt = dt.AddYears(-1600);
                    strMessageID = string.Format("0x{0:X4}{1:X4}{2:X4}{3:X4}",
                        message.LargeType,
                        message.MiddleType,
                        message.SmallType,
                        message.Number);
                    nodeMessage = doc.CreateElement(DECMessageHelper.NODE_MESSAGE);
                    nodeMessage.SetAttribute(DECMessageHelper.ATTR_MESSAGEID, strMessageID);
                    nodeMessage.SetAttribute(DECMessageHelper.ATTR_CURRENTTIME, dt.Ticks.ToString());
                    nodeDeviceInfo = doc.CreateElement(DECMessageHelper.NODE_DEVICEINFOMATION);
                    nodeDeviceInfo.SetAttribute(DECMessageHelper.ATTR_DEVICEID, ext.Extension);
                    nodeDeviceInfo.SetAttribute(DECMessageHelper.ATTR_DEVICETYPE, "1");
                    nodeMessage.AppendChild(nodeDeviceInfo);
                    doc.AppendChild(nodeMessage);
                    strMessage = doc.OuterXml;

                    mDecConnector.PublishMessage(strMessage, message);

                    //每向一个分机查询后，需要等待一下（5s），以免消息量太大
                    Thread.Sleep(waitNum);
                }


                //向VoiceServer服务器查询分机状态
                for (int i = 0; i < mListExtensionInfos.Count; i++)
                {
                    var ext = mListExtensionInfos[i];

                    message = new MessageString();
                    message.SourceModule = 0x0000;
                    message.SourceNumber = 0x0000;
                    message.TargetModule = 0x1514;
                    message.TargetNumber = 0x0000;
                    message.Number = 0x0002;
                    message.SmallType = 0x0002;
                    message.MiddleType = 0x0002;
                    message.LargeType = 0x0002;

                    //内容
                    mDecConnector.DataEncrypt = 0;
                    mDecConnector.DataFormat = DecDefines.NETPACK_BASEHEAD_VER1_FORMAT_XML;
                    doc = new XmlDocument();
                    doc.CreateXmlDeclaration("1.0", "UTF-8", "yes");

                    dt = DateTime.Now;
                    dt = dt.AddYears(-1600);
                    strMessageID = string.Format("0x{0:X4}{1:X4}{2:X4}{3:X4}",
                        message.LargeType,
                        message.MiddleType,
                        message.SmallType,
                        message.Number);
                    nodeMessage = doc.CreateElement(DECMessageHelper.NODE_MESSAGE);
                    nodeMessage.SetAttribute(DECMessageHelper.ATTR_MESSAGEID, strMessageID);
                    nodeMessage.SetAttribute(DECMessageHelper.ATTR_CURRENTTIME, dt.Ticks.ToString());
                    nodeDeviceInfo = doc.CreateElement(DECMessageHelper.NODE_DEVICEINFOMATION);
                    nodeDeviceInfo.SetAttribute(DECMessageHelper.ATTR_DEVICEID, ext.Extension);
                    nodeDeviceInfo.SetAttribute(DECMessageHelper.ATTR_DEVICETYPE, "1");
                    nodeMessage.AppendChild(nodeDeviceInfo);
                    doc.AppendChild(nodeMessage);
                    strMessage = doc.OuterXml;

                    mDecConnector.PublishMessage(strMessage, message);

                    //每向一个分机查询后，需要等待一下（5s），以免消息量太大
                    Thread.Sleep(waitNum);
                }

                //向ScreenServer服务器查询分机状态
                for (int i = 0; i < mListExtensionInfos.Count; i++)
                {
                    var ext = mListExtensionInfos[i];

                    message = new MessageString();
                    message.SourceModule = 0x0000;
                    message.SourceNumber = 0x0000;
                    message.TargetModule = 0x1514;
                    message.TargetNumber = 0x0000;
                    message.Number = 0x0002;
                    message.SmallType = 0x0002;
                    message.MiddleType = 0x0002;
                    message.LargeType = 0x0003;

                    //内容
                    mDecConnector.DataEncrypt = 0;
                    mDecConnector.DataFormat = DecDefines.NETPACK_BASEHEAD_VER1_FORMAT_XML;
                    doc = new XmlDocument();
                    doc.CreateXmlDeclaration("1.0", "UTF-8", "yes");

                    dt = DateTime.Now;
                    dt = dt.AddYears(-1600);
                    strMessageID = string.Format("0x{0:X4}{1:X4}{2:X4}{3:X4}",
                        message.LargeType,
                        message.MiddleType,
                        message.SmallType,
                        message.Number);
                    nodeMessage = doc.CreateElement(DECMessageHelper.NODE_MESSAGE);
                    nodeMessage.SetAttribute(DECMessageHelper.ATTR_MESSAGEID, strMessageID);
                    nodeMessage.SetAttribute(DECMessageHelper.ATTR_CURRENTTIME, dt.Ticks.ToString());
                    nodeDeviceInfo = doc.CreateElement(DECMessageHelper.NODE_DEVICEINFOMATION);
                    nodeDeviceInfo.SetAttribute(DECMessageHelper.ATTR_DEVICEID, ext.Extension);
                    nodeDeviceInfo.SetAttribute(DECMessageHelper.ATTR_DEVICETYPE, "1");
                    nodeMessage.AppendChild(nodeDeviceInfo);
                    doc.AppendChild(nodeMessage);
                    strMessage = doc.OuterXml;

                    mDecConnector.PublishMessage(strMessage, message);

                    //每向一个分机查询后，需要等待一下（5s），以免消息量太大
                    Thread.Sleep(waitNum);
                }

                OnDebug(LogMode.Info, string.Format("UpdateExtState end."));

            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("UpdateExtState fail.\t{0}", ex.Message));
            }
        }

        #endregion


        #region Others

        private string DecodeEncryptValue(string strValue)
        {
            string strReturn = strValue;
            //加密的(以连续三个char27开头）
            if (strValue.StartsWith(string.Format("{0}{0}{0}", ConstValue.SPLITER_CHAR)))
            {
                strValue = strValue.Substring(3);
                string[] arrContent = strValue.Split(new[] { ConstValue.SPLITER_CHAR }, StringSplitOptions.None);
                string strVersion = string.Empty, strMode = string.Empty, strPass = strValue;
                if (arrContent.Length > 0)
                {
                    strVersion = arrContent[0];
                }
                if (arrContent.Length > 1)
                {
                    strMode = arrContent[1];
                }
                if (arrContent.Length > 2)
                {
                    strPass = arrContent[2];
                }
                if (strVersion == "2" && strMode == "hex")
                {
                    strReturn = DecryptFromDB(strPass);
                }
            }
            return strReturn;
        }

        private string GetAppVersion()
        {
            string strReturn = string.Empty;
            try
            {
                var version = Assembly.GetExecutingAssembly().GetName().Version;
                strReturn = string.Format("{0:0}.{1:00}.{2:000}.{3:000}",
                    version.Major,
                    version.Minor,
                    version.Build,
                    version.Revision);
            }
            catch { }
            return strReturn;
        }

        #endregion


        #region Encryption and Decryption

        public string EncryptString(string strSource)
        {
            try
            {
                return ServerAESEncryption.EncryptString(strSource, EncryptionMode.AES256V04Hex);
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("EncryptString fail.\t{0}", ex.Message));
                return strSource;
            }
        }

        public string DecryptString(string strSource)
        {
            try
            {
                return ServerAESEncryption.DecryptString(strSource, EncryptionMode.AES256V04Hex);
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("DecryptString fail.\t{0}", ex.Message));
                return strSource;
            }
        }

        public string DecryptFromDB(string strSource)
        {
            try
            {
                return ServerAESEncryption.DecryptString(strSource, EncryptionMode.AES256V02Hex);
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("DecryptString fail.\t{0}", ex.Message));
                return strSource;
            }
        }

        public byte[] DecryptBytes(byte[] source, int mode)
        {
            try
            {

                return ServerAESEncryption.EncryptBytes(source, (EncryptionMode)mode);
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("EncryptBytes fail.\t{0}", ex.Message));
                return source;
            }
        }

        public byte[] DecryptBytes(byte[] source)
        {
            try
            {
                return ServerAESEncryption.DecryptBytes(source, EncryptionMode.AES256V04Hex);
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("DecryptBytes fail.\t{0}", ex.Message));
                return source;
            }
        }

        public string DecryptString(string source, int mode, Encoding encoding)
        {
            try
            {
                return ServerAESEncryption.DecryptString(source, (EncryptionMode)mode, encoding);
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("DecryptString fail.\t{0}", ex.Message));
                return source;
            }
        }

        public string DecryptString(string source, int mode)
        {
            try
            {
                return ServerAESEncryption.DecryptString(source, (EncryptionMode)mode);
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("DecryptString fail.\t{0}", ex.Message));
                return source;
            }
        }

        public byte[] EncryptBytes(byte[] source, int mode)
        {
            try
            {
                if ((mode / 1000) == 2 || (mode / 1000) == 3)
                {
                    return ServerHashEncryption.EncryptBytes(source, (EncryptionMode)mode);
                }
                return ServerAESEncryption.EncryptBytes(source, (EncryptionMode)mode);
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("EncryptBytes fail.\t{0}", ex.Message));
                return source;
            }
        }

        public byte[] EncryptBytes(byte[] source)
        {
            try
            {
                return ServerAESEncryption.EncryptBytes(source, EncryptionMode.AES256V04Hex);
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("EncryptBytes fail.\t{0}", ex.Message));
                return source;
            }
        }

        public string EncryptString(string source, int mode, Encoding encoding)
        {
            try
            {
                if ((mode / 1000) == 2 || (mode / 1000) == 3)
                {
                    return ServerHashEncryption.EncryptString(source, (EncryptionMode)mode, encoding);
                }
                return ServerAESEncryption.EncryptString(source, (EncryptionMode)mode, encoding);
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("EncryptString fail.\t{0}", ex.Message));
                return source;
            }
        }

        public string EncryptString(string source, int mode)
        {
            try
            {
                if ((mode / 1000) == 2 || (mode / 1000) == 3)
                {
                    return ServerHashEncryption.EncryptString(source, (EncryptionMode)mode);
                }
                return ServerAESEncryption.EncryptString(source, (EncryptionMode)mode);
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("EncryptString fail.\t{0}", ex.Message));
                return source;
            }
        }

        #endregion


        #region LocalMonitor

        private WebReturn DealLocalMonitorMessage(WebRequest request)
        {
            WebReturn webReturn = new WebReturn();
            webReturn.Session = mSession;
            webReturn.Result = true;
            webReturn.Code = 0;
            try
            {
                //ListData
                //0     Open/Close ( 0 表示关闭，之后不再向列表中添加监视对象；1 表示开启）
                //1     Command (指令，0 表示获取监控列表中的对象）
                if (request.ListData == null || request.ListData.Count < 2)
                {
                    webReturn.Result = false;
                    webReturn.Code = Defines.RET_PARAM_INVALID;
                    webReturn.Message = string.Format("ListData is null or count invalid");
                    return webReturn;
                }
                bool isRemember = request.ListData[0] == "1";
                mLocalMonHelper.IsRememberObject = isRemember;
                int command;
                if (!int.TryParse(request.ListData[1], out command))
                {
                    webReturn.Result = false;
                    webReturn.Code = Defines.RET_PARAM_INVALID;
                    webReturn.Message = string.Format("Command invalid");
                    return webReturn;
                }
                if (command == 0)
                {
                    return mLocalMonHelper.DealMonitorMessage(request);
                }
                OperationReturn optReturn;
                OnDebug(LogMode.Info, "LocalMonitor", string.Format("Command:{0}", command));
                switch (command)
                {
                    case ConstValue.MONITOR_COMMAND_GETSESSIONINFO:
                        webReturn.Data = mSession.ToString();
                        break;
                    case Service10Consts.MONITOR_COMMAND_GETRESOURCEOBJECT:
                        optReturn = LMGetResourceObject();
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.Data = optReturn.Data.ToString();
                        break;
                    default:
                        webReturn.Result = false;
                        webReturn.Code = Defines.RET_PARAM_INVALID;
                        webReturn.Message = string.Format("Command invalid.\t{0}", command);
                        break;
                }
            }
            catch (Exception ex)
            {
                webReturn.Result = false;
                webReturn.Code = Defines.RET_FAIL;
                webReturn.Message = ex.Message;
            }
            return webReturn;
        }

        private OperationReturn LMGetResourceObject()
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                StringBuilder sbInfo = new StringBuilder();
                int count = mListResourceInfos.Count;
                sbInfo.Append(string.Format("Count:{0}\r\n", count));
                for (int i = 0; i < count; i++)
                {
                    ResourceConfigInfo obj = mListResourceInfos[i];
                    string strInfo = obj.LogInfo();
                    sbInfo.Append(string.Format("Info:{0}\r\n", strInfo));
                }
                optReturn.Data = sbInfo.ToString();
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
                optReturn.Exception = ex;
            }
            return optReturn;
        }

        #endregion


        #region NetPipe

        /// <summary>
        /// 接收到NetPipe消息时触发此事件
        /// </summary>
        public Action<WebRequest> NetPipeEvent;

        private void OnNetPipeEvent(WebRequest webRequest)
        {
            if (NetPipeEvent != null)
            {
                NetPipeEvent(webRequest);
            }
        }

        private void CreateNetPipeService()
        {
            mNetPipeHelper = new NetPipeHelper(false, mSession.SessionID);
            mNetPipeHelper.DealMessageFunc += mNetPipeHelper_DealMessageFunc;
            try
            {
                mNetPipeHelper.Start();
                OnDebug(LogMode.Info, "NetPipe", string.Format("NetPipe service created"));
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, "NetPipe", string.Format("Start service fail.\t{0}", ex.Message));
            }
        }

        private WebReturn mNetPipeHelper_DealMessageFunc(WebRequest arg)
        {
            WebReturn webReturn = new WebReturn();
            webReturn.Result = true;
            webReturn.Code = 0;
            try
            {
                var code = arg.Code;
                var strData = arg.Data;
                OnDebug(LogMode.Info, "NetPipe", string.Format("RecieveMessage\t{0}\t{1}", code, strData));
                switch (code)
                {
                    case (int)RequestCode.CSMonitor:
                        webReturn = DealLocalMonitorMessage(arg);
                        webReturn.Session = mSession;
                        break;
                }
                if (NetPipeEvent != null)
                {
                    ThreadPool.QueueUserWorkItem(a => OnNetPipeEvent(arg));
                }
                return webReturn;
            }
            catch (Exception ex)
            {
                webReturn.Result = false;
                webReturn.Code = Defines.RET_FAIL;
                webReturn.Message = ex.Message;
                return webReturn;
            }
        }

        #endregion


        #region LogOperator

        private void CreateFileLog()
        {
            try
            {
                string path = GetLogPath();
                mLogOperator = new LogOperator();
                mLogOperator.LogPath = path;
                if (Program.IsDebug)
                {
                    //调试模式下记录所有日志信息
                    mLogOperator.LogMode = LogMode.All;
                }
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

        private void WriteLog(LogMode mode, string category, string msg)
        {
            if (mLogOperator != null)
            {
                mLogOperator.WriteLog(mode, category, msg);
            }
        }

        private void WriteLog(string category, string msg)
        {
            WriteLog(LogMode.Info, category, msg);
        }

        private void WriteLog(LogMode mode, string msg)
        {
            WriteLog(mode, "MonitorServer", msg);
        }

        private void WriteLog(string msg)
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
                    if (mLogOperator != null)
                    {
                        mLogOperator.LogMode = (LogMode)intValue;
                        OnDebug(LogMode.Info, string.Format("LogMode changed.\t{0}", (LogMode)intValue));
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLog(LogMode.Error, string.Format("SetLogMode fail.\t{0}", ex.Message));
            }
        }

        #endregion

    }
}
