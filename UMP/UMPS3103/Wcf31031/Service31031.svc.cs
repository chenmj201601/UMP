using System;
using System.Linq;
using System.Data;
using System.ServiceModel;
using System.Data.Common;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel.Activation;
using Oracle.DataAccess.Client;
using System.Data.SqlClient;
using PFShareClassesS;
using VoiceCyber.Common;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common31031;
using Wcf31031.Wcf11012;
using VoiceCyber.UMP.ScoreSheets;
using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace Wcf31031
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码、svc 和配置文件中的类名“Service1”。
    // 注意: 为了启动 WCF 测试客户端以测试此服务，请在解决方案资源管理器中选择 Service1.svc 或 Service1.svc.cs，然后开始调试。
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public partial class Service31031 : IService31031
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


        private string EncryptToClient(string strSource)//加密
        {
            string strReturn = EncryptionAndDecryption.EncryptDecryptString(strSource,
             CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004),
             EncryptionAndDecryption.UMPKeyAndIVType.M004);
            return strReturn;
        }

        private string DecryptFromClient(string strSource)//解密
        {
            string strReturn = EncryptionAndDecryption.EncryptDecryptString(strSource,
             CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104),
             EncryptionAndDecryption.UMPKeyAndIVType.M104);
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
                    case (int)S3103Codes.GetUserOperationList:
                        optReturn = GetUserOperationList(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S3103Codes.GetRoleOperationList:
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
                    case (int)S3103Codes.GetCurrentUserTasks:
                        optReturn = GetCurrentUserTasks(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        List<string> listtask = optReturn.Data as List<string>;
                        webReturn.ListData = listtask;
                        break;
                    case (int)S3103Codes.GetCanOperationTasks:
                        optReturn = GetCanOperationTasks(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        List<string> listtasks = optReturn.Data as List<string>;
                        webReturn.ListData = listtasks;
                        break;
                    case (int)S3103Codes.RemoveRecord2Task:
                        optReturn = RemoveRecord2Task(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        break;
                    case (int)S3103Codes.GetTaskRecordByTaskID:
                        optReturn = GetTaskRecordByTaskID(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        List<string> listdetail = optReturn.Data as List<string>;
                        webReturn.ListData = listdetail;
                        break;
                    case (int)S3103Codes.ModifyTaskDealLine:
                        optReturn = ModifyTaskDealLine(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        break;
                    case (int)S3103Codes.DeleteRecordFromTask:
                        optReturn = DeleteRecordFromTask(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        break;                        
                    case (int)S3103Codes.GetControlOrgInfoList:
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
                    case (int)S3103Codes.GetControlAgentInfoList:
                        optReturn = GetControlAgentInfoList(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S3103Codes.GetUserScoreSheetList:
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
                    case (int)S3103Codes.SaveScoreSheetResult:
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
                    case (int)S3103Codes.SaveScoreCommentResultInfos:
                        optReturn = SaveScoreCommentResultInfo(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S3103Codes.DeleteErrorDB:
                        optReturn = DeleteErrorDB(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        break;
                    case (int)S3103Codes.SaveScoreItemResult:
                        optReturn = SaveScoreItemResultInfo(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S3103Codes.GetScoreResultList:
                        optReturn = GetScoreResultList(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S3103Codes.GetScoreCommentResultList:
                        optReturn = GetScoreCommentResultList(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S3103Codes.GetRecordMemoList:
                        optReturn = GetRecordMemoList(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S3103Codes.SaveRecordMemoInfo:
                        optReturn = SaveRecordMemoInfo(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.Data = optReturn.Data.ToString();
                        break;
                    case (int)S3103Codes.GetRecordData:
                        optReturn = GetRecordData(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S3103Codes.GetQA:
                        optReturn = GetQA(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        List<string> listRoleUsers = optReturn.Data as List<string>;
                        webReturn.ListData = listRoleUsers;
                        break;
                    case (int)S3103Codes.SaveTask:
                        optReturn = SaveTask(session, webRequest.Data);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.Data = optReturn.Data.ToString();
                        break;
                    case (int)S3103Codes.SaveTaskQA:
                        optReturn = SaveTaskQA(session, webRequest.ListData, webRequest.Data);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.Data = optReturn.Data.ToString();
                        break;
                    case (int)S3103Codes.SaveTaskRecord:
                        optReturn = SaveTaskRecord(session, webRequest.ListData, webRequest.Data);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.Data = optReturn.Data.ToString();
                        break;
                    case (int)S3103Codes.UpdateTaskID2Record:
                        optReturn = UpdateTaskID2Record(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.Data = optReturn.Data.ToString();
                        break;
                    case (int)S3103Codes.UpdateTask:
                        optReturn = UpdateTask(session, webRequest.Data);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.Data = optReturn.Data.ToString();
                        break;
                    case (int)S3103Codes.InsertTempData:
                        optReturn = InsertTempData(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.Data = optReturn.Data.ToString();
                        break;
                    case (int)S3103Codes.SaveScoreDataResult:
                        optReturn = SaveScoreDataResult(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S3103Codes.GetRecheckRecordData:
                        optReturn = GetRecheckRecordData(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S3103Codes.GetAuInfoList:
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
                    case (int)S3103Codes.GetABCD:
                        optReturn = GetABCD(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S3103Codes.GetScoreTemplateID:
                        optReturn = GetScoreTemplateID(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.Data = optReturn.Data as string;
                        break;
                    case (int)S3103Codes.GetRecordHistoryOpt:
                        optReturn = GetRecordHistoryOpt(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.Data = optReturn.Data as string;
                        break;
                    case (int)S3103Codes.GetCtrolQA:
                        optReturn = GetCtrolQA(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;

                    case (int)S3103Codes.GetKeyWordOfRecord:
                        optReturn = GetKeyWordOfRecord(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S3103Codes.GetKeyWords:
                        optReturn = GetKeyWords(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S3103Codes.GetAllPointTable:
                        optReturn = GetAllPointTable(session);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S3103Codes.GetKeywordResultList:
                        optReturn = GetKeywordResultList(session, webRequest.ListData);
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

        private OperationReturn GetKeywordResultList(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0         RecordSerialNo 录音流水号(C002)
                if (listParams == null || listParams.Count < 1)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strSerialNo = listParams[0];
                string rentToken = session.RentInfo.Token;
                int dbType = session.DBType;
                string strConn = session.DBConnectionString;
                string strSql;
                switch (dbType)
                {
                    case 2:
                        strSql = string.Format("SELECT * FROM T_51_009_{0} WHERE C002 = {1} ORDER BY C002, C005",
                            rentToken, strSerialNo);
                        optReturn = MssqlOperation.GetDataSet(strConn, strSql);
                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM T_51_009_{0} WHERE C002 = {1} ORDER BY C002, C005",
                           rentToken, strSerialNo);
                        optReturn = OracleOperation.GetDataSet(strConn, strSql);
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("DBType invalid.");
                        return optReturn;
                }
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                DataSet objDataSet = optReturn.Data as DataSet;
                if (objDataSet == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("DataSet is null.");
                    return optReturn;
                }
                List<string> listReturn = new List<string>();
                for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = objDataSet.Tables[0].Rows[i];
                    KeywordResultInfo info = new KeywordResultInfo();
                    info.RecordNumber = Convert.ToInt64(dr["C001"]);
                    info.RecordSerialID = Convert.ToInt64(dr["C002"]);
                    info.RecordReference = dr["C003"].ToString();
                    info.Offset = Convert.ToInt32(dr["C005"]);
                    info.KeywordName = dr["C007"].ToString();
                    info.KeywordContent = dr["C008"].ToString();
                    info.KeywordNo = Convert.ToInt64(dr["C009"]);
                    info.ContentNo = Convert.ToInt64(dr["C010"]);
                    info.Agent = dr["C014"].ToString();
                    optReturn = XMLHelper.SeriallizeObject(info);
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
                optReturn.Exception = ex;
            }
            return optReturn;
        }

        private OperationReturn GetKeyWords(SessionInfo session, List<string> list)
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
                        strSql = string.Format("SELECT A.C001 AS AC001, A.C002 AS AC002, A.C003 AS AC003, B.C001 AS BC001, B.C002 AS BC002 FROM T_51_006_00000 A, T_51_007_00000 B, T_51_008_00000 C WHERE A.C001 = C.C002 AND B.C001 = C.C001 AND A.C005  = '1' AND B.C003 = '1' AND C.C003 = '1'");
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        strSql = string.Format("SELECT A.C001 AS AC001, A.C002 AS AC002, A.C003 AS AC003, B.C001 AS BC001, B.C002 AS BC002 FROM T_51_006_00000 A, T_51_007_00000 B, T_51_008_00000 C WHERE A.C001 = C.C002 AND B.C001 = C.C001 AND A.C005  = '1' AND B.C003 = '1' AND C.C003 = '1'");
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
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
                List<string> listReturn = new List<string>();
                for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = objDataSet.Tables[0].Rows[i];

                    KeywordInfo info = new KeywordInfo();
                    info.SerialNo = Convert.ToInt64(dr["AC001"]);
                    info.ContentNo = Convert.ToInt64(dr["BC001"]);
                    string strName = dr["AC002"].ToString();
                    strName = DecryptString02(strName);
                    info.Name = strName;
                    info.State = 0;     //0：正常；1：删除；2：禁用
                    info.Icon = dr["AC003"].ToString();
                    string strContent = dr["BC002"].ToString();
                    strContent = DecryptString02(strContent);
                    info.Content = strContent;
                    optReturn = XMLHelper.SeriallizeObject(info);
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

        private OperationReturn GetAllPointTable(SessionInfo session) 
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try 
            {
                string rentToken = session.RentInfo.Token;
                string strSQL;
                DataSet ds = new DataSet();
                switch (session.DBType) 
                {
                    case 2:
                        strSQL = string.Format("SELECT *  FROM T_31_001_{0} ORDER BY C001", rentToken);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSQL);
                        if (!optReturn.Result)
                       {
                           return optReturn;
                       }
                        ds = optReturn.Data as DataSet;
                        break;
                    case 3:
                        strSQL = string.Format("SELECT *  FROM T_31_001_{0} ORDER BY C001", rentToken);
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSQL);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        ds = optReturn.Data as DataSet;
                        break;
                }
                if (ds == null) 
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("DataSet is null.");
                    return optReturn;
                }
                List<string> listReturn = new List<string>();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++) 
                {
                    DataRow dr = ds.Tables[0].Rows[i];
                    string item = string.Format("{0};{1}",dr["C001"].ToString(),dr["C002"].ToString());              
                    listReturn.Add(item);
                }
                optReturn.Data = listReturn;
            }
            catch (Exception ex) 
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = string.Format(ex.Message);
                return optReturn;
            }
            return optReturn;
        }

        private OperationReturn UpdateTaskID2Record(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;

            if (listParams == null || listParams.Count < 4)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_PARAM_INVALID;
                optReturn.Message = string.Format("Request param is null or count invalid");
                return optReturn;
            }

            string rentToken = session.RentInfo.Token;
            string taskId = listParams[0];
            string tableName = listParams[1];
            //1 初检任务 2复检 
            string taskType = listParams[2];
            string recordReference = listParams[3];
            string strSql;
            IDbConnection objConn;
            IDbDataAdapter objAdapter;
            DbCommandBuilder objCmdBuilder;
            switch (session.DBType)
            {
                case 2:
                    strSql = string.Format("SELECT * FROM {0} WHERE C002 = {1} "
                        , tableName, recordReference);
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
                    strSql = string.Format("SELECT * FROM {0} WHERE C002 = {1} "
                        , tableName, recordReference);
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
                if (objDataSet.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = objDataSet.Tables[0].Rows[0];
                    dr.BeginEdit();
                    switch (taskType)
                    {
                        case "1":
                            dr["C104"] = taskId;
                            break;
                        case "2":
                            dr["C105"] = taskId;
                            break;
                        default:
                            break;
                    }
                    dr.EndEdit();
                }

                objAdapter.Update(objDataSet);
                objDataSet.AcceptChanges();
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = string.Format(ex.Message);
                return optReturn;
            }
            return optReturn;
        }

        private OperationReturn SaveTaskRecord(SessionInfo session, List<string> listParams, string taskId)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                string rentToken = session.RentInfo.Token;
                switch (session.DBType)
                {
                    //MSSQL
                    case 2:
                        using (SqlConnection connection = new SqlConnection(session.DBConnectionString))
                        {
                            DataSet dataSet = new DataSet();
                            connection.Open();
                            SqlDataAdapter sqlDA = new SqlDataAdapter(string.Format("SELECT  *  FROM  T_31_022_{0} WHERE  C001={1}", rentToken, taskId), connection);
                            sqlDA.Fill(dataSet);
                            //设置主键
                            dataSet.Tables[0].PrimaryKey = new DataColumn[] { dataSet.Tables[0].Columns[0], dataSet.Tables[0].Columns[1] };
                            SqlCommandBuilder sqlCB = new SqlCommandBuilder(sqlDA);
                            sqlDA.InsertCommand = sqlCB.GetInsertCommand();
                            foreach (string strData in listParams)
                            {
                                optReturn = XMLHelper.DeserializeObject<TaskInfoDetail>(strData);
                                if (!optReturn.Result)
                                {
                                    return optReturn;
                                }
                                TaskInfoDetail taskinfodetail = optReturn.Data as TaskInfoDetail;
                                if (taskinfodetail == null)
                                {
                                    optReturn.Result = false;
                                    optReturn.Code = Defines.RET_OBJECT_NULL;
                                    optReturn.Message = string.Format("taskinfodetail  Is Null");
                                    return optReturn;
                                }

                                //dataSet.Tables[0].Rows.Clear();
                                //for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                                //{
                                //    dataSet.Tables[0].Rows[0].Delete();
                                //} 
                                DataRow drNewRow = dataSet.Tables[0].NewRow();
                                drNewRow["C001"] = taskinfodetail.TaskID;
                                drNewRow["C002"] = taskinfodetail.RecoredReference;
                                drNewRow["C003"] = taskinfodetail.IsLock;
                                drNewRow["C006"] = taskinfodetail.AllotType;
                                drNewRow["C008"] = taskinfodetail.AgtOrExtID;
                                drNewRow["C009"] = taskinfodetail.AgtOrExtName;
                                drNewRow["C010"] = taskinfodetail.TaskType;
                                drNewRow["C011"] = taskinfodetail.StartRecordTime;
                                drNewRow["C012"] = taskinfodetail.Duration;
                                drNewRow["C014"] = DecryptFromClient(taskinfodetail.CallerID);
                                drNewRow["C015"] = DecryptFromClient(taskinfodetail.CalledID);
                                drNewRow["C016"] = taskinfodetail.Direction;
                                dataSet.Tables[0].Rows.Add(drNewRow);
                            }
                            sqlDA.Update(dataSet);
                            dataSet.AcceptChanges();
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
                            OracleDataAdapter oracleDA = new OracleDataAdapter(string.Format("SELECT  *  FROM  T_31_022_{0} WHERE  C001={1} ", rentToken, taskId), connection);
                            oracleDA.Fill(dataSet);
                            //设置主键
                            dataSet.Tables[0].PrimaryKey = new DataColumn[] { dataSet.Tables[0].Columns[0], dataSet.Tables[0].Columns[1] };
                            OracleCommandBuilder oracleCB = new OracleCommandBuilder(oracleDA);
                            oracleDA.InsertCommand = oracleCB.GetInsertCommand();
                            foreach (string strData in listParams)
                            {
                                optReturn = XMLHelper.DeserializeObject<TaskInfoDetail>(strData);
                                if (!optReturn.Result)
                                {
                                    return optReturn;
                                }
                                TaskInfoDetail taskinfodetail = optReturn.Data as TaskInfoDetail;
                                if (taskinfodetail == null)
                                {
                                    optReturn.Result = false;
                                    optReturn.Code = Defines.RET_OBJECT_NULL;
                                    optReturn.Message = string.Format("taskinfodetail  Is Null");
                                    return optReturn;
                                }
                                //// dataSet.Tables[0].Rows.Clear();

                                // for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                                // {

                                //     dataSet.Tables[0].Rows[0].Delete();

                                // }  
                                DataRow drNewRow = dataSet.Tables[0].NewRow();
                                drNewRow["C001"] = taskinfodetail.TaskID;
                                drNewRow["C002"] = taskinfodetail.RecoredReference;
                                drNewRow["C003"] = taskinfodetail.IsLock;
                                drNewRow["C006"] = taskinfodetail.AllotType;
                                drNewRow["C008"] = taskinfodetail.AgtOrExtID;
                                drNewRow["C009"] = taskinfodetail.AgtOrExtName;
                                drNewRow["C010"] = taskinfodetail.TaskType;
                                drNewRow["C011"] = taskinfodetail.StartRecordTime;
                                drNewRow["C012"] = taskinfodetail.Duration;
                                drNewRow["C014"] = DecryptFromClient(taskinfodetail.CallerID);
                                drNewRow["C015"] = DecryptFromClient(taskinfodetail.CalledID);
                                drNewRow["C016"] = taskinfodetail.Direction;
                                dataSet.Tables[0].Rows.Add(drNewRow);
                            }
                            oracleDA.Update(dataSet);
                            dataSet.AcceptChanges();
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
                optReturn.Message = string.Format(ex.Message);
                return optReturn;
            }
            return optReturn;
        }

        private bool IsExitRecordTable(SessionInfo session,string tablename)
        {
            bool ret = false;
            string rentToken = session.RentInfo.Token;
            try
            {
                switch (session.DBType)
                {
                    //MSSQL
                    case 2:
                        using (SqlConnection connection = new SqlConnection(session.DBConnectionString))
                        {
                            DataSet dataSet = new DataSet();
                            connection.Open();
                            SqlDataAdapter sqlDA = new SqlDataAdapter(string.Format("SELECT Object_id('{0}')", tablename), connection);
                            sqlDA.Fill(dataSet);
                            if (dataSet != null && dataSet.Tables[0] != null && dataSet.Tables[0].Rows.Count > 0 && 
                                !string.IsNullOrEmpty(dataSet.Tables[0].Rows[0][0].ToString()))
                                ret = true;
                        }
                        break;
                    //ORCL
                    case 3:
                        using (OracleConnection connection = new OracleConnection(session.DBConnectionString))
                        {
                            DataSet dataSet = new DataSet();
                            connection.Open();
                            OracleDataAdapter oracleDA = new OracleDataAdapter(string.Format("SELECT OBJECT_NAME FROM USER_OBJECTS WHERE OBJECT_NAME = UPPER ('{0}')", tablename), connection);
                            oracleDA.Fill(dataSet);
                            if (dataSet != null && dataSet.Tables[0] != null && dataSet.Tables[0].Rows.Count > 0)
                                ret = true;
                        }
                        break;
                    default:
                        break;
                }
            }
            catch { }
            return ret;
        }

        private OperationReturn SaveTaskQA(SessionInfo session, List<string> listParams, string taskid)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                string rentToken = session.RentInfo.Token;
                switch (session.DBType)
                {
                    //MSSQL
                    case 2:
                        using (SqlConnection connection = new SqlConnection(session.DBConnectionString))
                        {
                            DataSet dataSet = new DataSet();
                            connection.Open();
                            SqlDataAdapter sqlDA = new SqlDataAdapter(string.Format("SELECT  *  FROM  T_31_021_{0} WHERE  C001={1}", rentToken, taskid), connection);
                            sqlDA.Fill(dataSet);
                            //设置主键
                            dataSet.Tables[0].PrimaryKey = new DataColumn[] { dataSet.Tables[0].Columns[0], dataSet.Tables[0].Columns[1] };
                            SqlCommandBuilder sqlCB = new SqlCommandBuilder(sqlDA);
                            sqlDA.InsertCommand = sqlCB.GetInsertCommand();
                            foreach (string strData in listParams)
                            {
                                optReturn = XMLHelper.DeserializeObject<CtrolQA>(strData);
                                if (!optReturn.Result)
                                {
                                    return optReturn;
                                }
                                CtrolQA ctrolqa = optReturn.Data as CtrolQA;
                                if (ctrolqa == null)
                                {
                                    optReturn.Result = false;
                                    optReturn.Code = Defines.RET_OBJECT_NULL;
                                    optReturn.Message = string.Format("ctrol qa  Is Null");
                                    return optReturn;
                                }
                                dataSet.Tables[0].Rows.Clear();
                                DataRow drCurrent = dataSet.Tables[0].NewRow();
                                drCurrent["C001"] = taskid;
                                drCurrent["C002"] = ctrolqa.UserID;
                                drCurrent["C003"] = ctrolqa.UserName;
                                drCurrent["C004"] = "Q";
                                dataSet.Tables[0].Rows.Add(drCurrent);
                            }
                            sqlDA.Update(dataSet);
                            dataSet.AcceptChanges();
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
                            OracleDataAdapter oracleDA = new OracleDataAdapter(string.Format("SELECT  *  FROM  T_31_021_{0} WHERE  C001={1}", rentToken, taskid), connection);
                            oracleDA.Fill(dataSet);
                            //设置主键
                            dataSet.Tables[0].PrimaryKey = new DataColumn[] { dataSet.Tables[0].Columns[0], dataSet.Tables[0].Columns[1] };
                            OracleCommandBuilder oracleCB = new OracleCommandBuilder(oracleDA);
                            oracleDA.InsertCommand = oracleCB.GetInsertCommand();
                            foreach (string strData in listParams)
                            {
                                optReturn = XMLHelper.DeserializeObject<CtrolQA>(strData);
                                if (!optReturn.Result)
                                {
                                    return optReturn;
                                }
                                CtrolQA ctrolqa = optReturn.Data as CtrolQA;
                                if (ctrolqa == null)
                                {
                                    optReturn.Result = false;
                                    optReturn.Code = Defines.RET_OBJECT_NULL;
                                    optReturn.Message = string.Format("ctrol qa  Is Null");
                                    return optReturn;
                                }
                                dataSet.Tables[0].Rows.Clear();
                                DataRow drCurrent = dataSet.Tables[0].NewRow();
                                drCurrent["C001"] = taskid;
                                drCurrent["C002"] = ctrolqa.UserID;
                                drCurrent["C003"] = ctrolqa.UserName;
                                drCurrent["C004"] = "Q";
                                dataSet.Tables[0].Rows.Add(drCurrent);
                                oracleDA.Update(dataSet);
                                dataSet.AcceptChanges();
                                oracleDA.Dispose();
                                connection.Close();
                            }
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
                optReturn.Message = string.Format(ex.Message);
                return optReturn;
            }
            return optReturn;
        }

        private OperationReturn UpdateTask(SessionInfo session, string strData)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;

            optReturn = XMLHelper.DeserializeObject<UserTasksInfoShow>(strData);
            if (!optReturn.Result)
            {
                return optReturn;
            }
            UserTasksInfoShow taskinfo = optReturn.Data as UserTasksInfoShow;
            if (taskinfo == null)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_OBJECT_NULL;
                optReturn.Message = string.Format("NewRoleInfo Is Null");
                return optReturn;
            }
            string rentToken = session.RentInfo.Token;
            long taskId = taskinfo.TaskID;
            string strSql;
            IDbConnection objConn;
            IDbDataAdapter objAdapter;
            DbCommandBuilder objCmdBuilder;
            switch (session.DBType)
            {
                case 2:
                    strSql = string.Format("SELECT * FROM T_31_020_{0} WHERE C001 = {1} "
                        , rentToken, taskId);
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
                    strSql = string.Format("SELECT * FROM T_31_020_{0} WHERE C001 = {1}  "
                        , rentToken, taskId);
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
                if (objDataSet.Tables[0].Rows.Count > 0)
                {
                    DataRow dr = objDataSet.Tables[0].Rows[0];
                    dr.BeginEdit();
                    dr["C008"] = taskinfo.AssignNum;
                    dr["C021"] = taskinfo.TaskAllRecordLength;
                    dr.EndEdit();
                }

                objAdapter.Update(objDataSet);
                objDataSet.AcceptChanges();
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = string.Format(ex.Message);
                return optReturn;
            }
            return optReturn;
        }

        private OperationReturn SaveTask(SessionInfo session, string strData)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            int errNum = 0;
            long serialID = 0;
            string errMsg = string.Empty;
            try
            {

                optReturn = XMLHelper.DeserializeObject<UserTasksInfoShow>(strData);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                UserTasksInfoShow newTask = optReturn.Data as UserTasksInfoShow;
                if (newTask == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("NewTask Is Null");
                    return optReturn;
                }
                string rentToken = session.RentInfo.Token;
                switch (session.DBType)
                {
                    //MSSQL
                    case 2:
                        DbParameter[] mssqlParameters =
                        {
                            MssqlOperation.GetDbParameter("@ainparam01",MssqlDataType.Varchar,5 ),
                            MssqlOperation.GetDbParameter("@ainparam02",MssqlDataType.NVarchar,64),
                            MssqlOperation.GetDbParameter("@ainparam03",MssqlDataType.NVarchar,1024),
                            MssqlOperation.GetDbParameter("@ainparam04",MssqlDataType.Varchar,5),
                            MssqlOperation.GetDbParameter("@ainparam05",MssqlDataType.Varchar,1),
                            MssqlOperation.GetDbParameter("@ainparam06",MssqlDataType.Varchar,20),
                            MssqlOperation.GetDbParameter("@ainparam07",MssqlDataType.Varchar,20),
                            MssqlOperation.GetDbParameter("@ainparam08",MssqlDataType.Varchar,10),
                            MssqlOperation.GetDbParameter("@ainparam09",MssqlDataType.Varchar,20),
                            MssqlOperation.GetDbParameter("@ainparam10",MssqlDataType.Varchar,10),
                            MssqlOperation.GetDbParameter("@ainparam11",MssqlDataType.Varchar,10),
                            //MssqlOperation.GetDbParameter("@ainparam12",MssqlDataType.NVarchar,1024),
                            MssqlOperation.GetDbParameter("@ainparam13",MssqlDataType.Varchar,10),
                            MssqlOperation.GetDbParameter("@ainparam14",MssqlDataType.Varchar,10),
                            MssqlOperation.GetDbParameter("@ainparam15",MssqlDataType.Varchar,20),
                            MssqlOperation.GetDbParameter("@ainparam16",MssqlDataType.NVarchar,1024),
                            MssqlOperation.GetDbParameter("@ainparam17",MssqlDataType.Varchar,20),
                            MssqlOperation.GetDbParameter("@ainparam18",MssqlDataType.Varchar,5),                            
                            MssqlOperation.GetDbParameter("@aoutparam01",MssqlDataType.Varchar,20),
                            MssqlOperation.GetDbParameter("@aouterrornumber",MssqlDataType.Bigint,0),
                            MssqlOperation.GetDbParameter("@aouterrorstring",MssqlDataType.NVarchar,4000),
                            MssqlOperation.GetDbParameter("@ainparam19",MssqlDataType.NVarchar,20)

                        };
                        mssqlParameters[0].Value = session.RentInfo.Token;
                        mssqlParameters[1].Value = newTask.TaskName;//*
                        mssqlParameters[2].Value = newTask.TaskDesc == null ? "" : newTask.TaskDesc;
                        mssqlParameters[3].Value = newTask.TaskType;//*
                        mssqlParameters[4].Value = newTask.IsShare;//*
                        mssqlParameters[5].Value = newTask.AssignTime;//*
                        mssqlParameters[6].Value = newTask.AssignUser;

                        mssqlParameters[7].Value = newTask.AssignNum;//*
                        mssqlParameters[8].Value = newTask.DealLine;
                        mssqlParameters[9].Value = newTask.AlreadyScoreNum;//*
                        mssqlParameters[10].Value = newTask.RemindDayTime;
                        // mssqlParameters[11].Value = newTask.ReminderIDs;
                        mssqlParameters[11].Value = newTask.BelongYear;//*
                        mssqlParameters[12].Value = newTask.BelongMonth;//*
                        mssqlParameters[13].Value = newTask.FinishTime;
                        mssqlParameters[14].Value = newTask.AssignUserFName;
                        mssqlParameters[15].Value = newTask.TaskAllRecordLength;//*
                        mssqlParameters[16].Value = newTask.TaskType == 1
                            ? S3103Consts.OBJTYPE_FIRSTTASK
                            : S3103Consts.OBJTYPE_RECHECKTASK;// 1 初檢；3 複檢

                        mssqlParameters[17].Value = serialID;
                        mssqlParameters[18].Value = errNum;
                        mssqlParameters[19].Value = errMsg;
                   //     mssqlParameters[20].Value = string.Format("{0}{1}{2}", newTask.strScoreSheetItemName, ConstValue.SPLITER_CHAR_2, newTask.strScoreSheetItemID);
                  //      mssqlParameters[20].Value = newTask.strScoreSheetItemName;
                        mssqlParameters[20].Value = newTask.strScoreSheetItemID;
                        mssqlParameters[17].Direction = ParameterDirection.Output;
                        mssqlParameters[18].Direction = ParameterDirection.Output;
                        mssqlParameters[19].Direction = ParameterDirection.Output;
                    
                        optReturn = MssqlOperation.ExecuteStoredProcedure(session.DBConnectionString, "P_31_003",
                           mssqlParameters);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        if (mssqlParameters[18].Value.ToString() != "0")
                        {
                            optReturn.Result = false;
                            optReturn.Code = Defines.RET_DBACCESS_FAIL;
                            optReturn.Message = string.Format("{0}\t{1}", mssqlParameters[19].Value, mssqlParameters[14].Value);
                        }
                        else
                        {
                            optReturn.Data = mssqlParameters[17].Value.ToString();
                        }
                        break;
                    //ORCL
                    case 3:
                        DbParameter[] dbParameters =
                        {
                            OracleOperation.GetDbParameter("ainparam01",OracleDataType.Varchar2,5 ),
                            OracleOperation.GetDbParameter("ainparam02",OracleDataType.Nvarchar2,64),
                            OracleOperation.GetDbParameter("ainparam03",OracleDataType.Nvarchar2,1024),
                            OracleOperation.GetDbParameter("ainparam04",OracleDataType.Varchar2,5),
                            OracleOperation.GetDbParameter("ainparam05",OracleDataType.Varchar2,1),
                            OracleOperation.GetDbParameter("ainparam06",OracleDataType.Varchar2,20),
                            OracleOperation.GetDbParameter("ainparam07",OracleDataType.Varchar2,20),
                            OracleOperation.GetDbParameter("ainparam08",OracleDataType.Varchar2,10),
                            OracleOperation.GetDbParameter("ainparam09",OracleDataType.Varchar2,20),
                            OracleOperation.GetDbParameter("ainparam10",OracleDataType.Varchar2,10),
                            OracleOperation.GetDbParameter("ainparam11",OracleDataType.Varchar2,10),
                            // OracleOperation.GetDbParameter("ainparam12",OracleDataType.Nvarchar2,1024),
                            OracleOperation.GetDbParameter("ainparam13",OracleDataType.Varchar2,10),
                            OracleOperation.GetDbParameter("ainparam14",OracleDataType.Varchar2,10),
                             OracleOperation.GetDbParameter("ainparam15",OracleDataType.Varchar2,20),
                            OracleOperation.GetDbParameter("ainparam16",OracleDataType.Nvarchar2,1024),
                            OracleOperation.GetDbParameter("ainparam17",OracleDataType.Varchar2,20),
                            OracleOperation.GetDbParameter("ainparam18",OracleDataType.Varchar2,5),
                            OracleOperation.GetDbParameter("aoutparam01",OracleDataType.Varchar2,19),
                            OracleOperation.GetDbParameter("errornumber",OracleDataType.Int32,0),
                            OracleOperation.GetDbParameter("errorstring",OracleDataType.Nvarchar2,200)
                        };
                        dbParameters[0].Value = session.RentInfo.Token;
                        dbParameters[1].Value = newTask.TaskName;
                        dbParameters[2].Value = newTask.TaskDesc;
                        dbParameters[3].Value = newTask.TaskType;
                        dbParameters[4].Value = newTask.IsShare;
                        dbParameters[5].Value = newTask.AssignTime;
                        dbParameters[6].Value = newTask.AssignUser;

                        dbParameters[7].Value = newTask.AssignNum;
                        dbParameters[8].Value = newTask.DealLine;
                        dbParameters[9].Value = newTask.AlreadyScoreNum;
                        dbParameters[10].Value = newTask.RemindDayTime;
                        //dbParameters[11].Value = newTask.ReminderIDs;
                        dbParameters[11].Value = newTask.BelongYear;
                        dbParameters[12].Value = newTask.BelongMonth;
                        dbParameters[13].Value = newTask.FinishTime;
                        dbParameters[14].Value = newTask.AssignUserFName;
                        dbParameters[15].Value = newTask.TaskAllRecordLength;
                        dbParameters[16].Value = newTask.TaskType == 1
                            ? S3103Consts.OBJTYPE_FIRSTTASK
                            : S3103Consts.OBJTYPE_RECHECKTASK;// 1 初檢；3 複檢


                        dbParameters[17].Value = serialID;
                        dbParameters[18].Value = errNum;
                        dbParameters[19].Value = errMsg;                      
                        dbParameters[17].Direction = ParameterDirection.Output;
                        dbParameters[18].Direction = ParameterDirection.Output;
                        dbParameters[19].Direction = ParameterDirection.Output;
                        optReturn = OracleOperation.ExecuteStoredProcedure(session.DBConnectionString, " P_31_003",
                            dbParameters);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        if (dbParameters[18].Value.ToString() != "0")
                        {
                            optReturn.Result = false;
                            optReturn.Code = Defines.RET_DBACCESS_FAIL;
                            optReturn.Message = dbParameters[19].Value.ToString();
                        }
                        else
                        {
                            optReturn.Data = dbParameters[17].Value.ToString();
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
                optReturn.Message = string.Format(ex.Message + "; " + ex.StackTrace + "; " + ex.Source);
                return optReturn;
            }
            return optReturn;
        }

        private OperationReturn GetQA(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParams
                //0     机构编号
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
                        strSql = string.Format("SELECT * FROM T_11_005_{0} WHERE  C006={1}  AND C001 IN  (SELECT  C004  FROM T_11_201_{0} WHERE C003 IN(SELECT C001 FROM T_11_202_{0} WHERE C001 LIKE '106%' AND C002={2}) AND C004 LIKE '102%') AND C007 <> 'H' ", rentToken, OrgID, S3103Consts.OPT_TASKRECORDSCORE);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        strSql =string.Format("SELECT * FROM T_11_005_{0} WHERE  C006={1}  AND C001 IN  (SELECT  C004  FROM T_11_201_{0} WHERE C003 IN(SELECT C001 FROM T_11_202_{0} WHERE C001 LIKE '106%' AND C002={2}) AND C004 LIKE '102%') AND C007 <> 'H' ", rentToken, OrgID, S3103Consts.OPT_TASKRECORDSCORE);
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
                    CtrolQA ctrolqa = new CtrolQA();
                    ctrolqa.UserID = dr["C001"].ToString();
                    ctrolqa.UserName = DecryptNamesFromDB(dr["C002"].ToString());
                    ctrolqa.UserFullName = DecryptNamesFromDB(dr["C003"].ToString());
                    ctrolqa.OrgID = OrgID;
                    optReturn = XMLHelper.SeriallizeObject(ctrolqa);
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
        
        #region  权限 && 读狗
        /// <summary>
        /// 老的读取权限的方式 2016年3月2日 13:50:16 备份
        /// </summary>
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

        /// <summary>
        /// 新的读取权限的方式，会读狗 2016年3月2日 13:50:16 
        /// </summary>
        private OperationReturn GetUserOperationList(SessionInfo session, List<string> listParams)
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
                if (listParams == null || listParams.Count < 3)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string roleID = session.RoleInfo.ID.ToString();
                string moduleID = listParams[1];
                string parentID = listParams[2];
                string rentToken = session.RentInfo.Token;
                string strLog = string.Empty;


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


                string strSql;
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        strSql =
                            string.Format(
                                "SELECT * FROM T_11_003_{0} WHERE C001 = {1} AND C002 IN (SELECT C002 FROM T_11_202_{0} WHERE C001 = {2} AND C003='1') AND C002 LIKE '{3}%' ORDER BY C001, C002",
                                rentToken,moduleID,roleID,parentID);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            strLog += strSql;
                            optReturn.Message += strLog;
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        strSql =
                             string.Format(
                                 "SELECT * FROM T_11_003_{0} WHERE C001 = {1} AND C002 IN (SELECT C002 FROM T_11_202_{0} WHERE C001 = {2} AND C003='1') AND C002 LIKE '{3}%' ORDER BY C001, C002",
                                 rentToken,moduleID,roleID,parentID);
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            strLog += strSql;
                            optReturn.Message += strLog;
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
                strLog += strSql;
                List<string> listOpts = new List<string>();
                for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = objDataSet.Tables[0].Rows[i];


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
                    strC006 = DecryptNamesFromDB(strC006);
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
                        strC008 = DecryptNamesFromDB(strC008);
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


                    OperationInfo opt = new OperationInfo();
                    opt.ID = Convert.ToInt64(dr["C002"]);
                    opt.ParentID = Convert.ToInt64(dr["C003"]);
                    opt.Display = string.Format("Opt({0})", opt.ID);
                    opt.Description = opt.Display;
                    opt.Icon = dr["C013"].ToString();
                    opt.SortID = Convert.ToInt32(dr["C004"]);
                    string strType = dr["C011"].ToString();
                    int intType = 0;
                    switch (strType)
                    {
                        case "M":
                            intType = 0;
                            break;
                        case "B":
                            intType = 1;
                            break;
                        case "T":
                            intType = 2;
                            break;
                        case "H":
                            intType = 3;
                            break;
                    }
                    opt.Type = intType;
                    optReturn = XMLHelper.SeriallizeObject(opt);
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                    listOpts.Add(optReturn.Data.ToString());
                }
                optReturn.Data = listOpts;
                optReturn.Message = strLog;
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

        #endregion

        private OperationReturn GetCurrentUserTasks(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            string strSql = "";
            try
            {
                if (listParams.Count < 4)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Param count invalid");
                    return optReturn;
                }
                string userid = listParams[0];
                string tstype = listParams[1];//0:我分配的 1:分配给我的 2:全部
                string fntype = listParams[2];//0:未完成 1:已完成 2:全部
                string taskType = listParams[3];//初檢 1；複檢 3
                string stdate = string.Empty;//开始时间
                string eddate = string.Empty;//截止时间
                if (listParams.Count > 4)
                {
                    stdate = listParams[4];
                    eddate = listParams[5];
                }
                string sql2 = "";
                string sql3 = "";
                string isfinished = "WHERE 1=1 ";
                sql2 = tstype == "0" ? " AND 1=2 " : "";//查我分配的，分配给我的不查
                sql3 = tstype == "1" ? " AND 1=2 " : "";//查分配给我的，则我分配的不查
                switch (fntype)
                { 
                    case "0":
                        isfinished += string.Format(" AND T.C017='N' AND T.C004 IN ({0})",taskType);
                        break;
                    case "1":
                        isfinished += string.Format(" AND T.C017='Y' AND T.C004 IN ({0})",taskType);
                        break;
                    case "2":
                        isfinished += string.Format(" AND T.C004 IN ({0})", taskType);
                        break;
                    default:
                        break;
                }
                IDbConnection objConn = null;
                IDbDataAdapter objAdapter = null;
                DataSet objDataSet = new DataSet();
                string rentToken = session.RentInfo.Token;
                switch (session.DBType)
                {
                    //MSSQL
                    case 2:
                        if (!string.IsNullOrEmpty(stdate) && !string.IsNullOrEmpty(eddate))
                            isfinished += string.Format(" AND T.C009 BETWEEN '{0}' AND '{1}'", stdate, eddate);
                        strSql = string.Format("SELECT * FROM ((SELECT T_31_020_{0}.* ,T_31_021_{0}.C003 FinishUserFName,T_31_021_{0}.C002 FinishUserID FROM T_31_020_{0}" +
                                               " LEFT JOIN T_31_021_{0} ON T_31_020_{0}.C001=T_31_021_{0}.C001 WHERE T_31_021_{0}.C002={1}{2}) union (SELECT T_31_020_{0}.* ," +
                                               "T_31_021_{0}.C003 FinishUserFName,T_31_021_{0}.C002 FinishUserID FROM T_31_020_{0} LEFT JOIN T_31_021_{0} ON T_31_020_{0}.C001=T_31_021_{0}.C001"+
                                               " WHERE T_31_020_{0}.C007={1}{3}) )t {4} ORDER BY C001, FinishUserID DESC ", 
                            rentToken, 
                            listParams[0],
                            sql2,
                            sql3,
                            isfinished);
                        objConn = MssqlOperation.GetConnection(session.DBConnectionString);
                        objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                        break;
                    //ORCL
                    case 3:
                        if (!string.IsNullOrEmpty(stdate) && !string.IsNullOrEmpty(eddate))
                            isfinished += string.Format(" AND T.C009>=to_date('{0}', 'yyyy-mm-dd hh24:mi:ss') and T.C009<=to_date('{1}', 'yyyy-mm-dd hh24:mi:ss')", stdate, eddate);
                        strSql = string.Format("SELECT * FROM ((SELECT T_31_020_{0}.* ,T_31_021_{0}.C003 FinishUserFName,T_31_021_{0}.C002 FinishUserID FROM T_31_020_{0} "+
                                                 "LEFT JOIN T_31_021_{0} ON T_31_020_{0}.C001=T_31_021_{0}.C001 WHERE T_31_021_{0}.C002={1}{2}) union (SELECT T_31_020_{0}.* ,"+
                                                 "T_31_021_{0}.C003 FinishUserFName,T_31_021_{0}.C002 FinishUserID FROM T_31_020_{0} LEFT JOIN T_31_021_{0} ON T_31_020_{0}.C001=T_31_021_{0}.C001" +
                                                 " WHERE T_31_020_{0}.C007={1}{3}) )t {4} ORDER BY C001, FinishUserID DESC", 
                            rentToken, 
                            listParams[0],
                            sql2,
                            sql3,
                            isfinished);
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
                #region
                try
                {
                    objAdapter.Fill(objDataSet);

                    List<string> listRoleUsers = new List<string>();
                    for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                    {
                        DataRow dr = objDataSet.Tables[0].Rows[i];
                        UserTasksInfoShow tasks = new UserTasksInfoShow();
                        tasks.TaskID = Convert.ToInt64(dr["C001"]);
                        tasks.TaskName = dr["C002"] != null ? dr["C002"].ToString() : "";
                        tasks.TaskDesc = dr["C003"] != null ? dr["C003"].ToString() : "";
                        tasks.TaskType = Convert.ToInt32(dr["C004"]);
                        tasks.IsShare = dr["C005"] != null ? dr["C005"].ToString() : "";
                        tasks.AssignTime = StrDatePress(dr["C006"].ToString());

                        tasks.AssignUser = Convert.ToInt64(dr["C007"]);
                        tasks.AssignNum = Convert.ToInt32(dr["C008"]);
                        tasks.DealLine = StrDatePress(dr["C009"]);
                        tasks.AlreadyScoreNum = Convert.ToInt32(dr["C010"]);
                        tasks.ModifyTime = StrDatePress(dr["C011"]);
                        tasks.ModifyUser = Int64Parse(dr["C012"].ToString(), -1);
                        //Convert.ToInt64(dr["C012"]);

                        tasks.RemindDayTime = Convert.ToInt32(dr["C013"]);
                        //tasks.ReminderIDs = dr["C014"] != null ? dr["C014"].ToString() : "";
                        tasks.BelongYear = Convert.ToInt32(dr["C015"]);
                        tasks.BelongMonth = Convert.ToInt32(dr["C016"]);
                        tasks.IsFinish = dr["C017"] != null ? dr["C017"].ToString() : "";
                        tasks.FinishTime = StrDatePress(dr["C018"]);
                        if (tasks.TaskType == 2 || tasks.TaskType==4)
                        {
                            tasks.AssignUserFName = "Administrator";
                        }
                        else
                        {
                            tasks.AssignUserFName = DecryptFNames(dr["C019"]);
                        }                        
                        tasks.ModifyUserFName = DecryptFNames(dr["C020"]);
                        tasks.TaskAllRecordLength = Convert.ToInt64(dr["C021"]);
                        tasks.TaskFinishRecordLength = Convert.ToInt64(dr["C022"]);

                        tasks.FinishUserFName = DecryptFNames(dr["FinishUserFName"]);
                        tasks.FinishUserID = Convert.ToInt64(dr["FinishUserID"]);
                        optReturn = XMLHelper.SeriallizeObject(tasks);
                        if (!optReturn.Result)
                        {
                            break;
                        }
                        listRoleUsers.Add(optReturn.Data.ToString());
                    }
                    optReturn.Data = listRoleUsers;
                }
                catch (Exception ex)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_DBACCESS_FAIL;
                    optReturn.Message = ex.Message;
                }
                finally
                {
                    objConn.Close();
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
            optReturn.Message += strSql;
            return optReturn;
        }

        private OperationReturn GetCanOperationTasks(SessionInfo session, List<string> listParams)
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
                string strSql;
                IDbConnection objConn = null;
                IDbDataAdapter objAdapter = null;
                DataSet objDataSet = new DataSet();
                string rentToken = session.RentInfo.Token;
                switch (session.DBType)
                {
                    //MSSQL
                    case 2:
                        strSql = string.Format("select * from T_31_020_{0} where c001 in (select distinct c001 from T_31_021_{0} where c002 in ({1})) AND (c004 = '{2}' OR c005 = 'Y') AND C017 = 'N'", rentToken, listParams[0], listParams[1]);
                        objConn = MssqlOperation.GetConnection(session.DBConnectionString);
                        objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                        break;
                    //ORCL
                    case 3:
                        strSql = string.Format("select * from T_31_020_{0} where c001 in (select distinct c001 from T_31_021_{0} where c002 in ({1})) AND (c004 = '{2}' OR c005 = 'Y') AND C017 = 'N'", rentToken, listParams[0], listParams[1]);
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
                #region
                try
                {
                    objAdapter.Fill(objDataSet);

                    List<string> listRoleUsers = new List<string>();
                    for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                    {
                        DataRow dr = objDataSet.Tables[0].Rows[i];
                        UserTasksInfoShow tasks = new UserTasksInfoShow();
                        tasks.TaskID = Convert.ToInt64(dr["C001"]);
                        tasks.TaskName = dr["C002"] != null ? dr["C002"].ToString() : "";
                        tasks.AssignNum = Convert.ToInt32(dr["C008"]);
                        tasks.TaskDesc = dr["C003"] != null ? dr["C003"].ToString() : "";
                        tasks.AssignUserFName = DecryptFNames(dr["C019"]);
                        tasks.TaskType = Convert.ToInt32(dr["C004"]);
                        tasks.IsShare = dr["C005"] != null ? dr["C005"].ToString() : "";
                        tasks.TaskAllRecordLength = Convert.ToInt32(dr["C021"]);
                        optReturn = XMLHelper.SeriallizeObject(tasks);
                        if (!optReturn.Result)
                        {
                            break;
                        }
                        listRoleUsers.Add(optReturn.Data.ToString());
                    }
                    optReturn.Data = listRoleUsers;
                }
                catch (Exception ex)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_DBACCESS_FAIL;
                    optReturn.Message = ex.Message;
                }
                finally
                {
                    objConn.Close();
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

        /// Author           : Luoyihua
        ///  Created          : 2014-11-06 16:46:00
        /// <summary>
        /// Gets the task record by task identifier.
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="listParams">参数0位置：任务ID，参数1位置：任务名称</param>
        /// <returns>OperationReturn.</returns>
        private OperationReturn GetTaskRecordByTaskID(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            OperationReturn optReturn1=new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            string strSql = "";
            try
            {
                if (listParams.Count < 1)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Param count invalid");
                    return optReturn;
                }
                IDbConnection objConn = null;
                IDbDataAdapter objAdapter = null;
                DataSet objDataSet = new DataSet();
                string rentToken = session.RentInfo.Token;
                switch (session.DBType)
                {
                    //MSSQL
                    case 2:
                        strSql = string.Format("select C001,C002,C003,C004,C005,C006,C007,C008,C009,C010,C011,C012,C013,C014,C015,C016,C08,C09,C10,C11,C12,C13,C14 from " +
                                               "(select a.*,row_number() over(partition by c002 order by C11 desc) as sid from " +
                                               "(SELECT T22.*,T20.C002 C08,T05.C003 C09,T08.C004 C10,T08.C006 C11,T08.C003 C12,T08.C001 C13,T08.C005 C14 " +
                                               "FROM T_31_022_{0} T22 LEFT JOIN T_31_008_{0} T08 " +
                                               "ON (T08.C002=T22.C002 AND (({2}) )) LEFT JOIN T_31_020_{0} T20 ON T22.C007 = T20.C001 LEFT JOIN T_31_021_{0} T21 " +
                                               "ON T22.C001 = T22.C001 LEFT JOIN T_11_005_{0} T05 ON T21.C002=T05.C001 " +
                                               " AND T05.C007<>'H' WHERE T22.C001={1} ) a) b where b.sid=1"
                                               , rentToken, listParams[0], listParams[3]);
                        objConn = MssqlOperation.GetConnection(session.DBConnectionString);
                        objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                        break;
                    //ORCL
                    case 3:
                        strSql = string.Format("select C001,C002,C003,C004,C005,C006,C007,C008,C009,C010,C08,C09,C10,C011,C012,C013,C014,C015,C016,C11,C12,C13,C14 from " +
                                               "(select a.*,row_number() over(partition by c002 order by C11 desc) as sid from " +
                                               "(SELECT T22.*,T20.C002 C08,T05.C003 C09,T08.C004 C10,T08.C006 C11,T08.C003 C12,T08.C001 C13,T08.C005 C14" +
                                               " FROM T_31_022_{0} T22 LEFT JOIN T_31_008_{0} T08 " +
                                               "ON (T08.C002=T22.C002 AND (({2}) )) LEFT JOIN T_31_020_{0} T20 ON T22.C007 = T20.C001 LEFT JOIN T_31_021_{0} T21 " +
                                               "ON T22.C001 = T22.C001 LEFT JOIN T_11_005_{0} T05 ON T21.C002=T05.C001 " +
                                               " AND T05.C007<>'H' WHERE T22.C001={1} ) a) b where b.sid=1"
                                               , rentToken, listParams[0], listParams[3]);
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
                #region
                try
                {
                    objAdapter.Fill(objDataSet);

                    List<string> listtaskdetail = new List<string>();
                    for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                    {
                        DataRow dr = objDataSet.Tables[0].Rows[i];
                        TaskInfoDetail tdetail = new TaskInfoDetail();
                        tdetail.TaskID = Convert.ToInt64(dr["C001"]);
                        tdetail.RecoredReference = Convert.ToInt64(dr["C002"]);
                        optReturn1 = GetMediaTypeByRecord(session, tdetail.RecoredReference.ToString(), listParams[2]);
                        if (!optReturn1.Result)
                        {
                            return optReturn1;
                        }
                        List<string> ListTemp = optReturn1.Data as List<string>;
                        if (ListTemp == null)
                        {
                            optReturn.Result = false;
                            optReturn.Code = Defines.RET_OBJECT_NULL;
                            optReturn.Message = string.Format("DataSet is null");
                            return optReturn;
                        }
                        tdetail.MediaType = Convert.ToInt32(ListTemp[0]);
                        tdetail.EncryptFlag = ListTemp[1];
                        tdetail.VoiceIP = ListTemp[2];
                        tdetail.ChannelID = Convert.ToInt32(ListTemp[3]);
                        tdetail.WaveFormat = ListTemp[4];
                        tdetail.RowID = Convert.ToInt64(ListTemp[5]);

                        tdetail.IsLock = dr["C003"] != null ? dr["C003"].ToString() : "";
                        tdetail.UserID = Convert2Long(dr["C004"]);
                        tdetail.LockTime = StrDatePress(dr["C005"]);
                        tdetail.AllotType = Convert2Int(dr["C006"]);

                        tdetail.FromTaskID = Convert2Long(dr["C007"]);
                        tdetail.FromTaskName = dr["C08"] == null ? "" : dr["C08"].ToString();
                        tdetail.TaskType = dr["C010"].ToString();
                        tdetail.UserFullName = DecryptFNames(dr["C09"]);
                        tdetail.TaskName = listParams[1];
                        tdetail.TaskScore = Convert2Double(dr["C10"]);
                        tdetail.AgtOrExtID = dr["C008"] != null ? dr["C008"].ToString() : "";
                        tdetail.AgtOrExtName = dr["C009"] != null ? dr["C009"].ToString() : "";
                        tdetail.TemplateID = Convert2Long(dr["C12"]);
                        tdetail.OldScoreID = Convert2Long(dr["C13"]);
                        tdetail.ScoreUserID = Convert2Long(dr["C14"]);
                        if (!string.IsNullOrWhiteSpace(dr["C011"].ToString()))
                        {
                            tdetail.StartRecordTime = Convert.ToDateTime(dr["C011"]).ToLocalTime();
                            tdetail.Duration = Converter.Second2Time(Convert.ToInt32(dr["C012"].ToString()));
                            tdetail.CallerID = EncryptToClient(dr["C014"].ToString());
                            tdetail.CalledID = EncryptToClient(dr["C015"].ToString());
                            tdetail.Direction = dr["C016"].ToString() == "1" ? 1 : 0;
                        }
                        optReturn = XMLHelper.SeriallizeObject(tdetail);
                        if (!optReturn.Result)
                        {
                            break;
                        }
                        listtaskdetail.Add(optReturn.Data.ToString());
                    }
                    optReturn.Data = listtaskdetail;
                }
                catch (Exception ex)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_DBACCESS_FAIL;
                    optReturn.Message = ex.Message;
                }
                finally
                {
                    objConn.Close();
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
            optReturn.Message += strSql;
            return optReturn;
        }

        /// Author           : Waves
        ///  Created          : 2015-9-4 11:17:20
        /// <summary>
        /// 查询该录音加密信息跟媒体类型
        /// </summary>
        /// <returns></returns>
        private OperationReturn GetMediaTypeByRecord(SessionInfo session, string RecordID, string isSeptable)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            string Str = string.Empty;
            string tableName = string.Empty;
            try
            {
                if(isSeptable=="1")//分表
                {
                    tableName = string.Format("T_21_001_{0}_{1}",session.RentInfo.Token,RecordID.Substring(0,4));
                }
                else
                { 
                    tableName = string.Format("T_21_001_{0}",session.RentInfo.Token); 
                }
                Str = string.Format("select T21.* from {0} T21 where T21.C002={1} AND T21.C014<2", tableName, RecordID);//录屏录音不允许被分配 so 要排除
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, Str);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, Str);
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
                DataRow dr = objDataSet.Tables[0].Rows[0];
                listReturn.Add(dr["C014"].ToString());//MediaType
                listReturn.Add(dr["C025"].ToString());//EncryptFlag
                listReturn.Add(dr["C020"].ToString());//VoiceIP
                listReturn.Add(dr["C038"].ToString());//ChannelID
                listReturn.Add(dr["C015"].ToString());//WaveFormat
                listReturn.Add(dr["C001"].ToString());//RowID
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

        /// Author           : Luoyihua
        ///  Created          : 2014-11-06 16:46:00
        /// <summary>
        /// 修改任务完成时间
        /// </summary>
        /// <param name="session">The session.</param>
        /// <param name="listParams">0:时间  1：任务ID</param>
        /// <returns>OperationReturn.</returns>
        public OperationReturn ModifyTaskDealLine(SessionInfo session, List<string> listParams)
        {
            //DBConnection or DBDataAdapter is null
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            string strSql = "";
            try
            {
                if (listParams.Count < 1)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Param count invalid");
                    return optReturn;
                }
                DataSet objDataSet = new DataSet();
                string rentToken = session.RentInfo.Token;
                switch (session.DBType)
                {
                    //MSSQL
                    case 2:
                        strSql = string.Format("update T_31_020_{0} set T_31_020_{0}.C009='{1}',T_31_020_{0}.C011='{5}',T_31_020_{0}.C012={3},T_31_020_{0}.C020='{4}' where T_31_020_{0}.C001={2} ", rentToken, listParams[0], listParams[1], session.UserInfo.UserID, session.UserInfo.UserName, listParams[2]);
                        optReturn = MssqlOperation.ExecuteSql(session.DBConnectionString, strSql);
                        break;
                    //ORCL
                    case 3:
                        strSql = string.Format("update T_31_020_{0} set T_31_020_{0}.C009=to_date('{1}','YYYY/MM/DD HH24:MI:SS'),T_31_020_{0}.C011=to_date('{5}','YYYY/MM/DD HH24:MI:SS'),T_31_020_{0}.C012={3},T_31_020_{0}.C020='{4}' where T_31_020_{0}.C001={2} ", rentToken, listParams[0], listParams[1], session.UserInfo.UserID, session.UserInfo.UserName, listParams[2]);
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

        private OperationReturn GetControlAgentInfoList(SessionInfo session, List<string> listParams)
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
                if (listParams == null || listParams.Count < 2)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strUserID = listParams[0];
                string strParentID = listParams[1];
                string path1 = string.Empty;
                string path2 = string.Empty;
                if (listParams[2] == "A")
                {
                    path1 = string.Format("1030000000000000000");
                    path2 = string.Format("1040000000000000000");
                }
                if (listParams[2] == "E")
                {
                    path1 = string.Format("1040000000000000000");
                    path2 = string.Format("1050000000000000000");
                }
                if (listParams[2] == "R")
                {
                    path1 = string.Format("1050000000000000000");
                    path2 = string.Format("1060000000000000000");
                }
                string rentToken = session.RentInfo.Token;
                string strSql;
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        strSql =
                            string.Format(
                                "SELECT * FROM T_11_101_{0} WHERE C011 = '{1}' AND C001 IN (SELECT C004 FROM T_11_201_{0} WHERE C003 = {2}) and C001 >= {3} and c001 < {4}"
                                , rentToken, strParentID, strUserID,path1,path2);
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
                                "SELECT * FROM T_11_101_{0} WHERE C011 = '{1}' AND C001 IN (SELECT C004 FROM T_11_201_{0} WHERE C003 = {2}) and c001 >= {3} and c001 < {4}"
                                , rentToken, strParentID, strUserID, path1, path2);
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
                    string strInfo = string.Format("{0}{1}{2}{1}{3}", strID, ConstValue.SPLITER_CHAR, strName, strFullName);
                    listReturn.Add(strInfo);
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

        private OperationReturn GetUserScoreSheetList(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            string strSql = "";
            string strSql1 = "";
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
                string strTaskID = listParams[3];
                string scoreUserID ;
                if (listParams.Count < 5)
                {
                    scoreUserID = session.UserID.ToString();
                }
                else
                {
                    scoreUserID = string.IsNullOrWhiteSpace(listParams[4]) ? session.UserID.ToString() : listParams[4];
                }
                string rentToken = session.RentInfo.Token;
                DataSet objDataSet;             
                switch(session.DBType)
                {
                    case 2:
                        strSql1 = string.Format("SELECT * FROM T_31_020_{0} WHERE C001={1}",rentToken,strTaskID);                   
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql1);
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
                string str = string.Empty;
                for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = objDataSet.Tables[0].Rows[i];
                    str = dr["C023"].ToString();
                }
                switch (session.DBType)
                {
                    case 2:
                        if (strMethod == "0")
                        {
                            if (str == "0")
                            {
                                strSql = string.Format("SELECT *  FROM T_31_001_{0} WHERE C001 IN (SELECT C004 FROM T_11_201_{0} WHERE C003 = {1}) AND C010 > '{2}' AND C018='Y' "
                                , rentToken
                                , strAgtID
                                , DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss")
                                , strTaskID);        
                            }
                            else 
                            {
                                strSql = string.Format("SELECT *  FROM T_31_001_{0} WHERE C001 IN (SELECT C004 FROM T_11_201_{0} WHERE C003 = {1}) AND C001 IN (SELECT C023 FROM T_31_020_{0} WHERE C001={3}) AND C010 > '{2}' AND C018='Y' "
                              , rentToken
                              , strAgtID
                              , DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss")
                              , strTaskID);        
                            }
                                      
                        }
                        else
                        {
                            strSql = string.Format("SELECT A.C001, A.C002, A.C004, B.C001 AS BC001, B.C002 AS BC002, B.C004 AS BC004, B.C013 AS BC013 ");
                            strSql += string.Format("FROM T_31_001_{0} A, T_31_008_{0} B ", rentToken);
                            strSql += string.Format("WHERE A.C001 = B.C003 AND B.C003 IN (SELECT C004 ");
                            strSql += string.Format("FROM T_11_201_{0} WHERE C003 = {1}) AND B.C002 = {2} AND B.C005 = {3}",
                                rentToken,strAgtID,strRecordID,scoreUserID);
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
                                rentToken,strAgtID,strRecordID,scoreUserID);
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
                if (listParams == null || listParams.Count < 4)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strScoreSheetResult = listParams[0];
                string totalSec = listParams[3];
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

                #region DBQuery

                string rentToken = session.RentInfo.Token;
                //是否增加记录
                bool isAdd = false;
                string strSql;
                IDbConnection objConn;
                IDbDataAdapter objAdapter;
                DbCommandBuilder objCmdBuilder;

                OperationReturn optReturnTask = new OperationReturn();
                string strSqlTask;
                string taskid = listParams[2];
                bool isNewScore = listParams[1] == "3" ? true : false;
                if (!isNewScore) isNewScore = listParams[1] == "5" ? true : false;
                IDbConnection objConnTask;
                IDbDataAdapter objAdapterTask;
                DbCommandBuilder objCmdBuilderTask;

                string str341;
                string RecheckUpdateStr ;
                OperationReturn optReturnTemplelate41 = new OperationReturn();
                IDbConnection objConnTemplelate41;
                IDbDataAdapter objAdapterTemplelate41;
                DbCommandBuilder objCmdBuilderTemplelate41;
                switch (session.DBType)
                {
                    case 2:
                        if (listParams[1] == "5")
                        {
                            RecheckUpdateStr = string.Format("UPDATE T_31_008_{0} SET C009='N' WHERE C002={1} AND C003={2};UPDATE T_31_041_{0} SET C005='0' WHERE C002={1} AND C000={2};", rentToken, scoreSheetResult.RecordSerialID, scoreSheetResult.ScoreSheetID);
                            optReturn = MssqlOperation.ExecuteSql(session.DBConnectionString, RecheckUpdateStr);
                        }

                        strSql = string.Format("SELECT * FROM T_31_008_{0} WHERE C001 = {1} OR C001 = {2}",rentToken,strResultID,strNewResultID);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objConn = MssqlOperation.GetConnection(session.DBConnectionString);
                        objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = MssqlOperation.GetCommandBuilder(objAdapter);

                        strSqlTask = string.Format("SELECT * FROM T_31_020_{0} WHERE C001={1}", rentToken, taskid);
                        optReturnTask = MssqlOperation.GetDataSet(session.DBConnectionString, strSqlTask);
                        if (!optReturnTask.Result)
                        {
                            return optReturnTask;
                        }
                        objConnTask = MssqlOperation.GetConnection(session.DBConnectionString);
                        objAdapterTask = MssqlOperation.GetDataAdapter(objConnTask, strSqlTask);
                        objCmdBuilderTask = MssqlOperation.GetCommandBuilder(objAdapterTask);

                        str341 = string.Format("SELECT * FROM T_31_041_{0} WHERE C001={1} ", rentToken, strResultID);
                        optReturnTemplelate41 = MssqlOperation.GetDataSet(session.DBConnectionString, str341);
                        if (!optReturnTemplelate41.Result)
                        {
                            return optReturnTemplelate41;
                        }
                        objConnTemplelate41 = MssqlOperation.GetConnection(session.DBConnectionString);
                        objAdapterTemplelate41 = MssqlOperation.GetDataAdapter(objConnTemplelate41, str341);
                        objCmdBuilderTemplelate41 = MssqlOperation.GetCommandBuilder(objAdapterTemplelate41);

                        break;
                    case 3:
                        if (listParams[1] == "5")
                        {
                            RecheckUpdateStr = string.Format("BEGIN UPDATE T_31_008_{0} SET C009='N' WHERE C002={1} AND C003={2};UPDATE T_31_041_{0} SET C005='0' WHERE C002={1} AND C000={2};END", rentToken, scoreSheetResult.RecordSerialID, scoreSheetResult.ScoreSheetID);
                            optReturn = OracleOperation.ExecuteSql(session.DBConnectionString, RecheckUpdateStr);
                        }

                        strSql = string.Format("SELECT * FROM T_31_008_{0} WHERE C001 = {1} OR C001 = {2}",rentToken,strResultID,strNewResultID);
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objConn = OracleOperation.GetConnection(session.DBConnectionString);
                        objAdapter = OracleOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = OracleOperation.GetCommandBuilder(objAdapter);

                        strSqlTask = string.Format("SELECT * FROM T_31_020_{0} WHERE C001={1}", rentToken, taskid);
                        optReturnTask = OracleOperation.GetDataSet(session.DBConnectionString, strSqlTask);
                        if (!optReturnTask.Result)
                        {
                            return optReturnTask;
                        }
                        objConnTask = OracleOperation.GetConnection(session.DBConnectionString);
                        objAdapterTask = OracleOperation.GetDataAdapter(objConnTask, strSqlTask);
                        objCmdBuilderTask = OracleOperation.GetCommandBuilder(objAdapterTask);

                        str341 = string.Format("SELECT * FROM T_31_041_{0} WHERE C001={1} ", rentToken, strResultID);
                        optReturnTemplelate41 = OracleOperation.GetDataSet(session.DBConnectionString, str341);
                        if (!optReturnTemplelate41.Result)
                        {
                            return optReturnTemplelate41;
                        }
                        objConnTemplelate41 = OracleOperation.GetConnection(session.DBConnectionString);
                        objAdapterTemplelate41 = OracleOperation.GetDataAdapter(objConnTemplelate41, str341);
                        objCmdBuilderTemplelate41 = OracleOperation.GetCommandBuilder(objAdapterTemplelate41);
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
                if (objConnTask == null || objAdapterTask == null || objCmdBuilderTask == null)
                {
                    optReturnTask.Result = false;
                    optReturnTask.Code = Defines.RET_OBJECT_NULL;
                    optReturnTask.Message = string.Format("Db1 object is null");
                    return optReturnTask;
                }
                if (objConnTemplelate41 == null || objAdapterTemplelate41 == null || objCmdBuilderTemplelate41 == null)
                {
                    optReturnTemplelate41.Result = false;
                    optReturnTemplelate41.Code = Defines.RET_OBJECT_NULL;
                    optReturnTemplelate41.Message = string.Format("Db1 object is null");
                    return optReturnTemplelate41;
                }
                #endregion

                #region 更新任务信息
                objCmdBuilderTask.ConflictOption = ConflictOption.OverwriteChanges;
                objCmdBuilderTask.SetAllValues = false;

                try
                {
                    DataSet objDataSetTask = new DataSet();
                    objAdapterTask.Fill(objDataSetTask);
                    DataRow dr = objDataSetTask.Tables[0].Rows[0];
                    int assignnum = int.Parse(dr["C008"].ToString());
                    int finishnum = int.Parse(dr["C010"].ToString());
                    if (isNewScore && assignnum > finishnum)//新增任分数 完成任务数量+1
                    {
                        dr["C010"] = finishnum + 1;
                    }
                    if (isNewScore && assignnum - 1 <= finishnum)
                    {
                        dr["C018"] = DateTime.Now.ToUniversalTime();
                        dr["C017"] = "Y";
                    }
                    objAdapterTask.Update(objDataSetTask);
                    objDataSetTask.AcceptChanges();
                }
                catch (Exception ex)
                {
                    optReturnTask.Result = false;
                    optReturnTask.Code = Defines.RET_DBACCESS_FAIL;
                    optReturnTask.Message = ex.Message;
                    return optReturnTask;
                }
                finally
                {
                    if (objConnTask.State == ConnectionState.Open)
                    {
                        objConnTask.Close();
                    }
                    objConnTask.Dispose();
                }
                optReturnTask.Data = taskid;
                optReturn.StringValue = "1";//更新任务表操作完成
                #endregion

                #region 更新T_31_041 的C005
                if (!isNewScore)
                {
                    objCmdBuilderTemplelate41.ConflictOption = ConflictOption.OverwriteChanges;
                    objCmdBuilderTemplelate41.SetAllValues = false;

                    try
                    {
                        DataSet objDataSetTemplelate41 = new DataSet();
                        objAdapterTemplelate41.Fill(objDataSetTemplelate41);
                        DataRow dr = objDataSetTemplelate41.Tables[0].Rows[0];
                        dr["C005"] = "0";
                        objAdapterTemplelate41.Update(objDataSetTemplelate41);
                        objDataSetTemplelate41.AcceptChanges();
                    }
                    catch (Exception ex)
                    {
                        optReturnTemplelate41.Result = false;
                        optReturnTemplelate41.Code = Defines.RET_DBACCESS_FAIL;
                        optReturnTemplelate41.Message = ex.Message;
                        return optReturnTemplelate41;
                    }
                    finally
                    {
                        if (objConnTemplelate41.State == ConnectionState.Open)
                        {
                            objConnTemplelate41.Close();
                        }
                        objConnTemplelate41.Dispose();
                    }

                    optReturn.StringValue = "3";//修改T_31_041操作完成
                }
                #endregion

                #region DBOperation
                objCmdBuilder.ConflictOption = ConflictOption.OverwriteChanges;
                objCmdBuilder.SetAllValues = false;
                try
                {
                    DataSet objDataSet = new DataSet();
                    objAdapter.Fill(objDataSet);
                    DataRow drModify, drAdd;
                    //修改成绩时，向成绩表里新增一行成绩信息，同时将原成绩记录的C009字段值为N
                    drModify = objDataSet.Tables[0].Select(string.Format("C001 = {0}", strResultID)).FirstOrDefault();
                    if (drModify != null)
                    {
                        drModify["C009"] = "N";
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
                    drAdd["C012"] = scoreSheetResult.OrgID;
                    drAdd["C013"] = scoreSheetResult.UserID;
                    if (taskid == "-1" && listParams[1] == "7")
                        drAdd["C014"] = "2";
                    else
                        drAdd["C014"] = "0";
                    drAdd["C015"] = 0;
                    drAdd["C016"] = totalSec;
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
                optReturn.StringValue = "3";//修改成绩表操作完成
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
                    optReturn.StringValue = "4";//更新评分表次数完成
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

        private OperationReturn DeleteErrorDB(SessionInfo session, List<string> listParams)
        {
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
                string strSql;
                DataSet objDataSet = new DataSet();
                string rentToken = session.RentInfo.Token;
                switch (session.DBType)
                {
                    //MSSQL
                    case 2:
                        strSql = string.Format("delete T_31_008_{0} where c001={1}", rentToken, listParams[0]);
                        optReturn = MssqlOperation.ExecuteSql(session.DBConnectionString, strSql);
                        break;
                    //ORCL
                    case 3:
                        strSql = string.Format("delete T_31_008_{0} where c001={1}", rentToken, listParams[0]);
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


        private OperationReturn SaveScoreItemResultInfo(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0     评分表编码
                //1     成绩编码
                //2     打分项总数
                //3..   打分项得分信息
                if (listParams == null || listParams.Count < 3)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strScoreSheetID = listParams[0];
                string strResultID = listParams[1];
                string strCount = listParams[2];
                int intCount;
                if (!int.TryParse(strCount, out intCount))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("ScoreResultCount param invalid");
                    return optReturn;
                }
                if (listParams.Count < 3 + intCount)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("ScoreResult count invalid");
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
                        strSql = string.Format("SELECT * FROM T_31_009_{0} WHERE C001 = {1} AND C003 = {2}",
                            rentToken,
                            strScoreSheetID,
                            strResultID);
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
                        strSql = string.Format("SELECT * FROM T_31_009_{0} WHERE C001 = {1} AND C003 = {2}",
                            rentToken,
                            strScoreSheetID,
                            strResultID);
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
                bool isAdd = false;
                List<string> listReturn = new List<string>();
                try
                {
                    DataSet objDataSet = new DataSet();
                    objAdapter.Fill(objDataSet);

                    for (int i = 0; i < intCount; i++)
                    {
                        string strScoreItemResult = listParams[3 + i];
                        optReturn = XMLHelper.DeserializeObject<BasicScoreItemInfo>(strScoreItemResult);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        BasicScoreItemInfo scoreItemResult = optReturn.Data as BasicScoreItemInfo;
                        if (scoreItemResult == null)
                        {
                            optReturn.Result = false;
                            optReturn.Code = Defines.RET_OBJECT_NULL;
                            optReturn.Message = string.Format("ScoreItemResult is null");
                            return optReturn;
                        }
                        DataRow[] drs =
                            objDataSet.Tables[0].Select(string.Format("C002 = {0}", scoreItemResult.ScoreItemID));
                        DataRow dr;
                        if (drs.Length <= 0)
                        {
                            dr = objDataSet.Tables[0].NewRow();
                            dr["C001"] = strScoreSheetID;
                            dr["C002"] = scoreItemResult.ScoreItemID;
                            dr["C003"] = strResultID;
                            isAdd = true;
                        }
                        else
                        {
                            dr = drs[0]; 
                            isAdd = false;
                        }
                        dr["C004"] = scoreItemResult.Score;
                        dr["C005"] = scoreItemResult.IsNA;
                        dr["C006"] = "N";
                        dr["C007"] = "N";
                        dr["C008"] = scoreItemResult.RealScore;
                        if (isAdd)
                        {
                            objDataSet.Tables[0].Rows.Add(dr);
                            listReturn.Add(string.Format("Add{0}{1}", ConstValue.SPLITER_CHAR,
                                scoreItemResult.ScoreItemID));
                        }
                        else
                        {
                            listReturn.Add(string.Format("Modify{0}{1}", ConstValue.SPLITER_CHAR,
                              scoreItemResult.ScoreItemID));
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

        private OperationReturn SaveScoreCommentResultInfo(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0     成绩编码
                //1     评分备注项总数
                //2...     评分备注项信息
                if (listParams == null || listParams.Count < 2)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strResultID = listParams[0];
                string strCount = listParams[1];
                int intCount;
                if (!int.TryParse(strCount, out intCount))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("ScoreCommentResultCount param invalid");
                    return optReturn;
                }
                if (listParams.Count < 2 + intCount)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("ScoreCommentResult count invalid");
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
                        strSql = string.Format("SELECT * FROM T_31_011_{0} WHERE C001 = {1}",
                            rentToken,
                            strResultID);
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
                        strSql = string.Format("SELECT * FROM T_31_011_{0} WHERE C001 = {1}",
                            rentToken,
                            strResultID);
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
                bool isAdd = false;
                List<string> listReturn = new List<string>();
                try
                {
                    DataSet objDataSet = new DataSet();
                    objAdapter.Fill(objDataSet);

                    for (int i = 0; i < intCount; i++)
                    {
                        string strScoreCommentResult = listParams[2 + i];
                        optReturn = XMLHelper.DeserializeObject<BasicScoreCommentInfo>(strScoreCommentResult);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        BasicScoreCommentInfo scoreCommentResult = optReturn.Data as BasicScoreCommentInfo;
                        if (scoreCommentResult == null)
                        {
                            optReturn.Result = false;
                            optReturn.Code = Defines.RET_OBJECT_NULL;
                            optReturn.Message = string.Format("ScoreItemResult is null");
                            return optReturn;
                        }
                        DataRow[] drs =
                            objDataSet.Tables[0].Select(string.Format("C002 = {0}", scoreCommentResult.ScoreCommentID));
                        DataRow dr;
                        if (drs.Length <= 0)
                        {
                            dr = objDataSet.Tables[0].NewRow();
                            dr["C001"] = strResultID;
                            dr["C002"] = scoreCommentResult.ScoreCommentID;
                            isAdd = true;
                        }
                        else
                        {
                            dr = drs[0];
                        }
                        dr["C003"] = scoreCommentResult.CommentText;
                        dr["C004"] = scoreCommentResult.CommentItemOrderID;
                        dr["C005"] = scoreCommentResult.CommentItemID;
                        if (isAdd)
                        {
                            objDataSet.Tables[0].Rows.Add(dr);

                            listReturn.Add(string.Format("Add{0}{1}", ConstValue.SPLITER_CHAR,
                                scoreCommentResult.ScoreCommentID));
                        }
                        else
                        {
                            listReturn.Add(string.Format("Modify{0}{1}", ConstValue.SPLITER_CHAR,
                              scoreCommentResult.ScoreCommentID));
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

        private OperationReturn GetScoreResultList(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            string strSql="";
            try
            {
                //ListParam
                //0      记录编码
                //1      用户编码
                //2      评分表编码
                if (listParams == null || listParams.Count < 4)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strRecordID = listParams[0];
                string strUserID = listParams[1];
                string strScoreSheetID = listParams[2];
                string strScoreResultID = listParams[3];
                if (string.IsNullOrEmpty(strRecordID)
                    || string.IsNullOrEmpty(strUserID)
                    || string.IsNullOrEmpty(strScoreSheetID))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param invalid");
                    return optReturn;
                }
                string rentToken = session.RentInfo.Token;
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT * FROM T_31_009_{0} WHERE C001 = {1} AND C003 IN ({2})", rentToken, strScoreSheetID, strScoreResultID);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM T_31_009_{0} WHERE C001 = {1} AND C003 IN ({2})", rentToken, strScoreSheetID,strScoreResultID);
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
                    BasicScoreItemInfo item = new BasicScoreItemInfo();
                    item.ScoreResultID = Convert.ToInt64(dr["C003"]);
                    item.ScoreSheetID = Convert.ToInt64(dr["C001"]);
                    item.ScoreItemID = Convert.ToInt64(dr["C002"]);
                    item.RecordSerialID = Convert.ToInt64(strRecordID);
                    item.UserID = Convert.ToInt64(strUserID);
                    item.Score = Convert.ToDouble(dr["C004"]);
                    item.IsNA = dr["C005"].ToString();
                    item.RealScore = Convert.ToDouble(dr["C008"]);
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

        private OperationReturn GetScoreCommentResultList(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0      成绩ID
                if (listParams == null || listParams.Count < 1)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strScoreResultID = listParams[0];
                if (string.IsNullOrEmpty(strScoreResultID))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("ScoreResultID param invalid\t{0}", strScoreResultID);
                    return optReturn;
                }
                string rentToken = session.RentInfo.Token;
                string strSql;
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT * FROM T_31_011_{0} WHERE C001 = {1}",
                            rentToken,
                            strScoreResultID);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM T_31_011_{0} WHERE C001 = {1}",
                             rentToken,
                             strScoreResultID);
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
                    BasicScoreCommentInfo item = new BasicScoreCommentInfo();
                    item.ScoreResultID = Convert.ToInt64(dr["C001"]);
                    item.ScoreCommentID = Convert.ToInt64(dr["C002"]);
                    item.CommentText = dr["C003"].ToString();
                    item.CommentItemOrderID = Convert.ToInt32(dr["C004"]);
                    item.CommentItemID = Convert.ToInt64(dr["C005"]);
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

        private OperationReturn GetRecordMemoList(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0      记录编码
                if (listParams == null || listParams.Count < 1)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strRecordID = listParams[0];
                string rentToken = session.RentInfo.Token;
                string strSql;
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT * FROM T_31_046_{0} WHERE C002 = {1}"
                            , rentToken
                            , strRecordID);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM T_31_046_{0} WHERE C002 = {1}"
                           , rentToken
                           , strRecordID);
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
                    RecordMemoInfo item = new RecordMemoInfo();
                    item.ID = Convert.ToInt64(dr["C001"]);
                    item.RecordSerialID = Convert.ToInt64(dr["C002"]);
                    item.UserID = Convert.ToInt64(dr["C003"]);
                    item.MemoTime = Converter.NumberToDatetime(dr["C004"].ToString());
                    item.Content = dr["C005"].ToString();
                    item.State = dr["C006"].ToString();
                    item.LastModifyUserID = string.IsNullOrEmpty(dr["C007"].ToString())
                        ? 0
                        : Convert.ToInt64(dr["C007"]);
                    item.LastModifyTime = string.IsNullOrEmpty(dr["C008"].ToString())
                        ? DateTime.MinValue
                        : Converter.NumberToDatetime(dr["C008"].ToString());
                    item.DeleteUserID = string.IsNullOrEmpty(dr["C009"].ToString()) ? 0 : Convert.ToInt64(dr["C009"]);
                    item.DeleteTime = string.IsNullOrEmpty(dr["C010"].ToString())
                        ? DateTime.MinValue
                        : Converter.NumberToDatetime(dr["C010"].ToString());
                    item.Source = dr["C011"].ToString();
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

        private OperationReturn SaveRecordMemoInfo(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0     记录ID
                //1     用户ID
                //2     记录备注信息
                if (listParams == null || listParams.Count < 3)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strRecordID = listParams[0];
                string strUserID = listParams[1];
                string strMemoInfo = listParams[2];
                if (string.IsNullOrEmpty(strRecordID)
                    || string.IsNullOrEmpty(strUserID)
                    || string.IsNullOrEmpty(strMemoInfo))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param invalid");
                    return optReturn;
                }
                optReturn = XMLHelper.DeserializeObject<RecordMemoInfo>(strMemoInfo);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                RecordMemoInfo memoInfo = optReturn.Data as RecordMemoInfo;
                if (memoInfo == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("RecordMemoInfo is null");
                    return optReturn;
                }
                if (memoInfo.ID <= 0)
                {
                    //获取新增备注ID
                    WebRequest webRequest = new WebRequest();
                    webRequest.Session = session;
                    webRequest.Code = (int)RequestCode.WSGetSerialID;
                    webRequest.ListData.Add("31");
                    webRequest.ListData.Add("306");
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
                    memoInfo.ID = Convert.ToInt64(webReturn.Data);
                }
                string rentToken = session.RentInfo.Token;
                string strSql;
                IDbConnection objConn;
                IDbDataAdapter objAdapter;
                DbCommandBuilder objCmdBuilder;
                bool bIsAdded = false;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT * FROM T_31_046_{0} WHERE C001 = {1}"
                           , rentToken
                           , memoInfo.ID);
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
                        strSql = string.Format("SELECT * FROM T_31_046_{0} WHERE C001 = {1}"
                           , rentToken
                           , memoInfo.ID);
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
                try
                {
                    DataSet objDataSet = new DataSet();
                    objAdapter.Fill(objDataSet);
                    DataRow dr;
                    if (objDataSet.Tables[0].Rows.Count <= 0)
                    {
                        dr = objDataSet.Tables[0].NewRow();
                        dr["C001"] = memoInfo.ID;
                        bIsAdded = true;
                    }
                    else
                    {
                        dr = objDataSet.Tables[0].Rows[0];
                    }
                    dr["C002"] = memoInfo.RecordSerialID;
                    dr["C003"] = memoInfo.UserID;
                    dr["C004"] = Convert.ToInt64(memoInfo.MemoTime.ToString("yyyyMMddHHmmss"));
                    dr["C005"] = memoInfo.Content;
                    dr["C006"] = memoInfo.State;
                    dr["C007"] = memoInfo.LastModifyUserID;
                    dr["C008"] = Convert.ToInt64(memoInfo.LastModifyTime.ToString("yyyyMMddHHmmss"));
                    dr["C009"] = memoInfo.DeleteUserID;
                    dr["C010"] = Convert.ToInt64(memoInfo.DeleteTime.ToString("yyyyMMddHHmmss"));
                    dr["C011"] = memoInfo.Source;
                    if (bIsAdded)
                    {
                        objDataSet.Tables[0].Rows.Add(dr);
                    }

                    objAdapter.Update(objDataSet);
                    objDataSet.AcceptChanges();
                    bIsAdded = true;
                }
                catch (Exception ex)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_DBACCESS_FAIL;
                    optReturn.Message = ex.Message;
                    bIsAdded = false;
                }
                finally
                {
                    if (objConn.State == ConnectionState.Open)
                    {
                        objConn.Close();
                    }
                    objConn.Dispose();
                }
                if (!bIsAdded)
                {
                    return optReturn;
                }
                optReturn.Data = memoInfo.ID;
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

        private OperationReturn GetKeyWordOfRecord(SessionInfo session, List<string> listParams) 
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0      查询语句
                if (listParams == null || listParams.Count < 2)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }

                if (!IsExitRecordTable(session, listParams[0]))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_FAIL;
                    optReturn.Message = S3103Consts.Err_TableNotExit;
                    return optReturn;
                }

                string strTableName = listParams[0];
                string strRecordSerialID = listParams[1];
                string strSql = string.Empty;
                DataSet objDataSet;
                switch (session.DBType)
                {
                   case 2:
                        strSql = string.Format("SELECT  {0}.C001, {0}.C002, {0}.C005, {0}.C006, {0}.C007, {0}.C008,{0}.C009,{0}.C010, {0}.C013  ,T_51_006_{2}.C003 AS Name ,T_51_006_{2}.C004 AS Path  FROM {0}  LEFT JOIN T_51_006_{2} ON  {0}.C009= T_51_006_{2}.C001 WHERE {0}.C002={1} ", strTableName, strRecordSerialID, session.RentInfo.Token);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        strSql = string.Format("SELECT  {0}.C001, {0}.C002, {0}.C005, {0}.C006, {0}.C007, {0}.C008,{0}.C009,{0}.C010, {0}.C013  ,T_51_006_{2}.C003 AS Name ,T_51_006_{2}.C004 AS Path FROM {0}  LEFT JOIN T_51_006_{2} ON  {0}.C009= T_51_006_{2}.C001 WHERE {0}.C002={1} ", strTableName, strRecordSerialID, session.RentInfo.Token);
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
                    RecordKeyWordInfo item = new RecordKeyWordInfo();
                    item.SerialID = Convert.ToInt64(dr["C001"]);
                    item.RecordID = Convert.ToInt64(dr["C002"]);
                    item.Offset=Convert.ToInt32(dr["C005"]);
                    item.Duration =Convert.ToInt32(dr["C006"]);
                    item.Title= dr["C007"].ToString();
                    item.Content=dr["C008"].ToString();  
                    item.TitleID = Convert.ToInt64(dr["C009"]);
                    item.ContentID = Convert.ToInt64(dr["C010"]);
                    item.GloryNumber = Convert.ToInt64(dr["C013"]);
                    item.PictureName = dr["Name"].ToString();
                    item.PicturePath = dr["Path"].ToString();
                  
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

        public static DateTime NumberToDatetime(string number)
        {
            number = number.PadLeft(14);
            string str = number.Substring(0, 4);
            string str2 = number.Substring(4, 2);
            string str3 = number.Substring(6, 2);
            string str4 = number.Substring(8, 2);
            string str5 = number.Substring(10, 2);
            string str6 = number.Substring(12, 2);
            return DateTime.Parse(string.Format("{0}-{1}-{2} {3}:{4}:{5}", new object[] { str, str2, str3, str4, str5, str6 }));
        }
        

        private OperationReturn GetRecordData(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0      查询语句
                if (listParams == null || listParams.Count < 2)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }

                if (!IsExitRecordTable(session, listParams[1]))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_FAIL;
                    optReturn.Message = S3103Consts.Err_TableNotExit;
                    return optReturn;
                }

                string strQueryString = listParams[0];
                string strSql = strQueryString;
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
                    RecordInfo item = new RecordInfo();
                    item.RowID = Convert.ToInt64(dr["C001"]);
                    item.SerialID = Convert.ToInt64(dr["C002"]);
                    item.RecordReference = dr["C077"].ToString();
                    item.StartRecordTime = Convert.ToDateTime(dr["C005"]).ToLocalTime();
                    item.StopRecordTime = Convert.ToDateTime(dr["C009"]).ToLocalTime();
                    item.VoiceIP = dr["C020"].ToString();
                    item.VoiceID = Convert.ToInt32(dr["C037"]);
                    item.ChannelID = Convert.ToInt32(dr["C038"]);
                    item.ChannelName = dr["C046"].ToString();
                    item.Extension = dr["C042"].ToString();
                    item.Agent = dr["C039"].ToString();
                    item.ReaExtension = dr["C058"].ToString();
                    item.Duration = Convert.ToInt32(dr["C012"]);
                    item.Direction = dr["C045"].ToString() == "1" ? 1 : 0;
                    item.CallerID = EncryptToClient(dr["C040"].ToString());
                    item.CalledID = EncryptToClient(dr["C041"].ToString());
                    item.WaveFormat = dr["C015"].ToString();
                    item.MediaType = Convert.ToInt32(dr["C014"]);
                    item.EncryptFlag = dr["C025"].ToString();
                    if (listParams.Count == 2)//任务分配的查询
                        item.IsScore = dr["ISSCORE"].ToString();
                    else
                        item.IsScore = null;
                    if (strSql.Contains("SV"))
                    {
                        item.ServiceAttitude = Convert.ToInt32(dr["SV"]);
                    }
                    if (strSql.Contains("ProL"))
                    {
                        item.ProfessionalLevel = Convert.ToInt32(dr["ProL"]);
                    }
                    if (strSql.Contains("RoDE"))
                    {
                        item.RecordDurationError = Convert.ToInt32(dr["RoDE"]);
                    }
                    if (strSql.Contains("ReCI"))
                    {
                        item.RepeatCallIn = Convert.ToInt32(dr["ReCI"]);
                    }
                    if (strSql.Contains("AbnS"))
                    {
                        item.AbnormalScores = Convert.ToInt32(dr["AbnS"]);
                    }
                    if (strSql.Contains("AfEP")) 
                    {
                        item.AfterEventProcessing = Convert.ToInt32(dr["AfEP"]);
                    }
                    if (strSql.Contains("SeASP")) 
                    {
                        item.SeatAgentSpeechAnomaly = Convert.ToInt32(dr["SeASP"]);
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

        public OperationReturn DeleteRecordFromTask(SessionInfo session, List<string> listParams)
        {
            //DBConnection or DBDataAdapter is null
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                if (listParams.Count < 4)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Param count invalid");
                    return optReturn;
                }
                string strSql = string.Empty;
                IDbConnection objConn;
                IDbDataAdapter objAdapter;
                DbCommandBuilder objCmdBuilder;
                DataSet objDataSet = new DataSet();
                string rentToken = session.RentInfo.Token;
                //string tempSQL = string.Empty;
                //if (listParams[1] == listParams[3])
                //{
                //    tempSQL = string.Format(",C017 ='Y' ");
                //}
                switch (session.DBType)
                {
                    //MSSQL
                    case 2:
                        strSql = string.Format("delete from T_31_022_{0} where c002 in ({1}) AND C001={2}", rentToken, listParams[0], listParams[2]);
                        optReturn = MssqlOperation.ExecuteSql(session.DBConnectionString, strSql);
                        strSql = string.Format("SELECT * FROM T_31_020_{0} where c001 ={1}",rentToken,listParams[2]);
                        objConn = MssqlOperation.GetConnection(session.DBConnectionString);
                        objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = MssqlOperation.GetCommandBuilder(objAdapter);
                        break;
                    //ORCL
                    case 3:
                        strSql = string.Format("delete from T_31_022_{0} where c002 in ({1}) AND C001={2}", rentToken, listParams[0], listParams[2]);
                        optReturn = OracleOperation.ExecuteSql(session.DBConnectionString, strSql);
                        strSql = string.Format("SELECT * FROM T_31_020_{0} where c001 ={1}",rentToken,listParams[2]);
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
                objAdapter.Fill(objDataSet);
                if (objDataSet.Tables.Count <= 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_NOT_EXIST;
                    optReturn.Message = string.Format("NoExist");
                    return optReturn;
                }
                DataRow dr = objDataSet.Tables[0].Rows[0];
                dr.BeginEdit();
                dr["C008"] = listParams[1];
                dr["C021"] = listParams[4];
                if (listParams[1] == listParams[3]) { dr["C017"] = "Y"; }
                dr.EndEdit();
                objAdapter.Update(objDataSet);
                objDataSet.AcceptChanges();
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

        public OperationReturn RemoveRecord2Task(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            string strSql = "";
            try
            {
                if (listParams.Count < 6)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Param count invalid");
                    return optReturn;
                }
                DataSet objDataSet = new DataSet();
                string rentToken = session.RentInfo.Token;
                string strrecord = listParams[0];  //选中的要移动的录音
                string taskidfrom = listParams[1]; //源任务ID
                string numfrom = listParams[2];    //源任务数量修改成
                string taskidto = listParams[3];   //目标任务ID
                string numto = listParams[4];      //目标任务数量修改成
                string tempSQL = string.Empty;
                if (numfrom == listParams[5])
                {
                    tempSQL = string.Format(",C017 ='Y' ");
                }
                switch (session.DBType)
                {
                    //MSSQL
                    case 2:
                        strSql = string.Format("UPDATE T_31_022_{0} SET C001={1} WHERE C002 in({2}) AND C001={4};UPDATE T_31_020_{0} SET C008={3}{7},C021=C021-{8} WHERE C001={4};UPDATE T_31_020_{0} SET C008={5},C021=C021+{8} WHERE C001={6};"
                            , rentToken, taskidto, strrecord, numfrom, taskidfrom, numto, taskidto, tempSQL, listParams[6]);
                        optReturn = MssqlOperation.ExecuteSql(session.DBConnectionString, strSql);
                        break;
                    //ORCL
                    case 3:
                        strSql = string.Format("begin UPDATE T_31_022_{0} SET C001={1} WHERE C002 in({2})  AND C001={4};UPDATE T_31_020_{0} SET C008={3}{7},C021=C021-{8} WHERE C001={4};UPDATE T_31_020_{0} SET C008={5},C021=C021+{8} WHERE C001={6};END;"
                           , rentToken, taskidto, strrecord, numfrom, taskidfrom, numto, taskidto, tempSQL,listParams[6]);
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
            //optReturn.Message = strSql;
            return optReturn;
        }

        private OperationReturn InsertTempData(SessionInfo session, List<string> listParams)
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
                //ListParams
                //0         tempID
                //1         count
                //2..       tempData(tempData property split by char 27, less than 5)
                string strTempID = listParams[0];
                string strCount = listParams[1];
                int intCount;
                if (!int.TryParse(strCount, out intCount) || intCount <= 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Resource count invalid");
                    return optReturn;
                }
                if (listParams.Count < 2 + intCount)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Resource count invalid");
                    return optReturn;
                }
                if (string.IsNullOrEmpty(strTempID))
                {
                    List<string> listGetSerialIDParams = new List<string>();
                    listGetSerialIDParams.Add("11");
                    listGetSerialIDParams.Add("911");
                    listGetSerialIDParams.Add(DateTime.Now.ToString("yyyyMMddHHmmss"));
                    OperationReturn getSerialIDReturn = GetSerialID(session, listGetSerialIDParams);
                    if (!getSerialIDReturn.Result)
                    {
                        return getSerialIDReturn;
                    }
                    strTempID = getSerialIDReturn.Data.ToString();
                }
                string strSql;
                IDbConnection objConn;
                IDbDataAdapter objAdapter;
                DbCommandBuilder objCmdBuilder;
                switch (session.DBType)
                {
                    //MSSQL
                    case 2:
                        strSql = string.Format("select * from t_00_901 where c001 = {0}", strTempID);
                        objConn = MssqlOperation.GetConnection(session.DBConnectionString);
                        objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = MssqlOperation.GetCommandBuilder(objAdapter);
                        break;
                    //ORCL
                    case 3:
                        strSql = string.Format("select * from t_00_901 where c001 = {0}", strTempID);
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
                    DataSet objDataSet = new DataSet();
                    objAdapter.Fill(objDataSet);

                    int number = 0;
                    for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                    {
                        number = Math.Max(number, Convert.ToInt32(objDataSet.Tables[0].Rows[i]["C002"]));
                    }
                    for (int i = 2; i < listParams.Count; i++)
                    {
                        DataRow dr = objDataSet.Tables[0].NewRow();
                        string strTempData = listParams[i];
                        string[] arrTempData = strTempData.Split(new[] { ConstValue.SPLITER_CHAR },
                            StringSplitOptions.RemoveEmptyEntries);
                        dr["C001"] = strTempID;
                        dr["C002"] = number + i - 1;
                        if (arrTempData.Length > 0)
                        {
                            dr["C011"] = arrTempData[0];
                        }
                        if (arrTempData.Length > 1)
                        {
                            dr["C012"] = arrTempData[1];
                        }
                        if (arrTempData.Length > 2)
                        {
                            dr["C013"] = arrTempData[2];
                        }
                        if (arrTempData.Length > 3)
                        {
                            dr["C014"] = arrTempData[3];
                        }
                        if (arrTempData.Length > 4)
                        {
                            dr["C015"] = arrTempData[4];
                        }
                        objDataSet.Tables[0].Rows.Add(dr);
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

                optReturn.Data = strTempID;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }

        private OperationReturn GetSerialID(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParams
                //0     模块编码
                //1     模块内编码
                //2     时间变量
                if (listParams == null || listParams.Count < 3)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string moduleID = listParams[0];
                string resourceID = listParams[1];
                string dateFormat = listParams[2];
                string rentToken = session.RentInfo.Token;
                string strSerialID = string.Empty;
                long errNumber = 0;
                string strErrMsg = string.Empty;
                switch (session.DBType)
                {
                    case 2:
                        DbParameter[] mssqlParameters =
                        {
                            MssqlOperation.GetDbParameter("@AInParam01",MssqlDataType.Varchar,2),
                            MssqlOperation.GetDbParameter("@AInParam02",MssqlDataType.Varchar,3),
                            MssqlOperation.GetDbParameter("@AInParam03",MssqlDataType.Varchar,5),
                            MssqlOperation.GetDbParameter("@Ainparam04",MssqlDataType.Varchar,20),
                            MssqlOperation.GetDbParameter("@AOutParam01",MssqlDataType.Varchar,20),
                            MssqlOperation.GetDbParameter("@AOutErrorNumber",MssqlDataType.Bigint,0),
                            MssqlOperation.GetDbParameter("@AOutErrorString",MssqlDataType.NVarchar,4000)
                        };
                        mssqlParameters[0].Value = moduleID;
                        mssqlParameters[1].Value = resourceID;
                        mssqlParameters[2].Value = rentToken;
                        mssqlParameters[3].Value = dateFormat;
                        mssqlParameters[4].Value = strSerialID;
                        mssqlParameters[5].Value = errNumber;
                        mssqlParameters[6].Value = strErrMsg;
                        mssqlParameters[4].Direction = ParameterDirection.Output;
                        mssqlParameters[5].Direction = ParameterDirection.Output;
                        mssqlParameters[6].Direction = ParameterDirection.Output;
                        optReturn = MssqlOperation.ExecuteStoredProcedure(session.DBConnectionString, "P_00_001",
                           mssqlParameters);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        if (mssqlParameters[5].Value.ToString() != "0")
                        {
                            optReturn.Result = false;
                            optReturn.Code = Defines.RET_DBACCESS_FAIL;
                            optReturn.Message = string.Format("{0}\t{1}", mssqlParameters[5].Value, mssqlParameters[6].Value);
                        }
                        else
                        {
                            strSerialID = mssqlParameters[4].Value.ToString();
                            optReturn.Data = strSerialID;
                        }
                        break;
                    case 3:
                        DbParameter[] orclParameters =
                        {
                            OracleOperation.GetDbParameter("AInParam01",OracleDataType.Varchar2,2),
                            OracleOperation.GetDbParameter("AInParam02",OracleDataType.Varchar2,3),
                            OracleOperation.GetDbParameter("AInParam03",OracleDataType.Varchar2,5),
                            OracleOperation.GetDbParameter("Ainparam04",OracleDataType.Varchar2,20),
                            OracleOperation.GetDbParameter("AOutParam01",OracleDataType.Varchar2,20),
                            OracleOperation.GetDbParameter("AOutErrorNumber",OracleDataType.Int32,0),
                            OracleOperation.GetDbParameter("AOutErrorString",OracleDataType.Nvarchar2,4000)
                        };
                        orclParameters[0].Value = moduleID;
                        orclParameters[1].Value = resourceID;
                        orclParameters[2].Value = rentToken;
                        orclParameters[3].Value = dateFormat;
                        orclParameters[4].Value = strSerialID;
                        orclParameters[5].Value = errNumber;
                        orclParameters[6].Value = strErrMsg;
                        orclParameters[4].Direction = ParameterDirection.Output;
                        orclParameters[5].Direction = ParameterDirection.Output;
                        orclParameters[6].Direction = ParameterDirection.Output;
                        optReturn = OracleOperation.ExecuteStoredProcedure(session.DBConnectionString, "P_00_001",
                           orclParameters);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        if (orclParameters[5].Value.ToString() != "0")
                        {
                            optReturn.Result = false;
                            optReturn.Code = Defines.RET_DBACCESS_FAIL;
                            optReturn.Message = string.Format("{0}\t{1}", orclParameters[5].Value, orclParameters[6].Value);
                        }
                        else
                        {
                            strSerialID = orclParameters[4].Value.ToString();
                            optReturn.Data = strSerialID;
                        }
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

        private OperationReturn SaveScoreDataResult(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0     评分表成绩信息
                //1     评分表信息
                //2     录音记录信息
                //3     部门ID
                //4     坐席ID
                //5     评分类型
                //6     用户ID
                //7     评分花费时间
                if (listParams == null || listParams.Count < 5)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strScoreSheetResult = listParams[0];
                string strScoreSheet = listParams[1];
                string strRecordInfo = listParams[2];
                string strOrgID = listParams[3];
                string strAgentID = listParams[4];
                int ScoreType = Convert.ToInt32(listParams[5]);
                string ScoreUser = listParams[6];
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
                optReturn = XMLHelper.DeserializeObject<ScoreSheet>(strScoreSheet);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                ScoreSheet scoreSheet = optReturn.Data as ScoreSheet;
                if (scoreSheet == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("ScoreSheet is null");
                    return optReturn;
                }
                optReturn = XMLHelper.DeserializeObject<RecordInfo>(strRecordInfo);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                RecordInfo recordInfo = optReturn.Data as RecordInfo;
                if (recordInfo == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("RecordInfo is null");
                    return optReturn;
                }
                string strResultID = scoreSheetResult.ScoreResultID.ToString();
                string strOldResultID = scoreSheetResult.OldScoreResultID.ToString();
                string rentToken = session.RentInfo.Token;
                string strSql;
                IDbConnection objConn;
                IDbDataAdapter objAdapter;
                DbCommandBuilder objCmdBuilder;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT * FROM T_31_041_{0} WHERE C001 = {1} OR C001 = {2}",
                            rentToken,
                            strResultID,
                            strOldResultID);
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
                        strSql = string.Format("SELECT * FROM T_31_041_{0} WHERE C001 = {1} OR C001 = {2}",
                            rentToken,
                            strResultID,
                            strOldResultID);
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
                try
                {
                    DataSet objDataSet = new DataSet();
                    objAdapter.Fill(objDataSet);
                    DataRow drAdd;
                    drAdd = objDataSet.Tables[0].NewRow();
                    drAdd["C000"] = scoreSheetResult.ScoreSheetID;
                    drAdd["C001"] = Convert.ToInt64(strResultID);
                    drAdd["C002"] = scoreSheetResult.RecordSerialID;
                    drAdd["C003"] = Convert.ToInt64(DateTime.Now.ToUniversalTime().ToString("yyyyMMddHHmmss"));
                    drAdd["C004"] = ScoreUser;
                    drAdd["C005"] = "1";
                    if (strOldResultID == "0")//新增分数
                    {
                        drAdd["C006"] = ScoreType;
                    }
                    else//修改分数
                    {
                        drAdd["C006"] = ScoreType + 1;
                    }
                    drAdd["C007"] = "0";
                    drAdd["C008"] = 0;
                    drAdd["C009"] = "0";
                    drAdd["C010"] = 0;
                    drAdd["C011"] = 0;
                    drAdd["C012"] = string.Empty;
                    drAdd["C013"] = Convert.ToInt32(listParams[7]);
                    drAdd["C014"] = Converter.Second2Time(Convert.ToInt64(listParams[7]));
                    drAdd["C015"] = Convert.ToInt64(strOrgID);
                    drAdd["C016"] = 0;
                    drAdd["C017"] = strAgentID;
                    drAdd["C051"] = recordInfo.SerialID;
                    drAdd["C052"] = Convert.ToInt64(recordInfo.StartRecordTime.ToUniversalTime().ToString("yyyyMMddHHmmss"));
                    drAdd["C053"] = Convert.ToInt64(recordInfo.StopRecordTime.ToUniversalTime().ToString("yyyyMMddHHmmss"));
                    drAdd["C054"] = recordInfo.Duration;
                    drAdd["C055"] = Converter.Second2Time(recordInfo.Duration);
                    drAdd["C056"] = recordInfo.Agent;
                    drAdd["C057"] = recordInfo.Extension;
                    drAdd["C058"] = DecryptFromClient(recordInfo.CallerID);
                    drAdd["C059"] = DecryptFromClient(recordInfo.CalledID);
                    drAdd["C060"] = recordInfo.Direction;
                    drAdd["C061"] = recordInfo.Extension;
                    drAdd["C062"] = recordInfo.ChannelName;
                    drAdd["C063"] = string.Empty;

                    int scale = 10000;
                    drAdd["C099"] = scale;
                    drAdd["C100"] = (int)(scoreSheetResult.Score * scale);
                    List<ScoreItem> listItems = new List<ScoreItem>();
                    scoreSheet.GetAllScoreItem(ref listItems);
                    listItems = listItems.OrderBy(s => s.ItemID).ToList();
                    int count = listItems.Count;
                    for (int i = 0; i < count; i++)
                    {
                        int itemID = listItems[i].ItemID;
                        drAdd[string.Format("C{0}", (itemID + 100).ToString("000"))] = (int)(listItems[i].Score * scale);
                    }
                    objDataSet.Tables[0].Rows.Add(drAdd);
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
                optReturn.Data = strResultID;
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

        #region 複檢

        private OperationReturn GetRecheckRecordData(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0      查询语句
                if (listParams == null || listParams.Count < 3)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }

                if (!IsExitRecordTable(session, listParams[1]))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_FAIL;
                    optReturn.Message = S3103Consts.Err_TableNotExit;
                    return optReturn;
                }
                List<long> listTemp = new List<long>();
                if(!String.IsNullOrWhiteSpace(listParams[2]))
                {
                    OperationReturn optReturn1 = GetQueryListData(session,listParams[1],listParams[2]);  
                    if(!optReturn1.Result)
                    {
                        return optReturn1;
                    }
                    listTemp = optReturn1.Data as List<long>;
                    if (listTemp == null)
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_OBJECT_NULL;
                        optReturn.Message = string.Format("DataSet is null");
                        return optReturn;
                    }
                }

                string strQueryString = listParams[0];
                string strSql = strQueryString;
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
                bool IsAdd;
                for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                {
                    IsAdd = true;
                    DataRow dr = objDataSet.Tables[0].Rows[i];
                    RecordInfo item = new RecordInfo();
                    item.RowID = Convert.ToInt64(dr["C001"]);
                    item.SerialID = Convert.ToInt64(dr["C002"]);
                    if(!string.IsNullOrWhiteSpace(listParams[2]))//不为空--不允许查出查询评过分的录音
                    {
                       for(int j=0;j<listTemp.Count();j++)
                       {
                           if(item.SerialID==listTemp[j])
                           {
                               IsAdd = false;
                           }
                       }
                    }
                    item.RecordReference = dr["C077"].ToString();
                    item.StartRecordTime = Convert.ToDateTime(dr["C005"]).ToLocalTime();
                    item.StopRecordTime = Convert.ToDateTime(dr["C009"]).ToLocalTime();
                    item.VoiceIP = dr["C020"].ToString();
                    item.VoiceID = Convert.ToInt32(dr["C037"]);
                    item.ChannelID = Convert.ToInt32(dr["C038"]);
                    item.Extension = dr["C042"].ToString();
                    item.Agent = dr["C039"].ToString();
                    item.ReaExtension = dr["C058"].ToString();
                    item.Duration = Convert.ToInt32(dr["C012"]);
                    item.Direction = dr["C045"].ToString() == "1" ? 1 : 0;
                    item.CallerID = EncryptToClient(dr["C040"].ToString());
                    item.CalledID = EncryptToClient(dr["C041"].ToString());
                    item.MediaType = Convert.ToInt32(dr["C014"]);
                    item.EncryptFlag = dr["C025"].ToString();
                    if (listParams.Count == 2)//任务分配的查询
                        item.IsScore = dr["ISSCORE"].ToString();
                    else
                        item.IsScore = null;
                    item.TaskUserID = dr["TaskUserID"].ToString();
                    item.TaskUserName = DecryptNamesFromDB(dr["UserAccount"].ToString());
                    optReturn = XMLHelper.SeriallizeObject(item);
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                    if (IsAdd)
                    {
                        listReturn.Add(optReturn.Data.ToString());
                    }
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

        private OperationReturn GetQueryListData(SessionInfo session, string TableName, string Str)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                List<long> listTemp = new List<long>();
                string NoQueryScoreStr = string.Format("select distinct T38.C002 from T_31_008_{0} T38 left join {1} T21 on T21.C002=T38.C002 where {2} and T38.C010 in (1,2)", session.RentInfo.Token, TableName, Str);
                DataSet NQDataSet = null; 
                switch (session.DBType)
                {
                    case 2:
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, NoQueryScoreStr);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        NQDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, NoQueryScoreStr);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        NQDataSet = optReturn.Data as DataSet;
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("Database type not surpport.\t{0}", session.DBType);
                        return optReturn;
                }
                if (NQDataSet == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("DataSet is null");
                    return optReturn;
                }
                for (int i = 0; i < NQDataSet.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = NQDataSet.Tables[0].Rows[i];
                    listTemp.Add(Convert.ToInt64(dr["C002"]));
                }
                optReturn.Data = listTemp;
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


        #endregion

        private OperationReturn GetABCD(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0      ABCD的统计ID
                if (listParams == null || listParams.Count < 1)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strSql = string.Format("SELECT T352.*,T106.C002 AS OrgName FROM T_31_052_{1} T352,T_11_006_{1} T106 WHERE T352.C002={0} AND T106.C001=T352.C003",listParams[0],session.RentInfo.Token);
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
                    ABCD_OrgSkillGroup item = new ABCD_OrgSkillGroup();
                    item.OrgSkillGroupID = long.Parse(dr["C003"].ToString());
                    item.ParamID = long.Parse(dr["C002"].ToString());
                    item.InColumn = int.Parse(dr["C008"].ToString());
                    item.OrgSkillGroupName = DecryptNamesFromDB(dr["OrgName"].ToString());
                    item.Isdrilling = Convert.ToInt32(dr["C004"].ToString()) == 2 ? true : false;
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
                strSql = string.Format("Select C001,C011,C017,C018 from T_11_101_{0} where C001>=1030000000000000001 And C001<1060000000000000001 AND C002='1'", rentToken);
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

        private OperationReturn GetScoreTemplateID(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                string rentToken = session.RentInfo.Token;
                string strSql = string.Format("SELECT * FROM ( SELECT T308.*, ROW_NUMBER () OVER ( PARTITION BY C003 ORDER BY C006 DESC ) AS SID FROM "+
                    "( SELECT * FROM T_31_008_{0} WHERE C002 = '{1}' AND C010 IN (3, 4)) T308 ) A WHERE A.SID = 1 ", rentToken,listParams[0]);
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
                DataRow dr = objDataset.Tables[0].Rows[0];
                optReturn.Data = dr["C003"].ToString();
                optReturn.Message = strSql;
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

        private OperationReturn GetRecordHistoryOpt(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                string result = string.Empty;
                string rentToken = session.RentInfo.Token;
                string strSql = string.Format("SELECT * FROM T_31_008_{0} T308 WHERE T308.C002='{1}' AND T308.C014 in(1,2)", rentToken, listParams[0]);
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
                if (objDataset.Tables[0].Rows.Count > 0) { result = string.Format("A"); }

                if (listParams[1] == "1" || listParams[1] == "2")//初检任务录音，需要查出是否被分配复检
                {
                    strSql = string.Format("SELECT * FROM T_31_022_00000 T322 WHERE T322.C002='{1}' AND T322.C010 in(3,4)", rentToken, listParams[0]);
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
                    if (objDataset.Tables[0].Rows.Count > 0) { result += string.Format(",T"); }
                }
                optReturn.Data = result;
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

        private OperationReturn GetCtrolQA(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                string result = string.Empty;
                string rentToken = session.RentInfo.Token;
                string strSql = string.Format("SELECT * FROM T_11_005_{0} WHERE C001 IN(SELECT C004 FROM T_11_201_{0} WHERE C003='{1}' AND C004 LIKE'102%')AND C007 <> 'H'", rentToken, listParams[0]);
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
                List<string> listOpts = new List<string>();
                for (int i = 0; i < objDataset.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = objDataset.Tables[0].Rows[i];
                    CtrolQA ctrolqa = new CtrolQA();
                    ctrolqa.UserID = dr["C001"].ToString();
                    ctrolqa.UserName = DecryptNamesFromDB(dr["C002"].ToString());
                    ctrolqa.UserFullName = DecryptNamesFromDB(dr["C003"].ToString());
                    ctrolqa.OrgID = dr["C006"].ToString();
                    optReturn = XMLHelper.SeriallizeObject(ctrolqa);
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
                optReturn.Message = ex.Message + ex.Source + ex.StackTrace;
                return optReturn;
            }
            return optReturn;
        }


        #region 数据处理
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