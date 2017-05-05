//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    788c8b72-71c7-4c37-b081-6ac51401a5ec
//        CLR Version:              4.0.30319.18444
//        Name:                     ObjectPropertyEncryptMode
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                Common11101
//        File Name:                ObjectPropertyEncryptMode
//
//        created by Charley at 2015/1/30 11:06:18
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.UMP.Common11101
{
    /// <summary>
    /// 属性值的加密方式
    /// </summary>
    public enum ObjectPropertyEncryptMode
    {
        /// <summary>
        /// 未知(不加密)
        /// </summary>
        Unkown = 0,
        /// <summary>
        /// 加密版本2，模式hex，AES加密
        /// </summary>
        E2Hex = 21,
        /// <summary>
        /// SHA256加密
        /// </summary>
        SHA256 = 31
    }
}
