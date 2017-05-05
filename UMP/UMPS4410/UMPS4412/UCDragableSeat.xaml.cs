//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    57369413-e312-4a02-8399-106d2fbe04fc
//        CLR Version:              4.0.30319.18408
//        Name:                     UCDragableSeat
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS4412
//        File Name:                UCDragableSeat
//
//        created by Charley at 2016/6/15 16:59:14
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using UMPS4412.Models;


namespace UMPS4412
{
    /// <summary>
    /// UCDragableSeat.xaml 的交互逻辑
    /// </summary>
    public partial class UCDragableSeat
    {
        public static readonly DependencyProperty RegionSeatItemProperty =
            DependencyProperty.Register("RegionSeatItem", typeof(RegionSeatItem), typeof(UCDragableSeat), new PropertyMetadata(default(RegionSeatItem)));

        public RegionSeatItem RegionSeatItem
        {
            get { return (RegionSeatItem)GetValue(RegionSeatItemProperty); }
            set { SetValue(RegionSeatItemProperty, value); }
        }

        private bool mIsInited;
        private bool mIsDrag;
        private Point mOriPosition;

        public UCDragableSeat()
        {
            InitializeComponent();

            Loaded += UCDragableSeat_Loaded;
            PanelSeatItem.MoveStarted += PanelSeatItem_MoveStarted;
            PanelSeatItem.MoveStopped += PanelSeatItem_MoveStopped;
            PanelSeatItem.Moved += PanelSeatItem_Moved;
        }

        void UCDragableSeat_Loaded(object sender, RoutedEventArgs e)
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
                RegionSeatItem.SeatPanel = this;
                TxtSeatName.Text = RegionSeatItem.SeatName;
                MatrixTransform transform = new MatrixTransform();
                Matrix mx = new Matrix();
                mx.OffsetX = RegionSeatItem.Left;
                mx.OffsetY = RegionSeatItem.Top;
                transform.Matrix = mx;
                PanelSeatItem.RenderTransform = transform;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }


        #region Operation

        public void SetPosition()
        {
            try
            {
                if (RegionSeatItem == null) { return; }
                SetPosition(RegionSeatItem.Left, RegionSeatItem.Top);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        public void SetPosition(int left, int top)
        {
            try
            {
                MatrixTransform transform = new MatrixTransform();
                Matrix mx = new Matrix();
                mx.OffsetX = left;
                mx.OffsetY = top;
                transform.Matrix = mx;
                PanelSeatItem.RenderTransform = transform;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion


        #region Drag

        void PanelSeatItem_Moved()
        {
            try
            {
                if (mIsDrag)
                {
                    Point point = Mouse.GetPosition(GridMain);
                    MatrixTransform mt = PanelSeatItem.RenderTransform as MatrixTransform;
                    if (mt != null)
                    {
                        Matrix mx = mt.Matrix;
                        mx.OffsetX += point.X - mOriPosition.X;
                        mx.OffsetY += point.Y - mOriPosition.Y;
                        PanelSeatItem.RenderTransform = new MatrixTransform { Matrix = mx };
                        mOriPosition = point;

                        if (RegionSeatItem != null)
                        {
                            RegionSeatItem.Left = (int)Math.Round(mx.OffsetX);
                            RegionSeatItem.Top = (int)Math.Round(mx.OffsetY);
                            var info = RegionSeatItem.Info;
                            if (info != null)
                            {
                                info.Left = RegionSeatItem.Left;
                                info.Top = RegionSeatItem.Top;
                            }

                            var pageParent = RegionSeatItem.PageParent;
                            if (pageParent != null)
                            {
                                pageParent.OnItemMoved(RegionSeatItem);
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        void PanelSeatItem_MoveStopped()
        {
            try
            {
                mOriPosition = new Point(0, 0);
                mIsDrag = false;
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        private void PanelSeatItem_MoveStarted()
        {
            try
            {
                mOriPosition = Mouse.GetPosition(GridMain);
                mIsDrag = true;

                if (RegionSeatItem == null) { return; }
                var pageParent = RegionSeatItem.PageParent;
                if (pageParent == null) { return; }
                pageParent.OnItemFocused(RegionSeatItem);
            }
            catch (Exception ex)
            {
                ShowException(ex.Message);
            }
        }

        #endregion

    }
}
