using System;
using System.IO;
using System.Threading;
using VoiceCyber.Common;

namespace LoggingUpdater
{
    class Program
    {
        public static string AppName = "LoggingUpdater";
        public static bool IsWorking;

        private static LogOperator mLogOperator;
        private static UpdateEngine mEngine;
        private static int mWorkeTimeout = 60;             //LoggingUpdater工作的超时时间，超过超时时间将主动关闭LoggingUpdater程序

        static void Main(string[] args)
        {
            CreateLogOperator();
            WriteLog(LogMode.Info, "Welcome LoggingUpdater, You need do nothing just wait...");
            IsWorking = true;
            mEngine = new UpdateEngine();
            mEngine.Debug += WriteLog;
            mEngine.Start(args);
            while (IsWorking)
            {
                DateTime dt = mEngine.LastActiveTime;
                DateTime now = DateTime.Now;
                if ((now - dt).TotalSeconds > mWorkeTimeout)
                {
                    WriteLog(LogMode.Info, "Timeout, will be shutdown now.");
                    mEngine.Stop();
                    mEngine = null;
                    break;
                }
                Thread.Sleep(100);
            }
            if (mEngine != null)
            {
                mEngine.Stop();
                mEngine = null;
            }
            if (mLogOperator != null)
            {
                mLogOperator.Stop();
            }
            mLogOperator = null;
        }


        #region LogOperator

        private static void CreateLogOperator()
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
                WriteLog(LogMode.Info, strInfo);
            }
            catch { }
        }

        /// <summary>
        /// 写运行日志
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="category">类别</param>
        /// <param name="msg">消息内容</param>
        public static void WriteLog(LogMode mode, string category, string msg)
        {
            try
            {
                Console.WriteLine("{0}\t{1}\t{2}\t{3}", DateTime.Now.ToString("HH:mm:ss"), mode, category, msg);
            }
            catch { }
            if (mLogOperator != null)
            {
                mLogOperator.WriteLog(mode, category, msg);
            }
        }

        /// <summary>
        /// 写运行日志
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="msg">消息类别</param>
        public static void WriteLog(LogMode mode, string msg)
        {
            WriteLog(mode, AppName, msg);
        }

        #endregion

    }
}
