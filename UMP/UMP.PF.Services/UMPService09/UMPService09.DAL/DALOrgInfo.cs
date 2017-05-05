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
    //机构信息
    public class DALOrgInfo : BasicMethod
    {
        /// <summary>
        /// 得到所有的机构
        /// </summary>
        /// <param name="AListOrgInfo"></param>
        /// <param name="AStrRent"></param>
        public static void GetAllOrgInfo(DataBaseConfig ADataBaseConfig, ref List<ObjectInfo> AListOrgInfo, string AStrRent)
        {
            AListOrgInfo.Clear();
            DataTable LDataTableReturn = new DataTable();
            string LStrDynamicSQL = string.Empty;
            string LStrRentToken = string.Empty;
            string LStrSingleObject = string.Empty;

            try
            {
                DatabaseOperation01Return LDatabaseOperationReturn = new DatabaseOperation01Return();
                DataOperations01 LDataOperations = new DataOperations01();

                LStrDynamicSQL = string.Format("SELECT * FROM T_11_006_{0} WHERE C005='1' "
                            , AStrRent);
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
                        ObjectInfo orgInfoTemp = new ObjectInfo();
                        orgInfoTemp.ObjID = LongParse(LDataRowSingleRow["C001"].ToString(), 0);
                        orgInfoTemp.ObjName = EncryptionAndDecryption.EncryptDecryptString(LDataRowSingleRow["C002"].ToString(), IStrVerificationCode102, EncryptionAndDecryption.UMPKeyAndIVType.M102);
                        orgInfoTemp.ObjType = 5;
                        orgInfoTemp.ParentOrgID = LongParse(LDataRowSingleRow["C004"].ToString(), 0);
                        AListOrgInfo.Add(orgInfoTemp);
                    }
                }
            }
            catch (Exception ex)
            {
                LDataTableReturn = null;
                FileLog.WriteInfo("GetAllOrgInfo()", ex.Message);
            }
        }
    }
}
