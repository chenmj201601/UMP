//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    0c9a87e4-1fbb-4cf7-98b2-0956a1b07f8e
//        CLR Version:              4.0.30319.18444
//        Name:                     CellOperationVisibilityConverter
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS3101.Converters
//        File Name:                CellOperationVisibilityConverter
//
//        created by Charley at 2014/10/14 10:37:41
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using UMPS3101.Models;
using VoiceCyber.UMP.Common;

namespace UMPS3101.Converters
{
    public class CellOperationVisibilityConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var scoreSheetItem = value as ScoreSheetItem;
            if (scoreSheetItem == null)
            {
                return Visibility.Collapsed;
            }
            string optID = parameter.ToString();
            var listOpts = SSMMainView.ListOperations;
            OperationInfo item = listOpts.FirstOrDefault(o => o.ID.ToString() == (string)parameter);
            if (item == null)
            {
                return Visibility.Collapsed;
            }
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
