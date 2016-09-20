using System.Windows;
using VoiceCyber.UMP.Controls;

namespace UMPS3105
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App
    {

        public static UMPApp CurrentApp;

        protected override void OnStartup(StartupEventArgs e)
        {
            CurrentApp = new S3105App(false);
            CurrentApp.Startup();
            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (Current != null)
            {
                CurrentApp.Exit();
            }
            base.OnExit(e);
        }



        ////当前xbap全部操作权限
        //public static List<OperationInfo> ListOperationInfos;


        ////当前xbap当前人管理全部部门
        //public static List<CtrolOrg> ListCtrolOrgInfos;

        ////当前xbap当前人管理的全部座席
        //public static List<CtrolAgent> ListCtrolAgentInfos;

        ////当前用户所在部门以及子部门的ID
        //public static List<long> lstdptid;
        //public static List<string> lsAgentInfos;
        ///// <summary>
        ///// Y:有审批流程，N:无审批流程
        ///// </summary>
        //public static string AppealProcess = "Y";

        ////当前用户的机构Id
        //public static string CurrentOrg = "-1";

        //public static List<SftpServerInfo> mListSftpServers;
        //public static Service03Helper mService03Helper;
        //public static List<DownloadParamInfo> mListDownloadParams;
        //public static List<RecordEncryptInfo> mListRecordEncryptInfos;

        ////从多个页面跳转里要用的
        //public static bool NativePageFlag = true;

        //#region Init & Load Info
        //protected override void SetAppInfo()
        //{
        //    base.SetAppInfo();

        //    AppName = "UMPS3105";
        //    ModuleID = 3105;
            
        //}

        //protected override void InitSessionInfo()
        //{
        //    base.InitSessionInfo();

        //    if (Session == null) { return; }
        //    //AppServerInfo serverInfo = new AppServerInfo();
        //    //serverInfo.Protocol = "https";
        //    //serverInfo.Address = "192.168.6.55";
        //    //serverInfo.Port = 8082;
        //    //serverInfo.SupportHttps = true;
        //    //Session.AppServerInfo = serverInfo;


        //    //RoleInfo roleInfo = new RoleInfo();
        //    //roleInfo.ID = ConstValue.ROLE_SYSTEMADMIN;
        //    //roleInfo.Name = "System Admin";
        //    //Session.RoleInfo = roleInfo;
        //    ////Session.RoleID = ConstValue.RESOURCE_USER;

        //    UserInfo userInfo = new UserInfo();
        //    userInfo.UserID = ConstValue.USER_ADMIN;
        //    userInfo.Account = "a";
        //    userInfo.UserName = "a";
        //    userInfo.Password = "voicecyber";
        //    Session.UserInfo = userInfo;
        //    Session.UserID = ConstValue.USER_ADMIN;

        //    //UserInfo userInfo = new UserInfo();
        //    //userInfo.UserID = ConstValue.USER_ADMIN;
        //    //userInfo.Account = "administrator";
        //    //userInfo.UserName = "Administrator";
        //    //userInfo.Password = "voicecyber";
        //    //Session.UserInfo = userInfo;
        //    //Session.UserID = ConstValue.USER_ADMIN;


        //    AppServerInfo serverInfo = new AppServerInfo();
        //    serverInfo.Protocol = "http";
        //    serverInfo.Address = "192.168.4.166";
        //    serverInfo.Port = 8081;
        //    serverInfo.SupportHttps = false;
        //    Session.AppServerInfo = serverInfo;

        //    //AppServerInfo serverInfo = new AppServerInfo();
        //    //serverInfo.Protocol = "http";
        //    //serverInfo.Address = "192.168.9.118";
        //    //serverInfo.Port = 8081;
        //    //serverInfo.SupportHttps = false;
        //    //Session.AppServerInfo = serverInfo;

        //    //DatabaseInfo dbInfo = new DatabaseInfo();
        //    //dbInfo.TypeID = 3;
        //    //dbInfo.TypeName = "ORCL";
        //    //dbInfo.Host = "192.168.4.182";
        //    //dbInfo.Port = 1521;
        //    //dbInfo.DBName = "PFOrcl";
        //    //dbInfo.LoginName = "PFDEV";
        //    //dbInfo.Password = "PF,123";
        //    //Session.DatabaseInfo = dbInfo;
        //    //Session.DBType = dbInfo.TypeID;
        //    //Session.DBConnectionString = dbInfo.GetConnectionString();

        //    //DatabaseInfo dbInfo = new DatabaseInfo();
        //    //dbInfo.TypeID = 3;
        //    //dbInfo.TypeName = "ORCL";
        //    //dbInfo.Host = "192.168.9.238";
        //    //dbInfo.Port = 1521;
        //    //dbInfo.DBName = "ORCL";
        //    //dbInfo.LoginName = "UMP5  ";
        //    //dbInfo.Password = "ump5";
        //    //Session.DatabaseInfo = dbInfo;
        //    //Session.DBType = dbInfo.TypeID;
        //    //Session.DBConnectionString = dbInfo.GetConnectionString();

        //    //DatabaseInfo dbInfo = new DatabaseInfo();
        //    //dbInfo.TypeID = 2;
        //    //dbInfo.Host = "192.168.6.80";
        //    //dbInfo.Port = 1433;
        //    //dbInfo.DBName = "UMPDataDB1026";
        //    //dbInfo.LoginName = "sa";
        //    //dbInfo.Password = "Voicecodes123";
        //    //Session.DatabaseInfo = dbInfo;
        //    //Session.DBType = dbInfo.TypeID;
        //    //Session.DBConnectionString = dbInfo.GetConnectionString();

        //    DatabaseInfo dbInfo = new DatabaseInfo();
        //    dbInfo.TypeID = 2;
        //    dbInfo.TypeName = "MSSQL";
        //    dbInfo.Host = "192.168.4.182";
        //    dbInfo.Port = 1433;
        //    dbInfo.DBName = "UMPDataDB1129";
        //    dbInfo.LoginName = "PFDEV";
        //    dbInfo.Password = "PF,123";
        //    Session.DatabaseInfo = dbInfo;
        //    Session.DBType = dbInfo.TypeID;
        //    Session.DBConnectionString = dbInfo.GetConnectionString();


        //    //分表之类的参数
        //    //Session.ListPartitionTables.Clear();
        //    //PartitionTableInfo partInfo = new PartitionTableInfo();
        //    //partInfo.TableName = ConstValue.TABLE_NAME_RECORD;
        //    //partInfo.PartType = TablePartType.DatetimeRange;
        //    //partInfo.Other1 = ConstValue.TABLE_FIELD_NAME_RECORD_STARTRECORDTIME;
        //    //Session.ListPartitionTables.Add(partInfo);
        //}

        //protected override void Init()
        //{
        //    base.Init();
        //    ListOperationInfos = new List<OperationInfo>();
        //    ListCtrolAgentInfos = new List<CtrolAgent>();
        //    ListCtrolOrgInfos = new List<CtrolOrg>();
        //    lstdptid = new List<long>();
        //    lsAgentInfos = new List<string>();
        //    mListSftpServers = new List<Common3105.SftpServerInfo>();
        //    mService03Helper = new Service03Helper();
        //    mListDownloadParams = new List<DownloadParamInfo>();
        //    mListRecordEncryptInfos = new List<RecordEncryptInfo>();
        //    LoadSftpServerList();
        //    SetService03Helper();
        //    LoadRecordEncryptInfos();
        //    LoadDownloadParamList();

        //    //得到所有操作权限
        //    InitControledOperations("31", "3105");
        //    //得到所能管理的部门管理的座席
        //    InitControledAgentAndOrg("-1");
        //    GetDepartmentInfo();
        //    GetAppealProcess();

        //    if (Session != null)
        //    {
        //        WriteLog("AppLoad", string.Format("SessionInfo\r\n{0}", Session.LogInfo()));
        //    }
        //    InitLanguageInfos();
        //}

        //#endregion
        
        //private void SetService03Helper()
        //{
        //    try
        //    {
        //        mService03Helper.HostAddress = App.Session.AppServerInfo.Address;
        //        if (App.Session.AppServerInfo.SupportHttps)
        //        {
        //            mService03Helper.HostPort = App.Session.AppServerInfo.Port - 4;
        //        }
        //        else
        //        {
        //            mService03Helper.HostPort = App.Session.AppServerInfo.Port - 3;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        App.ShowExceptionMessage(ex.Message);
        //    }
        //}

        //private void LoadSftpServerList()
        //{
        //    try
        //    {
        //        WebRequest webRequest = new WebRequest();
        //        webRequest.Session = App.Session;
        //        webRequest.Code = 33;
        //        webRequest.ListData.Add(App.Session.UserID.ToString());
        //        Service31021Client client = new Service31021Client(
        //            WebHelper.CreateBasicHttpBinding(App.Session),
        //            WebHelper.CreateEndpointAddress(
        //                App.Session.AppServerInfo,
        //                "Service31021"));
        //        WebReturn webReturn = client.DoOperation(webRequest);
        //        client.Close();
        //        if (!webReturn.Result)
        //        {
        //            App.ShowExceptionMessage(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
        //            return;
        //        }
        //        if (webReturn.ListData == null)
        //        {
        //            App.ShowExceptionMessage(App.GetLanguageInfo("3105T00101", string.Format("ListData is null")));
        //            return;
        //        }
        //        mListSftpServers.Clear();
        //        for (int i = 0; i < webReturn.ListData.Count; i++)
        //        {
        //            string strInfo = webReturn.ListData[i];
        //            OperationReturn optReturn = XMLHelper.DeserializeObject<Common3105.SftpServerInfo>(strInfo);
        //            if (!optReturn.Result)
        //            {
        //                App.WriteLog("LoadSftp", string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
        //                continue;
        //            }
        //            Common3105.SftpServerInfo sftpInfo = optReturn.Data as Common3105.SftpServerInfo;
        //            if (sftpInfo == null)
        //            {
        //                App.WriteLog("LoadSftp", string.Format("Fail.\tSftpServerInfo is null"));
        //                continue;
        //            }
        //            mListSftpServers.Add(sftpInfo);
        //        }

        //        App.WriteLog("PageLoad", string.Format("Load SftpServerInfo"));
        //    }
        //    catch (Exception ex)
        //    {
        //        App.ShowExceptionMessage(ex.Message);
        //    }
        //}

        //private void LoadRecordEncryptInfos()
        //{
        //    try
        //    {
        //        mListRecordEncryptInfos.Clear();
        //        WebRequest webRequest = new WebRequest();
        //        webRequest.Code = (int)RequestCode.WSGetUserParamList;
        //        webRequest.Session = App.Session;
        //        webRequest.ListData.Add(App.Session.UserInfo.UserID.ToString());
        //        webRequest.ListData.Add("1");
        //        webRequest.ListData.Add(S3105Consts.USER_PARAM_GROUP_ENCRYPTINFO.ToString());
        //        //App.MonitorHelper.AddWebRequest(webRequest);
        //        Service11012Client client = new Service11012Client(WebHelper.CreateBasicHttpBinding(App.Session),
        //            WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service11012"));
        //        WebHelper.SetServiceClient(client);
        //        WebReturn webReturn = client.DoOperation(webRequest);
        //        //App.MonitorHelper.AddWebReturn(webReturn);
        //        client.Close();
        //        if (!webReturn.Result)
        //        {
        //            App.ShowExceptionMessage(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
        //            return;
        //        }
        //        if (webReturn.ListData == null)
        //        {
        //            App.ShowExceptionMessage(string.Format("WSFail.\tListData is null"));
        //            return;
        //        }
        //        for (int i = 0; i < webReturn.ListData.Count; i++)
        //        {
        //            OperationReturn optReturn = XMLHelper.DeserializeObject<UserParamInfo>(webReturn.ListData[i]);
        //            if (!optReturn.Result)
        //            {
        //                App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
        //                return;
        //            }
        //            UserParamInfo info = optReturn.Data as UserParamInfo;
        //            if (info != null)
        //            {
        //                int paramID = info.ParamID;
        //                string strValue = info.ParamValue;
        //                string[] arrValue = strValue.Split(new[] { ConstValue.SPLITER_CHAR_3 }, StringSplitOptions.None);
        //                string strAddress = string.Empty;
        //                string strPassword = string.Empty;
        //                string strExpireTime = string.Empty;
        //                if (arrValue.Length > 0)
        //                {
        //                    strAddress = arrValue[0];
        //                }
        //                if (arrValue.Length > 1)
        //                {
        //                    strPassword = arrValue[1];
        //                }
        //                if (arrValue.Length > 2)
        //                {
        //                    strExpireTime = arrValue[2];
        //                }
        //                DateTime dtExpireTime = Converter.NumberToDatetime(strExpireTime);
        //                if (string.IsNullOrEmpty(strAddress)
        //                    || string.IsNullOrEmpty(strPassword))
        //                {
        //                    App.WriteLog("LoadEncryptInfo", string.Format("Fail.\tEncryptInfo invalid."));
        //                    continue;
        //                }
        //                if (paramID > S3105Consts.USER_PARAM_GROUP_ENCRYPTINFO * 1000
        //                    && paramID < (S3105Consts.USER_PARAM_GROUP_ENCRYPTINFO + 1) * 1000
        //                    && dtExpireTime > DateTime.Now.ToUniversalTime())
        //                {
        //                    RecordEncryptInfo encryptInfo = new RecordEncryptInfo();
        //                    encryptInfo.UserID = App.Session.UserID;
        //                    encryptInfo.ServerAddress = strAddress;
        //                    encryptInfo.Password = strPassword;
        //                    encryptInfo.EndTime = dtExpireTime;
        //                    mListRecordEncryptInfos.Add(encryptInfo);
        //                }
        //            }
        //        }

        //        App.WriteLog("PageLoad", string.Format("Init RecordEncryptInfo."));
        //    }
        //    catch (Exception ex)
        //    {
        //        App.ShowExceptionMessage(ex.Message);
        //    }
        //}

        //private void LoadDownloadParamList()
        //{
        //    try
        //    {
        //        WebRequest webRequest = new WebRequest();
        //        webRequest.Session = App.Session;
        //        webRequest.Code = 34;// (int)S3102Codes.GetDownloadParamList;
        //        webRequest.ListData.Add(App.Session.UserID.ToString());
        //        Service31021Client client = new Service31021Client(
        //            WebHelper.CreateBasicHttpBinding(App.Session),
        //            WebHelper.CreateEndpointAddress(
        //                App.Session.AppServerInfo,
        //                "Service31021"));
        //        WebHelper.SetServiceClient(client);
        //        WebReturn webReturn = client.DoOperation(webRequest);
        //        client.Close();
        //        if (!webReturn.Result)
        //        {
        //            App.ShowExceptionMessage(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
        //            return;
        //        }
        //        if (webReturn.ListData == null)
        //        {
        //            App.ShowExceptionMessage(string.Format("Fail.\tListData is null"));
        //            return;
        //        }
        //        mListDownloadParams.Clear();
        //        for (int i = 0; i < webReturn.ListData.Count; i++)
        //        {
        //            string strInfo = webReturn.ListData[i];
        //            OperationReturn optReturn = XMLHelper.DeserializeObject<DownloadParamInfo>(strInfo);
        //            if (!optReturn.Result)
        //            {
        //                App.WriteLog("LoadSftp", string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
        //                continue;
        //            }
        //            DownloadParamInfo info = optReturn.Data as DownloadParamInfo;
        //            if (info == null)
        //            {
        //                App.WriteLog("LoadSftp", string.Format("Fail.\tSftpServerInfo is null"));
        //                continue;
        //            }
        //            mListDownloadParams.Add(info);
        //        }

        //        App.WriteLog("PageLoad", string.Format("Load DownloadParams"));
        //    }
        //    catch (Exception ex)
        //    {
        //        App.ShowExceptionMessage(ex.Message);
        //    }
        //}

        ////得到当前用所有角色的权限并集
        //public void InitControledOperations(string modelId, string parentId)
        //{
        //    try
        //    {
        //        WebRequest webRequest = new WebRequest();
        //        webRequest.Code = (int)RequestCode.WSGetUserOptList;
        //        webRequest.Session = App.Session;
        //        webRequest.ListData.Add(App.Session.UserInfo.UserID.ToString());
        //        webRequest.ListData.Add(modelId);
        //        webRequest.ListData.Add(parentId);

        //        Service11012Client client = new Service11012Client(WebHelper.CreateBasicHttpBinding(App.Session), 
        //            WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service11012"));
        //        WebReturn webReturn = client.DoOperation(webRequest);
        //        client.Close();
        //        if (!webReturn.Result)
        //        {
        //            App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
        //            return;
        //        }
        //        ListOperationInfos.Clear();
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
        //                    optInfo.Description = optInfo.Display;
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


        ////得到当前用户所管理的部门和座席
        //public void InitControledAgentAndOrg(string OrgID)
        //{
        //    try
        //    {
        //        WebRequest webRequest = new WebRequest();
        //        webRequest.Session = App.Session;
        //        webRequest.Code = 5;// (int)S3103Codes.GetControlOrgInfoList;
        //        webRequest.ListData.Add(App.Session.UserID.ToString());
        //        webRequest.ListData.Add(OrgID);
        //        Service31031Client client = new Service31031Client(
        //            WebHelper.CreateBasicHttpBinding(App.Session),
        //            WebHelper.CreateEndpointAddress(
        //                App.Session.AppServerInfo,
        //                "Service31031"));
        //        WebReturn webReturn = client.UMPTaskOperation(webRequest);
        //        client.Close();
        //        #region MyRegion
        //        if (!webReturn.Result)
        //        {
        //            App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
        //            return;
        //        }
        //        if (webReturn.ListData == null)
        //        {
        //            App.ShowExceptionMessage(App.GetLanguageInfo("3105T00101", string.Format("ListData is null")));
        //            return;
        //        }
        //        for (int i = 0; i < webReturn.ListData.Count; i++)
        //        {
        //            string strInfo = webReturn.ListData[i];
        //            string[] arrInfo = strInfo.Split(new[] { ConstValue.SPLITER_CHAR },
        //                StringSplitOptions.RemoveEmptyEntries);
        //            if (arrInfo.Length < 3) { continue; }
        //            CtrolOrg ctrolOrg = new CtrolOrg();
        //            ctrolOrg.ID = arrInfo[0];
        //            ctrolOrg.OrgName = arrInfo[1];
        //            ctrolOrg.OrgParentID = arrInfo[2];

        //            if (OrgID.Equals("-1"))
        //            {
        //                CurrentOrg = ctrolOrg.OrgParentID;
        //            }


        //            if (ListCtrolOrgInfos.Where(p => p.ID == ctrolOrg.ID).Count() == 0)
        //            {
        //                ListCtrolOrgInfos.Add(ctrolOrg);
        //            }
        //            InitControledAgentAndOrg(arrInfo[0]);
        //            InitControlAgents(arrInfo[0]);
        //        }

        //        #endregion
        //    }
        //    catch (Exception ex)
        //    {
        //        App.ShowExceptionMessage(ex.Message);
        //    }
        //}

        //private void InitControlAgents(string parentID)
        //{
        //    try
        //    {
        //        WebRequest webRequest = new WebRequest();
        //        webRequest.Session = App.Session;
        //        webRequest.Code = 6;// (int)S3103Codes.GetControlAgentInfoList;
        //        webRequest.ListData.Add(App.Session.UserID.ToString());
        //        webRequest.ListData.Add(parentID);
        //        webRequest.ListData.Add("A");
        //        Service31031Client client = new Service31031Client(
        //            WebHelper.CreateBasicHttpBinding(App.Session),
        //            WebHelper.CreateEndpointAddress(
        //                App.Session.AppServerInfo,
        //                "Service31031"));
        //        WebReturn webReturn = client.UMPTaskOperation(webRequest);
        //        client.Close();
        //        if (!webReturn.Result)
        //        {
        //            App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
        //            return;
        //        }
        //        if (webReturn.ListData == null)
        //        {
        //            App.ShowExceptionMessage(App.GetLanguageInfo("3105T00101", string.Format("ListData is null")));
        //            return;
        //        }
        //        for (int i = 0; i < webReturn.ListData.Count; i++)
        //        {
        //            string strInfo = webReturn.ListData[i];
        //            string[] arrInfo = strInfo.Split(new[] { ConstValue.SPLITER_CHAR },
        //                StringSplitOptions.RemoveEmptyEntries);
        //            if (arrInfo.Length < 3) { continue; }

        //            CtrolAgent ctrolAgent = new CtrolAgent();
        //            ctrolAgent.AgentID = arrInfo[0];
        //            ctrolAgent.AgentName = arrInfo[1];
        //            ctrolAgent.AgentFullName = arrInfo[2];
        //            ctrolAgent.AgentOrgID = parentID;
        //            if (ListCtrolAgentInfos.Where(p => p.AgentID == ctrolAgent.AgentID).Count() == 0)
        //            {
        //                ListCtrolAgentInfos.Add(ctrolAgent);
        //                lsAgentInfos.Add(ctrolAgent.AgentID);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        App.ShowExceptionMessage(ex.Message);
        //    }
        //}

        //private void GetAppealProcess()
        //{
        //    WebRequest webRequest = new WebRequest();
        //    webRequest.Code = (int)S3105Codes.GetAppealProcess;
        //    webRequest.Session = App.Session;
        //    //Service31051Client client = new Service31051Client();
        //    Service31051Client client = new Service31051Client(WebHelper.CreateBasicHttpBinding(App.Session),
        //        WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service31051"));
        //    WebReturn webReturn = client.UMPTaskOperation(webRequest);
        //    client.Close();
        //    if (!webReturn.Result)
        //    {
        //        App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
        //    }
        //    if (webReturn.DataSetData != null && webReturn.DataSetData.Tables[0] != null && webReturn.DataSetData.Tables[0].Rows.Count > 0)
        //    {
        //        AppealProcess = webReturn.DataSetData.Tables[0].Rows[0][0].ToString();
        //    }
        //}

        //public static long GetSerialID()
        //{
        //    try
        //    {
        //        WebRequest webRequest = new WebRequest();
        //        webRequest.Code = (int)RequestCode.WSGetSerialID;
        //        webRequest.Session = App.Session;
        //        webRequest.ListData.Add("31");
        //        webRequest.ListData.Add("305");
        //        webRequest.ListData.Add(DateTime.Now.ToString("yyyyMMddHHmmss"));
        //        Service11012Client client = new Service11012Client(WebHelper.CreateBasicHttpBinding(App.Session),
        //            WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service11012"));
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

        //protected override void InitLanguageInfos()
        //{
        //    base.InitLanguageInfos();
        //    try
        //    {
        //        if (Session == null || Session.LangTypeInfo == null)
        //        {
        //            return;
        //        }
        //        //ListLanguageInfos.Clear();
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
        //        webRequest.ListData.Add("31");
        //        webRequest.ListData.Add("3105");
        //        webRequest.ListData.Add(string.Empty);
        //        webRequest.ListData.Add(string.Empty);
        //        Service11012Client client = new Service11012Client(WebHelper.CreateBasicHttpBinding(App.Session), 
        //            WebHelper.CreateEndpointAddress(Session.AppServerInfo, "Service11012"));
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

        //        WriteLog(string.Format("AppStart\t\tLanguage loaded"));
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //}

        //#region other
        //private static void GetDepartmentInfo()
        //{
        //    lstdptid.Add(S3105Consts.ORG_ROOT);
        //    DataSet ds = GetDeptInfo();
        //    if (ds != null)
        //    {
        //        GetCurrentUserChildDeptID(ref lstdptid, ds, S3105Consts.ORG_ROOT);
        //    }
        //}

        //private static void GetCurrentUserChildDeptID(ref List<long> lstdptid, DataSet ds, long pid)
        //{
        //    if (pid != -1)
        //    {
        //        foreach (DataRow dr in ds.Tables[0].Rows)
        //        {
        //            long curdid = Convert2Long(dr["C001"]);
        //            long curpid = Convert2Long(dr["C004"]);
        //            if (curdid != -1 && curpid == pid && !lstdptid.Contains(curdid))
        //            {
        //                lstdptid.Add(curdid);
        //                GetCurrentUserChildDeptID(ref lstdptid, ds, curdid);
        //            }
        //        }
        //    }
        //}

        //private static DataSet GetDeptInfo()
        //{
        //    List<long> lstchilddeptid = new List<long>();
        //    WebRequest webRequest = new WebRequest();
        //    webRequest.Code = (int)S3105Codes.GetDepartmentInfo;
        //    webRequest.Session = App.Session;
        //    Service31051Client client = new Service31051Client(WebHelper.CreateBasicHttpBinding(App.Session),
        //            WebHelper.CreateEndpointAddress(Session.AppServerInfo, "Service31051"));
        //    WebReturn webReturn = client.UMPTaskOperation(webRequest);
        //    client.Close();
        //    if (!webReturn.Result)
        //    {
        //        App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
        //        return null;
        //    }
        //    if (webReturn.DataSetData != null && webReturn.DataSetData.Tables[0] != null && webReturn.DataSetData.Tables[0].Rows.Count > 0)
        //    {
        //        return webReturn.DataSetData;
        //        //foreach(DataRow dr in ds_deptinfo.Tables[0].Rows)
        //    }
        //    return null;
        //}

        //private static long Convert2Long(object obj)
        //{
        //    if (obj == null || string.IsNullOrWhiteSpace(obj.ToString()))
        //    {
        //        return -1;
        //    }
        //    else
        //        return Convert.ToInt64(obj.ToString());
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

    }
}
