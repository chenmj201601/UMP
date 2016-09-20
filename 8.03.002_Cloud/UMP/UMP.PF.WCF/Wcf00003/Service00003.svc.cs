using Oracle.DataAccess.Client;
using PFShareClassesS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;
using System.Text;
using System.Xml;

namespace Wcf00003
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class Service00003 : IService00003
    {
        public OperationDataArgs OperationMethodA(int AIntOperationID, List<string> AListStringArgs)
        {
            OperationDataArgs LOperationReturn = new OperationDataArgs();

            try
            {
                if (AIntOperationID == 1) { LOperationReturn = OperationA01(AListStringArgs); }
                if (AIntOperationID == 2) { LOperationReturn = OperationA02(AListStringArgs); }
                if (AIntOperationID == 3) { LOperationReturn = OperationA03(AListStringArgs); }
                if (AIntOperationID == 4) { LOperationReturn = OperationA04(AListStringArgs); }
                if (AIntOperationID == 5) { LOperationReturn = OperationA05(AListStringArgs); }
                if (AIntOperationID == 6) { LOperationReturn = OperationA06(AListStringArgs); }
                if (AIntOperationID == 7) { LOperationReturn = OperationA07(AListStringArgs); }
                if (AIntOperationID == 8) { LOperationReturn = OperationA08(AListStringArgs); }
                if (AIntOperationID == 9) { LOperationReturn = OperationA09(AListStringArgs); }
                if (AIntOperationID == 10) { LOperationReturn = OperationA10(AListStringArgs); }
                if (AIntOperationID == 11) { LOperationReturn = OperationA11(AListStringArgs); }
            }
            catch (Exception ex)
            {
                LOperationReturn.BoolReturn = false;
                LOperationReturn.StringReturn = "WCF003E999" + AscCodeToChr(27) + "OperationMethodA\n" + ex.Message;
            }

            return LOperationReturn;
        }

        #region 01 判断客户端输入的IP地址与 GlobalSettings -- UMP.Server.01.xml中https配置的IP地址是否一样。
        /// <summary>
        /// 判断客户端输入的IP地址与 GlobalSettings\UMP.Server.01.xml中https配置的IP地址是否一样。
        /// </summary>
        /// <param name="AListStringArgs">
        /// 0：客户端输入的连接服务器的IP
        /// 1：客户端输入的端口
        /// </param>
        /// <returns>
        /// LOperationReturn.StringReturn:
        /// WCF003E001 + char(27):服务器未完成设置，即“IPAddress”，“OtherArgs”无值，Used = "0"
        /// WCF003E002 + char(27):配置的服务器IP与用户输入的连接IP不一致
        /// WCF003E003 + char(27):不允许远程连接
        /// WCF003E004 + char(27):已超过系统最大允许连接数
        /// WCF003E005 + char(27):已超过系统允许的尝试连接次数
        /// IPAddress + char(27) + OtherArgs:应用服务器IP 和 安全证书HASH值
        /// </returns>
        private OperationDataArgs OperationA01(List<string> AListStringArgs)
        {
            OperationDataArgs LOperationReturn = new OperationDataArgs();

            string LStrProgramDataFolder = string.Empty;
            string LStrSiteBaseDirectory = string.Empty;
            string LStrXmlFileName = string.Empty;

            string LStrIPAddress = string.Empty;
            string LStrOtherArgs = string.Empty;
            string LStrBindInfo = string.Empty;

            string LStrCanConnectFromRemote = string.Empty;
            string LStrAllowOnlineCount = string.Empty;
            string LStrTryConnectTimes = string.Empty;
            string LStrActiveMinutes = string.Empty;

            string LStrRemoteIPAddress = string.Empty;
            string LStrLocalIPAddress = string.Empty;

            try
            {
                CreateOrDeleteTagFile(false);
                LStrSiteBaseDirectory = GetIISBaseDirectory();
                LStrXmlFileName = System.IO.Path.Combine(LStrSiteBaseDirectory, @"GlobalSettings\UMP.Server.01.xml");
                XmlDocument LXmlDocServer01 = new XmlDocument();
                LXmlDocServer01.Load(LStrXmlFileName);

                #region 创建iTools目录，同时创建有客户端在连接的标志文件
                LStrProgramDataFolder = GetProgramDataDirectory();
                #endregion

                #region 读取绑定信息
                XmlNode LXMLNodeSection = LXmlDocServer01.SelectSingleNode("UMPSetted").SelectSingleNode("IISBindingProtocol");
                XmlNodeList LXmlNodeIISBinds = LXMLNodeSection.ChildNodes;
                foreach (XmlNode LXmlNodeSingleProtocolBind in LXmlNodeIISBinds)
                {
                    if (LXmlNodeSingleProtocolBind.Attributes["Protocol"].Value == "https")
                    {
                        if (LXmlNodeSingleProtocolBind.Attributes["Used"].Value == "0")
                        {
                            LOperationReturn.BoolReturn = false;
                            LOperationReturn.StringReturn = "WCF003E001" + AscCodeToChr(27);
                            CreateOrDeleteTagFile(true);
                            return LOperationReturn;
                        }
                        else
                        {
                            LStrBindInfo = LXmlNodeSingleProtocolBind.Attributes["BindInfo"].Value;
                            LStrIPAddress = LXmlNodeSingleProtocolBind.Attributes["IPAddress"].Value;
                            LStrOtherArgs = LXmlNodeSingleProtocolBind.Attributes["OtherArgs"].Value;
                            if (LStrIPAddress != AListStringArgs[0])
                            {
                                LOperationReturn.BoolReturn = false;
                                LOperationReturn.StringReturn = "WCF003E002" + AscCodeToChr(27);
                                CreateOrDeleteTagFile(true);
                                return LOperationReturn;
                            }
                            else
                            {
                                LOperationReturn.StringReturn = LStrIPAddress + AscCodeToChr(27) + LStrOtherArgs;
                            }
                        }
                        break;
                    }
                }
                #endregion

                #region 读取连接控制信息
                XmlNode LXMLNodeiToolConnectionLimit = LXmlDocServer01.SelectSingleNode("UMPSetted").SelectSingleNode("iToolConnectionLimit");
                if (LXMLNodeiToolConnectionLimit == null)
                {
                    XmlElement LXmlElementConnectionLimit = LXmlDocServer01.CreateElement("iToolConnectionLimit");
                    LXmlElementConnectionLimit.SetAttribute("Attribute01", "R");
                    LXmlElementConnectionLimit.SetAttribute("Attribute02", "1");
                    LXmlElementConnectionLimit.SetAttribute("Attribute03", "3");
                    LXmlElementConnectionLimit.SetAttribute("Attribute04", "20");
                    LXmlElementConnectionLimit.SetAttribute("Attribute05", "");
                    XmlNode LXMLNodeUMPSetted = LXmlDocServer01.SelectSingleNode("UMPSetted");
                    LXMLNodeUMPSetted.AppendChild(LXmlElementConnectionLimit);
                    LXmlDocServer01.Save(LStrXmlFileName);
                    LXMLNodeiToolConnectionLimit = LXmlDocServer01.SelectSingleNode("UMPSetted").SelectSingleNode("iToolConnectionLimit");
                }
                LStrCanConnectFromRemote = LXMLNodeiToolConnectionLimit.Attributes["Attribute01"].Value;
                LStrAllowOnlineCount = LXMLNodeiToolConnectionLimit.Attributes["Attribute02"].Value;
                LStrTryConnectTimes = LXMLNodeiToolConnectionLimit.Attributes["Attribute03"].Value;
                LStrActiveMinutes = LXMLNodeiToolConnectionLimit.Attributes["Attribute04"].Value;
                #endregion

                #region 判断是否允许远程连接
                LStrRemoteIPAddress = GetRemoteEndpointIPAddress();
                if (LStrCanConnectFromRemote != "R")
                {
                    LStrLocalIPAddress = GetNetworkAllIPAddress();
                    if(!LStrLocalIPAddress.Contains(LStrRemoteIPAddress + AscCodeToChr(27)))
                    {
                        LOperationReturn.BoolReturn = false; LOperationReturn.StringReturn = "WCF003E003" + AscCodeToChr(27);
                        CreateOrDeleteTagFile(true);
                        return LOperationReturn;
                    }
                }
                #endregion

                #region 判断同时在线人数
                int LIntOnlineClient = 0;
                string LStrClientActiveTime = string.Empty;
                XmlNodeList LXmlNodeListConnectedClient = LXMLNodeiToolConnectionLimit.ChildNodes;
                foreach (XmlNode LXmlNodeSingleConnectedClient in LXmlNodeListConnectedClient)
                {
                    //Attribute01:是否在线；Attribute02：登录的UserID；Attribute03：登录的机器名；Attribute04：登录的IP；Attribute05：登录后分配的流水号；Attribute06：最后心跳时间
                    if (LXmlNodeSingleConnectedClient.NodeType == XmlNodeType.Comment) { continue; }
                    if (LXmlNodeSingleConnectedClient.Attributes["Attribute01"].Value == "1")
                    {
                        LStrClientActiveTime = LXmlNodeSingleConnectedClient.Attributes["Attribute06"].Value;
                        if (DateTime.Parse(LStrClientActiveTime).AddMinutes(int.Parse(LStrActiveMinutes)) < DateTime.Now)
                        {
                            LXmlNodeSingleConnectedClient.Attributes["Attribute01"].Value = "0";
                            LXmlNodeSingleConnectedClient.Attributes["Attribute02"].Value = "";
                            LXmlNodeSingleConnectedClient.Attributes["Attribute03"].Value = "";
                            LXmlNodeSingleConnectedClient.Attributes["Attribute04"].Value = "";
                            LXmlNodeSingleConnectedClient.Attributes["Attribute05"].Value = "";
                            LXmlNodeSingleConnectedClient.Attributes["Attribute06"].Value = "";
                        }
                        else
                        {
                            LIntOnlineClient += 1;
                        }
                    }
                }
                LXmlDocServer01.Save(LStrXmlFileName);
                if (LIntOnlineClient >= int.Parse(LStrAllowOnlineCount))
                {
                    LOperationReturn.BoolReturn = false; LOperationReturn.StringReturn = "WCF003E004" + AscCodeToChr(27);
                    CreateOrDeleteTagFile(true);
                    return LOperationReturn;
                }
                #endregion

                #region 判断客户端尝试连接次数
                int LIntTryConnectedCount = 0;
                string LStrClientFileName = string.Empty;

                DirectoryInfo LDirectoryInfoiTools = new DirectoryInfo(System.IO.Path.Combine(GetProgramDataDirectory(), @"UMP.Server\iTools"));
                foreach (FileInfo LFileInfoSingleFile in LDirectoryInfoiTools.GetFiles())
                {
                    LStrClientFileName = LFileInfoSingleFile.Name;
                    if (!LStrClientFileName.Contains("C~")) { continue; }
                    if (LStrClientFileName.Substring(0, 2) != "C~") { continue; }
                    string[] LStrClientIPAndTime = LStrClientFileName.Split('~');
                    LStrClientIPAndTime[2] = LStrClientIPAndTime[2].Replace('^', ':');
                    if (LStrClientIPAndTime[1] == LStrRemoteIPAddress)
                    {
                        if (DateTime.Parse(LStrClientIPAndTime[2]).AddMinutes(30) > DateTime.Now) { LIntTryConnectedCount += 1; }
                    }
                }

                if (LIntTryConnectedCount <= 0)
                {
                    foreach (FileInfo LFileInfoSingleFile in LDirectoryInfoiTools.GetFiles())
                    {
                        LStrClientFileName = LFileInfoSingleFile.Name;
                        if (!LStrClientFileName.Contains("C~")) { continue; }
                        if (LStrClientFileName.Substring(0, 2) != "C~") { continue; }
                        string[] LStrClientIPAndTime = LStrClientFileName.Split('~');
                        if (LStrClientIPAndTime[1] == LStrRemoteIPAddress) { LFileInfoSingleFile.Delete(); }
                    }
                }

                System.IO.FileStream LFileStream = new System.IO.FileStream(System.IO.Path.Combine(GetProgramDataDirectory(), @"UMP.Server\iTools", "C~" + LStrRemoteIPAddress + "~" + DateTime.Now.ToString("yyyy-MM-dd HH^mm^ss")), System.IO.FileMode.Create, System.IO.FileAccess.ReadWrite);
                LFileStream.Close();

                if (LIntTryConnectedCount >= int.Parse(LStrTryConnectTimes))
                {
                    LOperationReturn.BoolReturn = false; LOperationReturn.StringReturn = "WCF003E005" + AscCodeToChr(27);
                    CreateOrDeleteTagFile(true);
                    return LOperationReturn;
                }
                #endregion
            }
            catch (Exception ex)
            {
                LOperationReturn.BoolReturn = false;
                LOperationReturn.StringReturn = "WCF003E999" + AscCodeToChr(27) + "OperationA01\n" + ex.Message;
                CreateOrDeleteTagFile(true);
            }

            return LOperationReturn;
        }
        #endregion

        #region 02 验证登录的用户名 和 密码
        /// <summary>
        /// 验证登录的用户名 和 密码
        /// </summary>
        /// <param name="AListStringArgs">
        /// 0:用户名
        /// 1:密码
        /// 2:机器名
        /// </param>
        /// <returns>
        /// LOperationReturn.StringReturn:
        /// WCF003E006 + char(27):用户不存在
        /// WCF003E007 + char(27):密码错误
        /// 登录流水号 + char(27) + 用户19位编码
        /// </returns>
        private OperationDataArgs OperationA02(List<string> AListStringArgs)
        {
            OperationDataArgs LOperationReturn = new OperationDataArgs();

            string LStrProgramDataFolder = string.Empty;
            string LStrSiteBaseDirectory = string.Empty;
            string LStrVerificationCode001 = string.Empty;
            string LStrVerificationCode101 = string.Empty;

            string LStrXmlFileConnect = string.Empty;
            string LStrXmlFileAccount = string.Empty;

            string LStrA01 = string.Empty, LStrA02 = string.Empty, LStrA03 = string.Empty, LStrA04 = string.Empty, LStrA05 = string.Empty, LStrA06 = string.Empty, LStrA07 = string.Empty;
            string LStrEncryptionPwd = string.Empty;

            string LStrRemoteIPAddress = string.Empty;

            try
            {
                #region 变量初始化
                LStrProgramDataFolder = GetProgramDataDirectory();
                LStrSiteBaseDirectory = GetIISBaseDirectory();
                LStrXmlFileConnect = System.IO.Path.Combine(LStrSiteBaseDirectory, @"GlobalSettings\UMP.Server.01.xml");
                LStrXmlFileAccount = System.IO.Path.Combine(LStrProgramDataFolder, @"UMP.Server\Args02.UMP.xml");

                LStrVerificationCode001 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M001);
                LStrVerificationCode101 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M101);

                LStrRemoteIPAddress = GetRemoteEndpointIPAddress();
                #endregion

                #region 查找用户 和 验证密码
                XmlDocument LXmlDocArgsAccount = new XmlDocument();
                LXmlDocArgsAccount.Load(LStrXmlFileAccount);
                XmlNodeList LXmlNodeListSAUsers = LXmlDocArgsAccount.SelectSingleNode("Parameters02").SelectSingleNode("SAUsers").ChildNodes;
                foreach (XmlNode LXmlNodeSingleUser in LXmlNodeListSAUsers)
                {
                    LStrA02 = LXmlNodeSingleUser.Attributes["A02"].Value;
                    LStrA02 = EncryptionAndDecryption.EncryptDecryptString(LStrA02, LStrVerificationCode101, EncryptionAndDecryption.UMPKeyAndIVType.M101);

                    if (LStrA02 == AListStringArgs[0])
                    {
                        LStrA01 = LXmlNodeSingleUser.Attributes["A01"].Value;
                        LStrA03 = LXmlNodeSingleUser.Attributes["A03"].Value;
                        LStrA04 = LXmlNodeSingleUser.Attributes["A04"].Value;
                        LStrA05 = LXmlNodeSingleUser.Attributes["A05"].Value;
                        LStrA06 = LXmlNodeSingleUser.Attributes["A06"].Value;
                        LStrA07 = LXmlNodeSingleUser.Attributes["A07"].Value;
                        break;
                    }
                }
                if (string.IsNullOrEmpty(LStrA01))
                {
                    LOperationReturn.BoolReturn = false;
                    LOperationReturn.StringReturn = "WCF003E006" + AscCodeToChr(27);
                    return LOperationReturn;
                }
                LStrEncryptionPwd = EncryptionAndDecryption.EncryptStringSHA512(LStrA01 + AListStringArgs[1], LStrVerificationCode001, EncryptionAndDecryption.UMPKeyAndIVType.M001);
                if (LStrEncryptionPwd != LStrA03)
                {
                    LOperationReturn.BoolReturn = false;
                    LOperationReturn.StringReturn = "WCF003E007" + AscCodeToChr(27);
                    return LOperationReturn;
                }
                #endregion

                //Attribute01:是否在线；Attribute02：登录的UserID；Attribute03：登录的机器名；Attribute04：登录的IP；Attribute05：登录后分配的流水号；Attribute06：最后心跳时间
                #region 将连接信息写入到 UMP.Server.01.xml 文件中
                bool LBoolWritedClientInfo = false;
                string LStrSessionID = string.Empty;
                string LStrLoginTime = string.Empty;

                XmlDocument LXmlDocServer01 = new XmlDocument();
                LXmlDocServer01.Load(LStrXmlFileConnect);
                XmlNode LXMLNodeiToolConnectionLimit = LXmlDocServer01.SelectSingleNode("UMPSetted").SelectSingleNode("iToolConnectionLimit");
                XmlNodeList LXmlNodeListConnectedClient = LXMLNodeiToolConnectionLimit.ChildNodes;
                foreach (XmlNode LXmlNodeSingleConnectedClient in LXmlNodeListConnectedClient)
                {
                    if (LXmlNodeSingleConnectedClient.NodeType == XmlNodeType.Comment) { continue; }
                    if (LXmlNodeSingleConnectedClient.Attributes["Attribute01"].Value == "1") { continue; }

                    LStrLoginTime = DateTime.Now.ToString("G");
                    LStrSessionID = DateTime.Parse(LStrLoginTime).ToString("yyyyMMddHHmmss");
                    LXmlNodeSingleConnectedClient.Attributes["Attribute01"].Value = "1";
                    LXmlNodeSingleConnectedClient.Attributes["Attribute02"].Value = LStrA01;
                    LXmlNodeSingleConnectedClient.Attributes["Attribute03"].Value = AListStringArgs[2];
                    LXmlNodeSingleConnectedClient.Attributes["Attribute04"].Value = LStrRemoteIPAddress;
                    LXmlNodeSingleConnectedClient.Attributes["Attribute05"].Value = LStrSessionID;
                    LXmlNodeSingleConnectedClient.Attributes["Attribute06"].Value = LStrLoginTime;

                    LBoolWritedClientInfo = true;
                    break;
                }
                if (!LBoolWritedClientInfo)
                {
                    XmlElement LXmlElementConnectedClient = LXmlDocServer01.CreateElement("ConnectedClient");
                    LStrLoginTime = DateTime.Now.ToString("G");
                    LStrSessionID = DateTime.Parse(LStrLoginTime).ToString("yyyyMMddHHmmss");
                    LXmlElementConnectedClient.SetAttribute("Attribute01", "1");
                    LXmlElementConnectedClient.SetAttribute("Attribute02", LStrA01);
                    LXmlElementConnectedClient.SetAttribute("Attribute03", AListStringArgs[2]);
                    LXmlElementConnectedClient.SetAttribute("Attribute04", LStrRemoteIPAddress);
                    LXmlElementConnectedClient.SetAttribute("Attribute05", LStrSessionID);
                    LXmlElementConnectedClient.SetAttribute("Attribute06", LStrLoginTime);
                    LXMLNodeiToolConnectionLimit.AppendChild(LXmlElementConnectedClient);
                }
                LXmlDocServer01.Save(LStrXmlFileConnect);
                #endregion

                #region 登录成功后，删除客户端尝试连接信息
                string LStrClientFileName = string.Empty;
                DirectoryInfo LDirectoryInfoiTools = new DirectoryInfo(System.IO.Path.Combine(GetProgramDataDirectory(), @"UMP.Server\iTools"));
                foreach (FileInfo LFileInfoSingleFile in LDirectoryInfoiTools.GetFiles())
                {
                    LStrClientFileName = LFileInfoSingleFile.Name;
                    if (!LStrClientFileName.Contains("C~")) { continue; }
                    if (LStrClientFileName.Substring(0, 2) != "C~") { continue; }
                    string[] LStrClientIPAndTime = LStrClientFileName.Split('~');
                    if (LStrClientIPAndTime[1] == LStrRemoteIPAddress) { LFileInfoSingleFile.Delete(); }
                }
                #endregion

                LOperationReturn.StringReturn = LStrSessionID + AscCodeToChr(27) + LStrA01;
            }
            catch (Exception ex)
            {
                LOperationReturn.BoolReturn = false;
                LOperationReturn.StringReturn = "WCF003E999" + AscCodeToChr(27) + "OperationA02\n" + ex.Message;
            }
            finally
            {
                CreateOrDeleteTagFile(true);
            }

            return LOperationReturn;
        }
        #endregion

        #region 03 获取数据库连接参数,并判断数据库是否能够正常连接
        /// <summary>
        /// 获取数据库连接参数,并判断数据库是否能够正常连接
        /// </summary>
        /// <param name="AListStringArgs">午</param>
        /// <returns>
        /// WCF003E008 + char(27):文件不存在数据库配置
        /// WCF003E009 + char(27):数据库未创建
        /// WCF003E010 + char(27):连接数据库失败
        /// WCF003E011 + char(27):数据库连接成功，但数据库不是 UMP 数据库
        /// WCF003E012 + char(27):数据库连接成功，合法的 UMP 数据库
        /// LOperationReturn.DataSetReturn.Tables[0]
        /// </returns>
        private OperationDataArgs OperationA03(List<string> AListStringArgs)
        {
            OperationDataArgs LOperationReturn = new OperationDataArgs();

            DataTable LDataTableReturn = new DataTable();

            string LStrXmlFileName = string.Empty;
            string LStrVerificationCode104 = string.Empty;
            string LStrP03 = string.Empty;

            SqlConnection LSqlConnection = null;
            OracleConnection LOracleConnection = null;

            List<string> LlistStrDatabaseProfile = new List<string>();
            string LStrConnectParam = string.Empty;

            string LStrDynamicSQL = string.Empty;
            string LStrSelectC000 = AscCodeToChr(27) + AscCodeToChr(27) + AscCodeToChr(27) + AscCodeToChr(27) + "1";
            DataSet LDataSetSelectReturn = new DataSet();
            string LStrDatabaseVersion = string.Empty;
            int LIntSelectedReturn = 0;

            string LStrVerificationCode004 = string.Empty;
            string LStrVerificationCode102 = string.Empty;

            try
            {
                LStrVerificationCode104 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M104);

                #region 获取数据库连接信息
                LStrXmlFileName = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                LStrXmlFileName = Path.Combine(LStrXmlFileName, @"UMP.Server\Args01.UMP.xml");
                if (!File.Exists(LStrXmlFileName))
                {
                    LOperationReturn.BoolReturn = false;
                    LOperationReturn.StringReturn = "WCF003E008" + AscCodeToChr(27);
                    return LOperationReturn;
                }

                XmlDocument LXmlDocArgs01 = new XmlDocument();
                LXmlDocArgs01.Load(LStrXmlFileName);
                XmlNodeList LXmlNodeListDatabase = LXmlDocArgs01.SelectSingleNode("DatabaseParameters").ChildNodes;

                LDataTableReturn.Columns.Add("DBID", typeof(int));
                LDataTableReturn.Columns.Add("DBType", typeof(int));
                LDataTableReturn.Columns.Add("ServerHost", typeof(string));
                LDataTableReturn.Columns.Add("ServerPort", typeof(string));
                LDataTableReturn.Columns.Add("NameService", typeof(string));
                LDataTableReturn.Columns.Add("LoginID", typeof(string));
                LDataTableReturn.Columns.Add("LoginPwd", typeof(string));
                LDataTableReturn.Columns.Add("OtherArgs", typeof(string));
                LDataTableReturn.Columns.Add("Describer", typeof(string));
                LDataTableReturn.Columns.Add("CanConnect", typeof(string));
                LDataTableReturn.Columns.Add("DatabaseVersion", typeof(string));

                foreach (XmlNode LXmlNodeSingleDatabase in LXmlNodeListDatabase)
                {
                    LStrP03 = LXmlNodeSingleDatabase.Attributes["P03"].Value;
                    LStrP03 = EncryptionAndDecryption.EncryptDecryptString(LStrP03, LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                    if (LStrP03 != "1") { continue; }

                    DataRow LDataRow = LDataTableReturn.NewRow();
                    LDataRow.BeginEdit();
                    LDataRow["DBID"] = int.Parse(LXmlNodeSingleDatabase.Attributes["P01"].Value);
                    LDataRow["DBType"] = int.Parse(LXmlNodeSingleDatabase.Attributes["P02"].Value);
                    LDataRow["ServerHost"] = EncryptionAndDecryption.EncryptDecryptString(LXmlNodeSingleDatabase.Attributes["P04"].Value, LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                    LDataRow["ServerPort"] = EncryptionAndDecryption.EncryptDecryptString(LXmlNodeSingleDatabase.Attributes["P05"].Value, LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                    LDataRow["NameService"] = EncryptionAndDecryption.EncryptDecryptString(LXmlNodeSingleDatabase.Attributes["P06"].Value, LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                    LDataRow["LoginID"] = EncryptionAndDecryption.EncryptDecryptString(LXmlNodeSingleDatabase.Attributes["P07"].Value, LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                    LDataRow["LoginPwd"] = EncryptionAndDecryption.EncryptDecryptString(LXmlNodeSingleDatabase.Attributes["P08"].Value, LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                    LDataRow["OtherArgs"] = EncryptionAndDecryption.EncryptDecryptString(LXmlNodeSingleDatabase.Attributes["P09"].Value, LStrVerificationCode104, EncryptionAndDecryption.UMPKeyAndIVType.M104);
                    LDataRow["Describer"] = LXmlNodeSingleDatabase.Attributes["P10"].Value;
                    LDataRow["CanConnect"] = "0";
                    LDataRow["DatabaseVersion"] = "0.00.000";
                    LDataRow.EndEdit();
                    LDataTableReturn.Rows.Add(LDataRow);
                }
                if (LDataTableReturn.Rows.Count == 0)
                {
                    LOperationReturn.BoolReturn = false;
                    LOperationReturn.StringReturn = "WCF003E009" + AscCodeToChr(27);
                    LOperationReturn.DataSetReturn.Tables.Add(LDataTableReturn);
                    return LOperationReturn;
                }
                #endregion

                #region 尝试连接到数据库，并判断是否为UMP数据库
                LlistStrDatabaseProfile.Add(LDataTableReturn.Rows[0]["DBType"].ToString());             //0
                LlistStrDatabaseProfile.Add(LDataTableReturn.Rows[0]["ServerHost"].ToString());         //1
                LlistStrDatabaseProfile.Add(LDataTableReturn.Rows[0]["ServerPort"].ToString());         //2
                LlistStrDatabaseProfile.Add(LDataTableReturn.Rows[0]["LoginID"].ToString());            //3
                LlistStrDatabaseProfile.Add(LDataTableReturn.Rows[0]["LoginPwd"].ToString());           //4
                LlistStrDatabaseProfile.Add(LDataTableReturn.Rows[0]["NameService"].ToString());        //5

                LStrDynamicSQL = "SELECT C002 FROM T_00_000 WHERE C000 = '" + LStrSelectC000 + "'";
                LStrDatabaseVersion = "0.00.000";

                #region 连接到MS SQL Server
                if (LlistStrDatabaseProfile[0] == "2")
                {
                    try
                    {
                        LStrConnectParam = string.Format("Data Source={0},{1};Initial Catalog={2};User Id={3};Password={4}", LlistStrDatabaseProfile[1], LlistStrDatabaseProfile[2], LlistStrDatabaseProfile[5], LlistStrDatabaseProfile[3], LlistStrDatabaseProfile[4]);
                        LSqlConnection = new SqlConnection(LStrConnectParam);
                        LSqlConnection.Open();
                        SqlDataAdapter LSqlDataAdapter = new SqlDataAdapter(LStrDynamicSQL, LSqlConnection);
                        LIntSelectedReturn = LSqlDataAdapter.Fill(LDataSetSelectReturn);
                        LSqlDataAdapter.Dispose();
                        LDataTableReturn.Rows[0]["CanConnect"] = "1";
                        if (LIntSelectedReturn != 1)
                        {
                            LOperationReturn.BoolReturn = false;
                            LOperationReturn.StringReturn = "WCF003E011" + AscCodeToChr(27);
                            return LOperationReturn;
                        }
                        LStrDatabaseVersion = LDataSetSelectReturn.Tables[0].Rows[0][0].ToString();
                        LDataTableReturn.Rows[0]["DatabaseVersion"] = LStrDatabaseVersion;
                        LOperationReturn.StringReturn = "WCF003E012" + AscCodeToChr(27);
                    }
                    catch
                    {
                        LOperationReturn.BoolReturn = false;
                        LOperationReturn.StringReturn = "WCF003E010" + AscCodeToChr(27);
                        return LOperationReturn;
                    }
                    finally
                    {
                        LOperationReturn.DataSetReturn.Tables.Add(LDataTableReturn);
                    }
                }
                #endregion

                #region 连接到 Oracle
                if (LlistStrDatabaseProfile[0] == "3")
                {
                    try
                    {
                        LStrConnectParam = string.Format("Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST={0}) (PORT={1})))(CONNECT_DATA=(SERVICE_NAME= {2})));User Id={3}; Password={4}", LlistStrDatabaseProfile[1], LlistStrDatabaseProfile[2], LlistStrDatabaseProfile[5], LlistStrDatabaseProfile[3], LlistStrDatabaseProfile[4]);
                        LOracleConnection = new OracleConnection(LStrConnectParam);
                        LOracleConnection.Open();
                        OracleDataAdapter LOracleDataAdapter = new OracleDataAdapter(LStrDynamicSQL, LOracleConnection);
                        LIntSelectedReturn = LOracleDataAdapter.Fill(LDataSetSelectReturn);
                        LOracleDataAdapter.Dispose();
                        LDataTableReturn.Rows[0]["CanConnect"] = "1";
                        if (LIntSelectedReturn != 1)
                        {
                            LOperationReturn.BoolReturn = false;
                            LOperationReturn.StringReturn = "WCF003E011" + AscCodeToChr(27);
                            return LOperationReturn;
                        }
                        LStrDatabaseVersion = LDataSetSelectReturn.Tables[0].Rows[0][0].ToString();
                        LDataTableReturn.Rows[0]["DatabaseVersion"] = LStrDatabaseVersion;
                        LOperationReturn.StringReturn = "WCF003E012" + AscCodeToChr(27);
                    }
                    catch
                    {
                        LOperationReturn.BoolReturn = false;
                        LOperationReturn.StringReturn = "WCF003E010" + AscCodeToChr(27);
                        return LOperationReturn;
                    }
                    finally
                    {
                        LOperationReturn.DataSetReturn.Tables.Add(LDataTableReturn);
                    }
                }
                #endregion

                #endregion

                #region 读取支持的语言包列表
                if (LStrDatabaseVersion != "0.00.000")
                {
                    LStrVerificationCode004 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    LStrVerificationCode102 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M102);

                    DataSet LDataSetSupportLanguages = new DataSet();
                    LStrDynamicSQL = "SELECT * FROM T_00_004 ORDER BY C002 ASC";
                    if (LlistStrDatabaseProfile[0] == "2")
                    {
                        SqlDataAdapter LSqlDataAdapter = new SqlDataAdapter(LStrDynamicSQL, LSqlConnection);
                        LSqlDataAdapter.Fill(LDataSetSupportLanguages);
                        LSqlDataAdapter.Dispose();
                    }
                    if (LlistStrDatabaseProfile[0] == "3")
                    {
                        OracleDataAdapter LOracleDataAdapter = new OracleDataAdapter(LStrDynamicSQL, LOracleConnection);
                        LOracleDataAdapter.Fill(LDataSetSupportLanguages);
                        LOracleDataAdapter.Dispose();
                    }
                    string LStr00004003 = string.Empty;
                    foreach (DataRow LDataRowSingleLanguage in LDataSetSupportLanguages.Tables[0].Rows)
                    {
                        LStr00004003 = LDataRowSingleLanguage["C003"].ToString();
                        LStr00004003 = EncryptionAndDecryption.EncryptDecryptString(LStr00004003, LStrVerificationCode102, EncryptionAndDecryption.UMPKeyAndIVType.M102);
                        LDataRowSingleLanguage["C003"] = EncryptionAndDecryption.EncryptDecryptString(LStr00004003, LStrVerificationCode004, EncryptionAndDecryption.UMPKeyAndIVType.M004);
                    }

                    LOperationReturn.ListDataSetReturn.Add(LDataSetSupportLanguages);
                }
                #endregion
            }
            catch (Exception ex)
            {
                LOperationReturn.BoolReturn = false;
                LOperationReturn.StringReturn = "WCF003E999" + AscCodeToChr(27) + "OperationA03\n" + ex.Message;
            }
            finally
            {
                if (LSqlConnection != null)
                {
                    if (LSqlConnection.State == ConnectionState.Open) { LSqlConnection.Close(); }
                    LSqlConnection.Dispose(); LSqlConnection = null;
                }
                if (LOracleConnection != null)
                {
                    if (LOracleConnection.State == ConnectionState.Open) { LOracleConnection.Close(); }
                    LOracleConnection.Dispose(); LOracleConnection = null;
                }
            }

            return LOperationReturn;
        }
        #endregion

        #region 04 客户端发送心跳消息 / 断开与应用服务器的连接
        /// <summary>
        /// 客户端发送心跳消息 / 断开与应用服务器的连接，记录在 UMP.Server.01.xml 中
        /// </summary>
        /// <param name="AListStringArgs">
        /// 0：登录成功后分配的SessionID
        /// 1：H-心跳；D-断开连接
        /// </param>
        /// <returns></returns>
        private OperationDataArgs OperationA04(List<string> AListStringArgs)
        {
            OperationDataArgs LOperationReturn = new OperationDataArgs();

            string LStrSiteBaseDirectory = string.Empty;
            string LStrXmlFileConnect = string.Empty;
            string LStrSessionID = string.Empty;
            string LStrHeartbeatTime = string.Empty;

            try
            {
                CreateOrDeleteTagFile(false);
                LStrSiteBaseDirectory = GetIISBaseDirectory();
                LStrXmlFileConnect = System.IO.Path.Combine(LStrSiteBaseDirectory, @"GlobalSettings\UMP.Server.01.xml");
                XmlDocument LXmlDocServer01 = new XmlDocument();
                LXmlDocServer01.Load(LStrXmlFileConnect);
                XmlNode LXMLNodeiToolConnectionLimit = LXmlDocServer01.SelectSingleNode("UMPSetted").SelectSingleNode("iToolConnectionLimit");
                XmlNodeList LXmlNodeListConnectedClient = LXMLNodeiToolConnectionLimit.ChildNodes;
                foreach (XmlNode LXmlNodeSingleConnectedClient in LXmlNodeListConnectedClient)
                {
                    if (LXmlNodeSingleConnectedClient.NodeType == XmlNodeType.Comment) { continue; }
                    if (LXmlNodeSingleConnectedClient.Attributes["Attribute01"].Value != "1") { continue; }
                    LStrSessionID = LXmlNodeSingleConnectedClient.Attributes["Attribute05"].Value;
                    if (LStrSessionID != AListStringArgs[0]) { continue; }
                    if (AListStringArgs[1] == "H")
                    {
                        LStrHeartbeatTime = DateTime.Now.ToString("G");
                        LXmlNodeSingleConnectedClient.Attributes["Attribute06"].Value = LStrHeartbeatTime;
                    }
                    else
                    {
                        LXmlNodeSingleConnectedClient.Attributes["Attribute01"].Value = "0";
                        LXmlNodeSingleConnectedClient.Attributes["Attribute02"].Value = "";
                        LXmlNodeSingleConnectedClient.Attributes["Attribute03"].Value = "";
                        LXmlNodeSingleConnectedClient.Attributes["Attribute04"].Value = "";
                        LXmlNodeSingleConnectedClient.Attributes["Attribute05"].Value = "";
                        LXmlNodeSingleConnectedClient.Attributes["Attribute06"].Value = "";
                    }
                    break;
                }
                LXmlDocServer01.Save(LStrXmlFileConnect);
            }
            catch (Exception ex)
            {
                LOperationReturn.BoolReturn = false;
                LOperationReturn.StringReturn = "WCF003E999" + AscCodeToChr(27) + "OperationA04\n" + ex.Message;
            }
            finally
            {
                CreateOrDeleteTagFile(true);
            }

            return LOperationReturn;
        }
        #endregion

        #region 05 从数据库中读取语言包/启用、禁用语言包
        /// <summary>
        /// 从数据库中读取支持的语言/指定的语言编码的语言包
        /// </summary>
        /// <param name="AListStringArgs">
        /// 0：00000-支持的语言；2052、1033等 指定的语言包编码
        /// 1：数据库类型
        /// 2：数据库服务器
        /// 3：端口
        /// 4：登录名
        /// 5：密码
        /// 6：数据库名或服务名
        /// 7：R-读取；E-启用；D-禁用
        /// </param>
        /// <returns></returns>
        private OperationDataArgs OperationA05(List<string> AListStringArgs)
        {
            OperationDataArgs LOperationReturn = new OperationDataArgs();

            SqlConnection LSqlConnection = null;
            SqlCommand LSqlCommand = null;
            OracleConnection LOracleConnection = null;
            OracleCommand LOracleCommand = null;

            string LStrConnectParam = string.Empty;
            string LStrDynamicSQL = string.Empty;
            DataSet LDataSetSelectReturn = new DataSet();

            List<string> LlistStrDatabaseProfile = new List<string>();

            string LStrVerificationCode004 = string.Empty;
            string LStrVerificationCode102 = string.Empty;

            try
            {
                LStrVerificationCode004 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M004);
                LStrVerificationCode102 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M102);

                for (int LIntLoopList = 1; LIntLoopList <= 6; LIntLoopList++)
                {
                    LlistStrDatabaseProfile.Add(AListStringArgs[LIntLoopList]);
                }

                if (AListStringArgs[7] == "R")
                {
                    LStrDynamicSQL = "SELECT * FROM T_00_005 WHERE C001 = " + AListStringArgs[0] + " ORDER BY C002 ASC";

                    if (LlistStrDatabaseProfile[0] == "2")
                    {
                        LStrConnectParam = string.Format("Data Source={0},{1};Initial Catalog={2};User Id={3};Password={4}", LlistStrDatabaseProfile[1], LlistStrDatabaseProfile[2], LlistStrDatabaseProfile[5], LlistStrDatabaseProfile[3], LlistStrDatabaseProfile[4]);
                        LSqlConnection = new SqlConnection(LStrConnectParam);
                        LSqlConnection.Open();
                        SqlDataAdapter LSqlDataAdapter = new SqlDataAdapter(LStrDynamicSQL, LSqlConnection);
                        LSqlDataAdapter.Fill(LDataSetSelectReturn);
                        LSqlDataAdapter.Dispose();

                    }
                    if (LlistStrDatabaseProfile[0] == "3")
                    {
                        LStrConnectParam = string.Format("Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST={0}) (PORT={1})))(CONNECT_DATA=(SERVICE_NAME= {2})));User Id={3}; Password={4}", LlistStrDatabaseProfile[1], LlistStrDatabaseProfile[2], LlistStrDatabaseProfile[5], LlistStrDatabaseProfile[3], LlistStrDatabaseProfile[4]);
                        LOracleConnection = new OracleConnection(LStrConnectParam);
                        LOracleConnection.Open();
                        OracleDataAdapter LOracleDataAdapter = new OracleDataAdapter(LStrDynamicSQL, LOracleConnection);
                        LOracleDataAdapter.Fill(LDataSetSelectReturn);
                        LOracleDataAdapter.Dispose();
                    }

                    LOperationReturn.DataSetReturn = LDataSetSelectReturn;
                }
                if (AListStringArgs[7] == "E" || AListStringArgs[7] == "D")
                {
                    if (AListStringArgs[7] == "E") { LStrDynamicSQL = "UPDATE T_00_004 SET C006 = '1' WHERE C001 = " + AListStringArgs[0]; }
                    if (AListStringArgs[7] == "D") { LStrDynamicSQL = "UPDATE T_00_004 SET C006 = '0' WHERE C001 = " + AListStringArgs[0]; }
                    if (LlistStrDatabaseProfile[0] == "2")
                    {
                        LStrConnectParam = string.Format("Data Source={0},{1};Initial Catalog={2};User Id={3};Password={4}", LlistStrDatabaseProfile[1], LlistStrDatabaseProfile[2], LlistStrDatabaseProfile[5], LlistStrDatabaseProfile[3], LlistStrDatabaseProfile[4]);
                        LSqlConnection = new SqlConnection(LStrConnectParam);
                        LSqlConnection.Open();
                        LSqlCommand = new SqlCommand(LStrDynamicSQL, LSqlConnection);
                        LSqlCommand.ExecuteNonQuery();
                    }
                    if (LlistStrDatabaseProfile[0] == "3")
                    {
                        LStrConnectParam = string.Format("Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST={0}) (PORT={1})))(CONNECT_DATA=(SERVICE_NAME= {2})));User Id={3}; Password={4}", LlistStrDatabaseProfile[1], LlistStrDatabaseProfile[2], LlistStrDatabaseProfile[5], LlistStrDatabaseProfile[3], LlistStrDatabaseProfile[4]);
                        LOracleConnection = new OracleConnection(LStrConnectParam);
                        LOracleConnection.Open();
                        LOracleCommand = new OracleCommand(LStrDynamicSQL, LOracleConnection);
                        LOracleCommand.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                LOperationReturn.BoolReturn = false;
                LOperationReturn.StringReturn = "WCF003E999" + AscCodeToChr(27) + "OperationA05\n" + ex.Message;
            }
            finally
            {
                if (LSqlCommand != null) { LSqlCommand.Dispose(); LSqlCommand = null; }
                if (LOracleCommand != null) { LOracleCommand.Dispose(); LOracleCommand = null; }

                if (LSqlConnection != null)
                {
                    if (LSqlConnection.State == ConnectionState.Open) { LSqlConnection.Close(); }
                    LSqlConnection.Dispose(); LSqlConnection = null;
                }
                if (LOracleConnection != null)
                {
                    if (LOracleConnection.State == ConnectionState.Open) { LOracleConnection.Close(); }
                    LOracleConnection.Dispose(); LOracleConnection = null;
                }
            }

            return LOperationReturn;
        }
        #endregion

        #region 06 将修改后的语言写入到数据库
        /// <summary>
        /// 将修改后的语言写入到数据库
        /// </summary>
        /// <param name="AListStringArgs">
        /// 0：数据库类型
        /// 1：数据库服务器
        /// 2：端口
        /// 3：登录名
        /// 4：密码
        /// 5：数据库名或服务名
        /// 6：LanguageID
        /// 7：MessageID
        /// 8～N：字段名 + char(27) + 数据
        /// </param>
        /// <returns></returns>
        private OperationDataArgs OperationA06(List<string> AListStringArgs)
        {
            OperationDataArgs LOperationReturn = new OperationDataArgs();
            List<string> LlistStrDatabaseProfile = new List<string>();

            SqlConnection LSqlConnection = null;
            SqlCommand LSqlCommand = null;
            OracleConnection LOracleConnection = null;
            OracleCommand LOracleCommand = null;

            string LStrConnectParam = string.Empty;
            string LStrLanguageID = string.Empty;
            string LStrMessageID = string.Empty;
            string LStrDynamicSQL = string.Empty;
            DataSet LDataSetSelectReturn = new DataSet();

            try
            {
                for (int LIntLoopList = 0; LIntLoopList <= 5; LIntLoopList++)
                {
                    LlistStrDatabaseProfile.Add(AListStringArgs[LIntLoopList]);
                }
                LStrLanguageID = AListStringArgs[6];
                LStrMessageID = AListStringArgs[7];
                LStrDynamicSQL = "SELECT * FROM T_00_005 WHERE C001 = " + LStrLanguageID + " AND C002 = '" + LStrMessageID + "'";

                if (LlistStrDatabaseProfile[0] == "2")
                {
                    LStrConnectParam = string.Format("Data Source={0},{1};Initial Catalog={2};User Id={3};Password={4}", LlistStrDatabaseProfile[1], LlistStrDatabaseProfile[2], LlistStrDatabaseProfile[5], LlistStrDatabaseProfile[3], LlistStrDatabaseProfile[4]);
                    LSqlConnection = new SqlConnection(LStrConnectParam);
                    LSqlConnection.Open();
                    SqlDataAdapter LSqlDataAdapter = new SqlDataAdapter(LStrDynamicSQL, LSqlConnection);
                    LSqlDataAdapter.Fill(LDataSetSelectReturn);
                    LSqlDataAdapter.Dispose();
                }
                if (LlistStrDatabaseProfile[0] == "3")
                {
                    LStrConnectParam = string.Format("Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST={0}) (PORT={1})))(CONNECT_DATA=(SERVICE_NAME= {2})));User Id={3}; Password={4}", LlistStrDatabaseProfile[1], LlistStrDatabaseProfile[2], LlistStrDatabaseProfile[5], LlistStrDatabaseProfile[3], LlistStrDatabaseProfile[4]);
                    LOracleConnection = new OracleConnection(LStrConnectParam);
                    LOracleConnection.Open();
                    OracleDataAdapter LOracleDataAdapter = new OracleDataAdapter(LStrDynamicSQL, LOracleConnection);
                    LOracleDataAdapter.Fill(LDataSetSelectReturn);
                    LOracleDataAdapter.Dispose();
                }

                if (LDataSetSelectReturn.Tables[0].Rows.Count != 1) { return LOperationReturn; }

                LDataSetSelectReturn.Tables[0].TableName = "T_00_005";
                for (int LIntLoopList = 8; LIntLoopList < AListStringArgs.Count; LIntLoopList++)
                {
                    string[] LStrArrayALanguageInfo = AListStringArgs[LIntLoopList].Split(AscCodeToChr(27).ToCharArray());
                    LDataSetSelectReturn.Tables[0].Rows[0][LStrArrayALanguageInfo[0]] = LStrArrayALanguageInfo[1];
                }

                #region 将数据写入MSSQL数据库
                if (LlistStrDatabaseProfile[0] == "2")
                {
                    SqlDataAdapter LSqlDataAdapter = new SqlDataAdapter(LStrDynamicSQL, LSqlConnection);
                    SqlCommandBuilder LSqlCommandBuilder = new SqlCommandBuilder();
                    LSqlCommandBuilder.ConflictOption = ConflictOption.OverwriteChanges;
                    LSqlCommandBuilder.SetAllValues = false;
                    LSqlCommandBuilder.DataAdapter = LSqlDataAdapter;
                    LSqlDataAdapter.Update(LDataSetSelectReturn, "T_00_005");
                    LDataSetSelectReturn.AcceptChanges();
                    LSqlCommandBuilder.Dispose();
                    LSqlDataAdapter.Dispose();
                }
                #endregion

                #region 将数据写入Oracle数据库
                if (LlistStrDatabaseProfile[0] == "3")
                {
                    OracleDataAdapter LOracleDataAdapter = new OracleDataAdapter(LStrDynamicSQL, LOracleConnection);
                    OracleCommandBuilder LOracleCommandBuilder = new OracleCommandBuilder();

                    LOracleCommandBuilder.ConflictOption = ConflictOption.OverwriteChanges;
                    LOracleCommandBuilder.SetAllValues = false;
                    LOracleCommandBuilder.DataAdapter = LOracleDataAdapter;
                    LOracleDataAdapter.Update(LDataSetSelectReturn, "T_00_005");
                    LDataSetSelectReturn.AcceptChanges();
                    LOracleCommandBuilder.Dispose();
                    LOracleDataAdapter.Dispose();
                }
                #endregion
            }
            catch (Exception ex)
            {
                LOperationReturn.BoolReturn = false;
                LOperationReturn.StringReturn = "WCF003E999" + AscCodeToChr(27) + "OperationA06\n" + ex.Message;
            }
            finally
            {
                if (LSqlCommand != null) { LSqlCommand.Dispose(); LSqlCommand = null; }
                if (LOracleCommand != null) { LOracleCommand.Dispose(); LOracleCommand = null; }

                if (LSqlConnection != null)
                {
                    if (LSqlConnection.State == ConnectionState.Open) { LSqlConnection.Close(); }
                    LSqlConnection.Dispose(); LSqlConnection = null;
                }
                if (LOracleConnection != null)
                {
                    if (LOracleConnection.State == ConnectionState.Open) { LOracleConnection.Close(); }
                    LOracleConnection.Dispose(); LOracleConnection = null;
                }
            }
            return LOperationReturn;
        }
        #endregion

        #region 07 导入语言包
        /// <summary>
        /// 导入语言包
        /// </summary>
        /// <param name="AListStringArgs">
        /// 0：数据库类型
        /// 1：数据库服务器
        /// 2：端口
        /// 3：登录名
        /// 4：密码
        /// 5：数据库名或服务名
        /// 6：LanguageID
        /// 7：导入方法 D-删除；I-插入；A-追加；U-更新并追加
        /// 8～N：字段名 + char(27) + 数据
        /// </param>
        /// <returns></returns>
        private OperationDataArgs OperationA07(List<string> AListStringArgs)
        {
            OperationDataArgs LOperationReturn = new OperationDataArgs();
            List<string> LlistStrDatabaseProfile = new List<string>();

            SqlConnection LSqlConnection = null;
            SqlCommand LSqlCommand = null;
            OracleConnection LOracleConnection = null;
            OracleCommand LOracleCommand = null;

            string LStrConnectParam = string.Empty;
            string LStrLanguageID = string.Empty;
            string LStrDynamicSQL = string.Empty;
            string LStrMethod = string.Empty;
            DataSet LDataSetSelectReturn = new DataSet();

            string LStrSplitChar = string.Empty;
            string LStrC002 = string.Empty;

            try
            {
                #region 局部变量初始化
                LStrSplitChar = AscCodeToChr(27);
                for (int LIntLoopList = 0; LIntLoopList <= 5; LIntLoopList++)
                {
                    LlistStrDatabaseProfile.Add(AListStringArgs[LIntLoopList]);
                }
                LStrLanguageID = AListStringArgs[6];
                LStrMethod = AListStringArgs[7];
                if (LlistStrDatabaseProfile[0] == "2")
                {
                    LStrConnectParam = string.Format("Data Source={0},{1};Initial Catalog={2};User Id={3};Password={4}", LlistStrDatabaseProfile[1], LlistStrDatabaseProfile[2], LlistStrDatabaseProfile[5], LlistStrDatabaseProfile[3], LlistStrDatabaseProfile[4]);
                    LSqlConnection = new SqlConnection(LStrConnectParam);
                    LSqlConnection.Open();
                }
                if (LlistStrDatabaseProfile[0] == "3")
                {
                    LStrConnectParam = string.Format("Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST={0}) (PORT={1})))(CONNECT_DATA=(SERVICE_NAME= {2})));User Id={3}; Password={4}", LlistStrDatabaseProfile[1], LlistStrDatabaseProfile[2], LlistStrDatabaseProfile[5], LlistStrDatabaseProfile[3], LlistStrDatabaseProfile[4]);
                    LOracleConnection = new OracleConnection(LStrConnectParam);
                    LOracleConnection.Open();
                }
                #endregion

                #region 删除语言包
                if (LStrMethod == "D")
                {
                    LStrDynamicSQL = "DELETE FROM T_00_005 WHERE C001 = " + LStrLanguageID;
                    if (LlistStrDatabaseProfile[0] == "2")
                    {
                        LSqlCommand = new SqlCommand(LStrDynamicSQL, LSqlConnection);
                        LSqlCommand.ExecuteNonQuery();
                    }
                    if (LlistStrDatabaseProfile[0] == "3")
                    {
                        LOracleCommand = new OracleCommand(LStrDynamicSQL, LOracleConnection);
                        LOracleCommand.ExecuteNonQuery();
                    }
                }
                #endregion

                #region 插入语言包
                if (LStrMethod == "I")
                {
                    LStrDynamicSQL = "SELECT * FROM T_00_005 WHERE 1 = 2";
                    if (LlistStrDatabaseProfile[0] == "2")
                    {
                        SqlDataAdapter LSqlDataAdapter = new SqlDataAdapter(LStrDynamicSQL, LSqlConnection);
                        LSqlDataAdapter.Fill(LDataSetSelectReturn);
                        LSqlDataAdapter.Dispose();
                    }
                    if (LlistStrDatabaseProfile[0] == "3")
                    {
                        OracleDataAdapter LOracleDataAdapter = new OracleDataAdapter(LStrDynamicSQL, LOracleConnection);
                        LOracleDataAdapter.Fill(LDataSetSelectReturn);
                        LOracleDataAdapter.Dispose();
                    }
                    LDataSetSelectReturn.Tables[0].TableName = "T_00_005";
                    DataRow LDataRowNew = LDataSetSelectReturn.Tables[0].NewRow();
                    LDataRowNew.BeginEdit();
                    LDataRowNew["C001"] = int.Parse(LStrLanguageID);
                    LDataRowNew["C002"] = AListStringArgs[9].Split(LStrSplitChar.ToCharArray())[1];
                    LDataRowNew["C003"] = int.Parse(AListStringArgs[10].Split(LStrSplitChar.ToCharArray())[1]);
                    LDataRowNew["C004"] = int.Parse(AListStringArgs[11].Split(LStrSplitChar.ToCharArray())[1]);
                    LDataRowNew["C005"] = AListStringArgs[12].Split(LStrSplitChar.ToCharArray())[1];
                    LDataRowNew["C006"] = AListStringArgs[13].Split(LStrSplitChar.ToCharArray())[1];
                    LDataRowNew["C007"] = AListStringArgs[14].Split(LStrSplitChar.ToCharArray())[1];
                    LDataRowNew["C008"] = AListStringArgs[15].Split(LStrSplitChar.ToCharArray())[1];
                    LDataRowNew["C009"] = int.Parse(AListStringArgs[16].Split(LStrSplitChar.ToCharArray())[1]);
                    LDataRowNew["C010"] = int.Parse(AListStringArgs[17].Split(LStrSplitChar.ToCharArray())[1]);
                    LDataRowNew["C011"] = AListStringArgs[18].Split(LStrSplitChar.ToCharArray())[1];
                    LDataRowNew["C012"] = AListStringArgs[19].Split(LStrSplitChar.ToCharArray())[1];
                    LDataRowNew.EndEdit();
                    LDataSetSelectReturn.Tables[0].Rows.Add(LDataRowNew);

                    #region 将数据写入MSSQL数据库
                    if (LlistStrDatabaseProfile[0] == "2")
                    {
                        SqlDataAdapter LSqlDataAdapter = new SqlDataAdapter(LStrDynamicSQL, LSqlConnection);
                        SqlCommandBuilder LSqlCommandBuilder = new SqlCommandBuilder();
                        LSqlCommandBuilder.ConflictOption = ConflictOption.OverwriteChanges;
                        LSqlCommandBuilder.SetAllValues = false;
                        LSqlCommandBuilder.DataAdapter = LSqlDataAdapter;
                        LSqlDataAdapter.Update(LDataSetSelectReturn, "T_00_005");
                        LDataSetSelectReturn.AcceptChanges();
                        LSqlCommandBuilder.Dispose();
                        LSqlDataAdapter.Dispose();
                    }
                    #endregion

                    #region 将数据写入Oracle数据库
                    if (LlistStrDatabaseProfile[0] == "3")
                    {
                        OracleDataAdapter LOracleDataAdapter = new OracleDataAdapter(LStrDynamicSQL, LOracleConnection);
                        OracleCommandBuilder LOracleCommandBuilder = new OracleCommandBuilder();

                        LOracleCommandBuilder.ConflictOption = ConflictOption.OverwriteChanges;
                        LOracleCommandBuilder.SetAllValues = false;
                        LOracleCommandBuilder.DataAdapter = LOracleDataAdapter;
                        LOracleDataAdapter.Update(LDataSetSelectReturn, "T_00_005");
                        LDataSetSelectReturn.AcceptChanges();
                        LOracleCommandBuilder.Dispose();
                        LOracleDataAdapter.Dispose();
                    }
                    #endregion
                }
                #endregion

                #region 追加语言包
                if (LStrMethod == "A")
                {
                    LStrC002 = AListStringArgs[9].Split(LStrSplitChar.ToCharArray())[1];
                    LStrDynamicSQL = "SELECT * FROM T_00_005 WHERE C001 = " + LStrLanguageID + " AND C002 = '" + LStrC002 + "'";
                    if (LlistStrDatabaseProfile[0] == "2")
                    {
                        SqlDataAdapter LSqlDataAdapter = new SqlDataAdapter(LStrDynamicSQL, LSqlConnection);
                        LSqlDataAdapter.Fill(LDataSetSelectReturn);
                        LSqlDataAdapter.Dispose();
                    }
                    if (LlistStrDatabaseProfile[0] == "3")
                    {
                        OracleDataAdapter LOracleDataAdapter = new OracleDataAdapter(LStrDynamicSQL, LOracleConnection);
                        LOracleDataAdapter.Fill(LDataSetSelectReturn);
                        LOracleDataAdapter.Dispose();
                    }
                    LDataSetSelectReturn.Tables[0].TableName = "T_00_005";
                    if (LDataSetSelectReturn.Tables[0].Rows.Count > 0) { return LOperationReturn; }

                    DataRow LDataRowNew = LDataSetSelectReturn.Tables[0].NewRow();
                    LDataRowNew.BeginEdit();
                    LDataRowNew["C001"] = int.Parse(LStrLanguageID);
                    LDataRowNew["C002"] = AListStringArgs[9].Split(LStrSplitChar.ToCharArray())[1];
                    LDataRowNew["C003"] = int.Parse(AListStringArgs[10].Split(LStrSplitChar.ToCharArray())[1]);
                    LDataRowNew["C004"] = int.Parse(AListStringArgs[11].Split(LStrSplitChar.ToCharArray())[1]);
                    LDataRowNew["C005"] = AListStringArgs[12].Split(LStrSplitChar.ToCharArray())[1];
                    LDataRowNew["C006"] = AListStringArgs[13].Split(LStrSplitChar.ToCharArray())[1];
                    LDataRowNew["C007"] = AListStringArgs[14].Split(LStrSplitChar.ToCharArray())[1];
                    LDataRowNew["C008"] = AListStringArgs[15].Split(LStrSplitChar.ToCharArray())[1];
                    LDataRowNew["C009"] = int.Parse(AListStringArgs[16].Split(LStrSplitChar.ToCharArray())[1]);
                    LDataRowNew["C010"] = int.Parse(AListStringArgs[17].Split(LStrSplitChar.ToCharArray())[1]);
                    LDataRowNew["C011"] = AListStringArgs[18].Split(LStrSplitChar.ToCharArray())[1];
                    LDataRowNew["C012"] = AListStringArgs[19].Split(LStrSplitChar.ToCharArray())[1];
                    LDataRowNew.EndEdit();
                    LDataSetSelectReturn.Tables[0].Rows.Add(LDataRowNew);

                    #region 将数据写入MSSQL数据库
                    if (LlistStrDatabaseProfile[0] == "2")
                    {
                        SqlDataAdapter LSqlDataAdapter = new SqlDataAdapter(LStrDynamicSQL, LSqlConnection);
                        SqlCommandBuilder LSqlCommandBuilder = new SqlCommandBuilder();
                        LSqlCommandBuilder.ConflictOption = ConflictOption.OverwriteChanges;
                        LSqlCommandBuilder.SetAllValues = false;
                        LSqlCommandBuilder.DataAdapter = LSqlDataAdapter;
                        LSqlDataAdapter.Update(LDataSetSelectReturn, "T_00_005");
                        LDataSetSelectReturn.AcceptChanges();
                        LSqlCommandBuilder.Dispose();
                        LSqlDataAdapter.Dispose();
                    }
                    #endregion

                    #region 将数据写入Oracle数据库
                    if (LlistStrDatabaseProfile[0] == "3")
                    {
                        OracleDataAdapter LOracleDataAdapter = new OracleDataAdapter(LStrDynamicSQL, LOracleConnection);
                        OracleCommandBuilder LOracleCommandBuilder = new OracleCommandBuilder();

                        LOracleCommandBuilder.ConflictOption = ConflictOption.OverwriteChanges;
                        LOracleCommandBuilder.SetAllValues = false;
                        LOracleCommandBuilder.DataAdapter = LOracleDataAdapter;
                        LOracleDataAdapter.Update(LDataSetSelectReturn, "T_00_005");
                        LDataSetSelectReturn.AcceptChanges();
                        LOracleCommandBuilder.Dispose();
                        LOracleDataAdapter.Dispose();
                    }
                    #endregion
                }
                #endregion

                #region 更新语言包，存在则覆盖，不存在添加
                if (LStrMethod == "U")
                {
                    LStrC002 = AListStringArgs[9].Split(LStrSplitChar.ToCharArray())[1];
                    LStrDynamicSQL = "SELECT * FROM T_00_005 WHERE C001 = " + LStrLanguageID + " AND C002 = '" + LStrC002 + "'";
                    if (LlistStrDatabaseProfile[0] == "2")
                    {
                        SqlDataAdapter LSqlDataAdapter = new SqlDataAdapter(LStrDynamicSQL, LSqlConnection);
                        LSqlDataAdapter.Fill(LDataSetSelectReturn);
                        LSqlDataAdapter.Dispose();
                    }
                    if (LlistStrDatabaseProfile[0] == "3")
                    {
                        OracleDataAdapter LOracleDataAdapter = new OracleDataAdapter(LStrDynamicSQL, LOracleConnection);
                        LOracleDataAdapter.Fill(LDataSetSelectReturn);
                        LOracleDataAdapter.Dispose();
                    }
                    LDataSetSelectReturn.Tables[0].TableName = "T_00_005";

                    if (LDataSetSelectReturn.Tables[0].Rows.Count == 0)
                    {
                        DataRow LDataRowNew = LDataSetSelectReturn.Tables[0].NewRow();
                        LDataRowNew.BeginEdit();
                        LDataRowNew["C001"] = int.Parse(LStrLanguageID);
                        LDataRowNew["C002"] = AListStringArgs[9].Split(LStrSplitChar.ToCharArray())[1];
                        LDataRowNew["C003"] = int.Parse(AListStringArgs[10].Split(LStrSplitChar.ToCharArray())[1]);
                        LDataRowNew["C004"] = int.Parse(AListStringArgs[11].Split(LStrSplitChar.ToCharArray())[1]);
                        LDataRowNew["C005"] = AListStringArgs[12].Split(LStrSplitChar.ToCharArray())[1];
                        LDataRowNew["C006"] = AListStringArgs[13].Split(LStrSplitChar.ToCharArray())[1];
                        LDataRowNew["C007"] = AListStringArgs[14].Split(LStrSplitChar.ToCharArray())[1];
                        LDataRowNew["C008"] = AListStringArgs[15].Split(LStrSplitChar.ToCharArray())[1];
                        LDataRowNew["C009"] = int.Parse(AListStringArgs[16].Split(LStrSplitChar.ToCharArray())[1]);
                        LDataRowNew["C010"] = int.Parse(AListStringArgs[17].Split(LStrSplitChar.ToCharArray())[1]);
                        LDataRowNew["C011"] = AListStringArgs[18].Split(LStrSplitChar.ToCharArray())[1];
                        LDataRowNew["C012"] = AListStringArgs[19].Split(LStrSplitChar.ToCharArray())[1];
                        LDataRowNew.EndEdit();
                        LDataSetSelectReturn.Tables[0].Rows.Add(LDataRowNew);
                    }
                    else
                    {
                        LDataSetSelectReturn.Tables[0].Rows[0]["C003"] = int.Parse(AListStringArgs[10].Split(LStrSplitChar.ToCharArray())[1]);
                        LDataSetSelectReturn.Tables[0].Rows[0]["C004"] = int.Parse(AListStringArgs[11].Split(LStrSplitChar.ToCharArray())[1]);
                        LDataSetSelectReturn.Tables[0].Rows[0]["C005"] = AListStringArgs[12].Split(LStrSplitChar.ToCharArray())[1];
                        LDataSetSelectReturn.Tables[0].Rows[0]["C006"] = AListStringArgs[13].Split(LStrSplitChar.ToCharArray())[1];
                        LDataSetSelectReturn.Tables[0].Rows[0]["C007"] = AListStringArgs[14].Split(LStrSplitChar.ToCharArray())[1];
                        LDataSetSelectReturn.Tables[0].Rows[0]["C008"] = AListStringArgs[15].Split(LStrSplitChar.ToCharArray())[1];
                        LDataSetSelectReturn.Tables[0].Rows[0]["C009"] = int.Parse(AListStringArgs[16].Split(LStrSplitChar.ToCharArray())[1]);
                        LDataSetSelectReturn.Tables[0].Rows[0]["C010"] = int.Parse(AListStringArgs[17].Split(LStrSplitChar.ToCharArray())[1]);
                        LDataSetSelectReturn.Tables[0].Rows[0]["C011"] = AListStringArgs[18].Split(LStrSplitChar.ToCharArray())[1];
                        LDataSetSelectReturn.Tables[0].Rows[0]["C012"] = AListStringArgs[19].Split(LStrSplitChar.ToCharArray())[1];
                    }
                    #region 将数据写入MSSQL数据库
                    if (LlistStrDatabaseProfile[0] == "2")
                    {
                        SqlDataAdapter LSqlDataAdapter = new SqlDataAdapter(LStrDynamicSQL, LSqlConnection);
                        SqlCommandBuilder LSqlCommandBuilder = new SqlCommandBuilder();
                        LSqlCommandBuilder.ConflictOption = ConflictOption.OverwriteChanges;
                        LSqlCommandBuilder.SetAllValues = false;
                        LSqlCommandBuilder.DataAdapter = LSqlDataAdapter;
                        LSqlDataAdapter.Update(LDataSetSelectReturn, "T_00_005");
                        LDataSetSelectReturn.AcceptChanges();
                        LSqlCommandBuilder.Dispose();
                        LSqlDataAdapter.Dispose();
                    }
                    #endregion

                    #region 将数据写入Oracle数据库
                    if (LlistStrDatabaseProfile[0] == "3")
                    {
                        OracleDataAdapter LOracleDataAdapter = new OracleDataAdapter(LStrDynamicSQL, LOracleConnection);
                        OracleCommandBuilder LOracleCommandBuilder = new OracleCommandBuilder();

                        LOracleCommandBuilder.ConflictOption = ConflictOption.OverwriteChanges;
                        LOracleCommandBuilder.SetAllValues = false;
                        LOracleCommandBuilder.DataAdapter = LOracleDataAdapter;
                        LOracleDataAdapter.Update(LDataSetSelectReturn, "T_00_005");
                        LDataSetSelectReturn.AcceptChanges();
                        LOracleCommandBuilder.Dispose();
                        LOracleDataAdapter.Dispose();
                    }
                    #endregion
                }
                #endregion
                
            }
            catch (Exception ex)
            {
                LOperationReturn.BoolReturn = false;
                LOperationReturn.StringReturn = "WCF003E999" + AscCodeToChr(27) + "OperationA07\n" + ex.Message;
            }
            finally
            {
                if (LSqlCommand != null) { LSqlCommand.Dispose(); LSqlCommand = null; }
                if (LOracleCommand != null) { LOracleCommand.Dispose(); LOracleCommand = null; }

                if (LSqlConnection != null)
                {
                    if (LSqlConnection.State == ConnectionState.Open) { LSqlConnection.Close(); }
                    LSqlConnection.Dispose(); LSqlConnection = null;
                }
                if (LOracleConnection != null)
                {
                    if (LOracleConnection.State == ConnectionState.Open) { LOracleConnection.Close(); }
                    LOracleConnection.Dispose(); LOracleConnection = null;
                }
            }

            return LOperationReturn;
        }
        #endregion

        #region 08 获取租户列表
        /// <summary>
        /// 获取租户列表
        /// </summary>
        /// <param name="AListStringArgs">
        /// 0：数据库类型
        /// 1：数据库服务器
        /// 2：端口
        /// 3：登录名
        /// 4：密码
        /// 5：数据库名或服务名
        /// </param>
        /// <returns></returns>
        private OperationDataArgs OperationA08(List<string> AListStringArgs)
        {
            OperationDataArgs LOperationReturn = new OperationDataArgs();

            List<string> LlistStrDatabaseProfile = new List<string>();
            string LStrConnectParam = string.Empty;
            SqlConnection LSqlConnection = null;
            SqlCommand LSqlCommand = null;
            OracleConnection LOracleConnection = null;
            OracleCommand LOracleCommand = null;
            string LStrDynamicSQL = string.Empty;
            DataSet LDataSetSelectReturn = new DataSet();

            string LStrSplitChar = string.Empty;
            string LStrVerificationCode102 = string.Empty;

            try
            {
                #region 局部变量初始化
                LStrSplitChar = AscCodeToChr(27);
                LStrVerificationCode102 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M102);

                for (int LIntLoopList = 0; LIntLoopList <= 5; LIntLoopList++)
                {
                    LlistStrDatabaseProfile.Add(AListStringArgs[LIntLoopList]);
                }
                if (LlistStrDatabaseProfile[0] == "2")
                {
                    LStrConnectParam = string.Format("Data Source={0},{1};Initial Catalog={2};User Id={3};Password={4}", LlistStrDatabaseProfile[1], LlistStrDatabaseProfile[2], LlistStrDatabaseProfile[5], LlistStrDatabaseProfile[3], LlistStrDatabaseProfile[4]);
                    LSqlConnection = new SqlConnection(LStrConnectParam);
                    LSqlConnection.Open();
                }
                if (LlistStrDatabaseProfile[0] == "3")
                {
                    LStrConnectParam = string.Format("Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST={0}) (PORT={1})))(CONNECT_DATA=(SERVICE_NAME= {2})));User Id={3}; Password={4}", LlistStrDatabaseProfile[1], LlistStrDatabaseProfile[2], LlistStrDatabaseProfile[5], LlistStrDatabaseProfile[3], LlistStrDatabaseProfile[4]);
                    LOracleConnection = new OracleConnection(LStrConnectParam);
                    LOracleConnection.Open();
                }
                #endregion

                LStrDynamicSQL = "SELECT * FROM T_00_121";

                if (LlistStrDatabaseProfile[0] == "2")
                {
                    SqlDataAdapter LSqlDataAdapter = new SqlDataAdapter(LStrDynamicSQL, LSqlConnection);
                    LSqlDataAdapter.Fill(LDataSetSelectReturn);
                    LSqlDataAdapter.Dispose();
                }
                if (LlistStrDatabaseProfile[0] == "3")
                {
                    OracleDataAdapter LOracleDataAdapter = new OracleDataAdapter(LStrDynamicSQL, LOracleConnection);
                    LOracleDataAdapter.Fill(LDataSetSelectReturn);
                    LOracleDataAdapter.Dispose();
                }

                foreach (DataRow LDataRowSingleRent in LDataSetSelectReturn.Tables[0].Rows)
                {
                    LDataRowSingleRent["C002"] = EncryptionAndDecryption.EncryptDecryptString(LDataRowSingleRent["C002"].ToString(), LStrVerificationCode102, EncryptionAndDecryption.UMPKeyAndIVType.M102);
                    LDataRowSingleRent["C011"] = EncryptionAndDecryption.EncryptDecryptString(LDataRowSingleRent["C011"].ToString(), LStrVerificationCode102, EncryptionAndDecryption.UMPKeyAndIVType.M102);
                    LDataRowSingleRent["C012"] = EncryptionAndDecryption.EncryptDecryptString(LDataRowSingleRent["C012"].ToString(), LStrVerificationCode102, EncryptionAndDecryption.UMPKeyAndIVType.M102);
                    LDataRowSingleRent["C021"] = EncryptionAndDecryption.EncryptDecryptString(LDataRowSingleRent["C021"].ToString(), LStrVerificationCode102, EncryptionAndDecryption.UMPKeyAndIVType.M102);
                    LDataRowSingleRent["C022"] = EncryptionAndDecryption.EncryptDecryptString(LDataRowSingleRent["C022"].ToString(), LStrVerificationCode102, EncryptionAndDecryption.UMPKeyAndIVType.M102);
                }

                LOperationReturn.DataSetReturn = LDataSetSelectReturn;
            }
            catch (Exception ex)
            {
                LOperationReturn.BoolReturn = false;
                LOperationReturn.StringReturn = "WCF003E999" + AscCodeToChr(27) + "OperationA08\n" + ex.Message;
            }
            finally
            {
                if (LSqlCommand != null) { LSqlCommand.Dispose(); LSqlCommand = null; }
                if (LOracleCommand != null) { LOracleCommand.Dispose(); LOracleCommand = null; }

                if (LSqlConnection != null)
                {
                    if (LSqlConnection.State == ConnectionState.Open) { LSqlConnection.Close(); }
                    LSqlConnection.Dispose(); LSqlConnection = null;
                }
                if (LOracleConnection != null)
                {
                    if (LOracleConnection.State == ConnectionState.Open) { LOracleConnection.Close(); }
                    LOracleConnection.Dispose(); LOracleConnection = null;
                }
            }

            return LOperationReturn;
        }
        #endregion

        #region 09 获取租户在线用户列表 / 强制注销在线用户
        /// <summary>
        /// 获取租户在线用户列表等
        /// </summary>
        /// <param name="AListStringArgs">
        /// 0：数据库类型
        /// 1：数据库服务器
        /// 2：端口
        /// 3：登录名
        /// 4：密码
        /// 5：数据库名或服务名
        /// 6：租户Token
        /// 7：操作 01-在线用户；02-注销用户
        /// 8：（02）登录用户的SessionID
        /// </param>
        /// <returns></returns>
        private OperationDataArgs OperationA09(List<string> AListStringArgs)
        {
            OperationDataArgs LOperationReturn = new OperationDataArgs();
            List<string> LlistStrDatabaseProfile = new List<string>();
            string LStrDynamicSQL = string.Empty;
            DatabaseOperationReturn LDatabaseOperationReturn;

            string LStrVerificationCode102 = string.Empty;
            string LStrVerificationCode002 = string.Empty;

            try
            {
                LStrVerificationCode102 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M102);
                LStrVerificationCode002 = CreateVerificationCode(EncryptionAndDecryption.UMPKeyAndIVType.M002);

                for (int LIntLoopList = 0; LIntLoopList <= 5; LIntLoopList++)
                {
                    LlistStrDatabaseProfile.Add(AListStringArgs[LIntLoopList]);
                }

                #region 01 - 获取在线用户
                if (AListStringArgs[7] == "01")
                {
                    LStrDynamicSQL = "SELECT A.C001 UserID, A.C002 UserAccount, B.C002 LoginTime, B.C004 LoginHost, B.C005 LoginIP, B.C006 SessionID FROM T_11_005_" + AListStringArgs[6] + " A, T_11_002_" + AListStringArgs[6] + " B WHERE A.C001 = B.C001 AND B.C008 = '0'";
                    LDatabaseOperationReturn = SelectDataByDynamicSQL(LlistStrDatabaseProfile, LStrDynamicSQL);
                    LOperationReturn.BoolReturn = LDatabaseOperationReturn.BoolReturn;
                    LOperationReturn.DataSetReturn = LDatabaseOperationReturn.DataSetReturn;
                    if (LOperationReturn.BoolReturn)
                    {
                        foreach (DataRow LDataRowSingleRent in LOperationReturn.DataSetReturn.Tables[0].Rows)
                        {
                            LDataRowSingleRent["UserAccount"] = EncryptionAndDecryption.EncryptDecryptString(LDataRowSingleRent["UserAccount"].ToString(), LStrVerificationCode102, EncryptionAndDecryption.UMPKeyAndIVType.M102);
                            LDataRowSingleRent["LoginTime"] = EncryptionAndDecryption.EncryptDecryptString(LDataRowSingleRent["LoginTime"].ToString(), LStrVerificationCode102, EncryptionAndDecryption.UMPKeyAndIVType.M102);
                            LDataRowSingleRent["LoginHost"] = EncryptionAndDecryption.EncryptDecryptString(LDataRowSingleRent["LoginHost"].ToString(), LStrVerificationCode102, EncryptionAndDecryption.UMPKeyAndIVType.M102);
                            LDataRowSingleRent["LoginIP"] = EncryptionAndDecryption.EncryptDecryptString(LDataRowSingleRent["LoginIP"].ToString(), LStrVerificationCode102, EncryptionAndDecryption.UMPKeyAndIVType.M102);
                        }
                    }
                }
                #endregion

                #region 02 - 注销用户
                if (AListStringArgs[7] == "02")
                {
                    string LStr11002009 = string.Empty;
                    LStr11002009 = EncryptionAndDecryption.EncryptDecryptString(DateTime.UtcNow.ToString("yyyyMMdd HHmmss"), LStrVerificationCode002, EncryptionAndDecryption.UMPKeyAndIVType.M002);
                    LStrDynamicSQL = "UPDATE T_11_002_" + AListStringArgs[6] + " SET C008 = '1', C009 = '" + LStr11002009 + "', C010 = 'U' WHERE C006 = " + AListStringArgs[8];
                    LDatabaseOperationReturn = ExecuteDynamicSQL(LlistStrDatabaseProfile, LStrDynamicSQL);
                    LOperationReturn.BoolReturn = LDatabaseOperationReturn.BoolReturn;
                    LOperationReturn.StringReturn = LDatabaseOperationReturn.StrReturn;
                }
                #endregion
            }
            catch (Exception ex)
            {
                LOperationReturn.BoolReturn = false;
                LOperationReturn.StringReturn = "WCF003E999" + AscCodeToChr(27) + "OperationA09\n" + ex.Message;
            }

            return LOperationReturn;
        }
        #endregion

        #region 10 读取第三方配置信息（ASM）
        /// <summary>
        /// 读取第三方配置信息
        /// </summary>
        /// <param name="AListStringArgs">
        /// 0：数据库类型
        /// 1：数据库服务器
        /// 2：端口
        /// 3：登录名
        /// 4：密码
        /// 5：数据库名或服务名
        /// 6：租户Token
        private OperationDataArgs OperationA10(List<string> AListStringArgs)
        {
            OperationDataArgs LOperationReturn = new OperationDataArgs();

            DataTable LDataTableReturn = new DataTable();
            string LStrSiteBaseDirectory = string.Empty;
            string LStrXmlFileThirdPartyApp = string.Empty;

            List<string> LlistStrDatabaseProfile = new List<string>();
            string LStrDynamicSQL = string.Empty;
            DataSet LDataSetSelectReturn = new DataSet();
            DatabaseOperationReturn LDatabaseOperationReturn;

            string LStrXml07A00 = string.Empty;

            try
            {
                #region 初始化局部变量
                LDataTableReturn.TableName = "T_00_XXX_" + AListStringArgs[6];
                LDataTableReturn.Columns.Add("Attribute00", typeof(string));
                LDataTableReturn.Columns.Add("Attribute01", typeof(string));
                LDataTableReturn.Columns.Add("Attribute02", typeof(string));
                LDataTableReturn.Columns.Add("Attribute03", typeof(string));
                LDataTableReturn.Columns.Add("Attribute04", typeof(string));
                LDataTableReturn.Columns.Add("Attribute05", typeof(string));
                LDataTableReturn.Columns.Add("Attribute06", typeof(string));
                LDataTableReturn.Columns.Add("Attribute07", typeof(string));
                LDataTableReturn.Columns.Add("Attribute08", typeof(string));
                LDataTableReturn.Columns.Add("Attribute09", typeof(string));
                LDataTableReturn.Columns.Add("Attribute11", typeof(string));
                LDataTableReturn.Columns.Add("Attribute12", typeof(string));
                LDataTableReturn.Columns.Add("Attribute13", typeof(string));
                LDataTableReturn.Columns.Add("Attribute14", typeof(string));
                LDataTableReturn.Columns.Add("Attribute15", typeof(string));

                LStrSiteBaseDirectory = GetIISBaseDirectory();
                LStrXmlFileThirdPartyApp = System.IO.Path.Combine(LStrSiteBaseDirectory, @"GlobalSettings\UMP.Server.07.xml");

                for (int LIntLoopList = 0; LIntLoopList <= 5; LIntLoopList++)
                {
                    LlistStrDatabaseProfile.Add(AListStringArgs[LIntLoopList]);
                }
                #endregion

                #region 从XML文件中读取第三方应用信息
                XmlDocument LXmlDocServer07 = new XmlDocument();
                LXmlDocServer07.Load(LStrXmlFileThirdPartyApp);
                XmlNode LXMLNodeThirdPartyApp = LXmlDocServer07.SelectSingleNode("UMPSetted").SelectSingleNode("ThirdPartyApplications");
                XmlNodeList LXmlNodeListThirdPartyApps = LXMLNodeThirdPartyApp.ChildNodes;
                foreach (XmlNode LXmlNodeSingleThirdPartyApp in LXmlNodeListThirdPartyApps)
                {
                    LStrXml07A00 = LXmlNodeSingleThirdPartyApp.Attributes["Attribute00"].Value;
                    DataRow LDataRow = LDataTableReturn.NewRow();
                    LDataRow.BeginEdit();
                    LDataRow["Attribute00"] = LStrXml07A00;
                    LDataRow["Attribute01"] = LXmlNodeSingleThirdPartyApp.Attributes["Attribute01"].Value;
                    LDataRow["Attribute02"] = LXmlNodeSingleThirdPartyApp.Attributes["Attribute02"].Value;
                    LDataRow["Attribute03"] = LXmlNodeSingleThirdPartyApp.Attributes["Attribute03"].Value;
                    LDataRow["Attribute04"] = LXmlNodeSingleThirdPartyApp.Attributes["Attribute04"].Value;
                    LDataRow["Attribute05"] = LXmlNodeSingleThirdPartyApp.Attributes["Attribute05"].Value;
                    LDataRow["Attribute06"] = LXmlNodeSingleThirdPartyApp.Attributes["Attribute06"].Value;
                    LDataRow["Attribute07"] = LXmlNodeSingleThirdPartyApp.Attributes["Attribute07"].Value;
                    LDataRow["Attribute08"] = LXmlNodeSingleThirdPartyApp.Attributes["Attribute08"].Value;
                    LDataRow["Attribute09"] = LXmlNodeSingleThirdPartyApp.Attributes["Attribute09"].Value;
                    if (LStrXml07A00 == "ASM")
                    {
                        LStrDynamicSQL = "SELECT C009 FROM T_11_003_" + AListStringArgs[6] + " WHERE C002 = 4401";
                        LDatabaseOperationReturn = SelectDataByDynamicSQL(LlistStrDatabaseProfile, LStrDynamicSQL);
                        if (LDatabaseOperationReturn.BoolReturn)
                        {
                            LDataRow["Attribute11"] = LDatabaseOperationReturn.DataSetReturn.Tables[0].Rows[0][0].ToString();
                        }
                    }
                    LDataRow.EndEdit();
                    LDataTableReturn.Rows.Add(LDataRow);
                }
                LDataSetSelectReturn.Tables.Add(LDataTableReturn);
                LOperationReturn.DataSetReturn = LDataSetSelectReturn;
                #endregion
            }
            catch (Exception ex)
            {
                LOperationReturn.BoolReturn = false;
                LOperationReturn.StringReturn = "WCF003E999" + AscCodeToChr(27) + "OperationA10\n" + ex.Message;
            }

            return LOperationReturn;
        }
        #endregion

        #region 11 设置第三方应用配置信息
        /// <summary>
        /// 设置第三方应用配置信息
        /// </summary>
        /// <param name="AListStringArgs">
        /// 0：数据库类型
        /// 1：数据库服务器
        /// 2：端口
        /// 3：登录名
        /// 4：密码
        /// 5：数据库名或服务名
        /// 6：租户Token
        /// 7~N：配置的信息
        /// </param>
        /// <returns></returns>
        private OperationDataArgs OperationA11(List<string> AListStringArgs)
        {
            OperationDataArgs LOperationReturn = new OperationDataArgs();

            string LStrSiteBaseDirectory = string.Empty;
            string LStrXmlFileThirdPartyApp = string.Empty;

            List<string> LlistStrDatabaseProfile = new List<string>();
            string LStrDynamicSQL = string.Empty;

            string LStrConnectParam = string.Empty;
            SqlConnection LSqlConnection = null;
            SqlCommand LSqlCommand = null;
            OracleConnection LOracleConnection = null;
            OracleCommand LOracleCommand = null;
            DataSet LDataSetSelectReturn = new DataSet();

            string LStrXml07A00 = string.Empty;

            try
            {
                #region 初始化局部变量
                LStrSiteBaseDirectory = GetIISBaseDirectory();
                if (AListStringArgs[7] == "ASM")
                {
                    LStrXmlFileThirdPartyApp = System.IO.Path.Combine(LStrSiteBaseDirectory, @"GlobalSettings\UMP.Server.07.xml");
                    LStrDynamicSQL = "SELECT * FROM T_11_003_" + AListStringArgs[6] + " WHERE C002 = 4401";
                }
                for (int LIntLoopList = 0; LIntLoopList <= 5; LIntLoopList++)
                {
                    LlistStrDatabaseProfile.Add(AListStringArgs[LIntLoopList]);
                }

                if (!string.IsNullOrEmpty(LStrDynamicSQL))
                {
                    if (LlistStrDatabaseProfile[0] == "2")
                    {
                        LStrConnectParam = string.Format("Data Source={0},{1};Initial Catalog={2};User Id={3};Password={4}", LlistStrDatabaseProfile[1], LlistStrDatabaseProfile[2], LlistStrDatabaseProfile[5], LlistStrDatabaseProfile[3], LlistStrDatabaseProfile[4]);
                        LSqlConnection = new SqlConnection(LStrConnectParam);
                        LSqlConnection.Open();
                        SqlDataAdapter LSqlDataAdapter = new SqlDataAdapter(LStrDynamicSQL, LSqlConnection);
                        LSqlDataAdapter.Fill(LDataSetSelectReturn);
                        LSqlDataAdapter.Dispose();
                    }
                    if (LlistStrDatabaseProfile[0] == "3")
                    {
                        LStrConnectParam = string.Format("Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST={0}) (PORT={1})))(CONNECT_DATA=(SERVICE_NAME= {2})));User Id={3}; Password={4}", LlistStrDatabaseProfile[1], LlistStrDatabaseProfile[2], LlistStrDatabaseProfile[5], LlistStrDatabaseProfile[3], LlistStrDatabaseProfile[4]);
                        LOracleConnection = new OracleConnection(LStrConnectParam);
                        LOracleConnection.Open();
                        OracleDataAdapter LOracleDataAdapter = new OracleDataAdapter(LStrDynamicSQL, LOracleConnection);
                        LOracleDataAdapter.Fill(LDataSetSelectReturn);
                        LOracleDataAdapter.Dispose();
                    }

                    if (LDataSetSelectReturn.Tables[0].Rows.Count != 1) { return LOperationReturn; }
                    if (AListStringArgs[7] == "ASM")
                    {
                        LDataSetSelectReturn.Tables[0].TableName = "T_11_003_" + AListStringArgs[6];
                    }
                }
                    
                #endregion

                #region 保存ASM配置
                if (AListStringArgs[7] == "ASM")
                {
                    LDataSetSelectReturn.Tables[0].Rows[0]["C009"] = AListStringArgs[11];

                    XmlDocument LXmlDocServer07 = new XmlDocument();
                    LXmlDocServer07.Load(LStrXmlFileThirdPartyApp);
                    XmlNode LXMLNodeThirdPartyApp = LXmlDocServer07.SelectSingleNode("UMPSetted").SelectSingleNode("ThirdPartyApplications");
                    XmlNodeList LXmlNodeListThirdPartyApps = LXMLNodeThirdPartyApp.ChildNodes;
                    foreach (XmlNode LXmlNodeSingleThirdPartyApp in LXmlNodeListThirdPartyApps)
                    {
                        LStrXml07A00 = LXmlNodeSingleThirdPartyApp.Attributes["Attribute00"].Value;
                        if (LStrXml07A00 != AListStringArgs[7]) { continue; }

                        LXmlNodeSingleThirdPartyApp.Attributes["Attribute01"].Value = AListStringArgs[8];
                        LXmlNodeSingleThirdPartyApp.Attributes["Attribute02"].Value = AListStringArgs[9];
                        LXmlNodeSingleThirdPartyApp.Attributes["Attribute03"].Value = AListStringArgs[10];
                        break;
                    }
                    LXmlDocServer07.Save(LStrXmlFileThirdPartyApp);
                }
                #endregion

                #region 将数据写入MSSQL数据库
                if (LlistStrDatabaseProfile[0] == "2")
                {
                    SqlDataAdapter LSqlDataAdapter = new SqlDataAdapter(LStrDynamicSQL, LSqlConnection);
                    SqlCommandBuilder LSqlCommandBuilder = new SqlCommandBuilder();
                    LSqlCommandBuilder.ConflictOption = ConflictOption.OverwriteChanges;
                    LSqlCommandBuilder.SetAllValues = false;
                    LSqlCommandBuilder.DataAdapter = LSqlDataAdapter;
                    LSqlDataAdapter.Update(LDataSetSelectReturn, "T_11_003_" + AListStringArgs[6]);
                    LDataSetSelectReturn.AcceptChanges();
                    LSqlCommandBuilder.Dispose();
                    LSqlDataAdapter.Dispose();
                }
                #endregion

                #region 将数据写入Oracle数据库
                if (LlistStrDatabaseProfile[0] == "3")
                {
                    OracleDataAdapter LOracleDataAdapter = new OracleDataAdapter(LStrDynamicSQL, LOracleConnection);
                    OracleCommandBuilder LOracleCommandBuilder = new OracleCommandBuilder();

                    LOracleCommandBuilder.ConflictOption = ConflictOption.OverwriteChanges;
                    LOracleCommandBuilder.SetAllValues = false;
                    LOracleCommandBuilder.DataAdapter = LOracleDataAdapter;
                    LOracleDataAdapter.Update(LDataSetSelectReturn, "T_11_003_" + AListStringArgs[6]);
                    LDataSetSelectReturn.AcceptChanges();
                    LOracleCommandBuilder.Dispose();
                    LOracleDataAdapter.Dispose();
                }
                #endregion
            }
            catch (Exception ex)
            {
                LOperationReturn.BoolReturn = false;
                LOperationReturn.StringReturn = "WCF003E999" + AscCodeToChr(27) + "OperationA11\n" + ex.Message;
            }
            finally
            {
                if (LSqlCommand != null) { LSqlCommand.Dispose(); LSqlCommand = null; }
                if (LOracleCommand != null) { LOracleCommand.Dispose(); LOracleCommand = null; }

                if (LSqlConnection != null)
                {
                    if (LSqlConnection.State == ConnectionState.Open) { LSqlConnection.Close(); }
                    LSqlConnection.Dispose(); LSqlConnection = null;
                }
                if (LOracleConnection != null)
                {
                    if (LOracleConnection.State == ConnectionState.Open) { LOracleConnection.Close(); }
                    LOracleConnection.Dispose(); LOracleConnection = null;
                }
            }

            return LOperationReturn;
        }
        #endregion

        #region 根据SQL语句返回查询结果集
        private DatabaseOperationReturn SelectDataByDynamicSQL(List<string> AlistStrDatabaseProfile, string AStrSQLString)
        {
            DatabaseOperationReturn LClassReturn = new DatabaseOperationReturn();

            string LStrConnectParam = string.Empty;
            SqlConnection LSqlConnection = null;
            OracleConnection LOracleConnection = null;

            try
            {
                if (AlistStrDatabaseProfile[0] == "2")
                {
                    LStrConnectParam = string.Format("Data Source={0},{1};Initial Catalog={2};User Id={3};Password={4}", AlistStrDatabaseProfile[1], AlistStrDatabaseProfile[2], AlistStrDatabaseProfile[5], AlistStrDatabaseProfile[3], AlistStrDatabaseProfile[4]);
                    LSqlConnection = new SqlConnection(LStrConnectParam);
                    LSqlConnection.Open();
                    SqlDataAdapter LSqlDataAdapter = new SqlDataAdapter(AStrSQLString, LSqlConnection);
                    LSqlDataAdapter.Fill(LClassReturn.DataSetReturn);
                    LSqlDataAdapter.Dispose();
                }
                if (AlistStrDatabaseProfile[0] == "3")
                {
                    LStrConnectParam = string.Format("Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST={0}) (PORT={1})))(CONNECT_DATA=(SERVICE_NAME= {2})));User Id={3}; Password={4}", AlistStrDatabaseProfile[1], AlistStrDatabaseProfile[2], AlistStrDatabaseProfile[5], AlistStrDatabaseProfile[3], AlistStrDatabaseProfile[4]);
                    LOracleConnection = new OracleConnection(LStrConnectParam);
                    LOracleConnection.Open();
                    OracleDataAdapter LOracleDataAdapter = new OracleDataAdapter(AStrSQLString, LOracleConnection);
                    LOracleDataAdapter.Fill(LClassReturn.DataSetReturn);
                    LOracleDataAdapter.Dispose();
                }
            }
            catch (Exception ex)
            {
                LClassReturn.BoolReturn = false;
                LClassReturn.StrReturn = "WCF003E999" + AscCodeToChr(27) + "SelectDataByDynamicSQL()\n" + ex.ToString();
            }
            finally
            {
                if (LSqlConnection != null)
                {
                    if (LSqlConnection.State == ConnectionState.Open) { LSqlConnection.Close(); }
                    LSqlConnection.Dispose(); LSqlConnection = null;
                }
                if (LOracleConnection != null)
                {
                    if (LOracleConnection.State == ConnectionState.Open) { LOracleConnection.Close(); }
                    LOracleConnection.Dispose(); LOracleConnection = null;
                }
            }

            return LClassReturn;
        }
        #endregion

        #region 执行SQL语句，返回受影响的行数
        private DatabaseOperationReturn ExecuteDynamicSQL(List<string> AlistStrDatabaseProfile, string AStrSQLString)
        {
            DatabaseOperationReturn LClassReturn = new DatabaseOperationReturn();

            string LStrConnectParam = string.Empty;
            SqlConnection LSqlConnection = null;
            SqlCommand LSqlCommand = null;
            OracleConnection LOracleConnection = null;
            OracleCommand LOracleCommand = null;
            string LStrDynamicSQL = string.Empty;
            int LIntExecureReturn = 0;

            try
            {
                if (AlistStrDatabaseProfile[0] == "2")
                {
                    LStrConnectParam = string.Format("Data Source={0},{1};Initial Catalog={2};User Id={3};Password={4}", AlistStrDatabaseProfile[1], AlistStrDatabaseProfile[2], AlistStrDatabaseProfile[5], AlistStrDatabaseProfile[3], AlistStrDatabaseProfile[4]);
                    LSqlConnection = new SqlConnection(LStrConnectParam);
                    LSqlConnection.Open();
                    LSqlCommand = new SqlCommand(AStrSQLString, LSqlConnection);
                    LIntExecureReturn = LSqlCommand.ExecuteNonQuery();
                }
                if (AlistStrDatabaseProfile[0] == "3")
                {
                    LStrConnectParam = string.Format("Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST={0}) (PORT={1})))(CONNECT_DATA=(SERVICE_NAME= {2})));User Id={3}; Password={4}", AlistStrDatabaseProfile[1], AlistStrDatabaseProfile[2], AlistStrDatabaseProfile[5], AlistStrDatabaseProfile[3], AlistStrDatabaseProfile[4]);
                    LOracleConnection = new OracleConnection(LStrConnectParam);
                    LOracleConnection.Open();
                    LOracleCommand = new OracleCommand(AStrSQLString, LOracleConnection);
                    LIntExecureReturn = LOracleCommand.ExecuteNonQuery();
                }
                LClassReturn.StrReturn = LIntExecureReturn.ToString();
            }
            catch (Exception ex)
            {
                LClassReturn.BoolReturn = false;
                LClassReturn.StrReturn = "WCF003E999" + AscCodeToChr(27) + "ExecuteDynamicSQL()\n" + ex.ToString();
            }
            finally
            {
                if (LSqlCommand != null) { LSqlCommand.Dispose(); LSqlCommand = null; }
                if (LOracleCommand != null) { LOracleCommand.Dispose(); LOracleCommand = null; }

                if (LSqlConnection != null)
                {
                    if (LSqlConnection.State == ConnectionState.Open) { LSqlConnection.Close(); }
                    LSqlConnection.Dispose(); LSqlConnection = null;
                }
                if (LOracleConnection != null)
                {
                    if (LOracleConnection.State == ConnectionState.Open) { LOracleConnection.Close(); }
                    LOracleConnection.Dispose(); LOracleConnection = null;
                }
            }

            return LClassReturn;
        }
        #endregion

        #region 生成分割符号 AscCodeToChr
        private string AscCodeToChr(int AsciiCode)
        {
            System.Text.ASCIIEncoding asciiEncoding = new System.Text.ASCIIEncoding();
            byte[] byteArray = new byte[] { (byte)AsciiCode };
            string strCharacter = asciiEncoding.GetString(byteArray);
            return (strCharacter);
        }
        #endregion

        #region 获取当前UMP安装的目录
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
        #endregion

        #region 创建加密、解密验证码
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
        #endregion

        #region 获取远程连接的IP（客户端IP）
        private string GetRemoteEndpointIPAddress()
        {
            string LStrRemoteIPAddress = string.Empty;
            try
            {
                OperationContext LOpContext = OperationContext.Current;
                MessageProperties LMessageProterty = LOpContext.IncomingMessageProperties;
                RemoteEndpointMessageProperty LRemoteEndpoint = LMessageProterty[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                LStrRemoteIPAddress = LRemoteEndpoint.Address.ToString();
            }
            catch { LStrRemoteIPAddress = string.Empty; }

            return LStrRemoteIPAddress;
        }
        #endregion

        #region 获取本机所有IP
        private string GetNetworkAllIPAddress()
        {
            string LStrAllIPAddress = string.Empty;

            try
            {
                LStrAllIPAddress = string.Empty;
                NetworkInterface[] LNetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
                foreach (NetworkInterface LNetworkInterfaceSingle in LNetworkInterfaces)
                {
                    IPInterfaceProperties LIPInterfaceProperty = LNetworkInterfaceSingle.GetIPProperties();
                    UnicastIPAddressInformationCollection LUnicastIPAddressInformationCollection = LIPInterfaceProperty.UnicastAddresses;
                    foreach (UnicastIPAddressInformation LUnicastIPAddressInformationSingle in LUnicastIPAddressInformationCollection)
                    {
                        if (LUnicastIPAddressInformationSingle.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            LStrAllIPAddress += LUnicastIPAddressInformationSingle.Address.ToString() + AscCodeToChr(27);
                        }
                    }
                }
            }
            catch { LStrAllIPAddress = string.Empty; }

            return LStrAllIPAddress;
        }
        #endregion

        #region 获取C:\ProgramData目录
        private string GetProgramDataDirectory()
        {
            string LStrProgramDataFolder = string.Empty;

            try
            {
                LStrProgramDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                if (!System.IO.Directory.Exists(System.IO.Path.Combine(LStrProgramDataFolder, "UMP.Server")))
                {
                    System.IO.Directory.CreateDirectory(System.IO.Path.Combine(LStrProgramDataFolder, "UMP.Server"));
                }
                if (!System.IO.Directory.Exists(System.IO.Path.Combine(LStrProgramDataFolder, "UMP.Server\\iTools")))
                {
                    System.IO.Directory.CreateDirectory(System.IO.Path.Combine(LStrProgramDataFolder, "UMP.Server\\iTools"));
                }
            }
            catch { LStrProgramDataFolder = @"C:\ProgramData"; }

            return LStrProgramDataFolder;
        }
        #endregion

        #region 创建或删除正在操作标志文件
        private void CreateOrDeleteTagFile(bool ABoolIsDelete)
        {
            if (ABoolIsDelete)
            {
                System.IO.File.Delete(System.IO.Path.Combine(GetProgramDataDirectory(), @"UMP.Server\iTools", "ClientConnecting"));
                System.Threading.Thread.Sleep(500);
            }
            else
            {
                while (System.IO.File.Exists(System.IO.Path.Combine(GetProgramDataDirectory(), @"UMP.Server\iTools", "ClientConnecting")))
                {
                    System.Threading.Thread.Sleep(1000);
                }
                System.IO.FileStream LFileStream = new System.IO.FileStream(System.IO.Path.Combine(GetProgramDataDirectory(), @"UMP.Server\iTools", "ClientConnecting"), System.IO.FileMode.Create, System.IO.FileAccess.ReadWrite);
                LFileStream.Close();
                System.Threading.Thread.Sleep(500);
            }
        }
        #endregion

    }

    public class DatabaseOperationReturn
    {
        public bool BoolReturn = true;
        public string StrReturn = string.Empty;
        public DataSet DataSetReturn = new DataSet();
    }
}
