using System;
using System.ComponentModel;
using System.Windows;
using VoiceCyber.Common;
using VoiceCyber.UMP.Controls;
using VoiceCyber.UMP.Controls.Players;


namespace UMPS3603
{
    /// <summary>
    /// UCPlayBox.xaml 的交互逻辑
    /// </summary>
    public partial class UCPlayBox
    {
        /// <summary>
        /// 播放结束事件
        /// </summary>
        public event Action PlayStopped;

        public event RoutedPropertyChangedEventHandler<UMPEventArgs> PlayerEvent;

        //public BrowseQustions ParentPage;
        public TrueOrFlasePage ToFParentPage;
        public SingleChoicePage ScParentPage;
        public MultipleChoicePage McParentPage;
        public string PlayUrl;

        private BackgroundWorker mWorker;

        private TimeSpan mCurrentTime;
        public bool IsAutoPlay;
        public double StartPosition;
        public double StopPostion;

        /// <summary>
        /// 0       不循环
        /// 1       单循环
        /// 2       列表循环
        /// </summary>
        public int CircleMode;


        public UCPlayBox()
        {
            InitializeComponent();
            VoicePlayer.PlayerEvent += VoicePlayer_PlayerEvent;
            Loaded += UCPlayBox_Loaded;
            Unloaded += UCPlayBox_Unloaded;

            IsAutoPlay = false;
            CircleMode = 0;
        }

        private void VoicePlayer_PlayerEvent(object sender,
            RoutedPropertyChangedEventArgs<VoiceCyber.UMP.Controls.UMPEventArgs> e)
        {
            try
            {
                OnPlayerEvent(sender, e);
                if (e.NewValue == null)
                {
                    return;
                }
                int code = e.NewValue.Code;
                var param = e.NewValue.Data;

                TimeSpan ts;
                switch (code)
                {
                    case MediaPlayerEventCodes.BTN_PLAY:
                        VoicePlayer.Play();
                        break;
                    case Defines.EVT_PAGE_LOADED:

                        break;
                    case MediaPlayerEventCodes.PLAYING:
                        ts = TimeSpan.Parse(param.ToString());
                        mCurrentTime = ts;
                        if (ts.TotalMilliseconds < StartPosition - 500)
                        {
                            if (CircleMode != 0) //当选择循环的时候,那么就会设置播放的开始位置
                            {
                                VoicePlayer.SetPosition(TimeSpan.FromMilliseconds(StartPosition + 500));
                            }
                        }
                        if (ts.TotalMilliseconds > StopPostion - 500)
                        {
                            if (CircleMode != 0)
                            {
                                OnPlayStopped();
                            }
                            if (CircleMode == 1)
                            {
                                VoicePlayer.SetPosition(TimeSpan.FromMilliseconds(StartPosition));
                            }
                        }
                        break;
                    case MediaPlayerEventCodes.PLAYBACKSTOPPED:
                        if (CircleMode != 0)
                        {
                            OnPlayStopped();
                        }
                        break;
                    case MediaPlayerEventCodes.BTN_STOP:
                        OnPlayStopped();
                        break;
                    case Defines.EVT_EXCEPTION:
                        CurrentApp.WriteLog("MediaPlayer", string.Format("MediaPlayer exception.\t{0}", param));
                        break;
                    case Defines.EVT_COMMON:
                        //CurrentApp.WriteLog("MediaPlayer", string.Format("MediaPlayer debug.\t{0}", param));
                        break;
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }


        private void UCPlayBox_Loaded(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(PlayUrl))
            {
                VoicePlayer.MediaType = 1;
                VoicePlayer.AudioUrl = PlayUrl;

            }
        }

        private void UCPlayBox_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (VoicePlayer != null)
                {
                    VoicePlayer.Close();
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// 获得当前播放的位置
        /// </summary>
        /// <returns></returns>

        public void Stop()
        {
            try
            {
                VoicePlayer.Close();
            }
            catch
            {
            }
        }


        /// <summary>
        /// 关闭
        /// </summary>
        public void Close()
        {
            try
            {
                if (VoicePlayer != null)
                {
                    VoicePlayer.Close();
                }
            }
            catch
            {
            }
        }

        private void OnPlayStopped()
        {
            if (PlayStopped != null)
            {
                PlayStopped();
            }
        }

        private void OnPlayerEvent(object sender, RoutedPropertyChangedEventArgs<UMPEventArgs> e)
        {
            if (PlayerEvent != null)
            {
                PlayerEvent(sender, e);
            }
        }
    }
}
