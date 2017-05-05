using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common3107;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using UMPS3107.Models;
using UMPS3107.Wcf11012;
using UMPS3107.Wcf31071;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;

namespace UMPS3107
{
    public class S3107App : UMPApp
    {
        public S3107App(bool runAsModule)
            : base(runAsModule)
        {

        }

        public S3107App(IRegionManager regionManager, IEventAggregator eventAggregator, IAppControlService appController)
            : base(regionManager, eventAggregator, appController)
        {

        }

        protected override void SetAppInfo()
        {
            base.SetAppInfo();

            AppName = "UMPS3107";
            AppTitle = string.Format("Query Condition Configuration");
            ModuleID = 3107;
            AppType = (int)VoiceCyber.UMP.Common.AppType.UMPClient;
        }



        #region members
        /// <summary>
        /// 任务设置  False 新建 | True 修改
        /// </summary>
        public static bool TaskModify = false;

        /// <summary>
        /// 查询设置  False 新建 | True 修改
        /// </summary>
        public static bool QueryModify = false;


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

        //用户能管理的全部QA
        public static List<CtrolQA> ListCtrolQAInfos;
        public static ObjectItem mRootItem;
        public static List<ObjectItem> mListAllObjects;

        //关键词
        public static List<KeyWordItems> ListKeyWordItems;

        //当前用户的机构Id
        public static string CurrentOrg = "-1";

        #endregion

        protected override void Init()
        {
            base.Init();

            try
            {
                ListCtrolAgentInfos = new List<CtrolAgent>();
                ListCtrolRealityExtension = new List<CtrolAgent>();
                ListCtrolExtension = new List<CtrolAgent>();
                ListCtrolOrgInfos = new List<CtrolOrg>();
                ListCtrolQAInfos = new List<CtrolQA>();
                mListAllObjects = new List<ObjectItem>();
                mRootItem = new ObjectItem();
                ListKeyWordItems = new List<KeyWordItems>();

                GroupingWay = string.Empty;
                LoadGroupingMethodParams();
                //得到所能管理的部门管理的座席
                InitControledAgentAndOrg("-1");
            }
            catch (Exception ex)
            {
                ShowExceptionMessage(ex.Message);
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
            dbInfo.Host = "192.168.4.182";
            dbInfo.Port = 1433;
            dbInfo.DBName = "UMPDataDB11292";
            dbInfo.LoginName = "PFDEV";
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
                webRequest.ListData.Add("3107");
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

            CurrentView = new QueryConditionMainView();
            CurrentView.PageName = "QueryConditionSetting";
        }



        #region 得到当前用户所管理的部门和QA、座席、分机、真实分机
        public void InitControledAgentAndOrg(string OrgID)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = Session;
                webRequest.Code = (int)S3107Codes.GetControlOrgInfoList;
                webRequest.ListData.Add(Session.UserID.ToString());
                webRequest.ListData.Add(OrgID);
                //Service31071Client client = new Service31071Client();
                Service31071Client client = new Service31071Client(WebHelper.CreateBasicHttpBinding(Session), WebHelper.CreateEndpointAddress(Session.AppServerInfo, "Service31071"));
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
                    InitControlQA(arrInfo[0]);
                    InitControlAgents(arrInfo[0]);
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
                webRequest.Code = (int)S3107Codes.GetQA;
                webRequest.ListData.Add(parentID);
                webRequest.ListData.Add("3103005");//任务中评分权限
                //Service31071Client client = new Service31071Client();
                Service31071Client client = new Service31071Client(WebHelper.CreateBasicHttpBinding(Session), WebHelper.CreateEndpointAddress(Session.AppServerInfo, "Service31071"));
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
                webRequest.Code = (int)S3107Codes.GetControlAgentInfoList;
                webRequest.ListData.Add(Session.UserID.ToString());
                webRequest.ListData.Add(parentID);
                webRequest.ListData.Add("A");
                //Service31071Client client = new Service31071Client();
                Service31071Client client = new Service31071Client(WebHelper.CreateBasicHttpBinding(Session), WebHelper.CreateEndpointAddress(Session.AppServerInfo, "Service31071"));
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
                    if (ListCtrolAgentInfos.Where(p => p.AgentID == ctrolAgent.AgentID).Count() == 0 && ctrolAgent.AgentFullName.ToUpper() != "N/A")
                    {
                        ListCtrolAgentInfos.Add(ctrolAgent);
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
                webRequest.Code = (int)S3107Codes.GetControlAgentInfoList;
                webRequest.ListData.Add(Session.UserID.ToString());
                webRequest.ListData.Add(parentID);
                webRequest.ListData.Add("R");
                //Service31071Client client = new Service31071Client();
                Service31071Client client = new Service31071Client(WebHelper.CreateBasicHttpBinding(Session), WebHelper.CreateEndpointAddress(Session.AppServerInfo, "Service31071"));
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
                webRequest.Code = (int)S3107Codes.GetControlAgentInfoList;
                webRequest.ListData.Add(Session.UserID.ToString());
                webRequest.ListData.Add(parentID);
                webRequest.ListData.Add("E");
                //Service31071Client client = new Service31071Client();
                Service31071Client client = new Service31071Client(WebHelper.CreateBasicHttpBinding(Session), WebHelper.CreateEndpointAddress(Session.AppServerInfo, "Service31071"));
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

        #endregion


        #region 是否添加分机的全局参数
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

        #endregion

    }
}
