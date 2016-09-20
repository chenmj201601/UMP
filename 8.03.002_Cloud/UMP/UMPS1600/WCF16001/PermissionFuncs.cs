using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VoiceCyber.Common;
using Common1600;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;
using WCF16001.Service16002;
using WCF16001.Service11012;
using System.Data;

namespace WCF16001
{
    public class PermissionFuncs
    {
        /// <summary>
        /// 获得我的联系人（包括可以管我的用户、我可以管理用户和坐席）
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public static OperationReturn GetContacters(SessionInfo session)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = Defines.RET_SUCCESS;
            List<string> lstContacters = new List<string>();
            string strMsg = string.Empty;

            try
            {
                #region 获得管我的用户 和我可以管的用户跟坐席 合并到一个dataset
                optReturn = GetSuperior(session);
                strMsg = optReturn.Message;
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                DataSet dsAll = new DataSet();

                DataSet ds = optReturn.Data as DataSet;
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    DataTable dt = ds.Tables[0].Copy();
                    dsAll.Tables.Add(dt);
                }
                optReturn = GetSubordinate(session);
                strMsg += " ; " + optReturn.Message;
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                ds = optReturn.Data as DataSet;
                if (ds.Tables.Count > 0 || ds.Tables[0].Rows.Count > 0)
                {
                    DataTable dt = ds.Tables[0].Copy();
                    dt.TableName = "Subordinate";
                    dsAll.Tables.Add(dt);
                }
                #endregion
                for (int i = 0; i < dsAll.Tables.Count; i++)
                {
                    Contacter contacter = null;
                    string strStatus = string.Empty;
                    foreach (DataRow row in dsAll.Tables[i].Rows)
                    {
                        contacter = new Contacter();
                        contacter.UserID = long.Parse(row["UserID"].ToString());
                        contacter.UserName =WCF16001EncryptOperation.DecryptWithM002( row["UserName"].ToString());
                        contacter.FullName = WCF16001EncryptOperation.DecryptWithM002(row["FullName"].ToString());
                        contacter.OrgName = WCF16001EncryptOperation.DecryptWithM002(row["OrgName"].ToString());
                        contacter.OrgID = long.Parse(row["OrgID"].ToString());
                        contacter.ParentOrgID = long.Parse(row["ParentOrgID"].ToString());
                        strStatus = row["OnLineStatus"].ToString();
                        if (strStatus == "0" || string.IsNullOrEmpty(strStatus))
                        {
                            contacter.Status = "0";
                        }
                        else
                        {
                            contacter.Status = "1";
                        }
                        optReturn = XMLHelper.SeriallizeObject<Contacter>(contacter);
                        if (optReturn.Result)
                        {
                            lstContacters.Add(optReturn.Data.ToString());
                        }
                    }
                }
                optReturn.Result = true;
                optReturn.Data = lstContacters;
                optReturn.Code = Defines.RET_SUCCESS;
                optReturn.Message = strMsg;
            }
            catch (Exception ex)
            {
                optReturn.Code = (int)S1600WcfError.GetContacterException;
                optReturn.Result = false;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }

        /// <summary>
        /// 获得管我的人
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        private static OperationReturn GetSuperior(SessionInfo session)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = Defines.RET_SUCCESS;
            List<string> lstContacters = new List<string>();

            try
            {
                string strSql = string.Empty;
                string strToken = session.RentInfo.Token;
                string strUserID = session.UserID.ToString();
                switch (session.DBType)
                {
                    case 2:
                        strSql = "select c.UserID,c.UserName,c.fullName,c.OrgID,c.OrgName,d.C015 as OnLineStatus,c.ParentOrgID from " +
                                     "(select a.C001 as UserID,a.C002 as UserName,a.C003 as fullName,b.C001 as OrgID,b.C002 as OrgName,B.C004 as ParentOrgID " +
                                     "FROM t_11_005_{0} a left join t_11_006_{1} b  " +
                                     "on a.C006 = b.C001 where a.C001 in (select C003 from t_11_201_{2} where C004 ={3} and C003 >1020000000000000000  " +
                                     "and C003 <1030000000000000000  and C003 !=C004)) as c left join t_11_101_{4} as d on c.UserID = d.C001 ";
                        strSql = string.Format(strSql, strToken, strToken, strToken, strUserID, strToken);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        break;
                    case 3:
                        strSql = "SELECT C.*,D.C015 as OnLineStatus FROM (select a.C001 UserID,a.C002 UserName,a.C003 FullName,b.C002 OrgName ,b.C001 as OrgID,b.C004 as ParentOrgID " +
                                   "FROM t_11_005_{0} a left join t_11_006_{1} b " +
                                   "on a.C006 = b.C001 where a.C001 in (select C003 from t_11_201_{2} where C004 ={3} and C003 >1020000000000000000  " +
                                   "and C003 <1030000000000000000  and C003 !=C004)) C ,T_11_101_{4} D WHERE C.UserID=D.C001(+)";
                        strSql = string.Format(strSql, strToken, strToken, strToken, strUserID, strToken);
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        break;
                }
                optReturn.Message += "; " + strSql;
                if (!optReturn.Result)
                {
                    optReturn.Code = (int)S1600WcfError.GetSuperiorError;
                }
                return optReturn;
            }
            catch (Exception ex)
            {
                optReturn.Code = (int)S1600WcfError.GetSuperiorException;
                optReturn.Result = false;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }

        /// <summary>
        /// 获得我管的用户和坐席
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        private static OperationReturn GetSubordinate(SessionInfo session)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = Defines.RET_SUCCESS;
            List<string> lstContacters = new List<string>();
            try
            {
                string strSql = string.Empty;
                string strToken = session.RentInfo.Token;
                string strUserID = session.UserID.ToString();
                switch (session.DBType)
                {
                    case 2:
                        strSql = "select a.C001 UserID,a.C002 UserName,a.C003 FullName,b.C002 OrgName,c.C015 as OnlineStatus,b.C001 as OrgID,b.C004 as ParentOrgID  " +
                                        "FROM t_11_005_{0} a left join t_11_006_{1} b " +
                                        "on a.C006 = b.C001  left join T_11_101_{2} as c  on a.C001 =  CONVERT(varchar(1024), c.C001) " +
                                         "where a.C001 in (select C004 from t_11_201_{3} where C003 ={4} and C004 >1020000000000000000  " +
                                        "and C004 <1030000000000000000 and C003 !=C004) " +
                                        "union " +
                                        "select a.C001 as UserID,a.C017 as UserName,a.C018 as FullName,c.C002 as OrgName,b.C020 as OnlineStatus,b.C001 as OrgID,c.C004 as ParentOrgID " +
                                        "from t_11_101_{5} as a  left join t_11_101_{6} as b  on a.C001 = b.C001 left join T_11_006_{7} as c on a.C011 = CONVERT(varchar(1024), c.C001) " +
                                        "where a.C002<b.C002 and a.C001 in  " +
                                        "(select C004 from t_11_201_{8} where C003 ={9} and C004 >1030000000000000000 and C004 <1040000000000000000 and C003 !=C004)";
                        strSql = string.Format(strSql, strToken, strToken, strToken, strToken, strUserID, strToken, strToken, strToken, strToken, strUserID);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        break;
                    case 3:
                        strSql = "SELECT C.UserID,c.UserName,c.FullName,c.OrgName,c.orgID,D.C015 as OnlineStatus,c.ParentOrgID FROM " +
                                        "(select a.C001 UserID,a.C002 UserName,a.C003 FullName,b.C002 OrgName,a.C006 as orgID,b.C004 as ParentOrgID " +
                                        "FROM t_11_005_{0} a left join t_11_006_{1} b on a.C006 = b.C001 where a.C001 in " +
                                        "(select C004 from t_11_201_{2} where C003 ={3} and C004 >1020000000000000000 " +
                                        "and C004 <1030000000000000000 and C003 !=C004)) C ,T_11_101_{4} D WHERE C.UserID=D.C001(+) " +
                                        "union " +
                                        "select c.UserID,c.UserName,c.FullName,d.c002 OrgName,d.c001 as orgID,c.OnlineStatus,d.C004 as ParentOrgID  from  " +
                                        "(select a.C001 UserID,a.C017 UserName,a.C018 FullName,b.C020 OnlineStatus,to_char(a.C011) as OrgID " +
                                        "from t_11_101_{5} a  left join t_11_101_{6}  b on a.C001 = b.C001 where a.C002<b.C002 and a.C001 in  " +
                                        "(select C004 from t_11_201_{7} where C003 ={8} and C004 >1030000000000000000  " +
                                        "and C004 <1040000000000000000 and C003 !=C004)) c ,t_11_006_{9} d where c.orgID = TO_CHAR(d.c001)";
                        strSql = string.Format(strSql, strToken, strToken, strToken, strUserID, strToken, strToken, strToken, strToken, strUserID, strToken);
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        break;
                }
                optReturn.Message += "; " + strSql;
                if (!optReturn.Result)
                {
                    optReturn.Code = (int)S1600WcfError.GetSubordinateError;
                }
                return optReturn;
            }
            catch (Exception ex)
            {
                optReturn.Code = (int)S1600WcfError.GetSubordinateException;
                optReturn.Result = false;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }
    }
}