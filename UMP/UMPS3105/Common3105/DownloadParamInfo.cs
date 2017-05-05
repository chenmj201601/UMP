using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common3105
{
    public class DownloadParamInfo
    {
        public long ObjID { get; set; }
        public int ID { get; set; }
        public bool IsEnabled { get; set; }
        public int Method { get; set; }
        public int VoiceID { get; set; }
        public string VoiceAddress { get; set; }
        public string Address { get; set; }
        public int Port { get; set; }
        public string RootDir { get; set; }
        public string VocPathFormat { get; set; }
        public string ScrPathFormat { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
