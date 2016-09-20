using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PFShareControls
{
    public interface PFShareInterface
    {
        event EventHandler<OperationEventArgs> IOperationEvent;
    }

    public class OperationEventArgs : EventArgs
    {
        public String StrObjectTag;
        public Object ObjectSource;
    }
}
