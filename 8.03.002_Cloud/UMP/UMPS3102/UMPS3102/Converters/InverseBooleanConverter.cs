//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    a37ec91d-ec74-465a-8d58-0a4419d9bbfe
//        CLR Version:              4.0.30319.18444
//        Name:                     InverseBooleanConverter
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS3102.Converters
//        File Name:                InverseBooleanConverter
//
//        created by Charley at 2014/11/8 13:30:54
//        http://www.voicecyber.com 
//
//======================================================================

using System.Windows.Data;

namespace UMPS3102.Converters
{
    public class InverseBooleanConverter : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool bValue;
            if (bool.TryParse(value.ToString(), out bValue))
            {
                return !bValue;
            }
            return true;
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new System.NotImplementedException();
        }
    }
}
