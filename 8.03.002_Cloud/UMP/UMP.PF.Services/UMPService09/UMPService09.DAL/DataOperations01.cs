using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;
using System.Data.SqlClient;
using System.Data;


namespace UMPService09.DAL
{
    public class DataOperations01
    {
        /// <summary>
        /// 执行SQL语句，返回受影响的行数
        /// </summary>
        /// <param name="AIntDBType">数据库类型。2：MS SQL；3：Oracle</param>
        /// <param name="AStrDBConnectProfile">数据库连接字符串</param>
        /// <param name="AStrSQLString">需要执行的SQL语句</param>
        /// <returns></returns>
        public DatabaseOperation01Return ExecuteDynamicSQL(int AIntDBType, string AStrDBConnectProfile, string AStrSQLString)
        {
            DatabaseOperation01Return LClassReturn = new DatabaseOperation01Return();
            int LIntExecureReturn = 0;

            SqlConnection LSqlConnection = null;
            SqlCommand LSqlCommand = null;
            OracleConnection LOracleConnection = null;
            OracleCommand LOracleCommand = null;

            try
            {
                if (AIntDBType == 2)
                {
                    LSqlConnection = new SqlConnection(AStrDBConnectProfile);
                    LSqlConnection.Open();
                    LSqlCommand = new SqlCommand(AStrSQLString, LSqlConnection);
                    LIntExecureReturn = LSqlCommand.ExecuteNonQuery();
                }
                if (AIntDBType == 3)
                {
                    LOracleConnection = new OracleConnection(AStrDBConnectProfile);
                    LOracleConnection.Open();
                    LOracleCommand = new OracleCommand(AStrSQLString, LOracleConnection);
                    LIntExecureReturn = LOracleCommand.ExecuteNonQuery();
                }
                LClassReturn.StrReturn = LIntExecureReturn.ToString();
            }
            catch (Exception ex)
            {
                LClassReturn.BoolReturn = false;
                LClassReturn.StrReturn = "DataOperations01.ExecuteDynamicSQL()\n" + ex.ToString();
            }
            finally
            {
                if (LSqlCommand != null) { LSqlCommand.Dispose(); LSqlCommand = null; }
                if (LSqlConnection != null)
                {
                    if (LSqlConnection.State == System.Data.ConnectionState.Open) { LSqlConnection.Close(); }
                    LSqlConnection.Dispose();
                }
                if (LOracleCommand != null) { LOracleCommand.Dispose(); LOracleCommand = null; }
                if (LOracleConnection != null)
                {
                    if (LOracleConnection.State == ConnectionState.Open) { LOracleConnection.Close(); }
                    LOracleConnection.Dispose(); LOracleConnection = null;
                }
            }

            return LClassReturn;
        }

        /// <summary>
        /// 根据SQL语句返回查询结果集
        /// </summary>
        /// <param name="AIntDBType">数据库类型。2：MS SQL；3：Oracle</param>
        /// <param name="AStrDBConnectProfile">数据库连接字符串</param>
        /// <param name="AStrSQLString">SELECT 语句</param>
        /// <returns></returns>
        public DatabaseOperation01Return SelectDataByDynamicSQL(int AIntDBType, string AStrDBConnectProfile, string AStrSQLString)
        {
            DatabaseOperation01Return LClassReturn = new DatabaseOperation01Return();

            SqlConnection LSqlConnection = null;
            OracleConnection LOracleConnection = null;

            try
            {
                if (AIntDBType == 2)
                {
                    LSqlConnection = new SqlConnection(AStrDBConnectProfile);
                    LSqlConnection.Open();
                    SqlDataAdapter LSqlDataAdapter = new SqlDataAdapter(AStrSQLString, LSqlConnection);
                    LClassReturn.StrReturn = LSqlDataAdapter.Fill(LClassReturn.DataSetReturn).ToString();
                    LSqlDataAdapter.Dispose();
                }
                if (AIntDBType == 3)
                {
                    LOracleConnection = new OracleConnection(AStrDBConnectProfile);
                    LOracleConnection.Open();
                    OracleDataAdapter LOracleDataAdapter = new OracleDataAdapter(AStrSQLString, LOracleConnection);
                    LClassReturn.StrReturn = LOracleDataAdapter.Fill(LClassReturn.DataSetReturn).ToString();
                    LOracleDataAdapter.Dispose();
                }
            }
            catch (Exception ex)
            {
                LClassReturn.BoolReturn = false;
                LClassReturn.StrReturn = "DataOperations01.SelectDataByDynamicSQL()\n" + ex.ToString();
            }
            finally
            {
                if (LSqlConnection != null)
                {
                    if (LSqlConnection.State == System.Data.ConnectionState.Open) { LSqlConnection.Close(); }
                    LSqlConnection.Dispose();
                }
                if (LOracleConnection != null)
                {
                    if (LOracleConnection.State == ConnectionState.Open) { LOracleConnection.Close(); }
                    LOracleConnection.Dispose(); LOracleConnection = null;
                }
            }
            return LClassReturn;
        }

        /// <summary>
        /// 获取流水号
        /// </summary>
        /// <param name="AIntDBType">数据库类型。2：MS SQL；3：Oracle</param>
        /// <param name="AStrDBConnectProfile">数据库连接字符串</param>
        /// <param name="AIntModuleID">模块编码 11 - 99</param>
        /// <param name="AIntSerialType">流水号类型编码 100 ～ 920</param>
        /// <param name="AStrRent5">租户编码（表5位）</param>
        /// <param name="AStrTime">流水号时间（yyyyMMddHHmmss）</param>
        /// <returns>19位长度的字符串，DatabaseOperation01Return.StrReturn</returns>
        public DatabaseOperation01Return GetSerialNumberByProcedure(int AIntDBType, string AStrDBConnectProfile, int AIntModuleID, int AIntSerialType, string AStrRentID5, string AStrTime)
        {
            DatabaseOperation01Return LClassReturn = new DatabaseOperation01Return();

            SqlConnection LSqlConnection = null;
            SqlCommand LSqlCommand = null;
            OracleConnection LOracleConnection = null;
            OracleCommand LOracleCommand = null;

            try
            {
                if (AIntDBType == 2)
                {
                    #region MS SQL 数据库
                    LSqlConnection = new SqlConnection(AStrDBConnectProfile);
                    LSqlConnection.Open();
                    LSqlCommand = new SqlCommand();
                    LSqlCommand.Connection = LSqlConnection;
                    LSqlCommand.CommandType = CommandType.StoredProcedure;
                    LSqlCommand.CommandText = "P_00_001";
                    SqlParameter[] LSqlParameter = 
                    {
                        new SqlParameter("@AInParam01", SqlDbType.VarChar, 2),            //0
                        new SqlParameter("@AInParam02", SqlDbType.VarChar, 3),            //1
                        new SqlParameter("@AInParam03", SqlDbType.VarChar, 5),            //2
                        new SqlParameter("@AInParam04", SqlDbType.VarChar, 20),           //3
                        new SqlParameter("@AOutParam01", SqlDbType.VarChar, 20),          //4
                        new SqlParameter("@AOutErrorNumber", SqlDbType.Int),                  //5
                        new SqlParameter("@AOutErrorString", SqlDbType.NVarChar, 2000)        //6
                    };
                    LSqlParameter[0].Value = AIntModuleID.ToString();
                    LSqlParameter[1].Value = AIntSerialType.ToString();
                    LSqlParameter[2].Value = AStrRentID5;
                    LSqlParameter[3].Value = AStrTime;

                    LSqlParameter[4].Direction = ParameterDirection.Output;
                    LSqlParameter[5].Direction = ParameterDirection.Output;
                    LSqlParameter[6].Direction = ParameterDirection.Output;

                    foreach (SqlParameter LSqlParameterSingle in LSqlParameter) { LSqlCommand.Parameters.Add(LSqlParameterSingle); }

                    LSqlCommand.ExecuteNonQuery();
                    if (LSqlParameter[5].Value.ToString() != "0")
                    {
                        LClassReturn.BoolReturn = false;
                        LClassReturn.StrReturn = LSqlParameter[6].Value.ToString();
                    }
                    else
                    {
                        LClassReturn.StrReturn = LSqlParameter[4].Value.ToString();
                    }
                    #endregion
                }
                if (AIntDBType == 3)
                {
                    #region Oracle 数据库
                    LOracleConnection = new OracleConnection(AStrDBConnectProfile);
                    LOracleConnection.Open();
                    LOracleCommand = new OracleCommand();
                    LOracleCommand.Connection = LOracleConnection;
                    LOracleCommand.CommandType = CommandType.StoredProcedure;
                    LOracleCommand.CommandText = "P_00_001";
                    OracleParameter[] LOracleParameter =
                    {
                        new OracleParameter("AInParam01", OracleDbType.Varchar2, 2),            //0
                        new OracleParameter("AInParam02", OracleDbType.Varchar2, 3),            //1
                        new OracleParameter("AInParam03", OracleDbType.Varchar2, 5),            //2
                        new OracleParameter("AInParam04", OracleDbType.Varchar2, 20),           //3
                        new OracleParameter("AOutParam01", OracleDbType.Varchar2, 20),          //4
                        new OracleParameter("ErrorNumber", OracleDbType.Int32),                 //5
                        new OracleParameter("ErrorString", OracleDbType.NVarchar2, 2000)        //6
                    };

                    LOracleParameter[0].Value = AIntModuleID.ToString();
                    LOracleParameter[1].Value = AIntSerialType.ToString();
                    LOracleParameter[2].Value = AStrRentID5;
                    LOracleParameter[3].Value = AStrTime;

                    LOracleParameter[4].Direction = ParameterDirection.Output;
                    LOracleParameter[5].Direction = ParameterDirection.Output;
                    LOracleParameter[6].Direction = ParameterDirection.Output;

                    foreach (OracleParameter LOracleParameterSingle in LOracleParameter) { LOracleCommand.Parameters.Add(LOracleParameterSingle); }

                    LOracleCommand.ExecuteNonQuery();
                    if (LOracleParameter[5].Value.ToString() != "0")
                    {
                        LClassReturn.BoolReturn = false;
                        LClassReturn.StrReturn = LOracleParameter[6].Value.ToString();
                    }
                    else
                    {
                        LClassReturn.StrReturn = LOracleParameter[4].Value.ToString();
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                LClassReturn.BoolReturn = false;
                LClassReturn.StrReturn = "DataOperations01.GetSerialNumberByProcedure()\n" + ex.ToString();
            }
            finally
            {
                if (LSqlCommand != null) { LSqlCommand.Dispose(); LSqlCommand = null; }
                if (LSqlConnection != null)
                {
                    if (LSqlConnection.State == System.Data.ConnectionState.Open) { LSqlConnection.Close(); }
                    LSqlConnection.Dispose();
                }
                if (LOracleCommand != null) { LOracleCommand.Dispose(); LOracleCommand = null; }
                if (LOracleConnection != null)
                {
                    if (LOracleConnection.State == ConnectionState.Open) { LOracleConnection.Close(); }
                    LOracleConnection.Dispose(); LOracleConnection = null;
                }
            }

            return LClassReturn;
        }
    }

    public class DatabaseOperation01Return
    {
        public bool BoolReturn = true;
        public string StrReturn = string.Empty;
        public DataSet DataSetReturn = new DataSet();
    }
}
