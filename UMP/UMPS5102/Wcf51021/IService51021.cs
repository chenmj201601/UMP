using System.ServiceModel;
using VoiceCyber.UMP.Communications;

namespace Wcf51021
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService51021" in both code and config file together.
    [ServiceContract]
    public interface IService51021
    {
        [OperationContract]
        WebReturn UmpTaskOperation(WebRequest webRequest);
    }
}
