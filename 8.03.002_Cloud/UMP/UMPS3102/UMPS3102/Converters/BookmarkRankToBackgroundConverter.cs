//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    5b2b4c55-4adb-4c2c-94be-e05b29f2120c
//        CLR Version:              4.0.30319.18444
//        Name:                     BookmarkRankToBackgroundConverter
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS3102.Converters
//        File Name:                BookmarkRankToBackgroundConverter
//
//        created by Charley at 2014/12/11 14:04:37
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Windows.Data;
using System.Windows.Media;
using UMPS3102.Models;

namespace UMPS3102.Converters
{
    public class BookmarkRankToBackgroundConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var rank = value as BookmarkRankItem;
            if (rank != null)
            {
                string color = rank.Color;
                try
                {
                    string r = color.Substring(0, 2);
                    string g = color.Substring(2, 2);
                    string b = color.Substring(4, 2);
                    Color temp = Color.FromRgb((byte)System.Convert.ToInt32(r, 16), (byte)System.Convert.ToInt32(g, 16),
                        (byte)System.Convert.ToInt32(b, 16));
                    return new SolidColorBrush(temp);
                }
                catch{}
            }
            return Brushes.White;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
