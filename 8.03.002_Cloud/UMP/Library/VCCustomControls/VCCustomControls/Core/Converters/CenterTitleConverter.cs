//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    a9f91608-10da-43c5-bbad-33101f53bcc2
//        CLR Version:              4.0.30319.18444
//        Name:                     CenterTitleConverter
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Wpf.CustomControls.Core.Converters
//        File Name:                CenterTitleConverter
//
//        created by Charley at 2014/10/8 11:06:30
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace VoiceCyber.Wpf.CustomControls.Core.Converters
{
    /// <summary>
    /// 
    /// </summary>
    public class CenterTitleConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            // Parameters: DesiredSize, WindowWidth, HeaderColumns
            double titleTextWidth = ((Size)values[0]).Width;
            double windowWidth = (double)values[1];

            ColumnDefinitionCollection headerColumns = (ColumnDefinitionCollection)values[2];
            double titleColWidth = headerColumns[2].ActualWidth;
            double buttonsColWidth = headerColumns[3].ActualWidth;


            // Result (1) Title is Centered across all HeaderColumns
            if ((titleTextWidth + buttonsColWidth * 2) < windowWidth)
                return 1;

            // Result (2) Title is Centered in HeaderColumns[2]
            if (titleTextWidth < titleColWidth)
                return 2;

            // Result (3) Title is Left-Aligned in HeaderColumns[2]
            return 3;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
