//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    a0174913-8167-4a42-998f-07b4d60c48d7
//        CLR Version:              4.0.30319.18444
//        Name:                     MessageType
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Communications
//        File Name:                MessageType
//
//        created by Charley at 2015/3/5 16:30:59
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.UMP.Communications
{
    /// <summary>
    /// 消息类型
    /// </summary>
    public enum MessageType
    {
        /// <summary>
        /// 未知
        /// </summary>
        Unkown = 0,
        /// <summary>
        /// 通知消息
        /// </summary>
        Notify = 1,
        /// <summary>
        /// 请求消息
        /// </summary>
        Request = 2,
        /// <summary>
        /// 应答消息
        /// </summary>
        Response = 3,
        /// <summary>
        /// 原始字节流数据
        /// </summary>
        RawData = 10,
    }
}
