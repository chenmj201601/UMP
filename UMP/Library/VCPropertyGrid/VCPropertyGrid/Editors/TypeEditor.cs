//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    afd14214-ca84-49b7-a084-6a6cbfc97e10
//        CLR Version:              4.0.30319.18444
//        Name:                     TypeEditor
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Wpf.PropertyGrids.Editors
//        File Name:                TypeEditor
//
//        created by Charley at 2014/7/23 12:03:15
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Data;
using VoiceCyber.Wpf.CustomControls.Primitives;

namespace VoiceCyber.Wpf.PropertyGrids.Editors
{
    public abstract class TypeEditor<T> : ITypeEditor where T : FrameworkElement, new()
    {
        #region Properties

        protected T Editor
        {
            get;
            set;
        }
        protected DependencyProperty ValueProperty
        {
            get;
            set;
        }

        #endregion //Properties

        #region ITypeEditor Members

        public virtual FrameworkElement ResolveEditor(PropertyItem propertyItem)
        {
            Editor = this.CreateEditor();
            SetValueDependencyProperty();
            SetControlProperties();
            ResolveValueBinding(propertyItem);
            return Editor;
        }

        #endregion //ITypeEditor Members

        #region Methods

        protected virtual T CreateEditor()
        {
            return new T();
        }

        protected virtual IValueConverter CreateValueConverter()
        {
            return null;
        }

        protected virtual void ResolveValueBinding(PropertyItem propertyItem)
        {
            var _binding = new Binding("Value");
            _binding.Source = propertyItem;
            _binding.UpdateSourceTrigger = (Editor is InputBase) ? UpdateSourceTrigger.PropertyChanged : UpdateSourceTrigger.Default;
            _binding.Mode = propertyItem.IsReadOnly ? BindingMode.OneWay : BindingMode.TwoWay;
            _binding.Converter = CreateValueConverter();
            BindingOperations.SetBinding(Editor, ValueProperty, _binding);
        }

        protected virtual void SetControlProperties()
        {
            //TODO: implement in derived class
        }

        protected abstract void SetValueDependencyProperty();

        #endregion //Methods
    }
}
