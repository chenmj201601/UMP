//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    da114d2f-00fc-4070-a23f-d29aeffa05af
//        CLR Version:              4.0.30319.18444
//        Name:                     IOverlayWindowHost
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Wpf.AvalonDock.Controls
//        File Name:                IOverlayWindowHost
//
//        created by Charley at 2014/7/22 10:03:38
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace VoiceCyber.Wpf.AvalonDock.Controls
{
    internal interface IOverlayWindowHost
    {
        bool HitTest(Point dragPoint);

        IOverlayWindow ShowOverlayWindow(LayoutFloatingWindowControl draggingWindow);

        void HideOverlayWindow();

        IEnumerable<IDropArea> GetDropAreas(LayoutFloatingWindowControl draggingWindow);

        DockingManager Manager { get; }
    }
}
