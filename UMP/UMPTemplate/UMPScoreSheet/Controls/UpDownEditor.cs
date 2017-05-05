//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    802c4ea3-7893-4709-9093-3f51c357dddf
//        CLR Version:              4.0.30319.18444
//        Name:                     DoubleUpDownEditor
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.ScoreSheets.Controls
//        File Name:                DoubleUpDownEditor
//
//        created by Charley at 2014/7/28 12:11:25
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
    public class DoubleUpDownEditor:DoubleUpDown
    {
        static DoubleUpDownEditor()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DoubleUpDownEditor), new FrameworkPropertyMetadata(typeof(DoubleUpDownEditor)));
        }
    }
}
