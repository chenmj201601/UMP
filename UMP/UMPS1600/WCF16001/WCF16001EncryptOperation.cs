using PFShareClassesS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WCF16001
{
    public class WCF16001EncryptOperation
    {
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

        public static string DecryptWithM002(string strSource)
        {
            string LStrVerificationCode102 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M102);
            return EncryptionAndDecryption.EncryptDecryptString(strSource, LStrVerificationCode102, EncryptionAndDecryption.UMPKeyAndIVType.M102);
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


    }
}