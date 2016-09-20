using Common2400;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.ServiceModel.Activation;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;
using VoiceCyber.UMP.Encryptions;

namespace Wcf24041
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码、svc 和配置文件中的类名“Service1”。
    // 注意: 为了启动 WCF 测试客户端以测试此服务，请在解决方案资源管理器中选择 Service1.svc 或 Service1.svc.cs，然后开始调试。
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class Service24041 : IService24041
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
                    case (int)S2400RequestCode.GetEmailInfo:
                        optReturn = GetEmailInfo(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.Data = optReturn.Data as string;
                        break;
                    case (int)S2400RequestCode.UpdateEmailInfo:
                         optReturn = UpdateEmailInfo(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                webReturn.Result = false;
                webReturn.Code = Defines.RET_FAIL;
                webReturn.Message = ex.Message;
            }
            return webReturn;
        }

        /// <summary>
        /// 获得邮件通知配置信息
        /// </summary>
        /// <param name="session"></param>
        /// <param name="lstParams"></param>
        /// <returns></returns>
        private OperationReturn GetEmailInfo(SessionInfo session, List<string> lstParams)
        {
            OperationReturn optReturn = new OperationReturn();
            try
            {
                string rentToken = session.RentInfo.Token;
                string strSql = string.Empty;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT * FROM T_11_001_{0} WHERE C001 = '{1}' and C002 =24 and C004  = 24041 ORDER BY C005", rentToken,session.RentID);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM T_11_001_{0} WHERE C001 = '{1}' AND C002 =24 AND C004  = 24041  ORDER BY C005", rentToken,session.RentID);
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        break;
                }
                DataSet ds = optReturn.Data as DataSet;
                if (ds.Tables.Count <= 0 || ds.Tables[0].Rows.Count <= 0 || ds.Tables[0].Rows.Count < 8)
                {
                    return optReturn;
                }

                List<string> lstData = new List<string>();
                EncryptEmail emailInfo = new EncryptEmail();

                emailInfo.IsEnableEmail = ds.Tables[0].Rows[0]["C006"].ToString();
                emailInfo.SMTP = ds.Tables[0].Rows[1]["C006"].ToString();
                emailInfo.SMTPPort = ds.Tables[0].Rows[2]["C006"].ToString();
                emailInfo.NeedSSL = ds.Tables[0].Rows[3]["C006"].ToString();
                emailInfo.Account = ds.Tables[0].Rows[4]["C006"].ToString();
                emailInfo.Pwd = ds.Tables[0].Rows[5]["C006"].ToString();
                emailInfo.NeedAuthentication = ds.Tables[0].Rows[6]["C006"].ToString();
                emailInfo.EmailAddress = ds.Tables[0].Rows[7]["C006"].ToString();
                optReturn = XMLHelper.SeriallizeObject<EncryptEmail>(emailInfo);
                if (!optReturn.Result)
                {
                    return optReturn;
                }

                optReturn.Result = true;
                optReturn.Code = Defines.RET_SUCCESS;
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

        /// <summary>
        /// 添加或更新邮件通知参数
        /// </summary>
        /// <param name="session"></param>
        /// <param name="lstParams"></param>
        /// lstParams[0] : 是否启用邮件通知
        /// lstParams[1] : SMTP
        /// lstParams[2] : SMTP Port
        /// lstParams[3] : 需要SSL
        /// lstParams[4] : 账号
        /// lstParams[5] : 密码
        /// lstParams[6] : 需要身份认证 
        /// lstParams[7] : Email Address
        /// <returns></returns>
        private OperationReturn UpdateEmailInfo(SessionInfo session, List<string> lstParams)
        {
            OperationReturn optReturn = new OperationReturn();
            try
            {
                string rentToken = session.RentInfo.Token;
                string strSql = string.Empty;

                lstParams[5] = S2400EncryptOperation.EncryptWithM002(lstParams[5]);

                IDbConnection objConn = null;
                IDbDataAdapter objAdapter = null;
                DbCommandBuilder objCmdBuilder = null;

                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT * FROM T_11_001_{0} WHERE C001 = '{1}' and C002 =24 and C004  = 24041 ORDER BY C005", rentToken,session.RentID);
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
                        strSql = string.Format("SELECT * FROM T_11_001_{0} WHERE C001 = '{1}' AND C002 =24 AND C004  = 24041  ORDER BY C005", rentToken,session.RentID);
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objConn = OracleOperation.GetConnection(session.DBConnectionString);
                        objAdapter = OracleOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = OracleOperation.GetCommandBuilder(objAdapter);
                        break;
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

                List<string> listMsg = new List<string>();
                if (lstParams[0] == "0")
                {
                    DataRow[] drs = objDataSet.Tables[0].Select("c003 = 240401");
                    if (drs.Count() > 0)
                    {
                        drs[0]["C006"] = lstParams[0];
                        drs[0]["C018"] = DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss");
                        drs[0]["C019"] = session.UserID;
                        listMsg.Add("Update Email Enable : 0");
                    }
                }
                else
                {
                    for (int i = 0; i < lstParams.Count; i++)
                    {
                        string strParamID = "24040" + (i + 1);
                        DataRow[] drs = objDataSet.Tables[0].Select(string.Format("c003 = {0}", strParamID));
                        //如果存在 则修改
                        if (drs.Count() > 0)
                        {
                            drs[0]["C006"] = lstParams[i];
                            drs[0]["C018"] = DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss");
                            drs[0]["C019"] = session.UserID;
                            listMsg.Add(string.Format("Update {0}", lstParams[i]));
                        }
                        //如果不存在 则插入 
                        else
                        {
                            DataRow row = objDataSet.Tables[0].NewRow();
                            row["C001"] = session.RentID;
                            row["C002"] = 24;
                            row["C003"] = "24040" + (i + 1);
                            row["C004"] = "24041";
                            row["C005"] = i;
                            row["C006"] = lstParams[i];
                            row["C007"] = 13;
                            row["C009"] = 0;
                            row["C010"] = 0;
                            row["C011"] = 0;
                            row["C015"] = 0;
                            row["C018"] = DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss");
                            row["C019"] = session.UserID;
                            row["C020"] = -1;
                            row["C021"] = DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss");
                            objDataSet.Tables[0].Rows.Add(row);
                            listMsg.Add(string.Format("Add {0}", lstParams[i]));
                        }
                    }
                }
               
                objAdapter.Update(objDataSet);
                objDataSet.AcceptChanges();
                optReturn.Data = listMsg;


                optReturn.Result = true;
                optReturn.Code = Defines.RET_SUCCESS;
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

        private static string DecryptFromClient(string strSource)
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
