//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    f70840dd-fc8b-46b9-a520-c74e6c186f52
//        CLR Version:              4.0.30319.18444
//        Name:                     ScoreSheetViewClassConverter
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS3101.Converters
//        File Name:                ScoreSheetViewClassConverter
//
//        created by Charley at 2014/10/14 9:59:36
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
    public class ScoreSheetViewClassConverter:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }
            BitmapImage image = new BitmapImage();
            string vc = value.ToString();
            switch (vc)
            {
                case "0":
                    image.BeginInit();
                    image.UriSource = new Uri("Images/vc_tree.ico", UriKind.Relative);
                    image.EndInit();
                    return image;
                case "1":
                    image.BeginInit();
                    image.UriSource = new Uri("Images/vc_table.ico", UriKind.Relative);
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
