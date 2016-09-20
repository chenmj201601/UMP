//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    0edeb4d2-aabd-47cb-88d2-36316ce25526
//        CLR Version:              4.0.30319.18444
//        Name:                     EditorDefinitionBase
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Wpf.PropertyGrids.Definitions
//        File Name:                EditorDefinitionBase
//
//        created by Charley at 2014/7/23 11:57:08
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace VoiceCyber.Wpf.PropertyGrids.Definitions
{
    public abstract class EditorDefinitionBase : PropertyDefinitionBase
    {

        internal EditorDefinitionBase() { }

        internal FrameworkElement GenerateEditingElementInternal(PropertyItemBase propertyItem)
        {
            return this.GenerateEditingElement(propertyItem);
        }

        protected virtual FrameworkElement GenerateEditingElement(PropertyItemBase propertyItem) { return null; }

        internal void UpdateProperty(FrameworkElement element, DependencyProperty elementProp, DependencyProperty definitionProperty)
        {
            object currentValue = this.GetValue(definitionProperty);
            object localValue = this.ReadLocalValue(definitionProperty);
            // Avoid setting values if it does not affect anything 
            // because setting a local value may prevent a style setter from being active.
            if ((localValue != DependencyProperty.UnsetValue)
              || currentValue != element.GetValue(elementProp))
            {
                element.SetValue(elementProp, currentValue);
            }
            else
            {
                element.ClearValue(elementProp);
            }
        }
    }
}
