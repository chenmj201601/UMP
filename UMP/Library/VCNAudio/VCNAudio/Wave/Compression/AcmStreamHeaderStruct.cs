//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    de249bc7-5221-4737-9f36-6ef5e2185302
//        CLR Version:              4.0.30319.34003
//        Name:                     AcmStreamHeaderStruct
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.NAudio.Wave.Compression
//        File Name:                AcmStreamHeaderStruct
//
//        created by Charley at 2013/12/1 13:11:44
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Runtime.InteropServices;

namespace VoiceCyber.NAudio.Wave.Compression
{
    /// <summary>
    /// Interop structure for ACM stream headers.
    /// ACMSTREAMHEADER 
    /// http://msdn.microsoft.com/en-us/library/dd742926%28VS.85%29.aspx
    /// </summary>    
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Size = 128)] // explicit size to make it work for x64
    class AcmStreamHeaderStruct
    {
        public int cbStruct;
        public AcmStreamHeaderStatusFlags fdwStatus = 0;
        public IntPtr userData;
        public IntPtr sourceBufferPointer;
        public int sourceBufferLength;
        public int sourceBufferLengthUsed;
        public IntPtr sourceUserData;
        public IntPtr destBufferPointer;
        public int destBufferLength;
        public int destBufferLengthUsed = 0;
        public IntPtr destUserData;

        // 10 reserved values follow this, we don't need to declare them
        // since we have set the struct size explicitly and don't
        // need to access them in client code (thanks Brian)
        /*public int reserved0;
        public int reserved1;
        public int reserved2;
        public int reserved3;
        public int reserved4;
        public int reserved5;
        public int reserved6;
        public int reserved7;
        public int reserved8;
        public int reserved9;*/
    }
}
