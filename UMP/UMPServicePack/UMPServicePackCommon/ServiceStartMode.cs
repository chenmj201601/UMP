//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    69fad828-9c8d-4fa5-8aff-8a8ed2184503
//        CLR Version:              4.0.30319.18408
//        Name:                     ServiceStartMode
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPUpdater
//        File Name:                ServiceStartMode
//
//        created by Charley at 2016/5/9 10:59:24
//        http://www.voicecyber.com 
//
//======================================================================

namespace UMPServicePackCommon
{
    /// <summary>
    /// 服务安装后启动模式
    /// </summary>
    public enum ServiceStartMode
    {
        /// <summary>
        /// 保持原状
        /// </summary>
        None = 0,
        /// <summary>
        /// 安装程序退出前统一启动
        /// </summary>
        Default = 1,
        /// <summary>
        /// 安装成功后立马启动
        /// </summary>
        Immediately = 2,
        /// <summary>
        /// 启动后等待一段时间才继续下一步操作
        /// </summary>
        DelayTime=3,
    }
}
