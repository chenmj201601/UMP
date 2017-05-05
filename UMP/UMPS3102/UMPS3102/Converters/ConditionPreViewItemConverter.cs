//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    93b1c27b-a784-4d57-8e38-0e7859f53991
//        CLR Version:              4.0.30319.18444
//        Name:                     ConditionPreViewItemConverter
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS3102.Converters
//        File Name:                ConditionPreViewItemConverter
//
//        created by Charley at 2014/11/23 13:17:29
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Windows.Data;
using UMPS3102.Models;

namespace UMPS3102.Converters
{
    public class ConditionPreViewItemConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var conditionItem = value as ConditionItemItem;
            if (conditionItem != null)
            {
                UCConditionPreViewItem uc = new UCConditionPreViewItem();
                uc.ConditionItemItem = conditionItem;
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
