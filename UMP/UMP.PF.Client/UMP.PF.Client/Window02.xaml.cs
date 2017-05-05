using IWshRuntimeLibrary;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;
using UMP.PF.Client.Wcf00000.Service00000;

namespace UMP.PF.Client
{
    public partial class Window02 : Window
    {
        private BackgroundWorker InstanceBackgroundWorkerInstalling = null;

        bool IBoolInInstalling = false;

        string IStrAppServer = string.Empty;
        string IStrServerPort = string.Empty;

        List<bool> IListBoolInstallResult = new List<bool>();
        List<string> IListStrInstallResult = new List<string>();

        #region 从WCF服务获取的信息
        private string IStrAppShortName = string.Empty;
        private List<string> IListStrSurportLanguages = new List<string>();
        private DataTable IDataTableProtocolBind = new DataTable();
        private DataTable IDataTableTrustZones = new DataTable();
        #endregion

        #region DoWork过程中临时使用数据
        private string IStrTempA = string.Empty;
        #endregion

        public Window02()
        {
            InitializeComponent();
            this.MouseLeftButtonDown += Window02_MouseLeftButtonDown;
            this.Closing += Window02_Closing;
            this.Loaded += Window02_Loaded;
            ButtonCloseInstall.Click += ButtonClose_Click;
            ButtonCloseWindow.Click += ButtonClose_Click;
            ButtonApplicationMenu.Click += ButtonApplicationMenu_Click;
            ButtonInstall.Click += ButtonInstall_Click;
        }

        private void ButtonInstall_Click(object sender, RoutedEventArgs e)
        {
            string LStrAppServer = string.Empty;
            string LStrServerPort = string.Empty;
            int LIntPort = 0;

            try
            {
                if (IBoolInInstalling) { return; }

                TextBoxLogDetails.Text = string.Empty;

                LStrAppServer = ComboBoxServerName.Text.Trim();
                LStrServerPort = TextPort.Text.Trim();

                #region 判断服务器参数是否正确
                if (string.IsNullOrEmpty(LStrAppServer))
                {
                    MessageBox.Show(App.GetDisplayCharater("Window02", "N001"), this.Title);
                    ComboBoxServerName.Focus();
                    return;
                }
                if (!ServerHostIsIPAddress(LStrAppServer))
                {
                    if (LStrAppServer.Contains("."))
                    {
                        MessageBox.Show(App.GetDisplayCharater("Window02", "N001"), this.Title);
                        ComboBoxServerName.Focus();
                        return;
                    }
                }
                if (string.IsNullOrEmpty(LStrServerPort))
                {
                    MessageBox.Show(App.GetDisplayCharater("Window02", "N002"), this.Title);
                    TextPort.Focus(); return;
                }
                if (int.TryParse(LStrServerPort, out LIntPort) == false)
                {
                    MessageBox.Show(App.GetDisplayCharater("Window02", "N002"), this.Title);
                    TextPort.Focus(); return;
                }
                if (LIntPort <= 0)
                {
                    MessageBox.Show(App.GetDisplayCharater("Window02", "N002"), this.Title);
                    TextPort.Focus(); return;
                }
                #endregion

                IStrAppServer = LStrAppServer;
                IStrServerPort = LStrServerPort;

                IListBoolInstallResult.Clear();
                IListStrInstallResult.Clear();

                IBoolInInstalling = true;
                InstanceBackgroundWorkerInstalling = new BackgroundWorker();
                InstanceBackgroundWorkerInstalling.WorkerReportsProgress = true;
                InstanceBackgroundWorkerInstalling.RunWorkerCompleted += InstanceBackgroundWorkerInstalling_RunWorkerCompleted;
                InstanceBackgroundWorkerInstalling.DoWork += InstanceBackgroundWorkerInstalling_DoWork;
                InstanceBackgroundWorkerInstalling.ProgressChanged += InstanceBackgroundWorkerInstalling_ProgressChanged;
                InstanceBackgroundWorkerInstalling.RunWorkerAsync();
            }
            catch { }
        }

        private void InstanceBackgroundWorkerInstalling_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            int LIntCurrentStep = e.ProgressPercentage;

            if (LIntCurrentStep == 6 || LIntCurrentStep == 8 || LIntCurrentStep == 51 || LIntCurrentStep == 53)
            {
                LabelCurrentDoing.Content = string.Format(App.GetDisplayCharater("Window02", "InsStep" + LIntCurrentStep.ToString("00")), IStrTempA);
            }
            else
            {
                LabelCurrentDoing.Content = App.GetDisplayCharater("Window02", "InsStep" + LIntCurrentStep.ToString("00"));
            }
            if (IListBoolInstallResult.Count > 0)
            {
                if (IListBoolInstallResult[IListBoolInstallResult.Count - 1])
                {
                    TextBoxLogDetails.Text += App.GetDisplayCharater("Window02", "InsResult1") + "\n";
                }
                else
                {
                    TextBoxLogDetails.Text += App.GetDisplayCharater("Window02", "InsResult0") + "\n" + IListStrInstallResult[IListBoolInstallResult.Count - 1] + "\n";
                }
                TextBoxLogDetails.Text += "\n=======================================================\n\n";
            }
            TextBoxLogDetails.Text += LabelCurrentDoing.Content.ToString() + "\n";
        }

        private void InstanceBackgroundWorkerInstalling_DoWork(object sender, DoWorkEventArgs e)
        {
            string LStrCallReturn = string.Empty;
            List<string> LListStrCertificateFiles = new List<string>();
            List<string> LListStrCertificatePwd = new List<string>();
            List<string> LListStrCertificateHashString = new List<string>();

            BackgroundWorker LBackgroundWorker = sender as BackgroundWorker;

            #region 获取服务器配置的信息
            LBackgroundWorker.ReportProgress(1);
            LStrCallReturn = GetApplicationShortName();
            IListBoolInstallResult.Add(string.IsNullOrEmpty(LStrCallReturn));
            IListStrInstallResult.Add(LStrCallReturn);
            System.Threading.Thread.Sleep(500);

            if (!IListBoolInstallResult[0]) { return; }

            LBackgroundWorker.ReportProgress(2);
            LStrCallReturn = GetApplicationIco();
            IListBoolInstallResult.Add(string.IsNullOrEmpty(LStrCallReturn));
            IListStrInstallResult.Add(LStrCallReturn);
            System.Threading.Thread.Sleep(500);

            LBackgroundWorker.ReportProgress(3);
            LStrCallReturn = GetSurportLanguages();
            IListBoolInstallResult.Add(string.IsNullOrEmpty(LStrCallReturn));
            IListStrInstallResult.Add(LStrCallReturn);
            System.Threading.Thread.Sleep(500);

            LBackgroundWorker.ReportProgress(4);
            LStrCallReturn = GetIISBindingProtocol();
            IListBoolInstallResult.Add(string.IsNullOrEmpty(LStrCallReturn));
            IListStrInstallResult.Add(LStrCallReturn);
            System.Threading.Thread.Sleep(500);

            LBackgroundWorker.ReportProgress(5);
            LStrCallReturn = GetSettedTrustZones();
            IListBoolInstallResult.Add(string.IsNullOrEmpty(LStrCallReturn));
            IListStrInstallResult.Add(LStrCallReturn);
            System.Threading.Thread.Sleep(500);
            #endregion

            #region 下载安全证书
            LBackgroundWorker.ReportProgress(6);
            IStrTempA = "UMP.PF.Certificate.pfx";
            LListStrCertificateFiles.Add("UMP.PF.Certificate.pfx");
            LListStrCertificatePwd.Add("VoiceCyber,123");
            LListStrCertificateHashString.Add("2D508A175B6836ADB6E220BA63E00F2F0881E75F");
            LStrCallReturn = DownloadUMPCertificate("UMP.PF.Certificate.pfx");
            IListBoolInstallResult.Add(string.IsNullOrEmpty(LStrCallReturn));
            IListStrInstallResult.Add(LStrCallReturn);
            System.Threading.Thread.Sleep(500);

            DataRow[] LDataRowProtocolBind = IDataTableProtocolBind.Select("Activated = '1' AND Protocol = 'https'");
            foreach (DataRow LDataRowSingleProtocolBind in LDataRowProtocolBind)
            {
                string LStrBindingAddress = LDataRowSingleProtocolBind["IPAddress"].ToString();
                if (LStrBindingAddress.ToUpper() != IStrAppServer.ToUpper()) { continue; }
                string LStrCertificateHashString = LDataRowSingleProtocolBind["OtherArgs"].ToString();
                string LStrCertificateFileName = "UMP.S." + IStrAppServer + ".pfx";
                IStrTempA = LStrCertificateFileName;
                LListStrCertificateFiles.Add(LStrCertificateFileName);
                LListStrCertificatePwd.Add("VoiceCyber,123");
                LListStrCertificateHashString.Add(LStrCertificateHashString);
                LBackgroundWorker.ReportProgress(6);
                LStrCallReturn = DownloadUMPCertificate(LStrCertificateFileName);
                IListBoolInstallResult.Add(string.IsNullOrEmpty(LStrCallReturn));
                IListStrInstallResult.Add(LStrCallReturn);
                System.Threading.Thread.Sleep(500);
            }
            #endregion

            #region 下载Mage.exe
            LBackgroundWorker.ReportProgress(7);
            LStrCallReturn = DownloadMageTool();
            IListBoolInstallResult.Add(string.IsNullOrEmpty(LStrCallReturn));
            IListStrInstallResult.Add(LStrCallReturn);
            System.Threading.Thread.Sleep(500);
            #endregion

            #region 下载ReportViewer组件
            foreach (string LStrSingleLanguageID in IListStrSurportLanguages)
            {
                IStrTempA = LStrSingleLanguageID;
                LBackgroundWorker.ReportProgress(8);
                LStrCallReturn = DownloadReportViewerRuntime(LStrSingleLanguageID);
                IListBoolInstallResult.Add(string.IsNullOrEmpty(LStrCallReturn));
                IListStrInstallResult.Add(LStrCallReturn);
                System.Threading.Thread.Sleep(500);
            }
            #endregion

            #region 下载Style组件
            //LBackgroundWorker.ReportProgress(9);
            //DownloadStyleFiles();
            //IListBoolInstallResult.Add(string.IsNullOrEmpty(LStrCallReturn));
            //IListStrInstallResult.Add(LStrCallReturn);
            //System.Threading.Thread.Sleep(500);
            #endregion

            #region 判断目前是否支持该版本的操作系统
            LStrCallReturn = VerificationSystemVersion();
            if (!string.IsNullOrEmpty(LStrCallReturn))
            {
                LBackgroundWorker.ReportProgress(11);
                System.Threading.Thread.Sleep(500);
                return;
            }
            #endregion

            #region 判断所有的组件是否已经全部下载成功
            foreach (bool LBoolGetted in IListBoolInstallResult)
            {
                if (!LBoolGetted)
                {
                    LBackgroundWorker.ReportProgress(21);
                    System.Threading.Thread.Sleep(500);
                    return;
                }
            }
            #endregion

            #region 安装安全证书组件
            int LIntAllCertificateFiles = LListStrCertificateFiles.Count;
            for (int LIntLoopCertificateFile = 0; LIntLoopCertificateFile < LIntAllCertificateFiles; LIntLoopCertificateFile++)
            {
                IStrTempA = LListStrCertificateFiles[LIntLoopCertificateFile];
                LBackgroundWorker.ReportProgress(51);
                LStrCallReturn = InstallCertificateFile(LListStrCertificateFiles[LIntLoopCertificateFile], LListStrCertificatePwd[LIntLoopCertificateFile], LListStrCertificateHashString[LIntLoopCertificateFile]);
                IListBoolInstallResult.Add(string.IsNullOrEmpty(LStrCallReturn));
                IListStrInstallResult.Add(LStrCallReturn);
                System.Threading.Thread.Sleep(500);
            }
            #endregion

            #region 安装ReportViewer运行时组件
            LBackgroundWorker.ReportProgress(52);
            LStrCallReturn = InstallCLRTypesComponent();
            IListBoolInstallResult.Add(string.IsNullOrEmpty(LStrCallReturn));
            IListStrInstallResult.Add(LStrCallReturn);

            LStrCallReturn = InstallReportViewerComponent();
            IListBoolInstallResult.Add(string.IsNullOrEmpty(LStrCallReturn));
            IListStrInstallResult.Add(LStrCallReturn);
            System.Threading.Thread.Sleep(500);
            #endregion

            #region 添加信任站点
            IStrTempA = IStrAppServer;
            LBackgroundWorker.ReportProgress(53);
            LStrCallReturn = SetTrustWebSite();
            IListBoolInstallResult.Add(string.IsNullOrEmpty(LStrCallReturn));
            IListStrInstallResult.Add(LStrCallReturn);
            System.Threading.Thread.Sleep(500);
            #endregion

            #region 将服务器配置信息写入本地文件
            LBackgroundWorker.ReportProgress(54);
            LStrCallReturn = SaveAppServerProtocolBind();
            IListBoolInstallResult.Add(string.IsNullOrEmpty(LStrCallReturn));
            IListStrInstallResult.Add(LStrCallReturn);
            System.Threading.Thread.Sleep(500);
            #endregion

            #region 创建快捷方式
            LBackgroundWorker.ReportProgress(55);
            LStrCallReturn = CreateShortCutInThisComputer();
            IListBoolInstallResult.Add(string.IsNullOrEmpty(LStrCallReturn));
            IListStrInstallResult.Add(LStrCallReturn);
            System.Threading.Thread.Sleep(500);
            #endregion

            //#region 创建清除UMP联机应用程序缓存
            //LBackgroundWorker.ReportProgress(56);
            //LStrCallReturn = CreateApplicationCacheClearShortCut();
            //IListBoolInstallResult.Add(string.IsNullOrEmpty(LStrCallReturn));
            //IListStrInstallResult.Add(LStrCallReturn);
            //System.Threading.Thread.Sleep(500);
            //#endregion

            #region 修改注册表，把自动更新根证书关闭

            ModifyAuthRootValue();

            #endregion

            LBackgroundWorker.ReportProgress(99);
            System.Threading.Thread.Sleep(500);
        }

        private void InstanceBackgroundWorkerInstalling_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            bool LBoolInstallExistsFailed = false;
            string LStrMessageID = "N006";

            IBoolInInstalling = false;
            LabelCurrentDoing.Content = string.Empty;
            if (InstanceBackgroundWorkerInstalling != null)
            {
                InstanceBackgroundWorkerInstalling.Dispose(); InstanceBackgroundWorkerInstalling = null;
            }

            foreach (bool LBoolInstallResult in IListBoolInstallResult)
            {
                if (!LBoolInstallResult) { LBoolInstallExistsFailed = true; LStrMessageID = "N005"; break; }
            }
            MessageBox.Show(App.GetDisplayCharater("Window02", LStrMessageID), IStrAppShortName, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        #region 安装客户端组件所有调用的方法
        /// <summary>
        /// 获取该应用程序简称
        /// </summary>
        /// <returns>如果为空，表示获取成功，值保存在IStrAppShortName中，否则为错误信息</returns>
        private string GetApplicationShortName()
        {
            string LStrReturn = string.Empty;
            Service00000Client LWcfClient = null;
            List<string> LListStrArgs = new List<string>();

            try
            {
                BasicHttpBinding LBinding = CreateBasicHttpBinding();
                EndpointAddress LEndAddress = CreateEndpointAddress(IStrAppServer, IStrServerPort, "Service00000");
                OperationDataArgs LOperationReturn = new OperationDataArgs();
                LWcfClient = new Service00000Client(LBinding, LEndAddress);
                LOperationReturn = LWcfClient.OperationMethodA(1, LListStrArgs);
                LWcfClient.Close();
                if (LOperationReturn.BoolReturn)
                {
                    IStrAppShortName = LOperationReturn.StringReturn;
                }
                else
                {
                    LStrReturn = LOperationReturn.StringReturn;
                }
            }
            catch (Exception ex)
            {
                LStrReturn = ex.ToString();
            }
            finally
            {
                if (LWcfClient != null)
                {
                    if (LWcfClient.State == CommunicationState.Opened) { LWcfClient.Close(); }
                }
            }

            return LStrReturn;
        }

        /// <summary>
        /// 获取该应用程序的图标
        /// </summary>
        /// <returns>如果为空，表示获取成功，值保存在系统目录（system）中，文件名为: UMPAppLogo.ico，否则为错误信息</returns>
        private string GetApplicationIco()
        {
            string LStrReturn = string.Empty;
            HttpWebRequest LHttpWebRequest = null;
            Stream LStream = null;

            try
            {
                if (System.IO.File.Exists(App.GStrSystemRoot + @"\UMPAppLogo.ico")) { return string.Empty; }
                FileStream LFileStream = new FileStream(App.GStrSystemRoot + @"\UMPAppLogo.ico", FileMode.Create);
                LHttpWebRequest = (HttpWebRequest)HttpWebRequest.Create("http://" + IStrAppServer + ":" + IStrServerPort + "/Components/Images/ApplicationLogo.ico");
                long LContenctLength = LHttpWebRequest.GetResponse().ContentLength;
                LHttpWebRequest.AddRange(0);

                LStream = LHttpWebRequest.GetResponse().GetResponseStream();

                byte[] LbyteRead = new byte[1024];
                int LIntReadedSize = LStream.Read(LbyteRead, 0, 1024);
                while (LIntReadedSize > 0)
                {
                    LFileStream.Write(LbyteRead, 0, LIntReadedSize);
                    LIntReadedSize = LStream.Read(LbyteRead, 0, 1024);
                }
                LFileStream.Close(); LFileStream.Dispose();
            }
            catch (Exception ex)
            {
                LStrReturn = ex.ToString();
            }
            finally
            {
                if (LHttpWebRequest != null) { LHttpWebRequest.Abort(); }
                if (LStream != null) { LStream.Close(); LStream.Dispose(); }
            }

            return LStrReturn;
        }

        /// <summary>
        /// 获取UMP支持的语言列表
        /// </summary>
        /// <returns>如果为空，表示获取成功，值保存在 IListStrSurportLanguages 中，否则为错误信息</returns>
        private string GetSurportLanguages()
        {
            string LStrReturn = string.Empty;
            Service00000Client LWcfClient = null;
            List<string> LListStrArgs = new List<string>();

            try
            {
                BasicHttpBinding LBinding = CreateBasicHttpBinding();
                EndpointAddress LEndAddress = CreateEndpointAddress(IStrAppServer, IStrServerPort, "Service00000");
                OperationDataArgs LOperationReturn = new OperationDataArgs();
                LWcfClient = new Service00000Client(LBinding, LEndAddress);
                LOperationReturn = LWcfClient.OperationMethodA(3, LListStrArgs);
                LWcfClient.Close();
                if (LOperationReturn.BoolReturn)
                {
                    IListStrSurportLanguages.Clear();
                    IListStrSurportLanguages.Add("0000");
                    IListStrSurportLanguages.Add("0001");
                    foreach (string LStrLanguageID in LOperationReturn.ListStringReturn) { IListStrSurportLanguages.Add(LStrLanguageID); }
                }
                else
                {
                    LStrReturn = LOperationReturn.StringReturn;
                }
            }
            catch (Exception ex)
            {
                LStrReturn = ex.ToString();
            }
            finally
            {
                if (LWcfClient != null)
                {
                    if (LWcfClient.State == CommunicationState.Opened) { LWcfClient.Close(); }
                }
            }

            return LStrReturn;
        }

        /// <summary>
        /// 获取UMP应用服务器绑定的协议
        /// </summary>
        /// <returns>如果为空，表示获取成功，值保存在 IDataTableProtocolBind 中，否则为错误信息</returns>
        private string GetIISBindingProtocol()
        {
            string LStrReturn = string.Empty;
            Service00000Client LWcfClient = null;
            List<string> LListStrArgs = new List<string>();

            try
            {
                BasicHttpBinding LBinding = CreateBasicHttpBinding();
                EndpointAddress LEndAddress = CreateEndpointAddress(IStrAppServer, IStrServerPort, "Service00000");
                OperationDataArgs LOperationReturn = new OperationDataArgs();
                LWcfClient = new Service00000Client(LBinding, LEndAddress);
                LOperationReturn = LWcfClient.OperationMethodA(4, LListStrArgs);
                LWcfClient.Close();
                if (LOperationReturn.BoolReturn)
                {
                    IDataTableProtocolBind = LOperationReturn.DataSetReturn.Tables[0];
                }
                else
                {
                    LStrReturn = LOperationReturn.StringReturn;
                }
            }
            catch (Exception ex)
            {
                LStrReturn = ex.ToString();
            }
            finally
            {
                if (LWcfClient != null)
                {
                    if (LWcfClient.State == CommunicationState.Opened) { LWcfClient.Close(); }
                }
            }

            return LStrReturn;
        }

        /// <summary>
        /// 获取可设置安全站点的注册表路径
        /// </summary>
        /// <returns>如果为空，表示获取成功，值保存在 IDataTableTrustZones 中，否则为错误信息</returns>
        private string GetSettedTrustZones()
        {
            string LStrReturn = string.Empty;
            Service00000Client LWcfClient = null;
            List<string> LListStrArgs = new List<string>();

            try
            {
                BasicHttpBinding LBinding = CreateBasicHttpBinding();
                EndpointAddress LEndAddress = CreateEndpointAddress(IStrAppServer, IStrServerPort, "Service00000");
                OperationDataArgs LOperationReturn = new OperationDataArgs();
                LWcfClient = new Service00000Client(LBinding, LEndAddress);
                LOperationReturn = LWcfClient.OperationMethodA(2, LListStrArgs);
                LWcfClient.Close();
                if (LOperationReturn.BoolReturn)
                {
                    IDataTableTrustZones = LOperationReturn.DataSetReturn.Tables[0];
                }
                else
                {
                    LStrReturn = LOperationReturn.StringReturn;
                }
            }
            catch (Exception ex)
            {
                LStrReturn = ex.ToString();
            }
            finally
            {
                if (LWcfClient != null)
                {
                    if (LWcfClient.State == CommunicationState.Opened) { LWcfClient.Close(); }
                }
            }

            return LStrReturn;
        }

        /// <summary>
        /// 下载UMP开发时使用的安全证书\支持https的安全证书
        /// </summary>
        /// <param name="AStrCertificateFileName">安全证书名</param>
        /// <returns>如果为空，表示下载成功，值保存在 App.GStrLoginUserApplicationDataPath\UMP.Client 中，否则为错误信息</returns>
        private string DownloadUMPCertificate(string AStrCertificateFileName)
        {
            string LStrReturn = string.Empty;
            HttpWebRequest LHttpWebRequest = null;
            Stream LStreamResponse = null;
            string LStrCertificateFileFullName = string.Empty;

            try
            {
                LStrCertificateFileFullName = System.IO.Path.Combine(App.GStrLoginUserApplicationDataPath, @"UMP.Client\" + AStrCertificateFileName);
                if (System.IO.File.Exists(LStrCertificateFileFullName))
                {
                    System.IO.File.Delete(LStrCertificateFileFullName);
                    System.Threading.Thread.Sleep(500);
                }
                System.IO.FileStream LStreamCertificateFile = new FileStream(LStrCertificateFileFullName, FileMode.Create);
                LHttpWebRequest = (HttpWebRequest)HttpWebRequest.Create("http://" + IStrAppServer + ":" + IStrServerPort + "/Components/Certificates/" + AStrCertificateFileName);
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
            }
            catch (Exception ex)
            {
                LStrReturn = ex.ToString();
            }
            finally
            {
                if (LHttpWebRequest != null) { LHttpWebRequest.Abort(); }
                if (LStreamResponse != null) { LStreamResponse.Close(); LStreamResponse.Dispose(); }
            }

            return LStrReturn;
        }

        /// <summary>
        /// 下载ReportViewer运行时组件
        /// </summary>
        /// <param name="AStrLanguageID">语言编码</param>
        /// <returns>如果为空，表示下载成功，值保存在 App.GStrLoginUserApplicationDataPath\UMP.Client 中，否则为错误信息</returns>
        private string DownloadReportViewerRuntime(string AStrLanguageID)
        {
            string LStrReturn = string.Empty;
            HttpWebRequest LHttpWebRequest = null;
            Stream LStreamResponse = null;
            string LStrReportViewerFileFullName = string.Empty;

            try
            {
                LStrReportViewerFileFullName = System.IO.Path.Combine(App.GStrLoginUserApplicationDataPath, @"UMP.Client\ReportViewer" + AStrLanguageID + ".msi");
                if (System.IO.File.Exists(LStrReportViewerFileFullName))
                {
                    System.IO.File.Delete(LStrReportViewerFileFullName);
                    System.Threading.Thread.Sleep(500);
                }
                System.IO.FileStream LStreamReportViewerFile = new FileStream(LStrReportViewerFileFullName, FileMode.Create);
                LHttpWebRequest = (HttpWebRequest)HttpWebRequest.Create("http://" + IStrAppServer + ":" + IStrServerPort + "/Components/ReportViewer/ReportViewer" + AStrLanguageID + ".msi");
                long LContenctLength = LHttpWebRequest.GetResponse().ContentLength;
                LHttpWebRequest.AddRange(0);

                LStreamResponse = LHttpWebRequest.GetResponse().GetResponseStream();
                byte[] LbyteRead = new byte[1024];
                int LIntReadedSize = LStreamResponse.Read(LbyteRead, 0, 1024);
                while (LIntReadedSize > 0)
                {
                    LStreamReportViewerFile.Write(LbyteRead, 0, LIntReadedSize);
                    LIntReadedSize = LStreamResponse.Read(LbyteRead, 0, 1024);
                }
                LStreamReportViewerFile.Close(); LStreamReportViewerFile.Dispose();
            }
            catch (Exception ex)
            {
                LStrReturn = ex.ToString();
            }
            finally
            {
                if (LHttpWebRequest != null) { LHttpWebRequest.Abort(); }
                if (LStreamResponse != null) { LStreamResponse.Close(); LStreamResponse.Dispose(); }
            }

            return LStrReturn;
        }

        /// <summary>
        /// 下载Style组件
        /// </summary>
        /// <returns></returns>
        private string DownloadStyleFiles()
        {
            string LStrReturn = string.Empty;
            HttpWebRequest LHttpWebRequest = null;
            Stream LStreamResponse = null;
            string LStrMageFileFullName = string.Empty;
            Service00000Client LWcfClient = null;
            List<string> LListStrArgs = new List<string>();
            List<string> LListStrAllStyleFiles = new List<string>();
            string LStrCommonApplicationData = string.Empty;
            int LIntArrayLength = 0, LIntLoopArray = 0;
            string LStrCreateFolderBase = string.Empty;
            string LStrServerFilePath = string.Empty;

            try
            {
                BasicHttpBinding LBinding = CreateBasicHttpBinding();
                EndpointAddress LEndAddress = CreateEndpointAddress(IStrAppServer, IStrServerPort, "Service00000");
                OperationDataArgs LOperationReturn = new OperationDataArgs();
                LWcfClient = new Service00000Client(LBinding, LEndAddress);
                LOperationReturn = LWcfClient.OperationMethodA(6, LListStrArgs);
                LWcfClient.Close();
                if (LOperationReturn.BoolReturn)
                {
                    LListStrAllStyleFiles = LOperationReturn.ListStringReturn;
                }
                else
                {
                    return LOperationReturn.StringReturn;
                }

                LStrCommonApplicationData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                LStrCommonApplicationData = System.IO.Path.Combine(LStrCommonApplicationData, "UMP.Client");
                if (!Directory.Exists(LStrCommonApplicationData)) { Directory.CreateDirectory(LStrCommonApplicationData); }
                foreach (string LStrSingleStyleFile in LListStrAllStyleFiles)
                {
                    LStrCreateFolderBase = LStrCommonApplicationData;
                    string[] LStrFileInfo = LStrSingleStyleFile.Split('\\');
                    LIntArrayLength = LStrFileInfo.Length;
                    for (LIntLoopArray = 0; LIntLoopArray < LIntArrayLength - 1; LIntLoopArray++)
                    {
                        LStrCreateFolderBase = System.IO.Path.Combine(LStrCreateFolderBase, LStrFileInfo[LIntLoopArray]);
                        if (!Directory.Exists(LStrCreateFolderBase)) { Directory.CreateDirectory(LStrCreateFolderBase); }
                    }
                    LStrCreateFolderBase = System.IO.Path.Combine(LStrCreateFolderBase, LStrFileInfo[LIntArrayLength - 1]);
                    if (System.IO.File.Exists(LStrCreateFolderBase)) { System.IO.File.Delete(LStrCreateFolderBase); }

                    System.IO.FileStream LStreamTargetFile = new FileStream(LStrCreateFolderBase, FileMode.Create);
                    LStrServerFilePath = LStrSingleStyleFile.Replace(@"\", "/");
                    LHttpWebRequest = (HttpWebRequest)HttpWebRequest.Create("http://" + IStrAppServer + ":" + IStrServerPort + "/" + LStrServerFilePath);
                    long LContenctLength = LHttpWebRequest.GetResponse().ContentLength;
                    LHttpWebRequest.AddRange(0);
                    LStreamResponse = LHttpWebRequest.GetResponse().GetResponseStream();
                    byte[] LbyteRead = new byte[1024];
                    int LIntReadedSize = LStreamResponse.Read(LbyteRead, 0, 1024);
                    while (LIntReadedSize > 0)
                    {
                        LStreamTargetFile.Write(LbyteRead, 0, LIntReadedSize);
                        LIntReadedSize = LStreamResponse.Read(LbyteRead, 0, 1024);
                    }
                    LStreamTargetFile.Close(); LStreamTargetFile.Dispose();
                }
            }
            catch (Exception ex)
            {
                LStrReturn = ex.ToString();
            }
            finally
            {
                if (LWcfClient != null)
                {
                    if (LWcfClient.State == CommunicationState.Opened) { LWcfClient.Close(); }
                }
                if (LHttpWebRequest != null) { LHttpWebRequest.Abort(); }
                if (LStreamResponse != null) { LStreamResponse.Close(); LStreamResponse.Dispose(); }
            }

            return LStrReturn;
        }

        /// <summary>
        /// 下载 mage.exe
        /// </summary>
        /// <returns>如果为空，表示下载成功，值保存在 App.GStrLoginUserApplicationDataPath\UMP.Client 中，否则为错误信息</returns>
        private string DownloadMageTool()
        {
            string LStrReturn = string.Empty;
            HttpWebRequest LHttpWebRequest = null;
            Stream LStreamResponse = null;
            string LStrMageFileFullName = string.Empty;

            try
            {
                LStrMageFileFullName = System.IO.Path.Combine(App.GStrSystemRoot, @"mage.exe");
                if (System.IO.File.Exists(LStrMageFileFullName)) { return string.Empty; }
                System.IO.FileStream LStreamReportViewerFile = new FileStream(LStrMageFileFullName, FileMode.Create);
                LHttpWebRequest = (HttpWebRequest)HttpWebRequest.Create("http://" + IStrAppServer + ":" + IStrServerPort + "/Components/Others/mage.exe");
                long LContenctLength = LHttpWebRequest.GetResponse().ContentLength;
                LHttpWebRequest.AddRange(0);

                LStreamResponse = LHttpWebRequest.GetResponse().GetResponseStream();
                byte[] LbyteRead = new byte[1024];
                int LIntReadedSize = LStreamResponse.Read(LbyteRead, 0, 1024);
                while (LIntReadedSize > 0)
                {
                    LStreamReportViewerFile.Write(LbyteRead, 0, LIntReadedSize);
                    LIntReadedSize = LStreamResponse.Read(LbyteRead, 0, 1024);
                }
                LStreamReportViewerFile.Close(); LStreamReportViewerFile.Dispose();
            }
            catch (Exception ex)
            {
                LStrReturn = ex.ToString();
            }
            finally
            {
                if (LHttpWebRequest != null) { LHttpWebRequest.Abort(); }
                if (LStreamResponse != null) { LStreamResponse.Close(); LStreamResponse.Dispose(); }
            }

            return LStrReturn;
        }

        /// <summary>
        /// 验证目前是否支持当前OS
        /// </summary>
        /// <returns>如果为空，表示验证成功，支持该版本的Windows</returns>
        private string VerificationSystemVersion()
        {
            string LStrReturn = string.Empty;

            try
            {
                DataRow[] LDataRowMatchedWinVersion = IDataTableTrustZones.Select("WinVersion = '" + App.GStrWindowVersion + "' AND InstallType = '" + App.GStrWindowType + "'");
                if (LDataRowMatchedWinVersion.Length <= 0)
                {
                    LStrReturn = "NotSurportVersion";
                }
            }
            catch (Exception ex)
            {
                LStrReturn = ex.ToString();
            }

            return LStrReturn;
        }

        /// <summary>
        /// 安装安全证书，区域为：受信任的根证书发布机构、受信任的发布者
        /// </summary>
        /// <param name="AStrCertificateFileName">证书名</param>
        /// <param name="AStrPassword">证书安装密码</param>
        /// <returns>如果为空，表示安装成功，否则为错误信息</returns>
        private string InstallCertificateFile(string AStrCertificateFileName, string AStrPassword, string AStrHashString)
        {
            string LStrReturn = string.Empty;
            string LStrCertificateFileFullName = string.Empty;
            string LStrCallReturn = string.Empty;

            try
            {

                LStrCertificateFileFullName = System.IO.Path.Combine(App.GStrLoginUserApplicationDataPath, @"UMP.Client\" + AStrCertificateFileName);
                byte[] LByteReadedCertificate = System.IO.File.ReadAllBytes(LStrCertificateFileFullName);
                if (!CertificateIsExist(AStrHashString, StoreName.TrustedPublisher, StoreLocation.LocalMachine, ref LStrCallReturn))
                {
                    LStrReturn = InstallCertificateToStore(StoreName.TrustedPublisher, LByteReadedCertificate, AStrPassword);
                    if (!string.IsNullOrEmpty(LStrReturn)) { return LStrReturn; }
                }
                if (!CertificateIsExist(AStrHashString, StoreName.AuthRoot, StoreLocation.LocalMachine, ref LStrCallReturn))
                {
                    LStrReturn = InstallCertificateToStore(StoreName.AuthRoot, LByteReadedCertificate, AStrPassword);
                }
            }
            catch (Exception ex)
            {
                LStrReturn = ex.ToString();
            }

            return LStrReturn;
        }

        /// <summary>
        /// 根据 HashString 判断证书是否在 指定的存储区域、存储位置 中存在
        /// </summary>
        /// <param name="AStrHashString">HashString</param>
        /// <param name="AStoreNameArea">存储区的名称</param>
        /// <param name="AStoreLocationPositon">存储区的位置</param>
        /// <param name="AStrReturn">当返回False时，返回获取时的错误信息</param>
        /// <returns>True / False</returns>
        public static bool CertificateIsExist(string AStrHashString, StoreName AStoreNameArea, StoreLocation AStoreLocationPositon, ref string AStrReturn)
        {
            bool LBoolReturn = false;

            try
            {
                AStrReturn = string.Empty;
                X509Store LX509Store = new X509Store(AStoreNameArea, AStoreLocationPositon);
                LX509Store.Open(OpenFlags.ReadOnly);
                foreach (X509Certificate LX509CertificateSingle in LX509Store.Certificates)
                {
                    if (LX509CertificateSingle.GetCertHashString().Trim() == AStrHashString) { LBoolReturn = true; break; }
                }
                LX509Store.Close(); LX509Store = null;
            }
            catch (Exception ex)
            {
                LBoolReturn = false;
                AStrReturn = "CertificateIsExist()\n" + ex.ToString();
            }

            return LBoolReturn;
        }

        /// <summary>
        /// 将证书安装到对应存储区域
        /// </summary>
        /// <param name="AStoreName">证书存储区的名称</param>
        /// <param name="AByteCertificate">证书文件内容</param>
        /// <param name="AStrPassword">证书密码</param>
        /// <returns>如果为空，表示安装成功，否则为错误信息</returns>
        private string InstallCertificateToStore(StoreName AStoreName, byte[] AByteCertificate, string AStrPassword)
        {
            string LStrReturn = string.Empty;

            try
            {
                X509Certificate2 LX509Certificate = new X509Certificate2(AByteCertificate, AStrPassword);
                X509Store LX509Store = new X509Store(AStoreName, StoreLocation.LocalMachine);

                LX509Certificate.FriendlyName = IStrAppShortName;
                LX509Store.Open(OpenFlags.ReadWrite);
                LX509Store.Remove(LX509Certificate);
                LX509Store.Add(LX509Certificate);
                LX509Store.Close();
            }
            catch (Exception ex)
            {
                LStrReturn = ex.ToString();
            }

            return LStrReturn;
        }

        /// <summary>
        /// 安装Microsoft CLR Types 运行时组件
        /// </summary>
        /// <returns></returns>
        private string InstallCLRTypesComponent()
        {
            string LStrReturn = string.Empty;
            string LStrReportViewerFile = string.Empty;

            try
            {
                if (Environment.Is64BitOperatingSystem)
                {
                    LStrReportViewerFile = System.IO.Path.Combine(App.GStrLoginUserApplicationDataPath, @"UMP.Client\ReportViewer0000.msi");
                }
                else
                {
                    LStrReportViewerFile = System.IO.Path.Combine(App.GStrLoginUserApplicationDataPath, @"UMP.Client\ReportViewer0001.msi");
                }

                Process LocalProExec = new Process();
                LocalProExec.StartInfo.FileName = LStrReportViewerFile;
                LocalProExec.StartInfo.Arguments = "/qn";
                LocalProExec.Start();
                LocalProExec.WaitForExit();
                if (LocalProExec.HasExited == false)
                {
                    LocalProExec.Kill();
                }
                LocalProExec.Dispose();
            }
            catch (Exception ex)
            {
                LStrReturn = ex.ToString();
            }

            return LStrReturn;
        }

        /// <summary>
        /// 安装ReportViewer运行时组件
        /// </summary>
        /// <returns></returns>
        private string InstallReportViewerComponent()
        {
            string LStrReturn = string.Empty;
            string LStrReportViewerFile = string.Empty;

            try
            {
                List<string> listLangs = new List<string>();
                listLangs.Add("1033");
                listLangs.Add("2052");
                listLangs.Add("1028");
                listLangs.Add("1041");
                for (int i = 0; i < listLangs.Count; i++)
                {
                    LStrReportViewerFile = System.IO.Path.Combine(App.GStrLoginUserApplicationDataPath, @"UMP.Client\ReportViewer" + listLangs[i] + ".msi");
                    if (!System.IO.File.Exists(LStrReportViewerFile))
                    {
                        continue;
                    }
                    Process LocalProExec = new Process();
                    LocalProExec.StartInfo.FileName = LStrReportViewerFile;
                    LocalProExec.StartInfo.Arguments = "/qn";
                    LocalProExec.Start();
                    LocalProExec.WaitForExit();
                    if (LocalProExec.HasExited == false)
                    {
                        LocalProExec.Kill();
                    }
                    LocalProExec.Dispose();
                }
            }
            catch (Exception ex)
            {
                LStrReturn = ex.ToString();
            }

            return LStrReturn;
        }

        #region 将UMP服务器加入到信任站点
        /// <summary>
        /// 将UMP服务器加入到信任站点
        /// </summary>
        /// <returns>如果为空，表示成功，否则为错误信息</returns>
        private string SetTrustWebSite()
        {
            string LStrReturn = string.Empty;
            string[] LStrUsersList;
            RegistryKey LEnvironmentRegistryKey = null;
            string LStrUserName = string.Empty;
            string LStrSourceType = string.Empty;
            RegistryKey LocalSingleUserKey = null;
            try
            {
                RegistryKey LocalUsersRegistryKey = Registry.Users;
                LStrUsersList = LocalUsersRegistryKey.GetSubKeyNames();
                foreach (string LStrSingleUser in LStrUsersList)
                {
                    try
                    {
                        LocalSingleUserKey = LocalUsersRegistryKey.OpenSubKey(LStrSingleUser);
                    }
                    catch { continue; }
                    LEnvironmentRegistryKey = LocalSingleUserKey.OpenSubKey("Volatile Environment");
                    if (LEnvironmentRegistryKey == null) { continue; }
                    LStrUserName = (string)LEnvironmentRegistryKey.GetValue("USERNAME");
                    if (LStrUserName != Environment.UserName)
                    {
                        LEnvironmentRegistryKey.Close();
                        LEnvironmentRegistryKey = null;
                        continue;
                    }
                    if (ServerHostIsIPAddress(IStrAppServer)) { LStrSourceType = "IPAddress"; } else { LStrSourceType = "MachineName"; }
                    DataRow[] LDataRowMatchedWinVersion = IDataTableTrustZones.Select("WinVersion = '" + App.GStrWindowVersion + "' AND InstallType = '" + App.GStrWindowType + "' AND SourceType = '" + LStrSourceType + "'");
                    LStrReturn = SetTrustZoneToRangesDomains(LocalSingleUserKey, LDataRowMatchedWinVersion[0]);
                    break;
                }
                LocalUsersRegistryKey.Close();
            }
            catch (Exception ex)
            {
                LStrReturn = ex.ToString();
            }

            return LStrReturn;
        }

        private string SetTrustZoneToRangesDomains(RegistryKey ASingleUserKey, DataRow ADataRowTrustZonesArgs)
        {
            string LStrReturn = string.Empty;
            string LStrRootRegistryKey = string.Empty;
            string LStrSubRegistryKey = string.Empty;
            string[] LStrSubkeyArrayNames;
            RegistryKey LRegistryKeyOpened = null;

            try
            {
                LStrRootRegistryKey = ADataRowTrustZonesArgs["RootRegistryKey"].ToString();
                LStrSubRegistryKey = ADataRowTrustZonesArgs["SubRegistryKey"].ToString();
                try
                {
                    ASingleUserKey.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Internet Settings\ZoneMap", true).CreateSubKey(LStrSubRegistryKey);
                }
                catch { }
                LRegistryKeyOpened = ASingleUserKey.OpenSubKey(LStrRootRegistryKey + @"\" + LStrSubRegistryKey, true);
                LStrSubkeyArrayNames = LRegistryKeyOpened.GetSubKeyNames();

                if (LStrSubRegistryKey.Contains("Ranges"))
                {
                    foreach (string LStrRangeName in LStrSubkeyArrayNames)
                    {
                        RegistryKey LRegistryKeySingleRange = null;
                        try
                        {
                            LRegistryKeySingleRange = LRegistryKeyOpened.OpenSubKey(LStrRangeName);
                            string LStrExistRange = LRegistryKeySingleRange.GetValue(":Range").ToString();
                            if (LStrExistRange == IStrAppServer)
                            {
                                LRegistryKeyOpened.DeleteSubKey(LStrRangeName);
                                //LRegistryKeySingleRange.SetValue("http", 2, RegistryValueKind.DWord);
                                //LRegistryKeySingleRange.SetValue("https", 2, RegistryValueKind.DWord);
                                //return string.Empty;
                            }
                        }
                        catch { }
                        finally
                        {
                            if (LRegistryKeySingleRange != null) { LRegistryKeySingleRange.Close(); }
                        }
                    }
                    for (UInt16 LUIntLoop = UInt16.MaxValue; LUIntLoop > 1; --LUIntLoop)
                    {
                        bool LBoolIDExist = false;

                        foreach (string LStrKeyName in LStrSubkeyArrayNames)
                        {
                            if (LStrKeyName == "Range" + LUIntLoop.ToString()) { LBoolIDExist = true; break; }
                        }
                        if (!LBoolIDExist)
                        {
                            RegistryKey NewRangeKey = LRegistryKeyOpened.CreateSubKey("Range" + LUIntLoop);
                            NewRangeKey.SetValue(":Range", IStrAppServer, RegistryValueKind.String);
                            NewRangeKey.SetValue("http", 2, RegistryValueKind.DWord);
                            NewRangeKey.SetValue("https", 2, RegistryValueKind.DWord);
                            NewRangeKey.Close();
                            break;
                        }
                    }
                }
                else
                {
                    foreach (string LStrDomainsName in LStrSubkeyArrayNames)
                    {
                        if (LStrDomainsName.ToUpper() == IStrAppServer.ToUpper())
                        {
                            LRegistryKeyOpened.DeleteSubKey(LStrDomainsName);
                            //return string.Empty;
                        }
                    }
                    RegistryKey LRegistryKeyDomainKey = LRegistryKeyOpened.CreateSubKey(IStrAppServer);
                    LRegistryKeyDomainKey.SetValue("http", 2, RegistryValueKind.DWord);
                    LRegistryKeyDomainKey.SetValue("https", 2, RegistryValueKind.DWord);
                    LRegistryKeyDomainKey.Close();
                }
            }
            catch (Exception ex)
            {
                LStrReturn = ex.ToString();
            }
            finally
            {
                if (LRegistryKeyOpened != null) { LRegistryKeyOpened.Close(); }
            }

            return LStrReturn;
        }

        #endregion


        #region 修改注册表，把自动更新根证书关闭

        private void ModifyAuthRootValue()
        {
            try
            {
                string path = @"SOFTWARE\Policies\Microsoft\SystemCertificates\AuthRoot";
                RegistryKey rootKey = Registry.LocalMachine;
                RegistryKey authRootKey = rootKey.CreateSubKey(path);
                if (authRootKey == null) { return; }
                authRootKey.SetValue("DisableRootAutoUpdate", 1, RegistryValueKind.DWord);

                if (Environment.Is64BitOperatingSystem)
                {
                    path = @"SOFTWARE\Wow6432Node\Policies\Microsoft\SystemCertificates\AuthRoot";
                    rootKey = Registry.LocalMachine;
                    authRootKey = rootKey.CreateSubKey(path);
                    if (authRootKey == null) { return; }
                    authRootKey.SetValue("DisableRootAutoUpdate", 1, RegistryValueKind.DWord);
                }
            }
            catch (Exception ex)
            {

            }
        }

        #endregion


        /// <summary>
        /// 将UMP应用服务器绑定的协议写入文件
        /// </summary>
        /// <returns>如果为空，表示写入成功，否则为错误信息</returns>
        private string SaveAppServerProtocolBind()
        {
            string LStrReturn = string.Empty;
            string LStrXmlFileName = string.Empty;
            string LStrProtocol = string.Empty;

            try
            {
                LStrReturn = App.WriteUserSettings("UMPServerSetted", "UMPServerHost", IStrAppServer);
                if (!string.IsNullOrEmpty(LStrReturn)) { return LStrReturn; }
                LStrXmlFileName = System.IO.Path.Combine(App.GStrLoginUserApplicationDataPath, @"UMP.Client\UMP.Setted.xml");
                XmlDocument LXmlDocUserSetting = new XmlDocument();
                LXmlDocUserSetting.Load(LStrXmlFileName);
                XmlNode LXMLNodeSection = LXmlDocUserSetting.SelectSingleNode("UserSetted").SelectSingleNode("UMPServerSetted");
                XmlNodeList LXmlNodeListChild = LXMLNodeSection.ChildNodes;
                foreach (XmlNode LXmlNodeSingle in LXmlNodeListChild)
                {
                    LStrProtocol = LXmlNodeSingle.Attributes["Protocol"].Value;
                    DataRow[] LDataRowServerArgs = IDataTableProtocolBind.Select("Protocol='" + LStrProtocol + "'");
                    LXmlNodeSingle.Attributes["Activated"].Value = LDataRowServerArgs[0]["Activated"].ToString();
                    LXmlNodeSingle.Attributes["BindInfo"].Value = LDataRowServerArgs[0]["BindInfo"].ToString();
                    LXmlNodeSingle.Attributes["Used"].Value = LDataRowServerArgs[0]["Used"].ToString();
                }
                LXmlDocUserSetting.Save(LStrXmlFileName);
            }
            catch (Exception ex)
            {
                LStrReturn = ex.ToString();
            }

            return LStrReturn;
        }

        /// <summary>
        /// 创建桌面和开始菜单快捷方式
        /// </summary>
        /// <returns>如果为空，表示创建成功，否则为错误信息</returns>
        private string CreateShortCutInThisComputer()
        {
            string LStrReturn = string.Empty;
            List<string> LListStrShortCutFolder = new List<string>();
            string LStrDescription = string.Empty;
            string LStrTemp = string.Empty;

            try
            {
                LListStrShortCutFolder.Add(System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
                LListStrShortCutFolder.Add(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu));
                LStrDescription = string.Format(App.GetDisplayCharater("Window02", "N004"), IStrAppShortName);
                foreach (string LStrShortCutFolder in LListStrShortCutFolder)
                {
                    LStrTemp = "1:" + LStrShortCutFolder;
                    WshShell LWshShell = new WshShell();
                    IWshShortcut LShortCut = (IWshShortcut)LWshShell.CreateShortcut(LStrShortCutFolder + "//" + LStrDescription + ".lnk");
                    LStrTemp = "2:" + LStrShortCutFolder + "/" + LStrDescription + ".lnk";
                    LShortCut.TargetPath = @"%HOMEDRIVE%\Program Files\Internet Explorer\IEXPLORE.EXE";
                    LShortCut.IconLocation = @"%HOMEDRIVE%\Program Files\Internet Explorer\IEXPLORE.EXE, 0";//图标

                    DataRow[] LDataRowServerArgs = IDataTableProtocolBind.Select("Protocol='https' AND Activated='1' AND IPAddress = '" + IStrAppServer + "'");
                    if (LDataRowServerArgs.Length > 0)
                    {
                        LShortCut.Arguments = "https://" + IStrAppServer + ":" + (int.Parse(IStrServerPort) + 1) + "/UMPMain.xbap";// 参数 
                    }
                    else
                    {
                        LShortCut.Arguments = "http://" + IStrAppServer + ":" + IStrServerPort + "/UMPMain.xbap";// 参数 
                    }
                    LStrTemp = "3:" + LShortCut.Arguments;
                    LShortCut.Description = LStrDescription;
                    LShortCut.WorkingDirectory = "%HOMEDRIVE%%HOMEPATH%";
                    LShortCut.IconLocation = System.IO.Path.Combine(App.GStrSystemRoot, "UMPAppLogo.ico");
                    LStrTemp = "4:" + System.IO.Path.Combine(App.GStrSystemRoot, "UMPAppLogo.ico");
                    LShortCut.WindowStyle = 3;
                    LShortCut.Save();
                }
            }
            catch (Exception ex)
            {
                LStrReturn = LStrTemp + "\n" + ex.ToString();
            }

            return LStrReturn;
        }

        /// <summary>
        /// 创建清除UMP联机应用程序缓存
        /// </summary>
        /// <returns>如果为空，表示创建成功，否则为错误信息</returns>
        private string CreateApplicationCacheClearShortCut()
        {
            string LStrReturn = string.Empty;
            string LStrStartMenuPath = string.Empty;
            string LStrDescription = string.Empty;
            try
            {
                LStrStartMenuPath = Environment.GetFolderPath(Environment.SpecialFolder.StartMenu);
                LStrDescription = string.Format(App.GetDisplayCharater("Window02", "N003"), IStrAppShortName);
                WshShell LWshShell = new WshShell();

                IWshShortcut LShortCutStartMenu = (IWshShortcut)LWshShell.CreateShortcut(LStrStartMenuPath + @"\" + LStrDescription + ".lnk");
                LShortCutStartMenu.TargetPath = System.IO.Path.Combine(App.GStrSystemRoot, "mage.exe");
                LShortCutStartMenu.Arguments = "-cc";
                LShortCutStartMenu.Description = LStrDescription;
                LShortCutStartMenu.WorkingDirectory = App.GStrSystemRoot;
                LShortCutStartMenu.IconLocation = System.IO.Path.Combine(App.GStrSystemRoot, "UMPAppLogo.ico");
                LShortCutStartMenu.WindowStyle = 1;
                LShortCutStartMenu.Save();

            }
            catch (Exception ex)
            {
                LStrReturn = ex.Message;
            }

            return LStrReturn;
        }

        #endregion

        private void ButtonApplicationMenu_Click(object sender, RoutedEventArgs e)
        {
            Button ClickedButton = sender as Button;
            //目标   
            ClickedButton.ContextMenu.PlacementTarget = ClickedButton;
            //位置   
            ClickedButton.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
            //显示菜单   
            ClickedButton.ContextMenu.IsOpen = true;
        }

        private void Window02_Loaded(object sender, RoutedEventArgs e)
        {
            InitOpenedUMPServerList();
            InitDropDownMenu();
            DiplayUIElementCharater();
            ComboBoxServerName.Focus();
        }

        private void ButtonClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window02_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (IBoolInInstalling) { e.Cancel = true; }
        }

        private void Window02_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        /// <summary>
        /// 显示已经打开过的UMP服务器
        /// </summary>
        private void InitOpenedUMPServerList()
        {
            int LIntLoopServer = 0;
            int LIntAllServer = 0;

            try
            {
                ComboBoxServerName.Items.Clear();
                LIntAllServer = App.GListStrAppServerName.Count - 1;
                for (LIntLoopServer = LIntAllServer; LIntLoopServer >= 0; LIntLoopServer--)
                {
                    if (!ServerHostIsIPAddress(App.GListStrAppServerName[LIntLoopServer]))
                    {
                        if (App.GListStrAppServerName[LIntLoopServer].Contains(".")) { continue; }
                    }
                    ComboBoxItem LComboBoxServerItem = new ComboBoxItem();
                    LComboBoxServerItem.Background = Brushes.Transparent;
                    LComboBoxServerItem.Margin = new Thickness(0, 1, 0, 1);
                    LComboBoxServerItem.Style = (Style)App.Current.Resources["ComboBoxItemFontStyle"];
                    LComboBoxServerItem.Height = 26;
                    LComboBoxServerItem.Content = App.GListStrAppServerName[LIntLoopServer];
                    LComboBoxServerItem.DataContext = App.GListStrAppServerName[LIntLoopServer] + App.GStrSpliterCharater + App.GListIntAppServerPort[LIntLoopServer].ToString();
                    LComboBoxServerItem.Tag = App.GListIntAppServerPort[LIntLoopServer].ToString();
                    ComboBoxServerName.Items.Add(LComboBoxServerItem);
                }
                ComboBoxServerName.SelectionChanged += ComboBoxServerName_SelectionChanged;
                if (string.IsNullOrEmpty(App.GStrLastSettedUMPServerHost))
                {
                    ComboBoxServerName.SelectedIndex = 0;
                }
                else
                {
                    ComboBoxServerName.Text = App.GStrLastSettedUMPServerHost;
                    TextPort.Text = App.GIntLastSettedUMPServerPort.ToString();
                }
            }
            catch { }
        }

        private bool ServerHostIsIPAddress(string AStrServerHost)
        {
            bool LBoolReturn = true;

            try
            {
                IPAddress LIPAddress = null;
                LBoolReturn = IPAddress.TryParse(AStrServerHost, out LIPAddress);
            }
            catch { LBoolReturn = false; }

            return LBoolReturn;
        }

        private void ComboBoxServerName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                ComboBoxItem LComboBoxItemSelectedServer = ComboBoxServerName.SelectedItem as ComboBoxItem;
                if (LComboBoxItemSelectedServer == null) { return; }
                TextPort.Text = LComboBoxItemSelectedServer.Tag.ToString();
            }
            catch { }
        }

        /// <summary>
        /// 初始化下拉菜单
        /// </summary>
        private void InitDropDownMenu()
        {
            string LStrFilter = string.Empty;

            try
            {
                ContextMenu LocalContextMenu = new ContextMenu();
                LocalContextMenu.Opacity = 0.8;
                MenuItem LMenuItemSurportLanguage = new MenuItem();
                LMenuItemSurportLanguage.Header = App.GetDisplayCharater("Window02", "MenuLanguage");
                LMenuItemSurportLanguage.DataContext = "T-SLL";

                Image LImageIcon = new Image();
                LImageIcon.Height = 16; LImageIcon.Width = 16;
                LImageIcon.Source = new BitmapImage(new Uri(@"\Images\00000004.ico", UriKind.RelativeOrAbsolute));
                LMenuItemSurportLanguage.Icon = LImageIcon;
                LMenuItemSurportLanguage.Style = (Style)App.Current.Resources["MenuItemFontStyle"];

                LStrFilter = "ObjectID like 'Language%'";
                DataRow[] LanguageDataRow = App.GDataTableLanguage.Select(LStrFilter, "ObjectID ASC");
                foreach (DataRow LDataRowSingleLanguage in LanguageDataRow)
                {
                    MenuItem LMenuItemSingle = new MenuItem();
                    LMenuItemSingle.Header = "(" + (string)LDataRowSingleLanguage[1] + ")" + (string)LDataRowSingleLanguage[2];
                    LMenuItemSingle.DataContext = "L-" + (string)LDataRowSingleLanguage[1];
                    if ((string)LDataRowSingleLanguage[1] == App.GStrLanguageID) { LMenuItemSingle.IsChecked = true; }
                    LMenuItemSingle.Style = (Style)App.Current.Resources["MenuItemFontStyle"];
                    LMenuItemSingle.Click += LMenuItemSingle_Click;
                    LMenuItemSurportLanguage.Items.Add(LMenuItemSingle);
                }

                LocalContextMenu.Items.Add(LMenuItemSurportLanguage);

                ButtonApplicationMenu.ContextMenu = LocalContextMenu;
            }
            catch { }
        }

        private void LMenuItemSingle_Click(object sender, RoutedEventArgs e)
        {
            string LStrClickedData = string.Empty;
            string LStrSelectedLanguageID = string.Empty;

            try
            {
                if (IBoolInInstalling) { return; }

                MenuItem LMenuItemClicked = sender as MenuItem;
                LStrClickedData = LMenuItemClicked.DataContext.ToString();
                if (LStrClickedData.Substring(0, 2) != "L-") { return; }
                LStrSelectedLanguageID = LStrClickedData.Substring(2);
                if (LStrSelectedLanguageID == App.GStrLanguageID) { return; }
                if (App.InitializeLanguagePackage(LStrSelectedLanguageID))
                {
                    InitDropDownMenu();
                    DiplayUIElementCharater();
                    App.WriteUserSettings("LanguageSetted", "LanguageID", LStrSelectedLanguageID);
                }
            }
            catch { }
        }

        private void DiplayUIElementCharater()
        {
            LabelComponentsInstall.Content = App.GetDisplayCharater("Window02", "LabelComponentsInstall");
            this.Title = LabelComponentsInstall.Content.ToString();

            TabItemServer.Header = " " + App.GetDisplayCharater("Window02", "TabItemServer") + " ";
            TabItemInstallLog.Header = " " + App.GetDisplayCharater("Window02", "TabItemInstallLog") + " ";

            LabelAppServer.Content = App.GetDisplayCharater("Window02", "LabelAppServer");
            LabelServer.Content = App.GetDisplayCharater("Window02", "LabelServer");
            LabelPort.Content = App.GetDisplayCharater("Window02", "LabelPort");

            ButtonInstall.Content = App.GetDisplayCharater("Window02", "ButtonInstall");
            ButtonCloseWindow.Content = App.GetDisplayCharater("Window02", "ButtonCloseWindow");
        }

        private BasicHttpBinding CreateBasicHttpBinding()
        {
            BasicHttpBinding LocalReturnBinding = new BasicHttpBinding(BasicHttpSecurityMode.None);

            LocalReturnBinding.MaxReceivedMessageSize = int.MaxValue;
            LocalReturnBinding.MaxBufferSize = int.MaxValue;
            LocalReturnBinding.SendTimeout = new TimeSpan(0, 10, 0);
            LocalReturnBinding.ReceiveTimeout = new TimeSpan(0, 20, 0);

            return LocalReturnBinding;
        }

        private EndpointAddress CreateEndpointAddress(string AStrHost, string AStrPort, string AStrSvcName)
        {
            string LStrUri = string.Empty;

            LStrUri = "http://" + AStrHost + ":" + AStrPort + "/Wcf2Client/" + AStrSvcName + ".svc";
            EndpointAddress LocalReturnEndAddress = new EndpointAddress(new Uri(LStrUri, UriKind.Absolute));
            return LocalReturnEndAddress;
        }
    }
}
