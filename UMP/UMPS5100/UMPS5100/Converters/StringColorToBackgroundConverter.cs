using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Media;
using VoiceCyber.UMP.Common;

namespace UMPS5100.Converters
{
    public class StringColorToBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
           // return new SolidColorBrush(Utils.GetColorFromRgbString(value.ToString()));
            try
            {
                return new SolidColorBrush(Utils.GetColorFromRgbString(value.ToString()));
            }
            catch { }
            return Brushes.White;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
