using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UMPS5100.Service11012;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;

namespace UMPS5100
{
    public class S5100App : UMPApp
    {
        public static MainPage mainPage = null;

        public S5100App(bool runAsModule)
            : base(runAsModule)
        {

        }

        public S5100App(IRegionManager regionManager,
           IEventAggregator eventAggregator,
           IAppControlService appController)
            : base(regionManager, eventAggregator, appController)
        {

        }
        /// <summary>
        /// S5100的子模块 
        /// </summary>
        public enum S5100Module
        {
            //标签等级管理
            BookmarkLevel = 5101,
            //关键词管理
            KeyWord = 5102,
            //语音转文字
            SpeechToTtext = 5103
        }

        //当前加载的子模块
        public static S5100Module CurrentLoadingModule;

        /// <summary>
        /// 设置模块ID和Name
        /// </summary>
        protected override void SetAppInfo()
        {
            base.SetAppInfo();
            AppName = "UMPS5100";
            ModuleID = 5100;
            AppTitle = string.Format("Speech Analysis");
            AppType = (int)VoiceCyber.UMP.Common.AppType.UMPClient;
        }

        protected override void InitSessionInfo()
        {
            base.InitSessionInfo();
            if (Session == null) { return; }
            AppServerInfo serverInfo = new AppServerInfo();
            serverInfo.Protocol = "http";
            serverInfo.Address = "192.168.6.7";
            serverInfo.Port = 8081;
            serverInfo.SupportHttps = false;
            Session.AppServerInfo = serverInfo;

            //AppServerInfo serverInfo = new AppServerInfo();
            //serverInfo.Protocol = "https";
            //serverInfo.Address = "192.168.6.86";
            //serverInfo.Port = 8082;
            //serverInfo.SupportHttps = true;
            //Session.AppServerInfo = serverInfo;

            //DatabaseInfo dbInfo = new DatabaseInfo();
            //dbInfo.TypeID = 3;
            //dbInfo.TypeName = "ORCL";
            //dbInfo.Host = "192.168.4.182";
            //dbInfo.Port = 1521;
            //dbInfo.DBName = "PFOrcl";
            //dbInfo.LoginName = "PFDEV";
            //dbInfo.Password = "PF,123";
            //Session.DatabaseInfo = dbInfo;
            //Session.DBType = dbInfo.TypeID;
            //Session.DBConnectionString = dbInfo.GetConnectionString();

            DatabaseInfo dbInfo = new DatabaseInfo();
            dbInfo.TypeID = 2;
            dbInfo.TypeName = "MSSQL";
            dbInfo.Host = "192.168.4.182";
            dbInfo.Port = 1433;
            dbInfo.DBName = "UMPDataDB0412";
            dbInfo.LoginName = "PFDEV";
            dbInfo.Password = "PF,123";
            Session.DatabaseInfo = dbInfo;
            Session.DBType = dbInfo.TypeID;
            Session.DBConnectionString = dbInfo.GetConnectionString();
        }

        //protected override void Init()
        //{
        //    base.Init();
        //    try
        //    {
        //        if (Session != null)
        //        {
        //            WriteLog("AppLoad", string.Format("SessionInfo\r\n{0}", Session.LogInfo()));
        //        }
        //        if (RunAsModule)
        //        {
        //            MainPage mainView = new MainPage();
        //            mainView.CurrentApp = Current;
        //            var app = AppController.ListAppConfigs.FirstOrDefault(a => a.ModuleID == ModuleID);
        //            if (app != null)
        //            {
        //                RegionManager.Regions[app.PanelName].Add(mainView);
        //            }
        //        }

        //        if (!string.IsNullOrEmpty(StartArgs))
        //        {
        //            var data = StartArgs;
        //            switch (data.ToString())
        //            {
        //                case "5101":
        //                    CurrentLoadingModule = S5100Module.BookmarkLevel;
        //                    break;
        //                case "5102":
        //                    CurrentLoadingModule = S5100Module.KeyWord;
        //                    break;
        //                case "5103":
        //                    CurrentLoadingModule = S5100Module.SpeechToTtext;
        //                    break;
        //            }
        //            InitLanguageInfos(data.ToString());
        //            WriteLog("Loading " + data.ToString());
        //        }
        //        else
        //        {
        //            CurrentLoadingModule = S5100Module.KeyWord;
        //            InitLanguageInfos("5102");
        //            WriteLog("data is null,Loading 5101");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        WriteLog("CurrentApp Init()," + ex.Message);
        //    }

        //}


        protected override void InitLanguageInfos()
        {
            base.InitLanguageInfos();
            InitLanguageInfo();
        }

        public void InitLanguageInfo()
        {
            try
            {
                if (Session == null || Session.LangTypeInfo == null) { return; }
                VoiceCyber.UMP.Communications.WebRequest webRequest = new VoiceCyber.UMP.Communications.WebRequest();
                webRequest.Code = (int)RequestCode.WSGetLangList;
                webRequest.Session = Session;
                webRequest.ListData.Add(Session.LangTypeInfo.LangID.ToString());
                webRequest.ListData.Add(string.Empty);
                webRequest.ListData.Add(string.Format("51"));
                webRequest.ListData.Add(string.Format("{0}{1}{2}{1}{3}", "5101", ConstValue.SPLITER_CHAR, "5102", "5103"));
                webRequest.ListData.Add(string.Empty);
                webRequest.ListData.Add(string.Empty);
                MonitorHelper.AddWebRequest(webRequest);
                Service11012Client client = new Service11012Client(
                    WebHelper.CreateBasicHttpBinding(Session)
                    , WebHelper.CreateEndpointAddress(Session.AppServerInfo, "Service11012"));
                WebReturn webReturn = client.DoOperation(webRequest);
                MonitorHelper.AddWebReturn(webReturn);
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

                WriteLog("AppLang", string.Format("Init LanguageInfos end"));
            }
            catch (Exception ex)
            {
                WriteLog("InitLang", string.Format("InitLang fail.\t{0}", ex.Message));
            }
        }

        protected override void SetView()
        {
            base.SetView();
            CurrentView = new MainPage();
            CurrentView.PageName = "SpeechAnalysisMainPage";
            if (Session != null)
            {
                WriteLog("AppLoad", string.Format("SessionInfo\r\n{0}", Session.LogInfo()));
            }
            WriteLog("StartArgs = "+StartArgs);
            if (!string.IsNullOrEmpty(StartArgs))
            {
                var data = StartArgs;
                switch (data.ToString())
                {
                    case "5101":
                        CurrentLoadingModule = S5100Module.BookmarkLevel;
                        break;
                    case "5102":
                        CurrentLoadingModule = S5100Module.KeyWord;
                        break;
                    case "5103":
                        CurrentLoadingModule = S5100Module.SpeechToTtext;
                        break;
                }
               // InitLanguageInfo(data.ToString());
                WriteLog("Loading " + data.ToString());
            }
            else
            {
                CurrentLoadingModule = S5100Module.KeyWord;
               // InitLanguageInfo("5102");
                WriteLog("data is null,Loading 5102");
            }
        }
    }
}
