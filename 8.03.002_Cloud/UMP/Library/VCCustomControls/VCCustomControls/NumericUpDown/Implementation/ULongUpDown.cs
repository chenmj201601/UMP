//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    ff2000e7-11da-4db1-b1d6-fdbe1b30904e
//        CLR Version:              4.0.30319.18444
//        Name:                     ULongUpDown
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.CustomControls.NumericUpDown.Implementation
//        File Name:                ULongUpDown
//
//        created by Charley at 2014/7/18 15:19:41
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace VoiceCyber.Wpf.CustomControls
{
    public class ULongUpDown : CommonNumericUpDown<ulong>
    {
        #region Constructors

        static ULongUpDown()
        {
            UpdateMetadata(typeof(ULongUpDown), (ulong)1, ulong.MinValue, ulong.MaxValue);
        }

        public ULongUpDown()
            : base(ulong.Parse, Decimal.ToUInt64, (v1, v2) => v1 < v2, (v1, v2) => v1 > v2)
        {
           
        }

        #endregion //Constructors

        #region Base Class Overrides

        protected override ulong IncrementValue(ulong value, ulong increment)
        {
            return (ulong)(value + increment);
        }

        protected override ulong DecrementValue(ulong value, ulong increment)
        {
            return (ulong)(value - increment);
        }

        #endregion //Base Class Overrides
    }
}
