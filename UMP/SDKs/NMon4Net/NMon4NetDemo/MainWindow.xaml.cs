using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Interop;
using VoiceCyber.SDKs.NMon;

namespace NMon4NetDemo
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {

        private Dictionary<string, NMonCore> mVLNMonCores;
        private DirectXPlayer mDxPlayer;
        private NAudioPlayer mNAudioPlayer;

        private bool mUseVLNMonPlayer;
        private bool mUseDxPlayer;
        private bool mUseNAudioPlayer;

        public MainWindow()
        {
            InitializeComponent();

            mVLNMonCores = new Dictionary<string, NMonCore>();

            Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing;
            BtnTest.Click += BtnTest_Click;
            BtnStartMon.Click += BtnStartMon_Click;
            BtnStopMon.Click += BtnStopMon_Click;
            BtnClose.Click += BtnClose_Click;
        }
       

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            TxtVoiceIp.Text = "192.168.6.27";
            TxtMonitorPort.Text = "3002";
            TxtChannel0.Text = "0";
            RbMix0.IsChecked = true;
            RbMix1.IsChecked = true;
            CbConnectVoice.IsChecked = true;
            CbDecodeData.IsChecked = true;
            CbPlayWave.IsChecked = true;
            CbWriteSrcFile.IsChecked = false;
            CbWritePcmFile.IsChecked = false;
            TxtWaveDir.Text = "WaveFiles";
            RbVLNMon.IsChecked = true;
        }

        void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            StopMon();
        }

        void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        void BtnStopMon_Click(object sender, RoutedEventArgs e)
        {
            StopMon();
        }

        void BtnStartMon_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckInput())
            {
                return;
            }
            SubDebug(string.Format("Start monitor start."));
            if (TxtChannel0.Text != string.Empty)
            {
                //Create NetMonitor object,can simply create it like:
                //VLNMonCore core=new VLNMonCore()
                //if need't any particular settings
                NETMON_PARAM param = new NETMON_PARAM();
                param.Host = TxtVoiceIp.Text;
                param.Port = int.Parse(TxtMonitorPort.Text);
                param.Channel = int.Parse(TxtChannel0.Text);
                string str = "VLNMon0";
                NMonCore core = new NMonCore(str);
                core.Debug += Core_Debug;
                core.HeadReceived += Core_HeadReceived;
                core.DataReceived += Core_DataReceived;
                core.IsConnectServer = CbConnectVoice.IsChecked == true;
                //core.IsConnectServer = false;
                core.IsDecodeData = CbDecodeData.IsChecked == true;
                //core.IsDecodeData = false;
                core.IsPlayWave = CbPlayWave.IsChecked == true;
                //core.IsPlayWave = true;
                core.IsSourceWaveWriteFile = CbWriteSrcFile.IsChecked == true;
                core.IsPcmWaveWriteFile = CbWritePcmFile.IsChecked == true;
                core.WaveDirectory = TxtWaveDir.Text;
                if (RbRight0.IsChecked == true)
                {
                    core.Volume = 5;
                }
                else if (RbLeft0.IsChecked == true)
                {
                    core.Volume = 3;
                }
                else
                {
                    core.Volume = 1;
                }
                mVLNMonCores.Add(str, core);
                core.StartMon(param);
            }
            if (TxtChannel1.Text != string.Empty)
            {
                NETMON_PARAM param = new NETMON_PARAM();
                param.Host = TxtVoiceIp.Text;
                param.Port = int.Parse(TxtMonitorPort.Text);
                param.Channel = int.Parse(TxtChannel1.Text);
                string str = "VLNMon1";
                NMonCore core = new NMonCore(str);
                core.Debug += Core_Debug;
                core.HeadReceived += Core_HeadReceived;
                core.DataReceived += Core_DataReceived;
                core.IsConnectServer = CbConnectVoice.IsChecked == true;
                core.IsDecodeData = CbDecodeData.IsChecked == true;
                core.IsPlayWave = CbPlayWave.IsChecked == true;
                //core.IsPlayWave = false;
                core.IsSourceWaveWriteFile = CbWriteSrcFile.IsChecked == true;
                core.IsPcmWaveWriteFile = CbWritePcmFile.IsChecked == true;
                core.WaveDirectory = TxtWaveDir.Text;
                if (RbRight1.IsChecked == true)
                {
                    core.Volume = 5;
                }
                else if (RbLeft1.IsChecked == true)
                {
                    core.Volume = 3;
                }
                else
                {
                    core.Volume = 1;
                }
                mVLNMonCores.Add(str, core);
                core.StartMon(param);
            }
        }

        void BtnTest_Click(object sender, RoutedEventArgs e)
        {

        }

        void I_NAudioPlayer_Debug(string msg)
        {
            SubDebug(string.Format("NAudioPlayer\t{0}", msg));
        }

        void I_DXPlayer_Debug(string msg)
        {
            SubDebug(string.Format("DirectXPLayer\t{0}", msg));
        }

        void Core_Debug(object user, string msg)
        {
            SubDebug(string.Format("[VLNMon4Net]\t{0}\t{1}", user, msg));
        }

        void Core_HeadReceived(object user, SNM_RESPONSE head)
        {
            SubDebug(string.Format("[VLNMon4Net]\t{0}\tVoice head received,voice format:{1}\tchannel:{2}", user, (EVLVoiceFormat)head.format, head.channel));
            //to set audio format
            EVLVoiceFormat format = (EVLVoiceFormat)head.format;
            Dispatcher.Invoke(new Action(() =>
            {
                if (RbDiretX.IsChecked == true)
                {
                    mUseDxPlayer = true;
                    IntPtr intPtr = (new WindowInteropHelper(this)).Handle;
                    mDxPlayer = new DirectXPlayer(intPtr);
                    mDxPlayer.Debug += I_DXPlayer_Debug;
                    //I_DXPlayer.Prepare(format);
                    SubDebug(string.Format("Use DirectX player"));
                }
                else
                {
                    mUseDxPlayer = false;
                }
                if (RbVLNMon.IsChecked == true)
                {
                    mUseVLNMonPlayer = true;
                    mVLNMonCores[(string)user].IsPlayWave = true;
                    SubDebug(string.Format("Use VLNMon player"));
                }
                else
                {
                    mUseVLNMonPlayer = false;
                    mVLNMonCores[(string)user].IsPlayWave = false;
                }
                if (RbNAudio.IsChecked == true)
                {
                    mUseNAudioPlayer = true;
                    mNAudioPlayer = new NAudioPlayer();
                    mNAudioPlayer.Debug += I_NAudioPlayer_Debug;
                    mNAudioPlayer.Prepare(format);
                    SubDebug(string.Format("Use NAudio player"));
                }
                else
                {
                    mUseNAudioPlayer = false;
                }

                //if ((string)user == "VLNMon1")
                //{
                //    if (I_VLNMonCores["VLNMon0"] != null)
                //    {
                //        I_VLNMonCores["VLNMon0"].ReceiveHead(head);
                //    }
                //}
            }));
        }

        void Core_DataReceived(object user, byte[] data, int length)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                //to play voice data
                if (mUseDxPlayer)
                {
                    //I_DXPlayer.AddSamples(data, length);
                }
                if (mUseVLNMonPlayer)
                {
                    mVLNMonCores[(string) user].IsPlayWave = true;
                }
                if (mUseNAudioPlayer)
                {
                    mNAudioPlayer.AddSamples(data, length);
                }

                //if ((string)user == "VLNMon1")
                //{
                //    if (I_VLNMonCores["VLNMon0"] != null)
                //    {
                //        I_VLNMonCores["VLNMon0"].ReceiveData(data, length);
                //    }
                //}
            }));
        }

        private void StopMon()
        {
            foreach (NMonCore core in mVLNMonCores.Values)
            {
                core.StopMon();
            }
            mVLNMonCores.Clear();
            if (mDxPlayer != null)
            {
                //I_DXPlayer.Stop();
            }
            if (mNAudioPlayer != null)
            {
                mNAudioPlayer.Stop();
            }
            SubDebug(string.Format("Monitor stopped."));
        }

        private bool CheckInput()
        {
            if (TxtVoiceIp.Text == string.Empty)
            {
                SubDebug(string.Format("Voice ip is empty."));
                return false;
            }
            int iIntValue;
            if (!int.TryParse(TxtMonitorPort.Text, out iIntValue))
            {
                SubDebug(string.Format("Monitor port invalid."));
                return false;
            }
            if (TxtChannel0.Text == string.Empty && TxtChannel1.Text == string.Empty)
            {
                SubDebug(string.Format("Channel empty."));
                return false;
            }
            if (TxtChannel0.Text != string.Empty && !int.TryParse(TxtChannel0.Text, out iIntValue))
            {
                SubDebug(string.Format("Channel0 invalid."));
                return false;
            }
            if (TxtChannel1.Text != string.Empty && !int.TryParse(TxtChannel1.Text, out iIntValue))
            {
                SubDebug(string.Format("Channel1 invalid."));
                return false;
            }
            if (CbWriteSrcFile.IsChecked == true || CbWritePcmFile.IsChecked == true)
            {
                if (TxtWaveDir.Text == string.Empty)
                {
                    SubDebug(string.Format("Save diretory is empty."));
                    return false;
                }
            }
            return true;
        }

        private void SubDebug(string msg)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                TbMsg.Text = string.Format("{0}\t{1}\r\n{2}", DateTime.Now.ToString("HH:mm:ss"), msg, TbMsg.Text);
            }));
        }
    }
}
