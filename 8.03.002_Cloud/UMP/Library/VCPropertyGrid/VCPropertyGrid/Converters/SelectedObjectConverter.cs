//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    aaa3d651-8091-4d55-9590-6d3ce5844339
//        CLR Version:              4.0.30319.18444
//        Name:                     SelectedObjectConverter
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Wpf.PropertyGrids.Converters
//        File Name:                SelectedObjectConverter
//
//        created by Charley at 2014/7/23 11:56:30
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Data;

namespace VoiceCyber.Wpf.PropertyGrids.Converters
{
    public class SelectedObjectConverter : IValueConverter
    {
        private const string ValidParameterMessage = @"parameter must be one of the following strings: 'Type', 'TypeName', 'SelectedObjectName'";
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter == null)
                throw new ArgumentNullException("parameter");

            if (!(parameter is string))
                throw new ArgumentException(SelectedObjectConverter.ValidParameterMessage);

            if (this.CompareParam(parameter, "Type"))
            {
                return this.ConvertToType(value, culture);
            }
            else if (this.CompareParam(parameter, "TypeName"))
            {
                return this.ConvertToTypeName(value, culture);
            }
            else if (this.CompareParam(parameter, "SelectedObjectName"))
            {
                return this.ConvertToSelectedObjectName(value, culture);
            }
            else
            {
                throw new ArgumentException(SelectedObjectConverter.ValidParameterMessage);
            }
        }

        private bool CompareParam(object parameter, string parameterValue)
        {
            return string.Compare((string)parameter, parameterValue, true) == 0;
        }

        private object ConvertToType(object value, CultureInfo culture)
        {
            return (value != null)
              ? value.GetType()
              : null;
        }

        private object ConvertToTypeName(object value, CultureInfo culture)
        {
            if (value == null)
                return string.Empty;

            Type newType = value.GetType();

            DisplayNameAttribute displayNameAttribute = newType.GetCustomAttributes(false).OfType<DisplayNameAttribute>().FirstOrDefault();

            return (displayNameAttribute == null)
              ? newType.Name
              : displayNameAttribute.DisplayName;
        }

        private object ConvertToSelectedObjectName(object value, CultureInfo culture)
        {
            if (value == null)
                return String.Empty;

            Type newType = value.GetType();
            PropertyInfo[] properties = newType.GetProperties();
            foreach (PropertyInfo property in properties)
            {
                if (property.Name == "Name")
                    return property.GetValue(value, null);
            }

            return String.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
