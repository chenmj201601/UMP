using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using UMP.MAMT.PublicClasses;

namespace UMP.MAMT.CertificateSetting
{
    public partial class CertificateInstalling : Window, MamtOperationsInterface
    {
        [DllImport("advapi32.dll")]
        private static extern bool LogonUser(string lpszUsername, string lpszDomain, string lpszPassword, int dwLogonType, int dwLogonProvider, ref IntPtr phToken);

        public event EventHandler<MamtOperationEventArgs> IOperationEvent;

        //是否在处理的过程中
        private bool IBoolInDoing = false;

        public CertificateInstalling()
        {
            InitializeComponent();
            this.Loaded += CertificateInstalling_Loaded;
            this.Closing += CertificateInstalling_Closing;
            this.MouseLeftButtonDown += CertificateInstalling_MouseLeftButtonDown;

            MainPanel.KeyDown += MainPanel_KeyDown;

            ButtonApplicationMenu.Click += WindowsButtonClicked;
            ButtonCloseInstall.Click += WindowsButtonClicked;
            ButtonCertificateInstall.Click += WindowsButtonClicked;
            ButtonCloseWindow.Click += WindowsButtonClicked;
        }

        private void DisplayElementCharacters(bool ABoolLanguageChange)
        {
            LabelCertificateInstallTip.Content = App.GetDisplayCharater("M01051");
            TabItemAdministator.Header = " " + App.GetDisplayCharater("M01050") + " ";
            LabelAccount.Content = App.GetDisplayCharater("M01045");
            LabelPassword.Content = App.GetDisplayCharater("M01046");
            ButtonCertificateInstall.Content = App.GetDisplayCharater("M01047");
            ButtonCloseWindow.Content = App.GetDisplayCharater("M01048");
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
                    case "ButtonCloseInstall":
                        CloseThisWindow();
                        break;
                    case "ButtonCertificateInstall":
                        BeginInstallCertificate();
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

        private void CertificateInstalling_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void CertificateInstalling_Closing(object sender, System.ComponentModel.CancelEventArgs e)
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

        private void CertificateInstalling_Loaded(object sender, RoutedEventArgs e)
        {
            App.DrawWindowsBackGround(this);
            ButtonApplicationMenu.ContextMenu = App.InitApplicationMenu();
            App.GSystemMainWindow.IOperationEvent += GSystemMainWindow_IOperationEvent;
            ImageCertificateInstall.Source = new BitmapImage(new Uri(System.IO.Path.Combine(App.GStrApplicationDirectory, @"Images\00000023.ico"), UriKind.RelativeOrAbsolute));

            TextBoxAccount.Text = Environment.UserName;

            DisplayElementCharacters(false);

            PasswordBoxPassword.Focus();
        }

        #region 安装安全证书，并设置 Service ** 登录身份
        private BackgroundWorker IBackgroundWorkerApplyInstalling = null;
        private bool IBoolApplyReturn = false;
        private string IStrApplyReturn = string.Empty;

        //验证当前用户的登录密码
        private bool VerfiyInputParameteres()
        {
            bool LBoolReturn = true;
            string LStrAccount = string.Empty;
            string LStrPassword = string.Empty;

            try
            {
                LStrAccount = TextBoxAccount.Text;
                LStrPassword = PasswordBoxPassword.Password;
                if (string.IsNullOrEmpty(LStrPassword)) { LStrPassword = ""; }

                IntPtr LIntPtrTokenHandle = new IntPtr(0);
                LIntPtrTokenHandle = IntPtr.Zero;

                bool LBoolLogonReturn = LogonUser(LStrAccount, System.Environment.MachineName, LStrPassword, 2, 0, ref LIntPtrTokenHandle);
                if (!LBoolReturn)
                {
                    MessageBox.Show(App.GetDisplayCharater("M01043"), App.GStrApplicationReferredTo, MessageBoxButton.OK, MessageBoxImage.Error);
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

        //重新启动服务
        private bool RestartUMPServices(List<string> AListStrSetting, ref string AStrReturn)
        {
            bool LBoolReturn = true;
            string LStrBatchFile = string.Empty;

            try
            {
                AStrReturn = string.Empty;
                LStrBatchFile = App.GStrSiteRootFolder + @"\ResetAndRestartService.bat";

                Stream LStreamBatch = File.Create(LStrBatchFile);
                LStreamBatch.Close();

                //string[] LStrArrayWriteLine = new string[6];
                
                //if (AListStrSetting[0].Contains(" "))
                //{
                //    LStrArrayWriteLine[0] = "sc config \"UMP Service 00\" obj=  \".\\" + AListStrSetting[0] + "\" password= \"" + AListStrSetting[1] + "\"";
                //    LStrArrayWriteLine[1] = "sc config \"UMP Service 01\" obj=  \".\\" + AListStrSetting[0] + "\" password= \"" + AListStrSetting[1] + "\"";
                //}
                //else
                //{
                //    LStrArrayWriteLine[0] = "sc config \"UMP Service 00\" obj= .\\" + AListStrSetting[0] + " password= " + AListStrSetting[1];
                //    LStrArrayWriteLine[1] = "sc config \"UMP Service 01\" obj= .\\" + AListStrSetting[0] + " password= " + AListStrSetting[1];
                //}

                //LStrArrayWriteLine[2] = "net stop \"UMP Service 00\"";
                //LStrArrayWriteLine[3] = "net start \"UMP Service 00\"";
                //LStrArrayWriteLine[4] = "net stop \"UMP Service 01\"";
                //LStrArrayWriteLine[5] = "net start \"UMP Service 01\"";

                string[] LStrArrayWriteLine = new string[4];

                LStrArrayWriteLine[0] = "net stop \"UMP Service 00\"";
                LStrArrayWriteLine[1] = "net start \"UMP Service 00\"";
                LStrArrayWriteLine[2] = "net stop \"UMP Service 01\"";
                LStrArrayWriteLine[3] = "net start \"UMP Service 01\"";

                File.WriteAllLines(LStrBatchFile, LStrArrayWriteLine);
                
                App.ExecuteBatchCommand(LStrBatchFile);
                File.Delete(LStrBatchFile);
            }
            catch (Exception ex)
            {
                LBoolReturn = false;
                AStrReturn = "004";
                App.GStrCatchException = "ReStartUMPServices()\n" + ex.ToString();
            }
            return LBoolReturn;
        }

        private void BeginInstallCertificate()
        {
            string LStrAccount = string.Empty;
            string LStrPassword = string.Empty;
            List<string> LListStrParameter = new List<string>();

            try
            {
                if (IBoolInDoing) { return; }

                if (!VerfiyInputParameteres()) { return; }

                LStrAccount = TextBoxAccount.Text;
                LStrPassword = PasswordBoxPassword.Password;

                LListStrParameter.Add(LStrAccount);
                LListStrParameter.Add(LStrPassword);

                IBoolInDoing = true;
                App.ShowCurrentStatus(1, App.GetDisplayCharater("M01044"));

                IBackgroundWorkerApplyInstalling = new BackgroundWorker();
                IBackgroundWorkerApplyInstalling.WorkerReportsProgress = true;
                IBackgroundWorkerApplyInstalling.ProgressChanged += IBackgroundWorkerApplyInstalling_ProgressChanged;
                IBackgroundWorkerApplyInstalling.RunWorkerCompleted += IBackgroundWorkerApplyInstalling_RunWorkerCompleted;
                IBackgroundWorkerApplyInstalling.DoWork += IBackgroundWorkerApplyInstalling_DoWork;
                IBackgroundWorkerApplyInstalling.RunWorkerAsync(LListStrParameter);
            }
            catch(Exception ex)
            {
                IBoolInDoing = false;
                if (IBackgroundWorkerApplyInstalling != null)
                {
                    IBackgroundWorkerApplyInstalling.Dispose();
                    IBackgroundWorkerApplyInstalling = null;
                }
                App.ShowExceptionMessage("BeginInstallCertificate()\n" + ex.ToString());
            }
        }

        private void IBackgroundWorkerApplyInstalling_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            App.ShowCurrentStatus(1, App.GetDisplayCharater("P02" + e.ProgressPercentage.ToString("000")));
        }

        private void IBackgroundWorkerApplyInstalling_DoWork(object sender, DoWorkEventArgs e)
        {
            List<string> LListStrParameters = e.Argument as List<string>;

            IBoolApplyReturn = true;

            IBackgroundWorkerApplyInstalling.ReportProgress(1);
            if (!CheckCertificateIsExist(StoreName.My))
            {
                IStrApplyReturn = InstallCertificateToStore(StoreName.My);
                if (!string.IsNullOrEmpty(IStrApplyReturn))
                {
                    IBoolApplyReturn = false;
                    App.GStrCatchException = IStrApplyReturn;
                    IStrApplyReturn = "001";
                    return;
                }
            }
            System.Threading.Thread.Sleep(1000);
            if (!IBoolApplyReturn) { return; }

            IBoolApplyReturn = true;
            IBackgroundWorkerApplyInstalling.ReportProgress(2);
            if (!CheckCertificateIsExist(StoreName.Root))
            {
                IStrApplyReturn = InstallCertificateToStore(StoreName.Root);
                if (!string.IsNullOrEmpty(IStrApplyReturn))
                {
                    IBoolApplyReturn = false;
                    App.GStrCatchException = IStrApplyReturn;
                    IStrApplyReturn = "002";
                    return;
                }
            }
            System.Threading.Thread.Sleep(1000);
            if (!IBoolApplyReturn) { return; }

            IBoolApplyReturn = true;
            IBackgroundWorkerApplyInstalling.ReportProgress(3);
            if (!CheckCertificateIsExist(StoreName.TrustedPublisher))
            {
                IStrApplyReturn = InstallCertificateToStore(StoreName.TrustedPublisher);
                if (!string.IsNullOrEmpty(IStrApplyReturn))
                {
                    IBoolApplyReturn = false;
                    App.GStrCatchException = IStrApplyReturn;
                    IStrApplyReturn = "003";
                    return;
                }
            }
            System.Threading.Thread.Sleep(1000);
            if (!IBoolApplyReturn) { return; }

            IBoolApplyReturn = true;
            IBackgroundWorkerApplyInstalling.ReportProgress(4);
            IBoolApplyReturn = RestartUMPServices(LListStrParameters, ref IStrApplyReturn);
            if (!IBoolApplyReturn) { return; }
        }

        private void IBackgroundWorkerApplyInstalling_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            IBoolInDoing = false;
            App.ShowCurrentStatus(int.MaxValue, string.Empty);
            IBackgroundWorkerApplyInstalling.Dispose();
            IBackgroundWorkerApplyInstalling = null;

            MamtOperationEventArgs LEventArgs = new MamtOperationEventArgs();
            LEventArgs.StrElementTag = "RICER";
            if (IOperationEvent != null) { IOperationEvent(this, LEventArgs); }

            if (!IBoolApplyReturn)
            {
                MessageBox.Show(App.GetDisplayCharater("E01" + IStrApplyReturn) + "\n" + App.GStrCatchException, App.GStrApplicationReferredTo, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                MessageBox.Show(App.GetDisplayCharater("M01049"), App.GStrApplicationReferredTo, MessageBoxButton.OK, MessageBoxImage.Information);
            }
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
                    if (LX509CertificateSingle.GetCertHashString().Trim() == "C3BBF9EA2C0DA7FEAA17043A0A6010A522ABAB87") { LBoolReturn = true; break; }
                }
                
                LX509Store.Close(); LX509Store = null;
            }
            catch { LBoolReturn = true; }

            return LBoolReturn;
        }

        /// <summary>
        /// 将证书安装到制定存储区域
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
                LStrCertificateFileFullName = System.IO.Path.Combine(App.GStrSiteRootFolder, @"Components\Certificates\UMP.PF.Certificate.pfx");

                //byte[] LByteReadedCertificate = System.IO.File.ReadAllBytes(LStrCertificateFileFullName);
                //X509Certificate2 LX509Certificate = new X509Certificate2(LByteReadedCertificate, "VoiceCyber,123");

                X509Certificate2 LX509Certificate = new X509Certificate2(LStrCertificateFileFullName, "VoiceCyber,123", X509KeyStorageFlags.Exportable | X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.MachineKeySet);

                X509Store LX509Store = null;
                
                LX509Store = new X509Store(AStoreName, StoreLocation.LocalMachine);
                LX509Store.Open(OpenFlags.ReadWrite);
                LX509Store.Remove(LX509Certificate);
                LX509Store.Add(LX509Certificate);
                LX509Store.Close();
            }
            catch (Exception ex)
            {
                LStrReturn = "InstallCertificateToStore()\n" + ex.ToString();
            }

            return LStrReturn;
        }

        #endregion
    }
}
