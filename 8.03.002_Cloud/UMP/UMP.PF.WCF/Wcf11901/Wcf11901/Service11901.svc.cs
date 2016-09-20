using Oracle.DataAccess.Client;
using PFShareClasses01;
using PFShareClassesS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;
using System.Text;
using System.Xml;

namespace Wcf11901
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class Service11901 : IService11901
    {
        public OperationDataArgs OperationMethodA(int AIntOperationID, List<string> AListStringArgs)
        {
            OperationDataArgs LOperationReturn = new OperationDataArgs();

            try
            {
                if (AIntOperationID == 1) { LOperationReturn = OperationA01(AListStringArgs); }
                if (AIntOperationID == 2) { LOperationReturn = OperationA02(AListStringArgs); }
            }
            catch (Exception ex)
            {
                LOperationReturn.BoolReturn = false;
                LOperationReturn.StringReturn = ex.ToString();
            }

            return LOperationReturn;
        }

        private OperationDataArgs OperationA01(List<string> AListStringArgs)
        {
            OperationDataArgs LOperationReturn = new OperationDataArgs();

            int LIntHttpBindingPort = 0;
            int LIntService01Port = 0;
            string LStrRemoteIPAddress = string.Empty;
            string LStrSendMessage = string.Empty;
            string LStrReadMessage = string.Empty;
            string LStrVerificationCode004 = string.Empty;

            TcpClient LTcpClient = null;
            SslStream LSslStream = null;

            string LStrCallReturn = string.Empty;

            List<string> LListStringArgs = new List<string>();

            try
            {
                LStrVerificationCode004 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004);

                #region 获取UMP Service 01端口
                LIntHttpBindingPort = GetIISHttpBindingPort(ref LStrCallReturn);
                if (LIntHttpBindingPort <= 0)
                {
                    LOperationReturn.BoolReturn = false;
                    LOperationReturn.StringReturn = LStrCallReturn;
                    return LOperationReturn;
                }
                LIntService01Port = LIntHttpBindingPort - 1;
                #endregion

                #region 获取客户端IP地址
                OperationContext LOperationContext = OperationContext.Current;
                MessageProperties LMessageProperties = LOperationContext.IncomingMessageProperties;
                RemoteEndpointMessageProperty LRemoteEndpointMessageProperty = LMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                LStrRemoteIPAddress = LRemoteEndpointMessageProperty.Address.ToString();
                #endregion

                #region 创建消息字符串
                foreach (string LStrSingleArgs in AListStringArgs) { LListStringArgs.Add(LStrSingleArgs); }
                LListStringArgs[7] = LStrRemoteIPAddress;
                LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString("M01B01", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                foreach (string LStrSingleArgs in LListStringArgs) { LStrSendMessage += AscCodeToChr(27) + LStrSingleArgs; }
                #endregion

                #region 发送消息给服务，写操作日志
                LTcpClient = new TcpClient("127.0.0.1", LIntService01Port);
                LSslStream = new SslStream(LTcpClient.GetStream(), false, new RemoteCertificateValidationCallback(ValidateServerCertificate), null);
                LSslStream.AuthenticateAsClient("VoiceCyber.PF", null, SslProtocols.Default, false);
                byte[] LByteMesssage = Encoding.UTF8.GetBytes(LStrSendMessage + "\r\n");
                LSslStream.Write(LByteMesssage); LSslStream.Flush();
                if (!ReadMessageFromServer(LSslStream, ref LStrReadMessage))
                {
                    LOperationReturn.BoolReturn = false;
                }
                LOperationReturn.StringReturn = LStrReadMessage;
                #endregion
            }
            catch (Exception ex)
            {
                LOperationReturn.BoolReturn = false;
                LOperationReturn.StringReturn = ex.ToString();
            }
            finally
            {
                if (LSslStream != null) { LSslStream.Close(); }
                if (LTcpClient != null) { LTcpClient.Close(); }

            }
            return LOperationReturn;
        }

        /// <summary>
        /// 查询操作日志
        /// </summary>
        /// <param name="AListStringArgs">
        /// 0：数据库类型
        /// 1：数据库连接串
        /// 2：租户Token
        /// 3～N：查询条件</param>
        /// <returns></returns>
        private OperationDataArgs OperationA02(List<string> AListStringArgs)
        {
            OperationDataArgs LOperationReturn = new OperationDataArgs();

            string LStrFirst4Char = string.Empty;
            string LStrAfter4Char = string.Empty;
            string LStr11901008B = string.Empty;        //开始时间 yyyy/MM/dd HH:mm:ss
            string LStr11901008E = string.Empty;        //结束时间 yyyy/MM/dd HH:mm:ss
            List<string> LListStrOperationID = new List<string>();
            List<string> LListStrUserID = new List<string>();
            List<string> LListStrOperationResult = new List<string>();
            List<string> LListStrHostName = new List<string>();
            List<string> LListStrHostIPAddress = new List<string>();

            string LStrOP911 = string.Empty, LStrUA911 = string.Empty, LStrOR911 = string.Empty, LStrHN911 = string.Empty, LStrIP911 = string.Empty;

            int LIntDBType = 0;
            string LStrDBProfile = string.Empty;
            string LStrSelectSQL = string.Empty;
            int LIntTempID = 0;

            SqlConnection LSqlConnection = null;
            OracleConnection LOracleConnection = null;

            try
            {
                LIntDBType = int.Parse(AListStringArgs[0]);
                LStrDBProfile = AListStringArgs[1];

                DataOperations01 LDataOperation = new DataOperations01();
                DatabaseOperation01Return LDBOperationReturn = new DatabaseOperation01Return();

                #region 分析日志查询条件
                foreach (string LStrSelectSingleArgs in AListStringArgs)
                {
                    if (LStrSelectSingleArgs.Length < 4) { continue; }
                    LStrFirst4Char = LStrSelectSingleArgs.Substring(0, 4);
                    LStrAfter4Char = LStrSelectSingleArgs.Substring(4);
                    if(LStrFirst4Char == AscCodeToChr(27) + "BT" + AscCodeToChr(27))
                    {
                        LStr11901008B = (DateTime.Parse(LStrAfter4Char)).ToString("yyyyMMddHHmmss");
                        continue;
                    }
                    if (LStrFirst4Char == AscCodeToChr(27) + "ET" + AscCodeToChr(27))
                    {
                        LStr11901008E = (DateTime.Parse(LStrAfter4Char)).ToString("yyyyMMddHHmmss");
                        continue;
                    }
                    if (LStrFirst4Char == AscCodeToChr(27) + "OP" + AscCodeToChr(27))
                    {
                        LListStrOperationID.Add(LStrAfter4Char); continue;
                    }
                    if (LStrFirst4Char == AscCodeToChr(27) + "UA" + AscCodeToChr(27))
                    {
                        LListStrUserID.Add(LStrAfter4Char); continue;
                    }
                    if (LStrFirst4Char == AscCodeToChr(27) + "OR" + AscCodeToChr(27))
                    {
                        LListStrOperationResult.Add(LStrAfter4Char); continue;
                    }
                    if (LStrFirst4Char == AscCodeToChr(27) + "HN" + AscCodeToChr(27))
                    {
                        LListStrHostName.Add(LStrAfter4Char); continue;
                    }
                    if (LStrFirst4Char == AscCodeToChr(27) + "IP" + AscCodeToChr(27))
                    {
                        LListStrHostIPAddress.Add(LStrAfter4Char); continue;
                    }
                }
                #endregion

                #region 保存数据查询条件
                LStrSelectSQL = "SELECT * FROM T_00_901 WHERE 1 = 2";
                LDBOperationReturn = LDataOperation.SelectDataByDynamicSQL(LIntDBType, LStrDBProfile, LStrSelectSQL);
                DataSet LDataSetSave2DB = LDBOperationReturn.DataSetReturn;
                LDataSetSave2DB.Tables[0].TableName = "T_00_901";

                if (LListStrOperationID.Count > 0)
                {
                    LIntTempID = 0;
                    LDBOperationReturn = LDataOperation.GetSerialNumberByProcedure(LIntDBType, LStrDBProfile, 11, 911, AListStringArgs[2], DateTime.UtcNow.ToString("yyyyMMddHHmmss"));
                    if (LDBOperationReturn.BoolReturn) { LStrOP911 = LDBOperationReturn.StrReturn; }
                    else { LOperationReturn.BoolReturn = false; LOperationReturn.StringReturn = LDBOperationReturn.StrReturn; return LOperationReturn; }
                    foreach (string LStrSingleOperation in LListStrOperationID)
                    {
                        LIntTempID += 1;
                        DataRow LDataRowNewOperation = LDataSetSave2DB.Tables[0].NewRow();
                        LDataRowNewOperation.BeginEdit();
                        LDataRowNewOperation["C001"] = long.Parse(LStrOP911);
                        LDataRowNewOperation["C002"] = LIntTempID;
                        LDataRowNewOperation["C011"] = LStrSingleOperation;
                        LDataRowNewOperation.EndEdit();
                        LDataSetSave2DB.Tables[0].Rows.Add(LDataRowNewOperation);
                    }
                }

                if (LListStrUserID.Count > 0)
                {
                    LIntTempID = 0;
                    LDBOperationReturn = LDataOperation.GetSerialNumberByProcedure(LIntDBType, LStrDBProfile, 11, 911, AListStringArgs[2], DateTime.UtcNow.ToString("yyyyMMddHHmmss"));
                    if (LDBOperationReturn.BoolReturn) { LStrUA911 = LDBOperationReturn.StrReturn; }
                    else { LOperationReturn.BoolReturn = false; LOperationReturn.StringReturn = LDBOperationReturn.StrReturn; return LOperationReturn; }
                    foreach (string LStrSingleUser in LListStrUserID)
                    {
                        LIntTempID += 1;
                        DataRow LDataRowNewOperation = LDataSetSave2DB.Tables[0].NewRow();
                        LDataRowNewOperation.BeginEdit();
                        LDataRowNewOperation["C001"] = long.Parse(LStrUA911);
                        LDataRowNewOperation["C002"] = LIntTempID;
                        LDataRowNewOperation["C011"] = LStrSingleUser;
                        LDataRowNewOperation.EndEdit();
                        LDataSetSave2DB.Tables[0].Rows.Add(LDataRowNewOperation);
                    }
                }

                if (LListStrOperationResult.Count > 0)
                {
                    LIntTempID = 0;
                    LDBOperationReturn = LDataOperation.GetSerialNumberByProcedure(LIntDBType, LStrDBProfile, 11, 911, AListStringArgs[2], DateTime.UtcNow.ToString("yyyyMMddHHmmss"));
                    if (LDBOperationReturn.BoolReturn) { LStrOR911 = LDBOperationReturn.StrReturn; }
                    else { LOperationReturn.BoolReturn = false; LOperationReturn.StringReturn = LDBOperationReturn.StrReturn; return LOperationReturn; }
                    foreach (string LStrSingleResult in LListStrOperationResult)
                    {
                        LIntTempID += 1;
                        DataRow LDataRowNewOperation = LDataSetSave2DB.Tables[0].NewRow();
                        LDataRowNewOperation.BeginEdit();
                        LDataRowNewOperation["C001"] = long.Parse(LStrOR911);
                        LDataRowNewOperation["C002"] = LIntTempID;
                        LDataRowNewOperation["C011"] = LStrSingleResult;
                        LDataRowNewOperation.EndEdit();
                        LDataSetSave2DB.Tables[0].Rows.Add(LDataRowNewOperation);
                    }
                }

                if (LListStrHostName.Count > 0)
                {
                    LIntTempID = 0;
                    LDBOperationReturn = LDataOperation.GetSerialNumberByProcedure(LIntDBType, LStrDBProfile, 11, 911, AListStringArgs[2], DateTime.UtcNow.ToString("yyyyMMddHHmmss"));
                    if (LDBOperationReturn.BoolReturn) { LStrHN911 = LDBOperationReturn.StrReturn; }
                    else { LOperationReturn.BoolReturn = false; LOperationReturn.StringReturn = LDBOperationReturn.StrReturn; return LOperationReturn; }
                    foreach (string LStrSingleHost in LListStrHostName)
                    {
                        LIntTempID += 1;
                        DataRow LDataRowNewOperation = LDataSetSave2DB.Tables[0].NewRow();
                        LDataRowNewOperation.BeginEdit();
                        LDataRowNewOperation["C001"] = long.Parse(LStrHN911);
                        LDataRowNewOperation["C002"] = LIntTempID;
                        LDataRowNewOperation["C011"] = LStrSingleHost;
                        LDataRowNewOperation.EndEdit();
                        LDataSetSave2DB.Tables[0].Rows.Add(LDataRowNewOperation);
                    }
                }

                if (LListStrHostIPAddress.Count > 0)
                {
                    LIntTempID = 0;
                    LDBOperationReturn = LDataOperation.GetSerialNumberByProcedure(LIntDBType, LStrDBProfile, 11, 911, AListStringArgs[2], DateTime.UtcNow.ToString("yyyyMMddHHmmss"));
                    if (LDBOperationReturn.BoolReturn) { LStrIP911 = LDBOperationReturn.StrReturn; }
                    else { LOperationReturn.BoolReturn = false; LOperationReturn.StringReturn = LDBOperationReturn.StrReturn; return LOperationReturn; }
                    foreach (string LStrSingleIPAddress in LListStrHostIPAddress)
                    {
                        LIntTempID += 1;
                        DataRow LDataRowNewOperation = LDataSetSave2DB.Tables[0].NewRow();
                        LDataRowNewOperation.BeginEdit();
                        LDataRowNewOperation["C001"] = long.Parse(LStrIP911);
                        LDataRowNewOperation["C002"] = LIntTempID;
                        LDataRowNewOperation["C011"] = LStrSingleIPAddress;
                        LDataRowNewOperation.EndEdit();
                        LDataSetSave2DB.Tables[0].Rows.Add(LDataRowNewOperation);
                    }
                }

                if (LIntDBType == 2)
                {
                    LSqlConnection = new SqlConnection(LStrDBProfile);
                    SqlDataAdapter LSqlDataAdapter = new SqlDataAdapter(LStrSelectSQL, LSqlConnection);
                    SqlCommandBuilder LSqlCommandBuilder = new SqlCommandBuilder();

                    LSqlCommandBuilder.ConflictOption = ConflictOption.OverwriteChanges;
                    LSqlCommandBuilder.SetAllValues = false;
                    LSqlCommandBuilder.DataAdapter = LSqlDataAdapter;
                    LSqlDataAdapter.Update(LDataSetSave2DB, "T_00_901");
                    LDataSetSave2DB.AcceptChanges();
                    LSqlCommandBuilder.Dispose();
                    LSqlDataAdapter.Dispose();
                }

                if (LIntDBType == 3)
                {
                    LOracleConnection = new OracleConnection(LStrDBProfile);
                    OracleDataAdapter LOracleDataAdapter = new OracleDataAdapter(LStrSelectSQL, LOracleConnection);
                    OracleCommandBuilder LOracleCommandBuilder = new OracleCommandBuilder();

                    LOracleCommandBuilder.ConflictOption = ConflictOption.OverwriteChanges;
                    LOracleCommandBuilder.SetAllValues = false;
                    LOracleCommandBuilder.DataAdapter = LOracleDataAdapter;
                    LOracleDataAdapter.Update(LDataSetSave2DB, "T_00_901");
                    LDataSetSave2DB.AcceptChanges();
                    LOracleCommandBuilder.Dispose();
                    LOracleDataAdapter.Dispose();
                }

                #endregion

                #region 创建查询SQL语句
                LStrSelectSQL = "SELECT * FROM T_11_901_" + AListStringArgs[2] + " WHERE C008 >= " + LStr11901008B + " AND C008 <= " + LStr11901008E;
                if (!string.IsNullOrEmpty(LStrOP911))
                {
                    if (LIntDBType == 2)
                    {
                        LStrSelectSQL += " AND (C004 IN (SELECT CONVERT(BIGINT, C011) FROM T_00_901 WHERE C001 = " + LStrOP911 + "))";
                    }
                    else
                    {
                        LStrSelectSQL += " AND (C004 IN (SELECT TO_NUMBER(C011) FROM T_00_901 WHERE C001 = " + LStrOP911 + "))";
                    }
                }

                if (!string.IsNullOrEmpty(LStrUA911))
                {
                    if (LIntDBType == 2)
                    {
                        LStrSelectSQL += " AND (C005 IN (SELECT CONVERT(BIGINT, C011) FROM T_00_901 WHERE C001 = " + LStrUA911 + "))";
                    }
                    else
                    {
                        LStrSelectSQL += " AND (C005 IN (SELECT TO_NUMBER(C011) FROM T_00_901 WHERE C001 = " + LStrUA911 + "))";
                    }
                }

                if (!string.IsNullOrEmpty(LStrOR911))
                {
                    LStrSelectSQL += " AND (C009 IN (SELECT C011 FROM T_00_901 WHERE C001 = " + LStrOR911 + "))";
                }

                if (!string.IsNullOrEmpty(LStrHN911))
                {
                    LStrSelectSQL += " AND (C006 IN (SELECT C011 FROM T_00_901 WHERE C001 = " + LStrHN911 + "))";
                }

                if (!string.IsNullOrEmpty(LStrIP911))
                {
                    LStrSelectSQL += " AND (C007 IN (SELECT C011 FROM T_00_901 WHERE C001 = " + LStrIP911 + "))";

                }
                #endregion

                #region 根据SQL语句查询操作日志
                LDBOperationReturn = LDataOperation.SelectDataByDynamicSQL(LIntDBType, LStrDBProfile, LStrSelectSQL);
                if (LDBOperationReturn.BoolReturn)
                {
                    LOperationReturn.DataSetReturn = LDBOperationReturn.DataSetReturn;
                    LOperationReturn.StringReturn = LDBOperationReturn.StrReturn;
                    LOperationReturn.ListStringReturn.Add(LStrSelectSQL);
                }
                else
                {
                    LOperationReturn.BoolReturn = false;
                    LOperationReturn.StringReturn = LDBOperationReturn.StrReturn;
                }
                #endregion
            }
            catch (Exception ex)
            {
                LOperationReturn.BoolReturn = false;
                LOperationReturn.StringReturn = ex.ToString();
            }
            finally
            {
                if (LSqlConnection != null)
                {
                    if (LSqlConnection.State == System.Data.ConnectionState.Open) { LSqlConnection.Close(); }
                    LSqlConnection.Dispose();
                }
                if (LOracleConnection != null)
                {
                    if (LOracleConnection.State == ConnectionState.Open) { LOracleConnection.Close(); }
                    LOracleConnection.Dispose(); LOracleConnection = null;
                }
            }

            return LOperationReturn;
        }

        private bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.RemoteCertificateChainErrors || sslPolicyErrors == SslPolicyErrors.None) { return true; }
            return false;
        }

        private bool ReadMessageFromServer(SslStream ASslStream, ref string AStrReadedMessage)
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
                    if (LStringBuilderData.ToString().IndexOf("\r\n") > 0) { break; }
                }
                while (LIntReadedBytes != 0);
                AStrReadedMessage = LStringBuilderData.ToString();
                LIntEndKeyPosition = AStrReadedMessage.IndexOf("\r\n");
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

        private string AscCodeToChr(int AsciiCode)
        {
            System.Text.ASCIIEncoding asciiEncoding = new System.Text.ASCIIEncoding();
            byte[] byteArray = new byte[] { (byte)AsciiCode };
            string strCharacter = asciiEncoding.GetString(byteArray);
            return (strCharacter);
        }

        private string CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType AKeyIVID)
        {
            string LStrReturn = string.Empty;
            int LIntRand = 0;
            string LStrTemp = string.Empty;

            try
            {
                Random LRandom = new Random();
                LStrReturn = DateTime.Now.ToString("yyyyMMddHHmmss");
                LIntRand = LRandom.Next(0, 14);
                LStrTemp = LIntRand.ToString("00");
                LStrReturn = LStrReturn.Insert(LIntRand, "VCT");
                LIntRand = LRandom.Next(0, 17);
                LStrTemp += LIntRand.ToString("00");
                LStrReturn = LStrReturn.Insert(LIntRand, "UMP");
                LIntRand = LRandom.Next(0, 20);
                LStrTemp += LIntRand.ToString("00");
                LStrReturn = LStrReturn.Insert(LIntRand, ((int)AKeyIVID).ToString("000"));

                LStrReturn = EncryptionAndDecryption.EncryptStringY(LStrTemp + LStrReturn);
            }
            catch { LStrReturn = string.Empty; }

            return LStrReturn;
        }

        private string GetIISBaseDirectory()
        {
            string LStrBaseDirectory = string.Empty;

            try
            {
                LStrBaseDirectory = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
                string[] LStrArrayDirectory = LStrBaseDirectory.Split(@"\".ToCharArray());
                LStrBaseDirectory = string.Empty;
                foreach (string LStrSingleDirectory in LStrArrayDirectory)
                {
                    LStrBaseDirectory += LStrSingleDirectory + @"\";
                    if (System.IO.Directory.Exists(LStrBaseDirectory + "GlobalSettings") && System.IO.Directory.Exists(LStrBaseDirectory + "Components") && System.IO.Directory.Exists(LStrBaseDirectory + "WcfServices")) { break; }
                }
            }
            catch { LStrBaseDirectory = string.Empty; }

            return LStrBaseDirectory;
        }

        #region 取系统当前支持的协议、及相关参数
        /// <summary>
        /// </summary>
        /// <returns>Activated、Protocol、BindInfo、IPAddress、OtherArgs、Used， 以DataSet形式返回</returns>
        private OperationDataArgs OperationA04()
        {
            OperationDataArgs LOperationReturn = new OperationDataArgs();

            string LStrSiteBaseDirectory = string.Empty;
            string LStrXmlFileName = string.Empty;

            try
            {
                DataTable LDataTableBindingProtocol = new DataTable();
                LDataTableBindingProtocol.Columns.Add("Activated", typeof(string));
                LDataTableBindingProtocol.Columns.Add("Protocol", typeof(string));
                LDataTableBindingProtocol.Columns.Add("BindInfo", typeof(string));
                LDataTableBindingProtocol.Columns.Add("IPAddress", typeof(string));
                LDataTableBindingProtocol.Columns.Add("OtherArgs", typeof(string));
                LDataTableBindingProtocol.Columns.Add("Used", typeof(string));
                LStrSiteBaseDirectory = GetIISBaseDirectory();
                LStrXmlFileName = System.IO.Path.Combine(LStrSiteBaseDirectory, @"GlobalSettings\UMP.Server.01.xml");
                XmlDocument LXmlDocServer01 = new XmlDocument();
                LXmlDocServer01.Load(LStrXmlFileName);
                XmlNode LXMLNodeSection = LXmlDocServer01.SelectSingleNode("UMPSetted").SelectSingleNode("IISBindingProtocol");
                XmlNodeList LXmlNodeBindingProtocol = LXMLNodeSection.ChildNodes;
                foreach (XmlNode LXmlNodeSingleBinding in LXmlNodeBindingProtocol)
                {
                    DataRow LDataRow = LDataTableBindingProtocol.NewRow();
                    LDataRow.BeginEdit();
                    LDataRow["Activated"] = LXmlNodeSingleBinding.Attributes["Activated"].Value;
                    LDataRow["Protocol"] = LXmlNodeSingleBinding.Attributes["Protocol"].Value;
                    LDataRow["BindInfo"] = LXmlNodeSingleBinding.Attributes["BindInfo"].Value;
                    LDataRow["IPAddress"] = LXmlNodeSingleBinding.Attributes["IPAddress"].Value;
                    LDataRow["OtherArgs"] = LXmlNodeSingleBinding.Attributes["OtherArgs"].Value;
                    LDataRow["Used"] = LXmlNodeSingleBinding.Attributes["Used"].Value;
                    LDataRow.EndEdit();
                    LDataTableBindingProtocol.Rows.Add(LDataRow);
                }
                LOperationReturn.DataSetReturn.Tables.Add(LDataTableBindingProtocol);
            }
            catch (Exception ex)
            {
                LOperationReturn.BoolReturn = false;
                LOperationReturn.StringReturn = ex.ToString();
            }

            return LOperationReturn;
        }
        #endregion

        #region 获取UMP.PF站点绑定的端口
        private int GetIISHttpBindingPort(ref string AStrErrorReturn)
        {
            int LIntHttpBindingPort = 0;

            try
            {
                OperationDataArgs LOperationDataArgs1 = OperationA04();
                DataRow[] LDataRowBindings = LOperationDataArgs1.DataSetReturn.Tables[0].Select("Protocol = 'http'");
                LIntHttpBindingPort = int.Parse(LDataRowBindings[0]["BindInfo"].ToString());
            }
            catch (Exception ex)
            {
                LIntHttpBindingPort = 0;
                AStrErrorReturn = ex.ToString();
            }
            return LIntHttpBindingPort;
        }

        #endregion
    }
}
