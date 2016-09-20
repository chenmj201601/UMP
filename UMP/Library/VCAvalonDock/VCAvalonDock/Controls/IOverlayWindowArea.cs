//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    a53bc38a-b772-46ae-9ec2-b97bdda962eb
//        CLR Version:              4.0.30319.18444
//        Name:                     IOverlayWindowArea
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Wpf.AvalonDock.Controls
//        File Name:                IOverlayWindowArea
//
//        created by Charley at 2014/7/22 10:03:01
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace VoiceCyber.Wpf.AvalonDock.Controls
{
    internal interface IOverlayWindowArea
    {
        Rect ScreenDetectionArea { get; }
    }
}
