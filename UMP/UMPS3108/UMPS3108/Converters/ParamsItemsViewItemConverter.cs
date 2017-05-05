using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using UMPS3108.Models;
using VoiceCyber.UMP.Common31081;

namespace UMPS3108.Converters
{
    public class ParamsItemsViewItemConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var conditionItem = value as CombinedParamItemModel;
            if (conditionItem != null)
            {
                ParamItemViewItem uc = new ParamItemViewItem();
                uc.CombinedParamItem = conditionItem;
                conditionItem.ParamItemViewItem_ = uc;
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
