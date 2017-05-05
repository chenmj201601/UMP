//======================================================================
//
//        Copyright © 2016 VoiceCyber Technologies Ltd.
//        All rights reserved
//        guid1:                    4f5ff6a3-92ce-48a1-b64f-16e594f6fec7
//        CLR Version:              4.0.30319.42000
//        Name:                     UCAlarmIconPanel
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             VoiceCyber
//        Namespace:                UMPS4411
//        File Name:                UCAlarmIconPanel
//
//        Created by Charley at 2016/10/26 10:01:16
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using UMPS4411.Models;
using VoiceCyber.UMP.Common;

namespace UMPS4411
{
    /// <summary>
    /// UCAlarmIconPanel.xaml 的交互逻辑
    /// </summary>
    public partial class UCAlarmIconPanel
    {

        #region Members

        private bool mIsInited;

        public StateDurationInfo StateDurationInfo;
        public StateAlarmInfo StateAlarmInfo;

        #endregion


        public UCAlarmIconPanel()
        {
            InitializeComponent();

            Loaded += UCAlarmIconPanel_Loaded;
        }

        void UCAlarmIconPanel_Loaded(object sender, RoutedEventArgs e)
        {
            if (!mIsInited)
            {
                Init();
                mIsInited = true;
            }
        }

        private void Init()
        {
            try
            {
                if (StateDurationInfo == null) { return;}
                if (StateAlarmInfo == null) { return;}
                StateDurationInfo.AlarmIconPanel = this;
                var alarmInfo = StateAlarmInfo.AlarmMessage;
                if (alarmInfo != null)
                {
                    string strBg = alarmInfo.Color;
                    Color color = GetColorFromString(strBg);
                    Brush bg = new SolidColorBrush(color);
                    BorderPanel.Background = bg;

                    string strIcon = alarmInfo.Icon;
                    if (!string.IsNullOrEmpty(strIcon))
                    {
                        string strUrl = string.Format("{0}://{1}:{2}/{3}/UMPS4415/{4}",
                         CurrentApp.Session.AppServerInfo.Protocol,
                         CurrentApp.Session.AppServerInfo.Address,
                         CurrentApp.Session.AppServerInfo.Port,
                         ConstValue.TEMP_DIR_UPLOADFILES,
                         strIcon);
                        BitmapImage image = new BitmapImage();
                        image.BeginInit();
                        image.UriSource = new Uri(strUrl, UriKind.Absolute);
                        image.EndInit();
                        ImageIcon.Source = image;

                        Image imgTip = new Image();
                        imgTip.Source = new BitmapImage(new Uri(strUrl, UriKind.Absolute));
                        imgTip.Stretch = Stretch.Uniform;
                        ImageIcon.ToolTip = imgTip;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private Color GetColorFromString(string strColor)
        {
            Color color = Brushes.Transparent.Color;
            try
            {
                string strA = strColor.Substring(1, 2);
                string strR = strColor.Substring(3, 2);
                string strG = strColor.Substring(5, 2);
                string strB = strColor.Substring(7, 2);
                color = Color.FromArgb((byte)Convert.ToInt32(strA, 16), (byte)Convert.ToInt32(strR, 16), (byte)Convert.ToInt32(strG, 16),
                    (byte)Convert.ToInt32(strB, 16));
            }
            catch { }
            return color;
        }
    }
}
