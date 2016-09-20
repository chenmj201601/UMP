
using System;

namespace Common3105
{
    public class AppealInfo
    {
        public int RowNumber { get; set; }
        public long RecordReference { get; set; }
        public int Appealint { get; set; }
        public string AppealState { get; set; }
        public long ReferenceID { get; set; }
        public DateTime AppealTime { get; set; }
        public long AgentID { get; set; }
        public string AgentName { get; set; }
        public long TemplateID { get; set; }
        public string TemplateName { get; set; }
        public double Score { get; set; }
        public long ScoreID { get; set; }
    }
}
