using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UMPS1103
{
    public class ObjctColumnDefine
    {
        public string VObjectData { get; set; }
        public string VObjectNumber { get; set; }
        public string VObjectName { get; set; }
        public string VObjectOther1 { get; set; }

        public ObjctColumnDefine(string AObjectNumber, string AObjectName)
        {
            VObjectNumber = AObjectNumber;
            VObjectName = AObjectName;
        }

        public ObjctColumnDefine(string AObjectData, string AObjectNumber, string AObjectName, string AObjectOther1)
        {
            VObjectData = AObjectData;
            VObjectNumber = AObjectNumber;
            VObjectName = AObjectName;
            VObjectOther1 = AObjectOther1;
        }
    }
}
