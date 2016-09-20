using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Security;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Xml;
using PFShareClassesS;
using UMP.MAMT.BasicModule;
using UMP.MAMT.PublicClasses;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using PFShareClasses01;
using VoiceCyber.SDKs.Licenses;
using VoiceCyber.UMP.Encryptions;
using UMP.MAMT.LicenseServer;
using System.Threading;

namespace UMP.MAMT
{
    public partial class App : Application, IEncryptable
    {
        [DllImport("kernel32.dll")]
        public static extern int GetUserDefaultUILanguage();

        public static SystemMainWindow GSystemMainWindow = null;

        public static LogOperator LogOperator;
        public static string AppName = "UMPMAMT";

        #region License Connect Member

        private static LicConnector mLicenseConn = null;
        private static int mConnectTimeout = 8;   //连接超时时间，单位s
        private static int mReceiveTimeout = 15;   //接收消息超时时间，单位s
        private static string LicReturn;
        private static bool mIsConnected;
        private static ManualResetEvent mResetEvent = new ManualResetEvent(false);

        #endregion

        #region 全局变量
        //系统简称
        public static string GStrApplicationReferredTo = "UMP";
        //当前机器的所有IP地址，中间用char(27)分开
        public static string GStrComputerIPAddress = string.Empty;
        //当前机器的名称
        public static string GStrComputerName = string.Empty;
        //当前windows用户登录的默认语言ID
        public static string GStrComputerCurrentLanguageID = string.Empty;
        //当前登录Application用户的默认语言ID，如系统是简体中文的，用户登录时修改成英语 由 2052 -> 1033
        public static string GStrLoginUserCurrentLanguageID = string.Empty;
        //当前程序运行的基本目录
        public static string GStrApplicationDirectory = string.Empty;
        //当前系统ProgramData路径
        public static string GStrProgramDataDirectory = string.Empty;
        //当前用户的MyDocuments目录
        public static string GStrUserMyDocumentsDirectory = string.Empty;
        //UMP.PF站点的根目录
        public static string GStrSiteRootFolder = string.Empty;
        //系统默认分割符
        public static string GStrSpliterChar = string.Empty;
        //改应用是否在UMP应用服务器上运行
        public static bool GBoolRunAtServer = true;
        //当前季节编码 01 - 04
        public static string GStrSeasonCode = string.Empty;
        //公司名称
        public static string GStrCompanyName = string.Empty;
        //当前程序版本
        public static string GStrApplicationVersion = string.Empty;
        //当前运行时的Exception
        public static string GStrCatchException = string.Empty;
        //是否允许远程连接
        public static string GStrAllowRemoteConnect = string.Empty;
        //系统默认的安装证书Hash值
        public static string GStrCertificateHash = "C3BBF9EA2C0DA7FEAA17043A0A6010A522ABAB87";
        //UMP.PF站点绑定的端口
        public static int GIntSiteBindingPort = 8081;
        //当前登录用户的密码
        public static string GStrLoginPassword = string.Empty;
        /// <summary>
        /// 当前是否已创建数据库,如果创建好了就去掉创建数据库按钮，并打开修改数据库按钮（只允许修改数据库账户跟密码）。
        /// </summary>
        public static bool IsCreatedDB = false;

        public static SessionInfo GClassSessionInfo = new SessionInfo();
        public static bool IBoolIsBusy = false;
        #endregion

        #region 系统启动时的语言包、风格
        private static DataTable GDataTableSupportLanguages = null;
        private static DataTable GDataTableSupportStyles = null;
        private static DataTable GDataTableNecessaryLanguages = null;
        private static DataTable GDataTableConvertData = null;
        #endregion

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            CreateLogOperator();
            CreateSpliterCharater();
            GetApplicationBasicDirectory();
            GetSeasonCode();
            GetUserWindowsDefaultLanguage();
            GetComputerMachineName();
            GetProgramDataDirectory();
            GetUserMyDocumentsDirectory();
            GetNetworkAllIPAddress();
            ApplicationRunAtServer();
            GetApplicationVersion();
            if (GBoolRunAtServer) { InitStartedAppliactionData(); }
            InitGlobalParameterFromLangPackage();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (LogOperator != null)
            {
                LogOperator.Stop();
            }
            LogOperator = null;
            if (mLicenseConn != null)
            {
                mLicenseConn.Close();
            }
            mLicenseConn = null;
            base.OnExit(e);
        }

        #region 加载各种资源
        /// <summary>
        /// 加载系统样式资源
        /// </summary>
        public static void LoadApplicationResources()
        {
            try
            {
                string LStrResourceFile = string.Empty;

                LStrResourceFile = System.IO.Path.Combine(GStrApplicationDirectory, @"Styles\Style" + GStrSeasonCode + ".xaml");
                ResourceDictionary LResourceDictionary = new ResourceDictionary();
                LResourceDictionary.Source = new Uri(LStrResourceFile, UriKind.Absolute);
                Application.Current.Resources.MergedDictionaries.Clear();
                Application.Current.Resources.MergedDictionaries.Add(LResourceDictionary);

                App.WriteLog("LoadAppResources", string.Format("AppResources loaded.\t{0}", LStrResourceFile));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// 画窗口背景图片
        /// </summary>
        /// <param name="AWindowTarget"></param>
        public static void DrawWindowsBackGround(Window AWindowTarget)
        {
            DrawingBackground.DrawWindowsBackgond(AWindowTarget);
        }
        #endregion

        #region 初始化全局变量
        private void GetApplicationBasicDirectory()
        {
            int LIntLastIndexOf = 0;

            try
            {
                App.GStrApplicationDirectory = Environment.CurrentDirectory;
                LIntLastIndexOf = App.GStrApplicationDirectory.LastIndexOf("\\");
                GStrSiteRootFolder = App.GStrApplicationDirectory.Substring(0, LIntLastIndexOf);

                WriteLog("AppInit", string.Format("GStrSiteRootFolder is {0}", GStrSiteRootFolder));
            }
            catch { App.GStrApplicationDirectory = string.Empty; }
        }

        private void GetSeasonCode()
        {
            try
            {
                DateTime LDateTimeNow = DateTime.Now;

                if (LDateTimeNow.Month == 2 || LDateTimeNow.Month == 3 || LDateTimeNow.Month == 4) { GStrSeasonCode = "01"; }
                if (LDateTimeNow.Month == 5 || LDateTimeNow.Month == 6 || LDateTimeNow.Month == 7) { GStrSeasonCode = "02"; }
                if (LDateTimeNow.Month == 8 || LDateTimeNow.Month == 9 || LDateTimeNow.Month == 10) { GStrSeasonCode = "03"; }
                if (LDateTimeNow.Month == 11 || LDateTimeNow.Month == 12 || LDateTimeNow.Month == 1) { GStrSeasonCode = "04"; }
            }
            catch { GStrSeasonCode = "04"; }

            WriteLog("AppInit", string.Format("GStrSeasonCode is {0}", GStrSeasonCode));
        }

        private void CreateSpliterCharater()
        {
            try
            {
                System.Text.ASCIIEncoding LAsciiEncoding = new System.Text.ASCIIEncoding();
                byte[] LByteArray = new byte[] { (byte)27 };
                string LStrCharacter = LAsciiEncoding.GetString(LByteArray);
                GStrSpliterChar = LStrCharacter;
            }
            catch { GStrSpliterChar = string.Empty; }
        }

        private void GetUserWindowsDefaultLanguage()
        {
            try
            {
                GStrComputerCurrentLanguageID = GetUserDefaultUILanguage().ToString();
                GClassSessionInfo.LangTypeInfo.LangID = GetUserDefaultUILanguage();
                GClassSessionInfo.LangTypeID = GetUserDefaultUILanguage();
            }
            catch
            {
                GStrComputerCurrentLanguageID = "2052";
                GClassSessionInfo.LangTypeInfo.LangID = 2052;
                GClassSessionInfo.LangTypeID = 2052;
            }
            finally
            {
                GStrLoginUserCurrentLanguageID = GStrComputerCurrentLanguageID;
            }

            WriteLog("AppInit", string.Format("GStrLoginUserCurrentLanguageID is {0}", GStrLoginUserCurrentLanguageID));
        }

        private void GetComputerMachineName()
        {
            try
            {
                GStrComputerName = Environment.MachineName;
            }
            catch { GStrComputerName = string.Empty; }

            WriteLog("AppInit", string.Format("GStrComputerName is {0}", GStrComputerName));
        }

        private void GetProgramDataDirectory()
        {
            try
            {
                GStrProgramDataDirectory = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            }
            catch { App.GStrProgramDataDirectory = string.Empty; }

            WriteLog("AppInit", string.Format("GStrProgramDataDirectory is {0}", GStrProgramDataDirectory));
        }

        private void GetUserMyDocumentsDirectory()
        {
            try
            {
                GStrUserMyDocumentsDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }
            catch { GStrUserMyDocumentsDirectory = string.Empty; }

            WriteLog("AppInit", string.Format("GStrUserMyDocumentsDirectory is {0}", GStrUserMyDocumentsDirectory));
        }

        private void GetNetworkAllIPAddress()
        {
            try
            {
                App.GStrComputerIPAddress = string.Empty;
                NetworkInterface[] LNetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
                foreach (NetworkInterface LNetworkInterfaceSingle in LNetworkInterfaces)
                {
                    IPInterfaceProperties LIPInterfaceProperty = LNetworkInterfaceSingle.GetIPProperties();
                    UnicastIPAddressInformationCollection LUnicastIPAddressInformationCollection = LIPInterfaceProperty.UnicastAddresses;
                    foreach (UnicastIPAddressInformation LUnicastIPAddressInformationSingle in LUnicastIPAddressInformationCollection)
                    {
                        if (LUnicastIPAddressInformationSingle.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            GStrComputerIPAddress += LUnicastIPAddressInformationSingle.Address.ToString() + GStrSpliterChar;
                        }
                    }
                }
            }
            catch { GStrComputerIPAddress = string.Empty; }

            WriteLog("AppInit", string.Format("GStrComputerIPAddress is {0}", GStrComputerIPAddress));
        }

        private void ApplicationRunAtServer()
        {
            int LIntLastIndexOf = 0;
            string LStrParentFolder = string.Empty;
            string LStrCheckExistFileUMPMain = string.Empty;
            string LStrCheckExistFileUMPPF = string.Empty;

            try
            {
                GBoolRunAtServer = false;
                LIntLastIndexOf = GStrApplicationDirectory.LastIndexOf("\\");
                LStrParentFolder = GStrApplicationDirectory.Substring(0, LIntLastIndexOf);
                LStrCheckExistFileUMPMain = System.IO.Path.Combine(LStrParentFolder, "UMPMain.xbap");
                LStrCheckExistFileUMPPF = System.IO.Path.Combine(LStrParentFolder, "UMP.PF.html");
                if (System.IO.File.Exists(LStrCheckExistFileUMPMain) && System.IO.File.Exists(LStrCheckExistFileUMPPF)) { GBoolRunAtServer = true; }
            }
            catch { GBoolRunAtServer = false; }

            WriteLog("AppInit", string.Format("GBoolRunAtServer is {0}", GBoolRunAtServer));
        }

        private void InitGlobalParameterFromLangPackage()
        {
            GStrCompanyName = GetDisplayCharater("M01003");
            GStrApplicationReferredTo = GetDisplayCharater("M01015");
            GStrAllowRemoteConnect = GetDisplayCharater("M02001");

            WriteLog("InitGlobalParam", string.Format("GStrCompanyName:{0};GStrApplicationReferredTo:{1};GStrAllowRemoteConnect:{2}",
                GStrCompanyName,
                GStrApplicationReferredTo,
                GStrAllowRemoteConnect));
        }

        private void GetApplicationVersion()
        {
            string LStrVersion = string.Empty;
            LStrVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            string[] LStrArrayVersion = LStrVersion.Split('.');
            //GStrApplicationVersion = "Version: " + LStrArrayVersion[0] + "." + int.Parse(LStrArrayVersion[1]).ToString("00") + "." + int.Parse(LStrArrayVersion[2]).ToString("000");
            GStrApplicationVersion = LStrArrayVersion[0] + "." + int.Parse(LStrArrayVersion[1]).ToString("00") + "." + int.Parse(LStrArrayVersion[2]).ToString("000");

            WriteLog("AppInit", string.Format("GStrApplicationVersion is {0}", GStrApplicationVersion));
        }
        #endregion

        #region 初始化 系统启动时的语言包、风格（该应用程序运行在UMP应用服务器上）
        public static void InitStartedAppliactionData()
        {
            string LStrXMLFileFolder = string.Empty;
            string LStrXmlFileName = string.Empty;

            try
            {
                #region 初始化表
                GDataTableSupportLanguages = new DataTable();
                GDataTableSupportStyles = new DataTable();
                GDataTableNecessaryLanguages = new DataTable();
                GDataTableConvertData = new DataTable();

                GDataTableSupportLanguages.Columns.Add("C001", typeof(string));
                GDataTableSupportLanguages.Columns.Add("C002", typeof(string));
                GDataTableSupportLanguages.Columns.Add("C003", typeof(int));
                GDataTableSupportLanguages.Columns.Add("C004", typeof(string));
                GDataTableSupportLanguages.Columns.Add("C005", typeof(string));

                GDataTableSupportStyles.Columns.Add("C001", typeof(string));
                GDataTableSupportStyles.Columns.Add("C002", typeof(string));

                GDataTableNecessaryLanguages.Columns.Add("C001", typeof(string));
                GDataTableNecessaryLanguages.Columns.Add("C002", typeof(string));
                GDataTableNecessaryLanguages.Columns.Add("C003", typeof(string));
                GDataTableNecessaryLanguages.Columns.Add("C004", typeof(string));
                GDataTableNecessaryLanguages.Columns.Add("C005", typeof(string));

                GDataTableConvertData.Columns.Add("DataSource", typeof(string));
                GDataTableConvertData.Columns.Add("DataConverted", typeof(string));
                #endregion

                #region 加载配置文件
                LStrXMLFileFolder = System.IO.Path.Combine(App.GStrApplicationDirectory, "Languages");
                LStrXmlFileName = System.IO.Path.Combine(LStrXMLFileFolder, "S" + App.GStrLoginUserCurrentLanguageID + ".xml");
                if (!File.Exists(LStrXmlFileName))
                {
                    LStrXmlFileName = System.IO.Path.Combine(LStrXMLFileFolder, "S2052.xml");
                    App.GStrLoginUserCurrentLanguageID = "2052";
                }
                XmlDocument LXmlDocument = new XmlDocument();
                LXmlDocument.Load(LStrXmlFileName);

                WriteLog("AppInit", string.Format("Languages loaded.\t{0}", LStrXmlFileName));

                #endregion

                #region 读取支持的语言列表
                XmlNode LXMLNodeSupportLanguages = LXmlDocument.SelectSingleNode("UMPMamt").SelectSingleNode("SupportLanguages");
                XmlNodeList LXmlNodeSupportLanguages = LXMLNodeSupportLanguages.ChildNodes;
                foreach (XmlNode LXmlNodeSingleLanguage in LXmlNodeSupportLanguages)
                {
                    DataRow LDataRow = GDataTableSupportLanguages.NewRow();
                    LDataRow.BeginEdit();
                    LDataRow["C001"] = LXmlNodeSingleLanguage.Attributes["C001"].Value;
                    LDataRow["C002"] = LXmlNodeSingleLanguage.Attributes["C002"].Value;
                    LDataRow["C003"] = int.Parse(LXmlNodeSingleLanguage.Attributes["C003"].Value);
                    LDataRow["C004"] = LXmlNodeSingleLanguage.Attributes["C004"].Value;
                    LDataRow["C005"] = LXmlNodeSingleLanguage.Attributes["C005"].Value;
                    LDataRow.EndEdit();
                    GDataTableSupportLanguages.Rows.Add(LDataRow);
                }
                #endregion

                #region 读取支持的Style
                XmlNode LXMLNodeSupportStyle = LXmlDocument.SelectSingleNode("UMPMamt").SelectSingleNode("SupportStyle");
                XmlNodeList LXmlNodeSupportStyles = LXMLNodeSupportStyle.ChildNodes;
                foreach (XmlNode LXmlNodeSingleStyle in LXmlNodeSupportStyles)
                {
                    DataRow LDataRow = GDataTableSupportStyles.NewRow();
                    LDataRow.BeginEdit();
                    LDataRow["C001"] = LXmlNodeSingleStyle.Attributes["C001"].Value;
                    LDataRow["C002"] = LXmlNodeSingleStyle.Attributes["C002"].Value;
                    LDataRow.EndEdit();
                    GDataTableSupportStyles.Rows.Add(LDataRow);
                }
                #endregion

                #region 读取语言包
                XmlNode LXMLNodeLanguages = LXmlDocument.SelectSingleNode("UMPMamt").SelectSingleNode("Languages");
                XmlNodeList LXmlNodeLanguages = LXMLNodeLanguages.ChildNodes;
                foreach (XmlNode LXmlNodeSingleLanguageItem in LXmlNodeLanguages)
                {
                    DataRow LDataRow = GDataTableNecessaryLanguages.NewRow();
                    LDataRow.BeginEdit();
                    LDataRow["C001"] = LXmlNodeSingleLanguageItem.Attributes["C001"].Value;
                    LDataRow["C002"] = LXmlNodeSingleLanguageItem.Attributes["C002"].Value;
                    LDataRow["C003"] = LXmlNodeSingleLanguageItem.Attributes["C003"].Value;
                    LDataRow["C004"] = LXmlNodeSingleLanguageItem.Attributes["C004"].Value;
                    LDataRow["C005"] = LXmlNodeSingleLanguageItem.Attributes["C005"].Value;
                    LDataRow.EndEdit();
                    GDataTableNecessaryLanguages.Rows.Add(LDataRow);
                }
                #endregion

                #region 读取数据转换信息
                XmlNode LXMLDataConvert = LXmlDocument.SelectSingleNode("UMPMamt").SelectSingleNode("DataConvert");
                XmlNodeList LXmlNodeDataConverts = LXMLDataConvert.ChildNodes;
                foreach (XmlNode LXmlNodeSingleDataConvert in LXmlNodeDataConverts)
                {
                    DataRow LDataRow = GDataTableConvertData.NewRow();
                    LDataRow.BeginEdit();
                    LDataRow["DataSource"] = LXmlNodeSingleDataConvert.Attributes["DataSource"].Value;
                    LDataRow["DataConverted"] = LXmlNodeSingleDataConvert.Attributes["DataConverted"].Value;
                    LDataRow.EndEdit();
                    GDataTableConvertData.Rows.Add(LDataRow);
                }
                #endregion

                WriteLog("AppInit", string.Format("Init end."));
            }
            catch
            {
                GDataTableSupportLanguages = null; GDataTableSupportStyles = null; GDataTableNecessaryLanguages = null; GDataTableConvertData = null;
            }
        }
        #endregion

        public static ContextMenu InitApplicationMenu()
        {
            ContextMenu LContextMenu = null;
            string LStrImagePath = string.Empty;
            string LStrLangID = string.Empty;

            string LStrStylesPath = string.Empty;
            string LStrStyleName = string.Empty;

            try
            {
                LStrImagePath = System.IO.Path.Combine(GStrApplicationDirectory, "Images");
                LContextMenu = new ContextMenu();
                LContextMenu.Opacity = 0.85;

                #region 系统支持的语言列表
                //MenuItem LMenuItemSurportLanguage = new MenuItem();
                //LMenuItemSurportLanguage.Header = GetDisplayCharater("M01000");
                //LMenuItemSurportLanguage.DataContext = "M100";
                //Image LImageIcon1 = new Image();
                //LImageIcon1.Height = 16; LImageIcon1.Width = 16;
                //LImageIcon1.Source = new BitmapImage(new Uri(System.IO.Path.Combine(LStrImagePath, @"00000004.ico"), UriKind.Absolute));
                //LMenuItemSurportLanguage.Icon = LImageIcon1;
                //LMenuItemSurportLanguage.Style = (Style)App.Current.Resources["MenuItemFontStyle"];
                foreach (DataRow LDataRowSingleLanguage in GDataTableSupportLanguages.Rows)
                {
                    MenuItem LMenuItemSingle = new MenuItem();
                    LStrLangID = LDataRowSingleLanguage["C004"].ToString();
                    LMenuItemSingle.Header = "(" + LStrLangID + ")" + LDataRowSingleLanguage["C002"].ToString();
                    LMenuItemSingle.DataContext = "L-" + LStrLangID;
                    LMenuItemSingle.Tag = LDataRowSingleLanguage["C002"].ToString();
                    if (LStrLangID == GStrLoginUserCurrentLanguageID) { LMenuItemSingle.IsChecked = true; }
                    LMenuItemSingle.Style = (Style)App.Current.Resources["MenuItemFontStyle"];
                    LMenuItemSingle.Click += LMenuItemApplicationMenuClicked;
                    //LMenuItemSurportLanguage.Items.Add(LMenuItemSingle);
                    LContextMenu.Items.Add(LMenuItemSingle);
                }

                //LContextMenu.Items.Add(LMenuItemSurportLanguage);
                #endregion

                LContextMenu.Items.Add(new Separator());

                #region 系统可以选择的样式
                //MenuItem LMenuItemSurportStyles = new MenuItem();
                //LMenuItemSurportStyles.Header = App.GetDisplayCharater("M01001");
                //LMenuItemSurportStyles.DataContext = "M200";
                //Image LImageIcon2 = new Image();
                //LImageIcon2.Height = 16; LImageIcon2.Width = 16;
                //LImageIcon2.Source = new BitmapImage(new Uri(System.IO.Path.Combine(LStrImagePath, "00000008.png"), UriKind.Absolute));
                //LMenuItemSurportStyles.Icon = LImageIcon2;
                //LMenuItemSurportStyles.Style = (Style)App.Current.Resources["MenuItemFontStyle"];

                LStrStylesPath = System.IO.Path.Combine(GStrApplicationDirectory, "Styles");

                DirectoryInfo LDirInfo = new DirectoryInfo(LStrStylesPath);
                FileInfo[] LFileInfoSubFiles = LDirInfo.GetFiles();
                foreach (FileInfo LFileInfoSingle in LFileInfoSubFiles)
                {
                    LStrStyleName = LFileInfoSingle.Name.Replace(LFileInfoSingle.Extension, "");
                    if (!LStrStyleName.Contains("Style")) { continue; }

                    MenuItem LMenuItemSingle = new MenuItem();
                    LMenuItemSingle.Header = GetStyleShowName(LStrStyleName);
                    LMenuItemSingle.DataContext = "S-" + LStrStyleName.Substring(5);
                    if ("Style" + GStrSeasonCode == LStrStyleName) { LMenuItemSingle.IsChecked = true; }
                    LMenuItemSingle.Style = (Style)App.Current.Resources["MenuItemFontStyle"];
                    LMenuItemSingle.Click += LMenuItemApplicationMenuClicked;
                    //LMenuItemSurportStyles.Items.Add(LMenuItemSingle);
                    LContextMenu.Items.Add(LMenuItemSingle);
                }
                //LContextMenu.Items.Add(LMenuItemSurportStyles);
                #endregion

            }
            catch { LContextMenu = null; }

            return LContextMenu;
        }

        private static void LMenuItemApplicationMenuClicked(object sender, RoutedEventArgs e)
        {
            string LStrClickedData = string.Empty;
            string LStrLanguageID = string.Empty;
            string LStrStyleID = string.Empty;

            try
            {
                MamtOperationEventArgs LEventArgs = new MamtOperationEventArgs();
                MenuItem LMenuItemClicked = sender as MenuItem;

                LStrClickedData = LMenuItemClicked.DataContext.ToString();
                if (LStrClickedData.Substring(0, 2) == "L-")
                {
                    LEventArgs.StrElementTag = "CLID";
                    LEventArgs.ObjSource = LStrClickedData.Substring(2);
                }
                if (LStrClickedData.Substring(0, 2) == "S-")
                {
                    LEventArgs.StrElementTag = "CSID";
                    LEventArgs.ObjSource = LStrClickedData.Substring(2);
                }
                GSystemMainWindow.ObjectOperationsEvent(LEventArgs);
            }
            catch { }
        }

        public static string GetDisplayCharater(string AStrMessageID)
        {
            string LStrReturn = string.Empty;

            try
            {
                DataRow[] LDataRowObjectLanguages = GDataTableNecessaryLanguages.Select("C001 = '" + AStrMessageID + "'");
                if (LDataRowObjectLanguages.Length <= 0) { return LStrReturn; }
                LStrReturn = LDataRowObjectLanguages[0]["C002"].ToString();
            }
            catch { LStrReturn = string.Empty; }

            return LStrReturn;
        }

        public static string GetConvertedData(string AStrDataSource)
        {
            string LStrReturn = string.Empty;

            try
            {
                LStrReturn = GDataTableConvertData.Select("DataSource = '" + AStrDataSource + "'").FirstOrDefault().Field<string>(1);
            }
            catch { LStrReturn = string.Empty; }

            return LStrReturn;
        }

        public static string GetStyleShowName(string AStrStyleName)
        {
            string LStrReturn = string.Empty;

            try
            {
                DataRow[] LDataRowObjectStyle = GDataTableSupportStyles.Select("C001 = '" + AStrStyleName + "'");
                if (LDataRowObjectStyle.Length <= 0) { return LStrReturn; }
                LStrReturn = LDataRowObjectStyle[0]["C002"].ToString();
            }
            catch { LStrReturn = string.Empty; }

            return LStrReturn;
        }

        #region 显示当前状态
        /// <summary>
        /// 在状态栏中显示当前正在处理的操作提示
        /// </summary>
        /// <param name="AIntType">
        /// (0)-错误
        /// (1)-运行中
        /// (2)-警告
        /// (int.MaxValue)-显示就绪
        /// </param>
        /// <param name="AStrStatusDesc">提示内容</param>
        public static void ShowCurrentStatus(int AIntType, string AStrStatusDesc)
        {
            MamtOperationEventArgs LEventArgs = new MamtOperationEventArgs();

            LEventArgs.StrElementTag = "SSLC";
            LEventArgs.ObjSource = AIntType.ToString();
            LEventArgs.AppenObjeSource1 = AStrStatusDesc;
            LEventArgs.AppenObjeSource2 = "0";
            GSystemMainWindow.ObjectOperationsEvent(LEventArgs);
        }

        public static void ShowCurrentStatus(int AIntType, string AStrStatusDesc, bool ABoolWithWait)
        {
            MamtOperationEventArgs LEventArgs = new MamtOperationEventArgs();

            LEventArgs.StrElementTag = "SSLC";
            LEventArgs.ObjSource = AIntType.ToString();
            LEventArgs.AppenObjeSource1 = AStrStatusDesc;
            LEventArgs.AppenObjeSource2 = "1";
            GSystemMainWindow.ObjectOperationsEvent(LEventArgs);
        }

        #endregion

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

        //显示系统运行过程中异常
        public static void ShowExceptionMessage(string AStrMessage)
        {
            MessageBox.Show(GetDisplayCharater("E00000") + "\n" + AStrMessage, GStrApplicationReferredTo, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        //获取本地网站的绑定信息
        public static DataTable GetIISBindingProtocol()
        {
            DataTable LDataTableReturn = new DataTable();

            string LStrXmlFileName = string.Empty;
            string LStrParentFolder = string.Empty;
            int LIntLastIndexOf = 0;

            try
            {
                App.GStrCatchException = string.Empty;

                LIntLastIndexOf = GStrApplicationDirectory.LastIndexOf("\\");
                LStrParentFolder = GStrApplicationDirectory.Substring(0, LIntLastIndexOf);

                LStrXmlFileName = System.IO.Path.Combine(LStrParentFolder, @"GlobalSettings\UMP.Server.01.xml");

                LDataTableReturn.Columns.Add("Activated", typeof(string));
                LDataTableReturn.Columns.Add("Protocol", typeof(string));
                LDataTableReturn.Columns.Add("BindInfo", typeof(string));
                LDataTableReturn.Columns.Add("IPAddress", typeof(string));
                LDataTableReturn.Columns.Add("OtherArgs", typeof(string));
                LDataTableReturn.Columns.Add("Used", typeof(string));
                LDataTableReturn.Columns.Add("Attribute02", typeof(string));//是否开启云功能

                XmlDocument LXmlDocServer01 = new XmlDocument();
                LXmlDocServer01.Load(LStrXmlFileName);
                XmlNode LXMLNodeSection = LXmlDocServer01.SelectSingleNode("UMPSetted").SelectSingleNode("IISBindingProtocol");
                XmlNodeList LXmlNodeBindingProtocol = LXMLNodeSection.ChildNodes;
                AppServerInfo serverInfo = new AppServerInfo();
                serverInfo.Protocol = "https";
                foreach (XmlNode LXmlNodeSingleBinding in LXmlNodeBindingProtocol)
                {
                    DataRow LDataRow = LDataTableReturn.NewRow();
                    LDataRow.BeginEdit();
                    LDataRow["Activated"] = LXmlNodeSingleBinding.Attributes["Activated"].Value;
                    LDataRow["Protocol"] = LXmlNodeSingleBinding.Attributes["Protocol"].Value;
                    LDataRow["BindInfo"] = LXmlNodeSingleBinding.Attributes["BindInfo"].Value;
                    LDataRow["IPAddress"] = LXmlNodeSingleBinding.Attributes["IPAddress"].Value;
                    LDataRow["OtherArgs"] = LXmlNodeSingleBinding.Attributes["OtherArgs"].Value;
                    LDataRow["Used"] = LXmlNodeSingleBinding.Attributes["Used"].Value;
                    LDataRow.EndEdit();
                    if (LDataRow["Protocol"].ToString().ToUpper() == "HTTP")
                    {
                        GIntSiteBindingPort = int.Parse(LDataRow["BindInfo"].ToString());
                        serverInfo.Address = LDataRow["IPAddress"].ToString();
                        serverInfo.Port = GIntSiteBindingPort + 1;
                    }
                    LDataTableReturn.Rows.Add(LDataRow);
                }

                DataRow LDataRow2 = LDataTableReturn.NewRow();
                LDataRow2.BeginEdit();
                LDataRow2["Protocol"] = "cloud";
                LDataRow2["Attribute02"] = LXmlDocServer01.SelectSingleNode("UMPSetted").SelectSingleNode("UMPApplication").Attributes["Attribute02"].Value;
                LDataRow2.EndEdit();
                LDataTableReturn.Rows.Add(LDataRow2);
                serverInfo.SupportHttps = true;
                serverInfo.SupportNetTcp = true;
                GClassSessionInfo.AppServerInfo = serverInfo;
                GClassSessionInfo.AppName = string.Empty;
            }
            catch (Exception ex)
            {
                LDataTableReturn = new DataTable();
                App.GStrCatchException = ex.ToString();
            }

            return LDataTableReturn;
        }

        //获取本地已经安装的UMP.PF.Certificate.pfx信息
        public static DataTable GetCertificateInstalledInfo()
        {
            DataTable LDataTableReturn = new DataTable();

            try
            {
                LDataTableReturn.Columns.Add("StoreName", typeof(string));
                LDataTableReturn.Columns.Add("IsInstalled", typeof(string));

                StoreName[] LStoreNameArray = new StoreName[3];
                string[] LStrStoreNameArray = new string[3];

                LStoreNameArray[0] = StoreName.My;
                LStoreNameArray[1] = StoreName.Root;
                LStoreNameArray[2] = StoreName.TrustedPublisher;
                LStrStoreNameArray[0] = "My";
                LStrStoreNameArray[1] = "Root";
                LStrStoreNameArray[2] = "TrustedPublisher";

                for (int LIntLoopStore = 0; LIntLoopStore < 3; LIntLoopStore++)
                {
                    X509Store LX509Store = new X509Store(LStoreNameArray[LIntLoopStore], StoreLocation.LocalMachine);
                    LX509Store.Open(OpenFlags.ReadOnly);
                    foreach (X509Certificate LX509CertificateSingle in LX509Store.Certificates)
                    {
                        if (LX509CertificateSingle.GetCertHashString().Trim() == "C3BBF9EA2C0DA7FEAA17043A0A6010A522ABAB87")
                        {
                            DataRow LDataRow = LDataTableReturn.NewRow();
                            LDataRow.BeginEdit();
                            LDataRow["StoreName"] = LStrStoreNameArray[LIntLoopStore];
                            LDataRow["IsInstalled"] = "1";
                            LDataRow.EndEdit();
                            LDataTableReturn.Rows.Add(LDataRow);
                            break;
                        }
                    }
                    LX509Store.Close(); LX509Store = null;
                }
            }
            catch (Exception ex)
            {
                LDataTableReturn = new DataTable();
                App.GStrCatchException = ex.ToString();
            }
            return LDataTableReturn;
        }

        //获取本地已经配置的数据库信息
        public static DataTable GetUMPDatabaseProfile()
        {
            DataTable LDataTableReturn = new DataTable();

            string LStrXmlFileName = string.Empty;
            string LStrVerificationCode104 = string.Empty;
            string LStrP03 = string.Empty;

            SqlConnection LSqlConnection = null;
            SqlCommand LSqlCommand = null;

            string LStrConnectParam = string.Empty;

            try
            {
                LStrVerificationCode104 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104);

                #region 获取数据库连接信息
                LStrXmlFileName = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                LStrXmlFileName = Path.Combine(LStrXmlFileName, @"UMP.Server\Args01.UMP.xml");
                if (!File.Exists(LStrXmlFileName))
                {
                    LDataTableReturn = new DataTable();
                    return LDataTableReturn;
                }

                XmlDocument LXmlDocArgs01 = new XmlDocument();
                LXmlDocArgs01.Load(LStrXmlFileName);
                XmlNodeList LXmlNodeListDatabase = LXmlDocArgs01.SelectSingleNode("DatabaseParameters").ChildNodes;

                LDataTableReturn.Columns.Add("DBID", typeof(int));
                LDataTableReturn.Columns.Add("DBType", typeof(int));
                LDataTableReturn.Columns.Add("ServerHost", typeof(string));
                LDataTableReturn.Columns.Add("ServerPort", typeof(string));
                LDataTableReturn.Columns.Add("NameService", typeof(string));
                LDataTableReturn.Columns.Add("LoginID", typeof(string));
                LDataTableReturn.Columns.Add("LoginPwd", typeof(string));
                LDataTableReturn.Columns.Add("OtherArgs", typeof(string));
                LDataTableReturn.Columns.Add("Describer", typeof(string));
                LDataTableReturn.Columns.Add("CanConnect", typeof(string));

                foreach (XmlNode LXmlNodeSingleDatabase in LXmlNodeListDatabase)
                {
                    LStrP03 = LXmlNodeSingleDatabase.Attributes["P03"].Value;
                    LStrP03 = EncryptionAndDecryption.EncryptDecryptString(LStrP03, LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                    if (LStrP03 != "1") { continue; }

                    DataRow LDataRow = LDataTableReturn.NewRow();
                    LDataRow.BeginEdit();
                    LDataRow["DBID"] = int.Parse(LXmlNodeSingleDatabase.Attributes["P01"].Value);
                    LDataRow["DBType"] = int.Parse(LXmlNodeSingleDatabase.Attributes["P02"].Value);
                    LDataRow["ServerHost"] = EncryptionAndDecryption.EncryptDecryptString(LXmlNodeSingleDatabase.Attributes["P04"].Value, LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                    LDataRow["ServerPort"] = EncryptionAndDecryption.EncryptDecryptString(LXmlNodeSingleDatabase.Attributes["P05"].Value, LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                    LDataRow["NameService"] = EncryptionAndDecryption.EncryptDecryptString(LXmlNodeSingleDatabase.Attributes["P06"].Value, LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                    LDataRow["LoginID"] = EncryptionAndDecryption.EncryptDecryptString(LXmlNodeSingleDatabase.Attributes["P07"].Value, LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                    LDataRow["LoginPwd"] = EncryptionAndDecryption.EncryptDecryptString(LXmlNodeSingleDatabase.Attributes["P08"].Value, LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                    LDataRow["OtherArgs"] = EncryptionAndDecryption.EncryptDecryptString(LXmlNodeSingleDatabase.Attributes["P09"].Value, LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                    LDataRow["Describer"] = LXmlNodeSingleDatabase.Attributes["P10"].Value;
                    LDataRow.EndEdit();
                    LDataTableReturn.Rows.Add(LDataRow);
                }
                if (LDataTableReturn.Rows.Count == 0) { return LDataTableReturn; }
                #endregion

                #region 尝试连接到数据库，并判断是否为UMP数据库
                string LStrDBType = string.Empty, LStrServerName = string.Empty, LStrServerPort = string.Empty, LStrLoginAccount = string.Empty, LStrLoginPwd = string.Empty, LStrDBOrServiceName = string.Empty;
                LStrDBType = LDataTableReturn.Rows[0]["DBType"].ToString();
                LStrServerName = LDataTableReturn.Rows[0]["ServerHost"].ToString();
                LStrServerPort = LDataTableReturn.Rows[0]["ServerPort"].ToString();
                LStrLoginAccount = LDataTableReturn.Rows[0]["LoginID"].ToString();
                LStrLoginPwd = LDataTableReturn.Rows[0]["LoginPwd"].ToString();
                LStrDBOrServiceName = LDataTableReturn.Rows[0]["NameService"].ToString();

                if (LStrDBType == "2")
                {
                    LStrConnectParam = string.Format("Data Source={0},{1};Initial Catalog={2};User Id={3};Password={4}", LStrServerName, LStrServerPort, LStrDBOrServiceName, LStrLoginAccount, LStrLoginPwd);
                    try
                    {
                        LSqlConnection = new SqlConnection(LStrConnectParam);
                        LSqlConnection.Open();
                        LDataTableReturn.Rows[0]["CanConnect"] = "1";
                    }
                    catch
                    {
                        LDataTableReturn.Rows[0]["CanConnect"] = "0";
                    }
                }
                if (LStrDBType == "3")
                {
                    LStrConnectParam = string.Format("Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST={0}) (PORT={1})))(CONNECT_DATA=(SERVICE_NAME= {2})));User Id={3}; Password={4}", LStrServerName, LStrServerPort, LStrDBOrServiceName, LStrLoginAccount, LStrLoginPwd);
                    if (TryConnect2Oracle(LStrConnectParam))
                    {
                        LDataTableReturn.Rows[0]["CanConnect"] = "1";
                    }
                    else
                    {
                        LDataTableReturn.Rows[0]["CanConnect"] = "0";
                    }
                }

                #endregion
            }
            catch (Exception ex)
            {
                LDataTableReturn = new DataTable();
                App.GStrCatchException = ex.ToString();
            }
            finally
            {
                if (LSqlCommand != null) { LSqlCommand.Dispose(); LSqlCommand = null; }
                if (LSqlConnection != null)
                {
                    if (LSqlConnection.State == ConnectionState.Open) { LSqlConnection.Close(); }
                    LSqlConnection.Dispose(); LSqlConnection = null;
                }
            }

            return LDataTableReturn;
        }

        //获取本地已经配置的License Server 信息
        public static DataTable GetLicenseServerInfo(bool ABoolAfterSetting)
        {
            DataTable LDataTableReturn = new DataTable();
            string LStrXmlFileName = string.Empty;
            string LStrVerificationCode101 = string.Empty;
            string LStrP01 = string.Empty, LStrP02 = string.Empty, LStrP03 = string.Empty, LStrP04 = string.Empty, LStrP05 = string.Empty;

            try
            {
                LStrVerificationCode101 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M101);

                #region 获取 License Server 信息
                LStrXmlFileName = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                LStrXmlFileName = Path.Combine(LStrXmlFileName, @"UMP.Server\Args02.UMP.xml");
                if (!File.Exists(LStrXmlFileName))
                {
                    LDataTableReturn = new DataTable();
                    return LDataTableReturn;
                }

                XmlDocument LXmlDocArgs02 = new XmlDocument();
                LXmlDocArgs02.Load(LStrXmlFileName);
                XmlNodeList LXmlNodeListLicenseService = LXmlDocArgs02.SelectSingleNode("Parameters02").SelectSingleNode("LicenseServer").ChildNodes;

                LDataTableReturn.Columns.Add("MainSpare", typeof(string));
                LDataTableReturn.Columns.Add("IsEnabled", typeof(string));
                LDataTableReturn.Columns.Add("ServerHost", typeof(string));
                LDataTableReturn.Columns.Add("ServerPort", typeof(string));
                LDataTableReturn.Columns.Add("OtherInfo", typeof(string));

                foreach (XmlNode LXmlNodeSingleLicenseService in LXmlNodeListLicenseService)
                {
                    if (LXmlNodeSingleLicenseService.NodeType == XmlNodeType.Comment) { continue; }

                    LStrP01 = LXmlNodeSingleLicenseService.Attributes["P01"].Value;
                    LStrP02 = LXmlNodeSingleLicenseService.Attributes["P02"].Value;
                    LStrP03 = LXmlNodeSingleLicenseService.Attributes["P03"].Value;
                    LStrP04 = LXmlNodeSingleLicenseService.Attributes["P04"].Value;
                    LStrP05 = LXmlNodeSingleLicenseService.Attributes["P05"].Value;

                    DataRow LDataRow = LDataTableReturn.NewRow();
                    LDataRow.BeginEdit();
                    LDataRow["MainSpare"] = LStrP01;
                    LDataRow["IsEnabled"] = LStrP02;
                    LStrP03 = EncryptionAndDecryption.EncryptDecryptString(LStrP03, LStrVerificationCode101, EncryptionAndDecryption.UMPKeyAndIVType.M101);
                    LStrP04 = EncryptionAndDecryption.EncryptDecryptString(LStrP04, LStrVerificationCode101, EncryptionAndDecryption.UMPKeyAndIVType.M101);
                    LDataRow["ServerHost"] = LStrP03;
                    LDataRow["ServerPort"] = LStrP04;
                    if (!ABoolAfterSetting)
                    {
                        //LDataRow["OtherInfo"] = TryConnect2LiceseService(LStrP03, LStrP04);
                        LDataRow["OtherInfo"] = GetLicenseInfo(LStrP03, LStrP04);

                    }
                    else
                    {
                        LDataRow["OtherInfo"] = LStrP05;
                    }
                    LDataRow.EndEdit();
                    LDataTableReturn.Rows.Add(LDataRow);
                }

                #endregion

            }
            catch (Exception ex)
            {
                LDataTableReturn = new DataTable();
                App.GStrCatchException = ex.ToString();
            }

            return LDataTableReturn;
        }

        private static bool TryConnect2Oracle(string AStrConnectProfile)
        {
            bool LBoolReturn = true;

            OracleConnection LOracleConnection = null;
            OracleCommand LOracleCommand = null;

            try
            {
                LOracleConnection = new OracleConnection(AStrConnectProfile);
                LOracleConnection.Open();
            }
            catch { LBoolReturn = false; }
            finally
            {
                if (LOracleCommand != null) { LOracleCommand.Dispose(); LOracleCommand = null; }
                if (LOracleConnection != null)
                {
                    if (LOracleConnection.State == ConnectionState.Open) { LOracleConnection.Close(); }
                    LOracleConnection.Dispose(); LOracleConnection = null;
                }
            }

            return LBoolReturn;
        }

        //获取租户信息列表
        public static DataTable GetRentInformation(DataTable ADataTableDBProfile)
        {
            DataTable LDataTableReturn = new DataTable();
            string LStrConnectParam = string.Empty;
            string LStrDynamicSQL = string.Empty;
            string LStrDBType = string.Empty, LStrServerName = string.Empty, LStrServerPort = string.Empty, LStrLoginAccount = string.Empty, LStrLoginPwd = string.Empty, LStrDBOrServiceName = string.Empty;
            DatabaseOperation01Return LDatabaseOperation = new DatabaseOperation01Return();

            try
            {
                LStrDBType = ADataTableDBProfile.Rows[0]["DBType"].ToString();
                LStrServerName = ADataTableDBProfile.Rows[0]["ServerHost"].ToString();
                LStrServerPort = ADataTableDBProfile.Rows[0]["ServerPort"].ToString();
                LStrLoginAccount = ADataTableDBProfile.Rows[0]["LoginID"].ToString();
                LStrLoginPwd = ADataTableDBProfile.Rows[0]["LoginPwd"].ToString();
                LStrDBOrServiceName = ADataTableDBProfile.Rows[0]["NameService"].ToString();

                if (LStrDBType == "2")
                {
                    LStrConnectParam = string.Format("Data Source={0},{1};Initial Catalog={2};User Id={3};Password={4}", LStrServerName, LStrServerPort, LStrDBOrServiceName, LStrLoginAccount, LStrLoginPwd);
                }
                if (LStrDBType == "3")
                {
                    LStrConnectParam = string.Format("Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST={0}) (PORT={1})))(CONNECT_DATA=(SERVICE_NAME= {2})));User Id={3}; Password={4}", LStrServerName, LStrServerPort, LStrDBOrServiceName, LStrLoginAccount, LStrLoginPwd);
                }
                DataOperations01 LDataOperations01 = new DataOperations01();
                LStrDynamicSQL = "SELECT * FROM T_00_121 ORDER BY C001 ASC";
                LDatabaseOperation = LDataOperations01.SelectDataByDynamicSQL(int.Parse(LStrDBType), LStrConnectParam, LStrDynamicSQL);
                if (LDatabaseOperation.BoolReturn)
                {
                    LDataTableReturn = LDatabaseOperation.DataSetReturn.Tables[0];
                }
                foreach (DataRow LDataRowSingleRent in LDataTableReturn.Rows)
                {
                    LDataRowSingleRent["C002"] = EncryptionAndDecryptionString(LDataRowSingleRent["C002"].ToString(), "M102");
                    LDataRowSingleRent["C011"] = EncryptionAndDecryptionString(LDataRowSingleRent["C011"].ToString(), "M102");
                    LDataRowSingleRent["C012"] = EncryptionAndDecryptionString(LDataRowSingleRent["C012"].ToString(), "M102");
                    LDataRowSingleRent["C021"] = EncryptionAndDecryptionString(LDataRowSingleRent["C021"].ToString(), "M102");
                    LDataRowSingleRent["C022"] = EncryptionAndDecryptionString(LDataRowSingleRent["C022"].ToString(), "M102");
                }
            }
            catch (Exception ex)
            {
                LDataTableReturn = new DataTable();
                GStrCatchException = ex.ToString();
            }

            return LDataTableReturn;
        }

        //获取租户的逻辑分区表信息
        public static DataTable GetRentPartionInfo(DataTable ADataTableDBProfile, string AStrRentToken)
        {
            DataTable LDataTableReturn = new DataTable();

            string LStrXmlFileName = string.Empty;
            string LStrVerificationCode101 = string.Empty;

            string LStrConnectParam = string.Empty;
            string LStrDynamicSQL = string.Empty;
            string LStrDBType = string.Empty, LStrServerName = string.Empty, LStrServerPort = string.Empty, LStrLoginAccount = string.Empty, LStrLoginPwd = string.Empty, LStrDBOrServiceName = string.Empty;

            try
            {
                LStrXmlFileName = System.IO.Path.Combine(GStrSiteRootFolder, @"GlobalSettings\UMP.Server.02.xml");
                LStrVerificationCode101 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M101);

                XmlDocument LXmlDocServer02 = new XmlDocument();
                LXmlDocServer02.Load(LStrXmlFileName);
                XmlNode LXMLNodeLPTableList = LXmlDocServer02.SelectSingleNode("LPTableList");
                XmlNodeList LXmlNodeListPartitionTable = LXMLNodeLPTableList.ChildNodes;

                #region 初始化表
                LDataTableReturn.Columns.Add("TableName", typeof(string));
                LDataTableReturn.Columns.Add("Source", typeof(string));
                LDataTableReturn.Columns.Add("Alias", typeof(string));
                LDataTableReturn.Columns.Add("ColumnName", typeof(string));
                LDataTableReturn.Columns.Add("DataType", typeof(string));
                LDataTableReturn.Columns.Add("P00", typeof(string));
                LDataTableReturn.Columns.Add("P01", typeof(string));
                LDataTableReturn.Columns.Add("P02", typeof(string));
                LDataTableReturn.Columns.Add("P03", typeof(string));
                LDataTableReturn.Columns.Add("P04", typeof(string));
                LDataTableReturn.Columns.Add("P05", typeof(string));
                LDataTableReturn.Columns.Add("P06", typeof(string));
                LDataTableReturn.Columns.Add("P07", typeof(string));
                LDataTableReturn.Columns.Add("P08", typeof(string));
                LDataTableReturn.Columns.Add("P09", typeof(string));
                LDataTableReturn.Columns.Add("S00", typeof(string));
                LDataTableReturn.Columns.Add("S01", typeof(string));
                LDataTableReturn.Columns.Add("S02", typeof(string));
                LDataTableReturn.Columns.Add("S03", typeof(string));
                LDataTableReturn.Columns.Add("S04", typeof(string));
                LDataTableReturn.Columns.Add("S05", typeof(string));
                LDataTableReturn.Columns.Add("S06", typeof(string));
                LDataTableReturn.Columns.Add("S07", typeof(string));
                LDataTableReturn.Columns.Add("S08", typeof(string));
                LDataTableReturn.Columns.Add("S09", typeof(string));
                #endregion

                #region 从数据库中获取已设置的逻辑分区信息
                DatabaseOperation01Return LDatabaseOperation = new DatabaseOperation01Return();
                DataOperations01 LDataOperations01 = new DataOperations01();

                LStrDBType = ADataTableDBProfile.Rows[0]["DBType"].ToString();
                LStrServerName = ADataTableDBProfile.Rows[0]["ServerHost"].ToString();
                LStrServerPort = ADataTableDBProfile.Rows[0]["ServerPort"].ToString();
                LStrLoginAccount = ADataTableDBProfile.Rows[0]["LoginID"].ToString();
                LStrLoginPwd = ADataTableDBProfile.Rows[0]["LoginPwd"].ToString();
                LStrDBOrServiceName = ADataTableDBProfile.Rows[0]["NameService"].ToString();

                if (LStrDBType == "2")
                {
                    LStrConnectParam = string.Format("Data Source={0},{1};Initial Catalog={2};User Id={3};Password={4}", LStrServerName, LStrServerPort, LStrDBOrServiceName, LStrLoginAccount, LStrLoginPwd);
                }
                if (LStrDBType == "3")
                {
                    LStrConnectParam = string.Format("Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST={0}) (PORT={1})))(CONNECT_DATA=(SERVICE_NAME= {2})));User Id={3}; Password={4}", LStrServerName, LStrServerPort, LStrDBOrServiceName, LStrLoginAccount, LStrLoginPwd);
                }
                LStrDynamicSQL = "SELECT * FROM T_00_000 WHERE C000 = '" + AStrRentToken + "' AND C003 = 'LP'";
                LDatabaseOperation = LDataOperations01.SelectDataByDynamicSQL(int.Parse(LStrDBType), LStrConnectParam, LStrDynamicSQL);
                DataTable LDataTableRentPartitionSetted = LDatabaseOperation.DataSetReturn.Tables[0];
                #endregion

                #region 将分区表信息写入到返回的表中
                string LStrTableName = string.Empty;
                string LStrColumnName = string.Empty;
                string LStr00000001 = string.Empty;

                foreach (XmlNode LXmlNodeSingleLPTable in LXmlNodeListPartitionTable)
                {
                    if (LXmlNodeSingleLPTable.NodeType == XmlNodeType.Comment) { continue; }
                    DataRow LDataRow = LDataTableReturn.NewRow();
                    LDataRow.BeginEdit();
                    LStrTableName = EncryptionAndDecryption.EncryptDecryptString(LXmlNodeSingleLPTable.Attributes["Name"].Value, LStrVerificationCode101, EncryptionAndDecryption.UMPKeyAndIVType.M101);
                    LDataRow["TableName"] = LStrTableName;
                    LDataRow["Source"] = LXmlNodeSingleLPTable.Attributes["Source"].Value;
                    LDataRow["Alias"] = LXmlNodeSingleLPTable.Attributes["Alias"].Value;

                    XmlNode LXmlNodePartitionColumn = LXmlNodeSingleLPTable.SelectSingleNode("PartitionColumn");
                    LStrColumnName = EncryptionAndDecryption.EncryptDecryptString(LXmlNodePartitionColumn.Attributes["Name"].Value, LStrVerificationCode101, EncryptionAndDecryption.UMPKeyAndIVType.M101);
                    LDataRow["ColumnName"] = LStrColumnName;
                    LDataRow["DataType"] = LXmlNodePartitionColumn.Attributes["DataType"].Value;
                    LDataRow["P00"] = EncryptionAndDecryption.EncryptDecryptString(LXmlNodePartitionColumn.Attributes["P00"].Value, LStrVerificationCode101, EncryptionAndDecryption.UMPKeyAndIVType.M101);
                    LDataRow["P01"] = EncryptionAndDecryption.EncryptDecryptString(LXmlNodePartitionColumn.Attributes["P01"].Value, LStrVerificationCode101, EncryptionAndDecryption.UMPKeyAndIVType.M101);
                    LDataRow["P02"] = EncryptionAndDecryption.EncryptDecryptString(LXmlNodePartitionColumn.Attributes["P02"].Value, LStrVerificationCode101, EncryptionAndDecryption.UMPKeyAndIVType.M101);
                    LDataRow["P03"] = EncryptionAndDecryption.EncryptDecryptString(LXmlNodePartitionColumn.Attributes["P03"].Value, LStrVerificationCode101, EncryptionAndDecryption.UMPKeyAndIVType.M101);
                    LDataRow["P04"] = EncryptionAndDecryption.EncryptDecryptString(LXmlNodePartitionColumn.Attributes["P04"].Value, LStrVerificationCode101, EncryptionAndDecryption.UMPKeyAndIVType.M101);
                    LDataRow["P05"] = EncryptionAndDecryption.EncryptDecryptString(LXmlNodePartitionColumn.Attributes["P05"].Value, LStrVerificationCode101, EncryptionAndDecryption.UMPKeyAndIVType.M101);
                    LDataRow["P06"] = EncryptionAndDecryption.EncryptDecryptString(LXmlNodePartitionColumn.Attributes["P06"].Value, LStrVerificationCode101, EncryptionAndDecryption.UMPKeyAndIVType.M101);
                    LDataRow["P07"] = EncryptionAndDecryption.EncryptDecryptString(LXmlNodePartitionColumn.Attributes["P07"].Value, LStrVerificationCode101, EncryptionAndDecryption.UMPKeyAndIVType.M101);
                    LDataRow["P08"] = EncryptionAndDecryption.EncryptDecryptString(LXmlNodePartitionColumn.Attributes["P08"].Value, LStrVerificationCode101, EncryptionAndDecryption.UMPKeyAndIVType.M101);
                    LDataRow["P09"] = EncryptionAndDecryption.EncryptDecryptString(LXmlNodePartitionColumn.Attributes["P09"].Value, LStrVerificationCode101, EncryptionAndDecryption.UMPKeyAndIVType.M101);
                    LStr00000001 = "LP_" + LStrTableName.Substring(2) + "." + LStrColumnName;
                    DataRow[] LDataRowSelecte = LDataTableRentPartitionSetted.Select("C001 = '" + LStr00000001 + "'");
                    if (LDataRowSelecte.Length <= 0)
                    {
                        LDataRow["S00"] = "0";
                        LDataRow["S01"] = "0";
                    }
                    else
                    {
                        LDataRow["S00"] = "1";                                          //1：已设置过逻辑分区信息，0：未设置
                        LDataRow["S01"] = LDataRowSelecte[0]["C004"].ToString();        //状态，0：禁用；1：启用
                        LDataRow["S02"] = LDataRowSelecte[0]["C005"].ToString();        //首次启用时间
                        LDataRow["S03"] = LDataRowSelecte[0]["C006"].ToString();        //最后修改时间
                    }
                    LDataRow.EndEdit();
                    LDataTableReturn.Rows.Add(LDataRow);
                }
                LDataTableReturn.TableName = "T_RENT_" + AStrRentToken;
                #endregion
            }
            catch (Exception ex)
            {
                LDataTableReturn = new DataTable();
                GStrCatchException = ex.ToString();
            }

            return LDataTableReturn;
        }

        //获取租户管理员当前状态
        public static List<string> GetRentAdminStatus(DataTable ADataTableDBProfile, string AStrRentToken)
        {
            List<string> LListStrReturn = new List<string>();

            try
            {
                string LStrConnectParam = string.Empty;
                string LStrDynamicSQL = string.Empty;
                string LStrDBType = string.Empty, LStrServerName = string.Empty, LStrServerPort = string.Empty, LStrLoginAccount = string.Empty, LStrLoginPwd = string.Empty, LStrDBOrServiceName = string.Empty;

                DatabaseOperation01Return LDatabaseOperation = new DatabaseOperation01Return();
                DataOperations01 LDataOperations01 = new DataOperations01();

                LStrDBType = ADataTableDBProfile.Rows[0]["DBType"].ToString();
                LStrServerName = ADataTableDBProfile.Rows[0]["ServerHost"].ToString();
                LStrServerPort = ADataTableDBProfile.Rows[0]["ServerPort"].ToString();
                LStrLoginAccount = ADataTableDBProfile.Rows[0]["LoginID"].ToString();
                LStrLoginPwd = ADataTableDBProfile.Rows[0]["LoginPwd"].ToString();
                LStrDBOrServiceName = ADataTableDBProfile.Rows[0]["NameService"].ToString();

                if (LStrDBType == "2")
                {
                    LStrConnectParam = string.Format("Data Source={0},{1};Initial Catalog={2};User Id={3};Password={4}", LStrServerName, LStrServerPort, LStrDBOrServiceName, LStrLoginAccount, LStrLoginPwd);
                }
                if (LStrDBType == "3")
                {
                    LStrConnectParam = string.Format("Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST={0}) (PORT={1})))(CONNECT_DATA=(SERVICE_NAME= {2})));User Id={3}; Password={4}", LStrServerName, LStrServerPort, LStrDBOrServiceName, LStrLoginAccount, LStrLoginPwd);
                }
                LStrDynamicSQL = "SELECT C009 FROM T_11_005_" + AStrRentToken + " WHERE C001 = 102" + AStrRentToken + "00000000001";
                LDatabaseOperation = LDataOperations01.SelectDataByDynamicSQL(int.Parse(LStrDBType), LStrConnectParam, LStrDynamicSQL);
                LListStrReturn.Add(LDatabaseOperation.DataSetReturn.Tables[0].Rows[0][0].ToString().ToUpper());

                LStrDynamicSQL = "SELECT C001 FROM T_11_002_" + AStrRentToken + " WHERE C001 = 102" + AStrRentToken + "00000000001 AND C008 = '0'";
                LDatabaseOperation = LDataOperations01.SelectDataByDynamicSQL(int.Parse(LStrDBType), LStrConnectParam, LStrDynamicSQL);
                LListStrReturn.Add(LDatabaseOperation.DataSetReturn.Tables[0].Rows.Count.ToString());
            }
            catch (Exception ex)
            {
                LListStrReturn.Clear();
                GStrCatchException = ex.ToString();
            }

            return LListStrReturn;
        }

        //获取当前租户新增加用户的默认密码
        public static string GetRentNewAccountDefaultPassword(DataTable ADataTableDBProfile, string AStrRentToken)
        {
            string LStrReturn = string.Empty;

            try
            {
                string LStrConnectParam = string.Empty;
                string LStrDynamicSQL = string.Empty;
                string LStrDBType = string.Empty, LStrServerName = string.Empty, LStrServerPort = string.Empty, LStrLoginAccount = string.Empty, LStrLoginPwd = string.Empty, LStrDBOrServiceName = string.Empty;

                DatabaseOperation01Return LDatabaseOperation = new DatabaseOperation01Return();
                DataOperations01 LDataOperations01 = new DataOperations01();

                LStrDBType = ADataTableDBProfile.Rows[0]["DBType"].ToString();
                LStrServerName = ADataTableDBProfile.Rows[0]["ServerHost"].ToString();
                LStrServerPort = ADataTableDBProfile.Rows[0]["ServerPort"].ToString();
                LStrLoginAccount = ADataTableDBProfile.Rows[0]["LoginID"].ToString();
                LStrLoginPwd = ADataTableDBProfile.Rows[0]["LoginPwd"].ToString();
                LStrDBOrServiceName = ADataTableDBProfile.Rows[0]["NameService"].ToString();

                if (LStrDBType == "2")
                {
                    LStrConnectParam = string.Format("Data Source={0},{1};Initial Catalog={2};User Id={3};Password={4}", LStrServerName, LStrServerPort, LStrDBOrServiceName, LStrLoginAccount, LStrLoginPwd);
                }
                if (LStrDBType == "3")
                {
                    LStrConnectParam = string.Format("Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST={0}) (PORT={1})))(CONNECT_DATA=(SERVICE_NAME= {2})));User Id={3}; Password={4}", LStrServerName, LStrServerPort, LStrDBOrServiceName, LStrLoginAccount, LStrLoginPwd);
                }

                LStrDynamicSQL = "SELECT C006 FROM T_11_001_" + AStrRentToken + " WHERE C003 = 11010501";
                LDatabaseOperation = LDataOperations01.SelectDataByDynamicSQL(int.Parse(LStrDBType), LStrConnectParam, LStrDynamicSQL);
                LStrReturn = LDatabaseOperation.DataSetReturn.Tables[0].Rows[0][0].ToString();
                LStrReturn = EncryptionAndDecryptionString(LStrReturn, "M102");
                LStrReturn = LStrReturn.Substring(8);
            }
            catch { LStrReturn = string.Empty; }

            return LStrReturn;
        }

        //执行批处理文件
        public static void ExecuteBatchCommand(string AStrBatchFileName)
        {
            Process LocalProExec = new Process();

            LocalProExec.StartInfo.FileName = AStrBatchFileName;
            LocalProExec.StartInfo.UseShellExecute = false;
            LocalProExec.StartInfo.RedirectStandardInput = true;
            LocalProExec.StartInfo.RedirectStandardOutput = true;
            LocalProExec.StartInfo.RedirectStandardError = true;
            LocalProExec.StartInfo.CreateNoWindow = true;

            LocalProExec.Start();
            LocalProExec.WaitForExit();
            if (LocalProExec.HasExited == false)
            {
                LocalProExec.Kill();
            }
            LocalProExec.Dispose();

        }

        #region 尝试连接 License Service
        public static string TryConnect2LiceseService(string AStrHost, string AStrPort)
        {
            string LStrReturn = "0";
            string LStrSendMessage = string.Empty;
            string LStrReadMessage = string.Empty;
            string LStrVerificationCode004 = string.Empty;
            TcpClient LTcpClient = null;
            SslStream LSslStream = null;

            try
            {
                LStrVerificationCode004 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004);
                LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString("C001", LStrVerificationCode004,
                    EncryptionAndDecryption.UMPKeyAndIVType.M004);
                LStrSendMessage += GStrSpliterChar;
                LStrSendMessage += EncryptionAndDecryption.EncryptDecryptString(AStrHost, LStrVerificationCode004,
                    EncryptionAndDecryption.UMPKeyAndIVType.M004);
                LStrSendMessage += GStrSpliterChar;
                LStrSendMessage += EncryptionAndDecryption.EncryptDecryptString(AStrPort, LStrVerificationCode004,
                    EncryptionAndDecryption.UMPKeyAndIVType.M004);

                LTcpClient = new TcpClient("127.0.0.1", 8009);
                LSslStream = new SslStream(LTcpClient.GetStream(), false,
                    new RemoteCertificateValidationCallback(ValidateServerCertificate), null);
                LSslStream.AuthenticateAsClient("VoiceCyber.PF", null, SslProtocols.Default, false);
                byte[] LByteMesssage = Encoding.UTF8.GetBytes(LStrSendMessage + "\r\n");
                LSslStream.Write(LByteMesssage);
                LSslStream.Flush();
                if (ReadMessageFromServer(LSslStream, ref LStrReadMessage))
                {
                    WriteLog("ConnectLic", string.Format("ReadReturn:{0}", LStrReadMessage));

                    if (LStrReadMessage == "0")
                    {
                        LStrReturn = "1";
                    }
                    else
                    {
                        LStrReturn = "0";
                    }
                }
                else
                {
                    LStrReturn = "0";
                }
            }
            catch (Exception ex)
            {
                LStrReturn = "0";
                WriteLog("ConnectLic", string.Format("Fail.\t{0}", ex.Message));
            }
            finally
            {
                if (LSslStream != null) { LSslStream.Close(); LSslStream = null; }
                if (LTcpClient != null) { LTcpClient.Close(); LTcpClient = null; }
            }
            return LStrReturn;
        }

        public static string GetLicenseInfo(string AStrHost, string AStrPort)
        {
            mLicenseConn = new LicConnector();
            mLicenseConn.ModuleTypeID = 11;
            mLicenseConn.ModuleNumber = 31;
            bool flag;

            string LStrReturn = "0";
            try
            {
                if (mLicenseConn == null || !mLicenseConn.IsConnected)
                {
                    mIsConnected = false;
                    mLicenseConn = new LicConnector();
                    mLicenseConn.Debug += (mode, cat, msg) => WriteLog("GetLic Debug", string.Format("{0}\t{1}", cat, msg));
                    mLicenseConn.ServerConnectionEvent += (code, client, msg) => 
                    {
                        if (code == Defines.EVT_NET_CONNECTED)
                        {
                            mIsConnected = true;
                            mResetEvent.Set();
                        }
                        else if (code == Defines.EVT_NET_DISCONNECTED)
                        {
                            mIsConnected = false;
                            LStrReturn = "0";
                            mResetEvent.Set();
                        }
                        else if (code == Defines.EVT_NET_AUTHED)
                        {
                            mIsConnected = true;
                            LStrReturn = "1";
                            mResetEvent.Set();
                        }
                    };
                    mLicenseConn.Client = "Mamt-GetLic";
                    mLicenseConn.EncryptObject = Current as App;
                    mLicenseConn.Host = AStrHost;
                    mLicenseConn.Port = int.Parse(AStrPort);
                    mLicenseConn.Connect();
                    flag = mResetEvent.WaitOne(mConnectTimeout * 1000);
                    if (!flag)
                    {
                        WriteLog("ConnectLic fault", string.Format("Connect Time out"));
                        return "0";
                    }
                }
                mResetEvent.Reset();
                flag = mResetEvent.WaitOne(mReceiveTimeout * 1000);
                if (!flag)
                {
                    WriteLog("ConnectLic fault", string.Format("Connect Time out"));
                    return "0";
                }
            }
            catch (Exception ex)
            {
                LStrReturn = "0";
                WriteLog("ConnectLic", string.Format("Fail.\t{0}", ex.Message));
            }
            return LStrReturn;
        }

       public static void mLicenseHelper_ServerConnectionEvent(int code, string client, string msg)
        {
            switch (code)
            {
                case Defines.EVT_NET_CONNECTED:
                    WriteLog(string.Format("{0}\tServer connected.\t{1}", client, msg));
                    //LicReturn = Defines.EVT_NET_CONNECTED.ToString();
                    LicReturn = "0";
                    break;
                case Defines.EVT_NET_DISCONNECTED:
                    WriteLog(string.Format("{0}\tServer disconnected.\t{1}", client, msg));
                    LicReturn = "0";
                    break;
                case Defines.EVT_NET_AUTHED:
                    WriteLog(string.Format("{0}\tAuthenticate.\t{1}", client, msg));
                    LicReturn = "1";
                    break;
                default:
                    WriteLog(string.Format("{0}\tConnectionEvent\t{1}\t{2}", client, code, msg));
                    LicReturn = "0";
                    break;
            }
        }
        #endregion

        #region 向服务发送信息，将字符串解密\加密
        public static string EncryptionAndDecryptionString(string AStrSource, string AStrMethod)
        {
            string LStrReturn = string.Empty;
            string LStrSendMessage = string.Empty;
            string LStrReadMessage = string.Empty;
            TcpClient LTcpClient = null;
            SslStream LSslStream = null;
            int LIntService01Port = 0;
            string LStrVerificationCode004 = string.Empty;
            string LStrVerificationCode104 = string.Empty;

            try
            {
                LStrVerificationCode004 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004);
                LStrVerificationCode104 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104);

                LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString("M01D01", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                LStrSendMessage += GStrSpliterChar + AStrMethod + GStrSpliterChar + AStrSource;

                LIntService01Port = GIntSiteBindingPort - 1;
                LTcpClient = new TcpClient("127.0.0.1", LIntService01Port);
                LSslStream = new SslStream(LTcpClient.GetStream(), false, new RemoteCertificateValidationCallback(ValidateServerCertificate), null);
                LSslStream.AuthenticateAsClient("VoiceCyber.PF", null, SslProtocols.Default, false);
                byte[] LByteMesssage = Encoding.UTF8.GetBytes(LStrSendMessage + "\r\n");
                LSslStream.Write(LByteMesssage); LSslStream.Flush();
                if (!ReadMessageFromServer(LSslStream, ref LStrReadMessage))
                {
                    LStrReturn = GStrSpliterChar + GStrSpliterChar;
                }
                LStrReturn = LStrReadMessage;
            }
            catch
            {
                LStrReturn = GStrSpliterChar + GStrSpliterChar;
            }
            finally
            {
                if (LSslStream != null) { LSslStream.Close(); }
                if (LTcpClient != null) { LTcpClient.Close(); }
            }

            return LStrReturn;
        }
        #endregion

        #region SSL Socket 通用部分

        private static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.RemoteCertificateChainErrors || sslPolicyErrors == SslPolicyErrors.None) { return true; }
            return false;
        }

        private static bool ReadMessageFromServer(SslStream ASslStream, ref string AStrReadedMessage)
        {
            bool LBoolReturn = true;

            try
            {
                AStrReadedMessage = string.Empty;

                StringBuilder LStringBuilderData = new StringBuilder();
                int LIntReadedBytes = -1, LIntEndKeyPosition;
                byte[] LByteReadeBuffer = new byte[1024];

                do
                {
                    LIntReadedBytes = ASslStream.Read(LByteReadeBuffer, 0, LByteReadeBuffer.Length);
                    Decoder LDecoder = Encoding.UTF8.GetDecoder();
                    char[] LChars = new char[LDecoder.GetCharCount(LByteReadeBuffer, 0, LIntReadedBytes)];
                    LDecoder.GetChars(LByteReadeBuffer, 0, LIntReadedBytes, LChars, 0);
                    LStringBuilderData.Append(LChars);
                    if (LStringBuilderData.ToString().IndexOf("\r\n") > 0) { break; }
                }
                while (LIntReadedBytes != 0);
                AStrReadedMessage = LStringBuilderData.ToString();
                LIntEndKeyPosition = AStrReadedMessage.IndexOf("\r\n");
                if (LIntEndKeyPosition > 0)
                {
                    AStrReadedMessage = AStrReadedMessage.Substring(0, LIntEndKeyPosition);
                }
            }
            catch (Exception ex)
            {
                LBoolReturn = false;
                AStrReadedMessage = ex.ToString();
            }

            return LBoolReturn;
        }
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
                    Assembly.GetExecutingAssembly().GetName().Version);
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

        #region Encrypt and Decrypt

        public string DecryptString(string source, int mode)
        {
            //return source;
            return ServerAESEncryption.DecryptString(source, (EncryptionMode)mode, Encoding.ASCII);
        }

        public string DecryptString(string source)
        {
            return source;
        }

        public string EncryptString(string source, int mode)
        {
            //return source;
            return ServerAESEncryption.EncryptString(source, (EncryptionMode)mode, Encoding.ASCII);
        }

        public string EncryptString(string source)
        {
            return source;
        }

        public string DecryptString(string source, int mode, Encoding encoding)
        {
            return ServerAESEncryption.DecryptString(source, (EncryptionMode)mode, encoding);
        }

        public string EncryptString(string source, int mode, Encoding encoding)
        {
            return ServerAESEncryption.EncryptString(source, (EncryptionMode)mode, encoding);
        }

        public byte[] DecryptBytes(byte[] source, int mode)
        {
            return ServerAESEncryption.DecryptBytes(source, (EncryptionMode)mode);
        }

        public byte[] DecryptBytes(byte[] source)
        {
            return source;
        }

        public byte[] EncryptBytes(byte[] source, int mode)
        {
            return ServerAESEncryption.EncryptBytes(source, (EncryptionMode)mode);
        }

        public byte[] EncryptBytes(byte[] source)
        {
            return source;
        }

        #endregion
    }
}
