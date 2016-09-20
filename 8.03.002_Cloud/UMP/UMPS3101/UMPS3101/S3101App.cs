//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    14d15740-7647-43f5-b45d-c5d6e8d0e3c0
//        CLR Version:              4.0.30319.42000
//        Name:                     S3101App
//        Computer:                 DESKTOP-VUMCK8M
//        Organization:             VoiceCyber
//        Namespace:                UMPS3101
//        File Name:                S3101App
//
//        created by Charley at 2016/2/23 16:21:37
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Linq;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using UMPS3101.Wcf11012;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common31011;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;


namespace UMPS3101
{
    public class S3101App : UMPApp
    {
        public static bool IsModifyScoreSheet;
        public static BasicScoreSheetInfo CurrentScoreSheetInfo;
        /// <summary>
        /// 分组管理方式，E虚拟分机，A座席，R真实分机；
        /// </summary>
        public static string GroupingWay;

        public S3101App(bool runAsModule)
            : base(runAsModule)
        {

        }

        public S3101App(IRegionManager regionManager,
            IEventAggregator eventAggregator,
            IAppControlService appController)
            : base(regionManager, eventAggregator, appController)
        {

        }

        protected override void SetAppInfo()
        {
            base.SetAppInfo();

            AppName = "UMPS3101";
            AppTitle = string.Format("ScoreSheet Management");
            ModuleID = 3101;
            AppType = (int)VoiceCyber.UMP.Common.AppType.UMPClient;
        }

        protected override void Init()
        {
            base.Init();

            LoadGroupingMethodParams();
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
            dbInfo.DBName = "UMPDataDB0516";
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

            CurrentView = new SSMMainView();
            CurrentView.PageName = "SSMMainPage";
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
                webRequest.ListData.Add("3101");
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


        #region 权限管理添加真实分机--虚拟分机没有实际意义，不管。
        private void LoadGroupingMethodParams()
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)RequestCode.WSGetGlobalParamList;
                webRequest.Session = Session;
                webRequest.ListData.Add("11");
                webRequest.ListData.Add("12010401");
                webRequest.ListData.Add("120104");
                Service11012Client client = new Service11012Client(WebHelper.CreateBasicHttpBinding(Session),
                    WebHelper.CreateEndpointAddress(Session.AppServerInfo, "Service11012"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowExceptionMessage(string.Format("WSFail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData.Count <= 0) { return; }
                string str = webReturn.ListData[0];
                str = str.Replace("&#x1B;", "");
                OperationReturn optReturn = XMLHelper.DeserializeObject<GlobalParamInfo>(str);
                if (!optReturn.Result)
                {
                    ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                GlobalParamInfo GlobalParamInfo = optReturn.Data as GlobalParamInfo;
                if (GlobalParamInfo == null) { return; }
                string tempGroupWay = GlobalParamInfo.ParamValue.Substring(8);
                GroupingWay = tempGroupWay;
            }
            catch (Exception ex)
            {
                ShowExceptionMessage(ex.Message);
            }
        }

        #endregion
      
    }
}
