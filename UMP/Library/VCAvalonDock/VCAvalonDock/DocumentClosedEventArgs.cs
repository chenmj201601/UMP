//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    c6a103d3-2385-4aed-8a94-ebc44bea43f0
//        CLR Version:              4.0.30319.18444
//        Name:                     DocumentClosedEventArgs
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Wpf.AvalonDock
//        File Name:                DocumentClosedEventArgs
//
//        created by Charley at 2014/7/22 10:15:15
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;
using VoiceCyber.Wpf.AvalonDock.Layout;

namespace VoiceCyber.Wpf.AvalonDock
{
    public class DocumentClosedEventArgs : EventArgs
    {
        public DocumentClosedEventArgs(LayoutDocument document)
        {
            Document = document;
        }

        public LayoutDocument Document
        {
            get;
            private set;
        }
    }
}
