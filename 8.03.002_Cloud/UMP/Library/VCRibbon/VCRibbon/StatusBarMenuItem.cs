//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    229b99c2-f082-4fd5-a924-80a19486f2cf
//        CLR Version:              4.0.30319.18444
//        Name:                     StatusBarMenuItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Ribbon
//        File Name:                StatusBarMenuItem
//
//        created by Charley at 2014/5/27 22:48:03
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace VoiceCyber.Ribbon
{
    /// <summary>
    /// Represents menu item in ribbon status bar menu
    /// </summary>
    public class StatusBarMenuItem : MenuItem
    {
        #region Properties

        /// <summary>
        /// Gets or sets Ribbon Status Bar menu item
        /// </summary>
        public StatusBarItem StatusBarItem
        {
            get { return (StatusBarItem)GetValue(StatusBarItemProperty); }
            set { SetValue(StatusBarItemProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for StatusBarItem.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty StatusBarItemProperty =
            DependencyProperty.Register("StatusBarItem", typeof(StatusBarItem), typeof(StatusBarMenuItem), new UIPropertyMetadata(null));


        #endregion

        #region Constructors

        /// <summary>
        /// Static constructor
        /// </summary>
        static StatusBarMenuItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(StatusBarMenuItem), new FrameworkPropertyMetadata(typeof(StatusBarMenuItem)));
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="item">Ribbon Status Bar menu item</param>
        public StatusBarMenuItem(StatusBarItem item)
        {
            StatusBarItem = item;
        }

        #endregion
    }
}
