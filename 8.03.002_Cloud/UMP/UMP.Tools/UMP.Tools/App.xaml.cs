using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using UMP.Tools.BasicModule;
using UMP.Tools.PublicClasses;

namespace UMP.Tools
{
    public partial class App : Application
    {
        [DllImport("kernel32.dll")]
        public static extern int GetUserDefaultUILanguage();

        public static SystemMainWindow GSystemMainWindow = null;

        #region 全局变量
        //系统简称
        public static string GStrApplicationReferredTo = "UMP iTools";
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
        //public static string GStrSiteRootFolder = string.Empty;
        //系统默认分割符
        public static string GStrSpliterChar = string.Empty;
        //该应用是否连接到UMP应用服务器上运行
        public static bool GBoolConnectRemote = true;
        //服务端是否允许远程连接
        public static string GStrAllowRemoteConnect = string.Empty;
        //当前季节编码 01 - 04
        public static string GStrSeasonCode = string.Empty;
        //公司名称
        public static string GStrCompanyName = string.Empty;
        //当前程序版本
        public static string GStrApplicationVersion = string.Empty;
        //当前运行时的Exception
        public static string GStrCatchException = string.Empty;
        //系统默认的安装证书Hash值
        public static string GStrCertificateHash = "2D508A175B6836ADB6E220BA63E00F2F0881E75F";
        //UMP.PF站点绑定的端口
        public static int GIntSiteBindingPort = 8081;
        //当前登录用户的密码
        public static string GStrLoginPassword = string.Empty;
        //系统是否处于忙状态中
        public static bool GBoolApplicationIsBusing = false;
        #endregion

        #region 系统启动时的语言包、风格
        private static DataTable GDataTableSupportLanguages = null;
        private static DataTable GDataTableSupportStyles = null;
        private static DataTable GDataTableNecessaryLanguages = null;
        private static DataTable GDataTableConvertData = null;

        /// <summary>
        /// 初始化 系统启动时的语言包、风格
        /// </summary>
        public static void InitStartedAppliactionData()
        {
            string LStrXMLFileFolder = string.Empty;
            string LStrXmlFileName = string.Empty;
            string LStrSupportAllLanguagesText = string.Empty;

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
            LStrSupportAllLanguagesText = System.IO.Path.Combine(LStrXMLFileFolder, "SAllLanguages.txt");
            string[] LStrArrayLangInfo = File.ReadAllLines(LStrSupportAllLanguagesText, Encoding.UTF8);
            foreach (string LStrSingleLang in LStrArrayLangInfo)
            {
                string[] LStrArraySingleLanguage = LStrSingleLang.Split('|');
                if (LStrArraySingleLanguage[1] == GStrLoginUserCurrentLanguageID)
                {
                    GStrLoginUserCurrentLanguageID = LStrArraySingleLanguage[0];
                    break;
                }
            }
            LStrXmlFileName = System.IO.Path.Combine(LStrXMLFileFolder, "S" + GStrLoginUserCurrentLanguageID + ".xml");
            if (!File.Exists(LStrXmlFileName))
            {
                LStrXmlFileName = System.IO.Path.Combine(LStrXMLFileFolder, "S2052.xml");
                App.GStrLoginUserCurrentLanguageID = "2052";
            }
            XmlDocument LXmlDocument = new XmlDocument();
            LXmlDocument.Load(LStrXmlFileName);
            #endregion

            #region 读取支持的语言列表
            XmlNode LXMLNodeSupportLanguages = LXmlDocument.SelectSingleNode("UMPTools").SelectSingleNode("SupportLanguages");
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
            XmlNode LXMLNodeSupportStyle = LXmlDocument.SelectSingleNode("UMPTools").SelectSingleNode("SupportStyle");
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
            XmlNode LXMLNodeLanguages = LXmlDocument.SelectSingleNode("UMPTools").SelectSingleNode("Languages");
            XmlNodeList LXmlNodeLanguages = LXMLNodeLanguages.ChildNodes;
            foreach (XmlNode LXmlNodeSingleLanguageItem in LXmlNodeLanguages)
            {
                if (LXmlNodeSingleLanguageItem.NodeType == XmlNodeType.Comment) { continue; }
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
            XmlNode LXMLDataConvert = LXmlDocument.SelectSingleNode("UMPTools").SelectSingleNode("DataConvert");
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
        }
        #endregion

        protected override void OnStartup(StartupEventArgs e)
        {
            CreateSpliterCharater();
            GetApplicationBasicDirectory();
            GetSeasonCode();
            GetUserWindowsDefaultLanguage();
            GetComputerMachineName();
            GetProgramDataDirectory();
            GetUserMyDocumentsDirectory();
            GetNetworkAllIPAddress();
            GetApplicationVersion();
            InitStartedAppliactionData();
            InitGlobalParameterFromLangPackage();

            base.OnStartup(e);
        }

        #region 初始化全局变量
        private void GetApplicationBasicDirectory()
        {
            GStrApplicationDirectory = Environment.CurrentDirectory;
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
            }
            catch { GStrComputerCurrentLanguageID = "2052"; }
            finally
            {
                GStrLoginUserCurrentLanguageID = GStrComputerCurrentLanguageID;
            }
        }

        private void GetComputerMachineName()
        {
            try
            {
                GStrComputerName = Environment.MachineName;
            }
            catch { GStrComputerName = string.Empty; }
        }

        private void GetProgramDataDirectory()
        {
            try
            {
                GStrProgramDataDirectory = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                if (!System.IO.Directory.Exists(System.IO.Path.Combine(App.GStrProgramDataDirectory, "UMP.Client")))
                {
                    System.IO.Directory.CreateDirectory(System.IO.Path.Combine(App.GStrProgramDataDirectory, "UMP.Client"));
                }
                if (!System.IO.Directory.Exists(System.IO.Path.Combine(App.GStrProgramDataDirectory, "UMP.Client\\iTools")))
                {
                    System.IO.Directory.CreateDirectory(System.IO.Path.Combine(App.GStrProgramDataDirectory, "UMP.Client\\iTools"));
                }
            }
            catch { App.GStrProgramDataDirectory = string.Empty; }
        }

        private void GetUserMyDocumentsDirectory()
        {
            try
            {
                GStrUserMyDocumentsDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }
            catch { GStrUserMyDocumentsDirectory = string.Empty; }
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
        }

        private void GetApplicationVersion()
        {
            string LStrVersion = string.Empty;
            LStrVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            string[] LStrArrayVersion = LStrVersion.Split('.');
            GStrApplicationVersion = LStrArrayVersion[0] + "." + int.Parse(LStrArrayVersion[1]).ToString("00") + "." + int.Parse(LStrArrayVersion[2]).ToString("000");
        }

        private void InitGlobalParameterFromLangPackage()
        {
            GStrCompanyName = GetDisplayCharater("M01003");
            GStrApplicationReferredTo = GetDisplayCharater("M01015");
            //GStrAllowRemoteConnect = GetDisplayCharater("M02001");
        }
        #endregion

        #region 加载各种资源
        /// <summary>
        /// 加载系统样式资源
        /// </summary>
        public static void LoadApplicationResources()
        {
            string LStrResourceFile = string.Empty;

            LStrResourceFile = System.IO.Path.Combine(GStrApplicationDirectory, @"Styles\Style" + GStrSeasonCode + ".xaml");
            ResourceDictionary LResourceDictionary = new ResourceDictionary();
            LResourceDictionary.Source = new Uri(LStrResourceFile, UriKind.Absolute);
            Application.Current.Resources.MergedDictionaries.Clear();
            Application.Current.Resources.MergedDictionaries.Add(LResourceDictionary);
        }

        /// <summary>
        /// 画窗口背景图片
        /// </summary>
        /// <param name="AWindowTarget">目标窗口</param>
        public static void DrawWindowsBackGround(Window AWindowTarget)
        {
            DrawingBackground.DrawWindowsBackgond(AWindowTarget);
        }

        #endregion

        #region 初始化主界面下拉菜单
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
                    LContextMenu.Items.Add(LMenuItemSingle);
                }
                #endregion

                LContextMenu.Items.Add(new Separator());

                #region 系统可以选择的样式
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
                    LContextMenu.Items.Add(LMenuItemSingle);
                }
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
                OperationEventArgs LEventArgs = new OperationEventArgs();
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
        #endregion

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

        /// <summary>
        ///显示系统运行过程中异常 
        /// </summary>
        /// <param name="AStrMessage">需要显示的异常消息</param>
        public static void ShowExceptionMessage(string AStrMessage)
        {
            MessageBox.Show(GetDisplayCharater("E00000") + "\n" + AStrMessage, GStrApplicationReferredTo, MessageBoxButton.OK, MessageBoxImage.Error);
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
            OperationEventArgs LEventArgs = new OperationEventArgs();

            LEventArgs.StrElementTag = "SSLC";
            LEventArgs.ObjSource = AIntType.ToString();
            LEventArgs.AppenObjeSource1 = AStrStatusDesc;
            LEventArgs.AppenObjeSource2 = "0";
            GSystemMainWindow.ObjectOperationsEvent(LEventArgs);

            if (AIntType == 1) { GBoolApplicationIsBusing = true; } else { GBoolApplicationIsBusing = false; }
        }

        public static void ShowCurrentStatus(int AIntType, string AStrStatusDesc, bool ABoolWithWait)
        {
            OperationEventArgs LEventArgs = new OperationEventArgs();

            LEventArgs.StrElementTag = "SSLC";
            LEventArgs.ObjSource = AIntType.ToString();
            LEventArgs.AppenObjeSource1 = AStrStatusDesc;
            LEventArgs.AppenObjeSource2 = "1";
            GSystemMainWindow.ObjectOperationsEvent(LEventArgs);

            if (AIntType == 1) { GBoolApplicationIsBusing = true; } else { GBoolApplicationIsBusing = false; }
        }

        #endregion

        #region 创建WCF连接信息
        public static BasicHttpBinding CreateBasicHttpBinding(bool ABoolIsHttps, int AIntSendTimeOut)
        {
            BasicHttpBinding LBasicHttpBinding = new BasicHttpBinding();

            TimeSpan LTimeSpan;
            if (AIntSendTimeOut == 0)
            {
                LTimeSpan = new TimeSpan(0, 5, 0);
            }
            else
            {
                LTimeSpan = new TimeSpan(AIntSendTimeOut * 10000000);
            }

            BasicHttpSecurity LBasicHttpSecurity = new BasicHttpSecurity();
            if (ABoolIsHttps)
            {
                LBasicHttpSecurity.Mode = BasicHttpSecurityMode.Transport;
                LBasicHttpSecurity.Transport.ClientCredentialType = HttpClientCredentialType.None;
            }
            else
            {
                LBasicHttpSecurity.Mode = BasicHttpSecurityMode.None;
            }
            LBasicHttpBinding.Security = LBasicHttpSecurity;

            XmlDictionaryReaderQuotas LXmlDictionaryReaderQuotas = new XmlDictionaryReaderQuotas();
            LXmlDictionaryReaderQuotas.MaxArrayLength = int.MaxValue;
            LXmlDictionaryReaderQuotas.MaxStringContentLength = int.MaxValue;
            LBasicHttpBinding.ReaderQuotas = LXmlDictionaryReaderQuotas;

            LBasicHttpBinding.MaxReceivedMessageSize = int.MaxValue;
            LBasicHttpBinding.MaxBufferSize = int.MaxValue;
            LBasicHttpBinding.MaxBufferPoolSize = int.MaxValue;

            LBasicHttpBinding.SendTimeout = new TimeSpan(0, LTimeSpan.Minutes, LTimeSpan.Seconds);
            LBasicHttpBinding.ReceiveTimeout = new TimeSpan(0, 10, 0);

            return LBasicHttpBinding;
        }

        public static EndpointAddress CreateEndpointAddress(string AStrServer, string AStrPort, bool ABoolIsHttps, string AStrServiceName)
        {
            string LStrUrl = string.Empty;
            int LIntPort = 0;

            if (ABoolIsHttps)
            {
                LIntPort = int.Parse(AStrPort) + 1;
                LStrUrl = string.Format("https://{0}:{1}/WcfServices/{2}.svc", AStrServer, LIntPort.ToString(), AStrServiceName);
            }
            else
            {
                LStrUrl = string.Format("http://{0}:{1}/Wcf2Client/{2}.svc", AStrServer, AStrPort, AStrServiceName);
            }

            EndpointAddress LEndpointAddressReturn = new EndpointAddress(new Uri(LStrUrl, UriKind.Absolute));
            return LEndpointAddressReturn;
        }
        #endregion

    }
}
