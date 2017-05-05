using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace PFShareClassesC
{
    [ServiceContract(SessionMode = SessionMode.Allowed)]
    public interface IMessageServer2Client
    {
        [OperationContract(IsOneWay = true)]
        void CommandInClient(ShareClassForInterface AInterfaceArgs);
    }
}
