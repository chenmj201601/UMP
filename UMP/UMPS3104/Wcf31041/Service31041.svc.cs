using System;
using System.Linq;
using System.Collections.Generic;
using System.Data.Common;
using System.ServiceModel.Activation;
using System.ServiceModel.Channels;
using PFShareClassesS;
using VoiceCyber.Common;
using VoiceCyber.UMP.CommonService03;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Common31041;
using System.Data;
using VoiceCyber.UMP.ScoreSheets;
using System.Data.SqlClient;
using Oracle.DataAccess.Client;
using VoiceCyber.UMP.Common31041.Common3102;

namespace Wcf31041
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码、svc 和配置文件中的类名“Service1”。
    // 注意: 为了启动 WCF 测试客户端以测试此服务，请在解决方案资源管理器中选择 Service1.svc 或 Service1.svc.cs，然后开始调试。
     [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public partial class Service31041 : IService31041
    {
         public WebReturn UMPClientOperation(WebRequest webRequest)
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
                    case (int)S3104Codes.WritePlayHistory:
                        optReturn = WritePlayHistory(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        break;
                    case(int)S3104Codes.GetPlayHistory:
                        optReturn = GetPlayHistory(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S3104Codes.WriteAppeal:
                        optReturn = WriteAppeal(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        break;
                    case(int)S3104Codes.GetOwerAppeal:
                        optReturn = GetOwerAppeal(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S3104Codes.AgentLoginValidate:
                        optReturn = AgentLoginValidate(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        string stringData = optReturn.StringValue;
                        webReturn.Data = stringData;
                        break;
                    case (int)S3104Codes.GetRoleOperationList:
                        optReturn = GetRoleOperationList(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S3104Codes.InsertTempData:
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
                    case (int)S3104Codes.GetControlOrgInfoList:
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
                    case (int)S3104Codes.GetRecordData:
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
                    case (int)S3104Codes.GetScoreDate:
                        optReturn = GetScoreDate(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S3104Codes.GetAuInfoList:
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
                    case(int)S3104Codes.GetSerialID:
                        optReturn = GetSerialID(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData.Add(optReturn.Data.ToString());
                        break;
                    case (int)S3104Codes.GetControledOrganizationList:
                        optReturn = GetControledOrganizationList(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S3104Codes.GetControledUserList:
                        optReturn = GetControledUserList(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S3104Codes.GetScoreSheetInfo:
                        optReturn = GetScoreSheetInfo(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.Data = optReturn.Data.ToString();
                        break;
                    case (int)S3104Codes.GetScoreResultList:
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
                    case (int)S3104Codes.GetRecordFile:
                        optReturn = GetRecordFile(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.Data = optReturn.Data.ToString();
                        break;
                    case (int)S3104Codes.GetSftpServerList:
                        optReturn = GetSftpServerList(session);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S3104Codes.GetRelativeRecordList:
                        optReturn = GetRelativeRecordList(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S3104Codes.GetDownloadParamList:
                        optReturn = GetDownloadParamList(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S3104Codes.GetFolder:
                        optReturn = GetFolder(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S3104Codes.GetFiles:
                        optReturn = GetFiles(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S3104Codes.GetScoreCommentResultList:
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
                    case (int)S3104Codes.GetABCD:
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
                    case (int)S3104Codes.WriteBrowseHistory:
                        optReturn = WriteBrowseHistory(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        break;
                    case (int)S3104Codes.GetExamInfo:
                        optReturn = GetExamInfo(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.Data = optReturn.Data.ToString();
                        break;
                    case (int)S3104Codes.GetIMRole:
                        optReturn = GetIMRole(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.Result = optReturn.Result;
                        webReturn.Message = optReturn.Message;
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
                 if (!IsExistRecordTable(session, listParams[1]))
                 {
                     optReturn.Result = true;
                     optReturn.Message = S3104Consts.Err_TableNotExit;
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
                 string temp = string.Empty;
                 for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                 {
                     DataRow dr = objDataSet.Tables[0].Rows[i];
                     RecordInfo item = new RecordInfo();
                     item.RowID = Convert.ToInt64(dr["C001"]);
                     item.SerialID = Convert.ToInt64(dr["C002"]);
                     item.RecordReference = dr["C077"].ToString();
                     item.StartRecordTime = Convert.ToDateTime(dr["C005"]).ToLocalTime();
                     item.StopRecordTime = Convert.ToDateTime(dr["C009"]).ToLocalTime();
                     item.VoiceID = Convert.ToInt32(dr["C037"]);
                     item.VoiceIP = dr["C020"].ToString();
                     item.ChannelID = Convert.ToInt32(dr["C038"]);
                     item.Extension = dr["C042"].ToString();
                     item.Agent = dr["C039"].ToString();
                     item.Duration = Convert.ToInt32(dr["C012"]);
                     item.Direction = dr["C045"].ToString() == "1" ? "1" : "0";
                     item.CallerID = EncryptToClient(dr["C040"].ToString());
                     item.CalledID = EncryptToClient(dr["C041"].ToString());
                     item.WaveFormat = dr["C015"].ToString();
                     item.MediaType = Convert.ToInt32(dr["C014"]);
                     item.EncryptFlag = dr["C025"].ToString();
                     if (strSql.Contains("SCOREID"))//为了能够适用其它的查询
                     {
                         item.ScoreID = Int64Parse(dr["SCOREID"].ToString(), 0);
                         item.Score = dr["SCORE"].ToString();
                         item.TemplateID = Int64Parse(dr["TEMPLATEID"].ToString(), 0);
                         temp = dr["AppealState"].ToString();
                         if (!string.IsNullOrWhiteSpace(temp))
                         {
                             if(temp=="1"||temp=="2"||temp=="4")
                             {
                                 item.AppealMark = "1";
                             }
                             else if (temp == "3" || temp == "5" || temp == "6" || temp == "7" )
                             {
                                 item.AppealMark = "2";
                             }
                             else { item.AppealMark = "0"; }
                         }
                         else
                         {
                             item.AppealMark = "0";
                         }
                         item.BookMark = dr["BOOKMARK"].ToString() == "1" ? "3104T00100" : "3104T00130";
                     }

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
                 optReturn.Message = ex.Message+"\t"+ex.StackTrace;
                 return optReturn;
             }
             return optReturn;
         }

         private bool IsExistRecordTable(SessionInfo session, string tablename)
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

         private OperationReturn GetScoreDate(SessionInfo session, List<string> listParams)
         {
             OperationReturn optReturn = new OperationReturn();
             optReturn.Result = true;
             optReturn.Code = 0;
             try
             {
                 //ListParam
                 //0      錄音流水號T_21_001_00000.C002
                 if (listParams == null || listParams.Count < 1)
                 {
                     optReturn.Result = false;
                     optReturn.Code = Defines.RET_PARAM_INVALID;
                     optReturn.Message = string.Format("Request param is null or count invalid");
                     return optReturn;
                 }
                 string strQuery = string.Format("Select T308.C006,T308.C004,T308.C009,T308.C005,T308.C003,T308.C014,T308.C010," +
                                                 "T308.C001,T308.C002,T301.C002 as TemplateName from T_31_008_{0} T308 LEFT JOIN T_31_001_{0} T301 " +
                                                 "ON T308.C003=T301.C001 where T308.C002='{1}' AND T308.C009 = 'Y' order by T308.C003,T308.C014", session.RentInfo.Token, listParams[0]);
                 DataSet objDataSet;
                 switch (session.DBType)
                 {
                     case 2:
                         optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strQuery);
                         if (!optReturn.Result)
                         {
                             return optReturn;
                         }
                         objDataSet = optReturn.Data as DataSet;
                         break;
                     case 3:
                         optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strQuery);
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
                     RecordScoreInfo item=new RecordScoreInfo();
                     item.ScoreID = Int64Parse(dr["C001"].ToString(), 0);
                     item.ScoreTime = Convert.ToDateTime(dr["C006"].ToString());
                     item.Score = Convert.ToDouble(dr["C004"].ToString());
                     item.IsFinal = dr["C009"].ToString().ToUpper()=="Y"?"1":"0";
                     item.ScorePerson = Int64Parse(dr["C005"].ToString(), 0);
                     item.TemplateID = Int64Parse(dr["C003"].ToString(), 0);
                     item.TemplateName = dr["TemplateName"].ToString();
                     item.AppealMark = dr["C014"].ToString();
                     item.ScoreType = dr["C010"].ToString();
                     optReturn = XMLHelper.SeriallizeObject(item);
                     if (!optReturn.Result)
                     {
                         return optReturn;
                     }
                     listReturn.Add(optReturn.Data.ToString());
                 }
                 optReturn.Data = listReturn;
                 optReturn.Message = strQuery;
             }
             catch (Exception ex)
             {
                 optReturn.Result = false;
                 optReturn.Code = Defines.RET_FAIL;
                 optReturn.Message = ex.Message + "\t" + ex.StackTrace;
                 return optReturn;
             }
             return optReturn;
         }

         private OperationReturn GetOwerAppeal(SessionInfo session, List<string> listParams)
         {
             OperationReturn optReturn = new OperationReturn();
             optReturn.Result = true;
             optReturn.Code = 0;
             try
             {
                 //ListParam
                 //0      C003
                 //1      C002
                 if (listParams == null || listParams.Count <2)
                 {
                     optReturn.Result = false;
                     optReturn.Code = Defines.RET_PARAM_INVALID;
                     optReturn.Message = string.Format("Request param is null or count invalid");
                     return optReturn;
                 }
                 string strQueryString = string.Format("SELECT * FROM T_31_008_{0} WHERE C002={1} AND C003={2} AND C014>0 AND C015>1", session.RentInfo.Token, listParams[0], listParams[1]);
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
                     optReturn.Message = S3104Consts.AppealOvered;
                     return optReturn;
                 }
                 DataRow dr = objDataSet.Tables[0].Rows[0];//在只能申诉一次的情况下
                 temp = Convert.ToInt64(dr["C001"]);
                 strQueryString = string.Format("select * from T_31_019_{0} where C003={1} And C002={2}", session.RentInfo.Token, listParams[0], temp);
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
                     optReturn.Message=S3104Consts.AppealOvered;
                     return optReturn;
                 }
                 dr = objDataSet.Tables[0].Rows[0];
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
                 optReturn.Message = ex.Message+ex.Source+ex.StackTrace;
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
                 if (listParams == null || listParams.Count < 1)
                 {
                     optReturn.Result = false;
                     optReturn.Code = Defines.RET_PARAM_INVALID;
                     optReturn.Message = string.Format("Request param is null or count invalid");
                     return optReturn;
                 }
                 string strParentID = listParams[0];
                 string rentToken = session.RentInfo.Token;
                 string strSql=string.Empty;
                 DataSet objDataSet;
                 switch (session.DBType)
                 {
                     case 2:
                             strSql = string.Format("SELECT * FROM T_11_006_{0} WHERE C001 ={1}"
                            , rentToken                           
                            , strParentID);
                        
                         optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                         if (!optReturn.Result)
                         {
                             return optReturn;
                         }
                         objDataSet = optReturn.Data as DataSet;
                         break;
                     case 3:
                         strSql = string.Format("SELECT * FROM T_11_006_{0} WHERE C001 ={1}"
                            , rentToken
                            , strParentID);
                         
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
                     strName = DecryptFromDB(strName);
                     string strInfo = string.Format("{0}{1}{2}", strID, ConstValue.SPLITER_CHAR, strName);
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
         
         private OperationReturn GetSftpServerList(SessionInfo session)
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
                         strSql =
                             string.Format(
                                 "SELECT * FROM T_11_101_{0} WHERE C001 > 2190000000000000000 AND C001 < 2200000000000000000 ORDER BY C001,C002",
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
                             string.Format(
                                 "SELECT * FROM T_11_101_{0} WHERE C001 > 2190000000000000000 AND C001 < 2200000000000000000 ORDER BY C001,C002",
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
                 List<string> listReturn = new List<string>();
                 int intValue;
                 for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                 {
                     DataRow dr = objDataSet.Tables[0].Rows[i];
                     long id = Convert.ToInt64(dr["C001"]);
                     int row = Convert.ToInt32(dr["C002"]);
                     if (row == 1)
                     {
                         string strIP = dr["C017"].ToString();
                         strIP = DecryptResourcePropertyValue(strIP);
                         string strPort = dr["C019"].ToString();
                         strPort = DecryptResourcePropertyValue(strPort);
                         if (int.TryParse(strPort, out intValue))
                         {
                             SftpServerInfo item = new SftpServerInfo();
                             item.ObjID = id;
                             item.HostAddress = strIP;
                             item.HostPort = intValue;
                             optReturn = XMLHelper.SeriallizeObject(item);
                             if (!optReturn.Result)
                             {
                                 return optReturn;
                             }
                             listReturn.Add(optReturn.Data.ToString());
                         }
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

        private OperationReturn AgentLoginValidate(SessionInfo session,List<string> listparams) //登录验证
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0      用户编号
                if (listparams == null || listparams.Count < 1)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                //账号加密
                string strUserID =EncryptToDB( listparams[0]);
                string rentToken = session.RentInfo.Token;
                string strSql;
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        strSql =
                            string.Format(
                                "SELECT * FROM T_11_101_{0} WHERE C017 = '{1}' "
                                , rentToken
                                , strUserID);
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
                                "SELECT * FROM T_11_101_{0} WHERE C017 = '{1}' "
                                , rentToken
                                , strUserID);
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
                if (objDataSet.Tables[0].Rows.Count == 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("UserAccount Is Not Wrong");
                }
                else 
                {
                    DataRow dr = objDataSet.Tables[0].Rows[0];
                    string strID = dr["C001"].ToString();
                    string strOrgID = dr["C011"].ToString();
                    string strName = dr["C017"].ToString();
                    string strFullName = dr["C018"].ToString();
                    //账号解密
                    strName = DecryptFromDB(strName);
                    strFullName = DecryptFromDB(strFullName);
                    optReturn.StringValue = strOrgID + ConstValue.SPLITER_CHAR + strName;
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
                if (listParams == null || listParams.Count !=1)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string moduleID = listParams[0];
                string rentToken = session.RentInfo.Token;
                string strSql;
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        strSql =
                            string.Format(
                                "SELECT * FROM T_11_003_{0} WHERE C003={1} ORDER BY  C004",
                                rentToken,
                                moduleID);
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
                                "SELECT * FROM T_11_003_{0} WHERE C003={1} ORDER BY  C004",
                                rentToken,
                                moduleID
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

        private OperationReturn GetIMRole(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParams
                if (listParams == null || listParams.Count <=0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string moduleID = listParams[0];
                string rentToken = session.RentInfo.Token;
                string strSql;
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        strSql =
                            string.Format(
                                "SELECT * FROM T_11_003_{0} WHERE C002={1} ",
                                rentToken,
                                moduleID);
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
                                "SELECT * FROM T_11_003_{0} WHERE C002={1}",
                                rentToken,
                                moduleID
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
                if (objDataSet.Tables[0].Rows.Count >=1)
                {
                    optReturn.Result = true;
                }
                else
                {
                    optReturn.Result = false;
                }
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
         /// 评分表信息
         /// </summary>
         /// <param name="session"></param>
         /// <param name="listParams"></param>
         /// <returns></returns>
        private OperationReturn GetScoreSheetInfo(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0      评分表编码
                if (listParams == null || listParams.Count < 2)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strScoreSheetID = listParams[0];
                string strScoreID = listParams[1];
                optReturn = LoadScoreSheet(session, strScoreSheetID, strScoreID);
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
                optReturn = XMLHelper.SeriallizeObject(scoreSheet);
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
         /// 评分结果
         /// </summary>
         /// <param name="session"></param>
         /// <param name="listParams"></param>
         /// <returns></returns>
        private OperationReturn GetScoreResultList(SessionInfo session, List<string> listParams)
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
                        strSql = string.Format("SELECT * FROM T_31_009_{0} WHERE C003 = {1}",
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
                        strSql = string.Format("SELECT * FROM T_31_009_{0} WHERE C003 = {1}",
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
                optReturn.Message = strSql;
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
                    item.Score = Convert.ToDouble(dr["C004"]);
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
            return optReturn;
        }


         /// <summary>
         /// 写入播放历史
         /// </summary>
         /// <param name="session"></param>
         /// <param name="listParams"></param>
         /// <returns></returns>
        private OperationReturn WritePlayHistory(SessionInfo session, List<string> listParams)
         {
             OperationReturn optReturn = new OperationReturn();
             optReturn.Result = true;
             optReturn.Code = 0;
            try
            {

                //ListParam
                //C001 播放历史ID，主键
                //C002 录音流水号,T_21_000.C002 录音记录表的ID
                //C003 播放人ID，UserID
                //C004 播放日期
                //C005 播放时长
                //C006 1
                //C007 播放次数
                //C008 开始时间点
                //C009 结束时间点
                string C001 = listParams[0];
                string C002 = listParams[1];
                string C003 = listParams[2];
                string C004 = listParams[3];
                string C005 = listParams[4];
                string C006 = listParams[5];
                string C007 = listParams[6];
                string C008 = listParams[7];
                string C009 = listParams[8];
                string strSerialID = string.Empty;//返回流水号ID   
                long errNumber = 0;
                string strErrMsg = string.Empty;
                //查询相同C002、C003、C006的记录是否存在，存在更新C004,C005,C007+1,C008,C009. 不存在新建一条数据
                bool isUpdate = false;//是否需要更新，如果存在相同记录更新true，新建为false
                string strSQL = string.Format("select * from T_31_038_{0} where C002={1}", session.RentInfo.Token, listParams[1]);
                IDbConnection objConn;
                IDbDataAdapter objAdapter;
                DbCommandBuilder objCmdBuilder;
                switch (session.DBType)
                {
                    case 2:
                        objConn = MssqlOperation.GetConnection(session.DBConnectionString);
                        objAdapter = MssqlOperation.GetDataAdapter(objConn, strSQL);
                        objCmdBuilder = MssqlOperation.GetCommandBuilder(objAdapter);
                        break;
                    case 3:
                        objConn = OracleOperation.GetConnection(session.DBConnectionString);
                        objAdapter = OracleOperation.GetDataAdapter(objConn, strSQL);
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
                    int temp = 0, C7 = 98;
                    for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
                    {
                        if (dataSet.Tables[0].Rows[i][1].ToString() == listParams[1] && 
                            dataSet.Tables[0].Rows[i][2].ToString() == listParams[2] &&
                            dataSet.Tables[0].Rows[i][5].ToString() == listParams[5])
                        {
                            C7 = Convert.ToInt32(dataSet.Tables[0].Rows[i][6].ToString());
                            isUpdate = true;
                            temp = i;
                            break;
                        }
                    }
                    if (isUpdate)
                    {
                        DataRow dr = dataSet.Tables[0].Rows[temp];
                        dr.BeginEdit();
                        dr["C004"] = listParams[3];
                        dr["C005"] = listParams[4];
                        dr["C007"] = C7 + 1;
                        dr["C008"] = listParams[7];
                        dr["C009"] = listParams[8];
                        dr.EndEdit();
                        objAdapter.Update(dataSet);
                        dataSet.AcceptChanges();
                    }
                    else
                    {
                        //新建
                        switch (session.DBType)
                        {
                            case 2:
                                DbParameter[] msqlParameters =
                                {
                                    MssqlOperation.GetDbParameter("@AInParam01", MssqlDataType.Varchar, 25),
                                    MssqlOperation.GetDbParameter("@AInParam02", MssqlDataType.Varchar, 25),
                                    MssqlOperation.GetDbParameter("@AInParam03", MssqlDataType.Varchar, 25),
                                    MssqlOperation.GetDbParameter("@AInParam04", MssqlDataType.Varchar, 25),
                                    MssqlOperation.GetDbParameter("@AInParam05", MssqlDataType.Varchar, 25),
                                    MssqlOperation.GetDbParameter("@AInParam06", MssqlDataType.Varchar, 2),
                                    MssqlOperation.GetDbParameter("@AInParam07", MssqlDataType.Varchar, 25),
                                    MssqlOperation.GetDbParameter("@AInParam08", MssqlDataType.Varchar, 25),
                                    MssqlOperation.GetDbParameter("@AInParam09", MssqlDataType.Varchar, 25),
                                    MssqlOperation.GetDbParameter("@AOutParam01", MssqlDataType.Varchar, 25),
                                    MssqlOperation.GetDbParameter("@AOutErrornumber", MssqlDataType.Bigint, 0),
                                    MssqlOperation.GetDbParameter("@AOutErrorstring", MssqlDataType.NVarchar, 4000)
                                };
                                msqlParameters[0].Value = session.RentInfo.Token; //租户标识
                                msqlParameters[1].Value = C002;
                                msqlParameters[2].Value = C003;
                                msqlParameters[3].Value = C004;
                                msqlParameters[4].Value = C005;
                                msqlParameters[5].Value = C006;
                                msqlParameters[6].Value = C007;
                                msqlParameters[7].Value = C008;
                                msqlParameters[8].Value = C009;
                                msqlParameters[9].Value = strSerialID;
                                msqlParameters[10].Value = errNumber;
                                msqlParameters[11].Value = strErrMsg;
                                msqlParameters[9].Direction = ParameterDirection.Output;
                                msqlParameters[10].Direction = ParameterDirection.Output;
                                msqlParameters[11].Direction = ParameterDirection.Output;
                                optReturn = MssqlOperation.ExecuteStoredProcedure(session.DBConnectionString, "P_31_005",
                                    msqlParameters);
                                if (!optReturn.Result)
                                {
                                    return optReturn;
                                }
                                if (msqlParameters[10].Value.ToString() != "0")
                                {
                                    optReturn.Result = false;
                                    optReturn.Code = Defines.RET_DBACCESS_FAIL;
                                    optReturn.Message = string.Format("{0}\t{1}", msqlParameters[10].Value,
                                        msqlParameters[11].Value);
                                }
                                else
                                {
                                    strSerialID = msqlParameters[9].Value.ToString();
                                }
                                break;
                            case 3:
                                DbParameter[] oraclParameters =
                                {
                                    OracleOperation.GetDbParameter("@AInParam01", OracleDataType.Varchar2, 25),
                                    OracleOperation.GetDbParameter("@AInParam02", OracleDataType.Varchar2, 25),
                                    OracleOperation.GetDbParameter("@AInParam03", OracleDataType.Varchar2, 25),
                                    OracleOperation.GetDbParameter("@AInParam04", OracleDataType.Varchar2, 25),
                                    OracleOperation.GetDbParameter("@AInParam05", OracleDataType.Varchar2, 25),
                                    OracleOperation.GetDbParameter("@AInParam06", OracleDataType.Varchar2, 2),
                                    OracleOperation.GetDbParameter("@AInParam07", OracleDataType.Varchar2, 25),
                                    OracleOperation.GetDbParameter("@AInParam08", OracleDataType.Varchar2, 25),
                                    OracleOperation.GetDbParameter("@AInParam09", OracleDataType.Varchar2, 25),
                                    OracleOperation.GetDbParameter("@AOutParam01", OracleDataType.Varchar2, 25),
                                    OracleOperation.GetDbParameter("@AOutErrornumber", OracleDataType.Int32, 0),
                                    OracleOperation.GetDbParameter("@AOutErrorstring", OracleDataType.Nvarchar2, 4000)
                                };
                                oraclParameters[0].Value = session.RentInfo.Token; //租户标识
                                oraclParameters[1].Value = C002;
                                oraclParameters[2].Value = C003;
                                oraclParameters[3].Value = C004;
                                oraclParameters[4].Value = C005;
                                oraclParameters[5].Value = C006;
                                oraclParameters[6].Value = C007;
                                oraclParameters[7].Value = C008;
                                oraclParameters[8].Value = C009;
                                oraclParameters[9].Value = strSerialID;
                                oraclParameters[10].Value = errNumber;
                                oraclParameters[11].Value = strErrMsg;
                                oraclParameters[9].Direction = ParameterDirection.Output;
                                oraclParameters[10].Direction = ParameterDirection.Output;
                                oraclParameters[11].Direction = ParameterDirection.Output;
                                optReturn = OracleOperation.ExecuteStoredProcedure(session.DBConnectionString,
                                    "P_31_005",
                                    oraclParameters);
                                if (!optReturn.Result)
                                {
                                    return optReturn;
                                }
                                if (oraclParameters[10].Value.ToString() != "0")
                                {
                                    optReturn.Result = false;
                                    optReturn.Code = Defines.RET_DBACCESS_FAIL;
                                    optReturn.Message = string.Format("{0}\t{1}", oraclParameters[10].Value,
                                        oraclParameters[11].Value);
                                }
                                else
                                {
                                    strSerialID = oraclParameters[9].Value.ToString();
                                }
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
                finally
                {
                    if (objConn.State == ConnectionState.Open)
                    {
                        objConn.Close();
                    }
                    objConn.Dispose();
                }
                
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

         /// <summary>
         /// 获取播放历史清单
         /// </summary>
         /// <param name="session"></param>
         /// <param name="listParams"></param>
         /// <returns></returns>
        private OperationReturn GetPlayHistory(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0     座席ID
                if (listParams == null || listParams.Count < 1)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strSQL ;
                DataSet objDataSet;
                List<string> listReturn=new List<string>();
                switch (session.DBType)
                {
                    case 2:
                        strSQL = string.Format("select Top 180 * from T_31_038_{0} where C003={1} order by C004 DESC", session.RentInfo.Token, listParams[0]);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSQL);
                         if (!optReturn.Result)
                         {
                             return optReturn;
                         }
                         objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        strSQL = string.Format("select * from T_31_038_{0} where C003={1} and rownum <=180 order by C004 DESC", session.RentInfo.Token, listParams[0]);
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSQL);
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
                    RecordPlayHistoryInfo item=new RecordPlayHistoryInfo();
                    item.RecordReference = Convert.ToInt64(dr["C002"]);
                    item.longUserID = Convert.ToInt64(dr["C003"]);
                    item.PlayDate = Convert.ToDateTime(dr["C004"]).ToLocalTime();
                    item.PlayDuration = Convert.ToInt32(dr["C005"]);
                    item.intType = Convert.ToInt32(dr["C006"]);
                    item.Type = item.intType == 1 ? "3104T00138" : "3104T00139";
                    item.PlayTimes = Convert.ToInt32(dr["C007"]);
                    item.StartPosition = Convert.ToInt32(dr["C008"]);
                    item.StopPosition = Convert.ToInt32(dr["C009"]);
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
                optReturn.Message = ex.Message + ex.Source + ex.StackTrace;
                return optReturn;
            }
            return optReturn;
        }

         /// <summary>
         /// 写申诉
         /// </summary>
         /// <param name="session"></param>
         /// <param name="listParams"></param>
         /// <returns></returns>
        private OperationReturn WriteAppeal(SessionInfo session, List<string> listParams)
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
                //ListParam
                //0     C001 详情表主键  P_31_004获取P_00_001的serialID
                //1     C002 T_31_008.C001评分成绩表的成绩ID
                //2     C003 T_21_000.C002 录音记录表的ID
                //3     C004 录音所属座席工号对应的资源编号
                //4     C005  申诉人ID,如果是座席自己申诉的
                //5     C006  申诉流程ID
                //6     C007 申诉流程子项ID（通过ActionID+AppealFlowItemID得到该流程走到那一步了）
                //7     C008 1为申诉，2为审批，3为复核
                //8     C009 对于申诉(1座席申诉，2他人替申)，对于审批(3审批驳回,4审批过通)，对于复核（5复核通过不修改分数，6复核通过重新评分，7复核驳回） ,主要用于通过ActionID+AppealFlowItemID得到该流程走到那一步了
                //9     C010 Y为申诉流程完毕，N 在处理流程中
                //10   C011 当能多次申诉时启用，每再申诉一次+1，客户端第一版写1
                //11    C012 创建时间(UTC)
                //12    申诉备注
                //13    評分來源
                string C001 = listParams[0];
                string C002 = listParams[1];
                string C003 = listParams[2];
                string C004 = listParams[3];
                string C005 = listParams[4];
                string C006 = listParams[5];
                string C007 = listParams[6];
                string C008 = listParams[7];
                string C009 = listParams[8];
                string C010 = listParams[9];
                string C011 = listParams[10];
                string C012 = listParams[11];
                string appealDemo = listParams[12];
                string strSerialID = string.Empty;
                long errNumber = 0;
                string strErrMsg = string.Empty;
                switch (session.DBType)
                {
                    case 2:
                        DbParameter[] msqlParameters =
                        {
                            MssqlOperation.GetDbParameter("@AInParam01",MssqlDataType.Varchar, 5),
                            MssqlOperation.GetDbParameter("@AInParam02",MssqlDataType.Varchar,25),
                            MssqlOperation.GetDbParameter("@AInParam03",MssqlDataType.Varchar,25),
                            MssqlOperation.GetDbParameter("@AInParam04",MssqlDataType.Varchar,25),
                            MssqlOperation.GetDbParameter("@AInParam05",MssqlDataType.Varchar,25),
                            MssqlOperation.GetDbParameter("@AInParam06",MssqlDataType.Varchar,25),
                            MssqlOperation.GetDbParameter("@AInParam07",MssqlDataType.Varchar,25),
                            MssqlOperation.GetDbParameter("@AInParam08",MssqlDataType.Varchar,25),
                            MssqlOperation.GetDbParameter("@AInParam09",MssqlDataType.Varchar,2),
                            MssqlOperation.GetDbParameter("@AInParam10",MssqlDataType.Varchar,2),
                            MssqlOperation.GetDbParameter("@AInParam11",MssqlDataType.Varchar,3),
                            MssqlOperation.GetDbParameter("@AInParam12",MssqlDataType.Varchar,25),
                            MssqlOperation.GetDbParameter("@AOutParam01",MssqlDataType.Varchar,25),
                            MssqlOperation.GetDbParameter("@AOutErrornumber",MssqlDataType.Bigint,0),
                            MssqlOperation.GetDbParameter("@AOutErrorstring",MssqlDataType.NVarchar,4000)
                        };
                        msqlParameters[0].Value = session.RentInfo.Token;//租户标识
                        msqlParameters[1].Value = C002;
                        msqlParameters[2].Value = C003;
                        msqlParameters[3].Value = C004;
                        msqlParameters[4].Value = C005;
                        msqlParameters[5].Value = C006;
                        msqlParameters[6].Value = C007;
                        msqlParameters[7].Value = C008;
                        msqlParameters[8].Value = C009;
                        msqlParameters[9].Value = C010;
                        msqlParameters[10].Value = C011;
                        msqlParameters[11].Value = C012;
                        msqlParameters[12].Value = strSerialID;
                        msqlParameters[13].Value = errNumber;
                        msqlParameters[14].Value = strErrMsg;
                        msqlParameters[12].Direction = ParameterDirection.Output;
                        msqlParameters[13].Direction = ParameterDirection.Output;
                        msqlParameters[14].Direction = ParameterDirection.Output;
                        optReturn = MssqlOperation.ExecuteStoredProcedure(session.DBConnectionString, "P_31_004",
                            msqlParameters);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        if (msqlParameters[13].Value.ToString() != "0")
                        {
                            optReturn.Result = false;
                            optReturn.Code = Defines.RET_DBACCESS_FAIL;
                            optReturn.Message = string.Format("{0}\t{1}", msqlParameters[13].Value, msqlParameters[14].Value);
                        }
                        else
                        {
                            strSerialID = msqlParameters[12].Value.ToString();
                        }
                        break;
                    case 3:
                        DbParameter[] oraclParameters =
                        {
                            OracleOperation.GetDbParameter("@AInParam01",OracleDataType.Varchar2, 5),
                            OracleOperation.GetDbParameter("@AInParam02",OracleDataType.Varchar2,25),
                            OracleOperation.GetDbParameter("@AInParam03",OracleDataType.Varchar2,25),
                            OracleOperation.GetDbParameter("@AInParam04",OracleDataType.Varchar2,25),
                            OracleOperation.GetDbParameter("@AInParam05",OracleDataType.Varchar2,25),
                            OracleOperation.GetDbParameter("@AInParam06",OracleDataType.Varchar2,25),
                            OracleOperation.GetDbParameter("@AInParam07",OracleDataType.Varchar2,25),
                            OracleOperation.GetDbParameter("@AInParam08",OracleDataType.Varchar2,25),
                            OracleOperation.GetDbParameter("@AInParam09",OracleDataType.Varchar2,2),
                            OracleOperation.GetDbParameter("@AInParam10",OracleDataType.Varchar2,2),
                            OracleOperation.GetDbParameter("@AInParam11",OracleDataType.Varchar2,3),
                            OracleOperation.GetDbParameter("@AInParam12",OracleDataType.Varchar2,25),
                            OracleOperation.GetDbParameter("@AOutParam01",OracleDataType.Varchar2,25),
                            OracleOperation.GetDbParameter("@AOutErrornumber",OracleDataType.Int32,0),
                            OracleOperation.GetDbParameter("@AOutErrorstring",OracleDataType.Nvarchar2,4000)
                        };
                        oraclParameters[0].Value = session.RentInfo.Token;//租户标识
                        oraclParameters[1].Value = C002;
                        oraclParameters[2].Value = C003;
                        oraclParameters[3].Value = C004;
                        oraclParameters[4].Value = C005;
                        oraclParameters[5].Value = C006;
                        oraclParameters[6].Value = C007;
                        oraclParameters[7].Value = C008;
                        oraclParameters[8].Value = C009;
                        oraclParameters[9].Value = C010;
                        oraclParameters[10].Value = C011;
                        oraclParameters[11].Value = C012;
                        oraclParameters[12].Value = strSerialID;
                        oraclParameters[13].Value = errNumber;
                        oraclParameters[14].Value = strErrMsg;
                        oraclParameters[12].Direction = ParameterDirection.Output;
                        oraclParameters[13].Direction = ParameterDirection.Output;
                        oraclParameters[14].Direction = ParameterDirection.Output;
                        optReturn = OracleOperation.ExecuteStoredProcedure(session.DBConnectionString, "P_31_004",
                            oraclParameters);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        if (oraclParameters[13].Value.ToString() != "0")
                        {
                            optReturn.Result = false;
                            optReturn.Code = Defines.RET_DBACCESS_FAIL;
                            optReturn.Message = string.Format("{0}\t{1}", oraclParameters[13].Value, oraclParameters[14].Value);
                        }
                        else
                        {
                            strSerialID = oraclParameters[12].Value.ToString();
                        }
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("Database type not surpport.\t{0}", session.DBType);
                        return optReturn;
                }

                //  T_31_047
                string strSql1 = string.Format("select * from T_31_047_{0} where 1<>1", session.RentInfo.Token);
                IDbConnection objConn;
                IDbDataAdapter objAdapter;
                DbCommandBuilder objCmdBuilder;
                switch (session.DBType)
                {
                    case 2:
                        objConn = MssqlOperation.GetConnection(session.DBConnectionString);
                        objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql1);
                        objCmdBuilder = MssqlOperation.GetCommandBuilder(objAdapter);
                        break;
                    case 3:
                        objConn = OracleOperation.GetConnection(session.DBConnectionString);
                        objAdapter = OracleOperation.GetDataAdapter(objConn, strSql1);
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
                    dr["C001"] = strSerialID;//详情表主键
                    dr["C002"] = listParams[5];//申诉流程ID
                    dr["C003"] = listParams[6];//申诉流程子项ID
                    dr["C004"] = appealDemo;//申诉备注
                    dr["C005"] = "1";//对于申诉(1座席申诉，2他人替申)暂写1
                    dr["C006"] = listParams[4];//操作人ID
                    dr["C007"] = listParams[11];//操作人
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
                //更新T_31_008
                string strSql2 = string.Format("select * from T_31_008_{0} where C001={1}", session.RentInfo.Token,listParams[1]);
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
                    dr1["C014"] = "1";
                    dr1["C015"] = listParams[6];//申诉流程子项ID
                    dr1.EndEdit();
                    objAdapter.Update(dataSet1);
                    dataSet1.AcceptChanges();
                }
                catch (Exception ex)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_FAIL;
                    optReturn.Message = ex.Message + ex.Source + ex.StackTrace;
                    return optReturn;
                }
                //更新T_31_041
                string strSql3 = string.Format("select * from T_31_041_{0} where C002={1} AND C001={2}", session.RentInfo.Token,listParams[2], listParams[1]);
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
                    DataRow dr2 = dataSet2.Tables[0].Rows[0];
                    dr2.BeginEdit();
                    dr2["C007"] = "1";
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
                finally
                {
                    if (objConn.State == ConnectionState.Open)
                    {
                        objConn.Close();
                    }
                    objConn.Dispose();
                }
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

        private OperationReturn GetAuInfoList(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                string rentToken = session.RentInfo.Token;
                string strSql = string.Format("select C001,C002,C003 from T_11_005_{0} ", rentToken);
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
                    auInfoList.Name = EncryptToClient(DecryptFromDB(dr["C002"].ToString()));
                    auInfoList.FullName = EncryptToClient(DecryptFromDB(dr["C003"].ToString()));
                    optReturn = XMLHelper.SeriallizeObject(auInfoList);
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                    listReturn.Add(optReturn.Data.ToString());
                }
                strSql = string.Format("Select C001,C017,C018 from T_11_101_{0} where C001>=1030000000000000001 And C001<1040000000000000001 AND C002='1'", rentToken);
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
                    auInfoList.Name = EncryptToClient(DecryptFromDB(dr["C017"].ToString()));
                    auInfoList.FullName = EncryptToClient(DecryptFromDB(dr["C018"].ToString()));
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
                optReturn.Message = ex.Message+ex.Source+ex.StackTrace;
                return optReturn;
            }
            return optReturn;
        }

        private OperationReturn GetControledOrganizationList(SessionInfo session, List<string> listParams)
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
                            strSql = string.Format("SELECT  A.*  FROM  T_11_006_{0} A , T_11_005_{0} B WHERE  A.C001 = B.C006 AND B.C001 = {1}"
                                , rentToken
                                , userID);
                        }
                        else
                        {
                            strSql = string.Format("SELECT A.*  FROM  T_11_006_{0} A, T_11_201_{0} B WHERE A.C001 = B.C004 AND A.C004 = {1} AND B.C003 = {2}",
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
                            strSql = string.Format("SELECT A.* FROM  T_11_006_{0} A, T_11_005_{0} B WHERE  A.C001 = B.C006 AND B.C001 = {1}"
                                , rentToken
                                , userID);
                        }
                        else
                        {
                            strSql = string.Format("SELECT  A.*  FROM  T_11_006_{0} A, T_11_201_{0} B WHERE A.C001 = B.C004 AND A.C004 = {1} AND B.C003 = {2}",
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

                    OrganizationInfo orgInfo = new OrganizationInfo();
                    orgInfo.OrgID = Convert.ToInt64(dr["C001"]);
                    orgInfo.OrgName = DecryptFromDB(dr["C002"].ToString());
                    //DecryptFromDB(dr["C002"].ToString());
                    orgInfo.OrgType = Convert.ToInt32(dr["C003"]);
                    orgInfo.ParentID = Convert.ToInt64(dr["C004"]);
                    orgInfo.IsActived = dr["C005"].ToString();
                    orgInfo.IsDeleted = dr["C006"].ToString();
                    orgInfo.State = dr["C007"].ToString();
                    orgInfo.StrStartTime = DecryptFromDB(dr["C008"].ToString());
                    orgInfo.StrEndTime = DecryptFromDB(dr["C009"].ToString());
                    //DecryptFromDB(dr["C009"].ToString());
                    orgInfo.Creator = Convert.ToInt64(dr["C010"]);
                    orgInfo.CreateTime = Convert.ToDateTime(dr["C011"]).ToLocalTime();
                    orgInfo.Description = dr["C012"].ToString();
                    optReturn = XMLHelper.SeriallizeObject(orgInfo);
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                    listOrgs.Add(optReturn.Data.ToString());
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

        private OperationReturn GetControledUserList(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                if (listParams.Count != 2)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Param count invalid");
                    return optReturn;
                }
                string rentToken = session.RentInfo.Token;
                string userID = listParams[0];
                string parentID = listParams[1];
                string strSql;
                DataSet objDataSet = null;
                switch (session.DBType)
                {
                    //MSSQL
                    case 2:
                        strSql = string.Format("SELECT  * FROM T_11_005_{0} WHERE C006 = {1} AND C001 IN (SELECT  C004 FROM T_11_201_{0} WHERE C003 = {2}   )",
                            rentToken,
                            parentID,
                            userID);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    //ORCL
                    case 3:
                        strSql = string.Format("SELECT  * FROM T_11_005_{0} WHERE C006 = {1} AND  C001 IN  (SELECT  C004 FROM  T_11_201_{0} WHERE C003 = {2}   )",
                            rentToken,
                            parentID,
                            userID);
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
                List<string> listUsers = new List<string>();
                for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = objDataSet.Tables[0].Rows[i];
                    BasicUserInfo userInfo = new BasicUserInfo();
                    userInfo.UserID = Convert.ToInt64(dr["C001"]);
                    userInfo.Account = DecryptFromDB(dr["C002"].ToString());
                    userInfo.FullName = DecryptFromDB(dr["C003"].ToString());
                    userInfo.OrgID = Convert.ToInt64(dr["C006"]);
                    userInfo.SourceFlag = dr["C007"].ToString();
                    userInfo.IsLocked = dr["C008"].ToString();
                    userInfo.LockMethod = dr["C009"].ToString();
                    userInfo.StrStartTime = DecryptFromDB(dr["C017"].ToString());
                    userInfo.StrEndTime = DecryptFromDB(dr["C018"].ToString());
                    userInfo.IsActived = dr["C010"].ToString();
                    userInfo.IsDeleted = dr["C011"].ToString();
                    userInfo.State = dr["C012"].ToString();
                    userInfo.Creator = Convert.ToInt64(dr["C019"]);
                    userInfo.StrCreateTime = DecryptFromDB(dr["C020"].ToString());
                    optReturn = XMLHelper.SeriallizeObject(userInfo);
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                    listUsers.Add(optReturn.Data.ToString());
                }
                optReturn.Data = listUsers;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }

        private OperationReturn GetRecordFile(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0     SFTP服务器地址
                //1     SFTP服务器端口
                //2     录音流水号
                //3     录音编号
                //4     分表信息
                if (listParams == null || listParams.Count < 5)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strAddress = listParams[0];
                string strPort = listParams[1];
                string strReference = listParams[2];
                string strNo = listParams[3];
                string strPartition = listParams[4];
                int intValue;
                if (!int.TryParse(strPort, out intValue))
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Port param  invalid");
                    return optReturn;
                }
                int intPort = intValue;
                Service03Helper helper = new Service03Helper();
                helper.HostAddress = session.AppServerInfo.Address;
                if (session.AppServerInfo.SupportHttps)
                {
                    helper.HostPort = session.AppServerInfo.Port - 4;
                }
                else
                {
                    helper.HostPort = session.AppServerInfo.Port - 3;
                }
                RequestMessage request = new RequestMessage();
                request.Command = (int)Service03Command.DownloadRecordFile;
                request.ListData.Add(strAddress);
                request.ListData.Add(intPort.ToString());
                request.ListData.Add(string.Format("{0}|{1}", session.UserID, session.RentInfo.Token));
                request.ListData.Add(session.UserInfo.Password);
                request.ListData.Add(strNo);
                request.ListData.Add(strReference);
                request.ListData.Add(strPartition);
                optReturn = helper.DoRequest(request);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                ReturnMessage retMessage = optReturn.Data as ReturnMessage;
                if (retMessage == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("ReturnMessage is null");
                    return optReturn;
                }
                if (!retMessage.Result)
                {
                    optReturn.Result = false;
                    optReturn.Code = retMessage.Code;
                    optReturn.Message = retMessage.Message;
                    return optReturn;
                }
                optReturn.Data = retMessage.Data;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }
         
        private OperationReturn GetRelativeRecordList(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0      录音记录信息（RecordInfo）
                if (listParams == null || listParams.Count < 1)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strRecordInfo = listParams[0];
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
                long serialID = recordInfo.SerialID;
                DateTime startRecordTime = recordInfo.StartRecordTime;
                string rentToken = session.RentInfo.Token;
                DataSet objDataSet;
                int errNum = 0;
                string errMsg = string.Empty;
                switch (session.DBType)
                {
                    case 2:
                        DbParameter[] mssqlParameters =
                        {
                            MssqlOperation.GetDbParameter("@ainparam01", MssqlDataType.Varchar, 5),
                            MssqlOperation.GetDbParameter("@ainparam02", MssqlDataType.Varchar, 20),
                            MssqlOperation.GetDbParameter("@ainparam03", MssqlDataType.Varchar, 20),
                            MssqlOperation.GetDbParameter("@ainparam04", MssqlDataType.Varchar, 2000),
                            MssqlOperation.GetDbParameter("@aouterrornumber", MssqlDataType.Bigint, 0),
                            MssqlOperation.GetDbParameter("@aouterrorstring", MssqlDataType.Varchar, 1024)
                        };
                        mssqlParameters[0].Value = rentToken;
                        mssqlParameters[1].Value = startRecordTime.ToString("yyyy-MM-dd HH:mm:ss");
                        mssqlParameters[2].Value = serialID.ToString();
                        mssqlParameters[3].Value = string.Empty;
                        mssqlParameters[4].Value = errNum;
                        mssqlParameters[5].Value = errMsg;
                        mssqlParameters[4].Direction = ParameterDirection.Output;
                        mssqlParameters[5].Direction = ParameterDirection.Output;
                        optReturn = MssqlOperation.GetDataSetFromStoredProcedure(session.DBConnectionString, "P_21_002",
                           mssqlParameters);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        if (mssqlParameters[4].Value.ToString() != "0")
                        {
                            optReturn.Result = false;
                            optReturn.Code = Defines.RET_DBACCESS_FAIL;
                            optReturn.Message = string.Format("{0}\t{1}", mssqlParameters[4].Value, mssqlParameters[5].Value);
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        DbParameter[] orcalParameters =
                        {
                            OracleOperation.GetDbParameter("ainparam01", OracleDataType.Varchar2, 5),
                            OracleOperation.GetDbParameter("ainparam02", OracleDataType.Varchar2, 20),
                            OracleOperation.GetDbParameter("ainparam03", OracleDataType.Varchar2, 20),
                            OracleOperation.GetDbParameter("ainparam04", OracleDataType.Varchar2, 1024),
                            OracleOperation.GetDbParameter("result",OracleDataType.RefCursor,0),
                            OracleOperation.GetDbParameter("aouterrornumber", OracleDataType.Int32, 0),
                            OracleOperation.GetDbParameter("aouterrorstring", OracleDataType.Varchar2, 1024)
                        };
                        orcalParameters[0].Value = rentToken;
                        orcalParameters[1].Value = startRecordTime.ToString("yyyy-MM-dd HH:mm:ss");
                        orcalParameters[2].Value = serialID.ToString();
                        orcalParameters[3].Value = string.Empty;
                        orcalParameters[4].Value = null;
                        orcalParameters[5].Value = errNum;
                        orcalParameters[6].Value = errMsg;
                        orcalParameters[4].Direction = ParameterDirection.Output;
                        orcalParameters[5].Direction = ParameterDirection.Output;
                        orcalParameters[6].Direction = ParameterDirection.Output;
                        optReturn = OracleOperation.GetDataSetFromStoredProcedure(session.DBConnectionString, "P_21_002",
                           orcalParameters);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        if (orcalParameters[5].Value.ToString() != "0")
                        {
                            optReturn.Result = false;
                            optReturn.Code = Defines.RET_DBACCESS_FAIL;
                            optReturn.Message = string.Format("{0}\t{1}", orcalParameters[5].Value, orcalParameters[6].Value);
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
                    item.StartRecordTime = Convert.ToDateTime(dr["C005"]);
                    item.StopRecordTime = Convert.ToDateTime(dr["C009"]);
                    item.VoiceID = Convert.ToInt32(dr["C037"]);
                    item.VoiceIP = dr["C020"].ToString();
                    item.ChannelID = Convert.ToInt32(dr["C038"]);
                    item.Extension = dr["C042"].ToString();
                    item.Agent = dr["C039"].ToString();
                    item.Duration = Convert.ToInt32(dr["C012"]);
                    item.Direction = dr["C045"].ToString() == "1" ? "1" : "0";
                    item.CallerID = EncryptToClient(dr["C040"].ToString());
                    item.CalledID = EncryptToClient(dr["C041"].ToString());
                    item.WaveFormat = dr["C015"].ToString();
                    item.MediaType = Convert.ToInt32(dr["C014"]);
                    item.EncryptFlag = dr["C025"].ToString();
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

        private OperationReturn GetDownloadParamList(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParam
                //0     用户编码
                if (listParams == null || listParams.Count < 1)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strUserID = listParams[0];
                string rentToken = session.RentInfo.Token;
                string strSql;
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        strSql =
                            string.Format(
                                "SELECT * FROM T_11_101_{0} WHERE C001 > 2910000000000000000 AND C001 < 2920000000000000000 ORDER BY C001,C002",
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
                            string.Format(
                                "SELECT * FROM T_11_101_{0} WHERE C001 > 2910000000000000000 AND C001 < 2920000000000000000 ORDER BY C001,C002",
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
                List<string> listReturn = new List<string>();
                int intValue;
                List<DownloadParamInfo> listItems = new List<DownloadParamInfo>();
                for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = objDataSet.Tables[0].Rows[i];
                    long objID = Convert.ToInt64(dr["C001"]);
                    var temp = listItems.FirstOrDefault(d => d.ObjID == objID);
                    if (temp == null)
                    {
                        temp = new DownloadParamInfo();
                        temp.ObjID = objID;
                        listItems.Add(temp);
                    }
                    int row = Convert.ToInt32(dr["C002"]);
                    if (row == 1)
                    {
                        string strID = dr["C012"].ToString();
                        if (int.TryParse(strID, out intValue))
                        {
                            temp.ID = intValue;
                        }
                        temp.IsEnabled = dr["C016"].ToString() == "1";
                    }
                    if (row == 2)
                    {
                        string strMethod = dr["C011"].ToString();
                        if (int.TryParse(strMethod, out intValue))
                        {
                            temp.Method = intValue;
                        }
                        string strVoiceID = dr["C012"].ToString();
                        if (int.TryParse(strVoiceID, out intValue))
                        {
                            temp.VoiceID = intValue;
                        }
                        temp.Address = DecryptResourcePropertyValue(dr["C013"].ToString());
                        string strPort = DecryptResourcePropertyValue(dr["C014"].ToString());
                        if (int.TryParse(strPort, out intValue))
                        {
                            temp.Port = intValue;
                        }
                        temp.RootDir = dr["C015"].ToString();
                        temp.VoiceAddress = DecryptResourcePropertyValue(dr["C016"].ToString());
                    }
                    if (row == 3)
                    {
                        string strVocPathFormat = dr["C011"].ToString();
                        temp.VocPathFormat = strVocPathFormat;
                        string strScrPathFormat = dr["C012"].ToString();
                        temp.ScrPathFormat = strScrPathFormat;
                    }
                    if (row == 92)
                    {
                        temp.UserName = DecryptResourcePropertyValue(dr["C011"].ToString());
                        temp.Password = DecryptResourcePropertyValue(dr["C012"].ToString());
                    }
                }
                for (int i = 0; i < listItems.Count; i++)
                {
                    var item = listItems[i];
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

        /// <summary>
        /// 获取DB中所有文件夹
        /// </summary>
        private OperationReturn GetFolder(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                if (listParams == null || listParams.Count < 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                DataSet objDataSet;
                string strSql = string.Format("SELECT * FROM T_31_058_{0} ", session.RentInfo.Token);
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
                    FolderInfo Info = new FolderInfo();
                    Info.FolderID = Convert.ToInt64(dr["C001"]);
                    Info.TreeParentID = Convert.ToInt64(dr["C002"]);
                    Info.FolderName = dr["C003"].ToString();
                    Info.TreeParentName = dr["C004"].ToString();
                    Info.CreatorId = Convert.ToInt64(dr["C005"]);
                    Info.CreatorName = dr["C006"].ToString();
                    Info.CreatorTime = Convert.ToDateTime(dr["C007"]).ToLocalTime().ToString("yyyy/MM/dd HH:mm:ss");
                    Info.UserID1 = dr["C008"].ToString();
                    Info.UserID2 = dr["C009"].ToString();
                    Info.UserID3 = dr["C010"].ToString();

                    optReturn = XMLHelper.SeriallizeObject(Info);
                    if (!optReturn.Result)
                    {
                        optReturn.Code = i;
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

        /// <summary>
        /// 获取该目录下所有文件
        /// </summary>
        private OperationReturn GetFiles(SessionInfo session, List<string> listParams)
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
                DataSet objDataSet;
                string strSql = string.Format("SELECT * FROM T_31_060_{0} WHERE C002={1}", session.RentInfo.Token, listParams[0]);
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
                    FilesItemInfo Info = new FilesItemInfo();
                    Info.FileID = Convert.ToInt64(dr["C001"]);
                    Info.FolderID = Convert.ToInt64(dr["C002"]);
                    Info.FileName = dr["C003"].ToString();
                    Info.FilePath = dr["C004"].ToString();
                    Info.FileDescription = dr["C005"].ToString();
                    Info.FromType = dr["C006"].ToString();
                    Info.IsEncrytp = dr["C007"].ToString();
                    Info.FileType = dr["C008"].ToString();
                    optReturn = XMLHelper.SeriallizeObject(Info);
                    if (!optReturn.Result)
                    {
                        optReturn.Code = i;
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
                string strSql = string.Format("SELECT T352.*,T106.C002 AS OrgName FROM T_31_052_{1} T352,T_11_006_{1} T106 WHERE T352.C002={0} AND T106.C001=T352.C003", listParams[0],session.RentInfo.Token);
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
                    item.OrgSkillGroupName = DecryptFromDB(dr["OrgName"].ToString());
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

        /// <summary>
        /// 写入浏览历史
        /// </summary>
        private OperationReturn WriteBrowseHistory(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;

            optReturn = XMLHelper.DeserializeObject<FilesItemInfo>(listParams[0]);
            if (!optReturn.Result)
            {
                return optReturn;
            }
            FilesItemInfo bookInfo = optReturn.Data as FilesItemInfo;
            if (bookInfo == null)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_OBJECT_NULL;
                optReturn.Message = string.Format("BrowseInfo Is Null");
                return optReturn;
            }
            string rentToken = session.RentInfo.Token;
            string strSql;
            IDbConnection objConn;
            IDbDataAdapter objAdapter;
            DbCommandBuilder objCmdBuilder;
            strSql = string.Format("SELECT * FROM T_31_059_{0} WHERE C001 = {1} AND C002={2}",rentToken, bookInfo.FileID,listParams[1]);
            switch (session.DBType)
            {
                case 2:
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
            if (objConn == null || objAdapter == null ||objCmdBuilder == null)
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
                    dr["C005"] = Convert.ToInt32(dr["C005"])+1;
                    dr["C006"] =DateTime.UtcNow;
                    dr.EndEdit();
                }
                else
                {
                    DataRow dr = objDataSet.Tables[0].NewRow();
                    dr["C001"] = bookInfo.FileID;
                    dr["C002"] = listParams[1];
                    dr["C003"] = bookInfo.FileName;
                    dr["C004"] =listParams[2];
                    dr["C005"] = 1;
                    dr["C006"] = DateTime.UtcNow;
                    objDataSet.Tables[0].Rows.Add(dr);
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

        private OperationReturn GetExamInfo(SessionInfo session, List<string> listParams)//获取试卷信息
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
                string strQueryString = string.Format("SELECT T26.* FROM T_11_201_{0} T21 LEFT JOIN T_36_026_{0} T26 ON T21.C004=T26.C001 WHERE T21.C003='{1}' AND T26.C009='N' AND T26.C002='{1}'", session.RentInfo.Token, listParams[0]);
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
                    optReturn.Message = S3104Consts.AppealOvered;
                    return optReturn;
                }
                DataRow dr = objDataSet.Tables[0].Rows[0];
                strQueryString = string.Format("Select * from T_36_024_{0} where C003={1} And C002={2}", session.RentInfo.Token, listParams[0]);
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
                    optReturn.Message = S3104Consts.AppealOvered;
                    return optReturn;
                }
                dr = objDataSet.Tables[0].Rows[0];
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

        private string EncryptToDB(string strSource)//加密
        {
            string strReturn = EncryptionAndDecryption.EncryptDecryptString(strSource,
              CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M002),
              EncryptionAndDecryption.UMPKeyAndIVType.M002);
            return strReturn;
        }

        private string DecryptFromDB(string strSource)//解密
        {
            string strReturn = EncryptionAndDecryption.EncryptDecryptString(strSource,
              CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M102),
              EncryptionAndDecryption.UMPKeyAndIVType.M102);
            return strReturn;
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

        private string EncryptShaToDB(string strSource)
        {
            string strReturn = EncryptionAndDecryption.EncryptStringSHA512(strSource,
             CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M002),
             EncryptionAndDecryption.UMPKeyAndIVType.M002);
            return strReturn;
        }

        private string DecryptResourcePropertyValue(string source)
        {
            string strReturn = source;
            if (source.StartsWith(string.Format("{0}{0}{0}", ConstValue.SPLITER_CHAR)))
            {
                string strContent = source.Substring(3);
                string[] arrContent = strContent.Split(new[] { ConstValue.SPLITER_CHAR },
                    StringSplitOptions.RemoveEmptyEntries);
                string strVersion = string.Empty, strMode = string.Empty, strPass = string.Empty;
                if (arrContent.Length > 0)
                {
                    strVersion = arrContent[0];
                }
                if (arrContent.Length > 1)
                {
                    strMode = arrContent[1];
                }
                if (arrContent.Length > 2)
                {
                    strPass = arrContent[2];
                }
                strReturn = strPass;
                if (strVersion == "2" && strMode == "hex")
                {
                    strReturn = DecryptFromDB(strPass);
                }
            }
            return strReturn;
        }

        #endregion

        #region
        public static Int64 Int64Parse(string str, Int64 defaultValue)
        {
            Int64 outRet = defaultValue;
            if (!Int64.TryParse(str, out outRet))
            {
                outRet = defaultValue;
            }

            return outRet;
        }

        public static int IntParse(string str, int defaultValue)
        {
            int outRet = defaultValue;
            if (!int.TryParse(str, out outRet))
            {
                outRet = defaultValue;
            }

            return outRet;
        }
        #endregion

    }
}
