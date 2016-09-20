using PFShareClassesC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UMP.Tools.PublicClasses
{
    public static class EncryptionAndDescryption
    {
        public static string CreateVerificationCode(int AIntKeyIVID)
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
                LStrReturn = LStrReturn.Insert(LIntRand, AIntKeyIVID.ToString("000"));

                LStrReturn = EncryptionAndDecryption.EncryptStringY(LStrTemp + LStrReturn);
            }
            catch { LStrReturn = string.Empty; }

            return LStrReturn;
        }

        public static string EncryptDecryptString(string AStrSource, string AStrVerificationCode, int AIntKeyIVID)
        {
            string LStrRetrun = string.Empty;
            EncryptionAndDecryption.UMPKeyAndIVType LKeyIVID = EncryptionAndDecryption.UMPKeyAndIVType.M004;

            if (AIntKeyIVID == 1) { LKeyIVID = EncryptionAndDecryption.UMPKeyAndIVType.M001; }
            if (AIntKeyIVID == 4) { LKeyIVID = EncryptionAndDecryption.UMPKeyAndIVType.M004; }
            if (AIntKeyIVID == 101) { LKeyIVID = EncryptionAndDecryption.UMPKeyAndIVType.M101; }
            if (AIntKeyIVID == 104) { LKeyIVID = EncryptionAndDecryption.UMPKeyAndIVType.M104; }

            LStrRetrun = EncryptionAndDecryption.EncryptDecryptString(AStrSource, AStrVerificationCode, LKeyIVID);
            return LStrRetrun;
        }

        public static string EncryptDecryptString(string AStrSource, int AIntKeyIVID)
        {
            string LStrRetrun = string.Empty;
            string LStrVerificationCode = string.Empty;

            LStrVerificationCode = CreateVerificationCode(AIntKeyIVID);
            LStrRetrun = EncryptDecryptString(AStrSource, LStrVerificationCode, AIntKeyIVID);

            return LStrRetrun;
        }
    }
}
