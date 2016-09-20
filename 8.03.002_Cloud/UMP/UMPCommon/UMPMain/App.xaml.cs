using System;
using System.Threading;
using System.Windows;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;

namespace UMPMain
{ 
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App
    {
        public static SessionInfo Session;
        public static string AppName = "UMPMain";
        public static NetPipeHelper NetPipeHelper;

        protected override void OnStartup(StartupEventArgs e)
        {
            Init();
            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (NetPipeHelper != null)
            {
                NetPipeHelper.Stop();
            }
            base.OnExit(e);
        }

        private void Init()
        {
            Session = new SessionInfo();
            Session.SessionID = Guid.NewGuid().ToString();
            Session.AppName = AppName;
            Session.LastActiveTime = DateTime.Now;

            RentInfo rentInfo = new RentInfo();
            rentInfo.ID = 1000000000000000001;
            rentInfo.Token = "00000";
            rentInfo.Domain = "voicecyber.com";
            rentInfo.Name = "voicecyber";
            Session.RentInfo = rentInfo;
            Session.RentID = 1000000000000000001;

            UserInfo userInfo = new UserInfo();
            userInfo.UserID = 1020000000000000001;
            userInfo.Account = "administrator";
            userInfo.UserName = "Administrator";
            Session.UserInfo = userInfo;
            Session.UserID = 1020000000000000001;

            RoleInfo roleInfo = new RoleInfo();
            roleInfo.ID = 1060000000000000001;
            roleInfo.Name = "System Admin";
            Session.RoleInfo = roleInfo;
            Session.RoleID = 1060000000000000001;

            AppServerInfo serverInfo = new AppServerInfo();
            serverInfo.Protocol = "http";
            serverInfo.Address = "192.168.6.75";
            serverInfo.Port = 8081;
            Session.AppServerInfo = serverInfo;

            ThemeInfo themeInfo = new ThemeInfo();
            themeInfo.Name = "Default";
            themeInfo.Color = "Brown";
            Session.ThemeInfo = themeInfo;
            Session.ThemeName = "Default";

            themeInfo = new ThemeInfo();
            themeInfo.Name = "Style01";
            themeInfo.Color = "Green";
            Session.SupportThemes.Add(themeInfo);

            themeInfo = new ThemeInfo();
            themeInfo.Name = "Style02";
            themeInfo.Color = "Yellow";
            Session.SupportThemes.Add(themeInfo);

            themeInfo = new ThemeInfo();
            themeInfo.Name = "Style03";
            themeInfo.Color = "Brown";
            Session.SupportThemes.Add(themeInfo);

            themeInfo = new ThemeInfo();
            themeInfo.Name = "Style04";
            themeInfo.Color = "Blue";
            Session.SupportThemes.Add(themeInfo);

            LangTypeInfo langType = new LangTypeInfo();
            langType.LangID = 1033;
            langType.LangName = "en-us";
            langType.Display = "English";
            Session.LangTypeInfo = langType;
            Session.LangTypeID = 1033;
            Session.SupportLangTypes.Add(langType);

            langType = new LangTypeInfo();
            langType.LangID = 2052;
            langType.LangName = "zh-cn";
            langType.Display = "简体中文";
            Session.SupportLangTypes.Add(langType);

            DatabaseInfo dbInfo = new DatabaseInfo();
            dbInfo.TypeID = 3;
            dbInfo.TypeName = "ORCL";
            dbInfo.Host = "192.168.4.182";
            dbInfo.Port = 1521;
            dbInfo.DBName = "PFOrcl";
            dbInfo.LoginName = "PFDEV";
            dbInfo.Password = "PF,123";
            Session.DatabaseInfo = dbInfo;
            Session.DBType = dbInfo.TypeID;
            Session.DBConnectionString = dbInfo.GetConnectionString();

            //DatabaseInfo dbInfo = new DatabaseInfo();
            //dbInfo.TypeID = 2;
            //dbInfo.TypeName = "MSSQL";
            //dbInfo.Host = "192.168.4.182";
            //dbInfo.Port = 1433;
            //dbInfo.DBName = "UMPDataDB";
            //dbInfo.LoginName = "sa";
            //dbInfo.Password = "PF,123";
            //Session.DBType = dbInfo.TypeID;
            //Session.DBConnectionString = dbInfo.GetConnectionString();
            //Session.DatabaseInfo = dbInfo;

            Session.InstallPath = @"C:\UMPRelease";
        }

        #region Basic

        public static void ShowExceptionMessage(string msg)
        {
            MessageBox.Show(msg, AppName, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public static void ShowInfoMessage(string msg)
        {
            MessageBox.Show(msg, AppName, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        #endregion

        #region NetPipe

        public static void CreateNetPipeService()
        {
            NetPipeHelper = new NetPipeHelper(true, AppName);
            NetPipeHelper.DealMessageFunc += mNetPipeHelper_DealMessageFunc;
            try
            {
                NetPipeHelper.Start();
                //ShowInfoMessage(string.Format("Service started."));
            }
            catch (Exception ex)
            {
                ShowExceptionMessage(string.Format("Start service fail.\t{0}", ex.Message));
            }
        }

        private static WebReturn mNetPipeHelper_DealMessageFunc(WebRequest arg)
        {
            WebReturn webReturn = new WebReturn();
            webReturn.Result = true;
            webReturn.Code = 0;
            try
            {
                var code = arg.Code;
                var strData = arg.Data;
                ThreadPool.QueueUserWorkItem(a => ShowExceptionMessage(string.Format("Recieve:{0}\t{1}", code, strData)));
                return webReturn;
            }
            catch (Exception ex)
            {
                webReturn.Result = false;
                webReturn.Code = Defines.RET_FAIL;
                webReturn.Message = ex.Message;
                return webReturn;
            }
        }

        public static WebReturn SendNetPipeMessage(WebRequest request)
        {
            if (NetPipeHelper == null) { CreateNetPipeService(); }
            WebReturn webReturn = new WebReturn();
            webReturn.Result = true;
            webReturn.Code = 0;
            try
            {
                if (NetPipeHelper == null)
                {
                    webReturn.Result = false;
                    webReturn.Code = Defines.RET_OBJECT_NULL;
                    webReturn.Message = string.Format("NetPipe Service is null");
                    return webReturn;
                }
                var temp = NetPipeHelper.SendMessage(request, "UMPS3101");
                return temp;
            }
            catch (Exception ex)
            {
                webReturn.Result = false;
                webReturn.Code = Defines.RET_FAIL;
                webReturn.Message = ex.Message;
                return webReturn;
            }
        }

        private static void SendModuleCloseMessage()
        {
            if (NetPipeHelper == null)
            {
                CreateNetPipeService();
            }
            WebRequest request = new WebRequest();
            request.Code = (int)RequestCode.CSModuleClose;
            request.Session = Session;
            SendNetPipeMessage(request);
        }

        #endregion

    }
}
