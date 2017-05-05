using System;
using System.Linq;
using Common3604;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using PFShareClassesC;
using UMPS3604.Models;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;
using VoiceCyber.UMP.Controls.Wcf11012;

namespace UMPS3604
{
    public class S3604App : UMPApp
    {
         public S3604App(bool runAsModule)
            : base(runAsModule)
        {

        }

        public S3604App(IRegionManager regionManager, IEventAggregator eventAggregator, IAppControlService appController)
            : base(regionManager, eventAggregator, appController)
        {

        }

        #region Members
        public static bool GQueryModify = false;
        public static int GiOptContentsInfo = 0;
        public static ContentsTree GContentsTree = new ContentsTree();
        #endregion

        #region Init & load Info
        protected override void SetAppInfo()
        {
            base.SetAppInfo();

            AppName = "UMPS3604";
            ModuleID = 3604;
        }

        protected override void InitSessionInfo()
        {
            base.InitSessionInfo();

            if (Session == null) { return; }

            AppServerInfo appServerInfo = new AppServerInfo
            {
                Protocol = "http",
                Address = "192.168.6.29",
                Port = 8081,
                SupportHttps = false,
                SupportNetTcp = false
            };
            Session.AppServerInfo = appServerInfo;

            //             DatabaseInfo dbInfo = new DatabaseInfo();
            //             dbInfo.TypeID = 2;
            //             dbInfo.Host = "192.168.4.182";
            //             dbInfo.Port = 1433;
            //             dbInfo.DBName = "UMPDataDB0420";
            //             dbInfo.LoginName = "PFDEV";
            //             dbInfo.Password = "PF,123";
            //             Session.DatabaseInfo = dbInfo;
            //             Session.DBType = dbInfo.TypeID;
            //             Session.DBConnectionString = dbInfo.GetConnectionString();

            DatabaseInfo dbInfo = new DatabaseInfo
            {
                TypeID = 3,
                TypeName = "ORCL",
                Host = "192.168.4.182",
                Port = 1521,
                DBName = "PFOrcl",
                LoginName = "PFDEV832",
                Password = "pfdev832",
                RealPassword = "pfdev832"
            };
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

        protected override void Init()
        {
            base.Init();

            try
            {

            }
            catch (Exception ex)
            {
                ShowExceptionMessage(ex.Message);
            }
        }

        protected override void SetView()
        {
            base.SetView();
            CurrentView = new MaterialLibraryView { PageName = "MaterialLibrary" };
        }

        protected override void InitLanguageInfos()
        {
            base.InitLanguageInfos();

            try
            {
                if (Session == null || Session.LangTypeInfo == null) { return; }
                WebRequest webRequest = new WebRequest
                {
                    Code = (int)RequestCode.WSGetLangList,
                    Session = Session
                };
                webRequest.ListData.Add(Session.LangTypeInfo.LangID.ToString());
                webRequest.ListData.Add(string.Empty);
                webRequest.ListData.Add("36");
                webRequest.ListData.Add("3604");
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
                foreach (string strDate in webReturn.ListData)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<LanguageInfo>(strDate);
                    if (!optReturn.Result)
                    {
                        ShowExceptionMessage(string.Format("{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    LanguageInfo langInfo = optReturn.Data as LanguageInfo;
                    if (langInfo == null)
                    {
                        ShowExceptionMessage("LanguageInfo is null");
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

        public string CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType aKeyIvid)
        {
            string lStrReturn;
            Random lRandom = new Random();

            try
            {
                lStrReturn = DateTime.Now.ToString("yyyyMMddHHmmss");
                var lIntRand = lRandom.Next(0, 14);
                var lStrTemp = lIntRand.ToString("00");
                lStrReturn = lStrReturn.Insert(lIntRand, "VCT");
                lIntRand = lRandom.Next(0, 17);
                lStrTemp += lIntRand.ToString("00");
                lStrReturn = lStrReturn.Insert(lIntRand, "UMP");
                lIntRand = lRandom.Next(0, 20);
                lStrTemp += lIntRand.ToString("00");
                lStrReturn = lStrReturn.Insert(lIntRand, ((int)aKeyIvid).ToString("000"));

                lStrReturn = EncryptionAndDecryption.EncryptStringY(lStrTemp + lStrReturn);
            }
            catch { lStrReturn = string.Empty; }

            return lStrReturn;
        }

        #endregion
    }
}
