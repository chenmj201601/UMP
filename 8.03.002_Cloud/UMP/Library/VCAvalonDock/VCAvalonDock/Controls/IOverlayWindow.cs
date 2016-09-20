//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    8d2241a8-5b05-44c7-8048-f77a6cb8d122
//        CLR Version:              4.0.30319.18444
//        Name:                     IOverlayWindow
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Wpf.AvalonDock.Controls
//        File Name:                IOverlayWindow
//
//        created by Charley at 2014/7/22 10:02:46
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;

namespace VoiceCyber.Wpf.AvalonDock.Controls
{
    internal interface IOverlayWindow
    {
        IEnumerable<IDropTarget> GetTargets();

        void DragEnter(LayoutFloatingWindowControl floatingWindow);
        void DragLeave(LayoutFloatingWindowControl floatingWindow);

        void DragEnter(IDropArea area);
        void DragLeave(IDropArea area);

        void DragEnter(IDropTarget target);
        void DragLeave(IDropTarget target);
        void DragDrop(IDropTarget target);
    }
}
