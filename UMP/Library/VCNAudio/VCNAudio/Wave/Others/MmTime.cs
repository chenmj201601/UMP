//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    24afc857-540b-4481-9ac9-bbe50b097a82
//        CLR Version:              4.0.30319.18444
//        Name:                     MmTime
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.NAudio.Wave
//        File Name:                MmTime
//
//        created by Charley at 2014/12/8 15:20:01
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Runtime.InteropServices;

namespace VoiceCyber.NAudio.Wave
{
    // http://msdn.microsoft.com/en-us/library/dd757347(v=VS.85).aspx
    [StructLayout(LayoutKind.Explicit)]
    struct MmTime
    {
        public const int TIME_MS = 0x0001;
        public const int TIME_SAMPLES = 0x0002;
        public const int TIME_BYTES = 0x0004;

        [FieldOffset(0)]
        public UInt32 wType;
        [FieldOffset(4)]
        public UInt32 ms;
        [FieldOffset(4)]
        public UInt32 sample;
        [FieldOffset(4)]
        public UInt32 cb;
        [FieldOffset(4)]
        public UInt32 ticks;
        [FieldOffset(4)]
        public Byte smpteHour;
        [FieldOffset(5)]
        public Byte smpteMin;
        [FieldOffset(6)]
        public Byte smpteSec;
        [FieldOffset(7)]
        public Byte smpteFrame;
        [FieldOffset(8)]
        public Byte smpteFps;
        [FieldOffset(9)]
        public Byte smpteDummy;
        [FieldOffset(10)]
        public Byte smptePad0;
        [FieldOffset(11)]
        public Byte smptePad1;
        [FieldOffset(4)]
        public UInt32 midiSongPtrPos;
    }
}
