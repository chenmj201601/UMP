//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    02872953-14a0-404f-b91e-c2d8f0a8c244
//        CLR Version:              4.0.30319.18408
//        Name:                     ServiceInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Updates
//        File Name:                ServiceInfo
//
//        created by Charley at 2016/6/10 16:26:17
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.UMP.Updates
{
    /// <summary>
    /// 服务信息
    /// </summary>
    public class ServiceInfo
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 服务名
        /// </summary>
        public string ServiceName { get; set; }
        /// <summary>
        /// 显示名
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// 所属安装包
        /// </summary>
        public string Package { get; set; }
        /// <summary>
        /// 关联的进程名
        /// </summary>
        public string ProcessName { get; set; }
        /// <summary>
        /// 进程ID
        /// </summary>
        public int ProcessID { get; set; }
        /// <summary>
        /// 服务当前状态
        /// </summary>
        public int Status { get; set; }

    }
}
