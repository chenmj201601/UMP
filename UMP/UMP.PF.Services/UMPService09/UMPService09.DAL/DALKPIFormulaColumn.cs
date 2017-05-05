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
    public class DALKPIFormulaColumn:BasicMethod
    {

        //T_46_004
        public static void GetAllKPIFormulaColumn(DataBaseConfig ADataBaseConfig, GlobalSetting AGolbalSetting, ref List<KPIFormulaColumn> AListKPIFormulaColumn)
        {
            AListKPIFormulaColumn.Clear();
            DataTable LDataTableReturn = new DataTable();
            string LStrDynamicSQL = string.Empty;
            string LStrRentToken = string.Empty;
            string LStrSingleObject = string.Empty;

            try
            {
                DatabaseOperation01Return LDatabaseOperationReturn = new DatabaseOperation01Return();
                DataOperations01 LDataOperations = new DataOperations01();

                LStrDynamicSQL = string.Format("SELECT * FROM T_46_004_{0} "
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
                        KPIFormulaColumn kpiformulatemp = new KPIFormulaColumn();
                        kpiformulatemp.FormulaCharID = LongParse(dr["C001"].ToString(), 0);
                        kpiformulatemp.ColumnName = dr["C002"].ToString();
                        kpiformulatemp.ColumnSource = IntParse(dr["C004"].ToString(), 0);
                        kpiformulatemp.ApplayName = dr["C005"].ToString();
                        kpiformulatemp.DataType = IntParse(dr["C006"].ToString(), 0);
                        kpiformulatemp.ApplyCycle = dr["C007"].ToString();
                        kpiformulatemp.SpecialObjectTypeNumber = IntParse(dr["C008"].ToString(), 0);
                        AListKPIFormulaColumn.Add(kpiformulatemp);
                    }
                }
            }
            catch (Exception ex)
            {
                LDataTableReturn = null;
                FileLog.WriteInfo("GetAllKPIFormulaColumn()", ex.Message);
            }
        }
    }
}
