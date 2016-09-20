using PFShareClassesC;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceModel;
using System.Windows;
using System.Windows.Navigation;
using System.Xml;
using UMPS0000.WCFService00000;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;

namespace UMPS0000
{
    public partial class App : Application
    {
        [DllImport("kernel32.dll")]
        private static extern int GetUserDefaultUILanguage();

        #region 所有信息全部保存在 SessionInfo 中
        public static SessionInfo GClassSessionInfo = null;
        #endregion

        #region 用户默认打开功能
        public static bool GBoolExistDefaultFeature = false;
        public static DataRow GDataRowDefaultFeature = null;
        public static string GStrImageDefaultFeature = string.Empty;
        #endregion

        #region 程序间相互通讯对象、属性定义
        public static NetPipeHelper GNetPipeHelper = null;
        public static ServiceHost GServiceHostCommunication = null;
        #endregion

        #region 本模块使用的语言包
        private static DataTable IDataTableLanguage = null;
        #endregion

        #region MAMT使用的语言包等等
        public static DataTable IDataTableMAMTSupportL = null;
        private static DataTable IDataTableMAMTSupportS = null;
        private static DataTable IDataTableMAMTLanguages = null;
        #endregion

        #region 系统默认分割符
        public static string GStrSpliterChar = string.Empty;
        #endregion

        #region 当前操作系统的登录用户 和 域名
        public static string GStrLoginComputer = string.Empty;
        public static string GStrLoginWorkgroup = string.Empty;
        public static bool GBoolCanAutoLogin = false;
        public static bool GBoolIsLDAPLogin = false;
        public static string GStrAutoLoginRentCode5 = string.Empty;
        #endregion

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            string LStrUserStyle = string.Empty;

            GStrSpliterChar = AscCodeToChr(27);
            GClassSessionInfo = new SessionInfo();
            GClassSessionInfo.RoleInfo.ID = -1;
            LoadCurrentWindowUserSettedInformation();
            GStrLoginComputer = Environment.UserName.ToLower();
            GStrLoginWorkgroup = Environment.UserDomainName.ToLower();
            //MessageBox.Show("GStrLoginComputer = " + GStrLoginComputer + "\n" + "GStrLoginWorkgroup = " + GStrLoginWorkgroup);
            if (string.IsNullOrEmpty(GClassSessionInfo.ThemeInfo.Name))
            {
                DateTime LDateTimeNow = DateTime.Now;

                if (LDateTimeNow.Month == 2 || LDateTimeNow.Month == 3 || LDateTimeNow.Month == 4) { LStrUserStyle = "Style01"; }
                if (LDateTimeNow.Month == 5 || LDateTimeNow.Month == 6 || LDateTimeNow.Month == 7) { LStrUserStyle = "Style02"; }
                if (LDateTimeNow.Month == 8 || LDateTimeNow.Month == 9 || LDateTimeNow.Month == 10) { LStrUserStyle = "Style03"; }
                if (LDateTimeNow.Month == 11 || LDateTimeNow.Month == 12 || LDateTimeNow.Month == 1) { LStrUserStyle = "Style04"; }
                GClassSessionInfo.ThemeInfo.Name = LStrUserStyle;
            }

            if (GClassSessionInfo.LangTypeInfo.LangID == 0)
            {
                GClassSessionInfo.LangTypeInfo.LangID = GetUserDefaultUILanguage();
            }

            LoadApplicationResources();
        }

        /// <summary>
        /// 加载基本信息(最后使用的LanguageID， StyleID，最后登录的用户，Application Server Information)
        /// </summary>
        private void LoadCurrentWindowUserSettedInformation()
        {
            string LStrXmlFileName = string.Empty;
            string LStrTemp = string.Empty;

            try
            {
                LStrXmlFileName = System.IO.Path.Combine(GClassSessionInfo.LocalMachineInfo.StrLocalApplicationData, @"UMP.Client\UMP.Setted.xml");
                XmlDocument LXmlDocSetting = new XmlDocument();
                LXmlDocSetting.Load(LStrXmlFileName);
                XmlNode LXMLNodeUserSetted = LXmlDocSetting.SelectSingleNode("UserSetted");
                XmlNode LXMLNodeLanguageSetted = LXMLNodeUserSetted.SelectSingleNode("LanguageSetted");
                XmlNode LXMLNodeStyleSetted = LXMLNodeUserSetted.SelectSingleNode("StyleSetted");
                XmlNode LXMLNodeLastUserSetted = LXMLNodeUserSetted.SelectSingleNode("SaveLastUser");
                XmlNode LXMLNodeServerSetted = LXMLNodeUserSetted.SelectSingleNode("UMPServerSetted");

                //最后使用的语言ID
                LStrTemp = LXMLNodeLanguageSetted.Attributes["LanguageID"].Value;
                if (!string.IsNullOrEmpty(LStrTemp))
                {
                    GClassSessionInfo.LangTypeInfo.LangID = int.Parse(LStrTemp);
                }

                //最后使用的Style ID
                GClassSessionInfo.ThemeInfo.Name = LXMLNodeStyleSetted.Attributes["StyleID"].Value;

                #region 最后登录用户
                if (LXMLNodeLastUserSetted.Attributes["IsSaved"].Value == "1")
                {
                    GClassSessionInfo.LocalMachineInfo.StrLastLoginAccount = LXMLNodeLastUserSetted.Attributes["UserAccount"].Value;
                }
                #endregion

                #region App Server Information
                GClassSessionInfo.AppServerInfo.Address = LXMLNodeServerSetted.Attributes["UMPServerHost"].Value;
                XmlNodeList LXmlNodeListChild = LXMLNodeServerSetted.ChildNodes;
                foreach (XmlNode LXmlNodeSingle in LXmlNodeListChild)
                {
                    if (LXmlNodeSingle.Attributes["Protocol"].Value == "http")
                    {
                        GClassSessionInfo.AppServerInfo.Port = int.Parse(LXmlNodeSingle.Attributes["BindInfo"].Value);
                        GClassSessionInfo.AppServerInfo.Protocol = "http";
                        continue;
                    }
                    if (LXmlNodeSingle.Attributes["Protocol"].Value == "https")
                    {
                        if (LXmlNodeSingle.Attributes["Used"].Value == "1")
                        {
                            GClassSessionInfo.AppServerInfo.SupportHttps = true;
                            GClassSessionInfo.AppServerInfo.Protocol = "https";
                            GClassSessionInfo.AppServerInfo.Port += 1;
                        }
                        else
                        {
                            GClassSessionInfo.AppServerInfo.SupportHttps = false;
                        }
                        continue;
                    }
                    if (LXmlNodeSingle.Attributes["Protocol"].Value == "net.tcp")
                    {
                        if (LXmlNodeSingle.Attributes["Used"].Value == "1")
                        {
                            GClassSessionInfo.AppServerInfo.SupportNetTcp = true;
                        }
                        else
                        {
                            GClassSessionInfo.AppServerInfo.SupportNetTcp = false;
                        }
                        continue;
                    }
                }
                #endregion
            }
            catch { }
        }

        /// <summary>
        /// 加载Style0000.xml资源
        /// </summary>
        public static void LoadApplicationResources()
        {
            string LStrLocalResourcePath = string.Empty;

            try
            {
                LStrLocalResourcePath = System.IO.Path.Combine(GClassSessionInfo.LocalMachineInfo.StrCommonApplicationData, @"UMP.Client\Themes", GClassSessionInfo.ThemeInfo.Name, "Style0000.xaml");
                ResourceDictionary LResourceDictionary = new ResourceDictionary();
                LResourceDictionary.Source = new Uri(LStrLocalResourcePath, UriKind.Absolute);
                Application.Current.Resources.MergedDictionaries.Clear();
                Application.Current.Resources.MergedDictionaries.Add(LResourceDictionary);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "123");
            }
        }

        /// <summary>
        /// 加载语言包
        /// </summary>
        public static void LoadApplicationLanguages()
        {
            Service00000Client LService00000Client = null;
            BasicHttpBinding LBasicHttpBinding = null;
            EndpointAddress LEndpointAddress = null;
            List<string> LListStrWcfArgs = new List<string>();

            try
            {
                IDataTableLanguage = new DataTable();

                LListStrWcfArgs.Add(GClassSessionInfo.DatabaseInfo.TypeID.ToString());
                LListStrWcfArgs.Add(GClassSessionInfo.DatabaseInfo.GetConnectionString());
                LListStrWcfArgs.Add(GClassSessionInfo.LangTypeInfo.LangID.ToString());
                LListStrWcfArgs.Add("M22");
                LListStrWcfArgs.Add("0");
                LListStrWcfArgs.Add("0");
                LBasicHttpBinding = WebHelper.CreateBasicHttpBinding();
                LEndpointAddress = WebHelper.CreateEndpointAddress(App.GClassSessionInfo.AppServerInfo, "Service00000");
                OperationDataArgs LWCFOperationReturn = new OperationDataArgs();
                LService00000Client = new Service00000Client(LBasicHttpBinding, LEndpointAddress);
                LWCFOperationReturn = LService00000Client.OperationMethodA(8, LListStrWcfArgs);
                if (LWCFOperationReturn.BoolReturn)
                {
                    IDataTableLanguage = LWCFOperationReturn.DataSetReturn.Tables[0];
                }
                
            }
            catch { IDataTableLanguage = null; }
            finally
            {
                if (LService00000Client != null)
                {
                    if (LService00000Client.State == CommunicationState.Opened) { LService00000Client.Close(); }
                }
            }
        }

        /// <summary>
        /// 加载MAMT需要的数据
        /// </summary>
        public static void LoadMamtApplicationData()
        {
            Service00000Client LService00000Client = null;
            BasicHttpBinding LBasicHttpBinding = null;
            EndpointAddress LEndpointAddress = null;
            List<string> LListStrWcfArgs = new List<string>();

            try
            {
                IDataTableMAMTSupportL = new DataTable();
                IDataTableMAMTSupportS = new DataTable();
                IDataTableMAMTLanguages = new DataTable();

                LListStrWcfArgs.Add(App.GClassSessionInfo.LangTypeInfo.LangID.ToString());

                LBasicHttpBinding = WebHelper.CreateBasicHttpBinding();
                LEndpointAddress = WebHelper.CreateEndpointAddress(App.GClassSessionInfo.AppServerInfo, "Service00000");
                OperationDataArgs LWCFOperationReturn = new OperationDataArgs();
                LService00000Client = new Service00000Client(LBasicHttpBinding, LEndpointAddress);
                LWCFOperationReturn = LService00000Client.OperationMethodA(31, LListStrWcfArgs);
                if (LWCFOperationReturn.BoolReturn)
                {
                    IDataTableMAMTSupportL = LWCFOperationReturn.DataSetReturn.Tables[0];
                    IDataTableMAMTSupportS = LWCFOperationReturn.DataSetReturn.Tables[1];
                    IDataTableMAMTLanguages = LWCFOperationReturn.DataSetReturn.Tables[2];
                    GClassSessionInfo.LangTypeInfo.LangID = int.Parse(LWCFOperationReturn.StringReturn);
                }
                else
                { IDataTableMAMTSupportL = null; IDataTableMAMTSupportS = null; IDataTableMAMTLanguages = null; }

            }
            catch { IDataTableMAMTSupportL = null; IDataTableMAMTSupportS = null; IDataTableMAMTLanguages = null; }
            finally
            {
                if (LService00000Client != null)
                {
                    if (LService00000Client.State == CommunicationState.Opened) { LService00000Client.Close(); }
                }
            }
        }

        public static string GetDisplayCharater(string AStrMessageID)
        {
            string LStrReturn = string.Empty;
            string LStrC005 = string.Empty;
            string LStrC006 = string.Empty;

            try
            {
                DataRow[] ObjectLanguageRow = IDataTableLanguage.Select("C002 = '" + AStrMessageID + "'");
                if (ObjectLanguageRow.Count() == 0) { return LStrReturn; }
                LStrC005 = ObjectLanguageRow[0]["C005"].ToString();
                LStrC006 = ObjectLanguageRow[0]["C006"].ToString();
                if (string.IsNullOrEmpty(LStrC005)) { LStrC005 = ""; }
                if (string.IsNullOrEmpty(LStrC006)) { LStrC006 = ""; }
                LStrReturn = LStrC005 + LStrC006;
            }
            catch { LStrReturn = string.Empty; }

            return LStrReturn;
        }

        public static string GetDisplayCharater(string AStrObjectName, string AStrTargetName)
        {
            string LStrReturn = string.Empty;
            string LStrC005 = string.Empty;
            string LStrC006 = string.Empty;

            try
            {
                DataRow[] ObjectLanguageRow = IDataTableLanguage.Select("C011 = '" + AStrObjectName + "' AND C012 = '" + AStrTargetName + "'");
                LStrC005 = ObjectLanguageRow[0]["C005"].ToString();
                LStrC006 = ObjectLanguageRow[0]["C006"].ToString();
                if (string.IsNullOrEmpty(LStrC005)) { LStrC005 = ""; }
                if (string.IsNullOrEmpty(LStrC006)) { LStrC006 = ""; }
                LStrReturn = LStrC005 + LStrC006;
            }
            catch { LStrReturn = string.Empty; }

            return LStrReturn;
        }

        public static string GetDisplayCharaterMAMT(string AStrMessageID)
        {
            string LStrReturn = string.Empty;

            try
            {
                DataRow[] LDataRowObjectLanguages = IDataTableMAMTLanguages.Select("C001 = '" + AStrMessageID + "'");
                if (LDataRowObjectLanguages.Length <= 0) { return LStrReturn; }
                LStrReturn = LDataRowObjectLanguages[0]["C002"].ToString();
            }
            catch { LStrReturn = string.Empty; }

            return LStrReturn;
        }

        public static string GetStyleShowName(string AStrStyleName)
        {
            string LStrReturn = string.Empty;

            try
            {
                DataRow[] LDataRowObjectStyle = IDataTableMAMTSupportS.Select("C001 = '" + AStrStyleName + "'");
                if (LDataRowObjectStyle.Length <= 0) { return LStrReturn; }
                LStrReturn = LDataRowObjectStyle[0]["C002"].ToString();
            }
            catch { LStrReturn = string.Empty; }

            return LStrReturn;
        }

        public static string CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType AKeyIVID)
        {
            string LStrReturn = string.Empty;
            int LIntRand = 0;
            string LStrTemp = string.Empty;

            try
            {
                Random LRandom = new Random();
                LStrReturn = DateTime.Now.ToString("yyyyMMddHHmmss");
                LIntRand = LRandom.Next(0, 14);
                LStrTemp = LIntRand.ToString("00");
                LStrReturn = LStrReturn.Insert(LIntRand, "VCT");
                LIntRand = LRandom.Next(0, 17);
                LStrTemp += LIntRand.ToString("00");
                LStrReturn = LStrReturn.Insert(LIntRand, "UMP");
                LIntRand = LRandom.Next(0, 20);
                LStrTemp += LIntRand.ToString("00");
                LStrReturn = LStrReturn.Insert(LIntRand, ((int)AKeyIVID).ToString("000"));

                LStrReturn = EncryptionAndDecryption.EncryptStringY(LStrTemp + LStrReturn);
            }
            catch { LStrReturn = string.Empty; }

            return LStrReturn;
        }

        /// <summary>
        /// 创建分割符
        /// </summary>
        /// <param name="AsciiCode">Ascii码</param>
        /// <returns></returns>
        public static string AscCodeToChr(int AsciiCode)
        {
            System.Text.ASCIIEncoding asciiEncoding = new System.Text.ASCIIEncoding();
            byte[] byteArray = new byte[] { (byte)AsciiCode };
            string strCharacter = asciiEncoding.GetString(byteArray);
            return (strCharacter);
        }

        #region 关闭浏览器，用户退出系统
        protected override void OnExit(ExitEventArgs e)
        {
            string LStrTemp = string.Empty;
            if (GNetPipeHelper != null)
            {
                this.Dispatcher.Invoke(new Action(() =>
                        {
                            GNetPipeHelper.Stop();
                        }));
            }
            UserSignOutSystem("E", ref LStrTemp);
        }
        #endregion

        #region 用户退出系统或切换用户
        public static bool UserSignOutSystem(string AStrExitType, ref string AStrMessage)
        {
            bool LBoolReturn = true;

            Service00000Client LService00000Client = null;
            BasicHttpBinding LBasicHttpBinding = null;
            EndpointAddress LEndpointAddress = null;
            List<string> LListStrSignOutArgs = new List<string>();
            string LStrVerificationCode004 = string.Empty;
            string LStrRentCode5 = string.Empty;
            string LStrUserID = string.Empty;
            string LStrSessionID = string.Empty;

            try
            {
                if (!string.IsNullOrEmpty(GClassSessionInfo.UserInfo.Account))
                {
                    LStrVerificationCode004 = App.CreateVerificationCode(PFShareClassesC.EncryptionAndDecryption.UMPKeyAndIVType.M004);

                    LStrRentCode5 = GClassSessionInfo.RentInfo.Token;
                    LStrRentCode5 = EncryptionAndDecryption.EncryptDecryptString(LStrRentCode5, LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    LListStrSignOutArgs.Add(LStrRentCode5);

                    LStrUserID = GClassSessionInfo.UserInfo.UserID.ToString();
                    LStrUserID = EncryptionAndDecryption.EncryptDecryptString(LStrUserID, LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    LListStrSignOutArgs.Add(LStrUserID);

                    LStrSessionID = GClassSessionInfo.SessionID;
                    LStrSessionID = EncryptionAndDecryption.EncryptDecryptString(LStrSessionID, LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    LListStrSignOutArgs.Add(GClassSessionInfo.SessionID);

                    LBasicHttpBinding = WebHelper.CreateBasicHttpBinding();
                    LEndpointAddress = WebHelper.CreateEndpointAddress(App.GClassSessionInfo.AppServerInfo, "Service00000");
                    OperationDataArgs LWCFOperationReturn = new OperationDataArgs();
                    LService00000Client = new Service00000Client(LBasicHttpBinding, LEndpointAddress);
                    LWCFOperationReturn = LService00000Client.OperationMethodA(12, LListStrSignOutArgs);
                }
            }
            catch(Exception ex)
            {
                LBoolReturn = false;
                AStrMessage = ex.ToString();
            }
            finally
            {
                if (LService00000Client != null)
                {
                    if (LService00000Client.State == CommunicationState.Opened) { LService00000Client.Close(); }
                }
            }

            return LBoolReturn;
        }
        #endregion
    }
}
