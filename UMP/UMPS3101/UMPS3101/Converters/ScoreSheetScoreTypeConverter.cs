//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    fbdd3ebe-2328-458c-a75a-36a4959c9210
//        CLR Version:              4.0.30319.18444
//        Name:                     ScoreSheetScoreTypeConverter
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS3101.Converters
//        File Name:                ScoreSheetScoreTypeConverter
//
//        created by Charley at 2014/10/14 10:20:30
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace UMPS3101.Converters
{
    public class ScoreSheetScoreTypeConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }
            BitmapImage image = new BitmapImage();
            string st = value.ToString();
            switch (st)
            {
                case "0":
                    image.BeginInit();
                    image.UriSource = new Uri("Images/st_numeric.ico", UriKind.Relative);
                    image.EndInit();
                    return image;
                case "1":
                    image.BeginInit();
                    image.UriSource = new Uri("Images/st_percentage.ico", UriKind.Relative);
                    image.EndInit();
                    return image;
                case "2":
                    image.BeginInit();
                    image.UriSource = new Uri("Images/st_allyesno.ico", UriKind.Relative);
                    image.EndInit();
                    return image;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
