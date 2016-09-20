//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    db32beed-20c1-4d73-b0d6-33e0517e43b3
//        CLR Version:              4.0.30319.18444
//        Name:                     QueryStateInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPS3102.Models
//        File Name:                QueryStateInfo
//
//        created by Charley at 2014/12/5 13:49:01
//        http://www.voicecyber.com 
//
//======================================================================

using System.Collections.Generic;
using VoiceCyber.UMP.Common31021;
namespace UMPS3102.Models
{
    /// <summary>
    /// 查询状态信息
    /// </summary>
    public class QueryStateInfo
    {
        /// <summary>
        /// 查询目标表的名称
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// 条件语句
        /// </summary>
        public string ConditionString { get; set; }
        /// <summary>
        /// 已查得记录的最大行号
        /// </summary>
        public long RowID { get; set; }
        /// <summary>
        /// 统计分析表的名称 T_31_054
        /// </summary>
        public string StatisticalTableName { get; set; }

        public List<QueryConditionDetail> ConditionDetail { get; set; } 
    }
}
