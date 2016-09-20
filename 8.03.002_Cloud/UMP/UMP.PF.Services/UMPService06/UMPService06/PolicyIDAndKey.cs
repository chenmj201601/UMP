using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMPService06
{
    class   ObjectBindingPolicy
    {
        public long PolicyID { set; get; }
        public DateTime DurationBegin { set; get; }
        public DateTime DurationEnd { set; get; }
    }

    class EncryptionKeyDictionary 
    {
        public string KeyID { set; get; }
        public string PolicyID { set; get; }
        public string Key1b { set; get; }
        public string Key1d { set; get; }
        public DateTime EffectiveTime { set; get; }
        public DateTime InvalidTime { set; get; }
    }
}
