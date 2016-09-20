//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    17406a9a-2c88-491f-bc24-9045e5564450
//        CLR Version:              4.0.30319.34003
//        Name:                     AcmFormatTagDetails
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.NAudio.Wave.Compression
//        File Name:                AcmFormatTagDetails
//
//        created by Charley at 2013/12/1 13:15:47
//        http://www.voicecyber.com 
//
//======================================================================

using System.Runtime.InteropServices;

namespace VoiceCyber.NAudio.Wave.Compression
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    struct AcmFormatTagDetails
    {
        /// <summary>
        /// DWORD cbStruct; 
        /// </summary>
        public int structureSize;
        /// <summary>
        /// DWORD dwFormatTagIndex; 
        /// </summary>
        public int formatTagIndex;
        /// <summary>
        /// DWORD dwFormatTag; 
        /// </summary>
        public int formatTag;
        /// <summary>
        /// DWORD cbFormatSize; 
        /// </summary>
        public int formatSize;
        /// <summary>
        /// DWORD fdwSupport;
        /// </summary>
        public AcmDriverDetailsSupportFlags supportFlags;
        /// <summary>
        /// DWORD cStandardFormats; 
        /// </summary>
        public int standardFormatsCount;
        /// <summary>
        /// TCHAR szFormatTag[ACMFORMATTAGDETAILS_FORMATTAG_CHARS]; 
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = FormatTagDescriptionChars)]
        public string formatDescription;



        /// <summary>
        /// ACMFORMATTAGDETAILS_FORMATTAG_CHARS
        /// </summary>
        public const int FormatTagDescriptionChars = 48;

    }
}
