//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    7039db95-6683-4882-b6dd-2ecc46e61f26
//        CLR Version:              4.0.30319.18444
//        Name:                     DragPanel
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Wpf.CustomControls.DragElement
//        File Name:                DragPanel
//
//        created by Charley at 2014/8/22 15:19:41
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace VoiceCyber.Wpf.CustomControls
{
    /// <summary>
    /// 可拖动的面板
    /// </summary>
    public class DragPanel : UserControl, IDragElement
    {
        public DragPanel()
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
            this.PreviewMouseUp += (s, e) =>
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

        public event Action Moved;

        public event Action MoveStopped;
    }
}
