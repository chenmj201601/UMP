using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Encryptions;

namespace Wcf31021
{
    public partial class Service31021
    {
        private string EncryptString04(string strSource)
        {
            try
            {
                return ServerAESEncryption.EncryptString(strSource, EncryptionMode.AES256V04Hex);
            }
            catch
            {
                return strSource;
            }
        }

        private string DecryptString04(string strSource)
        {
            try
            {
                return ServerAESEncryption.DecryptString(strSource, EncryptionMode.AES256V04Hex);
            }
            catch
            {
                return strSource;
            }
        }

        private string EncryptString03(string strSource)
        {
            try
            {
                return ServerAESEncryption.EncryptString(strSource, EncryptionMode.AES256V03Hex);
            }
            catch
            {
                return strSource;
            }
        }

        private string DecryptString03(string strSource)
        {
            try
            {
                return ServerAESEncryption.DecryptString(strSource, EncryptionMode.AES256V03Hex);
            }
            catch
            {
                return strSource;
            }
        }

        private string EncryptString02(string strSource)
        {
            try
            {
                return ServerAESEncryption.EncryptString(strSource, EncryptionMode.AES256V02Hex);
            }
            catch
            {
                return strSource;
            }
        }

        private string DecryptString02(string strSource)
        {
            try
            {
                return ServerAESEncryption.DecryptString(strSource, EncryptionMode.AES256V02Hex);
            }
            catch
            {
                return strSource;
            }
        }

        private string EncryptString01(string strSource)
        {
            try
            {
                return ServerAESEncryption.EncryptString(strSource, EncryptionMode.AES256V01Hex);
            }
            catch
            {
                return strSource;
            }
        }

        private string DecryptString01(string strSource)
        {
            try
            {
                return ServerAESEncryption.DecryptString(strSource, EncryptionMode.AES256V01Hex);
            }
            catch
            {
                return strSource;
            }
        }

        private string DecryptStringKeyIV(string strSource, string key, string iv)
        {
            try
            {
                string strReturn = string.Empty;
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                int length = strSource.Length / 2;
                byte[] byteData = new byte[length];
                for (int i = 0; i < length; i++) { byteData[i] = (byte)Convert.ToInt32(strSource.Substring(i * 2, 2), 16); }
                des.Key = UnicodeEncoding.ASCII.GetBytes(key);
                des.IV = UnicodeEncoding.ASCII.GetBytes(iv);

                MemoryStream ms = new MemoryStream();
                CryptoStream stream = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
                stream.Write(byteData, 0, byteData.Length);
                stream.FlushFinalBlock();
                strReturn = Encoding.Unicode.GetString(ms.ToArray());
                return strReturn;
            }
            catch (Exception ex)
            {
                return strSource;
            }
        }
    }
}