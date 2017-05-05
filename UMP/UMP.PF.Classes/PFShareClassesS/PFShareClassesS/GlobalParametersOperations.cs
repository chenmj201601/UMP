using PFShareClasses01;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PFShareClassesS
{
    public class GlobalParametersOperations
    {
        public OperationsReturn GetParameterSettedValue(int AIntDBType, string AStrDBConnectProfile, string AStrRentToken, string AStrParameterID)
        {
            OperationsReturn LClassReturn = new OperationsReturn();

            string LStrDynamicSQL = string.Empty;
            string LStrVerificationCode102 = string.Empty;
            string LStr11001006 = string.Empty;

            try
            {
                DataOperations01 LDataOperations = new DataOperations01();
                DatabaseOperation01Return LOperationReturn = new DatabaseOperation01Return();

                LStrVerificationCode102 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M102);

                LStrDynamicSQL = "SELECT C006 FROM T_11_001_" + AStrRentToken + " WHERE C003 = " + AStrParameterID;
                LOperationReturn = LDataOperations.SelectDataByDynamicSQL(AIntDBType, AStrDBConnectProfile, LStrDynamicSQL);
                LClassReturn.StrReturn = LOperationReturn.StrReturn;
                if (!LOperationReturn.BoolReturn)
                {
                    LClassReturn.BoolReturn = false;
                    return LClassReturn;
                }
                LStr11001006 = LOperationReturn.DataSetReturn.Tables[0].Rows[0][0].ToString();
                LStr11001006 = EncryptionAndDecryption.EncryptDecryptString(LStr11001006, LStrVerificationCode102, EncryptionAndDecryption.UMPKeyAndIVType.M102);
                LStr11001006 = LStr11001006.Substring(8);
                LClassReturn.StrReturn = LStr11001006;
            }
            catch (Exception ex)
            {
                LClassReturn.BoolReturn = false;
                LClassReturn.StrReturn = "GlobalParametersOperations.GetParameterSettedValue()\n" + ex.ToString();
            }

            return LClassReturn;
        }

        private string CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType AKeyIVID)
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

    public class OperationsReturn
    {
        public bool BoolReturn = true;
        public string StrReturn = string.Empty;
        public List<string> ListStrReturn = new List<string>();
    }
}
