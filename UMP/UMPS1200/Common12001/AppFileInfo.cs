//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    d51f4032-0fb6-412e-82a5-b000cd9e40b7
//        CLR Version:              4.0.30319.18408
//        Name:                     AppFileInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Common12001
//        File Name:                AppFileInfo
//
//        created by Charley at 2016/7/15 09:38:26
//        http://www.voicecyber.com 
//
//======================================================================
using System;


namespace VoiceCyber.UMP.Common12001
{
    /// <summary>
    /// 文件信息
    /// </summary>
    public class AppFileInfo
    {
        /// <summary>
        /// 文件名（包扩展名）
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 完整路径
        /// </summary>
        public string FullName { get; set; }
        /// <summary>
        /// 扩展名（.dll .exe等）
        /// </summary>
        public string Extension { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 修改日期
        /// </summary>
        public DateTime ModifyTime { get; set; }
    }
}
