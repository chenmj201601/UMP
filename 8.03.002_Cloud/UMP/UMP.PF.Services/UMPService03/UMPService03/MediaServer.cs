//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    558406c9-a229-4330-9215-bfd49476c27b
//        CLR Version:              4.0.30319.18444
//        Name:                     MediaServer
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPService03
//        File Name:                MediaServer
//
//        created by Charley at 2015/3/26 15:55:37
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
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.CommonService03;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Encryptions;
using WebRequest = VoiceCyber.UMP.Communications.WebRequest;

/*
 * ======================================================================
 * 
 * MediaServer 工作逻辑
 * 
 * ***启动***
 *      与一般的UMP服务一样，MediaServer在启动的过程分为以下几个部分：
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
 * 4、加载配置资源信息（Sftp，DownloadParam，IsaServer，StorageDevice等）
 * 5、加载SSL证书信息
 * 6、启动Listener侦听客户端连接
 * 7、创建资源配置信息更新处理器
 * 
 * ***客户端连接（详见MediaSession及MediaOperation）***
 * 1、客户端连接成功，创建一个MediaOperation，并生成一个SessionID
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
 * ***配置参数状态更新***
 * 1、ConfigChangeOperator中创建一个UDP连接接收Service00发送的参数更新消息（广播消息，所以要通过MachineID筛选）
 * 2、收到参数更新消息后（通过绑定ConfigChangeOperator的ConfigChanged事件），执行以下操作
 *      （1）重新加载配置参数
 *      
 * ***本地监视***
 * 1、创建NetPipe管道服务，监听外部调用
 * 2、收到外部调用请求后，根据指令（Command），执行响应的操作，并返回处理结果
 * 3、目前实现的本地监视指令：
 *      （1）MONITOR_COMMAND_GETRESOURCEOBJECT：获取当前加载的配置资源信息
 * 
 * ======================================================================
 */

namespace UMPService03
{
    public class MediaServer : IEncryptable,INetServer
    {

        #region Memebers

        private string CERT_PASSWORD = "VoiceCyber,123";
        private string mAppName = "UMPService03";
        private LogOperator mLogOperator;
        private ConfigInfo mConfigInfo;
        private SessionInfo mSession;
        private string mRootDir;
        private string mMediaDataDir;
        private int mPort;
        private bool mCanDBConnected;
        private TcpListener mTcpListener;
        private X509Certificate2 mCertificate;
        private ConfigChangeOperator mConfigChangeOperator;
        private NetPipeHelper mNetPipeHelper;
        private LocalMonitorHelper mLocalMonHelper;

        private List<INetSession> mListMediaSessions;
        private List<ResourceConfigInfo> mListResourceInfos;

        private Thread mThreadCheckSession;
        private Thread mThreadReReadParam;

        private int mSessionTimeout = 600;       //Session超时时间，单位s
        private int mCheckSessionInterval = 30;  //检查客户端间隔，单位s
        private int mReReadParamInterval = 30;              //重读参数（数据库，AppServer，ConfigInfo等）时间间隔，单位m
        private int mReConnectDBInterval = 10;              //当数据库连接失败，重连数据库时间间隔，单位s

        #endregion



        #region ListSessions

        public IList<INetSession> ListSessions
        {
            get { return mListMediaSessions; }
        }

        #endregion


        public MediaServer()
        {
            mListMediaSessions = new List<INetSession>();
            mListResourceInfos = new List<ResourceConfigInfo>();

            mPort = 8081 - 3;
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
                OnDebug(LogMode.Info, string.Format("MediaServer starting..."));
                Init();
                LoadResourceInfos();
                LoadServerCertificate();
                StartListener();
                CreateConfigChangeOperator();

                //启动线程
                CreateCheckSessionThread();
                CreateReReadParamThread();

                OnDebug(LogMode.Info, string.Format("MediaServer started"));
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("MediaServer start fail.\t{0}", ex.Message));
            }
        }

        public void Stop()
        {
            try
            {
                //停止线程
                StopCheckSessionThread();
                StopReReadParamThread();

                if (mConfigChangeOperator != null)
                {
                    mConfigChangeOperator.Stop();
                    mConfigChangeOperator = null;
                }
                for (int i = 0; i < mListMediaSessions.Count; i++)
                {
                    mListMediaSessions[i].Stop();
                }
                StopListener();
                OnDebug(LogMode.Info, string.Format("MediaServer stopped"));
                if (mLogOperator != null)
                {
                    mLogOperator.Stop();
                    mLogOperator = null;
                }
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("MediaServer stop fail.\t{0}", ex.Message));
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
                mMediaDataDir = Path.Combine(mRootDir, ConstValue.TEMP_DIR_MEDIADATA);
                if (!Directory.Exists(mMediaDataDir))
                {
                    Directory.CreateDirectory(mMediaDataDir);
                }
                OnDebug(LogMode.Info, string.Format("MediaData path:{0}", mMediaDataDir));

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
                OnDebug(LogMode.Error, string.Format("MediaServer init fail.\t{0}", ex.Message));
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
            appServerInfo.Address = "192.168.6.27";
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

            //dbInfo = new DatabaseInfo();
            //dbInfo.TypeID = 3;
            //dbInfo.Host = "192.168.4.182";
            //dbInfo.Port = 1521;
            //dbInfo.DBName = "PFOrcl";
            //dbInfo.LoginName = "PFDEV_TEST";
            //dbInfo.Password = "pfdev_test";

            dbInfo = new DatabaseInfo();
            dbInfo.TypeID = 2;
            dbInfo.Host = "192.168.4.182";
            dbInfo.Port = 1433;
            dbInfo.DBName = "UMPDataDB1027";
            dbInfo.LoginName = "PFDEV";
            dbInfo.Password = "PF,123";

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
                                mPort = intPort - 3;
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
                        Thread.Sleep(mReReadParamInterval * 1000 * 60);     //等待30分钟后重读参数

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
                    }
                }
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
                int sftpServerCount = 0;
                int downloadParamCount = 0;
                int isaServerCount = 0;
                int deviceCount = 0;


                #region 获取SftpServer资源

                switch (dbInfo.TypeID)
                {
                    case 2:
                        strSql =
                            string.Format(
                                "SELECT * FROM T_11_101_{0} WHERE C001 > {1} AND C001 < {2} AND C002 = 1 ORDER BY C001, C002",
                                rentToken,
                                SftpServerInfo.RESOURCE_SFTPSERVER * 10000000000000000,
                                (SftpServerInfo.RESOURCE_SFTPSERVER + 1) * 10000000000000000);
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
                                 SftpServerInfo.RESOURCE_SFTPSERVER * 10000000000000000,
                                 (SftpServerInfo.RESOURCE_SFTPSERVER + 1) * 10000000000000000);
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

                    SftpServerInfo sftp = new SftpServerInfo();
                    sftp.ObjType = SftpServerInfo.RESOURCE_SFTPSERVER;
                    long objID = Convert.ToInt64(dr["C001"]);
                    sftp.ObjID = objID;

                    var temp = listConfigInfos.FirstOrDefault(r => r.ObjID == sftp.ObjID);
                    if (temp != null)
                    {
                        listConfigInfos.Remove(temp);
                    }
                    sftpServerCount++;
                    listConfigInfos.Add(sftp);
                }

                #endregion


                #region 获取SftpServer的配置信息

                for (int i = 0; i < listConfigInfos.Count; i++)
                {
                    var resource = listConfigInfos[i];
                    if (resource.ObjType != SftpServerInfo.RESOURCE_SFTPSERVER) { continue; }
                    var sftp = resource as SftpServerInfo;
                    if (sftp == null) { continue; }
                    switch (dbInfo.TypeID)
                    {
                        case 2:
                            strSql =
                                string.Format(
                                    "SELECT * FROM T_11_101_{0} WHERE C001 ={1} AND (C002 = 1 OR C002 = 2 OR C002 = 3 OR C002 = 92) ORDER BY C001, C002",
                                    rentToken,
                                    sftp.ObjID);
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
                                    "SELECT * FROM T_11_101_{0} WHERE C001 ={1} AND (C002 = 1 OR C002 = 2 OR C002 = 3 OR C002 = 92) ORDER BY C001, C002",
                                    rentToken,
                                    sftp.ObjID);
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
                            sftp.Key = key;
                            int id = Convert.ToInt32(dr["C012"]);
                            sftp.ID = id;
                            string strHostAddress = dr["C017"].ToString();
                            strHostAddress = DecodeEncryptValue(strHostAddress);
                            sftp.HostAddress = strHostAddress;
                            string strHostName = dr["C018"].ToString();
                            strHostName = DecodeEncryptValue(strHostName);
                            sftp.HostName = strHostName;
                            string strPort = dr["C019"].ToString();
                            strPort = DecodeEncryptValue(strPort);
                            int intValue;
                            if (int.TryParse(strPort, out intValue))
                            {
                            }
                            sftp.HostPort = intValue;
                        }
                        if (row == 3)
                        {
                            string strRootDir = dr["C011"].ToString();
                            sftp.RootDir = strRootDir;
                        }
                    }
                }

                #endregion


                #region 获取DownloadParam资源

                switch (dbInfo.TypeID)
                {
                    case 2:
                        strSql =
                            string.Format(
                                "SELECT * FROM T_11_101_{0} WHERE C001 > {1} AND C001 < {2} AND C002 = 1 ORDER BY C001, C002",
                                rentToken,
                                DownloadParamInfo.RESOURCE_DOWNLOADPARAM * 10000000000000000,
                                (DownloadParamInfo.RESOURCE_DOWNLOADPARAM + 1) * 10000000000000000);
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
                                  DownloadParamInfo.RESOURCE_DOWNLOADPARAM * 10000000000000000,
                                 (DownloadParamInfo.RESOURCE_DOWNLOADPARAM + 1) * 10000000000000000);
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

                    DownloadParamInfo downloadParam = new DownloadParamInfo();
                    downloadParam.ObjType = DownloadParamInfo.RESOURCE_DOWNLOADPARAM;
                    long objID = Convert.ToInt64(dr["C001"]);
                    downloadParam.ObjID = objID;

                    var temp = listConfigInfos.FirstOrDefault(r => r.ObjID == downloadParam.ObjID);
                    if (temp != null)
                    {
                        listConfigInfos.Remove(temp);
                    }
                    downloadParamCount++;
                    listConfigInfos.Add(downloadParam);
                }

                #endregion


                #region 获取DownloadParam的配置信息

                for (int i = 0; i < listConfigInfos.Count; i++)
                {
                    var resource = listConfigInfos[i];
                    if (resource.ObjType != DownloadParamInfo.RESOURCE_DOWNLOADPARAM) { continue; }
                    var downloadParam = resource as DownloadParamInfo;
                    if (downloadParam == null) { continue; }
                    switch (dbInfo.TypeID)
                    {
                        case 2:
                            strSql =
                                string.Format(
                                    "SELECT * FROM T_11_101_{0} WHERE C001 ={1} AND (C002 = 1 OR C002 = 2 OR C002 = 3 OR C002 = 92) ORDER BY C001, C002",
                                    rentToken,
                                    downloadParam.ObjID);
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
                                    "SELECT * FROM T_11_101_{0} WHERE C001 ={1} AND (C002 = 1 OR C002 = 2 OR C002 = 3 OR C002 = 92) ORDER BY C001, C002",
                                    rentToken,
                                    downloadParam.ObjID);
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
                            downloadParam.Key = key;
                            int id = Convert.ToInt32(dr["C012"]);
                            downloadParam.ID = id;
                        }
                        if (row == 2)
                        {
                            downloadParam.Method = Convert.ToInt32(dr["C011"]);
                            string strAddress = dr["C013"].ToString();
                            strAddress = DecodeEncryptValue(strAddress);
                            downloadParam.Address = strAddress;
                            string strPort = dr["C014"].ToString();
                            strPort = DecodeEncryptValue(strPort);
                            int intPort;
                            if (int.TryParse(strPort, out intPort))
                            {
                                downloadParam.Port = intPort;
                            }
                            downloadParam.RootDir = dr["C015"].ToString();
                            string strServerIP = dr["C016"].ToString();
                            strServerIP = DecodeEncryptValue(strServerIP);
                            downloadParam.ServerIP = strServerIP;
                        }
                        if (row == 3)
                        {
                            downloadParam.VocPathFormat = dr["C011"].ToString();
                            downloadParam.ScrPathFormat = dr["C012"].ToString();
                        }
                        if (row == 92)
                        {
                            downloadParam.AuthName = DecodeEncryptValue(dr["C011"].ToString());
                            downloadParam.AuthPassword = DecodeEncryptValue(dr["C012"].ToString());
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

                    IsaServerInfo isaServer = new IsaServerInfo();
                    isaServer.ObjType = IsaServerInfo.RESOURCE_ISASERVER;
                    long objID = Convert.ToInt64(dr["C001"]);
                    isaServer.ObjID = objID;

                    var temp = listConfigInfos.FirstOrDefault(r => r.ObjID == isaServer.ObjID);
                    if (temp != null)
                    {
                        listConfigInfos.Remove(temp);
                    }
                    isaServerCount++;
                    listConfigInfos.Add(isaServer);
                }

                #endregion


                #region 获取IsaServer的配置信息

                for (int i = 0; i < listConfigInfos.Count; i++)
                {
                    var resource = listConfigInfos[i];
                    if (resource.ObjType != IsaServerInfo.RESOURCE_ISASERVER) { continue; }
                    var isaServer = resource as IsaServerInfo;
                    if (isaServer == null) { continue; }
                    switch (dbInfo.TypeID)
                    {
                        case 2:
                            strSql =
                                string.Format(
                                    "SELECT * FROM T_11_101_{0} WHERE C001 ={1} AND (C002 = 1 OR C002 = 2) ORDER BY C001, C002",
                                    rentToken,
                                    isaServer.ObjID);
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
                                    "SELECT * FROM T_11_101_{0} WHERE C001 ={1} AND (C002 = 1 OR C002 = 2) ORDER BY C001, C002",
                                    rentToken,
                                    isaServer.ObjID);
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
                            isaServer.Key = key;
                            int id = Convert.ToInt32(dr["C012"]);
                            isaServer.ID = id;
                            string strHostAddress = dr["C017"].ToString();
                            strHostAddress = DecodeEncryptValue(strHostAddress);
                            isaServer.HostAddress = strHostAddress;
                            string strHostName = dr["C018"].ToString();
                            strHostName = DecodeEncryptValue(strHostName);
                            isaServer.HostName = strHostName;
                            string strPort = dr["C019"].ToString();
                            strPort = DecodeEncryptValue(strPort);
                            int intValue;
                            if (int.TryParse(strPort, out intValue))
                            {
                            }
                            isaServer.HostPort = intValue;
                        }
                        if (row == 2)
                        {
                            isaServer.AccessToken = dr["C011"].ToString();
                        }
                    }
                }

                #endregion


                #region 获取StorageDevice资源

                switch (dbInfo.TypeID)
                {
                    case 2:
                        strSql =
                            string.Format(
                                "SELECT * FROM T_11_101_{0} WHERE C001 > {1} AND C001 < {2} AND C002 = 1 ORDER BY C001, C002",
                                rentToken,
                                StorageDeviceInfo.RESOURCE_STORAGEDEVICE * 10000000000000000,
                                (StorageDeviceInfo.RESOURCE_STORAGEDEVICE + 1) * 10000000000000000);
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
                                 StorageDeviceInfo.RESOURCE_STORAGEDEVICE * 10000000000000000,
                                 (StorageDeviceInfo.RESOURCE_STORAGEDEVICE + 1) * 10000000000000000);
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

                    StorageDeviceInfo device = new StorageDeviceInfo();
                    device.ObjType = StorageDeviceInfo.RESOURCE_STORAGEDEVICE;
                    long objID = Convert.ToInt64(dr["C001"]);
                    device.ObjID = objID;

                    var temp = listConfigInfos.FirstOrDefault(r => r.ObjID == device.ObjID);
                    if (temp != null)
                    {
                        listConfigInfos.Remove(temp);
                    }
                    deviceCount++;
                    listConfigInfos.Add(device);
                }

                #endregion


                #region 获取StorageDevice的配置信息

                for (int i = 0; i < listConfigInfos.Count; i++)
                {
                    var resource = listConfigInfos[i];
                    if (resource.ObjType != StorageDeviceInfo.RESOURCE_STORAGEDEVICE) { continue; }
                    var device = resource as StorageDeviceInfo;
                    if (device == null) { continue; }
                    switch (dbInfo.TypeID)
                    {
                        case 2:
                            strSql =
                                string.Format(
                                    "SELECT * FROM T_11_101_{0} WHERE C001 ={1} AND (C002 = 1 OR C002 = 2 OR C002 = 92) ORDER BY C001, C002",
                                    rentToken,
                                    device.ObjID);
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
                                    "SELECT * FROM T_11_101_{0} WHERE C001 ={1} AND (C002 = 1 OR C002 = 2 OR C002 = 92) ORDER BY C001, C002",
                                    rentToken,
                                    device.ObjID);
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
                            device.Key = key;
                            int id = Convert.ToInt32(dr["C012"]);
                            device.ID = id;
                        }
                        if (row == 2)
                        {
                            string strDeviceType = dr["C011"].ToString();
                            int intDeviceType;
                            if (int.TryParse(strDeviceType, out intDeviceType))
                            {

                            }
                            device.DeviceType = intDeviceType;
                            device.Address = DecodeEncryptValue(dr["C013"].ToString());
                            device.RootDir = dr["C014"].ToString();
                        }
                        if (row == 92)
                        {
                            device.AuthName = DecodeEncryptValue(dr["C011"].ToString());
                            device.AuthPassword = DecodeEncryptValue(dr["C012"].ToString());
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
                    string.Format(
                        "LoadResourceInfos end.\tSftpServerCount:{0};\tDownloadParamCount:{1};\tIsaServerCount:{2};\tStorageDevice:{3};",
                        sftpServerCount,
                        downloadParamCount,
                        isaServerCount,
                        deviceCount));
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("LoadResourceInfos fail.\t{0}", ex.Message));
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
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("DealConfigChanged fail.\t{0}", ex.Message));
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
                OnDebug(LogMode.Info, string.Format("MediaListener started.\t{0}", mTcpListener.LocalEndpoint));
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
                OnDebug(LogMode.Info, string.Format("MediaLisener stopped."));
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
                OnDebug(LogMode.Error, string.Format("EndAcceptTcpClient fail.\t{0}", ex.Message));
            }
            if (client != null)
            {
                try
                {
                    OnDebug(LogMode.Info, string.Format("New client connected.\t{0}", client.Client.RemoteEndPoint));
                    MediaOperation session = new MediaOperation(client);
                    session.Debug += OnDebug;
                    session.Name = mAppName;
                    session.IsSSL = true;
                    session.ListResourceInfos = mListResourceInfos;
                    session.Certificate = mCertificate;
                    session.RootDir = mRootDir;
                    session.MediaDataDir = mMediaDataDir;
                    session.Start();
                    mListMediaSessions.Add(session);
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
                OnDebug(LogMode.Error, string.Format("BeginAcceptTcpClient fail.\t{0}", ex.Message));
            }
        }

        #endregion


        #region 服务器证书

        private void LoadServerCertificate()
        {
            try
            {
                string path = Path.Combine(mRootDir, "Components\\Certificates\\UMP.SSL.Certificate.pfx");
                if (Program.IsDebug)
                {
                    path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UMP.SSL.Certificate.pfx");
                }
                if (!File.Exists(path))
                {
                    OnDebug(LogMode.Error, string.Format("Server certificate file not exist.\t{0}", path));
                    return;
                }
                X509Certificate2 cert = new X509Certificate2(path, CERT_PASSWORD);
                mCertificate = cert;
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


        #region 检查会话

        private void CreateCheckSessionThread()
        {
            try
            {
                mThreadCheckSession = new Thread(CheckSessionWorker);
                mThreadCheckSession.Start();
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
                    for (int i = mListMediaSessions.Count - 1; i >= 0; i--)
                    {
                        INetSession session = mListMediaSessions[i];
                        DateTime lastTime = session.LastActiveTime;
                        DateTime now = DateTime.Now;
                        //Session超时
                        if ((now - lastTime).TotalSeconds > mSessionTimeout)
                        {
                            OnDebug(LogMode.Info, string.Format("Session timeout.\t{0}", session.RemoteEndpoint));
                            session.Stop();
                            mListMediaSessions.Remove(session);
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
                    OnDebug(LogMode.Info, string.Format("Check clients:{0}", mListMediaSessions.Count));
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
                OnDebug(LogMode.Error, string.Format("CheckSessionWorker fail.\t{0}", ex.Message));
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
                    case Service03Consts.MONITOR_COMMAND_GETRESOURCEOBJECT:
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
            OnDebug(mode, "MediaServer", msg);
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
            WriteLog(mode, "MediaServer", msg);
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
