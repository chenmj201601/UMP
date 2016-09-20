using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using UMPS3108.Models;

namespace UMPS3108.Converters
{
    class ABCDItemConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var conditionItem = value as StatisticalParamItemModel;
            if (conditionItem != null)
            {
                ABCDItem uc = new ABCDItem();
                uc.StatisticItemModel = conditionItem;
                conditionItem.abcdItem = uc;
                return uc;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
