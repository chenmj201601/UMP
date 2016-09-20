//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    e0cc5077-68b0-447d-aeeb-c64f6c629aeb
//        CLR Version:              4.0.30319.34003
//        Name:                     AcmStreamHeaderStatusFlags
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.NAudio.Wave.Compression
//        File Name:                AcmStreamHeaderStatusFlags
//
//        created by Charley at 2013/12/1 13:12:18
//        http://www.voicecyber.com 
//
//======================================================================
using System;

namespace VoiceCyber.NAudio.Wave.Compression
{
    [Flags]
    enum AcmStreamHeaderStatusFlags
    {
        /// <summary>
        /// ACMSTREAMHEADER_STATUSF_DONE
        /// </summary>
        Done = 0x00010000,
        /// <summary>
        /// ACMSTREAMHEADER_STATUSF_PREPARED
        /// </summary>
        Prepared = 0x00020000,
        /// <summary>
        /// ACMSTREAMHEADER_STATUSF_INQUEUE
        /// </summary>
        InQueue = 0x00100000,
    }
}
