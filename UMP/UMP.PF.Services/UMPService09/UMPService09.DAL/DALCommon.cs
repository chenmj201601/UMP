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
using VoiceCyber.Common;
using System.Data.Common;


namespace UMPService09.DAL
{
    public class DALCommon:BasicMethod
    {

        /// <summary>
        /// 写入临时表
        /// </summary>
        /// <param name="listParams"></param>
        /// <param name="AStrRent"></param>
        /// <returns></returns>
        public static OperationReturn InsertTempData(DataBaseConfig ADataBaseConfig, List<string> listParams, String AStrRent)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                if (listParams.Count < 2)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Param count invalid");
                    return optReturn;
                }
                //ListParams
                //0         tempID
                //1         count
                //2..       tempData(tempData property split by char 27, less than 5)
                string strTempID = listParams[0];
                string strCount = listParams[1];
                int intCount;
                if (!int.TryParse(strCount, out intCount) || intCount <= 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Resource count invalid");
                    return optReturn;
                }
                if (listParams.Count < 2 + intCount)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Resource count invalid");
                    return optReturn;
                }
                if (string.IsNullOrEmpty(strTempID))
                {
                    List<string> listGetSerialIDParams = new List<string>();
                    listGetSerialIDParams.Add("11");
                    listGetSerialIDParams.Add("911");
                    listGetSerialIDParams.Add(DateTime.Now.ToString("yyyyMMddHHmmss"));
                    OperationReturn getSerialIDReturn = GetSerialID(ADataBaseConfig,listGetSerialIDParams, AStrRent);
                    if (!getSerialIDReturn.Result)
                    {
                        return getSerialIDReturn;
                    }
                    strTempID = getSerialIDReturn.Data.ToString();
                }
                string strSql;
                IDbConnection objConn;
                IDbDataAdapter objAdapter;
                DbCommandBuilder objCmdBuilder;
                switch (ADataBaseConfig.IntDatabaseType)
                {
                    //MSSQL
                    case 2:
                        strSql = string.Format("select * from t_00_901 where c001 = {0}", strTempID);
                        objConn = MssqlOperation.GetConnection(ADataBaseConfig.StrDatabaseProfile);
                        objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = MssqlOperation.GetCommandBuilder(objAdapter);
                        break;
                    //ORCL
                    case 3:
                        strSql = string.Format("select * from t_00_901 where c001 = {0}", strTempID);
                        objConn = OracleOperation.GetConnection(ADataBaseConfig.StrDatabaseProfile);
                        objAdapter = OracleOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = OracleOperation.GetCommandBuilder(objAdapter);
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("Database type not support");
                        return optReturn;
                }
                if (objConn == null || objAdapter == null || objCmdBuilder == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("Db object is null");
                    return optReturn;
                }
                objCmdBuilder.ConflictOption = ConflictOption.OverwriteChanges;
                objCmdBuilder.SetAllValues = false;
                try
                {
                    DataSet objDataSet = new DataSet();
                    objAdapter.Fill(objDataSet);

                    int number = 0;
                    for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                    {
                        number = Math.Max(number, Convert.ToInt32(objDataSet.Tables[0].Rows[i]["C002"]));
                    }
                    for (int i = 2; i < listParams.Count; i++)
                    {
                        DataRow dr = objDataSet.Tables[0].NewRow();
                        string strTempData = listParams[i];
                        string[] arrTempData = strTempData.Split(new[] { AscCodeToChr(27) },
                            StringSplitOptions.RemoveEmptyEntries);
                        dr["C001"] = strTempID;
                        dr["C002"] = number + i - 1;
                        if (arrTempData.Length > 0)
                        {
                            dr["C011"] = arrTempData[0];
                        }
                        if (arrTempData.Length > 1)
                        {
                            dr["C012"] = arrTempData[1];
                        }
                        if (arrTempData.Length > 2)
                        {
                            dr["C013"] = arrTempData[2];
                        }
                        if (arrTempData.Length > 3)
                        {
                            dr["C014"] = arrTempData[3];
                        }
                        if (arrTempData.Length > 4)
                        {
                            dr["C015"] = arrTempData[4];
                        }
                        objDataSet.Tables[0].Rows.Add(dr);
                    }
                    objAdapter.Update(objDataSet);
                    objDataSet.AcceptChanges();
                }
                catch (Exception ex)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_DBACCESS_FAIL;
                    optReturn.Message = ex.Message;
                }
                finally
                {
                    if (objConn.State == ConnectionState.Open)
                    {
                        objConn.Close();
                    }
                    objConn.Dispose();
                }

                optReturn.Data = strTempID;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }



        /// <summary>
        /// 创建主键
        /// </summary>
        /// <param name="listParams"></param>
        /// <param name="AStrRent"></param>
        /// <returns></returns>
        public static  OperationReturn GetSerialID(DataBaseConfig ADataBaseConfig, List<string> listParams, string AStrRent)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParams
                //0     模块编码
                //1     模块内编码
                //2     时间变量
                if (listParams == null || listParams.Count < 3)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string moduleID = listParams[0];
                string resourceID = listParams[1];
                string dateFormat = listParams[2];
                string rentToken = AStrRent;
                string strSerialID = string.Empty;
                long errNumber = 0;
                string strErrMsg = string.Empty;
                switch (ADataBaseConfig.IntDatabaseType)
                {
                    case 2:
                        DbParameter[] mssqlParameters =
                        {
                            MssqlOperation.GetDbParameter("@AInParam01",MssqlDataType.Varchar,2),
                            MssqlOperation.GetDbParameter("@AInParam02",MssqlDataType.Varchar,3),
                            MssqlOperation.GetDbParameter("@AInParam03",MssqlDataType.Varchar,5),
                            MssqlOperation.GetDbParameter("@Ainparam04",MssqlDataType.Varchar,20),
                            MssqlOperation.GetDbParameter("@AOutParam01",MssqlDataType.Varchar,20),
                            MssqlOperation.GetDbParameter("@AOutErrorNumber",MssqlDataType.Bigint,0),
                            MssqlOperation.GetDbParameter("@AOutErrorString",MssqlDataType.NVarchar,4000)
                        };
                        mssqlParameters[0].Value = moduleID;
                        mssqlParameters[1].Value = resourceID;
                        mssqlParameters[2].Value = rentToken;
                        mssqlParameters[3].Value = dateFormat;
                        mssqlParameters[4].Value = strSerialID;
                        mssqlParameters[5].Value = errNumber;
                        mssqlParameters[6].Value = strErrMsg;
                        mssqlParameters[4].Direction = ParameterDirection.Output;
                        mssqlParameters[5].Direction = ParameterDirection.Output;
                        mssqlParameters[6].Direction = ParameterDirection.Output;
                        optReturn = MssqlOperation.ExecuteStoredProcedure(ADataBaseConfig.StrDatabaseProfile, "P_00_001",
                           mssqlParameters);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        if (mssqlParameters[5].Value.ToString() != "0")
                        {
                            optReturn.Result = false;
                            optReturn.Code = Defines.RET_DBACCESS_FAIL;
                            optReturn.Message = string.Format("{0}\t{1}", mssqlParameters[5].Value, mssqlParameters[6].Value);
                        }
                        else
                        {
                            strSerialID = mssqlParameters[4].Value.ToString();
                            optReturn.Data = strSerialID;
                        }
                        break;
                    case 3:
                        DbParameter[] orclParameters =
                        {
                            OracleOperation.GetDbParameter("AInParam01",OracleDataType.Varchar2,2),
                            OracleOperation.GetDbParameter("AInParam02",OracleDataType.Varchar2,3),
                            OracleOperation.GetDbParameter("AInParam03",OracleDataType.Varchar2,5),
                            OracleOperation.GetDbParameter("Ainparam04",OracleDataType.Varchar2,20),
                            OracleOperation.GetDbParameter("AOutParam01",OracleDataType.Varchar2,20),
                            OracleOperation.GetDbParameter("AOutErrorNumber",OracleDataType.Int32,0),
                            OracleOperation.GetDbParameter("AOutErrorString",OracleDataType.Nvarchar2,4000)
                        };
                        orclParameters[0].Value = moduleID;
                        orclParameters[1].Value = resourceID;
                        orclParameters[2].Value = rentToken;
                        orclParameters[3].Value = dateFormat;
                        orclParameters[4].Value = strSerialID;
                        orclParameters[5].Value = errNumber;
                        orclParameters[6].Value = strErrMsg;
                        orclParameters[4].Direction = ParameterDirection.Output;
                        orclParameters[5].Direction = ParameterDirection.Output;
                        orclParameters[6].Direction = ParameterDirection.Output;
                        optReturn = OracleOperation.ExecuteStoredProcedure(ADataBaseConfig.StrDatabaseProfile, "P_00_001",
                           orclParameters);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        if (orclParameters[5].Value.ToString() != "0")
                        {
                            optReturn.Result = false;
                            optReturn.Code = Defines.RET_DBACCESS_FAIL;
                            optReturn.Message = string.Format("{0}\t{1}", orclParameters[5].Value, orclParameters[6].Value);
                        }
                        else
                        {
                            strSerialID = orclParameters[4].Value.ToString();
                            optReturn.Data = strSerialID;
                        }
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("Database type not surpport.\t{0}", ADataBaseConfig.IntDatabaseType);
                        return optReturn;
                }
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
                return optReturn;
            }
            return optReturn;
        }


        /// <summary>
        /// 根据机构或技能组在T_11_201表查询它下面对应的用户和座席和分机
        /// </summary>
        /// <param name="ADataBaseConfig"></param>
        /// <param name="AResourceCodeBegin"></param>
        /// <param name="AResourceCodeEnd"></param>
        /// <param name="AStrRent"></param>
        /// <param name="AListObjMappingID"></param>
        public static void GetObjInfoMapping(DataBaseConfig ADataBaseConfig, string AResourceCodeBegin, string AResourceCodeEnd, string AStrRent, ref List<long> AListObjMappingID)
        {
            AListObjMappingID.Clear();

            DataTable LDataTableReturn = new DataTable();
            string LStrDynamicSQL = string.Empty;
            string LStrRentToken = string.Empty;
            string LStrSingleObject = string.Empty;
            long LResourceCodeBegin = LongParse(AResourceCodeBegin + "0000000000000001", 0);
            long LResourceCodeEnd = LongParse(AResourceCodeEnd + "0000000000000001", 0);
            try
            {
                DatabaseOperation01Return LDatabaseOperationReturn = new DatabaseOperation01Return();
                DataOperations01 LDataOperations = new DataOperations01();

                LStrDynamicSQL = string.Format("SELECT * FROM T_11_201_{0} WHERE C003 >={1} AND C003<{2} "
                            , AStrRent, LResourceCodeBegin, LResourceCodeEnd);
                LDatabaseOperationReturn = LDataOperations.SelectDataByDynamicSQL(ADataBaseConfig.IntDatabaseType,ADataBaseConfig.StrDatabaseProfile, LStrDynamicSQL);
                if (!LDatabaseOperationReturn.BoolReturn)
                {
                    LDataTableReturn = null;
                }
                else
                {
                    LDataTableReturn = LDatabaseOperationReturn.DataSetReturn.Tables[0];
                    foreach (DataRow LDataRowSingleRow in LDataTableReturn.Rows)
                    {
                        long LLongObjMappingID = 0;
                        LLongObjMappingID = LongParse(LDataRowSingleRow["C004"].ToString(), 0);
                        AListObjMappingID.Add(LLongObjMappingID);
                    }
                }
            }
            catch (Exception ex)
            {
                LDataTableReturn = null;
                FileLog.WriteInfo("GetObjInfoMapping()", ex.Message);
            }
        }

        /// <summary>
        /// 得到所有的租户
        /// </summary>
        public static void ObtainRentList(DataBaseConfig ADataBaseConfig,   ref List<string> AListStrRentExistObjects)
        {
            DataTable LDataTableReturn = new DataTable();
            string LStrDynamicSQL = string.Empty;
            string LStrRentToken = string.Empty;
            string LStrSingleObject = string.Empty;

            try
            {
                DatabaseOperation01Return LDatabaseOperationReturn = new DatabaseOperation01Return();
                DataOperations01 LDataOperations = new DataOperations01();

                LStrDynamicSQL = "SELECT * FROM T_00_121 ORDER BY C001 ASC";
                LDatabaseOperationReturn = LDataOperations.SelectDataByDynamicSQL(ADataBaseConfig.IntDatabaseType,ADataBaseConfig.StrDatabaseProfile , LStrDynamicSQL);
                if (!LDatabaseOperationReturn.BoolReturn)
                {
                    LDataTableReturn = null;
                }
                else
                {
                    LDataTableReturn = LDatabaseOperationReturn.DataSetReturn.Tables[0];
                    foreach (DataRow LDataRowSingleRent in LDataTableReturn.Rows)
                    {
                        LStrRentToken = EncryptionAndDecryption.EncryptDecryptString(LDataRowSingleRent["C021"].ToString(),IStrVerificationCode102, EncryptionAndDecryption.UMPKeyAndIVType.M102);
                        while (!Regex.IsMatch(LStrRentToken, @"^\d{5}$"))
                        {
                            LStrRentToken = EncryptionAndDecryption.EncryptDecryptString(LDataRowSingleRent["C021"].ToString(),IStrVerificationCode102, EncryptionAndDecryption.UMPKeyAndIVType.M102);
                        }
                        LDataRowSingleRent["C021"] = LStrRentToken;
                        LStrRentToken = LDataRowSingleRent["C021"].ToString();
                        AListStrRentExistObjects.Add(LStrRentToken);
                    }
                }
            }
            catch (Exception ex)
            {
                LDataTableReturn = null;
                FileLog.WriteInfo("ObtainRentList()", ex.Message);
            }
        }


        /// <summary>
        /// 获取是否表有逻辑分表
        /// </summary>
        /// <param name="ARent"></param>
        /// <returns> 0、运行错误1、有按月分表 2、 无按月分表 </returns>
        public static int ObtainRentLogicTable(DataBaseConfig ADataBaseConfig, string ARentToken, string ATableNameField)
        {
            int Flag = 2;
            string LStrDynamicSQL = string.Empty;
            DatabaseOperation01Return LDatabaseOperationReturn = new DatabaseOperation01Return();
            DataOperations01 LDataOperations = new DataOperations01();
            LStrDynamicSQL = "SELECT * FROM T_00_000 WHERE C000 = '" + ARentToken + "' AND C001 = '" + ATableNameField + "' AND C004 = '1'";
            LDatabaseOperationReturn = LDataOperations.SelectDataByDynamicSQL(ADataBaseConfig.IntDatabaseType, ADataBaseConfig.StrDatabaseProfile, LStrDynamicSQL);
            if (!LDatabaseOperationReturn.BoolReturn)
            {
                return Flag = 0;
            }

            if (LDatabaseOperationReturn.StrReturn == "1")
            {
                return Flag = 1;
            }

            return Flag;
        }


        //得到分表信息
        public static DataTable ObtainRentExistLogicPartitionTables(DataBaseConfig ADataBaseConfig,string AStrRentToken, string AStrTableName, ref List<string> AListStringTableName)
        {
            DataTable LDataTableReturn = new DataTable();
            string LStrDynamicSQL = string.Empty;
            AListStringTableName.Clear();
            try
            {
                DatabaseOperation01Return LDatabaseOperationReturn = new DatabaseOperation01Return();
                DataOperations01 LDataOperations = new DataOperations01();

                if (ADataBaseConfig.IntDatabaseType == 2)
                {
                    LStrDynamicSQL = "SELECT NAME AS TABLE_NAME FROM SYSOBJECTS WHERE NAME LIKE '" + AStrTableName + "_" + AStrRentToken + "_%' ORDER BY NAME ASC";
                }
                if (ADataBaseConfig.IntDatabaseType == 3)
                {
                    LStrDynamicSQL = "SELECT TABLE_NAME FROM USER_TABLES WHERE TABLE_NAME LIKE '" + AStrTableName + "_" + AStrRentToken + "_%' ORDER BY TABLE_NAME ASC";
                }
                LDatabaseOperationReturn = LDataOperations.SelectDataByDynamicSQL(ADataBaseConfig.IntDatabaseType, ADataBaseConfig.StrDatabaseProfile, LStrDynamicSQL);
                if (!LDatabaseOperationReturn.BoolReturn)
                {
                    LDataTableReturn = null;

                }
                else
                {
                    LDataTableReturn = LDatabaseOperationReturn.DataSetReturn.Tables[0];
                    string LStringTableName = string.Empty;
                    foreach (DataRow dr in LDataTableReturn.Rows)
                    {
                        LStringTableName = dr["TABLE_NAME"].ToString();
                        AListStringTableName.Add(LStringTableName);
                    }
                }
            }
            catch (Exception ex)
            {
                LDataTableReturn = null;
                FileLog.WriteInfo("ObtainRentExistLogicPartitionTables()", "ERROR:" + ex.Message);
            }
            return LDataTableReturn;
        }


        //得到录音初统计表成绩初统计表切片的天以上切片的数据的统计
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ADataBaseConfig"></param>
        /// <param name="AGlobalSetting"></param>
        /// <param name="TableName"></param>
        /// <param name="ColumnName"></param>
        /// <param name="ObjectType">// 1座席 2分机  3用户 4真实分机 5机构  6 技能组</param>
        /// <param name="StartTimeLocal"></param>
        /// <param name="StopTimeLocal"></param>
        /// <param name="ObjectSerialID"></param>
        /// <returns></returns>
        public static double GetStatisticsValueDayUp(DataBaseConfig ADataBaseConfig, GlobalSetting AGlobalSetting,string TableName,string ColumnName,int  ObjectType,string StartTimeLocal,string StopTimeLocal,string ObjectSerialID)
        {
            double Value01 = 0;
            DataTable LDataTableReturn = new DataTable();
            string LStrDynamicSQL = string.Empty;
            string LStrRentToken = AGlobalSetting.StrRent;
            string LStrSingleObject = string.Empty;
            string LColumnName = ColumnName;
            try
            {
                LStrDynamicSQL = string.Format("SELECT ISNULL(SUM({0}),0) AS Value01  FROM {6} WHERE C001={1} AND C002={2} AND C003 IN (SELECT C011 FROM T_00_901 WHERE C001 = {3})  AND C006>={4} AND C006<{5} ",
                      LColumnName,
                      ObjectType,
                      LStrRentToken,
                      ObjectSerialID,
                       StartTimeLocal,
                       StopTimeLocal,
                       TableName);

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
                        Value01 += DoubleParse(LDataRowSingleRow["Value01"].ToString(), 0);
                    }
                }
            }
            catch (Exception ex)
            {
                FileLog.WriteInfo("GetDayRecordStatistics()", "Error :" + ex.Message);
            }
            return Value01;
        }


        
        /// <summary>
        /// 得到录音表初统计分钟切片表或CTI统计表
        /// </summary>
        /// <param name="ADataBaseConfig"></param>
        /// <param name="AGlobalSetting"></param>
        /// <param name="ATableName"></param>
        /// <param name="ColumnName"></param>
        /// <param name="AObjectType">// 1座席 2分机  3用户 4真实分机 5机构  6 技能组</param>
        /// <param name="AStartTimeLocal"></param>
        /// <param name="AStopTimeLocal"></param>
        /// <param name="AObjectSerialID"></param>
        /// <param name="AStrSum">调用存储过程时的Sum内的列拼的字符串</param>
        /// <returns></returns>
        public static double GetStatisticsValueDayDown(DataBaseConfig ADataBaseConfig, GlobalSetting AGlobalSetting, string ATableName, string AColumnName, int AObjectType, string AStartTimeLocal, string AStopTimeLocal, string AObjectSerialID, KPIFormulaColumn AKpiFormulaColumn)
        {
            double Value01 = 0;
            string ProduceName = string.Empty;
            if(ATableName.Substring(6,1)=="4")
            {
                ProduceName = "P_46_002";
            }else if(ATableName.Substring(6,1)=="3")
            {
                ProduceName = "P_46_001";
            }
            if (AKpiFormulaColumn.SpecialObjectTypeNumber <= 0)
                return 0;

            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            int errNum = 0;
            string errMsg = string.Empty;
            string AOutParam01 = string.Empty;
            try
            {
                switch (ADataBaseConfig.IntDatabaseType)
                {
                    //MSSQL
                    case 2:
                        {
                            DbParameter[] mssqlParameters =
                                {
                                    MssqlOperation.GetDbParameter("@ainparam01",MssqlDataType.Varchar ,5),
                                    MssqlOperation.GetDbParameter("@ainparam02",MssqlDataType.Varchar,5),
                                    MssqlOperation.GetDbParameter("@ainparam03",MssqlDataType.Varchar,20),
                                    MssqlOperation.GetDbParameter("@ainparam04",MssqlDataType.Varchar,5 ),
                                    MssqlOperation.GetDbParameter("@ainparam05",MssqlDataType.Varchar,20),
                                    MssqlOperation.GetDbParameter("@ainparam06",MssqlDataType.Varchar,20),
                                    MssqlOperation.GetDbParameter("@ainparam07",MssqlDataType.Varchar,5000),
                                    MssqlOperation.GetDbParameter("@AOutParam01",MssqlDataType.Varchar,100),
                                    MssqlOperation.GetDbParameter("@aouterrornumber",MssqlDataType.Bigint,0),
                                    MssqlOperation.GetDbParameter("@aouterrorstring",MssqlDataType.Varchar,200)
                                };
                            mssqlParameters[0].Value = AGlobalSetting.StrRent;
                            mssqlParameters[1].Value = AObjectType;
                            mssqlParameters[2].Value = AObjectSerialID;
                            mssqlParameters[3].Value = AKpiFormulaColumn.SpecialObjectTypeNumber.ToString();
                            mssqlParameters[4].Value = AStartTimeLocal;
                            mssqlParameters[5].Value = AStopTimeLocal;
                            mssqlParameters[6].Value = AColumnName;

                            mssqlParameters[7].Value = AOutParam01;
                            mssqlParameters[8].Value = errNum;
                            mssqlParameters[9].Value = errMsg;
                            mssqlParameters[7].Direction = ParameterDirection.Output;
                            mssqlParameters[8].Direction = ParameterDirection.Output;
                            mssqlParameters[9].Direction = ParameterDirection.Output;
                            optReturn = MssqlOperation.ExecuteStoredProcedure(ADataBaseConfig.StrDatabaseProfile, ProduceName,
                               mssqlParameters);

                            if (mssqlParameters[8].Value.ToString() != "0")
                            {
                                optReturn.Result = false;
                                optReturn.Code = Defines.RET_DBACCESS_FAIL;
                                optReturn.Message = mssqlParameters[9].Value.ToString();
                            }
                            else
                            {
                                Value01 = DoubleParse(mssqlParameters[7].Value.ToString(), 0);
                            }
                        }
                        break;
                    //ORCL
                    case 3:
                        {
                            DbParameter[] orclParameters =
                                {
                                    OracleOperation.GetDbParameter("ainparam01",OracleDataType.Varchar2,5 ),
                                    OracleOperation.GetDbParameter("ainparam02",OracleDataType.Varchar2,5),
                                    OracleOperation.GetDbParameter("ainparam03",OracleDataType.Varchar2,20),
                                    OracleOperation.GetDbParameter("ainparam04",OracleDataType.Varchar2,5 ),
                                    OracleOperation.GetDbParameter("ainparam05",OracleDataType.Varchar2,20),
                                    OracleOperation.GetDbParameter("ainparam06",OracleDataType.Varchar2,20),
                                    OracleOperation.GetDbParameter("ainparam07",OracleDataType.Varchar2,5000 ),
                                    OracleOperation.GetDbParameter("AOutParam01",OracleDataType.Varchar2,100),
                                    OracleOperation.GetDbParameter("aouterrornumber",OracleDataType.Int32,0),
                                    OracleOperation.GetDbParameter("aouterrorstring",OracleDataType.Varchar2,200)
                                };
                            orclParameters[0].Value = AGlobalSetting.StrRent;
                            orclParameters[1].Value = AObjectType;
                            orclParameters[2].Value = AObjectSerialID;
                            orclParameters[3].Value = AKpiFormulaColumn.SpecialObjectTypeNumber.ToString();
                            orclParameters[4].Value = AStartTimeLocal;
                            orclParameters[5].Value = AStopTimeLocal;
                            orclParameters[6].Value = AColumnName;

                            orclParameters[7].Value = AOutParam01;
                            orclParameters[8].Value = errNum;
                            orclParameters[9].Value = errMsg;
                            orclParameters[7].Direction = ParameterDirection.Output;
                            orclParameters[8].Direction = ParameterDirection.Output;
                            orclParameters[9].Direction = ParameterDirection.Output;
                            optReturn = OracleOperation.ExecuteStoredProcedure(ADataBaseConfig.StrDatabaseProfile, ProduceName,
                                orclParameters);
                            if (orclParameters[8].Value.ToString() != "0")
                            {
                                optReturn.Result = false;
                                optReturn.Code = Defines.RET_DBACCESS_FAIL;
                                optReturn.Message = orclParameters[9].Value.ToString();
                            }
                            else
                            {
                                Value01 = DoubleParse(orclParameters[7].Value.ToString(), 0);
                            }
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                FileLog.WriteInfo("GetCallPeakValue()", "Error:" + ex.Message.ToString());
            }


            return Value01;
        }
    }
}
