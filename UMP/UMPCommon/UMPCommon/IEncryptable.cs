//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    41dd0e47-1527-4425-9572-9be16bae629e
//        CLR Version:              4.0.30319.18444
//        Name:                     IEncryptable
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Common
//        File Name:                IEncryptable
//
//        created by Charley at 2015/3/15 13:03:07
//        http://www.voicecyber.com 
//
//======================================================================

using System.Text;

namespace VoiceCyber.UMP.Common
{
    /// <summary>
    /// 处理加密解密
    /// </summary>
    public interface IEncryptable
    {
        /// <summary>
        /// 对指定的数据加密
        /// </summary>
        /// <param name="source">源数据</param>
        /// <returns></returns>
        byte[] EncryptBytes(byte[] source);
        /// <summary>
        /// 指定加密模式，对指定的数据加密
        /// </summary>
        /// <param name="source">源数据</param>
        /// <param name="mode">加密模式</param>
        /// <returns></returns>
        byte[] EncryptBytes(byte[] source, int mode);
        /// <summary>
        /// 对字符串加密
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <returns></returns>
        string EncryptString(string source);
        /// <summary>
        /// 指定加密模式对字符串加密
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <param name="mode">
        /// Mode
        /// 参考EncryptionMode的定义
        /// </param>
        /// <returns></returns>
        string EncryptString(string source, int mode);
        /// <summary>
        /// 指定加密模式对字符串加密，源字符串使用指定的编码方式解析
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <param name="mode">加密模式</param>
        /// <param name="encoding">编码方式</param>
        /// <returns></returns>
        string EncryptString(string source, int mode, Encoding encoding);
        /// <summary>
        /// 对指定的数据解密
        /// </summary>
        /// <param name="source">源数据</param>
        /// <returns></returns>
        byte[] DecryptBytes(byte[] source);
        /// <summary>
        /// 指定加密模式，对指定的数据解密
        /// </summary>
        /// <param name="source">源数据</param>
        /// <param name="mode">加密模式</param>
        /// <returns></returns>
        byte[] DecryptBytes(byte[] source, int mode);
        /// <summary>
        /// 对字符串解密
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <returns></returns>
        string DecryptString(string source);
        /// <summary>
        /// 指定解密模式对字符串解密
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <param name="mode">
        /// Mode
        /// 参考EncryptionMode的定义
        /// </param>
        /// <returns></returns>
        string DecryptString(string source, int mode);
        /// <summary>
        /// 指定解密模式对字符串解密，解密出的结果使用指定的编码方式编码
        /// </summary>
        /// <param name="source">源字符串</param>
        /// <param name="mode">加密方式</param>
        /// <param name="encoding">编码方式</param>
        /// <returns></returns>
        string DecryptString(string source, int mode, Encoding encoding);
    }
}
