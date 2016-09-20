using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Controls;

namespace UMPS1600Demo
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App :UMPApp
    {
        protected override void InitSessionInfo()
        {
            base.InitSessionInfo();
            if (Session == null) { return; }
            //Session.UserID = 1021401010000000151;
            //Session.UserInfo.UserID = 1021401010000000151;
            //Session.UserInfo.Account = "charley";
            //Session.UserInfo.UserName = "charley";

            //Session.UserID = 1021401010000000152;
            //Session.UserInfo.UserID = 1021401010000000152;
            //Session.UserInfo.Account = "PFUser1";
            //Session.UserInfo.UserName = "PFUser1";

            Session.UserID = 1020000000000000001;
            Session.UserInfo.UserID = 1020000000000000001;
            Session.UserInfo.Account = "administrator";
            Session.UserInfo.UserName = "administrator";

            AppServerInfo serverInfo = new AppServerInfo();
            serverInfo.Protocol = "http";
            serverInfo.Address = "192.168.6.44";
            serverInfo.Port = 8081;
            serverInfo.SupportHttps = false;
            Session.AppServerInfo = serverInfo;

            Session.LangTypeID = 2052;

            //AppServerInfo serverInfo = new AppServerInfo();
            //serverInfo.Protocol = "https";
            //serverInfo.Address = "192.168.6.86";
            //serverInfo.Port = 8082;
            //serverInfo.SupportHttps = true;
            //Session.AppServerInfo = serverInfo;

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
            //dbInfo.DBName = "UMPDataDB1027";
            //dbInfo.LoginName = "PFDEV";
            //dbInfo.Password = "PF,123";
            //Session.DatabaseInfo = dbInfo;
            //Session.DBType = dbInfo.TypeID;
            //Session.DBConnectionString = dbInfo.GetConnectionString();
        }
    }
}
