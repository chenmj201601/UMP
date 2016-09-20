//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    a168a386-ad1b-4d61-a0f6-3991ca813862
//        CLR Version:              4.0.30319.18444
//        Name:                     EditorTemplateDefinition
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Wpf.PropertyGrids.Definitions
//        File Name:                EditorTemplateDefinition
//
//        created by Charley at 2014/7/23 11:57:35
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace VoiceCyber.Wpf.PropertyGrids.Definitions
{
    public class EditorTemplateDefinition : EditorDefinitionBase
    {


        #region EditingTemplate
        public static readonly DependencyProperty EditingTemplateProperty =
            DependencyProperty.Register("EditingTemplate", typeof(DataTemplate), typeof(EditorTemplateDefinition), new UIPropertyMetadata(null));

        public DataTemplate EditingTemplate
        {
            get { return (DataTemplate)GetValue(EditingTemplateProperty); }
            set { SetValue(EditingTemplateProperty, value); }
        }
        #endregion //EditingTemplate

        protected override sealed FrameworkElement GenerateEditingElement(PropertyItemBase propertyItem)
        {
            return (this.EditingTemplate != null)
              ? this.EditingTemplate.LoadContent() as FrameworkElement
              : null;
        }
    }
}
