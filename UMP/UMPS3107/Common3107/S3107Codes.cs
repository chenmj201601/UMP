using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common3107
{
    /// <summary>
    /// WCF 相关
    /// </summary>
    public enum S3107Codes
    {
        GetControlAgentInfoList=1,
        GetControlOrgInfoList=2,
        GetQA=3,
        /// <summary>
        /// 插入查询设置参数
        /// </summary>
        QuerySettingDBO = 4,
        /// <summary>
        /// 插入任务配置参数
        /// </summary>
        TaskSettingDBO=5,
        /// <summary>
        /// 获取查询参数
        /// </summary>
        GetQueryDetail = 6,
        /// <summary>
        /// 获取任务参数
        /// </summary>
        GetTaskDetail = 7,
        /// <summary>
        /// 刪除操作
        /// </summary>
        DeleteDBO=8,
        /// <summary>
        /// 獲取時長比率詳情
        /// </summary>
        GetRateDetail=9,
        /// <summary>
        /// 获取ABCD查询配置信息
        /// </summary>
        GetABCD=10,
        /// <summary>
        /// 获取关键词信息
        /// </summary>
        GetKeyWordItems=11,
        /// <summary>
        /// 获取座席可用评分表
        /// </summary>
        GetAgentsTemplate=12,
    }
}
