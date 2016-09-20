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
     public class DALKPIDefine:BasicMethod
    {
        /// <summary>
        /// 得到所有的用户信息
        /// </summary>
        /// <param name="AListUserInfo"></param>
        /// <param name="AStrRent"></param>
        public static void GetAllKPIDefine(DataBaseConfig ADataBaseConfig, GlobalSetting AGolbalSetting, ref List<KPIDefine> AListKPIDefineInfo)
        {
            AListKPIDefineInfo.Clear();
            DataTable LDataTableReturn = new DataTable();
            string LStrDynamicSQL = string.Empty;
            string LStrRentToken = string.Empty;
            string LStrSingleObject = string.Empty;

            try
            {
                DatabaseOperation01Return LDatabaseOperationReturn = new DatabaseOperation01Return();
                DataOperations01 LDataOperations = new DataOperations01();

                LStrDynamicSQL = string.Format("SELECT * FROM T_46_001_{0} WHERE C009='1' "
                            , AGolbalSetting.StrRent);
                LDatabaseOperationReturn = LDataOperations.SelectDataByDynamicSQL(ADataBaseConfig.IntDatabaseType, ADataBaseConfig.StrDatabaseProfile, LStrDynamicSQL);
                if (!LDatabaseOperationReturn.BoolReturn)
                {
                    LDataTableReturn = null;
                }
                else
                {
                    LDataTableReturn = LDatabaseOperationReturn.DataSetReturn.Tables[0];
                    foreach (DataRow dr in LDataTableReturn.Rows)
                    {
                         KPIDefine kpiInfoTemp = new KPIDefine();
                         kpiInfoTemp.KpiID = LongParse(dr["C001"].ToString(), 0);
                         kpiInfoTemp.KpiName = dr["C002"].ToString();
                         kpiInfoTemp.ApplyObject = dr["C006"].ToString();
                         kpiInfoTemp.KpiType = IntParse(dr["C007"].ToString(), 0);
                         kpiInfoTemp.IsSystemContain = dr["C008"].ToString();
                         kpiInfoTemp.IsStart = dr["C009"].ToString();
                         kpiInfoTemp.ValueType = dr["C010"].ToString();
                         kpiInfoTemp.NewFormula = dr["C012"].ToString();
                         AListKPIDefineInfo.Add(kpiInfoTemp);
                    }
                }
            }
            catch (Exception ex)
            {
                LDataTableReturn = null;
                FileLog.WriteInfo("GetAllKPIDefine()", ex.Message);
            }
        }
    }
}
