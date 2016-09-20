using PFShareClasses01;
using PFShareClassesS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using System.Xml;

namespace Wcf00001
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class Service00001 : IService00001
    {
        public OperationDataArgs OperationMethodA(int AIntOperationID, List<string> AListStringArgs)
        {
            OperationDataArgs LOperationReturn = new OperationDataArgs();

            try
            {
                if (AIntOperationID == 1) { LOperationReturn = OperationA01(); return LOperationReturn; }

                if (AIntOperationID == 11) { LOperationReturn = OperationA11(AListStringArgs); return LOperationReturn; }
                if (AIntOperationID == 12) { LOperationReturn = OperationA12(AListStringArgs); return LOperationReturn; }
                if (AIntOperationID == 13) { LOperationReturn = OperationA13(AListStringArgs); return LOperationReturn; }
                if (AIntOperationID == 14) { LOperationReturn = OperationA14(AListStringArgs); return LOperationReturn; }
                if (AIntOperationID == 15) { LOperationReturn = OperationA15(AListStringArgs); return LOperationReturn; }

                if (AIntOperationID == 90) { LOperationReturn = OperationA90(AListStringArgs); return LOperationReturn; }

                if (AIntOperationID == 201) { LOperationReturn = OperationA201(AListStringArgs); return LOperationReturn; }
                if (AIntOperationID == 202) { LOperationReturn = OperationA202(AListStringArgs); return LOperationReturn; }
                if (AIntOperationID == 203) { LOperationReturn = OperationA203(AListStringArgs); return LOperationReturn; }
                if (AIntOperationID == 204) { LOperationReturn = OperationA204(AListStringArgs); return LOperationReturn; }
                if (AIntOperationID == 205) { LOperationReturn = OperationA205(AListStringArgs); return LOperationReturn; }
                if (AIntOperationID == 206) { LOperationReturn = OperationA206(AListStringArgs); return LOperationReturn; }

                if (AIntOperationID == 305) { LOperationReturn = OperationA305(AListStringArgs); return LOperationReturn; }
            }
            catch (Exception ex)
            {
                LOperationReturn.BoolReturn = false;
                LOperationReturn.StringReturn = ex.ToString();
            }

            return LOperationReturn;
        }

        /// <summary>
        /// 取数据库连接参数
        /// </summary>
        /// <returns>DBID、DBType、ServerHost、ServerPort、NameService、LoginID、LoginPwd、OtherArgs、Describer、IsEnabled 以DataSet形式返回</returns>
        private OperationDataArgs OperationA01()
        {
            OperationDataArgs LOperationReturn = new OperationDataArgs();
            bool LBoolCreated = false;
            string LStrXmlFileName = string.Empty;
            string LStrVerificationCode = string.Empty;
            string LStrP03 = string.Empty;

            try
            {
                LStrXmlFileName = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                LStrXmlFileName = Path.Combine(LStrXmlFileName, @"UMP.Server\Args01.UMP.xml");
                if (!File.Exists(LStrXmlFileName))
                {
                    LOperationReturn.BoolReturn = false;
                    LOperationReturn.StringReturn = "Error01";
                    return LOperationReturn;
                }
                XmlDocument LXmlDocArgs01 = new XmlDocument();
                LXmlDocArgs01.Load(LStrXmlFileName);
                XmlNodeList LXmlNodeListDatabase = LXmlDocArgs01.SelectSingleNode("DatabaseParameters").ChildNodes;

                DataTable LDataTableDatabaseList = new DataTable();
                LDataTableDatabaseList.Columns.Add("DBID", typeof(int));
                LDataTableDatabaseList.Columns.Add("DBType", typeof(int));
                LDataTableDatabaseList.Columns.Add("ServerHost", typeof(string));
                LDataTableDatabaseList.Columns.Add("ServerPort", typeof(string));
                LDataTableDatabaseList.Columns.Add("NameService", typeof(string));
                LDataTableDatabaseList.Columns.Add("LoginID", typeof(string));
                LDataTableDatabaseList.Columns.Add("LoginPwd", typeof(string));
                LDataTableDatabaseList.Columns.Add("OtherArgs", typeof(string));
                LDataTableDatabaseList.Columns.Add("Describer", typeof(string));
                LDataTableDatabaseList.Columns.Add("IsEnabled", typeof(string));

                LStrVerificationCode = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004);

                foreach (XmlNode LXmlNodeSingleDatabase in LXmlNodeListDatabase)
                {
                    DataRow LDataRow = LDataTableDatabaseList.NewRow();
                    LDataRow.BeginEdit();
                    LDataRow["DBID"] = int.Parse(LXmlNodeSingleDatabase.Attributes["P01"].Value);
                    LDataRow["DBType"] = int.Parse(LXmlNodeSingleDatabase.Attributes["P02"].Value);
                    LDataRow["ServerHost"] = EncryptionAndDecryption.EncryptDecryptString(LXmlNodeSingleDatabase.Attributes["P04"].Value, LStrVerificationCode, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    LDataRow["ServerPort"] = EncryptionAndDecryption.EncryptDecryptString(LXmlNodeSingleDatabase.Attributes["P05"].Value, LStrVerificationCode, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    LDataRow["NameService"] = EncryptionAndDecryption.EncryptDecryptString(LXmlNodeSingleDatabase.Attributes["P06"].Value, LStrVerificationCode, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    LDataRow["LoginID"] = EncryptionAndDecryption.EncryptDecryptString(LXmlNodeSingleDatabase.Attributes["P07"].Value, LStrVerificationCode, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    LDataRow["LoginPwd"] = EncryptionAndDecryption.EncryptDecryptString(LXmlNodeSingleDatabase.Attributes["P08"].Value, LStrVerificationCode, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    LDataRow["OtherArgs"] = EncryptionAndDecryption.EncryptDecryptString(LXmlNodeSingleDatabase.Attributes["P09"].Value, LStrVerificationCode, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    LDataRow["Describer"] = LXmlNodeSingleDatabase.Attributes["P10"].Value;
                    LStrP03 = LXmlNodeSingleDatabase.Attributes["P03"].Value;
                    LDataRow["IsEnabled"] = LStrP03;
                    LDataRow.EndEdit();
                    LDataTableDatabaseList.Rows.Add(LDataRow);
                    if (LStrP03 == "1") { LBoolCreated = true; }
                }
                if (LBoolCreated) { LOperationReturn.StringReturn = "1"; } else { LOperationReturn.StringReturn = "0"; }
                LOperationReturn.DataSetReturn.Tables.Add(LDataTableDatabaseList);
            }
            catch (Exception ex)
            {
                LOperationReturn.BoolReturn = false;
                LOperationReturn.StringReturn = ex.ToString();
            }
            return LOperationReturn;
        }

        /// <summary>
        /// 获取需要创建的数据库对象
        /// </summary>
        /// <param name="AListStringArgs"></param>
        /// <returns></returns>
        private OperationDataArgs OperationA11(List<string> AListStringArgs)
        {
            OperationDataArgs LOperationReturn = new OperationDataArgs();
            LOperationReturn = DatabaseType0Operation.ObtainCreateObjects(AListStringArgs);
            return LOperationReturn;
        }

        /// <summary>
        /// 创建数据库对象
        /// </summary>
        /// <param name="AListStringArgs">
        /// 0-是否已经创建
        /// 1-数据库类型
        /// 2-数据库服务器
        /// 3-端口
        /// 4-登录名
        /// 5-登录密码
        /// 6-目标数据库
        /// 7-对象类型
        /// 8-对象路径
        /// 9-租户Token
        /// <returns></returns>
        private OperationDataArgs OperationA12(List<string> AListStringArgs)
        {
            OperationDataArgs LOperationReturn = new OperationDataArgs();
            if (AListStringArgs[0] == "1")
            {
                LOperationReturn.BoolReturn = true;
                LOperationReturn.StringReturn = AscCodeToChr(27);
                return LOperationReturn;
            }

            if (AListStringArgs[1] == "2" && AListStringArgs[7] == "T") { LOperationReturn = DatabaseType2Operation.CreateObjectTable(AListStringArgs, GetIISBaseDirectory()); }
            if (AListStringArgs[1] == "2" && AListStringArgs[7] == "F") { LOperationReturn = DatabaseType2Operation.CreateObjectBySQL(AListStringArgs, GetIISBaseDirectory()); }
            if (AListStringArgs[1] == "2" && AListStringArgs[7] == "P") { LOperationReturn = DatabaseType2Operation.CreateObjectBySQL(AListStringArgs, GetIISBaseDirectory()); }

            if (AListStringArgs[1] == "3" && AListStringArgs[7] == "T") { LOperationReturn = DatabaseType3Operation.CreateObjectTable(AListStringArgs, GetIISBaseDirectory()); }
            if (AListStringArgs[1] == "3" && AListStringArgs[7] == "F") { LOperationReturn = DatabaseType3Operation.CreateObjectBySQL(AListStringArgs, GetIISBaseDirectory()); }
            if (AListStringArgs[1] == "3" && AListStringArgs[7] == "P") { LOperationReturn = DatabaseType3Operation.CreateObjectBySQL(AListStringArgs, GetIISBaseDirectory()); }

            return LOperationReturn;
        }

        /// <summary>
        /// 获取初始化数据对象
        /// </summary>
        /// <param name="AListStringArgs"></param>
        /// <returns></returns>
        private OperationDataArgs OperationA13(List<string> AListStringArgs)
        {
            OperationDataArgs LOperationReturn = new OperationDataArgs();
            LOperationReturn = DatabaseType0Operation.ObtainInitializationData(AListStringArgs);
            return LOperationReturn;
        }

        /// <summary>
        /// 初始化表数据
        /// </summary>
        /// <param name="AListStringArgs">
        /// 0-是否已经初始化
        /// 1-数据库类型
        /// 2-数据库服务器
        /// 3-端口
        /// 4-登录名
        /// 5-登录密码
        /// 6-目标数据库
        /// 7-对象类型
        /// 8-对象路径
        /// 9-租户Token
        /// </param>
        /// <returns></returns>
        private OperationDataArgs OperationA14(List<string> AListStringArgs)
        {
            OperationDataArgs LOperationReturn = new OperationDataArgs();

            if (AListStringArgs[0] == "1")
            {
                LOperationReturn.BoolReturn = true;
                LOperationReturn.StringReturn = AscCodeToChr(27);
                return LOperationReturn;
            }
            if (AListStringArgs[1] == "2") { LOperationReturn = DatabaseType2Operation.InitTablesData(AListStringArgs, GetIISBaseDirectory()); }
            if (AListStringArgs[1] == "3") { LOperationReturn = DatabaseType3Operation.InitTablesData(AListStringArgs, GetIISBaseDirectory()); }

            return LOperationReturn;
        }

        /// <summary>
        /// 根据租户信息，更改表中的初始化数据
        /// </summary>
        /// <param name="AListStringArgs">
        /// 0-数据库类型
        /// 1-数据库服务器
        /// 2-端口
        /// 3-登录名
        /// 4-登录密码
        /// 5-数据库名或服务名
        /// 6-0租户名称
        /// 7-1租户Token
        /// 8-2超级系统管理员密码
        /// 9-3系统默认界面显示语言
        /// </param>
        /// <returns></returns>
        private OperationDataArgs OperationA15(List<string> AListStringArgs)
        {
            OperationDataArgs LOperationReturn = new OperationDataArgs();
            List<string> LListStrDatabaseProfile = new List<string>();
            List<string> LListStrRentDataSetted = new List<string>();

            string LStrXmlFileName = string.Empty;
            string LStrVerificationCode001 = string.Empty;
            string LStrVerificationCode104 = string.Empty;
            string LStrUserID19 = string.Empty;
            string LStrXmlSAPassword = string.Empty;
            string LStrA01 = string.Empty;

            try
            {
                for (int LIntLoopTemp = 0; LIntLoopTemp <= 5; LIntLoopTemp++) { LListStrDatabaseProfile.Add(AListStringArgs[LIntLoopTemp]); }
                for (int LIntLoopTemp = 6; LIntLoopTemp <= 9; LIntLoopTemp++) { LListStrRentDataSetted.Add(AListStringArgs[LIntLoopTemp]); }

                if (LListStrDatabaseProfile[0] == "2") { LOperationReturn = DatabaseType2Operation.InitTableByRentInfo(LListStrDatabaseProfile, LListStrRentDataSetted); }
                if (LListStrDatabaseProfile[0] == "3") { LOperationReturn = DatabaseType3Operation.InitTableByRentInfo(LListStrDatabaseProfile, LListStrRentDataSetted); }

                #region 修改XML文件的administrator的密码
                LStrVerificationCode001 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M001);
                LStrVerificationCode104 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104);
                LStrUserID19 = "102" + AListStringArgs[7] + "00000000001";
                LStrXmlSAPassword = EncryptionAndDecryption.EncryptDecryptString(AListStringArgs[8], LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                LStrXmlSAPassword = EncryptionAndDecryption.EncryptStringSHA512(LStrUserID19 + LStrXmlSAPassword, LStrVerificationCode001, EncryptionAndDecryption.UMPKeyAndIVType.M001);
                LStrXmlFileName = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                LStrXmlFileName = Path.Combine(LStrXmlFileName, @"UMP.Server\Args02.UMP.xml");
                XmlDocument LXmlDocArgs02 = new XmlDocument();
                LXmlDocArgs02.Load(LStrXmlFileName);
                XmlNodeList LXmlNodeListSAUsers = LXmlDocArgs02.SelectSingleNode("Parameters02").SelectSingleNode("SAUsers").ChildNodes;
                foreach (XmlNode LXmlNodeSingleUser in LXmlNodeListSAUsers)
                {
                    LStrA01 = LXmlNodeSingleUser.Attributes["A01"].Value;
                    if (LStrA01 != LStrUserID19) { continue; }
                    LXmlNodeSingleUser.Attributes["A03"].Value = LStrXmlSAPassword;
                    break;
                }
                LXmlDocArgs02.Save(LStrXmlFileName);
                #endregion
            }
            catch (Exception ex)
            {
                LOperationReturn.BoolReturn = false;
                LOperationReturn.StringReturn = "OperationA15()\n" + ex.ToString();
            }

            return LOperationReturn;
        }

        /// <summary>
        /// 向XML文件中写入数据库连接参数信息,通知 Service 00 重新获得数据库连接参数
        /// </summary>
        /// <param name="AListStringArgs">
        /// 0-数据库类型
        /// 1-数据库服务器
        /// 2-端口
        /// 3-登录名
        /// 4-登录密码
        /// 5-数据库名或服务名
        /// 6-当前版本
        /// </param>
        /// <returns></returns>
        private OperationDataArgs OperationA90(List<string> AListStringArgs)
        {
            OperationDataArgs LOperationReturn = new OperationDataArgs();
            string LStrXmlFileName = string.Empty;
            string LStrVerificationCode004 = string.Empty;
            string LStrVerificationCode104 = string.Empty;
            string LStrP02 = string.Empty;
            string LStrP04 = string.Empty, LStrP05 = string.Empty, LStrP06 = string.Empty, LStrP07 = string.Empty, LStrP08 = string.Empty;
            string LStrP09 = string.Empty;
            string LStrLoginPassword = string.Empty;

            string LStrDBConnectProfile = string.Empty;
            string LStrDynamicSQL = string.Empty;

            int LIntHttpBindingPort = 0;
            int LIntService01Port = 0;
            string LStrCallReturn = string.Empty;
            string LStrSendMessage = string.Empty;
            TcpClient LTcpClient = null;
            SslStream LSslStream = null;

            try
            {
                LStrVerificationCode004 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004);
                LStrVerificationCode104 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104);

                LStrLoginPassword = EncryptionAndDecryption.EncryptDecryptString(AListStringArgs[4], LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);

                #region 保存数据库连接信息至 UMP.Server\Args01.UMP.xml
                LStrXmlFileName = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                LStrXmlFileName = Path.Combine(LStrXmlFileName, @"UMP.Server\Args01.UMP.xml");

                XmlDocument LXmlDocArgs01 = new XmlDocument();
                LXmlDocArgs01.Load(LStrXmlFileName);
                XmlNodeList LXmlNodeListDatabaseParameters = LXmlDocArgs01.SelectSingleNode("DatabaseParameters").ChildNodes;
                foreach (XmlNode LXmlNodeSingleDatabaseParameter in LXmlNodeListDatabaseParameters)
                {
                    LXmlNodeSingleDatabaseParameter.Attributes["P03"].Value = EncryptionAndDecryption.EncryptDecryptString("0", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    LStrP02 = LXmlNodeSingleDatabaseParameter.Attributes["P02"].Value;
                    LStrP02 = EncryptionAndDecryption.EncryptDecryptString(LStrP02, LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                    if (LStrP02 != AListStringArgs[0]) { continue; }
                    LXmlNodeSingleDatabaseParameter.Attributes["P03"].Value = EncryptionAndDecryption.EncryptDecryptString("1", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    LStrP04 = EncryptionAndDecryption.EncryptDecryptString(AListStringArgs[1], LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    LStrP05 = EncryptionAndDecryption.EncryptDecryptString(AListStringArgs[2], LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    LStrP06 = EncryptionAndDecryption.EncryptDecryptString(AListStringArgs[5], LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    LStrP07 = EncryptionAndDecryption.EncryptDecryptString(AListStringArgs[3], LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    LStrP08 = EncryptionAndDecryption.EncryptDecryptString(LStrLoginPassword, LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    LStrP09 = EncryptionAndDecryption.EncryptDecryptString(AListStringArgs[6], LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);

                    LXmlNodeSingleDatabaseParameter.Attributes["P04"].Value = LStrP04;
                    LXmlNodeSingleDatabaseParameter.Attributes["P05"].Value = LStrP05;
                    LXmlNodeSingleDatabaseParameter.Attributes["P06"].Value = LStrP06;
                    LXmlNodeSingleDatabaseParameter.Attributes["P07"].Value = LStrP07;
                    LXmlNodeSingleDatabaseParameter.Attributes["P08"].Value = LStrP08;
                    LXmlNodeSingleDatabaseParameter.Attributes["P09"].Value = LStrP09;
                }
                LXmlDocArgs01.Save(LStrXmlFileName);
                #endregion

                #region 将创建的版本信息写入到 T_00_000
                string LStrSelectC000 = AscCodeToChr(27) + AscCodeToChr(27) + AscCodeToChr(27) + AscCodeToChr(27) + "1";
                if (AListStringArgs[0] == "2")
                {
                    LStrDBConnectProfile = string.Format("Data Source={0},{1};Initial Catalog={2};User Id={3};Password={4}", AListStringArgs[1], AListStringArgs[2], AListStringArgs[5], AListStringArgs[3], LStrLoginPassword);
                    LStrDynamicSQL = "INSERT INTO T_00_000(C000, C001, C002, C003, C004, C005) VALUES('" + LStrSelectC000 + "', 'CreateDB', '" + AListStringArgs[6] + "', 'CREATEDB', '1', GETUTCDATE())";
                }
                if (AListStringArgs[0] == "3")
                {
                    LStrDBConnectProfile = string.Format("Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST={0}) (PORT={1})))(CONNECT_DATA=(SERVICE_NAME= {2})));User Id={3}; Password={4}", AListStringArgs[1], AListStringArgs[2], AListStringArgs[5], AListStringArgs[3], LStrLoginPassword);
                    LStrDynamicSQL = "INSERT INTO T_00_000(C000, C001, C002, C003, C004, C005) VALUES('" + LStrSelectC000 + "', 'CreateDB', '" + AListStringArgs[6] + "', 'CREATEDB', '1', F_00_004())";
                }
                DataOperations01 LDataOperations = new DataOperations01();
                DatabaseOperation01Return LDatabaseOperation01Return = LDataOperations.SelectDataByDynamicSQL(int.Parse(AListStringArgs[0]), LStrDBConnectProfile, LStrDynamicSQL);
                if (!LDatabaseOperation01Return.BoolReturn)
                {
                    LOperationReturn.BoolReturn = false;
                    LOperationReturn.StringReturn = LDatabaseOperation01Return.StrReturn;
                    return LOperationReturn;
                }
                #endregion

                #region 通知 Service 00 重新获得数据库连接参数
                LStrSendMessage = EncryptionAndDecryption.EncryptDecryptString("M01C01", LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                foreach (string LStrSingleArgs in AListStringArgs) { LStrSendMessage += AscCodeToChr(27) + LStrSingleArgs; }
                LIntHttpBindingPort = GetIISHttpBindingPort(ref LStrCallReturn);
                if (LIntHttpBindingPort <= 0)
                {
                    LOperationReturn.BoolReturn = false;
                    LOperationReturn.StringReturn = "GetIISHttpBindingPort()\n" + LStrCallReturn;
                    return LOperationReturn;
                }
                LIntService01Port = LIntHttpBindingPort - 1;
                if (LIntService01Port == 8009) { LIntService01Port = 8008; }
                try
                {
                    LTcpClient = new TcpClient("127.0.0.1", LIntService01Port);
                    LSslStream = new SslStream(LTcpClient.GetStream(), false, new RemoteCertificateValidationCallback(ValidateServerCertificate), null);
                    LSslStream.AuthenticateAsClient("VoiceCyber.PF", null, SslProtocols.Default, false);
                    byte[] LByteMesssage = Encoding.UTF8.GetBytes(LStrSendMessage + "\r\n");
                    LSslStream.Write(LByteMesssage); LSslStream.Flush();
                }
                catch { }
                #endregion
            }
            catch (Exception ex)
            {
                LOperationReturn.BoolReturn = false;
                LOperationReturn.StringReturn = "OperationA90()\n" + ex.ToString();
            }
            finally
            {
                if (LSslStream != null) { LSslStream.Close(); }
                if (LTcpClient != null) { LTcpClient.Close(); }
            }

            return LOperationReturn;
        }

        private bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.RemoteCertificateChainErrors || sslPolicyErrors == SslPolicyErrors.None) { return true; }
            return false;
        }

        /// <summary>
        /// 连接到MS SQL Server 获取数据库服务器相关配置信息
        /// </summary>
        /// <param name="AListStringArgs"></param>
        /// <returns></returns>
        private OperationDataArgs OperationA201(List<string> AListStringArgs)
        {
            OperationDataArgs LOperationReturn = new OperationDataArgs();

            LOperationReturn = DatabaseType2Operation.ConnectToDatabase(AListStringArgs);

            return LOperationReturn;
        }

        /// <summary>
        /// 创建数据库
        /// </summary>
        /// <param name="AListStringArgs"></param>
        /// <returns></returns>
        private OperationDataArgs OperationA202(List<string> AListStringArgs)
        {
            OperationDataArgs LOperationReturn = new OperationDataArgs();

            LOperationReturn = DatabaseType2Operation.CreateDatabase(AListStringArgs);

            return LOperationReturn;
        }

        /// <summary>
        /// 创建数据库登录用户
        /// </summary>
        /// <param name="AListStringArgs"></param>
        /// <returns></returns>
        private OperationDataArgs OperationA203(List<string> AListStringArgs)
        {
            OperationDataArgs LOperationReturn = new OperationDataArgs();

            LOperationReturn = DatabaseType2Operation.CreateLoginAccount(AListStringArgs);

            return LOperationReturn;
        }

        /// <summary>
        /// 获取数据库中的用户数据库 和 登录名（LoginType = SqlLogin）
        /// </summary>
        /// <param name="AListStringArgs"></param>
        /// <returns></returns>
        private OperationDataArgs OperationA204(List<string> AListStringArgs)
        {
            OperationDataArgs LOperationReturn = new OperationDataArgs();

            LOperationReturn = DatabaseType2Operation.ObtainDatabasesLogins(AListStringArgs);

            return LOperationReturn;
        }

        /// <summary>
        /// 尝试连接到指定的MS SQL Server数据库，且获得当前T_00_000的版本
        /// </summary>
        /// <param name="AListStringArgs"></param>
        /// <returns></returns>
        private OperationDataArgs OperationA205(List<string> AListStringArgs)
        {
            OperationDataArgs LOperationReturn = new OperationDataArgs();

            LOperationReturn = DatabaseType2Operation.Connect2SpecifiedDatabaseObtainCurrentVersion(AListStringArgs);

            return LOperationReturn;
        }

        /// <summary>
        /// 连接到指定的 MS SQL Server 数据库，获取指定目录下的子目录
        /// </summary>
        /// <param name="AListStringArgs">
        /// 0～3 - 数据库连接参数
        /// 4 - 当前路径
        /// </param>
        /// <returns></returns>
        private OperationDataArgs OperationA206(List<string> AListStringArgs)
        {
            OperationDataArgs LOperationReturn = new OperationDataArgs();
            LOperationReturn = DatabaseType2Operation.GetSubdirectories(AListStringArgs);
            return LOperationReturn;
        }

        /// <summary>
        /// 尝试连接到指定的Oracle数据库，且获得当前T_00_000的版本
        /// </summary>
        /// <param name="AListStringArgs"></param>
        /// <returns></returns>
        private OperationDataArgs OperationA305(List<string> AListStringArgs)
        {
            OperationDataArgs LOperationReturn = new OperationDataArgs();

            LOperationReturn = DatabaseType3Operation.Connect2SpecifiedServiceObtainCurrentVersion(AListStringArgs);

            return LOperationReturn;
        }

        private string AscCodeToChr(int AsciiCode)
        {
            System.Text.ASCIIEncoding asciiEncoding = new System.Text.ASCIIEncoding();
            byte[] byteArray = new byte[] { (byte)AsciiCode };
            string strCharacter = asciiEncoding.GetString(byteArray);
            return (strCharacter);
        }

        /// <summary>
        /// 获取当前UMP安装的目录
        /// </summary>
        /// <returns></returns>
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

        private int GetIISHttpBindingPort(ref string AStrErrorReturn)
        {
            int LIntHttpBindingPort = 0;
            string LStrSiteBaseDirectory = string.Empty;
            string LStrXmlFileName = string.Empty;

            try
            {
                
                LStrSiteBaseDirectory = GetIISBaseDirectory();
                LStrXmlFileName = System.IO.Path.Combine(LStrSiteBaseDirectory, @"GlobalSettings\UMP.Server.01.xml");
                XmlDocument LXmlDocServer01 = new XmlDocument();
                LXmlDocServer01.Load(LStrXmlFileName);
                XmlNode LXMLNodeSection = LXmlDocServer01.SelectSingleNode("UMPSetted").SelectSingleNode("IISBindingProtocol");
                XmlNodeList LXmlNodeBindingProtocol = LXMLNodeSection.ChildNodes;
                foreach (XmlNode LXmlNodeSingleBinding in LXmlNodeBindingProtocol)
                {
                    if (LXmlNodeSingleBinding.Attributes["Protocol"].Value == "http")
                    {
                        LIntHttpBindingPort = int.Parse(LXmlNodeSingleBinding.Attributes["BindInfo"].Value);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                LIntHttpBindingPort = 0;
                AStrErrorReturn = ex.ToString();
            }

            return LIntHttpBindingPort;
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
    }
}
