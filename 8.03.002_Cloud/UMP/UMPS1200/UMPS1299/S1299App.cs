//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    da8b1f30-60a6-4790-877f-266934883f93
//        CLR Version:              4.0.30319.42000
//        Name:                     S1299App
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS1299
//        File Name:                S1299App
//
//        created by Charley at 2016/1/24 19:08:03
//        http://www.voicecyber.com 
//
//======================================================================

using System.Linq;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Controls;

namespace UMPS1299
{
    public class S1299App : UMPApp
    {

        #region Members



        #endregion


        public S1299App(bool runAsModule)
            : base(runAsModule)
        {
           
        }

        public S1299App(IRegionManager regionManager,
            IEventAggregator eventAggregator,
            IAppControlService appController) :
            base(regionManager, eventAggregator, appController)
        {

        }


        #region Init and Load

        protected override void SetAppInfo()
        {
            base.SetAppInfo();

            AppName = "UMPS1299";
            AppTitle = string.Format("UMP Temp Application");
            ModuleID = 1299;
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
        }

        protected override void SetView()
        {
            base.SetView();

            CurrentView=new TempView();
            CurrentView.PageName = "TempView";
        }

        #endregion

       
    }
}
