//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    47383dd5-a0a9-4f89-8649-0e7715d5df0d
//        CLR Version:              4.0.30319.18063
//        Name:                     MSSqlOperations
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Common
//        File Name:                MSSqlOperations
//
//        created by Charley at 2014/3/23 10:03:25
//        http://www.voicecyber.com 
//
//======================================================================

using System.Data;
using System.Data.SqlClient;

namespace VoiceCyber.Common
{
    class MsSqlOperations
    {
        public static SqlConnection GetConnection(string strConn)
        {
            return new SqlConnection(strConn);
        }

        public static SqlDataAdapter GetDataAdapter(IDbConnection objConn, string strSql)
        {
            SqlConnection sqlConn = objConn as SqlConnection;
            if (sqlConn == null)
            {
                return null;
            }
            return new SqlDataAdapter(strSql, sqlConn);
        }

        public static SqlParameter GetDataParameter(DBCommandParameter parameter)
        {
            switch (parameter.ParamType)
            {
                case DBCommandParamType.Int:
                    return new SqlParameter(string.Format("@{0}", parameter.ParamName), SqlDbType.Int, parameter.ParamLength);
                case DBCommandParamType.VarChar:
                    return new SqlParameter(string.Format("@{0}", parameter.ParamName), SqlDbType.VarChar, parameter.ParamLength);
                case DBCommandParamType.NVarChar:
                    return new SqlParameter(string.Format("@{0}", parameter.ParamName), SqlDbType.NVarChar, parameter.ParamLength);
                default:
                    return null;
            }
        }
    }
}
