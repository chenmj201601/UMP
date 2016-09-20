//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    7c8929cb-9b73-44f2-93ad-0be2a39d0a4d
//        CLR Version:              4.0.30319.18063
//        Name:                     MySqlOperations
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Common
//        File Name:                MySqlOperations
//
//        created by Charley at 2014/3/23 10:11:00
//        http://www.voicecyber.com 
//
//======================================================================

using System.Data;
using MySql.Data.MySqlClient;

namespace VoiceCyber.Common
{
    class MySqlOperations
    {
        public static MySqlConnection GetConnection(string strConn)
        {
            return new MySqlConnection(strConn);
        }

        public static MySqlDataAdapter GetDataAdapter(IDbConnection objConn, string strSql)
        {
            MySqlConnection sqlConn = objConn as MySqlConnection;
            if (sqlConn == null)
            {
                return null;
            }
            return new MySqlDataAdapter(strSql, sqlConn);
        }

        public static MySqlParameter GetDataParameter(DBCommandParameter parameter)
        {
            switch (parameter.ParamType)
            {
                case DBCommandParamType.Int:
                    return new MySqlParameter(string.Format("@{0}", parameter.ParamName), MySqlDbType.Int32, parameter.ParamLength);
                case DBCommandParamType.VarChar:
                    return new MySqlParameter(string.Format("@{0}", parameter.ParamName), MySqlDbType.VarChar, parameter.ParamLength);
                default:
                    return null;
            }
        }
    }
}
