//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    4d8c6b27-16f2-48f0-9ab6-c8ea809a2787
//        CLR Version:              4.0.30319.42000
//        Name:                     S1204App
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS1204
//        File Name:                S1204App
//
//        created by Charley at 2016/1/22 12:13:28
//        http://www.voicecyber.com 
//
//======================================================================

using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Controls;

namespace UMPS1204
{
    public class S1204App : UMPApp
    {

        #region Memebers



        #endregion


        public S1204App(bool runAsModule)
            : base(runAsModule)
        {

        }

        public S1204App(IRegionManager regionManager,
            IEventAggregator eventAggregator,
            IAppControlService appController)
            : base(regionManager, eventAggregator, appController)
        {

        }


        protected override void SetAppInfo()
        {
            base.SetAppInfo();

            AppName = "UMPS1204";
            AppTitle = string.Format("UMP");
            ModuleID = 1204;
            AppType = (int)VoiceCyber.UMP.Common.AppType.UMPClient;
        }

        protected override void InitSessionInfo()
        {
            base.InitSessionInfo();

            AppServerInfo appServerInfo = new AppServerInfo();
            appServerInfo.Protocol = "http";
            appServerInfo.Address = "192.168.6.74";
            appServerInfo.Port = 8081;
            appServerInfo.SupportHttps = false;
            appServerInfo.SupportNetTcp = false;
            Session.AppServerInfo = appServerInfo;

            DatabaseInfo dbInfo = new DatabaseInfo();
            dbInfo.TypeID = 2;
            dbInfo.Host = "192.168.4.182";
            dbInfo.Port = 1433;
            dbInfo.DBName = "UMPDataDB0826";
            dbInfo.LoginName = "PFDEV";
            dbInfo.Password = "PF,123";
            Session.DatabaseInfo = dbInfo;
            Session.DBType = dbInfo.TypeID;
            Session.DBConnectionString = dbInfo.GetConnectionString();
        }

        protected override void SetView()
        {
            base.SetView();

            CurrentView=new StatusBarView();
            CurrentView.PageName = "StatusBarView";
        }
    }
}
