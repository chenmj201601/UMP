//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    39692f0e-0c6f-40fd-8591-834c5cc3d612
//        CLR Version:              4.0.30319.18444
//        Name:                     BasicScoreSheetInfo
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Common31021
//        File Name:                BasicScoreSheetInfo
//
//        created by Charley at 2014/11/16 14:38:54
//        http://www.voicecyber.com 
//
//======================================================================

namespace VoiceCyber.UMP.Common31021
{
    /// <summary>
    /// 评分表信息
    /// </summary>
    public class BasicScoreSheetInfo
    {
        public long ScoreResultID { get; set; }
        public long ScoreSheetID { get; set; }
        public long RecordSerialID { get; set; }

        //这里是用户ID,但是评分表是和坐席关联的,考虑加个坐席
        public long UserID { get; set; }

        public long AgentID { get; set; }

        public long OrgID { get; set; }

        public string Title { get; set; }
        public double TotalScore { get; set; }
        public double Score { get; set; }
        /// <summary>
        /// 修改成绩前的评分成绩ID，新评分的此字段为0
        /// </summary>
        public long OldScoreResultID { get; set; }

        public bool IsFinalScore { get; set; }
        /// <summary>
        /// Flag为1 表示增加按钮;Flag为2 表示修改按钮
        /// </summary>
        public int Flag { get; set; }
        /// <summary>
        /// 评分所花时间[单位 s]
        /// </summary>
        public double WasteTime { get; set; }
    }
}
