using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UMPService09.Model
{
    public enum EnumQMFunction
    {
        /// <summary>
        /// 坐席\分机被质检的数量
        /// </summary>
        ScoreNumber = 1,
        /// <summary>
        /// 坐席\分机申述数量
        /// </summary>
        AppealNumber = 2,
        /// <summary>
        /// 坐席\分机重新打分的数量（申诉成功并重新打分）
        /// </summary>
        AppealSucNum = 3,
        /// <summary>
        /// 坐席\分机坐席被质检的总分数
        /// </summary>
        TotalScore = 4,
        /// <summary>
        /// 质检员被申述的数量
        /// </summary>
        QaBeAppealedNum = 5,
        /// <summary>
        /// 质检员质检的数量
        /// </summary>
        QaScoreNum = 6,
        /// <summary>
        /// 质检员完成的任务数量
        /// </summary>
        QaTaskFinishedNum = 7,
        /// <summary>
        /// 质检员接到的任务数量
        /// </summary>
        QaTaskReceivedNum = 8
    }
}
