using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace UMP.MAMT.PublicClasses
{
    public static class CertificateOperations
    {
        /// <summary>
        /// 根据机器名或IP地址创建证书
        /// </summary>
        /// <param name="AStrMachineNameOrIPAddress">机器名或IP地址</param>
        /// <param name="AStrReturn">当返回True时，返回该证书的HashString，否则为创建时的错误信息</param>
        /// <returns>True / False</returns>
        public static bool CreateCertificate(string AStrMachineNameOrIPAddress, ref string AStrReturn)
        {
            bool LBoolReturn = true;
            string LStrMakecertFile = string.Empty;
            string LStrCreateArguments = string.Empty;
            string LStrCNOptions = string.Empty;

            string LStrExistHashString = string.Empty;
            string LStrCallReturn = string.Empty;

            try
            {
                AStrReturn = string.Empty;

                if (CertificateIsExist(AStrMachineNameOrIPAddress, ref LStrExistHashString, ref LStrCallReturn))
                {
                    AStrReturn = LStrExistHashString;
                    return LBoolReturn;
                }
                else
                {
                    if (!string.IsNullOrEmpty(LStrCallReturn))
                    {
                        LBoolReturn = false;
                        AStrReturn = LStrCallReturn;
                        return LBoolReturn;
                    }
                }

                LStrMakecertFile = System.IO.Path.Combine(App.GStrSiteRootFolder, @"Components\Certificates", "makecert.exe");
                LStrCNOptions = "CN=" + AStrMachineNameOrIPAddress;
                LStrCreateArguments = "-A SHA512 -R -PE -SS MY -B 01/01/2015 -E 12/31/2028 -N \"" + LStrCNOptions + "\" ";

                Process LProcessCreate = new Process();
                LProcessCreate.StartInfo.FileName = LStrMakecertFile;
                LProcessCreate.StartInfo.Arguments = LStrCreateArguments.Trim();
                LProcessCreate.StartInfo.CreateNoWindow = true;
                LProcessCreate.StartInfo.UseShellExecute = false;
                LProcessCreate.StartInfo.RedirectStandardInput = true;
                LProcessCreate.StartInfo.RedirectStandardOutput = true;
                LProcessCreate.StartInfo.RedirectStandardError = true;
                LProcessCreate.Start();
                LProcessCreate.WaitForExit();
                if (LProcessCreate.HasExited == false)
                {
                    LProcessCreate.Kill();
                }
                LProcessCreate.Dispose();

                X509Store LX509Store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
                LX509Store.Open(OpenFlags.ReadOnly);
                foreach (X509Certificate2 LX509CertificateSingle in LX509Store.Certificates)
                {
                    if (LX509CertificateSingle.Subject.IndexOf(LStrCNOptions) >= 0)
                    {
                        AStrReturn = LX509CertificateSingle.GetCertHashString();
                        break;
                    }
                }
                LX509Store.Close(); LX509Store = null;
            }
            catch (Exception ex)
            {
                LBoolReturn = false;
                AStrReturn = "CreateCertificate()\n" + ex.ToString();
            }

            return LBoolReturn;
        }

        /// <summary>
        /// 根据 器名或IP地址 判断证书是否在 当前用户的 My 区域里已经存在
        /// </summary>
        /// <param name="AStrMachineNameOrIPAddress">机器名或IP地址</param>
        /// <param name="AStrHashString">当返回True时，返回该证书的HashString</param>
        /// <param name="AStrReturn">当返回False时，返回导出时的错误信息</param>
        /// <returns>True / False</returns>
        public static bool CertificateIsExist(string AStrMachineNameOrIPAddress, ref string AStrHashString, ref string AStrReturn)
        {
            bool LBoolReturn = false;
            string LStrCNOptions = string.Empty;

            try
            {
                AStrHashString = string.Empty;
                AStrReturn = string.Empty;

                LStrCNOptions = "CN=" + AStrMachineNameOrIPAddress;
                X509Store LX509Store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
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
            catch(Exception ex)
            {
                LBoolReturn = false;
                AStrReturn = "CertificateIsExist()\n" + ex.ToString();
            }

            return LBoolReturn;
        }

        /// <summary>
        /// 根据 HashString 判断证书是否在 指定的存储区域、存储位置 中存在
        /// </summary>
        /// <param name="AStrHashString">HashString</param>
        /// <param name="AStoreNameArea">存储区的名称</param>
        /// <param name="AStoreLocationPositon">存储区的位置</param>
        /// <param name="AStrReturn">当返回False时，返回获取时的错误信息</param>
        /// <returns>True / False</returns>
        public static bool CertificateIsExist(string AStrHashString, StoreName AStoreNameArea, StoreLocation AStoreLocationPositon, ref string AStrReturn)
        {
            bool LBoolReturn = false;

            try
            {
                AStrReturn = string.Empty;
                X509Store LX509Store = new X509Store(AStoreNameArea, AStoreLocationPositon);
                LX509Store.Open(OpenFlags.ReadOnly);
                foreach (X509Certificate LX509CertificateSingle in LX509Store.Certificates)
                {
                    if (LX509CertificateSingle.GetCertHashString().Trim() == AStrHashString) { LBoolReturn = true; break; }
                }
                LX509Store.Close(); LX509Store = null;
            }
            catch (Exception ex)
            {
                LBoolReturn = false;
                AStrReturn = "CertificateIsExist()\n" + ex.ToString();
            }

            return LBoolReturn;
        }

        /// <summary>
        /// 从StoreLocation.CurrentUser的StoreName.My中导出证书
        /// </summary>
        /// <param name="AStrHashString">证书的HashString</param>
        /// <param name="AStrExportPassword">导出证书时的密码</param>
        /// <param name="AStrExportTargerFile">证书保存的路径</param>
        /// <param name="AStrReturn">当返回False时，返回导出时的错误信息</param>
        /// <returns>True / False</returns>
        public static bool ExportCertificate(string AStrHashString, string AStrExportPassword, string AStrExportTargerFile, ref string AStrReturn)
        {
            bool LBoolReturn = true;

            try
            {
                AStrReturn = string.Empty;
                X509Store LX509Store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
                LX509Store.Open(OpenFlags.MaxAllowed);
                foreach (X509Certificate2 LX509CertificateSingle in LX509Store.Certificates)
                {
                    if (LX509CertificateSingle.GetCertHashString().Trim() == AStrHashString)
                    {
                        byte[] LByteExport = LX509CertificateSingle.Export(X509ContentType.Pfx, AStrExportPassword);
                        using (FileStream LFileStream = new FileStream(AStrExportTargerFile, FileMode.OpenOrCreate, FileAccess.Write))
                        {
                            LFileStream.Seek(0, SeekOrigin.Begin);
                            LFileStream.Write(LByteExport, 0, LByteExport.Length);
                            LFileStream.Close();
                            LFileStream.Dispose();
                        }

                        #region 带数据校验的写入方法，目前不采用
                        //using (FileStream fileStream = new FileStream(Path.Combine(labelExportPfxPath.Text.Trim(), textBoxCnName.Text.Trim() + ".pfx"), FileMode.Create))
                        //{
                        //    // Write the data to the file, byte by byte.   
                        //    for (int i = 0; i < pfxByte.Length; i++)
                        //        fileStream.WriteByte(pfxByte[i]);
                        //    // Set the stream position to the beginning of the file.   
                        //    fileStream.Seek(0, SeekOrigin.Begin);
                        //    // Read and verify the data.   
                        //    for (int i = 0; i < fileStream.Length; i++)
                        //    {
                        //        if (pfxByte[i] != fileStream.ReadByte())
                        //        {
                        //            MessageBox.Show("Error writing data.");
                        //            return;
                        //        }
                        //    }
                        //    fileStream.Close();
                        //    MessageBox.Show("导出PFX完毕");
                        //}
                        #endregion
                        
                        break;
                    }
                }
                LX509Store.Close(); LX509Store = null;
            }
            catch (Exception ex)
            {
                LBoolReturn = false;
                AStrReturn = "ExportCertificate()\n" + ex.ToString();
            }

            return LBoolReturn;
        }

        /// <summary>
        /// 安装证书
        /// </summary>
        /// <param name="AStrCertificateFile">证书文件完整路径和名称</param>
        /// <param name="AStrImportPassword">安装证书的密码</param>
        /// <param name="AStoreNameArea">存储区的名称</param>
        /// <param name="AStoreLocationPositon">存储区的位置</param>
        /// <param name="AStrReturn">当返回False时，返回安装时的错误信息</param>
        /// <returns>True / False</returns>
        public static bool InstallCertificate(string AStrCertificateFile, string AStrImportPassword, StoreName AStoreNameArea, StoreLocation AStoreLocationPositon, ref string AStrReturn)
        {
            bool LBoolReturn = true;

            try
            {
                AStrReturn = string.Empty;

                //byte[] LByteReadedCertificate = System.IO.File.ReadAllBytes(AStrCertificateFile);
                //X509Certificate2 LX509Certificate = new X509Certificate2(LByteReadedCertificate, AStrImportPassword);
                X509Certificate2 LX509Certificate = new X509Certificate2(AStrCertificateFile, AStrImportPassword, X509KeyStorageFlags.Exportable | X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.MachineKeySet);

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
                AStrReturn = "InstallCertificate()\n" + ex.ToString();
            }

            return LBoolReturn;
        }

        /// <summary>
        /// 根据 器名或IP地址 移除所有的证书
        /// </summary>
        /// <param name="AStrMachineNameOrIPAddress">机器名或IP地址</param>
        /// <param name="AStoreNameArea">存储区的名称</param>
        /// <param name="AStoreLocationPositon">存储区的位置</param>
        /// <param name="AStrReturn">当返回False时，返回移除时的错误信息</param>
        /// <returns>True / False</returns>
        public static bool UninstallCertificate(string AStrMachineNameOrIPAddress, StoreName AStoreNameArea, StoreLocation AStoreLocationPositon, ref string AStrReturn)
        {
            bool LBoolReturn = true;
            string LStrCNOptions = string.Empty;

            try
            {
                AStrReturn = string.Empty;

                LStrCNOptions = "CN=" + AStrMachineNameOrIPAddress;
                X509Store LX509Store = new X509Store(AStoreNameArea, AStoreLocationPositon);
                LX509Store.Open(OpenFlags.MaxAllowed);
                foreach (X509Certificate2 LX509CertificateSingle in LX509Store.Certificates)
                {
                    if (LX509CertificateSingle.Subject.IndexOf(LStrCNOptions) >= 0)
                    {
                        LX509Store.Remove(LX509CertificateSingle);
                    }
                }
                LX509Store.Close(); LX509Store = null;
            }
            catch (Exception ex)
            {
                LBoolReturn = false;
                AStrReturn = "UninstallCertificate()\n" + ex.ToString();
            }

            return LBoolReturn;
        }


        /// <summary>
        /// 根据HashString获取证书的 Hash 值，作为字节数组返回
        /// </summary>
        /// <param name="AStrHashString">HashString</param>
        /// <param name="AStoreNameArea">存储区的名称</param>
        /// <param name="AStoreLocationPositon">存储区的位置</param>
        /// <param name="AStrReturn">当返回False时，返回获取时的错误信息</param>
        /// <returns>Hash 值的字节数组</returns>
        public static byte[] ObtainCertificateCertHash(string AStrHashString, StoreName AStoreNameArea, StoreLocation AStoreLocationPositon, ref string AStrReturn)
        {
            byte[] LByteReturn = null;

            try
            {
                AStrReturn = string.Empty;
                X509Store LX509Store = new X509Store(AStoreNameArea, AStoreLocationPositon);
                LX509Store.Open(OpenFlags.ReadOnly);
                foreach (X509Certificate LX509CertificateSingle in LX509Store.Certificates)
                {
                    if (LX509CertificateSingle.GetCertHashString().Trim() == AStrHashString)
                    {
                        LByteReturn = LX509CertificateSingle.GetCertHash();
                        break;
                    }
                }
                LX509Store.Close(); LX509Store = null;
            }
            catch (Exception ex)
            {
                LByteReturn = null;
                AStrReturn = "ObtainCertificateCertHash()\n" + ex.ToString();
            }

            return LByteReturn;
        }
    }
}
