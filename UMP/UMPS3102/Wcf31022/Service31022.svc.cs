using System;
using System.Collections.Generic;
using System.ServiceModel.Activation;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common31021;
using VoiceCyber.UMP.CommonService03;
using VoiceCyber.UMP.Communications;

namespace Wcf31022
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码、svc 和配置文件中的类名“Service31022”。
    // 注意: 为了启动 WCF 测试客户端以测试此服务，请在解决方案资源管理器中选择 Service31022.svc 或 Service31022.svc.cs，然后开始调试。
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class Service31022 : IService31022
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
                    case (int)S3102Codes.DownloadRecordFile:
                        optReturn = DownloadRecordFile(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.Data = optReturn.Data.ToString();
                        break;
                    case (int)S3102Codes.DecryptRecordFile:
                        optReturn = DecryptRecordFile(session, webRequest.ListData);
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

        private OperationReturn DownloadRecordFile(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0     SFTP服务器地址
                //1     SFTP服务器端口
                //2     录音流水号
                //3     录音编号
                //4     分表信息
                if (listParams == null || listParams.Count < 5)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strAddress = listParams[0];
                string strPort = listParams[1];
                string strReference = listParams[2];
                string strNo = listParams[3];
                string strPartition = listParams[4];
                int intValue;
                if (!int.TryParse(strPort, out intValue))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Port param  invalid");
                    return optReturn;
                }
                int intPort = intValue;
                Service03Helper helper = new Service03Helper();
                helper.HostAddress = session.AppServerInfo.Address;
                if (session.AppServerInfo.SupportHttps)
                {
                    helper.HostPort = session.AppServerInfo.Port - 4;
                }
                else
                {
                    helper.HostPort = session.AppServerInfo.Port - 3;
                }
                RequestMessage request = new RequestMessage();
                request.Command = (int)Service03Command.DownloadRecordFile;
                request.ListData.Add(strAddress);
                request.ListData.Add(intPort.ToString());
                request.ListData.Add(string.Format("{0}|{1}", session.UserID, session.RentInfo.Token));
                request.ListData.Add(session.UserInfo.Password);
                request.ListData.Add(strNo);
                request.ListData.Add(strReference);
                request.ListData.Add(strPartition);
                optReturn = helper.DoRequest(request);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                ReturnMessage retMessage = optReturn.Data as ReturnMessage;
                if (retMessage == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("ReturnMessage is null");
                    return optReturn;
                }
                if (!retMessage.Result)
                {
                    optReturn.Result = false;
                    optReturn.Code = retMessage.Code;
                    optReturn.Message = retMessage.Message;
                    return optReturn;
                }
                optReturn.Data = retMessage.Data;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }

        private OperationReturn DecryptRecordFile(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0     HostAddress
                //1     HostPort
                //2     SourceFile
                //3     Password
                if (listParams == null || listParams.Count < 4)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                //暂未实现
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }
    }
}
