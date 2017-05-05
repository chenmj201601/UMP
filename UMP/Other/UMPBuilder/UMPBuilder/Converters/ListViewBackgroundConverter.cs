//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    97b6869c-9b46-41ad-84b1-c16263f2273f
//        CLR Version:              4.0.30319.18063
//        Name:                     ListViewBackgroundConverter
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPBuilder.Converters
//        File Name:                ListViewBackgroundConverter
//
//        created by Charley at 2015/12/23 12:38:21
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace UMPBuilder.Converters
{
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
