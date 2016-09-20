//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    48a20636-cbc6-4070-ae97-ab815e1cfb26
//        CLR Version:              4.0.30319.18444
//        Name:                     ContextMenuUtilities
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Wpf.PropertyGrids.Utilities
//        File Name:                ContextMenuUtilities
//
//        created by Charley at 2014/7/23 16:44:50
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace VoiceCyber.Wpf.PropertyGrids.Utilities
{
    public class ContextMenuUtilities
    {
        public static readonly DependencyProperty OpenOnMouseLeftButtonClickProperty = DependencyProperty.RegisterAttached("OpenOnMouseLeftButtonClick", typeof(bool), typeof(ContextMenuUtilities), new FrameworkPropertyMetadata(false, OpenOnMouseLeftButtonClickChanged));
        public static void SetOpenOnMouseLeftButtonClick(FrameworkElement element, bool value)
        {
            element.SetValue(OpenOnMouseLeftButtonClickProperty, value);
        }
        public static bool GetOpenOnMouseLeftButtonClick(FrameworkElement element)
        {
            return (bool)element.GetValue(OpenOnMouseLeftButtonClickProperty);
        }

        public static void OpenOnMouseLeftButtonClickChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var control = (FrameworkElement)sender;
            if ((bool)e.NewValue)
            {
                control.PreviewMouseLeftButtonDown += (s, args) =>
                {
                    if (control.ContextMenu != null)
                    {
                        control.ContextMenu.PlacementTarget = control;
                        control.ContextMenu.IsOpen = true;
                    }
                };
            }
            //TODO: remove handler when set to false
        }
    }
}
