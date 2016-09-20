using System;
using System.Threading;
using PFShareClassesC;
using UMPDemo.Wcf11012;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;

namespace UMPDemo
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App
    {
        #region Memebers


        #endregion


        #region Init and Load

        protected override void SetAppInfo()
        {
            base.SetAppInfo();

            AppName = "UMPDemo";
            ModuleID = 0;
        }

        protected override void InitSessionInfo()
        {
            base.InitSessionInfo();

            if (Session == null) { return; }
            //AppServerInfo serverInfo = new AppServerInfo();
            //serverInfo.Protocol = "http";
            //serverInfo.Address = "192.168.6.55";
            //serverInfo.Port = 8081;
            //serverInfo.SupportHttps = false;
            //Session.AppServerInfo = serverInfo;

            AppServerInfo serverInfo = new AppServerInfo();
            serverInfo.Protocol = "http";
            serverInfo.Address = "192.168.6.7";
            serverInfo.Port = 8081;
            serverInfo.SupportHttps = false;
            Session.AppServerInfo = serverInfo;

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

            DatabaseInfo dbInfo = new DatabaseInfo();
            dbInfo.TypeID = 2;
            dbInfo.TypeName = "MSSQL";
            dbInfo.Host = "192.168.4.182";
            dbInfo.Port = 1433;
            dbInfo.DBName = "UMPDataDB0817";
            dbInfo.LoginName = "PFDEV";
            dbInfo.Password = "PF,123";
            Session.DatabaseInfo = dbInfo;
            Session.DBType = dbInfo.TypeID;
            Session.DBConnectionString = dbInfo.GetConnectionString();
        }

        protected override void Init()
        {
            base.Init();
            if (Session != null)
            {
                WriteLog("AppLoad", string.Format("SessionInfo\r\n{0}", Session.LogInfo()));
            }
            InitLanguageInfos();
        }

        #endregion


        #region Language

        public static void InitLanguageInfos()
        {
            try
            {
                Thread.Sleep(5000);
                if (Session == null || Session.LangTypeInfo == null) { return; }
                ListLanguageInfos.Clear();
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)RequestCode.WSGetLangList;
                webRequest.Session = Session;
                webRequest.ListData.Add(Session.LangTypeInfo.LangID.ToString());
                webRequest.ListData.Add(string.Empty);
                webRequest.ListData.Add("31");
                webRequest.ListData.Add("3102");
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

        #endregion


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
            Random lRandom = new Random();
            string LStrTemp;

            try
            {
                lStrReturn = DateTime.Now.ToString("yyyyMMddHHmmss");
                int lIntRand = lRandom.Next(0, 14);
                LStrTemp = lIntRand.ToString("00");
                lStrReturn = lStrReturn.Insert(lIntRand, "VCT");
                lIntRand = lRandom.Next(0, 17);
                LStrTemp += lIntRand.ToString("00");
                lStrReturn = lStrReturn.Insert(lIntRand, "UMP");
                lIntRand = lRandom.Next(0, 20);
                LStrTemp += lIntRand.ToString("00");
                lStrReturn = lStrReturn.Insert(lIntRand, ((int)aKeyIVID).ToString("000"));

                lStrReturn = EncryptionAndDecryption.EncryptStringY(LStrTemp + lStrReturn);
            }
            catch { lStrReturn = string.Empty; }

            return lStrReturn;
        }

        #endregion

    }
}
