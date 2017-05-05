using Common3602;

namespace UMPS3602.Models
{
    public class CSearchPaperParam : CPaperParam
    {
        public int IntScoresMax { get; set; }
        public int IntScoresMin { get; set; }
        public int IntPassMarkMax { get; set; }
        public int IntPassMarkMin { get; set; }
        public int IntTimeMax { get; set; }
        public int IntTimeMin { get; set; }
        public string StrStartTime { get; set; }
        public string StrEndTime { get; set; }
    }
}
