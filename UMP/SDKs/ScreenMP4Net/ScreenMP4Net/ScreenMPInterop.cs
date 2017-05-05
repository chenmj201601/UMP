//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    0569c2e1-9916-4020-a51f-805af0b94cf7
//        CLR Version:              4.0.30319.18063
//        Name:                     ScreenMPInterop
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.SDKs.ScreenMP
//        File Name:                ScreenMPInterop
//
//        created by Charley at 2015/7/17 14:19:09
//        http://www.voicecyber.com 
//
//======================================================================

using System;
using System.Runtime.InteropServices;

namespace VoiceCyber.SDKs.ScreenMP
{
    /// <summary>
    /// 录屏监视平台调用
    /// </summary>
    public class ScreenMPInterop
    {
        /// Return Type: int
        ///pSrcMon: PSRCMON_PARAM->_SRCMON_PARAM*
        ///pSMonCB: VLSMEVENT_CB
        ///lpData: LPARAM->LONG_PTR->int
        [DllImportAttribute("ScreenMP.dll", EntryPoint = "VLSMonStart", CallingConvention = CallingConvention.StdCall)]
        public static extern int VLSMonStart(
            ref SRCMON_PARAM pSrcMon,
            VLSMEVENT_CB pSMonCB,
            [MarshalAsAttribute(UnmanagedType.I4)] int lpData);

        /// Return Type: int
        [DllImportAttribute("ScreenMP.dll", EntryPoint = "VLSMonStop", CallingConvention = CallingConvention.StdCall)]
        public static extern int VLSMonStop();

        /// Return Type: int
        ///sFilename: char*
        ///pSMonCB: VLSMEVENT_CB
        ///lpData: LPARAM->LONG_PTR->int
        [DllImportAttribute("ScreenMP.dll", EntryPoint = "VLSMonPlay", CallingConvention = CallingConvention.StdCall)]
        public static extern int VLSMonPlay(
            [InAttribute] 
            [MarshalAsAttribute(UnmanagedType.LPStr)] 
            string sFilename,
            VLSMEVENT_CB pSMonCB,
            [MarshalAsAttribute(UnmanagedType.I4)] 
            int lpData);

        /// Return Type: int
        ///sMessage: char*
        [DllImportAttribute("ScreenMP.dll", EntryPoint = "VLSMonSendMsg", CallingConvention = CallingConvention.StdCall)]
        public static extern int VLSMonSendMsg(
            [InAttribute] 
            [MarshalAsAttribute(UnmanagedType.LPStr)] 
            string sMessage);

        /// Return Type: int
        ///seconds: int
        [DllImportAttribute("ScreenMP.dll", EntryPoint = "VLSMonSetWaterMark", CallingConvention = CallingConvention.StdCall)]
        public static extern int VLSMonSetWaterMark(int seconds);


        /// Return Type: int
        ///sFilename: char*
        ///pSec: int*
        ///boldFileModel: BOOL->int
        [DllImportAttribute("ScreenMP.dll", EntryPoint = "VLSMonGetFileTotalTime", CallingConvention = CallingConvention.StdCall)]
        public static extern int VLSMonGetFileTotalTime(
            [InAttribute] 
            [MarshalAsAttribute(UnmanagedType.LPStr)] 
            string sFilename,
            ref int pSec,
            [MarshalAsAttribute(UnmanagedType.Bool)] 
            bool boldFileModel);


        /// Return Type: int
        ///nSecond: int
        [DllImportAttribute("ScreenMP.dll", EntryPoint = "VLSMonSetPlayPos", CallingConvention = CallingConvention.StdCall)]
        public static extern int VLSMonSetPlayPos(int nSecond);


        /// Return Type: int
        [DllImportAttribute("ScreenMP.dll", EntryPoint = "VLSMonCloseWnd", CallingConvention = CallingConvention.StdCall)]
        public static extern int VLSMonCloseWnd();


        /// Return Type: int
        ///percentage: int
        [DllImportAttribute("ScreenMP.dll", EntryPoint = "VLSMonSetScale", CallingConvention = CallingConvention.StdCall)]
        public static extern int VLSMonSetScale(int percentage);


        /// Return Type: int
        ///fSoundOn: BOOL->int
        [DllImportAttribute("ScreenMP.dll", EntryPoint = "VLSMonSetAudioOn", CallingConvention = CallingConvention.StdCall)]
        public static extern int VLSMonSetAudioOn(
            [MarshalAsAttribute(UnmanagedType.Bool)] 
            bool fSoundOn);


        /// Return Type: int
        ///fEnable: BOOL->int
        [DllImportAttribute("ScreenMP.dll", EntryPoint = "VLSMonSetFullScr", CallingConvention = CallingConvention.StdCall)]
        public static extern int VLSMonSetFullScr(
            [MarshalAsAttribute(UnmanagedType.Bool)] 
            bool fEnable);


        /// Return Type: int
        ///sFilename: char*
        ///pSMonCB: VLSMEVENT_CB
        ///lpData: LPARAM->LONG_PTR->int
        [DllImportAttribute("ScreenMP.dll", EntryPoint = "VLSMonMakeAVI", CallingConvention = CallingConvention.StdCall)]
        public static extern int VLSMonMakeAVI(
            [InAttribute] 
            [MarshalAsAttribute(UnmanagedType.LPStr)] 
            string sFilename,
            VLSMEVENT_CB pSMonCB,
            [MarshalAsAttribute(UnmanagedType.SysInt)] 
            int lpData);


        /// Return Type: int
        ///sCompressCodec: char*
        [DllImportAttribute("ScreenMP.dll", EntryPoint = "VLSMonSetAVICodec", CallingConvention = CallingConvention.StdCall)]
        public static extern int VLSMonSetAVICodec(
            [InAttribute] 
            [MarshalAsAttribute(UnmanagedType.LPStr)] 
            string sCompressCodec);


        /// Return Type: int
        ///sServerIP: char*
        ///dwPort: DWORD->unsigned int
        ///nChannel: int
        ///pSMonCB: VLSMEVENT_CB
        ///lpData: LPARAM->LONG_PTR->int
        [DllImportAttribute("ScreenMP.dll", EntryPoint = "VLSMonStart2", CallingConvention = CallingConvention.StdCall)]
        public static extern int VLSMonStart2(
            [InAttribute] 
            [MarshalAsAttribute(UnmanagedType.LPStr)] 
            string sServerIP,
            uint dwPort,
            int nChannel,
            VLSMEVENT_CB pSMonCB,
            [MarshalAsAttribute(UnmanagedType.SysInt)] 
            int lpData);


        /// Return Type: int
        ///sFilename: char*
        ///pSMonCB: VLSMEVENT_CB
        ///lpData: LPARAM->LONG_PTR->int
        ///dwMsgIDConnectionLost: DWORD->unsigned int
        ///dwMsgIDUnknowError: DWORD->unsigned int
        ///dwMsgIDPlayCompleted: DWORD->unsigned int
        ///dwMsgIDWaterMark: DWORD->unsigned int
        ///m_dwMsgIDMonitStart: DWORD->unsigned int
        ///m_dwMsgIDClose: DWORD->unsigned int
        [DllImportAttribute("ScreenMP.dll", EntryPoint = "VLSMonMakeAVI3", CallingConvention = CallingConvention.StdCall)]
        public static extern int VLSMonMakeAVI3(
            [InAttribute] 
            [MarshalAsAttribute(UnmanagedType.LPStr)] 
            string sFilename,
            VLSMEVENT_CB pSMonCB,
            [MarshalAsAttribute(UnmanagedType.SysInt)] 
            int lpData,
            uint dwMsgIDConnectionLost,
            uint dwMsgIDUnknowError,
            uint dwMsgIDPlayCompleted,
            uint dwMsgIDWaterMark,
            uint m_dwMsgIDMonitStart,
            uint m_dwMsgIDClose);


        /// Return Type: int
        ///sFilename: char*
        ///pSMonCB: VLSMEVENT_CB
        ///lpData: LPARAM->LONG_PTR->int
        ///dwMsgIDConnectionLost: DWORD->unsigned int
        ///dwMsgIDUnknowError: DWORD->unsigned int
        ///dwMsgIDPlayCompleted: DWORD->unsigned int
        ///dwMsgIDWaterMark: DWORD->unsigned int
        ///m_dwMsgIDMonitStart: DWORD->unsigned int
        ///m_dwMsgIDClose: DWORD->unsigned int
        [DllImportAttribute("ScreenMP.dll", EntryPoint = "VLSMonPlay3", CallingConvention = CallingConvention.StdCall)]
        public static extern int VLSMonPlay3(
            [InAttribute] 
            [MarshalAsAttribute(UnmanagedType.LPStr)] 
            string sFilename,
            VLSMEVENT_CB pSMonCB,
            [MarshalAsAttribute(UnmanagedType.SysInt)] 
            int lpData,
            uint dwMsgIDConnectionLost,
            uint dwMsgIDUnknowError,
            uint dwMsgIDPlayCompleted,
            uint dwMsgIDWaterMark,
            uint m_dwMsgIDMonitStart,
            uint m_dwMsgIDClose);


        /// Return Type: HWND->HWND__*
        [DllImportAttribute("ScreenMP.dll", EntryPoint = "VLSMonGetPlayerHwnd", CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr VLSMonGetPlayerHwnd();


        /// Return Type: int
        ///lpszOutputFilename: LPCTSTR->LPCWSTR->WCHAR*
        ///pSMonCB: VLSMEVENT_CB
        ///lpData: LPARAM->LONG_PTR->int
        [DllImportAttribute("ScreenMP.dll", EntryPoint = "VLSMonMakeAVI2", CallingConvention = CallingConvention.StdCall)]
        public static extern int VLSMonMakeAVI2(
            [InAttribute] 
            [MarshalAsAttribute(UnmanagedType.LPTStr)] 
            string lpszOutputFilename,
            VLSMEVENT_CB pSMonCB,
            [MarshalAsAttribute(UnmanagedType.SysInt)] 
            int lpData);


        /// Return Type: int
        ///sFilename: char*
        ///iType: int
        [DllImportAttribute("ScreenMP.dll", EntryPoint = "VLSMonAddFile", CallingConvention = CallingConvention.StdCall)]
        public static extern int VLSMonAddFile(
            [InAttribute] 
            [MarshalAsAttribute(UnmanagedType.LPStr)] 
            string sFilename,
            int iType);


        /// Return Type: int
        ///lpszOutputFilename: LPCTSTR->LPCWSTR->WCHAR*
        [DllImportAttribute("ScreenMP.dll", EntryPoint = "VLSMonMakeAVI4", CallingConvention = CallingConvention.StdCall)]
        public static extern int VLSMonMakeAVI4(
            [InAttribute] 
            [MarshalAsAttribute(UnmanagedType.LPTStr)] 
            string lpszOutputFilename);

        /// Return Type: int
        /// pSMonCB: VLSMEVENT_CB
        /// lpData: LPARAM->LONG_PTR->int
        [DllImportAttribute("ScreenMP.dll", EntryPoint = "VLSMonPlayVls", CallingConvention = CallingConvention.StdCall)]
        public static extern int VLSMonPlayVls(
            VLSMEVENT_CB pSMonCB,
            int lpData);

        /// Return Type: int
        [DllImportAttribute("ScreenMP.dll", EntryPoint = "VLSMonClearFile", CallingConvention = CallingConvention.StdCall)]
        public static extern int VLSMonClearFile();
    }
}
