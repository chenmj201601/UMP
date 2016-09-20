//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    5dd7fd61-2eb3-4ca5-b4f6-f52543e131c6
//        CLR Version:              4.0.30319.18063
//        Name:                     MonitorServer
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPService04
//        File Name:                MonitorServer
//
//        created by Charley at 2015/6/23 13:15:20
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
using VoiceCyber.UMP.CommonService04;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Encryptions;
using WebRequest = VoiceCyber.UMP.Communications.WebRequest;

/*
 * ======================================================================
 * 
 * Monitor Server 工作逻辑
 * 
 * ***启动***
 *      与一般的UMP服务一样，SyncServer在启动的过程分为以下几个部分：
 * 1、创建日志处理器（仅控制台模式需要）
 * 2、初始化，初始化执行以下操作：
 * （1）加载配置选项
 * （2）应用配置选项
 * （3）获取主目录
 * （4）创建LocalMonitorHelper和SessionInfo
 * （5）初始化SessionInfo，包括AppServerInfo，DatabaseInfo，RentInfo等
 * （6）从xml中读取AppServerInfo，DatabaseInfo
 * （7）测试数据库连接状况
 * （8）创建NetPipeService
 * 3、加载全局参数信息（本服务不需要）
 * 4、加载配置资源信息（DEC，VoiceServer，ScreenServer, IsaServer，VoiceChannel，ScreenChannel）
 * 5、初始化通道状态信息
 * 6、加载SSL证书信息
 * 7、启动Listener侦听客户端连接
 * 8、创建DEC消息处理帮助器
 * 9、创建DEC的连接器，DEC连接成功后，执行以下操作：
 * （1）订阅VoiceServer和ScreenServer的消息
 * （2）发布查询通道状态的消息
 * 10、创建资源配置信息更新处理器
 * 11、创建艺赛旗录屏控制处理器
 * 12、启动后台线程
 * （1）检查客户端连接
 * （2）重读参数
 * （3）更新通道状态
 * 
 * *** 客户端连接（详见MonitorSession及MonitorOperation）***
 * 1、客户端连接成功，创建一个MonitorSession，并生成一个SessionID
 * 2、接收客户端的请求消息（RequestMessage），然后处理请求消息并返回结果（ReturnMessage）给客户端
 * 3、处理通知消息（NotifyMessage），然后发送给客户端
 * 
 * *** 重读参数逻辑 ***
 *      重读参数由一个独立的线程负责（mThreadReReadParam）处理
 * 1、重读参数由判断mCanDBConnected标识开始，此标识在服务启动过程中首次调用TestDBConnection方法时被赋值
 * 2、当mCanDBConnected标识为false，表示服务启动过程中尝试连接数据库失败，此时需要重新连接数据库
 * 3、重读参数线程等待一小段时间（10s），然后重新调用GetDatabaseInfo方法和TestDBConnection方法
 * 4、直到数据库连接成功，mCanDBConnected被标识为true
 * 5、当mCanDBConnected标识为true，表示数据库连接成功了，此时需要定时重读参数
 * 6、重读参数线程等待一大段时间（30m），然后重新调用重读参数相关的方法获取最新的参数，同时重读配置选项文件获取最新的配置选项信息
 * 7、每次重连数据库获取重读参数后，如果mCanDBConnected被标识为true，会调用LoadGlobalParamInfo，LoadResourceInfos方法获取最新的全局参数和资源配置参数
 * 
 * *** 检查客户端连接 ***
 *      检查客户端连接由一个独立的线程（mThreadCheckSession）负责处理
 * 1、遍历mListMonitorSessions列表，检查Session的LastActiveTime
 * 2、如果超出mSessionTimeout指定的时间，则表示客户端超时，此时停止Session，剔除客户端
 * 
 * *** DEC消息处理 ***
 * 1、订阅VoiceServer和ScreenServer发布的消息
 * 2、发布查询通道状态的消息
 * 3、某写情况下，当接收到VoiceServer的启停录音的消息后，要通知IsaServer启停录屏（详见IsaControlOperator）
 * 
 * *** 定时更新通道状态 *** 
 *      定时检查通道状态由一个独立的线程（mThreadUpdateChanState）负责处理
 * 1、遍历mListResourceInfos中的所有通道，分别向VoiceServer，ScreenServer发布查询通道状态的消息
 * 2、每个通道之间会间隔一小段时间，防止消息压力太大
 * 
 * *** 配置参数状态更新 ***
 * 1、ConfigChangeOperator中创建一个UDP连接接收Service00发送的参数更新消息（广播消息，所以要通过MachineID筛选）
 * 2、收到参数更新消息后（通过绑定ConfigChangeOperator的ConfigChanged事件），执行以下操作
 *  （1）重新加载配置参数
 *  （2）初始化ChanState集合
 *  （3）重新创建DEC的连接，接收并处理订阅消息
 *      
 * *** 本地监视 ***
 * 1、创建NetPipe管道服务，监听外部调用
 * 2、收到外部调用请求后，根据指令（Command），执行响应的操作，并返回处理结果
 * 3、目前实现的本地监视指令：
 *      （1）MONITOR_COMMAND_GETSESSIONINFO：获取当前SessionInfo信息
 *      （2）MONITOR_COMMAND_GETRESOURCEOBJECT：获取当前加载配置资源信息
 *      （3）MONITOR_COMMAND_GETCHANSTATE：获取当前通道状态信息
 * 
 * ======================================================================
 */

namespace UMPService04
{
    /// <summary>
    /// 
    /// </summary>
    public class MonitorServer : IEncryptable, INetServer
    {

        #region Members

        private const string CERT_PASSWORD = "VoiceCyber,123";
        private string mAppName = "UMPService04";
        private LogOperator mLogOperator;
        private ConfigInfo mConfigInfo;
        private SessionInfo mSession;
        private object mChanStateLocker;
        private string mRootDir;
        private int mPort;
        private bool mCanDBConnected;
        private TcpListener mTcpListener;
        private X509Certificate2 mCertificate;
        private ConfigChangeOperator mConfigChangeOperator;
        private IsaControlOperator mIsaControlOperator;
        private DECMessageHelper mDECMessageHelper;
        private NetPipeHelper mNetPipeHelper;
        private LocalMonitorHelper mLocalMonHelper;

        private DecConnector mDecConnector;

        private List<INetSession> mListMonitorSessions;
        private List<ResourceConfigInfo> mListResourceInfos;
        private List<ChanState> mListChanStates;

        private Thread mThreadCheckSession;
        private Thread mThreadReReadParam;
        private Thread mThreadUpdateChanState;              //定时检查通道状态，向录音录屏服务器请求最新的通道信息

        private int mSessionTimeout = 600;                  //Session超时时间，单位s，超过此时间没有发送或接收消息，则关闭本次会话
        private int mCheckSessionInterval = 30;             //检查客户端间隔，单位s，检查过程中发现会话已经超时，则关闭连接
        private int mReReadParamInterval = 30;              //重读参数（数据库，AppServer，ConfigInfo等）时间间隔，单位m
        private int mReConnectDBInterval = 10;              //当数据库连接失败，重连数据库时间间隔，单位s
        private int mUpdateChanStateInterval = 30;          //定时检查通道状态的时间间隔，单位m
        private int mQueryChanStateWaitNum = 10;            //每查询一个通道状态要等待的时间，单位ms

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
            mListChanStates = new List<ChanState>();
            mListResourceInfos = new List<ResourceConfigInfo>();

            mChanStateLocker = new object();
            mPort = 8081 - 4;
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
                InitChanStateInfos();
                LoadServerCertificate();
                StartListener();
                CreateDECMessageHelper();
                CreateDECConnector();
                CreateConfigChangeOperator();
                CreateIsaControlOperator();

                //启动线程
                CreateCheckSessionThread();
                CreateReReadParamThread();
                CreateUpdateChanStateThread();

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
                StopUpdateChanStateThread();
                StopCheckSessionThread();
                StopReReadParamThread();

                if (mConfigChangeOperator != null)
                {
                    mConfigChangeOperator.Stop();
                    mConfigChangeOperator = null;
                }
                if (mIsaControlOperator != null)
                {
                    mIsaControlOperator.Stop();
                    mIsaControlOperator = null;
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
            catch (ObjectDisposedException ex)
            {
                //Socket disposed
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("EndAcceptTcpClient fail.\t{0}", ex.Message));
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
                    session.ListAllChanStates = mListChanStates;
                    session.ListAllResources = mListResourceInfos;
                    session.ConfigInfo = mConfigInfo;
                    session.Certificate = mCertificate;
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
            catch (ObjectDisposedException ex)
            {
                //Socket disposed
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("BeginAcceptTcpClient fail.\t{0}", ex.Message));
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
                                mPort = intPort - 4;
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
                            dbInfo.Password = strValue;
                            strValue = DecryptString(strValue);
                            dbInfo.RealPassword = strValue;
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
                  mConfigInfo.ListSettings.FirstOrDefault(s => s.Key == Service04Consts.GS_KEY_S04_INTERVAL_UPDATECHANSTATE);
                if (setting != null)
                {
                    strValue = setting.Value;

                    if (int.TryParse(strValue, out intValue)
                        && intValue >= 1
                        && intValue <= 60 * 24)
                    {
                        mUpdateChanStateInterval = intValue;
                        strLog += string.Format("{0}:{1};", Service04Consts.GS_KEY_S04_INTERVAL_UPDATECHANSTATE, intValue);
                    }
                }
                setting =
                    mConfigInfo.ListSettings.FirstOrDefault(
                        s => s.Key == Service04Consts.GS_KEY_S04_QUERYCHANSTATE_WAITNUM);
                if (setting != null)
                {
                    strValue = setting.Value;

                    if (int.TryParse(strValue, out intValue)
                        && intValue >= 1
                        && intValue <= 60 * 1000)
                    {
                        mQueryChanStateWaitNum = intValue;
                        strLog += string.Format("{0}:{1};", Service04Consts.GS_KEY_S04_QUERYCHANSTATE_WAITNUM, intValue);
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
                        Thread.Sleep(mReConnectDBInterval*1000); //如果数据库没有连接成功，等待10s重连

                        GetDatabaseInfo();
                    }
                    else
                    {
                        Thread.Sleep(mReReadParamInterval*1000*60); //等待30分钟后重读参数

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
                        if (mIsaControlOperator != null)
                        {
                            mIsaControlOperator.Refresh(); //更新IsaControlOperator中的IsaServer列表
                        }
                        InitChanStateInfos(); //重新初始化通道状态
                        CreateDECConnector(); //重新连接DEC并查询通道状态信息
                    }
                }
            }
            catch (ThreadAbortException ex)
            {
                //ThreadAbort
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("ReReadParamWorker fail.\t{0}", ex.Message));
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
            catch (ThreadAbortException ex)
            {
                //ThreadAbort
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("CheckSessionWorker fail.\t{0}", ex.Message));
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
                int voiceCount = 0;
                int screenCount = 0;
                int voiceChanCount = 0;
                int screenChanCount = 0;
                int decCount = 0;
                int isaCount = 0;


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


                #region 获取VoiceServer的配置信息

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


                #region 获取ScreenServer 的配置信息

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


                #region 获取IsaServer资源

                switch (dbInfo.TypeID)
                {
                    case 2:
                        strSql =
                            string.Format(
                                "SELECT * FROM T_11_101_{0} WHERE C001 > {1} AND C001 < {2} AND C002 = 1 ORDER BY C001, C002",
                                rentToken,
                                IsaServerInfo.RESOURCE_ISASERVER * 10000000000000000,
                                (IsaServerInfo.RESOURCE_ISASERVER + 1) * 10000000000000000);
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
                                  IsaServerInfo.RESOURCE_ISASERVER * 10000000000000000,
                                 (IsaServerInfo.RESOURCE_ISASERVER + 1) * 10000000000000000);
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

                    IsaServerInfo isa = new IsaServerInfo();
                    isa.ObjType = IsaServerInfo.RESOURCE_ISASERVER;
                    long objID = Convert.ToInt64(dr["C001"]);
                    isa.ObjID = objID;

                    var temp = listConfigInfos.FirstOrDefault(r => r.ObjID == isa.ObjID);
                    if (temp != null)
                    {
                        listConfigInfos.Remove(temp);
                    }
                    isaCount++;
                    listConfigInfos.Add(isa);
                }

                #endregion


                #region 获取IsaServer 的配置信息

                for (int i = 0; i < listConfigInfos.Count; i++)
                {
                    var resource = listConfigInfos[i];
                    if (resource.ObjType != IsaServerInfo.RESOURCE_ISASERVER) { continue; }
                    var isa = resource as IsaServerInfo;
                    if (isa == null) { continue; }
                    switch (dbInfo.TypeID)
                    {
                        case 2:
                            strSql =
                                string.Format(
                                    "SELECT * FROM T_11_101_{0} WHERE C001 ={1} and (C002 = 1 OR C002 = 2 OR C002 = 3 OR C002 = 91) ORDER BY C001, C002",
                                    rentToken,
                                    isa.ObjID);
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
                                    isa.ObjID);
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
                            isa.Key = key;
                            int id = Convert.ToInt32(dr["C012"]);
                            isa.ID = id;
                            string strHostAddress = dr["C017"].ToString();
                            strHostAddress = DecodeEncryptValue(strHostAddress);
                            isa.HostAddress = strHostAddress;
                            string strHostName = dr["C018"].ToString();
                            strHostName = DecodeEncryptValue(strHostName);
                            isa.HostName = strHostName;
                            string strPort = dr["C019"].ToString();
                            strPort = DecodeEncryptValue(strPort);
                            int intValue;
                            if (int.TryParse(strPort, out intValue))
                            {
                            }
                            isa.HostPort = intValue;
                        }
                        if (row == 2)
                        {
                            string strAccessToken = dr["C011"].ToString();
                            strAccessToken = DecodeEncryptValue(strAccessToken);
                            isa.AccessToken = strAccessToken;
                        }
                        if (row == 91)
                        {
                            isa.Continent = dr["C011"].ToString();
                            isa.Country = dr["C012"].ToString();
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


                #region 获取录屏通道配置信息

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

                OnDebug(LogMode.Info,
                    string.Format("LoadResourceInfos end.\tDECCount:{0};\tVoiceCount:{1};\tScreenCount:{2};\tIsaServerCount:{3};\tVoiceChanCount:{4};\tScreenChanCount:{5};",
                    decCount,
                    voiceCount,
                    screenCount,
                    isaCount,
                    voiceChanCount,
                    screenChanCount));
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("LoadVoiceChanInfos fail.\t{0}", ex.Message));
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
                if (mIsaControlOperator != null)
                {
                    mIsaControlOperator.Refresh();  //更新IsaControlOperator中的IsaServer列表
                }
                InitChanStateInfos();       //重新初始化通道状态
                CreateDECConnector();       //重新连接DEC并查询通道状态信息
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("DealConfigChanged fail.\t{0}", ex.Message));
            }
        }

        #endregion


        #region IsaControl 创建与处理

        private void CreateIsaControlOperator()
        {
            try
            {
                if (mIsaControlOperator != null)
                {
                    mIsaControlOperator.Stop();
                    mIsaControlOperator = null;
                }
                mIsaControlOperator = new IsaControlOperator();
                mIsaControlOperator.Debug += OnDebug;
                mIsaControlOperator.Session = mSession;
                mIsaControlOperator.ConfigInfo = mConfigInfo;
                mIsaControlOperator.ListAllResources = mListResourceInfos;
                mIsaControlOperator.DECMessageHelper = mDECMessageHelper;
                mIsaControlOperator.Start();

                OnDebug(LogMode.Info, string.Format("IsaControlOperator started."));
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("CreateIsaControlOperator fail.\t{0}", ex.Message));
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
                mDecConnector.AppName = string.Format("UMP Service 04");
                mDecConnector.AppVersion = GetAppVersion();
                mDecConnector.IsSubscribeAfterAuthed = false;

                mDecConnector.Host = dec.HostAddress;
                mDecConnector.Port = dec.HostPort;

                //mDecConnector.Host = "192.168.6.27";
                //mDecConnector.Port = 3072;

                mDecConnector.ModuleNumber = 0;
                mDecConnector.ModuleType = 2103;

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


                    //查询通道状态
                    UpdateChanState(mQueryChanStateWaitNum);
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
                        if (mIsaControlOperator != null)
                        {
                            mIsaControlOperator.DealDECMessage(message, strData);
                        }
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


        #region DEC消息处理

        private void DealDECMessage(MessageString message, string strData)
        {
            //处理DEC消息
            try
            {
                string strVoiceID;
                string strScreenID;
                string strChannelID;
                string strAgentID;
                string strRecordReference;
                DateTime dtRecordTime;
                int intRecordLength;
                string strDirection;
                string strCallerID;
                string strCalledID;
                string strRealExtension;
                ChanState chanState;

                if (mDECMessageHelper == null) { return; }
                //string strMessage = message.ToString();
                string strMessage = string.Format("{0:X4}:{1:X4}:{2:X4}:{3:X4}",
                    message.LargeType,
                    message.MiddleType,
                    message.SmallType,
                    message.Number);
                switch (strMessage)
                {

                    #region 处理VoiceServer发布的消息

                    case DECMessageHelper.MSG_VOC_HEARTBEAT:
                        //OnDebug(LogMode.Info, string.Format("{0}", strData));
                        break;
                    case DECMessageHelper.MSG_VOC_RECORDSTARTED:
                        strVoiceID = mDECMessageHelper.GetVoiceIDValue(strData);
                        strChannelID = mDECMessageHelper.GetChannelIDValue(strData);
                        strRecordReference = mDECMessageHelper.GetRecordReferenceValue(strData);
                        dtRecordTime = mDECMessageHelper.GetRecordTimeValue(strData);
                        intRecordLength = mDECMessageHelper.GetRecordLengthValue(strData);
                        OnDebug(LogMode.Debug, "DECConnector",
                            string.Format("RecordStarted\t{0};\t{1};\t{2};\t{3};\t{4};",
                                strVoiceID,
                                strChannelID,
                                strRecordReference,
                                dtRecordTime,
                                intRecordLength));
                        lock (mChanStateLocker)
                        {
                            chanState =
                                mListChanStates.FirstOrDefault(
                                    c => c.ObjType == ConstValue.RESOURCE_VOICECHANNEL
                                         && c.ServerID.ToString() == strVoiceID
                                         && c.ChanID.ToString() == strChannelID);
                            if (chanState != null)
                            {
                                chanState.RecordState = RecordState.Recoding;
                                chanState.RecordReference = strRecordReference;
                                chanState.StartRecordTime = dtRecordTime;
                                DateTime now = DateTime.Now.ToUniversalTime();
                                chanState.TimeDeviation = (now - chanState.StartRecordTime).TotalSeconds -
                                                          intRecordLength;
                                DoChanStateChangedMessage(chanState);
                            }
                        }
                        break;
                    case DECMessageHelper.MSG_VOC_RECORDSTOPPED:
                        strVoiceID = mDECMessageHelper.GetVoiceIDValue(strData);
                        strChannelID = mDECMessageHelper.GetChannelIDValue(strData);
                        strRecordReference = mDECMessageHelper.GetRecordReferenceValue(strData);
                        dtRecordTime = mDECMessageHelper.GetRecordTimeValue(strData);
                        intRecordLength = mDECMessageHelper.GetRecordLengthValue(strData);
                        OnDebug(LogMode.Debug, "DECConnector",
                            string.Format("RecordStopped\t{0};\t{1};\t{2};\t{3};\t{4};",
                                strVoiceID,
                                strChannelID,
                                strRecordReference,
                                dtRecordTime,
                                intRecordLength));
                        lock (mChanStateLocker)
                        {
                            chanState =
                                mListChanStates.FirstOrDefault(
                                    c => c.ObjType == ConstValue.RESOURCE_VOICECHANNEL
                                         && c.ServerID.ToString() == strVoiceID
                                         && c.ChanID.ToString() == strChannelID);
                            if (chanState != null)
                            {
                                chanState.RecordState = RecordState.None;
                                chanState.RecordReference = strRecordReference;
                                chanState.StopRecordTime = dtRecordTime;
                                chanState.RecordLength = intRecordLength;
                                DoChanStateChangedMessage(chanState);
                            }
                        }
                        break;
                    case DECMessageHelper.MSG_VOC_CALLINFO:
                        strVoiceID = mDECMessageHelper.GetVoiceIDValue(strData);
                        strChannelID = mDECMessageHelper.GetChannelIDValue(strData);
                        strDirection = mDECMessageHelper.GetDirectionFlagValue(strData);
                        strCallerID = mDECMessageHelper.GetCallerIDValue(strData);
                        strCalledID = mDECMessageHelper.GetCalledIDValue(strData);
                        strAgentID = mDECMessageHelper.GetCallInfoAgentIDValue(strData);
                        strRealExtension = mDECMessageHelper.GetRealExtensionValue(strData);
                        OnDebug(LogMode.Debug, "DECConnector",
                            string.Format("CallInfo\t{0};\t{1};\t{2};\t{3};\t{4};\t{5};\t{6};",
                                strVoiceID,
                                strChannelID,
                                strDirection,
                                strCallerID,
                                strCalledID,
                                strAgentID,
                                strRealExtension));
                        lock (mChanStateLocker)
                        {
                            chanState =
                                mListChanStates.FirstOrDefault(
                                    c => c.ObjType == ConstValue.RESOURCE_VOICECHANNEL
                                         && c.ServerID.ToString() == strVoiceID
                                         && c.ChanID.ToString() == strChannelID);
                            if (chanState != null)
                            {
                                chanState.DirectionFlag = strDirection == "1" ? 1 : 0;
                                chanState.CallerID = strCallerID;
                                chanState.CalledID = strCalledID;
                                chanState.AgentID = strAgentID;
                                chanState.RealExt = strRealExtension;
                                CheckAgentLoginState(strAgentID, chanState);    //分析并检查通道状态
                                CheckRealExtLoginState(strRealExtension, chanState);
                                DoChanStateChangedMessage(chanState);
                            }
                        }
                        break;
                    case DECMessageHelper.MSG_VOC_AGENTLOGON:
                        strVoiceID = mDECMessageHelper.GetVoiceIDValue(strData);
                        strChannelID = mDECMessageHelper.GetChannelIDValue(strData);
                        strAgentID = mDECMessageHelper.GetAgentIDValue(strData);
                        OnDebug(LogMode.Debug, "DECConnector", string.Format("AgentLogOn\t{0};\t{1};\t{2};",
                            strVoiceID,
                            strChannelID,
                            strAgentID));
                        lock (mChanStateLocker)
                        {
                            chanState =
                                mListChanStates.FirstOrDefault(
                                    c => c.ObjType == ConstValue.RESOURCE_VOICECHANNEL
                                         && c.ServerID.ToString() == strVoiceID
                                         && c.ChanID.ToString() == strChannelID);
                            if (chanState != null)
                            {
                                chanState.LoginState = LoginState.LogOn;
                                chanState.AgentID = strAgentID;
                                CheckAgentLoginState(strAgentID, chanState);    //分析并检查通道状态
                                DoChanStateChangedMessage(chanState);
                            }
                        }
                        break;
                    case DECMessageHelper.MSG_VOC_AGENTLOGOFF:
                        strVoiceID = mDECMessageHelper.GetVoiceIDValue(strData);
                        strChannelID = mDECMessageHelper.GetChannelIDValue(strData);
                        OnDebug(LogMode.Debug, "DECConnector", string.Format("AgentLogOff\t{0};\t{1};",
                            strVoiceID,
                            strChannelID));
                        lock (mChanStateLocker)
                        {
                            chanState =
                                mListChanStates.FirstOrDefault(
                                    c => c.ObjType == ConstValue.RESOURCE_VOICECHANNEL
                                         && c.ServerID.ToString() == strVoiceID
                                         && c.ChanID.ToString() == strChannelID);
                            if (chanState != null)
                            {
                                chanState.LoginState = LoginState.LogOff;
                                chanState.AgentID = string.Empty;
                                DoChanStateChangedMessage(chanState);
                            }
                        }
                        break;
                    case DECMessageHelper.MSG_VOC_CHANNELCONNECTED:

                        break;
                    case DECMessageHelper.MSG_VOC_CHANNELDISCONNECTED:

                        break;

                    #endregion


                    #region 处理ScreenServer发布的消息

                    case DECMessageHelper.MSG_SCR_HEARTBEAT:
                        //OnDebug(LogMode.Info, string.Format("{0}", strData));
                        break;
                    case DECMessageHelper.MSG_SCR_RECORDSTARTED:
                        strScreenID = mDECMessageHelper.GetScreenIDValue(strData);
                        strChannelID = mDECMessageHelper.GetChannelIDValue(strData);
                        strRecordReference = mDECMessageHelper.GetRecordReferenceValue(strData);
                        dtRecordTime = mDECMessageHelper.GetRecordTimeValue(strData);
                        intRecordLength = mDECMessageHelper.GetRecordLengthValue(strData);
                        OnDebug(LogMode.Debug, "DECConnector",
                            string.Format("RecordStarted\t{0};\t{1};\t{2};\t{3};\t{4};",
                                strScreenID,
                                strChannelID,
                                strRecordReference,
                                dtRecordTime,
                                intRecordLength));
                        lock (mChanStateLocker)
                        {
                            chanState =
                                mListChanStates.FirstOrDefault(
                                    c => c.ObjType == ScreenChanInfo.RESOURCE_SCREENCHANNEL
                                         && c.ServerID.ToString() == strScreenID
                                         && c.ChanID.ToString() == strChannelID);
                            if (chanState != null)
                            {
                                chanState.RecordState = RecordState.Recoding;
                                chanState.RecordReference = strRecordReference;
                                chanState.StartRecordTime = dtRecordTime;
                                DateTime now = DateTime.Now.ToUniversalTime();
                                chanState.TimeDeviation = (now - chanState.StartRecordTime).TotalSeconds -
                                                          intRecordLength;
                                DoChanStateChangedMessage(chanState);
                            }
                        }
                        break;
                    case DECMessageHelper.MSG_SCR_RECORDSTOPPED:
                        strScreenID = mDECMessageHelper.GetScreenIDValue(strData);
                        strChannelID = mDECMessageHelper.GetChannelIDValue(strData);
                        strRecordReference = mDECMessageHelper.GetRecordReferenceValue(strData);
                        dtRecordTime = mDECMessageHelper.GetRecordTimeValue(strData);
                        intRecordLength = mDECMessageHelper.GetRecordLengthValue(strData);
                        OnDebug(LogMode.Debug, "DECConnector",
                            string.Format("RecordStopped\t{0};\t{1};\t{2};\t{3};\t{4};",
                                strScreenID,
                                strChannelID,
                                strRecordReference,
                                dtRecordTime,
                                intRecordLength));
                        lock (mChanStateLocker)
                        {
                            chanState =
                                mListChanStates.FirstOrDefault(
                                    c => c.ObjType == ScreenChanInfo.RESOURCE_SCREENCHANNEL
                                         && c.ServerID.ToString() == strScreenID
                                         && c.ChanID.ToString() == strChannelID);
                            if (chanState != null)
                            {
                                chanState.RecordState = RecordState.None;
                                chanState.RecordReference = strRecordReference;
                                chanState.StopRecordTime = dtRecordTime;
                                chanState.RecordLength = intRecordLength;
                                DoChanStateChangedMessage(chanState);
                            }
                        }
                        break;
                    case DECMessageHelper.MSG_SCR_AGENTLOGON:
                        strScreenID = mDECMessageHelper.GetScreenIDValue(strData);
                        strChannelID = mDECMessageHelper.GetChannelIDValue(strData);
                        strAgentID = mDECMessageHelper.GetAgentIDValue(strData);
                        OnDebug(LogMode.Debug, "DECConnector", string.Format("AgentLogOn\t{0};\t{1};\t{2};",
                            strScreenID,
                            strChannelID,
                            strAgentID));
                        lock (mChanStateLocker)
                        {
                            chanState =
                                mListChanStates.FirstOrDefault(
                                    c => c.ObjType == ScreenChanInfo.RESOURCE_SCREENCHANNEL
                                         && c.ServerID.ToString() == strScreenID
                                         && c.ChanID.ToString() == strChannelID);
                            if (chanState != null)
                            {
                                chanState.LoginState = LoginState.LogOn;
                                chanState.AgentID = strAgentID;
                                CheckAgentLoginState(strAgentID, chanState);    //分析并检查通道状态
                                DoChanStateChangedMessage(chanState);
                            }
                        }
                        break;
                    case DECMessageHelper.MSG_SCR_AGENTLOGOFF:
                        strScreenID = mDECMessageHelper.GetScreenIDValue(strData);
                        strChannelID = mDECMessageHelper.GetChannelIDValue(strData);
                        OnDebug(LogMode.Debug, "DECConnector", string.Format("AgentLogOff\t{0};\t{1};",
                            strScreenID,
                            strChannelID));
                        lock (mChanStateLocker)
                        {
                            chanState =
                                mListChanStates.FirstOrDefault(
                                    c => c.ObjType == ScreenChanInfo.RESOURCE_SCREENCHANNEL
                                         && c.ServerID.ToString() == strScreenID
                                         && c.ChanID.ToString() == strChannelID);
                            if (chanState != null)
                            {
                                chanState.LoginState = LoginState.LogOff;
                                chanState.AgentID = string.Empty;
                                DoChanStateChangedMessage(chanState);
                            }
                        }
                        break;

                    #endregion

                }
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("DealDECMessage fail.\t{0}", ex.Message));
            }
        }

        private void CheckAgentLoginState(string strAgentID, ChanState chanState)
        {
            try
            {
                if (string.IsNullOrEmpty(strAgentID))
                {
                    //坐席为空，则认为坐席登出改通道，修改登录状态
                    chanState.LoginState = LoginState.LogOff;
                }
                else
                {
                    //坐席不为空，则认为坐席登录该通道，修改登录状态
                    chanState.LoginState = LoginState.LogOn;

                    if (strAgentID != chanState.AgentID)
                    {
                        for (int i = 0; i < mListChanStates.Count; i++)
                        {
                            var temp = mListChanStates[i];
                            if (temp.ObjID == chanState.ObjID) { continue; }
                            if (temp.ObjType == chanState.ObjType
                                && temp.AgentID == strAgentID)
                            {
                                //如果查询到该坐席还登录在其他通道上，则在其他通道上登出，同时发出通知
                                temp.AgentID = string.Empty;
                                temp.LoginState = LoginState.LogOff;

                                DoChanStateChangedMessage(temp);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("CheckAgentLoginState fail.\t{0}", ex.Message));
            }
        }

        private void CheckRealExtLoginState(string strRealExt, ChanState chanState)
        {
            try
            {
                if (string.IsNullOrEmpty(strRealExt))
                {
                    //真实分机登出通道
                }
                else
                {
                    //真实分机登录通道，此时真实分机应该从其他通道上登出
                    for (int i = 0; i < mListChanStates.Count; i++)
                    {
                        var temp = mListChanStates[i];
                        if (temp.ObjID == chanState.ObjID) { continue; }
                        if (temp.ObjType == chanState.ObjType
                            && temp.RealExt == strRealExt)
                        {
                            temp.RealExt = string.Empty;

                            DoChanStateChangedMessage(temp);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("CheckRealExtLoginState fail.\t{0}", ex.Message));
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


        #region 处理通道状态更新消息

        private void DoChanStateChangedMessage(ChanState chanState)
        {
            try
            {
                //传统方式
                for (int i = 0; i < mListMonitorSessions.Count; i++)
                {
                    var session = mListMonitorSessions[i] as MonitorOperations;
                    if (session != null)
                    {
                        session.DoNotifyMessage(chanState);
                    }
                }

                ////并行方式
                //Parallel.ForEach(mListMonitorSessions, s =>
                //{
                //    var o = s as MonitorOperations;
                //    if (o != null)
                //    {
                //        o.DoNotifyMessage(chanState);
                //    }
                //});
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("DoChanStateChangedMessage fail.\t{0}", ex.Message));
            }
        }

        #endregion


        #region 通道状态定时更新

        private void CreateUpdateChanStateThread()
        {
            try
            {
                mThreadUpdateChanState = new Thread(UpdateChanStateWorker);
                mThreadUpdateChanState.Start();

                OnDebug(LogMode.Info,
                    string.Format("UpdateChanState thread started.\t{0}", mThreadUpdateChanState.ManagedThreadId));
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("CreateUpdateChanStateThread fail.\t{0}", ex.Message));
            }
        }

        private void StopUpdateChanStateThread()
        {
            try
            {
                if (mThreadUpdateChanState != null)
                {
                    mThreadUpdateChanState.Abort();
                    mThreadUpdateChanState = null;
                }

                OnDebug(LogMode.Info, string.Format("UpdateChanState thread stopped"));
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("StopUpdateChanStateThread fail.\t{0}", ex.Message));
            }
        }

        private void UpdateChanStateWorker()
        {
            try
            {
                while (true)
                {
                    Thread.Sleep(mUpdateChanStateInterval * 1000 * 60);

                    try
                    {
                        OnDebug(LogMode.Info, string.Format("Begin UpdateChanState."));

                        //CreateDECConnector();       //重新连接DEC并查询通道状态信息

                        UpdateChanState(mQueryChanStateWaitNum * 10);
                    }
                    catch (Exception ex)
                    {
                        OnDebug(LogMode.Error, string.Format("UpdateChanStateWorker fail.\t{0}", ex.Message));
                    }
                }
            }
            catch (ThreadAbortException ex)
            {
                //ThreadAbort
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("UpdateChanStateWorker fail.\t{0}", ex.Message));
            }
        }

        private void UpdateChanState(int waitNum)
        {
            try
            {
                //获取所有通道，然后依次向每个通道发送查询通道状态的消息
                MessageString message;
                XmlDocument doc;
                DateTime dt;
                string strMessageID;
                XmlElement nodeMessage;
                XmlElement nodeDeviceInfo;
                string strMessage;

                if (mDecConnector == null) { return; }

                //向录音服务器查询录音通道状态
                var listVoiceChannels =
                    mListResourceInfos.Where(r => r.ObjType == VoiceChanInfo.RESOURCE_VOICECHANNEL)
                        .OrderBy(r => r.ObjID)
                        .ToList();
                for (int i = 0; i < listVoiceChannels.Count; i++)
                {
                    var chan = listVoiceChannels[i] as VoiceChanInfo;
                    if (chan == null) { continue; }

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
                    nodeDeviceInfo.SetAttribute(DECMessageHelper.ATTR_DEVICEID, chan.Extension);
                    nodeDeviceInfo.SetAttribute(DECMessageHelper.ATTR_DEVICETYPE, "1");
                    nodeMessage.AppendChild(nodeDeviceInfo);
                    doc.AppendChild(nodeMessage);
                    strMessage = doc.OuterXml;

                    mDecConnector.PublishMessage(strMessage, message);

                    //每向一个通道查询后，需要等待一下（5s），以免消息量太大
                    Thread.Sleep(waitNum);
                }

                //向录屏服务器查询录屏状态
                var listScreenChannels =
                    mListResourceInfos.Where(r => r.ObjType == ScreenChanInfo.RESOURCE_SCREENCHANNEL)
                        .OrderBy(r => r.ObjID)
                        .ToList();
                for (int i = 0; i < listScreenChannels.Count; i++)
                {
                    var chan = listScreenChannels[i] as ScreenChanInfo;
                    if (chan == null) { continue; }

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
                    nodeDeviceInfo.SetAttribute(DECMessageHelper.ATTR_DEVICEID, chan.Extension);
                    nodeDeviceInfo.SetAttribute(DECMessageHelper.ATTR_DEVICETYPE, "1");
                    nodeMessage.AppendChild(nodeDeviceInfo);
                    doc.AppendChild(nodeMessage);
                    strMessage = doc.OuterXml;

                    mDecConnector.PublishMessage(strMessage, message);

                    //每向一个通道查询后，需要等待一下，以免消息量太大
                    Thread.Sleep(waitNum);
                }

                OnDebug(LogMode.Info, string.Format("UpdateChanState end."));
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("UpdateChanState fail.\t{0}", ex.Message));
            }
        }

        #endregion


        #region 通道状态初始化

        private void InitChanStateInfos()
        {
            try
            {
                lock (mChanStateLocker)
                {
                    for (int i = 0; i < mListResourceInfos.Count; i++)
                    {
                        var resource = mListResourceInfos[i];
                        if (resource.ObjType == VoiceChanInfo.RESOURCE_VOICECHANNEL)
                        {
                            //录音通道
                            var channel = resource as VoiceChanInfo;
                            if (channel == null) { continue; }
                            var voice =
                                mListResourceInfos.FirstOrDefault(r => r.ObjID == channel.ParentObjID) as VoiceServerInfo;
                            if (voice == null) { continue; }

                            ChanState chanState = new ChanState();
                            chanState.ObjID = channel.ObjID;
                            chanState.ObjType = ConstValue.RESOURCE_VOICECHANNEL;
                            chanState.ServerID = voice.ID;
                            chanState.ChanID = channel.ID;
                            chanState.Extension = channel.Extension;
                            chanState.AgentID = string.Empty;
                            chanState.OrgObjID = 0;

                            chanState.Other03 = voice.HostAddress;

                            chanState.LoginState = LoginState.LogOff;
                            chanState.CallState = CallState.Idle;
                            chanState.RecordState = RecordState.None;

                            var temp = mListChanStates.FirstOrDefault(c => c.ObjID == chanState.ObjID);
                            if (temp != null)
                            {
                                mListChanStates.Remove(temp);
                            }
                            mListChanStates.Add(chanState);
                        }
                        if (resource.ObjType == ScreenChanInfo.RESOURCE_SCREENCHANNEL)
                        {
                            //录屏通道
                            var channel = resource as ScreenChanInfo;
                            if (channel == null) { continue; }
                            var screen =
                                mListResourceInfos.FirstOrDefault(r => r.ObjID == channel.ParentObjID) as ScreenServerInfo;
                            if (screen == null) { continue; }

                            ChanState chanState = new ChanState();
                            chanState.ObjID = channel.ObjID;
                            chanState.ObjType = ScreenChanInfo.RESOURCE_SCREENCHANNEL;
                            chanState.ServerID = screen.ID;
                            chanState.ChanID = channel.ID;
                            chanState.Extension = channel.Extension;
                            chanState.AgentID = string.Empty;
                            chanState.OrgObjID = 0;

                            chanState.Other03 = screen.HostAddress;

                            chanState.LoginState = LoginState.LogOff;
                            chanState.CallState = CallState.Idle;
                            chanState.RecordState = RecordState.None;

                            var temp = mListChanStates.FirstOrDefault(c => c.ObjID == chanState.ObjID);
                            if (temp != null)
                            {
                                mListChanStates.Remove(temp);
                            }
                            mListChanStates.Add(chanState);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("InitChanStateInfos fail.\t{0}", ex.Message));
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
                    case Service04Consts.MONITOR_COMMAND_GETRESOURCEOBJECT:
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
                    case Service04Consts.MONITOR_COMMAND_GETCHANSTATE:
                        optReturn = LMGetChanState();
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

        private OperationReturn LMGetChanState()
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                StringBuilder sbInfo = new StringBuilder();
                int count = mListChanStates.Count;
                sbInfo.Append(string.Format("Count:{0}\r\n", count));
                for (int i = 0; i < count; i++)
                {
                    ChanState obj = mListChanStates[i];
                    optReturn = XMLHelper.SeriallizeObject(obj);
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                    sbInfo.Append(string.Format("Info:{0}\r\n", optReturn.Data));
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
