//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    3349276a-f56c-42b6-aedc-5564307bad0e
//        CLR Version:              4.0.30319.18444
//        Name:                     LevelToMarginConverter
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.CustomControls.Core.Converters
//        File Name:                LevelToMarginConverter
//
//        created by Charley at 2014/7/17 15:27:08
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace VoiceCyber.Wpf.CustomControls.Core.Converters
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
