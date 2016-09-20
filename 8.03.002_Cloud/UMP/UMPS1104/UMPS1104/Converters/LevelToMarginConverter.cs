//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    9a4e6665-0de0-456e-a089-801076fb6f8b
//        CLR Version:              4.0.30319.18444
//        Name:                     LevelToMarginConverter
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS1101.Converters
//        File Name:                LevelToMarginConverter
//
//        created by Charley at 2014/8/27 10:57:15
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace UMPS1104.Converters
{
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
