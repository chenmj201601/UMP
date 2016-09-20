using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;

namespace Wcf61061
{
    public class WarningCountFuncs
    {
        /// <summary>
        /// 创建告警数量统计的sql语句
        /// </summary>
        /// <param name="session"></param>
        /// <param name="lstParam"></param>
        /// lstParam[0] : 开始时间(日期类型字符串 yyyy-MM-dd HH:mm:ss)
        /// lstParam[1] : 结束时间(日期类型字符串 yyyy-MM-dd HH:mm:ss)
        /// <returns></returns>
        public static OperationReturn CreateSql(SessionInfo session, List<string> lstParam)
        {
            OperationReturn optReturn = new OperationReturn();
            try
            {
                string startTime = lstParam[0];
                string EndTime = lstParam[1];
                string strSql = string.Empty;
                string strToken = session.RentInfo.Token;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("select C001,C002,C012,C018,C019 from T_25_001 where C019 > '{0}' and C019 < '{1}'  and C002 =0", startTime, EndTime);
                        break;
                    case 3:
                        strSql = string.Format("select C001,C002,C012,C018,C019 from T_25_001 where " +
                                                        " C019 >= to_date('{1}','yyyy-mm-dd,hh24:mi:ss')  " +
                                                         "and C019 <= to_date('{2}','yyyy-mm-dd,hh24:mi:ss')  and C002 =0", startTime, EndTime);
                        break;
                }
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