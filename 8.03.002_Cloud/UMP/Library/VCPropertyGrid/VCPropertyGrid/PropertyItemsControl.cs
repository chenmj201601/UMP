//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    9c651b02-4371-4550-8164-deb8447725d5
//        CLR Version:              4.0.30319.18444
//        Name:                     PropertyItemsControl
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Wpf.PropertyGrids
//        File Name:                PropertyItemsControl
//
//        created by Charley at 2014/7/23 12:10:22
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace VoiceCyber.Wpf.PropertyGrids
{
    /// <summary>
    /// This Control is intended to be used in the template of the 
    /// PropertyItemBase and PropertyGrid classes to contain the
    /// sub-children properties.
    /// </summary>
    public class PropertyItemsControl : ItemsControl
    {

        #region PreparePropertyItemEvent Attached Routed Event

        internal static readonly RoutedEvent PreparePropertyItemEvent = EventManager.RegisterRoutedEvent("PreparePropertyItem", RoutingStrategy.Bubble, typeof(PropertyItemEventHandler), typeof(PropertyItemsControl));
        internal event PropertyItemEventHandler PreparePropertyItem
        {
            add
            {
                AddHandler(PropertyItemsControl.PreparePropertyItemEvent, value);
            }
            remove
            {
                RemoveHandler(PropertyItemsControl.PreparePropertyItemEvent, value);
            }
        }

        private void RaisePreparePropertyItemEvent(PropertyItemBase propertyItem, object item)
        {
            this.RaiseEvent(new PropertyItemEventArgs(PropertyItemsControl.PreparePropertyItemEvent, this, propertyItem, item));
        }

        #endregion

        #region ClearPropertyItemEvent Attached Routed Event

        internal static readonly RoutedEvent ClearPropertyItemEvent = EventManager.RegisterRoutedEvent("ClearPropertyItem", RoutingStrategy.Bubble, typeof(PropertyItemEventHandler), typeof(PropertyItemsControl));
        internal event PropertyItemEventHandler ClearPropertyItem
        {
            add
            {
                AddHandler(PropertyItemsControl.ClearPropertyItemEvent, value);
            }
            remove
            {
                RemoveHandler(PropertyItemsControl.ClearPropertyItemEvent, value);
            }
        }

        private void RaiseClearPropertyItemEvent(PropertyItemBase propertyItem, object item)
        {
            this.RaiseEvent(new PropertyItemEventArgs(PropertyItemsControl.ClearPropertyItemEvent, this, propertyItem, item));
        }

        #endregion

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return (item is PropertyItemBase);
        }


        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            base.PrepareContainerForItemOverride(element, item);
            this.RaisePreparePropertyItemEvent((PropertyItemBase)element, item);
        }

        protected override void ClearContainerForItemOverride(DependencyObject element, object item)
        {
            this.RaiseClearPropertyItemEvent((PropertyItemBase)element, item);
            base.ClearContainerForItemOverride(element, item);
        }
    }
}
