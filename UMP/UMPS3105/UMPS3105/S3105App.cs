using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Common3105;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using PFShareClassesC;
using UMPS3105.Codes;
using UMPS3105.Wcf11012;
using UMPS3105.Wcf31021;
using UMPS3105.Wcf31031;
using UMPS3105.Wcf31051;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;

namespace UMPS3105
{
    public class S3105App : UMPApp
    {
        public S3105App(bool runAsModule)
            : base(runAsModule)
        {

        }

        public S3105App(IRegionManager regionManager, IEventAggregator eventAggregator, IAppControlService appController)
            : base(regionManager, eventAggregator, appController)
        {

        }



        protected override void SetAppInfo()
        {
            base.SetAppInfo();

            AppName = "UMPS3105";
            AppTitle = string.Format("Appeal Management");
            ModuleID = 3105;
            AppType = (int)VoiceCyber.UMP.Common.AppType.UMPClient;
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

            AppServerInfo appServerInfo = new AppServerInfo();
            appServerInfo.Protocol = "http";
            appServerInfo.Address = "192.168.4.166";
            appServerInfo.Port = 8081;
            appServerInfo.SupportHttps = false;
            appServerInfo.SupportNetTcp = false;
            Session.AppServerInfo = appServerInfo;

            UserInfo userInfo = new UserInfo();
            userInfo.UserID = ConstValue.USER_ADMIN;
            //userInfo.UserID = 1021605171400000001;
            //userInfo.Account = "qa1";
            userInfo.UserName = "a";
            userInfo.Password = "voicecyber";
            Session.UserInfo = userInfo;
            Session.UserID = ConstValue.USER_ADMIN;
            //Session.UserID = 1021605171400000001;

            DatabaseInfo dbInfo = new DatabaseInfo();
            dbInfo.TypeID = 2;
            dbInfo.TypeName = "MSSQL";
            dbInfo.Host = "192.168.4.182";
            dbInfo.Port = 1433;
            dbInfo.DBName = "UMPDataDB1115-1";
            dbInfo.LoginName = "sa";
            dbInfo.Password = "PF,123";
            Session.DatabaseInfo = dbInfo;
            Session.DBType = dbInfo.TypeID;
            Session.DBConnectionString = dbInfo.GetConnectionString();

            ThemeInfo theme = Session.SupportThemes.FirstOrDefault(t => t.Name == "Style01");
            if (theme != null)
            {
                Session.ThemeInfo = theme;
                Session.ThemeName = theme.Name;
            }
        }

        protected override void InitLanguageInfos()
        {
            base.InitLanguageInfos();

            try
            {
                if (Session == null || Session.LangTypeInfo == null) { return; }
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)RequestCode.WSGetLangList;
                webRequest.Session = Session;
                webRequest.ListData.Add(Session.LangTypeInfo.LangID.ToString());
                webRequest.ListData.Add(string.Empty);
                webRequest.ListData.Add("31");
                webRequest.ListData.Add("3105");
                webRequest.ListData.Add(string.Empty);
                webRequest.ListData.Add(string.Empty);
                Service11012Client client = new Service11012Client(
                    WebHelper.CreateBasicHttpBinding(Session)
                    , WebHelper.CreateEndpointAddress(Session.AppServerInfo, "Service11012"));
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
            }
            catch (Exception ex)
            {
                ShowExceptionMessage(ex.Message);
            }
        }

        protected override void SetView()
        {
            base.SetView();

            CurrentView = new AppealManageMainView();
            CurrentView.PageName = "AppealManage";
        }

        #region staticmembers

        //当前xbap全部操作权限
        public List<OperationInfo> ListOperationInfos;


        //当前xbap当前人管理全部部门
        public static List<CtrolOrg> ListCtrolOrgInfos;

        //当前xbap当前人管理的全部座席
        public static List<CtrolAgent> ListCtrolAgentInfos;

        //当前用户所在部门以及子部门的ID
        public List<long> lstdptid;
        public static List<string> lsAgentInfos;
        /// <summary>
        /// Y:有审批流程，N:无审批流程
        /// </summary>
        public static string AppealProcess = "Y";

        //当前用户的机构Id
        public static string CurrentOrg = "-1";


        /// <summary>
        /// 当前用户管理的用户
        /// </summary>
        public static List<CtrolQA> ListCtrolQAInfos;


        public static List<SftpServerInfo> mListSftpServers;
        public static Service03Helper mService03Helper;
        public static List<DownloadParamInfo> mListDownloadParams;
        public static List<RecordEncryptInfo> mListRecordEncryptInfos;

        //从多个页面跳转里要用的
        public static bool NativePageFlag = true;

        /// <summary>
        /// 获取评分相关的参数配置
        /// </summary>
        public static List<GlobalParamInfo> ListScoreParam;
        #endregion

        protected override void Init()
        {
            base.Init();

            try
            {
                ListOperationInfos = new List<OperationInfo>();
                ListCtrolAgentInfos = new List<CtrolAgent>();
                ListCtrolOrgInfos = new List<CtrolOrg>();
                ListCtrolQAInfos = new List<CtrolQA>();
                lstdptid = new List<long>();
                lsAgentInfos = new List<string>();
                mListSftpServers = new List<SftpServerInfo>();
                mService03Helper = new Service03Helper();
                mListDownloadParams = new List<DownloadParamInfo>();
                mListRecordEncryptInfos = new List<RecordEncryptInfo>();
                ListScoreParam = new List<GlobalParamInfo>();
                LoadSftpServerList();
                SetService03Helper();
                LoadRecordEncryptInfos();
                LoadDownloadParamList();

                //得到所有操作权限
                InitControledOperations("31", "3105");
                //得到所能管理的部门管理的座席
                InitControledAgentAndOrg("-1");
                GetDepartmentInfo();
                GetAppealProcess();
                GetScoreParams();
               
            }
            catch (Exception ex)
            {
                ShowExceptionMessage(ex.Message);
            }
        }

        
        private void SetService03Helper()
        {
            try
            {
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

        private void LoadSftpServerList()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = Session;
                webRequest.Code = 33;
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
                    ShowExceptionMessage(GetLanguageInfo("3105T00101", string.Format("ListData is null")));
                    return;
                }
                mListSftpServers.Clear();
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[i];
                    OperationReturn optReturn = XMLHelper.DeserializeObject<Common3105.SftpServerInfo>(strInfo);
                    if (!optReturn.Result)
                    {
                        WriteLog("LoadSftp", string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        continue;
                    }
                    Common3105.SftpServerInfo sftpInfo = optReturn.Data as Common3105.SftpServerInfo;
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
                webRequest.ListData.Add(S3105Consts.USER_PARAM_GROUP_ENCRYPTINFO.ToString());
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
                        if (paramID > S3105Consts.USER_PARAM_GROUP_ENCRYPTINFO * 1000
                            && paramID < (S3105Consts.USER_PARAM_GROUP_ENCRYPTINFO + 1) * 1000
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

        //得到当前用所有角色的权限并集
        public void InitControledOperations(string modelId, string parentId)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)RequestCode.WSGetUserOptList;
                webRequest.Session = Session;
                webRequest.ListData.Add(Session.UserInfo.UserID.ToString());
                webRequest.ListData.Add(modelId);
                webRequest.ListData.Add(parentId);

                Service11012Client client = new Service11012Client(WebHelper.CreateBasicHttpBinding(Session),
                    WebHelper.CreateEndpointAddress(Session.AppServerInfo, "Service11012"));
                WebReturn webReturn = client.DoOperation(webRequest);
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


        //得到当前用户所管理的部门和座席
        public void InitControledAgentAndOrg(string OrgID)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = Session;
                webRequest.Code = 5;// (int)S3103Codes.GetControlOrgInfoList;
                webRequest.ListData.Add(Session.UserID.ToString());
                webRequest.ListData.Add(OrgID);
                Service31031Client client = new Service31031Client(
                    WebHelper.CreateBasicHttpBinding(Session),
                    WebHelper.CreateEndpointAddress(
                        Session.AppServerInfo,
                        "Service31031"));
                WebReturn webReturn = client.UMPTaskOperation(webRequest);
                client.Close();
                #region MyRegion
                if (!webReturn.Result)
                {
                    ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    ShowExceptionMessage(GetLanguageInfo("3105T00101", string.Format("ListData is null")));
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
                    InitControlQA(arrInfo[0]);
                    InitControlAgents(arrInfo[0]);
                }

                #endregion
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
                webRequest.Code = 6;// (int)S3103Codes.GetControlAgentInfoList;
                webRequest.ListData.Add(Session.UserID.ToString());
                webRequest.ListData.Add(parentID);
                webRequest.ListData.Add("A");
                Service31031Client client = new Service31031Client(
                    WebHelper.CreateBasicHttpBinding(Session),
                    WebHelper.CreateEndpointAddress(
                        Session.AppServerInfo,
                        "Service31031"));
                WebReturn webReturn = client.UMPTaskOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    ShowExceptionMessage(GetLanguageInfo("3105T00101", string.Format("ListData is null")));
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
                        lsAgentInfos.Add(ctrolAgent.AgentID);
                    }
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
                webRequest.Code = 14;
                webRequest.ListData.Add(parentID);
                webRequest.ListData.Add("3103005");
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

        private void GetAppealProcess()
        {
            WebRequest webRequest = new WebRequest();
            webRequest.Code = (int)S3105Codes.GetAppealProcess;
            webRequest.Session = Session;
            //Service31051Client client = new Service31051Client();
            Service31051Client client = new Service31051Client(WebHelper.CreateBasicHttpBinding(Session),
                WebHelper.CreateEndpointAddress(Session.AppServerInfo, "Service31051"));
            WebReturn webReturn = client.UMPTaskOperation(webRequest);
            client.Close();
            if (!webReturn.Result)
            {
                ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
            }
            if (webReturn.DataSetData != null && webReturn.DataSetData.Tables[0] != null && webReturn.DataSetData.Tables[0].Rows.Count > 0)
            {
                AppealProcess = webReturn.DataSetData.Tables[0].Rows[0][0].ToString();
            }
        }

        public long GetSerialID()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)RequestCode.WSGetSerialID;
                webRequest.Session = Session;
                webRequest.ListData.Add("31");
                webRequest.ListData.Add("305");
                webRequest.ListData.Add(DateTime.Now.ToString("yyyyMMddHHmmss"));
                Service11012Client client = new Service11012Client(WebHelper.CreateBasicHttpBinding(Session),
                    WebHelper.CreateEndpointAddress(Session.AppServerInfo, "Service11012"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return -1;
                }
                long id = Convert.ToInt64(webReturn.Data);
                return id;
            }
            catch (Exception ex)
            {
                ShowExceptionMessage(ex.Message);
                return -1;
            }
        }

        #region 获取全局参数配置
        private void GetScoreParams()
        {
            try
            {
                List<string> tempList = new List<string>() { "3103","3104" };
                foreach(string tempStr in  tempList)
                {
                    WebRequest webRequest = new WebRequest();
                    webRequest.Session = Session;
                    webRequest.Code = (int)RequestCode.WSGetGlobalParamList;
                    webRequest.ListData.Add("31");
                    webRequest.ListData.Add("0");
                    webRequest.ListData.Add(tempStr);
                    //webRequest.ListData.Add("310104");
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
                        ListScoreParam.Add(GlobalParamInfo);
                    }
                }

               
            }
            catch (Exception ex)
            {
                ShowExceptionMessage(ex.Message);
            }
        }

        #endregion


        #region other
        private void GetDepartmentInfo()
        {
            lstdptid.Add(S3105Consts.ORG_ROOT);
            DataSet ds = GetDeptInfo();
            if (ds != null)
            {
                GetCurrentUserChildDeptID(ref lstdptid, ds, S3105Consts.ORG_ROOT);
            }
        }

        private void GetCurrentUserChildDeptID(ref List<long> lstdptid, DataSet ds, long pid)
        {
            if (pid != -1)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    long curdid = Convert2Long(dr["C001"]);
                    long curpid = Convert2Long(dr["C004"]);
                    if (curdid != -1 && curpid == pid && !lstdptid.Contains(curdid))
                    {
                        lstdptid.Add(curdid);
                        GetCurrentUserChildDeptID(ref lstdptid, ds, curdid);
                    }
                }
            }
        }

        private DataSet GetDeptInfo()
        {
            List<long> lstchilddeptid = new List<long>();
            WebRequest webRequest = new WebRequest();
            webRequest.Code = (int)S3105Codes.GetDepartmentInfo;
            webRequest.Session = Session;
            Service31051Client client = new Service31051Client(WebHelper.CreateBasicHttpBinding(Session),
                    WebHelper.CreateEndpointAddress(Session.AppServerInfo, "Service31051"));
            WebReturn webReturn = client.UMPTaskOperation(webRequest);
            client.Close();
            if (!webReturn.Result)
            {
                ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                return null;
            }
            if (webReturn.DataSetData != null && webReturn.DataSetData.Tables[0] != null && webReturn.DataSetData.Tables[0].Rows.Count > 0)
            {
                return webReturn.DataSetData;
                //foreach(DataRow dr in ds_deptinfo.Tables[0].Rows)
            }
            return null;
        }

        private long Convert2Long(object obj)
        {
            if (obj == null || string.IsNullOrWhiteSpace(obj.ToString()))
            {
                return -1;
            }
            else
                return Convert.ToInt64(obj.ToString());
        }
        #endregion


        #region Encryption and Decryption

        public string EncryptString(string strSource)
        {
            string strTemp = EncryptionAndDecryption.EncryptDecryptString(strSource,
             CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004),
             EncryptionAndDecryption.UMPKeyAndIVType.M004);
            return strTemp;
        }

        public string DecryptString(string strSource)
        {
            string strTemp = EncryptionAndDecryption.EncryptDecryptString(strSource,
              CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104),
              EncryptionAndDecryption.UMPKeyAndIVType.M104);
            return strTemp;
        }

        public string CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType aKeyIVID)
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
