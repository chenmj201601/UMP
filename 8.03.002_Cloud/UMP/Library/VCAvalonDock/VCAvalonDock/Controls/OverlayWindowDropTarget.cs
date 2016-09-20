//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    2795ae28-50ad-4e46-b92f-3701c0022f50
//        CLR Version:              4.0.30319.18444
//        Name:                     OverlayWindowDropTarget
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Wpf.AvalonDock.Controls
//        File Name:                OverlayWindowDropTarget
//
//        created by Charley at 2014/7/22 10:11:59
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace VoiceCyber.Wpf.AvalonDock.Controls
{
    public class OverlayWindowDropTarget : IOverlayWindowDropTarget
    {
        internal OverlayWindowDropTarget(IOverlayWindowArea overlayArea, OverlayWindowDropTargetType targetType, FrameworkElement element)
        {
            _overlayArea = overlayArea;
            _type = targetType;
            _screenDetectionArea = new Rect(element.TransformToDeviceDPI(new Point()), element.TransformActualSizeToAncestor());
        }

        IOverlayWindowArea _overlayArea;

        Rect _screenDetectionArea;
        Rect IOverlayWindowDropTarget.ScreenDetectionArea
        {
            get
            {
                return _screenDetectionArea;
            }

        }

        OverlayWindowDropTargetType _type;
        OverlayWindowDropTargetType IOverlayWindowDropTarget.Type
        {
            get { return _type; }
        }


    }
}
