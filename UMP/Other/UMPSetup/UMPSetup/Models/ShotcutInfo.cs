//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    d73bb8ab-01c2-4a75-aef6-42b0cada70d7
//        CLR Version:              4.0.30319.18063
//        Name:                     ShotcutInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPSetup.Models
//        File Name:                ShotcutInfo
//
//        created by Charley at 2015/12/30 15:34:15
//        http://www.voicecyber.com 
//
//======================================================================

namespace UMPSetup.Models
{
    /// <summary>
    /// 快捷方式信息
    /// </summary>
    public class ShotcutInfo
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 文件名
        /// </summary>
        public string LinkName { get; set; }
        /// <summary>
        /// 快捷方式路径
        /// </summary>
        public string LinkPath { get; set; }
        /// <summary>
        /// 目标路径
        /// </summary>
        public string TargetPath { get; set; }
        /// <summary>
        /// 参数
        /// </summary>
        public string Args { get; set; }
        /// <summary>
        /// 说明（提示）
        /// </summary>
        public string Description { get; set; }
    }
}
