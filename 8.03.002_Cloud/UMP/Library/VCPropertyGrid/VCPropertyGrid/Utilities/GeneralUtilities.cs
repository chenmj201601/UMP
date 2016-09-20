//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    7a76c576-0be7-4df8-b3d2-e6d42b20c438
//        CLR Version:              4.0.30319.18444
//        Name:                     GeneralUtilities
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Wpf.PropertyGrids.Utilities
//        File Name:                GeneralUtilities
//
//        created by Charley at 2014/7/23 15:25:55
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace VoiceCyber.Wpf.PropertyGrids.Utilities
{
    internal sealed class GeneralUtilities : DependencyObject
    {
        private GeneralUtilities() { }

        #region StubValue attached property

        internal static readonly DependencyProperty StubValueProperty = DependencyProperty.RegisterAttached(
          "StubValue",
          typeof(object),
          typeof(GeneralUtilities),
          new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        internal static object GetStubValue(DependencyObject obj)
        {
            return (object)obj.GetValue(GeneralUtilities.StubValueProperty);
        }

        internal static void SetStubValue(DependencyObject obj, object value)
        {
            obj.SetValue(GeneralUtilities.StubValueProperty, value);
        }

        #endregion StubValue attached property

        public static object GetPathValue(object sourceObject, string path)
        {
            var targetObj = new GeneralUtilities();
            BindingOperations.SetBinding(targetObj, GeneralUtilities.StubValueProperty, new Binding(path) { Source = sourceObject });
            object value = GeneralUtilities.GetStubValue(targetObj);
            BindingOperations.ClearBinding(targetObj, GeneralUtilities.StubValueProperty);
            return value;
        }

        public static object GetBindingValue(object sourceObject, Binding binding)
        {
            Binding bindingClone = new Binding()
            {
                BindsDirectlyToSource = binding.BindsDirectlyToSource,
                Converter = binding.Converter,
                ConverterCulture = binding.ConverterCulture,
                ConverterParameter = binding.ConverterParameter,
                FallbackValue = binding.FallbackValue,
                Mode = BindingMode.OneTime,
                Path = binding.Path,
                StringFormat = binding.StringFormat,
                TargetNullValue = binding.TargetNullValue,
                XPath = binding.XPath
            };

            bindingClone.Source = sourceObject;

            var targetObj = new GeneralUtilities();
            BindingOperations.SetBinding(targetObj, GeneralUtilities.StubValueProperty, bindingClone);
            object value = GeneralUtilities.GetStubValue(targetObj);
            BindingOperations.ClearBinding(targetObj, GeneralUtilities.StubValueProperty);
            return value;
        }

        internal static bool CanConvertValue(object value, object targetType)
        {
            return ((value != null)
                    && (!object.Equals(value.GetType(), targetType))
                    && (!object.Equals(targetType, typeof(object))));
        }
    }
}
