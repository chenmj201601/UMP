﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using VoiceCyber.UMP.Communications;

namespace Wcf31081
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的接口名“IService1”。
    
    [ServiceContract(Namespace = "http://www.voicecyber.com/UMP/Services/2015/03")]

    public interface IService31081
    {
        [OperationContract]
        WebReturn DoOperation(WebRequest webRequest);
    }
}
