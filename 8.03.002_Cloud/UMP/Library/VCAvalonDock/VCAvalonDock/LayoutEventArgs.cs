//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    f848bb9b-3b28-43b2-9670-f9a9152896d9
//        CLR Version:              4.0.30319.18444
//        Name:                     LayoutEventArgs
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Wpf.AvalonDock
//        File Name:                LayoutEventArgs
//
//        created by Charley at 2014/7/22 10:16:14
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;
using VoiceCyber.Wpf.AvalonDock.Layout;

namespace VoiceCyber.Wpf.AvalonDock
{
    class LayoutEventArgs : EventArgs
    {
        public LayoutEventArgs(LayoutRoot layoutRoot)
        {
            LayoutRoot = layoutRoot;
        }

        public LayoutRoot LayoutRoot
        {
            get;
            private set;
        }
    }
}
