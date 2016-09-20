//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    41476cb1-2fd0-47b3-b85b-5d9626f2ced5
//        CLR Version:              4.0.30319.18063
//        Name:                     LevelToMarginConverter
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS2501.Converters
//        File Name:                LevelToMarginConverter
//
//        created by Charley at 2015/5/28 10:18:26
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace UMPS2501.Converters
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
