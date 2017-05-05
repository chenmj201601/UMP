//======================================================================
//
//        Copyright © 2016-2017 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        GUID1:                    0baf97b5-fb97-4f22-9d66-3c0d3d9f10c0
//        CLR Version:              4.0.30319.42000
//        Name:                     MessageReceivedEventArgs
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Communications
//        File Name:                MessageReceivedEventArgs
//
//        Created by Charley at 2016/9/11 17:49:48
//        http://www.voicecyber.com 
//
//======================================================================
using System;


namespace VoiceCyber.UMP.Communications
{
    public class MessageReceivedEventArgs : EventArgs
    {
        /// <summary>
        /// 会话名称，通常为SessionID
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 消息头信息
        /// </summary>
        public MessageHead Head { get; set; }
        /// <summary>
        /// 消息数据
        /// </summary>
        public byte[] Data { get; set; }
    }
}
