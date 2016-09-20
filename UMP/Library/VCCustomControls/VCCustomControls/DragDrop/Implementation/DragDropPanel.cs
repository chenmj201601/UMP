//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    438723bb-9c91-44de-a166-7459954cb7dc
//        CLR Version:              4.0.30319.18444
//        Name:                     DragDropPanel
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Wpf.CustomControls.DragDrop.Implementation
//        File Name:                DragDropPanel
//
//        created by Charley at 2014/9/12 10:46:24
//        http://www.voicecyber.com 
//
//======================================================================

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace VoiceCyber.Wpf.CustomControls
{
    /// <summary>
    /// 拖放操作的容器
    /// </summary>
    public class DragDropPanel : ContentControl
    {
        static DragDropPanel()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DragDropPanel),
                new FrameworkPropertyMetadata(typeof(DragDropPanel)));
        }
        /// <summary>
        /// 
        /// </summary>
        public DragDropPanel()
        {
            MouseMove += DragDropPanel_MouseMove;
            DragEnter += DragDropPanel_DragEnter;
            DragOver += DragDropPanel_DragOver;
            DragLeave += DragDropPanel_DragLeave;
            Drop += DragDropPanel_Drop;
        }

        void DragDropPanel_Drop(object sender, DragEventArgs e)
        {
            Background = Brushes.Transparent;
            var dragData = Tag as IDragDropObject;
            if (dragData != null)
            {
                DragDropEventArgs args = new DragDropEventArgs();
                args.DragSource = this;
                args.DragData = e.Data;
                dragData.OnDropped(args);
            }
        }

        void DragDropPanel_DragLeave(object sender, DragEventArgs e)
        {
            Background = Brushes.Transparent;
        }

        void DragDropPanel_DragOver(object sender, DragEventArgs e)
        {
            var dragData = Tag as IDragDropObject;
            if (dragData != null)
            {
                DragDropEventArgs args = new DragDropEventArgs();
                args.DragSource = this;
                args.DragData = e.Data;
                dragData.OnDragOver(args);
                if (AllowDrop)
                {
                    Background = Brushes.Thistle;
                }
            }
        }

        void DragDropPanel_DragEnter(object sender, DragEventArgs e)
        {
            var dragData = Tag as IDragDropObject;
            if (dragData != null)
            {
                DragDropEventArgs args = new DragDropEventArgs();
                args.DragSource = this;
                args.DragData = e.Data;
                dragData.OnDragOver(args);
            }
        }

        void DragDropPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var dragData = Tag as IDragDropObject;
                if (dragData != null)
                {
                    DragDropEventArgs args = new DragDropEventArgs();
                    args.DragSource = this;
                    dragData.OnStartDragged(args);
                }
            }
        }
    }
}
