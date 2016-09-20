using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace UMP.Tools.PublicClasses
{
    public class VoiceCyberPrivateEncryptionDecryption
    {
        #region Young自定义加密向量，一般用来加密文件内容
        private static string IStrEncryptoIV = "Y0ungPassword";
        #endregion

        #region 文件加密解密
        /// <summary>
        /// 将输入的字符串转换成32位的hash值
        /// </summary>
        /// <param name="AStrPassword"></param>
        /// <returns></returns>
        public static string CreateMD5HashString(string AStrPassword)
        {
            string LStrHashPassword = string.Empty;
            try
            {
                MD5CryptoServiceProvider LMD5Crypto = new MD5CryptoServiceProvider();
                byte[] LByteArray = Encoding.Unicode.GetBytes(AStrPassword);
                LByteArray = LMD5Crypto.ComputeHash(LByteArray);
                StringBuilder LStrBuilder = new StringBuilder();
                foreach (byte LByte in LByteArray) { LStrBuilder.Append(LByte.ToString("X2").ToUpper()); }
                LStrHashPassword = LStrBuilder.ToString();
            }
            catch { LStrHashPassword = "        "; }
            return LStrHashPassword;
        }

        /// <summary>
        /// 加密数据块
        /// </summary>
        /// <param name="AByteSource"></param>
        /// <param name="AStrPassowrd"></param>
        /// <returns></returns>
        private static byte[] EncryptingDataBlock(byte[] AByteSource, string AStrPassowrd)
        {
            MemoryStream LMemoryStream = new MemoryStream();
            DESCryptoServiceProvider LDESCrypto = new DESCryptoServiceProvider();
            LDESCrypto.Key = ASCIIEncoding.ASCII.GetBytes(CreateMD5HashString(AStrPassowrd).Substring(0, 8));
            LDESCrypto.IV = ASCIIEncoding.ASCII.GetBytes(CreateMD5HashString(IStrEncryptoIV).Substring(0, 8));
            CryptoStream LCryptoStream = new CryptoStream(LMemoryStream, LDESCrypto.CreateEncryptor(), CryptoStreamMode.Write);
            LCryptoStream.Write(AByteSource, 0, AByteSource.Length);
            LCryptoStream.FlushFinalBlock();

            return LMemoryStream.GetBuffer();
        }

        /// <summary>
        /// 解密数据块
        /// </summary>
        /// <param name="AByteSource"></param>
        /// <returns></returns>
        private static byte[] DecryptingDataBlock(byte[] AByteSource, string AStrPassowrd)
        {
            MemoryStream LMemoryStream = new MemoryStream(AByteSource, 0, AByteSource.Length);
            DESCryptoServiceProvider LDESCrypto = new DESCryptoServiceProvider();
            LDESCrypto.Key = ASCIIEncoding.ASCII.GetBytes(CreateMD5HashString(AStrPassowrd).Substring(0, 8));
            LDESCrypto.IV = ASCIIEncoding.ASCII.GetBytes(CreateMD5HashString(IStrEncryptoIV).Substring(0, 8));
            CryptoStream LCryptoStream = new CryptoStream(LMemoryStream, LDESCrypto.CreateDecryptor(), CryptoStreamMode.Write);
            LCryptoStream.Write(AByteSource, 0, AByteSource.Length);
            LCryptoStream.Flush();
            return LMemoryStream.ToArray();
        }

        /// <summary>
        /// 对文件进行加密，写入到新文件中
        /// AStrReturn = '000' 源文件不存在
        /// AStrReturn = '001' 目标文件已经存在
        /// </summary>
        /// <param name="AStrSourceFile">源文件</param>
        /// <param name="AStrTargetFile">目标文件</param>
        /// <param name="AStrPassword">加密密码</param>
        /// <param name="ABoolRemoveSource">是否删除源文件</param>
        /// <param name="ABoolReplaceTarget">目标文件已存在，是否覆盖</param>
        /// <param name="AStrReturn">返回加密失败的错误原因</param>
        /// <returns>True:加密成功， Fase:加密失败</returns>
        public static bool EncryptFileToFile(string AStrSourceFile, string AStrTargetFile, string AStrPassword, bool ABoolRemoveSource, bool ABoolReplaceTarget, ref string AStrReturn)
        {
            bool LBoolReturn = true;
            byte[] LByteSourceBuffer = new byte[1024];
            byte[] LByteTargetBuffer = new byte[1032];
            int LIntReadedByte;

            try
            {
                AStrReturn = string.Empty;
                if (!File.Exists(AStrSourceFile)) { LBoolReturn = false; AStrReturn = "000"; return LBoolReturn; }
                if (File.Exists(AStrTargetFile) && !ABoolReplaceTarget) { LBoolReturn = false; AStrReturn = "001"; return LBoolReturn; }
                if (File.Exists(AStrTargetFile)) { File.Delete(AStrTargetFile); }

                Stream LStreamSource = File.Open(AStrSourceFile, FileMode.Open);
                Stream LStreamTarget = File.Create(AStrTargetFile);

                do
                {
                    LIntReadedByte = LStreamSource.Read(LByteSourceBuffer, 0, 1024);
                    if (LIntReadedByte > 0)
                    {
                        LByteTargetBuffer = EncryptingDataBlock(LByteSourceBuffer, AStrPassword);
                        LStreamTarget.Write(LByteTargetBuffer, 0, LIntReadedByte + 8);
                    }
                    else { break; }
                } while (true);

                LStreamSource.Close();
                LStreamTarget.Close();
                LStreamSource.Dispose();
                LStreamTarget.Dispose();

                if (ABoolRemoveSource) { File.Delete(AStrSourceFile); }
            }
            catch (Exception ex)
            {
                LBoolReturn = false;
                AStrReturn = ex.Message;
            }

            return LBoolReturn;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="AStrSourceFile">源文件</param>
        /// <param name="AStrPassword">解密密码</param>
        /// <param name="AStrReturn">解密后结果， '000' 源文件不存在，如果是解密失败，则保存失败原因</param>
        /// <returns>True:解密成功， Fase:解密失败</returns>
        public static bool DecryptFileToString(string AStrSourceFile, string AStrPassword, ref string AStrReturn)
        {
            bool LBoolReturn = true;
            byte[] LByteSourceBuffer = new byte[1032];
            byte[] LByteTargetBuffer = new byte[1024];
            int LIntReadedByte;

            try
            {
                if (!File.Exists(AStrSourceFile)) { LBoolReturn = false; AStrReturn = "000"; return LBoolReturn; }
                Stream LStreamSource = File.Open(AStrSourceFile, FileMode.Open);
                MemoryStream LStreamTarget = new MemoryStream();

                do
                {
                    LIntReadedByte = LStreamSource.Read(LByteSourceBuffer, 0, 1032);
                    if (LIntReadedByte > 0)
                    {
                        LByteTargetBuffer = DecryptingDataBlock(LByteSourceBuffer, AStrPassword);
                        LStreamTarget.Write(LByteTargetBuffer, 0, LIntReadedByte - 8);
                    }
                    else { break; }
                } while (true);
                LStreamTarget.Seek(0, SeekOrigin.Begin);
                StreamReader LStreamReader = new StreamReader(LStreamTarget, Encoding.UTF8);
                AStrReturn = LStreamReader.ReadToEnd();
                LStreamSource.Close();
                LStreamTarget.Close();
                LStreamSource.Dispose();
                LStreamTarget.Dispose();
            }
            catch (Exception ex)
            {
                LBoolReturn = false;
                AStrReturn = ex.Message;
            }
            return LBoolReturn;
        }
        #endregion
    }
}
