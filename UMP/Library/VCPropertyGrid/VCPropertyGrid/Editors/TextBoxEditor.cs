//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    85821c35-3b48-4dad-924c-5788c1b17b97
//        CLR Version:              4.0.30319.18444
//        Name:                     TextBoxEditor
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Wpf.PropertyGrids.Editors
//        File Name:                TextBoxEditor
//
//        created by Charley at 2014/7/23 12:02:52
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using VoiceCyber.Wpf.CustomControls;

namespace VoiceCyber.Wpf.PropertyGrids.Editors
{
    public class TextBoxEditor : TypeEditor<WatermarkTextBox>
    {
        protected override WatermarkTextBox CreateEditor()
        {
            return new PropertyGridEditorTextBox();
        }

        protected override void SetValueDependencyProperty()
        {
            ValueProperty = TextBox.TextProperty;
        }
    }

    public class PropertyGridEditorTextBox : WatermarkTextBox
    {
        static PropertyGridEditorTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyGridEditorTextBox), new FrameworkPropertyMetadata(typeof(PropertyGridEditorTextBox)));
        }
    }
}
