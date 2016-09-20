//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    13be5235-721a-4e4c-a8f1-b576da9fbed2
//        CLR Version:              4.0.30319.18444
//        Name:                     Spinner
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.CustomControls.ButtonSpinner.Implementation
//        File Name:                Spinner
//
//        created by Charley at 2014/7/17 16:17:08
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace VoiceCyber.Wpf.CustomControls
{
    /// <summary>
    /// Base class for controls that represents controls that can spin.
    /// </summary>
    public abstract class Spinner : Control
    {
        #region Properties

        /// <summary>
        /// Identifies the ValidSpinDirection dependency property.
        /// </summary>
        public static readonly DependencyProperty ValidSpinDirectionProperty = DependencyProperty.Register("ValidSpinDirection", typeof(ValidSpinDirections), typeof(Spinner), new PropertyMetadata(ValidSpinDirections.Increase | ValidSpinDirections.Decrease, OnValidSpinDirectionPropertyChanged));
        public ValidSpinDirections ValidSpinDirection
        {
            get
            {
                return (ValidSpinDirections)GetValue(ValidSpinDirectionProperty);
            }
            set
            {
                SetValue(ValidSpinDirectionProperty, value);
            }
        }

        /// <summary>
        /// ValidSpinDirectionProperty property changed handler.
        /// </summary>
        /// <param name="d">ButtonSpinner that changed its ValidSpinDirection.</param>
        /// <param name="e">Event arguments.</param>
        private static void OnValidSpinDirectionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Spinner source = (Spinner)d;
            ValidSpinDirections oldvalue = (ValidSpinDirections)e.OldValue;
            ValidSpinDirections newvalue = (ValidSpinDirections)e.NewValue;
            source.OnValidSpinDirectionChanged(oldvalue, newvalue);
        }

        #endregion //Properties

        /// <summary>
        /// Occurs when spinning is initiated by the end-user.
        /// </summary>
        public event EventHandler<SpinEventArgs> Spin;

        /// <summary>
        /// Initializes a new instance of the Spinner class.
        /// </summary>
        protected Spinner()
        {
        }

        /// <summary>
        /// Raises the OnSpin event when spinning is initiated by the end-user.
        /// </summary>
        /// <param name="e">Spin event args.</param>
        protected virtual void OnSpin(SpinEventArgs e)
        {
            ValidSpinDirections valid = e.Direction == SpinDirection.Increase ? ValidSpinDirections.Increase : ValidSpinDirections.Decrease;

            //Only raise the event if spin is allowed.
            if ((ValidSpinDirection & valid) == valid)
            {
                EventHandler<SpinEventArgs> handler = Spin;
                if (handler != null)
                {
                    handler(this, e);
                }
            }
        }

        /// <summary>
        /// Called when valid spin direction changed.
        /// </summary>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        protected virtual void OnValidSpinDirectionChanged(ValidSpinDirections oldValue, ValidSpinDirections newValue)
        {
        }
    }
}
