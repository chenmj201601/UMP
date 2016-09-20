//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    8100ddb3-9e48-4989-a968-7700ed15f218
//        CLR Version:              4.0.30319.18444
//        Name:                     NotifyObjectInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Common11101
//        File Name:                NotifyObjectInfo
//
//        created by Charley at 2015/3/7 17:37:50
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.UMP.Common11101
{
    /// <summary>
    /// 通知对象信息
    /// </summary>
    public class NotifyObjectInfo
    {
        /// <summary>
        /// 对象ObjID
        /// </summary>
        public long ObjID { get; set; }
        /// <summary>
        /// 对象类型
        /// </summary>
        public int ObjType { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 端口
        /// </summary>
        public string Port { get; set; }
    }
}
