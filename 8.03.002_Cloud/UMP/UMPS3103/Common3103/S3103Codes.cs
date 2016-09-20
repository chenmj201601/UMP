using System;

namespace VoiceCyber.UMP.Common31031
{
    public enum S3103Codes
    {
        GetUserOperationList = 0,//新的获取权限方式，需要读狗
        GetRoleOperationList = 1,//老的获取权限的方式
        GetCurrentUserTasks = 2,
        GetTaskRecordByTaskID = 3,
        ModifyTaskDealLine = 4,
        GetControlOrgInfoList = 5,
        GetControlAgentInfoList = 6,
        GetUserScoreSheetList = 7,
        SaveScoreSheetResult = 8,
        SaveScoreItemResult = 9,
        GetScoreResultList = 10,
        GetRecordMemoList = 11,
        SaveRecordMemoInfo = 12,
        GetRecordData = 13,
        GetQA = 14,
        SaveTask = 15,
        SaveTaskQA = 16,
        SaveTaskRecord = 17,
        UpdateTaskID2Record = 18,
        UpdateTask = 19,
        DeleteRecordFromTask = 20,
        GetCanOperationTasks = 21,
        RemoveRecord2Task = 22,
        InsertTempData=23,
        SaveScoreDataResult = 24,
        //複檢
        GetRecheckRecordData=25,
        //评分写入数据库失败，删除评分数据
        DeleteErrorDB = 26,
        //获取所有座席、用户信息
        GetAuInfoList=27,
        /// <summary>
        /// 获取ABCD查询配置信息
        /// </summary>
        GetABCD=28,
        /// <summary>
        /// 获取评分备注结果列表
        /// </summary>
        GetScoreCommentResultList = 29,
        /// <summary>
        /// 保存评分备注结果信息
        /// </summary>
        SaveScoreCommentResultInfos = 30,
        /// <summary>
        /// 获取评分表id（获取初检使用的评分表）
        /// </summary>
        GetScoreTemplateID=31,
        /// <summary>
        /// 获取该录音的历史操作（是否申诉，是否被分配到复检任务）
        /// </summary>
        GetRecordHistoryOpt=32,
        /// <summary>
        /// 获取当前用户管理的用户
        /// </summary>
        GetCtrolQA=33,
    }
}
