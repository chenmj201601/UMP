using PFShareClassesS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using VoiceCyber.UMP.Common;

namespace Common2400
{
    public class S2400EncryptOperation
    {
        public static string DecodeEncryptValue(string strValue)
        {
            string strReturn = string.Empty;
            //加密的(以连续三个char27开头）
            if (strValue.StartsWith(string.Format("{0}{0}{0}", ConstValue.SPLITER_CHAR)))
            {
                strValue = strValue.Substring(3);
                string[] arrContent = strValue.Split(new[] { ConstValue.SPLITER_CHAR }, StringSplitOptions.None);
                string strVersion = string.Empty, strMode = string.Empty, strPass = strValue;
                if (arrContent.Length > 0)
                {
                    strVersion = arrContent[0];
                }
                if (arrContent.Length > 1)
                {
                    strMode = arrContent[1];
                }
                if (arrContent.Length > 2)
                {
                    strPass = arrContent[2];
                }
                if (strVersion == "2" && strMode == "hex")
                {
                    strReturn = DecryptFromDB(strPass);
                }
                else if (strVersion == "3" && strMode == "hex")
                {
                    strReturn = DecryptFromDB(strPass);
                }
                else
                {
                    strReturn = strPass;
                }
            }
            else
            {
                strReturn = strValue;
            }

            return strReturn;
        }

        public static string DecryptFromDB(string strSource)
        {
            string strReturn = EncryptionAndDecryption.EncryptDecryptString(strSource,
              CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M102),
              EncryptionAndDecryption.UMPKeyAndIVType.M102);
            return strReturn;
        }

        private static string CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType keyIVID)
        {
            string strReturn;
            Random random = new Random();
            string strTemp;

            try
            {
                strReturn = DateTime.Now.ToString("yyyyMMddHHmmss");
                int intRand = random.Next(0, 14);
                strTemp = intRand.ToString("00");
                strReturn = strReturn.Insert(intRand, "VCT");
                intRand = random.Next(0, 17);
                strTemp += intRand.ToString("00");
                strReturn = strReturn.Insert(intRand, "UMP");
                intRand = random.Next(0, 20);
                strTemp += intRand.ToString("00");
                strReturn = strReturn.Insert(intRand, ((int)keyIVID).ToString("000"));

                strReturn = EncryptionAndDecryption.EncryptStringY(strTemp + strReturn);
            }
            catch { strReturn = string.Empty; }

            return strReturn;
        }

        /// <summary>
        /// 用M004加密
        /// </summary>
        /// <param name="strSource"></param>
        /// <returns></returns>
        public static string EncryptWithM004(string strSource)
        {
            string LStrVerificationCode004 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004);
            return EncryptionAndDecryption.EncryptDecryptString(strSource, LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
        }

        /// <summary>
        /// 用M104解密
        /// </summary>
        /// <param name="strSource"></param>
        /// <returns></returns>
        public static string DecryptWithM004(string strSource)
        {
            string LStrVerificationCode104 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104);
            return EncryptionAndDecryption.EncryptDecryptString(strSource, LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
        }

        public static string EncryptWithM002(string strSource)
        {
             string LStrVerificationCode002 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M002);
            return EncryptionAndDecryption.EncryptDecryptString(strSource, LStrVerificationCode002, EncryptionAndDecryption.UMPKeyAndIVType.M002);
        }

        public static string DecryptWithM002(string strSource)
        {
            string LStrVerificationCode102 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M102);
            return EncryptionAndDecryption.EncryptDecryptString(strSource, LStrVerificationCode102, EncryptionAndDecryption.UMPKeyAndIVType.M102);
        }

        public static string EncodeEncryptValue(int iEncryptMode, string strValue)
        {
            string strReturn = string.Empty;
            //加密的
            if (iEncryptMode > 0)
            {
                string strStart = string.Format("{0}{0}{0}", ConstValue.SPLITER_CHAR);
                switch (iEncryptMode)
                {
                    case (int)ObjectPropertyEncryptMode.E2Hex:
                        strReturn = string.Format("{0}2{1}hex{1}{2}", strStart, ConstValue.SPLITER_CHAR,
                        EncryptToDB(strValue));
                        break;
                    case (int)ObjectPropertyEncryptMode.SHA256:
                        strReturn = string.Format("{0}3{1}hex{1}{2}", strStart, ConstValue.SPLITER_CHAR,
                        EncryptToDB(strValue));
                        break;
                }
            }
            return strReturn;
        }

        private static string EncryptToDB(string strSource)
        {
            string strReturn = EncryptionAndDecryption.EncryptDecryptString(strSource,
            CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M002),
            EncryptionAndDecryption.UMPKeyAndIVType.M002);
            return strReturn;
        }

        public static string DecryptLL256(string AStrSource)
        {
            string LStrReturn = string.Empty;
            string LStrSource = string.Empty;

            if (AStrSource.Length == 0 || AStrSource == string.Empty) { return ""; }

            LStrSource = ConvertHexToString(AStrSource);

            byte[] LByteArrayKey256Low1 = Encoding.ASCII.GetBytes(S2400Const.VCT_KEY256_LOW1);
            byte[] LByteArrayAexIv = Encoding.ASCII.GetBytes(S2400Const.VCT_ENCRYPTION_AES_IVEC_LOW);
            char[] LCharTemp = LStrSource.ToCharArray();

            byte[] LByteEncrypt = new byte[LCharTemp.Length];
            for (int LIntLoop = 0; LIntLoop < LCharTemp.Length; LIntLoop++) { LByteEncrypt[LIntLoop] = Convert.ToByte(LCharTemp[LIntLoop]); }

            RijndaelManaged LRijndaelManaged = new RijndaelManaged();
            LRijndaelManaged.Key = LByteArrayKey256Low1;
            LRijndaelManaged.Mode = CipherMode.CBC;
            LRijndaelManaged.Padding = PaddingMode.None;
            LRijndaelManaged.IV = LByteArrayAexIv;

            ICryptoTransform LICryptoTransform = LRijndaelManaged.CreateDecryptor();
            byte[] LByteArrayResult = LICryptoTransform.TransformFinalBlock(LByteEncrypt, 0, LByteEncrypt.Length);

            LStrReturn = Encoding.Unicode.GetString(LByteArrayResult);
            LStrReturn = LStrReturn.Substring(0, LStrReturn.IndexOf("\0"));

            return LStrReturn;
        }

        public static string ConvertHexToString(string AStrHexValue)
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

        public static string ConvertSessionToCerification(string AStrSessionCode)
        {
            string LocalStrCerification = string.Empty;
            string LocalStrSourceString = string.Empty;

            LocalStrSourceString = ConvertHexToString(AStrSessionCode);
            char[] LocalCharSourceArray = LocalStrSourceString.ToCharArray();
            byte[] LocalByteSourceArray = new byte[LocalCharSourceArray.Length];
            for (int LIntLoop = 0; LIntLoop < LocalCharSourceArray.Length; LIntLoop++) { LocalByteSourceArray[LIntLoop] = Convert.ToByte(LocalCharSourceArray[LIntLoop]); }

            int LocalIntOffset = LocalByteSourceArray[0] & 0x001f;

            byte LocalByteMask = (byte)(LocalByteSourceArray[LocalIntOffset] ^ LocalIntOffset);

            byte[] LocalByteCerification = new byte[32];

            for (int LIntLoop = 0; LIntLoop < 32; LIntLoop++) { LocalByteCerification[LIntLoop] = (byte)(LocalByteSourceArray[LIntLoop] ^ LocalByteMask); }

            LocalStrCerification = ConvertByteToHexStr(LocalByteCerification);

            return LocalStrCerification;
        }

        public static string ConvertByteToHexStr(byte[] ABytesValue)
        {
            string LStrReturn = "";

            if (ABytesValue != null)
            {
                for (int LIntLoopByte = 0; LIntLoopByte < ABytesValue.Length; LIntLoopByte++) { LStrReturn += ABytesValue[LIntLoopByte].ToString("X2"); }
            }

            return LStrReturn;
        }

        /// <summary>
        /// 用M104解密
        /// </summary>
        /// <param name="strSource"></param>
        /// <returns></returns>
        public static string DecryptWithM025(string strSource)
        {
            string LStrVerificationCode125 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M125);
            return EncryptionAndDecryption.EncryptDecryptString(strSource, LStrVerificationCode125, EncryptionAndDecryption.UMPKeyAndIVType.M125);
        }
    }
}
