//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    6265907e-b2a6-4889-a5d7-bf2765201ac4
//        CLR Version:              4.0.30319.18063
//        Name:                     Converters
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.CustomControls
//        File Name:                Converters
//
//        created by Charley at 2014/4/5 16:40:38
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace VoiceCyber.CustomControls
{
    /// <summary>
    /// bool类型格式化Visibility
    /// </summary>
    public class BoolToVisibilityConverter : IValueConverter
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
                return Visibility.Collapsed;
            }
            bool boolValue;
            if (!bool.TryParse(value.ToString(), out boolValue))
            {
                return Visibility.Collapsed;
            }
            return boolValue ? Visibility.Visible : Visibility.Collapsed;
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
            return null;
        }
        #endregion
    }
    /// <summary>
    /// Level类型格式化Margin
    /// </summary>
    public class LevelToMarginConverter : IValueConverter
    {
        #region IValueConverter 成员
        /// <summary>
        /// Convert
        /// </summary>
        /// <param name="o"></param>
        /// <param name="type"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object o, Type type, object parameter,
                              CultureInfo culture)
        {
            return new Thickness((int)o * C_INDENT_SIZE, 0, 0, 0);
        }
        /// <summary>
        /// ConvertBack
        /// </summary>
        /// <param name="o"></param>
        /// <param name="type"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        public object ConvertBack(object o, Type type, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        private const double C_INDENT_SIZE = 15.0;
        #endregion
    }
}
