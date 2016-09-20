using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PFShareClassesC;

namespace UMPCommon
{
    class EncryDecryHelper
    {
        private static object syncRoot = new object();
        #region EncryOrDecry
        static public string EncryptionCommuString(string DecString)
        {
            lock (syncRoot)
            {
                return EncryptionAndDecryption.EncryptDecryptString(
                    DecString,
                    CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004),
                    EncryptionAndDecryption.UMPKeyAndIVType.M004
                    );
            }
        }

        static public string DecryptionCommuString(string EncString)
        {
            lock (syncRoot)
            {
                return EncryptionAndDecryption.EncryptDecryptString(
                    EncString,
                    CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104),
                    EncryptionAndDecryption.UMPKeyAndIVType.M104
                    );
            }
        }

        static public string EncryptionXMLString(string DecString)
        {
            lock (syncRoot)
            {
                return EncryptionAndDecryption.EncryptDecryptString(
                    DecString,
                    CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M001),
                    EncryptionAndDecryption.UMPKeyAndIVType.M001
                    );
            }
        }

        static public string DecryptionXMLString(string EncString)
        {
            lock (syncRoot)
            {
                return EncryptionAndDecryption.EncryptDecryptString(
                    EncString,
                    CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M101),
                    EncryptionAndDecryption.UMPKeyAndIVType.M101
                    );
            }
        }

        static private string CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType AKeyIVID)
        {
            lock (syncRoot)
            {
                string LStrReturn = string.Empty;
                int LIntRand = 0;
                string LStrTemp = string.Empty;

                try
                {
                    Random LRandom = new Random();
                    LStrReturn = DateTime.Now.ToString("yyyyMMddHHmmss");
                    LIntRand = LRandom.Next(0, 14);
                    LStrTemp = LIntRand.ToString("00");
                    LStrReturn = LStrReturn.Insert(LIntRand, "VCT");
                    LIntRand = LRandom.Next(0, 17);
                    LStrTemp += LIntRand.ToString("00");
                    LStrReturn = LStrReturn.Insert(LIntRand, "UMP");
                    LIntRand = LRandom.Next(0, 20);
                    LStrTemp += LIntRand.ToString("00");
                    LStrReturn = LStrReturn.Insert(LIntRand, ((int)AKeyIVID).ToString("000"));

                    LStrReturn = EncryptionAndDecryption.EncryptStringY(LStrTemp + LStrReturn);
                }
                catch { LStrReturn = string.Empty; }

                return LStrReturn;
            }
        }
        #endregion
    }
}
