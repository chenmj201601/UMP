//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    6e3d9bb1-56bf-4b4c-a42e-271a5f85e56c
//        CLR Version:              4.0.30319.18444
//        Name:                     ServerCommand
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                Common11101
//        File Name:                ServerCommand
//
//        created by Charley at 2015/2/3 10:25:38
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.UMP.Common11101
{
    /// <summary>
    /// 请求命令
    /// </summary>
    public enum ServerRequestCommand
    {
        /// <summary>
        /// 未知
        /// </summary>
        Unkown = 0,
        /// <summary>
        /// 获取磁盘驱动器的列表
        /// </summary>
        GetDiskDriverList = 1,
        /// <summary>
        /// 获取子目录列表
        /// </summary>
        GetChildDirectoryList = 2,
        /// <summary>
        /// 获取子文件列表
        /// </summary>
        GetChildFileList = 3,
        /// <summary>
        /// 获取网卡信息
        /// </summary>
        GetNetworkCardList = 4,
        /// <summary>
        /// 获取CTIConnection的ServiceName值
        /// </summary>
        GetCTIServiceName = 5,
        /// <summary>
        /// 获取机器名
        /// </summary>
        GetServerName = 10,

        /// <summary>
        /// 通知UMPService00，资源参数变更
        /// </summary>
        SetResourceChanged = 101
    }
}
