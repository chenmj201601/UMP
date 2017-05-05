using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.ServiceModel;
using System.Threading;
using System.Windows;
using PFShareClassesC;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.ResourceXmls;

namespace UMPS1110Demos
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : IEncryptable
    {
        private SessionInfo mSession;
        private ResourceXmlHelper mXmlHelper;
        private ObservableCollection<OperationItem> mListOperations;
        private Service00Helper mService00Helper;

        public MainWindow()
        {
            InitializeComponent();

            mListOperations = new ObservableCollection<OperationItem>();

            Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing;
            BtnTest.Click += BtnTest_Click;
            BtnGenerate.Click += BtnGenerate_Click;
            BtnDoOperation.Click += BtnDoOperation_Click;
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Init();

            ComboOperations.ItemsSource = mListOperations;
            InitOperationItems();

            WindowState = WindowState.Maximized;
            TxtHostAddress.Text = "192.168.4.166";
            TxtHostPort.Text = "8009";
            if (ComboOperations.Items.Count > 0)
            {
                ComboOperations.SelectedIndex = 0;
            }
        }

        void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (mService00Helper != null)
            {
                mService00Helper.Stop();
            }
        }


        #region Init and Load

        private void InitOperationItems()
        {
            mListOperations.Clear();
            OperationItem item = new OperationItem();
            item.Name = string.Format("GetServerName");
            item.Value = "G001";
            mListOperations.Add(item);
            item = new OperationItem();
            item.Name = string.Format("GetDiskInformation");
            item.Value = "G002";
            mListOperations.Add(item);
            item = new OperationItem();
            item.Name = string.Format("GetNetworkCards");
            item.Value = "G003";
            mListOperations.Add(item);
            item = new OperationItem();
            item.Name = string.Format("GetSubDirectory");
            item.Value = "G004";
            mListOperations.Add(item);
            item = new OperationItem();
            item.Name = string.Format("GetSubFiles");
            item.Value = "G007";
            mListOperations.Add(item);
        }

        private void Init()
        {
            mSession = new SessionInfo();
            mSession.SessionID = Guid.NewGuid().ToString();
            mSession.AppName = "UMPS1110Demo";
            mSession.LastActiveTime = DateTime.Now;

            RentInfo rentInfo = new RentInfo();
            rentInfo.ID = ConstValue.RENT_DEFAULT;
            rentInfo.Token = ConstValue.RENT_DEFAULT_TOKEN;
            rentInfo.Domain = "voicecyber.com";
            rentInfo.Name = "voicecyber";
            mSession.RentInfo = rentInfo;
            mSession.RentID = ConstValue.RENT_DEFAULT;

            UserInfo userInfo = new UserInfo();
            userInfo.UserID = ConstValue.USER_ADMIN;
            userInfo.Account = "administrator";
            userInfo.UserName = "Administrator";
            mSession.UserInfo = userInfo;
            mSession.UserID = ConstValue.USER_ADMIN;

            RoleInfo roleInfo = new RoleInfo();
            roleInfo.ID = ConstValue.ROLE_SYSTEMADMIN;
            roleInfo.Name = "System Admin";
            mSession.RoleInfo = roleInfo;
            mSession.RoleID = ConstValue.ROLE_SYSTEMADMIN;

            AppServerInfo serverInfo = new AppServerInfo();
            serverInfo.Protocol = "http";
            serverInfo.Address = "192.168.6.74";
            serverInfo.Port = 8081;
            serverInfo.SupportHttps = false;
            mSession.AppServerInfo = serverInfo;

            //AppServerInfo serverInfo = new AppServerInfo();
            //serverInfo.Protocol = "http";
            //serverInfo.Address = "192.168.8.100";
            //serverInfo.Port = 8081;
            //serverInfo.SupportHttps = false;
            //mSession.AppServerInfo = serverInfo;

            ThemeInfo themeInfo = new ThemeInfo();
            themeInfo.Name = "Default";
            themeInfo.Color = "Brown";
            mSession.ThemeInfo = themeInfo;
            mSession.ThemeName = "Default";

            themeInfo = new ThemeInfo();
            themeInfo.Name = "Style01";
            themeInfo.Color = "Green";
            mSession.SupportThemes.Add(themeInfo);

            themeInfo = new ThemeInfo();
            themeInfo.Name = "Style02";
            themeInfo.Color = "Yellow";
            mSession.SupportThemes.Add(themeInfo);

            themeInfo = new ThemeInfo();
            themeInfo.Name = "Style03";
            themeInfo.Color = "Brown";
            mSession.SupportThemes.Add(themeInfo);

            themeInfo = new ThemeInfo();
            themeInfo.Name = "Style04";
            themeInfo.Color = "Blue";
            mSession.SupportThemes.Add(themeInfo);

            LangTypeInfo langType = new LangTypeInfo();
            langType.LangID = 1033;
            langType.LangName = "en-us";
            langType.Display = "English";
            mSession.SupportLangTypes.Add(langType);

            langType = new LangTypeInfo();
            langType.LangID = 2052;
            langType.LangName = "zh-cn";
            langType.Display = "简体中文";
            mSession.SupportLangTypes.Add(langType);
            mSession.LangTypeInfo = langType;
            mSession.LangTypeID = langType.LangID;

            langType = new LangTypeInfo();
            langType.LangID = 1028;
            langType.LangName = "zh-cn";
            langType.Display = "繁体中文";
            mSession.SupportLangTypes.Add(langType);

            //DatabaseInfo dbInfo = new DatabaseInfo();
            //dbInfo.TypeID = 3;
            //dbInfo.TypeName = "ORCL";
            //dbInfo.Host = "192.168.4.182";
            //dbInfo.Port = 1521;
            //dbInfo.DBName = "PFOrcl";
            //dbInfo.LoginName = "PFDEV831";
            //dbInfo.Password = "pfdev831";
            //mSession.DatabaseInfo = dbInfo;
            //mSession.DBType = dbInfo.TypeID;
            //mSession.DBConnectionString = dbInfo.GetConnectionString();

            DatabaseInfo dbInfo = new DatabaseInfo();
            dbInfo.TypeID = 2;
            dbInfo.TypeName = "MSSQL";
            dbInfo.Host = "192.168.4.182";
            dbInfo.Port = 1433;
            dbInfo.DBName = "UMPDataDB0909";
            dbInfo.LoginName = "PFDEV";
            dbInfo.Password = "PF,123";
            dbInfo.RealPassword = "PF,123";
            mSession.DatabaseInfo = dbInfo;
            mSession.DBType = dbInfo.TypeID;
            mSession.DBConnectionString = dbInfo.GetConnectionString();

            mSession.InstallPath = @"C:\UMPRelease";

            mSession.ListPartitionTables.Clear();
            PartitionTableInfo partInfo = new PartitionTableInfo();
            partInfo.TableName = ConstValue.TABLE_NAME_RECORD;
            partInfo.PartType = TablePartType.DatetimeRange;
            partInfo.Other1 = ConstValue.TABLE_FIELD_NAME_RECORD_STARTRECORDTIME;
            mSession.ListPartitionTables.Add(partInfo);
        }

        #endregion


        #region EventHandlers

        void BtnTest_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                AppServerInfo info = new AppServerInfo();
                info.Protocol = "http";
                info.Address = "192.168.6.75";
                info.Port = 8081;
                WebRequest webRequest = new WebRequest();
                webRequest.Session = new SessionInfo();
                //BasicHttpSecurity security=new BasicHttpSecurity();
                //security.Mode=BasicHttpSecurityMode.Transport;
                //security.Transport.ClientCredentialType=HttpClientCredentialType.None;
                BasicHttpBinding binding = new BasicHttpBinding();
                //binding.Security = security;
                binding.MaxReceivedMessageSize = int.MaxValue;
                binding.MaxBufferSize = int.MaxValue;
                binding.MaxBufferPoolSize = int.MaxValue;
                binding.SendTimeout = new TimeSpan(0, 10, 0);
                binding.ReceiveTimeout = new TimeSpan(0, 20, 0);
                string url = string.Format("{0}://{1}:{2}/WcfServices/{3}.svc",
                    info.Protocol,
                    info.Address,
                    info.Port,
                    "Service11011");
                EndpointAddress address = new EndpointAddress(new Uri(url, UriKind.Absolute));

                //Service11011Client client = new Service11011Client(
                //    binding,address);
                ////Service11012Client client = new Service11012Client();
                //WebReturn webReturn = client.DoOperation(webRequest);
                //client.Close();
                //if (!webReturn.Result)
                //{
                //    AppendMessage(webReturn.Message);
                //}
            }
            catch (Exception ex)
            {
                AppendMessage(ex.Message);
            }
        }

        void BtnDoOperation_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var opt = ComboOperations.SelectedItem as OperationItem;
                if (opt == null) { return; }
                string command = opt.Value;
                string args = TxtParams.Text;
                string[] arrArgs = args.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                List<string> listArgs = new List<string>();
                switch (command)
                {
                    case RequestCommand.GET_HOST_NAME:

                        break;
                    case RequestCommand.GET_DISK_INFO:
                        listArgs.Add("1");
                        break;
                    case RequestCommand.GET_SUBDIRECTORY:
                        if (arrArgs.Length < 1)
                        {
                            AppendMessage(string.Format("Param is null"));
                            return;
                        }
                        listArgs.Add(arrArgs[0]);
                        break;
                    case RequestCommand.GET_NETWORK_CARD:

                        break;
                    case RequestCommand.GET_SUBFILE:
                        if (arrArgs.Length < 1)
                        {
                            AppendMessage(string.Format("Param is null"));
                            return;
                        }
                        listArgs.Add(arrArgs[0]);
                        break;
                    default:
                        return;
                }
                if (mService00Helper == null)
                {
                    mService00Helper = new Service00Helper();
                    mService00Helper.Debug += mService00Helper_Debug;
                    mService00Helper.Start();
                }
                mService00Helper.HostAddress = TxtHostAddress.Text;
                mService00Helper.HostPort = int.Parse(TxtHostPort.Text);
                ThreadPool.QueueUserWorkItem(s =>
                {
                    OperationReturn optReturn = mService00Helper.DoOperation(command, listArgs);
                    if (!optReturn.Result)
                    {
                        AppendMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                        return;
                    }
                    string strMessage = optReturn.Data.ToString();
                    AppendMessage(string.Format("{0}", strMessage));
                });
            }
            catch (Exception ex)
            {
                AppendMessage(string.Format("Fail.\t{0}", ex.Message));
            }
        }

        void BtnGenerate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                mXmlHelper = new ResourceXmlHelper(mSession);
                mXmlHelper.EncryptObject = this;
                OperationReturn optReturn = mXmlHelper.Init();
                if (!optReturn.Result)
                {
                    AppendMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "test.xml");
                //optReturn = mXmlHelper.GenerateTypeXmlFile(221, path);
                optReturn = mXmlHelper.GenerateAllResourceXmlFile(path);
                //optReturn = mXmlHelper.GenerateTypeXmlFile(221, path,
                //    GenerateOption.Default | GenerateOption.IgnoreChannel);
                mXmlHelper.CleanData();
                if (!optReturn.Result)
                {
                    AppendMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                    return;
                }
                AppendMessage(string.Format("End.\t{0}", optReturn.Message));
            }
            catch (Exception ex)
            {
                AppendMessage(ex.Message);
            }
        }

        void mService00Helper_Debug(string msg)
        {
            AppendMessage(string.Format("Service00Helper\t{0}", msg));
        }

        #endregion


        #region Others

        private void AppendMessage(string msg)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                TxtMsg.AppendText(string.Format("{0}\t{1}\r\n", DateTime.Now.ToString("HH:mm:ss"), msg));
                TxtMsg.ScrollToEnd();
            }));
        }

        #endregion


        #region Encryption and Decryption

        public string EncryptString(string strSource)
        {
            string strReturn = string.Empty;
            string strTemp;
            do
            {
                if (strSource.Length > 128)
                {
                    strTemp = strSource.Substring(0, 128);
                    strSource = strSource.Substring(128, strSource.Length - 128);
                }
                else
                {
                    strTemp = strSource;
                    strSource = string.Empty;
                }
                strReturn += EncryptionAndDecryption.EncryptDecryptString(strTemp,
                    CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004),
                    EncryptionAndDecryption.UMPKeyAndIVType.M004);
            } while (strSource.Length > 0);
            return strReturn;
        }

        public string DecryptString(string strSource)
        {
            string strReturn = string.Empty;
            string strTemp;
            do
            {
                if (strSource.Length > 512)
                {
                    strTemp = strSource.Substring(0, 512);
                    strSource = strSource.Substring(512, strSource.Length - 512);
                }
                else
                {
                    strTemp = strSource;
                    strSource = string.Empty;
                }
                strReturn += EncryptionAndDecryption.EncryptDecryptString(strTemp,
                    CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104),
                    EncryptionAndDecryption.UMPKeyAndIVType.M104);
            } while (strSource.Length > 0);
            return strReturn;
        }

        public string DecryptString(string strSource, int mode)
        {
            string strReturn = string.Empty;
            string strTemp;
            do
            {
                if (strSource.Length > 128)
                {
                    strTemp = strSource.Substring(0, 128);
                    strSource = strSource.Substring(128, strSource.Length - 128);
                }
                else
                {
                    strTemp = strSource;
                    strSource = string.Empty;
                }
                strReturn += EncryptionAndDecryption.EncryptDecryptString(strTemp,
                    CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004),
                    EncryptionAndDecryption.UMPKeyAndIVType.M004);
            } while (strSource.Length > 0);
            return strReturn;
        }

        public string EncryptString(string strSource, int mode)
        {
            string strReturn = string.Empty;
            string strTemp;
            switch (mode)
            {
                case (int)EncryptionMode.AES256V01Hex:
                    do
                    {
                        if (strSource.Length > 512)
                        {
                            strTemp = strSource.Substring(0, 512);
                            strSource = strSource.Substring(512, strSource.Length - 512);
                        }
                        else
                        {
                            strTemp = strSource;
                            strSource = string.Empty;
                        }
                        strReturn += EncryptionAndDecryption.EncryptDecryptString(strTemp,
                            CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M001),
                            EncryptionAndDecryption.UMPKeyAndIVType.M001);
                    } while (strSource.Length > 0);
                    break;
                case (int)EncryptionMode.SHA256V00Hex:
                    strReturn = EncryptionAndDecryption.EncryptStringSHA256(strSource);
                    break;
            }
            return strReturn;
        }

        public string CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType aKeyIVID)
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

        public byte[] DecryptBytes(byte[] source, int mode)
        {
            return source;
        }

        public byte[] DecryptBytes(byte[] source)
        {
            return source;
        }

        public string DecryptString(string source, int mode, System.Text.Encoding encoding)
        {
            return source;
        }

        public byte[] EncryptBytes(byte[] source, int mode)
        {
            return source;
        }

        public byte[] EncryptBytes(byte[] source)
        {
            return source;
        }

        public string EncryptString(string source, int mode, System.Text.Encoding encoding)
        {
            return source;
        }

        #endregion
       
    }
}
