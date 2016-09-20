//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    adbbd593-2027-48f5-a02d-b2d5244d6406
//        CLR Version:              4.0.30319.42000
//        Name:                     BasicAppInfo
//        Computer:                 DESKTOP-VUMCK8M
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Common12001
//        File Name:                BasicAppInfo
//
//        created by Charley at 2016/2/18 15:59:50
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.UMP.Common12001
{
    /// <summary>
    /// 应用模块基本信息
    /// </summary>
    public class BasicAppInfo
    {
        /// <summary>
        /// 模块号（4位小模块号）
        /// </summary>
        public int ModuleID { get; set; }
        /// <summary>
        /// 大漠块号
        /// </summary>
        public int MasterID { get; set; }
        /// <summary>
        /// 所在App的ID（通常与ModuleID一样）
        /// </summary>
        public int AppID { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 分组
        /// </summary>
        public string Category { get; set; }
        /// <summary>
        /// 序号
        /// </summary>
        public int SortID { get; set; }
        /// <summary>
        /// 启动参数
        /// </summary>
        public string Args { get; set; }
        /// <summary>
        /// 打开方式
        /// </summary>
        public string OpenMethod { get; set; }
        /// <summary>
        /// 图标
        /// </summary>
        public string Icon { get; set; }
    }
}
