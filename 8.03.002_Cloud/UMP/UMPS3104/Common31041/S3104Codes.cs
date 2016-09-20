
namespace VoiceCyber.UMP.Common31041
{
    public enum S3104Codes
    {
        /// <summary>
        /// 获取播放历史
        /// </summary>
        GetPlayHistory=1,
        /// <summary>
        /// 获取单条申诉进度详情
        /// </summary>
        GetOwerAppeal=2,
        /// <summary>
        /// 写播放历史
        /// </summary>
        WritePlayHistory=3,
        /// <summary>
        /// 写申诉
        /// </summary>
        WriteAppeal=4,
        /// <summary>
        /// 1获取机构信息
        /// </summary>
        GetControlOrgInfoList = 5,
        /// <summary>
        /// 2
        /// </summary>
        GetControlAgentInfoList = 6,
        /// <summary>
        /// 直接取T_11_003_00000 表里3104的所有权限
        /// </summary>
        GetRoleOperationList = 7,
        /// <summary>
        /// 登录获取用户\座席信息
        /// </summary>
        AgentLoginValidate = 8,
        /// <summary>
        /// 插入临时表
        /// </summary>
        InsertTempData=9,
        /// <summary>
        /// 获取查询数据
        /// </summary>
        GetRecordData=10,
        /// <summary>
        /// 获取T_11_005、T_11_101全部座席、用戶信息
        /// </summary>
        GetAuInfoList=11,
        /// <summary>
        /// 获取详情表ID主键 T_31_019,T_31_047 C001
        /// </summary>
        GetSerialID = 12,
        /// <summary>
        /// 获取机构信息
        /// </summary>
        GetControledOrganizationList = 13,
        /// <summary>
        /// 获取受管理的座席信息
        /// </summary>
        GetControledUserList = 14,
        /// <summary>
        /// 获取评分表详细信息
        /// </summary>
        GetScoreSheetInfo = 15,
        /// <summary>
        /// 获取评分成绩信息（子项成绩）
        /// </summary>
        GetScoreResultList = 16,
        /// <summary>
        /// 下載錄音文件
        /// </summary>
        GetRecordFile=17,
        /// <summary>
        /// 获取Sftp服务器信息
        /// </summary>
        GetSftpServerList=18,
        /// <summary>
        /// 獲取評分成績
        /// </summary>
        GetUserScoreSheetList = 19,
        /// <summary>
        /// 獲取評分成績清單
        /// </summary>
        GetScoreDate=20,
        /// <summary>
        /// 获取录音所关联的录屏文件信息列表
        /// </summary>
        GetRelativeRecordList = 21,
        /// <summary>
        /// 获取下载参数列表
        /// </summary>
        GetDownloadParamList = 22,
        /// <summary>
        /// 获取文件夹列表
        /// </summary>
        GetFolder = 23,
        /// <summary>
        /// 获取文件信息
        /// </summary>
        GetFiles = 24,
        /// <summary>
        /// 获取评分表备注
        /// </summary>
        GetScoreCommentResultList=25,
        /// <summary>
        /// 获取ABCD配置
        /// </summary>
        GetABCD=26,
        /// <summary>
        /// 浏览教材记录
        /// </summary>
        WriteBrowseHistory=27,
        /// <summary>
        /// 获取录音标记
        /// </summary>
        GetRecordBookMark=28,
        /// <summary>
        /// 获取考试信息
        /// </summary>
        GetExamInfo=29,
        /// <summary>
        /// 获取IM是否可用
        /// </summary>
        GetIMRole=30,
    }
}
