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
    //录音的初次统计表 T_46_031,T_46_032,T_46_033,T_46_035这些表写,读数据数据
    public  class DALFirstStatisticsRecord : BasicMethod
    {
        //将数据插入录音的统计表
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ADataBaseConfig"></param>
        /// <param name="AGlobalSetting"></param>
        /// <param name="TabelNumber"> 表示T_46_031,T_46_032,T_46_033,T_46_034,T_46_035</param>
        /// <param name="ADataFirstStatisticsSlice"></param>
        public static void InsertRecordStatistics(DataBaseConfig ADataBaseConfig,GlobalSetting AGlobalSetting,int TabelNumber,DataFirstStatisticsSlice ADataFirstStatisticsSlice)
        {
            DataTable LDataTableReturn = new DataTable();
            string LStrDynamicSQL = string.Empty;
            string LStrRentToken = string.Empty;
            string LStrSingleObject = string.Empty;
            string TableName = string.Empty;
            string ColumnName = string.Empty;
            TableName = string.Format("T_46_03{0}",TabelNumber);
            try
            {

                switch (TabelNumber)
                {
                    case 1:
                        {
                            ColumnName = GetColumnName(ADataFirstStatisticsSlice.OrderID);
                        }
                        break;
                    case 2:                        
                    case 3:
                    case 4:
                    case 5:
                        {

                            ColumnName = "C1" + Convert.ToInt16(ADataFirstStatisticsSlice.RecordFunction).ToString("00");
                        }
                        break;
                    default:
                        break;
                }


                ///在31表是C004为 数据类型   其它是C004是一年里的第几天
                ///
                if (TabelNumber == 1)
                {
                    if (ADataBaseConfig.IntDatabaseType == 2)
                    {
                        LStrDynamicSQL = string.Format(" IF NOT EXISTS (SELECT * FROM {0} WHERE C001={1} AND C002='{2}' AND C003={3} AND C004={4} AND C006={6} )	INSERT INTO {0} (C001,C002,C003,C004,C005,C006,C007,C008,C009,C010,C011,{12}) VALUES ({1},'{2}',{3},{4},{5},{6},'{7}',{8},{9},{10},1000,{11} ) ELSE 	UPDATE {0} SET {12}={11} WHERE  C001={1} AND C002='{2}' AND C003={3} AND C004={4} AND C006={6} ",
                            TableName,//0
            ADataFirstStatisticsSlice.ObjectType,//1
            ADataFirstStatisticsSlice.StrRent,
            ADataFirstStatisticsSlice.ObjectID,
            Convert.ToInt16( ADataFirstStatisticsSlice.RecordFunction),
            ADataFirstStatisticsSlice.StartTimeUTC,
            ADataFirstStatisticsSlice.StartTimeLocal,
            ADataFirstStatisticsSlice.UpdateTime,
            ADataFirstStatisticsSlice.Year,
            ADataFirstStatisticsSlice.Month,
            ADataFirstStatisticsSlice.Day,
            ADataFirstStatisticsSlice.Value01,//11
            ColumnName
            );
                    }
                    else
                    {
                        LStrDynamicSQL = string.Format("BEGIN  UPDATE {0} SET {12}={11} WHERE  C001={1} AND C002='{2}' AND C003={3} AND C004={4} AND C006={6} ;   IF SQL%NOTFOUND THEN   INSERT INTO {0} (C001,C002,C003,C004,C005,C006,C007,C008,C009,C010,C011,{12} ) VALUES ({1},'{2}',{3},{4},{5},{6},TO_DATE ('{7}','YYYY-MM-DD HH24:MI:SS'),{8},{9},{10},1000,{11});  END IF; COMMIT; END;",
                            TableName,
           ADataFirstStatisticsSlice.ObjectType,
           ADataFirstStatisticsSlice.StrRent,
           ADataFirstStatisticsSlice.ObjectID,
           Convert.ToInt16( ADataFirstStatisticsSlice.RecordFunction),
           ADataFirstStatisticsSlice.StartTimeUTC,
           ADataFirstStatisticsSlice.StartTimeLocal,
           ADataFirstStatisticsSlice.UpdateTime,
           ADataFirstStatisticsSlice.Year,
           ADataFirstStatisticsSlice.Month,
           ADataFirstStatisticsSlice.Day,
           ADataFirstStatisticsSlice.Value01,
           ColumnName);
                    }
                }
                else 
                {
                    if (ADataBaseConfig.IntDatabaseType == 2)
                    {
                        LStrDynamicSQL = string.Format(" IF NOT EXISTS (SELECT * FROM {0} WHERE C001={1} AND C002='{2}' AND C003={3} AND C004={4} AND C006={6} )	INSERT INTO {0} (C001,C002,C003,C004,C005,C006,C007,C008,C009,C010,C011,{12} ) VALUES ({1},'{2}',{3},{4},{5},{6},'{7}',{8},{9},{10},1000,{11}) ELSE 	UPDATE {0} SET {12}={11} WHERE  C001={1} AND C002='{2}' AND C003={3} AND C004={4} AND C006={6} ",
                            TableName,
            ADataFirstStatisticsSlice.ObjectType,//1
            ADataFirstStatisticsSlice.StrRent,
            ADataFirstStatisticsSlice.ObjectID,
            0,//一年里这周，这个月的顺序
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
                        LStrDynamicSQL = string.Format("BEGIN  UPDATE {0} SET {12}={11} WHERE  C001={1} AND C002='{2}' AND C003={3} AND C004={4} AND C006={6} ;   IF SQL%NOTFOUND THEN   INSERT INTO {0} (C001,C002,C003,C004,C005,C006,C007,C008,C009,C010,C011,{12} ) VALUES ({1},'{2}',{3},{4},{5},{6},TO_DATE ('{7}','YYYY-MM-DD HH24:MI:SS'),{8},{9},{10},1000,{11})  END IF;COMMIT;  END;",
                            TableName,
           ADataFirstStatisticsSlice.ObjectType,
           ADataFirstStatisticsSlice.StrRent,
           ADataFirstStatisticsSlice.ObjectID,
           0,
           ADataFirstStatisticsSlice.StartTimeUTC,
           ADataFirstStatisticsSlice.StartTimeLocal,
           ADataFirstStatisticsSlice.UpdateTime,
           ADataFirstStatisticsSlice.Year,
           ADataFirstStatisticsSlice.Month,
           ADataFirstStatisticsSlice.Day,
           ADataFirstStatisticsSlice.Value01,
           ColumnName);
                    } 
                }
                
                FileLog.WriteInfo("InsertRecordStatistics()", LStrDynamicSQL);
                DatabaseOperation01Return LDatabaseOperationReturn = new DatabaseOperation01Return();
                DataOperations01 LDataOperations = new DataOperations01();
                LDatabaseOperationReturn = LDataOperations.ExecuteDynamicSQL(ADataBaseConfig.IntDatabaseType,ADataBaseConfig.StrDatabaseProfile, LStrDynamicSQL);
                if (!LDatabaseOperationReturn.BoolReturn)
                {
                    FileLog.WriteInfo("InsertRecordStatistics()", LStrDynamicSQL + "Error");
                }

            }
            catch (Exception ex)
            {

                FileLog.WriteInfo("InsertRecordStatistics()", "Error :" + ex.Message);
            }
        }

        //得到录音统计表天的数据
        public static long GetDayRecordStatistics(DataBaseConfig ADataBaseConfig,ObjectInfo AObjectInfo, GlobalSetting AGlobalSetting, long StartTimeLocal, long StopTimeLocal, int AFuncType, int AExtensionAgentType)
        {
            long Value01 = 0;
            DataTable LDataTableReturn = new DataTable();
            string LStrDynamicSQL = string.Empty;
            string LStrRentToken = string.Empty;
            string LStrSingleObject = string.Empty;
            try
            {
                LStrDynamicSQL = string.Format("SELECT ISNULL(SUM({0}),0) AS Value01  FROM T_46_031 WHERE C001={1} AND C002={2} AND C003={3} AND   C004={4} AND C006>={5} AND C006<{6} ",
                       GetAllColumnName(),
                      AExtensionAgentType,
                       AGlobalSetting.StrRent,
                       AObjectInfo.ObjID,
                       AFuncType,
                       StartTimeLocal,
                       StopTimeLocal);

                FileLog.WriteInfo("GetDayRecordStatistics()", LStrDynamicSQL);
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
                        Value01 += LongParse(LDataRowSingleRow["Value01"].ToString(), 0);
                    }
                }
            }
            catch (Exception ex)
            {
                FileLog.WriteInfo("GetDayRecordStatistics()", "Error :" + ex.Message);
            }
            return Value01;
        }

        //得到录音表月的数据
        public static long GetWeekRecordStatistics(DataBaseConfig ADataBaseConfig, ObjectInfo AObjectInfo, GlobalSetting AGlobalSetting, long StartTimeLocal, long StopTimeLocal, int AFuncType, int AExtensionAgentType)
        {
            long Value01 = 0;
            DataTable LDataTableReturn = new DataTable();
            string LStrDynamicSQL = string.Empty;
            string LStrRentToken = string.Empty;
            string LStrSingleObject = string.Empty;
            string LColumnName = string.Format("C10{0}", AFuncType);
            try
            {
                LStrDynamicSQL = string.Format("SELECT ISNULL(SUM({0}),0) AS Value01  FROM T_46_032 WHERE C001={1} AND C002={2} AND C003={3} AND    C006>={4} AND C006<{5} ",
                      LColumnName,
                      AExtensionAgentType,
                       AGlobalSetting.StrRent,
                       AObjectInfo.ObjID,
                       StartTimeLocal,
                       StopTimeLocal);

                FileLog.WriteInfo("GetDayRecordStatistics()", LStrDynamicSQL);
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
                        Value01 += LongParse(LDataRowSingleRow["Value01"].ToString(), 0);
                    }
                }
            }
            catch (Exception ex)
            {
                FileLog.WriteInfo("GetDayRecordStatistics()", "Error :" + ex.Message);
            }
            return Value01;
        }

        //得到录音表周的数据
        public static long GetMonthRecordStatistics(DataBaseConfig ADataBaseConfig, ObjectInfo AObjectInfo, GlobalSetting AGlobalSetting, long StartTimeLocal, long StopTimeLocal, int AFuncType, int AExtensionAgentType)
        {
            long Value01 = 0;
            DataTable LDataTableReturn = new DataTable();
            string LStrDynamicSQL = string.Empty;
            string LStrRentToken = string.Empty;
            string LStrSingleObject = string.Empty;
            string LColumnName = string.Format("C10{0}", AFuncType);
            try
            {
                LStrDynamicSQL = string.Format("SELECT ISNULL(SUM({0}),0) AS Value01  FROM T_46_032 WHERE C001={1} AND C002={2} AND C003={3} AND    C006>={4} AND C006<{5} ",
                      LColumnName,
                      AExtensionAgentType,
                       AGlobalSetting.StrRent,
                       AObjectInfo.ObjID,
                       StartTimeLocal,
                       StopTimeLocal);

                FileLog.WriteInfo("GetDayRecordStatistics()", LStrDynamicSQL);
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
                        Value01 += LongParse(LDataRowSingleRow["Value01"].ToString(), 0);
                    }
                }
            }
            catch (Exception ex)
            {
                FileLog.WriteInfo("GetDayRecordStatistics()", "Error :" + ex.Message);
            }
            return Value01;
        }

        //得到录音表年的数据
        public static long GetYearRecordStatistics(DataBaseConfig ADataBaseConfig, ObjectInfo AObjectInfo, GlobalSetting AGlobalSetting, long StartTimeLocal, long StopTimeLocal, int AFuncType, int AExtensionAgentType)
        {
            long Value01 = 0;
            DataTable LDataTableReturn = new DataTable();
            string LStrDynamicSQL = string.Empty;
            string LStrRentToken = string.Empty;
            string LStrSingleObject = string.Empty;
            string LColumnName = string.Format("C10{0}", AFuncType);
            try
            {
                LStrDynamicSQL = string.Format("SELECT ISNULL(SUM({0}),0) AS Value01  FROM T_46_034 WHERE C001={1} AND C002={2} AND C003={3}   AND C006>={4} AND C006<{5} ",
                      LColumnName,
                      AExtensionAgentType,
                       AGlobalSetting.StrRent,
                       AObjectInfo.ObjID,
                       StartTimeLocal,
                       StopTimeLocal);

                FileLog.WriteInfo("GetDayRecordStatistics()", LStrDynamicSQL);
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
                        Value01 += LongParse(LDataRowSingleRow["Value01"].ToString(), 0);
                    }
                }
            }
            catch (Exception ex)
            {
                FileLog.WriteInfo("GetDayRecordStatistics()", "Error :" + ex.Message);
            }
            return Value01;
        }
    }
}
