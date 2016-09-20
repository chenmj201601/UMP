//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    3b031421-25ee-43b2-ba5c-f88050f6a368
//        CLR Version:              4.0.30319.18444
//        Name:                     ItemAddingEventArgs
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.CustomControls.CollectionControl.Implementation
//        File Name:                ItemAddingEventArgs
//
//        created by Charley at 2014/7/21 10:36:53
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using VoiceCyber.Wpf.CustomControls.Core;

namespace VoiceCyber.Wpf.CustomControls
{
    public class ItemAddingEventArgs : CancelRoutedEventArgs
    {
        #region Constructor

        public ItemAddingEventArgs(RoutedEvent itemAddingEvent, object itemAdding)
            : base(itemAddingEvent)
        {
            Item = itemAdding;
        }

        #endregion

        #region Properties

        #region Item Property

        public object Item
        {
            get;
            set;
        }

        #endregion

        #endregion //Properties
    }
}
