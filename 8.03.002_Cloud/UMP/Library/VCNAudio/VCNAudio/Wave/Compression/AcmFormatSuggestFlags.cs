//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    27bc04ed-3e1a-48d0-88d9-671e6f4aa7b7
//        CLR Version:              4.0.30319.34003
//        Name:                     AcmFormatSuggestFlags
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.NAudio.Wave.Compression
//        File Name:                AcmFormatSuggestFlags
//
//        created by Charley at 2013/12/1 13:21:01
//        http://www.voicecyber.com 
//
//======================================================================
using System;

namespace VoiceCyber.NAudio.Wave.Compression
{
    [Flags]
    enum AcmFormatSuggestFlags
    {
        /// <summary>
        /// ACM_FORMATSUGGESTF_WFORMATTAG
        /// </summary>
        FormatTag = 0x00010000,
        /// <summary>
        /// ACM_FORMATSUGGESTF_NCHANNELS
        /// </summary>
        Channels = 0x00020000,
        /// <summary>
        /// ACM_FORMATSUGGESTF_NSAMPLESPERSEC
        /// </summary>
        SamplesPerSecond = 0x00040000,
        /// <summary>
        /// ACM_FORMATSUGGESTF_WBITSPERSAMPLE
        /// </summary>
        BitsPerSample = 0x00080000,
        /// <summary>
        /// ACM_FORMATSUGGESTF_TYPEMASK
        /// </summary>
        TypeMask = 0x00FF0000,
    }
}
