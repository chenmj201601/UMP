using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel.Activation;
using System.Xml;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common98001;
using VoiceCyber.UMP.Communications;

namespace Wcf98001
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码、svc 和配置文件中的类名“Service98001”。
    // 注意: 为了启动 WCF 测试客户端以测试此服务，请在解决方案资源管理器中选择 Service98001.svc 或 Service98001.svc.cs，然后开始调试。
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class Service98001 : IService98001
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
                switch (webRequest.Code)
                {
                    case (int)S9800Codes.GetModuleList:
                        optReturn = GetModuleList(session, webRequest.ListData);
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
                webReturn.Message = ex.Message;
                return webReturn;
            }
            return webReturn;
        }

        private OperationReturn GetModuleList(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0      用户编号
                if (listParams == null || listParams.Count < 1)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strUserID = listParams[0];
                string dir = AppDomain.CurrentDomain.BaseDirectory;
                dir = dir.Substring(0, dir.LastIndexOf("\\"));
                dir = dir.Substring(0, dir.LastIndexOf("\\"));
                dir = Path.Combine(dir, "GlobalSettings");
                string path = Path.Combine(dir, "UMP.Server.04.xml");
                if (!File.Exists(path))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_FILE_NOT_EXIST;
                    optReturn.Message = string.Format("Config file not exist.\t{0}", path);
                    return optReturn;
                }
                XmlDocument doc = new XmlDocument();
                doc.Load(path);
                var appsNode = doc.SelectSingleNode("UMPSetted/ThirdPartyApplications");
                if (appsNode == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("ThirdPartyApplications node not exist.");
                    return optReturn;
                }
                var moduleNodes = appsNode.ChildNodes;
                List<ModuleInfo> listModules = new List<ModuleInfo>();
                for (int i = 0; i < moduleNodes.Count; i++)
                {
                    var moduleNode = moduleNodes[i];
                    if (moduleNode.Name != "Application") { continue; }
                    if (moduleNode.Attributes == null) { continue; }
                    int moduleID;
                    string url;
                    string appName;
                    string fullName;
                    var attr = moduleNode.Attributes["Attribute01"];
                    if (attr == null) { continue; }
                    string str = attr.Value;
                    if (!int.TryParse(str, out moduleID))
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_CONFIG_INVALID;
                        optReturn.Message = string.Format("ModuleID(Atrribute01) invalid.");
                        return optReturn;
                    }
                    attr = moduleNode.Attributes["Attribute02"];
                    if (attr == null) { continue; }
                    str = attr.Value;
                    url = str;
                    attr = moduleNode.Attributes["Attribute03"];
                    if (attr == null) { continue; }
                    appName = attr.Value;
                    attr = moduleNode.Attributes["Attribute04"];
                    if (attr == null) { continue; }
                    fullName = attr.Value;
                    ModuleInfo info = new ModuleInfo();
                    info.Module = moduleID;
                    info.Url = url;
                    info.AppName = appName;
                    info.FullName = fullName;
                    var temp = listModules.FirstOrDefault(t => t.Module == info.Module);
                    if (temp == null)
                    {
                        listModules.Add(info);
                    }
                }
                List<string> listReturn = new List<string>();
                for (int i = 0; i < listModules.Count; i++)
                {
                    var info = listModules[i];
                    optReturn = XMLHelper.SeriallizeObject(info);
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                    listReturn.Add(optReturn.Data.ToString());
                }
                optReturn.Data = listReturn;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
                return optReturn;
            }
            return optReturn;
        }
    }
}
