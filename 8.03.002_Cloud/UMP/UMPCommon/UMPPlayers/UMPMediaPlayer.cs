//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    5fa030fe-afbc-4c91-ae81-20f60d9f550c
//        CLR Version:              4.0.30319.18408
//        Name:                     UMPMediaPlayer
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Controls.Players
//        File Name:                UMPMediaPlayer
//
//        created by Charley at 2016/7/28 10:53:00
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using VoiceCyber.Common;
using VoiceCyber.NAudio;
using VoiceCyber.NAudio.Wave;
using VoiceCyber.NAudio.Wave.SampleProviders;
using VoiceCyber.SDKs.ScreenMP;
using VoiceCyber.SDKs.Windows;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.CommonService03;
using VoiceCyber.UMP.Communications;
using Defines = VoiceCyber.Common.Defines;
using Image = System.Windows.Controls.Image;

namespace VoiceCyber.UMP.Controls.Players
{
    /// <summary>
    /// UMP通用媒体播放器
    /// </summary>
    public class UMPMediaPlayer : Control
    {
        static UMPMediaPlayer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof (UMPMediaPlayer),
                new FrameworkPropertyMetadata(typeof (UMPMediaPlayer)));

            PlayerEventEvent = EventManager.RegisterRoutedEvent("PlayerEvent", RoutingStrategy.Bubble,
               typeof(RoutedPropertyChangedEventHandler<UMPEventArgs>), typeof(UMPMediaPlayer));
        }

        public UMPMediaPlayer()
        {
            mListLeftButtons = new ObservableCollection<ImageButtonItem>();
            mListRightButtons = new ObservableCollection<ImageButtonItem>();

            Loaded += UMPMediaPlayer_Loaded;
            Unloaded += UMPMediaPlayer_Unloaded;

            mTimer = new Timer();
            mTimer.Elapsed += mTimer_Elapsed;

            CommandBindings.Add(new CommandBinding(ButtonCommand, ButtonCommand_Executed, (s, e) => e.CanExecute = true));

            Init();
        }

        void UMPMediaPlayer_Loaded(object sender, RoutedEventArgs e)
        {
            InitLeftButtons();
            InitRightButtons();
            InitState();
            OnPlayerEventEvent(Defines.EVT_PAGE_LOADED, null);
        }

        void UMPMediaPlayer_Unloaded(object sender, RoutedEventArgs e)
        {
            Close();
            OnPlayerEventEvent(Defines.EVT_PAGE_UNLOADED, null);
        }


        #region Members

        private WaveOut mWaveOut;
        private WaveStream mWaveStream;
        private ISampleProvider mSampleProvider;
        private WaveOutChannelMode mChannelMode;
        private Timer mTimer;
        private double mRate;
        private int mMediaType;  //0：未知；1：录音；2：录屏；3：录音+录屏
        private string mAudioUrl;
        private string mVideoUrl;
        private bool mIsDraging;
        private bool mCanCloseScreenWindow;     //是否可以关闭录屏播放窗口，只有打开了录屏窗口才能调用接口关闭录屏窗口，否则容易弹出错误消息窗口

        private IntPtr mHwndPlayer;
        private double mScreenTotalLength;
        private double mScreenPlayedLength;

        private ObservableCollection<ImageButtonItem> mListLeftButtons;
        private ObservableCollection<ImageButtonItem> mListRightButtons;

        private bool mIsIsaScreen;
        private string mIsaServer;
        private string mIsaRefID;
        private bool mIsScreenPlaying;
        private NetClient mMediaClient;
        private IsaPlaybox mIsaPlaybox;
        private int mDuration;

        public SessionInfo Session;

        #endregion


        #region Init and Load

        private void Init()
        {
            try
            {
                mMediaType = 1;
                mAudioUrl = string.Empty;
                mVideoUrl = string.Empty;
                mTimer.Interval = 100;
                mRate = 1.0;
                Volume = 1.0;
                mChannelMode = WaveOutChannelMode.Default;
                State = "11111111111";
                mIsDraging = false;
                mCanCloseScreenWindow = false;
                mIsIsaScreen = false;
                mIsaServer = string.Empty;
                mIsaRefID = string.Empty;
                mIsScreenPlaying = false;
                mDuration = 5 * 60;

                mHwndPlayer = IntPtr.Zero;
                mScreenTotalLength = 0;
                mScreenPlayedLength = 0;

                Title = string.Empty;
                LbRate = mRate.ToString("0.0");
                LbVolume = Volume.ToString("0.0");
                LbCurrentTime = "00:00:00";
                LbTotalTime = "00:00:00";
                IsImageLeftVisible = true;
                IsImageRightVisible = true;
                IsVolumeVisible = true;

            }
            catch { }
        }

        private void InitLeftButtons()
        {
            mListLeftButtons.Clear();
            ImageButtonItem item;

            if (State.Length > 3 && State.Substring(3, 1) == "1")
            {
                item = new ImageButtonItem();
                item.Name = "Play";
                item.Icon = "Images/play.png";
                mListLeftButtons.Add(item);
            }
            if (State.Length > 4 && State.Substring(4, 1) == "1")
            {
                item = new ImageButtonItem();
                item.Name = "Pause";
                item.Icon = "Images/pause.png";
                mListLeftButtons.Add(item);
            }
            if (State.Length > 5 && State.Substring(5, 1) == "1")
            {
                item = new ImageButtonItem();
                item.Name = "Stop";
                item.Icon = "Images/stop.png";
                mListLeftButtons.Add(item);
            }
            if (State.Length > 6 && State.Substring(6, 1) == "1")
            {
                item = new ImageButtonItem();
                item.Name = "Slower";
                item.Icon = "Images/slower.png";
                mListLeftButtons.Add(item);
            }
            if (State.Length > 7 && State.Substring(7, 1) == "1")
            {
                item = new ImageButtonItem();
                item.Name = "Faster";
                item.Icon = "Images/faster.png";
                mListLeftButtons.Add(item);
            }
        }

        private void InitRightButtons()
        {
            mListRightButtons.Clear();
            ImageButtonItem item;
            if (State.Length > 9 && State.Substring(9, 1) == "1")
            {
                item = new ImageButtonItem();
                item.Name = "ChannelMode";
                item.Display = "D";
                mListRightButtons.Add(item);
            }
            if (State.Length > 10 && State.Substring(10, 1) == "1")
            {
                item = new ImageButtonItem();
                item.Name = "Close";
                item.Display = "X";
                mListRightButtons.Add(item);
            }
        }

        private void InitState()
        {
            if (State.Length > 0 && State.Substring(0, 1) == "1")
            {
                IsImageLeftVisible = true;
            }
            else
            {
                IsImageLeftVisible = false;
            }
            if (State.Length > 1 && State.Substring(1, 1) == "1")
            {
                IsImageRightVisible = true;
            }
            else
            {
                IsImageRightVisible = false;
            }
            if (State.Length > 8 && State.Substring(8, 1) == "1")
            {
                IsVolumeVisible = true;
            }
            else
            {
                IsVolumeVisible = false;
            }
        }

        #endregion


        #region Public Members

        /// <summary>
        /// 媒体类型
        /// 0：未知
        /// 1：录音
        /// 2：录屏
        /// 3：录音+录屏
        /// </summary>
        public int MediaType
        {
            get { return mMediaType; }
            set { mMediaType = value; }
        }

        public string AudioUrl
        {
            get { return mAudioUrl; }
            set { mAudioUrl = value; }
        }

        public string VideoUrl
        {
            get { return mVideoUrl; }
            set { mVideoUrl = value; }
        }

        public void Play()
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            if ((mMediaType & 1) > 0)
            {
                optReturn = PlayAudio();
                if (!optReturn.Result)
                {
                    ShowException(optReturn);
                }
            }
            if ((mMediaType & 2) > 0)
            {
                optReturn = PlayVideo();
                if (!optReturn.Result)
                {
                    ShowException(optReturn);
                }
            }
        }

        public void Close()
        {
            try
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
                        LbCurrentTime = "00:00:00";
                        if (mSliderPosition != null)
                        {
                            mSliderPosition.Value = 0;
                        }
                    }
                    mWaveOut.Dispose();
                    mWaveOut = null;
                }
                if (mWaveStream != null)
                {
                    mWaveStream.Dispose();
                    mWaveStream = null;
                }
                if (mCanCloseScreenWindow)
                {
                    ScreenMPInterop.VLSMonCloseWnd();
                }
                if (mIsaPlaybox != null)
                {
                    try
                    {
                        mIsaPlaybox.Close();
                    }
                    catch { }
                    mIsaPlaybox = null;
                }
            }
            catch { }
        }

        public void SetPosition(TimeSpan ts)
        {
            OperationReturn optReturn;
            try
            {
                int intValue = (int)ts.TotalSeconds;
                if (mWaveStream != null)
                {
                    mWaveStream.CurrentTime = TimeSpan.FromSeconds(intValue);
                }
            }
            catch (Exception ex)
            {
                optReturn = new OperationReturn();
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
                ShowException(optReturn);
            }
        }

        public TimeSpan GetCurrentPosition()
        {
            OperationReturn optReturn;
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
                optReturn = new OperationReturn();
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
                ShowException(optReturn);
            }
            return TimeSpan.FromSeconds(0);
        }

        public bool IsIsaScreen
        {
            get { return mIsIsaScreen; }
            set { mIsIsaScreen = value; }
        }

        public string IsaServer
        {
            get { return mIsaServer; }
            set { mIsaServer = value; }
        }

        public string IsaRefID
        {
            get { return mIsaRefID; }
            set { mIsaRefID = value; }
        }

        public int Duration
        {
            get { return mDuration; }
            set { mDuration = value; }
        }

        #endregion


        #region Button Operations

        private void BtnPlay_Click()
        {
            OperationReturn optReturn;
            try
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
                        if (mWaveOut != null)
                        {
                            mWaveOut.Play();
                            mTimer.Start();
                        }
                    }
                }
                if (mMediaType == 2
                    || mMediaType == 3)
                {
                    //PlayVideo();

                    if (mSliderPosition != null)
                    {
                        var value = mSliderPosition.Value;
                        if (value > 0)
                        {
                            //定位到当前位置继续播放
                            if (mIsIsaScreen)
                            {
                                IsaPlay(value);
                            }
                            else
                            {
                                optReturn = PlayVClogVedio();
                                if (!optReturn.Result)
                                {
                                    ShowDebugMessage(string.Format("PlayFail\t{0}\t{1}", optReturn.Code,
                                        optReturn.Message));
                                    return;
                                }
                                int intReturn = ScreenMPInterop.VLSMonSetPlayPos((int)value);
                                ShowDebugMessage(string.Format("VLSMonSetPlayPosReturn:{0}", intReturn));
                            }
                        }
                        else
                        {
                            if (mIsIsaScreen)
                            {
                                IsaPlay();
                            }
                            else
                            {
                                PlayVClogVedio();
                            }
                        }
                    }
                }
                OnPlayerEventEvent(MediaPlayerEventCodes.BTN_PLAY, mAudioUrl);
            }
            catch (Exception ex)
            {
                optReturn = new OperationReturn();
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
                ShowException(optReturn);
            }
        }

        private void BtnPause_Click()
        {
            OperationReturn optReturn;
            try
            {
                if (mWaveOut != null
                              && mWaveStream != null
                              && mWaveOut.PlaybackState == PlaybackState.Playing)
                {
                    mWaveOut.Pause();
                    mTimer.Stop();
                }
                if (mIsIsaScreen)
                {
                    IsaPause();
                }
            }
            catch (Exception ex)
            {
                optReturn = new OperationReturn();
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
                ShowException(optReturn);
            }

        }

        private void BtnStop_Click()
        {
            OperationReturn optReturn;
            try
            {
                if (mWaveOut != null
                             && mWaveStream != null)
                {
                    TimeSpan ts = mWaveStream.CurrentTime;
                    mWaveStream.Position = 0;
                    mWaveOut.Stop();
                    mTimer.Stop();
                    LbCurrentTime = "00:00:00";
                    OnPlayerEventEvent(MediaPlayerEventCodes.BTN_STOP, ts);
                }
                if (mMediaType == 2
                    || mMediaType == 3)
                {
                    //录屏停止
                    if (mIsIsaScreen)
                    {
                        IsaStop();
                    }
                    else
                    {
                        int intReturn = ScreenMPInterop.VLSMonCloseWnd();
                        ShowDebugMessage(string.Format("VLSMonCloseWndReturn:{0}", intReturn));
                    }
                }
            }
            catch (Exception ex)
            {
                optReturn = new OperationReturn();
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
                ShowException(optReturn);
            }
        }

        private void BtnSlower_Click()
        {
            OperationReturn optReturn;
            try
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
                LbRate = mRate.ToString("0.0");
            }
            catch (Exception ex)
            {
                optReturn = new OperationReturn();
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
                ShowException(optReturn);
            }
        }

        private void BtnFaster_Click()
        {
            OperationReturn optReturn;
            try
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
                LbRate = mRate.ToString("0.0");
            }
            catch (Exception ex)
            {
                optReturn = new OperationReturn();
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
                ShowException(optReturn);
            }
        }

        private void BtnChannelMode_Click()
        {
            OperationReturn optReturn;
            try
            {
                var btnChannelMode = mListRightButtons.FirstOrDefault(b => b.Name == "ChannelMode");
                if (btnChannelMode == null) { return; }
                if (mChannelMode == WaveOutChannelMode.Default)
                {
                    mChannelMode = WaveOutChannelMode.Left;
                    btnChannelMode.Display = "L";
                    if (mImageLeft != null)
                    {
                        mImageLeft.Opacity = 1;
                    }
                    if (mImageRight != null)
                    {
                        mImageRight.Opacity = 0.5;
                    }
                }
                else if (mChannelMode == WaveOutChannelMode.Left)
                {
                    mChannelMode = WaveOutChannelMode.Right;
                    btnChannelMode.Display = "R";
                    if (mImageLeft != null)
                    {
                        mImageLeft.Opacity = 0.5;
                    }
                    if (mImageRight != null)
                    {
                        mImageRight.Opacity = 1;
                    }
                }
                else
                {
                    mChannelMode = WaveOutChannelMode.Default;
                    btnChannelMode.Display = "D";
                    if (mImageLeft != null)
                    {
                        mImageLeft.Opacity = 1;
                    }
                    if (mImageRight != null)
                    {
                        mImageRight.Opacity = 1;
                    }
                }
                if (mWaveOut != null)
                {
                    mWaveOut.ChannelMode = mChannelMode;
                }
            }
            catch (Exception ex)
            {
                optReturn = new OperationReturn();
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
                ShowException(optReturn);
            }
        }

        private void BtnClose_Click()
        {
            OperationReturn optReturn;
            try
            {
                OnPlayerEventEvent(MediaPlayerEventCodes.BTN_CLOSE, null);
                Close();
            }
            catch (Exception ex)
            {
                optReturn = new OperationReturn();
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
                ShowException(optReturn);
            }
        }

        #endregion


        #region Event Handlers

        private void mTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (mMediaType == 2)
            {
                //纯录屏显示录屏的播放进度及总时长
                Dispatcher.Invoke(new Action(() =>
                {
                    OperationReturn optReturn;
                    try
                    {
                        LbCurrentTime = Converter.Second2Time(mScreenPlayedLength);
                        TimeSpan ts = TimeSpan.FromSeconds(mScreenPlayedLength);
                        if (mSliderPosition != null && !mIsDraging)
                        {
                            mSliderPosition.Value = mScreenPlayedLength;
                        }
                        OnPlayerEventEvent(MediaPlayerEventCodes.PLAYING, ts);
                    }
                    catch (Exception ex)
                    {
                        optReturn = new OperationReturn();
                        optReturn.Code = Defines.RET_FAIL;
                        optReturn.Message = ex.Message;
                        ShowException(optReturn);
                    }
                }));
                return;
            }
            if (mWaveStream != null)
            {
                TimeSpan ts = mWaveStream.CurrentTime;
                Dispatcher.Invoke(new Action(() =>
                {
                    OperationReturn optReturn;
                    try
                    {
                        LbCurrentTime = Converter.Second2Time(ts.TotalSeconds);
                        if (mWaveStream != null)
                        {
                            if (mSliderPosition != null && !mIsDraging)
                            {
                                mSliderPosition.Value = mWaveStream.CurrentTime.TotalSeconds;
                            }
                            OnPlayerEventEvent(MediaPlayerEventCodes.PLAYING, ts);
                        }
                    }
                    catch (Exception ex)
                    {
                        optReturn = new OperationReturn();
                        optReturn.Code = Defines.RET_FAIL;
                        optReturn.Message = ex.Message;
                        ShowException(optReturn);
                    }
                }));
            }
        }

        void mWaveOut_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            if (mWaveStream != null)
            {
                mWaveStream.Position = 0;
                if (mSliderPosition != null)
                {
                    mSliderPosition.Value = 0;
                }
                mWaveOut.Stop();
                mTimer.Stop();
            }
            OnPlayerEventEvent(MediaPlayerEventCodes.PLAYBACKSTOPPED, null);
        }

        void postVolumeMeter_StreamVolume(object sender, StreamVolumeEventArgs e)
        {

        }

        void waveChannel_PreVolumeMeter(object sender, StreamVolumeEventArgs e)
        {

        }

        void mSliderPosition_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (mIsDraging)
                {
                    var value = mSliderPosition.Value;
                    TimeSpan ts = TimeSpan.FromSeconds((int)value);
                    if (mWaveStream != null)
                    {
                        mWaveStream.CurrentTime = ts;
                    }
                    if (mMediaType == 2
                        || mMediaType == 3)
                    {
                        //为录屏播放器定位
                        if (mIsIsaScreen)
                        {
                            IsaPosition(value);
                        }
                        else
                        {
                            int intReturn = ScreenMPInterop.VLSMonSetPlayPos((int)value);
                            ShowDebugMessage(string.Format("VLSMonSetPlayPosReturn:{0}", intReturn));
                        }
                    }
                    mIsDraging = false;
                }
            }
            catch (Exception ex)
            {
                ShowDebugMessage(string.Format("Fail.\t{0}", ex.Message));
            }
        }

        #endregion


        #region 按钮命令

        public static RoutedUICommand ButtonCommand = new RoutedUICommand();

        private static void ButtonCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var player = sender as UMPMediaPlayer;
            if (player != null)
            {
                var btn = e.Parameter as ImageButtonItem;
                if (btn != null)
                {
                    player.ButtonCommand_Executed(player, btn);
                }
            }
        }

        public void ButtonCommand_Executed(object sender, ImageButtonItem item)
        {
            switch (item.Name)
            {
                case "Play":
                    BtnPlay_Click();
                    break;
                case "Pause":
                    BtnPause_Click();
                    break;
                case "Stop":
                    BtnStop_Click();
                    break;
                case "Slower":
                    BtnSlower_Click();
                    break;
                case "Faster":
                    BtnFaster_Click();
                    break;
                case "ChannelMode":
                    BtnChannelMode_Click();
                    break;
                case "Close":
                    BtnClose_Click();
                    break;
            }
        }

        #endregion


        #region Operations

        private OperationReturn PlayAudio()
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            if (string.IsNullOrEmpty(mAudioUrl))
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_STRING_EMPTY;
                optReturn.Message = string.Format(("AudioUrl is empty"));
                return optReturn;
            }
            try
            {
                WaveStream reader;
                if (mAudioUrl.StartsWith("http", StringComparison.InvariantCultureIgnoreCase))
                {

                    if (mAudioUrl.EndsWith(".mp3", StringComparison.InvariantCultureIgnoreCase))
                    {
                        reader = new Mp3NetworkStream(mAudioUrl);
                    }
                    else
                    {
                        reader = new NetWorkWaveReader(mAudioUrl);
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
                    if (mAudioUrl.EndsWith(".mp3", StringComparison.InvariantCultureIgnoreCase))
                    {
                        reader = new Mp3FileReader(mAudioUrl);
                    }
                    else
                    {
                        reader = new WaveFileReader(mAudioUrl);
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
                OnPlayerEventEvent(MediaPlayerEventCodes.MEDIAOPENED, mWaveStream.TotalTime);

                if (!CreateWaveImage(mWaveStream))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_FAIL;
                    optReturn.Message = string.Format(("CreateWaveImage fail."));
                    return optReturn;
                }

                LbTotalTime = Converter.Second2Time(mWaveStream.TotalTime.TotalSeconds);
                LbCurrentTime = "00:00:00";
                if (mSliderPosition != null)
                {
                    mSliderPosition.Minimum = 0;
                    mSliderPosition.Maximum = mWaveStream.TotalTime.TotalSeconds;
                }

                if (!CreateWaveStream())
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_FAIL;
                    optReturn.Message = string.Format(("CreateWaveStream fail."));
                    return optReturn;
                }
                if (!CreateWaveOut())
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_FAIL;
                    optReturn.Message = string.Format(("CreateWaveOut fail."));
                    return optReturn;
                }
                if (mWaveOut != null)
                {
                    mWaveOut.Play();
                    mTimer.Start();

                    OnPlayerEventEvent(MediaPlayerEventCodes.PLAYBACKSTARTED, mAudioUrl);
                }
                return optReturn;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
                optReturn.Exception = ex;
                return optReturn;
            }
        }

        private OperationReturn PlayVideo()
        {
            if (mIsIsaScreen)
            {
                return PlayIsaVedio();
            }
            return PlayVClogVedio();
        }

        private OperationReturn PlayVClogVedio()
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //获取本窗口的句柄
                int handle = 0;
                HwndSource hwndSource = HwndSource.FromVisual(this) as HwndSource;
                if (hwndSource != null)
                {
                    hwndSource.AddHook(WndProc);
                    handle = hwndSource.Handle.ToInt32();
                }
                int intReturn;
                bool boolReturn;
                int intTotalLength = 0;
                //设置反馈进度的频率
                intReturn = ScreenMPInterop.VLSMonSetWaterMark(1);
                ShowDebugMessage(string.Format("SetWaterMarkReturn:{0}", intReturn));
                //设置窗口尺寸
                int intScale = 100;
                if (ScreenScale > 0 && ScreenScale <= 100)
                {
                    intScale = ScreenScale;
                }
                intReturn = ScreenMPInterop.VLSMonSetScale(intScale);
                ShowDebugMessage(string.Format("SetScale:{0}", intReturn));
                //播放录屏数据
                intReturn = ScreenMPInterop.VLSMonPlay(VideoUrl, null, handle);
                ShowDebugMessage(string.Format("PlayReturn:{0}", intReturn));
                if (intReturn == 0)
                {
                    mCanCloseScreenWindow = true;

                    //获取播放窗口句柄
                    IntPtr ptr = ScreenMPInterop.VLSMonGetPlayerHwnd();
                    ShowDebugMessage(string.Format("IntPtr:{0}", ptr));
                    mHwndPlayer = ptr;

                    //窗口置顶
                    if (ScreenTopMost && mHwndPlayer != IntPtr.Zero)
                    {
                        intReturn = User32Interop.SetWindowPos(mHwndPlayer, -1, 0, 0, 0, 0, 3);
                        ShowDebugMessage(string.Format("SetWinPosReturn:{0}", intReturn));
                    }

                    //设置标题
                    if (mHwndPlayer != IntPtr.Zero)
                    {
                        boolReturn = User32Interop.SetWindowText(mHwndPlayer, VideoUrl);
                        ShowDebugMessage(string.Format("SetWinTxtReturn:{0}", boolReturn));
                    }

                    //获取媒体文件总时长
                    intReturn = ScreenMPInterop.VLSMonGetFileTotalTime(VideoUrl, ref intTotalLength, false);
                    ShowDebugMessage(string.Format("GetTotalTimeReturn:{0}", intReturn));
                    if (intReturn == 0)
                    {
                        mScreenTotalLength = intTotalLength;
                        if (mMediaType == 2)
                        {
                            //纯录屏显示录屏的播放进度及总时长
                            if (mSliderPosition != null)
                            {
                                mSliderPosition.Maximum = mScreenTotalLength;
                            }
                            LbTotalTime = Converter.Second2Time(mScreenTotalLength);
                        }
                    }

                    mTimer.Start();
                }
                else
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_FAIL;
                    optReturn.Message = string.Format("Fail.\t{0}", intReturn);
                }
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
                optReturn.Exception = ex;
            }
            return optReturn;
        }

        private OperationReturn PlayIsaVedio()
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                if (string.IsNullOrEmpty(mIsaRefID)
                    || string.IsNullOrEmpty(mIsaServer))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("RefID or ServerAddress empty");
                    return optReturn;
                }
                if (Session == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Session is null");
                    return optReturn;
                }
                optReturn = CreateMediaClient();
                if (!optReturn.Result)
                {
                    return optReturn;
                }

                mScreenTotalLength = mDuration;
                if (mMediaType == 2)
                {
                    //纯录屏显示录屏的播放进度及总时长
                    if (mSliderPosition != null)
                    {
                        mSliderPosition.Maximum = mScreenTotalLength;
                    }
                    LbTotalTime = Converter.Second2Time(mScreenTotalLength);
                }

                mTimer.Start();
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
                optReturn.Exception = ex;
            }
            return optReturn;
        }

        private OperationReturn CreateMediaClient()
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                if (Session == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Session is null");
                    return optReturn;
                }
                var appServer = Session.AppServerInfo;
                if (appServer == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("AppServerInfo is null");
                    return optReturn;
                }
                if (mMediaClient != null)
                {
                    mMediaClient.Stop();
                    mMediaClient = null;
                }
                mMediaClient = new NetClient();
                mMediaClient.Debug +=
                    (mode, cat, msg) => ShowDebugMessage(string.Format("MediaClient\t{0}\t{1}", cat, msg));
                mMediaClient.ConnectionEvent += MediaClient_ConnectionEvent;
                mMediaClient.ReturnMessageReceived += MediaClient_ReturnMessageReceived;
                mMediaClient.NotifyMessageReceived += MediaClient_NotifyMessageReceived;
                mMediaClient.IsSSL = true;
                mMediaClient.Host = appServer.Address;
                mMediaClient.Port = appServer.SupportHttps ? appServer.Port - 4 : appServer.Port - 3;
                optReturn = mMediaClient.Connect();
                if (!optReturn.Result)
                {
                    return optReturn;
                }
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
                optReturn.Exception = ex;
            }
            return optReturn;
        }

        private void CreateIsaPlaybox()
        {
            try
            {
                if (mIsaPlaybox != null)
                {
                    try
                    {
                        mIsaPlaybox.Close();
                        mIsaPlaybox = null;
                    }
                    catch { }
                }
                mIsaPlaybox = new IsaPlaybox();
                mIsaPlaybox.PlayboxEvent += IsaPlaybox_PlayboxEvent;
                //设置窗口尺寸
                int intScale = 100;
                if (ScreenScale > 0 && ScreenScale <= 100)
                {
                    intScale = ScreenScale;
                }
                mIsaPlaybox.ScreenScale = intScale;
                //窗口置顶
                mIsaPlaybox.Topmost = ScreenTopMost;
                mIsaPlaybox.Title = string.Format("{0}({1})", mIsaRefID, mIsaServer);
                mIsaPlaybox.Show();
            }
            catch (Exception ex)
            {
                ShowDebugMessage(string.Format("CreateIsaPlaybox fail.\t{0}", ex.Message));
            }
        }

        private void StartIsa()
        {
            try
            {
                if (mMediaClient == null) { return; }
                if (string.IsNullOrEmpty(mIsaServer))
                {
                    ShowDebugMessage(string.Format("IsaServer is empty"));
                    return;
                }
                string strAddress = mIsaServer;
                RequestMessage request = new RequestMessage();
                request.SessionID = mMediaClient.SessionID;
                request.Command = (int)Service03Command.IsaStart;
                request.ListData.Add(strAddress);
                mMediaClient.SendMessage(request.Command, request);
                ShowDebugMessage(string.Format("StartIsa command sended"));
            }
            catch (Exception ex)
            {
                ShowDebugMessage(string.Format("StartIsa fail.\t{0}", ex.Message));
            }
        }

        private void IsaPlay()
        {
            try
            {
                if (mMediaClient == null) { return; }
                if (string.IsNullOrEmpty(mIsaRefID))
                {
                    ShowDebugMessage(string.Format("IsaRefIDis empty"));
                    return;
                }
                string strAction = "play";
                string strRefID = mIsaRefID;
                string strPosition = "0";
                string strSpeed = "0";
                RequestMessage request = new RequestMessage();
                request.SessionID = mMediaClient.SessionID;
                request.Command = (int)Service03Command.IsaBehavior;
                request.ListData.Add(strAction);
                request.ListData.Add(strRefID);
                request.ListData.Add(strPosition);
                request.ListData.Add(strSpeed);
                mMediaClient.SendMessage(request.Command, request);
                mIsScreenPlaying = true;
                ShowDebugMessage(string.Format("IsaPlay command sended"));
            }
            catch (Exception ex)
            {
                ShowDebugMessage(string.Format("IsaPlay fail.\t{0}", ex.Message));
            }
        }

        private void IsaPlay(double pos)
        {
            try
            {
                if (mMediaClient == null) { return; }
                if (string.IsNullOrEmpty(mIsaRefID))
                {
                    ShowDebugMessage(string.Format("IsaRefIDis empty"));
                    return;
                }
                string strAction = "resume";
                string strRefID = mIsaRefID;
                string strPosition = (pos * 1000).ToString();
                string strSpeed = "0";
                RequestMessage request = new RequestMessage();
                request.SessionID = mMediaClient.SessionID;
                request.Command = (int)Service03Command.IsaBehavior;
                request.ListData.Add(strAction);
                request.ListData.Add(strRefID);
                request.ListData.Add(strPosition);
                request.ListData.Add(strSpeed);
                mMediaClient.SendMessage(request.Command, request);
                mIsScreenPlaying = true;
                ShowDebugMessage(string.Format("IsaPlay command sended"));
            }
            catch (Exception ex)
            {
                ShowDebugMessage(string.Format("IsaPlay fail.\t{0}", ex.Message));
            }
        }

        private void IsaPause()
        {
            try
            {
                if (mMediaClient == null) { return; }
                if (string.IsNullOrEmpty(mIsaRefID))
                {
                    ShowDebugMessage(string.Format("IsaRefIDis empty"));
                    return;
                }
                string strAction = "pause";
                string strRefID = mIsaRefID;
                string strPosition = "0";
                string strSpeed = "0";
                RequestMessage request = new RequestMessage();
                request.SessionID = mMediaClient.SessionID;
                request.Command = (int)Service03Command.IsaBehavior;
                request.ListData.Add(strAction);
                request.ListData.Add(strRefID);
                request.ListData.Add(strPosition);
                request.ListData.Add(strSpeed);
                mMediaClient.SendMessage(request.Command, request);
                mIsScreenPlaying = false;
                ShowDebugMessage(string.Format("IsaPause command sended"));
            }
            catch (Exception ex)
            {
                ShowDebugMessage(string.Format("IsaPause fail.\t{0}", ex.Message));
            }
        }

        private void IsaStop()
        {
            try
            {
                if (mMediaClient == null) { return; }
                if (string.IsNullOrEmpty(mIsaRefID))
                {
                    ShowDebugMessage(string.Format("IsaRefIDis empty"));
                    return;
                }
                string strAction = "stop";
                string strRefID = mIsaRefID;
                string strPosition = "0";
                string strSpeed = "0";
                RequestMessage request = new RequestMessage();
                request.SessionID = mMediaClient.SessionID;
                request.Command = (int)Service03Command.IsaBehavior;
                request.ListData.Add(strAction);
                request.ListData.Add(strRefID);
                request.ListData.Add(strPosition);
                request.ListData.Add(strSpeed);
                mMediaClient.SendMessage(request.Command, request);
                mIsScreenPlaying = false;
                if (mMediaType == 2)
                {
                    mScreenPlayedLength = 0;
                }
                ShowDebugMessage(string.Format("IsaStop command sended"));
            }
            catch (Exception ex)
            {
                ShowDebugMessage(string.Format("IsaStop fail.\t{0}", ex.Message));
            }
        }

        private void IsaPosition(double pos)
        {
            try
            {
                if (mMediaClient == null) { return; }
                if (string.IsNullOrEmpty(mIsaRefID))
                {
                    ShowDebugMessage(string.Format("IsaRefIDis empty"));
                    return;
                }
                string strAction = "jump";
                string strRefID = mIsaRefID;
                string strPosition = (pos * 1000).ToString();
                string strSpeed = "0";
                RequestMessage request = new RequestMessage();
                request.SessionID = mMediaClient.SessionID;
                request.Command = (int)Service03Command.IsaBehavior;
                request.ListData.Add(strAction);
                request.ListData.Add(strRefID);
                request.ListData.Add(strPosition);
                request.ListData.Add(strSpeed);
                mMediaClient.SendMessage(request.Command, request);
                ShowDebugMessage(string.Format("IsaPosition command sended"));
            }
            catch (Exception ex)
            {
                ShowDebugMessage(string.Format("IsaPosition fail.\t{0}", ex.Message));
            }
        }

        private OperationReturn DownloadImage(string file)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                if (string.IsNullOrEmpty(file))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_STRING_EMPTY;
                    optReturn.Message = string.Format("FileName empty");
                    return optReturn;
                }
                if (Session == null || Session.AppServerInfo == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("SessionInfo or AppServerInfo is null");
                    return optReturn;
                }
                AppServerInfo appInfo = Session.AppServerInfo;
                string requestPath = Path.Combine(ConstValue.TEMP_DIR_MEDIADATA, file);
                string savePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), ConstValue.TEMP_DIR_UMP, ConstValue.TEMP_DIR_MEDIADATA,
                    file);
                DownloadConfig config = new DownloadConfig();
                config.Method = appInfo.Protocol == "https" ? 2 : 1;
                config.Host = appInfo.Address;
                config.Port = appInfo.Port;
                config.IsAnonymous = true;
                config.RequestPath = requestPath;
                config.SavePath = savePath;
                config.IsReplace = true;
                optReturn = DownloadHelper.DownloadFile(config);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                optReturn.Data = savePath;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
                optReturn.Exception = ex;
            }
            return optReturn;
        }

        void MediaClient_NotifyMessageReceived(object sender, NotifyMessageReceivedEventArgs e)
        {
            try
            {
                var notify = e.NotifyMessage;
                string str = string.Empty;
                if (notify.ListData != null)
                {
                    if (notify.ListData.Count >= 2)
                    {
                        string strPos = notify.ListData[0];
                        string strFile = notify.ListData[1];
                        str += string.Format("{0}\t{1};", strPos, strFile);

                        if (!mIsScreenPlaying) { return; }
                        if (!string.IsNullOrEmpty(strFile))
                        {
                            if (Session != null && Session.AppServerInfo != null)
                            {
                                OperationReturn optReturn = DownloadImage(strFile);
                                if (!optReturn.Result)
                                {
                                    ShowDebugMessage(string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message));
                                    return;
                                }
                                string path = optReturn.Data.ToString();
                                if (!string.IsNullOrEmpty(path))
                                {
                                    if (mIsaPlaybox != null)
                                    {
                                        mIsaPlaybox.SetImgSource(path);
                                    }
                                }
                            }
                        }
                        double doublePos;
                        if (double.TryParse(strPos, out doublePos))
                        {
                            mScreenPlayedLength = doublePos / 1000;
                        }
                    }
                }
                ShowDebugMessage(str);
            }
            catch (Exception ex)
            {
                ShowDebugMessage(string.Format("DealNotify fail.\t{0}", ex.Message));
            }
        }

        void MediaClient_ReturnMessageReceived(object sender, ReturnMessageReceivedEventArgs e)
        {
            try
            {
                var retMessage = e.ReturnMessage;
                if (retMessage.Command == (int)Service03Command.IsaStart)
                {
                    ShowDebugMessage(string.Format("Replay:{0}", (Service03Command)retMessage.Command));
                    Dispatcher.Invoke(new Action(CreateIsaPlaybox));
                }
            }
            catch (Exception ex)
            {
                ShowDebugMessage(string.Format("DealReturn fail.\t{0}", ex.Message));
            }
        }

        void MediaClient_ConnectionEvent(object sender, ConnectionEventArgs e)
        {
            try
            {
                if (e.Code == Defines.EVT_NET_CONNECTED)
                {
                    StartIsa();
                }
            }
            catch (Exception ex)
            {
                ShowDebugMessage(string.Format("DealConnection fail.\t{0}", ex.Message));
            }
        }

        void IsaPlaybox_PlayboxEvent(object sender, UMPEventArgs e)
        {
            try
            {
                if (e.Code == Defines.EVT_PAGE_LOADED)
                {
                    IsaPlay();
                }
                if (e.Code == Defines.EVT_PAGE_UNLOADED)
                {
                    IsaStop();
                    if (mMediaClient != null)
                    {
                        mMediaClient.Stop();
                        mMediaClient = null;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowDebugMessage(string.Format("DealPlaybox fail.\t{0}", ex.Message));
            }
        }

        #endregion


        #region Others

        private bool CreateWaveStream()
        {
            OperationReturn optReturn;
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
                optReturn = new OperationReturn();
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
                ShowException(optReturn);
                return false;
            }
        }

        private bool CreateWaveOut()
        {
            OperationReturn optReturn;
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
                mWaveOut.Volume = (float)Volume;
                return true;
            }
            catch (Exception ex)
            {
                optReturn = new OperationReturn();
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
                ShowException(optReturn);
                return false;
            }
        }

        private bool CreateWaveImage(WaveStream waveStream)
        {
            OperationReturn optReturn;
            if (waveStream == null)
            {
                return false;
            }
            try
            {
                WavePainter wavePainter = new WavePainter();
                wavePainter.VolumnNum = 2;
                wavePainter.IsSaveFile = false;
                OptReturn audioReturn = wavePainter.Draw(waveStream);
                if (!audioReturn.Result)
                {
                    optReturn = new OperationReturn();
                    optReturn.Result = false;
                    optReturn.Code = audioReturn.Code;
                    optReturn.Message = audioReturn.Message;
                    ShowException(optReturn);
                    return false;
                }
                var data = audioReturn.Data as Bitmap[];
                if (data != null)
                {
                    var bitmap = data[0];
                    var imgsource = Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                    if (mImageLeft != null)
                    {
                        mImageLeft.Source = imgsource;
                    }
                    bitmap = data[1];
                    imgsource = Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                    if (mImageRight != null)
                    {
                        mImageRight.Source = imgsource;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                optReturn = new OperationReturn();
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
                ShowException(optReturn);
                return false;
            }
        }

        private void ShowException(OperationReturn optReturn)
        {
            Dispatcher.Invoke(new Action(() => OnPlayerEventEvent(Defines.EVT_EXCEPTION, optReturn)));
        }

        private void ShowDebugMessage(string msg)
        {
            Dispatcher.Invoke(new Action(() => OnPlayerEventEvent(Defines.EVT_COMMON, msg)));
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case ScreenMPDefines.WM_WATERMARK:
                    ShowDebugMessage(string.Format("Hwnd:{0}\tMsg:0x{1:X}\tWParam:{2}\tLParam:{3}",
                       hwnd, msg,
                       wParam,
                       lParam));
                    handled = true;
                    try
                    {
                        var pos = wParam.ToInt32();
                        mScreenPlayedLength = pos;
                    }
                    catch (Exception ex)
                    {
                        ShowDebugMessage(string.Format("Fail.\t{0}", ex.Message));
                    }
                    break;
                case ScreenMPDefines.WM_CONNECTION_LOST:
                case ScreenMPDefines.WM_UNKNOWN_ERROR:
                case ScreenMPDefines.WM_PLAY_COMPLETED:
                case ScreenMPDefines.WM_MONIT_START:
                case ScreenMPDefines.WM_WND_CLOSE:
                case ScreenMPDefines.VM_PLAY_OVER:
                    ShowDebugMessage(string.Format("Hwnd:{0}\tMsg:0x{1:X}\tWParam:{2}\tLParam:{3}",
                        hwnd, msg,
                        wParam,
                        lParam));
                    handled = true;
                    break;
            }
            return IntPtr.Zero;
        }

        #endregion


        #region DependencyProperties

        #region Title

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof (string), typeof (UMPMediaPlayer), new PropertyMetadata(default(string)));

        public string Title
        {
            get { return (string) GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        #endregion

        #region LbRate

        public static readonly DependencyProperty LbRateProperty =
           DependencyProperty.Register("LbRate", typeof(string), typeof(UMPMediaPlayer), new PropertyMetadata(default(string)));

        public string LbRate
        {
            get { return (string)GetValue(LbRateProperty); }
            set { SetValue(LbRateProperty, value); }
        }

        #endregion


        #region LbVolume

        public static readonly DependencyProperty LbVolumeProperty =
           DependencyProperty.Register("LbVolume", typeof(string), typeof(UMPMediaPlayer), new PropertyMetadata(default(string)));

        public string LbVolume
        {
            get { return (string)GetValue(LbVolumeProperty); }
            set { SetValue(LbVolumeProperty, value); }
        }

        #endregion


        #region LbCurrentTime

        public static readonly DependencyProperty LbCurrentTimeProperty =
          DependencyProperty.Register("LbCurrentTime", typeof(string), typeof(UMPMediaPlayer), new PropertyMetadata(default(string)));

        public string LbCurrentTime
        {
            get { return (string)GetValue(LbCurrentTimeProperty); }
            set { SetValue(LbCurrentTimeProperty, value); }
        }

        #endregion


        #region LbTotalTime

        public static readonly DependencyProperty LbTotalTimeProperty =
          DependencyProperty.Register("LbTotalTime", typeof(string), typeof(UMPMediaPlayer), new PropertyMetadata(default(string)));

        public string LbTotalTime
        {
            get { return (string)GetValue(LbTotalTimeProperty); }
            set { SetValue(LbTotalTimeProperty, value); }
        }

        #endregion


        #region IsImageLeftVisible

        public static readonly DependencyProperty IsImageLeftVisibleProperty =
           DependencyProperty.Register("IsImageLeftVisible", typeof(bool), typeof(UMPMediaPlayer), new PropertyMetadata(default(bool)));

        public bool IsImageLeftVisible
        {
            get { return (bool)GetValue(IsImageLeftVisibleProperty); }
            set { SetValue(IsImageLeftVisibleProperty, value); }
        }

        #endregion


        #region IsImageRightVisible

        public static readonly DependencyProperty IsImageRightVisibleProperty =
          DependencyProperty.Register("IsImageRightVisible", typeof(bool), typeof(UMPMediaPlayer), new PropertyMetadata(default(bool)));

        public bool IsImageRightVisible
        {
            get { return (bool)GetValue(IsImageRightVisibleProperty); }
            set { SetValue(IsImageRightVisibleProperty, value); }
        }

        #endregion


        #region IsVolumeVisible

        public static readonly DependencyProperty IsVolumeVisibleProperty =
          DependencyProperty.Register("IsVolumeVisible", typeof(bool), typeof(UMPMediaPlayer), new PropertyMetadata(default(bool)));

        public bool IsVolumeVisible
        {
            get { return (bool)GetValue(IsVolumeVisibleProperty); }
            set { SetValue(IsVolumeVisibleProperty, value); }
        }

        #endregion


        #region Volume

        public static readonly DependencyProperty VolumeProperty =
          DependencyProperty.Register("Volume", typeof(double), typeof(UMPMediaPlayer), new PropertyMetadata(1.0, OnVolumeChanged));

        public double Volume
        {
            get { return (double)GetValue(VolumeProperty); }
            set { SetValue(VolumeProperty, value); }
        }

        private static void OnVolumeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var player = d as UMPMediaPlayer;
            if (player != null)
            {
                player.OnVolumeChanged((double)e.OldValue, (double)e.NewValue);
            }
        }

        protected void OnVolumeChanged(double oldValue, double newValue)
        {
            LbVolume = Volume.ToString("0.0");
            if (mWaveOut != null)
            {
                mWaveOut.Volume = (float)Volume;
            }
        }

        #endregion


        #region State

        public static readonly DependencyProperty StateProperty =
          DependencyProperty.Register("State", typeof(string), typeof(UMPMediaPlayer), new PropertyMetadata(default(string)));
        /// <summary>
        /// 左起
        /// 1：ImageLeft
        /// 2：ImageRight
        /// 3：Open
        /// 4：Play
        /// 5：Pause
        /// 6：Stop
        /// 7：Slower
        /// 8：Faster
        /// 9：Volume
        /// 10：ChannelMode
        /// 11：Close
        /// </summary>
        public string State
        {
            get { return (string)GetValue(StateProperty); }
            set { SetValue(StateProperty, value); }
        }

        #endregion


        #region ScreenTopMost

        public static readonly DependencyProperty ScreenTopMostProperty =
            DependencyProperty.Register("ScreenTopMost", typeof(bool), typeof(UMPMediaPlayer), new PropertyMetadata(true));

        public bool ScreenTopMost
        {
            get { return (bool)GetValue(ScreenTopMostProperty); }
            set { SetValue(ScreenTopMostProperty, value); }
        }

        #endregion


        #region ScreenScale

        public static readonly DependencyProperty ScreenScalProperty =
            DependencyProperty.Register("ScreenScal", typeof(int), typeof(UMPMediaPlayer), new PropertyMetadata(0));

        public int ScreenScale
        {
            get { return (int)GetValue(ScreenScalProperty); }
            set { SetValue(ScreenScalProperty, value); }
        }

        #endregion

        #endregion


        #region 播放器事件

        public static readonly RoutedEvent PlayerEventEvent;

        public event RoutedPropertyChangedEventHandler<UMPEventArgs> PlayerEvent
        {
            add { AddHandler(PlayerEventEvent, value); }
            remove { RemoveHandler(PlayerEventEvent, value); }
        }

        private void OnPlayerEventEvent(int code, object data)
        {
            if (PlayerEventEvent != null)
            {
                UMPEventArgs args = new UMPEventArgs();
                args.Code = code;
                args.Data = data;
                RoutedPropertyChangedEventArgs<UMPEventArgs> a = new RoutedPropertyChangedEventArgs<UMPEventArgs>(
                    null, args);
                a.RoutedEvent = PlayerEventEvent;
                RaiseEvent(a);
            }
        }

        #endregion


        #region Template

        private const string PART_ImageLeft = "PART_ImageLeft";
        private const string PART_ImageRight = "PART_ImageRight";
        private const string PART_SliderPosition = "PART_SliderPosition";
        private const string PART_ListLeftButtons = "PART_ListLeftButtons";
        private const string PART_ListRightButtons = "PART_ListRightButtons";

        private Image mImageLeft;
        private Image mImageRight;
        private Slider mSliderPosition;
        private ListBox mListBoxLeft;
        private ListBox mListBoxRight;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            mImageLeft = GetTemplateChild(PART_ImageLeft) as Image;
            if (mImageLeft != null)
            {

            }
            mImageRight = GetTemplateChild(PART_ImageRight) as Image;
            if (mImageRight != null)
            {

            }
            mSliderPosition = GetTemplateChild(PART_SliderPosition) as Slider;
            if (mSliderPosition != null)
            {
                mSliderPosition.AddHandler(MouseLeftButtonDownEvent,
                   new MouseButtonEventHandler((s, e) => mIsDraging = true), true);
                mSliderPosition.AddHandler(MouseLeftButtonUpEvent,
                    new MouseButtonEventHandler(mSliderPosition_MouseLeftButtonUp), true);
                mSliderPosition.MouseLeave += (s, e) => mIsDraging = false;
            }
            mListBoxLeft = GetTemplateChild(PART_ListLeftButtons) as ListBox;
            if (mListBoxLeft != null)
            {
                mListBoxLeft.ItemsSource = mListLeftButtons;
            }
            mListBoxRight = GetTemplateChild(PART_ListRightButtons) as ListBox;
            if (mListBoxRight != null)
            {
                mListBoxRight.ItemsSource = mListRightButtons;
            }
        }

        #endregion

    }
}
