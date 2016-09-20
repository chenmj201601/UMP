//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    4609d625-de3a-4c57-a638-64f1722a7d49
//        CLR Version:              4.0.30319.18444
//        Name:                     SByteUpDown
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.CustomControls.NumericUpDown.Implementation
//        File Name:                SByteUpDown
//
//        created by Charley at 2014/7/18 15:17:40
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace VoiceCyber.Wpf.CustomControls
{
    public class SByteUpDown : CommonNumericUpDown<sbyte>
    {
        #region Constructors

        static SByteUpDown()
        {
            UpdateMetadata(typeof(SByteUpDown), (sbyte)1, sbyte.MinValue, sbyte.MaxValue);
        }

        public SByteUpDown()
            : base(sbyte.Parse, Decimal.ToSByte, (v1, v2) => v1 < v2, (v1, v2) => v1 > v2)
        {
           
        }

        #endregion //Constructors

        #region Base Class Overrides

        protected override sbyte IncrementValue(sbyte value, sbyte increment)
        {
            return (sbyte)(value + increment);
        }

        protected override sbyte DecrementValue(sbyte value, sbyte increment)
        {
            return (sbyte)(value - increment);
        }

        #endregion //Base Class Overrides
    }
}
