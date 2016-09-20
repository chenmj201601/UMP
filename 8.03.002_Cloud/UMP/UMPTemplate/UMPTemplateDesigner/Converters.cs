//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    0a9c3870-b991-41aa-be4f-e4a701e97ef7
//        CLR Version:              4.0.30319.18444
//        Name:                     Converters
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPTemplateDesigner
//        File Name:                Converters
//
//        created by Charley at 2014/6/23 10:35:45
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Data;
using VoiceCyber.UMP.ScoreSheets;

namespace UMPTemplateDesigner
{
    public class InvalidCodeConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return string.Empty;
            }
            int intValue;
            if (int.TryParse(value.ToString(), out intValue))
            {
                if (intValue != 0)
                {
                    return string.Format("Images/invalid.png");
                }
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class MultLineTextConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return string.Empty;
            }
            string str = value.ToString();
            str = str.Replace("\r", "");
            str = str.Replace("\n", "");
            return str;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class IconNameToScoreSettingConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()))
            {
                return null;
            }
            ObservableCollection<ScoreSetting> settings = parameter as ObservableCollection<ScoreSetting>;
            if (settings == null)
            {
                return null;
            }
            ScoreSetting setting = settings.FirstOrDefault(s => s.Category == "I" && s.Code == value.ToString());
            if (setting == null)
            {
                return null;
            }
            return setting;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            ScoreSetting setting = value as ScoreSetting;
            if (setting == null)
            {
                return string.Empty;
            }
            return setting.Code;
        }
    }

    public class EnumConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return null;
            }
            var list = parameter as List<EnumItem>;
            if (list != null)
            {
                EnumItem temp = list.FirstOrDefault(e => e.Name == value.ToString());
                if (temp != null)
                {
                    temp.IsSelected = true;
                    return temp;
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            EnumItem enumItem = value as EnumItem;
            if (enumItem == null)
            {
                return 0;
            }
            var enumValue = Enum.Parse(enumItem.Type, enumItem.Name);
            return enumValue;
        }
    }

}
