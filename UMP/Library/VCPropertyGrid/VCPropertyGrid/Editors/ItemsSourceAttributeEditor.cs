//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    b2067b88-f033-42fe-a45f-4ffa733017e4
//        CLR Version:              4.0.30319.18444
//        Name:                     ItemsSourceAttributeEditor
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Wpf.PropertyGrids.Editors
//        File Name:                ItemsSourceAttributeEditor
//
//        created by Charley at 2014/7/23 12:01:23
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;
using VoiceCyber.Wpf.PropertyGrids.Attributes;

namespace VoiceCyber.Wpf.PropertyGrids.Editors
{
    public class ItemsSourceAttributeEditor : TypeEditor<System.Windows.Controls.ComboBox>
    {
        private readonly ItemsSourceAttribute _attribute;

        public ItemsSourceAttributeEditor(ItemsSourceAttribute attribute)
        {
            _attribute = attribute;
        }

        protected override void SetValueDependencyProperty()
        {
            ValueProperty = System.Windows.Controls.ComboBox.SelectedValueProperty;
        }

        protected override System.Windows.Controls.ComboBox CreateEditor()
        {
            return new PropertyGridEditorComboBox();
        }

        protected override void ResolveValueBinding(PropertyItem propertyItem)
        {
            SetItemsSource();
            base.ResolveValueBinding(propertyItem);
        }

        protected override void SetControlProperties()
        {
            Editor.DisplayMemberPath = "DisplayName";
            Editor.SelectedValuePath = "Value";
        }

        private void SetItemsSource()
        {
            Editor.ItemsSource = CreateItemsSource();
        }

        private System.Collections.IEnumerable CreateItemsSource()
        {
            var instance = Activator.CreateInstance(_attribute.Type);
            return (instance as IItemsSource).GetValues();
        }
    }
}
