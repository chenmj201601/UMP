using System.ServiceModel;
using VoiceCyber.UMP.Communications;

namespace Wcf51011
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的接口名“IService51011”。
    [ServiceContract]
    public interface IService51011
    {
        [OperationContract]
        WebReturn UmpTaskOperation(WebRequest webRequest);
    }
}
