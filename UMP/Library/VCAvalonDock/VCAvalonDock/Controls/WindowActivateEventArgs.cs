//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    99e86521-94d7-44b9-bfbe-e56100fe7b07
//        CLR Version:              4.0.30319.18444
//        Name:                     WindowActivateEventArgs
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Wpf.AvalonDock.Controls
//        File Name:                WindowActivateEventArgs
//
//        created by Charley at 2014/7/22 10:13:29
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;

namespace VoiceCyber.Wpf.AvalonDock.Controls
{
    class WindowActivateEventArgs : EventArgs
    {
        public WindowActivateEventArgs(IntPtr hwndActivating)
        {
            HwndActivating = hwndActivating;
        }

        public IntPtr HwndActivating
        {
            get;
            private set;
        }
    }
}
