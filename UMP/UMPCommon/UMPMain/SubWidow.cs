//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    016e233e-cad1-4855-8d81-fb191037ad9a
//        CLR Version:              4.0.30319.18444
//        Name:                     SubWidow
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPMain
//        File Name:                SubWidow
//
//        created by Charley at 2014/9/28 11:59:10
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Windows.Controls;
using VoiceCyber.Wpf.CustomControls;

namespace UMPMain
{
    public class SubWidow:Border,IDragElement
    {
        public SubWidow()
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
