//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    6209c968-54cd-4da8-8969-af2bb2898c46
//        CLR Version:              4.0.30319.18063
//        Name:                     RecordEncryptInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Common31021
//        File Name:                RecordEncryptInfo
//
//        created by Charley at 2015/8/3 17:17:14
//        http://www.voicecyber.com 
//
//======================================================================
using System;

namespace VoiceCyber.UMP.Common31021
{
    /// <summary>
    /// 录音加密信息
    /// </summary>
    public class RecordEncryptInfo
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public long UserID { get; set; }
        /// <summary>
        /// 服务器地址
        /// </summary>
        public string ServerAddress { get; set; }
        /// <summary>
        /// 有效开始时间
        /// </summary>
        public DateTime StartTime { get; set; }
        /// <summary>
        /// 有效结束时间
        /// </summary>
        public DateTime EndTime { get; set; }
        public bool IsRemember { get; set; }
        /// <summary>
        /// 输入密码
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 实际密码
        /// </summary>
        public string RealPassword { get; set; }
    }
}
