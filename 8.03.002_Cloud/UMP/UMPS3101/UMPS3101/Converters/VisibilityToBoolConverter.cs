//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    88b08885-05cf-4dec-b126-6b68e4697d67
//        CLR Version:              4.0.30319.18444
//        Name:                     VisibilityToBoolConverter
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS3101.Converters
//        File Name:                VisibilityToBoolConverter
//
//        created by Charley at 2014/10/14 17:33:40
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace UMPS3101.Converters
{
    public class VisibilityToBoolConverter : IValueConverter
    {
        #region IValueConverter 成员
        /// <summary>
        /// Convert
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return false;
            }
            Visibility vis;
            if (Enum.TryParse(value.ToString(), out vis))
            {
                if (vis == Visibility.Visible)
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// ConvertBack
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return Visibility.Collapsed;
            }
            bool boolValue;
            if (!bool.TryParse(value.ToString(), out boolValue))
            {
                return Visibility.Collapsed;
            }
            return boolValue ? Visibility.Visible : Visibility.Collapsed;
        }
        #endregion
    }
}
