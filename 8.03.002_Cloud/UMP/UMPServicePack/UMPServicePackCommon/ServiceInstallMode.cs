//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    95ae1171-53d3-408b-ac2e-e4ee8563329d
//        CLR Version:              4.0.30319.18408
//        Name:                     ServiceInstallMode
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPUpdater
//        File Name:                ServiceInstallMode
//
//        created by Charley at 2016/5/9 10:54:59
//        http://www.voicecyber.com 
//
//======================================================================

namespace UMPServicePackCommon
{
    /// <summary>
    /// 服务安装模式
    /// </summary>
    public enum ServiceInstallMode
    {
        /// <summary>
        /// 不安装(仅重启服务）
        /// </summary>
        None = 0,
        /// <summary>
        /// 安装，如果存在，先卸载
        /// </summary>
        Install = 1,
        /// <summary>
        /// 安装，如果存在，跳过
        /// </summary>
        InstallSkip = 2,
        /// <summary>
        /// 安装，如果存在，报告错误
        /// </summary>
        InstallError = 3,
        /// <summary>
        /// 卸载，如果不存在，跳过
        /// </summary>
        Uninstall = 11,
        /// <summary>
        /// 卸载，如果不存在，报告错误
        /// </summary>
        UninstallError = 12,
    }
}
