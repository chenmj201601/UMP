using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using Common3105;
using PFShareClassesS;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;
using Wcf31051.Wcf11012;
using VoiceCyber.UMP.ScoreSheets;

namespace Wcf31051
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码、svc 和配置文件中的类名“Service1”。
    // 注意: 为了启动 WCF 测试客户端以测试此服务，请在解决方案资源管理器中选择 Service1.svc 或 Service1.svc.cs，然后开始调试。
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class Service31051 : IService31051
    {
        #region Encryption and Decryption
        private string CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType keyIVID)
        {
            string strReturn;
            int LIntRand;
            Random random = new Random();
            string strTemp;

            try
            {
                strReturn = DateTime.Now.ToString("yyyyMMddHHmmss");
                LIntRand = random.Next(0, 14);
                strTemp = LIntRand.ToString("00");
                strReturn = strReturn.Insert(LIntRand, "VCT");
                LIntRand = random.Next(0, 17);
                strTemp += LIntRand.ToString("00");
                strReturn = strReturn.Insert(LIntRand, "UMP");
                LIntRand = random.Next(0, 20);
                strTemp += LIntRand.ToString("00");
                strReturn = strReturn.Insert(LIntRand, ((int)keyIVID).ToString("000"));

                strReturn = EncryptionAndDecryption.EncryptStringY(strTemp + strReturn);
            }
            catch { strReturn = string.Empty; }

            return strReturn;
        }

        private string EncryptDecryptToDB(string strSource)
        {
            string strTemp = EncryptionAndDecryption.EncryptDecryptString(strSource,
               CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104),
               EncryptionAndDecryption.UMPKeyAndIVType.M104);
            string strReturn = EncryptionAndDecryption.EncryptDecryptString(strTemp,
                CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M002),
                EncryptionAndDecryption.UMPKeyAndIVType.M002);
            return strReturn;
        }

        private string EncryptDecryptFromDB(string strSource)
        {
            string strTemp = EncryptionAndDecryption.EncryptDecryptString(strSource,
                CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M102),
                EncryptionAndDecryption.UMPKeyAndIVType.M102);
            string strReturn = EncryptionAndDecryption.EncryptDecryptString(strTemp,
                CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004),
                EncryptionAndDecryption.UMPKeyAndIVType.M004);
            return strReturn;
        }

        private string DecryptNamesFromDB(string strSource)
        {
            string strTemp = EncryptionAndDecryption.EncryptDecryptString(strSource,
                CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M102),
                EncryptionAndDecryption.UMPKeyAndIVType.M102);
            return strTemp;
        }

        private string DecryptFromClient(string strSource)
        {
            string strTemp = EncryptionAndDecryption.EncryptDecryptString(strSource,
               CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104),
               EncryptionAndDecryption.UMPKeyAndIVType.M104);
            return strTemp;
        }

        #endregion

        public WebReturn UMPTaskOperation(WebRequest webRequest)
        {
            WebReturn webReturn = new WebReturn();
            if (webRequest == null)
            {
                webReturn.Result = false;
                webReturn.Code = Defines.RET_PARAM_INVALID;
                webReturn.Message = string.Format("WebRequest is null");
                return webReturn;
            }
            SessionInfo session = webRequest.Session;
            if (session == null)
            {
                webReturn.Result = false;
                webReturn.Code = Defines.RET_PARAM_INVALID;
                webReturn.Message = string.Format("SessionInfo is null");
                return webReturn;
            }
            webReturn.Session = session;
            try
            {
                OperationReturn optReturn;
                DatabaseInfo dbInfo = session.DatabaseInfo;
                if (dbInfo != null)
                {
                    dbInfo.RealPassword = DecryptFromClient(dbInfo.Password);
                    session.DBConnectionString = dbInfo.GetConnectionString();
                }
                switch (webRequest.Code)
                {
                    case (int)S3105Codes.GetRoleOperationList:
                        optReturn = GetRoleOperationList(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        List<string> listOpts = optReturn.Data as List<string>;
                        webReturn.ListData = listOpts;
                        break;
                    case (int)S3105Codes.GetDepartmentInfo:
                        optReturn = GetDepartmentInfo(session);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.DataSetData = optReturn.Data as DataSet;
                        break;
                    case (int)S3105Codes.GetCheckDatasOrRecheckDatas:
                        optReturn = GetCheckDatasOrRecheckDatas(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S3105Codes.GetCheckAndReCheckData:
                        optReturn = GetCheckAndReCheckData(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S3105Codes.SubmitCheckData:
                        optReturn = SubmitCheckData(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        break;
                    case (int)S3105Codes.GetAppealProcess:
                        optReturn = GetAppealProcess(session);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.DataSetData = optReturn.Data as DataSet;
                        break;
                    case (int)S3105Codes.GetAppealInfoData:
                        optReturn = GetAppealInfoData(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S3105Codes.GetAuInfoList:
                        optReturn = GetAuInfoList(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S3105Codes.UpdateTable19:
                        optReturn = UpdateTable19(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        break;
                    case (int)S3105Codes.GetAppealProcessHistory:
                        optReturn = GetAppealProcessHistory(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S3105Codes.GetAppealRecordsHistory:
                        optReturn = GetAppealRecordsHistory(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S3105Codes.GetUserScoreSheetList:
                        optReturn = GetUserScoreSheetList(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S3105Codes.SaveScoreSheetResult:
                        optReturn = SaveScoreSheetResult(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.Data = optReturn.Data.ToString();
                        webReturn.ListData.Add(optReturn.StringValue);
                        break;
                    case (int)S3105Codes.GetNewScore:
                        optReturn = GetNewScore(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.Data = optReturn.Data.ToString();
                        break;
                    default:
                        webReturn.Result = false;
                        webReturn.Code = Defines.RET_PARAM_INVALID;
                        webReturn.Message = string.Format("WebCodes invalid.\t{0}", webRequest.Code);
                        return webReturn;
                }
                webReturn.Result = true;
                webReturn.Code = 0;
                webReturn.Message = optReturn.Message;
                return webReturn;
            }
            catch (Exception ex)
            {
                webReturn.Result = false;
                webReturn.Code = Defines.RET_FAIL;
                webReturn.Message = ex.Message;
                return webReturn;
            }
        }

        private OperationReturn GetRoleOperationList(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParams
                //0     用户编号
                //1     模块代码
                //2     上级模块或操作编号
                if (listParams == null || listParams.Count < 2)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string moduleID = listParams[0];
                string parentID = listParams[1];
                string rentToken = session.RentInfo.Token;
                string roleId = session.RoleInfo.ID.ToString();
                string strSql;
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        strSql =
                            string.Format(
                                "SELECT * FROM T_11_003_{0} WHERE C001 = {1} AND (C002 LIKE '{2}%' ) AND C002 IN (SELECT C002 FROM T_11_202_{0} WHERE C001 = {3} AND  C003='1' ) ORDER BY  C004 ",
                                rentToken,
                                moduleID,
                                parentID,
                                roleId);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        strSql =
                            string.Format(
                                "SELECT * FROM T_11_003_{0} WHERE C001 = {1} AND (C002 LIKE  '{2}%' ) AND C002 IN (SELECT C002 FROM T_11_202_{0} WHERE C001 = {3} AND  C003='1' ) ORDER BY  C004",
                                rentToken,
                                moduleID,
                                parentID,
                                roleId
                                );
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
                List<string> listOpts = new List<string>();
                for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = objDataSet.Tables[0].Rows[i];
                    OperationInfo opt = new OperationInfo();
                    opt.ID = Convert.ToInt64(dr["C002"]);
                    opt.ParentID = Convert.ToInt64(dr["C003"]);
                    opt.Display = string.Format("Opt({0})", opt.ID);
                    opt.Description = opt.Description;
                    opt.Icon = dr["C013"].ToString();
                    optReturn = XMLHelper.SeriallizeObject(opt);
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                    listOpts.Add(optReturn.Data.ToString());
                }
                optReturn.Data = listOpts;
                optReturn.Message = strSql;
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

        private OperationReturn GetDepartmentInfo(SessionInfo session)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                string rentToken = session.RentInfo.Token;
                string roleId = session.RoleInfo.ID.ToString();
                string strSql;
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        strSql =
                            string.Format("SELECT * FROM T_11_006_{0}",
                                rentToken);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        strSql =
                            string.Format("SELECT * FROM T_11_006_{0}",
                                rentToken);
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
                optReturn.Data = objDataSet;
                optReturn.Message = strSql;
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

        private OperationReturn GetCheckDatasOrRecheckDatas(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                if (listParams == null || listParams.Count < 4)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strdeptid = listParams[0];
                string strcheck = listParams[1];
                string strrecheck= listParams[2];
                string type = listParams[3];//获取数据类型：1：审批数据，2：复核数据，3：无审批过程的复核数据
                string rentToken = session.RentInfo.Token;
                string strSql;
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        strSql =
                            string.Format(
                                //"SELECT T19.*,T05.C003 AGENTNAME,T47.C001 ADID FROM T_31_019_{0} T19 LEFT JOIN T_11_005_{0} T05 ON T19.C004=T05.C001 LEFT JOIN T_31_047_{0} T47 ON T19.C001=T47.C001 AND T47.C005 IN({2}) WHERE T19.C004 IN (SELECT C001 FROM T_11_005_{0} WHERE C006 IN({1}))",
                                "SELECT T19.*,T05.C003 AGENTNAME,T471.C001 ADID1,T472.C001 ADID2,T471.C007 AD1TIME,T472.C007 AD2TIME,T08.C004 SCORE,T08.C003 TempplateID,T08.C010 ScoreType,T08.C005 ScoreUserID"
                                +" FROM T_31_019_{0} T19 LEFT JOIN T_11_005_{0} T05 ON T19.C004=T05.C001 AND T05.C007<>'H' LEFT JOIN T_31_047_{0} T471 ON T19.C001=T471.C001 AND T471.C005 IN({2})"
                                +" LEFT JOIN T_31_008_{0} T08 ON T08.C001=T19.C002 LEFT JOIN T_31_047_{0} T472 ON T19.C001=T472.C001 AND T472.C005 IN({3}) WHERE T19.C004 IN ({1})",
                                rentToken,
                                strdeptid,
                                strcheck,
                                strrecheck);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        strSql =
                            string.Format(
                                //"SELECT T19.*,T05.C003 AGENTNAME,T47.C001 ADID FROM T_31_019_{0} T19 LEFT JOIN T_11_005_{0} T05 ON T19.C004=T05.C001 LEFT JOIN T_31_047_{0} T47 ON T19.C001=T47.C001 AND T47.C005 IN({2}) WHERE T19.C004 IN (SELECT C001 FROM T_11_005_{0} WHERE C006 IN({1}))",
                                "SELECT T19.*,T05.C003 AGENTNAME,T471.C001 ADID1,T472.C001 ADID2,T471.C007 AD1TIME,T472.C007 AD2TIME,T08.C004 SCORE,T08.C003 TempplateID,T08.C010 ScoreType,T08.C005 ScoreUserID"
                                +" FROM T_31_019_{0} T19 LEFT JOIN T_11_005_{0} T05 ON T19.C004=T05.C001 AND T05.C007<>'H' LEFT JOIN T_31_047_{0} T471 ON T19.C001=T471.C001 AND T471.C005 IN({2})"
                                +" LEFT JOIN T_31_008_{0} T08 ON T08.C001=T19.C002 LEFT JOIN T_31_047_{0} T472 ON T19.C001=T472.C001 AND T472.C005 IN({3}) WHERE T19.C004 IN ({1})",
                                rentToken,
                                strdeptid,
                                strcheck,
                                strrecheck);
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
                #region
                try
                {
                    List<string> listtaskdetail = new List<string>();
                    for (int i = 0,tempRNum=0,tempANum=0,temp=0; i < objDataSet.Tables[0].Rows.Count; i++)
                    {
                        DataRow dr = objDataSet.Tables[0].Rows[i];
                        string adid1 = GetObjData(dr["ADID1"]);
                        string adid2 = GetObjData(dr["ADID2"]);
                        //1：复核数据，2：审批数据，3：无复核过程的审批数据
                        if (type == "1")
                        {
                            if (adid1 == "")//t47表中無記錄--未複核
                            {
                                tempRNum += 1;
                                CCheckData ccd = new CCheckData();
                                ccd.RowNumber = tempRNum;
                                ccd.AppealDetailID = GetObjData(dr["C001"]);
                                ccd.RecoredReference = GetObjData(dr["C003"]);
                                ccd.AgentID = GetObjData(dr["C004"]);
                                ccd.AgentName = DecryptFNames(dr["AGENTNAME"]);
                                ccd.AppealFlowID = GetObjData(dr["C006"]);
                                ccd.ScoreResultID = GetObjData(dr["C002"]);
                                ccd.AppealDatetime = DatePress(dr["C012"]).ToString("yyyy-MM-dd HH:mm:ss");
                                ccd.TemplateID = dr["TempplateID"].ToString();
                                ccd.Score = dr["SCORE"].ToString();
                                ccd.ScoreType = dr["ScoreType"].ToString();
                                ccd.ScoreUserID = Convert.ToInt64(dr["ScoreUserID"]);
                                ccd.OperationTime = ccd.AppealDatetime;
                                optReturn = XMLHelper.SeriallizeObject(ccd);
                                if (!optReturn.Result)
                                {
                                    break;
                                }
                                listtaskdetail.Add(optReturn.Data.ToString());
                            }
                        }
                        else if (type == "2")
                        {
                            if (adid1 != "" && adid2 == "")
                            {
                                tempANum += 1;
                                CCheckData ccd = new CCheckData();
                                ccd.RowNumber = tempANum;
                                ccd.AppealDetailID = GetObjData(dr["C001"]);
                                ccd.RecoredReference = GetObjData(dr["C003"]);
                                ccd.AgentID = GetObjData(dr["C004"]);
                                ccd.AgentName = DecryptFNames(dr["AGENTNAME"]);
                                ccd.AppealFlowID = GetObjData(dr["C006"]);
                                ccd.ScoreResultID = GetObjData(dr["C002"]);
                                ccd.AppealDatetime = DatePress(dr["C012"]).ToString("yyyy-MM-dd HH:mm:ss");
                                ccd.TemplateID = dr["TempplateID"].ToString();
                                ccd.Score = dr["SCORE"].ToString();
                                ccd.ScoreType = dr["ScoreType"].ToString();
                                ccd.OperationTime = DatePress(dr["AD1TIME"]).ToString("yyyy-MM-dd HH:mm:ss");
                                optReturn = XMLHelper.SeriallizeObject(ccd);
                                if (!optReturn.Result)
                                {
                                    break;
                                }
                                listtaskdetail.Add(optReturn.Data.ToString());
                            }
                        }
                        else if (type == "3")
                        {
                            if (adid2 == "")
                            {
                                temp += 1;
                                CCheckData ccd = new CCheckData();
                                ccd.RowNumber = temp;
                                ccd.AppealDetailID = GetObjData(dr["C001"]);
                                ccd.RecoredReference = GetObjData(dr["C003"]);
                                ccd.AgentID = GetObjData(dr["C004"]);
                                ccd.AgentName = DecryptFNames(dr["AGENTNAME"]);
                                ccd.AppealFlowID = GetObjData(dr["C006"]);
                                ccd.ScoreResultID = GetObjData(dr["C002"]);
                                ccd.AppealDatetime = DatePress(dr["C012"]).ToString("yyyy-MM-dd HH:mm:ss");
                                ccd.TemplateID = dr["TempplateID"].ToString();
                                ccd.Score = dr["SCORE"].ToString();
                                ccd.ScoreType = dr["ScoreType"].ToString();
                                optReturn = XMLHelper.SeriallizeObject(ccd);
                                if (!optReturn.Result)
                                {
                                    break;
                                }
                                listtaskdetail.Add(optReturn.Data.ToString());
                            }
                        }
                    }
                    optReturn.Message = strSql;
                    optReturn.Data = listtaskdetail;
                }
                catch (Exception ex)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_DBACCESS_FAIL;
                    optReturn.Message = ex.Message;
                }
                #endregion
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

        private OperationReturn GetCheckAndReCheckData(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                if (listParams == null || listParams.Count < 2)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strapid = listParams[0];
                string strtypes = listParams[1];
                string rentToken = session.RentInfo.Token;
                string strSql;
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        strSql =
                            string.Format(
                                "SELECT T47.*,T05.C003 AGENTNAME FROM T_31_047_{0} T47 LEFT JOIN T_11_005_{0} T05 ON T47.C006=T05.C001 AND T05.C007<>'H' WHERE T47.C001 = {1} AND T47.C005 IN({2})",
                                rentToken,
                                strapid,
                                strtypes);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        strSql =
                            string.Format(
                                "SELECT T47.*,T05.C003 AGENTNAME FROM T_31_047_{0} T47 LEFT JOIN T_11_005_{0} T05 ON T47.C006=T05.C001 AND T05.C007<>'H' WHERE T47.C001 = {1} AND T47.C005 IN({2})",
                                rentToken,
                                strapid,
                                strtypes);
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
                #region
                try
                {
                    List<string> listtaskdetail = new List<string>();
                    for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                    {
                        DataRow dr = objDataSet.Tables[0].Rows[i];
                        CAppeaDetail ccd = new CAppeaDetail();
                        ccd.AppealDetailID = GetObjData(dr["C001"]);
                        ccd.AppealFlowID = GetObjData(dr["C002"]);
                        ccd.AppealFlowItemID = GetObjData(dr["C003"]);
                        ccd.Note = GetObjData(dr["C004"]);
                        ccd.ActionID = GetObjData(dr["C005"]);
                        ccd.OperationerID = GetObjData(dr["C006"]);
                        ccd.OperationerName = DecryptFNames(dr["AGENTNAME"]);
                        optReturn = XMLHelper.SeriallizeObject(ccd);
                        if (!optReturn.Result)
                        {
                            break;
                        }
                        listtaskdetail.Add(optReturn.Data.ToString());
                    }
                    optReturn.Message = strSql;
                    optReturn.Data = listtaskdetail;
                }
                catch (Exception ex)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_DBACCESS_FAIL;
                    optReturn.Message = ex.Message;
                }
                #endregion
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

        public OperationReturn SubmitCheckData(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                if (listParams.Count < 6)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Param count invalid");
                    return optReturn;
                }
                string strSql;
                int actionID = Convert.ToInt32(listParams[4]);
                if (listParams.Count > 11)
                {
                    string strScoreSheet = listParams[11];
                    optReturn = XMLHelper.DeserializeObject<ScoreSheet>(strScoreSheet);
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                }
                ScoreSheet scoreSheet = optReturn.Data as ScoreSheet;
                if (scoreSheet == null && listParams.Count > 11)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("ScoreSheet is null");
                    return optReturn;
                }
                DataSet objDataSet = new DataSet();
                string rentToken = session.RentInfo.Token;
                strSql = string.Format("select * from T_31_047_{0} where 1<>1", rentToken); 
                IDbConnection objConn;
                IDbDataAdapter objAdapter;
                DbCommandBuilder objCmdBuilder;
                switch (session.DBType)
                {
                    //MSSQL
                    case 2:
                        objConn = MssqlOperation.GetConnection(session.DBConnectionString);
                        objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = MssqlOperation.GetCommandBuilder(objAdapter);
                        break;
                    //ORCL
                    case 3:
                        objConn = OracleOperation.GetConnection(session.DBConnectionString);
                        objAdapter = OracleOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = OracleOperation.GetCommandBuilder(objAdapter);
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("Database type not support");
                        return optReturn;
                }
                if (objConn == null || objAdapter == null || objCmdBuilder == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("Db object is null");
                    return optReturn;
                }
                objCmdBuilder.ConflictOption = ConflictOption.OverwriteChanges;
                objCmdBuilder.SetAllValues = false;
                try
                {
                    DataSet dataSet = new DataSet();
                    objAdapter.Fill(dataSet);
                    DataRow dr = dataSet.Tables[0].NewRow();
                    dr["C001"] = listParams[0];//详情表主键
                    dr["C002"] = listParams[1];//申诉流程ID
                    dr["C003"] = listParams[2];//申诉流程子项ID
                    dr["C004"] = listParams[3];//备注
                    dr["C005"] = listParams[4];
                    dr["C006"] = listParams[5];//操作人ID
                    dr["C007"] = DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss");//操作时间
                    dataSet.Tables[0].Rows.Add(dr);
                    objAdapter.Update(dataSet);
                    dataSet.AcceptChanges();
                }
                catch (Exception ex)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_FAIL;
                    optReturn.Message = ex.Message + ex.Source + ex.StackTrace;
                    return optReturn;
                }
                if (actionID != 4)//申訴處理結束  ActionID 3审批驳回，4审批通过，5复核通过不修改分数，6复核通过重新评分，7复核驳回
                {
                    string strSql2 = string.Format("select * from T_31_008_{0} T08 where T08.C001 ='{1}'", rentToken, listParams[7]);
                    
                    switch (session.DBType)
                    {
                        case 2:
                            objConn = MssqlOperation.GetConnection(session.DBConnectionString);
                            objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql2);
                            objCmdBuilder = MssqlOperation.GetCommandBuilder(objAdapter);
                            break;
                        case 3:
                            objConn = OracleOperation.GetConnection(session.DBConnectionString);
                            objAdapter = OracleOperation.GetDataAdapter(objConn, strSql2);
                            objCmdBuilder = OracleOperation.GetCommandBuilder(objAdapter);
                            break;
                        default:
                            optReturn.Result = false;
                            optReturn.Code = Defines.RET_PARAM_INVALID;
                            optReturn.Message = string.Format("Database type not support");
                            return optReturn;
                    }
                    if (objConn == null || objAdapter == null || objCmdBuilder == null)
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_OBJECT_NULL;
                        optReturn.Message = string.Format("Db object is null");
                        return optReturn;
                    }
                    objCmdBuilder.ConflictOption = ConflictOption.OverwriteChanges;
                    objCmdBuilder.SetAllValues = false;
                    try
                    {
                        DataSet dataSet1 = new DataSet();
                        objAdapter.Fill(dataSet1);
                        DataRow dr1 = dataSet1.Tables[0].Rows[0];
                        dr1.BeginEdit();
                        dr1["C014"] = "2";
                        if (actionID == 6)
                        {
                            dr1["C009"] = "N";
                        }
                        dr1["C015"] = listParams[2];//申诉流程子项ID
                        dr1.EndEdit();
                        objAdapter.Update(dataSet1);
                        dataSet1.AcceptChanges();
                    }
                    catch (Exception ex)
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_FAIL;
                        optReturn.Message = ex.Message + strSql2;
                        return optReturn;
                    }
                    DataSet Tempdata = new DataSet();
                    //更新T_31_041
                    string strSql3 = string.Format("select * from T_31_041_{0} where C002={1} AND C001={2}", session.RentInfo.Token, listParams[8], listParams[7]);
                    switch (session.DBType)
                    {
                        case 2:
                            objConn = MssqlOperation.GetConnection(session.DBConnectionString);
                            objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql3);
                            objCmdBuilder = MssqlOperation.GetCommandBuilder(objAdapter);
                            break;
                        case 3:
                            objConn = OracleOperation.GetConnection(session.DBConnectionString);
                            objAdapter = OracleOperation.GetDataAdapter(objConn, strSql3);
                            objCmdBuilder = OracleOperation.GetCommandBuilder(objAdapter);
                            break;
                        default:
                            optReturn.Result = false;
                            optReturn.Code = Defines.RET_PARAM_INVALID;
                            optReturn.Message = string.Format("Database type not support");
                            return optReturn;
                    }
                    if (objConn == null || objAdapter == null || objCmdBuilder == null)
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_OBJECT_NULL;
                        optReturn.Message = string.Format("Db object is null");
                        return optReturn;
                    }
                    objCmdBuilder.ConflictOption = ConflictOption.OverwriteChanges;
                    objCmdBuilder.SetAllValues = false;
                    try
                    {
                        DataSet dataSet2 = new DataSet();
                        objAdapter.Fill(dataSet2);
                        Tempdata = dataSet2;
                        DataRow dr2 = dataSet2.Tables[0].Rows[0];
                        dr2.BeginEdit();
                        dr2["C007"] = "2";
                        dr2.EndEdit();
                        objAdapter.Update(dataSet2);
                        dataSet2.AcceptChanges();
                    }
                    catch (Exception ex)
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_FAIL;
                        optReturn.Message = ex.Message + ex.Source + ex.StackTrace;
                        return optReturn;
                    }

                    if (actionID == 6)//重新評分
                    {
                        string tempID = string.Empty;
                        //新建T341記錄
                        if (Tempdata == null || string.IsNullOrWhiteSpace(listParams[10]))
                        {
                            optReturn.Result = false;
                            optReturn.Code = Defines.RET_OBJECT_NULL;
                            optReturn.Message = string.Format("Db object is null");
                            return optReturn;
                        }
                        try
                        {
                            DataRow dr = Tempdata.Tables[0].Rows[0];
                            DataRow drNewRow = Tempdata.Tables[0].NewRow();
                            tempID = dr["C000"].ToString();
                            if (string.IsNullOrWhiteSpace(dr["C001"].ToString()))
                            {
                                optReturn.Result = false;
                                optReturn.Code = Defines.RET_OBJECT_NULL;
                                optReturn.Message = string.Format("Db object is null");
                                return optReturn;
                            }
                            drNewRow.BeginEdit();
                            drNewRow["C000"] = dr["C000"];
                            drNewRow["C001"] = listParams[10];
                            drNewRow["C002"] = dr["C002"];
                            drNewRow["C003"] = Convert.ToInt64(DateTime.Now.ToUniversalTime().ToString("yyyyMMddHHmmss"));
                            drNewRow["C004"] = listParams[5];
                            drNewRow["C005"] = "1";
                            drNewRow["C006"] = "7";
                            drNewRow["C007"] = "2";
                            drNewRow["C008"] = dr["C001"].ToString();
                            drNewRow["C009"] = "0";
                            drNewRow["C010"] = 0;
                            drNewRow["C011"] = 0;
                            drNewRow["C012"] = string.Empty;
                            drNewRow["C013"] = Convert.ToInt32(listParams[11]);
                            drNewRow["C014"] = Converter.Second2Time(Convert.ToInt64(listParams[11]));
                            drNewRow["C015"] = dr["C015"];
                            drNewRow["C016"] = dr["C016"];
                            drNewRow["C017"] = dr["C017"];//座席ID
                            drNewRow["C051"] = dr["C051"];
                            drNewRow["C052"] = dr["C052"];
                            drNewRow["C053"] = dr["C052"];
                            drNewRow["C054"] = dr["C054"];
                            drNewRow["C055"] = dr["C055"];
                            drNewRow["C056"] = dr["C056"];
                            drNewRow["C057"] = dr["C057"];
                            drNewRow["C058"] = dr["C058"];
                            drNewRow["C059"] = dr["C059"];
                            drNewRow["C060"] = dr["C060"];
                            drNewRow["C061"] = dr["C061"];
                            drNewRow["C062"] = dr["C062"];
                            drNewRow["C062"] = dr["C063"];
                            drNewRow["C099"] = dr["C099"];
                            drNewRow["C100"] = (int)(scoreSheet.Score * 10000);
                            List<ScoreItem> listItems = new List<ScoreItem>();
                            scoreSheet.GetAllScoreItem(ref listItems);
                            listItems = listItems.OrderBy(s => s.ItemID).ToList();
                            int count = listItems.Count;
                            for (int i = 0; i < count; i++)
                            {
                                int itemID = listItems[i].ItemID;
                                drNewRow[string.Format("C{0}", (itemID + 100).ToString("000"))] = (int)(listItems[i].Score * 10000);
                            }
                            Tempdata.Tables[0].Rows.Add(drNewRow);
                            objAdapter.Update(Tempdata);
                            Tempdata.AcceptChanges();
                        }
                        catch (Exception ex)
                        {
                            optReturn.Result = false;
                            optReturn.Code = Defines.RET_FAIL;
                            optReturn.Message = ex.Message + ex.Source + ex.StackTrace;
                            return optReturn;
                        }


                        //更新T_31_001 評分次數
                        //++++++++++功能:没使用一次评分表都在T_31_001里面增加次数  by  tangche  +++++++++++++++++++++++++++++++
                        string strSql_IncreaseTimes;
                        OperationReturn optReturn_ = new OperationReturn();
                        optReturn_.Result = true;
                        optReturn_.Code = 0;
                        //IDbConnection objConn_;
                        //IDbDataAdapter objAdapter_;
                        //DbCommandBuilder objCmdBuilder_;
                        switch (session.DBType)
                        {
                            case 2:
                                strSql_IncreaseTimes = string.Format("UPDATE T_31_001_{0} SET C017=C017+1 WHERE C001={1}", rentToken, tempID);
                                optReturn_ = MssqlOperation.ExecuteSql(session.DBConnectionString, strSql_IncreaseTimes);
                                //if (!optReturn.Result)
                                //{
                                //    return optReturn;
                                //}
                                //objConn_ = MssqlOperation.GetConnection(session.DBConnectionString);
                                //objAdapter_ = MssqlOperation.GetDataAdapter(objConn_, strSql_IncreaseTimes);
                                //objCmdBuilder_ = MssqlOperation.GetCommandBuilder(objAdapter_);
                                break;
                            case 3:
                                strSql_IncreaseTimes = string.Format("UPDATE T_31_001_{0} SET C017=C017+1 WHERE C001={1}", rentToken, tempID);
                                optReturn_ = OracleOperation.ExecuteSql(session.DBConnectionString, strSql_IncreaseTimes);
                                //if (!optReturn.Result)
                                //{
                                //    return optReturn;
                                //}
                                //objConn_ = OracleOperation.GetConnection(session.DBConnectionString);
                                //objAdapter_ = OracleOperation.GetDataAdapter(objConn_, strSql_IncreaseTimes);
                                //objCmdBuilder_ = OracleOperation.GetCommandBuilder(objAdapter_);
                                break;
                            default:
                                optReturn_.Result = false;
                                optReturn_.Code = Defines.RET_PARAM_INVALID;
                                optReturn_.Message = string.Format("Database type not surpport.\t{0}", session.DBType);
                                return optReturn_;
                        }
                        //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

                    }

                    optReturn.Message = strSql2;
                }
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message+ex.Source+ex.StackTrace;
                return optReturn;
            }
            return optReturn;
        }
        
        public OperationReturn GetAppealProcess(SessionInfo session)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                string rentToken = session.RentInfo.Token;
                string roleId = session.RoleInfo.ID.ToString();
                string strSql;
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        strSql =
                            string.Format("SELECT T3117.C007 FROM T_31_016_{0} T3116 LEFT JOIN T_31_017_{0} T3117 ON T3116.C001=T3117.C002 WHERE T3117.C005=2",
                                rentToken);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        strSql =
                            string.Format("SELECT T3117.C007 FROM T_31_016_{0} T3116 LEFT JOIN T_31_017_{0} T3117 ON T3116.C001=T3117.C002 WHERE T3117.C005=2",
                                rentToken);
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
                optReturn.Data = objDataSet;
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

        //查詢頁面
        public OperationReturn GetAppealInfoData(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0      查询语句
                if (listParams == null || listParams.Count < 1)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strSql = listParams[0];
                DataSet objDataSet;
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
                 List<string> listReturn = new List<string>();
                for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = objDataSet.Tables[0].Rows[i];
                    AppealInfo item=new AppealInfo();
                    item.RowNumber = i + 1;
                    item.ScoreID = Convert.ToInt64(dr["C002"]);
                    item.ReferenceID = Convert.ToInt64(dr["C003"]);
                    item.Appealint = Convert.ToInt32(dr["C009"]);
                    item.AppealTime = Convert.ToDateTime(dr["C012"]).ToLocalTime();
                    item.AgentID = Convert.ToInt64(dr["C004"]);
                    item.Score = Convert.ToDouble(dr["Score"]);
                    item.TemplateName = dr["TemplateName"].ToString();
                    optReturn = XMLHelper.SeriallizeObject(item);
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                    listReturn.Add(optReturn.Data.ToString());
                }
                optReturn.Data = listReturn;
                optReturn.Message = null;
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
        
        private OperationReturn GetAuInfoList(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                string rentToken = session.RentInfo.Token;
                string strSql = string.Format("select C001,C002,C003,C006 from T_11_005_{0} ", rentToken);
                DataSet objDataset;
                switch (session.DBType)
                {
                    case 2:
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataset = optReturn.Data as DataSet;
                        break;
                    case 3:
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataset = optReturn.Data as DataSet;
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("Database type not surpport.\t{0}", session.DBType);
                        return optReturn;
                }
                if (objDataset == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("DataSet is null");
                    return optReturn;
                }
                List<string> listReturn = new List<string>();
                for (int i = 0; i < objDataset.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = objDataset.Tables[0].Rows[i];
                    AgentAndUserInfoList auInfoList = new AgentAndUserInfoList();
                    auInfoList.ID = Convert.ToInt64(dr["C001"].ToString());
                    auInfoList.Name = EncryptDecryptFromDB(dr["C002"].ToString());
                    auInfoList.FullName = EncryptDecryptFromDB(dr["C003"].ToString());
                    auInfoList.OrgID = Convert.ToInt64(dr["C006"].ToString());
                    optReturn = XMLHelper.SeriallizeObject(auInfoList);
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                    listReturn.Add(optReturn.Data.ToString());
                }
                strSql = string.Format("Select C001,C011,C017,C018 from T_11_101_{0} where C001>=1030000000000000001 And C001<1040000000000000001 AND C002='1'", rentToken);
                DataSet objDataset1;
                switch (session.DBType)
                {
                    case 2:
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataset1 = optReturn.Data as DataSet;
                        break;
                    case 3:
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataset1 = optReturn.Data as DataSet;
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("Database type not surpport.\t{0}", session.DBType);
                        return optReturn;
                }
                if (objDataset1 == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("DataSet is null");
                    return optReturn;
                }
                for (int i = 0; i < objDataset1.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = objDataset1.Tables[0].Rows[i];
                    AgentAndUserInfoList auInfoList = new AgentAndUserInfoList();
                    auInfoList.ID = Convert.ToInt64(dr["C001"].ToString());
                    auInfoList.OrgID = Convert.ToInt64(dr["C011"].ToString());
                    auInfoList.Name = EncryptDecryptFromDB(dr["C017"].ToString());
                    auInfoList.FullName = EncryptDecryptFromDB(dr["C018"].ToString());
                    optReturn = XMLHelper.SeriallizeObject(auInfoList);
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                    listReturn.Add(optReturn.Data.ToString());
                }
                optReturn.Data = listReturn;
                optReturn.Message = null;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message + ex.Source + ex.StackTrace;
                return optReturn;
            }
            return optReturn;
        }

        private OperationReturn UpdateTable19(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //0      C001
                //1      C009
                //2       C008
                //3       C010
                if (listParams == null || listParams.Count <3)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strCondition = string.Empty;
                switch (listParams.Count)
                {
                    case 3:
                        strCondition = string.Format("UPDATE T_31_019_{0} SET C009='{1}',C008={2} WHERE C001='{3}'", session.RentInfo.Token,listParams[1],listParams[2],listParams[0]);
                        break;
                    case 4:
                        strCondition = string.Format("UPDATE T_31_019_{0} SET C009='{1}',C008={2},C010='{3}' WHERE C001='{4}'", session.RentInfo.Token,listParams[1],listParams[2],listParams[3],listParams[0]);
                        break;
                }
                switch (session.DBType)
                {
                    case 2:
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strCondition);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        break;
                    case 3:
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strCondition);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("Database type not surpport.\t{0}", session.DBType);
                        return optReturn;
                }
                optReturn.Message = strCondition;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message + ex.Source + ex.StackTrace;
                return optReturn;
            }
            return optReturn;
        }

        private OperationReturn GetAppealProcessHistory(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                if (listParams == null || listParams.Count < 2)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strdeptid = listParams[0];
                string appealID = listParams[1];
                string rentToken = session.RentInfo.Token;
                string starTime = DateTime.Now.AddMonths(-1).ToUniversalTime().ToString("yyyy/MM/dd HH:mm:ss");
                string stopTime = DateTime.Now.ToUniversalTime().ToString("yyyy/MM/dd HH:mm:ss");
                string strCondition = string.Empty;
                
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        strCondition = string.Format("SELECT T19.*, T05.C003 AGENTNAME,T08.C003 TemplateID,T08.C004 SCORE,T47.C007 OptTIME FROM T_31_019_{0} T19 LEFT JOIN T_11_005_{0} T05 ON " +
                                             "T19.C004 = T05.C001 AND T05.C007 <> 'H' LEFT JOIN T_31_008_{0} T08 ON T08.C001 = T19.C002 LEFT JOIN T_31_047_{0} T47 ON T19.C001 = T47.C001 " +
                                             " where  T19.C004 IN ({2}) AND T47.C005 IN ({1}) AND T19.C012>='{3}' AND T19.C012<='{4}'",
                                             rentToken, appealID, strdeptid, starTime, stopTime);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strCondition);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        strCondition = string.Format("SELECT T19.*, T05.C003 AGENTNAME,T08.C003 TemplateID,T08.C004 SCORE,T47.C007 OptTIME FROM T_31_019_{0} T19 LEFT JOIN T_11_005_{0} T05 ON " +
                                             "T19.C004 = T05.C001 AND T05.C007 <> 'H' LEFT JOIN T_31_008_{0} T08 ON T08.C001 = T19.C002 LEFT JOIN T_31_047_{0} T47 ON T19.C001 = T47.C001 " +
                                             " where  T19.C004 IN ({2}) AND T47.C005 IN ({1}) AND T19.C012>=TO_DATE ('{3}','YYYY-MM-DD HH24:MI:SS') AND T19.C012<=TO_DATE( '{4}','YYYY-MM-DD HH24:MI:SS')",
                                             rentToken, appealID, strdeptid, starTime, stopTime);
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strCondition);
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
                List<string> listAppealInfo = new List<string>();
                for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = objDataSet.Tables[0].Rows[i];
                    CCheckData ccd = new CCheckData();
                    ccd.RowNumber = i + 1;
                    ccd.AppealDetailID = GetObjData(dr["C001"]);
                    ccd.RecoredReference = GetObjData(dr["C003"]);
                    ccd.AgentID = GetObjData(dr["C004"]);
                    ccd.AgentName = DecryptFNames(dr["AGENTNAME"]);
                    ccd.AppealFlowID = GetObjData(dr["C006"]);
                    ccd.ScoreResultID = GetObjData(dr["C002"]);
                    ccd.AppealInt = Convert.ToInt32(dr["C009"]);
                    ccd.AppealDatetime = DatePress(dr["C012"]).ToString("yyyy-MM-dd HH:mm:ss");
                    ccd.Score = dr["SCORE"].ToString();
                    ccd.TemplateID = dr["TemplateID"].ToString();
                    ccd.OperationTime = DatePress(dr["OptTIME"]).ToString("yyyy-MM-dd HH:mm:ss");
                    optReturn = XMLHelper.SeriallizeObject(ccd);
                    if (!optReturn.Result)
                    {
                        break;
                    }
                    listAppealInfo.Add(optReturn.Data.ToString());
                }
                optReturn.Message = strCondition;
                optReturn.Data = listAppealInfo;
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

        private OperationReturn GetAppealRecordsHistory(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0      C003
                //1      C002
                if (listParams == null || listParams.Count < 2)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strQueryString = string.Format("select * from T_31_019_{0} where C003={1} And C002={2}", session.RentInfo.Token, listParams[0], listParams[1]);
                long temp;
                DataSet objDataSet;
                List<string> listReturn = new List<string>();
                switch (session.DBType)
                {
                    case 2:
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strQueryString);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strQueryString);
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
                if (objDataSet.Tables[0].Rows.Count == 0)
                {
                    optReturn.Result = true;
                    optReturn.Message = S3105Consts.AppealOvered;
                    return optReturn;
                }
                DataRow dr = objDataSet.Tables[0].Rows[0];
                AppealInfoDetailItem appealInfoItem = new AppealInfoDetailItem();
                temp = Convert.ToInt64(dr["C001"]);
                strQueryString = string.Format("select * from T_31_047_{0} where C001={1} order by C005 DESC", session.RentInfo.Token, temp);
                switch (session.DBType)
                {
                    case 2:
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strQueryString);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strQueryString);
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
                for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                {
                    dr = objDataSet.Tables[0].Rows[i];
                    appealInfoItem.ID = Convert.ToInt32(dr["C005"].ToString());
                    appealInfoItem.PersonID = Convert.ToInt64(dr["C006"].ToString());
                    appealInfoItem.Time = Convert.ToDateTime(dr["C007"].ToString()).ToLocalTime();
                    appealInfoItem.Demo = dr["C004"].ToString();
                    appealInfoItem.Result = dr["C005"].ToString();
                    optReturn = XMLHelper.SeriallizeObject(appealInfoItem);
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                    listReturn.Add(optReturn.Data.ToString());
                }
                optReturn.Data = listReturn;
                optReturn.Message = strQueryString;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message + ex.Source + ex.StackTrace;
                return optReturn;
            }
            return optReturn;
        }


        private OperationReturn GetUserScoreSheetList(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            string strSql = "";
            try
            {
                //ListParam
                //0      记录编码
                //1      用户编码
                //2      Method  0：获取用户管理的评分表列表，即可用的评分表
                //               1：获取用户已打分的评分表
                if (listParams == null || listParams.Count < 3)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strRecordID = listParams[0];
                string strAgtID = listParams[1];
                string strMethod = listParams[2];
                string rentToken = session.RentInfo.Token;
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        if (strMethod == "0")
                        {
                            strSql = string.Format("SELECT *  FROM T_31_001_{0} WHERE C001 IN (SELECT C004 FROM T_11_201_{0} WHERE C003 = {1}) AND C010 > '{2}' AND C018='Y' "
                              , rentToken
                              , strAgtID
                              , DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss"));
                        }
                        else
                        {
                            strSql = string.Format("SELECT A.C001, A.C002, A.C004, B.C001 AS BC001, B.C002 AS BC002, B.C004 AS BC004, B.C013 AS BC013 ");
                            strSql += string.Format("FROM T_31_001_{0} A, T_31_008_{0} B ", rentToken);
                            strSql += string.Format("WHERE A.C001 = B.C003 AND B.C003 IN (SELECT C004 ");
                            strSql += string.Format("FROM T_11_201_{0} WHERE C003 = {1}) AND B.C002 = {2} AND B.C005 = {3}",
                                rentToken,
                                strAgtID,
                                strRecordID,
                                session.UserID);
                        }

                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        if (strMethod == "0")
                        {
                            strSql = string.Format("SELECT *  FROM T_31_001_{0} WHERE C001 IN (SELECT C004 FROM T_11_201_{0} WHERE C003 = {1}) AND C010 > to_date('{2}','YYYY/MM/DD HH24:MI:SS')  AND C018='Y' "
                              , rentToken
                              , strAgtID
                              , DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss"));
                        }
                        else
                        {
                            strSql = string.Format("select c001,c002,c004,BC001,BC002,BC004,BC013 from (select a.*,row_number() over(order by BC001 desc) as sid from (SELECT A.C001, A.C002, A.C004, B.C001 AS BC001, B.C002 AS BC002, B.C004 AS BC004, B.C013 AS BC013 ");
                            strSql += string.Format("FROM T_31_001_{0} A, T_31_008_{0} B ", rentToken);
                            strSql += string.Format("WHERE A.C001 = B.C003 AND B.C003 IN (SELECT C004 ");
                            strSql += string.Format("FROM T_11_201_{0} WHERE C003 = {1}) AND B.C002 = {2} AND B.C005 = {3}) a) b where b.sid=1",
                                rentToken,
                                strAgtID,
                                strRecordID,
                                session.UserID);
                        }
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
                    BasicScoreSheetInfo item = new BasicScoreSheetInfo();
                    if (strMethod == "0")
                    {
                        item.ScoreSheetID = Convert.ToInt64(dr["C001"]);
                        item.RecordSerialID = Convert.ToInt64(strRecordID);
                        item.UserID = Convert.ToInt64(strAgtID);
                        item.Title = dr["C002"].ToString();
                        item.TotalScore = Convert.ToDouble(dr["C004"]);
                    }
                    else
                    {
                        item.ScoreSheetID = Convert.ToInt64(dr["C001"]);
                        item.ScoreResultID = Convert.ToInt64(dr["BC001"]);
                        item.RecordSerialID = Convert.ToInt64(dr["BC002"]);
                        item.UserID = Convert.ToInt64(dr["BC013"]);
                        item.Score = Convert.ToDouble(dr["BC004"]);
                    }
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
            optReturn.Message = strSql;
            return optReturn;
        }


        private OperationReturn SaveScoreSheetResult(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0     评分表成绩信息
                //2     评分所花时间
                if (listParams == null || listParams.Count < 1)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strScoreSheetResult = listParams[0];
                optReturn = XMLHelper.DeserializeObject<BasicScoreSheetInfo>(strScoreSheetResult);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                BasicScoreSheetInfo scoreSheetResult = optReturn.Data as BasicScoreSheetInfo;
                if (scoreSheetResult == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("ScoreSheetResult is null");
                    return optReturn;
                }
                string strResultID = scoreSheetResult.ScoreResultID.ToString();
                //是否修改成绩
                bool isModify = !string.IsNullOrEmpty(strResultID) && strResultID != "0";

                #region 获取新的成绩ID

                //生成一个新的成绩ID
                WebRequest webRequest = new WebRequest();
                webRequest.Session = session;
                webRequest.Code = (int)RequestCode.WSGetSerialID;
                webRequest.ListData.Add("31");
                webRequest.ListData.Add("307");
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
                string strNewResultID = webReturn.Data;
                if (string.IsNullOrEmpty(strNewResultID))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_STRING_EMPTY;
                    optReturn.Message = string.Format("New ScoreResultID is empty");
                    return optReturn;
                }

                #endregion

                #region DBQuery 更新T_31_008、T_31_041标记

                string rentToken = session.RentInfo.Token;
                //是否增加记录
                bool isAdd = false;
                string strSql;
                IDbConnection objConn;
                IDbDataAdapter objAdapter;
                DbCommandBuilder objCmdBuilder;

                string strSqlTask;
                long OrgID = 0;
                DataSet tempDS;
                string sql = string.Format("SELECT * FROM T_31_008_{0} WHERE C002={1}", rentToken, scoreSheetResult.RecordSerialID);
                bool isNewScore = listParams[1] == "3" ? true : false;
                if (!isNewScore) isNewScore = listParams[1] == "5" ? true : false;
                switch (session.DBType)
                {
                    case 2:
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, sql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        tempDS = optReturn.Data as DataSet;

                        //更新T_31_008、T_31_041标记
                        sql = string.Format("UPDATE T_31_008_{0} SET C009='N' WHERE C002={1} AND C003={2};UPDATE T_31_041_{0} SET C005='0' WHERE C002={1} AND C000={2};", rentToken, scoreSheetResult.RecordSerialID, scoreSheetResult.ScoreSheetID);
                        optReturn = MssqlOperation.ExecuteSql(session.DBConnectionString, sql);

                        strSql = string.Format("SELECT * FROM T_31_008_{0} WHERE C001 = {1} OR C001 = {2}",
                            rentToken,
                            strResultID,
                            strNewResultID);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objConn = MssqlOperation.GetConnection(session.DBConnectionString);
                        objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = MssqlOperation.GetCommandBuilder(objAdapter);


                        break;
                    case 3:
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, sql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        tempDS = optReturn.Data as DataSet;

                        //更新T_31_008、T_31_041标记
                        sql = string.Format("BEGIN UPDATE T_31_008_{0} SET C009='N' WHERE C002={1} AND C003={2};UPDATE T_31_041_{0} SET C005='0' WHERE C002={1} AND C000={2};END", rentToken, scoreSheetResult.RecordSerialID, scoreSheetResult.ScoreSheetID);
                        optReturn = OracleOperation.ExecuteSql(session.DBConnectionString, sql);

                        strSql = string.Format("SELECT * FROM T_31_008_{0} WHERE C001 = {1} OR C001 = {2}",
                            rentToken,
                            strResultID,
                            strNewResultID);
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objConn = OracleOperation.GetConnection(session.DBConnectionString);
                        objAdapter = OracleOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = OracleOperation.GetCommandBuilder(objAdapter);

                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("Database type not surpport.\t{0}", session.DBType);
                        return optReturn;
                }
                if (objConn == null || objAdapter == null || objCmdBuilder == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("Db object is null");
                    return optReturn;
                }
                #endregion
                DataRow tempDR = tempDS.Tables[0].Rows[0];
                OrgID = Convert.ToInt64(tempDR["C012"].ToString());


                #region DBOperation
                objCmdBuilder.ConflictOption = ConflictOption.OverwriteChanges;
                objCmdBuilder.SetAllValues = false;
                try
                {
                    DataSet objDataSet = new DataSet();
                    objAdapter.Fill(objDataSet);
                    DataRow drModify, drAdd;
                    if (!string.IsNullOrWhiteSpace(strResultID))
                    {
                        //修改成绩时，向成绩表里新增一行成绩信息，同时将原成绩记录的C009字段值为N
                    drModify = objDataSet.Tables[0].Select(string.Format("C001 = {0}", strResultID)).FirstOrDefault();
                    if (drModify != null)
                    {
                        drModify["C009"] = "N";
                    }
                    }

                    drAdd = objDataSet.Tables[0].Select(string.Format("C001 = {0}", strNewResultID)).FirstOrDefault();
                    if (drAdd == null)
                    {
                        drAdd = objDataSet.Tables[0].NewRow();
                        drAdd["C001"] = Convert.ToInt64(strNewResultID);
                        isAdd = true;
                    }
                    drAdd["C002"] = scoreSheetResult.RecordSerialID;
                    drAdd["C003"] = scoreSheetResult.ScoreSheetID;
                    drAdd["C004"] = scoreSheetResult.Score;
                    drAdd["C005"] = session.UserID;
                    drAdd["C006"] = DateTime.UtcNow;
                    drAdd["C007"] = "N";
                    drAdd["C008"] = 0;
                    drAdd["C009"] = "Y";
                    drAdd["C010"] = listParams[1];
                    drAdd["C011"] = "N";
                    drAdd["C012"] = OrgID;
                    drAdd["C013"] = scoreSheetResult.UserID;
                    if ( listParams[1] == "7")
                        drAdd["C014"] = "2";
                    else
                        drAdd["C014"] = "0";
                    drAdd["C015"] = 0;
                    drAdd["C016"] = listParams[2];
                    if (isAdd)
                    {
                        objDataSet.Tables[0].Rows.Add(drAdd);
                    }

                    objAdapter.Update(objDataSet);
                    objDataSet.AcceptChanges();
                }
                catch (Exception ex)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_DBACCESS_FAIL;
                    optReturn.Message = ex.Message;
                    return optReturn;
                }
                finally
                {
                    if (objConn.State == ConnectionState.Open)
                    {
                        objConn.Close();
                    }
                    objConn.Dispose();
                }
                optReturn.Data = strNewResultID;
                optReturn.StringValue = "2";//修改成绩表操作完成
                #endregion

                #region 更新评分表C017+1
                string strSqlTemplate = "";
                IDbConnection objConnTemplate;
                IDbDataAdapter objAdapterTemplate;
                DbCommandBuilder objCmdBuilderTemplate;
                OperationReturn optReturnTemplate = new OperationReturn();
                try
                {
                    switch (session.DBType)
                    {
                        case 2:
                            strSqlTemplate = string.Format("SELECT * FROM T_31_001_{0} WHERE C001={1}", rentToken, scoreSheetResult.ScoreSheetID);
                            optReturnTemplate = MssqlOperation.GetDataSet(session.DBConnectionString, strSqlTemplate);
                            if (!optReturnTemplate.Result)
                            {
                                return optReturnTemplate;
                            }
                            objConnTemplate = MssqlOperation.GetConnection(session.DBConnectionString);
                            objAdapterTemplate = MssqlOperation.GetDataAdapter(objConnTemplate, strSqlTemplate);
                            objCmdBuilderTemplate = MssqlOperation.GetCommandBuilder(objAdapterTemplate);

                            break;
                        case 3:
                            strSqlTemplate = string.Format("SELECT * FROM T_31_001_{0} WHERE C001={1}", rentToken, scoreSheetResult.ScoreSheetID);
                            optReturnTemplate = OracleOperation.GetDataSet(session.DBConnectionString, strSqlTemplate);
                            if (!optReturnTemplate.Result)
                            {
                                return optReturnTemplate;
                            }
                            objConnTemplate = OracleOperation.GetConnection(session.DBConnectionString);
                            objAdapterTemplate = OracleOperation.GetDataAdapter(objConnTemplate, strSqlTemplate);
                            objCmdBuilderTemplate = OracleOperation.GetCommandBuilder(objAdapterTemplate);
                            break;
                        default:
                            optReturn.Result = false;
                            optReturn.Code = Defines.RET_PARAM_INVALID;
                            optReturn.Message = string.Format("Database type not surpport.\t{0}", session.DBType);
                            return optReturn;
                    }

                    objCmdBuilderTemplate.ConflictOption = ConflictOption.OverwriteChanges;
                    objCmdBuilderTemplate.SetAllValues = false;
                    try
                    {
                        DataSet objDataSetTemplate = new DataSet();
                        objAdapterTemplate.Fill(objDataSetTemplate);
                        if (objDataSetTemplate.Tables[0].Rows.Count > 0)
                        {
                            DataRow dr = objDataSetTemplate.Tables[0].Rows[0];
                            int mCurrentCount = int.Parse(dr["C017"].ToString());
                            dr["C017"] = mCurrentCount + 1;
                            objAdapterTemplate.Update(objDataSetTemplate);
                            objDataSetTemplate.AcceptChanges();
                        }
                    }
                    catch (Exception ex)
                    {
                        optReturnTemplate.Result = false;
                        optReturnTemplate.Code = Defines.RET_DBACCESS_FAIL;
                        optReturnTemplate.Message = ex.Message;
                        return optReturnTemplate;
                    }
                    finally
                    {
                        if (objConnTemplate.State == ConnectionState.Open)
                        {
                            objConnTemplate.Close();
                        }
                        objConnTemplate.Dispose();
                    }
                    optReturn.StringValue = "3";//更新评分表次数完成
                }
                catch (Exception ex)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_DBACCESS_FAIL;
                    optReturn.Message = ex.Message;
                    return optReturn;
                }
                #endregion
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

        public OperationReturn GetNewScore(SessionInfo session,List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                string rentToken = session.RentInfo.Token;
                //ListParam
                //0     记录流水号
                //1     评分表ID
                if (listParams == null || listParams.Count < 2)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strSql = string.Format("SELECT * FROM T_31_008_{0} WHERE C014='2' AND C010='7' AND C002={1} AND C003={2}",rentToken,listParams[0],listParams[1]);
                DataSet objDataSet;
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
                    optReturn.Message = strSql;
                    return optReturn;
                }
                DataRow dr = objDataSet.Tables[0].Rows[0];
                optReturn.Data = dr["C004"].ToString();
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
        
        private string GetObjData(object obj)
        {
            return obj == null ? "" : obj.ToString();
        }

        private string DecryptFNames(object obj)
        {
            string result = "";
            if (obj != null && obj.ToString().Trim() != "")
            {
                result = DecryptNamesFromDB(obj.ToString());
            }
            return result;
        }

        private DateTime DatePress(object date)
        {
            DateTime dt = DateTime.Now;
            if (date != null && !string.IsNullOrEmpty(date.ToString()))
            {
                dt = DateTime.Parse(date.ToString()).ToLocalTime();
            }
            return dt;
        }

        private DateTime? NDatePress(object date)
        {
            DateTime? dt = null;
            if (date != null && !string.IsNullOrEmpty(date.ToString()))
            {
                dt = DateTime.Parse(date.ToString()).ToLocalTime();
            }
            return dt;
        }
    }
}
