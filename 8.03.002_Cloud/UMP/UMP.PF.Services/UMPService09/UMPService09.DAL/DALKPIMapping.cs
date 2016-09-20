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
    //读取KPIMapping 
    public    class DALKPIMapping:BasicMethod
    {
        /// <summary>
        /// 得到所有的用户信息
        /// </summary>
        /// <param name="AListUserInfo"></param>
        /// <param name="AStrRent"></param>
        public static void GetAllKPIMapping(DataBaseConfig ADataBaseConfig, GlobalSetting AGolbalSetting, ref List<KPIMapping> AListKPIDMapping)
        {
            AListKPIDMapping.Clear();
            DataTable LDataTableReturn = new DataTable();
            string LStrDynamicSQL = string.Empty;
            string LStrRentToken = string.Empty;
            string LStrSingleObject = string.Empty;

            try
            {
                DatabaseOperation01Return LDatabaseOperationReturn = new DatabaseOperation01Return();
                DataOperations01 LDataOperations = new DataOperations01();

                LStrDynamicSQL = string.Format("SELECT * FROM T_46_003_{0} WHERE C006='1' AND C011='0' "
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
                        KPIMapping kpimappingtemp = new KPIMapping();
                        kpimappingtemp.KPIMappingID = LongParse(dr["C001"].ToString(), 0);
                        kpimappingtemp.KpiID = LongParse(dr["C002"].ToString(), 0);
                        kpimappingtemp.ObjectID = LongParse(dr["C003"].ToString(), 0);
                        kpimappingtemp.ActualApplyObjType = dr["C004"].ToString();
                        kpimappingtemp.ActualApplyCycle = dr["C005"].ToString();
                        kpimappingtemp.IsStart = dr["C006"].ToString();
                        kpimappingtemp.StatisticsStartTime = LongParse(dr["C007"].ToString(), 0);
                        kpimappingtemp.StatisticsStopTime = LongParse(dr["C008"].ToString(), 0);
                        kpimappingtemp.IsDrop = IntParse(dr["C009"].ToString(), 0);
                        kpimappingtemp.IsApplyAll = dr["C010"].ToString();
                        kpimappingtemp.IsDelete = IntParse(dr["C011"].ToString(), 0);


                        kpimappingtemp.IsStartGoal1 = dr["C014"].ToString();
                        kpimappingtemp.Goal1 = DecimalParse(dr["C015"].ToString(), 0);
                        kpimappingtemp.CompareSign1 = dr["C016"].ToString();

                        kpimappingtemp.IsStartGoal2 = dr["C018"].ToString();
                        kpimappingtemp.Goal2 = DecimalParse(dr["C019"].ToString(), 0);
                        kpimappingtemp.CompareSign2 = dr["C020"].ToString();


                        AListKPIDMapping.Add(kpimappingtemp);



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
