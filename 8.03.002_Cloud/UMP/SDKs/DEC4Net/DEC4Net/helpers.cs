//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    bee4eb98-4135-4d5b-a85e-b9483b6a8c1d
//        CLR Version:              4.0.30319.18063
//        Name:                     helpers
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.SDKs.DEC
//        File Name:                helpers
//
//        created by Charley at 2015/6/15 11:34:50
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace VoiceCyber.SDKs.DEC
{
    public class Helpers
    {
        /// <summary>
        /// 创建基本识别头
        /// </summary>
        /// <returns></returns>
        internal static _TAG_NETPACK_DISTINGUISHHEAD CreateDistinguishHead()
        {
            _TAG_NETPACK_DISTINGUISHHEAD head = new _TAG_NETPACK_DISTINGUISHHEAD();
            head._flag = DecDefines.NETPACK_FLAG;
            head._version = DecDefines.NETPACK_DISTHEAD_VER1;
            head._cbsize = (byte)Marshal.SizeOf(typeof(_TAG_NETPACK_DISTINGUISHHEAD_VER1));
            return head;
        }

        #region 创建消息Message

        internal static _TAG_NETPACK_MESSAGE CreateMessage(ushort sourceModule, ushort sourceNumber, ushort targetModule,
            ushort targetNumber, ushort number, ushort smallType, ushort middleType, ushort largeType)
        {
            _TAG_NETPACK_MESSAGE message = new _TAG_NETPACK_MESSAGE();
            message._targetNumber = targetNumber;
            message._targetModule = targetModule;
            message._sourceNumber = sourceNumber;
            message._sourceModule = sourceModule;
            message._number = number;
            message._samllType = smallType;
            message._middleType = middleType;
            message._largeType = largeType;
            return message;
        }

        #endregion


        #region Others
        /// <summary>
        /// 计算加密后数据的长度
        /// </summary>
        /// <param name="encrypt">加密算法</param>
        /// <param name="validSize">实际数据长度</param>
        /// <returns></returns>
        internal static ushort GetEncryptStoreSize(byte encrypt, ushort validSize)
        {
            switch (encrypt)
            {
                case DecDefines.NETPACK_ENCRYPT_AES_128_CBC:
                case DecDefines.NETPACK_ENCRYPT_AES_256_CBC:
                    return (ushort)(validSize + (((validSize % DecDefines.AES_BLOCK_SIZE) != 0) ? (DecDefines.AES_BLOCK_SIZE - (validSize % DecDefines.AES_BLOCK_SIZE)) : 0));
                case DecDefines.NETPACK_ENCRYPT_NOTHING:
                    return validSize;
            }
            return validSize;
        }
        /// <summary>
        /// 将字节数组转换成字符串，使用UTF8编码，并截去空字符串
        /// </summary>
        /// <param name="arr"></param>
        /// <returns></returns>
        internal static string ConvertByteArrayToString(byte[] arr)
        {
            string str = Encoding.UTF8.GetString(arr);
            return str.TrimEnd('\0');
        }
        /// <summary>
        /// 将字符串转换成字节数组
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        internal static byte[] ConvertStringToByteArray(string msg)
        {
            return ConvertStringToByteArray(msg, msg.Length);
        }
        /// <summary>
        /// 将字符串转换成字节数组，根据指定的长度左对齐
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        internal static byte[] ConvertStringToByteArray(string msg, int length)
        {
            byte[] arr = new byte[length];
            arr.Initialize();
            byte[] data = Encoding.UTF8.GetBytes(msg);
            Array.Copy(data, 0, arr, 0, data.Length);
            return arr;
        }
        /// <summary>
        /// 使用指定的日期时间创建type_datetime，时间偏移为0
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        internal static type_datetime CreateTimestamp(DateTime datetime)
        {
            type_datetime time = new type_datetime();
            time._time = (ulong)(datetime - DateTime.Parse("1600-1-1")).Ticks;
            time._bias = 0;
            return time;
        }
        /// <summary>
        /// 根据指定的时间戳计算出日期时间
        /// </summary>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        internal static DateTime GetTimeFromTimestamp(long timestamp)
        {
            DateTime dt = new DateTime(timestamp);
            dt = dt.AddYears(DateTime.Parse("1600-1-1").Year);
            return dt;
        }
        /// <summary>
        /// 根据主版本和此版本创建协议版本
        /// </summary>
        /// <param name="major"></param>
        /// <param name="minor"></param>
        /// <returns></returns>
        internal static ulong MakeNetPackProtocol(ulong major, ulong minor)
        {
            ulong protocol = major;
            protocol = protocol << 32 | minor;
            return protocol;
        }
        /// <summary>
        /// 计算认证码
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        internal static byte[] GetValidicationCode(byte[] session)
        {
            int intOffset = session[0] & 0x001f;
            byte byteMask = (byte)(session[intOffset] ^ intOffset);
            byte[] byteValidication = new byte[32];
            for (int i = 0; i < 32; i++) { byteValidication[i] = (byte)(session[i] ^ byteMask); }
            return byteValidication;
        }

        #endregion


        #region 数据加密解密
        /// <summary>
        /// 解密消息数据
        /// </summary>
        /// <param name="encrypt">加密方式</param>
        /// <param name="endpointID">会话ID</param>
        /// <param name="byteSource"></param>
        /// <returns></returns>
        public static byte[] DecryptData(int encrypt, ulong endpointID, byte[] byteSource)
        {

            switch (encrypt)
            {
                case DecDefines.NETPACK_ENCRYPT_AES_128_CBC:
                case DecDefines.NETPACK_ENCRYPT_AES_256_CBC:
                    //==========================================
                    //AES256_CBC(2)加密方式的解密过程
                    //Key     sha256(endpointid+slat)
                    //IV      sha256(slat+endpointid)
                    //===========================================
                    byte[] byteSalt = BitConverter.GetBytes(DecDefines.NETPACK_ENCRYPT_SLAT);
                    byte[] byteEndpoint = BitConverter.GetBytes(endpointID);
                    byte[] key = new byte[byteSalt.Length + byteEndpoint.Length];
                    Array.Copy(byteEndpoint, 0, key, 0, byteEndpoint.Length);
                    Array.Copy(byteSalt, 0, key, byteEndpoint.Length, byteSalt.Length);
                    SHA256 sha256 = new SHA256Managed();
                    key = sha256.ComputeHash(key);
                    byte[] iv = new byte[byteSalt.Length + byteEndpoint.Length];
                    Array.Copy(byteSalt, 0, iv, 0, byteSalt.Length);
                    Array.Copy(byteEndpoint, 0, iv, byteSalt.Length, byteEndpoint.Length);
                    iv = sha256.ComputeHash(iv);
                    byte[] iv16 = new byte[16];
                    Array.Copy(iv, 0, iv16, 0, 16);
                    //byte[] key = new byte[32];
                    //byte[] iv16 = new byte[16];
                    RijndaelManaged managed = new RijndaelManaged();
                    managed.Key = key;
                    managed.IV = iv16;
                    managed.Mode = CipherMode.CBC;
                    managed.Padding = PaddingMode.Zeros;
                    ICryptoTransform transform = managed.CreateDecryptor();
                    byte[] byteDecrypt = transform.TransformFinalBlock(byteSource, 0, byteSource.Length);
                    return byteDecrypt;
            }
            return byteSource;
        }
        /// <summary>
        /// 解密消息数据
        /// </summary>
        /// <param name="encrypt"></param>
        /// <param name="endpointID"></param>
        /// <param name="byteSource"></param>
        /// <returns></returns>
        public static string DecryptString(int encrypt, ulong endpointID, byte[] byteSource)
        {
            byte[] byteResult = DecryptData(encrypt, endpointID, byteSource);
            string str = Encoding.UTF8.GetString(byteResult);
            str = str.TrimEnd('\0');
            return str;
        }
        /// <summary>
        /// 加密消息数据
        /// </summary>
        /// <param name="encrypt">加密方式</param>
        /// <param name="endpointID">会话ID</param>
        /// <param name="byteSource"></param>
        /// <returns></returns>
        public static byte[] EncryptData(int encrypt, ulong endpointID, byte[] byteSource)
        {
            switch (encrypt)
            {
                case DecDefines.NETPACK_ENCRYPT_AES_128_CBC:
                case DecDefines.NETPACK_ENCRYPT_AES_256_CBC:
                    byte[] byteSalt = BitConverter.GetBytes(DecDefines.NETPACK_ENCRYPT_SLAT);
                    byte[] byteEndpoint = BitConverter.GetBytes(endpointID);
                    byte[] key = new byte[byteSalt.Length + byteEndpoint.Length];
                    Array.Copy(byteEndpoint, 0, key, 0, byteEndpoint.Length);
                    Array.Copy(byteSalt, 0, key, byteEndpoint.Length, byteSalt.Length);
                    SHA256 sha256 = new SHA256Managed();
                    key = sha256.ComputeHash(key);
                    byte[] iv = new byte[byteSalt.Length + byteEndpoint.Length];
                    Array.Copy(byteSalt, 0, iv, 0, byteSalt.Length);
                    Array.Copy(byteEndpoint, 0, iv, byteSalt.Length, byteEndpoint.Length);
                    iv = sha256.ComputeHash(iv);
                    byte[] iv16 = new byte[16];
                    Array.Copy(iv, 0, iv16, 0, 16);
                    //byte[] key = new byte[32];
                    //byte[] iv16 = new byte[16];
                    RijndaelManaged managed = new RijndaelManaged();
                    managed.Key = key;
                    managed.IV = iv16;
                    managed.Mode = CipherMode.CBC;
                    managed.Padding = PaddingMode.Zeros;
                    ICryptoTransform transform = managed.CreateEncryptor();
                    byte[] byteEncrypt = transform.TransformFinalBlock(byteSource, 0, byteSource.Length);
                    return byteEncrypt;
            }
            return byteSource;
        }
        /// <summary>
        /// 加密消息数据
        /// </summary>
        /// <param name="encrypt"></param>
        /// <param name="endpointID"></param>
        /// <param name="strSource"></param>
        /// <returns></returns>
        public static byte[] EncryptString(int encrypt, ulong endpointID, string strSource)
        {
            byte[] data = Encoding.UTF8.GetBytes(strSource);
            return EncryptData(encrypt, endpointID, data);
        }

        #endregion

    }
}
