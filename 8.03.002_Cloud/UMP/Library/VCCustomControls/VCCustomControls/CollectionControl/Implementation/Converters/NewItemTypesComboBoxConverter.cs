//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    99c33188-9b7e-4224-be53-414a85a60eb6
//        CLR Version:              4.0.30319.18444
//        Name:                     NewItemTypesComboBoxConverter
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.CustomControls.CollectionControl.Implementation.Converters
//        File Name:                NewItemTypesComboBoxConverter
//
//        created by Charley at 2014/7/21 10:34:38
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;
using VoiceCyber.Wpf.CustomControls.Core.Utilities;

namespace VoiceCyber.Wpf.CustomControls.Converters
{
    /// <summary>
    /// This multi-value converter is used in the CollectionControl template
    /// to determine the list of possible new item types that will be shown in the combo box.
    /// 
    /// If the second value (i.e., CollectionControl.NewItemTypes) is not null, this list will be used.
    /// Otherwise, if the first value (i.e., CollectionControl.ItemsSourceType) is a "IList&lt;T&gt;"
    /// type, the new item type list will contain "T".
    /// 
    /// </summary>
    public class NewItemTypesComboBoxConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {

            if (values.Length != 2)
                throw new ArgumentException("The 'values' argument should contain 2 objects.");

            if (values[1] != null)
            {
                if (!values[1].GetType().IsGenericType || !(values[1].GetType().GetGenericArguments().First().GetType() is Type))
                    throw new ArgumentException("The 'value' argument is not of the correct type.");

                return values[1];
            }
            else if (values[0] != null)
            {
                if (!(values[0].GetType() is Type))
                    throw new ArgumentException("The 'value' argument is not of the correct type.");

                List<Type> types = new List<Type>();
                Type listType = ListUtilities.GetListItemType((Type)values[0]);
                if (listType != null)
                {
                    types.Add(listType);
                }

                return types;
            }

            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
