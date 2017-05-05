using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;

namespace UMP.PF.Client
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
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
        /// 语言包数据
        /// </summary>
        public static DataTable GDataTableLanguage = null;

        /// <summary>
        /// 当前语言ID
        /// </summary>
        public static string GStrLanguageID = string.Empty;

        /// <summary>
        /// 当前系统System路径
        /// </summary>
        public static string GStrSystemRoot = string.Empty;

        /// <summary>
        /// 当前Windows 的版本号
        /// </summary>
        public static string GStrWindowVersion = string.Empty;

        /// <summary>
        /// 当前Windows 的类型，针对版本号 6.1以上版本。数据值为 Server，Client
        /// </summary>
        public static string GStrWindowType = string.Empty;

        /// <summary>
        /// 安装了UMP的服务器IP或名称
        /// </summary>
        public static List<string> GListStrAppServerName = new List<string>();

        /// <summary>
        /// 安装了UMP的服务器端口
        /// </summary>
        public static List<int> GListIntAppServerPort = new List<int>();

        /// <summary>
        /// 当前操作系统是否为 64 位操作系统
        /// </summary>
        public static bool GBoolIs64BitOperatingSystem = true;

        /// <summary>
        /// 当前操作系统的用户 ProgramData 路径
        /// </summary>
        public static string GStrLoginUserApplicationDataPath = string.Empty;

        #region 用户设置的最后连接服务器信息（Host、Port）
        public static string GStrLastSettedUMPServerHost = string.Empty;
        public static int GIntLastSettedUMPServerPort = 8081;
        #endregion

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            #region 当前程序所在路径
            int LIntLastIndex = 0;
            GStrApplicationDirectory = Environment.CurrentDirectory;
            GStrLoginUserApplicationDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            
            if (!System.IO.File.Exists(System.IO.Path.Combine(GStrApplicationDirectory, "UMP.PF.Client.exe")))
            {
                GStrApplicationDirectory = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
                LIntLastIndex = GStrApplicationDirectory.LastIndexOf("\\");
                GStrApplicationDirectory = GStrApplicationDirectory.Substring(0, LIntLastIndex);
            }
            #endregion

            #region 创建分割符
            CreateSpliterCharater();
            #endregion
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

        [DllImport("kernel32.dll")]
        private static extern int GetUserDefaultUILanguage();

        public static void InitializeLanguagePackage()
        {
            string LStrOpenedSetting = string.Empty;
            string LStrXmlFileName = string.Empty;
            string LStrFilter = string.Empty;

            LStrOpenedSetting = System.IO.Path.Combine(GStrLoginUserApplicationDataPath, @"UMP.Client\UMP.Setted.xml");
            if (!System.IO.File.Exists(LStrOpenedSetting))
            {
                GStrLanguageID = GetUserDefaultUILanguage().ToString();
                WriteUserSettings("LanguageSetted", "LanguageID", GStrLanguageID);
            }
            
            GStrLanguageID = GetUserLastSelectedLanguageID(LStrOpenedSetting);
            if (string.IsNullOrEmpty(GStrLanguageID)) { GStrLanguageID = "2052"; }
            
            LStrXmlFileName = System.IO.Path.Combine(GStrApplicationDirectory, @"Languages\11-01-" + GStrLanguageID + ".xml");
            if (!System.IO.File.Exists(LStrXmlFileName)) { LStrXmlFileName = GStrApplicationDirectory + @"\Languages\11-01-2052.xml"; }

            GDataTableLanguage = new DataTable();
            GDataTableLanguage.Columns.Add("ObjectID", typeof(string));
            GDataTableLanguage.Columns.Add("ObjectData", typeof(string));
            GDataTableLanguage.Columns.Add("ObjectDescribe", typeof(string));

            DataSet LocalDataSet = new DataSet();
            LocalDataSet.ReadXml(LStrXmlFileName);
            GDataTableLanguage = LocalDataSet.Tables[0];
        }

        public static bool InitializeLanguagePackage(string AStrLanguageID)
        {
            bool LBoolReturn = true;
            string LStrXmlFileName = string.Empty;
            string LStrFilter = string.Empty;
            DataTable LDataTableNewLanguage = new DataTable();
            try
            {
                LStrXmlFileName = System.IO.Path.Combine(App.GStrApplicationDirectory, @"Languages\11-01-" + AStrLanguageID + ".xml");
                if (!System.IO.File.Exists(LStrXmlFileName)) { return false; }

                LDataTableNewLanguage = new DataTable();
                LDataTableNewLanguage.Columns.Add("ObjectID", typeof(string));
                LDataTableNewLanguage.Columns.Add("ObjectData", typeof(string));
                LDataTableNewLanguage.Columns.Add("ObjectDescribe", typeof(string));

                DataSet LocalDataSet = new DataSet();
                LocalDataSet.ReadXml(LStrXmlFileName);
                LDataTableNewLanguage = LocalDataSet.Tables[0];
                if (LDataTableNewLanguage.Rows.Count > 0)
                {
                    GDataTableLanguage = LDataTableNewLanguage;
                    GStrLanguageID = AStrLanguageID;
                }
                else
                {
                    LBoolReturn = false;
                }
            }
            catch
            {
                LBoolReturn = false;
            }

            return LBoolReturn;
        }

        private static string GetUserLastSelectedLanguageID(string AStrOpenedSetting)
        {
            string LStrLanguageID = string.Empty;

            try
            {
                XmlDocument LXmlDocSetting = new XmlDocument();
                LXmlDocSetting.Load(AStrOpenedSetting);
                XmlNode LXMLNodeUserSetted = LXmlDocSetting.SelectSingleNode("UserSetted");
                XmlNode LXMLNodeLanguageSetted = LXMLNodeUserSetted.SelectSingleNode("LanguageSetted");
                LStrLanguageID = LXMLNodeLanguageSetted.Attributes["LanguageID"].Value;
                XmlNode LXMLNodeUMPServerSetted = LXMLNodeUserSetted.SelectSingleNode("UMPServerSetted");
                GStrLastSettedUMPServerHost = LXMLNodeUMPServerSetted.Attributes["UMPServerHost"].Value;
                XmlNodeList LXmlNodeListChild = LXMLNodeUMPServerSetted.ChildNodes;
                foreach (XmlNode LXmlNodeSingle in LXmlNodeListChild)
                {
                    if (LXmlNodeSingle.Attributes["Protocol"].Value != "http") { continue; }
                    GIntLastSettedUMPServerPort = int.Parse(LXmlNodeSingle.Attributes["BindInfo"].Value);
                }
            }
            catch
            {
                LStrLanguageID = GetUserDefaultUILanguage().ToString();
                GStrLastSettedUMPServerHost = string.Empty;
                GIntLastSettedUMPServerPort = 8081;
            }
            return LStrLanguageID;
        }

        public static string WriteUserSettings(string AStrSection, string AStrAttribute, string AStrValue)
        {
            string LStrReturn = string.Empty;
            string LStrDirectory = string.Empty;
            string LStrXmlFileName = string.Empty;

            try
            {
                LStrReturn = string.Empty;
                LStrDirectory = System.IO.Path.Combine(GStrLoginUserApplicationDataPath, "UMP.Client");
                if (!System.IO.Directory.Exists(LStrDirectory)) { System.IO.Directory.CreateDirectory(LStrDirectory); }
                LStrXmlFileName = System.IO.Path.Combine(GStrLoginUserApplicationDataPath, @"UMP.Client\UMP.Setted.xml");
                if (!System.IO.File.Exists(LStrXmlFileName))
                {
                    LStrReturn = CreateSettingXMLFile(LStrXmlFileName);
                    if (!string.IsNullOrEmpty(LStrReturn)) { return LStrReturn; }
                }
                XmlDocument LXmlDocUserSetting = new XmlDocument();
                LXmlDocUserSetting.Load(LStrXmlFileName);
                XmlNode LXMLNodeSection = LXmlDocUserSetting.SelectSingleNode("UserSetted").SelectSingleNode(AStrSection);
                LXMLNodeSection.Attributes[AStrAttribute].Value = AStrValue;
                LXmlDocUserSetting.Save(LStrXmlFileName);
            }
            catch (Exception ex)
            {
                LStrReturn = ex.Message;
            }
            return LStrReturn;
        }

        private static string CreateSettingXMLFile(string AStrXMLFile)
        {
            string LStrReturn = string.Empty;

            try
            {
                XmlDocument LXmlDocUserSetting = new XmlDocument();
                LXmlDocUserSetting.AppendChild(LXmlDocUserSetting.CreateXmlDeclaration("1.0", "UTF-8", "yes"));

                XmlElement LXmlElementRoot = LXmlDocUserSetting.CreateElement("UserSetted");
                LXmlDocUserSetting.AppendChild(LXmlElementRoot);

                XmlElement LXmlElementLanguageSetted = LXmlDocUserSetting.CreateElement("LanguageSetted");
                LXmlElementLanguageSetted.SetAttribute("LanguageID", "");
                LXmlElementRoot.AppendChild(LXmlElementLanguageSetted);

                XmlElement LXmlElementStyleSetted = LXmlDocUserSetting.CreateElement("StyleSetted");
                LXmlElementStyleSetted.SetAttribute("StyleID", "");
                LXmlElementRoot.AppendChild(LXmlElementStyleSetted);

                XmlElement LXmlElementSaveLastUserSetted = LXmlDocUserSetting.CreateElement("SaveLastUser");
                LXmlElementSaveLastUserSetted.SetAttribute("IsSaved", "0");
                LXmlElementSaveLastUserSetted.SetAttribute("UserAccount", "");
                LXmlElementSaveLastUserSetted.SetAttribute("LoginPassword", "");
                LXmlElementSaveLastUserSetted.SetAttribute("IsLDAPAccount", "");
                LXmlElementRoot.AppendChild(LXmlElementSaveLastUserSetted);

                XmlElement LXmlElementUMPServerSetted = LXmlDocUserSetting.CreateElement("UMPServerSetted");
                LXmlElementUMPServerSetted.SetAttribute("UMPServerHost", "");
                XmlElement LXmlElementProtocolBindHttp = LXmlDocUserSetting.CreateElement("ProtocolBind");
                LXmlElementProtocolBindHttp.SetAttribute("Activated", "1");
                LXmlElementProtocolBindHttp.SetAttribute("Protocol", "http");
                LXmlElementProtocolBindHttp.SetAttribute("BindInfo", "8081");
                LXmlElementProtocolBindHttp.SetAttribute("OtherArgs", "");
                LXmlElementProtocolBindHttp.SetAttribute("Used", "1");
                LXmlElementUMPServerSetted.AppendChild(LXmlElementProtocolBindHttp);

                XmlElement LXmlElementProtocolBindHttps = LXmlDocUserSetting.CreateElement("ProtocolBind");
                LXmlElementProtocolBindHttps.SetAttribute("Activated", "0");
                LXmlElementProtocolBindHttps.SetAttribute("Protocol", "https");
                LXmlElementProtocolBindHttps.SetAttribute("BindInfo", "8082");
                LXmlElementProtocolBindHttps.SetAttribute("OtherArgs", "");
                LXmlElementProtocolBindHttps.SetAttribute("Used", "0");
                LXmlElementUMPServerSetted.AppendChild(LXmlElementProtocolBindHttps);

                XmlElement LXmlElementProtocolBindNetTcp = LXmlDocUserSetting.CreateElement("ProtocolBind");
                LXmlElementProtocolBindNetTcp.SetAttribute("Activated", "0");
                LXmlElementProtocolBindNetTcp.SetAttribute("Protocol", "net.tcp");
                LXmlElementProtocolBindNetTcp.SetAttribute("BindInfo", "8083");
                LXmlElementProtocolBindNetTcp.SetAttribute("OtherArgs", "");
                LXmlElementProtocolBindNetTcp.SetAttribute("Used", "0");
                LXmlElementUMPServerSetted.AppendChild(LXmlElementProtocolBindNetTcp);

                LXmlElementRoot.AppendChild(LXmlElementUMPServerSetted);


                LXmlDocUserSetting.Save(AStrXMLFile);
            }
            catch (Exception ex)
            {
                LStrReturn = ex.Message;
            }
            return LStrReturn;
        }

        /// <summary>
        /// 获取显示语言
        /// </summary>
        /// <param name="AStrObjectID"></param>
        /// <param name="AStrObjectData"></param>
        /// <returns></returns>
        public static string GetDisplayCharater(string AStrObjectID, string AStrObjectData)
        {
            string LStrReturn = string.Empty;

            DataRow[] ObjectLanguageRow = GDataTableLanguage.Select("ObjectID = '" + AStrObjectID + "' AND ObjectData = '" + AStrObjectData + "'");
            if (ObjectLanguageRow.Count() == 0) { return LStrReturn; }
            LStrReturn = ObjectLanguageRow.FirstOrDefault().Field<string>(2);
            return LStrReturn;
        }
    }
}
