//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    87d43db9-2703-41f9-867c-03d211457d51
//        CLR Version:              4.0.30319.18444
//        Name:                     IconNameToScoreSettingConverter
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS3101.Converters
//        File Name:                IconNameToScoreSettingConverter
//
//        created by Charley at 2014/10/14 17:31:07
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

namespace UMPS3101.Converters
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
