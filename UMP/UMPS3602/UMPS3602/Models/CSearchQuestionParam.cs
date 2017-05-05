using Common3602;

namespace UMPS3602.Models
{
    public class CSearchQuestionParam : CQuestionsParam
    {
        public int IntUseMax { get; set; }

        public int IntUseMin { get; set; }

        public string StrQuestionType { get; set; }

        public string StrStartTime { get; set; }

        public string StrEndTime { get; set; }
    }
}
