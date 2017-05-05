using System.ServiceModel;
using Common3601;
using VoiceCyber.UMP.Communications;

namespace Wcf36011
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的接口名“IService36011”。
    [ServiceContract]
    public interface IService36011
    {
        [OperationContract]
        WebReturn UmpTaskOperation(WebRequest webRequest);
        [OperationContract]
        WebReturn UmpUpOperation(UpRequest upRequest); 
    }
}
