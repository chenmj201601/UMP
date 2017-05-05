using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.ServiceModel.Activation;
using Common5102;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Encryptions;

namespace Wcf51021
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service51021" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service51021.svc or Service51021.svc.cs at the Solution Explorer and start debugging.
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class Service51021 : IService51021
    {
        private OperationReturn OptGetSerialID(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParams
                //0     模块编码
                //1     资源编码
                //2     时间变量
                if (listParams == null || listParams.Count < 3)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = "Request param is null or count invalid";
                    return optReturn;
                }
                string moduleId = listParams[0];
                string resourceId = listParams[1];
                string dateFormat = listParams[2];
                string rentToken = session.RentInfo.Token;
                string strSerialId = string.Empty;
                long errNumber = 0;
                string strErrMsg = string.Empty;
                switch (session.DBType)
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
                        mssqlParameters[0].Value = moduleId;
                        mssqlParameters[1].Value = resourceId;
                        mssqlParameters[2].Value = rentToken;
                        mssqlParameters[3].Value = dateFormat;
                        mssqlParameters[4].Value = strSerialId;
                        mssqlParameters[5].Value = errNumber;
                        mssqlParameters[6].Value = strErrMsg;
                        mssqlParameters[4].Direction = ParameterDirection.Output;
                        mssqlParameters[5].Direction = ParameterDirection.Output;
                        mssqlParameters[6].Direction = ParameterDirection.Output;
                        optReturn = MssqlOperation.ExecuteStoredProcedure(session.DBConnectionString, "P_00_001",
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
                            strSerialId = mssqlParameters[4].Value.ToString();
                            optReturn.Data = strSerialId;
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
                        orclParameters[0].Value = moduleId;
                        orclParameters[1].Value = resourceId;
                        orclParameters[2].Value = rentToken;
                        orclParameters[3].Value = dateFormat;
                        orclParameters[4].Value = strSerialId;
                        orclParameters[5].Value = errNumber;
                        orclParameters[6].Value = strErrMsg;
                        orclParameters[4].Direction = ParameterDirection.Output;
                        orclParameters[5].Direction = ParameterDirection.Output;
                        orclParameters[6].Direction = ParameterDirection.Output;
                        optReturn = OracleOperation.ExecuteStoredProcedure(session.DBConnectionString, "P_00_001",
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
                            strSerialId = orclParameters[4].Value.ToString();
                            optReturn.Data = strSerialId;
                        }
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("Database type not surpport.\t{0}", session.DBType);
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

        private OperationReturn OptAddKwContent(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                if (listParams == null || listParams.Count < 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = "Request param is null or count invalid";
                    return optReturn;
                }
                string strTestInformation = listParams[0];
                optReturn = XMLHelper.DeserializeObject<KwContentInfoParam>(strTestInformation);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                KwContentInfoParam kwConnectInfo = optReturn.Data as KwContentInfoParam;
                if (kwConnectInfo == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = "AddPaperParam is null";
                    return optReturn;
                }
                string rentToken = session.RentInfo.Token;
                string strSql = string.Format("SELECT * FROM T_51_007_{0}",rentToken);
                IDbConnection objConn;
                IDbDataAdapter objAdapter;
                DbCommandBuilder objCmdBuilder;
                switch (session.DBType)
                {
                    case 2:
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objConn = MssqlOperation.GetConnection(session.DBConnectionString);
                        objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = MssqlOperation.GetCommandBuilder(objAdapter);
                        break;
                    case 3:
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objConn = OracleOperation.GetConnection(session.DBConnectionString);
                        objAdapter = OracleOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = OracleOperation.GetCommandBuilder(objAdapter);
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("Database type not surpport.\t{0}", session.DBType);
                        return optReturn;
                }
                if (objConn == null || objAdapter == null || objCmdBuilder == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = "Db object is null";
                    return optReturn;
                }

                objCmdBuilder.ConflictOption = ConflictOption.OverwriteChanges;
                objCmdBuilder.SetAllValues = false;
                DataSet objDataSet = new DataSet();
                objAdapter.Fill(objDataSet);
                DataRow dr = objDataSet.Tables[0].NewRow();
                dr["c001"] = kwConnectInfo.LongKwContentNum;
                dr["c002"] = kwConnectInfo.StrKwContent;
                dr["c003"] = kwConnectInfo.State;
                dr["c004"] = kwConnectInfo.StrAddUtcTime;
                dr["c005"] = kwConnectInfo.StrAddLocalTime;
                dr["c006"] = kwConnectInfo.LongAddPaperNum;
                dr["c007"] = kwConnectInfo.StrAddPaperName;
                dr["c008"] = kwConnectInfo.IntDelete;
                objDataSet.Tables[0].Rows.Add(dr);
                objAdapter.Update(objDataSet);
                objDataSet.AcceptChanges();

                strSql =string.Format( "SELECT * FROM T_51_008_{0}",rentToken);
                switch (session.DBType)
                {
                    case 2:
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objConn = MssqlOperation.GetConnection(session.DBConnectionString);
                        objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = MssqlOperation.GetCommandBuilder(objAdapter);
                        break;
                    case 3:
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objConn = OracleOperation.GetConnection(session.DBConnectionString);
                        objAdapter = OracleOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = OracleOperation.GetCommandBuilder(objAdapter);
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("Database type not surpport.\t{0}", session.DBType);
                        return optReturn;
                }
                if (objConn == null || objAdapter == null || objCmdBuilder == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = "Db object is null";
                    return optReturn;
                }

                objCmdBuilder.ConflictOption = ConflictOption.OverwriteChanges;
                objCmdBuilder.SetAllValues = false;
                objDataSet = new DataSet();
                objAdapter.Fill(objDataSet);
                dr = objDataSet.Tables[0].NewRow();
                dr["c001"] = kwConnectInfo.LongKwContentNum;
                dr["c002"] = 0;
                dr["c003"] = 0;
                objDataSet.Tables[0].Rows.Add(dr);
                objAdapter.Update(objDataSet);
                objDataSet.AcceptChanges();
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

        private OperationReturn OptAddKeyword(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                if (listParams == null || listParams.Count < 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = "Request param is null or count invalid";
                    return optReturn;
                }
                string strTestInformation = listParams[0];
                optReturn = XMLHelper.DeserializeObject<KeywordInfoParam>(strTestInformation);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                KeywordInfoParam kwInfo = optReturn.Data as KeywordInfoParam;
                if (kwInfo == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = "AddPaperParam is null";
                    return optReturn;
                }
                string rentToken = session.RentInfo.Token;
                string strSql = string.Format("SELECT * FROM T_51_006_{0}", rentToken);
                IDbConnection objConn;
                IDbDataAdapter objAdapter;
                DbCommandBuilder objCmdBuilder;
                switch (session.DBType)
                {
                    case 2:
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objConn = MssqlOperation.GetConnection(session.DBConnectionString);
                        objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = MssqlOperation.GetCommandBuilder(objAdapter);
                        break;
                    case 3:
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objConn = OracleOperation.GetConnection(session.DBConnectionString);
                        objAdapter = OracleOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = OracleOperation.GetCommandBuilder(objAdapter);
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("Database type not surpport.\t{0}", session.DBType);
                        return optReturn;
                }
                if (objConn == null || objAdapter == null || objCmdBuilder == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = "Db object is null";
                    return optReturn;
                }

                objCmdBuilder.ConflictOption = ConflictOption.OverwriteChanges;
                objCmdBuilder.SetAllValues = false;
                DataSet objDataSet = new DataSet();
                objAdapter.Fill(objDataSet);
                DataRow dr = objDataSet.Tables[0].NewRow();
                dr["c001"] = kwInfo.LongKwNum;
                dr["c002"] = kwInfo.StrKw;
                dr["c003"] = kwInfo.StrImage;
                dr["c004"] = kwInfo.StrImagePath;
                dr["c005"] = kwInfo.State;
                dr["c006"] = kwInfo.StrAddUtcTime;
                dr["c007"] = kwInfo.StrAddLocalTime;
                dr["c008"] = kwInfo.LongAddPaperNum;
                dr["c009"] = kwInfo.StrAddPaperName;
                dr["c010"] = kwInfo.IntDelete;
                objDataSet.Tables[0].Rows.Add(dr);
                objAdapter.Update(objDataSet);
                objDataSet.AcceptChanges();
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

        private OperationReturn OptSelectKwContent(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                if (listParams == null || listParams.Count < 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = "Request param is null or count invalid";
                    return optReturn;
                }
                string strParam = listParams[0];
                optReturn = XMLHelper.DeserializeObject<string>(strParam);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                string strSql = optReturn.Data as string;
                if (strSql == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = "PapersCategoryParam is null";
                    return optReturn;
                }
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("Database type not surpport.\t{0}", session.DBType);
                        return optReturn;
                }
                List<string> listReturn = new List<string>();
                if (objDataSet == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = "DataSet is null";
                    return optReturn;
                }
                for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = objDataSet.Tables[0].Rows[i];
                    KwContentInfoParam param = new KwContentInfoParam();
                    param.LongKwContentNum = Convert.ToInt64(dr["C001"]);
                    param.StrKwContent = dr["C002"].ToString();
                    param.State = Convert.ToInt16(dr["C003"]);
                    param.StrAddUtcTime = dr["C004"].ToString();
                    param.StrAddLocalTime = dr["C005"].ToString();
                    param.LongAddPaperNum = Convert.ToInt64(dr["C006"]);
                    param.StrAddPaperName = dr["C007"].ToString();
                    param.IntDelete = Convert.ToInt16(dr["C008"]);
                    param.StrDeleteUtcTime = dr["C009"].ToString();
                    param.StrDeleteLocalTime = dr["C010"].ToString();
                    if (!string.IsNullOrEmpty(dr["C011"].ToString()))
                    {
                        param.LongDeletePaperNum = Convert.ToInt64(dr["C011"]);
                    }
                    param.StrDeletePaperName = dr["C012"].ToString();
                    param.StrChangeUtcTime = dr["C013"].ToString();
                    param.StrChangeLocalTime = dr["C014"].ToString();
                    if (!string.IsNullOrEmpty(dr["C015"].ToString()))
                    {
                        param.LongChangePaperNum = Convert.ToInt64(dr["C015"]);
                    }
                    param.StrChangePaperName = dr["C016"].ToString();
                    param.StrDeleteUtcTime = dr["C017"].ToString();
                    param.StrDeleteLocalTime = dr["C018"].ToString();
                    if (!string.IsNullOrEmpty(dr["C019"].ToString()))
                    {
                        param.LongDeletePaperNum = Convert.ToInt64(dr["C019"]);
                    }
                    param.StrDeletePaperName = dr["C020"].ToString();
                    optReturn = XMLHelper.SeriallizeObject(param);
                    if (!optReturn.Result)
                    {
                        optReturn.Code = i;
                        return optReturn;
                    }
                    listReturn.Add(optReturn.Data.ToString());
                }
                optReturn.Data = listReturn;
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

        private OperationReturn OptSelectKeyword(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                if (listParams == null || listParams.Count < 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = "Request param is null or count invalid";
                    return optReturn;
                }
                string strParam = listParams[0];
                optReturn = XMLHelper.DeserializeObject<string>(strParam);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                string strSql = optReturn.Data as string;
                if (strSql == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = "PapersCategoryParam is null";
                    return optReturn;
                }
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("Database type not surpport.\t{0}", session.DBType);
                        return optReturn;
                }
                List<string> listReturn = new List<string>();
                if (objDataSet == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = "DataSet is null";
                    return optReturn;
                }
                for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = objDataSet.Tables[0].Rows[i];
                    KeywordInfoParam param = new KeywordInfoParam();
                    param.LongKwNum = Convert.ToInt64(dr["C001"]);
                    param.StrKw = dr["C002"].ToString();
                    param.StrImage = dr["C003"].ToString();
                    param.StrImagePath = dr["C004"].ToString();
                    param.State = Convert.ToInt16(dr["C005"]);
                    param.StrAddUtcTime = dr["C006"].ToString();
                    param.StrAddLocalTime = dr["C007"].ToString();
                    param.LongAddPaperNum = Convert.ToInt64(dr["C008"]);
                    param.StrAddPaperName = dr["C009"].ToString();
                    param.IntDelete = Convert.ToInt16(dr["C010"]);
                    param.StrDeleteUtcTime = dr["C011"].ToString();
                    param.StrDeleteLocalTime = dr["C012"].ToString();
                    if (!string.IsNullOrEmpty(dr["C013"].ToString()))
                    {
                        param.LongDeletePaperNum = Convert.ToInt64(dr["C013"]);
                    }
                    param.StrDeletePaperName = dr["C014"].ToString();
                    param.StrChangeUtcTime = dr["C015"].ToString();
                    param.StrChangeLocalTime = dr["C016"].ToString();
                    if (!string.IsNullOrEmpty(dr["C017"].ToString()))
                    {
                        param.LongChangePaperNum = Convert.ToInt64(dr["C017"]);
                    }
                    param.StrChangePaperName = dr["C018"].ToString();
                    param.StrRestoreUtcTime = dr["C019"].ToString();
                    param.StrRestoreLocalTime = dr["C020"].ToString();
                    if (!string.IsNullOrEmpty(dr["C021"].ToString()))
                    {
                        param.LongRestorePaperNum = Convert.ToInt64(dr["C021"]);
                    }
                    param.StrRestorePaperName = dr["C022"].ToString();
                    optReturn = XMLHelper.SeriallizeObject(param);
                    if (!optReturn.Result)
                    {
                        optReturn.Code = i;
                        return optReturn;
                    }
                    listReturn.Add(optReturn.Data.ToString());
                }
                optReturn.Data = listReturn;
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

        private OperationReturn OptUpdateKwContent(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                if (listParams == null || listParams.Count < 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = "Request param is null or count invalid";
                    return optReturn;
                }
                string strPaperParam = listParams[0];
                optReturn = XMLHelper.DeserializeObject<KwContentInfoParam>(strPaperParam);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                var param = optReturn.Data as KwContentInfoParam;
                if (param == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = "Paper Param is null";
                    return optReturn;
                }
                string rentToken = session.RentInfo.Token;
                string strSql = string.Format("SELECT * FROM T_51_007_{0}  where c001='{1}' ", rentToken, param.LongKwContentNum);
                IDbConnection objConn;
                IDbDataAdapter objAdapter;
                DbCommandBuilder objCmdBuilder;
                switch (session.DBType)
                {
                    case 2:
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objConn = MssqlOperation.GetConnection(session.DBConnectionString);
                        objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = MssqlOperation.GetCommandBuilder(objAdapter);
                        break;
                    case 3:
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objConn = OracleOperation.GetConnection(session.DBConnectionString);
                        objAdapter = OracleOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = OracleOperation.GetCommandBuilder(objAdapter);
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("Database type not surpport.\t{0}", session.DBType);
                        return optReturn;
                }
                if (objConn == null || objAdapter == null || objCmdBuilder == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = "Db object is null";
                    return optReturn;
                }

                objCmdBuilder.ConflictOption = ConflictOption.CompareRowVersion;
                objCmdBuilder.SetAllValues = false;
                DataSet objDataSet = new DataSet();
                objAdapter.Fill(objDataSet);
                DataRow dr = objDataSet.Tables[0].Rows[0];
                dr.BeginEdit();
                dr["c002"] = param.StrKwContent;
                dr["c003"] = param.State;
                if (!string.IsNullOrEmpty(param.StrChangeUtcTime))
                {
                    dr["c013"] = param.StrChangeUtcTime;
                }
                if (!string.IsNullOrEmpty(param.StrChangeLocalTime))
                {
                    dr["c014"] = param.StrChangeLocalTime;
                }
                dr["c015"] = param.LongChangePaperNum;
                if (!string.IsNullOrEmpty(param.StrChangePaperName))
                {
                    dr["c016"] = param.StrChangePaperName;
                }
                dr.EndEdit();
                objAdapter.Update(objDataSet);
                objDataSet.AcceptChanges();
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

        private OperationReturn OptDeleteKwContent(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                if (listParams == null || listParams.Count < 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = "Request param is null or count invalid";
                    return optReturn;
                }
                string strPaperParam = listParams[0];
                optReturn = XMLHelper.DeserializeObject<KwContentInfoParam>(strPaperParam);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                var param = optReturn.Data as KwContentInfoParam;
                if (param == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = "Paper Param is null";
                    return optReturn;
                }
                string rentToken = session.RentInfo.Token;
                string strSql = string.Format("SELECT * FROM T_51_007_{0}  where c001='{1}' ", rentToken, param.LongKwContentNum);
                IDbConnection objConn;
                IDbDataAdapter objAdapter;
                DbCommandBuilder objCmdBuilder;
                switch (session.DBType)
                {
                    case 2:
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objConn = MssqlOperation.GetConnection(session.DBConnectionString);
                        objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = MssqlOperation.GetCommandBuilder(objAdapter);
                        break;
                    case 3:
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objConn = OracleOperation.GetConnection(session.DBConnectionString);
                        objAdapter = OracleOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = OracleOperation.GetCommandBuilder(objAdapter);
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("Database type not surpport.\t{0}", session.DBType);
                        return optReturn;
                }
                if (objConn == null || objAdapter == null || objCmdBuilder == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = "Db object is null";
                    return optReturn;
                }

                objCmdBuilder.ConflictOption = ConflictOption.CompareRowVersion;
                objCmdBuilder.SetAllValues = false;
                DataSet objDataSet = new DataSet();
                objAdapter.Fill(objDataSet);
                DataRow dr = objDataSet.Tables[0].Rows[0];
                dr.BeginEdit();
                dr["c008"] = param.IntDelete;
                dr["c003"] = param.State;
                if (!string.IsNullOrEmpty(param.StrDeleteUtcTime))
                {
                    dr["c009"] = param.StrDeleteUtcTime;
                }
                if (!string.IsNullOrEmpty(param.StrDeleteLocalTime))
                {
                    dr["c010"] = param.StrDeleteLocalTime;
                }
                dr["c011"] = param.LongDeletePaperNum;
                if (!string.IsNullOrEmpty(param.StrDeletePaperName))
                {
                    dr["c012"] = param.StrDeletePaperName;
                }
                if (!string.IsNullOrEmpty(param.StrRestoreUtcTime))
                {
                    dr["c017"] = param.StrRestoreUtcTime;
                }
                if (!string.IsNullOrEmpty(param.StrRestoreLocalTime))
                {
                    dr["c018"] = param.StrRestoreLocalTime;
                }
                dr["c019"] = param.LongRestorePaperNum;
                if (!string.IsNullOrEmpty(param.StrRestorePaperName))
                {
                    dr["c020"] = param.StrRestorePaperName;
                }
                dr.EndEdit();
                objAdapter.Update(objDataSet);
                objDataSet.AcceptChanges();
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

        private OperationReturn OptSelectAssignKwContent(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                if (listParams == null || listParams.Count < 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = "Request param is null or count invalid";
                    return optReturn;
                }
                string strParam = listParams[0];
                optReturn = XMLHelper.DeserializeObject<string>(strParam);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                string strSql = optReturn.Data as string;
                if (strSql == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = "PapersCategoryParam is null";
                    return optReturn;
                }
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("Database type not surpport.\t{0}", session.DBType);
                        return optReturn;
                }
                List<string> listReturn = new List<string>();
                if (objDataSet == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = "DataSet is null";
                    return optReturn;
                }

                List<AssociationTableParam> lstAssociationTableInfos = new List<AssociationTableParam>();
                for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                {
                    AssociationTableParam parm = new AssociationTableParam();
                    DataRow dr = objDataSet.Tables[0].Rows[i];
                    parm.LongKwConNum = Convert.ToInt64(dr["C001"]);
                    parm.LongKwNum = Convert.ToInt64(dr["C002"]);
                    parm.IntEnable = Convert.ToInt16(dr["C003"]);
                    lstAssociationTableInfos.Add(parm);
                }
                string rentToken = session.RentInfo.Token;
                listReturn = new List<string>();
                foreach (var assparam in lstAssociationTableInfos)
                {
                    strSql = string.Format("select * from T_51_007_{0} where c001='{1}' and c008='0' and c003='1'",rentToken, assparam.LongKwConNum);
                    switch (session.DBType)
                    {
                        case 2:
                            optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                            if (!optReturn.Result)
                            {
                                return optReturn;
                            }
                            objDataSet = optReturn.Data as DataSet;
                            break;
                        case 3:
                            optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                            if (!optReturn.Result)
                            {
                                return optReturn;
                            }
                            objDataSet = optReturn.Data as DataSet;
                            break;
                        default:
                            optReturn.Result = false;
                            optReturn.Code = Defines.RET_PARAM_INVALID;
                            optReturn.Message = string.Format("Database type not surpport.\t{0}", session.DBType);
                            return optReturn;
                    }

                    if (objDataSet == null)
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_OBJECT_NULL;
                        optReturn.Message = "DataSet is null";
                        return optReturn;
                    }


                    for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                    {
                        DataRow dr = objDataSet.Tables[0].Rows[i];
                        KwContentInfoParam param = new KwContentInfoParam();
                        param.LongKwContentNum = Convert.ToInt64(dr["C001"]);
                        param.StrKwContent = dr["C002"].ToString();
                        param.State = Convert.ToInt16(dr["C003"]);
                        param.StrAddUtcTime = dr["C004"].ToString();
                        param.StrAddLocalTime = dr["C005"].ToString();
                        param.LongAddPaperNum = Convert.ToInt64(dr["C006"]);
                        param.StrAddPaperName = dr["C007"].ToString();
                        param.IntDelete = Convert.ToInt16(dr["C008"]);
                        param.StrDeleteUtcTime = dr["C009"].ToString();
                        param.StrDeleteLocalTime = dr["C010"].ToString();
                        if (!string.IsNullOrEmpty(dr["C011"].ToString()))
                        {
                            param.LongDeletePaperNum = Convert.ToInt64(dr["C011"]);
                        }
                        param.StrDeletePaperName = dr["C012"].ToString();
                        param.StrChangeUtcTime = dr["C013"].ToString();
                        param.StrChangeLocalTime = dr["C014"].ToString();
                        if (!string.IsNullOrEmpty(dr["C015"].ToString()))
                        {
                            param.LongChangePaperNum = Convert.ToInt64(dr["C015"]);
                        }
                        param.StrChangePaperName = dr["C016"].ToString();
                        param.StrDeleteUtcTime = dr["C017"].ToString();
                        param.StrDeleteLocalTime = dr["C018"].ToString();
                        if (!string.IsNullOrEmpty(dr["C019"].ToString()))
                        {
                            param.LongDeletePaperNum = Convert.ToInt64(dr["C019"]);
                        }
                        param.StrDeletePaperName = dr["C020"].ToString();
                        param.IntBindingKw = assparam.IntEnable;
                        optReturn = XMLHelper.SeriallizeObject(param);
                        if (!optReturn.Result)
                        {
                            optReturn.Code = 0;
                            return optReturn;
                        }
                        listReturn.Add(optReturn.Data.ToString());
                    }
                }
                optReturn.Data = listReturn;
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

        private OperationReturn OptUpdate51008(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                if (listParams == null || listParams.Count < 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = "Request param is null or count invalid";
                    return optReturn;
                }
                string rentToken = session.RentInfo.Token;
                string strPaperParam = listParams[0];
                optReturn = XMLHelper.DeserializeObject<List<AssociationTableParam>>(strPaperParam);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                var param = optReturn.Data as List<AssociationTableParam>;
                if (param == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = "Paper Param is null";
                    return optReturn;
                }
                foreach (var associationTableParam in param)
                {
                    string strSql = string.Format("SELECT * FROM T_51_008_{0}  where c001='{1}' ", rentToken, associationTableParam.LongKwConNum);
                    IDbConnection objConn;
                    IDbDataAdapter objAdapter;
                    DbCommandBuilder objCmdBuilder;
                    switch (session.DBType)
                    {
                        case 2:
                            optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                            if (!optReturn.Result)
                            {
                                return optReturn;
                            }
                            objConn = MssqlOperation.GetConnection(session.DBConnectionString);
                            objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                            objCmdBuilder = MssqlOperation.GetCommandBuilder(objAdapter);
                            break;
                        case 3:
                            optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                            if (!optReturn.Result)
                            {
                                return optReturn;
                            }
                            objConn = OracleOperation.GetConnection(session.DBConnectionString);
                            objAdapter = OracleOperation.GetDataAdapter(objConn, strSql);
                            objCmdBuilder = OracleOperation.GetCommandBuilder(objAdapter);
                            break;
                        default:
                            optReturn.Result = false;
                            optReturn.Code = Defines.RET_PARAM_INVALID;
                            optReturn.Message = string.Format("Database type not surpport.\t{0}", session.DBType);
                            return optReturn;
                    }
                    if (objConn == null || objAdapter == null || objCmdBuilder == null)
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_OBJECT_NULL;
                        optReturn.Message = "Db object is null";
                        return optReturn;
                    }

                    objCmdBuilder.ConflictOption = ConflictOption.CompareRowVersion;
                    objCmdBuilder.SetAllValues = false;
                    DataSet objDataSet = new DataSet();
                    objAdapter.Fill(objDataSet);
                    DataRow dr = objDataSet.Tables[0].Rows[0];
                    dr.BeginEdit();
                    dr["c002"] = associationTableParam.LongKwNum;
                    dr["c003"] = associationTableParam.IntEnable;
                    dr.EndEdit();
                    objAdapter.Update(objDataSet);
                    objDataSet.AcceptChanges();
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

        private OperationReturn OptClearKwContent(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                if (listParams == null || listParams.Count < 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = "Request param is null or count invalid";
                    return optReturn;
                }
                string strPaperParam = listParams[0];
                optReturn = XMLHelper.DeserializeObject<string>(strPaperParam);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                var strSql = optReturn.Data as string;
                if (strSql == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = "Paper Param is null";
                    return optReturn;
                }
                IDbConnection objConn;
                IDbDataAdapter objAdapter;
                DbCommandBuilder objCmdBuilder;
                switch (session.DBType)
                {
                    case 2:
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objConn = MssqlOperation.GetConnection(session.DBConnectionString);
                        objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = MssqlOperation.GetCommandBuilder(objAdapter);
                        break;
                    case 3:
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objConn = OracleOperation.GetConnection(session.DBConnectionString);
                        objAdapter = OracleOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = OracleOperation.GetCommandBuilder(objAdapter);
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("Database type not surpport.\t{0}", session.DBType);
                        return optReturn;
                }
                if (objConn == null || objAdapter == null || objCmdBuilder == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = "Db object is null";
                    return optReturn;
                }

                objCmdBuilder.ConflictOption = ConflictOption.CompareRowVersion;
                objCmdBuilder.SetAllValues = false;
                DataSet objDataSet = new DataSet();
                objAdapter.Fill(objDataSet);


                for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = objDataSet.Tables[0].Rows[i];
                    dr.BeginEdit();
                    dr["c002"] = 0;
                    dr["c003"] = 0;
                    dr.EndEdit();
                    objAdapter.Update(objDataSet);
                    objDataSet.AcceptChanges();
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

        private OperationReturn OptUpdateKw(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                if (listParams == null || listParams.Count < 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = "Request param is null or count invalid";
                    return optReturn;
                }
                string rentToken = session.RentInfo.Token;
                string strPaperParam = listParams[0];
                optReturn = XMLHelper.DeserializeObject<KeywordInfoParam>(strPaperParam);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                var param = optReturn.Data as KeywordInfoParam;
                if (param == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = "Paper Param is null";
                    return optReturn;
                }

                string strSql = string.Format("SELECT * FROM T_51_006_{0}  where c001='{1}' ", rentToken, param.LongKwNum);
                IDbConnection objConn;
                IDbDataAdapter objAdapter;
                DbCommandBuilder objCmdBuilder;
                switch (session.DBType)
                {
                    case 2:
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objConn = MssqlOperation.GetConnection(session.DBConnectionString);
                        objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = MssqlOperation.GetCommandBuilder(objAdapter);
                        break;
                    case 3:
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objConn = OracleOperation.GetConnection(session.DBConnectionString);
                        objAdapter = OracleOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = OracleOperation.GetCommandBuilder(objAdapter);
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("Database type not surpport.\t{0}", session.DBType);
                        return optReturn;
                }
                if (objConn == null || objAdapter == null || objCmdBuilder == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = "Db object is null";
                    return optReturn;
                }

                objCmdBuilder.ConflictOption = ConflictOption.CompareRowVersion;
                objCmdBuilder.SetAllValues = false;
                DataSet objDataSet = new DataSet();
                objAdapter.Fill(objDataSet);
                DataRow dr = objDataSet.Tables[0].Rows[0];
                dr.BeginEdit();
                dr["c002"] = param.StrKw;
                dr["c005"] = param.State;
                if (!string.IsNullOrEmpty(param.StrChangeUtcTime))
                {
                    dr["c015"] = param.StrChangeUtcTime;
                }
                if (!string.IsNullOrEmpty(param.StrChangeLocalTime))
                {
                    dr["c016"] = param.StrChangeLocalTime;
                }
                dr["c017"] = param.LongChangePaperNum;
                if (!string.IsNullOrEmpty(param.StrChangePaperName))
                {
                    dr["c018"] = param.StrChangePaperName;
                }
                dr.EndEdit();
                objAdapter.Update(objDataSet);
                objDataSet.AcceptChanges();
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

        private OperationReturn OptDeleteKw(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                if (listParams == null || listParams.Count < 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = "Request param is null or count invalid";
                    return optReturn;
                }
                string rentToken = session.RentInfo.Token;
                string strPaperParam = listParams[0];
                optReturn = XMLHelper.DeserializeObject<KeywordInfoParam>(strPaperParam);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                var param = optReturn.Data as KeywordInfoParam;
                if (param == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = "Paper Param is null";
                    return optReturn;
                }

                string strSql = string.Format("SELECT * FROM T_51_006_{0}  where c001='{1}' ", rentToken, param.LongKwNum);
                IDbConnection objConn;
                IDbDataAdapter objAdapter;
                DbCommandBuilder objCmdBuilder;
                switch (session.DBType)
                {
                    case 2:
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objConn = MssqlOperation.GetConnection(session.DBConnectionString);
                        objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = MssqlOperation.GetCommandBuilder(objAdapter);
                        break;
                    case 3:
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objConn = OracleOperation.GetConnection(session.DBConnectionString);
                        objAdapter = OracleOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = OracleOperation.GetCommandBuilder(objAdapter);
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("Database type not surpport.\t{0}", session.DBType);
                        return optReturn;
                }
                if (objConn == null || objAdapter == null || objCmdBuilder == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = "Db object is null";
                    return optReturn;
                }

                objCmdBuilder.ConflictOption = ConflictOption.CompareRowVersion;
                objCmdBuilder.SetAllValues = false;
                DataSet objDataSet = new DataSet();
                objAdapter.Fill(objDataSet);
                DataRow dr = objDataSet.Tables[0].Rows[0];
                dr.BeginEdit();
                dr["c010"] = param.IntDelete;
                dr["c005"] = param.State;
                if (!string.IsNullOrEmpty(param.StrDeleteUtcTime))
                {
                    dr["c011"] = param.StrDeleteUtcTime;
                }
                if (!string.IsNullOrEmpty(param.StrDeleteLocalTime))
                {
                    dr["c012"] = param.StrDeleteLocalTime;
                }
                dr["c013"] = param.LongDeletePaperNum;
                if (!string.IsNullOrEmpty(param.StrDeletePaperName))
                {
                    dr["c014"] = param.StrDeletePaperName;
                }
                if (!string.IsNullOrEmpty(param.StrRestoreUtcTime))
                {
                    dr["c019"] = param.StrRestoreUtcTime;
                }
                if (!string.IsNullOrEmpty(param.StrRestoreLocalTime))
                {
                    dr["c020"] = param.StrRestoreLocalTime;
                }
                dr["c021"] = param.LongRestorePaperNum;
                if (!string.IsNullOrEmpty(param.StrRestorePaperName))
                {
                    dr["c022"] = param.StrRestorePaperName;
                }
                dr.EndEdit();
                objAdapter.Update(objDataSet);
                objDataSet.AcceptChanges();
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

        public WebReturn UmpTaskOperation(WebRequest webRequest)
        {
            var webReturn = new WebReturn();
            SessionInfo session = webRequest.Session;
            if (session == null)
            {
                webReturn.Result = false;
                webReturn.Code = Defines.RET_PARAM_INVALID;
                webReturn.Message = "SessionInfo is null";
                return webReturn;
            }
            webReturn.Session = session;
            try
            {
                OperationReturn optReturn;
                DatabaseInfo dbInfo = session.DatabaseInfo;
                if (dbInfo != null)
                {
                    dbInfo.RealPassword = DecryptFromClient(dbInfo.Password);
                    session.DBConnectionString = dbInfo.GetConnectionString();
                }
                switch (webRequest.Code)
                {

                    case (int)RequestCode.WSGetSerialID:
                        optReturn = OptGetSerialID(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.Data = optReturn.Data.ToString();
                        break;
                    case (int)S5102Codes.OptAddKwContent:
                        optReturn = OptAddKwContent(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        break;
                    case (int)S5102Codes.OptSelectKwContent:
                        optReturn = OptSelectKwContent(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S5102Codes.OptUpdateKwContent:
                        optReturn = OptUpdateKwContent(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        break;
                    case (int)S5102Codes.OptDeleteKwContent:
                        optReturn = OptDeleteKwContent(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        break;
                    case (int)S5102Codes.OptAddKeyword:
                        optReturn = OptAddKeyword(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        break;
                    case (int)S5102Codes.OptSelectKeyword:
                        optReturn = OptSelectKeyword(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S5102Codes.OptSelectAssignKwContent:
                        optReturn = OptSelectAssignKwContent(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S5102Codes.OptUpdate51008:
                        optReturn = OptUpdate51008(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        break;
                    case (int)S5102Codes.OptUpdateKw:
                        optReturn = OptUpdateKw(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        break;
                    case (int)S5102Codes.OptDeleteKw:
                        optReturn = OptDeleteKw(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        break;
                    case (int)S5102Codes.OptClearKwContent:
                        optReturn = OptClearKwContent(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        break;
                    default:
                        webReturn.Result = false;
                        webReturn.Code = Defines.RET_PARAM_INVALID;
                        webReturn.Message = string.Format("WebCodes invalid.\t{0}", webRequest.Code);
                        return webReturn;
                }
                webReturn.Result = true;
                webReturn.Code = 0;
                webReturn.Message = optReturn.Message;
                return webReturn;
            }
            catch (Exception ex)
            {
                webReturn.Result = false;
                webReturn.Code = Defines.RET_FAIL;
                webReturn.Message = ex.Message;
                return webReturn;
            }
        }

        private string DecryptFromClient(string strSource)
        {
            try
            {
                return ServerAESEncryption.DecryptString(strSource, EncryptionMode.AES256V04Hex);
            }
            catch
            {
                return strSource;
            }
        }
    }
}
