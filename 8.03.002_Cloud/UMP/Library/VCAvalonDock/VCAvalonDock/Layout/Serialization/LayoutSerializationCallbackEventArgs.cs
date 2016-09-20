//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    8cff5885-cdd1-47a7-bb1a-9a338f56ce4a
//        CLR Version:              4.0.30319.18444
//        Name:                     LayoutSerializationCallbackEventArgs
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Wpf.AvalonDock.Layout.Serialization
//        File Name:                LayoutSerializationCallbackEventArgs
//
//        created by Charley at 2014/7/22 9:34:41
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace VoiceCyber.Wpf.AvalonDock.Layout.Serialization
{
    public class LayoutSerializationCallbackEventArgs : CancelEventArgs
    {
        public LayoutSerializationCallbackEventArgs(LayoutContent model, object previousContent)
        {
            Cancel = false;
            Model = model;
            Content = previousContent;
        }

        public LayoutContent Model { get; private set; }

        public object Content { get; set; }
    }
}
