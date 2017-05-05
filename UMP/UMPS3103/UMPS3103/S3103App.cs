using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using PFShareClassesC;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using UMPS3103.Codes;
using UMPS3103.DoubleTask;
using UMPS3103.Models;
using UMPS3103.Wcf11012;
using UMPS3103.Wcf31021;
using UMPS3103.Wcf31031;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common31031;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;

namespace UMPS3103
{
    class S3103App : UMPApp
    {
        public S3103App(bool runAsModule)
            : base(runAsModule)
        {

        }                  
        public S3103App(IRegionManager regionManager,
            IEventAggregator eventAggregator,
            IAppControlService appController)
            : base(regionManager, eventAggregator, appController)
        {

        }

        #region Members

        //当前全部操作权限
        public static List<OperationInfo> ListOperationInfos;

        //当前人管理全部部门
        public static List<CtrolOrg> ListCtrolOrgInfos;

        /// <summary>
        /// 当前人管理的全部座席
        /// </summary>
        public static List<CtrolAgent> ListCtrolAgentInfos;

        /// <summary>
        /// 当前人管理的全部真实分机
        /// </summary>
        public static List<CtrolAgent> ListCtrolRealityExtension;

        /// <summary>
        /// 当前人管理的全部虚拟分机
        /// </summary>
        public static List<CtrolAgent> ListCtrolExtension;

        /// <summary>
        /// 当前用户管理的用户
        /// </summary>
        public static List<CtrolQA> ListCtrolQA;


        /// <summary>
        /// 分组管理方式，E虚拟分机，A座席，R真实分机；
        /// </summary>
        public static string GroupingWay;

        /// <summary>
        /// 全部QA
        /// </summary>
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

        public static List<GlobalParamInfo> ListAppealTaskParam;

        //从多个页面跳转里要用的
        public static bool NativePageFlag = true;

        #endregion

        #region Init & Load Info
        protected override void SetAppInfo()
        {
            base.SetAppInfo();

            AppName = "UMPS3103";
            AppTitle = string.Format("Task Management");
            ModuleID = 3103;
            AppType = (int)VoiceCyber.UMP.Common.AppType.UMPClient;

        }

        protected override void SetView()
        {
            base.SetView();

            CurrentView = new TaskAssign();
            CurrentView.PageName = "TaskAssign";
        }

        private void SetTaskView()
        {
            if (ListOperationInfos.Where(p => p.ID == S3103Consts.OPT_TASKASSIGN).Count() > 0)
            {
                CurrentView = new TaskAssign();
                CurrentView.PageName = "TaskAssign";
            }
            else
            {
                CurrentView = new TaskTrack();
                CurrentView.PageName = "TaskTrack";
            }
        }

        public void InitMainView(UMPUserControl view)
        {
            CurrentView = view;
            if (RunAsModule)
            {
                InitView();
            }
            else
            {
                var app = App.Current;
                if (app == null) { return; }
                CurrentView.CurrentApp = App.CurrentApp;
                var window = app.MainWindow;
                if (window == null) { return; }
                var shell = window.Content as Shell;
                if (shell == null) { return; }
                shell.SetView(CurrentView);
            }
        }


        protected override void InitSessionInfo()
        {
            base.InitSessionInfo();

            if (Session == null) { return; }

            UserInfo userInfo = new UserInfo();
            userInfo.UserID = ConstValue.USER_ADMIN;
            //userInfo.UserID = 1021607131000000004;
            userInfo.UserName = "administrator";
            //userInfo.Account = "qa3";
            userInfo.Password = "111111";
            Session.UserInfo = userInfo;
            Session.UserID = ConstValue.USER_ADMIN;
            //Session.UserID = 1021607131000000004;

            //UserInfo userInfo = new UserInfo();
            //userInfo.UserID = ConstValue.USER_ADMIN;
            //userInfo.Account = "administrator";
            //userInfo.UserName = "Administrator";
            //userInfo.Password = "voicecyber";
            //Session.UserInfo = userInfo;
            //Session.UserID = ConstValue.USER_ADMIN;


            AppServerInfo serverInfo = new AppServerInfo();
            serverInfo.Protocol = "http";
            serverInfo.Address = "192.168.4.166";
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

            DatabaseInfo dbInfo = new DatabaseInfo();
            dbInfo.TypeID = 2;
            dbInfo.Host = "192.168.4.182";
            dbInfo.Port = 1433;
            dbInfo.DBName = "UMPDataDB1214";
            dbInfo.LoginName = "sa";
            dbInfo.Password = "PF,123";
            Session.DatabaseInfo = dbInfo;
            Session.DBType = dbInfo.TypeID;
            Session.DBConnectionString = dbInfo.GetConnectionString();

            //DatabaseInfo dbInfo = new DatabaseInfo();
            //dbInfo.TypeID = 2;
            //dbInfo.TypeName = "MSSQL";
            //dbInfo.Host = "192.168.9.236";
            //dbInfo.Port = 1433;
            //dbInfo.DBName = "UMPDataDB_taiwan";
            //dbInfo.LoginName = "sa";
            //dbInfo.Password = "voicecodes";
            //Session.DatabaseInfo = dbInfo;
            //Session.DBType = dbInfo.TypeID;
            //Session.DBConnectionString = dbInfo.GetConnectionString();

            //DatabaseInfo dbInfo = new DatabaseInfo();
            //dbInfo.TypeID = 2;
            //dbInfo.TypeName = "MSSQL";
            //dbInfo.Host = "192.168.9.223";
            //dbInfo.Port = 1433;
            //dbInfo.DBName = "UMPDataDB0711";
            //dbInfo.LoginName = "sa";
            //dbInfo.Password = "voicecodes";
            //Session.DatabaseInfo = dbInfo;
            //Session.DBType = dbInfo.TypeID;
            //Session.DBConnectionString = dbInfo.GetConnectionString();


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
         
            ListLanguageInfos = new List<LanguageInfo>();
            ListOperationInfos = new List<OperationInfo>();
            ListCtrolAgentInfos = new List<CtrolAgent>();
            ListCtrolOrgInfos = new List<CtrolOrg>();
            ListCtrolQAInfos = new List<CtrolQA>();
            ListCtrolQA = new List<CtrolQA>();
            ListCtrolRealityExtension = new List<CtrolAgent>();
            ListCtrolExtension = new List<CtrolAgent>();
            mListSftpServers = new List<SftpServerInfo>();
            mService03Helper = new Service03Helper();
            mListDownloadParams = new List<DownloadParamInfo>();
            mListRecordEncryptInfos = new List<RecordEncryptInfo>();
            mListAllObjects = new List<ObjectItems>();
            mRootItem = new ObjectItems();
            ListAppealTaskParam = new List<GlobalParamInfo>();

            base.Init();

            LoadSftpServerList();
            SetService03Helper();
            LoadRecordEncryptInfos();
            LoadDownloadParamList();

            GroupingWay = string.Empty;
            LoadGroupingMethodParams();
            TaskParams();
            InitControlObjects();
            //得到所有操作权限
            InitControledOperations("31", "3103");
            WriteLog("PageInit", string.Format("Init UserOperation"));
            //得到该用户所属的部门以及该部门所属的用户、座席 
            InitControledAgentAndOrg("-1");
            GetAuInfoLists();
            GetAllObjects();
            InitControlQA();

            if (Session != null)
            {
                WriteLog("AppLoad", string.Format("SessionInfo\r\n{0}", Session.LogInfo()));
            }
            //InitLanguageInfos();
            //SetTaskView();

          
        }


        #endregion

        #region 得到该用户所属的部门以及该部门所属的用户、座席 
        
        public void InitControledAgentAndOrg(string OrgID)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = Session;
                webRequest.Code = (int)S3103Codes.GetControlOrgInfoList;
                webRequest.ListData.Add(Session.UserID.ToString());
                webRequest.ListData.Add(OrgID);
                //Service31031Client client = new Service31031Client();
                Service31031Client client = new Service31031Client(WebHelper.CreateBasicHttpBinding(Session), WebHelper.CreateEndpointAddress(Session.AppServerInfo, "Service31031"));
                //WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.UMPTaskOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    ShowExceptionMessage(string.Format("Fail.\tListData is null"));
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
                ShowExceptionMessage(ex.Message);
            }
        }

        private void InitControlQA(string parentID)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = Session;
                webRequest.Code = (int)S3103Codes.GetQA;
                webRequest.ListData.Add(parentID);
                webRequest.ListData.Add(S3103Consts.OPT_TASKRECORDSCORE.ToString());
                //Service31031Client client = new Service31031Client();
                Service31031Client client = new Service31031Client(WebHelper.CreateBasicHttpBinding(Session), WebHelper.CreateEndpointAddress(Session.AppServerInfo, "Service31031"));
                //WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.UMPTaskOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    ShowExceptionMessage(string.Format("Fail.\tListData is null"));
                    return;
                }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[i];
                    OperationReturn optReturn = XMLHelper.DeserializeObject<CtrolQA>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
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
                ShowExceptionMessage(ex.Message);
            }
        }

        private void InitControlAgents(string parentID)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = Session;
                webRequest.Code = (int)S3103Codes.GetControlAgentInfoList;
                webRequest.ListData.Add(Session.UserID.ToString());
                webRequest.ListData.Add(parentID);
                webRequest.ListData.Add("A");
                //Service31031Client client = new Service31031Client();
                Service31031Client client = new Service31031Client(WebHelper.CreateBasicHttpBinding(Session), WebHelper.CreateEndpointAddress(Session.AppServerInfo, "Service31031"));
                //WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.UMPTaskOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    ShowExceptionMessage(string.Format("Fail.\tListData is null"));
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
                        if (ctrolAgent.AgentName.ToUpper() != "N/A")
                        {
                            ListCtrolAgentInfos.Add(ctrolAgent);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowExceptionMessage(ex.Message);
            }
        }

        private void InitControlRealityExtension(string parentID)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = Session;
                webRequest.Code = (int)S3103Codes.GetControlAgentInfoList;
                webRequest.ListData.Add(Session.UserID.ToString());
                webRequest.ListData.Add(parentID);
                webRequest.ListData.Add("R");
                //Service31031Client client = new Service31031Client();
                Service31031Client client = new Service31031Client(WebHelper.CreateBasicHttpBinding(Session), WebHelper.CreateEndpointAddress(Session.AppServerInfo, "Service31031"));
                //WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.UMPTaskOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    ShowExceptionMessage(string.Format("Fail.\tListData is null"));
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
                ShowExceptionMessage(ex.Message);
            }
        }

        private void InitControlExtension(string parentID)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = Session;
                webRequest.Code = (int)S3103Codes.GetControlAgentInfoList;
                webRequest.ListData.Add(Session.UserID.ToString());
                webRequest.ListData.Add(parentID);
                webRequest.ListData.Add("E");
                //Service31031Client client = new Service31031Client();
                Service31031Client client = new Service31031Client(WebHelper.CreateBasicHttpBinding(Session), WebHelper.CreateEndpointAddress(Session.AppServerInfo, "Service31031"));
                //WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.UMPTaskOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    ShowExceptionMessage(string.Format("Fail.\tListData is null"));
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
                ShowExceptionMessage(ex.Message);
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
                webRequest.Session = Session;
                webRequest.Code = (int)S3103Codes.GetAuInfoList;
                webRequest.ListData.Add("11");
                Service31031Client client = new Service31031Client(WebHelper.CreateBasicHttpBinding(Session), WebHelper.CreateEndpointAddress(Session.AppServerInfo, "Service31031"));
                WebReturn webReturn = client.UMPTaskOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    ShowExceptionMessage("ListData is null");
                    return;
                }
                if (webReturn.ListData.Count <= 0) { return; }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<AgentAndUserInfoList>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
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
                ShowExceptionMessage(ex.Message);
            }
        }

        #endregion

        #region 得到当前用户所管理的部门和座席 2

        private void InitControlObjects()
        {
            InitControlOrgs(mRootItem, "-1");

            WriteLog("PageLoad", string.Format("Load ControlObject"));
        }

        private void InitControlOrgs(ObjectItems parentItem, string parentID)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = Session;
                webRequest.Code = 10;//(int)S3102Codes.GetControlOrgInfoList;
                webRequest.ListData.Add(Session.UserID.ToString());
                webRequest.ListData.Add(parentID);
                Service31021Client client = new Service31021Client(
                    WebHelper.CreateBasicHttpBinding(Session),
                    WebHelper.CreateEndpointAddress(
                        Session.AppServerInfo,
                        "Service31021"));
                //WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowExceptionMessage(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    ShowExceptionMessage(string.Format("Fail.\tListData is null"));
                    return;
                }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[i];
                    string[] arrInfo = strInfo.Split(new[] { ConstValue.SPLITER_CHAR },
                        StringSplitOptions.RemoveEmptyEntries);
                    if (arrInfo.Length < 2) { continue; }
                    string strID = arrInfo[1];
                    string strName = arrInfo[0];
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
                    parentItem.AddChild(item);
                }
            }
            catch (Exception ex)
            {
                ShowExceptionMessage(ex.Message);
            }
        }

        private void InitControlExtensions(ObjectItems parentItem, string parentID)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = Session;
                webRequest.Code = 23;// (int)S3102Codes.GetControlExtensionInfoList;
                webRequest.ListData.Add(Session.UserID.ToString());
                webRequest.ListData.Add(parentID);
                Service31021Client client = new Service31021Client(
                    WebHelper.CreateBasicHttpBinding(Session),
                    WebHelper.CreateEndpointAddress(
                        Session.AppServerInfo,
                        "Service31021"));
                //WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowExceptionMessage(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    ShowExceptionMessage(string.Format("Fail.\tListData is null"));
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
                    parentItem.AddChild(item);
                }
            }
            catch (Exception ex)
            {
                ShowExceptionMessage(ex.Message);
            }
        }

        private void InitControlAgents(ObjectItems parentItem, string parentID)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = Session;
                webRequest.Code = 11;// (int)S3102Codes.GetControlAgentInfoList;
                webRequest.ListData.Add(Session.UserID.ToString());
                webRequest.ListData.Add(parentID);
                Service31021Client client = new Service31021Client(
                    WebHelper.CreateBasicHttpBinding(Session),
                    WebHelper.CreateEndpointAddress(
                        Session.AppServerInfo,
                        "Service31021"));
                //WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowExceptionMessage(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    ShowExceptionMessage(string.Format("Fail.\tListData is null"));
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
                    parentItem.AddChild(item);
                }
            }
            catch (Exception ex)
            {
                ShowExceptionMessage(ex.Message);
            }
        }

        private void InitControlQA()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = Session;
                webRequest.Code = (int)S3103Codes.GetCtrolQA;
                webRequest.ListData.Add(Session.UserID.ToString());
                Service31031Client client = new Service31031Client(WebHelper.CreateBasicHttpBinding(Session),WebHelper.CreateEndpointAddress(Session.AppServerInfo,"Service31031"));
                //WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.UMPTaskOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowExceptionMessage(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    ShowExceptionMessage(string.Format("Fail.\tListData is null"));
                    return;
                }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[i];
                    OperationReturn optReturn = XMLHelper.DeserializeObject<CtrolQA>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    CtrolQA ctrolQa = optReturn.Data as CtrolQA;

                    if (ctrolQa != null)
                    {
                        ListCtrolQA.Add(ctrolQa);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowExceptionMessage(ex.Message);
            }
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
                ShowExceptionMessage(ex.Message);
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
                ShowExceptionMessage(ex.Message);
            }
        }

        #endregion

        #region 全局参数
        /// <summary>
        /// 是否添加分机的
        /// </summary>
        private void LoadGroupingMethodParams()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)RequestCode.WSGetGlobalParamList;
                webRequest.Session = Session;
                webRequest.ListData.Add("11");
                webRequest.ListData.Add("12010401");
                webRequest.ListData.Add("120104");
                Service11012Client client = new Service11012Client(WebHelper.CreateBasicHttpBinding(Session),
                    WebHelper.CreateEndpointAddress(Session.AppServerInfo, "Service11012"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowExceptionMessage(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData.Count <= 0) { return; }
                string str = webReturn.ListData[0];
                str = str.Replace("&#x1B;", "");
                OperationReturn optReturn = XMLHelper.DeserializeObject<GlobalParamInfo>(str);
                if (!optReturn.Result)
                {
                    ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                GlobalParamInfo GlobalParamInfo = optReturn.Data as GlobalParamInfo;
                if (GlobalParamInfo == null) { return; }
                string tempGroupWay = GlobalParamInfo.ParamValue.Substring(8);
                GroupingWay = tempGroupWay;
            }
            catch (Exception ex)
            {
                ShowExceptionMessage(ex.Message);
            }
        }

        private void TaskParams()
        {
            ListAppealTaskParam.Clear();
            try
            {
                List<string> tempList = new List<string>();
                tempList.Add("310102");
                tempList.Add("310103");
                tempList.Add("310104");
                foreach (string tempStr in tempList)
                {
                    WebRequest webRequest = new WebRequest();
                    webRequest.Session = Session;
                    webRequest.Code = (int)RequestCode.WSGetGlobalParamList;
                    webRequest.ListData.Add("31");
                    webRequest.ListData.Add("0");
                    webRequest.ListData.Add(tempStr);
                    Service11012Client client = new Service11012Client(WebHelper.CreateBasicHttpBinding(Session), WebHelper.CreateEndpointAddress(Session.AppServerInfo, "Service11012"));
                    //WebHelper.SetServiceClient(client);
                    WebReturn webReturn = client.DoOperation(webRequest);
                    client.Close();
                    if (!webReturn.Result)
                    {
                        ShowExceptionMessage(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    }

                    for (int i = 0; i < webReturn.ListData.Count; i++)
                    {
                        OperationReturn optReturn = XMLHelper.DeserializeObject<GlobalParamInfo>(webReturn.ListData[i]);
                        if (!optReturn.Result)
                        {
                            ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        }
                        GlobalParamInfo GlobalParamInfo = optReturn.Data as GlobalParamInfo;
                        ListAppealTaskParam.Add(GlobalParamInfo);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowExceptionMessage(ex.Message);
            }
        }
        #endregion


        #region 下载、播放相关

        private void SetService03Helper()
        {
            try
            {
                mService03Helper.Debug += (mode,cat, msg) => WriteLog("category", string.Format("{0}\t{1}", cat, msg));
                mService03Helper.HostAddress = Session.AppServerInfo.Address;
                if (Session.AppServerInfo.SupportHttps)
                {
                    mService03Helper.HostPort = Session.AppServerInfo.Port - 4;
                }
                else
                {
                    mService03Helper.HostPort = Session.AppServerInfo.Port - 3;
                }
            }
            catch (Exception ex)
            {
                ShowExceptionMessage(ex.Message);
            }
        }

        private void LoadRecordEncryptInfos()
        {
            try
            {
                mListRecordEncryptInfos.Clear();
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)RequestCode.WSGetUserParamList;
                webRequest.Session = Session;
                webRequest.ListData.Add(Session.UserInfo.UserID.ToString());
                webRequest.ListData.Add("1");
                webRequest.ListData.Add(S3103Consts.USER_PARAM_GROUP_ENCRYPTINFO.ToString());
                //MonitorHelper.AddWebRequest(webRequest);
                Service11012Client client = new Service11012Client(WebHelper.CreateBasicHttpBinding(Session),
                    WebHelper.CreateEndpointAddress(Session.AppServerInfo, "Service11012"));
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.DoOperation(webRequest);
                //MonitorHelper.AddWebReturn(webReturn);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowExceptionMessage(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    ShowExceptionMessage(string.Format("WSFail.\tListData is null"));
                    return;
                }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<UserParamInfo>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
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
                            WriteLog("LoadEncryptInfo", string.Format("Fail.\tEncryptInfo invalid."));
                            continue;
                        }
                        if (paramID > S3103Consts.USER_PARAM_GROUP_ENCRYPTINFO * 1000
                            && paramID < (S3103Consts.USER_PARAM_GROUP_ENCRYPTINFO + 1) * 1000
                            && dtExpireTime > DateTime.Now.ToUniversalTime())
                        {
                            RecordEncryptInfo encryptInfo = new RecordEncryptInfo();
                            encryptInfo.UserID = Session.UserID;
                            encryptInfo.ServerAddress = strAddress;
                            encryptInfo.Password = strPassword;
                            encryptInfo.EndTime = dtExpireTime;
                            mListRecordEncryptInfos.Add(encryptInfo);
                        }
                    }
                }

                WriteLog("PageLoad", string.Format("Init RecordEncryptInfo."));
            }
            catch (Exception ex)
            {
                ShowExceptionMessage(ex.Message);
            }
        }

        private void LoadSftpServerList()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = Session;
                webRequest.Code = 33;// (int)S3102Codes.GetSftpServerList
                webRequest.ListData.Add(Session.UserID.ToString());
                Service31021Client client = new Service31021Client(
                    WebHelper.CreateBasicHttpBinding(Session),
                    WebHelper.CreateEndpointAddress(
                        Session.AppServerInfo,
                        "Service31021"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowExceptionMessage(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    ShowExceptionMessage(string.Format("Fail.\tListData is null"));
                    return;
                }
                mListSftpServers.Clear();
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[i];
                    OperationReturn optReturn = XMLHelper.DeserializeObject<SftpServerInfo>(strInfo);
                    if (!optReturn.Result)
                    {
                        WriteLog("LoadSftp", string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        continue;
                    }
                    SftpServerInfo sftpInfo = optReturn.Data as SftpServerInfo;
                    if (sftpInfo == null)
                    {
                        WriteLog("LoadSftp", string.Format("Fail.\tSftpServerInfo is null"));
                        continue;
                    }
                    mListSftpServers.Add(sftpInfo);
                }

                WriteLog("PageLoad", string.Format("Load SftpServerInfo"));
            }
            catch (Exception ex)
            {
                ShowExceptionMessage(ex.Message);
            }
        }

        private void LoadDownloadParamList()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = Session;
                webRequest.Code = 34;// (int)S3102Codes.GetDownloadParamList;
                webRequest.ListData.Add(Session.UserID.ToString());
                Service31021Client client = new Service31021Client(
                    WebHelper.CreateBasicHttpBinding(Session),
                    WebHelper.CreateEndpointAddress(
                        Session.AppServerInfo,
                        "Service31021"));
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowExceptionMessage(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    ShowExceptionMessage(string.Format("Fail.\tListData is null"));
                    return;
                }
                mListDownloadParams.Clear();
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[i];
                    OperationReturn optReturn = XMLHelper.DeserializeObject<DownloadParamInfo>(strInfo);
                    if (!optReturn.Result)
                    {
                        WriteLog("LoadSftp", string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        continue;
                    }
                    DownloadParamInfo info = optReturn.Data as DownloadParamInfo;
                    if (info == null)
                    {
                        WriteLog("LoadSftp", string.Format("Fail.\tSftpServerInfo is null"));
                        continue;
                    }
                    mListDownloadParams.Add(info);
                }

                WriteLog("PageLoad", string.Format("Load DownloadParams"));
            }
            catch (Exception ex)
            {
                ShowExceptionMessage(ex.Message);
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
                webRequest.Session = Session;
                webRequest.ListData.Add(Session.UserInfo.UserID.ToString());
                webRequest.ListData.Add(modelId);
                webRequest.ListData.Add(parentId);

                //Service11012Client client = new Service31031Client();
                Service31031Client client = new Service31031Client(WebHelper.CreateBasicHttpBinding(Session), WebHelper.CreateEndpointAddress(Session.AppServerInfo, "Service31031"));
                //WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.UMPTaskOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
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
                            ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                            return;
                        }
                        OperationInfo optInfo = optReturn.Data as OperationInfo;
                        if (optInfo != null)
                        {
                            optInfo.Display = GetLanguageInfo(string.Format("FO{0}", optInfo.ID), optInfo.ID.ToString());
                            optInfo.Description = optInfo.Display;
                            ListOperationInfos.Add(optInfo);

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowExceptionMessage(ex.Message);
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
                Service11012Client client = new Service11012Client(WebHelper.CreateBasicHttpBinding(Session), WebHelper.CreateEndpointAddress(Session.AppServerInfo, "Service11012"));
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
    }
}
