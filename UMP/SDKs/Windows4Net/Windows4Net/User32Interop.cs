//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    3c692cce-28c4-4f0c-bdef-4aa1bed66875
//        CLR Version:              4.0.30319.18063
//        Name:                     User32Interop
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.SDKs.Windows
//        File Name:                User32Interop
//
//        created by Charley at 2015/7/20 9:18:16
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Runtime.InteropServices;

namespace VoiceCyber.SDKs.Windows
{
    /// <summary>
    /// User32的平台调用，参考WindowsApi
    /// </summary>
    public class User32Interop
    {
        /// <summary>
        /// 向指定的窗口发消息
        /// </summary>
        /// <param name="hwnd">窗口句柄</param>
        /// <param name="wMsg">消息指令</param>
        /// <param name="wParam">消息数据</param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        [DllImport("user32.dll", EntryPoint = "PostMessage")]
        public static extern int PostMessage(
            IntPtr hwnd,
            int wMsg,
            int wParam,
            int lParam);
        /// <summary>
        /// 设定指定窗口的位置
        /// </summary>
        /// <param name="hwnd">窗口句柄</param>
        /// <param name="order">Z轴顺序（-1表示将窗口置顶）</param>
        /// <param name="x">水平位置</param>
        /// <param name="y">垂直位置</param>
        /// <param name="dx">宽度</param>
        /// <param name="dy">高度</param>
        /// <param name="flag">0：不可伸缩；1：水平伸缩；2：垂直伸缩；3：水平垂直均可伸缩</param>
        /// <returns></returns>
        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        public static extern int SetWindowPos(
            IntPtr hwnd,
            int order,
            int x,
            int y,
            int dx,
            int dy,
            int flag);
        /// <summary>
        /// 将指定的窗口放置在最前面
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool BringWindowToTop(IntPtr hWnd);
        /// <summary>
        /// 设置窗口标题
        /// </summary>
        /// <param name="hwnd">窗口句柄</param>
        /// <param name="lpString">标题文本</param>
        /// <returns></returns>
        [DllImport("user32.dll", EntryPoint = "SetWindowText")]
        public static extern bool SetWindowText(
            IntPtr hwnd,
            [InAttribute]
            [MarshalAsAttribute(UnmanagedType.LPStr)] 
            string lpString);
    }
}
