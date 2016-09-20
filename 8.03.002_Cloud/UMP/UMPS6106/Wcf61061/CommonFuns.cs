using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;

namespace Wcf61061
{
    public class CommonFuns
    {
        /// <summary>
        /// 检查表是否存在
        /// </summary>
        /// <param name="session"></param>
        /// <param name="lstParams"></param>
        /// lstParams[0] : TableName
        /// <returns></returns>
        public static OperationReturn CheckTableExists(SessionInfo session, string strTableName)
        {
            OperationReturn optReturn = new OperationReturn();
            try
            {
                string rentToken = session.RentInfo.Token;
                string strSql = string.Empty;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("use {0}; SELECT  * FROM dbo.SysObjects WHERE ID = object_id(N'[{1}]') AND OBJECTPROPERTY(ID, 'IsTable') = 1", session.DatabaseInfo.DBName, strTableName);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        break;
                    case 3:
                        strSql = string.Format("select *   from   user_tables    where   table_name ='{0}'  ", strTableName);
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        break;
                }
                optReturn.Message = strSql;
                if (optReturn.Data == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_FAIL;
                    return optReturn;
                }
                DataSet ds = optReturn.Data as DataSet;
                if (ds.Tables.Count <= 0 || ds.Tables[0].Rows.Count <= 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_FAIL;
                    return optReturn;
                }

                optReturn.Result = true;
                optReturn.Code = Defines.RET_SUCCESS;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
                optReturn.Exception = ex;
            }
            return optReturn;
        }

        public static OperationReturn DeleteTempDataByID(SessionInfo session, string strKey)
        {
            OperationReturn optReturn = new OperationReturn();
            try
            {
                string rentToken = session.RentInfo.Token;
                string strSql = string.Empty;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("delete from T_00_901 where C001 = {0}", strKey);
                        optReturn = MssqlOperation.ExecuteSql(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        break;
                    case 3:
                        strSql = string.Format("delete from T_00_901 where C001 = {0}", strKey);
                        optReturn = OracleOperation.ExecuteSql(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        break;
                }
                optReturn.Message = strSql;
                if (optReturn.Data == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_FAIL;
                    return optReturn;
                }
                optReturn.Result = true;
                optReturn.Code = Defines.RET_SUCCESS;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
                optReturn.Exception = ex;
            }
            return optReturn;
        }
    }
}