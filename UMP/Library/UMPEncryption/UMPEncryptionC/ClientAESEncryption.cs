//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    d5ca4c0e-4ad2-499f-a75b-e41b737ffce9
//        CLR Version:              4.0.30319.18063
//        Name:                     ClientAESEncryption
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Encryptions
//        File Name:                ClientAESEncryption
//
//        created by Charley at 2015/9/11 17:41:46
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Security.Cryptography;
using System.Text;
using VoiceCyber.UMP.Common;

namespace VoiceCyber.UMP.Encryptions
{
    /// <summary>
    /// AES算法加密解密处理
    /// </summary>
    public class ClientAESEncryption
    {
        #region 加密

        /// <summary>
        /// 对指定的字节数组，使用指定的密钥和向量进行加密
        /// </summary>
        /// <param name="byteSource"></param>
        /// <param name="byteKey"></param>
        /// <param name="byteIV"></param>
        /// <returns></returns>
        public static byte[] EncryptBytes(byte[] byteSource, byte[] byteKey, byte[] byteIV)
        {
            int length = (int)Math.Ceiling((byteSource.Length / (double)16)) * 16;
            byte[] temp = new byte[length];
            Array.Copy(byteSource, 0, temp, 0, byteSource.Length);
            byte[] byteResult;
            if (byteKey.Length > 0 && byteIV.Length > 0)
            {
                var rijndael = new RijndaelManaged();
                rijndael.Key = byteKey;
                rijndael.IV = byteIV;
                rijndael.Mode = CipherMode.CBC;
                rijndael.Padding = PaddingMode.None;
                ICryptoTransform transform = rijndael.CreateEncryptor();
                byteResult = transform.TransformFinalBlock(temp, 0, temp.Length);
            }
            else
            {
                byteResult = byteSource;
            }
            return byteResult;
        }
        /// <summary>
        /// 对指定的字节数组，根据加密模式进行加密
        /// </summary>
        /// <param name="byteSource"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static byte[] EncryptBytes(byte[] byteSource, EncryptionMode mode)
        {
            byte[] byteKey;
            byte[] byteIV;
            switch (mode)
            {
                case EncryptionMode.None:
                case EncryptionMode.AES256V00Hex:
                case EncryptionMode.AES256V00B64:
                    byteKey = new byte[0];
                    byteIV = new byte[0];
                    break;
                case EncryptionMode.AES256V01Hex:
                case EncryptionMode.AES256V01B64:
                    byteKey = Encoding.ASCII.GetBytes(ClientKeys.VCT_KEY256_LOW1);
                    byteIV = Encoding.ASCII.GetBytes(ClientIVs.VCT_ENCRYPTION_AES_IVEC_LOW);
                    break;
                case EncryptionMode.AES256V04Hex:
                case EncryptionMode.AES256V04B64:
                    byteKey = Encoding.ASCII.GetBytes(ClientKeys.VCT_KEY256_NORMAL1);
                    byteIV = Encoding.ASCII.GetBytes(ClientIVs.VCT_ENCRYPTION_AES_IVEC_LOW);
                    break;
                default:
                    throw new ArgumentException(string.Format("EncryptionMode invalid"), "mode");
            }
            return EncryptBytes(byteSource, byteKey, byteIV);
        }
        /// <summary>
        /// 对指定的字符串，根据加密模式和编码方式进行加密
        /// </summary>
        /// <param name="strSource"></param>
        /// <param name="mode"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string EncryptString(string strSource, EncryptionMode mode, Encoding encoding)
        {
            byte[] byteSource = encoding.GetBytes(strSource);
            byte[] byteResult = EncryptBytes(byteSource, mode);
            int mod = (int)mode % 10;
            string strReturn;
            switch (mod)
            {
                case 1:
                    strReturn = ClientEncryptionUtils.Byte2Hex(byteResult);
                    break;
                case 2:
                    strReturn = Convert.ToBase64String(byteResult);
                    break;
                default:
                    throw new ArgumentException(string.Format("EncryptionMode invalid"), "mode");
            }
            return strReturn;
        }
        /// <summary>
        /// 对指定的字符串，根据加密模式进行加密
        /// </summary>
        /// <param name="strSource"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static string EncryptString(string strSource, EncryptionMode mode)
        {
            return EncryptString(strSource, mode, Encoding.Unicode);
        }

        #endregion


        #region 解密

        /// <summary>
        /// 对指定的字节数组，使用指定的密钥和向量进行解密
        /// </summary>
        /// <param name="byteSource"></param>
        /// <param name="byteKey"></param>
        /// <param name="byteIV"></param>
        /// <returns></returns>
        public static byte[] DecryptBytes(byte[] byteSource, byte[] byteKey, byte[] byteIV)
        {
            byte[] byteResult;
            if (byteKey.Length > 0 && byteIV.Length > 0)
            {
                var rijndael = new RijndaelManaged();
                rijndael.Key = byteKey;
                rijndael.IV = byteIV;
                rijndael.Mode = CipherMode.CBC;
                rijndael.Padding = PaddingMode.None;
                ICryptoTransform transform = rijndael.CreateDecryptor();
                byteResult = transform.TransformFinalBlock(byteSource, 0, byteSource.Length);
            }
            else
            {
                byteResult = byteSource;
            }
            return byteResult;
        }
        /// <summary>
        /// 对指定的字节数组，根据加密模式进行解密
        /// </summary>
        /// <param name="byteSource"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static byte[] DecryptBytes(byte[] byteSource, EncryptionMode mode)
        {
            byte[] byteKey;
            byte[] byteIV;
            switch (mode)
            {
                case EncryptionMode.None:
                case EncryptionMode.AES256V00Hex:
                case EncryptionMode.AES256V00B64:
                    byteKey = new byte[0];
                    byteIV = new byte[0];
                    break;
                case EncryptionMode.AES256V01Hex:
                case EncryptionMode.AES256V01B64:
                    byteKey = Encoding.ASCII.GetBytes(ClientKeys.VCT_KEY256_LOW1);
                    byteIV = Encoding.ASCII.GetBytes(ClientIVs.VCT_ENCRYPTION_AES_IVEC_LOW);
                    break;
                case EncryptionMode.AES256V04Hex:
                case EncryptionMode.AES256V04B64:
                    byteKey = Encoding.ASCII.GetBytes(ClientKeys.VCT_KEY256_NORMAL1);
                    byteIV = Encoding.ASCII.GetBytes(ClientIVs.VCT_ENCRYPTION_AES_IVEC_LOW);
                    break;
                default:
                    throw new ArgumentException(string.Format("EncryptionMode invalid"), "mode");
            }
            return DecryptBytes(byteSource, byteKey, byteIV);
        }
        /// <summary>
        /// 对指定字符串，根据加密模式和编码方式进行解密
        /// </summary>
        /// <param name="strSource"></param>
        /// <param name="mode"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string DecryptString(string strSource, EncryptionMode mode, Encoding encoding)
        {
            int mod = (int)mode % 10;
            byte[] byteSource;
            switch (mod)
            {
                case 1:
                    byteSource = ClientEncryptionUtils.Hex2Byte(strSource);
                    break;
                case 2:
                    byteSource = Convert.FromBase64String(strSource);
                    break;
                default:
                    throw new ArgumentException(string.Format("EncryptionMode invalid"), "mode");
            }

            byte[] byteResult = DecryptBytes(byteSource, mode);
            string strTemp = encoding.GetString(byteResult);
            strTemp = strTemp.TrimEnd('\0');
            return strTemp;
        }
        /// <summary>
        /// 对指定字符串，根据加密模式进行解密
        /// </summary>
        /// <param name="strSource"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static string DecryptString(string strSource, EncryptionMode mode)
        {
            return DecryptString(strSource, mode, Encoding.Unicode);
        }

        #endregion

    }
}
