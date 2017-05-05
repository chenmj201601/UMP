using System;
using System.Windows;
using Microsoft.Win32;
using VoiceCyber.NAudio;
using VoiceCyber.NAudio.Controls;
using VoiceCyber.NAudio.Wave;

namespace VCNAudioDemo
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {
       
        public MainWindow()
        {
            InitializeComponent();

            Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing;
            BtnTest.Click += BtnTest_Click;
            BtnPlay.Click += BtnPlay_Click;
            BtnBrowse.Click += BtnBrowse_Click;
            MyPlayer.PlayerEvent += MyPlayer_PlayerEvent;
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {

        }

        void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MyPlayer.Close();
            DefaultPlayer.Close();
        }

        void BtnBrowse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "Wave Files(*.wav)|*.wav|Mp3 Files(*.mp3)|*.mp3|All Files(*.*)|*.*";
            var result = openFile.ShowDialog();
            if (result == true)
            {
                TxtUrl.Text = openFile.FileName;
            }
        }

        void BtnPlay_Click(object sender, RoutedEventArgs e)
        {
            string url = TxtUrl.Text;
            try
            {
                if (string.IsNullOrEmpty(url))
                {
                    ShowMessage(string.Format("Url empty"), 0);
                    return;
                }
                MyPlayer.Url = url;
                MyPlayer.Play();

                //DefaultPlayer.Url = url;
                //DefaultPlayer.Play();
            }
            catch (Exception ex)
            {
                ShowMessage(string.Format("Play fail.\r\n{0}", ex.Message), 0);
            }
        }

        void BtnTest_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //var count = WaveOut.DeviceCount;
                //ShowMessage(string.Format("Count:{0}", count), 1);
                //for (int i = 0; i < count; i++)
                //{
                //    var device = WaveOut.GetCapabilities(i);
                //    ShowMessage(string.Format("Name:{0}", device.ProductName), 1);
                //}

                string strFile = TxtUrl.Text;
                WaveStream reader = new WaveFileReader(strFile);
                if (reader.WaveFormat.Encoding != WaveFormatEncoding.Pcm &&
                           reader.WaveFormat.Encoding != WaveFormatEncoding.IeeeFloat)
                {
                    reader = WaveFormatConversionStream.CreatePcmStream(reader);
                    reader = new BlockAlignReductionStream(reader);
                }
                WaveOut waveOut = new WaveOut();
                waveOut.Init(reader);
                waveOut.Play();
            }
            catch (Exception ex)
            {
                ShowMessage(ex.Message, 0);
            }
        }

        void MyPlayer_PlayerEvent(object sender, RoutedPropertyChangedEventArgs<PlayerEventArgs> e)
        {
            if (e.NewValue != null)
            {
                int code = e.NewValue.Code;
                var data = e.NewValue.Data;
                switch (code)
                {
                    case Defines.EVT_UC_LOADED:
                      
                        break;
                    case Defines.EVT_EXCEPTION:
                        ShowMessage(data.ToString(), 0);
                        break;
                }
            }
        }

        private void ShowMessage(string msg, int type)
        {
            switch (type)
            {
                case 0:
                    MessageBox.Show(msg, "DemoPlayer", MessageBoxButton.OK, MessageBoxImage.Error);
                    break;
                case 1:
                    MessageBox.Show(msg, "DemoPlayer", MessageBoxButton.OK, MessageBoxImage.Information);
                    break;
            }
        }
    }
}
