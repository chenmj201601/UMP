using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace UMPS1102.Converters
{
    public class EnabledStatusConverter : IValueConverter
    {
        #region
        public object Convert(object value, Type targetType, object paramter, System.Globalization.CultureInfo culture) 
        {

            if (value.ToString().Equals("1"))
            {
                return App.CurrentApp.GetLanguageInfo("1102T00021", "Active");
            }
            else
            {
                return App.CurrentApp.GetLanguageInfo("1102T00022", "Disable");
            }
        }


        public object ConvertBack(object value, Type targetType, object paramter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
