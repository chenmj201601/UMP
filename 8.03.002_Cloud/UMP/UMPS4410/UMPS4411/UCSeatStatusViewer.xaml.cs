//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    601eae9b-95c4-490f-826a-1b634f434c7b
//        CLR Version:              4.0.30319.18408
//        Name:                     UCSeatStatusViewer
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS4411
//        File Name:                UCSeatStatusViewer
//
//        created by Charley at 2016/6/20 16:37:19
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using UMPS4411.Models;

namespace UMPS4411
{
    /// <summary>
    /// UCSeatStatusViewer.xaml 的交互逻辑
    /// </summary>
    public partial class UCSeatStatusViewer
    {

        private bool mIsInited;

        public UCSeatStatusViewer()
        {
            InitializeComponent();

            Loaded += UCSeatStatusViewer_Loaded;
        }

        void UCSeatStatusViewer_Loaded(object sender, RoutedEventArgs e)
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
                if (RegionSeatItem == null) { return; }
                RegionSeatItem.Viewer = this;
                var brush = Brushes.SlateGray;
                if (brush != null)
                {
                    ItemBorderBrush = brush;
                }
                brush = Brushes.SlateGray;
                if (brush != null)
                {
                    HeadBackground = brush;
                }
                ContentBackground = Brushes.Transparent;
                LabelTitle = RegionSeatItem.SeatName;
                IsLogined = false;
                IsCalling = false;
                MatrixTransform transform = new MatrixTransform();
                Matrix mx = new Matrix();
                mx.OffsetX = RegionSeatItem.Left;
                mx.OffsetY = RegionSeatItem.Top;
                transform.Matrix = mx;
                PanelSeatItem.RenderTransform = transform;

                ChangeLanguage();
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }


        #region Operations

        public void SetItemBorderBrush(string strColor)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                if (string.IsNullOrEmpty(strColor))
                {
                    ItemBorderBrush = Brushes.Transparent;
                    return;
                }
                var brush = new SolidColorBrush(GetColorFromString(strColor));
                ItemBorderBrush = brush;
            }));
        }

        public void SetContentBackground(string strColor)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                if (string.IsNullOrEmpty(strColor))
                {
                    ContentBackground = Brushes.Transparent;
                    return;
                }
                var brush = new SolidColorBrush(GetColorFromString(strColor));
                ContentBackground = brush;
            }));
        }

        public void SetHeadBackground(string strColor)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                if (string.IsNullOrEmpty(strColor))
                {
                    HeadBackground = Brushes.Transparent;
                    return;
                }
                var brush = new SolidColorBrush(GetColorFromString(strColor));
                HeadBackground = brush;
            }));
        }

        public void SetLabelAgent(string content)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                LabelAgent = content;
            }));
        }

        public void SetIsLogined(bool isLogined)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                IsLogined = isLogined;
            }));
        }

        public void SetLabelMain(string content)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                LabelMain = content;
            }));
        }

        public void SetLabelSub(string content)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                LabelSub = content;
            }));
        }

        public void SetIsCalling(bool isCalling, int state)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                IsCalling = isCalling;
                if (state == 1)
                {
                    ImageDirection.SetResourceReference(StyleProperty, "SeatViewerImageCallin");
                }
                if (state == 2)
                {
                    ImageDirection.SetResourceReference(StyleProperty, "SeatViewerImageCallout");
                }
            }));
        }

        #endregion


        #region Others

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

        #endregion


        #region RegionSeatItem

        public static readonly DependencyProperty RegionSeatItemProperty =
            DependencyProperty.Register("RegionSeatItem", typeof(RegionSeatItem), typeof(UCSeatStatusViewer), new PropertyMetadata(default(RegionSeatItem)));

        public RegionSeatItem RegionSeatItem
        {
            get { return (RegionSeatItem)GetValue(RegionSeatItemProperty); }
            set { SetValue(RegionSeatItemProperty, value); }
        }

        #endregion


        #region HeadBackground

        public static readonly DependencyProperty HeadBackgroundProperty =
            DependencyProperty.Register("HeadBackground", typeof(Brush), typeof(UCSeatStatusViewer), new PropertyMetadata(default(Brush)));

        public Brush HeadBackground
        {
            get { return (Brush)GetValue(HeadBackgroundProperty); }
            set { SetValue(HeadBackgroundProperty, value); }
        }

        #endregion


        #region ContentBackground

        public static readonly DependencyProperty ContentBackgroundProperty =
            DependencyProperty.Register("ContentBackground", typeof(Brush), typeof(UCSeatStatusViewer), new PropertyMetadata(default(Brush)));

        public Brush ContentBackground
        {
            get { return (Brush)GetValue(ContentBackgroundProperty); }
            set { SetValue(ContentBackgroundProperty, value); }
        }

        #endregion


        #region ItemBorderBrush

        public static readonly DependencyProperty ItemBorderBrushProperty =
            DependencyProperty.Register("ItemBorderBrush", typeof(Brush), typeof(UCSeatStatusViewer), new PropertyMetadata(default(Brush)));

        public Brush ItemBorderBrush
        {
            get { return (Brush)GetValue(ItemBorderBrushProperty); }
            set { SetValue(ItemBorderBrushProperty, value); }
        }

        #endregion


        #region LabelTitle

        public static readonly DependencyProperty LabelTitleProperty =
            DependencyProperty.Register("LabelTitle", typeof(string), typeof(UCSeatStatusViewer), new PropertyMetadata(default(string)));

        public string LabelTitle
        {
            get { return (string)GetValue(LabelTitleProperty); }
            set { SetValue(LabelTitleProperty, value); }
        }

        #endregion


        #region LabelAgent

        public static readonly DependencyProperty LabelAgentProperty =
            DependencyProperty.Register("LabelAgent", typeof(string), typeof(UCSeatStatusViewer), new PropertyMetadata(default(string)));

        public string LabelAgent
        {
            get { return (string)GetValue(LabelAgentProperty); }
            set { SetValue(LabelAgentProperty, value); }
        }

        #endregion


        #region LabelMain

        public static readonly DependencyProperty LabelMainProperty =
            DependencyProperty.Register("LabelMain", typeof(string), typeof(UCSeatStatusViewer), new PropertyMetadata(default(string)));

        public string LabelMain
        {
            get { return (string)GetValue(LabelMainProperty); }
            set { SetValue(LabelMainProperty, value); }
        }

        #endregion


        #region LabelSub

        public static readonly DependencyProperty LabelSubProperty =
            DependencyProperty.Register("LabelSub", typeof(string), typeof(UCSeatStatusViewer), new PropertyMetadata(default(string)));

        public string LabelSub
        {
            get { return (string)GetValue(LabelSubProperty); }
            set { SetValue(LabelSubProperty, value); }
        }

        #endregion


        #region IsLogined

        public static readonly DependencyProperty IsLoginedProperty =
            DependencyProperty.Register("IsLogined", typeof(bool), typeof(UCSeatStatusViewer), new PropertyMetadata(default(bool)));

        public bool IsLogined
        {
            get { return (bool)GetValue(IsLoginedProperty); }
            set { SetValue(IsLoginedProperty, value); }
        }

        #endregion


        #region IsCalling

        public static readonly DependencyProperty IsCallingProperty =
            DependencyProperty.Register("IsCalling", typeof(bool), typeof(UCSeatStatusViewer), new PropertyMetadata(default(bool)));

        public bool IsCalling
        {
            get { return (bool)GetValue(IsCallingProperty); }
            set { SetValue(IsCallingProperty, value); }
        }

        #endregion


        #region IsDetailOpen

        public static readonly DependencyProperty IsDetailOpenProperty =
            DependencyProperty.Register("IsDetailOpen", typeof(bool), typeof(UCSeatStatusViewer), new PropertyMetadata(default(bool)));

        public bool IsDetailOpen
        {
            get { return (bool)GetValue(IsDetailOpenProperty); }
            set { SetValue(IsDetailOpenProperty, value); }
        }

        #endregion


        #region ItemClickCommand

        private static readonly RoutedUICommand mItemClickCommand = new RoutedUICommand();

        public static RoutedUICommand ItemClickCommand
        {
            get { return mItemClickCommand; }
        }

        #endregion


        #region ChangeLanguage



        #endregion

    }
}
