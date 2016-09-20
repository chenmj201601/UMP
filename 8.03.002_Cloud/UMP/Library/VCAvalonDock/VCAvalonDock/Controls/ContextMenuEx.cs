//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    292c0bb9-2176-4930-ba5f-c0a7ecaca5d7
//        CLR Version:              4.0.30319.18444
//        Name:                     ContextMenuEx
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Wpf.AvalonDock.Controls
//        File Name:                ContextMenuEx
//
//        created by Charley at 2014/7/22 9:56:58
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows.Data;

namespace VoiceCyber.Wpf.AvalonDock.Controls
{
    public class ContextMenuEx : ContextMenu
    {
        static ContextMenuEx()
        {
        }

        public ContextMenuEx()
        {

        }

        protected override System.Windows.DependencyObject GetContainerForItemOverride()
        {
            return new MenuItemEx();
        }

        protected override void OnOpened(System.Windows.RoutedEventArgs e)
        {
            BindingOperations.GetBindingExpression(this, ItemsSourceProperty).UpdateTarget();

            base.OnOpened(e);
        }


    }
}
