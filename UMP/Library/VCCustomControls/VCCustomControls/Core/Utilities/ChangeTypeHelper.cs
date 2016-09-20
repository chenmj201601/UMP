//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    b42967ad-d49a-401b-a1d4-264be6e1bad3
//        CLR Version:              4.0.30319.18444
//        Name:                     ChangeTypeHelper
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.CustomControls.Core.Utilities
//        File Name:                ChangeTypeHelper
//
//        created by Charley at 2014/7/21 10:28:04
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace VoiceCyber.Wpf.CustomControls.Core.Utilities
{
    internal static class ChangeTypeHelper
    {
        internal static object ChangeType(object value, Type conversionType, IFormatProvider provider)
        {
            if (conversionType == null)
            {
                throw new ArgumentNullException("conversionType");
            }
            if (conversionType == typeof(Guid))
            {
                return new Guid(value.ToString());
            }
            else if (conversionType == typeof(Guid?))
            {
                if (value == null)
                    return null;
                return new Guid(value.ToString());
            }
            else if (conversionType.IsGenericType && conversionType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                if (value == null)
                    return null;
                NullableConverter nullableConverter = new NullableConverter(conversionType);
                conversionType = nullableConverter.UnderlyingType;
            }

            return System.Convert.ChangeType(value, conversionType, provider);
        }
    }
}
