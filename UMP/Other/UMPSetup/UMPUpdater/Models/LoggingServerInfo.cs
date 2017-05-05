//======================================================================
//
//        Copyright © 2016-2017 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        GUID1:                    98f91fbc-a456-4b51-bcf4-1ee8de82874b
//        CLR Version:              4.0.30319.42000
//        Name:                     LoggingServerInfo
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             VoiceCyber
//        Namespace:                UMPUpdater.Models
//        File Name:                LoggingServerInfo
//
//        Created by Charley at 2016/9/7 12:24:36
//        http://www.voicecyber.com 
//
//======================================================================

using System.IO;
using System.Net.Sockets;
using VoiceCyber.UMP.Common;

namespace UMPUpdater.Models
{
    public class LoggingServerInfo
    {
        public long ObjID { get; set; }
        public string HostAddress { get; set; }
        public int HostPort { get; set; }

        public bool IsConnected { get; set; }

        public byte[] Buffer { get; set; }
        public int BufferedSize { get; set; }
        public TcpClient TcpClient { get; set; }
        public Stream Stream { get; set; }

        public string RecieveMessage { get; set; }

        public string Token { get; set; }

        public double Progress { get; set; }
        public int UpdateFlag { get; set; }

        public LoggingServerInfo()
        {
            BufferedSize = 0;
            Buffer = new byte[ConstValue.NET_BUFFER_MAX_SIZE];
        }
    }
}
