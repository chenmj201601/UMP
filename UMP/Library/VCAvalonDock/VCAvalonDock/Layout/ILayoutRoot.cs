//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    7b5ca4d1-207c-49c3-a70d-c68b1e5ac44b
//        CLR Version:              4.0.30319.18444
//        Name:                     ILayoutRoot
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Wpf.AvalonDock.Layout
//        File Name:                ILayoutRoot
//
//        created by Charley at 2014/7/22 9:43:34
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace VoiceCyber.Wpf.AvalonDock.Layout
{
    public interface ILayoutRoot
    {
        DockingManager Manager { get; }

        LayoutPanel RootPanel { get; }

        LayoutAnchorSide TopSide { get; }
        LayoutAnchorSide LeftSide { get; }
        LayoutAnchorSide RightSide { get; }
        LayoutAnchorSide BottomSide { get; }

        LayoutContent ActiveContent { get; set; }

        void CollectGarbage();

        ObservableCollection<LayoutFloatingWindow> FloatingWindows { get; }
        ObservableCollection<LayoutAnchorable> Hidden { get; }
    }
}
