//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    66590859-948e-40b6-8168-1dc38e54a6ee
//        CLR Version:              4.0.30319.18444
//        Name:                     ColorToSolidColorBrushConverter
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.CustomControls.Core.Converters
//        File Name:                ColorToSolidColorBrushConverter
//
//        created by Charley at 2014/7/18 12:12:16
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;

namespace VoiceCyber.Wpf.CustomControls.Core.Converters
{
    public class ColorToSolidColorBrushConverter : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        /// Converts a Color to a SolidColorBrush.
        /// </summary>
        /// <param name="value">The Color produced by the binding source.</param>
        /// <param name="targetType">The type of the binding target property.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted SolidColorBrush. If the method returns null, the valid null value is used.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
                return new SolidColorBrush((Color)value);

            return value;
        }


        /// <summary>
        /// Converts a SolidColorBrush to a Color.
        /// </summary>
        /// <remarks>Currently not used in toolkit, but provided for developer use in their own projects</remarks>
        /// <param name="value">The SolidColorBrush that is produced by the binding target.</param>
        /// <param name="targetType">The type to convert to.</param>
        /// <param name="parameter">The converter parameter to use.</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
                return ((SolidColorBrush)value).Color;

            return value;
        }

        #endregion
    }
}
