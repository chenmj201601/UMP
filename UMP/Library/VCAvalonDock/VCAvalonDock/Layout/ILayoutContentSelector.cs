//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    01907595-df52-4b45-984d-49cea30569e6
//        CLR Version:              4.0.30319.18444
//        Name:                     ILayoutContentSelector
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Wpf.AvalonDock.Layout
//        File Name:                ILayoutContentSelector
//
//        created by Charley at 2014/7/22 9:39:05
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;

namespace VoiceCyber.Wpf.AvalonDock.Layout
{
    public interface ILayoutContentSelector
    {
        int SelectedContentIndex { get; set; }

        int IndexOf(LayoutContent content);

        LayoutContent SelectedContent { get; }
    }
}
