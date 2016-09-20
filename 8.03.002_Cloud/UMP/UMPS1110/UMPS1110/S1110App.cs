//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    17d968c6-506b-4330-b259-aba85d5983a8
//        CLR Version:              4.0.30319.42000
//        Name:                     S1110App
//        Computer:                 DESKTOP-VUMCK8M
//        Organization:             VoiceCyber
//        Namespace:                UMPS1110
//        File Name:                S1110App
//
//        created by Charley at 2016/2/22 10:34:47
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Documents;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using UMPS1110.Wcf11012;
using UMPS1110.Wizard;
using VoiceCyber.Common;
using VoiceCyber.SDKs.Licenses;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;


namespace UMPS1110
{
    public class S1110App : UMPApp
    {

        #region Memebers
        public static List<License> mListLicenseRev = new List<License>();
        public static WizardHelper mWizardHelper;
        #endregion


        public S1110App(bool runAsModule)
            : base(runAsModule)
        {

        }

        public S1110App(IRegionManager regionManager,
            IEventAggregator eventAggregator,
            IAppControlService appController)
            : base(regionManager, eventAggregator, appController)
        {
            
        }

        protected override void SetAppInfo()
        {
            base.SetAppInfo();

            AppName = "UMPS1110";
            AppTitle = GetLanguageInfo("FO1110", string.Format("Resource Management"));
            ModuleID = 1110;
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

            //DatabaseInfo dbInfo = new DatabaseInfo();
            //dbInfo.TypeID = 2;
            //dbInfo.Host = "192.168.4.182";
            //dbInfo.Port = 1433;
            //dbInfo.DBName = "UMPDataDB0418";
            //dbInfo.LoginName = "PFDEV";
            //dbInfo.Password = "PF,123";
            //Session.DatabaseInfo = dbInfo;
            //Session.DBType = dbInfo.TypeID;
            //Session.DBConnectionString = dbInfo.GetConnectionString();

            DatabaseInfo dbInfo = new DatabaseInfo();
            dbInfo.TypeID = 3;
            dbInfo.TypeName = "ORCL";
            dbInfo.Host = "192.168.4.182";
            dbInfo.Port = 1521;
            dbInfo.DBName = "PFOrcl";
            dbInfo.LoginName = "PFDEV832";
            dbInfo.Password = "pfdev832";
            Session.DBType = dbInfo.TypeID;
            Session.DBConnectionString = dbInfo.GetConnectionString();
            Session.DatabaseInfo = dbInfo;

            var theme = Session.SupportThemes.FirstOrDefault(t => t.Name == "Style01");
            if (theme != null)
            {
                Session.ThemeInfo = theme;
                Session.ThemeName = theme.Name;
            }
        }

        protected override void SetView()
        {
            base.SetView();

            CurrentView =new ResourceMainView();
            CurrentView.PageName = "ResourceMainPage";
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
                webRequest.ListData.Add("11");
                webRequest.ListData.Add("1110");
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
    }
}
