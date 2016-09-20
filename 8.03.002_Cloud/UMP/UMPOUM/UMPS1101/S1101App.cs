using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using PFShareClassesC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UMPS1101.Models;
using UMPS1101.Wcf11012;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;

namespace UMPS1101
{
    public class S1101App : UMPApp
    {
        public S1101App(bool runAsModule)
            : base(runAsModule)
        {
        }

        public S1101App(IRegionManager regionManager,
         IEventAggregator eventAggregator,
         IAppControlService appController)
            : base(regionManager, eventAggregator, appController)
        {
        }

        protected override void SetView()
        {
            base.SetView();

            CurrentView = new OUMMainView();
            CurrentView.PageName = "OUMMainPage";
        }
        #region Members


        public static List<CacheInfo> ListCacheInfos = new List<CacheInfo>();


        public static string LoadParameter;

        //private static LogOperator mLogOperator;

        #endregion

        #region Init and Load

        protected override void SetAppInfo()
        {
            base.SetAppInfo();

            AppName = "UMPS1101";
            ModuleID = 1101;
        }

        protected override void InitSessionInfo()
        {
            base.InitSessionInfo();

            if (Session == null) { return; }

            AppServerInfo serverInfo = new AppServerInfo();
            serverInfo.Protocol = "http";
            serverInfo.Address = "192.168.6.120";
            serverInfo.Port = 8081;
            serverInfo.SupportHttps = false;
            Session.AppServerInfo = serverInfo;

            //DatabaseInfo dbInfo = new DatabaseInfo();
            //dbInfo.TypeID = 3;
            //dbInfo.TypeName = "ORCL";
            //dbInfo.Host = "192.168.4.182";
            //dbInfo.Port = 1521;
            //dbInfo.DBName = "PFOrcl";
            //dbInfo.LoginName = "PFDEV832";
            //dbInfo.Password = "pfdev832";
            //Session.DatabaseInfo = dbInfo;
            //Session.DBType = dbInfo.TypeID;
            //Session.DBConnectionString = dbInfo.GetConnectionString();
            //Session.DBType = 3;

            DatabaseInfo dbInfo = new DatabaseInfo();
            dbInfo.TypeID = 2;
            dbInfo.TypeName = "MSSQL";
            dbInfo.Host = "192.168.4.182";
            dbInfo.Port = 1433;
            dbInfo.DBName = "UMPDataDB0908";
            dbInfo.LoginName = "PFDEV";
            dbInfo.Password = "PF,123";
            Session.DatabaseInfo = dbInfo;
            Session.DBType = dbInfo.TypeID;
            Session.DBConnectionString = dbInfo.GetConnectionString();

            Session.InstallPath = @"C:\UMPRelease";

            WriteLog("AppInit", string.Format("SessionInfo inited."));
        }

        #endregion

        protected override void Init()
        {
            base.Init();
            if (Session != null)
            {
                WriteLog("AppLoad", string.Format("SessionInfo\r\n{0}", Session.LogInfo()));
            }
            InitLanguageInfos();
        }
        #region Language

        protected override void InitLanguageInfos()
        {
            base.InitLanguageInfos();
            try
            {
                if (Session == null || Session.LangTypeInfo == null) { return; }
                //ListLanguageInfos.Clear();
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)RequestCode.WSGetLangList;
                webRequest.Session = Session;
                webRequest.ListData.Add(Session.LangTypeInfo.LangID.ToString());
                webRequest.ListData.Add(string.Empty);
                webRequest.ListData.Add(string.Format("0{0}11", ConstValue.SPLITER_CHAR));
                webRequest.ListData.Add(string.Format("0{0}1101", ConstValue.SPLITER_CHAR));
                webRequest.ListData.Add(string.Empty);
                webRequest.ListData.Add(string.Empty);
                Service11012Client client = new Service11012Client(
                    WebHelper.CreateBasicHttpBinding(Session)
                    , WebHelper.CreateEndpointAddress(Session.AppServerInfo, "Service11012"));
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
                WriteLog("AppInit", string.Format("LanguageInfos inited."));
            }
            catch (Exception ex)
            {
                //ShowExceptionMessage(ex.Message);
            }
        }

        #endregion

        #region Encryption and Decryption

        public static string EncryptString(string strSource)
        {
            string strReturn = string.Empty;
            string strTemp;
            do
            {
                if (strSource.Length > 128)
                {
                    strTemp = strSource.Substring(0, 128);
                    strSource = strSource.Substring(128, strSource.Length - 128);
                }
                else
                {
                    strTemp = strSource;
                    strSource = string.Empty;
                }
                strReturn += EncryptionAndDecryption.EncryptDecryptString(strTemp,
                    CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004),
                    EncryptionAndDecryption.UMPKeyAndIVType.M004);
            } while (strSource.Length > 0);
            return strReturn;
        }

        public static string DecryptString(string strSource)
        {
            string strReturn = string.Empty;
            string strTemp;
            do
            {
                if (strSource.Length > 512)
                {
                    strTemp = strSource.Substring(0, 512);
                    strSource = strSource.Substring(512, strSource.Length - 512);
                }
                else
                {
                    strTemp = strSource;
                    strSource = string.Empty;
                }
                strReturn += EncryptionAndDecryption.EncryptDecryptString(strTemp,
                    CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104),
                    EncryptionAndDecryption.UMPKeyAndIVType.M104);
            } while (strSource.Length > 0);
            return strReturn;
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
