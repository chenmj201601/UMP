//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    a353452d-0987-403d-bb2f-6278f1e28433
//        CLR Version:              4.0.30319.18444
//        Name:                     IconNameToScoreSettingConverter
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPScoreSheetDesigner.Converters
//        File Name:                IconNameToScoreSettingConverter
//
//        created by Charley at 2014/7/29 11:46:41
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Data;
using VoiceCyber.UMP.ScoreSheets;

namespace UMPScoreSheetDesigner.Converters
{
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
}
