//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    f8de33fb-a020-4b4f-b4bc-28ba814fcd86
//        CLR Version:              4.0.30319.18063
//        Name:                     DragHelper
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.CustomControls
//        File Name:                DragHelper
//
//        created by Charley at 2014/4/4 12:14:50
//        http://www.voicecyber.com 
//
//======================================================================

using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace VoiceCyber.Wpf.CustomControls
{
    /// <summary>
    /// 一个实现WPF元素可以拖动的帮助类型
    /// </summary>
    public class DragHelper
    {
        private IInputElement mContainer;
        private object mDragObject;
        private Point mOriPosition;
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
            mOriPosition = new Point(0, 0);
            mIsDrag = false;
        }

        void dragElement_Moved()
        {
            if (mIsDrag)
            {
                if (mContainer != null && mDragObject != null)
                {
                    IDragElement dragElement = mDragObject as IDragElement;
                    FrameworkElement frameworkElement = mDragObject as FrameworkElement;
                    if (dragElement != null && frameworkElement != null)
                    {
                        Point point = Mouse.GetPosition(mContainer);
                        MatrixTransform mt = frameworkElement.RenderTransform as MatrixTransform;
                        if (mt != null)
                        {
                            Matrix mx = mt.Matrix;
                            mx.OffsetX += point.X - mOriPosition.X;
                            mx.OffsetY += point.Y - mOriPosition.Y;
                            frameworkElement.RenderTransform = new MatrixTransform { Matrix = mx };
                            mOriPosition = point;
                        }
                    }
                }
            }
        }

        void dragElement_MoveStopped()
        {
            mOriPosition = new Point(0, 0);
            mIsDrag = false;
        }

        void dragElement_MoveStarted()
        {
            if (mContainer != null && mDragObject != null)
            {
                IDragElement dragElement = mDragObject as IDragElement;
                FrameworkElement frameworkElement = mDragObject as FrameworkElement;
                if (dragElement != null && frameworkElement != null)
                {
                    mOriPosition = Mouse.GetPosition(mContainer);
                    mIsDrag = true;
                }
            }
        }
    }
}
