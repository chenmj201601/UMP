//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    16789183-f443-4d49-bccc-d1757a0b7471
//        CLR Version:              4.0.30319.18444
//        Name:                     ChildrenTreeChangedEventArgs
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Wpf.AvalonDock.Layout
//        File Name:                ChildrenTreeChangedEventArgs
//
//        created by Charley at 2014/7/22 9:37:41
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;

namespace VoiceCyber.Wpf.AvalonDock.Layout
{
    public enum ChildrenTreeChange
    {
        /// <summary>
        /// Direct insert/remove operation has been perfomed to the group
        /// </summary>
        DirectChildrenChanged,

        /// <summary>
        /// An element below in the hierarchy as been added/removed
        /// </summary>
        TreeChanged
    }

    public class ChildrenTreeChangedEventArgs : EventArgs
    {
        public ChildrenTreeChangedEventArgs(ChildrenTreeChange change)
        {
            Change = change;
        }

        public ChildrenTreeChange Change { get; private set; }
    }
}
