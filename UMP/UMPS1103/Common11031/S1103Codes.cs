using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common11031
{
    public enum S1103Codes
    {
        Unkown = 0,
        /// <summary>
        /// 坐席添加，删除，修改
        /// </summary>
        ModifyAgent = 1,
        /// <summary>
        /// 获取所有管理指定坐席的用户编码
        /// </summary>
        GetAgentCtledUserIDs = 2,
        /// <summary>
        /// 保存坐席的管理权限
        /// </summary>
        SaveAgentMMT = 3,
        /// <summary>
        /// 修改坐席密码
        /// </summary>
        UPAgentPwd = 4,
        /// <summary>
        /// 修改坐席所属机构
        /// </summary>
        ModifyAgentORGC = 5,
        /// <summary>
        /// 获取所以坐席
        /// </summary>
        GetAllAgent = 6,

    }
}