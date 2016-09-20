//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    70022224-9ba5-43a6-b2b6-5fe81adeefd5
//        CLR Version:              4.0.30319.18444
//        Name:                     ItemDeletingEventArgs
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.CustomControls.CollectionControl.Implementation
//        File Name:                ItemDeletingEventArgs
//
//        created by Charley at 2014/7/21 10:38:08
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
    public class ItemDeletingEventArgs : CancelRoutedEventArgs
    {
        #region Private Members

        private object _item;

        #endregion

        #region Constructor

        public ItemDeletingEventArgs(RoutedEvent itemDeletingEvent, object itemDeleting)
            : base(itemDeletingEvent)
        {
            _item = itemDeleting;
        }

        #region Property Item

        public object Item
        {
            get
            {
                return _item;
            }
        }

        #endregion

        #endregion
    }
}
