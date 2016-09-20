//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    069babef-5f75-4d22-bf87-525fc4a205bd
//        CLR Version:              4.0.30319.18444
//        Name:                     TextBlockEditor
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Wpf.PropertyGrids.Editors
//        File Name:                TextBlockEditor
//
//        created by Charley at 2014/7/23 12:02:33
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
    public class TextBlockEditor : TypeEditor<TextBlock>
    {
        protected override TextBlock CreateEditor()
        {
            return new PropertyGridEditorTextBlock();
        }

        protected override void SetValueDependencyProperty()
        {
            ValueProperty = TextBlock.TextProperty;
        }

        protected override void SetControlProperties()
        {
            Editor.Margin = new System.Windows.Thickness(5, 0, 0, 0);
        }
    }

    public class PropertyGridEditorTextBlock : TextBlock
    {
        static PropertyGridEditorTextBlock()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyGridEditorTextBlock), new FrameworkPropertyMetadata(typeof(PropertyGridEditorTextBlock)));
        }
    }
}
