//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    53156253-bec6-4627-a973-0890772eb037
//        CLR Version:              4.0.30319.18444
//        Name:                     CommonPropertyExceptionValidationRule
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Wpf.PropertyGrids
//        File Name:                CommonPropertyExceptionValidationRule
//
//        created by Charley at 2014/7/23 12:04:44
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Windows.Controls;
using VoiceCyber.Wpf.PropertyGrids.Utilities;

namespace VoiceCyber.Wpf.PropertyGrids
{
    internal class CommonPropertyExceptionValidationRule : ValidationRule
    {
        private TypeConverter _propertyTypeConverter;
        private Type _type;

        internal CommonPropertyExceptionValidationRule(Type type)
        {
            _propertyTypeConverter = TypeDescriptor.GetConverter(type);
            _type = type;
        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            ValidationResult result = new ValidationResult(true, null);

            if (GeneralUtilities.CanConvertValue(value, _type))
            {
                try
                {
                    _propertyTypeConverter.ConvertFrom(value);
                }
                catch (Exception e)
                {
                    // Will display a red border in propertyGrid
                    result = new ValidationResult(false, e.Message);
                }
            }
            return result;
        }
    }
}
