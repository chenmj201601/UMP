//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    813ccd23-cde9-4f85-a40d-824cb4e8570d
//        CLR Version:              4.0.30319.18063
//        Name:                     MonitorData
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS2102.Models
//        File Name:                MonitorData
//
//        created by Charley at 2015/7/29 15:29:27
//        http://www.voicecyber.com 
//
//======================================================================

namespace UMPS2102.Models
{
    /// <summary>
    /// 监视数据对象
    /// </summary>
    public class MonitorData
    {
        /// <summary>
        /// 对象编码
        /// </summary>
        public long ObjID { get; set; }
        /// <summary>
        /// 对象类型
        /// </summary>
        public int ObjType { get; set; }
        /// <summary>
        /// 对象名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 对象角色
        /// 0：无
        /// 1：录音
        /// 2：录屏
        /// 3：录音录屏
        /// </summary>
        public int Role { get; set; }
        /// <summary>
        /// 其他信息
        /// </summary>
        public string Other01 { get; set; }
        /// <summary>
        /// 其他信息
        /// </summary>
        public string Other02 { get; set; }
        /// <summary>
        /// 其他信息（所在服务器地址）
        /// </summary>
        public string Other03 { get; set; }
        /// <summary>
        /// 其他信息
        /// </summary>
        public string Other04 { get; set; }
        /// <summary>
        /// 其他信息
        /// </summary>
        public string Other05 { get; set; }
    }
}
