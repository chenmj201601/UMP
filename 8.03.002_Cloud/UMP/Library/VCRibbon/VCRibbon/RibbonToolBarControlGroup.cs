//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    6584d332-d194-4597-ba68-f1a70e4f890b
//        CLR Version:              4.0.30319.18444
//        Name:                     RibbonToolBarControlGroup
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Ribbon
//        File Name:                RibbonToolBarControlGroup
//
//        created by Charley at 2014/5/27 22:22:32
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace VoiceCyber.Ribbon
{
    /// <summary>
    /// Represent logical container for toolbar items
    /// </summary>
    [ContentProperty("Children")]
    public class RibbonToolBarControlGroup : ItemsControl
    {
        #region Properties

        /// <summary>
        /// Gets whether the group is the fisrt control in the row
        /// </summary>
        public bool IsFirstInRow
        {
            get { return (bool)GetValue(IsFirstInRowProperty); }
            set { SetValue(IsFirstInRowProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsFirstInRow.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsFirstInRowProperty =
            DependencyProperty.Register("IsFirstInRow", typeof(bool), typeof(RibbonToolBarControlGroup), new UIPropertyMetadata(true));

        /// <summary>
        /// Gets whether the group is the last control in the row
        /// </summary>
        public bool IsLastInRow
        {
            get { return (bool)GetValue(IsLastInRowProperty); }
            set { SetValue(IsLastInRowProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsFirstInRow.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsLastInRowProperty =
            DependencyProperty.Register("IsLastInRow", typeof(bool), typeof(RibbonToolBarControlGroup), new UIPropertyMetadata(true));

        #endregion

        #region Initialization

        [SuppressMessage("Microsoft.Performance", "CA1810")]
        static RibbonToolBarControlGroup()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonToolBarControlGroup), new FrameworkPropertyMetadata(typeof(RibbonToolBarControlGroup)));
            StyleProperty.OverrideMetadata(typeof(RibbonToolBarControlGroup), new FrameworkPropertyMetadata(null, new CoerceValueCallback(OnCoerceStyle)));
        }

        // Coerce object style
        static object OnCoerceStyle(DependencyObject d, object basevalue)
        {
            if (basevalue == null)
            {
                basevalue = (d as FrameworkElement).TryFindResource(typeof(RibbonToolBarControlGroup));
            }

            return basevalue;
        }

        #endregion
    }
}
