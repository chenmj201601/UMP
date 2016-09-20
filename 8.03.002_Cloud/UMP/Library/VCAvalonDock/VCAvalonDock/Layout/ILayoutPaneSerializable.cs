//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    2ddf2737-905c-4aa1-bd59-3bbb1ff5788f
//        CLR Version:              4.0.30319.18444
//        Name:                     ILayoutPaneSerializable
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Wpf.AvalonDock.Layout
//        File Name:                ILayoutPaneSerializable
//
//        created by Charley at 2014/7/22 9:42:14
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;

namespace VoiceCyber.Wpf.AvalonDock.Layout
{
    interface ILayoutPaneSerializable
    {
        string Id { get; set; }
    }
}
