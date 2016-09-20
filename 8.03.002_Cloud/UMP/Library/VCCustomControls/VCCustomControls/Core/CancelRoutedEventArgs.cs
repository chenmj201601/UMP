//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    dd857eaf-d188-411c-80bf-7d9dd54d617d
//        CLR Version:              4.0.30319.18444
//        Name:                     CancelRoutedEventArgs
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.CustomControls.Core
//        File Name:                CancelRoutedEventArgs
//
//        created by Charley at 2014/7/21 10:37:38
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace VoiceCyber.Wpf.CustomControls.Core
{
    public delegate void CancelRoutedEventHandler(object sender, CancelRoutedEventArgs e);

    /// <summary>
    /// An event data class that allows to inform the sender that the handler wants to cancel
    /// the ongoing action.
    /// 
    /// The handler can set the "Cancel" property to false to cancel the action.
    /// </summary>
    public class CancelRoutedEventArgs : RoutedEventArgs
    {
        public CancelRoutedEventArgs()
            : base()
        {
        }

        public CancelRoutedEventArgs(RoutedEvent routedEvent)
            : base(routedEvent)
        {
        }

        public CancelRoutedEventArgs(RoutedEvent routedEvent, object source)
            : base(routedEvent, source)
        {
        }

        #region Cancel Property

        public bool Cancel
        {
            get;
            set;
        }

        #endregion Cancel Property
    }
}
