//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    57aa7ee0-c03e-4dd1-9d22-006ac1f0c458
//        CLR Version:              4.0.30319.18444
//        Name:                     MultiLineEditor
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.ScoreSheets.Controls
//        File Name:                MultiLineEditor
//
//        created by Charley at 2014/7/28 12:28:26
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using VoiceCyber.Wpf.CustomControls;

namespace VoiceCyber.UMP.ScoreSheets.Controls
{
    public class MultiLineEditor : MultiLineTextEditor
    {
        static MultiLineEditor()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MultiLineEditor), new FrameworkPropertyMetadata(typeof(MultiLineEditor)));
        }
    }
}
