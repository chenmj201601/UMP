//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    f8fdacaf-3102-471b-aec8-e392708b83a5
//        CLR Version:              4.0.30319.18444
//        Name:                     AllowedSpecialValuesEnum
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.CustomControls.NumericUpDown.Implementation
//        File Name:                AllowedSpecialValuesEnum
//
//        created by Charley at 2014/7/18 15:13:28
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Text;

namespace VoiceCyber.Wpf.CustomControls
{
    [Flags]
    public enum AllowedSpecialValues
    {
        None = 0,
        NaN = 1,
        PositiveInfinity = 2,
        NegativeInfinity = 4,
        AnyInfinity = PositiveInfinity | NegativeInfinity,
        Any = NaN | AnyInfinity
    }
}
