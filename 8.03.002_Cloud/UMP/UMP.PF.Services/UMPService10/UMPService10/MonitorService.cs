using System;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;

namespace UMPService10
{
    partial class MonitorService : ServiceBase
    {

        #region Members

        private string mAppName = "UMPService10";
        private LogOperator mLogOperator;
        private MonitorServer mMonitorServer;
        private ConfigInfo mConfigInfo;

        #endregion

        public MonitorService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            // TODO:  在此处添加代码以启动服务。
            try
            {
                CreateFileLog();
                WriteLog(LogMode.Info, string.Format("Service starting..."));
                LoadConfigInfo();
                SetLogMode();
                CreateMonitorServer();
            }
            catch (Exception ex)
            {
                WriteLog(LogMode.Error, string.Format("Service start fail.\t{0}", ex.Message));
            }
        }

        protected override void OnStop()
        {
            // TODO:  在此处添加代码以执行停止服务所需的关闭操作。
            if (mMonitorServer != null)
            {
                mMonitorServer.Stop();
            }
            WriteLog(LogMode.Info, string.Format("Service stopped"));
            if (mLogOperator != null)
            {
                mLogOperator.Stop();
                mLogOperator = null;
            }
        }


        #region MonitorServer

        private void CreateMonitorServer()
        {
            try
            {
                mMonitorServer = new MonitorServer();
                mMonitorServer.Debug += MonitorServer_Debug;
                mMonitorServer.Start();
            }
            catch (Exception ex)
            {
                WriteLog(LogMode.Error, string.Format("CreateMonitorServer fail.\t{0}", ex.Message));
            }
        }

        void MonitorServer_Debug(LogMode mode, string category, string msg)
        {
            WriteLog(mode, category, msg);
        }

        #endregion


        #region LogOperator

        private void CreateFileLog()
        {
            try
            {
                string path = GetLogPath();
                mLogOperator = new LogOperator();
                mLogOperator.LogPath = path;
                if (Program.IsDebug)
                {
                    //调试模式下记录所有日志信息
                    mLogOperator.LogMode = LogMode.All;
                }
                mLogOperator.Start();
                string strInfo = string.Empty;
                strInfo += string.Format("LogPath:{0}\r\n", path);
                strInfo += string.Format("\tExePath:{0}\r\n", AppDomain.CurrentDomain.BaseDirectory);
                strInfo += string.Format("\tName:{0}\r\n", AppDomain.CurrentDomain.FriendlyName);
                strInfo += string.Format("\tVersion:{0}\r\n",
                    System.Reflection.Assembly.GetExecutingAssembly().GetName().Version);
                strInfo += string.Format("\tHost:{0}\r\n", Environment.MachineName);
                strInfo += string.Format("\tAccount:{0}", Environment.UserName);
                WriteLog(LogMode.Info, string.Format("{0}", strInfo));
            }
            catch (Exception ex)
            {
                WriteLog(LogMode.Error, string.Format("CreateFileLog fail.\t{0}", ex.Message));
            }
        }

        private void WriteLog(LogMode mode, string category, string msg)
        {
            if (mLogOperator != null)
            {
                mLogOperator.WriteLog(mode, category, msg);
            }
        }

        private void WriteLog(string category, string msg)
        {
            WriteLog(LogMode.Info, category, msg);
        }

        private void WriteLog(LogMode mode, string msg)
        {
            WriteLog(mode, "MonitorService", msg);
        }

        private void WriteLog(string msg)
        {
            WriteLog(LogMode.Info, msg);
        }

        private string GetLogPath()
        {
            string strReturn = string.Empty;
            try
            {
                //从LocalMachine文件中读取日志路径
                string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                    @"VoiceCyber\UMP\config\localmachine.ini");
                if (File.Exists(path))
                {
                    string[] arrInfos = File.ReadAllLines(path, Encoding.Default);
                    for (int i = 0; i < arrInfos.Length; i++)
                    {
                        string strInfo = arrInfos[i];
                        if (strInfo.StartsWith("LogPath="))
                        {
                            string str = strInfo.Substring(8);
                            if (!string.IsNullOrEmpty(str))
                            {
                                strReturn = str;
                                break;
                            }
                        }
                    }
                }
                if (string.IsNullOrEmpty(strReturn)
                    || !Directory.Exists(strReturn))
                {
                    //如果读取失败，或者目录不存在，使用默认目录
                    strReturn = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                        string.Format("UMP\\{0}\\Logs", mAppName));
                }
                else
                {
                    strReturn = Path.Combine(strReturn, mAppName);
                }
                //创建日志文件夹
                if (!Directory.Exists(strReturn))
                {
                    Directory.CreateDirectory(strReturn);
                }
            }
            catch { }
            return strReturn;
        }

        private void SetLogMode()
        {
            try
            {
                if (mConfigInfo == null) { return; }
                var setting = mConfigInfo.ListSettings.FirstOrDefault(s => s.Key == ConstValue.GS_KEY_LOG_MODE);
                if (setting == null) { return; }
                string strValue = setting.Value;
                int intValue;
                if (int.TryParse(strValue, out intValue)
                    && intValue > 0)
                {
                    if (mLogOperator != null)
                    {
                        mLogOperator.LogMode = (LogMode)intValue;
                        WriteLog(LogMode.Info, string.Format("LogMode changed.\t{0}", (LogMode)intValue));
                    }
                }
            }
            catch (Exception ex)
            {
                WriteLog(LogMode.Error, string.Format("SetLogMode fail.\t{0}", ex.Message));
            }
        }

        #endregion


        #region ConfigInfo

        private void LoadConfigInfo()
        {
            try
            {
                string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                    ConstValue.TEMP_DIR_UMP);
                path = Path.Combine(path, mAppName);
                path = Path.Combine(path, ConstValue.TEMP_FILE_CONFIGINFO);
                if (!File.Exists(path))
                {
                    WriteLog(LogMode.Error, string.Format("ConfigInfo file not exist.\t{0}", path));
                    return;
                }
                OperationReturn optReturn = XMLHelper.DeserializeFile<ConfigInfo>(path);
                if (!optReturn.Result)
                {
                    WriteLog(LogMode.Error,
                        string.Format("LoadConfigInfo fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                ConfigInfo configInfo = optReturn.Data as ConfigInfo;
                if (configInfo == null)
                {
                    WriteLog(LogMode.Error, string.Format("LoadConfigInfo fail.\tConfigInfo is null"));
                    return;
                }
                mConfigInfo = configInfo;
                WriteLog(LogMode.Info, string.Format("LoadConfigInfo end.\t{0}", path));
            }
            catch (Exception ex)
            {
                WriteLog(LogMode.Error, string.Format("LoadConfigInfo fail.\t{0}", ex.Message));
            }
        }

        #endregion

    }
}
