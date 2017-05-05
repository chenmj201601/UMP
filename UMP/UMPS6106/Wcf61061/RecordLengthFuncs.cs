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
    /// 通话时长统计
    /// </summary>
    public class RecordLengthFuncs
    {
        /// <summary>
        /// 组成sql语句
        /// </summary>
        ///    /// lstParam[0] : 开始时间 （数字型）
        /// lstParam[1] : 结束时间 （数字型）
        /// lstParam[2] : 统计分机号还是坐席
        /// lstParam[3] : 临时表中的ID号
        /// <returns></returns>
        public static OperationReturn CreateSql(SessionInfo session, List<string> lstParam)
        {
            OperationReturn optReturn = new OperationReturn();
            string strSql = string.Empty;
            try
            {
                long lstartTime = long.Parse(lstParam[0]);
                long lEndTime = long.Parse(lstParam[1]);
                string rentToken = session.RentInfo.Token;
                string strMode = lstParam[2];

                string strWhere = string.Empty;

                if (lstParam.Count >= 3)
                {
                    if (strMode == "A")
                    {
                        strWhere = string.Format(" and C039 in (select C012 from T_00_901 where C001 = {0})", lstParam[3]);
                    }
                    else if (strMode == "E")
                    {
                        strWhere = string.Format(" and C042 in (select C012 from T_00_901 where C001 = {0})", lstParam[3]);
                    }
                }

                //是否分表
                var tableInfo =
                       session.ListPartitionTables.FirstOrDefault(
                       t => t.TableName == ConstValue.TABLE_NAME_RECORD && t.PartType == TablePartType.DatetimeRange);

                if (tableInfo == null)
                {
                    //没有分表 直接从T_21_001_00000中查询
                    strSql = string.Format("select C002,C004,C005,C006,C012,C068 from t_21_001_{0} where C006 >= {1} and C006 <= {2}", rentToken, lstartTime, lEndTime);
                    strSql += strWhere;
                }
                else
                {
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
                        strSql = string.Format("select C002,C004,C005,C006,C012,C068 from {0} where C006 >= {1} and C006 <= {2} ", strStartTableName, lstartTime, lEndTime);
                        strSql += strWhere;
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
                                if (!string.IsNullOrEmpty(strSql))
                                {
                                    strSql += " union ";
                                }
                                strSql += string.Format("select C002,C004,C005,C006,C012,C068 from {0} where  C006 <= {1} ", lstTables[i], lEndTime);
                                strSql += strWhere;
                            }
                        }
                    }
                }

                if (string.IsNullOrEmpty(strSql))
                {
                    optReturn.Result = false;
                    optReturn.Code = (int)S6106WcfErrorCode.NoData;
                    optReturn.Message = "sql is null, all table does not exist";
                    return optReturn;
                }

                optReturn.Result = true;
                optReturn.Code = Defines.RET_SUCCESS;
                optReturn.Data = strSql;
                optReturn.StringValue = strSql;
                optReturn.Message = strSql;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
                optReturn.StringValue = strSql;
                return optReturn;
            }
            return optReturn;
        }
    }
}