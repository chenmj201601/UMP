//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    0f410968-8e3f-46f2-a10e-d07fec93bd61
//        CLR Version:              4.0.30319.18063
//        Name:                     LocalMonitorObject
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Communications
//        File Name:                LocalMonitorObject
//
//        created by Charley at 2015/4/30 11:21:10
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.UMP.Communications
{
    /// <summary>
    /// 在线监视对象
    /// </summary>
    public class LocalMonitorObject
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public int Type { get; set; }
        /// <summary>
        /// 数据
        /// </summary>
        public string Data { get; set; }
    }
}
