//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    63358b57-ccdf-40f1-98f2-10d63919e7a6
//        CLR Version:              4.0.30319.18444
//        Name:                     SpinEventArgs
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.CustomControls.ButtonSpinner.Implementation
//        File Name:                SpinEventArgs
//
//        created by Charley at 2014/7/17 16:18:28
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace VoiceCyber.Wpf.CustomControls
{
    /// <summary>
    /// Provides data for the Spinner.Spin event.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    public class SpinEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// Gets the SpinDirection for the spin that has been initiated by the 
        /// end-user.
        /// </summary>
        public SpinDirection Direction
        {
            get;
            private set;
        }

        /// <summary>
        /// Get or set whheter the spin event originated from a mouse wheel event.
        /// </summary>
        public bool UsingMouseWheel
        {
            get;
            private set;
        }

        /// <summary>
        /// Initializes a new instance of the SpinEventArgs class.
        /// </summary>
        /// <param name="direction">Spin direction.</param>
        public SpinEventArgs(SpinDirection direction)
            : base()
        {
            Direction = direction;
        }

        public SpinEventArgs(SpinDirection direction, bool usingMouseWheel)
            : base()
        {
            Direction = direction;
            UsingMouseWheel = usingMouseWheel;
        }
    }
}
