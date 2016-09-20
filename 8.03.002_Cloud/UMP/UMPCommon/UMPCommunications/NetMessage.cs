//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    fd731d74-3f25-4980-be99-0a6f45b16854
//        CLR Version:              4.0.30319.18444
//        Name:                     NetMessage
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Communications
//        File Name:                NetMessage
//
//        created by Charley at 2015/3/5 16:28:53
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace VoiceCyber.UMP.Communications
{
    /// <summary>
    /// 网络消息，用Socket通讯，定义通讯消息的一般格式
    /// </summary>
    public class NetMessage
    {
        public MessageHead Head;
        public int Command;
        public long MsgID { get; set; }
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string UserData;

        public string StringData;
    }
}
