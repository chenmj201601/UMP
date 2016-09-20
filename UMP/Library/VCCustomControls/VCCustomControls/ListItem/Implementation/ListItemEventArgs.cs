//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    7c9944f5-1f4d-4cb2-b5b9-c32988dce025
//        CLR Version:              4.0.30319.18063
//        Name:                     ListItemEventArgs
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Wpf.CustomControls.ListItem.Implementation
//        File Name:                ListItemEventArgs
//
//        created by Charley at 2015/6/9 17:41:11
//        http://www.voicecyber.com 
//
//======================================================================

using System.Windows;

namespace VoiceCyber.Wpf.CustomControls.ListItem.Implementation
{
    /// <summary>
    /// 
    /// </summary>
    public class ListItemEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        public int Code { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public object Data { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public const int EVT_MOUSEDOUBLECLICK = 101;
    }
}
