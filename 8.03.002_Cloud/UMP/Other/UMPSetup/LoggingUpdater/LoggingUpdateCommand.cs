//======================================================================
//
//        Copyright © 2016-2017 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        GUID1:                    84559a64-7285-4f54-aa55-70e58f7f6950
//        CLR Version:              4.0.30319.42000
//        Name:                     LoggingUpdateCommand
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             VoiceCyber
//        Namespace:                LoggingUpdater
//        File Name:                LoggingUpdateCommand
//
//        Created by Charley at 2016/9/8 19:32:21
//        http://www.voicecyber.com 
//
//======================================================================

namespace LoggingUpdater
{
    /// <summary>
    /// 通讯指令
    /// </summary>
    public enum LoggingUpdateCommand
    {
        /// <summary>
        /// 未知
        /// </summary>
        Unkown = 0,
        /// <summary>
        /// 
        /// LoggingUpdater请求下载UMPData.zip
        /// 
        /// 参数：
        /// 0：  Token（验证码）
        /// 
        /// 响应：
        /// 0：  TotalSize
        /// 1：  FileName
        /// </summary>
        DownloadUMPData = 1001,
        /// <summary>
        /// 
        /// LoggingUpdater向UMPUpdater报告更新进度
        /// 
        /// 参数：
        /// 0：  Token（验证码）
        /// 1：  Progress（当前进度值，百分比）
        /// 2：  Flag（1表示完成，2表示出错）
        /// 
        /// 响应：
        /// 无
        /// </summary>
        UpdateProgress = 1002,

        /// <summary>
        /// 
        /// UMPUpdater向LoggingUpdater传送UMPData.zip
        /// 
        /// 注意，这里传输的数据是二进制的字节流数据
        /// MessageHead中指定了数据大小，Command 及 State
        /// 
        /// </summary>
        TransferUMPData = 2001,

    }
}
