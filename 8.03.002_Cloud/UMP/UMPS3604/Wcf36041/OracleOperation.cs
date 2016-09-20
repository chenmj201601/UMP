using System;
using System.Data;
using System.Data.Common;
using Oracle.DataAccess.Client;
using VoiceCyber.Common;
namespace Wcf36041
{
    public class OracleOperation
    {
        public static OracleConnection GetConnection(string strConn)
        {
            return new OracleConnection(strConn);
        }

        public static OracleDataAdapter GetDataAdapter(IDbConnection objConn, string strSql)
        {
            return new OracleDataAdapter(strSql, objConn as OracleConnection);
        }

        public static OracleCommandBuilder GetCommandBuilder(IDbDataAdapter objAdapter)
        {
            return new OracleCommandBuilder(objAdapter as OracleDataAdapter);
        }

        public static OperationReturn GetDataSet(string strConn, string strSql)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            OracleConnection orclConnection = new OracleConnection(strConn);
            OracleDataAdapter orclAdapter = new OracleDataAdapter(strSql, orclConnection);
            DataSet objDataSet = new DataSet();
            try
            {
                orclAdapter.Fill(objDataSet);
                optReturn.Data = objDataSet;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_DBACCESS_FAIL;
                optReturn.Message = ex.Message;
            }
            finally
            {
                if (orclConnection.State == ConnectionState.Open)
                {
                    orclConnection.Close();
                }
                orclConnection.Dispose();
            }
            return optReturn;
        }

        public static OperationReturn ExecuteStoredProcedure(string strConn, string procedureName, DbParameter[] parameters)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            int count = parameters.Length;
            OracleConnection orclConnection = new OracleConnection(strConn);
            OracleCommand orclCmd = new OracleCommand();
            orclCmd.Connection = orclConnection;
            orclCmd.CommandType = CommandType.StoredProcedure;
            orclCmd.CommandText = procedureName;
            for (int i = 0; i < parameters.Length; i++)
            {
                orclCmd.Parameters.Add(parameters[i]);
            }
            try
            {
                orclConnection.Open();
                orclCmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_DBACCESS_FAIL;
                optReturn.Message = ex.Message;
                if (count > 2)
                {
                    string strErrorNumber = parameters[count - 2].Value.ToString();
                    int intErrorNumber;
                    if (int.TryParse(strErrorNumber, out intErrorNumber))
                    {
                        optReturn.Message = string.Format("{0}\t{1}", intErrorNumber, parameters[count - 1].Value);
                    }
                }
            }
            finally
            {
                if (orclConnection.State == ConnectionState.Open)
                {
                    orclConnection.Close();
                }
                orclConnection.Dispose();
            }
            return optReturn;
        }

        public static DbParameter GetDbParameter(string name, OracleDataType dataType, int length)
        {
            switch (dataType)
            {
                case OracleDataType.Varchar2:
                    return new OracleParameter(name, OracleDbType.Varchar2, length);
                case OracleDataType.Nvarchar2:
                    return new OracleParameter(name, OracleDbType.NVarchar2, length);
                case OracleDataType.Int32:
                    return new OracleParameter(name, OracleDbType.Int32);
                case OracleDataType.Char:
                    return new OracleParameter(name, OracleDbType.Char, length);
                case OracleDataType.Date:
                    return new OracleParameter(name, OracleDbType.Date);
            }
            return null;
        }

        public static OperationReturn ExecuteSql(string strConn, string strSql)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            OracleConnection orclConnection = new OracleConnection(strConn);
            OracleCommand orclCmd = new OracleCommand();
            orclCmd.Connection = orclConnection;
            orclCmd.CommandType = CommandType.Text;
            orclCmd.CommandText = strSql;
            try
            {
                orclConnection.Open();
                int count = orclCmd.ExecuteNonQuery();
                optReturn.Data = count;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_DBACCESS_FAIL;
                optReturn.Message = ex.Message;
            }
            finally
            {
                if (orclConnection.State == ConnectionState.Open)
                {
                    orclConnection.Close();
                }
                orclConnection.Dispose();
            }
            return optReturn;
        }

        public static OperationReturn GetRecordCount(string strConn, string strSql)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            OracleConnection orclConnection = new OracleConnection(strConn);
            OracleCommand orclCmd = new OracleCommand();
            orclCmd.Connection = orclConnection;
            orclCmd.CommandType = CommandType.Text;
            orclCmd.CommandText = strSql;
            try
            {
                orclConnection.Open();
                int count = Convert.ToInt32(orclCmd.ExecuteScalar());
                optReturn.Data = count;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_DBACCESS_FAIL;
                optReturn.Message = ex.Message;
            }
            finally
            {
                if (orclConnection.State == ConnectionState.Open)
                {
                    orclConnection.Close();
                }
                orclConnection.Dispose();
            }
            return optReturn;
        }
    }

    public enum OracleDataType
    {
        Varchar2,
        Nvarchar2,
        Int32,
        Char,
        Date
    }
}