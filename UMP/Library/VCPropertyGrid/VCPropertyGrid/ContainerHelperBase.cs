//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    77da2a87-2543-4e6f-add8-6c57b493a7b2
//        CLR Version:              4.0.30319.18444
//        Name:                     ContainerHelperBase
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Wpf.PropertyGrids
//        File Name:                ContainerHelperBase
//
//        created by Charley at 2014/7/23 12:05:15
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using VoiceCyber.Wpf.PropertyGrids.Definitions;
using VoiceCyber.Wpf.PropertyGrids.Utilities;

namespace VoiceCyber.Wpf.PropertyGrids
{
    internal abstract class ContainerHelperBase
    {
        protected readonly IPropertyContainer PropertyContainer;

        public ContainerHelperBase(IPropertyContainer propertyContainer)
        {
            if (propertyContainer == null)
                throw new ArgumentNullException("propertyContainer");

            PropertyContainer = propertyContainer;

            var propChange = propertyContainer as INotifyPropertyChanged;
            if (propChange != null)
            {
                propChange.PropertyChanged += new PropertyChangedEventHandler(OnPropertyContainerPropertyChanged);
            }
        }

        #region IsGenerated attached property

        internal static readonly DependencyProperty IsGeneratedProperty = DependencyProperty.RegisterAttached(
          "IsGenerated",
          typeof(bool),
          typeof(ContainerHelperBase),
          new PropertyMetadata(false));

        internal static bool GetIsGenerated(DependencyObject obj)
        {
            return (bool)obj.GetValue(ContainerHelperBase.IsGeneratedProperty);
        }

        internal static void SetIsGenerated(DependencyObject obj, bool value)
        {
            obj.SetValue(ContainerHelperBase.IsGeneratedProperty, value);
        }

        #endregion IsGenerated attached property

        public abstract IList Properties
        {
            get;
        }

        internal ItemsControl ChildrenItemsControl
        {
            get;
            set;
        }

        public virtual void ClearHelper()
        {
            var propChange = PropertyContainer as INotifyPropertyChanged;
            if (propChange != null)
            {
                propChange.PropertyChanged -= new PropertyChangedEventHandler(OnPropertyContainerPropertyChanged);
            }

            // Calling RemoveAll() will force the ItemsContol displaying the
            // properties to clear all the current container (i.e., ClearContainerForItem).
            // This will make the call at "ClearChildrenPropertyItem" for every prepared
            // container. Fortunately, the ItemsContainer will not re-prepare the items yet
            // (i.e., probably made on next measure pass), allowing us to set up the new
            // parent helper.
            if (ChildrenItemsControl != null)
            {
                ((IItemContainerGenerator)ChildrenItemsControl.ItemContainerGenerator).RemoveAll();
            }
        }

        public virtual void PrepareChildrenPropertyItem(PropertyItemBase propertyItem, object item)
        {
            // Initialize the parent node
            propertyItem.ParentNode = PropertyContainer;

            PropertyGrid.RaisePreparePropertyItemEvent((UIElement)PropertyContainer, propertyItem, item);
        }

        public virtual void ClearChildrenPropertyItem(PropertyItemBase propertyItem, object item)
        {

            propertyItem.ParentNode = null;

            PropertyGrid.RaiseClearPropertyItemEvent((UIElement)PropertyContainer, propertyItem, item);
        }

        protected FrameworkElement GenerateCustomEditingElement(Type definitionKey, PropertyItemBase propertyItem)
        {
            return (PropertyContainer.EditorDefinitions != null)
              ? this.CreateCustomEditor(PropertyContainer.EditorDefinitions.GetRecursiveBaseTypes(definitionKey), propertyItem)
              : null;
        }

        protected FrameworkElement GenerateCustomEditingElement(object definitionKey, PropertyItemBase propertyItem)
        {
            return (PropertyContainer.EditorDefinitions != null)
              ? this.CreateCustomEditor(PropertyContainer.EditorDefinitions[definitionKey], propertyItem)
              : null;
        }

        protected FrameworkElement CreateCustomEditor(EditorDefinitionBase customEditor, PropertyItemBase propertyItem)
        {
            return (customEditor != null)
              ? customEditor.GenerateEditingElementInternal(propertyItem)
              : null;
        }

        protected virtual void OnPropertyContainerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var propertyName = e.PropertyName;
            IPropertyContainer ps = null;
            if (propertyName == ReflectionHelper.GetPropertyOrFieldName(() => ps.FilterInfo))
            {
                this.OnFilterChanged();
            }
            else if (propertyName == ReflectionHelper.GetPropertyOrFieldName(() => ps.IsCategorized))
            {
                this.OnCategorizationChanged();
            }
            else if (propertyName == ReflectionHelper.GetPropertyOrFieldName(() => ps.AutoGenerateProperties))
            {
                this.OnAutoGeneratePropertiesChanged();
            }
            else if (propertyName == ReflectionHelper.GetPropertyOrFieldName(() => ps.EditorDefinitions))
            {
                this.OnEditorDefinitionsChanged();
            }
            else if (propertyName == ReflectionHelper.GetPropertyOrFieldName(() => ps.PropertyDefinitions))
            {
                this.OnPropertyDefinitionsChanged();
            }
        }

        protected virtual void OnCategorizationChanged() { }

        protected virtual void OnFilterChanged() { }

        protected virtual void OnAutoGeneratePropertiesChanged() { }

        protected virtual void OnEditorDefinitionsChanged() { }

        protected virtual void OnPropertyDefinitionsChanged() { }


        public virtual void OnEndInit() { }

        public abstract PropertyItemBase ContainerFromItem(object item);

        public abstract object ItemFromContainer(PropertyItemBase container);

        public abstract Binding CreateChildrenDefaultBinding(PropertyItemBase propertyItem);

        public virtual void NotifyEditorDefinitionsCollectionChanged() { }
        public virtual void NotifyPropertyDefinitionsCollectionChanged() { }

        public abstract void UpdateValuesFromSource();

    }
}
