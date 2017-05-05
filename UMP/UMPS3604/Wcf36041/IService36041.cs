using System.ServiceModel;
using Common3604;
using VoiceCyber.UMP.Communications;

namespace Wcf36041
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService36041" in both code and config file together.
    [ServiceContract]
    public interface IService36041
    {
        [OperationContract]
        WebReturn UmpTaskOperation(WebRequest webRequest);
        [OperationContract]
        WebReturn UmpUpOperation(UpRequest upRequest);  
    }
}
