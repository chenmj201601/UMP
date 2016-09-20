//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    407b8608-2e61-447d-a7b1-3683b997fb8a
//        CLR Version:              4.0.30319.18444
//        Name:                     CheckBoxEditor
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Wpf.PropertyGrids.Editors
//        File Name:                CheckBoxEditor
//
//        created by Charley at 2014/7/23 11:58:15
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace VoiceCyber.Wpf.PropertyGrids.Editors
{
    public class CheckBoxEditor : TypeEditor<CheckBox>
    {
        protected override CheckBox CreateEditor()
        {
            return new PropertyGridEditorCheckBox();
        }

        protected override void SetControlProperties()
        {
            Editor.Margin = new Thickness(5, 0, 0, 0);
        }

        protected override void SetValueDependencyProperty()
        {
            ValueProperty = CheckBox.IsCheckedProperty;
        }
    }

    public class PropertyGridEditorCheckBox : CheckBox
    {
        static PropertyGridEditorCheckBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyGridEditorCheckBox), new FrameworkPropertyMetadata(typeof(PropertyGridEditorCheckBox)));
        }
    }
}
