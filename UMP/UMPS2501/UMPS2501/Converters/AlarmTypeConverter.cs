//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    31758c62-1809-466c-8a6e-0851ff22a7d0
//        CLR Version:              4.0.30319.18063
//        Name:                     AlarmTypeConverter
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS2501.Converters
//        File Name:                AlarmTypeConverter
//
//        created by Charley at 2015/5/21 17:56:22
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Windows.Data;
using VoiceCyber.UMP.Common25011;

namespace UMPS2501.Converters
{
    public class AlarmTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string strReturn = string.Empty;
            int intValue = (int) value;
            switch (intValue)
            {
                case S2501Consts.ALARM_TYPE_PROMPT:
                    //strReturn = App.GetLanguageInfo(string.Format("BID{0}{1}",S2501Consts.BID_ALARM_TYPE,intValue.ToString("000")), "Prompt");
                    break;
                case S2501Consts.ALARM_TYPE_NOTIFY:
                    //strReturn = App.GetLanguageInfo(string.Format("BID{0}{1}", S2501Consts.BID_ALARM_TYPE, intValue.ToString("000")), "Notify");
                    break;
                case S2501Consts.ALARM_TYPE_ALARM:
                    //strReturn = App.GetLanguageInfo(string.Format("BID{0}{1}", S2501Consts.BID_ALARM_TYPE, intValue.ToString("000")), "Alarm");
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
