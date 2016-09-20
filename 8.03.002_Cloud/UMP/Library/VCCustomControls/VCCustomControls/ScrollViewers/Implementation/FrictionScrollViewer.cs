//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    050ab305-8a3f-4112-974f-1264b7eab57f
//        CLR Version:              4.0.30319.18444
//        Name:                     FrictionScrollViewer
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Wpf.CustomControls.ScrollViewers.Implementation
//        File Name:                FrictionScrollViewer
//
//        created by Charley at 2014/9/10 15:43:31
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace VoiceCyber.Wpf.CustomControls
{
    /// <summary>
    /// 一个自定义的ScrollViewer控件，支持拖动内容
    /// 可以将内容拖动到指定的点
    /// 提供缓冲动画效果
    /// </summary>
    public class FrictionScrollViewer:ScrollViewer
    {
        #region Data

        // Used when manually scrolling.
        private DispatcherTimer mAnimationTimer = new DispatcherTimer();
        private Point mPreviousPoint;
        private Point mScrollStartOffset;
        private Point mScrollStartPoint;
        private Point mScrollTarget;
        private Vector mVelocity;
        private Point mAutoScrollTarget;
        private bool mShouldAutoScroll;
        #endregion

        #region Ctor
        /// <summary>
        /// Overrides metadata
        /// </summary>
        static FrictionScrollViewer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
            typeof(FrictionScrollViewer),
            new FrameworkPropertyMetadata(typeof(FrictionScrollViewer)));
        }

        /// <summary>
        /// Initialises all friction related variables
        /// </summary>
        public FrictionScrollViewer()
        {
            Friction = 0.95;
            mAnimationTimer.Interval = new TimeSpan(0, 0, 0, 0, 20);
            mAnimationTimer.Tick += HandleWorldTimerTick;
            mAnimationTimer.Start();
        }
        #endregion

        #region DPs
        /// <summary>
        /// The ammount of friction to use. Use the Friction property to set a 
        /// value between 0 and 1, 0 being no friction 1 is full friction 
        /// meaning the panel won’t "auto-scroll".
        /// </summary>
        public double Friction
        {
            get { return (double)GetValue(FrictionProperty); }
            set { SetValue(FrictionProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Friction.  
        /// </summary>
        public static readonly DependencyProperty FrictionProperty =
            DependencyProperty.Register("Friction", typeof(double),
            typeof(FrictionScrollViewer), new UIPropertyMetadata(0.0));
        #endregion

        #region overrides
        /// <summary>
        /// Get position and CaptureMouse
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            if (IsMouseOver)
            {
                mShouldAutoScroll = false;
                // Save starting point, used later when determining how much to scroll.
                mScrollStartPoint = e.GetPosition(this);
                mScrollStartOffset.X = HorizontalOffset;
                mScrollStartOffset.Y = VerticalOffset;
                // Update the cursor if can scroll or not. 
                Cursor = (ExtentWidth > ViewportWidth) ||
                    (ExtentHeight > ViewportHeight) ?
                    Cursors.ScrollAll : Cursors.Arrow;
                CaptureMouse();
            }
            base.OnMouseDown(e);
        }


        /// <summary>
        /// If IsMouseCaptured scroll to correct position. 
        /// Where position is updated by animation timer
        /// </summary>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (IsMouseCaptured)
            {
                mShouldAutoScroll = false;
                Point currentPoint = e.GetPosition(this);
                // Determine the new amount to scroll.
                Point delta = new Point(mScrollStartPoint.X -
                    currentPoint.X, mScrollStartPoint.Y - currentPoint.Y);
                mScrollTarget.X = mScrollStartOffset.X + delta.X;
                mScrollTarget.Y = mScrollStartOffset.Y + delta.Y;
                // Scroll to the new position.
                ScrollToHorizontalOffset(mScrollTarget.X);
                ScrollToVerticalOffset(mScrollTarget.Y);
            }
            base.OnMouseMove(e);
        }


        /// <summary>
        /// Release MouseCapture if its captured
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            if (IsMouseCaptured)
            {
                Cursor = Cursors.Arrow;
                ReleaseMouseCapture();
            }
            base.OnMouseUp(e);
        }
        #endregion

        #region Animation timer Tick
        /// <summary>
        /// Animation timer tick, used to move the scrollviewer incrementally
        /// to the desired position. This also uses the friction setting
        /// when determining how much to move the scrollviewer
        /// </summary>
        private void HandleWorldTimerTick(object sender, EventArgs e)
        {
            if (IsMouseCaptured)
            {
                Point currentPoint = Mouse.GetPosition(this);
                mVelocity = mPreviousPoint - currentPoint;
                mPreviousPoint = currentPoint;
            }
            else
            {
                if (mShouldAutoScroll)
                {
                    Point currentScroll = new Point(ScrollInfo.HorizontalOffset + ScrollInfo.ViewportWidth / 2.0, ScrollInfo.VerticalOffset + ScrollInfo.ViewportHeight / 2.0);
                    Vector offset = mAutoScrollTarget - currentScroll;
                    mShouldAutoScroll = offset.Length > 2.0;

                    // FIXME: 10.0 here is the scroll speed factor, a higher value means slower auto-scroll, 1 means no animation
                    ScrollToHorizontalOffset(HorizontalOffset + offset.X / 10.0);
                    ScrollToVerticalOffset(VerticalOffset + offset.Y / 10.0);
                }
                else
                {
                    if (mVelocity.Length > 1)
                    {
                        ScrollToHorizontalOffset(mScrollTarget.X);
                        ScrollToVerticalOffset(mScrollTarget.Y);
                        mScrollTarget.X += mVelocity.X;
                        mScrollTarget.Y += mVelocity.Y;
                        mVelocity *= Friction;
                        System.Diagnostics.Debug.WriteLine("Scroll @ " + ScrollInfo.HorizontalOffset + ", " + ScrollInfo.VerticalOffset);

                    }
                }

                InvalidateScrollInfo();
                InvalidateVisual();
            }
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        public Point AutoScrollTarget
        {
            set
            {
                mAutoScrollTarget = value;
                mShouldAutoScroll = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        public void ScrollToCenterTarget(Point target)
        {
            if (ScrollInfo != null)
            {
                ScrollToHorizontalOffset(target.X - ScrollInfo.ViewportWidth / 2.0);
                ScrollToVerticalOffset(target.Y - ScrollInfo.ViewportHeight / 2.0); 
            }
        }
    }
}
