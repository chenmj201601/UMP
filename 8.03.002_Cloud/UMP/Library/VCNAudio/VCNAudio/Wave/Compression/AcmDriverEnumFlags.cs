//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    395e44ec-7e15-4119-9c91-4a03c0c3bb1a
//        CLR Version:              4.0.30319.34003
//        Name:                     AcmDriverEnumFlags
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.NAudio.Wave.Compression
//        File Name:                AcmDriverEnumFlags
//
//        created by Charley at 2013/12/1 13:16:42
//        http://www.voicecyber.com 
//
//======================================================================
using System;

namespace VoiceCyber.NAudio.Wave.Compression
{
    [Flags]
    enum AcmDriverEnumFlags
    {
        /// <summary>
        /// ACM_DRIVERENUMF_NOLOCAL, Only global drivers should be included in the enumeration
        /// </summary>
        NoLocal = 0x40000000,
        /// <summary>
        /// ACM_DRIVERENUMF_DISABLED, Disabled ACM drivers should be included in the enumeration
        /// </summary>
        Disabled = unchecked((int)0x80000000),
    }
}
