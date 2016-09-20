//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    7df3eca0-755d-4087-a445-75544e31f0e4
//        CLR Version:              4.0.30319.18444
//        Name:                     ByteUpDown
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.CustomControls.NumericUpDown.Implementation
//        File Name:                ByteUpDown
//
//        created by Charley at 2014/7/18 15:13:56
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace VoiceCyber.Wpf.CustomControls
{
    public class ByteUpDown : CommonNumericUpDown<byte>
    {
        #region Constructors

        static ByteUpDown()
        {
            UpdateMetadata(typeof(ByteUpDown), (byte)1, byte.MinValue, byte.MaxValue);
        }

        public ByteUpDown()
            : base(Byte.Parse, Decimal.ToByte, (v1, v2) => v1 < v2, (v1, v2) => v1 > v2)
        {
          
        }

        #endregion //Constructors

        #region Base Class Overrides

        protected override byte IncrementValue(byte value, byte increment)
        {
            return (byte)(value + increment);
        }

        protected override byte DecrementValue(byte value, byte increment)
        {
            return (byte)(value - increment);
        }

        #endregion //Base Class Overrides
    }
}
