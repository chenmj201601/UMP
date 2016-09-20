//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    c9aac610-ff9d-4af7-beed-ddb15af26b81
//        CLR Version:              4.0.30319.18444
//        Name:                     OverlayArea
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Wpf.AvalonDock.Controls
//        File Name:                OverlayArea
//
//        created by Charley at 2014/7/22 10:11:22
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace VoiceCyber.Wpf.AvalonDock.Controls
{
    public abstract class OverlayArea : IOverlayWindowArea
    {
        internal OverlayArea(IOverlayWindow overlayWindow)
        {
            _overlayWindow = overlayWindow;
        }

        IOverlayWindow _overlayWindow;

        Rect? _screenDetectionArea;
        Rect IOverlayWindowArea.ScreenDetectionArea
        {
            get
            {
                return _screenDetectionArea.Value;
            }
        }

        protected void SetScreenDetectionArea(Rect rect)
        {
            _screenDetectionArea = rect;
        }




    }
}
