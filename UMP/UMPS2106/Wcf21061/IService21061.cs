﻿using System.ServiceModel;
using VoiceCyber.UMP.Communications;

namespace Wcf21061
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的接口名“IService21061”。
    [ServiceContract(Namespace = "http://www.voicecyber.com/UMP/Services/2015/03")]
    public interface IService21061
    {
        [OperationContract]
        WebReturn DoOperation(WebRequest webRequest);
    }
}
