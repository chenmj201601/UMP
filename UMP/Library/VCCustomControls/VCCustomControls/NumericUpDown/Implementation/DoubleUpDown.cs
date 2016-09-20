//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    78505cad-10ee-4a26-8f9b-19b9893e393f
//        CLR Version:              4.0.30319.18444
//        Name:                     DoubleUpDown
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.CustomControls.NumericUpDown.Implementation
//        File Name:                DoubleUpDown
//
//        created by Charley at 2014/7/18 15:16:06
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace VoiceCyber.Wpf.CustomControls
{
    public class DoubleUpDown : CommonNumericUpDown<double>
    {
        #region Constructors

        static DoubleUpDown()
        {
            UpdateMetadata(typeof(DoubleUpDown), 1d, double.NegativeInfinity, double.PositiveInfinity);
        }

        public DoubleUpDown()
            : base(Double.Parse, Decimal.ToDouble, (v1, v2) => v1 < v2, (v1, v2) => v1 > v2)
        {
           
        }

        #endregion //Constructors

        #region Properties


        #region AllowInputSpecialValues

        public static readonly DependencyProperty AllowInputSpecialValuesProperty =
            DependencyProperty.Register("AllowInputSpecialValues", typeof(AllowedSpecialValues), typeof(DoubleUpDown), new UIPropertyMetadata(AllowedSpecialValues.None));

        public AllowedSpecialValues AllowInputSpecialValues
        {
            get { return (AllowedSpecialValues)GetValue(AllowInputSpecialValuesProperty); }
            set { SetValue(AllowInputSpecialValuesProperty, value); }
        }

        #endregion //AllowInputSpecialValues

        #endregion

        #region Base Class Overrides

        protected override double? OnCoerceIncrement(double? baseValue)
        {
            if (baseValue.HasValue && double.IsNaN(baseValue.Value))
                throw new ArgumentException("NaN is invalid for Increment.");

            return base.OnCoerceIncrement(baseValue);
        }

        protected override double? OnCoerceMaximum(double? baseValue)
        {
            if (baseValue.HasValue && double.IsNaN(baseValue.Value))
                throw new ArgumentException("NaN is invalid for Maximum.");

            return base.OnCoerceMaximum(baseValue);
        }

        protected override double? OnCoerceMinimum(double? baseValue)
        {
            if (baseValue.HasValue && double.IsNaN(baseValue.Value))
                throw new ArgumentException("NaN is invalid for Minimum.");

            return base.OnCoerceMinimum(baseValue);
        }

        protected override double IncrementValue(double value, double increment)
        {
            return value + increment;
        }

        protected override double DecrementValue(double value, double increment)
        {
            return value - increment;
        }

        protected override void SetValidSpinDirection()
        {
            if (Value.HasValue && double.IsInfinity(Value.Value) && (Spinner != null))
            {
                Spinner.ValidSpinDirection = ValidSpinDirections.None;
            }
            else
            {
                base.SetValidSpinDirection();
            }
        }

        protected override double? ConvertTextToValue(string text)
        {
            double? result = base.ConvertTextToValue(text);
            if (result != null)
            {
                if (double.IsNaN(result.Value))
                    TestInputSpecialValue(this.AllowInputSpecialValues, AllowedSpecialValues.NaN);
                else if (double.IsPositiveInfinity(result.Value))
                    TestInputSpecialValue(this.AllowInputSpecialValues, AllowedSpecialValues.PositiveInfinity);
                else if (double.IsNegativeInfinity(result.Value))
                    TestInputSpecialValue(this.AllowInputSpecialValues, AllowedSpecialValues.NegativeInfinity);
            }

            return result;
        }

        #endregion
    }
}
