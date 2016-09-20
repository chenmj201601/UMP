//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    2327999a-ad36-4c6c-a063-065cc47e1ed3
//        CLR Version:              4.0.30319.18444
//        Name:                     DropTargetType
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Wpf.AvalonDock.Controls
//        File Name:                DropTargetType
//
//        created by Charley at 2014/7/22 10:01:18
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;

namespace VoiceCyber.Wpf.AvalonDock.Controls
{
    public enum DropTargetType
    {
        DockingManagerDockLeft,
        DockingManagerDockTop,
        DockingManagerDockRight,
        DockingManagerDockBottom,

        DocumentPaneDockLeft,
        DocumentPaneDockTop,
        DocumentPaneDockRight,
        DocumentPaneDockBottom,
        DocumentPaneDockInside,

        DocumentPaneGroupDockInside,

        AnchorablePaneDockLeft,
        AnchorablePaneDockTop,
        AnchorablePaneDockRight,
        AnchorablePaneDockBottom,
        AnchorablePaneDockInside,

        DocumentPaneDockAsAnchorableLeft,
        DocumentPaneDockAsAnchorableTop,
        DocumentPaneDockAsAnchorableRight,
        DocumentPaneDockAsAnchorableBottom,
    }
}
