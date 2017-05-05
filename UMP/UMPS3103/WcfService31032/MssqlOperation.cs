﻿using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;


namespace WcfService31032
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

        public static Service02Return GetDataSet(string strConn, string strSql)
        {
            Service02Return optReturn = new Service02Return();
            optReturn.ReturnValueBool = true;
            optReturn.ErrorFlag = "T";
            SqlConnection sqlConnection = new SqlConnection(strConn);
            SqlDataAdapter sqlAdapter = new SqlDataAdapter(strSql, sqlConnection);
            //sqlAdapter.SelectCommand.CommandTimeout = 30;
            DataSet objDataSet = new DataSet();
            try
            {
                sqlAdapter.Fill(objDataSet);
                optReturn.ReturnValueDataSet = objDataSet;
            }
            catch (Exception ex)
            {
                optReturn.ReturnValueBool = false;
                optReturn.ErrorFlag = "F";
                optReturn.ErrorMessage = ex.Message.ToString();
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

        public static Service02Return ExecuteStoredProcedure(string strConn, string procedureName, DbParameter[] parameters)
        {
            Service02Return optReturn = new Service02Return();
            optReturn.ReturnValueBool = true;
            optReturn.ErrorFlag = "T";
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
                optReturn.ReturnValueBool = false;
                optReturn.ErrorFlag = "F";
                optReturn.ErrorMessage = ex.Message.ToString();
                if (count > 2)
                {
                    string strErrorNumber = parameters[count - 2].Value.ToString();
                    int intErrorNumber;
                    if (int.TryParse(strErrorNumber, out intErrorNumber))
                    {
                        optReturn.ErrorMessage += string.Format("{0}\t{1}", intErrorNumber, parameters[count - 1].Value);
                    }
                }
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

        public static Service02Return ExecuteSql(string strConn, string strSql)
        {
            Service02Return optReturn = new Service02Return();
            optReturn.ReturnValueBool = true;
            optReturn.ErrorFlag = "T";
            SqlConnection sqlConnection = new SqlConnection(strConn);
            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.Connection = sqlConnection;
            sqlCmd.CommandType = CommandType.Text;
            sqlCmd.CommandText = strSql;
            try
            {
                sqlConnection.Open();
                int count = sqlCmd.ExecuteNonQuery();
                optReturn.ReturnValueInt = count;
            }
            catch (Exception ex)
            {
                optReturn.ReturnValueBool = false;
                optReturn.ErrorFlag = "F";
                optReturn.ErrorMessage = ex.Message;
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

        public static Service02Return GetRecordCount(string strConn, string strSql)
        {
            Service02Return optReturn = new Service02Return();
            optReturn.ReturnValueBool = true;
            optReturn.ErrorFlag = "T";
            SqlConnection sqlConnection = new SqlConnection(strConn);
            SqlCommand sqlCmd = new SqlCommand();
            sqlCmd.Connection = sqlConnection;
            sqlCmd.CommandType = CommandType.Text;
            sqlCmd.CommandText = strSql;
            try
            {
                sqlConnection.Open();
                int count = Convert.ToInt32(sqlCmd.ExecuteScalar());
                optReturn.ReturnValueInt = count;
            }
            catch (Exception ex)
            {
                optReturn.ReturnValueBool = false;
                optReturn.ErrorFlag = "F";
                optReturn.ErrorMessage = ex.Message;
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