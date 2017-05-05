using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace YoungClassesLibrary
{
    [ServiceContract(SessionMode = SessionMode.Allowed)]
    public interface IClient2ServerMessage
    {
        [OperationContract(IsOneWay = true)]
        void ProcessingClientMessage(ShareClassesInterface AInterfaceArgs);
    }
}
