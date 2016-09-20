//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    a4457298-173a-42c6-acdb-e0fa0f51988f
//        CLR Version:              4.0.30319.18063
//        Name:                     Converter
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Controls
//        File Name:                Converter
//
//        created by Charley at 2015/7/2 10:20:12
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace VoiceCyber.UMP.Controls
{
    /// <summary>
    /// 是非到可见性的转化
    /// 只有真才会转化成可见，假或空都是不可见
    /// </summary>
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return Visibility.Collapsed;
            }
            bool bValue = (bool)value;
            if (bValue)
            {
                return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// 是非到可见性的转化
    /// 只有真才会转化成不可见，假或空都是可见
    /// </summary>
    public class InverseBoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return Visibility.Visible;
            }
            bool bValue = (bool)value;
            if (bValue)
            {
                return Visibility.Collapsed;
            }
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ListViewBackgroundConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                ListViewItem item = (ListViewItem)value;
                ListView listview = ItemsControl.ItemsControlFromItemContainer(item) as ListView;
                if (listview == null)
                {
                    return Brushes.Transparent;
                }
                int index = listview.ItemContainerGenerator.IndexFromContainer(item);
                if (index % 2 == 1)
                {
                    return Brushes.LightGray;
                }
                return Brushes.Transparent;
            }
            catch
            {
                return Brushes.Transparent;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
