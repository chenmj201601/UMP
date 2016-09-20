//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    719e7704-9466-45f9-97ea-8a774e5ce165
//        CLR Version:              4.0.30319.18444
//        Name:                     TypeToIconConverter
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS3101.Converters
//        File Name:                TypeToIconConverter
//
//        created by Charley at 2014/10/14 17:33:06
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;
using VoiceCyber.UMP.ScoreSheets;

namespace UMPS3101.Converters
{
    public class TypeToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return string.Empty;
            }
            ScoreObjectType objType;
            if (Enum.TryParse(value.ToString(), out objType))
            {
                switch (objType)
                {
                    case ScoreObjectType.StandardItem:
                        return string.Format("Images/standarditem.ico");
                    case ScoreObjectType.CommentItem:
                        return string.Format("Images/commentitem.ico");
                    case ScoreObjectType.ControlItem:
                        return string.Format("Images/controlitem.png");
                }
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
