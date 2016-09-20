//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    6564f757-f00b-4196-836a-b885c7895ba4
//        CLR Version:              4.0.30319.42000
//        Name:                     S2101App
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS2102
//        File Name:                S2101App
//
//        created by Charley at 2016/1/29 16:05:28
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using UMPS2101;
using UMPS2101.Wcf11012;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;

namespace UMPS2101
{
    public class S2101App : UMPApp
    {

        #region Memebers



        #endregion


        public S2101App(bool runAsModule)
            : base(runAsModule)
        {

        }

        public S2101App(IRegionManager regionManager,
            IEventAggregator eventAggregator,
            IAppControlService appController)
            : base(regionManager, eventAggregator, appController)
        {

        }


        protected override void SetAppInfo()
        {
            base.SetAppInfo();

            AppName = "UMPS2101";
            AppTitle = string.Format("UMP Filter Strategy");
            ModuleID = 2101;
            AppType = (int)VoiceCyber.UMP.Common.AppType.UMPClient;
        }

        protected override void InitSessionInfo()
        {
            base.InitSessionInfo();

            AppServerInfo appServerInfo = new AppServerInfo();
            appServerInfo.Protocol = "http";
            appServerInfo.Address = "192.168.6.7";
            appServerInfo.Port = 8081;
            appServerInfo.SupportHttps = false;
            appServerInfo.SupportNetTcp = false;
            Session.AppServerInfo = appServerInfo;

            DatabaseInfo dbInfo = new DatabaseInfo();
            dbInfo.TypeID = 2;
            dbInfo.Host = "192.168.4.182";
            dbInfo.Port = 1433;
            dbInfo.DBName = "UMPDataDB04121";
            dbInfo.LoginName = "PFDEV";
            dbInfo.Password = "PF,123";
            Session.DatabaseInfo = dbInfo;
            Session.DBType = dbInfo.TypeID;
            Session.DBConnectionString = dbInfo.GetConnectionString();
        }

        protected override void SetView()
        {
            base.SetView();

            CurrentView = new FilterConditionMainView();
            CurrentView.PageName = "ChanMonitorMainView";
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
                webRequest.ListData.Add("21");
                webRequest.ListData.Add("2101");
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
