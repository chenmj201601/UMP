using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using PFShareClassesC;
using UMPS3104.Wcf11012;
using UMPS3104.Wcf11901;
using UMPS3104.Wcf31041;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common31041;
using VoiceCyber.UMP.Communications;

namespace UMPS3104
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public static SessionInfo Session;
        //当前xbap全部语言包
        public static List<LanguageInfo> ListLanguageInfos;


        //当前UMPClientInit.xml文件里支持语言包
        public static List<LocalXmlLanguage> ListSupportLanguage;

        //当前UMPClientInit.xml文件里存储的语言
        public static List<LocalXmlLanguage> ListXmlLanguage;

        //当前UMPClientInit.xml文件里支持样式
        public static List<LocalXmlTheme> ListSupportTheme;


        //当前xbap全部操作权限
        public static List<OperationInfo> ListOperationInfos;

        //当前xbap当前人管理全部部门
        public static List<CtrolOrg> ListCtrolOrgInfos;

        //当前xbap当前人管理的全部座席
        public static List<CtrolAgent> ListCtrolAgentInfos;


        public static string AppName = "UMPS3104";

        private static LogOperator mLogOperator;
        //当前用户的机构Id
        public static string CurrentOrg = "-1";
        //用户租户编码（5位）
        public static string renterID = string.Empty;

        //当前用户类型
        public static int UserType = 1;

        //获取当前用户的密码过期时间
        public static double PwdState = 999.00;//设定(999)不需修改

        //当前 UMPClientInit.xml文件的存储路径
        public static string XmlPathString = string.Empty;

        //储存查询条件
        public static List<QueryConditionDetail> ListQueryConditionDetails;
        public static List<CtrolAgent> ListCtrolAgent;
        public static List<DateTimeSpliteAsDay> ListDateTime;

        /// <summary>
        /// 是否分表
        /// </summary>
        public static bool isCutMonth = false;

        /// <summary>
        /// 是否使用Http
        /// </summary>
        public static bool defaultHttp =false;


        protected override void OnStartup(StartupEventArgs e)
        {
            CreateLogOperator();
            ListSupportLanguage = new List<LocalXmlLanguage>();
            ListXmlLanguage = new List<LocalXmlLanguage>();
            ListSupportTheme = new List<LocalXmlTheme>();
            ListLanguageInfos = new List<LanguageInfo>();
            ListOperationInfos = new List<OperationInfo>();
            ListCtrolAgentInfos = new List<CtrolAgent>();
            ListCtrolOrgInfos = new List<CtrolOrg>();
            ListQueryConditionDetails = new List<QueryConditionDetail>();
            ListCtrolAgent = new List<CtrolAgent>();
            ListDateTime = new List<DateTimeSpliteAsDay>();
            Init();
            base.OnStartup(e);
        }

        /// <summary>
        /// 判断配置文件是否存在
        /// </summary>
        /// <returns></returns>
        public static bool IsExsitXml()
        {
            if (File.Exists(XmlPathString))
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        public static void Init()
        {
            string path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"UMP.Client");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            Session = new SessionInfo();
            Session.SessionID = Guid.NewGuid().ToString();
            Session.AppName = AppName;
            Session.LastActiveTime = DateTime.Now;
            Session.UserID = S3104Consts.USER_ADMIN;


            //PartitionTableInfo partInfo = new PartitionTableInfo();
            //partInfo.TableName = ConstValue.TABLE_NAME_RECORD;
            //partInfo.PartType = TablePartType.DatetimeRange;
            //partInfo.Other1 = ConstValue.TABLE_FIELD_NAME_RECORD_STARTRECORDTIME;
            //Session.ListPartitionTables.Add(partInfo);
            //partInfo = new PartitionTableInfo();
            //partInfo.TableName = ConstValue.TABLE_NAME_OPTLOG;
            //partInfo.PartType = TablePartType.DatetimeRange;
            //partInfo.Other1 = ConstValue.TABLE_FIELD_NAME_OPTLOG_OPERATETIME;
            //Session.ListPartitionTables.Add(partInfo);


            RentInfo rentInfo = new RentInfo();
            rentInfo.ID = S3104Consts.RENT_DEFAULT;
            rentInfo.Token = S3104Consts.RENT_DEFAULT_TOKEN;
            rentInfo.Domain = "voicecyber.com";
            rentInfo.Name = "voicecyber";
            Session.RentInfo = rentInfo;

            UserInfo userInfo = new UserInfo();
            userInfo.UserID = S3104Consts.USER_ADMIN;
            userInfo.Account = "administrator";
            userInfo.UserName = "Administrator";
            Session.UserInfo = userInfo;

            RoleInfo roleInfo = new RoleInfo();
            roleInfo.ID = S3104Consts.ROLE_SYSTEMADMIN;
            roleInfo.Name = "System Admin";
            Session.RoleInfo = roleInfo;

            AppServerInfo serverInfo = new AppServerInfo();
            serverInfo.Protocol = "https";
            serverInfo.Address = "192.168.6.27";
            serverInfo.Port = 8082;
            serverInfo.SupportHttps = true;
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
            langType.LangName = "en-US";
            langType.Display = "English(U.S.)";
            Session.LangTypeInfo = langType;
            Session.LangTypeID = 1033;
            Session.SupportLangTypes.Add(langType);

            langType = new LangTypeInfo();
            langType.LangID = 2052;
            langType.LangName = "zh-CN";
            langType.Display = "简体中文";
            Session.SupportLangTypes.Add(langType);

            langType = new LangTypeInfo();
            langType.LangID = 1028;
            langType.LangName = "tc-CN";
            langType.Display = "繁體中文";
            Session.SupportLangTypes.Add(langType);

            langType = new LangTypeInfo();
            langType.LangID = 1041;
            langType.LangName = "jp-JP";
            langType.Display = "日本语";
            Session.SupportLangTypes.Add(langType);

            DatabaseInfo dbInfo = new DatabaseInfo();
            dbInfo.TypeID = 3;
            dbInfo.TypeName = "ORCL";
            dbInfo.Host = "192.168.4.182";
            dbInfo.Port = 1521;
            dbInfo.DBName = "PFOrcl";
            dbInfo.LoginName = "PFDEV_TEST";
            dbInfo.Password = "pfdev_test";
            Session.DBType = dbInfo.TypeID;
            Session.DBConnectionString = dbInfo.GetConnectionString();
            Session.DatabaseInfo = dbInfo;

            //DatabaseInfo dbInfo = new DatabaseInfo();
            //dbInfo.TypeID = 2;
            //dbInfo.TypeName = "MSSQL";
            //dbInfo.Host = "192.168.4.182";
            //dbInfo.Port = 1433;
            //dbInfo.DBName = "UMPDataDB0826";
            //dbInfo.LoginName = "PFDEV";
            //dbInfo.Password = "PF,123";
            //Session.DBConnectionString = dbInfo.GetConnectionString();
            //Session.DatabaseInfo = dbInfo;


            Session.InstallPath = @"C:\UMPRelease";
            WriteLog("AppInit", string.Format("SessionInfo inited."));
        }

        public static void InitControledOrg(string OrgID)
        {
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Session = App.Session;
                webRequest.Code = (int)S3104Codes.GetControlOrgInfoList;
                webRequest.ListData.Add(OrgID);
                //Service31041Client client = new Service31041Client();
                Service31041Client client = new Service31041Client(WebHelper.CreateBasicHttpBinding(Session), WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service31041"));
                WebReturn webReturn = client.UMPClientOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    App.ShowExceptionMessage(string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message));
                    return;
                }
                if (webReturn.ListData == null)
                {
                    App.ShowExceptionMessage(string.Format("Fail.\tListData is null"));
                    return;
                }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    string strInfo = webReturn.ListData[i];
                    string[] arrInfo = strInfo.Split(new[] { ConstValue.SPLITER_CHAR },
                        StringSplitOptions.RemoveEmptyEntries);
                    if (arrInfo.Length < 2) { continue; }
                    CtrolOrg ctrolOrg = new CtrolOrg();
                    ctrolOrg.ID = arrInfo[0];
                    ctrolOrg.OrgName = arrInfo[1];

                    ListCtrolOrgInfos.Add(ctrolOrg);

                }
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        public static void InitLanguageInfos()
        {
            try
            {
                if (Session == null || Session.LangTypeInfo == null)
                {
                    return;
                }
                ListLanguageInfos.Clear();
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)RequestCode.WSGetLangList;
                webRequest.Session = Session;
                //ListParams
                //0     LangID
                //1     PreName（语言内容编码的前缀，比如 FO:模块、操作显示语言）
                //1     ModuleID
                //2     SubModuleID
                //3     Page
                //4     Name
                webRequest.ListData.Add(Session.LangTypeInfo.LangID.ToString());
                webRequest.ListData.Add(string.Empty);
                webRequest.ListData.Add("31");
                webRequest.ListData.Add("3104");
                webRequest.ListData.Add(string.Empty);
                webRequest.ListData.Add(string.Empty);
                Service11012Client client = new Service11012Client(WebHelper.CreateBasicHttpBinding(Session), WebHelper.CreateEndpointAddress(Session.AppServerInfo, "Service11012"));
                //Service11012Client client = new Service11012Client();
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    ShowExceptionMessage(string.Format("{0}\t{1}", webReturn.Code, webReturn.Message));
                }
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    OperationReturn optReturn = XMLHelper.DeserializeObject<LanguageInfo>(webReturn.ListData[i]);
                    if (!optReturn.Result)
                    {
                        ShowExceptionMessage(string.Format("{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    LanguageInfo langInfo = optReturn.Data as LanguageInfo;
                    if (langInfo == null)
                    {
                        ShowExceptionMessage(string.Format("LanguageInfo is null"));
                        return;
                    }
                    ListLanguageInfos.Add(langInfo);
                }

            }
            catch (Exception ex)
            {
                //ShowExceptionMessage(ex.Message);
            }
        }

        public static string GetLanguageInfo(string name, string display)
        {
            LanguageInfo lang =
                ListLanguageInfos.FirstOrDefault(l => l.LangID == Session.LangTypeInfo.LangID && l.Name == name);
            if (lang == null)
            {
                return display;
            }
            return lang.Display;
        }


        #region OperationLog

        public static void WriteOperationLog(string optID, string result, string contentID, List<string> listParams)
        {
            try
            {
                //Result
                //R0        操作失败
                //R1        操作成功
                //R2        失败（异常）
                //R3        关闭（取消）
                //R4        其他
                string strParams = string.Empty;
                for (int i = 0; i < listParams.Count; i++)
                {
                    strParams += string.Format("{0}{1}{1}{1}", listParams[i], ConstValue.SPLITER_CHAR_2);
                }
                strParams = strParams.Substring(0, strParams.Length - 3);
                List<string> listLogInfos = new List<string>();
                listLogInfos.Add("0");
                listLogInfos.Add("11000");
                listLogInfos.Add(optID);
                listLogInfos.Add(Session.RentInfo.Token);
                listLogInfos.Add(Session.UserID.ToString());
                listLogInfos.Add(Session.RoleID.ToString());
                listLogInfos.Add(Session.LocalMachineInfo.StrHostName);
                listLogInfos.Add("0.0.0.0");
                listLogInfos.Add(DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss"));
                listLogInfos.Add(result);
                listLogInfos.Add(contentID);
                listLogInfos.Add(strParams);
                listLogInfos.Add(string.Empty);
                Service11901Client client = new Service11901Client(WebHelper.CreateBasicHttpBinding(Session),
                        WebHelper.CreateEndpointAddress(Session.AppServerInfo, "Service11901"));
                VoiceCyber.UMP.Controls.Wcf11901.OperationDataArgs args = client.OperationMethodA(1, listLogInfos);
                client.Close();
            }
            catch (Exception ex)
            {
                WriteLog("OperationLog", string.Format("Write operation log fail.\t{0}", ex.Message));
            }
        }

        public static void WriteOperationLog(string optID, string result, string msg)
        {
            try
            {
                //Result
                //R0        操作失败
                //R1        操作成功
                //R2        失败（异常）
                //R3        关闭（取消）
                //R4        其他
                List<string> listLogInfos = new List<string>();
                listLogInfos.Add("0");
                listLogInfos.Add("11000");
                listLogInfos.Add(optID);
                listLogInfos.Add(Session.RentInfo.Token);
                listLogInfos.Add(Session.UserID.ToString());
                listLogInfos.Add(Session.RoleID.ToString());
                listLogInfos.Add(Session.LocalMachineInfo.StrHostName);
                listLogInfos.Add("0.0.0.0");
                listLogInfos.Add(DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss"));
                listLogInfos.Add(result);
                listLogInfos.Add("3104Log0000");
                listLogInfos.Add(msg);
                listLogInfos.Add(string.Empty);
                Service11901Client client = new Service11901Client(WebHelper.CreateBasicHttpBinding(Session),
                        WebHelper.CreateEndpointAddress(Session.AppServerInfo, "Service11901"));
                VoiceCyber.UMP.Controls.Wcf11901.OperationDataArgs args = client.OperationMethodA(1, listLogInfos);
                client.Close();
            }
            catch (Exception ex)
            {
                WriteLog("OperationLog", string.Format("Write operation log fail.\t{0}", ex.Message));
            }
        }

        #endregion

        #region CreateLocalLog
        private void CreateLogOperator()
        {
            try
            {
                string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                   string.Format("UMP/{0}/Logs", AppName));
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                mLogOperator = new LogOperator();
                mLogOperator.LogPath = path;
                mLogOperator.Start();
            }
            catch { }
        }

        public static void WriteLog(string category, string msg)
        {
            if (mLogOperator != null)
            {
                mLogOperator.WriteLog(LogMode.Info, category, msg);
            }
        }

        public static void WriteLog(string msg)
        {
            if (mLogOperator != null)
            {
                mLogOperator.WriteLog(LogMode.Info, AppName, msg);
            }
        }

        public static void ShowExceptionMessage(string msg)
        {
            MessageBox.Show(msg, AppName, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public static void ShowInfoMessage(string msg)
        {
            MessageBox.Show(msg, AppName, MessageBoxButton.OK, MessageBoxImage.Information);
        }


        #endregion

        #region Encryption and Decryption

        public static string EncryptString(string strSource)
        {
            string strTemp = EncryptionAndDecryption.EncryptDecryptString(strSource,
             CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004),
             EncryptionAndDecryption.UMPKeyAndIVType.M004);
            return strTemp;
        }

        public static string DecryptString(string strSource)
        {
            string strTemp = EncryptionAndDecryption.EncryptDecryptString(strSource,
              CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104),
              EncryptionAndDecryption.UMPKeyAndIVType.M104);
            return strTemp;
        }

        public static string CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType aKeyIVID)
        {
            string lStrReturn;
            int LIntRand;
            Random lRandom = new Random();
            string LStrTemp;

            try
            {
                lStrReturn = DateTime.Now.ToString("yyyyMMddHHmmss");
                LIntRand = lRandom.Next(0, 14);
                LStrTemp = LIntRand.ToString("00");
                lStrReturn = lStrReturn.Insert(LIntRand, "VCT");
                LIntRand = lRandom.Next(0, 17);
                LStrTemp += LIntRand.ToString("00");
                lStrReturn = lStrReturn.Insert(LIntRand, "UMP");
                LIntRand = lRandom.Next(0, 20);
                LStrTemp += LIntRand.ToString("00");
                lStrReturn = lStrReturn.Insert(LIntRand, ((int)aKeyIVID).ToString("000"));

                lStrReturn = EncryptionAndDecryption.EncryptStringY(LStrTemp + lStrReturn);
            }
            catch { lStrReturn = string.Empty; }

            return lStrReturn;
        }

        #endregion

        public static Int64 Int64Parse(string str, Int64 defaultValue)
        {
            Int64 outRet = defaultValue;
            if (!Int64.TryParse(str, out outRet))
            {
                outRet = defaultValue;
            }

            return outRet;
        }

        public static int IntParse(string str, int defaultValue)
        {
            int outRet = defaultValue;
            if (!int.TryParse(str, out outRet))
            {
                outRet = defaultValue;
            }

            return outRet;
        }
    }
}
