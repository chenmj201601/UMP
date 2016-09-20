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
    public  class DALKPIFormulaChar:BasicMethod
    {
       
            /// <summary>
            /// 得到所有公式对应的字符
            /// </summary>
            /// <param name="AListUserInfo"></param>
            /// <param name="AStrRent"></param>
            public static void GetAllKPIFormulaChar(DataBaseConfig ADataBaseConfig, GlobalSetting AGolbalSetting, ref List<KPIFormulaChar> AListKPIFormulaChar)
            {
                AListKPIFormulaChar.Clear();
                DataTable LDataTableReturn = new DataTable();
                string LStrDynamicSQL = string.Empty;
                string LStrRentToken = string.Empty;
                string LStrSingleObject = string.Empty;

                try
                {
                    DatabaseOperation01Return LDatabaseOperationReturn = new DatabaseOperation01Return();
                    DataOperations01 LDataOperations = new DataOperations01();

                    LStrDynamicSQL = string.Format("SELECT * FROM T_46_005_{0} "
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
                            KPIFormulaChar kpiFormulaChar = new KPIFormulaChar();
                            kpiFormulaChar.KpiID = LongParse(dr["C001"].ToString(), 0);
                            kpiFormulaChar.FormulaCharID = LongParse(dr["C002"].ToString(), 0);
                            kpiFormulaChar.MappingChar = dr["C003"].ToString();
                            AListKPIFormulaChar.Add(kpiFormulaChar);
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
