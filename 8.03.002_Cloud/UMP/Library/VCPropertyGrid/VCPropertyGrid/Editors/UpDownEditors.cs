//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    00a5e118-6274-4458-8a8c-42696149554d
//        CLR Version:              4.0.30319.18444
//        Name:                     UpDownEditors
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Wpf.PropertyGrids.Editors
//        File Name:                UpDownEditors
//
//        created by Charley at 2014/7/23 12:03:32
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using VoiceCyber.Wpf.CustomControls;
using VoiceCyber.Wpf.CustomControls.Primitives;

namespace VoiceCyber.Wpf.PropertyGrids.Editors
{
    public class UpDownEditor<TEditor, TType> : TypeEditor<TEditor> where TEditor : UpDownBase<TType>, new()
    {
        protected override void SetControlProperties()
        {
            Editor.TextAlignment = System.Windows.TextAlignment.Left;
        }
        protected override void SetValueDependencyProperty()
        {
            ValueProperty = UpDownBase<TType>.ValueProperty;
        }
    }

    public class ByteUpDownEditor : UpDownEditor<ByteUpDown, byte?>
    {
        protected override ByteUpDown CreateEditor()
        {
            return new PropertyGridEditorByteUpDown();
        }
    }

    public class DecimalUpDownEditor : UpDownEditor<DecimalUpDown, decimal?>
    {
        protected override DecimalUpDown CreateEditor()
        {
            return new PropertyGridEditorDecimalUpDown();
        }
    }

    public class DoubleUpDownEditor : UpDownEditor<DoubleUpDown, double?>
    {
        protected override DoubleUpDown CreateEditor()
        {
            return new PropertyGridEditorDoubleUpDown();
        }

        protected override void SetControlProperties()
        {
            base.SetControlProperties();
            Editor.AllowInputSpecialValues = AllowedSpecialValues.Any;
        }
    }

    public class IntegerUpDownEditor : UpDownEditor<IntegerUpDown, int?>
    {
        protected override IntegerUpDown CreateEditor()
        {
            return new PropertyGridEditorIntegerUpDown();
        }
    }

    public class LongUpDownEditor : UpDownEditor<LongUpDown, long?>
    {
        protected override LongUpDown CreateEditor()
        {
            return new PropertyGridEditorLongUpDown();
        }
    }

    public class ShortUpDownEditor : UpDownEditor<ShortUpDown, short?>
    {
        protected override ShortUpDown CreateEditor()
        {
            return new PropertyGridEditorShortUpDown();
        }
    }

    public class SingleUpDownEditor : UpDownEditor<SingleUpDown, float?>
    {
        protected override SingleUpDown CreateEditor()
        {
            return new PropertyGridEditorSingleUpDown();
        }

        protected override void SetControlProperties()
        {
            base.SetControlProperties();
            Editor.AllowInputSpecialValues = AllowedSpecialValues.Any;
        }
    }

    public class DateTimeUpDownEditor : UpDownEditor<DateTimeUpDown, DateTime?>
    {
        protected override DateTimeUpDown CreateEditor()
        {
            return new PropertyGridEditorDateTimeUpDown();
        }
    }

    public class TimeSpanUpDownEditor : UpDownEditor<TimeSpanUpDown, TimeSpan?>
    {
        protected override TimeSpanUpDown CreateEditor()
        {
            return new PropertyGridEditorTimeSpanUpDown();
        }
    }

    internal class SByteUpDownEditor : UpDownEditor<SByteUpDown, sbyte?>
    {
        protected override SByteUpDown CreateEditor()
        {
            return new PropertyGridEditorSByteUpDown();
        }
    }

    internal class UIntegerUpDownEditor : UpDownEditor<UIntegerUpDown, uint?>
    {
        protected override UIntegerUpDown CreateEditor()
        {
            return new PropertyGridEditorUIntegerUpDown();
        }
    }

    internal class ULongUpDownEditor : UpDownEditor<ULongUpDown, ulong?>
    {
        protected override ULongUpDown CreateEditor()
        {
            return new PropertyGridEditorULongUpDown();
        }
    }

    internal class UShortUpDownEditor : UpDownEditor<UShortUpDown, ushort?>
    {
        protected override UShortUpDown CreateEditor()
        {
            return new PropertyGridEditorUShortUpDown();
        }
    }



    public class PropertyGridEditorByteUpDown : ByteUpDown
    {
        static PropertyGridEditorByteUpDown()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyGridEditorByteUpDown), new FrameworkPropertyMetadata(typeof(PropertyGridEditorByteUpDown)));
        }
    }

    public class PropertyGridEditorDecimalUpDown : DecimalUpDown
    {
        static PropertyGridEditorDecimalUpDown()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyGridEditorDecimalUpDown), new FrameworkPropertyMetadata(typeof(PropertyGridEditorDecimalUpDown)));
        }
    }

    public class PropertyGridEditorDoubleUpDown : DoubleUpDown
    {
        static PropertyGridEditorDoubleUpDown()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyGridEditorDoubleUpDown), new FrameworkPropertyMetadata(typeof(PropertyGridEditorDoubleUpDown)));
        }
    }

    public class PropertyGridEditorIntegerUpDown : IntegerUpDown
    {
        static PropertyGridEditorIntegerUpDown()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyGridEditorIntegerUpDown), new FrameworkPropertyMetadata(typeof(PropertyGridEditorIntegerUpDown)));
        }
    }

    public class PropertyGridEditorLongUpDown : LongUpDown
    {
        static PropertyGridEditorLongUpDown()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyGridEditorLongUpDown), new FrameworkPropertyMetadata(typeof(PropertyGridEditorLongUpDown)));
        }
    }

    public class PropertyGridEditorShortUpDown : ShortUpDown
    {
        static PropertyGridEditorShortUpDown()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyGridEditorShortUpDown), new FrameworkPropertyMetadata(typeof(PropertyGridEditorShortUpDown)));
        }
    }

    public class PropertyGridEditorSingleUpDown : SingleUpDown
    {
        static PropertyGridEditorSingleUpDown()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyGridEditorSingleUpDown), new FrameworkPropertyMetadata(typeof(PropertyGridEditorSingleUpDown)));
        }
    }

    public class PropertyGridEditorDateTimeUpDown : DateTimeUpDown
    {
        static PropertyGridEditorDateTimeUpDown()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyGridEditorDateTimeUpDown), new FrameworkPropertyMetadata(typeof(PropertyGridEditorDateTimeUpDown)));
        }
    }

    public class PropertyGridEditorTimeSpanUpDown : TimeSpanUpDown
    {
        static PropertyGridEditorTimeSpanUpDown()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyGridEditorTimeSpanUpDown), new FrameworkPropertyMetadata(typeof(PropertyGridEditorTimeSpanUpDown)));
        }
    }

    internal class PropertyGridEditorSByteUpDown : SByteUpDown
    {
        static PropertyGridEditorSByteUpDown()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyGridEditorSByteUpDown), new FrameworkPropertyMetadata(typeof(PropertyGridEditorSByteUpDown)));
        }
    }

    internal class PropertyGridEditorUIntegerUpDown : UIntegerUpDown
    {
        static PropertyGridEditorUIntegerUpDown()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyGridEditorUIntegerUpDown), new FrameworkPropertyMetadata(typeof(PropertyGridEditorUIntegerUpDown)));
        }
    }

    internal class PropertyGridEditorULongUpDown : ULongUpDown
    {
        static PropertyGridEditorULongUpDown()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyGridEditorULongUpDown), new FrameworkPropertyMetadata(typeof(PropertyGridEditorULongUpDown)));
        }
    }

    internal class PropertyGridEditorUShortUpDown : UShortUpDown
    {
        static PropertyGridEditorUShortUpDown()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyGridEditorUShortUpDown), new FrameworkPropertyMetadata(typeof(PropertyGridEditorUShortUpDown)));
        }
    }
}
