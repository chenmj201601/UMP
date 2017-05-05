using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Text;

namespace PFShareClassesS
{
    public class EncryptionAndDecryption
    {
        #region AES算法 加密或解密时使用的中间变量
        /// <summary>加密向量</summary>
        private static string IStrEncryptDecryptIV = string.Empty;
        /// <summary>加密密钥</summary>
        private static string IStrEncryptDecryptKey = string.Empty;
        #endregion

        #region 加密向量

        #region CBC模式加密向量(低保密等级)
        private static string VCT_ENCRYPTION_AES_IVEC_LOW = "4I!9V6X8Af98^5bC";
        #endregion

        #region CBC模式加密向量(一般保密等级,对VCT/VCC以外保密)
        private static string VCT_ENCRYPTION_AES_IVEC_NORMAL = ")26V!7Cwh6i5Y1Qd";
        #endregion

        #region CBC模式加密向量(高保密等级,对VCT/VCC以外保密)
        private static string VCT_ENCRYPTION_AES_IVEC_HIGH = "ev04Z0VDLn!58}]o";
        #endregion

        #endregion

        #region 加密密钥

        #region 低保密等级加密密钥.用于数据库登录密码等第三方数据的加密。
        private static string VCT_KEY256_LOW1 = "74kjs&33$2240JsfkUgalaujYRDCasa#";
        private static string VCT_KEY256_LOW2 = "4sfc[3J24safafQ%kG56*20+xfc[3Jas";
        private static string VCT_KEY256_LOW3 = "724safafQ%kG56*2033YRDCr7X_(asa#";
        #endregion

        #region 一般保密等级加密密钥,用于数据传输过程中的加密
        private static string VCT_KEY256_NORMAL1 = "%^2kfgXCgtHk2%719Kf)1edj^4jAdfRa";
        private static string VCT_KEY256_NORMAL2 = "$k2%fgcnRa$k2%4jA1edj^4at2ad#8d.";
        private static string VCT_KEY256_NORMAL3 = "*tk%k2d*3cC1edBej4d%3#-x3()feL4a";
        #endregion

        #region 高保密等级加密密钥,用于客户数据、加密系统密钥的加密
        private static string VCT_KEY256_HIGH1 = "&4jskYBdfa%33()234'sjasfFabfTQA%";
        private static string VCT_KEY256_HIGH2 = "&$ofm*fM<dgta%z0%>*?hFa!24A)j*Sl";
        private static string VCT_KEY256_HIGH3 = "#$szL3sl(>!e3uAczX?!8)cc36+,a7h@";
        #endregion
        #endregion

        #region 枚举 - UMP加密或解密使用的密钥和向量类型
        /// <summary>
        /// 加密或解密使用的密钥和向量类型
        /// </summary>
        public enum UMPKeyAndIVType
        {
            /// <summary>IVEC_LOW,KEY256_LOW1</summary>
            M001 = 1,
            /// <summary>IVEC_LOW,KEY256_LOW2</summary>
            M002 = 2,
            /// <summary>IVEC_LOW,KEY256_LOW3</summary>
            M003 = 3,

            /// <summary>IVEC_LOW,KEY256_NORMAL1</summary>
            M004 = 4,
            /// <summary>IVEC_LOW,KEY256_NORMAL2</summary>
            M005 = 5,
            /// <summary>IVEC_LOW,KEY256_NORMAL3</summary>
            M006 = 6,

            /// <summary>IVEC_LOW,KEY256_HIGH1</summary>
            M007 = 7,
            /// <summary>IVEC_LOW,KEY256_HIGH2</summary>
            M008 = 8,
            /// <summary>IVEC_LOW,KEY256_HIGH3</summary>
            M009 = 9,

            /// <summary>IVEC_NORMAL,KEY256_LOW1</summary>
            M010 = 10,
            /// <summary>IVEC_NORMAL,KEY256_LOW2</summary>
            M011 = 11,
            /// <summary>IVEC_NORMAL,KEY256_LOW3</summary>
            M012 = 12,

            /// <summary>IVEC_NORMAL,KEY256_NORMAL1</summary>
            M013 = 13,
            /// <summary>IVEC_NORMAL,KEY256_NORMAL2</summary>
            M014 = 14,
            /// <summary>IVEC_NORMAL,KEY256_NORMAL3</summary>
            M015 = 15,

            /// <summary>IVEC_NORMAL,KEY256_HIGH1</summary>
            M016 = 16,
            /// <summary>IVEC_NORMAL,KEY256_HIGH2</summary>
            M017 = 17,
            /// <summary>IVEC_NORMAL,KEY256_HIGH3</summary>
            M018 = 18,

            /// <summary>IVEC_HIGH,KEY256_LOW1</summary>
            M019 = 19,
            /// <summary>IVEC_HIGH,KEY256_LOW2</summary>
            M020 = 20,
            /// <summary>IVEC_HIGH,KEY256_LOW3</summary>
            M021 = 21,

            /// <summary>IVEC_HIGH,KEY256_NORMAL1</summary>
            M022 = 22,
            /// <summary>IVEC_HIGH,KEY256_NORMAL2</summary>
            M023 = 23,
            /// <summary>IVEC_HIGH,KEY256_NORMAL3</summary>
            M024 = 24,

            /// <summary>IVEC_HIGH,KEY256_HIGH1</summary>
            M025 = 25,
            /// <summary>IVEC_HIGH,KEY256_HIGH2</summary>
            M026 = 26,
            /// <summary>IVEC_HIGH,KEY256_HIGH3</summary>
            M027 = 27,

            /// <summary>IVEC_LOW,KEY256_LOW1</summary>
            M101 = 101,
            /// <summary>IVEC_LOW,KEY256_LOW2</summary>
            M102 = 102,
            /// <summary>IVEC_LOW,KEY256_LOW3</summary>
            M103 = 103,

            /// <summary>IVEC_LOW,KEY256_NORMAL1</summary>
            M104 = 104,
            /// <summary>IVEC_LOW,KEY256_NORMAL2</summary>
            M105 = 105,
            /// <summary>IVEC_LOW,KEY256_NORMAL3</summary>
            M106 = 106,

            /// <summary>IVEC_LOW,KEY256_HIGH1</summary>
            M107 = 107,
            /// <summary>IVEC_LOW,KEY256_HIGH2</summary>
            M108 = 108,
            /// <summary>IVEC_LOW,KEY256_HIGH3</summary>
            M109 = 109,

            /// <summary>IVEC_NORMAL,KEY256_LOW1</summary>
            M110 = 110,
            /// <summary>IVEC_NORMAL,KEY256_LOW2</summary>
            M111 = 111,
            /// <summary>IVEC_NORMAL,KEY256_LOW3</summary>
            M112 = 112,

            /// <summary>IVEC_NORMAL,KEY256_NORMAL1</summary>
            M113 = 113,
            /// <summary>IVEC_NORMAL,KEY256_NORMAL2</summary>
            M114 = 114,
            /// <summary>IVEC_NORMAL,KEY256_NORMAL3</summary>
            M115 = 115,

            /// <summary>IVEC_NORMAL,KEY256_HIGH1</summary>
            M116 = 116,
            /// <summary>IVEC_NORMAL,KEY256_HIGH2</summary>
            M117 = 117,
            /// <summary>IVEC_NORMAL,KEY256_HIGH3</summary>
            M118 = 118,

            /// <summary>IVEC_HIGH,KEY256_LOW1</summary>
            M119 = 119,
            /// <summary>IVEC_HIGH,KEY256_LOW2</summary>
            M120 = 120,
            /// <summary>IVEC_HIGH,KEY256_LOW3</summary>
            M121 = 121,

            /// <summary>IVEC_HIGH,KEY256_NORMAL1</summary>
            M122 = 122,
            /// <summary>IVEC_HIGH,KEY256_NORMAL2</summary>
            M123 = 123,
            /// <summary>IVEC_HIGH,KEY256_NORMAL3</summary>
            M124 = 124,

            /// <summary>IVEC_HIGH,KEY256_HIGH1</summary>
            M125 = 125,
            /// <summary>IVEC_HIGH,KEY256_HIGH2</summary>
            M126 = 126,
            /// <summary>IVEC_HIGH,KEY256_HIGH3</summary>
            M127 = 127,
        }
        #endregion

        #region 用户传入加密密钥和向量进行字符串加密
        public static string EncryptStringYKeyIV(string AStrSource, string AStrDESCryptoKey, string AStrDESCryptoIV)
        {
            string LStrReturn = string.Empty;

            try
            {
                DESCryptoServiceProvider LDESCrypto = new DESCryptoServiceProvider();
                byte[] LByteArraySource = Encoding.Unicode.GetBytes(AStrSource);
                LDESCrypto.Key = UnicodeEncoding.ASCII.GetBytes(AStrDESCryptoKey);
                LDESCrypto.IV = UnicodeEncoding.ASCII.GetBytes(AStrDESCryptoIV);
                MemoryStream LMemoryStream = new MemoryStream();
                CryptoStream LCryptoStream = new CryptoStream(LMemoryStream, LDESCrypto.CreateEncryptor(), CryptoStreamMode.Write);
                LCryptoStream.Write(LByteArraySource, 0, LByteArraySource.Length);
                LCryptoStream.FlushFinalBlock();
                StringBuilder LStrBuilder = new StringBuilder();
                foreach (byte LByte in LMemoryStream.ToArray()) { LStrBuilder.Append(LByte.ToString("X2").ToUpper()); }
                LStrReturn = LStrBuilder.ToString();
            }
            catch { LStrReturn = AStrSource; }

            return LStrReturn;
        }

        public static string DecryptStringYKeyIV(string AStrSource, string AStrDESCryptoKey, string AStrDESCryptoIV)
        {
            string LStrReturn = string.Empty;

            try
            {
                DESCryptoServiceProvider LDESCrypto = new DESCryptoServiceProvider();
                int LIntSourceLen = AStrSource.Length / 2;
                byte[] LByteArraintySource = new byte[LIntSourceLen];
                for (int LIntLoop = 0; LIntLoop < LIntSourceLen; LIntLoop++) { LByteArraintySource[LIntLoop] = (byte)Convert.ToInt32(AStrSource.Substring(LIntLoop * 2, 2), 16); }
                LDESCrypto.Key = UnicodeEncoding.ASCII.GetBytes(AStrDESCryptoKey);
                LDESCrypto.IV = UnicodeEncoding.ASCII.GetBytes(AStrDESCryptoIV);

                MemoryStream LMemoryStream = new MemoryStream();
                CryptoStream LCryptoStream = new CryptoStream(LMemoryStream, LDESCrypto.CreateDecryptor(), CryptoStreamMode.Write);
                LCryptoStream.Write(LByteArraintySource, 0, LByteArraintySource.Length);
                LCryptoStream.FlushFinalBlock();
                LStrReturn = Encoding.Unicode.GetString(LMemoryStream.ToArray());
            }
            catch { LStrReturn = AStrSource; }

            return LStrReturn;
        }

        #endregion

        #region public 字符串简单加密/解密
        /// <summary>
        /// 字符串简单加密（DES算法），字符串长度尽量控制在 64 个字符内。
        /// 使用默认的加密向量和加密密钥
        /// </summary>
        /// <param name="AStrSource">需要加密的字符串</param>
        /// <returns>如果返回值 与 传入的值 相等，则说明加密失败</returns>
        public static string EncryptStringY(string AStrSource)
        {
            string LStrReturn = string.Empty;

            try
            {
                DESCryptoServiceProvider LDESCrypto = new DESCryptoServiceProvider();
                byte[] LByteArraySource = Encoding.Unicode.GetBytes(AStrSource);
                LDESCrypto.Key = UnicodeEncoding.ASCII.GetBytes(CreateMD5HashString("Y0ungEncryptKEY").Substring(0, 8));
                LDESCrypto.IV = UnicodeEncoding.ASCII.GetBytes(CreateMD5HashString("Y0ungEncryptIV").Substring(0, 8));
                MemoryStream LMemoryStream = new MemoryStream();
                CryptoStream LCryptoStream = new CryptoStream(LMemoryStream, LDESCrypto.CreateEncryptor(), CryptoStreamMode.Write);
                LCryptoStream.Write(LByteArraySource, 0, LByteArraySource.Length);
                LCryptoStream.FlushFinalBlock();
                StringBuilder LStrBuilder = new StringBuilder();
                foreach (byte LByte in LMemoryStream.ToArray()) { LStrBuilder.Append(LByte.ToString("X2").ToUpper()); }
                LStrReturn = LStrBuilder.ToString();
            }
            catch { LStrReturn = AStrSource; }

            return LStrReturn;
        }

        /// <summary>
        /// 字符串简单解密（DES算法）
        /// 使用默认的加密向量和加密密钥
        /// </summary>
        /// <param name="AStrSource">需要解密的字符串</param>
        /// <returns>如果返回值 与 传入的值 相等，则说明解密失败</returns>
        public static string DecryptString(string AStrSource)
        {
            string LStrReturn = string.Empty;

            try
            {
                DESCryptoServiceProvider LDESCrypto = new DESCryptoServiceProvider();
                int LIntSourceLen = AStrSource.Length / 2;
                byte[] LByteArraintySource = new byte[LIntSourceLen];
                for (int LIntLoop = 0; LIntLoop < LIntSourceLen; LIntLoop++) { LByteArraintySource[LIntLoop] = (byte)Convert.ToInt32(AStrSource.Substring(LIntLoop * 2, 2), 16); }
                LDESCrypto.Key = UnicodeEncoding.ASCII.GetBytes(CreateMD5HashString("Y0ungEncryptKEY").Substring(0, 8));
                LDESCrypto.IV = UnicodeEncoding.ASCII.GetBytes(CreateMD5HashString("Y0ungEncryptIV").Substring(0, 8));
                MemoryStream LMemoryStream = new MemoryStream();
                CryptoStream LCryptoStream = new CryptoStream(LMemoryStream, LDESCrypto.CreateDecryptor(), CryptoStreamMode.Write);
                LCryptoStream.Write(LByteArraintySource, 0, LByteArraintySource.Length);
                LCryptoStream.FlushFinalBlock();
                LStrReturn = Encoding.Unicode.GetString(LMemoryStream.ToArray());
            }
            catch { LStrReturn = AStrSource; }

            return LStrReturn;
        }
        #endregion

        #region 内部使用方法/函数

        #region 生成 MD5 Hash 字符串
        /// <summary>
        /// 生成 MD5 Hash 字符串
        /// </summary>
        /// <param name="AStrSource"></param>
        /// <returns></returns>
        private static string CreateMD5HashString(string AStrSource)
        {
            string LStrHashPassword = string.Empty;
            try
            {
                MD5CryptoServiceProvider LMD5Crypto = new MD5CryptoServiceProvider();
                byte[] LByteArray = Encoding.Unicode.GetBytes(AStrSource);
                LByteArray = LMD5Crypto.ComputeHash(LByteArray);
                StringBuilder LStrBuilder = new StringBuilder();
                foreach (byte LByte in LByteArray) { LStrBuilder.Append(LByte.ToString("X2").ToUpper()); }
                LStrHashPassword = LStrBuilder.ToString();
            }
            catch { LStrHashPassword = "        "; }
            return LStrHashPassword;
        }
        #endregion

        private static string ConvertHexToString(string AStrHexValue)
        {
            string LStrReturn = "";
            string LStrSource = string.Empty;

            LStrSource = AStrHexValue;
            while (LStrSource.Length > 0)
            {
                LStrReturn += Convert.ToChar(Convert.ToUInt32(LStrSource.Substring(0, 2), 16)).ToString();
                LStrSource = LStrSource.Substring(2);
            }
            return LStrReturn;
        }

        private static string ConvertByteToHexStr(byte[] ABytesValue)
        {
            string LStrReturn = "";

            if (ABytesValue != null)
            {
                for (int LIntLoopByte = 0; LIntLoopByte < ABytesValue.Length; LIntLoopByte++) { LStrReturn += ABytesValue[LIntLoopByte].ToString("X2"); }
            }

            return LStrReturn;
        }

        private static byte[] GetKeyByteArray(string AStrSource)
        {
            UnicodeEncoding LUnicodeEncoding = new UnicodeEncoding();
            int LIntLength = AStrSource.Length;
            byte[] LByteTemp = new byte[LIntLength - 1];

            LByteTemp = LUnicodeEncoding.GetBytes(AStrSource);

            return LByteTemp;
        }

        private static string AnasysVerificationCodeAndKeyIV(string AStrVerificationCode, UMPKeyAndIVType AIntEDType)
        {
            string LStrReturn = string.Empty;
            string LStrVerificationCode = string.Empty;
            string LStrVCodeData = string.Empty;
            int LIntVCTIndex = 0, LIntUMPIndex = 0, LIntIVKeyIndex = 0;
            string LStrVCT = string.Empty, LStrUMP = string.Empty, LStrIVKey = string.Empty;

            try
            {
                LStrVerificationCode = DecryptString(AStrVerificationCode);
                if (LStrVerificationCode == AStrVerificationCode) { return "Error02"; }

                LIntVCTIndex = int.Parse(LStrVerificationCode.Substring(0, 2));
                LIntUMPIndex = int.Parse(LStrVerificationCode.Substring(2, 2));
                LIntIVKeyIndex = int.Parse(LStrVerificationCode.Substring(4, 2));

                LStrVCodeData = LStrVerificationCode.Substring(6);
                LStrIVKey = LStrVCodeData.Substring(LIntIVKeyIndex, 3);

                LStrVCodeData = LStrVCodeData.Substring(0, LIntIVKeyIndex) + LStrVCodeData.Substring(LIntIVKeyIndex + 3);
                LStrUMP = LStrVCodeData.Substring(LIntUMPIndex, 3);

                LStrVCodeData = LStrVCodeData.Substring(0, LIntUMPIndex) + LStrVCodeData.Substring(LIntUMPIndex + 3);
                LStrVCT = LStrVCodeData.Substring(LIntVCTIndex, 3);

                if (int.Parse(LStrIVKey) != (int)AIntEDType) { LStrReturn = "Error02"; }

                IStrEncryptDecryptIV = string.Empty;
                IStrEncryptDecryptKey = string.Empty;

                #region IVEC_LOW,KEY256_LOW
                if ((int)AIntEDType % 100 == 1)
                {
                    IStrEncryptDecryptIV = VCT_ENCRYPTION_AES_IVEC_LOW;
                    IStrEncryptDecryptKey = VCT_KEY256_LOW1;
                }
                if ((int)AIntEDType % 100 == 2)
                {
                    IStrEncryptDecryptIV = VCT_ENCRYPTION_AES_IVEC_LOW;
                    IStrEncryptDecryptKey = VCT_KEY256_LOW2;
                }
                if ((int)AIntEDType % 100 == 3)
                {
                    IStrEncryptDecryptIV = VCT_ENCRYPTION_AES_IVEC_LOW;
                    IStrEncryptDecryptKey = VCT_KEY256_LOW3;
                }
                #endregion

                #region IVEC_LOW,KEY256_NORMAL
                if ((int)AIntEDType % 100 == 4)
                {
                    IStrEncryptDecryptIV = VCT_ENCRYPTION_AES_IVEC_LOW;
                    IStrEncryptDecryptKey = VCT_KEY256_NORMAL1;
                }
                if ((int)AIntEDType % 100 == 5)
                {
                    IStrEncryptDecryptIV = VCT_ENCRYPTION_AES_IVEC_LOW;
                    IStrEncryptDecryptKey = VCT_KEY256_NORMAL2;
                }
                if ((int)AIntEDType % 100 == 6)
                {
                    IStrEncryptDecryptIV = VCT_ENCRYPTION_AES_IVEC_LOW;
                    IStrEncryptDecryptKey = VCT_KEY256_NORMAL3;
                }
                #endregion

                #region IVEC_LOW,KEY256_HIGH
                if ((int)AIntEDType % 100 == 7)
                {
                    IStrEncryptDecryptIV = VCT_ENCRYPTION_AES_IVEC_LOW;
                    IStrEncryptDecryptKey = VCT_KEY256_HIGH1;
                }
                if ((int)AIntEDType % 100 == 8)
                {
                    IStrEncryptDecryptIV = VCT_ENCRYPTION_AES_IVEC_LOW;
                    IStrEncryptDecryptKey = VCT_KEY256_HIGH2;
                }
                if ((int)AIntEDType % 100 == 9)
                {
                    IStrEncryptDecryptIV = VCT_ENCRYPTION_AES_IVEC_LOW;
                    IStrEncryptDecryptKey = VCT_KEY256_HIGH3;
                }
                #endregion

                #region IVEC_NORMAL,KEY256_LOW
                if ((int)AIntEDType % 100 == 10)
                {
                    IStrEncryptDecryptIV = VCT_ENCRYPTION_AES_IVEC_NORMAL;
                    IStrEncryptDecryptKey = VCT_KEY256_LOW1;
                }
                if ((int)AIntEDType % 100 == 11)
                {
                    IStrEncryptDecryptIV = VCT_ENCRYPTION_AES_IVEC_NORMAL;
                    IStrEncryptDecryptKey = VCT_KEY256_LOW2;
                }
                if ((int)AIntEDType % 100 == 12)
                {
                    IStrEncryptDecryptIV = VCT_ENCRYPTION_AES_IVEC_NORMAL;
                    IStrEncryptDecryptKey = VCT_KEY256_LOW3;
                }
                #endregion

                #region IVEC_NORMAL,KEY256_NORMAL
                if ((int)AIntEDType % 100 == 13)
                {
                    IStrEncryptDecryptIV = VCT_ENCRYPTION_AES_IVEC_NORMAL;
                    IStrEncryptDecryptKey = VCT_KEY256_NORMAL1;
                }
                if ((int)AIntEDType % 100 == 14)
                {
                    IStrEncryptDecryptIV = VCT_ENCRYPTION_AES_IVEC_NORMAL;
                    IStrEncryptDecryptKey = VCT_KEY256_NORMAL2;
                }
                if ((int)AIntEDType % 100 == 15)
                {
                    IStrEncryptDecryptIV = VCT_ENCRYPTION_AES_IVEC_NORMAL;
                    IStrEncryptDecryptKey = VCT_KEY256_NORMAL3;
                }
                #endregion

                #region IVEC_NORMAL,KEY256_HIGH
                if ((int)AIntEDType % 100 == 16)
                {
                    IStrEncryptDecryptIV = VCT_ENCRYPTION_AES_IVEC_NORMAL;
                    IStrEncryptDecryptKey = VCT_KEY256_HIGH1;
                }
                if ((int)AIntEDType % 100 == 17)
                {
                    IStrEncryptDecryptIV = VCT_ENCRYPTION_AES_IVEC_NORMAL;
                    IStrEncryptDecryptKey = VCT_KEY256_HIGH2;
                }
                if ((int)AIntEDType % 100 == 18)
                {
                    IStrEncryptDecryptIV = VCT_ENCRYPTION_AES_IVEC_NORMAL;
                    IStrEncryptDecryptKey = VCT_KEY256_HIGH3;
                }
                #endregion

                #region IVEC_HIGH,KEY256_LOW
                if ((int)AIntEDType % 100 == 19)
                {
                    IStrEncryptDecryptIV = VCT_ENCRYPTION_AES_IVEC_HIGH;
                    IStrEncryptDecryptKey = VCT_KEY256_LOW1;
                }
                if ((int)AIntEDType % 100 == 20)
                {
                    IStrEncryptDecryptIV = VCT_ENCRYPTION_AES_IVEC_HIGH;
                    IStrEncryptDecryptKey = VCT_KEY256_LOW2;
                }
                if ((int)AIntEDType % 100 == 21)
                {
                    IStrEncryptDecryptIV = VCT_ENCRYPTION_AES_IVEC_HIGH;
                    IStrEncryptDecryptKey = VCT_KEY256_LOW3;
                }
                #endregion

                #region IVEC_HIGH,KEY256_NORMAL
                if ((int)AIntEDType % 100 == 22)
                {
                    IStrEncryptDecryptIV = VCT_ENCRYPTION_AES_IVEC_HIGH;
                    IStrEncryptDecryptKey = VCT_KEY256_NORMAL1;
                }
                if ((int)AIntEDType % 100 == 23)
                {
                    IStrEncryptDecryptIV = VCT_ENCRYPTION_AES_IVEC_HIGH;
                    IStrEncryptDecryptKey = VCT_KEY256_NORMAL2;
                }
                if ((int)AIntEDType % 100 == 24)
                {
                    IStrEncryptDecryptIV = VCT_ENCRYPTION_AES_IVEC_HIGH;
                    IStrEncryptDecryptKey = VCT_KEY256_NORMAL3;
                }
                #endregion

                #region IVEC_HIGH,KEY256_HIGH
                if ((int)AIntEDType % 100 == 25)
                {
                    IStrEncryptDecryptIV = VCT_ENCRYPTION_AES_IVEC_HIGH;
                    IStrEncryptDecryptKey = VCT_KEY256_HIGH1;
                }
                if ((int)AIntEDType % 100 == 26)
                {
                    IStrEncryptDecryptIV = VCT_ENCRYPTION_AES_IVEC_HIGH;
                    IStrEncryptDecryptKey = VCT_KEY256_HIGH2;
                }
                if ((int)AIntEDType % 100 == 27)
                {
                    IStrEncryptDecryptIV = VCT_ENCRYPTION_AES_IVEC_HIGH;
                    IStrEncryptDecryptKey = VCT_KEY256_HIGH3;
                }
                #endregion
            }
            catch { LStrReturn = "Error02"; }

            return LStrReturn;
        }

        private static string EncryptByte(byte[] AByteSource)
        {
            string LStrReturn = string.Empty;

            try
            {
                byte[] LByteArrayKey = Encoding.ASCII.GetBytes(IStrEncryptDecryptKey);
                byte[] LByteArrayIV = Encoding.ASCII.GetBytes(IStrEncryptDecryptIV);

                RijndaelManaged LRijndaelManaged = new RijndaelManaged();
                LRijndaelManaged.Key = LByteArrayKey;
                LRijndaelManaged.Mode = CipherMode.CBC;
                LRijndaelManaged.Padding = PaddingMode.None;
                LRijndaelManaged.IV = LByteArrayIV;

                ICryptoTransform LICryptoTransform = LRijndaelManaged.CreateEncryptor();
                byte[] LByteArrayResult = LICryptoTransform.TransformFinalBlock(AByteSource, 0, AByteSource.Length);

                LStrReturn = ConvertByteToHexStr(LByteArrayResult);
            }
            catch { LStrReturn = string.Empty; }

            return LStrReturn;
        }

        private static string AscCodeToChr(int AsciiCode)
        {
            System.Text.ASCIIEncoding asciiEncoding = new System.Text.ASCIIEncoding();
            byte[] byteArray = new byte[] { (byte)AsciiCode };
            string strCharacter = asciiEncoding.GetString(byteArray);
            return (strCharacter);
        }
        #endregion

        /// <summary>
        /// 字符串加密(SHA512)。加密后的密码不可逆。
        /// </summary>
        /// <param name="AStrSource">待加密字符串</param>
        /// <param name="AStrVerificationCode">验证码</param>
        /// <param name="AIntEDType">加密方法,如果 = 0：直接SHA512后返回，> 0 : SHA512后根据该参数进行AES256加密</param>
        /// <returns>如果返回值与AStrSource，表示加密失败
        /// Error01:加解密方法错误；
        /// Error02:验证码错误；
        /// </returns>
        public static string EncryptStringSHA512(string AStrSource, string AStrVerificationCode, UMPKeyAndIVType AIntEDType)
        {
            string LStrReturn = string.Empty;
            string LStrSource = string.Empty;

            try
            {
                LStrSource = AStrSource;

                if (!string.IsNullOrEmpty(LStrReturn)) { return LStrReturn; }
                byte[] LByteTemp;
                SHA512 LSHA512Class = new SHA512Managed();
                
                LByteTemp = LSHA512Class.ComputeHash(GetKeyByteArray(AStrSource));
                LSHA512Class.Clear();
                if (AIntEDType > 0)
                {
                    if ((int)AIntEDType > 100) { return "Error01"; }
                    LStrReturn = AnasysVerificationCodeAndKeyIV(AStrVerificationCode, AIntEDType);
                    if (!string.IsNullOrEmpty(LStrReturn)) { return LStrReturn; }
                    LStrReturn = EncryptByte(LByteTemp);
                }
                else
                {
                    LStrReturn = ConvertByteToHexStr(LByteTemp);
                }
            }
            catch { LStrReturn = AStrSource; }

            return LStrReturn;
        }

        /// <summary>
        /// 字符串加密(SHA256)。加密后的密码不可逆。
        /// </summary>
        /// <param name="AStrSource">待加密字符串</param>
        /// <returns>如果返回值与AStrSource，表示加密失败</returns>
        public static string EncryptStringSHA256(string AStrSource)
        {
            string LStrReturn = string.Empty;
            string LStrSource = string.Empty;

            try
            {
                LStrSource = AStrSource;

                if (!string.IsNullOrEmpty(LStrReturn)) { return LStrReturn; }
                byte[] LByteTemp;

                SHA256 LSHA256Class = new SHA256Managed();
                LByteTemp = LSHA256Class.ComputeHash(GetKeyByteArray(AStrSource));
                LStrReturn = ConvertByteToHexStr(LByteTemp);
            }
            catch { LStrReturn = AStrSource; }

            return LStrReturn;
        }

        /// <summary>
        /// 字符串加密或解密
        /// </summary>
        /// <param name="AStrSource">待加密或解密字符串</param>
        /// <param name="AStrVerificationCode">验证码</param>
        /// <param name="AIntEDType">加密或解密方法 (1、101)：IVEC_LOW,KEY256_LOW1；(4、104)：IVEC_LOW,KEY256_NORMAL1</param>
        /// <returns>
        /// Error01:加解密方法错误；
        /// Error02:验证码错误；
        /// Error03:待加密字符串长度错误，最大只能支持128
        /// 如果返回值与AStrSource，表示加密或解密失败
        /// </returns>
        public static string EncryptDecryptString(string AStrSource, string AStrVerificationCode, UMPKeyAndIVType AIntEDType)
        {
            string LStrReturn = string.Empty;
            string LStrSource = string.Empty;

            int LIntMaxLength = 0;

            try
            {
                LStrReturn = AnasysVerificationCodeAndKeyIV(AStrVerificationCode, AIntEDType);
                if (!string.IsNullOrEmpty(LStrReturn)) { return LStrReturn; }
                if (string.IsNullOrEmpty(IStrEncryptDecryptIV) || string.IsNullOrEmpty(IStrEncryptDecryptKey)) { return "Error01"; }

                byte[] LByteArrayKey = Encoding.ASCII.GetBytes(IStrEncryptDecryptKey);
                byte[] LByteArrayAexIv = Encoding.ASCII.GetBytes(IStrEncryptDecryptIV);

                RijndaelManaged LRijndaelManaged = new RijndaelManaged();
                LRijndaelManaged.Key = LByteArrayKey;
                LRijndaelManaged.Mode = CipherMode.CBC;
                LRijndaelManaged.Padding = PaddingMode.None;
                LRijndaelManaged.IV = LByteArrayAexIv;

                if ((int)AIntEDType < 100)
                {
                    //字符串加密
                    LStrSource = AStrSource;
                    LIntMaxLength = (int)Math.Ceiling(((double)LStrSource.Length / (double)8)) * 8;
                    if (LIntMaxLength > 128) { return "Error03"; }
                    while (LStrSource.Length < LIntMaxLength) { LStrSource += "\0"; }

                    byte[] LByteArrayEncrypt = Encoding.Unicode.GetBytes(LStrSource);
                    ICryptoTransform LICryptoTransform = LRijndaelManaged.CreateEncryptor();
                    byte[] LByteArrayResult = LICryptoTransform.TransformFinalBlock(LByteArrayEncrypt, 0, LByteArrayEncrypt.Length);
                    LStrReturn = ConvertByteToHexStr(LByteArrayResult);
                }
                else
                {
                    //字符串解密
                    LStrSource = ConvertHexToString(AStrSource);
                    char[] LCharTemp = LStrSource.ToCharArray();
                    byte[] LByteEncrypt = new byte[LCharTemp.Length];
                    for (int LIntLoop = 0; LIntLoop < LCharTemp.Length; LIntLoop++) { LByteEncrypt[LIntLoop] = Convert.ToByte(LCharTemp[LIntLoop]); }
                    ICryptoTransform LICryptoTransform = LRijndaelManaged.CreateDecryptor();
                    byte[] LByteArrayResult = LICryptoTransform.TransformFinalBlock(LByteEncrypt, 0, LByteEncrypt.Length);

                    LStrReturn = Encoding.Unicode.GetString(LByteArrayResult);
                    if (LStrReturn.IndexOf("\0") > 0)
                    {
                        LStrReturn = LStrReturn.Substring(0, LStrReturn.IndexOf("\0"));
                    }
                }
            }
            catch { LStrReturn = AStrSource; }

            return LStrReturn;
        }

        public static byte[] EncryptDecryptBytes(byte[] byteSource, string code, UMPKeyAndIVType type)
        {
            string strResult = AnasysVerificationCodeAndKeyIV(code, type);
            if (!string.IsNullOrEmpty(strResult))
            {
                throw new AuthenticationException("Vetification code invalid");
            }
            byte[] byteKey = Encoding.ASCII.GetBytes(IStrEncryptDecryptKey);
            byte[] byteIV = Encoding.ASCII.GetBytes(IStrEncryptDecryptIV);
            var rijindeal = new RijndaelManaged();
            rijindeal.Key = byteKey;
            rijindeal.IV = byteIV;
            rijindeal.Mode = CipherMode.CBC;
            rijindeal.Padding = PaddingMode.None;
            ICryptoTransform transform;
            if ((int) type > 100)
            {
                //解密
                transform = rijindeal.CreateDecryptor();
            }
            else
            {
                //加密
                transform = rijindeal.CreateEncryptor();
            }
            var byteResult = transform.TransformFinalBlock(byteSource, 0, byteSource.Length);
            return byteResult;
        } 
        
    }
}
