//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    1c8f0aed-f59c-40f6-aea2-91e18debedf1
//        CLR Version:              4.0.30319.18444
//        Name:                     EnumConverter
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS3101.Converters
//        File Name:                EnumConverter
//
//        created by Charley at 2014/10/14 17:28:09
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using UMPS3101.Models;

namespace UMPS3101.Converters
{
    public class EnumConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }
            var list = parameter as List<EnumItem>;
            if (list != null)
            {
                EnumItem temp = list.FirstOrDefault(e => e.Name == value.ToString());
                if (temp != null)
                {
                    temp.IsSelected = true;
                    return temp;
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            EnumItem enumItem = value as EnumItem;
            if (enumItem == null)
            {
                return 0;
            }
            var enumValue = Enum.Parse(enumItem.Type, enumItem.Name);
            return enumValue;
        }
    }
}
