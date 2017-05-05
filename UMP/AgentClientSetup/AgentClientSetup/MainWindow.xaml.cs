using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;

namespace AgentClientSetup
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern int GetUserDefaultUILanguage();//获取当前系统使用的语言ID

        /// <summary>
        /// 是否使用http
        /// </summary>
        private static bool defaultHttp = true;


        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
            border01.MouseLeftButtonDown += border01_MouseLeftButtonDown;
            //ButtonMenu.Click += ButtonMenu_Click;
            ButtonClose.Click += butCancel_Click;
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //InitLanguages();
            txPort.Text = "8081";
        }

        #region 触发事件

        void ButtonMenu_Click(object sender, RoutedEventArgs e)
        {
            Button ClickedButton = sender as Button;
            //目标   
            ClickedButton.ContextMenu.PlacementTarget = ClickedButton;
            //位置   
            ClickedButton.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
            //显示菜单   
            ClickedButton.ContextMenu.IsOpen = true;
        }

        void border01_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void butOk_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if(string.IsNullOrWhiteSpace(txAdd.Text) || string.IsNullOrWhiteSpace(txPort.Text))
                {
                    MessageBox.Show("ServerInfoIsNull", "Warning", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
                if(!ServerHostIsIPAddress(txAdd.Text))
                {
                    MessageBox.Show("AddressIsError", "Warning", MessageBoxButton.OK, MessageBoxImage.Information);
                    txAdd.Focus();
                    return;
                }
                if(!ServerHostIsIPPort(txPort.Text))
                {
                    txPort.Focus();
                    MessageBox.Show("PortIsError", "Warning", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
                butCancel.IsEnabled = false;
                butOk.IsEnabled = false;
                GetCertificateHash();
                string temStr=GetCertificateInfo();
                if(string.IsNullOrWhiteSpace(temStr))
                {
                    MessageBox.Show("InstallSuccessed", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    if (defaultHttp)
                    {
                        WriteLog.WriteLogToFile("defaultHttp", "\t" + string.Format("serverAdd:{0}  Port:{1}", txAdd.Text, txPort.Text)); 
                    }
                    else
                    {
                        WriteLog.WriteLogToFile("Install Successed", "\t" + string.Format("serverAdd:{0}  Port:{1}", txAdd.Text, txPort.Text));
                    }
                    
                    WriteLog.CreatServerInfoXml(txAdd.Text,txPort.Text);
                    this.Close();
                }
                else
                {
                    MessageBox.Show(temStr, "Warning".ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
                    WriteLog.WriteLogToFile("InstallFailed", "\t" + string.Format("{0} serverAdd:{1}  Port:{2}", temStr, txAdd.Text, txPort.Text));
                    butCancel.IsEnabled = true;
                    butOk.IsEnabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Warning", MessageBoxButton.OK, MessageBoxImage.Error);
                WriteLog.WriteLogToFile("InstallFailed", "\t" + string.Format("{0} serverAdd:{1}  Port:{2}", ex.Message,txAdd.Text,txPort.Text));
                butCancel.IsEnabled = true;
                butOk.IsEnabled = true;
            }
        }

        private void butCancel_Click(object sender, RoutedEventArgs e)
        {
            //if (MessageBox.Show("取消安装？", "注意", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.OK)
            //{
                     
            //}
            //else
            //{
            //    return;
            //}
            System.Environment.Exit(System.Environment.ExitCode);    
        }

        #endregion
        
        /// <summary>
        /// 判断服务器地址是否有效
        /// </summary>
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
            try
            {
                isPort = Int32.TryParse(port, out portNum);
                if (isPort && portNum >= 0 && portNum <= 65535)
                {
                    isPort = true;
                }
                else
                {
                    isPort = false;
                }
            }
            catch (Exception ex)
            {
                
            }
            
            return isPort;
        }

        /// <summary>
        /// 加載本地語言包  根據系統語言自動匹配
        /// </summary>
        void InitLanguages()
        {
            try
            {
                int localmachinelanguageid = GetUserDefaultUILanguage();
                string languagefileName = Environment.CurrentDirectory;
                if (localmachinelanguageid == 2052 || localmachinelanguageid == 1028|| localmachinelanguageid == 1033 || localmachinelanguageid == 1041)
                {
                }
                else
                {
                    localmachinelanguageid=1033;
                }
                languagefileName += string.Format("\\Languages\\{0}.xaml", localmachinelanguageid);
                this.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri(languagefileName, UriKind.RelativeOrAbsolute) });   

            }
            catch (Exception ex)
            {
                
            }
            return;
        }

        #region 下载证书主体

        /// <summary>
        /// 获取证书，存放本地
        /// 返回值為空 操作正常，否則返回ex
        /// StoreName， 0 僅下載證書，1，2 安裝到StoreName.Root，3 安裝到StoreName.TrustedPublisher
        /// </summary>
        /// <returns></returns>
        private string GetCertificateInfo()
        {
            string Result = string.Empty;
            HttpWebRequest LHttpWebRequest = null;
            Stream LStreamResponse = null;
            System.IO.FileStream LStreamCertificateFile = null;
            string LStrCertificateFileFullName = string.Empty;
            string CertificateName = string.Format(@"UMP.S.{0}.pfx", txAdd.Text);
            LStrCertificateFileFullName = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), string.Format(@"UMP.Client\UMP.S.{0}.pfx", txAdd.Text));
            if (defaultHttp) { return string.Empty; }//如果走http就不需要下载、安装证书
            try
            {
                if (System.IO.File.Exists(LStrCertificateFileFullName))
                {
                    System.IO.FileInfo objInfo = new FileInfo(LStrCertificateFileFullName);
                    long len = objInfo.Length;
                    if (len <= 0)
                    {
                        System.IO.File.Delete(LStrCertificateFileFullName);
                        Result = GetCertificateInfo();
                        if (!string.IsNullOrWhiteSpace(Result))
                        {
                            return Result;
                        }
                    }
                    return Result;
                }
                LStreamCertificateFile = new FileStream(LStrCertificateFileFullName, FileMode.Create);
                LHttpWebRequest = (HttpWebRequest)HttpWebRequest.Create("http://" + txAdd.Text + ":" + txPort.Text + "/Components/Certificates/" + CertificateName);
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
                Result = InstallCertificateToStore(StoreName.Root);
                if (!string.IsNullOrWhiteSpace(Result)) WriteLog.WriteLogToFile("(ಥ_ಥ) InstallCertificateToRoot" , "\t" + Result);
                Result = InstallCertificateToStore(StoreName.TrustedPublisher);
                if (!string.IsNullOrWhiteSpace(Result)) WriteLog.WriteLogToFile("(ಥ_ಥ) InstallCertificateToTrustedPublisher",   "\t" + Result);
            }
            catch (Exception ex)
            {
                Result = string.Format(ex.Message);
                //MessageBox.Show(ex.Message);
            }
            finally
            {
                if (LHttpWebRequest != null) { LHttpWebRequest.Abort(); }
                if (LStreamResponse != null) { LStreamResponse.Close(); LStreamResponse.Dispose(); }
                if (LStreamCertificateFile != null) { LStreamCertificateFile.Close(); LStreamCertificateFile.Dispose(); }
                //System.IO.File.Delete(LStrCertificateFileFullName);
            }
            return Result;
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
                        string.Format(@"UMP.Client\UMP.S.{0}.pfx", txAdd.Text));

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
            //finally
            //{
            //    string XmlName = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), string.Format(@"UMP.Client\UMP.Server.01.xml"));
            //    System.IO.File.Delete(XmlName);
            //}
            return LStrReturn;

        }

        /// <summary>
        /// 获取证书Hash值
        /// 返回值为空即服务器地址问题
        /// </summary>
        private string GetCertificateHash()
        {
            Stopwatch watch = new Stopwatch();
            string HashValue = string.Empty;
            HttpWebRequest LHttpWebRequest = null;
            Stream LStreamResponse = null;
            System.IO.FileStream LStreamCertificateFile = null;
            string LStrCertificateFileFullName = string.Empty;
            string XmlName = string.Format(@"UMP.Client\UMP.Server.01.xml");
            LStrCertificateFileFullName = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), XmlName);
            string TempFile = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), string.Format(@"UMP.Client"));
            string time = string.Empty;
            try
            {
                if (!Directory.Exists(TempFile))
                {
                    Directory.CreateDirectory(TempFile);
                }
                watch.Start();
                LStreamCertificateFile = new FileStream(LStrCertificateFileFullName, FileMode.Create);
                LHttpWebRequest = (HttpWebRequest)HttpWebRequest.Create("http://" + txAdd.Text + ":" + txPort.Text + "/GlobalSettings/UMP.Server.01.xml");
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


                watch.Restart();
                //获取xml中的Hash值
                XmlDocument umpclientinitxml = new XmlDocument();
                System.IO.FileInfo objInfo = new FileInfo(LStrCertificateFileFullName);
                long len = objInfo.Length;
                if (len <= 0)//下載下來的xml文件有問題
                {
                    HashValue = GetCertificateHash();
                    if (HashValue == string.Empty )
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
                            defaultHttp = false;
                        }
                        else
                        {
                            defaultHttp = true;
                        }
                    }
                }
                watch.Stop();
                time = watch.ElapsedMilliseconds.ToString();
                WriteLog.WriteLogToFile("獲取Hash值", time);

                if (string.IsNullOrWhiteSpace(HashValue))
                {
                    GetCertificateHash();
                    WriteLog.WriteLogToFile("獲取Hash值为空，重新获取UMP.Server.01.xml文件", LStrCertificateFileFullName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Download UMP.Server.01.xml Failed", "Warning", MessageBoxButton.OK, MessageBoxImage.Error);
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

        #endregion
        
    }
}
