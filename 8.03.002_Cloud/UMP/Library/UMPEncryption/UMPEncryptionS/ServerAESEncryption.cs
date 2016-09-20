//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    bd159a58-9b90-49f3-991b-f66037510f97
//        CLR Version:              4.0.30319.18063
//        Name:                     ServerAESEncryption
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Encryptions
//        File Name:                ServerAESEncryption
//
//        created by Charley at 2015/9/11 17:16:11
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
    public class ServerAESEncryption
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
                    byteKey = Encoding.ASCII.GetBytes(ServerKeys.VCT_KEY256_LOW1);
                    byteIV = Encoding.ASCII.GetBytes(ServerIVs.VCT_ENCRYPTION_AES_IVEC_LOW);
                    break;
                case EncryptionMode.AES256V02Hex:
                case EncryptionMode.AES256V02B64:
                    byteKey = Encoding.ASCII.GetBytes(ServerKeys.VCT_KEY256_LOW2);
                    byteIV = Encoding.ASCII.GetBytes(ServerIVs.VCT_ENCRYPTION_AES_IVEC_LOW);
                    break;
                case EncryptionMode.AES256V03Hex:
                case EncryptionMode.AES256V03B64:
                    byteKey = Encoding.ASCII.GetBytes(ServerKeys.VCT_KEY256_LOW3);
                    byteIV = Encoding.ASCII.GetBytes(ServerIVs.VCT_ENCRYPTION_AES_IVEC_LOW);
                    break;
                case EncryptionMode.AES256V04Hex:
                case EncryptionMode.AES256V04B64:
                    byteKey = Encoding.ASCII.GetBytes(ServerKeys.VCT_KEY256_NORMAL1);
                    byteIV = Encoding.ASCII.GetBytes(ServerIVs.VCT_ENCRYPTION_AES_IVEC_LOW);
                    break;
                case EncryptionMode.AES256V05Hex:
                case EncryptionMode.AES256V05B64:
                    byteKey = Encoding.ASCII.GetBytes(ServerKeys.VCT_KEY256_NORMAL2);
                    byteIV = Encoding.ASCII.GetBytes(ServerIVs.VCT_ENCRYPTION_AES_IVEC_LOW);
                    break;
                case EncryptionMode.AES256V06Hex:
                case EncryptionMode.AES256V06B64:
                    byteKey = Encoding.ASCII.GetBytes(ServerKeys.VCT_KEY256_NORMAL3);
                    byteIV = Encoding.ASCII.GetBytes(ServerIVs.VCT_ENCRYPTION_AES_IVEC_LOW);
                    break;
                case EncryptionMode.AES256V07Hex:
                case EncryptionMode.AES256V07B64:
                    byteKey = Encoding.ASCII.GetBytes(ServerKeys.VCT_KEY256_HIGH1);
                    byteIV = Encoding.ASCII.GetBytes(ServerIVs.VCT_ENCRYPTION_AES_IVEC_LOW);
                    break;
                case EncryptionMode.AES256V08Hex:
                case EncryptionMode.AES256V08B64:
                    byteKey = Encoding.ASCII.GetBytes(ServerKeys.VCT_KEY256_HIGH2);
                    byteIV = Encoding.ASCII.GetBytes(ServerIVs.VCT_ENCRYPTION_AES_IVEC_LOW);
                    break;
                case EncryptionMode.AES256V09Hex:
                case EncryptionMode.AES256V09B64:
                    byteKey = Encoding.ASCII.GetBytes(ServerKeys.VCT_KEY256_HIGH3);
                    byteIV = Encoding.ASCII.GetBytes(ServerIVs.VCT_ENCRYPTION_AES_IVEC_LOW);
                    break;
                case EncryptionMode.AES256V10Hex:
                case EncryptionMode.AES256V10B64:
                    byteKey = Encoding.ASCII.GetBytes(ServerKeys.VCT_KEY256_LOW1);
                    byteIV = Encoding.ASCII.GetBytes(ServerIVs.VCT_ENCRYPTION_AES_IVEC_NORMAL);
                    break;
                case EncryptionMode.AES256V11Hex:
                case EncryptionMode.AES256V11B64:
                    byteKey = Encoding.ASCII.GetBytes(ServerKeys.VCT_KEY256_LOW2);
                    byteIV = Encoding.ASCII.GetBytes(ServerIVs.VCT_ENCRYPTION_AES_IVEC_NORMAL);
                    break;
                case EncryptionMode.AES256V12Hex:
                case EncryptionMode.AES256V12B64:
                    byteKey = Encoding.ASCII.GetBytes(ServerKeys.VCT_KEY256_LOW3);
                    byteIV = Encoding.ASCII.GetBytes(ServerIVs.VCT_ENCRYPTION_AES_IVEC_NORMAL);
                    break;
                case EncryptionMode.AES256V13Hex:
                case EncryptionMode.AES256V13B64:
                    byteKey = Encoding.ASCII.GetBytes(ServerKeys.VCT_KEY256_NORMAL1);
                    byteIV = Encoding.ASCII.GetBytes(ServerIVs.VCT_ENCRYPTION_AES_IVEC_NORMAL);
                    break;
                case EncryptionMode.AES256V14Hex:
                case EncryptionMode.AES256V14B64:
                    byteKey = Encoding.ASCII.GetBytes(ServerKeys.VCT_KEY256_NORMAL2);
                    byteIV = Encoding.ASCII.GetBytes(ServerIVs.VCT_ENCRYPTION_AES_IVEC_NORMAL);
                    break;
                case EncryptionMode.AES256V15Hex:
                case EncryptionMode.AES256V15B64:
                    byteKey = Encoding.ASCII.GetBytes(ServerKeys.VCT_KEY256_NORMAL3);
                    byteIV = Encoding.ASCII.GetBytes(ServerIVs.VCT_ENCRYPTION_AES_IVEC_NORMAL);
                    break;
                case EncryptionMode.AES256V16Hex:
                case EncryptionMode.AES256V16B64:
                    byteKey = Encoding.ASCII.GetBytes(ServerKeys.VCT_KEY256_HIGH1);
                    byteIV = Encoding.ASCII.GetBytes(ServerIVs.VCT_ENCRYPTION_AES_IVEC_NORMAL);
                    break;
                case EncryptionMode.AES256V17Hex:
                case EncryptionMode.AES256V17B64:
                    byteKey = Encoding.ASCII.GetBytes(ServerKeys.VCT_KEY256_HIGH2);
                    byteIV = Encoding.ASCII.GetBytes(ServerIVs.VCT_ENCRYPTION_AES_IVEC_NORMAL);
                    break;
                case EncryptionMode.AES256V18Hex:
                case EncryptionMode.AES256V18B64:
                    byteKey = Encoding.ASCII.GetBytes(ServerKeys.VCT_KEY256_HIGH3);
                    byteIV = Encoding.ASCII.GetBytes(ServerIVs.VCT_ENCRYPTION_AES_IVEC_NORMAL);
                    break;
                case EncryptionMode.AES256V19Hex:
                case EncryptionMode.AES256V19B64:
                    byteKey = Encoding.ASCII.GetBytes(ServerKeys.VCT_KEY256_LOW1);
                    byteIV = Encoding.ASCII.GetBytes(ServerIVs.VCT_ENCRYPTION_AES_IVEC_HIGH);
                    break;
                case EncryptionMode.AES256V20Hex:
                case EncryptionMode.AES256V20B64:
                    byteKey = Encoding.ASCII.GetBytes(ServerKeys.VCT_KEY256_LOW2);
                    byteIV = Encoding.ASCII.GetBytes(ServerIVs.VCT_ENCRYPTION_AES_IVEC_HIGH);
                    break;
                case EncryptionMode.AES256V21Hex:
                case EncryptionMode.AES256V21B64:
                    byteKey = Encoding.ASCII.GetBytes(ServerKeys.VCT_KEY256_LOW3);
                    byteIV = Encoding.ASCII.GetBytes(ServerIVs.VCT_ENCRYPTION_AES_IVEC_HIGH);
                    break;
                case EncryptionMode.AES256V22Hex:
                case EncryptionMode.AES256V22B64:
                    byteKey = Encoding.ASCII.GetBytes(ServerKeys.VCT_KEY256_NORMAL1);
                    byteIV = Encoding.ASCII.GetBytes(ServerIVs.VCT_ENCRYPTION_AES_IVEC_HIGH);
                    break;
                case EncryptionMode.AES256V23Hex:
                case EncryptionMode.AES256V23B64:
                    byteKey = Encoding.ASCII.GetBytes(ServerKeys.VCT_KEY256_NORMAL2);
                    byteIV = Encoding.ASCII.GetBytes(ServerIVs.VCT_ENCRYPTION_AES_IVEC_HIGH);
                    break;
                case EncryptionMode.AES256V24Hex:
                case EncryptionMode.AES256V24B64:
                    byteKey = Encoding.ASCII.GetBytes(ServerKeys.VCT_KEY256_NORMAL3);
                    byteIV = Encoding.ASCII.GetBytes(ServerIVs.VCT_ENCRYPTION_AES_IVEC_HIGH);
                    break;
                case EncryptionMode.AES256V25Hex:
                case EncryptionMode.AES256V25B64:
                    byteKey = Encoding.ASCII.GetBytes(ServerKeys.VCT_KEY256_HIGH1);
                    byteIV = Encoding.ASCII.GetBytes(ServerIVs.VCT_ENCRYPTION_AES_IVEC_HIGH);
                    break;
                case EncryptionMode.AES256V26Hex:
                case EncryptionMode.AES256V26B64:
                    byteKey = Encoding.ASCII.GetBytes(ServerKeys.VCT_KEY256_HIGH2);
                    byteIV = Encoding.ASCII.GetBytes(ServerIVs.VCT_ENCRYPTION_AES_IVEC_HIGH);
                    break;
                case EncryptionMode.AES256V27Hex:
                case EncryptionMode.AES256V27B64:
                    byteKey = Encoding.ASCII.GetBytes(ServerKeys.VCT_KEY256_HIGH3);
                    byteIV = Encoding.ASCII.GetBytes(ServerIVs.VCT_ENCRYPTION_AES_IVEC_HIGH);
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
                    strReturn = ServerEncryptionUtils.Byte2Hex(byteResult);
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
                    byteKey = Encoding.ASCII.GetBytes(ServerKeys.VCT_KEY256_LOW1);
                    byteIV = Encoding.ASCII.GetBytes(ServerIVs.VCT_ENCRYPTION_AES_IVEC_LOW);
                    break;
                case EncryptionMode.AES256V02Hex:
                case EncryptionMode.AES256V02B64:
                    byteKey = Encoding.ASCII.GetBytes(ServerKeys.VCT_KEY256_LOW2);
                    byteIV = Encoding.ASCII.GetBytes(ServerIVs.VCT_ENCRYPTION_AES_IVEC_LOW);
                    break;
                case EncryptionMode.AES256V03Hex:
                case EncryptionMode.AES256V03B64:
                    byteKey = Encoding.ASCII.GetBytes(ServerKeys.VCT_KEY256_LOW3);
                    byteIV = Encoding.ASCII.GetBytes(ServerIVs.VCT_ENCRYPTION_AES_IVEC_LOW);
                    break;
                case EncryptionMode.AES256V04Hex:
                case EncryptionMode.AES256V04B64:
                    byteKey = Encoding.ASCII.GetBytes(ServerKeys.VCT_KEY256_NORMAL1);
                    byteIV = Encoding.ASCII.GetBytes(ServerIVs.VCT_ENCRYPTION_AES_IVEC_LOW);
                    break;
                case EncryptionMode.AES256V05Hex:
                case EncryptionMode.AES256V05B64:
                    byteKey = Encoding.ASCII.GetBytes(ServerKeys.VCT_KEY256_NORMAL2);
                    byteIV = Encoding.ASCII.GetBytes(ServerIVs.VCT_ENCRYPTION_AES_IVEC_LOW);
                    break;
                case EncryptionMode.AES256V06Hex:
                case EncryptionMode.AES256V06B64:
                    byteKey = Encoding.ASCII.GetBytes(ServerKeys.VCT_KEY256_NORMAL3);
                    byteIV = Encoding.ASCII.GetBytes(ServerIVs.VCT_ENCRYPTION_AES_IVEC_LOW);
                    break;
                case EncryptionMode.AES256V07Hex:
                case EncryptionMode.AES256V07B64:
                    byteKey = Encoding.ASCII.GetBytes(ServerKeys.VCT_KEY256_HIGH1);
                    byteIV = Encoding.ASCII.GetBytes(ServerIVs.VCT_ENCRYPTION_AES_IVEC_LOW);
                    break;
                case EncryptionMode.AES256V08Hex:
                case EncryptionMode.AES256V08B64:
                    byteKey = Encoding.ASCII.GetBytes(ServerKeys.VCT_KEY256_HIGH2);
                    byteIV = Encoding.ASCII.GetBytes(ServerIVs.VCT_ENCRYPTION_AES_IVEC_LOW);
                    break;
                case EncryptionMode.AES256V09Hex:
                case EncryptionMode.AES256V09B64:
                    byteKey = Encoding.ASCII.GetBytes(ServerKeys.VCT_KEY256_HIGH3);
                    byteIV = Encoding.ASCII.GetBytes(ServerIVs.VCT_ENCRYPTION_AES_IVEC_LOW);
                    break;
                case EncryptionMode.AES256V10Hex:
                case EncryptionMode.AES256V10B64:
                    byteKey = Encoding.ASCII.GetBytes(ServerKeys.VCT_KEY256_LOW1);
                    byteIV = Encoding.ASCII.GetBytes(ServerIVs.VCT_ENCRYPTION_AES_IVEC_NORMAL);
                    break;
                case EncryptionMode.AES256V11Hex:
                case EncryptionMode.AES256V11B64:
                    byteKey = Encoding.ASCII.GetBytes(ServerKeys.VCT_KEY256_LOW2);
                    byteIV = Encoding.ASCII.GetBytes(ServerIVs.VCT_ENCRYPTION_AES_IVEC_NORMAL);
                    break;
                case EncryptionMode.AES256V12Hex:
                case EncryptionMode.AES256V12B64:
                    byteKey = Encoding.ASCII.GetBytes(ServerKeys.VCT_KEY256_LOW3);
                    byteIV = Encoding.ASCII.GetBytes(ServerIVs.VCT_ENCRYPTION_AES_IVEC_NORMAL);
                    break;
                case EncryptionMode.AES256V13Hex:
                case EncryptionMode.AES256V13B64:
                    byteKey = Encoding.ASCII.GetBytes(ServerKeys.VCT_KEY256_NORMAL1);
                    byteIV = Encoding.ASCII.GetBytes(ServerIVs.VCT_ENCRYPTION_AES_IVEC_NORMAL);
                    break;
                case EncryptionMode.AES256V14Hex:
                case EncryptionMode.AES256V14B64:
                    byteKey = Encoding.ASCII.GetBytes(ServerKeys.VCT_KEY256_NORMAL2);
                    byteIV = Encoding.ASCII.GetBytes(ServerIVs.VCT_ENCRYPTION_AES_IVEC_NORMAL);
                    break;
                case EncryptionMode.AES256V15Hex:
                case EncryptionMode.AES256V15B64:
                    byteKey = Encoding.ASCII.GetBytes(ServerKeys.VCT_KEY256_NORMAL3);
                    byteIV = Encoding.ASCII.GetBytes(ServerIVs.VCT_ENCRYPTION_AES_IVEC_NORMAL);
                    break;
                case EncryptionMode.AES256V16Hex:
                case EncryptionMode.AES256V16B64:
                    byteKey = Encoding.ASCII.GetBytes(ServerKeys.VCT_KEY256_HIGH1);
                    byteIV = Encoding.ASCII.GetBytes(ServerIVs.VCT_ENCRYPTION_AES_IVEC_NORMAL);
                    break;
                case EncryptionMode.AES256V17Hex:
                case EncryptionMode.AES256V17B64:
                    byteKey = Encoding.ASCII.GetBytes(ServerKeys.VCT_KEY256_HIGH2);
                    byteIV = Encoding.ASCII.GetBytes(ServerIVs.VCT_ENCRYPTION_AES_IVEC_NORMAL);
                    break;
                case EncryptionMode.AES256V18Hex:
                case EncryptionMode.AES256V18B64:
                    byteKey = Encoding.ASCII.GetBytes(ServerKeys.VCT_KEY256_HIGH3);
                    byteIV = Encoding.ASCII.GetBytes(ServerIVs.VCT_ENCRYPTION_AES_IVEC_NORMAL);
                    break;
                case EncryptionMode.AES256V19Hex:
                case EncryptionMode.AES256V19B64:
                    byteKey = Encoding.ASCII.GetBytes(ServerKeys.VCT_KEY256_LOW1);
                    byteIV = Encoding.ASCII.GetBytes(ServerIVs.VCT_ENCRYPTION_AES_IVEC_HIGH);
                    break;
                case EncryptionMode.AES256V20Hex:
                case EncryptionMode.AES256V20B64:
                    byteKey = Encoding.ASCII.GetBytes(ServerKeys.VCT_KEY256_LOW2);
                    byteIV = Encoding.ASCII.GetBytes(ServerIVs.VCT_ENCRYPTION_AES_IVEC_HIGH);
                    break;
                case EncryptionMode.AES256V21Hex:
                case EncryptionMode.AES256V21B64:
                    byteKey = Encoding.ASCII.GetBytes(ServerKeys.VCT_KEY256_LOW3);
                    byteIV = Encoding.ASCII.GetBytes(ServerIVs.VCT_ENCRYPTION_AES_IVEC_HIGH);
                    break;
                case EncryptionMode.AES256V22Hex:
                case EncryptionMode.AES256V22B64:
                    byteKey = Encoding.ASCII.GetBytes(ServerKeys.VCT_KEY256_NORMAL1);
                    byteIV = Encoding.ASCII.GetBytes(ServerIVs.VCT_ENCRYPTION_AES_IVEC_HIGH);
                    break;
                case EncryptionMode.AES256V23Hex:
                case EncryptionMode.AES256V23B64:
                    byteKey = Encoding.ASCII.GetBytes(ServerKeys.VCT_KEY256_NORMAL2);
                    byteIV = Encoding.ASCII.GetBytes(ServerIVs.VCT_ENCRYPTION_AES_IVEC_HIGH);
                    break;
                case EncryptionMode.AES256V24Hex:
                case EncryptionMode.AES256V24B64:
                    byteKey = Encoding.ASCII.GetBytes(ServerKeys.VCT_KEY256_NORMAL3);
                    byteIV = Encoding.ASCII.GetBytes(ServerIVs.VCT_ENCRYPTION_AES_IVEC_HIGH);
                    break;
                case EncryptionMode.AES256V25Hex:
                case EncryptionMode.AES256V25B64:
                    byteKey = Encoding.ASCII.GetBytes(ServerKeys.VCT_KEY256_HIGH1);
                    byteIV = Encoding.ASCII.GetBytes(ServerIVs.VCT_ENCRYPTION_AES_IVEC_HIGH);
                    break;
                case EncryptionMode.AES256V26Hex:
                case EncryptionMode.AES256V26B64:
                    byteKey = Encoding.ASCII.GetBytes(ServerKeys.VCT_KEY256_HIGH2);
                    byteIV = Encoding.ASCII.GetBytes(ServerIVs.VCT_ENCRYPTION_AES_IVEC_HIGH);
                    break;
                case EncryptionMode.AES256V27Hex:
                case EncryptionMode.AES256V27B64:
                    byteKey = Encoding.ASCII.GetBytes(ServerKeys.VCT_KEY256_HIGH3);
                    byteIV = Encoding.ASCII.GetBytes(ServerIVs.VCT_ENCRYPTION_AES_IVEC_HIGH);
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
                    byteSource = ServerEncryptionUtils.Hex2Byte(strSource);
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
