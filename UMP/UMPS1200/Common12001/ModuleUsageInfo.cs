//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    72a72034-7927-4312-83a3-01275d9eabf5
//        CLR Version:              4.0.30319.42000
//        Name:                     ModuleUsageInfo
//        Computer:                 DESKTOP-VUMCK8M
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Common12001
//        File Name:                ModuleUsageInfo
//
//        created by Charley at 2016/3/3 13:33:00
//        http://www.voicecyber.com 
//
//======================================================================
using System;


namespace VoiceCyber.UMP.Common12001
{
    /// <summary>
    /// 模块使用信息
    /// </summary>
    public class ModuleUsageInfo
    {
        /// <summary>
        /// 应用程序编号
        /// </summary>
        public int AppID { get; set; }
        /// <summary>
        /// 模块号
        /// </summary>
        public int ModuleID { get; set; }
        /// <summary>
        /// 使用者
        /// </summary>
        public long UserID { get; set; }
        /// <summary>
        /// 进入模块的时间
        /// </summary>
        public DateTime BeginTime { get; set; }
        /// <summary>
        /// 离开模块的时间
        /// </summary>
        public DateTime EndTime { get; set; }
        /// <summary>
        /// 会话ID
        /// </summary>
        public string SessionID { get; set; }
        /// <summary>
        /// 启动参数
        /// </summary>
        public string StartArgs { get; set; }
        /// <summary>
        /// 主机名
        /// </summary>
        public string HostName { get; set; }
        /// <summary>
        /// 主机地址
        /// </summary>
        public string HostAddress { get; set; }

        public override string ToString()
        {
            return string.Format("{0}({1})({2})({3})", AppID, ModuleID, UserID, BeginTime.ToString("yyyy-MM-dd HH:mm:ss"));
        }
    }
}
