using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UMPS0001
{
    public interface S0001Interface
    {
        event EventHandler<OperationEventArgs> IOperationEvent;
    }

    public interface S0001ChangeLanguageInterface
    {
        event EventHandler<OperationEventArgs> IChangeLanguageEvent;
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

    public class OperationParameters : EventArgs
    {
        public String StrObjectTag;
        public Object ObjectSource0;
        public Object ObjectSource1;
        public Object ObjectSource2;
        public Object ObjectSource3;
        public Object ObjectSource4;
        public List<object> ListObjectSource = new List<object>();
        public string StrErrorMessage;
    }
}
