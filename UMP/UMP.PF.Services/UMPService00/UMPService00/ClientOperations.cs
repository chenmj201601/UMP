using PFShareClassesS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.ResourceXmls;

namespace UMPService00
{
    public class ClientOperations
    {
        private TcpClient ITcpClient = null;

        private bool IBoolCanSendMessage = true;
        private bool IBoolInSendingMessage = false;
        private bool IBoolCanRecive = true;
        private string IStrSpliterChar = string.Empty;
        public SslStream ISslStream = null;

        private static readonly object ILockObject = new object();

        public ClientOperations(TcpClient ATcpClient)
        {
            ITcpClient = ATcpClient;
            IStrSpliterChar = Common.AscCodeToChr(27);
        }

        public void ClientMessageOperation()
        {
            string LStrReadedData = string.Empty;
            string LStrEntryBody = string.Empty;

            try
            {
                LStrEntryBody = "A Client Connected\n";
                LStrEntryBody += "IPAddress : " + ((IPEndPoint)ITcpClient.Client.RemoteEndPoint).Address.ToString();
                LStrEntryBody += " Port : " + ((IPEndPoint)ITcpClient.Client.RemoteEndPoint).Port.ToString() + "\n";
                UMPService00.WriteLog(LogMode.Info, LStrEntryBody);

                //ISslStream = new SslStream(ITcpClient.GetStream(), false);
                //ISslStream.AuthenticateAsServer(UMPService00.IX509CertificateServer, false, System.Security.Authentication.SslProtocols.Default, false);
                //ISslStream.ReadTimeout = 2000;
                //ISslStream.WriteTimeout = 2000;
                ISslStream = new SslStream(ITcpClient.GetStream(), false, (s, cert, chain, err) => true);
                ISslStream.AuthenticateAsServer(UMPService00.IX509CertificateServer);
                while (IBoolCanRecive)
                {
                    try
                    {
                        LStrReadedData = ReadMessageFromClient();
                        if (string.IsNullOrEmpty(LStrReadedData)) { continue; }
                        DealClientMessage(LStrReadedData);
                    }
                    catch (Exception ex)
                    {
                        IBoolCanRecive = false;

                        UMPService00.WriteLog(LogMode.Error, "ISslStream.Read()\n" + ex.ToString());
                    }
                }
                while (IBoolInSendingMessage) { Thread.Sleep(50); }
                IBoolCanSendMessage = false;
            }
            catch (Exception ex)
            {
                IBoolCanSendMessage = false;
                IBoolCanRecive = false;

                UMPService00.WriteLog(LogMode.Error, "ClientMessageOperation()\n" + ex.ToString());

            }
        }

        public void StopThisClientThread()
        {
            try
            {

                UMPService00.WriteLog(LogMode.Info, "StopThisClientThread() Client : " + ITcpClient.Client.RemoteEndPoint.ToString());
                SendMessageToClient("Result=StopService");
                IBoolCanSendMessage = false;
                IBoolCanRecive = false;
                while (IBoolCanSendMessage) { Thread.Sleep(50); }
                if (ISslStream != null) { ISslStream.Close(); }
                if (ITcpClient != null) { ITcpClient.Close(); }
            }
            catch (Exception ex)
            {
                UMPService00.WriteLog(LogMode.Error, "StopThisClientThread()\n" + ex.Message);

            }
        }

        private void SendMessageToClient(string AStrMessage)
        {
            string LStrSendMessage = string.Empty;

            try
            {
                if (!IBoolCanSendMessage) { return; }
                IBoolInSendingMessage = true;
                LStrSendMessage = AStrMessage;
                if (string.IsNullOrEmpty(LStrSendMessage)) { LStrSendMessage = " "; }
                LStrSendMessage += "\r\n";
                byte[] LByteMessage = Encoding.UTF8.GetBytes(LStrSendMessage);
                ISslStream.Write(LByteMessage);
                ISslStream.Flush();
            }
            catch (Exception ex)
            {

                UMPService00.WriteLog(LogMode.Error, "SendMessageToClient()\n" + ex.ToString());

            }
            finally
            {
                IBoolInSendingMessage = false;
            }
        }

        private string ReadMessageFromClient()
        {
            string LStrReadedData = string.Empty;

            try
            {
                StringBuilder LStringBuilderData = new StringBuilder();
                int LIntReadedBytes = -1, LIntEndKeyPosition;
                byte[] LByteReadeBuffer = new byte[1024];

                do
                {
                    LIntReadedBytes = ISslStream.Read(LByteReadeBuffer, 0, LByteReadeBuffer.Length);
                    Decoder LDecoder = Encoding.UTF8.GetDecoder();
                    char[] LChars = new char[LDecoder.GetCharCount(LByteReadeBuffer, 0, LIntReadedBytes)];
                    LDecoder.GetChars(LByteReadeBuffer, 0, LIntReadedBytes, LChars, 0);
                    LStringBuilderData.Append(LChars);
                    if (LStringBuilderData.ToString().IndexOf("\r\n") > 0) { break; }
                }
                while (LIntReadedBytes != 0);
                LStrReadedData = LStringBuilderData.ToString();
                LIntEndKeyPosition = LStrReadedData.IndexOf("\r\n");
                if (LIntEndKeyPosition > 0)
                {
                    LStrReadedData = LStrReadedData.Substring(0, LIntEndKeyPosition);

                    UMPService00.WriteLog(LogMode.Info, "ReadMessageFromClient() " + LStrReadedData + " (" + ITcpClient.Client.RemoteEndPoint.ToString() + ")");

                }
                IBoolCanRecive = false;
            }
            catch (Exception ex)
            {
                IBoolCanRecive = false;
                LStrReadedData = string.Empty;

                UMPService00.WriteLog(LogMode.Error, "ReadMessageFromClient()\n" + ex.ToString() + "\n" + ITcpClient.Client.RemoteEndPoint.ToString());
            }

            return LStrReadedData;
        }

        private void DealClientMessage(string AStrMessage)
        {
            string LStrVerificationCode104 = string.Empty;
            string LStrVerificationCode004 = string.Empty;
            string LStrClientMethod = string.Empty;
            string LStrSendMessage = string.Empty;

            try
            {
                string[] LStrUserSendData = AStrMessage.Split(IStrSpliterChar.ToArray());

                if (LStrUserSendData.Length <= 0) { return; }
                LStrVerificationCode004 = Common.CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004);
                LStrVerificationCode104 = Common.CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104);
                LStrClientMethod = EncryptionAndDecryption.EncryptDecryptString(LStrUserSendData[0], LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                if (LStrClientMethod.Length != 4)
                {

                    UMPService00.WriteLog(LogMode.Warn, "DealClientMessage() Error Command : " + LStrClientMethod + "\n" + ITcpClient.Client.RemoteEndPoint.ToString());
                    LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString("ErrorE001", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    SendMessageToClient(LStrSendMessage);             //错误的指令
                    SendMessageToClient(Common.AscCodeToChr(27) + "End" + Common.AscCodeToChr(27));
                    return;
                }

                UMPService00.WriteLog(LogMode.Info, string.Format("DealClientMessage() Receive Command : {0}" + "\n" + ITcpClient.Client.RemoteEndPoint.ToString(), LStrClientMethod));
                int LIntDataLength = LStrUserSendData.Length;
                for (int LIntDataLoop = 0; LIntDataLoop < LIntDataLength; LIntDataLoop++)
                {
                    LStrUserSendData[LIntDataLoop] = EncryptionAndDecryption.EncryptDecryptString(LStrUserSendData[LIntDataLoop], LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                }
                switch (LStrClientMethod)
                {
                    case "G001":            //获取机器名
                        ClientMessageG001(LStrUserSendData);
                        break;
                    case "G002":            //获取磁盘信息
                        ClientMessageG002(LStrUserSendData);
                        break;
                    case "G003":            //获取网卡信息
                        ClientMessageG003(LStrUserSendData);
                        break;
                    case "G004":            //获得路径下级所以文件夹
                        ClientMessageG004(LStrUserSendData);
                        break;
                    case "R001":        //wcf11101发送的 用于通知更新参数
                        ClientMessageR001(LStrUserSendData);
                        break;
                    case "G005":    //Service00之间使用
                        ClientMessageG005(LStrUserSendData, ITcpClient.Client.RemoteEndPoint.ToString());
                        break;
                    case "G006":    //通过ServiceName.exe获得机器名
                        ClientMessageG006(LStrUserSendData);
                        break;
                    case "G007":    //获得文件夹下的文件
                        ClientMessageG007(LStrUserSendData);
                        break;
                    case "G008":    //获得License信息（同时在线用户数、许可期限、UMP功能列表、UMP版本名称）
                        ClientMessageG008(LStrUserSendData);
                        break;
                    case "G009":    //获得所有的license（license在 1100100 ~ 1100199 的值全部返回）
                        ClientMessageG009(LStrUserSendData);
                        break;
                    case "G010":
                        ClientMessageG010(LStrUserSendData);
                        break;
                    case "C001":    //检查LicenseServer是否可用
                        ClientMessageC001(LStrUserSendData);
                        break;
                    case "N001":        //启用备机接替主机 用于UMP服务器
                        ClientMessageN001(LStrUserSendData);
                        break;
                    case "N002":        //启用备机接替主机 用于Voice服务器
                        ClientMessageN002(LStrUserSendData);
                        break;
                    case "N003":
                        ClientMessageN003(LStrUserSendData);
                        break;
                    case "N004":    //更新主备机状态
                        ClientMessageN004(LStrUserSendData);
                        break;
                    case "U001":        //升级程序通知本Logging服务器准备进行升级
                        ClientMessageU001(LStrUserSendData);
                        break;
                    default:
                        LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString("ErrorE002", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                        SendMessageToClient(LStrSendMessage);             //系统不能识别的指令
                        SendMessageToClient(Common.AscCodeToChr(27) + "End" + Common.AscCodeToChr(27));
                        IBoolCanRecive = false;
                        break;
                }
            }
            catch (Exception ex)
            {
                LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString("ErrorE999", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                SendMessageToClient(LStrSendMessage);
                SendMessageToClient(Common.AscCodeToChr(27) + "End" + Common.AscCodeToChr(27));

                UMPService00.WriteLog(LogMode.Error, "DealClientMessage()\n" + ex.ToString() + "\n" + ITcpClient.Client.RemoteEndPoint.ToString());
            }
        }


        /// <summary>
        /// 获取机器名
        /// </summary>
        /// <param name="AStrArrayInfo">无</param>
        private void ClientMessageG001(string[] AStrArrayInfo)
        {
            string LStrSendReturn = string.Empty;

            try
            {
                LStrSendReturn = System.Environment.MachineName;
                SendMessageToClient(LStrSendReturn);
                SendMessageToClient(Common.AscCodeToChr(27) + "End" + Common.AscCodeToChr(27));
            }
            catch (Exception ex)
            {
                SendMessageToClient("ErrorG001");
                SendMessageToClient(Common.AscCodeToChr(27) + "End" + Common.AscCodeToChr(27));
                UMPService00.WriteLog(LogMode.Error, "ClientMessageG001()\n" + ex.ToString() + "\n" + ITcpClient.Client.RemoteEndPoint.ToString());
            }
        }

        /// <summary>
        /// 获取磁盘信息
        /// </summary>
        /// <param name="AStrArrayInfo">
        /// [1]: 是否需要获取系统盘符  0:不需要 1：需要
        /// </param>
        private void ClientMessageG002(string[] AStrArrayInfo)
        {
            try
            {
                int iGetSys = 0;
                int.TryParse(AStrArrayInfo[1], out iGetSys);
                ManagementObjectCollection ObjectCollection;
                ObjectCollection = GetServiceCollection("SELECT Name, VolumeName, FreeSpace, Size, FileSystem, DriveType FROM Win32_LogicalDisk where DriveType=3");
                string strSend = string.Empty;
                foreach (ManagementObject ObjectSingleReturn in ObjectCollection)
                {
                    if (ObjectSingleReturn == null) { continue; }
                    //如果不需要获取系统盘符
                    if (ObjectSingleReturn["Name"].ToString().Equals(Environment.SystemDirectory.Substring(0, Environment.SystemDirectory.IndexOf(':') + 1)))
                    {
                        if (iGetSys == 0)
                        {
                            continue;
                        }
                        else if (iGetSys == 1)
                        {
                            strSend = ObjectSingleReturn["Name"].ToString() + Common.AscCodeToChr(27);
                            strSend += ObjectSingleReturn["VolumeName"].ToString() + Common.AscCodeToChr(27);
                            strSend += ObjectSingleReturn["FreeSpace"].ToString() + Common.AscCodeToChr(27);
                            strSend += ObjectSingleReturn["Size"].ToString() + Common.AscCodeToChr(27);
                            strSend += ObjectSingleReturn["FileSystem"].ToString() + Common.AscCodeToChr(27);
                            strSend += ObjectSingleReturn["DriveType"].ToString();
                            SendMessageToClient(strSend);
                            continue;
                        }
                    }
                    strSend = ObjectSingleReturn["Name"].ToString() + Common.AscCodeToChr(27);
                    strSend += ObjectSingleReturn["VolumeName"].ToString() + Common.AscCodeToChr(27);
                    strSend += ObjectSingleReturn["FreeSpace"].ToString() + Common.AscCodeToChr(27);
                    strSend += ObjectSingleReturn["Size"].ToString() + Common.AscCodeToChr(27);
                    strSend += ObjectSingleReturn["FileSystem"].ToString() + Common.AscCodeToChr(27);
                    strSend += ObjectSingleReturn["DriveType"].ToString();
                    SendMessageToClient(strSend);
                }
                SendMessageToClient(Common.AscCodeToChr(27) + "End" + Common.AscCodeToChr(27));
            }
            catch (Exception ex)
            {
                SendMessageToClient("ErrorG002");
                SendMessageToClient(Common.AscCodeToChr(27) + "End" + Common.AscCodeToChr(27));

                UMPService00.WriteLog(LogMode.Error, "ClientMessageG002()\n" + ex.ToString() + "\n" + ITcpClient.Client.RemoteEndPoint.ToString());
            }
        }

        private void ClientMessageG003(string[] AStrArrayInfo)
        {
            try
            {
                ManagementObjectCollection ObjectCollection;
                ObjectCollection = GetServiceCollection("SELECT * FROM Win32_NetworkAdapterConfiguration WHERE (MACAddress IS NOT NULL) ");
                foreach (ManagementObject ObjectSingleReturn in ObjectCollection)
                {
                    try
                    {
                        SendMessageToClient(ObjectSingleReturn["Description"].ToString() + Common.AscCodeToChr(27) + ObjectSingleReturn["SettingID"].ToString());
                    }
                    catch { }
                }
                SendMessageToClient(Common.AscCodeToChr(27) + "End" + Common.AscCodeToChr(27));
            }
            catch (Exception ex)
            {
                SendMessageToClient("ErrorG003");
                SendMessageToClient(Common.AscCodeToChr(27) + "End" + Common.AscCodeToChr(27));

                UMPService00.WriteLog(LogMode.Error, "ClientMessageG003()\n" + ex.ToString() + "\n" + ITcpClient.Client.RemoteEndPoint.ToString());
            }
        }

        /// <summary>
        /// 获得路径下级所以文件夹
        /// </summary>
        /// <param name="AStrArrayInfo">
        /// [1]:路径
        /// </param>
        private void ClientMessageG004(string[] AStrArrayInfo)
        {
            try
            {
                if (AStrArrayInfo.Length >= 2)
                {
                    string strDirPath = AStrArrayInfo[1];
                    DirectoryInfo dir = new DirectoryInfo(strDirPath);
                    List<DirectoryInfo> lstDirs = dir.GetDirectories().ToList();
                    if (lstDirs.Count > 0)
                    {
                        for (int i = 0; i < lstDirs.Count; i++)
                        {
                            if (lstDirs[i].FullName.Contains('$'))
                            {
                                continue;
                            }
                            SendMessageToClient(lstDirs[i].Name + Common.AscCodeToChr(27) + lstDirs[i].FullName);
                        }
                    }
                }
                SendMessageToClient(Common.AscCodeToChr(27) + "End" + Common.AscCodeToChr(27));
            }
            catch (Exception ex)
            {
                SendMessageToClient("ErrorG004");
                SendMessageToClient(Common.AscCodeToChr(27) + "End" + Common.AscCodeToChr(27));

                UMPService00.WriteLog(LogMode.Error, "ClientMessageG004()\n" + ex.ToString() + "\n" + ITcpClient.Client.RemoteEndPoint.ToString());
            }
        }

        /// <summary>
        /// 发送需要更新参数的命令给每个服务器上的service00 
        /// 参数个数无限制，是需要更新参数服务器IP列表
        /// 发送G005和数据库服务器的信息 
        /// G005
        /// TypeID
        /// Host
        /// Port
        /// LoginName
        /// LoginPassword
        /// DBName
        /// </summary>
        /// <param name="AStrArrayInfo"></param>
        private void ClientMessageR001(string[] AStrArrayInfo)
        {
            try
            {
                SendMessageToClient("OK");
                SendMessageToClient(Common.AscCodeToChr(27) + "End" + Common.AscCodeToChr(27));

                if (AStrArrayInfo.Count() > 0)
                {
                    string strDbInfo = string.Empty;
                    string strPDBFileath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                    strDbInfo = DataBaseXmlOperator.GetDBInfo(strPDBFileath);

                    //如果数据库信息不包括char27
                    if (!strDbInfo.Contains(Common.AscCodeToChr(27)))
                    {
                        SendMessageToClient("Get database info error");
                        SendMessageToClient(Common.AscCodeToChr(27) + "End" + Common.AscCodeToChr(27));

                        return;
                    }

                    List<string> lstServers = AStrArrayInfo.ToList();
                    //在本机生成参数XML 如果发过来的参数不包含本机 则加入
                    string strLocalServerPort = ITcpClient.Client.LocalEndPoint.ToString();
                    if (!lstServers.Contains(strLocalServerPort))
                    {
                        lstServers.Add(strLocalServerPort);
                    }

                    string strHost = string.Empty;
                    int iPort = 0;
                    int iIISPort = 0;
                    // SendMessageToClient(Common.GetCurrentBaseDirectory().Trim('\\'));
                    GloableParamOperation gloable = new GloableParamOperation(Common.GetCurrentBaseDirectory().Trim('\\'));

                    bool bGetWebPortResult = gloable.GetWebSitePort(ref iIISPort);
                    if (!bGetWebPortResult)
                    {
                        SendMessageToClient("Get website port error");
                        SendMessageToClient(Common.AscCodeToChr(27) + "End" + Common.AscCodeToChr(27));
                        return;
                    }


                    List<string> lstAuthenServerInfo = new List<string>();
                    bool IsGetAuthenServer = gloable.GetAuthenticateServerInfo(ref lstAuthenServerInfo);
                    if (!IsGetAuthenServer)
                    {

                        UMPService00.WriteLog(LogMode.Error, "Get Authenticate Server Info error");
                    }
                    string LStrVerificationCode004 = Common.CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    string strAuthenticateServerHost = EncryptionAndDecryption.EncryptDecryptString(lstAuthenServerInfo[0], LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    string strAuthenticateServerPort = EncryptionAndDecryption.EncryptDecryptString(lstAuthenServerInfo[1], LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);

                    string strSendMessage = string.Empty;
                    List<string> lstArgs = null;
                    BackgroundWorker bgSendToVoice = null;
                    foreach (string strServerInfo in lstServers)
                    {
                        if (strServerInfo.Contains(':'))
                        {
                            strHost = strServerInfo.Substring(0, strServerInfo.IndexOf(':'));
                            iPort = int.Parse(strServerInfo.Substring(strServerInfo.IndexOf(':') + 1));
                            strSendMessage = EncryptionAndDecryption.EncryptDecryptString("G005", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                            strSendMessage += Common.AscCodeToChr(27) + strDbInfo + Common.AscCodeToChr(27) + iIISPort + Common.AscCodeToChr(27)
                                + strAuthenticateServerHost + Common.AscCodeToChr(27) + strAuthenticateServerPort;
                            lstArgs = new List<string>();
                            lstArgs.Add(strHost);
                            lstArgs.Add(iPort.ToString());
                            lstArgs.Add(strSendMessage);
                            bgSendToVoice = new BackgroundWorker();
                            bgSendToVoice.DoWork += bgSendToVoice_DoWork;
                            bgSendToVoice.RunWorkerCompleted += bgSendToVoice_RunWorkerCompleted;
                            bgSendToVoice.RunWorkerAsync(lstArgs);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SendMessageToClient("Error:" + ex.Message);
                SendMessageToClient(Common.AscCodeToChr(27) + "End" + Common.AscCodeToChr(27));
            }
        }

        void bgSendToVoice_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            BackgroundWorker bg = sender as BackgroundWorker;
            bg.Dispose();
        }

        void bgSendToVoice_DoWork(object sender, DoWorkEventArgs e)
        {
            List<string> lstArgs = e.Argument as List<string>;
            string strHost = lstArgs[0];
            int iPort = int.Parse(lstArgs[1]);
            string strSendMessage = lstArgs[2];
            SendMessageToService00(strHost, iPort, strSendMessage);
        }

        /// <summary>
        ///G005
        ///TypeID
        ///TypeName
        ///Host
        ///Port
        ///LoginName
        ///Password
        ///DBName
        ///iIISPort
        ///认证服务器IP
        ///认证服务器端口
        ///是备机（仅N004命令传过来的命令带此参数，用于重启voice服务） 是：1  否：0
        /// </summary>
        /// <param name="AStrArrayInfo"></param>
        private void ClientMessageG005(string[] AStrArrayInfo, string strEndPoint)
        {

            SendMessageToClient("OK");
            SendMessageToClient(Common.AscCodeToChr(27) + "End" + Common.AscCodeToChr(27));
            if (AStrArrayInfo.Count() <= 0)
            {

                UMPService00.WriteLog("Command error");

                return;
            }

            try
            {

                UMPService00.WriteLog("AStrArrayInfo count = " + AStrArrayInfo.Count());
                string strHost = strEndPoint.Substring(0, strEndPoint.IndexOf(':'));

                AppServerInfo serverInfo = new AppServerInfo();
                serverInfo.Protocol = "http";
                serverInfo.Address = strHost;
                serverInfo.Port = int.Parse(AStrArrayInfo[8]);
                serverInfo.SupportHttps = false;

                UMPService00.WriteLog(LogMode.Warn, "Address:" + serverInfo.Address + "   Port: " + serverInfo.Port);
                string LStrVerificationCode = Common.CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104);
                DatabaseInfo dbInfo = new DatabaseInfo();
                dbInfo.TypeID = int.Parse(EncryptionAndDecryption.EncryptDecryptString(AStrArrayInfo[1], LStrVerificationCode, EncryptionAndDecryption.UMPKeyAndIVType.M104));
                dbInfo.TypeName = EncryptionAndDecryption.EncryptDecryptString(AStrArrayInfo[2], LStrVerificationCode, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                dbInfo.Host = EncryptionAndDecryption.EncryptDecryptString(AStrArrayInfo[3], LStrVerificationCode, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                dbInfo.Port = int.Parse(EncryptionAndDecryption.EncryptDecryptString(AStrArrayInfo[4], LStrVerificationCode, EncryptionAndDecryption.UMPKeyAndIVType.M104));
                dbInfo.LoginName = EncryptionAndDecryption.EncryptDecryptString(AStrArrayInfo[5], LStrVerificationCode, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                dbInfo.Password = EncryptionAndDecryption.EncryptDecryptString(AStrArrayInfo[6], LStrVerificationCode, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                dbInfo.DBName = EncryptionAndDecryption.EncryptDecryptString(AStrArrayInfo[7], LStrVerificationCode, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                string strAuthenServerHost = EncryptionAndDecryption.EncryptDecryptString(AStrArrayInfo[9], LStrVerificationCode, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                string strAuthenServerPort = EncryptionAndDecryption.EncryptDecryptString(AStrArrayInfo[10], LStrVerificationCode, EncryptionAndDecryption.UMPKeyAndIVType.M104);

                //读取备机代替的主机信息
                //string strReplaceMdouleNumber = ParamXmlOperation.GetBackupMachineInfo();
                string strPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\Temp";
                int iResult = 0;
                bool bIsParamsExists = false;
                string strUmpServer = ITcpClient.Client.RemoteEndPoint.ToString().Substring(0, ITcpClient.Client.RemoteEndPoint.ToString().IndexOf(':'));
                string strLocalServer = ITcpClient.Client.LocalEndPoint.ToString().Substring(0, ITcpClient.Client.LocalEndPoint.ToString().IndexOf(':'));
                bool bIsUMPServer = false;
                if (strUmpServer == strLocalServer)
                {
                    bIsUMPServer = true;
                }
                else
                {
                    bIsUMPServer = false;
                }
                try
                {
                    GenerateXml generate = new GenerateXml(strPath, dbInfo, serverInfo, strAuthenServerHost, strAuthenServerPort);
                    iResult = generate.Generate(bIsUMPServer, ref bIsParamsExists);
                    generate.ClenData();
                }
                catch (Exception ex)
                {

                    UMPService00.WriteLog(LogMode.Error, "Geneerate xml failed ,ErrorInfo : " + ex.Message);
                }
                ResourceXmlHelper hel = new ResourceXmlHelper();
                hel.CleanData();

                if (iResult == 0)
                {

                    UMPService00.WriteLog(LogMode.Error, "Replace param xml success");
                    //创建socket 开始发送广播
                    string strSimpFilePath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\VoiceCyber\\UMP\\config" + "\\umpparam_simp.xml";
                    SendBroadcastData.WriteLogPath(strSimpFilePath);
                    SendBroadcastData.SendBroadcastMessage(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\VoiceCyber\\UMP\\config");

                    //在Voice服务器生成数据库连接的xml
                    // 如果不是UMP服务器 则生成数据库连接的xml
                    if (!bIsUMPServer)
                    {

                        string strDBAndWebParamInVoiceServerPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\VoiceServer";
                        bool bWriteDBInfo = DataBaseXmlOperator.WriteDBInfoInVoiceServer(dbInfo, strDBAndWebParamInVoiceServerPath);
                        if (!bWriteDBInfo)
                        {

                            UMPService00.WriteLog(LogMode.Warn, "Write database info error");
                        }
                        else
                        {

                            UMPService00.WriteLog(LogMode.Info, "Write database info success");
                        }
                        GloableParamOperation gloParam = new GloableParamOperation(strDBAndWebParamInVoiceServerPath);
                        bool bIsWriteWebSiteInfo = gloParam.WriteWebSiteInfoOnVoiceServer(serverInfo.Address, serverInfo.Port);
                        if (!bIsWriteWebSiteInfo)
                        {

                            UMPService00.WriteLog(LogMode.Warn, "Write website info error");
                        }
                        else
                        {

                            UMPService00.WriteLog(LogMode.Info, "Write website info success");

                        }
                    }

                    //如果在生成参数文件前 没有文件 当作第一次生成参数文件处理
                    if (!bIsParamsExists)
                    {
                        RestartService(AppDomain.CurrentDomain.BaseDirectory + "\\RestartService.bat");
                    }
                    else if (AStrArrayInfo.Count() == 12 && AStrArrayInfo[11] == "1")
                    {

                        UMPService00.WriteLog("bIsParamsExists is true,and restart voice service");
                        RestartService(AppDomain.CurrentDomain.BaseDirectory + "\\RestartVoiceService.bat");
                    }
                }
                else
                {

                    UMPService00.WriteLog(LogMode.Error, "Generate param xml failed result = " + iResult);
                }
            }
            catch (Exception ex)
            {

                UMPService00.WriteLog(LogMode.Error, "ClientMessageG005 error:" + ex.Message);
            }
        }

        /// <summary>
        /// 通过ServiceName.exe获得机器名
        /// </summary>
        /// <param name="AStrArrayInfo">IP 调用serviceName.exe需要传入IP</param>
        private void ClientMessageG006(string[] AStrArrayInfo)
        {
            try
            {
                if (AStrArrayInfo.Count() <= 1)
                {
                    SendMessageToClient("Param Error");
                    SendMessageToClient(Common.AscCodeToChr(27) + "End" + Common.AscCodeToChr(27));

                    UMPService00.WriteLog(LogMode.Error, "Param Error");
                    return;
                }
                string strServerHost = AStrArrayInfo[1];
                //生成配置文件TSLIB.INI
                string strIniFilePath = Environment.GetFolderPath(Environment.SpecialFolder.Windows) + "\\TSLIB.INI";
                FileInfo file = new FileInfo(strIniFilePath);
                IniOperation iniOperation = null;
                if (!file.Exists)
                {
                    FileStream fs = file.Create();
                    fs.Close();
                }

                iniOperation = new IniOperation(strIniFilePath);
                iniOperation.IniWriteValue("Telephony Servers", strServerHost, "450");

                //C:\windows
                string strExeFileDir = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
                Process myProcess = new Process();
                string strExtFilePath = strExeFileDir + "\\ServerNames.exe";
                ProcessStartInfo myProcessStartInfo = new ProcessStartInfo(strExtFilePath);
                myProcess.StartInfo = myProcessStartInfo;
                myProcess.StartInfo.UseShellExecute = false;
                myProcess.StartInfo.RedirectStandardInput = true;
                myProcess.StartInfo.RedirectStandardOutput = true;
                myProcess.StartInfo.RedirectStandardError = true;
                myProcess.Start();
                while (!myProcess.HasExited)
                {
                    myProcess.WaitForExit();
                }
                int returnValue = myProcess.ExitCode;
                string strServerCount = iniOperation.IniReadValue("ServerNames", "num");
                int iServerCount = 0;
                int.TryParse(strServerCount, out iServerCount);
                if (iServerCount > 0)
                {
                    string strServerNameValue;
                    for (int i = 0; i < iServerCount; i++)
                    {
                        strServerNameValue = iniOperation.IniReadValue("ServerNames", "Server" + i);
                        SendMessageToClient(strServerNameValue);
                    }
                }
                else
                {
                    SendMessageToClient("Error01");
                }
                SendMessageToClient(Common.AscCodeToChr(27) + "End" + Common.AscCodeToChr(27));
                if (iServerCount > 0)
                {

                    UMPService00.WriteLog(LogMode.Info, "Get server name success");
                }
                else
                {

                    UMPService00.WriteLog(LogMode.Error, "Get server name failed");
                }
            }
            catch (Exception ex)
            {
                SendMessageToClient("ErrorG006");
                SendMessageToClient(Common.AscCodeToChr(27) + "End" + Common.AscCodeToChr(27));

                UMPService00.WriteLog(LogMode.Error, "ClientMessageG007()\n" + ex.ToString() + "\n" + ITcpClient.Client.RemoteEndPoint.ToString());
            }
        }

        /// <summary>
        /// 获得文件夹下的文件
        /// </summary>
        /// <param name="AStrArrayInfo">
        /// [0]: G007
        /// [1]: 文件夹路径
        /// </param>
        /// Error001:传入参数出错
        private void ClientMessageG007(string[] AStrArrayInfo)
        {
            try
            {
                if (AStrArrayInfo.Length >= 2)
                {
                    string strDirPath = AStrArrayInfo[1];
                    DirectoryInfo dir = new DirectoryInfo(strDirPath);
                    List<FileInfo> lstFiles = dir.GetFiles().ToList();
                    if (lstFiles.Count > 0)
                    {
                        foreach (FileInfo file in lstFiles)
                        {
                            SendMessageToClient(file.Name + Common.AscCodeToChr(27) + file.FullName);
                        }
                    }
                }
                else
                {
                    SendMessageToClient("Error001");

                    UMPService00.WriteLog(LogMode.Error, "Param Error");
                }
                SendMessageToClient(Common.AscCodeToChr(27) + "End" + Common.AscCodeToChr(27));
            }
            catch (Exception ex)
            {
                SendMessageToClient("ErrorG007");
                SendMessageToClient(Common.AscCodeToChr(27) + "End" + Common.AscCodeToChr(27));

                UMPService00.WriteLog(LogMode.Error, "ClientMessageG007()\n" + ex.ToString() + "\n" + ITcpClient.Client.RemoteEndPoint.ToString());
            }
        }

        /// <summary>
        /// 获得License信息（同时在线用户数、许可期限、UMP功能列表、UMP版本名称）
        /// </summary>
        /// <param name="AStrArrayInfo">无</param>
        /// Error001:获取不到Licensever
        /// Error002:连接不上LicenseServer
        /// Error003:Welcome包出错
        /// Error004:License 服务器认证失败
        /// Error005:获得License信息失败
        /// Error006:License服务器证书无效
        private void ClientMessageG008(string[] AStrArrayInfo)
        {
            try
            {

                //先从ProgramData\VoiceCyber\UMP\config下读认证服务器参数
                string strLicXmlPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\VoiceCyber\\UMP\\config\\";
                LicenseServerOperation licOperation = new LicenseServerOperation(strLicXmlPath, "umpparam_simp.xml");
                List<LicenseServer> lstLicenseServers = new List<LicenseServer>();

                //如果读不到认证服务器参数 则从Args02.UMP.xml中读取
                //if (!licOperation.bIsLoadXml)
                //{
                //    //从GloableSetting中读取LicenseServer信息
                //    strLicXmlPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\UMP.Server\\";
                //    licOperation = new LicenseServerOperation(strLicXmlPath, "Args02.UMP.xml");
                //    if (!licOperation.bIsLoadXml)
                //    {
                //        SendMessageToClient("Error003");
                //        SendMessageToClient(Common.AscCodeToChr(27) + "End" + Common.AscCodeToChr(27));
                //        UMPService00.IEventLog.WriteEntry("Can not find the umpparam_simp.xml and Args02.UMP.xml", EventLogEntryType.Error);
                //        return;
                //    }
                //    lstLicenseServers = licOperation.GetLicenseServersOnUMPServer();
                //}
                //else
                //{
                //    lstLicenseServers = licOperation.GetLicenseServerOnVoiceServer();
                //    if (lstLicenseServers.Count <= 0)
                //    {
                //        //从GloableSetting中读取LicenseServer信息
                //        strLicXmlPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\UMP.Server\\";
                //       // UMPService00.IEventLog.WriteEntry("strLicXmlPath = " + strLicXmlPath, EventLogEntryType.Warning);
                //        licOperation = new LicenseServerOperation(strLicXmlPath, "Args02.UMP.xml");
                //        if (!licOperation.bIsLoadXml)
                //        {
                //            SendMessageToClient("Error003");
                //            SendMessageToClient(Common.AscCodeToChr(27) + "End" + Common.AscCodeToChr(27));
                //            UMPService00.IEventLog.WriteEntry("Can not find the umpparam_simp.xml and Args02.UMP.xml", EventLogEntryType.Error);
                //            return;
                //        }
                //        lstLicenseServers = licOperation.GetLicenseServersOnUMPServer();
                //    }
                //}

                if (lstLicenseServers.Count <= 0)
                {
                    //从GloableSetting中读取LicenseServer信息
                    strLicXmlPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\UMP.Server\\";

                    licOperation = new LicenseServerOperation(strLicXmlPath, "Args02.UMP.xml");
                    if (!licOperation.bIsLoadXml)
                    {
                        SendMessageToClient("Error003");
                        SendMessageToClient(Common.AscCodeToChr(27) + "End" + Common.AscCodeToChr(27));

                        UMPService00.WriteLog(LogMode.Error, "Can not find the umpparam_simp.xml and Args02.UMP.xml");
                        return;
                    }
                    lstLicenseServers = licOperation.GetLicenseServersOnUMPServer();
                }


                if (lstLicenseServers.Count <= 0)
                {

                    UMPService00.WriteLog(LogMode.Warn, "No license server  ");
                    SendMessageToClient("Error001");
                    SendMessageToClient(Common.AscCodeToChr(27) + "End" + Common.AscCodeToChr(27));
                    return;
                }
                string strMsg = "lstLicenseServers.Count = " + lstLicenseServers.Count + " ; \r\n";
                for (int i = 0; i < lstLicenseServers.Count; i++)
                {
                    strMsg += "IP = " + lstLicenseServers[i].Host + " ; Port = " + lstLicenseServers[i].Port + " ; \r\n";
                }

                UMPService00.WriteLog(LogMode.Warn, strMsg);


                TcpClient clientToLicServer = null;
                SslStream sslStreamToLicServer = null;
                bool bIsConneted = false;
                int iConnResult = 0;
                foreach (LicenseServer licServer in lstLicenseServers)
                {
                    iConnResult = licOperation.ConnectToLicenseServer(licServer.Host, licServer.Port, 0, ref sslStreamToLicServer, ref clientToLicServer);

                    UMPService00.WriteLog("Conn licserver " + licServer.Host + ":" + licServer.Port + " , result = " + iConnResult.ToString());
                    //如果返回值为0 则跳出循环
                    if (iConnResult == 0)
                    {
                        bIsConneted = true;
                        break;
                    }
                    //如果返回值不为0 则关掉sslstream和client
                    if (sslStreamToLicServer != null)
                    {
                        if (sslStreamToLicServer.CanRead)
                        {
                            sslStreamToLicServer.Close();
                        }
                    }
                    if (clientToLicServer != null)
                    {
                        if (clientToLicServer.Connected)
                        {
                            clientToLicServer.Close();
                        }
                    }
                }
                if (!bIsConneted)
                {
                    SendMessageToClient("Error002");
                    SendMessageToClient(Common.AscCodeToChr(27) + "End" + Common.AscCodeToChr(27));

                    UMPService00.WriteLog(LogMode.Error, "Can not connect to all the license server");
                    return;
                }
                string strResult = licOperation.GetLicenseInfo(sslStreamToLicServer, clientToLicServer, "G008");
                SendMessageToClient(strResult);
                SendMessageToClient(Common.AscCodeToChr(27) + "End" + Common.AscCodeToChr(27));
            }
            catch (Exception ex)
            {
                SendMessageToClient("ErrorG008");
                SendMessageToClient(Common.AscCodeToChr(27) + "End" + Common.AscCodeToChr(27));

                UMPService00.WriteLog(LogMode.Error, "ClientMessageG008()\n" + ex.ToString() + "\n" + ITcpClient.Client.RemoteEndPoint.ToString());
            }
        }

        /// <summary>
        /// 获得所有的license（license在 300000000以下的值全部返回）
        /// </summary>
        /// <param name="AStrArrayInfo"></param>
        private void ClientMessageG009(string[] AStrArrayInfo)
        {
            try
            {
                //先从ProgramData\VoiceCyber\UMP\config下读认证服务器参数
                string strLicXmlPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\VoiceCyber\\UMP\\config\\";
                LicenseServerOperation licOperation = new LicenseServerOperation(strLicXmlPath, "umpparam_simp.xml");
                List<LicenseServer> lstLicenseServers = new List<LicenseServer>();
                //如果读不到认证服务器参数 则从Args02.UMP.xml中读取
                //if (!licOperation.bIsLoadXml)
                //{
                //    //从GloableSetting中读取LicenseServer信息
                //    strLicXmlPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\UMP.Server\\";
                //    licOperation = new LicenseServerOperation(strLicXmlPath, "Args02.UMP.xml");
                //    if (!licOperation.bIsLoadXml)
                //    {
                //        SendMessageToClient("Error003");
                //        SendMessageToClient(Common.AscCodeToChr(27) + "End" + Common.AscCodeToChr(27));
                //        UMPService00.IEventLog.WriteEntry("Can not find the umpparam_simp.xml and Args02.UMP.xml", EventLogEntryType.Error);
                //        return;
                //    }
                //    lstLicenseServers = licOperation.GetLicenseServersOnUMPServer();
                //}
                //else
                //{
                //    lstLicenseServers = licOperation.GetLicenseServerOnVoiceServer();
                //    if (lstLicenseServers.Count <= 0)
                //    {
                //        //从GloableSetting中读取LicenseServer信息
                //        strLicXmlPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\UMP.Server\\";
                //        // UMPService00.IEventLog.WriteEntry("strLicXmlPath = " + strLicXmlPath, EventLogEntryType.Warning);
                //        licOperation = new LicenseServerOperation(strLicXmlPath, "Args02.UMP.xml");
                //        if (!licOperation.bIsLoadXml)
                //        {
                //            SendMessageToClient("Error003");
                //            SendMessageToClient(Common.AscCodeToChr(27) + "End" + Common.AscCodeToChr(27));
                //            UMPService00.IEventLog.WriteEntry("Can not find the umpparam_simp.xml and Args02.UMP.xml", EventLogEntryType.Error);
                //            return;
                //        }
                //        lstLicenseServers = licOperation.GetLicenseServersOnUMPServer();
                //    }
                //}

                if (lstLicenseServers.Count <= 0)
                {
                    //从GloableSetting中读取LicenseServer信息
                    strLicXmlPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\UMP.Server\\";

                    licOperation = new LicenseServerOperation(strLicXmlPath, "Args02.UMP.xml");
                    if (!licOperation.bIsLoadXml)
                    {
                        SendMessageToClient("Error003");
                        SendMessageToClient(Common.AscCodeToChr(27) + "End" + Common.AscCodeToChr(27));

                        UMPService00.WriteLog(LogMode.Error, "Can not find the umpparam_simp.xml and Args02.UMP.xml");
                        return;
                    }
                    lstLicenseServers = licOperation.GetLicenseServersOnUMPServer();
                }

                if (lstLicenseServers.Count <= 0)
                {

                    UMPService00.WriteLog(LogMode.Error, "No license server  ");
                    SendMessageToClient("Error001");
                    SendMessageToClient(Common.AscCodeToChr(27) + "End" + Common.AscCodeToChr(27));
                    return;
                }
                string strMsg = "lstLicenseServers.Count = " + lstLicenseServers.Count + " ; \r\n";
                for (int i = 0; i < lstLicenseServers.Count; i++)
                {
                    strMsg += "IP = " + lstLicenseServers[i].Host + " ; Port = " + lstLicenseServers[i].Port + " ; \r\n";
                }

                UMPService00.WriteLog(LogMode.Warn, strMsg);
                TcpClient clientToLicServer = null;
                SslStream sslStreamToLicServer = null;
                bool bIsConneted = false;
                int iConnResult = 0;
                foreach (LicenseServer licServer in lstLicenseServers)
                {
                    iConnResult = licOperation.ConnectToLicenseServer(licServer.Host, licServer.Port, 0, ref sslStreamToLicServer, ref clientToLicServer);

                    UMPService00.WriteLog("Conn licserver " + licServer.Host + ":" + licServer.Port + " , result = " + iConnResult.ToString());
                    //如果返回值为0 则跳出循环
                    if (iConnResult == 0)
                    {
                        bIsConneted = true;
                        break;
                    }
                    //如果返回值不为0 则关掉sslstream和client
                    if (sslStreamToLicServer != null)
                    {
                        if (sslStreamToLicServer.CanRead)
                        {
                            sslStreamToLicServer.Close();
                        }
                    }
                    if (clientToLicServer != null)
                    {
                        if (clientToLicServer.Connected)
                        {
                            clientToLicServer.Close();
                        }
                    }
                }
                if (!bIsConneted)
                {
                    SendMessageToClient("Error002");
                    SendMessageToClient(Common.AscCodeToChr(27) + "End" + Common.AscCodeToChr(27));

                    UMPService00.WriteLog(LogMode.Error, "Can not connect to all the license server");
                    return;
                }
                string strResult = licOperation.GetLicenseInfo(sslStreamToLicServer, clientToLicServer, "G009");
                SendMessageToClient(strResult);
                SendMessageToClient(Common.AscCodeToChr(27) + "End" + Common.AscCodeToChr(27));
            }
            catch (Exception ex)
            {
                SendMessageToClient("ErrorG009");
                SendMessageToClient(Common.AscCodeToChr(27) + "End" + Common.AscCodeToChr(27));

                UMPService00.WriteLog(LogMode.Error, "ClientMessageG009()\n" + ex.ToString() + "\n" + ITcpClient.Client.RemoteEndPoint.ToString());
            }
        }

        /// <summary>
        /// 获得软件狗序列号
        /// </summary>
        /// <param name="AStrArrayInfo"></param>
        private void ClientMessageG010(string[] AStrArrayInfo)
        {
            try
            {
                //先从ProgramData\VoiceCyber\UMP\config下读认证服务器参数
                string strLicXmlPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\VoiceCyber\\UMP\\config\\";
                LicenseServerOperation licOperation = new LicenseServerOperation(strLicXmlPath, "umpparam_simp.xml");
                List<LicenseServer> lstLicenseServers = new List<LicenseServer>();
                ////如果读不到认证服务器参数 则从Args02.UMP.xml中读取
                //if (!licOperation.bIsLoadXml)
                //{
                //    //从GloableSetting中读取LicenseServer信息
                //    strLicXmlPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\UMP.Server\\";
                //    licOperation = new LicenseServerOperation(strLicXmlPath, "Args02.UMP.xml");
                //    if (!licOperation.bIsLoadXml)
                //    {
                //        SendMessageToClient("Error003");
                //        SendMessageToClient(Common.AscCodeToChr(27) + "End" + Common.AscCodeToChr(27));
                //        UMPService00.IEventLog.WriteEntry("Can not find the umpparam_simp.xml and Args02.UMP.xml", EventLogEntryType.Error);
                //        return;
                //    }
                //    lstLicenseServers = licOperation.GetLicenseServersOnUMPServer();
                //}
                //else
                //{
                //    lstLicenseServers = licOperation.GetLicenseServerOnVoiceServer();
                //    if (lstLicenseServers.Count <= 0)
                //    {
                //        //从GloableSetting中读取LicenseServer信息
                //        strLicXmlPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\UMP.Server\\";
                //        // UMPService00.IEventLog.WriteEntry("strLicXmlPath = " + strLicXmlPath, EventLogEntryType.Warning);
                //        licOperation = new LicenseServerOperation(strLicXmlPath, "Args02.UMP.xml");
                //        if (!licOperation.bIsLoadXml)
                //        {
                //            SendMessageToClient("Error003");
                //            SendMessageToClient(Common.AscCodeToChr(27) + "End" + Common.AscCodeToChr(27));
                //            UMPService00.IEventLog.WriteEntry("Can not find the umpparam_simp.xml and Args02.UMP.xml", EventLogEntryType.Error);
                //            return;
                //        }
                //        lstLicenseServers = licOperation.GetLicenseServersOnUMPServer();
                //    }
                //}

                if (lstLicenseServers.Count <= 0)
                {
                    //从GloableSetting中读取LicenseServer信息
                    strLicXmlPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\UMP.Server\\";

                    licOperation = new LicenseServerOperation(strLicXmlPath, "Args02.UMP.xml");
                    if (!licOperation.bIsLoadXml)
                    {
                        SendMessageToClient("Error003");
                        SendMessageToClient(Common.AscCodeToChr(27) + "End" + Common.AscCodeToChr(27));

                        UMPService00.WriteLog(LogMode.Error, "Can not find the umpparam_simp.xml and Args02.UMP.xml");
                        return;
                    }
                    lstLicenseServers = licOperation.GetLicenseServersOnUMPServer();
                }

                if (lstLicenseServers.Count <= 0)
                {

                    UMPService00.WriteLog(LogMode.Warn, "No license server  ");
                    SendMessageToClient("Error001");
                    SendMessageToClient(Common.AscCodeToChr(27) + "End" + Common.AscCodeToChr(27));
                    return;
                }
                string strMsg = "lstLicenseServers.Count = " + lstLicenseServers.Count + " ; \r\n";
                for (int i = 0; i < lstLicenseServers.Count; i++)
                {
                    strMsg += "IP = " + lstLicenseServers[i].Host + " ; Port = " + lstLicenseServers[i].Port + " ; \r\n";
                }

                UMPService00.WriteLog(LogMode.Error, strMsg);
                TcpClient clientToLicServer = null;
                SslStream sslStreamToLicServer = null;
                bool bIsConneted = false;
                int iConnResult = 0;
                foreach (LicenseServer licServer in lstLicenseServers)
                {
                    iConnResult = licOperation.ConnectToLicenseServer(licServer.Host, licServer.Port, 7692, ref sslStreamToLicServer, ref clientToLicServer);

                    UMPService00.WriteLog("Conn licserver " + licServer.Host + ":" + licServer.Port + " , result = " + iConnResult.ToString());
                    //如果返回值为0 则跳出循环
                    if (iConnResult == 0)
                    {
                        bIsConneted = true;
                        break;
                    }
                    //如果返回值不为0 则关掉sslstream和client
                    if (sslStreamToLicServer != null)
                    {
                        if (sslStreamToLicServer.CanRead)
                        {
                            sslStreamToLicServer.Close();
                        }
                    }
                    if (clientToLicServer != null)
                    {
                        if (clientToLicServer.Connected)
                        {
                            clientToLicServer.Close();
                        }
                    }
                }
                if (!bIsConneted)
                {
                    SendMessageToClient("Error002");
                    SendMessageToClient(Common.AscCodeToChr(27) + "End" + Common.AscCodeToChr(27));

                    UMPService00.WriteLog(LogMode.Error, "Can not connect to all the license server");
                    return;
                }
                string strResult = licOperation.GetLicenseInfo(sslStreamToLicServer, clientToLicServer, "G010");
                SendMessageToClient(strResult);
                SendMessageToClient(Common.AscCodeToChr(27) + "End" + Common.AscCodeToChr(27));
            }
            catch (Exception ex)
            {
                SendMessageToClient("ErrorG008");
                SendMessageToClient(Common.AscCodeToChr(27) + "End" + Common.AscCodeToChr(27));

                UMPService00.WriteLog(LogMode.Error, "ClientMessageG008()\n" + ex.ToString() + "\n" + ITcpClient.Client.RemoteEndPoint.ToString());
            }
        }

        /// <summary>
        /// 检查LicenseServer是否可用
        /// </summary>
        /// <param name="AStrArrayInfo">
        /// [0]: C001
        /// [1]: 软件狗服务器IP
        /// [2]: 端口（默认3070）
        /// </param>
        private void ClientMessageC001(string[] AStrArrayInfo)
        {
            try
            {
                if (AStrArrayInfo.Count() < 3)
                {

                    UMPService00.WriteLog(LogMode.Error, "Param error");
                    SendMessageToClient("-1");
                    SendMessageToClient(Common.AscCodeToChr(27) + "End" + Common.AscCodeToChr(27));
                    return;
                }
                int iPort = int.Parse(AStrArrayInfo[2]);
                LicenseServerOperation licOperation = new LicenseServerOperation();
                int iResult = licOperation.CheckLicenseServer(AStrArrayInfo[1], iPort);
                SendMessageToClient(iResult.ToString());
                SendMessageToClient(Common.AscCodeToChr(27) + "End" + Common.AscCodeToChr(27));
            }
            catch (Exception ex)
            {
                SendMessageToClient("ErrorC001");
                SendMessageToClient(Common.AscCodeToChr(27) + "End" + Common.AscCodeToChr(27));

                UMPService00.WriteLog(LogMode.Error, "ClientMessageG008()\n" + ex.ToString() + "\n" + ITcpClient.Client.RemoteEndPoint.ToString());
            }
        }

        /// <summary>
        /// 通知消息 备机启动 接替主机（已废弃）
        /// </summary>
        /// <param name="?">N001
        /// 备机ModuleNumber/Key
        /// 要接替的主机的ModuleNumber/key
        /// 备机的IP</param>
        private void ClientMessageN001(string[] LStrUserSendData)
        {
            if (LStrUserSendData.Count() < 4)
            {

                UMPService00.WriteLog(LogMode.Error, "Param error");
                SendMessageToClient("-1");
                SendMessageToClient(Common.AscCodeToChr(27) + "End" + Common.AscCodeToChr(27));
                return;
            }
            try
            {
                //读取数据库信息
                string strPDBFileath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                string strDbInfo = DataBaseXmlOperator.GetDBInfo(strPDBFileath);

                //如果数据库信息不包括char27
                if (!strDbInfo.Contains(Common.AscCodeToChr(27)))
                {
                    SendMessageToClient("Get database info error");
                    SendMessageToClient(Common.AscCodeToChr(27) + "End" + Common.AscCodeToChr(27));

                    return;
                }

                //备机Key/ModuleNumber
                string strSourceKey = LStrUserSendData[1];
                //要接替的主机Key/ModuleNumber
                string strTargetKey = LStrUserSendData[2];
                //备机的IP地址
                string strSourceAddress = LStrUserSendData[3];
                //给客户端回消息
                SendMessageToClient("OK");

                SendMessageToClient(Common.AscCodeToChr(27) + "End" + Common.AscCodeToChr(27));
                //给备机发消息 内容 : 备机Key+Char27+要接替的主机Key
                string strSendMessage = "N002" + Common.AscCodeToChr(27) + strSourceKey + Common.AscCodeToChr(27) + strTargetKey;
                strSendMessage += Common.AscCodeToChr(27) + strDbInfo;

                UMPService00.WriteLog("Info = " + strSendMessage);
                SendMessageToService00(strSourceAddress, 8009, strSendMessage);
            }
            catch (Exception ex)
            {
                SendMessageToClient("ErrorC001");
                SendMessageToClient(Common.AscCodeToChr(27) + "End" + Common.AscCodeToChr(27));

                UMPService00.WriteLog(LogMode.Error, "ClientMessageN001()\n" + ex.ToString() + "\n" + ITcpClient.Client.RemoteEndPoint.ToString());
            }
        }

        /// <summary>
        /// 通知消息 启动备机 接替主机 （已废弃）
        /// </summary>
        /// <param name="LStrUserSendData">N002
        /// 备机ModuleNumber/Key
        /// 要接替的主机的ModuleNumber/key</param>
        /// ---以下是数据库信息--
        /// DbTypeID
        /// TypeName
        /// Host
        /// Port
        /// LoginName
        /// Password
        /// DBName
        private void ClientMessageN002(string[] LStrUserSendData)
        {
            if (LStrUserSendData.Count() < 10)
            {

                UMPService00.WriteLog(LogMode.Error, "Param error");
                SendMessageToClient("-1");
                SendMessageToClient(Common.AscCodeToChr(27) + "End" + Common.AscCodeToChr(27));
                return;
            }
            SendMessageToClient("OK");
            SendMessageToClient(Common.AscCodeToChr(27) + "End" + Common.AscCodeToChr(27));
            try
            {
                //修改数据库
                DatabaseInfo dbInfo = new DatabaseInfo();
                int intTemp = 0;
                int.TryParse(LStrUserSendData[3], out intTemp);
                dbInfo.TypeID = intTemp;
                dbInfo.TypeName = LStrUserSendData[4];
                dbInfo.Host = LStrUserSendData[5];
                int.TryParse(LStrUserSendData[6], out intTemp);
                dbInfo.Port = intTemp;
                dbInfo.LoginName = LStrUserSendData[7];
                dbInfo.Password = LStrUserSendData[8];
                dbInfo.DBName = LStrUserSendData[9];
                OperationReturn optReturn = ResourceOperation.UpdateReplaceModuleNumberInDB(dbInfo, LStrUserSendData[2], LStrUserSendData[1]);
                if (optReturn.Result == false)
                {

                    UMPService00.WriteLog(LogMode.Error, "UpdateReplaceModuleNumberInDB failed");
                    return;
                }

                //修改XML
                ParamXmlOperation.StartBackupMachine(LStrUserSendData[1], LStrUserSendData[2]);
                //重启Voice服务
                bool bIsRestartSer = Common.RestartService("UMPVoiceServer");
                if (bIsRestartSer)
                {

                    UMPService00.WriteLog(LogMode.Info, "Restart service UMPVoiceServer success");
                }
                else
                {

                    UMPService00.WriteLog(LogMode.Error, "Restart service UMPVoiceServer failed");
                }

                #region 通知主机更新xml
                //读simpxml，根据modulenumber获得主机IP
                string strHost = ParamXmlOperation.GetVoiceServerHostByModuleNumber(int.Parse(LStrUserSendData[2]));
                string strMsg = "N003" + Common.AscCodeToChr(27) + LStrUserSendData[2] + Common.AscCodeToChr(27) + LStrUserSendData[1];
                SendMessageToService00(strHost, 8009, strMsg);
                #endregion
            }
            catch (Exception ex)
            {

                UMPService00.WriteLog(LogMode.Error, "ClientMessageN002()\n" + ex.ToString() + "\n" + ITcpClient.Client.RemoteEndPoint.ToString());
            }
        }

        /// <summary>
        /// 备机发消息给主机 更新主机的xml 告知主机已经被备机接替
        /// </summary>
        /// <param name="LStrUserSendData"></param>
        /// 0: N003
        /// 1: 主机ID
        /// 2：备机ID
        private void ClientMessageN003(string[] LStrUserSendData)
        {
            if (LStrUserSendData.Count() < 3)
            {

                UMPService00.WriteLog(LogMode.Error, "Param error");
                SendMessageToClient("-1");
                SendMessageToClient(Common.AscCodeToChr(27) + "End" + Common.AscCodeToChr(27));
                return;
            }
            SendMessageToClient("OK");
            SendMessageToClient(Common.AscCodeToChr(27) + "End" + Common.AscCodeToChr(27));

            ParamXmlOperation.StartBackupMachine(LStrUserSendData[2], LStrUserSendData[1]);
        }

        /// <summary>
        /// 接收cmserver发来的各个物理机器的key和replacemodulenumber
        /// </summary>
        /// <param name="LStrUserSendData"></param>
        private void ClientMessageN004(string[] LStrUserSendData)
        {
            //如果只有一个命令号 就忽略
            if (LStrUserSendData.Count() <= 1)
            {
                SendMessageToClient("param error");
                SendMessageToClient(Common.AscCodeToChr(27) + "End" + Common.AscCodeToChr(27));
                return;
            }
            SendMessageToClient("OK");
            SendMessageToClient(Common.AscCodeToChr(27) + "End" + Common.AscCodeToChr(27));

            UMPService00.WriteLog("00");
            Dictionary<int, string> lstMachines = new Dictionary<int, string>();
            int iKey = 0;
            for (int i = 0; i < LStrUserSendData.Count(); i++)
            {
                if (LStrUserSendData[i].Contains(":"))
                {
                    string strKey = LStrUserSendData[i].Substring(0, LStrUserSendData[i].IndexOf(":"));
                    int.TryParse(strKey, out iKey);
                    lstMachines.Add(iKey, LStrUserSendData[i].Substring(LStrUserSendData[i].IndexOf(":") + 1));
                }
            }

            UMPService00.WriteLog("01");
            string strPDBFileath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            DatabaseInfo dbInfo = null;
            bool bo = DataBaseXmlOperator.GetDBInfo(strPDBFileath, ref dbInfo);

            UMPService00.WriteLog("02");
            OperationReturn optReturn = ResourceOperation.GetAllMachines(dbInfo);
            if (!optReturn.Result)
            {

                UMPService00.WriteLog(optReturn.Message);
                return;
            }

            UMPService00.WriteLog("03");
            if (optReturn.Data == null)
            {

                UMPService00.WriteLog("No machine");

                return;
            }
            Dictionary<int, MachineInfo> dicMachineInDB = optReturn.Data as Dictionary<int, MachineInfo>;
            bo = true;
            long lResID = 0;
            string strReplaceModuleNumber = string.Empty;
            //判断状态是否有更新
            foreach (KeyValuePair<int, string> pair in lstMachines)
            {
                //如果不一样 
                if (!dicMachineInDB.Keys.Contains(pair.Key))
                {
                    bo = false;
                    continue;
                }
                if (dicMachineInDB[pair.Key].ReplaceModuleNumber != pair.Value.ToString())
                {
                    lResID = dicMachineInDB[pair.Key].ResID;
                    strReplaceModuleNumber = pair.Value;
                    optReturn = ResourceOperation.UpdateReplaceModuleNumberInDB(dbInfo, lResID, strReplaceModuleNumber);
                    if (!optReturn.Result)
                    {

                        UMPService00.WriteLog("UpdateReplaceModuleNumberInDB error: " + optReturn.Message);
                    }

                    UMPService00.WriteLog("key = " + pair.Key + " , resID = " + dicMachineInDB[pair.Key].ResID);
                    bo = false;
                    continue;
                }
            }

            UMPService00.WriteLog("04;result =" + bo.ToString());
            if (bo)
            {
                //状态没有变化 不做处理
                return;
            }
            //如果状态有变化 并向所有机器的service00发出通知 更新xml
            #region 获得IIS端口、认证服务器IP、端口,把DBInfo对象转为string型，用char27连接
            int iIISPort = 0;
            // SendMessageToClient(Common.GetCurrentBaseDirectory().Trim('\\'));
            GloableParamOperation gloable = new GloableParamOperation(Common.GetCurrentBaseDirectory().Trim('\\'));
            bool bGetWebPortResult = gloable.GetWebSitePort(ref iIISPort);
            if (!bGetWebPortResult)
            {
                SendMessageToClient("Get website port error");
                SendMessageToClient(Common.AscCodeToChr(27) + "End" + Common.AscCodeToChr(27));

                UMPService00.WriteLog(LogMode.Warn, "Get website port error. path = " + Common.GetCurrentBaseDirectory().Trim('\\'));
                return;
            }
            UMPService00.WriteLog("05");

            //UMPService00.IEventLog.WriteEntry("iIISPort: " + iIISPort, EventLogEntryType.Warning);
            List<string> lstAuthenServerInfo = new List<string>();
            bool IsGetAuthenServer = gloable.GetAuthenticateServerInfo(ref lstAuthenServerInfo);
            if (!IsGetAuthenServer)
            {
                UMPService00.WriteLog(LogMode.Error, "Get Authenticate Server Info error");
            }
            string LStrVerificationCode004 = Common.CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004);
            string strAuthenticateServerHost = EncryptionAndDecryption.EncryptDecryptString(lstAuthenServerInfo[0], LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
            string strAuthenticateServerPort = EncryptionAndDecryption.EncryptDecryptString(lstAuthenServerInfo[1], LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);

            string LStrVerificationCode = Common.CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104);
            string strDBInfo = dbInfo.TypeID + Common.AscCodeToChr(27) + dbInfo.TypeName + Common.AscCodeToChr(27) + dbInfo.Host + Common.AscCodeToChr(27);
            strDBInfo += dbInfo.Port.ToString()
                + Common.AscCodeToChr(27) + dbInfo.LoginName + Common.AscCodeToChr(27);
            strDBInfo += dbInfo.Password + Common.AscCodeToChr(27) + dbInfo.DBName;
            ;
            UMPService00.WriteLog("06");
            #endregion

            string strSendMessage = string.Empty;
            List<string> lstArgs = null;
            BackgroundWorker bgSendToVoice = null;
            //再循环数组 给每个机器发消息 重新生成xml
            //给该机器的service00发消息G005
            foreach (KeyValuePair<int, string> pair in lstMachines)
            {
                strSendMessage = EncryptionAndDecryption.EncryptDecryptString("G005", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                strSendMessage += Common.AscCodeToChr(27) + strDBInfo + Common.AscCodeToChr(27) + iIISPort + Common.AscCodeToChr(27)
                    + strAuthenticateServerHost + Common.AscCodeToChr(27) + strAuthenticateServerPort;
                string strStandByRole = dicMachineInDB[pair.Key].StandByRole;

                UMPService00.WriteLog("strStandByRole = " + strStandByRole);
                ///是备机 则多加一个参数 用以重启备机的voice服务
                if (strStandByRole == "3")
                {
                    if (pair.Value == dicMachineInDB[pair.Key].ReplaceModuleNumber)
                    {
                        strSendMessage += Common.AscCodeToChr(27) + "0";
                    }
                    else
                    {
                        strSendMessage += Common.AscCodeToChr(27) + "1";
                    }
                }
                else
                {
                    strSendMessage += Common.AscCodeToChr(27) + "0";
                }
                lstArgs = new List<string>();
                lstArgs.Add(dicMachineInDB[pair.Key].Host);
                lstArgs.Add("8009");
                lstArgs.Add(strSendMessage);

                UMPService00.WriteLog(LogMode.Info, "strSendMessage = " + strSendMessage + " ; host = " + lstArgs[0] + " ; port = " + lstArgs[1]);
                bgSendToVoice = new BackgroundWorker();
                bgSendToVoice.DoWork += bgSendToVoice_DoWork;
                bgSendToVoice.RunWorkerCompleted += bgSendToVoice_RunWorkerCompleted;
                bgSendToVoice.RunWorkerAsync(lstArgs);
            }

            UMPService00.WriteLog("05");
        }

        private void ClientMessageU001(string[] arrArgs)
        {
            try
            {
                /*
                 * Args
                 * 
                 * 0：       UMP 服务器地址
                 * 1：       http下载LoggingUpdater升级程序的端口
                 * 2：       升级程序UMPUpdater开放的升级端口
                 * 3：       LoggingUpdater升级程序的文件名，位于UMP服务器临时目录MediaData中，是升级程序临时拷入的
                 * 4：       验证码
                 * 
                 * 返回
                 * 
                 * S01      成功，表示LoggingUpdater已经下载成功并启动运行了
                 * 
                 * E01      失败，参数个数无效
                 * E02      失败，参数无效
                 * E03      失败，下载LoggingUpdater失败
                 * E04      失败，启动LoggingUpdater失败
                 * 
                 */

                if (arrArgs.Length < 6)
                {
                    SendMessageToClient("ErrorU001" + "E01");
                    SendMessageToClient(Common.AscCodeToChr(27) + "End" + Common.AscCodeToChr(27));
                    UMPService00.WriteLog(LogMode.Error,
                        string.Format("{0}{1}{2}", "ErrorU001", "E01", string.Format("Args count invalid")));
                    return;
                }
                string strHost = arrArgs[1];
                string strHttpPort = arrArgs[2];
                string strPort = arrArgs[3];
                string strFile = arrArgs[4];
                string strToken = arrArgs[5];
                int httpPort;
                int intPort;
                if (string.IsNullOrEmpty(strHost)
                    || string.IsNullOrEmpty(strHttpPort)
                    || string.IsNullOrEmpty(strPort)
                    || string.IsNullOrEmpty(strFile)
                    || string.IsNullOrEmpty(strToken))
                {
                    SendMessageToClient("ErrorU001" + "E02");
                    SendMessageToClient(Common.AscCodeToChr(27) + "End" + Common.AscCodeToChr(27));
                    UMPService00.WriteLog(LogMode.Error,
                        string.Format("{0}{1}{2}", "ErrorU001", "E02", string.Format("Param invalid. Empty!")));
                    return;
                }
                if (!int.TryParse(strHttpPort, out httpPort)
                    || !int.TryParse(strPort, out intPort))
                {
                    SendMessageToClient("ErrorU001" + "E02");
                    SendMessageToClient(Common.AscCodeToChr(27) + "End" + Common.AscCodeToChr(27));
                    UMPService00.WriteLog(LogMode.Error,
                        string.Format("{0}{1}{2}", "ErrorU001", "E02", string.Format("Param invalid. Port invalid!")));
                    return;
                }
                UMPService00.WriteLog(LogMode.Info,
                   string.Format("{0}\t{1}", "U001", string.Format("Args\tHost:{0};HttpPort:{1};UpdatePort:{2};LoggingUpdaterFile:{3}", strHost, httpPort, intPort, strFile)));
                string strRequestFile = Path.Combine("MediaData", strFile);
                string strLocalPath =
                    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                        "UMP\\UMPService00");
                if (!Directory.Exists(strLocalPath))
                {
                    Directory.CreateDirectory(strLocalPath);
                }
                string strLocalFile = Path.Combine(strLocalPath, strFile);
                DownloadConfig config = new DownloadConfig();
                config.Method = 1;
                config.Host = strHost;
                config.Port = httpPort;
                config.RequestPath = strRequestFile;
                config.SavePath = strLocalFile;
                OperationReturn optReturn = DownloadHelper.DownloadFile(config);
                if (!optReturn.Result)
                {
                    SendMessageToClient("ErrorU001" + "E03");
                    SendMessageToClient(Common.AscCodeToChr(27) + "End" + Common.AscCodeToChr(27));
                    UMPService00.WriteLog(LogMode.Error,
                        string.Format("{0}{1}{2}", "ErrorU001", "E03",
                            string.Format("Fail.\t{0}\t{1}", optReturn.Code, optReturn.Message)));
                    return;
                }
                UMPService00.WriteLog(LogMode.Info,
                    string.Format("{0}\t{1}", "U001", string.Format("LoggingUpdate download end.\t{0}", strLocalFile)));
                //下载完成，启动LoggingUpdater
                int updateMode = 2;     //在线升级模式
                Process process = new Process();
                process.StartInfo.FileName = strLocalFile;
                process.StartInfo.WorkingDirectory = strLocalPath;
                process.StartInfo.Arguments = string.Format("{0} {1} {2} {3}", updateMode, strHost, intPort, strToken);
                var result = process.Start();
                if (!result)
                {
                    SendMessageToClient("ErrorU001" + "E04");
                    SendMessageToClient(Common.AscCodeToChr(27) + "End" + Common.AscCodeToChr(27));
                    UMPService00.WriteLog(LogMode.Error,
                        string.Format("{0}{1}{2}", "ErrorU001", "E04", string.Format("Fail.")));
                    return;
                }
                SendMessageToClient(string.Format("S01"));
                SendMessageToClient(Common.AscCodeToChr(27) + "End" + Common.AscCodeToChr(27));
            }
            catch (Exception ex)
            {
                SendMessageToClient("ErrorU001");
                SendMessageToClient(Common.AscCodeToChr(27) + "End" + Common.AscCodeToChr(27));
                UMPService00.WriteLog(LogMode.Error, "ClientMessageU001()\n" + ex.ToString() + "\n" + ITcpClient.Client.RemoteEndPoint.ToString());
            }
        }

        private ManagementObjectCollection GetServiceCollection(string AStrQuery)
        {
            ConnectionOptions ComputerConnect = new ConnectionOptions();

            ManagementScope ComputerManagement = new ManagementScope("\\\\localhost\\root\\cimv2", ComputerConnect);
            ComputerConnect.Timeout = new TimeSpan(0, 10, 0);
            ComputerManagement.Connect();
            ObjectQuery VoiceServiceQuery = new ObjectQuery(AStrQuery);
            ManagementObjectSearcher ObjectSearcher = new ManagementObjectSearcher(ComputerManagement, VoiceServiceQuery);
            ManagementObjectCollection ReturnCollection = ObjectSearcher.Get();
            return ReturnCollection;
        }

        /// <summary>
        /// 发送消息给录音服务器上的service00
        /// </summary>
        /// <param name="AStrHost"></param>
        /// <param name="AIPort">string strDBInfo, int iisPort, string strAuthenServerHost, string strAuthenServerPort</param>
        public void SendMessageToService00(string AStrHost, int AIPort, string LStrSendMessage)
        {

            TcpClient LTcpClient = null;
            SslStream LSslStream = null;
            try
            {
                LTcpClient = new TcpClient(AStrHost, AIPort);

                //LTcpClient.ReceiveTimeout = 5;
                //LSslStream = new SslStream(LTcpClient.GetStream(), false, new RemoteCertificateValidationCallback(ValidateServerCertificate), null);
                //LSslStream.AuthenticateAsClient("VoiceCyber.PF", null, SslProtocols.Default, false);

                LSslStream = new SslStream(LTcpClient.GetStream(), false, (s, cert, chain, err) => true);
                LSslStream.AuthenticateAsClient(AStrHost);

                LSslStream.Write(Encoding.UTF8.GetBytes(LStrSendMessage + "\r\n"));
                LSslStream.Flush();
                ClientOperations client = new ClientOperations(LTcpClient);
                client.ISslStream = LSslStream;
                Thread thread = new Thread(new ThreadStart(client.DealMessageFromClient));
                bool LBoolReturn = false;
                string LStrReturn = string.Empty;
                AddConnectedClientAndThread(client, thread, ref LBoolReturn, ref LStrReturn);
                if (LBoolReturn) { thread.Start(); } else { UMPService00.WriteLog(LogMode.Error, LStrReturn); }
            }
            catch (Exception ex)
            {

                UMPService00.WriteLog(LogMode.Error, "Send to service00 in " + AStrHost + " error, " + ex.Message);
            }

        }

        private bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.RemoteCertificateChainErrors || sslPolicyErrors == SslPolicyErrors.None) { return true; }
            return false;
        }

        /// <summary>
        /// 当service00作为客户端 给其他service00发消息时 获取从服务端回过来的消息 收到消息就关闭当前客户端
        /// </summary>
        public void DealMessageFromClient()
        {
            string AStrReadedMessage = string.Empty;
            try
            {
                StringBuilder LStringBuilderData = new StringBuilder();
                int LIntReadedBytes = -1, LIntEndKeyPosition;
                byte[] LByteReadeBuffer = new byte[1024];

                do
                {
                    LIntReadedBytes = ISslStream.Read(LByteReadeBuffer, 0, LByteReadeBuffer.Length);
                    Decoder LDecoder = Encoding.UTF8.GetDecoder();
                    char[] LChars = new char[LDecoder.GetCharCount(LByteReadeBuffer, 0, LIntReadedBytes)];
                    LDecoder.GetChars(LByteReadeBuffer, 0, LIntReadedBytes, LChars, 0);
                    LStringBuilderData.Append(LChars);
                    if (LStringBuilderData.ToString().IndexOf(Common.AscCodeToChr(27) + "End" + Common.AscCodeToChr(27)) > 0) { break; }
                }
                while (LIntReadedBytes != 0);
                AStrReadedMessage = LStringBuilderData.ToString();
                LIntEndKeyPosition = AStrReadedMessage.IndexOf(Common.AscCodeToChr(27) + "End" + Common.AscCodeToChr(27));
                if (LIntEndKeyPosition > 0)
                {
                    ISslStream.Flush();
                    ISslStream.Close();
                    ITcpClient.Close();
                }
            }
            catch (Exception ex)
            {
                ISslStream.Flush();
                ISslStream.Close();
                ITcpClient.Close();
            }
        }

        private void AddConnectedClientAndThread(ClientOperations AClientOperation, Thread AClientThread, ref bool ABoolReturn, ref string AStrReturn)
        {
            try
            {
                ABoolReturn = true;
                AStrReturn = string.Empty;

                lock (ILockObject)
                {
                    UMPService00.GlobalListConnectedClient.Add(AClientOperation);
                    UMPService00.GlobalListClientThread.Add(AClientThread);
                    UMPService00.WriteLog("AddConnectedClientAndThread() GlobalListClientThread.Count = " + UMPService00.GlobalListClientThread.Count.ToString()
                        + "    GlobalListConnectedClient.Count = " + UMPService00.GlobalListConnectedClient.Count.ToString(), EventLogEntryType.Information);

                }
            }
            catch (Exception ex)
            {
                ABoolReturn = false;
                AStrReturn = "AddConnectedClientAndThread()\n" + ex.Message;
            }
        }

        /// <summary>
        /// 重启服务 
        /// </summary>
        /// <param name="strBatPath">重启服务的bat路径</param>
        private void RestartService(string strBatPath)
        {
            try
            {
                Process myProcess = new Process();
                ProcessStartInfo myProcessStartInfo = new ProcessStartInfo(strBatPath);
                //UMPService00.IEventLog.WriteEntry("bat path = " + AppDomain.CurrentDomain.BaseDirectory + "\\RestartService.bat");
                myProcessStartInfo.UseShellExecute = false;
                myProcessStartInfo.CreateNoWindow = true;
                myProcessStartInfo.RedirectStandardOutput = true;
                myProcess.StartInfo = myProcessStartInfo;
                myProcessStartInfo.Arguments = "/c";
                myProcess.Start();
                StreamReader myStreamReader = myProcess.StandardOutput;
                string rInfo = myStreamReader.ReadToEnd();
                myProcess.Close();

                UMPService00.WriteLog(LogMode.Warn, "Reatart service result : \r\n" + rInfo);
            }
            catch (Exception ex)
            {

                UMPService00.WriteLog(LogMode.Error, "RestartService  error : path = " + strBatPath);
            }
        }
    }
}
