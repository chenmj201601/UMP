using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;
using Wcf61061.Service11012;
using Common6106;
using System.Data;

namespace Wcf61061
{
    /// <summary>
    /// 获得各种权限的类
    /// </summary>
    public class PermissionFuncs
    {
        /// <summary>
        /// 获得可以管理的坐席或者分机
        /// </summary>
        /// <param name="session"></param>
        /// <param name="strRecordModeParam">根据录音模式计算出的字符串 用于  webRequest.ListData【2】</param>
        /// <returns></returns>
        private static OperationReturn GetCtrlAgentOrExtensionByRecordMode(SessionInfo session, string strRecordModeParam, string strOrgID)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = Defines.RET_SUCCESS;

            try
            {
                string strUserID = session.UserID.ToString();
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)RequestCode.WSGetUserCtlObjList;
                webRequest.Session = session;
                webRequest.ListData.Add(session.UserID.ToString());
                webRequest.ListData.Add("0");
                webRequest.ListData.Add(strRecordModeParam);
                webRequest.ListData.Add(strOrgID);
                webRequest.ListData.Add("3");
                Service11012Client client = new Service11012Client(WebHelper.CreateBasicHttpBinding(session),
                    WebHelper.CreateEndpointAddress(session.AppServerInfo, "Service11012"));
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                optReturn.Message = webReturn.Message;
                if (!webReturn.Result)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_FAIL;
                    return optReturn;
                }
                if (webReturn.ListData.Count <= 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = (int)S6106WcfErrorCode.NoAgentOrExtension;
                    return optReturn;
                }

                List<ResourceObject> lstRes = new List<ResourceObject>();
                ResourceObject res = null;
                for (int i = 0; i < webReturn.ListData.Count; i++)
                {
                    optReturn = XMLHelper.DeserializeObject<ResourceObject>(webReturn.ListData[i]);
                    if (optReturn.Result)
                    {
                        res = optReturn.Data as ResourceObject;
                        lstRes.Add(res);
                    }
                }
                optReturn.Data = lstRes;
                return optReturn;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Message = ex.Message;
                optReturn.Code = (int)S6106WcfErrorCode.GetUserPermissionFailed;
                return optReturn;
            }
        }

        /// <summary>
        /// 获得可以管理的坐席和分机的名字 
        /// <param name="session"></param>
        /// strRecordMode 统计分机还是坐席   E：分机  A：坐席
        /// <returns></returns>
        public static OperationReturn GetCtrlAgentOrExtensionNames(SessionInfo session, string strRecordMode, string strOrgID)
        {
            OperationReturn optReturn = new OperationReturn();
            try
            {
                string strParam = string.Empty;
                if (strRecordMode == "E")
                {
                    //使用Extension
                    strParam = ConstValue.RESOURCE_EXTENSION.ToString();
                }
                else if (strRecordMode == "A")
                {
                    strParam = ConstValue.RESOURCE_AGENT.ToString();
                }
                else
                {
                    optReturn.Result = false;
                    optReturn.Code = (int)S6106WcfErrorCode.RecordModeValueError;
                    return optReturn;
                }
                optReturn = GetCtrlAgentOrExtensionByRecordMode(session, strParam, strOrgID);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                List<ResourceObject> lstRes = optReturn.Data as List<ResourceObject>;
                List<string> lstNames = new List<string>();
                for (int i = 0; i < lstRes.Count; i++)
                {
                    string strTemp = lstRes[i].ObjID.ToString() + ConstValue.SPLITER_CHAR + lstRes[i].Name;
                    lstNames.Add(strTemp);
                }

                //将获取的agent或者extension信息写到临时表00_901
                optReturn = InsertTempTable(session, lstNames);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                optReturn.Result = true;
                optReturn.Code = Defines.RET_SUCCESS;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }

        /// <summary>
        /// 将记录写入临时表
        /// </summary>
        /// <param name="session"></param>
        /// <param name="lstParams"></param>
        /// <returns></returns>
        private static OperationReturn InsertTempTable(SessionInfo session, List<string> lstParams)
        {
            OperationReturn optReturn = new OperationReturn();
            Service11012Client client = null;
            try
            {
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)RequestCode.WSInsertTempData;
                webRequest.Session = session;
                webRequest.ListData.Add(string.Empty);  //此值为空 
                webRequest.ListData.Add(lstParams.Count.ToString());
                lstParams.ForEach(obj => webRequest.ListData.Add(obj));
                client = new Service11012Client(
                    WebHelper.CreateBasicHttpBinding(session)
                    , WebHelper.CreateEndpointAddress(session.AppServerInfo, "Service11012"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                optReturn.Message = webReturn.Message;
                if (!webReturn.Result)
                {
                    optReturn.Result = false;
                    optReturn.Code = webReturn.Code;
                    return optReturn;
                }

                optReturn.Result = true;
                optReturn.Code = Defines.RET_SUCCESS;
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

        public static OperationReturn GetCotrlUser(SessionInfo session)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = Defines.RET_SUCCESS;
            Service11012Client client = null;
            try
            {
                string strUserID = session.UserID.ToString();
                WebRequest webRequest = new WebRequest();
                webRequest.Code = (int)RequestCode.WSGetUserCtlObjList;
                webRequest.Session = session;
                webRequest.ListData.Add(session.UserID.ToString());
                webRequest.ListData.Add("1");
                webRequest.ListData.Add(ConstValue.RESOURCE_USER.ToString());
                webRequest.ListData.Add(string.Empty);
                webRequest.ListData.Add("3");
                client = new Service11012Client(WebHelper.CreateBasicHttpBinding(session),
                    WebHelper.CreateEndpointAddress(session.AppServerInfo, "Service11012"));
                WebHelper.SetServiceClient(client);
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    optReturn.Result = false;
                    optReturn.Code = webReturn.Code;
                    optReturn.Message = webReturn.Message;
                    return optReturn;
                }
                if (webReturn.ListData.Count <= 0)
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
                optReturn = InsertTempTable(session, lstUserIDs);
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