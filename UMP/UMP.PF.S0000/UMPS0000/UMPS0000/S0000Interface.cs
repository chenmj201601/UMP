using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UMPS0000
{
    public interface S0000Interface
    {
        event EventHandler<OperationEventArgs> IOperationEvent;
    }

    public class OperationEventArgs : EventArgs
    {
        public String StrObjectTag;
        public Object ObjectSource0;
        public Object ObjectSource1;
        public Object ObjectSource2;
        public Object ObjectSource3;
        public Object ObjectSource4;
        public List<object> ListObjectSource = new List<object>();
    }
}
