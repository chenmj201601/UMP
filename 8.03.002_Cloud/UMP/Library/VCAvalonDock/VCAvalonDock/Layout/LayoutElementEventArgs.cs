//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    132d1ff0-1ba8-43cf-a995-546d1de83e36
//        CLR Version:              4.0.30319.18444
//        Name:                     LayoutElementEventArgs
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Wpf.AvalonDock.Layout
//        File Name:                LayoutElementEventArgs
//
//        created by Charley at 2014/7/22 9:48:29
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;

namespace VoiceCyber.Wpf.AvalonDock.Layout
{
    public class LayoutElementEventArgs : EventArgs
    {
        public LayoutElementEventArgs(LayoutElement element)
        {
            Element = element;
        }


        public LayoutElement Element
        {
            get;
            private set;
        }
    }
}
