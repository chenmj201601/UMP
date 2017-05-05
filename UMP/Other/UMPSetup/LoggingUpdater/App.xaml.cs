using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;
using VoiceCyber.Common;

namespace LoggingUpdater
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App
    {
        public static string AppName = "LoggingUpdater";
        /// <summary>
        /// 升级模式
        /// 0：普通升级，通过双击exe，指定升级包或搜索升级包进行升级
        /// 1：在线升级，LoggingUpdater在后台与UMPUpdater连接通讯完成在线升级
        /// </summary>
        public static int UpdateMode = 0;

        private static LogOperator mLogOperator;

        protected override void OnStartup(StartupEventArgs e)
        {
            CreateLogOperator();
            string[] args = e.Args;
            if (args.Length > 0)
            {
                if (args.Length < 2)
                {
                    WriteLog("Startup", string.Format("Args count invalid."));
                    return;
                }
                //在线升级
                UpdateMode = 1;
                WriteLog("Startup", string.Format("Online update mode"));
            }
            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (mLogOperator != null)
            {
                mLogOperator.Stop();
            }
            mLogOperator = null;
            base.OnExit(e);
        }


        #region LogOperator

        private void CreateLogOperator()
        {
            try
            {
                string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                    string.Format("{0}\\{1}\\Logs", "UMP", AppName));
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                mLogOperator = new LogOperator();
                mLogOperator.LogPath = path;
                mLogOperator.Start();
                string strInfo = string.Empty;
                strInfo += string.Format("AppInfo\r\n");
                strInfo += string.Format("\tLogPath:{0}\r\n", path);
                strInfo += string.Format("\tExePath:{0}\r\n", AppDomain.CurrentDomain.BaseDirectory);
                strInfo += string.Format("\tName:{0}\r\n", AppDomain.CurrentDomain.FriendlyName);
                strInfo += string.Format("\tVersion:{0}\r\n",
                    System.Reflection.Assembly.GetExecutingAssembly().GetName().Version);
                strInfo += string.Format("\tHost:{0}\r\n", Environment.MachineName);
                strInfo += string.Format("\tAccount:{0}", Environment.UserName);
                WriteLog("AppLoad", strInfo);
            }
            catch { }
        }
        /// <summary>
        /// 写运行日志
        /// </summary>
        /// <param name="category">类别</param>
        /// <param name="msg">消息内容</param>
        public static void WriteLog(string category, string msg)
        {
            if (mLogOperator != null)
            {
                mLogOperator.WriteLog(LogMode.Info, category, msg);
            }
        }
        /// <summary>
        /// 写运行日志
        /// </summary>
        /// <param name="msg">消息类别</param>
        public static void WriteLog(string msg)
        {
            if (mLogOperator != null)
            {
                mLogOperator.WriteLog(LogMode.Info, AppName, msg);
            }
        }

        #endregion

    }
}
