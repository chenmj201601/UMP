using System;
using System.Linq;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using UMPS3102.Wcf11012;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;
using VoiceCyber.UMP.Encryptions;


namespace UMPS3102
{
    public class S3102App : UMPApp
    {
        public S3102App(bool runAsModule)
            : base(runAsModule)
        {

        }

        public S3102App(IRegionManager regionManager,
            IEventAggregator eventAggregator,
            IAppControlService appController)
            : base(regionManager, eventAggregator, appController)
        {

        }

        protected override void SetAppInfo()
        {
            base.SetAppInfo();

            AppName = "UMPS3102";
            AppTitle = string.Format("Query Management");
            ModuleID = 3102;
            AppType = (int)VoiceCyber.UMP.Common.AppType.UMPClient;
        }


        protected override void SetView()
        {
            base.SetView();

            CurrentView = new QMMainView();
            CurrentView.PageName = "QMMainView";
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

            //AppServerInfo appServerInfo = new AppServerInfo();
            //appServerInfo.Protocol = "http";
            //appServerInfo.Address = "192.168.9.223";
            //appServerInfo.Port = 8081;
            //appServerInfo.SupportHttps = false;
            //appServerInfo.SupportNetTcp = false;
            //Session.AppServerInfo = appServerInfo;


            UserInfo userInfo = new UserInfo();
            userInfo.UserID = ConstValue.USER_ADMIN;
            userInfo.Account = "administrator";
            userInfo.UserName = "administrator";
            userInfo.Password = "111111";
            userInfo.RealPassword = "111111";
            Session.UserInfo = userInfo;
            Session.UserID = ConstValue.USER_ADMIN;


            //DatabaseInfo dbInfo = new DatabaseInfo();
            //dbInfo.TypeID = 2;
            //dbInfo.Host = "192.168.9.223";
            //dbInfo.Port = 1433;
            //dbInfo.DBName = "UMPDataDB0711";
            //dbInfo.LoginName = "sa";
            //dbInfo.Password = "voicecodes";
            //Session.DatabaseInfo = dbInfo;
            //Session.DBType = dbInfo.TypeID;
            //Session.DBConnectionString = dbInfo.GetConnectionString();

            DatabaseInfo dbInfo = new DatabaseInfo();
            dbInfo.TypeID = 2;
            dbInfo.Host = "192.168.4.182";
            dbInfo.Port = 1433;
            dbInfo.DBName = "UMPDataDB0911";
            dbInfo.LoginName = "PFDEV";
            dbInfo.Password = "PF,123";
            dbInfo.RealPassword = "PF,123";
            Session.DatabaseInfo = dbInfo;
            Session.DBType = dbInfo.TypeID;
            Session.DBConnectionString = dbInfo.GetConnectionString();
        }

        protected override void Init()
        {
            base.Init();

            try
            {
                if (Session != null)
                {
                    WriteLog("AppLoad", string.Format("{0}", Session.LogInfo()));
                }

                if (RunAsModule)
                {
                    QMMainView view = new QMMainView();
                    view.CurrentApp = Current;
                    var CurrentApp = AppController.ListAppConfigs.FirstOrDefault(a => a.ModuleID == ModuleID);
                    if (CurrentApp != null)
                    {
                        RegionManager.Regions[CurrentApp.PanelName].Add(view);
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
                if (Session == null || Session.LangTypeInfo == null) { return; }
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


        #region Encryption and Decryption

        public static string EncryptString(string strSource)
        {
            try
            {
                return ClientAESEncryption.EncryptString(strSource, EncryptionMode.AES256V04Hex);
            }
            catch (Exception ex)
            {
                return strSource;
            }
        }

        public static string DecryptString(string strSource)
        {
            try
            {
                return ClientAESEncryption.DecryptString(strSource, EncryptionMode.AES256V04Hex);
            }
            catch (Exception ex)
            {
                return strSource;
            }
        }

        public static string DecryptString001(string strSource)
        {
            try
            {
                return ClientAESEncryption.DecryptString(strSource, EncryptionMode.AES256V01Hex);
            }
            catch (Exception ex)
            {
                return strSource;
            }
        }

        #endregion

    }
}