//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    c83b1c9f-ce3f-4c33-91dc-722806434038
//        CLR Version:              4.0.30319.18063
//        Name:                     MssqlOperation
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                UMPLanguageManager
//        File Name:                MssqlOperation
//
//        created by Charley at 2015/6/4 16:07:32
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using VoiceCyber.Common;

namespace UMPLanguageManager
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
                case MssqlDataType.Int:
                    return new SqlParameter(name, SqlDbType.Int);
                case MssqlDataType.Bigint:
                    return new SqlParameter(name, SqlDbType.BigInt, length);
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
                optReturn.IntValue = count;
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
        Int,
        Bigint
    }
}
