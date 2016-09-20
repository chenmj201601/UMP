using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Configuration;


namespace FileLog
{
    class FileLog
    {
        public static string FilePath = System.AppDomain.CurrentDomain.BaseDirectory.ToString() + "\\Log";//存放日志的路径
        public static long FileMaxSize = 10240000;//日志文件的最大长度
        public static string FileName = string.Empty;//日志文件名
        public static bool EnabledConsole = true;//是否向控制台输出日志
        public static bool EnabledDebug = true;//是否充许Debug信息输出
        public static bool EnabledInfo = true;//是否充许Info信息输出
        public static bool EnabledWarning = true;//是否充许Warning信息输出
        public static bool EnableError = true;//是否充许Error信息输出
        public static int  DateCountSaveLog = 30;//保留日志的天数

        public enum LogLevel //日记级别
        {
            DEBUG = 0,
            INFO = 1,
            WARNING = 2,
            ERROR = 3
        }

        private string m_Local = string.Empty;//信息所在的位置

        public string Local
        {
            get { return m_Local; }
            set { m_Local = value; }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="local"></param>
        public FileLog(string local)
        {
            this.Local = local;
        }
        /// <summary>
        /// 将日志级别转化为相应的字符串
        /// </summary>
        /// <param name="logType"></param>
        /// <returns></returns>
        public static string GetLogLevelString(LogLevel logLevel)
        {
            string ret = "INFO";
            switch (logLevel)
            {
                case LogLevel.DEBUG:
                    ret = "DEBUG";
                    break;
                case LogLevel.INFO:
                    ret = "INFO";
                    break;
                case LogLevel.WARNING:
                    ret = "WARNING";
                    break;
                case LogLevel.ERROR:
                    ret = "ERROR";
                    break;
                default:
                    break;
            }
            return ret;
        }


        /// <summary>
        /// 将文本写入日志文件
        /// </summary>
        /// <param name="txt"></param>
        private static void WriteTextToFile(string txt)
        {

            if ((FileLog.FilePath != string.Empty) && (!Directory.Exists(FileLog.FilePath)))
            {
                Directory.CreateDirectory(FileLog.FilePath);
            }
            
            if (FileLog.FileName == string.Empty)
                FileLog.FileName = DateTime.Now.ToString("yyyyMMddHHmmss") + ".log";

            StreamWriter sw = null;
            FileInfo finfo = new FileInfo(FileLog.FilePath + "\\" + FileLog.FileName);
            if (!finfo.Exists)
            {
                sw = File.CreateText(FileLog.FilePath + "\\" + FileLog.FileName);
            }
            else
            {
                if (finfo.Length >= FileLog.FileMaxSize)
                {
                    FileName = DateTime.Now.ToString("yyyyMMddHHmmss") + ".log";
                    sw = File.CreateText(FileLog.FilePath + "\\" + FileLog.FileName);
                }
                else
                    sw = new StreamWriter(finfo.OpenWrite());

            }

            sw.BaseStream.Seek(0, SeekOrigin.End);

            sw.WriteLine(txt);
            sw.Flush();
            sw.Close();
        }

        /// <summary>
        /// 通过配置文件对FileLog进行初始化
        /// </summary>
        /// <param name="logConfigFile"></param>
        public static void InitFileLog(string logConfigFile)
        {
            FileLog.FilePath = ReadLogConfig(logConfigFile, "FilePath");
            if (ReadLogConfig(logConfigFile, "FileMaxSize") != string.Empty)
                FileLog.FileMaxSize = Convert.ToInt64(ReadLogConfig(logConfigFile, "FileMaxSize"));

            if (ReadLogConfig(logConfigFile, "EnabledConsole") != string.Empty)
                FileLog.EnabledConsole = Convert.ToBoolean(ReadLogConfig(logConfigFile, "EnabledConsole"));

            if (ReadLogConfig(logConfigFile, "EnabledDebug") != string.Empty)
                FileLog.EnabledDebug = Convert.ToBoolean(ReadLogConfig(logConfigFile, "EnabledDebug"));

            if (ReadLogConfig(logConfigFile, "EnabledInfo") != string.Empty)
                FileLog.EnabledInfo = Convert.ToBoolean(ReadLogConfig(logConfigFile, "EnabledInfo"));

            if (ReadLogConfig(logConfigFile, "EnabledWarning") != string.Empty)
                FileLog.EnabledWarning = Convert.ToBoolean(ReadLogConfig(logConfigFile, "EnabledWarning"));

            if (ReadLogConfig(logConfigFile, "EnableError") != string.Empty)
                FileLog.EnableError = Convert.ToBoolean(ReadLogConfig(logConfigFile, "EnableError"));

            if (FileLog.FilePath == string.Empty)
                FileLog.FilePath = AppDomain.CurrentDomain.BaseDirectory;

        }

        /// <summary>
        /// 读取配置文件
        /// </summary>
        /// <param name="logConfigFile"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        private static string ReadLogConfig(string logConfigFile, string key)
        {
            if (File.Exists(logConfigFile))
            {
                ExeConfigurationFileMap file = new ExeConfigurationFileMap();
                file.ExeConfigFilename = logConfigFile;
                Configuration config = ConfigurationManager.OpenMappedExeConfiguration(file, ConfigurationUserLevel.None);
                if (config.AppSettings.Settings[key] != null)
                    return config.AppSettings.Settings[key].Value;
                else
                    return string.Empty;
            }
            else
                return string.Empty;
        }
        /// <summary>
        /// 判断相应的级别是否充许输出
        /// </summary>
        /// <param name="logLevel"></param>
        /// <returns></returns>
        private static bool LogLevelEnabled(LogLevel logLevel)
        {

            bool ret = false;
            switch (logLevel)
            {
                case LogLevel.DEBUG:
                    ret = EnabledDebug;
                    break;
                case LogLevel.INFO:
                    ret = EnabledInfo;
                    break;
                case LogLevel.WARNING:
                    ret = EnabledWarning;
                    break;
                case LogLevel.ERROR:
                    ret = EnableError;
                    break;
                default:
                    ret = false;
                    break;
            }
            return ret;
        }

        /// <summary>
        /// 写日记 实例方法
        /// </summary>
        /// <param name="logLevel"></param>
        /// <param name="msg"></param>
        public void Log(LogLevel logLevel, string msg)
        {
            FileLog.WriteLog(logLevel, string.Empty, msg);
        }

        public void Log(LogLevel logLevel, string local, string msg)
        {
            if (local == string.Empty)
                local = this.Local;
            FileLog.WriteLog(logLevel, this.Local, msg);
        }

        public void Debug(string msg)
        {
            Log(LogLevel.DEBUG, msg);
        }

        public void Debug(string local, string msg)
        {
            Log(LogLevel.DEBUG, local, msg);
        }

        public void Info(string msg)
        {
            Log(LogLevel.INFO, msg);
        }

        public void Info(string local, string msg)
        {
            Log(LogLevel.INFO, local, msg);
        }

        public void Warning(string msg)
        {
            Log(LogLevel.WARNING, msg);
        }

        public void Warning(string local, string msg)
        {
            Log(LogLevel.WARNING, local, msg);
        }

        public void Error(string msg)
        {
            Log(LogLevel.ERROR, msg);
        }

        public void Error(string local, string msg)
        {
            Log(LogLevel.ERROR, local, msg);
        }


        /// <summary>
        /// 写日记 静态方法
        /// </summary>
        /// <param name="logLevel"></param>
        /// <param name="local"></param>
        /// <param name="msg"></param>
        public static void WriteLog(LogLevel logLevel, string local, string msg)
        {
            if (!LogLevelEnabled(logLevel)) return;

            StringBuilder sb = new StringBuilder();
            sb.Append(GetLogLevelString(logLevel) + "\t" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\t" + local);
            sb.Append("\t");
            sb.Append(msg);

            if (EnabledConsole)
            {
                Console.WriteLine(sb.ToString());
            }

            try
            {
                WriteTextToFile(sb.ToString());
            }
            catch
            {
                ;
            }


        }

        public static void WriteDebug(string local, string msg)
        {
            WriteLog(LogLevel.DEBUG, local, msg);
        }

        public static void WriteInfo(string local, string msg)
        {
            WriteLog(LogLevel.INFO, local, msg);
        }

        public static void WriteWarning(string local, string msg)
        {
            WriteLog(LogLevel.WARNING, local, msg);
        }

        public static void WriteError(string local, string msg)
        {
            WriteLog(LogLevel.ERROR, local, msg);
        }

        public static void DeleteOldLog()
        {
            if (FileLog.FilePath != string.Empty)
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(FileLog.FilePath);

                foreach (FileInfo fileInfo in directoryInfo.GetFiles())
                {
                    if (fileInfo.Extension.ToLower() == ".log")
                    {
                        TimeSpan timespan = new TimeSpan(FileLog.DateCountSaveLog, 0, 0, 0);
                        if ((DateTime.Now - fileInfo.LastWriteTime) > timespan)
                        {
                            fileInfo.Delete();
                        }
                    }
                }
            }
        }
    }
}
