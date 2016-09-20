using System.ServiceModel;
using Common3602;
using VoiceCyber.UMP.Communications;

namespace Wcf36021
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的接口名“IService36021”。
    [ServiceContract]
    public interface IService36021
    {
        [OperationContract]
        WebReturn UmpTaskOperation(WebRequest webRequest);
        [OperationContract]
        WebReturn UmpUpOperation(UpRequest upRequest);
    }
}
