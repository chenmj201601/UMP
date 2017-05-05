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
        /// 获得我的联系人（包括可以管我的用户、我可以管理用户和坐席，如果传入的UserID是用户（智能客户端），则获取管理这个坐席的人）
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        public static OperationReturn GetContacters(SessionInfo session)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                string strMsg = string.Empty;
                string strSql;
                string strToken = session.RentInfo.Token;
                string strUserID = session.UserID.ToString();
                long userID;
                if (!long.TryParse(strUserID, out userID))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("UserID invalid.");
                    return optReturn;
                }
                int dbType = session.DatabaseInfo.TypeID;
                DataSet objDataSet;
                List<Contacter> listContactors = new List<Contacter>();


                #region 获取管理我的用户

                switch (dbType)
                {
                    case 2:
                        strSql =
                            string.Format(
                                "SELECT C.*, D.C015 AS ONLINESTATUS FROM (SELECT A.C001 AS USERID, A.C002 AS USERNAME, A.C003 AS FULLNAME, B.C001 AS ORGID, B.C002 AS ORGNAME, B.C004 AS PARENTORGID FROM T_11_005_{0} A, T_11_006_{0} B WHERE A.C006 = B.C001 AND A.C001 IN (SELECT C003 FROM T_11_201_{0} WHERE C004 = {1} AND C003 > 1020000000000000000 AND C003 < 1030000000000000000 AND C003 != C004)) C LEFT JOIN T_11_101_{0} D ON C.USERID = D.C001 ORDER BY C.USERID",
                                strToken, userID);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        break;
                    case 3:
                        strSql =
                           string.Format(
                               "SELECT C.*, D.C015 AS ONLINESTATUS FROM (SELECT A.C001 AS USERID, A.C002 AS USERNAME, A.C003 AS FULLNAME, B.C001 AS ORGID, B.C002 AS ORGNAME, B.C004 AS PARENTORGID FROM T_11_005_{0} A, T_11_006_{0} B WHERE A.C006 = B.C001 AND A.C001 IN (SELECT C003 FROM T_11_201_{0} WHERE C004 = {1} AND C003 > 1020000000000000000 AND C003 < 1030000000000000000 AND C003 != C004)) C LEFT JOIN T_11_101_{0} D ON C.USERID = D.C001 ORDER BY C.USERID",
                               strToken, userID);
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("DBType invalid.");
                        return optReturn;
                }
                strMsg += "Step1:" + strSql;
                if (!optReturn.Result)
                {
                    optReturn.Message += strMsg;
                    return optReturn;
                }
                objDataSet = optReturn.Data as DataSet;
                if (objDataSet == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("DataSet is null.") + strMsg;
                    return optReturn;
                }
                for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = objDataSet.Tables[0].Rows[i];

                    long id = Convert.ToInt64(dr["USERID"]);
                    Contacter contactor = new Contacter();
                    contactor.UserID = id;
                    contactor.UserName = WCF16001EncryptOperation.DecryptWithM002(dr["USERNAME"].ToString());
                    contactor.FullName = WCF16001EncryptOperation.DecryptWithM002(dr["FULLNAME"].ToString());
                    contactor.OrgID = Convert.ToInt64(dr["ORGID"]);
                    contactor.OrgName = WCF16001EncryptOperation.DecryptWithM002(dr["ORGNAME"].ToString());
                    contactor.ParentOrgID = Convert.ToInt64(dr["PARENTORGID"]);
                    string strStatus = dr["ONLINESTATUS"].ToString();
                    if (string.IsNullOrEmpty(strStatus))
                    {
                        contactor.Status = "0";
                    }
                    else
                    {
                        contactor.Status = strStatus;
                    }
                    var temp = listContactors.FirstOrDefault(c => c.UserID == id);
                    if (temp == null)
                    {
                        listContactors.Add(contactor);
                    }
                }

                #endregion


                if (userID > 1020000000000000000
                    && userID < 1030000000000000000)
                {

                    #region 获取我管理的用户

                    switch (dbType)
                    {
                        case 2:
                            strSql =
                                string.Format(
                                    "SELECT C.*, D.C015 AS ONLINESTATUS FROM (SELECT A.C001 AS USERID, A.C002 AS USERNAME, A.C003 AS FULLNAME, B.C001 AS ORGID, B.C002 AS ORGNAME, B.C004 AS PARENTORGID FROM T_11_005_{0} A, T_11_006_{0} B WHERE A.C006 = B.C001 AND A.C001 IN (SELECT C004 FROM T_11_201_{0} WHERE C003 = {1} AND C004 > 1020000000000000000 AND C004 < 1030000000000000000 AND C003 != C004)) C LEFT JOIN T_11_101_{0} D ON C.USERID = D.C001 ORDER BY C.USERID",
                                    strToken, userID);
                            optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                            break;
                        case 3:
                            strSql =
                               string.Format(
                                   "SELECT C.*, D.C015 AS ONLINESTATUS FROM (SELECT A.C001 AS USERID, A.C002 AS USERNAME, A.C003 AS FULLNAME, B.C001 AS ORGID, B.C002 AS ORGNAME, B.C004 AS PARENTORGID FROM T_11_005_{0} A, T_11_006_{0} B WHERE A.C006 = B.C001 AND A.C001 IN (SELECT C004 FROM T_11_201_{0} WHERE C003 = {1} AND C004 > 1020000000000000000 AND C004 < 1030000000000000000 AND C003 != C004)) C LEFT JOIN T_11_101_{0} D ON C.USERID = D.C001 ORDER BY C.USERID",
                                   strToken, userID);
                            optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                            break;
                        default:
                            optReturn.Result = false;
                            optReturn.Code = Defines.RET_PARAM_INVALID;
                            optReturn.Message = string.Format("DBType invalid.");
                            return optReturn;
                    }
                    strMsg += "Step2:" + strSql;
                    if (!optReturn.Result)
                    {
                        optReturn.Message += strMsg;
                        return optReturn;
                    }
                    objDataSet = optReturn.Data as DataSet;
                    if (objDataSet == null)
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_OBJECT_NULL;
                        optReturn.Message = string.Format("DataSet is null.") + strMsg;
                        return optReturn;
                    }
                    for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                    {
                        DataRow dr = objDataSet.Tables[0].Rows[i];

                        long id = Convert.ToInt64(dr["USERID"]);
                        Contacter contactor = new Contacter();
                        contactor.UserID = id;
                        contactor.UserName = WCF16001EncryptOperation.DecryptWithM002(dr["USERNAME"].ToString());
                        contactor.FullName = WCF16001EncryptOperation.DecryptWithM002(dr["FULLNAME"].ToString());
                        contactor.OrgID = Convert.ToInt64(dr["ORGID"]);
                        contactor.OrgName = WCF16001EncryptOperation.DecryptWithM002(dr["ORGNAME"].ToString());
                        contactor.ParentOrgID = Convert.ToInt64(dr["PARENTORGID"]);
                        string strStatus = dr["ONLINESTATUS"].ToString();
                        if (string.IsNullOrEmpty(strStatus))
                        {
                            contactor.Status = "0";
                        }
                        else
                        {
                            contactor.Status = strStatus;
                        }
                        var temp = listContactors.FirstOrDefault(c => c.UserID == id);
                        if (temp == null)
                        {
                            listContactors.Add(contactor);
                        }
                    }

                    #endregion


                    #region 获取我管理的坐席


                    #region 获取坐席编码

                    switch (dbType)
                    {
                        case 2:
                            strSql =
                                string.Format(
                                    "SELECT	A.C001 AS AC001, B.C001 AS BC001, B.C002 AS BC002, B.C004 AS BC004 FROM T_11_101_{0} A, T_11_006_{0} B WHERE A.C011 = B.C001 AND A.C002 = 1 AND A.C001 > 1030000000000000000 AND A.C001 < 1040000000000000000 AND A.C001 IN (SELECT C004 FROM T_11_201_{0} WHERE C003 = {1} AND C004 > 1030000000000000000 AND C004 < 1040000000000000000)",
                                    strToken, userID);
                            optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                            break;
                        case 3:
                            strSql =
                                string.Format(
                                    "SELECT	A.C001 AS AC001, B.C001 AS BC001, B.C002 AS BC002, B.C004 AS BC004 FROM T_11_101_{0} A, T_11_006_{0} B WHERE A.C011 = B.C001 AND A.C002 = 1 AND A.C001 > 1030000000000000000 AND A.C001 < 1040000000000000000 AND A.C001 IN (SELECT C004 FROM T_11_201_{0} WHERE C003 = {1} AND C004 > 1030000000000000000 AND C004 < 1040000000000000000)",
                                    strToken, userID);
                            optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                            break;
                        default:
                            optReturn.Result = false;
                            optReturn.Code = Defines.RET_PARAM_INVALID;
                            optReturn.Message = string.Format("DBType invalid.");
                            return optReturn;
                    }
                    strMsg += "Step3:" + strSql;
                    if (!optReturn.Result)
                    {
                        optReturn.Message += strMsg;
                        return optReturn;
                    }
                    objDataSet = optReturn.Data as DataSet;
                    if (objDataSet == null)
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_OBJECT_NULL;
                        optReturn.Message = string.Format("DataSet is null.") + strMsg;
                        return optReturn;
                    }
                    for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                    {
                        DataRow dr = objDataSet.Tables[0].Rows[i];

                        long id = Convert.ToInt64(dr["AC001"]);
                        Contacter contactor = new Contacter();
                        contactor.OrgID = Convert.ToInt64(dr["BC001"]);
                        contactor.OrgName = WCF16001EncryptOperation.DecryptWithM002(dr["BC002"].ToString());
                        contactor.ParentOrgID = Convert.ToInt64(dr["BC004"]);
                        contactor.UserID = id;
                        var temp = listContactors.FirstOrDefault(c => c.UserID == id);
                        if (temp == null)
                        {
                            listContactors.Add(contactor);
                        }
                    }

                    #endregion


                    #region 获取坐席其他信息

                    for (int i = 0; i < listContactors.Count; i++)
                    {
                        var contactor = listContactors[i];
                        long id = contactor.UserID;
                        if (id > 1030000000000000000
                            && id < 1040000000000000000)
                        {
                            switch (dbType)
                            {
                                case 2:
                                    strSql =
                                        string.Format(
                                            "SELECT * FROM T_11_101_{0} WHERE C001 = {1} ORDER BY C001, C002",
                                            strToken, id);
                                    optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                                    break;
                                case 3:
                                    strSql =
                                        string.Format(
                                            "SELECT * FROM T_11_101_{0} WHERE C001 = {1} ORDER BY C001, C002",
                                            strToken, id);
                                    optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                                    break;
                                default:
                                    optReturn.Result = false;
                                    optReturn.Code = Defines.RET_PARAM_INVALID;
                                    optReturn.Message = string.Format("DBType invalid.");
                                    return optReturn;
                            }
                            strMsg += "Step4:" + strSql;
                            if (!optReturn.Result)
                            {
                                optReturn.Message += strMsg;
                                return optReturn;
                            }
                            objDataSet = optReturn.Data as DataSet;
                            if (objDataSet == null)
                            {
                                optReturn.Result = false;
                                optReturn.Code = Defines.RET_OBJECT_NULL;
                                optReturn.Message = string.Format("DataSet is null.") + strMsg;
                                return optReturn;
                            }
                            for (int j = 0; j < objDataSet.Tables[0].Rows.Count; j++)
                            {
                                DataRow dr = objDataSet.Tables[0].Rows[j];

                                int row = Convert.ToInt32(dr["C002"]);
                                if (row == 1)
                                {
                                    contactor.UserName = WCF16001EncryptOperation.DecryptWithM002(dr["C017"].ToString());
                                    contactor.FullName = WCF16001EncryptOperation.DecryptWithM002(dr["C018"].ToString());
                                }
                                if (row == 2)
                                {
                                    string strStatus = dr["C020"].ToString();
                                    if (string.IsNullOrEmpty(strStatus))
                                    {
                                        contactor.Status = "0";
                                    }
                                    else
                                    {
                                        contactor.Status = strStatus;
                                    }
                                }
                            }
                        }
                    }

                    #endregion


                    #endregion

                }

                List<string> listReturn = new List<string>();
                for (int i = 0; i < listContactors.Count; i++)
                {
                    var contactor = listContactors[i];

                    optReturn = XMLHelper.SeriallizeObject(contactor);
                    if (!optReturn.Result)
                    {
                        optReturn.Message += "Step5:" + strMsg;
                        return optReturn;
                    }
                    listReturn.Add(optReturn.Data.ToString());
                }

                optReturn.Data = listReturn;
                optReturn.Message = strMsg;

            }
            catch (Exception ex)
            {
                optReturn.Code = (int)S1600WcfError.GetSuperiorException;
                optReturn.Result = false;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }
       
    }
}