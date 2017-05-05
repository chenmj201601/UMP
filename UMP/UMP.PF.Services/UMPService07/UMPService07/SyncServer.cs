//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    4d6c8fa8-7918-49d7-b149-844d4840d035
//        CLR Version:              4.0.30319.18063
//        Name:                     SyncServer
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPService07
//        File Name:                SyncServer
//
//        created by Charley at 2015/11/24 11:28:17
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Xml;
using VoiceCyber.Common;
using VoiceCyber.DBAccesses;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.CommonService07;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Encryptions;

/*
 * ==============================================================================
 * 
 * *** SyncServer 工作逻辑 ***
 * 
 * 说明：
 *      SyncServer是一个同步服务，负责定时同步分机，真实分机到UMP系统中
 *      
 * *** 启动 ***
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
 * 3、加载全局参数信息
 * 4、加载资源配置参数（本服务省略具体实现）
 * 5、创建资源配置参数更新处理器
 * 6、启动后台线程
 * （1）重读参数
 * （2）同步分机
 * （3）同步真实分机
 * （4）回删已加密记录（t_21_998）
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
 * *** 同步分机逻辑 ***
 *      同步分机由一个独立的线程负责（mSyncExtensionDataInterval）处理，总的来说是根据资源配置参数中录音录屏的通道配置情况，更新UMP中分机的信息
 * 1、获取所有录音录屏服务器及录音录屏通道信息列表listResourceInfos
 * 2、获取已经存在的分机信息列表listExtInfos
 * 3、遍历所有通道信息列表listResourceInfos，与现有的分机列表listExtInfos比较，如果listExtInfos中不存在，则新创建一个分机并添加入listAddExtensions列表中
 * 4、如果已经存在，则更新分机信息（状态、分机名、所在服务器及通道信息等）并加入listModifyExtensions列表中
 * 5、遍历listExtInfos列表，与通道信息列表比较，如果不存在，表示当前分机被删除了，分机状态标记为已删除，然后加入到listDeleteExtensions列表中
 * 6、将listDeleteExtensions列表中的分机追加到listModifyExtensions列表中
 * 7、将新增分机信息（listAddExtensions）写入数据库（同时增加默认管理权限）
 * 8、将修改的分机信息（listModifyExtensions）写入数据库中
 * 
 * *** 同步真实分机逻辑 ***
 *      同步真实分机由一个独立的线程负责（mSyncExtensionDataInterval）处理，总的来说是根据资源配置参数中PBXDevice的配置情况，更新UMP中真实分机的信息
 * 过程与同步分机的过程基本一样，不同的地方有：
 * 1、来源是PBXDevice信息而不是通道信息
 * 2、没有分机所在的服务器和通道信息
 * 
 * *** 回删已加密记录（T_21_998）***
 *      回删这个部分本不属于本服务的功能，这里只是附带处理一下
 * 处理过程很简单，就是查询T_21_998表的录音记录，如果加密标记C019不为‘E’就把这条记录删掉
 * 查询时每次最多查询500条记录，并间隔10ms
 * 
 * ==============================================================================
 */

namespace UMPService07
{
    /// <summary>
    /// 同步服务，负责定时同步分机，真实分机到UMP系统中
    /// </summary>
    public partial class SyncServer
    {

        #region Members

        private string mAppName = "UMPService07";
        private LogOperator mLogOperator;
        private ConfigInfo mConfigInfo;
        private SessionInfo mSession;
        private string mRootDir;
        private bool mCanDBConnected;

        private List<ResourceConfigInfo> mListResourceInfos;
        private List<GlobalParamInfo> mListGlobalParamInfos;
        private NetPipeHelper mNetPipeHelper;
        private LocalMonitorHelper mLocalMonHelper;
        private ConfigChangeOperator mConfigChangeOperator;

        private Thread mThreadReReadParam;
        private Thread mThreadSyncExtensionData;
        private Thread mThreadSyncRealExtData;
        private Thread mThreadDeleteEncryptionData;

        private int mReReadParamInterval = 30;              //重读参数（数据库，AppServer，ConfigInfo等）时间间隔，单位m
        private int mReConnectDBInterval = 10;              //当数据库连接失败，重连数据库时间间隔，单位s
        private int mSyncExtensionDataInterval = 30;        //同步分机时间间隔，单位s
        private int mSyncRealExtDataInterval = 30;          //同步真实分机时间间隔，单位s
        private int mDeleteEncryptionRecordInteval = 300;    //回删加密记录时间间隔，单位m

        #endregion


        public SyncServer()
        {
            mListResourceInfos = new List<ResourceConfigInfo>();
            mListGlobalParamInfos = new List<GlobalParamInfo>();

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
                OnDebug(LogMode.Info, string.Format("SyncServer starting..."));
                Init();
                LoadGlobalParamInfo();
                LoadResourceInfos();
                CreateConfigChangeOperator();

                //启动线程
                CreateReReadParamThread();
                CreateSyncExtensionThread();
                CreateSyncRealExtThread();
                CreateDeleteEncryptionThread();

                OnDebug(LogMode.Info, string.Format("SyncServer started"));
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("SyncServer start fail.\t{0}", ex.Message));
            }
        }

        public void Stop()
        {
            try
            {
                //停止线程
                StopReReadParamThread();
                StopSyncExtensionThread();
                StopSyncRealExtThread();
                StopDeleteEncryptionThread();

                if (mConfigChangeOperator != null)
                {
                    mConfigChangeOperator.Stop();
                    mConfigChangeOperator = null;
                }
                OnDebug(LogMode.Info, string.Format("SyncServer stopped"));
                if (mLogOperator != null)
                {
                    mLogOperator.Stop();
                    mLogOperator = null;
                }
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("SyncServer stop fail.\t{0}", ex.Message));
            }
        }


        #region Init and Load

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

        private void LoadGlobalParamInfo()
        {
            try
            {
                mListGlobalParamInfos.Clear();
                if (mSession == null
                  || mSession.DatabaseInfo == null)
                {
                    OnDebug(LogMode.Error, string.Format("SessionInfo or DatabaseInfo is null"));
                    return;
                }
                DatabaseInfo dbInfo = mSession.DatabaseInfo;
                string strConn = dbInfo.GetConnectionString();
                int dbType = dbInfo.TypeID;
                string rentToken = mSession.RentInfo.Token;
                long rentID = mSession.RentInfo.ID;
                string strTemp;
                string strSql;
                DataSet objDataSet;
                OperationReturn optReturn;

                switch (dbType)
                {
                    case 2:
                        strSql = string.Format("SELECT * FROM T_11_001_{0} WHERE C001 = {1} AND C003 = {2}", rentToken,
                            rentID, Service07Consts.GP_DEFULT_PASSWORD);
                        optReturn = MssqlOperation.GetDataSet(strConn, strSql);
                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM T_11_001_{0} WHERE C001 = {1} AND C003 = {2}", rentToken,
                           rentID, Service07Consts.GP_DEFULT_PASSWORD);
                        optReturn = OracleOperation.GetDataSet(strConn, strSql);
                        break;
                    default:
                        OnDebug(LogMode.Error, string.Format("Database type invalid.\t{0}", dbType));
                        return;
                }
                if (!optReturn.Result)
                {
                    OnDebug(LogMode.Error,
                        string.Format("LoadGlobalParamInfo fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                objDataSet = optReturn.Data as DataSet;
                if (objDataSet == null)
                {
                    OnDebug(LogMode.Error,
                       string.Format("LoadGlobalParamInfo fail.\tDataSet is null"));
                    return;
                }
                if (objDataSet.Tables[0].Rows.Count <= 0)
                {
                    OnDebug(LogMode.Error,
                     string.Format("LoadGlobalParamInfo fail.\tDefaultPassword param not exist."));
                    return;
                }
                DataRow dr = objDataSet.Tables[0].Rows[0];
                GlobalParamInfo info = new GlobalParamInfo();
                info.ModuleID = Convert.ToInt32(dr["C002"]);
                info.ParamID = Convert.ToInt32(dr["C003"]);
                info.GroupID = Convert.ToInt32(dr["C004"]);
                info.SortID = Convert.ToInt32(dr["C005"]);
                string strValue = dr["C006"].ToString();
                strTemp = strValue;
                strValue = DecryptFromDB(strValue);
                if (strValue.Length >= Service07Consts.GP_DEFULT_PASSWORD.ToString().Length)
                {
                    strValue = strValue.Substring(Service07Consts.GP_DEFULT_PASSWORD.ToString().Length);
                }
                info.ParamValue = strValue;
                mListGlobalParamInfos.Add(info);

                OnDebug(LogMode.Info, string.Format("LoadGlobalParamInfo \tParamID:{0};ParamValue:{1}", info.ParamID, strTemp));
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("LoadGlobalParamInfo fail.\t{0}", ex.Message));
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
                    mConfigInfo.ListSettings.FirstOrDefault(s => s.Key == Service07Consts.GS_KEY_INTERVAL_SYNCEXTENSION);
                if (setting != null)
                {
                    strValue = setting.Value;
                    if (int.TryParse(strValue, out intValue)
                        && intValue >= 10
                        && intValue <= 60 * 60)
                    {
                        mSyncExtensionDataInterval = intValue;
                        strLog += string.Format("{0}:{1};", Service07Consts.GS_KEY_INTERVAL_SYNCEXTENSION, intValue);
                    }
                }
                setting =
                    mConfigInfo.ListSettings.FirstOrDefault(s => s.Key == Service07Consts.GS_KEY_INTERVAL_SYNCREALEXT);
                if (setting != null)
                {
                    strValue = setting.Value;
                    if (int.TryParse(strValue, out intValue)
                        && intValue >= 10
                        && intValue <= 60 * 60)
                    {
                        mSyncExtensionDataInterval = intValue;
                        strLog += string.Format("{0}:{1};", Service07Consts.GS_KEY_INTERVAL_SYNCREALEXT, intValue);
                    }
                }
                setting =
                   mConfigInfo.ListSettings.FirstOrDefault(s => s.Key == Service07Consts.GS_KEY_INTERVAL_DELETEENCRYPTION);
                if (setting != null)
                {
                    strValue = setting.Value;
                    if (int.TryParse(strValue, out intValue)
                        && intValue >= 1
                        && intValue <= 60 * 24)
                    {
                        mSyncExtensionDataInterval = intValue;
                        strLog += string.Format("{0}:{1};", Service07Consts.GS_KEY_INTERVAL_DELETEENCRYPTION, intValue);
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
                        //重新加载全局参数和配置参数
                        LoadGlobalParamInfo();
                        LoadResourceInfos();
                    }
                }
            }
            catch (Exception ex)
            {
                var abort = ex as ThreadAbortException;
                if (abort == null)
                {
                    OnDebug(LogMode.Error, string.Format("ReReadParamWorker fail.\t{0}", ex.Message));
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


        #region 资源配置信息

        private void LoadResourceInfos()
        {
            try
            {
                //if (mSession == null
                //    || mSession.AppServerInfo == null
                //    || mSession.DatabaseInfo == null)
                //{
                //    OnDebug(LogMode.Error, string.Format("AppServerInfo or DatabaseInfo is null"));
                //    return;
                //}
                //DatabaseInfo dbInfo = mSession.DatabaseInfo;
                //string strConn = mSession.DBConnectionString;

                //OperationReturn optReturn;
                //string rentToken = "00000";
                //string strSql;
                //DataSet objDataSet;

                //List<ResourceConfigInfo> listConfigInfos = new List<ResourceConfigInfo>();

                //一下加载需要的资源信息

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
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("DealConfigChanged fail.\t{0}", ex.Message));
            }
        }

        #endregion


        #region 同步分机数据

      

        #endregion


        #region 同步真实分机数据

       

        #endregion


        #region 回删加密记录（t_21_998)

        private void CreateDeleteEncryptionThread()
        {
            try
            {
                if (mThreadDeleteEncryptionData != null)
                {
                    try
                    {
                        mThreadDeleteEncryptionData.Abort();
                    }
                    catch { }
                    mThreadDeleteEncryptionData = null;
                }
                mThreadDeleteEncryptionData = new Thread(DeleteEncryptionRecordWorker);
                mThreadDeleteEncryptionData.Start();
                OnDebug(LogMode.Info,
                    string.Format("DeleteEncryptionThread started.\t{0}", mThreadDeleteEncryptionData.ManagedThreadId));
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("CreateDeleteEncryptionThread fail.\t{0}", ex.Message));
            }
        }

        private void StopDeleteEncryptionThread()
        {
            try
            {
                if (mThreadDeleteEncryptionData != null)
                {
                    try
                    {
                        mThreadDeleteEncryptionData.Abort();
                    }
                    catch { }
                    mThreadDeleteEncryptionData = null;
                    OnDebug(LogMode.Info, string.Format("DeleteEncryptionThread stopped"));
                }
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("StopDeleteEncryptionThread fail.\t{0}", ex.Message));
            }
        }

        private void DeleteEncryptionRecordWorker()
        {
            try
            {
                while (true)
                {
                    DoDeleteEncryptionRecord();

                    Thread.Sleep(mDeleteEncryptionRecordInteval * 1000 * 60);
                }
            }
            catch (Exception ex)
            {
                var abort = ex as ThreadAbortException;
                if (abort == null)
                {
                    OnDebug(LogMode.Error, string.Format("DeleteEncryptionRecordWorker fail.\t{0}", ex.Message));
                }
            }
        }

        private void DoDeleteEncryptionRecord()
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
                string strSql;
                DataSet objDataSet;
                IDbConnection objConn;
                IDbDataAdapter objAdapter;
                DbCommandBuilder objCmdBuilder;

                while (true)
                {
                    switch (dbType)
                    {
                        case 2:
                            strSql = string.Format("SELECT TOP 500 * FROM T_21_998 WHERE C019 <> 'E' ORDER BY C001");
                            objConn = MssqlOperation.GetConnection(strConn);
                            objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                            objCmdBuilder = MssqlOperation.GetCommandBuilder(objAdapter);
                            break;
                        case 3:
                            strSql = string.Format("SELECT * FROM T_21_998 WHERE C019 <> 'E' AND ROWNUM <= 500 ORDER BY C001");
                            objConn = OracleOperation.GetConnection(strConn);
                            objAdapter = OracleOperation.GetDataAdapter(objConn, strSql);
                            objCmdBuilder = OracleOperation.GetCommandBuilder(objAdapter);
                            break;
                        default:
                            OnDebug(LogMode.Error, string.Format("Database type invalid.\t{0}", dbType));
                            return;
                    }
                    if (objConn == null || objAdapter == null || objCmdBuilder == null)
                    {
                        OnDebug(LogMode.Error, string.Format("ObjDataAdapter is null"));
                        return;
                    }
                    objCmdBuilder.ConflictOption = ConflictOption.OverwriteChanges;
                    objCmdBuilder.SetAllValues = false;
                    try
                    {
                        objDataSet = new DataSet();
                        objAdapter.Fill(objDataSet);

                        int count = objDataSet.Tables[0].Rows.Count;
                        if (count <= 0)
                        {
                            return;
                        }
                        OnDebug(LogMode.Debug, string.Format("Begin delete encrypted record (21_998).\t{0}", count));
                        for (int i = count - 1; i >= 0; i--)
                        {
                            objDataSet.Tables[0].Rows[i].Delete();
                        }

                        objAdapter.Update(objDataSet);
                        objDataSet.AcceptChanges();
                    }
                    catch (Exception ex)
                    {
                        OnDebug(LogMode.Error, string.Format("DoDeleteEncryptionRecord fail.\t{0}", ex.Message));
                        return;
                    }
                    finally
                    {
                        if (objConn.State == ConnectionState.Open)
                        {
                            objConn.Close();
                        }
                        objConn.Dispose();
                    }
                    Thread.Sleep(10);
                }
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("DoDeleteEncryptionRecord fail.\t{0}", ex.Message));
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

        private OperationReturn GetSerialID(int moduleID, int resourceType, bool hasDatetime)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                if (mSession == null
                    || mSession.DatabaseInfo == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("SessionInfo or DatabaseInfo is null");
                    return optReturn;
                }
                DatabaseInfo dbInfo = mSession.DatabaseInfo;
                int dbType = dbInfo.TypeID;
                string strConn = dbInfo.GetConnectionString();
                string rentToken = mSession.RentInfo.Token;
                string strNow = DateTime.Now.ToString("yyyyMMddHHmmss");
                long errNumber = 0;
                string strErrMsg = string.Empty;
                string strSerialID = string.Empty;
                switch (dbType)
                {
                    case 2:
                        DbParameter[] mssqlParameters =
                        {
                            MssqlOperation.GetDbParameter("@AInParam01",MssqlDataType.Varchar,2),
                            MssqlOperation.GetDbParameter("@AInParam02",MssqlDataType.Varchar,3),
                            MssqlOperation.GetDbParameter("@AInParam03",MssqlDataType.Varchar,5),
                            MssqlOperation.GetDbParameter("@Ainparam04",MssqlDataType.Varchar,20),
                            MssqlOperation.GetDbParameter("@AOutParam01",MssqlDataType.Varchar,20),
                            MssqlOperation.GetDbParameter("@AOutErrorNumber",MssqlDataType.Bigint,0),
                            MssqlOperation.GetDbParameter("@AOutErrorString",MssqlDataType.NVarchar,4000)
                        };
                        mssqlParameters[0].Value = moduleID;
                        mssqlParameters[1].Value = resourceType;
                        mssqlParameters[2].Value = rentToken;
                        mssqlParameters[3].Value = hasDatetime ? strNow : string.Empty;
                        mssqlParameters[4].Value = strSerialID;
                        mssqlParameters[5].Value = errNumber;
                        mssqlParameters[6].Value = strErrMsg;
                        mssqlParameters[4].Direction = ParameterDirection.Output;
                        mssqlParameters[5].Direction = ParameterDirection.Output;
                        mssqlParameters[6].Direction = ParameterDirection.Output;
                        optReturn = MssqlOperation.ExecuteStoredProcedure(strConn, "P_00_001",
                           mssqlParameters);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        if (mssqlParameters[5].Value.ToString() != "0")
                        {
                            optReturn.Result = false;
                            optReturn.Code = Defines.RET_DBACCESS_FAIL;
                            optReturn.Message = string.Format("{0}\t{1}", mssqlParameters[5].Value, mssqlParameters[6].Value);
                        }
                        else
                        {
                            strSerialID = mssqlParameters[4].Value.ToString();
                            optReturn.Data = strSerialID;
                        }
                        break;
                    case 3:
                        DbParameter[] orclParameters =
                        {
                            OracleOperation.GetDbParameter("AInParam01",OracleDataType.Varchar2,2),
                            OracleOperation.GetDbParameter("AInParam02",OracleDataType.Varchar2,3),
                            OracleOperation.GetDbParameter("AInParam03",OracleDataType.Varchar2,5),
                            OracleOperation.GetDbParameter("Ainparam04",OracleDataType.Varchar2,20),
                            OracleOperation.GetDbParameter("AOutParam01",OracleDataType.Varchar2,20),
                            OracleOperation.GetDbParameter("AOutErrorNumber",OracleDataType.Int32,0),
                            OracleOperation.GetDbParameter("AOutErrorString",OracleDataType.Nvarchar2,4000)
                        };
                        orclParameters[0].Value = moduleID;
                        orclParameters[1].Value = resourceType;
                        orclParameters[2].Value = rentToken;
                        orclParameters[3].Value = hasDatetime ? strNow : string.Empty;
                        orclParameters[4].Value = strSerialID;
                        orclParameters[5].Value = errNumber;
                        orclParameters[6].Value = strErrMsg;
                        orclParameters[4].Direction = ParameterDirection.Output;
                        orclParameters[5].Direction = ParameterDirection.Output;
                        orclParameters[6].Direction = ParameterDirection.Output;
                        optReturn = OracleOperation.ExecuteStoredProcedure(strConn, "P_00_001",
                           orclParameters);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        if (orclParameters[5].Value.ToString() != "0")
                        {
                            optReturn.Result = false;
                            optReturn.Code = Defines.RET_DBACCESS_FAIL;
                            optReturn.Message = string.Format("{0}\t{1}", orclParameters[5].Value, orclParameters[6].Value);
                        }
                        else
                        {
                            strSerialID = orclParameters[4].Value.ToString();
                            optReturn.Data = strSerialID;
                        }
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("Database type not surpport.\t{0}", dbType);
                        return optReturn;
                }
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
            OnDebug(mode, "SyncServer", msg);
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
                    case Service07Consts.MONITOR_COMMAND_GETRESOURCEOBJECT:
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
            WriteLog(mode, "SyncServer", msg);
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

        public string EncryptToDB(string strSource)
        {
            try
            {
                return ServerAESEncryption.EncryptString(strSource, EncryptionMode.AES256V02Hex);
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("EncryptToDB fail.\t{0}", ex.Message));
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
                OnDebug(LogMode.Error, string.Format("DecryptFromDB fail.\t{0}", ex.Message));
                return strSource;
            }
        }

        public string EncryptSHA512(string strSource)
        {
            try
            {
                return ServerHashEncryption.EncryptString(strSource, EncryptionMode.SHA512V00Hex);
            }
            catch (Exception ex)
            {
                OnDebug(LogMode.Error, string.Format("EncryptSHA512 fail.\t{0}", ex.Message));
                return strSource;
            }
        }

        #endregion

    }
}
