//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    85d81553-f705-48de-b1d3-fda14d4c1151
//        CLR Version:              4.0.30319.18408
//        Name:                     S4415App
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS4415
//        File Name:                S4415App
//
//        created by Charley at 2016/7/11 10:10:50
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Linq;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using UMPS4415.Wcf11012;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;
using VoiceCyber.UMP.Encryptions;


namespace UMPS4415
{
    public class S4415App : UMPApp
    {
        public S4415App(bool runAsModule)
            : base(runAsModule)
        {

        }

        public S4415App(IRegionManager regionManager, IEventAggregator eventAggregator, IAppControlService appController)
            : base(regionManager, eventAggregator, appController)
        {

        }

        protected override void SetAppInfo()
        {
            base.SetAppInfo();

            AppName = "UMPS4415";
            AppTitle = String.Format("ASM Alarm Setting");
            ModuleID = 4415;
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
            dbInfo.DBName = "UMPDataDB0606";
            dbInfo.LoginName = "PFDEV";
            dbInfo.Password = "PF,123";
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
                webRequest.ListData.Add(String.Empty);
                webRequest.ListData.Add("44");
                webRequest.ListData.Add("4415");
                webRequest.ListData.Add(String.Empty);
                webRequest.ListData.Add(String.Empty);
                Service11012Client client = new Service11012Client(
                    WebHelper.CreateBasicHttpBinding(Session)
                    , WebHelper.CreateEndpointAddress(Session.AppServerInfo, "Service11012"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowExceptionMessage(String.Format("{0}\t{1}", webReturn.Code, webReturn.Message));
                }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<LanguageInfo>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowExceptionMessage(String.Format("{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    LanguageInfo langInfo = optReturn.Data as LanguageInfo;
                    if (langInfo == null)
                    {
                        ShowExceptionMessage(String.Format("LanguageInfo is null"));
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

        protected override void SetView()
        {
            base.SetView();

            CurrentView = new AlarmSettingMainView();
            CurrentView.PageName = "AlarmSettingMainView";
        }

        public override void Exit()
        {
            if (CurrentView != null)
            {
                var main = CurrentView as UMPMainView;
                if (main != null)
                {
                    main.Close();
                }
            }

            base.Exit();
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
