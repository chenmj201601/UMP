using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoiceCyber.VCLDAP
{
    /**/
    /// <summary>
    /// AD中对象的类型
    /// </summary>
    public enum ADObjectType
    {
        /**/
        /// <summary>
        /// 用户
        /// </summary>
        User = 0,
        /**/
        /// <summary>
        /// 计算机
        /// </summary>
        Computer = 1,
        /**/
        /// <summary>
        /// 组
        /// </summary>
        Group = 2,
        /**/
        /// <summary>
        /// 组织单位
        /// </summary>
        OrganizeUnit = 3
    }
}
