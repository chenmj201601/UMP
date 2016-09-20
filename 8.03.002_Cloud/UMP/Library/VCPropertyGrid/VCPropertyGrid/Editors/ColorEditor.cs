//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    50eed0e0-cb21-41d3-9139-77100dc394af
//        CLR Version:              4.0.30319.18444
//        Name:                     ColorEditor
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Wpf.PropertyGrids.Editors
//        File Name:                ColorEditor
//
//        created by Charley at 2014/7/23 11:59:54
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using VoiceCyber.Wpf.CustomControls;

namespace VoiceCyber.Wpf.PropertyGrids.Editors
{
    public class ColorEditor : TypeEditor<ColorPicker>
    {
        protected override ColorPicker CreateEditor()
        {
            return new PropertyGridEditorColorPicker();
        }

        protected override void SetControlProperties()
        {
            Editor.BorderThickness = new System.Windows.Thickness(0);
            Editor.DisplayColorAndName = true;
        }
        protected override void SetValueDependencyProperty()
        {
            ValueProperty = ColorPicker.SelectedColorProperty;
        }
    }

    public class PropertyGridEditorColorPicker : ColorPicker
    {
        static PropertyGridEditorColorPicker()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyGridEditorColorPicker), new FrameworkPropertyMetadata(typeof(PropertyGridEditorColorPicker)));
        }
    }
}
