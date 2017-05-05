using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;

namespace UMPS1102.Converters
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
                               System.Globalization.CultureInfo culture)
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
        public object ConvertBack(object o, Type type, object parameter,System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        private const double C_INDENT_SIZE = 15.0;
        #endregion
    }
}
