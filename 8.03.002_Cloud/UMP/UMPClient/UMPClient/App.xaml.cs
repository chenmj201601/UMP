using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using UMPClient.Models;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;

namespace UMPClient
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App
    {

        #region Members

        public static LogOperator LogOperator;
        public static string AppName = "UMPClient";

        public static int LangID;
        public static List<LanguageInfo> ListLanguageInfos;

        #endregion


        protected override void OnStartup(StartupEventArgs e)
        {
            ListLanguageInfos = new List<LanguageInfo>();
            CreateLogOperator();
            LangID = 1033;
            try
            {
                LangID = GetUserDefaultUILanguage();
            }
            catch { }
            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (LogOperator != null)
            {
                LogOperator.Stop();
                LogOperator = null;
            }
            base.OnExit(e);
        }


        #region Languages

        public static void LoadAllLanguages()
        {
            try
            {
                ListLanguageInfos.Clear();
                int langID = LangID;
                var resource =
                    GetResourceStream(new Uri(string.Format("Languages/{0}.XML", langID), UriKind.RelativeOrAbsolute));
                if (resource == null)
                {
                    WriteLog("LoadAllLanguages", string.Format("Language file not exist."));
                    return;
                }
                var stream = resource.Stream;
                if (stream == null)
                {
                    WriteLog("LoadAllLanguages", string.Format("Stream is null."));
                    return;
                }
                StreamReader reader = new StreamReader(stream);
                string strContent = reader.ReadToEnd();
                OperationReturn optReturn = XMLHelper.DeserializeObject<LangLister>(strContent);
                if (!optReturn.Result)
                {
                    WriteLog("LoadAllLanguages",
                        string.Format("Read language file fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                LangLister lister = optReturn.Data as LangLister;
                if (lister == null)
                {
                    WriteLog("LoadAllLanguages", string.Format("LangLister is null"));
                    return;
                }
                for (int i = 0; i < lister.ListLanguageInfos.Count; i++)
                {
                    var lang = lister.ListLanguageInfos[i];
                    ListLanguageInfos.Add(lang);
                }
                WriteLog("LoadAllLanguages", string.Format("End.\t{0}", ListLanguageInfos.Count));
            }
            catch (Exception ex)
            {
                WriteLog("LoadAllLanguages", string.Format("Fail.\t{0}", ex.Message));
            }
        }

        public static string GetLanguageInfo(string name, string display)
        {
            try
            {
                var lang = ListLanguageInfos.FirstOrDefault(l => l.LangID == LangID && l.Name == name);
                if (lang == null)
                {
                    return display;
                }
                return lang.Display;
            }
            catch
            {
                return display;
            }
        }

        [DllImport("kernel32.dll")]
        private static extern int GetUserDefaultUILanguage();

        #endregion


        #region LogOperator

        private void CreateLogOperator()
        {
            try
            {
                string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                    string.Format("UMP\\{0}\\Logs", AppName));
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                LogOperator = new LogOperator();
                LogOperator.LogPath = path;
                LogOperator.Start();
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
            if (LogOperator != null)
            {
                LogOperator.WriteLog(LogMode.Info, category, msg);
            }
        }
        /// <summary>
        /// 写运行日志
        /// </summary>
        /// <param name="msg">消息类别</param>
        public static void WriteLog(string msg)
        {
            if (LogOperator != null)
            {
                LogOperator.WriteLog(LogMode.Info, AppName, msg);
            }
        }

        #endregion
    }
}
