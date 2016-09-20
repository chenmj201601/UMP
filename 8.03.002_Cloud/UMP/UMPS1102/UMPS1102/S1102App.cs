using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using PFShareClassesC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UMPS1102.Wcf11012;
using UMPS1102.Wcf11021;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common11021;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;

namespace UMPS1102
{
    public class S1102App : UMPApp
    { 
        public S1102App(bool runAsModule)
            : base(runAsModule)
        {
        }

        public S1102App(IRegionManager regionManager,
         IEventAggregator eventAggregator,
         IAppControlService appController)
            : base(regionManager, eventAggregator, appController)
        {
        }

        protected override void SetView()
        {
            base.SetView();

            CurrentView = new RoleManage();
            CurrentView.PageName = "RoleManage";
        }
        public static string LoadParameter;

        private static LogOperator mLogOperator;

        //当前用户的机构Id
        public static long CurrentOrg = 0;

        //当前xbap全部操作权限
        public static List<OperationInfo> ListOperationInfos;

        protected override void SetAppInfo()
        {
            base.SetAppInfo();

            AppName = "UMPS1102";
            ModuleID = 1102;
        }

        protected override void Init()
        {
            base.Init();

            //CreateLogOperator();
            ListLanguageInfos = new List<LanguageInfo>();
            ListOperationInfos = new List<OperationInfo>();
            //Init();
            if (Session != null)
            {
                WriteLog("AppLoad", string.Format("SessionInfo\r\n{0}", Session.LogInfo()));
            }
            InitLanguageInfos();
            InitControledOperations("11", "1102");
        }

        protected override void InitSessionInfo()
        {
            base.InitSessionInfo();

            if (Session == null) { return; }

            UserInfo userInfo = new UserInfo();
            userInfo.UserID = LongParse(string.Format(S1102Consts.USER_ADMIN, S1102Consts.RENT_DEFAULT_TOKEN), 0);
            userInfo.Account = "administrator";
            userInfo.UserName = "Administrator";
            Session.UserInfo = userInfo;
            Session.UserID = LongParse(string.Format(S1102Consts.USER_ADMIN, S1102Consts.RENT_DEFAULT_TOKEN), 0);

            RoleInfo roleInfo = new RoleInfo();
            roleInfo.ID = LongParse(string.Format(S1102Consts.ROLE_SYSTEMADMIN, S1102Consts.RENT_DEFAULT_TOKEN), 0);
            roleInfo.Name = "System Admin";
            Session.RoleInfo = roleInfo;

            AppServerInfo serverInfo = new AppServerInfo();

            //serverInfo.Protocol = "https";
            //serverInfo.Address = "192.168.4.184";
            //serverInfo.Port = 8082;
            //serverInfo.SupportHttps = true;

            serverInfo.Protocol = "http";
            serverInfo.Address = "192.168.4.166";
            serverInfo.Port = 8081;
            serverInfo.SupportHttps = false;

            Session.AppServerInfo = serverInfo;

            //DatabaseInfo dbInfo = new DatabaseInfo();
            //dbInfo.TypeID = 3;
            //dbInfo.TypeName = "ORCL";
            //dbInfo.Host = "192.168.4.182";
            //dbInfo.Port = 1521;
            //dbInfo.DBName = "PFOrcl";
            //dbInfo.LoginName = "PFDEV831";
            //dbInfo.Password = "pfdev831";
            //Session.DBType = dbInfo.TypeID;
            //Session.DBConnectionString = dbInfo.GetConnectionString();
            //Session.DatabaseInfo = dbInfo;

            DatabaseInfo dbInfo = new DatabaseInfo();
            dbInfo.TypeID = 2;
            dbInfo.TypeName = "MSSQL";
            dbInfo.Host = "192.168.4.182";
            dbInfo.Port = 1433;
            dbInfo.DBName = "UMPDataDB0418";
            dbInfo.LoginName = "PFDEV";
            dbInfo.Password = "PF,123";
            Session.DBConnectionString = dbInfo.GetConnectionString();
            Session.DatabaseInfo = dbInfo;
            Session.DBType = dbInfo.TypeID;

            //DatabaseInfo dbInfo = new DatabaseInfo();
            //dbInfo.TypeID = 2;
            //dbInfo.TypeName = "MSSQL";
            //dbInfo.Host = "192.168.8.138";
            //dbInfo.Port = 1433;
            //dbInfo.DBName = "UMPDataDB8100";
            //dbInfo.LoginName = "sa";
            //dbInfo.Password = "voicecodes";
            //Session.DBType = dbInfo.TypeID;
            //Session.DBConnectionString = dbInfo.GetConnectionString();
            //Session.DatabaseInfo = dbInfo;

            Session.InstallPath = @"C:\UMPRelease";
            WriteLog("AppInit", string.Format("SessionInfo inited."));
        }
        //得到当前用所有角色的权限并集
        public void InitControledOperations(string modelId, string parentId)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S1102Codes.GetCurrentOperationList;
                webRequest.Session = Session;
                webRequest.ListData.Add(modelId);
                webRequest.ListData.Add(parentId);

                //Service11021Client client = new Service11021Client();
                Service11021Client client = new Service11021Client(WebHelper.CreateBasicHttpBinding(Session), WebHelper.CreateEndpointAddress(Session.AppServerInfo, "Service11021"));
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.URPOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
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
                //2     ModuleID
                //3     SubModuleID
                //4     Page
                //5     Name
                webRequest.ListData.Add(Session.LangTypeInfo.LangID.ToString());
                webRequest.ListData.Add(string.Empty);
                webRequest.ListData.Add("11");
                webRequest.ListData.Add("1102");
                webRequest.ListData.Add(string.Empty);
                webRequest.ListData.Add(string.Empty);
                Service11012Client client = new Service11012Client(WebHelper.CreateBasicHttpBinding(Session), WebHelper.CreateEndpointAddress(Session.AppServerInfo, "Service11012"));
                WebHelper.SetServiceClient(client);
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
                //MessageBox.Show(name+"  1:ok");

                //ListParams
                //0     LangID
                //1     PreName（语言内容编码的前缀，比如 FO:模块、操作显示语言）
                //2     ModuleID
                //3     SubModuleID
                //4     Page
                //5     Name
                webRequest = new WebRequest();
                webRequest.Session = Session;
                webRequest.Code = (int)RequestCode.WSGetLangList;
                webRequest.ListData.Add(string.Empty);
                webRequest.ListData.Add("FO");
                webRequest.ListData.Add(string.Empty);
                webRequest.ListData.Add(string.Empty);
                webRequest.ListData.Add(string.Empty);
                webRequest.ListData.Add(string.Empty);
                client = new Service11012Client(WebHelper.CreateBasicHttpBinding(Session), WebHelper.CreateEndpointAddress(Session.AppServerInfo, "Service11012"));
                WebHelper.SetServiceClient(client);
                webReturn = client.DoOperation(webRequest);
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
                    if (ListLanguageInfos.Where(p => p.LangID == langInfo.LangID && p.Name == langInfo.Name).Count() == 0)
                    {
                        ListLanguageInfos.Add(langInfo);
                    }
                }
                //MessageBox.Show(name+"  2:ok");

                webRequest = new WebRequest();
                webRequest.Session = Session;
                webRequest.Code = (int)RequestCode.WSGetLangList;
                webRequest.ListData.Add(string.Empty);
                webRequest.ListData.Add("COM");
                webRequest.ListData.Add(string.Empty);
                webRequest.ListData.Add(string.Empty);
                webRequest.ListData.Add(string.Empty);
                webRequest.ListData.Add(string.Empty);
                client = new Service11012Client(WebHelper.CreateBasicHttpBinding(Session), WebHelper.CreateEndpointAddress(Session.AppServerInfo, "Service11012"));
                WebHelper.SetServiceClient(client);
                webReturn = client.DoOperation(webRequest);
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
                    if (ListLanguageInfos.Where(p => p.LangID == langInfo.LangID && p.Name == langInfo.Name).Count() == 0)
                    {
                        ListLanguageInfos.Add(langInfo);
                    }
                } //MessageBox.Show(name+"  3:ok");
            }
            catch (Exception ex)
            {
                //ShowExceptionMessage(ex.Message);
            }
        }

        #region Encryption and Decryption

        public static string EncryptString(string strSource)
        {
            string strTemp = EncryptionAndDecryption.EncryptDecryptString(strSource,
             CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004),
             EncryptionAndDecryption.UMPKeyAndIVType.M004);
            return strTemp;
        }

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

        #region
        public static long LongParse(string str, long defaultValue)
        {
            long outRet = defaultValue;
            if (!long.TryParse(str, out outRet))
            {
                outRet = defaultValue;
            }

            return outRet;
        }
        #endregion
    }
}
