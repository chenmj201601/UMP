//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    49fd527e-123c-4663-b189-bc3dbafdc16a
//        CLR Version:              4.0.30319.18444
//        Name:                     ObjectToUIElementConverter
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Wpf.PropertyGrids.Converters
//        File Name:                ObjectToUIElementConverter
//
//        created by Charley at 2014/7/23 11:56:11
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace VoiceCyber.Wpf.PropertyGrids.Converters
{
    public class ObjectToUIElementConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is UIElement)
                return value;

            return new Control();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
