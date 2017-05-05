using System;
using System.Linq;
using Common5102;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;
using VoiceCyber.UMP.Controls.Wcf11012;

namespace UMPS5102
{
    class S5102App : UMPApp
    {
        public S5102App(bool runAsModule)
            : base(runAsModule)
        {

        }

        public S5102App(IRegionManager regionManager, IEventAggregator eventAggregator, IAppControlService appController)
            : base(regionManager, eventAggregator, appController)
        {

        }

        #region Members

        public static bool GQueryModify = false;
        public static S5102Codes GOptInfo;
        public static KwContentInfoParam GKwConnectInfo;
        public static KeywordInfoParam GKwInfo;
        public const int GMsSql = 2;
        public const int GOracle = 3;
        public static long GLongKeywordNum = 0;
        #endregion

        #region Init & load Info

        protected override void SetAppInfo()
        {
            base.SetAppInfo();

            AppName = "UMPS5102";
            ModuleID = 5102;
        }

        protected override void InitSessionInfo()
        {
            base.InitSessionInfo();

            if (Session == null)
            {
                return;
            }

            AppServerInfo appServerInfo = new AppServerInfo
            {
                Protocol = "http",
                Address = "192.168.6.37",
                Port = 8081,
                SupportHttps = false,
                SupportNetTcp = false
            };
            Session.AppServerInfo = appServerInfo;

            DatabaseInfo dbInfo = new DatabaseInfo
            {
                TypeID = 2,
                Host = "192.168.4.182",
                Port = 1433,
                DBName = "UMPDataDB1114",
                LoginName = "sa",
                Password = "PF,123",
                RealPassword = "PF,123"
            };
            Session.DatabaseInfo = dbInfo;
            Session.DBType = dbInfo.TypeID;
            Session.DBConnectionString = dbInfo.GetConnectionString();

//             DatabaseInfo dbInfo = new DatabaseInfo
//             {
//                 TypeID = 3,
//                 TypeName = "ORCL",
//                 Host = "192.168.4.182",
//                 Port = 1521,
//                 DBName = "PFOrcl",
//                 LoginName = "PFDEV831",
//                 Password = "pfdev831"
//             };
//             Session.DatabaseInfo = dbInfo;
//             Session.DBType = dbInfo.TypeID;
//             Session.DBConnectionString = dbInfo.GetConnectionString();

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
                GKwConnectInfo = new KwContentInfoParam();
            }
            catch (Exception ex)
            {
                ShowExceptionMessage(ex.Message);
            }
        }

        protected override void SetView()
        {
            base.SetView();
            CurrentView = new MainView {PageName = "KeywordConfig"};
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
                WebRequest webRequest = new WebRequest
                {
                    Code = (int) RequestCode.WSGetLangList,
                    Session = Session
                };
                webRequest.ListData.Add(Session.LangTypeInfo.LangID.ToString());
                webRequest.ListData.Add(string.Empty);
                webRequest.ListData.Add("51");
                webRequest.ListData.Add("5102");
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
    }
}
