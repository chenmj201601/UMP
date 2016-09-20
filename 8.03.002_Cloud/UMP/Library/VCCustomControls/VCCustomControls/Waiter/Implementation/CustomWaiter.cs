//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    715cd69a-6edb-44ad-ac39-cb928d59645c
//        CLR Version:              4.0.30319.18063
//        Name:                     CustomWaiter
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Wpf.CustomControls.Waiter.Implementation
//        File Name:                CustomWaiter
//
//        created by Charley at 2015/5/6 14:36:52
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace VoiceCyber.Wpf.CustomControls
{
    /// <summary>
    /// 自定义的等待按钮
    /// </summary>
    public class CustomWaiter : Control
    {
        static CustomWaiter()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CustomWaiter),
                new FrameworkPropertyMetadata(typeof(CustomWaiter)));
        }

        /// <summary>
        /// 
        /// </summary>
        public CustomWaiter()
        {
            Loaded += CustomWaiter_Loaded;
            SizeChanged += CustomWaiter_SizeChanged;
        }

        void CustomWaiter_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            PlayAnimation();
        }

        void CustomWaiter_Loaded(object sender, RoutedEventArgs e)
        {
            PlayAnimation();
        }


        #region ViewColor
        /// <summary>
        /// 圆点的颜色
        /// </summary>
        public static readonly DependencyProperty ViewColorProperty =
            DependencyProperty.Register("ViewColor", typeof(Brush), typeof(CustomWaiter), new PropertyMetadata(default(Brush)));

        /// <summary>
        /// 圆点的颜色
        /// </summary>
        public Brush ViewColor
        {
            get { return (Brush)GetValue(ViewColorProperty); }
            set { SetValue(ViewColorProperty, value); }
        }

        #endregion


        #region PointSize
        /// <summary>
        /// 圆点的尺寸
        /// </summary>
        public static readonly DependencyProperty PointSizeProperty =
            DependencyProperty.Register("PointSize", typeof(double), typeof(CustomWaiter), new PropertyMetadata(5.0));
        /// <summary>
        /// 圆点的尺寸
        /// </summary>
        public double PointSize
        {
            get { return (double)GetValue(PointSizeProperty); }
            set { SetValue(PointSizeProperty, value); }
        }

        #endregion


        #region Speed

        /// <summary>
        /// 动画持续时长
        /// </summary>
        public static readonly DependencyProperty DurationProperty =
            DependencyProperty.Register("Duration", typeof(double), typeof(CustomWaiter), new PropertyMetadata(2.0));
        /// <summary>
        /// 动画持续时长
        /// </summary>
        public double Duration
        {
            get { return (double)GetValue(DurationProperty); }
            set { SetValue(DurationProperty, value); }
        }

        #endregion


        #region Animation

        private void PlayAnimation()
        {

            double offset = ActualWidth / 3.0;

            double duration = 2.0;
            if (Duration > 0)
            {
                duration = Duration;
            }
            DoubleAnimationUsingKeyFrames ani = new DoubleAnimationUsingKeyFrames();
            ani.Duration = TimeSpan.FromSeconds(duration);
            ani.RepeatBehavior = RepeatBehavior.Forever;
            LinearDoubleKeyFrame key = new LinearDoubleKeyFrame();
            key.KeyTime = TimeSpan.FromSeconds(0);
            key.Value = 0;
            ani.KeyFrames.Add(key);
            key = new LinearDoubleKeyFrame();
            key.KeyTime = TimeSpan.FromSeconds(duration * 0.2);
            key.Value = offset;
            ani.KeyFrames.Add(key);
            key = new LinearDoubleKeyFrame();
            key.KeyTime = TimeSpan.FromSeconds(duration * 0.8);
            key.Value = offset * 2;
            ani.KeyFrames.Add(key);
            key = new LinearDoubleKeyFrame();
            key.KeyTime = TimeSpan.FromSeconds(duration);
            key.Value = offset * 3;
            ani.KeyFrames.Add(key);

            if (mBorderPoint != null)
            {
                Storyboard storyBoard = new Storyboard();
                storyBoard.BeginTime = TimeSpan.FromSeconds(0);
                Storyboard.SetTarget(ani, mBorderPoint);
                Storyboard.SetTargetProperty(ani, new PropertyPath("(Canvas.Left)"));
                storyBoard.Children.Add(ani);
                storyBoard.Begin();
            }

            if (mBorderPoint1 != null)
            {
                Storyboard storyBoard = new Storyboard();
                storyBoard.BeginTime = TimeSpan.FromSeconds(0.1);
                Storyboard.SetTarget(ani, mBorderPoint1);
                Storyboard.SetTargetProperty(ani, new PropertyPath("(Canvas.Left)"));
                storyBoard.Children.Add(ani);
                storyBoard.Begin();
            }

            if (mBorderPoint2 != null)
            {
                Storyboard storyBoard = new Storyboard();
                storyBoard.BeginTime = TimeSpan.FromSeconds(0.2);
                Storyboard.SetTarget(ani, mBorderPoint2);
                Storyboard.SetTargetProperty(ani, new PropertyPath("(Canvas.Left)"));
                storyBoard.Children.Add(ani);
                storyBoard.Begin();
            }

            if (mBorderPoint3 != null)
            {
                Storyboard storyBoard = new Storyboard();
                storyBoard.BeginTime = TimeSpan.FromSeconds(0.3);
                Storyboard.SetTarget(ani, mBorderPoint3);
                Storyboard.SetTargetProperty(ani, new PropertyPath("(Canvas.Left)"));
                storyBoard.Children.Add(ani);
                storyBoard.Begin();
            }

            if (mBorderPoint4 != null)
            {
                Storyboard storyBoard = new Storyboard();
                storyBoard.BeginTime = TimeSpan.FromSeconds(0.4);
                Storyboard.SetTarget(ani, mBorderPoint4);
                Storyboard.SetTargetProperty(ani, new PropertyPath("(Canvas.Left)"));
                storyBoard.Children.Add(ani);
                storyBoard.Begin();
            }
        }

        #endregion


        #region Templates

        private const string PART_Point = "PART_Point";
        private const string PART_Point1 = "PART_Point1";
        private const string PART_Point2 = "PART_Point2";
        private const string PART_Point3 = "PART_Point3";
        private const string PART_Point4 = "PART_Point4";

        private Border mBorderPoint;
        private Border mBorderPoint1;
        private Border mBorderPoint2;
        private Border mBorderPoint3;
        private Border mBorderPoint4;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            mBorderPoint = GetTemplateChild(PART_Point) as Border;
            if (mBorderPoint != null)
            {

            }
            mBorderPoint1 = GetTemplateChild(PART_Point1) as Border;
            if (mBorderPoint1 != null)
            {

            }
            mBorderPoint2 = GetTemplateChild(PART_Point2) as Border;
            if (mBorderPoint2 != null)
            {

            }
            mBorderPoint3 = GetTemplateChild(PART_Point3) as Border;
            if (mBorderPoint3 != null)
            {

            }
            mBorderPoint4 = GetTemplateChild(PART_Point4) as Border;
            if (mBorderPoint4 != null)
            {

            }
        }

        #endregion
    }
}
