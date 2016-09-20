//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    d1746e0b-26f7-4ca5-b615-8e504b51d9bc
//        CLR Version:              4.0.30319.18063
//        Name:                     DBAccessHelper
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Common
//        File Name:                DBAccessHelper
//
//        created by Charley at 2014/3/23 9:46:28
//        http://www.voicecyber.com 
//
//======================================================================
using System;
using System.Collections.Generic;
using System.Data;

namespace VoiceCyber.Common
{
    /// <summary>
    /// 数据库访问帮助类
    /// </summary>
    public class DBAccessHelper
    {
        #region 公共静态方法
        /// <summary>
        /// 静态方法，获取数据集
        /// </summary>
        /// <param name="strConn">数据库连接字符串</param>
        /// <param name="strSql">Sql 命令</param>
        /// <returns></returns>
        public static OperationReturn GetDataSet(string strConn, string strSql)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = Defines.RET_SUCCESS;
            int dbType = GetDBType(strConn);
            if (dbType == Defines.VCT_DBTYPE_UNKNOWN)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_NOT_EXIST;
                optReturn.Message = string.Format("Unkown database type\t{0}", dbType);
                return optReturn;
            }
            optReturn = GetConnection(dbType, strConn);
            if (!optReturn.Result)
            {
                return optReturn;
            }
            IDbConnection objConn = optReturn.Data as IDbConnection;
            if (objConn == null)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_OBJECT_NULL;
                optReturn.Message = string.Format("Connection is null");
                return optReturn;
            }
            optReturn = GetDataAdapter(dbType, objConn, strSql);
            if (!optReturn.Result)
            {
                return optReturn;
            }
            IDataAdapter objAdapter = optReturn.Data as IDataAdapter;
            if (objAdapter == null)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_OBJECT_NULL;
                optReturn.Message = string.Format("DataAdapter is null");
                return optReturn;
            }
            DataSet objDS = new DataSet();
            try
            {
                objConn.Open();
                objAdapter.Fill(objDS);
                optReturn.Data = objDS;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
                optReturn.Exception = ex;
            }
            finally
            {
                CloseConnection(objConn);
            }
            return optReturn;
        }
        /// <summary>
        /// 静态方法，执行Sql命令
        /// </summary>
        /// <param name="strConn">数据库连接字符串</param>
        /// <param name="strSql">Sql命令</param>
        /// <returns></returns>
        public static OperationReturn ExecuteCommand(string strConn, string strSql)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = Defines.RET_SUCCESS;
            int dbType = GetDBType(strConn);
            if (dbType == Defines.VCT_DBTYPE_UNKNOWN)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_NOT_EXIST;
                optReturn.Message = string.Format("Unkown database type\t{0}", dbType);
                return optReturn;
            }
            optReturn = GetConnection(dbType, strConn);
            if (!optReturn.Result)
            {
                return optReturn;
            }
            IDbConnection objConn = optReturn.Data as IDbConnection;
            if (objConn == null)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_OBJECT_NULL;
                optReturn.Message = string.Format("Connection is null");
                return optReturn;
            }
            try
            {
                IDbCommand objCmd = objConn.CreateCommand();
                objCmd.CommandText = strSql;
                objConn.Open();
                optReturn.IntValue = objCmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
                optReturn.Exception = ex;
            }
            finally
            {
                CloseConnection(objConn);
            }
            return optReturn;
        }
        /// <summary>
        /// 静态方法，执行存储过程
        /// </summary>
        /// <param name="strConn">数据库连接字符串</param>
        /// <param name="procedureName">存储过程名称</param>
        /// <param name="parameters">参数列表</param>
        /// <returns></returns>
        public static OperationReturn ExecuteProcedure(string strConn, string procedureName,
            List<DBCommandParameter> parameters)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = Defines.RET_SUCCESS;
            int dbType = GetDBType(strConn);
            if (dbType == Defines.VCT_DBTYPE_UNKNOWN)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_NOT_EXIST;
                optReturn.Message = string.Format("Unkown database type\t{0}", dbType);
                return optReturn;
            }
            optReturn = GetConnection(dbType, strConn);
            if (!optReturn.Result)
            {
                return optReturn;
            }
            IDbConnection objConn = optReturn.Data as IDbConnection;
            if (objConn == null)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_OBJECT_NULL;
                optReturn.Message = string.Format("Connection is null");
                return optReturn;
            }
            try
            {
                IDbDataParameter[] arrDataParameters = new IDbDataParameter[parameters.Count];
                for (int i = 0; i < parameters.Count; i++)
                {
                    IDbDataParameter parameter;
                    switch (parameters[i].DBType)
                    {
                        //case Defines.VCT_DBTYPE_MYSQL:
                        //    parameter = MySqlOperations.GetDataParameter(parameters[i]);
                        //    break;
                        case Defines.VCT_DBTYPE_MSSQL:
                            parameter = MsSqlOperations.GetDataParameter(parameters[i]);
                            break;
                        //case Defines.VCT_DBTYPE_ORACLE:
                        //    parameter = OracleOperations.GetDataParameter(parameters[i]);
                        //    break;
                        default:
                            optReturn.Result = false;
                            optReturn.Code = Defines.RET_NOT_IMPLIMENT;
                            optReturn.Message = string.Format("Database type not support.\t{0}", parameters[i].DBType);
                            return optReturn;
                    }
                    if (parameter == null)
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_OBJECT_NULL;
                        optReturn.Message = string.Format("Parameter is null");
                        return optReturn;
                    }
                    parameter.Value = parameters[i].ParamValue;
                    parameter.Direction = parameters[i].Direction;
                    arrDataParameters[i] = parameter;
                }
                IDbCommand objCmd = objConn.CreateCommand();
                objCmd.CommandType = CommandType.StoredProcedure;
                objCmd.CommandText = procedureName;
                for (int i = 0; i < arrDataParameters.Length; i++)
                {
                    objCmd.Parameters.Add(arrDataParameters[i]);
                }
                objConn.Open();
                optReturn.IntValue = objCmd.ExecuteNonQuery();
                for (int i = 0; i < arrDataParameters.Length; i++)
                {
                    if (arrDataParameters[i].Direction == ParameterDirection.Output ||
                        arrDataParameters[i].Direction == ParameterDirection.InputOutput)
                    {
                        parameters[i].ParamValue = arrDataParameters[i].Value;
                    }
                }
                optReturn.Data = parameters;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
                optReturn.Exception = ex;
            }
            finally
            {
                CloseConnection(objConn);
            }
            return optReturn;
        }
        /// <summary>
        /// 关闭数据库连接
        /// </summary>
        /// <param name="objConn">要关闭的连接</param>
        public static void CloseConnection(IDbConnection objConn)
        {
            if (objConn != null)
            {
                if (objConn.State == ConnectionState.Open)
                {
                    objConn.Close();
                }
                objConn.Dispose();
            }
        }
        /// <summary>
        /// 获取数据库连接字符串
        /// </summary>
        /// <param name="dbParam">数据库连接参数</param>
        /// <returns></returns>
        public static string GetConnectionString(DatabaseParam dbParam)
        {
            string strConn = string.Empty;
            switch (dbParam.DBType)
            {
                case Defines.VCT_DBTYPE_MYSQL:
                    strConn = string.Format("Server={0};Port={1};Database={2};Uid={3};Pwd={4}"
                        , dbParam.DBServer
                        , dbParam.DBPort
                        , dbParam.DBName
                        , dbParam.LoginUser
                        , dbParam.LoginPassword);
                    break;
                case Defines.VCT_DBTYPE_MSSQL:
                    strConn = string.Format("Data Source={0},{1};Initial Catalog={2};User Id={3};Password={4}"
                       , dbParam.DBServer
                       , dbParam.DBPort
                       , dbParam.DBName
                       , dbParam.LoginUser
                       , dbParam.LoginPassword);
                    break;
                case Defines.VCT_DBTYPE_ORACLE:
                    strConn = string.Format("Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST={0})(PORT={1})))(CONNECT_DATA=(SERVICE_NAME={2})));User ID={3};Password={4};"
                       , dbParam.DBServer
                       , dbParam.DBPort
                       , dbParam.DBName
                       , dbParam.LoginUser
                       , dbParam.LoginPassword);
                    break;
            }
            return strConn;
        }
        #endregion

        #region 私有静态方法
        /// <summary>
        /// 获取Connection
        /// </summary>
        /// <param name="dbType"></param>
        /// <param name="strConn"></param>
        /// <returns></returns>
        private static OperationReturn GetConnection(int dbType, string strConn)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = Defines.RET_SUCCESS;
            switch (dbType)
            {
                case Defines.VCT_DBTYPE_MSSQL:
                    optReturn.Data = MsSqlOperations.GetConnection(strConn);
                    break;
                //case Defines.VCT_DBTYPE_MYSQL:
                //    optReturn.Data = MySqlOperations.GetConnection(strConn);
                //    break;
                //case Defines.VCT_DBTYPE_ORACLE:
                //    optReturn.Data = OracleOperations.GetConnection(strConn);
                //    break;
                default:
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_NOT_IMPLIMENT;
                    optReturn.Message = string.Format("Database not impliment now\t{0}", dbType);
                    break;
            }
            return optReturn;
        }
        /// <summary>
        /// 获取DataAdapter
        /// </summary>
        /// <param name="dbType"></param>
        /// <param name="objConn"></param>
        /// <param name="strSql"></param>
        /// <returns></returns>
        private static OperationReturn GetDataAdapter(int dbType, IDbConnection objConn, string strSql)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = Defines.RET_SUCCESS;
            switch (dbType)
            {
                case Defines.VCT_DBTYPE_MSSQL:
                    optReturn.Data = MsSqlOperations.GetDataAdapter(objConn, strSql);
                    break;
                //case Defines.VCT_DBTYPE_MYSQL:
                //    optReturn.Data = MySqlOperations.GetDataAdapter(objConn, strSql);
                //    break;
                //case Defines.VCT_DBTYPE_ORACLE:
                //    optReturn.Data = OracleOperations.GetDataAdapter(objConn, strSql);
                //    break;
                default:
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_NOT_IMPLIMENT;
                    optReturn.Message = string.Format("Database not impliment now\t{0}", dbType);
                    break;
            }
            return optReturn;
        }
        /// <summary>
        /// 通过分析数据库连接字符串，猜测数据库类型
        /// </summary>
        /// <param name="strConn"></param>
        /// <returns></returns>
        private static int GetDBType(string strConn)
        {
            if (strConn.IndexOf("DESCRIPTION", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return Defines.VCT_DBTYPE_ORACLE;
            }
            if (strConn.IndexOf("Data Source", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return Defines.VCT_DBTYPE_MSSQL;
            }
            if (strConn.IndexOf("Uid", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return Defines.VCT_DBTYPE_MYSQL;
            }
            return Defines.VCT_DBTYPE_UNKNOWN;
        }
        #endregion

        #region 私有成员

        private DatabaseParam mDBParam;
        private IDbConnection mObjConnection;
        private object mObjConnectionLocker;

        #endregion

        #region 公共属性
        /// <summary>
        /// 获取或设置数据库连接参数
        /// </summary>
        public DatabaseParam DBParam
        {
            get { return mDBParam; }
            set { mDBParam = value; }
        }
        /// <summary>
        /// 获取或设置DBConnection
        /// </summary>
        public IDbConnection DbConnection
        {
            get { return mObjConnection; }
            set { mObjConnection = value; }
        }
        #endregion

        /// <summary>
        /// 创建一个DBAccessHelper实例
        /// </summary>
        public DBAccessHelper()
        {
            mObjConnectionLocker = new object();
        }
        /// <summary>
        /// 使用指定的数据库连接参数创建一个DBAccessHelper实例
        /// </summary>
        /// <param name="dbParam"></param>
        public DBAccessHelper(DatabaseParam dbParam)
            : this()
        {
            mDBParam = dbParam;
        }

        #region 公共方法
        /// <summary>
        /// 获取数据集
        /// </summary>
        /// <param name="strSql">Sql命令</param>
        /// <returns></returns>
        public OperationReturn GetDataSet(string strSql)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = Defines.RET_SUCCESS;
            if (mDBParam == null)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_OBJECT_NULL;
                optReturn.Message = string.Format("DatabaseParam is null");
                return optReturn;
            }
            if (mObjConnection == null)
            {
                string strConn = GetConnectionString(mDBParam);
                lock (mObjConnectionLocker)
                {
                    optReturn = GetConnection(mDBParam.DBType, strConn);
                }
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                mObjConnection = optReturn.Data as IDbConnection;
            }
            if (mObjConnection == null)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_OBJECT_NULL;
                optReturn.Message = string.Format("Connection is null");
                return optReturn;
            }
            optReturn = GetDataAdapter(mDBParam.DBType, mObjConnection, strSql);
            if (!optReturn.Result)
            {
                return optReturn;
            }
            IDataAdapter objAdapter = optReturn.Data as IDataAdapter;
            if (objAdapter == null)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_OBJECT_NULL;
                optReturn.Message = string.Format("DataAdapter is null");
                return optReturn;
            }
            DataSet objDS = new DataSet();
            try
            {
                if (mObjConnection.State != ConnectionState.Open)
                {
                    lock (mObjConnectionLocker)
                    {
                        mObjConnection.Open();
                    }
                }
                objAdapter.Fill(objDS);
                optReturn.Data = objDS;
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
        /// <summary>
        /// 执行Sql命令
        /// </summary>
        /// <param name="strSql">Sql命令</param>
        /// <returns></returns>
        public OperationReturn ExecuteCommand(string strSql)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = Defines.RET_SUCCESS;
            if (mDBParam == null)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_OBJECT_NULL;
                optReturn.Message = string.Format("DatabaseParam is null");
                return optReturn;
            }
            if (mObjConnection == null)
            {
                string strConn = GetConnectionString(mDBParam);
                lock (mObjConnectionLocker)
                {
                    optReturn = GetConnection(mDBParam.DBType, strConn);
                }
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                mObjConnection = optReturn.Data as IDbConnection;
            }
            if (mObjConnection == null)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_OBJECT_NULL;
                optReturn.Message = string.Format("Connection is null");
                return optReturn;
            }
            try
            {
                if (mObjConnection.State != ConnectionState.Open)
                {
                    lock (mObjConnectionLocker)
                    {
                        mObjConnection.Open();
                    }
                }
                IDbCommand objCmd = mObjConnection.CreateCommand();
                objCmd.CommandText = strSql;
                optReturn.Data = objCmd.ExecuteNonQuery();
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
        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="procedureName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public OperationReturn ExecuteProcedure(string procedureName, List<DBCommandParameter> parameters)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = Defines.RET_SUCCESS;
            if (mDBParam == null)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_OBJECT_NULL;
                optReturn.Message = string.Format("DatabaseParam is null");
                return optReturn;
            }
            if (mObjConnection == null)
            {
                string strConn = GetConnectionString(mDBParam);
                lock (mObjConnectionLocker)
                {
                    optReturn = GetConnection(mDBParam.DBType, strConn);
                }
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                mObjConnection = optReturn.Data as IDbConnection;
            }
            if (mObjConnection == null)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_OBJECT_NULL;
                optReturn.Message = string.Format("Connection is null");
                return optReturn;
            }
            try
            {
                IDbDataParameter[] arrDataParameters = new IDbDataParameter[parameters.Count];
                for (int i = 0; i < parameters.Count; i++)
                {
                    IDbDataParameter parameter;
                    switch (parameters[i].DBType)
                    {
                        //case Defines.VCT_DBTYPE_MYSQL:
                        //    parameter = MySqlOperations.GetDataParameter(parameters[i]);
                        //    break;
                        case Defines.VCT_DBTYPE_MSSQL:
                            parameter = MsSqlOperations.GetDataParameter(parameters[i]);
                            break;
                        //case Defines.VCT_DBTYPE_ORACLE:
                        //    parameter = OracleOperations.GetDataParameter(parameters[i]);
                        //    break;
                        default:
                            optReturn.Result = false;
                            optReturn.Code = Defines.RET_NOT_IMPLIMENT;
                            optReturn.Message = string.Format("Database type not support.\t{0}", parameters[i].DBType);
                            return optReturn;
                    }
                    if (parameter == null)
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_OBJECT_NULL;
                        optReturn.Message = string.Format("Parameter is null");
                        return optReturn;
                    }
                    parameter.Value = parameters[i].ParamValue;
                    parameter.Direction = parameters[i].Direction;
                    arrDataParameters[i] = parameter;
                }
                IDbCommand objCmd = mObjConnection.CreateCommand();
                objCmd.CommandType = CommandType.StoredProcedure;
                objCmd.CommandText = procedureName;
                for (int i = 0; i < arrDataParameters.Length; i++)
                {
                    objCmd.Parameters.Add(arrDataParameters[i]);
                }
                if (mObjConnection.State != ConnectionState.Open)
                {
                    lock (mObjConnectionLocker)
                    {
                        mObjConnection.Open();
                    }
                }
                optReturn.IntValue = objCmd.ExecuteNonQuery();
                for (int i = 0; i < arrDataParameters.Length; i++)
                {
                    if (arrDataParameters[i].Direction == ParameterDirection.Output ||
                        arrDataParameters[i].Direction == ParameterDirection.InputOutput)
                    {
                        parameters[i].ParamValue = arrDataParameters[i].Value;
                    }
                }
                optReturn.Data = parameters;
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
        /// <summary>
        /// 关闭连接
        /// </summary>
        public void CloseConnection()
        {
            if (mObjConnection != null)
            {
                lock (mObjConnectionLocker)
                {
                    if (mObjConnection.State == ConnectionState.Open)
                    {
                        mObjConnection.Close();
                    }
                    mObjConnection.Dispose();
                }
            }
        }
        #endregion

    }
}
