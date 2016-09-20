//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    769b6007-4b35-4295-9f1b-c6266cefa216
//        CLR Version:              4.0.30319.18063
//        Name:                     Structs
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.SDKs.ScreenMP
//        File Name:                Structs
//
//        created by Charley at 2015/7/17 14:19:47
//        http://www.voicecyber.com 
//
//======================================================================

using System.Runtime.InteropServices;

namespace VoiceCyber.SDKs.ScreenMP
{
    /// <summary>
    /// 录屏监视参数
    /// </summary>
    [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct SRCMON_PARAM
    {

        /// TCHAR[16]
        [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 16)]
        public byte[] sVocIp;

        /// USHORT->unsigned short
        public ushort nPort;

        /// USHORT->unsigned short
        public ushort nChannel;
    }

    /// <summary>
    /// 录屏监视事件参数
    /// </summary>
    [StructLayoutAttribute(LayoutKind.Sequential)]
    public struct SRCMON_EVT
    {

        /// int
        public int nEvtCode;

        /// int
        public int nSubCode;
    }

    /// 回调函数
    /// Return Type: void
    ///pNetMonEvt: PSRCMON_EVT->_SRCMON_EVT*
    ///lpData: LPARAM->LONG_PTR->int
    public delegate void VLSMEVENT_CB(ref SRCMON_EVT pNetMonEvt, System.IntPtr lpData);
}
