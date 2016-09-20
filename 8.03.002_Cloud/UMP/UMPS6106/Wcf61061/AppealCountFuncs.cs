using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;

namespace Wcf61061
{
    public class AppealCountFuncs
    {
        /// <summary>
        /// 创建申诉数量统计的sql语句
        /// </summary>
        /// <param name="session"></param>
        /// <param name="lstParam"></param>
        /// lstParam[0] : 开始时间（yyyy-MM-dd hh:mm:ss格式）
        /// lstParam[1] : 结束时间（yyyy-MM-dd hh:mm:ss格式）
        /// lstParam[2] : TempID
        /// <returns></returns>
        public static OperationReturn CreateSql(SessionInfo session, List<string> lstParam)
        {
            OperationReturn optReturn = new OperationReturn();
            try
            {
                string startTime = lstParam[0];
                string EndTime = lstParam[1];
                string strTempID = lstParam[2];
                string strSql = string.Empty;
                string strToken = session.RentInfo.Token;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT *   FROM T_31_008_{0} where   C006 between '{1}' and '{2}' and C014<>'0'", strToken, startTime, EndTime);
                        break;
                    case 3:
                        strSql = string.Format("SELECT *  FROM T_31_008_{0} where " +
                                                        " C006 >= to_date('{1}','yyyy-mm-dd,hh24:mi:ss')  " +
                                                         "and C006 <= to_date('{2}','yyyy-mm-dd,hh24:mi:ss') and C014<>'0'", strToken, startTime, EndTime);
                        break;
                }
                strSql += string.Format(" and C005 in (select C011 from T_00_901 where C001 = {0})", strTempID);
                optReturn.Result = true;
                optReturn.Code = Defines.RET_SUCCESS;
                optReturn.Data = strSql;
                optReturn.Message = strSql;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }
    }
}