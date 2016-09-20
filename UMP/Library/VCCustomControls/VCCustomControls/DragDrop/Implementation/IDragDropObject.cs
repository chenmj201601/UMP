//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    42768444-2b96-4fe7-9404-ad279c4243fe
//        CLR Version:              4.0.30319.18444
//        Name:                     IDragDropObject
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Wpf.CustomControls.DragDrop.Implementation
//        File Name:                IDragDropObject
//
//        created by Charley at 2014/9/12 10:41:14
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Windows;

namespace VoiceCyber.Wpf.CustomControls
{
    /// <summary>
    /// 拖放对象，这个对象是拖放容器的关联对象
    /// </summary>
    public interface IDragDropObject
    {
        /// <summary>
        /// 开始拖放操作
        /// </summary>
        event EventHandler<DragDropEventArgs> StartDragged;
        /// <summary>
        /// 拖放对象位于容器之上，容器背景变色
        /// </summary>
        event EventHandler<DragDropEventArgs> DragOver;
        /// <summary>
        /// 放下拖放对象，应用拖放操作
        /// </summary>
        event EventHandler<DragDropEventArgs> Dropped;

        /// <summary>
        /// 开始拖放操作
        /// </summary>
        /// <param name="e"></param>
        void OnStartDragged(DragDropEventArgs e);
        /// <summary>
        /// 拖放对象位于容器之上，容器背景变色
        /// </summary>
        /// <param name="e"></param>
        void OnDragOver(DragDropEventArgs e);
        /// <summary>
        /// 放下拖放对象，应用拖放操作
        /// </summary>
        /// <param name="e"></param>
        void OnDropped(DragDropEventArgs e);
    }

    /// <summary>
    /// 拖放事件的参数
    /// </summary>
    public class DragDropEventArgs : EventArgs
    {
        /// <summary>
        /// 拖放源，一般也就是拖放容器
        /// </summary>
        public UIElement DragSource { get; set; }
        /// <summary>
        /// 拖放的数据
        /// </summary>
        public object DragData { get; set; }
        /// <summary>
        /// 拖放效果
        /// </summary>
        public DragDropEffects Effects { get; set; }
    }
}
