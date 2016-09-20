//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    799e4999-97ca-4755-a8f8-56ae7a8f881c
//        CLR Version:              4.0.30319.42000
//        Name:                     S1205App
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS1205
//        File Name:                S1205App
//
//        created by Charley at 2016/1/24 17:43:37
//        http://www.voicecyber.com 
//
//======================================================================

using System.Linq;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common12001;
using VoiceCyber.UMP.Controls;

namespace UMPS1205
{
    public class S1205App : UMPApp
    {

        #region Memebers



        #endregion


        public S1205App(bool runAsModule)
            : base(runAsModule)
        {

        }

        public S1205App(IRegionManager regionManager,
            IEventAggregator eventAggregator,
            IAppControlService appController)
            : base(regionManager, eventAggregator, appController)
        {
           
        }


        protected override void SetAppInfo()
        {
            base.SetAppInfo();

            AppName = "UMPS1205";
            AppTitle = string.Format("TaskPage Application");
            ModuleID = 1205;
            AppType = (int)VoiceCyber.UMP.Common.AppType.UMPClient;
        }

        protected override void InitSessionInfo()
        {
            base.InitSessionInfo();

            AppServerInfo appServerInfo = new AppServerInfo();
            appServerInfo.Protocol = "http";
            appServerInfo.Address = "192.168.6.63";
            appServerInfo.Port = 8081;
            appServerInfo.SupportHttps = false;
            appServerInfo.SupportNetTcp = false;
            Session.AppServerInfo = appServerInfo;

            DatabaseInfo dbInfo = new DatabaseInfo();
            dbInfo.TypeID = 2;
            dbInfo.Host = "192.168.4.182";
            dbInfo.Port = 1433;
            dbInfo.DBName = "UMPDataDB0126";
            dbInfo.LoginName = "PFDEV";
            dbInfo.Password = "PF,123";
            Session.DatabaseInfo = dbInfo;
            Session.DBType = dbInfo.TypeID;
            Session.DBConnectionString = dbInfo.GetConnectionString();

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

            CurrentView=new TaskPageView();
            CurrentView.PageName = "TaskPageView";
        }
    }
}
