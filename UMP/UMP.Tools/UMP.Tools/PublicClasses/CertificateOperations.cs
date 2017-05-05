using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace UMP.Tools.PublicClasses
{
    public static class CertificateOperations
    {
        /// <summary>
        /// 安装证书
        /// </summary>
        /// <param name="AStrCertificateFile">证书文件完整路径和名称</param>
        /// <param name="AStrImportPassword">安装证书的密码</param>
        /// <param name="AStoreNameArea">存储区的名称</param>
        /// <param name="AStoreLocationPositon">存储区的位置</param>
        /// <param name="AStrReturn">当返回True时，返回该证书的哈希值（十六进制）；当返回False时，返回安装时的错误信息</param>
        /// <returns></returns>
        public static bool InstallCertificate(string AStrCertificateFile, string AStrImportPassword, StoreName AStoreNameArea, StoreLocation AStoreLocationPositon, ref string AStrReturn)
        {
            bool LBoolReturn = true;

            try
            {
                AStrReturn = string.Empty;

                X509Certificate2 LX509Certificate = new X509Certificate2(AStrCertificateFile, AStrImportPassword, X509KeyStorageFlags.Exportable | X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.MachineKeySet);
                AStrReturn = LX509Certificate.GetCertHashString();

                X509Store LX509Store = null;
                LX509Store = new X509Store(AStoreNameArea, AStoreLocationPositon);
                LX509Store.Open(OpenFlags.MaxAllowed);
                LX509Store.Remove(LX509Certificate);
                LX509Store.Add(LX509Certificate);
                LX509Store.Close();
                LX509Store = null;
            }
            catch (Exception ex)
            {
                LBoolReturn = false;
                AStrReturn = "C001003" + AscCodeToChr(27) + ex.Message;
            }

            return LBoolReturn;
        }

        /// <summary>
        /// 根据 机器名或IP地址 判断证书是否在 相应的存储区的名称、位置中 已经存在
        /// </summary>
        /// <param name="AStrMachineNameOrIPAddress">机器名或IP地址</param>
        /// <param name="AStoreNameArea">存储区的名称</param>
        /// <param name="AStoreLocationPositon">存储区的位置</param>
        /// <param name="AStrHashString">当返回True时，返回该证书的HashString</param>
        /// <param name="AStrReturn">Catch错误信息返回</param>
        /// <returns></returns>
        public static bool CertificateIsExist(string AStrMachineNameOrIPAddress, StoreName AStoreNameArea, StoreLocation AStoreLocationPositon, ref string AStrHashString, ref string AStrReturn)
        {
            bool LBoolReturn = false;
            string LStrCNOptions = string.Empty;

            try
            {
                AStrHashString = string.Empty;
                AStrReturn = string.Empty;

                LStrCNOptions = "CN=" + AStrMachineNameOrIPAddress;
                X509Store LX509Store = new X509Store(AStoreNameArea, AStoreLocationPositon);
                LX509Store.Open(OpenFlags.ReadOnly);
                foreach (X509Certificate2 LX509CertificateSingle in LX509Store.Certificates)
                {
                    if (LX509CertificateSingle.Subject.IndexOf(LStrCNOptions) >= 0)
                    {
                        AStrHashString = LX509CertificateSingle.GetCertHashString();
                        LBoolReturn = true;
                        break;
                    }
                }
                LX509Store.Close(); LX509Store = null;
            }
            catch (Exception ex)
            {
                LBoolReturn = false;
                AStrReturn = ex.Message;
            }

            return LBoolReturn;
        }

        /// <summary>
        /// 根据 机器名或IP地址 和 证书的HASH值 判断证书是否在 相应的存储区的名称、位置中 已经存在
        /// </summary>
        /// <param name="AStrMachineNameOrIPAddress">机器名或IP地址</param>
        /// <param name="AStrHashString">证书的HashString</param>
        /// <param name="AStoreNameArea">存储区的名称</param>
        /// <param name="AStoreLocationPositon">存储区的位置</param>
        /// <param name="AStrReturn">Catch错误信息返回</param>
        /// <returns>True:已经安装；False:未安装</returns>
        public static bool CertificateIsExist(string AStrMachineNameOrIPAddress, string AStrHashString, StoreName AStoreNameArea, StoreLocation AStoreLocationPositon, ref string AStrReturn)
        {
            bool LBoolReturn = false;
            string LStrCNOptions = string.Empty;

            try
            {
                AStrReturn = string.Empty;
                LStrCNOptions = "CN=" + AStrMachineNameOrIPAddress;
                X509Store LX509Store = new X509Store(AStoreNameArea, AStoreLocationPositon);
                LX509Store.Open(OpenFlags.ReadOnly);
                foreach (X509Certificate2 LX509CertificateSingle in LX509Store.Certificates)
                {
                    if (LX509CertificateSingle.Subject.IndexOf(LStrCNOptions) >= 0)
                    {
                        if (LX509CertificateSingle.GetCertHashString() == AStrHashString)
                        {
                            LBoolReturn = true;
                            break;
                        }
                    }
                }
                LX509Store.Close(); LX509Store = null;
            }
            catch (Exception ex)
            {
                LBoolReturn = false;
                AStrReturn = "C001001" + AscCodeToChr(27) + ex.Message;
            }

            return LBoolReturn;
        }

        /// <summary>
        /// 根据 机器名或IP地址 移除证书
        /// </summary>
        /// <param name="AStrMachineNameOrIPAddress">机器名或IP地址</param>
        /// <param name="AStoreNameArea">存储区的名称</param>
        /// <param name="AStoreLocationPositon">存储区的位置</param>
        /// <param name="AStrHashString">与该Hash值相等的不移除</param>
        /// <param name="AStrReturn">False:Catch错误信息返回；True:1-存在，0-不存在</param>
        /// <returns></returns>
        public static bool RemoveCertificates(string AStrMachineNameOrIPAddress, StoreName AStoreNameArea, StoreLocation AStoreLocationPositon, string AStrHashString, ref string AStrReturn)
        {
            bool LBoolReturn = true;
            string LStrCNOptions = string.Empty;

            try
            {
                AStrReturn = "0";

                LStrCNOptions = "CN=" + AStrMachineNameOrIPAddress;
                X509Store LX509Store = new X509Store(AStoreNameArea, AStoreLocationPositon);
                LX509Store.Open(OpenFlags.MaxAllowed);
                foreach (X509Certificate2 LX509CertificateSingle in LX509Store.Certificates)
                {
                    if (LX509CertificateSingle.Subject.IndexOf(LStrCNOptions) >= 0)
                    {
                        if (LX509CertificateSingle.GetCertHashString() == AStrHashString) { AStrReturn = "1"; continue; }
                        LX509Store.Remove(LX509CertificateSingle);
                    }
                }
                LX509Store.Close(); LX509Store = null;
            }
            catch (Exception ex)
            {
                LBoolReturn = false;
                AStrReturn = ex.Message;
            }

            return LBoolReturn;
        }

        /// <summary>
        /// 下载UMP 支持https的安全证书
        /// </summary>
        /// <param name="AStrServer">应用服务器</param>
        /// <param name="AStrPort">http端口</param>
        /// <param name="AStrReturn">失败：返回的错误信息；成功：证书完整路径</param>
        /// <returns></returns>
        public static bool DownloadCertificate(string AStrServer, string AStrPort, string AStrCertificateFileName, ref string AStrReturn)
        {
            bool LBoolReturn = true;
            HttpWebRequest LHttpWebRequest = null;
            Stream LStreamResponse = null;
            string LStrCertificateFileFullName = string.Empty;

            try
            {
                AStrReturn = string.Empty;
                LStrCertificateFileFullName = System.IO.Path.Combine(App.GStrProgramDataDirectory, @"UMP.Client\iTools\" + AStrCertificateFileName);
                if (System.IO.File.Exists(LStrCertificateFileFullName))
                {
                    System.IO.File.Delete(LStrCertificateFileFullName);
                    System.Threading.Thread.Sleep(500);
                }
                System.IO.FileStream LStreamCertificateFile = new FileStream(LStrCertificateFileFullName, FileMode.Create);
                LHttpWebRequest = (HttpWebRequest)HttpWebRequest.Create("http://" + AStrServer + ":" + AStrPort + "/Components/Certificates/" + AStrCertificateFileName);
                long LContenctLength = LHttpWebRequest.GetResponse().ContentLength;
                LHttpWebRequest.AddRange(0);

                LStreamResponse = LHttpWebRequest.GetResponse().GetResponseStream();
                byte[] LbyteRead = new byte[1024];
                int LIntReadedSize = LStreamResponse.Read(LbyteRead, 0, 1024);
                while (LIntReadedSize > 0)
                {
                    LStreamCertificateFile.Write(LbyteRead, 0, LIntReadedSize);
                    LIntReadedSize = LStreamResponse.Read(LbyteRead, 0, 1024);
                }
                LStreamCertificateFile.Close(); LStreamCertificateFile.Dispose();
                AStrReturn = LStrCertificateFileFullName;
            }
            catch (Exception ex)
            {
                LBoolReturn = false;
                AStrReturn = "C001002" + AscCodeToChr(27) + ex.Message;
            }

            return LBoolReturn;
        }


        #region 生成分割符号 AscCodeToChr
        private static string AscCodeToChr(int AsciiCode)
        {
            System.Text.ASCIIEncoding asciiEncoding = new System.Text.ASCIIEncoding();
            byte[] byteArray = new byte[] { (byte)AsciiCode };
            string strCharacter = asciiEncoding.GetString(byteArray);
            return (strCharacter);
        }
        #endregion
    }
}
