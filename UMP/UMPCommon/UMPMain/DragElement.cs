//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    35677512-73e5-4b51-94c2-42f265b24d4d
//        CLR Version:              4.0.30319.18444
//        Name:                     DragElement
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPMain
//        File Name:                DragElement
//
//        created by Charley at 2014/9/28 10:42:16
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using VoiceCyber.Wpf.CustomControls;

namespace UMPMain
{
    public class DragElement:Border,IDragElement
    {
        public DragElement()
        {
            this.MouseDown += (s, e) =>
            {
                if (MoveStarted != null)
                {
                    MoveStarted();
                }
            };
            this.MouseMove += (s, e) =>
            {
                if (Moved != null)
                {
                    Moved();
                }
            };
            this.MouseUp += (s, e) =>
            {
                if (MoveStopped != null)
                {
                    MoveStopped();
                }
            };
            this.MouseLeave += (s, e) =>
            {
                if (MoveStopped != null)
                {
                    MoveStopped();
                }
            };
        }

        public event Action MoveStarted;

        public event Action MoveStopped;

        public event Action Moved;

    }
}
