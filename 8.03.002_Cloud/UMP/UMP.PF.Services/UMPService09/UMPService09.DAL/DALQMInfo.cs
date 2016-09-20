using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using UMPService09.Log;
using UMPService09.Model;
using UMPService09.Utility;

namespace UMPService09.DAL
{
    public class DALQMInfo : BasicMethod
    {
        public static long GetQMDayStatisticsInfo(DataBaseConfig ADataBaseConfig, ObjectInfo AObjectInfo, GlobalSetting AGlobalSetting, 
            DateTime ADateTimeStart, DateTime ADateTimeStop, int AFuncType, int AExtensionAgentType)
        {
            long Value01 = 0;
            DataTable LDataTableReturn = new DataTable();
            string LStrDynamicSQL = string.Empty;
            string LStrRentToken = string.Empty;
            string LStrSingleObject = string.Empty;

            //根据座席和分机和真实分机查询相应  功能的值
            string TableName21 = string.Empty;
            string TableName38 = string.Format("T_31_008_{0}", AGlobalSetting.StrRent);
            string TableName319 = string.Format("T_31_019_{0}", AGlobalSetting.StrRent);
            string TableName320 = string.Format("T_31_020_{0}", AGlobalSetting.StrRent);
            string TableName321 = string.Format("T_31_021_{0}", AGlobalSetting.StrRent);
            DateTime utcTimeStart = ADateTimeStart.ToUniversalTime();
            DateTime utcTimeEnd = ADateTimeStop.ToUniversalTime();
            if (AGlobalSetting.IlogicPartMark == 1)//为1按月分表
            {
                TableName21 = string.Format("T_21_001_{0}_{1}{2}", AGlobalSetting.StrRent, utcTimeStart.ToString("yy"), utcTimeStart.ToString("MM"));
                if (!AGlobalSetting.LStrRecordName.Contains(TableName21))
                {
                    return 0;
                }
            }
            else
            {
                TableName21 = string.Format("T_21_001_{0}", AGlobalSetting.StrRent);
            }

            try {
                DatabaseOperation01Return LDatabaseOperationReturn = new DatabaseOperation01Return();
                DataOperations01 LDataOperations = new DataOperations01();
                switch (AFuncType)
                {
                    case 1: //ScoreNumber 获取评分数量
                        {
                            if (ADataBaseConfig.IntDatabaseType == 2)
                            {
                                LStrDynamicSQL = string.Format("SELECT COUNT(1) AS VALUE01 FROM {0} T38,{1} T21 WHERE T38.C002=T21.C002 AND T21.C004>='{2}' AND T21.C004<'{3}' AND T38.C013={4}", TableName38, TableName21, ADateTimeStart, ADateTimeStop, AObjectInfo.ObjID);
                            }
                            else if (ADataBaseConfig.IntDatabaseType == 3)
                            {
                                LStrDynamicSQL = string.Format("SELECT COUNT(1) AS VALUE01 FROM {0} T38,{1} T21 WHERE T38.C002=T21.C002 AND T21.C004>=TO_DATE ('{2}','YYYY-MM-DD HH24:MI:SS') AND T21.C004<TO_DATE ('{3}','YYYY-MM-DD HH24:MI:SS') AND T38.C013={4}", TableName38, TableName21, ADateTimeStart, ADateTimeStop, AObjectInfo.ObjID);
                            }
                        }
                        break;
                    case 2://坐席\分机申述数量
                        {
                            if (ADataBaseConfig.IntDatabaseType == 2)
                            {
                                LStrDynamicSQL = string.Format("SELECT COUNT(1) AS VALUE01 FROM {0} WHERE C012>='{1}' AND C012<'{2}' AND C004={3} AND C008=1", TableName319, utcTimeStart, utcTimeEnd, AObjectInfo.ObjID);
                            }
                            else if (ADataBaseConfig.IntDatabaseType == 3)
                            {
                                LStrDynamicSQL = string.Format("SELECT COUNT(1) AS VALUE01 FROM {0} WHERE C012>=TO_DATE ('{1}','YYYY-MM-DD HH24:MI:SS') AND C012<TO_DATE ('{2}','YYYY-MM-DD HH24:MI:SS') AND C004={3} AND C008=1", TableName319, utcTimeStart, utcTimeEnd, AObjectInfo.ObjID);
                            }
                        }
                        break;
                    case 3://坐席\分机重新打分的数量（申诉成功并重新打分）
                        {
                            if (ADataBaseConfig.IntDatabaseType == 2)
                            {
                                LStrDynamicSQL = string.Format("SELECT COUNT(1) AS VALUE01 FROM {0} WHERE C012>='{1}' AND C012<'{2}' AND C004={3} AND C008=6", TableName319, utcTimeStart, utcTimeEnd, AObjectInfo.ObjID);
                            }
                            else if (ADataBaseConfig.IntDatabaseType == 3)
                            {
                                LStrDynamicSQL = string.Format("SELECT COUNT(1) AS VALUE01 FROM {0} WHERE C012>=TO_DATE ('{1}','YYYY-MM-DD HH24:MI:SS') AND C012<TO_DATE ('{2}','YYYY-MM-DD HH24:MI:SS') AND C004={3} AND C008=6", TableName319, utcTimeStart, utcTimeEnd, AObjectInfo.ObjID);
                            }
                        }
                        break;
                    case 4://坐席\分机坐席被质检的总分数
                        {
                            if (ADataBaseConfig.IntDatabaseType == 2)
                            {
                                LStrDynamicSQL = string.Format("SELECT SUM(T38.C004) AS VALUE01 FROM {0} T38,{1} T21 WHERE T38.C002=T21.C002 AND T21.C004>='{2}' AND T21.C004<'{3}' AND T38.C013={4}", TableName38, TableName21, ADateTimeStart, ADateTimeStop, AObjectInfo.ObjID);
                            }
                            else if (ADataBaseConfig.IntDatabaseType == 3)
                            {
                                LStrDynamicSQL = string.Format("SELECT SUM(T38.C004) AS VALUE01 FROM {0} T38,{1} T21 WHERE T38.C002=T21.C002 AND T21.C004>=TO_DATE ('{2}','YYYY-MM-DD HH24:MI:SS') AND T21.C004<TO_DATE ('{3}','YYYY-MM-DD HH24:MI:SS') AND T38.C013={4}", TableName38, TableName21, ADateTimeStart, ADateTimeStop, AObjectInfo.ObjID);
                            }
                        }
                        break;
                    case 5://质检员被申述的数量
                        {
                            if (ADataBaseConfig.IntDatabaseType == 2)
                            {
                                LStrDynamicSQL = string.Format("SELECT COUNT(1) AS VALUE01 FROM {0} T319,{1} T308 WHERE T319.C002 = T308.C001 AND T319.C012>='{2}' AND T319.C012<'{3}' AND T319.C0008=1 AND T308.C005={4}", TableName319, TableName38, utcTimeStart, utcTimeEnd, AObjectInfo.ObjID);
                            }
                            else if (ADataBaseConfig.IntDatabaseType == 3)
                            {
                                LStrDynamicSQL = string.Format("SELECT COUNT(1) AS VALUE01 FROM {0} T319,{1} T308 WHERE T319.C002 = T308.C001 AND T319.C012>=TO_DATE ('{2}','YYYY-MM-DD HH24:MI:SS') AND T319.C012<TO_DATE ('{3}','YYYY-MM-DD HH24:MI:SS') AND T319.C0008=1 AND T308.C005={4}", TableName319, TableName38, utcTimeStart, utcTimeEnd, AObjectInfo.ObjID);
                            }
                        }
                        break;
                    case 6://质检员质检的数量
                        {
                            if (ADataBaseConfig.IntDatabaseType == 2)
                            {
                                LStrDynamicSQL = string.Format("SELECT COUNT(1) AS VALUE01 FROM {0} WHERE C006>='{1}' AND C006<'{2}' AND C005={3}", TableName38, utcTimeStart, utcTimeEnd, AObjectInfo.ObjID);
                            }
                            else if (ADataBaseConfig.IntDatabaseType == 3)
                            {
                                LStrDynamicSQL = string.Format("SELECT COUNT(1) AS VALUE01 FROM {0} WHERE C006>=TO_DATE ('{1}','YYYY-MM-DD HH24:MI:SS') AND C006<TO_DATE ('{2}','YYYY-MM-DD HH24:MI:SS') AND C005={3}", TableName38, utcTimeStart, utcTimeEnd, AObjectInfo.ObjID);
                            }
                        }
                        break;
                    case 7://质检员完成的任务数量
                        {
                            if (ADataBaseConfig.IntDatabaseType == 2)
                            {
                                LStrDynamicSQL = string.Format("SELECT COUNT(1) AS VALUE01 FROM {0} T20,{1} T21 WHERE T20.C001=T21.C001 AND T20.C018>='{2}' AND T20.C018<'{3}' AND T21.C002={4} AND T20.C017='Y'", TableName320, TableName321,utcTimeStart, utcTimeEnd, AObjectInfo.ObjID);
                            }
                            else if (ADataBaseConfig.IntDatabaseType == 3)
                            {
                                LStrDynamicSQL = string.Format("SELECT COUNT(1) AS VALUE01 FROM {0} T20,{1} T21 WHERE T20.C001=T21.C001 AND T20.C018>=TO_DATE ('{2}','YYYY-MM-DD HH24:MI:SS') AND T20.C018<TO_DATE ('{3}','YYYY-MM-DD HH24:MI:SS') AND T21.C002={4} T20.C017='Y'", TableName320, TableName321, utcTimeStart, utcTimeEnd, AObjectInfo.ObjID);
                            }
                        }
                        break;
                    case 8://质检员接到的任务数量
                        {
                            if (ADataBaseConfig.IntDatabaseType == 2)
                            {
                                LStrDynamicSQL = string.Format("SELECT COUNT(1) AS VALUE01 FROM {0} T20,{1} T21 WHERE T20.C001=T21.C001 AND T20.C006>='{2}' AND T20.C006<'{3}' AND T21.C002={4}", TableName320, TableName321, utcTimeStart, utcTimeEnd, AObjectInfo.ObjID);
                            }
                            else if (ADataBaseConfig.IntDatabaseType == 3)
                            {
                                LStrDynamicSQL = string.Format("SELECT COUNT(1) AS VALUE01 FROM {0} T20,{1} T21 WHERE T20.C001=T21.C001 AND T20.C006>=TO_DATE ('{2}','YYYY-MM-DD HH24:MI:SS') AND T20.C006<TO_DATE ('{3}','YYYY-MM-DD HH24:MI:SS') AND T21.C002={4}", TableName320, TableName321, utcTimeStart, utcTimeEnd, AObjectInfo.ObjID);
                            }
                        }
                        break;
                    default:
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
                    foreach (DataRow LDataRowSingleRow in LDataTableReturn.Rows)
                    {
                        Value01 += Convert.ToInt64(DoubleParse(LDataRowSingleRow["VALUE01"].ToString(), 0)*1000);
                    }
                }
            }
            catch (Exception ex)
            {
                LDataTableReturn = null;
                FileLog.WriteInfo("GetQMDayStatisticsInfo()", ex.Message);
            }
            return Value01;
        }
    }
}
