//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    ca082473-8148-409f-b097-b02fcbabd1e9
//        CLR Version:              4.0.30319.18063
//        Name:                     BackgroundConverter
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPLanguageManager.Converters
//        File Name:                BackgroundConverter
//
//        created by Charley at 2015/6/5 13:26:57
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace UMPLanguageManager.Converters
{
    public class BackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
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
                return Brushes.WhiteSmoke;
            }
            return Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
