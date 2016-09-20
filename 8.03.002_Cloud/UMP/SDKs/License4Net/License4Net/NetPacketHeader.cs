//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    b109afcb-6273-4c3c-9420-006c244ee673
//        CLR Version:              4.0.30319.18063
//        Name:                     NetPacketHeader
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.SDKs.Licenses
//        File Name:                NetPacketHeader
//
//        created by Charley at 2015/7/27 10:42:23
//        http://www.voicecyber.com 
//
//======================================================================

using System.Runtime.InteropServices;

namespace VoiceCyber.SDKs.Licenses
{
    /// <summary>
    /// 消息头结构
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class NetPacketHeader
    {
        /// char[2]
        /// 包同步标志('L','M')
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] Flag;

        /// unsigned short
        /// 状态标志，右起
        /// 1   bit         最后一个网络包。对于发送方，发送了此数据后将断开网络。
        /// 2   bit         有后续协议包。当一个协议包超过一个网络包能保存的最大大小时，需要分为多个网络包发送，除最后一个包外，前面的所有包需要此标志  
        /// 3   bit         本数据包是否被加密处理     
        public ushort State;

        /// unsigned char
        ///  数据包格式
        public byte Format;

        /// unsigned char[3]
        /// 保留1
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] Reserved1;

        /// size_t->unsigned int
        /// 保留2
        public uint Reserved2;

        /// size_t->unsigned int
        /// 数据包大小
        public uint Size;
    }
}
