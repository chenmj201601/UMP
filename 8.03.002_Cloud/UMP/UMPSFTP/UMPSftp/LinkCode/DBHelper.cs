using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
//using MySql.Data.MySqlClient;
//using System.Data.SQLite;

namespace UMPCommon
{
    public class DBConnectionInfo
    {
        public enum SqlType { Mysql = 0, MSSql, Oracle, Sqlite };
        public SqlType DBType { get; set; }
        public string Server { get; set; }
        public string Database { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int Port { get; set; }
        public string ConnectionString()
        {
            string con_string = "";
            switch (DBType)
            {
                case SqlType.Mysql:
                    {
                        con_string = string.Format("Server={0};Port={4};Database={1};Uid={2};Pwd={3};",
                            Server, Database, Username, Password, Port);
                    }
                    break;
                case SqlType.MSSql:
                    {
                        con_string = string.Format("Data Source={0},{3};Network Library=DBMSSOCN;Initial Catalog={1};User ID={2};Password={3};",
                            Server, Database, Username, Password, Port);
                    }
                    break;
                case SqlType.Oracle:
                    {

                    }
                    break;
                case SqlType.Sqlite:
                    {
                        con_string = string.Format("Data Source={0}", Server);
                    }
                    break;
            }

            return con_string;
        }
    }

    public class DBHelper
    {
        DBConnectionInfo _conntor;

        public DBHelper(DBConnectionInfo connect_info)
        {
            _conntor = connect_info;
        }

        DbConnection FactoryConnection()
        {
            switch (_conntor.DBType)
            {
                //case DBConnectionInfo.SqlType.Mysql:
                //    return new MySqlConnection(_conntor.ConnectionString());
                case DBConnectionInfo.SqlType.MSSql:
                    return new SqlConnection(_conntor.ConnectionString());
                case DBConnectionInfo.SqlType.Oracle:
                    return null;
                //case DBConnectionInfo.SqlType.Sqlite:
                //    return new SQLiteConnection(_conntor.ConnectionString());
            }
            return null;
        }

        DbCommand FactoryCommand(string sql, DbConnection conn)
        {
            switch (_conntor.DBType)
            {
                //case DBConnectionInfo.SqlType.Mysql:
                //    return new MySqlCommand(sql, (MySqlConnection)conn);
                case DBConnectionInfo.SqlType.MSSql:
                    return new SqlCommand(sql, (SqlConnection)conn);
                case DBConnectionInfo.SqlType.Oracle:
                    return null;
                //case DBConnectionInfo.SqlType.Sqlite:
                //    return new SQLiteCommand(sql, (SQLiteConnection)conn);
            }
            return null;
        }

        DbDataAdapter FactoryAdapter(string sql, DbConnection conn)
        {
            switch (_conntor.DBType)
            {
                //case DBConnectionInfo.SqlType.Mysql:
                //    return new MySqlDataAdapter(sql, (MySqlConnection)conn);
                case DBConnectionInfo.SqlType.MSSql:
                    return new SqlDataAdapter(sql, (SqlConnection)conn);
                case DBConnectionInfo.SqlType.Oracle:
                    return null;
                //case DBConnectionInfo.SqlType.Sqlite:
                //    return new SQLiteDataAdapter(sql, (SQLiteConnection)conn);
            }
            return null;
        }

        /// <summary>
        /// 执行SQL语句，返回数据到DataSet中
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <returns>返回DataSet</returns>
        public DataSet ReturnDataSet(string sql, string tablename)
        {
            using (DbConnection conn = FactoryConnection())
            {
                conn.Open();
                using (DbDataAdapter mda = FactoryAdapter(sql, conn))
                {
                    DataSet dataSet = new DataSet(tablename);
                    mda.Fill(dataSet, tablename);
                    return dataSet;
                }
            }
        }

        public int ExecuteQuery(string sql)
        {
            using (DbConnection conn = FactoryConnection())
            {
                conn.Open();
                DbCommand command = FactoryCommand(sql, conn);
                return command.ExecuteNonQuery();
            }
        }
    }
}
