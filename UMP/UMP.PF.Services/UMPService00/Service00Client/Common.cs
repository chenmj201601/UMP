using PFShareClassesS;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Xml;

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

        public static XmlDocument CreateXmlDocumentIfNotExists(string strPath, string strFileName, string strRootEleName)
        {
            //if (strFileName == "UMP.Server.01.xml")
            //{
            //    UMPService00.IEventLog.WriteEntry("strPath = " + strPath + "; strFileName = " + strFileName, EventLogEntryType.Warning);
            //}
            DirectoryInfo dir = new DirectoryInfo(strPath);
            if (!dir.Exists)
            {
                //if (strFileName == "UMP.Server.01.xml")
                //{
                //    UMPService00.IEventLog.WriteEntry("dir is not Exists", EventLogEntryType.Warning);
                //}
                dir.Create();
            }
            XmlDocument xmlDocument = null;
            try
            {
                List<string> lstFiles = Directory.GetFiles(strPath).ToList();
                string strXmlFilePath = strPath + @"\" + strFileName;
                //if (strFileName == "UMP.Server.01.xml")
                //{
                //    UMPService00.IEventLog.WriteEntry("strXmlFilePath = " + strXmlFilePath, EventLogEntryType.Warning);
                //}
                if (!lstFiles.Contains(strXmlFilePath))
                {
                    //if (strFileName == "UMP.Server.01.xml")
                    //{
                    //    string strMsg = string.Empty;
                    //    foreach (string str in lstFiles)
                    //    {
                    //        strMsg += str + ";\r\n";
                    //    }
                    //    UMPService00.IEventLog.WriteEntry("lstFiles not  Contains  " + strXmlFilePath + "\r\nstrMsg = " + strMsg, EventLogEntryType.Warning);
                    //}
                    xmlDocument = new XmlDocument();
                    XmlNode root = xmlDocument.CreateNode(XmlNodeType.XmlDeclaration, "", "");
                    xmlDocument.AppendChild(root);
                    XmlElement ele = xmlDocument.CreateElement(strRootEleName);
                    xmlDocument.AppendChild(ele);
                    xmlDocument.Save(strXmlFilePath);
                }
                else
                {
                    //if (strFileName == "UMP.Server.01.xml")
                    //{
                    //    UMPService00.IEventLog.WriteEntry("exists file " + strXmlFilePath, EventLogEntryType.Warning);
                    //}
                    xmlDocument = new XmlDocument();
                    xmlDocument.Load(strXmlFilePath);
                }
                return xmlDocument;
            }
            catch(Exception ex)
            {
                return null;
            }
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
                LStrReturn = AppDomain.CurrentDomain.BaseDirectory;
                string[] LStrDirectoryArray = LStrReturn.Split('\\');
                LStrReturn = string.Empty;
                foreach (string LStrDirectorySingle in LStrDirectoryArray)
                {
                    if (LStrDirectorySingle == "WinServices") { break; }
                    LStrReturn += LStrDirectorySingle + "\\";
                }
            }
            catch (Exception ex)
            {
                LStrReturn = string.Empty;
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

        #region DEC相关函数
    
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
        #endregion

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
    }
}
