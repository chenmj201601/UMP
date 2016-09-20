//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    ffc6bdcf-0906-42e3-8c43-d7bf89a07c52
//        CLR Version:              4.0.30319.18444
//        Name:                     BandWaiter
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Wpf.CustomControls.Waiter.Implementation
//        File Name:                BandWaiter
//
//        created by Charley at 2014/8/24 23:25:40
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace VoiceCyber.Wpf.CustomControls
{
    public class BandWaiter:Control
    {
        static BandWaiter()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof (BandWaiter),
                new FrameworkPropertyMetadata(typeof (BandWaiter)));
        }

        public BandWaiter()
        {
            Loaded += BandWaiter_Loaded;
        }

        void BandWaiter_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        #region CircleSizeProperty

        public static readonly DependencyProperty CircleSizeProperty =
            DependencyProperty.Register("CircleSize", typeof (int), typeof (BandWaiter), new PropertyMetadata(default(int)));

        public int CircleSize
        {
            get { return (int) GetValue(CircleSizeProperty); }
            set { SetValue(CircleSizeProperty, value); }
        }

        #endregion


        #region StartPoint

        public static readonly DependencyProperty StartPointProperty =
            DependencyProperty.Register("StartPoint", typeof (int), typeof (BandWaiter), new PropertyMetadata(default(int)));

        public int StartPoint
        {
            get { return (int) GetValue(StartPointProperty); }
            set { SetValue(StartPointProperty, value); }
        }

        #endregion


        #region SlowPonitProperty

        public static readonly DependencyProperty SlowPointProperty =
            DependencyProperty.Register("SlowPoint", typeof (int), typeof (BandWaiter), new PropertyMetadata(default(int)));

        public int SlowPoint
        {
            get { return (int) GetValue(SlowPointProperty); }
            set { SetValue(SlowPointProperty, value); }
        }

        #endregion


        #region SlowEndPoint

        public static readonly DependencyProperty SlowEndPointProperty =
            DependencyProperty.Register("SlowEndPoint", typeof (int), typeof (BandWaiter), new PropertyMetadata(default(int)));

        public int SlowEndPoint
        {
            get { return (int) GetValue(SlowEndPointProperty); }
            set { SetValue(SlowEndPointProperty, value); }
        }

        #endregion


        #region EndPoint

        public static readonly DependencyProperty EndPointProperty =
            DependencyProperty.Register("EndPoint", typeof (int), typeof (BandWaiter), new PropertyMetadata(default(int)));

        public int EndPoint
        {
            get { return (int) GetValue(EndPointProperty); }
            set { SetValue(EndPointProperty, value); }
        }

        #endregion


        #region CirclBrushProperty

        public static readonly DependencyProperty CircleBrushProperty =
            DependencyProperty.Register("CircleBrush", typeof (Brush), typeof (BandWaiter), new PropertyMetadata(default(Brush)));

        public Brush CircleBrush
        {
            get { return (Brush) GetValue(CircleBrushProperty); }
            set { SetValue(CircleBrushProperty, value); }
        }

        #endregion

    }
}
