//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    06e2abf2-e79c-471e-a1df-a243cc95d026
//        CLR Version:              4.0.30319.18444
//        Name:                     FontComboBoxEditor
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Wpf.PropertyGrids.Editors
//        File Name:                FontComboBoxEditor
//
//        created by Charley at 2014/7/23 12:01:00
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;
using VoiceCyber.Wpf.PropertyGrids.Utilities;

namespace VoiceCyber.Wpf.PropertyGrids.Editors
{
    public class FontComboBoxEditor : ComboBoxEditor
    {
        protected override IEnumerable CreateItemsSource(PropertyItem propertyItem)
        {
            if (propertyItem.PropertyType == typeof(FontFamily))
                return FontUtilities.Families;
            else if (propertyItem.PropertyType == typeof(FontWeight))
                return FontUtilities.Weights;
            else if (propertyItem.PropertyType == typeof(FontStyle))
                return FontUtilities.Styles;
            else if (propertyItem.PropertyType == typeof(FontStretch))
                return FontUtilities.Stretches;

            return null;
        }
    }
}
