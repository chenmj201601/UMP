//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    4fe49ef6-fb9c-4426-9a5e-8becda01880a
//        CLR Version:              4.0.30319.18063
//        Name:                     MessageReceivedEventArgs
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.SDKs.Licenses
//        File Name:                MessageReceivedEventArgs
//
//        created by Charley at 2015/7/27 11:05:42
//        http://www.voicecyber.com 
//
//======================================================================
using System;

namespace VoiceCyber.SDKs.Licenses
{
    /// <summary>
    /// 消息参数
    /// </summary>
    public class MessageReceivedEventArgs : EventArgs
    {
        /// <summary>
        /// 自定义名称
        /// </summary>
        public string Client { get; set; }
        /// <summary>
        /// 消息头
        /// </summary>
        public NetPacketHeader Header { get; set; }
        /// <summary>
        /// 数据大小
        /// </summary>
        public int DataSize { get; set; }
        /// <summary>
        /// 数据内容
        /// </summary>
        public byte[] Data { get; set; }
        /// <summary>
        /// 数据内容（以字符串表示）
        /// </summary>
        public string StringData { get; set; }
    }
}
