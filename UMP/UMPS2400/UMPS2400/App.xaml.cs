using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.Deployment.Application;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Navigation;
using System.Windows.Threading;
using UMPS2400.MainUserControls;
using UMPS2400.Service11012;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Controls;

namespace UMPS2400
{ 
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App 
    {

        public static UMPApp CurrentApp;

        protected override void OnStartup(StartupEventArgs e)
        {
            CurrentApp = new S2400App(false);
            CurrentApp.Startup();
            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (Current != null)
            {
                CurrentApp.Exit();
            }
            base.OnExit(e);
        }

        //#region 定义全局变量 
        ////数据库当前时间，Refresh后重新刷新数据
        //public static DateTime GolbalCurrentEncryptionDBTime = new DateTime();
        //public static EncryptMainPage mainPage = null;

        ///// <summary>
        ///// S2400的子模块 
        ///// </summary>
        //public enum S2400Module
        //{
        //    KeyGenServer=2401,
        //    EncryptionPolicy=2402,
        //    ServerPolicyBinding=2403,
        //    KeyRemindSetting=2404
        //}

        ////当前加载的子模块
        //public static S2400Module CurrentLoadingModule;
        //#endregion

        ///// <summary>
        ///// 设置模块ID和Name
        ///// </summary>
        //protected override void SetAppInfo()
        //{
        //    base.SetAppInfo();
        //    AppName = "UMPS2400";
        //    ModuleID = 2400;
        //}

        //protected override void InitSessionInfo()
        //{
        //    base.InitSessionInfo();
        //    if (Session == null) { return; }
        //    AppServerInfo serverInfo = new AppServerInfo();
        //    serverInfo.Protocol = "http";
        //    serverInfo.Address = "192.168.6.7";
        //    serverInfo.Port = 8081;
        //    serverInfo.SupportHttps = false;
        //    Session.AppServerInfo = serverInfo;

        //    //AppServerInfo serverInfo = new AppServerInfo();
        //    //serverInfo.Protocol = "https";
        //    //serverInfo.Address = "192.168.6.86";
        //    //serverInfo.Port = 8082;
        //    //serverInfo.SupportHttps = true;
        //    //Session.AppServerInfo = serverInfo;

        //    DatabaseInfo dbInfo = new DatabaseInfo();
        //    dbInfo.TypeID = 3;
        //    dbInfo.TypeName = "ORCL";
        //    dbInfo.Host = "192.168.4.182";
        //    dbInfo.Port = 1521;
        //    dbInfo.DBName = "PFOrcl";
        //    dbInfo.LoginName = "PFDEV";
        //    dbInfo.Password = "PF,123";
        //    Session.DatabaseInfo = dbInfo;
        //    Session.DBType = dbInfo.TypeID;
        //    Session.DBConnectionString = dbInfo.GetConnectionString();

        //    //DatabaseInfo dbInfo = new DatabaseInfo();
        //    //dbInfo.TypeID = 2; 
        //    //dbInfo.TypeName = "MSSQL";
        //    //dbInfo.Host = "192.168.4.182";
        //    //dbInfo.Port = 1433;
        //    //dbInfo.DBName = "UMPDataDB1225";
        //    //dbInfo.LoginName = "PFDEV";
        //    //dbInfo.Password = "PF,123";
        //    //Session.DatabaseInfo = dbInfo;
        //    //Session.DBType = dbInfo.TypeID;
        //    //Session.DBConnectionString = dbInfo.GetConnectionString();
        //}

        //protected override void Init()
        //{
        //    base.Init();
        //    try
        //    {
        //        if (Session != null)
        //        {
        //            WriteLog("AppLoad", string.Format("SessionInfo\r\n{0}", Session.LogInfo()));
        //        }
        //        if (LoadingMessageReturn != null)
        //        {
        //            var data = LoadingMessageReturn.Data;
        //            switch (data.ToString())
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
        //            InitLanguageInfos(data.ToString());
        //            WriteLog("Loading " + data.ToString());
        //        }
        //        else
        //        {
        //            CurrentLoadingModule = S2400Module.EncryptionPolicy;
        //            InitLanguageInfos("2402");
        //            WriteLog("data is null,Loading 2403");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //        WriteLog("App Init()," + ex.Message);
        //    }

        //}

        //protected override void OnDealNetPipeEvent(WebRequest webRequest)
        //{
        //    base.OnDealNetPipeEvent(webRequest);
        //    if (webRequest.Code == (int)RequestCode.SCOperation)
        //    {
        //        string strData = webRequest.Data;
        //        switch (strData.ToString())
        //        {
        //            case "2401":
        //                CurrentLoadingModule = S2400Module.KeyGenServer;
        //                break;
        //            case "2402":
        //                CurrentLoadingModule = S2400Module.EncryptionPolicy;
        //                break;
        //            case "2403":
        //                CurrentLoadingModule = S2400Module.ServerPolicyBinding;
        //                break;
        //            case "2404":
        //                CurrentLoadingModule = S2400Module.KeyRemindSetting;
        //                break;
        //        }
        //        WriteLog("CurrentLoadingModule : " + CurrentLoadingModule.ToString());
        //        InitLanguageInfos(strData);
        //        ThreadPool.QueueUserWorkItem(a =>
        //        {
        //            Dispatcher.Invoke(new Action(() =>   mainPage.Load_Page()));
        //        });
        //    }
        //}

        //public static void InitLanguageInfos(string strModuleID)
        //{
        //    try
        //    {
                
        //        if (Session == null || Session.LangTypeInfo == null) { return; }
        //        ListLanguageInfos.Clear();
        //        VoiceCyber.UMP.Communications.WebRequest webRequest = new VoiceCyber.UMP.Communications.WebRequest();
        //        webRequest.Code = (int)RequestCode.WSGetLangList;
        //        webRequest.Session = Session;
        //        webRequest.ListData.Add(Session.LangTypeInfo.LangID.ToString());
        //        webRequest.ListData.Add(string.Empty);
        //        webRequest.ListData.Add(string.Format("0{0}24", ConstValue.SPLITER_CHAR));
        //        webRequest.ListData.Add(string.Format("0{0}{1}", ConstValue.SPLITER_CHAR,strModuleID));
        //        webRequest.ListData.Add(string.Empty);
        //        webRequest.ListData.Add(string.Empty);
        //        MonitorHelper.AddWebRequest(webRequest);
        //        Service11012Client client = new Service11012Client(
        //            WebHelper.CreateBasicHttpBinding(Session)
        //            , WebHelper.CreateEndpointAddress(Session.AppServerInfo, "Service11012"));
        //        WebReturn webReturn = client.DoOperation(webRequest);
        //        MonitorHelper.AddWebReturn(webReturn);
        //        client.Close();
        //        if (!webReturn.Result)
        //        {
        //            ShowExceptionMessage(string.Format("{0}\t{1}", webReturn.Code, webReturn.Message));
        //        }
        //        for (int i = 0; i < webReturn.ListData.Count; i++)
        //        {
        //            OperationReturn optReturn = XMLHelper.DeserializeObject<LanguageInfo>(webReturn.ListData[i]);
        //            if (!optReturn.Result)
        //            {
        //                ShowExceptionMessage(string.Format("{0}\t{1}", optReturn.Code, optReturn.Message));
        //                return;
        //            }
        //            LanguageInfo langInfo = optReturn.Data as LanguageInfo;
        //            if (langInfo == null)
        //            {
        //                ShowExceptionMessage(string.Format("LanguageInfo is null"));
        //                return;
        //            }
        //            ListLanguageInfos.Add(langInfo);
        //        }

        //        WriteLog("AppLang", string.Format("Init LanguageInfos end"));
        //    }
        //    catch (Exception ex)
        //    {
        //        WriteLog("InitLang", string.Format("InitLang fail.\t{0}", ex.Message));
        //    }
        //}
    }
}
