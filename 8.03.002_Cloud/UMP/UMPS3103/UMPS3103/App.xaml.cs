using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Threading;
using System.Collections.Generic;
using PFShareClassesC;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common31031;
using VoiceCyber.UMP.Communications;
using UMPS3103.Wcf11012;
using UMPS3103.Wcf31031;
using UMPS3103.Wcf11901;
using UMPS3103.Wcf31021;
using UMPS3103.Codes;
using UMPS3103.Models;
using System.Collections.ObjectModel;
using VoiceCyber.UMP.Controls;



namespace UMPS3103
{
    /// <summary>
    /// S3103App.xaml 的交互逻辑
    /// </summary>
    public partial class App
    {
        public static UMPApp CurrentApp;

        protected override void OnStartup(StartupEventArgs e)
        {
            CurrentApp = new S3103App(false);
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

        #region
        /*
        #region Members

        //当前全部操作权限
        public static List<OperationInfo> ListOperationInfos;

        //当前人管理全部部门
        public static List<CtrolOrg> ListCtrolOrgInfos;

        //当前人管理的全部座席
        public static List<CtrolAgent> ListCtrolAgentInfos;

        //当前人管理的全部真实分机
        public static List<CtrolAgent> ListCtrolRealityExtension;

        //当前人管理的全部虚拟分机
        public static List<CtrolAgent> ListCtrolExtension;


        /// <summary>
        /// 分组管理方式，E虚拟分机，A座席，R真实分机；
        /// </summary>
        public static string GroupingWay;

        //当前用户能管理的全部QA
        public static List<CtrolQA> ListCtrolQAInfos;
        public static ObjectItems mRootItem;
        public static List<ObjectItems> mListAllObjects;
        /// <summary>
        /// 获取所有座席、分机、用户信息 
        /// </summary>
        public static ObservableCollection<AgentAndUserInfoItems> mListAuInfoItems;
        
        //当前用户的机构Id
        public static string CurrentOrg = "-1";

        public static List<SftpServerInfo> mListSftpServers;
        public static Service03Helper mService03Helper;
        public static List<DownloadParamInfo> mListDownloadParams;
        public static List<RecordEncryptInfo> mListRecordEncryptInfos;

        //从多个页面跳转里要用的
        public static bool NativePageFlag = true;

        #endregion

        #region Init & Load Info
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            //根据权限决定是跳到任务跟踪还是任务分配里
            Application currApp = Application.Current;
            if (ListOperationInfos.Where(p => p.ID == S3103Consts.OPT_TASKASSIGN).Count() > 0)
            {
                currS3103App.StartupUri = new Uri("TaskAssign.xaml", UriKind.RelativeOrAbsolute);
            }
            else
            {
                currS3103App.StartupUri = new Uri("TaskTrack.xaml", UriKind.RelativeOrAbsolute);
            }
        }
        protected override void SetAppInfo()
        {
            base.SetAppInfo();

            AppName = "UMPS3103";
            ModuleID = 3103;

        }

        protected override void InitSessionInfo()
        {
            base.InitSessionInfo();

            if (Session == null) { return; }
            //AppServerInfo serverInfo = new AppServerInfo();
            //serverInfo.Protocol = "https";
            //serverInfo.Address = "192.168.6.55";
            //serverInfo.Port = 8082;
            //serverInfo.SupportHttps = true;
            //Session.AppServerInfo = serverInfo;


            //RoleInfo roleInfo = new RoleInfo();
            //roleInfo.ID = ConstValue.ROLE_SYSTEMADMIN;
            //roleInfo.Name = "System Admin";
            //Session.RoleInfo = roleInfo;
            ////Session.RoleID = ConstValue.RESOURCE_USER;

            UserInfo userInfo = new UserInfo();
            userInfo.UserID = ConstValue.USER_ADMIN;
            userInfo.Account = "a";
            userInfo.UserName = "a";
            userInfo.Password = "voicecyber";
            Session.UserInfo = userInfo;
            Session.UserID = ConstValue.USER_ADMIN;

            //UserInfo userInfo = new UserInfo();
            //userInfo.UserID = ConstValue.USER_ADMIN;
            //userInfo.Account = "administrator";
            //userInfo.UserName = "Administrator";
            //userInfo.Password = "voicecyber";
            //Session.UserInfo = userInfo;
            //Session.UserID = ConstValue.USER_ADMIN;


            AppServerInfo serverInfo = new AppServerInfo();
            serverInfo.Protocol = "http";
            serverInfo.Address = "192.168.6.7";
            serverInfo.Port = 8081;
            serverInfo.SupportHttps = false;
            Session.AppServerInfo = serverInfo;

            //AppServerInfo serverInfo = new AppServerInfo();
            //serverInfo.Protocol = "http";
            //serverInfo.Address = "192.168.9.118";
            //serverInfo.Port = 8081;
            //serverInfo.SupportHttps = false;
            //Session.AppServerInfo = serverInfo;

            //DatabaseInfo dbInfo = new DatabaseInfo();
            //dbInfo.TypeID = 3;
            //dbInfo.TypeName = "ORCL";
            //dbInfo.Host = "192.168.4.182";
            //dbInfo.Port = 1521;
            //dbInfo.DBName = "PFOrcl";
            //dbInfo.LoginName = "PFDEV";
            //dbInfo.Password = "PF,123";
            //Session.DatabaseInfo = dbInfo;
            //Session.DBType = dbInfo.TypeID;
            //Session.DBConnectionString = dbInfo.GetConnectionString();

            //DatabaseInfo dbInfo = new DatabaseInfo();
            //dbInfo.TypeID = 3;
            //dbInfo.TypeName = "ORCL";
            //dbInfo.Host = "192.168.9.238";
            //dbInfo.Port = 1521;
            //dbInfo.DBName = "ORCL";
            //dbInfo.LoginName = "UMP5  ";
            //dbInfo.Password = "ump5";
            //Session.DatabaseInfo = dbInfo;
            //Session.DBType = dbInfo.TypeID;
            //Session.DBConnectionString = dbInfo.GetConnectionString();

            //DatabaseInfo dbInfo = new DatabaseInfo();
            //dbInfo.TypeID = 2;
            //dbInfo.Host = "192.168.6.80";
            //dbInfo.Port = 1433;
            //dbInfo.DBName = "UMPDataDB1026";
            //dbInfo.LoginName = "sa";
            //dbInfo.Password = "Voicecodes123";
            //Session.DatabaseInfo = dbInfo;
            //Session.DBType = dbInfo.TypeID;
            //Session.DBConnectionString = dbInfo.GetConnectionString();

            DatabaseInfo dbInfo = new DatabaseInfo();
            dbInfo.TypeID = 2;
            dbInfo.TypeName = "MSSQL";
            dbInfo.Host = "192.168.4.182";
            dbInfo.Port = 1433;
            dbInfo.DBName = "UMPDataDB0310";
            dbInfo.LoginName = "PFDEV";
            dbInfo.Password = "PF,123";
            Session.DatabaseInfo = dbInfo;
            Session.DBType = dbInfo.TypeID;
            Session.DBConnectionString = dbInfo.GetConnectionString();


            //分表之类的参数
            //Session.ListPartitionTables.Clear();
            //PartitionTableInfo partInfo = new PartitionTableInfo();
            //partInfo.TableName = ConstValue.TABLE_NAME_RECORD;
            //partInfo.PartType = TablePartType.DatetimeRange;
            //partInfo.Other1 = ConstValue.TABLE_FIELD_NAME_RECORD_STARTRECORDTIME;
            //Session.ListPartitionTables.Add(partInfo);
        }

        protected override void Init()
        {
            base.Init();
            ListLanguageInfos = new List<LanguageInfo>();
            ListOperationInfos = new List<OperationInfo>();
            ListCtrolAgentInfos = new List<CtrolAgent>();
            ListCtrolOrgInfos = new List<CtrolOrg>();
            ListCtrolQAInfos = new List<CtrolQA>();
            ListCtrolRealityExtension = new List<CtrolAgent>();
            ListCtrolExtension = new List<CtrolAgent>();
            mListSftpServers = new List<SftpServerInfo>();
            mService03Helper = new Service03Helper();
            mListDownloadParams = new List<DownloadParamInfo>();
            mListRecordEncryptInfos = new List<RecordEncryptInfo>();
            mListAllObjects = new List<ObjectItems>();
            mRootItem = new ObjectItems();

            LoadSftpServerList();
            SetService03Helper();
            LoadRecordEncryptInfos();
            LoadDownloadParamList();

            GroupingWay = string.Empty;
            LoadGroupingMethodParams();
            InitControlObjects();
            //得到所有操作权限
            InitControledOperations("31", "3103");
            S3103App.WriteLog("PageInit", string.Format("Init UserOperation"));
            //得到所能管理的部门管理的座席
            InitControledAgentAndOrg("-1");
            GetAuInfoLists();
            GetAllObjects();

            if (Session != null)
            {
                WriteLog("AppLoad", string.Format("SessionInfo\r\n{0}", Session.LogInfo()));
            }
            InitLanguageInfos();
        }


        #endregion
                
        #region 得到当前用户所管理的部门和座席 1

        //得到当前用户所管理的部门和座席
        public void InitControledAgentAndOrg(string OrgID)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = S3103App.Session;
                webRequest.Code = (int)S3103Codes.GetControlOrgInfoList;
                webRequest.ListData.Add(S3103App.Session.UserID.ToString());
                webRequest.ListData.Add(OrgID);
                //Service31031Client client = new Service31031Client();
                Service31031Client client = new Service31031Client(WebHelper.CreateBasicHttpBinding(S3103App.Session), WebHelper.CreateEndpointAddress(S3103App.Session.AppServerInfo, "Service31031"));
                //WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.UMPTaskOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    S3103App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    S3103App.ShowExceptionMessage(string.Format("Fail.\tListData is null"));
                    return;
                }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[i];
                    string[] arrInfo = strInfo.Split(new[] { ConstValue.SPLITER_CHAR },
                        StringSplitOptions.RemoveEmptyEntries);
                    if (arrInfo.Length < 3) { continue; }
                    CtrolOrg ctrolOrg = new CtrolOrg();
                    ctrolOrg.ID = arrInfo[0];
                    ctrolOrg.OrgName = arrInfo[1];
                    ctrolOrg.OrgParentID = arrInfo[2];

                    if (OrgID.Equals("-1"))
                    {
                        CurrentOrg = ctrolOrg.OrgParentID;
                    }


                    if (ListCtrolOrgInfos.Where(p => p.ID == ctrolOrg.ID).Count() == 0)
                    {
                        ListCtrolOrgInfos.Add(ctrolOrg);
                    }
                    InitControledAgentAndOrg(arrInfo[0]);
                    InitControlAgents(arrInfo[0]);
                    InitControlQA(arrInfo[0]);
                    InitControlRealityExtension(arrInfo[0]);
                    InitControlExtension(arrInfo[0]);
                }
            }
            catch (Exception ex)
            {
                S3103App.ShowExceptionMessage(ex.Message);
            }
        }

        private void InitControlQA(string parentID)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = S3103App.Session;
                webRequest.Code = (int)S3103Codes.GetQA;
                webRequest.ListData.Add(parentID);
                webRequest.ListData.Add(S3103Consts.OPT_TASKRECORDSCORE.ToString());
                //Service31031Client client = new Service31031Client();
                Service31031Client client = new Service31031Client(WebHelper.CreateBasicHttpBinding(S3103App.Session), WebHelper.CreateEndpointAddress(S3103App.Session.AppServerInfo, "Service31031"));
                //WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.UMPTaskOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    S3103App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    S3103App.ShowExceptionMessage(string.Format("Fail.\tListData is null"));
                    return;
                }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[i];
                    OperationReturn optReturn = XMLHelper.DeserializeObject<CtrolQA>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        S3103App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    CtrolQA ctrolQa = optReturn.Data as CtrolQA;

                    if (ctrolQa != null)
                    {
                        ListCtrolQAInfos.Add(ctrolQa);
                    }
                }
            }
            catch (Exception ex)
            {
                S3103App.ShowExceptionMessage(ex.Message);
            }
        }

        private void InitControlAgents(string parentID)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = S3103App.Session;
                webRequest.Code = (int)S3103Codes.GetControlAgentInfoList;
                webRequest.ListData.Add(S3103App.Session.UserID.ToString());
                webRequest.ListData.Add(parentID);
                webRequest.ListData.Add("A");
                //Service31031Client client = new Service31031Client();
                Service31031Client client = new Service31031Client(WebHelper.CreateBasicHttpBinding(S3103App.Session), WebHelper.CreateEndpointAddress(S3103App.Session.AppServerInfo, "Service31031"));
                //WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.UMPTaskOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    S3103App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    S3103App.ShowExceptionMessage(string.Format("Fail.\tListData is null"));
                    return;
                }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[i];
                    string[] arrInfo = strInfo.Split(new[] { ConstValue.SPLITER_CHAR },
                        StringSplitOptions.RemoveEmptyEntries);
                    if (arrInfo.Length < 3) { continue; }

                    CtrolAgent ctrolAgent = new CtrolAgent();
                    ctrolAgent.AgentID = arrInfo[0];
                    ctrolAgent.AgentName = arrInfo[1];
                    ctrolAgent.AgentFullName = arrInfo[2];
                    ctrolAgent.AgentOrgID = parentID;
                    if (ListCtrolAgentInfos.Where(p => p.AgentID == ctrolAgent.AgentID).Count() == 0)
                    {
                        ListCtrolAgentInfos.Add(ctrolAgent);
                    }
                }
            }
            catch (Exception ex)
            {
                S3103App.ShowExceptionMessage(ex.Message);
            }
        }
        
        private void InitControlRealityExtension(string parentID)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = S3103App.Session;
                webRequest.Code = (int)S3103Codes.GetControlAgentInfoList;
                webRequest.ListData.Add(S3103App.Session.UserID.ToString());
                webRequest.ListData.Add(parentID);
                webRequest.ListData.Add("R");
                //Service31031Client client = new Service31031Client();
                Service31031Client client = new Service31031Client(WebHelper.CreateBasicHttpBinding(S3103App.Session), WebHelper.CreateEndpointAddress(S3103App.Session.AppServerInfo, "Service31031"));
                //WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.UMPTaskOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    S3103App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    S3103App.ShowExceptionMessage(string.Format("Fail.\tListData is null"));
                    return;
                }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[i];
                    string[] arrInfo = strInfo.Split(new[] { ConstValue.SPLITER_CHAR },
                        StringSplitOptions.RemoveEmptyEntries);
                    if (arrInfo.Length < 3) { continue; }

                    CtrolAgent ctrolRex = new CtrolAgent();
                    ctrolRex.AgentID = arrInfo[0];
                    ctrolRex.AgentName = arrInfo[1];
                    ctrolRex.AgentFullName = arrInfo[2];
                    ctrolRex.AgentOrgID = parentID;
                    if (ListCtrolRealityExtension.Where(p => p.AgentID == ctrolRex.AgentID).Count() == 0)
                    {
                        ListCtrolRealityExtension.Add(ctrolRex);
                    }
                }
            }
            catch (Exception ex)
            {
                S3103App.ShowExceptionMessage(ex.Message);
            }
        }

        private void InitControlExtension(string parentID)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = S3103App.Session;
                webRequest.Code = (int)S3103Codes.GetControlAgentInfoList;
                webRequest.ListData.Add(S3103App.Session.UserID.ToString());
                webRequest.ListData.Add(parentID);
                webRequest.ListData.Add("E");
                //Service31031Client client = new Service31031Client();
                Service31031Client client = new Service31031Client(WebHelper.CreateBasicHttpBinding(S3103App.Session), WebHelper.CreateEndpointAddress(S3103App.Session.AppServerInfo, "Service31031"));
                //WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.UMPTaskOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    S3103App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    S3103App.ShowExceptionMessage(string.Format("Fail.\tListData is null"));
                    return;
                }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[i];
                    string[] arrInfo = strInfo.Split(new[] { ConstValue.SPLITER_CHAR },
                        StringSplitOptions.RemoveEmptyEntries);
                    if (arrInfo.Length < 3) { continue; }

                    CtrolAgent ctrolEx = new CtrolAgent();
                    ctrolEx.AgentID = arrInfo[0];
                    ctrolEx.AgentName = arrInfo[1];
                    ctrolEx.AgentFullName = arrInfo[2];
                    ctrolEx.AgentOrgID = parentID;
                    if (ListCtrolExtension.Where(p => p.AgentID == ctrolEx.AgentID).Count() == 0)
                    {
                        ListCtrolExtension.Add(ctrolEx);
                    }
                }
            }
            catch (Exception ex)
            {
                S3103App.ShowExceptionMessage(ex.Message);
            }
        }

        /// <summary>
        ///  獲取座席、用戶名
        /// Creater by Waves
        /// </summary>
        public void GetAuInfoLists()
        {
            try
            {
                mListAuInfoItems = new ObservableCollection<AgentAndUserInfoItems>();
                WebRequest webRequest = new WebRequest();
                webRequest.Session = S3103App.Session;
                webRequest.Code = (int)S3103Codes.GetAuInfoList;
                webRequest.ListData.Add("11");
                Service31031Client client = new Service31031Client(WebHelper.CreateBasicHttpBinding(S3103App.Session), WebHelper.CreateEndpointAddress(S3103App.Session.AppServerInfo, "Service31031"));
                WebReturn webReturn = client.UMPTaskOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    S3103App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    S3103App.ShowExceptionMessage("ListData is null");
                    return;
                }
                if (webReturn.ListData.Count <= 0) { return; }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<AgentAndUserInfoList>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        S3103App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        continue;
                    }
                    AgentAndUserInfoList auInfoList = optReturn.Data as AgentAndUserInfoList;
                    if (auInfoList == null)
                    {
                        return;
                    }
                    AgentAndUserInfoItems item = new AgentAndUserInfoItems(auInfoList);
                    item.FullName = DecryptString(item.FullName);
                    item.Name = DecryptString(item.Name);
                    mListAuInfoItems.Add(item);
                }
            }
            catch (Exception ex)
            {
                S3103App.ShowExceptionMessage(ex.Message);
            }
        }

        #endregion

        #region 得到当前用户所管理的部门和座席 2

        private void InitControlObjects()
        {
            InitControlOrgs(mRootItem, "-1");

            S3103App.WriteLog("PageLoad", string.Format("Load ControlObject"));
        }

        private void InitControlOrgs(ObjectItems parentItem, string parentID)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = S3103App.Session;
                webRequest.Code = 10;//(int)S3102Codes.GetControlOrgInfoList;
                webRequest.ListData.Add(S3103App.Session.UserID.ToString());
                webRequest.ListData.Add(parentID);
                Service31021Client client = new Service31021Client(
                    WebHelper.CreateBasicHttpBinding(S3103App.Session),
                    WebHelper.CreateEndpointAddress(
                        S3103App.Session.AppServerInfo,
                        "Service31021"));
                //WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    S3103App.ShowExceptionMessage(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    S3103App.ShowExceptionMessage(string.Format("Fail.\tListData is null"));
                    return;
                }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[i];
                    string[] arrInfo = strInfo.Split(new[] { ConstValue.SPLITER_CHAR },
                        StringSplitOptions.RemoveEmptyEntries);
                    if (arrInfo.Length < 2) { continue; }
                    string strID = arrInfo[0];
                    string strName = arrInfo[1];
                    ObjectItems item = new ObjectItems();
                    item.ObjType = ConstValue.RESOURCE_ORG;
                    item.ObjID = Convert.ToInt64(strID);
                    item.Name = strName;
                    item.Data = strInfo;
                    if (strID == ConstValue.ORG_ROOT.ToString())
                    {
                        item.Icon = "Images/rootorg.ico";
                    }
                    else
                    {
                        item.Icon = "Images/org.ico";
                    }
                    InitControlOrgs(item, strID);
                    InitControlAgents(item, strID);
                    InitControlExtensions(item, strID);
                    AddChildObject(parentItem, item);
                }
            }
            catch (Exception ex)
            {
                S3103App.ShowExceptionMessage(ex.Message);
            }
        }

        private void InitControlExtensions(ObjectItems parentItem, string parentID)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = S3103App.Session;
                webRequest.Code = 23;// (int)S3102Codes.GetControlExtensionInfoList;
                webRequest.ListData.Add(S3103App.Session.UserID.ToString());
                webRequest.ListData.Add(parentID);
                Service31021Client client = new Service31021Client(
                    WebHelper.CreateBasicHttpBinding(S3103App.Session),
                    WebHelper.CreateEndpointAddress(
                        S3103App.Session.AppServerInfo,
                        "Service31021"));
                //WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    S3103App.ShowExceptionMessage(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    S3103App.ShowExceptionMessage(string.Format("Fail.\tListData is null"));
                    return;
                }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[i];
                    string[] arrInfo = strInfo.Split(new[] { ConstValue.SPLITER_CHAR },
                        StringSplitOptions.RemoveEmptyEntries);
                    if (arrInfo.Length < 3) { continue; }
                    string strID = arrInfo[0];
                    string strName = arrInfo[1];
                    string strFullName = arrInfo[2];
                    ObjectItems item = new ObjectItems();
                    item.ObjType = ConstValue.RESOURCE_EXTENSION;
                    item.ObjID = Convert.ToInt64(strID);
                    item.Name = strName;
                    item.Description = strFullName;
                    item.Data = strInfo;
                    item.Icon = "Images/user.ico";
                    AddChildObject(parentItem, item);
                }
            }
            catch (Exception ex)
            {
                S3103App.ShowExceptionMessage(ex.Message);
            }
        }

        private void InitControlAgents(ObjectItems parentItem, string parentID)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = S3103App.Session;
                webRequest.Code = 11;// (int)S3102Codes.GetControlAgentInfoList;
                webRequest.ListData.Add(S3103App.Session.UserID.ToString());
                webRequest.ListData.Add(parentID);
                Service31021Client client = new Service31021Client(
                    WebHelper.CreateBasicHttpBinding(S3103App.Session),
                    WebHelper.CreateEndpointAddress(
                        S3103App.Session.AppServerInfo,
                        "Service31021"));
                //WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    S3103App.ShowExceptionMessage(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    S3103App.ShowExceptionMessage(string.Format("Fail.\tListData is null"));
                    return;
                }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[i];
                    string[] arrInfo = strInfo.Split(new[] { ConstValue.SPLITER_CHAR },
                        StringSplitOptions.RemoveEmptyEntries);
                    if (arrInfo.Length < 3) { continue; }
                    string strID = arrInfo[0];
                    string strName = arrInfo[1];
                    string strFullName = arrInfo[2];
                    ObjectItems item = new ObjectItems();
                    item.ObjType = ConstValue.RESOURCE_AGENT;
                    item.ObjID = Convert.ToInt64(strID);
                    item.Name = strName;
                    item.Description = strFullName;
                    item.Data = strInfo;
                    item.Icon = "Images/agent.ico";
                    AddChildObject(parentItem, item);
                }
            }
            catch (Exception ex)
            {
                S3103App.ShowExceptionMessage(ex.Message);
            }
        }

        private void AddChildObject(ObjectItems parentItem, ObjectItems item)
        {
            Dispatcher.Invoke(new Action(() => parentItem.AddChild(item)));
        }

        private void GetAllObjects()
        {
            try
            {
                mListAllObjects.Clear();
                GetAllObjects(mRootItem);
            }
            catch (Exception ex)
            {
                S3103App.ShowExceptionMessage(ex.Message);
            }
        }

        private void GetAllObjects(ObjectItems parentItem)
        {
            try
            {
                for (int i = 0; i < parentItem.Children.Count; i++)
                {
                    ObjectItems item = parentItem.Children[i] as ObjectItems;
                    if (item != null)
                    {
                        mListAllObjects.Add(item);
                        GetAllObjects(item);
                    }
                }
            }
            catch (Exception ex)
            {
                S3103App.ShowExceptionMessage(ex.Message);
            }
        }

        #endregion
        
        #region 是否添加分机的全局参数
        private void LoadGroupingMethodParams()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)RequestCode.WSGetGlobalParamList;
                webRequest.Session = S3103App.Session;
                webRequest.ListData.Add("11");
                webRequest.ListData.Add("12010401");
                webRequest.ListData.Add("120104");
                Service11012Client client = new Service11012Client(WebHelper.CreateBasicHttpBinding(S3103App.Session),
                    WebHelper.CreateEndpointAddress(S3103App.Session.AppServerInfo, "Service11012"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    S3103App.ShowExceptionMessage(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData.Count <= 0) { return; }
                string str = webReturn.ListData[0];
                str = str.Replace("&#x1B;", "");
                OperationReturn optReturn = XMLHelper.DeserializeObject<GlobalParamInfo>(str);
                if (!optReturn.Result)
                {
                    S3103App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                GlobalParamInfo GlobalParamInfo = optReturn.Data as GlobalParamInfo;
                if (GlobalParamInfo == null) { return; }
                string tempGroupWay = GlobalParamInfo.ParamValue.Substring(8);
                GroupingWay = tempGroupWay;
            }
            catch (Exception ex)
            {
                S3103App.ShowExceptionMessage(ex.Message);
            }
        }

        #endregion


        #region 下载、播放相关

        private void SetService03Helper()
        {
            try
            {
                mService03Helper.HostAddress = S3103App.Session.AppServerInfo.Address;
                if (S3103App.Session.AppServerInfo.SupportHttps)
                {
                    mService03Helper.HostPort = S3103App.Session.AppServerInfo.Port - 4;
                }
                else
                {
                    mService03Helper.HostPort = S3103App.Session.AppServerInfo.Port - 3;
                }
            }
            catch (Exception ex)
            {
                S3103App.ShowExceptionMessage(ex.Message);
            }
        }

        private void LoadRecordEncryptInfos()
        {
            try
            {
                mListRecordEncryptInfos.Clear();
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)RequestCode.WSGetUserParamList;
                webRequest.Session = S3103App.Session;
                webRequest.ListData.Add(S3103App.Session.UserInfo.UserID.ToString());
                webRequest.ListData.Add("1");
                webRequest.ListData.Add(S3103Consts.USER_PARAM_GROUP_ENCRYPTINFO.ToString());
                //S3103App.MonitorHelper.AddWebRequest(webRequest);
                Service11012Client client = new Service11012Client(WebHelper.CreateBasicHttpBinding(S3103App.Session),
                    WebHelper.CreateEndpointAddress(S3103App.Session.AppServerInfo, "Service11012"));
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.DoOperation(webRequest);
                //S3103App.MonitorHelper.AddWebReturn(webReturn);
                client.Close();
                if (!webReturn.Result)
                {
                    S3103App.ShowExceptionMessage(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    S3103App.ShowExceptionMessage(string.Format("WSFail.\tListData is null"));
                    return;
                }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<UserParamInfo>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        S3103App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    UserParamInfo info = optReturn.Data as UserParamInfo;
                    if (info != null)
                    {
                        int paramID = info.ParamID;
                        string strValue = info.ParamValue;
                        string[] arrValue = strValue.Split(new[] { ConstValue.SPLITER_CHAR_3 }, StringSplitOptions.None);
                        string strAddress = string.Empty;
                        string strPassword = string.Empty;
                        string strExpireTime = string.Empty;
                        if (arrValue.Length > 0)
                        {
                            strAddress = arrValue[0];
                        }
                        if (arrValue.Length > 1)
                        {
                            strPassword = arrValue[1];
                        }
                        if (arrValue.Length > 2)
                        {
                            strExpireTime = arrValue[2];
                        }
                        DateTime dtExpireTime = Converter.NumberToDatetime(strExpireTime);
                        if (string.IsNullOrEmpty(strAddress)
                            || string.IsNullOrEmpty(strPassword))
                        {
                            S3103App.WriteLog("LoadEncryptInfo", string.Format("Fail.\tEncryptInfo invalid."));
                            continue;
                        }
                        if (paramID > S3103Consts.USER_PARAM_GROUP_ENCRYPTINFO * 1000
                            && paramID < (S3103Consts.USER_PARAM_GROUP_ENCRYPTINFO + 1) * 1000
                            && dtExpireTime > DateTime.Now.ToUniversalTime())
                        {
                            RecordEncryptInfo encryptInfo = new RecordEncryptInfo();
                            encryptInfo.UserID = S3103App.Session.UserID;
                            encryptInfo.ServerAddress = strAddress;
                            encryptInfo.Password = strPassword;
                            encryptInfo.EndTime = dtExpireTime;
                            mListRecordEncryptInfos.Add(encryptInfo);
                        }
                    }
                }

                S3103App.WriteLog("PageLoad", string.Format("Init RecordEncryptInfo."));
            }
            catch (Exception ex)
            {
                S3103App.ShowExceptionMessage(ex.Message);
            }
        }

        private void LoadSftpServerList()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = S3103App.Session;
                webRequest.Code = 33;// (int)S3102Codes.GetSftpServerList
                webRequest.ListData.Add(S3103App.Session.UserID.ToString());
                Service31021Client client = new Service31021Client(
                    WebHelper.CreateBasicHttpBinding(S3103App.Session),
                    WebHelper.CreateEndpointAddress(
                        S3103App.Session.AppServerInfo,
                        "Service31021"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    S3103App.ShowExceptionMessage(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    S3103App.ShowExceptionMessage(string.Format("Fail.\tListData is null"));
                    return;
                }
                mListSftpServers.Clear();
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[i];
                    OperationReturn optReturn = XMLHelper.DeserializeObject<SftpServerInfo>(strInfo);
                    if (!optReturn.Result)
                    {
                        S3103App.WriteLog("LoadSftp", string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        continue;
                    }
                    SftpServerInfo sftpInfo = optReturn.Data as SftpServerInfo;
                    if (sftpInfo == null)
                    {
                        S3103App.WriteLog("LoadSftp", string.Format("Fail.\tSftpServerInfo is null"));
                        continue;
                    }
                    mListSftpServers.Add(sftpInfo);
                }

                S3103App.WriteLog("PageLoad", string.Format("Load SftpServerInfo"));
            }
            catch (Exception ex)
            {
                S3103App.ShowExceptionMessage(ex.Message);
            }
        }

        private void LoadDownloadParamList()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = S3103App.Session;
                webRequest.Code = 34;// (int)S3102Codes.GetDownloadParamList;
                webRequest.ListData.Add(S3103App.Session.UserID.ToString());
                Service31021Client client = new Service31021Client(
                    WebHelper.CreateBasicHttpBinding(S3103App.Session),
                    WebHelper.CreateEndpointAddress(
                        S3103App.Session.AppServerInfo,
                        "Service31021"));
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    S3103App.ShowExceptionMessage(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    S3103App.ShowExceptionMessage(string.Format("Fail.\tListData is null"));
                    return;
                }
                mListDownloadParams.Clear();
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[i];
                    OperationReturn optReturn = XMLHelper.DeserializeObject<DownloadParamInfo>(strInfo);
                    if (!optReturn.Result)
                    {
                        S3103App.WriteLog("LoadSftp", string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        continue;
                    }
                    DownloadParamInfo info = optReturn.Data as DownloadParamInfo;
                    if (info == null)
                    {
                        S3103App.WriteLog("LoadSftp", string.Format("Fail.\tSftpServerInfo is null"));
                        continue;
                    }
                    mListDownloadParams.Add(info);
                }

                S3103App.WriteLog("PageLoad", string.Format("Load DownloadParams"));
            }
            catch (Exception ex)
            {
                S3103App.ShowExceptionMessage(ex.Message);
            }
        }

        #endregion

        //得到当前用所有角色的权限并集
        public void InitControledOperations(string modelId, string parentId)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S3103Codes.GetUserOperationList;
                //webRequest.Code = (int)RequestCode.WSGetUserOptList;
                webRequest.Session = S3103App.Session;
                webRequest.ListData.Add(S3103App.Session.UserInfo.UserID.ToString());
                webRequest.ListData.Add(modelId);
                webRequest.ListData.Add(parentId);

                //Service11012Client client = new Service31031Client();
                Service31031Client client = new Service31031Client(WebHelper.CreateBasicHttpBinding(S3103App.Session), WebHelper.CreateEndpointAddress(S3103App.Session.AppServerInfo, "Service31031"));
                //WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.UMPTaskOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    S3103App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                ListOperationInfos.Clear();
                if (webReturn.ListData.Count > 0)
                {
                    for (int i = 0; i < webReturn.ListData.Count; i++)
                    {
                        OperationReturn optReturn = XMLHelper.DeserializeObject<OperationInfo>(webReturn.ListData[i]);
                        if (!optReturn.Result)
                        {
                            S3103App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                            return;
                        }
                        OperationInfo optInfo = optReturn.Data as OperationInfo;
                        if (optInfo != null)
                        {
                            optInfo.Display = S3103App.GetLanguageInfo(string.Format("FO{0}", optInfo.ID), optInfo.ID.ToString());
                            optInfo.Description = optInfo.Display;
                            ListOperationInfos.Add(optInfo);

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                S3103App.ShowExceptionMessage(ex.Message);
            }
        }

        protected override void InitLanguageInfos()
        {
            base.InitLanguageInfos();
            try
            {
                if (Session == null || Session.LangTypeInfo == null)
                {
                    return;
                }
                //ListLanguageInfos.Clear();
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)RequestCode.WSGetLangList;
                webRequest.Session = Session;
                //ListParams
                //0     LangID
                //1     PreName（语言内容编码的前缀，比如 FO:模块、操作显示语言）
                //1     ModuleID
                //2     SubModuleID
                //3     Page
                //4     Name
                webRequest.ListData.Add(Session.LangTypeInfo.LangID.ToString());
                webRequest.ListData.Add(string.Empty);
                webRequest.ListData.Add("31");
                webRequest.ListData.Add("3103");
                webRequest.ListData.Add(string.Empty);
                webRequest.ListData.Add(string.Empty);
                Service11012Client client = new Service11012Client(WebHelper.CreateBasicHttpBinding(S3103App.Session), WebHelper.CreateEndpointAddress(Session.AppServerInfo, "Service11012"));
                //Service11012Client client = new Service11012Client();
                //WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowExceptionMessage(string.Format("{0}\t{1}", webReturn.Code, webReturn.Message));
                }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<LanguageInfo>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowExceptionMessage(string.Format("{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    LanguageInfo langInfo = optReturn.Data as LanguageInfo;
                    if (langInfo == null)
                    {
                        ShowExceptionMessage(string.Format("LanguageInfo is null"));
                        return;
                    }
                    ListLanguageInfos.Add(langInfo);
                }

                WriteLog(string.Format("AppStart\t\tLanguage loaded"));
            }
            catch (Exception ex)
            {
                //ShowExceptionMessage(ex.Message);
            }
        }


        #region Encryption and Decryption
        /// <summary>
        /// 加密
        /// </summary>
        public static string EncryptString(string strSource)
        {
            string strTemp = EncryptionAndDecryption.EncryptDecryptString(strSource,
             CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004),
             EncryptionAndDecryption.UMPKeyAndIVType.M004);
            return strTemp;
        }
        /// <summary>
        /// 解密
        /// </summary>
        public static string DecryptString(string strSource)
        {
            string strTemp = EncryptionAndDecryption.EncryptDecryptString(strSource,
              CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104),
              EncryptionAndDecryption.UMPKeyAndIVType.M104);
            return strTemp;
        }

        public static string CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType aKeyIVID)
        {
            string lStrReturn;
            int LIntRand;
            Random lRandom = new Random();
            string LStrTemp;

            try
            {
                lStrReturn = DateTime.Now.ToString("yyyyMMddHHmmss");
                LIntRand = lRandom.Next(0, 14);
                LStrTemp = LIntRand.ToString("00");
                lStrReturn = lStrReturn.Insert(LIntRand, "VCT");
                LIntRand = lRandom.Next(0, 17);
                LStrTemp += LIntRand.ToString("00");
                lStrReturn = lStrReturn.Insert(LIntRand, "UMP");
                LIntRand = lRandom.Next(0, 20);
                LStrTemp += LIntRand.ToString("00");
                lStrReturn = lStrReturn.Insert(LIntRand, ((int)aKeyIVID).ToString("000"));

                lStrReturn = EncryptionAndDecryption.EncryptStringY(LStrTemp + lStrReturn);
            }
            catch { lStrReturn = string.Empty; }

            return lStrReturn;
        }

        #endregion
        **/
        #endregion
    }
}
