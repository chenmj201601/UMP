//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    a6639e0a-00ca-4718-a117-da7d271b16ea
//        CLR Version:              4.0.30319.18444
//        Name:                     ObjectContainerHelper
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Wpf.PropertyGrids
//        File Name:                ObjectContainerHelper
//
//        created by Charley at 2014/7/23 12:07:26
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using VoiceCyber.Wpf.PropertyGrids.Attributes;

namespace VoiceCyber.Wpf.PropertyGrids
{
    internal class ObjectContainerHelper : ObjectContainerHelperBase
    {
        private object _selectedObject;

        public ObjectContainerHelper(IPropertyContainer propertyContainer, object selectedObject)
            : base(propertyContainer)
        {
            _selectedObject = selectedObject;
        }

        private object SelectedObject
        {
            get
            {
                return _selectedObject;
            }
        }

        protected override string GetDefaultPropertyName()
        {
            object selectedObject = SelectedObject;
            return (selectedObject != null) ? ObjectContainerHelperBase.GetDefaultPropertyName(SelectedObject) : (string)null;
        }

        protected override IEnumerable<PropertyItem> GenerateSubPropertiesCore()
        {
            var propertyItems = new List<PropertyItem>();

            if (SelectedObject != null)
            {
                try
                {
                    List<PropertyDescriptor> descriptors = ObjectContainerHelperBase.GetPropertyDescriptors(SelectedObject);
                    foreach (var descriptor in descriptors)
                    {
                        var propertyDef = this.GetPropertyDefinition(descriptor);
                        bool isBrowsable = descriptor.IsBrowsable && this.PropertyContainer.AutoGenerateProperties;
                        if (propertyDef != null)
                        {
                            isBrowsable = propertyDef.IsBrowsable.GetValueOrDefault(isBrowsable);
                        }
                        if (isBrowsable)
                        {
                            propertyItems.Add(this.CreatePropertyItem(descriptor, propertyDef));
                        }
                    }
                }
                catch (Exception e)
                {
                    //TODO: handle this some how
                    Debug.WriteLine("Property creation failed");
                    Debug.WriteLine(e.StackTrace);
                }
            }

            return propertyItems;
        }


        private PropertyItem CreatePropertyItem(PropertyDescriptor property, PropertyDefinition propertyDef)
        {
            DescriptorPropertyDefinition definition = new DescriptorPropertyDefinition(property, SelectedObject, this.PropertyContainer.IsCategorized);
            definition.InitProperties();

            this.InitializeDescriptorDefinition(definition, propertyDef);

            PropertyItem propertyItem = new PropertyItem(definition);
            Debug.Assert(SelectedObject != null);
            propertyItem.Instance = SelectedObject;
            propertyItem.CategoryOrder = this.GetCategoryOrder(definition.CategoryValue);

            return propertyItem;
        }

        private int GetCategoryOrder(object categoryValue)
        {
            Debug.Assert(SelectedObject != null);

            if (categoryValue == null)
                return int.MaxValue;

            int order = int.MaxValue;
            object selectedObject = SelectedObject;
            CategoryOrderAttribute[] orderAttributes = (selectedObject != null)
              ? (CategoryOrderAttribute[])selectedObject.GetType().GetCustomAttributes(typeof(CategoryOrderAttribute), true)
              : new CategoryOrderAttribute[0];

            var orderAttribute = orderAttributes
              .FirstOrDefault((a) => object.Equals(a.CategoryValue, categoryValue));

            if (orderAttribute != null)
            {
                order = orderAttribute.Order;
            }

            return order;
        }




    }
}
