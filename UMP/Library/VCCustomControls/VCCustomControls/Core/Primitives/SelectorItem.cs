//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    d21a09a5-2050-485d-9d13-d460583e8985
//        CLR Version:              4.0.30319.18444
//        Name:                     SelectorItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.CustomControls.Core.Primitives
//        File Name:                SelectorItem
//
//        created by Charley at 2014/7/18 15:49:58
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace VoiceCyber.Wpf.CustomControls.Primitives
{
    public class SelectorItem : ContentControl
    {
        #region Constructors

        static SelectorItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SelectorItem), new FrameworkPropertyMetadata(typeof(SelectorItem)));
        }

        #endregion //Constructors

        #region Properties

        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register("IsSelected", typeof(bool), typeof(SelectorItem), new UIPropertyMetadata(false, OnIsSelectedChanged));
        public bool IsSelected
        {
            get
            {
                return (bool)GetValue(IsSelectedProperty);
            }
            set
            {
                SetValue(IsSelectedProperty, value);
            }
        }

        private static void OnIsSelectedChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            SelectorItem selectorItem = o as SelectorItem;
            if (selectorItem != null)
                selectorItem.OnIsSelectedChanged((bool)e.OldValue, (bool)e.NewValue);
        }

        protected virtual void OnIsSelectedChanged(bool oldValue, bool newValue)
        {
            if (newValue)
                this.RaiseEvent(new RoutedEventArgs(Selector.SelectedEvent, this));
            else
                this.RaiseEvent(new RoutedEventArgs(Selector.UnSelectedEvent, this));
        }

        internal Selector ParentSelector
        {
            get
            {
                return ItemsControl.ItemsControlFromItemContainer(this) as Selector;
            }
        }

        #endregion //Properties

        #region Events

        public static readonly RoutedEvent SelectedEvent = Selector.SelectedEvent.AddOwner(typeof(SelectorItem));
        public static readonly RoutedEvent UnselectedEvent = Selector.UnSelectedEvent.AddOwner(typeof(SelectorItem));

        #endregion
    }
}
