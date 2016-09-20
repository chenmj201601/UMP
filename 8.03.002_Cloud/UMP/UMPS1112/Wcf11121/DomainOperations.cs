using Common11121;
using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;
using VoiceCyber.VCLDAP;
using Wcf11121.Wcf11012;

namespace Wcf11121
{
    public partial class Service11121
    {
        private OperationReturn GetDomainInfo(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0      用户编号
                //if (listParams == null || listParams.Count < 1)
                //{
                //    optReturn.Result = false;
                //    optReturn.Code = Defines.RET_PARAM_INVALID;
                //    optReturn.Message = string.Format("Request param is null or count invalid");
                //    return optReturn;
                //}

                string rentToken = session.RentInfo.Token;
                string strSql;
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:

                        strSql = string.Format("SELECT * FROM T_00_012 WHERE C002={0}", rentToken);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;

                        break;
                    case 3:

                        strSql = string.Format("SELECT * FROM T_00_012 WHERE C002={0}", rentToken);
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;

                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("Database type not surpport.\t{0}", session.DBType);
                        return optReturn;
                }
                if (objDataSet == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("DataSet is null");
                    return optReturn;
                }
                List<string> listReturn = new List<string>();
                for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = objDataSet.Tables[0].Rows[i];
                    BasicDomainInfo item = new BasicDomainInfo();

                    item.RentID = Convert.ToInt64(dr["C002"]);
                    item.DomainID = Convert.ToInt64(dr["C001"]);
                    item.DomainName = DecryptFromDB102(dr["C003"].ToString().Trim()).Trim();
                    item.DomainCode = Convert.ToInt32(dr["C004"]);
                    item.DomainUserName = DecryptFromDB102(dr["C005"].ToString().Trim()).Trim();
                    item.DomainUserPassWord = DecryptFromDB103(dr["C006"].ToString().Trim()).Trim();
                    item.RootDirectory = dr["C007"].ToString();
                    item.IsActive = dr["C008"].ToString() == "1";
                    item.IsActiveLogin = dr["C010"].ToString() == "1";
                    item.IsDelete = dr["C009"].ToString() == "1";
                    item.Creator = Convert.ToInt64(dr["C011"].ToString());
                    item.CreatTime = dr["C012"].ToString();
                    item.Description = dr["C999"].ToString();

                    optReturn = XMLHelper.SeriallizeObject(item);
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                    listReturn.Add(optReturn.Data.ToString());
                }

                optReturn.Data = listReturn;
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

        private OperationReturn SaveDomainInfo(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            string Mes = string.Empty;
            try
            {
                //ListParam
                //0      操作编号：0：删除；1：添加；2：修改
                //1      被操作的对象DomainInfo类系列化的string
                if (listParams == null || listParams.Count < 2)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string OperationCode = listParams[0];
                optReturn = XMLHelper.DeserializeObject<BasicDomainInfo>(listParams[1]);
                BasicDomainInfo DI = optReturn.Data as BasicDomainInfo;
                string domainName = EncryptToDB002(DI.DomainName);
                string rentToken = session.RentInfo.Token;
                string strSql = string.Empty;
                DataSet objDataSet;
                
                if (OperationCode == "1")
                {
                    switch (session.DBType)
                    {
                        //MSSQL
                        case 2:
                            using (SqlConnection connection = new SqlConnection(session.DBConnectionString))
                            {
                                DataSet dataSet = new DataSet();
                                connection.Open();
                                strSql = string.Format("SELECT * FROM T_00_012 WHERE C002 = '{0}' AND C009=0 ORDER BY C004", rentToken);
                                SqlDataAdapter sqlDA = new SqlDataAdapter(strSql, connection);
                                sqlDA.Fill(dataSet);
                                //设置主键
                                //dataSet.Tables[0].PrimaryKey = new DataColumn[] { dataSet.Tables[0].Columns[0] };
                                SqlCommandBuilder sqlCB = new SqlCommandBuilder(sqlDA);                                
                                sqlDA.InsertCommand = sqlCB.GetInsertCommand();                                
                                int drCurrent = dataSet.Tables[0].Select(string.Format("C003 = '{0}'", domainName)).Count() > 0 ? 1 : 0;
                                
                                if (drCurrent == 0)
                                {
                                    optReturn = SaveNewDomainInfo(dataSet, session, DI);
                                    Mes += "SaveNewDomainInfo ok/";
                                    if (optReturn.Result)
                                    {
                                        dataSet = optReturn.Data as DataSet;
                                        sqlDA.Update(dataSet);
                                        dataSet.AcceptChanges();
                                    }
                                }
                                else
                                {
                                    sqlDA.Dispose();
                                    connection.Close();
                                    //如果重名，返回报错提醒
                                    optReturn.Result = false;
                                    optReturn.Code = 99;
                                    return optReturn;
                                }
                                sqlDA.Dispose();
                                connection.Close();
                            }
                            break;
                        //ORCL
                        case 3:
                            using (OracleConnection connection = new OracleConnection(session.DBConnectionString))
                            {
                                DataSet dataSet = new DataSet();
                                connection.Open();
                                strSql = string.Format("SELECT * FROM T_00_012 WHERE C002 = '{0}' AND C009=0 ORDER BY C004", rentToken);
                                OracleDataAdapter oracleDA = new OracleDataAdapter(strSql, connection);
                                oracleDA.Fill(dataSet);
                                //设置主键
                                //dataSet.Tables[0].PrimaryKey = new DataColumn[] { dataSet.Tables[0].Columns[0] };
                                OracleCommandBuilder oracleCB = new OracleCommandBuilder(oracleDA);
                                oracleDA.InsertCommand = oracleCB.GetInsertCommand();
                                int drCurrent = dataSet.Tables[0].Select(string.Format("C003='{0}'", domainName)).Count() > 0 ? 1 : 0;
                                //dataSet.Tables[0].Rows.Find(newRole.RoleID.ToString());
                                if (drCurrent == 0)
                                {
                                    optReturn = SaveNewDomainInfo(dataSet, session, DI);
                                    if (optReturn.Result)
                                    {
                                        dataSet = optReturn.Data as DataSet;
                                        oracleDA.Update(dataSet);
                                        dataSet.AcceptChanges();
                                    }
                                }
                                else
                                {
                                    oracleDA.Update(dataSet);
                                    dataSet.AcceptChanges();
                                    //如果重名，返回报错提醒
                                    optReturn.Result = false;
                                    optReturn.Code = 99;
                                    return optReturn;
                                }
                                oracleDA.Dispose();
                                connection.Close();
                            }
                            break;
                        default:
                            break;
                    }                   
                }
                else
                {
                    switch (OperationCode)
                    {
                        case "0"://删除
                            strSql = string.Format("UPDATE T_00_012 SET C009=1 WHERE C001={1} AND C002='{0}'", rentToken, DI.DomainID);
                            break;
                        case "2"://修改
                            strSql = string.Format("UPDATE T_00_012 SET C003='{2}',C005='{3}',C006='{4}',C010='{5}',C999='{6}' WHERE C001={1} AND C002='{0}'"
                                , rentToken, DI.DomainID
                                , EncryptToDB002(DI.DomainName.Trim()).Trim()
                                , EncryptToDB002(DI.DomainUserName.Trim()).Trim()
                                , EncryptToDB003(string.Format("{0}@{1}", DI.DomainID.ToString().Trim(), DI.DomainUserPassWord.Trim())).Trim()
                                , DI.IsActiveLogin ? 1 : 0, DI.Description);
                            break;
                    }
                    switch (session.DBType)
                    {
                        case 2:
                            optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                            if (!optReturn.Result)
                            {
                                return optReturn;
                            }
                            objDataSet = optReturn.Data as DataSet;

                            break;
                        case 3:
                            optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                            if (!optReturn.Result)
                            {
                                return optReturn;
                            }
                            objDataSet = optReturn.Data as DataSet;

                            break;
                        default:
                            optReturn.Result = false;
                            optReturn.Code = Defines.RET_PARAM_INVALID;
                            optReturn.Message = string.Format("Database type not surpport.\t{0}", session.DBType);
                            return optReturn;
                    }
                    if (objDataSet == null)
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_OBJECT_NULL;
                        optReturn.Message = string.Format("DataSet is null");
                        return optReturn;
                    }
                }
               
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

        private OperationReturn SaveNewDomainInfo(DataSet dataSet, SessionInfo session,BasicDomainInfo DI)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //如果没有重名，则添加到数据库
                WebRequest webRequest = new WebRequest();
                webRequest.Session = session;
                webRequest.Code = (int)RequestCode.WSGetSerialID;
                webRequest.ListData.Add("11");
                webRequest.ListData.Add("110");
                webRequest.ListData.Add(DateTime.Now.ToString("yyyyMMddHHmmss"));
                Service11012Client client = new Service11012Client(WebHelper.CreateBasicHttpBinding(session),
                    WebHelper.CreateEndpointAddress(session.AppServerInfo, "Service11012"));
                WebReturn webReturn = client.DoOperation(webRequest);
                client.Close();
                if (!webReturn.Result)
                {
                    optReturn.Result = false;
                    optReturn.Code = webReturn.Code;
                    optReturn.Message = webReturn.Message;
                    return optReturn;
                }
                string strNewID = webReturn.Data;
                if (string.IsNullOrEmpty(strNewID))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_STRING_EMPTY;
                    optReturn.Message = string.Format("New Domain ID is empty");
                    return optReturn;
                }
                int dex = dataSet.Tables[0].Rows.Count;
                DataRow NewRow = dataSet.Tables[0].NewRow();
                NewRow["C001"] = strNewID;
                NewRow["C002"] = session.RentInfo.Token;
                NewRow["C003"] = EncryptToDB002(DI.DomainName.Trim());
                NewRow["C004"] = dex.ToString();
                NewRow["C005"] = EncryptToDB002(DI.DomainUserName.Trim());
                NewRow["C006"] = EncryptToDB003(string.Format("{0}@{1}", strNewID, DI.DomainUserPassWord.Trim()).Trim());
                NewRow["C007"] = null;
                NewRow["C008"] = "1";
                NewRow["C009"] = "0";
                NewRow["C010"] = DI.IsActiveLogin ? 1 : 0;
                NewRow["C011"] = session.UserID.ToString();
                NewRow["C012"] = DateTime.Now.ToUniversalTime().ToString("yyyyMMddHHmmss");
                NewRow["C999"] = DI.Description;
                dataSet.Tables[0].Rows.Add(NewRow);
                optReturn.Data = dataSet;
                return optReturn;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = 88;
                optReturn.Message = string.Format("SaveNewDomainInfo Fail:{0}",ex.Message);
                return optReturn;
            }
        }

        private OperationReturn CheckDomainInfo(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0      域连接串串
                //1      域登录账号
                //2      域登录密码
                if (listParams == null || listParams.Count < 3)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string IStrADPath = listParams[0];
                string username = listParams[1];
                string password = listParams[2];
                ADUtility util = new ADUtility(IStrADPath, username, password);

                ADUserCollection users = util.GetAllUsers();
                if (users == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_FAIL;
                    optReturn.Message = "Users null";
                    return optReturn;
                }
                else
                {
                    optReturn.Result = true;
                    optReturn.Code = (int)S1112Codes.CheckDomainInfo;
                    optReturn.Message = "Check OK";
                    return optReturn;
                }
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
                return optReturn;
            }
        }

    }
        
}