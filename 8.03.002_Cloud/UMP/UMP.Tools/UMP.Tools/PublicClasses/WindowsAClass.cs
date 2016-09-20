using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

namespace UMP.Tools.PublicClasses
{
    public class WindowsAClass : Window
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        private const int WM_SYSCOMMAND = 0x112;
        public const int WM_LBUTTONUP = 0x0202;
        private HwndSource IHwndSource;
        public double IDoubleRelativeClip = 10;

        public WindowsAClass()
        {
            this.Loaded += delegate
            {
                InitializeEvent();
            };
            this.SourceInitialized += WindowsAClass_SourceInitialized;
        }

        private void WindowsAClass_SourceInitialized(object sender, EventArgs e)
        {
            IHwndSource = PresentationSource.FromVisual((Visual)sender) as HwndSource;
        }

        #region 为元素注册事件
        private void InitializeEvent()
        {
            ControlTemplate baseWindowTemplate = (ControlTemplate)App.Current.Resources["WindowATemplate"];
            Border LBorderClip = (Border)baseWindowTemplate.FindName("WindowABorder", this);

            //LBorderClip.MouseMove += delegate
            //{
            //    DisplayResizeCursor(null, null);
            //};

            //LBorderClip.PreviewMouseDown += delegate
            //{
            //    Resize(null, null);
            //};

            LBorderClip.MouseLeftButtonDown += delegate
            {
                DragMove();
            };

            //this.PreviewMouseMove += delegate
            //{
            //    ResetCursor(null, null);
            //};
        }
        #endregion

        #region 重写的DragMove，以便解决利用系统自带的DragMove出现Exception的情况
        public new void DragMove()
        {
            if (this.WindowState == WindowState.Normal)
            {
                SendMessage(IHwndSource.Handle, WM_SYSCOMMAND, (IntPtr)0xf012, IntPtr.Zero);
                SendMessage(IHwndSource.Handle, WM_LBUTTONUP, IntPtr.Zero, IntPtr.Zero);
            }
        }
        #endregion

        bool enableXResize = true;
        bool enableYResize = true;

        protected override void OnRender(DrawingContext drawingContext)
        {
            enableXResize = true;
            enableYResize = true;
            double x = this.ActualWidth;
            double y = this.ActualHeight;
            if (x < MinWidth)
                enableXResize = false;
            if (y < MinHeight)
                enableYResize = false;

            if (!enableXResize)
            {
                this.Width = MinWidth;
                return;
            }

            if (!enableYResize)
            {
                this.Height = MinHeight;
                return;
            }

            base.OnRender(drawingContext);
        }

        #region 显示拖拉鼠标形状
        private void DisplayResizeCursor(object sender, MouseEventArgs e)
        {
            Point pos = Mouse.GetPosition(this);
            double x = pos.X;
            double y = pos.Y;
            double w = this.ActualWidth;  //注意这个地方使用ActualWidth,才能够实时显示宽度变化
            double h = this.ActualHeight;

            if (x <= IDoubleRelativeClip & y <= IDoubleRelativeClip) // left top
            {
                this.Cursor = Cursors.SizeNWSE;
            }
            if (x >= w - IDoubleRelativeClip & y <= IDoubleRelativeClip) //right top
            {
                this.Cursor = Cursors.SizeNESW;
            }

            if (x >= w - IDoubleRelativeClip & y >= h - IDoubleRelativeClip) //bottom right
            {
                this.Cursor = Cursors.SizeNWSE;
            }

            if (x <= IDoubleRelativeClip & y >= h - IDoubleRelativeClip)  // bottom left
            {
                this.Cursor = Cursors.SizeNESW;
            }

            if ((x >= IDoubleRelativeClip & x <= w - IDoubleRelativeClip) & y <= IDoubleRelativeClip) //top
            {
                this.Cursor = Cursors.SizeNS;
            }

            if (x >= w - IDoubleRelativeClip & (y >= IDoubleRelativeClip & y <= h - IDoubleRelativeClip)) //right
            {
                this.Cursor = Cursors.SizeWE;
            }

            if ((x >= IDoubleRelativeClip & x <= w - IDoubleRelativeClip) & y > h - IDoubleRelativeClip) //bottom
            {
                this.Cursor = Cursors.SizeNS;
            }

            if (x <= IDoubleRelativeClip & (y <= h - IDoubleRelativeClip & y >= IDoubleRelativeClip)) //left
            {
                this.Cursor = Cursors.SizeWE;
            }
        }
        #endregion

        #region  还原鼠标形状
        private void ResetCursor(object sender, MouseEventArgs e)
        {
            if (Mouse.LeftButton != MouseButtonState.Pressed)
            {
                this.Cursor = Cursors.Arrow;
            }
        }
        #endregion

        #region 这一部分是四个边加上四个角
        public enum ResizeDirection
        {
            Left = 1,
            Right = 2,
            Top = 3,
            TopLeft = 4,
            TopRight = 5,
            Bottom = 6,
            BottomLeft = 7,
            BottomRight = 8,
        }
        #endregion

        #region 判断区域，改变窗体大小

        private void ResizeWindow(ResizeDirection direction)
        {
            SendMessage(IHwndSource.Handle, WM_SYSCOMMAND, (IntPtr)(61440 + direction), IntPtr.Zero);
        }

        private void Resize(object sender, MouseButtonEventArgs e)
        {
            Point pos = Mouse.GetPosition(this);
            double x = pos.X;
            double y = pos.Y;
            double w = this.ActualWidth;
            double h = this.ActualHeight;

            #region TODO:resize details

            if (enableXResize && enableYResize)
            {
                #region corners
                if (x <= IDoubleRelativeClip & y <= IDoubleRelativeClip) // left top
                {
                    this.Cursor = Cursors.SizeNWSE;
                    ResizeWindow(ResizeDirection.TopLeft);
                }
                if (x >= w - IDoubleRelativeClip & y <= IDoubleRelativeClip) //right top
                {
                    this.Cursor = Cursors.SizeNESW;
                    ResizeWindow(ResizeDirection.TopRight);
                }

                if (x >= w - IDoubleRelativeClip & y >= h - IDoubleRelativeClip) //bottom right
                {
                    this.Cursor = Cursors.SizeNWSE;
                    ResizeWindow(ResizeDirection.BottomRight);
                }

                if (x <= IDoubleRelativeClip & y >= h - IDoubleRelativeClip)  // bottom left
                {
                    this.Cursor = Cursors.SizeNESW;
                    ResizeWindow(ResizeDirection.BottomLeft);
                }
                #endregion
            }

            if (enableXResize)
            {
                #region x direction
                if ((x >= IDoubleRelativeClip & x <= w - IDoubleRelativeClip) & y <= IDoubleRelativeClip) //top
                {
                    this.Cursor = Cursors.SizeNS;
                    ResizeWindow(ResizeDirection.Top);
                }
                if ((x >= IDoubleRelativeClip & x <= w - IDoubleRelativeClip) & y > h - IDoubleRelativeClip) //bottom
                {
                    this.Cursor = Cursors.SizeNS;
                    ResizeWindow(ResizeDirection.Bottom);
                }
                #endregion
            }

            if (enableYResize)
            {
                #region y direction
                if (x >= w - IDoubleRelativeClip & (y >= IDoubleRelativeClip & y <= h - IDoubleRelativeClip)) //right
                {
                    this.Cursor = Cursors.SizeWE;
                    ResizeWindow(ResizeDirection.Right);
                }
                if (x <= IDoubleRelativeClip & (y <= h - IDoubleRelativeClip & y >= IDoubleRelativeClip)) //left
                {
                    this.Cursor = Cursors.SizeWE;
                    ResizeWindow(ResizeDirection.Left);
                }
                #endregion
            }
            #endregion
        }
        #endregion
    }
}
