using System.ServiceModel;
using VoiceCyber.UMP.Common000A1;

namespace Wcf000A1
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的接口名“IService000A1”。
    [ServiceContract(Namespace = "http://www.voicecyber.com/UMP/Services/2015/03")]
    public interface IService000A1
    {
        [OperationContract]
        SDKReturn DoOperation(SDKRequest webRequest);
    }
}
