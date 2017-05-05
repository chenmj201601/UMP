using Common1600;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;
using WCF16001.Service16002;

namespace WCF16001
{
    /// <summary>
    /// 操作数据库资源表  更新用户的在线状态
    /// </summary>
    public class ResourceOperations
    {
        /// <summary>
        /// 更新用户在线状态
        /// </summary>
        /// <param name="session"></param>
        /// <param name="strStatus"></param>
        /// strStatus :  0:离线        1：在线
        /// <returns></returns>
        public static OperationReturn ChangeUserStatus(SessionInfo session, string strStatus)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = Defines.RET_SUCCESS;
            Service16002Client client = null;
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S1600RequestCode.ChangeUserStatus;
                webRequest.Session = session;
                webRequest.ListData.Add(strStatus);
                client = new Service16002Client(WebHelper.CreateBasicHttpBinding(session),
                      WebHelper.CreateEndpointAddress(session.AppServerInfo, "Service16002"));
                WebReturn webReturn = client.DoOperation(webRequest);
                optReturn.Message = webReturn.Message;
                if (!webReturn.Result)
                {
                    optReturn.Result = false;
                    optReturn.Code = (int)S1600WcfError.SetUserStatusError;
                    return optReturn;
                }
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
            }
            finally
            {
                if (client != null)
                {
                    if (client.State == System.ServiceModel.CommunicationState.Opened)
                    {
                        client.Close();
                    }
                }
            }
            return optReturn;
        }
    }
}