//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    547ef973-d41a-4f32-a2b9-36031b7b6be6
//        CLR Version:              4.0.30319.18444
//        Name:                     ILayoutPanelElement
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Wpf.AvalonDock.Layout
//        File Name:                ILayoutPanelElement
//
//        created by Charley at 2014/7/22 9:41:52
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;

namespace VoiceCyber.Wpf.AvalonDock.Layout
{
    public interface ILayoutPanelElement : ILayoutElement
    {
        bool IsVisible { get; }
    }
}
