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

    //得到录音的统计信息
    public class DALRecordInfo : BasicMethod
    {

        /// <summary>
        ///  以本地时间查询录音表的数据
        /// </summary>
        /// <param name="ADataBaseConfig"></param>
        /// <param name="AObjectInfo"></param>
        /// <param name="AGlobalSetting"></param>
        /// <param name="ADateTimeStart"></param>
        /// <param name="ADateTimeStop"></param>
        /// <param name="AFuncType"></param>
        /// <param name="AExtensionAgentType"></param>
        /// <returns></returns>
        public static long GetAllRecordStatisticsInfo(DataBaseConfig ADataBaseConfig,ObjectInfo AObjectInfo, GlobalSetting AGlobalSetting,DateTime ADateTimeStart,DateTime ADateTimeStop,int AFuncType)
        {
            long Value01 = 0;
            DataTable LDataTableReturn = new DataTable();
            string LStrDynamicSQL = string.Empty;
            string LStrRentToken = string.Empty;
            string LStrSingleObject = string.Empty;

            

            //根据座席和分机和真实分机查询相应  功能的值
            string TableName = string.Empty;
            DateTime utcTime = ADateTimeStart.ToUniversalTime();
            if (AGlobalSetting.IlogicPartMark == 1)//为1按月分表
            {
                TableName = string.Format("T_21_001_{0}_{1}{2}", AGlobalSetting.StrRent, utcTime.ToString("yy"), utcTime.ToString("MM"));
                if(!AGlobalSetting.LStrRecordName.Contains(TableName))
                {
                    return  0;
                }
            }
            else 
            {
                TableName = string.Format("T_21_001_{0}", AGlobalSetting.StrRent);
            }


            try
            {
                DatabaseOperation01Return LDatabaseOperationReturn = new DatabaseOperation01Return();
                DataOperations01 LDataOperations = new DataOperations01();
                switch (AFuncType)
                {
                    case 1: //RecordLength
                        {
                            if (ADataBaseConfig.IntDatabaseType == 2)
                            {
                                if (AObjectInfo.ObjType == 2) //分机
                                {
                                    LStrDynamicSQL = string.Format("SELECT SUM(C012) AS VALUE01 FROM {0} WHERE C004>='{2}' AND C004<'{3}' AND C042='{1}' AND C020='{4}'  ", TableName, AObjectInfo.ObjName, ADateTimeStart,ADateTimeStop,AObjectInfo.ExtensionIP);
                                }
                                else if (AObjectInfo.ObjType == 1) //座席
                                {
                                    LStrDynamicSQL = string.Format("SELECT SUM(C012) AS VALUE01 FROM {0} WHERE C004>='{2}' AND C004<'{3}' AND C039='{1}' ", TableName, AObjectInfo.ObjName, ADateTimeStart, ADateTimeStop);
                                }
                                else if (AObjectInfo.ObjType == 4) //真实分机
                                {
                                    LStrDynamicSQL = string.Format("SELECT SUM(C012) AS VALUE01 FROM {0} WHERE C004>='{2}' AND C004<'{3}' AND C058='{1}' AND C020='{4}' ", TableName, AObjectInfo.ObjName, ADateTimeStart, ADateTimeStop, AObjectInfo.ExtensionIP);
                                }
                            }
                            else if(ADataBaseConfig.IntDatabaseType==3)
                            {
                                if (AObjectInfo.ObjType == 2) //分机
                                {
                                    LStrDynamicSQL = string.Format("SELECT SUM(C012) AS VALUE01 FROM {0} WHERE C004>=TO_DATE ('{2}','YYYY-MM-DD HH24:MI:SS') AND C004<TO_DATE ('{3}','YYYY-MM-DD HH24:MI:SS') AND C042='{1}' AND C020='{4}' ", TableName, AObjectInfo.ObjName, ADateTimeStart, ADateTimeStop, AObjectInfo.ExtensionIP);
                                }
                                else if (AObjectInfo.ObjType == 1) //座席
                                {
                                    LStrDynamicSQL = string.Format("SELECT SUM(C012) AS VALUE01 FROM {0} WHERE C004>=TO_DATE ('{2}','YYYY-MM-DD HH24:MI:SS') AND C004<TO_DATE ('{3}','YYYY-MM-DD HH24:MI:SS') AND C039='{1}' ", TableName, AObjectInfo.ObjName, ADateTimeStart, ADateTimeStop);
                                }
                                else if (AObjectInfo.ObjType == 4) //真实分机
                                {
                                    LStrDynamicSQL = string.Format("SELECT SUM(C012) AS VALUE01 FROM {0} WHERE C004>=TO_DATE ('{2}','YYYY-MM-DD HH24:MI:SS') AND C004<TO_DATE ('{3}','YYYY-MM-DD HH24:MI:SS') AND C058='{1}'  AND C020='{4}' ", TableName, AObjectInfo.ObjName, ADateTimeStart, ADateTimeStop, AObjectInfo.ExtensionIP);
                                }
                            }
                            
                            
                        }
                        break;
                    case 2://RecordNumber
                        {

                            if (ADataBaseConfig.IntDatabaseType == 2)
                            {
                                if (AObjectInfo.ObjType == 2) //分机
                                {
                                    LStrDynamicSQL = string.Format("SELECT COUNT(C001) AS VALUE01 FROM {0} WHERE C004>='{2}' AND C004<'{3}' AND C042='{1}' AND C020='{4}' ", TableName, AObjectInfo.ObjName, ADateTimeStart, ADateTimeStop, AObjectInfo.ExtensionIP);
                                }
                                else if (AObjectInfo.ObjType == 1) //座席
                                {
                                    LStrDynamicSQL = string.Format("SELECT COUNT(C001) AS VALUE01 FROM {0} WHERE C004>='{2}' AND C004<'{3}' AND C039='{1}' ", TableName, AObjectInfo.ObjName, ADateTimeStart, ADateTimeStop);
                                }
                                else if (AObjectInfo.ObjType == 4) //真实分机
                                {
                                    LStrDynamicSQL = string.Format("SELECT COUNT(C001) AS VALUE01 FROM {0} WHERE C004>='{2}' AND C004<'{3}' AND C059='{1}' AND C020='{4}' ", TableName, AObjectInfo.ObjName, ADateTimeStart, ADateTimeStop, AObjectInfo.ExtensionIP);
                                }
                            }
                            else if(ADataBaseConfig.IntDatabaseType==3)
                            {

                                if (AObjectInfo.ObjType == 2) //分机
                                {
                                    LStrDynamicSQL = string.Format("SELECT COUNT(C001) AS VALUE01 FROM {0} WHERE C004>=TO_DATE ('{2}','YYYY-MM-DD HH24:MI:SS') AND C004<TO_DATE ('{3}','YYYY-MM-DD HH24:MI:SS') AND C042='{1}' AND C020='{4}' ", TableName, AObjectInfo.ObjName, ADateTimeStart, ADateTimeStop, AObjectInfo.ExtensionIP);
                                }
                                else if (AObjectInfo.ObjType == 1) //座席
                                {
                                    LStrDynamicSQL = string.Format("SELECT COUNT(C001) AS VALUE01 FROM {0} WHERE C004>=TO_DATE ('{2}','YYYY-MM-DD HH24:MI:SS') AND C004<TO_DATE ('{3}','YYYY-MM-DD HH24:MI:SS') AND C039='{1}' ", TableName, AObjectInfo.ObjName, ADateTimeStart, ADateTimeStop);

                                }
                                else if (AObjectInfo.ObjType == 4) //真实分机
                                {
                                    LStrDynamicSQL = string.Format("SELECT COUNT(C001) AS VALUE01 FROM {0} WHERE C004>=TO_DATE ('{2}','YYYY-MM-DD HH24:MI:SS') AND C004<TO_DATE ('{3}','YYYY-MM-DD HH24:MI:SS') AND C058='{1}' AND C020='{4}' ", TableName, AObjectInfo.ObjName, ADateTimeStart, ADateTimeStop, AObjectInfo.ExtensionIP);
                                }
                            }

                        }
                        break;
                    case 3: //RingTime
                        {
                            if (ADataBaseConfig.IntDatabaseType == 2)
                            {
                                if (AObjectInfo.ObjType == 2) //分机
                                {
                                    LStrDynamicSQL = string.Format("SELECT SUM(C061) AS VALUE01 FROM {0} WHERE C004>='{2}' AND C004<'{3}' AND C042='{1}' AND C020='{4}'  ", TableName, AObjectInfo.ObjName, ADateTimeStart, ADateTimeStop, AObjectInfo.ExtensionIP);
                                }
                                else if (AObjectInfo.ObjType == 1) //座席
                                {
                                    LStrDynamicSQL = string.Format("SELECT SUM(C061) AS VALUE01 FROM {0} WHERE C004>='{2}' AND C004<'{3}' AND C039='{1}' ", TableName, AObjectInfo.ObjName, ADateTimeStart, ADateTimeStop);
                                }
                                else if (AObjectInfo.ObjType == 4) //真实分机
                                {
                                    LStrDynamicSQL = string.Format("SELECT SUM(C061) AS VALUE01 FROM {0} WHERE C004>='{2}' AND C004<'{3}' AND C058='{1}' AND C020='{4}' ", TableName, AObjectInfo.ObjName, ADateTimeStart, ADateTimeStop, AObjectInfo.ExtensionIP);
                                }
                            }
                            else if (ADataBaseConfig.IntDatabaseType == 3) 
                            {
                                if (AObjectInfo.ObjType == 2) //分机
                                {
                                    LStrDynamicSQL = string.Format("SELECT SUM(C061) AS VALUE01 FROM {0} WHERE C004>=TO_DATE ('{2}','YYYY-MM-DD HH24:MI:SS') AND C004<TO_DATE ('{3}','YYYY-MM-DD HH24:MI:SS') AND C042='{1}' AND C020='{4}' ", TableName, AObjectInfo.ObjName, ADateTimeStart, ADateTimeStop, AObjectInfo.ExtensionIP);
                                }
                                else if (AObjectInfo.ObjType == 1) //座席
                                {
                                    LStrDynamicSQL = string.Format("SELECT SUM(C061) AS VALUE01 FROM {0} WHERE C004>=TO_DATE ('{2}','YYYY-MM-DD HH24:MI:SS') AND C004<TO_DATE ('{3}','YYYY-MM-DD HH24:MI:SS') AND C039='{1}' ", TableName, AObjectInfo.ObjName, ADateTimeStart, ADateTimeStop);
                                }
                                else if (AObjectInfo.ObjType == 4) //真实分机
                                {
                                    LStrDynamicSQL = string.Format("SELECT SUM(C061) AS VALUE01 FROM {0} WHERE C004>=TO_DATE ('{2}','YYYY-MM-DD HH24:MI:SS') AND C004<TO_DATE ('{3}','YYYY-MM-DD HH24:MI:SS') AND C058='{1}'  AND C020='{4}' ", TableName, AObjectInfo.ObjName, ADateTimeStart, ADateTimeStop, AObjectInfo.ExtensionIP);
                                }
                            }
                        }
                        break;
                    case 4://holdTime
                        {
                            if (ADataBaseConfig.IntDatabaseType == 2)
                            {
                                if (AObjectInfo.ObjType == 2) //分机
                                {
                                    LStrDynamicSQL = string.Format("SELECT SUM(C060) AS VALUE01 FROM {0} WHERE C004>='{2}' AND C004<'{3}' AND C042='{1}' AND C020='{4}'  ", TableName, AObjectInfo.ObjName, ADateTimeStart, ADateTimeStop, AObjectInfo.ExtensionIP);
                                }
                                else if (AObjectInfo.ObjType == 1) //座席
                                {
                                    LStrDynamicSQL = string.Format("SELECT SUM(C060) AS VALUE01 FROM {0} WHERE C004>='{2}' AND C004<'{3}' AND C039='{1}' ", TableName, AObjectInfo.ObjName, ADateTimeStart, ADateTimeStop);
                                }
                                else if (AObjectInfo.ObjType == 4) //真实分机
                                {
                                    LStrDynamicSQL = string.Format("SELECT SUM(C060) AS VALUE01 FROM {0} WHERE C004>='{2}' AND C004<'{3}' AND C058='{1}' AND C020='{4}' ", TableName, AObjectInfo.ObjName, ADateTimeStart, ADateTimeStop, AObjectInfo.ExtensionIP);
                                }
                            }
                            else if (ADataBaseConfig.IntDatabaseType == 3)
                            {
                                if (AObjectInfo.ObjType == 2) //分机
                                {
                                    LStrDynamicSQL = string.Format("SELECT SUM(C060) AS VALUE01 FROM {0} WHERE C004>=TO_DATE ('{2}','YYYY-MM-DD HH24:MI:SS') AND C004<TO_DATE ('{3}','YYYY-MM-DD HH24:MI:SS') AND C042='{1}' AND C020='{4}' ", TableName, AObjectInfo.ObjName, ADateTimeStart, ADateTimeStop, AObjectInfo.ExtensionIP);
                                }
                                else if (AObjectInfo.ObjType == 1) //座席
                                {
                                    LStrDynamicSQL = string.Format("SELECT SUM(C060) AS VALUE01 FROM {0} WHERE C004>=TO_DATE ('{2}','YYYY-MM-DD HH24:MI:SS') AND C004<TO_DATE ('{3}','YYYY-MM-DD HH24:MI:SS') AND C039='{1}' ", TableName, AObjectInfo.ObjName, ADateTimeStart, ADateTimeStop);
                                }
                                else if (AObjectInfo.ObjType == 4) //真实分机
                                {
                                    LStrDynamicSQL = string.Format("SELECT SUM(C060) AS VALUE01 FROM {0} WHERE C004>=TO_DATE ('{2}','YYYY-MM-DD HH24:MI:SS') AND C004<TO_DATE ('{3}','YYYY-MM-DD HH24:MI:SS') AND C058='{1}'  AND C020='{4}' ", TableName, AObjectInfo.ObjName, ADateTimeStart, ADateTimeStop, AObjectInfo.ExtensionIP);
                                }
                            }
                        }
                        break;
                    case 5: 
                        {
                            if (ADataBaseConfig.IntDatabaseType == 2)
                            {
                                if (AObjectInfo.ObjType == 2) //分机
                                {
                                }
                                else if (AObjectInfo.ObjType == 1) //座席
                                {
                                }
                                else if (AObjectInfo.ObjType == 4) //真实分机
                                {
                                }
                            }
                            else if (ADataBaseConfig.IntDatabaseType == 3)
                            {
                                if (AObjectInfo.ObjType == 2) //分机
                                {
                                }
                                else if (AObjectInfo.ObjType == 1) //座席
                                {
                                }
                                else if (AObjectInfo.ObjType == 4) //真实分机
                                {
                                }
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
                        //Value01 += LongParse(LDataRowSingleRow["VALUE01"].ToString(), 0);
                        //扩大1000倍
                        Value01 += Convert.ToInt64(DoubleParse(LDataRowSingleRow["VALUE01"].ToString(), 0) * 1000);
                    }
                }
            }
            catch (Exception ex)
            {
                LDataTableReturn = null;
                FileLog.WriteInfo("GetAllRecordStatisticsInfo()", ex.Message);
            }
            return Value01;
        }
    }
}
