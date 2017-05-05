using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using UMPS1102.Models;
using System.Windows;
using VoiceCyber.UMP.Common11021;

namespace UMPS1102.Converters
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
            var listOpts = RoleManage.ListOperations;
            ROperationInfo item = listOpts.FirstOrDefault(o => o.ID.ToString() == (string)parameter);
            if (item == null)
            {
                return Visibility.Collapsed;
            }
            switch (obj.ObjType)
            {
                case S1102Consts.OBJTYPE_ORG:
                    if (optID == S1102Consts.OPT_ADDORG.ToString()
                        || optID == S1102Consts.OPT_DELETEORG.ToString()
                        || optID == S1102Consts.OPT_MODIFYORG.ToString()
                        || optID == S1102Consts.OPT_ADDUSER.ToString())
                    {
                        return Visibility.Visible;
                    }
                    break;
                case S1102Consts.OBJTYPE_USER:
                    if (optID == S1102Consts.OPT_DELETEUSER.ToString()
                        || optID == S1102Consts.OPT_MODIFYUSER.ToString())
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
