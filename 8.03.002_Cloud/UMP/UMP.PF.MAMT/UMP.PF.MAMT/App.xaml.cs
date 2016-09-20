using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using UMP.PF.MAMT.Classes;
using System.IO;
using System.Windows.Resources;
using XmlHelper;

namespace UMP.PF.MAMT
{
    public partial class App : Application
    {
        /// <summary>
        /// 系统中分隔符char(27)
        /// </summary>
        public static string GStrSpliterCharater = string.Empty;

        /// <summary>
        /// 当前应用程序路径
        /// </summary>
        public static string GStrApplicationDirectory = string.Empty;

        /// <summary>
        /// 当前机器基本信息
        /// </summary>
        public static ComputerInformation IComputerInfo = new ComputerInformation();

        /// <summary>
        /// 当前操作系统的用户 ProgramData 路径
        /// </summary>
        public static string GStrLoginUserApplicationDataPath = string.Empty;

        /// <summary>
        /// 当前IIS基本信息
        /// </summary>
        public static IISInformation IIISInfo = new IISInformation();

        /// <summary>
        /// 默认语言
        /// </summary>
        public static string GStrDefaultLanguage = string.Empty;

        /// <summary>
        /// 当前语言
        /// </summary>
        public static string GStrCurrentLanguage = string.Empty;

        /// <summary>
        /// 当前应用程序可以切换的语言
        /// </summary>
        public static List<string> GLstLanguages = new List<string>();

        /// <summary>
        /// 当前正在编辑的语言项
        /// </summary>
        public static List<LanguageInfo> GLstLanguageItemInEdit = new List<LanguageInfo>();
 
        /// <summary>
        /// 已经连接的UMP应用程序（从客户端配置文件中读取）
        /// </summary>
        public static List<ServerInfomation> GlstServers = new List<ServerInfomation>();

        /// <summary>
        /// 当前正在连接的数据库信息
        /// </summary>
        public static DBInfo GCurrentDBServer ;

        /// <summary>
        /// 客户端当前连接的所有UMP应用程序
        /// </summary>
        public static List<ServerInfomation> GLstConnectedServers = new List<ServerInfomation>();

        /// <summary>
        /// 客户端当前正在使用的UMP服务器
        /// </summary>
        public static ServerInfomation GCurrentUmpServer;

        /// <summary>
        /// 当前用户的MyDocuments目录
        /// </summary>
        public static string GStrUserMyDocumentsDirectory = string.Empty;

        /// <summary>
        /// 保存当前登录的用户信息
        /// </summary>
        public static LoginUserInfo GCurrentUser = new LoginUserInfo();

        /// <summary>
        /// 保存正在创建的数据库信息
        /// </summary>
        public static DBInfo GCreatingDBInfo = new DBInfo();

        /// <summary>
        /// 创建oracle的连接字符串
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <returns></returns>
        public static string CreateOracleConnString(DBInfo dbInfo)
        {
           return "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=" + dbInfo.Host + ")(PORT=" + dbInfo.Port
                                    + ")))(CONNECT_DATA=(SERVICE_NAME=" + dbInfo.ServiceName + ")));User Id=" + dbInfo.LoginName + ";Password=" + dbInfo.Password + ";";
        }

        /// <summary>
        /// 创建mssql的连接字符串
        /// </summary>
        /// <param name="dbInfo"></param>
        /// <returns></returns>
        public static string CreateMSSqlConnString(DBInfo dbInfo)
        {
            string strConnString = string.Format("Data Source={0},{1};Initial Catalog={2};User Id={3};Password={4}",
                                dbInfo.Host, dbInfo.Port, dbInfo.ServiceName, dbInfo.LoginName, dbInfo.Password);
            return strConnString;
        }

        private void CreateSpliterCharater()
        {
            try
            {
                System.Text.ASCIIEncoding LAsciiEncoding = new System.Text.ASCIIEncoding();
                byte[] LByteArray = new byte[] { (byte)27 };
                string LStrCharacter = LAsciiEncoding.GetString(LByteArray);
                App.GStrSpliterCharater = LStrCharacter;
            }
            catch { App.GStrSpliterCharater = string.Empty; }
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            App.GStrApplicationDirectory = Environment.CurrentDirectory;
            CreateSpliterCharater();
            //加载默认语言
            AboutLanguage.LoadDefaultLanguage();
            //获得语言列表
            AboutLanguage.GetLanguageList();
            //获得所有已经配置过的服务器信息
            GlstServers = ServerConfigOperationInLocal.GetAllServerInfo();

            GetUserMyDocumentsDirectory();
        }


        /// <summary>
        /// 画窗口背景
        /// </summary>
        /// <param name="AWindowTarger"></param>
        public static void DrawWindowsBackGround(Window AWindowTarger,string strImagePath)
        {
            DrawingBackground.DrawWindowsBackgond(AWindowTarger, strImagePath);
        }

        public static void DrawWindowsBackGround(System.Windows.Controls.UserControl AWindowTarger, string strImagePath)
        {
            DrawingBackground.DrawWindowsBackgond(AWindowTarger, strImagePath);
        }

        private void GetUserMyDocumentsDirectory()
        {
            try
            {
                GStrUserMyDocumentsDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }
            catch { GStrUserMyDocumentsDirectory = string.Empty; }
        }
    }
}
