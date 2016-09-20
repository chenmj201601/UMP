//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    de1a9f51-68f3-40d0-9c4e-3d42b2642da7
//        CLR Version:              4.0.30319.18444
//        Name:                     BoolToVisibilityConverter
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.CustomControls.Core.Converters
//        File Name:                BoolToVisibilityConverter
//
//        created by Charley at 2014/7/17 15:26:19
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
}
