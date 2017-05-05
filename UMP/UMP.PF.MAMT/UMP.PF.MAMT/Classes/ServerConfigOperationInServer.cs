using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UMP.PF.MAMT.WCF_ServerConfig;
using System.ServiceModel;
using PFShareClassesC;
using System.Data;

namespace UMP.PF.MAMT.Classes
{
    /// <summary>
    /// UMP服务器上的配置文件操作
    /// </summary>
    class ServerConfigOperationInServer
    {
        public static List<DBInfo> GetAllDBs(string strHost, string strPort)
        {
            List<DBInfo> lstResult = new List<DBInfo>();
            BasicHttpBinding binding = Common.CreateBasicHttpBinding(60);
            EndpointAddress adress = Common.CreateEndPoint("HTTP", strHost, strPort, "WcfServices", "Service00000");
            Service00000Client client = new Service00000Client(binding, adress);
            try
            {
                OperationDataArgs resultArgs = client.OperationMethodA(5, null);
                if (resultArgs.BoolReturn)
                {
                    DBInfo dbInfo;
                    DataRow row;
                    int iDBtype = 0;
                    string LStrVerificationCode = Common.CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104);
                    for (int i = 0; i < resultArgs.DataSetReturn.Tables[0].Rows.Count; i++)
                    {
                        row = resultArgs.DataSetReturn.Tables[0].Rows[i];
                        dbInfo = new DBInfo();
                        int.TryParse(row["DBType"].ToString(), out iDBtype);
                        dbInfo.DbType = iDBtype;
                        dbInfo.Host = EncryptionAndDecryption.EncryptDecryptString(row["ServerHost"].ToString(), LStrVerificationCode, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                        dbInfo.Port = EncryptionAndDecryption.EncryptDecryptString(row["ServerPort"].ToString(), LStrVerificationCode, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                        dbInfo.ServiceName = EncryptionAndDecryption.EncryptDecryptString(row["NameService"].ToString(), LStrVerificationCode, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                        dbInfo.LoginName = EncryptionAndDecryption.EncryptDecryptString(row["LoginID"].ToString(), LStrVerificationCode, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                        dbInfo.Password = EncryptionAndDecryption.EncryptDecryptString(row["LoginPwd"].ToString(), LStrVerificationCode, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                        lstResult.Add(dbInfo);
                    }
                }
            }
            catch
            {

            }
            finally
            {
                if (client.State == CommunicationState.Opened)
                {
                    client.Close();
                }
            }

            return lstResult;
        }

        /// <summary>
        /// 用户登录(检查密码是否正确 是否已经在另一台机器登录等)
        /// </summary>
        /// <param name="strHost"></param>
        /// <param name="strPort"></param>
        /// <param name="strUser"></param>
        /// <param name="strPwd"></param>
        /// <param name="strLoginMethod"></param>
        /// <param name="strClientMachineName"></param>
        /// <returns></returns>
        public static OperationDataArgs UserLogin(string strHost, string strPort, string strUser, string strPwd, string strLoginMethod, string strClientMachineName)
        {
            OperationDataArgs result = new OperationDataArgs();
            BasicHttpBinding binding = Common.CreateBasicHttpBinding(60);
            EndpointAddress adress = Common.CreateEndPoint("HTTP", strHost, strPort, "WcfServices", "Service00000");
            Service00000Client client = new Service00000Client(binding, adress);
            try
            {
                List<string> lstParams = new List<string>();
                string LStrVerificationCode004 = Common.CreateVerificationCode(PFShareClassesC.EncryptionAndDecryption.UMPKeyAndIVType.M004);
                lstParams.Add(EncryptionAndDecryption.EncryptDecryptString(strUser, LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004));
                lstParams.Add(EncryptionAndDecryption.EncryptDecryptString(strPwd, LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004));
                lstParams.Add(EncryptionAndDecryption.EncryptDecryptString(strLoginMethod, LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004));
                lstParams.Add(EncryptionAndDecryption.EncryptDecryptString(strClientMachineName, LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004));
                result = client.OperationMethodA(11, lstParams);
            }
            catch (Exception ex)
            {
                result.BoolReturn = false;
                result.StringReturn = ex.Message;
            }
            finally
            {
                if (client.State == CommunicationState.Opened)
                {
                    client.Close();
                }
            }
            return result;
        }

        /// <summary>
        /// 根据messageid获得对应的语言
        /// </summary>
        /// <param name="strUmpServerPort"></param>
        /// <param name="strUmpServerHost"></param>
        /// <param name="strDBHost"></param>
        /// <param name="strDBPort"></param>
        /// <param name="strMessageID"></param>
        /// <returns></returns>
        public static string GetLanguageItemInDBByMessageID(ServerInfomation UmpServer, DBInfo dbInfo, string strMessageID, string strLanCode)
        {
            string strConnString = string.Empty;
            switch (dbInfo.DbType)
            {
                case (int)Enums.DBType.MSSQL:
                    strConnString = App.CreateMSSqlConnString(dbInfo);
                    break;
                case (int)Enums.DBType.Oracle:
                    strConnString = App.CreateOracleConnString(dbInfo);
                    break;
            }
            string strMessageContent = string.Empty;
            OperationDataArgs result = new OperationDataArgs();
            BasicHttpBinding binding = Common.CreateBasicHttpBinding(60);
            EndpointAddress adress = Common.CreateEndPoint("HTTP", UmpServer.Host, UmpServer.Port, "WcfServices", "Service00000");
            Service00000Client client = new Service00000Client(binding, adress);
            try
            {
                List<string> lstParams = new List<string>();
                lstParams.Add(dbInfo.DbType.ToString());
                lstParams.Add(strConnString);
                lstParams.Add(strLanCode);
                lstParams.Add("M3");
                lstParams.Add(strMessageID);
                result = client.OperationMethodA(8, lstParams);
            }
            catch (Exception ex)
            {
                result.BoolReturn = false;
                result.StringReturn = ex.Message;
            }
            finally
            {
                if (client.State == CommunicationState.Opened)
                {
                    client.Close();
                }
            }
            if (result.BoolReturn)
            {
                if (result.DataSetReturn.Tables.Count > 0)
                {
                    if (result.DataSetReturn.Tables[0].Rows.Count > 0)
                    {
                        strMessageContent = result.DataSetReturn.Tables[0].Rows[0][4].ToString();
                    }
                }
            }
            return strMessageContent;
        }

        /// <summary>
        /// 根据ObjectID和Page获得对应的语言
        /// </summary>
        /// <param name="strUmpServerPort"></param>
        /// <param name="strUmpServerHost"></param>
        /// <param name="strDBHost"></param>
        /// <param name="strDBPort"></param>
        /// <param name="strMessageID"></param>
        /// <returns></returns>
        public static string GetLanguageItemInDBByObjectIDAndPage(ServerInfomation UmpServer, DBInfo dbInfo, string strObjectID, string strLanCode, string strPage)
        {
            string strConnString = string.Empty;
            switch (dbInfo.DbType)
            {
                case (int)Enums.DBType.MSSQL:
                    strConnString = App.CreateMSSqlConnString(dbInfo);
                    break;
                case (int)Enums.DBType.Oracle:
                    strConnString = App.CreateOracleConnString(dbInfo);
                    break;
            }
            string strMessageContent = string.Empty;
            OperationDataArgs result = new OperationDataArgs();
            BasicHttpBinding binding = Common.CreateBasicHttpBinding(60);
            EndpointAddress adress = Common.CreateEndPoint("HTTP", UmpServer.Host, UmpServer.Port, "WcfServices", "Service00000");
            Service00000Client client = new Service00000Client(binding, adress);
            try
            {
                List<string> lstParams = new List<string>();
                lstParams.Add(dbInfo.DbType.ToString());
                lstParams.Add(strConnString);
                lstParams.Add(strLanCode);
                lstParams.Add("M21");
                lstParams.Add("0");
                lstParams.Add(strPage);
                result = client.OperationMethodA(8, lstParams);
            }
            catch (Exception ex)
            {
                result.BoolReturn = false;
                result.StringReturn = ex.Message;
            }
            finally
            {
                if (client.State == CommunicationState.Opened)
                {
                    client.Close();
                }
            }
            if (result.BoolReturn)
            {
                if (result.DataSetReturn.Tables.Count > 0)
                {
                    List<DataRow> lstRows = result.DataSetReturn.Tables[0].Select("C012 = '" + strObjectID+"'").ToList();
                    if (lstRows.Count > 0)
                    {
                        strMessageContent = lstRows[0]["C005"].ToString();
                    }
                }
            }
            return strMessageContent;
        }

        public static OperationDataArgs UserLogOff()
        {
            OperationDataArgs result = new OperationDataArgs();
            BasicHttpBinding binding = Common.CreateBasicHttpBinding(60);
            EndpointAddress adress = Common.CreateEndPoint("HTTP", App.GCurrentUmpServer.Host, App.GCurrentUmpServer.Port, "WcfServices", "Service00000");
            Service00000Client client = new Service00000Client(binding, adress);
            try
            {
                List<string> lstParams = new List<string>();
                string LStrVerificationCode004 = Common.CreateVerificationCode(PFShareClassesC.EncryptionAndDecryption.UMPKeyAndIVType.M004);
                lstParams.Add(EncryptionAndDecryption.EncryptDecryptString(App.GCurrentUser.TenantID, LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004));
                lstParams.Add(EncryptionAndDecryption.EncryptDecryptString(App.GCurrentUser.UserID, LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004));
                lstParams.Add(EncryptionAndDecryption.EncryptDecryptString(App.GCurrentUser.SessionID, LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004));
                result = client.OperationMethodA(12, lstParams);
            }
            catch (Exception ex)
            {
                result.BoolReturn = false;
                result.StringReturn = ex.Message;
            }
            finally
            {
                if (client.State == CommunicationState.Opened)
                {
                    client.Close();
                }
            }
            return result;
        }
    }
}
