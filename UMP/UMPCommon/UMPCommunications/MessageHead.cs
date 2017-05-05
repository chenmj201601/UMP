//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    eb1f4550-f88e-4a1f-b12e-befce739fce3
//        CLR Version:              4.0.30319.18444
//        Name:                     NetMessageHead
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Communications
//        File Name:                NetMessageHead
//
//        created by Charley at 2015/3/5 13:20:01
//        http://www.voicecyber.com 
//
//======================================================================

using System.Runtime.InteropServices;

namespace VoiceCyber.UMP.Communications
{
    /// <summary>
    /// 消息头结构
    /// 消息头固定大小为26个Byte，指明消息的编码，加密，状态，类型，命令，大小等信息
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct MessageHead
    {
        /// <summary>
        /// 对齐标识
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 2)]
        public string Flag;
        /// <summary>
        /// 编码方式
        /// </summary>
        public int Encoding;
        /// <summary>
        /// 加密方式
        /// </summary>
        public int Encryption;
        /// <summary>
        /// 状态标识，这是个按位的标识
        /// </summary>
        public int State;
        /// <summary>
        /// 消息类型
        /// </summary>
        public int Type;
        /// <summary>
        /// 消息指令（模块内唯一标识消息的内容）
        /// </summary>
        public int Command;
        /// <summary>
        /// 消息主体部分大小
        /// </summary>
        public int Size;
    }
}
