//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    e898471f-0eaf-41cb-9de4-b712bf3877e3
//        CLR Version:              4.0.30319.18063
//        Name:                     Converter
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Common
//        File Name:                Converter
//
//        created by Charley at 2014/3/22 17:53:57
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;

namespace VoiceCyber.Common
{
    /// <summary>
    /// 转换类，实现数据类型、格式的转换 
    /// 这是一个静态类
    /// </summary>
    public static class Converter
    {
        /// <summary>
        /// 将整型值转成二进制
        /// </summary>
        /// <param name="intValue"></param>
        /// <returns></returns>
        public static string Int2Binary(int intValue)
        {
            string strBinary = string.Empty;
            int val1, val2;
            while (intValue > 0)
            {
                val1 = intValue / 2;
                val2 = intValue % 2;
                intValue = val1;
                strBinary = val2 + strBinary;
            }
            return strBinary;
        }
        /// <summary>
        /// 将二进制转成整型值
        /// </summary>
        /// <param name="strBinary"></param>
        /// <returns></returns>
        public static int Binary2Int(string strBinary)
        {
            if (string.IsNullOrEmpty(strBinary)) { return 0; }
            int intValue = 0;
            for (int i = strBinary.Length - 1; i >= 0; i--)
            {
                if (strBinary[i] == '1')
                {
                    intValue += (int)Math.Pow(2, strBinary.Length - i - 1);
                }
            }
            return intValue;
        }
        /// <summary>
        /// 将结构体转换成字节数组
        /// </summary>
        /// <param name="objStruct">结构体</param>
        /// <returns></returns>
        public static byte[] Struct2Bytes(object objStruct)
        {
            //得到结构体的大小
            int size = Marshal.SizeOf(objStruct);
            //创建byte数组
            byte[] bytes = new byte[size];
            //分配结构体大小的内存空间
            IntPtr structPtr = Marshal.AllocHGlobal(size);
            //将结构体拷到分配好的内存空间
            Marshal.StructureToPtr(objStruct, structPtr, false);
            //从内存空间拷到byte数组
            Marshal.Copy(structPtr, bytes, 0, size);
            //释放内存空间
            Marshal.FreeHGlobal(structPtr);
            //返回byte数组
            return bytes;
        }
        /// <summary>
        /// 将字节数组转换成结构体
        /// </summary>
        /// <param name="data">字节数组</param>
        /// <param name="type">结构体类型</param>
        /// <returns></returns>
        public static object Bytes2Struct(byte[] data, Type type)
        {
            //得到结构体的大小
            int size = Marshal.SizeOf(type);
            //数组长度小于结构体的大小
            if (size > data.Length)
            {
                //返回空
                return null;
            }
            //分配结构体大小的内存空间
            IntPtr structPtr = Marshal.AllocHGlobal(size);
            //将byte数组拷到分配好的内存空间
            Marshal.Copy(data, 0, structPtr, size);
            //将内存空间转换为目标结构体
            object obj = Marshal.PtrToStructure(structPtr, type);
            //释放内存空间
            Marshal.FreeHGlobal(structPtr);
            //返回结构体
            return obj;
        }
        /// <summary>
        /// 将16进制数据转换成字符串
        /// </summary>
        /// <param name="strSource">16进制数据，长度应该是2的倍数</param>
        /// <returns></returns>
        public static string Hex2String(string strSource)
        {
            string strReturn = string.Empty;
            while (strSource.Length > 0)
            {
                strReturn +=
                    Convert.ToChar(Convert.ToUInt32(strSource.Substring(0, 2), 16))
                        .ToString(CultureInfo.InvariantCulture);
                if (strSource.Length < 2)
                {
                    throw new Exception(string.Format("Source string length invalid"));
                }
                strSource = strSource.Substring(2);
            }
            return strReturn;
        }
        /// <summary>
        /// 将16进制数据转换成字节数组
        /// </summary>
        /// <param name="strSource"></param>
        /// <returns></returns>
        public static byte[] Hex2Byte(string strSource)
        {
            int intDataLength = strSource.Length / 2;
            byte[] data = new byte[intDataLength];
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = Convert.ToByte(strSource.Substring(i * 2, 2), 16);
            }
            return data;
        }
        /// <summary>
        /// 将字符串以16进制形式表示
        /// </summary>
        /// <param name="strSource">输入字符串</param>
        /// <returns></returns>
        public static string String2Hex(string strSource)
        {
            byte[] data = Encoding.UTF8.GetBytes(strSource);
            return Byte2Hex(data);
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
        /// <summary>
        /// 以时间形式表示秒数
        /// </summary>
        /// <param name="totalSecond"></param>
        /// <returns></returns>
        public static string Second2Time(double totalSecond)
        {
            string strReturn;
            int second, minute, hour;
            hour = (int)(totalSecond / (60 * 60));
            totalSecond = totalSecond % (60 * 60);
            minute = (int)(totalSecond / 60);
            totalSecond = totalSecond % 60;
            second = (int)totalSecond;
            strReturn = string.Format("{0}:{1}:{2}", hour.ToString("00"), minute.ToString("00"), second.ToString("00"));
            return strReturn;
        }
        /// <summary>
        /// 毫秒转换成时间
        /// </summary>
        /// <param name="millisecond"></param>
        /// <returns></returns>
        public static string MilliSecond2Time(double millisecond)
        {
            string strReturn;
            int minute, hour;
            double second;
            hour = (int)(millisecond / (60 * 60 * 1000));
            millisecond = millisecond % (60 * 60 * 1000);
            minute = (int)(millisecond / (60 * 1000));
            millisecond = millisecond % (60 * 1000);
            second = millisecond / 1000;
            strReturn = string.Format("{0}:{1}:{2}", hour.ToString("00"), minute.ToString("00"), second.ToString("00.0"));
            return strReturn;
        }
        /// <summary>
        /// 将时间转换成秒
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static double Time2Second(string time)
        {
            string[] arrTime = time.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
            if (arrTime.Length != 3)
            {
                throw new Exception("Time string invalid");
            }
            int hour = int.Parse(arrTime[0]);
            int minute = int.Parse(arrTime[1]);
            double second = double.Parse(arrTime[2]);
            return hour * 60 * 60 + minute * 60 + second;
        }
        /// <summary>
        /// 日期转换，如将20140101000000转换成2014-01-01 00:00:00
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static DateTime NumberToDatetime(string number)
        {
            number = number.PadLeft(14);
            string year, month, day, hour, minute, second;
            year = number.Substring(0, 4);
            month = number.Substring(4, 2);
            day = number.Substring(6, 2);
            hour = number.Substring(8, 2);
            minute = number.Substring(10, 2);
            second = number.Substring(12, 2);
            string str = string.Format("{0}-{1}-{2} {3}:{4}:{5}", year, month, day, hour, minute, second);
            return DateTime.Parse(str);
        }
        /// <summary>
        /// 将指定的字符串转成字节数组
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static byte[] String2ByteArray(string str)
        {
            return String2ByteArray(str, Encoding.Default);
        }
        /// <summary>
        /// 将指定的字符串转成指定大小的字节数组，不足的补充空（\0）
        /// </summary>
        /// <param name="str"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static byte[] String2ByteArray(string str, int length)
        {
            return String2ByteArray(str, length, Encoding.Default);
        }
        /// <summary>
        /// 使用指定的编码将指定的字符串转成字节数组
        /// </summary>
        /// <param name="str"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static byte[] String2ByteArray(string str, Encoding encoding)
        {
            return encoding.GetBytes(str);
        }
        /// <summary>
        /// 使用指定的编码将指定的字符串转成指定大小的字节数组，不足的补充空（\0）
        /// </summary>
        /// <param name="str"></param>
        /// <param name="length"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static byte[] String2ByteArray(string str, int length, Encoding encoding)
        {
            byte[] temp = new byte[length];
            byte[] data = Encoding.Default.GetBytes(str);
            int len = Math.Min(length, data.Length);
            Array.Copy(data, 0, temp, 0, len);
            return temp;
        }
    }
}
