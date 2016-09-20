//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    b2888b1e-1692-4fb5-bca7-7c628c15efe7
//        CLR Version:              4.0.30319.18444
//        Name:                     PropertyGridUtilities
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Wpf.PropertyGrids
//        File Name:                PropertyGridUtilities
//
//        created by Charley at 2014/7/23 12:09:10
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using VoiceCyber.Wpf.PropertyGrids.Editors;
using VoiceCyber.Wpf.PropertyGrids.Utilities;

namespace VoiceCyber.Wpf.PropertyGrids
{
    internal class PropertyGridUtilities
    {
        internal static T GetAttribute<T>(PropertyDescriptor property) where T : Attribute
        {
            return property.Attributes.OfType<T>().FirstOrDefault();
        }

        internal static ITypeEditor CreateDefaultEditor(Type propertyType, TypeConverter typeConverter)
        {
            ITypeEditor editor = null;

            if (propertyType == typeof(string))
                editor = new TextBoxEditor();
            else if (propertyType == typeof(bool) || propertyType == typeof(bool?))
                editor = new CheckBoxEditor();
            else if (propertyType == typeof(decimal) || propertyType == typeof(decimal?))
                editor = new DecimalUpDownEditor();
            else if (propertyType == typeof(double) || propertyType == typeof(double?))
                editor = new DoubleUpDownEditor();
            else if (propertyType == typeof(int) || propertyType == typeof(int?))
                editor = new IntegerUpDownEditor();
            else if (propertyType == typeof(short) || propertyType == typeof(short?))
                editor = new ShortUpDownEditor();
            else if (propertyType == typeof(long) || propertyType == typeof(long?))
                editor = new LongUpDownEditor();
            else if (propertyType == typeof(float) || propertyType == typeof(float?))
                editor = new SingleUpDownEditor();
            else if (propertyType == typeof(byte) || propertyType == typeof(byte?))
                editor = new ByteUpDownEditor();
            else if (propertyType == typeof(sbyte) || propertyType == typeof(sbyte?))
                editor = new SByteUpDownEditor();
            else if (propertyType == typeof(uint) || propertyType == typeof(uint?))
                editor = new UIntegerUpDownEditor();
            else if (propertyType == typeof(ulong) || propertyType == typeof(ulong?))
                editor = new ULongUpDownEditor();
            else if (propertyType == typeof(ushort) || propertyType == typeof(ushort?))
                editor = new UShortUpDownEditor();
            else if (propertyType == typeof(DateTime) || propertyType == typeof(DateTime?))
                editor = new DateTimeUpDownEditor();
            else if ((propertyType == typeof(Color)))
                editor = new ColorEditor();
            else if (propertyType.IsEnum)
                editor = new EnumComboBoxEditor();
            else if (propertyType == typeof(TimeSpan) || propertyType == typeof(TimeSpan?))
                editor = new TimeSpanUpDownEditor();
            else if (propertyType == typeof(FontFamily) || propertyType == typeof(FontWeight) || propertyType == typeof(FontStyle) || propertyType == typeof(FontStretch))
                editor = new FontComboBoxEditor();
            else if (propertyType == typeof(Guid) || propertyType == typeof(Guid?))
                editor = new MaskedTextBoxEditor() { ValueDataType = propertyType, Mask = "AAAAAAAA-AAAA-AAAA-AAAA-AAAAAAAAAAAA" };
            else if (propertyType == typeof(char) || propertyType == typeof(char?))
                editor = new MaskedTextBoxEditor() { ValueDataType = propertyType, Mask = "&" };
            else if (propertyType == typeof(object))
                // If any type of object is possible in the property, default to the TextBoxEditor.
                // Useful in some case (e.g., Button.Content).
                // Can be reconsidered but was the legacy behavior on the PropertyGrid.
                editor = new TextBoxEditor();
            else
            {
                Type listType = ListUtilities.GetListItemType(propertyType);

                if (listType != null)
                {
                    //Comment by Charley at 2014/7/23
                    //if (!listType.IsPrimitive && !listType.Equals(typeof(String)) && !listType.IsEnum)
                    //    editor = new CollectionEditor();
                    //else
                    //    editor = new PrimitiveTypeCollectionEditor();
                }
                else
                {
                    // If the type is not supported, check if there is a converter that supports
                    // string conversion to the object type. Use TextBox in theses cases.
                    // Otherwise, return a TextBlock editor since no valid editor exists.
                    editor = (typeConverter != null && typeConverter.CanConvertFrom(typeof(string)))
                      ? (ITypeEditor)new TextBoxEditor()
                      : (ITypeEditor)new TextBlockEditor();
                }
            }

            return editor;
        }
    }
}
