using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UMP.Tools.PublicClasses
{
    interface OperationsInterface
    {
        event EventHandler<OperationEventArgs> IOperationEvent;
    }

    public class OperationEventArgs : EventArgs
    {
        public String StrElementTag;
        public Object ObjSource;
        public Object AppenObjeSource1;
        public Object AppenObjeSource2;
        public Object AppenObjeSource3;
        public Object AppenObjeSource4;
        public Object AppenObjeSource5;
    }
}
