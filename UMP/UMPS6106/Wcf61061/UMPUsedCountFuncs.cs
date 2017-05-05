using Common6106;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;

namespace Wcf61061
{
    /// <summary>
    /// UMP使用情况统计
    /// </summary>
    public class UMPUsedCountFuncs
    {
        /// <summary>
        /// 获得mssql时的sql语句
        /// </summary>
        /// <param name="session"></param>
        /// <param name="lstParam"></param>
        /// lstParam[0] : 开始时间 （数字型）
        /// lstParam[1] : 结束时间 （数字型）
        /// lstParam[2]及以后：可管理的用户
        /// <returns></returns>
        public static OperationReturn CreateUMPUsedCountSql(SessionInfo session, List<string> lstParam)
        {
            OperationReturn optReturn = new OperationReturn();
            try
            {
                long lstartTime = long.Parse(lstParam[0]);
                long lEndTime = long.Parse(lstParam[1]);
                string rentToken = session.RentInfo.Token;

                //先组织处需要包含的用户ID的where语句
                string strUser = string.Empty;
                if (lstParam.Count > 2)
                {
                    strUser = " and C005 in (";
                    for (int i = 2; i < lstParam.Count; i++)
                    {
                        strUser += lstParam[i] + ",";
                    }
                    strUser = strUser.TrimEnd(',');
                    strUser += ")";
                }

                //是否分表
                var tableInfo =
                       session.ListPartitionTables.FirstOrDefault(
                       t => t.TableName == ConstValue.TABLE_NAME_OPTLOG && t.PartType == TablePartType.DatetimeRange);
                string strSql = string.Empty;
                if (tableInfo == null)      //没有分表 直接从T_11_901_租户表中查询
                {
                    strSql = string.Format("SELECT C001,C008 FROM T_11_901_{0}" +
                                                    " where C004 = 110001 and C008 > {1} and C008 < {2} and C009 = 'R1' ", rentToken, lstartTime, lEndTime);
                    strSql += strUser;
                }
                else
                {
                    //分表  判断起始时间是否跨月  如果没有跨月 直接从对应的月份查询 如果跨月 则分开两个表查询
                    // 算出表名
                    string strStartTableName = string.Format("T_11_901_{0}_{1}", rentToken, lstartTime.ToString().Substring(2, 4));
                    string strEndTableName = string.Format("T_11_901_{0}_{1}", rentToken, lEndTime.ToString().Substring(2, 4));

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
                        strSql = string.Format("SELECT C001,C008 FROM {0} " +
                                               " where C004 = 110001 and C008 > {1} and C008 <= {2} and C009 = 'R1' "
                                                , strStartTableName, lstartTime, lEndTime);
                        strSql += strUser;
                    }
                    else
                    {
                        DateTime dtStart = CommonFunctions.StringToDateTime(lstartTime.ToString());
                        DateTime dtEnd = CommonFunctions.StringToDateTime(lEndTime.ToString());
                        List<string> lstTables = CommonFunctions.GetTablesByTime(ConstValue.TABLE_NAME_OPTLOG, dtStart, dtEnd, rentToken);
                        for (int i = 0; i < lstTables.Count; i++)
                        {
                            optReturn = CommonFuns.CheckTableExists(session, lstTables[i]);
                            if (optReturn.Result)
                            {
                                if (!string.IsNullOrEmpty(strSql))
                                {
                                    strSql += " union ";
                                }
                                strSql = string.Format("SELECT C001,C008 FROM {0}" +
                                        " where C004 = 110001 and C008 > {1}  and C008 <= {2}  and C009 = 'R1'  "
                                        , lstTables[i], lstartTime, lEndTime);
                                strSql += strUser;
                            }
                        }
                    }
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


    }
}