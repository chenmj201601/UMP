//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    d6990474-879a-4971-8440-191cac5419a6
//        CLR Version:              4.0.30319.18444
//        Name:                     WaveHeader
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.NAudio.Wave
//        File Name:                WaveHeader
//
//        created by Charley at 2014/12/8 15:22:17
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Runtime.InteropServices;

namespace VoiceCyber.NAudio.Wave
{
    /// <summary>
    /// WaveHeader interop structure (WAVEHDR)
    /// http://msdn.microsoft.com/en-us/library/dd743837%28VS.85%29.aspx
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    class WaveHeader
    {
        /// <summary>pointer to locked data buffer (lpData)</summary>
        public IntPtr dataBuffer;
        /// <summary>length of data buffer (dwBufferLength)</summary>
        public int bufferLength;
        /// <summary>used for input only (dwBytesRecorded)</summary>
        public int bytesRecorded;
        /// <summary>for client's use (dwUser)</summary>
        public IntPtr userData;
        /// <summary>assorted flags (dwFlags)</summary>
        public WaveHeaderFlags flags;
        /// <summary>loop control counter (dwLoops)</summary>
        public int loops;
        /// <summary>PWaveHdr, reserved for driver (lpNext)</summary>
        public IntPtr next;
        /// <summary>reserved for driver</summary>
        public IntPtr reserved;
    }
}
