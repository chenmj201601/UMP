//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    29ffe521-3600-4e77-8393-75977b6d4138
//        CLR Version:              4.0.30319.18063
//        Name:                     ScreenMPDefines
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.SDKs.ScreenMP
//        File Name:                ScreenMPDefines
//
//        created by Charley at 2015/7/17 15:54:03
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.SDKs.ScreenMP
{
    /// <summary>
    /// 录屏监视的定义
    /// </summary>
    public class ScreenMPDefines
    {
        //操作返回值
        public const int VLSM_RET_BASE = 400;
        public const int VLSM_ERR_THREAD_PROTECTED = VLSM_RET_BASE + 1;
        public const int VLSM_RET_UNKNOWN_ERR = VLSM_RET_BASE + 2;
        public const int VLSM_RET_ALREADY_CONNECTED = VLSM_RET_BASE + 3;
        public const int VLSM_RET_SOCKET_ERR = VLSM_RET_BASE + 4;
        public const int VLSM_RET_FILE_NOTFOUND = VLSM_RET_BASE + 5;
        public const int VLSM_RET_INVALIDFORMAT = VLSM_RET_BASE + 6;
        public const int VLSM_RET_DEVICE_BUSY = VLSM_RET_BASE + 7;
        public const int VLSM_RET_MSG_NO_BUF = VLSM_RET_BASE + 8;
        public const int VLSM_RET_NOT_CONNECTED = VLSM_RET_BASE + 9;
        public const int VLSM_RET_INVALID_PARAM = VLSM_RET_BASE + 10;

        //事件
        public const int SMEVT_BASE = 300;
        public const int SMEVT_CONNECTION_LOST = SMEVT_BASE + 1;
        public const int SMEVT_PLAY_COMPLETED = SMEVT_BASE + 2;
        public const int SMEVT_WATERMARK = SMEVT_BASE + 3;
        public const int SMEVT_UNKNOWN_ERROR = SMEVT_BASE + 4;
        public const int SMEVT_WND_CLOSE = SMEVT_BASE + 5;
        public const int SMEVT_MONIT_START = SMEVT_BASE + 6;
        public const int SMEVT_PLAY_OVER = SMEVT_BASE + 7;

        //Windows消息定义
        public const int WM_USER = 0x0400;
        public const int WM_CONNECTION_LOST = WM_USER + SMEVT_CONNECTION_LOST;
        public const int WM_PLAY_COMPLETED = WM_USER + SMEVT_PLAY_COMPLETED;
        public const int WM_WATERMARK = WM_USER + SMEVT_WATERMARK;
        public const int WM_UNKNOWN_ERROR = WM_USER + SMEVT_UNKNOWN_ERROR;
        public const int WM_WND_CLOSE = WM_USER + SMEVT_WND_CLOSE;
        public const int WM_MONIT_START = WM_USER + SMEVT_MONIT_START;
        public const int VM_PLAY_OVER = WM_USER + SMEVT_PLAY_OVER;


        //其他
        public const int SIZE_IPADDRESS = 16;

    }
}
