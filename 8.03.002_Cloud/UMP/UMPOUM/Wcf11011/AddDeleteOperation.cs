using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common11011;
using System.Linq;

namespace Wcf11011
{
    public partial class Service11011
    {
        private OperationReturn AddNewOrgInfo(SessionInfo session, string strOrgInfo)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            int errNum = 0;
            long serialID = 0;
            string errMsg = string.Empty;
            string ObjID = string.Empty;
            try
            {
                optReturn = XMLHelper.DeserializeObject<BasicOrgInfo>(strOrgInfo);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                BasicOrgInfo newOrgInfo = optReturn.Data as BasicOrgInfo;
                if (newOrgInfo == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("NewOrgInfo is null");
                    return optReturn;
                }
                newOrgInfo.OrgName = EncryptToDB(newOrgInfo.OrgName);
                newOrgInfo.StrStartTime = EncryptToDB(newOrgInfo.StrStartTime);
                newOrgInfo.StrEndTime = EncryptToDB(newOrgInfo.StrEndTime);
                switch (session.DBType)
                {
                    //MSSQL
                    case 2:
                        DbParameter[] mssqlParameters =
                        {
                            MssqlOperation.GetDbParameter("@ainparam01",MssqlDataType.Varchar,5 ),
                            MssqlOperation.GetDbParameter("@ainparam02",MssqlDataType.NVarchar,1024),
                            MssqlOperation.GetDbParameter("@ainparam03",MssqlDataType.Varchar,5),
                            MssqlOperation.GetDbParameter("@ainparam04",MssqlDataType.Varchar,19),
                            MssqlOperation.GetDbParameter("@ainparam05",MssqlDataType.Char,1),
                            MssqlOperation.GetDbParameter("@ainparam06",MssqlDataType.Char,1),
                            MssqlOperation.GetDbParameter("@ainparam07",MssqlDataType.Varchar,32),
                            MssqlOperation.GetDbParameter("@ainparam08",MssqlDataType.Varchar,512),
                            MssqlOperation.GetDbParameter("@ainparam09",MssqlDataType.Varchar,512),
                            MssqlOperation.GetDbParameter("@ainparam10",MssqlDataType.Varchar,19),
                            MssqlOperation.GetDbParameter("@ainparam11",MssqlDataType.Varchar,0),
                            MssqlOperation.GetDbParameter("@ainparam12",MssqlDataType.Varchar,1024),
                            MssqlOperation.GetDbParameter("@aoutparam01",MssqlDataType.Varchar,19),
                            MssqlOperation.GetDbParameter("@aouterrornumber",MssqlDataType.Bigint,0),
                            MssqlOperation.GetDbParameter("@aouterrorstring",MssqlDataType.Varchar,1024)
                        };
                        mssqlParameters[0].Value = session.RentInfo.Token;
                        mssqlParameters[1].Value = newOrgInfo.OrgName;
                        mssqlParameters[2].Value = newOrgInfo.OrgType;
                        mssqlParameters[3].Value = newOrgInfo.ParentID;
                        mssqlParameters[4].Value = newOrgInfo.IsActived;
                        mssqlParameters[5].Value = newOrgInfo.IsDeleted;
                        mssqlParameters[6].Value = newOrgInfo.State;
                        mssqlParameters[7].Value = newOrgInfo.StrStartTime;
                        mssqlParameters[8].Value = newOrgInfo.StrEndTime;
                        mssqlParameters[9].Value = newOrgInfo.Creator;
                        mssqlParameters[10].Value = newOrgInfo.CreateTime.ToUniversalTime().ToString("yyyy/MM/dd HH:mm:ss");
                        mssqlParameters[11].Value = newOrgInfo.Description;
                        mssqlParameters[12].Value = serialID;
                        mssqlParameters[13].Value = errNum;
                        mssqlParameters[14].Value = errMsg;
                        mssqlParameters[12].Direction = ParameterDirection.Output;
                        mssqlParameters[13].Direction = ParameterDirection.Output;
                        mssqlParameters[14].Direction = ParameterDirection.Output;
                        optReturn = MssqlOperation.ExecuteStoredProcedure(session.DBConnectionString, "P_11_010",
                           mssqlParameters);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        if (mssqlParameters[13].Value.ToString() != "0")
                        {
                            if (mssqlParameters[13].Value.ToString() == "-1")
                            {
                                //同名机构名称
                                optReturn.Result = false;
                                optReturn.Code = Defines.RET_DBACCESS_EXIST;
                                optReturn.Message = mssqlParameters[14].Value.ToString();
                            }
                            else
                            {
                                optReturn.Result = false;
                                optReturn.Code = Defines.RET_DBACCESS_FAIL;
                                optReturn.Message = string.Format("{0}\t{1}", mssqlParameters[13].Value, mssqlParameters[14].Value);
                            }
                        }
                        else
                        {
                            ObjID = mssqlParameters[12].Value.ToString();
                            optReturn.Data = mssqlParameters[12].Value.ToString();
                        }
                        break;
                    //ORCL
                    case 3:
                        DbParameter[] orclParameters =
                        {
                            OracleOperation.GetDbParameter("ainparam01",OracleDataType.Varchar2,5 ),
                            OracleOperation.GetDbParameter("ainparam02",OracleDataType.Nvarchar2,1024),
                            OracleOperation.GetDbParameter("ainparam03",OracleDataType.Int32,0),
                            OracleOperation.GetDbParameter("ainparam04",OracleDataType.Varchar2,19),
                            OracleOperation.GetDbParameter("ainparam05",OracleDataType.Char,1),
                            OracleOperation.GetDbParameter("ainparam06",OracleDataType.Char,1),
                            OracleOperation.GetDbParameter("ainparam07",OracleDataType.Varchar2,32),
                            OracleOperation.GetDbParameter("ainparam08",OracleDataType.Varchar2,512),
                            OracleOperation.GetDbParameter("ainparam09",OracleDataType.Varchar2,512),
                            OracleOperation.GetDbParameter("ainparam10",OracleDataType.Varchar2,19),
                            OracleOperation.GetDbParameter("ainparam11",OracleDataType.Varchar2,0),
                            OracleOperation.GetDbParameter("ainparam12",OracleDataType.Varchar2,1024),
                            OracleOperation.GetDbParameter("aoutparam01",OracleDataType.Varchar2,19),
                            OracleOperation.GetDbParameter("errornumber",OracleDataType.Int32,0),
                            OracleOperation.GetDbParameter("errorstring",OracleDataType.Varchar2,200)
                        };
                        orclParameters[0].Value = session.RentInfo.Token;
                        orclParameters[1].Value = newOrgInfo.OrgName;
                        orclParameters[2].Value = newOrgInfo.OrgType;
                        orclParameters[3].Value = newOrgInfo.ParentID;
                        orclParameters[4].Value = newOrgInfo.IsActived;
                        orclParameters[5].Value = newOrgInfo.IsDeleted;
                        orclParameters[6].Value = newOrgInfo.State;
                        orclParameters[7].Value = newOrgInfo.StrStartTime;
                        orclParameters[8].Value = newOrgInfo.StrEndTime;
                        orclParameters[9].Value = newOrgInfo.Creator;
                        orclParameters[10].Value = newOrgInfo.CreateTime.ToUniversalTime().ToString("yyyy/MM/dd HH:mm:ss");
                        orclParameters[11].Value = newOrgInfo.Description;
                        orclParameters[12].Value = serialID;
                        orclParameters[13].Value = errNum;
                        orclParameters[14].Value = errMsg;
                        orclParameters[12].Direction = ParameterDirection.Output;
                        orclParameters[13].Direction = ParameterDirection.Output;
                        orclParameters[14].Direction = ParameterDirection.Output;
                        optReturn = OracleOperation.ExecuteStoredProcedure(session.DBConnectionString, "P_11_010",
                            orclParameters);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        if (orclParameters[13].Value.ToString() != "0")
                        {
                            if (orclParameters[13].Value.ToString() == "-1")
                            {
                                //同名机构名称
                                optReturn.Result = false;
                                optReturn.Code = Defines.RET_DBACCESS_EXIST;
                                optReturn.Message = orclParameters[14].Value.ToString();
                            }
                            else
                            {
                                optReturn.Result = false;
                                optReturn.Code = Defines.RET_DBACCESS_FAIL;
                                optReturn.Message = string.Format("{0}\t{1}", orclParameters[13].Value, orclParameters[14].Value);
                            }
                        }
                        else
                        {
                            ObjID = orclParameters[12].Value.ToString();
                            optReturn.Data = orclParameters[12].Value.ToString();
                        }
                        break;
                }
                OperationReturn opt_R = new OperationReturn();
                opt_R.Result = true;
                opt_R.Code = 0;
                if (ObjID != string.Empty)
                {
                    //将新机构绑给管理员管理
                    opt_R = ManagementRelationParentObject(session, ObjID, ObjID);
                    if (!opt_R.Result) { return opt_R; }
                }
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }

        private OperationReturn DeleteOrgInfo(SessionInfo session, string orgID)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            int errNum = 0;
            string errMsg = string.Empty;
            try
            {
                if (string.IsNullOrEmpty(orgID))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("OrgID param invalid");
                    return optReturn;
                }
                switch (session.DBType)
                {
                    //MSSQL
                    case 2:
                        DbParameter[] mssqlParameters =
                        {
                            MssqlOperation.GetDbParameter("@ainparam01",MssqlDataType.Varchar,5 ),
                            MssqlOperation.GetDbParameter("@ainparam02",MssqlDataType.Varchar,19),
                            MssqlOperation.GetDbParameter("@aouterrornumber",MssqlDataType.Bigint,0),
                            MssqlOperation.GetDbParameter("@aouterrorstring",MssqlDataType.Varchar,200)
                        };
                        mssqlParameters[0].Value = session.RentInfo.Token;
                        mssqlParameters[1].Value = orgID;
                        mssqlParameters[2].Value = errNum;
                        mssqlParameters[3].Value = errMsg;
                        mssqlParameters[2].Direction = ParameterDirection.Output;
                        mssqlParameters[3].Direction = ParameterDirection.Output;
                        optReturn = MssqlOperation.ExecuteStoredProcedure(session.DBConnectionString, "P_11_011",
                           mssqlParameters);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        if (mssqlParameters[2].Value.ToString() != "0")
                        {
                            optReturn.Result = false;
                            optReturn.Code = Defines.RET_DBACCESS_FAIL;
                            optReturn.Message = string.Format("{0}\t{1}", mssqlParameters[2].Value, mssqlParameters[3].Value);
                        }
                        else
                        {
                            optReturn.Data = orgID;
                        }
                        break;
                    //ORCL
                    case 3:
                        DbParameter[] orclParameters =
                        {
                            OracleOperation.GetDbParameter("ainparam01",OracleDataType.Varchar2,5 ),
                            OracleOperation.GetDbParameter("ainparam02",OracleDataType.Varchar2,19),
                            OracleOperation.GetDbParameter("errornumber",OracleDataType.Int32,0),
                            OracleOperation.GetDbParameter("errorstring",OracleDataType.Varchar2,200)
                        };
                        orclParameters[0].Value = session.RentInfo.Token;
                        orclParameters[1].Value = orgID;
                        orclParameters[2].Value = errNum;
                        orclParameters[3].Value = errMsg;
                        orclParameters[2].Direction = ParameterDirection.Output;
                        orclParameters[3].Direction = ParameterDirection.Output;
                        optReturn = OracleOperation.ExecuteStoredProcedure(session.DBConnectionString, "P_11_011",
                            orclParameters);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        if (orclParameters[2].Value.ToString() != "0")
                        {
                            optReturn.Result = false;
                            optReturn.Code = Defines.RET_DBACCESS_FAIL;
                            optReturn.Message = string.Format("{0}\t{1}", orclParameters[2].Value, orclParameters[3].Value);
                        }
                        else
                        {
                            optReturn.Data = orgID;
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }

        private OperationReturn AddNewUser(SessionInfo session, string strUserInfo)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            int errNum = 0;
            long serialID = 0;
            string errMsg = string.Empty;
            try
            {
                optReturn = XMLHelper.DeserializeObject<BasicUserInfo>(strUserInfo);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                BasicUserInfo newUserInfo = optReturn.Data as BasicUserInfo;
                if (newUserInfo == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("NewUserInfo is null");
                    return optReturn;
                }
                newUserInfo.Account = EncryptToDB(newUserInfo.Account);
                newUserInfo.FullName = EncryptToDB(newUserInfo.FullName);
                //newUserInfo.StrStartTime = EncryptToDB(DateTime.Now.ToUniversalTime().ToString("yyy/MM/dd HH:mm:ss"));
                //newUserInfo.StrEndTime = EncryptToDB("UNLIMITED");
                string StarTime = newUserInfo.StrStartTime;
                newUserInfo.StrStartTime = EncryptToDB(newUserInfo.StrStartTime);
                newUserInfo.StrEndTime = EncryptToDB(newUserInfo.StrEndTime);
                string strID = string.Empty;
                switch (session.DBType)
                {
                    //MSSQL
                    case 2:
                        DbParameter[] mssqlParameters =
                        {
                            MssqlOperation.GetDbParameter("@ainparam01",MssqlDataType.Varchar,5 ),
                            MssqlOperation.GetDbParameter("@ainparam02",MssqlDataType.NVarchar,1024),
                            MssqlOperation.GetDbParameter("@ainparam03",MssqlDataType.NVarchar,1024),
                            MssqlOperation.GetDbParameter("@ainparam04",MssqlDataType.Varchar,1024),
                            MssqlOperation.GetDbParameter("@ainparam05",MssqlDataType.Varchar,5),
                            MssqlOperation.GetDbParameter("@ainparam06",MssqlDataType.Varchar,19),
                            MssqlOperation.GetDbParameter("@ainparam07",MssqlDataType.Char,1),
                            MssqlOperation.GetDbParameter("@ainparam08",MssqlDataType.Char,1),
                            MssqlOperation.GetDbParameter("@ainparam09",MssqlDataType.Char,1),
                            MssqlOperation.GetDbParameter("@ainparam10",MssqlDataType.Char,1),
                            MssqlOperation.GetDbParameter("@ainparam11",MssqlDataType.Char,1),
                            MssqlOperation.GetDbParameter("@ainparam12",MssqlDataType.Varchar,32),
                            MssqlOperation.GetDbParameter("@ainparam13",MssqlDataType.Varchar,19),
                            MssqlOperation.GetDbParameter("@ainparam14",MssqlDataType.Varchar,512),
                            MssqlOperation.GetDbParameter("@ainparam15",MssqlDataType.Varchar,512),
                            MssqlOperation.GetDbParameter("@ainparam16",MssqlDataType.Varchar,19),
                            MssqlOperation.GetDbParameter("@ainparam17",MssqlDataType.Varchar,512),
                            MssqlOperation.GetDbParameter("@ainparam18",MssqlDataType.Varchar,20),
                            MssqlOperation.GetDbParameter("@ainparam19",MssqlDataType.Varchar,20),
                            MssqlOperation.GetDbParameter("@ainparam20",MssqlDataType.Varchar,20),
                            MssqlOperation.GetDbParameter("@ainparam21",MssqlDataType.Varchar,20),
                            MssqlOperation.GetDbParameter("@aoutparam01",MssqlDataType.Varchar,19),
                            MssqlOperation.GetDbParameter("@aouterrornumber",MssqlDataType.Bigint,0),
                            MssqlOperation.GetDbParameter("@aouterrorstring",MssqlDataType.Varchar,1024)
                        };
                        mssqlParameters[0].Value = session.RentInfo.Token;
                        mssqlParameters[1].Value = newUserInfo.Account;
                        mssqlParameters[2].Value = newUserInfo.FullName;
                        mssqlParameters[3].Value = string.Empty;
                        mssqlParameters[4].Value = null;
                        mssqlParameters[5].Value = newUserInfo.OrgID;
                        mssqlParameters[6].Value = newUserInfo.SourceFlag;
                        //null;
                        mssqlParameters[7].Value = null;
                        mssqlParameters[8].Value = null;
                        mssqlParameters[9].Value = newUserInfo.IsActived;
                        //null;
                        mssqlParameters[10].Value = null;
                        mssqlParameters[11].Value = null;
                        mssqlParameters[12].Value = null;
                        mssqlParameters[13].Value = newUserInfo.StrStartTime;
                        mssqlParameters[14].Value = newUserInfo.StrEndTime;
                        mssqlParameters[15].Value = session.UserInfo.UserID;
                        //mssqlParameters[16].Value = EncryptToDB(DateTime.Now.ToUniversalTime().ToString("yyyy/MM/dd HH:mm:ss"));
                        mssqlParameters[16].Value = StarTime;
                        //mssqlParameters[17].Value = DateTime.Now.ToUniversalTime().ToString("yyyy/MM/dd HH:mm:ss");
                        mssqlParameters[17].Value = StarTime;
                        mssqlParameters[18].Value = string.Empty;
                        mssqlParameters[19].Value = DateTime.Now.ToUniversalTime().ToString("yyyy/MM/dd HH:mm:ss");
                        mssqlParameters[20].Value = newUserInfo.IsOrgManagement;
                        mssqlParameters[21].Value = serialID;
                        mssqlParameters[22].Value = errNum;
                        mssqlParameters[23].Value = errMsg;
                        mssqlParameters[21].Direction = ParameterDirection.Output;
                        mssqlParameters[22].Direction = ParameterDirection.Output;
                        mssqlParameters[23].Direction = ParameterDirection.Output;
                        optReturn = MssqlOperation.ExecuteStoredProcedure(session.DBConnectionString, "P_11_012",
                            mssqlParameters);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        if (mssqlParameters[22].Value.ToString() != "0")
                        {
                            //帐号已经存在
                            if (mssqlParameters[22].Value.ToString() == "-1")
                            {
                                optReturn.Result = false;
                                optReturn.Code = Defines.RET_DBACCESS_EXIST;
                                optReturn.Message = mssqlParameters[23].Value.ToString();
                            }
                            else
                            {
                                optReturn.Result = false;
                                optReturn.Code = Defines.RET_DBACCESS_FAIL;
                                optReturn.Message = string.Format("{0}\t{1}", mssqlParameters[22].Value, mssqlParameters[23].Value);
                            }
                        }
                        else
                        {
                            strID = mssqlParameters[21].Value.ToString();

                            //修改密码
                            List<string> listParams = new List<string>();
                            listParams.Add(strID);
                            listParams.Add("0");
                            listParams.Add(string.Empty);
                            optReturn = ModifyUserPassword(session, listParams);
                            if (!optReturn.Result)
                            {
                                return optReturn;
                            }
                            optReturn.Data = strID;
                        }
                        if (newUserInfo.IsOrgManagement == "1")
                        {
                            optReturn = ManagementRelationChildObject(session, newUserInfo.OrgID.ToString(), strID);
                            if (!optReturn.Result) { return optReturn; }
                        }
                        else
                        {
                            //该用户不是机构管理者，暂不做任何处理。若之前是部门管理者，则保留用户管理对象内容
                            //将添加对象绑给管理员管理
                            OperationReturn opt_Temp = new OperationReturn();
                            opt_Temp.Result = true;
                            opt_Temp.Code = 0;
                            opt_Temp = ManagementRelationParentObject(session, newUserInfo.OrgID.ToString(), strID);
                            if (!opt_Temp.Result) { return opt_Temp; }
                        }
                        break;
                    //ORCL
                    case 3:
                        DbParameter[] dbParameters =
                        {
                            OracleOperation.GetDbParameter("ainparam01",OracleDataType.Varchar2,5 ),
                            OracleOperation.GetDbParameter("ainparam02",OracleDataType.Nvarchar2,1024),
                            OracleOperation.GetDbParameter("ainparam03",OracleDataType.Nvarchar2,1024),
                            OracleOperation.GetDbParameter("ainparam04",OracleDataType.Varchar2,1024),
                            OracleOperation.GetDbParameter("ainparam05",OracleDataType.Varchar2,5),
                            OracleOperation.GetDbParameter("ainparam06",OracleDataType.Varchar2,19),
                            OracleOperation.GetDbParameter("ainparam07",OracleDataType.Char,1),
                            OracleOperation.GetDbParameter("ainparam08",OracleDataType.Char,1),
                            OracleOperation.GetDbParameter("ainparam09",OracleDataType.Char,1),
                            OracleOperation.GetDbParameter("ainparam10",OracleDataType.Char,1),
                            OracleOperation.GetDbParameter("ainparam11",OracleDataType.Char,1),
                            OracleOperation.GetDbParameter("ainparam12",OracleDataType.Varchar2,32),
                            OracleOperation.GetDbParameter("ainparam13",OracleDataType.Date,0),
                            OracleOperation.GetDbParameter("ainparam14",OracleDataType.Varchar2,512),
                            OracleOperation.GetDbParameter("ainparam15",OracleDataType.Varchar2,512),
                            OracleOperation.GetDbParameter("ainparam16",OracleDataType.Varchar2,19),
                            OracleOperation.GetDbParameter("ainparam17",OracleDataType.Varchar2,512),
                            OracleOperation.GetDbParameter("ainparam18",OracleDataType.Varchar2,20),
                            OracleOperation.GetDbParameter("ainparam19",OracleDataType.Varchar2,20),
                            OracleOperation.GetDbParameter("ainparam20",OracleDataType.Varchar2,20),
                            OracleOperation.GetDbParameter("ainparam21",OracleDataType.Varchar2,20),
                            OracleOperation.GetDbParameter("aoutparam01",OracleDataType.Varchar2,19),
                            OracleOperation.GetDbParameter("errornumber",OracleDataType.Int32,0),
                            OracleOperation.GetDbParameter("errorstring",OracleDataType.Varchar2,1024)
                        };
                        dbParameters[0].Value = session.RentInfo.Token;
                        dbParameters[1].Value = newUserInfo.Account;
                        dbParameters[2].Value = newUserInfo.FullName;
                        dbParameters[3].Value = null;
                        dbParameters[4].Value = null;
                        dbParameters[5].Value = newUserInfo.OrgID;
                        dbParameters[6].Value = newUserInfo.SourceFlag;
                        dbParameters[7].Value = null;
                        dbParameters[8].Value = null;
                        dbParameters[9].Value = newUserInfo.IsActived;
                        dbParameters[10].Value = null;
                        dbParameters[11].Value = null;
                        dbParameters[12].Value = null;
                        dbParameters[13].Value = newUserInfo.StrStartTime;
                        dbParameters[14].Value = newUserInfo.StrEndTime;
                        dbParameters[15].Value = session.UserInfo.UserID;
                        //dbParameters[16].Value = EncryptToDB(DateTime.Now.ToUniversalTime().ToString("yyy/MM/dd HH:mm:ss"));
                        dbParameters[16].Value = StarTime;
                        //dbParameters[17].Value = DateTime.Now.ToUniversalTime().ToString("yyy/MM/dd HH:mm:ss");
                        dbParameters[17].Value = StarTime;
                        dbParameters[18].Value = null;
                        dbParameters[19].Value = DateTime.Now.ToUniversalTime().ToString("yyyy/MM/dd HH:mm:ss");
                        dbParameters[20].Value = newUserInfo.IsOrgManagement;
                        dbParameters[21].Value = serialID;
                        dbParameters[22].Value = errNum;
                        dbParameters[23].Value = errMsg;
                        dbParameters[21].Direction = ParameterDirection.Output;
                        dbParameters[22].Direction = ParameterDirection.Output;
                        dbParameters[23].Direction = ParameterDirection.Output;
                        optReturn = OracleOperation.ExecuteStoredProcedure(session.DBConnectionString, "P_11_012",
                            dbParameters);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        if (dbParameters[22].Value.ToString() != "0")
                        {
                            //帐号已经存在
                            if (dbParameters[22].Value.ToString() == "-1")
                            {
                                optReturn.Result = false;
                                optReturn.Code = Defines.RET_DBACCESS_EXIST;
                                optReturn.Message = dbParameters[23].Value.ToString();
                            }
                            else
                            {
                                optReturn.Result = false;
                                optReturn.Code = Defines.RET_DBACCESS_FAIL;
                                optReturn.Message = string.Format("{0}\t{1}", dbParameters[22].Value, dbParameters[23].Value);
                            }
                        }
                        else
                        {
                            strID = dbParameters[21].Value.ToString();

                            //修改密码
                            List<string> listParams = new List<string>();
                            listParams.Add(strID);
                            listParams.Add("0");
                            listParams.Add(string.Empty);
                            optReturn = ModifyUserPassword(session, listParams);
                            if (!optReturn.Result)
                            {
                                return optReturn;
                            }
                            optReturn.Data = strID;
                        }
                        break;
                }
                if (newUserInfo.IsOrgManagement == "1")
                {
                    optReturn = ManagementRelationChildObject(session, newUserInfo.OrgID.ToString(), strID);
                    if (!optReturn.Result) { return optReturn; }
                }
                else
                {
                    //该用户不是机构管理者，暂不做任何处理。若之前是部门管理者，则保留用户管理对象内容
                    //将添加对象绑给管理员管理
                    //OperationReturn opt_Temp = new OperationReturn();
                    //opt_Temp.Result = true;
                    //opt_Temp.Code = 0;
                    //opt_Temp = ManagementRelationParentObject(session, newUserInfo.OrgID.ToString(), strID);
                    //if (!opt_Temp.Result) {
                    //    return opt_Temp; }
                }
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }

        private OperationReturn DeleteUserInfo(SessionInfo session, string strUserID)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            int errNum = 0;
            string errMsg = string.Empty;
            try
            {
                if (string.IsNullOrEmpty(strUserID))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("UserID param invalid");
                    return optReturn;
                }
                string[] listUserID = strUserID.Split(new[] { ConstValue.SPLITER_CHAR },
                    StringSplitOptions.RemoveEmptyEntries);
                switch (session.DBType)
                {
                    //MSSQL
                    case 2:
                        for (int i = 0; i < listUserID.Length; i++)
                        {
                            string userID = listUserID[i];
                            DbParameter[] mssqlParameters =
                            {
                                MssqlOperation.GetDbParameter("@ainparam01", MssqlDataType.Varchar, 5),
                                MssqlOperation.GetDbParameter("@ainparam02", MssqlDataType.Varchar, 19),
                                MssqlOperation.GetDbParameter("@aouterrornumber", MssqlDataType.Bigint, 0),
                                MssqlOperation.GetDbParameter("@aouterrorstring", MssqlDataType.Varchar, 200)
                            };
                            mssqlParameters[0].Value = session.RentInfo.Token;
                            mssqlParameters[1].Value = userID;
                            mssqlParameters[2].Value = errNum;
                            mssqlParameters[3].Value = errMsg;
                            mssqlParameters[2].Direction = ParameterDirection.Output;
                            mssqlParameters[3].Direction = ParameterDirection.Output;
                            optReturn = MssqlOperation.ExecuteStoredProcedure(session.DBConnectionString,
                                "P_11_013",
                                mssqlParameters);
                            if (!optReturn.Result)
                            {
                                return optReturn;
                            }
                            if (mssqlParameters[2].Value.ToString() != "0")
                            {
                                optReturn.Result = false;
                                optReturn.Code = Defines.RET_DBACCESS_FAIL;
                                optReturn.Message = string.Format("{0}\t{1}", mssqlParameters[2].Value, mssqlParameters[3].Value);
                                return optReturn;
                            }
                            optReturn.Data = strUserID;
                        }
                        break;
                    //ORCL
                    case 3:
                        for (int i = 0; i < listUserID.Length; i++)
                        {
                            string userID = listUserID[i];
                            DbParameter[] dbParameters =
                            {
                                OracleOperation.GetDbParameter("ainparam01", OracleDataType.Varchar2, 5),
                                OracleOperation.GetDbParameter("ainparam02", OracleDataType.Varchar2, 19),
                                OracleOperation.GetDbParameter("errornumber", OracleDataType.Int32, 0),
                                OracleOperation.GetDbParameter("errorstring", OracleDataType.Varchar2, 200)
                            };
                            dbParameters[0].Value = session.RentInfo.Token;
                            dbParameters[1].Value = userID;
                            dbParameters[2].Value = errNum;
                            dbParameters[3].Value = errMsg;
                            dbParameters[2].Direction = ParameterDirection.Output;
                            dbParameters[3].Direction = ParameterDirection.Output;
                            optReturn = OracleOperation.ExecuteStoredProcedure(session.DBConnectionString,
                                "P_11_013",
                                dbParameters);
                            if (!optReturn.Result)
                            {
                                return optReturn;
                            }
                            if (dbParameters[2].Value.ToString() != "0")
                            {
                                optReturn.Result = false;
                                optReturn.Code = Defines.RET_DBACCESS_FAIL;
                                optReturn.Message = string.Format("{0}\t{1}", dbParameters[2].Value, dbParameters[3].Value);
                                return optReturn;
                            }
                            optReturn.Data = strUserID;
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }

        private OperationReturn LoadNewUser(SessionInfo session, string strUserInfo)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            int errNum = 0;
            long serialID = 0;
            string errMsg = string.Empty;
            try
            {
                optReturn = XMLHelper.DeserializeObject<BasicUserInfo>(strUserInfo);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                BasicUserInfo newUserInfo = optReturn.Data as BasicUserInfo;
                if (newUserInfo == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("NewUserInfo is null");
                    return optReturn;
                }
                newUserInfo.Account = EncryptToDB(newUserInfo.Account);
                newUserInfo.FullName = EncryptToDB(newUserInfo.FullName);
                //newUserInfo.StrStartTime = EncryptToDB(DateTime.Now.ToUniversalTime().ToString("yyy/MM/dd HH:mm:ss"));
                //newUserInfo.StrEndTime = EncryptToDB("UNLIMITED");
                string StarTime = newUserInfo.StrStartTime;
                newUserInfo.StrStartTime = EncryptToDB(newUserInfo.StrStartTime);
                newUserInfo.StrEndTime = EncryptToDB(newUserInfo.StrEndTime);
                switch (session.DBType)
                {
                    //MSSQL
                    case 2:
                        DbParameter[] mssqlParameters =
                        {
                            MssqlOperation.GetDbParameter("@ainparam01",MssqlDataType.Varchar,5 ),
                            MssqlOperation.GetDbParameter("@ainparam02",MssqlDataType.NVarchar,1024),
                            MssqlOperation.GetDbParameter("@ainparam03",MssqlDataType.NVarchar,1024),
                            MssqlOperation.GetDbParameter("@ainparam04",MssqlDataType.Varchar,1024),
                            MssqlOperation.GetDbParameter("@ainparam05",MssqlDataType.Varchar,5),
                            MssqlOperation.GetDbParameter("@ainparam06",MssqlDataType.Varchar,19),
                            MssqlOperation.GetDbParameter("@ainparam07",MssqlDataType.Char,1),
                            MssqlOperation.GetDbParameter("@ainparam08",MssqlDataType.Char,1),
                            MssqlOperation.GetDbParameter("@ainparam09",MssqlDataType.Char,1),
                            MssqlOperation.GetDbParameter("@ainparam10",MssqlDataType.Char,1),
                            MssqlOperation.GetDbParameter("@ainparam11",MssqlDataType.Char,1),
                            MssqlOperation.GetDbParameter("@ainparam12",MssqlDataType.Varchar,32),
                            MssqlOperation.GetDbParameter("@ainparam13",MssqlDataType.Varchar,19),
                            MssqlOperation.GetDbParameter("@ainparam14",MssqlDataType.Varchar,512),
                            MssqlOperation.GetDbParameter("@ainparam15",MssqlDataType.Varchar,512),
                            MssqlOperation.GetDbParameter("@ainparam16",MssqlDataType.Varchar,19),
                            MssqlOperation.GetDbParameter("@ainparam17",MssqlDataType.Varchar,512),
                            MssqlOperation.GetDbParameter("@ainparam18",MssqlDataType.Varchar,20),
                            MssqlOperation.GetDbParameter("@ainparam19",MssqlDataType.Varchar,20),
                            MssqlOperation.GetDbParameter("@ainparam20",MssqlDataType.Varchar,20),
                            MssqlOperation.GetDbParameter("@aoutparam01",MssqlDataType.Varchar,19),
                            MssqlOperation.GetDbParameter("@aouterrornumber",MssqlDataType.Bigint,0),
                            MssqlOperation.GetDbParameter("@aouterrorstring",MssqlDataType.Varchar,1024)
                        };
                        mssqlParameters[0].Value = session.RentInfo.Token;
                        mssqlParameters[1].Value = newUserInfo.Account;
                        mssqlParameters[2].Value = newUserInfo.FullName;
                        mssqlParameters[3].Value = string.Empty;
                        mssqlParameters[4].Value = null;
                        mssqlParameters[5].Value = newUserInfo.OrgID;
                        mssqlParameters[6].Value = newUserInfo.SourceFlag;
                        //null;
                        mssqlParameters[7].Value = null;
                        mssqlParameters[8].Value = null;
                        mssqlParameters[9].Value = newUserInfo.IsActived;
                        //null;
                        mssqlParameters[10].Value = null;
                        mssqlParameters[11].Value = null;
                        mssqlParameters[12].Value = null;
                        mssqlParameters[13].Value = newUserInfo.StrStartTime;
                        mssqlParameters[14].Value = newUserInfo.StrEndTime;
                        mssqlParameters[15].Value = session.UserInfo.UserID;
                        //mssqlParameters[16].Value = EncryptToDB(DateTime.Now.ToUniversalTime().ToString("yyy/MM/dd HH:mm:ss"));
                        mssqlParameters[16].Value = StarTime;
                        //mssqlParameters[17].Value = DateTime.Now.ToUniversalTime().ToString("yyy/MM/dd HH:mm:ss");
                        mssqlParameters[17].Value = StarTime;
                        mssqlParameters[18].Value = string.Empty;
                        mssqlParameters[19].Value = DateTime.Now.ToUniversalTime().ToString("yyy/MM/dd HH:mm:ss");
                        mssqlParameters[20].Value = serialID;
                        mssqlParameters[21].Value = errNum;
                        mssqlParameters[22].Value = errMsg;
                        mssqlParameters[20].Direction = ParameterDirection.Output;
                        mssqlParameters[21].Direction = ParameterDirection.Output;
                        mssqlParameters[22].Direction = ParameterDirection.Output;
                        optReturn = MssqlOperation.ExecuteStoredProcedure(session.DBConnectionString, "P_11_018",
                            mssqlParameters);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        if (mssqlParameters[21].Value.ToString() != "0")
                        {
                            //帐号已经存在
                            if (mssqlParameters[21].Value.ToString() == "-1")
                            {
                                optReturn.Result = false;
                                optReturn.Code = Defines.RET_DBACCESS_EXIST;
                                optReturn.Message = mssqlParameters[22].Value.ToString();
                            }
                            else
                            {
                                optReturn.Result = false;
                                optReturn.Code = Defines.RET_DBACCESS_FAIL;
                                optReturn.Message = string.Format("{0}\t{1}", mssqlParameters[21].Value, mssqlParameters[22].Value);
                            }
                        }
                        else
                        {
                            string strID = mssqlParameters[20].Value.ToString();

                            //修改密码
                            List<string> listParams = new List<string>();
                            listParams.Add(strID);
                            if (newUserInfo.Password == null || newUserInfo.Password == string.Empty)
                            {
                                listParams.Add("0");
                                listParams.Add(string.Empty);
                            }
                            else
                            {
                                listParams.Add("1");
                                listParams.Add(newUserInfo.Password);
                            }
                            optReturn = ModifyUserPassword(session, listParams);
                            if (!optReturn.Result)
                            {
                                return optReturn;
                            }
                            optReturn.Data = strID;
                        }
                        break;
                    //ORCL
                    case 3:
                        DbParameter[] dbParameters =
                        {
                            OracleOperation.GetDbParameter("ainparam01",OracleDataType.Varchar2,5 ),
                            OracleOperation.GetDbParameter("ainparam02",OracleDataType.Nvarchar2,1024),
                            OracleOperation.GetDbParameter("ainparam03",OracleDataType.Nvarchar2,1024),
                            OracleOperation.GetDbParameter("ainparam04",OracleDataType.Varchar2,1024),
                            OracleOperation.GetDbParameter("ainparam05",OracleDataType.Varchar2,5),
                            OracleOperation.GetDbParameter("ainparam06",OracleDataType.Varchar2,19),
                            OracleOperation.GetDbParameter("ainparam07",OracleDataType.Char,1),
                            OracleOperation.GetDbParameter("ainparam08",OracleDataType.Char,1),
                            OracleOperation.GetDbParameter("ainparam09",OracleDataType.Char,1),
                            OracleOperation.GetDbParameter("ainparam10",OracleDataType.Char,1),
                            OracleOperation.GetDbParameter("ainparam11",OracleDataType.Char,1),
                            OracleOperation.GetDbParameter("ainparam12",OracleDataType.Varchar2,32),
                            OracleOperation.GetDbParameter("ainparam13",OracleDataType.Date,0),
                            OracleOperation.GetDbParameter("ainparam14",OracleDataType.Varchar2,512),
                            OracleOperation.GetDbParameter("ainparam15",OracleDataType.Varchar2,512),
                            OracleOperation.GetDbParameter("ainparam16",OracleDataType.Varchar2,19),
                            OracleOperation.GetDbParameter("ainparam17",OracleDataType.Varchar2,512),
                            OracleOperation.GetDbParameter("ainparam18",OracleDataType.Varchar2,20),
                            OracleOperation.GetDbParameter("ainparam19",OracleDataType.Varchar2,20),
                            OracleOperation.GetDbParameter("ainparam20",OracleDataType.Varchar2,20),
                            OracleOperation.GetDbParameter("aoutparam01",OracleDataType.Varchar2,19),
                            OracleOperation.GetDbParameter("errornumber",OracleDataType.Int32,0),
                            OracleOperation.GetDbParameter("errorstring",OracleDataType.Varchar2,1024)
                        };
                        dbParameters[0].Value = session.RentInfo.Token;
                        dbParameters[1].Value = newUserInfo.Account;
                        dbParameters[2].Value = newUserInfo.FullName;
                        dbParameters[3].Value = null;
                        dbParameters[4].Value = null;
                        dbParameters[5].Value = newUserInfo.OrgID;
                        dbParameters[6].Value = newUserInfo.SourceFlag;
                        dbParameters[7].Value = null;
                        dbParameters[8].Value = null;
                        dbParameters[9].Value = newUserInfo.IsActived;
                        dbParameters[10].Value = null;
                        dbParameters[11].Value = null;
                        dbParameters[12].Value = null;
                        dbParameters[13].Value = newUserInfo.StrStartTime;
                        dbParameters[14].Value = newUserInfo.StrEndTime;
                        dbParameters[15].Value = session.UserInfo.UserID;
                        //dbParameters[16].Value = EncryptToDB(DateTime.Now.ToUniversalTime().ToString("yyy/MM/dd HH:mm:ss"));
                        dbParameters[16].Value = StarTime;
                        //dbParameters[17].Value = DateTime.Now.ToUniversalTime().ToString("yyy/MM/dd HH:mm:ss");
                        dbParameters[17].Value = StarTime;
                        dbParameters[18].Value = null;
                        dbParameters[19].Value = DateTime.Now.ToUniversalTime().ToString("yyy/MM/dd HH:mm:ss");
                        dbParameters[20].Value = serialID;
                        dbParameters[21].Value = errNum;
                        dbParameters[22].Value = errMsg;
                        dbParameters[20].Direction = ParameterDirection.Output;
                        dbParameters[21].Direction = ParameterDirection.Output;
                        dbParameters[22].Direction = ParameterDirection.Output;
                        optReturn = OracleOperation.ExecuteStoredProcedure(session.DBConnectionString, "P_11_018",
                            dbParameters);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        if (dbParameters[21].Value.ToString() != "0")
                        {
                            //帐号已经存在
                            if (dbParameters[21].Value.ToString() == "-1")
                            {
                                optReturn.Result = false;
                                optReturn.Code = Defines.RET_DBACCESS_EXIST;
                                optReturn.Message = dbParameters[22].Value.ToString();
                            }
                            else
                            {
                                optReturn.Result = false;
                                optReturn.Code = Defines.RET_DBACCESS_FAIL;
                                optReturn.Message = string.Format("{0}\t{1}", dbParameters[21].Value, dbParameters[22].Value);
                            }
                        }
                        else
                        {
                            string strID = dbParameters[20].Value.ToString();

                            //修改密码
                            List<string> listParams = new List<string>();
                            listParams.Add(strID);
                            if (newUserInfo.Password == null || newUserInfo.Password == string.Empty)
                            {
                                listParams.Add("0");
                                listParams.Add(string.Empty);
                            }
                            else
                            {
                                listParams.Add("1");
                                listParams.Add(newUserInfo.Password);
                            }
                            optReturn = ModifyUserPassword(session, listParams);
                            if (!optReturn.Result)
                            {
                                return optReturn;
                            }
                            optReturn.Data = strID;
                        }
                        break;
                }
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