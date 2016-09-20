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
    //完成对T_46_011,T_46_012,T_46_013,T_46_015,
    public class DALKPIStatistics:BasicMethod
    {
        //插入数据
        public static void InsertKpiStatisticsSliceData(DataBaseConfig ADataBaseConfig, GlobalSetting AGlobalSetting, int TabelNumber, KPIStatisticsData ADataKPIStatistics) 
        {
            DataTable LDataTableReturn = new DataTable();
            string LStrDynamicSQL = string.Empty;
            string LStrRentToken = string.Empty;
            string LStrSingleObject = string.Empty;
            string TableName = string.Empty;
            string ColumnName = string.Empty;
            TableName = string.Format("T_46_01{0}_{1}", TabelNumber, AGlobalSetting.StrRent);
            try
            {
                switch (TabelNumber)
                {
                    case 1://分钟表
                        {
                            string LC101=string.Empty;
                            string LC102=string.Empty;
                            string LC103=string.Empty;
                            string LC104=string.Empty;
                            string LC105=string.Empty;
                            string LC106=string.Empty;
                            string LC107 = string.Empty;
                            string LC109 = string.Empty;
                            if (ADataBaseConfig.IntDatabaseType == 2)
                            {
                                LC101 = string.Format("C{0}01", ADataKPIStatistics.ColumnOrder.ToString("00"));
                                LC102 = string.Format("C{0}02", ADataKPIStatistics.ColumnOrder.ToString("00"));
                                LC103 = string.Format("C{0}03", ADataKPIStatistics.ColumnOrder.ToString("00"));
                                LC104 = string.Format("C{0}04", ADataKPIStatistics.ColumnOrder.ToString("00"));
                                LC105 = string.Format("C{0}05", ADataKPIStatistics.ColumnOrder.ToString("00"));
                                LC106 = string.Format("C{0}06", ADataKPIStatistics.ColumnOrder.ToString("00"));
                                LC107 = string.Format("C{0}07", ADataKPIStatistics.ColumnOrder.ToString("00"));
                                LC109 = string.Format("C{0}09", ADataKPIStatistics.ColumnOrder.ToString("00"));

                                LStrDynamicSQL = string.Format("IF NOT EXISTS (SELECT * FROM {0} WHERE C001={1} AND C003={2} AND C007={5} AND C004={24} AND C002={25} )	INSERT INTO {0} (C001,C002,C003,C004,C005,C006,C007,C008,C009,C010,C011,C015 ,{17},{18},{19},{20},{21},{22},{23},{26} ) VALUES ({1},{25},{2},{24},{3},{4},{5},'{6}',{7},{8},{9},{28},{10},{11},'{12}',{13},{14},'{15}',{16},{27}) ELSE 	UPDATE {0} SET C008='{6}',{17}={10},{18}={11},{19}='{12}',{20}={13},{21}={14},{22}='{15}',{23}={16},{26}={27}  WHERE  C001={1} AND C003={2} AND C007={5} AND C004={24}  AND C002={25} ",
                                    TableName,//0
                                    ADataKPIStatistics.KPIMappingID,
                                    ADataKPIStatistics.ObjectID,                                    
                                    ADataKPIStatistics.SliceInOrder,//3
                                    ADataKPIStatistics.StartTimeUTC,
                                    ADataKPIStatistics.StartTimeLocal,
                                    ADataKPIStatistics.UpdateTime,//6
                                    ADataKPIStatistics.Year,
                                    ADataKPIStatistics.Month,
                                    ADataKPIStatistics.Day,
                                    ADataKPIStatistics.ActualValue, //10
                                    ADataKPIStatistics.Goal1,//11
                                    ADataKPIStatistics.CompareSign1,
                                    ADataKPIStatistics.Trend1,
                                    ADataKPIStatistics.ActualCompareGoal1,
                                    ADataKPIStatistics.Show1,
                                    ADataKPIStatistics.Goal2,//16
                                    LC101,
                                    LC102,
                                    LC103,
                                    LC104,
                                    LC105,
                                    LC106,
                                    LC107,
                                    ADataKPIStatistics.SliceType ,//24                                   
                                    ADataKPIStatistics.RowID,
                                    LC109,
                                    ADataKPIStatistics.ComparePrior,
                                    ADataKPIStatistics.KPIID
                                    );
                            }
                            else if (ADataBaseConfig.IntDatabaseType == 3)
                            {
                                LStrDynamicSQL = string.Format("BEGIN  UPDATE {0} SET C008=TO_DATE ('{6}','YYYY-MM-DD HH24:MI:SS'),{17}={10},{18}={11},{19}='{12}',{20}={13},{21}={14},{22}='{15}',{23}={16},{26}={27}  WHERE  C001={1} AND C002={2} AND C007={5} AND C004={24}  AND C002={25} ;   IF SQL%NOTFOUND THEN  INSERT INTO {0} (C001,C002,C003,C004,C005,C006,C007,C008,C009, C010,C011,C015 ,{17},{18},{19},{20},{21},{22},{23},{26} ) VALUES ({1},{25},{2},{24},{3},{4},{5},TO_DATE ('{6}','YYYY-MM-DD HH24:MI:SS'),{7},{8},{9},{28},{10},{11},'{12}',{13},{14},'{15}',{16},{27});  END IF;COMMIT; END;",
                                     TableName, //0
                                    ADataKPIStatistics.KPIMappingID,
                                    ADataKPIStatistics.ObjectID,
                                    ADataKPIStatistics.SliceInOrder,
                                    ADataKPIStatistics.StartTimeUTC,
                                    ADataKPIStatistics.StartTimeLocal,//5
                                    ADataKPIStatistics.UpdateTime,
                                    ADataKPIStatistics.Year,
                                    ADataKPIStatistics.Month,
                                    ADataKPIStatistics.Day,
                                    ADataKPIStatistics.ActualValue, //10
                                    ADataKPIStatistics.Goal1,//11
                                    ADataKPIStatistics.CompareSign1,
                                    ADataKPIStatistics.Trend1,
                                    ADataKPIStatistics.ActualCompareGoal1,
                                    ADataKPIStatistics.Show1,
                                    ADataKPIStatistics.Goal2,
                                    LC101,
                                    LC102,
                                    LC103,
                                    LC104,
                                    LC105,
                                    LC106,
                                    LC107,
                                    ADataKPIStatistics.SliceType,                                    
                                    ADataKPIStatistics.RowID,
                                    LC109,
                                    ADataKPIStatistics.ComparePrior,
                                    ADataKPIStatistics.KPIID
                                    );
                            }
                        }
                        break;
                    case 2:
                    case 3:
                    case 4:
                        {
                            if (ADataBaseConfig.IntDatabaseType == 2)
                            {
                                LStrDynamicSQL = string.Format("IF NOT EXISTS (SELECT * FROM {0} WHERE C001={1} AND C002={2} AND C005={5} )	INSERT INTO {0} (C001,C002,C003,C004,C005,C006,C007,C008,C009,C101,C102,C103,C104,C105,C106,C107 ,C109,C117) VALUES  ({1},{2},{3},{4},{5},'{6}',{7},{8},{9},{10},{11},'{12}',{13},{14},'{15}',{16},{17},{18}) ELSE 	UPDATE {0} SET C006='{6}',C101={10},C102={11},C103='{12}',C104={13},C105={14},C106='{15}',C107={16},C109={17}  WHERE  C001={1} AND C002={2} AND C005={5} ",
                                    TableName,
                                    ADataKPIStatistics.KPIMappingID,//1
                                    ADataKPIStatistics.ObjectID,
                                    ADataKPIStatistics.SliceInOrder,
                                    ADataKPIStatistics.StartTimeUTC,
                                    ADataKPIStatistics.StartTimeLocal,//5
                                    ADataKPIStatistics.UpdateTime,
                                    ADataKPIStatistics.Year,
                                    ADataKPIStatistics.Month,
                                    ADataKPIStatistics.Day,
                                    ADataKPIStatistics.ActualValue, //10
                                    ADataKPIStatistics.Goal1,//11
                                    ADataKPIStatistics.CompareSign1,
                                    ADataKPIStatistics.Trend1,
                                    ADataKPIStatistics.ActualCompareGoal1,
                                    ADataKPIStatistics.Show1,
                                    ADataKPIStatistics.Goal2,//16
                                    ADataKPIStatistics.ComparePrior,
                                    ADataKPIStatistics.KPIID
                                    );
                            }
                            else if (ADataBaseConfig.IntDatabaseType == 3)
                            {
                                LStrDynamicSQL = string.Format("BEGIN  UPDATE {0} SET C006=TO_DATE ('{6}','YYYY-MM-DD HH24:MI:SS'),C101={10},C102={11},C103='{12}',C104={13},C105={14},C106='{15}',C107={16},C109={17}  WHERE  C001={1} AND C002={2} AND C005={5} ;   IF SQL%NOTFOUND THEN  INSERT INTO {0} (C001,C002,C003,C004,C005,C006,C007,C008,C009,C101,C102,C103,C104,C105,C106,C107 ,C109,C117) VALUES    ({1},{2},{3},{4},{5},TO_DATE ('{6}','YYYY-MM-DD HH24:MI:SS'),{7},{8},{9},{10},{11},'{12}',{13},{14},'{15}',{16},{17},{18});  END IF;  COMMIT;  END;",
                                     TableName,
                                    ADataKPIStatistics.KPIMappingID,//1
                                    ADataKPIStatistics.ObjectID,
                                    ADataKPIStatistics.SliceInOrder,
                                    ADataKPIStatistics.StartTimeUTC,
                                    ADataKPIStatistics.StartTimeLocal,///5
                                    ADataKPIStatistics.UpdateTime,
                                    ADataKPIStatistics.Year,
                                    ADataKPIStatistics.Month,
                                    ADataKPIStatistics.Day,
                                    ADataKPIStatistics.ActualValue, //10
                                    ADataKPIStatistics.Goal1,//11
                                    ADataKPIStatistics.CompareSign1,
                                    ADataKPIStatistics.Trend1,
                                    ADataKPIStatistics.ActualCompareGoal1,
                                    ADataKPIStatistics.Show1,
                                    ADataKPIStatistics.Goal2,//16);
                                    ADataKPIStatistics.ComparePrior,
                                    ADataKPIStatistics.KPIID);
                            }
                        }
                        break;
                    case 5://年表
                        {
                            if (ADataBaseConfig.IntDatabaseType == 2)
                            {
                                LStrDynamicSQL = string.Format("IF NOT EXISTS (SELECT * FROM {0} WHERE C001={1} AND C002={2} AND C005={5} )	INSERT INTO {0} (C001,C002,C003,C004,C005,C006,C101,C102,C103,C104,C105,C106,C107 ,C109,C117) VALUES  ({1},{2},{3},{4},{5},'{6}',{7},{8},'{9}',{10},{11},'{12}',{13},{14},{15}) ELSE 	UPDATE {0} SET C006='{6}',C101={7},C102={8},C103='{9}',C104={10},C105={11},C106='{12}',C107={13},C109={14}  WHERE  C001={1} AND C002={2} AND C005={5} ",
                                    TableName,//0
                                    ADataKPIStatistics.KPIMappingID,
                                    ADataKPIStatistics.ObjectID,//2
                                    ADataKPIStatistics.SliceInOrder,
                                    ADataKPIStatistics.StartTimeUTC,
                                    ADataKPIStatistics.StartTimeLocal,
                                    ADataKPIStatistics.UpdateTime,//6
                                    ADataKPIStatistics.ActualValue, //7
                                    ADataKPIStatistics.Goal1,//8
                                    ADataKPIStatistics.CompareSign1,
                                    ADataKPIStatistics.Trend1,
                                    ADataKPIStatistics.ActualCompareGoal1,
                                    ADataKPIStatistics.Show1,
                                    ADataKPIStatistics.Goal2, //13
                                    ADataKPIStatistics.ComparePrior,
                                    ADataKPIStatistics.KPIID
                                    );
                            }
                            else if (ADataBaseConfig.IntDatabaseType == 3)
                            {
                                LStrDynamicSQL = string.Format("BEGIN  UPDATE {0} SET C006=TO_DATE ('{6}','YYYY-MM-DD HH24:MI:SS'),C101={7},C102={8},C103='{9}',C104={10},C105={11},C106='{12}',C107={13},C109={14}  WHERE  C001={1} AND C002={2} AND C005={5} ;   IF SQL%NOTFOUND THEN  INSERT INTO {0} (C001,C002,C003,C004,C005,C006,C101,C102,C103,C104,C105,C106,C107,C109 ,C117) VALUES ({1},{2},{3},{4},{5},TO_DATE ('{6}','YYYY-MM-DD HH24:MI:SS'),{7},{8},'{9}',{10},{11},'{12}',{13},{14},{15});  END IF; COMMIT; END;",
                                    TableName,//0
                                    ADataKPIStatistics.KPIMappingID,
                                    ADataKPIStatistics.ObjectID,//2
                                    ADataKPIStatistics.SliceInOrder,
                                    ADataKPIStatistics.StartTimeUTC,
                                    ADataKPIStatistics.StartTimeLocal,//5
                                    ADataKPIStatistics.UpdateTime,//6
                                    ADataKPIStatistics.ActualValue, //7
                                    ADataKPIStatistics.Goal1,
                                    ADataKPIStatistics.CompareSign1,
                                    ADataKPIStatistics.Trend1,//10
                                    ADataKPIStatistics.ActualCompareGoal1,
                                    ADataKPIStatistics.Show1,
                                    ADataKPIStatistics.Goal2,//13
                                    ADataKPIStatistics.ComparePrior,
                                    ADataKPIStatistics.KPIID);
                            }
                        }
                        break;
                    default:
                        break;
                }

                FileLog.WriteInfo("InsertKpiStatisticsSliceData()", LStrDynamicSQL);
                DatabaseOperation01Return LDatabaseOperationReturn = new DatabaseOperation01Return();
                DataOperations01 LDataOperations = new DataOperations01();
                LDatabaseOperationReturn = LDataOperations.ExecuteDynamicSQL(ADataBaseConfig.IntDatabaseType, ADataBaseConfig.StrDatabaseProfile, LStrDynamicSQL);
                if (!LDatabaseOperationReturn.BoolReturn)
                {
                    FileLog.WriteInfo("InsertKpiStatisticsSliceData()", LStrDynamicSQL + "Error");
                }

            }           
            catch (Exception ex)
            {

                FileLog.WriteInfo("InsertKpiStatisticsSliceData()", "Error :" + ex.Message);
            }
        }

        
        //查询数据
        public static KPIStatisticsData SelectKPIStatisticsData(DataBaseConfig ADataBaseConfig, GlobalSetting AGlobalSetting, int ATabelNumber,long AKPIMapingID,long AStartTimeLocal,long AObjectID,int ARowID=-1,int ASliceType=-1  ,int AColumnNumber=-1) 
        {
            KPIStatisticsData dataKPIStatisticsTemp = new KPIStatisticsData();
             DataTable LDataTableReturn = new DataTable();
            string LStrDynamicSQL = string.Empty;
            string LStrRentToken = string.Empty;
            string LStrSingleObject = string.Empty;
            string TableName = string.Empty;
            string ColumnName = string.Empty;
            string ColumnNameTrend = string.Empty;
            string ColumnNameCompareProior = string.Empty;
            TableName = string.Format("T_46_01{0}_{1}", ATabelNumber,AGlobalSetting.StrRent);
            try
            {
                DatabaseOperation01Return LDatabaseOperationReturn = new DatabaseOperation01Return();
                DataOperations01 LDataOperations = new DataOperations01();
               
                switch (ATabelNumber)
                {
                    case 1:
                        {
                            ColumnName = string.Format("C{0}01",AColumnNumber.ToString("00"));
                            ColumnNameTrend = string.Format("C{0}04", AColumnNumber.ToString("00"));
                            ColumnNameCompareProior = string.Format("C{0}09", AColumnNumber.ToString("00"));
                            LStrDynamicSQL = string.Format("SELECT * FROM {0} WHERE C001={1} AND C002={4} AND C003={2} AND C004={5} AND C007={3} "
                           , TableName,
                           AKPIMapingID,//1
                           AObjectID,
                           AStartTimeLocal,
                           ARowID,//4
                           ASliceType);
                        }
                        break;
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                        {
                          
                            LStrDynamicSQL = string.Format("SELECT * FROM {0} WHERE C001={1} AND C002={2} AND C005={3} "
                           , TableName,
                           AKPIMapingID,
                           AObjectID,
                           AStartTimeLocal);
                        }
                        break;
                }

                LDatabaseOperationReturn = LDataOperations.SelectDataByDynamicSQL(ADataBaseConfig.IntDatabaseType, ADataBaseConfig.StrDatabaseProfile, LStrDynamicSQL);
                if (!LDatabaseOperationReturn.BoolReturn)
                {
                    LDataTableReturn = null;
                }
                else
                {
                    LDataTableReturn = LDatabaseOperationReturn.DataSetReturn.Tables[0];
                    if (LDataTableReturn.Rows.Count==0)
                    {
                        return null;
                    }
                    foreach (DataRow dr in LDataTableReturn.Rows)
                    {
                        dataKPIStatisticsTemp.KPIMappingID = AKPIMapingID;
                        dataKPIStatisticsTemp.ObjectID = AObjectID;
                        dataKPIStatisticsTemp.StartTimeLocal = AStartTimeLocal;                        
                        if (ATabelNumber == 1)
                        {
                            dataKPIStatisticsTemp.ActualValue = DecimalParse(dr[ColumnName].ToString(), 0);
                            dataKPIStatisticsTemp.Trend1 = DecimalParse(dr[ColumnNameTrend].ToString(), 0);
                            dataKPIStatisticsTemp.ComparePrior = DecimalParse(dr[ColumnNameCompareProior].ToString(), 0);
                        } 
                        else 
                        {
                             dataKPIStatisticsTemp.ActualValue = DecimalParse(dr["C101"].ToString(), 0); 
                             dataKPIStatisticsTemp.Trend1 = DecimalParse(dr["C104"].ToString(), 0);
                             dataKPIStatisticsTemp.ComparePrior = DecimalParse(dr["C109"].ToString(), 0);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LDataTableReturn = null;
                FileLog.WriteInfo("SelectKPIStatisticsData()", ex.Message);
            }    
            return dataKPIStatisticsTemp;
        }
    }
}
