//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    230102ae-3c3f-4eed-a84e-fc2734056d0e
//        CLR Version:              4.0.30319.18063
//        Name:                     MessageReceivedEventArgs
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.SDKs.DEC
//        File Name:                MessageReceivedEventArgs
//
//        created by Charley at 2015/6/17 13:24:56
//        http://www.voicecyber.com 
//
//======================================================================
using System;

namespace VoiceCyber.SDKs.DEC
{
    /// <summary>
    /// DEC订阅消息的参数
    /// </summary>
    public class MessageReceivedEventArgs : EventArgs
    {
        /// <summary>
        /// 识别头
        /// </summary>
        public DistHead DistHead { get; set; }
        /// <summary>
        /// 应用层消息头
        /// </summary>
        public AppHead AppHead { get; set; }
        /// <summary>
        /// 基本头数据
        /// </summary>
        public byte[] BaseHead { get; set; }
        /// <summary>
        /// 扩展头数据
        /// </summary>
        public byte[] ExtHead { get; set; }
        /// <summary>
        /// 数据区数据
        /// </summary>
        public byte[] Data { get; set; }
    }
}
