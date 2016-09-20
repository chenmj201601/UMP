//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    22811df3-5dfb-430d-a96e-84cb4cb123d4
//        CLR Version:              4.0.30319.18444
//        Name:                     UIntegerUpDown
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.CustomControls.NumericUpDown.Implementation
//        File Name:                UIntegerUpDown
//
//        created by Charley at 2014/7/18 15:19:07
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace VoiceCyber.Wpf.CustomControls
{
    public class UIntegerUpDown : CommonNumericUpDown<uint>
    {
        #region Constructors

        static UIntegerUpDown()
        {
            UpdateMetadata(typeof(UIntegerUpDown), (uint)1, uint.MinValue, uint.MaxValue);
        }

        public UIntegerUpDown()
            : base(uint.Parse, Decimal.ToUInt32, (v1, v2) => v1 < v2, (v1, v2) => v1 > v2)
        {
           
        }

        #endregion //Constructors

        #region Base Class Overrides

        protected override uint IncrementValue(uint value, uint increment)
        {
            return (uint)(value + increment);
        }

        protected override uint DecrementValue(uint value, uint increment)
        {
            return (uint)(value - increment);
        }

        #endregion //Base Class Overrides
    }
}
