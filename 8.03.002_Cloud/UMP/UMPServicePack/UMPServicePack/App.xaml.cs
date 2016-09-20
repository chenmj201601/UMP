using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using UMPServicePack.PublicClasses;
using UMPServicePackCommon;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;

namespace UMPServicePack
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        #region 定义全局变量
        // 保存所有服务在升级前的状态
        public static Dictionary<string, ServiceEnty> dicAllServiceStatus = new Dictionary<string, ServiceEnty>();

        //保存当前的语言（与语言资源文件的名字相同，用于在切换语言时 先移除前一个资源文件，程序启动时为1033）
        public string GstrCurrentLanguage = "1033";

        //UMP和Logging涉及到的所有服务的服务名
        public static List<string> lstAllServiceNames;

        //UMP和Logging中所有模块ID
        public static List<string> lstAllModules;

        //当前文件夹路径
        public static string AppPath = AppDomain.CurrentDomain.BaseDirectory;

        //升级xml的名字
        public const string FILE_NAME = "UpdateInfo.xml";

        //升级xml序列化出的类
        public static UpdateInfo updateInfo = null;

        public static bool bIsUMPServer = false;
        public static bool bIsLoggingServer = false;

        //保存已安装的UMP安装包
        public static Dictionary<string, UMPAppInfo> dicAppInstalled = new Dictionary<string, UMPAppInfo>();
        //保存当前的数据库=信息
        public static DatabaseInfo currDBInfo = null;

        public const string strRent = "00000";

        public static string CurrUserName;
        public static long CurrUserID;

        /// <summary>
        /// 保存T_00_000在升级前的数据 以便在升级失败时回滚
        /// </summary>
        public static DataSet dsT000 = null;
        //已安装的版本
        public static string gStrLastVersion = string.Empty;

        public static string gStrCurrLang = "1033";

        //机器ID 由Service00生成的  如果没有 则自己生成 用来判断该机器是否升级 及升级后的版本
        public static string gStrMachineID = string.Empty;

        //解压升级文件的目标路径（用于升级完成后删除）
        public static string gStrUpdateFolderTempPath = string.Empty;

        //在程序退出前启动的服务的列表
        public static List<string> lstServicesToStart = new List<string>();

        //升级是否成功
        public static bool bIsUpgrageSuccess = false;

        #endregion

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            try
            {
                int langID = GetUserDefaultUILanguage();
                gStrCurrLang = langID.ToString();
            }
            catch { }
            LoadUpdaterInfo();
            InitAllServiceNames();
            InitAllModules();
        }

        /// <summary>
        /// 写日志
        /// </summary>
        /// <param name="strLog"></param>
        public static void WriteLog(string strLog)
        {
            string strLogFileName = string.Empty;
            if (updateInfo != null)
            {
                strLogFileName = "Service Pack " + updateInfo.Version + ".log";
            }
            else
            {
                strLogFileName = "Service Pack " + DateTime.Now.Year.ToString() + DateTime.Now.Month + DateTime.Now.Day + ".log";
            }
            FileStream fs = null;
            StreamWriter sw = null;
            try
            {
                if (string.IsNullOrEmpty(strLog))
                {
                    strLog = " null";
                }
                strLog = string.Format("{0}\t{1}", DateTime.Now.ToString(), strLog);
               
                string strDir = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\UMP\\";
                if (!Directory.Exists(strDir))
                {
                    Directory.CreateDirectory(strDir);
                }
                string strFilePath = strDir + "\\" + strLogFileName;
                if (File.Exists(strFilePath))
                {
                    fs = new FileStream(strFilePath, FileMode.Append);
                }
                else
                {
                    fs = new FileStream(strFilePath, FileMode.Create);
                }
                sw = new StreamWriter(fs);
                sw.WriteLine(strLog);
            }
            catch
            {

            }
            finally
            {
                sw.Flush();
                fs.Flush();
                sw.Close();
                fs.Close();
            }
        }

        /// <summary>
        /// 读出升级内容的xml
        /// </summary>
        private void LoadUpdaterInfo()
        {
            var strUpdaterFile = App.GetResourceStream(new Uri(FILE_NAME, UriKind.Relative));
            if (strUpdaterFile == null)
            {
                MessageBox.Show(GetLanguage("string7", "string7"));
                Application.Current.Shutdown();
            }
            else
            {
                StreamReader reader = new StreamReader(strUpdaterFile.Stream);
                string strContent = reader.ReadToEnd();
                OperationReturn optReturn = XMLHelper.DeserializeObject<UpdateInfo>(strContent);
                if (!optReturn.Result)
                {
                    ShowException(GetLanguage("string7", "string7"));
                    WriteLog("LoadUpdaterInfo error : " + optReturn.Message);
                    Application.Current.Shutdown();
                }
                else
                {
                    updateInfo = optReturn.Data as UpdateInfo;
                    WriteLog("LoadUpdaterInfo success .");
                }
                reader.Close();
            }
        }

        public static void ShowException(string msg)
        {
            MessageBox.Show(msg, "UMP Service Pack", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public static void ShowMessage(string msg)
        {
            MessageBox.Show(msg, "UMP Service Pack", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public static void ShowWarnning(string msg)
        {
            MessageBox.Show(msg, "UMP Service Pack", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        /// <summary>
        /// 将所有服务名加入全局变量，以便循环获取和设置状态
        /// </summary>
        private void InitAllServiceNames()
        {
            lstAllServiceNames = new List<string>();

            lstAllServiceNames.Add("UMPCRYPTION");
            lstAllServiceNames.Add("UMPDBbridgeServer");
            lstAllServiceNames.Add("UMPDEC");
            lstAllServiceNames.Add("UMPFS");
            lstAllServiceNames.Add("UMPKEYGEN");
            lstAllServiceNames.Add("UMP Service 00");
            lstAllServiceNames.Add("UMP Service 01");
            lstAllServiceNames.Add("UMP Service 02");
            lstAllServiceNames.Add("UMP Service 03");
            lstAllServiceNames.Add("UMP Service 04");
            lstAllServiceNames.Add("UMP Service 05");
            lstAllServiceNames.Add("UMP Service 06");
            lstAllServiceNames.Add("UMP Service 07");
            lstAllServiceNames.Add("UMP Service 08");
            lstAllServiceNames.Add("UMP Service 09");
            lstAllServiceNames.Add("UMPSFTP");
            lstAllServiceNames.Add("UMPTellServer");
            lstAllServiceNames.Add("UMPVoiceServer");
            lstAllServiceNames.Add("UMPScreen");
            lstAllServiceNames.Add("UMPSpeechAnalysis");
            lstAllServiceNames.Add("UMPAlarmMonitor");
            lstAllServiceNames.Add("UMPAlarmServer");
            lstAllServiceNames.Add("UMPCTIDBB");
            lstAllServiceNames.Add("UMPCTIHUB");
            lstAllServiceNames.Add("UMPCMServer");
            lstAllServiceNames.Add("UMPLicenseServer");
            lstAllServiceNames.Add("UMPSIPServer");
            lstAllServiceNames.Add("UMPScreenCenter");
            lstAllServiceNames.Add("UMPParamServer");
            lstAllServiceNames.Add("UMPSadrsServer");
            lstAllServiceNames.Add("UMPUploadRecord");
        }

        /// <summary>
        /// 初始化所有的模块ID
        /// </summary>
        private void InitAllModules()
        {
            lstAllModules = new List<string>();
            lstAllModules.Add("0");
            lstAllModules.Add("1101");
            lstAllModules.Add("1102");
            lstAllModules.Add("1103");
            lstAllModules.Add("1104");
            lstAllModules.Add("1105");
            lstAllModules.Add("1106");
            lstAllModules.Add("1107");
            lstAllModules.Add("1108");
            lstAllModules.Add("1109");
            lstAllModules.Add("1110");
            lstAllModules.Add("1111");
            lstAllModules.Add("1112");
            lstAllModules.Add("1200");
            lstAllModules.Add("2101");
            lstAllModules.Add("2102");
            lstAllModules.Add("2103");
            lstAllModules.Add("2401");
            lstAllModules.Add("2402");
            lstAllModules.Add("2403");
            lstAllModules.Add("2404");
            lstAllModules.Add("2501");
            lstAllModules.Add("3101");
            lstAllModules.Add("3102");
            lstAllModules.Add("3103");
            lstAllModules.Add("3104");
            lstAllModules.Add("3105");
            lstAllModules.Add("3106");
            lstAllModules.Add("3107");
            lstAllModules.Add("3108");
            lstAllModules.Add("3601");
            lstAllModules.Add("3602");
            lstAllModules.Add("3603");
            lstAllModules.Add("3604");
            lstAllModules.Add("4401");
            lstAllModules.Add("4410");
            lstAllModules.Add("4511");
            lstAllModules.Add("4601");
            lstAllModules.Add("5100");
            lstAllModules.Add("6101");
            lstAllModules.Add("9801");
            lstAllModules.Add("9802");
            
            //logging
            lstAllModules.Add("2120");
            lstAllModules.Add("2122");
            lstAllModules.Add("2129");
            lstAllModules.Add("2130");
            lstAllModules.Add("2131");
            lstAllModules.Add("2132");
            lstAllModules.Add("2133");
            lstAllModules.Add("2134");
            lstAllModules.Add("2135");
            lstAllModules.Add("2136");
            lstAllModules.Add("2137");
            lstAllModules.Add("2138");
            lstAllModules.Add("2139");
            lstAllModules.Add("2140");
            lstAllModules.Add("2141");
            lstAllModules.Add("2142");
            lstAllModules.Add("2511");
            lstAllModules.Add("2512");
            lstAllModules.Add("2513");
            lstAllModules.Add("2514");
            lstAllModules.Add("3011");
            lstAllModules.Add("3012");
            lstAllModules.Add("2099");
            lstAllModules.Add("3211");
            lstAllModules.Add("3212");
            lstAllModules.Add("3213");
            lstAllModules.Add("3214");
            lstAllModules.Add("3299");
            lstAllModules.Add("3311");
            lstAllModules.Add("3312");
            lstAllModules.Add("3411");
            lstAllModules.Add("5111");
        }

        /// <summary>
        /// 获得显示语言
        /// </summary>
        /// <param name="strKey"></param>
        /// <param name="strDefault"></param>
        /// <returns></returns>
        public static string GetLanguage(string strKey, string strDefault)
        {
            string strReturn = string.Empty;
            try
            {
                strReturn = Application.Current.FindResource(strKey).ToString();
            }
            catch (Exception ex)
            {
                strReturn = string.Empty;
                //WriteLog("Get language error: " + strKey);
                WriteLog(string.Format("Get language error.\t{0}", ex.Message));
            }
            if (string.IsNullOrEmpty(strReturn))
            {
                strReturn = strDefault;
            }
            return strReturn;
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            if (bIsUpgrageSuccess)
            {
                List<string> lstServices = App.lstServicesToStart;
                if (lstServices.Count > 0)
                {
                    OperationReturn optReturn = new OperationReturn();
                    optReturn.Result = true;
                    optReturn.Code = Defines.RET_SUCCESS;

                    for (int i = 0; i < lstServices.Count; i++)
                    {
                        optReturn = CommonFuncs.StartServiceByNmae(lstServices[i]);
                        if (!optReturn.Result)
                        {
                            App.WriteLog("Start service " + lstServices[i] + " failed. " + optReturn.Message);
                        }
                        else
                        {
                            App.WriteLog("Start service " + lstServices[i] + " success. ");
                        }
                    }
                }
            }

            //删掉解压出来的文件
            if (Directory.Exists(App.gStrUpdateFolderTempPath))
            {
                try
                {
                    Directory.Delete(gStrUpdateFolderTempPath, true);
                }
                catch (Exception ex)
                {
                    App.WriteLog("Delete " + gStrUpdateFolderTempPath + " failed. " + ex.Message);
                }
            }
        }


        #region DefaultLanguage

        [DllImport("kernel32.dll")]
        private static extern int GetUserDefaultUILanguage();

        #endregion
    }
}
