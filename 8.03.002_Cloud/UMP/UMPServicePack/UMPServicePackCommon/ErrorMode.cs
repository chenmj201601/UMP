//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    3440345a-0135-4c9d-99c9-55616fbbd400
//        CLR Version:              4.0.30319.18408
//        Name:                     ErrorMode
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPUpdater
//        File Name:                ErrorMode
//
//        created by Charley at 2016/5/9 10:09:58
//        http://www.voicecyber.com 
//
//======================================================================

namespace UMPServicePackCommon
{
    /// <summary>
    /// ErrorReply的类型
    /// </summary>
    public enum ErrorMode
    {
        /// <summary>
        /// 遇到错误不做任何处理
        /// </summary>
        None = 0,
        /// <summary>
        /// 遇到错误记录到日志文件中以供参考
        /// </summary>
        LogFile = 1,
        /// <summary>
        /// 遇到错误弹窗提示
        /// </summary>
        MessageBox = 2,
    }
}
