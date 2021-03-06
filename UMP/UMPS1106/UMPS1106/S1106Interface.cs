﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UMPS1106
{
    public interface S1106Interface
    {
        event EventHandler<OperationEventArgs> IOperationEvent;
    }

    public interface S1106ChangeLanguageInterface
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
}
