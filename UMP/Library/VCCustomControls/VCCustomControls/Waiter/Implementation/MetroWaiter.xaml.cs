//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    d2c2f95c-6621-4948-90d5-7a3aa0edd73a
//        CLR Version:              4.0.30319.18444
//        Name:                     MetroWaiter
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Wpf.CustomControls.Waiter.Implementation
//        File Name:                MetroWaiter
//
//        created by Charley at 2014/10/29 14:28:26
//        http://www.voicecyber.com
//
//======================================================================

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace VoiceCyber.Wpf.CustomControls
{
    /// <summary>
    /// MetroWaiter.xaml 的交互逻辑
    /// </summary>
    public partial class MetroWaiter : INotifyPropertyChanged
    {
        #region 属性
        private double mLeftFrom;
        /// <summary>
        /// 左边第一个起点
        /// </summary>
        public double LeftFrom
        {
            get { return mLeftFrom; }
            set
            {
                mLeftFrom = value;
                if (PropertyChanged != null)
                {
                    NotifyPropertyChanged("LeftFrom");
                }
            }
        }

        private double mLeftTo;
        /// <summary>
        /// 第一个终点
        /// </summary>
        public double LeftTo
        {
            get
            {
                return mLeftTo;
            }
            set
            {
                mLeftTo = value;
                if (PropertyChanged != null)
                {
                    NotifyPropertyChanged("LeftTo");
                }
            }
        }

        private double mSlowFrom;
        /// <summary>
        /// 缓动起点
        /// </summary>
        public double SlowFrom
        {
            get
            {
                return mSlowFrom;
            }
            set
            {
                mSlowFrom = value;
                if (PropertyChanged != null)
                {
                    NotifyPropertyChanged("SlowFrom");
                }
            }
        }

        private double mSlowTo;
        /// <summary>
        /// 缓动终点
        /// </summary>
        public double SlowTo
        {
            get
            {
                return mSlowTo;
            }
            set
            {
                mSlowTo = value;
                if (PropertyChanged != null)
                {
                    NotifyPropertyChanged("SlowTo");
                }
            }
        }

        private double mRightFrom;
        /// <summary>
        /// 右边起点
        /// </summary>
        public double RightFrom
        {
            get
            {
                return mRightFrom;
            }
            set
            {
                mRightFrom = value;
                if (PropertyChanged != null)
                {
                    NotifyPropertyChanged("RightFrom");
                }
            }
        }

        private double mRightTo;
        /// <summary>
        /// 右边终点
        /// </summary>
        public double RightTo
        {
            get
            {
                return mRightTo;
            }
            set
            {
                mRightTo = value;
                if (PropertyChanged != null)
                {
                    NotifyPropertyChanged("RightTo");
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ViewColorProperty =
            DependencyProperty.Register("ViewColor", typeof(Brush), typeof(MetroWaiter), new PropertyMetadata(default(Brush)));
        /// <summary>
        /// 
        /// </summary>
        public Brush ViewColor
        {
            get { return (Brush)GetValue(ViewColorProperty); }
            set { SetValue(ViewColorProperty, value); }
        }
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ViewWidthHeightProperty =
            DependencyProperty.Register("ViewWidthHeight", typeof(double), typeof(MetroWaiter), new PropertyMetadata(default(double)));
        /// <summary>
        /// 
        /// </summary>
        public double ViewWidthHeight
        {
            get { return (double)GetValue(ViewWidthHeightProperty); }
            set { SetValue(ViewWidthHeightProperty, value); }
        }

        #endregion
        /// <summary>
        /// 
        /// </summary>
        public MetroWaiter()
        {
            InitializeComponent();

            Loaded += MetroWaiter_Loaded;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="color"></param>
        /// <param name="widthHeight"></param>
        public MetroWaiter(Brush color, double widthHeight)
            : this()
        {
            ViewColor = color;
            ViewWidthHeight = widthHeight;
        }

        void MetroWaiter_Loaded(object sender, RoutedEventArgs e)
        {
            double width;

            try
            {
                width = ActualWidth;
                if (double.IsNaN(width))
                {
                    width = 600;
                }
                LeftFrom = 0;
                LeftTo = width / 2 - (width / 7) / 2;
                SlowFrom = LeftTo;
                SlowTo = LeftTo + (width / 7);
                RightFrom = SlowTo;
                RightTo = width;

                SizeChanged += WaitProgressBar_SizeChanged;

                StartAnimation();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        void MetroWaiter_Initialized(object sender, EventArgs e)
        {
            try
            {
                DataContext = this;

                ViewColor = new SolidColorBrush(Colors.WhiteSmoke);
                ViewWidthHeight = 8.0;

                Ellipse00.Opacity = 0;
                Ellipse01.Opacity = 0;
                Ellipse02.Opacity = 0;
                Ellipse03.Opacity = 0;
                Ellipse04.Opacity = 0;

                var left00 = Ellipse00.FindResource("StoryboardLeft00") as Storyboard;
                var slow00 = Ellipse00.FindResource("StoryboardSlow00") as Storyboard;
                var right00 = Ellipse00.FindResource("StoryboardRight00") as Storyboard;

                var left01 = Ellipse01.FindResource("StoryboardLeft01") as Storyboard;
                var slow01 = Ellipse01.FindResource("StoryboardSlow01") as Storyboard;
                var right01 = Ellipse01.FindResource("StoryboardRight01") as Storyboard;

                var left02 = Ellipse02.FindResource("StoryboardLeft02") as Storyboard;
                var slow02 = Ellipse02.FindResource("StoryboardSlow02") as Storyboard;
                var right02 = Ellipse02.FindResource("StoryboardRight02") as Storyboard;

                var left03 = Ellipse03.FindResource("StoryboardLeft03") as Storyboard;
                var slow03 = Ellipse03.FindResource("StoryboardSlow03") as Storyboard;
                var right03 = Ellipse03.FindResource("StoryboardRight03") as Storyboard;

                var left04 = Ellipse04.FindResource("StoryboardLeft04") as Storyboard;
                var slow04 = Ellipse04.FindResource("StoryboardSlow04") as Storyboard;
                var right04 = Ellipse04.FindResource("StoryboardRight04") as Storyboard;

                //第一个点第一个动画结束后开启缓动，第二个点启动
                left00.Completed += (a, b) =>
                {
                    slow00.Begin();
                    Ellipse01.Opacity = 1;
                    left01.Begin();
                };
                //第一个点缓动结束，右边动画启动
                slow00.Completed += (a, b) => right00.Begin();
                right00.Completed += (a, b) => Ellipse00.Opacity = 0;

                //以下类推
                left01.Completed += (a, b) =>
                {
                    slow01.Begin();
                    Ellipse02.Opacity = 1;
                    left02.Begin();
                };
                slow01.Completed += (a, b) => right01.Begin();
                right01.Completed += (a, b) => Ellipse01.Opacity = 0;

                left02.Completed += (a, b) =>
                {
                    slow02.Begin();
                    Ellipse03.Opacity = 1;
                    left03.Begin();
                };
                slow02.Completed += (a, b) => right02.Begin();
                right02.Completed += (a, b) => Ellipse02.Opacity = 0;

                left03.Completed += (a, b) =>
                {
                    slow03.Begin();
                    Ellipse04.Opacity = 1;
                    left04.Begin();
                };
                slow03.Completed += (a, b) => right03.Begin();
                right03.Completed += (a, b) => Ellipse03.Opacity = 0;


                left04.Completed += (a, b) => slow04.Begin();
                slow04.Completed += (a, b) => right04.Begin();
                //最后一个点动画结束，第一个点重启 如此循环
                right04.Completed += (a, b) =>
                {
                    Ellipse04.Opacity = 0;
                    Ellipse00.Opacity = 1;
                    left00.Begin();
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public void StartAnimation()
        {
            try
            {
                var left00 = Ellipse00.FindResource("StoryboardLeft00") as Storyboard;
                Ellipse00.Opacity = 1;
                if (left00 != null) { left00.Begin(); }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void WaitProgressBar_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double width;
            try
            {
                width = ActualWidth;
                LeftFrom = 0;
                LeftTo = width / 2 - (width / 7) / 2;
                SlowFrom = LeftTo;
                SlowTo = LeftTo + (width / 7);
                RightFrom = SlowTo;
                RightTo = width;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
