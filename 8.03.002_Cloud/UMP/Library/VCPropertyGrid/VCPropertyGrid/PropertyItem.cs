//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    e0c0223b-cf43-41fa-af66-f15b13762d44
//        CLR Version:              4.0.30319.18444
//        Name:                     PropertyItem
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Wpf.PropertyGrids
//        File Name:                PropertyItem
//
//        created by Charley at 2014/7/23 12:09:31
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace VoiceCyber.Wpf.PropertyGrids
{
    [TemplatePart(Name = "content", Type = typeof(ContentControl))]
    public class PropertyItem : CustomPropertyItem
    {
        private int _categoryOrder;

        #region Properties

        #region CategoryOrder

        public int CategoryOrder
        {
            get
            {
                return _categoryOrder;
            }
            internal set
            {
                if (_categoryOrder != value)
                {
                    _categoryOrder = value;
                    // Notify the parent helper since this property may affect ordering.
                    this.RaisePropertyChanged(() => this.CategoryOrder);
                }
            }
        }

        #endregion //CategoryOrder

        #region IsReadOnly

        /// <summary>
        /// Identifies the IsReadOnly dependency property
        /// </summary>
        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register("IsReadOnly", typeof(bool), typeof(PropertyItem), new UIPropertyMetadata(false));

        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            set { SetValue(IsReadOnlyProperty, value); }
        }

        #endregion //IsReadOnly

        #region PropertyOrder

        public static readonly DependencyProperty PropertyOrderProperty =
            DependencyProperty.Register("PropertyOrder", typeof(int), typeof(PropertyItem), new UIPropertyMetadata(0));

        public int PropertyOrder
        {
            get { return (int)GetValue(PropertyOrderProperty); }
            set { SetValue(PropertyOrderProperty, value); }
        }

        #endregion //PropertyOrder

        #region PropertyDescriptor

        public PropertyDescriptor PropertyDescriptor
        {
            get;
            internal set;
        }

        #endregion //PropertyDescriptor

        #region PropertyType

        public Type PropertyType
        {
            get
            {
                return (PropertyDescriptor != null)
                  ? PropertyDescriptor.PropertyType
                  : null;
            }
        }

        #endregion //PropertyType

        #region DescriptorDefinition

        internal DescriptorPropertyDefinitionBase DescriptorDefinition
        {
            get;
            private set;
        }

        #endregion DescriptorDefinition

        #region Instance

        public object Instance
        {
            get;
            internal set;
        }

        #endregion //Instance

        #endregion //Properties

        #region Methods

        protected override void OnIsExpandedChanged(bool oldValue, bool newValue)
        {
            if (newValue)
            {
                // This withholds the generation of all PropertyItem instances (recursively)
                // until the PropertyItem is expanded.
                var objectContainerHelper = ContainerHelper as ObjectContainerHelperBase;
                if (objectContainerHelper != null)
                {
                    objectContainerHelper.GenerateProperties();
                }
            }
        }

        protected override void OnEditorChanged(FrameworkElement oldValue, FrameworkElement newValue)
        {
            if (oldValue != null)
                oldValue.DataContext = null;

            if (newValue != null)
                newValue.DataContext = this;
        }

        protected override object OnCoerceValueChanged(object baseValue)
        {
            // Propagate error from DescriptorPropertyDefinitionBase to PropertyItem.Value
            // to see the red error rectangle in the propertyGrid.
            BindingExpression be = GetBindingExpression(PropertyItem.ValueProperty);
            if ((be != null) && be.DataItem is DescriptorPropertyDefinitionBase)
            {
                DescriptorPropertyDefinitionBase descriptor = be.DataItem as DescriptorPropertyDefinitionBase;
                if (Validation.GetHasError(descriptor))
                {
                    ReadOnlyObservableCollection<ValidationError> errors = Validation.GetErrors(descriptor);
                    Validation.MarkInvalid(be, errors[0]);
                }
            }
            return baseValue;
        }

        protected override void OnValueChanged(object oldValue, object newValue)
        {
            base.OnValueChanged(oldValue, newValue);
        }

        private void OnDefinitionContainerHelperInvalidated(object sender, EventArgs e)
        {
            var helper = this.DescriptorDefinition.CreateContainerHelper(this);
            this.ContainerHelper = helper;
            if (this.IsExpanded)
            {
                helper.GenerateProperties();
            }
        }

        #endregion

        #region Constructors

        internal PropertyItem(DescriptorPropertyDefinitionBase definition)
            : base()
        {
            if (definition == null)
                throw new ArgumentNullException("definition");

            this.DescriptorDefinition = definition;
            this.ContainerHelper = definition.CreateContainerHelper(this);
            definition.ContainerHelperInvalidated += new EventHandler(OnDefinitionContainerHelperInvalidated);
        }

        #endregion //Constructors
    }
}
