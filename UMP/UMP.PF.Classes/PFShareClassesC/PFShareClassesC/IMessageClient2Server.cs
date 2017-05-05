using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace PFShareClassesC
{
    [ServiceContract(SessionMode = SessionMode.Allowed)]
    public interface IMessageClient2Server
    {
        [OperationContract(IsOneWay = true)]
        void ProcessingClientMessage(ShareClassForInterface AInterfaceArgs);
    }
}
