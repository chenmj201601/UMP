//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    a306f2d9-214c-42d9-a4cf-cd119137d22e
//        CLR Version:              4.0.30319.18063
//        Name:                     ByteOrder
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.WebSockets
//        File Name:                ByteOrder
//
//        created by Charley at 2015/10/8 17:56:50
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.WebSockets
{
    /// <summary>
    /// Contains the values that indicate whether the byte order is a Little-endian or Big-endian.
    /// </summary>
    public enum ByteOrder : byte
    {
        /// <summary>
        /// Indicates a Little-endian.
        /// </summary>
        Little,
        /// <summary>
        /// Indicates a Big-endian.
        /// </summary>
        Big
    }
}
