//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    1a3b7fce-35a2-43e7-ad1d-6cbd3ac51352
//        CLR Version:              4.0.30319.34003
//        Name:                     AcmDriverDetailsSupportFlags
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.NAudio.Wave.Compression
//        File Name:                AcmDriverDetailsSupportFlags
//
//        created by Charley at 2013/12/1 13:14:22
//        http://www.voicecyber.com 
//
//======================================================================
using System;

namespace VoiceCyber.NAudio.Wave.Compression
{
    /// <summary>
    /// Flags indicating what support a particular ACM driver has
    /// </summary>
    [Flags]
    public enum AcmDriverDetailsSupportFlags
    {
        /// <summary>ACMDRIVERDETAILS_SUPPORTF_CODEC - Codec</summary>
        Codec = 0x00000001,
        /// <summary>ACMDRIVERDETAILS_SUPPORTF_CONVERTER - Converter</summary>
        Converter = 0x00000002,
        /// <summary>ACMDRIVERDETAILS_SUPPORTF_FILTER - Filter</summary>
        Filter = 0x00000004,
        /// <summary>ACMDRIVERDETAILS_SUPPORTF_HARDWARE - Hardware</summary>
        Hardware = 0x00000008,
        /// <summary>ACMDRIVERDETAILS_SUPPORTF_ASYNC - Async</summary>
        Async = 0x00000010,
        /// <summary>ACMDRIVERDETAILS_SUPPORTF_LOCAL - Local</summary>
        Local = 0x40000000,
        /// <summary>ACMDRIVERDETAILS_SUPPORTF_DISABLED - Disabled</summary>
        Disabled = unchecked((int)0x80000000),
    }
}
