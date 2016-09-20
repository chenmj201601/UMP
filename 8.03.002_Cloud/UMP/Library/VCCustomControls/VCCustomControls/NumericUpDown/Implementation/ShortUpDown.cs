//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    ce3fe895-52f1-4dd4-a525-98ca49dcfd48
//        CLR Version:              4.0.30319.18444
//        Name:                     ShortUpDown
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.CustomControls.NumericUpDown.Implementation
//        File Name:                ShortUpDown
//
//        created by Charley at 2014/7/18 15:18:08
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace VoiceCyber.Wpf.CustomControls
{
    public class ShortUpDown : CommonNumericUpDown<short>
    {
        #region Constructors

        static ShortUpDown()
        {
            UpdateMetadata(typeof(ShortUpDown), (short)1, short.MinValue, short.MaxValue);
        }

        public ShortUpDown()
            : base(Int16.Parse, Decimal.ToInt16, (v1, v2) => v1 < v2, (v1, v2) => v1 > v2)
        {
           
        }

        #endregion //Constructors

        #region Base Class Overrides

        protected override short IncrementValue(short value, short increment)
        {
            return (short)(value + increment);
        }

        protected override short DecrementValue(short value, short increment)
        {
            return (short)(value - increment);
        }

        #endregion //Base Class Overrides
    }
}
