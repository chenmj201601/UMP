//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    e61d46d6-df95-4063-8b13-8cde6176ccb5
//        CLR Version:              4.0.30319.18063
//        Name:                     NMonDefines
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.SDKs.NMon
//        File Name:                NMonDefines
//
//        created by Charley at 2015/6/18 15:59:59
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.SDKs.NMon
{
    /// <summary>
    /// 相关定义
    /// </summary>
    public class NMonDefines
    {
        public const int NM_KEEP_ON_IDLE = 1;
        public const int NM_LEFT_CHANNEL = 2;
        public const int NM_RIGHT_CHANNEL = 4;

        public const int NM_EVT_BASE = 0x1000;
        public const int NM_EVT_INITED = NM_EVT_BASE + 1;
        public const int NM_EVT_CLOSED = NM_EVT_BASE + 2;
        public const int NM_EVT_VOCCONNECTED = NM_EVT_BASE + 3;
        public const int NM_EVT_MONITORSTARTSTOP = NM_EVT_BASE + 4;

        public const int CONVERTOR_BUFFERSIZE = 10 * 1024;

        public const int CONNECTOR_RECONINTEVAL = 10;
        public const int CONNECTOR_SERVERPORT = 3002;
        public const int CONNECTOR_BUFFERSIZE = 2048;
    }
}
