//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    010d0200-70b0-40b1-a99a-2f0c64f1e80b
//        CLR Version:              4.0.30319.18444
//        Name:                     WaveFilter
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.NAudio.Wave
//        File Name:                WaveFilter
//
//        created by Charley at 2014/12/8 15:48:34
//        http://www.voicecyber.com 
//
//======================================================================

using System.Runtime.InteropServices;

namespace VoiceCyber.NAudio.Wave
{
    /// <summary>
    /// Summary description for WaveFilter.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public class WaveFilter
    {
        /// <summary>
        /// cbStruct
        /// </summary>
        public int StructureSize = Marshal.SizeOf(typeof(WaveFilter));
        /// <summary>
        /// dwFilterTag
        /// </summary>
        public int FilterTag = 0;
        /// <summary>
        /// fdwFilter
        /// </summary>
        public int Filter = 0;
        /// <summary>
        /// reserved
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
        public int[] Reserved = null;
    }
}
