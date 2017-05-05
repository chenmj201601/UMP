//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    dde27406-37fc-4381-a728-4361980d0a04
//        CLR Version:              4.0.30319.18408
//        Name:                     CustomWaiter
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPUpdater.UserControls
//        File Name:                CustomWaiter
//
//        created by Charley at 2016/8/1 17:25:58
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace UMPUpdater.UserControls
{
    /// <summary>
    /// CustomWaiter.xaml 的交互逻辑
    /// </summary>
    public partial class CustomWaiter
    {

        public CustomWaiter()
        {
            InitializeComponent();

            Loaded += CustomWaiter_Loaded;
            SizeChanged += CustomWaiter_SizeChanged;
        }

        void CustomWaiter_Loaded(object sender, RoutedEventArgs e)
        {
            PlayAnimation();
        }

        void CustomWaiter_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            PlayAnimation();
        }

        private void PlayAnimation()
        {
            double offset = ActualWidth / 3.0;

            double duration = 2.0;
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

            if (Point != null)
            {
                Storyboard storyBoard = new Storyboard();
                storyBoard.BeginTime = TimeSpan.FromSeconds(0);
                Storyboard.SetTarget(ani, Point);
                Storyboard.SetTargetProperty(ani, new PropertyPath("(Canvas.Left)"));
                storyBoard.Children.Add(ani);
                storyBoard.Begin();
            }

            if (Point1 != null)
            {
                Storyboard storyBoard = new Storyboard();
                storyBoard.BeginTime = TimeSpan.FromSeconds(0.1);
                Storyboard.SetTarget(ani, Point1);
                Storyboard.SetTargetProperty(ani, new PropertyPath("(Canvas.Left)"));
                storyBoard.Children.Add(ani);
                storyBoard.Begin();
            }

            if (Point2 != null)
            {
                Storyboard storyBoard = new Storyboard();
                storyBoard.BeginTime = TimeSpan.FromSeconds(0.2);
                Storyboard.SetTarget(ani, Point2);
                Storyboard.SetTargetProperty(ani, new PropertyPath("(Canvas.Left)"));
                storyBoard.Children.Add(ani);
                storyBoard.Begin();
            }

            if (Point3 != null)
            {
                Storyboard storyBoard = new Storyboard();
                storyBoard.BeginTime = TimeSpan.FromSeconds(0.3);
                Storyboard.SetTarget(ani, Point3);
                Storyboard.SetTargetProperty(ani, new PropertyPath("(Canvas.Left)"));
                storyBoard.Children.Add(ani);
                storyBoard.Begin();
            }

            if (Point4 != null)
            {
                Storyboard storyBoard = new Storyboard();
                storyBoard.BeginTime = TimeSpan.FromSeconds(0.4);
                Storyboard.SetTarget(ani, Point4);
                Storyboard.SetTargetProperty(ani, new PropertyPath("(Canvas.Left)"));
                storyBoard.Children.Add(ani);
                storyBoard.Begin();
            }
        }
    }
}
