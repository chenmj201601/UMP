using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace PFShareControls
{
    public class FeatureMagnetIcoChangeConverter : IValueConverter
    {
        public object Convert(object AObjValue, Type ATypeTarget, object AObjParameter, System.Globalization.CultureInfo ACultureInfo)
        {
            if (AObjValue == null) { return null; }
            return new BitmapImage(new Uri(AObjValue.ToString(), UriKind.Absolute));
        }

        public object ConvertBack(object AObjValue, Type ATypeTarget, object AObjParameter, System.Globalization.CultureInfo ACultureInfo)
        {
            return null;
        }
    }

    public class VisibilityPropertyConvert : IValueConverter
    {
        public object Convert(object AObjValue, Type ATypeTarget, object AObjParameter, System.Globalization.CultureInfo ACultureInfo)
        {
            if (AObjValue == null) { return null; }

            return ((int)AObjValue == 0) ? Visibility.Collapsed : Visibility.Visible;

        }

        public object ConvertBack(object AObjValue, Type ATypeTarget, object AObjParameter, System.Globalization.CultureInfo ACultureInfo)
        {
            return null;
        }
    }
}
