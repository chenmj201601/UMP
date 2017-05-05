using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using VoiceCyber.Common;

namespace Wcf31041
{
    public class MssqlOperation
    {
        /// <summary>
        /// 创建一个DBConnection对象
        /// </summary>
        /// <param name="strConn">连接字符串</param>
        /// <returns></returns>
        public static SqlConnection GetConnection(string strConn)
        {
            return new SqlConnection(strConn);
        }

        /// <summary>
        /// 创建一个数据库适配器
        /// </summary>
        /// <param name="objConn">DBConnection</param>
        /// <param name="strSql">查询语句</param>
        /// <returns></returns>
        public static SqlDataAdapter GetDataAdapter(IDbConnection objConn, string strSql)
        {
            return new SqlDataAdapter(strSql, objConn as SqlConnection);
        }

        /// <summary>
        /// 创建一个CommandBuilder
        /// </summary>
        /// <param name="objAdapter">DataAdapter</param>
        /// <returns></returns>
        public static SqlCommandBuilder GetCommandBuilder(IDbDataAdapter objAdapter)
        {
            return new SqlCommandBuilder(objAdapter as SqlDataAdapter);
        }

        /// <summary>
        /// 测试数据库是否连接成功
        /// </summary>
        /// <param name="strConn">连接字符串</param>
        /// <returns></returns>
        public static OperationReturn TestDBConnection(string strConn)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            SqlConnection sqlConnection = new SqlConnection(strConn);
            try
            {
                sqlConnection.Open();
                optReturn.Message = sqlConnection.Database;
                optReturn.Data = sqlConnection.ConnectionTimeout;
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

        /// <summary>
        /// 获取数据集
        /// </summary>
        /// <param name="strConn">连接字符串</param>
        /// <param name="strSql">查询语句</param>
        /// <returns></returns>
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

        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="strConn">连接字符串</param>
        /// <param name="procedureName">存储过程名称</param>
        /// <param name="parameters">参数列表</param>
        /// <returns></returns>
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

        /// <summary>
        /// 执行存储过程，获取数据集
        /// </summary>
        /// <param name="strConn"></param>
        /// <param name="procedureName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static OperationReturn GetDataSetFromStoredProcedure(string strConn, string procedureName,DbParameter[] parameters)
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
            SqlDataAdapter sqlAdapter = new SqlDataAdapter(sqlCmd);
            DataSet objDataSet = new DataSet();
            for (int i = 0; i < parameters.Length; i++)
            {
                sqlCmd.Parameters.Add(parameters[i]);
            }
            try
            {
                sqlConnection.Open();
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

        /// <summary>
        /// 根据指定的数据类型创建DBCommand的参数
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="dataType">数据类型</param>
        /// <param name="length">长度</param>
        /// <returns></returns>
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
                    return new SqlParameter(name, SqlDbType.Char, length);
            }
            return null;
        }

        /// <summary>
        /// 执行指定的Sql语句
        /// </summary>
        /// <param name="strConn">连接字符串</param>
        /// <param name="strSql">Sql语句</param>
        /// <returns></returns>
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

        /// <summary>
        /// 获取记录条数
        /// </summary>
        /// <param name="strConn">连接字符串</param>
        /// <param name="strSql">查询语句</param>
        /// <returns>OperationReturn的IntValue为记录条数</returns>
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
        /// <summary>
        /// 可变长字符型
        /// </summary>
        Varchar,
        /// <summary>
        /// 可变长字符型
        /// </summary>
        NVarchar,
        /// <summary>
        /// 固定大小字符型
        /// </summary>
        Char,
        /// <summary>
        /// 整型
        /// </summary>
        Int,
        /// <summary>
        /// 长整型
        /// </summary>
        Bigint
    }
}