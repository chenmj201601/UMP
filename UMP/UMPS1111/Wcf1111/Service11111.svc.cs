using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;
using VoiceCyber.UMP.Communications;
using System.Data;
using System.Data.SqlClient;
using System.ServiceModel.Activation;
using System.Data.Common;
using Common1111;
using VoiceCyber.UMP.Encryptions;

namespace Wcf11111
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码、svc 和配置文件中的类名“Service1”。
    // 注意: 为了启动 WCF 测试客户端以测试此服务，请在解决方案资源管理器中选择 Service1.svc 或 Service1.svc.cs，然后开始调试。
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class Service11111 : IService11111
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
                    case (int)WebCodes.GetVoiceIP_Name201:
                        optReturn = GetVoiceIP_Name201(session);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)WebCodes.GetRelation:
                        optReturn = GetRelation(session);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)WebCodes.GetRentInfo:
                        optReturn = GetRentInfo(session);
                        if (!optReturn.Result)
                        {
                            webReturn.Result = false;
                            webReturn.Code = optReturn.Code;
                            webReturn.Message = optReturn.Message;
                            return webReturn;
                        }
                        webReturn.ListData = optReturn.Data as List<string>;
                        break;
                    case (int)WebCodes.Relation:
                        optReturn = Relation(session, webRequest.ListData);
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

        private OperationReturn GetRentInfo(SessionInfo session)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Result = true;
            optReturn.Code = 0;
            try
            {
                string strSql;
                DataSet objDataSet;
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT * from T_00_121");
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM T_00_121");
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
                    DateTime today = System.DateTime.Now;
                    DateTime Start = Convert.ToDateTime(DecryptString02(dr["C011"].ToString()));
                    DateTime End = Convert.ToDateTime(DecryptString02(dr["C012"].ToString()));
                    TimeSpan TS = today - Start; TimeSpan TE = End - today;
                    if (TS.TotalSeconds >= 0 && TE.TotalSeconds >= 0)
                    {
                        TenantInfo item = new TenantInfo();
                        item.RentID = Convert.ToInt64(dr["C001"].ToString());
                        item.RentName = EncryptString04(DecryptString02(dr["C002"].ToString()));
                        optReturn = XMLHelper.SeriallizeObject(item);
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

        private OperationReturn GetRelation(SessionInfo session)
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
                        strSql = string.Format("SELECT * from T_11_201_{0} where C003 LIKE '100%'", rentToken);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM T_11_201_{0} WHERE C003 LIKE '100%'", rentToken);
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
                    Relation item = new Relation();
                    item.ResourceID = Convert.ToInt64(dr["C004"].ToString());
                    item.StartTime = Convert.ToDateTime(dr["C005"].ToString());
                    item.EndTime = Convert.ToDateTime(dr["C006"].ToString());
                    item.UserID = Convert.ToInt64(dr["C003"].ToString());
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

        private OperationReturn GetVoiceIP_Name201(SessionInfo session)
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
                        strSql = string.Format("SELECT * from T_11_101_{0} where C001 like '221%'  and C002=1 ", rentToken);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        objDataSet = optReturn.Data as DataSet;
                        break;
                    case 3:
                        strSql = string.Format("SELECT * FROM T_11_101_{0} WHERE C001 LIKE '221%'  AND C002=1 ", rentToken);
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
                    ResourceInfo item = new ResourceInfo();
                    string[] arrInfo = dr["C017"].ToString().Substring(3).Split(new[] { ConstValue.SPLITER_CHAR }, StringSplitOptions.RemoveEmptyEntries);
                    if (arrInfo.Length >= 3)
                    {
                        string EnVoiceIP = arrInfo[2];//从数据库里面取出来加密的那个
                        //string strVoiceName = DecryptString(dr["C018"].ToString());
                        string strVoiceIP = DecryptString02(EnVoiceIP);
                        string strInfo = string.Format("{0}{1}", ConstValue.SPLITER_CHAR, strVoiceIP);
                        arrInfo = strInfo.Split(new[] { ConstValue.SPLITER_CHAR }, StringSplitOptions.RemoveEmptyEntries);
                        if (arrInfo.Length >= 1)
                        {
                            item.ResourceName = arrInfo[0];
                        }
                    }

                    string[] NameInfo = dr["C018"].ToString().Substring(3).Split(new[] { ConstValue.SPLITER_CHAR }, StringSplitOptions.RemoveEmptyEntries);
                    if (NameInfo.Length >= 3)
                    {
                        string EnVoiceIP = NameInfo[2];//从数据库里面取出来加密的那个
                        //string strVoiceName = DecryptString(dr["C018"].ToString());
                        string strVoiceIP = DecryptString02(EnVoiceIP);
                        string strInfo = string.Format("{0}{1}", ConstValue.SPLITER_CHAR, strVoiceIP);
                        arrInfo = strInfo.Split(new[] { ConstValue.SPLITER_CHAR }, StringSplitOptions.RemoveEmptyEntries);
                        if (arrInfo.Length >= 1)
                        {
                            item.ResourceFullName = arrInfo[0];
                        }
                    }
                    item.ResourceID = Convert.ToInt64(dr["C001"].ToString());
                    item.ResourceCode = S1111Consts.VCLogServer;

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

        private OperationReturn Relation(SessionInfo session, List<string> listParams)
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
                List<string> Users = GetAllUserID(session);
                bool IsOk = false;
                if (Users != null)
                {
                    if (Users.Count != 0)
                    { IsOk = true; }
                }
                string strUserID, strResourceID;
                string strST = string.Empty;
                string strET = string.Empty;
                //0     userID
                //1     resourceID

                //2     start time
                //3     end time
                List<string> User = new List<string>();
                User.Add(listParams[0]);
                User.Add(string.Format("102{0}00000000001", session.RentInfo.Token));
                strResourceID = listParams[1];
                if (listParams.Count == 4)
                {
                    strST = listParams[2];
                    strET = listParams[3];
                }
                string rentToken = session.RentInfo.Token;
                string strSql;
                IDbConnection objConn;
                IDbDataAdapter objAdapter;
                DbCommandBuilder objCmdBuilder;
                switch (session.DBType)
                {
                    //MSSQL
                    case 2:
                        strSql = string.Format("SELECT * FROM T_11_201_{0} WHERE C001 = 0 AND C004 = {1}"
                            , rentToken
                            , strResourceID);
                        objConn = MssqlOperation.GetConnection(session.DBConnectionString);
                        objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                        objCmdBuilder = MssqlOperation.GetCommandBuilder(objAdapter);
                        break;
                    //ORCL
                    case 3:
                        strSql = string.Format("SELECT * FROM T_11_201_{0} WHERE C001 = 0 AND C004 = {1}"
                            , rentToken
                            , strResourceID);
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
                    List<string> listMsg = new List<string>();
                    for (int i = 0; i < User.Count; i++)
                    {
                        strUserID = User[i];
                        //DataRow[] drs = objDataSet.Tables[0].Select(string.Format("C004 = {0}", strResourceID));
                        DataRow[] drs = objDataSet.Tables[0].Select(string.Format("C003={0}",strUserID));
                        //不存在，则插入
                        if (drs.Length <= 0)
                        {
                            if (listParams.Count == 4)
                            {
                                DataRow newRow = objDataSet.Tables[0].NewRow();
                                newRow["C001"] = 0;
                                newRow["C002"] = 0;
                                newRow["C003"] = Convert.ToInt64(strUserID);
                                newRow["C004"] = Convert.ToInt64(strResourceID);
                                newRow["C005"] = Convert.ToDateTime(strST);
                                newRow["C006"] = Convert.ToDateTime(strET);
                                objDataSet.Tables[0].Rows.Add(newRow.ItemArray);

                                listMsg.Add(string.Format("{0}{1}{2}", "A", ConstValue.SPLITER_CHAR, strResourceID));
                            }
                            else
                            {
                                optReturn.Result = false;
                                optReturn.Code = Defines.RET_DBACCESS_NOT_EXIST;
                                optReturn.Message = string.Format("Data is not exist");
                            }
                        }
                        else
                        {
                            if (listParams.Count == 2)
                            {
                                //存在，则移除
                                if (drs.Length > 0)
                                {
                                    for (int j = drs.Length - 1; j >= 0; j--)
                                    {
                                        drs[j].Delete();

                                        listMsg.Add(string.Format("{0}{1}{2}", "D", ConstValue.SPLITER_CHAR, strResourceID));
                                    }
                                }
                            }
                            else
                            {
                                optReturn.Result = false;
                                optReturn.Code = Defines.RET_DBACCESS_EXIST;
                                optReturn.Message = string.Format("Data is exist");
                            }
                        }
                    }
                    objAdapter.Update(objDataSet);
                    objDataSet.AcceptChanges();
                    optReturn.Data = listMsg;
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
            }
            catch (Exception ex)
            {
                optReturn.Result = false;
                optReturn.Code = Defines.RET_FAIL;
                optReturn.Message = ex.Message;
            }
            return optReturn;
        }

        private List<string> GetAllUserID(SessionInfo session)
        {
            List<string> AllUserID = new List<string>();
            string rentToken = session.RentInfo.Token;
            string strSql;
            IDbConnection objConn;
            IDbDataAdapter objAdapter;
            DbCommandBuilder objCmdBuilder;
            switch (session.DBType)
            {
                //MSSQL
                case 2:
                    strSql = string.Format("SELECT C001 FROM T_11_005_{0} WHERE C011 = 0 ORDER BY C001"
                        , rentToken);
                    objConn = MssqlOperation.GetConnection(session.DBConnectionString);
                    objAdapter = MssqlOperation.GetDataAdapter(objConn, strSql);
                    objCmdBuilder = MssqlOperation.GetCommandBuilder(objAdapter);
                    break;
                case 3:
                    strSql = string.Format("SELECT C001 FROM T_11_005_{0} WHERE C011 = 0 ORDER BY C001"
                        , rentToken);
                    objConn = OracleOperation.GetConnection(session.DBConnectionString);
                    objAdapter = OracleOperation.GetDataAdapter(objConn, strSql);
                    objCmdBuilder = OracleOperation.GetCommandBuilder(objAdapter);
                    break;
                default:
                    return AllUserID;
            }
            if (objConn == null || objAdapter == null || objCmdBuilder == null)
            {
                return AllUserID;
            }
            objCmdBuilder.ConflictOption = ConflictOption.OverwriteChanges;
            objCmdBuilder.SetAllValues = false;
            try
            {
                DataSet objDataSet = new DataSet();
                objAdapter.Fill(objDataSet);
                foreach (DataRow dr in objDataSet.Tables[0].Rows)
                {
                    AllUserID.Add(dr[0].ToString());
                }
            }
            catch (Exception EX)
            {
                return AllUserID;
            }
            return AllUserID;
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
