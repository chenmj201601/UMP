using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;
using Common1600;
using System.Data;
using VoiceCyber.UMP.Encryptions;

namespace WCF16002
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码、svc 和配置文件中的类名“Service1”。
    // 注意: 为了启动 WCF 测试客户端以测试此服务，请在解决方案资源管理器中选择 Service1.svc 或 Service1.svc.cs，然后开始调试。
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class Service16002 : IService16002
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
                    dbInfo.RealPassword = DecryptString04(dbInfo.Password);
                    session.DBConnectionString = dbInfo.GetConnectionString();
                }
                switch (webRequest.Code)
                {
                    case (int)S1600RequestCode.ChangeUserStatus:
                        optReturn = ChangeUserStatus(session, webRequest.ListData);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            return webReturn;
                        }
                        break;
                    case (int)S1600RequestCode.GetCookieStatus:
                        optReturn = GetCookieStatusByID(session, webRequest.ListData);
                        webReturn.Message = optReturn.Message;
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            return webReturn;
                        }
                        webReturn.Data = optReturn.Data.ToString();
                        break;
                }
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

        /// <summary>
        /// 更新用户的在线状态
        /// </summary>
        /// <param name="session"></param>
        /// <param name="lstParams"></param>
        /// lstParams[0] : 在线状态 0：离线 1：在线
        /// <returns></returns>
        private OperationReturn ChangeUserStatus(SessionInfo session, List<string> lstParams)
        {
            OperationReturn optReturn = new OperationReturn();
            try
            {
                string strRent = session.RentInfo.Token;
                string strSql = string.Empty;
                string strUserID = session.UserID.ToString();

                //判断t_11_101中有没有记录 有则更新 没有则新加
                optReturn = CheckUserPropertyByID(session);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                string strType = strUserID.Substring(0, 3);
                if (optReturn.Code == (int)S1600WcfError.PropertyNone)
                {
                    //需要增加记录
                    switch (session.DBType)
                    {
                        case 2:
                            if (strType == ConstValue.RESOURCE_USER.ToString())
                            {
                                strSql = "insert into t_11_101_{0} (c001,c002,c015) values ({1},1,{2})";
                                strSql = string.Format(strSql, strRent, strUserID, lstParams[0]);
                            }
                            else if (strType == ConstValue.RESOURCE_AGENT.ToString())
                            {
                                strSql = "insert into t_11_101_{0} (c001,c002,c020) values ({1},2,{2})";
                                strSql = string.Format(strSql, strRent, strUserID, lstParams[0]);
                            }
                            optReturn = MssqlOperation.ExecuteSql(session.DBConnectionString, strSql);
                            break;
                        case 3:
                            if (strType == ConstValue.RESOURCE_USER.ToString())
                            {
                                strSql = "insert into t_11_101_{0} (c001,c002,c015) values ({1},1,{2})";
                                strSql = string.Format(strSql, strRent, strUserID, lstParams[0]);
                            }
                            else if (strType == ConstValue.RESOURCE_AGENT.ToString())
                            {
                                strSql = "insert into t_11_101_{0} (c001,c002,c020) values ({1},2,{2})";
                                strSql = string.Format(strSql, strRent, strUserID, lstParams[0]);
                            }
                            optReturn = OracleOperation.ExecuteSql(session.DBConnectionString, strSql);
                            break;
                    }
                }
                else
                {
                    switch (session.DBType)
                    {
                        case 2:
                            if (strType == ConstValue.RESOURCE_USER.ToString())
                            {
                                strSql = "update T_11_101_{0} set C015 = '{1}' where C001 = {2} and C002 = 1";
                                strSql = string.Format(strSql, strRent, lstParams[0], strUserID);
                            }
                            else if (strType == ConstValue.RESOURCE_AGENT.ToString())
                            {
                                strSql = "update T_11_101_{0} set C020 = '{1}' where C001 = {2} and C002 = 2";
                                strSql = string.Format(strSql, strRent, lstParams[0], strUserID);
                            }
                            optReturn = MssqlOperation.ExecuteSql(session.DBConnectionString, strSql);
                            break;
                        case 3:
                            if (strType == ConstValue.RESOURCE_USER.ToString())
                            {
                                strSql = "update T_11_101_{0} set C015 = '{1}' where C001 = {2} and C002 = 1";
                                strSql = string.Format(strSql, strRent, lstParams[0], strUserID);
                            }
                            else if (strType == ConstValue.RESOURCE_AGENT.ToString())
                            {
                                strSql = "update T_11_101_{0} set C020 = '{1}' where C001 = {2} and C002 = 2";
                                strSql = string.Format(strSql, strRent, lstParams[0], strUserID);
                            }
                            optReturn = OracleOperation.ExecuteSql(session.DBConnectionString, strSql);
                            break;
                    }
                }
                optReturn.Message += " ; " + strSql;
                if (!optReturn.Result)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_FAIL;
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

        private OperationReturn CheckUserPropertyByID(SessionInfo session)
        {
            OperationReturn optReturn = new OperationReturn();
            try
            {
                string strRent = session.RentInfo.Token;
                string strSql = string.Empty;
                string strUserID = session.UserID.ToString();

                //判断是坐席还是用户
                string strType = strUserID.Substring(0, 3);
                if (strType == ConstValue.RESOURCE_AGENT.ToString())
                {
                    //坐席
                    switch (session.DBType)
                    {
                        case 2:
                            strSql = "select * from t_11_101_{0} where C001 = {1} and C002 = 2";
                            strSql = string.Format(strSql, strRent, strUserID);
                            optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                            break;
                        case 3:
                            strSql = "select * from t_11_101_{0} where C001 = {1} and C002 = 2";
                            strSql = string.Format(strSql, strRent, strUserID);
                            optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                            break;
                    }
                }
                else if (strType == ConstValue.RESOURCE_USER.ToString())
                {
                    //用户
                    switch (session.DBType)
                    {
                        case 2:
                            strSql = "select * from t_11_101_{0} where C001 = {1} ";
                            strSql = string.Format(strSql, strRent, strUserID);
                            optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                            break;
                        case 3:
                            strSql = "select * from t_11_101_{0} where C001 = {1} ";
                            strSql = string.Format(strSql, strRent, strUserID);
                            optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                            break;
                    }
                }
                optReturn.Message += " ; " + strSql;
                if (!optReturn.Result)
                {
                    optReturn.Code = (int)S1600WcfError.CheckUserPropertyError;
                    return optReturn;
                }

                DataSet ds = optReturn.Data as DataSet;
                if (ds.Tables.Count <= 0 || ds.Tables[0].Rows.Count <= 0)
                {
                    optReturn.Result = true;
                    optReturn.Code = (int)S1600WcfError.PropertyNone;
                    return optReturn;
                }

                optReturn.Result = true;
                optReturn.Code = Defines.RET_SUCCESS;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = (int)S1600WcfError.CheckUserPropertyException;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }

        /// <summary>
        /// 根据cookie查看它的状态（是否结束）
        /// </summary>
        /// <param name="session"></param>
        /// <param name="lstParams">
        /// lstParams[0] : cookieID
        /// </param>
        /// <returns>opt.data = 0: 会话已经结束  1：会话未结束</returns>
        private OperationReturn GetCookieStatusByID(SessionInfo session, List<string> lstParams)
        {
            OperationReturn optReturn = new OperationReturn();
            try
            {
                if (lstParams == null || lstParams.Count < 1 || session == null)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_PARAM_INVALID;
                    optReturn.Message = string.Format("Request param is null or count invalid");
                    return optReturn;
                }
                string strToken = session.RentInfo.Token;
                string strSql = string.Empty;
                switch (session.DBType)
                {
                    case 2:
                        strSql = "select * from t_16_001_{0} where C001 = {1}";
                        strSql = string.Format(strSql, strToken, lstParams[0]);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        break;
                    case 3:
                        strSql = "select * from t_16_001_{0} where C001 = {1}";
                        strSql = string.Format(strSql, strToken, lstParams[0]);
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        break;
                }
                optReturn.Message += "; " + strSql;
                if (!optReturn.Result)
                {
                    optReturn.Code = (int)S1600WcfError.GetCookieStatusError;
                    return optReturn;
                }
                DataSet ds = optReturn.Data as DataSet;
                if (ds.Tables.Count <= 0 || ds.Tables[0].Rows.Count <= 0)
                {
                    optReturn.Data = "0";
                    return optReturn;
                }
                string str = ds.Tables[0].Rows[0]["C005"].ToString();
                if (string.IsNullOrEmpty(str))
                {
                    optReturn.Data = "1";
                }
                else
                {
                    optReturn.Data = "0";
                }
                optReturn.Result = true;
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = (int)S1600WcfError.GetCookieStatusException;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }

        #region Encryption and Decryption

        private string EncryptString04(string strSource)
        {
            try
            {
                return ServerAESEncryption.EncryptString(strSource, EncryptionMode.AES256V04Hex);
            }
            catch
            {
                return strSource;
            }
        }

        private string DecryptString04(string strSource)
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

        private string EncryptString03(string strSource)
        {
            try
            {
                return ServerAESEncryption.EncryptString(strSource, EncryptionMode.AES256V03Hex);
            }
            catch
            {
                return strSource;
            }
        }

        private string DecryptString03(string strSource)
        {
            try
            {
                return ServerAESEncryption.DecryptString(strSource, EncryptionMode.AES256V03Hex);
            }
            catch
            {
                return strSource;
            }
        }

        private string EncryptString02(string strSource)
        {
            try
            {
                return ServerAESEncryption.EncryptString(strSource, EncryptionMode.AES256V02Hex);
            }
            catch
            {
                return strSource;
            }
        }

        private string DecryptString02(string strSource)
        {
            try
            {
                return ServerAESEncryption.DecryptString(strSource, EncryptionMode.AES256V02Hex);
            }
            catch
            {
                return strSource;
            }
        }

        private string EncryptString01(string strSource)
        {
            try
            {
                return ServerAESEncryption.EncryptString(strSource, EncryptionMode.AES256V01Hex);
            }
            catch
            {
                return strSource;
            }
        }

        private string DecryptString01(string strSource)
        {
            try
            {
                return ServerAESEncryption.DecryptString(strSource, EncryptionMode.AES256V01Hex);
            }
            catch
            {
                return strSource;
            }
        }

        private string DecryptStringKeyIV(string strSource, string key, string iv)
        {
            try
            {
                string strReturn = string.Empty;
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                int length = strSource.Length / 2;
                byte[] byteData = new byte[length];
                for (int i = 0; i < length; i++) { byteData[i] = (byte)Convert.ToInt32(strSource.Substring(i * 2, 2), 16); }
                des.Key = UnicodeEncoding.ASCII.GetBytes(key);
                des.IV = UnicodeEncoding.ASCII.GetBytes(iv);

                MemoryStream ms = new MemoryStream();
                CryptoStream stream = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
                stream.Write(byteData, 0, byteData.Length);
                stream.FlushFinalBlock();
                strReturn = Encoding.Unicode.GetString(ms.ToArray());
                return strReturn;
            }
            catch (Exception ex)
            {
                return strSource;
            }
        }

        #endregion
    }
}
