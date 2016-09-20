using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UMPServicePack.PublicClasses
{
    public class StringHexConver
    {
        /// <summary>
        /// 字符串转十六进制
        /// </summary>
        /// <param name="s">字符串</param>
        /// <param name="charset">编码格式</param>
        /// <returns></returns>
        public static string ToHex(string s, string charset)
        {
            if ((s.Length % 2) != 0)
            {
                s += " ";//空格
                //throw new ArgumentException("s is not valid chinese string!");
            }
            System.Text.Encoding chs = System.Text.Encoding.GetEncoding(charset);
            byte[] bytes = chs.GetBytes(s);
            string str = "";
            for (int i = 0; i < bytes.Length; i++)
            {
                str += string.Format("{0:X}", bytes[i]);
                if ((i != bytes.Length - 1))
                {
                    str += string.Format("{0}", ",");
                }
            }
            return str.ToLower();
        }

        /// <summary>
        /// 十六进制转字符串
        /// </summary>
        /// <param name="hex">十六进制串</param>
        /// <param name="charset">编码格式</param>
        /// <returns></returns>
        public static string UnHex(string hex, string charset)
        {
            if (hex == null)
                throw new ArgumentNullException("hex");
            hex = hex.Replace(",", "");
            hex = hex.Replace("\n", "");
            hex = hex.Replace("\\", "");
            hex = hex.Replace(" ", "");
            if (hex.Length % 2 != 0)
            {
                hex += "20";//空格
            }
            // 需要将 hex 转换成 byte 数组。 
            byte[] bytes = new byte[hex.Length / 2];

            for (int i = 0; i < bytes.Length; i++)
            {
                try
                {
                    // 每两个字符是一个 byte。 
                    bytes[i] = byte.Parse(hex.Substring(i * 2, 2),
                    System.Globalization.NumberStyles.HexNumber);
                }
                catch
                {
                    // Rethrow an exception with custom message. 
                    throw new ArgumentException("hex is not a valid hex number!", "hex");
                }
            }
            System.Text.Encoding chs = System.Text.Encoding.GetEncoding(charset);
            return chs.GetString(bytes);
        }
    }
}
