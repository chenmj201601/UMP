using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace UMP.PF.Client
{
    public partial class Window01 : Window
    {
        private BackgroundWorker InstanceBackgroundWorkerGetBasicInformation = null;

        private bool IBoolCanClose = false;

        public Window01()
        {
            InitializeComponent();
            this.Loaded += Window01_Loaded;
            this.Closing += Window01_Closing;
            this.MouseLeftButtonDown += Window01_MouseLeftButtonDown;
        }

        private void Window01_Closing(object sender, CancelEventArgs e)
        {
            if (!IBoolCanClose) { e.Cancel = true; }
        }

        private void Window01_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Window01_Loaded(object sender, RoutedEventArgs e)
        {
            WaitPorgressBarLoading.StartAnimation();
            App.InitializeLanguagePackage();
            ReadBasicInformation();
        }

        private void ReadBasicInformation()
        {
            IBoolCanClose = false;
            InstanceBackgroundWorkerGetBasicInformation = new BackgroundWorker();
            InstanceBackgroundWorkerGetBasicInformation.WorkerReportsProgress = true;
            InstanceBackgroundWorkerGetBasicInformation.RunWorkerCompleted += InstanceBackgroundWorkerGetBasicInformation_RunWorkerCompleted;
            InstanceBackgroundWorkerGetBasicInformation.DoWork += InstanceBackgroundWorkerGetBasicInformation_DoWork;
            InstanceBackgroundWorkerGetBasicInformation.ProgressChanged += InstanceBackgroundWorkerGetBasicInformation_ProgressChanged;
            InstanceBackgroundWorkerGetBasicInformation.RunWorkerAsync();
        }

        private void InstanceBackgroundWorkerGetBasicInformation_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            int LIntCurrentStep = e.ProgressPercentage;
            LabelCurrentStep.Content = App.GetDisplayCharater("Window01", "Get" + LIntCurrentStep.ToString("00"));
        }

        private void InstanceBackgroundWorkerGetBasicInformation_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker LBackgroundWorker = sender as BackgroundWorker;
            LBackgroundWorker.ReportProgress(1);
            GetSystemRoot();
            System.Threading.Thread.Sleep(1000);

            LBackgroundWorker.ReportProgress(2);
            GetOSVersionAndType();
            System.Threading.Thread.Sleep(1000);

            LBackgroundWorker.ReportProgress(3);
            GetAppServerHostAndPort();
            System.Threading.Thread.Sleep(1000);
        }

        private void InstanceBackgroundWorkerGetBasicInformation_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            LabelCurrentStep.Content = string.Empty;
            IBoolCanClose = true;
            if (InstanceBackgroundWorkerGetBasicInformation != null)
            {
                InstanceBackgroundWorkerGetBasicInformation.Dispose(); InstanceBackgroundWorkerGetBasicInformation = null;
            }

            Window02 LWindow02 = new Window02();
            this.Close();
            LWindow02.ShowDialog();
        }

        /// <summary>
        /// 获取系统System路径
        /// </summary>
        private void GetSystemRoot()
        {
            App.GStrSystemRoot = System.Environment.SystemDirectory;
        }

        /// <summary>
        /// 获取windows 版本号 和 windows 的安装类型
        /// </summary>
        private void GetOSVersionAndType()
        {
            double LDoubleVersion = 0.0;
            App.GStrWindowVersion = (string)Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion").GetValue("CurrentVersion");
            App.GStrWindowType = "NONE";
            LDoubleVersion = Double.Parse(App.GStrWindowVersion);
            if (LDoubleVersion > 6.0)
            {
                App.GStrWindowType = ((string)Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion").GetValue("InstallationType")).ToUpper();
            }

            if (Environment.Is64BitOperatingSystem) { App.GBoolIs64BitOperatingSystem = true; }
            else { App.GBoolIs64BitOperatingSystem = false; }
        }

        /// <summary>
        /// 获取IE浏览器打开UMP.PF.html记录
        /// </summary>
        private void GetAppServerHostAndPort()
        {
            string LStrReadedUrl = string.Empty;
            int LIntProtoIndex, LIntLastIndex, LIntPortIndex;
            string LStrServerInfo = string.Empty;

            App.GListStrAppServerName.Clear();
            App.GListIntAppServerPort.Clear();

            

            int LIntLoop, LIntEnd = 20;
            for (LIntLoop = 1; LIntLoop <= LIntEnd; LIntLoop++)
            {
                try
                {
                    LStrReadedUrl = ((string)Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Internet Explorer\\TypedURLs").GetValue("url" + LIntLoop.ToString()));
                    if (string.IsNullOrEmpty(LStrReadedUrl)) { break; }
                    if (!LStrReadedUrl.Contains("UMP.PF.html")) { continue; }
                    LIntProtoIndex = LStrReadedUrl.IndexOf("://");
                    if (LIntProtoIndex <= 0) { continue; }
                    LIntLastIndex = LStrReadedUrl.IndexOf("/UMP.PF.html");
                    if (LIntProtoIndex <= 0) { continue; }
                    LStrServerInfo = LStrReadedUrl.Substring(LIntProtoIndex + 3, LIntLastIndex - LIntProtoIndex - 3);
                    if (LStrServerInfo.Equals(App.GStrLastSettedUMPServerHost + ":" + App.GIntLastSettedUMPServerPort.ToString())) { continue; }
                    LIntPortIndex = LStrServerInfo.IndexOf(":");
                    if (LIntPortIndex > 0)
                    {
                        string[] LStrServer = LStrServerInfo.Split(":".ToCharArray());
                        App.GListStrAppServerName.Add(LStrServer[0]);
                        App.GListIntAppServerPort.Add(int.Parse(LStrServer[1]));
                    }
                    else
                    {
                        App.GListStrAppServerName.Add(LStrServerInfo);
                        App.GListIntAppServerPort.Add(80);
                    }
                }
                catch { break; }
            }

            if (!string.IsNullOrEmpty(App.GStrLastSettedUMPServerHost))
            {
                App.GListStrAppServerName.Add(App.GStrLastSettedUMPServerHost);
                App.GListIntAppServerPort.Add(App.GIntLastSettedUMPServerPort);
            }
        }
    }
}
