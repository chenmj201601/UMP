//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    ab920af1-5567-4278-b1aa-5c30b97a9852
//        CLR Version:              4.0.30319.18408
//        Name:                     ShortcutInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Updates
//        File Name:                ShortcutInfo
//
//        created by Charley at 2016/6/10 16:47:21
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.UMP.Updates
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
