//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    89d5cfbf-e6db-46ae-8270-fdb772ff0c09
//        CLR Version:              4.0.30319.18444
//        Name:                     UShortUpDown
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.CustomControls.NumericUpDown.Implementation
//        File Name:                UShortUpDown
//
//        created by Charley at 2014/7/18 15:20:10
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace VoiceCyber.Wpf.CustomControls
{
    public class UShortUpDown : CommonNumericUpDown<ushort>
    {
        #region Constructors

        static UShortUpDown()
        {
            UpdateMetadata(typeof(UShortUpDown), (ushort)1, ushort.MinValue, ushort.MaxValue);
        }

        public UShortUpDown()
            : base(ushort.Parse, Decimal.ToUInt16, (v1, v2) => v1 < v2, (v1, v2) => v1 > v2)
        {
           
        }

        #endregion //Constructors

        #region Base Class Overrides

        protected override ushort IncrementValue(ushort value, ushort increment)
        {
            return (ushort)(value + increment);
        }

        protected override ushort DecrementValue(ushort value, ushort increment)
        {
            return (ushort)(value - increment);
        }

        #endregion //Base Class Overrides
    }
}
