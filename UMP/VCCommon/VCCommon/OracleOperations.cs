//======================================================================
//
//        Copyright © 2014-2015 VoiceCyber Technology (SH) Ltd.
//        All rights reserved
//        guid1:                    b69bfaff-3f8f-4417-809f-278578fe7eeb
//        CLR Version:              4.0.30319.18063
//        Name:                     OracleOperations
//        Computer:                 CHARLEY-PC
//        Organization:             VoiceCyber
//        Namespace:                VoiceCyber.Common
//        File Name:                OracleOperations
//
//        created by Charley at 2014/3/23 10:13:19
//        http://www.voicecyber.com 
//
//======================================================================

using System.Data;
using System.Data.OracleClient;

namespace VoiceCyber.Common
{
    class OracleOperations
    {
        public static OracleConnection GetConnection(string strConn)
        {
            return new OracleConnection(strConn);
        }

        public static OracleDataAdapter GetDataAdapter(IDbConnection objConn, string strSql)
        {
            OracleConnection sqlConn = objConn as OracleConnection;
            if (sqlConn == null)
            {
                return null;
            }
            return new OracleDataAdapter(strSql, sqlConn);
        }

        public static OracleParameter GetDataParameter(DBCommandParameter parameter)
        {
            switch (parameter.ParamType)
            {
                case DBCommandParamType.Int:
                    return new OracleParameter(string.Format("{0}", parameter.ParamName), OracleType.Int32, parameter.ParamLength);
                case DBCommandParamType.Number:
                    return new OracleParameter(string.Format("{0}", parameter.ParamName), OracleType.Number, parameter.ParamLength);
                case DBCommandParamType.VarChar:
                    return new OracleParameter(string.Format("{0}", parameter.ParamName), OracleType.VarChar, parameter.ParamLength);
                case DBCommandParamType.NVarChar:
                    return new OracleParameter(string.Format("{0}", parameter.ParamName), OracleType.NVarChar, parameter.ParamLength);
                default:
                    return null;
            }
        }
    }
}
