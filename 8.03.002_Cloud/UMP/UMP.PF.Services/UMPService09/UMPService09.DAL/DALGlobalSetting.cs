using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using UMPService09.Log;
using UMPService09.Utility;
using PFShareClassesS;
using UMPService09.Model;


namespace UMPService09.DAL
{
    public class DALGlobalSetting :BasicMethod
    {

        /// <summary>
        /// 得到该租户的月和周的设定
        /// 12010101每周开始于默认值为0,表示从周日晚上24点开始
        /// 0为周日，1星期一，6为星期六
        /// 12010102每月开始于默认值为1
        /// 1为自然月,2为2号,最大28为28号
        /// 12010401 为分机和座席 E为分机 A为座席 E char(27)A为座席+分机 R为真实分机
        /// </summary>
        public static void GetGlobalSetting(DataBaseConfig ADataBaseConfig, ref string AStrParamValue, string AStrRent, string AStrParamNumber)
        {
            AStrParamValue = string.Empty;
            DataTable LDataTableReturn = new DataTable();
            string LStrDynamicSQL = string.Empty;
            string LStrRentToken = string.Empty;
            string LStrSingleObject = string.Empty;

            try
            {
                DatabaseOperation01Return LDatabaseOperationReturn = new DatabaseOperation01Return();
                DataOperations01 LDataOperations = new DataOperations01();

                LStrDynamicSQL = string.Format("SELECT * FROM T_11_001_{0} WHERE C003={1}"
                            , AStrRent
                            , AStrParamNumber);
                LDatabaseOperationReturn = LDataOperations.SelectDataByDynamicSQL(ADataBaseConfig.IntDatabaseType,ADataBaseConfig.StrDatabaseProfile, LStrDynamicSQL);
                if (!LDatabaseOperationReturn.BoolReturn)
                {
                    LDataTableReturn = null;
                }
                else
                {
                    LDataTableReturn = LDatabaseOperationReturn.DataSetReturn.Tables[0];
                    foreach (DataRow LDataRowSingleRow in LDataTableReturn.Rows)
                    {
                        AStrParamValue = EncryptionAndDecryption.EncryptDecryptString(LDataRowSingleRow["C006"].ToString(), IStrVerificationCode102, EncryptionAndDecryption.UMPKeyAndIVType.M102).Trim(' ').Substring(AStrParamNumber.Length);
                    }
                }
            }
            catch (Exception ex)
            {
                LDataTableReturn = null;
                FileLog.WriteInfo("GetGlobalSetting()", ex.Message);
            }
        }
    }
}
