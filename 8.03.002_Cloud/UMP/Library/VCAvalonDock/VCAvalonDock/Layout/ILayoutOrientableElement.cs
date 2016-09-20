//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    f92236fd-06c9-4a23-85f1-d6978d209c9b
//        CLR Version:              4.0.30319.18444
//        Name:                     ILayoutOrientableElement
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Wpf.AvalonDock.Layout
//        File Name:                ILayoutOrientableElement
//
//        created by Charley at 2014/7/22 9:41:08
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace VoiceCyber.Wpf.AvalonDock.Layout
{
    public interface ILayoutOrientableGroup : ILayoutGroup
    {
        Orientation Orientation { get; set; }
    }
}
