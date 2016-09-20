//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    75f23215-4d9b-48de-8b40-25dfa3fb8a8d
//        CLR Version:              4.0.30319.18444
//        Name:                     CustomPropertyItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Wpf.PropertyGrids
//        File Name:                CustomPropertyItem
//
//        created by Charley at 2014/7/23 12:05:32
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace VoiceCyber.Wpf.PropertyGrids
{
    /// <summary>
    /// Used when properties are provided using a list source of items (eg. Properties or PropertiesSource). 
    /// 
    /// An instance of this class can be used as an item to easily customize the 
    /// display of the property directly by modifying the values of this class 
    /// (e.g., DisplayName, value, Category, etc.).
    /// </summary>
    public class CustomPropertyItem : PropertyItemBase
    {
        internal CustomPropertyItem() : base() { }

        #region Category

        public static readonly DependencyProperty CategoryProperty =
            DependencyProperty.Register("Category", typeof(string), typeof(CustomPropertyItem), new UIPropertyMetadata(null));

        public string Category
        {
            get { return (string)GetValue(CategoryProperty); }
            set { SetValue(CategoryProperty, value); }
        }

        #endregion //Category

        #region Value

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(object), typeof(CustomPropertyItem), new UIPropertyMetadata(null, OnValueChanged, OnCoerceValueChanged));
        public object Value
        {
            get
            {
                return (object)GetValue(ValueProperty);
            }
            set
            {
                SetValue(ValueProperty, value);
            }
        }

        private static object OnCoerceValueChanged(DependencyObject o, object baseValue)
        {
            CustomPropertyItem prop = o as CustomPropertyItem;
            if (prop != null)
                return prop.OnCoerceValueChanged(baseValue);

            return baseValue;
        }

        protected virtual object OnCoerceValueChanged(object baseValue)
        {
            return baseValue;
        }

        private static void OnValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            CustomPropertyItem propertyItem = o as CustomPropertyItem;
            if (propertyItem != null)
            {
                propertyItem.OnValueChanged((object)e.OldValue, (object)e.NewValue);
            }
        }

        protected virtual void OnValueChanged(object oldValue, object newValue)
        {
            if (IsInitialized)
            {
                RaiseEvent(new PropertyValueChangedEventArgs(PropertyGrid.PropertyValueChangedEvent, this, oldValue, newValue));
            }
        }

        #endregion //Value
    }
}
