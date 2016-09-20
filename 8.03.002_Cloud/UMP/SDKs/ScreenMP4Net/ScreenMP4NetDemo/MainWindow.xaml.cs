using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using VoiceCyber.Common;
using VoiceCyber.SDKs.ScreenMP;
using VoiceCyber.SDKs.Windows;

namespace ScreenMP4NetDemo
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow
    {

        private int mScale;
        private double mTotalLength;
        private double mPlayedLength;
        private IntPtr mHwndPlayer;

        public MainWindow()
        {
            InitializeComponent();

            Loaded += MainWindow_Loaded;
            BtnTest.Click += BtnTest_Click;
            BtnPlay.Click += BtnPlay_Click;
            BtnPause.Click += BtnPause_Click;
            BtnStop.Click += BtnStop_Click;
            BtnMonStart.Click += BtnMonStart_Click;
            BtnMonStop.Click += BtnMonStop_Click;

            mHwndPlayer = IntPtr.Zero;
            mScale = 100;
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            SliderPosition.AddHandler(MouseLeftButtonUpEvent, new MouseButtonEventHandler(SliderPosition_MouseLeftButtonUp), true);
            SliderScale.AddHandler(MouseLeftButtonUpEvent, new MouseButtonEventHandler(SliderScale_MouseLeftButtonUp), true);
        }


        #region EventHandler

        void BtnTest_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ////播放窗口最大化
                // if (mHwndPlayer != IntPtr.Zero)
                // {
                //     int intReturn = User32Interop.PostMessage(mHwndPlayer, ScreenMPDefines.WM_SYS_COMMAND,
                //         ScreenMPDefines.WM_SC_MAXIMIZE, 0);
                //     AppendMessage(string.Format("WndMaxReturn:{0}", intReturn));
                // }


            }
            catch (Exception ex)
            {
                AppendMessage(ex.Message);
            }
        }

        void BtnPlay_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                mPlayedLength = 0;
                mTotalLength = 60;
                SliderPosition.Maximum = 60;
                SliderPosition.Minimum = 0;
                SliderPosition.Value = 0;
                int intTotalLength = 0;

                //获取本窗口的句柄
                int handle = 0;
                HwndSource hwndSource = HwndSource.FromVisual(this) as HwndSource;
                if (hwndSource != null)
                {
                    hwndSource.AddHook(WndProc);
                    handle = hwndSource.Handle.ToInt32();
                }

                //string file = string.Format(@"E:\20130819\00\001\ASCHN000000000002000120150609044520744.vls");
                string file = string.Format(@"test.vls");
                int intReturn;
                bool boolReturn;
                //设置反馈进度的频率
                intReturn = ScreenMPInterop.VLSMonSetWaterMark(1);
                AppendMessage(string.Format("SetWaterMarkReturn:{0}", intReturn));
                //播放录屏数据
                intReturn = ScreenMPInterop.VLSMonPlay(file, null, handle);
                AppendMessage(string.Format("PlayReturn:{0}", intReturn));
                if (intReturn == 0)
                {
                    //获取播放窗口句柄
                    IntPtr ptr = ScreenMPInterop.VLSMonGetPlayerHwnd();
                    AppendMessage(string.Format("IntPtr:{0}", ptr));
                    mHwndPlayer = ptr;

                    //窗口置顶
                    if (mHwndPlayer != IntPtr.Zero)
                    {
                        intReturn = User32Interop.SetWindowPos(mHwndPlayer, -1, 0, 0, 0, 0, 3);
                        AppendMessage(string.Format("SetWinPosReturn:{0}", intReturn));
                    }

                    //设置标题
                    if (mHwndPlayer != IntPtr.Zero)
                    {
                        boolReturn = User32Interop.SetWindowText(mHwndPlayer, file);
                        AppendMessage(string.Format("SetWinTxtReturn:{0}", boolReturn));
                    }
                }

                //获取媒体文件总时长
                intReturn = ScreenMPInterop.VLSMonGetFileTotalTime(file, ref intTotalLength, false);
                AppendMessage(string.Format("GetTotalTimeReturn:{0}", intReturn));
                if (intReturn == 0)
                {
                    mTotalLength = intTotalLength;

                    SliderPosition.Maximum = mTotalLength;
                    string str = string.Format("{0}/{1}", Converter.Second2Time(mPlayedLength),
                      Converter.Second2Time(mTotalLength));

                    ShowStateMessage(str);
                }
            }
            catch (Exception ex)
            {
                AppendMessage(ex.Message);
            }
        }

        void BtnPause_Click(object sender, RoutedEventArgs e)
        {

        }

        void BtnStop_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //关闭播放器
                int intValue = ScreenMPInterop.VLSMonCloseWnd();
                AppendMessage(string.Format("CloseWndReturn:{0}", intValue));
                mHwndPlayer = IntPtr.Zero;
            }
            catch (Exception ex)
            {
                AppendMessage(ex.Message);
            }
        }

        void BtnMonStop_Click(object sender, RoutedEventArgs e)
        {

        }

        void BtnMonStart_Click(object sender, RoutedEventArgs e)
        {
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

                SRCMON_PARAM monParam = new SRCMON_PARAM();
                monParam.sVocIp = Converter.String2ByteArray("192.168.6.7", ScreenMPDefines.SIZE_IPADDRESS);
                monParam.nPort = 3010;
                monParam.nChannel = 0;
                intReturn = ScreenMPInterop.VLSMonStart(ref monParam, null, handle);
                AppendMessage(string.Format("MonStartReturn:{0}", intReturn));
            }
            catch (Exception ex)
            {
                AppendMessage(string.Format("Fail.\t{0}", ex.Message));
            }
        }

        void SliderScale_MouseLeftButtonUp(object send, MouseButtonEventArgs e)
        {
            try
            {
                var scale = SliderScale.Value;
                if (scale > 0)
                {
                    mScale = (int)scale;

                    //设置缩放
                    int intValue = ScreenMPInterop.VLSMonSetScale(mScale);
                    AppendMessage(string.Format("SetScaleReturn:{0}", intValue));
                }
            }
            catch (Exception ex)
            {
                AppendMessage(ex.Message);
            }
        }

        void SliderPosition_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var pos = SliderPosition.Value;
                int intValue = (int)pos;

                mPlayedLength = intValue;

                //设置播放位置
                intValue = ScreenMPInterop.VLSMonSetPlayPos(intValue);
                AppendMessage(string.Format("SetPlayPosReturn:{0}", intValue));
            }
            catch (Exception ex)
            {
                AppendMessage(ex.Message);
            }
        }

        #endregion


        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case ScreenMPDefines.WM_WATERMARK:
                    AppendMessage(string.Format("Hwnd:{0}\tMsg:0x{1:X}\tWParam:{2}\tLParam:{3}",
                       hwnd, msg,
                       wParam,
                       lParam));
                    handled = true;
                    try
                    {
                        var pos = wParam.ToInt32();
                        mPlayedLength = pos;
                        Dispatcher.Invoke(new Action(() =>
                        {
                            SliderPosition.Value = mPlayedLength;
                        }));
                        string str = string.Format("{0}/{1}", Converter.Second2Time(mPlayedLength),
                            Converter.Second2Time(mTotalLength));

                        ShowStateMessage(str);
                    }
                    catch (Exception ex)
                    {
                        AppendMessage(string.Format("Fail.\t{0}", ex.Message));
                    }
                    break;
                case ScreenMPDefines.WM_CONNECTION_LOST:
                case ScreenMPDefines.WM_UNKNOWN_ERROR:
                case ScreenMPDefines.WM_PLAY_COMPLETED:
                case ScreenMPDefines.WM_MONIT_START:
                case ScreenMPDefines.WM_WND_CLOSE:
                case ScreenMPDefines.VM_PLAY_OVER:
                    AppendMessage(string.Format("Hwnd:{0}\tMsg:0x{1:X}\tWParam:{2}\tLParam:{3}",
                        hwnd, msg,
                        wParam,
                        lParam));
                    handled = true;
                    break;
            }
            return IntPtr.Zero;
        }


        #region Others

        private void ShowStateMessage(string str)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                TxtPlayState.Text = str;
            }));
        }

        private void AppendMessage(string msg)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                TxtMsg.AppendText(string.Format("{0}\t{1}\r\n", DateTime.Now.ToString("HH:mm:ss"), msg));
                TxtMsg.ScrollToEnd();
            }));
        }

        #endregion




    }
}
