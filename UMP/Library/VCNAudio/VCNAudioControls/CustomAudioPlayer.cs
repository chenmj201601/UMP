using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using VoiceCyber.NAudio.Wave;
using VoiceCyber.NAudio.Wave.SampleProviders;
using Image = System.Windows.Controls.Image;

namespace VoiceCyber.NAudio.Controls
{
    /// <summary>
    /// 自定义的音频播放器
    /// </summary>
    public class CustomAudioPlayer : Control
    {
        static CustomAudioPlayer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomAudioPlayer),
                new FrameworkPropertyMetadata(typeof(CustomAudioPlayer)));

            PlayerEventEvent = EventManager.RegisterRoutedEvent("PlayerEvent", RoutingStrategy.Bubble,
                typeof(RoutedPropertyChangedEventHandler<PlayerEventArgs>), typeof(CustomAudioPlayer));
        }

        public CustomAudioPlayer()
        {
            Loaded += CustomAudioPlayer_Loaded;
            Unloaded += CustomAudioPlayer_Unloaded;

            mListLeftButtons = new ObservableCollection<ImageButtonItem>();
            mListRightButtons = new ObservableCollection<ImageButtonItem>();
            mTimer = new Timer();
            mTimer.Elapsed += mTimer_Elapsed;

            CommandBindings.Add(new CommandBinding(ButtonCommand, ButtonCommand_Executed, (s, e) => e.CanExecute = true));

            Init();
        }

        void CustomAudioPlayer_Loaded(object sender, RoutedEventArgs e)
        {
            InitLeftButtons();
            InitRightButtons();
            InitState();
            OnPlayerEventEvent(Defines.EVT_UC_LOADED, null);
        }

        void CustomAudioPlayer_Unloaded(object sender, RoutedEventArgs e)
        {
            OnPlayerEventEvent(Defines.EVT_UC_UNLOADED, null);
            Close();
        }


        #region Members

        private ObservableCollection<ImageButtonItem> mListLeftButtons;
        private ObservableCollection<ImageButtonItem> mListRightButtons;

        private WaveOut mWaveOut;
        private WaveStream mWaveStream;
        private ISampleProvider mSampleProvider;
        private WaveOutChannelMode mChannelMode;
        private Timer mTimer;
        private double mRate;
        private string mUrl;
        private bool mIsDraging;

        #endregion


        #region Init and Load

        private void Init()
        {
            try
            {
                mUrl = string.Empty;
                mTimer.Interval = 100;
                mRate = 1.0;
                mChannelMode = WaveOutChannelMode.Default;
                State = "11111111111";
                Volume = 1;
                mIsDraging = false;

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

            if (State.Length > 2 && State.Substring(2, 1) == "1")
            {
                item = new ImageButtonItem();
                item.Name = "Open";
                item.Icon = "/VCNAudioControls;component/Themes/Images/open.png";
                mListLeftButtons.Add(item);
            }
            if (State.Length > 3 && State.Substring(3, 1) == "1")
            {
                item = new ImageButtonItem();
                item.Name = "Play";
                item.Icon = "/VCNAudioControls;component/Themes/Images/play.png";
                mListLeftButtons.Add(item);
            }
            if (State.Length > 4 && State.Substring(4, 1) == "1")
            {
                item = new ImageButtonItem();
                item.Name = "Pause";
                item.Icon = "/VCNAudioControls;component/Themes/Images/pause.png";
                mListLeftButtons.Add(item);
            }
            if (State.Length > 5 && State.Substring(5, 1) == "1")
            {
                item = new ImageButtonItem();
                item.Name = "Stop";
                item.Icon = "/VCNAudioControls;component/Themes/Images/stop.png";
                mListLeftButtons.Add(item);
            }
            if (State.Length > 6 && State.Substring(6, 1) == "1")
            {
                item = new ImageButtonItem();
                item.Name = "Slower";
                item.Icon = "/VCNAudioControls;component/Themes/Images/slower.png";
                mListLeftButtons.Add(item);
            }
            if (State.Length > 7 && State.Substring(7, 1) == "1")
            {
                item = new ImageButtonItem();
                item.Name = "Faster";
                item.Icon = "/VCNAudioControls;component/Themes/Images/faster.png";
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

        public string Url
        {
            get { return mUrl; }
            set { mUrl = value; }
        }

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
                OnPlayerEventEvent(Defines.EVT_MEDIAOPENED, mWaveStream.TotalTime);

                if (!CreateWaveImage(mWaveStream)) { return; }

                LbTotalTime = GetTimeString(mWaveStream.TotalTime);
                LbCurrentTime = "00:00:00";
                if (mSliderPosition != null)
                {
                    mSliderPosition.Minimum = 0;
                    mSliderPosition.Maximum = mWaveStream.TotalTime.TotalSeconds;
                }

                if (!CreateWaveStream()) { return; }
                if (!CreateWaveOut()) { return; }
                if (mWaveOut != null)
                {
                    mWaveOut.Play();
                    mTimer.Start();

                    OnPlayerEventEvent(Defines.EVT_PLAYBACKSTARTED, mUrl);
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
            }catch{}
        }

        public void SetPosition(TimeSpan ts)
        {
            OptReturn optReturn;
            try
            {
                int intValue = (int) ts.TotalSeconds;
                if (mWaveStream != null)
                {
                    mWaveStream.CurrentTime = TimeSpan.FromSeconds(intValue);
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

        #endregion


        #region Others

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
                    if (mImageLeft != null)
                    {
                        mImageLeft.Source = imgsource;
                    }
                    bitmap = data[1];
                    imgsource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                    if (mImageRight != null)
                    {
                        mImageRight.Source = imgsource;
                    }
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

        private void ShowException(OptReturn optReturn)
        {
            Dispatcher.Invoke(new Action(() => OnPlayerEventEvent(Defines.EVT_EXCEPTION, optReturn)));
        }

        #endregion


        #region ButtonOperation

        private void BtnOpen_Click()
        {
            OptReturn optReturn;
            try
            {
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
            catch (Exception ex)
            {
                optReturn = new OptReturn();
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
                ShowException(optReturn);
            }
        }

        private void BtnPlay_Click()
        {
            OptReturn optReturn;
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
                OnPlayerEventEvent(Defines.EVT_BTN_PLAY, mUrl);
            }
            catch (Exception ex)
            {
                optReturn = new OptReturn();
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
                ShowException(optReturn);
            }
        }

        private void BtnPause_Click()
        {
            OptReturn optReturn;
            try
            {
                if (mWaveOut != null
                              && mWaveStream != null
                              && mWaveOut.PlaybackState == PlaybackState.Playing)
                {
                    mWaveOut.Pause();
                    mTimer.Stop();
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

        private void BtnStop_Click()
        {
            OptReturn optReturn;
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
                    OnPlayerEventEvent(Defines.EVT_BTN_STOP, ts);
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

        private void BtnSlower_Click()
        {
            OptReturn optReturn;
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
                optReturn = new OptReturn();
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
                ShowException(optReturn);
            }
        }

        private void BtnFaster_Click()
        {
            OptReturn optReturn;
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
                optReturn = new OptReturn();
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
                ShowException(optReturn);
            }
        }

        private void BtnChannelMode_Click()
        {
            OptReturn optReturn;
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
                optReturn = new OptReturn();
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
                ShowException(optReturn);
            }
        }

        private void BtnClose_Click()
        {
            OptReturn optReturn;
            try
            {
                OnPlayerEventEvent(Defines.EVT_BTN_CLOSE, null);
                Close();
            }
            catch (Exception ex)
            {
                optReturn = new OptReturn();
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
                ShowException(optReturn);
            }
        }

        #endregion


        #region EventHandlers

        private void mTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (mWaveStream != null)
            {
                TimeSpan ts = mWaveStream.CurrentTime;
                Dispatcher.Invoke(new Action(() =>
                {
                    LbCurrentTime = GetTimeString(ts);
                    if (mWaveStream != null)
                    {
                        if (mSliderPosition != null && !mIsDraging)
                        {
                            mSliderPosition.Value = mWaveStream.CurrentTime.TotalSeconds;
                        }
                        OnPlayerEventEvent(Defines.EVT_PLAYING, ts);
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
            OnPlayerEventEvent(Defines.EVT_PLAYBACKSTOPPED, null);
        }

        void postVolumeMeter_StreamVolume(object sender, StreamVolumeEventArgs e)
        {

        }

        void waveChannel_PreVolumeMeter(object sender, StreamVolumeEventArgs e)
        {

        }

        void mSliderPosition_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (mIsDraging)
            {
                var value = mSliderPosition.Value;
                TimeSpan ts = TimeSpan.FromSeconds((int)value);
                if (mWaveStream != null)
                {
                    mWaveStream.CurrentTime = ts;
                }
                mIsDraging = false;
            }
        }

        #endregion


        #region DependencyProperties

        #region Title

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(CustomAudioPlayer), new PropertyMetadata(default(string)));

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        #endregion


        #region LbRate

        public static readonly DependencyProperty LbRateProperty =
            DependencyProperty.Register("LbRate", typeof(string), typeof(CustomAudioPlayer), new PropertyMetadata(default(string)));

        public string LbRate
        {
            get { return (string)GetValue(LbRateProperty); }
            set { SetValue(LbRateProperty, value); }
        }

        #endregion


        #region LbVolume

        public static readonly DependencyProperty LbVolumeProperty =
            DependencyProperty.Register("LbVolume", typeof(string), typeof(CustomAudioPlayer), new PropertyMetadata(default(string)));

        public string LbVolume
        {
            get { return (string)GetValue(LbVolumeProperty); }
            set { SetValue(LbVolumeProperty, value); }
        }

        #endregion


        #region LbCurrentTime

        public static readonly DependencyProperty LbCurrentTimeProperty =
            DependencyProperty.Register("LbCurrentTime", typeof(string), typeof(CustomAudioPlayer), new PropertyMetadata(default(string)));

        public string LbCurrentTime
        {
            get { return (string)GetValue(LbCurrentTimeProperty); }
            set { SetValue(LbCurrentTimeProperty, value); }
        }

        #endregion


        #region LbTotalTime

        public static readonly DependencyProperty LbTotalTimeProperty =
            DependencyProperty.Register("LbTotalTime", typeof(string), typeof(CustomAudioPlayer), new PropertyMetadata(default(string)));

        public string LbTotalTime
        {
            get { return (string)GetValue(LbTotalTimeProperty); }
            set { SetValue(LbTotalTimeProperty, value); }
        }

        #endregion


        #region IsImageLeftVisible

        public static readonly DependencyProperty IsImageLeftVisibleProperty =
            DependencyProperty.Register("IsImageLeftVisible", typeof (bool), typeof (CustomAudioPlayer), new PropertyMetadata(default(bool)));

        public bool IsImageLeftVisible
        {
            get { return (bool) GetValue(IsImageLeftVisibleProperty); }
            set { SetValue(IsImageLeftVisibleProperty, value); }
        }

        #endregion


        #region IsImageRightVisible

        public static readonly DependencyProperty IsImageRightVisibleProperty =
            DependencyProperty.Register("IsImageRightVisible", typeof (bool), typeof (CustomAudioPlayer), new PropertyMetadata(default(bool)));

        public bool IsImageRightVisible
        {
            get { return (bool) GetValue(IsImageRightVisibleProperty); }
            set { SetValue(IsImageRightVisibleProperty, value); }
        }

        #endregion


        #region IsVolumeVisible

        public static readonly DependencyProperty IsVolumeVisibleProperty =
            DependencyProperty.Register("IsVolumeVisible", typeof (bool), typeof (CustomAudioPlayer), new PropertyMetadata(default(bool)));

        public bool IsVolumeVisible
        {
            get { return (bool) GetValue(IsVolumeVisibleProperty); }
            set { SetValue(IsVolumeVisibleProperty, value); }
        }

        #endregion


        #region Volume

        public static readonly DependencyProperty VolumeProperty =
            DependencyProperty.Register("Volume", typeof(double), typeof(CustomAudioPlayer), new PropertyMetadata(1.0, OnVolumeChanged));

        public double Volume
        {
            get { return (double)GetValue(VolumeProperty); }
            set { SetValue(VolumeProperty, value); }
        }

        private static void OnVolumeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var player = d as CustomAudioPlayer;
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
            DependencyProperty.Register("State", typeof(string), typeof(CustomAudioPlayer), new PropertyMetadata(default(string)));

        public string State
        {
            get { return (string)GetValue(StateProperty); }
            set { SetValue(StateProperty, value); }
        }

        #endregion

        #endregion


        #region 播放器事件

        public static readonly RoutedEvent PlayerEventEvent;

        public event RoutedPropertyChangedEventHandler<PlayerEventArgs> PlayerEvent
        {
            add { AddHandler(PlayerEventEvent, value); }
            remove { RemoveHandler(PlayerEventEvent, value); }
        }

        private void OnPlayerEventEvent(int code, object data)
        {
            if (PlayerEventEvent != null)
            {
                PlayerEventArgs args = new PlayerEventArgs();
                args.Code = code;
                args.Data = data;
                RoutedPropertyChangedEventArgs<PlayerEventArgs> a = new RoutedPropertyChangedEventArgs<PlayerEventArgs>(
                    null, args);
                a.RoutedEvent = PlayerEventEvent;
                RaiseEvent(a);
            }
        }

        #endregion


        #region 按钮命令

        public static RoutedUICommand ButtonCommand = new RoutedUICommand();

        private static void ButtonCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var player = sender as CustomAudioPlayer;
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
                case "Open":
                    BtnOpen_Click();
                    break;
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


        #region Template

        private const string PART_ImageLeft = "PART_ImageLeft";
        private const string PART_ImageRight = "PART_ImageRight";
        private const string PART_Slider = "PART_Slider";
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
            mSliderPosition = GetTemplateChild(PART_Slider) as Slider;
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
