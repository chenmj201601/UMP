using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml;
using VoiceCyber.Common;
using System.Runtime.InteropServices;
using System.Threading;

namespace UMPService00
{
    /// <summary>
    /// 发送广播消息、写LocalMachine中日志路径的类
    /// </summary>
    public class SendBroadcastData
    {
        /// <summary>
        /// 写入日志路径到LocalMachine.ini中
        /// </summary>
        public static bool WriteLogPath(string strPath)
        {
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(strPath);
            XMLOperator xmlOperator = new XMLOperator(xmldoc);
            XmlNode LocalMachineNode = xmlOperator.SelectNode("Configurations/Configuration/LocalMachine", "");
            string strLogPath = xmlOperator.SelectAttrib(LocalMachineNode, "LogPath");
            string strProgramDataPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\VoiceCyber\\UMP\\config";
            string strLocalMachineIni = strProgramDataPath + "\\localmachine.ini";
            IniOperation ini = new IniOperation(strLocalMachineIni);
            ini.IniWriteValue("LocalMachine", "LogPath", strLogPath);
          //  UMPService00.IEventLog.WriteEntry("LogPath = " + strLogPath, EventLogEntryType.Warning);
            return true;
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="strMessage">xml文件路径</param>
        public static bool SendBroadcastMessage(string strMessage)
        {
            string strProgramDataPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\VoiceCyber\\UMP\\config";
            string strLocalMachineIni = strProgramDataPath + "\\localmachine.ini";
            IniOperation ini = new IniOperation(strLocalMachineIni);
            string strHostAndPort = ini.IniReadValue("LocalMachine", "SubscribeAddress");
            //IP端口字符串不正确
            if (string.IsNullOrEmpty(strHostAndPort) || !strHostAndPort.Contains(','))
            {
               // UMPService00.IEventLog.WriteEntry("localmachine.ini  SubscribeAddress value is error", EventLogEntryType.Error);
                UMPService00.WriteLog(LogMode.Error, "localmachine.ini  SubscribeAddress value is error");
                return false;
            }
            string strHost = strHostAndPort.Substring(0, strHostAndPort.IndexOf(','));
            int iPort = int.Parse(strHostAndPort.Substring(strHostAndPort.IndexOf(',') + 1));
            string strMachineID = ini.IniReadValue("LocalMachine", "MachineID");

            IPAddress address = IPAddress.Parse(strHost);
            // IPAddress address = IPAddress.Parse("fe80::c4c2:9114:91e8:fbad%12");
            Socket socket = null;
            if (address.AddressFamily == AddressFamily.InterNetwork)
            {
                //UMPService00.IEventLog.WriteEntry("IPV4", EventLogEntryType.Warning);
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            }
            else if (address.AddressFamily == AddressFamily.InterNetworkV6)
            {
                // UMPService00.IEventLog.WriteEntry("IPV6", EventLogEntryType.Warning);
                socket = new Socket(AddressFamily.InterNetworkV6, SocketType.Dgram, ProtocolType.Udp);
            }
            // UMPService00.IEventLog.WriteEntry("strHost = " + strHost + "\r\niPort = " + iPort.ToString(), EventLogEntryType.Warning);
            socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(address));
            //SocketOptionName.MulticastTimeToLive  IP多路广播生存时间。
            socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, 2);
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            IPEndPoint ipep = new IPEndPoint(address, iPort);
            socket.Connect(ipep);
            SendDgramMsg(strMessage, strMachineID, socket);
            return true;
        }

        private static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.RemoteCertificateChainErrors || sslPolicyErrors == SslPolicyErrors.None) { return true; }
            return false;
        }

        private static void SendDgramMsg(string strFilePath, string strMachineID, Socket socket)
        {
            //数据包
            _TAG_NETPACK_DISTINGUISHHEAD_VER1 pack = new _TAG_NETPACK_DISTINGUISHHEAD_VER1();
            //基本包头
            NETPACK_BASEHEAD_APPLICATION_VER1 baseHeadPack = new NETPACK_BASEHEAD_APPLICATION_VER1();
            baseHeadPack._encrypt = DecDefine.NETPACK_ENCRYPT_NOTHING;
            baseHeadPack._format = DecDefine.NETPACK_BASEHEAD_VER1_FORMAT_JSON;
            JsonObject json = new JsonObject();
            json["MachineID"] = new JsonProperty("\"" + strMachineID + "\"");
            json["ChangeID"] = new JsonProperty("\"" + System.DateTime.Now.Year.ToString() + string.Format("{0:00}", System.DateTime.Now.Month) + string.Format("{0:00}", System.DateTime.Now.Day)
               + string.Format("{0:00}", System.DateTime.Now.Hour) + string.Format("{0:00}", System.DateTime.Now.Minute) + string.Format("{0:00}", System.DateTime.Now.Second)
               + string.Format("{0:000}", System.DateTime.Now.Millisecond) + "\"");
            JsonObject jsonParam = new JsonObject();
            jsonParam["ParamChange"] = new JsonProperty(json);
            baseHeadPack._validsize = (short)jsonParam.ToString().Length;
            baseHeadPack._datasize = (short)jsonParam.ToString().Length;
            byte[] byteData = Encoding.ASCII.GetBytes(jsonParam.ToString());
            // string str = Encoding.ASCII.GetString(byteData);
            //  UMPService00.IEventLog.WriteEntry("send message = "+str, EventLogEntryType.Warning);
            _TAG_NETPACK_ENCRYPT_CONTEXT encrypt = new _TAG_NETPACK_ENCRYPT_CONTEXT();
            encrypt._encrypt = DecDefine.NETPACK_ENCRYPT_NOTHING;
            //识别头
            _TAG_NETPACK_DISTINGUISHHEAD disHead = new _TAG_NETPACK_DISTINGUISHHEAD();
            byte[] byteHeadPack = Common.StructToBytes(baseHeadPack);
            byte[] byteFollowData = new byte[byteHeadPack.Length + byteData.Length];
            Array.Copy(byteHeadPack, byteFollowData, byteHeadPack.Length);
            Array.Copy(byteData, 0, byteFollowData, byteHeadPack.Length, byteData.Length);
            _TAG_NETPACK_MESSAGE message = Common.CreateMessage(0xffff, 0xffff, 0xffff, 0xffff, DecDefine.NETMSG_SMALLTYPE_COMMON_PARAM_NOTIFY,
                DecDefine.NETMSG_MIDTYPE_COMMON_PARAM, DecDefine.NETMSG_LARGTYPE_COMMON, DecDefine.NETMSG_NUMBER_COMMON_PARAM_NOTIFY_CHANGE);
            _TAG_NETPACK_DISTINGUISHHEAD_VER1 ReleasePack = Common.CreatePack(DecDefine.NETPACK_DISTHEAD_VER1, disHead, DecDefine.NETPACK_BASETYPE_APPLICATION_VER1,
              DecDefine.NETPACK_EXTTYPE_NOTHING, encrypt, (ushort)byteData.Length, message, 0xffff, 0xffff, DecDefine.NETPACK_PACKTYPE_APPLICATION, 0, 0, 0, -1);
            bool bIsSend = false;
            //UMPService00.IEventLog.WriteEntry("_TAG_NETPACK_DISTINGUISHHEAD_VER1 size = " + Marshal.SizeOf(typeof(_TAG_NETPACK_DISTINGUISHHEAD_VER1))
            //    +"mes length = "+byteFollowData.Length.ToString(), EventLogEntryType.Warning);

            for (int i = 0; i <= 3; i++)
            {
                if (bIsSend)
                {
                    break;
                }
                try
                {
                    SendMsg(ReleasePack, byteFollowData, socket);
                    bIsSend = true;
                    socket.Close();
                }
                catch 
                {
                    bIsSend = false;
                    Thread.Sleep(2 * 1000);
                }
            }
        }

        public static void SendMsg(_TAG_NETPACK_DISTINGUISHHEAD_VER1 netpackHead, byte[] byteFollowData, Socket socket)
        {
            byte[] byteNetpackHead = Common.StructToBytes(netpackHead);
            int iFollowDataLength = 0;
            if (byteFollowData != null)
            {
                iFollowDataLength = byteFollowData.Length;
            }
            byte[] byteSend = new byte[iFollowDataLength + byteNetpackHead.Length];
            byteNetpackHead.CopyTo(byteSend, 0);
            if (byteFollowData != null)
            {
                byteFollowData.CopyTo(byteSend, byteNetpackHead.Length);
            }
            if (socket.Connected)
            {
                socket.Send(byteSend);
            }
            else
            {
                //UMPService00.IEventLog.WriteEntry("Socket connection closed,send message error", EventLogEntryType.Warning);
                UMPService00.WriteLog(LogMode.Error, "Socket connection closed,send message error");
            }
        }
    }
}
