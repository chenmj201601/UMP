using Common6106;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UMPS6106.Service11012;
using UMPS6106.Service61061;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;

namespace UMPS6106
{
    public class PermissionFuncs
    {
        /// <summary>
        /// 获得当前用户可以管理的用户
        /// </summary>
        /// <returns></returns>
        public static OperationReturn GetCotrlUser()
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = Defines.RET_SUCCESS;
            Service11012Client client = null;
            try
            {
                string strUserID = App.Session.UserID.ToString();
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)RequestCode.WSGetUserCtlObjList;
                webRequest.Session = App.Session;
                webRequest.ListData.Add(App.Session.UserInfo.UserID.ToString());
                webRequest.ListData.Add("1");
                webRequest.ListData.Add(ConstValue.RESOURCE_USER.ToString());
                webRequest.ListData.Add(string.Empty);
                webRequest.ListData.Add("3");
                App.MonitorHelper.AddWebRequest(webRequest);
                client = new Service11012Client(WebHelper.CreateBasicHttpBinding(App.Session),
                    WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service11012"));
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.DoOperation(webRequest);
                App.MonitorHelper.AddWebReturn(webReturn);
                client.Close();
                if (!webReturn.Result)
                {
                    optReturn.Result = false;
                    optReturn.Code = webReturn.Code;
                    optReturn.Message = webReturn.Message;
                    return optReturn;
                }
                if (webReturn.ListData == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = (int)S6106WcfErrorCode.GetUserPermissionFailed;
                    return optReturn;
                }

                List<string> lstUserIDs = new List<string>();
                ResourceObject res = null;
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    optReturn = XMLHelper.DeserializeObject<ResourceObject>(webReturn.ListData[i]);
                    if (optReturn.Result)
                    {
                        res = optReturn.Data as ResourceObject;
                        lstUserIDs.Add(res.ObjID.ToString());
                    }
                }
                optReturn.Data = lstUserIDs;
                return optReturn;
            }
            catch (Exception ex)
            {
                App.ShowExceptionMessage(ex.Message);
                return optReturn;
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
        }

        /// <summary>
        /// 获得录音使用的模式（分机或坐席）
        /// </summary>
        /// <returns></returns>
        public static OperationReturn GetRecordMode()
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = Defines.RET_SUCCESS;
            Service61061Client client = null;
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)S6106RequestCode.GetRecordMode;
                webRequest.Session = App.Session;
                client = new Service61061Client(WebHelper.CreateBasicHttpBinding(App.Session),
                    WebHelper.CreateEndpointAddress(App.Session.AppServerInfo, "Service61061"));
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.DoOperation(webRequest);
                App.MonitorHelper.AddWebReturn(webReturn);
                client.Close();
                if (!webReturn.Result)
                {
                    optReturn.Result = false;
                    optReturn.Code = webReturn.Code;
                    optReturn.Message = webReturn.Message;
                    return optReturn;
                }
                optReturn.Data = webReturn.Data;
                return optReturn;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
                return optReturn;
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
        }
    }
}
