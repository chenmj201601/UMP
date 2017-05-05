//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    d4d81128-4ac6-40ea-93c3-c91427cbc3ba
//        CLR Version:              4.0.30319.18444
//        Name:                     SubWindowHelper
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPMain
//        File Name:                SubWindowHelper
//
//        created by Charley at 2014/9/28 12:01:39
//        http://www.voicecyber.com 
//
//======================================================================

using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using VoiceCyber.Wpf.CustomControls;

namespace UMPMain
{
    public class SubWindowHelper
    {
        private IInputElement mContainer;
        private object mDragObject;
        private Point mOrgPoint;
        private bool mIsDrag;

        /// <summary>
        /// 初始化容器和被拖动的元素
        /// </summary>
        /// <typeparam name="T">被拖动元素的类型，此类必须实现IDragElement接口</typeparam>
        /// <param name="container">容器</param>
        /// <param name="dragObject">拖动对象</param>
        public void Init<T>(IInputElement container, T dragObject) where T : FrameworkElement, IDragElement
        {
            mContainer = container;
            mDragObject = dragObject;
            IDragElement dragElement = mDragObject as IDragElement;
            if (dragElement != null)
            {
                dragElement.MoveStarted += dragElement_MoveStarted;
                dragElement.MoveStopped += dragElement_MoveStopped;
                dragElement.Moved += dragElement_Moved;
            }
            mOrgPoint = new Point(0, 0);
            mIsDrag = false;
        }

        void dragElement_Moved()
        {
            if (mIsDrag)
            {
                if (mContainer != null && mDragObject != null)
                {
                    //IDragElement dragElement = mDragObject as IDragElement;
                    //FrameworkElement frameworkElement = mDragObject as FrameworkElement;
                    //if (dragElement != null && frameworkElement != null)
                    //{
                    //    Point point = Mouse.GetPosition(mContainer);
                    //    MatrixTransform mt = frameworkElement.RenderTransform as MatrixTransform;
                    //    if (mt != null)
                    //    {
                    //        Matrix mx = mt.Matrix;
                    //        mx.OffsetX += point.X - mOriPosition.X;
                    //        mx.OffsetY += point.Y - mOriPosition.Y;
                    //        frameworkElement.RenderTransform = new MatrixTransform { Matrix = mx };
                    //        mOriPosition = point;
                    //    }
                    //}

                    FrameworkElement dragElement = mDragObject as FrameworkElement;
                    FrameworkElement container = mContainer as FrameworkElement;
                    if (dragElement != null && container != null)
                    {
                        Point point1 = Mouse.GetPosition(container);
                        Point point2 = Mouse.GetPosition(dragElement);
                        double width1 = container.ActualWidth;
                        double width2 = dragElement.ActualWidth;
                        double height1 = container.ActualHeight;
                        double height2 = dragElement.ActualHeight;
                        double offsetX = 0.0;
                        double offsetY = 0.0;
                        if (point1.X < point2.X ||
                            point1.X - point2.X + width2 > width1)
                        {
                            offsetX = 0.0;
                        }
                        else
                        {
                            offsetX = point1.X - mOrgPoint.X;
                        }
                        if (point1.Y < point2.Y ||
                            point1.Y - point2.Y + height2 > height1)
                        {
                            offsetY = 0.0;
                        }
                        else
                        {
                            offsetY = point1.Y - mOrgPoint.Y;
                        }
                        MatrixTransform mt = dragElement.RenderTransform as MatrixTransform;
                        if (mt != null)
                        {
                            Matrix mx = mt.Matrix;
                            mx.OffsetX += offsetX;
                            mx.OffsetY += offsetY;
                            dragElement.RenderTransform = new MatrixTransform { Matrix = mx };
                            mOrgPoint = point1;
                        }
                    }
                }
            }
        }

        void dragElement_MoveStopped()
        {
            mOrgPoint = new Point(0, 0);
            mIsDrag = false;
        }

        void dragElement_MoveStarted()
        {
            if (mContainer != null && mDragObject != null)
            {
                //IDragElement dragElement = mDragObject as IDragElement;
                //FrameworkElement frameworkElement = mDragObject as FrameworkElement;
                //if (dragElement != null && frameworkElement != null)
                //{
                //    mPoint1 = Mouse.GetPosition(mContainer);

                //    mIsDrag = true;
                //}

                FrameworkElement dragElement = mDragObject as FrameworkElement;
                FrameworkElement container = mContainer as FrameworkElement;
                if (dragElement != null && container != null)
                {
                    mOrgPoint = Mouse.GetPosition(container);
                    mIsDrag = true;
                }
            }
        }
    }
}
