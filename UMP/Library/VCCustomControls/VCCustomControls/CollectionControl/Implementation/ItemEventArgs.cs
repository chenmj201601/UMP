//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    4ef0a19f-4cd9-4598-9896-c34fd072251c
//        CLR Version:              4.0.30319.18444
//        Name:                     ItemEventArgs
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.CustomControls.CollectionControl.Implementation
//        File Name:                ItemEventArgs
//
//        created by Charley at 2014/7/21 10:38:44
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace VoiceCyber.Wpf.CustomControls
{
    public class ItemEventArgs : RoutedEventArgs
    {
        #region Protected Members

        private object _item;

        #endregion

        #region Constructor

        internal ItemEventArgs(RoutedEvent routedEvent, object newItem)
            : base(routedEvent)
        {
            _item = newItem;
        }

        #endregion

        #region Property Item

        public object Item
        {
            get
            {
                return _item;
            }
        }

        #endregion
    }
}
