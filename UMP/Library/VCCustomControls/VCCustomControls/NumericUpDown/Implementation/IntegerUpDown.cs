//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    e095a9c1-cf46-48ba-8e6c-d9bff495e6fb
//        CLR Version:              4.0.30319.18444
//        Name:                     IntegerUpDown
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.CustomControls.NumericUpDown.Implementation
//        File Name:                IntegerUpDown
//
//        created by Charley at 2014/7/18 15:16:45
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace VoiceCyber.Wpf.CustomControls
{
    public class IntegerUpDown : CommonNumericUpDown<int>
    {
        #region Constructors

        static IntegerUpDown()
        {
            UpdateMetadata(typeof(IntegerUpDown), 1, int.MinValue, int.MaxValue);
        }

        public IntegerUpDown()
            : base(Int32.Parse, Decimal.ToInt32, (v1, v2) => v1 < v2, (v1, v2) => v1 > v2)
        {
          
        }

        #endregion //Constructors

        #region Base Class Overrides

        protected override int IncrementValue(int value, int increment)
        {
            return value + increment;
        }

        protected override int DecrementValue(int value, int increment)
        {
            return value - increment;
        }

        #endregion //Base Class Overrides
    }
}
