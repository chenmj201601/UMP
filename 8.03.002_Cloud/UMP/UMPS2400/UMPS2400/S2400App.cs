using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using UMPS2400.Service11012;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;

namespace UMPS2400
{
    public class S2400App : UMPApp
    {
        public S2400App(bool runAsModule)
            : base(runAsModule)
        {

        }

        public S2400App(IRegionManager regionManager,
            IEventAggregator eventAggregator,
            IAppControlService appController)
            : base(regionManager, eventAggregator, appController)
        {
        }

        #region 定义全局变量
        //数据库当前时间，Refresh后重新刷新数据
        public static DateTime GolbalCurrentEncryptionDBTime = new DateTime();
        public EncryptMainPage mainPage = null;

        /// <summary>
        /// S2400的子模块 
        /// </summary>
        public enum S2400Module
        {
            KeyGenServer = 2401,
            EncryptionPolicy = 2402,
            ServerPolicyBinding = 2403,
            KeyRemindSetting = 2404
        }

        //当前加载的子模块
        public static S2400Module CurrentLoadingModule;
        #endregion

        /// <summary>
        /// 设置模块ID和Name
        /// </summary>
        protected override void SetAppInfo()
        {
            base.SetAppInfo();
            AppName = "UMPS2400";
            ModuleID = 2400;
            AppTitle =  string.Format("Encryption Management");
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
        //            EncryptMainPage mainView = new EncryptMainPage();
        //            mainView.CurrentApp = Current;
        //            var app = AppController.ListAppConfigs.FirstOrDefault(a => a.ModuleID == ModuleID);
        //            if (app != null)
        //            {
        //                RegionManager.Regions[app.PanelName].Add(mainView);
        //            }
        //        }

        //        if (!string.IsNullOrEmpty(StartArgs))
        //        {

        //            switch (StartArgs)
        //            {
        //                case "2401":
        //                    CurrentLoadingModule = S2400Module.KeyGenServer;
        //                    break;
        //                case "2402":
        //                    CurrentLoadingModule = S2400Module.EncryptionPolicy;
        //                    break;
        //                case "2403":
        //                    CurrentLoadingModule = S2400Module.ServerPolicyBinding;
        //                    break;
        //                case "2404":
        //                    CurrentLoadingModule = S2400Module.KeyRemindSetting;
        //                    break;
        //            }
        //            InitLanguageInfos(StartArgs);
        //            WriteLog("Loading " + StartArgs);

        //        }
        //        else
        //        {
        //            CurrentLoadingModule = S2400Module.EncryptionPolicy;
        //            InitLanguageInfos("2402");
        //            WriteLog("data is null,Loading 2402");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //        WriteLog("App Init()," + ex.Message);
        //    }
        //}

        protected override void SetView()
        {
            base.SetView();
            CurrentView = new EncryptMainPage();
            CurrentView.PageName = "EncryptMainPage";

            if (!string.IsNullOrEmpty(StartArgs))
            {
                switch (StartArgs)
                {
                    case "2401":
                        CurrentLoadingModule = S2400Module.KeyGenServer;
                        break;
                    case "2402":
                        CurrentLoadingModule = S2400Module.EncryptionPolicy;
                        break;
                    case "2403":
                        CurrentLoadingModule = S2400Module.ServerPolicyBinding;
                        break;
                    case "2404":
                        CurrentLoadingModule = S2400Module.KeyRemindSetting;
                        break;
                }
                WriteLog("Loading " + StartArgs);
            }
            else
            {
                CurrentLoadingModule = S2400Module.ServerPolicyBinding;
                WriteLog("data is null,Loading 2403");
            }
        }

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
                //ListLanguageInfos.Clear();
                VoiceCyber.UMP.Communications.WebRequest webRequest = new VoiceCyber.UMP.Communications.WebRequest();
                webRequest.Code = (int)RequestCode.WSGetLangList;
                webRequest.Session = Session;
                webRequest.ListData.Add(Session.LangTypeInfo.LangID.ToString());
                webRequest.ListData.Add(string.Empty);
                webRequest.ListData.Add("24");
                webRequest.ListData.Add(string.Format("{0}{1}{2}{1}{3}{1}{4}","2401",ConstValue.SPLITER_CHAR,"2402","2403","2404"));
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

    }
}
