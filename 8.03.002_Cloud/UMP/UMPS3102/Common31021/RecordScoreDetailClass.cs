using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoiceCyber.UMP.Common31021
{
    public class RecordScoreDetailClass
    {
        //录音流水号
        public string RecordSerialID { get; set; }
        //坐席工号
        public string AgentID { get; set; }
        //评分表名
        public string ScoreSheet { get; set; }
        //评分人
        public string Inspector { get; set; }
        //分数
        public string Score { get; set; }
        //是否为最终分
        public string IsLastScore { get; set; }
    }
}
