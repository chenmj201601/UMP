//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    1b29ff95-a592-499f-a795-8e3df887b778
//        CLR Version:              4.0.30319.18063
//        Name:                     ServerEncryptionUtils
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.UMP.Encryptions
//        File Name:                ServerEncryptionUtils
//
//        created by Charley at 2015/9/11 15:18:18
//        http://www.voicecyber.com 
//
//======================================================================
using System;

namespace VoiceCyber.UMP.Encryptions
{
    public class ServerEncryptionUtils
    {
        /// <summary>
        /// 将16进制数据转换成字节数组
        /// </summary>
        /// <param name="strSource"></param>
        /// <returns></returns>
        public static byte[] Hex2Byte(string strSource)
        {
            int mode = strSource.Length % 2;
            if (mode != 0)
            {
                throw new ArgumentException(string.Format("Input string invalid"));
            }
            int intDataLength = strSource.Length / 2;
            byte[] data = new byte[intDataLength];
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = Convert.ToByte(strSource.Substring(i * 2, 2), 16);
            }
            return data;
        }

        /// <summary>
        /// 字节数组以16进制形式表示
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string Byte2Hex(byte[] data)
        {
            string strReturn = string.Empty;
            for (int i = 0; i < data.Length; i++)
            {
                strReturn += data[i].ToString("X2");
            }
            return strReturn;
        }
    }
}
