//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    d6cd99ce-0ada-45a6-993e-53c9749b0bc6
//        CLR Version:              4.0.30319.18063
//        Name:                     AlarmLevelConverter
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS2501.Converters
//        File Name:                AlarmLevelConverter
//
//        created by Charley at 2015/5/21 18:14:41
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Windows.Data;
using VoiceCyber.UMP.Common25011;

namespace UMPS2501.Converters
{
    public class AlarmLevelConverter:IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string strReturn = string.Empty;
            int intValue = (int)value;
            switch (intValue)
            {
                case S2501Consts.ALARM_LEVEL_NORMAL:
                    //strReturn = App.GetLanguageInfo(string.Format("BID{0}{1}", S2501Consts.BID_ALARM_LEVEL, intValue.ToString("000")), "Normal");
                    break;
                case S2501Consts.ALARM_LEVEL_LOW:
                    //strReturn = App.GetLanguageInfo(string.Format("BID{0}{1}", S2501Consts.BID_ALARM_LEVEL, intValue.ToString("000")), "Low");
                    break;
                case S2501Consts.ALARM_LEVEL_MID:
                   //strReturn = App.GetLanguageInfo(string.Format("BID{0}{1}", S2501Consts.BID_ALARM_LEVEL, intValue.ToString("000")), "Middle");
                    break;
                case S2501Consts.ALARM_LEVEL_HIGH:
                    //strReturn = App.GetLanguageInfo(string.Format("BID{0}{1}", S2501Consts.BID_ALARM_LEVEL, intValue.ToString("000")), "High");
                    break;
            }
            return strReturn;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
