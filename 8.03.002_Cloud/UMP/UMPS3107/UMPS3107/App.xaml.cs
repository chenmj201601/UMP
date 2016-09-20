using System.Windows;
using VoiceCyber.UMP.Controls;

namespace UMPS3107
{ 
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App
    {

        public static UMPApp CurrentApp;

        protected override void OnStartup(StartupEventArgs e)
        {
            CurrentApp = new S3107App(false);
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


        //#region members
        ///// <summary>
        ///// 任务设置  False 新建 | True 修改
        ///// </summary>
        //public static bool TaskModify = false;

        ///// <summary>
        ///// 查询设置  False 新建 | True 修改
        ///// </summary>
        //public static bool QueryModify = false;


        ////当前人管理全部部门
        //public static List<CtrolOrg> ListCtrolOrgInfos;

        ////当前人管理的全部座席
        //public static List<CtrolAgent> ListCtrolAgentInfos;

        ////当前人管理的全部真实分机
        //public static List<CtrolAgent> ListCtrolRealityExtension;

        ////当前人管理的全部虚拟分机
        //public static List<CtrolAgent> ListCtrolExtension;

        ///// <summary>
        ///// 分组管理方式，E虚拟分机，A座席，R真实分机；
        ///// </summary>
        //public static string GroupingWay;

        ////用户能管理的全部QA
        //public static List<CtrolQA> ListCtrolQAInfos;
        //public static ObjectItem mRootItem;
        //public static List<ObjectItem> mListAllObjects;

        ////当前用户的机构Id
        //public static string CurrentOrg = "-1";

        //#endregion

        //#region Init & load Info

        //protected override void SetAppInfo()
        //{
        //    base.SetAppInfo();

        //    AppName = "UMPS3107";
        //    ModuleID = 3107;
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
        //    userInfo.Password = "a";
        //    Session.UserInfo = userInfo;
        //    Session.UserID = ConstValue.USER_ADMIN;


        //    AppServerInfo serverInfo = new AppServerInfo();
        //    serverInfo.Protocol = "http";
        //    serverInfo.Address = "192.168.6.7";
        //    serverInfo.Port = 8081;
        //    serverInfo.SupportHttps = false;
        //    Session.AppServerInfo = serverInfo;

        //    //DatabaseInfo dbInfo = new DatabaseInfo();
        //    //dbInfo.TypeID = 3;
        //    //dbInfo.TypeName = "ORCL";
        //    //dbInfo.Host = "192.168.4.182";
        //    //dbInfo.Port = 1521;
        //    //dbInfo.DBName = "PFOrcl";
        //    //dbInfo.LoginName = "PFDEV_test";
        //    //dbInfo.Password = "pfdev_test";
        //    //Session.DatabaseInfo = dbInfo;
        //    //Session.DBType = dbInfo.TypeID;
        //    //Session.DBConnectionString = dbInfo.GetConnectionString();

        //    //DatabaseInfo dbInfo = new DatabaseInfo();
        //    //dbInfo.TypeID = 2;
        //    //dbInfo.Host = "192.168.9.236";
        //    //dbInfo.Port = 1433;
        //    //dbInfo.DBName = "UMPDataDB0107";
        //    //dbInfo.LoginName = "sa";
        //    //dbInfo.Password = "voicecodes";
        //    //Session.DatabaseInfo = dbInfo;
        //    //Session.DBType = dbInfo.TypeID;
        //    //Session.DBConnectionString = dbInfo.GetConnectionString();

        //    DatabaseInfo dbInfo = new DatabaseInfo();
        //    dbInfo.TypeID = 2;
        //    dbInfo.TypeName = "MSSQL";
        //    dbInfo.Host = "192.168.4.182";
        //    dbInfo.Port = 1433;
        //    dbInfo.DBName = "UMPDataDB0304";
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
        //    ListCtrolAgentInfos = new List<CtrolAgent>();
        //    ListCtrolRealityExtension = new List<CtrolAgent>();
        //    ListCtrolExtension = new List<CtrolAgent>();
        //    ListCtrolOrgInfos = new List<CtrolOrg>();
        //    ListCtrolQAInfos = new List<CtrolQA>();
        //    mListAllObjects = new List<ObjectItem>();
        //    mRootItem = new ObjectItem();

        //    GroupingWay = string.Empty;
        //    LoadGroupingMethodParams();
        //    //得到所能管理的部门管理的座席
        //    InitControledAgentAndOrg("-1");
        //    if (Session != null)
        //    {
        //        WriteLog("AppLoad", string.Format("SessionInfo\r\n{0}", Session.LogInfo()));
        //    }
        //    InitLanguageInfos();
        //}

        //protected override void InitLanguageInfos()
        //{
        //    base.InitLanguageInfos();
        //    try
        //    {
        //        if (Session == null || Session.LangTypeInfo == null) { return; }
        //        //ListLanguageInfos.Clear();
        //        WebRequest webRequest = new WebRequest();
        //        webRequest.Code = (int)RequestCode.WSGetLangList;
        //        webRequest.Session = Session;
        //        webRequest.ListData.Add(Session.LangTypeInfo.LangID.ToString());
        //        webRequest.ListData.Add(string.Empty);
        //        webRequest.ListData.Add("31");
        //        webRequest.ListData.Add("3107");
        //        webRequest.ListData.Add(string.Empty);
        //        webRequest.ListData.Add(string.Empty);
        //        Service11012Client client = new Service11012Client(
        //            WebHelper.CreateBasicHttpBinding(Session)
        //            , WebHelper.CreateEndpointAddress(Session.AppServerInfo, "Service11012"));
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
        //        ShowExceptionMessage(ex.Message);
        //    }
        //}



        //#endregion


        //#region 得到当前用户所管理的部门和QA、座席、分机、真实分机
        //public void InitControledAgentAndOrg(string OrgID)
        //{
        //    try
        //    {
        //        WebRequest webRequest = new WebRequest();
        //        webRequest.Session = App.Session;
        //        webRequest.Code = (int)S3107Codes.GetControlOrgInfoList;
        //        webRequest.ListData.Add(App.Session.UserID.ToString());
        //        webRequest.ListData.Add(OrgID);
        //        //Service31071Client client = new Service31071Client();
        //        Service31071Client client = new Service31071Client(WebHelper.CreateBasicHttpBinding(App.Session), WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service31071"));
        //        //WebHelper.SetServiceClient(client);
        //        WebReturn webReturn = client.UMPTaskOperation(webRequest);
        //        client.Close();
        //        if (!webReturn.Result)
        //        {
        //            App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
        //            return;
        //        }
        //        if (webReturn.ListData == null)
        //        {
        //            App.ShowExceptionMessage(string.Format("Fail.\tListData is null"));
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
        //            InitControlQA(arrInfo[0]);
        //            InitControlAgents(arrInfo[0]);
        //            InitControlRealityExtension(arrInfo[0]);
        //            InitControlExtension(arrInfo[0]);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        App.ShowExceptionMessage(ex.Message);
        //    }
        //}

        //private void InitControlQA(string parentID)
        //{
        //    try
        //    {
        //        WebRequest webRequest = new WebRequest();
        //        webRequest.Session = App.Session;
        //        webRequest.Code = (int)S3107Codes.GetQA;
        //        webRequest.ListData.Add(parentID);
        //        webRequest.ListData.Add("3103005");//任务中评分权限
        //        //Service31071Client client = new Service31071Client();
        //        Service31071Client client = new Service31071Client(WebHelper.CreateBasicHttpBinding(App.Session), WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service31071"));
        //        //WebHelper.SetServiceClient(client);
        //        WebReturn webReturn = client.UMPTaskOperation(webRequest);
        //        client.Close();
        //        if (!webReturn.Result)
        //        {
        //            App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
        //            return;
        //        }
        //        if (webReturn.ListData == null)
        //        {
        //            App.ShowExceptionMessage(string.Format("Fail.\tListData is null"));
        //            return;
        //        }
        //        for (int i = 0; i < webReturn.ListData.Count; i++)
        //        {
        //            string strInfo = webReturn.ListData[i];
        //            OperationReturn optReturn = XMLHelper.DeserializeObject<CtrolQA>(webReturn.ListData[i]);
        //            if (!optReturn.Result)
        //            {
        //                App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
        //                return;
        //            }
        //            CtrolQA ctrolQa = optReturn.Data as CtrolQA;

        //            if (ctrolQa != null)
        //            {
        //                ListCtrolQAInfos.Add(ctrolQa);
        //            }
        //        }
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
        //        webRequest.Code = (int)S3107Codes.GetControlAgentInfoList;
        //        webRequest.ListData.Add(App.Session.UserID.ToString());
        //        webRequest.ListData.Add(parentID);
        //        webRequest.ListData.Add("A");
        //        //Service31071Client client = new Service31071Client();
        //        Service31071Client client = new Service31071Client(WebHelper.CreateBasicHttpBinding(App.Session), WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service31071"));
        //        //WebHelper.SetServiceClient(client);
        //        WebReturn webReturn = client.UMPTaskOperation(webRequest);
        //        client.Close();
        //        if (!webReturn.Result)
        //        {
        //            App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
        //            return;
        //        }
        //        if (webReturn.ListData == null)
        //        {
        //            App.ShowExceptionMessage(string.Format("Fail.\tListData is null"));
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
        //            if (ListCtrolAgentInfos.Where(p => p.AgentID == ctrolAgent.AgentID).Count() == 0 && ctrolAgent.AgentFullName.ToUpper()!="N/A")
        //            {
        //                ListCtrolAgentInfos.Add(ctrolAgent);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        App.ShowExceptionMessage(ex.Message);
        //    }
        //}

        //private void InitControlRealityExtension(string parentID)
        //{
        //    try
        //    {
        //        WebRequest webRequest = new WebRequest();
        //        webRequest.Session = App.Session;
        //        webRequest.Code = (int)S3107Codes.GetControlAgentInfoList;
        //        webRequest.ListData.Add(App.Session.UserID.ToString());
        //        webRequest.ListData.Add(parentID);
        //        webRequest.ListData.Add("R");
        //        //Service31071Client client = new Service31071Client();
        //        Service31071Client client = new Service31071Client(WebHelper.CreateBasicHttpBinding(App.Session), WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service31071"));
        //        //WebHelper.SetServiceClient(client);
        //        WebReturn webReturn = client.UMPTaskOperation(webRequest);
        //        client.Close();
        //        if (!webReturn.Result)
        //        {
        //            App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
        //            return;
        //        }
        //        if (webReturn.ListData == null)
        //        {
        //            App.ShowExceptionMessage(string.Format("Fail.\tListData is null"));
        //            return;
        //        }
        //        for (int i = 0; i < webReturn.ListData.Count; i++)
        //        {
        //            string strInfo = webReturn.ListData[i];
        //            string[] arrInfo = strInfo.Split(new[] { ConstValue.SPLITER_CHAR },
        //                StringSplitOptions.RemoveEmptyEntries);
        //            if (arrInfo.Length < 3) { continue; }

        //            CtrolAgent ctrolRex = new CtrolAgent();
        //            ctrolRex.AgentID = arrInfo[0];
        //            ctrolRex.AgentName = arrInfo[1];
        //            ctrolRex.AgentFullName = arrInfo[2];
        //            ctrolRex.AgentOrgID = parentID;
        //            if (ListCtrolRealityExtension.Where(p => p.AgentID == ctrolRex.AgentID).Count() == 0)
        //            {
        //                ListCtrolRealityExtension.Add(ctrolRex);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        App.ShowExceptionMessage(ex.Message);
        //    }
        //}

        //private void InitControlExtension(string parentID)
        //{
        //    try
        //    {
        //        WebRequest webRequest = new WebRequest();
        //        webRequest.Session = App.Session;
        //        webRequest.Code = (int)S3107Codes.GetControlAgentInfoList;
        //        webRequest.ListData.Add(App.Session.UserID.ToString());
        //        webRequest.ListData.Add(parentID);
        //        webRequest.ListData.Add("E");
        //        //Service31071Client client = new Service31071Client();
        //        Service31071Client client = new Service31071Client(WebHelper.CreateBasicHttpBinding(App.Session), WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service31071"));
        //        //WebHelper.SetServiceClient(client);
        //        WebReturn webReturn = client.UMPTaskOperation(webRequest);
        //        client.Close();
        //        if (!webReturn.Result)
        //        {
        //            App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
        //            return;
        //        }
        //        if (webReturn.ListData == null)
        //        {
        //            App.ShowExceptionMessage(string.Format("Fail.\tListData is null"));
        //            return;
        //        }
        //        for (int i = 0; i < webReturn.ListData.Count; i++)
        //        {
        //            string strInfo = webReturn.ListData[i];
        //            string[] arrInfo = strInfo.Split(new[] { ConstValue.SPLITER_CHAR },
        //                StringSplitOptions.RemoveEmptyEntries);
        //            if (arrInfo.Length < 3) { continue; }

        //            CtrolAgent ctrolEx = new CtrolAgent();
        //            ctrolEx.AgentID = arrInfo[0];
        //            ctrolEx.AgentName = arrInfo[1];
        //            ctrolEx.AgentFullName = arrInfo[2];
        //            ctrolEx.AgentOrgID = parentID;
        //            if (ListCtrolExtension.Where(p => p.AgentID == ctrolEx.AgentID).Count() == 0)
        //            {
        //                ListCtrolExtension.Add(ctrolEx);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        App.ShowExceptionMessage(ex.Message);
        //    }
        //}

        //#endregion


        //#region 是否添加分机的全局参数
        //private void LoadGroupingMethodParams()
        //{
        //    try
        //    {
        //        WebRequest webRequest = new WebRequest();
        //        webRequest.Code = (int)RequestCode.WSGetGlobalParamList;
        //        webRequest.Session = App.Session;
        //        webRequest.ListData.Add("11");
        //        webRequest.ListData.Add("12010401");
        //        webRequest.ListData.Add("120104");
        //        Service11012Client client = new Service11012Client(WebHelper.CreateBasicHttpBinding(App.Session),
        //            WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service11012"));
        //        WebReturn webReturn = client.DoOperation(webRequest);
        //        client.Close();
        //        if (!webReturn.Result)
        //        {
        //            App.ShowExceptionMessage(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
        //            return;
        //        }
        //        if (webReturn.ListData.Count <= 0) { return; }
        //        string str = webReturn.ListData[0];
        //        str = str.Replace("&#x1B;", "");
        //        OperationReturn optReturn = XMLHelper.DeserializeObject<GlobalParamInfo>(str);
        //        if (!optReturn.Result)
        //        {
        //            App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
        //            return;
        //        }
        //        GlobalParamInfo GlobalParamInfo = optReturn.Data as GlobalParamInfo;
        //        if (GlobalParamInfo == null) { return; }
        //        string tempGroupWay = GlobalParamInfo.ParamValue.Substring(8);
        //        GroupingWay = tempGroupWay;
        //    }
        //    catch (Exception ex)
        //    {
        //        App.ShowExceptionMessage(ex.Message);
        //    }
        //}

        //#endregion
    }
}
