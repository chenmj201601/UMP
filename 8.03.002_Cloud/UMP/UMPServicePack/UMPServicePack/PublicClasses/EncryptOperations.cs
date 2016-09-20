using System;
using System.Security.Cryptography;
using System.Text;
using UMPServicePackCommon;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Encryptions;

namespace UMPServicePack.PublicClasses
{
    /// <summary>
    /// 加解密操作
    /// </summary>
    public class EncryptOperations
    {
        public static string DecodeEncryptValue(string strValue)
        {
            string strReturn = string.Empty;
            //加密的(以连续三个char27开头）
            if (strValue.StartsWith(string.Format("{0}{0}{0}", ConstDefines.SPLITER_CHAR)))
            {
                strValue = strValue.Substring(3);
                string[] arrContent = strValue.Split(new[] { ConstDefines.SPLITER_CHAR }, StringSplitOptions.None);
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
            try
            {
                return ServerAESEncryption.DecryptString(strSource, EncryptionMode.AES256V02Hex);
            }
            catch
            {
                return strSource;
            }
        }

        /// <summary>
        /// 用M004加密
        /// </summary>
        /// <param name="strSource"></param>
        /// <returns></returns>
        public static string EncryptWithM004(string strSource)
        {
            try
            {
                return ServerAESEncryption.EncryptString(strSource, EncryptionMode.AES256V04Hex);
            }
            catch
            {
                return strSource;
            }
        }

        /// <summary>
        /// 用M104解密
        /// </summary>
        /// <param name="strSource"></param>
        /// <returns></returns>
        public static string DecryptWithM004(string strSource)
        {
            try
            {
                return ServerAESEncryption.DecryptString(strSource, EncryptionMode.AES256V04Hex);
            }
            catch
            {
                return strSource;
            }
        }

        public static string EncryptWithM002(string strSource)
        {
            try
            {
                return ServerAESEncryption.EncryptString(strSource, EncryptionMode.AES256V02Hex);
            }
            catch
            {
                return strSource;
            }
        }

        public static string DecryptWithM002(string strSource)
        {
            try
            {
                return ServerAESEncryption.DecryptString(strSource, EncryptionMode.AES256V02Hex);
            }
            catch
            {
                return strSource;
            }
        }

        public static string EncodeEncryptValue(int iEncryptMode, string strValue)
        {
            string strReturn = string.Empty;
            //加密的
            if (iEncryptMode > 0)
            {
                string strStart = string.Format("{0}{0}{0}", ConstDefines.SPLITER_CHAR);
                switch (iEncryptMode)
                {
                    case (int)ConstDefines.ObjectPropertyEncryptMode.E2Hex:
                        strReturn = string.Format("{0}2{1}hex{1}{2}", strStart, ConstDefines.SPLITER_CHAR,
                        EncryptToDB(strValue));
                        break;
                    case (int)ConstDefines.ObjectPropertyEncryptMode.SHA256:
                        strReturn = string.Format("{0}3{1}hex{1}{2}", strStart, ConstDefines.SPLITER_CHAR,
                        EncryptToDB(strValue));
                        break;
                }
            }
            return strReturn;
        }

        private static string EncryptToDB(string strSource)
        {
            try
            {
                return ServerAESEncryption.EncryptString(strSource, EncryptionMode.AES256V02Hex);
            }
            catch
            {
                return strSource;
            }
        }

        public static string DecryptLL256(string AStrSource)
        {
            string LStrReturn = string.Empty;
            string LStrSource = string.Empty;

            if (AStrSource.Length == 0 || AStrSource == string.Empty) { return ""; }

            LStrSource = ConvertHexToString(AStrSource);

            byte[] LByteArrayKey256Low1 = Encoding.ASCII.GetBytes(ConstDefines.VCT_KEY256_LOW1);
            byte[] LByteArrayAexIv = Encoding.ASCII.GetBytes(ConstDefines.VCT_ENCRYPTION_AES_IVEC_LOW);
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
        /// 用M125解密
        /// </summary>
        /// <param name="strSource"></param>
        /// <returns></returns>
        public static string DecryptWithM025(string strSource)
        {
            try
            {
                return ServerAESEncryption.DecryptString(strSource, EncryptionMode.AES256V25Hex);
            }
            catch
            {
                return strSource;
            }
        }

        /// <summary>
        ///  用户密码加密
        /// </summary>
        /// <param name="strUserID"></param>
        /// <param name="strPwdInput"></param>
        /// <returns></returns>
        public static string EncryptUserPwd(string strUserID, string strPwdInput)
        {
            try
            {
                string str = strUserID + strPwdInput;
                byte[] temp = ServerHashEncryption.EncryptBytes(Encoding.Unicode.GetBytes(str),
                    EncryptionMode.SHA512V00Hex);
                var aes = ServerAESEncryption.EncryptBytes(temp, EncryptionMode.AES256V02Hex);
                return ServerEncryptionUtils.Byte2Hex(aes);
            }
            catch
            {
                return strUserID+strPwdInput;
            }
        }
    }
}
