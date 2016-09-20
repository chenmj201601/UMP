//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    8349d8f9-b3c7-4408-bbd3-3a9db48389ba
//        CLR Version:              4.0.30319.18444
//        Name:                     WaveHeaderFlags
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.NAudio.Wave
//        File Name:                WaveHeaderFlags
//
//        created by Charley at 2014/12/8 15:22:39
//        http://www.voicecyber.com 
//
//======================================================================
using System;

namespace VoiceCyber.NAudio.Wave
{
    /// <summary>
    /// Wave Header Flags enumeration
    /// </summary>
    [Flags]
    public enum WaveHeaderFlags
    {
        /// <summary>
        /// WHDR_BEGINLOOP
        /// This buffer is the first buffer in a loop.  This flag is used only with output buffers.
        /// </summary>
        BeginLoop = 0x00000004,
        /// <summary>
        /// WHDR_DONE
        /// Set by the device driver to indicate that it is finished with the buffer and is returning it to the application.
        /// </summary>
        Done = 0x00000001,
        /// <summary>
        /// WHDR_ENDLOOP
        /// This buffer is the last buffer in a loop.  This flag is used only with output buffers.
        /// </summary>
        EndLoop = 0x00000008,
        /// <summary>
        /// WHDR_INQUEUE
        /// Set by Windows to indicate that the buffer is queued for playback.
        /// </summary>
        InQueue = 0x00000010,
        /// <summary>
        /// WHDR_PREPARED
        /// Set by Windows to indicate that the buffer has been prepared with the waveInPrepareHeader or waveOutPrepareHeader function.
        /// </summary>
        Prepared = 0x00000002
    }
}
