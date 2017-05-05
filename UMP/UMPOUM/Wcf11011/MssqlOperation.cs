using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using Oracle.DataAccess.Client;
using VoiceCyber.Common;

namespace Wcf11011
{
    public class MssqlOperation
    {
        public static SqlConnection GetConnection(string strConn)
        {
            return new SqlConnection(strConn);
        }

        public static SqlDataAdapter GetDataAdapter(IDbConnection objConn, string strSql)
        {
            return new SqlDataAdapter(strSql, objConn as SqlConnection);
        }

        public static SqlCommandBuilder GetCommandBuilder(IDbDataAdapter objAdapter)
        {
            return new SqlCommandBuilder(objAdapter as SqlDataAdapter);
        }

        public static OperationReturn GetDataSet(string strConn, string strSql)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            SqlConnection sqlConnection = new SqlConnection(strConn);
            SqlDataAdapter sqlAdapter = new SqlDataAdapter(strSql, sqlConnection);
            DataSet objDataSet = new DataSet();
            try
            {
                sqlAdapter.Fill(objDataSet);
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
                if (sqlConnection.State == ConnectionState.Open)
                {
                    sqlConnection.Close();
                }
                sqlConnection.Dispose();
            }
            return optReturn;
        }

        public static OperationReturn ExecuteStoredProcedure(string strConn, string procedureName, DbParameter[] parameters)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            int count = parameters.Length;
            SqlConnection sqlConnection = new SqlConnection(strConn);
            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.Connection = sqlConnection;
            sqlCmd.CommandType = CommandType.StoredProcedure;
            sqlCmd.CommandText = procedureName;
            for (int i = 0; i < parameters.Length; i++)
            {
                sqlCmd.Parameters.Add(parameters[i]);
            }
            try
            {
                sqlConnection.Open();
                sqlCmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_DBACCESS_FAIL;
                optReturn.Message = ex.Message;
            }
            finally
            {
                if (sqlConnection.State == ConnectionState.Open)
                {
                    sqlConnection.Close();
                }
                sqlConnection.Dispose();
            }
            return optReturn;
        }

        public static DbParameter GetDbParameter(string name, MssqlDataType dataType, int length)
        {
            switch (dataType)
            {
                case MssqlDataType.Varchar:
                    return new SqlParameter(name, SqlDbType.VarChar, length);
                case MssqlDataType.NVarchar:
                    return new SqlParameter(name, SqlDbType.NVarChar, length);
                case MssqlDataType.Char:
                    return new SqlParameter(name, SqlDbType.Char);
                case MssqlDataType.Bigint:
                    return new SqlParameter(name, SqlDbType.BigInt, length);
                case MssqlDataType.Int:
                    return new SqlParameter(name, SqlDbType.Int, length);
            }
            return null;
        }

        public static OperationReturn ExecuteSql(string strConn, string strSql)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            SqlConnection sqlConnection = new SqlConnection(strConn);
            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.Connection = sqlConnection;
            sqlCmd.CommandType = CommandType.Text;
            sqlCmd.CommandText = strSql;
            try
            {
                sqlConnection.Open();
                int count = sqlCmd.ExecuteNonQuery();
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
                if (sqlConnection.State == ConnectionState.Open)
                {
                    sqlConnection.Close();
                }
                sqlConnection.Dispose();
            }
            return optReturn;
        }

        public static OperationReturn GetRecordCount(string strConn, string strSql)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            SqlConnection sqlConnection = new SqlConnection(strConn);
            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.Connection = sqlConnection;
            sqlCmd.CommandType = CommandType.Text;
            sqlCmd.CommandText = strSql;
            try
            {
                sqlConnection.Open();
                int count = Convert.ToInt32(sqlCmd.ExecuteScalar());
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
                if (sqlConnection.State == ConnectionState.Open)
                {
                    sqlConnection.Close();
                }
                sqlConnection.Dispose();
            }
            return optReturn;
        }
    }

    public enum MssqlDataType
    {
        Varchar,
        NVarchar,
        Char,
        Bigint,
        Int
    }
}