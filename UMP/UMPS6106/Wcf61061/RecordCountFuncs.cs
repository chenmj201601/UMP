using Common6106;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;

namespace Wcf61061
{
    /// <summary>
    /// 最近7天通话量统计的类
    /// </summary>
    public class RecordCountFuncs
    {
        /// <summary>
        /// 获得mssql时的sql语句
        /// </summary>
        /// <param name="session"></param>
        /// <param name="lstParam"></param>
        /// lstParam[0] : 开始时间 （数字型）
        /// lstParam[1] : 结束时间 （数字型）
        /// <returns></returns>
        public static OperationReturn CreateRecordCountSqlInMSSql(SessionInfo session, List<string> lstParam)
        {
            OperationReturn optReturn = new OperationReturn();
            try
            {
                long lstartTime = long.Parse(lstParam[0]);
                long lEndTime = long.Parse(lstParam[1]);
                string rentToken = session.RentInfo.Token;

                //是否分表
                var tableInfo =
                       session.ListPartitionTables.FirstOrDefault(
                       t => t.TableName == ConstValue.TABLE_NAME_RECORD && t.PartType == TablePartType.DatetimeRange);
                string strSql = string.Empty;
                if (tableInfo == null)      //没有分表 直接从T_21_001_租户表中查询
                {
                    strSql = string.Format("select count(C002) as RecCount, Convert ( VARCHAR(10),  C005,  120) as RecDate,C045 as RecDir  from T_21_001_{0} " +
                                               " where C006 > {1} and C006 <{2} " +
                                               "group by Convert ( VARCHAR(10),  C005,  120),C045 " +
                                               "order by Convert ( VARCHAR(10), C005,  120)", rentToken, lstartTime, lEndTime);
                }
                else
                {
                    //分表  判断起始时间是否跨月  如果没有跨月 直接从对应的月份查询 如果跨月 则分开两个表查询
                    // 算出表名
                    string strStartTableName = string.Format("T_21_001_{0}_{1}", rentToken, lstartTime.ToString().Substring(2, 4));
                    string strEndTableName = string.Format("T_21_001_{0}_{1}", rentToken, lEndTime.ToString().Substring(2, 4));

                    //如果没有跨月
                    if (strStartTableName == strEndTableName)
                    {
                        optReturn = CommonFuns.CheckTableExists(session, strStartTableName);
                        //如果表不存在 或者检查失败
                        if (!optReturn.Result)
                        {
                            optReturn.Code = (int)S6106WcfErrorCode.NoData;
                            return optReturn;
                        }
                        strSql = string.Format("select count(C002) as RecCount, Convert ( VARCHAR(10),  C004,  120) as RecDate,C045 as RecDir  from {0} " +
                                               " where C006 > {1} and C006 <{2} " +
                                               "group by Convert ( VARCHAR(10),  C004,  120),C045 " +
                                               "", strStartTableName, lstartTime, lEndTime);
                    }
                    else
                    {
                        DateTime dtStart = CommonFunctions.StringToDateTime(lstartTime.ToString());
                        DateTime dtEnd = CommonFunctions.StringToDateTime(lEndTime.ToString());
                        List<string> lstTables = CommonFunctions.GetTablesByTime(ConstValue.TABLE_NAME_RECORD, dtStart, dtEnd, rentToken);
                        for (int i = 0; i < lstTables.Count; i++)
                        {
                            optReturn = CommonFuns.CheckTableExists(session, lstTables[i]);
                            if (optReturn.Result)
                            {
                                if (i == 0)
                                {
                                    strSql = string.Format("select count(C002) as RecCount, Convert ( VARCHAR(10),  C004,  120) as RecDate,C045 as RecDir  from {0} " +
                                            " where C006 > {1} " +
                                            "group by Convert ( VARCHAR(10),  C004,  120),C045 ", lstTables[i], lstartTime);
                                    continue;
                                }
                                if (!string.IsNullOrEmpty(strSql))
                                {
                                    strSql += " union ";
                                }
                                strSql += string.Format("select count(C002) as RecCount, Convert ( VARCHAR(10),  C004,  120) as RecDate,C045 as RecDir  from {0} " +
                                                          " where C006 < {1} " +
                                                          "group by Convert ( VARCHAR(10),  C004,  120),C045 ", lstTables[i], lEndTime);
                            }
                        }
                        
                        if (!string.IsNullOrEmpty(strSql))
                        {
                            strSql += "order by Convert ( VARCHAR(10), C004,  120)";
                        }
                    }
                    optReturn.Message = "strStartTableName = " + strStartTableName + " , strEndTableName = " + strEndTableName;
                }
                if (string.IsNullOrEmpty(strSql))
                {
                    optReturn.Code = (int)S6106WcfErrorCode.NoData;
                    optReturn.Result = false;
                    return optReturn;
                }
                optReturn.Result = true;
                optReturn.Code = Defines.RET_SUCCESS;
                optReturn.StringValue = strSql;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
                return optReturn;
            }
            return optReturn;
        }

        public static OperationReturn CreateRecordCountSqlInOracle(SessionInfo session, List<string> lstParam)
        {
            OperationReturn optReturn = new OperationReturn();

            return optReturn;
        }
    }
}