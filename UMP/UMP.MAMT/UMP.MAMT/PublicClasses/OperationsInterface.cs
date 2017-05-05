using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UMP.MAMT.PublicClasses
{
    interface MamtOperationsInterface
    {
        event EventHandler<MamtOperationEventArgs> IOperationEvent;
    }

    public class MamtOperationEventArgs : EventArgs
    {
        public String StrElementTag;
        public Object ObjSource;
        public Object AppenObjeSource0;
        public Object AppenObjeSource1;
        public Object AppenObjeSource2;
        public Object AppenObjeSource3;
        public List<object> ListObjectSource = new List<object>();
        public string StrErrorMessage;
    }
}
