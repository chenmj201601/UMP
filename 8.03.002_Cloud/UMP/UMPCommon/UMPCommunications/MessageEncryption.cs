//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    a6c37f23-c648-4eb3-b5f6-7f2490998d2f
//        CLR Version:              4.0.30319.18444
//        Name:                     MessageEncryption
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Communications
//        File Name:                MessageEncryption
//
//        created by Charley at 2015/3/5 13:31:16
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.UMP.Communications
{
    /// <summary>
    /// 消息加密版本
    /// </summary>
    public enum MessageEncryption
    {
        /// <summary>
        /// 无加密
        /// </summary>
        None,
        /// <summary>
        /// M001加密
        /// </summary>
        M001,
        /// <summary>
        /// M002加密
        /// </summary>
        M002,
        /// <summary>
        /// M004加密
        /// </summary>
        M004,
    }
}
