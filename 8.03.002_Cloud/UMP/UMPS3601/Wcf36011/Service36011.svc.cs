using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.ServiceModel.Activation;
using System.Text;
using Common3601;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Encryptions;

namespace Wcf36011
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码、svc 和配置文件中的类名“Service36011”。
    // 注意: 为了启动 WCF 测试客户端以测试此服务，请在解决方案资源管理器中选择 Service36011.svc 或 Service36011.svc.cs，然后开始调试。
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class Service36011 : IService36011
    {
        private string ParsedString(string str)
        {
            string strTemp = string.Empty;
            if (str == null)
                return str;

            string[] strArray = str.Split(new char[] { '\'' });
            for (int i = 0; i < strArray.Length; i++)
            {
                if (strArray.Length == 1)
                    return str;
                if (i == 0)
                    strTemp = strArray[i];
                else
                    strTemp += "''" + strArray[i];
            }
            return strTemp;
        }

        private readonly List<string> _mListDeleteInfo = new List<string>();

        private string _mStrUmpPath = null;

        private string StringToHexString(string s, Encoding encode)
        {
            byte[] b = encode.GetBytes(s);//按照指定编码将string编程字节数组
            string result = string.Empty;
            for (int i = 0; i < b.Length; i++)//逐字节变为16进制字符，以%隔开
            {
                result += Convert.ToString(b[i], 16);
            }
            return result;
        }

        private bool AnalyzingFieldIsNull(string strString)
        {
            if (strString == "")
                return false;
            return true;
        }

        private OperationReturn OptGetQuestinCategory(SessionInfo session, List<string> listParams)
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
                string strParam = listParams[0];
                optReturn = XMLHelper.DeserializeObject<string>(strParam);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                string strSql = optReturn.Data as string;
                if (strSql == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("PapersCategoryParam is null");
                    return optReturn;
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
                List<string> listReturn = new List<string>();
                if (objDataSet == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("DataSet is null");
                    return optReturn;
                }
                for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++ )
                {
                    DataRow dr = objDataSet.Tables[0].Rows[i];
                    PapersCategoryParam param = new PapersCategoryParam();
                    param.LongNum = Convert.ToInt64(dr["C001"]);
                    param.StrName = dr["C002"].ToString();
                    param.LongParentNodeId = Convert.ToInt64(dr["C003"]);
                    param.StrParentNodeName = dr["C004"].ToString();
                    param.LongFounderId = Convert.ToInt64(dr["C005"]);
                    param.StrFounderName = dr["C006"].ToString();
                    param.StrDateTime = Convert.ToDateTime(dr["C007"]).ToString("yyyy/MM/dd HH:mm:ss");
                    optReturn = XMLHelper.SeriallizeObject(param);
                    if (!optReturn.Result)
                    {
                        optReturn.Code = i;
                        return optReturn;
                    }
                    listReturn.Add(optReturn.Data.ToString());
                }
                optReturn.Data = listReturn;
            }
            catch(Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
                return optReturn;
            }
            return optReturn;
        }

        private OperationReturn OptAddPaperQuestions(SessionInfo session, List<string> listParams)
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
                string strPaperQuestionParam = listParams[0];
                optReturn = XMLHelper.DeserializeObject<List<CPaperQuestionParam>>(strPaperQuestionParam);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                var listeEditPapers = optReturn.Data as List<CPaperQuestionParam>;
                if (listeEditPapers == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("listeEditPapers is null");
                    return optReturn;
                }

                string rentToken = session.RentInfo.Token;
                string strSql = string.Format("SELECT * FROM T_36_024_{0}", rentToken);
                IDbConnection objConn;
                IDbDataAdapter objAdapter;
                DbCommandBuilder objCmdBuilder;
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
                if (objConn == null || objAdapter == null || objCmdBuilder == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("Db object is null");
                    return optReturn;
                }

                foreach (var editPaper in listeEditPapers)
                {
                    objCmdBuilder.ConflictOption = ConflictOption.OverwriteChanges;
                    objCmdBuilder.SetAllValues = false;
                    DataSet objDataSet = new DataSet();
                    objAdapter.Fill(objDataSet);
                    DataRow dr = objDataSet.Tables[0].NewRow();
                    dr["c001"] = editPaper.LongPaperNum;
                    dr["c002"] = editPaper.LongQuestionNum;
                    dr["c003"] = editPaper.IntQuestionType;
                    dr["c004"] = editPaper.StrQuestions;
                    dr["c005"] = editPaper.EnableChange;
                    dr["c007"] = editPaper.StrAnswerA;
                    dr["c008"] = editPaper.StrAnswerB;
                    dr["c009"] = editPaper.StrAnswerC;
                    dr["c010"] = editPaper.StrAnswerD;
                    dr["c011"] = editPaper.StrAnswerE;
                    dr["c012"] = editPaper.StrAnswerF;
                    dr["c013"] = editPaper.CorrectAnswerOne;
                    dr["c014"] = editPaper.CorrectAnswerTwo;
                    dr["c015"] = editPaper.CorrectAnswerThree;
                    dr["c016"] = editPaper.CorrectAnswerFour;
                    dr["c017"] = editPaper.CorrectAnswerFive;
                    dr["c018"] = editPaper.CorrectAnswerSix;
                    dr["c019"] = editPaper.IntScore;
                    dr["c021"] = editPaper.StrAccessoryType;
                    dr["c022"] = editPaper.StrAccessoryName;
                    dr["c023"] = editPaper.StrAccessoryPath;
                    dr["c024"] = editPaper.StrQuestionCategory;
                    objDataSet.Tables[0].Rows.Add(dr);
                    objAdapter.Update(objDataSet);
                    objDataSet.AcceptChanges();        
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

        private OperationReturn OptGetQuestions(SessionInfo session, List<string> listParams)
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
                string strParam = listParams[0];
                optReturn = XMLHelper.DeserializeObject<string>(strParam);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                string strSql = optReturn.Data as string;
                if (strSql == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("PapersCategoryParam is null");
                    return optReturn;
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
                List<string> listReturn = new List<string>();
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
                    CQuestionsParam param = new CQuestionsParam();
                    param.LongNum = Convert.ToInt64(dr["C001"]);
                    param.LongCategoryNum = Convert.ToInt64(dr["C002"]);
                    param.StrCategoryName = dr["C003"].ToString();
                    param.IntType = Convert.ToInt32(dr["C004"]);
                    param.StrQuestionsContect = dr["C005"].ToString();
                    param.StrShortAnswerAnswer = dr["C006"].ToString();
                    param.StrAnswerOne = dr["C007"].ToString();
                    param.StrAnswerTwo = dr["C008"].ToString();
                    param.StrAnswerThree = dr["C009"].ToString();
                    param.StrAnswerFour = dr["C010"].ToString();
                    param.StrAnswerFive = dr["C011"].ToString();
                    param.StrAnswerSix = dr["C012"].ToString();
                    param.CorrectAnswerOne = dr["C013"].ToString();
                    param.CorrectAnswerTwo = dr["C014"].ToString();
                    param.CorrectAnswerThree = dr["C015"].ToString();
                    param.CorrectAnswerFour = dr["C016"].ToString();
                    param.CorrectAnswerFive = dr["C017"].ToString();
                    param.CorrectAnswerSix = dr["C018"].ToString();
                    param.IntUseNumber = Convert.ToInt32(dr["C019"]);
                    param.StrAccessoryType = dr["C020"].ToString();
                    param.StrAccessoryName = dr["C021"].ToString();
                    param.StrAccessoryPath = dr["C022"].ToString();
                    param.LongFounderId = Convert.ToInt64(dr["C023"]);
                    param.StrFounderName = dr["C024"].ToString();
                    param.StrDateTime = Convert.ToDateTime(dr["C025"]).ToString("yyyy/MM/dd HH:mm:ss");
                    optReturn = XMLHelper.SeriallizeObject(param);
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

        private OperationReturn OptAddQuestion(SessionInfo session, List<string> listParams)
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
                string strCategoryParam = listParams[0];
                optReturn = XMLHelper.DeserializeObject<CQuestionsParam>(strCategoryParam);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                CQuestionsParam cExamQuestions = optReturn.Data as CQuestionsParam;
                if (cExamQuestions == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("PapersCategoryParam is null");
                    return optReturn;
                }
                string rentToken = session.RentInfo.Token;
                string strSql = string.Format("SELECT * FROM T_36_022_{0}", rentToken);
                IDbConnection objConn;
                IDbDataAdapter objAdapter;
                DbCommandBuilder objCmdBuilder;
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
                DataRow dr = objDataSet.Tables[0].NewRow();
                dr["c001"] = cExamQuestions.LongNum;
                dr["c002"] = cExamQuestions.LongCategoryNum;
                dr["c003"] = cExamQuestions.StrCategoryName;
                dr["c004"] = cExamQuestions.IntType;
                dr["c005"] = cExamQuestions.StrQuestionsContect;
                dr["c006"] = cExamQuestions.StrShortAnswerAnswer;
                dr["c007"] = cExamQuestions.StrAnswerOne;
                dr["c008"] = cExamQuestions.StrAnswerTwo;
                dr["c009"] = cExamQuestions.StrAnswerThree;
                dr["c010"] = cExamQuestions.StrAnswerFour;
                dr["c011"] = cExamQuestions.StrAnswerFive;
                dr["c012"] = cExamQuestions.StrAnswerSix;
                dr["c013"] = cExamQuestions.CorrectAnswerOne;
                dr["c014"] = cExamQuestions.CorrectAnswerTwo;
                dr["c015"] = cExamQuestions.CorrectAnswerThree;
                dr["c016"] = cExamQuestions.CorrectAnswerFour;
                dr["c017"] = cExamQuestions.CorrectAnswerFive;
                dr["c018"] = cExamQuestions.CorrectAnswerSix;
                dr["c019"] = cExamQuestions.IntUseNumber;
                dr["c020"] = cExamQuestions.StrAccessoryType;
                dr["c021"] = cExamQuestions.StrAccessoryName;
                dr["c022"] = cExamQuestions.StrAccessoryPath;
                dr["c023"] = session.UserInfo.UserID;
                dr["c024"] = session.UserInfo.UserName;
                dr["c025"] = cExamQuestions.StrDateTime;
                objDataSet.Tables[0].Rows.Add(dr);
                objAdapter.Update(objDataSet);
                objDataSet.AcceptChanges();  
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

        private OperationReturn OptUpdateQuestion(SessionInfo session, List<string> listParams)
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
                string strCategoryParam = listParams[0];
                optReturn = XMLHelper.DeserializeObject<CQuestionsParam>(strCategoryParam);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                CQuestionsParam cExamQuestions = optReturn.Data as CQuestionsParam;
                if (cExamQuestions == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("PapersCategoryParam is null");
                    return optReturn;
                }
                string rentToken = session.RentInfo.Token;
                string strSql = string.Format("SELECT * FROM T_36_022_{0}  where c001='{1}' ", rentToken,
                    cExamQuestions.LongNum);
                IDbConnection objConn;
                IDbDataAdapter objAdapter;
                DbCommandBuilder objCmdBuilder;
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
                if (objConn == null || objAdapter == null || objCmdBuilder == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("Db object is null");
                    return optReturn;
                }

                DataSet objDataSet = new DataSet();
                objCmdBuilder.ConflictOption = ConflictOption.OverwriteChanges;
                objCmdBuilder.SetAllValues = false;
                objAdapter.Fill(objDataSet);
                if (objDataSet.Tables.Count <= 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_NOT_EXIST;
                    optReturn.Message = string.Format("Select is Null!");
                    return optReturn;
                }
                DataRow dr = objDataSet.Tables[0].Rows[0];
                dr["c002"] = cExamQuestions.LongCategoryNum;
                dr["c003"] = cExamQuestions.StrCategoryName;
                dr["c004"] = cExamQuestions.IntType;
                dr["c005"] = cExamQuestions.StrQuestionsContect;
                dr["c006"] = cExamQuestions.StrShortAnswerAnswer;
                dr["c007"] = cExamQuestions.StrAnswerOne;
                dr["c008"] = cExamQuestions.StrAnswerTwo;
                dr["c009"] = cExamQuestions.StrAnswerThree;
                dr["c010"] = cExamQuestions.StrAnswerFour;
                dr["c011"] = cExamQuestions.StrAnswerFive;
                dr["c012"] = cExamQuestions.StrAnswerSix;
                dr["c013"] = cExamQuestions.CorrectAnswerOne;
                dr["c014"] = cExamQuestions.CorrectAnswerTwo;
                dr["c015"] = cExamQuestions.CorrectAnswerThree;
                dr["c016"] = cExamQuestions.CorrectAnswerFour;
                dr["c017"] = cExamQuestions.CorrectAnswerFive;
                dr["c018"] = cExamQuestions.CorrectAnswerSix;
                dr["c019"] = cExamQuestions.IntUseNumber;
                dr["c020"] = cExamQuestions.StrAccessoryType;
                dr["c021"] = cExamQuestions.StrAccessoryName;
                dr["c022"] = cExamQuestions.StrAccessoryPath;
                dr["c023"] = session.UserInfo.UserID;
                dr["c024"] = session.UserInfo.UserName;
                dr["c025"] = cExamQuestions.StrDateTime;
                objAdapter.Update(objDataSet);
                objDataSet.AcceptChanges();
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

        private OperationReturn OptDeleteCategory(SessionInfo session, List<string> listParams)
        {
            _mListDeleteInfo.Clear();
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

                string strCategoryParam = listParams[0];
                optReturn = XMLHelper.DeserializeObject<PapersCategoryParam>(strCategoryParam);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                PapersCategoryParam papersCategoryParam = optReturn.Data as PapersCategoryParam;
                if (papersCategoryParam == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("PapersCategoryParam is null");
                    return optReturn;
                }
                
                OperationReturn optReturnTemp = DeleteChildrenNodes(session, papersCategoryParam.StrName, 0);
                if (optReturnTemp.Message == S3601Consts.HadUse)
                {
                    optReturn.Message = S3601Consts.HadUse;
                    return optReturn;
                }
                foreach ( string strSql in _mListDeleteInfo )
                {
                    switch (session.DBType)
                    {
                    case 2:
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        break;
                    case 3:
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
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
                }
                optReturn.Message = S3601Consts.DeleteSuccess;
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

        private OperationReturn DeleteChildrenNodes(SessionInfo session, string strNodeName, int iDepth)
        {
            OperationReturn optReturn = new OperationReturn();
            string strSql1 = string.Format("SELECT * FROM T_36_021_{0} WHERE C004='{1}'", session.RentInfo.Token, ParsedString(strNodeName));
            DataSet objDataSet;
            switch (session.DBType)
            {
                case 2:
                    optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql1);
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                    objDataSet = optReturn.Data as DataSet;
                    break;
                case 3:
                    optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql1);
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
            DataSet objDataSet1;
            string strSql2 = string.Format("SELECT * FROM T_36_022_{0} WHERE C003='{1}'", session.RentInfo.Token, ParsedString(strNodeName));
            switch (session.DBType)
            {
                case 2:
                    optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql2);
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                    objDataSet1 = optReturn.Data as DataSet;
                    break;
                case 3:
                    optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql2);
                    if (!optReturn.Result)
                    {
                        return optReturn;
                    }
                    objDataSet1 = optReturn.Data as DataSet;
                    break;
                default:
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Database type not surpport.\t{0}", session.DBType);
                    return optReturn;
            }

            if (objDataSet1 == null)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_OBJECT_NULL;
                optReturn.Message = string.Format("DataSet is null");
                return optReturn;
            }
            if (objDataSet1.Tables[0].Rows.Count == 0)
            {
                iDepth++;
                string strSqlTemp = string.Format("DELETE FROM T_36_021_{0} WHERE C002='{1}'", session.RentInfo.Token, ParsedString(strNodeName));
                _mListDeleteInfo.Add(strSqlTemp);
            }
            else
            {
                optReturn.Message = S3601Consts.HadUse;
            }
            if (objDataSet.Tables[0].Rows.Count == 0 && iDepth == 0)
            {
                return optReturn;
            }
            for (int i = 0; i < objDataSet.Tables[0].Rows.Count; i++)
            {
                DataRow dr = objDataSet.Tables[0].Rows[i];
                string strName = dr["C002"].ToString();
                iDepth++;
                OperationReturn optReturnTemp = DeleteChildrenNodes(session, strName,iDepth);
                if (optReturnTemp.Message == S3601Consts.HadUse)
                {
                    optReturn.Message = S3601Consts.HadUse;
                    return optReturn;
                }
                string strSql3 = string.Format("SELECT * FROM T_36_022_{0} WHERE C003='{1}'", session.RentInfo.Token, strName);
                DataSet objDataSet2;
                switch (session.DBType)
                {
                    case 2:
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql3);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet2 = optReturn.Data as DataSet;
                        break;
                    case 3:
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql3);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet2 = optReturn.Data as DataSet;
                        break;
                    default:
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_PARAM_INVALID;
                        optReturn.Message = string.Format("Database type not surpport.\t{0}", session.DBType);
                        return optReturn;
                }
               
                if (objDataSet2 == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("DataSet is null");
                    return optReturn;
                }
                if (objDataSet2.Tables[0].Rows.Count > 0)
                {
                    optReturn.Message = S3601Consts.HadUse;
                }
                else
                {
                    iDepth++;
                    string strSqlTemp = string.Format("DELETE FROM T_36_021_{0} WHERE C002='{1}'", session.RentInfo.Token, strName);
                    _mListDeleteInfo.Add(strSqlTemp); 
                }
            }
            return optReturn;
        }

        private OperationReturn OptDeleteQuestions(SessionInfo session, List<string> listParams)
        {
            _mListDeleteInfo.Clear();
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

                string strCategoryParam = listParams[0];
                optReturn = XMLHelper.DeserializeObject<List<CQuestionsParam>>(strCategoryParam);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                List<CQuestionsParam> listExamQuestions = optReturn.Data as List<CQuestionsParam>;
                if (listExamQuestions == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("PapersCategoryParam is null");
                    return optReturn;
                }

                foreach (CQuestionsParam examQuestion in listExamQuestions)
                {
                    string strSql = string.Format("delete T_36_022_{0} Where C001 = '{1}'", session.RentInfo.Token, examQuestion.LongNum);
                    switch (session.DBType)
                    {
                        case 2:
                            optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                            if (!optReturn.Result)
                            {
                                return optReturn;
                            }
                            break;
                        case 3:
                            optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
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
                }
                optReturn.Message = S3601Consts.DeleteSuccess;
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

        private OperationReturn OptGetSerialID(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                //ListParams
                //0     模块编码
                //1     资源编码
                //2     时间变量
                if (listParams == null || listParams.Count < 3)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string moduleId = listParams[0];
                string resourceId = listParams[1];
                string dateFormat = listParams[2];
                string rentToken = session.RentInfo.Token;
                string strSerialId = string.Empty;
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
                        mssqlParameters[0].Value = moduleId;
                        mssqlParameters[1].Value = resourceId;
                        mssqlParameters[2].Value = rentToken;
                        mssqlParameters[3].Value = dateFormat;
                        mssqlParameters[4].Value = strSerialId;
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
                            strSerialId = mssqlParameters[4].Value.ToString();
                            optReturn.Data = strSerialId;
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
                        orclParameters[0].Value = moduleId;
                        orclParameters[1].Value = resourceId;
                        orclParameters[2].Value = rentToken;
                        orclParameters[3].Value = dateFormat;
                        orclParameters[4].Value = strSerialId;
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
                            strSerialId = orclParameters[4].Value.ToString();
                            optReturn.Data = strSerialId;
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

        /// <summary>
        /// 获取服务端UMP安装目录
        /// </summary>
        private OperationReturn OptGetUMPSetupPath()
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                string umpPath = System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory);//根树地址
                umpPath = umpPath.Substring(0, umpPath.LastIndexOf("\\"));
                umpPath = umpPath.Substring(0, umpPath.LastIndexOf("\\"));
                umpPath = Path.Combine(umpPath, "MediaData");
                if (!Directory.Exists(umpPath))
                {
                    Directory.CreateDirectory(umpPath);
                }
                optReturn.StringValue = umpPath;
                _mStrUmpPath = umpPath;
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

        private OperationReturn OptGetUploadFilePath(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                DataSet objDataSet;
                string strSql = string.Format("SELECT * FROM T_11_001_{0} WHERE C003='36010101'", session.RentInfo.Token);
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
                    string strPath = string.Empty;
                    strPath = dr["C006"].ToString();
                    optReturn.StringValue = strPath;
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

        private OperationReturn OptAddPaper(SessionInfo session, List<string> listParams)
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
                string strCAddPaper = listParams[0];
                optReturn = XMLHelper.DeserializeObject<CPaperParam>(strCAddPaper);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                CPaperParam cAddPaper = optReturn.Data as CPaperParam;
                if (cAddPaper == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("AddPaperParam is null");
                    return optReturn;
                }
                string rentToken = session.RentInfo.Token;
                string strSql;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("INSERT INTO T_36_023_{0} ( c001, c002, c003, c004, c007, c008, c009, c010, c011, c012, c013 ) values ('{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}')",
                            rentToken, cAddPaper.LongNum, cAddPaper.StrName, cAddPaper.StrDescribe, cAddPaper.CharType, cAddPaper.IntQuestionNum, cAddPaper.IntScores, cAddPaper.IntPassMark, cAddPaper.IntTestTime, cAddPaper.LongEditorId, cAddPaper.StrEditor, cAddPaper.StrDateTime);
                        optReturn = MssqlOperation.ExecuteSql(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        break;
                    case 3:
                        strSql = string.Format("INSERT INTO T_36_023_{0} ( c001, c002, c003, c004, c007, c008, c009, c010, c011, c012, c013 ) values ('{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}')",
                            rentToken, cAddPaper.LongNum, cAddPaper.StrName, cAddPaper.StrDescribe, cAddPaper.CharType, cAddPaper.IntQuestionNum, cAddPaper.IntScores, cAddPaper.IntPassMark, cAddPaper.IntTestTime, cAddPaper.LongEditorId, cAddPaper.StrEditor, cAddPaper.StrDateTime);
                        optReturn = OracleOperation.ExecuteSql(session.DBConnectionString, strSql);
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

        private OperationReturn OptUpdatePaper(SessionInfo session, List<string> listParams)
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
                string strPaperParam = listParams[0];
                optReturn = XMLHelper.DeserializeObject<CPaperParam>(strPaperParam);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                var param = optReturn.Data as CPaperParam;
                if (param == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("Paper Param is null");
                    return optReturn;
                }
                string rentToken = session.RentInfo.Token;
                string strSql;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("UPDATE T_36_023_{0} set c002 ='{1}', c003='{2}', c004='{3}', c008='{4}', c009='{5}', c010='{6}' where c001='{7}'",
                            rentToken, param.StrName, param.StrDescribe, param.CharType, param.IntScores, param.IntPassMark, param.IntTestTime, param.LongNum);
                        optReturn = MssqlOperation.ExecuteSql(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        break;
                    case 3:
                        strSql = string.Format("UPDATE T_36_023_{0} set c002 ='{1}', c003='{2}', c004='{3}', c008='{4}', c009='{5}', c010='{6}' where c001='{7}'",
                            rentToken, param.StrName, param.StrDescribe, param.CharType, param.IntScores, param.IntPassMark, param.IntTestTime, param.LongNum);
                        optReturn = OracleOperation.ExecuteSql(session.DBConnectionString, strSql);

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

        private OperationReturn OptGetPapers(SessionInfo session, List<string> listParams)
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
                string strSql = string.Format("SELECT * FROM T_36_023_{0} WHERE C011='{1}'", session.RentInfo.Token, session.UserInfo.UserID);
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
                List<string> listReturn = new List<string>();
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
                    CPaperParam param = new CPaperParam();
                    param.LongNum = Convert.ToInt64(dr["C001"]);
                    param.StrName = dr["C002"].ToString();
                    param.StrDescribe = dr["C003"].ToString();
                    param.CharType = Convert.ToChar(dr["C004"]);
                    if (AnalyzingFieldIsNull(dr["C005"].ToString()))
                        param.LongSource = Convert.ToInt64(dr["C005"]);
                    if (AnalyzingFieldIsNull(dr["C006"].ToString()))
                        param.IntWeight = Convert.ToInt32(dr["C006"]);
                    param.IntQuestionNum = Convert.ToInt32(dr["C007"]);
                    param.IntScores = Convert.ToInt32(dr["C008"]);
                    param.IntPassMark = Convert.ToInt32(dr["C009"]);
                    param.IntTestTime = Convert.ToInt32(dr["C010"]);
                    param.LongEditorId = Convert.ToInt64(dr["C011"]);
                    param.StrEditor = dr["C012"].ToString();
                    param.StrDateTime = Convert.ToDateTime(dr["C013"]).ToString("yyyy/MM/dd HH:mm:ss");
                    if (AnalyzingFieldIsNull(dr["C014"].ToString()))
                        param.IntUsed = Convert.ToInt16(dr["C014"]);
                    if (AnalyzingFieldIsNull(dr["C015"].ToString()))
                        param.IntAudit = Convert.ToInt16(dr["C015"]);
                    if (AnalyzingFieldIsNull(dr["C016"].ToString()))
                        param.LongVerifierId = Convert.ToInt64(dr["C016"]);
                    param.StrVerifier = dr["C017"].ToString();
                    optReturn = XMLHelper.SeriallizeObject(param);
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

        private OperationReturn OptSearchPapers(SessionInfo session, List<string> listParams)
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

                string strPaperParam = listParams[0];
                optReturn = XMLHelper.DeserializeObject<List<CPaperParam>>(strPaperParam);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                var listParam = optReturn.Data as List<CPaperParam>;
                if (listParam == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("Paper Param is null");
                    return optReturn;
                }

                DataSet objDataSet;
                List<string> listReturn = new List<string>();
                foreach (var param in listParam)
                {
                    string strSql = string.Format("SELECT * FROM T_36_023_{0} WHERE C001='{1}'", session.RentInfo.Token,
                        param.LongNum);
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
                        CPaperParam paperParam = new CPaperParam();
                        paperParam.LongNum = Convert.ToInt64(dr["C001"]);
                        paperParam.StrName = dr["C002"].ToString();
                        paperParam.StrDescribe = dr["C003"].ToString();
                        paperParam.CharType = Convert.ToChar(dr["C004"]);
                        if (AnalyzingFieldIsNull(dr["C005"].ToString()))
                            paperParam.LongSource = Convert.ToInt64(dr["C005"]);
                        if (AnalyzingFieldIsNull(dr["C006"].ToString()))
                            paperParam.IntWeight = Convert.ToInt32(dr["C006"]);
                        paperParam.IntQuestionNum = Convert.ToInt32(dr["C007"]);
                        paperParam.IntScores = Convert.ToInt32(dr["C008"]);
                        paperParam.IntPassMark = Convert.ToInt32(dr["C009"]);
                        paperParam.IntTestTime = Convert.ToInt32(dr["C010"]);
                        paperParam.LongEditorId = Convert.ToInt64(dr["C011"]);
                        paperParam.StrEditor = dr["C012"].ToString();
                        paperParam.StrDateTime = Convert.ToDateTime(dr["C013"]).ToString("yyyy/MM/dd HH:mm:ss");
                        if (AnalyzingFieldIsNull(dr["C014"].ToString()))
                            paperParam.IntUsed = Convert.ToInt16(dr["C014"]);
                        if (AnalyzingFieldIsNull(dr["C015"].ToString()))
                            paperParam.IntAudit = Convert.ToInt16(dr["C015"]);
                        if (AnalyzingFieldIsNull(dr["C016"].ToString()))
                            paperParam.LongVerifierId = Convert.ToInt64(dr["C016"]);
                        paperParam.StrVerifier = dr["C017"].ToString();
                        optReturn = XMLHelper.SeriallizeObject(paperParam);
                        if (!optReturn.Result)
                        {
                            optReturn.Code = i;
                            return optReturn;
                        }
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

        private OperationReturn OptGetPaperQuestion( SessionInfo session, List<string> listParams )
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
                string strPaperParam = listParams[0];
                optReturn = XMLHelper.DeserializeObject<CPaperParam>(strPaperParam);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                var paperParam = optReturn.Data as CPaperParam;
                if (paperParam == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("PapersCategoryParam is null");
                    return optReturn;
                }
                DataSet objDataSet;
                string strSql = string.Format("SELECT * FROM T_36_024_{0} WHERE C001='{1}'", session.RentInfo.Token, paperParam.LongNum);
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
                List<string> listReturn = new List<string>();
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
                    CPaperQuestionParam param = new CPaperQuestionParam();
                    param.LongPaperNum = Convert.ToInt64(dr["C001"]);
                    param.LongQuestionNum = Convert.ToInt64(dr["C002"]);
                    param.IntQuestionType = Convert.ToInt32(dr["C003"]);
                    param.StrQuestions = dr["C004"].ToString();
                    param.EnableChange = Convert.ToInt16(dr["C005"]);
                    param.StrShortAnswerAnswer = dr["C006"].ToString();
                    param.StrAnswerA = dr["C007"].ToString();
                    param.StrAnswerB = dr["C008"].ToString();
                    param.StrAnswerC = dr["C009"].ToString();
                    param.StrAnswerD = dr["C010"].ToString();
                    param.StrAnswerE = dr["C011"].ToString();
                    param.StrAnswerF = dr["C012"].ToString();
                    param.CorrectAnswerOne = dr["C013"].ToString();
                    param.CorrectAnswerTwo = dr["C014"].ToString();
                    param.CorrectAnswerThree = dr["C015"].ToString();
                    param.CorrectAnswerFour = dr["C016"].ToString();
                    param.CorrectAnswerFive = dr["C017"].ToString();
                    param.CorrectAnswerSix = dr["C018"].ToString();
                    param.IntScore = Convert.ToInt32(dr["C019"]);
                    param.StrAccessoryType = dr["C021"].ToString();
                    param.StrAccessoryName = dr["C022"].ToString();
                    param.StrAccessoryPath = dr["C023"].ToString();
                    param.StrQuestionCategory = dr["C024"].ToString();
                    optReturn = XMLHelper.SeriallizeObject(param);
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

        private OperationReturn OptPaperSameName(SessionInfo session, List<string> listParams)
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

                string strPaperParam = listParams[0];
                optReturn = XMLHelper.DeserializeObject<CPaperParam>(strPaperParam);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                CPaperParam paperParam = optReturn.Data as CPaperParam;
                if (paperParam == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("PapersCategoryParam is null");
                    return optReturn;
                }

                DataSet objDataSet;
                string strSql;
                strSql = paperParam.LongSource != 0 ? string.Format("SELECT * FROM T_36_023_{0} WHERE C002='{1}' and c005='{2}' ", session.RentInfo.Token, paperParam.StrName, paperParam.LongSource) : string.Format("SELECT * FROM T_36_023_{0} WHERE C002='{1}'", session.RentInfo.Token, paperParam.StrName);
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
                if( objDataSet.Tables[0].Rows.Count > 0)
                {
                    optReturn.Message = S3601Consts.PaperExist;
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

        private OperationReturn OptCreateCategory(SessionInfo session, List<string> listParams)
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
                string strCategoryParam = listParams[0];
                optReturn = XMLHelper.DeserializeObject<PapersCategoryParam>(strCategoryParam);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                PapersCategoryParam papersCategoryParam = optReturn.Data as PapersCategoryParam;
                if (papersCategoryParam == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("PapersCategoryParam is null");
                    return optReturn;
                }

                string rentToken = session.RentInfo.Token;

                string strSql = string.Format("SELECT * FROM T_36_021_{0}", rentToken);
                IDbConnection objConn;
                IDbDataAdapter objAdapter;
                DbCommandBuilder objCmdBuilder;
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
                DataRow dr = objDataSet.Tables[0].NewRow();
                dr["C001"] = papersCategoryParam.LongNum;
                dr["C002"] = papersCategoryParam.StrName;
                dr["C003"] = papersCategoryParam.LongParentNodeId;
                dr["C004"] = papersCategoryParam.StrParentNodeName;
                dr["C005"] = session.UserInfo.UserID;
                dr["C006"] = session.UserInfo.UserName;
                dr["C007"] = papersCategoryParam.StrDateTime;
                objDataSet.Tables[0].Rows.Add(dr);
                objAdapter.Update(objDataSet);
                objDataSet.AcceptChanges(); 
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

        private OperationReturn OptUpdateCategory(SessionInfo session, List<string> listParams)
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
                string strCategoryParam = listParams[0];
                optReturn = XMLHelper.DeserializeObject<PapersCategoryParam>(strCategoryParam);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                PapersCategoryParam papersCategoryParam = optReturn.Data as PapersCategoryParam;
                if (papersCategoryParam == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("PapersCategoryParam is null");
                    return optReturn;
                }

                string rentToken = session.RentInfo.Token;

                string strSql = string.Format("SELECT * FROM T_36_021_{0} WHERE C001 = '{1}'", rentToken, papersCategoryParam.LongNum);
                IDbConnection objConn;
                IDbDataAdapter objAdapter;
                DbCommandBuilder objCmdBuilder;
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
                if (objDataSet.Tables.Count <= 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_NOT_EXIST;
                    optReturn.Message = string.Format("Select is Null!");
                    return optReturn;
                }
                DataRow dr = objDataSet.Tables[0].Rows[0];
                dr["C002"] = papersCategoryParam.StrName;
                objAdapter.Update(objDataSet);
                objDataSet.AcceptChanges();
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

        private OperationReturn OptPaperQuestionsExist(SessionInfo session, List<string> listParams)
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
                string strEditPaper = listParams[0];
                optReturn = XMLHelper.DeserializeObject<List<CPaperQuestionParam>>(strEditPaper);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                List<CPaperQuestionParam> listEditPapers = optReturn.Data as List<CPaperQuestionParam>;
                if (listEditPapers == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("PapersCategoryParam is null");
                    return optReturn;
                }

                List<string> listReturn = new List<string>();
                foreach (var editPaper in listEditPapers)
                {
                    DataSet objDataSet;
                    string strSql = string.Format("SELECT * FROM T_36_024_{0} WHERE C001='{1}' and C002='{2}'", session.RentInfo.Token, editPaper.LongPaperNum, editPaper.LongQuestionNum);
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
                    if( objDataSet.Tables[0].Rows.Count > 0)
                    {
                        optReturn = XMLHelper.SeriallizeObject(editPaper.LongQuestionNum);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
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

        private OperationReturn OptGetQuestionsOfPaper(SessionInfo session, List<string> listParams)
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
                string strQuestionParam = listParams[0];
                optReturn = XMLHelper.DeserializeObject<List<CQuestionsParam>>(strQuestionParam);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                List<CQuestionsParam> listQuestionParams = optReturn.Data as List<CQuestionsParam>;
                if (listQuestionParams == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("PapersCategoryParam is null");
                    return optReturn;
                }

                List<string> listReturn = new List<string>();
                foreach (var param in listQuestionParams)
                {
                    DataSet objDataSet;
                    string strSql = string.Format("SELECT * FROM T_36_024_{0} WHERE C002='{1}'", session.RentInfo.Token, param.LongNum);
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
                    for (int i = 0; i< objDataSet.Tables[0].Rows.Count; i++)
                    {
                        DataRow dr = objDataSet.Tables[0].Rows[i];
                        CPaperQuestionParam paperParam = new CPaperQuestionParam();
                        paperParam.LongPaperNum = Convert.ToInt64(dr["C001"]);
                        optReturn = XMLHelper.SeriallizeObject(paperParam);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
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

        private OperationReturn OptQueryQuestions(SessionInfo session, List<string> listParams)
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
                string strQuestionParam = listParams[0];
                optReturn = XMLHelper.DeserializeObject<CQuestionsParam>(strQuestionParam);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                var questionParam = optReturn.Data as CQuestionsParam;
                if (questionParam == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("PapersCategoryParam is null");
                    return optReturn;
                }
                DataSet objDataSet;
                string strSql = string.Format("SELECT * FROM T_36_022_{0} WHERE C001='{1}'", session.RentInfo.Token, questionParam.LongNum);
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
                List<string> listReturn = new List<string>();
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
                    CQuestionsParam param = new CQuestionsParam();
                    param.LongNum = Convert.ToInt64(dr["C001"]);
                    param.LongCategoryNum = Convert.ToInt64(dr["C002"]);
                    param.StrCategoryName = dr["C003"].ToString();
                    param.IntType = Convert.ToInt32(dr["C004"]);
                    param.StrQuestionsContect = dr["C005"].ToString();
                    param.StrShortAnswerAnswer = dr["C006"].ToString();
                    param.StrAnswerOne = dr["C007"].ToString();
                    param.StrAnswerTwo = dr["C008"].ToString();
                    param.StrAnswerThree = dr["C009"].ToString();
                    param.StrAnswerFour = dr["C010"].ToString();
                    param.StrAnswerFive = dr["C011"].ToString();
                    param.StrAnswerSix = dr["C012"].ToString();
                    param.CorrectAnswerOne = dr["C013"].ToString();
                    param.CorrectAnswerTwo = dr["C014"].ToString();
                    param.CorrectAnswerThree = dr["C015"].ToString(); ;
                    param.CorrectAnswerFour = dr["C016"].ToString();;
                    param.CorrectAnswerFive = dr["C017"].ToString();
                    param.CorrectAnswerSix = dr["C018"].ToString();
                    param.IntUseNumber = Convert.ToInt32(dr["C019"]);
                    param.StrAccessoryType = dr["C020"].ToString();
                    param.StrAccessoryName = dr["C021"].ToString();
                    param.StrAccessoryPath = dr["C022"].ToString();
                    param.LongFounderId = Convert.ToInt64(dr["C023"]);
                    param.StrFounderName = dr["C024"].ToString();
                    param.StrDateTime = Convert.ToDateTime(dr["C025"]).ToString("yyyy/MM/dd HH:mm:ss");
                    optReturn = XMLHelper.SeriallizeObject(param);
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

        private OperationReturn OptDeletePaperQuestions(SessionInfo session, List<string> listParams)
        {
            _mListDeleteInfo.Clear();
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

                string strEditPaperParam = listParams[0];
                optReturn = XMLHelper.DeserializeObject<List<CPaperQuestionParam>>(strEditPaperParam);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                var listPaperQuestions = optReturn.Data as List<CPaperQuestionParam>;
                if (listPaperQuestions == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("listPaperQuestions is null");
                    return optReturn;
                }

                foreach (CPaperQuestionParam param in listPaperQuestions)
                {
                    string strSql = string.Format("delete T_36_024_{0} Where C001='{1}' and C002 = '{2}'", session.RentInfo.Token, param.LongPaperNum, param.LongQuestionNum);
                    switch (session.DBType)
                    {
                        case 2:
                            optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                            if (!optReturn.Result) return optReturn;
                            break;
                        case 3:
                            optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                            if (!optReturn.Result) return optReturn;
                            break;
                        default:
                            optReturn.Result = false;
                            optReturn.Code = Defines.RET_PARAM_INVALID;
                            optReturn.Message = string.Format("Database type not surpport.\t{0}", session.DBType);
                            return optReturn;
                    }
                }
                optReturn.Message = S3601Consts.DeleteSuccess;
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

        private OperationReturn OptDeletePaperAllQuestions(SessionInfo session, List<string> listParams)
        {
            _mListDeleteInfo.Clear();
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

                string strPaperParam = listParams[0];
                optReturn = XMLHelper.DeserializeObject<List<CPaperParam>>(strPaperParam);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                var listPaperParam = optReturn.Data as List<CPaperParam>;
                if (listPaperParam == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("listPaperQuestions is null");
                    return optReturn;
                }

                foreach (CPaperParam param in listPaperParam)
                {
                    string strSql = string.Format("delete T_36_024_{0} Where C001='{1}'", session.RentInfo.Token, param.LongNum);
                    switch (session.DBType)
                    {
                        case 2:
                            optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                            if (!optReturn.Result) return optReturn;
                            break;
                        case 3:
                            optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                            if (!optReturn.Result) return optReturn;
                            break;
                        default:
                            optReturn.Result = false;
                            optReturn.Code = Defines.RET_PARAM_INVALID;
                            optReturn.Message = string.Format("Database type not surpport.\t{0}", session.DBType);
                            return optReturn;
                    }
                }
                optReturn.Message = S3601Consts.DeleteSuccess;
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

        private OperationReturn OptDeletePaper(SessionInfo session, List<string> listParams)
        {
            _mListDeleteInfo.Clear();
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

                string strPaperParam = listParams[0];
                optReturn = XMLHelper.DeserializeObject<List<CPaperParam>>(strPaperParam);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                var listPaperParam = optReturn.Data as List<CPaperParam>;
                if (listPaperParam == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("listPaperQuestions is null");
                    return optReturn;
                }

                foreach (CPaperParam param in listPaperParam)
                {
                    string strSql = string.Format("delete T_36_023_{0} Where C001='{1}'", session.RentInfo.Token, param.LongNum);
                    switch (session.DBType)
                    {
                        case 2:
                            optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                            if (!optReturn.Result) return optReturn;
                            break;
                        case 3:
                            optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                            if (!optReturn.Result) return optReturn;
                            break;
                        default:
                            optReturn.Result = false;
                            optReturn.Code = Defines.RET_PARAM_INVALID;
                            optReturn.Message = string.Format("Database type not surpport.\t{0}", session.DBType);
                            return optReturn;
                    }
                }
                optReturn.Message = S3601Consts.DeleteSuccess;
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

        private OperationReturn OptSearchQuestion(SessionInfo session, List<string> listParams)
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
                string strParam = listParams[0];
                optReturn = XMLHelper.DeserializeObject<string>(strParam);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                string strSql = optReturn.Data as string;
                if (strSql == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("PapersCategoryParam is null");
                    return optReturn;
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
                List<string> listReturn = new List<string>();
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
                    CQuestionsParam param = new CQuestionsParam();
                    param.LongNum = Convert.ToInt64(dr["C001"]);
                    param.LongCategoryNum = Convert.ToInt64(dr["C002"]);
                    param.StrCategoryName = dr["C003"].ToString();
                    param.IntType = Convert.ToInt32(dr["C004"]);
                    param.StrQuestionsContect = dr["C005"].ToString();
                    param.StrShortAnswerAnswer = dr["C006"].ToString();
                    param.StrAnswerOne = dr["C007"].ToString();
                    param.StrAnswerTwo = dr["C008"].ToString();
                    param.StrAnswerThree = dr["C009"].ToString();
                    param.StrAnswerFour = dr["C010"].ToString();
                    param.StrAnswerFive = dr["C011"].ToString();
                    param.StrAnswerSix = dr["C012"].ToString();
                    param.CorrectAnswerOne = dr["C013"].ToString();
                    param.CorrectAnswerTwo = dr["C014"].ToString();
                    param.CorrectAnswerThree = dr["C015"].ToString();
                    param.CorrectAnswerFour = dr["C016"].ToString();
                    param.CorrectAnswerFive =dr["C017"].ToString();
                    param.CorrectAnswerSix = dr["C018"].ToString();
                    param.IntUseNumber = Convert.ToInt32(dr["C019"]);
                    param.StrAccessoryType = dr["C020"].ToString();
                    param.StrAccessoryName = dr["C021"].ToString();
                    param.StrAccessoryPath = dr["C022"].ToString();
                    param.LongFounderId = Convert.ToInt64(dr["C023"]);
                    param.StrFounderName = dr["C024"].ToString();
                    param.StrDateTime = Convert.ToDateTime(dr["C025"]).ToString("yyyy/MM/dd HH:mm:ss");
                    optReturn = XMLHelper.SeriallizeObject(param);
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

        private OperationReturn OptLoadFiles(SessionInfo session, List<string> listParams)
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

                string struestionParam = listParams[0];
                optReturn = XMLHelper.DeserializeObject<CQuestionsParam>(struestionParam);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                var questionParam = optReturn.Data as CQuestionsParam;
                if (questionParam == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("listPaperQuestions is null");
                    return optReturn;
                }

                string[] nameTemp = questionParam.StrAccessoryName.Split(new char[] {'.'});
                string name = null;
                for (int i = 0; i < nameTemp.Length -1; i++)
                {
                    name += nameTemp[i].ToString();
                }
                name += StringToHexString(questionParam.StrAccessoryName, Encoding.UTF8) + "." + questionParam.StrAccessoryType;
                string targetFile = _mStrUmpPath + "/" + name;
                if ( !File.Exists(targetFile) )
                {
                    File.Copy(questionParam.StrAccessoryPath, targetFile, true);
                }
                optReturn.StringValue = "MediaData/" + name;
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

        private OperationReturn OptImportExcelFile(SessionInfo session, List<string> listParams)
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
                string strCategoryParam = listParams[0];
                optReturn = XMLHelper.DeserializeObject<List<CQuestionsParam>>(strCategoryParam);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                List<CQuestionsParam> lstExamQuestions = optReturn.Data as List<CQuestionsParam>;
                if (lstExamQuestions == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("PapersCategoryParam is null");
                    return optReturn;
                }
                string rentToken = session.RentInfo.Token;
                string strSql = string.Format("SELECT * FROM T_36_022_{0}", rentToken);
                IDbConnection objConn;
                IDbDataAdapter objAdapter;
                DbCommandBuilder objCmdBuilder;
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
                if (objConn == null || objAdapter == null || objCmdBuilder == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_OBJECT_NULL;
                    optReturn.Message = string.Format("Db object is null");
                    return optReturn;
                }
                foreach (var cExamQuestions in lstExamQuestions)
                {
                    objCmdBuilder.ConflictOption = ConflictOption.OverwriteChanges;
                    objCmdBuilder.SetAllValues = false;
                    DataSet objDataSet = new DataSet();
                    objAdapter.Fill(objDataSet);
                    DataRow dr = objDataSet.Tables[0].NewRow();
                    dr["c001"] = cExamQuestions.LongNum;
                    dr["c002"] = cExamQuestions.LongCategoryNum;
                    dr["c003"] = cExamQuestions.StrCategoryName;
                    dr["c004"] = cExamQuestions.IntType;
                    dr["c005"] = cExamQuestions.StrQuestionsContect;
                    dr["c006"] = cExamQuestions.StrShortAnswerAnswer;
                    dr["c007"] = cExamQuestions.StrAnswerOne;
                    dr["c008"] = cExamQuestions.StrAnswerTwo;
                    dr["c009"] = cExamQuestions.StrAnswerThree;
                    dr["c010"] = cExamQuestions.StrAnswerFour;
                    dr["c011"] = cExamQuestions.StrAnswerFive;
                    dr["c012"] = cExamQuestions.StrAnswerSix;
                    dr["c013"] = cExamQuestions.CorrectAnswerOne;
                    dr["c014"] = cExamQuestions.CorrectAnswerTwo;
                    dr["c015"] = cExamQuestions.CorrectAnswerThree;
                    dr["c016"] = cExamQuestions.CorrectAnswerFour;
                    dr["c017"] = cExamQuestions.CorrectAnswerFive;
                    dr["c018"] = cExamQuestions.CorrectAnswerSix;
                    dr["c019"] = cExamQuestions.IntUseNumber;
                    dr["c020"] = cExamQuestions.StrAccessoryType;
                    dr["c021"] = cExamQuestions.StrAccessoryName;
                    dr["c022"] = cExamQuestions.StrAccessoryPath;
                    dr["c023"] = session.UserInfo.UserID;
                    dr["c024"] = session.UserInfo.UserName;
                    dr["c025"] = cExamQuestions.StrDateTime;
                    objDataSet.Tables[0].Rows.Add(dr);
                    objAdapter.Update(objDataSet);
                    objDataSet.AcceptChanges();
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
        

        public WebReturn UmpTaskOperation(WebRequest webRequest)
        {
            var webReturn = new WebReturn();
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
                    dbInfo.RealPassword = DecryptString004(dbInfo.Password);
                    session.DBConnectionString = dbInfo.GetConnectionString();
                }
                switch (webRequest.Code)
                {
                    case (int)S3601Codes.OperationGetUmpsetuppath:
                        optReturn = OptGetUMPSetupPath();
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.Data = optReturn.StringValue;
                        break;
                    case (int)S3601Codes.OperationGetUploadFilePath:
                        optReturn = OptGetUploadFilePath(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.Data = optReturn.StringValue;
                        break;
                    case (int)S3601Codes.OperationGetQuestionCategory:
                        optReturn = OptGetQuestinCategory(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)RequestCode.WSGetSerialID:
                        optReturn = OptGetSerialID(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.Data = optReturn.Data.ToString();
                        break;
                    case (int)S3601Codes.OperationCreateCategory:
                        optReturn = OptCreateCategory(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        break;
                    case (int)S3601Codes.OperationUpdateCategory:
                        optReturn = OptUpdateCategory(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        break;
                    case (int)S3601Codes.OperationDeleteCategory:
                        optReturn = OptDeleteCategory( session, webRequest.ListData );
                        if( !optReturn.Result )
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        break;
                    case (int)S3601Codes.OperationAddQuestion:
                        optReturn = OptAddQuestion(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        break;
                    case (int)S3601Codes.OperationGetQuestions:
                        optReturn = OptGetQuestions(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S3601Codes.OperationUpdateQuestion:
                        optReturn = OptUpdateQuestion(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S3601Codes.OperationDeleteQuestion:
                        optReturn = OptDeleteQuestions(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        break;
                    case (int)S3601Codes.OperationPaperSameName:
                        optReturn = OptPaperSameName(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S3601Codes.OperationAddPaper:
                        optReturn = OptAddPaper(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        break;
                    case (int)S3601Codes.OperationGetPapers:
                        optReturn = OptGetPapers(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S3601Codes.OperationGetPaperQuestions:
                        optReturn = OptGetPaperQuestion(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S3601Codes.OperationPaperQuestionsExist:
                        optReturn = OptPaperQuestionsExist(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S3601Codes.OperationAddPaperQuestions:
                        optReturn = OptAddPaperQuestions(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        break;
                    case (int)S3601Codes.OperationQueryQuestions:
                        optReturn = OptQueryQuestions(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S3601Codes.OperationDeletePaperQuestions:
                        optReturn = OptDeletePaperQuestions(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        break;
                    case (int)S3601Codes.OperationDeletePaperAllQuestions:
                        optReturn = OptDeletePaperAllQuestions(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        break;
                    case (int)S3601Codes.OperationDeletePaper:
                        optReturn = OptDeletePaper(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        break;
                    case (int)S3601Codes.OperationUpdatePaper:
                        optReturn = OptUpdatePaper(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        break;
                    case (int)S3601Codes.OperationSearchPapers:
                        optReturn = OptSearchPapers(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S3601Codes.OperationGetQuestionsOfPaper:
                        optReturn = OptGetQuestionsOfPaper(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S3601Codes.OperationSearchQuestions:
                        optReturn = OptSearchQuestion(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)S3601Codes.OperationLoadFile:
                        optReturn = OptGetUMPSetupPath();
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        optReturn = OptLoadFiles(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.Data = optReturn.StringValue;
                        break;
                    case (int)S3601Codes.OperationImportExcelFile:
                        optReturn = OptImportExcelFile(session, webRequest.ListData);
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

        public WebReturn UmpUpOperation(UpRequest upRequest)
        {
            var webReturn = new WebReturn();
            SessionInfo session = upRequest.Session;
            webReturn.Session = session;
            try
            {
                var fileStm = new FileStream(upRequest.SvPath, FileMode.OpenOrCreate);
                fileStm.Seek(0, SeekOrigin.End);
                fileStm.Write(upRequest.ListByte, 0, upRequest.ListByte.Length);
                fileStm.Flush();
                fileStm.Close();
                fileStm.Dispose();
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


        private string DecryptString004(string strSource)
        {
            try
            {
                return ServerAESEncryption.DecryptString(strSource, EncryptionMode.AES256V04Hex);
            }
            catch
            {
                return strSource;
            }
        }
    }
}
