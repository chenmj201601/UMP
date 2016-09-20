using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common4602
{
    public enum S4602Codes
    {
        /// <summary>
        /// 获取机构
        /// </summary>
        GetOrgList = 1,
        /// <summary>
        /// 获取所属机构下的座席、真实、虚拟分机. A,代表座席,E,虚拟分机,R,代表真实分机
        /// </summary>
        GetCAgentREx = 2,
        /// <summary>
        /// 获取所属机构下的用户
        /// </summary>
        GetCUser = 3,
        /// <summary>
        /// 获取PM设定
        /// </summary>
        GetPMSetting = 4,
        /// <summary>
        /// 查询PM统计数据
        /// </summary>
        QueryPMDatas = 5,
        /// <summary>
        /// 获取技能组
        /// </summary>
        GetSkillGroup = 6,
        /// <summary>
        /// 获取全局配置（月、周的开始设定, 座席分机虚拟分机全局参数的编号）
        /// </summary>
        GetGlobalSetting=7,
        /// <summary>
        /// 保存列的信息s
        /// </summary>
        SaveViewColumnInfos=8,
    }
}
