//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    66f0fe90-1c0a-42f7-89b0-9ec089faf762
//        CLR Version:              4.0.30319.18444
//        Name:                     EnumConverter
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPScoreSheetDesigner.Converters
//        File Name:                EnumConverter
//
//        created by Charley at 2014/7/29 11:48:22
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using UMPScoreSheetDesigner.Models;

namespace UMPScoreSheetDesigner.Converters
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
