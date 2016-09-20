using Common4602;
using PFShareClassesS;
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
using VoiceCyber.Common;
using VoiceCyber.DBAccesses;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;

namespace Wcf46012
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码、svc 和配置文件中的类名“Service1”。
    // 注意: 为了启动 WCF 测试客户端以测试此服务，请在解决方案资源管理器中选择 Service1.svc 或 Service1.svc.cs，然后开始调试。

    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class Service46012 : IService46012
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


        public WebReturn UPMSOperation(WebRequest webRequest)
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
                List<string> listReturn=new List<string>();
                DatabaseInfo dbInfo = session.DatabaseInfo;
                if (dbInfo != null)
                {
                    dbInfo.RealPassword = DecryptFromClient(dbInfo.Password);
                    session.DBConnectionString = dbInfo.GetConnectionString();
                }
                switch (webRequest.Code)
                {
                    case (int)S4602Codes.GetOrgList:
                        optReturn = GetControlOrgInfoList(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S4602Codes.GetCAgentREx:
                        optReturn = GetControlAgentExtension(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S4602Codes.GetCUser:
                        optReturn = GetCUser(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        listReturn = optReturn.Data as List<string>;
                        webReturn.ListData = listReturn;
                        break;
                    case (int)S4602Codes.GetSkillGroup:
                        optReturn = GetSkillGroup(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        listReturn = optReturn.Data as List<string>;
                        webReturn.ListData = listReturn;
                        break;
                    case (int)S4602Codes.GetPMSetting:
                        optReturn = GetPMSetting(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        listReturn = optReturn.Data as List<string>;
                        webReturn.ListData = listReturn;
                        break;
                    case (int)S4602Codes.QueryPMDatas:
                        optReturn = QueryPMDatas(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        listReturn = optReturn.Data as List<string>;
                        webReturn.ListData = listReturn;
                        break;
                    case (int)S4602Codes.GetGlobalSetting:
                        optReturn = GetGlobalSetting(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.Data = optReturn.Data as string;
                        break;
                    case (int)S4602Codes.SaveViewColumnInfos:
                        optReturn = SaveViewColumnInfos(session, webRequest.ListData);
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

        private OperationReturn GetControlOrgInfoList(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0      用户编号
                //1      上级机构编号（-1表示获取当前所属机构信息）
                if (listParams == null || listParams.Count < 2)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strUserID = listParams[0];
                string strParentID = listParams[1];
                string rentToken = session.RentInfo.Token;
                string strSql;
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        if (strParentID == "-1")
                        {
                            strSql = string.Format("SELECT * FROM T_11_006_{0} WHERE C001 IN (SELECT C006 FROM T_11_005_{0} WHERE C001 = {1} AND C007<>'H' )"
                           , rentToken
                           , strUserID);
                        }
                        else
                        {
                            strSql = string.Format("SELECT * FROM T_11_006_{0} WHERE C001 IN (SELECT C004 FROM T_11_201_{0} WHERE C003 = {1}) AND C004 = {2}"
                           , rentToken
                           , strUserID
                           , strParentID);
                        }
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        if (strParentID == "-1")
                        {
                            strSql = string.Format("SELECT * FROM T_11_006_{0} WHERE C001 IN (SELECT C006 FROM T_11_005_{0} WHERE C001 = {1} AND C007<>'H')"
                           , rentToken
                           , strUserID);
                        }
                        else
                        {
                            strSql = string.Format("SELECT * FROM T_11_006_{0} WHERE C001 IN (SELECT C004 FROM T_11_201_{0} WHERE C003 = {1}) AND C004 = {2}"
                           , rentToken
                           , strUserID
                           , strParentID);
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
                    string strID = dr["C001"].ToString();
                    string strName = dr["C002"].ToString();
                    strName = DecryptNamesFromDB(strName);
                    string ParentID = dr["C004"].ToString();
                    string strInfo = string.Format("{0}{1}{2}{1}{3}", strID, ConstValue.SPLITER_CHAR, strName, ParentID);
                    listReturn.Add(strInfo);
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

        private OperationReturn GetControlAgentExtension(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0      用户编号
                //1      所属机构编号
                //2     A,代表座席,E,虚拟分机,R,代表真实分机
                //3     机构下属AER，G/技能组下属AER，S
                if (listParams == null || listParams.Count < 2)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strUserID = listParams[0];
                string strParentID = listParams[1];
                string rentToken = session.RentInfo.Token;
                string path1 = string.Empty;
                string path2 = string.Empty;
                string strSql = string.Empty;
                if (listParams[2] == "A")
                {
                    path1 = string.Format("1030000000000000000");
                    path2 = string.Format("1040000000000000000");
                }
                else if (listParams[2] == "E")
                {
                    path1 = string.Format("1040000000000000000");
                    path2 = string.Format("1050000000000000000");
                }
                else if (listParams[2] == "R")
                {
                    path1 = string.Format("1050000000000000000");
                    path2 = string.Format("1060000000000000000");
                }
                if (listParams[3] == "G")//查机构
                {
                    strSql = string.Format("SELECT * FROM T_11_101_{0} WHERE C011 = '{1}' AND C012='1' AND C001 IN (SELECT C004 FROM T_11_201_{0} WHERE C003 = {2}) and C001 >= {3} and c001 < {4}"
                                    , rentToken, strParentID, strUserID, path1, path2);
                }
                else if (listParams[3] == "S")//查技能组
                {
                    strSql = string.Format("SELECT * FROM T_11_101_{0} WHERE C001 IN (SELECT C004 FROM T_11_201_{0} WHERE C003={1} AND C004 IN (SELECT C004 FROM T_11_201_{0} WHERE C003 = {2})) AND C002 =1 and C001 >= {3} and c001 < {4}",
                        rentToken, strParentID, strUserID, path1, path2);
                }
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
                    string strID = dr["C001"].ToString();
                    string strName = dr["C017"].ToString();
                    string strFullName = dr["C018"].ToString();
                    strName = DecryptNamesFromDB(strName);
                    strFullName = DecryptNamesFromDB(strFullName);
                    if (listParams[2] == "E" || listParams[2] == "R")
                    {
                        strFullName = strName;
                    }
                    string strInfo = string.Format("{0}{1}{2}{1}{3}", strID, ConstValue.SPLITER_CHAR, strName, strFullName);
                    listReturn.Add(strInfo);
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

        private OperationReturn GetCUser(SessionInfo session, List<string> listParams)
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
                if (listParams == null || listParams.Count < 1)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string OrgID = listParams[0];
                string rentToken = session.RentInfo.Token;
                string strSql;
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT * FROM T_11_005_{0} WHERE  C006={1}  AND C007 <> 'H' ", rentToken, OrgID);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM T_11_005_{0} WHERE  C006={1}  AND C007 <> 'H' ", rentToken, OrgID);
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
                    UERAListInfo ctrolU = new UERAListInfo();
                    ctrolU.ID = dr["C001"].ToString();
                    ctrolU.Name = DecryptNamesFromDB(dr["C002"].ToString());
                    ctrolU.FullName = DecryptNamesFromDB(dr["C003"].ToString());
                    ctrolU.OrgID = OrgID;
                    optReturn = XMLHelper.SeriallizeObject(ctrolU);
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

        private OperationReturn GetPMSetting(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParams
                //0     
                //if (listParams == null || listParams.Count < 1)
                //{
                //    optReturn.Result = false;
                //    optReturn.Code = Defines.RET_PARAM_INVALID;
                //    optReturn.Message = string.Format("Request param is null or count invalid");
                //    return optReturn;
                //}
                string type = listParams[0];
                string rentToken = session.RentInfo.Token;
                string strSql = string.Empty; ;
                if (type == "1")//可用Pm Setting
                {
                    strSql = string.Format("SELECT T461.C002 KpiName,T461.C010 KpiFormat,T463.* FROM T_46_003_{0} T463 LEFT JOIN T_46_001_{0} T461 ON T461.C001=T463.c002 WHERE T463.C006='1' AND T463.C011='0' ", rentToken);
                }
                else if (type == "0")
                {
                    strSql = string.Format("SELECT T461.C002 KpiName,T461.C010 KpiFormat,T463.* FROM T_46_003_{0} T463 LEFT JOIN T_46_001_{0} T461 ON T461.C001=T463.c002 WHERE T463.C011='1' ", rentToken);
                }
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
                List<string> listOpts = new List<string>();
                for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = objDataSet.Tables[0].Rows[i];
                    PMItems Items = new PMItems();
                    Items.KPIMappingID = dr["C001"].ToString();
                    Items.KPIID = Convert.ToInt64(dr["C002"]);
                    Items.KPIName = dr["KpiName"].ToString();
                    Items.KPIFormat = Convert.ToInt32(dr["KpiFormat"]);
                    Items.ObjectID = Convert.ToInt64(dr["C003"]);
                    Items.KPITypeID = dr["C004"].ToString();
                    Items.KPICycle = dr["C005"].ToString();
                    Items.StartTime = dr["C007"].ToString();
                    Items.StopTime = dr["C008"].ToString();
                    Items.dropDown = dr["C009"].ToString();
                    Items.GoalValue1 = dr["C015"].ToString();
                    Items.GoalValue2 = dr["C019"].ToString();
                    Items.GoalValue3 = dr["C023"].ToString();
                    Items.KpiObjectID = Items.ObjectID.ToString();
                    optReturn = XMLHelper.SeriallizeObject(Items);
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

        private OperationReturn GetSkillGroup(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
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
                        strSql = string.Format("SELECT * FROM T_11_009_{0} WHERE C000 ='2' ", rentToken);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        optReturn.StringValue = strSql;
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;

                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM T_11_009_{0} WHERE C000 ='2' ", rentToken);
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        optReturn.StringValue = strSql;
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
                    optReturn.StringValue = strSql;
                    return optReturn;
                }
                List<SkillGroupInfo> tempList = new List<SkillGroupInfo>();
                for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = objDataSet.Tables[0].Rows[i];
                    SkillGroupInfo item = new SkillGroupInfo();
                    item.SkillGroupID = dr["C001"].ToString();
                    if (dr["C006"] != null)
                    {
                        item.SkillGroupCode = DecryptNamesFromDB(dr["C006"].ToString());
                    }
                    item.SkillGroupName = dr["C008"].ToString();
                    tempList.Add(item);
                }
                List<string> listReturn = new List<string>();
                for (int i = 0; i < tempList.Count; i++)
                {
                    var item = tempList[i];
                    optReturn = XMLHelper.SeriallizeObject(item);
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                    listReturn.Add(optReturn.Data.ToString());
                }
                optReturn.Data = listReturn;
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

        private OperationReturn QueryPMDatas(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParams
                //0     周期类型
                //1     KPIMappingID--string
                //2     对象ID
                //3     开始时间
                //4     结束时间
                //5
                if (listParams == null || listParams.Count < 1)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string rentToken = session.RentInfo.Token;
                string strSql = string.Empty;
                long startTime = Convert.ToInt64(listParams[3]);
                long endTime = Convert.ToInt64(listParams[4]);
                string KPIMappingStr = string.Empty;//kpi 查询条件，优化查询时间用的 ，尽量避免使用in
                if (listParams[1].Split(',').Count() == 1)
                {
                    KPIMappingStr = string.Format("={0}", listParams[1]);
                }
                else if (listParams[1].Split(',').Count() >= 2)
                {
                    KPIMappingStr = string.Format(" in({0})", listParams[1]);
                }
                string objectIDStr = string.Empty;//对象 查询条件,优化查询时间用的 ，尽量避免使用in
                if (listParams[2].Split(',').Count() == 1)
                {
                    objectIDStr = string.Format("={0}", listParams[2]);
                }
                else if (listParams[2].Split(',').Count() >= 2)
                {
                    objectIDStr = string.Format(" in({0})", listParams[2]);
                }
                DataSet objDataSet;
                List<string> listOpts = new List<string>();
                switch (listParams[0])
                {
                    case "1"://年
                        #region Year
                        strSql = string.Format("SELECT * FROM T_46_015_{0} T4615 WHERE T4615.C117 {1} AND T4615.C002{2} AND T4615.C005>={3} AND T4615.C005<={4} ORDER BY T4615.C117,T4615.C005,T4615.C002",
                            rentToken, KPIMappingStr, objectIDStr, startTime, endTime);
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
                        for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                        {
                            DataRow dr = objDataSet.Tables[0].Rows[i];
                            PMShowDataItems Items = new PMShowDataItems();
                            Items.KPIMappingID = Convert.ToInt64(dr["C001"]);
                            Items.KPIID = Convert.ToInt64(dr["C117"]);
                            Items.UERAId = Convert.ToInt64(dr["C002"]);
                            Items.StartLocalTime = Convert.ToInt64(dr["C005"]);
                            Items.BelongsId = Convert.ToInt64(dr["C009"]);
                            Items.ActualValue = string.IsNullOrWhiteSpace(dr["C101"].ToString()) ? 0.0 : Convert.ToDouble(dr["C101"].ToString());
                            Items.Goal1 = dr["C102"].ToString();
                            if (!string.IsNullOrWhiteSpace(dr["C104"].ToString()))
                            {
                                Items.Trend1 = dr["C104"].ToString();
                                if (Items.Trend1 == "2")
                                {
                                    Items.Trend1 = string.Empty;
                                }
                            }
                            Items.ActualGoal1 = dr["C105"].ToString();
                            Items.BoundaryShow1 = dr["C106"].ToString();
                            Items.Goal2 = dr["C107"].ToString();
                            Items.Compare = dr["C109"].ToString();
                            //Items.Trend2 = string.IsNullOrWhiteSpace(dr["C109"].ToString()) ? 0.0 : Convert.ToDouble(dr["C109"].ToString()); 
                            Items.ActualGoal2 = dr["C110"].ToString();
                            Items.BoundaryShow2 = dr["C111"].ToString();
                            Items.Goal3= dr["C112"].ToString();
                            Items.Trend3 = string.IsNullOrWhiteSpace(dr["C114"].ToString()) ? 0.0 : Convert.ToDouble(dr["C114"].ToString()); 
                            Items.ActualGoal3 = dr["C115"].ToString();
                            Items.BoundaryShow3 = dr["C116"].ToString();
                            optReturn = XMLHelper.SeriallizeObject(Items);
                            if (!optReturn.Result)
                            {
                                return optReturn;
                            }
                            listOpts.Add(optReturn.Data.ToString());
                        }
                        #endregion
                        break;
                    case "2"://月
                        #region Month
                        strSql = string.Format("SELECT * FROM T_46_014_{0} T4614 WHERE T4614.C117 {1} AND T4614.C002{2} AND T4614.C005>={3} AND T4614.C005<={4} ORDER BY T4614.C117,T4614.C005,T4614.C002",
                            rentToken, KPIMappingStr, objectIDStr, startTime, endTime);
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
                        for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                        {
                            DataRow dr = objDataSet.Tables[0].Rows[i];
                            PMShowDataItems Items = new PMShowDataItems();
                            Items.KPIMappingID = Convert.ToInt64(dr["C001"]);
                            Items.KPIID = Convert.ToInt64(dr["C117"]);
                            Items.UERAId = Convert.ToInt64(dr["C002"]);
                            Items.StartLocalTime = Convert.ToInt64(dr["C005"]);
                            Items.pmYear = Convert.ToInt32(dr["C007"]);
                            Items.pmMonth = Convert.ToInt32(dr["C008"]);
                            Items.pmDay = Convert.ToInt32(dr["C009"]);
                            Items.BelongsId = Convert.ToInt64(dr["C012"]);
                            Items.ActualValue = string.IsNullOrWhiteSpace(dr["C101"].ToString()) ? 0.0 : Convert.ToDouble(dr["C101"].ToString());
                            Items.Goal1 = dr["C102"].ToString();
                            if (!string.IsNullOrWhiteSpace(dr["C104"].ToString()))
                            {
                                Items.Trend1 = dr["C104"].ToString();
                                if (Items.Trend1 == "2")
                                {
                                    Items.Trend1 = string.Empty;
                                }
                            }
                            Items.ActualGoal1 = dr["C105"].ToString();
                            Items.BoundaryShow1 = dr["C106"].ToString();
                            Items.Goal2 = dr["C107"].ToString();
                            Items.Compare = dr["C109"].ToString();
                            //Items.Trend2 = string.IsNullOrWhiteSpace(dr["C109"].ToString()) ? 0.0 : Convert.ToDouble(dr["C109"].ToString());
                            Items.ActualGoal2 = dr["C110"].ToString();
                            Items.BoundaryShow2 = dr["C111"].ToString();
                            Items.Goal3= dr["C112"].ToString();
                            Items.Trend3 = string.IsNullOrWhiteSpace(dr["C114"].ToString()) ? 0.0 : Convert.ToDouble(dr["C114"].ToString());
                            Items.ActualGoal3 = dr["C115"].ToString();
                            Items.BoundaryShow3 = dr["C116"].ToString();
                            optReturn = XMLHelper.SeriallizeObject(Items);
                            if (!optReturn.Result)
                            {
                                return optReturn;
                            }
                            listOpts.Add(optReturn.Data.ToString());
                        }
                        #endregion
                        break;
                    case "3"://周
                        #region Week
                        strSql = string.Format("SELECT * FROM T_46_013_{0} T4613 WHERE T4613.C117 {1} AND T4613.C002{2} AND T4613.C005>={3} AND T4613.C005<={4}  ORDER BY T4613.C117,T4613.C005,T4613.C002",
                            rentToken, KPIMappingStr, objectIDStr, startTime, endTime);
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
                        for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                        {
                            DataRow dr = objDataSet.Tables[0].Rows[i];
                            PMShowDataItems Items = new PMShowDataItems();
                            Items.KPIMappingID = Convert.ToInt64(dr["C001"]);
                            Items.KPIID = Convert.ToInt64(dr["C117"]);
                            Items.UERAId = Convert.ToInt64(dr["C002"]);
                            Items.StartLocalTime = Convert.ToInt64(dr["C005"]);
                            Items.pmYear = Convert.ToInt32(dr["C007"]);
                            Items.pmMonth = Convert.ToInt32(dr["C008"]);
                            Items.pmDay = Convert.ToInt32(dr["C009"]);
                            Items.BelongsId = Convert.ToInt64(dr["C012"]);
                            Items.ActualValue = string.IsNullOrWhiteSpace(dr["C101"].ToString()) ? 0.0 : Convert.ToDouble(dr["C101"].ToString());
                            Items.Goal1 = dr["C102"].ToString();
                            if (!string.IsNullOrWhiteSpace(dr["C104"].ToString()))
                            {
                                Items.Trend1 = dr["C104"].ToString();
                                if (Items.Trend1 == "2")
                                {
                                    Items.Trend1 = string.Empty;
                                }
                            }
                            Items.ActualGoal1 = dr["C105"].ToString();
                            Items.BoundaryShow1 = dr["C106"].ToString();
                            Items.Goal2 = dr["C107"].ToString();
                            Items.Compare = dr["C109"].ToString();
                            //Items.Trend2 = string.IsNullOrWhiteSpace(dr["C109"].ToString()) ? 0.0 : Convert.ToDouble(dr["C109"].ToString()); 
                            Items.ActualGoal2 = dr["C110"].ToString();
                            Items.BoundaryShow2 = dr["C111"].ToString();
                            Items.Goal3 = dr["C112"].ToString();
                            Items.Trend3 = string.IsNullOrWhiteSpace(dr["C114"].ToString()) ? 0.0 : Convert.ToDouble(dr["C114"].ToString()); 
                            Items.ActualGoal3 = dr["C115"].ToString();
                            Items.BoundaryShow3 = dr["C116"].ToString();
                            optReturn = XMLHelper.SeriallizeObject(Items);
                            if (!optReturn.Result)
                            {
                                return optReturn;
                            }
                            listOpts.Add(optReturn.Data.ToString());
                        }
                        #endregion
                        break;
                    case "4"://天
                        #region Day
                        strSql = string.Format("SELECT * FROM T_46_012_{0} T4612 WHERE T4612.C117 {1} AND T4612.C002{2} AND T4612.C005>={3} AND T4612.C005<={4}  ORDER BY T4612.C117,T4612.C005,T4612.C002",
                            rentToken, KPIMappingStr, objectIDStr, startTime, endTime);
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
                        for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                        {
                            DataRow dr = objDataSet.Tables[0].Rows[i];
                            PMShowDataItems Items = new PMShowDataItems();
                            Items.KPIMappingID = Convert.ToInt64(dr["C001"]);
                            Items.KPIID = Convert.ToInt64(dr["C117"]);
                            Items.UERAId = Convert.ToInt64(dr["C002"]);
                            Items.StartLocalTime = Convert.ToInt64(dr["C005"]);
                            Items.pmYear = Convert.ToInt32(dr["C007"]);
                            Items.pmMonth = Convert.ToInt32(dr["C008"]);
                            Items.pmDay = Convert.ToInt32(dr["C009"]);
                            Items.BelongsId = Convert.ToInt64(dr["C012"]);
                            Items.ActualValue = string.IsNullOrWhiteSpace(dr["C101"].ToString()) ? 0.0 : Convert.ToDouble(dr["C101"].ToString());
                            Items.Goal1 = dr["C102"].ToString();
                            if (!string.IsNullOrWhiteSpace(dr["C104"].ToString()))
                            {
                                Items.Trend1 = dr["C104"].ToString();
                                if (Items.Trend1 == "2")
                                {
                                    Items.Trend1 = string.Empty;
                                }
                            }
                            Items.ActualGoal1 = dr["C105"].ToString();
                            Items.BoundaryShow1 = dr["C106"].ToString();
                            Items.Goal2 = dr["C107"].ToString();
                            Items.Compare = dr["C109"].ToString();
                            //Items.Trend2 = string.IsNullOrWhiteSpace(dr["C109"].ToString()) ? 0.0 : Convert.ToDouble(dr["C109"].ToString()); 
                            Items.ActualGoal2 = dr["C110"].ToString();
                            Items.BoundaryShow2 = dr["C111"].ToString();
                            Items.Goal3 = dr["C112"].ToString();
                            Items.Trend3 = string.IsNullOrWhiteSpace(dr["C114"].ToString()) ? 0.0 : Convert.ToDouble(dr["C114"].ToString());
                            Items.ActualGoal3 = dr["C115"].ToString();
                            Items.BoundaryShow3 = dr["C116"].ToString();
                            optReturn = XMLHelper.SeriallizeObject(Items);
                            if (!optReturn.Result)
                            {
                                return optReturn;
                            }
                            listOpts.Add(optReturn.Data.ToString());
                        }
                        #endregion
                        break;
                    case "5"://1小时
                    case "6":
                    case "7":
                    case "8":
                    case "9":
                        #region 小时之后的片段处理 
                        /*2016-07-08 10:23:13修改，因为之前的方法是遍历46_11表满足条件的所有记录，这样就会至少有2条，然后再拆分这些记录为单个周期的记录。然后wpf后台处理逻辑会把机构的数据再进行递归查询，这样就会重复查询许多次。
                         * 现在只查单个周期，不需要多个周期的数据进行比较，所以把c002字段值跟那个周期对应的字段查出就好了，就只会产生一条数据，即时递归查询也不会有问题。
                        strSql = string.Format("SELECT * FROM T_46_011_{0} T4611 WHERE T4611.C015 IN({1}) AND T4611.C004=({5}) AND T4611.C003 IN ({2}) AND T4611.C007>={3} AND T4611.C007<={4} " +
                        "ORDER BY T4611.C015,T4611.C007,T4611.C005,T4611.C003",rentToken, listParams[1], listParams[2], startTime, endTime, listParams[0]);   
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
                        int dateBaseLines = 0;
                        for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                        {
                            DataRow dr = objDataSet.Tables[0].Rows[i];
                            DateTime tempLocalTime = DateTime.ParseExact(dr["C007"].ToString(), "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture);
                            for (int s = 1; s <= 12; s++)
                            {
                                string tempCol = string.Empty;
                                PMShowDataItems Items = new PMShowDataItems();
                                Items.KPIMappingID = Convert.ToInt64(dr["C001"]);
                                Items.KPIID = Convert.ToInt64(dr["C015"]);
                                dateBaseLines = Convert.ToInt32(dr["C002"]);
                                Items.UERAId = Convert.ToInt64(dr["C003"]);
                                Items.pmYear = Convert.ToInt32(dr["C009"]);
                                Items.pmMonth = Convert.ToInt32(dr["C010"]);
                                Items.pmDay = Convert.ToInt32(dr["C011"]);
                                Items.BelongsId = Convert.ToInt64(dr["C014"]);
                                if (s < 10)
                                {
                                    tempCol = string.Format("C0{0}", s);
                                }
                                else
                                {
                                    tempCol = string.Format("C{0}", s);
                                }
                                switch (listParams[0])
                                {
                                    case "5"://1H
                                        Items.StartLocalTime = Convert.ToInt64(tempLocalTime.AddHours((s - 1) * 1.0 + 12 * (dateBaseLines - 1)).ToString("yyyyMMddHHmmss"));
                                        break;
                                    case "6"://30Min
                                        Items.StartLocalTime = Convert.ToInt64(tempLocalTime.AddHours((s - 1) * 0.5 + 6 * (dateBaseLines - 1)).ToString("yyyyMMddHHmmss"));
                                        break;
                                    case "7"://15Min
                                        Items.StartLocalTime = Convert.ToInt64(tempLocalTime.AddMinutes((s - 1) * 15.0 + 12 *15* (dateBaseLines - 1)).ToString("yyyyMMddHHmmss"));
                                        break;
                                    case "8"://10Min
                                        Items.StartLocalTime = Convert.ToInt64(tempLocalTime.AddMinutes((s - 1) * 10.0 + 120 * (dateBaseLines - 1)).ToString("yyyyMMddHHmmss"));
                                        break;
                                    case "9"://5Min
                                        Items.StartLocalTime = Convert.ToInt64(tempLocalTime.AddMinutes((s - 1) * 5.0 + 60 * (dateBaseLines - 1)).ToString("yyyyMMddHHmmss"));
                                        break;
                                    default:
                                        Items.StartLocalTime = Convert.ToInt64(tempLocalTime.ToString("yyyyMMddHHmmss"));
                                        break;
                                }
                                Items.ActualValue = string.IsNullOrWhiteSpace(dr[string.Format("{0}01", tempCol)].ToString()) ? 0.0 : Convert.ToDouble(dr[string.Format("{0}01", tempCol)].ToString());
                                Items.Goal1 = dr[string.Format("{0}02", tempCol)].ToString();
                                if (!string.IsNullOrWhiteSpace(dr[string.Format("{0}04", tempCol)].ToString()))
                                {
                                    Items.Trend1 = dr[string.Format("{0}04", tempCol)].ToString();
                                    if (Items.Trend1 == "2")
                                    {
                                        Items.Trend1 = string.Empty;
                                    }
                                }
                                //Items.Trend1 = string.IsNullOrWhiteSpace(dr[string.Format("{0}04", tempCol)].ToString()) ? 0.0 : Convert.ToDouble(dr[string.Format("{0}04", tempCol)].ToString());
                                Items.ActualGoal1 = dr[string.Format("{0}05", tempCol)].ToString();
                                Items.BoundaryShow1 = dr[string.Format("{0}06", tempCol)].ToString();
                                Items.Goal2 = dr[string.Format("{0}07", tempCol)].ToString();
                                Items.Compare = dr[string.Format("{0}09", tempCol)].ToString();
                                //Items.Trend2 = string.IsNullOrWhiteSpace(dr[string.Format("{0}09", tempCol)].ToString()) ? 0.0 : Convert.ToDouble(dr[string.Format("{0}09", tempCol)].ToString());
                                Items.ActualGoal2 = dr[string.Format("{0}10", tempCol)].ToString();
                                Items.BoundaryShow2 = dr[string.Format("{0}11", tempCol)].ToString();
                                Items.Goal3 = dr[string.Format("{0}12", tempCol)].ToString();
                                Items.Trend3 = string.IsNullOrWhiteSpace(dr[string.Format("{0}14", tempCol)].ToString()) ? 0.0 : Convert.ToDouble(dr[string.Format("{0}14", tempCol)].ToString());
                                Items.ActualGoal3 = dr[string.Format("{0}15", tempCol)].ToString();
                                Items.BoundaryShow3 = dr[string.Format("{0}16", tempCol)].ToString();
                                optReturn = XMLHelper.SeriallizeObject(Items);
                                if (!optReturn.Result)
                                {
                                    return optReturn;
                                }
                                listOpts.Add(optReturn.Data.ToString());
                            }
                        }**/
                        #endregion
                        #region 新的处理
                        string[] arrinfo = listParams[5].Split(',');
                        if (arrinfo.Count() != 2)
                        {
                            optReturn.Message = "周期为小时之后的更细致的查询条件丢失";
                            return optReturn;
                        }
                         strSql = string.Format("SELECT * FROM T_46_011_{0} T4611 WHERE T4611.C015{1} AND T4611.C004=({5}) AND T4611.C003{2} AND T4611.C007>={3} AND T4611.C007<={4} " +
                        "AND T4611.C002={5} ORDER BY T4611.C015,T4611.C007,T4611.C005,T4611.C003", rentToken, KPIMappingStr, objectIDStr, startTime, endTime, listParams[0], arrinfo[0]);   
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
                        for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                        {
                            DataRow dr = objDataSet.Tables[0].Rows[i];
                            PMShowDataItems Items = new PMShowDataItems();
                            Items.KPIMappingID = Convert.ToInt64(dr["C001"]);
                            Items.KPIID = Convert.ToInt64(dr["C015"]);
                            Items.UERAId = Convert.ToInt64(dr["C003"]);
                            Items.StartLocalTime = Convert.ToInt64(dr["C007"]);
                            Items.pmYear = Convert.ToInt32(dr["C009"]);
                            Items.pmMonth = Convert.ToInt32(dr["C010"]);
                            Items.pmDay = Convert.ToInt32(dr["C011"]);
                            Items.BelongsId = Convert.ToInt64(dr["C014"]);
                            Items.ActualValue = string.IsNullOrWhiteSpace(dr[string.Format("C{0}01", arrinfo[1])].ToString()) ? 0.0 : Convert.ToDouble(dr[string.Format("C{0}01", arrinfo[1])].ToString());
                            Items.Goal1 = dr[string.Format("C{0}02", arrinfo[1])].ToString();
                            if (!string.IsNullOrWhiteSpace(dr[string.Format("C{0}04", arrinfo[1])].ToString()))
                            {
                                Items.Trend1 = dr[string.Format("C{0}04", arrinfo[1])].ToString();
                                if (Items.Trend1 == "2")
                                {
                                    Items.Trend1 = string.Empty;
                                }
                            } Items.ActualGoal1 = dr[string.Format("C{0}05", arrinfo[1])].ToString();
                            Items.BoundaryShow1 = dr[string.Format("C{0}06", arrinfo[1])].ToString();
                            Items.Goal2 = dr[string.Format("C{0}07", arrinfo[1])].ToString();
                            Items.Compare = dr[string.Format("C{0}09", arrinfo[1])].ToString();
                            Items.ActualGoal2 = dr[string.Format("C{0}10", arrinfo[1])].ToString();
                            Items.BoundaryShow2 = dr[string.Format("C{0}11", arrinfo[1])].ToString();
                            Items.Goal3 = dr[string.Format("C{0}12", arrinfo[1])].ToString();
                            Items.Trend3 = string.IsNullOrWhiteSpace(dr[string.Format("C{0}14", arrinfo[1])].ToString()) ? 0.0 : Convert.ToDouble(dr[string.Format("C{0}14", arrinfo[1])].ToString());
                            Items.ActualGoal3 = dr[string.Format("C{0}15", arrinfo[1])].ToString();
                            Items.BoundaryShow3 = dr[string.Format("C{0}16", arrinfo[1])].ToString();
                            optReturn = XMLHelper.SeriallizeObject(Items);
                            if (!optReturn.Result)
                            {
                                return optReturn;
                            }
                            listOpts.Add(optReturn.Data.ToString());
                        }
                        #endregion
                        break;
                    default:
                        break;
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

        /// <summary>
        /// 得到该租户的月和周的设定、座席分机虚拟分机全局参数的编号
        /// 12010101每周开始于默认值为0,表示从周日晚上24点开始
        /// 0为周日，1星期一，6为星期六
        /// 12010102每月开始于默认值为1
        /// 1为自然月,2为2号,最大28为28号
        /// 12010401 为分机和座席 E为分机 A为座席 E char(27)A为座席+分机 R为真实分机
        /// </summary>
        private OperationReturn GetGlobalSetting(SessionInfo session, List<string> listParams)
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
                string strSql =string.Format("SELECT * FROM T_11_001_{0} WHERE C003 ={1} ", rentToken,listParams[0]);
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        optReturn.StringValue = strSql;
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;

                        break;
                    case 3:
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        optReturn.StringValue = strSql;
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
                    optReturn.StringValue = strSql;
                    return optReturn;
                }
                string listReturn = string.Empty; ;
                for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = objDataSet.Tables[0].Rows[i];
                    if (dr["C006"] != null)
                    {
                        listReturn=DecryptNamesFromDB(dr["C006"].ToString()).Trim(' ').Substring(listParams[0].Length);
                    }
                }
                optReturn.Data = listReturn;
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

        private OperationReturn SaveViewColumnInfos(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0     用户编号
                //1     视图编号
                //2     个数
                //3...  列配置信息
                if (listParams == null || listParams.Count < 3)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strUserID = listParams[0];
                string strViewID = listParams[1];
                string strCount = listParams[2];
                int intCount;
                if (!int.TryParse(strCount, out intCount))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("ViewColumnInfoCount param invalid");
                    return optReturn;
                }
                if (listParams.Count < 3 + intCount)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("ViewColumnInfo count invalid");
                    return optReturn;
                }
                string rentToken = session.RentInfo.Token;
                string strSql;
                IDbConnection objConn;
                IDbDataAdapter objAdapter;
                DbCommandBuilder objCmdBuilder;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT * FROM T_11_203_{0} WHERE C001 = {1} and C002 = {2}"
                           , rentToken
                           , strUserID
                           , strViewID);
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
                        strSql = string.Format("SELECT * FROM T_11_203_{0} WHERE C001 = {1} and C002 = {2}"
                          , rentToken
                          , strUserID
                          , strViewID);
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
                List<string> listReturn = new List<string>();
                try
                {
                    DataSet objDataSet = new DataSet();
                    objAdapter.Fill(objDataSet);

                    for (int i = 0; i < intCount; i++)
                    {
                        string strSettingInfo = listParams[i + 3];
                        optReturn = XMLHelper.DeserializeObject<ViewColumnInfo>(strSettingInfo);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        ViewColumnInfo columnInfo = optReturn.Data as ViewColumnInfo;
                        if (columnInfo == null)
                        {
                            optReturn.Result = false;
                            optReturn.Code = Defines.RET_OBJECT_NULL;
                            optReturn.Message = string.Format("SettingInfo is null");
                            return optReturn;
                        }
                        DataRow[] drs = objDataSet.Tables[0].Select(string.Format("C003 = '{0}'", columnInfo.ColumnName));
                        DataRow dr;
                        bool isAdd = false;
                        if (drs.Length <= 0)
                        {
                            dr = objDataSet.Tables[0].NewRow();
                            dr["C001"] = strUserID;
                            dr["C002"] = strViewID;
                            dr["C003"] = columnInfo.ColumnName;
                            isAdd = true;
                        }
                        else
                        {
                            dr = drs[0];
                        }
                        dr["C004"] = columnInfo.SortID;
                        dr["C005"] = "1";
                        dr["C006"] = "1";
                        dr["C007"] = "1";
                        dr["C008"] = "1";
                        dr["C009"] = "1";
                        dr["C010"] = "1";
                        dr["C011"] = columnInfo.Visibility;
                        dr["C012"] = "0";
                        dr["C013"] = "0";
                        dr["C014"] = "0";
                        dr["C015"] = "0";
                        dr["C016"] = columnInfo.Width;
                        dr["C017"] = 26;
                        if (isAdd)
                        {
                            objDataSet.Tables[0].Rows.Add(dr);
                            listReturn.Add(string.Format("Add{0}{1}", ConstValue.SPLITER_CHAR, columnInfo.ColumnName));
                        }
                        else
                        {
                            listReturn.Add(string.Format("Update{0}{1}", ConstValue.SPLITER_CHAR, columnInfo.ColumnName));
                        }
                    }

                    objAdapter.Update(objDataSet);
                    objDataSet.AcceptChanges();
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
    }
}
