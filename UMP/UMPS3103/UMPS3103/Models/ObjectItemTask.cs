using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VoiceCyber.UMP.Common31031;
using VoiceCyber.Wpf.CustomControls;

namespace UMPS3103.Models
{
    public class ObjectItemTask : CheckableItemBase
    {
        public long ObjID { get; set; }
        public string Name { get; set; }
        public int ObjType { get; set; }
        public string Description { get; set; }
        public object Data { get; set; }
        private List<KeywordInfo> mListKeywordInfos = new List<KeywordInfo>();

        public List<KeywordInfo> ListKeywordInfos
        {
            get { return mListKeywordInfos; }
        }
    }
}
