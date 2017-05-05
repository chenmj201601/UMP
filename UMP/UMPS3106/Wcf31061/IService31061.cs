using Common3106;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;

namespace Wcf31061
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的接口名“IService31061”。
    [ServiceContract]
    public interface IService31061
    {
        [OperationContract]
        WebReturn UMPTreeOperation(WebRequest webRequest);
        [OperationContract]
        WebReturn UMPUpOperation(UpRequest upRequest);        
    }
}
