using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoiceCyber.UMP.Common31041.Common3102
{
    public class SettingInfo
    {
        public long UserID { get; set; }
        public int ParamID { get; set; }
        public int GroupID { get; set; }
        public int SortID { get; set; }
        public int DataType { get; set; }
        public string Description { get; set; }
        public int IntValue { get; set; }
        public string StringValue { get; set; }
        public object ObjValue { get; set; }
    }
}
