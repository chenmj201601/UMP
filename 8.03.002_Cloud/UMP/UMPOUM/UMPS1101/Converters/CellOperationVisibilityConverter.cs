//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    856fb5d8-17c5-47ed-838e-a6cd4a516135
//        CLR Version:              4.0.30319.18444
//        Name:                     CellOperationVisibilityConverter
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS1101.Converters
//        File Name:                CellOperationVisibilityConverter
//
//        created by Charley at 2014/8/27 13:53:08
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using UMPS1101.Models;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common11011;

namespace UMPS1101.Converters
{
    public class CellOperationVisibilityConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            ObjectItem obj = value as ObjectItem;
            if (obj == null)
            {
                return Visibility.Collapsed;
            }
            string optID = parameter.ToString();
            if (string.IsNullOrEmpty(optID))
            {
                return Visibility.Collapsed;
            }
            var listOpts = OUMMainView.ListOperations;
            OperationInfo item = listOpts.FirstOrDefault(o => o.ID.ToString() == (string)parameter);
            if (item == null)
            {
                return Visibility.Collapsed;
            }
            switch (obj.ObjType)
            {
                case S1101Consts.OBJTYPE_ORG:
                    if (optID == S1101Consts.OPT_ADDORG.ToString()
                        || optID == S1101Consts.OPT_DELETEORG.ToString()
                        || optID == S1101Consts.OPT_MODIFYORG.ToString()
                        || optID == S1101Consts.OPT_ADDUSER.ToString())
                    {
                        return Visibility.Visible;
                    }
                    break;
                case S1101Consts.OBJTYPE_USER:
                    if (optID == S1101Consts.OPT_DELETEUSER.ToString()
                        || optID == S1101Consts.OPT_MODIFYUSER.ToString()
                        || optID == S1101Consts.OPT_SETUSERROLE.ToString()
                        || optID == S1101Consts.OPT_SETUSERMANAGEMENT.ToString())
                    {
                        return Visibility.Visible;
                    }
                    break;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
