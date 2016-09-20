//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    e48dce90-db2e-4d69-8167-1eb8bab64194
//        CLR Version:              4.0.30319.18444
//        Name:                     DecimalUpDown
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.CustomControls.NumericUpDown.Implementation
//        File Name:                DecimalUpDown
//
//        created by Charley at 2014/7/18 15:15:23
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace VoiceCyber.Wpf.CustomControls
{
    public class DecimalUpDown : CommonNumericUpDown<decimal>
    {
        #region Constructors

        static DecimalUpDown()
        {
            UpdateMetadata(typeof(DecimalUpDown), 1m, decimal.MinValue, decimal.MaxValue);
        }

        public DecimalUpDown()
            : base(Decimal.Parse, (d) => d, (v1, v2) => v1 < v2, (v1, v2) => v1 > v2)
        {
           
        }

        #endregion //Constructors

        #region Base Class Overrides

        protected override decimal IncrementValue(decimal value, decimal increment)
        {
            return value + increment;
        }

        protected override decimal DecrementValue(decimal value, decimal increment)
        {
            return value - increment;
        }

        #endregion //Base Class Overrides
    }
}
