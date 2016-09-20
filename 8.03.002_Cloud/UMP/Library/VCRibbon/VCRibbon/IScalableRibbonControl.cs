//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    ffc1350e-398a-45cf-b0b1-997c1b81842d
//        CLR Version:              4.0.30319.18444
//        Name:                     IScalableRibbonControl
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Fluent
//        File Name:                IScalableRibbonControl
//
//        created by Charley at 2014/5/27 17:37:30
//        http://www.voicecyber.com 
//
//======================================================================
using System;

namespace VoiceCyber.Ribbon
{
    /// <summary>
    /// Repesents scalable ribbon contol
    /// </summary>
    public interface IScalableRibbonControl
    {
        /// <summary>
        /// Enlarge control size
        /// </summary>
        void Enlarge();
        /// <summary>
        /// Reduce control size
        /// </summary>
        void Reduce();

        /// <summary>
        /// Occurs when contol is scaled
        /// </summary>
        event EventHandler Scaled;
    }
}
