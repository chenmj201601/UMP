//======================================================================
//
//        Copyright © 2016-2017 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        GUID1:                    b06a232a-1d08-4969-8223-26521a044620
//        CLR Version:              4.0.30319.42000
//        Name:                     Service91Command
//        Computer:                 DESKTOP-5OJRDKD
//        Organization:             VoiceCyber
//        Namespace:                UMPService91
//        File Name:                Service91Command
//
//        Created by Charley at 2016/8/18 11:16:16
//        http://www.voicecyber.com 
//
//======================================================================

namespace UMPService91
{
    public enum Service91Command
    {

        #region 请求命令

        /// <summary>
        /// 未知
        /// </summary>
        Unkown = 0,


        #region 录音文件下载

        /// <summary>
        /// 下载录音文件
        /// </summary>
        DownloadRecordFile = 1000,
        /// <summary>
        /// 下载录音文件FTP
        /// </summary>
        DownloadRecordFileNas = 1001,
        /// <summary>
        /// 下载录音文件，无需指定下载服务器信息，系统自动加载配置信息
        /// </summary>
        DownloadRecord = 1002,

        #endregion


        #region 录音格式转换

        /// <summary>
        /// 转换音频格式
        /// </summary>
        ConvertWaveFormat = 1010,

        #endregion


        #region 录音文件加解密

        /// <summary>
        /// 解密录音文件
        /// </summary>
        DecryptRecordFile = 1020,
        /// <summary>
        /// 对文件进行原始解密
        /// </summary>
        OriginalDecryptFile = 1021,
        /// <summary>
        /// 加密录音文件
        /// </summary>
        EncryptRecordFile = 1022,

        #endregion


        #region Isa播放控制

        /// <summary>
        /// 艺赛旗录屏回放控制
        /// </summary>
        IsaControl = 1030,
        /// <summary>
        /// 开始艺赛旗录屏控制
        /// 0：      录屏服务器地址
        /// </summary>
        IsaStart = 1031,
        /// <summary>
        /// 停止艺赛旗录屏控制
        /// </summary>
        IsaStop = 1032,
        /// <summary>
        /// 艺赛旗录屏回放控制
        /// 0：      Action
        /// 1:       RefID
        /// 2：      Position
        /// 3：      Speed
        /// </summary>
        IsaBehavior = 1033,

        #endregion


        #region 文件上传下载

        /// <summary>
        /// 从指定的存储设备下载文件，源文件将下载到MediaData临时目录中
        /// 返回生成的文件名
        /// </summary>
        DownloadFile = 1040,
        /// <summary>
        /// 将文件上传到指定的存储设备中，源文件存在于MediaData临时目录中
        /// 返回存储设备名称
        /// </summary>
        UploadFile = 1041,

        #endregion


        #endregion


        #region 通知命令


        #region Isa播放控制

        /// <summary>
        /// Isa服务器通知消息
        /// </summary>
        IsaNotify = 3000,
        /// <summary>
        /// Isa服务器通知播放进度消息
        /// </summary>
        IsaNotifyPosition = 30001,

        #endregion


        #endregion

    }
}
