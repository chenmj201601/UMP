using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using UMPService09.Log;
using System.Text.RegularExpressions;
using UMPService09.Model;
using UMPService09.Utility;
using PFShareClassesS;

namespace UMPService09.DAL
{
     public class DALSkillInfo:BasicMethod
    {
        /// <summary>
        /// 得到技能组信息
        /// </summary>
        /// <param name="AListSkillInfo"></param>
        /// <param name="AStrRent"></param>
         public static void GetSkillInfo(DataBaseConfig ADataBaseConfig, ref List<ObjectInfo> AListSkillInfo, string AStrRent)
        {
            AListSkillInfo.Clear();

            DataTable LDataTableReturn = new DataTable();
            string LStrDynamicSQL = string.Empty;
            string LStrRentToken = string.Empty;
            string LStrSingleObject = string.Empty;

            try
            {
                DatabaseOperation01Return LDatabaseOperationReturn = new DatabaseOperation01Return();
                DataOperations01 LDataOperations = new DataOperations01();

                LStrDynamicSQL = string.Format("SELECT * FROM T_11_009_{0} WHERE C000 = 2 AND C004 = 1 ORDER BY C002"
                            , AStrRent);
                LDatabaseOperationReturn = LDataOperations.SelectDataByDynamicSQL(ADataBaseConfig.IntDatabaseType, ADataBaseConfig.StrDatabaseProfile, LStrDynamicSQL);
                if (!LDatabaseOperationReturn.BoolReturn)
                {
                    LDataTableReturn = null;
                }
                else
                {
                    LDataTableReturn = LDatabaseOperationReturn.DataSetReturn.Tables[0];
                    foreach (DataRow LDataRowSingleRow in LDataTableReturn.Rows)
                    {
                        ObjectInfo skillInfoTemp = new ObjectInfo();
                        skillInfoTemp.ObjID = LongParse(LDataRowSingleRow["C001"].ToString(), 0);
                        skillInfoTemp.ObjName = EncryptionAndDecryption.EncryptDecryptString(LDataRowSingleRow["C008"].ToString(), IStrVerificationCode102, EncryptionAndDecryption.UMPKeyAndIVType.M102);
                        skillInfoTemp.ObjType = 6;
                        AListSkillInfo.Add(skillInfoTemp);
                    }
                }
            }
            catch (Exception ex)
            {
                LDataTableReturn = null;
                FileLog.WriteInfo("GetSkillInfo()", ex.Message);
            }
        }
    }
}
