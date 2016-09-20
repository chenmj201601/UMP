//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    1a9fc7dc-a358-4f5a-bf44-60a3fa999354
//        CLR Version:              4.0.30319.18444
//        Name:                     ContextMenuService
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Fluent
//        File Name:                ContextMenuService
//
//        created by Charley at 2014/5/27 17:22:50
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Windows;

namespace VoiceCyber.Ribbon
{
    /// <summary>
    /// Represents additional context menu service
    /// </summary>
    public static class ContextMenuService
    {
        /// <summary>
        /// Attach needed parameters to control
        /// </summary>
        /// <param name="type"></param>
        public static void Attach(Type type)
        {
            System.Windows.Controls.ContextMenuService.ShowOnDisabledProperty.OverrideMetadata(type, new FrameworkPropertyMetadata(true));
            FrameworkElement.ContextMenuProperty.OverrideMetadata(type, new FrameworkPropertyMetadata(null, OnContextMenuChanged, CoerceContextMenu));
        }

        private static void OnContextMenuChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            d.CoerceValue(FrameworkElement.ContextMenuProperty);
        }

        private static object CoerceContextMenu(DependencyObject d, object basevalue)
        {
            IQuickAccessItemProvider control = d as IQuickAccessItemProvider;
            if ((basevalue == null) && ((control == null) || control.CanAddToQuickAccessToolBar)) return Ribbon.RibbonContextMenu;
            return basevalue;
        }

        /// <summary>
        /// Coerce control context menu
        /// </summary>
        /// <param name="o">Control</param>
        public static void Coerce(DependencyObject o)
        {
            o.CoerceValue(FrameworkElement.ContextMenuProperty);
        }
    }
}
