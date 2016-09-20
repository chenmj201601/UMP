//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    372e6020-2479-4e48-9d28-186053ff3a67
//        CLR Version:              4.0.30319.18444
//        Name:                     S3102Codes
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Common31021
//        File Name:                S3102Codes
//
//        created by Charley at 2014/11/2 15:21:49
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.UMP.Common31021
{
    /// <summary>
    /// S3102消息码
    /// </summary>
    public enum S3102Codes
    {
        /// <summary>
        /// 获取可用的自定义查询条件项
        /// </summary>
        GetAllCustomConditionItem = 1,
        /// <summary>
        /// 获取用户已保存的查询条件
        /// </summary>
        GetUserQueryCondition = 2,
        /// <summary>
        /// 获取用户的自定义查询条件项
        /// </summary>
        GetUserCustomConditionItem = 3,
        /// <summary>
        /// 获取查询条件的子项
        /// </summary>
        GetConditionSubItem = 4,
        /// <summary>
        /// 获取查询条件的详情
        /// </summary>
        GetQueryConditionDetail = 5,
        /// <summary>
        /// 创建查询条件的SQL语句
        /// </summary>
        CreateQueryConditionString = 6,
        /// <summary>
        /// 获取记录列表
        /// </summary>
        GetRecordData = 7,
        /// <summary>
        /// 保存查询条件
        /// </summary>
        SaveConditions = 8,
        /// <summary>
        /// 保存查询条件子项
        /// </summary>
        SaveConditionSubItems = 9,
        /// <summary>
        /// 获取用户管理的机构列表
        /// </summary>
        GetControlOrgInfoList = 10,
        /// <summary>
        /// 获取用户管理的工号列表
        /// </summary>
        GetControlAgentInfoList = 11,
        /// <summary>
        /// 获取记录备注列表
        /// </summary>
        GetRecordMemoList = 12,
        /// <summary>
        /// 保存备注信息
        /// </summary>
        SaveRecordMemoInfo = 13,
        /// <summary>
        /// 获取用户拥有的评分表列表
        /// </summary>
        GetUserScoreSheetList = 14,
        /// <summary>
        /// 获取评分表详细信息
        /// </summary>
        GetScoreSheetInfo = 15,
        /// <summary>
        /// 获取评分成绩信息（子项成绩）
        /// </summary>
        GetScoreResultList = 16,
        /// <summary>
        /// 保存评分表的打分信息
        /// </summary>
        SaveScoreSheetResult = 17,
        /// <summary>
        /// 保存子项的打分信息
        /// </summary>
        SaveScoreItemResult = 18,
        /// <summary>
        /// 获取用户自定义的设置信息
        /// </summary>
        GetUserSettingList = 19,
        /// <summary>
        /// 保存设置信息
        /// </summary>
        SaveUserSettingInfos = 20,
        /// <summary>
        /// 保存列配置信息
        /// </summary>
        SaveViewColumnInfos = 21,
        /// <summary>
        /// 保存用户自定义的查询条件项
        /// </summary>
        SaveUserConditionItemInfos = 22,
        /// <summary>
        /// 获取用户管理的分机列表
        /// </summary>
        GetControlExtensionInfoList = 23,
        /// <summary>
        /// 保存播放历史信息
        /// </summary>
        SavePlayInfo = 24,
        /// <summary>
        /// 获取播放历史信息
        /// </summary>
        GetPlayInfoList = 25,
        /// <summary>
        /// 查询条件子项保存到临时表中
        /// </summary>
        InsertConditionSubItems = 26,
        /// <summary>
        /// 管理对象序列保存到临时表中
        /// </summary>
        InsertManageObjectQueryInfos = 27,
        /// <summary>
        /// 获取bookmark列表
        /// </summary>
        GetRecordBookmarkList = 28,
        /// <summary>
        /// 保存Bookmark信息
        /// </summary>
        SaveRecordBookmarkInfo = 29,
        /// <summary>
        /// 获取Bookmark等级列表
        /// </summary>
        GetBookmarkRankList = 30,
        /// <summary>
        /// 保存书签等级信息
        /// </summary>
        SaveBookmarkRankInfo = 31,
        /// <summary>
        /// 保存评分数据信息，写入（31_041）
        /// </summary>
        SaveScoreDataResult = 32,
        /// <summary>
        /// 获取Sftp服务器信息
        /// </summary>
        GetSftpServerList = 33,
        /// <summary>
        /// 获取下载参数列表
        /// </summary>
        GetDownloadParamList = 34,


        //我加的+++++++++++++++++++++++++++++++++++++++++++++++++
        ///<summary>
        ///保存录音备注信息到T_21_001
        ///</summary>
        SaveMemoInfoToT_21_001 = 35,
        ///<summary>
        ///保存标注标题到T_21_001
        /// </summary> 
        SaveBookMarkTitleToT_21_001 = 36,
        ///<summary>
        ///获取质检员(评分人)信息
        ///</summary>
        GetInspectorList = 37,
        ///<summary>
        ///获取所有的评分表信息
        ///</summary>
        GetAllScoreSheetList = 38,
        /// <summary>
        /// 获取一条录音的评分详情
        /// </summary>
        GetRecordScoreDetail = 39,
        /// <summary>
        /// 该条录音是否被申诉过
        /// </summary>
        IsComplainedRecord = 40,
        /// <summary>
        /// 获取录音所关联的录屏文件信息列表
        /// </summary>
        GetRelativeRecordList = 41,
        /// <summary>
        /// 该条录音是否被分配到任务中
        /// </summary>
        IsTaskedRecord = 42,
        /// <summary>
        /// 获取可管理的技能组的信息
        /// </summary>
        GetSkillGroupInfo = 50,
        /// <summary>
        /// 获得ABCD里面已经绑定了参数大项的机构或者技能组
        /// </summary>
        GetOrgSkillGroupInf = 51,
        /// <summary>
        /// 获得组织机构列表
        /// </summary>
        GetOrgList = 52,
        /// <summary>
        /// 获得机构下直属的坐席的List
        /// </summary>
        GetAgentList = 53,
        /// <summary>
        /// 获得机构下直属的分机的List
        /// </summary>
        GetExtensionList = 54,
        /// <summary>
        /// 执行sql语句
        /// </summary>
        ExecuteStrSql = 55,
        /// <summary>
        /// 获得教材库的文件夹树
        /// </summary>
        GetLibraryFolder = 56,
        /// <summary>
        /// 获得教材库的文件夹里的内容
        /// </summary>
        GetLibraryFolderContent = 57,
        /// <summary>
        /// 获得会话信息(从T_51_002)
        /// </summary>
        GetConversationInfo = 58,
        /// <summary>
        /// 将录音教材存入教材库，也就是存入T_31_060
        /// </summary>
        InsertLearningToLibrary =59,
        //+++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>
        /// 获取评分备注结果列表
        /// </summary>
        GetScoreCommentResultList = 60,
        /// <summary>
        /// 保存评分备注结果信息
        /// </summary>
        SaveScoreCommentResultInfos = 61,
        /// <summary>
        /// 获得自动评分项的成绩
        /// </summary>
        GetAutoScore = 62,
        /// <summary>
        /// 获得自己管理的真实分机
        /// </summary>
        GetControlRealExtensionInfoList =63,
        /// <summary>
        /// 上传文件
        /// </summary>
        UpLoadFiles = 64,
        /// <summary>
        /// 获得关键词
        /// </summary>
        GetKeyWordsInfo = 65,
        /// <summary>
        /// 删除文件
        /// </summary>
        UMPDeleteOperation = 66,
        /// <summary>
        /// 获得BookMarkRecord文件夹在服务器上的绝对路径
        /// </summary>
        GetBookMarkRecordPath =67,
        /// <summary>
        /// 保存查询结果
        /// </summary>
        SaveQueryResult=68,
        /// <summary>
        /// 下载录音文件
        /// </summary>
        DownloadRecordFile = 101,
        /// <summary>
        /// 删除该录音
        /// </summary>
        DeleteUsersRecordBookMark = 102,

        /// <summary>
        /// 解密录音文件
        /// </summary>
        DecryptRecordFile = 110,
    }
}
