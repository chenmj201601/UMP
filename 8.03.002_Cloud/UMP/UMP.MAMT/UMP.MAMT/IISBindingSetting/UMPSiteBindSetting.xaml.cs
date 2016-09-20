using Microsoft.Web.Administration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;
using UMP.MAMT.PublicClasses;

namespace UMP.MAMT.IISBindingSetting
{
    public partial class UMPSiteBindSetting : Window, MamtOperationsInterface
    {
        public event EventHandler<MamtOperationEventArgs> IOperationEvent;

        private DataTable IDataTableBindingInfo = null;

        //是否在处理的过程中
        private bool IBoolInDoing = false;

        //Site 根路径
        private string IStrSiteRootFolder = string.Empty;

        public UMPSiteBindSetting(DataTable ADataTableBindingInfo)
        {
            InitializeComponent();

            IDataTableBindingInfo = ADataTableBindingInfo;
            this.Loaded += UMPSiteBindSetting_Loaded;
            this.Closing += UMPSiteBindSetting_Closing;
            this.MouseLeftButtonDown += UMPSiteBindSetting_MouseLeftButtonDown;

            MainPanel.KeyDown += MainPanel_KeyDown;

            ComboBoxBindingAddress.SelectionChanged += ComboBoxBindingAddress_SelectionChanged;
            ComboBoxBindingAddress.KeyUp += ComboBoxBindingAddress_KeyUp;
            ComboBoxBindingPort.LostFocus += ComboBoxBindingPort_LostFocus;

            ButtonApplicationMenu.Click += WindowsButtonClicked;
            ButtonCloseConnect.Click += WindowsButtonClicked;
            ButtonSetBinding.Click += WindowsButtonClicked;
            ButtonCloseWindow.Click += WindowsButtonClicked;
        }


        private void ComboBoxBindingPort_LostFocus(object sender, RoutedEventArgs e)
        {
            int LIntBindPort = 0;
            string LStrBindPort = string.Empty;

            try
            {
                LStrBindPort = ComboBoxBindingPort.Text.Trim();
                if (string.IsNullOrEmpty(LStrBindPort))
                {
                    MessageBox.Show(App.GetDisplayCharater("M01034"), App.GStrApplicationReferredTo, MessageBoxButton.OK, MessageBoxImage.Warning);
                    ComboBoxBindingPort.Focus();
                    return;
                }
                if (!int.TryParse(LStrBindPort, out LIntBindPort))
                {
                    MessageBox.Show(App.GetDisplayCharater("M01034"), App.GStrApplicationReferredTo, MessageBoxButton.OK, MessageBoxImage.Warning);
                    ComboBoxBindingPort.Focus();
                    return;
                }
                if (LIntBindPort < 8020 || LIntBindPort > 8999)
                {
                    MessageBox.Show(App.GetDisplayCharater("M01034"), App.GStrApplicationReferredTo, MessageBoxButton.OK, MessageBoxImage.Warning);
                    ComboBoxBindingPort.Focus();
                    return;
                }
                TextBoxBindingPort.Text = (LIntBindPort + 1).ToString();
            }
            catch{}

        }

        private void ComboBoxBindingAddress_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string LStrServerAddress = string.Empty;

            try
            {
                LStrServerAddress = (ComboBoxBindingAddress.SelectedItem as ComboBoxItem).Content.ToString();
                TextBoxBindingAddress.Text = LStrServerAddress;
                tbNetTcpBindingAddress.Text = LStrServerAddress;
            }
            catch { }
        }

        void ComboBoxBindingAddress_KeyUp(object sender, KeyEventArgs e)
        {
            string LStrServerAddress = string.Empty;

            try
            {
                LStrServerAddress = ComboBoxBindingAddress.Text;
                TextBoxBindingAddress.Text = LStrServerAddress;
                tbNetTcpBindingAddress.Text = LStrServerAddress;
            }
            catch { }
        }

        private void DisplayElementCharacters(bool ABoolLanguageChange)
        {
            LabelSiteBindingTip.Content = App.GetDisplayCharater("M01028");
            TabItemSiteBinding.Header = " " + App.GetDisplayCharater("M01029") + " ";
            LabelServiceAddress.Content = App.GetDisplayCharater("M01030");
            //LabelMonitorPort.Content = App.GetDisplayCharater("M01031");
            ButtonSetBinding.Content = App.GetDisplayCharater("M01032");
            ButtonCloseWindow.Content = App.GetDisplayCharater("M01033");
            CbDefaultUseHttp.Content = App.GetDisplayCharater("M02087");
            CbOpenCloud.Content = App.GetDisplayCharater("M02090");

            TabItemMore.Header = " " + App.GetDisplayCharater("M02091") + " ";
        }

        private void UMPSiteBindSetting_Loaded(object sender, RoutedEventArgs e)
        {
            App.DrawWindowsBackGround(this);
            ButtonApplicationMenu.ContextMenu = App.InitApplicationMenu();
            App.GSystemMainWindow.IOperationEvent += GSystemMainWindow_IOperationEvent;
            ImageSiteBinding.Source = new BitmapImage(new Uri(System.IO.Path.Combine(App.GStrApplicationDirectory, @"Images\00000021.ico"), UriKind.RelativeOrAbsolute));

            InitServerAddressAndPort();
            GetSiteRootFolder();

            DisplayElementCharacters(false);
        }

        private void WindowsButtonClicked(object sender, RoutedEventArgs e)
        {
            try
            {
                Button LButtonClicked = sender as Button;
                string LStrClickedName = LButtonClicked.Name;

                switch (LStrClickedName)
                {
                    case "ButtonApplicationMenu":
                        //目标   
                        LButtonClicked.ContextMenu.PlacementTarget = LButtonClicked;
                        //位置   
                        LButtonClicked.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
                        //显示菜单   
                        LButtonClicked.ContextMenu.IsOpen = true;
                        break;
                    case "ButtonCloseConnect":
                        CloseThisWindow();
                        break;
                    case "ButtonSetBinding":
                        BeginSetBingdingParameters();
                        break;
                    case "ButtonCloseWindow":
                        CloseThisWindow();
                        break;
                    default:
                        break;
                }

            }
            catch { }
        }

        private void CloseThisWindow()
        {
            if (!IBoolInDoing) { this.Close(); }
        }

        private void MainPanel_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                var uie = e.OriginalSource as UIElement;
                if (e.Key == Key.Enter)
                {
                    uie.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                    e.Handled = true;
                }
            }
            catch { }
        }

        private void UMPSiteBindSetting_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void UMPSiteBindSetting_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (IBoolInDoing) { e.Cancel = true; return; }
            App.GSystemMainWindow.IOperationEvent -= GSystemMainWindow_IOperationEvent;
        }

        private void GSystemMainWindow_IOperationEvent(object sender, MamtOperationEventArgs e)
        {
            if (e.StrElementTag == "CSID")
            {
                App.DrawWindowsBackGround(this);
            }

            if (e.StrElementTag == "CLID")
            {
                DisplayElementCharacters(true);
            }

            ButtonApplicationMenu.ContextMenu = App.InitApplicationMenu();
        }

        private void InitServerAddressAndPort()
        {

            ComboBoxItem LComboBoxItemSetted = null;

            string LStrSettedIPAddress = IDataTableBindingInfo.Rows[0]["IPAddress"].ToString();
            string LStrSettedPort = IDataTableBindingInfo.Rows[0]["BindInfo"].ToString();

            string LStrTcpSettedPort = IDataTableBindingInfo.Rows[2]["BindInfo"].ToString();
            if (IDataTableBindingInfo.Rows[0]["Activated"].ToString() == "1")
            {
                CbDefaultUseHttp.IsChecked = true;
            }
            else
            {
                CbDefaultUseHttp.IsChecked = false;
            }
            if (IDataTableBindingInfo.Rows[3]["Attribute02"].ToString() == "1")
            {
                CbOpenCloud.IsChecked = true;
            }
            else
            {
                CbOpenCloud.IsChecked = false;
            }
            string[] LStrServerAddress = App.GStrComputerIPAddress.Split(App.GStrSpliterChar.ToCharArray());

            foreach (string LStrSingleServerAddress in LStrServerAddress)
            {
                if (string.IsNullOrEmpty(LStrSingleServerAddress)) { continue; }

                ComboBoxItem LComboBoxItemAddress = new ComboBoxItem();
                LComboBoxItemAddress.Margin = new Thickness(0, 1, 0, 1);
                LComboBoxItemAddress.Content = LStrSingleServerAddress;
                LComboBoxItemAddress.Height = 26;
                LComboBoxItemAddress.Style = (Style)App.Current.Resources["NormalFontStyle"];
                LComboBoxItemAddress.Background = Brushes.Transparent;
                ComboBoxBindingAddress.Items.Add(LComboBoxItemAddress);
                if (LStrSingleServerAddress == LStrSettedIPAddress) { LComboBoxItemSetted = LComboBoxItemAddress; }
            }

            ComboBoxItem LComboBoxItemServerName = new ComboBoxItem();
            LComboBoxItemServerName.Margin = new Thickness(0, 1, 0, 1);
            LComboBoxItemServerName.Content = Environment.MachineName;
            LComboBoxItemServerName.Height = 26;
            LComboBoxItemServerName.Style = (Style)App.Current.Resources["NormalFontStyle"];
            LComboBoxItemServerName.Background = Brushes.Transparent;
            ComboBoxBindingAddress.Items.Add(LComboBoxItemServerName);
            if (Environment.MachineName == LStrSettedIPAddress) { LComboBoxItemSetted = LComboBoxItemServerName; }

            ComboBoxItem LComboBoxItemServerPort8081 = new ComboBoxItem();
            LComboBoxItemServerPort8081.Margin = new Thickness(0, 1, 0, 1);
            LComboBoxItemServerPort8081.Content = "8081";
            LComboBoxItemServerPort8081.Height = 26;
            LComboBoxItemServerPort8081.Style = (Style)App.Current.Resources["NormalFontStyle"];
            LComboBoxItemServerPort8081.Background = Brushes.Transparent;
            ComboBoxBindingPort.Items.Add(LComboBoxItemServerPort8081);

            if (string.IsNullOrEmpty(LStrSettedPort))
            {
                ComboBoxBindingPort.Text = "8081";
                TextBoxBindingPort.Text = "8082";
            }
            else
            {
                ComboBoxBindingPort.Text = LStrSettedPort;
                TextBoxBindingPort.Text = (int.Parse(LStrSettedPort) + 1).ToString();
            }
            if (string.IsNullOrWhiteSpace(LStrTcpSettedPort))
            {
                TbNetTcpBindingPort.Text = "8083";
            }
            else
            {
                TbNetTcpBindingPort.Text = LStrTcpSettedPort;
            }

            if (LComboBoxItemSetted == null)
            {
                ComboBoxBindingAddress.SelectedIndex = 0;
            }
            else
            {
                LComboBoxItemSetted.IsSelected = true;
            }
        }

        #region 获取当前路径的父级路径
        private void GetSiteRootFolder()
        {
            int LIntLastIndexOf = 0;

            LIntLastIndexOf = App.GStrApplicationDirectory.LastIndexOf("\\");
            IStrSiteRootFolder = App.GStrApplicationDirectory.Substring(0, LIntLastIndexOf);
        }
        #endregion

        #region 设置 setup.exe 和 重新启动服务 UMP Service 00/01
        private BackgroundWorker IBackgroundWorkerApplyBinding = null;
        private bool IBoolApplyReturn = false;
        private string IStrApplyReturn = string.Empty;
        private string IStrBindHashString = string.Empty;

        private bool IsDefaultHttp = false;//是否默认http
        private bool IsOpenCloud = false;//是否启用云
        //执行批处理文件
        private void ExecuteBatchCommand(string AStrBatchFileName)
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

        //判断文件是否被占用
        private bool FileIsInUse(string AStrFileName)
        {
            bool LBoolReturn = true;
            FileStream LocalFileStream = null;

            try
            {
                LocalFileStream = new FileStream(AStrFileName, FileMode.Open, FileAccess.Read, FileShare.None);
                LBoolReturn = false;
            }
            catch { }
            finally
            {
                if (LocalFileStream != null) { LocalFileStream.Close(); }
            }

            return LBoolReturn;
        }

        private bool VerfiyInputParameteres()
        {
            bool LBoolReturn = true;
            string LStrBindPort = string.Empty;
            string LStrTcpBindPort = string.Empty;
            int LIntBindPort = 0;
            int LIntTcpBindPort = 0;

            try
            {
                LStrBindPort = ComboBoxBindingPort.Text.Trim();
                LStrTcpBindPort = TbNetTcpBindingPort.Text.Trim();
                if (string.IsNullOrEmpty(LStrBindPort) || string.IsNullOrEmpty(LStrTcpBindPort))
                {
                    MessageBox.Show(App.GetDisplayCharater("M01034"), App.GStrApplicationReferredTo, MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
                if (!int.TryParse(LStrBindPort, out LIntBindPort))
                {
                    MessageBox.Show(App.GetDisplayCharater("M01034"), App.GStrApplicationReferredTo, MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
                if (LIntBindPort < 8020 || LIntBindPort > 8999)
                {
                    MessageBox.Show(App.GetDisplayCharater("M01034"), App.GStrApplicationReferredTo, MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
                if (!int.TryParse(LStrTcpBindPort, out LIntTcpBindPort))
                {
                    MessageBox.Show(App.GetDisplayCharater("M01034"), App.GStrApplicationReferredTo, MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
                if (LIntTcpBindPort < 8020 || LIntTcpBindPort > 8999 || LIntTcpBindPort == LIntBindPort)
                {
                    MessageBox.Show(App.GetDisplayCharater("M01034"), App.GStrApplicationReferredTo, MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }
            }
            catch (Exception ex)
            {
                LBoolReturn = false;
                App.ShowExceptionMessage("VerfiyInputParameteres()\n" + ex.ToString());
            }

            return LBoolReturn;
        }

        //设置站点绑定的协议、端口
        private bool SetSiteProtolPort(List<string> AListStrSetting, ref string AStrReturn)
        {
            bool LBoolReturn = true;
            string LStrCallReturn = string.Empty;
            Site LSiteUMPPF = null;
            int LIntHttpBindingPort = 0;
            int LintHttpsBindingPort = 0;
            int LIntNetTcpBindingPort = 0;
            string LStrCertificateFile = string.Empty;
            string LStrCertificateHashString = string.Empty;
            byte[] LByteCertificateHash = null;

            try
            {
                App.GStrCatchException = string.Empty;
                AStrReturn = "006";
                ServerManager LServerManager = new ServerManager();
                foreach (Site LSiteSingle in LServerManager.Sites)
                {
                    if (LSiteSingle.Name.Equals("UMP.PF")) { LSiteUMPPF = LSiteSingle; AStrReturn = string.Empty; break; }
                }
                if (AStrReturn == "006") { return false; }

                App.WriteLog("SetBinding", string.Format("UMPSite getted."));

                LSiteUMPPF.Bindings.Clear();
                LServerManager.CommitChanges();
                LServerManager.Dispose();
                LServerManager = null;

                LBoolReturn = CertificateOperations.UninstallCertificate(AListStrSetting[0], StoreName.My, StoreLocation.CurrentUser, ref LStrCallReturn);
                LBoolReturn = CertificateOperations.UninstallCertificate(AListStrSetting[0], StoreName.My, StoreLocation.LocalMachine, ref LStrCallReturn);
                LBoolReturn = CertificateOperations.UninstallCertificate(AListStrSetting[0], StoreName.Root, StoreLocation.LocalMachine, ref LStrCallReturn);

                LStrCertificateFile = System.IO.Path.Combine(App.GStrSiteRootFolder, @"Components\Certificates", "UMP.S." + AListStrSetting[0] + ".pfx");
                if (File.Exists(LStrCertificateFile)) { File.Delete(LStrCertificateFile); }
                LBoolReturn = CertificateOperations.CreateCertificate(AListStrSetting[0], ref LStrCallReturn);
                if (!LBoolReturn)
                {
                    AStrReturn = "013";             //在当前用户的 My 区域中创建证书失败
                    App.GStrCatchException = LStrCallReturn;
                    return LBoolReturn;
                }

                App.WriteLog("SetBinding", string.Format("Server certificate created.\t{0}", LStrCallReturn));

                LStrCertificateHashString = LStrCallReturn;
                IStrBindHashString = LStrCertificateHashString;
                LBoolReturn = CertificateOperations.ExportCertificate(LStrCertificateHashString, "VoiceCyber,123", LStrCertificateFile, ref LStrCallReturn);
                if (!LBoolReturn)
                {
                    AStrReturn = "014";             //从当前用户的 My 区域中导出证书失败
                    App.GStrCatchException = LStrCallReturn;
                    return LBoolReturn;
                }

                App.WriteLog("SetBinding", string.Format("Server certificate exported.\t{0}", LStrCertificateFile));

                LBoolReturn = CertificateOperations.CertificateIsExist(LStrCertificateHashString, StoreName.My, StoreLocation.LocalMachine, ref LStrCallReturn);
                if (!LBoolReturn)
                {
                    if (!string.IsNullOrEmpty(LStrCallReturn))
                    {
                        AStrReturn = "018";             //检测本计算机的 My 区域中是否安装证书失败
                        App.GStrCatchException = LStrCallReturn;
                        return LBoolReturn;
                    }
                    LBoolReturn = CertificateOperations.InstallCertificate(LStrCertificateFile, "VoiceCyber,123", StoreName.My, StoreLocation.LocalMachine, ref LStrCallReturn);
                    if (!LBoolReturn)
                    {
                        AStrReturn = "015";             //向本计算机的 My 区域中安装证书失败
                        App.GStrCatchException = LStrCallReturn;
                        return LBoolReturn;
                    }

                    App.WriteLog("SetBinding", string.Format("Install server certificate end.\t{0}", "My"));
                }

                LByteCertificateHash = CertificateOperations.ObtainCertificateCertHash(LStrCertificateHashString, StoreName.My, StoreLocation.LocalMachine, ref LStrCallReturn);
                if (LByteCertificateHash == null)
                {
                    LBoolReturn = false;
                    AStrReturn = "016";             //从本计算机的 My 区域中获取证书的哈希值数组失败
                    App.GStrCatchException = LStrCallReturn;
                    return LBoolReturn;
                }

                App.WriteLog("SetBinding", string.Format("ObtainCertificateCertHash end."));

                LBoolReturn = CertificateOperations.CertificateIsExist(LStrCertificateHashString, StoreName.Root, StoreLocation.LocalMachine, ref LStrCallReturn);
                if (!LBoolReturn)
                {
                    if (!string.IsNullOrEmpty(LStrCallReturn))
                    {
                        AStrReturn = "019";             //检测本计算机的 Root 区域中是否安装证书失败
                        App.GStrCatchException = LStrCallReturn;
                        return LBoolReturn;
                    }
                    LBoolReturn = CertificateOperations.InstallCertificate(LStrCertificateFile, "VoiceCyber,123", StoreName.Root, StoreLocation.LocalMachine, ref LStrCallReturn);
                    if (!LBoolReturn)
                    {
                        AStrReturn = "017";             //向本计算机的 Root 区域中安装证书失败
                        App.GStrCatchException = LStrCallReturn;
                        return LBoolReturn;
                    }

                    App.WriteLog("SetBinding", string.Format("Install server certificate end.\t{0}", "Root"));
                }

                LServerManager = new ServerManager();
                foreach (Site LSiteSingle in LServerManager.Sites)
                {
                    if (LSiteSingle.Name.Equals("UMP.PF")) { LSiteUMPPF = LSiteSingle; AStrReturn = string.Empty; break; }
                }
                LIntHttpBindingPort = int.Parse(AListStrSetting[1]);
                LintHttpsBindingPort = LIntHttpBindingPort + 1;
                
                LSiteUMPPF.Bindings.Add("*:" + AListStrSetting[1] + ":", "http");
                LSiteUMPPF.Bindings.Add("*:" + LintHttpsBindingPort.ToString() + ":", LByteCertificateHash, "MY");
                LSiteUMPPF.Bindings.Add(AListStrSetting[2] + ":*", "net.tcp");

                App.WriteLog("SetBinding", string.Format("Add binding end."));

                //LSiteUMPPF.ApplicationDefaults.EnabledProtocols
                var app = LSiteUMPPF.Applications["/WCF1600"];
                if (app != null)
                {
                    app.EnabledProtocols = "http,net.tcp";

                    App.WriteLog("SetBinding", string.Format("Set protocol for WCF1600 end."));
                }
                LServerManager.CommitChanges();
                LServerManager.Dispose();
                LServerManager = null;
            }
            catch (Exception ex)
            {
                LBoolReturn = false;
                AStrReturn = "010";
                App.GStrCatchException = "SetSiteProtolPort()" + ex.ToString();
            }

            return LBoolReturn;
        }

        //设置目录的访问权限
        private bool SetPathPermission(ref string AStrReturn)
        {
            bool LBoolReturn = true;

            string LStrProgramDataDirectory = string.Empty;
            string LStrGlobalSettingsDirectory = string.Empty;
            string LStrLayoutsDirectory = string.Empty;
            List<string> LListStrShareFolder = new List<string>();

            try
            {
                AStrReturn = string.Empty;
                LStrProgramDataDirectory = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                if (!Directory.Exists(System.IO.Path.Combine(LStrProgramDataDirectory, "UMP.Server")))
                {
                    Directory.CreateDirectory(System.IO.Path.Combine(LStrProgramDataDirectory, "UMP.Server"));
                }
                DirectoryInfo LDirectoryInfoProgramData = new DirectoryInfo(System.IO.Path.Combine(LStrProgramDataDirectory, "UMP.Server"));
                DirectorySecurity LDirectorySecurityProgramData = LDirectoryInfoProgramData.GetAccessControl();
                LDirectorySecurityProgramData.AddAccessRule(new FileSystemAccessRule("Everyone", FileSystemRights.FullControl, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow));
                LDirectoryInfoProgramData.SetAccessControl(LDirectorySecurityProgramData);

                App.WriteLog("SetDirPermission", string.Format("Set directory permission end.\t{0}", "UMP.Server"));

                LListStrShareFolder.Add(System.IO.Path.Combine(IStrSiteRootFolder, "GlobalSettings"));
                LListStrShareFolder.Add(System.IO.Path.Combine(IStrSiteRootFolder, "Layouts"));
                LListStrShareFolder.Add(System.IO.Path.Combine(IStrSiteRootFolder, "MediaData"));

                foreach (string LStrSingleFolder in LListStrShareFolder)
                {
                    if (!System.IO.Directory.Exists(LStrSingleFolder)) { Directory.CreateDirectory(LStrSingleFolder); }
                    DirectoryInfo LDirectoryInfoSingle = new DirectoryInfo(LStrSingleFolder);
                    DirectorySecurity LDirectorySecuritySingle = LDirectoryInfoSingle.GetAccessControl();
                    LDirectorySecuritySingle.AddAccessRule(new FileSystemAccessRule("Everyone", FileSystemRights.FullControl, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow));
                    LDirectoryInfoSingle.SetAccessControl(LDirectorySecuritySingle);

                    App.WriteLog("SetDirPermission", string.Format("Set directory permission end.\t{0}", LStrSingleFolder));
                }

                App.WriteLog("SetDirPermission", string.Format("Set directory permission end."));
            }
            catch (Exception ex)
            {
                LBoolReturn = false;
                AStrReturn = "007";
                App.GStrCatchException = "SetPathPermission()\n" + ex.ToString();
            }

            return LBoolReturn;
        }

        //重新设置 setup.exe
        private bool ResetSetupExe(List<string> AListStrSetting, ref string AStrReturn)
        {
            bool LBoolReturn = true;
            string[] LStrArrayWriteLine = new string[3];
            string LStrHttpsPort = string.Empty;

            try
            {
                AStrReturn = string.Empty;
                LStrHttpsPort = (int.Parse(AListStrSetting[1]) + 1).ToString();

                Stream LStreamBatch = File.Create(IStrSiteRootFolder + @"\ResetConfig.bat");
                LStreamBatch.Close();

                while (FileIsInUse(IStrSiteRootFolder + @"\setup.exe")) { System.Threading.Thread.Sleep(500); }
                while (FileIsInUse(IStrSiteRootFolder + @"\ClientSetup.exe")) { System.Threading.Thread.Sleep(500); }
                System.Threading.Thread.Sleep(1000);

                string defaultHttp = IsDefaultHttp == true ? "http://" : "https://";
                LStrHttpsPort = IsDefaultHttp == true ? AListStrSetting[1].ToString() : LStrHttpsPort;
                LStrArrayWriteLine[0] = "cd \"" + IStrSiteRootFolder + "\"";
                LStrArrayWriteLine[1] = @"setup -url=" + defaultHttp + AListStrSetting[0] + ":" + LStrHttpsPort + " -componentsurl=http://" + AListStrSetting[0] + ":" + AListStrSetting[1] + " -homesite=false";
                LStrArrayWriteLine[2] = @"ClientSetup -url=" + defaultHttp + AListStrSetting[0] + ":" + LStrHttpsPort + " -componentsurl=http://" + AListStrSetting[0] + ":" + AListStrSetting[1] + " -homesite=false";

                File.WriteAllLines(IStrSiteRootFolder + @"\ResetConfig.bat", LStrArrayWriteLine);
                ExecuteBatchCommand(IStrSiteRootFolder + @"\ResetConfig.bat");
                File.Delete(IStrSiteRootFolder + @"\ResetConfig.bat");
            }
            catch (Exception ex)
            {
                LBoolReturn = false;
                AStrReturn = "008";
                App.GStrCatchException = "ResetSetupExe()\n" + ex.ToString();
            }

            return LBoolReturn;
        }

        //安装ODAC
        private bool InstallOracleODAC(ref string AStrReturn)
        {
            bool LBoolReturn = true;
            string LStrSystemDriver = string.Empty;
            string LStrODACFolder = string.Empty;

            try
            {
                AStrReturn = string.Empty;
                LStrSystemDriver = Environment.GetFolderPath(Environment.SpecialFolder.System);
                LStrSystemDriver = LStrSystemDriver.Substring(0, 2);
                if (Directory.Exists(LStrSystemDriver + @"\YoungODAC")) { return true; }
                string[] LStrArrayWriteLine = new string[2];
                Stream LStreamBatch = File.Create(IStrSiteRootFolder + @"\InstallODAC.bat");
                LStreamBatch.Close();
                if (Environment.Is64BitOperatingSystem)
                {
                    LStrODACFolder = System.IO.Path.Combine(IStrSiteRootFolder, @"Components\ODACx64");
                }
                else
                {
                    LStrODACFolder = System.IO.Path.Combine(IStrSiteRootFolder, @"Components\ODACx86");
                }
                LStrArrayWriteLine[0] = "cd \"" + LStrODACFolder + "\"";
                LStrArrayWriteLine[1] = "install.bat ODP.NET4 " + LStrSystemDriver + @"\YoungODAC" + " ODAC";
                File.WriteAllLines(IStrSiteRootFolder + @"\InstallODAC.bat", LStrArrayWriteLine);
                ExecuteBatchCommand(IStrSiteRootFolder + @"\InstallODAC.bat");
                File.Delete(IStrSiteRootFolder + @"\InstallODAC.bat");
            }
            catch (Exception ex)
            {
                LBoolReturn = false;
                AStrReturn = "011";
                App.GStrCatchException = "InstallOracleODAC()\n" + ex.ToString();
            }

            return LBoolReturn;
        }

        //重新启动服务
        private bool ReStartUMPServices(ref string AStrReturn)
        {
            bool LBoolReturn = true;

            try
            {
                AStrReturn = string.Empty;
                string[] LStrArrayWriteLine = new string[8];
                Stream LStreamBatch = File.Create(IStrSiteRootFolder + @"\RestarService.bat");
                LStreamBatch.Close();
                LStrArrayWriteLine[0] = "net stop \"UMP Service 00\"";
                LStrArrayWriteLine[1] = "net start \"UMP Service 00\"";
                LStrArrayWriteLine[2] = "net stop \"UMP Service 01\"";
                LStrArrayWriteLine[3] = "net start \"UMP Service 01\"";
                LStrArrayWriteLine[4] = "net stop \"UMP Service 02\"";
                LStrArrayWriteLine[5] = "net start \"UMP Service 02\"";
                LStrArrayWriteLine[6] = "net stop \"UMP Service 03\"";
                LStrArrayWriteLine[7] = "net start \"UMP Service 03\"";
                File.WriteAllLines(IStrSiteRootFolder + @"\RestarService.bat", LStrArrayWriteLine);
                ExecuteBatchCommand(IStrSiteRootFolder + @"\RestarService.bat");
                File.Delete(IStrSiteRootFolder + @"\RestarService.bat");

                App.WriteLog("RestartService", string.Format("Restart service end."));
            }
            catch (Exception ex)
            {
                LBoolReturn = false;
                AStrReturn = "009";
                App.GStrCatchException = "ReStartUMPServices()\n" + ex.ToString();
            }

            return LBoolReturn;
        }

        //重新启动 IIS 服务
        private bool ReStartIISService(ref string AStrReturn)
        {
            bool LBoolReturn = true;

            try
            {
                AStrReturn = string.Empty;
                string[] LStrArrayWriteLine = new string[1];
                Stream LStreamBatch = File.Create(IStrSiteRootFolder + @"\RestarIISService.bat");
                LStreamBatch.Close();
                LStrArrayWriteLine[0] = "IISRESET";
                File.WriteAllLines(IStrSiteRootFolder + @"\RestarIISService.bat", LStrArrayWriteLine);
                ExecuteBatchCommand(IStrSiteRootFolder + @"\RestarIISService.bat");
                File.Delete(IStrSiteRootFolder + @"\RestarIISService.bat");

                App.WriteLog("RestartIISService", string.Format("Restart IIS service end."));
            }
            catch (Exception ex)
            {
                LBoolReturn = false;
                AStrReturn = "012";
                App.GStrCatchException = "ReStartIISService()\n" + ex.ToString();
            }

            return LBoolReturn;
        }

        //重新设置 UMP.PF.html / i-AC.html
        private bool ResetIndexHtml(List<string> AListStrSetting, ref string AStrReturn)
        {
            bool LBoolReturn = true;
            string LStrHttpsPort = string.Empty;

            try
            {
                AStrReturn = string.Empty;

                string LStrIndexIMC = System.IO.Path.Combine(IStrSiteRootFolder, "UMP.PF.html");
                string LStrIndexIAC = System.IO.Path.Combine(IStrSiteRootFolder, "i-AC.html");
                //string LStr3104Application = System.IO.Path.Combine(IStrSiteRootFolder, "UMPS3104.application");
                //string LStrFiles3104Application = System.IO.Path.Combine(IStrSiteRootFolder, @"Application Files\UMPS3104_8_02_001_1\UMPS3104.application");

                LStrHttpsPort = (int.Parse(AListStrSetting[1]) + 1).ToString();

                Regex LRegexXbapSimple = new Regex("<a href=\u0022UMPMain.xbap\u0022>");
                Regex LRegexXbapFull = new Regex("<a href=\u0022http[s]?://[a-zA-Z0-9_:\\.\\-]+/UMPMain.xbap\u0022>");
                string[] LStrArrayIMC = File.ReadAllLines(LStrIndexIMC, Encoding.UTF8);
                string defaultHttp = IsDefaultHttp == true ? "http" : "https";
                LStrHttpsPort = IsDefaultHttp == true ? AListStrSetting[1].ToString() : LStrHttpsPort;
                for (int LIntLoopLine = 0; LIntLoopLine < LStrArrayIMC.Length; LIntLoopLine++)
                {
                    if (LRegexXbapFull.IsMatch(LStrArrayIMC[LIntLoopLine]))
                    {
                        LStrArrayIMC[LIntLoopLine] = LRegexXbapFull.Replace(LStrArrayIMC[LIntLoopLine], string.Format("<a href=\"{0}://{1}:{2}/UMPMain.xbap\">", defaultHttp, AListStrSetting[0], LStrHttpsPort));
                    }

                    if (LRegexXbapSimple.IsMatch(LStrArrayIMC[LIntLoopLine]))
                    {
                        LStrArrayIMC[LIntLoopLine] = LRegexXbapSimple.Replace(LStrArrayIMC[LIntLoopLine], string.Format("<a href=\"{0}://{1}:{2}/UMPMain.xbap\">", defaultHttp, AListStrSetting[0], LStrHttpsPort));
                    }
                }
                File.WriteAllLines(LStrIndexIMC, LStrArrayIMC, Encoding.UTF8);

                Regex LRegexS3104Simple = new Regex("<a href=\u0022UMPS3104.application\u0022>");
                Regex LRegexS3104Full = new Regex("<a href=\u0022http[s]?://[a-zA-Z0-9_:\\.\\-]+/UMPS3104.application\u0022>");
                string[] LStrArrayIAC = File.ReadAllLines(LStrIndexIAC, Encoding.UTF8);
                for (int LIntLoopLine = 0; LIntLoopLine < LStrArrayIAC.Length; LIntLoopLine++)
                {
                    if (LRegexS3104Full.IsMatch(LStrArrayIAC[LIntLoopLine]))
                    {
                        LStrArrayIAC[LIntLoopLine] = LRegexS3104Full.Replace(LStrArrayIAC[LIntLoopLine], string.Format("<a href=\"{0}://{1}:{2}/UMPS3104.application\">", "http", AListStrSetting[0], AListStrSetting[1]));
                    }

                    if (LRegexS3104Simple.IsMatch(LStrArrayIAC[LIntLoopLine]))
                    {
                        LStrArrayIAC[LIntLoopLine] = LRegexS3104Simple.Replace(LStrArrayIAC[LIntLoopLine], string.Format("<a href=\"{0}://{1}:{2}/UMPS3104.application\">", "http", AListStrSetting[0], AListStrSetting[1]));
                    }
                }
                File.WriteAllLines(LStrIndexIAC, LStrArrayIAC, Encoding.UTF8);


                //try { File.Delete(IStrSiteRootFolder + @"\UMPS3104.application"); }
                //catch { }
                //try { File.Delete(IStrSiteRootFolder + @"\Application Files\UMPS3104_8_02_001_1\UMPS3104.application"); }
                //catch { }
                //try { File.Delete(IStrSiteRootFolder + @"\Application Files\UMPS3104_8_02_001_1\UMPS3104.exe.manifest"); }
                //catch { }

                //string[] LStrArrayWriteBatLine = new string[7];
                //Stream LStreamBatch = File.Create(IStrSiteRootFolder + @"\MageConfig.bat");
                //LStreamBatch.Close();

                //LStrArrayWriteBatLine[0] = "cd \"" + IStrSiteRootFolder + "\"" ;
                //LStrArrayWriteBatLine[1] = "mage -n Application -t \"" + IStrSiteRootFolder + "\\Application Files\\UMPS3104_8_02_001_1\\UMPS3104.exe.manifest\" -n UMPS3104.exe -v 8.02.001.1 -fd \"" + IStrSiteRootFolder + "\\Application Files\\UMPS3104_8_02_001_1\" -IconFile \"00000038.ico\"";
                //LStrArrayWriteBatLine[2] = "mage -s \"" + IStrSiteRootFolder + "\\Application Files\\UMPS3104_8_02_001_1\\UMPS3104.exe.manifest\" -cf \"" + IStrSiteRootFolder + "\\Components\\Certificates\\UMP.PF.Certificate.pfx\" -pwd \"VoiceCyber,123\"";
                //LStrArrayWriteBatLine[3] = "mage -n Deployment -t \"" + IStrSiteRootFolder + "\\Application Files\\UMPS3104_8_02_001_1\\UMPS3104.application\" -n \"UMP Interlligent Client\" -v 8.02.001.1 -appm \"" + IStrSiteRootFolder + "\\Application Files\\UMPS3104_8_02_001_1\\UMPS3104.exe.manifest\" -install true -Publisher \"VoiceCyber\"";
                //LStrArrayWriteBatLine[4] = "mage -s \"" + IStrSiteRootFolder + "\\Application Files\\UMPS3104_8_02_001_1\\UMPS3104.application\" -cf \"" + IStrSiteRootFolder + "\\Components\\Certificates\\UMP.PF.Certificate.pfx\" -pwd \"VoiceCyber,123\"";
                //LStrArrayWriteBatLine[5] = "mage -n Deployment -t \"" + IStrSiteRootFolder + "\\UMPS3104.application\" -n \"UMP Interlligent Client\" -v 8.02.001.1 -appm \"" + IStrSiteRootFolder + "\\Application Files\\UMPS3104_8_02_001_1\\UMPS3104.exe.manifest\" -install true -Publisher \"VoiceCyber\"";
                //LStrArrayWriteBatLine[6] = "mage -s \"" + IStrSiteRootFolder + "\\UMPS3104.application\" -cf \"" + IStrSiteRootFolder + "\\Components\\Certificates\\UMP.PF.Certificate.pfx\" -pwd \"VoiceCyber,123\"";

                //File.WriteAllLines(IStrSiteRootFolder + @"\MageConfig.bat", LStrArrayWriteBatLine);
                //ExecuteBatchCommand(IStrSiteRootFolder + @"\MageConfig.bat");
                //File.Delete(IStrSiteRootFolder + @"\MageConfig.bat");
            }
            catch (Exception ex)
            {
                LBoolReturn = false;
                AStrReturn = "020";
                App.GStrCatchException = "ResetIndexHtml()\n" + ex.ToString();
            }

            return LBoolReturn;
        }

        //写入GlobalSettings.UMP.Server.01.xml
        private bool WriteGlobalSettingsServer01Xml(List<string> AListStrSetting, ref string AStrReturn, bool defaultHttp,bool openCloud)
        {
            bool LBoolReturn = true;
            string LStrXmlFileName = string.Empty;

            try
            {
                LStrXmlFileName = System.IO.Path.Combine(IStrSiteRootFolder, @"GlobalSettings\UMP.Server.01.xml");
                XmlDocument LXmlDocServer01 = new XmlDocument();
                LXmlDocServer01.Load(LStrXmlFileName);

                App.WriteLog("WriteXml", string.Format("Xml loaded.\t{0}", LStrXmlFileName));

                XmlNodeList LXmlNodeListIISBindingProtocol = LXmlDocServer01.SelectSingleNode("UMPSetted").SelectSingleNode("IISBindingProtocol").ChildNodes;

                foreach (XmlNode LXmlNodeBindingProtocol in LXmlNodeListIISBindingProtocol)
                {
                    if (LXmlNodeBindingProtocol.Attributes["Protocol"].Value == "http")
                    {
                        LXmlNodeBindingProtocol.Attributes["BindInfo"].Value = AListStrSetting[1];
                        LXmlNodeBindingProtocol.Attributes["IPAddress"].Value = AListStrSetting[0];
                        LXmlNodeBindingProtocol.Attributes["Used"].Value = "1";
                        if (defaultHttp)
                        {
                            LXmlNodeBindingProtocol.Attributes["Activated"].Value = "1";
                        }
                        else
                        {
                            LXmlNodeBindingProtocol.Attributes["Activated"].Value = "0";
                        }
                    }
                    if (LXmlNodeBindingProtocol.Attributes["Protocol"].Value == "https")
                    {
                        LXmlNodeBindingProtocol.Attributes["BindInfo"].Value = (int.Parse(AListStrSetting[1]) + 1).ToString();
                        LXmlNodeBindingProtocol.Attributes["IPAddress"].Value = AListStrSetting[0];
                        LXmlNodeBindingProtocol.Attributes["OtherArgs"].Value = IStrBindHashString;
                        LXmlNodeBindingProtocol.Attributes["Used"].Value = "1";
                        if (defaultHttp)
                        {
                            LXmlNodeBindingProtocol.Attributes["Activated"].Value = "0";
                        }
                        else
                        {
                            LXmlNodeBindingProtocol.Attributes["Activated"].Value = "1";
                        }
                    }
                    if (LXmlNodeBindingProtocol.Attributes["Protocol"].Value == "net.tcp")
                    {
                        LXmlNodeBindingProtocol.Attributes["BindInfo"].Value = AListStrSetting[2];
                        LXmlNodeBindingProtocol.Attributes["IPAddress"].Value = AListStrSetting[0];
                        LXmlNodeBindingProtocol.Attributes["Used"].Value = "1";
                    }
                }
                LXmlDocServer01.SelectSingleNode("UMPSetted").SelectSingleNode("UMPApplication").Attributes["Attribute02"].Value = openCloud == true ? "1" : "0";
                LXmlDocServer01.Save(LStrXmlFileName);

                App.WriteLog("WriteXml", string.Format("Xml save end.\t{0}", LStrXmlFileName));
            }
            catch (Exception ex)
            {
                LBoolReturn = false;
                AStrReturn = "010";
                App.GStrCatchException = "WriteGlobalSettingsServer01Xml()\n" + ex.ToString();
            }
            return LBoolReturn;
        }

        private void BeginSetBingdingParameters()
        {
            string LStrServerAddress = string.Empty;
            string LStrServerPort = string.Empty;
            List<string> LListStrParameter = new List<string>();

            try
            {
                if (IBoolInDoing) { return; }

                if (!VerfiyInputParameteres()) { return; }
                if (ComboBoxBindingAddress.SelectedItem == null)
                {
                    LStrServerAddress = ComboBoxBindingAddress.Text;
                }
                else
                {
                    LStrServerAddress = (ComboBoxBindingAddress.SelectedItem as ComboBoxItem).Content.ToString();
                }
                LStrServerPort = ComboBoxBindingPort.Text.Trim();
                LListStrParameter.Add(LStrServerAddress);
                LListStrParameter.Add(LStrServerPort);
                LListStrParameter.Add(TbNetTcpBindingPort.Text.Trim());

                for (int i = 0; i < LListStrParameter.Count; i++)
                {
                    App.WriteLog("SetBinding", string.Format("Param{0}:{1}", i, LListStrParameter[i]));
                }

                IsDefaultHttp = CbDefaultUseHttp.IsChecked == true?true:false;
                IsOpenCloud = CbOpenCloud.IsChecked == true ? true : false;

                IBoolInDoing = true;
                App.ShowCurrentStatus(1, App.GetDisplayCharater("M01035"));

                IBackgroundWorkerApplyBinding = new BackgroundWorker();
                IBackgroundWorkerApplyBinding.WorkerReportsProgress = true;
                IBackgroundWorkerApplyBinding.RunWorkerCompleted += IBackgroundWorkerApplyBinding_RunWorkerCompleted;
                IBackgroundWorkerApplyBinding.DoWork += IBackgroundWorkerApplyBinding_DoWork;
                IBackgroundWorkerApplyBinding.ProgressChanged += IBackgroundWorkerApplyBinding_ProgressChanged;
                IBackgroundWorkerApplyBinding.RunWorkerAsync(LListStrParameter);
            }
            catch (Exception ex)
            {
                IBoolInDoing = false;
                if (IBackgroundWorkerApplyBinding != null)
                {
                    IBackgroundWorkerApplyBinding.Dispose();
                    IBackgroundWorkerApplyBinding = null;
                }
                App.ShowExceptionMessage("BeginSetBingdingParameters()\n" + ex.ToString());
            }

        }

        private void IBackgroundWorkerApplyBinding_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            App.ShowCurrentStatus(1, App.GetDisplayCharater("P01" + e.ProgressPercentage.ToString("000")));
        }

        private void IBackgroundWorkerApplyBinding_DoWork(object sender, DoWorkEventArgs e)
        {

            List<string> LListStrParameters = e.Argument as List<string>;

            IBackgroundWorkerApplyBinding.ReportProgress(1);
            IBoolApplyReturn = SetSiteProtolPort(LListStrParameters, ref IStrApplyReturn);
            System.Threading.Thread.Sleep(1000);
            if (!IBoolApplyReturn) { return; }

            IBackgroundWorkerApplyBinding.ReportProgress(2);
            IBoolApplyReturn = SetPathPermission(ref IStrApplyReturn);
            System.Threading.Thread.Sleep(1000);
            if (!IBoolApplyReturn) { return; }

            //IBackgroundWorkerApplyBinding.ReportProgress(3);
            //IBoolApplyReturn = ResetSetupExe(LListStrParameters, ref IStrApplyReturn);
            //System.Threading.Thread.Sleep(1000);
            //if (!IBoolApplyReturn) { return; }

            //IBackgroundWorkerApplyBinding.ReportProgress(6);
            //IBoolApplyReturn = InstallOracleODAC(ref IStrApplyReturn);
            //System.Threading.Thread.Sleep(1000);
            //if (!IBoolApplyReturn) { return; }

            IBackgroundWorkerApplyBinding.ReportProgress(5);
            IBoolApplyReturn = WriteGlobalSettingsServer01Xml(LListStrParameters, ref IStrApplyReturn, IsDefaultHttp,IsOpenCloud);
            System.Threading.Thread.Sleep(1000);
            if (!IBoolApplyReturn) { return; }

            IBackgroundWorkerApplyBinding.ReportProgress(8);
            IBoolApplyReturn = ResetIndexHtml(LListStrParameters, ref IStrApplyReturn);
            System.Threading.Thread.Sleep(1000);
            if (!IBoolApplyReturn) { return; }

            IBackgroundWorkerApplyBinding.ReportProgress(4);
            IBoolApplyReturn = ReStartUMPServices(ref IStrApplyReturn);
            System.Threading.Thread.Sleep(1000);
            if (!IBoolApplyReturn) { return; }

            IBackgroundWorkerApplyBinding.ReportProgress(7);
            IBoolApplyReturn = ReStartIISService(ref IStrApplyReturn);
            System.Threading.Thread.Sleep(1000);
            if (!IBoolApplyReturn) { return; }
        }

        private void IBackgroundWorkerApplyBinding_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            IBoolInDoing = false;
            App.ShowCurrentStatus(int.MaxValue, string.Empty);
            IBackgroundWorkerApplyBinding.Dispose();
            IBackgroundWorkerApplyBinding = null;

            if (!IBoolApplyReturn)
            {
                if (IStrApplyReturn == "006")
                {
                    MessageBox.Show(App.GetDisplayCharater("E00" + IStrApplyReturn), App.GStrApplicationReferredTo, MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    MessageBox.Show(App.GetDisplayCharater("E00" + IStrApplyReturn) + "\n" + App.GStrCatchException, App.GStrApplicationReferredTo, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MamtOperationEventArgs LEventArgs = new MamtOperationEventArgs();
                LEventArgs.StrElementTag = "RIISB";
                if (IOperationEvent != null) { IOperationEvent(this, LEventArgs); }
                MessageBox.Show(App.GetDisplayCharater("M01036"), App.GStrApplicationReferredTo, MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        #endregion
    }
}
