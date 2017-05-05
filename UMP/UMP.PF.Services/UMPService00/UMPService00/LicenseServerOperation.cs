using PFShareClassesS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Text;
using System.Xml;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Encryptions;

namespace UMPService00
{
    public class LicenseServerOperation
    {
        private string strXmlFilePath;
        private XmlDocument xmlDoc = null;
        private XMLOperator xmlOperator = null;
        private bool bIsEncryted = false;
        private string strSession = string.Empty;
        public bool bIsLoadXml = false;     //Load xml是否成功

        /// <summary>
        /// 在检查License服务器是否可用时调用 由于会传入IP和Port，所以不需要初始化数据
        /// </summary>
        public LicenseServerOperation()
        {
        }

        /// <summary>
        /// 在获取License信息时调用 需要传入当前服务所在的路径 拼出xml地址
        /// </summary>
        /// <param name="strFilePath"></param>
        public LicenseServerOperation(string strFilePath, string strFileName)
        {
            try
            {
                strXmlFilePath = strFilePath + strFileName;
                xmlDoc = new XmlDocument();
                xmlDoc.Load(strXmlFilePath);
                xmlOperator = new XMLOperator(xmlDoc);
                bIsLoadXml = true;
            }
            catch (Exception ex)
            {
                bIsLoadXml = false;
                UMPService00.WriteLog("LicenseServerOperation() error : " + ex.Message);
            }
        }

        public int CheckLicenseServer(string strHost, int iPort)
        {
            SslStream stream = null;
            TcpClient client = null;
            int iResult = ConnectToLicenseServer(strHost, iPort, 7692, ref stream, ref client);
            if (stream != null)
            {
                if (stream.CanRead)
                {
                    stream.Close();
                }
            }
            if (client != null)
            {
                if (client.Connected)
                {
                    client.Close();
                }
            }
            return iResult;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strHost"></param>
        /// <param name="iPort"></param>
        /// <param name="moduleTypeID">用来区分是管理员方式连接 和 取license的客户端连接</param>
        /// <param name="sslStream"></param>
        /// <param name="client"></param>
        /// <returns></returns>
        public int ConnectToLicenseServer(string strHost, int iPort, int moduleTypeID, ref SslStream sslStream, ref TcpClient client)
        {
            int iResult = 0;
            try
            {
                client = new TcpClient();
                for (int i = 0; i < 3; i++)
                {
                    try
                    {
                        client.Connect(strHost, iPort);
                        if (client.Connected)
                        {
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        UMPService00.WriteLog("ConnLic", string.Format("Fail.\t{0}", ex.Message));
                        continue;
                    }
                }
                if (!client.Connected)
                {
                    return -1;
                }
                sslStream = new SslStream(client.GetStream(), false, Common.ServerValidation, null);
                sslStream.AuthenticateAsClient(client.Client.RemoteEndPoint.ToString());
                string strResult = string.Empty;
                NetPacketHeader header = new NetPacketHeader();
                byte[] byteFollowMsg = new byte[1];
                ReceiveMessage(ref header, ref byteFollowMsg, sslStream, client);
                bIsEncryted = (header.State & 4) != 0;
                string strMsg = Processmessage(header, byteFollowMsg);
                JsonObject obj = new JsonObject(strMsg);
                strSession = obj[LicenseDefine.KEYWORD_MSG_DATA][LicenseDefine.KEYWORD_MSG_SESSION].GetValue().ToString();
                int iMessageID = int.Parse(obj[LicenseDefine.KEYWORD_MSG_MESSAGEID].ToString());
                if (iMessageID != 1)
                {
                    UMPService00.WriteLog("The welcome package error");
                    return -1;
                }

                if (iMessageID == 1)       //收到的是welcome 则返回logon数据包
                {
                    strMsg = string.Empty;
                    JsonObject json = new JsonObject();
                    json[LicenseDefine.KEYWORD_MSG_CLASSDESC] = new JsonProperty("\"authenticate\"");
                    json[LicenseDefine.KEYWORD_MSG_CLASSID] = new JsonProperty(LicenseDefine.LICENSE_MSG_CLASS_AUTHENTICATE);
                    json[LicenseDefine.KEYWORD_MSG_CURRENTTIME] = new JsonProperty(DateTime.Now.ToString("\""
                        + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\""));
                    json[LicenseDefine.KEYWORD_MSG_DATA] = new JsonProperty(new JsonObject());
                    json[LicenseDefine.KEYWORD_MSG_DATA][LicenseDefine.KEYWORD_MSG_HEARTBEAT] = new JsonProperty(LicenseDefine.NET_HEARTBEAT_INTEVAL);
                    json[LicenseDefine.KEYWORD_MSG_DATA][LicenseDefine.KEYWORD_MSG_MODULENAME] = new JsonProperty("\"License Test\"");
                    json[LicenseDefine.KEYWORD_MSG_DATA][LicenseDefine.KEYWORD_MSG_MODULENUMBER] = new JsonProperty(-1);
                    json[LicenseDefine.KEYWORD_MSG_DATA][LicenseDefine.KEYWORD_MSG_MODULETYPEID] = new JsonProperty(moduleTypeID);
                    json[LicenseDefine.KEYWORD_MSG_DATA][LicenseDefine.KEYWORD_MSG_PROTOCOL] = new JsonProperty("\"" + LicenseDefine.NET_PROTOCOL_VERSION + "\"");
                    json[LicenseDefine.KEYWORD_MSG_DATA][LicenseDefine.KEYWORD_MSG_SESSION] = new JsonProperty(strSession);
                    json[LicenseDefine.KEYWORD_MSG_DATA][LicenseDefine.KEYWORD_MSG_VERIFICATION] = new JsonProperty("\"" + Common.GetValidication(strSession) + "\"");
                    json[LicenseDefine.KEYWORD_MSG_MESSAGEDESC] = new JsonProperty("\"longon\"");
                    json[LicenseDefine.KEYWORD_MSG_MESSAGEID] = new JsonProperty(LicenseDefine.LICENSE_MSG_CLASS_CONNECTION);
                    strMsg = json.ToString();
                    SendMessage(json.ToString(), bIsEncryted, sslStream);
                    ReceiveMessage(ref header, ref byteFollowMsg, sslStream, client);
                    strMsg = Processmessage(header, byteFollowMsg);
                    obj = new JsonObject(strMsg);
                    iMessageID = int.Parse(obj[LicenseDefine.KEYWORD_MSG_MESSAGEID].GetValue().ToString());
                    if (iMessageID != 3)
                    {
                        UMPService00.WriteLog("License server verification error : ");
                        return -1;
                    }
                }
            }
            catch (Exception ex)
            {
                UMPService00.WriteLog("ConnLic", string.Format("Fail.\t{0}", ex.Message));
                iResult = -1;
            }
            return iResult;
        }

        /// <summary>
        /// 从GloableSetting中读取LicenseServer信息
        /// </summary>
        /// <returns></returns>
        public List<LicenseServer> GetLicenseServersOnUMPServer()
        {
            List<LicenseServer> lstServers = new List<LicenseServer>();
            try
            {
                XmlNode serversNode = xmlOperator.SelectNode("Parameters02/LicenseServer", "");
                LicenseServer server = null;
                string LStrVerificationCode001 = Common.CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M101);
                foreach (XmlNode node in serversNode.ChildNodes)
                {
                    if (!node.Name.Equals("LicServer"))
                    {
                        continue;
                    }
                    server = new LicenseServer();
                    int iEnable = int.Parse(EncryptionAndDecryption.EncryptDecryptString(xmlOperator.SelectAttrib(node, "P02"), LStrVerificationCode001, EncryptionAndDecryption.UMPKeyAndIVType.M101));
                    if (iEnable == 1)
                    {
                        server = new LicenseServer();
                        server.IsMain = int.Parse(xmlOperator.SelectAttrib(node, "P01"));
                        server.Host = EncryptionAndDecryption.EncryptDecryptString(xmlOperator.SelectAttrib(node, "P03"), LStrVerificationCode001, EncryptionAndDecryption.UMPKeyAndIVType.M101);
                        server.Port = int.Parse(EncryptionAndDecryption.EncryptDecryptString(xmlOperator.SelectAttrib(node, "P04"), LStrVerificationCode001, EncryptionAndDecryption.UMPKeyAndIVType.M101));
                        lstServers.Add(server);
                    }
                }
            }
            catch (Exception ex)
            {
                UMPService00.WriteLog("GetLicenseServersOnUMPServer() " + ex.Message);
            }
            return lstServers;
        }

        /// <summary>
        /// 从umpparam_simp.xml中读取License信息
        /// </summary>
        /// <returns></returns>
        public List<LicenseServer> GetLicenseServerOnVoiceServer()
        {
            List<LicenseServer> lstServers = new List<LicenseServer>();
            try
            {
                XmlNode serversNode = xmlOperator.SelectNode("Configurations/Configuration/Sites/Site/Resources/LicenseServers", "");
                LicenseServer server = null;
                string LStrVerificationCode001 = Common.CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M101);
                if (serversNode == null)
                {
                    return lstServers;
                }
                string strEnable = string.Empty;
                string strHostAddress = string.Empty;
                string strPort = string.Empty;
                foreach (XmlNode node in serversNode.ChildNodes)
                {
                    if (!node.Name.Equals("LicenseServer"))
                    {
                        continue;
                    }
                    strEnable = xmlOperator.SelectAttrib(node, "Enable");
                    if (!strEnable.Equals("1"))
                    {
                        continue;
                    }
                    server = new LicenseServer();
                    bool bLicIsNotNul = false;
                    foreach (XmlNode nodeChild in node.ChildNodes)
                    {
                        bLicIsNotNul = false;
                        if (nodeChild.Name.Equals("HostAddress"))
                        {
                            strHostAddress = xmlOperator.SelectAttrib(nodeChild, "Value");
                            if (string.IsNullOrEmpty(strHostAddress))
                            {
                                UMPService00.WriteLog("GetLicenseServerOnVoiceServer4 ");
                                break;
                            }
                        }
                        if (nodeChild.Name.Equals("HostPort"))
                        {
                            strPort = xmlOperator.SelectAttrib(nodeChild, "Value");
                            if (string.IsNullOrEmpty(strPort))
                            {
                                UMPService00.WriteLog("GetLicenseServerOnVoiceServer5 ");
                                break;
                            }
                        }
                        bLicIsNotNul = true;
                    }
                    if (bLicIsNotNul)
                    {
                        server.Host = EncryptionAndDecryption.EncryptDecryptString(strHostAddress, LStrVerificationCode001, EncryptionAndDecryption.UMPKeyAndIVType.M101);
                        server.Port = int.Parse(EncryptionAndDecryption.EncryptDecryptString(strPort, LStrVerificationCode001, EncryptionAndDecryption.UMPKeyAndIVType.M101));
                        lstServers.Add(server);
                    }
                }
            }
            catch (Exception ex)
            {
                UMPService00.WriteLog("GetLicenseServerOnVoiceServer() " + ex.Message);
            }
            return lstServers;
        }

        public string GetLicenseInfo(SslStream sslStream, TcpClient client, string strOpt)
        {
            string strResult = string.Empty;
            JsonObject json = new JsonObject();
            json[LicenseDefine.KEYWORD_MSG_CLASSDESC] = new JsonProperty("\"Request and response\"");
            json[LicenseDefine.KEYWORD_MSG_CLASSID] = new JsonProperty(LicenseDefine.LICENSE_MSG_CLASS_REQRES);
            json[LicenseDefine.KEYWORD_MSG_CURRENTTIME] = new JsonProperty(DateTime.Now.ToString("\""
                + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\""));
            json[LicenseDefine.KEYWORD_MSG_MESSAGEDESC] = new JsonProperty("\"request query total license\"");
            json[LicenseDefine.KEYWORD_MSG_MESSAGEID] = new JsonProperty(2);
            json[LicenseDefine.KEYWORD_MSG_REQUESTID] = new JsonProperty(1);
            //发送请求 获得所有的license信息
            SendMessage(json.ToString(), bIsEncryted, sslStream);
            JsonObject objAllLicenses = ThreadWorker(sslStream, client);
            if (objAllLicenses == null)
            {
                return "Error005";
            }
            if (strOpt == "G008")
            {
                strResult = threadReceive_RunWorkerCompleted(objAllLicenses, sslStream, client);
            }
            else if (strOpt == "G009")
            {
                strResult = threadReceive_RunWorkerCompleted_GetAll(objAllLicenses, sslStream, client);
            }
            else if (strOpt == "G010")
            {
                strResult = threadReceive_RunWorkerCompleted_GetSerialNumber(objAllLicenses, sslStream, client);
            }
            return strResult;
        }


        private JsonObject ThreadWorker(SslStream sslStream, TcpClient client)
        {
            NetPacketHeader header = new NetPacketHeader();
            byte[] byteFollowMsg = new byte[1];
            bool bIsGetResponse = false;        //是否已经获得了请求回应信息
            JsonObject objAllLicenses = null;
            while (!bIsGetResponse)
            {
                ReceiveMessage(ref header, ref byteFollowMsg, sslStream, client);
                string strMsg = Processmessage(header, byteFollowMsg);
                JsonObject obj = new JsonObject(strMsg);
                string st = obj[LicenseDefine.KEYWORD_MSG_CLASSDESC].GetValue().ToString();
                if (int.Parse(obj[LicenseDefine.KEYWORD_MSG_CLASSID].GetValue().ToString()) == 5
                    && int.Parse(obj[LicenseDefine.KEYWORD_MSG_MESSAGEID].GetValue().ToString()) == 3
                    && int.Parse(obj[LicenseDefine.KEYWORD_MSG_REQUESTID].GetValue().ToString()) == 1)
                {
                    objAllLicenses = obj;
                    bIsGetResponse = true;
                }
            }
            return objAllLicenses;
        }

        string threadReceive_RunWorkerCompleted(JsonObject objAllLicenses, SslStream sslStream, TcpClient client)
        {
            string strResult = string.Empty;
            try
            {
                //已经得到license信息 
                int iCount = objAllLicenses[LicenseDefine.KEYWORD_MSG_DATA][LicenseDefine.KEYWORD_MSG_TOTAL].Items.Count;
                if (iCount > 0)
                {
                    JsonProperty obj = null;
                    string strExpiration = string.Empty;
                    for (int i = 0; i < iCount; i++)
                    {
                        obj = objAllLicenses[LicenseDefine.KEYWORD_MSG_DATA][LicenseDefine.KEYWORD_MSG_TOTAL][i];
                        long iLicenseID = long.Parse(obj[LicenseDefine.KEYWORD_MSG_LICENSE_LICENSEID].GetValue().ToString());
                        if (iLicenseID == 1100002 || iLicenseID == 1100001)
                        {
                            strExpiration = obj[LicenseDefine.KEYWORD_MSG_EXPIRATION].GetValue().ToString();
                            strResult += iLicenseID + Common.AscCodeToChr(30) + obj[LicenseDefine.KEYWORD_MSG_LICENSE_VALUE].GetValue().ToString() + Common.AscCodeToChr(27);
                        }
                    }
                    strResult += "1100000" + Common.AscCodeToChr(30) + strExpiration + Common.AscCodeToChr(27);
                    //  UMPService00.IEventLog.WriteEntry("strResult = " + strResult, System.Diagnostics.EventLogEntryType.Warning);
                }
                if (sslStream != null)
                {
                    sslStream.Close();
                }
                if (client != null)
                {
                    client.Close();
                }
            }
            catch
            {
                return string.Empty;
            }

            return strResult;
        }

        string threadReceive_RunWorkerCompleted_GetAll(JsonObject objAllLicenses, SslStream sslStream, TcpClient client)
        {
            string strResult = string.Empty;
            try
            {
                //已经得到license信息 
                int iCount = objAllLicenses[LicenseDefine.KEYWORD_MSG_DATA][LicenseDefine.KEYWORD_MSG_TOTAL].Items.Count;
                if (iCount > 0)
                {
                    JsonProperty obj = null;
                    string strExpiration = string.Empty;
                    for (int i = 0; i < iCount; i++)
                    {
                        obj = objAllLicenses[LicenseDefine.KEYWORD_MSG_DATA][LicenseDefine.KEYWORD_MSG_TOTAL][i];
                        long iLicenseID = long.Parse(obj[LicenseDefine.KEYWORD_MSG_LICENSE_LICENSEID].GetValue().ToString());
                        if (iLicenseID > 1100100 && iLicenseID < 1100199)
                        {
                            strExpiration = obj[LicenseDefine.KEYWORD_MSG_EXPIRATION].GetValue().ToString();
                            strResult += iLicenseID + Common.AscCodeToChr(30) + obj[LicenseDefine.KEYWORD_MSG_LICENSE_VALUE].GetValue().ToString() + Common.AscCodeToChr(27);
                        }
                    }
                    strResult += "1100000" + Common.AscCodeToChr(30) + strExpiration + Common.AscCodeToChr(27);
                    //  UMPService00.IEventLog.WriteEntry("strResult = " + strResult, System.Diagnostics.EventLogEntryType.Warning);
                }
                if (sslStream != null)
                {
                    sslStream.Close();
                }
                if (client != null)
                {
                    client.Close();
                }
            }
            catch
            {
                return string.Empty;
            }

            return strResult;
        }

        string threadReceive_RunWorkerCompleted_GetSerialNumber(JsonObject objAllLicenses, SslStream sslStream, TcpClient client)
        {
            string strResult = string.Empty;
            try
            {
                //已经得到license信息 
                string strLicenseID = objAllLicenses[LicenseDefine.KEYWORD_MSG_DATA][LicenseDefine.KEYWORD_MSG_SERIALNUMBER].Value;
                strResult += "1200000" + Common.AscCodeToChr(30) + strLicenseID + Common.AscCodeToChr(27);
                //  UMPService00.IEventLog.WriteEntry("strResult = " + strResult, System.Diagnostics.EventLogEntryType.Warning);
                if (sslStream != null)
                {
                    sslStream.Close();
                }
                if (client != null)
                {
                    client.Close();
                }
            }
            catch
            {
                return string.Empty;
            }

            return strResult;
        }

        /// <summary>
        /// 接受数据
        /// </summary>
        private void ReceiveMessage(ref NetPacketHeader packHeader, ref byte[] byteFollowMsg, SslStream sslStream, TcpClient client)
        {
            string str = string.Empty;
            byte[] byteHeadMsg = new byte[LicenseDefine.NET_HEAD_SIZE];
            byte[] byteTemp = new byte[1024];   //临时存放数据
            byte[] byteReceive = new byte[16];  //用于接收消息
            int iReceiveResult = 0;
            int iTotalRecive = 0;   //计算总共收下多少数据
            bool isReceivedHead = false;        //包头有没有收完 true为收完 
            byte[] byteFollowData = new byte[1];
            int iReceiveByteLength = LicenseDefine.NET_HEAD_SIZE;
            NetPacketHeader disHead;

            while (true)
            {
                if (!sslStream.CanRead || !client.Connected)
                {
                    break;
                }
                byteReceive = new byte[iReceiveByteLength];
                iReceiveResult = sslStream.Read(byteReceive, 0, iReceiveByteLength);
                if (iReceiveResult <= 0)
                {
                    break;
                }
                if (!isReceivedHead)
                {
                    byteReceive.CopyTo(byteTemp, iTotalRecive);
                    if (iReceiveResult + iTotalRecive >= byteReceive.Length)
                    {
                        isReceivedHead = true;
                        disHead = (NetPacketHeader)Common.BytesToStruct(byteTemp, typeof(NetPacketHeader));
                        if (disHead.Flag[0] == 'L' && disHead.Flag[1] == 'M')
                        {
                            byteFollowData = new byte[disHead.Size];
                            iReceiveByteLength = (int)disHead.Size;
                            packHeader = disHead;
                            iTotalRecive = 0;
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        iTotalRecive += iReceiveResult;
                        continue;
                    }
                }
                else
                {
                    Array.Copy(byteReceive, 0, byteFollowData, iTotalRecive, iReceiveResult);
                    iTotalRecive += iReceiveResult;
                    //判断本次接收完后 如果下次接收的数据加上已经收完的数据 大于总数据数
                    //则设置下次接收的数据大小为总数据-已经接收完的数据 避免接收到后续包的数据 导致数据不完整
                    if (iTotalRecive + iReceiveByteLength > byteFollowData.Length)
                    {
                        iReceiveByteLength = byteFollowData.Length - iTotalRecive;
                    }
                    if (iTotalRecive == packHeader.Size)
                    {
                        break;
                    }
                }
            }
            byteFollowMsg = new byte[byteFollowData.Count()];
            byteFollowData.CopyTo(byteFollowMsg, 0);
        }

        /// <summary>
        /// 处理消息
        /// </summary>
        /// <param name="header"></param>
        /// <param name="byteData"></param>
        /// <returns></returns>
        public string Processmessage(NetPacketHeader header, byte[] byteData)
        {
            bool mIsEncryt = (header.State & 4) != 0;
            string strData = string.Empty;
            if (mIsEncryt)
            {
                //strData = AESEncryption.DecryptNN256(byteData);

                byte[] temp = ServerAESEncryption.DecryptBytes(byteData, EncryptionMode.AES256V13Hex);
                strData = Encoding.ASCII.GetString(temp).TrimEnd('\0','\r','\n');
            }
            else
            {
                strData = Encoding.ASCII.GetString(byteData);
            }
            strData = strData.Trim('\n');
            return strData;
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="msg"></param>
        public void SendMessage(string msg, bool bIsEncryted, SslStream sslStream)
        {
            NetPacketHeader header = new NetPacketHeader();
            header.Flag = Encoding.ASCII.GetBytes("LM");
            header.Format = 1;
            byte[] data;
            if (bIsEncryted)
            {
                header.State = 4;
                //data = AESEncryption.EncryptNN256Data(msg);

                byte[] temp = Encoding.ASCII.GetBytes(msg);
                data = ServerAESEncryption.EncryptBytes(temp, EncryptionMode.AES256V13Hex);
            }
            else
            {
                header.State = 0;
                data = Encoding.ASCII.GetBytes(msg);
            }
            header.Size = (uint)data.Length;
            byte[] headData = Common.StructToBytes(header);
            sslStream.Write(headData);
            sslStream.Write(data);
            sslStream.Flush();
        }
    }
}
