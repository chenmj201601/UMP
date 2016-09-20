//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    a1be5761-9881-4f5d-b1e6-14d0633f0791
//        CLR Version:              4.0.30319.18444
//        Name:                     WaveOutSupport
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.NAudio.Wave
//        File Name:                WaveOutSupport
//
//        created by Charley at 2014/12/8 15:21:10
//        http://www.voicecyber.com 
//
//======================================================================
using System;

namespace VoiceCyber.NAudio.Wave
{
    /// <summary>
    /// Flags indicating what features this WaveOut device supports
    /// </summary>
    [Flags]
    enum WaveOutSupport
    {
        /// <summary>supports pitch control (WAVECAPS_PITCH)</summary>
        Pitch = 0x0001,
        /// <summary>supports playback rate control (WAVECAPS_PLAYBACKRATE)</summary>
        PlaybackRate = 0x0002,
        /// <summary>supports volume control (WAVECAPS_VOLUME)</summary>
        Volume = 0x0004,
        /// <summary>supports separate left-right volume control (WAVECAPS_LRVOLUME)</summary>
        LRVolume = 0x0008,
        /// <summary>(WAVECAPS_SYNC)</summary>
        Sync = 0x0010,
        /// <summary>(WAVECAPS_SAMPLEACCURATE)</summary>
        SampleAccurate = 0x0020,
    }
}
