//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    38422273-2a3f-4b96-a0ed-4fd30f2f7e1d
//        CLR Version:              4.0.30319.42000
//        Name:                     S1202App
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS1202
//        File Name:                S1202App
//
//        created by Charley at 2016/1/22 10:59:45
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.Linq;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;
using VoiceCyber.UMP.Encryptions;

namespace UMPS1202
{
    public class S1202App : UMPApp
    {

        #region Memebers

        /// <summary>
        /// 登录时由服务器生成的SessionID
        /// </summary>
        public static string LoginSessionID;
        /// <summary>
        /// 是否登录
        /// </summary>
        public static bool IsLogined;

        public static bool IsLDAPAutoLogin;     //是否LDAP自动登录
        public static bool IsLDAPAccount;       //是否域帐号登录
        public static string LoginDomainName;   //登录的域名
        
        /// <summary>
        /// 最后活动时间
        /// </summary>
        public static DateTime LastActiveTime;

        #endregion


        public S1202App(bool runAsModule)
            : base(runAsModule)
        {
        }

        public S1202App(IRegionManager regionManager,
            IEventAggregator eventAggregator,
            IAppControlService appController)
            : base(regionManager, eventAggregator, appController)
        {
        }


        public override void Exit()
        {
            var view = CurrentView as LoginView;
            if (view != null)
            {
                view.OnExitSystem();
            }

            base.Exit();
        }

        protected override void SetAppInfo()
        {
            base.SetAppInfo();

            AppName = "UMPS1202";
            AppTitle = string.Format("UMP");
            ModuleID = 1202;
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

            DatabaseInfo dbInfo = new DatabaseInfo();
            dbInfo.TypeID = 2;
            dbInfo.Host = "192.168.4.182";
            dbInfo.Port = 1433;
            dbInfo.DBName = "UMPDataDB0912";
            dbInfo.LoginName = "PFDEV";
            dbInfo.Password = "PF,123";
            dbInfo.RealPassword = "PF,123";
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

            CurrentView = new LoginView();
            CurrentView.PageName = "LoginView";
        }

        protected override void OnAppEvent(WebRequest webRequest)
        {
            try
            {
                //int code = webRequest.Code;
                //switch (code)
                //{
                //    case (int)RequestCode.ACPageHeadLogout:

                //        return;
                //}
            }
            catch (Exception ex)
            {
                ShowExceptionMessage(ex.Message);
            }

            //base.OnAppEvent(webRequest);
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

        #endregion

    }
}
