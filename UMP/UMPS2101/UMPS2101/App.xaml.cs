using System;
using System.Windows;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Controls;
using VoiceCyber.UMP.Encryptions;

namespace UMPS2101
{ 
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App
    {

        #region Memebers

        public static UMPApp CurrentApp;

        #endregion

        protected override void OnStartup(StartupEventArgs e)
        {
            CurrentApp = new S2101App(false);
            CurrentApp.Startup();
            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (CurrentApp != null)
            {
                CurrentApp.Exit();
            }
            base.OnExit(e);
        }

        #region Encryption and Decryption

        public static string EncryptString(string strSource)
        {
            try
            {
                return ClientAESEncryption.EncryptString(strSource, EncryptionMode.AES256V04Hex);
            }
            catch (Exception ex)
            {
                CurrentApp.WriteLog("Encryption", string.Format("Fail.\t{0}", ex.Message));
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
                CurrentApp.WriteLog("Encryption", string.Format("Fail.\t{0}", ex.Message));
                return strSource;
            }
        }

        #endregion

        //#region Init and Load

        //private void Application_Startup(object sender, StartupEventArgs e)
        //{
        //    //根据权限决定是跳到任务跟踪还是任务分配里
        //    Application currApp = Application.Current;
        //    currApp.StartupUri = new Uri("FilterConditionMainPage.xaml", UriKind.RelativeOrAbsolute);
        //}

        //protected override void SetAppInfo()
        //{
        //    base.SetAppInfo();
        //    AppName = "UMPS2101";
        //    ModuleID = 2101;
        //}

        //protected override void InitSessionInfo()
        //{
        //    base.InitSessionInfo();
        //    Session = new SessionInfo();
        //    Session.SessionID = Guid.NewGuid().ToString();
        //    Session.AppName = AppName;
        //    Session.LastActiveTime = DateTime.Now;

        //    RentInfo rentInfo = new RentInfo();
        //    rentInfo.ID = S2101Consts.RENT_DEFAULT;
        //    rentInfo.Token = S2101Consts.RENT_DEFAULT_TOKEN;
        //    rentInfo.Domain = "voicecyber.com";
        //    rentInfo.Name = "voicecyber";
        //    Session.RentInfo = rentInfo;

        //    UserInfo userInfo = new UserInfo();
        //    userInfo.UserID = App.LongParse(string.Format(S2101Consts.USER_ADMIN, S2101Consts.RENT_DEFAULT_TOKEN), 0);
        //    userInfo.Account = "administrator";
        //    userInfo.UserName = "a";
        //    userInfo.Password = "a";
        //    Session.UserInfo = userInfo;
        //    Session.UserID = App.LongParse(string.Format(S2101Consts.USER_ADMIN, S2101Consts.RENT_DEFAULT_TOKEN), 0);

        //    RoleInfo roleInfo = new RoleInfo();
        //    roleInfo.ID = App.LongParse(string.Format(S2101Consts.ROLE_SYSTEMADMIN, S2101Consts.RENT_DEFAULT_TOKEN), 0);
        //    roleInfo.Name = "System Admin";
        //    Session.RoleInfo = roleInfo;

        //    AppServerInfo serverInfo = new AppServerInfo();
        //    serverInfo.Protocol = "http";
        //    serverInfo.Address = "192.168.4.166";
        //    serverInfo.Port = 8081;
        //    serverInfo.SupportHttps = false;
        //    Session.AppServerInfo = serverInfo;

        //    ThemeInfo themeInfo = new ThemeInfo();
        //    themeInfo.Name = "Default";
        //    themeInfo.Color = "Brown";
        //    Session.ThemeInfo = themeInfo;
        //    Session.ThemeName = "Default";

        //    themeInfo = new ThemeInfo();
        //    themeInfo.Name = "Style01";
        //    themeInfo.Color = "Green";
        //    Session.SupportThemes.Add(themeInfo);

        //    themeInfo = new ThemeInfo();
        //    themeInfo.Name = "Style02";
        //    themeInfo.Color = "Yellow";
        //    Session.SupportThemes.Add(themeInfo);

        //    themeInfo = new ThemeInfo();
        //    themeInfo.Name = "Style03";
        //    themeInfo.Color = "Brown";
        //    Session.SupportThemes.Add(themeInfo);

        //    themeInfo = new ThemeInfo();
        //    themeInfo.Name = "Style04";
        //    themeInfo.Color = "Blue";
        //    Session.SupportThemes.Add(themeInfo);


        //    LangTypeInfo langType = new LangTypeInfo();
        //    langType = new LangTypeInfo();
        //    langType.LangID = 1033;
        //    langType.LangName = "en-us";
        //    langType.Display = "English";
        //    Session.SupportLangTypes.Add(langType);

        //    langType = new LangTypeInfo();
        //    langType.LangID = 2052;
        //    langType.LangName = "zh-cn";
        //    langType.Display = "简体中文";
        //    Session.LangTypeInfo = langType;
        //    Session.LangTypeID = 2052;
        //    Session.SupportLangTypes.Add(langType);

        //    langType = new LangTypeInfo();
        //    langType.LangID = 1028;
        //    langType.LangName = "tc-CN";
        //    langType.Display = "繁體中文";
        //    Session.SupportLangTypes.Add(langType);

        //    //DatabaseInfo dbInfo = new DatabaseInfo();
        //    //dbInfo.TypeID = 3;
        //    //dbInfo.TypeName = "ORCL";
        //    //dbInfo.Host = "192.168.4.182";
        //    //dbInfo.Port = 1521;
        //    //dbInfo.DBName = "PFOrcl";
        //    //dbInfo.LoginName = "PFDEV";
        //    //dbInfo.Password = "PF,123";
        //    //Session.DBType = dbInfo.TypeID;
        //    //Session.DBConnectionString = dbInfo.GetConnectionString();
        //    //Session.DatabaseInfo = dbInfo;

        //    DatabaseInfo dbInfo = new DatabaseInfo();
        //    dbInfo.TypeID = 2;
        //    dbInfo.TypeName = "MSSQL";
        //    dbInfo.Host = "192.168.4.182";
        //    dbInfo.Port = 1433;
        //    dbInfo.DBName = "UMPDataDB1129";
        //    dbInfo.LoginName = "PFDEV";
        //    dbInfo.Password = "PF,123";
        //    Session.DBType = dbInfo.TypeID;
        //    Session.DBConnectionString = dbInfo.GetConnectionString();
        //    Session.DatabaseInfo = dbInfo;

        //    //分表之类的参数
        //    Session.ListPartitionTables.Clear();
        //    //PartitionTableInfo partInfo = new PartitionTableInfo();
        //    //partInfo.TableName = ConstValue.TABLE_NAME_RECORD;
        //    //partInfo.PartType = TablePartType.DatetimeRange;
        //    //partInfo.Other1 = ConstValue.TABLE_FIELD_NAME_RECORD_STARTRECORDTIME;
        //    //Session.ListPartitionTables.Add(partInfo);


        //    Session.InstallPath = @"C:\UMPRelease";
        //    WriteLog("AppInit", string.Format("SessionInfo inited."));
        //}

        //protected override void Init()
        //{
        //    base.Init();
        //    ListLanguageInfos = new List<LanguageInfo>();
        //    ListOperationInfos = new List<OperationInfo>();

        //    InitLanguageInfos();
        //    //得到所有操作权限
        //    InitControledOperations("21", "2101");
        //    if (Session != null)
        //    {
        //        WriteLog("AppLoad", string.Format("SessionInfo\r\n{0}", Session.LogInfo()));
        //    }
        //}

        ////得到当前用所有角色的权限并集
        //public void InitControledOperations(string modelId, string parentId)
        //{
        //    try
        //    {
        //        WebRequest webRequest = new WebRequest();
        //        webRequest.Code = (int)S2101Codes.GetRoleOperationList;
        //        webRequest.Session = App.Session;
        //        webRequest.ListData.Add(modelId);
        //        webRequest.ListData.Add(parentId);

        //        //Service21011Client client = new Service21011Client();
        //        Service21011Client client = new Service21011Client(WebHelper.CreateBasicHttpBinding(App.Session), WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service21011"));
        //        //WebHelper.SetServiceClient(client);
        //        WebReturn webReturn = client.DoOperation(webRequest);
        //        client.Close();
        //        if (!webReturn.Result)
        //        {
        //            App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
        //            return;
        //        }
        //        if (webReturn.ListData.Count > 0)
        //        {
        //            for (int i = 0; i < webReturn.ListData.Count; i++)
        //            {
        //                OperationReturn optReturn = XMLHelper.DeserializeObject<OperationInfo>(webReturn.ListData[i]);
        //                if (!optReturn.Result)
        //                {
        //                    App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
        //                    return;
        //                }
        //                OperationInfo optInfo = optReturn.Data as OperationInfo;
        //                if (optInfo != null)
        //                {
        //                    optInfo.Display = App.GetLanguageInfo(string.Format("FO{0}", optInfo.ID), optInfo.ID.ToString());
        //                    ListOperationInfos.Add(optInfo);

        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        App.ShowExceptionMessage(ex.Message);
        //    }
        //}

        //#endregion

        //#region OperationLog

        //private void CreateLogOperator()
        //{
        //    try
        //    {
        //        string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
        //           string.Format("UMP/{0}/Logs", AppName));
        //        if (!Directory.Exists(path))
        //        {
        //            Directory.CreateDirectory(path);
        //        }
        //        mLogOperator = new LogOperator();
        //        mLogOperator.LogPath = path;
        //        mLogOperator.Start();
        //        string strInfo = string.Empty;
        //        strInfo += string.Format("LogPath:{0}\r\n", path);
        //        strInfo += string.Format("\tExePath:{0}\r\n", AppDomain.CurrentDomain.BaseDirectory);
        //        strInfo += string.Format("\tName:{0}\r\n", AppDomain.CurrentDomain.FriendlyName);
        //        strInfo += string.Format("\tVersion:{0}\r\n",
        //            System.Reflection.Assembly.GetExecutingAssembly().GetName().Version);
        //        strInfo += string.Format("\tHost:{0}\r\n", Environment.MachineName);
        //        strInfo += string.Format("\tAccount:{0}", Environment.UserName);
        //        WriteLog("Init", strInfo);
        //    }
        //    catch { }
        //}

        //#endregion

        //#region GetSerialID
        //public static long GetSerialID(string table)
        //{
        //    if (string.IsNullOrWhiteSpace(table))
        //        return 0;
        //    try
        //    {
        //        WebRequest webRequest = new WebRequest();
        //        webRequest.Code = (int)RequestCode.WSGetSerialID;
        //        webRequest.Session = App.Session;
        //        webRequest.ListData.Add("21");
        //        webRequest.ListData.Add(table);
        //        webRequest.ListData.Add(DateTime.Now.ToString("yyyyMMddHHmmss"));
        //        Service11012Client client = new Service11012Client(WebHelper.CreateBasicHttpBinding(App.Session),
        //            WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service11012"));
        //        //WebHelper.SetServiceClient(client);
        //        WebReturn webReturn = client.DoOperation(webRequest);
        //        client.Close();
        //        if (!webReturn.Result)
        //        {
        //            App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
        //            return -1;
        //        }
        //        long id = Convert.ToInt64(webReturn.Data);
        //        return id;
        //    }
        //    catch (Exception ex)
        //    {
        //        App.ShowExceptionMessage(ex.Message);
        //        return -1;
        //    }
        //}
        //#endregion

        //#region NetPipe
        //private static void SendModuleCloseMessage()
        //{
        //    WebRequest request = new WebRequest();
        //    request.Code = (int)RequestCode.CSModuleClose;
        //    request.Session = Session;
        //    SendNetPipeMessage(request);
        //}

        //private bool SendLoadingMessage()
        //{
        //    try
        //    {
        //        WebRequest request = new WebRequest();
        //        request.Code = (int)RequestCode.CSModuleLoading;
        //        request.Session = Session;
        //        var webReturn = SendNetPipeMessage(request);
        //        if (webReturn.Result)
        //        {
        //            var parameter = webReturn.Data;
        //            var dbType = webReturn.Session.DBType;
        //            var dbConnectionString = webReturn.Session.DBConnectionString;
        //            var roleInfo = webReturn.Session.RoleInfo;
        //            var roleName = roleInfo.Name;
        //            var appServerInfo = webReturn.Session.AppServerInfo;
        //            var host = appServerInfo.Address;
        //            var port = appServerInfo.Port;
        //            var userInfo = webReturn.Session.UserInfo;
        //            var userAccount = userInfo.Account;
        //            var themeInfo = webReturn.Session.ThemeInfo;
        //            var themeName = themeInfo.Name;
        //            var langTypeInfo = webReturn.Session.LangTypeInfo;
        //            var langTypeID = langTypeInfo.LangID;
        //            string strMsg = string.Empty;
        //            strMsg += string.Format("DBType:{0}\r\n", dbType);
        //            strMsg += string.Format("\tDBConnectionString:{0}\r\n", EncryptString(dbConnectionString));
        //            strMsg += string.Format("\tRoleName:{0}\r\n", roleName);
        //            strMsg += string.Format("\tAppServerHost:{0}\r\n", host);
        //            strMsg += string.Format("\tAppServerPort:{0}\r\n", port);
        //            strMsg += string.Format("\tUserAccount:{0}\r\n", userAccount);
        //            strMsg += string.Format("\tThemeName:{0}\r\n", themeName);
        //            strMsg += string.Format("\tLangTypeID:{0}\r\n", langTypeID);
        //            strMsg += string.Format("\tParameter:{0}", parameter);
        //            WriteLog("Init", string.Format("{0}", strMsg));
        //            Session.SetSessionInfo(webReturn.Session);
        //            return true;
        //        }
        //        WriteLog("Init", string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
        //    }
        //    catch (Exception ex)
        //    {
        //        ShowExceptionMessage(ex.Message);
        //    }
        //    return false;
        //}
        //#endregion

        //#region Language

        //protected override void InitLanguageInfos()
        //{
        //    base.InitLanguageInfos();
        //    try
        //    {
        //        #region MyRegion
        //        if (Session == null || Session.LangTypeInfo == null)
        //        {
        //            return;
        //        }
        //        ListLanguageInfos.Clear();
        //        WebRequest webRequest = new WebRequest();
        //        webRequest.Code = (int)RequestCode.WSGetLangList;
        //        webRequest.Session = Session;
        //        //ListParams
        //        //0     LangID
        //        //1     PreName（语言内容编码的前缀，比如 FO:模块、操作显示语言）
        //        //1     ModuleID
        //        //2     SubModuleID
        //        //3     Page
        //        //4     Name
        //        webRequest.ListData.Add(Session.LangTypeInfo.LangID.ToString());
        //        webRequest.ListData.Add(string.Empty);
        //        webRequest.ListData.Add("21");
        //        webRequest.ListData.Add("2101");
        //        webRequest.ListData.Add(string.Empty);
        //        webRequest.ListData.Add(string.Empty);
        //        Service11012Client client = new Service11012Client(WebHelper.CreateBasicHttpBinding(App.Session), WebHelper.CreateEndpointAddress(Session.AppServerInfo, "Service11012"));
        //        //Service11012Client client = new Service11012Client();
        //        //WebHelper.SetServiceClient(client);
        //        WebReturn webReturn = client.DoOperation(webRequest);
        //        client.Close();
        //        if (!webReturn.Result)
        //        {
        //            ShowExceptionMessage(string.Format("{0}\t{1}", webReturn.Code, webReturn.Message));
        //        }
        //        for (int i = 0; i < webReturn.ListData.Count; i++)
        //        {
        //            OperationReturn optReturn = XMLHelper.DeserializeObject<LanguageInfo>(webReturn.ListData[i]);
        //            if (!optReturn.Result)
        //            {
        //                ShowExceptionMessage(string.Format("{0}\t{1}", optReturn.Code, optReturn.Message));
        //                return;
        //            }
        //            LanguageInfo langInfo = optReturn.Data as LanguageInfo;
        //            if (langInfo == null)
        //            {
        //                ShowExceptionMessage(string.Format("LanguageInfo is null"));
        //                return;
        //            }
        //            ListLanguageInfos.Add(langInfo);
        //        }


        //        //ListParams
        //        //0     LangID
        //        //1     PreName（语言内容编码的前缀，比如 FO:模块、操作显示语言）
        //        //1     ModuleID
        //        //2     SubModuleID
        //        //3     Page
        //        //4     Name
        //        webRequest = new WebRequest();
        //        webRequest.Session = App.Session;
        //        webRequest.Code = (int)RequestCode.WSGetLangList;
        //        webRequest.ListData.Add(string.Empty);
        //        webRequest.ListData.Add("FO");
        //        webRequest.ListData.Add(string.Empty);
        //        webRequest.ListData.Add(string.Empty);
        //        webRequest.ListData.Add(string.Empty);
        //        webRequest.ListData.Add(string.Empty);
        //        client = new Service11012Client(WebHelper.CreateBasicHttpBinding(App.Session), WebHelper.CreateEndpointAddress(Session.AppServerInfo, "Service11012"));
        //        //WebHelper.SetServiceClient(client);
        //        webReturn = client.DoOperation(webRequest);
        //        client.Close();
        //        if (!webReturn.Result)
        //        {
        //            App.ShowExceptionMessage(string.Format("{0}\t{1}", webReturn.Code, webReturn.Message));
        //        }
        //        for (int i = 0; i < webReturn.ListData.Count; i++)
        //        {
        //            OperationReturn optReturn = XMLHelper.DeserializeObject<LanguageInfo>(webReturn.ListData[i]);
        //            if (!optReturn.Result)
        //            {
        //                App.ShowExceptionMessage(string.Format("{0}\t{1}", optReturn.Code, optReturn.Message));
        //                return;
        //            }
        //            LanguageInfo langInfo = optReturn.Data as LanguageInfo;
        //            if (langInfo == null)
        //            {
        //                App.ShowExceptionMessage(string.Format("LanguageInfo is null"));
        //                return;
        //            }
        //            if (ListLanguageInfos.Where(p => p.LangID == langInfo.LangID && p.Name == langInfo.Name).Count() == 0)
        //            {
        //                ListLanguageInfos.Add(langInfo);
        //            }
        //        }


        //        webRequest = new WebRequest();
        //        webRequest.Session = App.Session;
        //        webRequest.Code = (int)RequestCode.WSGetLangList;
        //        webRequest.ListData.Add(string.Empty);
        //        webRequest.ListData.Add("COM");
        //        webRequest.ListData.Add(string.Empty);
        //        webRequest.ListData.Add(string.Empty);
        //        webRequest.ListData.Add(string.Empty);
        //        webRequest.ListData.Add(string.Empty);
        //        client = new Service11012Client(WebHelper.CreateBasicHttpBinding(App.Session), WebHelper.CreateEndpointAddress(Session.AppServerInfo, "Service11012"));
        //        //WebHelper.SetServiceClient(client);
        //        webReturn = client.DoOperation(webRequest);
        //        client.Close();
        //        if (!webReturn.Result)
        //        {
        //            App.ShowExceptionMessage(string.Format("{0}\t{1}", webReturn.Code, webReturn.Message));
        //        }
        //        for (int i = 0; i < webReturn.ListData.Count; i++)
        //        {
        //            OperationReturn optReturn = XMLHelper.DeserializeObject<LanguageInfo>(webReturn.ListData[i]);
        //            if (!optReturn.Result)
        //            {
        //                App.ShowExceptionMessage(string.Format("{0}\t{1}", optReturn.Code, optReturn.Message));
        //                return;
        //            }
        //            LanguageInfo langInfo = optReturn.Data as LanguageInfo;
        //            if (langInfo == null)
        //            {
        //                App.ShowExceptionMessage(string.Format("LanguageInfo is null"));
        //                return;
        //            }
        //            if (ListLanguageInfos.Where(p => p.LangID == langInfo.LangID && p.Name == langInfo.Name).Count() == 0)
        //            {
        //                ListLanguageInfos.Add(langInfo);
        //            }
        //        }
        //        #endregion
        //    }
        //    catch (Exception ex)
        //    {
        //        //ShowExceptionMessage(ex.Message);
        //    }
        //}

        //#endregion

        //#region Encryption and Decryption

        //public static string EncryptString(string strSource)
        //{
        //    string strTemp = EncryptionAndDecryption.EncryptDecryptString(strSource,
        //     CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004),
        //     EncryptionAndDecryption.UMPKeyAndIVType.M004);
        //    return strTemp;
        //}

        //public static string DecryptString(string strSource)
        //{
        //    string strTemp = EncryptionAndDecryption.EncryptDecryptString(strSource,
        //      CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104),
        //      EncryptionAndDecryption.UMPKeyAndIVType.M104);
        //    return strTemp;
        //}

        //public static string CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType aKeyIVID)
        //{
        //    string lStrReturn;
        //    int LIntRand;
        //    Random lRandom = new Random();
        //    string LStrTemp;

        //    try
        //    {
        //        lStrReturn = DateTime.Now.ToString("yyyyMMddHHmmss");
        //        LIntRand = lRandom.Next(0, 14);
        //        LStrTemp = LIntRand.ToString("00");
        //        lStrReturn = lStrReturn.Insert(LIntRand, "VCT");
        //        LIntRand = lRandom.Next(0, 17);
        //        LStrTemp += LIntRand.ToString("00");
        //        lStrReturn = lStrReturn.Insert(LIntRand, "UMP");
        //        LIntRand = lRandom.Next(0, 20);
        //        LStrTemp += LIntRand.ToString("00");
        //        lStrReturn = lStrReturn.Insert(LIntRand, ((int)aKeyIVID).ToString("000"));

        //        lStrReturn = EncryptionAndDecryption.EncryptStringY(LStrTemp + lStrReturn);
        //    }
        //    catch { lStrReturn = string.Empty; }

        //    return lStrReturn;
        //}

        //#endregion

        //#region
        //public static long LongParse(string str, long defaultValue)
        //{
        //    long outRet = defaultValue;
        //    if (!long.TryParse(str, out outRet))
        //    {
        //        outRet = defaultValue;
        //    }

        //    return outRet;
        //}
        //#endregion
    }
}
