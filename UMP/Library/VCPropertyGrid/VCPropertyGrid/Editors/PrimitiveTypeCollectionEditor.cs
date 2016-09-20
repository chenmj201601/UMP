//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    d9b10f3b-a4f6-44f6-8b74-448e857dbc07
//        CLR Version:              4.0.30319.18444
//        Name:                     PrimitiveTypeCollectionEditor
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Wpf.PropertyGrids.Editors
//        File Name:                PrimitiveTypeCollectionEditor
//
//        created by Charley at 2014/7/23 12:02:17
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;
using VoiceCyber.Wpf.CustomControls;

namespace VoiceCyber.Wpf.PropertyGrids.Editors
{
    public class PrimitiveTypeCollectionEditor : TypeEditor<PrimitiveTypeCollectionControl>
    {
        protected override void SetControlProperties()
        {
            Editor.BorderThickness = new System.Windows.Thickness(0);
            Editor.Content = "(Collection)";
        }

        protected override void SetValueDependencyProperty()
        {
            ValueProperty = PrimitiveTypeCollectionControl.ItemsSourceProperty;
        }

        protected override void ResolveValueBinding(PropertyItem propertyItem)
        {
            var type = propertyItem.PropertyType;
            Editor.ItemsSourceType = type;

            if (type.BaseType == typeof(System.Array))
            {
                Editor.ItemType = type.GetElementType();
            }
            else
            {
                Editor.ItemType = type.GetGenericArguments()[0];
            }

            base.ResolveValueBinding(propertyItem);
        }
    }
}
