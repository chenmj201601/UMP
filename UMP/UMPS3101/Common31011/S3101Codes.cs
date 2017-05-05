//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    25ca79fe-731d-4e83-b33d-2487844e3af1
//        CLR Version:              4.0.30319.18444
//        Name:                     S3101Codes
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Common31011
//        File Name:                S3101Codes
//
//        created by Charley at 2014/10/13 10:34:07
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.UMP.Common31011
{
    /// <summary>
    /// 请求码
    /// </summary>
    public enum S3101Codes
    {
        /// <summary>
        /// 获取可管理的评分表列表
        /// </summary>
        GetScoreSheetList = 0,
        /// <summary>
        /// 获取评分表详细信息
        /// </summary>
        GetScoreSheetInfo = 1,
        /// <summary>
        /// 保存评分表信息
        /// </summary>
        SaveScoreSheetInfo = 2,
        /// <summary>
        /// 删除评分表
        /// </summary>
        RemoveScoreSheetInfo = 3,
        /// <summary>
        /// 获取评分表的管理人列表
        /// </summary>
        GetScoreSheetUserList = 5,
        /// <summary>
        /// 设置评分表管理人
        /// </summary>
        SetScoreSheetUser = 6,
        /// <summary>
        /// 获取评分表影响的坐席
        /// </summary>
        GetCtrolAgent = 7,
        /// <summary>
        /// 获取统计标准信息（自动评分标准基于统计标准）
        /// </summary>
        GetStatisticalInfoList = 8,
        /// <summary>
        /// 获取评分表影响的真实分机
        /// </summary>
        GetCtrolReExtension=9,
    }
}
