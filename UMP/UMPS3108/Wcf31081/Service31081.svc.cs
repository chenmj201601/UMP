using Oracle.DataAccess.Client;
using PFShareClassesS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using UMPS3108.Common31081;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common31081;
using VoiceCyber.UMP.Communications;

namespace Wcf31081
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码、svc 和配置文件中的类名“Service31081”。
    // 注意: 为了启动 WCF 测试客户端以测试此服务，请在解决方案资源管理器中选择 Service31081.svc 或 Service31081.svc.cs，然后开始调试。
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class Service31081 : IService31081
    {
        public WebReturn DoOperation(WebRequest webRequest)
        {
            WebReturn webReturn = new WebReturn();
            webReturn.Result = true;
            webReturn.Code = 0;
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
                    case (int)S3108Codes.GetQualityParam:
                        optReturn = GetQualityParam(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S3108Codes.SaveQualityParam:
                        optReturn = SaveQualityParam(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S3108Codes.GetAddedCombinedParamItems:
                        optReturn = GetAddedCombinedParamItems(session);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S3108Codes.GetAllCombinedParamItems:
                        optReturn = GetAllCombinedParamItems(session);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S3108Codes.GetAllParamItemsValue:
                        optReturn = GetAllParamItemsValue(session,webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S3108Codes.GetSelectParamItemsInfos:
                        optReturn = GetSelectParamItemsInfos(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S3108Codes.GetParamItemsValue:
                        optReturn = GetParamItemsValue(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S3108Codes.ModifyCombinedParamItems:
                        optReturn = ModifyCombinedParamItems(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S3108Codes.SaveAddedParamItemsInfos:
                        optReturn = SaveAddedParamItemsInfos(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;

                  
                    case (int)S3108Codes.GetStatisticalParam:
                        optReturn = GetStatisticalParam(session);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S3108Codes.GetOrganizationList:
                        optReturn = GetOrganizationList(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S3108Codes.GetABCDInfo:
                        optReturn = GetABCDInfo(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S3108Codes.GetABCDItemList:
                        optReturn = GetABCDItemList(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S3108Codes.GetABCDList:
                        optReturn = GetABCDList(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S3108Codes.GetOrgItemRelation:
                        optReturn = GetOrgItemRelation(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        webReturn.Data = optReturn.StringValue;
                        break;
                    case (int)S3108Codes.SaveConfig:
                        optReturn = SaveConfig(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S3108Codes.GetSkillGroupList:
                        optReturn = GetSkillGroupList(session);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S3108Codes.SaveParamItemValue:
                        optReturn = SaveParamItemValue(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S3108Codes.ModifyStatisticParam:
                        optReturn = ModifyStatisticParam(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.Data = optReturn.Data as string;
                        break;
                    case (int)S3108Codes.IsDistributeOrgSkg:
                        optReturn = IsDistributeOrgSkg(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.Data = optReturn.Data as string;
                        break;
                    case (int)S3108Codes.DeleteConfig:
                        optReturn = DeleteConfig(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.Data = optReturn.Data as string;
                        break;
                    case (int)S3108Codes.GetAuthorityParam:
                        optReturn = GetAuthorityParam(session);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    default:
                        webReturn.Result = false;
                        webReturn.Code = Defines.RET_PARAM_INVALID;
                        webReturn.Message = string.Format("Request code invalid.\t{0}", webRequest.Code);
                        return webReturn;
                }
                webReturn.Message = optReturn.Message;
            }
            catch (Exception ex)
            {
                webReturn.Result = false;
                webReturn.Code = Defines.RET_FAIL;
                webReturn.Message = ex.Message;
                return webReturn;
            }
            return webReturn;
        }

        private OperationReturn SaveQualityParam(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                if (listParams.Count < 2)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Param count invalid");
                    return optReturn;
                }
                List<QualityParam> ListQP = new List<QualityParam>();
                for (int i = 0; i < listParams.Count; i++)
                {
                    optReturn = XMLHelper.DeserializeObject<QualityParam>(listParams[i]);
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                    QualityParam QP = optReturn.Data as QualityParam;
                    if (QP == null)
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_OBJECT_NULL;
                        optReturn.Message = string.Format("NewRoleInfo Is Null");
                        return optReturn;
                    }
                    ListQP.Add(QP);
                }
                string RentID, ModuleId, GroupID;
                //string ParamID,SortID, pramaValue, ModifyTime, ModifyMan
                //0     RentID
                //1     ModuleId
                //2     ParamID
                //3     GroupID
                //4     SortID
                //5     pramaValue
                //6     ModifyTime
                //7     ModifyMan
                RentID = ListQP[0].RentID.ToString();
                ModuleId = ListQP[0].ModuleID.ToString();
                //ParamID = listParams[2];
                GroupID = ListQP[0].GroupID.ToString();
                //SortID = listParams[4];
                //pramaValue = listParams[5];
                //ModifyTime = listParams[6];
                //ModifyMan = listParams[7];

                string rentToken = session.RentInfo.Token;

                string strSql = string.Format("SELECT * FROM T_11_001_{0} WHERE C001 = {1} AND C002 = {2} AND C004={3}"
                                                   , rentToken
                                                   , RentID
                                                   , ModuleId
                                                   , GroupID);
                DataSet dataSet = new DataSet();
                switch (session.DBType)
                {
                    //MSSQL
                    case 2:
                        using (SqlConnection connection = new SqlConnection(session.DBConnectionString))
                        {
                            connection.Open();

                            SqlDataAdapter sqlDA = new SqlDataAdapter(strSql, connection);
                            sqlDA.Fill(dataSet);
                            //设置主键
                            //dataSet.Tables[0].PrimaryKey = new DataColumn[] { dataSet.Tables[0].Columns[0] };
                            SqlCommandBuilder sqlCB = new SqlCommandBuilder(sqlDA);
                            sqlDA.UpdateCommand = sqlCB.GetUpdateCommand();
                            for (int j = 0; j < listParams.Count; j++)
                            {
                                DataRow drCurrent = dataSet.Tables[0].Select(string.Format("C003={0}", ListQP[j].ParamID)).FirstOrDefault();
                                //dataSet.Tables[0].Rows.Find(newRole.RoleID.ToString());
                                if (drCurrent != null)
                                {
                                    drCurrent.BeginEdit();

                                    drCurrent["C006"] = EncryptToDB(ListQP[j].ParamValue);

                                    drCurrent["C018"] = ListQP[j].ModifyTime;
                                    drCurrent["C019"] = ListQP[j].ModifyMan.ToString();

                                    drCurrent.EndEdit();
                                    sqlDA.Update(dataSet);
                                }

                                dataSet.AcceptChanges();
                            }

                            sqlDA.Dispose();
                            connection.Close();
                        }
                        break;
                    //ORCL
                    case 3:
                        using (OracleConnection connection = new OracleConnection(session.DBConnectionString))
                        {

                            connection.Open();
                            optReturn.Message += strSql;
                            OracleDataAdapter oracleDA = new OracleDataAdapter(strSql, connection);
                            oracleDA.Fill(dataSet);
                            OracleCommandBuilder oracleCB = new OracleCommandBuilder(oracleDA);
                            oracleDA.UpdateCommand = oracleCB.GetUpdateCommand();
                            for (int j = 0; j < listParams.Count; j++)
                            {
                                DataRow drCurrent = dataSet.Tables[0].Select(string.Format("C003={0}", ListQP[j].ParamID)).First();
                                //dataSet.Tables[0].Rows.Find(newRole.RoleID.ToString());
                                if (drCurrent != null)
                                {
                                    drCurrent.BeginEdit();

                                    drCurrent["C006"] = EncryptToDB(ListQP[j].ParamValue);

                                    drCurrent["C018"] = ListQP[j].ModifyTime;
                                    drCurrent["C019"] = ListQP[j].ModifyMan.ToString();

                                    drCurrent.EndEdit();
                                    oracleDA.Update(dataSet);
                                }

                                dataSet.AcceptChanges();
                            }
                            oracleDA.Dispose();
                            connection.Close();
                        }
                        break;
                    default:
                        break;
                }

            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message += ex.Message;
            }
            return optReturn;
        }

        private OperationReturn GetQualityParam(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0     模块编号

                if (listParams == null || listParams.Count < 1)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strModuleID = listParams[0];
                //string strParamID = listParams[1];
                //string strGroupID = listParams[2];

                string rentToken = session.RentInfo.Token;
                string strSql;
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        strSql =
                           string.Format(
                             "SELECT * FROM T_11_001_{0} WHERE C002 = {1}"
                               , rentToken
                               , strModuleID);

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
                               "SELECT * FROM T_11_001_{0} WHERE C002 = {1} "
                                 , rentToken
                                 , strModuleID);

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
                    QualityParam GlobalParam = new QualityParam();
                    GlobalParam.RentID = Convert.ToInt64(dr["C001"].ToString());
                    GlobalParam.ModuleID = Convert.ToInt32(dr["C002"].ToString());
                    GlobalParam.ParamID = Convert.ToInt32(dr["C003"].ToString());
                    GlobalParam.GroupID = Convert.ToInt32(dr["C004"].ToString());
                    GlobalParam.SortID = Convert.ToInt32(dr["C005"].ToString());
                    GlobalParam.ParamValue = DecryptFromDB(dr["C006"].ToString());

                    GlobalParam.ModifyTime = dr["C018"].ToString();
                    GlobalParam.ModifyMan = Convert.ToInt64(dr["C019"].ToString());
                    GlobalParam.Type = Convert.ToInt32(dr["C007"].ToString());
                    GlobalParam.DisplayFormat = Convert.ToInt32(dr["C009"].ToString());

                    optReturn = XMLHelper.SeriallizeObject(GlobalParam);
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

        private OperationReturn GetStatisticalParam(SessionInfo session)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                string rentToken = session.RentInfo.Token;
                string strSql;
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT * FROM T_31_050_{0}", rentToken);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            //optReturn.Message += strSql;
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM T_31_050_{0}", rentToken);
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            //optReturn.Message += strSql;
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
                    StatisticalParam item = new StatisticalParam();
                    item.StatisticalParamID = Convert.ToInt64(dr["C001"]);
                    item.StatisticalParamName = dr["C002"].ToString();
                    item.IsUsed = dr["C006"].ToString();
                    item.IsCombine = dr["C010"].ToString();
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

        private OperationReturn GetAllCombinedParamItems(SessionInfo session)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                string rentToken = session.RentInfo.Token;
                string strSql;
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT * FROM T_31_051_{0} WHERE C004=1 ", rentToken);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            //optReturn.Message += strSql;
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM T_31_051_{0} WHERE C004=1 ", rentToken);
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            //optReturn.Message += strSql;
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
                    StatisticalParamItem item = new StatisticalParamItem();
                    item.StatisticalParamItemID = Convert.ToInt64(dr["C001"]);
                    item.StatisticalParamItemName = dr["C002"].ToString();
                    item.IsCombine = dr["C004"].ToString();
                    item.Formart = (CombStatiParaItemsFormat)Convert.ToInt32(dr["C006"]);
                    //item.strFormart = dr["C006"].ToString();
                    item.Type = (StatisticalParamItemType)Convert.ToInt32(dr["C007"]);
                    //item.strType = dr["C007"].ToString();
                    item.SortID = Convert.ToInt16(dr["C008"]);
                    item.StatisticalParamID = Convert.ToInt64(dr["C010"]);
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

        private OperationReturn GetAddedCombinedParamItems(SessionInfo session)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                string rentToken = session.RentInfo.Token;
                string strSql;
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT * FROM T_31_051_{0} WHERE C004=1 AND C010 LIKE '%311%'", rentToken);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            //optReturn.Message += strSql;
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM T_31_051_{0} WHERE C004=1 AND C010 LIKE '%311%'", rentToken);
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            //optReturn.Message += strSql;
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
                    StatisticalParamItem item = new StatisticalParamItem();
                    item.StatisticalParamItemID = Convert.ToInt64(dr["C001"]);
                    item.StatisticalParamItemName = dr["C002"].ToString();
                    item.IsCombine = dr["C004"].ToString();
                    item.Formart = (CombStatiParaItemsFormat)Convert.ToInt32(dr["C006"]);
                    //item.strFormart = dr["C006"].ToString();
                    item.Type = (StatisticalParamItemType)Convert.ToInt32(dr["C007"]);
                    //item.strType = dr["C007"].ToString();
                    item.SortID = Convert.ToInt16(dr["C008"]);
                    item.StatisticalParamID = Convert.ToInt64(dr["C010"]);
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

        private OperationReturn GetSelectParamItemsInfos(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0  参数大项ID
                if (listParams == null || listParams.Count < 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string ParamID = listParams[0];
                string rentToken = session.RentInfo.Token;
                string strSql;
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT * FROM T_31_051_{0} WHERE C010={1} ", rentToken, ParamID);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            //optReturn.Message += strSql;
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM T_31_051_{0} WHERE C010={1} ", rentToken, ParamID);
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            //optReturn.Message += strSql;
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
                    StatisticalParamItem item = new StatisticalParamItem();
                    item.StatisticalParamItemID = Convert.ToInt64(dr["C001"]);
                    item.StatisticalParamItemName = dr["C002"].ToString();
                    item.IsCombine = dr["C004"].ToString();
                    item.Formart = (CombStatiParaItemsFormat)Convert.ToInt32(dr["C006"]);
                    //item.strFormart = dr["C006"].ToString();
                    item.Type = (StatisticalParamItemType)Convert.ToInt32(dr["C007"]);
                    //item.strType = dr["C007"].ToString();
                    item.SortID = Convert.ToInt16(dr["C008"]);
                    item.StatisticalParamID = Convert.ToInt64(dr["C010"]);
                    item.ValueType = dr["C011"].ToString();
                    item.StatisticDuration = dr["C012"].ToString();
                    item.StatisticUnit = int.Parse(dr["C013"].ToString());
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

        private OperationReturn SaveAddedParamItemsInfos(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0...  查询条件项
                if (listParams == null || listParams.Count < 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string rentToken = session.RentInfo.Token;
                string strSql;
                for (int i = 0; i < listParams.Count; i++)
                {
                    string strCombinedParamItem = listParams[i];
                    optReturn = XMLHelper.DeserializeObject<StatisticalParamItem>(strCombinedParamItem);
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                    StatisticalParamItem tempCombinedParamItem = optReturn.Data as StatisticalParamItem;
                    string ParamID = tempCombinedParamItem.StatisticalParamID.ToString();
                    string SortID = tempCombinedParamItem.SortID.ToString();
                    string ParamItemID = tempCombinedParamItem.StatisticalParamItemID.ToString();
                    switch (session.DBType)
                    {
                        case 2:
                            strSql = string.Format("UPDATE T_31_051_{0} SET C010={1},C008={2} WHERE C001={3}"
                               , rentToken, ParamID, SortID, ParamItemID);
                            optReturn = MssqlOperation.ExecuteSql(session.DBConnectionString, strSql);
                            if (!optReturn.Result)
                            {
                                return optReturn;
                            }
                            optReturn.Message = strSql;
                            break;
                        case 3:
                            strSql = string.Format("UPDATE T_31_051_{0} SET C010={1},C008={2} WHERE C001={3}"
                               , rentToken, ParamID, SortID, ParamItemID);
                            optReturn = OracleOperation.ExecuteSql(session.DBConnectionString, strSql);
                            if (!optReturn.Result)
                            {
                                return optReturn;
                            }
                            optReturn.Message = strSql;
                            break;
                        default:
                            optReturn.Result = false;
                            optReturn.Code = Defines.RET_PARAM_INVALID;
                            optReturn.Message = string.Format("Database type not surpport.\t{0}", session.DBType);
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

        private OperationReturn GetAuthorityParam(SessionInfo session)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0     用户id
                //if (listParams == null || listParams.Count < 1)
                //{
                //    optReturn.Result = false;
                //    optReturn.Code = Defines.RET_PARAM_INVALID;
                //    optReturn.Message = string.Format("Request param is null or count invalid");
                //    return optReturn;
                //}
                string roleID = session.RoleID.ToString();
                string rentToken = session.RentInfo.Token;
                List<string> ResultList = new List<string>();
                string strSql; string strLog = string.Empty;
                DataSet objDataSet;

                #region 获取当前的狗号

                optReturn = GetDoggleNumber();
                if (!optReturn.Result)
                {
                    strLog += string.Format("Get doggle number fail.");
                    optReturn.Message += strLog;
                    return optReturn;
                }
                string doggleNumber = optReturn.Data.ToString();
                strLog += string.Format("{0};", doggleNumber);

                #endregion

                switch (session.DBType)
                {
                    case 2:
                        strSql =
                            string.Format(
                                "SELECT * FROM T_11_003_{0} WHERE C001 = 31 AND C002 IN (SELECT C002 FROM T_11_202_{0} WHERE C001 = {1} AND C003='1') AND C003 LIKE '3108%' ORDER BY C001, C002",
                                rentToken,
                                roleID);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);

                        break;
                    case 3:
                        strSql =
                              string.Format(
                                  "SELECT * FROM T_11_003_{0} WHERE C001 = 31 AND C002 IN (SELECT C002 FROM T_11_202_{0} WHERE C001 = {1} AND C003='1') AND C003 LIKE '3108%' ORDER BY C001, C002",
                                  rentToken,
                                  roleID);
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);

                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("Database type not surpport.\t{0}", session.DBType);
                        return optReturn;
                }
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                objDataSet = optReturn.Data as DataSet;
                if (objDataSet == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("DataSet is null");
                    return optReturn;
                }
                if (objDataSet.Tables == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("DataSet is null");
                    return optReturn;
                }
                if (objDataSet.Tables[0].Rows == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("DataSet is null");
                    return optReturn;
                }
                strLog += strSql;
                foreach (DataRow dr in objDataSet.Tables[0].Rows)
                {
                    #region License 控制

                    string strC006 = dr["C006"].ToString();
                    string strC007 = dr["C007"].ToString();
                    string strC008 = dr["C008"].ToString();
                    if (string.IsNullOrEmpty(strC006))
                    {
                        //C006为空的跳过
                        strLog += string.Format("C006 is empty;");
                        continue;
                    }
                    strC006 = DecryptFromDB(strC006);
                    string[] listC006 = strC006.Split(new[] { ConstValue.SPLITER_CHAR }, StringSplitOptions.None);
                    if (listC006.Length < 3)
                    {
                        //C006无效
                        strLog += string.Format("C006 is invalid;");
                        continue;
                    }
                    long optID = Convert.ToInt64(dr["C002"]);
                    long licID = optID + 1000000000;
                    if (licID.ToString() != listC006[0])
                    {
                        //LicenseID与操作ID不对应
                        strLog += string.Format("LicID not equal;");
                        continue;
                    }
                    if (listC006[2] == "Y")
                    {
                        string strC008Hash = GetMD5HasString(strC008);
                        string strC008Hash8 = strC008Hash.Substring(0, 8);
                        string strLicDoggle = string.Format("{0}{1}", licID, doggleNumber);
                        strC008 = DecryptFromDB(strC008);
                        if (strLicDoggle != strC008)
                        {
                            //与C008不匹配
                            strLog += string.Format("C008 not equal;");
                            continue;
                        }
                        string strDecryptC007 = EncryptionAndDecryption.DecryptStringYKeyIV(strC007, strC008Hash8,
                            strC008Hash8);
                        string[] listC007 = strDecryptC007.Split(new[] { ConstValue.SPLITER_CHAR },
                            StringSplitOptions.None);
                        if (listC007.Length < 2)
                        {
                            //C007无效
                            strLog += string.Format("C007 is invalid;");
                            continue;
                        }
                        if (listC007[1] != "Y")
                        {
                            //没有许可
                            strLog += string.Format("No license;");
                            continue;
                        }
                    }

                    #endregion
                    ResultList.Add(dr["C002"].ToString());
                }
                optReturn.Data = ResultList;
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

        //获得参数子项的值  没用到  可删掉
        private OperationReturn GetParamItemsValue(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0  参数大项ID
                if (listParams == null || listParams.Count < 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string ParamID = listParams[0];
                string rentToken = session.RentInfo.Token;
                string strSql;
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT * FROM T_31_044_{0} WHERE C001 LIKE '311%' ", rentToken, ParamID);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            //optReturn.Message += strSql;
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM T_31_044_{0} WHERE C001 LIKE '311%' ", rentToken, ParamID);
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            //optReturn.Message += strSql;
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
                    StatisticalParamItem item = new StatisticalParamItem();
                    item.StatisticalParamItemID = Convert.ToInt64(dr["C001"]);
                    item.StatisticalParamItemName = dr["C002"].ToString();
                    item.IsCombine = dr["C004"].ToString();
                    item.Formart = (CombStatiParaItemsFormat)Convert.ToInt32(dr["C006"]);
                    //item.strFormart = dr["C006"].ToString();
                    item.Type = (StatisticalParamItemType)Convert.ToInt32(dr["C007"]);
                    //item.strType = dr["C007"].ToString();
                    item.SortID = Convert.ToInt16(dr["C008"]);
                    item.StatisticalParamID = Convert.ToInt64(dr["C010"]);
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

        private OperationReturn ModifyCombinedParamItems(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0  对T_31_044表做的相应操作(Add或者Delete)
                //1  参数大项的ID 对应 T_31_044的C001
                //2  参数子项的ID 对应 T_31_044的C002
                if (listParams == null || listParams.Count < 3)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string StrSign = listParams[0];
                string ParamID = listParams[1];
                string ParamItemID = listParams[2];
                string rentToken = session.RentInfo.Token;
                string strSql;
                //DataSet objDataSet;
                switch (StrSign)
                {
                    case "Add":
                        switch (session.DBType)
                        {
                            case 2:
                                string testSql1 = string.Format("SELECT COUNT(1) FROM T_31_044_{0} WHERE C001 LIKE '311%' AND C002={2}"
                                    , rentToken, ParamID, ParamItemID);
                                OperationReturn testoptReturn1 = MssqlOperation.GetRecordCount(session.DBConnectionString, testSql1);
                                string testSql1_ = string.Format("SELECT COUNT(1) FROM T_31_044_{0} WHERE C001={1} AND C002={2}"
                                    , rentToken, ParamID, ParamItemID);
                                OperationReturn testoptReturn1_ = MssqlOperation.GetRecordCount(session.DBConnectionString, testSql1_);
                                if (testoptReturn1.IntValue == 0)
                                {
                                    strSql = string.Format("INSERT INTO T_31_044_{0} (C001,C002,C003) VALUES ({1},{2},'{3}')"
                                        , rentToken, ParamID, ParamItemID, "N");
                                    optReturn = MssqlOperation.ExecuteSql(session.DBConnectionString, strSql);
                                }
                                if (testoptReturn1.IntValue != 0)
                                {
                                    string strsql__ = string.Format("DELETE FROM T_31_044_{0} WHERE C001 LIKE '311%' AND C002 ={1}",
                                        rentToken, ParamItemID);
                                    OperationReturn optReturn_ = MssqlOperation.ExecuteSql(session.DBConnectionString, strsql__);
                                    if (!optReturn_.Result)
                                    {
                                        return optReturn_;
                                    }
                                    strSql = string.Format("INSERT INTO T_31_044_{0} (C001,C002,C003) VALUES ({1},{2},'{3}')"
                                        , rentToken, ParamID, ParamItemID, "N");
                                    optReturn = MssqlOperation.ExecuteSql(session.DBConnectionString, strSql);
                                }
                                else
                                {
                                    strSql = "ENPTY";
                                    optReturn.Result = true;
                                    return optReturn;
                                }
                                if (!optReturn.Result)
                                {
                                    return optReturn;
                                }
                                optReturn.Message = strSql;
                                break;
                            case 3:
                                string testSql = string.Format("SELECT COUNT(1) FROM T_31_044_{0} WHERE C001 LIKE '311%' AND C002={2}"
                                    , rentToken, ParamID, ParamItemID);
                                OperationReturn testoptReturn = OracleOperation.GetRecordCount(session.DBConnectionString, testSql);
                                string testSql_ = string.Format("SELECT COUNT(1) FROM T_31_044_{0} WHERE C001={1} AND C002={2}"
                                    , rentToken, ParamID, ParamItemID);
                                OperationReturn testoptReturn_ = OracleOperation.GetRecordCount(session.DBConnectionString, testSql_);
                                if (testoptReturn.IntValue == 0)
                                {
                                    strSql = string.Format("INSERT INTO T_31_044_{0} (C001,C002,C003) VALUES ({1},{2},'{3}')"
                                        , rentToken, ParamID, ParamItemID, "N");
                                    optReturn = OracleOperation.ExecuteSql(session.DBConnectionString, strSql);
                                }
                                if (testoptReturn.IntValue != 0)
                                {
                                    string strsql__ = string.Format("DELETE FROM T_31_044_{0} WHERE C001 LIKE '311%' AND C002 ={1}",
                                        rentToken, ParamItemID);
                                    OperationReturn optReturn_ = OracleOperation.ExecuteSql(session.DBConnectionString, strsql__);
                                    if (!optReturn_.Result)
                                    {
                                        return optReturn_;
                                    }
                                    strSql = string.Format("INSERT INTO T_31_044_{0} (C001,C002,C003) VALUES ({1},{2},'{3}')"
                                        , rentToken, ParamID, ParamItemID, "N");
                                    optReturn = OracleOperation.ExecuteSql(session.DBConnectionString, strSql);
                                }
                                else
                                {
                                    strSql = "ENPTY";
                                    optReturn.Result = true;
                                    return optReturn;
                                }
                                if (!optReturn.Result)
                                {
                                    return optReturn;
                                }
                                optReturn.Message = strSql;
                                break;
                            default:
                                optReturn.Result = false;
                                optReturn.Code = Defines.RET_PARAM_INVALID;
                                optReturn.Message = string.Format("Database type not surpport.\t{0}", session.DBType);
                                return optReturn;
                        }
                        break;
                    case "Delete":
                        switch (session.DBType)
                        {
                            case 2:
                                strSql = string.Format("DELETE FROM T_31_044_{0} WHERE C001 LIKE '311%' AND C002={1} "
                                    , rentToken, ParamItemID);
                                optReturn = MssqlOperation.ExecuteSql(session.DBConnectionString, strSql);
                                if (!optReturn.Result)
                                {
                                    return optReturn;
                                }
                                optReturn.Message = strSql;
                                break;
                            case 3:
                                strSql = string.Format("DELETE FROM T_31_044_{0} WHERE C001 LIKE '311%' AND C002={1} "
                                    , rentToken, ParamItemID);
                                optReturn = OracleOperation.ExecuteSql(session.DBConnectionString, strSql);
                                if (!optReturn.Result)
                                {
                                    return optReturn;
                                }
                                optReturn.Message = strSql;
                                break;
                            default:
                                optReturn.Result = false;
                                optReturn.Code = Defines.RET_PARAM_INVALID;
                                optReturn.Message = string.Format("Database type not surpport.\t{0}", session.DBType);
                                return optReturn;
                        }
                        break;
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

        private OperationReturn GetAllParamItemsValue(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0  T_31_044表中C001字段[也就是参数大项ID]
                //1  T_31_044表中C002字段[也就是参数子项ID]
                string rentToken = session.RentInfo.Token;
                string strSql;
                string ParamID = listParams[0];
                string ParamItemID = listParams[1];
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT  A.C011,A.C012,A.C013,B.* FROM T_31_051_{0} A,T_31_044_{0} B WHERE B.C001={1} AND B.C002={2} AND A.C001=B.C002", rentToken, ParamID, ParamItemID);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            //optReturn.Message += strSql;
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        strSql = string.Format("SELECT  A.C011,A.C012,A.C013,B.* FROM T_31_051_{0} A,T_31_044_{0} B WHERE B.C001={1} AND B.C002={2} AND A.C001=B.C002", rentToken, ParamID, ParamItemID);
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            //optReturn.Message += strSql;
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
                    StatisticalParamItemDetail item = new StatisticalParamItemDetail();
                    item.StatisticalParamID = Convert.ToInt64(dr["C001"]);
                    item.StatisticalParamItemID = Convert.ToInt64(dr["C002"]);
                    item.IsUsed = dr["C003"].ToString();
                    item.ValueType = dr["C011"].ToString();
                    item.Value1 = dr["C004"].ToString();
                    if (ParamItemID == "3140000000000000012")
                    {
                        item.Value2 = dr["C005"].ToString();
                        item.Value3 = dr["C006"].ToString();
                    }
                    else 
                    {
                        item.Value2 = dr["C012"].ToString();
                        item.Value3 = dr["C013"].ToString();
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
            return optReturn;
        }

        private OperationReturn GetOrganizationList(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                if (listParams.Count < 2)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Param count invalid");
                    return optReturn;
                }
                string rentToken = session.RentInfo.Token;
                //ListParam
                //0     userID
                //1     parentID( -1:用户所属机构信息）
                string userID = listParams[0];
                string parentID = listParams[1];
                string strSql;
                DataSet objDataSet = null;
                switch (session.DBType)
                {
                    //MSSQL
                    case 2:
                        //所属机构
                        if (parentID == "-1")
                        {
                            strSql = string.Format("SELECT A.* FROM T_11_006_{0} A, T_11_005_{0} B WHERE A.C001 = B.C006 AND B.C001 = {1}"
                                , rentToken
                                , userID);
                        }
                        else
                        {
                            strSql = string.Format("SELECT A.* FROM T_11_006_{0} A, T_11_201_{0} B WHERE A.C001 = B.C004 AND A.C004 = {1} AND B.C003 = {2}",
                                rentToken,
                                parentID,
                                userID);
                        }
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    //ORCL
                    case 3:
                        //所属机构
                        if (parentID == "-1")
                        {
                            strSql = string.Format("SELECT A.* FROM T_11_006_{0} A, T_11_005_{0} B WHERE A.C001 = B.C006 AND B.C001 = {1}"
                                , rentToken
                                , userID);
                        }
                        else
                        {
                            strSql = string.Format("SELECT A.* FROM T_11_006_{0} A, T_11_201_{0} B WHERE A.C001 = B.C004 AND A.C004 = {1} AND B.C003 = {2}",
                                rentToken,
                                parentID,
                                userID);
                        }
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                }
                if (objDataSet == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("DataSet is null");
                    return optReturn;
                }
                List<string> listOrgs = new List<string>();
                for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = objDataSet.Tables[0].Rows[i];

                    string orgInfo = string.Empty;
                    orgInfo = dr["C001"].ToString() + ";";
                    orgInfo += DecryptFromDB(dr["C002"].ToString());

                    listOrgs.Add(orgInfo);
                }
                optReturn.Data = listOrgs;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }

        private OperationReturn GetSkillGroupList(SessionInfo session)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                string rentToken = session.RentInfo.Token;
                //ListParam
                //0     userID
                //1     parentID( -1:用户所属机构信息）
                
                string strSql;
                DataSet objDataSet = null;
                switch (session.DBType)
                {
                    //MSSQL
                    case 2:
                        strSql = string.Format("SELECT * FROM T_11_009_{0} WHERE C000 = 2 AND C004 = 1 ORDER BY C002"
                            , rentToken);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    //ORCL
                    case 3:

                        strSql = string.Format("SELECT * FROM T_11_009_{0} WHERE C000 = 2 AND C004 = 1 ORDER BY C002"
                            , rentToken);
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                }
                if (objDataSet == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("DataSet is null");
                    return optReturn;
                }
                List<string> listOrgs = new List<string>();
                for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = objDataSet.Tables[0].Rows[i];

                    string orgInfo = string.Empty;
                    orgInfo = dr["C001"].ToString() + ";";
                    orgInfo += DecryptFromDB(dr["C006"].ToString()) + ";";
                    orgInfo += dr["C008"].ToString() + ";";
                    orgInfo += dr["C009"].ToString() + ";";
                    orgInfo += dr["C002"].ToString();

                    listOrgs.Add(orgInfo);
                }
                optReturn.Data = listOrgs;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }


        private OperationReturn GetABCDList(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                if (listParams == null || listParams.Count < 1)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string rentToken = session.RentInfo.Token;
                string ItemID = listParams[0];
                string strSql;
                DataSet objDataSet;
                switch (ItemID)
                {
                    case "1":
                        switch (session.DBType)
                        {
                            case 2:
                                strSql = string.Format("SELECT * FROM T_31_050_{0} WHERE C006 = 1 AND C008=0", rentToken);
                                optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                                break;
                            case 3:
                                strSql = string.Format("SELECT * FROM T_31_050_{0} WHERE C006 = 1 AND C008=0", rentToken);
                                optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                                break;
                            default:
                                optReturn.Result = false;
                                optReturn.Code = Defines.RET_PARAM_INVALID;
                                optReturn.Message = string.Format("Object type invalid");
                                return optReturn;
                        }

                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
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
                            StatisticParam item = new StatisticParam();
                            item.StatisticalParamID = Convert.ToInt64(dr["C001"].ToString());
                            item.StatisticalParamName = dr["C002"].ToString();
                            item.TableType = Convert.ToInt32(dr["C011"].ToString());
                            item.IsCombine = dr["C010"].ToString();
                            item.Description = dr["C003"].ToString();
                            //0     orgid
                            //1     IsApplyAll
                            //2     StatisticType
                            //3     STime
                            //4     ETime
                            //5     RowNum
                            //6     CycleTime
                            //7     UpdateTime
                            //8     UpdateUnit
                            //9     运行周期标记C001
                            //10    运行时间点C002
                            //11    运行时间
                            //12    52主键
                            List<string> ListTemp = GetItemOtherInfo(session, dr["C001"].ToString(), ItemID);
                            if (ListTemp.Count == 13)
                            {
                                item.OrgID = Convert.ToInt64(ListTemp[0]);
                                item.IsApplyAll = Convert.ToInt32(ListTemp[1]);
                                item.StatisticType = Convert.ToInt32(ListTemp[2]);
                                item.StartTime = Convert.ToDateTime(ListTemp[3]).ToString();
                                item.EndTime = Convert.ToDateTime(ListTemp[4]).ToString();
                                item.RowNum = Convert.ToInt32(ListTemp[5]);
                                item.CycleTime = Convert.ToInt64(ListTemp[6]);
                                item.UpdateTime = Convert.ToInt32(ListTemp[7]);
                                item.UpdateUnit = Convert.ToInt32(ListTemp[8]);
                                item.CycleTimeParam.Add(ListTemp[9]);
                                item.CycleTimeParam.Add(ListTemp[10]);
                                item.CycleTimeParam.Add(ListTemp[11]);
                                item.StatisticKey = Convert.ToInt64(ListTemp[12]);
                            }
                            optReturn = XMLHelper.SeriallizeObject(item);
                            if (!optReturn.Result)
                            {
                                return optReturn;
                            }
                            listReturn.Add(optReturn.Data.ToString());
                        }
                         optReturn.Message = strSql;
                         optReturn.Data = listReturn;
                        break;
                    default:
                        switch (session.DBType)
                        {
                            case 2:
                                strSql = string.Format("SELECT * FROM T_31_050_{0} WHERE C006 = 1 AND C008=0 AND C001={1}", rentToken, ItemID);
                                optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                                break;
                            case 3:
                                strSql = string.Format("SELECT * FROM T_31_050_{0} WHERE C006 = 1 AND C008=0 AND C001={1}", rentToken, ItemID);
                                optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                                break;
                            default:
                                optReturn.Result = false;
                                optReturn.Code = Defines.RET_PARAM_INVALID;
                                optReturn.Message = string.Format("Object type invalid");
                                return optReturn;
                        }

                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        if (objDataSet == null)
                        {
                            optReturn.Result = false;
                            optReturn.Code = Defines.RET_OBJECT_NULL;
                            optReturn.Message = string.Format("DataSet is null");
                            return optReturn;
                        }
                        listReturn = new List<string>();
                        //for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                        //{
                        DataRow row = objDataSet.Tables[0].Rows[0];
                        StatisticParam items = new StatisticParam();
                        items.StatisticalParamID = Convert.ToInt64(row["C001"].ToString());
                        items.StatisticalParamName = row["C002"].ToString();
                        items.TableType = Convert.ToInt32(row["C011"].ToString());
                        items.IsCombine = row["C010"].ToString();
                        items.Description = row["C003"].ToString();
                        //0     orgid
                        //1     IsApplyAll
                        //2     StatisticType
                        //3     STime
                        //4     ETime
                        //5     RowNum
                        //6     CycleTime
                        //7     UpdateTime
                        //8     UpdateUnit
                        //9     运行周期标记C001
                        //10    运行时间点C002
                        //11    运行时间
                        //12    52主键
                        List<string> ListTemps = GetItemOtherInfo(session, row["C001"].ToString(), ItemID);
                        if (ListTemps.Count == 13)
                        {
                            items.OrgID = Convert.ToInt64(ListTemps[0]);
                            items.IsApplyAll = Convert.ToInt32(ListTemps[1]);
                            items.StatisticType = Convert.ToInt32(ListTemps[2]);
                            items.StartTime = ListTemps[3].ToString();
                            items.EndTime = Convert.ToDateTime(ListTemps[4]).ToString();
                            items.RowNum = Convert.ToInt32(ListTemps[5]);
                            items.CycleTime = Convert.ToInt64(ListTemps[6]);
                            items.UpdateTime = Convert.ToInt32(ListTemps[7]);
                            items.UpdateUnit = Convert.ToInt32(ListTemps[8]);
                            items.CycleTimeParam.Add(ListTemps[9]);
                            items.CycleTimeParam.Add(ListTemps[10]);
                            items.CycleTimeParam.Add(ListTemps[11]);
                            items.StatisticKey = Convert.ToInt64(ListTemps[12]);
                        }
                        optReturn = XMLHelper.SeriallizeObject(items);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        listReturn.Add(optReturn.Data.ToString());
                        //}
                         optReturn.Message = strSql;
                         optReturn.Data = listReturn;
                        break;
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
        //加载51表中所有的小项
        private OperationReturn GetABCDItemList(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                if (listParams == null && listParams.Count < 1)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string rentToken = session.RentInfo.Token;
                string strSql;
                string ItemID = listParams[0];
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT A.*,B.C004 P004,B.C005 P005,B.C006 P006 FROM T_31_051_{0} A,T_31_044_{0} B WHERE A.C001=B.C002 AND B.C003='Y' AND A.C010={1} AND B.C001 LIKE '311%'", rentToken,ItemID);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        break;
                    case 3:
                        strSql = string.Format("SELECT A.*,B.C004 P004,B.C005 P005,B.C006 P006 FROM T_31_051_{0} A,T_31_044_{0} B WHERE A.C001=B.C002 AND B.C003='Y' AND A.C010={1} AND B.C001 LIKE '311%'", rentToken, ItemID);
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("Object type invalid");
                        return optReturn;
                }

                if (!optReturn.Result)
                {
                    return optReturn;
                }
                objDataSet = optReturn.Data as DataSet;
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
                    StatisticalParamItem item = new StatisticalParamItem();
                    item.StatisticalParamID = Convert.ToInt64(dr["C010"].ToString());
                    item.StatisticalParamItemName = dr["C002"].ToString();
                    item.StatisticalParamItemID = Convert.ToInt64(dr["C001"].ToString());
                    item.Type = (StatisticalParamItemType)Convert.ToInt32(dr["C007"].ToString());
                    item.SortID = Convert.ToInt32(dr["C008"].ToString());
                    item.Formart = (CombStatiParaItemsFormat)Convert.ToInt32(dr["C006"].ToString());
                    item.Value = dr["P004"].ToString() + "?" + dr["P005"].ToString() + "?" + dr["P006"].ToString();
                    int type = 0;
                    type = int.TryParse(dr["C011"].ToString(), out type) ? type : 0;
                    item.ValueType = type.ToString();
                    if (type > 1 && dr["C012"].ToString() != null && dr["C012"].ToString() != "")
                    {
                        item.StatisticDuration = dr["C012"].ToString();
                        item.StatisticUnit = Convert.ToInt32(dr["C013"].ToString());
                    }
                    optReturn = XMLHelper.SeriallizeObject(item);
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                    listReturn.Add(optReturn.Data.ToString());
                }
                optReturn.Message = strSql;
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
        //获取是否52表中存在该机构与大项的关系（是否已经配置过）
        private OperationReturn GetOrgItemRelation(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParams
                //0     52表主键
                if (listParams == null || listParams.Count < 1)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string KEYID = listParams[0];

                string rentToken = session.RentInfo.Token;
                string strSql;
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT C001,C002,C004,C005,C006 FROM T_31_044_{0} WHERE C001 = {1} AND C003='Y'", rentToken, KEYID);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        break;
                    case 3:
                        strSql = string.Format("SELECT C001,C002,C004,C005,C006 FROM T_31_044_{0} WHERE C001 = {1} AND C003='Y'", rentToken, KEYID);
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("Object type invalid");
                        return optReturn;
                }

                if (!optReturn.Result)
                {
                    return optReturn;
                }
                objDataSet = optReturn.Data as DataSet;
                if (objDataSet == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("DataSet is null");
                    return optReturn;
                }
                List<string> listReturn = new List<string>();
                optReturn.StringValue = objDataSet.Tables[0].Rows[0][0].ToString();
                for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = objDataSet.Tables[0].Rows[i];
                    listReturn.Add(dr["C002"].ToString());
                    listReturn.Add(string.Format("{0}", dr["C004"].ToString()));
                }
                optReturn.Message = strSql;
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

        private OperationReturn GetABCDInfo(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParams
                //0     52表机构
                if (listParams == null || listParams.Count <1)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string OrgID = listParams[0];
                //string key = listParams[1];
                string rentToken = session.RentInfo.Token;
                string strSql;
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT A.*,B.C002 B002,B.C003 B003,B.C004 B004,B.C005 B005,B.C007 B007 FROM T_31_052_{0} A,T_31_026_{0} B WHERE A.C003 = {1} AND A.C009=B.C001"
                            , rentToken, OrgID);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        break;
                    case 3:
                        strSql = string.Format("SELECT A.*,B.C002 B002,B.C003 B003,B.C004 B004,B.C005 B005,B.C007 B007 FROM T_31_052_{0} A,T_31_026_{0} B WHERE A.C003 = {1} AND A.C009=B.C001"
                            , rentToken, OrgID);
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("Object type invalid");
                        return optReturn;
                }

                if (!optReturn.Result)
                {
                    return optReturn;
                }
                objDataSet = optReturn.Data as DataSet;
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
                    StatisticParam item = new StatisticParam();
                    item.StatisticalParamID = Convert.ToInt64(dr["C002"].ToString());
                    item.OrgID = Convert.ToInt64(dr["C003"].ToString());
                    item.StatisticKey = Convert.ToInt64(dr["C001"].ToString());
                    item.StatisticType = Convert.ToInt32(dr["C004"].ToString());
                    item.IsApplyAll = Convert.ToInt32(dr["C005"].ToString());
                    item.StartTime = Convert.ToDateTime(dr["C006"].ToString()).ToString();
                    item.EndTime = Convert.ToDateTime(dr["C007"].ToString()).ToString();
                    item.RowNum = Convert.ToInt32(dr["C008"].ToString());
                    item.CycleTime = Convert.ToInt64(dr["C009"].ToString());

                    item.UpdateTime = Convert.ToInt32(dr["C010"].ToString());
                    item.UpdateUnit = Convert.ToInt32(dr["C011"].ToString());

                    item.CycleTimeParam.Add(dr["B002"].ToString());
                    item.CycleTimeParam.Add(dr["B003"].ToString());
                    string temp = string.Empty;
                    switch (dr["B002"].ToString())
                    {
                        case "W":
                            temp = dr["B004"].ToString();
                            break;
                        case "M":
                            temp = dr["B005"].ToString();
                            break;
                        case "Y":
                            temp = dr["B007"].ToString();
                            break;
                        default:
                            break;
                    }
                    item.CycleTimeParam.Add(temp);

                    optReturn = XMLHelper.SeriallizeObject(item);
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                    listReturn.Add(optReturn.Data.ToString());
                }
                optReturn.Message = strSql;
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

        private OperationReturn SaveConfig(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            List<string> listMsg = new List<string>();
            try
            {
                //ListParam
                //0  大项
                //1  小项
                //2  小项
                if (listParams == null || listParams.Count < 2)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                StatisticParam SP = new StatisticParam();
                List<StatisticalParamItem> ListSIP = new List<StatisticalParamItem>();
                optReturn = XMLHelper.DeserializeObject<StatisticParam>(listParams[0]);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                SP = optReturn.Data as StatisticParam;
                listMsg.Add(SP.StatisticalParamID.ToString());
                for (int i = 1; i < listParams.Count; i++)
                {
                    string tempSip = listParams[i];
                    optReturn = XMLHelper.DeserializeObject<StatisticalParamItem>(listParams[i]);
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                    StatisticalParamItem Sip = optReturn.Data as StatisticalParamItem;
                    ListSIP.Add(Sip);
                }
                string rentToken = session.RentInfo.Token;
                string strSql;

                if (SP.StatisticKey == 0)//add
                {
                    string strID = string.Empty;
                    #region 找rownum
                    IDbConnection objConn = null;
                    IDbDataAdapter objAdapter = null;
                    DataSet objDataSet = new DataSet();
                    switch (session.DBType)
                    {
                        //MSSQL
                        case 2:
                            strSql = string.Format("SELECT * FROM T_31_053_{0} WHERE C002=0 AND C001={1}",
                                rentToken, SP.TableType.ToString());
                            objConn = MssqlOperation.GetConnection(session.DBConnectionString);
                            objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                            break;
                        //ORCL
                        case 3:
                            strSql = string.Format("SELECT * FROM T_31_053_{0} WHERE C002=0 AND C001={1}",
                                rentToken, SP.TableType.ToString());
                            objConn = OracleOperation.GetConnection(session.DBConnectionString);
                            objAdapter = OracleOperation.GetDataAdapter(objConn, strSql);
                            break;
                    }
                    if (objConn == null
                      || objAdapter == null)
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_OBJECT_NULL;
                        optReturn.Message = string.Format("DBConnection or DBDataAdapter is null");
                        return optReturn;
                    }
                    objAdapter.Fill(objDataSet);
                    if (objDataSet == null)
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_OBJECT_NULL;
                        optReturn.Message = string.Format("No Row To Write Config");
                        //Delet31_26(strID, session);
                        return optReturn;
                    }
                    string rownum = string.Empty;
                    string StrCount = string.Empty;
                    if (SP.TableType == 1)
                    {
                        StrCount = "100";
                    }
                    else { StrCount = "100"; }
                    //53表完全没有数据,添加数据
                    if (objDataSet.Tables[0].Rows.Count == 0)
                    {
                        string ColCode = S3108Consts.CON_WCF_StartID;
                        switch (session.DBType)
                        {
                            //MSSQL
                            case 2:
                                strSql = string.Format("INSERT INTO T_31_053_{0} VALUES('{1}',0,'{2}',0,{3})",
                                    rentToken, SP.TableType.ToString(), ColCode, StrCount);

                                optReturn = MssqlOperation.ExecuteSql(session.DBConnectionString, strSql);
                                break;
                            //ORCL
                            case 3:
                                strSql = string.Format("INSERT INTO T_31_053_{0} VALUES('{1}',0,'{2}',0,{3})",
                                     rentToken, SP.TableType.ToString(), ColCode, StrCount);
                                optReturn = OracleOperation.ExecuteSql(session.DBConnectionString, strSql);
                                break;
                        }
                        if (!optReturn.Result)
                        {
                            optReturn.Result = false;
                            optReturn.Code = Defines.RET_DBACCESS_FAIL;
                            optReturn.Message = string.Format("Insert 31-53 fail");
                            return optReturn;
                        }
                        rownum = "1";
                    }
                    //31-53中有数据保存，需要获取c003后，判断
                    else
                    {
                        string RowC003 = objDataSet.Tables[0].Rows[0]["C003"].ToString();
                        string rowalluse = S3108Consts.CON_WCF_EndID;
                        if (rowalluse == RowC003)//所有列全部被占用
                        {
                            optReturn.Result = false;
                            optReturn.Code = Defines.RET_IN_USE;
                            optReturn.Message = string.Format("All colum is used");
                            return optReturn;
                        }
                        else//还有列没有被占用,
                        {
                            int n;
                            for (n = 0; n < RowC003.Count(); n++)
                            {
                                if (RowC003[n] == '0')
                                {
                                    rownum = (n + 1).ToString();
                                    break;
                                }
                                
                            }
                            string prestr = RowC003.Substring(0, n);
                            string backstr = RowC003.Substring(n + 1, 99 - n);
                            RowC003 = string.Format("{0}1{1}", prestr, backstr);
                            #region //标记为1（已占用）
                            switch (session.DBType)
                            {
                                //MSSQL
                                case 2:
                                    strSql = string.Format("UPDATE T_31_053_{0} SET C003='{2}' WHERE C001={1} AND C002=0 ",
                                        rentToken, SP.TableType.ToString(), RowC003);

                                    optReturn = MssqlOperation.ExecuteSql(session.DBConnectionString, strSql);
                                    break;
                                //ORCL
                                case 3:
                                    strSql = string.Format("UPDATE T_31_053_{0} SET C003='{2}' WHERE C001={1} AND C002=0 ",
                                         rentToken, SP.TableType.ToString(), RowC003);
                                    optReturn = OracleOperation.ExecuteSql(session.DBConnectionString, strSql);
                                    break;
                            }
                            if (!optReturn.Result)
                            {
                                optReturn.Result = false;
                                optReturn.Code = Defines.RET_DBACCESS_FAIL;
                                optReturn.Message += string.Format("Update 31-53 fail");
                                return optReturn;
                            }

                            #endregion
                        }
                    }
                    SP.RowNum = Convert.ToInt32(rownum);
                    listMsg.Add(rownum);
                    #endregion

                    #region 添加31-026
                    switch (session.DBType)
                    {
                        //MSSQL
                        case 2:
                            DbParameter[] mssqlParameters =
                        {
                            MssqlOperation.GetDbParameter("@ainparam00",MssqlDataType.Varchar,5 ),
                            MssqlOperation.GetDbParameter("@ainparam01",MssqlDataType.NVarchar,1024),
                            MssqlOperation.GetDbParameter("@ainparam02",MssqlDataType.NVarchar,1024),
                            MssqlOperation.GetDbParameter("@ainparam03",MssqlDataType.NVarchar,1024),
                            MssqlOperation.GetDbParameter("aoutparam01",MssqlDataType.Varchar,19),
                            MssqlOperation.GetDbParameter("aouterrornumber",MssqlDataType.Bigint,32),
                            MssqlOperation.GetDbParameter("aouterrorstring",MssqlDataType.Varchar,4000)
                        };
                            mssqlParameters[0].Value = session.RentInfo.Token;
                            mssqlParameters[1].Value = SP.CycleTimeParam[0];
                            mssqlParameters[2].Value = SP.CycleTimeParam[2];
                            mssqlParameters[3].Value = SP.CycleTimeParam[1];
                            mssqlParameters[4].Direction = ParameterDirection.Output;
                            mssqlParameters[5].Direction = ParameterDirection.Output;
                            mssqlParameters[6].Direction = ParameterDirection.Output;
                            optReturn = MssqlOperation.ExecuteStoredProcedure(session.DBConnectionString, "P_31_006",
                                mssqlParameters);
                            if (!optReturn.Result)
                            {
                                optReturn.Message += "<P_31_006>";
                                return optReturn;
                            }
                            if (mssqlParameters[5].Value.ToString().Replace(" ","") != "0")
                            {
                                optReturn.Result = false;
                                optReturn.Code = Defines.RET_DBACCESS_FAIL;
                                optReturn.Message = string.Format("add31-26:{0}\t{1}", mssqlParameters[5].Value, mssqlParameters[6].Value);
                                return optReturn;
                            }
                            strID = mssqlParameters[4].Value.ToString();

                            break;
                        //ORCL
                        case 3:
                            DbParameter[] dbParameters =
                        {
                            OracleOperation.GetDbParameter("ainparam00",OracleDataType.Varchar2,5 ),
                            OracleOperation.GetDbParameter("ainparam01",OracleDataType.Nvarchar2,1024),
                            OracleOperation.GetDbParameter("ainparam02",OracleDataType.Nvarchar2,1024),
                            OracleOperation.GetDbParameter("ainparam03",OracleDataType.Varchar2,1024),
                            
                            OracleOperation.GetDbParameter("aoutparam01",OracleDataType.Varchar2,19),
                            OracleOperation.GetDbParameter("aouterrornumber",OracleDataType.Int32,32),
                            OracleOperation.GetDbParameter("aouterrorstring",OracleDataType.Varchar2,1024)
                        };
                            dbParameters[0].Value = session.RentInfo.Token;
                            dbParameters[1].Value = SP.CycleTimeParam[0];
                            dbParameters[2].Value = SP.CycleTimeParam[2];
                            dbParameters[3].Value = SP.CycleTimeParam[1];

                            dbParameters[4].Direction = ParameterDirection.Output;
                            dbParameters[5].Direction = ParameterDirection.Output;
                            dbParameters[6].Direction = ParameterDirection.Output;
                            optReturn = OracleOperation.ExecuteStoredProcedure(session.DBConnectionString, "P_31_006",
                                dbParameters);
                            if (!optReturn.Result)
                            {
                                optReturn.Message += "<P_31_006>";
                                return optReturn;
                            }
                            if (dbParameters[5].Value.ToString().Replace(" ", "") != "0")
                            {
                                optReturn.Result = false;
                                optReturn.Code = Defines.RET_DBACCESS_FAIL;
                                optReturn.Message = string.Format("31-026:{0}\t{1}", dbParameters[5].Value, dbParameters[6].Value);
                               
                                return optReturn;
                            }
                            strID = dbParameters[4].Value.ToString();
                            break;
                    }
                    #endregion
                    SP.CycleTime = Convert.ToInt64(strID);
                    listMsg.Add(strID);

                    #region //保存到31-52
                    switch (session.DBType)
                    {
                        //MSSQL
                        case 2:
                            DbParameter[] mssqlParameters =
                        {
                            MssqlOperation.GetDbParameter("@ainparam00",MssqlDataType.Varchar,5 ),
                            MssqlOperation.GetDbParameter("@ainparam02",MssqlDataType.NVarchar,1024),
                            MssqlOperation.GetDbParameter("@ainparam03",MssqlDataType.NVarchar,1024),
                            MssqlOperation.GetDbParameter("@ainparam04",MssqlDataType.NVarchar,1024),
                            MssqlOperation.GetDbParameter("@ainparam05",MssqlDataType.NVarchar,1024 ),
                            MssqlOperation.GetDbParameter("@ainparam06",MssqlDataType.NVarchar,1024),
                            MssqlOperation.GetDbParameter("@ainparam07",MssqlDataType.NVarchar,1024),
                            MssqlOperation.GetDbParameter("@ainparam08",MssqlDataType.NVarchar,1024),
                            MssqlOperation.GetDbParameter("@ainparam09",MssqlDataType.NVarchar,1024 ),
                            MssqlOperation.GetDbParameter("@ainparam10",MssqlDataType.NVarchar,1024),
                            MssqlOperation.GetDbParameter("@ainparam11",MssqlDataType.NVarchar,1024),

                            MssqlOperation.GetDbParameter("aoutparam01",MssqlDataType.Varchar,19),
                            MssqlOperation.GetDbParameter("aouterrornumber",MssqlDataType.Bigint,32),
                            MssqlOperation.GetDbParameter("aouterrorstring",MssqlDataType.Varchar,1024)
                        };
                            mssqlParameters[0].Value = session.RentInfo.Token;
                            mssqlParameters[1].Value = SP.StatisticalParamID;
                            mssqlParameters[2].Value = SP.OrgID;
                            mssqlParameters[3].Value = SP.StatisticType;
                            mssqlParameters[4].Value = SP.IsApplyAll;
                            mssqlParameters[5].Value = SP.StartTime;
                            mssqlParameters[6].Value = SP.EndTime;
                            mssqlParameters[7].Value = SP.RowNum;
                            mssqlParameters[8].Value = SP.CycleTime;
                            mssqlParameters[9].Value = SP.UpdateTime;
                            mssqlParameters[10].Value = SP.UpdateUnit;
                            mssqlParameters[11].Direction = ParameterDirection.Output;
                            mssqlParameters[12].Direction = ParameterDirection.Output;
                            mssqlParameters[13].Direction = ParameterDirection.Output;
                            optReturn = MssqlOperation.ExecuteStoredProcedure(session.DBConnectionString, "P_31_007",
                                mssqlParameters);
                            if (!optReturn.Result)
                            {
                                optReturn.Message += "<P_31_007>";
                                return optReturn;
                            }
                            if (mssqlParameters[12].Value.ToString().Replace(" ", "") != "0")
                            {
                                optReturn.Result = false;
                                optReturn.Code = Defines.RET_DBACCESS_FAIL;
                                optReturn.Message = string.Format("P31-52:{0}\t{1}", mssqlParameters[12].Value, mssqlParameters[13].Value);
                               
                                return optReturn;
                            }
                            strID = mssqlParameters[11].Value.ToString();
                            break;
                        //ORCL
                        case 3:
                            DbParameter[] dbParameters =
                        {
                            OracleOperation.GetDbParameter("ainparam00",OracleDataType.Varchar2,5 ),
                            OracleOperation.GetDbParameter("ainparam02",OracleDataType.Nvarchar2,1024),
                            OracleOperation.GetDbParameter("ainparam03",OracleDataType.Nvarchar2,1024),
                            OracleOperation.GetDbParameter("ainparam04",OracleDataType.Varchar2,1024),
                            OracleOperation.GetDbParameter("ainparam05",OracleDataType.Nvarchar2,1024),
                            OracleOperation.GetDbParameter("ainparam06",OracleDataType.Nvarchar2,1024),
                            OracleOperation.GetDbParameter("ainparam07",OracleDataType.Varchar2,1024),
                            OracleOperation.GetDbParameter("ainparam08",OracleDataType.Nvarchar2,1024),
                            OracleOperation.GetDbParameter("ainparam09",OracleDataType.Nvarchar2,1024),
                            OracleOperation.GetDbParameter("ainparam10",OracleDataType.Varchar2,1024),
                            OracleOperation.GetDbParameter("ainparam11",OracleDataType.Varchar2,1024),
                            
                            OracleOperation.GetDbParameter("aoutparam01",OracleDataType.Varchar2,19),
                            OracleOperation.GetDbParameter("aouterrornumber",OracleDataType.Int32,32),
                            OracleOperation.GetDbParameter("aouterrorstring",OracleDataType.Varchar2,1024)
                        };
                            dbParameters[0].Value = session.RentInfo.Token;
                            dbParameters[1].Value = SP.StatisticalParamID;
                            dbParameters[2].Value = SP.OrgID;
                            dbParameters[3].Value = SP.StatisticType;
                            dbParameters[4].Value = SP.IsApplyAll;
                            dbParameters[5].Value = SP.StartTime;
                            dbParameters[6].Value = SP.EndTime;
                            dbParameters[7].Value = SP.RowNum;
                            dbParameters[8].Value = SP.CycleTime;
                            dbParameters[9].Value = SP.UpdateTime;
                            dbParameters[10].Value = SP.UpdateUnit;

                            dbParameters[11].Direction = ParameterDirection.Output;
                            dbParameters[12].Direction = ParameterDirection.Output;
                            dbParameters[13].Direction = ParameterDirection.Output;
                            optReturn = OracleOperation.ExecuteStoredProcedure(session.DBConnectionString, "P_31_007", dbParameters);
                            if (!optReturn.Result)
                            {
                                optReturn.Message += "<P_31_007>";
                                return optReturn;
                            }
                            if (dbParameters[12].Value.ToString().Replace(" ", "") != "0")
                            {
                                optReturn.Result = false;
                                optReturn.Code = Defines.RET_DBACCESS_FAIL;
                                optReturn.Message = string.Format("31-52:{0}\t{1}", dbParameters[12].Value, dbParameters[13].Value);
                             
                                return optReturn;
                            }
                            strID = dbParameters[11].Value.ToString();
                            break;
                    }
                    SP.StatisticKey = Convert.ToInt64(strID);
                    #endregion
                    listMsg.Add(strID);
                }
                else//update
                {
                    #region //更新31-26
                    string Key26 = SP.CycleTime.ToString();
                    string cycle = SP.CycleTimeParam[0];
                    string cycleTime = SP.CycleTimeParam[1];
                    string cycleNum = SP.CycleTimeParam[2];
                    string yuju = "";
                    switch (cycle)
                    {
                        case "W":
                            yuju = string.Format(",C004={0}", cycleNum);
                            break;
                        case "M":
                            yuju = string.Format(",C005={0}", cycleNum);
                            break;
                        case "Y":
                            yuju = string.Format(",C017={0}", cycleNum);
                            break;
                        default:
                            break;
                    }
                    DataSet objDataSet = new DataSet();
                    switch (session.DBType)
                    {
                        //MSSQL
                        case 2:
                            strSql = string.Format("UPDATE T_31_026_{0} SET C002='{2}',C003='{3}' {4} WHERE  C001={1} ",
                                rentToken, Key26, cycle, cycleTime, yuju);
                            optReturn = MssqlOperation.ExecuteSql(session.DBConnectionString, strSql);
                            break;
                        //ORCL
                        case 3:
                            strSql = string.Format("UPDATE T_31_026_{0} SET C002='{2}',C003='{3}' {4} WHERE  C001={1} ",
                                rentToken, Key26, cycle, cycleTime, yuju);
                            optReturn = OracleOperation.ExecuteSql(session.DBConnectionString, strSql);
                            break;
                    }
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                    #endregion

                    #region 更新31-052
                    switch (session.DBType)
                    {
                        //MSSQL
                        case 2:
                            strSql = string.Format("UPDATE T_31_052_{0} SET C004={1},C005={2},C006=CAST('{3}' AS datetime),C007=CAST('{4}' AS datetime),C010={5},C011={6} WHERE C001={7} ",
                                rentToken, SP.StatisticType.ToString(), SP.IsApplyAll.ToString()
                                , SP.StartTime.ToString(), SP.EndTime.ToString(), SP.UpdateTime.ToString()
                                , SP.UpdateUnit.ToString(), SP.StatisticKey);
                            optReturn = MssqlOperation.ExecuteSql(session.DBConnectionString, strSql);
                            break;
                        //ORCL
                        case 3:
                            strSql = string.Format("UPDATE T_31_052_{0} SET C004={1},C005={2},C006=TO_DATE('{3}','YYYY/MM/DD HH24:MI:SS'),C007=TO_DATE('{4}','YYYY/MM/DD HH24:MI:SS'),C010={5},C011={6} WHERE C001={7} ",
                                rentToken, SP.StatisticType.ToString(), SP.IsApplyAll.ToString()
                                , SP.StartTime.ToString(), SP.EndTime.ToString(), SP.UpdateTime.ToString()
                                , SP.UpdateUnit.ToString(), SP.StatisticKey);
                            optReturn = OracleOperation.ExecuteSql(session.DBConnectionString, strSql);
                            break;
                    }
                    if (!optReturn.Result)
                    {

                        return optReturn;
                    }
                    #endregion
                }
                #region //更新31-44
                IDbConnection Conn = null;
                IDbDataAdapter Adapter = null;
                DataSet dataSet = new DataSet();
                DbCommandBuilder objCmdBuilder;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT * FROM T_31_044_{0} WHERE C001 = {1}", rentToken, SP.StatisticKey);
                        Conn = MssqlOperation.GetConnection(session.DBConnectionString);
                        Adapter = MssqlOperation.GetDataAdapter(Conn, strSql);
                        objCmdBuilder = MssqlOperation.GetCommandBuilder(Adapter);
                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM T_31_044_{0} WHERE C001 = {1}", rentToken, SP.StatisticKey);
                        Conn = OracleOperation.GetConnection(session.DBConnectionString);
                        Adapter = OracleOperation.GetDataAdapter(Conn, strSql);
                        objCmdBuilder = OracleOperation.GetCommandBuilder(Adapter);
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("Method invalid");
                        return optReturn;
                }
                if (Conn == null || Adapter == null || objCmdBuilder == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("Db object is null");
                    return optReturn;
                }
                objCmdBuilder.ConflictOption = ConflictOption.OverwriteChanges;
                objCmdBuilder.SetAllValues = false;

                dataSet = new DataSet();
                Adapter.Fill(dataSet);
                for (int i = 0; i < ListSIP.Count; i++)
                {
                    bool isAdd = false;
                    var info = ListSIP[i];
                    string paramID = info.StatisticalParamItemID.ToString();
                    DataRow dr =
                        dataSet.Tables[0].Select(string.Format("C002 = {0}", paramID)).FirstOrDefault();
                    if (info.IsUsed == true)
                    {
                        if (dr == null)
                        {
                            isAdd = true;
                            dr = dataSet.Tables[0].NewRow();
                            dr["C001"] = SP.StatisticKey.ToString();
                            dr["C002"] = paramID;
                            dr["C003"] = "Y";
                        }

                        dr["C004"] = info.Value;

                        if (isAdd)
                        {
                            dataSet.Tables[0].Rows.Add(dr);
                        }
                    }
                    else
                    {
                        if (dr != null)
                        {
                            dr.Delete();
                        }
                    }
                }
                Adapter.Update(dataSet);
                dataSet.AcceptChanges();
                #endregion
                optReturn.Data = listMsg;
            }
            catch (Exception ex)
            {
                optReturn.Data = listMsg;
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
                return optReturn;
            }
            return optReturn;
        }

        private OperationReturn DeleteConfig(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                if (listParams == null || listParams.Count < 1)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                optReturn = XMLHelper.DeserializeObject<StatisticParam>(listParams[0]);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                StatisticParam SP = optReturn.Data as StatisticParam;
                string strSql = string.Empty;
                string rentToken = session.RentInfo.Token;
                // 获取key、cycletime、rowid、tabletype
                string KEY = SP.StatisticKey.ToString();
                string CycleTime = SP.CycleTime.ToString();
                string TableType = SP.TableType.ToString();
                string RowID = SP.RowNum.ToString();
                #region 根据rowid和tabletype=c001更新T_31_053的数据。一条
                IDbConnection objConn = null;
                IDbDataAdapter objAdapter = null;
                DataSet objDataSet = new DataSet();
                switch (session.DBType)
                {
                    //MSSQL
                    case 2:
                        strSql = string.Format("SELECT C003 FROM T_31_053_{0} WHERE C002=0 AND C001={1}",
                            rentToken, SP.TableType.ToString());
                        objConn = MssqlOperation.GetConnection(session.DBConnectionString);
                        objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                        break;
                    //ORCL
                    case 3:
                        strSql = string.Format("SELECT C003 FROM T_31_053_{0} WHERE C002=0 AND C001={1}",
                            rentToken, SP.TableType.ToString());
                        objConn = OracleOperation.GetConnection(session.DBConnectionString);
                        objAdapter = OracleOperation.GetDataAdapter(objConn, strSql);
                        break;
                }
                if (objConn == null
                  || objAdapter == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("DBConnection or DBDataAdapter is null");
                    return optReturn;
                }
                objAdapter.Fill(objDataSet);
                if (objDataSet == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("No Row To Write Config");
                    //Delet31_26(strID, session);
                    return optReturn;
                }
                if (objDataSet.Tables[0].Rows.Count == 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message += "No Rownum";
                    return optReturn;
                }
                if (objDataSet.Tables[0].Rows.Count == 1)
                {
                    optReturn.Message += objDataSet.Tables[0].Rows[0][0].ToString();
                }
                string ROW = objDataSet.Tables[0].Rows[0][0].ToString();
                string newRow = string.Empty; int i = 0;
                foreach (char num in ROW)
                {
                    i++; char number = num;
                    if (i.ToString() == RowID)
                    {
                        number = '0';
                    }
                    newRow += number;
                }
                switch (session.DBType)
                {
                    //MSSQL
                    case 2:
                        strSql = string.Format("UPDATE T_31_053_{0} SET C003='{2}'  WHERE  C001={1} ",
                            rentToken, TableType, newRow);
                        optReturn = MssqlOperation.ExecuteSql(session.DBConnectionString, strSql);
                        break;
                    //ORCL
                    case 3:
                        strSql = string.Format("UPDATE T_31_053_{0} SET C003='{2}'  WHERE  C001={1} ",
                            rentToken, TableType, newRow);
                        optReturn = OracleOperation.ExecuteSql(session.DBConnectionString, strSql);
                        break;
                }
                if (!optReturn.Result)
                {
                    optReturn.Message += "update t-31-053 fail";
                    return optReturn;
                }
                #endregion

                #region  //根据cycletime=C001删掉T_31_026中的数据。1条
                switch (session.DBType)
                {
                    //MSSQL
                    case 2:
                        strSql = string.Format("DELETE FROM T_31_026_{0} WHERE C001={1} ",
                            rentToken, CycleTime);
                        optReturn = MssqlOperation.ExecuteSql(session.DBConnectionString, strSql);
                        break;
                    //ORCL
                    case 3:
                        strSql = string.Format("DELETE FROM T_31_026_{0} WHERE C001={1} ",
                            rentToken, CycleTime);
                        optReturn = OracleOperation.ExecuteSql(session.DBConnectionString, strSql);
                        break;
                }
                if (!optReturn.Result)
                {
                    optReturn.Message += "DELETE T-31-026 FAIL";
                    return optReturn;
                }
                #endregion

                #region   //根据key=C001删掉T_31_044中的数据。多条
                switch (session.DBType)
                {
                    //MSSQL
                    case 2:
                        strSql = string.Format("DELETE FROM T_31_044_{0} WHERE C001={1} ",
                            rentToken, KEY);
                        optReturn = MssqlOperation.ExecuteSql(session.DBConnectionString, strSql);
                        break;
                    //ORCL
                    case 3:
                        strSql = string.Format("DELETE FROM T_31_044_{0} WHERE C001={1} ",
                            rentToken, KEY);
                        optReturn = OracleOperation.ExecuteSql(session.DBConnectionString, strSql);
                        break;
                }
                if (!optReturn.Result)
                {
                    optReturn.Message += "DELETE T-31-044 FAIL";
                    return optReturn;
                }
                #endregion

                #region 根据key=C001删掉T_31_052中的数据。1条
                switch (session.DBType)
                {
                    //MSSQL
                    case 2:
                        strSql = string.Format("DELETE FROM T_31_052_{0} WHERE C001={1} ",
                            rentToken, KEY);
                        optReturn = MssqlOperation.ExecuteSql(session.DBConnectionString, strSql);
                        break;
                    //ORCL
                    case 3:
                        strSql = string.Format("DELETE FROM T_31_052_{0} WHERE C001={1} ",
                            rentToken, KEY);
                        optReturn = OracleOperation.ExecuteSql(session.DBConnectionString, strSql);
                        break;
                }
                if (!optReturn.Result)
                {
                    optReturn.Message += "DELETE T-31-052 FAIL";
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

        private OperationReturn SaveParamItemValue(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0,1...  StatisticalParamItemDetail 序列化之后的字符串的一个list
                string rentToken = session.RentInfo.Token;
                string strSql;
                if (listParams.Count == 0)
                {
                    optReturn.Message = "参数少于一个";
                    optReturn.Result = false;
                    return optReturn;
                }
                for (int i = 0; i < listParams.Count; i++)
                {
                    string paramItemDetail = listParams[i];
                    optReturn = XMLHelper.DeserializeObject<StatisticalParamItemDetail>(paramItemDetail);
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                    StatisticalParamItemDetail tempDetail = optReturn.Data as StatisticalParamItemDetail;
                    string StatisticalParamID = tempDetail.StatisticalParamID.ToString();
                    string StatisticalParamItemID = tempDetail.StatisticalParamItemID.ToString();
                    string isUsed = tempDetail.IsUsed;
                    //string valuetype = tempDetail.ValueType;
                    string value1 = tempDetail.Value1;
                    string value2 = tempDetail.Value2;
                    string value3 = tempDetail.Value3;
                    if (tempDetail == null)
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_OBJECT_NULL;
                        optReturn.Message = string.Format("RecordInfo is null");
                        return optReturn;
                    }
                    switch (session.DBType)
                    {
                        case 2:
                            if (StatisticalParamItemID == "3140000000000000012")
                            {
                                strSql = string.Format("UPDATE T_31_044_{0} SET C003='{1}',C004='{2}',C005='{3}',C006='{4}' WHERE C001={5} AND C002={6}",
                                rentToken, isUsed, value1, value2, value3, StatisticalParamID, StatisticalParamItemID);
                                optReturn = MssqlOperation.ExecuteSql(session.DBConnectionString, strSql);
                                if (!optReturn.Result )
                                {
                                    return optReturn;
                                }
                            }
                            else 
                            {
                                strSql = string.Format("UPDATE T_31_044_{0} SET C003='{1}',C004='{2}' WHERE C001={3} AND C002={4}",
                                rentToken, isUsed, value1, StatisticalParamID, StatisticalParamItemID);
                                optReturn = MssqlOperation.ExecuteSql(session.DBConnectionString, strSql);
                                if (value2 == string.Empty)
                                {
                                    value2 = "null";
                                }
                                if (value3 == string.Empty)
                                {
                                    value3 = "null";
                                }
                                string strsql_ = string.Format("UPDATE T_31_051_{0} SET C012={1},C013={2} WHERE C001={3}", rentToken, value2, value3, StatisticalParamItemID);
                                OperationReturn optReturn_ = MssqlOperation.ExecuteSql(session.DBConnectionString, strsql_);
                                if (!optReturn.Result || !optReturn_.Result)
                                {
                                    optReturn.Message = strSql;
                                    optReturn.Result = false;
                                    return optReturn;
                                }
                            }
                            optReturn.Message = strSql;
                            break;
                        case 3:
                            if (StatisticalParamItemID == "3140000000000000012")
                            {
                                strSql = string.Format("UPDATE T_31_044_{0} SET C003='{1}',C004='{2}',C005='{3}',C006='{4}' WHERE C001={5} AND C002={6}",
                                rentToken, isUsed, value1, value2, value3, StatisticalParamID, StatisticalParamItemID);
                                optReturn = OracleOperation.ExecuteSql(session.DBConnectionString, strSql);
                                if (!optReturn.Result)
                                {
                                    return optReturn;
                                }
                            }
                            else
                            {
                                strSql = string.Format("UPDATE T_31_044_{0} SET C003='{1}',C004='{2}' WHERE C001={3} AND C002={4}",
                                rentToken, isUsed, value1, StatisticalParamID, StatisticalParamItemID);
                                optReturn = OracleOperation.ExecuteSql(session.DBConnectionString, strSql);
                                if (value2 == string.Empty)
                                {
                                    value2 = "null";
                                }
                                if (value3 == string.Empty)
                                {
                                    value3 = "null";
                                }
                                string strsql_ = string.Format("UPDATE T_31_051_{0} SET C012={1},C013={2} WHERE C001={3}", rentToken, value2, value3, StatisticalParamItemID);
                                OperationReturn optReturn_ = OracleOperation.ExecuteSql(session.DBConnectionString, strsql_);
                                if (!optReturn.Result || !optReturn_.Result)
                                {
                                    optReturn.Message = strsql_;
                                    optReturn.Result = false;
                                    return optReturn;
                                }
                            }
                            optReturn.Message = strSql;
                            break;
                        default:
                            optReturn.Result = false;
                            optReturn.Code = Defines.RET_PARAM_INVALID;
                            optReturn.Message = string.Format("Database type not surpport.\t{0}", session.DBType);
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

        private OperationReturn ModifyStatisticParam(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0 参数大项的ID
                //1 是否启用 1启用 0不启用
                string rentToken = session.RentInfo.Token;
                string strSql;
                string ParamID = listParams[0];
                string IsUsed = listParams[1];
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("UPDATE T_31_050_{0} SET C006={1} WHERE C001={2}", rentToken, IsUsed, ParamID);
                        optReturn = MssqlOperation.ExecuteSql(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        optReturn.Message = strSql;
                        break;
                    case 3:
                        strSql = string.Format("UPDATE T_31_050_{0} SET C006={1} WHERE C001={2}", rentToken, IsUsed, ParamID);
                        optReturn = OracleOperation.ExecuteSql(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        optReturn.Message = strSql;
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("Database type not surpport.\t{0}", session.DBType);
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
            return optReturn;
        }

        private OperationReturn IsDistributeOrgSkg(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0 参数大项的ID
                string rentToken = session.RentInfo.Token;
                string strSql;
                string ParamID = listParams[0];
                string TimeNow = DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss");
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT COUNT(1) FROM T_31_052_{0}  WHERE C002={1} ", rentToken, ParamID, TimeNow);
                        optReturn = MssqlOperation.GetRecordCount(session.DBConnectionString, strSql);
                        if (optReturn.IntValue == 0)
                        {
                            optReturn.Data = "1";
                        }
                        else
                        {
                            optReturn.Data = "0";
                        }
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        optReturn.Message = strSql;
                        break;
                    case 3:
                        strSql = string.Format("SELECT COUNT(1) FROM T_31_052_{0}  WHERE C002={1} ", rentToken, ParamID, TimeNow);
                        optReturn = OracleOperation.GetRecordCount(session.DBConnectionString, strSql);
                        if (optReturn.IntValue == 0)
                        {
                            optReturn.Data = "1";
                        }
                        else
                        {
                            optReturn.Data = "0";
                        }
                        if (!optReturn.Result)
                        {
                            optReturn.Message = strSql;
                            optReturn.Result = false;
                            return optReturn;
                        }
                        optReturn.Message = strSql;
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("Database type not surpport.\t{0}", session.DBType);
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
            return optReturn;
        }

        #region operation
        private List<string> GetItemOtherInfo(SessionInfo session, string ItemID,string OrgID)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            List<string> ListInfo = new List<string>();
            string rentToken = session.RentInfo.Token;
            string strSql;
            DataSet objDataSet;
            switch (session.DBType)
            {
                case 2:
                    strSql = string.Format("SELECT A.*,B.C002 P002,B.C003 P003,B.C004 P004,B.C005 P005,B.C017 P017 FROM T_31_052_{0} A,T_31_026_{0} B WHERE A.C009=B.C001 AND A.C002={1} AND A.C003={2}", rentToken, ItemID,OrgID);
                    optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                    break;
                case 3:
                    strSql = string.Format("SELECT A.*,B.C002 P002,B.C003 P003,B.C004 P004,B.C005 P005,B.C017 P017 FROM T_31_052_{0} A,T_31_026_{0} B WHERE A.C009=B.C001 AND A.C002={1} AND A.C003={2}", rentToken, ItemID, OrgID);
                    optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                    break;
                default:
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Object type invalid");
                    return ListInfo;
            }

            if (!optReturn.Result)
            {
                return ListInfo;
            }
            objDataSet = optReturn.Data as DataSet;
            if (objDataSet == null)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_OBJECT_NULL;
                optReturn.Message = string.Format("DataSet is null");
                return ListInfo;
            }
            foreach (DataRow row in objDataSet.Tables[0].Rows)
            {
                //0     orgid
                //1     IsApplyAll
                //2     StatisticType
                //3     STime
                //4     ETime
                //5     RowNum
                //6     CycleTime
                //7     UpdateTime
                //8     UpdateUnit
                //9     运行周期标记C002
                //10    运行时间点C003
                //11    运行时间
                //12    52主键
                ListInfo.Add(row["C003"].ToString());
                ListInfo.Add(row["C005"].ToString());
                ListInfo.Add(row["C004"].ToString());
                ListInfo.Add(row["C006"].ToString());
                ListInfo.Add(row["C007"].ToString());
                ListInfo.Add(row["C008"].ToString());
                ListInfo.Add(row["C009"].ToString());
                ListInfo.Add(row["C010"].ToString());
                ListInfo.Add(row["C011"].ToString());
                ListInfo.Add(row["P002"].ToString());
                ListInfo.Add(row["P003"].ToString());
                switch (row[11].ToString())
                {
                    case "D":
                        ListInfo.Add("");
                        break;
                    case "W":
                        ListInfo.Add(row["P004"].ToString());
                        break;
                    case "M":
                        ListInfo.Add(row["P005"].ToString());
                        break;
                    case "Y":
                        ListInfo.Add(row["P017"].ToString());
                        break;
                    default:
                        break;
                }
                ListInfo.Add(row["C001"].ToString());
            }
            return ListInfo;
        }
        #endregion

        #region dog
        private const string PATH_DOGGLECONFIG = "GlobalSettings\\UMP.Young.01";

        private OperationReturn GetDoggleNumber()
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                string path = AppDomain.CurrentDomain.BaseDirectory;
                path = path.Substring(0, path.LastIndexOf("\\"));
                path = path.Substring(0, path.LastIndexOf("\\"));
                path = Path.Combine(path, PATH_DOGGLECONFIG);
                if (!File.Exists(path))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_FILE_NOT_EXIST;
                    optReturn.Message = string.Format("DoggleConfig file not exist.\t{0}", path);
                    return optReturn;
                }
                string[] listContents = File.ReadAllLines(path);
                if (listContents.Length < 1)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_CONFIG_NOT_EXIST;
                    optReturn.Message = string.Format("DoggleConfig not exist.");
                    return optReturn;
                }
                string doggleNumber = listContents[0];
                doggleNumber = DecryptM001(doggleNumber);
                optReturn.Data = doggleNumber;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
                optReturn.Exception = ex;
            }
            return optReturn;
        }

        private string GetMD5HasString(string strSource)
        {
            byte[] byteSource = Encoding.Unicode.GetBytes(strSource);
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] byteReturn = md5.ComputeHash(byteSource);
            string strReturn = Converter.Byte2Hex(byteReturn);
            return strReturn;
        }

        private string DecryptM001(string strSource)
        {
            string strReturn = EncryptionAndDecryption.EncryptDecryptString(strSource,
            CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M101),
            EncryptionAndDecryption.UMPKeyAndIVType.M101);
            return strReturn;
        }
        #endregion

        #region Encryption and Decryption

        private string CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType keyIVID)
        {
            string strReturn;
            Random random = new Random();
            string strTemp;

            try
            {
                strReturn = DateTime.Now.ToString("yyyyMMddHHmmss");
                int intRand = random.Next(0, 14);
                strTemp = intRand.ToString("00");
                strReturn = strReturn.Insert(intRand, "VCT");
                intRand = random.Next(0, 17);
                strTemp += intRand.ToString("00");
                strReturn = strReturn.Insert(intRand, "UMP");
                intRand = random.Next(0, 20);
                strTemp += intRand.ToString("00");
                strReturn = strReturn.Insert(intRand, ((int)keyIVID).ToString("000"));

                strReturn = EncryptionAndDecryption.EncryptStringY(strTemp + strReturn);
            }
            catch { strReturn = string.Empty; }

            return strReturn;
        }

        private string EncryptToDB(string strSource)
        {
            string strReturn = EncryptionAndDecryption.EncryptDecryptString(strSource,
              CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M002),
              EncryptionAndDecryption.UMPKeyAndIVType.M002);
            return strReturn;
        }

        private string DecryptFromDB(string strSource)
        {
            string strReturn = EncryptionAndDecryption.EncryptDecryptString(strSource,
              CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M102),
              EncryptionAndDecryption.UMPKeyAndIVType.M102);
            return strReturn;
        }

        private string EncryptToClient(string strSource)
        {
            string strReturn = EncryptionAndDecryption.EncryptDecryptString(strSource,
             CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004),
             EncryptionAndDecryption.UMPKeyAndIVType.M004);
            return strReturn;
        }

        private string DecryptFromClient(string strSource)
        {
            string strReturn = EncryptionAndDecryption.EncryptDecryptString(strSource,
             CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104),
             EncryptionAndDecryption.UMPKeyAndIVType.M104);
            return strReturn;
        }

        private string EncryptShaToDB(string strSource)
        {
            string strReturn = EncryptionAndDecryption.EncryptStringSHA512(strSource,
             CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M002),
             EncryptionAndDecryption.UMPKeyAndIVType.M002);
            return strReturn;
        }

        #endregion
    }
}
