using Common3601;

namespace UMPS3601.Models
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
