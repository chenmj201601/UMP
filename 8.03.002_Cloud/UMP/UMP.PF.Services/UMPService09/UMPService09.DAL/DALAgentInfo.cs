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
    public class DALAgentInfo : BasicMethod
    {
        /// <summary>
        /// 得到所有座席信息
        /// </summary>
        /// <param name="AListAgentInfo"></param>
        /// <param name="AStrRent"></param>
        public static void GetAllAgentInfo(DataBaseConfig ADataBaseConfig, ref List<ObjectInfo> AListAgentInfo, GlobalSetting AGolbalSetting)
        {
            AListAgentInfo.Clear();
            DataTable LDataTableReturn = new DataTable();
            string LStrDynamicSQL = string.Empty;
            string LStrRentToken = string.Empty;
            string LStrSingleObject = string.Empty;

            try
            {
                DatabaseOperation01Return LDatabaseOperationReturn = new DatabaseOperation01Return();
                DataOperations01 LDataOperations = new DataOperations01();

                LStrDynamicSQL = string.Format("SELECT * FROM T_11_101_{0} WHERE C001 >= 1030000000000000000 AND C001 < 1040000000000000000 AND C002=1  AND C012='1' "
                            , AGolbalSetting.StrRent);
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
                        ObjectInfo agentInfoTemp = new ObjectInfo();
                        agentInfoTemp.ObjID = LongParse(LDataRowSingleRow["C001"].ToString(), 0);
                        agentInfoTemp.BeyondOrgID = LongParse(LDataRowSingleRow["C011"].ToString(), 0);
                        agentInfoTemp.ObjName = EncryptionAndDecryption.EncryptDecryptString(LDataRowSingleRow["C017"].ToString(), IStrVerificationCode102, EncryptionAndDecryption.UMPKeyAndIVType.M102);
                        agentInfoTemp.ObjType = 1;
                        AListAgentInfo.Add(agentInfoTemp);
                    }
                }
            }
            catch (Exception ex)
            {
                LDataTableReturn = null;
                FileLog.WriteInfo("GetAllAgentInfo()", ex.Message);
            }
        }
    }
}
