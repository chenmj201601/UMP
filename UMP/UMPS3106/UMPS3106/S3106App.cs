using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common3106;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using PFShareClassesC;
using UMPS3106.Models;
using UMPS3106.Wcf11012;
using UMPS3106.Wcf31021;
using UMPS3106.Wcf31061;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;

namespace UMPS3106
{
    public class S3106App : UMPApp
    {
        public S3106App(bool runAsModule)
            : base(runAsModule)
        {

        }

        public S3106App(IRegionManager regionManager, IEventAggregator eventAggregator, IAppControlService appController)
            : base(regionManager, eventAggregator, appController)
        {

        }

        protected override void SetAppInfo()
        {
            base.SetAppInfo();

            AppName = "UMPS3106";
            AppTitle = string.Format("Tutorial Repertory");
            ModuleID = 3106;
            AppType = (int)VoiceCyber.UMP.Common.AppType.UMPClient;
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

            DatabaseInfo dbInfo = new DatabaseInfo();
            dbInfo.TypeID = 2;
            dbInfo.Host = "192.168.4.182";
            dbInfo.Port = 1433;
            dbInfo.DBName = "UMPDataDB0527";
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
                webRequest.ListData.Add("3106");
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

            CurrentView = new TutorialRepertoryMainView();
            CurrentView.PageName = "TutorialRepertory";
        }

        protected override void Init()
        {
            base.Init();

            try
            {
                ListOperationInfos = new List<OperationInfo>();
                ListCtrolAgentInfos = new List<CtrolAgent>();
                ListCtrolOrgInfos = new List<CtrolOrg>();
                ListCtrolQAInfos = new List<CtrolQA>();
                mListAllObjects = new List<ObjectItem>();
                mRootItem = new ObjectItem();
                mListSftpServers = new List<VoiceCyber.UMP.Common31031.SftpServerInfo>();
                mService03Helper = new Service03Helper();
                mListDownloadParams = new List<VoiceCyber.UMP.Common31031.DownloadParamInfo>();
                mListRecordEncryptInfos = new List<VoiceCyber.UMP.Common31031.RecordEncryptInfo>();
                if (Session != null)
                {
                    WriteLog("AppLoad", string.Format("SessionInfo\r\n{0}", Session.LogInfo()));
                }
                //得到操作权限
                InitControledOperations("31", "3106");
                //得到所能管理的部门管理的座席
                InitControledAgentAndOrg("-1");
                InitLanguageInfos();
                LoadSftpServerList();
                SetService03Helper();
                LoadRecordEncryptInfos();
                LoadDownloadParamList();
            }
            catch (Exception ex)
            {
                ShowExceptionMessage(ex.Message);
            }
        }

        #region members

        //当前全部操作权限
        public List<OperationInfo> ListOperationInfos;

        /// <summary>
        /// 当前xbap当前人管理全部部门
        /// </summary>
        public List<CtrolOrg> ListCtrolOrgInfos;

        /// <summary>
        /// 当前xbap当前人管理的全部座席
        /// </summary>
        public List<CtrolAgent> ListCtrolAgentInfos;

        /// <summary>
        /// 当前xbap用户能管理的全部QA
        /// </summary>
        public List<CtrolQA> ListCtrolQAInfos;
        public ObjectItem mRootItem;
        public List<ObjectItem> mListAllObjects;
        public static List<VoiceCyber.UMP.Common31031.SftpServerInfo> mListSftpServers;
        public static Service03Helper mService03Helper;
        public static List<VoiceCyber.UMP.Common31031.DownloadParamInfo> mListDownloadParams;
        public static List<VoiceCyber.UMP.Common31031.RecordEncryptInfo> mListRecordEncryptInfos;

        //当前用户的机构Id
        public string CurrentOrg = "-1";

        #endregion


        //得到当前用所有角色的权限并集
        public void InitControledOperations(string modelId, string parentId)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S3106Codes.GetUserOperationList;
                //webRequest.Code = (int)RequestCode.WSGetUserOptList;
                webRequest.Session = Session;
                webRequest.ListData.Add(Session.UserInfo.UserID.ToString());
                webRequest.ListData.Add(modelId);
                webRequest.ListData.Add(parentId);

                //Service11012Client client = new Service31031Client();
                Service31061Client client = new Service31061Client(WebHelper.CreateBasicHttpBinding(Session), WebHelper.CreateEndpointAddress(Session.AppServerInfo, "Service31061"));
                //WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.UMPTreeOperation(webRequest);
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

        #region 录音加解密信息相关
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
                webRequest.ListData.Add(S3106Consts.USER_PARAM_GROUP_ENCRYPTINFO.ToString());
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
                        if (paramID > S3106Consts.USER_PARAM_GROUP_ENCRYPTINFO * 1000
                            && paramID < (S3106Consts.USER_PARAM_GROUP_ENCRYPTINFO + 1) * 1000
                            && dtExpireTime > DateTime.Now.ToUniversalTime())
                        {
                            VoiceCyber.UMP.Common31031.RecordEncryptInfo encryptInfo = new VoiceCyber.UMP.Common31031.RecordEncryptInfo();
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
                    OperationReturn optReturn = XMLHelper.DeserializeObject<VoiceCyber.UMP.Common31031.SftpServerInfo>(strInfo);
                    if (!optReturn.Result)
                    {
                        WriteLog("LoadSftp", string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        continue;
                    }
                    VoiceCyber.UMP.Common31031.SftpServerInfo sftpInfo = optReturn.Data as VoiceCyber.UMP.Common31031.SftpServerInfo;
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
                    OperationReturn optReturn = XMLHelper.DeserializeObject<VoiceCyber.UMP.Common31031.DownloadParamInfo>(strInfo);
                    if (!optReturn.Result)
                    {
                        WriteLog("LoadSftp", string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        continue;
                    }
                    VoiceCyber.UMP.Common31031.DownloadParamInfo info = optReturn.Data as VoiceCyber.UMP.Common31031.DownloadParamInfo;
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

        #region 得到当前用户所管理的部门和座席
        public void InitControledAgentAndOrg(string OrgID)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = Session;
                webRequest.Code = (int)S3106Codes.GetControlOrgInfoList;
                webRequest.ListData.Add(Session.UserID.ToString());
                webRequest.ListData.Add(OrgID);
                //Service31061Client client = new Service31061Client();
                Service31061Client client = new Service31061Client(WebHelper.CreateBasicHttpBinding(Session), WebHelper.CreateEndpointAddress(Session.AppServerInfo, "Service31061"));
                WebReturn webReturn = client.UMPTreeOperation(webRequest);
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
                webRequest.Code = (int)S3106Codes.GetQA;
                webRequest.ListData.Add(parentID);
                //Service31061Client client = new Service31061Client();
                Service31061Client client = new Service31061Client(WebHelper.CreateBasicHttpBinding(Session), WebHelper.CreateEndpointAddress(Session.AppServerInfo, "Service31061"));
                WebReturn webReturn = client.UMPTreeOperation(webRequest);
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
                webRequest.Code = (int)S3106Codes.GetControlAgentInfoList;
                webRequest.ListData.Add(Session.UserID.ToString());
                webRequest.ListData.Add(parentID);
                //Service31061Client client = new Service31061Client();
                Service31061Client client = new Service31061Client(WebHelper.CreateBasicHttpBinding(Session), WebHelper.CreateEndpointAddress(Session.AppServerInfo, "Service31061"));
                WebReturn webReturn = client.UMPTreeOperation(webRequest);
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
