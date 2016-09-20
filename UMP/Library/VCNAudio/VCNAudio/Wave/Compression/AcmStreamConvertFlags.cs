//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    ec00997b-bc64-42b3-9f37-3852aa0887fa
//        CLR Version:              4.0.30319.34003
//        Name:                     AcmStreamConvertFlags
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.NAudio.Wave.Compression
//        File Name:                AcmStreamConvertFlags
//
//        created by Charley at 2013/12/1 13:25:10
//        http://www.voicecyber.com 
//
//======================================================================
using System;

namespace VoiceCyber.NAudio.Wave.Compression
{
    [Flags]
    enum AcmStreamConvertFlags
    {
        /// <summary>
        /// ACM_STREAMCONVERTF_BLOCKALIGN
        /// </summary>
        BlockAlign = 0x00000004,
        /// <summary>
        /// ACM_STREAMCONVERTF_START
        /// </summary>
        Start = 0x00000010,
        /// <summary>
        /// ACM_STREAMCONVERTF_END
        /// </summary>
        End = 0x00000020,
    }
}
