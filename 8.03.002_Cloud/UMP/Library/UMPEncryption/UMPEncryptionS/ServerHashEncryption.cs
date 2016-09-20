//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    5f5a145f-435c-4c96-a238-435cf0283fe9
//        CLR Version:              4.0.30319.18063
//        Name:                     ServerHashEncryption
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Encryptions
//        File Name:                ServerHashEncryption
//
//        created by Charley at 2015/9/11 18:59:43
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
    /// Hash算法加密
    /// </summary>
    public class ServerHashEncryption
    {
        /// <summary>
        /// 对指定的字节数组，根据加密模式进行加密
        /// </summary>
        /// <param name="byteSource"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static byte[] EncryptBytes(byte[] byteSource, EncryptionMode mode)
        {
            byte[] byteResult;
            int type = (int) mode/1000;
            switch (type)
            {
                case 2:
                    var sha256 = new SHA256Managed();
                    byteResult = sha256.ComputeHash(byteSource);
                    break;
                case 3:
                    var sha512 = new SHA512Managed();
                    byteResult = sha512.ComputeHash(byteSource);
                    break;
                default:
                    throw new ArgumentException(string.Format("EncryptionMode invalid"), "mode");
            }
            int version = ((int)mode / 10) % 100;
            if (version > 0)
            {
                int aesMode = 1 * 1000 + version * 10 + ((int)mode % 10);
                byteResult = ServerAESEncryption.EncryptBytes(byteResult, (EncryptionMode)aesMode);
            }
            return byteResult;
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
    }
}
