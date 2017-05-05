//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    03f3026b-8a0a-4876-9cd8-026b462b932e
//        CLR Version:              4.0.30319.18063
//        Name:                     Kernel32Interop
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.SDKs.Windows
//        File Name:                Kernel32Interop
//
//        created by Charley at 2015/7/22 13:21:40
//        http://www.voicecyber.com 
//
//======================================================================

using System.Runtime.InteropServices;

namespace VoiceCyber.SDKs.Windows
{
    public class Kernel32Interop
    {
        /// <summary>
        /// 获取计算机当前用户的默认语言的ID
        /// </summary>
        /// <returns></returns>
        [DllImport("kernel32.dll")]
        public static extern ushort GetUserDefaultUILanguage();
    }
}
