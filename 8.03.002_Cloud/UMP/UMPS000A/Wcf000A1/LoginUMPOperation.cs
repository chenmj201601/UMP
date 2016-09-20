using System;
using System.Collections.Generic;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;

namespace Wcf000A1
{
    public partial class Service000A1
    {
        private OperationReturn LogOnUMP(List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {

                #region 解析参数

                //ListParams
                //参考S000ACodes中的说明，此处从略
                if (listParams == null || listParams.Count < 4)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strAccount = listParams[0];
                string strPassword = listParams[1];
                string strLoginMethod = listParams[2];
                string strLoginHost = listParams[3];
                string strLoginIP = string.Empty;
                if (listParams.Count > 4)
                {
                    strLoginIP = listParams[4];
                }
                else
                {

                    #region 获取客户端IP地址

                    string strRemote = string.Empty;
                    OperationContext context = OperationContext.Current;
                    MessageProperties properties = context.IncomingMessageProperties;
                    RemoteEndpointMessageProperty endpoint =
                        properties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                    if (endpoint != null)
                    {
                        strRemote = endpoint.Address;
                    }
                    strLoginIP = strRemote;

                    #endregion

                }
                WriteOperationLog(
                    string.Format(
                        "LogOnUMP:\tAccount:{0};Password:***;LoginMethod:{1};LoginHost:{2};LoginIP:{3};",
                        strAccount,
                        strLoginMethod,
                        strLoginHost,
                        strLoginIP));

                #endregion


                #region 参数验证



                #endregion


                #region 读取AppServerInfo

                optReturn = ReadAppServerInfo();
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                AppServerInfo appServerInfo = optReturn.Data as AppServerInfo;
                if (appServerInfo == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("AppServerInfo is null");
                    return optReturn;
                }

                #endregion


                #region 向Service01发送消息

                string strSendMessage = string.Format("{0}{1}{2}{1}{3}{1}{4}{1}{5}{1}{6}{1}{7}",
                    EncryptToClient("M01A01"),
                    ConstValue.SPLITER_CHAR,
                    EncryptToClient(strAccount),
                    EncryptToClient(strPassword),
                    EncryptToClient(strLoginMethod),
                    EncryptToClient("11000"),
                    EncryptToClient(strLoginHost),
                    EncryptToClient(strLoginIP));

                WriteOperationLog(string.Format("LogOnUMP:\tSendMessage:{0}", strSendMessage));

                TcpClient tcpClient = new TcpClient("127.0.0.1", appServerInfo.SupportHttps ? appServerInfo.Port - 2 : appServerInfo.Port - 1);
                SslStream sslStream = new SslStream(tcpClient.GetStream(), false, (s, cert, chain, err) => true);
                sslStream.AuthenticateAsClient("VoiceCyber.PF", null, SslProtocols.Default, false);
                byte[] byteData = Encoding.UTF8.GetBytes(strSendMessage + "\r\n");
                sslStream.Write(byteData, 0, byteData.Length);
                sslStream.Flush();
                string strReadedMessage = string.Empty;
                if (!ReadMessageFromServer(sslStream, ref strReadedMessage))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_FAIL;
                    optReturn.Message = strReadedMessage;
                    return optReturn;
                }
                WriteOperationLog(string.Format("LogOnUMP:\tReadedMessage:{0}", strReadedMessage));
                string[] arrReadedMessage = strReadedMessage.Split(new[] { ConstValue.SPLITER_CHAR },
                    StringSplitOptions.None);
                List<string> listReturn = new List<string>();
                for (int i = 0; i < arrReadedMessage.Length; i++)
                {
                    listReturn.Add(DecryptFromClient(arrReadedMessage[i]));
                }


                #region 如果登录成功，获取用户所在的部门的编码和部门名称

                if (listReturn.Count > 0)
                {
                    string strReturnCode = listReturn[0];
                    if (strReturnCode == "S01A00"
                        || strReturnCode == "S01A02"
                        || strReturnCode == "S01A03")
                    {
                        if (listReturn.Count > 2)
                        {
                            string strUserID = listReturn[2];
                            List<string> listRequestParams = new List<string>();
                            listRequestParams.Add(strUserID);
                            listRequestParams.Add("0");
                            listRequestParams.Add(ConstValue.RESOURCE_ORG.ToString());
                            listRequestParams.Add("-1");
                            optReturn = GetUserCtlObjList(listRequestParams);
                            if (!optReturn.Result)
                            {
                                return optReturn;
                            }
                            List<string> listOrgReturn = optReturn.Data as List<string>;
                            if (listOrgReturn != null
                                && listOrgReturn.Count > 0)
                            {
                                string strOrgInfo = listOrgReturn[0];
                                optReturn = XMLHelper.DeserializeObject<ResourceObject>(strOrgInfo);
                                if (!optReturn.Result)
                                {
                                    return optReturn;
                                }
                                ResourceObject org = optReturn.Data as ResourceObject;
                                if (org != null)
                                {
                                    listReturn.Add(org.ObjID.ToString());       //编码
                                    listReturn.Add(org.Name);                   //名称
                                }
                            }
                        }
                    }
                }

                #endregion


                sslStream.Close();
                tcpClient.Close();

                #endregion

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

        private OperationReturn LogOutUMP(List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {

                #region 解析参数

                //ListParams
                //参考S000ACodes中的说明，此处从略
                if (listParams == null || listParams.Count < 3)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strRentToken = listParams[0];
                string strUserID = listParams[1];
                string strSessionID = listParams[2];

                WriteOperationLog(
                    string.Format(
                        "LogOutUMP:\tRentToken:{0};UserID:{1};SessionID:{2};",
                        strRentToken,
                        strUserID,
                        strSessionID));

                #endregion


                #region 参数验证



                #endregion


                #region 读取AppServerInfo

                optReturn = ReadAppServerInfo();
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                AppServerInfo appServerInfo = optReturn.Data as AppServerInfo;
                if (appServerInfo == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("AppServerInfo is null");
                    return optReturn;
                }

                #endregion


                #region 向Service01发送消息

                string strSendMessage = string.Format("{0}{1}{2}{1}{3}{1}{4}",
                    EncryptToClient("M01A02"),
                    ConstValue.SPLITER_CHAR,
                    EncryptToClient(strRentToken),
                    EncryptToClient(strUserID),
                    EncryptToClient(strSessionID));

                WriteOperationLog(string.Format("LogOutUMP:\tSendMessage:{0}", strSendMessage));

                TcpClient tcpClient = new TcpClient("127.0.0.1", appServerInfo.SupportHttps ? appServerInfo.Port - 2 : appServerInfo.Port - 1);
                SslStream sslStream = new SslStream(tcpClient.GetStream(), false, (s, cert, chain, err) => true);
                sslStream.AuthenticateAsClient("VoiceCyber.PF", null, SslProtocols.Default, false);
                byte[] byteData = Encoding.UTF8.GetBytes(strSendMessage + "\r\n");
                sslStream.Write(byteData, 0, byteData.Length);
                sslStream.Flush();
                string strReadedMessage = string.Empty;
                if (!ReadMessageFromServer(sslStream, ref strReadedMessage))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_FAIL;
                    optReturn.Message = strReadedMessage;
                    return optReturn;
                }
                WriteOperationLog(string.Format("LogOutUMP:\tReadedMessage:{0}", strReadedMessage));
                string[] arrReadedMessage = strReadedMessage.Split(new[] { ConstValue.SPLITER_CHAR },
                    StringSplitOptions.None);
                List<string> listReturn = new List<string>();
                for (int i = 0; i < arrReadedMessage.Length; i++)
                {
                    listReturn.Add(DecryptFromClient(arrReadedMessage[i]));
                }
                sslStream.Close();
                tcpClient.Close();

                #endregion

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

        private bool ReadMessageFromServer(SslStream sslStream, ref string strReadedMessage)
        {
            bool bReturn = true;

            try
            {
                StringBuilder sb = new StringBuilder();
                int intLength, intPos;
                byte[] byteBuffer = new byte[1024];

                do
                {
                    intLength = sslStream.Read(byteBuffer, 0, byteBuffer.Length);
                    Decoder decoder = Encoding.UTF8.GetDecoder();
                    char[] arrChars = new char[decoder.GetCharCount(byteBuffer, 0, intLength)];
                    decoder.GetChars(byteBuffer, 0, intLength, arrChars, 0);
                    sb.Append(arrChars);
                    if (sb.ToString().IndexOf("\r\n") > 0) { break; }
                }
                while (intLength != 0);
                strReadedMessage = sb.ToString();
                intPos = strReadedMessage.IndexOf("\r\n");
                if (intPos > 0)
                {
                    strReadedMessage = strReadedMessage.Substring(0, intPos);
                }
            }
            catch (Exception ex)
            {
                bReturn = false;
                strReadedMessage = ex.ToString();
            }

            return bReturn;
        }
    }
}