using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using UMP.PF.MAMT.Classes;

namespace UMP.PF.MAMT
{
    public partial class SplashWindows : Window
    {
        private BackgroundWorker InstanceBackgroundWorkerGetBasicInformation = null;

        public SplashWindows()
        {
            InitializeComponent();
            this.Loaded += SplashWindows_Loaded;
        }

        private void SplashWindows_Loaded(object sender, RoutedEventArgs e)
        {
            App.DrawWindowsBackGround(this, @"Images\00000000.png");
            this.Icon = new BitmapImage(new Uri(System.IO.Path.Combine(App.GStrApplicationDirectory, @"Images\00000001.ico"), UriKind.RelativeOrAbsolute));
            WaitPorgressBarLoading.StartAnimation();
            ReadComputerBasicInformation();
        }

        private void ReadComputerBasicInformation()
        {
            InstanceBackgroundWorkerGetBasicInformation = new BackgroundWorker();
            InstanceBackgroundWorkerGetBasicInformation.RunWorkerCompleted += InstanceBackgroundWorkerGetBasicInformation_RunWorkerCompleted;
            InstanceBackgroundWorkerGetBasicInformation.DoWork += InstanceBackgroundWorkerGetBasicInformation_DoWork;
            InstanceBackgroundWorkerGetBasicInformation.RunWorkerAsync();
        }

        private void InstanceBackgroundWorkerGetBasicInformation_DoWork(object sender, DoWorkEventArgs e)
        {
            App.IComputerInfo = AboutComputer.GetOSInformation();
            App.IIISInfo = AboutComputer.GetIISInformatation();
            System.Threading.Thread.Sleep(3000);
        }

        private void InstanceBackgroundWorkerGetBasicInformation_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ConfigurationTypeSelect LConfigurationTypeSelect = new ConfigurationTypeSelect();
            LConfigurationTypeSelect.Show();
            this.Close();
        }
    }
}
