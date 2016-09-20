using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace UMPS1102.Converters
{
    public class IsDeleteStatusConvert : IValueConverter
    {
        #region
        public object Convert(object value, Type targetType, object paramter, System.Globalization.CultureInfo culture)
        {
            if (value.ToString().Equals("1"))
            {
                return  App.CurrentApp.GetLanguageInfo("1102T00019", "Yes");
            }
            else
            {
                return App.CurrentApp.GetLanguageInfo("1102T00020", "No");
            }
        }


        public object ConvertBack(object value, Type targetType, object paramter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
