//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    f52b08ce-5858-4621-b464-6db235f5369f
//        CLR Version:              4.0.30319.18444
//        Name:                     MessageEncoding
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Communications
//        File Name:                MessageEncoding
//
//        created by Charley at 2015/3/5 13:26:42
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.UMP.Communications
{
    /// <summary>
    /// 消息编码方式
    /// </summary>
    public enum MessageEncoding
    {
        /// <summary>
        /// 无编码，默认方式
        /// </summary>
        None = 0,
        /// <summary>
        /// UTF8编码的字符串
        /// </summary>
        UTF8String = 1,
        /// <summary>
        /// UTF8编码的Json串
        /// </summary>
        UTF8JSON = 2,
        /// <summary>
        /// UTF8编码的Xml
        /// </summary>
        UTF8XML = 3,
    }
}
