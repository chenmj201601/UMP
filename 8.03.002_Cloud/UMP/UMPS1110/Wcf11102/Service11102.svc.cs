using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.ServiceModel.Activation;
using System.Text;
using System.Threading;
using PFShareClassesS;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common11101;
using VoiceCyber.UMP.Communications;

namespace Wcf11102
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码、svc 和配置文件中的类名“Service11102”。
    // 注意: 为了启动 WCF 测试客户端以测试此服务，请在解决方案资源管理器中选择 Service11102.svc 或 Service11102.svc.cs，然后开始调试。
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class Service11102 : IService11102, IDisposable
    {
        private Service00Helper mService00Helper;
        private LogOperator mLogOperator;

        public Service11102()
        {
            CreateLogOperator();
        }

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
                DatabaseInfo dbInfo = session.DatabaseInfo;
                if (dbInfo != null)
                {
                    dbInfo.RealPassword = DecryptString(dbInfo.Password);
                    session.DBConnectionString = dbInfo.GetConnectionString();
                }
                optReturn = XMLHelper.DeserializeObject<ServerRequestInfo>(webRequest.Data);
                if (!optReturn.Result)
                {
                    webReturn.Result = false;
                    webReturn.Code = optReturn.Code;
                    webReturn.Message = optReturn.Message;
                    return webReturn;
                }
                ServerRequestInfo request = optReturn.Data as ServerRequestInfo;
                if (request == null)
                {
                    webReturn.Result = false;
                    webReturn.Code = Defines.RET_OBJECT_NULL;
                    webReturn.Message = string.Format("RequestInfo is null");
                    return webReturn;
                }
                switch (request.Command)
                {
                    case (int)ServerRequestCommand.GetDiskDriverList:
                        optReturn = GetDiskDriverList(request);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)ServerRequestCommand.GetChildDirectoryList:
                        optReturn = GetChildDirectoryList(request);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)ServerRequestCommand.GetChildFileList:
                        optReturn = GetChildFileList(request);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)ServerRequestCommand.GetNetworkCardList:
                        optReturn = GetNetworkCardList(request);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)ServerRequestCommand.GetCTIServiceName:
                        optReturn = GetCTIServiceName(request);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)ServerRequestCommand.GetServerName:
                        
                        break;
                    case (int)ServerRequestCommand.SetResourceChanged:
                        optReturn = SetResourceChanged(request);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        break;
                    default:
                        webReturn.Result = false;
                        webReturn.Code = Defines.RET_PARAM_INVALID;
                        webReturn.Message = string.Format("Request code invalid.\t{0}", webRequest.Code);
                        return webReturn;
                }
                webReturn.Message = optReturn.Message;
                return webReturn;
            }
            catch (Exception ex)
            {
                webReturn.Result = false;
                webReturn.Code = Defines.RET_FAIL;
                webReturn.Message = ex.Message;
                return webReturn;
            }
        }

        private OperationReturn GetDiskDriverList(ServerRequestInfo request)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                List<string> listParams = request.ListData;
                if (listParams.Count < 1)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Parameter count invalid");
                    return optReturn;
                }
                bool isGetSystemDisk = listParams[0] == "1";
                List<string> listReturn = new List<string>();
                List<string> args = new List<string>();
                //不获取系统盘
                args.Add(isGetSystemDisk ? "1" : "0");

                ////测试： 此处获取的是UMP服务器机器的磁盘，不是录音服务器的磁盘信息
                //DriveInfo[] listDrivers = DriveInfo.GetDrives();
                //for (int i = 0; i < listDrivers.Length; i++)
                //{
                //    DriveInfo driver = listDrivers[i];
                //    DirectoryInfo dirInfo = driver.RootDirectory;
                //    string info = string.Format("{0}{1}{2}", dirInfo.Name, ConstValue.SPLITER_CHAR, dirInfo.FullName);
                //    listReturn.Add(info);
                //}
                //optReturn.Data = listReturn;

                //异步模式（过指定的时间超时）
                if (mService00Helper == null)
                {
                    mService00Helper = new Service00Helper();
                    mService00Helper.Debug += mService00Helper_Debug;
                    mService00Helper.Start();
                }
                mService00Helper.HostAddress = request.ServerHost;
                mService00Helper.HostPort = request.ServerPort;
                optReturn = mService00Helper.DoOperation(RequestCommand.GET_DISK_INFO, args);
                if (!optReturn.Result)
                {
                    return optReturn;
                }

                ////同步模式
                //string strSendMessage = string.Format("{0}{1}{2}\r\n",
                //    EncryptString("G002"),
                //    ConstValue.SPLITER_CHAR,
                //    EncryptString("0"));          //不获取系统盘
                //optReturn = GetServerInformation(request, strSendMessage);
                //if (!optReturn.Result)
                //{
                //    return optReturn;
                //}

                string strMessage = optReturn.Data.ToString();
                if (!string.IsNullOrEmpty(strMessage))
                {
                    if (strMessage.StartsWith("Error"))
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_FAIL;
                        optReturn.Message = string.Format("{0}", strMessage);
                        return optReturn;
                    }
                    string[] drivers = strMessage.Split(new[] { "\r\n" }, StringSplitOptions.None);
                    if (drivers.Length > 0)
                    {
                        for (int i = 0; i < drivers.Length; i++)
                        {
                            string driver = drivers[i];
                            if (string.IsNullOrEmpty(driver)) { continue; }
                            string[] arrInfos = driver.Split(new[] { ConstValue.SPLITER_CHAR },
                                StringSplitOptions.None);
                            string strName = string.Empty;
                            string strVolumeName = string.Empty;
                            if (arrInfos.Length > 0)
                            {
                                strName = arrInfos[0];
                            }
                            if (arrInfos.Length > 1)
                            {
                                strVolumeName = arrInfos[1];
                            }
                            string strInfo = string.Format("{0}{1}{2}", strName, ConstValue.SPLITER_CHAR, strVolumeName);
                            listReturn.Add(strInfo);
                        }
                    }
                }
                optReturn.Data = listReturn;
                return optReturn;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
                return optReturn;
            }
        }

        private OperationReturn GetChildDirectoryList(ServerRequestInfo request)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                List<string> listParams = request.ListData;
                //ListParams
                //0     父级目录的完整路径
                if (listParams.Count < 1)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Parameter count invalid");
                    return optReturn;
                }
                string path = listParams[0];
                List<string> listReturn = new List<string>();
                List<string> args = new List<string>();
                //父目录路径
                args.Add(path);

                ////测试： 此处获取的是UMP服务器机器的目录信息，不是录音服务器的目录信息
                //if (!Directory.Exists(path))
                //{
                //    optReturn.Result = false;
                //    optReturn.Code = Defines.RET_NOT_EXIST;
                //    optReturn.Message = string.Format("Directory not exist");
                //    return optReturn;
                //}
                //DirectoryInfo parentDir = new DirectoryInfo(path);
                //DirectoryInfo[] childDirs = parentDir.GetDirectories();
                //for (int i = 0; i < childDirs.Length; i++)
                //{
                //    DirectoryInfo child = childDirs[i];
                //    string info = string.Format("{0}{1}{2}", child.Name, ConstValue.SPLITER_CHAR, child.FullName);
                //    listReturn.Add(info);
                //}
                //optReturn.Data = listReturn;

                //异步模式（过指定的时间超时）
                if (mService00Helper == null)
                {
                    mService00Helper = new Service00Helper();
                    mService00Helper.Debug += mService00Helper_Debug;
                    mService00Helper.Start();
                }
                mService00Helper.HostAddress = request.ServerHost;
                mService00Helper.HostPort = request.ServerPort;
                optReturn = mService00Helper.DoOperation(RequestCommand.GET_SUBDIRECTORY, args);
                if (!optReturn.Result)
                {
                    return optReturn;
                }

                //string strSendMessage = string.Format("{0}{1}{2}\r\n", EncryptString("G004"), ConstValue.SPLITER_CHAR, EncryptString(path));
                //optReturn = GetServerInformation(request, strSendMessage);
                //if (!optReturn.Result)
                //{
                //    return optReturn;
                //}

                string strMessage = optReturn.Data.ToString();
                if (!string.IsNullOrEmpty(strMessage))
                {
                    if (strMessage.StartsWith("Error"))
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_FAIL;
                        optReturn.Message = string.Format("{0}", strMessage);
                        return optReturn;
                    }
                    string[] dirs = strMessage.Split(new[] { "\r\n" }, StringSplitOptions.None);
                    if (dirs.Length > 0)
                    {
                        for (int i = 0; i < dirs.Length; i++)
                        {
                            string dir = dirs[i];
                            if (string.IsNullOrEmpty(dir)) { continue; }
                            string[] arrInfos = dir.Split(new[] { ConstValue.SPLITER_CHAR },
                                StringSplitOptions.None);
                            string strName = string.Empty;
                            if (arrInfos.Length > 0)
                            {
                                strName = arrInfos[0];
                            }
                            string strFullName = Path.Combine(path, strName);
                            string strInfo = string.Format("{0}{1}{2}", strName, ConstValue.SPLITER_CHAR, strFullName);
                            listReturn.Add(strInfo);
                        }
                    }
                }
                optReturn.Data = listReturn;
                return optReturn;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
                return optReturn;
            }
        }

        private OperationReturn GetChildFileList(ServerRequestInfo request)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                List<string> listParams = request.ListData;
                //ListParams
                //0     父级目录的完整路径
                if (listParams.Count < 1)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Parameter count invalid");
                    return optReturn;
                }
                string path = listParams[0];
                List<string> listReturn = new List<string>();
                List<string> args = new List<string>();
                //父目录路径
                args.Add(path);

                //异步模式（过指定的时间超时）
                if (mService00Helper == null)
                {
                    mService00Helper = new Service00Helper();
                    mService00Helper.Debug += mService00Helper_Debug;
                    mService00Helper.Start();
                }
                mService00Helper.HostAddress = request.ServerHost;
                mService00Helper.HostPort = request.ServerPort;
                optReturn = mService00Helper.DoOperation(RequestCommand.GET_SUBFILE, args);
                if (!optReturn.Result)
                {
                    return optReturn;
                }

                string strMessage = optReturn.Data.ToString();
                if (!string.IsNullOrEmpty(strMessage))
                {
                    if (strMessage.StartsWith("Error"))
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_FAIL;
                        optReturn.Message = string.Format("{0}", strMessage);
                        return optReturn;
                    }
                    string[] files = strMessage.Split(new[] { "\r\n" }, StringSplitOptions.None);
                    if (files.Length > 0)
                    {
                        for (int i = 0; i < files.Length; i++)
                        {
                            string file = files[i];
                            if (string.IsNullOrEmpty(file)) { continue; }
                            string[] arrInfos = file.Split(new[] { ConstValue.SPLITER_CHAR },
                                StringSplitOptions.None);
                            string strName = string.Empty;
                            if (arrInfos.Length > 0)
                            {
                                strName = arrInfos[0];
                            }
                            string strFullName = Path.Combine(path, strName);
                            string strInfo = string.Format("{0}{1}{2}", strName, ConstValue.SPLITER_CHAR, strFullName);
                            listReturn.Add(strInfo);
                        }
                    }
                }
                optReturn.Data = listReturn;
                return optReturn;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
                return optReturn;
            }
        }

        private OperationReturn GetNetworkCardList(ServerRequestInfo request)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                List<string> listParams = request.ListData;
                List<string> listReturn = new List<string>();

                //测试： 此处获取的是UMP服务器机器的网卡信息，不是录音服务器的网卡信息
                //NetworkInterface[] cards = NetworkInterface.GetAllNetworkInterfaces();
                //for (int i = 0; i < cards.Length; i++)
                //{
                //    NetworkInterface card = cards[i];
                //    string info = string.Format("{0}{1}{2}{1}{3}", card.Id, ConstValue.SPLITER_CHAR, card.Name,
                //        card.Description);
                //    listReturn.Add(info);
                //}
                //optReturn.Data = listReturn;

                //异步模式（过指定的时间超时）
                List<string> args = new List<string>();
                if (mService00Helper == null)
                {
                    mService00Helper = new Service00Helper();
                    mService00Helper.Debug += mService00Helper_Debug;
                    mService00Helper.Start();
                }
                mService00Helper.HostAddress = request.ServerHost;
                mService00Helper.HostPort = request.ServerPort;
                optReturn = mService00Helper.DoOperation(RequestCommand.GET_NETWORK_CARD, args);
                if (!optReturn.Result)
                {
                    return optReturn;
                }

                //阻塞模式
                //string strSendMessage = string.Format("{0}\r\n", EncryptString("G003"));
                //optReturn = GetServerInformation(request, strSendMessage);
                //if (!optReturn.Result)
                //{
                //    return optReturn;
                //}

                string strMessage = optReturn.Data.ToString();
                if (!string.IsNullOrEmpty(strMessage))
                {
                    if (strMessage.StartsWith("Error"))
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_FAIL;
                        optReturn.Message = string.Format("{0}", strMessage);
                        return optReturn;
                    }
                    string[] cards = strMessage.Split(new[] { "\r\n" }, StringSplitOptions.None);
                    if (cards.Length > 0)
                    {
                        for (int i = 0; i < cards.Length; i++)
                        {
                            string card = cards[i];
                            if (string.IsNullOrEmpty(card)) { continue; }
                            string[] arrInfos = card.Split(new[] { ConstValue.SPLITER_CHAR },
                                StringSplitOptions.None);
                            string strDesc = string.Empty;
                            string strID = string.Empty;
                            if (arrInfos.Length > 0)
                            {
                                strDesc = arrInfos[0];
                            }
                            if (arrInfos.Length > 1)
                            {
                                strID = arrInfos[1];
                            }
                            string strInfo = string.Format("{0}{1}{2}", strDesc, ConstValue.SPLITER_CHAR, strID);
                            listReturn.Add(strInfo);
                        }
                    }
                }
                optReturn.Data = listReturn;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }

        private OperationReturn GetCTIServiceName(ServerRequestInfo request)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                List<string> listParams = request.ListData;
                //ListParams
                //0     PBX主机地址
                if (listParams.Count < 1)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Parameter count invalid");
                    return optReturn;
                }
                string strPBXAddress = listParams[0];
                List<string> listReturn = new List<string>();

                //异步模式（过指定的时间超时）
                List<string> args = new List<string>();
                args.Add(strPBXAddress);
                if (mService00Helper == null)
                {
                    mService00Helper = new Service00Helper();
                    mService00Helper.Debug += mService00Helper_Debug;
                    mService00Helper.Start();
                }
                mService00Helper.HostAddress = request.ServerHost;
                mService00Helper.HostPort = request.ServerPort;
                optReturn = mService00Helper.DoOperation(RequestCommand.GET_CTI_SERVICENAME, args);
                if (!optReturn.Result)
                {
                    return optReturn;
                }

                string strMessage = optReturn.Data.ToString();
                if (!string.IsNullOrEmpty(strMessage))
                {
                    if (strMessage.StartsWith("Error"))
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_FAIL;
                        optReturn.Message = string.Format("{0}", strMessage);
                        return optReturn;
                    }
                    string[] serviceNames = strMessage.Split(new[] { "\r\n" }, StringSplitOptions.None);
                    if (serviceNames.Length > 0)
                    {
                        for (int i = 0; i < serviceNames.Length; i++)
                        {
                            string name = serviceNames[i];
                            if (string.IsNullOrEmpty(name)) { continue; }
                            string[] arrInfos = name.Split(new[] { ConstValue.SPLITER_CHAR },
                                StringSplitOptions.None);
                            string strName = string.Empty;
                            if (arrInfos.Length > 0)
                            {
                                strName = arrInfos[0];
                            }
                            string strInfo = string.Format("{0}", strName);
                            listReturn.Add(strInfo);
                        }
                    }
                }
                optReturn.Data = listReturn;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }

        private OperationReturn SetResourceChanged(ServerRequestInfo request)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                List<string> listParams = request.ListData;
                if (listParams.Count < 1)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Count parameter invalid");
                    return optReturn;
                }
                string strCount = listParams[0];
                int intCount;
                if (!int.TryParse(strCount, out intCount))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Parameter count invalid");
                    return optReturn;
                }
                if (intCount + 1 > listParams.Count)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Count parameter invalid");
                    return optReturn;
                }
                List<NotifyObjectInfo> listNotifyObjects = new List<NotifyObjectInfo>();
                for (int i = 0; i < intCount; i++)
                {
                    string strServerInfo = listParams[i + 1];
                    optReturn = XMLHelper.DeserializeObject<NotifyObjectInfo>(strServerInfo);
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                    NotifyObjectInfo serverInfo = optReturn.Data as NotifyObjectInfo;
                    if (serverInfo == null)
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_OBJECT_NULL;
                        optReturn.Message = string.Format("ServerInfo is null");
                        return optReturn;
                    }
                    listNotifyObjects.Add(serverInfo);
                }
                List<string> listReturn = new List<string>();

                //异步模式（过指定的时间超时）
                List<string> args = new List<string>();
                for (int i = 0; i < listNotifyObjects.Count; i++)
                {
                    args.Add(string.Format("{0}:8009", listNotifyObjects[i].Address));
                }
                if (mService00Helper == null)
                {
                    mService00Helper = new Service00Helper();
                    mService00Helper.Debug += mService00Helper_Debug;
                    mService00Helper.Start();
                }
                mService00Helper.HostAddress = request.ServerHost;
                mService00Helper.HostPort = request.ServerPort;
                optReturn = mService00Helper.DoOperation(RequestCommand.SET_RESOURCE_CHANGED, args);
                if (!optReturn.Result)
                {
                    return optReturn;
                }

                string strMessage = optReturn.Data.ToString();
                if (!string.IsNullOrEmpty(strMessage))
                {
                    if (strMessage.StartsWith("Error"))
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_FAIL;
                        optReturn.Message = string.Format("{0}", strMessage);
                        return optReturn;
                    }
                    optReturn.Message = strMessage;
                }
                optReturn.Data = listReturn;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }

        private OperationReturn GetServerInformation(ServerRequestInfo request, string sendMessage)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            TcpClient tcpClient = null;
            SslStream sslStream = null;
            try
            {
                tcpClient = new TcpClient(AddressFamily.InterNetwork);
                tcpClient.Connect(request.ServerHost, request.ServerPort);
                sslStream = new SslStream(tcpClient.GetStream(), false, (sender, cert, chain, errs) => true, null);
                sslStream.AuthenticateAsClient("VoiceCyber.PF", null, SslProtocols.Default, false);
                sslStream.Write(Encoding.UTF8.GetBytes(sendMessage));
                sslStream.Flush();
                int intReaded;
                byte[] buffer = new byte[1024];
                string strReturn = string.Empty;
                int intCount = 0;
                do
                {
                    intReaded = sslStream.Read(buffer, 0, 1024);
                    if (intReaded > 0)
                    {
                        string strMessage = Encoding.UTF8.GetString(buffer, 0, intReaded);
                        strReturn += strMessage;
                        if (strReturn.EndsWith(string.Format("{0}End{0}\r\n", ConstValue.SPLITER_CHAR)))
                        {
                            strReturn = strReturn.Substring(0, strReturn.Length - 7);
                            if (strReturn.EndsWith("\r\n"))
                            {
                                strReturn = strReturn.Substring(0, strReturn.Length - 2);
                            }
                            break;
                        }
                    }
                    intCount++;
                    //超时
                    if (intCount > 100)
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_TIMEOUT;
                        optReturn.Message = string.Format("RecieveMessage timeout");
                        sslStream.Close();
                        tcpClient.Close();
                        return optReturn;
                    }
                    Thread.Sleep(100);
                } while (intReaded > 0);
                sslStream.Close();
                tcpClient.Close();
                if (strReturn.StartsWith("Error"))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_FAIL;
                    optReturn.Message = string.Format("Server return error message. Please reference to log for detail");
                    sslStream.Close();
                    tcpClient.Close();
                    return optReturn;
                }
                optReturn.Data = strReturn;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
            }
            finally
            {
                if (sslStream != null) { sslStream.Close(); }
                if (tcpClient != null) { tcpClient.Close(); }
            }
            return optReturn;
        }


        #region Encryption and Decryption

        public static string EncryptString(string strSource)
        {
            string strReturn = string.Empty;
            string strTemp;
            do
            {
                if (strSource.Length > 128)
                {
                    strTemp = strSource.Substring(0, 128);
                    strSource = strSource.Substring(128, strSource.Length - 128);
                }
                else
                {
                    strTemp = strSource;
                    strSource = string.Empty;
                }
                strReturn += EncryptionAndDecryption.EncryptDecryptString(strTemp,
                    CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004),
                    EncryptionAndDecryption.UMPKeyAndIVType.M004);
            } while (strSource.Length > 0);
            return strReturn;
        }

        public static string DecryptString(string strSource)
        {
            string strReturn = string.Empty;
            string strTemp;
            do
            {
                if (strSource.Length > 512)
                {
                    strTemp = strSource.Substring(0, 512);
                    strSource = strSource.Substring(512, strSource.Length - 512);
                }
                else
                {
                    strTemp = strSource;
                    strSource = string.Empty;
                }
                strReturn += EncryptionAndDecryption.EncryptDecryptString(strTemp,
                    CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104),
                    EncryptionAndDecryption.UMPKeyAndIVType.M104);
            } while (strSource.Length > 0);
            return strReturn;
        }

        public static string CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType aKeyIVID)
        {
            string lStrReturn;
            int LIntRand;
            Random lRandom = new Random();
            string LStrTemp;

            try
            {
                lStrReturn = DateTime.Now.ToString("yyyyMMddHHmmss");
                LIntRand = lRandom.Next(0, 14);
                LStrTemp = LIntRand.ToString("00");
                lStrReturn = lStrReturn.Insert(LIntRand, "VCT");
                LIntRand = lRandom.Next(0, 17);
                LStrTemp += LIntRand.ToString("00");
                lStrReturn = lStrReturn.Insert(LIntRand, "UMP");
                LIntRand = lRandom.Next(0, 20);
                LStrTemp += LIntRand.ToString("00");
                lStrReturn = lStrReturn.Insert(LIntRand, ((int)aKeyIVID).ToString("000"));

                lStrReturn = EncryptionAndDecryption.EncryptStringY(LStrTemp + lStrReturn);
            }
            catch { lStrReturn = string.Empty; }

            return lStrReturn;
        }

        #endregion


        #region LogOperator

        private void mService00Helper_Debug(string msg)
        {
            WriteOperationLog(string.Format("Service00Helper\t{0}", msg));
        }

        private void CreateLogOperator()
        {
            mLogOperator = new LogOperator();
            mLogOperator.LogPath =
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "UMP\\Wcf11102\\Logs");
            mLogOperator.Start();
        }

        private void WriteOperationLog(string msg)
        {
            if (mLogOperator != null)
            {
                mLogOperator.WriteLog(LogMode.Info, "Wcf11102", msg);
            }
        }

        #endregion


        #region Dispose

        public void Dispose()
        {
            if (mService00Helper != null)
            {
                mService00Helper.Stop();
            }
            if (mLogOperator != null)
            {
                mLogOperator.Stop();
            }
        }

        #endregion


    }
}
