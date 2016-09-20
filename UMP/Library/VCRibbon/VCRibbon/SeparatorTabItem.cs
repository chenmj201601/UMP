//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    a49ae5b1-b794-4a4b-b7f2-afe52cc3d902
//        CLR Version:              4.0.30319.18444
//        Name:                     SeparatorTabItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Ribbon
//        File Name:                SeparatorTabItem
//
//        created by Charley at 2014/5/27 22:17:07
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

namespace VoiceCyber.Ribbon
{
    /// <summary>
    /// Represents separator to use in the TabControl
    /// </summary>
    public class SeparatorTabItem : TabItem
    {
        #region Constructors

        /// <summary>
        /// Static constructor
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810")]
        static SeparatorTabItem()
        {
            Type type = typeof(SeparatorTabItem);
            DefaultStyleKeyProperty.OverrideMetadata(type, new FrameworkPropertyMetadata(type));
            IsEnabledProperty.OverrideMetadata(type, new FrameworkPropertyMetadata(false, null, CoerceIsEnabledAndTabStop));
            IsTabStopProperty.OverrideMetadata(type, new FrameworkPropertyMetadata(false, null, CoerceIsEnabledAndTabStop));
            IsSelectedProperty.OverrideMetadata(type, new FrameworkPropertyMetadata(false, OnIsSelectedChanged));
            StyleProperty.OverrideMetadata(typeof(SeparatorTabItem), new FrameworkPropertyMetadata(null, new CoerceValueCallback(OnCoerceStyle)));
        }

        // Coerce object style
        static object OnCoerceStyle(DependencyObject d, object basevalue)
        {
            if (basevalue == null)
            {
                basevalue = (d as FrameworkElement).TryFindResource(typeof(SeparatorTabItem));
            }

            return basevalue;
        }

        static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(bool)e.NewValue) return;
            SeparatorTabItem separatorTabItem = (SeparatorTabItem)d;
            TabControl tabControl = separatorTabItem.Parent as TabControl;
            if (tabControl == null || tabControl.Items.Count <= 1) return;
            tabControl.SelectedIndex = tabControl.SelectedIndex == tabControl.Items.Count - 1
                ? tabControl.SelectedIndex - 1 :
                  tabControl.SelectedIndex + 1;
        }

        static object CoerceIsEnabledAndTabStop(DependencyObject d, object basevalue)
        {
            return false;
        }

        #endregion
    }
}
