using PFShareClassesS;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.Security;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Xml;
using VoiceCyber.Common;

namespace UMPService00
{
    public class Common
    {
        /// <summary>
        /// 创建认证码
        /// </summary>
        /// <param name="AKeyIVID"></param>
        /// <returns></returns>
        public static string CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType AKeyIVID)
        {
            string LStrReturn = string.Empty;
            int LIntRand = 0;
            string LStrTemp = string.Empty;

            try
            {
                Random LRandom = new Random();
                LStrReturn = DateTime.Now.ToString("yyyyMMddHHmmss");
                LIntRand = LRandom.Next(0, 14);
                LStrTemp = LIntRand.ToString("00");
                LStrReturn = LStrReturn.Insert(LIntRand, "VCT");
                LIntRand = LRandom.Next(0, 17);
                LStrTemp += LIntRand.ToString("00");
                LStrReturn = LStrReturn.Insert(LIntRand, "UMP");
                LIntRand = LRandom.Next(0, 20);
                LStrTemp += LIntRand.ToString("00");
                LStrReturn = LStrReturn.Insert(LIntRand, ((int)AKeyIVID).ToString("000"));

                LStrReturn = EncryptionAndDecryption.EncryptStringY(LStrTemp + LStrReturn);
            }
            catch { LStrReturn = string.Empty; }
            return LStrReturn;
        }

        public static string AscCodeToChr(int AsciiCode)
        {
            System.Text.ASCIIEncoding asciiEncoding = new System.Text.ASCIIEncoding();
            byte[] byteArray = new byte[] { (byte)AsciiCode };
            string strCharacter = asciiEncoding.GetString(byteArray);
            return (strCharacter);
        }

        /// <summary>
        /// 获取当前服务安装的路径
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentBaseDirectory()
        {
            string LStrReturn = string.Empty;

            try
            {
                //LStrReturn = AppDomain.CurrentDomain.BaseDirectory;
                //string[] LStrDirectoryArray = LStrReturn.Split('\\');
                //LStrReturn = string.Empty;
                //foreach (string LStrDirectorySingle in LStrDirectoryArray)
                //{
                //    if (LStrDirectorySingle == "WinServices") { break; }
                //    LStrReturn += LStrDirectorySingle + "\\";
                //}
                string path = AppDomain.CurrentDomain.BaseDirectory;
                path = path.Substring(0, path.LastIndexOf("\\"));
                path = path.Substring(0, path.LastIndexOf("\\"));
                LStrReturn = path;
                UMPService00.WriteLog(LogMode.Error, "Read AppDomain.CurrentDomain.BaseDirectory : " + LStrReturn);
            }
            catch (Exception ex)
            {
                LStrReturn = string.Empty;
                UMPService00.WriteLog(LogMode.Error, "Read AppDomain.CurrentDomain.BaseDirectory Failed\n" + ex.ToString());
            }

            return LStrReturn;
        }

        /// <summary>
        /// 获得本机的IP地址
        /// </summary>
        /// <returns></returns>
        public static List<IPAddress> GetLocalMachineIP()
        {
            string strHostName = Dns.GetHostName(); //得到本机的主机名
            IPHostEntry ipEntry = Dns.GetHostEntry(strHostName);
            return ipEntry.AddressList.ToList();
        }

        /// <summary>
        /// 拷贝文件 尝试拷贝5次 通过比较最后修改时间来判断是否拷贝成功 每次拷贝间隔2秒
        /// </summary>
        /// <param name="strSrcDir">源文件路径</param>
        /// <param name="strTargetDir">目标文件路径</param>
        public static bool CopyFile(string strSrcDir, string strTargetDir)
        {
            string strTarget = strTargetDir.Substring(0, strTargetDir.LastIndexOf('\\'));
            DirectoryInfo dir = new DirectoryInfo(strTarget);

            if (!dir.Exists)
            {
                UMPService00.WriteLog(LogMode.Warn,"strTargetDir  not exists");
                dir.Create();
            }
           
            FileInfo targetFile = null;
            FileInfo sourceFile = null;
            for (int i = 0; i < 5; i++)
            {
                sourceFile = new FileInfo(strSrcDir);
                targetFile = new FileInfo(strTargetDir);
                if (targetFile.Exists)
                {
                    if (sourceFile.LastWriteTime == targetFile.LastWriteTime)
                    {
                        break;
                    }
                    else
                    {
                        File.Copy(strSrcDir, strTargetDir, true);
                    }
                }
                else
                {
                    File.Copy(strSrcDir, strTargetDir, true);
                }
                Thread.Sleep(2000);
            }
            sourceFile = new FileInfo(strSrcDir);
            targetFile = new FileInfo(strTargetDir);
            if (targetFile.Exists)
            {
                if (targetFile.LastWriteTime == sourceFile.LastWriteTime)
                {
                    File.Delete(strSrcDir);
                    return true;
                }
                else
                {
                    File.Delete(strSrcDir);
                    return false;
                }
            }
            else
            {
                File.Delete(strSrcDir);
                return false;
            }

        }

        /// <summary>
        /// 在文件不存在时创建xml文档 
        /// </summary>
        /// <param name="strPath">文件路径（如果没有 就创建）</param>
        /// <param name="strFileName">文件名</param>
        /// <param name="strRootEleName">根节点名</param>
        /// <returns></returns>
        public static XmlDocument CreateXmlDocumentIfNotExists(string strPath, string strFileName,string strRootEleName)
        {
            DirectoryInfo dir = new DirectoryInfo(strPath);
            if (!dir.Exists)
            {
                dir.Create();
            }
            XmlDocument xmlDocument = null;
            try
            {
                List<string> lstFiles = Directory.GetFiles(strPath).ToList();
                string strXmlFilePath = strPath + @"\" + strFileName;
                if (!lstFiles.Contains(strXmlFilePath))
                {
                    xmlDocument = new XmlDocument();
                    XmlNode root = xmlDocument.CreateNode(XmlNodeType.XmlDeclaration, "", "");
                    xmlDocument.AppendChild(root);
                    XmlElement ele = xmlDocument.CreateElement(strRootEleName);
                    xmlDocument.AppendChild(ele);
                    xmlDocument.Save(strXmlFilePath);
                }
                else
                {
                    xmlDocument = new XmlDocument();
                    xmlDocument.Load(strXmlFilePath);
                }
                return xmlDocument;
            }
            catch(Exception ex)
            {
                UMPService00.WriteLog(LogMode.Error,"Load xml error :" + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// 重启服务
        /// </summary>
        /// <param name="strServiceName"></param>
        public static bool RestartService(string strServiceName)
        {
            try
            {
                InvokeMethodOptions OperationMethodOptions = new InvokeMethodOptions(null, new TimeSpan(0, 10, 0));
                ManagementBaseObject LOutStopService;

                ManagementObjectCollection ObjectCollection;
                ObjectCollection = GetServiceCollection(string.Format("SELECT * FROM Win32_Service WHERE Name = '{0}'", strServiceName));
                int i = ObjectCollection.Count;
                string LStrProcessId = string.Empty;
                string LStrDisplayName = string.Empty;
                int LIntReturnValue = -1;

                foreach (ManagementObject SingleCollenction in ObjectCollection)
                {
                    LStrProcessId = SingleCollenction["ProcessId"].ToString();
                    LStrDisplayName = SingleCollenction["DisplayName"].ToString();
                    LOutStopService = SingleCollenction.InvokeMethod("StopService", null, OperationMethodOptions);
                    LIntReturnValue = int.Parse(LOutStopService["ReturnValue"].ToString());
                    UMPService00.WriteLog("StopService ReturnValue = " + LIntReturnValue.ToString());
                    if (LIntReturnValue != 0)
                    {
                        int LIntWaitCount = 0;
                        string LStrServiceCurrentStatus = GetComputerServiceStatus(strServiceName);
                        if (string.IsNullOrEmpty(LStrServiceCurrentStatus))
                        {
                            UMPService00.WriteLog("GetComputerServiceStatus null");
                            return false;
                        }
                        while (LStrServiceCurrentStatus != "0" && LIntWaitCount < 30)
                        {
                            System.Threading.Thread.Sleep(2000);
                            LIntWaitCount += 1;
                            LStrServiceCurrentStatus = GetComputerServiceStatus(strServiceName);
                            UMPService00.WriteLog("GetComputerServiceStatus LStrServiceCurrentStatus = " + LStrServiceCurrentStatus.ToString());
                            if (string.IsNullOrEmpty(LStrServiceCurrentStatus))
                            {
                                UMPService00.WriteLog("GetComputerServiceStatus LStrServiceCurrentStatus is null ,return false");
                                return false;
                            }
                        }
                        if (LStrServiceCurrentStatus == "0") { LIntReturnValue = 0; }
                        else { KillProcess(LStrProcessId); LIntReturnValue = 0; }
                        
                        //for (int iStartCount = 0; iStartCount < 30; iStartCount++)
                        //{
                        //    Thread.Sleep(2000);
                        //    try
                        //    {
                        //        UMPService00.IEventLog.WriteEntry("start service begin " + iStartCount.ToString());
                        //        LOutStopService = SingleCollenction.InvokeMethod("StartService", null, OperationMethodOptions);
                        //        LIntReturnValue = int.Parse(LOutStopService["ReturnValue"].ToString());
                        //        UMPService00.IEventLog.WriteEntry("Restart service " + strServiceName + ", " + iStartCount.ToString() + ", result = " + LIntReturnValue.ToString(), EventLogEntryType.Warning);
                        //        if (LIntReturnValue == 0)
                        //        {
                        //            return true;
                        //        }
                        //    }
                        //    catch (Exception ex)
                        //    {
                        //        UMPService00.IEventLog.WriteEntry("start service exception : " + ex.Message);
                        //    }
                        //}
                    }
                    UMPService00.WriteLog("stop server success", EventLogEntryType.Information);
                    for (int iStartCount = 0; iStartCount < 30; iStartCount++)
                    {
                        Thread.Sleep(2000);
                        try
                        {
                            UMPService00.WriteLog("start service begin " + iStartCount.ToString());
                            LOutStopService = SingleCollenction.InvokeMethod("StartService", null, OperationMethodOptions);
                            LIntReturnValue = int.Parse(LOutStopService["ReturnValue"].ToString());
                            UMPService00.WriteLog(LogMode.Error,"Restart service " + strServiceName + ", " + iStartCount.ToString() + ", result = " + LIntReturnValue.ToString());
                            if (LIntReturnValue == 0)
                            {
                                return true;
                            }
                        }
                        catch (Exception ex)
                        {
                            UMPService00.WriteLog("start service exception : " + ex.Message);
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                UMPService00.WriteLog(EventLogEntryType.Error,"Restart service " + strServiceName + " error : " + ex.Message);
                return false;
            }
           
        }

        /// <summary>
        /// 杀进程
        /// </summary>
        /// <param name="AStrProcessId"></param>
        private static void KillProcess(string AStrProcessId)
        {
            try
            {
                ManagementObjectCollection ObjectCollection;
                ObjectCollection = GetServiceCollection("Select * from win32_process where ProcessID = '" + AStrProcessId + "'");
                foreach (ManagementObject SingleCollenction in ObjectCollection)
                {
                    SingleCollenction.InvokeMethod("Terminate", null);
                }
            }
            catch (Exception ex)
            {
                UMPService00.WriteLog(LogMode.Error, "KillProcess error : " + ex.Message);
            }
        }

        private static string GetComputerServiceStatus(string AStrServiceName)
        {
            string LStrSatus = string.Empty;

            try
            {
                ManagementObjectCollection ObjectCollection;
                ObjectCollection = GetServiceCollection("SELECT * FROM Win32_Service WHERE Name = '" + AStrServiceName + "'");
                foreach (ManagementObject ObjectSingleReturn in ObjectCollection)
                {
                    try
                    {
                        if (ObjectSingleReturn["Started"].Equals(true))
                            LStrSatus = "1";
                        else
                            LStrSatus = "0";
                        break;
                    }
                    catch (Exception ex)
                    {
                        LStrSatus = string.Empty;
                    }
                }
            }
            catch (Exception ex)
            {
                LStrSatus = string.Empty;
                UMPService00.WriteLog(LogMode.Error,"GetComputerServiceStatus error : " + ex.Message);
            }
            return LStrSatus;
        }

        private static ManagementObjectCollection GetServiceCollection(string AStrQuery)
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

        #region DEC相关函数
        public static _TAG_NETPACK_DISTINGUISHHEAD_VER1 CreatePack(byte strVersion, _TAG_NETPACK_DISTINGUISHHEAD DISTINGUISHHEAD, ushort BaseHead,
            ushort ExtHead, _TAG_NETPACK_ENCRYPT_CONTEXT encrypt, ushort validsize, _TAG_NETPACK_MESSAGE message, ushort module
            , ushort number, byte packtype, byte sequence, Int64 source, ushort state, Int64 target)
        {
            //计算识别头大小
            short iDistHeadSize = (short)Marshal.SizeOf(DISTINGUISHHEAD);
            //计算基本包头大小
            ushort baseheadsize = 0;
            switch (BaseHead)
            {
                case DecDefine.NETPACK_BASETYPE_NOTHING:
                    baseheadsize = 0;
                    break;
                case DecDefine.NETPACK_BASETYPE_CONNECT_ERROR:
                    baseheadsize = (ushort)Marshal.SizeOf(typeof(_TAG_NETPACK_BASEHEAD_ERROR));
                    break;
                case DecDefine.NETPACK_BASETYPE_CONNECT_HELLO:
                    baseheadsize = (ushort)Marshal.SizeOf(typeof(_TAG_NETPACK_BASEHEAD_CONNECT_HELLO));
                    break;
                case DecDefine.NETPACK_BASETYPE_CONNECT_WELCOME:
                    baseheadsize = (ushort)Marshal.SizeOf(typeof(_TAG_NETPACK_BASEHEAD_CONNECT_WELCOME));
                    break;
                case DecDefine.NETPACK_BASETYPE_CONNECT_LOGON:
                    baseheadsize = (ushort)Marshal.SizeOf(typeof(_TAG_NETPACK_BASEHEAD_CONNECT_LOGON));
                    break;
                case DecDefine.NETPACK_BASETYPE_CONNECT_AUTHEN:
                    baseheadsize = (ushort)Marshal.SizeOf(typeof(_TAG_NETPACK_BASETYPE_CONNECT_AUTHEN));
                    break;
                case DecDefine.NETPACK_BASETYPE_REQ_ADDSUBSCRIBE:
                    baseheadsize = (ushort)Marshal.SizeOf(typeof(_TAG_NETPACK_BASEHEAD_REQ_ADDSUBSCRIBE));
                    break;
                case DecDefine.NETPACK_BASETYPE_APPLICATION_VER1:
                    baseheadsize = (ushort)Marshal.SizeOf(typeof(NETPACK_BASEHEAD_APPLICATION_VER1));
                    break;
            }

            //计算扩展包头大小
            short extheadsize = (short)(ExtHead != DecDefine.NETPACK_EXTTYPE_NOTHING ? Marshal.SizeOf(ExtHead) : 0);
            // 计算包头部分大小(识别头+基本包头+扩展包头)
            int iHeadSize = iDistHeadSize + baseheadsize + extheadsize;
            //UMPService00.IEventLog.WriteEntry("iHeadSize = " + iHeadSize, EventLogEntryType.Warning);
            byte encrypeMethod = encrypt != null ? encrypt._encrypt : DecDefine.NETPACK_ENCRYPT_NOTHING;
            ushort dataSize = Common.get_netpack_encrypt_storesize(encrypeMethod, validsize);
           // UMPService00.IEventLog.WriteEntry("dataSize = " + dataSize, EventLogEntryType.Warning);
            short packsize = (short)(iHeadSize + dataSize);
           // UMPService00.IEventLog.WriteEntry("packsize = " + packsize, EventLogEntryType.Warning);
            _TAG_NETPACK_DISTINGUISHHEAD_VER1 netPack = new _TAG_NETPACK_DISTINGUISHHEAD_VER1();
            netPack._basehead = BaseHead;
            netPack._basesize = baseheadsize;
            netPack._dist = DISTINGUISHHEAD;
            netPack._exthead = ExtHead;
            netPack._extsize = (ushort)extheadsize;
            netPack._followsize = (ushort)(packsize - iDistHeadSize);
            //UMPService00.IEventLog.WriteEntry("_followsize = " + netPack._followsize, EventLogEntryType.Warning);
            netPack._message = message;
            netPack._moduleid = module;
            netPack._number = number;
            netPack._packtype = packtype;
            netPack._sequence = sequence;
            netPack._source = source;
            netPack._datasize = dataSize;
            netPack._state = state;
            netPack._target = target;
            netPack._dist._flag = new char[2];
            netPack._dist._flag[0] = DecDefine.NETPACK_FLAG[0];
            netPack._dist._flag[1] = DecDefine.NETPACK_FLAG[1];
            netPack._dist._version = strVersion;
            netPack._dist._cbsize = (byte)((char)Marshal.SizeOf(typeof(_TAG_NETPACK_DISTINGUISHHEAD_VER1)));
            netPack._timestamp = (long)(System.DateTime.Now.ToUniversalTime().Subtract(DateTime.Parse("1970-1-1")).TotalMilliseconds * 10);
            return netPack;
        }

        public static ushort get_netpack_encrypt_storesize(byte encrypt, ushort validsize)
        {
            switch (encrypt)
            {
                case DecDefine.NETPACK_ENCRYPT_AES_128_CBC:
                case DecDefine.NETPACK_ENCRYPT_AES_256_CBC:
                    return (ushort)(validsize + (((validsize % DecDefine.AES_BLOCK_SIZE) != 0) ? (DecDefine.AES_BLOCK_SIZE - (validsize % DecDefine.AES_BLOCK_SIZE)) : 0));
                default:
                    return validsize;

            }
        }

        /// <summary>
        /// Convert Structure to Bytes
        /// </summary>
        /// <param name="objStruct">Struct object</param>
        /// <returns></returns>
        public static byte[] StructToBytes(object objStruct)
        {
            //得到结构体的大小
            int size = Marshal.SizeOf(objStruct);
            //创建byte数组
            byte[] bytes = new byte[size];
            //分配结构体大小的内存空间
            IntPtr structPtr = Marshal.AllocHGlobal(size);
            //将结构体拷到分配好的内存空间
            Marshal.StructureToPtr(objStruct, structPtr, false);
            //从内存空间拷到byte数组
            Marshal.Copy(structPtr, bytes, 0, size);
            //释放内存空间
            Marshal.FreeHGlobal(structPtr);
            //返回byte数组
            return bytes;

        }

        /// <summary>
        /// Convert Bytes to Structure
        /// </summary>
        /// <param name="data">Data in byte</param>
        /// <param name="type">Structure type</param>
        /// <returns></returns>
        public static object BytesToStruct(byte[] data, Type type)
        {
            //得到结构体的大小
            int size = Marshal.SizeOf(type);
            //byte数组长度小于结构体的大小
            if (size > data.Length)
            {
                //返回空
                return null;
            }
            //分配结构体大小的内存空间
            IntPtr structPtr = Marshal.AllocHGlobal(size);
            //将byte数组拷到分配好的内存空间
            Marshal.Copy(data, 0, structPtr, size);
            //将内存空间转换为目标结构体
            object obj = Marshal.PtrToStructure(structPtr, type);
            //释放内存空间
            Marshal.FreeHGlobal(structPtr);
            //返回结构体
            return obj;
        }

        /// <summary>
        /// 创建message包
        /// </summary>
        /// <returns></returns>
        public static _TAG_NETPACK_MESSAGE CreateMessage(Int64 adress, Int64 identify)
        {
            _TAG_NETPACK_MESSAGE message = new _TAG_NETPACK_MESSAGE();
            message._adressLong = adress;
            message._identifyLong = identify;
            return message;
        }

        /// <summary>
        /// 创建message包
        /// </summary>
        /// <param name="sourceModule"></param>
        /// <param name="sourceNumber"></param>
        /// <param name="targetModule"></param>
        /// <param name="targetNumber"></param>
        /// <param name="smallType"></param>
        /// <param name="middleType"></param>
        /// <param name="largeType"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        public static _TAG_NETPACK_MESSAGE CreateMessage(ushort sourceModule, ushort sourceNumber, ushort targetModule
            , ushort targetNumber, ushort smallType, ushort middleType, ushort largeType, ushort number)
        {
            return CreateMessage(CreateMsgAdress(sourceModule, sourceNumber, targetModule, targetNumber),
                 CreateMsgIdentify(largeType, middleType, smallType, number));
        }

        public static Int64 CreateMsgAdress(ushort source_module, ushort source_number, ushort target_module, ushort target_number)
        {
            Int64 i = (((Int64)source_module) << 48) | (((Int64)source_number) << 32) | (((Int64)target_module) << 16) | target_number;
            return (((Int64)source_module) << 48) | (((Int64)source_number) << 32) | (((Int64)target_module) << 16) | target_number;
        }

        public static Int64 CreateMsgIdentify(ushort large_type, ushort middle_type, ushort small_type, ushort number)
        {
            Int64 i = (((Int64)large_type) << 48) | (((Int64)middle_type) << 32) | (((Int64)small_type) << 16) | number;
            return (((Int64)large_type) << 48) | (((Int64)middle_type) << 32) | (((Int64)small_type) << 16) | number;
        }
        #endregion

        #region LicenseServer部分
        /// <summary>
        /// 根据Session计算认证码
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public static string GetValidication(string session)
        {
            string strValidication;
            string strSource;

            strSource = ConvertHexToString(session);
            char[] charSource = strSource.ToCharArray();
            byte[] byteSource = new byte[charSource.Length];
            for (int lIntLoop = 0; lIntLoop < charSource.Length; lIntLoop++) { byteSource[lIntLoop] = Convert.ToByte(charSource[lIntLoop]); }
            int intOffset = byteSource[0] & 0x001f;
            byte byteMask = (byte)(byteSource[intOffset] ^ intOffset);
            byte[] byteValidication = new byte[32];
            for (int i = 0; i < 32; i++) { byteValidication[i] = (byte)(byteSource[i] ^ byteMask); }
            strValidication = ConvertByteToHexStr(byteValidication);

            return strValidication;
        }

        /// <summary>
        /// 将十六进制值转成字符串
        /// </summary>
        /// <param name="hexValue"></param>
        /// <returns></returns>
        public static string ConvertHexToString(string hexValue)
        {
            string strReturn = "";
            string strSource;

            strSource = hexValue;
            // strSource = strSource.Trim('"');
            while (strSource.Length > 0)
            {
                strReturn += Convert.ToChar(Convert.ToUInt32(strSource.Substring(0, 2), 16)).ToString(CultureInfo.InvariantCulture);
                strSource = strSource.Substring(2);
            }
            return strReturn;
        }

        /// <summary>
        /// 字节数组转成十六进制值
        /// </summary>
        /// <param name="byteValue"></param>
        /// <returns></returns>
        public static string ConvertByteToHexStr(byte[] byteValue)
        {
            string strReturn = "";

            if (byteValue != null)
            {
                for (int i = 0; i < byteValue.Length; i++) { strReturn += byteValue[i].ToString("X2"); }
            }

            return strReturn;
        }

        public static bool ServerValidation(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            //为对服务器证书验证
            if (certificate != null)
            {
                return true;
            }
            return true;
        }
        #endregion

        #region 加解密部分
          public static string DecodeEncryptValue(string strValue)
        {
            string strReturn = string.Empty;
            //加密的(以连续三个char27开头）
            if (strValue.StartsWith(string.Format("{0}{0}{0}", AscCodeToChr(27))))
            {
                strValue = strValue.Substring(3);
                string[] arrContent = strValue.Split(new[] { AscCodeToChr(27) }, StringSplitOptions.None);
                string strVersion = string.Empty, strMode = string.Empty, strPass = strValue;
                if (arrContent.Length > 0)
                {
                    strVersion = arrContent[0];
                }
                if (arrContent.Length > 1)
                {
                    strMode = arrContent[1];
                }
                if (arrContent.Length > 2)
                {
                    strPass = arrContent[2];
                }
                if (strVersion == "2" && strMode == "hex")
                {
                    strReturn = DecryptFromDB(strPass);
                }
                else if (strVersion == "3" && strMode == "hex")
                {
                    strReturn = DecryptFromDB(strPass);
                }
                else
                {
                    strReturn = strPass;
                }
            }
            else
            {
                strReturn = strValue;
            }

            return strReturn;
        }

          public static string DecryptFromDB(string strSource)
          {
              string strReturn = EncryptionAndDecryption.EncryptDecryptString(strSource,
                CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M102),
                EncryptionAndDecryption.UMPKeyAndIVType.M102);
              return strReturn;
          }
        #endregion
    }
}
