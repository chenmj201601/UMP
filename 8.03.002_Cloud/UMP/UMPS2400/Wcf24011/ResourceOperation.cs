using Common2400;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;
using VoiceCyber.Common;
using VoiceCyber.UMP.Common;

namespace Wcf24011
{
    /// <summary>
    /// 操作资源表
    /// </summary>
    public class ResourceOperation
    {
        /// <summary>
        /// listParams[0] : UserID   不需要用 可为空
        /// listParams[1] : MachineResID
        /// listParams[2] : HostAddress
        /// listParams[3] : HostPort
        /// listParams[4] : IsMain
        /// </summary>
        /// <param name="listParams"></param>
        /// <returns></returns>
        public static OperationReturn AddKeyGenResource(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            try
            {
                optReturn = GetResMax(session, "286");
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                int iResMax = 0;
                int.TryParse(optReturn.Data.ToString(), out iResMax);
                if (iResMax == 0)
                {
                    iResMax = 1024;
                }
                optReturn = GetNewKey(session, "286", iResMax);
                if (!optReturn.Result)
                {
                    return optReturn;
                }
                int key = 0;
                int.TryParse(optReturn.Data.ToString(), out key);
                string str = string.Format("{0:000}", key);
                string strResID = "2860000000000000" + str;

                //写入数据库 
                List<string> lstAdd = new List<string>();
                lstAdd.Add(strResID);
                lstAdd.Add(key.ToString());
                lstAdd.Add(listParams[4]);
                lstAdd.Add(listParams[2]);
                lstAdd.Add(listParams[3]);
                lstAdd.Add(listParams[1]);
                lstAdd.Add(string.Empty);
                lstAdd.Add(string.Empty);
                UpdateData(session, lstAdd);

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
        /// 获得资源的最大有效数 
        /// </summary>
        /// <param name="session"></param>
        /// <param name="strResourceID"></param>
        /// <returns></returns>
        private static OperationReturn GetResMax(SessionInfo session, string strResourceID)
        {
            OperationReturn optReturn = new OperationReturn();
            try
            {
                string strSql = string.Empty;

                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT C009 FROM T_00_010 WHERE C001 = {0}", strResourceID);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        break;
                    case 3:
                        strSql = string.Format(string.Format("SELECT C009 FROM T_00_010 WHERE C001 = {0}", strResourceID));
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        break;
                }
                if (!optReturn.Result)
                {
                    return optReturn;
                }

                DataSet ds = optReturn.Data as DataSet;
                if (ds.Tables.Count <= 0 || ds.Tables[0].Rows.Count <= 0)
                {
                    optReturn.Result = false;
                    optReturn.Code = Defines.RET_FAIL;
                    optReturn.Message = "Resource maximum effective number error";
                    return optReturn;
                }

                optReturn.Result = true;
                optReturn.Code = Defines.RET_SUCCESS;
                optReturn.Data = ds.Tables[0].Rows[0][0].ToString();
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
        /// 获得一个没有使用的key  xml中使用 从0开始
        /// </summary>
        /// <param name="session"></param>
        /// <param name="strResourceID"></param>
        /// <returns></returns>
        private static OperationReturn GetNewKey(SessionInfo session, string strResourceID, int iResMax)
        {
            OperationReturn optReturn = new OperationReturn();
            try
            {
                string strSql = string.Empty;
                string strStartResID = strResourceID + "0000000000000000";
                string strEndResID = (int.Parse(strResourceID) + 1).ToString() + "0000000000000000";

                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT C011 FROM T_11_101_{0} WHERE C001 > {1} AND C001<{2} AND C002 = 93", session.RentInfo.Token, strStartResID, strEndResID);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        break;
                    case 3:
                        strSql = string.Format("SELECT C011 FROM T_11_101_{0} WHERE C001 > {1} AND C001<{2} AND C002 = 93", session.RentInfo.Token, strStartResID, strEndResID);
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        break;
                }
                if (!optReturn.Result)
                {
                    return optReturn;
                }

                DataSet ds = optReturn.Data as DataSet;
                if (ds.Tables.Count <= 0 || ds.Tables[0].Rows.Count <= 0)
                {
                    optReturn.Result = true;
                    optReturn.Code = Defines.RET_SUCCESS;
                    optReturn.Data = "1";
                    return optReturn;
                }

                //如果已经有了资源 那就从0开始找没有使用的key
                List<int> lstKeysExists = new List<int>();
                foreach (DataRow row in (optReturn.Data as DataSet).Tables[0].Rows)
                {
                    int key = 0;
                    int.TryParse(row[0].ToString(), out key);
                    if (!lstKeysExists.Contains(key))
                    {
                        lstKeysExists.Add(key);
                    }
                }
                for (int i = 0; i < iResMax; i++)
                {
                    if (!lstKeysExists.Contains(i+1))
                    {
                        optReturn.Result = true;
                        optReturn.Code = Defines.RET_SUCCESS;
                        optReturn.Data = i + 1;   //在key基础上加1 因为资源ID是从1开始 key从0开始
                        break;
                    }
                }
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
        /// 
        /// </summary>
        /// <param name="session"></param>
        /// <param name="listParams"></param>
        /// listParams[0] : UserID
        /// listParams[1] : MachineResID
        /// listParams[2] : HostAddress
        /// listParams[3] : HostPort
        /// listParams[4] : IsMain
        /// <returns></returns>
        public static OperationReturn ModifyKeyGenResource(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            try
            {
                string rentToken = session.RentInfo.Token;
                string strSql = string.Empty;

                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT C001,C011 FROM T_11_101_{0} WHERE C020 = '{1}' AND C002 ='1' AND C001 > 2860000000000000000 AND C001 < 2870000000000000000 ", session.RentInfo.Token, listParams[1]);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        break;
                    case 3:
                        strSql = string.Format("SELECT C001,C011 FROM T_11_101_{0} WHERE C020 = '{1}' AND C002 =1  AND C001 > 2860000000000000000 AND C001 < 2870000000000000000", session.RentInfo.Token, listParams[1]);
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        break;
                }
                if (!optReturn.Result)
                {
                    return optReturn;
                }

                DataSet ds = optReturn.Data as DataSet;
                if (ds.Tables.Count <= 0 || ds.Tables[0].Rows.Count <= 0)
                {
                    //调用添加
                    AddKeyGenResource(session, listParams);
                    return optReturn;
                }

                //如果有记录 则直接更新
                string strResID = ds.Tables[0].Rows[0][0].ToString();
                string strKey = ds.Tables[0].Rows[0][1].ToString();
                List<string> lstUpdate = new List<string>();
                lstUpdate.Add(strResID);
                lstUpdate.Add(strKey);
                lstUpdate.Add(listParams[4]);
                lstUpdate.Add(listParams[2]);
                lstUpdate.Add(listParams[3]);
                lstUpdate.Add(listParams[1]);
                lstUpdate.Add(string.Empty);
                lstUpdate.Add(string.Empty);
                UpdateData(session, lstUpdate);

                optReturn.Result = true;
                optReturn.Code = Defines.RET_SUCCESS;
                optReturn.Data = ds.Tables[0].Rows[0].ToString();
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
        /// 
        /// </summary>
        /// <param name="session"></param>
        /// <param name="listParams"></param>
        /// listParams[0] : ResID
        /// listParams[1] : Key
        /// listParams[2] : IsMain
        /// listParams[3] : HostAddress(加密前的)
        /// listParams[4] : Port(加密前的 )
        /// listParams[5] : MachineID
        /// listParams[6] : UserName
        /// listParams[7] : Pwd
        private static OperationReturn UpdateData(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            try
            {
                string rentToken = session.RentInfo.Token;
                string strSql = string.Empty;

                long errNumber = 0;
                string strErrMsg = string.Empty;
                string strErrorCode = string.Empty;
                string strErrorMsg = string.Empty;
                switch (session.DBType)
                {
                    case 2:
                        DbParameter[] mssqlParameters =
                        {
                            MssqlOperation.GetDbParameter("@AInParam00",MssqlDataType.NVarchar,1024),
                            MssqlOperation.GetDbParameter("@AInParam01",MssqlDataType.NVarchar,1024),
                            MssqlOperation.GetDbParameter("@AInParam02",MssqlDataType.Varchar,1024),
                            MssqlOperation.GetDbParameter("@AInParam03",MssqlDataType.Varchar,1024),
                            MssqlOperation.GetDbParameter("@AInparam04",MssqlDataType.Varchar,1024),
                            MssqlOperation.GetDbParameter("@AInparam05",MssqlDataType.Varchar,1024),
                            MssqlOperation.GetDbParameter("@AInparam06",MssqlDataType.Varchar,1024),
                            MssqlOperation.GetDbParameter("@AInparam07",MssqlDataType.Varchar,1024),
                            MssqlOperation.GetDbParameter("@AInparam08",MssqlDataType.Varchar,1024),
                            MssqlOperation.GetDbParameter("@aouterrornumber",MssqlDataType.Bigint,0),
                            MssqlOperation.GetDbParameter("@aouterrorstring",MssqlDataType.NVarchar,4000)
                        };

                        mssqlParameters[0].Value = session.RentInfo.Token;
                        mssqlParameters[1].Value = listParams[0];
                        mssqlParameters[2].Value = listParams[1];
                        mssqlParameters[3].Value = listParams[2];
                        mssqlParameters[4].Value = S2400EncryptOperation.EncodeEncryptValue((int)ObjectPropertyEncryptMode.E2Hex, listParams[3]);
                        mssqlParameters[5].Value = listParams[4];
                        mssqlParameters[6].Value = listParams[5];
                        mssqlParameters[7].Value = listParams[6];
                        mssqlParameters[8].Value = listParams[7];

                        mssqlParameters[9].Value = errNumber;
                        mssqlParameters[10].Value = strErrMsg;
                        mssqlParameters[9].Direction = ParameterDirection.Output;
                        mssqlParameters[10].Direction = ParameterDirection.Output;
                        optReturn = MssqlOperation.ExecuteStoredProcedure(session.DBConnectionString, "P_11_017",
                           mssqlParameters);
                        strErrorCode = mssqlParameters[9].Value.ToString();
                        strErrorMsg = mssqlParameters[10].Value.ToString();
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        break;
                    case 3:
                        DbParameter[] oracleParameters =
                        {
                            OracleOperation.GetDbParameter("@AInParam00",OracleDataType.Nvarchar2,1024),
                            OracleOperation.GetDbParameter("@AInParam01",OracleDataType.Nvarchar2,1024),
                            OracleOperation.GetDbParameter("@AInParam02",OracleDataType.Nvarchar2,1024),
                            OracleOperation.GetDbParameter("@AInparam03",OracleDataType.Nvarchar2,1024),
                            OracleOperation.GetDbParameter("@AInparam04",OracleDataType.Nvarchar2,1024),
                            OracleOperation.GetDbParameter("@AInparam05",OracleDataType.Nvarchar2,1024),
                            OracleOperation.GetDbParameter("@AInparam06",OracleDataType.Nvarchar2,1024),
                            OracleOperation.GetDbParameter("@AInparam07",OracleDataType.Nvarchar2,1024),
                            OracleOperation.GetDbParameter("@AInparam08",OracleDataType.Nvarchar2,1024),
                            OracleOperation.GetDbParameter("@aouterrornumber",OracleDataType.Int32,0),
                            OracleOperation.GetDbParameter("@aouterrorstring",OracleDataType.Nvarchar2,4000)
                        };

                        oracleParameters[0].Value = session.RentInfo.Token;
                        oracleParameters[1].Value = listParams[0];
                        oracleParameters[2].Value = listParams[1];
                        oracleParameters[3].Value = listParams[2];
                        oracleParameters[4].Value = S2400EncryptOperation.EncodeEncryptValue((int)ObjectPropertyEncryptMode.E2Hex, listParams[3]);
                        oracleParameters[5].Value = listParams[4];
                        oracleParameters[6].Value = listParams[5];
                        oracleParameters[7].Value = listParams[6];
                        oracleParameters[8].Value = listParams[7];
                        oracleParameters[9].Value = errNumber;
                        oracleParameters[10].Value = strErrMsg;
                        oracleParameters[9].Direction = ParameterDirection.Output;
                        oracleParameters[10].Direction = ParameterDirection.Output;

                        optReturn = OracleOperation.ExecuteStoredProcedure(session.DBConnectionString, "P_11_017",
                          oracleParameters);

                        strErrorCode = oracleParameters[9].Value.ToString();
                        strErrorMsg = oracleParameters[10].Value.ToString();
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        break;
                }

                if (strErrorCode != "0")
                {
                    optReturn.Message = "Excute error:" + strErrorCode + " : " + strErrorMsg;
                    optReturn.Code = Defines.RET_FAIL;
                    optReturn.Result = false;
                    return optReturn;
                }

                optReturn = SendMsgToService00(listParams[3], listParams[4]);
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
        /// 从资源表中删除资源
        /// </summary>
        /// <param name="session"></param>
        /// <param name="listParams"></param>
        /// listParams[0] : MachineID
        /// <returns></returns>
        public static OperationReturn DeleteKeyGenResource(SessionInfo session, List<string> listParams)
        {
            OperationReturn optReturn = new OperationReturn();
            try
            {
                string rentToken = session.RentInfo.Token;
                string strSql = string.Empty;

                //先获得资源ID 
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("SELECT C001 FROM T_11_101_{0} WHERE C020 = '{1}' AND C002 ='1' AND C001 >2860000000000000000 AND C001<2870000000000000000", session.RentInfo.Token, listParams[0]);
                        optReturn = MssqlOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        break;
                    case 3:
                        strSql = string.Format("SELECT C001 FROM T_11_101_{0} WHERE C020 = '{1}' AND C002 =1 AND C001 >2860000000000000000 AND C001<2870000000000000000", session.RentInfo.Token, listParams[0]);
                        optReturn = OracleOperation.GetDataSet(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        break;
                }
                if (!optReturn.Result)
                {
                    return optReturn;
                }

                DataSet ds = optReturn.Data as DataSet;
                if (ds.Tables.Count <= 0 || ds.Tables[0].Rows.Count <= 0)
                {
                    return optReturn;
                }

                //如果有记录 则删除
                string strResID = ds.Tables[0].Rows[0][0].ToString();
                switch (session.DBType)
                {
                    case 2:
                        strSql = string.Format("DELETE FROM T_11_101_{0} WHERE C001 =  {1}", session.RentInfo.Token, strResID);
                        optReturn = MssqlOperation.ExecuteSql(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        break;
                    case 3:
                        strSql = string.Format("DELETE FROM T_11_101_{0} WHERE C001 =  {1}", session.RentInfo.Token, strResID);
                        optReturn = OracleOperation.ExecuteSql(session.DBConnectionString, strSql);
                        if (!optReturn.Result)
                        {
                            return optReturn;
                        }
                        break;
                }

                optReturn = SendMsgToService00(listParams[1], listParams[2]);
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

        private static OperationReturn SendMsgToService00(string strKeyGenHost,string strKeyGenPort)
        {
            OperationReturn optReturn = new OperationReturn();
            optReturn.Code = Defines.RET_SUCCESS;
            optReturn.Result = true;
            string LStrSendMessage = string.Empty;
            TcpClient LTcpClient = null;
            SslStream LSslStream = null;
            string LStrReadMessage = string.Empty;

            try
            {
                LStrSendMessage = Common2400.S2400EncryptOperation.EncryptWithM004("R001");
                LStrSendMessage += ConstValue.SPLITER_CHAR;
                LStrSendMessage += S2400EncryptOperation.EncryptWithM004(strKeyGenHost + ":8009");

                LTcpClient = new TcpClient("127.0.0.1", 8009);
                LSslStream = new SslStream(LTcpClient.GetStream(), false, new RemoteCertificateValidationCallback(ValidateServerCertificate), null);
                LSslStream.AuthenticateAsClient("VoiceCyber.PF", null, SslProtocols.Default, false);
                byte[] LByteMesssage = Encoding.UTF8.GetBytes(LStrSendMessage + "\r\n");
                LSslStream.Write(LByteMesssage); LSslStream.Flush();
                if (ReadMessageFromServer(LSslStream, ref LStrReadMessage))
                {
                    if (LStrReadMessage.TrimEnd("\r\n".ToCharArray()) == "OK")
                    {
                        optReturn.Code = Defines.RET_SUCCESS;
                        optReturn.Result = true;
                    }
                    else
                    {
                        optReturn.Result = false;
                        optReturn.Code = Defines.RET_FAIL;
                        optReturn.Message = LStrReadMessage;
                    }
                }
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
        private static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.RemoteCertificateChainErrors || sslPolicyErrors == SslPolicyErrors.None) { return true; }
            return false;
        }

        private static bool ReadMessageFromServer(SslStream ASslStream, ref string AStrReadedMessage)
        {
            bool LBoolReturn = true;

            try
            {
                AStrReadedMessage = string.Empty;

                StringBuilder LStringBuilderData = new StringBuilder();
                int LIntReadedBytes = -1, LIntEndKeyPosition;
                byte[] LByteReadeBuffer = new byte[1024];

                do
                {
                    LIntReadedBytes = ASslStream.Read(LByteReadeBuffer, 0, LByteReadeBuffer.Length);
                    Decoder LDecoder = Encoding.UTF8.GetDecoder();
                    char[] LChars = new char[LDecoder.GetCharCount(LByteReadeBuffer, 0, LIntReadedBytes)];
                    LDecoder.GetChars(LByteReadeBuffer, 0, LIntReadedBytes, LChars, 0);
                    LStringBuilderData.Append(LChars);
                    if (LStringBuilderData.ToString().IndexOf(ConstValue.SPLITER_CHAR + "End" + ConstValue.SPLITER_CHAR) > 0) { break; }
                }
                while (LIntReadedBytes != 0);
                AStrReadedMessage = LStringBuilderData.ToString();
                LIntEndKeyPosition = AStrReadedMessage.IndexOf(ConstValue.SPLITER_CHAR + "End" + ConstValue.SPLITER_CHAR);
                if (LIntEndKeyPosition > 0)
                {
                    AStrReadedMessage = AStrReadedMessage.Substring(0, LIntEndKeyPosition);
                }
            }
            catch (Exception ex)
            {
                LBoolReturn = false;
                AStrReadedMessage = ex.ToString();
            }

            return LBoolReturn;
        }
    }
}