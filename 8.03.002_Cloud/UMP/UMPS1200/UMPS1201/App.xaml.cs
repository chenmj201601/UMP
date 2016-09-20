using System;
using System.Collections.Generic;
using System.Deployment.Application;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Xml;
using UMPS1201.Wcf11012;
using UMPS1201.Wcf12001;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common12001;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Encryptions;

namespace UMPS1201
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App
    {

        #region Members

        public static string AppName = "UMPS1201";
        public static string AppTitle = "UMP";
        public static int ModuleID = 1201;
        public static int AppType = (int)VoiceCyber.UMP.Common.AppType.UMPClient;
        public static SessionInfo Session;
        public static NetPipeHelper NetPipeHelper;
        public static LocalMonitorHelper MonitorHelper;
        public static AppConfigs AppConfigs;
        public static List<LanguageInfo> ListLanguageInfos;
        public static List<LanguageInfo> ListLanguageInfosXml;
        public static bool IsDBSetted;
        public static bool IsProtocolValid;

        private static LogOperator mLogOperator;
        private Boot mBoot;
        private Thread mGCCollectThread;
        private int mGCCollectInterval = 5;//定时清理资源的频率，单位 s

        #endregion


        protected override void OnStartup(StartupEventArgs e)
        {
            try
            {
                //设置标题
                var window = Current.MainWindow;
                if (window != null)
                {
                    window.Title = ConstValue.UMP_PRODUCTER_LONGNAME;
                }

                ListLanguageInfos = new List<LanguageInfo>();
                ListLanguageInfosXml = new List<LanguageInfo>();
                MonitorHelper = new LocalMonitorHelper(false);
                IsDBSetted = false;

                CreateLogOperator();
                WriteLog("AppLoad", string.Format("App starting..."));

                InitSessionInfo();
                GetSettedDefaultLang();
                LoadSessionInfo();
                ParseAppServerInfo();
                CheckWebProtocol();
                LoadDatabaseInfo();

                if (Session != null)
                {
                    WriteLog("AppLoad", string.Format("SessionInfo:{0}", Session.LogInfo()));
                }

                LoadAppConfigs();
                CreateNetPipeService();
                InitLanguages();
                InitLanguagesXml();

                mBoot = new Boot();
                mBoot.Run();
                WriteLog("AppLoad", string.Format("Boot run end"));

                //CreateGCCollectThread();

                WriteLog("AppLoad", string.Format("App started"));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, AppTitle, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            StopGCCollectThread();
            if (mBoot != null)
            {
                mBoot.Close();
            }
            if (NetPipeHelper != null)
            {
                NetPipeHelper.Stop();
                NetPipeHelper = null;
            }
            TempDataRecylce();      //程序退出的时候回删临时文件
            WriteLog("AppExit", string.Format("App ended."));
            if (mLogOperator != null)
            {
                mLogOperator.Stop();
                mLogOperator = null;
            }
            base.OnExit(e);
        }


        #region Init and Load

        private void InitSessionInfo()
        {
            Session = SessionInfo.CreateSessionInfo(AppName, ModuleID, AppType);
            RentInfo rentInfo = new RentInfo();
            rentInfo.ID = ConstValue.RENT_DEFAULT;
            rentInfo.Token = ConstValue.RENT_DEFAULT_TOKEN;
            rentInfo.Domain = "voicecyber.com";
            rentInfo.Name = "voicecyber";
            Session.RentInfo = rentInfo;
            Session.RentID = ConstValue.RENT_DEFAULT;

            UserInfo userInfo = new UserInfo();
            userInfo.UserID = ConstValue.USER_ADMIN;
            userInfo.Account = "administrator";
            userInfo.UserName = "Administrator";
            Session.UserInfo = userInfo;
            Session.UserID = ConstValue.USER_ADMIN;

            RoleInfo roleInfo = new RoleInfo();
            roleInfo.ID = ConstValue.ROLE_SYSTEMADMIN;
            roleInfo.Name = "System Admin";
            Session.RoleInfo = roleInfo;
            Session.RoleID = ConstValue.ROLE_SYSTEMADMIN;


            ThemeInfo themeInfo = new ThemeInfo();
            themeInfo.Name = "Style01";
            themeInfo.Color = "Green";
            Session.SupportThemes.Add(themeInfo);

            themeInfo = new ThemeInfo();
            themeInfo.Name = "Style02";
            themeInfo.Color = "Yellow";
            Session.SupportThemes.Add(themeInfo);

            themeInfo = new ThemeInfo();
            themeInfo.Name = "Style03";
            themeInfo.Color = "Brown";
            Session.SupportThemes.Add(themeInfo);

            themeInfo = new ThemeInfo();
            themeInfo.Name = "Style04";
            themeInfo.Color = "Blue";
            Session.SupportThemes.Add(themeInfo);


            #region Set Default Theme

            string themeName = "Style01";
            DateTime now = DateTime.Now;
            if (now.Month == 3
                || now.Month == 4
                || now.Month == 5)
            {
                themeName = "Style01";
            }
            if (now.Month == 6
                || now.Month == 7
                || now.Month == 8)
            {
                themeName = "Style02";
            }
            if (now.Month == 9
                || now.Month == 10
                || now.Month == 11)
            {
                themeName = "Style03";
            }
            if (now.Month == 12
               || now.Month == 1
               || now.Month == 2)
            {
                themeName = "Style04";
            }
            var theme = Session.SupportThemes.FirstOrDefault(t => t.Name == themeName);
            if (theme != null)
            {
                Session.ThemeInfo = theme;
                Session.ThemeName = themeName;
            }

            #endregion


            LangTypeInfo langType = new LangTypeInfo();
            langType.LangID = 1033;
            langType.LangName = "en-us";
            langType.Display = "English";
            Session.SupportLangTypes.Add(langType);

            langType = new LangTypeInfo();
            langType.LangID = 2052;
            langType.LangName = "zh-cn";
            langType.Display = "简体中文";
            Session.SupportLangTypes.Add(langType);
            Session.LangTypeInfo = langType;
            Session.LangTypeID = langType.LangID;

            langType = new LangTypeInfo();
            langType.LangID = 1028;
            langType.LangName = "zh-cn";
            langType.Display = "繁体中文";
            Session.SupportLangTypes.Add(langType);

            langType = new LangTypeInfo();
            langType.LangID = 1041;
            langType.LangName = "jp";
            langType.Display = "日本语";
            Session.SupportLangTypes.Add(langType);


            #region Set Default Language

            int langID = GetUserDefaultUILanguage();
            var lang = Session.SupportLangTypes.FirstOrDefault(l => l.LangID == langID);
            if (lang != null)
            {
                Session.LangTypeInfo = lang;
                Session.LangTypeID = lang.LangID;
            }

            #endregion


            AppServerInfo appServerInfo = new AppServerInfo();
            appServerInfo.Protocol = "http";
            appServerInfo.Address = "192.168.6.74";
            appServerInfo.Port = 8081;
            appServerInfo.SupportHttps = false;
            appServerInfo.SupportNetTcp = false;
            Session.AppServerInfo = appServerInfo;

            DatabaseInfo dbInfo = new DatabaseInfo();
            dbInfo.TypeID = 2;
            dbInfo.Host = "192.168.4.182";
            dbInfo.Port = 1433;
            dbInfo.DBName = "UMPDataDB0826";
            dbInfo.LoginName = "PFDEV";
            Session.DatabaseInfo = dbInfo;
            Session.DBType = dbInfo.TypeID;

        }

        private void LoadSessionInfo()
        {
            try
            {
                string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                    ConstValue.TEMP_DIR_UMP, AppName, ConstValue.TEMP_FILE_UMPSESSION);
                if (!File.Exists(path))
                {
                    WriteLog("AppLoad", string.Format("umpsession.xml not exist.\t{0}", path));
                    return;
                }
                OperationReturn optReturn = XMLHelper.DeserializeFile<SessionInfo>(path);
                if (!optReturn.Result)
                {
                    WriteLog("AppLoad",
                        string.Format("Load SessionInfo from xml file fail.\t{0}\t{1}", optReturn.Code,
                            optReturn.Message));
                    return;
                }
                SessionInfo session = optReturn.Data as SessionInfo;
                if (session == null)
                {
                    WriteLog("AppLoad",
                       string.Format("Load SessionInfo from xml file fail.\tSessionInfo is null"));
                    return;
                }
                Session.SetSessionInfo(session);
                Session.IsMonitor = session.IsMonitor;
                WriteLog("AppLoad",
                      string.Format("Load SessionInfo from xml file end."));
            }
            catch (Exception ex)
            {
                WriteLog("AppLoad", string.Format("Load SessionInfo from xml file fail.\t{0}", ex.Message));
            }
        }

        private void ParseAppServerInfo()
        {
            try
            {
                if (ApplicationDeployment.IsNetworkDeployed)
                {
                    var deploy = ApplicationDeployment.CurrentDeployment;
                    if (deploy != null)
                    {
                        var version = deploy.CurrentVersion;
                        if (version != null)
                        {
                            string strVersion = string.Format("{0}.{1}.{2}", version.Major.ToString("0"),
                                version.Minor.ToString("00"), version.Build.ToString("000"));
                            Session.AppVersion = strVersion;
                            WriteLog("AppServerInfo",
                                  string.Format("Version \t{0}", strVersion));
                        }
                        var uri = deploy.ActivationUri;
                        if (uri != null)
                        {
                            var appServerInfo = Session.AppServerInfo;
                            if (appServerInfo != null)
                            {
                                appServerInfo.Protocol = uri.Scheme;
                                appServerInfo.Address = uri.Host;
                                appServerInfo.Port = uri.Port;
                                appServerInfo.SupportHttps = uri.Scheme == "https";
                                WriteLog("AppServerInfo",
                                    string.Format("AppServerInfo \t{0}://{1}:{2}", appServerInfo.Protocol, appServerInfo.Address, appServerInfo.Port));
                            }
                            Session.AppServerInfo = appServerInfo;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLog("AppServerInfo", string.Format("ParseAppServerInfo fail.\t{0}", ex.Message));
            }
        }

        private void CheckWebProtocol()
        {
            try
            {
                IsProtocolValid = false;
                string strPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                strPath = Path.Combine(strPath, @"UMP\UMPClient\UMP.Server.01.XML");
                if (!File.Exists(strPath))
                {
                    WriteLog("CheckWebProtocol", string.Format("File not exist.\t{0}", strPath));
                    return;
                }
                XmlDocument doc = new XmlDocument();
                doc.Load(strPath);
                var node = doc.SelectSingleNode("UMPSetted/IISBindingProtocol");
                if (node == null)
                {
                    WriteLog("CheckWebProtocol", string.Format("IISBindingProtocol node not exist."));
                    return;
                }
                var protocol = Session.AppServerInfo.Protocol.ToLower();
                for (int i = 0; i < node.ChildNodes.Count; i++)
                {
                    var ele = node.ChildNodes[i] as XmlElement;
                    if (ele == null) { continue; }
                    string strKey = ele.Attributes["Protocol"].Value;
                    if (strKey.ToLower() == protocol)
                    {
                        string strActived = ele.Attributes["Activated"].Value;
                        if (strActived == "1")
                        {
                            WriteLog("CheckWebProtocol", string.Format("Web protocol valid.\t{0}", protocol));
                            IsProtocolValid = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLog("CheckWebProtocol", string.Format("CheckWebProtocol fail.\t{0}", ex.Message));
            }
        }

        private void LoadDatabaseInfo()
        {
            try
            {
                if (Session == null || Session.AppServerInfo == null) { return; }
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)RequestCode.WSGetDBInfo;
                webRequest.Session = Session;
                webRequest.ListData.Add(Session.UserID.ToString());
                Service11012Client client = new Service11012Client(
                    WebHelper.CreateBasicHttpBinding(Session)
                    , WebHelper.CreateEndpointAddress(Session.AppServerInfo, "Service11012"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    WriteLog("LoadDBInfo", string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null
                    || webReturn.ListData.Count < 6)
                {
                    WriteLog("LoadDBInfo", string.Format("Fail.\tDatabase param count invalid"));
                    return;
                }

                DatabaseInfo dbInfo = Session.DatabaseInfo;
                if (dbInfo == null)
                {
                    dbInfo = new DatabaseInfo();
                }
                int intValue;
                string strValue;
                if (webReturn.ListData.Count > 0)
                {
                    strValue = webReturn.ListData[0];
                    if (int.TryParse(strValue, out intValue))
                    {
                        dbInfo.TypeID = intValue;
                    }
                }
                if (webReturn.ListData.Count > 1)
                {
                    strValue = webReturn.ListData[1];
                    dbInfo.Host = strValue;
                }
                if (webReturn.ListData.Count > 2)
                {
                    strValue = webReturn.ListData[2];
                    if (int.TryParse(strValue, out intValue))
                    {
                        dbInfo.Port = intValue;
                    }
                }
                if (webReturn.ListData.Count > 3)
                {
                    strValue = webReturn.ListData[3];
                    dbInfo.DBName = strValue;
                }
                if (webReturn.ListData.Count > 4)
                {
                    strValue = webReturn.ListData[4];
                    dbInfo.LoginName = strValue;
                }
                if (webReturn.ListData.Count > 5)
                {
                    strValue = webReturn.ListData[5];
                    dbInfo.Password = strValue;
                }
                Session.DatabaseInfo = dbInfo;
                Session.DBType = dbInfo.TypeID;
                IsDBSetted = true;

                WriteLog("LoadDBInfo", string.Format("LoadDatabaseInfo end.\t{0}", dbInfo));
            }
            catch (Exception ex)
            {
                WriteLog("LoadDBInfo", string.Format("LoadDatabaseInfo fail.\t{0}", ex.Message));
            }
        }

        private void LoadAppConfigs()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = Session;
                webRequest.Code = (int)S1200Codes.GetAppConfigList;
                Service12001Client client = new Service12001Client(
                  WebHelper.CreateBasicHttpBinding(Session)
                  , WebHelper.CreateEndpointAddress(Session.AppServerInfo, "Service12001"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    WriteLog("LoadAppConfigs", string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    WriteLog("LoadAppConfigs", string.Format("ListData is null"));
                    return;
                }
                int count = webReturn.ListData.Count;
                if (AppConfigs == null)
                {
                    AppConfigs = new AppConfigs();
                }
                AppConfigs.ListApps.Clear();
                OperationReturn optReturn;
                for (int i = 0; i < count; i++)
                {
                    string strInfo = webReturn.ListData[i];
                    optReturn = XMLHelper.DeserializeObject<AppConfigInfo>(strInfo);
                    if (!optReturn.Result)
                    {
                        WriteLog("LoadAppConfigs", string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        continue;
                    }
                    AppConfigInfo info = optReturn.Data as AppConfigInfo;
                    if (info == null)
                    {
                        WriteLog("LoadAppConfigs", string.Format("AppConfigInfo is null"));
                        continue;
                    }
                    AppConfigs.ListApps.Add(info);
                }

                WriteLog("LoadAppConfigs", string.Format("Load end.\tCount:{0}", count));
            }
            catch (Exception ex)
            {
                WriteLog("LoadAppConfigs", string.Format("Fail.\t{0}", ex.Message));
            }
        }

        private void GetSettedDefaultLang()
        {
            try
            {
                string path = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                path = Path.Combine(path, "UMP\\UMPClient");
                path = Path.Combine(path, "UMP.Setted.xml");
                if (!File.Exists(path))
                {
                    WriteLog("GetSettedLang", string.Format("File not exist.\t{0}", path));
                    return;
                }
                XmlDocument doc = new XmlDocument();
                doc.Load(path);
                var setNode = doc.SelectSingleNode("UserSetted");
                if (setNode == null)
                {
                    WriteLog("GetSettedLang", string.Format("UMPSetted node not exist"));
                    return;
                }
                var ele = setNode as XmlElement;
                if (ele == null) { return; }
                string strValue = ele.GetAttribute("LangID");
                int intValue;
                if (!int.TryParse(strValue, out intValue))
                {
                    WriteLog("GetSettedLang", string.Format("LanguageID invalid.\t{0}", strValue));
                    return;
                }
                var lang = Session.SupportLangTypes.FirstOrDefault(l => l.LangID == intValue);
                if (lang == null)
                {
                    WriteLog("GetSettedLang", string.Format("Language not supported.\t{0}", intValue));
                    return;
                }
                Session.LangTypeInfo = lang;
                Session.LangTypeID = lang.LangID;

                WriteLog("GetSettedLang", string.Format("Get setted default langguage end.\t{0}", lang.LangID));
            }
            catch (Exception ex)
            {
                WriteLog("GetSettedLang", string.Format("Fail.\t{0}", ex.Message));
            }
        }

        #endregion


        #region Language

        private void InitLanguages()
        {
            try
            {
                ListLanguageInfos.Clear();
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)RequestCode.WSGetLangList;
                webRequest.Session = Session;
                webRequest.ListData.Add(Session.LangTypeInfo.LangID.ToString());
                webRequest.ListData.Add(string.Empty);
                webRequest.ListData.Add("0");
                webRequest.ListData.Add("0");
                webRequest.ListData.Add(string.Empty);
                webRequest.ListData.Add(string.Empty);
                Service11012Client client = new Service11012Client(
                    WebHelper.CreateBasicHttpBinding(Session)
                    , WebHelper.CreateEndpointAddress(Session.AppServerInfo, "Service11012"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    WriteLog("InitLang", string.Format("{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    WriteLog("InitLang", "ListData is null");
                    return;
                }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<LanguageInfo>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        WriteLog("InitLang", string.Format("{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    LanguageInfo langInfo = optReturn.Data as LanguageInfo;
                    if (langInfo == null)
                    {
                        WriteLog("InitLang", string.Format("LanguageInfo is null"));
                        return;
                    }
                    ListLanguageInfos.Add(langInfo);
                }
            }
            catch (Exception ex)
            {
                WriteLog("InitLang", string.Format("Fail.\t{0}", ex.Message));
            }
        }

        public static void InitAllLanguages()
        {
            var app = Current as App;
            if (app != null)
            {
                app.InitLanguages();
            }
        }

        public static string GetLanguageInfo(string name, string display)
        {
            try
            {
                LanguageInfo lang =
                    ListLanguageInfos.FirstOrDefault(l => l.LangID == Session.LangTypeInfo.LangID && l.Name == name);
                if (lang == null)
                {
                    return display;
                }
                return lang.Display;
            }
            catch (Exception ex)
            {
                WriteLog("GetLang", string.Format("GetLang fail.\t{0}", ex.Message));
                return display;
            }
        }

        private void InitLanguagesXml()
        {
            try
            {
                ListLanguageInfosXml.Clear();
                WebRequest webRequest = new WebRequest();
                webRequest.Session = Session;
                webRequest.Code = (int)S1200Codes.GetLanguageInfoListXml;
                webRequest.ListData.Add(Session.UserID.ToString());
                webRequest.ListData.Add(Session.LangTypeID.ToString());
                Service12001Client client = new Service12001Client(WebHelper.CreateBasicHttpBinding(Session),
                    WebHelper.CreateEndpointAddress(Session.AppServerInfo, "Service12001"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    WriteLog("InitLang", string.Format("{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    WriteLog("InitLang", "ListData is null");
                    return;
                }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<LanguageInfo>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        WriteLog("InitLang", string.Format("{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    LanguageInfo langInfo = optReturn.Data as LanguageInfo;
                    if (langInfo == null)
                    {
                        WriteLog("InitLang", string.Format("LanguageInfo is null"));
                        return;
                    }
                    ListLanguageInfosXml.Add(langInfo);
                }
            }
            catch (Exception ex)
            {
                WriteLog("InitLang", string.Format("Fail.\t{0}", ex.Message));
            }
        }

        public static void InitAllLanguagesXml()
        {
            var app = Current as App;
            if (app != null)
            {
                app.InitLanguagesXml();
            }
        }

        public static string GetLanguageInfoXml(string name, string display)
        {
            try
            {
                LanguageInfo lang =
                    ListLanguageInfosXml.FirstOrDefault(l => l.LangID == Session.LangTypeInfo.LangID && l.Name == name);
                if (lang == null)
                {
                    return display;
                }
                return lang.Display;
            }
            catch (Exception ex)
            {
                WriteLog("GetLang", string.Format("GetLang fail.\t{0}", ex.Message));
                return display;
            }
        }

        #endregion


        #region NetPipeHelper

        private static void CreateNetPipeService()
        {
            NetPipeHelper = new NetPipeHelper(true, Session.SessionID);
            NetPipeHelper.DealMessageFunc += mNetPipeHelper_DealMessageFunc;
            try
            {
                NetPipeHelper.Start();
                WriteLog("NetPipe", string.Format("NetPipe service created.\t{0}", Session.SessionID));
            }
            catch (Exception ex)
            {
                WriteLog("NetPipe", string.Format("Start service fail.\t{0}", ex.Message));
            }
        }

        private static WebReturn mNetPipeHelper_DealMessageFunc(WebRequest arg)
        {
            WebReturn webReturn = new WebReturn();
            webReturn.Session = Session;
            webReturn.Result = true;
            webReturn.Code = 0;
            try
            {
                var code = arg.Code;
                var strData = arg.Data;
                WriteLog("NetPipe", string.Format("RecieveMessage\t{0}\t{1}", code, strData));
                //switch (code)
                //{
                //    //主模块修改当前登录用户的密码时，要同步修改子模块的用户密码
                //    case (int)RequestCode.SCChangePassword:
                //        string strNewPassword = strData;
                //        if (Session != null
                //            && Session.UserInfo != null)
                //        {
                //            Session.UserInfo.Password = strNewPassword;
                //        }
                //        break;
                //    //在线监视消息
                //    case (int)RequestCode.CSMonitor:
                //        webReturn = DealMonitorMessage(arg);
                //        break;
                //    //激活进程消息
                //    case (int)RequestCode.CSActiveProcess:
                //        webReturn = DealActiveProcessMessage(arg);
                //        break;
                //}
                //if (NetPipeEvent != null)
                //{
                //    ThreadPool.QueueUserWorkItem(a => OnNetPipeEvent(arg));
                //}
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


        #region 定时清理垃圾

        private void CreateGCCollectThread()
        {
            try
            {
                if (mGCCollectThread != null)
                {
                    try
                    {
                        mGCCollectThread.Abort();
                    }
                    catch { }
                    mGCCollectThread = null;
                }
                mGCCollectThread = new Thread(() =>
                {
                    try
                    {
                        Thread.Sleep(mGCCollectInterval * 1000);
                        GC.Collect();
                    }
                    catch (Exception ex)
                    {
                        var dispose = ex as ObjectDisposedException;
                        if (dispose == null)
                        {
                            WriteLog("GCCollect", string.Format("GCCollect work fail.\t{0}", ex.Message));
                        }
                    }
                });
                mGCCollectThread.Start();
                WriteLog("GCCollect", string.Format("CreateGCCollectThread end.\t{0}", mGCCollectThread.ManagedThreadId));
            }
            catch (Exception ex)
            {
                WriteLog("GCCollect", string.Format("CreateGCCollectThread fail.\t{0}", ex.Message));
            }
        }

        private void StopGCCollectThread()
        {
            try
            {
                if (mGCCollectThread != null)
                {
                    mGCCollectThread.Abort();
                    mGCCollectThread = null;

                    WriteLog("GCCollect", string.Format("GCCollectThread stopped"));
                }
            }
            catch (Exception ex)
            {
                WriteLog("GCCollect", string.Format("StopGCCollectThread fail.\t{0}", ex.Message));
            }
        }

        #endregion


        #region WriteLog

        private void CreateLogOperator()
        {
            try
            {
                string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                    ConstValue.TEMP_DIR_UMP, AppName, "Logs");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                mLogOperator = new LogOperator();
                mLogOperator.LogPath = path;
                mLogOperator.Start();
                string strInfo = string.Empty;
                strInfo += string.Format("AppInfo\r\n");
                strInfo += string.Format("\tLogPath:{0}\r\n", path);
                strInfo += string.Format("\tExePath:{0}\r\n", AppDomain.CurrentDomain.BaseDirectory);
                strInfo += string.Format("\tName:{0}\r\n", AppDomain.CurrentDomain.FriendlyName);
                strInfo += string.Format("\tVersion:{0}\r\n",
                    System.Reflection.Assembly.GetExecutingAssembly().GetName().Version);
                strInfo += string.Format("\tHost:{0}\r\n", Environment.MachineName);
                strInfo += string.Format("\tAccount:{0}", Environment.UserName);
                WriteLog("AppLoad", strInfo);
            }
            catch { }
        }
        /// <summary>
        /// 写运行日志
        /// </summary>
        /// <param name="category">类别</param>
        /// <param name="msg">消息内容</param>
        public static void WriteLog(string category, string msg)
        {
            if (mLogOperator != null)
            {
                mLogOperator.WriteLog(LogMode.Info, category, msg);
            }
        }
        /// <summary>
        /// 写运行日志
        /// </summary>
        /// <param name="msg">消息类别</param>
        public static void WriteLog(string msg)
        {
            if (mLogOperator != null)
            {
                mLogOperator.WriteLog(LogMode.Info, AppName, msg);
            }
        }

        #endregion


        #region 回删MediaData临时文件

        private void TempDataRecylce()
        {
            try
            {
                string path = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                path = Path.Combine(path, ConstValue.UMP_PRODUCTER_SHORTNAME, ConstValue.TEMP_DIR_MEDIADATA);
                if (Directory.Exists(path))
                {
                    DirectoryInfo dirInfo = new DirectoryInfo(path);
                    var files = dirInfo.GetFiles().OrderBy(f => f.CreationTime).ToList();
                    DateTime now = DateTime.Now;
                    for (int i = 0; i < files.Count; i++)
                    {
                        var file = files[i];
                        var diff = now - file.CreationTime;
                        if (diff.TotalDays > 2)
                        {
                            //回删两天之前的临时文件
                            file.Delete();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLog("TempDataRecylce", string.Format("Fail.\t{0}", ex.Message));
            }
        }

        #endregion


        #region Encryption and Decryption

        public static string EncryptString(string strSource)
        {
            try
            {
                return ClientAESEncryption.EncryptString(strSource, EncryptionMode.AES256V04Hex);
            }
            catch (Exception ex)
            {
                WriteLog("Encryption", ex.Message);
                return strSource;
            }
        }

        public static string DecryptString(string strSource)
        {
            try
            {
                return ClientAESEncryption.DecryptString(strSource, EncryptionMode.AES256V04Hex);
            }
            catch (Exception ex)
            {
                WriteLog("Decryption", ex.Message);
                return strSource;
            }
        }

        #endregion


        #region DefaultLanguage

        [DllImport("kernel32.dll")]
        private static extern int GetUserDefaultUILanguage();

        #endregion


    }
}
