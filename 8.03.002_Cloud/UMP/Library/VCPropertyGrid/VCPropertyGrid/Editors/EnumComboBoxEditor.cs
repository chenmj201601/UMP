//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    dfc40908-5b2a-4bdb-8633-9a163c26be21
//        CLR Version:              4.0.30319.18444
//        Name:                     EnumComboBoxEditor
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Wpf.PropertyGrids.Editors
//        File Name:                EnumComboBoxEditor
//
//        created by Charley at 2014/7/23 12:00:32
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace VoiceCyber.Wpf.PropertyGrids.Editors
{
    public class EnumComboBoxEditor : ComboBoxEditor
    {
        protected override IEnumerable CreateItemsSource(PropertyItem propertyItem)
        {
            return GetValues(propertyItem.PropertyType);
        }

        private static object[] GetValues(Type enumType)
        {
            List<object> values = new List<object>();

            if (enumType != null)
            {
                var fields = enumType.GetFields().Where(x => x.IsLiteral);
                foreach (FieldInfo field in fields)
                {
                    // Get array of BrowsableAttribute attributes
                    object[] attrs = field.GetCustomAttributes(typeof(BrowsableAttribute), false);
                    if (attrs.Length == 1)
                    {
                        // If attribute exists and its value is false continue to the next field...
                        BrowsableAttribute brAttr = (BrowsableAttribute)attrs[0];
                        if (brAttr.Browsable == false)
                            continue;
                    }

                    values.Add(field.GetValue(enumType));
                }
            }

            return values.ToArray();
        }
    }
}
