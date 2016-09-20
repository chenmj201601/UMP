using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common3107
{
    /// <summary>
    /// 常量、特定编码
    /// </summary>
    public static class S3107Consts
    {
        //权限
        public const long OPT_AutoTask = 3107001;

        //任务设置相关
        public const long WDE_TaskSetting = 31070010;
        public const long WDE_TaskNew = 31070011;
        public const long WDE_TaskUpdate = 31070012;
        public const long WDE_TaskDelete = 31070013;

        //查询项设置相关
        public const long WDE_QuerySetting = 31070020;
        public const long WDE_QueryNew = 31070021;
        public const long WDE_QueryUpdate = 31070022;
        public const long WDE_QueryDelete = 31070023;


        //被使用的查询项无法删除
        public const string HadUse = "Had Use";

        //ABCD ID
        /// <summary>
        /// 服务态度
        /// </summary>
        public const string WDE_ServiceAttitude = "3110000000000000001";
        /// <summary>
        /// 专业水平
        /// </summary>
        public const string WDE_ProfessionalLevel = "3110000000000000002";
        /// <summary>
        /// 重复呼入
        /// </summary>
        public const string WDE_RepeatCallIn = "3110000000000000003";
        /// <summary>
        /// 录音时长异常
        /// </summary>
        public const string WDE_RecordDurationError = "3110000000000000006";
    }
}
