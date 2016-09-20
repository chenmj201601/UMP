using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.ServiceModel.Activation;
using PFShareClassesS;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common21011;
using VoiceCyber.UMP.Communications;

namespace Wcf21011
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码、svc 和配置文件中的类名“Service1”。
    // 注意: 为了启动 WCF 测试客户端以测试此服务，请在解决方案资源管理器中选择 Service1.svc 或 Service1.svc.cs，然后开始调试。
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public partial class Service21011 : IService21011
    {
        #region Encryption and Decryption
        private string CreateVerificationCodes(EncryptionAndDecryption.UMPKeyAndIVType keyIVID)
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
            string strReturn = EncryptionAndDecryption.EncryptDecryptString(strSource,
                CreateVerificationCodes(EncryptionAndDecryption.UMPKeyAndIVType.M002),
                EncryptionAndDecryption.UMPKeyAndIVType.M002);
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

        public WebReturn DoOperation(WebRequest webRequest)
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
                    case (int)S2101Codes.GetAllowConditions:
                        optReturn = GetAllowConditions(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        List<string> listCons = optReturn.Data as List<string>;
                        webReturn.ListData = listCons;
                        break;
                    case (int)S2101Codes.GetRoleOperationList:
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
                    case (int)S2101Codes.SubmitStrategies:
                        optReturn = SubmitStrategies(session, webRequest.ListData, webRequest.Data);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.Data = optReturn.Data.ToString();
                        break;
                    case (int)S2101Codes.GetFilterWithCreator:
                        optReturn = GetFilterWithCreator(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        List<string> listFwc = optReturn.Data as List<string>;
                        webReturn.ListData = listFwc;
                        break;
                    case (int)S2101Codes.DeleteStrategy:
                        optReturn = DeleteStrategy(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
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

        private OperationReturn GetAllowConditions(SessionInfo session, List<string> listParams)
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

                string targettype = listParams[0];
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, "SELECT * FROM T_00_201");
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, "SELECT * FROM T_00_201");
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
                    StrategyInfo item = new StrategyInfo();
                    item.StrategyType = Convert.ToInt32(dr["C001"]);
                    item.ID = Convert.ToInt32(dr["C002"]);
                    item.FieldName = dr["C003"].ToString();
                    item.FieldType = Convert.ToInt32(dr["C004"]);
                    item.AllowCondition = dr["C011"].ToString();
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

        private OperationReturn SubmitStrategies(SessionInfo session, List<string> listParams, string strData)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;

            try
            {
                //ListParams
                //0 添加策略，1 修改策略
                if (listParams == null || listParams.Count < 1)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                bool mIsAdd = listParams[0] == "0" ? true : false;
                optReturn = XMLHelper.DeserializeObject<AllFilterData>(strData);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                AllFilterData allfilter = optReturn.Data as AllFilterData;
                if (allfilter == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("AllFilterData Is Null");
                    return optReturn;
                }
                string rentToken = session.RentInfo.Token;
                string strSql;
                #region 202Add
                IDbConnection objConn, objConnFc;
                IDbDataAdapter objAdapter, objAdapterFc;
                DbCommandBuilder objCmdBuilder, objCmdBuilderFc;
                switch (session.DBType)
                {
                    case 2:
                        if (mIsAdd)
                            strSql = string.Format("SELECT * FROM T_00_202 WHERE C004 IN ('{0}','{1}') AND C006<>'1'",
                                allfilter.StrategyName.ToUpper(),
                                allfilter.StrategyName.ToLower());
                        else
                            strSql = string.Format("SELECT * FROM T_00_202 WHERE C003 = {0}",
                                allfilter.StrategyCode);
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
                        if (mIsAdd)
                            strSql = string.Format("SELECT * FROM T_00_202 WHERE C004 IN ('{0}','{1}') AND C006<>'1'",
                                allfilter.StrategyName.ToUpper(),
                                allfilter.StrategyName.ToLower());
                        else
                            strSql = string.Format("SELECT * FROM T_00_202 WHERE C003 = {0}",
                                allfilter.StrategyCode);
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
                objCmdBuilder.ConflictOption = ConflictOption.OverwriteChanges;
                objCmdBuilder.SetAllValues = false;
                DataSet objDataSet = new DataSet();
                objAdapter.Fill(objDataSet);
                if (objDataSet.Tables[0].Rows.Count > 0 && mIsAdd)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_ALREADY_EXIST;
                    optReturn.Message = string.Format("Data already exists");
                    return optReturn;
                }
                DataRow drAdd, drModify;
                string tempStrCode = "-1";
                if (mIsAdd)//添加
                {
                    drAdd = objDataSet.Tables[0].NewRow();
                    drAdd["C000"] = rentToken;
                    drAdd["C001"] = allfilter.FilterType;
                    drAdd["C002"] = allfilter.StrategyType;
                    drAdd["C003"] = allfilter.StrategyCode;
                    drAdd["C004"] = EncryptDecryptToDB(allfilter.StrategyName);
                    drAdd["C005"] = allfilter.IsValid;
                    drAdd["C006"] = allfilter.IsDelete;
                    drAdd["C007"] = allfilter.Creator;
                    drAdd["C008"] = allfilter.CreateTime;
                    drAdd["C009"] = allfilter.FilterNumber;
                    drAdd["C010"] = allfilter.DateStart;
                    drAdd["C011"] = allfilter.DateEnd;
                    drAdd["C012"] = allfilter.Remarks;
                    drAdd["C021"] = 0;
                    objDataSet.Tables[0].Rows.Add(drAdd);
                }
                else
                {
                    drModify = objDataSet.Tables[0].Rows[0];
                    tempStrCode = drModify["C003"].ToString();
                    //drModify["C000"] = rentToken;
                    drModify["C001"] = allfilter.FilterType;
                    drModify["C002"] = allfilter.StrategyType;
                    //drModify["C003"] = allfilter.StrategyCode;
                    drModify["C004"] = EncryptDecryptToDB(allfilter.StrategyName);
                    drModify["C005"] = allfilter.IsValid;
                    drModify["C006"] = allfilter.IsDelete;
                    drModify["C007"] = allfilter.Creator;
                    drModify["C008"] = allfilter.CreateTime;
                    //drModify["C009"] = allfilter.FilterNumber;
                    drModify["C010"] = allfilter.DateStart;
                    drModify["C011"] = allfilter.DateEnd;
                    drModify["C012"] = allfilter.Remarks;
                }
                #endregion
                #region 204Add
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT * FROM T_00_204 WHERE C001={0}", tempStrCode);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objConnFc = MssqlOperation.GetConnection(session.DBConnectionString);
                        objAdapterFc = MssqlOperation.GetDataAdapter(objConnFc, strSql);
                        objCmdBuilderFc = MssqlOperation.GetCommandBuilder(objAdapterFc);
                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM T_00_204 WHERE C001={0}", tempStrCode);
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objConnFc = OracleOperation.GetConnection(session.DBConnectionString);
                        objAdapterFc = OracleOperation.GetDataAdapter(objConnFc, strSql);
                        objCmdBuilderFc = OracleOperation.GetCommandBuilder(objAdapterFc);
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("Database type not surpport.\t{0}", session.DBType);
                        return optReturn;
                }
                if (objConnFc == null || objAdapterFc == null || objCmdBuilderFc == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("Db object is null");
                    return optReturn;
                }
                objCmdBuilderFc.ConflictOption = ConflictOption.OverwriteChanges;
                objCmdBuilderFc.SetAllValues = false;
                DataSet objDataSetFc = new DataSet();
                objAdapterFc.Fill(objDataSetFc);
                int tempcount = objDataSetFc.Tables[0].Rows.Count;
                if (!mIsAdd)//修改
                {
                    //objDataSetFc.Tables[0].Rows.Clear();
                    try
                    {
                        switch (session.DBType)
                        {
                            //MSSQL
                            case 2:
                                strSql = string.Format("DELETE FROM T_00_204 WHERE C001={0}", tempStrCode);
                                optReturn = MssqlOperation.ExecuteSql(session.DBConnectionString, strSql);
                                break;
                            //ORCL
                            case 3:
                                strSql = string.Format("DELETE FROM T_00_204 WHERE C001={0}", tempStrCode);
                                optReturn = OracleOperation.ExecuteSql(session.DBConnectionString, strSql);
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
                }
                foreach (CFilterCondition cfc in allfilter.listFilterCondition)
                {
                    DataRow drAddFc = objDataSetFc.Tables[0].NewRow();
                    drAddFc["C001"] = allfilter.StrategyCode;
                    drAddFc["C002"] = cfc.FilterTarget;
                    drAddFc["C003"] = cfc.ID;
                    drAddFc["C005"] = cfc.ConditionName;
                    drAddFc["C006"] = cfc.Operator;
                    drAddFc["C007"] = cfc.isEnum;
                    drAddFc["C008"] = cfc.Value;
                    drAddFc["C009"] = cfc.Logical;
                    objDataSetFc.Tables[0].Rows.Add(drAddFc);
                }
                #endregion
                try
                {
                    objAdapter.Update(objDataSet);
                    objDataSet.AcceptChanges();
                    objAdapterFc.Update(objDataSetFc);
                    objDataSetFc.AcceptChanges();
                }
                catch (Exception ex)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_DBACCESS_FAIL;
                    optReturn.Message = ex.Message;
                }
                finally
                {
                    if (objConn.State == ConnectionState.Open)
                    {
                        objConn.Close();
                    }
                    objConn.Dispose();
                    if (objConnFc.State == ConnectionState.Open)
                    {
                        objConnFc.Close();
                    }
                    objConnFc.Dispose();
                }
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = string.Format(ex.Message + "; " + ex.StackTrace + "; " + ex.Source);
            }
            return optReturn;
        }

        private OperationReturn GetFilterWithCreator(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, "SELECT T202.*,T005.C002 USERCOUNT,T005.C003 USERNAME FROM T_00_202 T202 LEFT JOIN T_11_005_00000 T005 ON T202.C007=T005.C001 WHERE T202.C006<>'1'");
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, "SELECT T202.*,T005.C002 USERCOUNT,T005.C003 USERNAME FROM T_00_202 T202 LEFT JOIN T_11_005_00000 T005 ON T202.C007=T005.C001 WHERE T202.C006<>'1'");
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
                    AllFilterData allfd = new AllFilterData();
                    allfd.FilterType = Convert.ToInt32(dr["C001"]);
                    allfd.StrategyType = dr["C002"].ToString();
                    allfd.StrategyCode = Convert.ToInt64(dr["C003"].ToString());
                    allfd.StrategyName = DecryptFromDB(dr["C004"].ToString());
                    allfd.IsValid = dr["C005"].ToString();
                    allfd.IsDelete = dr["C006"].ToString();
                    allfd.Creator = Convert.ToInt64(dr["C007"].ToString());
                    allfd.CreateTime = dr["C008"].ToString();
                    allfd.FilterNumber = Convert.ToInt32(dr["C009"]);
                    allfd.DateStart = dr["C010"].ToString();
                    allfd.DateEnd = dr["C011"].ToString();
                    allfd.Remarks = dr["C012"].ToString();
                    allfd.CreatorName = DecryptFNames(dr["USERCOUNT"]);
                    #region 获取筛选条件
                    List<CFilterCondition> lstcf = new List<CFilterCondition>();
                    if (objDataSet.Tables[0].Rows.Count > 0)
                    {
                        DataSet objDataSetFc = new DataSet();
                        switch (session.DBType)
                        {
                            case 2:
                                optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, string.Format("SELECT * FROM T_00_204 WHERE C001={0}", allfd.StrategyCode));
                                if (!optReturn.Result)
                                {
                                    return optReturn;
                                }
                                objDataSetFc = optReturn.Data as DataSet;
                                break;
                            case 3:
                                optReturn = OracleOperation.GetDataSet(session.DBConnectionString, string.Format("SELECT * FROM T_00_204 WHERE C001={0}", allfd.StrategyCode));
                                if (!optReturn.Result)
                                {
                                    return optReturn;
                                }
                                objDataSetFc = optReturn.Data as DataSet;
                                break;
                        }
                        if (objDataSetFc != null && objDataSetFc.Tables[0].Rows.Count > 0)
                        {
                            for (int j = 0; j < objDataSetFc.Tables[0].Rows.Count; j++)
                            {
                                DataRow drj = objDataSetFc.Tables[0].Rows[j];
                                CFilterCondition tempfc = new CFilterCondition();
                                tempfc.FilterTarget = Convert.ToInt32(drj["C002"].ToString());
                                tempfc.ID = Convert.ToInt32(drj["C003"].ToString());
                                tempfc.ConditionName = StringPress(drj["C005"]);
                                tempfc.Operator = StringPress(drj["C006"]);
                                tempfc.isEnum = StringPress(drj["C007"]);
                                tempfc.Value = StringPress(drj["C008"]);
                                tempfc.Logical = StringPress(drj["C009"]);
                                lstcf.Add(tempfc);
                            }
                        }
                    }
                    #endregion                    
                    allfd.listFilterCondition = lstcf;
                    optReturn = XMLHelper.SeriallizeObject(allfd);
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

        public OperationReturn DeleteStrategy(SessionInfo session, List<string> listParams)
        {
            //DBConnection or DBDataAdapter is null
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                if (listParams.Count < 1)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Param count invalid");
                    return optReturn;
                }
                string mStrategyCode = listParams[0];
                string strSql;
                DataSet objDataSet = new DataSet();
                string rentToken = session.RentInfo.Token;
                switch (session.DBType)
                {
                    //MSSQL
                    case 2:
                        strSql = string.Format("UPDATE T_00_202 SET C006 = '1' WHERE C003={0};DELETE FROM T_00_204 WHERE C001 ={0}",
                            mStrategyCode);
                        optReturn = MssqlOperation.ExecuteSql(session.DBConnectionString, strSql);
                        break;
                    //ORCL
                    case 3:
                        strSql = string.Format("BEGIN UPDATE T_00_202 SET C006 = '1' WHERE C003={0};DELETE FROM T_00_204 WHERE C001 ={0};END;",
                            mStrategyCode);
                        optReturn = OracleOperation.ExecuteSql(session.DBConnectionString, strSql);
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
        
        #region 数据处理
        private string StringPress(object obj)
        {
            string result = "";
            if (obj != null && obj.ToString().Trim() != "")
            {
                result = obj.ToString();
            }
            return result;
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
        private double? Convert2Double(object obj)
        {
            if (obj == null || string.IsNullOrWhiteSpace(obj.ToString()))
            {
                return null;
            }
            else
                return Convert.ToDouble(obj.ToString());
        }
        private long? Convert2Long(object obj)
        {
            if (obj == null || string.IsNullOrWhiteSpace(obj.ToString()))
            {
                return null;
            }
            else
                return Convert.ToInt64(obj.ToString());
        }
        private int? Convert2Int(object obj)
        {
            try
            {
                return Convert.ToInt32(obj.ToString());
            }
            catch
            {
                return null;
            }
        }
        private DateTime? DatePress(object date)
        {
            DateTime? dt = null;
            if (date != null && !string.IsNullOrEmpty(date.ToString()))
            {
                dt = DateTime.Parse(date.ToString()).ToLocalTime();
            }
            return dt;
        }
        private string StrDatePress(object date)
        {
            string dt = "";
            if (date != null && !string.IsNullOrEmpty(date.ToString()))
            {
                dt = DateTime.Parse(date.ToString()).ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");
            }
            return dt;
        }

        protected Int64 Int64Parse(string str, Int64 defaultValue)
        {
            Int64 outRet = defaultValue;
            if (!Int64.TryParse(str, out outRet))
            {
                outRet = defaultValue;
            }

            return outRet;
        }
        #endregion
    }
}
