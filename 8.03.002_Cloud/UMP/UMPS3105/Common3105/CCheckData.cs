// ***********************************************************************
// Assembly         : Common3105
// Author           : Luoyihua
// Created          : 12-12-2014
//
// Last Modified By : Luoyihua
// Last Modified On : 12-15-2014
// ***********************************************************************
// <copyright file="CCheckData.cs" company="VoiceCodes">
//     Copyright (c) VoiceCodes. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common3105
{
    public class CCheckData
    {
        public int RowNumber { set; get; }
        /// <summary>
        /// 申诉详情主键
        /// </summary>
        public string AppealDetailID { set; get; }
        public string AppealState { set; get; }
        /// <summary>
        /// 对于申诉(1座席申诉，2他人替申)，对于复核(3复核驳回,4复核过通)，对于审批（5审批通过不修改分数，6审批通过重新评分，7审批驳回）
        /// </summary>
        public int AppealInt { set; get; }
        /// <summary>
        /// T_31_008_ScoreResult.ScoreResultID
        /// </summary>
        public string ScoreResultID { set; get; }
        public string RecoredReference { set; get; }
        public string AgentID { set; get; }
        public string AgentName { set; get; }
        /// <summary>
        /// 申诉流程ID
        /// </summary>
        public string AppealFlowID { set; get; }
        public string AppealDatetime { set; get; }
        public string Score { set; get; }
        /// <summary>
        /// 处理时间
        /// </summary>
        public string OperationTime { get; set; }
        /// <summary>
        /// 評分表ID
        /// </summary>
        public string TemplateID { get; set; }
        /// <summary>
        /// 評分來源
        /// </summary>
        public string ScoreType { get; set; }
        /// <summary>
        /// 重新评分分数
        /// </summary>
        public string NewScore { get; set; }

        /// <summary>
        /// 打分人ID
        /// </summary>
        public long ScoreUserID { get; set; }
    }
}
