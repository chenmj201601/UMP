//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    1e2d45fa-ffcd-4b24-ab00-c06db6266f49
//        CLR Version:              4.0.30319.18444
//        Name:                     ListConverter
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Wpf.PropertyGrids.Converters
//        File Name:                ListConverter
//
//        created by Charley at 2014/7/23 11:55:54
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;

namespace VoiceCyber.Wpf.PropertyGrids.Converters
{
    internal class ListConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return true;
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return (destinationType == typeof(string));
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value == null)
                return null;

            string names = value as string;

            var list = new List<object>();
            if (names == null && value != null)
            {
                list.Add(value);
            }
            else
            {
                if (names == null)
                    return null;

                foreach (var name in names.Split(','))
                {
                    list.Add(name.Trim());
                }
            }

            return new ReadOnlyCollection<object>(list);
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType != typeof(string))
                throw new InvalidOperationException("Can only convert to string.");


            IList strs = (IList)value;

            if (strs == null)
                return null;

            StringBuilder sb = new StringBuilder();
            bool first = true;
            foreach (object o in strs)
            {
                if (o == null)
                    throw new InvalidOperationException("Property names cannot be null.");

                string s = o as string;
                if (s == null)
                    throw new InvalidOperationException("Does not support serialization of non-string property names.");

                if (s.Contains(","))
                    throw new InvalidOperationException("Property names cannot contain commas.");
                //if (s.Contains(','))
                //    throw new InvalidOperationException("Property names cannot contain commas.");

                if (s.Trim().Length != s.Length)
                    throw new InvalidOperationException("Property names cannot start or end with whitespace characters.");

                if (!first)
                {
                    sb.Append(", ");
                }
                first = false;

                sb.Append(s);
            }

            return sb.ToString();
        }
    }
}
