//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    7ed555ee-e1c2-492c-9fe9-c1f652230ff1
//        CLR Version:              4.0.30319.34003
//        Name:                     AcmFormatDetails
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.NAudio.Wave.Compression
//        File Name:                AcmFormatDetails
//
//        created by Charley at 2013/12/1 13:15:08
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Runtime.InteropServices;

namespace VoiceCyber.NAudio.Wave.Compression
{
    /// <summary>
    /// ACMFORMATDETAILS
    /// http://msdn.microsoft.com/en-us/library/dd742913%28VS.85%29.aspx
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 4)]
    struct AcmFormatDetails
    {
        /// <summary>
        /// DWORD cbStruct; 
        /// </summary>
        public int structSize;
        /// <summary>
        /// DWORD dwFormatIndex; 
        /// </summary>
        public int formatIndex;
        /// <summary>
        /// DWORD dwFormatTag; 
        /// </summary>
        public int formatTag;
        /// <summary>
        /// DWORD fdwSupport; 
        /// </summary>
        public AcmDriverDetailsSupportFlags supportFlags;
        /// <summary>
        /// LPWAVEFORMATEX pwfx; 
        /// </summary>    
        public IntPtr waveFormatPointer;
        /// <summary>
        /// DWORD cbwfx; 
        /// </summary>
        public int waveFormatByteSize;
        /// <summary>
        /// TCHAR szFormat[ACMFORMATDETAILS_FORMAT_CHARS];
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = FormatDescriptionChars)]
        public string formatDescription;

        /// <summary>
        /// ACMFORMATDETAILS_FORMAT_CHARS
        /// </summary>
        public const int FormatDescriptionChars = 128;
    }
}
