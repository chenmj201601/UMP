using Common11121;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;

namespace Wcf11121
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码、svc 和配置文件中的类名“Service1”。
    // 注意: 为了启动 WCF 测试客户端以测试此服务，请在解决方案资源管理器中选择 Service1.svc 或 Service1.svc.cs，然后开始调试。
     [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public partial class Service11121 : IService11121
    {
         public WebReturn DoOperation(WebRequest webRequest)
         {
             WebReturn webReturn = new WebReturn();
             webReturn.Result = true;
             webReturn.Code = 0;
             if (webRequest == null)
             {
                 webReturn.Result = false;
                 webReturn.Code = Defines.RET_PARAM_INVALID;
                 webReturn.Message = string.Format("WebRequest is null");
                 return webReturn;
             }
             SessionInfo session = webRequest.Session;
             if (session == null)
             {
                 webReturn.Result = false;
                 webReturn.Code = Defines.RET_PARAM_INVALID;
                 webReturn.Message = string.Format("SessionInfo is null");
                 return webReturn;
             }
             webReturn.Session = session;
             try
             {
                 OperationReturn optReturn;
                 DatabaseInfo dbInfo = session.DatabaseInfo;
                 if (dbInfo != null)
                 {
                     dbInfo.RealPassword = DecryptFromClient(dbInfo.Password);
                     session.DBConnectionString = dbInfo.GetConnectionString();
                 }
                 switch (webRequest.Code)
                 {
                     case (int)S1112Codes.GetDomainInfo:
                         optReturn = GetDomainInfo(session, webRequest.ListData);
                         if (!optReturn.Result)
                         {
                             webReturn.Result = false;
                             webReturn.Code = optReturn.Code;
                             webReturn.Message = optReturn.Message;
                             return webReturn;
                         }
                         webReturn.ListData = optReturn.Data as List<string>;
                         break;
                     case (int)S1112Codes.SaveDomainInfo:
                         optReturn = SaveDomainInfo(session, webRequest.ListData);
                         if (!optReturn.Result)
                         {
                             webReturn.Result = false;
                             webReturn.Code = optReturn.Code;
                             webReturn.Message = optReturn.Message;
                             return webReturn;
                         }
                         webReturn.ListData = optReturn.Data as List<string>;
                         break;
                     case (int)S1112Codes.CheckDomainInfo:
                         optReturn = CheckDomainInfo(session, webRequest.ListData);
                         if (!optReturn.Result)
                         {
                             webReturn.Result = false;
                             webReturn.Code = optReturn.Code;
                             webReturn.Message = optReturn.Message;
                             return webReturn;
                         }
                         webReturn.ListData = optReturn.Data as List<string>;
                         break;
                     default:
                         webReturn.Result = false;
                         webReturn.Code = Defines.RET_PARAM_INVALID;
                         webReturn.Message = string.Format("Request code invalid.\t{0}", webRequest.Code);
                         return webReturn;
                 }
                 webReturn.Message = optReturn.Message;
             }
             catch (Exception ex)
             {
                 webReturn.Result = false;
                 webReturn.Code = Defines.RET_FAIL;
                 webReturn.Message = string.Format("DoOperation:{0}", ex.Message);
                 return webReturn;
             }
             return webReturn;
         }

    }
}
