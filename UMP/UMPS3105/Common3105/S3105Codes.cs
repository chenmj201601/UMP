using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common3105
{
    public enum S3105Codes
    {
        UnKown = 0,
        GetRoleOperationList = 1,
        GetDepartmentInfo = 2,
        GetCheckDatasOrRecheckDatas = 3,
        GetCheckAndReCheckData = 4,
        SubmitCheckData = 5,
        GetAppealProcess = 6,
        GetAppealInfoData=7,
        GetAuInfoList=8,
        UpdateTable19=9,
        /// <summary>
        /// 複核、審批歷史
        /// </summary>
        GetAppealProcessHistory=10,
        /// <summary>
        /// 申訴流程記錄
        /// </summary>
        GetAppealRecordsHistory=11,
        /// <summary>
        /// 獲取評分表
        /// </summary>
        GetUserScoreSheetList = 12,
        /// <summary>
        /// 新建评分表记录
        /// </summary>
        SaveScoreSheetResult=13,
        /// <summary>
        /// 获取重新评分后的分数
        /// </summary>
        GetNewScore=14,

    }
}
