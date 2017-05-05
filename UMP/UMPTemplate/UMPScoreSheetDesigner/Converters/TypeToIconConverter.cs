//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    f041473a-8bc8-4809-9785-42f444528306
//        CLR Version:              4.0.30319.18444
//        Name:                     TypeToIconConverter
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPScoreSheetDesigner.Converters
//        File Name:                TypeToIconConverter
//
//        created by Charley at 2014/7/30 13:29:46
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;
using VoiceCyber.UMP.ScoreSheets;

namespace UMPScoreSheetDesigner.Converters
{
    public class TypeToIconConverter:IValueConverter
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
                    case  ScoreObjectType.StandardItem:
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
