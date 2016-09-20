using System;
using System.Collections.Generic;
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
using UMPServicePackCommon;
using VoiceCyber.Common;
using UMPServicePack.PublicClasses;
using VoiceCyber.UMP.Common;
using System.ComponentModel;
using System.Threading;

namespace UMPServicePack
{
    /// <summary>
    /// Window1.xaml 的交互逻辑
    /// </summary>
    public partial class Window1 : Window
    {
        BackgroundWorker mBackgroundWorker = null;
        private ManualResetEvent manualReset = new ManualResetEvent(true);
        int iChooseResult = -1;

        public Window1()
        {
            InitializeComponent();
            Loaded += Window1_Loaded;
        }

        void Window1_Loaded(object sender, RoutedEventArgs e)
        {
            btnStart.Click += btnStart_Click;
            btnStop.Click += btnStop_Click;
            btnRetry.Click += btnRetry_Click;
            btnExit.Click += btnExit_Click;
            btnOpenDirectory.Click += btnOpenDirectory_Click;
            mBackgroundWorker = new BackgroundWorker();
            mBackgroundWorker.DoWork += mBackgroundWorker_DoWork;
            mBackgroundWorker.RunWorkerCompleted += mBackgroundWorker_RunWorkerCompleted;
            mBackgroundWorker.RunWorkerAsync();
        }

        void btnOpenDirectory_Click(object sender, RoutedEventArgs e)
        {
            CommonFuncs.CmdOperator("explorer.exe "+@"C:\Program Files\VoiceCyber\UMP\MAMT\DBObjects\8.03.001\21-F");
        }

        void btnExit_Click(object sender, RoutedEventArgs e)
        {
            iChooseResult = 1;
            manualReset.Set();
        }

        /// <summary>
        /// 重试
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btnRetry_Click(object sender, RoutedEventArgs e)
        {
            iChooseResult = 0;
            manualReset.Set();
        }

        void btnStop_Click(object sender, RoutedEventArgs e)
        {
            manualReset.Reset();
        }

        void btnStart_Click(object sender, RoutedEventArgs e)
        {
            manualReset.Set();
        }

        void mBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            MessageBox.Show("完成");
        }

        void mBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            
            for (int i = 0; i < 100; i++)
            {
                if (i == 2)
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        pop.IsOpen = true;
                    }));
                    manualReset.Reset();
                }
                manualReset.WaitOne();
                Thread.Sleep(1000);
                Dispatcher.Invoke(new Action(() =>
                    {
                        txt.Text += i + "  ";
                    }));
                if (iChooseResult == 1)
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        txt.Text += " 结束 ";
                    }));
                }
                else if(iChooseResult==0)
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        txt.Text += " 继续 ";
                    }));
                }
            }

        }
    }
}
