//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    e6ac1765-cba1-4cf8-9afa-43cd7c209987
//        CLR Version:              4.0.30319.34003
//        Name:                     AudioPlayer
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.NAudio.Controls
//        File Name:                AudioPlayer
//
//        created by Charley at 2013/12/1 12:07:16
//        http://www.voicecyber.com
//
//======================================================================

using System;
using System.Drawing;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using VoiceCyber.NAudio.Wave;
using VoiceCyber.NAudio.Wave.SampleProviders;

namespace VoiceCyber.NAudio.Controls
{
    /// <summary>
    /// AudioPlayer.xaml 的交互逻辑
    /// </summary>
    public partial class AudioPlayer
    {
        /// <summary>
        /// 音频播放完并停止
        /// </summary>
        public event Action PlaybackStopped;
        /// <summary>
        /// 用户点击了关闭按钮
        /// </summary>
        public event Action Closing;
        /// <summary>
        /// 播放器事件
        /// Code    参考Defines中的事件代码定义
        /// Value   事件的值，不同事件类型的值的类型不同
        /// 例如：
        /// EVT_BTN_STOP    返回值是TimeSpan类型，表示已经播放的时长
        /// EVT_BTN_PLAY    返回值是String类型，表示当前播放的音频url
        /// EVT_EXCEPTION   返回值为OptReturn类型，表示异常结果
        /// </summary>
        public event Action<int, object> PlayerEvent;

        private WaveOut mWaveOut;
        private WaveStream mWaveStream;
        private ISampleProvider mSampleProvider;
        private Timer mTimer;
        private WaveOutChannelMode mChannelMode;
        private double mRate;
        private float mVolumn;
        private string mState;
        private string mUrl;

        #region Properties

        /// <summary>
        /// 显示状态，11二进制字符串
        /// 0：左声道波形图
        /// 1：右声道波形图
        /// 2：打开按钮
        /// 3：播放按钮
        /// 4：暂停按钮
        /// 5：停止按钮
        /// 6：减速按钮
        /// 7：加速按钮
        /// 8：音量调节
        /// 9：声道控制
        /// 10：关闭按钮
        /// </summary>
        public string State
        {
            get { return mState; }
            set { mState = value; }
        }
        /// <summary>
        /// 音频Url
        /// </summary>
        public string Url
        {
            get { return mUrl; }
            set { mUrl = value; }
        }

        #endregion

        /// <summary>
        /// 实现简易播放器，功能：播放控制，快进快退，音量控制，声道控制以及波形图显示
        /// </summary>
        public AudioPlayer()
        {
            InitializeComponent();

            mTimer = new Timer();
            mTimer.Interval = 100;
            mTimer.Elapsed += mTimer_Elapsed;
            mRate = 1.0;
            mVolumn = 1;
            mChannelMode = WaveOutChannelMode.Default;
            mState = "11111111111";
        }
        /// <summary>
        /// 开始播放音频
        /// </summary>
        public void Play()
        {
            OptReturn optReturn;
            if (string.IsNullOrEmpty(mUrl))
            {
                optReturn = new OptReturn();
                optReturn.Code = Defines.RET_STRING_EMPTY;
                optReturn.Message = string.Format(("Url is empty"));
                ShowException(optReturn);
                return;
            }
            try
            {
                WaveStream reader;
                if (mUrl.StartsWith("http", StringComparison.InvariantCultureIgnoreCase))
                {

                    if (mUrl.EndsWith(".mp3", StringComparison.InvariantCultureIgnoreCase))
                    {
                        reader = new Mp3NetworkStream(mUrl);
                    }
                    else
                    {
                        reader = new NetWorkWaveReader(mUrl);
                        if (reader.WaveFormat.Encoding != WaveFormatEncoding.Pcm &&
                           reader.WaveFormat.Encoding != WaveFormatEncoding.IeeeFloat)
                        {
                            reader = WaveFormatConversionStream.CreatePcmStream(reader);
                            reader = new BlockAlignReductionStream(reader);
                        }
                    }
                }
                else
                {
                    if (mUrl.EndsWith(".mp3", StringComparison.InvariantCultureIgnoreCase))
                    {
                        reader = new Mp3FileReader(mUrl);
                    }
                    else
                    {
                        reader = new WaveFileReader(mUrl);
                        if (reader.WaveFormat.Encoding != WaveFormatEncoding.Pcm &&
                            reader.WaveFormat.Encoding != WaveFormatEncoding.IeeeFloat)
                        {
                            reader = WaveFormatConversionStream.CreatePcmStream(reader);
                            reader = new BlockAlignReductionStream(reader);
                        }
                    }
                }
                if (mWaveStream != null) { mWaveStream.Dispose(); }
                mWaveStream = reader;
                SubPlayerEvent(Defines.EVT_MEDIAOPENED, mWaveStream.TotalTime);

                if (!CreateWaveImage(mWaveStream)) { return; }

                LbTotalTime.Content = GetTimeString(mWaveStream.TotalTime);
                LbCurrentTime.Content = "00:00:00";
                SliderPosition.Maximum = mWaveStream.TotalTime.TotalSeconds;

                if (!CreateWaveStream()) { return; }
                if (!CreateWaveOut()) { return; }
                if (mWaveOut != null)
                {
                    mWaveOut.Play();
                    mTimer.Start();

                    SubPlayerEvent(Defines.EVT_PLAYBACKSTARTED, mUrl);
                }
            }
            catch (Exception ex)
            {
                optReturn = new OptReturn();
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
                ShowException(optReturn);
            }
        }

        /// <summary>
        /// 关闭播放器，释放资源
        /// </summary>
        public void Close()
        {
            if (mTimer != null)
            {
                mTimer.Stop();
            }
            if (mWaveOut != null)
            {
                if (mWaveStream != null)
                {
                    mWaveStream.Position = 0;
                    LbCurrentTime.Content = "00:00:00";
                    SliderPosition.Value = 0;
                }
                mWaveOut.Dispose();
                mWaveOut = null;
            }
            if (mWaveStream != null)
            {
                mWaveStream.Dispose();
                mWaveStream = null;
            }
        }

        /// <summary>
        /// 设定播放位置
        /// </summary>
        /// <param name="ts"></param>
        public void SetPosition(TimeSpan ts)
        {
            OptReturn optReturn;
            try
            {
                if (mWaveStream != null)
                {
                    mWaveStream.CurrentTime = ts;
                }
            }
            catch (Exception ex)
            {
                optReturn = new OptReturn();
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
                ShowException(optReturn);
            }
        }

        public TimeSpan GetCurrentPosition()
        {
            OptReturn optReturn;
            try
            {
                if (mWaveStream != null)
                {
                    TimeSpan ts = mWaveStream.CurrentTime;
                    return ts;
                }
            }
            catch (Exception ex)
            {
                optReturn = new OptReturn();
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
                ShowException(optReturn);
            }
            return TimeSpan.FromSeconds(0);
        }


        #region Basic

        private void ShowState()
        {
            if (mState.Length > 0)
            {
                ImgLeft.Visibility = mState.Substring(0, 1) == "0" ? Visibility.Collapsed : Visibility.Visible;
            }
            if (mState.Length > 1)
            {
                ImgRight.Visibility = mState.Substring(1, 1) == "0" ? Visibility.Collapsed : Visibility.Visible;
            }
            if (mState.Length > 2)
            {
                BtnOpen.Visibility = mState.Substring(2, 1) == "0" ? Visibility.Collapsed : Visibility.Visible;
            }
            if (mState.Length > 3)
            {
                BtnPlay.Visibility = mState.Substring(3, 1) == "0" ? Visibility.Collapsed : Visibility.Visible;
            }
            if (mState.Length > 4)
            {
                BtnPause.Visibility = mState.Substring(4, 1) == "0" ? Visibility.Collapsed : Visibility.Visible;
            }
            if (mState.Length > 5)
            {
                BtnStop.Visibility = mState.Substring(5, 1) == "0" ? Visibility.Collapsed : Visibility.Visible;
            }
            if (mState.Length > 6)
            {
                BtnSlower.Visibility = mState.Substring(6, 1) == "0" ? Visibility.Collapsed : Visibility.Visible;
            }
            if (mState.Length > 7)
            {
                BtnFaster.Visibility = mState.Substring(7, 1) == "0" ? Visibility.Collapsed : Visibility.Visible;
            }
            if (mState.Length > 8)
            {
                SliderVolume.Visibility = mState.Substring(8, 1) == "0" ? Visibility.Collapsed : Visibility.Visible;
            }
            if (mState.Length > 9)
            {
                BtnChannelMode.Visibility = mState.Substring(9, 1) == "0" ? Visibility.Collapsed : Visibility.Visible;
            }
            if (mState.Length > 10)
            {
                BtnClose.Visibility = mState.Substring(10, 1) == "0" ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        private bool CreateWaveImage(WaveStream waveStream)
        {
            OptReturn optReturn;
            if (waveStream == null)
            {
                return false;
            }
            try
            {
                WavePainter wavePainter = new WavePainter();
                wavePainter.VolumnNum = 2;
                wavePainter.IsSaveFile = false;
                optReturn = wavePainter.Draw(waveStream);
                if (!optReturn.Result)
                {
                    ShowException(optReturn);
                    return false;
                }
                var data = optReturn.Data as Bitmap[];
                if (data != null)
                {
                    var bitmap = data[0];
                    var imgsource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                    ImgLeft.Source = imgsource;
                    bitmap = data[1];
                    imgsource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                    ImgRight.Source = imgsource;
                }
                return true;
            }
            catch (Exception ex)
            {
                optReturn = new OptReturn();
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
                ShowException(optReturn);
                return false;
            }
        }

        private bool CreateWaveStream()
        {
            OptReturn optReturn;
            if (mWaveStream == null)
            {
                return false;
            }
            try
            {
                WaveStream waveStream = new CustomRateWaveStream(mWaveStream, mRate);
                var waveChannel = new SampleChannel(waveStream, true);
                waveChannel.PreVolumeMeter += waveChannel_PreVolumeMeter;
                var postVolumeMeter = new MeteringSampleProvider(waveChannel);
                postVolumeMeter.StreamVolume += postVolumeMeter_StreamVolume;
                mSampleProvider = postVolumeMeter;
                return true;
            }
            catch (Exception ex)
            {
                optReturn = new OptReturn();
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
                ShowException(optReturn);
                return false;
            }
        }

        private bool CreateWaveOut()
        {
            OptReturn optReturn;
            if (mSampleProvider == null)
            {
                return false;
            }
            if (mWaveStream == null)
            {
                return false;
            }
            if (mWaveOut != null)
            {
                try
                {
                    mWaveOut.Stop();
                    mWaveOut.Dispose();
                    mWaveOut = null;
                }
                catch { }
            }
            try
            {
                mWaveOut = new WaveOut();
                mWaveOut.PlaybackStopped += mWaveOut_PlaybackStopped;
                mWaveOut.Init(new SampleToWaveProvider(mSampleProvider));
                mWaveOut.ChannelMode = mChannelMode;
                return true;
            }
            catch (Exception ex)
            {
                optReturn = new OptReturn();
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
                ShowException(optReturn);
                return false;
            }
        }

        private void ShowException(OptReturn optReturn)
        {
            Dispatcher.Invoke(new Action(() => SubPlayerEvent(Defines.EVT_EXCEPTION, optReturn)));
        }

        private void SubPlaybackStopped()
        {
            if (PlaybackStopped != null)
            {
                PlaybackStopped();
            }
        }

        private void SubClosing()
        {
            if (Closing != null)
            {
                Closing();
            }
        }

        private void SubPlayerEvent(int code, object obj)
        {
            if (PlayerEvent != null)
            {
                PlayerEvent(code, obj);
            }
        }

        private string GetTimeString(TimeSpan ts)
        {
            string strReturn;

            int intHour, intMinute, intSecond;
            intHour = ts.Hours;
            intMinute = ts.Minutes;
            intSecond = ts.Seconds;
            strReturn = string.Format("{0}:{1}:{2}", intHour.ToString("00"), intMinute.ToString("00"),
               intSecond.ToString("00"));

            return strReturn;
        }

        #endregion


        #region Window Events

        private void AudioPlayer_OnLoaded(object sender, RoutedEventArgs e)
        {
            LbRate.Content = mRate.ToString("0.0");
            LbVolume.Content = mVolumn.ToString("0.0");
            ShowState();
            SubPlayerEvent(Defines.EVT_UC_LOADED, null);
        }

        private void AudioPlayer_OnUnloaded(object sender, RoutedEventArgs e)
        {
            SubClosing();
            SubPlayerEvent(Defines.EVT_UC_UNLOADED, null);
            Close();
        }

        #endregion


        #region Button Events

        private void BtnOpen_OnClick(object sender, RoutedEventArgs e)
        {
            OptReturn optReturn;
            OpenFileDialog opendDialog = new OpenFileDialog();
            opendDialog.Filter = "Wave File(*.wav)|*.wav|MP3 File(*.mp3)|*.mp3|All File(*.*)|*.*";
            var result = opendDialog.ShowDialog();
            if (result == true)
            {
                string filePath = opendDialog.FileName;
                if (!System.IO.File.Exists(filePath))
                {
                    optReturn = new OptReturn();
                    optReturn.Code = Defines.RET_FILE_NOT_EXIST;
                    optReturn.Message = filePath;
                    ShowException(optReturn);
                    return;
                }
                mUrl = filePath;
                Play();
            }
        }

        private void BtnPlay_OnClick(object sender, RoutedEventArgs e)
        {
            if (mWaveOut != null
                && mWaveStream != null
                && mWaveOut.PlaybackState == PlaybackState.Paused)
            {
                mWaveOut.Play();
                mTimer.Start();
            }
            else if (mWaveOut != null && mWaveOut.PlaybackState == PlaybackState.Playing)
            {

            }
            else
            {
                if (mWaveStream != null)
                {
                    //if (!CreateWaveStream() || !CreateWaveOut())
                    //{
                    //    return;
                    //}
                    if (mWaveOut != null)
                    {
                        mWaveOut.Play();
                        mTimer.Start();
                    }
                }
            }
            SubPlayerEvent(Defines.EVT_BTN_PLAY, mUrl);
        }

        private void BtnPause_OnClick(object sender, RoutedEventArgs e)
        {
            if (mWaveOut != null
                && mWaveStream != null
                && mWaveOut.PlaybackState == PlaybackState.Playing)
            {
                mWaveOut.Pause();
                mTimer.Stop();
            }
        }

        private void BtnStop_OnClick(object sender, RoutedEventArgs e)
        {
            if (mWaveOut != null
               && mWaveStream != null)
            {
                TimeSpan ts = mWaveStream.CurrentTime;
                mWaveStream.Position = 0;
                mWaveOut.Stop();
                mTimer.Stop();
                LbCurrentTime.Content = "00:00:00";
                SubPlayerEvent(Defines.EVT_BTN_STOP, ts);
            }
        }

        private void BtnSlower_OnClick(object sender, RoutedEventArgs e)
        {
            if (mRate > 0.1)
            {
                mRate = mRate - 0.1;
            }
            if (!CreateWaveStream() || !CreateWaveOut())
            {
                return;
            }
            if (mWaveOut != null)
            {
                mWaveOut.Play();
                mTimer.Start();
            }
            LbRate.Content = mRate.ToString("0.0");
        }

        private void BtnFaster_OnClick(object sender, RoutedEventArgs e)
        {
            if (mRate < 5.0)
            {
                mRate = mRate + 0.1;
            }
            if (!CreateWaveStream() || !CreateWaveOut())
            {
                return;
            }
            if (mWaveOut != null)
            {
                mWaveOut.Play();
                mTimer.Start();
            }
            LbRate.Content = mRate.ToString("0.0");
        }

        private void BtnChannelMode_OnClick(object sender, RoutedEventArgs e)
        {
            if (mChannelMode == WaveOutChannelMode.Default)
            {
                mChannelMode = WaveOutChannelMode.Left;
                BtnChannelMode.Content = "L";
                ImgRight.Opacity = 0.5;
                ImgLeft.Opacity = 1;
            }
            else if (mChannelMode == WaveOutChannelMode.Left)
            {
                mChannelMode = WaveOutChannelMode.Right;
                BtnChannelMode.Content = "R";
                ImgLeft.Opacity = 0.5;
                ImgRight.Opacity = 1;
            }
            else
            {
                mChannelMode = WaveOutChannelMode.Default;
                BtnChannelMode.Content = "D";
                ImgLeft.Opacity = 1;
                ImgRight.Opacity = 1;
            }
            if (mWaveOut != null)
            {
                mWaveOut.ChannelMode = mChannelMode;
            }
        }

        private void BtnClose_OnClick(object sender, RoutedEventArgs e)
        {
            SubClosing();
            SubPlayerEvent(Defines.EVT_BTN_CLOSE, null);
            Close();
        }

        #endregion


        #region Slider Events

        private void SliderVolume_OnLostMouseCapture(object sender, MouseEventArgs e)
        {
            double value = SliderVolume.Value;
            mVolumn = (float)Convert.ToDouble((value / 100).ToString("0.00"));
            if (mWaveOut != null)
            {
                mWaveOut.Volume = mVolumn;
            }
            LbVolume.Content = mVolumn.ToString("0.0");
        }

        private void SliderVolume_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            double value = SliderVolume.Value;
            mVolumn = (float)Convert.ToDouble((value / 100).ToString("0.00"));
            if (mWaveOut != null)
            {
                mWaveOut.Volume = mVolumn;
            }
            LbVolume.Content = mVolumn.ToString("0.0");
        }

        private void SliderPosition_OnLostMouseCapture(object sender, MouseEventArgs e)
        {
            if (mWaveStream != null)
            {
                mWaveStream.CurrentTime = TimeSpan.FromSeconds((int)SliderPosition.Value);
            }
        }

        private void SliderPosition_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (mWaveStream != null)
            {
                mWaveStream.CurrentTime = TimeSpan.FromSeconds((int)SliderPosition.Value);
            }
        }

        #endregion


        #region Other Events

        private void mTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (mWaveStream != null)
            {
                TimeSpan ts = mWaveStream.CurrentTime;
                Dispatcher.Invoke(new Action(() =>
                {
                    LbCurrentTime.Content = GetTimeString(ts);
                    if (mWaveStream != null)
                    {
                        SliderPosition.Value = mWaveStream.CurrentTime.TotalSeconds;
                        SubPlayerEvent(Defines.EVT_PLAYING, ts);
                    }
                }));
            }
        }

        void mWaveOut_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            if (mWaveStream != null)
            {
                mWaveStream.Position = 0;
                SliderPosition.Value = 0;
                mWaveOut.Stop();
                mTimer.Stop();
            }
            SubPlaybackStopped();
            SubPlayerEvent(Defines.EVT_PLAYBACKSTOPPED, null);
        }

        void postVolumeMeter_StreamVolume(object sender, StreamVolumeEventArgs e)
        {

        }

        void waveChannel_PreVolumeMeter(object sender, StreamVolumeEventArgs e)
        {

        }

        #endregion
    }
}
