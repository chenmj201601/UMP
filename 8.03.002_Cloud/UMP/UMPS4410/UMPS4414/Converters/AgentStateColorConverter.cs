//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    ba9fff6c-6f49-429d-afd5-7132156b7403
//        CLR Version:              4.0.30319.18408
//        Name:                     AgentStateColorConverter
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS4414.Converters
//        File Name:                AgentStateColorConverter
//
//        created by Charley at 2016/6/23 11:43:10
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Windows.Data;
using System.Windows.Media;
using UMPS4414.Models;


namespace UMPS4414.Converters
{
    public class AgentStateColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var item = value as ObjItem;
            if (item == null)
            {
                return null;
            }
            SolidColorBrush brush = new SolidColorBrush();
            brush.Color = GetColorFromString(item.StrColor);
            return brush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private Color GetColorFromString(string strColor)
        {
            Color color = Brushes.Transparent.Color;
            try
            {
                string strA = strColor.Substring(1, 2);
                string strR = strColor.Substring(3, 2);
                string strG = strColor.Substring(5, 2);
                string strB = strColor.Substring(7, 2);
                color = Color.FromArgb((byte)System.Convert.ToInt32(strA, 16), (byte)System.Convert.ToInt32(strR, 16), (byte)System.Convert.ToInt32(strG, 16),
                    (byte)System.Convert.ToInt32(strB, 16));
            }
            catch { }
            return color;
        }
    }
}
