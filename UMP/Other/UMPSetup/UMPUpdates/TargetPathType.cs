//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    0f045c2e-2e26-4daa-9fb2-1a6a01cada59
//        CLR Version:              4.0.30319.18408
//        Name:                     TargetPathType
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Updates
//        File Name:                TargetPathType
//
//        created by Charley at 2016/6/2 15:57:14
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.UMP.Updates
{
    /// <summary>
    /// 目标路径类型
    /// </summary>
    public enum TargetPathType
    {
        /// <summary>
        /// 安装包安装路径
        /// </summary>
        InstallPath = 0,
        /// <summary>
        /// 系统路径
        /// </summary>
        WinSysDir = 1,
        /// <summary>
        /// ProgramData路径
        /// </summary>
        ProgramData = 2,
    }
}
