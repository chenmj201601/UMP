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
    public class DALUserInfo:BasicMethod
    {
        /// <summary>
        /// 得到所有的用户信息
        /// </summary>
        /// <param name="AListUserInfo"></param>
        /// <param name="AStrRent"></param>
        public static void GetAllUserInfo(DataBaseConfig ADataBaseConfig,GlobalSetting AGolbalSetting, ref List<ObjectInfo> AListUserInfo)
        {
            AListUserInfo.Clear();
            DataTable LDataTableReturn = new DataTable();
            string LStrDynamicSQL = string.Empty;
            string LStrRentToken = string.Empty;
            string LStrSingleObject = string.Empty;

            try
            {
                DatabaseOperation01Return LDatabaseOperationReturn = new DatabaseOperation01Return();
                DataOperations01 LDataOperations = new DataOperations01();

                LStrDynamicSQL = string.Format("SELECT * FROM T_11_005_{0} WHERE C007<>'H' AND C010='1' AND C011='0' "
                            ,AGolbalSetting.StrRent);
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
                        ObjectInfo userInfoTemp = new ObjectInfo();
                        userInfoTemp.ObjID = LongParse(LDataRowSingleRow["C001"].ToString(), 0);
                        userInfoTemp.ObjName = EncryptionAndDecryption.EncryptDecryptString(LDataRowSingleRow["C002"].ToString(), IStrVerificationCode102, EncryptionAndDecryption.UMPKeyAndIVType.M102);
                        userInfoTemp.BeyondOrgID = LongParse(LDataRowSingleRow["C006"].ToString(), 0);
                        userInfoTemp.ObjType = 3;
                        AListUserInfo.Add(userInfoTemp);
                    }
                }
            }
            catch (Exception ex)
            {
                LDataTableReturn = null;
                FileLog.WriteInfo("GetAllUserInfo()", ex.Message);
            }
        }
    }
}
