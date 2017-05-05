using PFShareClassesS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using VoiceCyber.UMP.Communications;
using Wcf61012;

namespace Wcf61012
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的接口名“IService1”。
    [ServiceContract]
    public interface IService61012
    {
        [OperationContract]
        WebReturn UMPReportOperation(WebRequest webRequest);//这个公共方法是包括了界面上面的所有操作
        // TODO: 在此添加您的服务操作
    }
}
