using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace YoungClassesLibrary
{
    [ServiceContract(SessionMode = SessionMode.Allowed)]
    public interface IServer2ClientMessage
    {
        [OperationContract(IsOneWay = true)]
        void CommandInClient(ShareClassesInterface AInterfaceArgs);
    }
}
