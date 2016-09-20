//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    06a647ec-15e8-45ca-bc92-3d75705ff1bb
//        CLR Version:              4.0.30319.18444
//        Name:                     DockingManagerOverlayArea
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Wpf.AvalonDock.Controls
//        File Name:                DockingManagerOverlayArea
//
//        created by Charley at 2014/7/22 9:57:35
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace VoiceCyber.Wpf.AvalonDock.Controls
{
    public class DockingManagerOverlayArea : OverlayArea
    {
        internal DockingManagerOverlayArea(IOverlayWindow overlayWindow, DockingManager manager)
            : base(overlayWindow)
        {
            _manager = manager;

            base.SetScreenDetectionArea(new Rect(
                _manager.PointToScreenDPI(new Point()),
                _manager.TransformActualSizeToAncestor()));
        }

        DockingManager _manager;

    }
}
