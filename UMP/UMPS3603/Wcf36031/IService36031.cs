using System.ServiceModel;
using VoiceCyber.UMP.Communications;

namespace Wcf36031
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的接口名“IService36031”。
    [ServiceContract]
    public interface IService36031
    {
        [OperationContract]
        WebReturn UmpTaskOperation(WebRequest webRequest);
    }
}
