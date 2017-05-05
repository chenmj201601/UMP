using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using UMPS3102.Models;

namespace UMPS3102.Converters
{
    public class DirectionToHorizontalAlignmentConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //var conversationInfoItem = value as ConversationInfoItem;
            //if (conversationInfoItem != null)
            //{
            //    if (conversationInfoItem.Direction == "0")
            //    {
            //        return HorizontalAlignment.Right;
            //    }
            //    if (conversationInfoItem.Direction == "1")
            //    {
            //        return HorizontalAlignment.Left;
            //    }
            //}
            //return null;
            return value.ToString() == "1" ? HorizontalAlignment.Left : HorizontalAlignment.Right;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
