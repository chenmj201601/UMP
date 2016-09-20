//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    6150ab60-a31b-4d86-86fe-fbb1a7051465
//        CLR Version:              4.0.30319.18444
//        Name:                     RoleInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Common
//        File Name:                RoleInfo
//
//        created by Charley at 2014/8/27 15:29:18
//        http://www.voicecyber.com 
//
//======================================================================

using System.Collections.Generic;
using System.Runtime.Serialization;

namespace VoiceCyber.UMP.Common
{
    /// <summary>
    /// 角色信息
    /// </summary>
    [DataContract]
    public class RoleInfo
    {
        /// <summary>
        /// 角色编号，如果为 0 表示没有指定角色
        /// </summary>
        [DataMember]
        public long ID { get; set; }
        /// <summary>
        /// 角色名称
        /// </summary>
        [DataMember]
        public string Name { get; set; }
        /// <summary>
        /// 当前用户所属于的角色列表
        /// </summary>
        [DataMember]
        public List<long> ListIntRoleID { get; set; }
        /// <summary>
        /// 当前用户所属于的角色名称
        /// </summary>
        [DataMember]
        public List<string> ListStrRoleName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public RoleInfo()
        {
            ListIntRoleID = new List<long>();
            ListStrRoleName = new List<string>();
        }

        public override string ToString()
        {
            return string.Format("{0}[{1}]", Name, ID);
        }
    }
}
