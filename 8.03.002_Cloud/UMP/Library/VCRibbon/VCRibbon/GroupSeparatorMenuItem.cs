//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    7b156741-34e3-46a4-bcf2-4c4b09f2a829
//        CLR Version:              4.0.30319.18444
//        Name:                     GroupSeparatorMenuItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Ribbon
//        File Name:                GroupSeparatorMenuItem
//
//        created by Charley at 2014/5/27 22:50:41
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Markup;

namespace VoiceCyber.Ribbon
{
    /// <summary>
    /// Represents group separator menu item
    /// </summary>
    [ContentProperty("Header")]
    public class GroupSeparatorMenuItem : MenuItem
    {
        [SuppressMessage("Microsoft.Performance", "CA1810")]
        static GroupSeparatorMenuItem()
        {
            Type type = typeof(GroupSeparatorMenuItem);
            DefaultStyleKeyProperty.OverrideMetadata(type, new FrameworkPropertyMetadata(type));
            StyleProperty.OverrideMetadata(type, new FrameworkPropertyMetadata(null, new CoerceValueCallback(OnCoerceStyle)));
            IsEnabledProperty.OverrideMetadata(type, new FrameworkPropertyMetadata(false, null, CoerceIsEnabledAndTabStop));
            IsTabStopProperty.OverrideMetadata(type, new FrameworkPropertyMetadata(false, null, CoerceIsEnabledAndTabStop));
        }

        static object CoerceIsEnabledAndTabStop(DependencyObject d, object basevalue)
        {
            return false;
        }

        // Coerce object style
        static object OnCoerceStyle(DependencyObject d, object basevalue)
        {
            if (basevalue == null)
            {
                basevalue = (d as FrameworkElement).TryFindResource(typeof(GroupSeparatorMenuItem));
            }

            return basevalue;
        }
    }
}
