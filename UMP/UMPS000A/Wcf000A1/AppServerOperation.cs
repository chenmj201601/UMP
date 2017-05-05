using System;
using System.IO;
using System.Xml;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;

namespace Wcf000A1
{
    public partial class Service000A1
    {
        private OperationReturn ReadAppServerInfo()
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                string path = AppDomain.CurrentDomain.BaseDirectory;
                path = path.Substring(0, path.LastIndexOf("\\"));
                path = path.Substring(0, path.LastIndexOf("\\"));
                path = Path.Combine(path, "GlobalSettings\\UMP.Server.01.xml");
                if (!File.Exists(path))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_FILE_NOT_EXIST;
                    optReturn.Message = string.Format("UMP.Server\\Args01.UMP.xml file not exist.\t{0}", path);
                    return optReturn;
                }
                XmlDocument doc = new XmlDocument();
                doc.Load(path);
                XmlNode node = doc.SelectSingleNode("UMPSetted/IISBindingProtocol");
                if (node == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("IISBindingProtocol node not exist");
                    return optReturn;
                }
                AppServerInfo appServerInfo = new AppServerInfo();
                int intPort;
                XmlNodeList listNodes = node.ChildNodes;
                for (int i = 0; i < listNodes.Count; i++)
                {
                    XmlNode temp = listNodes[i];
                    if (temp.Attributes != null)
                    {
                        var protocol = temp.Attributes["Protocol"];
                        if (protocol != null && protocol.Value == "http")
                        {
                            appServerInfo.Protocol = "http";
                            var strPort = temp.Attributes["BindInfo"].Value;
                            if (int.TryParse(strPort, out intPort) && intPort > 0)
                            {
                                appServerInfo.Port = intPort;
                            }
                            var strAddress = temp.Attributes["IPAddress"].Value;
                            if (!string.IsNullOrEmpty(strAddress))
                            {
                                appServerInfo.Address = strAddress;
                            }
                        }
                    }
                }
                optReturn.Data = appServerInfo;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
                optReturn.Exception = ex;
            }
            return optReturn;
        }
    }
}