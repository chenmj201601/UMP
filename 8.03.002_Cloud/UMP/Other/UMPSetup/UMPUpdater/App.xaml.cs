using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using UMPUpdater.Models;
using VoiceCyber.Common;
using VoiceCyber.SharpZips.Zip;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Encryptions;

namespace UMPUpdater
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App
    {
        public static LogOperator mLogOperator;
        public static string AppName = "UMPUpdater";

        public static int LangID;
        public static List<LanguageInfo> ListLanguageInfos;

        protected override void OnStartup(StartupEventArgs e)
        {
            ListLanguageInfos = new List<LanguageInfo>();

            CreateLogOperator();
            string[] args = e.Args;
            if (args.Length > 0)
            {
                string strArgs0 = args[0];
                if (strArgs0.ToUpper() == "E")
                {
                    SaveUMPDataZip();
                    ExtractUMPData();
                }
                Shutdown(0);
            }
            LangID = 1033;
            try
            {
                LangID = GetUserDefaultUILanguage();
            }
            catch { }
            //LoadAllLanguages();
            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (mLogOperator != null)
            {
                mLogOperator.Stop();
                mLogOperator = null;
            }
            base.OnExit(e);
        }


        #region GetUMPData

        private void SaveUMPDataZip()
        {
            try
            {
                string strTargetDir =
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                        string.Format("UMP\\{0}\\Temp", AppName));
                if (!Directory.Exists(strTargetDir))
                {
                    Directory.CreateDirectory(strTargetDir);
                }
                string strTargetFile = Path.Combine(strTargetDir, "UMPData.zip");
                var rsUMPData =
                    GetResourceStream(new Uri("/UMPUpdater;component/Resources/UMPData.zip", UriKind.RelativeOrAbsolute));
                if (rsUMPData == null)
                {
                    return;
                }
                var stream = rsUMPData.Stream;
                if (stream == null)
                {
                    return;
                }
                var fs = File.Create(strTargetFile);
                byte[] buffer = new byte[1024];
                int num = stream.Read(buffer, 0, 1024);
                while (num > 0)
                {
                    fs.Write(buffer, 0, num);
                    num = stream.Read(buffer, 0, 1024);
                }
                fs.Flush();
                fs.Close();
                stream.Close();
            }
            catch (Exception ex)
            {
                WriteLog("SaveUMPDataZip", string.Format("Fail.\t{0}", ex.Message));
            }
        }

        private void ExtractUMPData()
        {
            try
            {
                string strZip = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                    "UMP\\UMPUpdater\\Temp\\UMPData.zip");
                if (!File.Exists(strZip))
                {
                    return;
                }
                string strDir = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                    "UMP\\UMPUpdater\\Temp\\UMPData");
                if (!Directory.Exists(strDir))
                {
                    Directory.CreateDirectory(strDir);
                }
                var stream = new FileStream(strZip, FileMode.Open, FileAccess.Read);
                using (var zipStream = new ZipInputStream(stream))
                {
                    ZipEntry theEntry;
                    while ((theEntry = zipStream.GetNextEntry()) != null)
                    {
                        string dirName = strDir;
                        string pathToZip = theEntry.Name;
                        if (!string.IsNullOrEmpty(pathToZip))
                        {
                            dirName = Path.GetDirectoryName(Path.Combine(dirName, pathToZip));
                        }
                        DateTime datetime = theEntry.DateTime;
                        if (string.IsNullOrEmpty(dirName))
                        {
                            continue;
                        }
                        string fileName = Path.GetFileName(pathToZip);
                        Directory.CreateDirectory(dirName);
                        if (!string.IsNullOrEmpty(fileName))
                        {
                            string filePath = Path.Combine(dirName, fileName);
                            using (FileStream streamWriter = File.Create(filePath))
                            {
                                int size;
                                byte[] data = new byte[2048];
                                while (true)
                                {
                                    size = zipStream.Read(data, 0, data.Length);
                                    if (size > 0)
                                        streamWriter.Write(data, 0, size);
                                    else
                                        break;
                                }
                                streamWriter.Close();

                                //还原文件的修改时间
                                FileInfo fileInfo = new FileInfo(filePath);
                                fileInfo.LastWriteTime = datetime;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLog("ExtractUMPData", string.Format("Fail.\t{0}", ex.Message));
            }
        }

        #endregion


        #region Languages

        public static void LoadAllLanguages()
        {
            try
            {
                ListLanguageInfos.Clear();
                int langID = LangID;
                OperationReturn optReturn;

                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                    string.Format("Languages\\{0}.XML", langID));
                if (!File.Exists(path))
                {
                    path = string.Format("/UMPUpdater;component/Languages/{0}.XML", langID);
                    var resource = GetResourceStream(new Uri(path, UriKind.RelativeOrAbsolute));
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
                    reader.Close();
                    optReturn = XMLHelper.DeserializeObject<LangLister>(strContent);
                    if (!optReturn.Result)
                    {
                        WriteLog("LoadAllLanguages", string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                }
                else
                {
                    optReturn = XMLHelper.DeserializeFile<LangLister>(path);
                    if (!optReturn.Result)
                    {
                        WriteLog("LoadAllLanguages", string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                }

                LangLister lister = optReturn.Data as LangLister;
                if (lister == null)
                {
                    WriteLog("LoadAllLanguages", string.Format("LangLister is null"));
                    return;
                }
                for (int i = 0; i < lister.ListLangInfos.Count; i++)
                {
                    var lang = lister.ListLangInfos[i];
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
            WriteLog(LogMode.Info, category, msg);
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="category"></param>
        /// <param name="msg"></param>
        public static void WriteLog(LogMode mode, string category, string msg)
        {
            if (mLogOperator != null)
            {
                mLogOperator.WriteLog(mode, category, msg);
            }
        }

        #endregion


        #region Encryption

        public static string EncryptStringM004(string strSource)
        {
            try
            {
                return ServerAESEncryption.EncryptString(strSource, EncryptionMode.AES256V04Hex);
            }
            catch
            {
                return strSource;
            }
        }

        public static string DecryptStringM004(string strSource)
        {
            try
            {
                return ServerAESEncryption.DecryptString(strSource, EncryptionMode.AES256V04Hex);
            }
            catch
            {
                return strSource;
            }
        }

        public static string EncryptStringM002(string strSource)
        {
            try
            {
                return ServerAESEncryption.EncryptString(strSource, EncryptionMode.AES256V02Hex);
            }
            catch
            {
                return strSource;
            }
        }

        public static string DecryptStringM002(string strSource)
        {
            try
            {
                return ServerAESEncryption.DecryptString(strSource, EncryptionMode.AES256V02Hex);
            }
            catch
            {
                return strSource;
            }
        }

        #endregion
    }
}
