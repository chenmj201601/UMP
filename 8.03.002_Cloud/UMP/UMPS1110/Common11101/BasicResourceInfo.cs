//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    92c4e573-2a3a-4e55-9e14-3b31b94f841a
//        CLR Version:              4.0.30319.18063
//        Name:                     BasicResourceInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Common11101
//        File Name:                BasicResourceInfo
//
//        created by Charley at 2015/5/14 17:44:54
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.UMP.Common11101
{
    /// <summary>
    /// 资源对象的简略信息
    /// </summary>
    public class BasicResourceInfo
    {
        /// <summary>
        /// 对象的ObjectID
        /// </summary>
        public long ObjectID { get; set; }
        /// <summary>
        /// 唯一序号
        /// </summary>
        public int Key { get; set; }
        /// <summary>
        /// 序号
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 上级对象的ObjectID
        /// </summary>
        public long ParentID { get; set; }
    }
}
