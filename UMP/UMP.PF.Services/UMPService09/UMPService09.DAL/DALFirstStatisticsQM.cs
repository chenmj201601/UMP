using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UMPService09.Log;
using System.Text.RegularExpressions;
using UMPService09.Model;
using UMPService09.Utility;
using PFShareClassesS;
using System.Data;

namespace UMPService09.DAL
{
    /// <summary>
    /// 往T_46_021,T_46_022,T_46_023,T_46_024,T_46_026表写QM数据
    /// </summary>
    public class DALFirstStatisticsQM : BasicMethod
    {
        /// <summary>
        /// 将数据插入QM的统计表
        /// </summary>
        /// <param name="ADataBaseConfig"></param>
        /// <param name="AGlobalSetting"></param>
        /// <param name="TabelNumber"> 表示T_46_022,T_46_023,T_46_024,T_46_025</param>
        /// <param name="ADataFirstStatisticsSlice"></param>
        public static void InsertQMStatistics(DataBaseConfig ADataBaseConfig, GlobalSetting AGlobalSetting, int TabelNumber, DataFirstStatisticsSlice ADataFirstStatisticsSlice)
        {
            DataTable LDataTableReturn = new DataTable();
            string LStrDynamicSQL = string.Empty;
            string LStrRentToken = string.Empty;
            string LStrSingleObject = string.Empty;
            string TableName = string.Empty;
            string ColumnName = string.Empty;
            int LTempOrder = 0;//天在当年的天数，天所在的周属于当年的第几周...


            switch (TabelNumber)
            {
                case 2:
                    {
                        LTempOrder = GetDayInYear(new DateTime(ADataFirstStatisticsSlice.Year, ADataFirstStatisticsSlice.Month, ADataFirstStatisticsSlice.Day));
                        TableName = "T_46_022";
                        ColumnName = "C10" + ADataFirstStatisticsSlice.OrderID;
                    }
                    break;
                case 3:
                    {
                        TableName = "T_46_023";
                        ColumnName = "C10" + ADataFirstStatisticsSlice.OrderID;
                    }
                    break;
                case 4:
                    {
                        LTempOrder = ADataFirstStatisticsSlice.Month;
                        TableName = "T_46_024";
                        ColumnName = "C10" + ADataFirstStatisticsSlice.OrderID;
                    }
                    break;
                case 5:
                    {
                        LTempOrder = ADataFirstStatisticsSlice.Year;
                        TableName = "T_46_025";
                        ColumnName = "C10" + ADataFirstStatisticsSlice.OrderID;
                    }
                    break;
                default:
                    break;
            }

            try
            {

                if (ADataBaseConfig.IntDatabaseType == 2)
                {
                    LStrDynamicSQL = string.Format("IF NOT EXISTS (SELECT * FROM {0} WHERE C001={1} AND C002='{2}' AND C003={3} AND C004={4} AND C006={6}) INSERT INTO {0} (C001,C002,C003,C004,C005,C006,C007,C008,C009,C010,C011,{12}) VALUES ({1},'{2}',{3},{4},{5},{6},'{7}',{8},{9},{10},1000,{11}) ELSE UPDATE {0} SET {12}={11},C011=1000 WHERE C001={1} AND C002='{2}' AND C003={3} AND C004={4} AND C006={6} ",
                        TableName,
        ADataFirstStatisticsSlice.ObjectType,
        ADataFirstStatisticsSlice.StrRent,
        ADataFirstStatisticsSlice.ObjectID,
        LTempOrder,
        ADataFirstStatisticsSlice.StartTimeUTC,
        ADataFirstStatisticsSlice.StartTimeLocal,
        ADataFirstStatisticsSlice.UpdateTime,
        ADataFirstStatisticsSlice.Year,
        ADataFirstStatisticsSlice.Month,
        ADataFirstStatisticsSlice.Day,
        ADataFirstStatisticsSlice.Value01,
        ColumnName);
                }
                else
                {
                    LStrDynamicSQL = string.Format("BEGIN UPDATE {0} SET {12}={11},C011=1000 WHERE  C001={1} AND C002='{2}' AND C003={3} AND C004={4} AND C006={6};IF SQL%NOTFOUND THEN INSERT INTO {0}(C001,C002,C003,C004,C005,C006,C007,C008,C009,C010,C011,{12}) VALUES ({1},'{2}',{3},{4},{5},{6},TO_DATE('{7}','YYYY-MM-DD HH24:MI:SS'),{8},{9},{10},1000,{11}); END IF; COMMIT; END;",
                        TableName,
       ADataFirstStatisticsSlice.ObjectType,
       ADataFirstStatisticsSlice.StrRent,
       ADataFirstStatisticsSlice.ObjectID,
       LTempOrder,
       ADataFirstStatisticsSlice.StartTimeUTC,
       ADataFirstStatisticsSlice.StartTimeLocal,
       ADataFirstStatisticsSlice.UpdateTime,
       ADataFirstStatisticsSlice.Year,
       ADataFirstStatisticsSlice.Month,
       ADataFirstStatisticsSlice.Day,
       ADataFirstStatisticsSlice.Value01,
       ColumnName);
                }
                FileLog.WriteInfo("InsertQMStatistics()", LStrDynamicSQL);
                DatabaseOperation01Return LDatabaseOperationReturn = new DatabaseOperation01Return();
                DataOperations01 LDataOperations = new DataOperations01();
                LDatabaseOperationReturn = LDataOperations.ExecuteDynamicSQL(ADataBaseConfig.IntDatabaseType, ADataBaseConfig.StrDatabaseProfile, LStrDynamicSQL);
                if (!LDatabaseOperationReturn.BoolReturn)
                {
                    FileLog.WriteInfo("InsertQMStatistics()", LStrDynamicSQL + "Error");
                }
            }
            catch (Exception ex)
            {

                FileLog.WriteInfo("InsertQMStatistics()", "Error :" + ex.Message);
            }
        }

        /// <summary>
        /// 得到录音表周的数据
        /// </summary>
        /// <param name="ADataBaseConfig"></param>
        /// <param name="AObjectInfo"></param>
        /// <param name="AGlobalSetting"></param>
        /// <param name="StartTimeLocal"></param>
        /// <param name="StopTimeLocal"></param>
        /// <param name="AFuncType"></param>
        /// <param name="AExtensionAgentType"></param>
        /// <returns></returns>
        public static long GetWeekQMStatistics(DataBaseConfig ADataBaseConfig, ObjectInfo AObjectInfo, GlobalSetting AGlobalSetting, long StartTimeLocal, long StopTimeLocal, int AFuncType, int AExtensionAgentType)
        {
            long Value01 = 0;
            DataTable LDataTableReturn = new DataTable();
            string LStrDynamicSQL = string.Empty;
            string LStrRentToken = string.Empty;
            string LStrSingleObject = string.Empty;
            string LColumnName = string.Format("C10{0}", AFuncType);
            try
            {
                LStrDynamicSQL = string.Format("SELECT SUM({0}) AS VALUE01 FROM T_46_022 WHERE C001={1} AND C002={2} AND C003={3} AND C006>={5} AND C006<{6} ",
                      LColumnName,
                      AExtensionAgentType,
                       AGlobalSetting.StrRent,
                       AObjectInfo.ObjID,
                       AFuncType,
                       StartTimeLocal,
                       StopTimeLocal);

                FileLog.WriteInfo("GetWeekQMStatistics()", LStrDynamicSQL);
                DatabaseOperation01Return LDatabaseOperationReturn = new DatabaseOperation01Return();
                DataOperations01 LDataOperations = new DataOperations01();
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
                        Value01 += LongParse(LDataRowSingleRow["VALUE01"].ToString(), 0);
                    }
                }
            }
            catch (Exception ex)
            {
                FileLog.WriteInfo("GetWeekQMStatistics()", "Error :" + ex.Message);
            }
            return Value01;
        }

        /// <summary>
        /// 得到录音表月的数据
        /// </summary>
        /// <param name="ADataBaseConfig"></param>
        /// <param name="AObjectInfo"></param>
        /// <param name="AGlobalSetting"></param>
        /// <param name="StartTimeLocal"></param>
        /// <param name="StopTimeLocal"></param>
        /// <param name="AFuncType"></param>
        /// <param name="AExtensionAgentType"></param>
        /// <returns></returns>
        public static long GetMonthQMStatistics(DataBaseConfig ADataBaseConfig, ObjectInfo AObjectInfo, GlobalSetting AGlobalSetting, long StartTimeLocal, long StopTimeLocal, int AFuncType, int AExtensionAgentType)
        {
            long Value01 = 0;
            DataTable LDataTableReturn = new DataTable();
            string LStrDynamicSQL = string.Empty;
            string LStrRentToken = string.Empty;
            string LStrSingleObject = string.Empty;
            string LColumnName = string.Format("C10{0}", AFuncType);
            try
            {
                LStrDynamicSQL = string.Format("SELECT SUM({0}) AS VALUE01  FROM T_46_022 WHERE C001={1} AND C002={2} AND C003={3} AND C006>={5} AND C006<{6} ",
                      LColumnName,
                      AExtensionAgentType,
                       AGlobalSetting.StrRent,
                       AObjectInfo.ObjID,
                       AFuncType,
                       StartTimeLocal,
                       StopTimeLocal);

                FileLog.WriteInfo("GetMonthQMStatistics()", LStrDynamicSQL);
                DatabaseOperation01Return LDatabaseOperationReturn = new DatabaseOperation01Return();
                DataOperations01 LDataOperations = new DataOperations01();
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
                        Value01 += LongParse(LDataRowSingleRow["VALUE01"].ToString(), 0);
                    }
                }
            }
            catch (Exception ex)
            {
                FileLog.WriteInfo("GetMonthQMStatistics()", "Error :" + ex.Message);
            }
            return Value01;
        }

        /// <summary>
        /// 得到录音表年的数据
        /// </summary>
        /// <param name="ADataBaseConfig"></param>
        /// <param name="AObjectInfo"></param>
        /// <param name="AGlobalSetting"></param>
        /// <param name="StartTimeLocal"></param>
        /// <param name="StopTimeLocal"></param>
        /// <param name="AFuncType"></param>
        /// <param name="AExtensionAgentType"></param>
        /// <returns></returns>
        public static long GetYearQMStatistics(DataBaseConfig ADataBaseConfig, ObjectInfo AObjectInfo, GlobalSetting AGlobalSetting, long StartTimeLocal, long StopTimeLocal, int AFuncType, int AExtensionAgentType)
        {
            long Value01 = 0;
            DataTable LDataTableReturn = new DataTable();
            string LStrDynamicSQL = string.Empty;
            string LStrRentToken = string.Empty;
            string LStrSingleObject = string.Empty;
            string LColumnName = string.Format("C10{0}", AFuncType);
            try
            {
                LStrDynamicSQL = string.Format("SELECT SUM({0}) AS VALUE01 FROM T_46_024 WHERE C001={1} AND C002={2} AND C003={3} AND C006>={5} AND C006<{6} ",
                      LColumnName,
                      AExtensionAgentType,
                      AGlobalSetting.StrRent,
                      AObjectInfo.ObjID,
                      AFuncType,
                      StartTimeLocal,
                      StopTimeLocal);

                FileLog.WriteInfo("GetYearQMStatistics()", LStrDynamicSQL);
                DatabaseOperation01Return LDatabaseOperationReturn = new DatabaseOperation01Return();
                DataOperations01 LDataOperations = new DataOperations01();
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
                        Value01 += LongParse(LDataRowSingleRow["VALUE01"].ToString(), 0);
                    }
                }
            }
            catch (Exception ex)
            {
                FileLog.WriteInfo("GetYearQMStatistics()", "Error :" + ex.Message);
            }
            return Value01;
        }

    }
}
