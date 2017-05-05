using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;
using PFShareClassesC;
using UMPS3104.Commands;
using UMPS3104.Models;
using UMPS3104.Wcf00000;
using UMPS3104.Wcf11012;
using UMPS3104.Wcf31041;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common31041;
using VoiceCyber.UMP.Communications;
using WebRequest = VoiceCyber.UMP.Communications.WebRequest;

namespace UMPS3104
{
    /// <summary>
    /// Login.xaml 的交互逻辑
    /// </summary>
    public partial class Login : Window
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern int GetUserDefaultUILanguage();//获取当前系统使用的语言ID

        #region Members
        public List<LoginXmlItems> LoginXml;
        /// <summary>
        /// 登录方式
        /// F、强制登陆，N、正常登录
        /// </summary>
        private string logWays = string.Empty;
        /// <summary>
        /// 登录界面的语言包
        /// </summary>
        private static List<string> logLangs;

        /// <summary>
        /// 保存数据库连接信息
        /// </summary>
        private static List<string> DbList = new List<string>();
        private bool IBoolApplyReturn = false;
        /// <summary>
        /// 检查证书安装情况返回的信息
        /// </summary>
        private string IStrApplyReturn = string.Empty;
        /// <summary>
        /// 計時操作
        /// </summary>
        private Stopwatch watch;

        /// <summary>
        /// 默認不需要下載新的證書、xml文件--------现在要根据配置文件来判断是否使用http，所以每次登录都重新下载UMP.Server.01.xml文件----2016年5月31日 09:45:34修改
        /// </summary>
        private Boolean newFile = true;

        /// <summary>
        /// 獲取證書的Hash值
        /// </summary>
        private static string CertificateHashValue = string.Empty;

        /// <summary>
        /// 計數器 統計下載證書次數 >2次獲取證書hash失敗
        /// </summary>
        private int getHashTimes = 0;
        #endregion

        public Login()
        {
            InitializeComponent();
            LoginXml = new List<LoginXmlItems>();
            logWays = "N";
            this.Loaded += Login_Loaded;
        }

        void Login_Loaded(object sender, RoutedEventArgs e)
        {
            ButtonMin.Click += ButtonMin_Click;
            ButtonClose.Click += ButtonClose_Click;
            border1.MouseLeftButtonDown += border_MouseLeftButtonDown;
            border2.MouseLeftButtonDown += border_MouseLeftButtonDown;
            ButtonLoginSystem.Click += ButtonLoginSystem_Click;
            ButtonLoginOptions.Click += ButtonLoginOptions_Click;
            //ButtonServerApply.Click += ButtonServerApply_Click;
            App.Init();
            InitListBoxLanguage();
            //加载本地的语言,本地数据库连接配置
            LoadCurrentInformation();

            combLanguage.SelectionChanged += combLanguage_SelectionChanged;
        }

        private void border_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            this.DragMove();
        }
        
        void combLanguage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int code = Int16.Parse(combLanguage.SelectedValue.ToString());
            App.Session.LangTypeID = code;
            App.Session.LangTypeInfo = App.Session.SupportLangTypes.Where(p => p.LangID == App.Session.LangTypeID).FirstOrDefault();
            try
            {
                string languagefileName = Environment.CurrentDirectory;
                //languagefileName = languagefileName.Substring(0, languagefileName.Length - 10) + string.Format("\\Language\\{0}.xaml", App.Session.LangTypeID);
                languagefileName += string.Format("\\Language\\{0}.xaml", App.Session.LangTypeID);
                this.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri(languagefileName, UriKind.RelativeOrAbsolute) });
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
            WriteXmlLanguage(code);
            themeChanged();
        }

        /// <summary>
        /// 加载可选择样式的语言
        /// </summary>
        private void themeChanged()
        {
            try
            {
                LoginXml.Clear();
                string xmlFileName = string.Empty;
                logLangs = new List<string>();
                //xmlFileName = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"UMP.Client\UMPClientInit.xml");
                xmlFileName = Environment.CurrentDirectory + string.Format("\\UMPClientInit.xml");
                App.XmlPathString = xmlFileName;
                if (!App.IsExsitXml())
                {
                    //App.ShowExceptionMessage(string.Format("UMPClientInit.xml  Is Not Exist"));
                    App.ShowExceptionMessage(TryFindResource("LoseXmlFile").ToString());
                    return;
                }
                XmlDocument umpclientinitxml = new XmlDocument();
                umpclientinitxml.Load(xmlFileName);
                XmlNodeList localThemeLangs = umpclientinitxml.SelectNodes("root/Languages");
                if (localThemeLangs != null)
                {
                    foreach (XmlNode localThemeLang in localThemeLangs)
                    {
                        string temp = localThemeLang.Attributes["value"].Value.ToString();
                        string temp1 = App.Session.LangTypeInfo.LangID.ToString();
                        if (temp == temp1)
                        {
                            for (int i = 0; i < localThemeLang.ChildNodes.Count; i++)
                            {
                                LoginXmlItems loginTempItems = new LoginXmlItems();
                                XmlNode tempNode = localThemeLang.ChildNodes.Item(i);
                                loginTempItems.name = tempNode.Attributes["name"].Value.ToString();
                                loginTempItems.key = tempNode.Attributes["key"].Value.ToString();
                                LoginXml.Add(loginTempItems);
                            }
                            //获取语言包的方法执行在登录之后，所以登录的语言包写在本地方法中
                            logLangs = LanguageClient.Langs(App.Session.LangTypeInfo.LangID);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        /// <summary>
        /// 更新C:\Users\Administrator\AppData\Local\UMP.Client\UMPClientInit.xml文件 默认语言
        /// </summary>
        /// <param name="langType"></param>
        private void WriteXmlLanguage(int code)
        {
            try
            {
                string xmlFileName = string.Empty;
                //xmlFileName = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"UMP.Client\UMPClientInit.xml");
                xmlFileName = Environment.CurrentDirectory + string.Format("\\UMPClientInit.xml");
                App.XmlPathString = xmlFileName;
                if (!App.IsExsitXml())
                {
                    App.ShowExceptionMessage(string.Format("UMPClientInit.xml  Is Not Exist"));
                    return;
                }
                DataSet Xmlds = new DataSet();
                Xmlds.ReadXml(xmlFileName);
                Xmlds.Tables["LanguagesType"].Rows[0]["LocalLanguage"] = code;
                Xmlds.WriteXml(xmlFileName);
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        /// <summary>
        /// 更新...\UMPClientInit.xml文件  1. 服务器信息;2.数据库连接信息 最近登录的账户
        /// 端口、地址加密
        /// 地址、端口、数据库名字、登录名、密码加密
        /// </summary>
        private void WriteXmlServerInfo(int i, string strUserAccount)
        {
            try
            {
                string xmlFileName = string.Empty;
                //xmlFileName = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"UMP.Client\UMPClientInit.xml");
                xmlFileName = Environment.CurrentDirectory + string.Format("\\UMPClientInit.xml");
                App.XmlPathString = xmlFileName;
                if (!App.IsExsitXml())
                {
                    //App.ShowExceptionMessage(string.Format("UMPClientInit.xml  Is Not Exist"));
                    App.ShowExceptionMessage(TryFindResource("LoseXmlFile").ToString());
                    return;
                }
                DataSet Xmlds = new DataSet();
                Xmlds.ReadXml(xmlFileName);
                XmlDocument umpclientinitxml;
                switch (i)
                {
                    case 1:
                        Xmlds.Tables["WcfConfig"].Rows[0]["IP"] = EncryptString(TextBoxServerAddress.Text);
                        Xmlds.Tables["WcfConfig"].Rows[0]["Port"] = EncryptString(TextBoxServerPort.Text);
                        Xmlds.WriteXml(xmlFileName);
                        //App.ShowInfoMessage(string.Format("Save Sucessed"));
                        App.ShowInfoMessage(TryFindResource("SaveSucessed").ToString());
                        tabAccountInfo.Focus();
                        newFile = true;
                        break;
                    case 2:
                        umpclientinitxml = new XmlDocument();
                        string temp = string.Empty;
                        if (int.Parse(DbList[1].ToString()) == 2)
                        { temp = "DatabaseMSSQL"; }
                        if (int.Parse(DbList[1].ToString()) == 3)
                        { temp = "DatabaseOracle"; }
                        temp = string.Format("root/DatabaseConnects/{0}/", temp);
                        Xmlds.Tables["DatabaseConnects"].Rows[0]["DatabaseType"] = DbList[1];//數據庫類型ID
                        Xmlds.WriteXml(xmlFileName);
                        umpclientinitxml.Load(xmlFileName);
                        umpclientinitxml.SelectSingleNode(string.Format("{0}TypeName", temp)).InnerText = int.Parse(DbList[1]) == 2 ? "MSSQL" : "Oracle";//數據庫類型名字
                        umpclientinitxml.SelectSingleNode(string.Format("{0}Host", temp)).InnerText = EncryptString(DbList[2]);//地址
                        umpclientinitxml.SelectSingleNode(string.Format("{0}Port", temp)).InnerText = EncryptString(DbList[3]);//端口
                        umpclientinitxml.SelectSingleNode(string.Format("{0}DBName", temp)).InnerText = EncryptString(DbList[4]);//数据库名字
                        umpclientinitxml.SelectSingleNode(string.Format("{0}LoginName", temp)).InnerText = EncryptString(DbList[5]);//数据库登录名
                        umpclientinitxml.SelectSingleNode(string.Format("{0}Password", temp)).InnerText = EncryptString(DbList[6]);//数据库登录密码
                        umpclientinitxml.SelectSingleNode("root/WcfConfig/Account").InnerText = string.IsNullOrWhiteSpace(strUserAccount) == true ? string.Empty : strUserAccount;//将最近登录的账户保存到xml中
                        umpclientinitxml.Save(xmlFileName);
                        //更新App.Session
                        DatabaseInfo dbInfo = new DatabaseInfo();
                        dbInfo.TypeID = int.Parse(DbList[1].ToString());
                        dbInfo.TypeName = umpclientinitxml.SelectSingleNode(string.Format("{0}TypeName", temp)).InnerText;
                        dbInfo.Host = DbList[2];
                        dbInfo.Port = Convert.ToInt32(DbList[3]);
                        dbInfo.DBName = DbList[4];
                        dbInfo.LoginName = DbList[5];
                        dbInfo.Password = DbList[6];
                        App.Session.DBType = dbInfo.TypeID;
                        App.Session.DBConnectionString = dbInfo.GetConnectionString();
                        App.Session.DatabaseInfo = dbInfo;
                        break;
                }
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        /// <summary>
        /// 更新C:\Users\Administrator\AppData\Local\UMP.Client\UMPClientInit.xml文件 默认样式
        /// </summary>
        /// <param name="themeInfo"></param>
        private void WriteXmlTheme(ThemeInfo themeInfo)
        {
            try
            {
                string xmlFileName = string.Empty;
                //xmlFileName = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"UMP.Client\UMPClientInit.xml");
                xmlFileName = Environment.CurrentDirectory + string.Format("\\UMPClientInit.xml");
                App.XmlPathString = xmlFileName;
                if (!App.IsExsitXml())
                {
                    App.ShowExceptionMessage(string.Format("UMPClientInit.xml  Is Not Exist"));
                    return;
                }
                DataSet Xmlds = new DataSet();
                Xmlds.ReadXml(xmlFileName);

                Xmlds.Tables["SupportStyle"].Rows[0]["Name"] = themeInfo.Name;
                Xmlds.Tables["SupportStyle"].Rows[0]["Color"] = themeInfo.Color;

                Xmlds.WriteXml(xmlFileName);
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        /// <summary>
        /// 加载基本信息(最后使用的LanguageID， StyleID，最后登录的用户，Application Server Information，数据库连接串)
        /// </summary>
        private void LoadCurrentInformation()
        {
            string xmlFileName = string.Empty;
            //xmlFileName = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"UMP.Client\UMPClientInit.xml");
            xmlFileName = Environment.CurrentDirectory + string.Format("\\UMPClientInit.xml");
            string xmlServer = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), string.Format("UMP\\UMPS3104\\AgentServerInfo.xml"));
            //App.ShowInfoMessage(xmlFileName);
            App.XmlPathString = xmlFileName;
            if (!App.IsExsitXml())
            {
                //labMsg.Content = "UMPClientInit.xml  Is Not Exist";
                labMsg.Content = TryFindResource("LoseXmlFile");
            }
            else
            {
                try
                {
                    XmlDocument umpclientinitxml = new XmlDocument();
                    umpclientinitxml.Load(xmlFileName);
                    string tempAdd = string.Empty;
                    string tempPort = string.Empty;
                    XmlDocument XmlServer = new XmlDocument();
                    if(File.Exists(xmlServer))
                    {
                        XmlServer.Load(xmlServer);
                        tempAdd = XmlServer.SelectSingleNode("ServerInfo/ServerIP").InnerText;
                        tempPort = XmlServer.SelectSingleNode("ServerInfo/Port").InnerText;
                    }

                    //加载wcf信息
                    App.Session.AppServerInfo.Protocol = DecryptString(umpclientinitxml.SelectSingleNode("root/WcfConfig/Protocol").InnerText);
                    TextBoxServerAddress.Text = DecryptString(umpclientinitxml.SelectSingleNode("root/WcfConfig/IP").InnerText);
                    App.Session.AppServerInfo.Address = DecryptString(umpclientinitxml.SelectSingleNode("root/WcfConfig/IP").InnerText);
                    TextBoxServerPort.Text = DecryptString(umpclientinitxml.SelectSingleNode("root/WcfConfig/Port").InnerText);
                    App.Session.AppServerInfo.Port = App.IntParse(DecryptString(umpclientinitxml.SelectSingleNode("root/WcfConfig/Port").InnerText), 0);

                    //加载上次登录的座席
                    TextBoxLoginAccount.Text = string.IsNullOrWhiteSpace(umpclientinitxml.SelectSingleNode("root/WcfConfig/Account").InnerText) == false ? umpclientinitxml.SelectSingleNode("root/WcfConfig/Account").InnerText : string.Empty;

                    if (string.IsNullOrWhiteSpace(App.Session.AppServerInfo.Address))
                    {
                        if(!string.IsNullOrWhiteSpace(tempAdd))
                        {
                            TextBoxServerAddress.Text = tempAdd;
                            TextBoxServerPort.Text = tempPort;
                        }
                        else
                        {
                            //ServerMsg.Content = string.Format("请输入服务器地址、端口");
                            ServerMsg.Content = TryFindResource("ServerInfoIsNull");
                            tabServerInfo.Focus();
                        }
                    }

                    //加载数据库信息
                    XmlNode databasexmlnode = umpclientinitxml.SelectSingleNode("root/DatabaseConnects");
                    string databasetype = databasexmlnode.Attributes["DatabaseType"].Value;
                    switch (databasetype)
                    {
                        case "3":
                            {
                                DatabaseInfo dbInfo = new DatabaseInfo();
                                dbInfo.TypeID = 3;
                                dbInfo.TypeName = umpclientinitxml.SelectSingleNode("root/DatabaseConnects/DatabaseOracle/TypeName").InnerText;
                                dbInfo.Host = DecryptString(umpclientinitxml.SelectSingleNode("root/DatabaseConnects/DatabaseOracle/Host").InnerText);
                                dbInfo.Port = App.IntParse(DecryptString(umpclientinitxml.SelectSingleNode("root/DatabaseConnects/DatabaseOracle/Port").InnerText), 0);
                                dbInfo.DBName = DecryptString(umpclientinitxml.SelectSingleNode("root/DatabaseConnects/DatabaseOracle/DBName").InnerText);
                                dbInfo.LoginName = DecryptString(umpclientinitxml.SelectSingleNode("root/DatabaseConnects/DatabaseOracle/LoginName").InnerText);
                                dbInfo.Password = DecryptString(umpclientinitxml.SelectSingleNode("root/DatabaseConnects/DatabaseOracle/Password").InnerText);
                                App.Session.DBType = dbInfo.TypeID;
                                App.Session.DBConnectionString = dbInfo.GetConnectionString();
                                App.Session.DatabaseInfo = dbInfo;
                            }
                            break;
                        case "2":
                            {
                                DatabaseInfo dbInfo = new DatabaseInfo();
                                dbInfo.TypeID = 2;
                                dbInfo.TypeName = umpclientinitxml.SelectSingleNode("root/DatabaseConnects/DatabaseMSSQL/TypeName").InnerText;
                                dbInfo.Host = DecryptString(umpclientinitxml.SelectSingleNode("root/DatabaseConnects/DatabaseMSSQL/Host").InnerText);
                                dbInfo.Port = App.IntParse(DecryptString(umpclientinitxml.SelectSingleNode("root/DatabaseConnects/DatabaseMSSQL/Port").InnerText), 0);
                                dbInfo.DBName = DecryptString(umpclientinitxml.SelectSingleNode("root/DatabaseConnects/DatabaseMSSQL/DBName").InnerText);
                                dbInfo.LoginName = DecryptString(umpclientinitxml.SelectSingleNode("root/DatabaseConnects/DatabaseMSSQL/LoginName").InnerText);
                                dbInfo.Password = DecryptString(umpclientinitxml.SelectSingleNode("root/DatabaseConnects/DatabaseMSSQL/Password").InnerText);
                                App.Session.DBConnectionString = dbInfo.GetConnectionString();
                                App.Session.DatabaseInfo = dbInfo;
                                App.Session.DBType = dbInfo.TypeID;
                            }
                            break;
                    }

                    //加载支持Style
                    XmlNode xmllocalstyle = umpclientinitxml.SelectSingleNode("root/SupportStyle");
                    App.ListSupportTheme.Clear();
                    foreach (XmlNode xnode in xmllocalstyle.ChildNodes)
                    {
                        LocalXmlTheme localxmltheme = new LocalXmlTheme();
                        localxmltheme.Name = xnode.Attributes["Name"].Value.ToString();
                        localxmltheme.Color = xnode.Attributes["Color"].Value.ToString();
                        App.ListSupportTheme.Add(localxmltheme);
                    }
                    ThemeInfo themeInfo = new ThemeInfo();
                    themeInfo.Name = xmllocalstyle.Attributes["Name"].Value.ToString();
                    themeInfo.Color = xmllocalstyle.Attributes["Color"].Value.ToString();
                    //默认style根据季节变化
                    if (themeInfo.Name == "Default")
                    {
                        switch (DateTime.Now.Month)
                        {
                            case 3:
                            case 4:
                            case 5:
                                themeInfo.Name = "Style01";
                                break;
                            case 6:
                            case 7:
                            case 8:
                                themeInfo.Name = "Style02";
                                break;
                            case 9:
                            case 10:
                            case 11:
                                themeInfo.Name = "Style03";
                                break;
                            case 12:
                            case 1:
                            case 2:
                                themeInfo.Name = "Style04";
                                break;
                        }
                    }
                    App.Session.ThemeInfo = themeInfo;
                    App.Session.ThemeName = themeInfo.Name;

                    //加载支持的语言
                    XmlNode xmllocalsavelanguage = umpclientinitxml.SelectSingleNode("root/LanguagesType");
                    string languageid = xmllocalsavelanguage.Attributes["LocalLanguage"].Value.ToString();
                    int localmachinelanguageid = GetUserDefaultUILanguage();

                    App.ListSupportLanguage.Clear();
                    foreach (XmlNode xnode in xmllocalsavelanguage.ChildNodes)
                    {
                        LocalXmlLanguage localxmllanguage = new LocalXmlLanguage();
                        localxmllanguage.Display = xnode.Attributes["name"].Value.ToString();
                        localxmllanguage.Key = xnode.Attributes["key"].Value.ToString();
                        localxmllanguage.Code = xnode.Attributes["code"].Value.ToString();
                        App.ListSupportLanguage.Add(localxmllanguage);
                    }

                    if (App.IntParse(languageid, 0) == 0)
                    {
                        if (localmachinelanguageid == 2052 || localmachinelanguageid == 1028
                            || localmachinelanguageid == 1033 || localmachinelanguageid == 1041)
                        {
                            App.Session.LangTypeID = localmachinelanguageid;
                            App.Session.LangTypeInfo = App.Session.SupportLangTypes.Where(p => p.LangID == App.Session.LangTypeID).FirstOrDefault();
                        }
                        else
                        {
                            App.Session.LangTypeID = 1033;
                            App.Session.LangTypeInfo = App.Session.SupportLangTypes.Where(p => p.LangID == 1033).FirstOrDefault();
                        }
                    }
                    else
                    {
                        App.Session.LangTypeID = App.IntParse(languageid, 0);
                        App.Session.LangTypeInfo = App.Session.SupportLangTypes.Where(p => p.LangID == App.Session.LangTypeID).FirstOrDefault();
                    }
                    for (int i = 0; i < App.ListSupportLanguage.Count; i++)
                    {
                        if (App.ListSupportLanguage[i].Code.Equals(App.Session.LangTypeID.ToString()))
                        {
                            combLanguage.SelectedIndex = i;
                            break;
                        }
                    }

                    foreach (LocalXmlLanguage locallanguage in App.ListSupportLanguage)
                    {
                        XmlNode languageXNode = umpclientinitxml.SelectSingleNode("root/Languages[@value='" + locallanguage.Code + "']");
                        if (languageXNode != null)
                        {
                            foreach (XmlNode childxnode in languageXNode.ChildNodes)
                            {
                                LocalXmlLanguage allxmllanguage = new LocalXmlLanguage();
                                allxmllanguage.Display = childxnode.Attributes["name"].Value.ToString();
                                allxmllanguage.Key = childxnode.Attributes["key"].Value.ToString();
                                App.ListXmlLanguage.Add(allxmllanguage);
                            }
                        }
                    }
                    string languagefileName = Environment.CurrentDirectory;
                    //languagefileName = languagefileName.Substring(0, languagefileName.Length - 10) + string.Format("\\Language\\{0}.xaml", App.Session.LangTypeID);
                    languagefileName += string.Format("\\Language\\{0}.xaml", App.Session.LangTypeID);
                    //App.ShowInfoMessage(languagefileName);
                    this.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri(languagefileName, UriKind.RelativeOrAbsolute) });

                    themeChanged();

                    umpclientinitxml = null;

                }
                catch (Exception e)
                {
                    labMsg.Content = e.Message;
                }
            }

        }

        //获取服务信息、数据库连接串    返回运行信息 
        private string GetServerInfo(int port, string strUserAccount)
        {
            string errorReturn = string.Empty;
            try
            {
                App.Session.AppServerInfo.Port = port;
                if (App.defaultHttp)
                {
                    App.Session.AppServerInfo.Protocol = "http";
                    App.Session.AppServerInfo.SupportHttps = false;
                }
                else//https
                {
                    App.Session.AppServerInfo.Protocol = "https";
                    App.Session.AppServerInfo.Port += 1;
                }
                DbList.Clear();
                DbList.Add("11");
                Service00000Client loginClient = new Service00000Client(WebHelper.CreateBasicHttpBinding(App.Session),
                                    WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service00000"));
                OperationDataArgs args = loginClient.OperationMethodA(05, DbList);
                DataRow dr = args.DataSetReturn.Tables[0].Rows[0];
                int count = args.DataSetReturn.Tables[0].Rows.Count;
                if (count != 1)
                {
                    //errorReturn = string.Format("获取数据库连接信息失败");
                    errorReturn = TryFindResource("ServerInfoIsError").ToString();
                    return errorReturn;
                }
                DbList = new List<string>();
                DbList.Add(dr["DBID"].ToString());
                DbList.Add(dr["DBType"].ToString());
                DbList.Add(dr["ServerHost"].ToString());
                DbList.Add(dr["ServerPort"].ToString());
                DbList.Add(dr["NameService"].ToString());
                DbList.Add(dr["LoginID"].ToString());
                DbList.Add(dr["LoginPwd"].ToString());
                DbList.Add(dr["OtherArgs"].ToString());
                DbList.Add(dr["Describer"].ToString());

                //DbList = new List<string>();
                //DbList.Add("2");
                //DbList.Add("2");
                //DbList.Add("192.168.8.138");
                //DbList.Add("1433");
                //DbList.Add("UMPDataDB0506_96");
                //DbList.Add("sa");
                //DbList.Add("voicecodes");
                //DbList.Add("1");
                //DbList.Add("1");
                WriteXmlServerInfo(2, strUserAccount);
            }
            catch (Exception ex)
            {
                errorReturn = ex.Message;
            }
            return errorReturn;
        }

        /// <summary>
        /// 获取证书，存放本地
        /// 返回值為空 操作正常，否則返回ex
        /// StoreName， 0 僅下載證書，1，2 安裝到StoreName.Root，3 安裝到StoreName.TrustedPublisher
        /// </summary>
        /// <returns></returns>
        private string GetCertificateInfo(int storeName)
        {
            string Result = string.Empty;
            HttpWebRequest LHttpWebRequest = null;
            Stream LStreamResponse = null;
            System.IO.FileStream LStreamCertificateFile = null;
            string LStrCertificateFileFullName = string.Empty;
            string CertificateName = string.Format(@"UMP.S.{0}.pfx", App.Session.AppServerInfo.Address);
            LStrCertificateFileFullName = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), string.Format(@"UMP.Client\UMP.S.{0}.pfx", App.Session.AppServerInfo.Address));
            try
            {
                if (System.IO.File.Exists(LStrCertificateFileFullName))
                {
                    System.IO.FileInfo objInfo = new FileInfo(LStrCertificateFileFullName);
                    long len = objInfo.Length;
                    if (len <= 0)
                    {
                        System.IO.File.Delete(LStrCertificateFileFullName);
                        Result = GetCertificateInfo(storeName);
                        if (!string.IsNullOrWhiteSpace(Result))
                        {
                            return Result;
                        }
                    }
                    return Result;
                }
                LStreamCertificateFile = new FileStream(LStrCertificateFileFullName, FileMode.Create);
                LHttpWebRequest = (HttpWebRequest)HttpWebRequest.Create("http://" + App.Session.AppServerInfo.Address + ":" + App.Session.AppServerInfo.Port + "/Components/Certificates/" + CertificateName);
                //LHttpWebRequest = (HttpWebRequest)HttpWebRequest.Create("http://192.168.4.184:8081/Components/Certificates/" + CertificateName);
                long LContenctLength = LHttpWebRequest.GetResponse().ContentLength;
                LHttpWebRequest.AddRange(0);

                LStreamResponse = LHttpWebRequest.GetResponse().GetResponseStream();
                byte[] LbyteRead = new byte[1024];
                int LIntReadedSize = LStreamResponse.Read(LbyteRead, 0, 1024);
                while (LIntReadedSize > 0)
                {
                    LStreamCertificateFile.Write(LbyteRead, 0, LIntReadedSize);
                    LIntReadedSize = LStreamResponse.Read(LbyteRead, 0, 1024);
                }
                LStreamCertificateFile.Close(); LStreamCertificateFile.Dispose();
                switch (storeName)
                {
                    case 0:
                        break;
                    case 2:
                        Result = InstallCertificateToStore(StoreName.Root);
                        break;
                    case 3:
                        Result = InstallCertificateToStore(StoreName.TrustedPublisher);
                        break;
                }
                if (!string.IsNullOrWhiteSpace(Result)) WriteLog.WriteLogToFile("(ಥ_ಥ) InstallCertificateToStore", storeName + "\t" + Result);
            }
            catch (Exception ex)
            {
                Result = string.Format("{0} \t {1} ", storeName, ex.Message);
                App.ShowExceptionMessage(ex.Message);
            }
            finally
            {
                if (LHttpWebRequest != null) { LHttpWebRequest.Abort(); }
                if (LStreamResponse != null) { LStreamResponse.Close(); LStreamResponse.Dispose(); }
                if (LStreamCertificateFile != null) { LStreamCertificateFile.Close(); LStreamCertificateFile.Dispose(); }
                System.IO.File.Delete(LStrCertificateFileFullName);
            }
            return Result;
        }


        /// <summary>
        /// 获取证书Hash值
        /// 返回值为空即服务器地址问题
        /// </summary>
        private string GetCertificateHash()
        {
            watch = new Stopwatch();
            string HashValue = string.Empty;
            HttpWebRequest LHttpWebRequest = null;
            Stream LStreamResponse = null;
            System.IO.FileStream LStreamCertificateFile = null;
            string LStrCertificateFileFullName = string.Empty;
            string XmlName = string.Format(@"UMP.Client\UMP.Server.01.xml");
            LStrCertificateFileFullName = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), XmlName);
            string time = string.Empty;
            try
            {
                if (!System.IO.File.Exists(LStrCertificateFileFullName) || newFile)//如果不存在xml文件 或者 需要下載新的xml文件
                {
                    watch.Start();
                    LStreamCertificateFile = new FileStream(LStrCertificateFileFullName, FileMode.Create);
                    LHttpWebRequest = (HttpWebRequest)HttpWebRequest.Create("http://" + App.Session.AppServerInfo.Address + ":" + App.Session.AppServerInfo.Port + "/GlobalSettings/UMP.Server.01.xml");
                    //LHttpWebRequest = (HttpWebRequest)HttpWebRequest.Create("http://192.168.4.184:8081/GlobalSettings/UMP.Server.01.xml");
                    //long LContenctLength = LHttpWebRequest.GetResponse().ContentLength;
                    LHttpWebRequest.AddRange(0);
                    LStreamResponse = LHttpWebRequest.GetResponse().GetResponseStream();
                    watch.Stop();
                    time = watch.ElapsedMilliseconds.ToString();
                    WriteLog.WriteLogToFile("返回數據流", time);

                    watch.Restart();
                    byte[] LbyteRead = new byte[1024];
                    int LIntReadedSize = LStreamResponse.Read(LbyteRead, 0, 1024);
                    while (LIntReadedSize > 0)
                    {
                        LStreamCertificateFile.Write(LbyteRead, 0, LIntReadedSize);
                        LIntReadedSize = LStreamResponse.Read(LbyteRead, 0, 1024);
                    }
                    LStreamCertificateFile.Close(); LStreamCertificateFile.Dispose();
                    watch.Stop();
                    time = watch.ElapsedMilliseconds.ToString();
                    WriteLog.WriteLogToFile("下載文件", time);
                }


                watch.Restart();
                //获取xml中的Hash值
                XmlDocument umpclientinitxml = new XmlDocument();
                System.IO.FileInfo objInfo = new FileInfo(LStrCertificateFileFullName);
                long len = objInfo.Length;
                if (len <= 0)//下載下來的xml文件有問題
                {
                    getHashTimes++;
                    newFile = true;
                    HashValue = GetCertificateHash();
                    if (HashValue == string.Empty || getHashTimes > 1)
                    {
                        WriteLog.WriteLogToFile("(ಥ_ಥ) ", "服務器中不存在UMP.Server.01.xml文件，或地址已更改");
                        return string.Empty;
                    }
                }
                umpclientinitxml.Load(LStrCertificateFileFullName);
                XmlNodeList xmlNodeList = umpclientinitxml.SelectNodes("UMPSetted/IISBindingProtocol/ProtocolBind");

                foreach (XmlNode xmlNode in xmlNodeList)
                {
                    if (xmlNode.Attributes["Protocol"].Value.ToLower().Equals("https"))
                    {
                        HashValue = xmlNode.Attributes["OtherArgs"].Value;
                        if (xmlNode.Attributes["Activated"].Value == "1")
                        {
                            App.defaultHttp = false;
                        }
                        else
                        {
                            App.defaultHttp = true;
                        }
                    }
                }
                watch.Stop();
                time = watch.ElapsedMilliseconds.ToString();
                WriteLog.WriteLogToFile("獲取Hash值", time);

                if (string.IsNullOrWhiteSpace(HashValue))
                {
                    if (getHashTimes > 1)
                    {
                        return string.Empty;
                    }
                    GetCertificateHash();
                    WriteLog.WriteLogToFile("獲取Hash值为空，重新获取UMP.Server.01.xml文件", LStrCertificateFileFullName);
                }
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
            finally
            {
                if (LHttpWebRequest != null) { LHttpWebRequest.Abort(); }
                if (LStreamResponse != null) { LStreamResponse.Close(); LStreamResponse.Dispose(); }
                if (LStreamCertificateFile != null) { LStreamCertificateFile.Close(); LStreamCertificateFile.Dispose(); }
                //if (!newFile) { System.IO.File.Delete(LStrCertificateFileFullName); }
            }
            return HashValue;
        }

        /// <summary>
        /// 检查证书是否在指定区域已经安装
        /// </summary>
        /// <param name="AStoreName">区域名</param>
        /// <returns>True:已经安装；False:未安装</returns>
        private bool CheckCertificateIsExist(StoreName AStoreName)
        {
            bool LBoolReturn = false;
            try
            {
                X509Store LX509Store = new X509Store(AStoreName, StoreLocation.LocalMachine);
                LX509Store.Open(OpenFlags.ReadOnly);
                foreach (X509Certificate LX509CertificateSingle in LX509Store.Certificates)
                {
                    if (LX509CertificateSingle.GetCertHashString().Trim() == CertificateHashValue)
                    {
                        LBoolReturn = true;
                        break;
                    }
                }
                LX509Store.Close();
                LX509Store = null;
            }
            catch
            {
                LBoolReturn = true;
            }
            return LBoolReturn;
        }

        /// <summary>
        /// 将证书安装到制定存储区域
        /// 如果安装失败不需要在此方法中弹出错误消息，把消息返回给引用函数
        /// </summary>
        /// <param name="AStoreName">证书存储区的名称</param>
        /// <param name="AByteCertificate">证书文件内容</param>
        /// <param name="AStrPassword">证书密码</param>
        /// <returns>如果为空，表示安装成功，否则为错误信息</returns>
        private string InstallCertificateToStore(StoreName AStoreName)
        {
            string LStrReturn = string.Empty;
            string LStrCertificateFileFullName = string.Empty;

            try
            {
                LStrCertificateFileFullName =
                    System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                        string.Format(@"UMP.Client\UMP.S.{0}.pfx", App.Session.AppServerInfo.Address));

                //byte[] LByteReadedCertificate = System.IO.File.ReadAllBytes(LStrCertificateFileFullName);
                //X509Certificate2 LX509Certificate = new X509Certificate2(LByteReadedCertificate, "VoiceCyber,123");

                X509Certificate2 LX509Certificate = new X509Certificate2(LStrCertificateFileFullName, "VoiceCyber,123",
                    X509KeyStorageFlags.Exportable | X509KeyStorageFlags.PersistKeySet |
                    X509KeyStorageFlags.MachineKeySet);

                X509Store LX509Store = null;

                LX509Store = new X509Store(AStoreName, StoreLocation.LocalMachine);
                LX509Store.Open(OpenFlags.ReadWrite);
                LX509Store.Remove(LX509Certificate);
                LX509Store.Add(LX509Certificate);
                LX509Store.Close();
            }
            catch (Exception ex)
            {
                LStrReturn = ex.Message + " \t " + ex.StackTrace;
            }
            //只要更新一次證書，xml文件就被刪除
            finally
            {
                string XmlName = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), string.Format(@"UMP.Client\UMP.Server.01.xml"));
                System.IO.File.Delete(XmlName);
            }
            return LStrReturn;

        }

        //綁定App.LocalXmlLanguage中的值
        private void InitListBoxLanguage()
        {
            combLanguage.ItemsSource = App.ListSupportLanguage;
            combLanguage.DisplayMemberPath = "Display";
            combLanguage.SelectedValuePath = "Code";
        }


        #region
        void ButtonMin_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;

        }

        void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            System.Environment.Exit(System.Environment.ExitCode);
        }

        void ButtonLoginSystem_Click(object sender, RoutedEventArgs e)
        {
            ButtonLoginSystem.IsEnabled = false;
            BackgroundWorker mWorker = new BackgroundWorker();
            string errorString = string.Empty;
            labMsg.Content = "";
            ServerMsg.Content = "";

            string strUserAccount = TextBoxLoginAccount.Text.Trim();
            string strUserPassword = PasswordBoxLoginPassword.Password.Trim();
            //客户端机器名
            string hostName = EncryptString(System.Net.Dns.GetHostName());

            //服務器地址、端口為空
            if (string.IsNullOrWhiteSpace(TextBoxServerAddress.Text) ||
                string.IsNullOrWhiteSpace(TextBoxServerPort.Text))
            {
                App.ShowExceptionMessage(TryFindResource("ServerInfoIsNull").ToString());
                ButtonLoginSystem.IsEnabled = true;
                TextBoxServerAddress.Focus();
                return;
            }
            //请输入正确的服务器地址、端口
            if (!ServerHostIsIPAddress(TextBoxServerAddress.Text))
            {
                App.ShowExceptionMessage(TryFindResource("AddressIsError").ToString());
                ButtonLoginSystem.IsEnabled = true;
                tabServerInfo.Focus();
                TextBoxServerAddress.Focus();
                return;
            }
            if (!ServerHostIsIPPort(TextBoxServerPort.Text))
            {
                App.ShowExceptionMessage(TryFindResource("PortIsError").ToString());
                ButtonLoginSystem.IsEnabled = true;
                tabServerInfo.Focus();
                TextBoxServerPort.Focus();
                return;
            }
            //IP、端口 有過修改、發生變化
            if (TextBoxServerAddress.Text != App.Session.AppServerInfo.Address ||
                TextBoxServerPort.Text != App.Session.AppServerInfo.Port.ToString())
            {
                App.Session.AppServerInfo.Address = TextBoxServerAddress.Text;
                App.Session.AppServerInfo.Port = Convert.ToInt32(TextBoxServerPort.Text);
                ServerMsg.Content = TryFindResource("SaveSetting").ToString();//设置保存中
                WriteXmlServerInfo(1, strUserAccount);
                //ServerMsg.Content = string.Format("设置保存成功");
                ServerMsg.Content = TryFindResource("SettingSaved");
                getHashTimes = 0;//恢復計數器
                tabAccountInfo.Focus();
            }
            if (TextBoxServerAddress.Text == App.Session.AppServerInfo.Address &&
                TextBoxServerPort.Text == App.Session.AppServerInfo.Port.ToString())
            {
                tabAccountInfo.Focus();
            }
            //端口
            int port = Convert.ToInt32(TextBoxServerPort.Text);

            //登录 禁止为空
            if (string.IsNullOrWhiteSpace(strUserAccount) || string.IsNullOrWhiteSpace(strUserPassword))
            {
                //labMsg.Content = "Account Or Password  Is Null";
                labMsg.Content = TryFindResource("AccountInfoIsNull");
                TextBoxLoginAccount.Focus();
                ButtonLoginSystem.IsEnabled = true;
                return;
            }


            try
            {
                int intError = 0;
                MyWaiter.Visibility = Visibility.Visible;
                mWorker.WorkerReportsProgress = true;
                mWorker.WorkerSupportsCancellation = true;
                //注册线程主体方法
                mWorker.DoWork += (s, de) =>
                {
                    intError = 1;
                    errorString = TryFindResource("CheckServerInfo").ToString();
                    mWorker.ReportProgress(intError);

                    CertificateHashValue = GetCertificateHash();
                    if (string.IsNullOrEmpty(CertificateHashValue))
                    {
                        intError = 2;
                        errorString = TryFindResource("ServerInfoIsNull").ToString();
                        mWorker.ReportProgress(intError);
                        mWorker.Dispose();
                        return;
                    }
                    if (!App.defaultHttp)//走https的时候才需要证书，所以走http不检查证书安装情况
                    {
                        //检查本机证书安装情况
                        IBoolApplyReturn = true;
                        //errorString = string.Format("检查证书安装情况");
                        errorString = TryFindResource("CheckCertificate").ToString();
                        intError = 4;
                        mWorker.ReportProgress(intError);
                        if (!CheckCertificateIsExist(StoreName.Root))
                        {
                            //檢查證書下載是否正常
                            errorString = GetCertificateInfo(0);
                            if (!string.IsNullOrEmpty(errorString))
                            {
                                intError = 4;
                                errorString = TryFindResource("ServerInfoIsNull").ToString();
                                mWorker.ReportProgress(intError);
                                mWorker.Dispose();
                                return;
                            }
                        }

                        if (!CheckCertificateIsExist(StoreName.Root))
                        {
                            IStrApplyReturn = GetCertificateInfo(2);
                            //如果不为空，安装失败返回错误信息
                            if (!string.IsNullOrEmpty(IStrApplyReturn))
                            {
                                intError = 6;
                                IBoolApplyReturn = false;
                                errorString = IStrApplyReturn;
                                mWorker.ReportProgress(intError);
                                mWorker.Dispose();
                                return;
                            }
                        }

                        if (!CheckCertificateIsExist(StoreName.TrustedPublisher))
                        {
                            IStrApplyReturn = GetCertificateInfo(3);
                            if (!string.IsNullOrEmpty(IStrApplyReturn))
                            {
                                intError = 8;
                                IBoolApplyReturn = false;
                                errorString = IStrApplyReturn;
                                mWorker.ReportProgress(intError);
                                mWorker.Dispose();
                                return;
                            }
                        }
                        if (IBoolApplyReturn)
                        {
                            intError = 10;
                            //errorString = string.Format("证书安装正常");
                            errorString = TryFindResource("CertificateIsExist").ToString();
                            mWorker.ReportProgress(intError);
                            Thread.Sleep(300);
                        }
                    }

                    intError = 11;
                    errorString = TryFindResource("Loading").ToString();
                    mWorker.ReportProgress(intError);

                    //获取数据库连接串+对http的修改->https,port+1
                    errorString = GetServerInfo(port, strUserAccount);
                    if (!string.IsNullOrEmpty(errorString))
                    {
                        intError = 12;
                        mWorker.ReportProgress(intError);
                        mWorker.Dispose();
                        return;
                    }
                    WriteLog.WriteLogToFile("serverInfo",
                        App.Session.AppServerInfo.Protocol + "," + App.Session.AppServerInfo.Address + "," +
                        App.Session.AppServerInfo.Port);
                    List<string> clientListString = new List<string>();
                    clientListString.Add(EncryptString(strUserAccount)); //用户帐号
                    clientListString.Add(EncryptString(strUserPassword)); //登录密码
                    clientListString.Add(EncryptString(logWays)); //登录方式   ‘N’正常登录；‘F’：强制登录
                    clientListString.Add(hostName); //客户端机器名
                    Service00000Client loginClient =new Service00000Client(WebHelper.CreateBasicHttpBinding(App.Session),
                            WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service00000"));
                    OperationDataArgs args = loginClient.OperationMethodA(41, clientListString);
                    string[] values = args.StringReturn.Split(new[] { ConstValue.SPLITER_CHAR },
                        StringSplitOptions.RemoveEmptyEntries);
                    WriteLog.WriteLogToFile("LoadInfo", args.StringReturn);
                    loginClient.Close();

                    if (values.Length < 5) //5个参数 即老用户 6个参数 即新用户
                    {
                        if (args.BoolReturn) //WCF錯誤BoolReturn返回值為false
                        {
                            errorString = DecryptString(values[0]);
                            errorString = TryFindResource(errorString).ToString();
                            intError = 14;
                        }
                        else
                        {
                            errorString = values[0];
                            intError = 15;
                        }
                        mWorker.ReportProgress(intError);
                        mWorker.Dispose();
                        return;
                    }
                    if (values.Count() >= 6)
                    {
                        switch (DecryptString(values[5]))
                        {
                            case "S01A01": //新用户，强制修改登录密码
                                App.PwdState = 0.0;
                                break;
                            case "S01A02": //密码已经过期，强制修改登录密码
                                App.PwdState = 0.1;
                                break;
                            case "S01A03": //密码将于 {0} 天后过期  因為修改密碼被禁用 那麼展示密碼過期時間也是無用，且展示位置不好放
                                //labMsg=string.format(values[5],values[6]);
                                break;
                        }
                    }
                    WriteLog.WriteLogToFile("LoadInfo",
                        DecryptString(values[0]) + "\t" + DecryptString(values[1]) + "\t" + DecryptString(values[2]) +
                        "\t" + DecryptString(values[3]) + "\t" + DecryptString(values[4]) + "\t");
                    App.renterID = DecryptString(values[1]); //租户编码

                    //更新session中部门id, 用户ID,  角色ID
                    UserInfo userInfo = new UserInfo();
                    userInfo.UserID = App.Int64Parse(DecryptString(values[2]).ToString(), 0); //用户ID
                    App.Session.UserID = App.Int64Parse(DecryptString(values[2]).ToString(), 0);
                    userInfo.Account = DecryptString(values[4]); //登录座席 在後面數行代碼更新為座席號
                    userInfo.UserName = DecryptString(values[4]); //用户全名
                    userInfo.Password = PasswordBoxLoginPassword.Password.Trim();
                    App.Session.SessionID = DecryptString(values[3]); //登錄流水號ID--SessionID
                    App.Session.UserInfo = userInfo;

                    RoleInfo roleInfo = new RoleInfo();
                    //超級管理員 106{0}00000000001
                    //座席角色ID
                    roleInfo.ID = Convert.ToInt64(string.Format("106{0}00000000004", App.renterID));
                    roleInfo.Name = "System Agent";
                    App.Session.RoleInfo = roleInfo;

                    RentInfo rentInfo = new RentInfo();
                    //租户ID
                    rentInfo.ID = roleInfo.ID;
                    //租户编码
                    rentInfo.Token = DecryptString(values[1]);
                    App.Session.RentInfo = rentInfo;

                    WebRequest webRequest = new WebRequest();
                    webRequest.Code = (int)S3104Codes.AgentLoginValidate;
                    webRequest.Session = App.Session;
                    webRequest.ListData.Add(strUserAccount);
                    //Service31041Client client=new Service31041Client();
                    Service31041Client client = new Service31041Client(WebHelper.CreateBasicHttpBinding(App.Session),
                        WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service31041"));
                    WebReturn webReturn = new WebReturn();
                    webReturn = client.UMPClientOperation(webRequest);
                    client.Close();
                    if (!webReturn.Result)
                    {
                        intError = 16;
                        errorString = string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message);
                        mWorker.ReportProgress(intError);
                        mWorker.Dispose();
                        return;
                    }
                    string[] agentInfo = webReturn.Data.Split(new[] { ConstValue.SPLITER_CHAR },
                        StringSplitOptions.RemoveEmptyEntries);
                    App.CurrentOrg = agentInfo[0]; //机构ID

                    App.ListCtrolAgentInfos.Clear();
                    CtrolAgent ctrolagent = new CtrolAgent();
                    ctrolagent.AgentID = userInfo.UserID.ToString();
                    //ctrolagent.AgentName = userInfo.Account;
                    ctrolagent.AgentName = agentInfo[1];
                    App.Session.UserInfo.Account = agentInfo[1];
                    ctrolagent.AgentOrgID = App.CurrentOrg;
                    ctrolagent.AgentFullName = userInfo.UserName;
                    App.ListCtrolAgentInfos.Add(ctrolagent);

                    //从数据库读取语言包
                    App.InitLanguageInfos();

                    //更新部门信息
                    App.InitControledOrg(App.CurrentOrg);

                    //更新权限 
                    webRequest = new WebRequest();
                    webRequest.Code = (int)RequestCode.WSGetUserOptList;
                    webRequest.Session = App.Session;
                    //根据当前用户角色查权限  
                    webRequest.ListData.Add(App.Session.RoleInfo.ID.ToString());
                    webRequest.ListData.Add("31");
                    webRequest.ListData.Add("3104");

                    Service11012Client client11012 =
                        new Service11012Client(WebHelper.CreateBasicHttpBinding(App.Session),
                            WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service11012"));
                    webReturn = new WebReturn();
                    webReturn = client11012.DoOperation(webRequest);
                    client11012.Close();

                    #region 直接取T_11_003_00000 表里3104的所有权限 測試用

                    ////更新权限 ,当前直接取T_11_003_00000 表里3104的所有权限
                    //webRequest = new WebRequest();
                    //webRequest.Code = (int)S3104Codes.GetRoleOperationList;
                    //webRequest.Session = App.Session;
                    //webRequest.ListData.Add("3104");

                    ////client = new Service31041Client();,
                    //client = new Service31041Client(WebHelper.CreateBasicHttpBinding(App.Session), WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service31041"));
                    //webReturn = new WebReturn();
                    //webReturn = client.UMPClientOperation(webRequest);
                    //client.Close();

                    #endregion

                    if (!webReturn.Result)
                    {
                        intError = 18;
                        errorString = string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message);
                        mWorker.ReportProgress(intError);
                        mWorker.Dispose();
                        return;
                    }
                    WriteLog.WriteLogToFile("UserOptList ", webReturn.ListData.Count.ToString());
                    App.ListOperationInfos.Clear();
                    if (webReturn.ListData.Count > 0)
                    {
                        for (int i = 0; i < webReturn.ListData.Count; i++)
                        {
                            OperationReturn optReturn = XMLHelper.DeserializeObject<OperationInfo>(webReturn.ListData[i]);
                            if (!optReturn.Result)
                            {
                                intError = 20;
                                errorString = string.Format("Fail.\t{0}\t{1}", webReturn.Code, webReturn.Message);
                                mWorker.ReportProgress(intError);
                                mWorker.Dispose();
                                return;
                            }
                            OperationInfo optInfo = optReturn.Data as OperationInfo;
                            if (optInfo != null)
                            {
                                optInfo.Display = App.GetLanguageInfo(string.Format("FO{0}", optInfo.ID),
                                    optInfo.ID.ToString());
                                optInfo.Description = optInfo.Display;
                                App.ListOperationInfos.Add(optInfo);
                                WriteLog.WriteLogToFile(
                                    string.Format("UserOptList  {0}", App.ListOperationInfos.Count), optInfo.Display);
                            }
                        }
                    }

                    //设置退出\在线状态
                    if (logWays.Equals("N")) //如果强制登陆不需要再设置在线状态
                    {
                        clientListString = new List<string>();
                        loginClient = new Service00000Client(WebHelper.CreateBasicHttpBinding(App.Session),
                            WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service00000"));
                        clientListString.Add(DecryptString(values[1])); //租户编码
                        clientListString.Add(DecryptString(values[2])); //用户编码
                        clientListString.Add(App.Session.SessionID); //登录分配的SessionID
                        args = loginClient.OperationMethodA(43, clientListString); //43登录、42退出
                        string temp = DecryptString(args.StringReturn);
                    }

                    errorString = string.Empty;
                };
                //可以与界面交互，需调用ReportProgress方法
                mWorker.ProgressChanged += (s, pe) =>
                {
                    switch (intError)
                    {
                        //端口錯誤
                        case 2:
                            labMsg.Content = errorString;
                            ServerMsg.Content = errorString;
                            tabServerInfo.Focus();
                            break;
                        //提示信息
                        case 1:
                        case 4:
                        case 10:
                        case 11:
                            labMsg.Content = errorString;
                            break;
                        case 14:
                            labMsg.Content = errorString;
                            LoadCurrentInformation();
                            break;
                        //程序錯誤
                        case 6:
                        case 8:
                        case 12:
                        case 15:
                        case 16:
                        case 18:
                        case 20:
                            App.ShowExceptionMessage(errorString);
                            labMsg.Content = TryFindResource("ClientError").ToString();
                            LoadCurrentInformation();
                            MyWaiter.Visibility = Visibility.Collapsed;
                            break;
                        default:
                            break;
                    }
                };
                //当后台操作已完成、被取消或引发异常时发生
                mWorker.RunWorkerCompleted += (s, re) =>
                {
                    mWorker.Dispose();
                    ButtonLoginSystem.IsEnabled = true;
                    MyWaiter.Visibility = Visibility.Collapsed;
                    if (!string.IsNullOrEmpty(errorString))//如果有錯誤消息
                    {
                        return;
                    }
                    AgentIntelligentClient agentclient = new AgentIntelligentClient();
                    agentclient.Show();
                    this.Close();

                };
                mWorker.RunWorkerAsync(); //触发DoWork事件

            }
            catch (Exception ex)
            {
                labMsg.Content = string.Format("Fail.\t{0}", ex.Message.ToString());
                return;
            }
        }

        void ButtonLoginOptions_Click(object sender, RoutedEventArgs e)
        {
            ButtonLoginOptions.ContextMenu = null;

            ContextMenu cMenuLoginOptions = new ContextMenu();
            cMenuLoginOptions.Opacity = 1;

            try
            {
                MenuItem menuItem;
                menuItem = new MenuItem();
                menuItem.Header = logLangs[0];
                menuItem.DataContext = "ForceLog";
                menuItem.Style = (Style)Resources["MenuItemFontStyle"];
                if (logWays == "F")
                {
                    menuItem.IsChecked = true;
                }
                menuItem.Click += menuItem_Click;
                cMenuLoginOptions.Items.Add(menuItem);

                menuItem = new MenuItem();
                menuItem.Header = logLangs[1];
                menuItem.DataContext = "ClientStyle";
                //Image imageStyle=new Image();
                //imageStyle.Height = 16;
                //imageStyle.Width = 16;
                //imageStyle.Source = new BitmapImage(new Uri(System.IO.Path.Combine(), UriKind.Absolute));
                //menuItem.Icon = imageStyle;
                menuItem.Style = (Style)Resources["MenuItemFontStyle"];
                menuItem.Click += menuItem_Click;
                cMenuLoginOptions.Items.Add(menuItem);

                //将右键功能复制到左键
                cMenuLoginOptions.PlacementTarget = ButtonLoginOptions;
                cMenuLoginOptions.Placement = PlacementMode.Bottom;
                cMenuLoginOptions.IsOpen = true;

                ButtonLoginOptions.ContextMenu = cMenuLoginOptions;
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }

        }

        /// <summary>
        /// 皮膚更換
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void menuItemStyles_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MenuItem LMenuItemClicked = sender as MenuItem;
                LMenuItemClicked.IsChecked = true;
                App.Session.ThemeInfo.Name = LMenuItemClicked.DataContext.ToString();
                ThemeInfo themeInfo = new ThemeInfo();
                themeInfo.Name = LMenuItemClicked.DataContext.ToString();
                string Color = string.Empty;
                string xmlFileName = string.Empty;
                //xmlFileName = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"UMP.Client\UMPClientInit.xml");
                xmlFileName = Environment.CurrentDirectory + string.Format("\\UMPClientInit.xml");
                App.XmlPathString = xmlFileName;
                if (!App.IsExsitXml())
                {
                    //App.ShowExceptionMessage(string.Format("UMPClientInit.xml  Is Not Exist"));
                    App.ShowExceptionMessage(TryFindResource("LoseXmlFile").ToString());
                    return;
                }
                XmlDocument umpclientinitxml = new XmlDocument();
                umpclientinitxml.Load(xmlFileName);
                XmlNode xmllocalstyle = umpclientinitxml.SelectSingleNode("root/SupportStyle");
                foreach (XmlNode xlNode in xmllocalstyle)
                {
                    if (xlNode.Attributes["Name"].Value.ToString() == themeInfo.Name)
                    {
                        Color = xlNode.Attributes["Color"].Value.ToString();
                        App.Session.ThemeInfo.Color = Color;
                        themeInfo.Color = Color;
                    }
                }
                WriteXmlTheme(themeInfo);
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
            }
        }

        void menuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem LMenuItemClicked = sender as MenuItem;
            switch (LMenuItemClicked.DataContext.ToString())
            {
                case "ForceLog":
                    if (logWays == "N")
                    {
                        logWays = "F";
                        LMenuItemClicked.IsChecked = true;
                    }
                    else
                    {
                        logWays = "N";
                        LMenuItemClicked.IsChecked = false;
                    }
                    break;
                case "ClientStyle":
                    ContextMenu cMenuLoginOptions = new ContextMenu();
                    cMenuLoginOptions.Opacity = 1;
                    try
                    {
                        MenuItem menuItemStyles;
                        int i = 0;
                        foreach (LocalXmlTheme localTheme in App.ListSupportTheme)
                        {
                            menuItemStyles = new MenuItem();
                            menuItemStyles.Header = LoginXml[i].name;
                            menuItemStyles.DataContext = localTheme.Name;
                            if (App.Session.ThemeInfo.Name == localTheme.Name)
                            {
                                menuItemStyles.IsChecked = true;
                            }
                            menuItemStyles.Style = (Style)App.Current.Resources["MenuItemFontStyle"];
                            menuItemStyles.Click += menuItemStyles_Click;
                            cMenuLoginOptions.Items.Add(menuItemStyles);
                            i += 1;
                        }
                        //将右键功能复制到左键
                        cMenuLoginOptions.PlacementTarget = ButtonLoginOptions;
                        cMenuLoginOptions.Placement = PlacementMode.Bottom;
                        cMenuLoginOptions.IsOpen = true;

                        LMenuItemClicked.ContextMenu = cMenuLoginOptions;
                    }
                    catch (Exception ex)
                    {
                        App.ShowExceptionMessage(ex.Message);
                    }

                    break;
            }
        }


        /// <summary>
        /// 判断服务器地址是否有效
        /// </summary>
        /// <param name="AStrServerHost"></param>
        /// <returns></returns>
        private bool ServerHostIsIPAddress(string ServerAddress)
        {
            bool LBoolReturn = true;

            try
            {
                IPAddress LIPAddress = null;
                LBoolReturn = IPAddress.TryParse(ServerAddress, out LIPAddress);
                string[] ipStrings = ServerAddress.Split('.');
                if (ipStrings.Count() < 4) LBoolReturn = false;
            }
            catch { LBoolReturn = false; }

            return LBoolReturn;
        }

        private static bool ServerHostIsIPPort(string port)
        {
            bool isPort = false;
            int portNum;

            isPort = Int32.TryParse(port, out portNum);

            if (isPort && portNum >= 0 && portNum <= 65535)
            {
                isPort = true;
            }
            else
            {
                isPort = false;
            }
            return isPort;
        }


        #endregion

        #region Encryption and Decryption

        public static string EncryptString(string strSource)//加密
        {
            string strTemp = EncryptionAndDecryption.EncryptDecryptString(strSource,
             CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004),
             EncryptionAndDecryption.UMPKeyAndIVType.M004);
            return strTemp;
        }

        public static string DecryptString(string strSource)//解密
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

    }
}
