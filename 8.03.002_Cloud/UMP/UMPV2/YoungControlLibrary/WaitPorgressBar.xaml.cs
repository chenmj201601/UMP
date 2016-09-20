using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace YoungControlLibrary
{
    public partial class WaitPorgressBar : UserControl, INotifyPropertyChanged
    {
        #region 属性
        private double _leftFrom;
        /// <summary>
        /// 左边第一个起点
        /// </summary>
        public double LeftFrom
        {
            get { return _leftFrom; }
            set
            {
                _leftFrom = value;
                if (this.PropertyChanged != null)
                {
                    NotifyPropertyChanged("LeftFrom");
                }
            }
        }

        private double _leftTo;
        /// <summary>
        /// 第一个终点
        /// </summary>
        public double LeftTo
        {
            get
            {
                return _leftTo;
            }
            set
            {
                _leftTo = value;
                if (this.PropertyChanged != null)
                {
                    NotifyPropertyChanged("LeftTo");
                }
            }
        }

        private double _slowFrom;
        /// <summary>
        /// 缓动起点
        /// </summary>
        public double SlowFrom
        {
            get
            {
                return _slowFrom;
            }
            set
            {
                _slowFrom = value;
                if (this.PropertyChanged != null)
                {
                    NotifyPropertyChanged("SlowFrom");
                }
            }
        }

        private double _slowTo;
        /// <summary>
        /// 缓动终点
        /// </summary>
        public double SlowTo
        {
            get
            {
                return _slowTo;
            }
            set
            {
                _slowTo = value;
                if (this.PropertyChanged != null)
                {
                    NotifyPropertyChanged("SlowTo");
                }
            }
        }

        private double _rightFrom;
        /// <summary>
        /// 右边起点
        /// </summary>
        public double RightFrom
        {
            get
            {
                return _rightFrom;
            }
            set
            {
                _rightFrom = value;
                if (this.PropertyChanged != null)
                {
                    NotifyPropertyChanged("RightFrom");
                }
            }
        }

        private double _rightTo;
        /// <summary>
        /// 右边终点
        /// </summary>
        public double RightTo
        {
            get
            {
                return _rightTo;
            }
            set
            {
                _rightTo = value;
                if (this.PropertyChanged != null)
                {
                    NotifyPropertyChanged("RightTo");
                }
            }
        }

        /// <summary>
        /// 圆点显示的颜色
        /// </summary>
        public SolidColorBrush ViewColor { get; set; }

        public double ViewWidthHeight { get; set; }

        #endregion

        public WaitPorgressBar()
        {
            InitializeComponent();
        }

        public WaitPorgressBar(SolidColorBrush AColorBrush, double ADoubleWidthHeight)
        {
            InitializeComponent();
            ViewColor = AColorBrush;
            ViewWidthHeight = ADoubleWidthHeight;
        }

        public void StartAnimation()
        {
            try
            {
                var LVarStoryboardLeft00 = this.Ellipse00.FindResource("StoryboardLeft00") as Storyboard;
                this.Ellipse00.Opacity = 1;
                if (LVarStoryboardLeft00 != null) { LVarStoryboardLeft00.Begin(); }
            }
            catch { }
        }

        private void UserControl_Initialized(object sender, EventArgs e)
        {
            try
            {
                this.DataContext = this;

                ViewColor = new SolidColorBrush(Colors.WhiteSmoke);
                ViewWidthHeight = 6.0;

                this.Ellipse00.Opacity = 0;
                this.Ellipse01.Opacity = 0;
                this.Ellipse02.Opacity = 0;
                this.Ellipse03.Opacity = 0;
                this.Ellipse04.Opacity = 0;

                var LVarStoryboardLeft00 = this.Ellipse00.FindResource("StoryboardLeft00") as Storyboard;
                var LVarStoryboardSlow00 = this.Ellipse00.FindResource("StoryboardSlow00") as Storyboard;
                var LVarStoryboardRight00 = this.Ellipse00.FindResource("StoryboardRight00") as Storyboard;

                var LVarStoryboardLeft01 = this.Ellipse01.FindResource("StoryboardLeft01") as Storyboard;
                var LVarStoryboardSlow01 = this.Ellipse01.FindResource("StoryboardSlow01") as Storyboard;
                var LVarStoryboardRight01 = this.Ellipse01.FindResource("StoryboardRight01") as Storyboard;

                var LVarStoryboardLeft02 = this.Ellipse02.FindResource("StoryboardLeft02") as Storyboard;
                var LVarStoryboardSlow02 = this.Ellipse02.FindResource("StoryboardSlow02") as Storyboard;
                var LVarStoryboardRight02 = this.Ellipse02.FindResource("StoryboardRight02") as Storyboard;

                var LVarStoryboardLeft03 = this.Ellipse03.FindResource("StoryboardLeft03") as Storyboard;
                var LVarStoryboardSlow03 = this.Ellipse03.FindResource("StoryboardSlow03") as Storyboard;
                var LVarStoryboardRight03 = this.Ellipse03.FindResource("StoryboardRight03") as Storyboard;

                var LVarStoryboardLeft04 = this.Ellipse04.FindResource("StoryboardLeft04") as Storyboard;
                var LVarStoryboardSlow04 = this.Ellipse04.FindResource("StoryboardSlow04") as Storyboard;
                var LVarStoryboardRight04 = this.Ellipse04.FindResource("StoryboardRight04") as Storyboard;

                //第一个点第一个动画结束后开启缓动，第二个点启动
                LVarStoryboardLeft00.Completed += (a, b) =>
                {
                    LVarStoryboardSlow00.Begin();
                    Ellipse01.Opacity = 1;
                    LVarStoryboardLeft01.Begin();
                };
                //第一个点缓动结束，右边动画启动
                LVarStoryboardSlow00.Completed += (a, b) => LVarStoryboardRight00.Begin();
                LVarStoryboardRight00.Completed += (a, b) => Ellipse00.Opacity = 0;

                //以下类推
                LVarStoryboardLeft01.Completed += (a, b) =>
                {
                    LVarStoryboardSlow01.Begin();
                    Ellipse02.Opacity = 1;
                    LVarStoryboardLeft02.Begin();
                };
                LVarStoryboardSlow01.Completed += (a, b) => LVarStoryboardRight01.Begin();
                LVarStoryboardRight01.Completed += (a, b) => Ellipse01.Opacity = 0;

                LVarStoryboardLeft02.Completed += (a, b) =>
                {
                    LVarStoryboardSlow02.Begin();
                    Ellipse03.Opacity = 1;
                    LVarStoryboardLeft03.Begin();
                };
                LVarStoryboardSlow02.Completed += (a, b) => LVarStoryboardRight02.Begin();
                LVarStoryboardRight02.Completed += (a, b) => Ellipse02.Opacity = 0;

                LVarStoryboardLeft03.Completed += (a, b) =>
                {
                    LVarStoryboardSlow03.Begin();
                    Ellipse04.Opacity = 1;
                    LVarStoryboardLeft04.Begin();
                };
                LVarStoryboardSlow03.Completed += (a, b) => LVarStoryboardRight03.Begin();
                LVarStoryboardRight03.Completed += (a, b) => Ellipse03.Opacity = 0;


                LVarStoryboardLeft04.Completed += (a, b) => LVarStoryboardSlow04.Begin();
                LVarStoryboardSlow04.Completed += (a, b) => LVarStoryboardRight04.Begin();
                //最后一个点动画结束，第一个点重启 如此循环
                LVarStoryboardRight04.Completed += (a, b) =>
                {
                    Ellipse04.Opacity = 0;
                    Ellipse00.Opacity = 1;
                    LVarStoryboardLeft00.Begin();
                };
            }
            catch { }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            double LDoubleWidth = 0.0;

            try
            {
                LDoubleWidth = this.ActualWidth;
                if (double.IsNaN(LDoubleWidth)) { LDoubleWidth = 600; }
                LeftFrom = 0;
                LeftTo = LDoubleWidth / 2 - (LDoubleWidth / 7) / 2;
                SlowFrom = LeftTo;
                SlowTo = LeftTo + (LDoubleWidth / 7);
                RightFrom = SlowTo;
                RightTo = LDoubleWidth;

                this.SizeChanged += WaitProgressBar_SizeChanged;
            }
            catch { }
        }

        private void WaitProgressBar_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double LDoubleWidth = 0.0;

            try
            {
                LDoubleWidth = this.ActualWidth;
                LeftFrom = 0;
                LeftTo = LDoubleWidth / 2 - (LDoubleWidth / 7) / 2;
                SlowFrom = LeftTo;
                SlowTo = LeftTo + (LDoubleWidth / 7);
                RightFrom = SlowTo;
                RightTo = LDoubleWidth;


            }
            catch { }
        }

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
